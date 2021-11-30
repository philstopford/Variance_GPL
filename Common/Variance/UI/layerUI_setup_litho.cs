using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    // 2D Layer Litho
    private NumericStepper num_lithoICRR, num_lithoOCRR, num_lithoCDUSide, num_lithoHorOverlay, num_lithoVerOverlay,
        num_lithoICV, num_lithoOCV, num_lithoCDUTips, num_lithoLWR, num_lithoLWRFreq, num_lithoLWR2, num_lithoLWR2Freq, num_lithoWobble, num_coeff1, num_coeff2;

    private Label lbl_lithoICRR, lbl_lithoOCRR, lbl_lithoCDUSide, lbl_lithoHorOverlay, lbl_lithoVerOverlay,
        lbl_lithoICV, lbl_lithoOCV, lbl_lithoCDUTips, lbl_lithoLWR, lbl_lithoLWRFreq, lbl_lithoLWR2, lbl_lithoLWR2Freq, lbl_lithoWobble, lbl_coeff1, lbl_coeff2;

    private GroupBox gB_overlayYReference, gB_overlayXReference,
        gB_overlayYReference_Av, gB_overlayXReference_Av,
        gB_overlayYCorrelation, gB_overlayXCorrelation,
        gB_CDUCorrelation, gB_TipCDUCorrelation,
        gB_LWRCorrelation, gB_LWR2Correlation;

    private RadioButton[] rB_OLRX, rB_OLRY_exp, rB_COLX, rB_COLY, rB_CCDU, rB_CTCDU, rB_CLWR, rB_CLWR2;
    private CheckBox cB_overlayXReference_Av, cB_overlayYReference_Av, cB_layer_LWRPreview;

    private CheckBox[] cB_OLRX_Av, cB_layer_OLRY_Av;

    private DropDown comboBox_LWRNoiseType, comboBox_LWR2NoiseType;

    private TableLayout overlayXReference_Av_table, overlayYReference_Av_table;

    private Panel pnl_overlayRefX, pnl_overlayRefY;

    private void twoD_LayerUISetup_litho(TableCell tc)
    {
        Application.Instance.Invoke(() =>
        {
            TableLayout lithography_table = new();
            gB_layer_lithography = new GroupBox {Text = "Lithography Parameters", Content = lithography_table};

            TableLayout t = new();
            t.Rows.Add(new TableRow());
            t.Rows[^1].Cells.Add(new TableCell { Control = gB_layer_lithography });
            t.Rows[^1].Cells.Add(new TableCell { Control = new Panel(), ScaleWidth = true });

            Panel p = new() {Content = t};
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
    private void litho_icr_ocr(TableLayout lithography_table)
    {
        TableLayout outer_tl = new();
        Panel outer = new() {Content = outer_tl};

        TableLayout tl_leftCol = new();
        tl_leftCol.Rows.Add(new TableRow());
        TableLayout tl_rightCol = new();
        tl_rightCol.Rows.Add(new TableRow());
        outer_tl.Rows.Add(new TableRow());
        outer_tl.Rows[^1].Cells.Add(new TableCell { Control = tl_leftCol });
        outer_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });
        outer_tl.Rows[^1].Cells.Add(new TableCell { Control = tl_rightCol });

        // Outer table, row 1
        lithography_table.Rows.Add(new TableRow());
        lithography_table.Rows[^1].Cells.Add(new TableCell { Control = outer });


        GroupBox groupBox_rounding = new() {Text = "Rounding"};
        // Table layout within cell.
        TableCell leftCol_tc = new() {Control = groupBox_rounding};
        tl_leftCol.Rows[^1].Cells.Add(leftCol_tc);

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

        groupBox_misc = new GroupBox {Text = "Misc"};

        TableCell rightCol_tc = new() {Control = groupBox_misc};
        tl_rightCol.Rows[^1].Cells.Add(rightCol_tc);


        TableLayout rounding_table = new();
        groupBox_rounding.Content = rounding_table;

        TableLayout litMisc_table = new();
        groupBox_misc.Content = litMisc_table;

        rounding_table.Rows.Add(new TableRow());
        litMisc_table.Rows.Add(new TableRow());

        lbl_lithoICRR = new Label
        {
            Text = "Inner Radius", ToolTip = "Inner vertex (concave) corner rounding radius."
        };
        rounding_table.Rows[^1].Cells.Add(new TableCell());
        rounding_table.Rows[^1].Cells[^1].Control = lbl_lithoICRR;

        num_lithoICRR = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            MinValue = 0,
            ToolTip = "Inner vertex (concave) corner rounding radius."
        };
        setSize(num_lithoICRR, 55);
        rounding_table.Rows[^1].Cells.Add(new TableCell());
        rounding_table.Rows[^1].Cells[^1].Control = TableLayout.AutoSized(num_lithoICRR);

        lbl_lithoICV = new Label
        {
            Text = "Var", ToolTip = "3-sigma inner vertex (concave) corner rounding radius variation."
        };
        lbl_lithoICV.MouseDoubleClick += ICV_RNG;
        rounding_table.Rows[^1].Cells.Add(new TableCell());
        rounding_table.Rows[^1].Cells[^1].Control = lbl_lithoICV;

        num_lithoICV = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            MinValue = 0,
            ToolTip = "3-sigma inner vertex (concave) corner rounding radius variation."
        };
        setSize(num_lithoICV, 55);
        rounding_table.Rows[^1].Cells.Add(new TableCell());
        rounding_table.Rows[^1].Cells[^1].Control = TableLayout.AutoSized(num_lithoICV);

        rounding_table.Rows.Add(new TableRow());

        lbl_lithoOCRR = new Label
        {
            Text = "Outer Radius", ToolTip = "Outer vertex (concave) corner rounding radius."
        };
        rounding_table.Rows[^1].Cells.Add(new TableCell());
        rounding_table.Rows[^1].Cells[^1].Control = lbl_lithoOCRR;

        num_lithoOCRR = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            MinValue = 0,
            ToolTip = "Outer vertex (concave) corner rounding radius."
        };
        setSize(num_lithoOCRR, 55);
        rounding_table.Rows[^1].Cells.Add(new TableCell());
        rounding_table.Rows[^1].Cells[^1].Control = TableLayout.AutoSized(num_lithoOCRR);

        lbl_lithoOCV = new Label
        {
            Text = "Var", ToolTip = "3-sigma outer vertex (concave) corner rounding radius variation."
        };
        lbl_lithoOCV.MouseDoubleClick += OCV_RNG;
        rounding_table.Rows[^1].Cells.Add(new TableCell());
        rounding_table.Rows[^1].Cells[^1].Control = lbl_lithoOCV;

        num_lithoOCV = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            MinValue = 0,
            ToolTip = "3-sigma outer vertex (concave) corner rounding radius variation."
        };
        setSize(num_lithoOCV, 55);
        rounding_table.Rows[^1].Cells.Add(new TableCell());
        rounding_table.Rows[^1].Cells[^1].Control = TableLayout.AutoSized(num_lithoOCV);


        TableLayout distortion_tl = new();
        distortion_tl.Rows.Add(new TableRow());
        litMisc_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(distortion_tl) });

        lbl_coeff1 = new Label {Text = "Lens k1", ToolTip = "Lens k1"};
        distortion_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_coeff1 });

        num_coeff1 = new NumericStepper {Increment = 0.1, DecimalPlaces = 2, Value = 0.0, ToolTip = "Lens k1"};
        setSize(num_coeff1, 55);
        distortion_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_coeff1) });


        lbl_coeff2 = new Label {Text = "Lens k2", ToolTip = "Lens k2"};
        distortion_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_coeff2 });

        num_coeff2 = new NumericStepper {Increment = 0.1, DecimalPlaces = 2, Value = 0.0, ToolTip = "Lens k2"};
        setSize(num_coeff2, 55);
        distortion_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_coeff2) });

        litMisc_table.Rows.Add(new TableRow());

        TableLayout edge_slide_tl = new();
        edge_slide_tl.Rows.Add(new TableRow());
        litMisc_table.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(edge_slide_tl) });

        cB_edgeSlide = new CheckBox
        {
            Text = "Edge Slide",
            Width = 90,
            ToolTip = "If checked, apply tension to each edge for the contour generation"
        };
        edge_slide_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(cB_edgeSlide) });
        if (EntropyLayerSettings.getDefaultInt(EntropyLayerSettings.properties_i.edgeSlide) == 1)
        {
            cB_edgeSlide.Checked = true;
        }

        lbl_edgeSlideTension = new Label
        {
            Text = "Tension",
            Width = 50,
            ToolTip = "Amount of tension to apply, to pull the midpoint towards the longer edge"
        };
        edge_slide_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_edgeSlideTension });

        num_edgeSlideTension = new NumericStepper
        {
            DecimalPlaces = 2,
            Increment = 0.1,
            Value = (double) EntropyLayerSettings.getDefaultDecimal(
                EntropyLayerSettings.properties_decimal.eTension),
            MinValue = 0.01
        };
        setSize(num_edgeSlideTension, 55);
        num_edgeSlideTension.ToolTip = "Amount of tension to apply, to pull the midpoint towards the longer edge";
        edge_slide_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_edgeSlideTension) });
        num_edgeSlideTension.Enabled = true;

        if (EntropyLayerSettings.getDefaultInt(EntropyLayerSettings.properties_i.edgeSlide) == 0)
        {
            num_edgeSlideTension.Enabled = false;
        }
    }

    private void litho_lwr(TableLayout lithography_table)
    {
        TableRow lit_lwr = new();
        lithography_table.Rows.Add(lit_lwr);

        TableLayout tl = new();
        tl.Rows.Add(new TableRow());

        Panel outer = new() {Content = tl};
        GroupBox outer_gb = new() {Text = "LWR", Content = outer};

        lit_lwr.Cells.Add(new TableCell { Control = outer_gb });

        TableLayout left_tl = new();
        left_tl.Rows.Add(new TableRow());
        Panel leftPnl = new() {Content = left_tl};

        TableLayout LWRCorrelation_table = new();
        gB_LWRCorrelation = new GroupBox {Text = "LWR Seed Correlation", Content = LWRCorrelation_table};
        Panel rightPnl = new() {Content = gB_LWRCorrelation};

        tl.Rows[^1].Cells.Add(new TableCell { Control = leftPnl });
        tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });
        tl.Rows[^1].Cells.Add(new TableCell { Control = rightPnl });

        TableLayout upper_tl = new();
        upper_tl.Rows.Add(new TableRow());
        left_tl.Rows[^1].Cells.Add(new TableCell { Control = upper_tl });

        lbl_lithoLWR = new Label
        {
            Text = "3-sigma",
            ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings."
        };
        lbl_lithoLWR.MouseDoubleClick += lwr_RNG;
        upper_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoLWR });

        num_lithoLWR = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            MinValue = 0,
            ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings."
        };
        setSize(num_lithoLWR, 55);
        upper_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoLWR) });

        lbl_lithoLWRFreq = new Label {Text = "Freq", ToolTip = "Frequency of LWR"};
        upper_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoLWRFreq });

        num_lithoLWRFreq = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            Value = 0.1,
            MinValue = 0.01,
            ToolTip = "Frequency of LWR"
        };
        setSize(num_lithoLWRFreq, 55);
        upper_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoLWRFreq) });

        comboBox_LWRNoiseType = new DropDown
        {
            DataContext = DataContext, SelectedIndex = 0, ToolTip = "Noise type for LWR"
        };
        comboBox_LWRNoiseType.BindDataContext(c => c.DataStore, (UIStringLists m) => m.noiseTypeList);
        left_tl.Rows.Add(new TableRow());
        left_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_LWRNoiseType) });


        TableRow clwr_tr0 = new();
        LWRCorrelation_table.Rows.Add(clwr_tr0);
        TableRow clwr_tr1 = new();
        LWRCorrelation_table.Rows.Add(clwr_tr1);

        rB_CLWR[0] = new RadioButton {Text = "0", Checked = true};

        TableCell rB_CLWR_0tc = new() {Control = rB_CLWR[0]};

        LWRCorrelation_table.Rows[0].Cells.Add(rB_CLWR_0tc);

        int rB_CLWR_ = 1;
        int clwr_rowIndex = 0;
        for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
        {
            rB_CLWR[rB_CLWR_] = new RadioButton(rB_CLWR[0]) {Text = rB_CLWR.ToString(), Checked = false};
            TableCell tc0 = new() {Control = rB_CLWR[rB_CLWR_]};
            LWRCorrelation_table.Rows[clwr_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (rB_CLWR_ + 1 == CentralProperties.maxLayersForMC / 2)
            {
                clwr_rowIndex++;
            }
            rB_CLWR_++;
        }
    }

    private void litho_lwr2(TableLayout lithography_table)
    {
        TableRow lit_lwr = new();
        lithography_table.Rows.Add(lit_lwr);

        TableLayout tl = new();
        tl.Rows.Add(new TableRow());
        Panel outer = new() {Content = tl};

        GroupBox outer_gb = new() {Content = outer, Text = "LWR2"};

        lit_lwr.Cells.Add(new TableCell { Control = outer_gb });

        TableLayout left_tl = new();
        left_tl.Rows.Add(new TableRow());
        Panel leftPnl = new() {Content = left_tl};

        TableLayout LWR2Correlation_table = new();
        gB_LWR2Correlation = new GroupBox {Text = "LWR Seed Correlation", Content = LWR2Correlation_table};
        Panel rightPnl = new() {Content = gB_LWR2Correlation};

        tl.Rows[^1].Cells.Add(new TableCell { Control = leftPnl });
        tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });
        tl.Rows[^1].Cells.Add(new TableCell { Control = rightPnl });

        TableLayout upper_tl = new();
        upper_tl.Rows.Add(new TableRow());
        left_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(upper_tl) });

        lbl_lithoLWR2 = new Label
        {
            Text = "3-sigma",
            ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings."
        };
        lbl_lithoLWR2.MouseDoubleClick += lwr2_RNG;
        upper_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoLWR2 });

        num_lithoLWR2 = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            MinValue = 0,
            ToolTip = "3-sigma line width roughness. Mapped to edge by setting in simulation settings."
        };
        setSize(num_lithoLWR2, 55);
        upper_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoLWR2) });

        lbl_lithoLWR2Freq = new Label {Text = "Freq", ToolTip = "Frequency of LWR"};
        upper_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoLWR2Freq });

        num_lithoLWR2Freq = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            Value = 0.1,
            MinValue = 0.01,
            ToolTip = "Frequency of LWR"
        };
        setSize(num_lithoLWR2Freq, 55);
        upper_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoLWR2Freq) });

        comboBox_LWR2NoiseType = new DropDown
        {
            DataContext = DataContext, SelectedIndex = 0, ToolTip = "Noise type for LWR"
        };
        comboBox_LWR2NoiseType.BindDataContext(c => c.DataStore, (UIStringLists m) => m.noiseTypeList);
        left_tl.Rows.Add(new TableRow());
        left_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(comboBox_LWR2NoiseType) });


        LWR2Correlation_table.Rows.Add(new TableRow());
        LWR2Correlation_table.Rows.Add(new TableRow());

        rB_CLWR2[0] = new RadioButton {Text = "0", Checked = true};

        TableCell rB_CLWR2_0tc = new() {Control = rB_CLWR2[0]};

        LWR2Correlation_table.Rows[0].Cells.Add(rB_CLWR2_0tc);

        int rB_CLWR_ = 1;
        int clwr_rowIndex = 0;
        for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
        {
            rB_CLWR2[rB_CLWR_] = new RadioButton(rB_CLWR2[0]) {Text = rB_CLWR.ToString(), Checked = false};
            TableCell tc0 = new() {Control = rB_CLWR2[rB_CLWR_]};
            LWR2Correlation_table.Rows[clwr_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (rB_CLWR_ + 1 == CentralProperties.maxLayersForMC / 2)
            {
                clwr_rowIndex++;
            }
            rB_CLWR_++;
        }
    }

    // SCDU
    private void litho_scdu(TableLayout lithography_table)
    {
        TableLayout outer_tl = new();
        Panel outer = new() {Content = outer_tl};

        GroupBox outer_gb = new() {Text = "Side CDU", Content = outer};

        TableLayout tc_tl = new();
        outer_tl.Rows.Add(new TableRow());
        outer_tl.Rows[0].Cells.Add(new TableCell { Control = tc_tl });
        tc_tl.Rows.Add(new TableRow());

        // Outer table, row 1
        lithography_table.Rows.Add(new TableRow());
        lithography_table.Rows[^1].Cells.Add(new TableCell { Control = outer_gb });

        // Table layout within cell.
        TableCell tc_0 = new();
        tc_tl.Rows[0].Cells.Add(tc_0);

        TableLayout tc_0_tl = new();
        tc_0.Control = tc_0_tl;

        tc_0_tl.Rows.Add(new TableRow());

        lbl_lithoCDUSide = new Label {Text = "3-sigma", ToolTip = "3-sigma CD variation applied to non-tip sides."};
        lbl_lithoCDUSide.MouseDoubleClick += sCDU_RNG;
        tc_0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoCDUSide, ScaleWidth = true });

        num_lithoCDUSide = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            MinValue = 0,
            ToolTip = "3-sigma CD variation applied to non-tip sides."
        };
        setSize(num_lithoCDUSide, 55);
        tc_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoCDUSide) });

        tc_tl.Rows[0].Cells.Add(new TableCell { ScaleWidth = true });

        TableCell tc_1 = new();
        tc_tl.Rows[0].Cells.Add(tc_1);

        Panel p = new();
        tc_1.Control = p;

        TableLayout ptl = new();
        p.Content = ptl;
        ptl.Rows.Add(new TableRow());
        TableCell ptc = new();
        ptl.Rows[0].Cells.Add(ptc);

        TableLayout CDUCorrelation_table = new();

        gB_CDUCorrelation = new GroupBox {Text = "Correlation", Content = CDUCorrelation_table};
        ptc.Control = gB_CDUCorrelation;//, x, y);

        CDUCorrelation_table.Rows.Add(new TableRow());
        CDUCorrelation_table.Rows.Add(new TableRow());

        rB_CCDU[0] = new RadioButton {Text = "0", Checked = true};

        TableCell rB_CCDU_0tc = new() {Control = rB_CCDU[0]};

        CDUCorrelation_table.Rows[0].Cells.Add(rB_CCDU_0tc);

        int rB_CCDU_ = 1;
        int ccdu_rowIndex = 0;
        for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
        {
            rB_CCDU[rB_CCDU_] = new RadioButton(rB_CCDU[0]) {Text = rB_CCDU.ToString(), Checked = false};
            TableCell tc0 = new() {Control = rB_CCDU[rB_CCDU_]};
            CDUCorrelation_table.Rows[ccdu_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (rB_CCDU_ + 1 == CentralProperties.maxLayersForMC / 2)
            {
                ccdu_rowIndex++;
            }
            rB_CCDU_++;
        }
    }

    // TCDU
    private void litho_tcdu(TableLayout lithography_table)
    {
        TableLayout outer_tl = new();
        Panel outer = new() {Content = outer_tl};

        GroupBox outer_gb = new() {Text = "Tips CDU", Content = outer};
        TableLayout tc_tl = new();
        outer_tl.Rows.Add(new TableRow());
        outer_tl.Rows[0].Cells.Add(new TableCell { Control = tc_tl });
        tc_tl.Rows.Add(new TableRow());

        // Outer table, row 1
        lithography_table.Rows.Add(new TableRow());
        lithography_table.Rows[^1].Cells.Add(new TableCell { Control = outer_gb });

        // Table layout within cell.
        TableCell tc_0 = new();
        tc_tl.Rows[^1].Cells.Add(tc_0);

        TableLayout tc_0_tl = new();
        tc_0.Control = tc_0_tl;

        tc_0_tl.Rows.Add(new TableRow());

        lbl_lithoCDUTips = new Label {Text = "3-sigma", ToolTip = "3-sigma CD variation applied to tip sides."};
        lbl_lithoCDUTips.MouseDoubleClick += tCDU_RNG;
        tc_0_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoCDUTips, ScaleWidth = true });

        num_lithoCDUTips = new NumericStepper
        {
            Increment = 0.1,
            DecimalPlaces = 2,
            MinValue = 0,
            ToolTip = "3-sigma CD variation applied to tip sides."
        };
        setSize(num_lithoCDUTips, 55);
        tc_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoCDUTips) });

        tc_tl.Rows[^1].Cells.Add(new TableCell());

        Panel p = new();
        TableCell tc_1 = new() {Control = p};
        tc_tl.Rows[^1].Cells.Add(tc_1);

        TableLayout ptl = new();
        p.Content = ptl;
        ptl.Rows.Add(new TableRow());
        ptl.Rows[0].Cells.Add(new TableCell { ScaleWidth = true });

        TableLayout tipCDUCorrelation_table = new();
        gB_TipCDUCorrelation = new GroupBox {Text = "Correlation", Content = tipCDUCorrelation_table};
        TableCell ptc = new() {Control = gB_TipCDUCorrelation};
        ptl.Rows[0].Cells.Add(ptc);

        tipCDUCorrelation_table.Rows.Add(new TableRow());
        tipCDUCorrelation_table.Rows.Add(new TableRow());

        rB_CTCDU[0] = new RadioButton {Text = "0", Checked = true};

        TableCell rB_CTCDU_0tc = new() {Control = rB_CTCDU[0]};

        tipCDUCorrelation_table.Rows[0].Cells.Add(rB_CTCDU_0tc);

        int rB_CTCDU_ = 1;
        int ctcdu_rowIndex = 0;
        for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
        {
            rB_CTCDU[rB_CTCDU_] = new RadioButton(rB_CTCDU[0]) {Text = rB_CTCDU.ToString(), Checked = false};
            TableCell tc0 = new() {Control = rB_CTCDU[rB_CTCDU_]};
            tipCDUCorrelation_table.Rows[ctcdu_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (rB_CTCDU_ + 1 == CentralProperties.maxLayersForMC / 2)
            {
                ctcdu_rowIndex++;
            }
            rB_CTCDU_++;
        }
    }

    // XOL, XOLR, XCOL
    private void litho_xol(TableLayout lithography_table)
    {
        TableLayout outer_tl = new();
        Panel outer = new() {Content = outer_tl};
        GroupBox outer_gb = new() {Text = "Horizontal Overlay", Content = outer};
        outer_tl.Rows.Add(new TableRow());

        TableLayout left_tl = new();
        left_tl.Rows.Add(new TableRow());
        TableLayout mid_tl = new();
        mid_tl.Rows.Add(new TableRow());
        TableLayout right_tl = new();
        right_tl.Rows.Add(new TableRow());

        Panel left = new() {Content = left_tl};
        Panel mid = new() {Content = mid_tl};
        Panel right = new() {Content = right_tl};

        outer_tl.Rows[0].Cells.Add(new TableCell { Control = left });
        outer_tl.Rows[0].Cells.Add(new TableCell { Control = mid, ScaleWidth = true });
        outer_tl.Rows[0].Cells.Add(new TableCell { Control = right });

        // Outer table, row 1
        lithography_table.Rows.Add(new TableRow());
        lithography_table.Rows[^1].Cells.Add(new TableCell { Control = outer_gb });

        // Table layout within cell.
        TableCell sigma_tc = new();
        left_tl.Rows[0].Cells.Add(sigma_tc);

        TableLayout sigma_tl = new();
        sigma_tc.Control = sigma_tl;

        sigma_tl.Rows.Add(new TableRow());

        lbl_lithoHorOverlay = new Label {Text = "3-sigma", ToolTip = "3-sigma horizontal overlay."};
        lbl_lithoHorOverlay.MouseDoubleClick += hOverlay_RNG;
        sigma_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoHorOverlay, ScaleWidth = true });

        num_lithoHorOverlay = new NumericStepper
        {
            Increment = 0.1, DecimalPlaces = 2, MinValue = 0, ToolTip = "3-sigma horizontal overlay."
        };
        setSize(num_lithoHorOverlay, 55);
        sigma_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoHorOverlay) });

        sigma_tl.Rows[^1].Cells.Add(new TableCell { ScaleWidth = true });

        TableCell check_tc = new();
        mid_tl.Rows[^1].Cells.Add(check_tc);

        TableLayout check_tl = new();
        check_tc.Control = check_tl;

        check_tl.Rows.Add(new TableRow());
        check_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });

        cB_overlayXReference_Av = new CheckBox
        {
            Text = "Average",
            ToolTip = "Use the average displaced location of layers as a reference for horizontal overlay"
        };
        check_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(cB_overlayXReference_Av) });

        TableLayout overlayXReference_table = new();
        gB_overlayXReference = new GroupBox {Text = "Reference", Content = overlayXReference_table};

        overlayXReference_table.Rows.Add(new TableRow());
        overlayXReference_table.Rows.Add(new TableRow());

        rB_OLRX[0] = new RadioButton {Text = "0", Checked = true};

        TableCell rB_OLRX_0tc = new() {Control = rB_OLRX[0]};

        overlayXReference_table.Rows[0].Cells.Add(rB_OLRX_0tc);

        int rB_OLRX_ = 1;
        int olrx_rowIndex = 0;
        for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
        {
            // Don't add a button for our current layer
            rB_OLRX[rB_OLRX_] = new RadioButton(rB_OLRX[0]) {Text = rB_OLRX.ToString(), Checked = false};
            TableCell tc0 = new() {Control = rB_OLRX[rB_OLRX_]};
            overlayXReference_table.Rows[olrx_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (rB_OLRX_ + 1 == CentralProperties.maxLayersForMC / 2)
            {
                olrx_rowIndex++;
            }
            rB_OLRX_++;
        }

        overlayXReference_Av_table = new TableLayout();
        gB_overlayXReference_Av = new GroupBox {Text = "References", Content = overlayXReference_Av_table};

        overlayXReference_Av_table.Rows.Add(new TableRow());
        overlayXReference_Av_table.Rows.Add(new TableRow());

        int avolrx_rowIndex = 0;
        for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
        {
            // Don't add a button for our current layer
            cB_OLRX_Av[cb] = new CheckBox {Text = (cb + 1).ToString()};

            TableCell tc0 = new() {Control = cB_OLRX_Av[cb]};
            overlayXReference_Av_table.Rows[avolrx_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (cb + 1 == CentralProperties.maxLayersForMC / 2)
            {
                avolrx_rowIndex++;
            }
        }

        pnl_overlayRefX = new Panel {Content = gB_overlayXReference};
        TableCell right_tc = new() {Control = pnl_overlayRefX};
        right_tl.Rows[^1].Cells.Add(right_tc);
            
        left_tl.Rows.Add(new TableRow());
        mid_tl.Rows.Add(new TableRow());
        right_tl.Rows.Add(new TableRow());

        left_tl.Rows[^1].Cells.Add(new TableCell { Control = null });
        mid_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });

        TableLayout overlayXCorrelation_table = new();
        gB_overlayXCorrelation = new GroupBox {Text = "Correlation", Content = overlayXCorrelation_table};
        TableCell right_tc2 = new() {Control = gB_overlayXCorrelation};
        right_tl.Rows[^1].Cells.Add(right_tc2);
            
        overlayXCorrelation_table.Rows.Add(new TableRow());
        overlayXCorrelation_table.Rows.Add(new TableRow());

        rB_COLX[0] = new RadioButton {Text = "0", Checked = true};

        TableCell rB_COLX_0tc = new() {Control = rB_COLX[0]};

        overlayXCorrelation_table.Rows[0].Cells.Add(rB_COLX_0tc);

        int rB_COLX_ = 1;
        int colx_rowIndex = 0;
        for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
        {
            rB_COLX[rB_COLX_] = new RadioButton(rB_COLX[0]) {Text = rB_COLX.ToString(), Checked = false};
            TableCell tc0 = new() {Control = rB_COLX[rB_COLX_]};
            overlayXCorrelation_table.Rows[colx_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (rB_COLX_ + 1 == CentralProperties.maxLayersForMC / 2)
            {
                colx_rowIndex++;
            }
            rB_COLX_++;
        }
    }

    // YOL, YOLR, YCOL
    private void litho_yol(TableLayout lithography_table)
    {
        TableLayout outer_tl = new();
        Panel outer = new() {Content = outer_tl};
        GroupBox outer_gb = new() {Text = "Vertical Overlay", Content = outer};
        outer_tl.Rows.Add(new TableRow());

        TableLayout left_tl = new();
        left_tl.Rows.Add(new TableRow());
        TableLayout mid_tl = new();
        mid_tl.Rows.Add(new TableRow());
        TableLayout right_tl = new();
        right_tl.Rows.Add(new TableRow());

        Panel left = new() {Content = left_tl};
        Panel mid = new() {Content = mid_tl};
        Panel right = new() {Content = right_tl};

        outer_tl.Rows[^1].Cells.Add(new TableCell { Control = left });
        outer_tl.Rows[^1].Cells.Add(new TableCell { Control = mid, ScaleWidth = true });
        outer_tl.Rows[^1].Cells.Add(new TableCell { Control = right });

        // Outer table, row 1
        lithography_table.Rows.Add(new TableRow());
        lithography_table.Rows[^1].Cells.Add(new TableCell { Control = outer_gb });

        // Table layout within cell.
        TableCell sigma_tc = new();
        left_tl.Rows[0].Cells.Add(sigma_tc);

        TableLayout sigma_tl = new();
        sigma_tc.Control = sigma_tl;

        sigma_tl.Rows.Add(new TableRow());

        lbl_lithoVerOverlay = new Label {Text = "3-sigma", ToolTip = "3-sigma vertical overlay."};
        lbl_lithoVerOverlay.MouseDoubleClick += vOverlay_RNG;
        sigma_tl.Rows[^1].Cells.Add(new TableCell { Control = lbl_lithoVerOverlay, ScaleWidth = true});

        num_lithoVerOverlay = new NumericStepper
        {
            Increment = 0.1, DecimalPlaces = 2, MinValue = 0, ToolTip = "3-sigma vertical overlay."
        };
        setSize(num_lithoVerOverlay, 55);
        sigma_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(num_lithoVerOverlay), ScaleWidth = true });

        TableCell check_tc = new();
        mid_tl.Rows[^1].Cells.Add(check_tc);

        TableLayout check_tl = new();
        check_tc.Control = check_tl;

        check_tl.Rows.Add(new TableRow());
        check_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });

        cB_overlayYReference_Av = new CheckBox
        {
            Text = "Average",
            ToolTip = "Use the average displaced location of layers as a reference for vertical overlay"
        };
        check_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(cB_overlayYReference_Av) });


        TableLayout overlayYReference_table = new();
        gB_overlayYReference = new GroupBox {Text = "Reference", Content = overlayYReference_table};

        overlayYReference_table.Rows.Add(new TableRow());
        overlayYReference_table.Rows.Add(new TableRow());

        rB_OLRY_exp[0] = new RadioButton {Text = "0", Checked = true};

        TableCell rB_layer_OLRY_0tc = new() {Control = rB_OLRY_exp[0]};

        overlayYReference_table.Rows[0].Cells.Add(rB_layer_OLRY_0tc);

        int rB_OLRY = 1;
        int olry_rowIndex = 0;
        for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
        {
            // Don't add a button for our current layer
            rB_OLRY_exp[rB_OLRY] = new RadioButton(rB_OLRY_exp[0]) {Text = rB_OLRY.ToString(), Checked = false};
            TableCell tc0 = new() {Control = rB_OLRY_exp[rB_OLRY]};
            overlayYReference_table.Rows[olry_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (rB_OLRY + 1 == CentralProperties.maxLayersForMC / 2)
            {
                olry_rowIndex++;
            }
            rB_OLRY++;
        }

        overlayYReference_Av_table = new TableLayout();
        gB_overlayYReference_Av = new GroupBox {Text = "References", Content = overlayYReference_Av_table};

        overlayYReference_Av_table.Rows.Add(new TableRow());
        overlayYReference_Av_table.Rows.Add(new TableRow());

        int avolry_rowIndex = 0;
        for (int cb = 0; cb < CentralProperties.maxLayersForMC; cb++)
        {
            // Don't add a button for our current layer
            cB_layer_OLRY_Av[cb] = new CheckBox {Text = (cb + 1).ToString()};

            TableCell tc0 = new() {Control = cB_layer_OLRY_Av[cb]};
            overlayYReference_Av_table.Rows[avolry_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (cb + 1 == CentralProperties.maxLayersForMC / 2)
            {
                avolry_rowIndex++;
            }
        }

        pnl_overlayRefY = new Panel {Content = gB_overlayYReference};
        TableCell right_tc = new() {Control = pnl_overlayRefY};
        right_tl.Rows[^1].Cells.Add(right_tc);

        left_tl.Rows.Add(new TableRow());
        mid_tl.Rows.Add(new TableRow());
        right_tl.Rows.Add(new TableRow());

        left_tl.Rows[^1].Cells.Add(new TableCell { Control = null });
        mid_tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true });

        TableCell right_tc2 = new();
        right_tl.Rows[^1].Cells.Add(right_tc2);

        gB_overlayYCorrelation = new GroupBox();
        TableLayout overlayYCorrelation_table = new();
        gB_overlayYCorrelation.Text = "Correlation";
        gB_overlayYCorrelation.Content = overlayYCorrelation_table;
        right_tc2.Control = gB_overlayYCorrelation;

        overlayYCorrelation_table.Rows.Add(new TableRow());
        overlayYCorrelation_table.Rows.Add(new TableRow());

        rB_COLY[0] = new RadioButton {Text = "0", Checked = true};

        TableCell rB_COLY_0tc = new() {Control = rB_COLY[0]};

        overlayYCorrelation_table.Rows[0].Cells.Add(rB_COLY_0tc);

        int rB_COLY_ = 1;
        int coly_rowIndex = 0;
        for (int rb = 1; rb < CentralProperties.maxLayersForMC; rb++)
        {
            rB_COLY[rB_COLY_] = new RadioButton(rB_COLY[0]) {Text = rB_COLY.ToString(), Checked = false};
            TableCell tc0 = new() {Control = rB_COLY[rB_COLY_]};
            overlayYCorrelation_table.Rows[coly_rowIndex].Cells.Add(tc0);
            // Wrap our positioning.
            if (rB_COLY_ + 1 == CentralProperties.maxLayersForMC / 2)
            {
                coly_rowIndex++;
            }
            rB_COLY_++;
        }
    }
}