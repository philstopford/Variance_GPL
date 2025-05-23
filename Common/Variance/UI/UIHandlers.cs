﻿using System;
using Eto.Drawing;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private void aboutMe(object sender, EventArgs e)
    {
        if (aboutBox is not {Visible: true})
        {
            string creditText = "Version " + commonVars.version + ", " +
                                "© " + commonVars.author + " 2014-2024" + "\r\n\r\n";
            creditText += varianceContext.vc.licenceName;
            creditText += "\r\n\r\n";
            creditText += "Libraries used:\r\n";
            creditText += "  Eto.Forms : UI framework\r\n\thttps://github.com/picoe/Eto\r\n";
            creditText += "  Eto.Veldrid : Eto Veldrid viewport\r\n\thttps://github.com/picoe/Eto.Veldrid\r\n";
            creditText += "  DesignLibs : Design libraries\r\n\thttps://github.com/philstopford/DesignLibs_GPL\r\n";
            creditText += "  Clipper2 : geometry processing\r\n\thttps://github.com/AngusJohnson/Clipper2\r\n";
            creditText += "  KD-Sharp : for spacing/enclosure\r\n\thttps://code.google.com/p/kd-sharp/\r\n";
            creditText += "  LibTessDotNet : for Delauney triangulation (tone inversion of n polygons)\r\n\thttps://github.com/speps/LibTessDotNet\r\n";
            creditText += "  Mersenne Twister : \r\n\thttp://www.centerspace.net/resources/free-stuff/mersenne-twister\r\n";
            creditText += "  ExpressionParser : \r\n\thttps://github.com/plundin/Mathparser\r\n";
            creditText += "  MiscUtil : \r\n\thttps://github.com/loory/MiscUtil\r\n";
            aboutBox = new CreditsScreen(this, creditText);
        }
        Point location = new(Location.X + (Width - aboutBox.Width) / 2,
            Location.Y + (Height - aboutBox.Height) / 2);
        aboutBox.Location = location;
        aboutBox.Show();
    }

    private void quit(object sender, EventArgs e)
    {
        savePrefs();
        Application.Instance.Quit();
    }

    private void quitHandler(object sender, EventArgs e)
    {
        savePrefs();
    }

    private void suspendUIHandlers()
    {
        Application.Instance.Invoke(() =>
        {
            globalUIFrozen = true;
            suspendImplantUI();
            suspendDOESettingsUI();
            suspendSettingsUI();
            suspendLayerUI_exp();
        });
    }

    private void resumeUIHandlers()
    {
        Application.Instance.Invoke(() =>
        {
            globalUIFrozen = false;
            resumeLayerUI_exp();
            resumeSettingsUI();
            resumeDOESettingsUI();
            resumeImplantUI();
            saveEnabler();
            uiFollowChanges();
        });
    }

    private void mcPreviewSettingsChanged(object sender, EventArgs e)
    {
        int tabIndex = getSubTabSelectedIndex();
        if (tabIndex is (int)CommonVars.twoDTabNames.settings or (int)CommonVars.twoDTabNames.paSearch)
        {
            upperGadgets_panel.Content = simPreviewBox;
        }

        commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.results,
            cB_displayResults.Checked == true ? 1 : 0);
        commonVars.getSimulationSettings_nonSim().setInt(EntropySettings_nonSim.properties_i.shape,
            cB_displayShapes.Checked == true ? 1 : 0);

        doStatusLine();
        drawSimulationPanelHandler(false);

        if (!commonVars.isSimRunning())
        {
            entropyControl.update(commonVars);
        }
    }

    private void mainTabChanged(object sender, EventArgs e)
    {
        btn_Run.Enabled = false;
        btn_STOP.Enabled = false;
        
        upperGadgets_panel.Content = new Panel();

        commentBox.Enabled = false;

        int mainIndex = getMainSelectedIndex();

        if (mainIndex == (int)CommonVars.upperTabNames.Implant)
        {
            updateImplantUIFromSettings();

            upperGadgets_panel.Content = simPreviewBox;

            commentBox.Enabled = true;

            viewPort.changeSettingsRef(ref otkVPSettings_implant);

        }
        else
        {
            subTabChanged();
        }

        if (mainIndex == (int)CommonVars.upperTabNames.twoD)
        {
            upperGadgets_panel.Content = upperGadgets_table;
            commentBox.Enabled = true;
        }

        bool showSomeUIStuff = mainIndex is (int)CommonVars.upperTabNames.Utilities or (int)CommonVars.upperTabNames.oneD;
        showSomeUIStuff = !showSomeUIStuff;

        // Hide right splitter in specific tabs.
        rightSplitter.Visible = showSomeUIStuff;

        int subTabIndex = getSubTabSelectedIndex();
        if (subTabIndex == (int)CommonVars.twoDTabNames.DOE)
        {
            showSomeUIStuff = false;
        }

        if (mainIndex == (int)CommonVars.upperTabNames.Implant)
        {
            showSomeUIStuff = true;
        }
        statusProgressBar.Visible = showSomeUIStuff;
        commentBox.Visible = showSomeUIStuff;
        commentBox.Enabled = showSomeUIStuff;

        if (subTabIndex == (int)CommonVars.twoDTabNames.layer)
        {
            showSomeUIStuff = false;
        }
        if (mainIndex == (int)CommonVars.upperTabNames.Implant)
        {
            showSomeUIStuff = true;
        }
        btn_STOP.Visible = showSomeUIStuff;
        btn_Run.Visible = showSomeUIStuff;
        btn_Cancel.Visible = showSomeUIStuff;

        if (mainIndex == (int)CommonVars.upperTabNames.Utilities)
        {
            updateUtilsValues();
        }
        getComment();
        startButtonCheck();
    }

    private void subTabChanged(object sender, EventArgs e)
    {
        subTabChanged();
    }

    private void subTabChanged()
    {
        int subTabIndex = getSubTabSelectedIndex();

        
        switch (subTabIndex)
        {
            case (int)CommonVars.twoDTabNames.layer:
                listbox_change();
                do2DLayerUI(getSelectedLayerIndex(), updateUI: true);
                break;
            case (int)CommonVars.twoDTabNames.settings:
                updateSettingsUIFromSettings();
                break;
            case (int)CommonVars.twoDTabNames.DOE:
                updateDOESettingsUIFromSettings();
                break;
        }
        
        bool showSomeUIStuff = true;
        lbl_simPreviewZoom.Visible = showSomeUIStuff;
        lbl_viewportPos.Visible = showSomeUIStuff;
        num_viewportZoom.Visible = showSomeUIStuff;
        num_viewportX.Visible = showSomeUIStuff;
        num_viewportY.Visible = showSomeUIStuff;

        if (subTabIndex == (int)CommonVars.twoDTabNames.DOE)
        {
            showSomeUIStuff = false;
        }
        statusProgressBar.Visible = showSomeUIStuff;
        commentBox.Visible = showSomeUIStuff;
        commentBox.Enabled = showSomeUIStuff;

        if (subTabIndex == (int)CommonVars.twoDTabNames.layer)
        {
            showSomeUIStuff = false;
        }
        btn_STOP.Visible = showSomeUIStuff;
        btn_Run.Visible = showSomeUIStuff;
        btn_Cancel.Visible = showSomeUIStuff;
        
        doeSettingsChanged();
        entropySettingsChanged(null);
        mcPreviewSettingsChanged(null, EventArgs.Empty);
        int vpindex = getSubTabSelectedIndex();
        if (vpindex == (int)CommonVars.twoDTabNames.DOE)
        {
            vpindex = CentralProperties.maxLayersForMC + 1; // layers and simulation viewport.
        }
        else
        {
            vpindex = vpindex != (int)CommonVars.twoDTabNames.layer ? CentralProperties.maxLayersForMC : getSelectedLayerIndex();
        }
        viewPort.changeSettingsRef(ref mcVPSettings[vpindex]);
    }

    private void uiFollowChanges()
    {
        commonVars.checkChanged();
        revertSim.Enabled = false;

        string title = commonVars.titleText;

        if (commonVars.projectFileName != "")
        {
            title += " - " + commonVars.projectFileName;

            if (commonVars.isChanged())
            {
                title += " *";
                revertSim.Enabled = true;
            }
        }

        Application.Instance.Invoke(() =>
        {
            Title = title;
        });

        updateLBContextMenu();
        pasteLayer.Enabled = commonVars.isCopyPrepped();
    }

    private void mainTabHandler(object sender, EventArgs e)
    {
        setGlobalUIValues();
        doStatusLine();
    }

    private void doStatusLine()
    {
        int mainIndex = getMainSelectedIndex();
        int subTabIndex = getSubTabSelectedIndex();

        switch (mainIndex)
        {
            case (int)CommonVars.upperTabNames.Implant:
                if (!openGLErrorReported)
                {
                    createVPContextMenu();
                    viewPort.changeSettingsRef(ref otkVPSettings_implant);
                }
                updateStatusLine("Configure simulation settings");
                break;
            case (int)CommonVars.upperTabNames.twoD:
                switch (subTabIndex)
                {
                    case (int)CommonVars.twoDTabNames.settings:
                        updateStatusLine("Configure simulation settings");
                        break;
                    case (int)CommonVars.twoDTabNames.paSearch:
                        updateStatusLine("Configure PA search settings.");
                        break;
                    case (int)CommonVars.twoDTabNames.DOE:
                        updateStatusLine("Configure DOE settings.");
                        break;
                    default:
                        updateStatusLine("Configure layer settings");
                        break;
                }
                break;
            case (int)CommonVars.upperTabNames.Utilities:
                updateStatusLine("Miscellaneous utilities");
                break;
            case (int)CommonVars.upperTabNames.oneD:
                updateStatusLine("Basic calculation");
                break;
        }
    }

    private void setGlobalUIValues()
    {
        Application.Instance.Invoke(() =>
        {
            globalUIFrozen = true;

            try
            {
                // Set the global UI elements (layer, zoom, etc.) here, if appropriate.
                // globalUIFrozen being set should avoid issues with event handlers firing.
                int mainIndex = getMainSelectedIndex();
                int settingsIndex = getSelectedLayerIndex() + getSubTabSelectedIndex();

                bool showCheckboxes = true;
                if (mainIndex != (int)CommonVars.upperTabNames.twoD)
                {
                    showCheckboxes = false;
                }
                else
                {
                    if (settingsIndex >= CentralProperties.maxLayersForMC)
                    {
                        showCheckboxes = false;
                    }
                }

                if (showCheckboxes)
                {
                    switch (getSubTabSelectedIndex())
                    {
                        case (int)CommonVars.twoDTabNames.layer:
                            upperGadgets_panel.Content = upperGadgets_table;
                            setBGLayerCheckboxes(settingsIndex);
                            break;
                        case (int)CommonVars.twoDTabNames.DOE:
                            upperGadgets_panel.Content = new Panel();
                            break;
                        default:
                            upperGadgets_panel.Content = simPreviewBox;
                            break;
                    }
                }
                else
                {
                    upperGadgets_panel.Content = new Panel();
                }

                // Set the zoom level from the viewport settings.
                if (mainIndex == (int)CommonVars.upperTabNames.Implant)
                {
                    double[] vals = getImplantViewportCamera();
                    num_viewportZoom.Value = vals[2];
                    num_viewportX.Value = vals[0];
                    num_viewportY.Value = vals[1];
                }
                if (mainIndex != (int)CommonVars.upperTabNames.twoD)
                {
                    double[] vals = getViewportCamera(settingsIndex);
                    num_viewportZoom.Value = vals[2];
                    num_viewportX.Value = vals[0];
                    num_viewportY.Value = vals[1];
                }
            }
            catch
            {
                // ignored
            }
            globalUIFrozen = false;
        });
    }

    private void suspendImplantUI()
    {
        implantUIFrozen = true;
        commonVars.setActiveUI(CommonVars.uiActive.implant, true);
    }

    private void resumeImplantUI()
    {
        implantUIFrozen = false;
        commonVars.setActiveUI(CommonVars.uiActive.implant, false);
    }

    private void commentChanged(object sender, EventArgs e)
    {
        if (globalUIFrozen)
        {
            return;
        }

        int mainIndex = getMainSelectedIndex();
        int subTabIndex = getSubTabSelectedIndex();

        switch (mainIndex)
        {
            case (int)CommonVars.upperTabNames.Implant:
                commonVars.getImplantSettings().setComment(commentBox.Text);
                break;
            case (int)CommonVars.upperTabNames.twoD:
            {
                switch (subTabIndex)
                {
                    case < (int)CommonVars.twoDTabNames.settings:
                        commonVars.getLayerSettings(getSelectedLayerIndex()).setString(EntropyLayerSettings.properties_s.comment, commentBox.Text);
                        break;
                    case (int)CommonVars.twoDTabNames.settings:
                        commonVars.getSimulationSettings_nonSim().setString(EntropySettings_nonSim.properties_s.comment, commentBox.Text);
                        break;
                    case (int)CommonVars.twoDTabNames.paSearch:
                        commonVars.getSimulationSettings_nonSim().setString(EntropySettings_nonSim.properties_s.paComment, commentBox.Text);
                        break;
                    case (int)CommonVars.twoDTabNames.DOE:
                        commonVars.getSimulationSettings().getDOESettings().setString(DOESettings.properties_s.comment, commentBox.Text);
                        break;
                }

                break;
            }
        }
    }

    private void getComment()
    {
        Application.Instance.Invoke(() =>
        {
            globalUIFrozen = true;
            commentBox.Text = "";

            int mainIndex = getMainSelectedIndex();
            int subTabIndex = getSubTabSelectedIndex();

            switch (mainIndex)
            {
                case (int)CommonVars.upperTabNames.Implant:
                    commentBox.Text = commonVars.getImplantSettings_nonSim().getString(EntropySettings_nonSim.properties_s.comment);
                    break;
                case (int)CommonVars.upperTabNames.twoD:
                {
                    switch (subTabIndex)
                    {
                        case < (int)CommonVars.twoDTabNames.settings:
                            commentBox.Text = commonVars.getLayerSettings(getSelectedLayerIndex()).getString(EntropyLayerSettings.properties_s.comment);
                            break;
                        case (int)CommonVars.twoDTabNames.settings:
                            commentBox.Text = commonVars.getSimulationSettings_nonSim().getString(EntropySettings_nonSim.properties_s.comment);
                            break;
                        case (int)CommonVars.twoDTabNames.paSearch:
                            commentBox.Text = commonVars.getSimulationSettings_nonSim().getString(EntropySettings_nonSim.properties_s.paComment);
                            break;
                        case (int)CommonVars.twoDTabNames.DOE:
                            commentBox.Text = commonVars.getSimulationSettings().getDOESettings().getString(DOESettings.properties_s.comment);
                            break;
                    }

                    break;
                }
            }

            globalUIFrozen = false;
        });
    }

    private void collapseExpanders(object sender, EventArgs e)
    {
        changeExpanders(false);
    }

    private void expandExpanders(object sender, EventArgs e)
    {
        changeExpanders(true);
    }

    private void changeExpanders(bool toState)
    {
        gadgets_exp.Expanded = toState;
        expander_gadgets.Expanded = toState;
        twoD_LayerUISetup_biasEtch_expanders(toState);
        twoD_LayerUISetup_boolean_expanders(toState);
        twoD_LayerUISetup_geoCore_expanders(toState);
        twoD_LayerUISetup_litho_expanders(toState);
        twoD_LayerUISetup_layoutOriginParameters_expanders(toState);
        twoD_LayerUISetup_subShape_expanders(toState);
    }
}
