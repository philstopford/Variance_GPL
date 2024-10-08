using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Clipper2Lib;
using EmailNS;
using Error;
using gds;
using geoCoreLib;
using geoWrangler;
using oasis;
using utility;
using Timer = System.Timers.Timer;

namespace Variance;

public partial class Entropy
{
    public delegate void forceImplantRepaint();
    public forceImplantRepaint forceImplantRepaintFunc { get; set; }

    public delegate void updateImplantSimUIMT();
    public updateSimUIMT updateImplantSimUIMTFunc { get; set; }

    public delegate void updateImplantSimUI(SimResultPackage implantResultPackage, string resultString);
    public updateSimUI updateImplantSimUIFunc { get; set; }

    public delegate void implantSimRunningUI();
    public simRunningUI implantSimRunningUIFunc { get; set; }

    private SimResultPackage implantResultPackage;

    public SimResultPackage getImplantResultPackage()
    {
        return pGetImplantResultPackage();
    }

    private SimResultPackage pGetImplantResultPackage()
    {
        return implantResultPackage;
    }

    private Sampler_Implant sampler_implant;

    private void setSampler_implant(int numberOfCases, bool previewMode)
    {
        sampler_implant = new Sampler_Implant(numberOfCases, previewMode, commonVars.getImplantSimulationSettings());
        string tmp = commonVars.getFriendly() ? Utils.friendlyNumber(sampler_implant.getDimensions() * numberOfCases) : (sampler_implant.getDimensions() * numberOfCases).ToString();
        updateStatus?.Invoke("Computing " + tmp + " samples.");
        sampler_implant.updateProgressBarFunc = uiProgressBarWrapper;
    }

    private void entropyRunCore_implant_singleThread(bool previewMode, int numberOfCases)
    {
        const int numberOfResultsFields = 1;
        // Let's sample this
        const int sampleRate = 100;

        setSampler_implant(numberOfCases, previewMode);
        sampler_implant.sample(false);
        sw.Start();

        implantResultPackage = new SimResultPackage(ref varianceContext.implantPreviewLock, numberOfCases, numberOfResultsFields);
        sw.Reset();
        for (int i = 0; i < numberOfCases; i++)
        {
            sw.Start();
            currentProgress = i + 1;
            try
            {
                // Get the results from the implant calc engine.
                ChaosSettings_implant cs = sampler_implant.getSample(i);
                implantResultPackage.Add(entropyEval_implant(cs));

                if (numberOfCases == 1 || currentProgress % sampleRate == 0)
                {
                    // Update the preview configuration.
                    if (implantResultPackage.getImplantResult(i).isValid())
                    {
                        if (numberOfCases > 1)
                        {
                            try
                            {
                                resultString = implantResultPackage.getMeanAndStdDev();
                            }
                            catch (Exception)
                            {
                                // Non-critical if an exception is raised. Ignore and carry on.
                            }
                        }
                        else
                        {
                            // Need to workaround some oddness here. The .ToString() calls below seemed to turn "0.0" into blanks.
                            string tmp = Convert.ToDouble(implantResultPackage.getImplantResult(i).getResult()).ToString("0.##");
                            resultString += tmp + ",";

                            tmp = Convert.ToDouble(implantResultPackage.getImplantResult(i).getMin()).ToString("0.##");
                            resultString += tmp + ",";

                            tmp = Convert.ToDouble(implantResultPackage.getImplantResult(i).getMax()).ToString("0.##");
                            resultString += tmp;
                        }
                        updateImplantSimUIFunc?.Invoke(false, implantResultPackage, resultString);
                    }
                    // Force redraw. We could use the progress bar to repaint, though.
                    // Note that this is an ugly hack to also ensure we collect stop button presses.
                    forceImplantRepaintFunc?.Invoke();
                }
            }
            catch (Exception)
            {
                // Reject the case - something happened.
            }
            // Nudge progress bar.
            if (numberOfCases > 1)
            {
                timeOfFlight_implant(sw.Elapsed.TotalSeconds);
                stepProgress?.Invoke();
            }

            // Check if user cancelled.
            if (abortRunFunc == null)
            {
                continue;
            }

            abortRunFunc();
            if (!commonVars.runAbort)
            {
                continue;
            }

            sw.Stop();
            implantResultPackage.setState(false);
            commonVars.runAbort = false; // reset state to allow user to abort save of results.
            break;
        }

        implantResultPackage.setRunTime(sw.Elapsed.TotalSeconds);
        sw.Stop();
    }

    private void entropyRunCore_implant_multipleThread(bool previewMode, int numberOfCases)
    {
        setSampler_implant(numberOfCases, previewMode);
        sampler.sample(true, false);

        const int numberOfResultsFields = 1;

        implantResultPackage = new SimResultPackage(ref varianceContext.implantPreviewLock, numberOfCases, numberOfResultsFields);

        multithreadWarningFunc?.Invoke();

        // Set up timers for the UI refresh
        commonVars.m_timer = new Timer {AutoReset = true, Interval = CentralProperties.timer_interval};
        updateImplantSimUIMTFunc?.Invoke();
        commonVars.m_timer.Start();

        // Set our parallel task options based on user settings.
        ParallelOptions po = new();
        // Attempt at parallelism.
        CancellationTokenSource cancelSource = new();
        CancellationToken cancellationToken = cancelSource.Token;
        switch (varianceContext.numberOfThreads)
        {
            case -1:
                po.MaxDegreeOfParallelism = commonVars.HTCount;
                break;
            default:
                po.MaxDegreeOfParallelism = varianceContext.numberOfThreads;
                break;
        }

        if (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.greedy) == 0)
        {
            if (po.MaxDegreeOfParallelism > 1) // avoid setting to 0
            {
                po.MaxDegreeOfParallelism -= 1;
            }
        }

        try
        {
            Parallel.For(0, numberOfCases, po, (i, loopState) =>
                {
                    try
                    {
                        ChaosSettings_implant cs = sampler_implant.getSample(i);
                        Results_implant currentResult = entropyEval_implant(cs);

                        if (currentResult.isValid()) // only update if result is valid.
                        {
                            try
                            {
                                {
                                    if (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.external) == 1 && baseFileName != "")
                                    {
                                        switch (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType))
                                        {
                                            case (int)CommonVars.external_Type.svg:
                                                writeSVG(currentResult, i, numberOfCases);
                                                break;
                                            default:
                                                writeLayout_implant(currentResult, i, numberOfCases, commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType));
                                                break;
                                        }
                                    }
                                    implantResultPackage.Add(currentResult);
                                }
                            }
                            catch (Exception)
                            {
                            }

                        }
                        Interlocked.Increment(ref currentProgress);

                        forceImplantRepaintFunc?.Invoke();

                        abortRunFuncMT?.Invoke(implantResultPackage, cancelSource, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        implantResultPackage.setState(false);
                        // ReSharper disable once AccessToDisposedClosure
                        commonVars.m_timer.Stop();
                        commonVars.runAbort = false; // reset state to allow user to abort save of results.
                        sw.Stop();
                        loopState.Stop();
                    }
                }
            );
        }
        catch (OperationCanceledException)
        {
            implantResultPackage.setState(false);
            commonVars.runAbort = false; // reset state to allow user to abort save of results.
            commonVars.m_timer.Stop();
            sw.Stop();
        }
        catch (AggregateException)
        {
            commonVars.m_timer.Stop();
            sw.Stop();
        }
        catch (Exception)
        {
        }
        commonVars.m_timer.Stop();
        commonVars.m_timer.Dispose();
        implantResultPackage.setRunTime(swTime);
        sw.Stop();
        sw.Reset();
    }

    private Results_implant entropyEval_implant(ChaosSettings_implant currentJobSettings)
    {
        // UI handler has already applied 'accuracy' to the resolution and angular resolution values.

        ChaosEngine_implant currentJobEngine = new(currentJobSettings, commonVars.getImplantSimulationSettings(), commonVars.getImplantSettings());

        Results_implant evalResults = new();

        if (!currentJobEngine.isValid())
        {
            return evalResults;
        }

        evalResults.setResult(currentJobEngine.getResult());
        evalResults.setMin(currentJobEngine.getMin());
        evalResults.setMax(currentJobEngine.getMax());
        evalResults.resistWidthVar = currentJobSettings.getValue(ChaosSettings_implant.Properties.resistCDVar);
        evalResults.resistHeightVar = currentJobSettings.getValue(ChaosSettings_implant.Properties.resistHeightVar);
        evalResults.resistCRRVar = currentJobSettings.getValue(ChaosSettings_implant.Properties.resistTopCRRVar);
        evalResults.tiltVar = currentJobSettings.getValue(ChaosSettings_implant.Properties.tiltVar);
        evalResults.twistVar = currentJobSettings.getValue(ChaosSettings_implant.Properties.twistVar);
        evalResults.setResistShapes(commonVars.getColors(), currentJobEngine.getGeom(), currentJobEngine.getBGGeom(), currentJobEngine.getShadow(), currentJobEngine.getMinShadow(), currentJobEngine.getMaxShadow());
        evalResults.setValid(currentJobEngine.isValid());
        return evalResults;
    }

    public void entropyRun_implant(int numberOfCases, string csvFile, bool useThreads)
    {
        pEntropyRun_implant(numberOfCases, csvFile, useThreads);
    }

    private void pEntropyRun_implant(int numberOfCases, string csvFile, bool useThreads)
    {
        reset();
        clearAbortFlagFunc?.Invoke();
        string emailString;

        bool simState = entropyRunCore(numberOfCases, row: 0, col: 0, csvFile, useThreads, tileHandling: false, implantMode: true, doPASearch: false);
        switch (simState)
        {
            case false:
                emailString = "Implant run aborted";
                break;
            default:
                emailString = "Implant run completed";
                break;
        }
        // Assume user error if perJob is set and not onCompletion.
        // We'll use simulationSettings here just because legacy put the settings there and it's the easiest option.
        if (commonVars.getNonSimulationSettings().emailOnCompletion && numberOfCases > 1)
        {
            try
            {
                Email.Send(commonVars.getNonSimulationSettings().host, commonVars.getNonSimulationSettings().port,
                    commonVars.getNonSimulationSettings().ssl, emailString, lastSimResultsOverview,
                    commonVars.getNonSimulationSettings().emailAddress, commonVars.getNonSimulationSettings().emailPwd);
            }
            catch (Exception e)
            {
                ErrorReporter.showMessage_OK(e.Message, "Error sending mail");
            }
        }

        if (numberOfCases > 1)
        {
            implantResultPackage.getMeanAndStdDev();
            updateStatus?.Invoke(lastSimResultsOverview);
        }

        postSimUIFunc?.Invoke();
    }

    public void timeOfFlight_implant(double swTime_)
    {
        pTimeOfFlight_implant(swTime_);
    }

    private void pTimeOfFlight_implant(double swTime_)
    {
        // Update status bar with elapsed and predicted time.
        try
        {
            if (swTime_ == 0.0)
            {
                return;
            }

            int numberOfCases = commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases);

            TimeSpan eT = TimeSpan.FromSeconds(swTime_);
            string statusLineString = eT.Seconds + " s elapsed";
            if (eT.Minutes >= 1)
            {
                // We have minutes
                statusLineString = eT.Minutes + " m, " + statusLineString;
            }
            if (eT.Hours >= 1)
            {
                // We have hours.
                statusLineString = eT.Hours + " h, " + statusLineString;
            }

            if (currentProgress != numberOfCases)
            {
                double completionTime = swTime_ / currentProgress * numberOfCases - currentProgress;
                TimeSpan cT = TimeSpan.FromSeconds(completionTime);
                statusLineString += "; ";

                if (cT.Hours >= 1)
                {
                    // We have hours.
                    statusLineString += cT.Hours + " h";
                }
                if (cT.Minutes >= 1)
                {
                    if (cT.Hours >= 1)
                    {
                        statusLineString += ", ";
                    }
                    // We have minutes
                    statusLineString += cT.Minutes + " m";
                }
                if (!(cT.Minutes < 1 && cT.Hours < 1))
                {
                    statusLineString += ", ";
                }
                statusLineString += cT.Seconds + " s remaining";
            }

            string tmp;
            if (commonVars.getFriendly())
            {
                tmp = Utils.friendlyNumber(currentProgress) + "/" + Utils.friendlyNumber(numberOfCases);
            }
            else
            {
                tmp = currentProgress + "/" + numberOfCases;
            }

            statusLineString += " => (" + tmp + ") complete";
            updateStatus?.Invoke(statusLineString);
        }
        catch (Exception)
        {
            // We don't care - this is a UI update call; non-critical.
        }
    }

    private bool saveResults_implant()
    {
        string csvFileName = baseFileName;
        csvFileName += ".csv";

        bool returnValue = false;

        int doneCases = implantResultPackage.getListOfResults_implant().Count;

        int rows = doneCases + 1;
        string[] stringList = new string[rows];

        configProgress?.Invoke(0, doneCases);
        string headerString = "Run,Implant Shadowing";
        headerString += ",";
        headerString += "CDUVar" + ",";
        headerString += "HeightVar" + ",";
        headerString += "CRRVar" + ",";
        headerString += "tiltAngleVar" + ",";
        headerString += "twistAngleVar";
        stringList[0] = headerString;
        commonVars.runAbort = false;

        try
        {
            summaryFile_implant(csvFileName);
        }
        catch (Exception)
        {
            // MessageBox.Show(ex.Message);
        }

        string statusLine = "No CSV or external files written per job settings.All done.";

        if (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1)
        {
            // Set our parallel task options based on user settings.
            // Attempt at parallelism.
            CancellationTokenSource cancelSource = new();
            CancellationToken cancellationToken = cancelSource.Token;

            string statusString = "Compiling CSV file for results";
            updateStatus?.Invoke(statusString);
            int counter = 0;
            Parallel.For(0, doneCases, (resultEntry, loopState) =>
            {
                try
                {
                    if (implantResultPackage.getImplantResult(resultEntry).isValid())
                    {
                        if (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1)
                        {
                            // result can be a CSV string for multiple values - header is aligned above
                            string csvString = resultEntry + "," + implantResultPackage.getImplantResult(resultEntry).getResult();
                            csvString += ",";
                            csvString += implantResultPackage.getImplantResult(resultEntry).resistWidthVar + ",";
                            csvString += implantResultPackage.getImplantResult(resultEntry).resistHeightVar + ",";
                            csvString += implantResultPackage.getImplantResult(resultEntry).resistCRRVar + ",";
                            csvString += implantResultPackage.getImplantResult(resultEntry).tiltVar + ",";
                            csvString += implantResultPackage.getImplantResult(resultEntry).twistVar.ToString(CultureInfo.InvariantCulture);
                            stringList[resultEntry + 1] = csvString;
                        }
                    }

                    Interlocked.Increment(ref counter);
                    abortCSVFunc?.Invoke(cancelSource, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    commonVars.runAbort = false;
                    commonVars.cancelling = false;
                    loopState.Stop();
                }
                catch (Exception)
                {
                }
            });

            File.WriteAllLines(csvFileName, stringList);
            statusLine = counter + " results saved to CSV file";
        }

        if (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.external) == 1)
        {
            if (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1)
            {
                statusLine += " and";
            }
            statusLine += " shapes saved to ";

            statusLine += commonVars.getExternalTypes()[commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType)] + " files";
        }

        updateStatus?.Invoke(statusLine);
        forceRepaintFunc?.Invoke();
        return returnValue;
    }

    private void writeSVG(Results_implant currentResult, int resultEntry, int numberOfCases)
    {
        string svgFileName = baseFileName;

        string paddingString = "D" + numberOfCases.ToString().Length; // count chars in the number of cases as a string, use that to define padding.
        svgFileName += "_run" + resultEntry.ToString(paddingString) + ".svg";

        SvgWriter svg = new();
        
        // Active resist contour
        svg.AddClosedPaths(currentResult.getResistShapes()[0].getPoints(),
            (uint)currentResult.getResistShapes()[0].getColor().toArgb(),
            (uint)currentResult.getResistShapes()[0].getColor().toArgb(),
            1.0, false);

        // Background resist contour
        svg.AddClosedPaths(currentResult.getResistShapes()[1].getPoints(),
            (uint)currentResult.getResistShapes()[1].getColor().toArgb(),
            (uint)currentResult.getResistShapes()[1].getColor().toArgb(),
            1.0, false);

        // Shadow
        svg.AddClosedPaths(currentResult.getLine(Results_implant.Lines.shadow).getPoints(),
            (uint)currentResult.getLine(Results_implant.Lines.shadow).getColor().toArgb(),
            (uint)currentResult.getLine(Results_implant.Lines.shadow).getColor().toArgb(),
            1.0, false);

        svg.SaveToFile(svgFileName);
    }

    private void writeLayout_implant(Results_implant currentResult, int resultEntry, int numberOfCases, int type)
    {
        string layoutFileName = baseFileName;

        string paddingString = "D" + numberOfCases.ToString().Length; // count chars in the number of cases as a string, use that to define padding.
        layoutFileName += "_run" + resultEntry.ToString(paddingString);

        int scale = 100; // for 0.01 nm resolution
        GeoCore g = new();
        g.reset();
        GCDrawingfield drawing_ = new("")
        {
            accyear = (short) DateTime.Now.Year,
            accmonth = (short) DateTime.Now.Month,
            accday = (short) DateTime.Now.Day,
            acchour = (short) DateTime.Now.Hour,
            accmin = (short) DateTime.Now.Minute,
            accsec = (short) DateTime.Now.Second,
            modyear = (short) DateTime.Now.Year,
            modmonth = (short) DateTime.Now.Month,
            modday = (short) DateTime.Now.Day,
            modhour = (short) DateTime.Now.Hour,
            modmin = (short) DateTime.Now.Minute,
            modsec = (short) DateTime.Now.Second,
            databaseunits = 1000 * scale,
            userunits = 0.001 / scale,
            libname = "variance"
        };

        GCCell gcell_root = drawing_.addCell();
        gcell_root.accyear = (short)DateTime.Now.Year;
        gcell_root.accmonth = (short)DateTime.Now.Month;
        gcell_root.accday = (short)DateTime.Now.Day;
        gcell_root.acchour = (short)DateTime.Now.Hour;
        gcell_root.accmin = (short)DateTime.Now.Minute;
        gcell_root.accsec = (short)DateTime.Now.Second;
        gcell_root.modyear = (short)DateTime.Now.Year;
        gcell_root.modmonth = (short)DateTime.Now.Month;
        gcell_root.modday = (short)DateTime.Now.Day;
        gcell_root.modhour = (short)DateTime.Now.Hour;
        gcell_root.modmin = (short)DateTime.Now.Minute;
        gcell_root.modsec = (short)DateTime.Now.Second;
        gcell_root.cellName = "implantCase" + resultEntry;

        // Resist
        for (int i = 0; i < 2; i++)
        {
            PathsD resistPolys = currentResult.getResistShapes()[i].getPoints();
            g.addLayerName("L" + (i + 1) + "D0", "resistPolys" + i);

            foreach (Path64 ePoly in resistPolys.Select(t => GeoWrangler.resize_to_int(t, scale)))
            {
                gcell_root.addPolygon(ePoly, i + 1, 0);
            }
        }

        // Shadowing line
        PathsD shadowLine = currentResult.getLine(Results_implant.Lines.shadow).getPoints();
        g.addLayerName("L2D0", "shadowLine");
        foreach (Path64 ePoly in shadowLine.Select(t => GeoWrangler.resize_to_int(t, scale)))
        {
            gcell_root.addPolygon(ePoly, 2, 0);
        }

        g.setDrawing(drawing_);
        g.setValid(true);

        switch (type)
        {
            case (int)CommonVars.external_Type.gds:
                gdsWriter gw = new(g, layoutFileName + ".gds");
                gw.save();
                break;
            case (int)CommonVars.external_Type.oas:
                oasWriter ow = new(g, layoutFileName + ".oas");
                ow.save();
                break;

        }
    }

    private void summaryFile_implant(string csvFileName)
    {
        // Force the evaluation.
        implantResultPackage.getMeanAndStdDev();
        // Create our summaryFile string, assuming it is needed.
        // This was simpler before, but now user can avoid CSV creation and will instead select the summary output filename (txt)
        string summaryFile_ = csvFileName;
        if (!summaryFile_.EndsWith("_summary.txt", StringComparison.CurrentCulture))
        {
            summaryFile_ = csvFileName.Substring(0, csvFileName.Length - 4);
            summaryFile_ += "_summary.txt";
        }

        List<string> linesToWrite = new()
        {
            "Results summary for job: " + csvFileName + " run on : " +
            DateTime.Today.ToLocalTime().ToLongDateString() + ". Runtime: " +
            implantResultPackage.runTime.ToString("0.##") + " seconds"
        };

        // Results first
        // Write the results first.
        if (Debugger.IsAttached)
        {
            linesToWrite.Add("Run under debugger : performance was lower");
        }

        linesToWrite.Add("");
        for (int resultGroup = 0; resultGroup < implantResultPackage.getValues(SimResultPackage.Properties.mean).Length; resultGroup++)
        {
            linesToWrite.Add("result " + resultGroup + " mean and standard deviation for " + implantResultPackage.getListOfResults_implant().Count + " cases: x: " + implantResultPackage.getValue(SimResultPackage.Properties.mean, resultGroup).ToString("0.##") + ", s: " + implantResultPackage.getValue(SimResultPackage.Properties.stdDev, resultGroup).ToString("0.##"));
        }

        linesToWrite.Add("");

        linesToWrite.AddRange(implantResultPackage.getHistograms());

        // Simulation Settings
        linesToWrite.Add("");
        linesToWrite.Add("Implant settings for job");
        linesToWrite.Add("");
        linesToWrite.Add("Resist Width: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.w));
        linesToWrite.Add("Resist CDU: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.wV));
        linesToWrite.Add("Resist Height: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.h));
        linesToWrite.Add("Resist Height Var: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.hV));
        linesToWrite.Add("Resist Top CRR: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.cRR));
        linesToWrite.Add("Resist Top CRR Var: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.cV));
        linesToWrite.Add("");
        linesToWrite.Add("Implant Tilt Angle: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.tilt));
        linesToWrite.Add("Implant Tilt Angle Var: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.tiltV));
        linesToWrite.Add("Implant Twist Angle: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.twist));
        linesToWrite.Add("Implant Twist Angle Var: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.twistV));

        // Simulation Settings
        linesToWrite.Add("");
        commonVars.getSimulationSettings_implant(linesToWrite);

        try
        {
            File.WriteAllLines(summaryFile_, linesToWrite);
        }
        catch (Exception ex)
        {
            ErrorReporter.showMessage_OK(ex.ToString(), "Exception");
        }
    }
}