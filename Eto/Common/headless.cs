using Error;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Variance
{
    public class Headless
    {
        class ConsoleSpinner
        {
            int counter;
            string[] sequence;

            public ConsoleSpinner()
            {
                counter = 0;
                sequence = new string[] { "/", "-", "\\", "|" };
                sequence = new string[] { ".", "o", "0", "o" };
                sequence = new string[] { "+", "x" };
                sequence = new string[] { "V", "<", "^", ">" };
                sequence = new string[] { ".   ", "..  ", "... ", "...." };
            }

            public void Turn()
            {
                counter++;

                if (counter >= sequence.Length)
                    counter = 0;

                Console.Write(sequence[counter]);
                Console.SetCursorPosition(Console.CursorLeft - sequence[counter].Length, Console.CursorTop);
            }
        }

        public VarianceContext varianceContext;
        ConsoleSpinner cs;

        CommonVars commonVars;
        Entropy entropyControl;

        public Headless(VarianceContext varianceContext)
        {
            pHeadless(varianceContext);
        }

        void pHeadless(VarianceContext _varianceContext)
        {
            core(_varianceContext);
        }

        void core(VarianceContext _varianceContext)
        {
            varianceContext = _varianceContext;
            commonVars = new CommonVars(varianceContext);
            varianceContext.emailPwd = varianceContext.aes.EncryptToString(varianceContext.emailPwd);
            entropyControl = new Entropy(ref varianceContext, commonVars);
            commonVars.getNonSimulationSettings().emailOnCompletion = varianceContext.completion;
            commonVars.getNonSimulationSettings().emailPerJob = varianceContext.perJob;
            commonVars.getNonSimulationSettings().emailAddress = varianceContext.emailAddress;
            commonVars.getNonSimulationSettings().emailPwd = varianceContext.emailPwd;
            commonVars.getNonSimulationSettings().host = varianceContext.host;
            commonVars.getNonSimulationSettings().ssl = varianceContext.ssl;
            commonVars.getNonSimulationSettings().port = varianceContext.port;
            commonVars.storage.setLayerSettings = setLayerSettings;
        }

        public void updateReadout(object sender, System.Timers.ElapsedEventArgs e)
        {
            pUpdateReadout();
        }

        void pUpdateReadout()
        {
            if ((entropyControl.sw_Preview.Elapsed.TotalMilliseconds - entropyControl.timeOfLastPreviewUpdate) < commonVars.m_timer.Interval)
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
                if (varianceContext.implantMode)
                {
                    entropyControl.timeOfFlight_implant(entropyControl.swTime);
                }
                else
                {
                    entropyControl.timeOfFlight(entropyControl.swTime, false);
                }
                if (Monitor.TryEnter(varianceContext.previewLock))
                {
                    try
                    {
                        List<string> t = new List<string>();
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
                        for (int l = 0; l < t.Count; l++)
                        {
                            Console.WriteLine(t[l]);
                        }
                    }
                    catch (Exception)
                    {
                        // Histogram can fail in case of insufficient variation - i.e. all values are the same.
                    }
                    finally
                    {
                        Monitor.Exit(varianceContext.previewLock);
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

        void summary()
        {
            List<string> t = new List<string>();
            if (varianceContext.implantMode)
            {
                Console.WriteLine("Converged on: " + entropyControl.getImplantResultPackage().getMeanAndStdDev());
                t = entropyControl.getImplantResultPackage().getHistograms(50);
            }
            else
            {
                Console.WriteLine("Converged on: " + entropyControl.getResultPackage().getMeanAndStdDev());
                t = entropyControl.getResultPackage().getHistograms(50);
            }
            for (int l = 0; l < t.Count; l++)
            {
                Console.WriteLine(t[l]);
            }
        }

        void updateSimUIST(bool doPASearch, SimResultPackage resultPackage, string resultString)
        {
            if (varianceContext.implantMode)
            {
                writeToConsole("Converging on: " + entropyControl.getImplantResultPackage().getMeanAndStdDev());
            }
            else
            {
                writeToConsole("Converging on: " + entropyControl.getResultPackage().getMeanAndStdDev());
            }
        }

        void directProgress(Int32 current, Int32 total)
        {
            Console.WriteLine(((current / (float)total) * 100.0f).ToString("0.##") + "% of results saved");
        }

        public void writeToConsole(string myString)
        {
            Console.WriteLine(myString);
        }

        void abortRun()
        {
            if (!commonVars.cancelling)
            {
                if ((commonVars.runAbort) && (!commonVars.userCancelQuery))
                {
                    commonVars.userCancelQuery = true;
                    commonVars.cancelling = true;
                    Console.WriteLine("Abort and save results so far? (y/n) ");
                    string userInput = Console.ReadLine();
                    if (userInput.ToUpper().StartsWith("Y"))
                    {
                        commonVars.runAbort = true;
                    }
                    else
                    {
                        commonVars.runAbort = false;
                    }
                    commonVars.userCancelQuery = false;
                }
                commonVars.cancelling = false;
            }
        }

        void abortRunMT(SimResultPackage resultPackage, CancellationTokenSource cancelSource, CancellationToken cancellationToken)
        {
            if (!commonVars.cancelling)
            {
                if ((commonVars.runAbort) && (!commonVars.userCancelQuery))
                {
                    commonVars.userCancelQuery = true;
                    commonVars.cancelling = true;
                    Console.WriteLine("Abort and save results so far? (y/n) ");
                    string userInput = Console.ReadLine();
                    if (userInput.ToUpper().StartsWith("Y"))
                    {
                        commonVars.runAbort = true;
                    }
                    else
                    {
                        commonVars.runAbort = false;
                    }
                    commonVars.userCancelQuery = false;

                    if (commonVars.runAbort)
                    {
                        resultPackage.setState(false);
                        cancelSource.Cancel();
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                commonVars.cancelling = false;
            }
        }

        public void cancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            commonVars.runAbort = true;
        }

        void hlRun(string csvFile)
        {
            Console.CancelKeyPress += cancelHandler;

            entropyControl.updateStatus = writeLine;
            entropyControl.directProgress = directProgress;
            entropyControl.clearAbortFlagFunc = clearAbortFlag;
            entropyControl.abortRunFunc = abortRun;
            entropyControl.abortRunFuncMT = abortRunMT;
            entropyControl.updateSimUIFunc = updateSimUIST;
            entropyControl.updateSimUIMTFunc = updateSimUIMT;
            entropyControl.updateProgressBarFunc = updateProgress;
            entropyControl.simRunningFunc = simRunning;
            entropyControl.postSimUIFunc = summary;

            string tmp;
            if (commonVars.getFriendly())
            {
                tmp = utility.Utils.friendlyNumber(commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases));
            }
            else
            {
                tmp = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases).ToString();
            }
            Console.WriteLine("Starting run for " + tmp + " cases.");
            entropyControl.swTime = 0.0;
            bool threaded = true;
            if (varianceContext.numberOfThreads == 1)
            {
                threaded = false;
            }

            entropyControl.EntropyRun(commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases), csvFile, threaded, false);
        }

        void hlImplantRun(string csvFile)
        {
            Console.CancelKeyPress += cancelHandler;

            entropyControl.updateStatus = writeLine;
            entropyControl.directProgress = directProgress;
            entropyControl.clearAbortFlagFunc = clearAbortFlag;
            entropyControl.abortRunFunc = abortRun;
            entropyControl.abortRunFuncMT = abortRunMT;
            entropyControl.updateImplantSimUIFunc = updateSimUIST;
            entropyControl.updateImplantSimUIMTFunc = updateSimUIMT;
            entropyControl.implantSimRunningUIFunc = simRunning;
            Console.WriteLine("Starting run for " + commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases).ToString() + " cases.");
            entropyControl.swTime = 0.0;
            bool threaded = true;
            if (varianceContext.numberOfThreads == 1)
            {
                threaded = false;
            }

            entropyControl.entropyRun_implant(commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases), csvFile, threaded);
        }

        void clearAbortFlag()
        {
            commonVars.runAbort = false;
        }

        void updateSimUIMT()
        {
            commonVars.m_timer.Elapsed += new System.Timers.ElapsedEventHandler(updateReadout);
        }

        void updateProgress(double val)
        {
            Console.WriteLine((val * 100.0f).ToString("0.##") + "% complete");
        }

        void writeLine(string statusLine)
        {
            Console.WriteLine(statusLine);
        }

        void simRunning()
        {
            commonVars.setSimRunning(true);
        }

        public void setLayerSettings(EntropyLayerSettings readSettings, int layer, bool gdsOnly, bool resumeUI = false, bool updateGeoCoreGeometryFromFile = false)
        {
            // Ignore resumeUI - only for GUI modes.
            pSetLayerSettings(readSettings, layer, gdsOnly, resumeUI, updateGeoCoreGeometryFromFile);
        }

        void pSetLayerSettings(EntropyLayerSettings entropyLayerSettings, int settingsIndex, bool gdsOnly, bool resumeUI, bool updateGeoCoreGeometryFromFile = false)
        {
            // This is used by the pasteHandler and clearHandler to set user interface values to align with the settings.
            // It is also used by the load from disk file system, using a temporary MCSettings instance as the source
            // In the case of the clearHandler, we get sent a new MCLayerSettings instance, so we have to handle that.
            // Check our copyFrom reference. We need to do this early before anything could change.

            if (!gdsOnly)
            {
                if (commonVars.isCopyPrepped())
                {
                    if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr) == 1) || (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr) == 1) || (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr) == 1))
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
                if (commonVars.isCopyPrepped())
                {
                    commonVars.getSimulationSettings().getDOESettings().setLayerAffected(settingsIndex, commonVars.getCopyDOEUse());
                }
            }
            catch (Exception)
            {
            }

            if ((entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.sCDU) != 0.0m) && (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE) &&
                (varianceContext.geoCoreCDVariation == false))
            {
                ErrorReporter.showMessage_OK("Project uses Oasis/GDS CD variation.", "Overriding preference.");
            }

            if (commonVars.isCopyPrepped())
            {
                // Align the external data.
                commonVars.pasteGeoCoreHandler(settingsIndex);
                // commonVars.getGeoCoreHandler(settingsIndex).readValues(commonVars.copyLayerGHSettings);
            }
            else
            {
                commonVars.getGeoCoreHandler(settingsIndex).setValid(false);
                if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE) && (entropyLayerSettings.isReloaded()))
                {
                    commonVars.getGeoCoreHandler(settingsIndex).setFilename(entropyLayerSettings.getString(EntropyLayerSettings.properties_s.file));
                    commonVars.getGeoCoreHandler(settingsIndex).setValid(true);
                    if (updateGeoCoreGeometryFromFile)
                    {
                        commonVars.getGeoCoreHandler(settingsIndex).setPoints(commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure), commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD));
                    }
                }
            }

            // Commit our settings to the list.
            commonVars.getLayerSettings(settingsIndex).adjustSettings(entropyLayerSettings, gdsOnly);

        }

        public void doStuff()
        {
            pDoStuff();
        }

        void pDoStuff()
        {
            Console.WriteLine("");
            Console.WriteLine(commonVars.titleText + ": headless mode");
            Console.WriteLine("brought to you by its creator: " + commonVars.author);
            Console.WriteLine();

            Console.WriteLine();
            if (varianceContext.xmlFileArg == "")
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
                else
                {
                    string[] tokens = varianceContext.xmlFileArg.Split(new char[] { '.' });
                    // XML file?
                    if ((tokens[tokens.Length - 1].ToUpper() != "VARIANCE") && (tokens[tokens.Length - 1].ToUpper() != "XML"))
                    {
                        ErrorReporter.showMessage_OK("Valid input file is expected. Exiting.", "Error: ");
                        return;
                    }
                }

                Console.Write("Loading from " + varianceContext.xmlFileArg + "....");
                commonVars.storage.setLayerSettings = setLayerSettings;
                string loadOK = commonVars.storage.loadSimulationSettings(CentralProperties.version, varianceContext.xmlFileArg, commonVars.getSimulationSettings(), commonVars.getSimulationSettings_nonSim(), commonVars.getListOfSettings(), commonVars.getImplantSimulationSettings(), commonVars.getImplantSettings_nonSim(), commonVars.getImplantSettings(), commonVars.getNonSimulationSettings());
                if (loadOK == "")
                {
                    // Loaded fine.
                    Console.WriteLine("OK");
                    commonVars.projectFileName = varianceContext.xmlFileArg;
                    string csvFile = varianceContext.xmlFileArg.Substring(0, varianceContext.xmlFileArg.Length - 3) + "csv";
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
                        Console.WriteLine("Something went wrong: " + ex.Message.ToString());
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
}
