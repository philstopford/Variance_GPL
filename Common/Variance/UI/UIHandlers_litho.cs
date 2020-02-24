using Eto.Forms;
using System;

namespace Variance
{
    public partial class MainForm
    {
        void updateLayerRadioButtons_exp()
        {
            // Set initial state based on layer enabled mode.

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                // Base enabled status
                bool enabled = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;

                for (int j = 0; j < CentralProperties.maxLayersForMC; j++)
                {
                    // Disable corresponding button for reference layer in UI.
                    bool colx_enabled = enabled;
                    if (colx_enabled && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.xOL_corr) == 1))
                    {
                        // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                        if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == j)
                        {
                            colx_enabled = false;
                        }
                    }
                    xCOLRBs_enabledState[j][i] = colx_enabled;

                    bool coly_enabled = enabled;
                    if (coly_enabled && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.yOL_corr) == 1))
                    {
                        // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                        if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == j)
                        {
                            coly_enabled = false;
                        }
                    }
                    yCOLRBs_enabledState[j][i] = coly_enabled;

                    bool olrx_enabled = enabled;
                    if (olrx_enabled)
                    {
                        // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                        if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.xOL_ref) == j)
                        {
                            olrx_enabled = false;
                        }
                    }
                    xOLRBs_enabledState[j][i] = olrx_enabled;

                    bool olry_enabled = enabled;
                    if (olry_enabled)
                    {
                        // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                        if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.yOL_ref) == j)
                        {
                            olry_enabled = false;
                        }
                    }
                    yOLRBs_enabledState[j][i] = olry_enabled;

                    bool ccdu_enabled = enabled;
                    if (ccdu_enabled && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.CDU_corr) == 1))
                    {
                        // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                        if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == j)
                        {
                            ccdu_enabled = false;
                        }
                    }
                    SCDURBs_enabledState[j][i] = ccdu_enabled;

                    bool tcdu_enabled = enabled;
                    if (tcdu_enabled && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.tCDU_corr) == 1))
                    {
                        // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                        if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == j)
                        {
                            tcdu_enabled = false;
                        }
                    }
                    TCDURBs_enabledState[j][i] = tcdu_enabled;
                }
            }
        }

        void updateLayerCDURadioButtons_exp(int layer)
        {
            Application.Instance.Invoke(() =>
            {
                // -1 is off, which is the 0th radio button.
                int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) + 1;

                rB_layer_CCDU_exp[0].Enabled = true;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (i == layer)
                    {
                        // No button for current layer, so skip this pass through the loop.
                        continue;
                    }
                    else
                    {
                        int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                        if (i > layer)
                        {
                            bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                        }
                        rB_layer_CCDU_exp[bIndex].Text = (i + 1).ToString(); // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        rB_layer_CCDU_exp[bIndex].Enabled = SCDURBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                    }
                }

                if (sIndex > layer)
                {
                    sIndex--;
                }

                rB_layer_CCDU_exp[sIndex].Checked = true;
            });
        }

        void updateLayerTCDURadioButtons_exp(int layer)
        {
            Application.Instance.Invoke(() =>
            {
                // -1 is off, which is the 0th radio button.
                int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) + 1;

                rB_layer_CTCDU_exp[0].Enabled = true;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (i == layer)
                    {
                        // No button for current layer, so skip this pass through the loop.
                        continue;
                    }
                    else
                    {
                        int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                        if (i > layer)
                        {
                            bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                        }
                        rB_layer_CTCDU_exp[bIndex].Text = (i + 1).ToString(); // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        rB_layer_CTCDU_exp[bIndex].Enabled = TCDURBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                    }
                }

                if (sIndex > layer)
                {
                    sIndex--;
                }

                rB_layer_CTCDU_exp[sIndex].Checked = true;
            });
        }

        void updateLayerOLRXRadioButtons_exp(int layer)
        {
            Application.Instance.Invoke(() =>
            {
                // -1 is off, which is the 0th radio button.
                int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.xOL_ref) + 1;

                rB_layer_OLRX_exp[0].Enabled = true;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (i == layer)
                    {
                        // No button for current layer, so skip this pass through the loop.
                        continue;
                    }
                    else
                    {
                        int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                        if (i > layer)
                        {
                            bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                        }
                        rB_layer_OLRX_exp[bIndex].Text = (i + 1).ToString(); // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        rB_layer_OLRX_exp[bIndex].Enabled = xOLRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                    }
                }

                if (sIndex > layer)
                {
                    sIndex--;
                }

                rB_layer_OLRX_exp[sIndex].Checked = true;
            });
        }

        void updateLayerOLRYRadioButtons_exp(int layer)
        {
            Application.Instance.Invoke(() =>
            {
                // -1 is off, which is the 0th radio button.
                int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.yOL_ref) + 1;

                rB_layer_OLRY_exp[0].Enabled = true;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (i == layer)
                    {
                        // No button for current layer, so skip this pass through the loop.
                        continue;
                    }
                    else
                    {
                        int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                        if (i > layer)
                        {
                            bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                        }
                        rB_layer_OLRY_exp[bIndex].Text = (i + 1).ToString(); // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        rB_layer_OLRY_exp[bIndex].Enabled = yOLRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                    }
                }

                if (sIndex > layer)
                {
                    sIndex--;
                }

                rB_layer_OLRY_exp[sIndex].Checked = true;
            });
        }

        void updateLayerCOLXRadioButtons_exp(int layer)
        {
            Application.Instance.Invoke(() =>
            {
                // -1 is off, which is the 0th radio button.
                int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) + 1;

                rB_layer_COLX_exp[0].Enabled = true;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (i == layer)
                    {
                        // No button for current layer, so skip this pass through the loop.
                        continue;
                    }
                    else
                    {
                        int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                        if (i > layer)
                        {
                            bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                        }
                        rB_layer_COLX_exp[bIndex].Text = (i + 1).ToString(); // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        rB_layer_COLX_exp[bIndex].Enabled = xCOLRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                    }
                }

                if (sIndex > layer)
                {
                    sIndex--;
                }

                rB_layer_COLX_exp[sIndex].Checked = true;
            });
        }

        void updateLayerCOLYRadioButtons_exp(int layer)
        {
            Application.Instance.Invoke(() =>
            {
                // -1 is off, which is the 0th radio button.
                int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) + 1;

                rB_layer_COLY_exp[0].Enabled = true;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (i == layer)
                    {
                        // No button for current layer, so skip this pass through the loop.
                        continue;
                    }
                    else
                    {
                        int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                        if (i > layer)
                        {
                            bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                        }
                        rB_layer_COLY_exp[bIndex].Text = (i + 1).ToString(); // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        rB_layer_COLY_exp[bIndex].Enabled = yCOLRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                    }
                }

                if (sIndex > layer)
                {
                    sIndex--;
                }

                rB_layer_COLY_exp[sIndex].Checked = true;
            });
        }

        void updateLayerBooleanRadioButtons_exp(int layer)
        {
            Application.Instance.Invoke(() =>
            {
                // -1 is off, which is the 0th radio button.
                int sIndex1 = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerA) + 1;
                int sIndex2 = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerB) + 1;

                rB_layerBooleanA_exp[0].Enabled = true;
                rB_layerBooleanB_exp[0].Enabled = true;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (i == layer)
                    {
                        continue;
                    }
                    else
                    {
                        int bIndex = i + 1;
                        if (i > layer)
                        {
                            bIndex = i;
                        }
                        rB_layerBooleanA_exp[bIndex].Text = (i + 1).ToString();
                        rB_layerBooleanA_exp[bIndex].Enabled = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                        rB_layerBooleanB_exp[bIndex].Text = (i + 1).ToString();
                        rB_layerBooleanB_exp[bIndex].Enabled = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                    }
                }

                if (sIndex1 > layer)
                {
                    sIndex1--;
                }
                if (sIndex2 > layer)
                {
                    sIndex2--;
                }

                rB_layerBooleanA_exp[sIndex1].Checked = true;
                rB_layerBooleanB_exp[sIndex2].Checked = true;
            });
        }

        void updateAverageOverlayCheckboxes_exp(Int32 index)
        {
            bool alreadyFrozen = globalUIFrozen;
            globalUIFrozen = true;
            Application.Instance.Invoke(() =>
            {
                int rowIndex = 0;
                int colIndex = 0;
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (index == i)
                    {
                        cB_layer_OLRX_Av_exp[i].Enabled = false;
                        cB_layer_OLRX_Av_exp[i].Checked = false;

                        cB_layer_OLRY_Av_exp[i].Enabled = false;
                        cB_layer_OLRY_Av_exp[i].Checked = false;
                    }
                    else
                    {
                        cB_layer_OLRX_Av_exp[i].Enabled = (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1);
                        cB_layer_OLRX_Av_exp[i].Checked = ((commonVars.getLayerSettings(index).getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, i) == 1) && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1));

                        cB_layer_OLRY_Av_exp[i].Enabled = (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1);
                        cB_layer_OLRY_Av_exp[i].Checked = ((commonVars.getLayerSettings(index).getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, i) == 1) && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1));
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
    }
}
