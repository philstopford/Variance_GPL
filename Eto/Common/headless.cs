using Error;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Variance;

public class Headless
{
    private class ConsoleSpinner
    {
        private int counter;
        private string[] sequence;

        public ConsoleSpinner()
        {
            counter = 0;
            sequence = ["/", "-", "\\", "|"];
            sequence = [".", "o", "0", "o"];
            sequence = ["+", "x"];
            sequence = ["V", "<", "^", ">"];
            sequence = [".   ", "..  ", "... ", "...."];
        }

        public void Turn()
        {
            counter++;

            if (counter >= sequence.Length)
            {
                counter = 0;
            }

            Console.Write(sequence[counter]);
            Console.SetCursorPosition(Console.CursorLeft - sequence[counter].Length, Console.CursorTop);
        }
    }

    public VarianceContext? varianceContext;

    private CommonVars? commonVars;
    private Entropy? entropyControl;

    private CommonVars getCommonVars()
    {
        return commonVars ?? throw new InvalidOperationException("commonVars has not been initialized");
    }

    private Entropy getEntropyControl()
    {
        return entropyControl ?? throw new InvalidOperationException("entropyControl has not been initialized");
    }

    public Headless(VarianceContext varianceContext)
    {
        pHeadless(varianceContext);
    }

    private void pHeadless(VarianceContext _varianceContext)
    {
        core(_varianceContext);
    }

    private void core(VarianceContext _varianceContext)
    {
        varianceContext = _varianceContext;
        commonVars = new CommonVars(varianceContext);
        if (varianceContext.aes != null && varianceContext.emailPwd != null)
        {
            varianceContext.emailPwd = varianceContext.aes.EncryptToString(varianceContext.emailPwd);
        }
        entropyControl = new Entropy(ref varianceContext, commonVars);
        commonVars.getNonSimulationSettings().emailOnCompletion = varianceContext.completion;
        commonVars.getNonSimulationSettings().emailPerJob = varianceContext.perJob;
        commonVars.getNonSimulationSettings().emailAddress = varianceContext.emailAddress ?? "";
        commonVars.getNonSimulationSettings().emailPwd = varianceContext.emailPwd ?? "";
        commonVars.getNonSimulationSettings().host = varianceContext.host ?? "";
        commonVars.getNonSimulationSettings().ssl = varianceContext.ssl;
        commonVars.getNonSimulationSettings().port = varianceContext.port ?? "";
        commonVars.storage.setLayerSettings = setLayerSettings;
    }

    public void updateReadout(object? sender, System.Timers.ElapsedEventArgs e)
    {
        pUpdateReadout();
    }

    private void pUpdateReadout()
    {
        if (entropyControl!.sw_Preview.Elapsed.TotalMilliseconds - entropyControl.timeOfLastPreviewUpdate < getCommonVars().m_timer.Interval)
        {
            return;
        }
        try
        {
            if (entropyControl.sw.IsRunning)
            {
                entropyControl.swTime += entropyControl.sw.Elapsed.TotalSeconds;
            }
            entropyControl.sw.Stop();
            if (varianceContext!.implantMode)
            {
                entropyControl.timeOfFlight_implant(entropyControl.swTime);
            }
            else
            {
                entropyControl.timeOfFlight(entropyControl.swTime, false);
            }
            if (Monitor.TryEnter(varianceContext.previewLock!))
            {
                try
                {
                    List<string> t;
                    if (varianceContext.implantMode)
                    {
                        Console.WriteLine("Converging on: " + entropyControl.getImplantResultPackage().getMeanAndStdDev());
                        t = entropyControl.getImplantResultPackage().getHistograms(50);
                    }
                    else
                    {
                        Console.WriteLine("Converging on: " + entropyControl.getResultPackage().getMeanAndStdDev());
                        t = entropyControl.getResultPackage().getHistograms(50);
                    }
                    foreach (string t1 in t)
                    {
                        Console.WriteLine(t1);
                    }
                }
                catch (Exception)
                {
                    // Histogram can fail in case of insufficient variation - i.e. all values are the same.
                }
                finally
                {
                    Monitor.Exit(varianceContext.previewLock!);
                }
            }
            entropyControl.sw.Reset();
            entropyControl.sw.Start();
            entropyControl.timeOfLastPreviewUpdate = entropyControl.sw_Preview.Elapsed.Milliseconds;
        }
        catch (Exception)
        {
            //Console.WriteLine("died in updatereadout");
            //Console.ReadLine();
        }
    }

    private void summary()
    {
        List<string> t;
        if (varianceContext!.implantMode)
        {
            Console.WriteLine("Converged on: " + entropyControl!.getImplantResultPackage().getMeanAndStdDev());
            t = entropyControl.getImplantResultPackage().getHistograms(50);
        }
        else
        {
            Console.WriteLine("Converged on: " + entropyControl!.getResultPackage().getMeanAndStdDev());
            t = entropyControl.getResultPackage().getHistograms(50);
        }
        foreach (string t1 in t)
        {
            Console.WriteLine(t1);
        }
    }

    private void updateSimUIST(bool doPASearch, SimResultPackage resultPackage, string resultString)
    {
        if (varianceContext!.implantMode)
        {
            writeToConsole("Converging on: " + entropyControl!.getImplantResultPackage().getMeanAndStdDev());
        }
        else
        {
            writeToConsole("Converging on: " + entropyControl!.getResultPackage().getMeanAndStdDev());
        }
    }

    private static void directProgress(int current, int total)
    {
        Console.WriteLine((current / (float)total * 100.0f).ToString("0.##") + "% of results saved");
    }

    public static void writeToConsole(string myString)
    {
        Console.WriteLine(myString);
    }

    private void abortRun()
    {
        if (getCommonVars().cancelling)
        {
            return;
        }

        if (commonVars is { runAbort: true, userCancelQuery: false })
        {
            commonVars.userCancelQuery = true;
            commonVars.cancelling = true;
            Console.WriteLine("Abort and save results so far? (y/n) ");
            string userInput = Console.ReadLine()!;
            getCommonVars().runAbort = userInput.StartsWith("Y", StringComparison.CurrentCultureIgnoreCase);

            getCommonVars().userCancelQuery = false;
        }
        getCommonVars().cancelling = false;
    }

    private void abortRunMT(SimResultPackage resultPackage, CancellationTokenSource cancelSource, CancellationToken cancellationToken)
    {
        if (getCommonVars().cancelling)
        {
            return;
        }

        if (commonVars is { runAbort: true, userCancelQuery: false })
        {
            getCommonVars().userCancelQuery = true;
            getCommonVars().cancelling = true;
            Console.WriteLine("Abort and save results so far? (y/n) ");
            string userInput = Console.ReadLine()!;
            getCommonVars().runAbort = false || userInput.StartsWith("Y", StringComparison.CurrentCultureIgnoreCase);
            getCommonVars().userCancelQuery = false;

            if (getCommonVars().runAbort)
            {
                resultPackage.setState(false);
                cancelSource.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        getCommonVars().cancelling = false;
    }

    public void cancelHandler(object? sender, ConsoleCancelEventArgs e)
    {
        getCommonVars().runAbort = true;
    }

    private void hlRun(string csvFile)
    {
        Console.CancelKeyPress += cancelHandler;

        var entropy = getEntropyControl();
        entropy.updateStatus = writeLine;
        entropy.directProgress = directProgress;
        entropy.clearAbortFlagFunc = clearAbortFlag;
        entropy.abortRunFunc = abortRun;
        entropy.abortRunFuncMT = abortRunMT;
        entropy.updateSimUIFunc = updateSimUIST;
        entropy.updateSimUIMTFunc = updateSimUIMT;
        entropyControl!.updateProgressBarFunc = updateProgress;
        entropyControl.simRunningFunc = simRunning;
        entropyControl.postSimUIFunc = summary;

        var tmp = getCommonVars().getFriendly() ? 
            utility.Utils.friendlyNumber(getCommonVars().getSimulationSettings().getValue(EntropySettings.properties_i.nCases))
            : getCommonVars().getSimulationSettings().getValue(EntropySettings.properties_i.nCases).ToString();
        Console.WriteLine("Starting run for " + tmp + " cases.");
        entropyControl.swTime = 0.0;
        bool threaded = varianceContext!.numberOfThreads != 1;

        entropyControl!.EntropyRun(getCommonVars().getSimulationSettings().getValue(EntropySettings.properties_i.nCases), csvFile, threaded, false);
    }

    private void hlImplantRun(string csvFile)
    {
        Console.CancelKeyPress += cancelHandler;

        var entropy = getEntropyControl();
        entropy.updateStatus = writeLine;
        entropy.directProgress = directProgress;
        entropy.clearAbortFlagFunc = clearAbortFlag;
        entropy.abortRunFunc = abortRun;
        entropy.abortRunFuncMT = abortRunMT;
        entropy.updateImplantSimUIFunc = updateSimUIST;
        entropy.updateImplantSimUIMTFunc = updateSimUIMT;
        entropyControl!.implantSimRunningUIFunc = simRunning;
        Console.WriteLine("Starting run for " + getCommonVars().getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases) + " cases.");
        entropyControl.swTime = 0.0;
        bool threaded = varianceContext!.numberOfThreads != 1;

        entropyControl!.entropyRun_implant(getCommonVars().getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases), csvFile, threaded);
    }

    private void clearAbortFlag()
    {
        getCommonVars().runAbort = false;
    }

    private void updateSimUIMT()
    {
        getCommonVars().m_timer.Elapsed += updateReadout;
    }

    private static void updateProgress(double val)
    {
        Console.WriteLine((val * 100.0f).ToString("0.##") + "% complete");
    }

    private static void writeLine(string statusLine)
    {
        Console.WriteLine(statusLine);
    }

    private void simRunning()
    {
        getCommonVars().setSimRunning(true);
    }

    public void setLayerSettings(EntropyLayerSettings readSettings, int layer, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false)
    {
        // Ignore resumeUI - only for GUI modes.
        pSetLayerSettings(readSettings, layer, gdsOnly, updateGeoCoreGeometryFromFile);
    }

    private void pSetLayerSettings(EntropyLayerSettings entropyLayerSettings, int settingsIndex, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false)
    {
        // This is used by the pasteHandler and clearHandler to set user interface values to align with the settings.
        // It is also used by the load from disk file system, using a temporary MCSettings instance as the source
        // In the case of the clearHandler, we get sent a new MCLayerSettings instance, so we have to handle that.
        // Check our copyFrom reference. We need to do this early before anything could change.

        if (!gdsOnly)
        {
            if (getCommonVars().isCopyPrepped())
            {
                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr) == 1 || entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr) == 1 || entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr) == 1)
                {
                    // User pasting into the correlation layer. Disable correlation settings accordingly.
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == settingsIndex)
                    {
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.xOL_corr, 0);
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.xOL_corr_ref, -1);
                    }
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == settingsIndex)
                    {
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.yOL_corr, 0);
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, -1);
                    }
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) == settingsIndex)
                    {
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.xOL_ref, -1);
                    }
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) == settingsIndex)
                    {
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.yOL_ref, -1);
                    }
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == settingsIndex)
                    {
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.CDU_corr, 0);
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.CDU_corr_ref, -1);
                    }
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == settingsIndex)
                    {
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.tCDU_corr, 0);
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, -1);
                    }
                }
            }
        }

        // Remove any average overlay reference to the layer we're in, just for safety.
        entropyLayerSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, settingsIndex, 0);
        entropyLayerSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, settingsIndex, 0);

        try
        {
            // Call with '-1' can be triggered by load of project file where the copyFrom isn't set because we're direct driving the DOE flags.
            if (getCommonVars().isCopyPrepped())
            {
                getCommonVars().getSimulationSettings().getDOESettings().setLayerAffected(settingsIndex, getCommonVars().getCopyDOEUse());
            }
        }
        catch (Exception)
        {
        }

        if (entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.sCDU) != 0.0m && entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CentralProperties.shapeNames.GEOCORE &&
            varianceContext!.geoCoreCDVariation == false)
        {
            ErrorReporter.showMessage_OK("Project uses Oasis/GDS CD variation.", "Overriding preference.");
        }

        if (getCommonVars().isCopyPrepped())
        {
            // Align the external data.
            getCommonVars().pasteGeoCoreHandler(settingsIndex);
            // commonVars.getGeoCoreHandler(settingsIndex).readValues(commonVars.copyLayerGHSettings);
        }
        else
        {
            getCommonVars().getGeoCoreHandler(settingsIndex).setValid(false);
            if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CentralProperties.shapeNames.GEOCORE && entropyLayerSettings.isReloaded())
            {
                getCommonVars().getGeoCoreHandler(settingsIndex).setFilename(entropyLayerSettings.getString(EntropyLayerSettings.properties_s.file));
                getCommonVars().getGeoCoreHandler(settingsIndex).setValid(true);
                if (updateGeoCoreGeometryFromFile)
                {
                    getCommonVars().getGeoCoreHandler(settingsIndex).setPoints(getCommonVars().getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure), getCommonVars().getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD));
                }
            }
        }

        // Commit our settings to the list.
        getCommonVars().getLayerSettings(settingsIndex).adjustSettings(entropyLayerSettings, gdsOnly);

    }

    public void doStuff()
    {
        pDoStuff();
    }

    private void pDoStuff()
    {
        Console.WriteLine("");
        Console.WriteLine(getCommonVars().titleText + ": headless mode");
        Console.WriteLine("brought to you by its creator: " + getCommonVars().author);
        Console.WriteLine();

        Console.WriteLine();
        if (varianceContext!.xmlFileArg == "")
        {
            Console.WriteLine("No input file specified.");
            Console.WriteLine("Quitting.");
        }
        else
        {
            // Test that file actually exists.
            bool fileOK = File.Exists(varianceContext.xmlFileArg);
            if (!fileOK)
            {
                ErrorReporter.showMessage_OK("File does not exist. Exiting.", "Error: ");
                return;
            }

            string[] tokens = varianceContext.xmlFileArg!.Split(['.']);
            // XML file?
            if (tokens[^1].ToUpper() != "VARIANCE" && tokens[^1].ToUpper() != "XML")
            {
                ErrorReporter.showMessage_OK("Valid input file is expected. Exiting.", "Error: ");
                return;
            }

            Console.Write("Loading from " + varianceContext.xmlFileArg + "....");
            commonVars.storage.setLayerSettings = setLayerSettings;
            string loadOK = commonVars.storage.loadSimulationSettings(CentralProperties.version, varianceContext.xmlFileArg, commonVars.getSimulationSettings(), commonVars.getSimulationSettings_nonSim(), commonVars.getListOfSettings(), commonVars.getImplantSimulationSettings(), commonVars.getImplantSettings_nonSim(), commonVars.getImplantSettings(), commonVars.getNonSimulationSettings());
            if (loadOK == "")
            {
                // Loaded fine.
                Console.WriteLine("OK");
                commonVars.projectFileName = varianceContext.xmlFileArg;
                string csvFile = string.Concat(varianceContext.xmlFileArg.AsSpan(0, varianceContext.xmlFileArg.Length - tokens[^1].Length), "csv");
                Console.WriteLine("Results will be written to: " + csvFile);
                Console.WriteLine("Initializing settings for simulation");
                try
                {
                    if (varianceContext.implantMode)
                    {
                        hlImplantRun(csvFile);
                    }
                    else
                    {
                        hlRun(csvFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong: " + ex.Message);
                }
            }
            else
            {
                // Something happened.
                Console.WriteLine("not OK: " + loadOK);
            }
        }
    }
}