using color;
using Eto.Drawing;
using Eto.Forms;
using System;

namespace Variance
{
    public partial class MainForm
    {
        void listbox_change(object sender, EventArgs e)
        {
            listbox_change();
        }

        void listbox_change()
        {
            if (layerUIFrozen_exp)
            {
                return;
            }

            int index = experimental_listBox_layers.SelectedIndex;

            if (index == -1)
            {
                experimental_listBox_layers.SelectedIndex = 0;
                index = 0;
            }

            selectedLayer = index;

            set_ui_from_settings(index);
            do2DLayerUI_exp(index);
        }

        void showDrawn_exp(object sender, EventArgs e)
        {
            Int32 settingsIndex = getSelectedLayerIndex();
            if (settingsIndex == -1)
            {
                return;
            }
            showDrawn_exp(settingsIndex);
        }

        void twoDLayerEventHandler_exp(object sender, EventArgs e)
        {
            if (layerUIFrozen_exp)
            {
                return;
            }

            int index = getSelectedLayerIndex();
            if (index == -1)
            {
                return;
            }

            // Do we need to trigger a large UI redraw?
            bool updateUI = false;
            try
            {
                if (comboBox_layerShape_exp == (DropDown)sender)
                {
                    updateUI = true;
                }
            }
            catch (Exception)
            {

            }
            try
            {
                if ((checkBox_layer_overlayXReference_Av_exp == (CheckBox)sender) || (checkBox_layer_overlayYReference_Av_exp == (CheckBox)sender))
                {
                    updateUI = true;
                }
            }
            catch (Exception)
            {

            }

            do2DLayerUI_exp(index, updateUI);
        }

        void suspendLayerUI_exp()
        {
            layerUIFrozen_exp = true;
        }

        void resumeLayerUI_exp()
        {
            layerUIFrozen_exp = false;
        }

        void customRNGContextMenuHandler(object sender, EventArgs e)
        {
            int layer = getSelectedLayerIndex();
            int itemIndex = menu_customRNG.Items.IndexOf((MenuItem)sender);
            try
            {
                switch (rngLabelIndex)
                {
                    case (int)layerLookUpOrder.ICV:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.iCV_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.OCV:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.oCV_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.SCDU:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.sCDU_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.TCDU:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.tCDU_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.XOL:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.xOL_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.YOL:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.yOL_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.HTNV:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.hTipNVar_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.HTPV:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.hTipPVar_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.VTNV:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.vTipNVar_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.VTPV:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.vTipPVar_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.WOB:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.wobble_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.LWR:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.lwr_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                    case (int)layerLookUpOrder.LWR2:
                        commonVars.getLayerSettings(layer).setString(EntropyLayerSettings.properties_s.lwr2_RNG, commonVars.rngCustomMapping[itemIndex].ToString());
                        break;
                }

                if (commonVars.rngCustomMapping[itemIndex] != CommonVars.boxMuller)
                {
                    sourceLabel_RNG.TextColor = Color.FromArgb(MyColor.OrangeRed.toArgb());
                }
                else
                {
                    sourceLabel_RNG.TextColor = Color.FromArgb(MyColor.Black.toArgb());
                }
            }
            catch (Exception)
            {

            }
        }

        void customRNGContextMenu()
        {
            menu_customRNG = new ContextMenu();
            int itemIndex = 0;
            for (int i = 0; i < commonVars.rngCustomMapping.Count; i++)
            {
                menu_customRNG.Items.Add(new ButtonMenuItem { Text = commonVars.rngCustomMapping[i] });
                menu_customRNG.Items[itemIndex].Click += customRNGContextMenuHandler;
                itemIndex++;
            }
            menu_customRNG.Show(sourceLabel_RNG);
        }

        void omitLayerCheckboxChanged(object sender, EventArgs e)
        {
            // Establish which sender raised the event so that we can push the value to the correct layer.
            int index = Array.IndexOf(checkBox_omit_lyr, sender);
            setOmitLayer(index, (bool)((CheckBox)sender).Checked);
        }

        void setOmitLayer(int index, bool status)
        {
            if (status)
            {
                commonVars.getLayerSettings(index).setInt(EntropyLayerSettings.properties_i.omit, 1);
            }
            else
            {
                commonVars.getLayerSettings(index).setInt(EntropyLayerSettings.properties_i.omit, 0);
            }
            uiFollowChanges();
        }

        void bgLayerCheckboxChanged(object sender, EventArgs e)
        {
            if (globalUIFrozen)
            {
                return; // flags are being set, so don't respond to them
            }

            int twoDIndex = getSelectedLayerIndex();

            if (twoDIndex < CentralProperties.maxLayersForMC)
            {
                bgLayerCheckboxChanged(twoDIndex);
            }
        }

        void bgLayerCheckboxChanged(int settingsIndex)
        {
            for (int i = 0; i < checkBox_bg_lyr.Length; i++)
            {
                if (((bool)checkBox_bg_lyr[i].Checked) && checkBox_bg_lyr[i].Enabled)
                {
                    commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, i, 1);
                }
                else
                {
                    commonVars.getLayerSettings(settingsIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, i, 0);
                }
            }
            showBG(settingsIndex);
        }

        void applyLayoutToAll(object sender, EventArgs e)
        {
            // Get our current selected tab. We don't have to validate it as the only caller is on one of the layer tabs.
            int orig = getSelectedLayerIndex();

            setCopyBuffer(orig);
            // Suspend our UI for the duration.
            suspendUIHandlers();

            Int32 index = 0;
            while (index < CentralProperties.maxLayersForMC)
            {
                if (index != orig)
                {
                    commonVars.paste(index, true, false);
                }
                index++;
            }

            commonVars.clearCopy();
            resumeUIHandlers();
        }
    }
}