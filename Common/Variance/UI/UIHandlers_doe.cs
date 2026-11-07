using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using System.Threading.Tasks;
using utility;
using VeldridEto;

namespace Variance;

public partial class MainForm
{
    private void suspendDOESettingsUI()
    {
        suspendDOESettingsUI(null);
    }

    private void suspendDOESettingsUI(Action callback)
    {
        Application.Instance.Invoke(() =>
        {
            DOEUIFrozen = true;
            commonVars.setActiveUI(CommonVars.uiActive.doe, false);
            if (callback != null)
                callback.Invoke();
        });
    }

    private void resumeDOESettingsUI()
    {
        DOEUIFrozen = false;
        commonVars.setActiveUI(CommonVars.uiActive.doe, true);
    }

    private void addDOESettingsHandlers()
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

    private void doeSettingsChanged()
    {
        if (DOEUIFrozen)
        {
            return;
        }

        Application.Instance.Invoke(() =>
        {
            mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.DOE].clear();

            // Cache frequently accessed objects to reduce method call overhead
            var doeSettings = commonVars.getSimulationSettings().getDOESettings();
            var colors = commonVars.getColors();
            float bgOpacity = (float)commonVars.getOpacity(CommonVars.opacity_gl.bg);
            var doeViewportSettings = mcVPSettings[CentralProperties.maxLayersForMC - 1 + (int)CommonVars.twoDTabNames.DOE];

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (doeSettings.getLayerAffected(i) != 1)
                {
                    continue;
                }

                var layerSettings = commonVars.getLayerSettings(i);
                var fileData = layerSettings.getFileData();
                int polyCount = fileData.Count;
                var polyColor = Color.FromArgb(colors.simPreviewColors[i].toArgb());

                for (int poly = 0; poly < polyCount; poly++)
                {
                    doeViewportSettings.addBGPolygon(
                        poly: OVPSettings.convertToClosedPointF(fileData[poly]),
                        polyColor: polyColor,
                        alpha: bgOpacity,
                        layerIndex: i
                    );
                }
            }

            if (!radio_useCSVQuilt.Checked || radio_useCSVQuilt.Checked && !doeSettings.getBool(DOESettings.properties_b.quilt))
            {
                btn_useCSVQuilt.Enabled = false;
                groupBox_DOESettings.Enabled = true;
            }

            if (!radio_useCSViDRM.Checked || radio_useCSViDRM.Checked && !doeSettings.getBool(DOESettings.properties_b.iDRM))
            {
                btn_useCSViDRM.Enabled = false;
                groupBox_DOESettings.Enabled = true;
            }
            
            // Batch DOE settings updates to reduce method call overhead
            doeSettings.setInt(DOESettings.properties_i.uTileList, 0);
            doeSettings.setInt(DOESettings.properties_i.sTile, 0);
            num_DOESCCol.Enabled = false;
            num_DOESCRow.Enabled = false;
            doeSettings.setDouble(DOESettings.properties_d.rowPitch, num_DOERowPitch.Value);
            doeSettings.setDouble(DOESettings.properties_d.colPitch, num_DOEColPitch.Value);
            
            // Cache converted values to avoid repeated conversions
            int rowsValue = Convert.ToInt32(num_DOERows.Value);
            int colsValue = Convert.ToInt32(num_DOECols.Value);
            
            doeSettings.setInt(DOESettings.properties_i.rows, rowsValue);
            doeSettings.setInt(DOESettings.properties_i.cols, colsValue);
            doeSettings.setDouble(DOESettings.properties_d.colOffset, num_colOffset.Value);
            doeSettings.setDouble(DOESettings.properties_d.rowOffset, num_rowOffset.Value);
            
            // Use cached values instead of re-fetching
            num_DOESCCol.MaxValue = colsValue;
            num_DOESCRow.MaxValue = rowsValue;
            doeSettings.setInt(DOESettings.properties_i.sTileCol, Convert.ToInt16(num_DOESCCol.Value));
            doeSettings.setInt(DOESettings.properties_i.sTileRow, Convert.ToInt16(num_DOESCRow.Value));

            if (!radio_useCSViDRM.Checked && !radio_useCSVQuilt.Checked)
            {
                doeSettings.setBool(DOESettings.properties_b.iDRM, false);
                doeSettings.setBool(DOESettings.properties_b.quilt, false);
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
                        commonVars.getSimulationSettings().getDOESettings().addTileList_Value([0, 0]);
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

            int tile = 0;
            // Cache DOE settings to avoid repeated method calls
            double colOffset = doeSettings.getDouble(DOESettings.properties_d.colOffset);
            double rowOffset = doeSettings.getDouble(DOESettings.properties_d.rowOffset);
            double colPitch = doeSettings.getDouble(DOESettings.properties_d.colPitch);
            double rowPitch = doeSettings.getDouble(DOESettings.properties_d.rowPitch);
            int rows = doeSettings.getInt(DOESettings.properties_i.rows);
            int cols = doeSettings.getInt(DOESettings.properties_i.cols);
            
            PointF[] tilePoly = new PointF[5];
            tilePoly[0] = new PointF((float)colOffset, (float)rowOffset);
            tilePoly[1] = new PointF(tilePoly[0].X, (float)(tilePoly[0].Y + rowPitch));
            tilePoly[2] = new PointF((float)(tilePoly[1].X + colPitch), tilePoly[1].Y);
            tilePoly[3] = new PointF(tilePoly[2].X, tilePoly[0].Y);
            tilePoly[4] = new PointF(tilePoly[0].X, tilePoly[0].Y);

            if (radioButton_allTiles.Checked || radio_useCSVQuilt.Checked)
            {
                // Calculate total tiles for potential parallelization
                int totalTiles = rows * cols;
                var resultColors = varianceContext.vc.colors.resultColors;
                float fgOpacity = (float)commonVars.getOpacity(CommonVars.opacity_gl.fg);
                var castColPitch = (float)colPitch;
                var castRowPitch = (float)rowPitch;
                
                // Optimize by parallelizing the tile generation when beneficial
                if (totalTiles > 100) // Threshold for parallelization
                {
                    ParallelProcessing.OptimizedParallelFor(0, totalTiles, tileIndex =>
                    {
                        int row = tileIndex / cols;
                        int col = tileIndex % cols;
                        int colorIndex = tileIndex % resultColors.Length;
                        
                        PointF[] currentTile = new PointF[tilePoly.Length];
                        Array.Copy(tilePoly, currentTile, tilePoly.Length);
                        
                        // Apply transformations using cached values
                        for (int pt = 0; pt < currentTile.Length; pt++)
                        {
                            currentTile[pt].X += col * castColPitch;
                            currentTile[pt].Y += row * castRowPitch;
                        }
                        
                        lock (doeViewportSettings) // Thread-safe viewport access
                        {
                            doeViewportSettings.addPolygon(
                                poly: currentTile,
                                polyColor: Color.FromArgb(resultColors[colorIndex].toArgb()),
                                alpha: fgOpacity,
                                drawn: true,
                                layerIndex: CentralProperties.maxLayersForMC + colorIndex);
                        }
                    });
                }
                else
                {
                    // Use original approach for small tile counts
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        {
                            int colorIndex = tile % resultColors.Length;
                            PointF[] currentTile = tilePoly.ToArray();
                            int length = currentTile.Length;
                            ParallelProcessing.OptimizedParallelFor(0, length, pt =>
                            {
                                currentTile[pt].X += col * castColPitch;
                                currentTile[pt].Y += row * castRowPitch;
                            }
                            );
                            doeViewportSettings.addPolygon(
                                poly: currentTile,
                                polyColor: Color.FromArgb(resultColors[colorIndex].toArgb()),
                                alpha: fgOpacity,
                                drawn: true,
                                layerIndex: CentralProperties.maxLayersForMC + colorIndex);
                            tile++;
                        }
                    }
                }
            }

            if (radio_useSpecificTile.Checked)
            {
                PointF[] currentTile = tilePoly.ToArray();
                int length = currentTile.Length;
                
                // Cache tile calculations using already cached values
                float tileColOffset = (float)((doeSettings.getInt(DOESettings.properties_i.sTileCol) - 1) * colPitch);
                float tileRowOffset = (float)((doeSettings.getInt(DOESettings.properties_i.sTileRow) - 1) * rowPitch);
                
                ParallelProcessing.OptimizedParallelFor(0, length, pt =>
                    {
                        currentTile[pt].X += tileColOffset;
                        currentTile[pt].Y += tileRowOffset;
                    }
                );
                doeViewportSettings.addPolygon(
                    poly: currentTile,
                    polyColor: Color.FromArgb(colors.resultColors[0].toArgb()),
                    alpha: (float)commonVars.getOpacity(CommonVars.opacity_gl.fg),
                    drawn: true,
                    layerIndex: CentralProperties.maxLayersForMC + 0);
            }

            if (radio_useSpecificTiles.Checked)
            {
                var tileList = doeSettings.getTileList_ColRow();
                int tileListCount = tileList.Count;
                var resultColors = varianceContext.vc.colors.resultColors;
                float fgOpacity = (float)commonVars.getOpacity(CommonVars.opacity_gl.fg);
                var castColPitch = (float)colPitch;
                var castRowPitch = (float)rowPitch;
                
                // Parallelize specific tiles processing when beneficial
                if (tileListCount > 50)
                {
                    ParallelProcessing.OptimizedParallelFor(0, tileListCount, tileIndex =>
                    {
                        int colorIndex = tileIndex % resultColors.Length;
                        int col = doeSettings.getTileList_Value(tileIndex, 0);
                        int row = doeSettings.getTileList_Value(tileIndex, 1);
                        PointF[] currentTile = new PointF[tilePoly.Length];
                        Array.Copy(tilePoly, currentTile, tilePoly.Length);
                        
                        for (int pt = 0; pt < currentTile.Length; pt++)
                        {
                            currentTile[pt].X += col * castColPitch;
                            currentTile[pt].Y += row * castRowPitch;
                        }
                        
                        lock (doeViewportSettings)
                        {
                            doeViewportSettings.addPolygon(
                                poly: currentTile,
                                polyColor: Color.FromArgb(resultColors[colorIndex].toArgb()),
                                alpha: fgOpacity,
                                drawn: true,
                                layerIndex: CentralProperties.maxLayersForMC + colorIndex);
                        }
                    });
                }
                else
                {
                    // Use optimized sequential approach for small lists
                    for (tile = 0; tile < tileListCount; tile++)
                    {
                        int colorIndex = tile % resultColors.Length;
                        int col = doeSettings.getTileList_Value(tile, 0);
                        int row = doeSettings.getTileList_Value(tile, 1);
                        PointF[] currentTile = tilePoly.ToArray();
                        int length = currentTile.Length;
                        ParallelProcessing.OptimizedParallelFor(0, length, pt =>
                        {
                            currentTile[pt].X += col * castColPitch;
                            currentTile[pt].Y += row * castRowPitch;
                        }
                        );
                        doeViewportSettings.addPolygon(
                            poly: currentTile,
                            polyColor: Color.FromArgb(resultColors[colorIndex].toArgb()),
                            alpha: fgOpacity,
                            drawn: true,
                            layerIndex: CentralProperties.maxLayersForMC + colorIndex);
                    }
                }
            }
            entropyControl.EntropyRun(numberOfCases: 1, csvFile: null, useThreads: false, doPASearch: false); // force a tile extraction run to update tool.
            refreshAllPreviewPanels();
            updateViewport();
        });
    }

    private void doeSettingsChanged(object sender, EventArgs e)
    {
        doeSettingsChanged();
    }
}