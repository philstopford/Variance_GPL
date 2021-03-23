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
                listBox_layers.SelectedIndex = index;
            });
        }

        void goToLayerUI(int index)
        {
            setMainSelectedIndex((Int32)CommonVars.upperTabNames.twoD);
            setSubTabSelectedIndex((Int32)CommonVars.twoDTabNames.layer);
            set2DSelectedIndex(index);
            set_ui_from_settings(index);
        }

        void layer_clampSubShape(double minHLength, double maxHLength, double minVLength, double maxVLength, double minHOffset, double maxHOffset, double minVOffset, double maxVOffset)
        {
            Application.Instance.Invoke(() =>
            {
                if (num_subshape_hl.Value < minHLength)
                {
                    num_subshape_hl.Value = minHLength;
                }

                if (num_subshape_hl.Value > maxHLength)
                {
                    num_subshape_hl.Value = maxHLength;
                }

                if (num_subshape_vl.Value < minVLength)
                {
                    num_subshape_vl.Value = minVLength;
                }

                if (num_subshape_vl.Value > maxVLength)
                {
                    num_subshape_vl.Value = maxVLength;
                }

                if (num_subshape_ho.Value < minHOffset)
                {
                    num_subshape_ho.Value = minHOffset;
                }

                if (num_subshape_ho.Value > maxHOffset)
                {
                    num_subshape_ho.Value = maxHOffset;
                }

                if (num_subshape_vo.Value < minVOffset)
                {
                    num_subshape_vo.Value = minVOffset;
                }

                if (num_subshape_vo.Value > maxVOffset)
                {
                    num_subshape_vo.Value = maxVOffset;
                }
            });
        }

        void layer_clampSubShape2(double minHLength, double maxHLength, double minVLength, double maxVLength, double minHOffset, double maxHOffset, double minVOffset, double maxVOffset)
        {
            Application.Instance.Invoke(() =>
            {
                if (num_subshape2_hl.Value < minHLength)
                {
                    num_subshape2_hl.Value = minHLength;
                }

                if (num_subshape2_hl.Value > maxHLength)
                {
                    num_subshape2_hl.Value = maxHLength;
                }

                if (num_subshape2_vl.Value < minVLength)
                {
                    num_subshape2_vl.Value = minVLength;
                }

                if (num_subshape2_vl.Value > maxVLength)
                {
                    num_subshape2_vl.Value = maxVLength;
                }

                if (num_subshape2_ho.Value < minHOffset)
                {
                    num_subshape2_ho.Value = minHOffset;
                }

                if (num_subshape2_ho.Value > maxHOffset)
                {
                    num_subshape2_ho.Value = maxHOffset;
                }

                if (num_subshape2_vo.Value < minVOffset)
                {
                    num_subshape2_vo.Value = minVOffset;
                }

                if (num_subshape2_vo.Value > maxVOffset)
                {
                    num_subshape2_vo.Value = maxVOffset;
                }
            });
        }

        void layer_clampSubShape3(double minHLength, double maxHLength, double minVLength, double maxVLength, double minHOffset, double maxHOffset, double minVOffset, double maxVOffset)
        {
            Application.Instance.Invoke(() =>
            {
                if (num_subshape3_hl.Value < minHLength)
                {
                    num_subshape3_hl.Value = minHLength;
                }

                if (num_subshape3_hl.Value > maxHLength)
                {
                    num_subshape3_hl.Value = maxHLength;
                }

                if (num_subshape3_vl.Value < minVLength)
                {
                    num_subshape3_vl.Value = minVLength;
                }

                if (num_subshape3_vl.Value > maxVLength)
                {
                    num_subshape3_vl.Value = maxVLength;
                }

                if (num_subshape3_ho.Value < minHOffset)
                {
                    num_subshape3_ho.Value = minHOffset;
                }

                if (num_subshape3_ho.Value > maxHOffset)
                {
                    num_subshape3_ho.Value = maxHOffset;
                }

                if (num_subshape3_vo.Value < minVOffset)
                {
                    num_subshape3_vo.Value = minVOffset;
                }

                if (num_subshape3_vo.Value > maxVOffset)
                {
                    num_subshape3_vo.Value = maxVOffset;
                }
            });
        }

        void addLayerHandlers()
        {
            layerUIFrozen_exp = false;
            btn_globalApply_geoCore.Click += applyLayoutToAll;

            cB_Layer.CheckedChanged += twoDLayerEventHandler_exp;
            text_layerName.LostFocus += twoDLayerEventHandler_exp;

            cB_edgeSlide.CheckedChanged += twoDLayerEventHandler_exp;
            num_edgeSlideTension.LostFocus += twoDLayerEventHandler_exp;

            cB_FlipH.CheckedChanged += twoDLayerEventHandler_exp;
            cB_FlipV.CheckedChanged += twoDLayerEventHandler_exp;
            cB_alignGeometryX.CheckedChanged += twoDLayerEventHandler_exp;
            cB_alignGeometryY.CheckedChanged += twoDLayerEventHandler_exp;

            comboBox_layerShape.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_subShapeRef.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_posSubShape.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_globalHorOffset.LostFocus += twoDLayerEventHandler_exp;
            num_globalVerOffset.LostFocus += twoDLayerEventHandler_exp;

            num_subshape_hl.LostFocus += twoDLayerEventHandler_exp;
            num_subshape_vl.LostFocus += twoDLayerEventHandler_exp;
            num_subshape_ho.LostFocus += twoDLayerEventHandler_exp;
            num_subshape_vo.LostFocus += twoDLayerEventHandler_exp;
            comboBox_tipLocations.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_subshape2_hl.LostFocus += twoDLayerEventHandler_exp;
            num_subshape2_vl.LostFocus += twoDLayerEventHandler_exp;
            num_subshape2_ho.LostFocus += twoDLayerEventHandler_exp;
            num_subshape2_vo.LostFocus += twoDLayerEventHandler_exp;
            comboBox_tipLocations2.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_subshape3_hl.LostFocus += twoDLayerEventHandler_exp;
            num_subshape3_vl.LostFocus += twoDLayerEventHandler_exp;
            num_subshape3_ho.LostFocus += twoDLayerEventHandler_exp;
            num_subshape3_vo.LostFocus += twoDLayerEventHandler_exp;
            comboBox_tipLocations3.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_lithoLWR.LostFocus += twoDLayerEventHandler_exp;
            num_lithoLWRFreq.LostFocus += twoDLayerEventHandler_exp;
            comboBox_LWRNoiseType.SelectedIndexChanged += twoDLayerEventHandler_exp;
            num_lithoLWR2.LostFocus += twoDLayerEventHandler_exp;
            num_lithoLWR2Freq.LostFocus += twoDLayerEventHandler_exp;
            comboBox_LWR2NoiseType.SelectedIndexChanged += twoDLayerEventHandler_exp;
            cB_layer_LWRPreview.CheckedChanged += twoDLayerEventHandler_exp;

            num_sidebias.LostFocus += twoDLayerEventHandler_exp;

            num_lithoCDUTips.LostFocus += twoDLayerEventHandler_exp;
            num_lithoCDUSide.LostFocus += twoDLayerEventHandler_exp;

            num_hTipbias.LostFocus += twoDLayerEventHandler_exp;
            num_vTipbias.LostFocus += twoDLayerEventHandler_exp;

            num_hTipPVar.LostFocus += twoDLayerEventHandler_exp;
            num_hTipNVar.LostFocus += twoDLayerEventHandler_exp;

            num_vTipPVar.LostFocus += twoDLayerEventHandler_exp;
            num_vTipNVar.LostFocus += twoDLayerEventHandler_exp;

            num_pitchDepBias.LostFocus += twoDLayerEventHandler_exp;
            num_pitchDepBiasIsoDistance.LostFocus += twoDLayerEventHandler_exp;
            num_pitchDepBiasSideRays.LostFocus += twoDLayerEventHandler_exp;

            comboBox_proxBiasFallOff.SelectedIndexChanged += twoDLayerEventHandler_exp;
            num_proxBiasFallOffMultiplier.LostFocus += twoDLayerEventHandler_exp;

            num_lithoICRR.LostFocus += twoDLayerEventHandler_exp;
            num_lithoICV.LostFocus += twoDLayerEventHandler_exp;
            num_lithoOCRR.LostFocus += twoDLayerEventHandler_exp;
            num_lithoOCV.LostFocus += twoDLayerEventHandler_exp;

            num_lithoHorOverlay.LostFocus += twoDLayerEventHandler_exp;
            num_lithoVerOverlay.LostFocus += twoDLayerEventHandler_exp;

            num_rotation.LostFocus += twoDLayerEventHandler_exp;
            num_lithoWobble.LostFocus += twoDLayerEventHandler_exp;

            num_coeff1.LostFocus += twoDLayerEventHandler_exp;
            num_coeff2.LostFocus += twoDLayerEventHandler_exp;

            btn_chooseFile_geoCore.Click += geoFileChooser_Handler_exp;
            comboBox_lDList_geoCore.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_structureList_geoCore.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_polyFill_geoCore.SelectedIndexChanged += twoDLayerEventHandler_exp;
            cB_geoCore_shapeEngine.CheckedChanged += twoDLayerEventHandler_exp;
            cB_geoCore_shapeEngine_perPoly.CheckedChanged += twoDLayerEventHandler_exp;
            comboBox_tipLocations_geoCore.SelectedIndexChanged += twoDLayerEventHandler_exp;
            cB_geoCore_layoutReference.CheckedChanged += twoDLayerEventHandler_exp;
            cB_DOE_geoCore.CheckedChanged += twoDLayerEventHandler_exp;

            cB_ShowDrawn.CheckedChanged += showDrawn_exp;

            comboBox_BooleanOpA.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_BooleanOpB.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_BooleanOpAB.SelectedIndexChanged += twoDLayerEventHandler_exp;
            comboBox_TipLocations_boolean.SelectedIndexChanged += twoDLayerEventHandler_exp;

            num_rayExtension.LostFocus += twoDLayerEventHandler_exp;

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                rB_COLX[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_COLY[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_OLRX[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_OLRY_exp[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_CLWR[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_CLWR2[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_CCDU[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_CTCDU[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_BooleanA[i].CheckedChanged += twoDLayerEventHandler_exp;
                rB_BooleanB[i].CheckedChanged += twoDLayerEventHandler_exp;
                cB_OLRX_Av[i].CheckedChanged += twoDLayerEventHandler_exp;
                cB_layer_OLRY_Av[i].CheckedChanged += twoDLayerEventHandler_exp;
            }

            cB_overlayXReference_Av.CheckedChanged += twoDLayerEventHandler_exp;
            cB_overlayYReference_Av.CheckedChanged += twoDLayerEventHandler_exp;
        }

        void do2DLayerUI(int settingsIndex, bool updateUI = false)
        {
            Application.Instance.Invoke(() =>
            {
                if (layerUIFrozen_exp)
                {
                    return;
                }

                suspendLayerUI_exp();

                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shapeIndex, comboBox_layerShape.SelectedIndex);

                // Set our tab name.
                if (text_layerName.Text != "")
                {
                    commonVars.getLayerSettings(settingsIndex).setString(EntropyLayerSettings.properties_s.name, text_layerName.Text);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setString(EntropyLayerSettings.properties_s.name, (settingsIndex + 1).ToString());
                }
                // We have to deregister the handler here otherwise the name update causes the selectedindexchanged event to fire and everything breaks.
                listBox_layers.SelectedIndexChanged -= listbox_change;
                commonVars.layerNames[settingsIndex] = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.name);
                listBox_layers.SelectedIndexChanged += listbox_change;

                listBox_layers.SelectedIndex = selectedLayer;

                if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE) &&
                    !((bool)cB_geoCore_shapeEngine.Checked))
                {
                    cB_FlipH.Checked = false;
                    cB_FlipV.Checked = false;
                    cB_alignGeometryX.Checked = false;
                    cB_alignGeometryY.Checked = false;

                    cB_FlipH.Enabled = false;
                    cB_FlipV.Enabled = false;
                    cB_alignGeometryX.Enabled = false;
                    cB_alignGeometryY.Enabled = false;

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipH, 0);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipV, 0);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignX, 0);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignY, 0);
                }
                else
                {
                    cB_FlipH.Enabled = true;
                    cB_FlipV.Enabled = true;

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipH, 0);
                    if ((bool)cB_FlipH.Checked)
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipH, 1);
                    }

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipV, 0);
                    if ((bool)cB_FlipV.Checked)
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.flipV, 1);
                    }

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignX, 0);
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignY, 0);
                    cB_alignGeometryX.Enabled = false;
                    cB_alignGeometryY.Enabled = false;
                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipH) == 1)
                    {
                        cB_alignGeometryX.Enabled = true;
                        if (cB_alignGeometryX.Checked == true)
                        {
                            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignX, 1);
                        }
                    }

                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.flipV) == 1)
                    {
                        cB_alignGeometryY.Enabled = true;
                        if (cB_alignGeometryY.Checked == true)
                        {
                            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.alignY, 1);
                        }
                    }
                }

                do2DLayerUI_edgeSlide(settingsIndex);

                Int32 previousIndex = comboBox_subShapeRef.SelectedIndex;

                if (updateUI)
                {
                    set_shape_type_ui(settingsIndex);
                }
                if (previousIndex >= commonVars.subshapes[settingsIndex].Count)
                {
                    previousIndex = commonVars.subshapes[settingsIndex].Count - 1;
                }

                comboBox_subShapeRef.SelectedIndex = previousIndex;

                comboBox_geoEqtn_Op[settingsIndex].Enabled = false;

                // Enable control requires various checks to ensure it is appropriate. These complicated checks are below and commented.
                if (
                    (
                     // Avoid offering enable options for zero length and height and no shape defined, with a wrapping check in case of layout (where zero height/length are permitted)
                     (num_subshape_hl.Value != 0) &&
                     (num_subshape_vl.Value != 0) &&
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
                    cB_Layer.Enabled = true;

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.showDrawn, 0);
                    if (cB_Layer.Checked == true)
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.enabled, 1);
                        comboBox_geoEqtn_Op[settingsIndex].Enabled = true;
                        cB_ShowDrawn.Enabled = true;
                        if (cB_ShowDrawn.Checked == true)
                        {
                            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.showDrawn, 1);
                        }
                    }
                    else
                    {
                        cB_ShowDrawn.Enabled = false;
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.enabled, 0);
                    }
                }
                else
                {
                    cB_Layer.Checked = false;
                    cB_Layer.Enabled = false;
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.enabled, 0);
                    cB_ShowDrawn.Enabled = false;
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.showDrawn, 0);
                }

                doSimSettingsCheck();
                startButtonCheck();

                textBox_fileLocation_geoCore.Text = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.file);

                comboBox_subShapeRef.Enabled = true;
                comboBox_posSubShape.Enabled = true;

                num_hTipbias.Enabled = true;
                num_vTipbias.Enabled = true;
                num_hTipNVar.Enabled = true;
                num_hTipPVar.Enabled = true;
                num_vTipNVar.Enabled = true;
                num_vTipPVar.Enabled = true;

                num_lithoICRR.Enabled = true;
                num_lithoICV.Enabled = true;
                num_lithoCDUSide.Enabled = true;
                num_lithoCDUTips.Enabled = true;
                gB_CDUCorrelation.Enabled = true;
                gB_TipCDUCorrelation.Enabled = true;
                num_lithoLWR.Enabled = true;
                num_lithoLWRFreq.Enabled = true;
                cB_layer_LWRPreview.Enabled = true;
                comboBox_LWRNoiseType.Enabled = true;
                num_lithoOCRR.Enabled = true;
                num_lithoOCV.Enabled = true;
                num_rotation.Enabled = true;
                num_lithoWobble.Enabled = true;

                if (!commonVars.getLayerSettings(settingsIndex).isReloaded())
                {
                    // We do this to avoid any misfires later. Might not be needed, but seems safer.
                    GeoLibPointF[] defaultPointArray = new GeoLibPointF[1];
                    defaultPointArray[0] = new GeoLibPointF(0, 0);
                    commonVars.getLayerSettings(settingsIndex).setFileData(new List<GeoLibPointF[]>() { defaultPointArray });
                }

                if ((bool)cB_geoCore_shapeEngine.Checked)
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.gCSEngine, 1);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.gCSEngine, 0);
                }

                if ((bool)cB_geoCore_shapeEngine_perPoly.Checked)
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.perPoly, 1);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.perPoly, 0);
                }

                if ((bool)cB_geoCore_layoutReference.Checked)
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.refLayout, 1);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.refLayout, 0);
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE) // layout option selected.
                {
                    do2DLayerUI_geoCore(settingsIndex);
                }

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.BOOLEAN)
                {
                    do2DLayerUI_Boolean(settingsIndex);
                }

                layer_clampSubShape(minHLength: 0, maxHLength: 1000000, minVLength: 0, maxVLength: 1000000, minHOffset: -1000000, maxHOffset: 1000000, minVOffset: -1000000, maxVOffset: 1000000);

                bool[] warnArray = new bool[] { false, false, false };

                if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.none) || (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.rect))
                {
                    layer_clampSubShape2(minHLength: 0, 
                        maxHLength: 1000000, 
                        minVLength: 0, 
                        maxVLength: 1000000, 
                        minHOffset: -1000000, 
                        maxHOffset: 1000000, 
                        minVOffset: -1000000, 
                        maxVOffset: 1000000
                    );
                    layer_clampSubShape3(minHLength: 0, 
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
                        num_subshape_hl.Value = 0;
                        num_subshape_vl.Value = 0;
                        num_subshape_ho.Value = 0;
                        num_subshape_vo.Value = 0;
                    }

                    num_subshape2_hl.Value = 0;
                    num_subshape2_vl.Value = 0;
                    num_subshape2_ho.Value = 0;
                    num_subshape2_vo.Value = 0;

                    num_subshape3_hl.Value = 0;
                    num_subshape3_vl.Value = 0;
                    num_subshape3_ho.Value = 0;
                    num_subshape3_vo.Value = 0;

                }

                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_subshape_hl.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_subshape_ho.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_subshape_vl.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_subshape_vo.Value));

                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_subshape2_hl.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_subshape2_ho.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_subshape2_vl.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_subshape2_vo.Value));
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape1Tip, comboBox_tipLocations2.SelectedIndex);

                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_subshape3_hl.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_subshape3_ho.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_subshape3_vl.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_subshape3_vo.Value));
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape2Tip, comboBox_tipLocations3.SelectedIndex);

                warnArray[0] = ((commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) == 0) || (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) == 0));

                // Boolean and geoCore have their own handling for this.
                if ((commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.GEOCORE) && (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.BOOLEAN))
                {
                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape0Tip, comboBox_tipLocations.SelectedIndex);
                }

                // Subshape 2 offsets contingent on shape selection choice
                if (
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.none) &&
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.rect) &&
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.GEOCORE) &&
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (int)CommonVars.shapeNames.BOOLEAN)
                   )
                {
                    layer_clampSubShape(minHLength: 0.01, maxHLength: 1000000, minVLength: 0.01, maxVLength: 1000000, minHOffset: -1000000, maxHOffset: 1000000, minVOffset: -1000000, maxVOffset: 1000000);

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape1Tip, comboBox_tipLocations2.SelectedIndex);
                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Xshape) // Limit offsets of subshape 2 for X-shape.
                    {
                        do2DLayerUI_X(settingsIndex);
                    }
                    else if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Tshape) // Disabled horizontal offset of subshape 2 for T-shape.
                    {
                        do2DLayerUI_T(settingsIndex);
                    }
                    else if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Lshape) // Disable horizontal and vertical offsets of subshape 2 for L-shape
                    {
                        do2DLayerUI_L(settingsIndex);
                    }
                    else if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Ushape) // U-shape
                    {
                        do2DLayerUI_U(settingsIndex);
                    }
                    else if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Sshape) // S-shape
                    {
                        do2DLayerUI_S(settingsIndex);
                    }
                    else
                    {
                        num_subshape2_ho.Enabled = true;
                        num_subshape2_vo.Enabled = true;
                        commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_subshape2_ho.Value));
                        commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_subshape2_vo.Value));
                    }

                    warnArray[1] = ((commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) == 0) || (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength) == 0));

                    if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Sshape)
                    {
                        warnArray[2] = ((commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength) == 0) || (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength) == 0));
                    }
                }

                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape1Tip, comboBox_tipLocations2.SelectedIndex);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape2Tip, comboBox_tipLocations3.SelectedIndex);
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.subShapeIndex, comboBox_subShapeRef.SelectedIndex);
                if (comboBox_subShapeRef.SelectedIndex < 0)
                {
                    comboBox_subShapeRef.SelectedIndex = 0;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.posIndex, comboBox_posSubShape.SelectedIndex);
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.gHorOffset, Convert.ToDecimal(num_globalHorOffset.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.gVerOffset, Convert.ToDecimal(num_globalVerOffset.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.rot, Convert.ToDecimal(num_rotation.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.wobble, Convert.ToDecimal(num_lithoWobble.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.sBias, Convert.ToDecimal(num_sidebias.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.hTBias, Convert.ToDecimal(num_hTipbias.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.hTPVar, Convert.ToDecimal(num_hTipPVar.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.hTNVar, Convert.ToDecimal(num_hTipNVar.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.vTBias, Convert.ToDecimal(num_vTipbias.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.vTPVar, Convert.ToDecimal(num_vTipPVar.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.vTNVar, Convert.ToDecimal(num_vTipNVar.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.pBias, Convert.ToDecimal(num_pitchDepBias.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.pBiasDist, Convert.ToDecimal(num_pitchDepBiasIsoDistance.Value));
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.proxSideRaysFallOff, Convert.ToInt32(comboBox_proxBiasFallOff.SelectedIndex));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.proxSideRaysMultiplier, Convert.ToDecimal(num_proxBiasFallOffMultiplier.Value));
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.proxRays, Convert.ToInt32(num_pitchDepBiasSideRays.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.iCR, Convert.ToDecimal(num_lithoICRR.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.oCR, Convert.ToDecimal(num_lithoOCRR.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.iCV, Convert.ToDecimal(num_lithoICV.Value));
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.oCV, Convert.ToDecimal(num_lithoOCV.Value));

                do2DLayerUI_litho(settingsIndex, updateUI);

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
                    mcVPSettings[settingsIndex].drawDrawn((bool)cB_ShowDrawn.Checked);
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
                        warningString += (w + 1) + " ";
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

                listBox_layers.SelectedIndex = settingsIndex;
                resumeLayerUI_exp();
                customRNGMappingHighlight(settingsIndex);
                updateGroupBoxVisibility(settingsIndex);
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
                num_subshape2_hl.Enabled = true;
                num_subshape2_vl.Enabled = true;
                num_subshape2_ho.Enabled = true;
                num_subshape2_vo.Enabled = true;
                comboBox_tipLocations2.Enabled = true;

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.Sshape)
                {
                    num_subshape3_hl.Enabled = true;
                    num_subshape3_vl.Enabled = true;
                    num_subshape3_ho.Enabled = true;
                    num_subshape3_vo.Enabled = true;
                    comboBox_tipLocations3.Enabled = true;

                    commonVars.subshapes[settingsIndex].Clear();
                    commonVars.subshapes[settingsIndex].Add("1");
                    commonVars.subshapes[settingsIndex].Add("2");
                    commonVars.subshapes[settingsIndex].Add("3");
                }
                else
                {
                    num_subshape3_hl.Enabled = false;
                    num_subshape3_vl.Enabled = false;
                    num_subshape3_ho.Enabled = false;
                    num_subshape3_vo.Enabled = false;
                    comboBox_tipLocations3.Enabled = false;

                    commonVars.subshapes[settingsIndex].Clear();
                    commonVars.subshapes[settingsIndex].Add("1");
                    commonVars.subshapes[settingsIndex].Add("2");
                }
            }
            else
            {
                num_subshape2_hl.Enabled = false;
                num_subshape2_vl.Enabled = false;
                num_subshape2_ho.Enabled = false;
                num_subshape2_vo.Enabled = false;
                comboBox_tipLocations2.Enabled = false;

                num_subshape3_hl.Enabled = false;
                num_subshape3_vl.Enabled = false;
                num_subshape3_ho.Enabled = false;
                num_subshape3_vo.Enabled = false;
                comboBox_tipLocations3.Enabled = false;

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

        void do2DLayerUI_edgeSlide(int settingsIndex)
        {
            cB_edgeSlide.Enabled = true;
            num_edgeSlideTension.Enabled = true;

            if ((bool)cB_edgeSlide.Checked)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.edgeSlide, 1);
                num_edgeSlideTension.Enabled = true;
            }
            else
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.edgeSlide, 0);
                num_edgeSlideTension.Enabled = false;
            }

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.eTension, (decimal)num_edgeSlideTension.Value);
            if (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.eTension) < 1E-2m)
            {
                commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.eTension, 1E-2m);
            }
        }

        void do2DLayerUI_Boolean(int settingsIndex)
        {
            comboBox_subShapeRef.SelectedIndex = 0;
            comboBox_posSubShape.SelectedIndex = (int)CommonVars.subShapeLocations.BL;
            comboBox_subShapeRef.Enabled = false;
            comboBox_posSubShape.Enabled = false;

            geoGBVisible[settingsIndex] = false;
            subShapeGBVisible[settingsIndex] = false;
            booleanGBVisible[settingsIndex] = true;

            int aIndex = 0;
            int bIndex = 0;

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (rB_BooleanA[i].Checked)
                {
                    aIndex = i;
                }
                if (rB_BooleanB[i].Checked)
                {
                    bIndex = i;
                }
            }

            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerA, aIndex - 1); // offset for the 0 case
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerB, bIndex - 1); // offset for the 0 case

            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpA, comboBox_BooleanOpA.SelectedIndex);
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpB, comboBox_BooleanOpB.SelectedIndex);
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.bLayerOpAB, comboBox_BooleanOpAB.SelectedIndex);

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.rayExtension, Convert.ToDecimal(num_rayExtension.Value));

            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape0Tip, comboBox_TipLocations_boolean.SelectedIndex);
        }

        void do2DLayerUI_geoCore(int settingsIndex)
        {
            if ((varianceContext.vc.geoCoreCDVariation) || (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1))
            {
                gB_CDUCorrelation.Enabled = true; // CDU variation permitted.
                gB_TipCDUCorrelation.Enabled = true;
            }
            else
            {
                gB_CDUCorrelation.Enabled = false; // no CDU variation permitted.
                gB_TipCDUCorrelation.Enabled = false;
            }

            geoGBVisible[settingsIndex] = true;
            subShapeGBVisible[settingsIndex] = false;
            booleanGBVisible[settingsIndex] = false;

            // Enable UI elements if layout is valid/loaded.
            // Block UI elements if reloaded as we don't have the layout to move to a different structure/layer-datatype.
            // Need to check state of this conditional in case 'reference' is selected.
            btn_globalApply_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
            comboBox_structureList_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid() && !commonVars.getLayerSettings(settingsIndex).isReloaded();
            comboBox_lDList_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid() && !commonVars.getLayerSettings(settingsIndex).isReloaded();
            comboBox_tipLocations_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();
            cB_DOE_geoCore.Enabled = commonVars.getGeoCoreHandler(settingsIndex).isValid();

            // Disable subshapes
            comboBox_subShapeRef.SelectedIndex = 0;
            comboBox_posSubShape.SelectedIndex = (int)CommonVars.subShapeLocations.BL;
            comboBox_subShapeRef.Enabled = false;
            comboBox_posSubShape.Enabled = false;

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
            {
                comboBox_tipLocations_geoCore.Enabled = false;
                // Disable other features. Assuming PV bands are being used.
                num_hTipbias.Enabled = false;
                num_vTipbias.Enabled = false;
                num_hTipNVar.Enabled = false;
                num_hTipPVar.Enabled = false;
                num_vTipNVar.Enabled = false;
                num_vTipPVar.Enabled = false;

                num_lithoICRR.Enabled = false;
                num_lithoICV.Enabled = false;
                num_lithoCDUSide.Enabled = commonVars.getGCCDV();
                gB_CDUCorrelation.Enabled = commonVars.getGCCDV();
                num_lithoCDUTips.Enabled = false;
                gB_TipCDUCorrelation.Enabled = false;
                num_lithoOCRR.Enabled = false;
                num_lithoOCV.Enabled = false;
                num_rotation.Enabled = false;
                num_lithoWobble.Enabled = false;
            }

            // Fix subshape menu to meet our needs:
            commonVars.subshapes[settingsIndex].Clear();
            commonVars.subshapes[settingsIndex].Add("1");
            comboBox_subShapeRef.SelectedIndex = 0;
            comboBox_posSubShape.SelectedIndex = (int)CommonVars.subShapeLocations.BL;

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
            {
                // Force all values for unsupported properties to 0. Simplifies runtime code.
                num_lithoICRR.Value = 0.0;
                num_lithoICV.Value = 0.0;
                num_lithoOCRR.Value = 0.0;
                num_lithoOCV.Value = 0.0;
                if (!commonVars.getGCCDV())
                {
                    num_lithoCDUSide.Value = 0.0;
                }
                num_lithoCDUTips.Value = 0.0;
                num_hTipbias.Value = 0.0;
                num_hTipNVar.Value = 0.0;
                num_hTipPVar.Value = 0.0;
                num_vTipbias.Value = 0.0;
                num_vTipNVar.Value = 0.0;
                num_vTipPVar.Value = 0.0;
                num_rotation.Value = 0.0;
                num_lithoWobble.Value = 0.0;
                num_subshape_ho.Value = 0.0;
                num_subshape2_ho.Value = 0.0;
                num_subshape3_ho.Value = 0.0;
                num_subshape_vo.Value = 0.0;
                num_subshape2_vo.Value = 0.0;
                num_subshape3_vo.Value = 0.0;
            }

            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape0Tip, comboBox_tipLocations_geoCore.SelectedIndex);
            }

            // Force update of comboboxes
            if (commonVars.getGeoCoreHandler(settingsIndex).isValid())// && (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1))
            {
                // Avoid triggering repaints as we update things.
                comboBox_polyFill_geoCore.SelectedIndexChanged -= twoDLayerEventHandler_exp;
                comboBox_lDList_geoCore.SelectedIndexChanged -= twoDLayerEventHandler_exp;
                comboBox_structureList_geoCore.SelectedIndexChanged -= twoDLayerEventHandler_exp;
                if (!commonVars.getLayerSettings(settingsIndex).isReloaded())
                {

                    commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.fill, comboBox_polyFill_geoCore.SelectedIndex);

                    if (commonVars.getGeoCoreHandler(settingsIndex).isChanged())
                    {
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.structure, 0);
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lD, 0);
                        commonVars.getGeoCoreHandler(settingsIndex).setChanged(false);
                    }
                    else
                    {
                        if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure) != comboBox_structureList_geoCore.SelectedIndex)
                        {
                            commonVars.getGeoCoreHandler(settingsIndex).setChanged(true);
                            try
                            {
                                comboBox_lDList_geoCore.SelectedIndex = 0; // reset selection.
                            }
                            catch (Exception)
                            {

                            }
                        }
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.structure, comboBox_structureList_geoCore.SelectedIndex);

                        if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD) != comboBox_lDList_geoCore.SelectedIndex)
                        {
                            commonVars.getGeoCoreHandler(settingsIndex).setChanged(true);
                        }
                        commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lD, comboBox_lDList_geoCore.SelectedIndex);
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
                        comboBox_structureList_geoCore.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.structure);
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        comboBox_lDList_geoCore.SelectedIndex = commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.lD);
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
                    comboBox_structureList_geoCore.SelectedIndex = 0;
                    comboBox_lDList_geoCore.SelectedIndex = 0;
                }
                // Enable repaints for updated things.
                comboBox_polyFill_geoCore.SelectedIndexChanged += twoDLayerEventHandler_exp;
                comboBox_lDList_geoCore.SelectedIndexChanged += twoDLayerEventHandler_exp;
                comboBox_structureList_geoCore.SelectedIndexChanged += twoDLayerEventHandler_exp;

                if ((bool)cB_DOE_geoCore.Checked)
                {
                    commonVars.getSimulationSettings().getDOESettings().setLayerAffected(settingsIndex, 1);
                }
                else
                {
                    commonVars.getSimulationSettings().getDOESettings().setLayerAffected(settingsIndex, 0);
                }
            }
        }

        void do2DLayerUI_X(int settingsIndex)
        {
            // Validate our settings and clamp the inputs as needed.
            layer_clampSubShape(minHLength: 0.04, 
                maxHLength: 1000000, 
                minVLength: 0.04, 
                maxVLength: 1000000, 
                minHOffset: -1000000, 
                maxHOffset: 1000000, 
                minVOffset: -1000000, 
                maxVOffset: 1000000
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_subshape_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_subshape_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_subshape_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_subshape_vo.Value));

            num_subshape3_hl.Value = 0;
            num_subshape3_vl.Value = 0;
            num_subshape3_ho.Value = 0;
            num_subshape3_vo.Value = 0;

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

            layer_clampSubShape2(minHLength: (double)minSS2HLength, 
                maxHLength: 1000000, 
                minVLength: 0.02, 
                maxVLength: (double)maxSS2VLength, 
                minHOffset: (double)minSS2HOffset, 
                maxHOffset: (double)maxSS2HOffset, 
                minVOffset: (double)minSS2VOffset, 
                maxVOffset: (double)maxSS2VOffset
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_subshape2_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_subshape2_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_subshape2_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_subshape2_vo.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_subshape3_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_subshape3_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_subshape3_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_subshape3_vo.Value));

            num_subshape2_ho.Enabled = true;
            num_subshape2_vo.Enabled = true;
        }

        void do2DLayerUI_T(int settingsIndex)
        {
            // Validate our settings and clamp the inputs as needed.
            layer_clampSubShape(minHLength: 0.01, 
                maxHLength: 1000000, 
                minVLength: 0.04, 
                maxVLength: 1000000, 
                minHOffset: -1000000, 
                maxHOffset: 1000000, 
                minVOffset: -1000000, 
                maxVOffset: 1000000
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_subshape_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_subshape_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_subshape_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_subshape_vo.Value));

            num_subshape3_hl.Value = 0;
            num_subshape3_vl.Value = 0;
            num_subshape3_ho.Value = 0;
            num_subshape3_vo.Value = 0;

            decimal minSS2HLength = 0.01m;
            decimal minSS2VLength = 0.02m;
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - (2 * 0.01m);
            decimal maxSS2VOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);

            layer_clampSubShape2(minHLength: (double)minSS2HLength, 
                maxHLength: 1000000, 
                minVLength: (double)minSS2VLength, 
                maxVLength: (double)maxSS2VLength,
                minHOffset: (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength),
                maxHOffset: (double)commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength),
                minVOffset: 1, 
                maxVOffset: (double)maxSS2VOffset
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_subshape2_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_subshape2_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_subshape2_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_subshape2_vo.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_subshape3_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_subshape3_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_subshape3_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_subshape3_vo.Value));

            num_subshape2_ho.Enabled = false;
            num_subshape2_vo.Enabled = true;
        }

        void do2DLayerUI_L(int settingsIndex)
        {
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_subshape_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_subshape_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_subshape_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_subshape_vo.Value));

            num_subshape3_hl.Value = 0;
            num_subshape3_vl.Value = 0;
            num_subshape3_ho.Value = 0;
            num_subshape3_vo.Value = 0;

            decimal minSS2HLength = 0;
            decimal minSS2VLength = 0;
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength);

            decimal minSS2HOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength);
            decimal maxSS2HOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength);
            decimal minSS2VOffset = 0;
            decimal maxSS2VOffset = 0;

            layer_clampSubShape2(minHLength: (double)minSS2HLength, 
                maxHLength: 1000000, 
                minVLength: (double)minSS2VLength, 
                maxVLength: (double)maxSS2VLength,
                minHOffset: (double)minSS2HOffset,
                maxHOffset: (double)maxSS2HOffset,
                minVOffset: (double)minSS2VOffset, 
                maxVOffset: (double)maxSS2VOffset
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_subshape2_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_subshape2_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_subshape2_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_subshape2_vo.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_subshape3_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_subshape3_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_subshape3_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_subshape3_vo.Value));

            num_subshape2_ho.Enabled = false;
            num_subshape2_vo.Enabled = false;
        }

        void do2DLayerUI_U(int settingsIndex)
        {
            // Validate our settings and clamp the inputs as needed.
            layer_clampSubShape(minHLength: 0.04, 
                maxHLength: 1000000, 
                minVLength: 0.04, 
                maxVLength: 1000000, 
                minHOffset: -1000000, 
                maxHOffset: 1000000, 
                minVOffset: -1000000, 
                maxVOffset: 1000000
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_subshape_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_subshape_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_subshape_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_subshape_vo.Value));

            num_subshape3_hl.Value = 0;
            num_subshape3_vl.Value = 0;
            num_subshape3_ho.Value = 0;
            num_subshape3_vo.Value = 0;

            decimal minSS2HLength = 0.02m;
            decimal minSS2VLength = 0.02m;
            decimal maxSS2HLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) - 0.02m;
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - 0.02m;

            decimal ss2HOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) - 0.02m;
            decimal ss2VOffset = (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));

            layer_clampSubShape2(minHLength: (double)minSS2HLength, 
                maxHLength: (double)maxSS2HLength, 
                minVLength: (double)minSS2VLength, 
                maxVLength: (double)maxSS2VLength,
                minHOffset: 0.02f, 
                maxHOffset: (double)ss2HOffset,
                minVOffset: (double)ss2VOffset, 
                maxVOffset: (double)ss2VOffset
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_subshape2_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_subshape2_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_subshape2_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_subshape2_vo.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_subshape3_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_subshape3_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_subshape3_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_subshape3_vo.Value));

            num_subshape2_ho.Enabled = true;
            num_subshape2_vo.Enabled = false;
        }

        void do2DLayerUI_S(int settingsIndex)
        {
            // Validate our settings and clamp the inputs as needed.
            layer_clampSubShape(minHLength: 0.04, 
                maxHLength: 1000000, 
                minVLength: 0.04, 
                maxVLength: 1000000, 
                minHOffset: -1000000, 
                maxHOffset: 1000000, 
                minVOffset: -1000000, 
                maxVOffset: 1000000
            );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(num_subshape_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset, Convert.ToDecimal(num_subshape_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(num_subshape_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset, Convert.ToDecimal(num_subshape_vo.Value));

            decimal minSS2HLength = 0.01m;
            decimal maxSS2HLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) - 0.01m;
            decimal minSS2VLength = 0.02m;
            decimal maxSS2VLength = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - 0.01m;
            decimal ss2HOffset = 0;
            decimal minSS2VOffset = 0.01m;
            decimal maxSS2VOffset = commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength) - commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);
            layer_clampSubShape2(minHLength: (double)minSS2HLength, 
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
            layer_clampSubShape3(minHLength: (double)minSS3HLength, 
                maxHLength: (double)maxSS3HLength, 
                minVLength: (double)minSS3VLength, 
                maxVLength: (double)maxSS3VLength,
                minHOffset: (double)ss3HOffset, 
                maxHOffset: (double)ss3HOffset, 
                minVOffset: (double)minSS3VOffset, 
                maxVOffset: (double)maxSS3VOffset
                );

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorLength, Convert.ToDecimal(num_subshape2_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerLength, Convert.ToDecimal(num_subshape2_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset, Convert.ToDecimal(num_subshape2_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset, Convert.ToDecimal(num_subshape2_vo.Value));

            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorLength, Convert.ToDecimal(num_subshape3_hl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerLength, Convert.ToDecimal(num_subshape3_vl.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset, Convert.ToDecimal(num_subshape3_ho.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset, Convert.ToDecimal(num_subshape3_vo.Value));

            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.shape2Tip, comboBox_tipLocations3.SelectedIndex);

            // FIXME: Need some logic here to avoid bisection of the S.

            num_subshape2_ho.Enabled = false;
            num_subshape2_vo.Enabled = true;

            num_subshape3_ho.Enabled = false;
            num_subshape3_vo.Enabled = true;
        }

        void do2DLayerUI_litho(int settingsIndex, bool updateUI)
        {
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lDC1, Convert.ToDecimal(num_coeff1.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lDC2, Convert.ToDecimal(num_coeff2.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lwr, Convert.ToDecimal(num_lithoLWR.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lwrFreq, Convert.ToDecimal(num_lithoLWRFreq.Value));
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwrType, comboBox_LWRNoiseType.SelectedIndex);
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lwr2, Convert.ToDecimal(num_lithoLWR2.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq, Convert.ToDecimal(num_lithoLWR2Freq.Value));
            commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwr2Type, comboBox_LWR2NoiseType.SelectedIndex);
            if ((bool)cB_layer_LWRPreview.Checked)
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwrPreview, 1);
            }
            else
            {
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.lwrPreview, 0);
            }
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.sCDU, Convert.ToDecimal(num_lithoCDUSide.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.tCDU, Convert.ToDecimal(num_lithoCDUTips.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.xOL, Convert.ToDecimal(num_lithoHorOverlay.Value));
            commonVars.getLayerSettings(settingsIndex).setDecimal(EntropyLayerSettings.properties_decimal.yOL, Convert.ToDecimal(num_lithoVerOverlay.Value));

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
                if (rB_COLX[i].Checked)
                {
                    COLXIndex = i;
                }
                if (rB_COLY[i].Checked)
                {
                    COLYIndex = i;
                }
                if (rB_OLRX[i].Checked)
                {
                    OLRXIndex = i;
                }
                if (rB_OLRY_exp[i].Checked)
                {
                    OLRYIndex = i;
                }
                if (rB_CLWR[i].Checked)
                {
                    CLWRIndex = i;
                }
                if (rB_CLWR2[i].Checked)
                {
                    CLWR2Index = i;
                }
                if (rB_CCDU[i].Checked)
                {
                    CCDUIndex = i;
                }
                if (rB_CTCDU[i].Checked)
                {
                    CTCDUIndex = i;
                }

                if (i != settingsIndex)
                {
                    if ((bool)cB_OLRX_Av[i].Checked)
                    {
                        commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, i, 1);
                    }
                    else
                    {
                        commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, i, 0);
                    }

                    if ((bool)cB_layer_OLRY_Av[i].Checked)
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
                int xcolRefIndex = Convert.ToInt32(rB_COLX[COLXIndex].Text);
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
                int ycolRefIndex = Convert.ToInt32(rB_COLY[COLYIndex].Text);
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
                int xolRefIndex = Convert.ToInt32(rB_OLRX[OLRXIndex].Text);
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
                int yolRefIndex = Convert.ToInt32(rB_OLRY_exp[OLRYIndex].Text);
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
                int clwrRefIndex = Convert.ToInt32(rB_CLWR[CLWRIndex].Text);
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
                int clwr2RefIndex = Convert.ToInt32(rB_CLWR2[CLWRIndex].Text);
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
                int ccduRefIndex = Convert.ToInt32(rB_CCDU[CCDUIndex].Text);
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
                int ctcduRefIndex = Convert.ToInt32(rB_CTCDU[CTCDUIndex].Text);
                if (ctcduRefIndex > settingsIndex)
                {
                    ctcduRefIndex--; // compensate for missing radio button for current layer.
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, ctcduRefIndex - 1);
            }

            // Process the average overlay arrays.
            if (cB_overlayXReference_Av.Checked == true)
            {
                if (updateUI)
                {
                    pnl_overlayRefX.Content = gB_overlayXReference_Av;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_av, 1);
            }
            else
            {
                if (updateUI)
                {
                    pnl_overlayRefX.Content = gB_overlayXReference;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.xOL_av, 0);
            }

            if ((bool)cB_overlayYReference_Av.Checked)
            {
                if (updateUI)
                {
                    pnl_overlayRefY.Content = gB_overlayYReference_Av;
                }
                commonVars.getLayerSettings(settingsIndex).setInt(EntropyLayerSettings.properties_i.yOL_av, 1);
            }
            else
            {
                if (updateUI)
                {
                    pnl_overlayRefY.Content = gB_overlayYReference;
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
                    cB_bg[i].Text = name;
                    if (settingsIndex != i)
                    {
                        cB_bg[i].Enabled = (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1);
                        cB_bg[i].Checked = ((commonVars.getLayerSettings(settingsIndex).getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, i) == 1) && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1));
                    }
                    else
                    {
                        cB_bg[i].Enabled = false;
                        cB_bg[i].Checked = false;
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
                                rB_BooleanA[0].Checked = true;
                            }
                            cB_omit[boolLayer].Checked = false;
                            cB_omit[boolLayer].Enabled = false;
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
                                rB_BooleanB[0].Checked = true;
                            }
                            cB_omit[boolLayer].Checked = false;
                            cB_omit[boolLayer].Enabled = false;
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
                cB_omit[i].Text = name;
                cB_omit[i].Enabled = bLayerStatus[i];
            }
        }

        void showDrawn(int settingsIndex)
        {
            bool showDrawn = (bool)cB_ShowDrawn.Checked;
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

        void updateGroupBoxVisibility(int layer)
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
                layerShapeProperties_tcPanel.Content = gB_subShapes;
                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
                comboBox_tipLocations.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                comboBox_tipLocations2.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape1Tip);
                comboBox_tipLocations3.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape2Tip);
            }
            else if (geoGBVisible[layer])
            {
                layerShapeProperties_tcPanel.Content = gB_layer_geoCore;
                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
                comboBox_tipLocations_geoCore.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                comboBox_polyFill_geoCore.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.fill);
                try
                {
                    comboBox_structureList_geoCore.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.structure);
                }
                catch (Exception)
                {

                }
                try
                {
                    comboBox_lDList_geoCore.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.lD);
                }
                catch (Exception)
                {

                }
            }
            else if (booleanGBVisible[layer])
            {
                layerShapeProperties_tcPanel.Content = gB_layerBoolean;
                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
                comboBox_TipLocations_boolean.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.shape0Tip);
                comboBox_BooleanOpA.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerOpA);
                comboBox_BooleanOpAB.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerOpAB);
                comboBox_BooleanOpB.SelectedIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerOpB);
                num_rayExtension.Value = Convert.ToDouble(commonVars.getLayerSettings(layer).getDecimal(EntropyLayerSettings.properties_decimal.rayExtension));
            }
            else
            {
                layerShapeProperties_tcPanel.Content = null;
                setLayerPropertiesContent(ref layerShapeProperties_tcPanel);
            }

            layerUIFrozen_exp = alreadyFrozen;
        }

        void customRNGMappingHighlight(int settingsIndex)
        {
            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.sCDU_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoCDUSide.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoCDUSide.TextColor = SystemColors.ControlText;
            }
            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.tCDU_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoCDUTips.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoCDUTips.TextColor = SystemColors.ControlText;
            }
            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.xOL_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoHorOverlay.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoHorOverlay.TextColor = SystemColors.ControlText;
            }
            lbl_lithoHorOverlay.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.xOL_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.yOL_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoVerOverlay.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoVerOverlay.TextColor = SystemColors.ControlText;
            }
            lbl_lithoVerOverlay.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.yOL_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.iCV_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoICV.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoICV.TextColor = SystemColors.ControlText;
            }
            lbl_lithoICV.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.iCV_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.oCV_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoOCV.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoOCV.TextColor = SystemColors.ControlText;
            }
            lbl_lithoOCV.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.oCV_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.wobble_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoWobble.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoWobble.TextColor = SystemColors.ControlText;
            }
            lbl_lithoWobble.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.wobble_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.hTipNVar_RNG) != CommonVars.boxMuller)
            {
                lbl_hTipNVar.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_hTipNVar.TextColor = SystemColors.ControlText;
            }
            lbl_hTipNVar.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.hTipNVar_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.hTipPVar_RNG) != CommonVars.boxMuller)
            {
                lbl_hTipPVar.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_hTipPVar.TextColor = SystemColors.ControlText;
            }
            lbl_hTipPVar.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.hTipPVar_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.vTipNVar_RNG) != CommonVars.boxMuller)
            {
                lbl_vTipNVar.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_vTipNVar.TextColor = SystemColors.ControlText;
            }
            lbl_vTipNVar.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.vTipNVar_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.vTipPVar_RNG) != CommonVars.boxMuller)
            {
                lbl_vTipPVar.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_vTipPVar.TextColor = SystemColors.ControlText;
            }
            lbl_vTipPVar.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.vTipPVar_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lwr_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoLWR.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoLWR.TextColor = SystemColors.ControlText;
            }
            lbl_lithoLWR.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lwr_RNG);

            if (commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lwr2_RNG) != CommonVars.boxMuller)
            {
                lbl_lithoLWR2.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
            }
            else
            {
                lbl_lithoLWR2.TextColor = SystemColors.ControlText;
            }
            lbl_lithoLWR2.ToolTip = commonVars.getLayerSettings(settingsIndex).getString(EntropyLayerSettings.properties_s.lwr2_RNG);
        }
    }
}
