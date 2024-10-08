using System.Collections.Generic;
using System.Collections.ObjectModel;
using Eto.Forms;
using Eto.Veldrid;
using VeldridEto;

namespace Variance;

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

    private const int viewportSize = 10;

    private bool[] geoGBVisible, subShapeGBVisible, booleanGBVisible;

    // Context menu to display when user wants to change the distribution behind an input.
    private int rngLabelIndex;
    private Label sourceLabel_RNG;
    private ContextMenu menu_customRNG;

    private int selectedLayer;

    private bool settingsUIFrozen, DOEUIFrozen, implantUIFrozen, colUIFrozen, globalUIFrozen, replayUIFrozen, utilsUIFrozen;

    private bool openGLErrorReported;

    private OVPSettings[] mcVPSettings;
    private OVPSettings otkVPSettings_implant;

    private VeldridSurface vSurface;
    private VeldridDriver viewPort;
    private Panel vp;
    private ContextMenu vp_menu;
    private int freezeThawIndex, loadBookmarkIndex;
    private RichTextArea commentBox;

    private CreditsScreen aboutBox;

    private Panel layerShapeProperties, layerShapeProperties_tcPanel;

    private bool layerUIFrozen_exp;

    // Intention here is to prevent cyclical dependencies. Outer list is for each layer, with the arrays then holding the 'enabled' state.
    // For example, if layer 1 depends on layer 0 for x overlay ref, we should set xOLRBs_enabledState[0][1] to disabled.
    private List<bool[]> xCOLRBs_enabledState;
    private List<bool[]> yCOLRBs_enabledState;

    private List<bool[]> xOLRBs_enabledState;
    private List<bool[]> yOLRBs_enabledState;

    private List<bool[]> SCDURBs_enabledState;
    private List<bool[]> TCDURBs_enabledState;

    private List<bool[]> CLWRRBs_enabledState;
    private List<bool[]> CLWR2RBs_enabledState;

    // Layer UI element arrays.
    // 2D Layer UI stuff.
    private DropDown comboBox_layerShape;
    private CheckBox cB_Layer, cB_alignGeometryX, cB_alignGeometryY, cB_ShowDrawn, cB_FlipH, cB_FlipV, cB_edgeSlide;
    private NumericStepper num_edgeSlideTension;
    private Label lbl_edgeSlideTension;
    private TextBox text_layerName;

    private TableLayout tabPage_2D_layer_table;

    private int simulationOutputGroupBoxHeight,
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
        label_Height,
        replayNumWidth
        ;

    private int simRunningTabToFreeze;

    private Scrollable controls;
    private Scrollable tabPage_2D_PASearch_scrollable;
    private TableLayout mainTable, tabPage2_table, tab_1DCalc_table, tabPage_2D_DOE_table, tabPage_implant_table, tabPage_utilities_table, tabPage_2D_PASearch_table;

    private Label statusReadout, lbl_simPreviewZoom, lbl_viewportPos;

    private ProgressBar statusProgressBar;

    private GroupBox bgLayerBox, omitLayerBox, simPreviewBox;
    private CheckBox cB_displayShapes, cB_displayResults;
    private CheckBox[] cB_bg;
    private CheckBox[] cB_omit;
    private NumericStepper num_viewportZoom, num_viewportX, num_viewportY;

    private Button btn_Run, btn_STOP, btn_Cancel;

    private TabControl tabControl_main, tabControl_2D_simsettings;
    private TabPage tab_1DCalc, tabPage2, tabPage_implant, tabPage_utilities, tabPage_2D_Settings, tabPage_2D_DOE, tabPage_2D_PASearch, tabPage_2D_experiment;

    private ListBox listBox_layers;
    private TableLayout experiment_table;

    private TableLayout bgLayerBox_table;

    private Panel upperGadgets_panel;

    private TableLayout upperGadgets_table; // background and omit.

    private ContextMenu listbox_menu;
    private ButtonMenuItem lb_copy, lb_paste, lb_enableDisable, lb_clear;
}