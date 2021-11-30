using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using geoWrangler;
using utility;

namespace Variance;

using Path = List<IntPoint>;
using Paths = List<List<IntPoint>>;

internal class angleHandler
{
    public double minimumIntersectionAngle { get; private set; }
    private Paths listOfOutputPoints;
    public Paths resultPaths { get; private set; } // will only have one path, for minimum angle.

    private void ZFillCallback(IntPoint bot1, IntPoint top1, IntPoint bot2, IntPoint top2, ref IntPoint pt)
    {
        pt.Z = -1; // Tag our intersection points.
    }

    // Distance functions to drive scale-up of intersection marker if needed.
    private double minDistance = 10.0;

    public angleHandler(Paths layerAPath, Paths layerBPath)
    {
        angleHandlerLogic(layerAPath, layerBPath);
    }

    private void angleHandlerLogic(Paths layerAPath, Paths layerBPath)
    {
        listOfOutputPoints = new Paths();
        resultPaths = new Paths();
        Path resultPath = new();
        Clipper c = new() {ZFillFunction = ZFillCallback};
        c.AddPaths(layerAPath, PolyType.ptSubject, true);
        c.AddPaths(layerBPath, PolyType.ptClip, true);

        // Boolean AND of the two levels for the area operation.
        c.Execute(ClipType.ctIntersection, listOfOutputPoints);

        // Set initial output value for the case there are no intersections
        minimumIntersectionAngle = 180.0; // no intersection angle.

        double tmpVal = listOfOutputPoints.Sum(t => Clipper.Area(t));
        if (tmpVal == 0.0)
        {
            // No overlap
            // Set output path and avoid heavy lifting
            resultPath.Add(new IntPoint(0, 0));
            resultPaths.Add(resultPath);
        }
        else
        {
            double temporaryResult = 180.0;
            Path temporaryPath = new() {new IntPoint(0, 0), new IntPoint(0, 0), new IntPoint(0, 0)};
            foreach (Path t in listOfOutputPoints)
            {
                Path overlapPath = GeoWrangler.clockwise(t);

                int pt = 0;
                while (pt < overlapPath.Count)
                {
                    if (overlapPath[pt].Z == -1)
                    {
                        // intersection point found - let's get our three points to find the angle.
                        // http://en.wikipedia.org/wiki/Law_of_cosines
                        IntPoint interSection_B;
                        IntPoint interSection_C;
                        IntPoint interSection_A;
                        if (pt == 0)
                        {
                            // Find preceding not-identical point.
                            int refPt = overlapPath.Count - 1;
                            while (Math.Abs(GeoWrangler.distanceBetweenPoints(overlapPath[refPt], overlapPath[pt])) == 0)
                            {
                                refPt--;
                                if (refPt == 0)
                                {
                                    break;
                                }
                            }
                            interSection_B = overlapPath[refPt]; // map to last point
                            interSection_C = overlapPath[pt];
                            // Find following not-identical point.
                            refPt = 0;
                            while (Math.Abs(GeoWrangler.distanceBetweenPoints(overlapPath[refPt], overlapPath[pt])) == 0)
                            {
                                refPt++;
                                if (refPt == overlapPath.Count - 1)
                                {
                                    break;
                                }
                            }
                            interSection_A = overlapPath[refPt];
                        }
                        else if (pt == overlapPath.Count - 1) // last point in the list
                        {
                            // Find preceding not-identical point.
                            int refPt = pt;
                            while (Math.Abs(GeoWrangler.distanceBetweenPoints(overlapPath[refPt], overlapPath[pt])) == 0)
                            {
                                refPt--;
                                if (refPt == 0)
                                {
                                    break;
                                }
                            }
                            interSection_B = overlapPath[refPt];
                            interSection_C = overlapPath[pt];
                            // Find following not-identical point.
                            refPt = 0;
                            while (Math.Abs(GeoWrangler.distanceBetweenPoints(overlapPath[refPt], overlapPath[pt])) == 0)
                            {
                                refPt++;
                                if (refPt == overlapPath.Count - 1)
                                {
                                    break;
                                }
                            }
                            interSection_A = overlapPath[0]; // map to the first point
                        }
                        else
                        {
                            // Find preceding not-identical point.
                            int refPt = pt;
                            while (Math.Abs(GeoWrangler.distanceBetweenPoints(overlapPath[refPt], overlapPath[pt])) == 0)
                            {
                                refPt--;
                                if (refPt == 0)
                                {
                                    break;
                                }
                            }
                            interSection_B = overlapPath[refPt];
                            interSection_C = overlapPath[pt];
                            // Find following not-identical point.
                            refPt = pt;
                            while (Math.Abs(GeoWrangler.distanceBetweenPoints(overlapPath[refPt], overlapPath[pt])) == 0)
                            {
                                refPt++;
                                if (refPt == overlapPath.Count - 1)
                                {
                                    break;
                                }
                            }
                            interSection_A = overlapPath[refPt];
                        }

                        IntPoint cBVector = new(interSection_B.X - interSection_C.X, interSection_B.Y - interSection_C.Y);
                        IntPoint cAVector = new(interSection_A.X - interSection_C.X, interSection_A.Y - interSection_C.Y);

                        long xComponents = cBVector.X * cAVector.X;
                        long yComponents = cBVector.Y * cAVector.Y;

                        long scalarProduct = xComponents + yComponents;

                        double cBMagnitude = Math.Sqrt(Utils.myPow(cBVector.X, 2) + Utils.myPow(cBVector.Y, 2));
                        double cAMagnitude = Math.Sqrt(Utils.myPow(cAVector.X, 2) + Utils.myPow(cAVector.Y, 2));

                        double theta = Math.Abs(Utils.toDegrees(Math.Acos(scalarProduct / (cBMagnitude * cAMagnitude)))); // Avoid falling into a trap with negative angles.

                        if (theta < temporaryResult)
                        {
                            temporaryResult = theta;
                            temporaryPath.Clear();
                            temporaryPath.Add(new IntPoint(interSection_A.X, interSection_A.Y));
                            temporaryPath.Add(new IntPoint(interSection_C.X, interSection_C.Y));
                            temporaryPath.Add(new IntPoint(interSection_B.X, interSection_B.Y));
                        }
                    }
                    pt++;
                }
            }
            minimumIntersectionAngle = temporaryResult;

            // Check our temporary path to see if we need to scale it up.
            double distance = GeoWrangler.distanceBetweenPoints(temporaryPath[0], temporaryPath[1]) / CentralProperties.scaleFactorForOperation;
            IntPoint distanceIntPoint = GeoWrangler.intPoint_distanceBetweenPoints(temporaryPath[0], temporaryPath[1]); // A to C
            if (distance < minDistance)
            {
                double X = temporaryPath[0].X;
                double Y = temporaryPath[0].Y;
                if (temporaryPath[1].X != temporaryPath[0].X)
                {
                    X = temporaryPath[1].X + distanceIntPoint.X * (minDistance / distance);
                }
                if (temporaryPath[1].Y != temporaryPath[0].Y)
                {
                    Y = temporaryPath[1].Y + distanceIntPoint.Y * (minDistance / distance);
                }
                temporaryPath[0] = new IntPoint((long)X, (long)Y);
            }
            distance = GeoWrangler.distanceBetweenPoints(temporaryPath[2], temporaryPath[1]) / CentralProperties.scaleFactorForOperation;
            distanceIntPoint = GeoWrangler.intPoint_distanceBetweenPoints(temporaryPath[2], temporaryPath[1]); // B to C
            if (distance < minDistance)
            {
                double X = temporaryPath[2].X;
                double Y = temporaryPath[2].Y;
                if (temporaryPath[1].Y != temporaryPath[2].Y)
                {
                    Y = temporaryPath[1].Y + distanceIntPoint.Y * (minDistance / distance);
                }
                if (temporaryPath[1].X != temporaryPath[2].X)
                {
                    X = temporaryPath[1].X + distanceIntPoint.X * (minDistance / distance);
                }
                temporaryPath[2] = new IntPoint((long)X, (long)Y);
            }
            resultPaths.Add(temporaryPath.ToList());
        }
    }
}