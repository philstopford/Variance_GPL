using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private void updateLayerRadioButtons_exp()
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
                if (colx_enabled && commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.xOL_corr) == 1)
                {
                    // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                    if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == j)
                    {
                        colx_enabled = false;
                    }
                }
                xCOLRBs_enabledState[j][i] = colx_enabled;

                bool coly_enabled = enabled;
                if (coly_enabled && commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.yOL_corr) == 1)
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
                if (ccdu_enabled && commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.CDU_corr) == 1)
                {
                    // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                    if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == j)
                    {
                        ccdu_enabled = false;
                    }
                }
                SCDURBs_enabledState[j][i] = ccdu_enabled;

                bool tcdu_enabled = enabled;
                if (tcdu_enabled && commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.tCDU_corr) == 1)
                {
                    // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                    if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == j)
                    {
                        tcdu_enabled = false;
                    }
                }
                TCDURBs_enabledState[j][i] = tcdu_enabled;

                bool lwr_enabled = enabled;
                if (lwr_enabled && commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.lwr_corr) == 1)
                {
                    // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                    if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.lwr_corr) == j)
                    {
                        lwr_enabled = false;
                    }
                }
                CLWRRBs_enabledState[j][i] = lwr_enabled;

                bool lwr2_enabled = enabled;
                if (lwr2_enabled && commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.lwr2_corr) == 1)
                {
                    // If i depends on j, we cannot allow j to enable dependency on i. First to assert a dependency, in layer order, wins.
                    if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.lwr2_corr) == j)
                    {
                        lwr2_enabled = false;
                    }
                }
                CLWR2RBs_enabledState[j][i] = lwr2_enabled;
            }
        }
    }

    private void updateLayerLWRRadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.lwr_corr_ref) + 1;

            rB_CLWR[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                    // No button for current layer, so skip this pass through the loop.
                }
                else
                {
                    int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                    if (i > layer)
                    {
                        bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_CLWR[bIndex].Text = name; 
                    rB_CLWR[bIndex].Enabled = CLWRRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                }
            }

            if (sIndex > layer)
            {
                sIndex--;
            }

            rB_CLWR[sIndex].Checked = true;
        });
    }

    private void updateLayerLWR2RadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.lwr2_corr_ref) + 1;

            rB_CLWR2[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                    // No button for current layer, so skip this pass through the loop.
                }
                else
                {
                    int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                    if (i > layer)
                    {
                        bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_CLWR2[bIndex].Text = name;
                    rB_CLWR2[bIndex].Enabled = CLWR2RBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                }
            }

            if (sIndex > layer)
            {
                sIndex--;
            }

            rB_CLWR2[sIndex].Checked = true;
        });
    }

    private void updateLayerCDURadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) + 1;

            rB_CCDU[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                    // No button for current layer, so skip this pass through the loop.
                }
                else
                {
                    int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                    if (i > layer)
                    {
                        bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_CCDU[bIndex].Text = name;
                    rB_CCDU[bIndex].Enabled = SCDURBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                }
            }

            if (sIndex > layer)
            {
                sIndex--;
            }

            rB_CCDU[sIndex].Checked = true;
        });
    }

    private void updateLayerTCDURadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) + 1;

            rB_CTCDU[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                    // No button for current layer, so skip this pass through the loop.
                }
                else
                {
                    int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                    if (i > layer)
                    {
                        bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_CTCDU[bIndex].Text = name;
                    rB_CTCDU[bIndex].Enabled = TCDURBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                }
            }

            if (sIndex > layer)
            {
                sIndex--;
            }

            rB_CTCDU[sIndex].Checked = true;
        });
    }

    private void updateLayerOLRXRadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.xOL_ref) + 1;

            rB_OLRX[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                    // No button for current layer, so skip this pass through the loop.
                }
                else
                {
                    int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                    if (i > layer)
                    {
                        bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_OLRX[bIndex].Text = name;
                    rB_OLRX[bIndex].Enabled = xOLRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                }
            }

            if (sIndex > layer)
            {
                sIndex--;
            }

            rB_OLRX[sIndex].Checked = true;
        });
    }

    private void updateLayerOLRYRadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.yOL_ref) + 1;

            rB_OLRY_exp[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                    // No button for current layer, so skip this pass through the loop.
                }
                else
                {
                    int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                    if (i > layer)
                    {
                        bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_OLRY_exp[bIndex].Text = name;
                    rB_OLRY_exp[bIndex].Enabled = yOLRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                }
            }

            if (sIndex > layer)
            {
                sIndex--;
            }

            rB_OLRY_exp[sIndex].Checked = true;
        });
    }

    private void updateLayerCOLXRadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) + 1;

            rB_COLX[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                    // No button for current layer, so skip this pass through the loop.
                }
                else
                {
                    int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                    if (i > layer)
                    {
                        bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_COLX[bIndex].Text = name;
                    rB_COLX[bIndex].Enabled = xCOLRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                }
            }

            if (sIndex > layer)
            {
                sIndex--;
            }

            rB_COLX[sIndex].Checked = true;
        });
    }

    private void updateLayerCOLYRadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) + 1;

            rB_COLY[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                    // No button for current layer, so skip this pass through the loop.
                }
                else
                {
                    int bIndex = i + 1; // offset button index by 1 as the 0-index is the 'off' button.
                    if (i > layer)
                    {
                        bIndex = i; // if we are above our layer, decrement (effectively) as we don't have a button for the current layer, so have to compensate the positional index.
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_COLY[bIndex].Text = name;
                    rB_COLY[bIndex].Enabled = yCOLRBs_enabledState[layer][i]; // don't offset this because the enabled array is matched to the full number of layers.
                }
            }

            if (sIndex > layer)
            {
                sIndex--;
            }

            rB_COLY[sIndex].Checked = true;
        });
    }

    private void updateLayerBooleanRadioButtons_exp(int layer)
    {
        Application.Instance.Invoke(() =>
        {
            // -1 is off, which is the 0th radio button.
            int sIndex1 = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerA) + 1;
            int sIndex2 = commonVars.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.bLayerB) + 1;

            rB_BooleanA[0].Enabled = true;
            rB_BooleanB[0].Enabled = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (i == layer)
                {
                }
                else
                {
                    int bIndex = i + 1;
                    if (i > layer)
                    {
                        bIndex = i;
                    }
                    string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                    if (name == "")
                    {
                        // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                        name = (i + 1).ToString();
                    }
                    rB_BooleanA[bIndex].Text = name;
                    rB_BooleanA[bIndex].Enabled = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                    rB_BooleanB[bIndex].Text = name;
                    rB_BooleanB[bIndex].Enabled = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
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

            rB_BooleanA[sIndex1].Checked = true;
            rB_BooleanB[sIndex2].Checked = true;
        });
    }

    private void updateAverageOverlayCheckboxes_exp(int index)
    {
        bool alreadyFrozen = globalUIFrozen;
        globalUIFrozen = true;
        Application.Instance.Invoke(() =>
        {
            int colIndex = 0;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                string name = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
                if (name == "")
                {
                    // add 1 as the arrays are 0-indexed, so the button text need to be compensated.
                    name = (i + 1).ToString();
                }
                cB_OLRX_Av[i].Text = name;
                cB_layer_OLRY_Av[i].Text = name;

                if (index == i)
                {
                    cB_OLRX_Av[i].Enabled = false;
                    cB_OLRX_Av[i].Checked = false;

                    cB_layer_OLRY_Av[i].Enabled = false;
                    cB_layer_OLRY_Av[i].Checked = false;
                }
                else
                {
                    cB_OLRX_Av[i].Enabled = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                    cB_OLRX_Av[i].Checked = commonVars.getLayerSettings(index).getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, i) == 1 && commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;

                    cB_layer_OLRY_Av[i].Enabled = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                    cB_layer_OLRY_Av[i].Checked = commonVars.getLayerSettings(index).getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, i) == 1 && commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                }
                colIndex++;
                if (colIndex == CentralProperties.maxLayersForMC / 2)
                {
                    colIndex = 0;
                }
            }
        });
        if (!alreadyFrozen)
        {
            globalUIFrozen = false;
        }
    }
}