using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    // 2D Layer Shapes
    private Expander expander_subShapes;
    private GroupBox groupBox_subShapes;

    private NumericStepper num_subshape_hl, num_subshape2_hl, num_subshape3_hl,
        num_subshape_ho, num_subshape2_ho, num_subshape3_ho,
        num_subshape_vl, num_subshape2_vl, num_subshape3_vl,
        num_subshape_vo, num_subshape2_vo, num_subshape3_vo;

    private Label lbl_subshape_hl, lbl_subshape_ho, lbl_subshape_vl, lbl_subshape_vo, lbl_tipLocations;
    private DropDown comboBox_tipLocations, comboBox_tipLocations2, comboBox_tipLocations3;

    private void twoD_LayerUISetup_subShape_expanders(bool toState)
    {
        expander_subShapes.Expanded = toState;
    }

    private void twoD_LayerUISetup_subShape()
    {
        Application.Instance.Invoke(() =>
        {
            TableLayout groupBox_layerSubShapes_table = new();
            groupBox_subShapes = new() {Content = TableLayout.AutoSized(groupBox_layerSubShapes_table)};
            expander_subShapes = new ()
            {
                Content = groupBox_subShapes,
                Header = "SubShapes",
                Expanded = true
            };

            subshapes_row1(groupBox_layerSubShapes_table);

            layerShapeProperties_tcPanel.Content = expander_subShapes;

            setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
        });
    }

    private void subshapes_row1(TableLayout subShapes_table)
    {
        // Outer table, row 1
        TableRow subshapes_tr0 = new();
        subShapes_table.Rows.Add(subshapes_tr0);
        subshapes_tr0.Cells.Add(new TableCell());

        // Table layout within row 1
        TableLayout row0_tl = new();
        subshapes_tr0.Cells[0].Control = row0_tl;
        row0_tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell sunshapes_tr0_0 = new();
        row0_tl.Rows[0].Cells.Add(sunshapes_tr0_0);

        TableLayout subshapes_tr0_0_tl = new();
        sunshapes_tr0_0.Control = subshapes_tr0_0_tl;

        subshapes_tr0_0_tl.Rows.Add(new TableRow());

        lbl_subshape_hl = new Label {Text = "Hor. Length"};
        subshapes_tr0_0_tl.Rows[0].Cells.Add(new TableCell { Control = lbl_subshape_hl });

        TableLayout row0_1 = new();
        Panel pr0 = new() { Content = row0_1 };
        subshapes_tr0_0_tl.Rows[0].Cells.Add(new TableCell { Control = pr0 });
        subshapes_tr0_0_tl.Rows[0].Cells[1].ScaleWidth = true;

        subshapes_row0_1(row0_1);

        subshapes_tr0_0_tl.Rows.Add(new TableRow());

        lbl_subshape_ho = new Label {Text = "Hor. Offset"};
        subshapes_tr0_0_tl.Rows[1].Cells.Add(new TableCell { Control = lbl_subshape_ho });

        TableLayout row1_1 = new();
        Panel pr1 = new() { Content = row1_1 };
        subshapes_tr0_0_tl.Rows[1].Cells.Add(new TableCell { Control = pr1 });
        subshapes_tr0_0_tl.Rows[1].Cells[1].ScaleWidth = true;

        subshapes_row1_1(row1_1);

        subshapes_tr0_0_tl.Rows.Add(new TableRow());

        lbl_tipLocations = new Label {Text = "Tip Locs"};
        subshapes_tr0_0_tl.Rows[2].Cells.Add(new TableCell { Control = lbl_tipLocations });

        TableLayout row2_1 = new();
        Panel pr2 = new() { Content = row2_1 };
        subshapes_tr0_0_tl.Rows[2].Cells.Add(new TableCell { Control = pr2 });
        subshapes_tr0_0_tl.Rows[2].Cells[1].ScaleWidth = true;

        subshapes_row2_1(row2_1);
    }

    private void subshapes_row0_1(TableLayout row0_1)
    {
        const int numWidth = 55;

        row0_1.Rows.Add(new TableRow());

        num_subshape_hl = new NumericStepper {Increment = 0.1, DecimalPlaces = 2, MinValue = 0};
        setSize(num_subshape_hl, numWidth);

        row0_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape_hl) });

        num_subshape2_hl = new NumericStepper {Increment = 0.1, DecimalPlaces = 2, MinValue = 0};
        setSize(num_subshape2_hl, numWidth);

        row0_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape2_hl) });

        num_subshape3_hl = new NumericStepper {Increment = 0.1, DecimalPlaces = 2, MinValue = 0};
        setSize(num_subshape3_hl, numWidth);

        row0_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape3_hl) });

        row0_1.Rows[0].Cells.Add(new TableCell { Control = new Panel() });
        row0_1.Rows[0].Cells[^1].ScaleWidth = true;

        lbl_subshape_vl = new Label {Text = "Ver. Length"};

        row0_1.Rows[0].Cells.Add(new TableCell { Control = lbl_subshape_vl });

        num_subshape_vl = new NumericStepper {Increment = 0.1, DecimalPlaces = 2, MinValue = 0};
        setSize(num_subshape_vl, numWidth);

        row0_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape_vl) });

        num_subshape2_vl = new NumericStepper {Increment = 0.1, DecimalPlaces = 2, MinValue = 0};
        setSize(num_subshape2_vl, numWidth);

        row0_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape2_vl) });

        num_subshape3_vl = new NumericStepper {Increment = 0.1, DecimalPlaces = 2, MinValue = 0};
        setSize(num_subshape3_vl, numWidth);

        row0_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape3_vl) });
    }

    private void subshapes_row1_1(TableLayout row1_1)
    {
        const int numWidth = 55;

        row1_1.Rows.Add(new TableRow());

        num_subshape_ho = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_subshape_ho, numWidth);

        row1_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape_ho) });

        num_subshape2_ho = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_subshape2_ho, numWidth);

        row1_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape2_ho) });

        num_subshape3_ho = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_subshape3_ho, numWidth);

        row1_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape3_ho) });

        row1_1.Rows[0].Cells.Add(new TableCell { Control = new Panel() });
        row1_1.Rows[0].Cells[^1].ScaleWidth = true;

        lbl_subshape_vo = new Label {Text = "Ver. Offset"};

        row1_1.Rows[0].Cells.Add(new TableCell { Control = lbl_subshape_vo });

        num_subshape_vo = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_subshape_vo, numWidth);

        row1_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape_vo) });

        num_subshape2_vo = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_subshape2_vo, numWidth);

        row1_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape2_vo) });

        num_subshape3_vo = new NumericStepper {Increment = 0.1, DecimalPlaces = 2};
        setSize(num_subshape3_vo, numWidth);

        row1_1.Rows[0].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_subshape3_vo) });
    }

    private void subshapes_row2_1(TableLayout row2_1)
    {
        row2_1.Rows.Add(new TableRow());

        comboBox_tipLocations = new DropDown();

        row2_1.Rows[0].Cells.Add(new TableCell { Control = comboBox_tipLocations });

        comboBox_tipLocations.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

        comboBox_tipLocations2 = new DropDown();

        row2_1.Rows[0].Cells.Add(new TableCell { Control = comboBox_tipLocations2 });

        comboBox_tipLocations2.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

        comboBox_tipLocations3 = new DropDown();

        row2_1.Rows[0].Cells.Add(new TableCell { Control = comboBox_tipLocations3 });

        comboBox_tipLocations3.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);
    }
}