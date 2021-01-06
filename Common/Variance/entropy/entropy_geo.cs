using ClipperLib; // for tile extraction
using Error;
using geoCoreLib;
using geoLib;
using geoWrangler;
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
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public partial class Entropy
    {
        public delegate void updateSimUI(bool doPASearch, SimResultPackage resultPackage, string resultString);
        public updateSimUI updateSimUIFunc { get; set; }

        public delegate void updateSimUIMT();
        public updateSimUIMT updateSimUIMTFunc { get; set; }

        public delegate void updateProgressBar(double val);
        public updateProgressBar updateProgressBarFunc { get; set; }

        public delegate bool tooManyPolysWarning();
        public tooManyPolysWarning tooManyPolysWarningFunc { get; set; }

        SimResultPackage resultPackage;

        public SimResultPackage getResultPackage()
        {
            return pGetResultPackage();
        }

        SimResultPackage pGetResultPackage()
        {
            return resultPackage;
        }

        bool useLoadedSettings;
        bool nonGaussianInput;
        ChaosSettings loadedJobSettings_; // used in replay mode to store job settings.
        int replayRow, replayCol;

        Sampler_Geo sampler;

        void preFlight(Int32 row, Int32 col, bool tileHandling)
        {
            // Do tile extraction here - it's a problem to do it later since we cause major runtime impact.
            if (tileHandling)
            {
                doTileExtraction(col, row);
            }
            lastSimResultsOverview = "";
            simJustDone = false;
            currentProgress = 0;
            multiCaseSim = false;
            swTime = 0.0; // reset time for the batch
            timeOfLastPreviewUpdate = 0;
            sw = new Stopwatch();
            sw.Stop();
            sw.Reset();
            sw_Preview = new Stopwatch();
            sw_Preview.Stop();
            sw_Preview.Reset();
        }

        void uiProgressBarWrapper(double val)
        {
            updateProgressBarFunc?.Invoke(val);
        }

        void setSampler(int numberOfCases, bool previewMode, bool doPASearch)
        {
            sampler = new Sampler_Geo(numberOfCases, previewMode, doPASearch, ref commonVars);
            string tmp;
            if (commonVars.getFriendly())
            {
                tmp = Utils.friendlyNumber(sampler.getDimensions() * CentralProperties.maxLayersForMC * numberOfCases);
            }
            else
            {
                tmp = (sampler.getDimensions() * CentralProperties.maxLayersForMC * numberOfCases).ToString();
            }
            updateStatus?.Invoke("Computing " + tmp + " samples.");
            sampler.updateProgressBarFunc = uiProgressBarWrapper;
        }

        void entropyRunCore_singleThread(bool previewMode, Int32 numberOfCases, Int32 row, Int32 col, bool tileHandling, bool doPASearch)
        {
            if (!doPASearch)
            {
                setSampler(numberOfCases, previewMode, doPASearch);
            }
            else
            {
                setSampler(1, previewMode, doPASearch);
            }
            sampler.sample(false);

            sw.Start();

            // Let's sample this for non PA search and non-preview mode runs.
            Int32 sampleRate = 100;

            // Set up our cleavedResults based on the max number of results we expect:
            Int32 numberOfResultsFields = 1;
            switch (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType))
            {
                case (int)CommonVars.calcModes.chord: // chord
                    numberOfResultsFields = 4;
                    break;
                default: // area, angle, spacing
                    numberOfResultsFields = 1;
                    break;
            }

            if (doPASearch)
            {
                resultPackage = new SimResultPackage(ref varianceContext.previewLock, commonVars.getPASearch().numberofPassCases, numberOfResultsFields);
            }
            else
            {
                resultPackage = new SimResultPackage(ref varianceContext.previewLock, numberOfCases, numberOfResultsFields);
            }

            // Set our input state in case of custom RNG mapping.
            resultPackage.nonGaussianInput = nonGaussianInput;
            sw.Reset();

            int generateExternal = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.external);
            if (doPASearch)
            {
                generateExternal = 0; // no SVG storage in PA search mode.
            }

            for (Int32 i = 0; i < numberOfCases; i++)
            {
                sw.Start();
                if (!doPASearch)
                {
                    currentProgress++;
                }
                try
                {
                    // Get the results from the MC engine.
                    ChaosSettings cs = sampler.getSample(i);
                    Results currentResult = EntropyEval(previewMode, doPASearch, row, col, cs);

                    bool addResultToPackage = true;
                    if (doPASearch)
                    {
                        // Review results and check against any limits from the PA search.
                        string[] tempString = currentResult.getResult().Split(csvSeparator);
                        for (int r = 0; r < tempString.Length; r++)
                        {
                            if (commonVars.getPASearch().useFilter[r])
                            {
                                if (tempString[r] != "N/A")
                                {
                                    double compareValue = Convert.ToDouble(tempString[r]);
                                    // Now we need to review the value against the limit values.
                                    // We have to pay attention to the min/max limit type
                                    if (commonVars.getPASearch().filterIsMaxValue[r])
                                    {
                                        if (compareValue > commonVars.getPASearch().filterValues[r])
                                        {
                                            addResultToPackage = false;
                                        }
                                    }
                                    else
                                    {
                                        if (compareValue < commonVars.getPASearch().filterValues[r])
                                        {
                                            addResultToPackage = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (addResultToPackage)
                    {
                        if (doPASearch)
                        {
                            currentProgress++;
                            stepProgress?.Invoke();
                        }
                        resultPackage.Add(currentResult, generateExternal);
                        if (doPASearch && (resultPackage.getListOfResults().Count == commonVars.getPASearch().numberofPassCases))
                        {
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    // PASearch - update with every match, at least for now.
                    if (doPASearch || (previewMode || (currentProgress % sampleRate == 0)))
                    {
                        int index = i;
                        if (doPASearch)
                        {
                            index = resultPackage.getListOfResults().Count - 1;
                        }
                        // Update the preview configuration.
                        if (resultPackage.getResult(index).isValid())
                        {
                            if (numberOfCases > 1)
                            {
                                try
                                {
                                    resultString = resultPackage.getMeanAndStdDev();
                                }
                                catch (Exception)
                                {
                                    // Non-critical if an exception is raised. Ignore and carry on.
                                }
                            }
                            else
                            {
                                resultString = "";
                                string[] tempString = resultPackage.getResult(index).getResult().Split(csvSeparator);
                                for (Int32 j = 0; j < tempString.Length; j++)
                                {
                                    if (j > 0)
                                    {
                                        resultString += ",";
                                    }
                                    if (tempString[j] == "N/A")
                                    {
                                        resultString += "N/A";
                                    }
                                    else
                                    {
                                        resultString += Convert.ToDouble(tempString[j]).ToString("0.##");
                                    }
                                }
                            }
                            updateSimUIFunc?.Invoke(doPASearch, resultPackage, resultString);
                        }
                        // Force redraw. We could use the progress bar to repaint, though.
                        // Note that this is an ugly hack to also ensure we collect stop button presses.

                        forceRepaintFunc?.Invoke();
                    }
                }
                catch (Exception)
                {
                    // Reject the case - something happened.
                }

                if (numberOfCases > 1)
                {
                    timeOfFlight(sw.Elapsed.TotalSeconds, doPASearch);
                    // Nudge progress bar for non PA search mode.
                    if (!doPASearch)
                    {
                        stepProgress?.Invoke();
                    }
                }

                // Check if user cancelled.
                if (abortRunFunc != null)
                {
                    abortRunFunc();
                    if (commonVars.runAbort)
                    {
                        sw.Stop();
                        resultPackage.setState(false);
                        commonVars.runAbort = false; // reset state to allow user to abort save of results.
                        break;
                    }
                }
            }

            resultPackage.setRunTime(sw.Elapsed.TotalSeconds);
            sw.Stop();
        }

        void entropyRunCore_multipleThread(bool previewMode, Int32 numberOfCases, Int32 row, Int32 col, bool tileHandling, bool doPASearch)
        {
            if (!doPASearch)
            {
                setSampler(numberOfCases, previewMode, doPASearch);
            }
            else
            {
                setSampler(1, previewMode, doPASearch);
            }
            sampler.sample(true);

            sw.Start();

            // Set up our cleavedResults based on the max number of results we expect:
            Int32 numberOfResultsFields = 1;
            switch (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType))
            {
                case (int)CommonVars.calcModes.chord: // chord
                    numberOfResultsFields = 4;
                    break;
                default: // area, angle, spacing
                    numberOfResultsFields = 1;
                    break;
            }

            if (doPASearch)
            {
                resultPackage = new SimResultPackage(ref varianceContext.previewLock, commonVars.getPASearch().numberofPassCases, numberOfResultsFields);
            }
            else
            {
                resultPackage = new SimResultPackage(ref varianceContext.previewLock, numberOfCases, numberOfResultsFields);
                multithreadWarningFunc?.Invoke();
            }

            // Set our input state in case of custom RNG mapping.
            resultPackage.nonGaussianInput = nonGaussianInput;

            commonVars.m_timer = new System.Timers.Timer();
            // Set up timers for the UI refresh
            commonVars.m_timer.AutoReset = true;
            commonVars.m_timer.Interval = CentralProperties.timer_interval;
            updateSimUIMTFunc?.Invoke();
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

            if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.greedy) == 0)
            {
                if (po.MaxDegreeOfParallelism > 1) // avoid setting to 0
                {
                    po.MaxDegreeOfParallelism -= 1;
                }
            }

            int generateExternal = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.external);
            if (doPASearch)
            {
                generateExternal = 0; // no SVG for the PA search mode.
            }

            try
            {
                Parallel.For(0, numberOfCases, po, (i, loopState) =>
                {
                    Results currentResult = new Results();
                    try
                    {
                        ChaosSettings cs = sampler.getSample(i);
                        currentResult = EntropyEval(previewMode, doPASearch, row, col, cs);

                        string[] tempString = currentResult.getResult().Split(csvSeparator);

                        bool addResultToPackage = true;
                        if (doPASearch)
                        {
                            // Review results and check against any limits from the PA search.
                            for (int r = 0; r < tempString.Length; r++)
                            {
                                if (tempString[r] != "N/A")
                                {
                                    double compareValue = Convert.ToDouble(tempString[r]);
                                    // Now we need to review the value against the limit values.
                                    if (commonVars.getPASearch().useFilter[r])
                                    {
                                        // We have to pay attention to the min/max limit type
                                        if (commonVars.getPASearch().filterIsMaxValue[r])
                                        {
                                            if (compareValue > commonVars.getPASearch().filterValues[r])
                                            {
                                                addResultToPackage = false;
                                            }
                                        }
                                        else
                                        {
                                            if (compareValue < commonVars.getPASearch().filterValues[r])
                                            {
                                                addResultToPackage = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (addResultToPackage && currentResult.isValid()) // only update if result is valid.
                        {
                            try
                            {
                                if (!doPASearch)
                                {
                                    if ((generateExternal == 1) && (baseFileName != ""))
                                    {
                                        bool doExternal = true;
                                        switch (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCriteria))
                                        {
                                            case 0:
                                                break;
                                            default:
                                                // Is first result being filtered?
                                                int rf = 0;
                                                decimal compVal = commonVars.getSimulationSettings_nonSim().getDecimal(EntropySettings_nonSim.properties_d.externalCritCond1);
                                                switch (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCritCond1))
                                                {
                                                    case (int)CommonVars.external_Filter.gte:
                                                        if (tempString[rf] != "N/A")
                                                        {
                                                            doExternal = Convert.ToDecimal(tempString[rf]) >= compVal;
                                                        }
                                                        break;
                                                    case (int)CommonVars.external_Filter.lte:
                                                        if (tempString[rf] != "N/A")
                                                        {
                                                            doExternal = Convert.ToDecimal(tempString[rf]) <= compVal;
                                                        }
                                                        break;
                                                }
                                                // Multi-result check.
                                                if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.chord)
                                                {
                                                    // Is second result being filtered?
                                                    rf = 1;
                                                    compVal = commonVars.getSimulationSettings_nonSim().getDecimal(EntropySettings_nonSim.properties_d.externalCritCond2);
                                                    switch (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCritCond2))
                                                    {
                                                        case (int)CommonVars.external_Filter.gte:
                                                            if (tempString[rf] != "N/A")
                                                            {
                                                                doExternal = doExternal && Convert.ToDecimal(tempString[1]) >= compVal;
                                                            }
                                                            break;
                                                        case (int)CommonVars.external_Filter.lte:
                                                            if (tempString[rf] != "N/A")
                                                            {
                                                                doExternal = doExternal && Convert.ToDecimal(tempString[1]) <= compVal;
                                                            }
                                                            break;
                                                    }

                                                    // Is third result being filtered?
                                                    rf = 2;
                                                    compVal = commonVars.getSimulationSettings_nonSim().getDecimal(EntropySettings_nonSim.properties_d.externalCritCond3);
                                                    switch (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCritCond3))
                                                    {
                                                        case (int)CommonVars.external_Filter.gte:
                                                            if (tempString[rf] != "N/A")
                                                            {
                                                                doExternal = doExternal && Convert.ToDecimal(tempString[1]) >= compVal;
                                                            }
                                                            break;
                                                        case (int)CommonVars.external_Filter.lte:
                                                            if (tempString[rf] != "N/A")
                                                            {
                                                                doExternal = doExternal && Convert.ToDecimal(tempString[1]) <= compVal;
                                                            }
                                                            break;
                                                    }

                                                    // Is fourth result being filtered?
                                                    rf = 3;
                                                    compVal = commonVars.getSimulationSettings_nonSim().getDecimal(EntropySettings_nonSim.properties_d.externalCritCond4);
                                                    switch (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCritCond4))
                                                    {
                                                        case (int)CommonVars.external_Filter.gte:
                                                            if (tempString[rf] != "N/A")
                                                            {
                                                                doExternal = doExternal && Convert.ToDecimal(tempString[1]) >= compVal;
                                                            }
                                                            break;
                                                        case (int)CommonVars.external_Filter.lte:
                                                            if (tempString[rf] != "N/A")
                                                            {
                                                                doExternal = doExternal && Convert.ToDecimal(tempString[1]) <= compVal;
                                                            }
                                                            break;
                                                    }
                                                }
                                                break;
                                        }
                                        if (doExternal)
                                        {
                                            switch (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType))
                                            {
                                                case (Int32)CommonVars.external_Type.svg:
                                                    writeSVG(currentResult, i, numberOfCases, tileHandling, col, row);
                                                    break;
                                                default:
                                                    writeLayout(currentResult, i, numberOfCases, commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType), tileHandling, col, row);
                                                    break;
                                            }
                                        }
                                    }
                                }
                                // resultPackage has its own locking.
                                resultPackage.Add(currentResult, 0); // disable retention of geometry given new write-during-run methodology.
                            }
                            catch (Exception ex)
                            {
                                string oops = ex.Message;
                            }

                        }

                        if ((!doPASearch) || (doPASearch && addResultToPackage))
                        {
                            Interlocked.Increment(ref currentProgress);
                            if (doPASearch && Equals(resultPackage.getListOfResults().Count, commonVars.getPASearch().numberofPassCases))
                            {
                                loopState.Stop();
                            }
                        }

                        forceRepaintFunc?.Invoke();

                        abortRunFuncMT?.Invoke(resultPackage, cancelSource, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        resultPackage.setState(false);
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
                resultPackage.setState(false);
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
            resultPackage.setRunTime(swTime);
            sw.Stop();
            sw.Reset();
        }

        void doTileExtraction(Int32 col, Int32 row)
        {
            double tileXOffset = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
            double tileXSize = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colPitch);
            double tileYOffset = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
            double tileYSize = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowPitch);
            Path tileCutter = new Path();
            Int64 tileLeftX = (Int64)(tileXOffset + (col * tileXSize)) * CentralProperties.scaleFactorForOperation;
            Int64 tileRightX = (Int64)(tileLeftX + (tileXSize * CentralProperties.scaleFactorForOperation));
            Int64 tileBottomY = (Int64)(tileYOffset + (row * tileYSize)) * CentralProperties.scaleFactorForOperation;
            Int64 tileTopY = (Int64)(tileBottomY + (tileYSize * CentralProperties.scaleFactorForOperation));
            tileCutter.Add(new IntPoint(tileLeftX, tileBottomY));
            tileCutter.Add(new IntPoint(tileLeftX, tileTopY));
            tileCutter.Add(new IntPoint(tileRightX, tileTopY));
            tileCutter.Add(new IntPoint(tileRightX, tileBottomY));
            tileCutter.Add(new IntPoint(tileLeftX, tileBottomY));

            for (Int32 layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                List<string> polyHashCodes = new List<string>();
                if (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(layer) == 1)
                {
                    Paths result = new Paths();
                    List<GeoLibPointF[]> extractedPolys = new List<GeoLibPointF[]>();
                    // Get our layout for clipping.
                    Paths layout = new Paths(); // Use Paths since we have polygon islands to wrangle.
                                                // fileData contains all of our polygons
                    for (Int32 poly = 0; poly < commonVars.getLayerSettings(layer).getFileData().Count(); poly++)
                    {
                        Path polyPath = GeoWrangler.pathFromPointF(commonVars.getLayerSettings(layer).getFileData()[poly], CentralProperties.scaleFactorForOperation);
                        string polyHash = Utils.GetMD5Hash(polyPath);
                        if (polyHashCodes.IndexOf(polyHash) == -1)
                        {
                            // Hash not found - assuming unique polygon. This is done to avoid impact of copy/paste fails in layout where
                            // totally coincident duplicate polygons cause the extracted tile to be empty.
                            // This avoids full overlaps causing complications in the Union used below to resolve partial overlaps.
                            layout.Add(polyPath.ToList());
                            polyHashCodes.Add(polyHash);
                        }
                    }

                    // We now use a Union() to merge any partially overlapping polys.
                    Clipper c = new Clipper();
                    if (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
                    {
                        c.PreserveCollinear = true; // important - if we don't do this, we lose the fragmentation on straight edges.
                    }
                    else
                    {
                        c.PreserveCollinear = false; // since we'll be using the contour engine, we can simplify our geometry.
                    }
                    c.AddPaths(layout, PolyType.ptSubject, true);
                    c.Execute(ClipType.ctUnion, result, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
                    layout = result.ToList();
                    result.Clear();

                    // Carve out our tile
                    c = new Clipper();
                    if (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
                    {
                        c.PreserveCollinear = true; // important - if we don't do this, we lose the fragmentation on straight edges.
                    }
                    else
                    {
                        c.PreserveCollinear = false; // since we'll be using the contour engine, we can simplify our geometry.
                    }
                    c.AddPath(tileCutter, PolyType.ptClip, true);
                    c.AddPaths(layout, PolyType.ptSubject, true);
                    c.Execute(ClipType.ctIntersection, result);

                    // Need to map output and downscale to floating points again.
                    // We might have more than one polygon in the result, so be careful.
                    double xCompensation = (double)tileLeftX / CentralProperties.scaleFactorForOperation;
                    double yCompensation = (double)tileBottomY / CentralProperties.scaleFactorForOperation;

                    for (Int32 poly = 0; poly < result.Count(); poly++)
                    {
                        GeoLibPointF[] tempPoly = GeoWrangler.pointFFromPath(result[poly], CentralProperties.scaleFactorForOperation);
                        // move back to origin from tile location
                        tempPoly = GeoWrangler.move(tempPoly, -xCompensation, -yCompensation);
                        extractedPolys.Add(tempPoly);
                    }

                    commonVars.getNonSimulationSettings().extractedTile[layer] = extractedPolys.ToList();
                }
            }
        }

        public void timeOfFlight(double swTime, bool doPASearch)
        {
            pTimeOfFlight(swTime, doPASearch);
        }

        void pTimeOfFlight(double swTime, bool doPASearch)
        {
            // Update status bar with elapsed and predicted time.
            try
            {
                if (swTime != 0.0)
                {
                    Int32 numberOfCases = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases);
                    if (doPASearch)
                    {
                        numberOfCases = commonVars.getPASearch().numberofPassCases;
                    }
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
                        double completionTime = ((swTime / currentProgress) * (numberOfCases - currentProgress));
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

        bool saveResults(Int32 numberOfCases, bool tileHandling, Int32 col, Int32 row)
        {
            string csvFileName = baseFileName;
            if (tileHandling)
            {
                csvFileName += "_col" + col.ToString() + "_row" + row.ToString();
            }

            csvFileName += ".csv";

            bool returnValue = false;
            Int32 doneCases = resultPackage.getListOfResults().Count();
            int rows = doneCases + 4;
            string[] stringList = new string[rows];

            configProgress?.Invoke(0, doneCases);
            // Create header for CSV output, if applicable
            string headerString = "Run,";
            if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.area) // area output
            {
                if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.areaCalcModes.all)
                {
                    headerString += "Total Area";
                }
                else
                {
                    headerString += "Minimum Area";
                }
            }
            if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.enclosure_spacing_overlap) // spacing output
            {
                switch (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode))
                {
                    case (int)CommonVars.spacingCalcModes.spacing:
                    case (int)CommonVars.spacingCalcModes.spacingOld:
                        headerString += "Spacing";
                        break;
                    case (int)CommonVars.spacingCalcModes.enclosure:
                    case (int)CommonVars.spacingCalcModes.enclosureOld:
                        headerString += "Enclosure";
                        break;
                }
            }
            if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.chord) // chord output
            {
                headerString += "AMinTopChord,AMinBottomChord,BMinLeftChord,BMinRightChord";
            }
            if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.angle) // angle output
            {
                headerString += "MinIntersectionAngle";
            }
            for (Int32 layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                if (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                {
                    headerString += ",";
                    for (int i = 0; i < CommonVars.csvHeader.Length; i++)
                    {
                        headerString += CommonVars.csvHeader[i] + layer.ToString();
                        if (i != CommonVars.csvHeader.Length - 1)
                        {
                            headerString += ",";
                        }
                    }
                }
            }
            stringList[0] = headerString;
            commonVars.runAbort = false;


            try
            {
                summaryFile(tileHandling, col, row);
            }
            catch (Exception)
            {
                // MessageBox.Show(ex.Message);
            }

            string statusLine = "No CSV or external files written per job settings. All done.";

            if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1)
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
                        if (resultPackage.getResult(resultEntry).isValid())
                        {
                            if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1)
                            {
                                // result can be a CSV string for multiple values - header is aligned above
                                string csvString = resultEntry.ToString() + "," + resultPackage.getResult(resultEntry).getResult();
                                for (Int32 layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
                                {
                                    if (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                                    {
                                        csvString += ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.svar, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.tvar, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.lwr, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getSeed(Results.fields_i.lwrs, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.lwr2, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getSeed(Results.fields_i.lwr2s, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.htip, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.vtip, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.icv, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.ocv, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.olx, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.oly, layer).ToString() + ",";
                                        csvString += resultPackage.getResult(resultEntry).getField(Results.fields_d.wob, layer).ToString();
                                    }
                                }
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

                    // stepProgress?.Invoke();
                    // directProgress?.Invoke(resultEntry, doneCases);
                });
                // We need to warn here if the project shows a changed state, because replay will be affected.
                if (commonVars.isChanged())
                {
                    ErrorReporter.showMessage_OK("Project should be saved to ensure replay can be used with this CSV file.", "Warning");
                }

                // Append hashes to the stringList to allow replay to warn against mismatch.
                // Ensure the hashes are current.
                string[] oldHashes = commonVars.getHashes();
                commonVars.setHashes();
                stringList[doneCases + 1] = "lHash," + commonVars.getHash(CommonVars.hashes.settings);
                stringList[doneCases + 2] = "eHash," + commonVars.getHash(CommonVars.hashes.entropy);
                stringList[doneCases + 3] = "gHash," + commonVars.getHash(CommonVars.hashes.gc);
                commonVars.setHashes(oldHashes);

                File.WriteAllLines(csvFileName, stringList);

                string tmp;
                if (commonVars.getFriendly())
                {
                    tmp = Utils.friendlyNumber(counter);
                }
                else
                {
                    tmp = counter.ToString();
                }
                statusLine = tmp + " results saved to CSV file";
            }

            if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.external) == 1)
            {
                if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1)
                {
                    statusLine += " and shapes ";
                }
                else
                {
                    statusLine = "Shapes ";
                }

                statusLine += "saved to " + commonVars.getExternalTypes()[commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType)] + " files";
            }

            updateStatus?.Invoke(statusLine);
            forceRepaintFunc?.Invoke();
            return returnValue;
        }

        void writeSVG(Results currentResult, int resultEntry, int numberOfCases, bool tileHandling, int col, int row)
        {
            string svgFileName = baseFileName;
            if (tileHandling)
            {
                svgFileName += "_col" + col.ToString() + "_row" + row.ToString();
            }

            string paddingString = "D" + numberOfCases.ToString().Length.ToString(); // count chars in the number of cases as a string, use that to define padding.
            svgFileName += "_run" + resultEntry.ToString(paddingString) + ".svg";

            SVGBuilder svg = new SVGBuilder();

            // Inputs
            for (Int32 previewShapeIndex = 0; previewShapeIndex < currentResult.getPreviewShapes().Count(); previewShapeIndex++)
            {
                svg.style.brushClr = commonVars.getColors().simPreviewColors[previewShapeIndex]; // Color.FromArgb(0x10, 0, 0, 0x9c);
                svg.style.penClr = commonVars.getColors().simPreviewColors[previewShapeIndex]; //Color.FromArgb(0xd3, 0xd3, 0xda);
                                                                                               // Overloaded operator can handle List<PointF[]>
                svg.AddPolygons(currentResult.getPreviewShapes()[previewShapeIndex]);
            }

            // Result
            for (Int32 poly = 0; poly < currentResult.getPoints().Count(); poly++)
            {
                svg.style.brushClr = commonVars.getColors().resultColors[poly]; // Color.FromArgb(0x10, 0x9c, 0, 0);
                svg.style.penClr = commonVars.getColors().resultColors[poly]; // Color.FromArgb(0xff, 0xa0, 0x7a);
                svg.AddPolygons(currentResult.getPoints()[poly]); // results from MC evaluation
            }

            svg.SaveToFile(svgFileName);
        }

        void writeLayout(Results currentResult, int resultEntry, int numberOfCases, int type, bool tileHandling, int col, int row)
        {
            string layoutFileName = baseFileName;

            if (tileHandling)
            {
                layoutFileName += "_col" + col.ToString() + "_row" + row.ToString();
            }

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
            gcell_root.cellName = "result" + resultEntry.ToString();

            List<Int32> layers = new List<int>();
            // Input shapes.
            for (Int32 previewShapeIndex = 0; previewShapeIndex < currentResult.getPreviewShapes().Count(); previewShapeIndex++)
            {
                // Get the geometry for the shape
                List<GeoLibPointF[]> polys = currentResult.getPreviewShapes()[previewShapeIndex];

                // Find our source layer value that we need to write the geometry into the correct geoCore layer.
                int layerIndex = currentResult.getLayerIndex(previewShapeIndex);
                if (layers.IndexOf(layerIndex) == -1)
                {
                    layers.Add(layerIndex);
                    // Register layer names with geoCore. Need to compensate the 1-index for the layer registration.
                    g.addLayerName("L" + (layerIndex + 1).ToString() + "D0", commonVars.getLayerSettings(layerIndex).getString(EntropyLayerSettings.properties_s.name));
                }

                for (int poly = 0; poly < polys.Count; poly++)
                {
                    int polyLength = polys[poly].Length;
                    if (polyLength > 2)
                    {
                        GeoLibPoint[] ePoly = GeoWrangler.resize_to_int(polys[poly], scale);

                        gcell_root.addPolygon(ePoly.ToArray(), layerIndex + 1, 0); // layer is 1-index based for output, so need to offset value accordingly.
                    }
                }
            }

            // Get the geometry for the result
            List<GeoLibPointF[]> rpolys = currentResult.getPoints();

            for (int poly = 0; poly < rpolys.Count; poly++)
            {
                int layerNum = CentralProperties.maxLayersForMC + poly + 1;
                g.addLayerName("L" + layerNum.ToString() + "D0", "result" + poly.ToString());
                int polyLength = rpolys[poly].Length;
                if (polyLength > 2)
                {
                    GeoLibPoint[] ePoly = GeoWrangler.resize_to_int(rpolys[poly], scale);

                    gcell_root.addPolygon(ePoly.ToArray(), layerNum, 0); // layer is 1-index based for output, so need to offset value accordingly.
                }
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

        void summaryFile(bool tileHandling, int col, int row)
        {
            string internalFileName = baseFileName; // (remove the .csv from the string)
            if (tileHandling)
            {
                internalFileName += "_col" + col.ToString() + "_row" + row.ToString();
            }

            // Force an evaluation.
            resultPackage.getMeanAndStdDev();
            // Create our summaryFile string, assuming it is needed.
            // This was simplier before, but now user can avoid CSV creation and will instead select the summary output filename (txt)
            string summaryFile_ = internalFileName + "_summary.txt";
            string csvFileName = internalFileName + ".csv";
            List<string> linesToWrite = new List<string>();

            // Results first
            // Write the results first.
            linesToWrite.Add(commonVars.titleText);
            linesToWrite.Add("Results summary for job: " + csvFileName + " run on : " + System.DateTime.Today.ToLocalTime().ToLongDateString() + ". Runtime: " + resultPackage.runTime.ToString("0.##") + " seconds");
            if (Debugger.IsAttached == true)
            {
                linesToWrite.Add("Run under debugger : performance was lower");
            }

            linesToWrite.Add("");
            commonVars.getBooleanEquation(linesToWrite);

            linesToWrite.Add("");
            for (int resultGroup = 0; resultGroup < resultPackage.getValues(SimResultPackage.properties.mean).Length; resultGroup++)
            {
                string tempString = "result " + resultGroup.ToString() + " mean";
                if (!resultPackage.nonGaussianInput)
                {
                    tempString += " and standard deviation";
                }
                tempString += " for " + resultPackage.getListOfResults().Count().ToString() + " cases:";
                tempString += " x: " + resultPackage.getValue(SimResultPackage.properties.mean, resultGroup).ToString("0.##");
                if (!resultPackage.nonGaussianInput)
                {
                    tempString += ", s: " + resultPackage.getValue(SimResultPackage.properties.stdDev, resultGroup).ToString("0.##");
                }
                else
                {
                    tempString += ". Non-Gaussian inputs require offline analysis of CSV file.";
                }
                linesToWrite.Add(tempString);
            }

            linesToWrite.Add("");

            try
            {
                linesToWrite.AddRange(resultPackage.getHistograms());
            }
            catch (Exception)
            {
                // Histogram can fail in case of insufficient variation - i.e. all values are the same.
            }

            // Simulation Settings
            linesToWrite.Add("");
            linesToWrite.Add("Layer settings for job");
            linesToWrite.Add("");
            for (int layer = 0; layer < commonVars.getListOfSettings().Count(); layer++)
            {
                commonVars.getLayerSettings(layer, ref linesToWrite, onlyActive: true);
            }

            commonVars.getSimulationSettings(linesToWrite);
            commonVars.getDOESettings(linesToWrite);

            try
            {
                File.WriteAllLines(summaryFile_, linesToWrite);
            }
            catch (Exception ex)
            {
                ErrorReporter.showMessage_OK(ex.ToString(), "Exception");
            }
        }

        public void EntropyRun(Int32 numberOfCases, string csvFile, bool useThreads, bool doPASearch, bool setJobSettings = false, ChaosSettings loadedJobSettings = null, int replayRow = 0, int replayCol = 0) // event handler will call this with 1 and null as the csvFile
        {
            this.replayCol = replayCol;
            this.replayRow = replayRow;

            useLoadedSettings = setJobSettings;

            // Replay mode
            if (useLoadedSettings)
            {
                loadedJobSettings_ = loadedJobSettings;
                numberOfCases = 1;
                csvFile = null;
            }
            pEntropyRun(numberOfCases, csvFile, useThreads, doPASearch);
        }

        void pEntropyRun(Int32 numberOfCases, string csvFile, bool useThreads, bool doPASearch)
        {
            reset();
            // Apply PA search values to settings.
            // This mode doesn't support replay, so checksum is not important.
            if (doPASearch)
            {
                commonVars.getPASearch().applySearchValues(ref commonVars);
                numberOfCases = Int32.MaxValue; // we're going to run until we find the number of pass cases the user requested.
            }

            // Check for non-Gaussian inputs and set the flag.
            nonGaussianInput = commonVars.nonGaussianInputs();

            // Warn user if the setup could be problematic.
            if ((numberOfCases > 1) && !doPASearch && nonGaussianInput && (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 0))
            {
                ErrorReporter.showMessage_OK("Non-Gaussian inputs with no CSV requested!\r\nNon-Gaussian inputs require offline analysis of simulation results, through CSV", "Warning");
            }

            string emailString = "";
            clearAbortFlagFunc?.Invoke();
            bool simState = true;
            bool tileHandling = false;
            bool listOfTiles = false;
            Int32 listOfTilesCount = 0;
            Int32 startRow = 0;
            Int32 endRow = 1;
            Int32 startCol = 0;
            Int32 endCol = 1;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(i) == 1)
                {
                    tileHandling = true;
                    break;
                }
            }
            if (tileHandling)
            {
                // We have some tiles to run.
                if (commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.uTileList) == 1)
                {
                    listOfTiles = true;
                    if (numberOfCases == 1)
                    {
                        // preview mode
                        listOfTilesCount = 1;
                    }
                    else
                    {
                        listOfTilesCount = commonVars.getSimulationSettings().getDOESettings().getTileList_ColRow().Count();
                    }
                }
                else
                {
                    if (commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.sTile) == 0)
                    {
                        if (csvFile != null) // avoid doing something stupid for a preview call.
                        {
                            // We have more than one tile to run.
                            endCol = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.cols);
                            endRow = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.rows);
                        }
                    }
                    else
                    {
                        // Convert from UI 1-based to 0-based indexing.
                        startCol = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.sTileCol) - 1;
                        startRow = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.sTileRow) - 1;
                        endCol = startCol + 1;
                        endRow = startRow + 1;
                    }
                }
            }

            Int32 row = 0;
            Int32 col = 0;

            if (useLoadedSettings)
            {
                row = replayRow;
                col = replayCol;
            }

            if (!tileHandling)
            {
                try
                {
                    simState = entropyRunCore(numberOfCases, row, col, csvFile, useThreads, tileHandling, implantMode: false, doPASearch);
                    if (!simState)
                    {
                        emailString = "Variance run aborted : " + commonVars.projectFileName;
                    }
                    else
                    {
                        emailString = "Variance run completed : " + commonVars.projectFileName;
                    }
                }
                catch (Exception)
                {
                    // Don't care about exceptions - we handle them internally.
                }
                // Assume user error if perJob is set and not onCompletion.
                if ((commonVars.getNonSimulationSettings().emailOnCompletion || commonVars.getNonSimulationSettings().emailPerJob) && (numberOfCases > 1))
                {
                    Email.Send(commonVars.getNonSimulationSettings().host, commonVars.getNonSimulationSettings().port, commonVars.getNonSimulationSettings().ssl, emailString, lastSimResultsOverview, commonVars.getNonSimulationSettings().emailAddress, commonVars.getNonSimulationSettings().emailPwd);
                }
            }
            else
            {
                if (listOfTiles)
                {
                    for (Int32 tile = 0; tile < listOfTilesCount; tile++)
                    {
                        try
                        {
                            int tileRow = commonVars.getSimulationSettings().getDOESettings().getTileList_Value(tile, 1);
                            int tileCol = commonVars.getSimulationSettings().getDOESettings().getTileList_Value(tile, 0);
                            simState = entropyRunCore(numberOfCases, tileRow, tileCol, csvFile, useThreads, tileHandling, implantMode: false, doPASearch);
                            if (!simState)
                            {
                                if (abortAllRunsFunc != null)
                                {
                                    bool really = true;
                                    // Only ask to abort all runs if we have multiple runs and more runs pending.
                                    if ((listOfTilesCount > 1) && (tile != listOfTilesCount - 1))
                                    {
                                        really = abortAllRunsFunc();
                                    }
                                    if (really)
                                    {
                                        listOfTilesCount = tile;
                                        // Email notification
                                        if (commonVars.getNonSimulationSettings().emailPerJob)
                                        {
                                            emailString = "Variance run aborted for tile " + tileCol.ToString() + "," + tileRow.ToString();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                emailString = "Variance run completed for tile " + tileCol.ToString() + "," + tileRow.ToString();
                            }
                        }
                        catch (Exception)
                        {
                            // Don't care about exceptions - we handle them internally.
                        }
                        if (commonVars.getNonSimulationSettings().emailPerJob)
                        {
                            Email.Send(commonVars.getNonSimulationSettings().host, commonVars.getNonSimulationSettings().port, commonVars.getNonSimulationSettings().ssl, emailString, lastSimResultsOverview, commonVars.getNonSimulationSettings().emailAddress, commonVars.getNonSimulationSettings().emailPwd);
                        }
                    }
                }
                else
                {
                    row = startRow;
                    col = startCol;
                    bool ending = false;
                    while (row < endRow)
                    {
                        while (col < endCol)
                        {
                            try
                            {
                                simState = entropyRunCore(numberOfCases, row, col, csvFile, useThreads, tileHandling, implantMode: false, doPASearch);
                                if (!simState)
                                {
                                    if (abortAllRunsFunc != null)
                                    {
                                        bool really = true;
                                        // Only ask to abort all runs if we have more runs pending.
                                        if ((row != endRow - 1) && (col != endCol - 1))
                                        {
                                            really = abortAllRunsFunc();
                                        }
                                        if (really)
                                        {
                                            ending = true;
                                            col = endCol;
                                            row = endRow;
                                            if (commonVars.getNonSimulationSettings().emailPerJob)
                                            {
                                                emailString = "Variance run aborted for tile " + col.ToString() + "," + row.ToString();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    emailString = "Variance run completed for tile " + col.ToString() + "," + row.ToString();
                                }
                            }
                            catch (Exception)
                            {
                                // Don't care about exceptions - we handle them internally.
                            }
                            if (commonVars.getNonSimulationSettings().emailPerJob)
                            {
                                Email.Send(commonVars.getNonSimulationSettings().host, commonVars.getNonSimulationSettings().port, commonVars.getNonSimulationSettings().ssl, emailString, lastSimResultsOverview, commonVars.getNonSimulationSettings().emailAddress, commonVars.getNonSimulationSettings().emailPwd);
                            }
                            col++;
                        }
                        row++;
                        if (!ending)
                        {
                            col = startCol; // reset column
                        }
                    }
                }
                if (tileHandling && (commonVars.getNonSimulationSettings().emailOnCompletion))
                {
                    Email.Send(commonVars.getNonSimulationSettings().host, commonVars.getNonSimulationSettings().port, commonVars.getNonSimulationSettings().ssl, "Variance batch complete: " + commonVars.projectFileName, lastSimResultsOverview, commonVars.getNonSimulationSettings().emailAddress, commonVars.getNonSimulationSettings().emailPwd);
                }
            }

            // Restore original values to settings.
            if (doPASearch)
            {
                commonVars.getPASearch().removeSearchValues(ref commonVars);
            }

            postSimUIFunc?.Invoke();
        }

        Results EntropyEval(bool previewMode, bool doPASearch, Int32 currentRow, Int32 currentCol, ChaosSettings currentJobSettings)
        {
            if (currentJobSettings == null)
            {
                throw new Exception("ChaosSettings are null!");
            }
            string value = "N/A";
            List<GeoLibPointF[]> listOfPoints;
            GeoLibPointF[] points;

            // Replay mode
            if (useLoadedSettings)
            {
                currentJobSettings = loadedJobSettings_;
            }

            // Number crunching goes here.
            List<PreviewShape> simShapes = new List<PreviewShape>();
            for (Int32 layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                // Show our variations - set preview to false
                // We share our jobSettings in case of correlated settings between levels.
                // currentRow/Col ignored in previewShape for non-tiled layout cases.
                // For omitted layers, we just set a blank entry. Slightly ugly, but it works.
                if (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.omit) == 1)
                {
                    simShapes.Add(new PreviewShape());
                }
                else
                {
                    simShapes.Add(new PreviewShape(
                        commonVars:commonVars,
                        jobSettings_: currentJobSettings,
                        settingsIndex: layer,
                        subShapeIndex: commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.subShapeIndex),
                        mode: 1,
                        doPASearch: doPASearch,
                        previewMode: false,
                        currentRow: currentRow,
                        currentCol: currentCol,
                        angularTolerance: varianceContext.angularTolerance)
                    );
                }
            }

            ChaosEngine currentJobEngine = new ChaosEngine(commonVars, simShapes, previewMode);

            listOfPoints = new List<GeoLibPointF[]>();
            if (currentJobEngine.isValid())
            {
                value = currentJobEngine.getResult();
                for (Int32 listMember = 0; listMember < currentJobEngine.getPaths().Count(); listMember++)
                {
                    // Check whether we need to manually close our shape or not, also only for the area case.
                    int length = currentJobEngine.getPaths()[listMember].Count();
                    int arraySize = length;
                    if ((commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.area) && (currentJobEngine.getPaths()[listMember][currentJobEngine.getPaths()[listMember].Count() - 1] != currentJobEngine.getPaths()[listMember][0]))
                    {
                        arraySize++;
                    }
                    points = new GeoLibPointF[arraySize];

#if ENTROPYTHREADED
                    Parallel.For(0, length, (i) =>
#else
                    for (Int32 i = 0; i < length; i++)
#endif
                    {
                        // The cast below is important - if this is missing or screwed up, we get clamped to integers and the viewport representation is fouled up.
                        points[i] = new GeoLibPointF(((float)currentJobEngine.getPaths()[listMember][i].X / CentralProperties.scaleFactorForOperation),
                                                     ((float)currentJobEngine.getPaths()[listMember][i].Y / CentralProperties.scaleFactorForOperation));
                    }
#if ENTROPYTHREADED
                    );
#endif
                    // Close the shape only if we have an area calculation; for other cases we expect lines.
                    if ((commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.area) && (points[points.Count() - 1] != points[0]))
                    {
                        points[points.Count() - 1] = points[0];
                    }
                    listOfPoints.Add(points);
                }
            }
            else
            {
                points = new GeoLibPointF[1];
                points[0] = new GeoLibPointF(0.0f, 0.0f);
                listOfPoints.Add(points);
            }

            Results evalResults = new Results();
            evalResults.setSimShapes(simShapes);
            evalResults.setResult(value);
            evalResults.setPoints(listOfPoints);
            evalResults.setFields(Results.fields_d.svar, currentJobSettings.getValues(ChaosSettings.properties.CDUSVar));
            evalResults.setFields(Results.fields_d.tvar, currentJobSettings.getValues(ChaosSettings.properties.CDUTVar));
            evalResults.setFields(Results.fields_d.lwr, currentJobSettings.getValues(ChaosSettings.properties.LWRVar));
            evalResults.setFields(Results.fields_d.lwr2, currentJobSettings.getValues(ChaosSettings.properties.LWR2Var));
            evalResults.setFields(Results.fields_d.htip, currentJobSettings.getValues(ChaosSettings.properties.hTipBiasVar));
            evalResults.setFields(Results.fields_d.vtip, currentJobSettings.getValues(ChaosSettings.properties.vTipBiasVar));
            evalResults.setFields(Results.fields_d.icv, currentJobSettings.getValues(ChaosSettings.properties.icVar));
            evalResults.setFields(Results.fields_d.ocv, currentJobSettings.getValues(ChaosSettings.properties.ocVar));
            evalResults.setFields(Results.fields_d.olx, currentJobSettings.getValues(ChaosSettings.properties.overlayX));
            evalResults.setFields(Results.fields_d.oly, currentJobSettings.getValues(ChaosSettings.properties.overlayY));
            evalResults.setFields(Results.fields_d.wob, currentJobSettings.getValues(ChaosSettings.properties.wobbleVar));
            evalResults.setSeeds(Results.fields_i.lwrs, currentJobSettings.getInts(ChaosSettings.ints.lwrSeed));
            evalResults.setSeeds(Results.fields_i.lwr2s, currentJobSettings.getInts(ChaosSettings.ints.lwr2Seed));
            evalResults.setValid(currentJobEngine.isValid());
            evalResults.genPreviewShapes();
            return evalResults;
        }
    }
}