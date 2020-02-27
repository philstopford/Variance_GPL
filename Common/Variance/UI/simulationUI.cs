using color;
using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        Button btn_Switch12, btn_Switch23, btn_Switch34, btn_Switch56, btn_Switch67, btn_Switch78,
            btn_Switch910, btn_Switch1011, btn_Switch1112, btn_Switch1314, btn_Switch1415, btn_Switch1516,
            btn_Switch15, btn_Switch26, btn_Switch37, btn_Switch48, btn_Switch913, btn_Switch1014, btn_Switch1115, btn_Switch1216, btn_SwitchA, btn_SwitchB;

        DropDown[] comboBox_geoEqtn_Op; // container to simplify access to the menus programmatically.
        DropDown[] comboBox_geoEqtn_Op_2Layer;
        DropDown[] comboBox_geoEqtn_Op_4Layer;
        DropDown[] comboBox_geoEqtn_Op_8Layer;
        DropDown comboBox_calcModes, comboBox_RNG, comboBox_externalTypes;

        Label[] label_geoEqtn_Op; // container to simplify access to the menus programmatically.
        Label label_AB, lbl_multiThreadResultNote, lbl_ssNumOfCases, lbl_ssPrecision,
                lbl_cornerSegments, lbl_rng, lbl_ssTotalPoints, lbl_testArea;

        GroupBox groupBox_setOutput, groupBox_GeoEqtn, groupBox_GeoEqtnA, groupBox_GeoEqtnB, groupBox_simSettings;

        CheckBox checkBox_withinMode, checkBox_useShortestEdge, checkBox_aChord, checkBox_bChord,
                    checkBox_limitCornerPoints, checkBox_greedyMultiCPU, checkBox_linkCDUVariation,
                    checkBox_LERMode, checkBox_external, checkBox_CSV, checkBox_debugCalc,
                    checkBox_perPoly;

        TextArea textBox_userGuidance;
        TextBox text_ssTotalPoints, text_testArea;

        NumericStepper num_ssPrecision, num_cornerSegments, num_ssNumOfCases;

        CheckBox checkBox_replay;

        GroupBox groupBox_replay;

        Button button_replay;

        NumericStepper num_replay;

        TableLayout tabPage_2D_Settings_table;

        void twoD_SettingsUISetup()
        {
            tabPage_2D_Settings_table.Rows.Add(new TableRow());

            TableCell upper_tc = new TableCell();
            tabPage_2D_Settings_table.Rows[tabPage_2D_Settings_table.Rows.Count - 1].Cells.Add(upper_tc);

            twoD_SettingsUISetup_upperRow(upper_tc);

            tabPage_2D_Settings_table.Rows.Add(new TableRow());

            Panel p = new Panel();
            tabPage_2D_Settings_table.Rows[tabPage_2D_Settings_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(p, centered: true) });

            TableLayout nestedTL = new TableLayout();
            p.Content = nestedTL;

            nestedTL.Rows.Add(new TableRow());

            TableCell geo_tc = new TableCell();
            nestedTL.Rows[nestedTL.Rows.Count - 1].Cells.Add(geo_tc);
            twoD_SettingsUI_geoEqtn(geo_tc);

            nestedTL.Rows.Add(new TableRow());
            TableCell replay_tc = new TableCell();
            nestedTL.Rows[nestedTL.Rows.Count - 1].Cells.Add(replay_tc);
            twoD_SettingsUI_replay(replay_tc);

            nestedTL.Rows.Add(new TableRow());
            TableCell results_tc = new TableCell();
            nestedTL.Rows[nestedTL.Rows.Count - 1].Cells.Add(results_tc);
            twoD_SettingsUI_results(results_tc);

            lbl_multiThreadResultNote = new Label();
            lbl_multiThreadResultNote.TextColor = UIHelper.myColorToColor(MyColor.Red);
            lbl_multiThreadResultNote.Width = multiThreadWarnWidth;
            lbl_multiThreadResultNote.TextAlignment = TextAlignment.Left;
            lbl_multiThreadResultNote.Text = "Update frequency readout";
            lbl_multiThreadResultNote.Visible = false;

            Panel p2 = new Panel();
            TableLayout t2 = new TableLayout();
            p2.Content = t2;
            t2.Rows.Add(new TableRow());


            TableCell updates_tc = new TableCell();
            updates_tc.Control = p2;
            nestedTL.Rows.Add(new TableRow());
            nestedTL.Rows[nestedTL.Rows.Count - 1].Cells.Add(updates_tc);

            t2.Rows[t2.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_multiThreadResultNote });
            t2.Rows[t2.Rows.Count - 1].Cells.Add(new TableCell()); // padding

            nestedTL.Rows.Add(new TableRow()); // padding
        }

        void twoD_SettingsUISetup_upperRow(TableCell tc)
        {
            Panel p = new Panel();
            TableLayout tl = new TableLayout();
            p.Content = tl;
            tc.Control = p;

            tl.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(tc0);

            TableCell tc1 = new TableCell();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(tc1);

            Panel p2 = new Panel();
            TableLayout tl2 = new TableLayout();
            p2.Content = tl2;
            tc0.Control = p2;

            tl2.Rows.Add(new TableRow());
            TableCell t2c0 = new TableCell();
            tl2.Rows[tl2.Rows.Count - 1].Cells.Add(t2c0);
            twoD_SettingsUI_setOutput();
            t2c0.Control = groupBox_setOutput;

            tl2.Rows.Add(new TableRow());
            TableCell t2c1 = new TableCell();
            tl2.Rows[tl2.Rows.Count - 1].Cells.Add(t2c1);
            twoD_SettingsUI_sim();
            t2c1.Control = groupBox_simSettings;

            textBox_userGuidance = new TextArea();
            setSize(textBox_userGuidance, userGuidanceWidth, userGuidanceHeight);
            textBox_userGuidance.Wrap = true;
            textBox_userGuidance.ReadOnly = true;
            textBox_userGuidance.Text = "Testing 1 2 3";
            tc1.Control = textBox_userGuidance;
        }

        void twoD_SettingsUI_setOutput()
        {
            groupBox_setOutput = new GroupBox();
            TableLayout groupBox_setOutput_table = new TableLayout();
            groupBox_setOutput.Content = groupBox_setOutput_table;
            groupBox_setOutput.Text = "Set Output Controls";

            groupBox_setOutput_table.Rows.Add(new TableRow());
            Panel row0Pnl = new Panel();
            groupBox_setOutput_table.Rows[groupBox_setOutput_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row0Pnl });
            TableLayout row0TL = new TableLayout();
            row0Pnl.Content = row0TL;
            row0TL.Rows.Add(new TableRow());

            comboBox_calcModes = new DropDown();
            comboBox_calcModes.Width = 400;
            comboBox_calcModes.BindDataContext(c => c.DataStore, (UIStringLists m) => m.calcModes);
            row0TL.Rows[row0TL.Rows.Count - 1].Cells.Add(comboBox_calcModes);

            groupBox_setOutput_table.Rows.Add(new TableRow());
            Panel row1Pnl = new Panel();
            groupBox_setOutput_table.Rows[groupBox_setOutput_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = row1Pnl });
            TableLayout row1TL = new TableLayout();
            row1Pnl.Content = row1TL;
            row1TL.Rows.Add(new TableRow());

            checkBox_withinMode = new CheckBox();
            checkBox_withinMode.Text = "Enclosure";
            checkBox_withinMode.ToolTip = "Report minimum enclosure rather than minimum spacing value.";
            row1TL.Rows[row1TL.Rows.Count - 1].Cells.Add(checkBox_withinMode);

            checkBox_useShortestEdge = new CheckBox();
            checkBox_useShortestEdge.Text = "Short Edge";
            checkBox_useShortestEdge.ToolTip = "Use shortest edge of overlap for raycast.\nUsed for cases where A-to-B cannot be guaranteed.\nImposes some calculation overhead.";
            row1TL.Rows[row1TL.Rows.Count - 1].Cells.Add(checkBox_useShortestEdge);

            checkBox_aChord = new CheckBox();
            checkBox_aChord.Text = "T/B";
            checkBox_aChord.Checked = true;
            checkBox_aChord.ToolTip = "Evaluate top/bottom chord lengths. Report 'N/A' if not chosen.";
            row1TL.Rows[row1TL.Rows.Count - 1].Cells.Add(checkBox_aChord);

            checkBox_bChord = new CheckBox();
            checkBox_bChord.Text = "L/R";
            checkBox_bChord.Checked = true;
            checkBox_bChord.ToolTip = "Evaluate left/right chord lengths. Report 'N/A' if not chosen.";
            row1TL.Rows[row1TL.Rows.Count - 1].Cells.Add(checkBox_bChord);

            checkBox_perPoly = new CheckBox();
            checkBox_perPoly.Text = "Per-Poly";
            checkBox_perPoly.ToolTip = "Assess each polygon from overlap, to report minimum overlap area.";
            row1TL.Rows[row1TL.Rows.Count - 1].Cells.Add(checkBox_perPoly);
        }

        void twoD_SettingsUI_sim()
        {
            groupBox_simSettings = new GroupBox();
            TableLayout groupBox_simSettings_table = new TableLayout();
            groupBox_simSettings.Content = groupBox_simSettings_table;
            groupBox_simSettings.Text = "Simulation Settings";

            groupBox_simSettings_table.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            groupBox_simSettings_table.Rows[groupBox_simSettings_table.Rows.Count - 1].Cells.Add(tc0);

            twoD_SettingsUI_sim_row0(tc0);

            groupBox_simSettings_table.Rows.Add(new TableRow());
            TableCell tc1 = new TableCell();
            groupBox_simSettings_table.Rows[groupBox_simSettings_table.Rows.Count - 1].Cells.Add(tc1);

            Panel tc1_p = new Panel();
            tc1.Control = tc1_p;
            TableLayout tc1_tl = new TableLayout();
            tc1_p.Content = tc1_tl;

            tc1_tl.Rows.Add(new TableRow());

            checkBox_linkCDUVariation = new CheckBox();
            checkBox_linkCDUVariation.Text = "Link variation for tip and side CDU";
            checkBox_linkCDUVariation.ToolTip = "Allow for independent tip and side CDU variation, if enabled.";
            tc1_tl.Rows[tc1_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_linkCDUVariation });

            checkBox_CSV = new CheckBox();
            checkBox_CSV.Text = "CSV";
            checkBox_CSV.ToolTip = "Write out a CSV file containing the result for each case and its inputs. Allows for offline deep-dive review.";
            tc1_tl.Rows[tc1_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_CSV });

            checkBox_external = new CheckBox();
            checkBox_external.Text = "External";
            checkBox_external.ToolTip = "Write out a file containing the result for each case and its inputs. Will require significantly more memory.";
            tc1_tl.Rows[tc1_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_external });

            comboBox_externalTypes = new DropDown();
            comboBox_externalTypes.DataContext = DataContext;
            comboBox_externalTypes.BindDataContext(c => c.DataStore, (UIStringLists m) => m.externalTypeList);
            comboBox_externalTypes.ToolTip = "Choose your external file type";
            tc1_tl.Rows[tc1_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = comboBox_externalTypes });

            tc1_tl.Rows[tc1_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            groupBox_simSettings_table.Rows.Add(new TableRow());
            TableCell tc2 = new TableCell();
            groupBox_simSettings_table.Rows[groupBox_simSettings_table.Rows.Count - 1].Cells.Add(tc2);

            Panel tc2_p = new Panel();
            tc2.Control = tc2_p;
            TableLayout tc2_tl = new TableLayout();
            tc2_p.Content = tc2_tl;

            tc2_tl.Rows.Add(new TableRow());

            lbl_rng = new Label();
            lbl_rng.Text = "RNG";
            lbl_rng.ToolTip = "Choice of random number generators. Cryptographic is 10% slower, but more rigorous.";
            tc2_tl.Rows[tc2_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_rng });

            comboBox_RNG = new DropDown();
            comboBox_RNG.DataContext = DataContext;
            comboBox_RNG.BindDataContext(c => c.DataStore, (UIStringLists m) => m.rngTypeList);
            comboBox_RNG.ToolTip = "Choice of random number generators. Cryptographic is 10% slower, but more rigorous.";
            tc2_tl.Rows[tc2_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = comboBox_RNG });

            groupBox_simSettings_table.Rows.Add(new TableRow());
            TableCell tc3 = new TableCell();
            groupBox_simSettings_table.Rows[groupBox_simSettings_table.Rows.Count - 1].Cells.Add(tc3);

            groupBox_simSettings_table.Rows[groupBox_simSettings_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            Panel tc3_p = new Panel();
            tc3.Control = tc3_p;
            TableLayout tc3_tl = new TableLayout();
            tc3_p.Content = tc3_tl;

            tc3_tl.Rows.Add(new TableRow());

            checkBox_debugCalc = new CheckBox();
            checkBox_debugCalc.Text = "Debug";
            checkBox_debugCalc.ToolTip = "Enable debug (calculation engine support depending).";
            tc3_tl.Rows[tc3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_debugCalc });

            tc3_tl.Rows[tc3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            lbl_ssTotalPoints = new Label();
            lbl_ssTotalPoints.Text = "Total # points";
            tc3_tl.Rows[tc3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_ssTotalPoints });

            text_ssTotalPoints = new TextBox();
            text_ssTotalPoints.ReadOnly = true;
            tc3_tl.Rows[tc3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = text_ssTotalPoints });


            groupBox_simSettings_table.Rows.Add(new TableRow()); // padding
        }

        void twoD_SettingsUI_sim_row0(TableCell tc)
        {
            Panel rp0 = new Panel();
            tc.Control = rp0;
            TableLayout r0_tl = new TableLayout();
            rp0.Content = r0_tl;
            r0_tl.Rows.Add(new TableRow());

            lbl_ssNumOfCases = new Label();
            lbl_ssNumOfCases.Text = "Number of Cases";
            lbl_ssNumOfCases.ToolTip = "Total number of cases to evaluate.";
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_ssNumOfCases });

            num_ssNumOfCases = new NumericStepper();
            num_ssNumOfCases.Value = 25000;
            num_ssNumOfCases.Increment = 1;
            num_ssNumOfCases.MinValue = 1;
            setSize(num_ssNumOfCases, 80, num_Height);
            num_ssNumOfCases.ToolTip = "Total number of cases to evaluate.";
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_ssNumOfCases) });

            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding

            checkBox_greedyMultiCPU = new CheckBox();
            checkBox_greedyMultiCPU.Text = "All CPUs (multi CPU)";
            checkBox_greedyMultiCPU.ToolTip = "In multi-threaded runs, use all cores on system, rather than leaving one idle";
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_greedyMultiCPU });

            r0_tl.Rows.Add(new TableRow());

            lbl_ssPrecision = new Label();
            lbl_ssPrecision.Text = "Resolution (nm)";
            lbl_ssPrecision.ToolTip = "Spacing interval for points on edges.";
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_ssPrecision });

            num_ssPrecision = new NumericStepper();
            num_ssPrecision.Value = 1.0;
            num_ssPrecision.Increment = 0.1;
            num_ssPrecision.MinValue = 0.01;
            num_ssPrecision.DecimalPlaces = 2;
            num_ssPrecision.ToolTip = "Spacing interval for points on edges.";
            setSize(num_ssPrecision, 80, num_Height);
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_ssPrecision) });

            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding

            checkBox_LERMode = new CheckBox();
            checkBox_LERMode.Text = "LER as LWR/sqrt(2)";
            checkBox_LERMode.ToolTip = "Define LER as LWR/1.414 instead of LWR/2.";
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_LERMode });

            r0_tl.Rows.Add(new TableRow());

            lbl_cornerSegments = new Label();
            lbl_cornerSegments.Text = "Corner Segments";
            lbl_cornerSegments.ToolTip = "Number of segments to create in corners.";
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_cornerSegments });

            num_cornerSegments = new NumericStepper();
            num_cornerSegments.Value = 90;
            num_cornerSegments.Increment = 1;
            num_cornerSegments.MinValue = 2;
            setSize(num_cornerSegments, 60, num_Height);
            num_cornerSegments.ToolTip = "Number of segments to create in corners.";
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_cornerSegments) });

            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null }); // padding

            checkBox_limitCornerPoints = new CheckBox();
            checkBox_limitCornerPoints.Text = "Optimize corner pt density";
            checkBox_limitCornerPoints.ToolTip = "Prevent points in corners being spaced more closely than the resolution limit for edges.";
            r0_tl.Rows[r0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_limitCornerPoints });
        }

        void twoD_SettingsUI_geoEqtn(TableCell tc)
        {
            Panel p = new Panel();
            TableLayout tl = new TableLayout();
            p.Content = tl;
            tc.Control = TableLayout.AutoSized(p, centered: true);

            tl.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            tl.Rows[0].Cells.Add(tc0);

            comboBox_geoEqtn_Op = new DropDown[CentralProperties.maxLayersForMC];
            label_geoEqtn_Op = new Label[CentralProperties.maxLayersForMC];
            comboBox_geoEqtn_Op_2Layer = new DropDown[CentralProperties.maxLayersForMC / 2];
            comboBox_geoEqtn_Op_4Layer = new DropDown[comboBox_geoEqtn_Op_2Layer.Length / 2];
            comboBox_geoEqtn_Op_8Layer = new DropDown[comboBox_geoEqtn_Op_4Layer.Length / 2];

            groupBox_GeoEqtn = new GroupBox();
            tc0.Control = groupBox_GeoEqtn;
            tl.Rows[0].Cells.Add(new TableCell() { Control = null });

            TableLayout groupBox_GeoEqtn_table = new TableLayout();
            groupBox_GeoEqtn.Content = groupBox_GeoEqtn_table;
            groupBox_GeoEqtn.Text = "Geometric Equation";

            TableRow tr0 = new TableRow();
            groupBox_GeoEqtn_table.Rows.Add(tr0);

            groupBox_GeoEqtnA = new GroupBox();
            TableLayout groupBox_GeoEqtnA_table = new TableLayout();
            groupBox_GeoEqtnA.Content = groupBox_GeoEqtnA_table;
            groupBox_GeoEqtnA.Text = "Geometric Equation A";
            tr0.Cells.Add(new TableCell() { Control = groupBox_GeoEqtnA });

            TableRow tr1 = new TableRow();
            groupBox_GeoEqtn_table.Rows.Add(tr1);

            label_AB = new Label();
            label_AB.Text = "testing 1 2 3";
            label_AB.TextAlignment = TextAlignment.Center;
            tr1.Cells.Add(new TableCell() { Control = label_AB });

            TableRow tr2 = new TableRow();
            groupBox_GeoEqtn_table.Rows.Add(tr2);

            groupBox_GeoEqtnB = new GroupBox();
            TableLayout groupBox_GeoEqtnB_table = new TableLayout();
            groupBox_GeoEqtnB.Content = groupBox_GeoEqtnB_table;
            groupBox_GeoEqtnB.Text = "Geometric Equation B";
            tr2.Cells.Add(new TableCell() { Control = groupBox_GeoEqtnB });

            for (int i = 0; i < comboBox_geoEqtn_Op_8Layer.Length; i++)
            {
                comboBox_geoEqtn_Op_8Layer[i] = new DropDown();
                comboBox_geoEqtn_Op_8Layer[i].Width = 40;
                comboBox_geoEqtn_Op_8Layer[i].BindDataContext(c => c.DataStore, (UIStringLists m) => m.booleanList);
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_4Layer.Length; i++)
            {
                comboBox_geoEqtn_Op_4Layer[i] = new DropDown();
                comboBox_geoEqtn_Op_4Layer[i].Width = 40;
                comboBox_geoEqtn_Op_4Layer[i].BindDataContext(c => c.DataStore, (UIStringLists m) => m.booleanList);
            }

            for (int i = 0; i < comboBox_geoEqtn_Op_2Layer.Length; i++)
            {
                comboBox_geoEqtn_Op_2Layer[i] = new DropDown();
                comboBox_geoEqtn_Op_2Layer[i].Width = 40;
                comboBox_geoEqtn_Op_2Layer[i].BindDataContext(c => c.DataStore, (UIStringLists m) => m.booleanList);
            }

            for (int i = 0; i < comboBox_geoEqtn_Op.Length; i++)
            {
                comboBox_geoEqtn_Op[i] = new DropDown();
                comboBox_geoEqtn_Op[i].Width = 40;
                comboBox_geoEqtn_Op[i].BindDataContext(c => c.DataStore, (UIStringLists m) => m.notList);

                label_geoEqtn_Op[i] = new Label();
                label_geoEqtn_Op[i].TextColor = UIHelper.myColorToColor(commonVars.getColors().simPreviewColors[i]);
                label_geoEqtn_Op[i].Text = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                label_geoEqtn_Op[i].Width = 50;

                var i2 = i;

                label_geoEqtn_Op[i2].MouseDoubleClick += delegate(object sender, MouseEventArgs e)
                {
                    goToLayerUI(i2);
                    e.Handled = true;
                };

            }

            groupBox_GeoEqtnA_table.Rows.Add(new TableRow());
            TableCell a_tc = new TableCell();
            groupBox_GeoEqtnA_table.Rows[0].Cells.Add(a_tc);
            twoD_SettingsUI_geoEqtnA(a_tc);

            groupBox_GeoEqtnB_table.Rows.Add(new TableRow());
            TableCell b_tc = new TableCell();
            groupBox_GeoEqtnB_table.Rows[0].Cells.Add(b_tc);
            twoD_SettingsUI_geoEqtnB(b_tc);
        }

        void twoD_SettingsUI_geoEqtnA(TableCell tc)
        {
            Panel p = new Panel();

            tc.Control = p;

            TableLayout outer_tl = new TableLayout();
            p.Content = outer_tl;
            TableRow row0 = new TableRow();
            outer_tl.Rows.Add(row0);
            aRow0(row0);

            TableRow row1 = new TableRow();
            outer_tl.Rows.Add(row1);
            aRow1(row1);

            TableRow row2 = new TableRow();
            outer_tl.Rows.Add(row2);
            aRow2(row2);

            TableRow padding = new TableRow();
            padding.ScaleHeight = true;
            outer_tl.Rows.Add(padding);
        }

        void aRow0(TableRow row0)
        {
            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[0] });
            row0.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[0] });

            btn_Switch12 = new Button();
            setSize(btn_Switch12, 10, 18);
            btn_Switch12.Click += switchSimulationLayers12Over;
            row0.Cells.Add(new TableCell() { Control = btn_Switch12 });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_2Layer[0] });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[1] });
            row0.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[1] });

            btn_Switch23 = new Button();
            setSize(btn_Switch23, 10, 18);
            btn_Switch23.Click += switchSimulationLayers23Over;
            row0.Cells.Add(new TableCell() { Control = btn_Switch23 });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_4Layer[0] });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[2] });
            row0.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[2] });

            btn_Switch34 = new Button();
            setSize(btn_Switch34, 10, 18);
            btn_Switch34.Click += switchSimulationLayers34Over;
            row0.Cells.Add(new TableCell() { Control = btn_Switch34 });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_2Layer[1] });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[3] });
            row0.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[3] });
        }

        void aRow1(TableRow row1)
        {
            row1.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_8Layer[0] });

            btn_Switch15 = new Button();
            setSize(btn_Switch15, 38, 10);
            btn_Switch15.Click += switchSimulationLayers15Over;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_Switch15) });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            btn_Switch26 = new Button();
            setSize(btn_Switch26, 38, 10);
            btn_Switch26.Click += switchSimulationLayers26Over;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_Switch26) });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            btn_Switch37 = new Button();
            setSize(btn_Switch37, 38, 10);
            btn_Switch37.Click += switchSimulationLayers37Over;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_Switch37) });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            btn_Switch48 = new Button();
            setSize(btn_Switch48, 38, 10);
            btn_Switch48.Click += switchSimulationLayers48ver;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_Switch48) });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            btn_SwitchA = new Button();
            setSize(btn_SwitchA, 38, 10);
            btn_SwitchA.Click += switchSimulationAllALayersOver;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_SwitchA) });
        }

        void aRow2(TableRow row2)
        {
            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[4] });
            row2.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[4] });

            btn_Switch56 = new Button();
            setSize(btn_Switch56, 10, 18);
            btn_Switch56.Click += switchSimulationLayers56Over;
            row2.Cells.Add(new TableCell() { Control = btn_Switch56 });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_2Layer[2] });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[5] });
            row2.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[5] });

            btn_Switch67 = new Button();
            setSize(btn_Switch67, 10, 18);
            btn_Switch67.Click += switchSimulationLayers67Over;
            row2.Cells.Add(new TableCell() { Control = btn_Switch67 });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_4Layer[1] });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[6] });
            row2.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[6] });

            btn_Switch78 = new Button();
            setSize(btn_Switch78, 10, 18);
            btn_Switch78.Click += switchSimulationLayers78Over;
            row2.Cells.Add(new TableCell() { Control = btn_Switch78 });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_2Layer[3] });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[7] });
            row2.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[7] });
        }

        void twoD_SettingsUI_geoEqtnB(TableCell tc)
        {
            Panel p = new Panel();

            tc.Control = p;

            TableLayout outer_tl = new TableLayout();
            p.Content = outer_tl;
            TableRow row0 = new TableRow();
            outer_tl.Rows.Add(row0);
            bRow0(row0);

            TableRow row1 = new TableRow();
            outer_tl.Rows.Add(row1);
            bRow1(row1);

            TableRow row2 = new TableRow();
            outer_tl.Rows.Add(row2);
            bRow2(row2);

            TableRow padding = new TableRow();
            padding.ScaleHeight = true;
            outer_tl.Rows.Add(padding);
        }

        void bRow0(TableRow row0)
        {
            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[8] });
            row0.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[8] });

            btn_Switch910 = new Button();
            setSize(btn_Switch910, 10, 18);
            btn_Switch910.Click += switchSimulationLayers910Over;
            row0.Cells.Add(new TableCell() { Control = btn_Switch910 });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_2Layer[4] });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[9] });
            row0.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[9] });

            btn_Switch1011 = new Button();
            setSize(btn_Switch1011, 10, 18);
            btn_Switch1011.Click += switchSimulationLayers1011Over;
            row0.Cells.Add(new TableCell() { Control = btn_Switch1011 });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_4Layer[2] });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[10] });
            row0.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[10] });

            btn_Switch1112 = new Button();
            setSize(btn_Switch1112, 10, 18);
            btn_Switch1112.Click += switchSimulationLayers1112Over;
            row0.Cells.Add(new TableCell() { Control = btn_Switch1112 });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_2Layer[5] });

            row0.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[11] });
            row0.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[11] });

        }

        void bRow1(TableRow row1)
        {
            row1.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_8Layer[1] });

            btn_Switch913 = new Button();
            setSize(btn_Switch913, 38, 10);
            btn_Switch913.Click += switchSimulationLayers913Over;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_Switch913) });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            btn_Switch1014 = new Button();
            setSize(btn_Switch1014, 38, 10);
            btn_Switch1014.Click += switchSimulationLayers26Over;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_Switch1014) });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            btn_Switch1115 = new Button();
            setSize(btn_Switch1115, 38, 10);
            btn_Switch1115.Click += switchSimulationLayers1115Over;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_Switch1115) });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            btn_Switch1216 = new Button();
            setSize(btn_Switch1216, 38, 10);
            btn_Switch1216.Click += switchSimulationLayers1216Over;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_Switch1216) });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            row1.Cells.Add(new TableCell() { Control = null });

            btn_SwitchB = new Button();
            setSize(btn_SwitchB, 38, 10);
            btn_SwitchB.Click += switchSimulationAllBLayersOver;
            row1.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(btn_SwitchB) });
        }

        void bRow2(TableRow row2)
        {
            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[12] });
            row2.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[12] });

            btn_Switch1314 = new Button();
            setSize(btn_Switch1314, 10, 18);
            btn_Switch1314.Click += switchSimulationLayers1314Over;
            row2.Cells.Add(new TableCell() { Control = btn_Switch1314 });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_2Layer[6] });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[13] });
            row2.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[13] });

            btn_Switch1415 = new Button();
            setSize(btn_Switch1415, 10, 18);
            btn_Switch1415.Click += switchSimulationLayers1415Over;
            row2.Cells.Add(new TableCell() { Control = btn_Switch1415 });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_4Layer[3] });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[14] });
            row2.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[14] });

            btn_Switch1516 = new Button();
            setSize(btn_Switch1516, 10, 18);
            btn_Switch1516.Click += switchSimulationLayers1516Over;
            row2.Cells.Add(new TableCell() { Control = btn_Switch1516 });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op_2Layer[7] });

            row2.Cells.Add(new TableCell() { Control = comboBox_geoEqtn_Op[15] });
            row2.Cells.Add(new TableCell() { Control = label_geoEqtn_Op[15] });
        }

        void twoD_SettingsUI_replay(TableCell tc)
        {
            Panel p = new Panel();
            TableLayout tl = new TableLayout();
            p.Content = tl;
            tc.Control = tc.Control = p; //TableLayout.AutoSized(p, centered: true);

            tl.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            tl.Rows[0].Cells.Add(tc0);

            groupBox_replay = new GroupBox();
            tc0.Control = groupBox_replay;
            tl.Rows[0].Cells.Add(new TableCell() { Control = null });

            TableLayout groupBox_replay_table = new TableLayout();
            TableRow tr0 = new TableRow();
            groupBox_replay_table.Rows.Add(tr0);
            groupBox_replay.Content = groupBox_replay_table;
            groupBox_replay.Text = "Replay";

            button_replay = new Button();
            button_replay.Text = "Load CSV File";
            button_replay.ToolTip = "Load CSV file generated from a run using this project file.\n\rThis will allow replay of the cases.";
            // setSize(button_replay, replayButtonWidth, replayButtonHeight);
            tr0.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(button_replay) });

            checkBox_replay = new CheckBox();
            checkBox_replay.Text = "Enable";
            checkBox_replay.ToolTip = "Enable replay using a CSV file from the loaded project";
            tr0.Cells.Add(new TableCell() { Control = checkBox_replay });

            num_replay = new NumericStepper();
            num_replay.MinValue = 0;
            num_replay.MaxValue = 0;
            num_replay.Increment = 1;
            num_replay.ToolTip = "Which case to view from the loaded CSV file";
            setSize(num_replay, replayNumWidth, replayNumHeight);
            tr0.Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_replay) });

            tr0.Cells.Add(new TableCell() { Control = null }); // padding
        }

        void twoD_SettingsUI_results(TableCell tc)
        {
            Panel p = new Panel();
            tc.Control = TableLayout.AutoSized(p);

            TableLayout tl = new TableLayout();
            tl.Rows.Add(new TableRow());

            lbl_testArea = new Label();
            lbl_testArea.Text = "Result for this run";
            lbl_testArea.TextAlignment = TextAlignment.Left;
            tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_testArea });

            text_testArea = new TextBox();
            text_testArea.ReadOnly = true;
            setSize(text_testArea, 470, 10);
            text_testArea.Text = "N/A";

            tl.Rows[0].Cells.Add(new TableCell() { Control = text_testArea });

            p.Content = tl;
        }
    }
}
