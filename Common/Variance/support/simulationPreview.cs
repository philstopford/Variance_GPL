using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using geoLib;

namespace Variance
{
    public class SimulationPreview
    {
        VarianceContext varianceContext;

        List<PreviewShape> simShapes;
        public List<PreviewShape> getSimShapes()
        {
            return pGetSimShapes();
        }

        List<PreviewShape> pGetSimShapes()
        {
            return simShapes;
        }

        List<List<GeoLibPointF[]>> previewShapes;

        public List<List<GeoLibPointF[]>> getPreviewShapes()
        {
            return pGetPreviewShapes();
        }

        List<List<GeoLibPointF[]>> pGetPreviewShapes()
        {
            return previewShapes;
        }

        public List<GeoLibPointF[]> getLayerPreviewShapes(int layer)
        {
            return pGetLayerPreviewShapes(layer);
        }

        List<GeoLibPointF[]> pGetLayerPreviewShapes(int layer)
        {
            return previewShapes[layer];
        }

        public GeoLibPointF[] getLayerPreviewShapePoly(int layer, int poly)
        {
            return pGetLayerPreviewShapePoly(layer, poly);
        }

        GeoLibPointF[] pGetLayerPreviewShapePoly(int layer, int poly)
        {
            return previewShapes[layer][poly];
        }

        List<GeoLibPointF[]> points;

        public List<GeoLibPointF[]> getPoints()
        {
            return pGetPoints();
        }

        List<GeoLibPointF[]> pGetPoints()
        {
            return points;
        }

        public GeoLibPointF[] getPoints(int index)
        {
            return pGetPoints(index);
        }

        GeoLibPointF[] pGetPoints(int index)
        {
            return points[index];
        }

        string resultText;

        public string getResult()
        {
            return pGetResult();
        }

        string pGetResult()
        {
            return resultText;
        }

        void doOffsets(EntropyLayerSettings entropyLayerSettings)
        {
            // OK. Now we need to pay attention to the subshape reference settings.
            /*
				* 0: Top left
				* 1: Top right
				* 2: Bottom left
				* 3: Bottom right
				* 4: Top middle
				* 5: Right middle
				* 6: Bottom middle
				* 7: Left middle
				* 8: Center center
				*/
            if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.TL) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.TR) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.TS) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.RS) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.LS) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.C))
            {
                // Vertical offset needed to put reference corner at world center
                // Our coordinates have placed bottom left at 0,0 so negative offsets needed (note origin comment above)
                // Find our subshape reference.
                Int32 tmp_yOffset = 0;
                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                {
                    tmp_yOffset = Convert.ToInt32(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));
                }
                else
                {
                    tmp_yOffset = Convert.ToInt32(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));
                }

                // Half the value for a vertical centering requirement
                if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.RS) ||
                    (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.LS) ||
                    (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.C))
                {
                    tmp_yOffset = Convert.ToInt32(tmp_yOffset / 2);
                }
            }

            // Coordinates placed bottom left at 0,0.
            Int32 tmp_xOffset = 0;
            if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 1)
            {
                tmp_xOffset = -1 * Convert.ToInt32(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
            }
            if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.TR) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.BR) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.TS) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.RS) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.BS) ||
                (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.C))
            {
                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                {
                    tmp_xOffset -= Convert.ToInt32(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
                }
                else
                {
                    tmp_xOffset -= Convert.ToInt32(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength));
                }

                // Half the value for horizontal centering conditions
                if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.TS) ||
                    (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.BS) ||
                    (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex) == (int)CommonVars.subShapeLocations.C))
                {
                    tmp_xOffset = Convert.ToInt32(tmp_xOffset / 2);
                }
            }

            // Now for global offset.
            Convert.ToInt32(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset));
            Convert.ToInt32(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset));
        }

        public SimulationPreview(ref VarianceContext varianceContext)
        {
            pSimulationPreview(ref varianceContext);
        }

        void pSimulationPreview(ref VarianceContext _varianceContext)
        {
            varianceContext = _varianceContext;
            simShapes = new List<PreviewShape>();
            previewShapes = new List<List<GeoLibPointF[]>>();
            points = new List<GeoLibPointF[]>();
            points.Add(new GeoLibPointF[1]);
        }
        
        void updatePreview(List<PreviewShape> simShapes_)
        {
            try
            {
                simShapes = simShapes_.ToList();
            }
            catch (Exception)
            {
                // Doesn't matter.
            }
        }

        public void updatePreview(string resultText_)
        {
            pUpdatePreview(resultText_);
        }

        void pUpdatePreview(string resultText_)
        {
            resultText = resultText_;
        }

        public void updatePreview(SimResultPackage resultPackage)
        {
            pUpdatePreview(resultPackage);
        }

        void pUpdatePreview(SimResultPackage resultPackage)
        {
            if (Monitor.IsEntered(varianceContext.previewLock))
            {
                updatePreview(resultPackage.getPreviewResult().getSimShapes(), resultPackage.getPreviewResult().getPreviewShapes(),
                resultPackage.getPreviewResult().getPoints(), resultPackage.getMeanAndStdDev());
            }
        }

        public void updatePreview(List<PreviewShape> simShapes_, List<List<GeoLibPointF[]>> previewShapes_, List<GeoLibPointF[]> points_, string resultText_)
        {
            pUpdatePreview(simShapes_, previewShapes_, points_, resultText_);
        }

        void pUpdatePreview(List<PreviewShape> simShapes_, List<List<GeoLibPointF[]>> previewShapes_, List<GeoLibPointF[]> points_, string resultText_)
        {
            try
            {
                updatePreview(simShapes_, previewShapes_, points_);
            }
            catch (Exception)
            {
                // Doesn't matter.
            }
            try
            {
                updatePreview(resultText_);
            }
            catch (Exception)
            {
                // Doesn't matter.
            }
        }

        void updatePreview(List<PreviewShape> simShapes_, List<List<GeoLibPointF[]>> previewShapes_, List<GeoLibPointF[]> points_)
        {
            updatePreview(simShapes_);
            try
            {
                previewShapes = previewShapes_.ToList();
            }
            catch (Exception)
            {
                // Doesn't matter.
            }
            try
            {
                points = points_.ToList();
            }
            catch (Exception)
            {
                // Doesn't matter.
            }
        }
    }
}
