using System;
using Eto.Forms;

namespace Variance
{
    public partial class MainForm
    {
        void hOverlay_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.XOL;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void vOverlay_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.YOL;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void vTipPVar_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.VTPV;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void vTipNVar_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.VTNV;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void hTipPVar_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.HTPV;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void hTipNVar_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.HTNV;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void ICV_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.ICV;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void OCV_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.OCV;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void sCDU_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.SCDU;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void tCDU_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.TCDU;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void wobble_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.WOB;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void lwr_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.LWR;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }

        void lwr2_RNG(object sender, EventArgs e)
        {
            rngLabelIndex = (int)layerLookUpOrder.LWR2;
            sourceLabel_RNG = (Label)sender;
            customRNGContextMenu();
        }
    }
}
