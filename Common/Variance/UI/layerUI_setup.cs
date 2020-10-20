using Eto.Forms;
using System;
using System.Collections.Generic;

namespace Variance
{
    public partial class MainForm : Form
    {
        TableLayout layerProperties_tl;
        TableCell layerProperties_tc;

        void twoD_LayerUISetup()
        {
            rB_layer_OLRX_exp = new RadioButton[CentralProperties.maxLayersForMC];
            rB_layer_OLRY_exp = new RadioButton[CentralProperties.maxLayersForMC];

            rB_layer_COLX_exp = new RadioButton[CentralProperties.maxLayersForMC];
            rB_layer_COLY_exp = new RadioButton[CentralProperties.maxLayersForMC];

            rB_layer_CCDU_exp = new RadioButton[CentralProperties.maxLayersForMC];
            rB_layer_CTCDU_exp = new RadioButton[CentralProperties.maxLayersForMC];

            cB_layer_OLRX_Av_exp = new CheckBox[CentralProperties.maxLayersForMC];
            cB_layer_OLRY_Av_exp = new CheckBox[CentralProperties.maxLayersForMC];

            rB_layer_CLWR_exp = new RadioButton[CentralProperties.maxLayersForMC];
            rB_layer_CLWR2_exp = new RadioButton[CentralProperties.maxLayersForMC];

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
            tabPage_2D_experiment = new TabPage();
            tabPage_2D_experiment.Text = "Experiment";

            tabPage_2D_experiment.Content = experiment_table;

            tabControl_2D_simsettings.Pages.Add(tabPage_2D_experiment);

            TableRow row0 = new TableRow();
            experiment_table.Rows.Add(row0);

            Panel p = new Panel();
            row0.Cells.Add(new TableCell() { Control = p });

            experimental_listBox_layers = new ListBox();
            experimental_listBox_layers.Width = 55;
            experimental_listBox_layers.DataContext = DataContext;
            experimental_listBox_layers.BindDataContext(c => c.DataStore, (UIStringLists m) => m.layerNames);
            experimental_listBox_layers.ContextMenu = listbox_menu;

            experimental_listBox_layers.SelectedIndexChanged += listbox_change;

            Scrollable scrollable_twoDLayer = new Scrollable();
            tabPage_2D_layer_content_exp = new TableLayout();
            scrollable_twoDLayer.Content = tabPage_2D_layer_content_exp;

            Splitter sp = new Splitter
            {
                Orientation = Orientation.Horizontal,
                FixedPanel = SplitterFixedPanel.Panel1,
                Panel1 = experimental_listBox_layers,
                Panel2 = scrollable_twoDLayer,
            };
            p.Content = sp;

            twoD_LayerUISetup_exp();
        }

        void twoD_LayerUISetup_exp()
        {
            Application.Instance.Invoke(() =>
            {
                TableLayout tl = new TableLayout();
                Panel p = new Panel();
                p.Content = tl;
                tabPage_2D_layer_content_exp.Rows.Add(new TableRow());
                tabPage_2D_layer_content_exp.Rows[0].Cells.Add(new TableCell() { Control = p });
                tabPage_2D_layer_content_exp.Rows.Add(new TableRow() { ScaleHeight = true });

                tl.Rows.Add(new TableRow());
                TableCell topRowTC = new TableCell();
                tl.Rows[tl.Rows.Count - 1].Cells.Add(topRowTC);

                TableLayout tl_toprow = new TableLayout();
                Panel tl_panel = new Panel() { Content = tl_toprow };
                topRowTC.Control = tl_panel;

                layerUI_topRow_setup(tl_toprow);

                // Need to inject another table here for the LOP content.

                tl.Rows.Add(new TableRow());
                Panel lop_panel = new Panel();
                tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = lop_panel });

                TableLayout tl_lop = new TableLayout();
                lop_panel.Content = tl_lop;

                tl_lop.Rows.Add(new TableRow());

                TableCell lopTC = new TableCell();
                tl_lop.Rows[0].Cells.Add(lopTC);

                twoD_LayerUISetup_layoutOriginParameters_exp(lopTC);

                TableCell gadgets2TC = new TableCell();
                tl_lop.Rows[0].Cells.Add(gadgets2TC);

                layerGadgets2(gadgets2TC);

                tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true }); // padding.

                // Bias etch

                tl.Rows.Add(new TableRow());
                TableCell biasEtchTC = new TableCell();
                tl.Rows[tl.Rows.Count - 1].Cells.Add(biasEtchTC);
                twoD_LayerUISetup_biasEtch_exp(biasEtchTC);

                tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true }); // padding.

                // Another table needs to be injected 

                tl.Rows.Add(new TableRow());
                TableCell lithoTC = new TableCell();
                tl.Rows[tl.Rows.Count - 1].Cells.Add(lithoTC);
                twoD_LayerUISetup_litho_exp(lithoTC);

                // Button export
                tl.Rows.Add(new TableRow());
                TableCell exportTC = new TableCell();
                tl.Rows[tl.Rows.Count - 1].Cells.Add(exportTC);

                Panel pExportTL = new Panel();
                exportTC.Control = pExportTL;
                TableLayout exportTL = new TableLayout();
                pExportTL.Content = exportTL;
                exportTL.Rows.Add(new TableRow());

                Button exportToLayout = new Button();
                exportToLayout.Text = "Export to Layout";
                exportTL.Rows[exportTL.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(exportToLayout) });
                exportToLayout.Click += exportActiveLayerToLayout;

                tl.Rows[tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = null, ScaleWidth = true }); // padding.
            });
        }

        void set_ui_from_settings(int settingsIndex)
        {
            Application.Instance.Invoke(() =>
            {
                bool alreadyFrozen = layerUIFrozen_exp;
                layerUIFrozen_exp = true;

                subShapeGBVisible[settingsIndex] = true;
                geoGBVisible[settingsIndex] = false;
                booleanGBVisible[settingsIndex] = false;

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.BOOLEAN)
                {
                    booleanGBVisible[settingsIndex] = true;
                    subShapeGBVisible[settingsIndex] = false;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE)
                {
                    geoGBVisible[settingsIndex] = true;
                    subShapeGBVisible[settingsIndex] = false;
                }

                commonVars.subShapesList_exp = commonVars.subshapes[settingsIndex];
                commonVars.structureList_exp = commonVars.structureList[settingsIndex];
                commonVars.activeStructure_LayerDataTypeList_exp = commonVars.activeStructure_LayerDataTypeList[settingsIndex];

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.edgeSlide) == 1)
                {
                    checkBox_layer_edgeSlide_exp.Checked = true;
                    num_layer_edgeSlideTension_exp.Enabled = true;
                }
                else
                {
                    checkBox_layer_edgeSlide_exp.Checked = false;
                    checkBox_layer_edgeSlide_exp.Enabled = false;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipH) == 1)
                {
                    checkBox_Layer_FlipH_exp.Checked = true;
                }
                else
                {
                    checkBox_Layer_FlipH_exp.Checked = false;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipV) == 1)
                {
                    checkBox_Layer_FlipV_exp.Checked = true;
                }
                else
                {
                    checkBox_Layer_FlipV_exp.Checked = false;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.alignX) == 1)
                {
                    checkBox_Layer_alignGeometryX_exp.Checked = true;
                }
                else
                {
                    checkBox_Layer_alignGeometryX_exp.Checked = false;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.alignY) == 1)
                {
                    checkBox_Layer_alignGeometryY_exp.Checked = true;
                }
                else
                {
                    checkBox_Layer_alignGeometryY_exp.Checked = false;
                }

                checkBox_Layer_alignGeometryX_exp.Enabled = false;
                checkBox_Layer_alignGeometryY_exp.Enabled = false;
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipH) == 1)
                {
                    checkBox_Layer_alignGeometryX_exp.Enabled = true;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipV) == 1)
                {
                    checkBox_Layer_alignGeometryY_exp.Enabled = true;
                }

                text_layerName_exp.Text = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.name);
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                {
                    checkBox_Layer_exp.Checked = true;
                    checkBox_Layer_ShowDrawn_exp.Enabled = true;
                }
                else
                {
                    checkBox_Layer_exp.Checked = false;
                    checkBox_Layer_ShowDrawn_exp.Enabled = false;
                }
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.omit) == 1)
                {
                    checkBox_omit_lyr[settingsIndex].Checked = true;
                }
                else
                {
                    checkBox_omit_lyr[settingsIndex].Checked = false;
                }
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.showDrawn) == 1)
                {
                    checkBox_Layer_ShowDrawn_exp.Checked = true;
                }
                else
                {
                    checkBox_Layer_ShowDrawn_exp.Checked = false;
                }
                num_layer_subshape_hl_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength);
                num_layer_subshape_vl_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength);
                num_layer_subshape_ho_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset);
                num_layer_subshape_vo_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset);
                num_layer_subshape2_hl_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength);
                num_layer_subshape2_vl_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);
                num_layer_subshape2_ho_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset);
                num_layer_subshape2_vo_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset);
                num_layer_subshape3_hl_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength);
                num_layer_subshape3_vl_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength);
                num_layer_subshape3_ho_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset);
                num_layer_subshape3_vo_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset);
                try
                {
                    comboBox_layerShape_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex);
                }
                catch (Exception)
                {
                }

                try
                {
                    comboBox_layerPolyFill_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.fill);
                }
                catch (Exception)
                {
                    // Don't care.
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1)
                {
                    checkBox_layer_geoCore_shapeEngine_exp.Checked = true;
                }
                else
                {
                    checkBox_layer_geoCore_shapeEngine_exp.Checked = false;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.perPoly) == 1)
                {
                    checkBox_layer_geoCore_shapeEngine_perPoly_exp.Checked = true;
                }
                else
                {
                    checkBox_layer_geoCore_shapeEngine_perPoly_exp.Checked = false;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.refLayout) == 1)
                {
                    checkBox_layer_geoCore_layoutReference_exp.Checked = true;
                }
                else
                {
                    checkBox_layer_geoCore_layoutReference_exp.Checked = false;
                }

                // Layout handling
                textBox_layer_FileLocation_geoCore_exp.Text = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.file);
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE)
                {
                    if (commonVars.isCopyPrepped())
                    {
                        commonVars.pasteGeoCoreHandler(settingsIndex);
                    }
                    try
                    {
                        comboBox_layerStructureList_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure);
                    }
                    catch (Exception)
                    {
                        // Don't care.
                    }
                    try
                    {
                        comboBox_layerLDList_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD);
                    }
                    catch (Exception)
                    {
                    }

                    // Enable UI elements if layout is valid/loaded.
                    // Block UI elements if reloaded as we don't have the layout to move to a different structure/layer-datatype.
                    // Need to check state of this conditional in case 'reference' is selected.
                    button_layer_globalApply_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
                    comboBox_layerStructureList_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid() && !commonVars.getLayerSettings(settingsIndex).isReloaded();
                    comboBox_layerLDList_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid() && !commonVars.getLayerSettings(settingsIndex).isReloaded();
                    comboBox_layerTipLocations_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
                }

                try
                {
                    comboBox_layerTipLocations_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                    comboBox_layerTipLocations_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                    comboBox_layerTipLocations_boolean_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape0Tip);
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
                    comboBox_layerTipLocations2_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape1Tip);
                }
                catch (Exception)
                {
                }
                try
                {
                    comboBox_layerTipLocations3_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape2Tip);
                }
                catch (Exception)
                {
                }
                try
                {
                    comboBox_layerSubShapeRef_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.subShapeIndex);
                }
                catch (Exception)
                {
                }
                try
                {
                    comboBox_layerPosSubShape_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.posIndex);
                }
                catch (Exception)
                {
                }
                num_layerGlobalHorOffset_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset);
                num_layerGlobalVerOffset_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset);
                num_layerRotation_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.rot);
                num_layer_lithoWobble_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.wobble);
                num_layerSidebias_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.sBias);
                num_layerHTipbias_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.hTBias);
                num_layerhTipPVar_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.hTPVar);
                num_layerhTipNVar_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.hTNVar);
                num_layerVTipbias_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.vTBias);
                num_layervTipPVar_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.vTPVar);
                num_layervTipNVar_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.vTNVar);
                num_pitchDepBias_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.pBias);
                num_pitchDepBiasIsoDistance_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.pBiasDist);
                num_pitchDepBiasSideRays_exp.Value = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.proxRays);
                comboBox_proxBiasFallOff.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.proxSideRaysFallOff);
                num_proxBiasFallOffMultiplier.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.proxSideRaysMultiplier);
                num_layer_lithoICRR_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.iCR);
                num_layer_lithoOCRR_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.oCR);
                num_layer_lithoICV_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.iCV);
                num_layer_lithoOCV_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.oCV);
                num_layer_lithoLWR_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lwr);
                num_layer_lithoLWRFreq_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lwrFreq);
                num_layer_lithoLWR2_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lwr2);
                num_layer_lithoLWR2Freq_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq);
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1)
                {
                    cB_layer_LWRPreview_exp.Checked = true;
                }
                else
                {
                    cB_layer_LWRPreview_exp.Checked = false;
                }
                try
                {
                    comboBox_layerLWRNoiseType_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lwrType);
                }
                catch (Exception)
                {
                }
                try
                {
                    comboBox_layerLWR2NoiseType_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lwr2Type);
                }
                catch (Exception)
                {
                }

                num_layer_coeff1_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC1);
                num_layer_coeff2_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC2);

                num_layer_lithoCDUSide_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.sCDU);
                num_layer_lithoCDUTips_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.tCDU);
                num_layer_lithoHorOverlay_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.xOL);
                num_layer_lithoVerOverlay_exp.Value = (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.yOL);

                if (booleanGBVisible[settingsIndex])
                {
                    comboBox_layerBooleanOpA_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpA);
                    comboBox_layerBooleanOpB_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpB);
                    comboBox_layerBooleanOpAB_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpAB);
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

                comboBox_layerSubShapeRef_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.subShapeIndex);

                // Average overlay handling
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.xOL_av) == 1)
                {
                    pnl_overlayRefX.Content = groupBox_layer_overlayXReference_Av_exp;
                    checkBox_layer_overlayXReference_Av_exp.Checked = true;
                }
                else
                {
                    pnl_overlayRefX.Content = groupBox_layer_overlayXReference_exp;
                    checkBox_layer_overlayXReference_Av_exp.Checked = false;
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.yOL_av) == 1)
                {
                    pnl_overlayRefY.Content = groupBox_layer_overlayYReference_Av_exp;
                    checkBox_layer_overlayYReference_Av_exp.Checked = true;
                }
                else
                {
                    pnl_overlayRefY.Content = groupBox_layer_overlayYReference_exp;
                    checkBox_layer_overlayYReference_Av_exp.Checked = false;
                }

                updateAverageOverlayCheckboxes_exp(settingsIndex);

                checkBox_DOELayer_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
                checkBox_DOELayer_geoCore_exp.Checked = false;
                if (commonVars.getGeoCoreHandler(settingsIndex).isValid())
                {
                    if (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1)
                    {
                        checkBox_DOELayer_geoCore_exp.Checked = true;
                    }
                }

                do2DLayerUI_exp(settingsIndex, updateUI: true);
                updateGroupBoxVisibility_exp(settingsIndex);

                layerUIFrozen_exp = alreadyFrozen;

                doShowDrawn(settingsIndex);
                viewPort.changeSettingsRef(ref mcVPSettings[settingsIndex]);
            });
        }

        void setLayerPropertiesContent(ref Panel _control)
        {
            try
            {
                layerProperties_tc.Control = _control;
            }
            catch (Exception)
            {
            }
        }

        void layerUI_topRow_setup(TableLayout tl)
        {
            layerShapeProperties = new Panel();
            layerShapeProperties_tcPanel = new Panel();

            tl.Rows.Add(new TableRow());
            TableCell tc0 = new TableCell();
            tl.Rows[0].Cells.Add(tc0);

            layerGadgets_setup(tc0);

            TableCell tc1 = new TableCell();

            tl.Rows[0].Cells.Add(tc1);

            layerProperties_tl = new TableLayout();
            layerProperties_tc = new TableCell();
            layerProperties_tl.Rows.Add(new TableRow());
            layerProperties_tl.Rows[0].Cells.Add(layerProperties_tc);

            layerShapeProperties.Content = layerProperties_tl;

            tc1.Control = layerShapeProperties;

            twoD_LayerUISetup_boolean_exp();

            twoD_LayerUISetup_subShape_exp();

            twoD_LayerUISetup_geoCore_exp();
        }

        void layerGadgets_setup(TableCell tc)
        {
            TableLayout layerGadgets_table = new TableLayout();

            layerGadgets_table.Rows.Add(new TableRow());

            layerGadgets_row1(layerGadgets_table);
            layerGadgets_row2(layerGadgets_table);
            layerGadgets_row3(layerGadgets_table);

            layerGadgets_table.Rows.Add(new TableRow());

            Panel p = new Panel();
            p.Content = TableLayout.AutoSized(layerGadgets_table);
            tc.Control = p;
        }

        void layerGadgets_row1(TableLayout layerGadgets_table)
        {
            // Outer table, row 1
            TableRow gadgets_tr0 = new TableRow();
            layerGadgets_table.Rows.Add(gadgets_tr0);
            gadgets_tr0.Cells.Add(new TableCell());

            // Table layout within row 1
            TableLayout row0_tl = new TableLayout();
            gadgets_tr0.Cells[0].Control = row0_tl;
            row0_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell gadgets_tr0_0 = new TableCell();
            row0_tl.Rows[0].Cells.Add(gadgets_tr0_0);

            TableLayout gadgets_tr0_0_tl = new TableLayout();
            gadgets_tr0_0.Control = gadgets_tr0_0_tl;

            gadgets_tr0_0_tl.Rows.Add(new TableRow());

            checkBox_Layer_exp = new CheckBox();
            checkBox_Layer_exp.Text = "Enabled";
            checkBox_Layer_exp.ToolTip = "If checked, include the layer in the simulation";
            gadgets_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = checkBox_Layer_exp });

            text_layerName_exp = new TextBox();
            text_layerName_exp.ToolTip = "Layer name. If blank, the layer number will be used";
            gadgets_tr0_0_tl.Rows[0].Cells.Add(new TableCell() { Control = text_layerName_exp });
            gadgets_tr0_0_tl.Rows[0].Cells[gadgets_tr0_0_tl.Rows[0].Cells.Count - 1].ScaleWidth = true;
        }

        void layerGadgets_row2(TableLayout layerGadgets_table)
        {
            // Outer table, row 2
            TableRow gadgets_tr1 = new TableRow();
            layerGadgets_table.Rows.Add(gadgets_tr1);
            gadgets_tr1.Cells.Add(new TableCell());

            // Table layout within row 2
            TableLayout row1_tl = new TableLayout();
            gadgets_tr1.Cells[0].Control = row1_tl;
            row1_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell gadgets_tr1_0 = new TableCell();
            row1_tl.Rows[0].Cells.Add(gadgets_tr1_0);

            TableLayout gadgets_tr1_0_tl = new TableLayout();
            gadgets_tr1_0.Control = gadgets_tr1_0_tl;

            gadgets_tr1_0_tl.Rows.Add(new TableRow());

            comboBox_layerShape_exp = new DropDown();
            comboBox_layerShape_exp.DataContext = DataContext;
            comboBox_layerShape_exp.BindDataContext(c => c.DataStore, (UIStringLists m) => m.shapes);
            comboBox_layerShape_exp.SelectedIndex = 0;
            comboBox_layerShape_exp.ToolTip = "Type of shape to generate";
            gadgets_tr1_0_tl.Rows[0].Cells.Add(new TableCell() { Control = comboBox_layerShape_exp });
        }

        void layerGadgets_row3(TableLayout layerGadgets_table)
        {
            // Outer table, row 3
            TableRow gadgets_tr2 = new TableRow();
            layerGadgets_table.Rows.Add(gadgets_tr2);
            gadgets_tr2.Cells.Add(new TableCell());

            // Table layout within row 3
            TableLayout row2_tl = new TableLayout();
            gadgets_tr2.Cells[0].Control = row2_tl;
            row2_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell gadgets_tr2_0 = new TableCell();
            row2_tl.Rows[0].Cells.Add(gadgets_tr2_0);

            TableLayout gadgets_tr2_0_tl = new TableLayout();
            gadgets_tr2_0.Control = gadgets_tr2_0_tl;

            gadgets_tr2_0_tl.Rows.Add(new TableRow());

            checkBox_layer_edgeSlide_exp = new CheckBox();
            checkBox_layer_edgeSlide_exp.Text = "Edge Slide";
            checkBox_layer_edgeSlide_exp.Width = 90;
            checkBox_layer_edgeSlide_exp.ToolTip = "If checked, apply tension to each edge for the contour generation";
            gadgets_tr2_0_tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(checkBox_layer_edgeSlide_exp) });
            if (EntropyLayerSettings.getDefaultInt(EntropyLayerSettings.properties_i.edgeSlide) == 1)
            {
                checkBox_layer_edgeSlide_exp.Checked = true;
            }

            lbl_layer_edgeSlideTension_exp = new Label();
            lbl_layer_edgeSlideTension_exp.Text = "Tension";
            lbl_layer_edgeSlideTension_exp.Width = 50;
            lbl_layer_edgeSlideTension_exp.ToolTip = "Amount of tension to apply, to pull the midpoint towards the longer edge";
            gadgets_tr2_0_tl.Rows[0].Cells.Add(new TableCell() { Control = lbl_layer_edgeSlideTension_exp });
            gadgets_tr2_0_tl.Rows[0].Cells[gadgets_tr2_0_tl.Rows[0].Cells.Count - 1].ScaleWidth = true;

            num_layer_edgeSlideTension_exp = new NumericStepper();
            num_layer_edgeSlideTension_exp.DecimalPlaces = 2;
            num_layer_edgeSlideTension_exp.Increment = 0.1;
            num_layer_edgeSlideTension_exp.Value = (double)EntropyLayerSettings.getDefaultDecimal(EntropyLayerSettings.properties_decimal.eTension);
            num_layer_edgeSlideTension_exp.MinValue = 0.01;
            setSize(num_layer_edgeSlideTension_exp, 55, num_Height);
            num_layer_edgeSlideTension_exp.ToolTip = "Amount of tension to apply, to pull the midpoint towards the longer edge";
            gadgets_tr2_0_tl.Rows[0].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(num_layer_edgeSlideTension_exp) });
            num_layer_edgeSlideTension_exp.Enabled = true;

            if (EntropyLayerSettings.getDefaultInt(EntropyLayerSettings.properties_i.edgeSlide) == 0)
            {
                num_layer_edgeSlideTension_exp.Enabled = false;
            }
        }

        void layerGadgets2(TableCell tc)
        {
            TableLayout layerGadgets2_table = new TableLayout();
            layerGadgets2_table.Rows.Add(new TableRow());

            layerGadgets2_row1(layerGadgets2_table);

            Panel p = new Panel();
            p.Content = layerGadgets2_table;
            tc.Control = p;
        }

        void layerGadgets2_row1(TableLayout layerGadgets2_table)
        {
            // Outer table, row 2
            TableRow gadgets_tr1 = new TableRow();
            layerGadgets2_table.Rows.Add(gadgets_tr1);
            gadgets_tr1.Cells.Add(new TableCell());

            // Table layout within row 2
            TableLayout row1_tl = new TableLayout();
            gadgets_tr1.Cells[0].Control = row1_tl;
            row1_tl.Rows.Add(new TableRow());

            // Table layout within cell.
            TableCell gadgets_tr1_0 = new TableCell();
            row1_tl.Rows[0].Cells.Add(gadgets_tr1_0);

            TableLayout gadgets_tr1_0_tl = new TableLayout();
            gadgets_tr1_0.Control = TableLayout.AutoSized(gadgets_tr1_0_tl);

            TableLayout flipAign_tl = new TableLayout();
            flipAign_tl.Rows.Add(new TableRow());

            checkBox_Layer_FlipH_exp = new CheckBox();
            checkBox_Layer_FlipH_exp.Text = "Flip H";
            checkBox_Layer_FlipH_exp.ToolTip = "Flip shape horizontally";
            flipAign_tl.Rows[flipAign_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_Layer_FlipH_exp });

            checkBox_Layer_alignGeometryX_exp = new CheckBox();
            checkBox_Layer_alignGeometryX_exp.Text = "Align X";
            checkBox_Layer_alignGeometryX_exp.ToolTip = "Centers the flipped shape on the non-flipped shape in X";
            flipAign_tl.Rows[flipAign_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_Layer_alignGeometryX_exp });

            flipAign_tl.Rows.Add(new TableRow());

            checkBox_Layer_FlipV_exp = new CheckBox();
            checkBox_Layer_FlipV_exp.Text = "Flip V";
            checkBox_Layer_FlipV_exp.ToolTip = "Flip shape vertically";
            flipAign_tl.Rows[flipAign_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_Layer_FlipV_exp });

            checkBox_Layer_alignGeometryY_exp = new CheckBox();
            checkBox_Layer_alignGeometryY_exp.Text = "Align Y";
            checkBox_Layer_alignGeometryY_exp.ToolTip = "Centers the flipped shape on the non-flipped shape in Y";
            flipAign_tl.Rows[flipAign_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_Layer_alignGeometryY_exp });

            gadgets_tr1_0_tl.Rows.Add(new TableRow());
            gadgets_tr1_0_tl.Rows[gadgets_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = TableLayout.AutoSized(flipAign_tl) });

            gadgets_tr1_0_tl.Rows.Add(new TableRow());

            checkBox_Layer_ShowDrawn_exp = new CheckBox();
            checkBox_Layer_ShowDrawn_exp.Text = "Show Drawn";
            checkBox_Layer_ShowDrawn_exp.ToolTip = "Show drawn shape for the contour";
            gadgets_tr1_0_tl.Rows[gadgets_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = checkBox_Layer_ShowDrawn_exp });

            gadgets_tr1_0_tl.Rows.Add(new TableRow());

            cB_layer_LWRPreview_exp = new CheckBox();
            cB_layer_LWRPreview_exp.Text = "Show LWR";
            cB_layer_LWRPreview_exp.ToolTip = "Preview of LWR";
            gadgets_tr1_0_tl.Rows[gadgets_tr1_0_tl.Rows.Count - 1].Cells.Add(new TableCell() { Control = cB_layer_LWRPreview_exp });
        }
    }
}