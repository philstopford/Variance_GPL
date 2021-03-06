﻿using Eto.Forms;
using VeldridEto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Eto.Veldrid;

namespace Variance
{
    public partial class MainForm
    {
        // Using this for MVVM binding to comboboxes.
        // A few of these won't change, but collecting them all together for consistent referencing/binding.
        // ObservableCollection used because it sends notification when contents changed, unlike List.
        // This allows for updates to the UI.
        public class UIStringLists
        {
            public ObservableCollection<string>[] subShapeList { get; set; }

            public List<string> shapes { get; set; }

            public List<string> tipLocs { get; set; }

            public List<string> subShapePos { get; set; }

            public ObservableCollection<string> calcModes { get; set; }

            public List<string> externalTypeList { get; set; }

            public List<string> externalFilterList { get; set; }

            public List<string> rngTypeList { get; set; }
            public List<string> noiseTypeList { get; set; }

            public List<string> polyFillList { get; set; }

            public ObservableCollection<string>[] geoCoreStructureList { get; set; }
            public ObservableCollection<string>[] geoCoreLDList { get; set; }

            public ObservableCollection<string> geoCoreStructureList_exp { get; set; }
            public ObservableCollection<string> geoCoreLDList_exp { get; set; }

            public ObservableCollection<string> subShapesList_exp { get; set; }

            public ObservableCollection<string> layerNames { get; set; }

            public List<string> fallOffList { get; set; }

            public List<string> notList { get; set; }
            public List<string> booleanList { get; set; }
            public List<string> openGLMode { get; set; }

            public ObservableCollection<string> rngMapping { get; set; }
        }

        int viewportSize = 484;

        bool[] geoGBVisible, subShapeGBVisible, booleanGBVisible;

        // Context menu to display when user wants to change the distribution behind an input.
        int rngLabelIndex;
        Label sourceLabel_RNG;
        ContextMenu menu_customRNG;

        int selectedLayer;

        bool settingsUIFrozen, DOEUIFrozen, implantUIFrozen, colUIFrozen, globalUIFrozen, replayUIFrozen, utilsUIFrozen;

        bool openGLErrorReported;

        OVPSettings[] mcVPSettings;
        OVPSettings otkVPSettings_implant;

        VeldridSurface vSurface;
        VeldridDriver viewPort;
        Panel vp;
        ContextMenu vp_menu;
        int freezeThawIndex, loadBookmarkIndex;
        RichTextArea commentBox;

        CreditsScreen aboutBox;

        Panel layerShapeProperties, layerShapeProperties_tcPanel;

        bool layerUIFrozen_exp;

        // Intention here is to prevent cyclical dependencies. Outer list is for each layer, with the arrays then holding the 'enabled' state.
        // For example, if layer 1 depends on layer 0 for x overlay ref, we should set xOLRBs_enabledState[0][1] to disabled.
        List<bool[]> xCOLRBs_enabledState;
        List<bool[]> yCOLRBs_enabledState;

        List<bool[]> xOLRBs_enabledState;
        List<bool[]> yOLRBs_enabledState;

        List<bool[]> SCDURBs_enabledState;
        List<bool[]> TCDURBs_enabledState;

        List<bool[]> CLWRRBs_enabledState;
        List<bool[]> CLWR2RBs_enabledState;

        // Layer UI element arrays.
        // 2D Layer UI stuff.
        DropDown comboBox_layerShape;
        CheckBox cB_Layer, cB_alignGeometryX, cB_alignGeometryY, cB_ShowDrawn, cB_FlipH, cB_FlipV, cB_edgeSlide;
        NumericStepper num_edgeSlideTension;
        Label lbl_edgeSlideTension;
        TextBox text_layerName;

        TableLayout tabPage_2D_layer_table;

        Int32 simulationOutputGroupBoxHeight,
                    simulationSettingsGroupBoxHeight,
                    userGuidanceWidth, userGuidanceHeight,
                    previewWidth,
                    oneDGuidanceWidth,
                    oneDLabelWidth,
                    oneDMinInsWidth,
                    oneDMinInsLblWidth,
                    multiThreadWarnWidth,
                    resultFieldWidth,
                    commentBoxWidth, commentBoxHeight,
                    num_Height,
                    label_Height,
                    radioButton_Height,
                    checkBox_Height,
                    simButtonWidth, simButtonHeight,
                    replayNumWidth, replayNumHeight
        ;

        float uiScaleFactor;
        int simRunningTabToFreeze;

        Scrollable controls;
        PixelLayout controls_content;
        Scrollable tabPage_2D_PASearch_scrollable;
        TableLayout mainTable, tabPage2_table, tab_1DCalc_table, tabPage_2D_DOE_table, tabPage_implant_table, tabPage_utilities_table, tabPage_2D_PASearch_table;

        Label statusReadout, lbl_simPreviewZoom, lbl_viewportPos;

        ProgressBar statusProgressBar;

        GroupBox bgLayerBox, omitLayerBox, simPreviewBox, miscControlsBox;
        CheckBox cB_displayShapes, cB_displayResults;
        CheckBox[] cB_bg, cB_omit;
        NumericStepper num_viewportZoom, num_viewportX, num_viewportY;

        Button btn_singleCPU, btn_multiCPU, btn_STOP, btn_Cancel;

        TabControl tabControl_main, tabControl_2D_simsettings;
        TabPage tab_1DCalc, tabPage2, tabPage_implant, tabPage_utilities, tabPage_2D_Settings, tabPage_2D_DOE, tabPage_2D_PASearch, tabPage_2D_experiment;

        ListBox listBox_layers;
        TableLayout experiment_table;

        TableLayout bgLayerBox_table;

        Panel upperGadgets_panel;

        TableLayout upperGadgets_table; // background and omit.

        CancellationTokenSource fileLoad_cancelTS;

        ContextMenu listbox_menu;
        ButtonMenuItem lb_copy, lb_paste, lb_enableDisable, lb_clear;
        ButtonMenuItem litho_rounding_copy, litho_rounding_paste;

    }
}
