﻿using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Shapes
        GroupBox gB_subShapes;
        NumericStepper num_subshape_hl, num_subshape2_hl, num_subshape3_hl,
                        num_subshape_ho, num_subshape2_ho, num_subshape3_ho,
                        num_subshape_vl, num_subshape2_vl, num_subshape3_vl,
                        num_subshape_vo, num_subshape2_vo, num_subshape3_vo;
        Label lbl_subshape_hl, lbl_subshape_ho, lbl_subshape_vl, lbl_subshape_vo, lbl_tipLocations;
        DropDown comboBox_tipLocations, comboBox_tipLocations2, comboBox_tipLocations3;

        void twoD_LayerUISetup_subShape()
        {
            Application.Instance.Invoke(() =>
            {
                gB_subShapes = new GroupBox();
                TableLayout groupBox_layerSubShapes_table = new TableLayout();
                gB_subShapes.Content = TableLayout.AutoSized(groupBox_layerSubShapes_table);
                gB_subShapes.Text = "SubShapes";

                subshapes_row1(groupBox_layerSubShapes_table);

                layerShapeProperties_tcPanel.Content = gB_subShapes;

                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
            });
        }

        void subshapes_row1(TableLayout subShapes_table)
        {
            // Outer table, row 1
            TableRow subshapes_tr0 = new TableRow();
            subShapes_table.Rows.Add(subshapes_tr0);
            subshapes_tr0.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout row0_tl = new TableLayout();
            subshapes_tr0.Cells[0].Control = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell sunshapes_tr0_0 = new TableCell();
            row0_tl.Rows[0].Cells.Add(sunshapes_tr0_0);

            TableLayout subshapes_tr0_0_tl = new TableLayout();
            sunshapes_tr0_0.Control = subshapes_tr0_0_tl;

            subshapes_tr0_0_tl.Rows.Add(new TableRow());

            lbl_subshape_hl = new Label();
            lbl_subshape_hl.Text = "Hor. Length";
            subshapes_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_subshape_hl });

            TableLayout row0_1 = new TableLayout();
            Panel pr0 = new Panel() { Content = row0_1 };
            subshapes_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = pr0 });
            subshapes_tr0_0_tl.Rows[0].Cells[1].ScaleWidth = true;

            subshapes_row0_1(row0_1);

            subshapes_tr0_0_tl.Rows.Add(new TableRow());

            lbl_subshape_ho = new Label();
            lbl_subshape_ho.Text = "Hor. Offset";
            subshapes_tr0_0_tl.Rows[1].Cells.Add(new TableCell() { Control = lbl_subshape_ho });

            TableLayout row1_1 = new TableLayout();
            Panel pr1 = new Panel() { Content = row1_1 };
            subshapes_tr0_0_tl.Rows[1].Cells.Add(new TableCell() { Control = pr1 });
            subshapes_tr0_0_tl.Rows[1].Cells[1].ScaleWidth = true;

            subshapes_row1_1(row1_1);

            subshapes_tr0_0_tl.Rows.Add(new TableRow());

            lbl_tipLocations = new Label();
            lbl_tipLocations.Text = "Tip Locs";
            subshapes_tr0_0_tl.Rows[2].Cells.Add(new TableCell() { Control = lbl_tipLocations });

            TableLayout row2_1 = new TableLayout();
            Panel pr2 = new Panel() { Content = row2_1 };
            subshapes_tr0_0_tl.Rows[2].Cells.Add(new TableCell() { Control = pr2 });
            subshapes_tr0_0_tl.Rows[2].Cells[1].ScaleWidth = true;

            subshapes_row2_1(row2_1);
        }

        void subshapes_row0_1(TableLayout row0_1)
        {
            int numWidth = 55;

            row0_1.Rows.Add(new TableRow());

            num_subshape_hl = new NumericStepper();
            num_subshape_hl.Increment = 0.1;
            num_subshape_hl.DecimalPlaces = 2;
            num_subshape_hl.MinValue = 0;
            setSize(num_subshape_hl, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape_hl) });

            num_subshape2_hl = new NumericStepper();
            num_subshape2_hl.Increment = 0.1;
            num_subshape2_hl.DecimalPlaces = 2;
            num_subshape2_hl.MinValue = 0;
            setSize(num_subshape2_hl, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape2_hl) });

            num_subshape3_hl = new NumericStepper();
            num_subshape3_hl.Increment = 0.1;
            num_subshape3_hl.DecimalPlaces = 2;
            num_subshape3_hl.MinValue = 0;
            setSize(num_subshape3_hl, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape3_hl) });

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = new Panel() });
            row0_1.Rows[0].Cells[row0_1.Rows[0].Cells.Count - 1].ScaleWidth = true;

            lbl_subshape_vl = new Label();
            lbl_subshape_vl.Text = "Ver. Length";

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = lbl_subshape_vl });

            num_subshape_vl = new NumericStepper();
            num_subshape_vl.Increment = 0.1;
            num_subshape_vl.DecimalPlaces = 2;
            num_subshape_vl.MinValue = 0;
            setSize(num_subshape_vl, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape_vl) });

            num_subshape2_vl = new NumericStepper();
            num_subshape2_vl.Increment = 0.1;
            num_subshape2_vl.DecimalPlaces = 2;
            num_subshape2_vl.MinValue = 0;
            setSize(num_subshape2_vl, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape2_vl) });

            num_subshape3_vl = new NumericStepper();
            num_subshape3_vl.Increment = 0.1;
            num_subshape3_vl.DecimalPlaces = 2;
            num_subshape3_vl.MinValue = 0;
            setSize(num_subshape3_vl, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape3_vl) });
        }

        void subshapes_row1_1(TableLayout row1_1)
        {
            int numWidth = 55;

            row1_1.Rows.Add(new TableRow());

            num_subshape_ho = new NumericStepper();
            num_subshape_ho.Increment = 0.1;
            num_subshape_ho.DecimalPlaces = 2;
            setSize(num_subshape_ho, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape_ho) });

            num_subshape2_ho = new NumericStepper();
            num_subshape2_ho.Increment = 0.1;
            num_subshape2_ho.DecimalPlaces = 2;
            setSize(num_subshape2_ho, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape2_ho) });

            num_subshape3_ho = new NumericStepper();
            num_subshape3_ho.Increment = 0.1;
            num_subshape3_ho.DecimalPlaces = 2;
            setSize(num_subshape3_ho, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape3_ho) });

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = new Panel() });
            row1_1.Rows[0].Cells[row1_1.Rows[0].Cells.Count - 1].ScaleWidth = true;

            lbl_subshape_vo = new Label();
            lbl_subshape_vo.Text = "Ver. Offset";

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = lbl_subshape_vo });

            num_subshape_vo = new NumericStepper();
            num_subshape_vo.Increment = 0.1;
            num_subshape_vo.DecimalPlaces = 2;
            setSize(num_subshape_vo, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape_vo) });

            num_subshape2_vo = new NumericStepper();
            num_subshape2_vo.Increment = 0.1;
            num_subshape2_vo.DecimalPlaces = 2;
            setSize(num_subshape2_vo, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape2_vo) });

            num_subshape3_vo = new NumericStepper();
            num_subshape3_vo.Increment = 0.1;
            num_subshape3_vo.DecimalPlaces = 2;
            setSize(num_subshape3_vo, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_subshape3_vo) });
        }

        void subshapes_row2_1(TableLayout row2_1)
        {
            row2_1.Rows.Add(new TableRow());

            comboBox_tipLocations = new DropDown();

            row2_1.Rows[0].Cells.Add(new TableCell() { Control = comboBox_tipLocations });

            comboBox_tipLocations.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

            comboBox_tipLocations2 = new DropDown();

            row2_1.Rows[0].Cells.Add(new TableCell() { Control = comboBox_tipLocations2 });

            comboBox_tipLocations2.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

            comboBox_tipLocations3 = new DropDown();

            row2_1.Rows[0].Cells.Add(new TableCell() { Control = comboBox_tipLocations3 });

            comboBox_tipLocations3.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);
        }
    }
}
