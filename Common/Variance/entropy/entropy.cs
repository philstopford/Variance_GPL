using System;
using System.Diagnostics;
using System.Threading;

namespace Variance
{
    public partial class Entropy
    {
        public delegate void updateStatusUI(string statusLine);
        public updateStatusUI updateStatus { get; set; }

        public delegate void configProgressUI(int value, int numberOfCases);
        public configProgressUI configProgress { get; set; }

        public delegate void stepProgressUI();
        public stepProgressUI stepProgress { get; set; }

        public delegate void directProgressUI(int current, int done);
        public directProgressUI directProgress { get; set; }

        public delegate bool abortCheck();
        public abortCheck abortCheckFunc { get; set; }

        public delegate void clearAbortFlag();
        public clearAbortFlag clearAbortFlagFunc { get; set; }

        public delegate void postSimUI();
        public postSimUI postSimUIFunc { get; set; }

        public delegate void postSimPASearchUI(SimResultPackage resultPackage);
        public postSimPASearchUI postSimPASearchUIFunc { get; set; }

        public delegate void forceRepaint();
        public forceRepaint forceRepaintFunc { get; set; }

        public delegate bool abortAllRuns();
        public abortAllRuns abortAllRunsFunc { get; set; }

        public delegate void abortCSV(CancellationTokenSource cancelSource, CancellationToken cancellationToken);
        public abortCSV abortCSVFunc { get; set; }

        public delegate void abortRun();
        public abortRun abortRunFunc { get; set; }

        public delegate void multithreadWarning();
        public multithreadWarning multithreadWarningFunc { get; set; }

        public delegate void abortRunMT(SimResultPackage resultPackage, CancellationTokenSource cancelSource, CancellationToken cancellationToken);
        public abortRunMT abortRunFuncMT { get; set; }

        public delegate void simRunning();
        public simRunning simRunningFunc { get; set; }

        public delegate void simRunningUI();
        public simRunningUI simRunningUIFunc { get; set; }

        char[] csvSeparator = new char[] { ',' }; // to cleave the results apart.
        public Stopwatch sw { get; set; }
        public Stopwatch sw_Preview { get; set; }
        public Int32 currentProgress;
        public bool multiCaseSim { get; set; }
        public double swTime { get; set; }
        public bool simJustDone { get; set; }
        public Int64 timeOfLastPreviewUpdate { get; set; }
        CommonVars commonVars;

        string baseFileName;

        VarianceContext varianceContext;

        public string lastSimResultsOverview { get; set; }
        string resultString;

        public Entropy(ref VarianceContext varianceContext, CommonVars commonVars)
        {
            this.varianceContext = varianceContext;
            this.commonVars = commonVars;
        }

        public void update(CommonVars commonVars)
        {
            this.commonVars = commonVars;
        }

        void reset()
        {
            sw = null;
            sw_Preview = null;
            currentProgress = 0;
            multiCaseSim = false;
            swTime = 0.0;
            simJustDone = false;
            resultString = null;
            resultPackage = null;
            timeOfLastPreviewUpdate = 0;
            commonVars.cancelling = false;
        }

        bool entropyRunCore(Int32 numberOfCases, Int32 row, Int32 col, string fileName, bool useThreads, bool tileHandling, bool implantMode, bool doPASearch)
        {
            baseFileName = "";

            if (fileName != null)
            {
                string[] tokens = fileName.Split(new char[] { '.' });

                for (int i = 0; i < tokens.Length - 2; i++)
                {
                    baseFileName += tokens[i] + ".";
                }
                baseFileName += tokens[tokens.Length - 2];
            }

            if (!implantMode)
            {
                preFlight(row, col, tileHandling);

                simRunningFunc?.Invoke();

                bool previewMode = true;
                if (numberOfCases > 1)
                {
                    multiCaseSim = true;
                    previewMode = false;
                    if (!doPASearch)
                    {
                        configProgress?.Invoke(0, commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases));
                    }
                    else
                    {
                        configProgress?.Invoke(0, commonVars.getPASearch().numberofPassCases);
                    }

                    simRunningUIFunc?.Invoke();
                }

                if (!useThreads)
                {
                    entropyRunCore_singleThread(previewMode, numberOfCases, row, col, tileHandling, doPASearch);
                }
                else
                {
                    sw_Preview.Start();
                    entropyRunCore_multipleThread(previewMode, numberOfCases, row, col, tileHandling, doPASearch);
                }

                if (doPASearch)
                {
                    postSimPASearchUIFunc?.Invoke(resultPackage);
                }

                // If user requested file save, let's add to the list for eventual writing.
                if (!doPASearch && (baseFileName != null))
                {
                    clearAbortFlagFunc?.Invoke(); // reset our abort handler in case user also wants to abort save.
                    if (!previewMode)
                    {
                        try
                        {
                            saveResults(numberOfCases, tileHandling, col, row);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    // Post-calc readout

                    for (Int32 i = 0; i < resultPackage.getValues(SimResultPackage.properties.mean).Length; i++)
                    {
                        if (i > 0)
                        {
                            lastSimResultsOverview += ", ";
                        }
                        lastSimResultsOverview += "x: " + resultPackage.getValue(SimResultPackage.properties.mean, i).ToString("0.##") + " (" + (resultPackage.getValue(SimResultPackage.properties.mean, i) - resultPackage.getValue(SimResultPackage.properties.lastMean, i)).ToString("0.##") + ")";
                        if (!resultPackage.nonGaussianInput)
                        {
                            lastSimResultsOverview += ", s: " + resultPackage.getValue(SimResultPackage.properties.stdDev, i).ToString("0.##") + " (" + (resultPackage.getValue(SimResultPackage.properties.stdDev, i) - resultPackage.getValue(SimResultPackage.properties.lastStdDev, i)).ToString("0.##") + ")";
                        }
                        simJustDone = true;
                    }
                }

                multiCaseSim = false;
                return resultPackage.getState();
            }
            else
            {
                bool previewMode = true;
                preFlight(row: 0, col: 0, false);

                // implant mode.
                simRunningFunc?.Invoke();

                if (numberOfCases > 1)
                {
                    previewMode = false;
                    multiCaseSim = true;
                    configProgress?.Invoke(0, commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases));

                    implantSimRunningUIFunc?.Invoke();
                }


                if (!useThreads)
                {
                    entropyRunCore_implant_singleThread(previewMode, numberOfCases);
                }
                else
                {
                    sw_Preview.Start();
                    entropyRunCore_implant_multipleThread(previewMode, numberOfCases);
                }

                // If user requested file save, let's add to the list for eventual writing.
                if ((baseFileName != null) && (baseFileName != ""))
                {
                    clearAbortFlagFunc?.Invoke(); // reset our abort handler in case user also wants to abort save.
                    if (!previewMode)
                    {
                        try
                        {
                            saveResults_implant(numberOfCases);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                lastSimResultsOverview = "x: " + implantResultPackage.getValue(SimResultPackage.properties.mean, 0).ToString("0.##");
                if (!implantResultPackage.nonGaussianInput)
                {
                    lastSimResultsOverview += ", s: " + implantResultPackage.getValue(SimResultPackage.properties.stdDev, 0).ToString("0.##");
                }
                simJustDone = true;
                multiCaseSim = false;
                return implantResultPackage.getState();
            }
        }
    }
}
