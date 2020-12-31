using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Layout Origin Parameters
        GroupBox groupBox_layer_LOP_exp;
        DropDown comboBox_layerSubShapeRef_exp, comboBox_layerPosSubShape_exp;
        Label lbl_layerSubShapeRef_exp, lbl_layerGlobalHorOffset_exp, lbl_layerGlobalVerOffset_exp, lbl_layerRotation_exp, lbl_layerPosSubShape_exp;
        NumericStepper num_layerGlobalHorOffset_exp, num_layerGlobalVerOffset_exp, num_layerRotation_exp;

        void twoD_LayerUISetup_layoutOriginParameters_exp(TableCell tc)
        {
            Application.Instance.Invoke(() =>
            {
                groupBox_layer_LOP_exp = new GroupBox();
                groupBox_layer_LOP_exp.Text = "Layout Origin Parameters";
                TableLayout groupBox_layer_LOP_table = new TableLayout();
                groupBox_layer_LOP_exp.Content = groupBox_layer_LOP_table;

                TableLayout t = new TableLayout();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell() { Control = groupBox_layer_LOP_exp });
                t.Rows[0].Cells.Add(new TableCell() { Control = new Panel(), ScaleWidth = true });

                Panel p = new Panel();
                p.Content = t;
                tc.Control = p;

                lop_row1(groupBox_layer_LOP_table);
                lop_row2(groupBox_layer_LOP_table);
            });
        }

        void lop_row1(TableLayout groupBox_layer_LOP_table)
        {
            // Outer table, row 1
            TableRow lop_tr0 = new TableRow();
            groupBox_layer_LOP_table.Rows.Add(lop_tr0);
            lop_tr0.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout row0_tl = new TableLayout();
            lop_tr0.Cells[0].Control = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lop_tr0_0 = new TableCell();
            row0_tl.Rows[0].Cells.Add(lop_tr0_0);

            TableLayout lop_tr0_0_tl = new TableLayout();
            lop_tr0_0.Control = lop_tr0_0_tl;

            lop_tr0_0_tl.Rows.Add(new TableRow());

            lbl_layerSubShapeRef_exp = new Label();
            lbl_layerSubShapeRef_exp.Text = "Subshape Reference";
            lbl_layerSubShapeRef_exp.Width = 120;
            lbl_layerSubShapeRef_exp.ToolTip = "Which subshape to use for placement with respect to the world origin";
            lop_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layerSubShapeRef_exp });

            comboBox_layerSubShapeRef_exp = new DropDown();
            comboBox_layerSubShapeRef_exp.DataContext = DataContext;
            comboBox_layerSubShapeRef_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.subShapesList_exp);
            comboBox_layerSubShapeRef_exp.SelectedIndex = 0;
            comboBox_layerSubShapeRef_exp.ToolTip = "Which subshape to use for placement with respect to the world origin";
            lop_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = comboBox_layerSubShapeRef_exp });

            lop_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = null });

            lbl_layerPosSubShape_exp = new Label();
            lbl_layerPosSubShape_exp.Text = "Position inside Subshape";
            lbl_layerPosSubShape_exp.Width = 140;
            lbl_layerPosSubShape_exp.ToolTip = "Which element of the subshape to use for placement with respect to the world origin";
            lop_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layerPosSubShape_exp });

            comboBox_layerPosSubShape_exp = new DropDown();
            comboBox_layerPosSubShape_exp.DataContext = DataContext;
            comboBox_layerPosSubShape_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.subShapePos);
            comboBox_layerPosSubShape_exp.SelectedIndex = (int)CommonVars.subShapeLocations.BL;
            comboBox_layerPosSubShape_exp.ToolTip = "Which element of the subshape to use for placement with respect to the world origin";
            lop_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = comboBox_layerPosSubShape_exp });

        }

        void lop_row2(TableLayout groupBox_layer_LOP_table)
        {
            // Outer table, row 1
            TableRow tr = new TableRow();
            groupBox_layer_LOP_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout tc_tl = new TableLayout();
            tr.Cells[0].Control = tc_tl;
            tc_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell tc_0 = new TableCell();
            tc_tl.Rows[0].Cells.Add(tc_0);

            TableLayout tc_0_tl = new TableLayout();
            tc_0.Control = TableLayout.AutoSized(tc_0_tl);

            tc_0_tl.Rows.Add(new TableRow());

            lbl_layerGlobalHorOffset_exp = new Label();
            lbl_layerGlobalHorOffset_exp.Text = "Global Hor Offset";
            lbl_layerGlobalHorOffset_exp.Width = 120;
            lbl_layerGlobalHorOffset_exp.ToolTip = "Horizontal offset from the world origin";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layerGlobalHorOffset_exp });

            num_layerGlobalHorOffset_exp = new NumericStepper();
            num_layerGlobalHorOffset_exp.Increment = 0.1;
            num_layerGlobalHorOffset_exp.DecimalPlaces = 2;
            setSize(num_layerGlobalHorOffset_exp, 55, num_Height);
            num_layerGlobalHorOffset_exp.ToolTip = "Horizontal offset from the world origin";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layerGlobalHorOffset_exp) });

            lbl_layerGlobalVerOffset_exp = new Label();
            lbl_layerGlobalVerOffset_exp.Text = "Global Ver Offset";
            lbl_layerGlobalVerOffset_exp.Width = 120;
            lbl_layerGlobalVerOffset_exp.ToolTip = "Horizontal offset from the world origin";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layerGlobalVerOffset_exp });

            num_layerGlobalVerOffset_exp = new NumericStepper();
            num_layerGlobalVerOffset_exp.Increment = 0.1;
            num_layerGlobalVerOffset_exp.DecimalPlaces = 2;
            setSize(num_layerGlobalVerOffset_exp, 55, num_Height);
            num_layerGlobalVerOffset_exp.ToolTip = "Horizontal offset from the world origin";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layerGlobalVerOffset_exp) });

            tc_0_tl.Rows.Add(new TableRow());

            lbl_layerRotation_exp = new Label();
            lbl_layerRotation_exp.Text = "Rotation";
            lbl_layerRotation_exp.Width = 70;
            lbl_layerRotation_exp.ToolTip = "Counter-clockwise rotation around center of bounding box of shape";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layerRotation_exp });

            num_layerRotation_exp = new NumericStepper();
            num_layerRotation_exp.Increment = 0.1;
            num_layerRotation_exp.DecimalPlaces = 2;
            setSize(num_layerRotation_exp, 55, num_Height);
            num_layerRotation_exp.ToolTip = "Counter-clockwise rotation around center of bounding box of shape";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layerRotation_exp) });

            lbl_layer_lithoWobble_exp = new Label();
            lbl_layer_lithoWobble_exp.Text = "Wobble";
            lbl_layer_lithoWobble_exp.MouseDoubleClick += wobble_RNG;
            lbl_layer_lithoWobble_exp.ToolTip = "3-sigma rotational variation.";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layer_lithoWobble_exp });

            num_layer_lithoWobble_exp = new NumericStepper();
            num_layer_lithoWobble_exp.Increment = 0.1;
            num_layer_lithoWobble_exp.DecimalPlaces = 2;
            num_layer_lithoWobble_exp.MinValue = 0;
            num_layer_lithoWobble_exp.ToolTip = "3-sigma rotational variation.";
            setSize(num_layer_lithoWobble_exp, 55, (int)(label_Height * uiScaleFactor));
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoWobble_exp) });
        }
    }
}
