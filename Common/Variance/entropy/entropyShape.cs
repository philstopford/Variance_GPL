using System;
using System.Collections.Generic;
using System.Linq;
using Clipper2Lib;
using Error;
using geoWrangler;
using shapeEngine;

namespace Variance;

public class EntropyShape
{
    private PathD points;

    public PathD getPoints()
    {
        return pGetPoints();
    }

    private PathD pGetPoints()
    {
        return points;
    }

    private PointD pivot = new PointD(double.NaN, double.NaN);
    
    private PathD preFlight(PathD mcPoints, EntropyLayerSettings entropyLayerSettings, double resolution)
    {
        // Fragment by resolution
        Fragmenter fragment = new Fragmenter(resolution);
        PathD newMCPoints = fragment.fragmentPath(mcPoints);

        bool H = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.flipH) == 1;
        bool V = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.flipV) == 1;
        bool alignX = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.alignX) == 1;
        bool alignY = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.alignY) == 1;

        pivot = GeoWrangler.midPoint(newMCPoints);

        newMCPoints = GeoWrangler.flip(H, V, alignX, alignY, pivot, newMCPoints);

        PathD tempList = new();
        // Now to start the re-indexing.
        for (int pt = 0; pt < newMCPoints.Count; pt++)
        {
            bool addPoint;
            if (pt == 0)
            {
                addPoint = true;
            }
            else
            {
                addPoint = !(Math.Abs(tempList[^1].x - newMCPoints[pt].x) < Constants.tolerance && Math.Abs(tempList[^1].y - newMCPoints[pt].y) < Constants.tolerance);
            }

            // Avoid adding duplicate vertices
            if (addPoint)
            {
                tempList.Add(new (newMCPoints[pt].x, newMCPoints[pt].y));
            }
        }

        return tempList;
    }

    public EntropyShape(EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, int settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, PointD pivot, ShapeLibrary shape = null)
    {
        makeEntropyShape(entropySettings, entropyLayerSettingsList, settingsIndex, doPASearch, previewMode, chaosSettings, pivot, shape);
    }

    private PathD makeShape(bool returnEarly, bool cornerCheck, EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, int settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, ShapeLibrary shape)
    {
        // Define our biases. We will use these later.
        double sideBiasVar = chaosSettings.getValue(ChaosSettings.Properties.CDUSVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.sCDU)) / 2;

        PathD mcPoints = new(); // overall points container. We'll use this to populate and send back our Point array later. Int only...

        double vTipBiasVar = chaosSettings.getValue(ChaosSettings.Properties.vTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.vTPVar));
        double vTipBiasType = chaosSettings.getValue(ChaosSettings.Properties.vTipBiasType, settingsIndex);
        if (vTipBiasType < 0.5)
        {
            vTipBiasVar = -chaosSettings.getValue(ChaosSettings.Properties.vTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.vTNVar));
        }

        double hTipBiasVar = chaosSettings.getValue(ChaosSettings.Properties.hTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.hTPVar));

        double hTipBiasType = chaosSettings.getValue(ChaosSettings.Properties.hTipBiasType, settingsIndex);
        if (hTipBiasType < 0.5)
        {
            hTipBiasVar = -chaosSettings.getValue(ChaosSettings.Properties.hTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.hTNVar));
        }

        shape.computeCage(vTipBiasVar, hTipBiasVar, sideBiasVar);
        
        if (returnEarly)
        {
            mcPoints.Clear();
            mcPoints.AddRange(shape.Vertex.Select(t => new PointD(t.X, t.Y)));
            return mcPoints;
        }
        
        double iCV = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.iCV));
        double oCV = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.oCV));
        double iCVariation = chaosSettings.getValue(ChaosSettings.Properties.icVar, settingsIndex);
        double oCVariation = chaosSettings.getValue(ChaosSettings.Properties.ocVar, settingsIndex);
        int cornerSegments = entropySettings.getValue(EntropySettings.properties_i.cSeg);
        int optimizeCorners = entropySettings.getValue(EntropySettings.properties_i.optC);
        double resolution = entropySettings.getResolution();
        bool icPA = chaosSettings.getBool(ChaosSettings.Bools.icPA, settingsIndex);
        bool ocPA = chaosSettings.getBool(ChaosSettings.Bools.ocPA, settingsIndex);

        mcPoints = shape.processCorners(previewMode,cornerCheck, cornerSegments, optimizeCorners, resolution, icPA, ocPA,
            iCV, iCVariation, oCV, oCVariation);

        return mcPoints;

    }
    
    private void makeEntropyShape(EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, int settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, PointD pivot_, ShapeLibrary shape = null)
    {
        bool geoCoreShapeDefined = shape != null;
        bool cornerCheck = false;
        bool returnEarly = false;

        if ( !double.IsNaN(pivot_.x) && !double.IsNaN(pivot_.y) )
        {
            pivot = new (pivot_.x, pivot_.y);
        }
        
        if (shape == null)
        {
            shape = new ShapeLibrary(CentralProperties.shapeTable, entropyLayerSettingsList[settingsIndex]);
            shape.setShape(entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.shapeIndex));
        }

        // Tip wrangling and shape closure will happen next
        bool failSafe = !shape.shapeValid || entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.enabled) == 0; // Set failsafe if shape is invalid.

        PathD mcPoints = new(); // overall points container. We'll use this to populate and send back our Point array later. Ints only...

        // Handle non-orthogonal case.
        if (!failSafe)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!geoCoreShapeDefined || geoCoreShapeDefined && shape.geoCoreShapeOrthogonal)
            {
                // ReSharper disable twice ConditionIsAlwaysTrueOrFalse
                mcPoints = makeShape(returnEarly, cornerCheck, entropySettings, entropyLayerSettingsList, settingsIndex, doPASearch, previewMode, chaosSettings, shape);
            }
            else
            {
                // We have a non-orthogonal geoCore shape, so we take the defined vertices and use them directly. No rounding or tips (tips might be doable at a later date).
                mcPoints.AddRange(shape.Vertex.Select(t => new PointD (t.X, t.Y)));
            }
            if (returnEarly || cornerCheck)
            {
                points = new (mcPoints);
                return;
            }
        }

        // Sort out our overlay values.
        if (!previewMode)
        {
            double xOverlayVal = chaosSettings.getValue(ChaosSettings.Properties.overlayX, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.xOL));

            if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.xOL_av) == 1) // overlay average
            {
                List<double> overlayValues = new();
                for (int avgolref_x = 0; avgolref_x < entropyLayerSettingsList[settingsIndex].getIntArray(EntropyLayerSettings.properties_intarray.xOLRefs).Length; avgolref_x++)
                {
                    if (entropyLayerSettingsList[settingsIndex].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, avgolref_x) == 1)
                    {
                        overlayValues.Add(chaosSettings.getValue(ChaosSettings.Properties.overlayX, avgolref_x) * Convert.ToDouble(entropyLayerSettingsList[avgolref_x].getDecimal(EntropyLayerSettings.properties_decimal.xOL))); // Overlay shift
                    }
                }

                try
                {
                    xOverlayVal += overlayValues.Average();
                }
                catch (Exception)
                {
                }
            }
            else // vanilla overlay reference mode
            {
                if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.xOL_ref) != -1)
                {
                    xOverlayVal += chaosSettings.getValue(ChaosSettings.Properties.overlayX, entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.xOL_ref)) * Convert.ToDouble(entropyLayerSettingsList[entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.xOL_ref)].getDecimal(EntropyLayerSettings.properties_decimal.xOL)); // Overlay shift
                }
            }

            double yOverlayVal = chaosSettings.getValue(ChaosSettings.Properties.overlayY, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.yOL)); // Overlay shift

            if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.yOL_av) == 1) // overlay average
            {
                List<double> overlayValues = new();
                for (int avgolref_y = 0; avgolref_y < entropyLayerSettingsList[settingsIndex].getIntArray(EntropyLayerSettings.properties_intarray.yOLRefs).Length; avgolref_y++)
                {
                    if (entropyLayerSettingsList[settingsIndex].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, avgolref_y) == 1)
                    {
                        overlayValues.Add(chaosSettings.getValue(ChaosSettings.Properties.overlayY, avgolref_y) * Convert.ToDouble(entropyLayerSettingsList[avgolref_y].getDecimal(EntropyLayerSettings.properties_decimal.yOL))); // Overlay shift
                    }
                }

                try
                {
                    yOverlayVal += overlayValues.Average();
                }
                catch (Exception)
                {

                }
            }
            else // vanilla overlay reference mode
            {
                if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.yOL_ref) != -1)
                {
                    yOverlayVal += chaosSettings.getValue(ChaosSettings.Properties.overlayY, entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.yOL_ref)) * Convert.ToDouble(entropyLayerSettingsList[entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.yOL_ref)].getDecimal(EntropyLayerSettings.properties_decimal.yOL)); // Overlay shift
                }
            }
            mcPoints = GeoWrangler.move(mcPoints, xOverlayVal, yOverlayVal);
        }

        
        double threshold = 1E-5; // threshold for proximity, to avoid normal extraction issues.
        if (mcPoints.Count > 1)
        {
            // Need to clean up any duplicate points at this point, to avoid causing /0 issues below.
            PathD newPoints = GeoWrangler.removeDuplicates(mcPoints, threshold);
            if (newPoints.Count != 0)
            {
                // Close shape.
                mcPoints = GeoWrangler.close(newPoints);
            }
        }
        double rotationVar = chaosSettings.getValue(ChaosSettings.Properties.wobbleVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.wobble));
        // Per-poly rotation for layout case (only scenario where we have polygon sheets at this level)
        // Note that we don't have CSV tracking for this case - there's no practical way to record the random values here.
        if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.perPoly) == 1 && entropyLayerSettingsList[settingsIndex].getInt(ShapeSettings.properties_i.shapeIndex) == (int)CentralProperties.shapeNames.GEOCORE)
        {
            rotationVar = UtilityFuncs.getGaussianRandomNumber3(entropySettings) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.wobble));
        }
        double rotationDirection = UtilityFuncs.getGaussianRandomNumber3(entropySettings);

        if (double.IsNaN(pivot.x) || double.IsNaN(pivot.y))
        {
            pivot = GeoWrangler.midPoint(mcPoints);
        }
        
        ShapeLibrary.RotateOutput ret = shape.rotateShape(mcPoints, entropyLayerSettingsList[settingsIndex], rotationVar, rotationDirection, pivot);

        mcPoints = ret.output;
        entropyLayerSettingsList[settingsIndex].setDecimal(EntropyLayerSettings.properties_decimal.totalRotation, (decimal)ret.totalRotation, 0);

        // Error handling (failSafe) for no points or no subshape  - safety measure.
        if (!mcPoints.Any())
        {
            mcPoints.Add(new (0.0f, 0.0f));
        }

        // Path direction, point order and re-fragmentation (as needed)
        points = new (preFlight(mcPoints, entropyLayerSettingsList[settingsIndex], entropySettings.getResolution()));
        
        if (Math.Abs(points[0].x - points[^1].x) > Constants.tolerance || Math.Abs(points[0].y - points[^1].y) > Constants.tolerance)
        {
            ErrorReporter.showMessage_OK("Start and end not the same - entropyShape", "Oops");
        }
    }
}