using System;
using System.Collections.Generic;
using System.Linq;
using Error;
using geoLib;
using geoWrangler;
using shapeEngine;

namespace Variance;

public class EntropyShape
{
    private GeoLibPointF[] points;

    public GeoLibPointF[] getPoints()
    {
        return pGetPoints();
    }

    private GeoLibPointF[] pGetPoints()
    {
        return points;
    }

    private GeoLibPointF pivot;
    
    private List<GeoLibPointF> preFlight(List<GeoLibPointF> mcPoints, EntropyLayerSettings entropyLayerSettings, double resolution, int scaleFactorForOperation)
    {
        // Fragment by resolution
        Fragmenter fragment = new Fragmenter(resolution, scaleFactorForOperation);
        List<GeoLibPointF> newMCPoints = fragment.fragmentPath(mcPoints);

        bool H = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.flipH) == 1;
        bool V = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.flipV) == 1;
        bool alignX = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.alignX) == 1;
        bool alignY = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.alignY) == 1;

        newMCPoints = GeoWrangler.flip(H, V, alignX, alignY, pivot, newMCPoints);

        List<GeoLibPointF> tempList = new();
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
                addPoint = !(Math.Abs(tempList[^1].X - newMCPoints[pt].X) < double.Epsilon && Math.Abs(tempList[^1].Y - newMCPoints[pt].Y) < double.Epsilon);
            }

            // Avoid adding duplicate vertices
            if (addPoint)
            {
                tempList.Add(new GeoLibPointF(newMCPoints[pt].X, newMCPoints[pt].Y));
            }
        }

        return tempList;
    }

    public EntropyShape(EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, int settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, ShapeLibrary shape = null, GeoLibPointF pivot = null)
    {
        makeEntropyShape(entropySettings, entropyLayerSettingsList, settingsIndex, doPASearch, previewMode, chaosSettings, shape, pivot);
    }

    private List<GeoLibPointF> makeShape(bool returnEarly, bool cornerCheck, EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, int settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, ShapeLibrary shape)
    {
        // Define our biases. We will use these later.
        double globalBias_Sides = Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.sBias));
        globalBias_Sides += chaosSettings.getValue(ChaosSettings.properties.CDUSVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.sCDU)) / 2;
        double globalBias_Tips = chaosSettings.getValue(ChaosSettings.properties.CDUTVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.tCDU)) / 2;

        List<GeoLibPointF> mcPoints = new(); // overall points container. We'll use this to populate and send back our Point array later. Int only...

        double vTipBias = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.vTBias));
        double vTipBiasType = chaosSettings.getValue(ChaosSettings.properties.vTipBiasType, settingsIndex);
        double vTipBiasNegVar = chaosSettings.getValue(ChaosSettings.properties.vTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.vTNVar));
        double vTipBiasPosVar = chaosSettings.getValue(ChaosSettings.properties.vTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.vTPVar));

        double hTipBias = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.hTBias));
        double hTipBiasType = chaosSettings.getValue(ChaosSettings.properties.hTipBiasType, settingsIndex);
        double hTipBiasNegVar = chaosSettings.getValue(ChaosSettings.properties.hTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.hTNVar));
        double hTipBiasPosVar = chaosSettings.getValue(ChaosSettings.properties.hTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.hTPVar));

        shape.computeTips(globalBias_Tips, hTipBias,hTipBiasType, hTipBiasNegVar, hTipBiasPosVar, vTipBias, vTipBiasType, vTipBiasNegVar, vTipBiasPosVar);

        shape.computeBias(globalBias_Sides);

        shape.biasCorners();

        int edgeSlide = (entropyLayerSettingsList[settingsIndex].getInt(ShapeSettings.properties_i.edgeSlide));
        double eTension = Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(ShapeSettings.properties_decimal.eTension));
        shape.edgeMidpoints(edgeSlide, eTension);

        if (returnEarly)
        {
            mcPoints.Clear();
            mcPoints.AddRange(shape.Vertex.Select(t => new GeoLibPointF(t.X, t.Y)));
            return mcPoints;
        }

        double iCR = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.iCR));
        double oCR = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.oCR));
        double iCV = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.iCV));
        double oCV = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.oCV));
        double iCVariation = chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex);
        double oCVariation = chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex);
        int cornerSegments = entropySettings.getValue(EntropySettings.properties_i.cSeg);
        int optimizeCorners = entropySettings.getValue(EntropySettings.properties_i.optC);
        double resolution = entropySettings.getResolution();
        bool icPA = chaosSettings.getBool(ChaosSettings.bools.icPA, settingsIndex);
        bool ocPA = chaosSettings.getBool(ChaosSettings.bools.ocPA, settingsIndex);
        double s0HO = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 0));
        double s0VO = Convert.ToDouble(entropyLayerSettingsList[settingsIndex]
            .getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 0));
        
        mcPoints = shape.processCorners(previewMode, cornerCheck, doPASearch, s0HO, s0VO, iCR, iCV, iCVariation, icPA, oCR, oCV,
            oCVariation, ocPA, cornerSegments, optimizeCorners, resolution, CentralProperties.scaleFactorForOperation);

        return mcPoints;

    }
    
    private void makeEntropyShape(EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, int settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, ShapeLibrary shape = null, GeoLibPointF pivot_ = null)
    {
        bool geoCoreShapeDefined = shape != null;
        bool cornerCheck = false;
        bool returnEarly = false;

        if (pivot_ != null)
        {
            pivot = new GeoLibPointF(pivot_.X, pivot_.Y);
        }
        
        if (shape == null)
        {
            shape = new ShapeLibrary(CentralProperties.shapeTable, entropyLayerSettingsList[settingsIndex]);
            shape.setShape(entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.shapeIndex));
        }

        // Tip wrangling and shape closure will happen next
        bool failSafe = !shape.shapeValid || entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.enabled) == 0; // Set failsafe if shape is invalid.

        List<GeoLibPointF> mcPoints = new(); // overall points container. We'll use this to populate and send back our Point array later. Ints only...

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
                mcPoints.AddRange(shape.Vertex.Select(t => new GeoLibPointF(t.X, t.Y)));
            }
            if (returnEarly || cornerCheck)
            {
                points = mcPoints.ToArray();
                return;
            }
        }

        // Sort out our overlay values.
        if (!previewMode)
        {
            double xOverlayVal = chaosSettings.getValue(ChaosSettings.properties.overlayX, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.xOL));

            if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.xOL_av) == 1) // overlay average
            {
                List<double> overlayValues = new();
                for (int avgolref_x = 0; avgolref_x < entropyLayerSettingsList[settingsIndex].getIntArray(EntropyLayerSettings.properties_intarray.xOLRefs).Length; avgolref_x++)
                {
                    if (entropyLayerSettingsList[settingsIndex].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, avgolref_x) == 1)
                    {
                        overlayValues.Add(chaosSettings.getValue(ChaosSettings.properties.overlayX, avgolref_x) * Convert.ToDouble(entropyLayerSettingsList[avgolref_x].getDecimal(EntropyLayerSettings.properties_decimal.xOL))); // Overlay shift
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
                    xOverlayVal += chaosSettings.getValue(ChaosSettings.properties.overlayX, entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.xOL_ref)) * Convert.ToDouble(entropyLayerSettingsList[entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.xOL_ref)].getDecimal(EntropyLayerSettings.properties_decimal.xOL)); // Overlay shift
                }
            }

            double yOverlayVal = chaosSettings.getValue(ChaosSettings.properties.overlayY, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.yOL)); // Overlay shift

            if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.yOL_av) == 1) // overlay average
            {
                List<double> overlayValues = new();
                for (int avgolref_y = 0; avgolref_y < entropyLayerSettingsList[settingsIndex].getIntArray(EntropyLayerSettings.properties_intarray.yOLRefs).Length; avgolref_y++)
                {
                    if (entropyLayerSettingsList[settingsIndex].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, avgolref_y) == 1)
                    {
                        overlayValues.Add(chaosSettings.getValue(ChaosSettings.properties.overlayY, avgolref_y) * Convert.ToDouble(entropyLayerSettingsList[avgolref_y].getDecimal(EntropyLayerSettings.properties_decimal.yOL))); // Overlay shift
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
                    yOverlayVal += chaosSettings.getValue(ChaosSettings.properties.overlayY, entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.yOL_ref)) * Convert.ToDouble(entropyLayerSettingsList[entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.yOL_ref)].getDecimal(EntropyLayerSettings.properties_decimal.yOL)); // Overlay shift
                }
            }
            mcPoints = GeoWrangler.move(mcPoints, xOverlayVal, yOverlayVal);
        }

        
        double threshold = 1E-5; // threshold for proximity, to avoid normal extraction issues.
        if (mcPoints.Count > 1)
        {
            // Need to clean up any duplicate points at this point, to avoid causing /0 issues below.
            List<GeoLibPointF> newPoints = GeoWrangler.removeDuplicates(mcPoints, threshold);
            if (newPoints.Count != 0)
            {
                // Close shape.
                mcPoints = GeoWrangler.close(newPoints);
            }
        }
        double rotationVar = chaosSettings.getValue(ChaosSettings.properties.wobbleVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.wobble));
        // Per-poly rotation for layout case (only scenario where we have polygon sheets at this level)
        // Note that we don't have CSV tracking for this case - there's no practical way to record the random values here.
        if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.perPoly) == 1 && entropyLayerSettingsList[settingsIndex].getInt(ShapeSettings.properties_i.shapeIndex) == (int)CentralProperties.shapeNames.GEOCORE)
        {
            rotationVar = UtilityFuncs.getGaussianRandomNumber3(entropySettings) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.wobble));
        }
        double rotationDirection = UtilityFuncs.getGaussianRandomNumber3(entropySettings);

        mcPoints = shape.rotateShape(mcPoints, entropyLayerSettingsList[settingsIndex], rotationVar, rotationDirection, pivot);

        // Error handling (failSafe) for no points or no subshape  - safety measure.
        if (!mcPoints.Any())
        {
            mcPoints.Add(new GeoLibPointF(0.0f, 0.0f));
        }

        // Path direction, point order and re-fragmentation (as needed)
        points = preFlight(mcPoints, entropyLayerSettingsList[settingsIndex], entropySettings.getResolution(), CentralProperties.scaleFactorForOperation).ToArray();
        
        if (Math.Abs(points[0].X - points[^1].X) > double.Epsilon || Math.Abs(points[0].Y - points[^1].Y) > double.Epsilon)
        {
            ErrorReporter.showMessage_OK("Start and end not the same - entropyShape", "Oops");
        }
    }
}