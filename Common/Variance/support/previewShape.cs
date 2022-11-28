using System;
using System.Collections.Generic;
using System.Linq;
using Clipper2Lib;
using color;
using Error;
using geoLib;
using geoWrangler;
using Noise;
using utility; // tiled layout handling, Layout biasing/CDU.
using System.Threading.Tasks;
using shapeEngine;

namespace Variance;

using Path = Path64;
using Paths = Paths64;

public class PreviewShape
{
    private bool DOEDependency; // due to the DOE grid, we need this to sort out offsets. This includes buried references in Booleans. The min X/Y values for this case need to be at least the col/row offset.

    // Class for our preview shapes.
    private List<GeoLibPointF[]> previewPoints; // list of polygons defining the shape(s) that will be drawn. In the complex case, we populate this from complexPoints.
    public List<GeoLibPointF[]> getPoints()
    {
        return pGetPoints();
    }

    private List<GeoLibPointF[]> pGetPoints()
    {
        return previewPoints;
    }

    public GeoLibPointF[] getPoints(int index)
    {
        return pGetPoints(index);
    }

    private GeoLibPointF[] pGetPoints(int index)
    {
        return previewPoints[index];
    }

    public void addPoints(GeoLibPointF[] poly)
    {
        pAddPoints(poly);
    }

    private void pAddPoints(GeoLibPointF[] poly)
    {
        previewPoints.Add(poly);
    }

    public void setPoints(List<GeoLibPointF[]> newPoints)
    {
        pSetPoints(newPoints);
    }

    private void pSetPoints(List<GeoLibPointF[]> newPoints)
    {
        previewPoints = newPoints.ToList();
    }

    public void clearPoints()
    {
        pClearPoints();
    }

    private void pClearPoints()
    {
        previewPoints.Clear();
    }

    private List<bool> drawnPoly; // to track drawn vs enabled polygons. Can then use for filtering elsewhere.

    public bool getDrawnPoly(int index)
    {
        return pGetDrawnPoly(index);
    }

    private bool pGetDrawnPoly(int index)
    {
        return drawnPoly[index];
    }

    private List<bool> geoCoreOrthogonalPoly;
    private MyColor color;

    public MyColor getColor()
    {
        return pGetColor();
    }

    private MyColor pGetColor()
    {
        return color;
    }

    public void setColor(MyColor c)
    {
        pSetColor(c);
    }

    private void pSetColor(MyColor c)
    {
        color = new MyColor(c);
    }

    private double xOffset;
    private double yOffset;

    private int _settingsIndex; // track originating layer.

    public int getIndex()
    {
        return pGetIndex();
    }

    private int pGetIndex()
    {
        return _settingsIndex;
    }
    
    public PreviewShape()
    {
        init();
    }

    private void init()
    {
        // Stub to enable direct drive of preview data, primarily for the implant system.
        previewPoints = new List<GeoLibPointF[]>();
        drawnPoly = new List<bool>();
        geoCoreOrthogonalPoly = new List<bool>();
        color = MyColor.Black;
    }

    public PreviewShape(PreviewShape source)
    {
        init(source);
    }

    private void init(PreviewShape source)
    {
        _settingsIndex = source._settingsIndex;
        previewPoints = source.previewPoints.ToList();
        drawnPoly = source.drawnPoly.ToList();
        geoCoreOrthogonalPoly = source.geoCoreOrthogonalPoly.ToList();
        color = new MyColor(source.color);
    }

    public PreviewShape(CommonVars commonVars, int settingsIndex, int subShapeIndex, int mode, bool doPASearch, bool previewMode, int currentRow, int currentCol)
    {
        xOffset = 0;
        yOffset = 0;
        init(commonVars, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol);
    }

    public PreviewShape(CommonVars commonVars, ChaosSettings jobSettings_, int settingsIndex, int subShapeIndex, int mode, bool doPASearch, bool previewMode, int currentRow, int currentCol)
    {
        xOffset = 0;
        yOffset = 0;
        init(commonVars, jobSettings_, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol);
    }

    private bool exitEarly;

    private void applyNoise(bool previewMode, CommonVars commonVars, ChaosSettings jobSettings, int settingsIndex)
    {
        EntropyLayerSettings entropyLayerSettings = commonVars.getLayerSettings(settingsIndex);

        double lwrConversionFactor = commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.ler) == 1 ? Math.Sqrt(2) : 0.5f;

        // LWR, skip if not requested to avoid runtime pain
        if ((!previewMode || entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1) && entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr) != 0)
        {
            double jitterScale = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr)) / lwrConversionFactor; // LWR jitter of edge; use RSS for stricter assessment
            if (!previewMode && entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1 && !jobSettings.getPreviewMode())
            {
                // This used to be easier, but now we have the case of a non-preview mode, but the layer setting calls for a preview.
                jitterScale *= jobSettings.getValue(ChaosSettings.properties.LWRVar, settingsIndex);
            }

            previewPoints = NoiseC.doNoise(previewPoints,drawnPoly,
                noiseType: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrType),
                seed: jobSettings.getInt(ChaosSettings.ints.lwrSeed, settingsIndex),
                freq: Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwrFreq)),
                jitterScale: jitterScale
            );
        }

        if ((!previewMode || entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1) && entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr2) != 0)
        {
            // LWR2, skip if not requested to avoid runtime pain
            double jitterScale = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr2)) / lwrConversionFactor; // LWR jitter of edge; use RSS for stricter assessment
            if (!previewMode && entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1 && !jobSettings.getPreviewMode())
            {
                // This used to be easier, but now we have the case of a non-preview mode, but the layer setting calls for a preview.
                jitterScale *= jobSettings.getValue(ChaosSettings.properties.LWR2Var, settingsIndex);
            }

            previewPoints = NoiseC.doNoise(previewPoints,drawnPoly,
                noiseType: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwr2Type),
                seed: jobSettings.getInt(ChaosSettings.ints.lwr2Seed, settingsIndex),
                freq: Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq)),
                jitterScale: jitterScale
            );
        }
    }

    private void init(CommonVars commonVars, int settingsIndex, int subShapeIndex, int mode, bool doPASearch, bool previewMode, int currentRow, int currentCol)
    {
        ChaosSettings jobSettings_ = new(previewMode, commonVars.getListOfSettings(), commonVars.getSimulationSettings());
        init(commonVars, jobSettings_, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol);
    }

    private void init(CommonVars commonVars, ChaosSettings chaosSettings, int settingsIndex, int subShapeIndex, int mode, bool doPASearch, bool previewMode, int currentRow, int currentCol, EntropyLayerSettings entropyLayerSettings = null, bool doClockwiseGeoFix = true, bool process_overlaps = true)
    {
        _settingsIndex = settingsIndex;
        try
        {
            DOEDependency = false;
            previewPoints = new List<GeoLibPointF[]>();
            drawnPoly = new List<bool>();
            geoCoreOrthogonalPoly = new List<bool>();
            color = MyColor.Black; // overridden later.

            switch (entropyLayerSettings)
            {
                case null:
                    entropyLayerSettings = commonVars.getLayerSettings(settingsIndex);
                    break;
            }
            if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CentralProperties.shapeNames.GEOCORE)
            {
                init_geoCore(commonVars, chaosSettings, settingsIndex, entropyLayerSettings, mode, doPASearch, previewMode, process_overlaps, doClockwiseGeoFix);
                // Get our offsets configured. We need to check for DOE settings here, to prevent relocation of extracted polygons within the tile during offset evaluation.
                if (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1)
                {
                    DOEDependency = true;
                    commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
                    commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
                }
                GeoLibPointF offset = shapeOffsets.doOffsets(0, entropyLayerSettings);
                xOffset = offset.X;
                yOffset = offset.Y;
            }
            else // not geoCore related.
            {
                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CentralProperties.shapeNames.BOOLEAN)
                {
                    try
                    {
                        init_boolean(commonVars, chaosSettings, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol, entropyLayerSettings);
                        // Get our offsets configured.
                        // Is any input layer coming from a GDS DOE tile? We need to check for DOE settings here, to prevent relocation of extracted polygons within the tile during offset evaluation.
                        int boolLayer = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerA);
                        while (boolLayer > 0)
                        {
                            DOEDependency = commonVars.getSimulationSettings().getDOESettings().getLayerAffected(boolLayer) == 1;
                            if (DOEDependency)
                            {
                                break;
                            }
                            boolLayer = commonVars.getLayerSettings(boolLayer).getInt(EntropyLayerSettings.properties_i.bLayerA);
                        }
                        if (!DOEDependency)
                        {
                            boolLayer = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerB);
                            while (boolLayer > 0)
                            {
                                DOEDependency = commonVars.getSimulationSettings().getDOESettings().getLayerAffected(boolLayer) == 1;
                                if (DOEDependency)
                                {
                                    break;
                                }
                                boolLayer = commonVars.getLayerSettings(boolLayer).getInt(EntropyLayerSettings.properties_i.bLayerB);
                            }
                        }
                        if (DOEDependency)
                        {
                            commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
                            commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
                        }

                        GeoLibPointF offset = shapeOffsets.doOffsets(0, entropyLayerSettings);
                        xOffset = offset.X;
                        yOffset = offset.Y;
                    }
                    catch (Exception)
                    {
                    }
                    exitEarly = true; // avoid second pass of distortion, etc.
                }
                else
                {
                    init_general(commonVars, chaosSettings, settingsIndex, entropyLayerSettings, mode, doPASearch, previewMode, subShapeIndex);
                }
            }

            if (exitEarly || mode != 1)
            {
                return;
            }

            // Fragment by resolution
            Fragmenter fragment = new Fragmenter(commonVars.getSimulationSettings().getResolution(), CentralProperties.scaleFactorForOperation);

            Parallel.For(0, previewPoints.Count, i =>
            {
                if (!drawnPoly[i])
                {
                    previewPoints[i] = fragment.fragmentPath(GeoWrangler.stripColinear(previewPoints[i]));
                }
            });

            // Apply lens distortion.
            previewPoints = distortShape.distortion(previewPoints, drawnPoly.ToArray(),
                commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC1),
                commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC2),
                commonVars.getSimulationSettings().getResolution(), CentralProperties.scaleFactorForOperation);

            Parallel.For(0, previewPoints.Count, i =>
            {
                if (!drawnPoly[i])
                {
                    previewPoints[i] = fragment.fragmentPath(GeoWrangler.stripColinear(previewPoints[i]));
                }
            });

            // Noise and proximity biasing.
            applyNoise(previewMode, commonVars, chaosSettings, settingsIndex);
            GeometryResult ret = Proximity.proximityBias(previewPoints, drawnPoly,
                commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.pBias),
                commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.pBiasDist),
                commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.proxRays),
                commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.proxSideRaysFallOff),
                commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.proxSideRaysMultiplier),
                commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.rayExtension),
                commonVars.getSimulationSettings().getResolution(),
                CentralProperties.scaleFactorForOperation, false, 0
                );

            previewPoints = ret.geometry.ToList();
            drawnPoly = ret.drawn.ToList();
            Parallel.For(0, previewPoints.Count, i =>
            {
                if (!drawnPoly[i])
                {
                    previewPoints[i] = fragment.fragmentPath(GeoWrangler.stripColinear(previewPoints[i]));
                }
            });
        }
        catch (Exception)
        {
        }
    }

    private void init_general(CommonVars commonVars, ChaosSettings chaosSettings, int settingsIndex, EntropyLayerSettings entropyLayerSettings, int mode, bool doPASearch, bool previewMode, int subShapeIndex)
    {
        if (mode == 0)
        {
            // Basic shape - 5 points to make a closed preview. 5th is identical to 1st.
            GeoLibPointF[] tempArray = new GeoLibPointF[5];

            // Need exception handling here for overflow cases?
            decimal bottom_leftX = 0, bottom_leftY = 0;
            decimal top_leftX = 0, top_leftY = 0;
            decimal top_rightX = 0, top_rightY = 0;
            decimal bottom_rightX = 0, bottom_rightY = 0;
            top_leftY = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verLength, subShapeIndex);
            top_rightX = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horLength, subShapeIndex);
            top_rightY = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verLength, subShapeIndex);
            bottom_rightX = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horLength, subShapeIndex);
            switch (subShapeIndex)
            {
                case 0:
                    bottom_leftX = 0;
                    bottom_leftY = 0;
                    top_leftX = 0;
                    bottom_rightY = 0;
                    xOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horOffset, subShapeIndex));
                    yOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verOffset, subShapeIndex));
                    break;
                case 1:
                    bottom_leftX = 0;
                    bottom_leftY = 0;
                    top_leftX = 0;
                    bottom_rightY = 0;
                    xOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 0) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horOffset, subShapeIndex));
                    yOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 0) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verOffset, subShapeIndex));
                    break;
                case 2:
                {
                    bottom_leftX = 0;
                    bottom_leftY = 0;
                    top_leftX = 0;
                    bottom_rightY = 0;
                    xOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 2) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 0));
                    yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 2) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verLength, 2) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 0));
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CentralProperties.shapeNames.Sshape)
                    {
                        yOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verLength, 0)); // offset our subshape to put it in the correct place in the UI.
                    }

                    break;
                }
            }

            // Populate array.
            tempArray[0] = new GeoLibPointF((double)bottom_leftX, (double)bottom_leftY);
            tempArray[1] = new GeoLibPointF((double)top_leftX, (double)top_leftY);
            tempArray[2] = new GeoLibPointF((double)top_rightX, (double)top_rightY);
            tempArray[3] = new GeoLibPointF((double)bottom_rightX, (double)bottom_rightY);
            tempArray[4] = new GeoLibPointF(tempArray[0]);

            // Apply our deltas
            int tLength = tempArray.Length;
#if !VARIANCESINGLETHREADED
            Parallel.For(0, tLength, i => 
#else
                for (Int32 i = 0; i < tLength; i++)
#endif
                {
                    tempArray[i].X += xOffset;
                    tempArray[i].Y += yOffset;
                }
#if !VARIANCESINGLETHREADED
            );
#endif
            previewPoints.Add(tempArray);
            drawnPoly.Add(true);
        }
        else
        {
            // Complex shape
            try
            {
                EntropyShape complexPoints = new(commonVars.getSimulationSettings(), commonVars.getListOfSettings(), settingsIndex, doPASearch, previewMode, chaosSettings);
                previewPoints.Add(complexPoints.getPoints());
                drawnPoly.Add(false);
            }
            catch (Exception)
            {

            }
        }
        // Get our offsets configured.
        GeoLibPointF offset = shapeOffsets.doOffsets(0, entropyLayerSettings);
        xOffset = offset.X;
        yOffset = offset.Y;

        int pCount = previewPoints.Count;
        for (int poly = 0; poly < pCount; poly++)
        {
            int ptCount = previewPoints[poly].Length;
            int poly1 = poly;
#if !VARIANCESINGLETHREADED
            Parallel.For(0, ptCount, point =>
#else
                for (Int32 point = 0; point < ptCount; point++)
#endif
                {
                    double px = previewPoints[poly1][point].X + xOffset;
                    double py = previewPoints[poly1][point].Y - yOffset;

                    previewPoints[poly1][point] = new GeoLibPointF(px, py);
                }
#if !VARIANCESINGLETHREADED
            );
#endif
            if (Math.Abs(previewPoints[poly][0].X - previewPoints[poly][previewPoints[poly].Length - 1].X) > double.Epsilon ||
                Math.Abs(previewPoints[poly][0].Y - previewPoints[poly][previewPoints[poly].Length - 1].Y) > double.Epsilon)
            {
                ErrorReporter.showMessage_OK("Start and end not the same - previewShape", "Oops");
            }
        }
    }
    private void init_geoCore(CommonVars commonVars, ChaosSettings chaosSettings, int settingsIndex, EntropyLayerSettings entropyLayerSettings, int mode, bool doPASearch, bool previewMode, bool process_overlaps, bool forceClockwise)
    {
        Fragmenter fragment = new Fragmenter(commonVars.getSimulationSettings().getResolution(), CentralProperties.scaleFactorForOperation);

        // We'll use these to shift the points around.
        double xOverlayVal = 0.0f;
        double yOverlayVal = 0.0f;

        xOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 0));
        yOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 0));

        // OK. We need to crop our layout based on the active tile if there is a DOE flag set.
        bool tileHandlingNeeded = commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1;

        if (mode == 1)
        {
            // We need this check and early return because previewShape is now used in the layer preview
            // mode to handle bias on geoCore elements. Populating this when the layer is not enabled
            // causes the shared structure with the simulation engine to be defined and breaks everything.
            // Instead we just make a zero area polygon (to avoid issues downstream) and return early.
            if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.enabled) == 0)
            {
                previewPoints.Add(new GeoLibPointF[4]);
                for (int i = 0; i < 4; i++)
                {
                    previewPoints[0][i] = new GeoLibPointF(0, 0);
                }
                drawnPoly.Add(false);
                geoCoreOrthogonalPoly.Add(true);
                return;
            }

            switch (previewMode)
            {
                case true when tileHandlingNeeded:
                {
                    if (!commonVars.getLayerPreviewDOETile())
                    {
                        tileHandlingNeeded = false;
                    }

                    break;
                }
                // Get overlay figured out.
                case false:
                {
                    xOverlayVal = chaosSettings.getValue(ChaosSettings.properties.overlayX, settingsIndex) * Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.xOL));
                    yOverlayVal = chaosSettings.getValue(ChaosSettings.properties.overlayY, settingsIndex) * Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.yOL));

                    // Handle overlay reference setting
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_av) == 1) // overlay average
                    {
                        List<double> overlayValues = new();
                        for (int avgolref_x = 0; avgolref_x < entropyLayerSettings.getIntArray(EntropyLayerSettings.properties_intarray.xOLRefs).Length; avgolref_x++)
                        {
                            if (entropyLayerSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, avgolref_x) == 1)
                            {
                                overlayValues.Add(chaosSettings.getValue(ChaosSettings.properties.overlayX, avgolref_x) * Convert.ToDouble(commonVars.getLayerSettings(avgolref_x).getDecimal(EntropyLayerSettings.properties_decimal.xOL))); // Overlay shift
                            }
                        }

                        xOverlayVal += overlayValues.Average();
                    }
                    else // vanilla overlay reference mode
                    {
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) != -1)
                        {
                            xOverlayVal += chaosSettings.getValue(ChaosSettings.properties.overlayX, entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref)) * Convert.ToDouble(commonVars.getLayerSettings(entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref)).getDecimal(EntropyLayerSettings.properties_decimal.xOL));
                        }
                    }

                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_av) == 1) // overlay average
                    {
                        List<double> overlayValues = new();
                        for (int avgolref_y = 0; avgolref_y < entropyLayerSettings.getIntArray(EntropyLayerSettings.properties_intarray.yOLRefs).Length; avgolref_y++)
                        {
                            if (entropyLayerSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, avgolref_y) == 1)
                            {
                                overlayValues.Add(chaosSettings.getValue(ChaosSettings.properties.overlayY, avgolref_y) * Convert.ToDouble(commonVars.getLayerSettings(avgolref_y).getDecimal(EntropyLayerSettings.properties_decimal.yOL))); // Overlay shift
                            }
                        }

                        yOverlayVal += overlayValues.Average();
                    }
                    else // vanilla overlay reference mode
                    {
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) != -1)
                        {
                            yOverlayVal += chaosSettings.getValue(ChaosSettings.properties.overlayY, entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref)) * Convert.ToDouble(commonVars.getLayerSettings(entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref)).getDecimal(EntropyLayerSettings.properties_decimal.yOL));
                        }
                    }

                    break;
                }
            }

            // Decouple the geometry here to avoid manipulation going back to original source.
            List<GeoLibPointF[]> tempPolyList;
            switch (tileHandlingNeeded)
            {
                case true:
                    tempPolyList = commonVars.getNonSimulationSettings().extractedTile[settingsIndex].ToList();
                    break;
                default:
                    tempPolyList = entropyLayerSettings.getFileData().ToList();
                    break;
            }
            try
            {
                double minx = tempPolyList[0][0].X;
                double miny = tempPolyList[0][0].Y;
                double maxx = tempPolyList[0][0].X;
                double maxy = tempPolyList[0][0].Y;
                int tPCount = tempPolyList.Count;
                for (int poly = 0; poly < tPCount; poly++)
                {
                    double min_x = tempPolyList[poly].Min(p => p.X);
                    double min_y = tempPolyList[poly].Min(p => p.Y);
                    double max_x = tempPolyList[poly].Max(p => p.X);
                    double max_y = tempPolyList[poly].Max(p => p.Y);

                    if (min_x < minx)
                    {
                        minx = min_x;
                    }
                    if (min_y < miny)
                    {
                        miny = min_y;
                    }
                    if (max_x > maxx)
                    {
                        maxx = max_x;
                    }
                    if (max_y > maxy)
                    {
                        maxy = max_y;
                    }
                }

                GeoLibPointF bb_mid = new(minx + (maxx - minx) / 2.0f, miny + (maxy - miny) / 2.0f);
                bb_mid.X += xOverlayVal + (double)entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset) + (double)entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 0);
                bb_mid.Y += yOverlayVal + (double)entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset) + (double)entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 0);

                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.perPoly) == 1)
                {
                    bb_mid = null;
                }

                for (int poly = 0; poly < tPCount; poly++)
                {
                    GeoLibPointF[] tempPoly;

                    if (tileHandlingNeeded)
                    {
                        // Poly is already closed - presents a problem if we use contouring.
                        int arraySize = tempPolyList[poly].Length;

                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1)
                        {
                            if (Math.Abs(tempPolyList[poly][0].X - tempPolyList[poly][tempPolyList[poly].Length - 1].X) < double.Epsilon && Math.Abs(tempPolyList[poly][0].Y - tempPolyList[poly][tempPolyList[poly].Length - 1].Y) < double.Epsilon)
                            {
                                arraySize--;
                            }
                        }

                        tempPoly = new GeoLibPointF[arraySize];
                        GeoLibPointF[] poly1 = tempPoly;
                        int poly2 = poly;

#if !VARIANCESINGLETHREADED
                        Parallel.For(0, arraySize, pt => 
#else
                            for (int pt = 0; pt < arraySize; pt++)
#endif
                            {
                                poly1[pt] = new GeoLibPointF(tempPolyList[poly2][pt].X + xOffset, tempPolyList[poly2][pt].Y + yOffset);
                            }
#if !VARIANCESINGLETHREADED
                        );
#endif
                    }
                    else
                    {
                        int polySize = entropyLayerSettings.getFileData()[poly].Length;

                        tempPoly = new GeoLibPointF[polySize];

                        GeoLibPointF[] poly1 = tempPoly;
                        int poly2 = poly;
#if !VARIANCESINGLETHREADED
                        Parallel.For(0, polySize, pt => 
#else
                            for (Int32 pt = 0; pt < polySize; pt++)
#endif
                            {
                                poly1[pt] = new GeoLibPointF(entropyLayerSettings.getFileData()[poly2][pt].X + xOffset, entropyLayerSettings.getFileData()[poly2][pt].Y + yOffset);
                            }
#if !VARIANCESINGLETHREADED
                        );
#endif
                    }

                    bool drawn = false;

                    // Compatibility shim - we need to toggle this behavior due to the ILB passing in mixed orientation geometry that we don't want to clobber.
                    // However, external geometry may need this spin fixing. Although the upper levels should also re-spin geometry properly - we don't assume this.
                    if (forceClockwise)
                    {
                        tempPoly = GeoWrangler.clockwiseAndReorderXY(tempPoly); // force clockwise order and lower-left at 0 index.
                    }

                    // Strip termination points. Set shape will take care of additional clean-up if needed.
                    // tempPoly = GeoWrangler.stripTerminators(tempPoly, false);

                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
                    {
                        previewPoints.Add(fragment.fragmentPath(tempPoly));
                        geoCoreOrthogonalPoly.Add(false); // We need to populate the list, but in this non-contoured case, the value doesn't matter.
                    }
                    else
                    {
                        // Feed tempPoly to shape engine.
                        ShapeLibrary shape = new(CentralProperties.shapeTable, entropyLayerSettings);

                        shape.setShape(entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex), tempPoly); // feed the shape engine with the geometry using our optional parameter.
                        EntropyShape complexPoints = new(commonVars.getSimulationSettings(), commonVars.getListOfSettings(), settingsIndex, doPASearch, previewMode, chaosSettings, shape, bb_mid);
                        // Add resulting shape to the previewPoints.
                        previewPoints.Add(complexPoints.getPoints());
                        // This list entry does matter - we need to choose the right expansion method in case contouring has been chosen, but the
                        // polygon is not orthogonal.
                        geoCoreOrthogonalPoly.Add(shape.geoCoreShapeOrthogonal);
                    }
                    drawnPoly.Add(drawn);
                }
            }
            catch (Exception)
            {
            }

            // Overlay
            if (!previewMode)
            {
                int pCount = previewPoints.Count;
                for (int poly = 0; poly < pCount; poly++)
                {
                    if (drawnPoly[poly])
                    {
                        continue;
                    }

                    int ptCount = previewPoints[poly].Length;
#if !VARIANCESINGLETHREADED
                    var poly1 = poly;
                    Parallel.For(0, ptCount, pt =>
#else
                            for (int pt = 0; pt < ptCount; pt++)
#endif
                        {
                            previewPoints[poly1][pt].X += xOverlayVal;
                            previewPoints[poly1][pt].Y += yOverlayVal;
                        }
#if !VARIANCESINGLETHREADED
                    );
#endif
                }
            }

            // Biasing and CDU thanks to clipperLib
            // Note that we have to guard against a number of situations here
            // We do not want to re-bias contoured geoCore data - it's been done already.
            // Additionally, we don't want to assume an overlap for processing where none exists : we'll get back an empty polygon.
            double globalBias_Sides = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.sBias));
            globalBias_Sides += chaosSettings.getValue(ChaosSettings.properties.CDUSVar, settingsIndex) * Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.sCDU)) / 2;
            List<GeoLibPointF[]> resizedLayoutData = new();
            try
            {
                if (globalBias_Sides > double.Epsilon)
                {
                    List<bool> new_Drawn = new();

                    int pCount = previewPoints.Count;
                    for (int poly = 0; poly < pCount; poly++)
                    {
                        // Need to iterate across all polygons and only bias in this manner either:
                        // non-contoured mode
                        // contoured, but non-orthogonal polygons.
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0 ||
                            !geoCoreOrthogonalPoly[poly] && entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1)
                        {
                            Paths resizedPolyData;
                            Path gdsPointData = GeoWrangler.pathFromPointF(previewPoints[poly], CentralProperties.scaleFactorForOperation);
                            ClipperOffset co = new() {PreserveCollinear = true, ReverseSolution = true};
                            co.AddPath(gdsPointData, JoinType.Miter, EndType.Polygon);
                            resizedPolyData = co.Execute(Convert.ToDouble(globalBias_Sides * CentralProperties.scaleFactorForOperation));
                            resizedPolyData = GeoWrangler.reOrderXY(resizedPolyData);
                            resizedPolyData = GeoWrangler.close(resizedPolyData);

                            // Store our polygon data (note that we could have ended up with two or more polygons due to reduction)
                            try
                            {
                                foreach (GeoLibPointF[] rPolyData in resizedPolyData.Select(t => GeoWrangler.pointFFromPath(t, CentralProperties.scaleFactorForOperation)))
                                {
                                    resizedLayoutData.Add(rPolyData);

                                    // We need to track our drawn state as we could have a polygon count change.
                                    new_Drawn.Add(drawnPoly[poly]);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            new_Drawn.Add(drawnPoly[poly]);
                        }

                        // In case of contoured mode, with orthogonal polygon, we need to store this:
                        if (geoCoreOrthogonalPoly[poly] && entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1)
                        {
                            // Decouple out of paranoia.
                            resizedLayoutData.Add(previewPoints[poly]);
                        }
                    }

                    previewPoints = resizedLayoutData;
                    drawnPoly = new_Drawn;
                }
            }
            catch (Exception)
            {

            }

            if (process_overlaps)
            {
                double customSizing = 0;

                if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) ==
                    (int) CentralProperties.shapeNames.GEOCORE)
                {
                    customSizing = GeoWrangler.keyhole_sizing * Convert.ToDouble(commonVars.getLayerSettings(settingsIndex)
                        .getDecimal(EntropyLayerSettings.properties_decimal.keyhole_factor));
                }

                double resolution = commonVars.getSimulationSettings().getResolution();
                double extension = Convert.ToDouble(commonVars.getLayerSettings(settingsIndex)
                    .getDecimal(EntropyLayerSettings.properties_decimal.rayExtension));
                
                GeometryResult ret = ProcessOverlaps.processOverlaps(previewPoints, drawnPoly, extension:extension , resolution:resolution, scaleFactorForOperation:CentralProperties.scaleFactorForOperation, customSizing:customSizing, forceOverride: false, (FillRule)commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.fill));

                previewPoints = ret.geometry.ToList();
                drawnPoly = ret.drawn.ToList();
            }
        }
        else
        {
            // Drawn polygons only.
            // Needed to take this approach, otherwise fileData gets tied to the previewPoints list and things go wrong quickly.
            // .ToList() was insufficient to avoid the link.
            
            // Decouple the geometry here to avoid manipulation going back to original source.
            switch (tileHandlingNeeded)
            {
                case true:
                    List<GeoLibPointF[]> tempPolyList = commonVars.getNonSimulationSettings().extractedTile[settingsIndex].ToList();
                    foreach (GeoLibPointF[] t in tempPolyList)
                    {
                        previewPoints.Add(GeoWrangler.close(t));
                        drawnPoly.Add(true);
                    }
                    break;
                default:
                    for (int poly = 0; poly < entropyLayerSettings.getFileData().Count; poly++)
                    {
                        int arraySize = entropyLayerSettings.getFileData()[poly].Length;
                        GeoLibPointF[] tmp = new GeoLibPointF[arraySize];
#if !VARIANCESINGLETHREADED
                        var poly1 = poly;
                        Parallel.For(0, arraySize, pt => 
#else
                    for (Int32 pt = 0; pt < arraySize; pt++)
#endif
                            {
                                tmp[pt] = new GeoLibPointF(entropyLayerSettings.getFileData()[poly1][pt].X + xOffset,
                                    entropyLayerSettings.getFileData()[poly1][pt].Y + yOffset);
                            }
#if !VARIANCESINGLETHREADED
                        );
#endif
                        previewPoints.Add(tmp);
                        drawnPoly.Add(true);
                    }
                    break;
            }

        }
    }

    private void init_boolean(CommonVars commonVars, ChaosSettings chaosSettings, int settingsIndex, int subShapeIndex, int mode, bool doPASearch, bool previewMode, int currentRow, int currentCol, EntropyLayerSettings entropyLayerSettings)
    {
        // Get our two layers' geometry. Avoid keyholes in the process.
        int layerAIndex = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerA);
        if (settingsIndex == layerAIndex || layerAIndex < 0)
        {
            return;
        }
        EntropyLayerSettings layerA = commonVars.getLayerSettings(layerAIndex);
        PreviewShape a_pShape = new(commonVars, layerAIndex, layerA.getInt(EntropyLayerSettings.properties_i.subShapeIndex), mode: 1, doPASearch, previewMode, currentRow, currentCol);

        int layerBIndex = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerB);
        if (settingsIndex == layerBIndex || layerBIndex < 0)
        {
            return;
        }
        EntropyLayerSettings layerB = commonVars.getLayerSettings(layerBIndex);
        PreviewShape b_pShape = new(commonVars, layerBIndex, layerB.getInt(EntropyLayerSettings.properties_i.subShapeIndex), mode: 1, doPASearch, previewMode, currentRow, currentCol);

        // We need to map the geometry into Paths for use in the Boolean
        Paths layerAPaths = GeoWrangler.pathsFromPointFs(a_pShape.getPoints(), CentralProperties.scaleFactorForOperation);
        Paths layerBPaths = GeoWrangler.pathsFromPointFs(b_pShape.getPoints(), CentralProperties.scaleFactorForOperation);

        
        // Now this gets interesting. We leverage the Boolean engine in GeoWrangler to get the result we want.
        // This should probably be relocated at some point, but for now, it's an odd interaction.
        Paths booleanPaths = GeoWrangler.customBoolean(
            firstLayerOperator: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerOpA),
            firstLayer: layerAPaths, 
            secondLayerOperator: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerOpB), 
            secondLayer: layerBPaths, 
            booleanFlag: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerOpAB),
            resolution: commonVars.getSimulationSettings().getResolution(),
            extension: Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.rayExtension)), CentralProperties.scaleFactorForOperation
            // fragmenter:new Fragmenter(fragment, CentralProperties.scaleFactorForOperation)
        );

        // This is set later, if needed, to force an early return from the overlap processing path.
        int bpCount = booleanPaths.Count;
#if !VARIANCESINGLETHREADED
        Parallel.For(0, bpCount, i => 
#else
            for (int i = 0; i < bpCount; i++)
#endif
            {
                try
                {
                    booleanPaths[i] = GeoWrangler.close(booleanPaths[i]);
                }
                catch (Exception)
                {

                }
            }
#if !VARIANCESINGLETHREADED
        );
#endif
        // Scale back down again.
        List<GeoLibPointF[]> booleanGeo = GeoWrangler.pointFsFromPaths(booleanPaths, CentralProperties.scaleFactorForOperation);

        // Process the geometry according to mode, etc.
        // We do this by treating our geometry as a geocore source and calling init with this to set up our instance properties.
        // Feels a little hacky, but it ought to work.
        EntropyLayerSettings tempSettings = new();
        tempSettings.adjustSettings(entropyLayerSettings, gdsOnly: false);
        tempSettings.setInt(EntropyLayerSettings.properties_i.shapeIndex, (int)CentralProperties.shapeNames.GEOCORE);
        tempSettings.setInt(EntropyLayerSettings.properties_i.gCSEngine, 1);
        tempSettings.setFileData(booleanGeo.ToList());
        drawnPoly.Clear();
        previewPoints.Clear();
        init(commonVars, chaosSettings, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol, tempSettings, doClockwiseGeoFix: true, process_overlaps: false); // Avoid the baked-in point order reprocessing which breaks our representation.

        double customSizing = 0;

        if (commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.shapeIndex) ==
            (int) CentralProperties.shapeNames.GEOCORE)
        {
            customSizing = GeoWrangler.keyhole_sizing * Convert.ToDouble(commonVars.getLayerSettings(settingsIndex)
                .getDecimal(EntropyLayerSettings.properties_decimal.keyhole_factor));
        }
        double resolution = commonVars.getSimulationSettings().getResolution();
        double extension = Convert.ToDouble(commonVars.getLayerSettings(settingsIndex)
            .getDecimal(EntropyLayerSettings.properties_decimal.rayExtension));
                
        GeometryResult ret = ProcessOverlaps.processOverlaps(previewPoints, drawnPoly, extension:extension , resolution:resolution, scaleFactorForOperation:CentralProperties.scaleFactorForOperation, customSizing:customSizing,  forceOverride:false, (FillRule)commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.fill));

        previewPoints = ret.geometry.ToList();
        drawnPoly = ret.drawn.ToList();
    }
}