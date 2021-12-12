using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private bool abortCheck()
    {
        if (!commonVars.runAbort)
        {
            return commonVars.runAbort;
        }

        DialogResult dR = MessageBox.Show("Abort saving of results?", "Abort save?", MessageBoxButtons.YesNo);
        if (dR != DialogResult.Yes)
        {
            commonVars.runAbort = false;
        }
        return commonVars.runAbort;
    }

    private void abortCSV(CancellationTokenSource cancelSource, CancellationToken cancellationToken)
    {
        if (commonVars.cancelling)
        {
            return;
        }

        if (commonVars.runAbort && !commonVars.userCancelQuery)
        {
            commonVars.userCancelQuery = true;
            commonVars.cancelling = true;
            // AsyncInvoke causes problems where run is aborted before user can respond to dialog.
            Application.Instance.Invoke(() =>
            {
                DialogResult dR = MessageBox.Show("Abort saving of results?", "Abort save?", MessageBoxButtons.YesNo);
                if (dR == DialogResult.Yes)
                {
                    updateStatusLine("Aborting saving of results.");
                    commonVars.runAbort = true;
                }
                else
                {
                    commonVars.runAbort = false;
                }
                commonVars.userCancelQuery = false;
            });

            if (commonVars.runAbort)
            {
                cancelSource.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        commonVars.cancelling = false;
    }

    private void abortRun()
    {
        if (commonVars.cancelling)
        {
            return;
        }

        if (commonVars.runAbort && !commonVars.userCancelQuery)
        {
            Application.Instance.Invoke(() =>
            {
                commonVars.userCancelQuery = true;
                commonVars.cancelling = true;
                DialogResult dR = MessageBox.Show("Abort and save results so far?", "Abort run?", MessageBoxButtons.YesNo);
                commonVars.runAbort = dR == DialogResult.Yes;
                commonVars.userCancelQuery = false;
            });
        }
        commonVars.cancelling = false;
    }

    private static bool abortAllRuns()
    {
        DialogResult dR = MessageBox.Show("Yes: Abort all runs\r\nNo: Abort just this run",
            "Abort all runs?", MessageBoxButtons.YesNo);
        return dR == DialogResult.Yes;
    }

    private void abortRunMT(SimResultPackage resultPackage, CancellationTokenSource cancelSource, CancellationToken cancellationToken)
    {
        if (commonVars.cancelling)
        {
            return;
        }

        if (commonVars.runAbort && !commonVars.userCancelQuery)
        {
            commonVars.userCancelQuery = true;
            commonVars.cancelling = true;
            // AsyncInvoke causes problems where run is aborted before user can respond to dialog.
            Application.Instance.Invoke(() =>
            {
                DialogResult dR = MessageBox.Show("Abort and save results so far?", "Abort run?", MessageBoxButtons.YesNo);
                if (dR == DialogResult.Yes)
                {
                    updateStatusLine("Aborting and saving results so far.");
                    commonVars.runAbort = true;
                }
                else
                {
                    commonVars.runAbort = false;
                }
                commonVars.userCancelQuery = false;
            });

            if (commonVars.runAbort)
            {
                resultPackage.setState(false);
                cancelSource.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        commonVars.cancelling = false;
    }

    private void simRunning()
    {
        commonVars.setSimRunning(true);
    }

    private void simRunningUIFunc()
    {
        simRunningUI();
    }

    private void launchSimulationRun(bool useThreads)
    {
        int mode = getMainSelectedIndex();
        int tabIndex = getSubTabSelectedIndex();
        bool doPASearch = mode == (int)CommonVars.upperTabNames.twoD && tabIndex == (int)CommonVars.twoDTabNames.paSearch;
        string outFile = null;

        bool fileChosen = false;
        if (!doPASearch)
        {
            string fileDialogTitle = "Select summary file to generate";
            string fileDialogFilter = "TXT Files (*.txt)";
            string fileDialogExt = ".txt";
            if (mode == (int)CommonVars.upperTabNames.twoD && commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1 ||
                mode == (int)CommonVars.upperTabNames.Implant && commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1)
            {
                fileDialogTitle = "Select CSV file to generate";
                fileDialogFilter = "CSV Files (*.csv)";
                fileDialogExt = ".csv";
            }

            SaveFileDialog sfd = new()
            {
                Title = fileDialogTitle,
                Filters =
                {
                    new FileFilter(fileDialogFilter, fileDialogExt)
                }
            };
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                string[] tokens = Path.GetFileName(commonVars.projectFileName).Split(new[] { '.' });
                string fileName = "";
                for (int i = 0; i < tokens.Length - 2; i++)
                {
                    fileName += tokens[i] + ".";
                }
                fileName += tokens[^2];
                sfd.FileName = fileName;
            }
            catch (Exception)
            {

            }

            if (sfd.ShowDialog(this) == DialogResult.Ok)
            {
                // Will overwrite if already exists.
                outFile = sfd.FileName;

                // GTK requester doesn't add the extension, so review and add if needed to avoid a crash later.
                if (!outFile.EndsWith(fileDialogExt, StringComparison.CurrentCulture))
                {
                    outFile += fileDialogExt;
                }

                fileChosen = true;
            }
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (doPASearch || !doPASearch && fileChosen)
        {
            // Tidy up cruft prior to run. May not be necessary.
            GC.Collect();

            Task.Run(() =>
                {
                    switch (mode)
                    {
                        case (int)CommonVars.upperTabNames.twoD:
                            entropyControl.EntropyRun(commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases), outFile, useThreads, doPASearch);
                            break;
                        case (int)CommonVars.upperTabNames.Implant:
                            entropyControl.entropyRun_implant(commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases), outFile, useThreads);
                            break;
                    }
                }
            );
        }
        GC.Collect();
    }

    private void monteCarloMultipleThreadEventHandler(object sender, EventArgs e)
    {
        launchSimulationRun(true);
    }

    private void monteCarloSingleThreadEventHandler(object sender, EventArgs e)
    {
        launchSimulationRun(false);
    }

    private void simRunningUI()
    {
        Application.Instance.Invoke(() =>
        {
            // Disable menu items to avoid trouble.
            var menu = Menu;
            var menuItems = menu.Items;
            foreach (MenuItem t in menuItems)
            {
                t.Enabled = false;
            }
            // Activate stop button to enable run abort.
            btn_STOP.Enabled = true;
            // Deactivate run button(s) since we're running a batch task.
            btn_singleCPU.Enabled = false;
            btn_multiCPU.Enabled = false;
            // Block out UI elements
            groupBox_setOutput.Enabled = false;
            groupBox_simSettings.Enabled = false;
            groupBox_GeoEqtn.Enabled = false;
            groupBox_implant.Enabled = false;
            if (commonVars.getReplayMode() == 0)
            {
                groupBox_replay.Enabled = false;
            }
            commentBox.Enabled = false;
            // Some systems don't appear to like the tab control being disabled, so apply some guarding methods in case.
            simRunningTabToFreeze = getSubTabSelectedIndex();
            tabControl_main.SelectedIndexChanged += freezeTabSelection;
            tabControl_2D_simsettings.SelectedIndexChanged += freezeTabSelection;
        });
    }

    private void btnSTOP(object sender, EventArgs e)
    {
        // Set abort flag.
        commonVars.runAbort = true;
    }

    private void btnCancel(object sender, EventArgs e)
    {
        // Set abort flag.
        commonVars.loadAbort = true;
    }

    private void displayMultiThreadWarning()
    {
        Application.Instance.AsyncInvoke(() =>
        {
            lbl_multiThreadResultNote.Text = "Update ~" + CentralProperties.timer_interval / 1000 + "s";
            lbl_multiThreadResultNote.Visible = true;
        });
    }

    private void configProgressBar(int value, int max)
    {
        if (Thread.CurrentThread == commonVars.mainThreadIndex)
        {
            statusProgressBar.Value = value;
            statusProgressBar.MaxValue = max;
        }
        else
        {
            Application.Instance.AsyncInvoke(() =>
            {
                configProgressBar(value, max);
            });
        }
    }

    private void postSimStatusLine()
    {
        int mainIndex = getMainSelectedIndex();
        int tabIndex = getSubTabSelectedIndex();

        if (mainIndex != (int)CommonVars.upperTabNames.twoD)
        {
            return;
        }

        if (tabIndex != (int)CommonVars.twoDTabNames.settings)
        {
            return;
        }

        string updateString = "";
        if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.shape) == 1)
        {
            updateString = "Input";
            if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.results) == 1)
            {
                updateString += " and ";
            }
        }
        if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.results) == 1)
        {
            updateString += "Results";
        }
        if (updateString != "")
        {
            updateString += " drawn.";
        }
        updateStatusLine(updateString);
    }

    private void postSimUI()
    {
        if (Platform.IsGtk)
        {
            Application.Instance.AsyncInvoke(postSimUI_2);
        }
        else
        {
            Application.Instance.Invoke(postSimUI_2);
        }
    }

    private void postSimUI_2()
    {
        // Run complete, deactivate stop button and activate run button(s) again.
        commonVars.setSimRunning(false);
        if (commonVars.getReplayMode() == 1)
        {
            return;
        }

        lbl_multiThreadResultNote.Visible = false;
        btn_STOP.Enabled = false;
        // Restore UI elements
        groupBox_setOutput.Enabled = true;
        groupBox_simSettings.Enabled = true;
        groupBox_GeoEqtn.Enabled = true;
        groupBox_implant.Enabled = true;
        groupBox_replay.Enabled = true;
        startButtonCheck();
        commentBox.Enabled = true;
        // Some systems don't appear to like the tab control being disabled. Remove the guards here.
        tabControl_main.SelectedIndexChanged -= freezeTabSelection;
        tabControl_2D_simsettings.SelectedIndexChanged -= freezeTabSelection;
        // Re-enable menu items.
        var menu = Menu;
        var menuItems = menu.Items;
        foreach (MenuItem t in menuItems)
        {
            t.Enabled = true;
        }
    }

    private void startIndeterminateProgress()
    {
        Application.Instance.Invoke(() =>
        {
            // statusProgressBar.Visible = true;
            statusProgressBar.Indeterminate = true;
        });
    }

    private void startButtonCheck()
    {
        Application.Instance.Invoke(() =>
        {
            btn_multiCPU.Enabled = false;
            btn_singleCPU.Enabled = false;

            if (getMainSelectedIndex() == (int)CommonVars.upperTabNames.Implant)
            {
                btn_multiCPU.Enabled = true;
                btn_singleCPU.Enabled = true;
            }
            else
            {
                if (getMainSelectedIndex() != (int) CommonVars.upperTabNames.twoD ||
                    getSubTabSelectedIndex() != (int) CommonVars.twoDTabNames.settings &&
                    getSubTabSelectedIndex() != (int) CommonVars.twoDTabNames.paSearch)
                {
                    return;
                }

                bool aEqtn = commonVars.getLayerSettings(0).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                bool bEqtn = commonVars.getLayerSettings(CentralProperties.maxLayersForMC / 2).getInt(EntropyLayerSettings.properties_i.enabled) == 1;

                for (int i = 1; i < CentralProperties.maxLayersForMC / 2; i++)
                {
                    aEqtn = aEqtn || commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                    bEqtn = bEqtn || commonVars.getLayerSettings(i + CentralProperties.maxLayersForMC / 2).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                }

                if (aEqtn && bEqtn)
                {
                    btn_singleCPU.Enabled = true;
                    btn_multiCPU.Enabled = true;
                }
                else
                {
                    btn_singleCPU.Enabled = false;
                    btn_multiCPU.Enabled = false;
                }
            }
        });
    }

    private void doSimSettingsCheck()
    {
        Application.Instance.Invoke(() =>
        {
            for (int i = 0; i < comboBox_geoEqtn_Op_2Layer.Length; i++)
            {
                if (commonVars.getLayerSettings(i * 2).getInt(EntropyLayerSettings.properties_i.enabled) == 1 && commonVars.getLayerSettings(i * 2 + 1).getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                {
                    comboBox_geoEqtn_Op_2Layer[i].Enabled = true;
                }
                else
                {
                    comboBox_geoEqtn_Op_2Layer[i].Enabled = false;
                }
            }
        });
    }

    private void suspendSettingsUI()
    {
        settingsUIFrozen = true;
        commonVars.setActiveUI(CommonVars.uiActive.settings, false);
    }

    private void resumeSettingsUI()
    {
        settingsUIFrozen = false;
        commonVars.setActiveUI(CommonVars.uiActive.settings, true);
    }

    private void addSettingsHandlers()
    {
        Application.Instance.Invoke(() =>
        {
            settingsUIFrozen = false;
            commonVars.setActiveUI(CommonVars.uiActive.settings, true);
            comboBox_calcModes.SelectedIndexChanged += entropySettingsChanged;
            checkBox_withinMode.CheckedChanged += entropySettingsChanged;
            checkBox_useShortestEdge.CheckedChanged += entropySettingsChanged;
            cB_displayResults.CheckedChanged += mcPreviewSettingsChanged;
            cB_displayShapes.CheckedChanged += mcPreviewSettingsChanged;
            num_ssNumOfCases.LostFocus += entropySettingsChanged;
            num_ssPrecision.LostFocus += entropySettingsChanged;
            num_cornerSegments.LostFocus += entropySettingsChanged;
            checkBox_debugCalc.CheckedChanged += entropySettingsChanged;
            checkBox_limitCornerPoints.CheckedChanged += entropySettingsChanged;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                comboBox_geoEqtn_Op[i].SelectedIndexChanged += entropySettingsChanged;
            }

            foreach (DropDown t in comboBox_geoEqtn_Op_2Layer)
            {
                t.SelectedIndexChanged += entropySettingsChanged;
            }

            foreach (DropDown t in comboBox_geoEqtn_Op_4Layer)
            {
                t.SelectedIndexChanged += entropySettingsChanged;
            }

            foreach (DropDown t in comboBox_geoEqtn_Op_8Layer)
            {
                t.SelectedIndexChanged += entropySettingsChanged;
            }

            checkBox_perPoly.CheckedChanged += entropySettingsChanged;
            checkBox_aChord.CheckedChanged += entropySettingsChanged;
            checkBox_bChord.CheckedChanged += entropySettingsChanged;
            checkBox_external.CheckedChanged += entropySettingsChanged;
            checkBox_externalCriteria.CheckedChanged += entropySettingsChanged;
            comboBox_externalCriteria1.SelectedIndexChanged += entropySettingsChanged;
            num_externalCriteria1.LostFocus += entropySettingsChanged;
            comboBox_externalCriteria2.SelectedIndexChanged += entropySettingsChanged;
            num_externalCriteria2.LostFocus += entropySettingsChanged;
            comboBox_externalCriteria3.SelectedIndexChanged += entropySettingsChanged;
            num_externalCriteria3.LostFocus += entropySettingsChanged;
            comboBox_externalCriteria4.SelectedIndexChanged += entropySettingsChanged;
            num_externalCriteria4.LostFocus += entropySettingsChanged;
            comboBox_externalTypes.SelectedIndexChanged += entropySettingsChanged;
            checkBox_CSV.CheckedChanged += entropySettingsChanged;
            checkBox_LERMode.CheckedChanged += entropySettingsChanged;
            comboBox_RNG.SelectedIndexChanged += entropySettingsChanged;

            text_server.LostFocus += emailSettingsChanged;
            text_emailAddress.LostFocus += emailSettingsChanged;
            text_emailPwd.LostFocus += emailSettingsChanged;

            num_port.LostFocus += emailSettingsChanged;
            checkBox_EmailCompletion.CheckedChanged += emailSettingsChanged;
            checkBox_perJob.CheckedChanged += emailSettingsChanged;
            checkBox_SSL.CheckedChanged += emailSettingsChanged;

            checkBox_friendlyNumbers.CheckedChanged += miscSettingsChanged;

            button_replay.Click += replayLoadCSV;
            checkBox_replay.CheckedChanged += replayChanged;
            num_replay.LostFocus += replayCaseChanged;
        });
    }

    private void freezeTabSelection(object sender, EventArgs e)
    {
        setMainSelectedIndex((int)CommonVars.upperTabNames.twoD);
        set2DSelectedIndex(simRunningTabToFreeze);
    }

    private void abUI()
    {
        Application.Instance.Invoke(() =>
        {
            // A layer
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                label_geoEqtn_Op[i].Text = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                // Check layer enable states for boolean menu activation states.
                comboBox_geoEqtn_Op[i].Enabled = commonVars.isLayerActive(i);
                label_geoEqtn_Op[i].Enabled = commonVars.isLayerActive(i);
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_2Layer.Length; i++)
            {
                comboBox_geoEqtn_Op_2Layer[i].Enabled = commonVars.interLayerRelationship_2(i);
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_4Layer.Length; i++)
            {
                comboBox_geoEqtn_Op_4Layer[i].Enabled = commonVars.interLayerRelationship_4(i);
            }
        });
    }

    private void xUI()
    {
        Application.Instance.Invoke(() =>
        {
            for (int i = 0; i < comboBox_geoEqtn_Op_8Layer.Length; i++)
            {
                if (
                    (commonVars.getLayerSettings(i * 8).getInt(EntropyLayerSettings.properties_i.enabled) == 1 || commonVars.getLayerSettings(i * 8 + 1).getInt(EntropyLayerSettings.properties_i.enabled) == 1 || commonVars.getLayerSettings(i * 8 + 2).getInt(EntropyLayerSettings.properties_i.enabled) == 1 || commonVars.getLayerSettings(i * 8 + 3).getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                    &&
                    (commonVars.getLayerSettings(i * 8 + 4).getInt(EntropyLayerSettings.properties_i.enabled) == 1 || commonVars.getLayerSettings(i * 8 + 5).getInt(EntropyLayerSettings.properties_i.enabled) == 1 || commonVars.getLayerSettings(i * 8 + 6).getInt(EntropyLayerSettings.properties_i.enabled) == 1 || commonVars.getLayerSettings(i * 8 + 7).getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                )
                {
                    comboBox_geoEqtn_Op_8Layer[i].Enabled = true;
                }
                else
                {
                    comboBox_geoEqtn_Op_8Layer[i].Enabled = false;
                }
            }
        });
    }

    private void entropySettingsChanged(object sender, EventArgs e)
    {
        if (settingsUIFrozen)
        {
            return;
        }

        Application.Instance.Invoke(() =>
        {
            entropySettingsChanged(sender);
            getComment();
        });
        doStatusLine();
        drawSimulationPanelHandler(false);
        updatePASearchUI();
    }

    private async void entropySettingsChanged(object sender)
    {
        // Force base configuration as the table layout lazy evaluation can result in trouble.
        try
        {
            if (comboBox_calcModes.SelectedIndex < 0)
            {
                comboBox_calcModes.SelectedIndex = (int)CommonVars.calcModes.area;
            }
        }
        catch (Exception)
        {
            // Harmless fail in case we're not in the right UI state.
        }
        if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) < 0)
        {
            commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.oType, (int)CommonVars.calcModes.area);
        }

        if (commonVars.getReplayMode() == 0)
        {
            int mainIndex = getMainSelectedIndex();
            int twoDIndex = getSubTabSelectedIndex();

            btn_singleCPU.Enabled = false;

            btn_multiCPU.Enabled = false;
            btn_STOP.Enabled = false;
            // statusProgressBar.Visible = false;
            if (mainIndex != (int)CommonVars.upperTabNames.twoD)
            {
                return;
            }

            if (!openGLErrorReported)
            {
                createVPContextMenu();
                if (twoDIndex != (int)CommonVars.twoDTabNames.paSearch)
                {
                    viewPort.changeSettingsRef(ref mcVPSettings[CentralProperties.maxLayersForMC - 1 + twoDIndex]);
                }
                else
                {
                    // Link to the simulation viewport to make things easier in the PA search mode, since we can hook into the simulation preview cheaply.
                    viewPort.changeSettingsRef(ref mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings]);
                }
            }

            setGlobalUIValues();

            bool preCheck = twoDIndex is not ((int)CommonVars.twoDTabNames.settings or (int)CommonVars.twoDTabNames.paSearch);

            var control = sender as TabControl;
            if (control == tabControl_2D_simsettings && preCheck)
            {
                return;
            }
                
            switch (twoDIndex)
            {
                case (int)CommonVars.twoDTabNames.settings:
                case (int)CommonVars.twoDTabNames.paSearch:
                    startButtonCheck();
                    upperGadgets_panel.Content = simPreviewBox;
                    // statusProgressBar.Visible = true;
                    break;
                case (int)CommonVars.twoDTabNames.DOE:
                    updateStatusLine("Configure DOE settings.");
                    break;
            }

            // Set update needed flag to cause a re-sim if desired.
            // We force the update flag to on and then clear it if the control is not supposed to trigger a re-evaluation.
        }

        bool updateNeeded = true;

        if (commonVars.getReplayMode() == 0)
        {
            var checkBoxControl = sender as CheckBox;
            if (checkBoxControl == checkBox_external || checkBoxControl == checkBox_CSV ||
                checkBoxControl == checkBox_greedyMultiCPU || checkBoxControl == checkBox_LERMode)
            {
                updateNeeded = false;
            }

            var numControl = sender as NumericStepper;
            if (numControl == num_ssNumOfCases)
            {
                updateNeeded = false;
            }

            var ddControl = sender as DropDown;
            if (ddControl == comboBox_RNG)
            {
                updateNeeded = false;
            }

            // Retrieve our settings.
            checkBox_perPoly.Enabled = comboBox_calcModes.SelectedIndex == (int)CommonVars.calcModes.area;

            checkBox_withinMode.Enabled = false;
            checkBox_useShortestEdge.Enabled = false;

            abUI();

            xUI();

            if ((bool)checkBox_debugCalc.Checked)
            {
                commonVars.getSimulationSettings().debugCalc = true;
            }
            else
            {
                commonVars.getSimulationSettings().debugCalc = false;
            }

            if (comboBox_calcModes.SelectedIndex == (int)CommonVars.calcModes.enclosure_spacing_overlap)
            {
                if ((bool)checkBox_withinMode.Checked)
                {
                    if ((bool)checkBox_useShortestEdge.Checked)
                    {
                        commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, (int)CommonVars.spacingCalcModes.enclosure);
                    }
                    else
                    {
                        commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, (int)CommonVars.spacingCalcModes.enclosureOld);
                    }
                }
                else
                {
                    if ((bool)checkBox_useShortestEdge.Checked)
                    {
                        commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, (int)CommonVars.spacingCalcModes.spacing);
                    }
                    else
                    {
                        commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, (int)CommonVars.spacingCalcModes.spacingOld);
                    }
                }
            }

            checkBox_aChord.Enabled = false;
            checkBox_bChord.Enabled = false;

            switch (comboBox_calcModes.SelectedIndex)
            {
                case (int)CommonVars.calcModes.chord:
                {
                    checkBox_aChord.Enabled = true;
                    checkBox_bChord.Enabled = true;
                    commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, (int)CommonVars.chordCalcElements.a + (int)CommonVars.chordCalcElements.b);
                    if (!(bool)checkBox_aChord.Checked && !(bool)checkBox_bChord.Checked)
                    {
                        checkBox_aChord.Checked = true; // force a default if the user is being silly.
                    }
                    if (!(bool)checkBox_aChord.Checked)
                    {
                        commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) - (int)CommonVars.chordCalcElements.a);

                    }
                    if (!(bool)checkBox_bChord.Checked)
                    {
                        commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) - (int)CommonVars.chordCalcElements.b);
                    }

                    break;
                }
                case (int)CommonVars.calcModes.area:
                {
                    commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.oType, (int)CommonVars.calcModes.area);
                    if ((bool)checkBox_perPoly.Checked)
                    {
                        commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, (int)CommonVars.areaCalcModes.perpoly);
                        textBox_userGuidance.Text = "The minimum overlap area will be calculated and reported.";
                    }
                    else
                    {
                        commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.subMode, (int)CommonVars.areaCalcModes.all);
                        textBox_userGuidance.Text = "The total overlap area of all polygons in the layers will be calculated and reported.";
                    }
                    label_AB.Text = "AND";
                    break;
                }
                case (int)CommonVars.calcModes.enclosure_spacing_overlap:
                {
                    comboBox_calcModes.SelectedIndexChanged -= entropySettingsChanged;
                    checkBox_withinMode.Enabled = true;
                    commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.oType, (int)CommonVars.calcModes.enclosure_spacing_overlap);
                    string t;
                    if ((bool)checkBox_withinMode.Checked)
                    {
                        checkBox_useShortestEdge.Enabled = false;
                        commonVars.calcMode_names[(int)CommonVars.calcModes.enclosure_spacing_overlap] = "Compute Enclosure Distribution";
                        label_AB.Text = "Min Enclosure To";
                        t = "enclosure";
                    }
                    else
                    {
                        checkBox_useShortestEdge.Enabled = true;
                        commonVars.calcMode_names[(int)CommonVars.calcModes.enclosure_spacing_overlap] = "Compute Spacing Distribution";
                        label_AB.Text = "Min Space To";
                        t = "spacing";
                    }
                    comboBox_calcModes.SelectedIndex = (int)CommonVars.calcModes.enclosure_spacing_overlap;
                    comboBox_calcModes.SelectedIndexChanged += entropySettingsChanged;
                    textBox_userGuidance.Text = "The system will report the minimum " + t + " value between the shapes, as a single value per case.\r\nNote that overlap cases will report a negative value to indicate that they are opposite to the case being evaluated";
                    break;
                }
            }

            switch (comboBox_calcModes.SelectedIndex)
            {
                case (int)CommonVars.calcModes.chord:
                    commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.oType, (int)CommonVars.calcModes.chord);
                    label_AB.Text = "Min Chord With";
                    textBox_userGuidance.Text = "The system will report multiple chord lengths as : \"AMinTopChord,AMinBottomChord,BMinLeftChord,BMinRightChord\".\r\n\r\nMissing chords or invalid cases for evaluation are reported as 0.0\r\nChords not requested by the user are shown as N/A in the output file.\r\n\r\nShape A is defined by geometric equation A; B is geometric equation B.\r\n\r\nMajor axis : Orient shape A horizontally and B vertically else results will be reversed (top/bottom <> left/right)";
                    break;
                case (int)CommonVars.calcModes.angle:
                    commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.oType, (int)CommonVars.calcModes.angle);
                    label_AB.Text = "Min Angle With";
                    textBox_userGuidance.Text = "The minimum intersection angle will be reported, in degrees. A lack of intersection will yield a 180-degree value in the output";
                    break;
            }

            commonVars.getSimulationSettings().setResolution(Convert.ToDouble(num_ssPrecision.Value));
            commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.cSeg, Convert.ToInt32(num_cornerSegments.Value));

            if ((bool)checkBox_limitCornerPoints.Checked)
            {
                commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.optC, 1);
            }
            else
            {
                commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.optC, 0);
            }

            commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.nCases, Convert.ToInt32(num_ssNumOfCases.Value));

            if ((bool)checkBox_greedyMultiCPU.Checked)
            {
                commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.greedy, 1);
            }
            else
            {
                commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.greedy, 0);
            }

            if ((bool)checkBox_linkCDUVariation.Checked)
            {
                commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.linkCDU, 1);
            }
            else
            {
                commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.linkCDU, 0);
            }

            if ((bool)checkBox_LERMode.Checked)
            {
                commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.ler, 1);
            }
            else
            {
                commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.ler, 0);
            }

            // Store our operators in simulation settings.
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                commonVars.getSimulationSettings().setOperatorValue(EntropySettings.properties_o.layer, i, comboBox_geoEqtn_Op[i].SelectedIndex);
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_2Layer.Length; i++)
            {
                commonVars.getSimulationSettings().setOperatorValue(EntropySettings.properties_o.twoLayer, i, comboBox_geoEqtn_Op_2Layer[i].SelectedIndex);
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_4Layer.Length; i++)
            {
                commonVars.getSimulationSettings().setOperatorValue(EntropySettings.properties_o.fourLayer, i, comboBox_geoEqtn_Op_4Layer[i].SelectedIndex);
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_8Layer.Length; i++)
            {
                commonVars.getSimulationSettings().setOperatorValue(EntropySettings.properties_o.eightLayer, i, comboBox_geoEqtn_Op_8Layer[i].SelectedIndex);
            }

            bool extActive = (bool)checkBox_external.Checked;
            comboBox_externalTypes.Enabled = extActive;
            checkBox_externalCriteria.Enabled = extActive;

            if (extActive)
            {
                commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.external, 1);
                commonVars.getImplantSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalType, (int)CommonVars.external_Type.svg);
            }
            else
            {
                commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.external, 0);
            }

            commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalType, comboBox_externalTypes.SelectedIndex);

            if (!extActive)
            {
                checkBox_externalCriteria.Checked = false;
            }

            bool extCriteriaActive = (bool)checkBox_externalCriteria.Checked;

            commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalCriteria, extCriteriaActive ? 1:0);

            comboBox_externalCriteria1.Enabled = extCriteriaActive;
            int extCrit1 = comboBox_externalCriteria1.SelectedIndex;
            commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalCritCond1, extCrit1);

            bool enable_numExtCrit1 = extCrit1 > 0;
            num_externalCriteria1.Enabled = extCriteriaActive && enable_numExtCrit1;

            commonVars.getSimulationSettings_nonSim().setDecimal(EntropySettings_nonSim.properties_d.externalCritCond1, Convert.ToDecimal(num_externalCriteria1.Value));

            bool multiFieldResult = comboBox_calcModes.SelectedIndex == (int)CommonVars.calcModes.chord;

            comboBox_externalCriteria2.Enabled = extCriteriaActive && multiFieldResult;
            int extCrit2 = comboBox_externalCriteria2.SelectedIndex;
            commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalCritCond2, extCrit2);

            bool enable_numExtCrit2 = extCrit2 > 0;
            num_externalCriteria2.Enabled = extCriteriaActive && enable_numExtCrit2 && multiFieldResult;

            commonVars.getSimulationSettings_nonSim().setDecimal(EntropySettings_nonSim.properties_d.externalCritCond2, Convert.ToDecimal(num_externalCriteria2.Value));

            comboBox_externalCriteria3.Enabled = extCriteriaActive && multiFieldResult;
            int extCrit3 = comboBox_externalCriteria3.SelectedIndex;
            commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalCritCond3, extCrit3);

            bool enable_numExtCrit3 = extCrit3 > 0;
            num_externalCriteria3.Enabled = extCriteriaActive && enable_numExtCrit3 && multiFieldResult;

            commonVars.getSimulationSettings_nonSim().setDecimal(EntropySettings_nonSim.properties_d.externalCritCond3, Convert.ToDecimal(num_externalCriteria3.Value));

            comboBox_externalCriteria4.Enabled = extCriteriaActive && multiFieldResult;
            int extCrit4 = comboBox_externalCriteria4.SelectedIndex;
            commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.externalCritCond4, extCrit4);

            bool enable_numExtCrit4 = extCrit4 > 0;
            num_externalCriteria4.Enabled = extCriteriaActive && enable_numExtCrit4 && multiFieldResult;

            commonVars.getSimulationSettings_nonSim().setDecimal(EntropySettings_nonSim.properties_d.externalCritCond4, Convert.ToDecimal(num_externalCriteria4.Value));

            if ((bool)checkBox_CSV.Checked)
            {
                commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.csv, 1);
            }
            else
            {
                commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.csv, 0);
            }

            commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.rngType, comboBox_RNG.SelectedIndex);

            // Ensure we set controls according to run state
            if (commonVars.isSimRunning())
            {
                btn_singleCPU.Enabled = false;
                btn_multiCPU.Enabled = false;
                btn_STOP.Enabled = true;
            }
        }

        // Make a single run when on the right tab, to populate the preview structures, and only if simulation not running.
        if (!commonVars.isSimRunning())
        {
            if (updateNeeded)
            {
                // Set indeterminate progress bar to show application is thinking.
                await Application.Instance.InvokeAsync(() =>
                    {
                        statusProgressBar.Indeterminate = true;
                    }
                );
                entropyControl.update(commonVars);
                // Spawn and await background task in case the calculation is long-running.
                // Avoids blocking the UI.
                // Freeze the UI to avoid cascading runs.
                simRunningUIFunc();
                btn_STOP.Enabled = false; // No ability to abort a single case, so don't mislead the user. They are along for the ride.

                try
                {
                    await Task.Run(() =>
                    {
                        updateStatusLine("Calculating - please wait");
                        if (commonVars.getReplayMode() == 0)
                        {
                            entropyControl.EntropyRun(numberOfCases: 1, csvFile: null, useThreads: false, doPASearch: false);
                        }
                        else
                        {
                            entropyControl.EntropyRun(numberOfCases: 1, csvFile: null, useThreads: false, doPASearch: false, setJobSettings: true, loadedJobSettings: simReplay.getChaosSettings(), replayRow_: simReplay.getValue(Replay.properties_i.row), replayCol_: simReplay.getValue(Replay.properties_i.col));
                        }
                    });
                }
                catch (Exception)
                {
                    // Handle any task cancelled exception without crashing the tool. The cancellation may occur due to close of the tool whilst evaluation is underway.
                }
                postSimUI();
                // Set the progress bar back to a fixed state
                await Application.Instance.InvokeAsync(() =>
                    {
                        statusProgressBar.Indeterminate = false;
                    }
                );
                postSimStatusLine();
            }
        }
        if (commonVars.getReplayMode() == 0)
        {
            uiFollowChanges();
        }
    }

    private void setReplayControls()
    {
        groupBox_replay.Enabled = true;
        button_replay.Enabled = true;
        checkBox_replay.Enabled = simReplay.isValid();
        num_replay.Enabled = simReplay.isValid();

        if ((bool)checkBox_replay.Checked && !simReplay.isValid())
        {
            checkBox_replay.Checked = false;
            // Will trigger event handler and update setup.
        }
    }

    private void replaySuspend()
    {
        groupBox_replay.Enabled = false;
        num_replay.Enabled = false;
        checkBox_replay.Enabled = false;
        button_replay.Enabled = true;
    }

    private void replayResume()
    {
        groupBox_replay.Enabled = true;
        num_replay.Enabled = true;
        checkBox_replay.Enabled = true;
        button_replay.Enabled = false;
    }

    private void replayLoadCSV(object sender, EventArgs e)
    {
        OpenFileDialog ofd = new()
        {
            Title = "Select CSV file to Load",
            MultiSelect = false,
            Filters =
            {
                new FileFilter("CSV Files (*.csv)", ".csv")
            }
        };
        if (ofd.ShowDialog(ParentWindow) == DialogResult.Ok)
        {
            try
            {
                simReplay.replay_loadCSV(ofd.FileName);
            }
            catch (Exception)
            {
                simReplay.reset();
            }
        }

        // Configure numeric control.
        if (simReplay.getValue(Replay.properties_i.max) < 1)
        {
            num_replay.MinValue = 0;
            num_replay.Value = 0;
            num_replay.MaxValue = 0;
        }
        else
        {
            num_replay.MaxValue = simReplay.getValue(Replay.properties_i.max);
            num_replay.Value = 1;
            num_replay.MinValue = 1;
        }

        setReplayControls();
    }

    private void replayChanged(object sender, EventArgs e)
    {
        replayChanged();
    }

    private void replayChanged()
    {
        if (replayUIFrozen)
        {
            return;
        }
        replayUIFrozen = true;

        if ((bool)checkBox_replay.Checked)
        {
            enableReplay();
        }
        else
        {
            disableReplay();
        }

        replayUIFrozen = false;
    }

    private void enableReplay()
    {
        commonVars.setReplayMode(1);
        simRunningUI();
        replayResume();
        replay();
    }

    private void disableReplay()
    {
        postSimUI();
        commonVars.setReplayMode(0);
        replaySuspend();
        entropySettingsChanged(null);
    }

    private void replayCaseChanged(object sender, EventArgs e)
    {
        if (replayUIFrozen)
        {
            return;
        }
        replayUIFrozen = true;
        replay();
        replayUIFrozen = false;
    }

    private void replay()
    {
        if (!simReplay.isValid())
        {
            return;
        }

        if (commonVars.getReplayMode() == 1)
        {
            // Retrieve chaos values from replay.
            simReplay.getState(Convert.ToInt32(num_replay.Value));

            // Update preview with replay state.
            entropySettingsChanged(null);
        }

        replayChanged();
    }
}