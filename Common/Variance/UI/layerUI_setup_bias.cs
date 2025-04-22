using System;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    // 2D Layer Bias Etch
    private Expander expander_etchandbias, expander_etchandbias_reg, expander_etchandbias_prox, expander_etchandbias_unidirectional;
    private GroupBox groupBox_etchandbias , groupBox_etchandbias_reg, groupBox_etchandbias_prox, groupBox_etchandbias_unidirectional;

    private RadioButton rB_unidirectional_x, rB_unidirectional_y;
    
    private NumericStepper num_sidebias, num_hTipbias, num_vTipbias, num_hTipNVar, num_vTipNVar, num_hTipPVar, num_vTipPVar,
        num_pitchDepBias, num_pitchDepBiasIsoDistance, num_pitchDepBiasSideRays, num_proxBiasFallOffMultiplier, num_unidirectionalbias;

    private Label lbl_sidebias, lbl_hTipbias, lbl_vTipbias, lbl_hTipNVar, lbl_vTipNVar, lbl_hTipPVar, lbl_vTipPVar,
        lbl_pitchDepBias, lbl_pitchDepBiasIsoDistance, lbl_pitchDepBiasSideRays, lbl_proxBiasFallOff, lbl_proxBiasFallOffMultiplier, lbl_unidirectionalbias;

    private DropDown comboBox_proxBiasFallOff;

    private CheckBox cb_removeArtifacts, cb_unidirectionalbias_after_rotation;
    private NumericStepper num_removeArtifactsEps;

    private void twoD_LayerUISetup_biasEtch_expanders(bool toState)
    {
        expander_etchandbias.Expanded = toState;
        expander_etchandbias_reg.Expanded = toState;
        expander_etchandbias_prox.Expanded = toState;
    }

    private void twoD_LayerUISetup_biasEtch(TableCell tc)
    {
        Application.Instance.Invoke(() =>
        {
            TableLayout groupBox_layer_etchandbias_table = new();
            groupBox_etchandbias = new GroupBox()
            {
                Content = groupBox_layer_etchandbias_table
            };
            expander_etchandbias = new()
            {
                Header = "Bias and Etch Parameters",
                Content = groupBox_etchandbias,
                Expanded = true
            };

            TableLayout t = new();
            t.Rows.Add(new TableRow());
            t.Rows[0].Cells.Add(new TableCell { Control = expander_etchandbias });
            t.Rows[0].Cells.Add(new TableCell { Control = new Panel(), ScaleWidth = true });

            Panel p = new() {Content = t};
            tc.Control = p;

            bias_row1(groupBox_layer_etchandbias_table);
            bias_row2(groupBox_layer_etchandbias_table);
            bias_row3(groupBox_layer_etchandbias_table);

        });
    }

    private void bias_row1(TableLayout etchandbias_table)
    {
        TableLayout uniBias_table = new();
        groupBox_etchandbias_unidirectional = new()
        {
            Content = uniBias_table
        };

        expander_etchandbias_unidirectional = new()
        {
            Header = "Unidirectional",
            Content = groupBox_etchandbias_unidirectional,
            Expanded = true
        };

        TableRow tr = new();
        etchandbias_table.Rows.Add(tr);
        tr.Cells.Add(new TableCell { Control = expander_etchandbias_unidirectional });
        tr.Cells.Add(new TableCell { Control = null });
        
        // Outer table, row 1
        TableRow biasEtch_tr0 = new();
        biasEtch_tr0.Cells.Add(new TableCell());
        uniBias_table.Rows.Add(biasEtch_tr0);

        // Table layout within row 1
        TableLayout row0_tl = new();
        biasEtch_tr0.Cells[0].Control = row0_tl;
        row0_tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell biasEtch_tr0_0 = new();
        row0_tl.Rows[0].Cells.Add(biasEtch_tr0_0);

        TableLayout biasEtch_tr0_0_tl = new();
        biasEtch_tr0_0.Control = biasEtch_tr0_0_tl;

        biasEtch_tr0_0_tl.Rows.Add(new TableRow());
        
        lbl_unidirectionalbias = new Label
        {
            Text = "Unidirectional Bias",
            Width = 90,
            ToolTip = "Unidirectional bias is applied after all other shape biases, rounding, etc. using the selected global axis.\n" +
                      "An option is available to apply this biasing before or after rotation."
        };
        biasEtch_tr0_0_tl.Rows[^1].Cells.Add(new TableCell());
        biasEtch_tr0_0_tl.Rows[^1].Cells[^1].Control = lbl_unidirectionalbias;

        num_unidirectionalbias = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            ToolTip = "Unidirectional bias is applied after all other shape biases, rounding, etc. using the selected global axis.\n" +
                      "An option is available to apply this biasing before or after rotation."
        };
        setSize(num_unidirectionalbias, 55);
        biasEtch_tr0_0_tl.Rows[^1].Cells.Add(new TableCell());
        biasEtch_tr0_0_tl.Rows[^1].Cells[^1].Control = TableLayout.AutoSized(num_unidirectionalbias);

        rB_unidirectional_x = new RadioButton()
        {
            Text = "X",
            ToolTip = "Bias along the global X axis. If 'After Rotation' is not checked, this will act like the shape's X axis",
            Checked = true
        };
        
        biasEtch_tr0_0_tl.Rows[^1].Cells.Add(new TableCell());
        biasEtch_tr0_0_tl.Rows[^1].Cells[^1].Control = TableLayout.AutoSized(rB_unidirectional_x);
        
        rB_unidirectional_y = new RadioButton(rB_unidirectional_x)
        {
            Text = "Y",
            ToolTip = "Bias along the global Y axis. If 'After Rotation' is not checked, this will act like the shape's Y axis"
        };
        
        biasEtch_tr0_0_tl.Rows[^1].Cells.Add(new TableCell());
        biasEtch_tr0_0_tl.Rows[^1].Cells[^1].Control = TableLayout.AutoSized(rB_unidirectional_y);

        cb_unidirectionalbias_after_rotation = new()
        {
            Text = "After Rotation",
            ToolTip = "When checked, the unidirectional bias is applied after all rotations are applied."
        };
        biasEtch_tr0_0_tl.Rows[^1].Cells.Add(new TableCell());
        biasEtch_tr0_0_tl.Rows[^1].Cells[^1].Control = TableLayout.AutoSized(cb_unidirectionalbias_after_rotation);
    }

    private void bias_row2(TableLayout etchandbias_table)
    {
        TableLayout regBias_table = new();
        groupBox_etchandbias_reg = new ()
        {
            Content = regBias_table
        };
        expander_etchandbias_reg = new ()
        {
            Header = "Regular",
            Content = groupBox_etchandbias_reg,
            Expanded = true
        };
        TableRow tr = new();
        etchandbias_table.Rows.Add(tr);
        tr.Cells.Add(new TableCell { Control = expander_etchandbias_reg });
        tr.Cells.Add(new TableCell { Control = null });
            
        // Outer table, row 1
        TableRow biasEtch_tr0 = new();
        biasEtch_tr0.Cells.Add(new TableCell());
        regBias_table.Rows.Add(biasEtch_tr0);

        // Table layout within row 1
        TableLayout row0_tl = new();
        biasEtch_tr0.Cells[0].Control = row0_tl;
        row0_tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell biasEtch_tr0_0 = new();
        row0_tl.Rows[0].Cells.Add(biasEtch_tr0_0);

        TableLayout biasEtch_tr0_0_tl = new();
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

        bias_row2_2(biasEtch_tr0_0_tl);
    }

    private void bias_row2_2(TableLayout outerTable)
    {
        TableLayout nestedTable = new();
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

        lbl_hTipPVar = new Label {Text = "Variation: +", ToolTip = "Positive 3-sigma bias variation for left/right tips."};
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

        lbl_vTipPVar = new Label {Text = "Variation: +", ToolTip = "Positive 3-sigma bias variation for top/bottom tips."};
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

    private void bias_row3(TableLayout etchandbias_table)
    {
        TableLayout proxBias_table = new();
        groupBox_etchandbias_prox = new () {Content = proxBias_table};
        expander_etchandbias_prox = new()
        {
            Header = "Proximity",
            Content = groupBox_etchandbias_prox,
            Expanded  = commonVars.getExpandedUI()
        };
        TableRow tr = new();
        etchandbias_table.Rows.Add(tr);
        tr.Cells.Add(new TableCell { Control = expander_etchandbias_prox });
        tr.Cells.Add(new TableCell { Control = null });

        // Outer table, row 2
        TableRow biasEtch_tr1 = new();
        proxBias_table.Rows.Add(biasEtch_tr1);
        biasEtch_tr1.Cells.Add(new TableCell());

        // Table layout within row 2
        TableLayout row1_tl = new();
        biasEtch_tr1.Cells[0].Control = row1_tl;
        row1_tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell biasEtch_tr1_0 = new();
        row1_tl.Rows[0].Cells.Add(biasEtch_tr1_0);

        TableLayout biasEtch_tr1_0_tl = new();
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
        
        biasEtch_tr1_0_tl.Rows.Add(new TableRow());
        cb_removeArtifacts = new CheckBox
        {
            Text = "Remove artifacts", ToolTip = "Attempt to remove artifacts from surface after biasing."
        };
        biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(cb_removeArtifacts) });

        num_removeArtifactsEps = new NumericStepper
        {
            Increment = 1,
            MinValue = 0,
            Value = 100,
            DecimalPlaces = 0,
            ToolTip = "Epsilon value for artifact removal."
        };
        setSize(num_removeArtifactsEps, 55);
        biasEtch_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_removeArtifactsEps) });

    }
}