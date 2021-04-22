using Eto.Forms;

namespace Variance
{
    public partial class MainForm
    {
        // 2D Layer Boolean
        GroupBox gB_layerBoolean;
        DropDown comboBox_BooleanOpAB, comboBox_BooleanOpA, comboBox_BooleanOpB, comboBox_TipLocations_boolean;
        RadioButton[] rB_BooleanA, rB_BooleanB;
        Label lbl_TipLocations_boolean, lbl_rayExtension;
        NumericStepper num_rayExtension;

        void twoD_LayerUISetup_boolean()
        {
            Application.Instance.Invoke(() =>
            {
                TableLayout groupBox_layerBoolean_table = new TableLayout();
                gB_layerBoolean = new GroupBox {Content = groupBox_layerBoolean_table, Text = "Boolean"};

                rB_BooleanA = new RadioButton[CentralProperties.maxLayersForMC];
                rB_BooleanB = new RadioButton[CentralProperties.maxLayersForMC];

                boolean_table(groupBox_layerBoolean_table);

                layerShapeProperties_tcPanel.Content = gB_layerBoolean;

                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
            });
        }

        void boolean_table(TableLayout layerBoolean_table)
        {
            // Outer table, row 1
            TableRow boolean_tr0 = new TableRow();
            layerBoolean_table.Rows.Add(boolean_tr0);
            boolean_tr0.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout row0_tl = new TableLayout();
            boolean_tr0.Cells[0].Control = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            boolean_row0(row0_tl);

            // Outer table, row 2
            TableRow boolean_tr1 = new TableRow();
            layerBoolean_table.Rows.Add(boolean_tr1);

            // Table layout within row 2
            TableLayout row1_tl = new TableLayout();
            boolean_tr1.Cells.Add(new TableCell { Control = row1_tl });
            row1_tl.Rows.Add(new TableRow());

            boolean_row1(row1_tl);

            // Outer table, row 3
            TableRow boolean_tr2 = new TableRow();
            layerBoolean_table.Rows.Add(boolean_tr2);

            // Table layout within row 3
            TableLayout row2_tl = new TableLayout();
            boolean_tr2.Cells.Add(new TableCell { Control = row2_tl });
            row2_tl.Rows.Add(new TableRow());

            boolean_row2(row2_tl);

            // Outer table, row 4
            TableRow boolean_tr3 = new TableRow();
            layerBoolean_table.Rows.Add(boolean_tr3);

            // Table layout within row 4
            TableLayout row3_tl = new TableLayout();
            boolean_tr3.Cells.Add(new TableCell { Control = row3_tl });
            row3_tl.Rows.Add(new TableRow());

            boolean_row3(row3_tl);
        }

        void boolean_row0(TableLayout row0_tl)
        {
            row0_tl.Rows.Add(new TableRow());

            comboBox_BooleanOpA = new DropDown();
            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_BooleanOpA) });
            comboBox_BooleanOpA.BindDataContext(c => c.DataStore, (UIStringLists m) => m.notList);

            TableLayout boolATable = new TableLayout();
            Panel p = new Panel {Content = boolATable};
            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = p });

            TableRow boolA_tr0 = new TableRow();
            boolATable.Rows.Add(boolA_tr0);
            TableRow boolA_tr1 = new TableRow();
            boolATable.Rows.Add(boolA_tr1);

            rB_BooleanA[0] = new RadioButton {Text = "0", Checked = true};

            TableCell rB_boolA_0tc = new TableCell {Control = rB_BooleanA[0]};

            boolATable.Rows[0].Cells.Add(rB_boolA_0tc);

            int button = 1;
            int rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_BooleanA[button] = new RadioButton(rB_BooleanA[0]) {Text = button.ToString(), Checked = false};
                TableCell tc0 = new TableCell {Control = rB_BooleanA[button]};
                boolATable.Rows[rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (button + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    rowIndex++;
                }
                button++;
            }
            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });
        }

        void boolean_row1(TableLayout row1_tl)
        {
            row1_tl.Rows.Add(new TableRow());

            comboBox_BooleanOpAB = new DropDown();
            row1_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_BooleanOpAB) });
            comboBox_BooleanOpAB.BindDataContext(c => c.DataStore, (UIStringLists m) => m.booleanList);

            row1_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });
        }

        void boolean_row2(TableLayout row2_tl)
        {
            row2_tl.Rows.Add(new TableRow());

            comboBox_BooleanOpB = new DropDown();
            row2_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_BooleanOpB) });
            comboBox_BooleanOpB.BindDataContext(c => c.DataStore, (UIStringLists m) => m.notList);

            TableLayout boolBTable = new TableLayout();
            Panel p = new Panel {Content = boolBTable};
            row2_tl.Rows[^1].Cells.Add(new TableCell { Control = p });

            TableRow boolB_tr0 = new TableRow();
            boolBTable.Rows.Add(boolB_tr0);
            TableRow boolB_tr1 = new TableRow();
            boolBTable.Rows.Add(boolB_tr1);

            rB_BooleanB[0] = new RadioButton {Text = "0", Checked = true};

            TableCell rB_boolB_0tc = new TableCell {Control = rB_BooleanB[0]};

            boolBTable.Rows[0].Cells.Add(rB_boolB_0tc);

            int button = 1;
            int rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_BooleanB[button] = new RadioButton(rB_BooleanB[0]) {Text = button.ToString(), Checked = false};
                TableCell tc0 = new TableCell {Control = rB_BooleanB[button]};
                boolBTable.Rows[rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (button + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    rowIndex++;
                }
                button++;
            }
            row2_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });
        }

        void boolean_row3(TableLayout row3_tl)
        {
            row3_tl.Rows.Add(new TableRow());

            lbl_TipLocations_boolean = new Label {Text = "Tip Locs"};
            row3_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_TipLocations_boolean });

            comboBox_TipLocations_boolean = new DropDown();
            row3_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_TipLocations_boolean) });
            comboBox_TipLocations_boolean.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

            row3_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });

            lbl_rayExtension = new Label {Text = "Extension", ToolTip = "Extension factor for keyhole raycast."};
            row3_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(lbl_rayExtension) });

            num_rayExtension = new NumericStepper
            {
                MinValue = 1.0, Increment = 0.1, DecimalPlaces = 2, ToolTip = "Line end extension."
            };
            setSize(num_rayExtension, 55);
            row3_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_rayExtension) });

        }
    }
}
