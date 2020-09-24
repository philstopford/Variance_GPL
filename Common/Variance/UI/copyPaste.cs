using Eto.Forms;
using System;

namespace Variance
{
    public partial class MainForm
    {
        public void setCopyBuffer(int index)
        {
            copy(index);
        }

        void copy(int index)
        {
            commonVars.setCopy(index);
        }

        void copyHandler(object sender, EventArgs e)
        {
            copy();
        }

        void copy()
        {
            Int32 tmp = getSelectedLayerIndex();
            if ((tmp >= 0) && (tmp <= (CentralProperties.maxLayersForMC - 1))) // we're on a layer settings page
            {
                copy(tmp);
            }
            else
            {
                commonVars.clearCopy(); // cannot copy from other pages.
            }
            pasteLayer.Enabled = commonVars.isCopyPrepped();
            updateLBContextMenu();
        }

        void pasteHandler(object sender, EventArgs e)
        {
            paste();
        }

        void paste()
        {
            Int32 index = getSelectedLayerIndex();
            if ((commonVars.isCopyPrepped()) && ((index >= 0) && (index < CentralProperties.maxLayersForMC))) // we have valid data and are on a valid page.
            {
                paste(index, updateUI: true);
            }
        }

        void paste(int index, bool updateUI = false)
        {
            commonVars.paste(index, gdsOnly: false, updateGeoCoreGeometryFromFile: true);
            if (updateUI)
            {
                set_ui_from_settings(index);
                do2DLayerUI_exp(index, updateUI: true);
            }
        }

        void clearHandler(object sender, EventArgs e)
        {
            clear();
        }

        void clear()
        {
            var result = MessageBox.Show("Are you sure?", "Clear layer", MessageBoxButtons.YesNo, MessageBoxType.Question);
            if (result == DialogResult.Yes)
            {
                Int32 index = getSelectedLayerIndex();
                if ((index >= 0) && (index < CentralProperties.maxLayersForMC)) // we have valid data and are on a valid page.
                {
                    setLayerSettings(new EntropyLayerSettings(), settingsIndex: index, gdsOnly: false, resumeUI: true);
                }
                commonVars.getGeoCoreHandler(index).reset();
            }
        }

        public void setLayerSettings(EntropyLayerSettings entropyLayerSettings, int settingsIndex, bool gdsOnly, bool resumeUI, bool updateGeoCoreGeometryFromFile = false)
        {
            pSetLayerSettings(entropyLayerSettings, settingsIndex, gdsOnly, resumeUI, updateGeoCoreGeometryFromFile);
        }

        void pSetLayerSettings(EntropyLayerSettings entropyLayerSettings, int settingsIndex, bool gdsOnly, bool resumeUI, bool updateGeoCoreGeometryFromFile = false)
        {
            commonVars.setLayerSettings(entropyLayerSettings, settingsIndex, gdsOnly, updateGeoCoreGeometryFromFile);
            // Ensure the simulation settings labels are consistent.
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                label_geoEqtn_Op[i].Text = commonVars.getLayerSettings(i).getString(EntropyLayerSettings.properties_s.name);
            }

            updateLayerNames();
        }
    }
}
