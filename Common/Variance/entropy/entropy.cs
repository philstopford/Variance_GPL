using System;
using System.Diagnostics;
using System.Threading;

namespace Variance;

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

    private readonly char[] csvSeparator = { ',' }; // to cleave the results apart.
    public Stopwatch sw { get; private set; }
    public Stopwatch sw_Preview { get; private set; }
    public int currentProgress;
    public bool multiCaseSim { get; private set; }
    public double swTime { get; set; }
    public bool simJustDone { get; private set; }
    public long timeOfLastPreviewUpdate { get; set; }
    private CommonVars commonVars;

    private string baseFileName;

    private readonly VarianceContext varianceContext;

    public string lastSimResultsOverview { get; private set; }
    private string resultString;

    public Entropy(ref VarianceContext varianceContext_, CommonVars commonVars_)
    {
        varianceContext = varianceContext_;
        commonVars = commonVars_;
    }

    public void update(CommonVars commonVars_)
    {
        commonVars = commonVars_;
    }

    private void reset()
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

    private bool entropyRunCore(int numberOfCases, int row, int col, string fileName, bool useThreads, bool tileHandling, bool implantMode, bool doPASearch)
    {
        baseFileName = "";

        if (fileName != null)
        {
            string[] tokens = fileName.Split(new [] { '.' });

            for (int i = 0; i < tokens.Length - 2; i++)
            {
                baseFileName += tokens[i] + ".";
            }
            baseFileName += tokens[^2];
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
                switch (doPASearch)
                {
                    case false:
                        configProgress?.Invoke(0, commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases));
                        break;
                    default:
                        configProgress?.Invoke(0, commonVars.getPASearch().numberofPassCases);
                        break;
                }

                simRunningUIFunc?.Invoke();
            }

            if (!useThreads)
            {
                entropyRunCore_singleThread(previewMode, numberOfCases, row, col, doPASearch);
            }
            else
            {
                sw_Preview.Start();
                entropyRunCore_multipleThread(previewMode, numberOfCases, row, col, tileHandling, doPASearch);
            }

            switch (doPASearch)
            {
                case true:
                    postSimPASearchUIFunc?.Invoke(resultPackage);
                    break;
                // If user requested file save, let's add to the list for eventual writing.
                case false when baseFileName != null:
                {
                    clearAbortFlagFunc?.Invoke(); // reset our abort handler in case user also wants to abort save.
                    if (!previewMode)
                    {
                        try
                        {
                            saveResults(tileHandling, col, row);
                        }
                        catch (Exception)
                        {

                        }
                    }

                    // Post-calc readout

                    for (int i = 0; i < resultPackage.getValues(SimResultPackage.properties.mean).Length; i++)
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

                    break;
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
            if (!string.IsNullOrEmpty(baseFileName))
            {
                clearAbortFlagFunc?.Invoke(); // reset our abort handler in case user also wants to abort save.
                if (!previewMode)
                {
                    try
                    {
                        saveResults_implant();
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