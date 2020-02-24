using Error;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using System;

namespace Variance
{
    public partial class MainForm
    {
        void layerColorChange(Label id, Color colDialogColor)
        {
            Application.Instance.Invoke(() =>
            {
                if (id == lbl_Layer1Color)
                {
                    lbl_Layer1Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer1_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer2Color)
                {
                    lbl_Layer2Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer2_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer3Color)
                {
                    lbl_Layer3Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer3_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer4Color)
                {
                    lbl_Layer4Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer4_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer5Color)
                {
                    lbl_Layer5Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer5_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer6Color)
                {
                    lbl_Layer6Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer6_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer7Color)
                {
                    lbl_Layer7Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer7_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer8Color)
                {
                    lbl_Layer8Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer8_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer9Color)
                {
                    lbl_Layer9Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer9_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer10Color)
                {
                    lbl_Layer10Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer10_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer11Color)
                {
                    lbl_Layer11Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer11_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer12Color)
                {
                    lbl_Layer12Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer12_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer13Color)
                {
                    lbl_Layer13Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer13_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer14Color)
                {
                    lbl_Layer14Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer14_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer15Color)
                {
                    lbl_Layer15Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer15_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Layer16Color)
                {
                    lbl_Layer16Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.layer16_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Result1Color)
                {
                    lbl_Result1Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.result_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Result2Color)
                {
                    lbl_Result2Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.result2_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Result3Color)
                {
                    lbl_Result3Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.result3_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_Result4Color)
                {
                    lbl_Result4Color.BackgroundColor = colDialogColor;
                    varianceContext.vc.colors.result4_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_minorGridColor)
                {
                    varianceContext.vc.colors.minor_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_majorGridColor)
                {
                    varianceContext.vc.colors.major_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_enabledColor)
                {
                    varianceContext.vc.colors.enabled_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_ss1Color)
                {
                    varianceContext.vc.colors.subshape1_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_ss2Color)
                {
                    varianceContext.vc.colors.subshape2_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_ss3Color)
                {
                    varianceContext.vc.colors.subshape3_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_implantMinColor)
                {
                    varianceContext.vc.colors.implantMin_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_implantMeanColor)
                {
                    varianceContext.vc.colors.implantMean_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_implantMaxColor)
                {
                    varianceContext.vc.colors.implantMax_Color = UIHelper.colorToMyColor(colDialogColor);
                }
                if (id == lbl_implantResistColor)
                {
                    varianceContext.vc.colors.implantResist_Color = UIHelper.colorToMyColor(colDialogColor);
                }

                varianceContext.vc.colors.rebuildLists();
                commonVars.setColors(varianceContext.vc.colors);

                updateUIColors();
            });
        }

        void layerColorChange(object sender, EventArgs e)
        {
            if (colUIFrozen)
            {
                return;
            }

            try
            {
                Label senderLabel = sender as Label;

                if (senderLabel != null)
                {
                    Color sourceColor = Eto.Drawing.Colors.Black;
                    if (senderLabel == lbl_Layer1Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer1_Color);
                    }
                    if (senderLabel == lbl_Layer2Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer2_Color);
                    }
                    if (senderLabel == lbl_Layer3Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer3_Color);
                    }
                    if (senderLabel == lbl_Layer4Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer4_Color);
                    }
                    if (senderLabel == lbl_Layer5Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer5_Color);
                    }
                    if (senderLabel == lbl_Layer6Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer6_Color);
                    }
                    if (senderLabel == lbl_Layer7Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer7_Color);
                    }
                    if (senderLabel == lbl_Layer8Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer8_Color);
                    }
                    if (senderLabel == lbl_Layer9Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer9_Color);
                    }
                    if (senderLabel == lbl_Layer10Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer10_Color);
                    }
                    if (senderLabel == lbl_Layer11Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer11_Color);
                    }
                    if (senderLabel == lbl_Layer12Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer12_Color);
                    }
                    if (senderLabel == lbl_Layer13Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer13_Color);
                    }
                    if (senderLabel == lbl_Layer14Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer14_Color);
                    }
                    if (senderLabel == lbl_Layer15Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer15_Color);
                    }
                    if (senderLabel == lbl_Layer16Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.layer16_Color);
                    }

                    if (senderLabel == lbl_Result1Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.result_Color);
                    }
                    if (senderLabel == lbl_Result2Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.result2_Color);
                    }
                    if (senderLabel == lbl_Result3Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.result3_Color);
                    }
                    if (senderLabel == lbl_Result4Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.result4_Color);
                    }

                    if (senderLabel == lbl_implantMinColor)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.implantMin_Color);
                    }
                    if (senderLabel == lbl_implantMeanColor)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.implantMean_Color);
                    }
                    if (senderLabel == lbl_implantMaxColor)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.implantMax_Color);
                    }
                    if (senderLabel == lbl_implantResistColor)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.implantResist_Color);
                    }

                    if (senderLabel == lbl_ss1Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.subshape1_Color);
                    }
                    if (senderLabel == lbl_ss2Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.subshape2_Color);
                    }
                    if (senderLabel == lbl_ss3Color)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.subshape3_Color);
                    }

                    if (senderLabel == lbl_majorGridColor)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.major_Color);
                    }
                    if (senderLabel == lbl_minorGridColor)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.minor_Color);
                    }

                    if (senderLabel == lbl_enabledColor)
                    {
                        sourceColor = UIHelper.myColorToColor(varianceContext.vc.colors.enabled_Color);
                    }

                    ColorDialog colDialog = new ColorDialog
                    {
                        Color = sourceColor
                    };

                    // Special OS X plumbing. The color picker on that platform is a floating dialog and has no OK/cancel.
                    if (Platform.IsMac)
                    {
                        colDialog.ColorChanged += delegate
                        {
                            layerColorChange(senderLabel, colDialog.Color);
                        };
                    }

                    DialogResult dR = colDialog.ShowDialog(this);

                    if (!Platform.IsMac)
                    {
                        if (dR == DialogResult.Ok)
                        {
                            layerColorChange(senderLabel, colDialog.Color);
                        }
                    }

                    //colDialog.Dispose();
                }
            }
            catch (Exception ec)
            {
                ErrorReporter.showMessage_OK(ec.ToString(), "Error");
            }
        }

        void preferencesChange(object sender, EventArgs e)
        {
            if (utilsUIFrozen)
            {
                return;
            }
            utilsUIFrozen = true;
            varianceContext.vc.geoCoreCDVariation = (bool)checkBox_geoCore_enableCDVariation.Checked;
            commonVars.setGCCDV((bool)checkBox_geoCore_enableCDVariation.Checked);
            varianceContext.vc.layerPreviewDOETile = (bool)checkBox_geoCore_tileLayerPreview.Checked;
            commonVars.setLayerPreviewDOETile((bool)checkBox_geoCore_tileLayerPreview.Checked);
            varianceContext.vc.AA = (bool)checkBox_OGLAA.Checked;
            commonVars.setOpenGLProp(CommonVars.properties_gl.aa, (bool)checkBox_OGLAA.Checked);
            varianceContext.vc.FilledPolygons = (bool)checkBox_OGLFill.Checked;
            commonVars.setOpenGLProp(CommonVars.properties_gl.fill, (bool)checkBox_OGLFill.Checked);
            varianceContext.vc.drawPoints = (bool)checkBox_OGLPoints.Checked;
            commonVars.setOpenGLProp(CommonVars.properties_gl.points, (bool)checkBox_OGLPoints.Checked);
            varianceContext.vc.openGLZoomFactor = Convert.ToInt32(num_zoomSpeed.Value);
            commonVars.setGLInt(CommonVars.gl_i.zoom, (Convert.ToInt32(num_zoomSpeed.Value)));
            varianceContext.vc.FGOpacity = num_fgOpacity.Value;
            commonVars.setOpacity(CommonVars.opacity_gl.fg, num_fgOpacity.Value);
            varianceContext.vc.BGOpacity = num_bgOpacity.Value;
            commonVars.setOpacity(CommonVars.opacity_gl.bg, num_bgOpacity.Value);

            for (int i = 0; i < mcVPSettings.Length; i++)
            {
                mcVPSettings[i].aA(commonVars.getOpenGLProp(CommonVars.properties_gl.aa));
                mcVPSettings[i].setZoomStep(commonVars.getGLInt(CommonVars.gl_i.zoom));
                mcVPSettings[i].drawFilled(commonVars.getOpenGLProp(CommonVars.properties_gl.fill));
                mcVPSettings[i].drawPoints(commonVars.getOpenGLProp(CommonVars.properties_gl.points));
            }

            utilsUIFrozen = false;
            refreshAllPreviewPanels();
            updateViewport();
        }

        void resetColors(object sender, EventArgs e)
        {
            varianceContext.vc.colors.reset();
            commonVars.setColors(varianceContext.vc.colors);
            updateUIColors();
        }

        void miscSettingsChanged(object sender, EventArgs e)
        {
            varianceContext.vc.friendlyNumber = (bool)checkBox_friendlyNumbers.Checked;
            commonVars.setFriendly(varianceContext.vc.friendlyNumber);
        }

    }
}