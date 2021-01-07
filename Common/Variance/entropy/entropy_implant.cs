using Error;
using geoCoreLib;
using geoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using utility;

namespace Variance
{
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

        SimResultPackage implantResultPackage;

        public SimResultPackage getImplantResultPackage()
        {
            return pGetImplantResultPackage();
        }

        SimResultPackage pGetImplantResultPackage()
        {
            return implantResultPackage;
        }

        Sampler_Implant sampler_implant;

        void setSampler_implant(int numberOfCases, bool previewMode)
        {
            sampler_implant = new Sampler_Implant(numberOfCases, previewMode, commonVars.getImplantSimulationSettings());
            string tmp;
            if (commonVars.getFriendly())
            {
                tmp = Utils.friendlyNumber(sampler_implant.getDimensions() * numberOfCases);
            }
            else
            {
                tmp = (sampler_implant.getDimensions() * numberOfCases).ToString();
            }
            updateStatus?.Invoke("Computing " + tmp + " samples.");
            sampler_implant.updateProgressBarFunc = uiProgressBarWrapper;
        }

        void entropyRunCore_implant_singleThread(bool previewMode, Int32 numberOfCases)
        {
            Int32 numberOfResultsFields = 1;
            // Let's sample this
            Int32 sampleRate = 100;

            setSampler_implant(numberOfCases, previewMode);
            sampler_implant.sample(false);
            sw.Start();

            implantResultPackage = new SimResultPackage(ref varianceContext.implantPreviewLock, numberOfCases, numberOfResultsFields);
            sw.Reset();
            for (Int32 i = 0; i < numberOfCases; i++)
            {
                sw.Start();
                currentProgress = i + 1;
                try
                {
                    // Get the results from the implant calc engine.
                    ChaosSettings_implant cs = sampler_implant.getSample(i);
                    implantResultPackage.Add(entropyEval_implant(previewMode, cs));

                    if ((numberOfCases == 1) || (currentProgress % sampleRate == 0))
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
                    // MessageBox.Show("MCRun() had an issue, aborting and saving results so far: " + MCRE.ToString(), "Oops", MessageBoxButtons.OK);
                    // Reject the case - something happened.
                }
                // Nudge progress bar.
                if (numberOfCases > 1)
                {
                    timeOfFlight_implant(sw.Elapsed.TotalSeconds);
                    stepProgress?.Invoke();
                }

                // Check if user cancelled.
                if (abortRunFunc != null)
                {
                    abortRunFunc();
                    if (commonVars.runAbort)
                    {
                        sw.Stop();
                        implantResultPackage.setState(false);
                        commonVars.runAbort = false; // reset state to allow user to abort save of results.
                        break;
                    }
                }
            }

            implantResultPackage.setRunTime(sw.Elapsed.TotalSeconds);
            sw.Stop();
        }

        void entropyRunCore_implant_multipleThread(bool previewMode, Int32 numberOfCases)
        {
            setSampler_implant(numberOfCases, previewMode);
            sampler.sample(true);

            Int32 numberOfResultsFields = 1;

            implantResultPackage = new SimResultPackage(ref varianceContext.implantPreviewLock, numberOfCases, numberOfResultsFields);

            multithreadWarningFunc?.Invoke();

            commonVars.m_timer = new System.Timers.Timer();
            // Set up timers for the UI refresh
            commonVars.m_timer.AutoReset = true;
            commonVars.m_timer.Interval = CentralProperties.timer_interval;
            updateImplantSimUIMTFunc?.Invoke();
            commonVars.m_timer.Start();

            // Set our parallel task options based on user settings.
            ParallelOptions po = new ParallelOptions();
            // Attempt at parallelism.
            CancellationTokenSource cancelSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelSource.Token;
            if (varianceContext.numberOfThreads == -1)
            {
                po.MaxDegreeOfParallelism = commonVars.HTCount;
            }
            else
            {
                po.MaxDegreeOfParallelism = varianceContext.numberOfThreads;
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
                    Results_implant currentResult = new Results_implant();
                    try
                    {
                        ChaosSettings_implant cs = sampler_implant.getSample(i);
                        currentResult = entropyEval_implant(previewMode, cs);

                        if (currentResult.isValid()) // only update if result is valid.
                        {
                            try
                            {
                                {
                                    if ((commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.external) == 1) && (baseFileName != ""))
                                    {
                                        switch (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType))
                                        {
                                            case (Int32)CommonVars.external_Type.svg:
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
                            catch (Exception ex)
                            {
                                string oops = ex.Message;
                            }

                        }
                        Interlocked.Increment(ref currentProgress);

                        forceImplantRepaintFunc?.Invoke();

                        abortRunFuncMT?.Invoke(implantResultPackage, cancelSource, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        implantResultPackage.setState(false);
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

        Results_implant entropyEval_implant(bool previewMode, ChaosSettings_implant currentJobSettings)
        {
            // UI handler has already applied 'accuracy' to the resolution and angular resolution values.

            ChaosEngine_implant currentJobEngine = new ChaosEngine_implant(currentJobSettings, commonVars.getImplantSimulationSettings(), commonVars.getImplantSettings());

            Results_implant evalResults = new Results_implant();

            if (currentJobEngine.isValid())
            {
                evalResults.setResult(currentJobEngine.getResult());
                evalResults.setMin(currentJobEngine.getMin());
                evalResults.setMax(currentJobEngine.getMax());
                evalResults.resistWidthVar = currentJobSettings.getValue(ChaosSettings_implant.properties.resistCDVar);
                evalResults.resistHeightVar = currentJobSettings.getValue(ChaosSettings_implant.properties.resistHeightVar);
                evalResults.resistCRRVar = currentJobSettings.getValue(ChaosSettings_implant.properties.resistTopCRRVar);
                evalResults.tiltVar = currentJobSettings.getValue(ChaosSettings_implant.properties.tiltVar);
                evalResults.twistVar = currentJobSettings.getValue(ChaosSettings_implant.properties.twistVar);
                evalResults.setResistShapes(commonVars.getColors(), currentJobEngine.getGeom(), currentJobEngine.getBGGeom(), currentJobEngine.getShadow(), currentJobEngine.getMinShadow(), currentJobEngine.getMaxShadow());
                evalResults.setValid(currentJobEngine.isValid());
            }
            return evalResults;
        }

        public void entropyRun_implant(Int32 numberOfCases, string csvFile, bool useThreads)
        {
            pEntropyRun_implant(numberOfCases, csvFile, useThreads);
        }

        void pEntropyRun_implant(Int32 numberOfCases, string csvFile, bool useThreads)
        {
            reset();
            clearAbortFlagFunc?.Invoke();
            bool simState = true;
            string emailString = "";

            simState = entropyRunCore(numberOfCases, row: 0, col: 0, csvFile, useThreads, tileHandling: false, implantMode: true, doPASearch: false);
            if (!simState)
            {
                emailString = "Implant run aborted";
            }
            else
            {
                emailString = "Implant run completed";
            }
            // Assume user error if perJob is set and not onCompletion.
            // We'll use simulationSettings here just because legacy put the settings there and it's the easiest option.
            if (commonVars.getNonSimulationSettings().emailOnCompletion && (numberOfCases > 1))
            {
                Email.Send(commonVars.getNonSimulationSettings().host, commonVars.getNonSimulationSettings().port, commonVars.getNonSimulationSettings().ssl, emailString, lastSimResultsOverview, commonVars.getNonSimulationSettings().emailAddress, commonVars.getNonSimulationSettings().emailPwd);
            }

            if (numberOfCases > 1)
            {
                implantResultPackage.getMeanAndStdDev();
                updateStatus?.Invoke(lastSimResultsOverview);
            }

            postSimUIFunc?.Invoke();
        }

        public void timeOfFlight_implant(double swTime)
        {
            pTimeOfFlight_implant(swTime);
        }

        void pTimeOfFlight_implant(double swTime)
        {
            // Update status bar with elapsed and predicted time.
            try
            {
                if (swTime != 0.0)
                {
                    int numberOfCases = commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases);

                    TimeSpan eT = TimeSpan.FromSeconds(swTime);
                    string statusLineString = eT.Seconds.ToString() + " s elapsed";
                    if (eT.Minutes >= 1)
                    {
                        // We have minutes
                        statusLineString = eT.Minutes.ToString() + " m, " + statusLineString;
                    }
                    if (eT.Hours >= 1)
                    {
                        // We have hours.
                        statusLineString = eT.Hours.ToString() + " h, " + statusLineString;
                    }

                    if (currentProgress != numberOfCases)
                    {
                        double completionTime = ((swTime / currentProgress) * numberOfCases - currentProgress);
                        TimeSpan cT = TimeSpan.FromSeconds(completionTime);
                        statusLineString += "; ";

                        if (cT.Hours >= 1)
                        {
                            // We have hours.
                            statusLineString += cT.Hours.ToString() + " h";
                        }
                        if (cT.Minutes >= 1)
                        {
                            if (cT.Hours >= 1)
                            {
                                statusLineString += ", ";
                            }
                            // We have minutes
                            statusLineString += cT.Minutes.ToString() + " m";
                        }
                        if (!((cT.Minutes < 1) && (cT.Hours < 1)))
                        {
                            statusLineString += ", ";
                        }
                        statusLineString += cT.Seconds.ToString() + " s remaining";
                    }

                    string tmp;
                    if (commonVars.getFriendly())
                    {
                        tmp = Utils.friendlyNumber(currentProgress) + "/" + Utils.friendlyNumber(numberOfCases);
                    }
                    else
                    {
                        tmp = currentProgress.ToString() + "/" + numberOfCases.ToString();
                    }

                    statusLineString += " => (" + tmp + ") complete";
                    updateStatus?.Invoke(statusLineString);
                }
            }
            catch (Exception)
            {
                // We don't care - this is a UI update call; non-critical.
            }
        }

        bool saveResults_implant(Int32 numberOfCases)
        {
            string csvFileName = baseFileName;
            csvFileName += ".csv";

            bool returnValue = false;

            Int32 doneCases = implantResultPackage.getListOfResults_implant().Count();

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
                ParallelOptions po = new ParallelOptions();
                // Attempt at parallelism.
                CancellationTokenSource cancelSource = new CancellationTokenSource();
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
                                string csvString = resultEntry.ToString() + "," + implantResultPackage.getImplantResult(resultEntry).getResult();
                                csvString += ",";
                                csvString += implantResultPackage.getImplantResult(resultEntry).resistWidthVar.ToString() + ",";
                                csvString += implantResultPackage.getImplantResult(resultEntry).resistHeightVar.ToString() + ",";
                                csvString += implantResultPackage.getImplantResult(resultEntry).resistCRRVar.ToString() + ",";
                                csvString += implantResultPackage.getImplantResult(resultEntry).tiltVar.ToString() + ",";
                                csvString += implantResultPackage.getImplantResult(resultEntry).twistVar.ToString();
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
                statusLine = counter.ToString() + " results saved to CSV file";
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

        void writeSVG(Results_implant currentResult, int resultEntry, int numberOfCases)
        {
            string svgFileName = baseFileName;

            string paddingString = "D" + numberOfCases.ToString().Length.ToString(); // count chars in the number of cases as a string, use that to define padding.
            svgFileName += "_run" + resultEntry.ToString(paddingString) + ".svg";

            SVGBuilder svg = new SVGBuilder();

            // Active resist contour
            svg.style.brushClr = currentResult.getResistShapes()[0].getColor();
            svg.style.penClr = currentResult.getResistShapes()[0].getColor();
            svg.AddPolygons(currentResult.getResistShapes()[0].getPoints());

            // Background resist contour
            svg.style.brushClr = currentResult.getResistShapes()[1].getColor();
            svg.style.penClr = currentResult.getResistShapes()[1].getColor();
            svg.AddPolygons(currentResult.getResistShapes()[1].getPoints());

            // Shadow
            svg.style.brushClr = currentResult.getLine(Results_implant.lines.shadow).getColor();
            svg.style.penClr = currentResult.getLine(Results_implant.lines.shadow).getColor();
            svg.AddPolygons(currentResult.getLine(Results_implant.lines.shadow).getPoints());

            svg.SaveToFile(svgFileName);
        }

        void writeLayout_implant(Results_implant currentResult, int resultEntry, int numberOfCases, int type)
        {
            string layoutFileName = baseFileName;

            string paddingString = "D" + numberOfCases.ToString().Length.ToString(); // count chars in the number of cases as a string, use that to define padding.
            layoutFileName += "_run" + resultEntry.ToString(paddingString);

            int scale = 100; // for 0.01 nm resolution
            GeoCore g = new GeoCore();
            g.reset();
            GCDrawingfield drawing_ = new GCDrawingfield("");
            drawing_.accyear = (short)DateTime.Now.Year;
            drawing_.accmonth = (short)DateTime.Now.Month;
            drawing_.accday = (short)DateTime.Now.Day;
            drawing_.acchour = (short)DateTime.Now.Hour;
            drawing_.accmin = (short)DateTime.Now.Minute;
            drawing_.accsec = (short)DateTime.Now.Second;
            drawing_.modyear = (short)DateTime.Now.Year;
            drawing_.modmonth = (short)DateTime.Now.Month;
            drawing_.modday = (short)DateTime.Now.Day;
            drawing_.modhour = (short)DateTime.Now.Hour;
            drawing_.modmin = (short)DateTime.Now.Minute;
            drawing_.modsec = (short)DateTime.Now.Second;
            drawing_.databaseunits = 1000 * scale;
            drawing_.userunits = 0.001 / scale;
            drawing_.libname = "variance";

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
            gcell_root.cellName = "implantCase" + resultEntry.ToString();

            // Resist
            for (int i = 0; i < 2; i++)
            {
                List<GeoLibPointF[]> resistPolys = currentResult.getResistShapes()[i].getPoints();
                g.addLayerName("L" + (i + 1).ToString() + "D0", "resistPolys" + i.ToString());

                for (int poly = 0; poly < resistPolys.Count; poly++)
                {
                    GeoLibPoint[] ePoly = geoWrangler.GeoWrangler.resize_to_int(resistPolys[poly], scale);

                    gcell_root.addPolygon(ePoly.ToArray(), i + 1, 0);
                }
            }

            // Shadowing line
            List<GeoLibPointF[]> shadowLine = currentResult.getLine(Results_implant.lines.shadow).getPoints();
            g.addLayerName("L2D0", "shadowLine");
            for (int poly = 0; poly < shadowLine.Count; poly++)
            {
                GeoLibPoint[] ePoly = geoWrangler.GeoWrangler.resize_to_int(shadowLine[poly], scale);

                gcell_root.addPolygon(ePoly.ToArray(), 2, 0);
            }

            g.setDrawing(drawing_);
            g.setValid(true);

            switch (type)
            {
                case (int)CommonVars.external_Type.gds:
                    gds.gdsWriter gw = new gds.gdsWriter(g, layoutFileName + ".gds");
                    gw.save();
                    break;
                case (int)CommonVars.external_Type.oas:
                    oasis.oasWriter ow = new oasis.oasWriter(g, layoutFileName + ".oas");
                    ow.save();
                    break;

            }
        }

        void summaryFile_implant(string csvFileName)
        {
            // Force the evaluation.
            implantResultPackage.getMeanAndStdDev();
            // Create our summaryFile string, assuming it is needed.
            // This was simplier before, but now user can avoid CSV creation and will instead select the summary output filename (txt)
            string summaryFile_ = csvFileName;
            if (!summaryFile_.EndsWith("_summary.txt", StringComparison.CurrentCulture))
            {
                summaryFile_ = csvFileName.Substring(0, csvFileName.Length - 4);
                summaryFile_ += "_summary.txt";
            }
            List<string> linesToWrite = new List<string>();

            // Results first
            // Write the results first.
            linesToWrite.Add("Results summary for job: " + csvFileName + " run on : " + System.DateTime.Today.ToLocalTime().ToLongDateString() + ". Runtime: " + implantResultPackage.runTime.ToString("0.##") + " seconds");
            if (Debugger.IsAttached == true)
            {
                linesToWrite.Add("Run under debugger : performance was lower");
            }

            linesToWrite.Add("");
            for (int resultGroup = 0; resultGroup < implantResultPackage.getValues(SimResultPackage.properties.mean).Length; resultGroup++)
            {
                linesToWrite.Add("result " + resultGroup.ToString() + " mean and standard deviation for " + implantResultPackage.getListOfResults_implant().Count().ToString() + " cases: x: " + implantResultPackage.getValue(SimResultPackage.properties.mean, resultGroup).ToString("0.##") + ", s: " + implantResultPackage.getValue(SimResultPackage.properties.stdDev, resultGroup).ToString("0.##"));
            }

            linesToWrite.Add("");

            linesToWrite.AddRange(implantResultPackage.getHistograms());

            // Simulation Settings
            linesToWrite.Add("");
            linesToWrite.Add("Implant settings for job");
            linesToWrite.Add("");
            linesToWrite.Add("Resist Width: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.w).ToString());
            linesToWrite.Add("Resist CDU: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.wV).ToString());
            linesToWrite.Add("Resist Height: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.h).ToString());
            linesToWrite.Add("Resist Height Var: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.hV).ToString());
            linesToWrite.Add("Resist Top CRR: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.cRR).ToString());
            linesToWrite.Add("Resist Top CRR Var: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.cV).ToString());
            linesToWrite.Add("");
            linesToWrite.Add("Implant Tilt Angle: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.tilt).ToString());
            linesToWrite.Add("Implant Tilt Angle Var: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.tiltV).ToString());
            linesToWrite.Add("Implant Twist Angle: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.twist).ToString());
            linesToWrite.Add("Implant Twist Angle Var: " + commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.twistV).ToString());

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
}