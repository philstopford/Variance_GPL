using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Clipper2Lib;
using geoWrangler;
using shapeEngine;
using utility;

namespace Variance;

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
    private PathD geom, bgGeom;
    public PathD getGeom()
    {
        return pGetGeom();
    }

    private PathD pGetGeom()
    {
        return geom;
    }

    public PathD getBGGeom()
    {
        return pGetBGGeom();
    }

    private PathD pGetBGGeom()
    {
        return bgGeom;
    }

    private PathD shadow, minShadow, maxShadow;
    public PathD getShadow()
    {
        return pGetShadow();
    }

    private PathD pGetShadow()
    {
        return shadow;
    }

    public PathD getMinShadow()
    {
        return pGetMinShadow();
    }

    private PathD pGetMinShadow()
    {
        return minShadow;
    }

    public PathD getMaxShadow()
    {
        return pGetMaxShadow();
    }

    private PathD pGetMaxShadow()
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
            mls.setInt(EntropyLayerSettings.properties_i.shapeIndex, (int)CentralProperties.shapeNames.rect);
            double resistWidth = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.w) + implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.wV) * chaosSettings.getValue(ChaosSettings_implant.Properties.resistCDVar);
            mls.setDecimal(EntropyLayerSettings.properties_decimal.horLength, Convert.ToDecimal(resistWidth), 0);
            double resistHeight = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.h) + implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.hV) * chaosSettings.getValue(ChaosSettings_implant.Properties.resistHeightVar);
            mls.setDecimal(EntropyLayerSettings.properties_decimal.verLength, Convert.ToDecimal(resistHeight) * 2.0m, 0); // double since we're making an ellipse and clipping later.
            mls.setDecimal(EntropyLayerSettings.properties_decimal.oCR, Convert.ToDecimal(implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.cRR) + implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.cV) * chaosSettings.getValue(ChaosSettings_implant.Properties.resistTopCRRVar)));
            mls.setInt(EntropyLayerSettings.properties_i.posIndex, (int)ShapeSettings.subShapeLocations.C);
            mls.setInt(EntropyLayerSettings.properties_i.enabled, 1);
            implant_MLS.Add(mls);

            // To permit random variations at a later date.
            // We take the existing simulation settings to make the shape engine, etc. more straightforward.
            ChaosSettings implant_js = new(true, implant_MLS, entropySettings);

            // Use our shape engine to create a nice ellipse.
            PointD pivot_unused = new(double.NaN, double.NaN);
            EntropyShape ms = new(entropySettings, implant_MLS, settingsIndex: 0, doPASearch: false, previewMode: true, implant_js, pivot_unused);

            // Set up our sourcePath for clipping, re-centering it at 0,0 as well.
            PathD sourcePath = new();
            for (int pt = 0; pt < ms.getPoints().Count; pt++)
            {
                double x = ms.getPoints()[pt].x - Convert.ToDouble(mls.getDecimal(EntropyLayerSettings.properties_decimal.horLength, 0)) / 2.0f;
                double y = ms.getPoints()[pt].y - resistHeight;
                sourcePath.Add(new PointD(x, y));
            }

            PathsD source = new() {sourcePath};

            // Build our mask polygon from the bounds and 0,0 reference. Curiously, Clipper's top/bottom bounds are flipped from what might be expected.
            RectD bounds = Clipper.GetBounds(source);
            PathD maskPoly = new()
            {
                new (0, 0),
                new (0, bounds.bottom),
                new (bounds.right, bounds.bottom),
                new (bounds.right, 0),
                new (0, 0)
            };

            // Get our region extracted using the mask.
            ClipperD c = new() {PreserveCollinear = false};
            c.AddSubject(sourcePath);
            c.AddClip(maskPoly);
            PathsD solution = new();
            c.Execute(ClipType.Intersection, FillRule.EvenOdd, solution);
            solution = GeoWrangler.reOrderXY(solution);

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
                if (solution[0][pt].x != 0 || solution[0][pt].y != 0)
                {
                    continue;
                }

                reIndexStart = pt;
                break;
            }

            // Build our point array, re-indexing if needed.
            PathD points = Helper.initedPathD(solution[0].Count + 1);
            if (reIndexStart != 0)
            {
                int index = 0; // just to track our position in the points array.
                for (int pt = reIndexStart; pt < solution[0].Count; pt++)
                {
                    points[index] = new (solution[0][pt].x, solution[0][pt].y);
                    index++;
                }
                // Ensure we close the shape by hitting the reIndexStart point again, since we will possibly have pushed it to the beginning of the shape.
                for (int pt = 0; pt <= reIndexStart; pt++)
                {
                    points[index] = new (solution[0][pt].x, solution[0][pt].y);
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
                        points[pt] = new (solution[0][pt].x, solution[0][pt].y);
                    }
#if !CHAOSSINGLETHREADED
                );
#endif
            }
            points[^1] = new (points[0].x, points[0].y); // close it, for the sake of it.

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
            for (int pt = points.Count - 3; pt > 1; pt--)
            {
                double deltaY = points[pt + 1].y - points[pt].y;
                double deltaX = points[pt + 1].x - points[pt].x;

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
                double c_ = points[pt].y - m * points[pt].x;

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
                    double tiltAngle_3sigmaVar = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tilt) + chaosSettings.getValue(ChaosSettings_implant.Properties.tiltVar) * implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.tiltV);
                    if (tiltAngle_3sigmaVar < 0)
                    {
                        tiltAngle_3sigmaVar = 0.0f;
                    }
                    if (tiltAngle_3sigmaVar > 90)
                    {
                        tiltAngle_3sigmaVar = 90.0f;
                    }
                    double twistAngle_3sigmaVar = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.twist) + chaosSettings.getValue(ChaosSettings_implant.Properties.twistVar) * implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.twistV);
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
            shadow = Helper.initedPathD(4);
            shadow[0] = new (0.0f, nomYIntercept);
            shadow[1] = new (nomXIntercept, 0.0f);
            shadow[2] = new (shadow[1].x, shadow[1].y);
            shadow[3] = new (shadow[0].x, shadow[0].y);

            // Calculate our shadow distance taking into account the resist width beyond the intercept point.
            double actualResistWidth = implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.w) + chaosSettings.getValue(ChaosSettings_implant.Properties.resistCDVar) * implantCalcSettings.getDouble(EntropyImplantSettings.properties_d.wV);
            // Force a format here to avoid blanks in some cases.
            result = (shadow[1].x - actualResistWidth / 2.0f).ToString("#.##");
            // Extra check in case of an escape above, but shouldn't be needed.
            if (result == "")
            {
                result = "0.0";
            }

            minShadow = Helper.initedPathD(4);
            maxShadow = Helper.initedPathD(4);

            if (chaosSettings.isPreview())
            {
                minShadow[0] = new (0.0f, minYIntercept);
                minShadow[1] = new (minXIntercept, 0.0f);
                minShadow[2] = new (minShadow[1].x, minShadow[1].y);
                minShadow[3] = new (minShadow[0].x, minShadow[0].y);

                maxShadow[0] = new (0.0f, maxYIntercept);
                maxShadow[1] = new (maxXIntercept, 0.0f);
                maxShadow[2] = new (maxShadow[1].x, maxShadow[1].y);
                maxShadow[3] = new (maxShadow[0].x, maxShadow[0].y);

                min = (minShadow[1].x - actualResistWidth / 2.0f).ToString(CultureInfo.InvariantCulture);
                // Seem to get periodic blanks for 0 cases so force the issue here.
                if (min == "")
                {
                    min = "0.0";
                }
                max = (maxShadow[1].x - actualResistWidth / 2.0f).ToString(CultureInfo.InvariantCulture);
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

                minShadow[0] = new (0, 0);
                minShadow[1] = new (0, 0);
                minShadow[2] = new (0, 0);
                minShadow[3] = new (0, 0);

                maxShadow[0] = new (0, 0);
                maxShadow[1] = new (0, 0);
                maxShadow[2] = new (0, 0);
                maxShadow[3] = new (0, 0);
            }

            // Now need to scale our polygon back down again for display.
            geom = Helper.initedPathD(points.Count); // resist profile that has been evaluated for shadowing.
            bgGeom = Helper.initedPathD(points.Count); // display the other resist side, to avoid confusing user, but this isn't evaluated in shadowing.
#if !CHAOSSINGLETHREADED
            Parallel.For(0, points.Count, pt =>
#else
                for (int pt = 0; pt < points.Length; pt++)
#endif
                {
                    double x = (double)points[pt].x;
                    double y = (double)points[pt].y;
                    geom[pt] = new (x, y);
                    bgGeom[pt] = new (-x, y);
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