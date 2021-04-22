using System;
using Eto.Forms;

namespace Variance
{
    public partial class MainForm
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
                TableLayout groupBox_layer_etchandbias_table = new TableLayout();
                groupBox_etchandbias = new GroupBox
                {
                    Text = "Bias and Etch Parameters", Content = groupBox_layer_etchandbias_table
                };

                TableLayout t = new TableLayout();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell { Control = groupBox_etchandbias });
                t.Rows[0].Cells.Add(new TableCell { Control = new Panel(), ScaleWidth = true });

                Panel p = new Panel {Content = t};
                tc.Control = p;

                bias_row1(groupBox_layer_etchandbias_table);
                bias_row2(groupBox_layer_etchandbias_table);

            });
        }

        void bias_row1(TableLayout etchandbias_table)
        {
            groupBox_etchandbias_reg = new GroupBox {Text = "Regular"};
            TableRow tr = new TableRow();
            etchandbias_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell { Control = groupBox_etchandbias_reg });
            tr.Cells.Add(new TableCell { Control = null });

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

            lbl_sidebias = new Label
            {
                Text = "Side Bias (Edge)",
                Width = 90,
                ToolTip = "Bias applied to each edge that is not defined as a tip."
            };
            biasEtch_tr0_0_tl.Rows[^1].Cells.Add(new TableCell());
            biasEtch_tr0_0_tl.Rows[^1].Cells[0].Control = lbl_sidebias;

            num_sidebias = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                ToolTip = "Bias applied to each edge that is not defined as a tip."
            };
            setSize(num_sidebias, 55);
            biasEtch_tr0_0_tl.Rows[^1].Cells.Add(new TableCell());
            biasEtch_tr0_0_tl.Rows[^1].Cells[1].Control = TableLayout.AutoSized(num_sidebias);

            bias_row1_2(biasEtch_tr0_0_tl);
        }

        void bias_row1_2(TableLayout outerTable)
        {
            TableLayout nestedTable = new TableLayout();
            outerTable.Rows[^1].Cells.Add(new TableCell { Control = nestedTable });
            outerTable.Rows[^1].Cells.Add(new TableCell { Control = null });

            nestedTable.Rows.Add(new TableRow());

            lbl_hTipbias = new Label
            {
                Text = "Horizontal tip bias",
                ToolTip = "Bias applied to each subshape edge that is a left or right tip."
            };
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = lbl_hTipbias });

            num_hTipbias = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                ToolTip = "Bias applied to each subshape edge that is a left or right tip."
            };
            setSize(num_hTipbias, 55);
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_hTipbias) });

            lbl_hTipPVar = new Label {Text = "Var +", ToolTip = "Positive 3-sigma bias variation for left/right tips."};
            lbl_hTipPVar.MouseDoubleClick += hTipPVar_RNG;
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = lbl_hTipPVar });

            num_hTipPVar = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                MinValue = 0,
                ToolTip = "Positive 3-sigma bias variation for left/right tips."
            };
            setSize(num_hTipPVar, 55);
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_hTipPVar) });

            lbl_hTipNVar = new Label {Text = "-", ToolTip = "Negative 3-sigma bias variation for left/right tips."};
            lbl_hTipNVar.MouseDoubleClick += hTipNVar_RNG;
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = lbl_hTipNVar });

            num_hTipNVar = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                MinValue = 0,
                ToolTip = "Negative 3-sigma bias variation for left/right tips."
            };
            setSize(num_hTipNVar, 55);
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_hTipNVar) });

            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = null });

            nestedTable.Rows.Add(new TableRow());

            lbl_vTipbias = new Label
            {
                Text = "Vertical tip bias",
                ToolTip = "Bias applied to each subshape edge that is a top or bottom tip."
            };
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = lbl_vTipbias });

            num_vTipbias = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                ToolTip = "Bias applied to each subshape edge that is a top or bottom tip."
            };
            setSize(num_vTipbias, 55);
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_vTipbias) });

            lbl_vTipPVar = new Label {Text = "Var +", ToolTip = "Positive 3-sigma bias variation for top/bottom tips."};
            lbl_vTipPVar.MouseDoubleClick += vTipPVar_RNG;
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = lbl_vTipPVar });

            num_vTipPVar = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                MinValue = 0,
                ToolTip = "Positive 3-sigma bias variation for top/bottom tips."
            };
            setSize(num_vTipPVar, 55);
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_vTipPVar) });

            lbl_vTipNVar = new Label {Text = "-", ToolTip = "Negative 3-sigma bias variation for top/bottom tips."};
            lbl_vTipNVar.MouseDoubleClick += vTipNVar_RNG;
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = lbl_vTipNVar });

            num_vTipNVar = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                MinValue = 0,
                ToolTip = "Negative 3-sigma bias variation for top/bottom tips."
            };
            setSize(num_vTipNVar, 55);
            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_vTipNVar) });

            nestedTable.Rows[^1].Cells.Add(new TableCell { Control = null });
        }

        void bias_row2(TableLayout etchandbias_table)
        {
            groupBox_etchandbias_prox = new GroupBox {Text = "Proximity"};
            TableRow tr = new TableRow();
            etchandbias_table.Rows.Add(tr);
            tr.Cells.Add(new TableCell { Control = groupBox_etchandbias_prox });
            tr.Cells.Add(new TableCell { Control = null });

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

            lbl_pitchDepBias = new Label
            {
                Text = "Proximity bias",
                ToolTip = "Maximum bias to be applied to edges based on visibility-based space to nearest edges."
            };
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(lbl_pitchDepBias) });

            num_pitchDepBias = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                ToolTip = "Maximum bias to be applied to edges based on visibility-based space to nearest edges."
            };
            setSize(num_pitchDepBias, 55);
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_pitchDepBias) });

            lbl_pitchDepBiasIsoDistance = new Label
            {
                Text = "Isolated edge distance",
                ToolTip =
                    "Distance from nearest feature (in-layer) when edge is considered isolated and gets full proximity-dependent bias applied."
            };
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(lbl_pitchDepBiasIsoDistance) });

            num_pitchDepBiasIsoDistance = new NumericStepper
            {
                Increment = 0.1,
                DecimalPlaces = 2,
                ToolTip =
                    "Distance from nearest feature (in-layer) when edge is considered isolated and gets full proximity-dependent bias applied."
            };
            setSize(num_pitchDepBiasIsoDistance, 55);
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_pitchDepBiasIsoDistance) });

            biasEtch_tr1_0_tl.Rows.Add(new TableRow());

            lbl_pitchDepBiasSideRays = new Label
            {
                Text = "Side Rays", ToolTip = "Number of additional rays to fire each side of main ray."
            };
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(lbl_pitchDepBiasSideRays) });

            num_pitchDepBiasSideRays = new NumericStepper
            {
                Increment = 1,
                MinValue = 0,
                Value = 2,
                DecimalPlaces = 0,
                ToolTip = "Number of additional rays to fire each side of main ray."
            };
            setSize(num_pitchDepBiasSideRays, 55);
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_pitchDepBiasSideRays) });

            lbl_proxBiasFallOff = new Label {Text = "Falloff", ToolTip = "Falloff for side rays"};
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_proxBiasFallOff });

            comboBox_proxBiasFallOff = new DropDown {ToolTip = "Falloff to apply for side rays."};
            comboBox_proxBiasFallOff.BindDataContext(c => c.DataStore, (UIStringLists m) => m.fallOffList);
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_proxBiasFallOff) });

            lbl_proxBiasFallOffMultiplier = new Label {Text = "x", ToolTip = "Multiplier for falloff"};
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(lbl_proxBiasFallOffMultiplier) });

            num_proxBiasFallOffMultiplier = new NumericStepper
            {
                Increment = 0.1,
                MinValue = 0,
                Value = Convert.ToDouble(
                    EntropyLayerSettings.getDefaultDecimal(EntropyLayerSettings.properties_decimal
                        .proxSideRaysMultiplier)),
                DecimalPlaces = 2,
                ToolTip = "Multiplier for falloff"
            };
            setSize(num_proxBiasFallOffMultiplier, 55);
            biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_proxBiasFallOffMultiplier) });
        }
    }
}
