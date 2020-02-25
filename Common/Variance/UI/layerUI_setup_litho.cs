﻿using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Litho
        NumericStepper num_layer_lithoICRR_exp, num_layer_lithoOCRR_exp, num_layer_lithoCDUSide_exp, num_layer_lithoHorOverlay_exp, num_layer_lithoVerOverlay_exp,
                        num_layer_lithoICV_exp, num_layer_lithoOCV_exp, num_layer_lithoCDUTips_exp, num_layer_lithoLWR_exp, num_layer_lithoLWRFreq_exp, num_layer_lithoLWR2_exp, num_layer_lithoLWR2Freq_exp, num_layer_lithoWobble_exp, num_layer_coeff1_exp, num_layer_coeff2_exp;
        Label lbl_layer_lithoICRR_exp, lbl_layer_lithoOCRR_exp, lbl_layer_lithoCDUSide_exp, lbl_layer_lithoHorOverlay_exp, lbl_layer_lithoVerOverlay_exp,
                lbl_layer_lithoICV_exp, lbl_layer_lithoOCV_exp, lbl_layer_lithoCDUTips_exp, lbl_layer_lithoLWR_exp, lbl_layer_lithoLWRFreq_exp, lbl_layer_lithoLWR2_exp, lbl_layer_lithoLWR2Freq_exp, lbl_layer_lithoWobble_exp, lbl_layer_coeff1_exp, lbl_layer_coeff2_exp;
        GroupBox groupBox_layer_overlayYReference_exp, groupBox_layer_overlayXReference_exp,
                    groupBox_layer_overlayYReference_Av_exp, groupBox_layer_overlayXReference_Av_exp,
                    groupBox_layer_overlayYCorrelation_exp, groupBox_layer_overlayXCorrelation_exp,
                    groupBox_layer_CDUCorrelation_exp, groupBox_layer_TipCDUCorrelation_exp;

        RadioButton[] rB_layer_OLRX_exp, rB_layer_OLRY_exp, rB_layer_COLX_exp, rB_layer_COLY_exp, rB_layer_CCDU_exp, rB_layer_CTCDU_exp;
        CheckBox checkBox_layer_overlayXReference_Av_exp, checkBox_layer_overlayYReference_Av_exp, cB_layer_LWRPreview_exp;

        CheckBox[] cB_layer_OLRX_Av_exp, cB_layer_OLRY_Av_exp;

        DropDown comboBox_layerLWRNoiseType_exp, comboBox_layerLWR2NoiseType_exp;

        TableLayout groupBox_layer_overlayXReference_Av_table, groupBox_layer_overlayYReference_Av_table;

        Panel pnl_overlayRefX, pnl_overlayRefY;

        void twoD_LayerUISetup_litho_exp(TableCell tc)
        {
            Application.Instance.Invoke(() =>
            {
                groupBox_layer_lithography_exp = new GroupBox();
                groupBox_layer_lithography_exp.Text = "Lithography Parameters";
                TableLayout groupBox_layer_lithography_table = new TableLayout();
                groupBox_layer_lithography_exp.Content = groupBox_layer_lithography_table;

                TableLayout t = new TableLayout();
                t.Rows.Add(new TableRow());
                t.Rows[0].Cells.Add(new TableCell() { Control = groupBox_layer_lithography_exp });
                t.Rows[0].Cells.Add(new TableCell() { Control = new Panel(), ScaleWidth = true });

                Panel p = new Panel();
                p.Content = t;
                tc.Control = p;

                litho_row1(groupBox_layer_lithography_table);
                litho_row2(groupBox_layer_lithography_table);
                litho_row3(groupBox_layer_lithography_table);
                litho_row4(groupBox_layer_lithography_table);
                litho_row5(groupBox_layer_lithography_table);
                litho_row6(groupBox_layer_lithography_table);
                litho_row7(groupBox_layer_lithography_table);
                litho_row8(groupBox_layer_lithography_table);
            });
        }

        // ICR, LWR
        void litho_row1(TableLayout groupBox_layer_lithography_table)
        {
            // Outer table, row 1
            TableRow lit_tr0 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_tr0);
            lit_tr0.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout row0_tl = new TableLayout();
            lit_tr0.Cells[0].Control = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lit_tr0_0 = new TableCell();
            row0_tl.Rows[0].Cells.Add(lit_tr0_0);

            TableLayout lit_tr0_0_tl = new TableLayout();
            lit_tr0_0.Control = lit_tr0_0_tl;

            lit_tr0_0_tl.Rows.Add(new TableRow());

            lbl_layer_lithoICRR_exp = new Label();
            lbl_layer_lithoICRR_exp.Text = "Inner Radius";
            lbl_layer_lithoICRR_exp.ToolTip = "Inner vertex (concave) corner rounding radius.";
            lit_tr0_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr0_0_tl.Rows[0].Cells[0].Control = lbl_layer_lithoICRR_exp;
            lit_tr0_0_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoICRR_exp = new NumericStepper();
            num_layer_lithoICRR_exp.Increment = 0.1;
            num_layer_lithoICRR_exp.DecimalPlaces = 2;
            num_layer_lithoICRR_exp.MinValue = 0;
            num_layer_lithoICRR_exp.ToolTip = "Inner vertex (concave) corner rounding radius.";
            setSize(num_layer_lithoICRR_exp, 55, (int)(label_Height * uiScaleFactor));
            lit_tr0_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr0_0_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoICRR_exp);

            TableCell lit_tr0_1 = new TableCell();
            lit_tr0.Cells.Add(lit_tr0_1);

            TableLayout lit_tr0_1_tl = new TableLayout();
            lit_tr0_1.Control = lit_tr0_1_tl;

            lit_tr0_1_tl.Rows.Add(new TableRow());

            lbl_layer_lithoICV_exp = new Label();
            lbl_layer_lithoICV_exp.Text = "Var";
            lbl_layer_lithoICV_exp.MouseDoubleClick += ICV_RNG;
            lbl_layer_lithoICV_exp.ToolTip = "3-sigma inner vertex (concave) corner rounding radius variation.";
            lit_tr0_1_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr0_1_tl.Rows[0].Cells[0].Control = lbl_layer_lithoICV_exp;

            num_layer_lithoICV_exp = new NumericStepper();
            num_layer_lithoICV_exp.Increment = 0.1;
            num_layer_lithoICV_exp.DecimalPlaces = 2;
            num_layer_lithoICV_exp.MinValue = 0;
            num_layer_lithoICV_exp.ToolTip = "3-sigma inner vertex (concave) corner rounding radius variation.";
            setSize(num_layer_lithoICV_exp, 55, (int)(label_Height * uiScaleFactor));
            lit_tr0_1_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr0_1_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoICV_exp);

            TableCell lit_tr0_2 = new TableCell();
            lit_tr0.Cells.Add(lit_tr0_2);

            Panel p = new Panel();
            lit_tr0_2.Control = p;

            TableLayout ptl = new TableLayout();
            p.Content = ptl;

            ptl.Rows.Add(new TableRow());

            ptl.Rows.Add(new TableRow());
            ptl.Rows[0].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
            TableCell tc0 = new TableCell();
            TableCell tc1 = new TableCell();
            ptl.Rows[0].Cells.Add(tc0);
            ptl.Rows[0].Cells.Add(tc1);

            Panel p1 = new Panel();
            Panel p2 = new Panel();
            tc0.Control = p1;
            tc1.Control = p2;

            TableLayout tc0tl = new TableLayout();
            TableLayout tc1tl = new TableLayout();
            p1.Content = tc0tl;
            p2.Content = tc1tl;

            tc0tl.Rows.Add(new TableRow());
            tc1tl.Rows.Add(new TableRow());


            lbl_layer_lithoWobble_exp = new Label();
            lbl_layer_lithoWobble_exp.Text = "Wobble";
            lbl_layer_lithoWobble_exp.MouseDoubleClick += wobble_RNG;
            lbl_layer_lithoWobble_exp.ToolTip = "3-sigma rotational variation.";
            tc0tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoWobble_exp });

            num_layer_lithoWobble_exp = new NumericStepper();
            num_layer_lithoWobble_exp.Increment = 0.1;
            num_layer_lithoWobble_exp.DecimalPlaces = 2;
            num_layer_lithoWobble_exp.MinValue = 0;
            num_layer_lithoWobble_exp.ToolTip = "3-sigma rotational variation.";
            setSize(num_layer_lithoWobble_exp, 55, (int)(label_Height * uiScaleFactor));
            tc0tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoWobble_exp) });

            lbl_layer_lithoLWR_exp = new Label();
            lbl_layer_lithoLWR_exp.Text = "LWR";
            lbl_layer_lithoLWR_exp.MouseDoubleClick += lwr_RNG;
            lbl_layer_lithoLWR_exp.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            tc1tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoLWR_exp });

            tc0tl.Rows[0].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            num_layer_lithoLWR_exp = new NumericStepper();
            num_layer_lithoLWR_exp.Increment = 0.1;
            num_layer_lithoLWR_exp.DecimalPlaces = 2;
            num_layer_lithoLWR_exp.MinValue = 0;
            num_layer_lithoLWR_exp.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            setSize(num_layer_lithoLWR_exp, 55, (int)(label_Height * uiScaleFactor));
            tc1tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoLWR_exp) });

            lbl_layer_lithoLWRFreq_exp = new Label();
            lbl_layer_lithoLWRFreq_exp.Text = "Freq";
            lbl_layer_lithoLWRFreq_exp.ToolTip = "Frequency of LWR";
            tc1tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoLWRFreq_exp });

            num_layer_lithoLWRFreq_exp = new NumericStepper();
            num_layer_lithoLWRFreq_exp.Increment = 0.1;
            num_layer_lithoLWRFreq_exp.DecimalPlaces = 2;
            num_layer_lithoLWRFreq_exp.Value = 0.1;
            num_layer_lithoLWRFreq_exp.MinValue = 0.01;
            num_layer_lithoLWRFreq_exp.ToolTip = "Frequency of LWR";
            setSize(num_layer_lithoLWRFreq_exp, 55, num_Height);
            tc1tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoLWRFreq_exp) });

            comboBox_layerLWRNoiseType_exp = new DropDown();
            comboBox_layerLWRNoiseType_exp.DataContext = DataContext;
            comboBox_layerLWRNoiseType_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.noiseTypeList);
            comboBox_layerLWRNoiseType_exp.SelectedIndex = 0;
            comboBox_layerLWRNoiseType_exp.ToolTip = "Noise type for LWR";
            lit_tr0.Cells.Add(new TableCell() { Control = comboBox_layerLWRNoiseType_exp });

            cB_layer_LWRPreview_exp = new CheckBox();
            cB_layer_LWRPreview_exp.Text = "Show";
            cB_layer_LWRPreview_exp.ToolTip = "Preview of LWR";
            lit_tr0.Cells.Add(new TableCell() { Control = cB_layer_LWRPreview_exp });
        }

        // OCR, LWR2
        void litho_row2(TableLayout groupBox_layer_lithography_table)
        {
            // Outer table, row 2
            TableRow lit_tr1 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_tr1);
            lit_tr1.Cells.Add(new TableCell());

            // Table layout within row 2
            TableLayout row1_tl = new TableLayout();
            lit_tr1.Cells[0].Control = row1_tl;
            row1_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lit_tr1_0 = new TableCell();
            row1_tl.Rows[0].Cells.Add(lit_tr1_0);

            TableLayout lit_tr1_0_tl = new TableLayout();
            lit_tr1_0.Control = lit_tr1_0_tl;

            lit_tr1_0_tl.Rows.Add(new TableRow());

            lbl_layer_lithoOCRR_exp = new Label();
            lbl_layer_lithoOCRR_exp.Text = "Outer Radius";
            lbl_layer_lithoOCRR_exp.ToolTip = "Outer vertex (concave) corner rounding radius.";
            lit_tr1_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr1_0_tl.Rows[0].Cells[0].Control = lbl_layer_lithoOCRR_exp;
            lit_tr1_0_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoOCRR_exp = new NumericStepper();
            num_layer_lithoOCRR_exp.Increment = 0.1;
            num_layer_lithoOCRR_exp.DecimalPlaces = 2;
            num_layer_lithoOCRR_exp.MinValue = 0;
            num_layer_lithoOCRR_exp.ToolTip = "Outer vertex (concave) corner rounding radius.";
            setSize(num_layer_lithoOCRR_exp, 55, (int)(label_Height * uiScaleFactor));
            lit_tr1_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr1_0_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoOCRR_exp);

            TableCell lit_tr1_1 = new TableCell();
            lit_tr1.Cells.Add(lit_tr1_1);

            TableLayout lit_tr1_1_tl = new TableLayout();
            lit_tr1_1.Control = lit_tr1_1_tl;

            lit_tr1_1_tl.Rows.Add(new TableRow());

            lbl_layer_lithoOCV_exp = new Label();
            lbl_layer_lithoOCV_exp.Text = "Var";
            lbl_layer_lithoOCV_exp.MouseDoubleClick += ICV_RNG;
            lbl_layer_lithoOCV_exp.ToolTip = "3-sigma outer vertex (concave) corner rounding radius variation.";
            lit_tr1_1_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr1_1_tl.Rows[0].Cells[0].Control = lbl_layer_lithoOCV_exp;

            num_layer_lithoOCV_exp = new NumericStepper();
            num_layer_lithoOCV_exp.Increment = 0.1;
            num_layer_lithoOCV_exp.DecimalPlaces = 2;
            num_layer_lithoOCV_exp.MinValue = 0;
            num_layer_lithoOCV_exp.ToolTip = "3-sigma outer vertex (concave) corner rounding radius variation.";
            setSize(num_layer_lithoOCV_exp, 55, (int)(label_Height * uiScaleFactor));
            lit_tr1_1_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr1_1_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoOCV_exp);

            TableCell lit_tr1_2 = new TableCell();
            lit_tr1.Cells.Add(lit_tr1_2);

            Panel p = new Panel();
            lit_tr1_2.Control = p;

            TableLayout ptl = new TableLayout();
            p.Content = ptl;

            ptl.Rows.Add(new TableRow());

            ptl.Rows[0].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            lbl_layer_lithoLWR2_exp = new Label();
            lbl_layer_lithoLWR2_exp.Text = "LWR2";
            lbl_layer_lithoLWR2_exp.MouseDoubleClick += lwr2_RNG;
            lbl_layer_lithoLWR2_exp.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            ptl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoLWR2_exp });

            num_layer_lithoLWR2_exp = new NumericStepper();
            num_layer_lithoLWR2_exp.Increment = 0.1;
            num_layer_lithoLWR2_exp.DecimalPlaces = 2;
            num_layer_lithoLWR2_exp.MinValue = 0;
            num_layer_lithoLWR2_exp.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            setSize(num_layer_lithoLWR2_exp, 55, (int)(label_Height * uiScaleFactor));
            ptl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoLWR2_exp) });

            lbl_layer_lithoLWR2Freq_exp = new Label();
            lbl_layer_lithoLWR2Freq_exp.Text = "Freq";
            lbl_layer_lithoLWR2Freq_exp.ToolTip = "Frequency of LWR2";
            ptl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoLWR2Freq_exp });

            num_layer_lithoLWR2Freq_exp = new NumericStepper();
            num_layer_lithoLWR2Freq_exp.Increment = 0.1;
            num_layer_lithoLWR2Freq_exp.DecimalPlaces = 2;
            num_layer_lithoLWR2Freq_exp.Value = 0.1;
            num_layer_lithoLWR2Freq_exp.MinValue = 0.01;
            num_layer_lithoLWR2Freq_exp.ToolTip = "Frequency of LWR2";
            setSize(num_layer_lithoLWR2Freq_exp, 55, num_Height);
            ptl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoLWR2Freq_exp) });

            comboBox_layerLWR2NoiseType_exp = new DropDown();
            comboBox_layerLWR2NoiseType_exp.Width = 120;
            comboBox_layerLWR2NoiseType_exp.DataContext = DataContext;
            comboBox_layerLWR2NoiseType_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.noiseTypeList);
            comboBox_layerLWR2NoiseType_exp.SelectedIndex = 0;
            comboBox_layerLWR2NoiseType_exp.ToolTip = "Noise type for LWR2";
            lit_tr1.Cells.Add(new TableCell() { Control = comboBox_layerLWR2NoiseType_exp });

            lit_tr1.Cells.Add(new TableCell() { Control = null });
        }

        // SCDU, lens distortion
        void litho_row3(TableLayout groupBox_layer_lithography_table)
        {
            // Outer table, row 3
            TableRow lit_tr2 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_tr2);
            lit_tr2.Cells.Add(new TableCell());

            // Table layout within row 3
            TableLayout row2_tl = new TableLayout();
            lit_tr2.Cells[0].Control = row2_tl;
            row2_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lit_tr2_0 = new TableCell();
            row2_tl.Rows[0].Cells.Add(lit_tr2_0);

            TableLayout lit_tr2_0_tl = new TableLayout();
            lit_tr2_0.Control = lit_tr2_0_tl;

            lit_tr2_0_tl.Rows.Add(new TableRow());

            lbl_layer_lithoCDUSide_exp = new Label();
            lbl_layer_lithoCDUSide_exp.Text = "Side CDU";
            lbl_layer_lithoCDUSide_exp.MouseDoubleClick += sCDU_RNG;
            lbl_layer_lithoCDUSide_exp.ToolTip = "3-sigma CD variation applied to non-tip sides.";
            lit_tr2_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr2_0_tl.Rows[0].Cells[0].Control = lbl_layer_lithoCDUSide_exp;
            lit_tr2_0_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoCDUSide_exp = new NumericStepper();
            num_layer_lithoCDUSide_exp.Increment = 0.1;
            num_layer_lithoCDUSide_exp.DecimalPlaces = 2;
            num_layer_lithoCDUSide_exp.MinValue = 0;
            num_layer_lithoCDUSide_exp.ToolTip = "3-sigma CD variation applied to non-tip sides.";
            setSize(num_layer_lithoCDUSide_exp, 55, num_Height);
            lit_tr2_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr2_0_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoCDUSide_exp);

            lit_tr2.Cells.Add(new TableCell());

            TableCell lit_tr2_1 = new TableCell();
            lit_tr2.Cells.Add(lit_tr2_1);

            Panel p = new Panel();
            lit_tr2_1.Control = p;

            TableLayout ptl = new TableLayout();
            p.Content = ptl;
            ptl.Rows.Add(new TableRow());
            TableCell ptc = new TableCell();
            ptl.Rows[0].Cells.Add(ptc);

            groupBox_layer_CDUCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_CDUCorrelation_content = new TableLayout();
            groupBox_layer_CDUCorrelation_exp.Text = "Side CDU Correlation";
            groupBox_layer_CDUCorrelation_exp.Content = groupBox_layer_CDUCorrelation_content;
            ptc.Control = groupBox_layer_CDUCorrelation_exp;//, x, y);

            TableRow ccdu_tr0 = new TableRow();
            groupBox_layer_CDUCorrelation_content.Rows.Add(ccdu_tr0);
            TableRow ccdu_tr1 = new TableRow();
            groupBox_layer_CDUCorrelation_content.Rows.Add(ccdu_tr1);

            rB_layer_CCDU_exp[0] = new RadioButton();
            rB_layer_CCDU_exp[0].Text = "0";
            rB_layer_CCDU_exp[0].Checked = true;

            TableCell rB_layer_CCDU_0tc = new TableCell();
            rB_layer_CCDU_0tc.Control = rB_layer_CCDU_exp[0];

            groupBox_layer_CDUCorrelation_content.Rows[0].Cells.Add(rB_layer_CCDU_0tc);

            int rB_CCDU = 1;
            int ccdu_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layer_CCDU_exp[rB_CCDU] = new RadioButton(rB_layer_CCDU_exp[0]);
                rB_layer_CCDU_exp[rB_CCDU].Text = rB_CCDU.ToString();
                rB_layer_CCDU_exp[rB_CCDU].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_CCDU_exp[rB_CCDU];
                groupBox_layer_CDUCorrelation_content.Rows[ccdu_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_CCDU + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    ccdu_rowIndex++;
                }
                rB_CCDU++;
            }


            Panel p2 = new Panel();
            TableLayout ltl = new TableLayout();
            p2.Content = ltl;
            lit_tr2.Cells.Add(new TableCell() { Control = p2 });

            ltl.Rows.Add(new TableRow());

            lbl_layer_coeff1_exp = new Label();
            lbl_layer_coeff1_exp.Text = "Lens k1";
            lbl_layer_coeff1_exp.ToolTip = "Lens k1";
            ltl.Rows[ltl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layer_coeff1_exp });

            num_layer_coeff1_exp = new NumericStepper();
            num_layer_coeff1_exp.Increment = 0.1;
            num_layer_coeff1_exp.DecimalPlaces = 2;
            num_layer_coeff1_exp.Value = 0.0;
            num_layer_coeff1_exp.ToolTip = "Lens k1";
            setSize(num_layer_coeff1_exp, 55, num_Height);
            ltl.Rows[ltl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_coeff1_exp) });

            ltl.Rows[ltl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            ltl.Rows.Add(new TableRow());

            lbl_layer_coeff2_exp = new Label();
            lbl_layer_coeff2_exp.Text = "Lens k2";
            lbl_layer_coeff2_exp.ToolTip = "Lens k2";
            ltl.Rows[ltl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layer_coeff2_exp });

            num_layer_coeff2_exp = new NumericStepper();
            num_layer_coeff2_exp.Increment = 0.1;
            num_layer_coeff2_exp.DecimalPlaces = 2;
            num_layer_coeff2_exp.Value = 0.0;
            num_layer_coeff2_exp.ToolTip = "Lens k2";
            setSize(num_layer_coeff2_exp, 55, num_Height);
            ltl.Rows[ltl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_coeff2_exp) });

            ltl.Rows[ltl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
        }

        // TCDU
        void litho_row4(TableLayout groupBox_layer_lithography_table)
        {
            // Outer table, row 4
            TableRow lit_tr3 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_tr3);
            lit_tr3.Cells.Add(new TableCell());

            // Table layout within row 4
            TableLayout row3_tl = new TableLayout();
            lit_tr3.Cells[0].Control = row3_tl;
            row3_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lit_tr3_0 = new TableCell();
            row3_tl.Rows[0].Cells.Add(lit_tr3_0);

            TableLayout lit_tr3_0_tl = new TableLayout();
            lit_tr3_0.Control = lit_tr3_0_tl;

            lit_tr3_0_tl.Rows.Add(new TableRow());

            lbl_layer_lithoCDUTips_exp = new Label();
            lbl_layer_lithoCDUTips_exp.Text = "Tips CDU";
            lbl_layer_lithoCDUTips_exp.MouseDoubleClick += tCDU_RNG;
            lbl_layer_lithoCDUTips_exp.ToolTip = "3-sigma CD variation applied to tip sides.";
            lit_tr3_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr3_0_tl.Rows[0].Cells[0].Control = lbl_layer_lithoCDUTips_exp;
            lit_tr3_0_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoCDUTips_exp = new NumericStepper();
            num_layer_lithoCDUTips_exp.Increment = 0.1;
            num_layer_lithoCDUTips_exp.DecimalPlaces = 2;
            num_layer_lithoCDUTips_exp.MinValue = 0;
            num_layer_lithoCDUTips_exp.ToolTip = "3-sigma CD variation applied to tip sides.";
            setSize(num_layer_lithoCDUTips_exp, 55, num_Height);
            lit_tr3_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr3_0_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoCDUTips_exp);

            lit_tr3.Cells.Add(new TableCell());

            TableCell lit_tr3_1 = new TableCell();
            lit_tr3.Cells.Add(lit_tr3_1);

            Panel p = new Panel();
            lit_tr3_1.Control = p;

            TableLayout ptl = new TableLayout();
            p.Content = ptl;
            ptl.Rows.Add(new TableRow());
            TableCell ptc = new TableCell();
            ptl.Rows[0].Cells.Add(ptc);

            groupBox_layer_TipCDUCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_TipCDUCorrelation_content = new TableLayout();
            groupBox_layer_TipCDUCorrelation_exp.Text = "Tip CDU Correlation";
            groupBox_layer_TipCDUCorrelation_exp.Content = groupBox_layer_TipCDUCorrelation_content;
            ptc.Control = groupBox_layer_TipCDUCorrelation_exp;

            TableRow tcdu_tr0 = new TableRow();
            groupBox_layer_TipCDUCorrelation_content.Rows.Add(tcdu_tr0);
            TableRow tcdu_tr1 = new TableRow();
            groupBox_layer_TipCDUCorrelation_content.Rows.Add(tcdu_tr1);

            rB_layer_CTCDU_exp[0] = new RadioButton();
            rB_layer_CTCDU_exp[0].Text = "0";
            rB_layer_CTCDU_exp[0].Checked = true;

            TableCell rB_layer_CTCDU_0tc = new TableCell();
            rB_layer_CTCDU_0tc.Control = rB_layer_CTCDU_exp[0];

            groupBox_layer_TipCDUCorrelation_content.Rows[0].Cells.Add(rB_layer_CTCDU_0tc);

            int rB_CTCDU = 1;
            int ctcdu_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layer_CTCDU_exp[rB_CTCDU] = new RadioButton(rB_layer_CTCDU_exp[0]);
                rB_layer_CTCDU_exp[rB_CTCDU].Text = rB_CTCDU.ToString();
                rB_layer_CTCDU_exp[rB_CTCDU].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_CTCDU_exp[rB_CTCDU];
                groupBox_layer_TipCDUCorrelation_content.Rows[ctcdu_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_CTCDU + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    ctcdu_rowIndex++;
                }
                rB_CTCDU++;
            }
        }

        // XOL, XOLR
        void litho_row5(TableLayout groupBox_layer_lithography_table)
        {
            // Outer table, row 5
            TableRow lit_tr4 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_tr4);
            lit_tr4.Cells.Add(new TableCell());

            // Table layout within row 5
            TableLayout row4_tl = new TableLayout();
            lit_tr4.Cells[0].Control = row4_tl;
            row4_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lit_tr4_0 = new TableCell();
            row4_tl.Rows[0].Cells.Add(lit_tr4_0);

            TableLayout lit_tr4_0_tl = new TableLayout();
            lit_tr4_0.Control = lit_tr4_0_tl;

            lit_tr4_0_tl.Rows.Add(new TableRow());

            lbl_layer_lithoHorOverlay_exp = new Label();
            lbl_layer_lithoHorOverlay_exp.Text = "Hor Overlay";
            lbl_layer_lithoHorOverlay_exp.MouseDoubleClick += hOverlay_RNG;
            lbl_layer_lithoHorOverlay_exp.ToolTip = "3-sigma horizontal overlay.";
            lit_tr4_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr4_0_tl.Rows[0].Cells[0].Control = lbl_layer_lithoHorOverlay_exp;
            lit_tr4_0_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoHorOverlay_exp = new NumericStepper();
            num_layer_lithoHorOverlay_exp.Increment = 0.1;
            num_layer_lithoHorOverlay_exp.DecimalPlaces = 2;
            num_layer_lithoHorOverlay_exp.MinValue = 0;
            num_layer_lithoHorOverlay_exp.ToolTip = "3-sigma horizontal overlay.";
            setSize(num_layer_lithoHorOverlay_exp, 55, num_Height);
            lit_tr4_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr4_0_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoHorOverlay_exp);

            TableCell lit_tr4_1 = new TableCell();
            lit_tr4.Cells.Add(lit_tr4_1);

            TableLayout lit_tr4_1_tl = new TableLayout();
            lit_tr4_1.Control = lit_tr4_1_tl;

            lit_tr4_1_tl.Rows.Add(new TableRow());

            checkBox_layer_overlayXReference_Av_exp = new CheckBox();
            checkBox_layer_overlayXReference_Av_exp.Text = "Av";
            checkBox_layer_overlayXReference_Av_exp.ToolTip = "Use the average displaced location of layers as a reference for horizontal overlay";
            lit_tr4_1_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr4_1_tl.Rows[0].Cells[0].Control = TableLayout.AutoSized(checkBox_layer_overlayXReference_Av_exp);

            TableCell lit_tr4_2 = new TableCell();
            lit_tr4.Cells.Add(lit_tr4_2);

            groupBox_layer_overlayXReference_exp = new GroupBox();
            TableLayout groupBox_layer_overlayXReference_table = new TableLayout();
            groupBox_layer_overlayXReference_exp.Text = "X Overlay Reference";
            groupBox_layer_overlayXReference_exp.Content = groupBox_layer_overlayXReference_table;
            pnl_overlayRefX = new Panel();
            lit_tr4_2.Control = pnl_overlayRefX;
            pnl_overlayRefX.Content = groupBox_layer_overlayXReference_exp;

            TableRow olrx_tr0 = new TableRow();
            groupBox_layer_overlayXReference_table.Rows.Add(olrx_tr0);
            TableRow olrx_tr1 = new TableRow();
            groupBox_layer_overlayXReference_table.Rows.Add(olrx_tr1);

            rB_layer_OLRX_exp[0] = new RadioButton();
            rB_layer_OLRX_exp[0].Text = "0";
            rB_layer_OLRX_exp[0].Checked = true;

            TableCell rB_layer_OLRX_0tc = new TableCell();
            rB_layer_OLRX_0tc.Control = rB_layer_OLRX_exp[0];

            groupBox_layer_overlayXReference_table.Rows[0].Cells.Add(rB_layer_OLRX_0tc);

            int rB_OLRX = 1;
            int olrx_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layer_OLRX_exp[rB_OLRX] = new RadioButton(rB_layer_OLRX_exp[0]);
                rB_layer_OLRX_exp[rB_OLRX].Text = rB_OLRX.ToString();
                rB_layer_OLRX_exp[rB_OLRX].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_OLRX_exp[rB_OLRX];
                groupBox_layer_overlayXReference_table.Rows[olrx_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_OLRX + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    olrx_rowIndex++;
                }
                rB_OLRX++;
            }

            groupBox_layer_overlayXReference_Av_exp = new GroupBox();
            groupBox_layer_overlayXReference_Av_table = new TableLayout();
            groupBox_layer_overlayXReference_Av_exp.Text = "X Overlay References";
            groupBox_layer_overlayXReference_Av_exp.Content = groupBox_layer_overlayXReference_Av_table;

            TableRow avolrx_tr0 = new TableRow();
            groupBox_layer_overlayXReference_Av_table.Rows.Add(avolrx_tr0);
            TableRow avolrx_tr1 = new TableRow();
            groupBox_layer_overlayXReference_Av_table.Rows.Add(avolrx_tr1);

            int avolrx_rowIndex = 0;
            for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
            {
                // Don't add a button for our current layer
                cB_layer_OLRX_Av_exp[cb] = new CheckBox();
                cB_layer_OLRX_Av_exp[cb].Text = (cb + 1).ToString();

                TableCell tc0 = new TableCell();
                tc0.Control = cB_layer_OLRX_Av_exp[cb];
                groupBox_layer_overlayXReference_Av_table.Rows[avolrx_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (cb + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    avolrx_rowIndex++;
                }
            }
        }

        // COLX
        void litho_row6(TableLayout groupBox_layer_lithography_table)
        {
            // Outer table, row 6
            TableRow lit_tr5 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_tr5);
            lit_tr5.Cells.Add(new TableCell());

            // Table layout within row 6
            TableLayout row5_tl = new TableLayout();
            lit_tr5.Cells[0].Control = row5_tl;
            row5_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lit_tr5_0 = new TableCell();
            row5_tl.Rows[0].Cells.Add(lit_tr5_0);

            TableLayout lit_tr5_0_tl = new TableLayout();
            lit_tr5_0.Control = lit_tr5_0_tl;

            lit_tr5_0_tl.Rows.Add(new TableRow());

            lit_tr5_0_tl.Rows.Add(new TableRow());

            Panel p1 = new Panel();
            lit_tr5_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr5_0_tl.Rows[0].Cells[0].Control = p1;

            Panel p2 = new Panel();
            lit_tr5_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr5_0_tl.Rows[0].Cells[1].Control = p2;

            TableCell lit_tr5_1 = new TableCell();
            lit_tr5.Cells.Add(lit_tr5_1);

            TableLayout lit_tr5_1_tl = new TableLayout();
            lit_tr5_1.Control = lit_tr5_1_tl;

            lit_tr5_1_tl.Rows.Add(new TableRow());

            Panel p3 = new Panel();
            lit_tr5_1_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr5_1_tl.Rows[0].Cells[0].Control = p3;

            TableCell lit_tr5_2 = new TableCell();
            lit_tr5.Cells.Add(lit_tr5_2);

            groupBox_layer_overlayXCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_overlayXCorrelation_content = new TableLayout();
            groupBox_layer_overlayXCorrelation_exp.Text = "X Overlay Correlation";
            groupBox_layer_overlayXCorrelation_exp.Content = groupBox_layer_overlayXCorrelation_content;
            lit_tr5_2.Control = groupBox_layer_overlayXCorrelation_exp;

            TableRow olcx_tr0 = new TableRow();
            groupBox_layer_overlayXCorrelation_content.Rows.Add(olcx_tr0);
            TableRow olcx_tr1 = new TableRow();
            groupBox_layer_overlayXCorrelation_content.Rows.Add(olcx_tr1);

            rB_layer_COLX_exp[0] = new RadioButton();
            rB_layer_COLX_exp[0].Text = "0";
            rB_layer_COLX_exp[0].Checked = true;

            TableCell rB_layer_COLX_0tc = new TableCell();
            rB_layer_COLX_0tc.Control = rB_layer_COLX_exp[0];

            groupBox_layer_overlayXCorrelation_content.Rows[0].Cells.Add(rB_layer_COLX_0tc);

            int rB_COLX = 1;
            int colx_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layer_COLX_exp[rB_COLX] = new RadioButton(rB_layer_COLX_exp[0]);
                rB_layer_COLX_exp[rB_COLX].Text = rB_COLX.ToString();
                rB_layer_COLX_exp[rB_COLX].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_COLX_exp[rB_COLX];
                groupBox_layer_overlayXCorrelation_content.Rows[colx_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_COLX + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    colx_rowIndex++;
                }
                rB_COLX++;
            }
        }

        // YOL, YOLR
        void litho_row7(TableLayout groupBox_layer_lithography_table)
        {
            // Outer table, row 7
            TableRow lit_tr6 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_tr6);
            lit_tr6.Cells.Add(new TableCell());

            // Table layout within row 7
            TableLayout row6_tl = new TableLayout();
            lit_tr6.Cells[0].Control = row6_tl;
            row6_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lit_tr6_0 = new TableCell();
            row6_tl.Rows[0].Cells.Add(lit_tr6_0);

            TableLayout lit_tr6_0_tl = new TableLayout();
            lit_tr6_0.Control = lit_tr6_0_tl;

            lit_tr6_0_tl.Rows.Add(new TableRow());

            lbl_layer_lithoVerOverlay_exp = new Label();
            lbl_layer_lithoVerOverlay_exp.Text = "Ver Overlay";
            lbl_layer_lithoVerOverlay_exp.MouseDoubleClick += hOverlay_RNG;
            lbl_layer_lithoVerOverlay_exp.Width = 86;
            lbl_layer_lithoVerOverlay_exp.ToolTip = "3-sigma horizontal overlay.";
            lit_tr6_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr6_0_tl.Rows[0].Cells[0].Control = lbl_layer_lithoVerOverlay_exp;
            lit_tr6_0_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoVerOverlay_exp = new NumericStepper();
            num_layer_lithoVerOverlay_exp.Increment = 0.1;
            num_layer_lithoVerOverlay_exp.DecimalPlaces = 2;
            num_layer_lithoVerOverlay_exp.MinValue = 0;
            num_layer_lithoVerOverlay_exp.ToolTip = "3-sigma vertical overlay.";
            setSize(num_layer_lithoVerOverlay_exp, 55, num_Height);
            lit_tr6_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr6_0_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoVerOverlay_exp);

            TableCell lit_tr6_1 = new TableCell();
            lit_tr6.Cells.Add(lit_tr6_1);

            TableLayout lit_tr6_1_tl = new TableLayout();
            lit_tr6_1.Control = lit_tr6_1_tl;

            lit_tr6_1_tl.Rows.Add(new TableRow());

            checkBox_layer_overlayYReference_Av_exp = new CheckBox();
            checkBox_layer_overlayYReference_Av_exp.Text = "Av";
            checkBox_layer_overlayYReference_Av_exp.ToolTip = "Use the average displaced location of layers as a reference for vertical overlay";
            lit_tr6_1_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr6_1_tl.Rows[0].Cells[0].Control = TableLayout.AutoSized(checkBox_layer_overlayYReference_Av_exp);

            TableCell lit_tr6_2 = new TableCell();
            lit_tr6.Cells.Add(lit_tr6_2);

            groupBox_layer_overlayYReference_exp = new GroupBox();
            TableLayout groupBox_layer_overlayYReference_content = new TableLayout();
            groupBox_layer_overlayYReference_exp.Text = "Y Overlay Reference";
            groupBox_layer_overlayYReference_exp.Content = groupBox_layer_overlayYReference_content;
            pnl_overlayRefY = new Panel();
            lit_tr6_2.Control = pnl_overlayRefY;

            TableRow olry_tr0 = new TableRow();
            groupBox_layer_overlayYReference_content.Rows.Add(olry_tr0);
            TableRow olry_tr1 = new TableRow();
            groupBox_layer_overlayYReference_content.Rows.Add(olry_tr1);

            rB_layer_OLRY_exp[0] = new RadioButton();
            rB_layer_OLRY_exp[0].Text = "0";
            rB_layer_OLRY_exp[0].Checked = true;

            TableCell rB_layer_OLRY_0tc = new TableCell();
            rB_layer_OLRY_0tc.Control = rB_layer_OLRY_exp[0];

            groupBox_layer_overlayYReference_content.Rows[0].Cells.Add(rB_layer_OLRY_0tc);

            int rB_OLRY = 1;
            int olry_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                // Don't add a button for our current layer
                rB_layer_OLRY_exp[rB_OLRY] = new RadioButton(rB_layer_OLRY_exp[0]);
                rB_layer_OLRY_exp[rB_OLRY].Text = rB_OLRY.ToString();
                rB_layer_OLRY_exp[rB_OLRY].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_OLRY_exp[rB_OLRY];
                groupBox_layer_overlayYReference_content.Rows[olry_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_OLRY + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    olry_rowIndex++;
                }
                rB_OLRY++;
            }

            groupBox_layer_overlayYReference_Av_exp = new GroupBox();
            groupBox_layer_overlayYReference_Av_table = new TableLayout();
            groupBox_layer_overlayYReference_Av_exp.Text = "Y Overlay References";
            groupBox_layer_overlayYReference_Av_exp.Content = groupBox_layer_overlayYReference_Av_table;

            TableRow avolry_tr0 = new TableRow();
            groupBox_layer_overlayYReference_Av_table.Rows.Add(avolry_tr0);
            TableRow avolry_tr1 = new TableRow();
            groupBox_layer_overlayYReference_Av_table.Rows.Add(avolry_tr1);

            int avolry_rowIndex = 0;
            for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
            {
                // Don't add a button for our current layer
                cB_layer_OLRY_Av_exp[cb] = new CheckBox();
                cB_layer_OLRY_Av_exp[cb].Text = (cb + 1).ToString();

                TableCell tc0 = new TableCell();
                tc0.Control = cB_layer_OLRY_Av_exp[cb];
                groupBox_layer_overlayYReference_Av_table.Rows[avolry_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if ((cb + 1) == CentralProperties.maxLayersForMC / 2)
                {
                    avolry_rowIndex++;
                }
            }
            pnl_overlayRefY.Content = groupBox_layer_overlayYReference_exp;
        }

        // COLY
        void litho_row8(TableLayout groupBox_layer_lithography_table)
        {
            // Outer table, row 8
            TableRow lit_tr7 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_tr7);
            lit_tr7.Cells.Add(new TableCell());

            // Table layout within row 8
            TableLayout row7_tl = new TableLayout();
            lit_tr7.Cells[0].Control = row7_tl;
            row7_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell lit_tr7_0 = new TableCell();
            row7_tl.Rows[0].Cells.Add(lit_tr7_0);

            TableLayout lit_tr7_0_tl = new TableLayout();
            lit_tr7_0.Control = lit_tr7_0_tl;

            lit_tr7_0_tl.Rows.Add(new TableRow());

            lit_tr7_0_tl.Rows.Add(new TableRow());

            Panel p1 = new Panel();
            lit_tr7_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr7_0_tl.Rows[0].Cells[0].Control = p1;

            Panel p2 = new Panel();
            lit_tr7_0_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr7_0_tl.Rows[0].Cells[1].Control = p2;

            TableCell lit_tr7_1 = new TableCell();
            lit_tr7.Cells.Add(lit_tr7_1);

            TableLayout lit_tr7_1_tl = new TableLayout();
            lit_tr7_1.Control = lit_tr7_1_tl;

            lit_tr7_1_tl.Rows.Add(new TableRow());

            Panel p3 = new Panel();
            lit_tr7_1_tl.Rows[0].Cells.Add(new TableCell());
            lit_tr7_1_tl.Rows[0].Cells[0].Control = p3;

            TableCell lit_tr7_2 = new TableCell();
            lit_tr7.Cells.Add(lit_tr7_2);

            groupBox_layer_overlayYCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_overlayYCorrelation_content = new TableLayout();
            groupBox_layer_overlayYCorrelation_exp.Text = "Y Overlay Correlation";
            groupBox_layer_overlayYCorrelation_exp.Content = groupBox_layer_overlayYCorrelation_content;
            lit_tr7_2.Control = groupBox_layer_overlayYCorrelation_exp;

            TableRow olcy_tr0 = new TableRow();
            groupBox_layer_overlayYCorrelation_content.Rows.Add(olcy_tr0);
            TableRow olcy_tr1 = new TableRow();
            groupBox_layer_overlayYCorrelation_content.Rows.Add(olcy_tr1);

            rB_layer_COLY_exp[0] = new RadioButton();
            rB_layer_COLY_exp[0].Text = "0";
            rB_layer_COLY_exp[0].Checked = true;

            TableCell rB_layer_COLY_0tc = new TableCell();
            rB_layer_COLY_0tc.Control = rB_layer_COLY_exp[0];

            groupBox_layer_overlayYCorrelation_content.Rows[0].Cells.Add(rB_layer_COLY_0tc);

            int rB_COLY = 1;
            int coly_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layer_COLY_exp[rB_COLY] = new RadioButton(rB_layer_COLY_exp[0]);
                rB_layer_COLY_exp[rB_COLY].Text = rB_COLY.ToString();
                rB_layer_COLY_exp[rB_COLY].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_COLY_exp[rB_COLY];
                groupBox_layer_overlayYCorrelation_content.Rows[coly_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_COLY + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    coly_rowIndex++;
                }
                rB_COLY++;
            }
        }
    }
}