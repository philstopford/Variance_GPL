using Eto.Forms;
using System;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Bias Etch
        GroupBox groupBox_layer_etchandbias_exp, groupBox_layer_etchandbias_reg, groupBox_layer_etchandbias_prox;
        NumericStepper num_layerSidebias_exp, num_layerHTipbias_exp, num_layerVTipbias_exp, num_layerhTipNVar_exp, num_layervTipNVar_exp, num_layerhTipPVar_exp, num_layervTipPVar_exp,
                         num_pitchDepBias_exp, num_pitchDepBiasIsoDistance_exp, num_pitchDepBiasSideRays_exp, num_proxBiasFallOffMultiplier;
        Label lbl_layerSidebias_exp, lbl_layerHTipbias_exp, lbl_layerVTipbias_exp, lbl_layerhTipNVar_exp, lbl_layervTipNVar_exp, lbl_layerhTipPVar_exp, lbl_layervTipPVar_exp,
                lbl_pitchDepBias_exp, lbl_pitchDepBiasIsoDistance_exp, lbl_pitchDepBiasSideRays_exp, lbl_proxBiasFallOff, lbl_proxBiasFallOffMultiplier;
        DropDown comboBox_proxBiasFallOff;

        void twoD_LayerUISetup_biasEtch_exp(TableCell tc)
        {
            Application.Instance.Invoke(() =>
            {
                groupBox_layer_etchandbias_exp = new GroupBox();
                groupBox_layer_etchandbias_exp.Text = "Bias and Etch Parameters";
                TableLayout groupBox_layer_etchandbias_table = new TableLayout();
                groupBox_layer_etchandbias_exp.Content = groupBox_layer_etchandbias_table;

                TableLayout t = new TableLayout();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell() { Control = groupBox_layer_etchandbias_exp });
                t.Rows[0].Cells.Add(new TableCell() { Control = new Panel(), ScaleWidth = true });

                Panel p = new Panel();
                p.Content = t;
                tc.Control = p;

                bias_row1(groupBox_layer_etchandbias_table);
                bias_row2(groupBox_layer_etchandbias_table);

            });
        }

        void bias_row1(TableLayout groupBox_layer_etchandbias_table)
        {
            groupBox_layer_etchandbias_reg = new GroupBox();
            groupBox_layer_etchandbias_reg.Text = "Regular";
            TableRow tr = new TableRow();
            groupBox_layer_etchandbias_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell() { Control = groupBox_layer_etchandbias_reg });
            tr.Cells.Add(new TableCell() { Control = null });

            TableLayout regBias_table = new TableLayout();
            groupBox_layer_etchandbias_reg.Content = regBias_table;

            // Outer table, row 1
            TableRow biasEtch_tr0 = new TableRow();
            biasEtch_tr0.Cells.Add(new TableCell());
            regBias_table.Rows.Add(biasEtch_tr0);

            // Table layout within row 1
            TableLayout row0_tl = new TableLayout();
            biasEtch_tr0.Cells[0].Control = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell biasEtch_tr0_0 = new TableCell();
            row0_tl.Rows[0].Cells.Add(biasEtch_tr0_0);

            TableLayout biasEtch_tr0_0_tl = new TableLayout();
            biasEtch_tr0_0.Control = biasEtch_tr0_0_tl;

            biasEtch_tr0_0_tl.Rows.Add(new TableRow());

            lbl_layerSidebias_exp = new Label();
            lbl_layerSidebias_exp.Text = "Side Bias (Edge)";
            lbl_layerSidebias_exp.Width = 90;
            lbl_layerSidebias_exp.ToolTip = "Bias applied to each edge that is not defined as a tip.";
            biasEtch_tr0_0_tl.Rows[biasEtch_tr0_0_tl.Rows.Count - 1].Cells.Add(new TableCell());
            biasEtch_tr0_0_tl.Rows[biasEtch_tr0_0_tl.Rows.Count - 1].Cells[0].Control = lbl_layerSidebias_exp;

            num_layerSidebias_exp = new NumericStepper();
            num_layerSidebias_exp.Increment = 0.1;
            num_layerSidebias_exp.DecimalPlaces = 2;
            num_layerSidebias_exp.ToolTip = "Bias applied to each edge that is not defined as a tip.";
            setSize(num_layerSidebias_exp, 55, num_Height);
            biasEtch_tr0_0_tl.Rows[biasEtch_tr0_0_tl.Rows.Count - 1].Cells.Add(new TableCell());
            biasEtch_tr0_0_tl.Rows[biasEtch_tr0_0_tl.Rows.Count - 1].Cells[1].Control = TableLayout.AutoSized(num_layerSidebias_exp);

            bias_row1_2(biasEtch_tr0_0_tl);
        }

        void bias_row1_2(TableLayout outerTable)
        {
            TableLayout nestedTable = new TableLayout();
            outerTable.Rows[outerTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = nestedTable });
            outerTable.Rows[outerTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            nestedTable.Rows.Add(new TableRow());

            lbl_layerHTipbias_exp = new Label();
            lbl_layerHTipbias_exp.Text = "Horizontal tip bias";
            lbl_layerHTipbias_exp.ToolTip = "Bias applied to each subshape edge that is a left or right tip.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layerHTipbias_exp });

            num_layerHTipbias_exp = new NumericStepper();
            num_layerHTipbias_exp.Increment = 0.1;
            num_layerHTipbias_exp.DecimalPlaces = 2;
            num_layerHTipbias_exp.ToolTip = "Bias applied to each subshape edge that is a left or right tip.";
            setSize(num_layerHTipbias_exp, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layerHTipbias_exp) });

            lbl_layerhTipPVar_exp = new Label();
            lbl_layerhTipPVar_exp.Text = "Var +";
            lbl_layerhTipPVar_exp.MouseDoubleClick += hTipPVar_RNG;
            lbl_layerhTipPVar_exp.ToolTip = "Positive 3-sigma bias variation for left/right tips.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layerhTipPVar_exp });

            num_layerhTipPVar_exp = new NumericStepper();
            num_layerhTipPVar_exp.Increment = 0.1;
            num_layerhTipPVar_exp.DecimalPlaces = 2;
            num_layerhTipPVar_exp.MinValue = 0;
            num_layerhTipPVar_exp.ToolTip = "Positive 3-sigma bias variation for left/right tips.";
            setSize(num_layerhTipPVar_exp, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layerhTipPVar_exp) });

            lbl_layerhTipNVar_exp = new Label();
            lbl_layerhTipNVar_exp.Text = "-";
            lbl_layerhTipNVar_exp.MouseDoubleClick += hTipNVar_RNG;
            lbl_layerhTipNVar_exp.ToolTip = "Negative 3-sigma bias variation for left/right tips.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layerhTipNVar_exp });

            num_layerhTipNVar_exp = new NumericStepper();
            num_layerhTipNVar_exp.Increment = 0.1;
            num_layerhTipNVar_exp.DecimalPlaces = 2;
            num_layerhTipNVar_exp.MinValue = 0;
            num_layerhTipNVar_exp.ToolTip = "Negative 3-sigma bias variation for left/right tips.";
            setSize(num_layerhTipNVar_exp, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layerhTipNVar_exp) });

            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            nestedTable.Rows.Add(new TableRow());

            lbl_layerVTipbias_exp = new Label();
            lbl_layerVTipbias_exp.Text = "Vertical tip bias";
            lbl_layerVTipbias_exp.ToolTip = "Bias applied to each subshape edge that is a top or bottom tip.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layerVTipbias_exp });

            num_layerVTipbias_exp = new NumericStepper();
            num_layerVTipbias_exp.Increment = 0.1;
            num_layerVTipbias_exp.DecimalPlaces = 2;
            num_layerVTipbias_exp.ToolTip = "Bias applied to each subshape edge that is a top or bottom tip.";
            setSize(num_layerVTipbias_exp, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layerVTipbias_exp) });

            lbl_layervTipPVar_exp = new Label();
            lbl_layervTipPVar_exp.Text = "Var +";
            lbl_layervTipPVar_exp.MouseDoubleClick += vTipPVar_RNG;
            lbl_layervTipPVar_exp.ToolTip = "Positive 3-sigma bias variation for top/bottom tips.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layervTipPVar_exp });

            num_layervTipPVar_exp = new NumericStepper();
            num_layervTipPVar_exp.Increment = 0.1;
            num_layervTipPVar_exp.DecimalPlaces = 2;
            num_layervTipPVar_exp.MinValue = 0;
            num_layervTipPVar_exp.ToolTip = "Positive 3-sigma bias variation for top/bottom tips.";
            setSize(num_layervTipPVar_exp, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layervTipPVar_exp) });

            lbl_layervTipNVar_exp = new Label();
            lbl_layervTipNVar_exp.Text = "-";
            lbl_layervTipNVar_exp.MouseDoubleClick += vTipNVar_RNG;
            lbl_layervTipNVar_exp.ToolTip = "Negative 3-sigma bias variation for top/bottom tips.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layervTipNVar_exp });

            num_layervTipNVar_exp = new NumericStepper();
            num_layervTipNVar_exp.Increment = 0.1;
            num_layervTipNVar_exp.DecimalPlaces = 2;
            num_layervTipNVar_exp.MinValue = 0;
            num_layervTipNVar_exp.ToolTip = "Negative 3-sigma bias variation for top/bottom tips.";
            setSize(num_layervTipNVar_exp, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layervTipNVar_exp) });

            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });
        }

        void bias_row2(TableLayout groupBox_layer_etchandbias_table)
        {
            groupBox_layer_etchandbias_prox = new GroupBox();
            groupBox_layer_etchandbias_prox.Text = "Proximity";
            TableRow tr = new TableRow();
            groupBox_layer_etchandbias_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell() { Control = groupBox_layer_etchandbias_prox });
            tr.Cells.Add(new TableCell() { Control = null });

            TableLayout proxBias_table = new TableLayout();
            groupBox_layer_etchandbias_prox.Content = proxBias_table;

            // Outer table, row 2
            TableRow biasEtch_tr1 = new TableRow();
            proxBias_table.Rows.Add(biasEtch_tr1);
            biasEtch_tr1.Cells.Add(new TableCell());

            // Table layout within row 2
            TableLayout row1_tl = new TableLayout();
            biasEtch_tr1.Cells[0].Control = row1_tl;
            row1_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell biasEtch_tr1_0 = new TableCell();
            row1_tl.Rows[0].Cells.Add(biasEtch_tr1_0);

            TableLayout biasEtch_tr1_0_tl = new TableLayout();
            biasEtch_tr1_0.Control = biasEtch_tr1_0_tl;

            biasEtch_tr1_0_tl.Rows.Add(new TableRow());

            lbl_pitchDepBias_exp = new Label();
            lbl_pitchDepBias_exp.Text = "Proximity bias";
            lbl_pitchDepBias_exp.ToolTip = "Maximum bias to be applied to edges based on visibility-based space to nearest edges.";
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(lbl_pitchDepBias_exp) });

            num_pitchDepBias_exp = new NumericStepper();
            num_pitchDepBias_exp.Increment = 0.1;
            num_pitchDepBias_exp.DecimalPlaces = 2;
            num_pitchDepBias_exp.ToolTip = "Maximum bias to be applied to edges based on visibility-based space to nearest edges.";
            setSize(num_pitchDepBias_exp, 55, num_Height);
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_pitchDepBias_exp) });

            lbl_pitchDepBiasIsoDistance_exp = new Label();
            lbl_pitchDepBiasIsoDistance_exp.Text = "Isolated edge distance";
            lbl_pitchDepBiasIsoDistance_exp.ToolTip = "Distance from nearest feature (in-layer) when edge is considered isolated and gets full proximity-dependent bias applied.";
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(lbl_pitchDepBiasIsoDistance_exp) });

            num_pitchDepBiasIsoDistance_exp = new NumericStepper();
            num_pitchDepBiasIsoDistance_exp.Increment = 0.1;
            num_pitchDepBiasIsoDistance_exp.DecimalPlaces = 2;
            num_pitchDepBiasIsoDistance_exp.ToolTip = "Distance from nearest feature (in-layer) when edge is considered isolated and gets full proximity-dependent bias applied.";
            setSize(num_pitchDepBiasIsoDistance_exp, 55, num_Height);
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_pitchDepBiasIsoDistance_exp) });

            biasEtch_tr1_0_tl.Rows.Add(new TableRow());

            lbl_pitchDepBiasSideRays_exp = new Label();
            lbl_pitchDepBiasSideRays_exp.Text = "Side Rays";
            lbl_pitchDepBiasSideRays_exp.ToolTip = "Number of additional rays to fire each side of main ray.";
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(lbl_pitchDepBiasSideRays_exp) });

            num_pitchDepBiasSideRays_exp = new NumericStepper();
            num_pitchDepBiasSideRays_exp.Increment = 1;
            num_pitchDepBiasSideRays_exp.MinValue = 0;
            num_pitchDepBiasSideRays_exp.Value = 2;
            num_pitchDepBiasSideRays_exp.DecimalPlaces = 0;
            num_pitchDepBiasSideRays_exp.ToolTip = "Number of additional rays to fire each side of main ray.";
            setSize(num_pitchDepBiasSideRays_exp, 55, num_Height);
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_pitchDepBiasSideRays_exp) });

            lbl_proxBiasFallOff = new Label();
            lbl_proxBiasFallOff.Text = "Falloff";
            lbl_proxBiasFallOff.ToolTip = "Falloff for side rays";
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_proxBiasFallOff });

            comboBox_proxBiasFallOff = new DropDown();
            comboBox_proxBiasFallOff.ToolTip = "Falloff to apply for side rays.";
            comboBox_proxBiasFallOff.BindDataContext(c => c.DataStore, (UIStringLists m) => m.fallOffList);
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_proxBiasFallOff) });

            lbl_proxBiasFallOffMultiplier = new Label();
            lbl_proxBiasFallOffMultiplier.Text = "x";
            lbl_proxBiasFallOffMultiplier.ToolTip = "Multiplier for falloff";
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(lbl_proxBiasFallOffMultiplier) });

            num_proxBiasFallOffMultiplier = new NumericStepper();
            num_proxBiasFallOffMultiplier.Increment = 0.1;
            num_proxBiasFallOffMultiplier.MinValue = 0;
            num_proxBiasFallOffMultiplier.Value = Convert.ToDouble(EntropyLayerSettings.getDefaultDecimal(EntropyLayerSettings.properties_decimal.proxSideRaysMultiplier));
            num_proxBiasFallOffMultiplier.DecimalPlaces = 2;
            num_proxBiasFallOffMultiplier.ToolTip = "Multipler for falloff";
            setSize(num_proxBiasFallOffMultiplier, 55, num_Height);
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_proxBiasFallOffMultiplier) });
        }
    }
}
