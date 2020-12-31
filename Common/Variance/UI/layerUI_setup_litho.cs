using Eto.Forms;

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
                    groupBox_layer_CDUCorrelation_exp, groupBox_layer_TipCDUCorrelation_exp,
                    groupBox_layer_LWRCorrelation_exp, groupBox_layer_LWR2Correlation_exp;

        RadioButton[] rB_layer_OLRX_exp, rB_layer_OLRY_exp, rB_layer_COLX_exp, rB_layer_COLY_exp, rB_layer_CCDU_exp, rB_layer_CTCDU_exp, rB_layer_CLWR_exp, rB_layer_CLWR2_exp;
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

                litho_icr_ocr(groupBox_layer_lithography_table);
                litho_lwr(groupBox_layer_lithography_table);
                litho_lwr2(groupBox_layer_lithography_table);
                litho_scdu(groupBox_layer_lithography_table);
                litho_tcdu(groupBox_layer_lithography_table);
                litho_xol(groupBox_layer_lithography_table);
                litho_yol(groupBox_layer_lithography_table);
            });
        }

        // ICR
        void litho_icr_ocr(TableLayout groupBox_layer_lithography_table)
        {
            Panel outer = new Panel();
            TableLayout outer_tl = new TableLayout();
            TableLayout tl_leftCol = new TableLayout();
            tl_leftCol.Rows.Add(new TableRow());
            TableLayout tl_rightCol = new TableLayout();
            tl_rightCol.Rows.Add(new TableRow());
            outer_tl.Rows.Add(new TableRow());
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = tl_leftCol });
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = tl_rightCol });
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = null });
            outer.Content = outer_tl;

            // Outer table, row 1
            groupBox_layer_lithography_table.Rows.Add(new TableRow());
            groupBox_layer_lithography_table.Rows[groupBox_layer_lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer });

            // Table layout within cell.
            TableCell leftCol_tc = new TableCell();
            tl_leftCol.Rows[0].Cells.Add(leftCol_tc);

            TableCell rightCol_tc = new TableCell();
            tl_rightCol.Rows[0].Cells.Add(rightCol_tc);

            GroupBox groupBox_rounding = new GroupBox();
            groupBox_rounding.Text = "Rounding";
            leftCol_tc.Control = groupBox_rounding;

            GroupBox groupBox_misc = new GroupBox();
            groupBox_misc.Text = "Misc";
            rightCol_tc.Control = groupBox_misc;

            TableLayout rounding_table = new TableLayout();
            groupBox_rounding.Content = rounding_table;

            TableLayout litMisc_table = new TableLayout();
            groupBox_misc.Content = litMisc_table;

            rounding_table.Rows.Add(new TableRow());
            litMisc_table.Rows.Add(new TableRow());

            lbl_layer_lithoICRR_exp = new Label();
            lbl_layer_lithoICRR_exp.Text = "Inner Radius";
            lbl_layer_lithoICRR_exp.ToolTip = "Inner vertex (concave) corner rounding radius.";
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = lbl_layer_lithoICRR_exp;

            num_layer_lithoICRR_exp = new NumericStepper();
            num_layer_lithoICRR_exp.Increment = 0.1;
            num_layer_lithoICRR_exp.DecimalPlaces = 2;
            num_layer_lithoICRR_exp.MinValue = 0;
            num_layer_lithoICRR_exp.ToolTip = "Inner vertex (concave) corner rounding radius.";
            setSize(num_layer_lithoICRR_exp, 55, (int)(label_Height * uiScaleFactor));
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = TableLayout.AutoSized(num_layer_lithoICRR_exp);

            lbl_layer_lithoICV_exp = new Label();
            lbl_layer_lithoICV_exp.Text = "Var";
            lbl_layer_lithoICV_exp.MouseDoubleClick += ICV_RNG;
            lbl_layer_lithoICV_exp.ToolTip = "3-sigma inner vertex (concave) corner rounding radius variation.";
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = lbl_layer_lithoICV_exp;

            num_layer_lithoICV_exp = new NumericStepper();
            num_layer_lithoICV_exp.Increment = 0.1;
            num_layer_lithoICV_exp.DecimalPlaces = 2;
            num_layer_lithoICV_exp.MinValue = 0;
            num_layer_lithoICV_exp.ToolTip = "3-sigma inner vertex (concave) corner rounding radius variation.";
            setSize(num_layer_lithoICV_exp, 55, (int)(label_Height * uiScaleFactor));
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = TableLayout.AutoSized(num_layer_lithoICV_exp);

            rounding_table.Rows.Add(new TableRow());

            lbl_layer_lithoOCRR_exp = new Label();
            lbl_layer_lithoOCRR_exp.Text = "Outer Radius";
            lbl_layer_lithoOCRR_exp.ToolTip = "Outer vertex (concave) corner rounding radius.";
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = lbl_layer_lithoOCRR_exp;

            num_layer_lithoOCRR_exp = new NumericStepper();
            num_layer_lithoOCRR_exp.Increment = 0.1;
            num_layer_lithoOCRR_exp.DecimalPlaces = 2;
            num_layer_lithoOCRR_exp.MinValue = 0;
            num_layer_lithoOCRR_exp.ToolTip = "Outer vertex (concave) corner rounding radius.";
            setSize(num_layer_lithoOCRR_exp, 55, (int)(label_Height * uiScaleFactor));
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = TableLayout.AutoSized(num_layer_lithoOCRR_exp);

            lbl_layer_lithoOCV_exp = new Label();
            lbl_layer_lithoOCV_exp.Text = "Var";
            lbl_layer_lithoOCV_exp.MouseDoubleClick += ICV_RNG;
            lbl_layer_lithoOCV_exp.ToolTip = "3-sigma outer vertex (concave) corner rounding radius variation.";
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = lbl_layer_lithoOCV_exp;

            num_layer_lithoOCV_exp = new NumericStepper();
            num_layer_lithoOCV_exp.Increment = 0.1;
            num_layer_lithoOCV_exp.DecimalPlaces = 2;
            num_layer_lithoOCV_exp.MinValue = 0;
            num_layer_lithoOCV_exp.ToolTip = "3-sigma outer vertex (concave) corner rounding radius variation.";
            setSize(num_layer_lithoOCV_exp, 55, (int)(label_Height * uiScaleFactor));
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = TableLayout.AutoSized(num_layer_lithoOCV_exp);


            lbl_layer_lithoWobble_exp = new Label();
            lbl_layer_lithoWobble_exp.Text = "Wobble";
            lbl_layer_lithoWobble_exp.MouseDoubleClick += wobble_RNG;
            lbl_layer_lithoWobble_exp.ToolTip = "3-sigma rotational variation.";
            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layer_lithoWobble_exp });

            num_layer_lithoWobble_exp = new NumericStepper();
            num_layer_lithoWobble_exp.Increment = 0.1;
            num_layer_lithoWobble_exp.DecimalPlaces = 2;
            num_layer_lithoWobble_exp.MinValue = 0;
            num_layer_lithoWobble_exp.ToolTip = "3-sigma rotational variation.";
            setSize(num_layer_lithoWobble_exp, 55, (int)(label_Height * uiScaleFactor));
            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoWobble_exp) });

            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });

            litMisc_table.Rows.Add(new TableRow());

            lbl_layer_coeff1_exp = new Label();
            lbl_layer_coeff1_exp.Text = "Lens k1";
            lbl_layer_coeff1_exp.ToolTip = "Lens k1";
            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layer_coeff1_exp });

            num_layer_coeff1_exp = new NumericStepper();
            num_layer_coeff1_exp.Increment = 0.1;
            num_layer_coeff1_exp.DecimalPlaces = 2;
            num_layer_coeff1_exp.Value = 0.0;
            num_layer_coeff1_exp.ToolTip = "Lens k1";
            setSize(num_layer_coeff1_exp, 55, num_Height);
            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_coeff1_exp) });


            lbl_layer_coeff2_exp = new Label();
            lbl_layer_coeff2_exp.Text = "Lens k2";
            lbl_layer_coeff2_exp.ToolTip = "Lens k2";
            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_layer_coeff2_exp });

            num_layer_coeff2_exp = new NumericStepper();
            num_layer_coeff2_exp.Increment = 0.1;
            num_layer_coeff2_exp.DecimalPlaces = 2;
            num_layer_coeff2_exp.Value = 0.0;
            num_layer_coeff2_exp.ToolTip = "Lens k2";
            setSize(num_layer_coeff2_exp, 55, num_Height);
            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_coeff2_exp) });

        }

        void litho_lwr(TableLayout groupBox_layer_lithography_table)
        {
            TableRow lit_lwr = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(lit_lwr);

            GroupBox outer_gb = new GroupBox();
            outer_gb.Text = "LWR";

            Panel outer = new Panel();
            outer_gb.Content = outer;

            TableLayout tl = new TableLayout();
            tl.Rows.Add(new TableRow());

            outer.Content = tl;

            lit_lwr.Cells.Add(new TableCell() { Control = outer_gb });

            Panel p1 = new Panel();
            TableLayout p1tl = new TableLayout();
            p1tl.Rows.Add(new TableRow());
            p1.Content = p1tl;
            Panel p2 = new Panel();
            TableLayout p2tl = new TableLayout();
            p2tl.Rows.Add(new TableRow());
            p2.Content = p2tl;

            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = p1 });
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = p2 });

            lbl_layer_lithoLWR_exp = new Label();
            lbl_layer_lithoLWR_exp.Text = "3-sigma";
            lbl_layer_lithoLWR_exp.MouseDoubleClick += lwr_RNG;
            lbl_layer_lithoLWR_exp.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            p1tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoLWR_exp });

            num_layer_lithoLWR_exp = new NumericStepper();
            num_layer_lithoLWR_exp.Increment = 0.1;
            num_layer_lithoLWR_exp.DecimalPlaces = 2;
            num_layer_lithoLWR_exp.MinValue = 0;
            num_layer_lithoLWR_exp.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            setSize(num_layer_lithoLWR_exp, 55, (int)(label_Height * uiScaleFactor));
            p1tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoLWR_exp) });

            lbl_layer_lithoLWRFreq_exp = new Label();
            lbl_layer_lithoLWRFreq_exp.Text = "Freq";
            lbl_layer_lithoLWRFreq_exp.ToolTip = "Frequency of LWR";
            p1tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoLWRFreq_exp });

            num_layer_lithoLWRFreq_exp = new NumericStepper();
            num_layer_lithoLWRFreq_exp.Increment = 0.1;
            num_layer_lithoLWRFreq_exp.DecimalPlaces = 2;
            num_layer_lithoLWRFreq_exp.Value = 0.1;
            num_layer_lithoLWRFreq_exp.MinValue = 0.01;
            num_layer_lithoLWRFreq_exp.ToolTip = "Frequency of LWR";
            setSize(num_layer_lithoLWRFreq_exp, 55, num_Height);
            p1tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoLWRFreq_exp) });

            comboBox_layerLWRNoiseType_exp = new DropDown();
            comboBox_layerLWRNoiseType_exp.DataContext = DataContext;
            comboBox_layerLWRNoiseType_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.noiseTypeList);
            comboBox_layerLWRNoiseType_exp.SelectedIndex = 0;
            comboBox_layerLWRNoiseType_exp.ToolTip = "Noise type for LWR";
            p1tl.Rows.Add(new TableRow());
            p1tl.Rows[1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerLWRNoiseType_exp) });

            TableLayout ptl = new TableLayout();
            p2.Content = ptl;
            ptl.Rows.Add(new TableRow());
            TableCell ptc = new TableCell();
            ptl.Rows[0].Cells.Add(ptc);

            groupBox_layer_LWRCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_LWRCorrelation_content = new TableLayout();
            groupBox_layer_LWRCorrelation_exp.Text = "LWR Seed Correlation";
            groupBox_layer_LWRCorrelation_exp.Content = groupBox_layer_LWRCorrelation_content;
            ptc.Control = groupBox_layer_LWRCorrelation_exp;

            TableRow clwr_tr0 = new TableRow();
            groupBox_layer_LWRCorrelation_content.Rows.Add(clwr_tr0);
            TableRow clwr_tr1 = new TableRow();
            groupBox_layer_LWRCorrelation_content.Rows.Add(clwr_tr1);

            rB_layer_CLWR_exp[0] = new RadioButton();
            rB_layer_CLWR_exp[0].Text = "0";
            rB_layer_CLWR_exp[0].Checked = true;

            TableCell rB_layer_CLWR_0tc = new TableCell();
            rB_layer_CLWR_0tc.Control = rB_layer_CLWR_exp[0];

            groupBox_layer_LWRCorrelation_content.Rows[0].Cells.Add(rB_layer_CLWR_0tc);

            int rB_CLWR = 1;
            int clwr_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layer_CLWR_exp[rB_CLWR] = new RadioButton(rB_layer_CLWR_exp[0]);
                rB_layer_CLWR_exp[rB_CLWR].Text = rB_CLWR.ToString();
                rB_layer_CLWR_exp[rB_CLWR].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_CLWR_exp[rB_CLWR];
                groupBox_layer_LWRCorrelation_content.Rows[clwr_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_CLWR + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    clwr_rowIndex++;
                }
                rB_CLWR++;
            }
        }

        void litho_lwr2(TableLayout groupBox_layer_lithography_table)
        {
            TableRow tr0 = new TableRow();
            groupBox_layer_lithography_table.Rows.Add(tr0);

            GroupBox outer_gb = new GroupBox();
            outer_gb.Text = "LWR2";

            Panel outer = new Panel();
            outer_gb.Content = outer;

            TableLayout tl = new TableLayout();
            tl.Rows.Add(new TableRow());

            outer.Content = tl;

            tr0.Cells.Add(new TableCell() { Control = outer_gb });

            Panel p1 = new Panel();
            TableLayout p1tl = new TableLayout();
            p1tl.Rows.Add(new TableRow());
            p1.Content = p1tl;
            Panel p2 = new Panel();
            TableLayout p2tl = new TableLayout();
            p2tl.Rows.Add(new TableRow());
            p2.Content = p2tl;

            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = p1 });
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = p2 });
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            lbl_layer_lithoLWR2_exp = new Label();
            lbl_layer_lithoLWR2_exp.Text = "3-sigma";
            lbl_layer_lithoLWR2_exp.MouseDoubleClick += lwr_RNG;
            lbl_layer_lithoLWR2_exp.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            p1tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoLWR2_exp });

            num_layer_lithoLWR2_exp = new NumericStepper();
            num_layer_lithoLWR2_exp.Increment = 0.1;
            num_layer_lithoLWR2_exp.DecimalPlaces = 2;
            num_layer_lithoLWR2_exp.MinValue = 0;
            num_layer_lithoLWR2_exp.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            setSize(num_layer_lithoLWR2_exp, 55, (int)(label_Height * uiScaleFactor));
            p1tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoLWR2_exp) });

            lbl_layer_lithoLWR2Freq_exp = new Label();
            lbl_layer_lithoLWR2Freq_exp.Text = "Freq";
            lbl_layer_lithoLWR2Freq_exp.ToolTip = "Frequency of LWR2";
            p1tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_lithoLWR2Freq_exp });

            num_layer_lithoLWR2Freq_exp = new NumericStepper();
            num_layer_lithoLWR2Freq_exp.Increment = 0.1;
            num_layer_lithoLWR2Freq_exp.DecimalPlaces = 2;
            num_layer_lithoLWR2Freq_exp.Value = 0.1;
            num_layer_lithoLWR2Freq_exp.MinValue = 0.01;
            num_layer_lithoLWR2Freq_exp.ToolTip = "Frequency of LWR2";
            setSize(num_layer_lithoLWR2Freq_exp, 55, num_Height);
            p1tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_lithoLWR2Freq_exp) });

            comboBox_layerLWR2NoiseType_exp = new DropDown();
            comboBox_layerLWR2NoiseType_exp.DataContext = DataContext;
            comboBox_layerLWR2NoiseType_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.noiseTypeList);
            comboBox_layerLWR2NoiseType_exp.SelectedIndex = 0;
            comboBox_layerLWR2NoiseType_exp.ToolTip = "Noise type for LWR2";
            p1tl.Rows.Add(new TableRow());
            p1tl.Rows[1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_layerLWR2NoiseType_exp) });

            TableLayout ptl = new TableLayout();
            p2.Content = ptl;
            ptl.Rows.Add(new TableRow());
            TableCell ptc = new TableCell();
            ptl.Rows[0].Cells.Add(ptc);

            groupBox_layer_LWR2Correlation_exp = new GroupBox();
            TableLayout groupBox_layer_LWR2Correlation_content = new TableLayout();
            groupBox_layer_LWR2Correlation_exp.Text = "LWR2 Seed Correlation";
            groupBox_layer_LWR2Correlation_exp.Content = groupBox_layer_LWR2Correlation_content;
            ptc.Control = groupBox_layer_LWR2Correlation_exp;

            TableRow clwr2_tr0 = new TableRow();
            groupBox_layer_LWR2Correlation_content.Rows.Add(clwr2_tr0);
            TableRow clwr2_tr1 = new TableRow();
            groupBox_layer_LWR2Correlation_content.Rows.Add(clwr2_tr1);

            rB_layer_CLWR2_exp[0] = new RadioButton();
            rB_layer_CLWR2_exp[0].Text = "0";
            rB_layer_CLWR2_exp[0].Checked = true;

            TableCell rB_layer_CLWR2_0tc = new TableCell();
            rB_layer_CLWR2_0tc.Control = rB_layer_CLWR2_exp[0];

            groupBox_layer_LWR2Correlation_content.Rows[0].Cells.Add(rB_layer_CLWR2_0tc);

            int rB_CLWR2 = 1;
            int clwr2_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_layer_CLWR2_exp[rB_CLWR2] = new RadioButton(rB_layer_CLWR2_exp[0]);
                rB_layer_CLWR2_exp[rB_CLWR2].Text = rB_CLWR2.ToString();
                rB_layer_CLWR2_exp[rB_CLWR2].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_CLWR2_exp[rB_CLWR2];
                groupBox_layer_LWR2Correlation_content.Rows[clwr2_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_CLWR2 + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    clwr2_rowIndex++;
                }
                rB_CLWR2++;
            }
        }

        // SCDU
        void litho_scdu(TableLayout groupBox_layer_lithography_table)
        {
            GroupBox outer_gb = new GroupBox();
            outer_gb.Text = "Side CDU";

            Panel outer = new Panel();
            outer_gb.Content = outer;
            TableLayout outer_tl = new TableLayout();
            TableLayout tc_tl = new TableLayout();
            outer_tl.Rows.Add(new TableRow());
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = tc_tl });
            outer.Content = outer_tl;
            tc_tl.Rows.Add(new TableRow());

            // Outer table, row 1
            groupBox_layer_lithography_table.Rows.Add(new TableRow());
            groupBox_layer_lithography_table.Rows[groupBox_layer_lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer_gb });

            // Table layout within cell.
            TableCell tc_0 = new TableCell();
            tc_tl.Rows[0].Cells.Add(tc_0);

            TableLayout tc_0_tl = new TableLayout();
            tc_0.Control = tc_0_tl;

            tc_0_tl.Rows.Add(new TableRow());

            lbl_layer_lithoCDUSide_exp = new Label();
            lbl_layer_lithoCDUSide_exp.Text = "3-sigma";
            lbl_layer_lithoCDUSide_exp.MouseDoubleClick += sCDU_RNG;
            lbl_layer_lithoCDUSide_exp.ToolTip = "3-sigma CD variation applied to non-tip sides.";
            tc_0_tl.Rows[0].Cells.Add(new TableCell());
            tc_0_tl.Rows[0].Cells[0].Control = lbl_layer_lithoCDUSide_exp;
            tc_0_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoCDUSide_exp = new NumericStepper();
            num_layer_lithoCDUSide_exp.Increment = 0.1;
            num_layer_lithoCDUSide_exp.DecimalPlaces = 2;
            num_layer_lithoCDUSide_exp.MinValue = 0;
            num_layer_lithoCDUSide_exp.ToolTip = "3-sigma CD variation applied to non-tip sides.";
            setSize(num_layer_lithoCDUSide_exp, 55, num_Height);
            tc_0_tl.Rows[0].Cells.Add(new TableCell());
            tc_0_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoCDUSide_exp);

            tc_tl.Rows[0].Cells.Add(new TableCell() { ScaleWidth = true });

            TableCell tc_1 = new TableCell();
            tc_tl.Rows[0].Cells.Add(tc_1);

            Panel p = new Panel();
            tc_1.Control = p;

            TableLayout ptl = new TableLayout();
            p.Content = ptl;
            ptl.Rows.Add(new TableRow());
            TableCell ptc = new TableCell();
            ptl.Rows[0].Cells.Add(ptc);

            groupBox_layer_CDUCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_CDUCorrelation_content = new TableLayout();
            groupBox_layer_CDUCorrelation_exp.Text = "Correlation";
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
        }

        // TCDU
        void litho_tcdu(TableLayout groupBox_layer_lithography_table)
        {
            GroupBox outer_gb = new GroupBox();
            outer_gb.Text = "Tips CDU";
            Panel outer = new Panel();
            outer_gb.Content = outer;
            TableLayout outer_tl = new TableLayout();
            TableLayout tc_tl = new TableLayout();
            outer_tl.Rows.Add(new TableRow());
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = tc_tl });
            outer.Content = outer_tl;
            tc_tl.Rows.Add(new TableRow());

            // Outer table, row 1
            groupBox_layer_lithography_table.Rows.Add(new TableRow());
            groupBox_layer_lithography_table.Rows[groupBox_layer_lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer_gb });

            // Table layout within cell.
            TableCell tc_0 = new TableCell();
            tc_tl.Rows[0].Cells.Add(tc_0);

            TableLayout tc_0_tl = new TableLayout();
            tc_0.Control = tc_0_tl;

            tc_0_tl.Rows.Add(new TableRow());

            lbl_layer_lithoCDUTips_exp = new Label();
            lbl_layer_lithoCDUTips_exp.Text = "3-sigma";
            lbl_layer_lithoCDUTips_exp.MouseDoubleClick += tCDU_RNG;
            lbl_layer_lithoCDUTips_exp.ToolTip = "3-sigma CD variation applied to tip sides.";
            tc_0_tl.Rows[0].Cells.Add(new TableCell());
            tc_0_tl.Rows[0].Cells[0].Control = lbl_layer_lithoCDUTips_exp;
            tc_0_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoCDUTips_exp = new NumericStepper();
            num_layer_lithoCDUTips_exp.Increment = 0.1;
            num_layer_lithoCDUTips_exp.DecimalPlaces = 2;
            num_layer_lithoCDUTips_exp.MinValue = 0;
            num_layer_lithoCDUTips_exp.ToolTip = "3-sigma CD variation applied to tip sides.";
            setSize(num_layer_lithoCDUTips_exp, 55, num_Height);
            tc_0_tl.Rows[0].Cells.Add(new TableCell());
            tc_0_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoCDUTips_exp);

            tc_tl.Rows[tc_tl.Rows.Count - 1].Cells.Add(new TableCell());

            TableCell tc_1 = new TableCell();
            tc_tl.Rows[tc_tl.Rows.Count - 1].Cells.Add(tc_1);

            Panel p = new Panel();
            tc_1.Control = p;

            TableLayout ptl = new TableLayout();
            p.Content = ptl;
            ptl.Rows.Add(new TableRow());
            ptl.Rows[0].Cells.Add(new TableCell() { ScaleWidth = true });
            TableCell ptc = new TableCell();
            ptl.Rows[0].Cells.Add(ptc);

            groupBox_layer_TipCDUCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_TipCDUCorrelation_content = new TableLayout();
            groupBox_layer_TipCDUCorrelation_exp.Text = "Correlation";
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

        // XOL, XOLR, XCOL
        void litho_xol(TableLayout groupBox_layer_lithography_table)
        {
            GroupBox outer_gb = new GroupBox();
            outer_gb.Text = "Horizontal Overlay";
            Panel outer = new Panel();
            outer_gb.Content = outer;
            TableLayout outer_tl = new TableLayout();
            outer_tl.Rows.Add(new TableRow());

            Panel left = new Panel();
            TableLayout left_tl = new TableLayout();
            left_tl.Rows.Add(new TableRow());
            Panel mid = new Panel();
            TableLayout mid_tl = new TableLayout();
            mid_tl.Rows.Add(new TableRow());
            Panel right = new Panel();
            TableLayout right_tl = new TableLayout();
            right_tl.Rows.Add(new TableRow());

            left.Content = left_tl;
            mid.Content = mid_tl;
            right.Content = right_tl;

            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = left });
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = mid, ScaleWidth = true });
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = right });
            outer.Content = outer_tl;

            // Outer table, row 1
            groupBox_layer_lithography_table.Rows.Add(new TableRow());
            groupBox_layer_lithography_table.Rows[groupBox_layer_lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer_gb });

            // Table layout within cell.
            TableCell sigma_tc = new TableCell();
            left_tl.Rows[0].Cells.Add(sigma_tc);

            TableLayout sigma_tl = new TableLayout();
            sigma_tc.Control = sigma_tl;

            sigma_tl.Rows.Add(new TableRow());

            lbl_layer_lithoHorOverlay_exp = new Label();
            lbl_layer_lithoHorOverlay_exp.Text = "3-sigma";
            lbl_layer_lithoHorOverlay_exp.MouseDoubleClick += hOverlay_RNG;
            lbl_layer_lithoHorOverlay_exp.ToolTip = "3-sigma horizontal overlay.";
            sigma_tl.Rows[0].Cells.Add(new TableCell());
            sigma_tl.Rows[0].Cells[0].Control = lbl_layer_lithoHorOverlay_exp;
            sigma_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoHorOverlay_exp = new NumericStepper();
            num_layer_lithoHorOverlay_exp.Increment = 0.1;
            num_layer_lithoHorOverlay_exp.DecimalPlaces = 2;
            num_layer_lithoHorOverlay_exp.MinValue = 0;
            num_layer_lithoHorOverlay_exp.ToolTip = "3-sigma horizontal overlay.";
            setSize(num_layer_lithoHorOverlay_exp, 55, num_Height);
            sigma_tl.Rows[0].Cells.Add(new TableCell());
            sigma_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoHorOverlay_exp);

            sigma_tl.Rows[sigma_tl.Rows.Count - 1].Cells.Add(new TableCell() { ScaleWidth = true });

            TableCell check_tc = new TableCell();
            mid_tl.Rows[mid_tl.Rows.Count - 1].Cells.Add(check_tc);

            TableLayout check_tl = new TableLayout();
            check_tc.Control = check_tl;

            check_tl.Rows.Add(new TableRow());
            check_tl.Rows[0].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            checkBox_layer_overlayXReference_Av_exp = new CheckBox();
            checkBox_layer_overlayXReference_Av_exp.Text = "Av";
            checkBox_layer_overlayXReference_Av_exp.ToolTip = "Use the average displaced location of layers as a reference for horizontal overlay";
            check_tl.Rows[0].Cells.Add(new TableCell());
            check_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(checkBox_layer_overlayXReference_Av_exp);

            TableCell right_tc = new TableCell();
            right_tl.Rows[right_tl.Rows.Count - 1].Cells.Add(right_tc);

            groupBox_layer_overlayXReference_exp = new GroupBox();
            TableLayout groupBox_layer_overlayXReference_content = new TableLayout();
            groupBox_layer_overlayXReference_exp.Text = "X Overlay Reference";
            groupBox_layer_overlayXReference_exp.Content = groupBox_layer_overlayXReference_content;
            pnl_overlayRefX = new Panel();
            right_tc.Control = pnl_overlayRefX;

            TableRow olrx_tr0 = new TableRow();
            groupBox_layer_overlayXReference_content.Rows.Add(olrx_tr0);
            TableRow olrx_tr1 = new TableRow();
            groupBox_layer_overlayXReference_content.Rows.Add(olrx_tr1);

            rB_layer_OLRX_exp[0] = new RadioButton();
            rB_layer_OLRX_exp[0].Text = "0";
            rB_layer_OLRX_exp[0].Checked = true;

            TableCell rB_layer_OLRX_0tc = new TableCell();
            rB_layer_OLRX_0tc.Control = rB_layer_OLRX_exp[0];

            groupBox_layer_overlayXReference_content.Rows[0].Cells.Add(rB_layer_OLRX_0tc);

            int rB_OLRX = 1;
            int olrx_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                // Don't add a button for our current layer
                rB_layer_OLRX_exp[rB_OLRX] = new RadioButton(rB_layer_OLRX_exp[0]);
                rB_layer_OLRX_exp[rB_OLRX].Text = rB_OLRX.ToString();
                rB_layer_OLRX_exp[rB_OLRX].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_layer_OLRX_exp[rB_OLRX];
                groupBox_layer_overlayXReference_content.Rows[olrx_rowIndex].Cells.Add(tc0);
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
                if ((cb + 1) == CentralProperties.maxLayersForMC / 2)
                {
                    avolrx_rowIndex++;
                }
            }
            pnl_overlayRefX.Content = groupBox_layer_overlayXReference_exp;

            left_tl.Rows.Add(new TableRow());
            mid_tl.Rows.Add(new TableRow());
            right_tl.Rows.Add(new TableRow());

            left_tl.Rows[left_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });
            mid_tl.Rows[mid_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });


            TableCell right_tc2 = new TableCell();
            right_tl.Rows[right_tl.Rows.Count - 1].Cells.Add(right_tc2);


            groupBox_layer_overlayXCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_overlayXCorrelation_content = new TableLayout();
            groupBox_layer_overlayXCorrelation_exp.Text = "X Overlay Correlation";
            groupBox_layer_overlayXCorrelation_exp.Content = groupBox_layer_overlayXCorrelation_content;
            right_tc2.Control = groupBox_layer_overlayXCorrelation_exp;

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

        // YOL, YOLR, YCOL
        void litho_yol(TableLayout groupBox_layer_lithography_table)
        {
            GroupBox outer_gb = new GroupBox();
            outer_gb.Text = "Vertical Overlay";
            Panel outer = new Panel();
            outer_gb.Content = outer;
            TableLayout outer_tl = new TableLayout();
            outer_tl.Rows.Add(new TableRow());

            Panel left = new Panel();
            TableLayout left_tl = new TableLayout();
            left_tl.Rows.Add(new TableRow());
            Panel mid = new Panel();
            TableLayout mid_tl = new TableLayout();
            mid_tl.Rows.Add(new TableRow());
            Panel right = new Panel();
            TableLayout right_tl = new TableLayout();
            right_tl.Rows.Add(new TableRow());

            left.Content = left_tl;
            mid.Content = mid_tl;
            right.Content = right_tl;

            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = left });
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = mid, ScaleWidth = true });
            outer_tl.Rows[0].Cells.Add(new TableCell() { Control = right });
            outer.Content = outer_tl;

            // Outer table, row 1
            groupBox_layer_lithography_table.Rows.Add(new TableRow());
            groupBox_layer_lithography_table.Rows[groupBox_layer_lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer_gb });

            // Table layout within cell.
            TableCell sigma_tc = new TableCell();
            left_tl.Rows[0].Cells.Add(sigma_tc);

            TableLayout sigma_tl = new TableLayout();
            sigma_tc.Control = sigma_tl;

            sigma_tl.Rows.Add(new TableRow());

            lbl_layer_lithoVerOverlay_exp = new Label();
            lbl_layer_lithoVerOverlay_exp.Text = "3-sigma";
            lbl_layer_lithoVerOverlay_exp.MouseDoubleClick += vOverlay_RNG;
            lbl_layer_lithoVerOverlay_exp.ToolTip = "3-sigma verizontal overlay.";
            sigma_tl.Rows[0].Cells.Add(new TableCell());
            sigma_tl.Rows[0].Cells[0].Control = lbl_layer_lithoVerOverlay_exp;
            sigma_tl.Rows[0].Cells[0].ScaleWidth = true;

            num_layer_lithoVerOverlay_exp = new NumericStepper();
            num_layer_lithoVerOverlay_exp.Increment = 0.1;
            num_layer_lithoVerOverlay_exp.DecimalPlaces = 2;
            num_layer_lithoVerOverlay_exp.MinValue = 0;
            num_layer_lithoVerOverlay_exp.ToolTip = "3-sigma vertical overlay.";
            setSize(num_layer_lithoVerOverlay_exp, 55, num_Height);
            sigma_tl.Rows[0].Cells.Add(new TableCell());
            sigma_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(num_layer_lithoVerOverlay_exp);

            sigma_tl.Rows[sigma_tl.Rows.Count - 1].Cells.Add(new TableCell() { ScaleWidth = true });

            TableCell check_tc = new TableCell();
            mid_tl.Rows[mid_tl.Rows.Count - 1].Cells.Add(check_tc);

            TableLayout check_tl = new TableLayout();
            check_tc.Control = check_tl;

            check_tl.Rows.Add(new TableRow());
            check_tl.Rows[0].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            checkBox_layer_overlayYReference_Av_exp = new CheckBox();
            checkBox_layer_overlayYReference_Av_exp.Text = "Av";
            checkBox_layer_overlayYReference_Av_exp.ToolTip = "Use the average displaced location of layers as a reference for vertical overlay";
            check_tl.Rows[0].Cells.Add(new TableCell());
            check_tl.Rows[0].Cells[1].Control = TableLayout.AutoSized(checkBox_layer_overlayYReference_Av_exp);

            TableCell right_tc = new TableCell();
            right_tl.Rows[right_tl.Rows.Count - 1].Cells.Add(right_tc);

            groupBox_layer_overlayYReference_exp = new GroupBox();
            TableLayout groupBox_layer_overlayYReference_content = new TableLayout();
            groupBox_layer_overlayYReference_exp.Text = "Y Overlay Reference";
            groupBox_layer_overlayYReference_exp.Content = groupBox_layer_overlayYReference_content;
            pnl_overlayRefY = new Panel();
            right_tc.Control = pnl_overlayRefY;

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

            left_tl.Rows.Add(new TableRow());
            mid_tl.Rows.Add(new TableRow());
            right_tl.Rows.Add(new TableRow());

            left_tl.Rows[left_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });
            mid_tl.Rows[mid_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });


            TableCell right_tc2 = new TableCell();
            right_tl.Rows[right_tl.Rows.Count - 1].Cells.Add(right_tc2);


            groupBox_layer_overlayYCorrelation_exp = new GroupBox();
            TableLayout groupBox_layer_overlayYCorrelation_content = new TableLayout();
            groupBox_layer_overlayYCorrelation_exp.Text = "Y Overlay Correlation";
            groupBox_layer_overlayYCorrelation_exp.Content = groupBox_layer_overlayYCorrelation_content;
            right_tc2.Control = groupBox_layer_overlayYCorrelation_exp;

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
