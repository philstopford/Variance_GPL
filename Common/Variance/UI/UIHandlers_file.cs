using System;
using System.IO;
using System.Threading.Tasks;
using Error;
using Eto.Forms;
using geoCoreLib;

namespace Variance;

public partial class MainForm
{
    private void saveHandler(object sender, EventArgs e)
    {
        saveProject(commonVars.projectFileName);
    }

    private void saveProject(string filename)
    {
        Application.Instance.Invoke(() =>
        {
            if (filename == "")
            {
                // Need to request output file location and name.
                SaveFileDialog sfd = new()
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

    private void saveAsHandler(object sender, EventArgs e)
    {
        saveProject("");
    }

    private void saveEnabler()
    {
        Application.Instance.Invoke(() =>
        {
            if (commonVars.projectFileName == "")
            {
                Title = commonVars.titleText;
            }
            else
            {
                Title = commonVars.titleText + " - " + commonVars.projectFileName;
            }
        });
    }

    private static void dragEvent(object sender, DragEventArgs e)
    {
        if (!e.Data.ContainsUris)
        {
            return;
        }

        // Check that we have a valid file somewhere in the dropped resources
        foreach (var t in e.Data.Uris)
        {
            string[] tokens = t.LocalPath.Split(new[] { '.' });
            if (tokens[^1].ToUpper() != "VARIANCE" && tokens[^1].ToUpper() != "XML")
            {
                continue;
            }

            e.Effects = DragEffects.Copy;
            break;
        }
    }

    private void dragAndDrop(object sender, DragEventArgs e)
    {
        // Only allow a single selection; pick the first.
        DataObject d = e.Data;
        int length = d.Uris.Length;
        if (length < 1)
        {
            return;
        }

        // Find our first file object.
        int index = -1;
        for (int i = 0; i < length; i++)
        {
            if (!d.Uris[i].IsFile)
            {
                continue;
            }

            // Actually a supported file?
            string[] tokens = d.Uris[i].LocalPath.Split(new[] { '.' });
            if (tokens[^1].ToUpper() != "VARIANCE" && tokens[^1].ToUpper() != "XML")
            {
                continue;
            }

            index = i;
            break;
        }

        if (index != -1)
        {
            doLoad(d.Uris[index].LocalPath);
        }
    }

    private void openHandler(object sender, EventArgs e)
    {
        // Need to request input file location and name.
        OpenFileDialog ofd = new()
        {
            Title = "Choose file to load",
            MultiSelect = false,
            Filters =
            {
                new FileFilter("Variance files", "*.variance"),
                new FileFilter("XML Files (*.xml)", ".xml")
            }
        };
        if (ofd.ShowDialog(ParentWindow) != DialogResult.Ok)
        {
            return;
        }

        pNew(false);
        doLoad(ofd.FileName);
        commonVars.setHashes();
    }

    private void revertHandler(object sender, EventArgs e)
    {
        doLoad(commonVars.projectFileName);
    }

    private void doLoad(string xmlFile)
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
            switch (subTabIndex)
            {
                case (int)CommonVars.twoDTabNames.DOE:
                    doeSettingsChanged();
                    break;
                case (int)CommonVars.twoDTabNames.layer:
                    refreshAllPreviewPanels();
                    break;
                case (int)CommonVars.twoDTabNames.settings:
                    drawSimulationPanelHandler(false);
                    set_ui_from_settings(getSelectedLayerIndex());
                    updateSettingsUIFromSettings();
                    entropySettingsChanged(this, EventArgs.Empty);
                    break;
            }

            if (subTabIndex == (int)CommonVars.twoDTabNames.layer)
            {
                do2DLayerUI(getSelectedLayerIndex(), updateUI: false);
            }
            if (mainTabIndex == (int)CommonVars.upperTabNames.Implant)
            {
                updateImplantUIFromSettings();
                doImplantShadowing(null, EventArgs.Empty);
            }
            resumeUIFromStorage();
        });
    }

    private void newHandler(object sender, EventArgs e)
    {
        Application.Instance.Invoke(() =>
        {
            pNew(commonVars.isChanged());
        });
    }

    private void pNew(bool prompt)
    {
        Application.Instance.Invoke(() =>
        {
            if (prompt)
            {
                var result = MessageBox.Show("Are you sure?", "New", MessageBoxButtons.YesNo, MessageBoxType.Question);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }
            int storeIndex = getSelectedLayerIndex();
            suspendUIHandlers();
            commonVars.reset(varianceContext.vc); // use this method to avoid clobbering the observable collections.
            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                setLayerSettings(new EntropyLayerSettings(), layer, gdsOnly: false);
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

    private void prepUIForLoad()
    {
        Application.Instance.Invoke(() =>
        {
            listBox_layers.SelectedIndexChanged -= listbox_change;
            suspendUIHandlers();
            btn_Cancel.Enabled = true;
        });
    }

    private void prepUIPostLoad()
    {
        Application.Instance.Invoke(() =>
        {
            btn_Cancel.Enabled = false;
            resumeUIHandlers();
            listBox_layers.SelectedIndexChanged += listbox_change;
        });
    }

    private void locateDOEResults(object sender, EventArgs e)
    {
        SelectFolderDialog ofd = new()
        {
            Title = "Please choose location of DOE results to summarize"
        };
        // Need to request output file location and name.
        if (ofd.ShowDialog(ParentWindow) != DialogResult.Ok)
        {
            return;
        }

        updateStatusLine("Generating summary file(s)");
        updateStatusLine(UtilityFuncs.summarizeDOEResults(ofd.Directory, Directory.GetFiles(ofd.Directory, "*_summary.txt")));
    }

    private async void geoFileChooser_Handler_exp(object sender, EventArgs e)
    {
        int settingsIndex = -1;

        await Application.Instance.InvokeAsync(() =>
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

        OpenFileDialog ofd = new()
        {
            Title = "Select GDSII or OASIS file to Load",
            MultiSelect = false,
            Filters =
            {
                new FileFilter("Layout Files (*.gds; *.gdsii; *.oas; *.oasis; *.gds.gz; *.gdsii.gz; *.oas.gz; *.oasis.gz)", ".gds", ".gdsii", "*.oas", "*.oasis", ".gds.gz", ".gdsii.gz", "*.oas.gz", "*.oasis.gz")
            }
        };

        bool fileOK = false;
        string error = "";

        if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
        {
            await Application.Instance.InvokeAsync(suspendUIHandlers);

            prepUIForLoad();

            commonVars.loadAbort = false;

            // The thread abort approach is regarded as an ugly hack and strongly discouraged, but seems to be the only way to abort a long-running single-stage task.
            // As the geoCore readers don't necessarily lend themselves well to gentle interruption, the big hammer below is used.

            Task fileLoadTask = Task.Run(() =>
                {
                    try
                    {
                        Application.Instance.Invoke(() =>
                        {
                            fileOK = layoutLoad(settingsIndex, ofd.FileName);
                        });
                    }
                    catch (Exception)
                    {
                        fileOK = false;
                    }
                }
            );

            try
            {
                await fileLoadTask;
            }
            catch (Exception)
            {
                fileOK = false;
            }
                
            if (fileOK)
            {
                await Application.Instance.InvokeAsync(() =>
                {
                    commonVars.getLayerSettings(settingsIndex).setString(EntropyLayerSettings.properties_s.file, commonVars.getGeoCoreHandler(settingsIndex).getFilename());
                    // Clear tracking for external point data.
                    commonVars.getLayerSettings(settingsIndex).setReloaded(false);
                    try
                    {
                        comboBox_structureList_geoCore.SelectedIndex = commonVars.getGeoCoreHandler(settingsIndex).getGeo().activeStructure;
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        comboBox_lDList_geoCore.SelectedIndex = commonVars.getGeoCoreHandler(settingsIndex).getGeo().activeLD;
                    }
                    catch (Exception)
                    {

                    }
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.structure, comboBox_structureList_geoCore.SelectedIndex);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lD, comboBox_lDList_geoCore.SelectedIndex);

                    commonVars.getGeoCoreHandler(settingsIndex).setPoints(commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure), commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD));
                });
            }
            else
            {
                error = "";
                int noOfErrors = commonVars.getGeoCoreHandler(settingsIndex).error_msgs.Count;
                for (int s = 0; s < noOfErrors; s++)
                {
                    error += commonVars.getGeoCoreHandler(settingsIndex).error_msgs[s];
                    if (s > 0 && s != noOfErrors - 1)
                    {
                        error += "\r\n";
                    }
                }
                commonVars.getGeoCoreHandler(settingsIndex).getGeo().reset();
            }
            await Application.Instance.InvokeAsync(() =>
                commonVars.getGeoCoreHandler(settingsIndex).getGeo().updateCollections()
            );

            await Application.Instance.InvokeAsync(() =>
            {
                // Refresh layer UI : a little hacky due to the suspended UI elements, but it works.
                do2DLayerUI(settingsIndex, updateUI: true);
                do2DLayerUI_geoCore(settingsIndex);
                prepUIPostLoad();
                resumeUIHandlers();
            });
        }

        await Application.Instance.InvokeAsync(() =>
        {
            stopIndeterminateProgress();
            geoCoreLoadingUI(false);
            do2DLayerUI(settingsIndex, updateUI: true);
            if (!fileOK)
            {
                ErrorReporter.showMessage_OK(error, "Error(s) loading file");
            }
        });
    }

    private bool layoutLoad(int settingsIndex, string filename)
    {
        string[] tokens = filename.Split(new[] { '.' });
        string ext = tokens[^1].ToUpper();

        switch (ext)
        {
            case "GDS":
            case "GDSII":
            case "OAS":
            case "OASIS":
            case "GZ":
            {
                if (ext is "GDS" or "GDSII" || ext == "GZ" && (tokens[^2].ToUpper() == "GDS" || tokens[^2].ToUpper() == "GDSII"))
                {
                    commonVars.getGeoCoreHandler(settingsIndex).updateGeoCoreHandler(filename, GeoCore.fileType.gds);
                }

                if (ext is "OAS" or "OASIS" || ext == "GZ" && (tokens[^2].ToUpper() == "OAS" || tokens[^2].ToUpper() == "OASIS"))
                {
                    commonVars.getGeoCoreHandler(settingsIndex).updateGeoCoreHandler(filename, GeoCore.fileType.oasis);
                }

                return commonVars.getGeoCoreHandler(settingsIndex).isValid();
            }
            default:
                return false;
        }
    }

    private void iDRMFileChooser_Handler(object sender, EventArgs e)
    {
        OpenFileDialog ofd = new()
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
                MessageBox.Show(reading ? exMsg.ToString() : "Could not read file", "Oops", MessageBoxButtons.OK);
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

    private void QuiltFileChooser_Handler(object sender, EventArgs e)
    {
        OpenFileDialog ofd = new()
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
                switch (reading)
                {
                    case true:
                        MessageBox.Show(exMsg.ToString(), "Oops", MessageBoxButtons.OK);
                        break;
                    default:
                        MessageBox.Show("Could not read file", "Oops", MessageBoxButtons.OK);
                        break;
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

    private void geoCoreLoadingUI(bool status)
    {
        Application.Instance.Invoke(() =>
        {
            tabControl_main.Enabled = !status;
            tabControl_2D_simsettings.Enabled = !status;
        });
    }

    private void resumeUIFromStorage()
    {
        Application.Instance.Invoke(() =>
        {
            int mainIndex = getMainSelectedIndex();
            switch (mainIndex)
            {
                case (int)CommonVars.upperTabNames.twoD:
                {
                    int twoDIndex = getSubTabSelectedIndex();
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

                    break;
                }
                case (int)CommonVars.upperTabNames.Implant:
                    updateImplantUIFromSettings();
                    break;
            }

            commonVars.setHashes();
            uiFollowChanges();
        });
    }

    private void updateLayerNames()
    {
        listBox_layers.SelectedIndexChanged -= listbox_change;
        for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
        {
            string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
            if (name == "")
            {
                name = (i + 1).ToString();
            }
            commonVars.layerNames[i] = name;
        }
        listBox_layers.SelectedIndexChanged += listbox_change;
    }

    private void stopIndeterminateProgress()
    {
        Application.Instance.Invoke(() =>
        {
            // statusProgressBar.Visible = false;
            statusProgressBar.Indeterminate = false;
            configProgressBar(0, commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases));
        });
    }
}