using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Boolean
        GroupBox groupBox_layerBoolean_exp;
        DropDown comboBox_layerBooleanOpAB_exp, comboBox_layerBooleanOpA_exp, comboBox_layerBooleanOpB_exp, comboBox_layerTipLocations_boolean_exp;
        RadioButton[] rB_layerBooleanA_exp, rB_layerBooleanB_exp;
        Label lbl_layerTipLocations_boolean_exp, lbl_rayExtension;
        NumericStepper num_rayExtension;

        void twoD_LayerUISetup_boolean_exp()
        {
            Application.Instance.Invoke(() =>
            {
                groupBox_layerBoolean_exp = new GroupBox();
                TableLayout groupBox_layerBoolean_table = new TableLayout();
                groupBox_layerBoolean_exp.Content = groupBox_layerBoolean_table;
                groupBox_layerBoolean_exp.Text = "Boolean";

                rB_layerBooleanA_exp = new RadioButton[CentralProperties.maxLayersForMC];
                rB_layerBooleanB_exp = new RadioButton[CentralProperties.maxLayersForMC];

                boolean_table(groupBox_layerBoolean_table);

                layerShapeProperties_tcPanel.Content = groupBox_layerBoolean_exp;

                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
            });
        }

        void boolean_table(TableLayout groupBox_layerBoolean_table)
        {
            // Outer table, row 1
            TableRow boolean_tr0 = new TableRow();
            groupBox_layerBoolean_table.Rows.Add(boolean_tr0);
            boolean_tr0.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout row0_tl = new TableLayout();
            boolean_tr0.Cells[0].Control = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            boolean_row0(row0_tl);

            // Outer table, row 2
            TableRow boolean_tr1 = new TableRow();
            groupBox_layerBoolean_table.Rows.Add(boolean_tr1);

            // Table layout within row 2
            TableLayout row1_tl = new TableLayout();
            boolean_tr1.Cells.Add(new TableCell() { Control = row1_tl });
            row1_tl.Rows.Add(new TableRow());

            boolean_row1(row1_tl);

            // Outer table, row 3
            TableRow boolean_tr2 = new TableRow();
            groupBox_layerBoolean_table.Rows.Add(boolean_tr2);

            // Table layout within row 3
            TableLayout row2_tl = new TableLayout();
            boolean_tr2.Cells.Add(new TableCell() { Control = row2_tl });
            row2_tl.Rows.Add(new TableRow());

            boolean_row2(row2_tl);

            // Outer table, row 4
            TableRow boolean_tr3 = new TableRow();
            groupBox_layerBoolean_table.Rows.Add(boolean_tr3);

            // Table layout within row 4
            TableLayout row3_tl = new TableLayout();
            boolean_tr3.Cells.Add(new TableCell() { Control = row3_tl });
            row3_tl.Rows.Add(new TableRow());

            boolean_row3(row3_tl);
        }

        void boolean_row0(TableLayout row0_tl)
        {
            row0_tl.Rows.Add(new TableRow());

            comboBox_layerBooleanOpA_exp = new DropDown();
            row0_tl.Rows[row0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerBooleanOpA_exp) });
            comboBox_layerBooleanOpA_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.notList);

            Panel p = new Panel();
            TableLayout boolATable = new TableLayout();
            p.Content = boolATable;
            row0_tl.Rows[row0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = p });

            TableRow boolA_tr0 = new TableRow();
            boolATable.Rows.Add(boolA_tr0);
            TableRow boolA_tr1 = new TableRow();
            boolATable.Rows.Add(boolA_tr1);

            rB_layerBooleanA_exp[0] = new RadioButton();
            rB_layerBooleanA_exp[0].Text = "0";
            rB_layerBooleanA_exp[0].Checked = true;

            TableCell rB_boolA_0tc = new TableCell();
            rB_boolA_0tc.Control = rB_layerBooleanA_exp[0];

            boolATable.Rows[0].Cells.Add(rB_boolA_0tc);

            int button = 1;
            int rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layerBooleanA_exp[button] = new RadioButton(rB_layerBooleanA_exp[0]);
                rB_layerBooleanA_exp[button].Text = button.ToString();
                rB_layerBooleanA_exp[button].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layerBooleanA_exp[button];
                boolATable.Rows[rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (button + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    rowIndex++;
                }
                button++;
            }
            row0_tl.Rows[row0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
        }

        void boolean_row1(TableLayout row1_tl)
        {
            row1_tl.Rows.Add(new TableRow());

            comboBox_layerBooleanOpAB_exp = new DropDown();
            row1_tl.Rows[row1_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerBooleanOpAB_exp) });
            comboBox_layerBooleanOpAB_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.booleanList);

            row1_tl.Rows[row1_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
        }

        void boolean_row2(TableLayout row2_tl)
        {
            row2_tl.Rows.Add(new TableRow());

            comboBox_layerBooleanOpB_exp = new DropDown();
            row2_tl.Rows[row2_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerBooleanOpB_exp) });
            comboBox_layerBooleanOpB_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.notList);

            Panel p = new Panel();
            TableLayout boolBTable = new TableLayout();
            p.Content = boolBTable;
            row2_tl.Rows[row2_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = p });

            TableRow boolB_tr0 = new TableRow();
            boolBTable.Rows.Add(boolB_tr0);
            TableRow boolB_tr1 = new TableRow();
            boolBTable.Rows.Add(boolB_tr1);

            rB_layerBooleanB_exp[0] = new RadioButton();
            rB_layerBooleanB_exp[0].Text = "0";
            rB_layerBooleanB_exp[0].Checked = true;

            TableCell rB_boolB_0tc = new TableCell();
            rB_boolB_0tc.Control = rB_layerBooleanB_exp[0];

            boolBTable.Rows[0].Cells.Add(rB_boolB_0tc);

            int button = 1;
            int rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layerBooleanB_exp[button] = new RadioButton(rB_layerBooleanB_exp[0]);
                rB_layerBooleanB_exp[button].Text = button.ToString();
                rB_layerBooleanB_exp[button].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layerBooleanB_exp[button];
                boolBTable.Rows[rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (button + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    rowIndex++;
                }
                button++;
            }
            row2_tl.Rows[row2_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
        }

        void boolean_row3(TableLayout row3_tl)
        {
            row3_tl.Rows.Add(new TableRow());

            lbl_layerTipLocations_boolean_exp = new Label();
            lbl_layerTipLocations_boolean_exp.Text = "Tip Locs";
            row3_tl.Rows[row3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layerTipLocations_boolean_exp });

            comboBox_layerTipLocations_boolean_exp = new DropDown();
            row3_tl.Rows[row3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerTipLocations_boolean_exp) });
            comboBox_layerTipLocations_boolean_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);


            row3_tl.Rows[row3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            lbl_rayExtension = new Label();
            lbl_rayExtension.Text = "Extension";
            lbl_rayExtension.ToolTip = "Extension factor for keyhole raycast.";
            row3_tl.Rows[row3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(lbl_rayExtension) });

            num_rayExtension = new NumericStepper();
            num_rayExtension.MinValue = 1.0;
            num_rayExtension.Increment = 0.1;
            num_rayExtension.DecimalPlaces = 2;
            num_rayExtension.ToolTip = "Line end extension.";
            setSize(num_rayExtension, 55, num_Height);
            row3_tl.Rows[row3_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_rayExtension) });

        }
    }
}
