using color;
using Eto.Drawing;
using Eto.Forms;
using geoLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Variance
{
    public partial class MainForm : Form
    {
        int getMainSelectedIndex()
        {
            Int32 tmp = -1;

            Application.Instance.Invoke(() =>
            {
                tmp = tabControl_main.SelectedIndex;
            });

            return tmp;
        }

        void setMainSelectedIndex(int index)
        {
            Application.Instance.Invoke(() =>
            {
                tabControl_main.SelectedIndex = index;
            });
        }

        int getSubTabSelectedIndex()
        {
            Int32 tmp = -1;

            Application.Instance.Invoke(() =>
            {
                tmp = tabControl_2D_simsettings.SelectedIndex;
            });

            return tmp;
        }

        void setSubTabSelectedIndex(int index)
        {
            Application.Instance.Invoke(() =>
            {
                tabControl_2D_simsettings.SelectedIndex = index;
            });
        }

        int getSelectedLayerIndex()
        {
            return selectedLayer;
        }

        void set2DSelectedIndex(int index)
        {
            Application.Instance.Invoke(() =>
            {
                selectedLayer = index;
                experimental_listBox_layers.SelectedIndex = index;
            });
        }

        void goToLayerUI(int index)
        {
            setMainSelectedIndex((Int32)CommonVars.upperTabNames.twoD);
            setSubTabSelectedIndex((Int32)CommonVars.twoDTabNames.layer);
            set2DSelectedIndex(index);
            set_ui_from_settings(index);
        }

        void layer_clampSubShape_exp(double minHLength, double maxHLength, double minVLength, double maxVLength, double minHOffset, double maxHOffset, double minVOffset, double maxVOffset)
        {
            Application.Instance.Invoke(() =>
            {
                if (num_layer_subshape_hl_exp.Value < minHLength)
                {
                    num_layer_subshape_hl_exp.Value = minHLength;
                }

                if (num_layer_subshape_hl_exp.Value > maxHLength)
                {
                    num_layer_subshape_hl_exp.Value = maxHLength;
                }

                if (num_layer_subshape_vl_exp.Value < minVLength)
                {
                    num_layer_subshape_vl_exp.Value = minVLength;
                }

                if (num_layer_subshape_vl_exp.Value > maxVLength)
                {
                    num_layer_subshape_vl_exp.Value = maxVLength;
                }

                if (num_layer_subshape_ho_exp.Value < minHOffset)
                {
                    num_layer_subshape_ho_exp.Value = minHOffset;
                }

                if (num_layer_subshape_ho_exp.Value > maxHOffset)
                {
                    num_layer_subshape_ho_exp.Value = maxHOffset;
                }

                if (num_layer_subshape_vo_exp.Value < minVOffset)
                {
                    num_layer_subshape_vo_exp.Value = minVOffset;
                }

                if (num_layer_subshape_vo_exp.Value > maxVOffset)
                {
                    num_layer_subshape_vo_exp.Value = maxVOffset;
                }
            });
        }

        void layer_clampSubShape2_exp(double minHLength, double maxHLength, double minVLength, double maxVLength, double minHOffset, double maxHOffset, double minVOffset, double maxVOffset)
        {
            Application.Instance.Invoke(() =>
            {
                if (num_layer_subshape2_hl_exp.Value < minHLength)
                {
                    num_layer_subshape2_hl_exp.Value = minHLength;
                }

                if (num_layer_subshape2_hl_exp.Value > maxHLength)
                {
                    num_layer_subshape2_hl_exp.Value = maxHLength;
                }

                if (num_layer_subshape2_vl_exp.Value < minVLength)
                {
                    num_layer_subshape2_vl_exp.Value = minVLength;
                }

                if (num_layer_subshape2_vl_exp.Value > maxVLength)
                {
                    num_layer_subshape2_vl_exp.Value = maxVLength;
                }

                if (num_layer_subshape2_ho_exp.Value < minHOffset)
                {
                    num_layer_subshape2_ho_exp.Value = minHOffset;
                }

                if (num_layer_subshape2_ho_exp.Value > maxHOffset)
                {
                    num_layer_subshape2_ho_exp.Value = maxHOffset;
                }

                if (num_layer_subshape2_vo_exp.Value < minVOffset)
                {
                    num_layer_subshape2_vo_exp.Value = minVOffset;
                }

                if (num_layer_subshape2_vo_exp.Value > maxVOffset)
                {
                    num_layer_subshape2_vo_exp.Value = maxVOffset;
                }
            });
        }

        void layer_clampSubShape3_exp(double minHLength, double maxHLength, double minVLength, double maxVLength, double minHOffset, double maxHOffset, double minVOffset, double maxVOffset)
        {
            Application.Instance.Invoke(() =>
            {
                if (num_layer_subshape3_hl_exp.Value < minHLength)
                {
                    num_layer_subshape3_hl_exp.Value = minHLength;
                }

                if (num_layer_subshape3_hl_exp.Value > maxHLength)
                {
                    num_layer_subshape3_hl_exp.Value = maxHLength;
                }

                if (num_layer_subshape3_vl_exp.Value < minVLength)
                {
                    num_layer_subshape3_vl_exp.Value = minVLength;
                }

                if (num_layer_subshape3_vl_exp.Value > maxVLength)
                {
                    num_layer_subshape3_vl_exp.Value = maxVLength;
                }

                if (num_layer_subshape3_ho_exp.Value < minHOffset)
                {
                    num_layer_subshape3_ho_exp.Value = minHOffset;
                }

                if (num_layer_subshape3_ho_exp.Value > maxHOffset)
                {
                    num_layer_subshape3_ho_exp.Value = maxHOffset;
                }

                if (num_layer_subshape3_vo_exp.Value < minVOffset)
                {
                    num_layer_subshape3_vo_exp.Value = minVOffset;
                }

                if (num_layer_subshape3_vo_exp.Value > maxVOffset)
                {
                    num_layer_subshape3_vo_exp.Value = maxVOffset;
                }
            });
        }

        void addLayerHandlers_exp()
        {
            layerUIFrozen_exp = false;
            button_layer_globalApply_geoCore_exp.Click += applyLayoutToAll;

            checkBox_Layer_exp.CheckedChanged += twoDLayerEventHandler_exp;
            text_layerName_exp.LostFocus += twoDLayerEventHandler_exp;

            checkBox_layer_edgeSlide_exp.CheckedChanged += twoDLayerEventHandler_exp;
            num_layer_edgeSlideTension_exp.LostFocus += twoDLayerEventHandler_exp;

            checkBox_Layer_FlipH_exp.CheckedChanged += twoDLayerEventHandler_exp;
            checkBox_Layer_FlipV_exp.CheckedChanged += twoDLayerEventHandler_exp;
            checkBox_Layer_alignGeometryX_exp.CheckedChanged += twoDLayerEventHandler_exp;
            checkBox_Layer_alignGeometryY_exp.CheckedChanged += twoDLayerEventHandler_exp;

            comboBox_layerShape_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_layerSubShapeRef_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_layerPosSubShape_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_layerGlobalHorOffset_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layerGlobalVerOffset_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layer_subshape_hl_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape_vl_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape_ho_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape_vo_exp.LostFocus += twoDLayerEventHandler_exp;
            comboBox_layerTipLocations_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_layer_subshape2_hl_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape2_vl_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape2_ho_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape2_vo_exp.LostFocus += twoDLayerEventHandler_exp;
            comboBox_layerTipLocations2_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_layer_subshape3_hl_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape3_vl_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape3_ho_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_subshape3_vo_exp.LostFocus += twoDLayerEventHandler_exp;
            comboBox_layerTipLocations3_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_layer_lithoLWR_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_lithoLWRFreq_exp.LostFocus += twoDLayerEventHandler_exp;
            comboBox_layerLWRNoiseType_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            num_layer_lithoLWR2_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_lithoLWR2Freq_exp.LostFocus += twoDLayerEventHandler_exp;
            comboBox_layerLWR2NoiseType_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            cB_layer_LWRPreview_exp.CheckedChanged += twoDLayerEventHandler_exp;

            num_layerSidebias_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layer_lithoCDUTips_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_lithoCDUSide_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layerHTipbias_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layerVTipbias_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layerhTipPVar_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layerhTipNVar_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layervTipPVar_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layervTipNVar_exp.LostFocus += twoDLayerEventHandler_exp;

            num_pitchDepBias_exp.LostFocus += twoDLayerEventHandler_exp;
            num_pitchDepBiasIsoDistance_exp.LostFocus += twoDLayerEventHandler_exp;
            num_pitchDepBiasSideRays_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layer_lithoICRR_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_lithoICV_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_lithoOCRR_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_lithoOCV_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layer_lithoHorOverlay_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_lithoVerOverlay_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layerRotation_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_lithoWobble_exp.LostFocus += twoDLayerEventHandler_exp;

            num_layer_coeff1_exp.LostFocus += twoDLayerEventHandler_exp;
            num_layer_coeff2_exp.LostFocus += twoDLayerEventHandler_exp;

            button_layer_chooseFile_geoCore_exp.Click += geoFileChooser_Handler_exp;
            comboBox_layerLDList_geoCore_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_layerStructureList_geoCore_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_layerPolyFill_geoCore_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            checkBox_layer_geoCore_shapeEngine_exp.CheckedChanged += twoDLayerEventHandler_exp;
            checkBox_layer_geoCore_shapeEngine_perPoly_exp.CheckedChanged += twoDLayerEventHandler_exp;
            comboBox_layerTipLocations_geoCore_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            checkBox_layer_geoCore_layoutReference_exp.CheckedChanged += twoDLayerEventHandler_exp;
            checkBox_DOELayer_geoCore_exp.CheckedChanged += twoDLayerEventHandler_exp;

            checkBox_Layer_ShowDrawn_exp.CheckedChanged += showDrawn_exp;

            comboBox_layerBooleanOpA_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_layerBooleanOpB_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_layerBooleanOpAB_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_layerTipLocations_boolean_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                rB_layer_COLX_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layer_COLY_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layer_OLRX_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layer_OLRY_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layer_CLWR_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layer_CLWR2_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layer_CCDU_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layer_CTCDU_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layerBooleanA_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_layerBooleanB_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                cB_layer_OLRX_Av_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                cB_layer_OLRY_Av_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
            }

            checkBox_layer_overlayXReference_Av_exp.CheckedChanged += twoDLayerEventHandler_exp;
            checkBox_layer_overlayYReference_Av_exp.CheckedChanged += twoDLayerEventHandler_exp;
        }

        void do2DLayerUI_exp(int settingsIndex, bool updateUI = false)
        {
            Application.Instance.Invoke(() =>
            {
                if (layerUIFrozen_exp)
                {
                    return;
                }

                suspendLayerUI_exp();

                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shapeIndex, comboBox_layerShape_exp.SelectedIndex);

                // Set our tab name.
                if (text_layerName_exp.Text != "")
                {
                    commonVars.getLayerSettings(settingsIndex).setString(EntropyLayerSettings.properties_s.name, text_layerName_exp.Text);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setString(EntropyLayerSettings.properties_s.name, (settingsIndex + 1).ToString());
                }
                // We have to deregister the handler here otherwise the name update causes the selectedindexchanged event to fire and everything breaks.
                experimental_listBox_layers.SelectedIndexChanged -= listbox_change;
                commonVars.layerNames[settingsIndex] = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.name);
                experimental_listBox_layers.SelectedIndexChanged += listbox_change;

                experimental_listBox_layers.SelectedIndex = selectedLayer;

                if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE) &&
                    !((bool)checkBox_layer_geoCore_shapeEngine_exp.Checked))
                {
                    checkBox_Layer_FlipH_exp.Checked = false;
                    checkBox_Layer_FlipV_exp.Checked = false;
                    checkBox_Layer_alignGeometryX_exp.Checked = false;
                    checkBox_Layer_alignGeometryY_exp.Checked = false;

                    checkBox_Layer_FlipH_exp.Enabled = false;
                    checkBox_Layer_FlipV_exp.Enabled = false;
                    checkBox_Layer_alignGeometryX_exp.Enabled = false;
                    checkBox_Layer_alignGeometryY_exp.Enabled = false;

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipH, 0);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipV, 0);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignX, 0);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignY, 0);
                }
                else
                {
                    checkBox_Layer_FlipH_exp.Enabled = true;
                    checkBox_Layer_FlipV_exp.Enabled = true;

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipH, 0);
                    if ((bool)checkBox_Layer_FlipH_exp.Checked)
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipH, 1);
                    }

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipV, 0);
                    if ((bool)checkBox_Layer_FlipV_exp.Checked)
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipV, 1);
                    }

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignX, 0);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignY, 0);
                    checkBox_Layer_alignGeometryX_exp.Enabled = false;
                    checkBox_Layer_alignGeometryY_exp.Enabled = false;
                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipH) == 1)
                    {
                        checkBox_Layer_alignGeometryX_exp.Enabled = true;
                        if (checkBox_Layer_alignGeometryX_exp.Checked == true)
                        {
                            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignX, 1);
                        }
                    }

                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipV) == 1)
                    {
                        checkBox_Layer_alignGeometryY_exp.Enabled = true;
                        if (checkBox_Layer_alignGeometryY_exp.Checked == true)
                        {
                            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignY, 1);
                        }
                    }
                }

                do2DLayerUI_edgeSlide_exp(settingsIndex);

                Int32 previousIndex = comboBox_layerSubShapeRef_exp.SelectedIndex;

                if (updateUI)
                {
                    set_shape_type_ui(settingsIndex);
                }
                if (previousIndex >= commonVars.subshapes[settingsIndex].Count)
                {
                    previousIndex = commonVars.subshapes[settingsIndex].Count - 1;
                }

                comboBox_layerSubShapeRef_exp.SelectedIndex = previousIndex;

                comboBox_geoEqtn_Op[settingsIndex].Enabled = false;

                // Enable control requires various checks to ensure it is appropriate. These complicated checks are below and commented.
                if (
                    (
                     // Avoid offering enable options for zero length and height and no shape defined, with a wrapping check in case of layout (where zero height/length are permitted)
                     (num_layer_subshape_hl_exp.Value != 0) &&
                     (num_layer_subshape_vl_exp.Value != 0) &&
                     ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.GEOCORE) && (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.none))
                    ) ||
                    (
                     // Allow enable for geoCore with valid input.
                     commonVars.getGeoCoreHandler(settingsIndex).isValid() && (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE)
                    ) ||
                    (
                     // Allow enable for geoCore with reloaded data.
                     commonVars.getLayerSettings(settingsIndex).isReloaded() && (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE)
                    ) ||
                    (
                     // Allow enable for Boolean type - should probably check input fields here, but we don't for now.
                     (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.BOOLEAN)
                    )
                   )
                {
                    // Also tweak 'show drawn' here.
                    checkBox_Layer_exp.Enabled = true;

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.showDrawn, 0);
                    if (checkBox_Layer_exp.Checked == true)
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.enabled, 1);
                        comboBox_geoEqtn_Op[settingsIndex].Enabled = true;
                        checkBox_Layer_ShowDrawn_exp.Enabled = true;
                        if (checkBox_Layer_ShowDrawn_exp.Checked == true)
                        {
                            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.showDrawn, 1);
                        }
                    }
                    else
                    {
                        checkBox_Layer_ShowDrawn_exp.Enabled = false;
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.enabled, 0);
                    }
                }
                else
                {
                    checkBox_Layer_exp.Checked = false;
                    checkBox_Layer_exp.Enabled = false;
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.enabled, 0);
                    checkBox_Layer_ShowDrawn_exp.Enabled = false;
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.showDrawn, 0);
                }

                doSimSettingsCheck();
                startButtonCheck();

                textBox_layer_FileLocation_geoCore_exp.Text = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.file);

                comboBox_layerSubShapeRef_exp.Enabled = true;
                comboBox_layerPosSubShape_exp.Enabled = true;

                num_layerHTipbias_exp.Enabled = true;
                num_layerVTipbias_exp.Enabled = true;
                num_layerhTipNVar_exp.Enabled = true;
                num_layerhTipPVar_exp.Enabled = true;
                num_layervTipNVar_exp.Enabled = true;
                num_layervTipPVar_exp.Enabled = true;

                num_layer_lithoICRR_exp.Enabled = true;
                num_layer_lithoICV_exp.Enabled = true;
                num_layer_lithoCDUSide_exp.Enabled = true;
                num_layer_lithoCDUTips_exp.Enabled = true;
                groupBox_layer_CDUCorrelation_exp.Enabled = true;
                groupBox_layer_TipCDUCorrelation_exp.Enabled = true;
                num_layer_lithoLWR_exp.Enabled = true;
                num_layer_lithoLWRFreq_exp.Enabled = true;
                cB_layer_LWRPreview_exp.Enabled = true;
                comboBox_layerLWRNoiseType_exp.Enabled = true;
                num_layer_lithoOCRR_exp.Enabled = true;
                num_layer_lithoOCV_exp.Enabled = true;
                num_layerRotation_exp.Enabled = true;
                num_layer_lithoWobble_exp.Enabled = true;

                if (!commonVars.getLayerSettings(settingsIndex).isReloaded())
                {
                    // We do this to avoid any misfires later. Might not be needed, but seems safer.
                    GeoLibPointF[] defaultPointArray = new GeoLibPointF[1];
                    defaultPointArray[0] = new GeoLibPointF(0, 0);
                    commonVars.getLayerSettings(settingsIndex).setFileData(new List<GeoLibPointF[]>() { defaultPointArray });
                }

                if ((bool)checkBox_layer_geoCore_shapeEngine_exp.Checked)
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.gCSEngine, 1);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.gCSEngine, 0);
                }

                if ((bool)checkBox_layer_geoCore_shapeEngine_perPoly_exp.Checked)
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.perPoly, 1);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.perPoly, 0);
                }

                if ((bool)checkBox_layer_geoCore_layoutReference_exp.Checked)
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.refLayout, 1);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.refLayout, 0);
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE) // layout option selected.
                {
                    do2DLayerUI_geoCore_exp(settingsIndex);
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.BOOLEAN)
                {
                    do2DLayerUI_Boolean_exp(settingsIndex);
                }

                layer_clampSubShape_exp(minHLength: 0, maxHLength: 1000000, minVLength: 0, maxVLength: 1000000, minHOffset: -1000000, maxHOffset: 1000000, minVOffset: -1000000, maxVOffset: 1000000);

                bool[] warnArray = new bool[] { false, false, false };

                if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.none) || (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.rect))
                {
                    layer_clampSubShape2_exp(minHLength: 0, 
                        maxHLength: 1000000, 
                        minVLength: 0, 
                        maxVLength: 1000000, 
                        minHOffset: -1000000, 
                        maxHOffset: 1000000, 
                        minVOffset: -1000000, 
                        maxVOffset: 1000000
                    );
                    layer_clampSubShape3_exp(minHLength: 0, 
                        maxHLength: 1000000, 
                        minVLength: 0, 
                        maxVLength: 1000000, 
                        minHOffset: -1000000, 
                        maxHOffset: 1000000, 
                        minVOffset: -1000000, 
                        maxVOffset: 1000000
                    );

                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.none)
                    {
                        num_layer_subshape_hl_exp.Value = 0;
                        num_layer_subshape_vl_exp.Value = 0;
                        num_layer_subshape_ho_exp.Value = 0;
                        num_layer_subshape_vo_exp.Value = 0;
                    }

                    num_layer_subshape2_hl_exp.Value = 0;
                    num_layer_subshape2_vl_exp.Value = 0;
                    num_layer_subshape2_ho_exp.Value = 0;
                    num_layer_subshape2_vo_exp.Value = 0;

                    num_layer_subshape3_hl_exp.Value = 0;
                    num_layer_subshape3_vl_exp.Value = 0;
                    num_layer_subshape3_ho_exp.Value = 0;
                    num_layer_subshape3_vo_exp.Value = 0;

                }

                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_layer_subshape_hl_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_layer_subshape_ho_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_layer_subshape_vl_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_layer_subshape_vo_exp.Value));

                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_layer_subshape2_hl_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_layer_subshape2_ho_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_layer_subshape2_vl_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_layer_subshape2_vo_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape1Tip, comboBox_layerTipLocations2_exp.SelectedIndex);

                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_layer_subshape3_hl_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_layer_subshape3_ho_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_layer_subshape3_vl_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_layer_subshape3_vo_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape2Tip, comboBox_layerTipLocations3_exp.SelectedIndex);

                warnArray[0] = ((commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) == 0) || (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) == 0));

                // Boolean and geoCore have their own handling for this.
                if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.GEOCORE) && (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.BOOLEAN))
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape0Tip, comboBox_layerTipLocations_exp.SelectedIndex);
                }

                // Subshape 2 offsets contingent on shape selection choice
                if (
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.none) &&
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.rect) &&
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.GEOCORE) &&
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.BOOLEAN)
                   )
                {
                    layer_clampSubShape_exp(minHLength: 0.01, maxHLength: 1000000, minVLength: 0.01, maxVLength: 1000000, minHOffset: -1000000, maxHOffset: 1000000, minVOffset: -1000000, maxVOffset: 1000000);

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape1Tip, comboBox_layerTipLocations2_exp.SelectedIndex);
                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Xshape) // Limit offsets of subshape 2 for X-shape.
                    {
                        do2DLayerUI_X_exp(settingsIndex);
                    }
                    else if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Tshape) // Disabled horizontal offset of subshape 2 for T-shape.
                    {
                        do2DLayerUI_T_exp(settingsIndex);
                    }
                    else if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Lshape) // Disable horizontal and vertical offsets of subshape 2 for L-shape
                    {
                        do2DLayerUI_L_exp(settingsIndex);
                    }
                    else if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Ushape) // U-shape
                    {
                        do2DLayerUI_U_exp(settingsIndex);
                    }
                    else if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Sshape) // S-shape
                    {
                        do2DLayerUI_S_exp(settingsIndex);
                    }
                    else
                    {
                        num_layer_subshape2_ho_exp.Enabled = true;
                        num_layer_subshape2_vo_exp.Enabled = true;
                        commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_layer_subshape2_ho_exp.Value));
                        commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_layer_subshape2_vo_exp.Value));
                    }

                    warnArray[1] = ((commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) == 0) || (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength) == 0));

                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Sshape)
                    {
                        warnArray[2] = ((commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength) == 0) || (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength) == 0));
                    }
                }

                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape1Tip, comboBox_layerTipLocations2_exp.SelectedIndex);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape2Tip, comboBox_layerTipLocations3_exp.SelectedIndex);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.subShapeIndex, comboBox_layerSubShapeRef_exp.SelectedIndex);
                if (comboBox_layerSubShapeRef_exp.SelectedIndex < 0)
                {
                    comboBox_layerSubShapeRef_exp.SelectedIndex = 0;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.posIndex, comboBox_layerPosSubShape_exp.SelectedIndex);
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.gHorOffset, Convert.ToDecimal(num_layerGlobalHorOffset_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.gVerOffset, Convert.ToDecimal(num_layerGlobalVerOffset_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.rot, Convert.ToDecimal(num_layerRotation_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.wobble, Convert.ToDecimal(num_layer_lithoWobble_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.sBias, Convert.ToDecimal(num_layerSidebias_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.hTBias, Convert.ToDecimal(num_layerHTipbias_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.hTPVar, Convert.ToDecimal(num_layerhTipPVar_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.hTNVar, Convert.ToDecimal(num_layerhTipNVar_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.vTBias, Convert.ToDecimal(num_layerVTipbias_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.vTPVar, Convert.ToDecimal(num_layervTipPVar_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.vTNVar, Convert.ToDecimal(num_layervTipNVar_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.pBias, Convert.ToDecimal(num_pitchDepBias_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.pBiasDist, Convert.ToDecimal(num_pitchDepBiasIsoDistance_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.proxRays, Convert.ToInt32(num_pitchDepBiasSideRays_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.iCR, Convert.ToDecimal(num_layer_lithoICRR_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.oCR, Convert.ToDecimal(num_layer_lithoOCRR_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.iCV, Convert.ToDecimal(num_layer_lithoICV_exp.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.oCV, Convert.ToDecimal(num_layer_lithoOCV_exp.Value));

                do2DLayerUI_litho_exp(settingsIndex, updateUI);

                // If we have a geoCore layer, and it's tagged for DOE, and we have the layer preview based on the DOE extraction, we need to trigger the tile extraction.
                if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE) && (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1) && (commonVars.getLayerPreviewDOETile()))
                {
                    // Need a tile extraction update
                    entropyControl.EntropyRun(numberOfCases: 1, csvFile: null, useThreads: false, doPASearch: false);
                }

                setBGLayerCheckboxes(settingsIndex);

                // Force line drawing for disabled layer.
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.enabled) == 0)
                {
                    mcVPSettings[settingsIndex].drawDrawn(true);
                }
                else
                {
                    mcVPSettings[settingsIndex].drawDrawn((bool)checkBox_Layer_ShowDrawn_exp.Checked);
                }

                drawPreviewPanelHandler();

                updatePASearchUI(settingsIndex);

                reviewBooleanInputs();
                setOmitLayerCheckboxes_EnableStatus();

                // Issue warning about zero dimension
                bool warnNeeded = false;
                string warningString = "Please choose non-zero width and height for subshape(s): ";
                for (int w = 0; w < warnArray.Length; w++)
                {
                    if (warnArray[w])
                    {
                        warnNeeded = true;
                        warningString += (w + 1).ToString() + " ";
                    }
                }

                if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE) ||
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.BOOLEAN) ||
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.none))
                {
                    warnNeeded = false;
                }

                if (warnNeeded)
                {
                    updateStatusLine(warningString);
                }
                else
                {
                    updateStatusLine("");
                }

                reviewComboBoxes(settingsIndex);

                experimental_listBox_layers.SelectedIndex = settingsIndex;
                resumeLayerUI_exp();
                customRNGMappingHighlight_exp(settingsIndex);
                updateGroupBoxVisibility_exp(settingsIndex);
                bgLayerCheckboxChanged(settingsIndex);
                uiFollowChanges();
            });
        }

        void set_shape_type_ui(int settingsIndex)
        {
            // Any configuration beyond the first couple requires a second shape to be defined so we need to display that part of the interface.
            if (
                (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.none) &&
                (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.rect) &&
                (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.GEOCORE) &&
                (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.BOOLEAN)
               )
            {
                // Let's display the subshape 2 section if a shape configuration is chosen that requires it.
                num_layer_subshape2_hl_exp.Enabled = true;
                num_layer_subshape2_vl_exp.Enabled = true;
                num_layer_subshape2_ho_exp.Enabled = true;
                num_layer_subshape2_vo_exp.Enabled = true;
                comboBox_layerTipLocations2_exp.Enabled = true;

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Sshape)
                {
                    num_layer_subshape3_hl_exp.Enabled = true;
                    num_layer_subshape3_vl_exp.Enabled = true;
                    num_layer_subshape3_ho_exp.Enabled = true;
                    num_layer_subshape3_vo_exp.Enabled = true;
                    comboBox_layerTipLocations3_exp.Enabled = true;

                    commonVars.subshapes[settingsIndex].Clear();
                    commonVars.subshapes[settingsIndex].Add("1");
                    commonVars.subshapes[settingsIndex].Add("2");
                    commonVars.subshapes[settingsIndex].Add("3");
                }
                else
                {
                    num_layer_subshape3_hl_exp.Enabled = false;
                    num_layer_subshape3_vl_exp.Enabled = false;
                    num_layer_subshape3_ho_exp.Enabled = false;
                    num_layer_subshape3_vo_exp.Enabled = false;
                    comboBox_layerTipLocations3_exp.Enabled = false;

                    commonVars.subshapes[settingsIndex].Clear();
                    commonVars.subshapes[settingsIndex].Add("1");
                    commonVars.subshapes[settingsIndex].Add("2");
                }
            }
            else
            {
                num_layer_subshape2_hl_exp.Enabled = false;
                num_layer_subshape2_vl_exp.Enabled = false;
                num_layer_subshape2_ho_exp.Enabled = false;
                num_layer_subshape2_vo_exp.Enabled = false;
                comboBox_layerTipLocations2_exp.Enabled = false;

                num_layer_subshape3_hl_exp.Enabled = false;
                num_layer_subshape3_vl_exp.Enabled = false;
                num_layer_subshape3_ho_exp.Enabled = false;
                num_layer_subshape3_vo_exp.Enabled = false;
                comboBox_layerTipLocations3_exp.Enabled = false;

                // Look at the 1 subshape list for displaying options
                commonVars.subshapes[settingsIndex].Clear();
                commonVars.subshapes[settingsIndex].Add("1");
            }
        }

        void reviewComboBoxes(int settingsIndex)
        {
            // Check the stored indices for comboboxes. Workaround for issues imposed by lazy evaluation in table layout, making things awkward as control indices are -1 until/unless drawn.
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape0Tip) < 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape0Tip, 0);
            }
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape1Tip) < 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape1Tip, 0);
            }
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shape2Tip) < 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape2Tip, 0);
            }
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpA) < 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpA, 0);
            }
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpAB) < 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpAB, 0);
            }
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.bLayerOpB) < 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpB, 0);
            }
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.fill) < 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.fill, EntropyLayerSettings.getDefaultInt(EntropyLayerSettings.properties_i.fill));
            }
        }

        void do2DLayerUI_edgeSlide_exp(int settingsIndex)
        {
            checkBox_layer_edgeSlide_exp.Enabled = true;
            num_layer_edgeSlideTension_exp.Enabled = true;

            if ((bool)checkBox_layer_edgeSlide_exp.Checked)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.edgeSlide, 1);
                num_layer_edgeSlideTension_exp.Enabled = true;
            }
            else
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.edgeSlide, 0);
                num_layer_edgeSlideTension_exp.Enabled = false;
            }

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.eTension, (decimal)num_layer_edgeSlideTension_exp.Value);
            if (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.eTension) < 1E-2m)
            {
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.eTension, 1E-2m);
            }
        }

        void do2DLayerUI_Boolean_exp(int settingsIndex)
        {
            comboBox_layerSubShapeRef_exp.SelectedIndex = 0;
            comboBox_layerPosSubShape_exp.SelectedIndex = (int)CommonVars.subShapeLocations.BL;
            comboBox_layerSubShapeRef_exp.Enabled = false;
            comboBox_layerPosSubShape_exp.Enabled = false;

            geoGBVisible[settingsIndex] = false;
            subShapeGBVisible[settingsIndex] = false;
            booleanGBVisible[settingsIndex] = true;

            int aIndex = 0;
            int bIndex = 0;

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (rB_layerBooleanA_exp[i].Checked)
                {
                    aIndex = i;
                }
                if (rB_layerBooleanB_exp[i].Checked)
                {
                    bIndex = i;
                }
            }

            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerA, aIndex - 1); // offset for the 0 case
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerB, bIndex - 1); // offset for the 0 case

            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpA, comboBox_layerBooleanOpA_exp.SelectedIndex);
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpB, comboBox_layerBooleanOpB_exp.SelectedIndex);
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpAB, comboBox_layerBooleanOpAB_exp.SelectedIndex);

            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape0Tip, comboBox_layerTipLocations_boolean_exp.SelectedIndex);
        }

        void do2DLayerUI_geoCore_exp(int settingsIndex)
        {
            if ((varianceContext.vc.geoCoreCDVariation) || (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1))
            {
                groupBox_layer_CDUCorrelation_exp.Enabled = true; // CDU variation permitted.
                groupBox_layer_TipCDUCorrelation_exp.Enabled = true;
            }
            else
            {
                groupBox_layer_CDUCorrelation_exp.Enabled = false; // no CDU variation permitted.
                groupBox_layer_TipCDUCorrelation_exp.Enabled = false;
            }

            geoGBVisible[settingsIndex] = true;
            subShapeGBVisible[settingsIndex] = false;
            booleanGBVisible[settingsIndex] = false;

            // Enable UI elements if layout is valid/loaded.
            // Block UI elements if reloaded as we don't have the layout to move to a different structure/layer-datatype.
            // Need to check state of this conditional in case 'reference' is selected.
            button_layer_globalApply_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
            comboBox_layerStructureList_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid() && !commonVars.getLayerSettings(settingsIndex).isReloaded();
            comboBox_layerLDList_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid() && !commonVars.getLayerSettings(settingsIndex).isReloaded();
            comboBox_layerTipLocations_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
            checkBox_DOELayer_geoCore_exp.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();

            // Disable subshapes
            comboBox_layerSubShapeRef_exp.SelectedIndex = 0;
            comboBox_layerPosSubShape_exp.SelectedIndex = (int)CommonVars.subShapeLocations.BL;
            comboBox_layerSubShapeRef_exp.Enabled = false;
            comboBox_layerPosSubShape_exp.Enabled = false;

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
            {
                comboBox_layerTipLocations_geoCore_exp.Enabled = false;
                // Disable other features. Assuming PV bands are being used.
                num_layerHTipbias_exp.Enabled = false;
                num_layerVTipbias_exp.Enabled = false;
                num_layerhTipNVar_exp.Enabled = false;
                num_layerhTipPVar_exp.Enabled = false;
                num_layervTipNVar_exp.Enabled = false;
                num_layervTipPVar_exp.Enabled = false;

                num_layer_lithoICRR_exp.Enabled = false;
                num_layer_lithoICV_exp.Enabled = false;
                num_layer_lithoCDUSide_exp.Enabled = commonVars.getGCCDV();
                groupBox_layer_CDUCorrelation_exp.Enabled = commonVars.getGCCDV();
                num_layer_lithoCDUTips_exp.Enabled = false;
                groupBox_layer_TipCDUCorrelation_exp.Enabled = false;
                num_layer_lithoOCRR_exp.Enabled = false;
                num_layer_lithoOCV_exp.Enabled = false;
                num_layerRotation_exp.Enabled = false;
                num_layer_lithoWobble_exp.Enabled = false;
            }

            // Fix subshape menu to meet our needs:
            commonVars.subshapes[settingsIndex].Clear();
            commonVars.subshapes[settingsIndex].Add("1");
            comboBox_layerSubShapeRef_exp.SelectedIndex = 0;
            comboBox_layerPosSubShape_exp.SelectedIndex = (int)CommonVars.subShapeLocations.BL;

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
            {
                // Force all values for unsupported properties to 0. Simplifies runtime code.
                num_layer_lithoICRR_exp.Value = 0.0;
                num_layer_lithoICV_exp.Value = 0.0;
                num_layer_lithoOCRR_exp.Value = 0.0;
                num_layer_lithoOCV_exp.Value = 0.0;
                if (!commonVars.getGCCDV())
                {
                    num_layer_lithoCDUSide_exp.Value = 0.0;
                }
                num_layer_lithoCDUTips_exp.Value = 0.0;
                num_layerHTipbias_exp.Value = 0.0;
                num_layerhTipNVar_exp.Value = 0.0;
                num_layerhTipPVar_exp.Value = 0.0;
                num_layerVTipbias_exp.Value = 0.0;
                num_layervTipNVar_exp.Value = 0.0;
                num_layervTipPVar_exp.Value = 0.0;
                num_layerRotation_exp.Value = 0.0;
                num_layer_lithoWobble_exp.Value = 0.0;
                num_layer_subshape_ho_exp.Value = 0.0;
                num_layer_subshape2_ho_exp.Value = 0.0;
                num_layer_subshape3_ho_exp.Value = 0.0;
                num_layer_subshape_vo_exp.Value = 0.0;
                num_layer_subshape2_vo_exp.Value = 0.0;
                num_layer_subshape3_vo_exp.Value = 0.0;
            }

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape0Tip, comboBox_layerTipLocations_geoCore_exp.SelectedIndex);
            }

            // Force update of comboboxes
            if (commonVars.getGeoCoreHandler(settingsIndex).isValid())// && (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1))
            {
                // Avoid triggering repaints as we update things.
                comboBox_layerPolyFill_geoCore_exp.SelectedIndexChanged -= twoDLayerEventHandler_exp;
                comboBox_layerLDList_geoCore_exp.SelectedIndexChanged -= twoDLayerEventHandler_exp;
                comboBox_layerStructureList_geoCore_exp.SelectedIndexChanged -= twoDLayerEventHandler_exp;
                if (!commonVars.getLayerSettings(settingsIndex).isReloaded())
                {

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.fill, comboBox_layerPolyFill_geoCore_exp.SelectedIndex);

                    if (commonVars.getGeoCoreHandler(settingsIndex).isChanged())
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.structure, 0);
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lD, 0);
                        commonVars.getGeoCoreHandler(settingsIndex).setChanged(false);
                    }
                    else
                    {
                        if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure) != comboBox_layerStructureList_geoCore_exp.SelectedIndex)
                        {
                            commonVars.getGeoCoreHandler(settingsIndex).setChanged(true);
                            try
                            {
                                comboBox_layerLDList_geoCore_exp.SelectedIndex = 0; // reset selection.
                            }
                            catch (Exception)
                            {

                            }
                        }
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.structure, comboBox_layerStructureList_geoCore_exp.SelectedIndex);

                        if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD) != comboBox_layerLDList_geoCore_exp.SelectedIndex)
                        {
                            commonVars.getGeoCoreHandler(settingsIndex).setChanged(true);
                        }
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lD, comboBox_layerLDList_geoCore_exp.SelectedIndex);
                    }

                    if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure) == -1) || (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure) >= commonVars.structureList[settingsIndex].Count()))
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.structure, 0);
                    }
                    if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD) == -1) || (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD) >= commonVars.getGeoCoreHandler(settingsIndex).getGeo().getStructureLayerDataTypeList()[commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure)].Count()))
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lD, 0);
                    }

                    commonVars.getLayerSettings(settingsIndex).setString(EntropyLayerSettings.properties_s.structure, commonVars.structureList[settingsIndex][commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure)]);
                    commonVars.getLayerSettings(settingsIndex).setString(EntropyLayerSettings.properties_s.lD, commonVars.getGeoCoreHandler(settingsIndex).getGeo().getStructureLayerDataTypeList()[commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure)][commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD)]);

                    commonVars.getGeoCoreHandler(settingsIndex).getGeo().updateGeometry(commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure), commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD));
                    commonVars.getGeoCoreHandler(settingsIndex).getGeo().updateCollections();

                    // Experimental optimization to try and reduce memory footprint.
                    commonVars.getGeoCoreHandler(settingsIndex).setPoints(commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure), commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD));
                    // Map our points into the layer's file data, if the layer is active.
                    commonVars.getLayerSettings(settingsIndex).setFileData(commonVars.getGeoCoreHandler(settingsIndex).getGeo().points(flatten: true));

                    try
                    {
                        comboBox_layerStructureList_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure);
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        comboBox_layerLDList_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD);
                    }
                    catch (Exception)
                    {

                    }

                }
                else
                {
                    commonVars.getGeoCoreHandler(settingsIndex).getGeo().structureList_.Clear();
                    commonVars.getGeoCoreHandler(settingsIndex).getGeo().structureList_.Add(commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.structure));
                    commonVars.getGeoCoreHandler(settingsIndex).getGeo().activeStructure_LayerDataTypeList_.Clear();
                    commonVars.getGeoCoreHandler(settingsIndex).getGeo().activeStructure_LayerDataTypeList_.Add(commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lD));
                    comboBox_layerStructureList_geoCore_exp.SelectedIndex = 0;
                    comboBox_layerLDList_geoCore_exp.SelectedIndex = 0;
                }
                // Enable repaints for updated things.
                comboBox_layerPolyFill_geoCore_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
                comboBox_layerLDList_geoCore_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;
                comboBox_layerStructureList_geoCore_exp.SelectedIndexChanged += twoDLayerEventHandler_exp;

                if ((bool)checkBox_DOELayer_geoCore_exp.Checked)
                {
                    commonVars.getSimulationSettings().getDOESettings().setLayerAffected(settingsIndex, 1);
                }
                else
                {
                    commonVars.getSimulationSettings().getDOESettings().setLayerAffected(settingsIndex, 0);
                }
            }
        }

        void do2DLayerUI_X_exp(int settingsIndex)
        {
            // Validate our settings and clamp the inputs as needed.
            layer_clampSubShape_exp(minHLength: 0.04, 
                maxHLength: 1000000, 
                minVLength: 0.04, 
                maxVLength: 1000000, 
                minHOffset: -1000000, 
                maxHOffset: 1000000, 
                minVOffset: -1000000, 
                maxVOffset: 1000000
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_layer_subshape_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_layer_subshape_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_layer_subshape_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_layer_subshape_vo_exp.Value));

            num_layer_subshape3_hl_exp.Value = 0;
            num_layer_subshape3_vl_exp.Value = 0;
            num_layer_subshape3_ho_exp.Value = 0;
            num_layer_subshape3_vo_exp.Value = 0;

            decimal minSS2VOffset = 1;
            decimal maxSS2VOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);

            decimal minSS2HOffset = -(commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
            decimal maxSS2HOffset = -1;

            decimal minSS2HLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) + (2 * 0.01m);
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - (2 * 0.01m);
            if (maxSS2VLength < 0)
            {
                maxSS2VLength = 0.02m;
            }

            layer_clampSubShape2_exp(minHLength: (double)minSS2HLength, 
                maxHLength: 1000000, 
                minVLength: 0.02, 
                maxVLength: (double)maxSS2VLength, 
                minHOffset: (double)minSS2HOffset, 
                maxHOffset: (double)maxSS2HOffset, 
                minVOffset: (double)minSS2VOffset, 
                maxVOffset: (double)maxSS2VOffset
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_layer_subshape2_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_layer_subshape2_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_layer_subshape2_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_layer_subshape2_vo_exp.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_layer_subshape3_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_layer_subshape3_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_layer_subshape3_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_layer_subshape3_vo_exp.Value));

            num_layer_subshape2_ho_exp.Enabled = true;
            num_layer_subshape2_vo_exp.Enabled = true;
        }

        void do2DLayerUI_T_exp(int settingsIndex)
        {
            // Validate our settings and clamp the inputs as needed.
            layer_clampSubShape_exp(minHLength: 0.01, 
                maxHLength: 1000000, 
                minVLength: 0.04, 
                maxVLength: 1000000, 
                minHOffset: -1000000, 
                maxHOffset: 1000000, 
                minVOffset: -1000000, 
                maxVOffset: 1000000
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_layer_subshape_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_layer_subshape_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_layer_subshape_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_layer_subshape_vo_exp.Value));

            num_layer_subshape3_hl_exp.Value = 0;
            num_layer_subshape3_vl_exp.Value = 0;
            num_layer_subshape3_ho_exp.Value = 0;
            num_layer_subshape3_vo_exp.Value = 0;

            decimal minSS2HLength = 0.01m;
            decimal minSS2VLength = 0.02m;
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - (2 * 0.01m);
            decimal maxSS2VOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);

            layer_clampSubShape2_exp(minHLength: (double)minSS2HLength, 
                maxHLength: 1000000, 
                minVLength: (double)minSS2VLength, 
                maxVLength: (double)maxSS2VLength,
                minHOffset: (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength),
                maxHOffset: (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength),
                minVOffset: 1, 
                maxVOffset: (double)maxSS2VOffset
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_layer_subshape2_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_layer_subshape2_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_layer_subshape2_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_layer_subshape2_vo_exp.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_layer_subshape3_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_layer_subshape3_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_layer_subshape3_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_layer_subshape3_vo_exp.Value));

            num_layer_subshape2_ho_exp.Enabled = false;
            num_layer_subshape2_vo_exp.Enabled = true;
        }

        void do2DLayerUI_L_exp(int settingsIndex)
        {
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_layer_subshape_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_layer_subshape_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_layer_subshape_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_layer_subshape_vo_exp.Value));

            num_layer_subshape3_hl_exp.Value = 0;
            num_layer_subshape3_vl_exp.Value = 0;
            num_layer_subshape3_ho_exp.Value = 0;
            num_layer_subshape3_vo_exp.Value = 0;

            decimal minSS2HLength = 0;
            decimal minSS2VLength = 0;
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength);

            decimal minSS2HOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength);
            decimal maxSS2HOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength);
            decimal minSS2VOffset = 0;
            decimal maxSS2VOffset = 0;

            layer_clampSubShape2_exp(minHLength: (double)minSS2HLength, 
                maxHLength: 1000000, 
                minVLength: (double)minSS2VLength, 
                maxVLength: (double)maxSS2VLength,
                minHOffset: (double)minSS2HOffset,
                maxHOffset: (double)maxSS2HOffset,
                minVOffset: (double)minSS2VOffset, 
                maxVOffset: (double)maxSS2VOffset
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_layer_subshape2_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_layer_subshape2_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_layer_subshape2_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_layer_subshape2_vo_exp.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_layer_subshape3_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_layer_subshape3_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_layer_subshape3_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_layer_subshape3_vo_exp.Value));

            num_layer_subshape2_ho_exp.Enabled = false;
            num_layer_subshape2_vo_exp.Enabled = false;
        }

        void do2DLayerUI_U_exp(int settingsIndex)
        {
            // Validate our settings and clamp the inputs as needed.
            layer_clampSubShape_exp(minHLength: 0.04, 
                maxHLength: 1000000, 
                minVLength: 0.04, 
                maxVLength: 1000000, 
                minHOffset: -1000000, 
                maxHOffset: 1000000, 
                minVOffset: -1000000, 
                maxVOffset: 1000000
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_layer_subshape_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_layer_subshape_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_layer_subshape_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_layer_subshape_vo_exp.Value));

            num_layer_subshape3_hl_exp.Value = 0;
            num_layer_subshape3_vl_exp.Value = 0;
            num_layer_subshape3_ho_exp.Value = 0;
            num_layer_subshape3_vo_exp.Value = 0;

            decimal minSS2HLength = 0.02m;
            decimal minSS2VLength = 0.02m;
            decimal maxSS2HLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) - 0.02m;
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - 0.02m;

            decimal ss2HOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) - 0.02m;
            decimal ss2VOffset = (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));

            layer_clampSubShape2_exp(minHLength: (double)minSS2HLength, 
                maxHLength: (double)maxSS2HLength, 
                minVLength: (double)minSS2VLength, 
                maxVLength: (double)maxSS2VLength,
                minHOffset: 0.02f, 
                maxHOffset: (double)ss2HOffset,
                minVOffset: (double)ss2VOffset, 
                maxVOffset: (double)ss2VOffset
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_layer_subshape2_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_layer_subshape2_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_layer_subshape2_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_layer_subshape2_vo_exp.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_layer_subshape3_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_layer_subshape3_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_layer_subshape3_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_layer_subshape3_vo_exp.Value));

            num_layer_subshape2_ho_exp.Enabled = true;
            num_layer_subshape2_vo_exp.Enabled = false;
        }

        void do2DLayerUI_S_exp(int settingsIndex)
        {
            // Validate our settings and clamp the inputs as needed.
            layer_clampSubShape_exp(minHLength: 0.04, 
                maxHLength: 1000000, 
                minVLength: 0.04, 
                maxVLength: 1000000, 
                minHOffset: -1000000, 
                maxHOffset: 1000000, 
                minVOffset: -1000000, 
                maxVOffset: 1000000
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_layer_subshape_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_layer_subshape_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_layer_subshape_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_layer_subshape_vo_exp.Value));

            decimal minSS2HLength = 0.01m;
            decimal maxSS2HLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) - 0.01m;
            decimal minSS2VLength = 0.02m;
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - 0.01m;
            decimal ss2HOffset = 0;
            decimal minSS2VOffset = 0.01m;
            decimal maxSS2VOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);
            layer_clampSubShape2_exp(minHLength: (double)minSS2HLength, 
                maxHLength: (double)maxSS2HLength, 
                minVLength: (double)minSS2VLength, 
                maxVLength: (double)maxSS2VLength,
                minHOffset: (double)ss2HOffset, 
                maxHOffset: (double)ss2HOffset, 
                minVOffset: (double)minSS2VOffset, 
                maxVOffset: (double)maxSS2VOffset
            );

            decimal minSS3HLength = 0.01m;
            decimal maxSS3HLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) - 0.01m;
            decimal minSS3VLength = 0.02m;
            decimal maxSS3VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - 0.01m;
            decimal ss3HOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength);
            decimal minSS3VOffset = 0.01m;
            decimal maxSS3VOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength);
            layer_clampSubShape3_exp(minHLength: (double)minSS3HLength, 
                maxHLength: (double)maxSS3HLength, 
                minVLength: (double)minSS3VLength, 
                maxVLength: (double)maxSS3VLength,
                minHOffset: (double)ss3HOffset, 
                maxHOffset: (double)ss3HOffset, 
                minVOffset: (double)minSS3VOffset, 
                maxVOffset: (double)maxSS3VOffset
                );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_layer_subshape2_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_layer_subshape2_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_layer_subshape2_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_layer_subshape2_vo_exp.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_layer_subshape3_hl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_layer_subshape3_vl_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_layer_subshape3_ho_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_layer_subshape3_vo_exp.Value));

            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape2Tip, comboBox_layerTipLocations3_exp.SelectedIndex);

            // FIXME: Need some logic here to avoid bisection of the S.

            num_layer_subshape2_ho_exp.Enabled = false;
            num_layer_subshape2_vo_exp.Enabled = true;

            num_layer_subshape3_ho_exp.Enabled = false;
            num_layer_subshape3_vo_exp.Enabled = true;
        }

        void do2DLayerUI_litho_exp(int settingsIndex, bool updateUI)
        {
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lDC1, Convert.ToDecimal(num_layer_coeff1_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lDC2, Convert.ToDecimal(num_layer_coeff2_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lwr, Convert.ToDecimal(num_layer_lithoLWR_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lwrFreq, Convert.ToDecimal(num_layer_lithoLWRFreq_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwrType, comboBox_layerLWRNoiseType_exp.SelectedIndex);
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lwr2, Convert.ToDecimal(num_layer_lithoLWR2_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq, Convert.ToDecimal(num_layer_lithoLWR2Freq_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr2Type, comboBox_layerLWR2NoiseType_exp.SelectedIndex);
            if ((bool)cB_layer_LWRPreview_exp.Checked)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwrPreview, 1);
            }
            else
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwrPreview, 0);
            }
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.sCDU, Convert.ToDecimal(num_layer_lithoCDUSide_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.tCDU, Convert.ToDecimal(num_layer_lithoCDUTips_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.xOL, Convert.ToDecimal(num_layer_lithoHorOverlay_exp.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.yOL, Convert.ToDecimal(num_layer_lithoVerOverlay_exp.Value));

            int COLXIndex = 0;
            int COLYIndex = 0;
            int OLRXIndex = 0;
            int OLRYIndex = 0;
            int CLWRIndex = 0;
            int CLWR2Index = 0;
            int CCDUIndex = 0;
            int CTCDUIndex = 0;
            bool[] avXIndices = new bool[CentralProperties.maxLayersForMC];
            bool[] avYIndices = new bool[CentralProperties.maxLayersForMC];

            commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, settingsIndex, 0); // can never have a self-reference for this, so reset it to avoid trouble.
            commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, settingsIndex, 0); // can never have a self-reference for this, so reset it to avoid trouble.

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (rB_layer_COLX_exp[i].Checked)
                {
                    COLXIndex = i;
                }
                if (rB_layer_COLY_exp[i].Checked)
                {
                    COLYIndex = i;
                }
                if (rB_layer_OLRX_exp[i].Checked)
                {
                    OLRXIndex = i;
                }
                if (rB_layer_OLRY_exp[i].Checked)
                {
                    OLRYIndex = i;
                }
                if (rB_layer_CLWR_exp[i].Checked)
                {
                    CLWRIndex = i;
                }
                if (rB_layer_CLWR2_exp[i].Checked)
                {
                    CLWR2Index = i;
                }
                if (rB_layer_CCDU_exp[i].Checked)
                {
                    CCDUIndex = i;
                }
                if (rB_layer_CTCDU_exp[i].Checked)
                {
                    CTCDUIndex = i;
                }

                if (i != settingsIndex)
                {
                    if ((bool)cB_layer_OLRX_Av_exp[i].Checked)
                    {
                        commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, i, 1);
                    }
                    else
                    {
                        commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, i, 0);
                    }

                    if ((bool)cB_layer_OLRY_Av_exp[i].Checked)
                    {
                        commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, i, 1);
                    }
                    else
                    {
                        commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, i, 0);
                    }
                }
            }

            if (COLXIndex == 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_corr, 0);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_corr_ref, -1);
            }
            else
            {
                int xcolRefIndex = Convert.ToInt32(rB_layer_COLX_exp[COLXIndex].Text);
                if (xcolRefIndex > settingsIndex)
                {
                    xcolRefIndex--; // compensate for missing radio button for current layer.
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_corr_ref, xcolRefIndex - 1);
            }

            if (COLYIndex == 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_corr, 0);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, -1);
            }
            else
            {
                int ycolRefIndex = Convert.ToInt32(rB_layer_COLY_exp[COLYIndex].Text);
                if (ycolRefIndex > settingsIndex)
                {
                    ycolRefIndex--; // compensate for missing radio button for current layer.
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_corr, 1);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, ycolRefIndex - 1);
            }

            if (OLRXIndex == 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_ref, -1);
            }
            else
            {
                int xolRefIndex = Convert.ToInt32(rB_layer_OLRX_exp[OLRXIndex].Text);
                if (xolRefIndex > settingsIndex)
                {
                    xolRefIndex--; // compensate for missing radio button for current layer.
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_ref, xolRefIndex - 1);
            }

            if (OLRYIndex == 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_ref, -1);
            }
            else
            {
                int yolRefIndex = Convert.ToInt32(rB_layer_OLRY_exp[OLRYIndex].Text);
                if (yolRefIndex > settingsIndex)
                {
                    yolRefIndex--; // compensate for missing radio button for current layer.
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_ref, yolRefIndex - 1);
            }

            if (CLWRIndex == 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr_corr, 0);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr_corr_ref, -1);
            }
            else
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr_corr, 1);
                int clwrRefIndex = Convert.ToInt32(rB_layer_CLWR_exp[CLWRIndex].Text);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr_corr_ref, clwrRefIndex - 1);

            }

            if (CLWR2Index == 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr2_corr, 0);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr2_corr_ref, -1);
            }
            else
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr2_corr, 1);
                int clwr2RefIndex = Convert.ToInt32(rB_layer_CLWR2_exp[CLWRIndex].Text);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr2_corr_ref, clwr2RefIndex - 1);

            }

            if (CCDUIndex == 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.CDU_corr, 0);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.CDU_corr_ref, -1);
            }
            else
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.CDU_corr, 1);
                int ccduRefIndex = Convert.ToInt32(rB_layer_CCDU_exp[CCDUIndex].Text);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.CDU_corr_ref, ccduRefIndex - 1);

            }

            if (CTCDUIndex == 0)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.tCDU_corr, 0);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, -1);
            }
            else
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.tCDU_corr, 1);
                int ctcduRefIndex = Convert.ToInt32(rB_layer_CTCDU_exp[CTCDUIndex].Text);
                if (ctcduRefIndex > settingsIndex)
                {
                    ctcduRefIndex--; // compensate for missing radio button for current layer.
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, ctcduRefIndex - 1);
            }

            // Process the average overlay arrays.
            if (checkBox_layer_overlayXReference_Av_exp.Checked == true)
            {
                if (updateUI)
                {
                    pnl_overlayRefX.Content = groupBox_layer_overlayXReference_Av_exp;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_av, 1);
            }
            else
            {
                if (updateUI)
                {
                    pnl_overlayRefX.Content = groupBox_layer_overlayXReference_exp;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_av, 0);
            }

            if ((bool)checkBox_layer_overlayYReference_Av_exp.Checked)
            {
                if (updateUI)
                {
                    pnl_overlayRefY.Content = groupBox_layer_overlayYReference_Av_exp;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_av, 1);
            }
            else
            {
                if (updateUI)
                {
                    pnl_overlayRefY.Content = groupBox_layer_overlayYReference_exp;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_av, 0);
            }
        }

        void setBGLayerCheckboxes(int settingsIndex)
        {
            bool alreadyFrozen = globalUIFrozen;
            globalUIFrozen = true;
            Application.Instance.Invoke(() =>
            {
                int rowIndex = 0;
                int colIndex = 0;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    checkBox_bg_lyr[i].Text = name;
                    if (settingsIndex != i)
                    {
                        checkBox_bg_lyr[i].Enabled = (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1);
                        checkBox_bg_lyr[i].Checked = ((commonVars.getLayerSettings(settingsIndex).getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, i) == 1) && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1));
                    }
                    else
                    {
                        checkBox_bg_lyr[i].Enabled = false;
                        checkBox_bg_lyr[i].Checked = false;
                    }
                    colIndex++;
                    if (colIndex == CentralProperties.maxLayersForMC / 2)
                    {
                        colIndex = 0;
                        rowIndex++;
                    }
                }
            });
            if (!alreadyFrozen)
            {
                globalUIFrozen = false;
            }
        }

        void reviewBooleanInputs()
        {
            // Here, we walk the entire UI and reset gadgets, so we should freeze the UI to prevent problems.
            bool alreadyFrozen = globalUIFrozen;
            globalUIFrozen = true;

            try
            {
                // Check for 'enabled' status of input layers to Booleans. In case those layers are now disabled, unset the input.
                for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
                {
                    int boolLayer = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerA);
                    if (boolLayer > -1)
                    {
                        bool activeStatus = (commonVars.getLayerSettings(boolLayer).getInt(EntropyLayerSettings.properties_i.enabled) == 1);

                        if (!activeStatus)
                        {
                            commonVars.getLayerSettings(layer).setInt(EntropyLayerSettings.properties_i.bLayerA, -1);
                            if (layer == getSelectedLayerIndex())
                            {
                                rB_layerBooleanA_exp[0].Checked = true;
                            }
                            checkBox_omit_lyr[boolLayer].Checked = false;
                            checkBox_omit_lyr[boolLayer].Enabled = false;
                        }
                    }

                    boolLayer = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerB);
                    if (boolLayer > -1)
                    {
                        bool activeStatus = (commonVars.getLayerSettings(boolLayer).getInt(EntropyLayerSettings.properties_i.enabled) == 1);

                        if (!activeStatus)
                        {
                            commonVars.getLayerSettings(layer).setInt(EntropyLayerSettings.properties_i.bLayerB, -1);
                            if (layer == getSelectedLayerIndex())
                            {
                                rB_layerBooleanB_exp[0].Checked = true;
                            }
                            checkBox_omit_lyr[boolLayer].Checked = false;
                            checkBox_omit_lyr[boolLayer].Enabled = false;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            if (!alreadyFrozen)
            {
                globalUIFrozen = false;
            }
        }

        void setOmitLayerCheckboxes_EnableStatus()
        {
            bool[] bLayerStatus = new bool[CentralProperties.maxLayersForMC];
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                bLayerStatus[i] = false;
            }

            // We have to scan all layers to check for boolean usage, as there may be more than one user for a given layer.
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                int layerUsedInBoolean = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.bLayerA);
                if (layerUsedInBoolean > -1)
                {
                    bLayerStatus[layerUsedInBoolean] = true;
                }
                layerUsedInBoolean = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.bLayerB);
                if (layerUsedInBoolean > -1)
                {
                    bLayerStatus[layerUsedInBoolean] = true;
                }
            }

            // Set our checkbox enabled status
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                if (name == "")
                {
                    // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                    name = (i + 1).ToString();
                }
                checkBox_omit_lyr[i].Text = name;
                checkBox_omit_lyr[i].Enabled = bLayerStatus[i];
            }
        }

        void showDrawn_exp(int settingsIndex)
        {
            bool showDrawn = (bool)checkBox_Layer_ShowDrawn_exp.Checked;
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.showDrawn, showDrawn ? 1 : 0);
            doShowDrawn(settingsIndex);
        }

        void doShowDrawn(int settingsIndex)
        {
            mcVPSettings[settingsIndex].drawDrawn(commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.showDrawn) == 1);
            if (!layerUIFrozen_exp)
            {
                updateViewport();
            }
        }

        void updateGroupBoxVisibility_exp(int layer)
        {
            // Have to force the comboboxes here - seems to be a needed workaround as the table layout does lazy evaluation and we can't rely on these elements knowing their state.
            bool alreadyFrozen = layerUIFrozen_exp;
            layerUIFrozen_exp = true;

            geoGBVisible[layer] = false;
            subShapeGBVisible[layer] = false;
            booleanGBVisible[layer] = false;

            if (
                 (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (Int32)CommonVars.shapeNames.none) &&
                 (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (Int32)CommonVars.shapeNames.GEOCORE) &&
                 (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (Int32)CommonVars.shapeNames.BOOLEAN)
                )
            {
                subShapeGBVisible[layer] = true;
            }

            if (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE)
            {
                geoGBVisible[layer] = true;
            }

            if (commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.BOOLEAN)
            {
                booleanGBVisible[layer] = true;
            }

            if (subShapeGBVisible[layer])
            {
                layerShapeProperties_tcPanel.Content = groupBox_layerSubShapes_exp;
                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
                comboBox_layerTipLocations_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                comboBox_layerTipLocations2_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape1Tip);
                comboBox_layerTipLocations3_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape2Tip);
            }
            else if (geoGBVisible[layer])
            {
                layerShapeProperties_tcPanel.Content = groupBox_layer_geoCore_exp;
                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
                comboBox_layerTipLocations_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                comboBox_layerPolyFill_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.fill);
                try
                {
                    comboBox_layerStructureList_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.structure);
                }
                catch (Exception)
                {

                }
                try
                {
                    comboBox_layerLDList_geoCore_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.lD);
                }
                catch (Exception)
                {

                }
            }
            else if (booleanGBVisible[layer])
            {
                layerShapeProperties_tcPanel.Content = groupBox_layerBoolean_exp;
                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
                comboBox_layerTipLocations_boolean_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                comboBox_layerBooleanOpA_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerOpA);
                comboBox_layerBooleanOpAB_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerOpAB);
                comboBox_layerBooleanOpB_exp.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerOpB);
            }
            else
            {
                layerShapeProperties_tcPanel.Content = null;
                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
            }

            layerUIFrozen_exp = alreadyFrozen;
        }

        void customRNGMappingHighlight_exp(int settingsIndex)
        {
            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.sCDU_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoCDUSide_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoCDUSide_exp.TextColor = SystemColors.ControlText;
            }
            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.tCDU_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoCDUTips_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoCDUTips_exp.TextColor = SystemColors.ControlText;
            }
            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.xOL_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoHorOverlay_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoHorOverlay_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layer_lithoHorOverlay_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.xOL_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.yOL_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoVerOverlay_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoVerOverlay_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layer_lithoVerOverlay_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.yOL_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.iCV_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoICV_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoICV_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layer_lithoICV_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.iCV_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.oCV_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoOCV_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoOCV_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layer_lithoOCV_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.oCV_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.wobble_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoWobble_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoWobble_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layer_lithoWobble_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.wobble_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.hTipNVar_RNG) != CommonVars.boxMuller)
            {
                lbl_layerhTipNVar_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layerhTipNVar_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layerhTipNVar_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.hTipNVar_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.hTipPVar_RNG) != CommonVars.boxMuller)
            {
                lbl_layerhTipPVar_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layerhTipPVar_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layerhTipPVar_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.hTipPVar_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.vTipNVar_RNG) != CommonVars.boxMuller)
            {
                lbl_layervTipNVar_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layervTipNVar_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layervTipNVar_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.vTipNVar_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.vTipPVar_RNG) != CommonVars.boxMuller)
            {
                lbl_layervTipPVar_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layervTipPVar_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layervTipPVar_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.vTipPVar_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lwr_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoLWR_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoLWR_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layer_lithoLWR_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lwr_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lwr2_RNG) != CommonVars.boxMuller)
            {
                lbl_layer_lithoLWR2_exp.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_layer_lithoLWR2_exp.TextColor = SystemColors.ControlText;
            }
            lbl_layer_lithoLWR2_exp.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lwr2_RNG);
        }
    }
}
