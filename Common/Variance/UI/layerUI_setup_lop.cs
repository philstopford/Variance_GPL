using Eto.Forms;
using shapeEngine;

namespace Variance;

public partial class MainForm
{
    // 2D Layer Layout Origin Parameters
    private Expander expander_LOP;
    private GroupBox groupBox_LOP;
    private DropDown comboBox_subShapeRef, comboBox_posSubShape;
    private Label lbl_subShapeRef, lbl_globalHorOffset, lbl_globalVerOffset, lbl_rotation, lbl_posSubShape;
    private NumericStepper num_globalHorOffset, num_globalVerOffset, num_rotation;

    private void twoD_LayerUISetup_layoutOriginParameters_expanders(bool toState)
    {
        expander_LOP.Expanded = toState;
    }

    private void twoD_LayerUISetup_layoutOriginParameters(TableCell tc)
    {
        Application.Instance.Invoke(() =>
        {
            TableLayout groupBox_layer_LOP_table = new();
            groupBox_LOP = new() {Content = groupBox_layer_LOP_table};
            expander_LOP = new () {Header = "Layout Origin Parameters",
                Content = groupBox_LOP,
                Expanded = true
            };

            TableLayout t = new();
            t.Rows.Add(new TableRow());
            t.Rows[0].Cells.Add(new TableCell { Control = expander_LOP });
            t.Rows[0].Cells.Add(new TableCell { Control = new Panel(), ScaleWidth = true });

            Panel p = new() {Content = t};
            tc.Control = p;

            lop_row1(groupBox_layer_LOP_table);
            lop_row2(groupBox_layer_LOP_table);
        });
    }

    private void lop_row1(TableLayout LOP_table)
    {
        // Outer table, row 1
        TableRow tr = new();
        LOP_table.Rows.Add(tr);
        tr.Cells.Add(new TableCell());

        // Table layout within row 1
        TableLayout tl = new();
        tr.Cells[0].Control = tl;
        tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell tr0 = new();
        tl.Rows[^1].Cells.Add(tr0);

        TableLayout tl_0 = new();
        tr0.Control = tl_0;

        tl_0.Rows.Add(new TableRow());

        lbl_subShapeRef = new Label
        {
            Text = "Subshape Reference",
            Width = 120,
            ToolTip = "Which subshape to use for placement with respect to the world origin"
        };
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = lbl_subShapeRef });

        comboBox_subShapeRef = new DropDown
        {
            DataContext = DataContext,
            SelectedIndex = 0,
            ToolTip = "Which subshape to use for placement with respect to the world origin"
        };
        comboBox_subShapeRef.BindDataContext(c => c.DataStore, (UIStringLists m) => m.subShapesList_exp);
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = comboBox_subShapeRef });

        tl_0.Rows[^1].Cells.Add(new TableCell { Control = null });

        lbl_posSubShape = new Label
        {
            Text = "Position inside Subshape",
            Width = 140,
            ToolTip = "Which element of the subshape to use for placement with respect to the world origin"
        };
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = lbl_posSubShape });

        comboBox_posSubShape = new DropDown
        {
            DataContext = DataContext,
            SelectedIndex = (int) ShapeSettings.subShapeLocations.BL,
            ToolTip = "Which element of the subshape to use for placement with respect to the world origin"
        };
        comboBox_posSubShape.BindDataContext(c => c.DataStore, (UIStringLists m) => m.subShapePos);
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = comboBox_posSubShape });

    }

    private void lop_row2(TableLayout LOP_table)
    {
        // Outer table, row 1
        TableRow tr = new();
        LOP_table.Rows.Add(tr);
        tr.Cells.Add(new TableCell());

        // Table layout within row 1
        TableLayout tl = new();
        tr.Cells[0].Control = tl;
        tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell tc = new();
        tl.Rows[0].Cells.Add(tc);

        TableLayout tl_0 = new();
        tc.Control = TableLayout.AutoSized(tl_0);

        tl_0.Rows.Add(new TableRow());

        lbl_globalHorOffset = new Label
        {
            Text = "Global Horizontal Offset", ToolTip = "Horizontal offset from the world origin"
        };
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = lbl_globalHorOffset });

        num_globalHorOffset = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_globalHorOffset, 55);
        num_globalHorOffset.ToolTip = "Horizontal offset from the world origin";
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_globalHorOffset) });

        lbl_globalVerOffset = new Label
        {
            Text = "Global Vertical Offset", ToolTip = "Horizontal offset from the world origin"
        };
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = lbl_globalVerOffset });

        num_globalVerOffset = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_globalVerOffset, 55);
        num_globalVerOffset.ToolTip = "Horizontal offset from the world origin";
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_globalVerOffset) });

        tl_0.Rows.Add(new TableRow());

        lbl_rotation = new Label
        {
            Text = "Rotation",
            Width = 70,
            ToolTip = "Counter-clockwise rotation around center of bounding box of shape"
        };
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = lbl_rotation });

        num_rotation = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_rotation, 55);
        num_rotation.ToolTip = "Counter-clockwise rotation around center of bounding box of shape";
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_rotation) });

        lbl_lithoWobble = new Label {Text = "Wobble"};
        lbl_lithoWobble.MouseDoubleClick += wobble_RNG;
        lbl_lithoWobble.ToolTip = "3-sigma rotational variation.";
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoWobble });

        num_lithoWobble = new NumericStepper
        {
            Increment = 0.1, DecimalPlaces = 2, MinValue = 0, ToolTip = "3-sigma rotational variation."
        };
        setSize(num_lithoWobble, 55);
        tl_0.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoWobble) });
    }
}