using System;
using System.Collections.Generic;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private TableLayout layerProperties_tl;
    private TableCell layerProperties_tc;

    private void twoD_LayerUISetup()
    {
        rB_OLRX = new RadioButton[CentralProperties.maxLayersForMC];
        rB_OLRY_exp = new RadioButton[CentralProperties.maxLayersForMC];

        rB_COLX = new RadioButton[CentralProperties.maxLayersForMC];
        rB_COLY = new RadioButton[CentralProperties.maxLayersForMC];

        rB_CCDU = new RadioButton[CentralProperties.maxLayersForMC];
        rB_CTCDU = new RadioButton[CentralProperties.maxLayersForMC];

        cB_OLRX_Av = new CheckBox[CentralProperties.maxLayersForMC];
        cB_layer_OLRY_Av = new CheckBox[CentralProperties.maxLayersForMC];

        rB_CLWR = new RadioButton[CentralProperties.maxLayersForMC];
        rB_CLWR2 = new RadioButton[CentralProperties.maxLayersForMC];

        xOLRBs_enabledState = new List<bool[]>();
        yOLRBs_enabledState = new List<bool[]>();

        xCOLRBs_enabledState = new List<bool[]>();
        yCOLRBs_enabledState = new List<bool[]>();

        SCDURBs_enabledState = new List<bool[]>();
        TCDURBs_enabledState = new List<bool[]>();

        CLWRRBs_enabledState = new List<bool[]>();
        CLWR2RBs_enabledState = new List<bool[]>();

        for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
        {
            xOLRBs_enabledState.Add(new bool[CentralProperties.maxLayersForMC]);
            yOLRBs_enabledState.Add(new bool[CentralProperties.maxLayersForMC]);

            xCOLRBs_enabledState.Add(new bool[CentralProperties.maxLayersForMC]);
            yCOLRBs_enabledState.Add(new bool[CentralProperties.maxLayersForMC]);

            SCDURBs_enabledState.Add(new bool[CentralProperties.maxLayersForMC]);
            TCDURBs_enabledState.Add(new bool[CentralProperties.maxLayersForMC]);

            CLWRRBs_enabledState.Add(new bool[CentralProperties.maxLayersForMC]);
            CLWR2RBs_enabledState.Add(new bool[CentralProperties.maxLayersForMC]);
        }

        experiment_table = new TableLayout();
        tabPage_2D_experiment = new TabPage {Text = "Experiment", Content = experiment_table};

        tabControl_2D_simsettings.Pages.Add(tabPage_2D_experiment);

        TableRow row0 = new();
        experiment_table.Rows.Add(row0);

        Panel p = new();
        row0.Cells.Add(new TableCell { Control = p });

        listBox_layers = new ListBox {Width = 55, DataContext = DataContext, ContextMenu = listbox_menu};
        listBox_layers.BindDataContext(c => c.DataStore, (UIStringLists m) => m.layerNames);
        listBox_layers.SelectedIndexChanged += listbox_change;

        Scrollable scrollable_twoDLayer = new();
        tabPage_2D_layer_table = new TableLayout();
        scrollable_twoDLayer.Content = tabPage_2D_layer_table;

        Splitter sp = new()
        {
            Orientation = Orientation.Horizontal,
            FixedPanel = SplitterFixedPanel.Panel1,
            Panel1 = listBox_layers,
            Panel2 = scrollable_twoDLayer
        };
        p.Content = sp;

        twoD_LayerUISetup_exp();
    }

    private void twoD_LayerUISetup_exp()
    {
        Application.Instance.Invoke(() =>
        {
            TableLayout tl = new();
            Panel p = new() {Content = tl};
            tabPage_2D_layer_table.Rows.Add(new TableRow());
            tabPage_2D_layer_table.Rows[0].Cells.Add(new TableCell { Control = p });
            tabPage_2D_layer_table.Rows.Add(new TableRow { ScaleHeight = true });

            tl.Rows.Add(new TableRow());
            TableCell topRowTC = new();
            tl.Rows[^1].Cells.Add(topRowTC);

            TableLayout tl_toprow = new();
            Panel tl_panel = new() { Content = tl_toprow };
            topRowTC.Control = tl_panel;

            layerUI_topRow_setup(tl_toprow);

            // Need to inject another table here for the LOP content.

            tl.Rows.Add(new TableRow());
            TableLayout tl_lop = new();
            Panel lop_panel = new() {Content = tl_lop};
            tl.Rows[^1].Cells.Add(new TableCell { Control = lop_panel });

            tl_lop.Rows.Add(new TableRow());

            TableCell lopTC = new();
            tl_lop.Rows[0].Cells.Add(lopTC);

            twoD_LayerUISetup_layoutOriginParameters(lopTC);

            TableCell gadgets2TC = new();
            tl_lop.Rows[0].Cells.Add(gadgets2TC);

            layerGadgets2(gadgets2TC);

            tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true }); // padding.

            // Bias etch

            tl.Rows.Add(new TableRow());
            TableCell biasEtchTC = new();
            tl.Rows[^1].Cells.Add(biasEtchTC);
            twoD_LayerUISetup_biasEtch(biasEtchTC);

            tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true }); // padding.

            // Another table needs to be injected 

            tl.Rows.Add(new TableRow());
            TableCell lithoTC = new();
            tl.Rows[^1].Cells.Add(lithoTC);
            twoD_LayerUISetup_litho(lithoTC);

            // Button export
            tl.Rows.Add(new TableRow());
            TableCell exportTC = new();
            tl.Rows[^1].Cells.Add(exportTC);

            TableLayout exportTL = new();
            Panel pExportTL = new() {Content = exportTL};
            exportTC.Control = pExportTL;
            exportTL.Rows.Add(new TableRow());

            Button exportToLayout = new() {Text = "Export to Layout"};
            exportTL.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(exportToLayout) });
            exportToLayout.Click += exportActiveLayerToLayout;

            tl.Rows[^1].Cells.Add(new TableCell { Control = null, ScaleWidth = true }); // padding.
        });
    }

    private void set_ui_from_settings(int settingsIndex)
    {
        Application.Instance.Invoke(() =>
        {
            bool alreadyFrozen = layerUIFrozen_exp;
            layerUIFrozen_exp = true;

            subShapeGBVisible[settingsIndex] = true;
            geoGBVisible[settingsIndex] = false;
            booleanGBVisible[settingsIndex] = false;

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.BOOLEAN)
            {
                booleanGBVisible[settingsIndex] = true;
                subShapeGBVisible[settingsIndex] = false;
            }

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE)
            {
                geoGBVisible[settingsIndex] = true;
                subShapeGBVisible[settingsIndex] = false;
            }

            commonVars.subShapesList_exp = commonVars.subshapes[settingsIndex];
            commonVars.structureList_exp = commonVars.structureList[settingsIndex];
            commonVars.activeStructure_LayerDataTypeList_exp = commonVars.activeStructure_LayerDataTypeList[settingsIndex];

            cB_edgeSlide.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.edgeSlide) == 1;

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.edgeSlide) == 1)
            {
                num_edgeSlideTension.Enabled = true;
            }
            else
            {
                cB_edgeSlide.Enabled = false;
            }

            cB_FlipH.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipH) == 1;

            cB_FlipV.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipV) == 1;

            cB_alignGeometryX.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.alignX) == 1;

            cB_alignGeometryY.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.alignY) == 1;

            cB_alignGeometryX.Enabled = false;
            cB_alignGeometryY.Enabled = false;
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipH) == 1)
            {
                cB_alignGeometryX.Enabled = true;
            }

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipV) == 1)
            {
                cB_alignGeometryY.Enabled = true;
            }

            text_layerName.Text = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.name);
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.enabled) == 1)
            {
                cB_Layer.Checked = true;
                cB_ShowDrawn.Enabled = true;
            }
            else
            {
                cB_Layer.Checked = false;
                cB_ShowDrawn.Enabled = false;
            }
            cB_omit[settingsIndex].Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.omit) == 1;
            cB_ShowDrawn.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.showDrawn) == 1;
            num_subshape_hl.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength);
            num_subshape_vl.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength);
            num_subshape_ho.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset);
            num_subshape_vo.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset);
            num_subshape2_hl.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength);
            num_subshape2_vl.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);
            num_subshape2_ho.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset);
            num_subshape2_vo.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset);
            num_subshape3_hl.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength);
            num_subshape3_vl.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength);
            num_subshape3_ho.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset);
            num_subshape3_vo.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset);
            try
            {
                comboBox_layerShape.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex);
            }
            catch (Exception)
            {
            }

            try
            {
                comboBox_polyFill_geoCore.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.fill);
            }
            catch (Exception)
            {
                // Don't care.
            }

            cB_geoCore_shapeEngine.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1;

            cB_geoCore_shapeEngine_perPoly.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.perPoly) == 1;

            cB_geoCore_layoutReference.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.refLayout) == 1;

            // Layout handling
            textBox_fileLocation_geoCore.Text = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.file);
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE)
            {
                if (commonVars.isCopyPrepped())
                {
                    commonVars.pasteGeoCoreHandler(settingsIndex);
                }
                try
                {
                    comboBox_structureList_geoCore.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure);
                }
                catch (Exception)
                {
                    // Don't care.
                }
                try
                {
                    comboBox_lDList_geoCore.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD);
                }
                catch (Exception)
                {
                }

                // Enable UI elements if layout is valid/loaded.
                // Block UI elements if reloaded as we don't have the layout to move to a different structure/layer-datatype.
                // Need to check state of this conditional in case 'reference' is selected.
                btn_globalApply_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
                comboBox_structureList_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid() && !commonVars.getLayerSettings(settingsIndex).isReloaded();
                comboBox_lDList_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid() && !commonVars.getLayerSettings(settingsIndex).isReloaded();
                comboBox_tipLocations_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
                num_geoCore_keyHoleSizing.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.keyhole_factor);

            }

            try
            {
                comboBox_tipLocations.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                comboBox_tipLocations_geoCore.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                comboBox_TipLocations_boolean.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape0Tip);
            }
            catch (Exception)
            {
            }

            try
            {
                num_rayExtension.Value = Convert.ToDouble(commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.rayExtension));
            }
            catch (Exception)
            {

            }

            try
            {
                comboBox_tipLocations2.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape1Tip);
            }
            catch (Exception)
            {
            }
            try
            {
                comboBox_tipLocations3.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape2Tip);
            }
            catch (Exception)
            {
            }
            try
            {
                comboBox_subShapeRef.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.subShapeIndex);
            }
            catch (Exception)
            {
            }
            try
            {
                comboBox_posSubShape.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.posIndex);
            }
            catch (Exception)
            {
            }
            num_globalHorOffset.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset);
            num_globalVerOffset.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset);
            num_rotation.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.rot);
            num_lithoWobble.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.wobble);
            num_sidebias.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.sBias);
            num_hTipbias.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.hTBias);
            num_hTipPVar.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.hTPVar);
            num_hTipNVar.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.hTNVar);
            num_vTipbias.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.vTBias);
            num_vTipPVar.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.vTPVar);
            num_vTipNVar.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.vTNVar);
            num_pitchDepBias.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.pBias);
            num_pitchDepBiasIsoDistance.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.pBiasDist);
            num_pitchDepBiasSideRays.Value = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.proxRays);
            comboBox_proxBiasFallOff.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.proxSideRaysFallOff);
            num_proxBiasFallOffMultiplier.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.proxSideRaysMultiplier);
            num_lithoICRR.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.iCR);
            num_lithoOCRR.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.oCR);
            num_lithoICV.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.iCV);
            num_lithoOCV.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.oCV);
            num_lithoLWR.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lwr);
            num_lithoLWRFreq.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lwrFreq);
            num_lithoLWR2.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lwr2);
            num_lithoLWR2Freq.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq);
            cB_layer_LWRPreview.Checked = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1;
            try
            {
                comboBox_LWRNoiseType.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lwrType);
            }
            catch (Exception)
            {
            }
            try
            {
                comboBox_LWR2NoiseType.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lwr2Type);
            }
            catch (Exception)
            {
            }

            num_coeff1.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC1);
            num_coeff2.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC2);

            num_lithoCDUSide.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.sCDU);
            num_lithoCDUTips.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.tCDU);
            num_lithoHorOverlay.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.xOL);
            num_lithoVerOverlay.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.yOL);

            if (booleanGBVisible[settingsIndex])
            {
                comboBox_BooleanOpA.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpA);
                comboBox_BooleanOpB.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpB);
                comboBox_BooleanOpAB.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpAB);
                num_rayExtension.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.rayExtension);
            }

            updateLayerRadioButtons_exp();
            updateLayerLWRRadioButtons_exp(settingsIndex);
            updateLayerLWR2RadioButtons_exp(settingsIndex);
            updateLayerCDURadioButtons_exp(settingsIndex);
            updateLayerTCDURadioButtons_exp(settingsIndex);
            updateLayerCOLXRadioButtons_exp(settingsIndex);
            updateLayerCOLYRadioButtons_exp(settingsIndex);
            updateLayerOLRXRadioButtons_exp(settingsIndex);
            updateLayerOLRYRadioButtons_exp(settingsIndex);
            updateLayerBooleanRadioButtons_exp(settingsIndex);

            comboBox_subShapeRef.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.subShapeIndex);

            // Average overlay handling
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.xOL_av) == 1)
            {
                pnl_overlayRefX.Content = gB_overlayXReference_Av;
                cB_overlayXReference_Av.Checked = true;
            }
            else
            {
                pnl_overlayRefX.Content = gB_overlayXReference;
                cB_overlayXReference_Av.Checked = false;
            }

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.yOL_av) == 1)
            {
                pnl_overlayRefY.Content = gB_overlayYReference_Av;
                cB_overlayYReference_Av.Checked = true;
            }
            else
            {
                pnl_overlayRefY.Content = gB_overlayYReference;
                cB_overlayYReference_Av.Checked = false;
            }

            updateAverageOverlayCheckboxes_exp(settingsIndex);

            cB_DOE_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
            cB_DOE_geoCore.Checked = false;
            if (commonVars.getGeoCoreHandler(settingsIndex).isValid())
            {
                if (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1)
                {
                    cB_DOE_geoCore.Checked = true;
                }
            }

            do2DLayerUI(settingsIndex, updateUI: true);
            updateGroupBoxVisibility(settingsIndex);

            layerUIFrozen_exp = alreadyFrozen;

            doShowDrawn(settingsIndex);
            viewPort.changeSettingsRef(ref mcVPSettings[settingsIndex]);
        });
    }

    private void setLayerPropertiesContent(ref Panel _control)
    {
        try
        {
            layerProperties_tc.Control = _control;
        }
        catch (Exception)
        {
        }
    }

    private void layerUI_topRow_setup(TableLayout tl)
    {
        layerShapeProperties = new Panel();
        layerShapeProperties_tcPanel = new Panel();

        tl.Rows.Add(new TableRow());
        TableCell tc0 = new();
        tl.Rows[0].Cells.Add(tc0);

        layerGadgets_setup(tc0);

        TableCell tc1 = new();

        tl.Rows[0].Cells.Add(tc1);

        layerProperties_tl = new TableLayout();
        layerProperties_tc = new TableCell();
        layerProperties_tl.Rows.Add(new TableRow());
        layerProperties_tl.Rows[0].Cells.Add(layerProperties_tc);

        layerShapeProperties.Content = layerProperties_tl;

        tc1.Control = layerShapeProperties;

        twoD_LayerUISetup_boolean();

        twoD_LayerUISetup_subShape();

        twoD_LayerUISetup_geoCore();
    }

    private void layerGadgets_setup(TableCell tc)
    {
        TableLayout layerGadgets_table = new();

        layerGadgets_table.Rows.Add(new TableRow());

        layerGadgets_row1(layerGadgets_table);
        layerGadgets_row2(layerGadgets_table);

        layerGadgets_table.Rows.Add(new TableRow());

        GroupBox gadgets_gb = new() {Text = "Layer", Content = TableLayout.AutoSized(layerGadgets_table)};
        tc.Control = gadgets_gb;
    }

    private void layerGadgets_row1(TableLayout layerGadgets_table)
    {
        // Outer table, row 1
        TableRow gadgets_tr0 = new();
        layerGadgets_table.Rows.Add(gadgets_tr0);
        gadgets_tr0.Cells.Add(new TableCell());

        // Table layout within row 1
        TableLayout row0_tl = new();
        gadgets_tr0.Cells[0].Control = row0_tl;
        row0_tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell gadgets_tr0_0 = new();
        row0_tl.Rows[0].Cells.Add(gadgets_tr0_0);

        TableLayout gadgets_tr0_0_tl = new();
        gadgets_tr0_0.Control = gadgets_tr0_0_tl;

        gadgets_tr0_0_tl.Rows.Add(new TableRow());

        cB_Layer = new CheckBox {Text = "Enabled", ToolTip = "If checked, include the layer in the simulation"};
        gadgets_tr0_0_tl.Rows[0].Cells.Add(new TableCell { Control = cB_Layer });

        text_layerName = new TextBox {ToolTip = "Layer name. If blank, the layer number will be used"};
        gadgets_tr0_0_tl.Rows[0].Cells.Add(new TableCell { Control = text_layerName });
        gadgets_tr0_0_tl.Rows[0].Cells[^1].ScaleWidth = true;
    }

    private void layerGadgets_row2(TableLayout layerGadgets_table)
    {
        // Outer table, row 2
        TableRow gadgets_tr1 = new();
        layerGadgets_table.Rows.Add(gadgets_tr1);
        gadgets_tr1.Cells.Add(new TableCell());

        // Table layout within row 2
        TableLayout row1_tl = new();
        gadgets_tr1.Cells[0].Control = row1_tl;
        row1_tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell gadgets_tr1_0 = new();
        row1_tl.Rows[0].Cells.Add(gadgets_tr1_0);

        TableLayout gadgets_tr1_0_tl = new();
        gadgets_tr1_0.Control = gadgets_tr1_0_tl;

        gadgets_tr1_0_tl.Rows.Add(new TableRow());

        comboBox_layerShape = new DropDown
        {
            DataContext = DataContext, SelectedIndex = 0, ToolTip = "Type of shape to generate"
        };
        comboBox_layerShape.BindDataContext(c => c.DataStore, (UIStringLists m) => m.shapes);
        gadgets_tr1_0_tl.Rows[0].Cells.Add(new TableCell { Control = comboBox_layerShape });
    }

    private void layerGadgets2(TableCell tc)
    {
        TableLayout layerGadgets2_table = new();
        layerGadgets2_table.Rows.Add(new TableRow());

        layerGadgets2_row1(layerGadgets2_table);

        Panel p = new() {Content = layerGadgets2_table};
        GroupBox gadgets_gb = new() {Text = "Misc", Content = p};
        tc.Control = gadgets_gb;
    }

    private void layerGadgets2_row1(TableLayout layerGadgets2_table)
    {
        // Outer table, row 2
        TableRow gadgets_tr1 = new();
        layerGadgets2_table.Rows.Add(gadgets_tr1);
        gadgets_tr1.Cells.Add(new TableCell());

        // Table layout within row 2
        TableLayout row1_tl = new();
        gadgets_tr1.Cells[0].Control = row1_tl;
        row1_tl.Rows.Add(new TableRow());

        // Table layout within cell.
        TableCell gadgets_tr1_0 = new();
        row1_tl.Rows[0].Cells.Add(gadgets_tr1_0);

        TableLayout gadgets_tr1_0_tl = new();
        gadgets_tr1_0.Control = TableLayout.AutoSized(gadgets_tr1_0_tl);

        TableLayout flipAign_tl = new();
        flipAign_tl.Rows.Add(new TableRow());

        cB_FlipH = new CheckBox {Text = "Flip H", ToolTip = "Flip shape horizontally"};
        flipAign_tl.Rows[^1].Cells.Add(new TableCell { Control = cB_FlipH });

        cB_alignGeometryX = new CheckBox
        {
            Text = "Align X", ToolTip = "Centers the flipped shape on the non-flipped shape in X"
        };
        flipAign_tl.Rows[^1].Cells.Add(new TableCell { Control = cB_alignGeometryX });

        flipAign_tl.Rows.Add(new TableRow());

        cB_FlipV = new CheckBox {Text = "Flip V", ToolTip = "Flip shape vertically"};
        flipAign_tl.Rows[^1].Cells.Add(new TableCell { Control = cB_FlipV });

        cB_alignGeometryY = new CheckBox
        {
            Text = "Align Y", ToolTip = "Centers the flipped shape on the non-flipped shape in Y"
        };
        flipAign_tl.Rows[^1].Cells.Add(new TableCell { Control = cB_alignGeometryY });

        gadgets_tr1_0_tl.Rows.Add(new TableRow());
        gadgets_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = TableLayout.AutoSized(flipAign_tl) });

        gadgets_tr1_0_tl.Rows.Add(new TableRow());

        cB_ShowDrawn = new CheckBox {Text = "Show Drawn", ToolTip = "Show drawn shape for the contour"};
        gadgets_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = cB_ShowDrawn });

        gadgets_tr1_0_tl.Rows.Add(new TableRow());

        cB_layer_LWRPreview = new CheckBox {Text = "Show LWR", ToolTip = "Preview of LWR"};
        gadgets_tr1_0_tl.Rows[^1].Cells.Add(new TableCell { Control = cB_layer_LWRPreview });
    }
}