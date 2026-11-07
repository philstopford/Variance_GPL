using System.Collections.Generic;
using System.Collections.ObjectModel;
using Eto.Forms;
using Eto.Veldrid;
using VeldridEto;

namespace Variance;

public partial class MainForm
{
    /// <summary>
    /// Container class for UI string collections used throughout the application.
    /// Uses MVVM pattern for data binding to comboboxes and other UI controls.
    /// ObservableCollection types send notifications when contents change, enabling automatic UI updates.
    /// Regular List types are used for static collections that don't change during runtime.
    /// </summary>
    public class UIStringLists
    {
        /// <summary>Array of observable collections containing sub-shape options for each layer</summary>
        public ObservableCollection<string>[] subShapeList { get; set; }

        /// <summary>List of available geometric shapes (rectangle, circle, polygon, etc.)</summary>
        public List<string> shapes { get; set; }

        /// <summary>List of tip location options for geometric shapes</summary>
        public List<string> tipLocs { get; set; }

        /// <summary>List of sub-shape positioning options</summary>
        public List<string> subShapePos { get; set; }

        /// <summary>Observable collection of calculation modes available in the system</summary>
        public ObservableCollection<string> calcModes { get; set; }

        /// <summary>List of external file type options</summary>
        public List<string> externalTypeList { get; set; }

        /// <summary>List of filtering options for external files</summary>
        public List<string> externalFilterList { get; set; }

        /// <summary>List of random number generator types</summary>
        public List<string> rngTypeList { get; set; }
        public List<string> samplerList { get; set; }
        
        /// <summary>List of noise generation types for Line Edge Roughness simulation</summary>
        public List<string> noiseTypeList { get; set; }

        /// <summary>List of polygon fill options</summary>
        public List<string> polyFillList { get; set; }

        /// <summary>Array of observable collections containing GeoCore structure names for each layer</summary>
        public ObservableCollection<string>[] geoCoreStructureList { get; set; }
        
        /// <summary>Array of observable collections containing GeoCore layer/datatype combinations for each layer</summary>
        public ObservableCollection<string>[] geoCoreLDList { get; set; }

        /// <summary>Observable collection of GeoCore structure names for export operations</summary>
        public ObservableCollection<string> geoCoreStructureList_exp { get; set; }
        
        /// <summary>Observable collection of GeoCore layer/datatype combinations for export operations</summary>
        public ObservableCollection<string> geoCoreLDList_exp { get; set; }

        /// <summary>Observable collection of sub-shapes for export operations</summary>
        public ObservableCollection<string> subShapesList_exp { get; set; }

        /// <summary>Observable collection of layer names used throughout the application</summary>
        public ObservableCollection<string> layerNames { get; set; }

        /// <summary>List of falloff function types for proximity biasing</summary>
        public List<string> fallOffList { get; set; }

        /// <summary>List of logical NOT operation options</summary>
        public List<string> notList { get; set; }
        
        /// <summary>List of Boolean operation types (union, difference, intersection, etc.)</summary>
        public List<string> booleanList { get; set; }
        
        /// <summary>List of OpenGL rendering mode options</summary>
        public List<string> openGLMode { get; set; }

        /// <summary>Observable collection for custom RNG mapping configurations</summary>
        public ObservableCollection<string> rngMapping { get; set; }
    }

    /// <summary>Size of the viewport used for rendering</summary>
    private const int viewportSize = 10;

    /// <summary>Arrays tracking visibility state of geometry, sub-shape, and boolean group boxes for each layer</summary>
    private bool[] geoGBVisible, subShapeGBVisible, booleanGBVisible;

    /// <summary>Context menu system for custom RNG distribution selection</summary>
    private int rngLabelIndex;
    private Label sourceLabel_RNG;
    private ContextMenu menu_customRNG;

    /// <summary>Index of currently selected layer in the UI</summary>
    private int selectedLayer;

    /// <summary>Flags to track UI freeze state for different sections to prevent updates during bulk operations</summary>
    private bool settingsUIFrozen, DOEUIFrozen, implantUIFrozen, colUIFrozen, globalUIFrozen, replayUIFrozen, utilsUIFrozen;

    /// <summary>Flag to prevent repeated OpenGL error reporting</summary>
    private bool openGLErrorReported;

    /// <summary>Viewport settings for Monte Carlo and implant simulation rendering</summary>
    private OVPSettings[] mcVPSettings;
    private OVPSettings otkVPSettings_implant;

    /// <summary>Veldrid graphics components for hardware-accelerated rendering</summary>
    private VeldridSurface vSurface;
    private VeldridDriver viewPort;
    private Panel vp;
    private ContextMenu vp_menu;
    
    /// <summary>UI state tracking for freeze/thaw operations and bookmark loading</summary>
    private int freezeThawIndex, loadBookmarkIndex;
    
    /// <summary>Text area for user comments and credits display</summary>
    private RichTextArea commentBox;
    private CreditsScreen aboutBox;

    /// <summary>Property panels for layer and shape configuration</summary>
    private Panel layerShapeProperties, layerShapeProperties_tcPanel;

    /// <summary>Flag to track if layer UI is frozen during export operations</summary>
    private bool layerUIFrozen_exp;

    /// <summary>
    /// Dependency tracking arrays to prevent cyclical dependencies between layers.
    /// Outer list is for each layer, inner arrays hold 'enabled' state for other layers.
    /// For example, if layer 1 depends on layer 0 for x overlay ref, 
    /// we set xOLRBs_enabledState[0][1] to disabled.
    /// </summary>
    private List<bool[]> xCOLRBs_enabledState;  // X coordinate overlay radio button states
    private List<bool[]> yCOLRBs_enabledState;  // Y coordinate overlay radio button states

    private List<bool[]> xOLRBs_enabledState;   // X overlay radio button states
    private List<bool[]> yOLRBs_enabledState;   // Y overlay radio button states

    private List<bool[]> SCDURBs_enabledState;
    private List<bool[]> TCDURBs_enabledState;

    private List<bool[]> CLWRRBs_enabledState;
    private List<bool[]> CLWR2RBs_enabledState;

    // Layer UI element arrays.
    // 2D Layer UI stuff.
    private DropDown comboBox_layerShape;
    private CheckBox cB_Layer, cB_alignGeometryX, cB_alignGeometryY, cB_ShowDrawn, cB_FlipH, cB_FlipV, cB_edgeSlide, cB_legacyRounding;
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

    private Splitter rightSplitter;
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