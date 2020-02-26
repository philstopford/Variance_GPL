using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Shapes
        GroupBox groupBox_layerSubShapes_exp;
        NumericStepper num_layer_subshape_hl_exp, num_layer_subshape2_hl_exp, num_layer_subshape3_hl_exp,
                        num_layer_subshape_ho_exp, num_layer_subshape2_ho_exp, num_layer_subshape3_ho_exp,
                        num_layer_subshape_vl_exp, num_layer_subshape2_vl_exp, num_layer_subshape3_vl_exp,
                        num_layer_subshape_vo_exp, num_layer_subshape2_vo_exp, num_layer_subshape3_vo_exp;
        Label lbl_layer_subshape_hl_exp, lbl_layer_subshape_ho_exp, lbl_layer_subshape_vl_exp, lbl_layer_subshape_vo_exp, lbl_layerTipLocations_exp;
        DropDown comboBox_layerTipLocations_exp, comboBox_layerTipLocations2_exp, comboBox_layerTipLocations3_exp;

        void twoD_LayerUISetup_subShape_exp()
        {
            Application.Instance.Invoke(() =>
            {
                groupBox_layerSubShapes_exp = new GroupBox();
                TableLayout groupBox_layerSubShapes_table = new TableLayout();
                groupBox_layerSubShapes_exp.Content = TableLayout.AutoSized(groupBox_layerSubShapes_table);
                groupBox_layerSubShapes_exp.Text = "SubShapes";

                subshapes_row1(groupBox_layerSubShapes_table);

                layerShapeProperties_tcPanel.Content = groupBox_layerSubShapes_exp;

                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
            });
        }

        void subshapes_row1(TableLayout groupBox_layerSubShapes_table)
        {
            // Outer table, row 1
            TableRow subshapes_tr0 = new TableRow();
            groupBox_layerSubShapes_table.Rows.Add(subshapes_tr0);
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

            lbl_layer_subshape_hl_exp = new Label();
            lbl_layer_subshape_hl_exp.Text = "Hor. Length";
            subshapes_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_subshape_hl_exp });

            TableLayout row0_1 = new TableLayout();
            Panel pr0 = new Panel() { Content = row0_1 };
            subshapes_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = pr0 });
            subshapes_tr0_0_tl.Rows[0].Cells[1].ScaleWidth = true;

            subshapes_row0_1(row0_1);

            subshapes_tr0_0_tl.Rows.Add(new TableRow());

            lbl_layer_subshape_ho_exp = new Label();
            lbl_layer_subshape_ho_exp.Text = "Hor. Offset";
            subshapes_tr0_0_tl.Rows[1].Cells.Add(new TableCell() { Control = lbl_layer_subshape_ho_exp });

            TableLayout row1_1 = new TableLayout();
            Panel pr1 = new Panel() { Content = row1_1 };
            subshapes_tr0_0_tl.Rows[1].Cells.Add(new TableCell() { Control = pr1 });
            subshapes_tr0_0_tl.Rows[1].Cells[1].ScaleWidth = true;

            subshapes_row1_1(row1_1);

            subshapes_tr0_0_tl.Rows.Add(new TableRow());

            lbl_layerTipLocations_exp = new Label();
            lbl_layerTipLocations_exp.Text = "Tip Locs";
            subshapes_tr0_0_tl.Rows[2].Cells.Add(new TableCell() { Control = lbl_layerTipLocations_exp });

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

            num_layer_subshape_hl_exp = new NumericStepper();
            num_layer_subshape_hl_exp.Increment = 0.1;
            num_layer_subshape_hl_exp.DecimalPlaces = 2;
            num_layer_subshape_hl_exp.MinValue = 0;
            setSize(num_layer_subshape_hl_exp, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape_hl_exp) });

            num_layer_subshape2_hl_exp = new NumericStepper();
            num_layer_subshape2_hl_exp.Increment = 0.1;
            num_layer_subshape2_hl_exp.DecimalPlaces = 2;
            num_layer_subshape2_hl_exp.MinValue = 0;
            setSize(num_layer_subshape2_hl_exp, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape2_hl_exp) });

            num_layer_subshape3_hl_exp = new NumericStepper();
            num_layer_subshape3_hl_exp.Increment = 0.1;
            num_layer_subshape3_hl_exp.DecimalPlaces = 2;
            num_layer_subshape3_hl_exp.MinValue = 0;
            setSize(num_layer_subshape3_hl_exp, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape3_hl_exp) });

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = new Panel() });
            row0_1.Rows[0].Cells[row0_1.Rows[0].Cells.Count - 1].ScaleWidth = true;

            lbl_layer_subshape_vl_exp = new Label();
            lbl_layer_subshape_vl_exp.Text = "Ver. Length";

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_subshape_vl_exp });

            num_layer_subshape_vl_exp = new NumericStepper();
            num_layer_subshape_vl_exp.Increment = 0.1;
            num_layer_subshape_vl_exp.DecimalPlaces = 2;
            num_layer_subshape_vl_exp.MinValue = 0;
            setSize(num_layer_subshape_vl_exp, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape_vl_exp) });

            num_layer_subshape2_vl_exp = new NumericStepper();
            num_layer_subshape2_vl_exp.Increment = 0.1;
            num_layer_subshape2_vl_exp.DecimalPlaces = 2;
            num_layer_subshape2_vl_exp.MinValue = 0;
            setSize(num_layer_subshape2_vl_exp, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape2_vl_exp) });

            num_layer_subshape3_vl_exp = new NumericStepper();
            num_layer_subshape3_vl_exp.Increment = 0.1;
            num_layer_subshape3_vl_exp.DecimalPlaces = 2;
            num_layer_subshape3_vl_exp.MinValue = 0;
            setSize(num_layer_subshape3_vl_exp, numWidth, num_Height);

            row0_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape3_vl_exp) });
        }

        void subshapes_row1_1(TableLayout row1_1)
        {
            int numWidth = 55;

            row1_1.Rows.Add(new TableRow());

            num_layer_subshape_ho_exp = new NumericStepper();
            num_layer_subshape_ho_exp.Increment = 0.1;
            num_layer_subshape_ho_exp.DecimalPlaces = 2;
            setSize(num_layer_subshape_ho_exp, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape_ho_exp) });

            num_layer_subshape2_ho_exp = new NumericStepper();
            num_layer_subshape2_ho_exp.Increment = 0.1;
            num_layer_subshape2_ho_exp.DecimalPlaces = 2;
            setSize(num_layer_subshape2_ho_exp, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape2_ho_exp) });

            num_layer_subshape3_ho_exp = new NumericStepper();
            num_layer_subshape3_ho_exp.Increment = 0.1;
            num_layer_subshape3_ho_exp.DecimalPlaces = 2;
            setSize(num_layer_subshape3_ho_exp, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape3_ho_exp) });

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = new Panel() });
            row1_1.Rows[0].Cells[row1_1.Rows[0].Cells.Count - 1].ScaleWidth = true;

            lbl_layer_subshape_vo_exp = new Label();
            lbl_layer_subshape_vo_exp.Text = "Ver. Offset";

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_subshape_vo_exp });

            num_layer_subshape_vo_exp = new NumericStepper();
            num_layer_subshape_vo_exp.Increment = 0.1;
            num_layer_subshape_vo_exp.DecimalPlaces = 2;
            setSize(num_layer_subshape_vo_exp, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape_vo_exp) });

            num_layer_subshape2_vo_exp = new NumericStepper();
            num_layer_subshape2_vo_exp.Increment = 0.1;
            num_layer_subshape2_vo_exp.DecimalPlaces = 2;
            setSize(num_layer_subshape2_vo_exp, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape2_vo_exp) });

            num_layer_subshape3_vo_exp = new NumericStepper();
            num_layer_subshape3_vo_exp.Increment = 0.1;
            num_layer_subshape3_vo_exp.DecimalPlaces = 2;
            setSize(num_layer_subshape3_vo_exp, numWidth, num_Height);

            row1_1.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_subshape3_vo_exp) });
        }

        void subshapes_row2_1(TableLayout row2_1)
        {
            row2_1.Rows.Add(new TableRow());

            comboBox_layerTipLocations_exp = new DropDown();

            row2_1.Rows[0].Cells.Add(new TableCell() { Control = comboBox_layerTipLocations_exp });

            comboBox_layerTipLocations_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

            comboBox_layerTipLocations2_exp = new DropDown();

            row2_1.Rows[0].Cells.Add(new TableCell() { Control = comboBox_layerTipLocations2_exp });

            comboBox_layerTipLocations2_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);

            comboBox_layerTipLocations3_exp = new DropDown();

            row2_1.Rows[0].Cells.Add(new TableCell() { Control = comboBox_layerTipLocations3_exp });

            comboBox_layerTipLocations3_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.tipLocs);
        }

        void subshapes_row2(TableLayout groupBox_layerSubShapes_table)
        {
            // Outer table, row 2
            TableRow subshapes_tr1 = new TableRow();
            groupBox_layerSubShapes_table.Rows.Add(subshapes_tr1);
            subshapes_tr1.Cells.Add(new TableCell());

            // Table layout within row 2
            TableLayout row1_tl = new TableLayout();
            subshapes_tr1.Cells[0].Control = row1_tl;
            row1_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell sunshapes_tr1_0 = new TableCell();
            row1_tl.Rows[0].Cells.Add(sunshapes_tr1_0);

            TableLayout subshapes_tr1_0_tl = new TableLayout();
            sunshapes_tr1_0.Control = subshapes_tr1_0_tl;

            subshapes_tr1_0_tl.Rows.Add(new TableRow());
        }
    }
}
