using System;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    public void setCopyBuffer(int index)
    {
        copy(index);
    }

    private void copy(int index)
    {
        commonVars.setCopy(index);
    }

    private void copyHandler(object sender, EventArgs e)
    {
        copy();
    }

    private void copy()
    {
        int tmp = getSelectedLayerIndex();
        if (tmp is >= 0 and <= CentralProperties.maxLayersForMC - 1) // we're on a layer settings page
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

    private void pasteHandler(object sender, EventArgs e)
    {
        paste();
    }

    private void paste()
    {
        int index = getSelectedLayerIndex();
        if (commonVars.isCopyPrepped() && index is >= 0 and < CentralProperties.maxLayersForMC) // we have valid data and are on a valid page.
        {
            paste(index, updateUI: true);
        }
    }

    private void paste(int index, bool updateUI = false)
    {
        commonVars.paste(index, gdsOnly: false, updateGeoCoreGeometryFromFile: true);
        if (!updateUI)
        {
            return;
        }

        set_ui_from_settings(index);
        do2DLayerUI(index, updateUI: true);
    }

    private void clearHandler(object sender, EventArgs e)
    {
        clear();
    }

    private void clear()
    {
        var result = MessageBox.Show("Are you sure?", "Clear layer", MessageBoxButtons.YesNo, MessageBoxType.Question);
        switch (result)
        {
            case DialogResult.Yes:
            {
                int index = getSelectedLayerIndex();
                if (index is >= 0 and < CentralProperties.maxLayersForMC) // we have valid data and are on a valid page.
                {
                    setLayerSettings(new EntropyLayerSettings(), settingsIndex: index, gdsOnly: false);
                }
                commonVars.getGeoCoreHandler(index).reset();
                break;
            }
        }
    }

    public void setLayerSettings(EntropyLayerSettings entropyLayerSettings, int settingsIndex, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false)
    {
        pSetLayerSettings(entropyLayerSettings, settingsIndex, gdsOnly, updateGeoCoreGeometryFromFile);
    }

    private void pSetLayerSettings(EntropyLayerSettings entropyLayerSettings, int settingsIndex, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false)
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