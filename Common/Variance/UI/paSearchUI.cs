using System;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private Label[] lbl_paSearchLayers, lbl_paSearchPAs;
    private CheckBox[,] cb_searchPA;
    private NumericStepper[,] num_searchPA_UpperLimit;
    private TextBox[,] text_searchPA_Result;

    private GroupBox[] groupBox_passCaseValues;
    private CheckBox[] cb_passCaseValues;
    private NumericStepper[] num_passCaseValues;
    private RadioButton[,] rb_passValueRadioButtons;

    private Label lbl_passCase, lbl_paSearch_readOut;
    private NumericStepper num_passCase;
    private TextBox text_paSearch;

    private TableLayout resultFilter_table;

    private bool paSearchUIFrozen;

    private void paSearchUI_setup()
    {
        paSearchUIFrozen = true;

        lbl_paSearchLayers = new Label[CentralProperties.maxLayersForMC];
        lbl_paSearchPAs = new Label[PASearch.paNames.Length];
        cb_searchPA = new CheckBox[lbl_paSearchLayers.Length, lbl_paSearchPAs.Length];
        num_searchPA_UpperLimit = new NumericStepper[lbl_paSearchLayers.Length, lbl_paSearchPAs.Length];
        text_searchPA_Result = new TextBox[lbl_paSearchLayers.Length, lbl_paSearchPAs.Length];

        tabPage_2D_PASearch_table.Rows.Add(new TableRow());

        Panel row0 = new();
        tabPage_2D_PASearch_table.Rows[^1].Cells.Add(new TableCell { Control = row0 });
        paSearchUI_setup_row0(ref row0);

        tabPage_2D_PASearch_table.Rows.Add(new TableRow());

        Panel row1 = new();
        tabPage_2D_PASearch_table.Rows[^1].Cells.Add(new TableCell { Control = row1 });
        paSearchUI_setup_row1(ref row1);

        tabPage_2D_PASearch_table.Rows.Add(new TableRow());

        Panel row2 = new();
        tabPage_2D_PASearch_table.Rows[^1].Cells.Add(new TableCell { Control = row2 });
        paSearchUI_setup_row2(ref row2);

        tabPage_2D_PASearch_table.Rows.Add(new TableRow()); // padding.

        updatePASearchUI();

        paSearchUIFrozen = false;
    }

    private void paSearchUI_setup_row0(ref Panel p)
    {
        TableLayout row0_tl = new();
        p.Content = row0_tl;
        row0_tl.Rows.Add(new TableRow());

        lbl_passCase = new Label {Text = "Pass Cases to Find", ToolTip = "Minimum number of pass cases to find."};
        row0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_passCase });

        num_passCase = new NumericStepper {Increment = 1, MinValue = 1, Value = 25000};
        num_passCase.LostFocus += paSearchUI_Handler;
        num_passCase.ToolTip = "Minimum number of pass cases to find.";
        setSize(num_passCase, 80);
        row0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_passCase) });

        lbl_paSearch_readOut = new Label {Text = "Last result:"};
        row0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_paSearch_readOut });

        text_paSearch = new TextBox {ReadOnly = true, Width = resultFieldWidth};
        row0_tl.Rows[^1].Cells.Add(new TableCell { Control = text_paSearch });

        row0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
    }

    private void paSearchUI_setup_row1(ref Panel p)
    {
        resultFilter_table = new TableLayout();
        p.Content = resultFilter_table;
        resultFilter_table.Rows.Add(new TableRow());

        // Chords provide 4 entries, so we need 4 potential limiting values.
        groupBox_passCaseValues = new GroupBox[4];
        cb_passCaseValues = new CheckBox[4];
        num_passCaseValues = new NumericStepper[4];
        rb_passValueRadioButtons = new RadioButton[4, 2];
        Panel[] passCaseValues_pnl = new Panel[4];

        for (int passCaseVal = 0; passCaseVal < cb_passCaseValues.Length; passCaseVal++)
        {
            var i = passCaseVal; // for event handler binding.

            groupBox_passCaseValues[passCaseVal] = new GroupBox {Text = "Filter result " + passCaseVal};

            passCaseValues_pnl[passCaseVal] = new Panel {Content = groupBox_passCaseValues[passCaseVal]};

            TableLayout tl = new();
            groupBox_passCaseValues[passCaseVal].Content = tl;

            tl.Rows.Add(new TableRow());

            cb_passCaseValues[passCaseVal] = new CheckBox {Text = ""};
            cb_passCaseValues[i].CheckedChanged += paSearchUI_Handler;
            tl.Rows[^1].Cells.Add(new TableCell { Control = cb_passCaseValues[passCaseVal] });

            rb_passValueRadioButtons[passCaseVal, 0] = new RadioButton
            {
                Text = "Min", Checked = true, Enabled = false
            };
            rb_passValueRadioButtons[i, 0].CheckedChanged += paSearchUI_Handler;
            tl.Rows[^1].Cells.Add(new TableCell { Control = rb_passValueRadioButtons[i, 0] });

            rb_passValueRadioButtons[passCaseVal, 1] = new RadioButton(rb_passValueRadioButtons[passCaseVal, 0])
            {
                Text = "Max", Enabled = false
            };
            rb_passValueRadioButtons[i, 1].CheckedChanged += paSearchUI_Handler;
            tl.Rows[^1].Cells.Add(new TableCell { Control = rb_passValueRadioButtons[i, 1] });

            num_passCaseValues[passCaseVal] = new NumericStepper
            {
                DecimalPlaces = 2, Increment = 0.01f, Enabled = false
            };
            setSize(num_passCaseValues[passCaseVal], 55);
            num_passCaseValues[i].LostFocus += paSearchUI_Handler;
            tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_passCaseValues[passCaseVal]) });

            resultFilter_table.Rows[^1].Cells.Add(new TableCell { Control = passCaseValues_pnl[passCaseVal] });
        }
        resultFilter_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
    }

    private void paSearchUI_setup_row2(ref Panel p)
    {
        TableLayout tabPage_2D_PASearch_bigTable = new();
        p.Content = tabPage_2D_PASearch_bigTable;

        tabPage_2D_PASearch_bigTable.Rows.Add(new TableRow()); // header row

        tabPage_2D_PASearch_bigTable.Rows[0].Cells.Add(new TableCell { Control = null }); // blank, for row header

        for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
        {
            lbl_paSearchLayers[layer] = new Label
            {
                Text = (layer + 1).ToString(),
                TextColor = UIHelper.myColorToColor(commonVars.getColors().simPreviewColors[layer]),
                TextAlignment = TextAlignment.Center
            };

            tabPage_2D_PASearch_bigTable.Rows[0].Cells.Add(new TableCell { Control = lbl_paSearchLayers[layer] });
        }

        for (int pa = 0; pa < PASearch.paNames.Length; pa++)
        {
            TableRow tr = new();

            lbl_paSearchPAs[pa] = new Label {Text = PASearch.paNames[pa]};
            tr.Cells.Add(new TableCell { Control = lbl_paSearchPAs[pa] });

            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                TableLayout tmp_tl = new();
                Panel bTc_p = new() {Content = tmp_tl};

                // Upper row of tmp_tl
                tmp_tl.Rows.Add(new TableRow());
                Panel tc0p = new();
                tmp_tl.Rows[0].Cells.Add(new TableCell { Control = tc0p });

                TableLayout tc0p_tl = new();
                tc0p.Content = tc0p_tl;
                tc0p_tl.Rows.Add(new TableRow());

                TableCell tc_ur_0 = new();
                cb_searchPA[layer, pa] = new CheckBox {Text = ""};
                cb_searchPA[layer, pa].CheckedChanged += paSearchUI_Handler;
                cb_searchPA[layer, pa].ToolTip = "Layer " + (pa + 1) + ": " + PASearch.paNames[pa];
                tc_ur_0.Control = cb_searchPA[layer, pa];
                tc0p_tl.Rows[0].Cells.Add(tc_ur_0);

                TableCell tc_ur_1 = new();
                num_searchPA_UpperLimit[layer, pa] = new NumericStepper
                {
                    Increment = 0.01f,
                    DecimalPlaces = 2,
                    TextColor = UIHelper.myColorToColor(commonVars.getColors().simPreviewColors[layer]),
                    ToolTip = "Layer " + (layer + 1) + ": " + PASearch.paNames[pa]
                };
                if (!commonVars.getPASearch().paAllowsNegativeValues(pa))
                {
                    num_searchPA_UpperLimit[layer, pa].MinValue = 0;
                }

                setSize(num_searchPA_UpperLimit[layer, pa], 55);
                num_searchPA_UpperLimit[layer, pa].LostFocus += paSearchUI_Handler;
                tc_ur_1.Control = TableLayout.AutoSized(num_searchPA_UpperLimit[layer, pa]);

                tc0p_tl.Rows[0].Cells.Add(tc_ur_1);

                // Lower row of tmp_tl
                tmp_tl.Rows.Add(new TableRow());

                text_searchPA_Result[layer, pa] = new TextBox
                {
                    ReadOnly = true,
                    Width = 55 * 2,
                    TextColor = UIHelper.myColorToColor(commonVars.getColors().simPreviewColors[layer]),
                    ToolTip = "Layer " + (layer + 1) + ": " + PASearch.paNames[pa]
                };
                tmp_tl.Rows[1].Cells.Add(new TableCell { Control = text_searchPA_Result[layer, pa] });

                tr.Cells.Add(new TableCell { Control = bTc_p });
            }
            tabPage_2D_PASearch_bigTable.Rows.Add(tr);
        }
    }

    private void paSearchUI_Handler(object sender, EventArgs e)
    {
        updatePASearchUI();
    }

    private void updatePASearchUI(int layer)
    {
        bool[] paEnabledState = commonVars.getPASearch().getEnabledState(commonVars.getLayerSettings(layer));
        for (int i = 0; i < paEnabledState.Length; i++)
        {
            cb_searchPA[layer, i].Enabled = paEnabledState[i];
            if (!paEnabledState[i])
            {
                cb_searchPA[layer, i].Checked = false;
            }
            num_searchPA_UpperLimit[layer, i].Enabled = (bool)cb_searchPA[layer, i].Checked!;
            commonVars.getPASearch().setUpperLimit(layer, i, (decimal)num_searchPA_UpperLimit[layer, i].Value);

            commonVars.getPASearch().setPASearchable(layer, i, (bool)cb_searchPA[layer, i].Checked);
        }
    }

    private void updatePASearchUI()
    {
        if (paSearchUIFrozen)
        {
            return;
        }

        paSearchUIFrozen = true;

        commonVars.getPASearch().numberofPassCases = (int)num_passCase.Value;

        for (int i = 0; i < groupBox_passCaseValues.Length; i++)
        {
            num_passCaseValues[i].Enabled = (bool)cb_passCaseValues[i].Checked!;
            rb_passValueRadioButtons[i, 0].Enabled = (bool)cb_passCaseValues[i].Checked;
            rb_passValueRadioButtons[i, 1].Enabled = (bool)cb_passCaseValues[i].Checked;

            commonVars.getPASearch().filterValues[i] = num_passCaseValues[i].Value;
            commonVars.getPASearch().filterIsMaxValue[i] = rb_passValueRadioButtons[i, 1].Checked;
        }

        bool chordMode = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)geoAnalysis.Supported.calcModes.chord;
        for (int i = 0; i < groupBox_passCaseValues.Length; i++)
        {
            if (!chordMode)
            {
                groupBox_passCaseValues[i].Enabled = false;
                cb_passCaseValues[i].Checked = false;
            }
            commonVars.getPASearch().useFilter[i] = (bool)cb_passCaseValues[i].Checked!;
        }
        if (!chordMode)
        {
            groupBox_passCaseValues[0].Enabled = true;
            cb_passCaseValues[0].Checked = true;
            commonVars.getPASearch().useFilter[0] = true;
        }

        for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
        {
            updatePASearchUI(layer);
        }

        paSearchUIFrozen = false;
    }

    private void paSearchUI_showResults(SimResultPackage resultPackage)
    {
        commonVars.getPASearch().calculateMeanAndStdDev(resultPackage);
        for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
        {
            for (int pa = 0; pa < PASearch.paNames.Length; pa++)
            {
                int layer1 = layer;
                int pa1 = pa;
                Application.Instance.Invoke(() =>
                {
                    if (commonVars.getPASearch().isPASearchable(layer1, pa1))
                    {
                        text_searchPA_Result[layer1, pa1].Text = "x:" + commonVars.getPASearch().getMeanValue(layer1, pa1) + ", s:" + commonVars.getPASearch().getSDValue(layer1, pa1);
                    }
                    else
                    {
                        text_searchPA_Result[layer1, pa1].Text = "";
                    }
                });
            }
        }

        updateProgressBar(resultPackage.getListOfResults().Count);
        updateStatusLine("PA Search for " + resultPackage.getListOfResults().Count + " results in " + resultPackage.runTime.ToString("0.##") + " seconds.");
    }
}