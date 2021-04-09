using System;
using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        Label[] lbl_paSearchLayers, lbl_paSearchPAs;
        CheckBox[,] cb_searchPA;
        NumericStepper[,] num_searchPA_UpperLimit;
        TextBox[,] text_searchPA_Result;

        GroupBox[] groupBox_passCaseValues;
        CheckBox[] cb_passCaseValues;
        NumericStepper[] num_passCaseValues;
        RadioButton[,] rb_passValueRadioButtons;

        Label lbl_passCase, lbl_paSearch_readOut;
        NumericStepper num_passCase;
        TextBox text_paSearch;

        TableLayout resultFilter_table;

        bool paSearchUIFrozen;

        void paSearchUI_setup()
        {
            paSearchUIFrozen = true;

            lbl_paSearchLayers = new Label[CentralProperties.maxLayersForMC];
            lbl_paSearchPAs = new Label[PASearch.paNames.Length];
            cb_searchPA = new CheckBox[lbl_paSearchLayers.Length, lbl_paSearchPAs.Length];
            num_searchPA_UpperLimit = new NumericStepper[lbl_paSearchLayers.Length, lbl_paSearchPAs.Length];
            text_searchPA_Result = new TextBox[lbl_paSearchLayers.Length, lbl_paSearchPAs.Length];

            tabPage_2D_PASearch_table.Rows.Add(new TableRow());

            Panel row0 = new Panel();
            tabPage_2D_PASearch_table.Rows[^1].Cells.Add(new TableCell { Control = row0 });
            paSearchUI_setup_row0(ref row0);

            tabPage_2D_PASearch_table.Rows.Add(new TableRow());

            Panel row1 = new Panel();
            tabPage_2D_PASearch_table.Rows[^1].Cells.Add(new TableCell { Control = row1 });
            paSearchUI_setup_row1(ref row1);

            tabPage_2D_PASearch_table.Rows.Add(new TableRow());

            Panel row2 = new Panel();
            tabPage_2D_PASearch_table.Rows[^1].Cells.Add(new TableCell { Control = row2 });
            paSearchUI_setup_row2(ref row2);

            tabPage_2D_PASearch_table.Rows.Add(new TableRow()); // padding.

            updatePASearchUI();

            paSearchUIFrozen = false;
        }

        void paSearchUI_setup_row0(ref Panel p)
        {
            TableLayout row0_tl = new TableLayout();
            p.Content = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            lbl_passCase = new Label();
            lbl_passCase.Text = "Pass Cases to Find";
            lbl_passCase.ToolTip = "Minimum number of pass cases to find.";
            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_passCase });

            num_passCase = new NumericStepper();
            num_passCase.Increment = 1;
            num_passCase.MinValue = 1;
            num_passCase.Value = 25000;
            num_passCase.LostFocus += paSearchUI_Handler;
            num_passCase.ToolTip = "Minimum number of pass cases to find.";
            setSize(num_passCase, 80);
            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_passCase) });

            lbl_paSearch_readOut = new Label();
            lbl_paSearch_readOut.Text = "Last result:";
            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_paSearch_readOut });

            text_paSearch = new TextBox();
            text_paSearch.ReadOnly = true;
            text_paSearch.Width = resultFieldWidth;
            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = text_paSearch });

            row0_tl.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
        }

        void paSearchUI_setup_row1(ref Panel p)
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

                groupBox_passCaseValues[passCaseVal] = new GroupBox();
                groupBox_passCaseValues[passCaseVal].Text = "Filter result " + passCaseVal;

                passCaseValues_pnl[passCaseVal] = new Panel();
                passCaseValues_pnl[passCaseVal].Content = groupBox_passCaseValues[passCaseVal];

                TableLayout tl = new TableLayout();
                groupBox_passCaseValues[passCaseVal].Content = tl;

                tl.Rows.Add(new TableRow());

                cb_passCaseValues[passCaseVal] = new CheckBox();
                cb_passCaseValues[passCaseVal].Text = "";
                cb_passCaseValues[i].CheckedChanged += paSearchUI_Handler;
                tl.Rows[^1].Cells.Add(new TableCell { Control = cb_passCaseValues[passCaseVal] });

                rb_passValueRadioButtons[passCaseVal, 0] = new RadioButton();
                rb_passValueRadioButtons[passCaseVal, 0].Text = "Min";
                rb_passValueRadioButtons[passCaseVal, 0].Checked = true;
                rb_passValueRadioButtons[passCaseVal, 0].Enabled = false;
                rb_passValueRadioButtons[i, 0].CheckedChanged += paSearchUI_Handler;
                tl.Rows[^1].Cells.Add(new TableCell { Control = rb_passValueRadioButtons[i, 0] });

                rb_passValueRadioButtons[passCaseVal, 1] = new RadioButton(rb_passValueRadioButtons[passCaseVal, 0]);
                rb_passValueRadioButtons[passCaseVal, 1].Text = "Max";
                rb_passValueRadioButtons[passCaseVal, 1].Enabled = false;
                rb_passValueRadioButtons[i, 1].CheckedChanged += paSearchUI_Handler;
                tl.Rows[^1].Cells.Add(new TableCell { Control = rb_passValueRadioButtons[i, 1] });

                num_passCaseValues[passCaseVal] = new NumericStepper();
                num_passCaseValues[passCaseVal].DecimalPlaces = 2;
                num_passCaseValues[passCaseVal].Increment = 0.01f;
                setSize(num_passCaseValues[passCaseVal], 55);
                num_passCaseValues[passCaseVal].Enabled = false;
                num_passCaseValues[i].LostFocus += paSearchUI_Handler;
                tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_passCaseValues[passCaseVal]) });

                resultFilter_table.Rows[^1].Cells.Add(new TableCell { Control = passCaseValues_pnl[passCaseVal] });
            }
            resultFilter_table.Rows[^1].Cells.Add(new TableCell { Control = null }); // padding
        }

        void paSearchUI_setup_row2(ref Panel p)
        {
            TableLayout tabPage_2D_PASearch_bigTable = new TableLayout();
            p.Content = tabPage_2D_PASearch_bigTable;

            tabPage_2D_PASearch_bigTable.Rows.Add(new TableRow()); // header row

            tabPage_2D_PASearch_bigTable.Rows[0].Cells.Add(new TableCell { Control = null }); // blank, for row header

            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                lbl_paSearchLayers[layer] = new Label();
                lbl_paSearchLayers[layer].Text = (layer + 1).ToString();
                lbl_paSearchLayers[layer].TextColor = UIHelper.myColorToColor(commonVars.getColors().simPreviewColors[layer]);
                lbl_paSearchLayers[layer].TextAlignment = TextAlignment.Center;

                tabPage_2D_PASearch_bigTable.Rows[0].Cells.Add(new TableCell { Control = lbl_paSearchLayers[layer] });
            }

            for (int pa = 0; pa < PASearch.paNames.Length; pa++)
            {
                TableRow tr = new TableRow();

                lbl_paSearchPAs[pa] = new Label();
                lbl_paSearchPAs[pa].Text = PASearch.paNames[pa];
                tr.Cells.Add(new TableCell { Control = lbl_paSearchPAs[pa] });

                for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
                {
                    Panel bTc_p = new Panel();
                    TableLayout tmp_tl = new TableLayout();
                    bTc_p.Content = tmp_tl;

                    // Upper row of tmp_tl
                    tmp_tl.Rows.Add(new TableRow());
                    Panel tc0p = new Panel();
                    tmp_tl.Rows[0].Cells.Add(new TableCell { Control = tc0p });

                    TableLayout tc0p_tl = new TableLayout();
                    tc0p.Content = tc0p_tl;
                    tc0p_tl.Rows.Add(new TableRow());

                    TableCell tc_ur_0 = new TableCell();
                    cb_searchPA[layer, pa] = new CheckBox();
                    cb_searchPA[layer, pa].Text = "";
                    cb_searchPA[layer, pa].CheckedChanged += paSearchUI_Handler;
                    cb_searchPA[layer, pa].ToolTip = "Layer " + (pa + 1) + ": " + PASearch.paNames[pa];
                    tc_ur_0.Control = cb_searchPA[layer, pa];
                    tc0p_tl.Rows[0].Cells.Add(tc_ur_0);

                    TableCell tc_ur_1 = new TableCell();
                    num_searchPA_UpperLimit[layer, pa] = new NumericStepper();
                    num_searchPA_UpperLimit[layer, pa].Increment = 0.01f;
                    num_searchPA_UpperLimit[layer, pa].DecimalPlaces = 2;
                    if (!commonVars.getPASearch().paAllowsNegativeValues(pa))
                    {
                        num_searchPA_UpperLimit[layer, pa].MinValue = 0;
                    }
                    num_searchPA_UpperLimit[layer, pa].TextColor = UIHelper.myColorToColor(commonVars.getColors().simPreviewColors[layer]);
                    num_searchPA_UpperLimit[layer, pa].ToolTip = "Layer " + (layer + 1) + ": " + PASearch.paNames[pa];

                    setSize(num_searchPA_UpperLimit[layer, pa], 55);
                    num_searchPA_UpperLimit[layer, pa].LostFocus += paSearchUI_Handler;
                    tc_ur_1.Control = TableLayout.AutoSized(num_searchPA_UpperLimit[layer, pa]);

                    tc0p_tl.Rows[0].Cells.Add(tc_ur_1);

                    // Lower row of tmp_tl
                    tmp_tl.Rows.Add(new TableRow());

                    text_searchPA_Result[layer, pa] = new TextBox();
                    text_searchPA_Result[layer, pa].ReadOnly = true;
                    text_searchPA_Result[layer, pa].Width = 55 * 2;
                    text_searchPA_Result[layer, pa].TextColor = UIHelper.myColorToColor(commonVars.getColors().simPreviewColors[layer]);
                    text_searchPA_Result[layer, pa].ToolTip = "Layer " + (layer + 1) + ": " + PASearch.paNames[pa];
                    tmp_tl.Rows[1].Cells.Add(new TableCell { Control = text_searchPA_Result[layer, pa] });

                    tr.Cells.Add(new TableCell { Control = bTc_p });
                }
                tabPage_2D_PASearch_bigTable.Rows.Add(tr);
            }
        }

        void paSearchUI_Handler(object sender, EventArgs e)
        {
            updatePASearchUI();
        }

        void updatePASearchUI(int layer)
        {
            bool[] paEnabledState = commonVars.getPASearch().getEnabledState(commonVars.getLayerSettings(layer));
            for (int i = 0; i < paEnabledState.Length; i++)
            {
                cb_searchPA[layer, i].Enabled = paEnabledState[i];
                if (!paEnabledState[i])
                {
                    cb_searchPA[layer, i].Checked = false;
                }
                num_searchPA_UpperLimit[layer, i].Enabled = (bool)cb_searchPA[layer, i].Checked;
                commonVars.getPASearch().setUpperLimit(layer, i, (decimal)num_searchPA_UpperLimit[layer, i].Value);

                commonVars.getPASearch().setPASearchable(layer, i, (bool)cb_searchPA[layer, i].Checked);
            }
        }

        void updatePASearchUI()
        {
            if (paSearchUIFrozen)
            {
                return;
            }

            paSearchUIFrozen = true;

            commonVars.getPASearch().numberofPassCases = (Int32)num_passCase.Value;

            for (int i = 0; i < groupBox_passCaseValues.Length; i++)
            {
                num_passCaseValues[i].Enabled = (bool)cb_passCaseValues[i].Checked;
                rb_passValueRadioButtons[i, 0].Enabled = (bool)cb_passCaseValues[i].Checked;
                rb_passValueRadioButtons[i, 1].Enabled = (bool)cb_passCaseValues[i].Checked;

                commonVars.getPASearch().filterValues[i] = num_passCaseValues[i].Value;
                commonVars.getPASearch().filterIsMaxValue[i] = rb_passValueRadioButtons[i, 1].Checked;
            }

            bool chordMode = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.chord;
            for (int i = 0; i < groupBox_passCaseValues.Length; i++)
            {
                if (!chordMode)
                {
                    groupBox_passCaseValues[i].Enabled = false;
                    cb_passCaseValues[i].Checked = false;
                }
                commonVars.getPASearch().useFilter[i] = (bool)cb_passCaseValues[i].Checked;
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

        void paSearchUI_showResults(SimResultPackage resultPackage)
        {
            commonVars.getPASearch().calculateMeanAndStdDev(resultPackage);
            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                for (int pa = 0; pa < PASearch.paNames.Length; pa++)
                {
                    Application.Instance.Invoke(() =>
                    {
                        if (commonVars.getPASearch().isPASearchable(layer, pa))
                        {
                            text_searchPA_Result[layer, pa].Text = "x:" + commonVars.getPASearch().getMeanValue(layer, pa) + ", s:" + commonVars.getPASearch().getSDValue(layer, pa);
                        }
                        else
                        {
                            text_searchPA_Result[layer, pa].Text = "";
                        }
                    });
                }
            }

            updateProgressBar(resultPackage.getListOfResults().Count);
            updateStatusLine("PA Search for " + resultPackage.getListOfResults().Count + " results in " + resultPackage.runTime.ToString("0.##") + " seconds.");
        }
    }
}