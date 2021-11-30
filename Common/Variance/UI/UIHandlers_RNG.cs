using System;
using Eto.Forms;

namespace Variance;

public partial class MainForm
{
    private void hOverlay_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.XOL;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void vOverlay_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.YOL;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void vTipPVar_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.VTPV;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void vTipNVar_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.VTNV;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void hTipPVar_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.HTPV;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void hTipNVar_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.HTNV;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void ICV_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.ICV;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void OCV_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.OCV;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void sCDU_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.SCDU;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void tCDU_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.TCDU;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void wobble_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.WOB;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void lwr_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.LWR;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }

    private void lwr2_RNG(object sender, EventArgs e)
    {
        rngLabelIndex = (int)layerLookUpOrder.LWR2;
        sourceLabel_RNG = (Label)sender;
        customRNGContextMenu();
    }
}