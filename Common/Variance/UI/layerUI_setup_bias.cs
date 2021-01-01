using Eto.Forms;
using System;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Bias Etch
        GroupBox groupBox_etchandbias, groupBox_etchandbias_reg, groupBox_etchandbias_prox;
        NumericStepper num_sidebias, num_hTipbias, num_vTipbias, num_hTipNVar, num_vTipNVar, num_hTipPVar, num_vTipPVar,
                         num_pitchDepBias, num_pitchDepBiasIsoDistance, num_pitchDepBiasSideRays, num_proxBiasFallOffMultiplier;
        Label lbl_sidebias, lbl_hTipbias, lbl_vTipbias, lbl_hTipNVar, lbl_vTipNVar, lbl_hTipPVar, lbl_vTipPVar,
                lbl_pitchDepBias, lbl_pitchDepBiasIsoDistance, lbl_pitchDepBiasSideRays, lbl_proxBiasFallOff, lbl_proxBiasFallOffMultiplier;
        DropDown comboBox_proxBiasFallOff;

        void twoD_LayerUISetup_biasEtch(TableCell tc)
        {
            Application.Instance.Invoke(() =>
            {
                groupBox_etchandbias = new GroupBox();
                groupBox_etchandbias.Text = "Bias and Etch Parameters";
                TableLayout groupBox_layer_etchandbias_table = new TableLayout();
                groupBox_etchandbias.Content = groupBox_layer_etchandbias_table;

                TableLayout t = new TableLayout();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell() { Control = groupBox_etchandbias });
                t.Rows[0].Cells.Add(new TableCell() { Control = new Panel(), ScaleWidth = true });

                Panel p = new Panel();
                p.Content = t;
                tc.Control = p;

                bias_row1(groupBox_layer_etchandbias_table);
                bias_row2(groupBox_layer_etchandbias_table);

            });
        }

        void bias_row1(TableLayout etchandbias_table)
        {
            groupBox_etchandbias_reg = new GroupBox();
            groupBox_etchandbias_reg.Text = "Regular";
            TableRow tr = new TableRow();
            etchandbias_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell() { Control = groupBox_etchandbias_reg });
            tr.Cells.Add(new TableCell() { Control = null });

            TableLayout regBias_table = new TableLayout();
            groupBox_etchandbias_reg.Content = regBias_table;

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

            lbl_sidebias = new Label();
            lbl_sidebias.Text = "Side Bias (Edge)";
            lbl_sidebias.Width = 90;
            lbl_sidebias.ToolTip = "Bias applied to each edge that is not defined as a tip.";
            biasEtch_tr0_0_tl.Rows[biasEtch_tr0_0_tl.Rows.Count - 1].Cells.Add(new TableCell());
            biasEtch_tr0_0_tl.Rows[biasEtch_tr0_0_tl.Rows.Count - 1].Cells[0].Control = lbl_sidebias;

            num_sidebias = new NumericStepper();
            num_sidebias.Increment = 0.1;
            num_sidebias.DecimalPlaces = 2;
            num_sidebias.ToolTip = "Bias applied to each edge that is not defined as a tip.";
            setSize(num_sidebias, 55, num_Height);
            biasEtch_tr0_0_tl.Rows[biasEtch_tr0_0_tl.Rows.Count - 1].Cells.Add(new TableCell());
            biasEtch_tr0_0_tl.Rows[biasEtch_tr0_0_tl.Rows.Count - 1].Cells[1].Control = TableLayout.AutoSized(num_sidebias);

            bias_row1_2(biasEtch_tr0_0_tl);
        }

        void bias_row1_2(TableLayout outerTable)
        {
            TableLayout nestedTable = new TableLayout();
            outerTable.Rows[outerTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = nestedTable });
            outerTable.Rows[outerTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            nestedTable.Rows.Add(new TableRow());

            lbl_hTipbias = new Label();
            lbl_hTipbias.Text = "Horizontal tip bias";
            lbl_hTipbias.ToolTip = "Bias applied to each subshape edge that is a left or right tip.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_hTipbias });

            num_hTipbias = new NumericStepper();
            num_hTipbias.Increment = 0.1;
            num_hTipbias.DecimalPlaces = 2;
            num_hTipbias.ToolTip = "Bias applied to each subshape edge that is a left or right tip.";
            setSize(num_hTipbias, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_hTipbias) });

            lbl_hTipPVar = new Label();
            lbl_hTipPVar.Text = "Var +";
            lbl_hTipPVar.MouseDoubleClick += hTipPVar_RNG;
            lbl_hTipPVar.ToolTip = "Positive 3-sigma bias variation for left/right tips.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_hTipPVar });

            num_hTipPVar = new NumericStepper();
            num_hTipPVar.Increment = 0.1;
            num_hTipPVar.DecimalPlaces = 2;
            num_hTipPVar.MinValue = 0;
            num_hTipPVar.ToolTip = "Positive 3-sigma bias variation for left/right tips.";
            setSize(num_hTipPVar, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_hTipPVar) });

            lbl_hTipNVar = new Label();
            lbl_hTipNVar.Text = "-";
            lbl_hTipNVar.MouseDoubleClick += hTipNVar_RNG;
            lbl_hTipNVar.ToolTip = "Negative 3-sigma bias variation for left/right tips.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_hTipNVar });

            num_hTipNVar = new NumericStepper();
            num_hTipNVar.Increment = 0.1;
            num_hTipNVar.DecimalPlaces = 2;
            num_hTipNVar.MinValue = 0;
            num_hTipNVar.ToolTip = "Negative 3-sigma bias variation for left/right tips.";
            setSize(num_hTipNVar, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_hTipNVar) });

            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            nestedTable.Rows.Add(new TableRow());

            lbl_vTipbias = new Label();
            lbl_vTipbias.Text = "Vertical tip bias";
            lbl_vTipbias.ToolTip = "Bias applied to each subshape edge that is a top or bottom tip.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_vTipbias });

            num_vTipbias = new NumericStepper();
            num_vTipbias.Increment = 0.1;
            num_vTipbias.DecimalPlaces = 2;
            num_vTipbias.ToolTip = "Bias applied to each subshape edge that is a top or bottom tip.";
            setSize(num_vTipbias, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_vTipbias) });

            lbl_vTipPVar = new Label();
            lbl_vTipPVar.Text = "Var +";
            lbl_vTipPVar.MouseDoubleClick += vTipPVar_RNG;
            lbl_vTipPVar.ToolTip = "Positive 3-sigma bias variation for top/bottom tips.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_vTipPVar });

            num_vTipPVar = new NumericStepper();
            num_vTipPVar.Increment = 0.1;
            num_vTipPVar.DecimalPlaces = 2;
            num_vTipPVar.MinValue = 0;
            num_vTipPVar.ToolTip = "Positive 3-sigma bias variation for top/bottom tips.";
            setSize(num_vTipPVar, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_vTipPVar) });

            lbl_vTipNVar = new Label();
            lbl_vTipNVar.Text = "-";
            lbl_vTipNVar.MouseDoubleClick += vTipNVar_RNG;
            lbl_vTipNVar.ToolTip = "Negative 3-sigma bias variation for top/bottom tips.";
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_vTipNVar });

            num_vTipNVar = new NumericStepper();
            num_vTipNVar.Increment = 0.1;
            num_vTipNVar.DecimalPlaces = 2;
            num_vTipNVar.MinValue = 0;
            num_vTipNVar.ToolTip = "Negative 3-sigma bias variation for top/bottom tips.";
            setSize(num_vTipNVar, 55, num_Height);
            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_vTipNVar) });

            nestedTable.Rows[nestedTable.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });
        }

        void bias_row2(TableLayout etchandbias_table)
        {
            groupBox_etchandbias_prox = new GroupBox();
            groupBox_etchandbias_prox.Text = "Proximity";
            TableRow tr = new TableRow();
            etchandbias_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell() { Control = groupBox_etchandbias_prox });
            tr.Cells.Add(new TableCell() { Control = null });

            TableLayout proxBias_table = new TableLayout();
            groupBox_etchandbias_prox.Content = proxBias_table;

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

            lbl_pitchDepBias = new Label();
            lbl_pitchDepBias.Text = "Proximity bias";
            lbl_pitchDepBias.ToolTip = "Maximum bias to be applied to edges based on visibility-based space to nearest edges.";
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(lbl_pitchDepBias) });

            num_pitchDepBias = new NumericStepper();
            num_pitchDepBias.Increment = 0.1;
            num_pitchDepBias.DecimalPlaces = 2;
            num_pitchDepBias.ToolTip = "Maximum bias to be applied to edges based on visibility-based space to nearest edges.";
            setSize(num_pitchDepBias, 55, num_Height);
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_pitchDepBias) });

            lbl_pitchDepBiasIsoDistance = new Label();
            lbl_pitchDepBiasIsoDistance.Text = "Isolated edge distance";
            lbl_pitchDepBiasIsoDistance.ToolTip = "Distance from nearest feature (in-layer) when edge is considered isolated and gets full proximity-dependent bias applied.";
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(lbl_pitchDepBiasIsoDistance) });

            num_pitchDepBiasIsoDistance = new NumericStepper();
            num_pitchDepBiasIsoDistance.Increment = 0.1;
            num_pitchDepBiasIsoDistance.DecimalPlaces = 2;
            num_pitchDepBiasIsoDistance.ToolTip = "Distance from nearest feature (in-layer) when edge is considered isolated and gets full proximity-dependent bias applied.";
            setSize(num_pitchDepBiasIsoDistance, 55, num_Height);
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_pitchDepBiasIsoDistance) });

            biasEtch_tr1_0_tl.Rows.Add(new TableRow());

            lbl_pitchDepBiasSideRays = new Label();
            lbl_pitchDepBiasSideRays.Text = "Side Rays";
            lbl_pitchDepBiasSideRays.ToolTip = "Number of additional rays to fire each side of main ray.";
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(lbl_pitchDepBiasSideRays) });

            num_pitchDepBiasSideRays = new NumericStepper();
            num_pitchDepBiasSideRays.Increment = 1;
            num_pitchDepBiasSideRays.MinValue = 0;
            num_pitchDepBiasSideRays.Value = 2;
            num_pitchDepBiasSideRays.DecimalPlaces = 0;
            num_pitchDepBiasSideRays.ToolTip = "Number of additional rays to fire each side of main ray.";
            setSize(num_pitchDepBiasSideRays, 55, num_Height);
            biasEtch_tr1_0_tl.Rows[biasEtch_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_pitchDepBiasSideRays) });

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
