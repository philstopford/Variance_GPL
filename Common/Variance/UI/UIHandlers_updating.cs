using Eto.Drawing;
using Eto.Forms;
using VeldridEto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Variance
{
    public partial class MainForm
    {
        void updateImplantPreview()
        {
            // Update the preview configuration.
            otkVPSettings_implant.polyList.Clear();
            otkVPSettings_implant.tessPolyList.Clear();
            otkVPSettings_implant.bgPolyList.Clear();
            otkVPSettings_implant.lineList.Clear();
            // Evaluated resist side. We know there's a single polygon in previewShapes and we know the resist evaluation is the first entry in the resistShapes list.
            int limit = entropyControl.getImplantResultPackage().getImplantPreviewResult().getResistShapes()[0].getPoints()[0].Count();
            PointF[] tmpPoints = new PointF[limit];
#if VARIANCETHREADED
            Parallel.For(0, limit, (point) =>
#else
            for (Int32 point = 0; point < tmpPoints.Count(); point++)
#endif
            {
                tmpPoints[point] = new PointF((float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getResistShapes()[0].getPoints()[0][point].X,
                    (float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getResistShapes()[0].getPoints()[0][point].Y);
            }
#if VARIANCETHREADED
            );
#endif
            otkVPSettings_implant.addPolygon(tmpPoints, Color.FromArgb(entropyControl.getImplantResultPackage().getImplantPreviewResult().getResistShapes()[0].getColor().toArgb()), (float)commonVars.getOpacity(CommonVars.opacity_gl.fg), false, 0);

            // Background - second item in the resistShapes, but again a single polygon.
            limit = entropyControl.getImplantResultPackage().getImplantPreviewResult().getResistShapes()[1].getPoints()[0].Count();
            tmpPoints = new PointF[limit];
#if VARIANCETHREADED
            Parallel.For(0, limit, (point) =>
#else
            for (Int32 point = 0; point < tmpPoints.Count(); point++)
#endif
            {
                tmpPoints[point] = new PointF((float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getResistShapes()[1].getPoints()[0][point].X,
                    (float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getResistShapes()[1].getPoints()[0][point].Y);
            }
#if VARIANCETHREADED
            );
#endif
            otkVPSettings_implant.addBGPolygon(tmpPoints, Color.FromArgb(entropyControl.getImplantResultPackage().getImplantPreviewResult().getResistShapes()[1].getColor().toArgb()), (float)commonVars.getOpacity(CommonVars.opacity_gl.bg), 1);

            // Now add our shadowing line. Single polygon in the shadowLine's previewPoints.
            limit = entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.shadow).getPoints()[0].Count();
            tmpPoints = new PointF[limit];
#if VARIANCETHREADED
            Parallel.For(0, limit, (point) =>
#else
            for (Int32 point = 0; point < tmpPoints.Count(); point++)
#endif
            {
                tmpPoints[point] = new PointF((float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.shadow).getPoints()[0][point].X,
                    (float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.shadow).getPoints()[0][point].Y);
            }
#if VARIANCETHREADED
            );
#endif
            otkVPSettings_implant.addLine(tmpPoints, Color.FromArgb(entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.shadow).getColor().toArgb()), (float)commonVars.getOpacity(CommonVars.opacity_gl.fg), 2);

            // Now add our min shadowing line. Single polygon in the shadowLine's previewPoints.
            limit = entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.min).getPoints()[0].Count();
            tmpPoints = new PointF[limit];
#if VARIANCETHREADED
            Parallel.For(0, limit, (point) =>
#else
            for (Int32 point = 0; point < tmpPoints.Count(); point++)
#endif
            {
                tmpPoints[point] = new PointF((float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.min).getPoints()[0][point].X,
                    (float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.min).getPoints()[0][point].Y);
            }
#if VARIANCETHREADED
            );
#endif
            otkVPSettings_implant.addLine(tmpPoints, Color.FromArgb(entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.min).getColor().toArgb()), (float)commonVars.getOpacity(CommonVars.opacity_gl.fg), 3);

            // Now add our max shadowing line. Single polygon in the shadowLine's previewPoints.
            limit = entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.max).getPoints()[0].Count();
            tmpPoints = new PointF[limit];
#if VARIANCETHREADED
            Parallel.For(0, limit, (point) =>
#else
            for (Int32 point = 0; point < tmpPoints.Count(); point++)
#endif
            {
                tmpPoints[point] = new PointF((float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.max).getPoints()[0][point].X,
                    (float)entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.max).getPoints()[0][point].Y);
            }
#if VARIANCETHREADED
            );
#endif
            otkVPSettings_implant.addLine(tmpPoints, Color.FromArgb(entropyControl.getImplantResultPackage().getImplantPreviewResult().getLine(Results_implant.lines.max).getColor().toArgb()), (float)commonVars.getOpacity(CommonVars.opacity_gl.fg), 4);

            Application.Instance.Invoke(() =>
            {
                updateViewport();
            });
        }

        void updateImplantSimUIST(bool doPASearch, SimResultPackage resultPackage, string resultString)
        {
            string[] resultTokens = resultString.Split(new char[] { ',' });
            updateImplantPreview();
            Application.Instance.Invoke(() =>
            {
                vSurface.Invalidate();
                if (resultTokens.Length == 3)
                {
                    // Preview mode
                    textBox_implantShadowNom.Text = resultTokens[0];
                    textBox_implantShadowMin.Text = resultTokens[1];
                    textBox_implantShadowMax.Text = resultTokens[2];
                }
                else
                if (resultTokens.Length == 2)
                {
                    // MC run
                    textBox_implantShadowNom.Text = resultString;
                    textBox_implantShadowMin.Text = "";
                    textBox_implantShadowMax.Text = "";
                }
            });
        }

        void updateImplantSimUIMT()
        {
            commonVars.m_timer.Elapsed += new System.Timers.ElapsedEventHandler(updateImplantPreview);
        }

        void updateSimUIST(bool doPASearch, SimResultPackage resultPackage, string resultString)
        {
            if (commonVars.getReplayMode() == 1)
            {
                resultString += " | replay: " + simReplay.getResult();
            }
            commonVars.getSimPreview().updatePreview(resultPackage.getPreviewResult().getSimShapes(), resultPackage.getPreviewResult().getPreviewShapes(), resultPackage.getPreviewResult().getPoints(), resultString);
            drawSimulationPanelHandler(doPASearch);
        }

        void updateSimUIMT()
        {
            commonVars.m_timer.Elapsed += new System.Timers.ElapsedEventHandler(updatePreview);
        }

        void updateSettingsUIFromSettings()
        {
            Application.Instance.Invoke(() =>
            {
                if (commonVars.getActiveUI(CommonVars.uiActive.settings))
                {
                    suspendSettingsUI();
                }
                if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.results) == 1)
                {
                    checkBox_displayResults.Checked = true;
                }
                else
                {
                    checkBox_displayResults.Checked = false;
                }

                if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.ler) == 1)
                {
                    checkBox_LERMode.Checked = true;
                }
                else
                {
                    checkBox_LERMode.Checked = false;
                }

                if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.shape) == 1)
                {
                    checkBox_displayShapes.Checked = true;
                }
                else
                {
                    checkBox_displayShapes.Checked = false;
                }

                if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.external) == 1)
                {
                    checkBox_external.Checked = true;
                }
                else
                {
                    checkBox_external.Checked = false;
                }

                comboBox_externalTypes.SelectedIndex = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType);

                bool extCrit = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCriteria) == 1;
                checkBox_externalCriteria.Checked = extCrit;

                int extCritCond1 = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCritCond1);
                comboBox_externalCriteria1.SelectedIndex = extCritCond1;

                decimal extCritVal1 = commonVars.getSimulationSettings_nonSim().getDecimal(EntropySettings_nonSim.properties_d.externalCritCond1);
                num_externalCriteria1.Value = Convert.ToDouble(extCritVal1);

                num_externalCriteria1.Enabled = extCritCond1 > 0;

                int extCritCond2 = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCritCond2);
                comboBox_externalCriteria2.SelectedIndex = extCritCond2;

                decimal extCritVal2 = commonVars.getSimulationSettings_nonSim().getDecimal(EntropySettings_nonSim.properties_d.externalCritCond2);
                num_externalCriteria2.Value = Convert.ToDouble(extCritVal2);

                num_externalCriteria2.Enabled = extCritCond2 > 0;

                int extCritCond3 = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCritCond3);
                comboBox_externalCriteria3.SelectedIndex = extCritCond3;

                decimal extCritVal3 = commonVars.getSimulationSettings_nonSim().getDecimal(EntropySettings_nonSim.properties_d.externalCritCond3);
                num_externalCriteria3.Value = Convert.ToDouble(extCritVal3);

                num_externalCriteria3.Enabled = extCritCond3 > 0;

                int extCritCond4 = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalCritCond4);
                comboBox_externalCriteria4.SelectedIndex = extCritCond4;

                decimal extCritVal4 = commonVars.getSimulationSettings_nonSim().getDecimal(EntropySettings_nonSim.properties_d.externalCritCond4);
                num_externalCriteria4.Value = Convert.ToDouble(extCritVal4);

                num_externalCriteria4.Enabled = extCritCond4 > 0;

                comboBox_externalTypes.SelectedIndex = commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType);

                if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1)
                {
                    checkBox_CSV.Checked = true;
                }
                else
                {
                    checkBox_CSV.Checked = false;
                }

                if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.linkCDU) == 1)
                {
                    checkBox_linkCDUVariation.Checked = true;
                }
                else
                {
                    checkBox_linkCDUVariation.Checked = false;
                }

                checkBox_aChord.Enabled = false;
                checkBox_bChord.Enabled = false;
                checkBox_withinMode.Enabled = false;
                checkBox_useShortestEdge.Enabled = false;
                checkBox_perPoly.Enabled = false;

                if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) < 0)
                {
                    commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.oType, 0);
                }

                comboBox_calcModes.SelectedIndex = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType);

                switch (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType))
                {
                    case (Int32)CommonVars.calcModes.area:
                        checkBox_perPoly.Enabled = true;
                        if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.areaCalcModes.perpoly)
                        {
                            checkBox_perPoly.Checked = true;
                        }
                        else
                        {
                            checkBox_perPoly.Checked = false;
                        }

                        break;
                    case (Int32)CommonVars.calcModes.enclosure_spacing_overlap:
                        checkBox_withinMode.Enabled = true;

                        if ((commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacing) || (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacingOld))
                        {
                            checkBox_useShortestEdge.Enabled = true;
                        }

                        if ((commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosure) || (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosureOld))
                        {
                            checkBox_withinMode.Checked = true;
                        }
                        else
                        {
                            checkBox_withinMode.Checked = false;
                        }

                        if ((commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosure) || (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacing))
                        {
                            checkBox_useShortestEdge.Checked = true;
                        }
                        else
                        {
                            checkBox_useShortestEdge.Checked = false;
                        }
                        break;
                    case (Int32)CommonVars.calcModes.chord:
                        checkBox_aChord.Enabled = true;
                        checkBox_bChord.Enabled = true;
                        if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) != (int)CommonVars.chordCalcElements.b)
                        {
                            checkBox_aChord.Checked = true;
                        }
                        else
                        {
                            checkBox_aChord.Checked = false;
                        }
                        if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) >= (int)CommonVars.chordCalcElements.b)
                        {
                            checkBox_bChord.Checked = true;
                        }
                        else
                        {
                            checkBox_bChord.Checked = false;
                        }
                        break;
                    case (Int32)CommonVars.calcModes.angle:
                        break;
                }

                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    comboBox_geoEqtn_Op[i].SelectedIndex = commonVars.getSimulationSettings().getOperatorValue(EntropySettings.properties_o.layer, i);
                }

                for (int i = 0; i < comboBox_geoEqtn_Op_2Layer.Length; i++)
                {
                    comboBox_geoEqtn_Op_2Layer[i].SelectedIndex = commonVars.getSimulationSettings().getOperatorValue(EntropySettings.properties_o.twoLayer, i);
                }

                for (int i = 0; i < comboBox_geoEqtn_Op_4Layer.Length; i++)
                {
                    comboBox_geoEqtn_Op_4Layer[i].SelectedIndex = commonVars.getSimulationSettings().getOperatorValue(EntropySettings.properties_o.fourLayer, i);
                }

                for (int i = 0; i < comboBox_geoEqtn_Op_8Layer.Length; i++)
                {
                    comboBox_geoEqtn_Op_8Layer[i].SelectedIndex = commonVars.getSimulationSettings().getOperatorValue(EntropySettings.properties_o.eightLayer, i);
                }

                if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.rngType) < 0)
                {
                    commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.rngType, commonVars.getSimulationSettings().getDefaultValue(EntropySettings.properties_i.rngType));
                }

                comboBox_RNG.SelectedIndex = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.rngType);

                num_cornerSegments.Value = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.cSeg);
                num_ssNumOfCases.Value = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.nCases);
                num_ssPrecision.Value = commonVars.getSimulationSettings().getResolution();

                if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.optC) == 1)
                {
                    checkBox_limitCornerPoints.Checked = true;
                }
                else
                {
                    checkBox_limitCornerPoints.Checked = false;
                }

                if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.greedy) == 1)
                {
                    checkBox_greedyMultiCPU.Checked = true;
                }
                else
                {
                    checkBox_greedyMultiCPU.Checked = false;
                }

                resumeSettingsUI();
            });
        }

        void updateDOESettingsUIFromSettings()
        {
            Application.Instance.Invoke(() =>
            {
                if (commonVars.getActiveUI(CommonVars.uiActive.doe))
                {
                    suspendDOESettingsUI();
                }

                num_DOERows.Value = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.rows);
                num_DOECols.Value = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.cols);
                try
                {
                    num_DOEColPitch.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colPitch);
                    num_DOERowPitch.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowPitch);
                }
                catch (Exception)
                {
                }

                radioButton_allTiles.Checked = true;

                if (commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.sTile) == 1)
                {
                    radio_useSpecificTile.Checked = true;
                }

                if (commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.uTileList) == 1)
                {
                    radio_useSpecificTiles.Checked = true;
                }

                try
                {
                    num_DOESCCol.Value = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.sTileCol);
                    num_DOESCRow.Value = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.sTileRow);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_rowOffset.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
                    num_colOffset.Value = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
                }
                catch (Exception)
                {
                }

                try
                {
                    textBox_useSpecificTiles.Text = commonVars.getSimulationSettings().tileListToString();
                }
                catch (Exception)
                {
                }

                if (commonVars.getSimulationSettings().getDOESettings().getBool(DOESettings.properties_b.iDRM))
                {
                    radio_useCSViDRM.Checked = true;
                }

                if (commonVars.getSimulationSettings().getDOESettings().getBool(DOESettings.properties_b.quilt))
                {
                    radio_useCSVQuilt.Checked = true;
                }

                doeSettingsChanged();
                resumeDOESettingsUI();
            });
        }

        void updateImplantUIFromSettings()
        {
            Application.Instance.Invoke(() =>
            {
                if (commonVars.getActiveUI(CommonVars.uiActive.implant))
                {
                    suspendImplantUI();
                }

                try
                {
                    num_implantResistCD.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.w);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantResistCDVar.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.wV);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantResistHeight.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.h);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantResistHeightVar.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.hV);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantResistTopCRR.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.cRR);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantResistTopCRRVar.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.cV);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantTiltAngle.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.tilt);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantTiltAngleVar.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.tiltV);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantTwistAngle.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.twist);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantTwistAngleVar.Value = commonVars.getImplantSettings().getDouble(EntropyImplantSettings.properties_d.twistV);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantNumOfCases.Value = commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.nCases);
                }
                catch (Exception)
                {
                }

                try
                {
                    num_implantCornerSegments.Value = commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.cSeg);
                }
                catch (Exception)
                {
                }

                try
                {
                    checkBox_CSV_implant.Checked = (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.csv) == 1);
                }
                catch (Exception)
                {
                }

                try
                {
                    checkBox_external_implant.Checked = ((commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.external) == 1) && (commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType) == (Int32)CommonVars.external_Type.svg));
                }
                catch (Exception)
                {
                }

                try
                {
                    comboBox_externalTypes_implant.SelectedIndex = commonVars.getImplantSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.externalType);
                }
                catch (Exception)
                {
                }

                try
                {
                    comboBox_implantRNG.SelectedIndex = commonVars.getImplantSimulationSettings().getValue(EntropySettings.properties_i.rngType);
                }
                catch (Exception)
                {
                }

                doImplantShadowing();
                resumeImplantUI();
            });
        }

        void updateUtilsValues()
        {
            Application.Instance.Invoke(() =>
            {
                utilsUIFrozen = true;
                num_port.Value = Convert.ToInt32(varianceContext.vc.port);
                text_server.Text = varianceContext.vc.host;
                text_emailAddress.Text = varianceContext.vc.emailAddress;
                try
                {
                    text_emailPwd.Text = varianceContext.vc.aes.DecryptString(varianceContext.vc.emailPwd);
                }
                catch (Exception)
                {
                    text_emailPwd.Text = "";
                    commonVars.getNonSimulationSettings().emailPwd = varianceContext.vc.emailPwd = varianceContext.vc.aes.EncryptToString("");
                }
                checkBox_SSL.Checked = varianceContext.vc.ssl;
                checkBox_perJob.Checked = varianceContext.vc.perJob;
                checkBox_EmailCompletion.Checked = varianceContext.vc.completion;
                checkBox_friendlyNumbers.Checked = varianceContext.vc.friendlyNumber;
                num_zoomSpeed.Value = varianceContext.vc.openGLZoomFactor;
                num_bgOpacity.Value = varianceContext.vc.BGOpacity;
                num_fgOpacity.Value = varianceContext.vc.FGOpacity;
                checkBox_OGLAA.Checked = varianceContext.vc.AA;
                checkBox_OGLFill.Checked = varianceContext.vc.FilledPolygons;
                checkBox_OGLPoints.Checked = varianceContext.vc.drawPoints;
                checkBox_geoCore_tileLayerPreview.Checked = varianceContext.vc.layerPreviewDOETile;
                checkBox_geoCore_enableCDVariation.Checked = varianceContext.vc.geoCoreCDVariation;
                utilsUIFrozen = false;
            });
        }

        void updateUIColors()
        {
            Application.Instance.Invoke(() =>
            {
                colUIFrozen = true;
                lbl_Layer1Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer1_Color.toArgb());
                lbl_Layer2Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer2_Color.toArgb());
                lbl_Layer3Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer3_Color.toArgb());
                lbl_Layer4Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer4_Color.toArgb());
                lbl_Layer5Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer5_Color.toArgb());
                lbl_Layer6Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer6_Color.toArgb());
                lbl_Layer7Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer7_Color.toArgb());
                lbl_Layer8Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer8_Color.toArgb());
                lbl_Layer9Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer9_Color.toArgb());
                lbl_Layer10Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer10_Color.toArgb());
                lbl_Layer11Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer11_Color.toArgb());
                lbl_Layer12Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer12_Color.toArgb());
                lbl_Layer13Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer13_Color.toArgb());
                lbl_Layer14Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer14_Color.toArgb());
                lbl_Layer15Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer15_Color.toArgb());
                lbl_Layer16Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.layer16_Color.toArgb());
                lbl_Result1Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.result_Color.toArgb());
                lbl_Result2Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.result2_Color.toArgb());
                lbl_Result3Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.result3_Color.toArgb());
                lbl_Result4Color.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.result4_Color.toArgb());
                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    label_geoEqtn_Op[i].TextColor = Color.FromArgb(varianceContext.vc.colors.simPreviewColors[i].toArgb());
                    checkBox_bg_lyr[i].TextColor = Color.FromArgb(varianceContext.vc.colors.simPreviewColors[i].toArgb());
                    checkBox_omit_lyr[i].TextColor = Color.FromArgb(varianceContext.vc.colors.simPreviewColors[i].toArgb());
                }

                Color ss1Color = Color.FromArgb(varianceContext.vc.colors.subshape1_Color.R,
                                                varianceContext.vc.colors.subshape1_Color.G,
                                                varianceContext.vc.colors.subshape1_Color.B);

                comboBox_layerTipLocations_exp.TextColor = ss1Color;

                num_layer_subshape_hl_exp.TextColor = ss1Color;
                num_layer_subshape_vl_exp.TextColor = ss1Color;
                num_layer_subshape_ho_exp.TextColor = ss1Color;
                num_layer_subshape_vo_exp.TextColor = ss1Color;

                Color ss2Color = Color.FromArgb(varianceContext.vc.colors.subshape2_Color.R,
                                                varianceContext.vc.colors.subshape2_Color.G,
                                                varianceContext.vc.colors.subshape2_Color.B);

                comboBox_layerTipLocations2_exp.TextColor = ss2Color;

                num_layer_subshape2_hl_exp.TextColor = ss2Color;
                num_layer_subshape2_vl_exp.TextColor = ss2Color;
                num_layer_subshape2_ho_exp.TextColor = ss2Color;
                num_layer_subshape2_vo_exp.TextColor = ss2Color;

                Color ss3Color = Color.FromArgb(varianceContext.vc.colors.subshape3_Color.R,
                                                varianceContext.vc.colors.subshape3_Color.G,
                                                varianceContext.vc.colors.subshape3_Color.B);

                comboBox_layerTipLocations3_exp.TextColor = ss3Color;

                num_layer_subshape3_hl_exp.TextColor = ss3Color;
                num_layer_subshape3_vl_exp.TextColor = ss3Color;
                num_layer_subshape3_ho_exp.TextColor = ss3Color;
                num_layer_subshape3_vo_exp.TextColor = ss3Color;

                lbl_ss1Color.BackgroundColor = ss1Color;
                lbl_ss2Color.BackgroundColor = ss2Color;
                lbl_ss3Color.BackgroundColor = ss3Color;

                lbl_enabledColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.enabled_Color.toArgb());
                lbl_axisColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.axis_Color.toArgb());
                lbl_majorGridColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.major_Color.toArgb());
                lbl_minorGridColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.minor_Color.toArgb());
                lbl_vpbgColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.background_Color.toArgb());
                lbl_implantMinColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.implantMin_Color.toArgb());
                lbl_implantMeanColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.implantMean_Color.toArgb());
                lbl_implantMaxColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.implantMax_Color.toArgb());
                lbl_implantResistColor.BackgroundColor = Color.FromArgb(varianceContext.vc.colors.implantResist_Color.toArgb());
                colUIFrozen = false;
            });

            doImplantShadowing();
            doeSettingsChanged();
            drawSimulationPanelHandler(false);
            refreshAllPreviewPanels();

            updateStatusLine("");
        }

        // Update status line with message. Blanks will reset back.
        void updateStatusLine(string message)
        {
            Application.Instance.AsyncInvoke(() =>
            {
                if (message == "")
                {
                    message = "Version " + commonVars.version;
                }
                statusReadout.Text = message;
            }
            );
        }

        void updateProgressBar()
        {
            if (Thread.CurrentThread == commonVars.mainThreadIndex)
            {
                if (statusProgressBar.Value < statusProgressBar.MaxValue)
                {
                    statusProgressBar.Value++;
                }
            }
            else
            {
                Application.Instance.AsyncInvoke(() =>
                {
                    updateProgressBar();
                });
            }
        }

        void updateProgressBar(Int32 value)
        {
            if (Thread.CurrentThread == commonVars.mainThreadIndex)
            {
                if (value > statusProgressBar.MaxValue)
                {
                    value = statusProgressBar.MaxValue;
                }
                statusProgressBar.Value = value;
            }
            else
            {
                Application.Instance.AsyncInvoke(() =>
                {
                    updateProgressBar(value);
                });
            }
        }

        void updateProgressBar(double val)
        {
            Application.Instance.Invoke(() =>
            {
                statusProgressBar.Indeterminate = false;
                if (statusProgressBar.MaxValue < 100)
                {
                    statusProgressBar.MaxValue = 100;
                }
                if (val > 1)
                {
                    val = 1;
                }
                if (val < 0)
                {
                    val = 0;
                }
                statusProgressBar.Value = (int)(val * statusProgressBar.MaxValue);
            });
        }

        void updatePreview(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Monitor.TryEnter(commonVars.drawingLock))
            {
                try
                {
                    if ((entropyControl.sw_Preview.Elapsed.TotalMilliseconds - entropyControl.timeOfLastPreviewUpdate) > commonVars.m_timer.Interval)
                    {
                        try
                        {
                            Application.Instance.Invoke(() =>
                            {
                                bool doPASearch = getSubTabSelectedIndex() == (int)CommonVars.twoDTabNames.paSearch;
                                previewUpdate(doPASearch);
                            });
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(commonVars.drawingLock);
                }
            }
        }

        void previewUpdate(bool doPASearch)
        {
            if ((entropyControl.sw_Preview.Elapsed.TotalMilliseconds - entropyControl.timeOfLastPreviewUpdate) < commonVars.m_timer.Interval)
            {
                return;
            }

            try
            {
                if (entropyControl.sw.IsRunning)
                {
                    entropyControl.swTime += entropyControl.sw.Elapsed.TotalSeconds;
                }
                entropyControl.sw.Stop();
                statusProgressBar.Value = entropyControl.currentProgress;
                entropyControl.timeOfFlight(entropyControl.swTime, doPASearch);
                if (Monitor.TryEnter(varianceContext.vc.previewLock))
                {
                    try
                    {
                        // Update the preview configuration.
                        mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].bgPolyList.Clear();
                        mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].tessPolyList.Clear();
                        mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].polyList.Clear();
                        mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].lineList.Clear();
                        if ((commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.results) == 1) || (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.shape) == 1))
                        {
                            commonVars.getSimPreview().updatePreview(entropyControl.getResultPackage());
                        }
                        else
                        {
                            commonVars.getSimPreview().updatePreview(entropyControl.getResultPackage().getMeanAndStdDev());
                        }
                    }
                    finally
                    {
                        Monitor.Exit(varianceContext.vc.previewLock);
                    }
                }
                drawSimulationPanelHandler(doPASearch);
            }
            catch (Exception)
            {
            }
            try
            {
                entropyControl.sw.Reset();
                entropyControl.sw.Start();
                entropyControl.timeOfLastPreviewUpdate = entropyControl.sw_Preview.Elapsed.Milliseconds;
            }
            catch (Exception)
            {
            }
        }

        void drawSimulationPanelHandler(bool doPASearch)
        {
            try
            {
                Int32 totalPoints = 0;
                if (!commonVars.isSimRunning())
                {
                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].minorGridColor = Color.FromArgb(commonVars.getColors().minor_Color.toArgb());
                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].majorGridColor = Color.FromArgb(commonVars.getColors().major_Color.toArgb());
                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].axisColor = Color.FromArgb(commonVars.getColors().axis_Color.toArgb());
                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].backColor = Color.FromArgb(commonVars.getColors().background_Color.toArgb());
                }

                mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].bgPolyList.Clear();
                mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].polyList.Clear();
                mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].tessPolyList.Clear();
                mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].lineList.Clear();
                // Draw our base shapes if user requested
                if ((commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.shape) == 1) || (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.results) == 1))
                {
                    if ((commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.shape) == 1) && (commonVars.getSimPreview().getPreviewShapes().Count() != 0))
                    {
                        for (Int32 layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
                        {
                            for (Int32 poly = 0; poly < commonVars.getSimPreview().getLayerPreviewShapes(layer).Count; poly++)
                            {
                                if (commonVars.getSimPreview().getLayerPreviewShapePoly(layer, poly).Count() >= 2)// Only draw if we have valid data to draw.
                                {
                                    int length = commonVars.getSimPreview().getLayerPreviewShapePoly(layer, poly).Count();
                                    PointF[] tmpPoints = new PointF[length];
#if VARIANCETHREADED
                                    Parallel.For(0, length, (point) =>
#else
                                    for (Int32 point = 0; point < length; point++)
#endif
                                    {
                                        tmpPoints[point] = new PointF((float)commonVars.getSimPreview().getLayerPreviewShapePoly(layer, poly)[point].X, (float)commonVars.getSimPreview().getLayerPreviewShapePoly(layer, poly)[point].Y);
                                    }
#if VARIANCETHREADED
                                    );
#endif
                                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].addPolygon(
                                        poly: tmpPoints,
                                        polyColor: Color.FromArgb(varianceContext.vc.colors.simPreviewColors[layer].toArgb()),
                                        alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.bg),
                                        drawn: false, 
                                        layerIndex: layer
                                    );
                                }
                            }
                        }
                    }

                    if (!commonVars.isSimRunning())
                    {
                        Application.Instance.Invoke(() =>
                        {
                            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                            {
                                label_geoEqtn_Op[i].TextColor = Color.FromArgb(varianceContext.vc.colors.simPreviewColors[i].toArgb());
                            }
                        });
                    }

                    if (commonVars.getSimulationSettings_nonSim().getInt(EntropySettings_nonSim.properties_i.results) == 1)
                    {
                        // Draw any boolean points :
                        for (Int32 poly = 0; poly < commonVars.getSimPreview().getPoints().Count(); poly++)
                        {
                            // Only draw if we can make a polygon.
                            if (commonVars.getSimPreview().getPoints(poly).Count() >= 2)
                            {
                                Int32 colorIndex = poly % varianceContext.vc.colors.resultColors.Count(); // map our result into the available colors.
                                totalPoints += commonVars.getSimPreview().getPoints(poly).Count();
                                if ((commonVars.getSimPreview().getPoints(poly).Count() == 2) || (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.chord) || (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.angle) || (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (int)CommonVars.calcModes.enclosure_spacing_overlap))
                                {
                                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].addLine(
                                        line: UIHelper.myPointFArrayToPointFArray(commonVars.getSimPreview().getPoints(poly)),
                                        lineColor: Color.FromArgb(varianceContext.vc.colors.resultColors[colorIndex].toArgb()),
                                        alpha: 1.0f,
                                        layerIndex: CentralProperties.maxLayersForMC + colorIndex
                                    );
                                }
                                else
                                {
                                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.settings].addPolygon(
                                        poly: UIHelper.myPointFArrayToPointFArray(commonVars.getSimPreview().getPoints(poly)),
                                        polyColor: Color.FromArgb(varianceContext.vc.colors.resultColors[colorIndex].toArgb()),
                                        alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.fg),
                                        drawn: false,
                                        layerIndex: CentralProperties.maxLayersForMC + colorIndex
                                    );
                                }
                            }
                        }
                    }
                }
                Application.Instance.Invoke(() =>
                {
                    text_ssTotalPoints.Text = totalPoints.ToString();
                    if (doPASearch)
                    {
                        text_paSearch.Text = commonVars.getSimPreview().getResult();
                    }
                    else
                    {
                        text_testArea.Text = commonVars.getSimPreview().getResult();
                    }
                });

                if (entropyControl.simJustDone)
                {
                    updateStatusLine(entropyControl.lastSimResultsOverview);
                }
                else
                {
                    if (!entropyControl.multiCaseSim)
                    {
                        postSimStatusLine();
                    }
                }
            }
            catch (Exception)
            {
                // Uncritical failure.
            }
            Application.Instance.Invoke(() =>
            {
                updateViewport();
            });
        }

        void updateImplantPreview(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Monitor.TryEnter(commonVars.implantDrawingLock))
            {
                try
                {
                    if ((entropyControl.sw_Preview.Elapsed.TotalMilliseconds - entropyControl.timeOfLastPreviewUpdate) > commonVars.m_timer.Interval)
                    {
                        try
                        {
                            if (System.Threading.Thread.CurrentThread == commonVars.mainThreadIndex)
                            {
                                // also sets commonVars.drawing to 'false'
                                implantPreviewUpdate();
                            }
                            else
                            {
                                Application.Instance.Invoke(() =>
                                {
                                    implantPreviewUpdate();
                                });
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(commonVars.implantDrawingLock);
                }
            }
        }

        void implantPreviewUpdate()
        {
            if ((entropyControl.sw_Preview.Elapsed.TotalMilliseconds - entropyControl.timeOfLastPreviewUpdate) < commonVars.m_timer.Interval)
            {
                return;
            }
            try
            {
                if (entropyControl.sw.IsRunning)
                {
                    entropyControl.swTime += entropyControl.sw.Elapsed.TotalSeconds;
                }
                entropyControl.sw.Stop();
                statusProgressBar.Value = entropyControl.currentProgress;
                entropyControl.timeOfFlight_implant(entropyControl.swTime);


                if (Monitor.TryEnter(varianceContext.vc.implantPreviewLock))
                {
                    try
                    {
                        textBox_implantShadowNom.Text = entropyControl.getImplantResultPackage().getMeanAndStdDev();
                        textBox_implantShadowMin.Text = "";
                        textBox_implantShadowMax.Text = "";
                        updateImplantPreview();
                    }
                    finally
                    {
                        Monitor.Exit(varianceContext.vc.implantPreviewLock);
                    }
                }
                Application.Instance.Invoke(() =>
                {
                    updateViewport();
                });
            }
            catch (Exception)
            {
            }
            try
            {
                entropyControl.sw.Reset();
                entropyControl.sw.Start();
                entropyControl.timeOfLastPreviewUpdate = entropyControl.sw_Preview.Elapsed.Milliseconds;
            }
            catch (Exception)
            {
            }
        }

        void showBG(int settingsIndex)
        {
            viewPort.ovpSettings.bgPolyList.Clear();

            bool useVPGeometry = false;

            // Now we need to deal with the background setting.
            // This is only a viewport construct, to avoid impacting the simulation system.
            for (Int32 bgLayer = 0; bgLayer < CentralProperties.maxLayersForMC; bgLayer++)
            {
                // User requested a background layer for this case.
                if (commonVars.getLayerSettings(settingsIndex).getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, bgLayer) == 1)
                {
                    if ((bgLayer != settingsIndex) && (commonVars.getLayerSettings(bgLayer).getInt(EntropyLayerSettings.properties_i.enabled) == 1)) // Don't process our current layer - that would be silly. Also don't process disabled layers.
                    {
                        Color tempColor = Color.FromArgb(varianceContext.vc.colors.simPreviewColors[bgLayer].toArgb());

                        if (useVPGeometry)
                        {
                            // Make a list to contain our background polys.
                            List<ovp_Poly> bgPolyList = new List<ovp_Poly>();

                            // We filter out any drawn or background polys in the referenced layer's viewport to retrieve legitimate shapes.
                            for (int poly = 0; poly < mcVPSettings[bgLayer].polyList.Count; poly++)
                            {
                                if (!mcVPSettings[bgLayer].drawnPoly[poly])
                                {
                                    bgPolyList.Add(mcVPSettings[bgLayer].polyList[poly]);
                                }
                            }

                            // Now we need to go through the polygons in the bgLayer.
                            for (int poly = 0; poly < bgPolyList.Count; poly++)
                            {
                                // Push the polygons to the right viewport, with our layer color to show the origin.
                                viewPort.ovpSettings.addBGPolygon(
                                    poly: bgPolyList[poly].poly, 
                                    polyColor: tempColor,
                                    alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.bg),
                                    layerIndex: bgLayer
                                );
                            }
                        }
                        else
                        {
                            List<PreviewShape> tmp = generate_shapes(bgLayer);
                            for (int shape = 0; shape < tmp.Count; shape++)
                            {
                                for (int poly = 0; poly < tmp[shape].getPoints().Count; poly++)
                                {
                                    if (!tmp[shape].getDrawnPoly(poly))
                                    {
                                        viewPort.ovpSettings.addBGPolygon(
                                            poly: UIHelper.myPointFArrayToPointFArray(tmp[shape].getPoints(poly)),
                                            polyColor: tempColor,
                                            alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.bg),
                                            layerIndex: bgLayer
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
            }
            updateViewport();
        }

        async void generatePreviewPanelContent(int settingsIndex)
        {
            if (openGLErrorReported)
            {
                return;
            }

            if ((settingsIndex < 0) || (settingsIndex > CentralProperties.maxLayersForMC))
            {
                return;
            }

            Application.Instance.Invoke(() =>
            {
                updateStatusLine("Reticulating splines.");
            });
            startIndeterminateProgress();

            List<PreviewShape> previewShapes = new List<PreviewShape>();

            try
            {
                previewShapes = await updateTask(settingsIndex);
            }
            catch (Exception)
            {
                // Handle any task cancelled exception without crashing the tool. The cancellation may occur due to close of the tool whilst evaluation is underway.
            }

            // Brute force setting, to ensure we're aligned with user preferences that might have changed.
            mcVPSettings[settingsIndex].minorGridColor = Color.FromArgb(commonVars.getColors().minor_Color.toArgb());
            mcVPSettings[settingsIndex].majorGridColor = Color.FromArgb(commonVars.getColors().major_Color.toArgb());
            mcVPSettings[settingsIndex].axisColor = Color.FromArgb(commonVars.getColors().axis_Color.toArgb());
            mcVPSettings[settingsIndex].backColor = Color.FromArgb(commonVars.getColors().background_Color.toArgb());
            mcVPSettings[settingsIndex].reset(false);

            // Iterate through our shapes
            // Note that we also load the viewport poly drawn/enabled state. This allows for background poly filtering later.
            for (Int32 i = 0; i < previewShapes.Count; i++)
            {
                for (Int32 poly = 0; poly < previewShapes[i].getPoints().Count; poly++)
                {
                    mcVPSettings[settingsIndex].addPolygon(
                        poly: UIHelper.myPointFArrayToPointFArray(previewShapes[i].getPoints()[poly]),
                        polyColor: Color.FromArgb(previewShapes[i].getColor().toArgb()),
                        alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.fg),
                        drawn: previewShapes[i].getDrawnPoly(poly),
                        layerIndex: settingsIndex
                    );
                }
            }

            updateViewport();
            stopIndeterminateProgress();
            Application.Instance.Invoke(() =>
            {
                updateStatusLine("");
            });
        }

        Task<List<PreviewShape>> updateTask(int settingsIndex)
        {
            return Task.Run(() => generate_shapes(settingsIndex));
        }

        void updateViewport()
        {
            Application.Instance.Invoke(() =>
            {
                try
                {
                    createVPContextMenu();
                    viewPort.updateViewport();
                    vSurface.Invalidate();
                }
                catch (Exception)
                {
                    openGLErrorReported = true;
                }
            });
        }

        void refreshAllPreviewPanels()
        {
            if (openGLErrorReported)
            {
                return;
            }
            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                generatePreviewPanelContent(layer);
            }
            updateViewport();
        }

        void drawPreviewPanelHandler()
        {
            if (getMainSelectedIndex() == (int)CommonVars.upperTabNames.twoD)
            {
                int index = getSelectedLayerIndex();
                // Check that we're in the 2D section and on a tab with a preview panel present for the mcLayers
                if ((index >= 0) && (index < CentralProperties.maxLayersForMC))
                {
                    generatePreviewPanelContent(index);
                }
            }
        }

        // Responsible for generating our preview shapes per the settings.
        // Output used by the previewPanelHandler in non-GDS mode.
        // All offsets need to be applied here - previewPanel takes what it is given and draws that.
        List<PreviewShape> generate_shapes(Int32 settingsIndex)
        {
            List<PreviewShape> previewShapes = new List<PreviewShape>();

            // User has a shape chosen so we can draw a preview
            if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) != (Int32)CommonVars.shapeNames.none)
            {
                // Drawn curves from combined shapes
                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                {
                    // Draw curves
                    PreviewShape pShape = new PreviewShape(commonVars, settingsIndex, subShapeIndex: 0, commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.enabled), doPASearch: false, previewMode: true, currentRow: 0, currentCol: 0);
                    pShape.setColor(varianceContext.vc.colors.enabled_Color);
                    previewShapes.Add(pShape);
                }
                // Get the drawn polygons.
                if (
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.rect) ||
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE) ||
                    (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.BOOLEAN)
                   )
                {
                    PreviewShape pShape1 = new PreviewShape(commonVars, settingsIndex, subShapeIndex: 0, mode: 0, doPASearch: false, previewMode: true, currentRow: 0, currentCol: 0);
                    pShape1.setColor(varianceContext.vc.colors.subshape1_Color);
                    previewShapes.Add(pShape1);
                }
                else
                {
                    PreviewShape pShape1 = new PreviewShape(commonVars, settingsIndex, subShapeIndex: 0, mode: 0, doPASearch: false, previewMode: true, currentRow: 0, currentCol: 0);
                    pShape1.setColor(varianceContext.vc.colors.subshape1_Color);
                    PreviewShape pShape2 = new PreviewShape(commonVars, settingsIndex, subShapeIndex: 1, mode: 0, doPASearch: false, previewMode: true, currentRow: 0, currentCol: 0);
                    pShape2.setColor(varianceContext.vc.colors.subshape2_Color);
                    PreviewShape pShape3 = new PreviewShape(commonVars, settingsIndex, subShapeIndex: 2, mode: 0, doPASearch: false, previewMode: true, currentRow: 0, currentCol: 0);
                    pShape3.setColor(varianceContext.vc.colors.subshape3_Color);
                    previewShapes.Add(pShape1);
                    previewShapes.Add(pShape2);
                    previewShapes.Add(pShape3);
                }
            }
            else
            {
                // No preview - no shape chosen.
            }
            return previewShapes;
        }
    }
}