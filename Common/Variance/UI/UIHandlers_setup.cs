using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private void setupGUI()
    {
        Application.Instance.Invoke(() =>
        {
            tabControl_main.SelectedIndexChanged += mainTabChanged;
            tabControl_2D_simsettings.SelectedIndexChanged += subTabChanged;

            entropyControl = new Entropy(ref varianceContext.vc, commonVars)
            {
                updateStatus = updateStatusLine,
                configProgress = configProgressBar,
                stepProgress = updateProgressBar,
                abortCheckFunc = abortCheck,
                clearAbortFlagFunc = clearAbortFlag,
                postSimUIFunc = postSimUI,
                updateSimUIFunc = updateSimUIST,
                updateSimUIMTFunc = updateSimUIMT,
                updateProgressBarFunc = updateProgressBar,
                updateImplantSimUIFunc = updateImplantSimUIST,
                updateImplantSimUIMTFunc = updateImplantSimUIMT,
                abortAllRunsFunc = abortAllRuns,
                abortCSVFunc = abortCSV,
                abortRunFunc = abortRun,
                abortRunFuncMT = abortRunMT,
                multithreadWarningFunc = displayMultiThreadWarning,
                simRunningFunc = simRunning,
                simRunningUIFunc = simRunningUIFunc,
                implantSimRunningUIFunc = simRunningUIFunc,
                postSimPASearchUIFunc = paSearchUI_showResults
            };

            // Set listbox defaults on the settings tab.
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                label_geoEqtn_Op[i].Enabled = false;
                comboBox_geoEqtn_Op[i].Enabled = false;
            }

            foreach (var t in comboBox_geoEqtn_Op_2Layer)
            {
                t.Enabled = false;
            }

            foreach (var t in comboBox_geoEqtn_Op_4Layer)
            {
                t.Enabled = false;
            }

            foreach (var t in comboBox_geoEqtn_Op_8Layer)
            {
                t.Enabled = false;
            }

            // Chord selection modes.

            checkBox_aChord.Enabled = false;
            checkBox_bChord.Enabled = false;

            btn_singleCPU.Enabled = false;

            btn_Cancel.Enabled = false;

            btn_multiCPU.Enabled = false;
            btn_STOP.Enabled = false;
            // statusProgressBar.Visible = false;

            // Configure GDS DOE UI defaults
            btn_useCSViDRM.Enabled = false;
            btn_useCSVQuilt.Enabled = false;
            radioButton_allTiles.Checked = true;
            textBox_useSpecificTiles.Enabled = false;
            num_DOESCCol.Enabled = false;
            num_DOESCRow.Enabled = false;

            updateUIColors();

            updateUtilsValues();

            commonVars.storage.setLayerSettings = setLayerSettings;
            commonVars.storage.resumeUI = resumeUIFromStorage;
            commonVars.storage.suspendSettingsUI = suspendSettingsUI;
            commonVars.storage.suspendDOESettingsUI = suspendDOESettingsUI;
            commonVars.storage.updateSettingsUIFromSettings = updateSettingsUIFromSettings;
            commonVars.storage.updateDOESettingsUIFromSettings = updateDOESettingsUIFromSettings;
            commonVars.storage.loadLayout = layoutLoad;
            commonVars.storage.viewportLoad = setViewportCamera;
            commonVars.storage.viewportSave = getViewportCamera;
            commonVars.storage.suspendImplantUI = suspendImplantUI;
            commonVars.storage.updateImplantUIFromSettings = updateImplantUIFromSettings;
            commonVars.storage.implantViewportLoad = setImplantViewportCamera;
            commonVars.storage.implantViewportSave = getImplantViewportCamera;

            // Push the UI to align to our settings at startup.
            updateSettingsUIFromSettings();
            updateDOESettingsUIFromSettings();

            for (int i = 0; i < cB_bg.Length; i++)
            {
                cB_bg[i].Checked = false;
                cB_bg[i].Enabled = false;
                cB_omit[i].Checked = false;
                cB_omit[i].Enabled = false;
            }

            updateStatusLine(CentralProperties.productName + " " + CentralProperties.version);
        });
    }

    private void addAllUIHandlers()
    {
        Application.Instance.Invoke(() =>
        {
            addLayerHandlers();
            addSettingsHandlers();
            addDOESettingsHandlers();
            addOmitHandlers();
            addBGHandlers();
            addPrefsHandlers();
        });
    }

    private void addOmitHandlers()
    {
        for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
        {
            cB_omit[i].CheckedChanged += omitLayerCheckboxChanged;
        }
    }

    private void addBGHandlers()
    {
        for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
        {
            cB_bg[i].CheckedChanged += bgLayerCheckboxChanged;
        }
    }

    private void addPrefsHandlers()
    {
        Application.Instance.Invoke(() =>
        {
            lbl_Layer1Color.MouseDoubleClick += layerColorChange;
            lbl_Layer2Color.MouseDoubleClick += layerColorChange;
            lbl_Layer3Color.MouseDoubleClick += layerColorChange;
            lbl_Layer4Color.MouseDoubleClick += layerColorChange;
            lbl_Layer5Color.MouseDoubleClick += layerColorChange;
            lbl_Layer6Color.MouseDoubleClick += layerColorChange;
            lbl_Layer7Color.MouseDoubleClick += layerColorChange;
            lbl_Layer8Color.MouseDoubleClick += layerColorChange;
            lbl_Layer9Color.MouseDoubleClick += layerColorChange;
            lbl_Layer10Color.MouseDoubleClick += layerColorChange;
            lbl_Layer11Color.MouseDoubleClick += layerColorChange;
            lbl_Layer12Color.MouseDoubleClick += layerColorChange;
            lbl_Layer13Color.MouseDoubleClick += layerColorChange;
            lbl_Layer14Color.MouseDoubleClick += layerColorChange;
            lbl_Layer15Color.MouseDoubleClick += layerColorChange;
            lbl_Layer16Color.MouseDoubleClick += layerColorChange;

            lbl_ss1Color.MouseDoubleClick += layerColorChange;
            lbl_ss2Color.MouseDoubleClick += layerColorChange;
            lbl_ss3Color.MouseDoubleClick += layerColorChange;

            lbl_minorGridColor.MouseDoubleClick += layerColorChange;
            lbl_majorGridColor.MouseDoubleClick += layerColorChange;
            lbl_vpbgColor.MouseDoubleClick += layerColorChange;
            lbl_axisColor.MouseDoubleClick += layerColorChange;

            lbl_Result1Color.MouseDoubleClick += layerColorChange;
            lbl_Result2Color.MouseDoubleClick += layerColorChange;
            lbl_Result3Color.MouseDoubleClick += layerColorChange;
            lbl_Result4Color.MouseDoubleClick += layerColorChange;

            lbl_enabledColor.MouseDoubleClick += layerColorChange;

            lbl_implantMinColor.MouseDoubleClick += layerColorChange;
            lbl_implantMeanColor.MouseDoubleClick += layerColorChange;
            lbl_implantMaxColor.MouseDoubleClick += layerColorChange;
            lbl_implantResistColor.MouseDoubleClick += layerColorChange;

            bool emailOK = validateEmailSettings();
            checkBox_EmailCompletion.Enabled = emailOK;
            checkBox_perJob.Enabled = emailOK;
            button_emailTest.Enabled = emailOK;
            button_emailTest.Click += emailTest;
            checkBox_geoCore_enableCDVariation.CheckedChanged += preferencesChange;
            checkBox_geoCore_tileLayerPreview.CheckedChanged += preferencesChange;
            checkBox_OGLAA.CheckedChanged += preferencesChange;
            checkBox_OGLFill.CheckedChanged += preferencesChange;
            checkBox_OGLPoints.CheckedChanged += preferencesChange;
            num_fgOpacity.LostFocus += preferencesChange;
            num_bgOpacity.LostFocus += preferencesChange;
            num_zoomSpeed.LostFocus += preferencesChange;

            btn_resetColors.Click += resetColors;
        });
    }

    private void clearAbortFlag()
    {
        commonVars.runAbort = false;
        commonVars.loadAbort = false;
    }
}