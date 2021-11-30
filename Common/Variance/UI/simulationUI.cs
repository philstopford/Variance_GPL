using color;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private Button btn_Switch12, btn_Switch23, btn_Switch34, btn_Switch56, btn_Switch67, btn_Switch78,
        btn_Switch910, btn_Switch1011, btn_Switch1112, btn_Switch1314, btn_Switch1415, btn_Switch1516,
        btn_Switch15, btn_Switch26, btn_Switch37, btn_Switch48, btn_Switch913, btn_Switch1014, btn_Switch1115, btn_Switch1216, btn_SwitchA, btn_SwitchB;

    private DropDown[] comboBox_geoEqtn_Op; // container to simplify access to the menus programmatically.
    private DropDown[] comboBox_geoEqtn_Op_2Layer;
    private DropDown[] comboBox_geoEqtn_Op_4Layer;
    private DropDown[] comboBox_geoEqtn_Op_8Layer;
    private DropDown comboBox_calcModes, comboBox_RNG, comboBox_externalTypes;

    private CheckBox checkBox_externalCriteria;
    private DropDown comboBox_externalCriteria1, comboBox_externalCriteria2, comboBox_externalCriteria3, comboBox_externalCriteria4;
    private NumericStepper num_externalCriteria1, num_externalCriteria2, num_externalCriteria3, num_externalCriteria4;

    private Label[] label_geoEqtn_Op; // container to simplify access to the menus programmatically.

    private Label label_AB, lbl_multiThreadResultNote, lbl_ssNumOfCases, lbl_ssPrecision,
        lbl_cornerSegments, lbl_rng, lbl_ssTotalPoints, lbl_testArea;

    private GroupBox groupBox_setOutput, groupBox_GeoEqtn, groupBox_GeoEqtnA, groupBox_GeoEqtnB, groupBox_simSettings;

    private CheckBox checkBox_withinMode, checkBox_useShortestEdge, checkBox_aChord, checkBox_bChord,
        checkBox_limitCornerPoints, checkBox_greedyMultiCPU, checkBox_linkCDUVariation,
        checkBox_LERMode, checkBox_external, checkBox_CSV, checkBox_debugCalc,
        checkBox_perPoly;

    private TextArea textBox_userGuidance;
    private TextBox text_ssTotalPoints, text_testArea;

    private NumericStepper num_ssPrecision, num_cornerSegments, num_ssNumOfCases;

    private CheckBox checkBox_replay;

    private GroupBox groupBox_replay;

    private Button button_replay;

    private NumericStepper num_replay;

    private TableLayout tabPage_2D_Settings_table;

    private void twoD_SettingsUISetup()
    {
        tabPage_2D_Settings_table.Rows.Add(new TableRow());

        TableCell upper_tc = new();
        tabPage_2D_Settings_table.Rows[^1].Cells.Add(upper_tc);

        twoD_SettingsUISetup_upperRow(upper_tc);

        tabPage_2D_Settings_table.Rows.Add(new TableRow());

        TableLayout nestedTL = new();
        Panel p = new() {Content = nestedTL};
        tabPage_2D_Settings_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(p, centered: true) });

        nestedTL.Rows.Add(new TableRow());

        TableCell geo_tc = new();
        nestedTL.Rows[^1].Cells.Add(geo_tc);
        twoD_SettingsUI_geoEqtn(geo_tc);

        nestedTL.Rows.Add(new TableRow());
        TableCell replay_tc = new();
        nestedTL.Rows[^1].Cells.Add(replay_tc);
        twoD_SettingsUI_replay(replay_tc);

        nestedTL.Rows.Add(new TableRow());
        TableCell results_tc = new();
        nestedTL.Rows[^1].Cells.Add(results_tc);
        twoD_SettingsUI_results(results_tc);

        lbl_multiThreadResultNote = new Label
        {
            TextColor = UIHelper.myColorToColor(MyColor.Red),
            Width = multiThreadWarnWidth,
            TextAlignment = TextAlignment.Left,
            Text = "Update frequency readout",
            Visible = false
        };

        TableLayout t2 = new();
        Panel p2 = new() {Content = t2};
        t2.Rows.Add(new TableRow());

        TableCell updates_tc = new() {Control = p2};
        nestedTL.Rows.Add(new TableRow());
        nestedTL.Rows[^1].Cells.Add(updates_tc);

        t2.Rows[^1].Cells.Add(new TableCell { Control = lbl_multiThreadResultNote });
        t2.Rows[^1].Cells.Add(new TableCell()); // padding

        nestedTL.Rows.Add(new TableRow()); // padding
    }

    private void twoD_SettingsUISetup_upperRow(TableCell tc)
    {
        TableLayout tl = new();
        Panel p = new() {Content = tl};
        tc.Control = p;

        tl.Rows.Add(new TableRow());
        TableCell tc0 = new();
        tl.Rows[^1].Cells.Add(tc0);

        TableCell tc1 = new();
        tl.Rows[^1].Cells.Add(tc1);

        TableLayout tl2 = new();
        Panel p2 = new() {Content = tl2};
        tc0.Control = p2;

        tl2.Rows.Add(new TableRow());
        TableCell t2c0 = new();
        tl2.Rows[^1].Cells.Add(t2c0);
        twoD_SettingsUI_setOutput();
        t2c0.Control = groupBox_setOutput;

        tl2.Rows.Add(new TableRow());
        TableCell t2c1 = new();
        tl2.Rows[^1].Cells.Add(t2c1);
        twoD_SettingsUI_sim();
        t2c1.Control = groupBox_simSettings;

        textBox_userGuidance = new TextArea {Wrap = true, ReadOnly = true, Text = "Testing 1 2 3"};
        setSize(textBox_userGuidance, userGuidanceWidth, userGuidanceHeight);
        tc1.Control = textBox_userGuidance;
    }

    private void twoD_SettingsUI_setOutput()
    {
        TableLayout groupBox_setOutput_table = new();
        groupBox_setOutput = new GroupBox {Content = groupBox_setOutput_table, Text = "Set Output Controls"};

        groupBox_setOutput_table.Rows.Add(new TableRow());
        TableLayout row0TL = new();
        row0TL.Rows.Add(new TableRow());

        Panel row0Pnl = new() {Content = row0TL};
        groupBox_setOutput_table.Rows[^1].Cells.Add(new TableCell { Control = row0Pnl });

        comboBox_calcModes = new DropDown {Width = 400};
        comboBox_calcModes.BindDataContext(c => c.DataStore, (UIStringLists m) => m.calcModes);
        row0TL.Rows[^1].Cells.Add(comboBox_calcModes);

        groupBox_setOutput_table.Rows.Add(new TableRow());
        TableLayout row1TL = new();
        Panel row1Pnl = new() {Content = row1TL};
        groupBox_setOutput_table.Rows[^1].Cells.Add(new TableCell { Control = row1Pnl });
        row1TL.Rows.Add(new TableRow());

        checkBox_withinMode = new CheckBox
        {
            Text = "Enclosure", ToolTip = "Report minimum enclosure rather than minimum spacing value."
        };
        row1TL.Rows[^1].Cells.Add(checkBox_withinMode);

        checkBox_useShortestEdge = new CheckBox
        {
            Text = "Short Edge",
            ToolTip =
                "Use shortest edge of overlap for raycast.\nUsed for cases where A-to-B cannot be guaranteed.\nImposes some calculation overhead."
        };
        row1TL.Rows[^1].Cells.Add(checkBox_useShortestEdge);

        checkBox_aChord = new CheckBox
        {
            Text = "T/B",
            Checked = true,
            ToolTip = "Evaluate top/bottom chord lengths. Report 'N/A' if not chosen."
        };
        row1TL.Rows[^1].Cells.Add(checkBox_aChord);

        checkBox_bChord = new CheckBox
        {
            Text = "L/R",
            Checked = true,
            ToolTip = "Evaluate left/right chord lengths. Report 'N/A' if not chosen."
        };
        row1TL.Rows[^1].Cells.Add(checkBox_bChord);

        checkBox_perPoly = new CheckBox
        {
            Text = "Per-Poly", ToolTip = "Assess each polygon from overlap, to report minimum overlap area."
        };
        row1TL.Rows[^1].Cells.Add(checkBox_perPoly);
    }

    private void twoD_SettingsUI_sim()
    {
        TableLayout groupBox_simSettings_table = new();
        groupBox_simSettings = new GroupBox {Content = groupBox_simSettings_table, Text = "Simulation Settings"};

        groupBox_simSettings_table.Rows.Add(new TableRow());
        TableCell tc0 = new();
        groupBox_simSettings_table.Rows[^1].Cells.Add(tc0);

        twoD_SettingsUI_sim_row0(tc0);

        groupBox_simSettings_table.Rows.Add(new TableRow());
        TableCell tc1 = new();
        groupBox_simSettings_table.Rows[^1].Cells.Add(tc1);

        TableLayout tc1_tl = new();
        Panel tc1_p = new() {Content = tc1_tl};
        tc1.Control = tc1_p;

        tc1_tl.Rows.Add(new TableRow());

        checkBox_linkCDUVariation = new CheckBox
        {
            Text = "Link variation for tip and side CDU",
            ToolTip = "Allow for independent tip and side CDU variation, if enabled."
        };
        tc1_tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_linkCDUVariation });

        checkBox_CSV = new CheckBox
        {
            Text = "CSV",
            ToolTip =
                "Write out a CSV file containing the result for each case and its inputs. Allows for offline deep-dive review."
        };
        tc1_tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_CSV });

        tc1_tl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_simSettings_table.Rows.Add(new TableRow());
        TableCell tce = new();
        groupBox_simSettings_table.Rows[^1].Cells.Add(tce);

        TableLayout tce_tl = new();
        Panel tce_p = new() {Content = tce_tl};
        tce.Control = tce_p;

        tce_tl.Rows.Add(new TableRow());

        checkBox_external = new CheckBox
        {
            Text = "Write out ",
            ToolTip =
                "Write out a file containing the result for each case and its inputs. Will require significantly more memory."
        };
        tce_tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_external });

        comboBox_externalTypes = new DropDown
        {
            DataContext = DataContext, ToolTip = "Choose your external file type"
        };
        comboBox_externalTypes.BindDataContext(c => c.DataStore, (UIStringLists m) => m.externalTypeList);
        tce_tl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_externalTypes });

        tce_tl.Rows[^1].Cells.Add(new TableCell { Control = null });


        groupBox_simSettings_table.Rows.Add(new TableRow());
        TableCell tce2 = new();
        groupBox_simSettings_table.Rows[^1].Cells.Add(tce2);

        TableLayout tce2_tl = new();
        Panel tce2_p = new() {Content = tce2_tl};
        tce2.Control = tce2_p;

        tce2_tl.Rows.Add(new TableRow());

        checkBox_externalCriteria = new CheckBox
        {
            Text = "if ",
            ToolTip =
                "Write out a file containing the result for each case and its inputs. Will require significantly more memory."
        };
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_externalCriteria });

        comboBox_externalCriteria1 = new DropDown {DataContext = DataContext, ToolTip = "Choose filter"};
        comboBox_externalCriteria1.BindDataContext(c => c.DataStore, (UIStringLists m) => m.externalFilterList);
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_externalCriteria1 });

        num_externalCriteria1 = new NumericStepper
        {
            Value = 0.0, Increment = 0.1, DecimalPlaces = 2, ToolTip = "Define value"
        };
        setSize(num_externalCriteria1, 55);
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_externalCriteria1) });

        comboBox_externalCriteria2 = new DropDown {DataContext = DataContext, ToolTip = "Choose filter"};
        comboBox_externalCriteria2.BindDataContext(c => c.DataStore, (UIStringLists m) => m.externalFilterList);
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_externalCriteria2 });

        num_externalCriteria2 = new NumericStepper
        {
            Value = 0.0, Increment = 0.1, DecimalPlaces = 2, ToolTip = "Define value"
        };
        setSize(num_externalCriteria2, 55);
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_externalCriteria2) });

        comboBox_externalCriteria3 = new DropDown {DataContext = DataContext, ToolTip = "Choose filter"};
        comboBox_externalCriteria3.BindDataContext(c => c.DataStore, (UIStringLists m) => m.externalFilterList);
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_externalCriteria3 });

        num_externalCriteria3 = new NumericStepper
        {
            Value = 0.0, Increment = 0.1, DecimalPlaces = 2, ToolTip = "Define value"
        };
        setSize(num_externalCriteria3, 55);
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_externalCriteria3) });

        comboBox_externalCriteria4 = new DropDown {DataContext = DataContext, ToolTip = "Choose filter"};
        comboBox_externalCriteria4.BindDataContext(c => c.DataStore, (UIStringLists m) => m.externalFilterList);
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_externalCriteria4 });

        num_externalCriteria4 = new NumericStepper
        {
            Value = 0.0, Increment = 0.1, DecimalPlaces = 2, ToolTip = "Define value"
        };
        setSize(num_externalCriteria4, 55);
        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_externalCriteria4) });

        tce2_tl.Rows[^1].Cells.Add(new TableCell { Control = null });

        groupBox_simSettings_table.Rows.Add(new TableRow());
        TableCell tc2 = new();
        groupBox_simSettings_table.Rows[^1].Cells.Add(tc2);

        TableLayout tc2_tl = new();
        Panel tc2_p = new() {Content = tc2_tl};
        tc2.Control = tc2_p;

        tc2_tl.Rows.Add(new TableRow());

        lbl_rng = new Label
        {
            Text = "RNG",
            ToolTip = "Choice of random number generators. Cryptographic is 10% slower, but more rigorous."
        };
        tc2_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_rng });

        comboBox_RNG = new DropDown
        {
            DataContext = DataContext,
            ToolTip = "Choice of random number generators. Cryptographic is 10% slower, but more rigorous."
        };
        comboBox_RNG.BindDataContext(c => c.DataStore, (UIStringLists m) => m.rngTypeList);
        tc2_tl.Rows[^1].Cells.Add(new TableCell { Control = comboBox_RNG });

        groupBox_simSettings_table.Rows.Add(new TableRow());
        TableCell tc3 = new();
        groupBox_simSettings_table.Rows[^1].Cells.Add(tc3);

        groupBox_simSettings_table.Rows[^1].Cells.Add(new TableCell { Control = null });

        TableLayout tc3_tl = new();
        Panel tc3_p = new() {Content = tc3_tl};
        tc3.Control = tc3_p;

        tc3_tl.Rows.Add(new TableRow());

        checkBox_debugCalc = new CheckBox
        {
            Text = "Debug", ToolTip = "Enable debug (calculation engine support depending)."
        };
        tc3_tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_debugCalc });

        tc3_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });

        lbl_ssTotalPoints = new Label {Text = "Total # points"};
        tc3_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_ssTotalPoints });

        text_ssTotalPoints = new TextBox {ReadOnly = true};
        tc3_tl.Rows[^1].Cells.Add(new TableCell { Control = text_ssTotalPoints });


        groupBox_simSettings_table.Rows.Add(new TableRow()); // padding
    }

    private void twoD_SettingsUI_sim_row0(TableCell tc)
    {
        Panel rp0 = new();
        tc.Control = rp0;
        TableLayout r0_tl = new();
        rp0.Content = r0_tl;
        r0_tl.Rows.Add(new TableRow());

        lbl_ssNumOfCases = new Label {Text = "Number of Cases", ToolTip = "Total number of cases to evaluate."};
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_ssNumOfCases });

        num_ssNumOfCases = new NumericStepper
        {
            Value = 25000, Increment = 1, MinValue = 1, ToolTip = "Total number of cases to evaluate."
        };
        setSize(num_ssNumOfCases, 80);
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_ssNumOfCases) });

        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        checkBox_greedyMultiCPU = new CheckBox
        {
            Text = "All CPUs (multi CPU)",
            ToolTip = "In multi-threaded runs, use all cores on system, rather than leaving one idle"
        };
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_greedyMultiCPU });

        r0_tl.Rows.Add(new TableRow());

        lbl_ssPrecision = new Label {Text = "Resolution (nm)", ToolTip = "Spacing interval for points on edges."};
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_ssPrecision });

        num_ssPrecision = new NumericStepper
        {
            Value = 1.0,
            Increment = 0.1,
            MinValue = 0.01,
            DecimalPlaces = 2,
            ToolTip = "Spacing interval for points on edges."
        };
        setSize(num_ssPrecision, 80);
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_ssPrecision) });

        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        checkBox_LERMode = new CheckBox
        {
            Text = "LER as LWR/sqrt(2)", ToolTip = "Define LER as LWR/1.414 instead of LWR/2."
        };
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_LERMode });

        r0_tl.Rows.Add(new TableRow());

        lbl_cornerSegments = new Label
        {
            Text = "Corner Segments", ToolTip = "Number of segments to create in corners."
        };
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_cornerSegments });

        num_cornerSegments = new NumericStepper
        {
            Value = 90, Increment = 1, MinValue = 2, ToolTip = "Number of segments to create in corners."
        };
        setSize(num_cornerSegments, 60);
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_cornerSegments) });

        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding

        checkBox_limitCornerPoints = new CheckBox
        {
            Text = "Optimize corner pt density",
            ToolTip = "Prevent points in corners being spaced more closely than the resolution limit for edges."
        };
        r0_tl.Rows[^1].Cells.Add(new TableCell { Control = checkBox_limitCornerPoints });
    }

    private void twoD_SettingsUI_geoEqtn(TableCell tc)
    {
        TableLayout tl = new();
        Panel p = new() {Content = tl};
        tc.Control = TableLayout.AutoSized(p, centered: true);

        tl.Rows.Add(new TableRow());
        TableCell tc0 = new();
        tl.Rows[0].Cells.Add(tc0);

        comboBox_geoEqtn_Op = new DropDown[CentralProperties.maxLayersForMC];
        label_geoEqtn_Op = new Label[CentralProperties.maxLayersForMC];
        comboBox_geoEqtn_Op_2Layer = new DropDown[CentralProperties.maxLayersForMC / 2];
        comboBox_geoEqtn_Op_4Layer = new DropDown[comboBox_geoEqtn_Op_2Layer.Length / 2];
        comboBox_geoEqtn_Op_8Layer = new DropDown[comboBox_geoEqtn_Op_4Layer.Length / 2];

        TableLayout groupBox_GeoEqtn_table = new();
        groupBox_GeoEqtn = new GroupBox {Content = groupBox_GeoEqtn_table, Text = "Geometric Equation"};
        tc0.Control = groupBox_GeoEqtn;
        tl.Rows[0].Cells.Add(new TableCell { Control = null });

        TableRow tr0 = new();
        groupBox_GeoEqtn_table.Rows.Add(tr0);

        TableLayout groupBox_GeoEqtnA_table = new();
        groupBox_GeoEqtnA = new GroupBox {Content = groupBox_GeoEqtnA_table, Text = "Geometric Equation A"};
        tr0.Cells.Add(new TableCell { Control = groupBox_GeoEqtnA });

        TableRow tr1 = new();
        groupBox_GeoEqtn_table.Rows.Add(tr1);

        label_AB = new Label {Text = "testing 1 2 3", TextAlignment = TextAlignment.Center};
        tr1.Cells.Add(new TableCell { Control = label_AB });

        TableRow tr2 = new();
        groupBox_GeoEqtn_table.Rows.Add(tr2);

        TableLayout groupBox_GeoEqtnB_table = new();
        groupBox_GeoEqtnB = new GroupBox {Content = groupBox_GeoEqtnB_table, Text = "Geometric Equation B"};
        tr2.Cells.Add(new TableCell { Control = groupBox_GeoEqtnB });

        for (int i = 0; i < comboBox_geoEqtn_Op_8Layer.Length; i++)
        {
            comboBox_geoEqtn_Op_8Layer[i] = new DropDown {Width = 40};
            comboBox_geoEqtn_Op_8Layer[i].BindDataContext(c => c.DataStore, (UIStringLists m) => m.booleanList);
        }

        for (int i = 0; i < comboBox_geoEqtn_Op_4Layer.Length; i++)
        {
            comboBox_geoEqtn_Op_4Layer[i] = new DropDown {Width = 40};
            comboBox_geoEqtn_Op_4Layer[i].BindDataContext(c => c.DataStore, (UIStringLists m) => m.booleanList);
        }

        for (int i = 0; i < comboBox_geoEqtn_Op_2Layer.Length; i++)
        {
            comboBox_geoEqtn_Op_2Layer[i] = new DropDown {Width = 40};
            comboBox_geoEqtn_Op_2Layer[i].BindDataContext(c => c.DataStore, (UIStringLists m) => m.booleanList);
        }

        for (int i = 0; i < comboBox_geoEqtn_Op.Length; i++)
        {
            comboBox_geoEqtn_Op[i] = new DropDown {Width = 40};
            comboBox_geoEqtn_Op[i].BindDataContext(c => c.DataStore, (UIStringLists m) => m.notList);

            label_geoEqtn_Op[i] = new Label
            {
                TextColor = UIHelper.myColorToColor(commonVars.getColors().simPreviewColors[i]),
                Text = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name),
                Width = 50
            };

            var i2 = i;

            label_geoEqtn_Op[i2].MouseDoubleClick += delegate(object _, MouseEventArgs e)
            {
                goToLayerUI(i2);
                e.Handled = true;
            };

        }

        groupBox_GeoEqtnA_table.Rows.Add(new TableRow());
        TableCell a_tc = new();
        groupBox_GeoEqtnA_table.Rows[0].Cells.Add(a_tc);
        twoD_SettingsUI_geoEqtnA(a_tc);

        groupBox_GeoEqtnB_table.Rows.Add(new TableRow());
        TableCell b_tc = new();
        groupBox_GeoEqtnB_table.Rows[0].Cells.Add(b_tc);
        twoD_SettingsUI_geoEqtnB(b_tc);
    }

    private void twoD_SettingsUI_geoEqtnA(TableCell tc)
    {
        TableLayout outer_tl = new();
        Panel p = new() {Content = outer_tl};

        tc.Control = p;

        TableRow row0 = new();
        outer_tl.Rows.Add(row0);
        aRow0(row0);

        TableRow row1 = new();
        outer_tl.Rows.Add(row1);
        aRow1(row1);

        TableRow row2 = new();
        outer_tl.Rows.Add(row2);
        aRow2(row2);

        TableRow padding = new() {ScaleHeight = true};
        outer_tl.Rows.Add(padding);
    }

    private void aRow0(TableRow row0)
    {
        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[0] });
        row0.Cells.Add(new TableCell { Control = label_geoEqtn_Op[0] });

        btn_Switch12 = new Button();
        setSize(btn_Switch12, 10, 18);
        btn_Switch12.Click += switchSimulationLayers12Over;
        row0.Cells.Add(new TableCell { Control = btn_Switch12 });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_2Layer[0] });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[1] });
        row0.Cells.Add(new TableCell { Control = label_geoEqtn_Op[1] });

        btn_Switch23 = new Button();
        setSize(btn_Switch23, 10, 18);
        btn_Switch23.Click += switchSimulationLayers23Over;
        row0.Cells.Add(new TableCell { Control = btn_Switch23 });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_4Layer[0] });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[2] });
        row0.Cells.Add(new TableCell { Control = label_geoEqtn_Op[2] });

        btn_Switch34 = new Button();
        setSize(btn_Switch34, 10, 18);
        btn_Switch34.Click += switchSimulationLayers34Over;
        row0.Cells.Add(new TableCell { Control = btn_Switch34 });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_2Layer[1] });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[3] });
        row0.Cells.Add(new TableCell { Control = label_geoEqtn_Op[3] });
    }

    private void aRow1(TableRow row1)
    {
        row1.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_8Layer[0] });

        btn_Switch15 = new Button();
        setSize(btn_Switch15, 38, 10);
        btn_Switch15.Click += switchSimulationLayers15Over;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_Switch15) });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        btn_Switch26 = new Button();
        setSize(btn_Switch26, 38, 10);
        btn_Switch26.Click += switchSimulationLayers26Over;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_Switch26) });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        btn_Switch37 = new Button();
        setSize(btn_Switch37, 38, 10);
        btn_Switch37.Click += switchSimulationLayers37Over;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_Switch37) });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        btn_Switch48 = new Button();
        setSize(btn_Switch48, 38, 10);
        btn_Switch48.Click += switchSimulationLayers48ver;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_Switch48) });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        btn_SwitchA = new Button();
        setSize(btn_SwitchA, 38, 10);
        btn_SwitchA.Click += switchSimulationAllALayersOver;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_SwitchA) });
    }

    private void aRow2(TableRow row2)
    {
        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[4] });
        row2.Cells.Add(new TableCell { Control = label_geoEqtn_Op[4] });

        btn_Switch56 = new Button();
        setSize(btn_Switch56, 10, 18);
        btn_Switch56.Click += switchSimulationLayers56Over;
        row2.Cells.Add(new TableCell { Control = btn_Switch56 });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_2Layer[2] });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[5] });
        row2.Cells.Add(new TableCell { Control = label_geoEqtn_Op[5] });

        btn_Switch67 = new Button();
        setSize(btn_Switch67, 10, 18);
        btn_Switch67.Click += switchSimulationLayers67Over;
        row2.Cells.Add(new TableCell { Control = btn_Switch67 });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_4Layer[1] });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[6] });
        row2.Cells.Add(new TableCell { Control = label_geoEqtn_Op[6] });

        btn_Switch78 = new Button();
        setSize(btn_Switch78, 10, 18);
        btn_Switch78.Click += switchSimulationLayers78Over;
        row2.Cells.Add(new TableCell { Control = btn_Switch78 });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_2Layer[3] });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[7] });
        row2.Cells.Add(new TableCell { Control = label_geoEqtn_Op[7] });
    }

    private void twoD_SettingsUI_geoEqtnB(TableCell tc)
    {
        Panel p = new();

        tc.Control = p;

        TableLayout outer_tl = new();
        p.Content = outer_tl;
        TableRow row0 = new();
        outer_tl.Rows.Add(row0);
        bRow0(row0);

        TableRow row1 = new();
        outer_tl.Rows.Add(row1);
        bRow1(row1);

        TableRow row2 = new();
        outer_tl.Rows.Add(row2);
        bRow2(row2);

        TableRow padding = new() {ScaleHeight = true};
        outer_tl.Rows.Add(padding);
    }

    private void bRow0(TableRow row0)
    {
        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[8] });
        row0.Cells.Add(new TableCell { Control = label_geoEqtn_Op[8] });

        btn_Switch910 = new Button();
        setSize(btn_Switch910, 10, 18);
        btn_Switch910.Click += switchSimulationLayers910Over;
        row0.Cells.Add(new TableCell { Control = btn_Switch910 });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_2Layer[4] });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[9] });
        row0.Cells.Add(new TableCell { Control = label_geoEqtn_Op[9] });

        btn_Switch1011 = new Button();
        setSize(btn_Switch1011, 10, 18);
        btn_Switch1011.Click += switchSimulationLayers1011Over;
        row0.Cells.Add(new TableCell { Control = btn_Switch1011 });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_4Layer[2] });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[10] });
        row0.Cells.Add(new TableCell { Control = label_geoEqtn_Op[10] });

        btn_Switch1112 = new Button();
        setSize(btn_Switch1112, 10, 18);
        btn_Switch1112.Click += switchSimulationLayers1112Over;
        row0.Cells.Add(new TableCell { Control = btn_Switch1112 });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_2Layer[5] });

        row0.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[11] });
        row0.Cells.Add(new TableCell { Control = label_geoEqtn_Op[11] });

    }

    private void bRow1(TableRow row1)
    {
        row1.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_8Layer[1] });

        btn_Switch913 = new Button();
        setSize(btn_Switch913, 38, 10);
        btn_Switch913.Click += switchSimulationLayers913Over;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_Switch913) });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        btn_Switch1014 = new Button();
        setSize(btn_Switch1014, 38, 10);
        btn_Switch1014.Click += switchSimulationLayers26Over;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_Switch1014) });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        btn_Switch1115 = new Button();
        setSize(btn_Switch1115, 38, 10);
        btn_Switch1115.Click += switchSimulationLayers1115Over;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_Switch1115) });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        btn_Switch1216 = new Button();
        setSize(btn_Switch1216, 38, 10);
        btn_Switch1216.Click += switchSimulationLayers1216Over;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_Switch1216) });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        row1.Cells.Add(new TableCell { Control = null });

        btn_SwitchB = new Button();
        setSize(btn_SwitchB, 38, 10);
        btn_SwitchB.Click += switchSimulationAllBLayersOver;
        row1.Cells.Add(new TableCell { Control = TableLayout.AutoSized(btn_SwitchB) });
    }

    private void bRow2(TableRow row2)
    {
        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[12] });
        row2.Cells.Add(new TableCell { Control = label_geoEqtn_Op[12] });

        btn_Switch1314 = new Button();
        setSize(btn_Switch1314, 10, 18);
        btn_Switch1314.Click += switchSimulationLayers1314Over;
        row2.Cells.Add(new TableCell { Control = btn_Switch1314 });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_2Layer[6] });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[13] });
        row2.Cells.Add(new TableCell { Control = label_geoEqtn_Op[13] });

        btn_Switch1415 = new Button();
        setSize(btn_Switch1415, 10, 18);
        btn_Switch1415.Click += switchSimulationLayers1415Over;
        row2.Cells.Add(new TableCell { Control = btn_Switch1415 });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_4Layer[3] });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[14] });
        row2.Cells.Add(new TableCell { Control = label_geoEqtn_Op[14] });

        btn_Switch1516 = new Button();
        setSize(btn_Switch1516, 10, 18);
        btn_Switch1516.Click += switchSimulationLayers1516Over;
        row2.Cells.Add(new TableCell { Control = btn_Switch1516 });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op_2Layer[7] });

        row2.Cells.Add(new TableCell { Control = comboBox_geoEqtn_Op[15] });
        row2.Cells.Add(new TableCell { Control = label_geoEqtn_Op[15] });
    }

    private void twoD_SettingsUI_replay(TableCell tc)
    {
        TableLayout tl = new();
        Panel p = new() {Content = tl};
        tc.Control = tc.Control = p; //TableLayout.AutoSized(p, centered: true);

        tl.Rows.Add(new TableRow());
        TableCell tc0 = new();
        tl.Rows[0].Cells.Add(tc0);

        TableLayout groupBox_replay_table = new();
        groupBox_replay = new GroupBox {Content = groupBox_replay_table, Text = "Replay"};
        tc0.Control = groupBox_replay;
        tl.Rows[0].Cells.Add(new TableCell { Control = null });

        TableRow tr0 = new();
        groupBox_replay_table.Rows.Add(tr0);

        button_replay = new Button
        {
            Text = "Load CSV File",
            ToolTip =
                "Load CSV file generated from a run using this project file.\n\rThis will allow replay of the cases."
        };
        tr0.Cells.Add(new TableCell { Control = TableLayout.AutoSized(button_replay) });

        checkBox_replay = new CheckBox
        {
            Text = "Enable", ToolTip = "Enable replay using a CSV file from the loaded project"
        };
        tr0.Cells.Add(new TableCell { Control = checkBox_replay });

        num_replay = new NumericStepper
        {
            MinValue = 0, MaxValue = 0, Increment = 1, ToolTip = "Which case to view from the loaded CSV file"
        };
        setSize(num_replay, replayNumWidth);
        tr0.Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_replay) });

        tr0.Cells.Add(new TableCell { Control = null }); // padding
    }

    private void twoD_SettingsUI_results(TableCell tc)
    {
        Panel p = new();
        tc.Control = TableLayout.AutoSized(p);

        TableLayout tl = new();
        tl.Rows.Add(new TableRow());

        lbl_testArea = new Label {Text = "Result for this run", TextAlignment = TextAlignment.Left};
        tl.Rows[0].Cells.Add(new TableCell { Control = lbl_testArea });

        text_testArea = new TextBox {ReadOnly = true};
        setSize(text_testArea, 470);
        text_testArea.Text = "N/A";

        tl.Rows[0].Cells.Add(new TableCell { Control = text_testArea });

        p.Content = tl;
    }
}