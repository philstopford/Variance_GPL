using Error;
using Eto.Forms;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Variance
{
    public partial class MainForm
    {
        void saveHandler(object sender, EventArgs e)
        {
            saveProject(commonVars.projectFileName);
        }

        void saveProject(string filename)
        {
            Application.Instance.Invoke(() =>
            {
                if (filename == "")
                {
                    // Need to request output file location and name.
                    SaveFileDialog sfd = new SaveFileDialog()
                    {
                        Title = "Enter file to save",
                        Filters =
                            {
                                new FileFilter("Variance files", "*.variance"),
                                new FileFilter("XML Files (*.xml)", ".xml")
                            }
                    };
                    if (sfd.ShowDialog(ParentWindow) == DialogResult.Ok)
                    {
                        filename = sfd.FileName;
                    }
                    else
                    {
                        saveEnabler();
                        return;
                    }
                }
                if (commonVars.storage.storeSimulationSettings(filename,
                        commonVars.getSimulationSettings(),
                        commonVars.getSimulationSettings_nonSim(),
                        commonVars.getListOfSettings(),
                        commonVars.getImplantSimulationSettings(),
                        commonVars.getImplantSettings_nonSim(),
                        commonVars.getImplantSettings(),
                        commonVars.getNonSimulationSettings()
                    )
                )
                {
                    commonVars.projectFileName = filename;
                }
                else
                {
                    MessageBox.Show("Error during save.", "Save failed", MessageBoxButtons.OK);
                }
                saveEnabler();
            });
        }

        void saveAsHandler(object sender, EventArgs e)
        {
            saveProject("");
        }

        void saveEnabler()
        {
            Application.Instance.Invoke(() =>
            {
                if (commonVars.projectFileName == "")
                {
                    //menu_fileSave.Enabled = false;
                    Title = commonVars.titleText;
                }
                else
                {
                    Title = commonVars.titleText + " - " + commonVars.projectFileName;
                    //menu_fileSave.Enabled = true;
                }
            });
        }

        void openHandler(object sender, EventArgs e)
        {
            // Need to request input file location and name.
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Choose file to load",
                MultiSelect = false,
                Filters =
                            {
                                new FileFilter("Variance files", "*.variance"),
                                new FileFilter("XML Files (*.xml)", ".xml")
                            }
            };
            if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
            {
                pNew();
                doLoad(ofd.FileName);
                commonVars.setHashes();
            }
        }

        void revertHandler(object sender, EventArgs e)
        {
            doLoad(commonVars.projectFileName);
        }

        void doLoad(string xmlFile)
        {
            // We suspend/resume the layout here to reduce the overhead from updating the huge number of UI controls.
            // We have to toggle this a few times to display progress messages to the user.
            Application.Instance.Invoke(() =>
            {
                int mainTabIndex = getMainSelectedIndex();
                int subTabIndex = -1;
                if (mainTabIndex == (int)CommonVars.twoDTabNames.layer)
                {
                    subTabIndex = getSubTabSelectedIndex();
                }
                suspendUIHandlers();
                updateStatusLine("Loading project. Please wait");
                string loadOK = commonVars.storage.loadSimulationSettings(CentralProperties.version, xmlFile, commonVars.getSimulationSettings(), commonVars.getSimulationSettings_nonSim(), commonVars.getListOfSettings(), commonVars.getImplantSimulationSettings(), commonVars.getImplantSettings_nonSim(), commonVars.getImplantSettings(), commonVars.getNonSimulationSettings());
                if (loadOK == "")
                {
                    updateStatusLine("Project loaded successfully");
                    commonVars.projectFileName = xmlFile;
                }
                else
                {
                    updateStatusLine("Project loading failed.");
                }
                resumeUIHandlers();
                updateStatusLine("Parsing project data. Please wait");
                if (subTabIndex == (int)CommonVars.twoDTabNames.DOE)
                {
                    doeSettingsChanged();
                }
                if (subTabIndex == (int)CommonVars.twoDTabNames.layer)
                {
                    refreshAllPreviewPanels();
                }
                if (subTabIndex == (int)CommonVars.twoDTabNames.settings)
                {
                    drawSimulationPanelHandler(false);
                    set_ui_from_settings(getSelectedLayerIndex());
                    updateSettingsUIFromSettings();
                    entropySettingsChanged(this, EventArgs.Empty);
                }
                if (subTabIndex == (int)CommonVars.twoDTabNames.layer)
                {
                    do2DLayerUI_exp(getSelectedLayerIndex(), updateUI: false);
                }
                if (mainTabIndex == (int)CommonVars.upperTabNames.Implant)
                {
                    updateImplantUIFromSettings();
                    doImplantShadowing(null, EventArgs.Empty);
                }
                resumeUIFromStorage();
            });
        }

        void newHandler(object sender, EventArgs e)
        {
            Application.Instance.Invoke(() =>
            {
                pNew();
            });
        }

        void pNew()
        {
            Application.Instance.Invoke(() =>
            {
                int storeIndex = getSelectedLayerIndex();
                suspendUIHandlers();
                commonVars.reset(varianceContext.vc); // use this method to avoid clobbering the observable collections.
                for (Int32 layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
                {
                    setLayerSettings(new EntropyLayerSettings(), layer, gdsOnly: false, resumeUI: true);
                    commonVars.getGeoCoreHandler(layer).reset();
                }
                commonVars.projectFileName = "";
                commonVars.getNonSimulationSettings().emailAddress = varianceContext.vc.emailAddress;
                commonVars.getNonSimulationSettings().emailPwd = varianceContext.vc.emailPwd;
                commonVars.getNonSimulationSettings().host = varianceContext.vc.host;
                commonVars.getNonSimulationSettings().port = varianceContext.vc.port;
                commonVars.setOpenGLProp(CommonVars.properties_gl.aa, varianceContext.vc.AA);
                commonVars.setGLInt(CommonVars.gl_i.zoom, varianceContext.vc.openGLZoomFactor);
                commonVars.setOpacity(CommonVars.opacity_gl.fg, varianceContext.vc.FGOpacity);
                commonVars.setOpacity(CommonVars.opacity_gl.bg, varianceContext.vc.BGOpacity);
                commonVars.setOpenGLProp(CommonVars.properties_gl.fill, varianceContext.vc.FilledPolygons);
                commonVars.setOpenGLProp(CommonVars.properties_gl.points, varianceContext.vc.drawPoints);
                commonVars.setGCCDV(varianceContext.vc.geoCoreCDVariation);
                commonVars.setLayerPreviewDOETile(varianceContext.vc.layerPreviewDOETile);
                commonVars.setFriendly(varianceContext.vc.friendlyNumber);
                setupGUI();
                resetViewPorts();
                set2DSelectedIndex(storeIndex);
                resumeUIHandlers();
            });

            replayUIFrozen = true;
            simReplay.reset();
            disableReplay();
            setReplayControls();
            replayUIFrozen = false;
            resumeUIFromStorage();
        }

        void prepUIForLoad()
        {
            Application.Instance.Invoke(() =>
            {
                experimental_listBox_layers.SelectedIndexChanged -= listbox_change;
                suspendUIHandlers();
                btn_Cancel.Enabled = true;
            });
        }

        void prepUIPostLoad()
        {
            Application.Instance.Invoke(() =>
            {
                btn_Cancel.Enabled = false;
                resumeUIHandlers();
                experimental_listBox_layers.SelectedIndexChanged += listbox_change;
            });
        }

        void locateDOEResults(object sender, EventArgs e)
        {
            SelectFolderDialog ofd = new SelectFolderDialog()
            {
                Title = "Please choose location of DOE results to summarize"
            };
            // Need to request output file location and name.
            if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
            {
                updateStatusLine("Generating summary file(s)");
                updateStatusLine(UtilityFuncs.summarizeDOEResults(ofd.Directory, Directory.GetFiles(ofd.Directory, "*_summary.txt")));
            }
        }

        void abortFileLoad(object sender, EventArgs e)
        {
            if (commonVars.loadAbort)
            {
                fileLoad_cancelTS.Cancel();
            }
        }

        async void geoFileChooser_Handler_exp(object sender, EventArgs e)
        {
            Int32 settingsIndex = -1;

            Application.Instance.Invoke(() =>
            {
                // Get our layer.
                settingsIndex = getSelectedLayerIndex();
                if (settingsIndex == -1)
                {
                    return;
                }

                geoCoreLoadingUI(false);

                startIndeterminateProgress();
            });

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select GDS or OAS file to Load",
                MultiSelect = false,
                Filters =
                            {
                                new FileFilter("Layout Files (*.gds; *.oas)", ".gds", "*.oas")
                            }
            };

            if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
            {
                Application.Instance.Invoke(() =>
                {
                    suspendUIHandlers();
                });

                prepUIForLoad();

                commonVars.loadAbort = false;
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.AutoReset = true;
                timer.Interval = 100;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(abortFileLoad);

                fileLoad_cancelTS = new CancellationTokenSource();

                bool fileOK = false;

                timer.Start();

                // The thread abort approach is regarded as an ugly hack and strongly discouraged, but seems to be the only way to abort a long-running single-stage task.
                // As the geoCore readers don't necessarily lend themselves well to gentle interruption, the big hammer below is used.

                Task fileLoadTask = Task.Run(() =>
                    {
                        try
                        {
                            using (fileLoad_cancelTS.Token.Register(Thread.CurrentThread.Abort))
                            {
                                fileOK = layoutLoad(settingsIndex, ofd.FileName);
                            }
                        }
                        catch (ThreadAbortException)
                        {
                            fileOK = false;
                        }
                        finally
                        {
                        }
                    }
                , fileLoad_cancelTS.Token);

                try
                {
                    await fileLoadTask;
                }
                catch (Exception)
                {
                    fileOK = false;
                }

                timer.Stop();
                timer.Dispose();
                fileLoad_cancelTS.Dispose();
                fileLoadTask.Dispose();

                if (fileOK)
                {
                    Application.Instance.Invoke(() =>
                    {
                        commonVars.getLayerSettings(settingsIndex).setString(EntropyLayerSettings.properties_s.file, commonVars.getGeoCoreHandler(settingsIndex).getFilename());
                        // Clear tracking for external point data.
                        commonVars.getLayerSettings(settingsIndex).setReloaded(false);
                        try
                        {
                            comboBox_layerStructureList_geoCore_exp.SelectedIndex = commonVars.getGeoCoreHandler(settingsIndex).getGeo().activeStructure;
                        }
                        catch (Exception)
                        {

                        }
                        try
                        {
                            comboBox_layerLDList_geoCore_exp.SelectedIndex = commonVars.getGeoCoreHandler(settingsIndex).getGeo().activeLD;
                        }
                        catch (Exception)
                        {

                        }
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.structure, comboBox_layerStructureList_geoCore_exp.SelectedIndex);
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lD, comboBox_layerLDList_geoCore_exp.SelectedIndex);

                        commonVars.getGeoCoreHandler(settingsIndex).setPoints(commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure), commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD));
                    });
                }
                else
                {
                    commonVars.getGeoCoreHandler(settingsIndex).getGeo().reset();
                }
                Application.Instance.Invoke(() =>
                    commonVars.getGeoCoreHandler(settingsIndex).getGeo().updateCollections()
                );

                Application.Instance.Invoke(() =>
                {
                    // Refresh layer UI : a little hacky due to the suspended UI elements, but it works.
                    do2DLayerUI_exp(settingsIndex, updateUI: true);
                    do2DLayerUI_geoCore_exp(settingsIndex);
                    prepUIPostLoad();
                    resumeUIHandlers();
                });
            }

            Application.Instance.Invoke(() =>
            {
                stopIndeterminateProgress();
                geoCoreLoadingUI(false);
                do2DLayerUI_exp(settingsIndex, updateUI: true);
            });
        }

        bool layoutLoad(int settingsIndex, string filename)
        {
            string[] tokens = filename.Split(new char[] { '.' });
            string ext = tokens[tokens.Length - 1].ToUpper();

            if ((ext == "GDS") || (ext == "OAS"))
            {

                if (ext == "GDS")
                {
                    commonVars.getGeoCoreHandler(settingsIndex).updateGeoCoreHandler(filename, geoCoreLib.GeoCore.fileType.gds);
                }

                if (ext == "OAS")
                {
                    commonVars.getGeoCoreHandler(settingsIndex).updateGeoCoreHandler(filename, geoCoreLib.GeoCore.fileType.oasis);
                }

                return commonVars.getGeoCoreHandler(settingsIndex).isValid();
            }

            return false;
        }

        void iDRMFileChooser_Handler(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select iDRM CSV file to Load",
                MultiSelect = false,
                Filters =
                        {
                            new FileFilter("CSV Files (*.csv)", ".csv")
                        }
            };
            bool reading = false;
            if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
            {
                try
                {
                    reading = UtilityFuncs.readiDRMCSVFile(ref commonVars, ofd.FileName);
                    suspendDOESettingsUI();
                    num_colOffset.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
                    num_rowOffset.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
                    num_DOEColPitch.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colPitch);
                    num_DOERowPitch.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowPitch);
                    num_DOECols.Value = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.cols);
                    num_DOERows.Value = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.rows);
                    resumeDOESettingsUI();
                }
                catch (Exception exMsg)
                {
                    commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.iDRM, false);
                    if (reading == true)
                    {
                        MessageBox.Show(exMsg.ToString(), "Oops", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Could not read file", "Oops", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                if (commonVars.getSimulationSettings().getDOESettings().getBool(DOESettings.properties_b.iDRM))
                {
                    // Don't change anything.
                }
                else
                {
                    // Failsafe conditions to avoid downstream fail.
                    commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTile, 0);
                    commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 0);
                }
            }
        }

        void QuiltFileChooser_Handler(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Select Quilt CSV file to Load",
                MultiSelect = false,
                Filters =
                            {
                                new FileFilter("CSV Files (*.csv)", ".csv")
                            }
            };
            bool reading = false;
            if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
            {
                try
                {
                    reading = UtilityFuncs.readQuiltCSVFile(ref commonVars, ofd.FileName);
                    suspendDOESettingsUI();
                    num_colOffset.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
                    num_rowOffset.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
                    num_DOEColPitch.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colPitch);
                    num_DOERowPitch.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowPitch);
                    num_DOECols.Value = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.cols);
                    num_DOERows.Value = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.rows);
                    resumeDOESettingsUI();
                    doeSettingsChanged();
                }
                catch (Exception exMsg)
                {
                    commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.iDRM, false);
                    if (reading == true)
                    {
                        MessageBox.Show(exMsg.ToString(), "Oops", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Could not read file", "Oops", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
                if (commonVars.getSimulationSettings().getDOESettings().getBool(DOESettings.properties_b.iDRM))
                {
                    // Don't change anything.
                }
                else
                {
                    // Failsafe conditions to avoid downstream fail.
                    commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTile, 0);
                    commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 0);
                }
            }
        }

        void geoCoreLoadingUI(bool status)
        {
            Application.Instance.Invoke(() =>
            {
                tabControl_main.Enabled = !status;
                tabControl_2D_simsettings.Enabled = !status;
            });
        }

        void resumeUIFromStorage()
        {
            Application.Instance.Invoke(() =>
            {
                int mainIndex = getMainSelectedIndex();
                int twoDIndex = -1;
                if (mainIndex == (int)CommonVars.upperTabNames.twoD)
                {
                    twoDIndex = getSubTabSelectedIndex();
                    switch (twoDIndex)
                    {
                        case (int)CommonVars.twoDTabNames.layer:
                            set_ui_from_settings(getSelectedLayerIndex());
                            setOmitLayerCheckboxes_EnableStatus();
                            updateOmitLayerCheckboxes();
                            break;
                        case (int)CommonVars.twoDTabNames.settings:
                            updateSettingsUIFromSettings();
                            resumeSettingsUI();
                            break;
                        case (int)CommonVars.twoDTabNames.DOE:
                            updateDOESettingsUIFromSettings();
                            resumeDOESettingsUI();
                            break;
                    }
                }
                if (mainIndex == (int)CommonVars.upperTabNames.Implant)
                {
                    updateImplantUIFromSettings();
                }
                commonVars.setHashes();
                uiFollowChanges();
            });
        }

        void updateLayerNames()
        {
            experimental_listBox_layers.SelectedIndexChanged -= listbox_change;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                if (name == "")
                {
                    name = (i + 1).ToString();
                }
                commonVars.layerNames[i] = name;
            }
            experimental_listBox_layers.SelectedIndexChanged += listbox_change;
        }

        void stopIndeterminateProgress()
        {
            Application.Instance.Invoke(() =>
            {
                statusProgressBar.Visible = false;
                statusProgressBar.Indeterminate = false;
                configProgressBar(0, commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases));
            });
        }
    }
}