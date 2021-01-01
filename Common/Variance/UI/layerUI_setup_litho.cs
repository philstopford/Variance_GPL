using Eto.Forms;

namespace Variance
{
    public partial class MainForm : Form
    {
        // 2D Layer Litho
        NumericStepper num_lithoICRR, num_lithoOCRR, num_lithoCDUSide, num_lithoHorOverlay, num_lithoVerOverlay,
                        num_lithoICV, num_lithoOCV, num_lithoCDUTips, num_lithoLWR, num_lithoLWRFreq, num_lithoLWR2, num_lithoLWR2Freq, num_lithoWobble, num_coeff1, num_coeff2;
        Label lbl_lithoICRR, lbl_lithoOCRR, lbl_lithoCDUSide, lbl_lithoHorOverlay, lbl_lithoVerOverlay,
                lbl_lithoICV, lbl_lithoOCV, lbl_lithoCDUTips, lbl_lithoLWR, lbl_lithoLWRFreq, lbl_lithoLWR2, lbl_lithoLWR2Freq, lbl_lithoWobble, lbl_coeff1, lbl_coeff2;
        GroupBox gB_overlayYReference, gB_overlayXReference,
                    gB_overlayYReference_Av, gB_overlayXReference_Av,
                    gB_overlayYCorrelation, gB_overlayXCorrelation,
                    gB_CDUCorrelation, gB_TipCDUCorrelation,
                    gB_LWRCorrelation, gB_LWR2Correlation;

        RadioButton[] rB_OLRX, rB_OLRY_exp, rB_COLX, rB_COLY, rB_CCDU, rB_CTCDU, rB_CLWR, rB_CLWR2;
        CheckBox cB_overlayXReference_Av, cB_overlayYReference_Av, cB_layer_LWRPreview;

        CheckBox[] cB_OLRX_Av, cB_layer_OLRY_Av;

        DropDown comboBox_LWRNoiseType, comboBox_LWR2NoiseType;

        TableLayout overlayXReference_Av_table, overlayYReference_Av_table;

        Panel pnl_overlayRefX, pnl_overlayRefY;

        void twoD_LayerUISetup_litho(TableCell tc)
        {
            Application.Instance.Invoke(() =>
            {
                gB_layer_lithography = new GroupBox();
                gB_layer_lithography.Text = "Lithography Parameters";
                TableLayout lithography_table = new TableLayout();
                gB_layer_lithography.Content = lithography_table;

                TableLayout t = new TableLayout();
                t.Rows.Add(new TableRow());
                t.Rows[t.Rows.Count - 1].Cells.Add(new TableCell() { Control = gB_layer_lithography });
                t.Rows[t.Rows.Count - 1].Cells.Add(new TableCell() { Control = new Panel(), ScaleWidth = true });

                Panel p = new Panel();
                p.Content = t;
                tc.Control = p;

                litho_icr_ocr(lithography_table);
                litho_lwr(lithography_table);
                litho_lwr2(lithography_table);
                litho_scdu(lithography_table);
                litho_tcdu(lithography_table);
                litho_xol(lithography_table);
                litho_yol(lithography_table);
            });
        }

        // ICR
        void litho_icr_ocr(TableLayout lithography_table)
        {
            Panel outer = new Panel();
            TableLayout outer_tl = new TableLayout();
            TableLayout tl_leftCol = new TableLayout();
            tl_leftCol.Rows.Add(new TableRow());
            TableLayout tl_rightCol = new TableLayout();
            tl_rightCol.Rows.Add(new TableRow());
            outer_tl.Rows.Add(new TableRow());
            outer_tl.Rows[outer_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = tl_leftCol });
            outer_tl.Rows[outer_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
            outer_tl.Rows[outer_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = tl_rightCol });
            outer.Content = outer_tl;

            // Outer table, row 1
            lithography_table.Rows.Add(new TableRow());
            lithography_table.Rows[lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer });

            // Table layout within cell.
            TableCell leftCol_tc = new TableCell();
            tl_leftCol.Rows[tl_leftCol.Rows.Count - 1].Cells.Add(leftCol_tc);

            TableCell rightCol_tc = new TableCell();
            tl_rightCol.Rows[tl_rightCol.Rows.Count - 1].Cells.Add(rightCol_tc);

            GroupBox groupBox_rounding = new GroupBox();
            groupBox_rounding.Text = "Rounding";
            leftCol_tc.Control = groupBox_rounding;

            /*
            ContextMenu cm_rounding = new ContextMenu();
            groupBox_rounding.ContextMenu = cm_rounding;
            int itemIndex = 0;
            litho_rounding_copy = new ButtonMenuItem() { Text = "Copy" };
            cm_rounding.Items.Add(litho_rounding_copy);
            cm_rounding.Items[itemIndex].Click += delegate
            {
                litho_rounding_copy();
            };
            itemIndex++;
            litho_rounding_paste = new ButtonMenuItem() { Text = "Paste" };
            cm_rounding.Items.Add(litho_rounding_paste);
            cm_rounding.Items[itemIndex].Click += delegate
            {
                litho_rounding_paste();
            };
            */

            GroupBox groupBox_misc = new GroupBox();
            groupBox_misc.Text = "Misc";
            rightCol_tc.Control = groupBox_misc;

            TableLayout rounding_table = new TableLayout();
            groupBox_rounding.Content = rounding_table;

            TableLayout litMisc_table = new TableLayout();
            groupBox_misc.Content = litMisc_table;

            rounding_table.Rows.Add(new TableRow());
            litMisc_table.Rows.Add(new TableRow());

            lbl_lithoICRR = new Label();
            lbl_lithoICRR.Text = "Inner Radius";
            lbl_lithoICRR.ToolTip = "Inner vertex (concave) corner rounding radius.";
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = lbl_lithoICRR;

            num_lithoICRR = new NumericStepper();
            num_lithoICRR.Increment = 0.1;
            num_lithoICRR.DecimalPlaces = 2;
            num_lithoICRR.MinValue = 0;
            num_lithoICRR.ToolTip = "Inner vertex (concave) corner rounding radius.";
            setSize(num_lithoICRR, 55, (int)(label_Height * uiScaleFactor));
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = TableLayout.AutoSized(num_lithoICRR);

            lbl_lithoICV = new Label();
            lbl_lithoICV.Text = "Var";
            lbl_lithoICV.MouseDoubleClick += ICV_RNG;
            lbl_lithoICV.ToolTip = "3-sigma inner vertex (concave) corner rounding radius variation.";
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = lbl_lithoICV;

            num_lithoICV = new NumericStepper();
            num_lithoICV.Increment = 0.1;
            num_lithoICV.DecimalPlaces = 2;
            num_lithoICV.MinValue = 0;
            num_lithoICV.ToolTip = "3-sigma inner vertex (concave) corner rounding radius variation.";
            setSize(num_lithoICV, 55, (int)(label_Height * uiScaleFactor));
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = TableLayout.AutoSized(num_lithoICV);

            rounding_table.Rows.Add(new TableRow());

            lbl_lithoOCRR = new Label();
            lbl_lithoOCRR.Text = "Outer Radius";
            lbl_lithoOCRR.ToolTip = "Outer vertex (concave) corner rounding radius.";
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = lbl_lithoOCRR;

            num_lithoOCRR = new NumericStepper();
            num_lithoOCRR.Increment = 0.1;
            num_lithoOCRR.DecimalPlaces = 2;
            num_lithoOCRR.MinValue = 0;
            num_lithoOCRR.ToolTip = "Outer vertex (concave) corner rounding radius.";
            setSize(num_lithoOCRR, 55, (int)(label_Height * uiScaleFactor));
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = TableLayout.AutoSized(num_lithoOCRR);

            lbl_lithoOCV = new Label();
            lbl_lithoOCV.Text = "Var";
            lbl_lithoOCV.MouseDoubleClick += ICV_RNG;
            lbl_lithoOCV.ToolTip = "3-sigma outer vertex (concave) corner rounding radius variation.";
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = lbl_lithoOCV;

            num_lithoOCV = new NumericStepper();
            num_lithoOCV.Increment = 0.1;
            num_lithoOCV.DecimalPlaces = 2;
            num_lithoOCV.MinValue = 0;
            num_lithoOCV.ToolTip = "3-sigma outer vertex (concave) corner rounding radius variation.";
            setSize(num_lithoOCV, 55, (int)(label_Height * uiScaleFactor));
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Add(new TableCell());
            rounding_table.Rows[rounding_table.Rows.Count - 1].Cells[rounding_table.Rows[rounding_table.Rows.Count - 1].Cells.Count - 1].Control = TableLayout.AutoSized(num_lithoOCV);


            TableLayout distortion_tl = new TableLayout();
            distortion_tl.Rows.Add(new TableRow());
            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(distortion_tl) });

            lbl_coeff1 = new Label();
            lbl_coeff1.Text = "Lens k1";
            lbl_coeff1.ToolTip = "Lens k1";
            distortion_tl.Rows[distortion_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_coeff1 });

            num_coeff1 = new NumericStepper();
            num_coeff1.Increment = 0.1;
            num_coeff1.DecimalPlaces = 2;
            num_coeff1.Value = 0.0;
            num_coeff1.ToolTip = "Lens k1";
            setSize(num_coeff1, 55, num_Height);
            distortion_tl.Rows[distortion_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_coeff1) });


            lbl_coeff2 = new Label();
            lbl_coeff2.Text = "Lens k2";
            lbl_coeff2.ToolTip = "Lens k2";
            distortion_tl.Rows[distortion_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_coeff2 });

            num_coeff2 = new NumericStepper();
            num_coeff2.Increment = 0.1;
            num_coeff2.DecimalPlaces = 2;
            num_coeff2.Value = 0.0;
            num_coeff2.ToolTip = "Lens k2";
            setSize(num_coeff2, 55, num_Height);
            distortion_tl.Rows[distortion_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_coeff2) });

            litMisc_table.Rows.Add(new TableRow());

            TableLayout edge_slide_tl = new TableLayout();
            edge_slide_tl.Rows.Add(new TableRow());
            litMisc_table.Rows[litMisc_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(edge_slide_tl) });

            cB_edgeSlide = new CheckBox();
            cB_edgeSlide.Text = "Edge Slide";
            cB_edgeSlide.Width = 90;
            cB_edgeSlide.ToolTip = "If checked, apply tension to each edge for the contour generation";
            edge_slide_tl.Rows[edge_slide_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(cB_edgeSlide) });
            if (EntropyLayerSettings.getDefaultInt(EntropyLayerSettings.properties_i.edgeSlide) == 1)
            {
                cB_edgeSlide.Checked = true;
            }

            lbl_edgeSlideTension = new Label();
            lbl_edgeSlideTension.Text = "Tension";
            lbl_edgeSlideTension.Width = 50;
            lbl_edgeSlideTension.ToolTip = "Amount of tension to apply, to pull the midpoint towards the longer edge";
            edge_slide_tl.Rows[edge_slide_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_edgeSlideTension });

            num_edgeSlideTension = new NumericStepper();
            num_edgeSlideTension.DecimalPlaces = 2;
            num_edgeSlideTension.Increment = 0.1;
            num_edgeSlideTension.Value = (double)EntropyLayerSettings.getDefaultDecimal(EntropyLayerSettings.properties_decimal.eTension);
            num_edgeSlideTension.MinValue = 0.01;
            setSize(num_edgeSlideTension, 55, num_Height);
            num_edgeSlideTension.ToolTip = "Amount of tension to apply, to pull the midpoint towards the longer edge";
            edge_slide_tl.Rows[edge_slide_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_edgeSlideTension) });
            num_edgeSlideTension.Enabled = true;

            if (EntropyLayerSettings.getDefaultInt(EntropyLayerSettings.properties_i.edgeSlide) == 0)
            {
                num_edgeSlideTension.Enabled = false;
            }
        }

        void litho_lwr(TableLayout lithography_table)
        {
            TableRow lit_lwr = new TableRow();
            lithography_table.Rows.Add(lit_lwr);

            GroupBox outer_gb = new GroupBox();
            outer_gb.Text = "LWR";

            Panel outer = new Panel();
            outer_gb.Content = outer;

            TableLayout tl = new TableLayout();
            tl.Rows.Add(new TableRow());

            outer.Content = tl;

            lit_lwr.Cells.Add(new TableCell() { Control = outer_gb });

            Panel leftPnl = new Panel();
            TableLayout left_tl = new TableLayout();
            left_tl.Rows.Add(new TableRow());
            leftPnl.Content = left_tl;
            Panel rightPnl = new Panel();
            gB_LWRCorrelation = new GroupBox();
            rightPnl.Content = gB_LWRCorrelation;

            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = leftPnl });
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = rightPnl });

            TableLayout upper_tl = new TableLayout();
            upper_tl.Rows.Add(new TableRow());
            left_tl.Rows[left_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = upper_tl });

            lbl_lithoLWR = new Label();
            lbl_lithoLWR.Text = "3-sigma";
            lbl_lithoLWR.MouseDoubleClick += lwr_RNG;
            lbl_lithoLWR.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            upper_tl.Rows[upper_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoLWR });

            num_lithoLWR = new NumericStepper();
            num_lithoLWR.Increment = 0.1;
            num_lithoLWR.DecimalPlaces = 2;
            num_lithoLWR.MinValue = 0;
            num_lithoLWR.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            setSize(num_lithoLWR, 55, (int)(label_Height * uiScaleFactor));
            upper_tl.Rows[upper_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoLWR) });

            lbl_lithoLWRFreq = new Label();
            lbl_lithoLWRFreq.Text = "Freq";
            lbl_lithoLWRFreq.ToolTip = "Frequency of LWR";
            upper_tl.Rows[upper_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoLWRFreq });

            num_lithoLWRFreq = new NumericStepper();
            num_lithoLWRFreq.Increment = 0.1;
            num_lithoLWRFreq.DecimalPlaces = 2;
            num_lithoLWRFreq.Value = 0.1;
            num_lithoLWRFreq.MinValue = 0.01;
            num_lithoLWRFreq.ToolTip = "Frequency of LWR";
            setSize(num_lithoLWRFreq, 55, num_Height);
            upper_tl.Rows[upper_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoLWRFreq) });

            comboBox_LWRNoiseType = new DropDown();
            comboBox_LWRNoiseType.DataContext = DataContext;
            comboBox_LWRNoiseType.BindDataContext(c => c.DataStore, (UIStringLists m) => m.noiseTypeList);
            comboBox_LWRNoiseType.SelectedIndex = 0;
            comboBox_LWRNoiseType.ToolTip = "Noise type for LWR";
            left_tl.Rows.Add(new TableRow());
            left_tl.Rows[left_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_LWRNoiseType) });

            TableLayout LWRCorrelation_table = new TableLayout();
            gB_LWRCorrelation.Text = "LWR Seed Correlation";
            gB_LWRCorrelation.Content = LWRCorrelation_table;

            TableRow clwr_tr0 = new TableRow();
            LWRCorrelation_table.Rows.Add(clwr_tr0);
            TableRow clwr_tr1 = new TableRow();
            LWRCorrelation_table.Rows.Add(clwr_tr1);

            this.rB_CLWR[0] = new RadioButton();
            this.rB_CLWR[0].Text = "0";
            this.rB_CLWR[0].Checked = true;

            TableCell rB_CLWR_0tc = new TableCell();
            rB_CLWR_0tc.Control = this.rB_CLWR[0];

            LWRCorrelation_table.Rows[0].Cells.Add(rB_CLWR_0tc);

            int rB_CLWR = 1;
            int clwr_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                this.rB_CLWR[rB_CLWR] = new RadioButton(this.rB_CLWR[0]);
                this.rB_CLWR[rB_CLWR].Text = rB_CLWR.ToString();
                this.rB_CLWR[rB_CLWR].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = this.rB_CLWR[rB_CLWR];
                LWRCorrelation_table.Rows[clwr_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_CLWR + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    clwr_rowIndex++;
                }
                rB_CLWR++;
            }
        }

        void litho_lwr2(TableLayout lithography_table)
        {
            TableRow lit_lwr = new TableRow();
            lithography_table.Rows.Add(lit_lwr);

            GroupBox outer_gb = new GroupBox();
            outer_gb.Text = "LWR2";

            Panel outer = new Panel();
            outer_gb.Content = outer;

            TableLayout tl = new TableLayout();
            tl.Rows.Add(new TableRow());

            outer.Content = tl;

            lit_lwr.Cells.Add(new TableCell() { Control = outer_gb });

            Panel leftPnl = new Panel();
            TableLayout left_tl = new TableLayout();
            left_tl.Rows.Add(new TableRow());
            leftPnl.Content = left_tl;
            Panel rightPnl = new Panel();

            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = leftPnl });
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });
            tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = rightPnl });

            TableLayout upper_tl = new TableLayout();
            upper_tl.Rows.Add(new TableRow());
            left_tl.Rows[left_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(upper_tl) });

            lbl_lithoLWR2 = new Label();
            lbl_lithoLWR2.Text = "3-sigma";
            lbl_lithoLWR2.MouseDoubleClick += lwr_RNG;
            lbl_lithoLWR2.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            upper_tl.Rows[upper_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoLWR2 });

            num_lithoLWR2 = new NumericStepper();
            num_lithoLWR2.Increment = 0.1;
            num_lithoLWR2.DecimalPlaces = 2;
            num_lithoLWR2.MinValue = 0;
            num_lithoLWR2.ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings.";
            setSize(num_lithoLWR2, 55, (int)(label_Height * uiScaleFactor));
            upper_tl.Rows[upper_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoLWR2) });

            lbl_lithoLWR2Freq = new Label();
            lbl_lithoLWR2Freq.Text = "Freq";
            lbl_lithoLWR2Freq.ToolTip = "Frequency of LWR";
            upper_tl.Rows[upper_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoLWR2Freq });

            num_lithoLWR2Freq = new NumericStepper();
            num_lithoLWR2Freq.Increment = 0.1;
            num_lithoLWR2Freq.DecimalPlaces = 2;
            num_lithoLWR2Freq.Value = 0.1;
            num_lithoLWR2Freq.MinValue = 0.01;
            num_lithoLWR2Freq.ToolTip = "Frequency of LWR";
            setSize(num_lithoLWR2Freq, 55, num_Height);
            upper_tl.Rows[upper_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoLWR2Freq) });

            comboBox_LWR2NoiseType = new DropDown();
            comboBox_LWR2NoiseType.DataContext = DataContext;
            comboBox_LWR2NoiseType.BindDataContext(c => c.DataStore, (UIStringLists m) => m.noiseTypeList);
            comboBox_LWR2NoiseType.SelectedIndex = 0;
            comboBox_LWR2NoiseType.ToolTip = "Noise type for LWR";
            left_tl.Rows.Add(new TableRow());
            left_tl.Rows[left_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(comboBox_LWR2NoiseType) });

            gB_LWR2Correlation = new GroupBox();
            TableLayout LWR2Correlation_table = new TableLayout();
            gB_LWR2Correlation.Text = "LWR Seed Correlation";
            gB_LWR2Correlation.Content = LWR2Correlation_table;
            rightPnl.Content = gB_LWR2Correlation;

            LWR2Correlation_table.Rows.Add(new TableRow());
            LWR2Correlation_table.Rows.Add(new TableRow());

            rB_CLWR2[0] = new RadioButton();
            rB_CLWR2[0].Text = "0";
            rB_CLWR2[0].Checked = true;

            TableCell rB_CLWR2_0tc = new TableCell();
            rB_CLWR2_0tc.Control = rB_CLWR2[0];

            LWR2Correlation_table.Rows[0].Cells.Add(rB_CLWR2_0tc);

            int rB_CLWR = 1;
            int clwr_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                rB_CLWR2[rB_CLWR] = new RadioButton(rB_CLWR2[0]);
                rB_CLWR2[rB_CLWR].Text = rB_CLWR.ToString();
                rB_CLWR2[rB_CLWR].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_CLWR2[rB_CLWR];
                LWR2Correlation_table.Rows[clwr_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_CLWR + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    clwr_rowIndex++;
                }
                rB_CLWR++;
            }
        }

        // SCDU
        void litho_scdu(TableLayout lithography_table)
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
            lithography_table.Rows.Add(new TableRow());
            lithography_table.Rows[lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer_gb });

            // Table layout within cell.
            TableCell tc_0 = new TableCell();
            tc_tl.Rows[0].Cells.Add(tc_0);

            TableLayout tc_0_tl = new TableLayout();
            tc_0.Control = tc_0_tl;

            tc_0_tl.Rows.Add(new TableRow());

            lbl_lithoCDUSide = new Label();
            lbl_lithoCDUSide.Text = "3-sigma";
            lbl_lithoCDUSide.MouseDoubleClick += sCDU_RNG;
            lbl_lithoCDUSide.ToolTip = "3-sigma CD variation applied to non-tip sides.";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoCDUSide, ScaleWidth = true });

            num_lithoCDUSide = new NumericStepper();
            num_lithoCDUSide.Increment = 0.1;
            num_lithoCDUSide.DecimalPlaces = 2;
            num_lithoCDUSide.MinValue = 0;
            num_lithoCDUSide.ToolTip = "3-sigma CD variation applied to non-tip sides.";
            setSize(num_lithoCDUSide, 55, num_Height);
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoCDUSide) });

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

            gB_CDUCorrelation = new GroupBox();
            TableLayout CDUCorrelation_table = new TableLayout();
            gB_CDUCorrelation.Text = "Correlation";
            gB_CDUCorrelation.Content = CDUCorrelation_table;
            ptc.Control = gB_CDUCorrelation;//, x, y);

            CDUCorrelation_table.Rows.Add(new TableRow());
            CDUCorrelation_table.Rows.Add(new TableRow());

            this.rB_CCDU[0] = new RadioButton();
            this.rB_CCDU[0].Text = "0";
            this.rB_CCDU[0].Checked = true;

            TableCell rB_CCDU_0tc = new TableCell();
            rB_CCDU_0tc.Control = this.rB_CCDU[0];

            CDUCorrelation_table.Rows[0].Cells.Add(rB_CCDU_0tc);

            int rB_CCDU = 1;
            int ccdu_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                this.rB_CCDU[rB_CCDU] = new RadioButton(this.rB_CCDU[0]);
                this.rB_CCDU[rB_CCDU].Text = rB_CCDU.ToString();
                this.rB_CCDU[rB_CCDU].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = this.rB_CCDU[rB_CCDU];
                CDUCorrelation_table.Rows[ccdu_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_CCDU + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    ccdu_rowIndex++;
                }
                rB_CCDU++;
            }
        }

        // TCDU
        void litho_tcdu(TableLayout lithography_table)
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
            lithography_table.Rows.Add(new TableRow());
            lithography_table.Rows[lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer_gb });

            // Table layout within cell.
            TableCell tc_0 = new TableCell();
            tc_tl.Rows[tc_tl.Rows.Count - 1].Cells.Add(tc_0);

            TableLayout tc_0_tl = new TableLayout();
            tc_0.Control = tc_0_tl;

            tc_0_tl.Rows.Add(new TableRow());

            lbl_lithoCDUTips = new Label();
            lbl_lithoCDUTips.Text = "3-sigma";
            lbl_lithoCDUTips.MouseDoubleClick += tCDU_RNG;
            lbl_lithoCDUTips.ToolTip = "3-sigma CD variation applied to tip sides.";
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoCDUTips, ScaleWidth = true });

            num_lithoCDUTips = new NumericStepper();
            num_lithoCDUTips.Increment = 0.1;
            num_lithoCDUTips.DecimalPlaces = 2;
            num_lithoCDUTips.MinValue = 0;
            num_lithoCDUTips.ToolTip = "3-sigma CD variation applied to tip sides.";
            setSize(num_lithoCDUTips, 55, num_Height);
            tc_0_tl.Rows[tc_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoCDUTips) });

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

            gB_TipCDUCorrelation = new GroupBox();
            TableLayout tipCDUCorrelation_table = new TableLayout();
            gB_TipCDUCorrelation.Text = "Correlation";
            gB_TipCDUCorrelation.Content = tipCDUCorrelation_table;
            ptc.Control = gB_TipCDUCorrelation;

            tipCDUCorrelation_table.Rows.Add(new TableRow());
            tipCDUCorrelation_table.Rows.Add(new TableRow());

            this.rB_CTCDU[0] = new RadioButton();
            this.rB_CTCDU[0].Text = "0";
            this.rB_CTCDU[0].Checked = true;

            TableCell rB_CTCDU_0tc = new TableCell();
            rB_CTCDU_0tc.Control = this.rB_CTCDU[0];

            tipCDUCorrelation_table.Rows[0].Cells.Add(rB_CTCDU_0tc);

            int rB_CTCDU = 1;
            int ctcdu_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                this.rB_CTCDU[rB_CTCDU] = new RadioButton(this.rB_CTCDU[0]);
                this.rB_CTCDU[rB_CTCDU].Text = rB_CTCDU.ToString();
                this.rB_CTCDU[rB_CTCDU].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = this.rB_CTCDU[rB_CTCDU];
                tipCDUCorrelation_table.Rows[ctcdu_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_CTCDU + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    ctcdu_rowIndex++;
                }
                rB_CTCDU++;
            }
        }

        // XOL, XOLR, XCOL
        void litho_xol(TableLayout lithography_table)
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
            lithography_table.Rows.Add(new TableRow());
            lithography_table.Rows[lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer_gb });

            // Table layout within cell.
            TableCell sigma_tc = new TableCell();
            left_tl.Rows[0].Cells.Add(sigma_tc);

            TableLayout sigma_tl = new TableLayout();
            sigma_tc.Control = sigma_tl;

            sigma_tl.Rows.Add(new TableRow());

            lbl_lithoHorOverlay = new Label();
            lbl_lithoHorOverlay.Text = "3-sigma";
            lbl_lithoHorOverlay.MouseDoubleClick += hOverlay_RNG;
            lbl_lithoHorOverlay.ToolTip = "3-sigma horizontal overlay.";
            sigma_tl.Rows[sigma_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoHorOverlay, ScaleWidth = true });

            num_lithoHorOverlay = new NumericStepper();
            num_lithoHorOverlay.Increment = 0.1;
            num_lithoHorOverlay.DecimalPlaces = 2;
            num_lithoHorOverlay.MinValue = 0;
            num_lithoHorOverlay.ToolTip = "3-sigma horizontal overlay.";
            setSize(num_lithoHorOverlay, 55, num_Height);
            sigma_tl.Rows[sigma_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoHorOverlay) });

            sigma_tl.Rows[sigma_tl.Rows.Count - 1].Cells.Add(new TableCell() { ScaleWidth = true });

            TableCell check_tc = new TableCell();
            mid_tl.Rows[mid_tl.Rows.Count - 1].Cells.Add(check_tc);

            TableLayout check_tl = new TableLayout();
            check_tc.Control = check_tl;

            check_tl.Rows.Add(new TableRow());
            check_tl.Rows[check_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            cB_overlayXReference_Av = new CheckBox();
            cB_overlayXReference_Av.Text = "Average";
            cB_overlayXReference_Av.ToolTip = "Use the average displaced location of layers as a reference for horizontal overlay";
            check_tl.Rows[check_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(cB_overlayXReference_Av) });

            TableCell right_tc = new TableCell();
            right_tl.Rows[right_tl.Rows.Count - 1].Cells.Add(right_tc);

            gB_overlayXReference = new GroupBox();
            TableLayout overlayXReference_table = new TableLayout();
            gB_overlayXReference.Text = "Reference";
            gB_overlayXReference.Content = overlayXReference_table;
            pnl_overlayRefX = new Panel();
            right_tc.Control = pnl_overlayRefX;

            overlayXReference_table.Rows.Add(new TableRow());
            overlayXReference_table.Rows.Add(new TableRow());

            this.rB_OLRX[0] = new RadioButton();
            this.rB_OLRX[0].Text = "0";
            this.rB_OLRX[0].Checked = true;

            TableCell rB_OLRX_0tc = new TableCell();
            rB_OLRX_0tc.Control = this.rB_OLRX[0];

            overlayXReference_table.Rows[0].Cells.Add(rB_OLRX_0tc);

            int rB_OLRX = 1;
            int olrx_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                // Don't add a button for our current layer
                this.rB_OLRX[rB_OLRX] = new RadioButton(this.rB_OLRX[0]);
                this.rB_OLRX[rB_OLRX].Text = rB_OLRX.ToString();
                this.rB_OLRX[rB_OLRX].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = this.rB_OLRX[rB_OLRX];
                overlayXReference_table.Rows[olrx_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_OLRX + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    olrx_rowIndex++;
                }
                rB_OLRX++;
            }

            gB_overlayXReference_Av = new GroupBox();
            overlayXReference_Av_table = new TableLayout();
            gB_overlayXReference_Av.Text = "References";
            gB_overlayXReference_Av.Content = overlayXReference_Av_table;

            overlayXReference_Av_table.Rows.Add(new TableRow());
            overlayXReference_Av_table.Rows.Add(new TableRow());

            int avolrx_rowIndex = 0;
            for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
            {
                // Don't add a button for our current layer
                cB_OLRX_Av[cb] = new CheckBox();
                cB_OLRX_Av[cb].Text = (cb + 1).ToString();

                TableCell tc0 = new TableCell();
                tc0.Control = cB_OLRX_Av[cb];
                overlayXReference_Av_table.Rows[avolrx_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if ((cb + 1) == CentralProperties.maxLayersForMC / 2)
                {
                    avolrx_rowIndex++;
                }
            }
            pnl_overlayRefX.Content = gB_overlayXReference;

            left_tl.Rows.Add(new TableRow());
            mid_tl.Rows.Add(new TableRow());
            right_tl.Rows.Add(new TableRow());

            left_tl.Rows[left_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });
            mid_tl.Rows[mid_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            TableCell right_tc2 = new TableCell();
            right_tl.Rows[right_tl.Rows.Count - 1].Cells.Add(right_tc2);

            gB_overlayXCorrelation = new GroupBox();
            TableLayout overlayXCorrelation_table = new TableLayout();
            gB_overlayXCorrelation.Text = "Correlation";
            gB_overlayXCorrelation.Content = overlayXCorrelation_table;
            right_tc2.Control = gB_overlayXCorrelation;

            overlayXCorrelation_table.Rows.Add(new TableRow());
            overlayXCorrelation_table.Rows.Add(new TableRow());

            this.rB_COLX[0] = new RadioButton();
            this.rB_COLX[0].Text = "0";
            this.rB_COLX[0].Checked = true;

            TableCell rB_COLX_0tc = new TableCell();
            rB_COLX_0tc.Control = this.rB_COLX[0];

            overlayXCorrelation_table.Rows[0].Cells.Add(rB_COLX_0tc);

            int rB_COLX = 1;
            int colx_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                this.rB_COLX[rB_COLX] = new RadioButton(this.rB_COLX[0]);
                this.rB_COLX[rB_COLX].Text = rB_COLX.ToString();
                this.rB_COLX[rB_COLX].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = this.rB_COLX[rB_COLX];
                overlayXCorrelation_table.Rows[colx_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_COLX + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    colx_rowIndex++;
                }
                rB_COLX++;
            }
        }

        // YOL, YOLR, YCOL
        void litho_yol(TableLayout lithography_table)
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

            outer_tl.Rows[outer_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = left });
            outer_tl.Rows[outer_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = mid, ScaleWidth = true });
            outer_tl.Rows[outer_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = right });
            outer.Content = outer_tl;

            // Outer table, row 1
            lithography_table.Rows.Add(new TableRow());
            lithography_table.Rows[lithography_table.Rows.Count - 1].Cells.Add(new TableCell() { Control = outer_gb });

            // Table layout within cell.
            TableCell sigma_tc = new TableCell();
            left_tl.Rows[0].Cells.Add(sigma_tc);

            TableLayout sigma_tl = new TableLayout();
            sigma_tc.Control = sigma_tl;

            sigma_tl.Rows.Add(new TableRow());

            lbl_lithoVerOverlay = new Label();
            lbl_lithoVerOverlay.Text = "3-sigma";
            lbl_lithoVerOverlay.MouseDoubleClick += vOverlay_RNG;
            lbl_lithoVerOverlay.ToolTip = "3-sigma verizontal overlay.";
            sigma_tl.Rows[sigma_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lbl_lithoVerOverlay, ScaleWidth = true});

            num_lithoVerOverlay = new NumericStepper();
            num_lithoVerOverlay.Increment = 0.1;
            num_lithoVerOverlay.DecimalPlaces = 2;
            num_lithoVerOverlay.MinValue = 0;
            num_lithoVerOverlay.ToolTip = "3-sigma vertical overlay.";
            setSize(num_lithoVerOverlay, 55, num_Height);
            sigma_tl.Rows[sigma_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_lithoVerOverlay), ScaleWidth = true });

            TableCell check_tc = new TableCell();
            mid_tl.Rows[mid_tl.Rows.Count - 1].Cells.Add(check_tc);

            TableLayout check_tl = new TableLayout();
            check_tc.Control = check_tl;

            check_tl.Rows.Add(new TableRow());
            check_tl.Rows[check_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            cB_overlayYReference_Av = new CheckBox();
            cB_overlayYReference_Av.Text = "Average";
            cB_overlayYReference_Av.ToolTip = "Use the average displaced location of layers as a reference for vertical overlay";
            check_tl.Rows[check_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(cB_overlayYReference_Av) });

            TableCell right_tc = new TableCell();
            right_tl.Rows[right_tl.Rows.Count - 1].Cells.Add(right_tc);

            gB_overlayYReference = new GroupBox();
            TableLayout overlayYReference_table = new TableLayout();
            gB_overlayYReference.Text = "Reference";
            gB_overlayYReference.Content = overlayYReference_table;
            pnl_overlayRefY = new Panel();
            right_tc.Control = pnl_overlayRefY;

            overlayYReference_table.Rows.Add(new TableRow());
            overlayYReference_table.Rows.Add(new TableRow());

            rB_OLRY_exp[0] = new RadioButton();
            rB_OLRY_exp[0].Text = "0";
            rB_OLRY_exp[0].Checked = true;

            TableCell rB_layer_OLRY_0tc = new TableCell();
            rB_layer_OLRY_0tc.Control = rB_OLRY_exp[0];

            overlayYReference_table.Rows[0].Cells.Add(rB_layer_OLRY_0tc);

            int rB_OLRY = 1;
            int olry_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                // Don't add a button for our current layer
                rB_OLRY_exp[rB_OLRY] = new RadioButton(rB_OLRY_exp[0]);
                rB_OLRY_exp[rB_OLRY].Text = rB_OLRY.ToString();
                rB_OLRY_exp[rB_OLRY].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = rB_OLRY_exp[rB_OLRY];
                overlayYReference_table.Rows[olry_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if (rB_OLRY + 1 == CentralProperties.maxLayersForMC / 2)
                {
                    olry_rowIndex++;
                }
                rB_OLRY++;
            }

            gB_overlayYReference_Av = new GroupBox();
            overlayYReference_Av_table = new TableLayout();
            gB_overlayYReference_Av.Text = "References";
            gB_overlayYReference_Av.Content = overlayYReference_Av_table;

            overlayYReference_Av_table.Rows.Add(new TableRow());
            overlayYReference_Av_table.Rows.Add(new TableRow());

            int avolry_rowIndex = 0;
            for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
            {
                // Don't add a button for our current layer
                cB_layer_OLRY_Av[cb] = new CheckBox();
                cB_layer_OLRY_Av[cb].Text = (cb + 1).ToString();

                TableCell tc0 = new TableCell();
                tc0.Control = cB_layer_OLRY_Av[cb];
                overlayYReference_Av_table.Rows[avolry_rowIndex].Cells.Add(tc0);
                // Wrap our positioning.
                if ((cb + 1) == CentralProperties.maxLayersForMC / 2)
                {
                    avolry_rowIndex++;
                }
            }
            pnl_overlayRefY.Content = gB_overlayYReference;

            left_tl.Rows.Add(new TableRow());
            mid_tl.Rows.Add(new TableRow());
            right_tl.Rows.Add(new TableRow());

            left_tl.Rows[left_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null });
            mid_tl.Rows[mid_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true });

            TableCell right_tc2 = new TableCell();
            right_tl.Rows[right_tl.Rows.Count - 1].Cells.Add(right_tc2);

            gB_overlayYCorrelation = new GroupBox();
            TableLayout overlayYCorrelation_table = new TableLayout();
            gB_overlayYCorrelation.Text = "Correlation";
            gB_overlayYCorrelation.Content = overlayYCorrelation_table;
            right_tc2.Control = gB_overlayYCorrelation;

            overlayYCorrelation_table.Rows.Add(new TableRow());
            overlayYCorrelation_table.Rows.Add(new TableRow());

            this.rB_COLY[0] = new RadioButton();
            this.rB_COLY[0].Text = "0";
            this.rB_COLY[0].Checked = true;

            TableCell rB_COLY_0tc = new TableCell();
            rB_COLY_0tc.Control = this.rB_COLY[0];

            overlayYCorrelation_table.Rows[0].Cells.Add(rB_COLY_0tc);

            int rB_COLY = 1;
            int coly_rowIndex = 0;
            for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
            {
                this.rB_COLY[rB_COLY] = new RadioButton(this.rB_COLY[0]);
                this.rB_COLY[rB_COLY].Text = rB_COLY.ToString();
                this.rB_COLY[rB_COLY].Checked = false;
                TableCell tc0 = new TableCell();
                tc0.Control = this.rB_COLY[rB_COLY];
                overlayYCorrelation_table.Rows[coly_rowIndex].Cells.Add(tc0);
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
