using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using System.Threading.Tasks;

namespace Variance
{
    public partial class MainForm
    {
        void suspendDOESettingsUI()
        {
            DOEUIFrozen = true;
            commonVars.setActiveUI(CommonVars.uiActive.doe, false);
        }

        void resumeDOESettingsUI()
        {
            DOEUIFrozen = false;
            commonVars.setActiveUI(CommonVars.uiActive.doe, true);
        }

        void addDOESettingsHandlers()
        {
            DOEUIFrozen = false;
            commonVars.setActiveUI(CommonVars.uiActive.doe, true);
            num_DOERows.LostFocus += doeSettingsChanged;
            num_DOECols.LostFocus += doeSettingsChanged;
            num_DOEColPitch.LostFocus += doeSettingsChanged;
            num_DOERowPitch.LostFocus += doeSettingsChanged;
            radio_useSpecificTile.CheckedChanged += doeSettingsChanged;
            radio_useSpecificTiles.CheckedChanged += doeSettingsChanged;
            radioButton_allTiles.CheckedChanged += doeSettingsChanged;
            radio_useCSViDRM.CheckedChanged += doeSettingsChanged;
            radio_useCSVQuilt.CheckedChanged += doeSettingsChanged;
            textBox_useSpecificTiles.LostFocus += doeSettingsChanged;
            num_DOESCCol.LostFocus += doeSettingsChanged;
            num_DOESCRow.LostFocus += doeSettingsChanged;
            num_rowOffset.LostFocus += doeSettingsChanged;
            num_colOffset.LostFocus += doeSettingsChanged;
        }

        void doeSettingsChanged()
        {
            if (DOEUIFrozen)
            {
                return;
            }

            Application.Instance.Invoke(() =>
            {
                mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.DOE].clear();

                for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
                {
                    if (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(i) == 1)
                    {
                        for (int poly = 0; poly < commonVars.getLayerSettings(i).getFileData().Count(); poly++)
                        {
                            mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.DOE].addBGPolygon(
                                poly: UIHelper.myPointFArrayToPointFArray(commonVars.getLayerSettings(i).getFileData()[poly]),
                                polyColor: Color.FromArgb(commonVars.getColors().simPreviewColors[i].toArgb()),
                                alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.bg),
                                layerIndex: i
                                );
                        }
                    }
                }

                if (!(radio_useCSVQuilt.Checked) || ((radio_useCSVQuilt.Checked) && (!commonVars.getSimulationSettings().getDOESettings().getBool(DOESettings.properties_b.quilt))))
                {
                    btn_useCSVQuilt.Enabled = false;
                    groupBox_DOESettings.Enabled = true;
                }

                if ((!radio_useCSViDRM.Checked) || ((radio_useCSViDRM.Checked) && (!commonVars.getSimulationSettings().getDOESettings().getBool(DOESettings.properties_b.iDRM))))
                {
                    btn_useCSViDRM.Enabled = false;
                    groupBox_DOESettings.Enabled = true;
                }
                commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 0);
                commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTile, 0);
                num_DOESCCol.Enabled = false;
                num_DOESCRow.Enabled = false;
                commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.rowPitch, num_DOERowPitch.Value);
                commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.colPitch, num_DOEColPitch.Value);
                commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.rows, Convert.ToInt32(num_DOERows.Value));
                commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.cols, Convert.ToInt32(num_DOECols.Value));
                commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.colOffset, num_colOffset.Value);
                commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.rowOffset, num_rowOffset.Value);
                num_DOESCCol.MaxValue = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.cols);
                num_DOESCRow.MaxValue = commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.rows);
                commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTileCol, Convert.ToInt16(num_DOESCCol.Value));
                commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTileRow, Convert.ToInt16(num_DOESCRow.Value));

                if ((!radio_useCSViDRM.Checked) && (!radio_useCSVQuilt.Checked))
                {
                    commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.iDRM, false);
                    commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.quilt, false);
                    if (radioButton_allTiles.Checked)
                    {
                        // Nothing to do.
                    }

                    if (radio_useSpecificTile.Checked)
                    {
                        commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTile, 1);
                        num_DOESCCol.Enabled = true;
                        num_DOESCRow.Enabled = true;
                    }

                    if (radio_useSpecificTiles.Checked)
                    {
                        commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 1);
                        textBox_useSpecificTiles.Enabled = true;
                        string tmpString = textBox_useSpecificTiles.Text;
                        bool entryOK = commonVars.getSimulationSettings().setTileList(tmpString, false);

                        if (!entryOK) // failsafe.
                        {
                            commonVars.getSimulationSettings().getDOESettings().resetTileList_ColRow();
                            commonVars.getSimulationSettings().getDOESettings().addTileList_Value(new [] { 0, 0 });
                        }

                    }
                }
                else
                {
                    if (radio_useCSViDRM.Checked)
                    {
                        btn_useCSViDRM.Enabled = true;
                        commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.quilt, false);
                        // Grid settings come from CSV file.
                        commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 1);
                    }
                    if (radio_useCSVQuilt.Checked)
                    {
                        btn_useCSVQuilt.Enabled = true;
                        commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.iDRM, false);
                        // Grid settings come from CSV file.
                        commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 0);
                    }
                    groupBox_DOESettings.Enabled = false;
                    // Failsafe is handled earlier by checking for the iDRM/OPC button, but no 'configured' flag.
                }

                Int32 tile = 0;
                PointF[] tilePoly = new PointF[5];
                tilePoly[0] = new PointF((float)commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset), (float)commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset));
                tilePoly[1] = new PointF(tilePoly[0].X, (float)(tilePoly[0].Y + commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowPitch)));
                tilePoly[2] = new PointF((float)(tilePoly[1].X + commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colPitch)), tilePoly[1].Y);
                tilePoly[3] = new PointF(tilePoly[2].X, tilePoly[0].Y);
                tilePoly[4] = new PointF(tilePoly[0].X, tilePoly[0].Y);

                if (radioButton_allTiles.Checked || radio_useCSVQuilt.Checked)
                {
                    for (int row = 0; row < commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.rows); row++)
                    {
                        int col = 0;
                        while (col < commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.cols))
                        {
                            Int32 colorIndex = tile % varianceContext.vc.colors.resultColors.Count(); // map our result into the available colors.
                            PointF[] currentTile = tilePoly.ToArray();
                            int length = currentTile.Length;
#if !VARIANCESINGLETHREADED
                            Parallel.For(0, length, (pt) =>
#else
                            for (int pt = 0; pt < length; pt++)
#endif
                            {
                                currentTile[pt].X += (float)(col * commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colPitch));
                                currentTile[pt].Y += (float)(row * commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowPitch));
                            }
#if !VARIANCESINGLETHREADED
                            );
#endif
                            mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.DOE].addPolygon(
                                poly: currentTile,
                                polyColor: Color.FromArgb(commonVars.getColors().resultColors[colorIndex].toArgb()),
                                alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.fg),
                                drawn: true,
                                layerIndex: CentralProperties.maxLayersForMC + colorIndex);

                            tile++;
                            col++;
                        }
                    }
                }

                if (radio_useSpecificTile.Checked)
                {
                    PointF[] currentTile = tilePoly.ToArray();
                    int length = currentTile.Length;
#if !VARIANCESINGLETHREADED
                    Parallel.For(0, length, (pt) =>
#else
                    for (int pt = 0; pt < length; pt++)
#endif
                    {
                        currentTile[pt].X += (float)((commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.sTileCol) - 1) * commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colPitch));
                        currentTile[pt].Y += (float)((commonVars.getSimulationSettings().getDOESettings().getInt(DOESettings.properties_i.sTileRow) - 1) * commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowPitch));
                    }
#if !VARIANCESINGLETHREADED
                    );
#endif
                    mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.DOE].addPolygon(
                        poly: currentTile,
                        polyColor: Color.FromArgb(commonVars.getColors().resultColors[0].toArgb()),
                        alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.fg),
                        drawn: true,
                        layerIndex: CentralProperties.maxLayersForMC + 0);
                }

                if (radio_useSpecificTiles.Checked)
                {
                    while (tile < commonVars.getSimulationSettings().getDOESettings().getTileList_ColRow().Count())
                    {
                        Int32 colorIndex = tile % varianceContext.vc.colors.resultColors.Count(); // map our result into the available colors.
                        Int32 col = commonVars.getSimulationSettings().getDOESettings().getTileList_Value(tile, 0);
                        Int32 row = commonVars.getSimulationSettings().getDOESettings().getTileList_Value(tile, 1);
                        PointF[] currentTile = tilePoly.ToArray();
                        int length = currentTile.Length;
#if !VARIANCESINGLETHREADED
                        Parallel.For(0, length, (pt) =>
#else
                        for (int pt = 0; pt < length; pt++)
#endif
                        {
                            currentTile[pt].X += (float)(col * commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colPitch));
                            currentTile[pt].Y += (float)(row * commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowPitch));
                        }
#if !VARIANCESINGLETHREADED
                        );
#endif
                        mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.DOE].addPolygon(
                            poly: currentTile,
                            polyColor: Color.FromArgb(commonVars.getColors().resultColors[colorIndex].toArgb()),
                            alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.fg),
                            drawn: true,
                            layerIndex: CentralProperties.maxLayersForMC + colorIndex);
                        tile++;
                    }
                }
                entropyControl.EntropyRun(numberOfCases: 1, csvFile: null, useThreads: false, doPASearch: false); // force a tile extraction run to update tool.
                refreshAllPreviewPanels();
                updateViewport();
            });
        }

        void doeSettingsChanged(object sender, EventArgs e)
        {
            doeSettingsChanged();
        }
    }
}