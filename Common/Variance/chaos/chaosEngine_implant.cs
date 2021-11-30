using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ClipperLib;
using geoLib;
using geoWrangler;
using utility;

namespace Variance;

using Path = List<IntPoint>;
using Paths = List<List<IntPoint>>;

internal class ChaosEngine_implant
{
    private string result, min, max;
    public string getResult()
    {
        return pGetResult();
    }

    private string pGetResult()
    {
        return result;
    }

    public string getMin()
    {
        return pGetMin();
    }

    private string pGetMin()
    {
        return min;
    }

    public string getMax()
    {
        return pGetMax();
    }

    private string pGetMax()
    {
        return max;
    }

    private bool outputValid;
    public bool isValid()
    {
        return pIsValid();
    }

    private bool pIsValid()
    {
        return outputValid;
    }

    // Output geometry.
    private GeoLibPointF[] geom, bgGeom;
    public GeoLibPointF[] getGeom()
    {
        return pGetGeom();
    }

    private GeoLibPointF[] pGetGeom()
    {
        return geom;
    }

    public GeoLibPointF[] getBGGeom()
    {
        return pGetBGGeom();
    }

    private GeoLibPointF[] pGetBGGeom()
    {
        return bgGeom;
    }

    private GeoLibPointF[] shadow, minShadow, maxShadow;
    public GeoLibPointF[] getShadow()
    {
        return pGetShadow();
    }

    private GeoLibPointF[] pGetShadow()
    {
        return shadow;
    }

    public GeoLibPointF[] getMinShadow()
    {
        return pGetMinShadow();
    }

    private GeoLibPointF[] pGetMinShadow()
    {
        return minShadow;
    }

    public GeoLibPointF[] getMaxShadow()
    {
        return pGetMaxShadow();
    }

    private GeoLibPointF[] pGetMaxShadow()
    {
        return maxShadow;
    }

    public ChaosEngine_implant(ChaosSettings_implant chaosSettings, EntropySettings entropySettings, EntropyImplantSettings implantCalcSettings)
    {
        pChaosEngine_implant(chaosSettings, entropySettings, implantCalcSettings);
    }

    private void pChaosEngine_implant(ChaosSettings_implant chaosSettings, EntropySettings entropySettings, EntropyImplantSettings implantCalcSettings)
    {
        outputValid = false;

        try
        {
            List<EntropyLayerSettings> implant_MLS = new();
            EntropyLayerSettings mls = new();
            mls.setInt(EntropyLayerSettings.properties_i.edgeSlide, 0); // we don't want the edge slide in this situation.
            mls.setInt(EntropyLayerSettings.properties_i.shapeIndex, (int)CommonVars.shapeNames.rect);
            double resistWidth = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.w) + implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.wV) * chaosSettings.getValue(ChaosSettings_implant.properties.resistCDVar);
            mls.setDecimal(EntropyLayerSettings.properties_decimal.s0HorLength, Convert.ToDecimal(resistWidth));
            double resistHeight = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.h) + implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.hV) * chaosSettings.getValue(ChaosSettings_implant.properties.resistHeightVar);
            mls.setDecimal(EntropyLayerSettings.properties_decimal.s0VerLength, Convert.ToDecimal(resistHeight) * 2.0m); // double since we're making an ellipse and clipping later.
            mls.setDecimal(EntropyLayerSettings.properties_decimal.oCR, Convert.ToDecimal(implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.cRR) + implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.cV) * chaosSettings.getValue(ChaosSettings_implant.properties.resistTopCRRVar)));
            mls.setInt(EntropyLayerSettings.properties_i.posIndex, (int)CommonVars.subShapeLocations.C);
            mls.setInt(EntropyLayerSettings.properties_i.enabled, 1);
            implant_MLS.Add(mls);

            // To permit random variations at a later date.
            // We take the existing simulation settings to make the shape engine, etc. more straightforward.
            ChaosSettings implant_js = new(true, implant_MLS, entropySettings);

            // Use our shape engine to create a nice ellipse.
            EntropyShape ms = new(entropySettings, implant_MLS, settingsIndex: 0, doPASearch: false, previewMode: true, implant_js);

            // Set up our sourcePath for clipping, re-centering it at 0,0 as well.
            Path sourcePath = new();
            for (int pt = 0; pt < ms.getPoints().Length; pt++)
            {
                double x = ms.getPoints()[pt].X - Convert.ToDouble(mls.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength)) / 2.0f;
                double y = ms.getPoints()[pt].Y - resistHeight;
                sourcePath.Add(new IntPoint((long)(x * CentralProperties.scaleFactorForOperation),
                    (long)(y * CentralProperties.scaleFactorForOperation)));
            }

            Paths source = new() {sourcePath};

            // Build our mask polygon from the bounds and 0,0 reference. Curiously, Clipper's top/bottom bounds are flipped from what might be expected.
            IntRect bounds = ClipperBase.GetBounds(source);
            Path maskPoly = new()
            {
                new IntPoint(0, 0),
                new IntPoint(0, bounds.bottom),
                new IntPoint(bounds.right, bounds.bottom),
                new IntPoint(bounds.right, 0),
                new IntPoint(0, 0)
            };

            // Get our region extracted using the mask.
            Clipper c = new() {PreserveCollinear = false};
            c.AddPath(sourcePath, PolyType.ptSubject, true);
            c.AddPath(maskPoly, PolyType.ptClip, true);
            Paths solution = new();
            c.Execute(ClipType.ctIntersection, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            if (solution.Count == 0)
            {
                outputValid = false;
                return;
            }
            // We only ever have one path in the solution.
            // Re-order the points by finding our first (0,0) coordinate.
            int reIndexStart = 0;
            for (int pt = 0; pt < solution[0].Count; pt++)
            {
                if (solution[0][pt].X != 0 || solution[0][pt].Y != 0)
                {
                    continue;
                }

                reIndexStart = pt;
                break;
            }

            // Build our point array, re-indexing if needed.
            GeoLibPoint[] points = new GeoLibPoint[solution[0].Count + 1];
            if (reIndexStart != 0)
            {
                int index = 0; // just to track our position in the points array.
                for (int pt = reIndexStart; pt < solution[0].Count; pt++)
                {
                    points[index] = new GeoLibPoint(solution[0][pt].X, solution[0][pt].Y);
                    index++;
                }
                // Ensure we close the shape by hitting the reIndexStart point again, since we will possibly have pushed it to the beginning of the shape.
                for (int pt = 0; pt <= reIndexStart; pt++)
                {
                    points[index] = new GeoLibPoint(solution[0][pt].X, solution[0][pt].Y);
                    index++;
                }
            }
            else
            {
#if !CHAOSSINGLETHREADED
                Parallel.For(0, solution[0].Count, pt =>
#else
                    for (int pt = 0; pt < solution[0].Count; pt++)
#endif
                    {
                        points[pt] = new GeoLibPoint(solution[0][pt].X, solution[0][pt].Y);
                    }
#if !CHAOSSINGLETHREADED
                );
#endif
            }
            points[^1] = new GeoLibPoint(points[0].X, points[0].Y); // close it, for the sake of it.

            // Clockwise spin.
            points = GeoWrangler.clockwise(points);

            // We can now get our tangent extraction sorted out.
            int nomPtIndex = -1;
            double nomXIntercept = 0;
            double nomYIntercept = 0;

            int minPtIndex = -1;
            double minXIntercept = 0;
            double minYIntercept = 0;

            int maxPtIndex = -1;
            double maxXIntercept = 0;
            double maxYIntercept = 0
                ;
            // Reverse walk due to 0 angle shadowing occuring at the resist edge.
            for (int pt = points.Length - 3; pt > 1; pt--)
            {
                double deltaY = points[pt + 1].Y - points[pt].Y;
                double deltaX = points[pt + 1].X - points[pt].X;

                // A zero deltaX breaks the gradient calculation because it results in infinity.
                // Instead, set an arbitrary small value.
                // We also clamp for negative values as there is no prospect of negative shadowing.
                if (deltaX <= 0)
                {
                    deltaX = 1E-9;
                }

                // Flip the sign - we're walking backwards, but want the positive change. Doing it this way made the code easier to understand.
                deltaX *= -1;
                deltaY *= -1;

                // retrieve our angle for the line segment. 0 is vertical; 90 would be parallel to surface.
                // angle is negative as we're walking down the resist profile.
                double angle = Utils.toDegrees(Math.Atan(deltaY / deltaX));

                // Line parameters.
                double m = deltaY / deltaX;
                double c_ = points[pt].Y - m * points[pt].X;

                if (chaosSettings.isPreview())
                {
                    // X intersect : y = mx + c and so we need to solve for y = 0, i.e. mx = -c
                    double twistAngle = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.twist) + implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.twistV);
                    if (twistAngle > 90)
                    {
                        twistAngle = 90.0f;
                    }
                    double tiltAngle = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tilt) - implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tiltV);
                    if (tiltAngle < 0)
                    {
                        tiltAngle = 0.0f;
                    }
                    if (minPtIndex == -1 && angle + 90.0f >= Convert.ToDouble(tiltAngle) * Math.Cos(Utils.toRadians(twistAngle)))
                    {
                        minPtIndex = pt;
                        minXIntercept = -c_ / m;
                        minYIntercept = c_;
                    }

                    if (minPtIndex != -1 && nomPtIndex == -1 && angle + 90.0f >= Convert.ToDouble(implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tilt)) * Math.Cos(Utils.toRadians(implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.twist))))
                    {
                        nomPtIndex = pt;
                        nomXIntercept = -c_ / m;
                        nomYIntercept = c_;
                    }

                    tiltAngle = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tilt) + implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tiltV);
                    if (tiltAngle > 90)
                    {
                        tiltAngle = 90.0f;
                    }

                    if (nomPtIndex == -1 || maxPtIndex != -1 || !(angle + 90.0f >= Convert.ToDouble(tiltAngle) *
                            Math.Cos(Utils.toRadians(
                                implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.twist)))))
                    {
                        continue;
                    }

                    maxPtIndex = pt;
                    maxXIntercept = -c_ / m;
                    maxYIntercept = c_;
                }
                else
                {
                    double tiltAngle_3sigmaVar = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tilt) + chaosSettings.getValue(ChaosSettings_implant.properties.tiltVar) * implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tiltV);
                    if (tiltAngle_3sigmaVar < 0)
                    {
                        tiltAngle_3sigmaVar = 0.0f;
                    }
                    if (tiltAngle_3sigmaVar > 90)
                    {
                        tiltAngle_3sigmaVar = 90.0f;
                    }
                    double twistAngle_3sigmaVar = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.twist) + chaosSettings.getValue(ChaosSettings_implant.properties.twistVar) * implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.twistV);
                    if (twistAngle_3sigmaVar < 0)
                    {
                        twistAngle_3sigmaVar = 0.0f;
                    }
                    if (twistAngle_3sigmaVar > 90)
                    {
                        twistAngle_3sigmaVar = 90.0f;
                    }

                    // X intersect : y = mx + c and so we need to solve for y = 0, i.e. mx = -c
                    if (!(angle + 90.0f >=
                          Convert.ToDouble(tiltAngle_3sigmaVar) * Math.Cos(Utils.toRadians(twistAngle_3sigmaVar))))
                    {
                        continue;
                    }

                    nomXIntercept = -c_ / m;
                    nomYIntercept = c_;
                    break;
                }
            }

            // Get our shadow line (polys) sorted out.
            shadow = new GeoLibPointF[4];
            shadow[0] = new GeoLibPointF(0.0f, nomYIntercept / CentralProperties.scaleFactorForOperation);
            shadow[1] = new GeoLibPointF(nomXIntercept / CentralProperties.scaleFactorForOperation, 0.0f);
            shadow[2] = new GeoLibPointF(shadow[1].X, shadow[1].Y);
            shadow[3] = new GeoLibPointF(shadow[0].X, shadow[0].Y);

            // Calculate our shadow distance taking into account the resist width beyond the intercept point.
            double actualResistWidth = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.w) + chaosSettings.getValue(ChaosSettings_implant.properties.resistCDVar) * implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.wV);
            // Force a format here to avoid blanks in some cases.
            result = (shadow[1].X - actualResistWidth / 2.0f).ToString("#.##");
            // Extra check in case of an escape above, but shouldn't be needed.
            if (result == "")
            {
                result = "0.0";
            }

            minShadow = new GeoLibPointF[4];
            maxShadow = new GeoLibPointF[4];

            if (chaosSettings.isPreview())
            {
                minShadow[0] = new GeoLibPointF(0.0f, minYIntercept / CentralProperties.scaleFactorForOperation);
                minShadow[1] = new GeoLibPointF(minXIntercept / CentralProperties.scaleFactorForOperation, 0.0f);
                minShadow[2] = new GeoLibPointF(minShadow[1].X, minShadow[1].Y);
                minShadow[3] = new GeoLibPointF(minShadow[0].X, minShadow[0].Y);

                maxShadow[0] = new GeoLibPointF(0.0f, maxYIntercept / CentralProperties.scaleFactorForOperation);
                maxShadow[1] = new GeoLibPointF(maxXIntercept / CentralProperties.scaleFactorForOperation, 0.0f);
                maxShadow[2] = new GeoLibPointF(maxShadow[1].X, maxShadow[1].Y);
                maxShadow[3] = new GeoLibPointF(maxShadow[0].X, maxShadow[0].Y);

                min = (minShadow[1].X - actualResistWidth / 2.0f).ToString(CultureInfo.InvariantCulture);
                // Seem to get periodic blanks for 0 cases so force the issue here.
                if (min == "")
                {
                    min = "0.0";
                }
                max = (maxShadow[1].X - actualResistWidth / 2.0f).ToString(CultureInfo.InvariantCulture);
                // Seem to get periodic blanks for 0 cases so force the issue here.
                if (max == "")
                {
                    max = "0.0";
                }

                // Preview mode result push.
            }
            else
            {
                min = "N/A";
                max = "N/A";

                minShadow[0] = new GeoLibPointF(0, 0);
                minShadow[1] = new GeoLibPointF(0, 0);
                minShadow[2] = new GeoLibPointF(0, 0);
                minShadow[3] = new GeoLibPointF(0, 0);

                maxShadow[0] = new GeoLibPointF(0, 0);
                maxShadow[1] = new GeoLibPointF(0, 0);
                maxShadow[2] = new GeoLibPointF(0, 0);
                maxShadow[3] = new GeoLibPointF(0, 0);
            }

            // Now need to scale our polygon back down again for display.
            geom = new GeoLibPointF[points.Length]; // resist profile that has been evaluated for shadowing.
            bgGeom = new GeoLibPointF[points.Length]; // display the other resist side, to avoid confusing user, but this isn't evaluated in shadowing.
#if !CHAOSSINGLETHREADED
            Parallel.For(0, points.Length, pt =>
#else
                for (int pt = 0; pt < points.Length; pt++)
#endif
                {
                    double x = (double)points[pt].X / CentralProperties.scaleFactorForOperation;
                    double y = (double)points[pt].Y / CentralProperties.scaleFactorForOperation;
                    geom[pt] = new GeoLibPointF(x, y);
                    bgGeom[pt] = new GeoLibPointF(-x, y);
                }
#if !CHAOSSINGLETHREADED
            );
#endif
            outputValid = true;
        }
        catch (Exception)
        {
            outputValid = false;
        }
    }
}