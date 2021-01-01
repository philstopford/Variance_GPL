using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Layout Origin Parameters
        GroupBox groupBox_LOP;
        DropDown comboBox_subShapeRef, comboBox_posSubShape;
        Label lbl_subShapeRef, lbl_globalHorOffset, lbl_globalVerOffset, lbl_rotation, lbl_posSubShape;
        NumericStepper num_globalHorOffset, num_globalVerOffset, num_rotation;

        void twoD_LayerUISetup_layoutOriginParameters(TableCell tc)
        {
            Application.Instance.Invoke(() =>
            {
                groupBox_LOP = new GroupBox();
                groupBox_LOP.Text = "Layout Origin Parameters";
                TableLayout groupBox_layer_LOP_table = new TableLayout();
                groupBox_LOP.Content = groupBox_layer_LOP_table;

                TableLayout t = new TableLayout();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell() { Control = groupBox_LOP });
                t.Rows[0].Cells.Add(new TableCell() { Control = new Panel(), ScaleWidth = true });

                Panel p = new Panel();
                p.Content = t;
                tc.Control = p;

                lop_row1(groupBox_layer_LOP_table);
                lop_row2(groupBox_layer_LOP_table);
            });
        }

        void lop_row1(TableLayout LOP_table)
        {
            // Outer table, row 1
            TableRow tr = new TableRow();
            LOP_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout tl = new TableLayout();
            tr.Cells[0].Control = tl;
            tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell tr0 = new TableCell();
            tl.Rows[tl.Rows.Count - 1].Cells.Add(tr0);

            TableLayout tl_0 = new TableLayout();
            tr0.Control = tl_0;

            tl_0.Rows.Add(new TableRow());

            lbl_subShapeRef = new Label();
            lbl_subShapeRef.Text = "Subshape Reference";
            lbl_subShapeRef.Width = 120;
            lbl_subShapeRef.ToolTip = "Which subshape to use for placement with respect to the world origin";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_subShapeRef });

            comboBox_subShapeRef = new DropDown();
            comboBox_subShapeRef.DataContext = DataContext;
            comboBox_subShapeRef.BindDataContext(c => c.DataStore, (UIStringLists m) => m.subShapesList_exp);
            comboBox_subShapeRef.SelectedIndex = 0;
            comboBox_subShapeRef.ToolTip = "Which subshape to use for placement with respect to the world origin";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = comboBox_subShapeRef });

            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            lbl_posSubShape = new Label();
            lbl_posSubShape.Text = "Position inside Subshape";
            lbl_posSubShape.Width = 140;
            lbl_posSubShape.ToolTip = "Which element of the subshape to use for placement with respect to the world origin";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_posSubShape });

            comboBox_posSubShape = new DropDown();
            comboBox_posSubShape.DataContext = DataContext;
            comboBox_posSubShape.BindDataContext(c => c.DataStore, (UIStringLists m) => m.subShapePos);
            comboBox_posSubShape.SelectedIndex = (int)CommonVars.subShapeLocations.BL;
            comboBox_posSubShape.ToolTip = "Which element of the subshape to use for placement with respect to the world origin";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = comboBox_posSubShape });

        }

        void lop_row2(TableLayout LOP_table)
        {
            // Outer table, row 1
            TableRow tr = new TableRow();
            LOP_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout tl = new TableLayout();
            tr.Cells[0].Control = tl;
            tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell tc = new TableCell();
            tl.Rows[0].Cells.Add(tc);

            TableLayout tl_0 = new TableLayout();
            tc.Control = TableLayout.AutoSized(tl_0);

            tl_0.Rows.Add(new TableRow());

            lbl_globalHorOffset = new Label();
            lbl_globalHorOffset.Text = "Global Hor Offset";
            lbl_globalHorOffset.Width = 120;
            lbl_globalHorOffset.ToolTip = "Horizontal offset from the world origin";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_globalHorOffset });

            num_globalHorOffset = new NumericStepper();
            num_globalHorOffset.Increment = 0.1;
            num_globalHorOffset.DecimalPlaces = 2;
            setSize(num_globalHorOffset, 55, num_Height);
            num_globalHorOffset.ToolTip = "Horizontal offset from the world origin";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_globalHorOffset) });

            lbl_globalVerOffset = new Label();
            lbl_globalVerOffset.Text = "Global Ver Offset";
            lbl_globalVerOffset.Width = 120;
            lbl_globalVerOffset.ToolTip = "Horizontal offset from the world origin";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_globalVerOffset });

            num_globalVerOffset = new NumericStepper();
            num_globalVerOffset.Increment = 0.1;
            num_globalVerOffset.DecimalPlaces = 2;
            setSize(num_globalVerOffset, 55, num_Height);
            num_globalVerOffset.ToolTip = "Horizontal offset from the world origin";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_globalVerOffset) });

            tl_0.Rows.Add(new TableRow());

            lbl_rotation = new Label();
            lbl_rotation.Text = "Rotation";
            lbl_rotation.Width = 70;
            lbl_rotation.ToolTip = "Counter-clockwise rotation around center of bounding box of shape";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_rotation });

            num_rotation = new NumericStepper();
            num_rotation.Increment = 0.1;
            num_rotation.DecimalPlaces = 2;
            setSize(num_rotation, 55, num_Height);
            num_rotation.ToolTip = "Counter-clockwise rotation around center of bounding box of shape";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_rotation) });

            lbl_lithoWobble = new Label();
            lbl_lithoWobble.Text = "Wobble";
            lbl_lithoWobble.MouseDoubleClick += wobble_RNG;
            lbl_lithoWobble.ToolTip = "3-sigma rotational variation.";
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoWobble });

            num_lithoWobble = new NumericStepper();
            num_lithoWobble.Increment = 0.1;
            num_lithoWobble.DecimalPlaces = 2;
            num_lithoWobble.MinValue = 0;
            num_lithoWobble.ToolTip = "3-sigma rotational variation.";
            setSize(num_lithoWobble, 55, (int)(label_Height * uiScaleFactor));
            tl_0.Rows[tl_0.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoWobble) });
        }
    }
}
