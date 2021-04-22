using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ClipperLib;
using Error;
using geoLib;
using geoWrangler;
using KDTree;
using utility;

namespace Variance
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    class DistanceHandler
    {
        bool debug = false;
        public Paths resultPaths { get; private set; }
        double resultDistance;
        public string distanceString { get; private set; }
        bool invert;

        void ZFillCallback(IntPoint bot1, IntPoint top1, IntPoint bot2, IntPoint top2, ref IntPoint pt)
        {
            pt.Z = -1; // Tag our intersection points.
        }

        class spaceResult
        {
            public bool done { get; set; } // to flag early return.
            public double distance { get; set; }
            public Paths resultPaths { get; set; }

            public spaceResult()
            {
                resultPaths = new Paths();
            }
        }

        public DistanceHandler(Paths aPaths, Paths bPaths, EntropySettings simulationSettings, bool runThreaded)
        {
            distanceHandlerLogic(aPaths, bPaths, simulationSettings, runThreaded);
        }

        void distanceHandlerLogic(Paths aPaths, Paths bPaths, EntropySettings simulationSettings, bool runThreaded)
        {
            resultPaths = new Paths();
            // Safety check for no active layers.
            if (((aPaths.Count == 1) && (aPaths[0].Count == 1)) ||
                ((bPaths.Count == 1) && (bPaths[0].Count == 1)))
            {
                Path tempPath = new Path {new IntPoint(0, 0)};
                resultPaths.Add(tempPath.ToList());
                distanceString = "N/A";
            }
            else
            {
                invert = true;
                // Experimental check to see whether we can simplify this approach.
                bool isEnclosed = GeoWrangler.enclosed(aPaths, bPaths);


                if (!isEnclosed)
                {
                    // Overlap method sets result fields.
                    overlap(aPaths, bPaths, simulationSettings, runThreaded);
                }
                else
                {
                    spaceResult result = fastKDTree(aPaths, bPaths, simulationSettings);

                    resultDistance = result.distance;
                    resultPaths = result.resultPaths;
                }

                distanceString = (resultDistance).ToString(CultureInfo.InvariantCulture);
            }
        }

        spaceResult fastKDTree(Paths aPaths, Paths bPaths, EntropySettings simulationSettings)
        {
            Int32 numberOfPoints = 0;
            double currentMinimum = 0;
            Path minimumDistancePath = new Path();
            bool resultNeedsInversion = false;

            double refArea = 0;

            for (Int32 shapeB = 0; shapeB < bPaths.Count(); shapeB++)
            {
                numberOfPoints += bPaths[shapeB].Count();
                refArea += Clipper.Area(bPaths[shapeB]);
            }
            // KDTree to store the points from our target shape(s)
            KDTree<GeoLibPointF> pTree = new KDTree<GeoLibPointF>(2, numberOfPoints);

            for (Int32 shapeA = 0; shapeA < aPaths.Count(); shapeA++)
            {
                for (Int32 shapeB = 0; shapeB < bPaths.Count(); shapeB++)
                {
                    for (Int32 pointB = 0; pointB < bPaths[shapeB].Count(); pointB++)
                    {
                        try
                        {
                            pTree.AddPoint(new double[] { bPaths[shapeB][pointB].X, bPaths[shapeB][pointB].Y }, new GeoLibPointF(bPaths[shapeB][pointB].X, bPaths[shapeB][pointB].Y));
                        }
                        catch (Exception ex)
                        {
                            ErrorReporter.showMessage_OK("Oops", "jobEngine() KDTree error: " + ex);
                        }
                    }
                }

                // Do we need to invert the result?
                Clipper c = new Clipper {PreserveCollinear = true};
                Paths oCheck = new Paths();
                c.AddPaths(bPaths, PolyType.ptClip, true);
                c.AddPath(aPaths[shapeA], PolyType.ptSubject, true);
                c.Execute(ClipType.ctUnion, oCheck);

                double oCheckArea = 0;
                foreach (Path t in oCheck)
                {
                    oCheckArea += Clipper.Area(t);
                }

                if ((simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosure) || (simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosureOld)) // negative value since we're fully inside a containing polygon.
                {
                    resultNeedsInversion = Math.Abs(Math.Abs(oCheckArea) - Math.Abs(refArea)) > Double.Epsilon;
                }
                if ((simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacing) || (simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacingOld)) // negative value since we're fully outside a containing polygon.
                {
                    resultNeedsInversion = Math.Abs(Math.Abs(oCheckArea) - Math.Abs(refArea)) < Double.Epsilon;
                }

                // We can query here for the minimum distance for each shape combination.
                for (Int32 pointA = 0; pointA < aPaths[shapeA].Count; pointA++)
                {
                    // '1' forces a single nearest neighbor to be returned.
                    NearestNeighbour<GeoLibPointF> pIter = pTree.NearestNeighbors(new double[] { aPaths[shapeA][pointA].X, aPaths[shapeA][pointA].Y }, 1);
                    while (pIter.MoveNext())
                    {
                        double currentDistance = pIter.CurrentDistance;

                        if (((shapeA == 0) && (pointA == 0)) || (currentDistance < currentMinimum))
                        {
                            minimumDistancePath.Clear();
                            minimumDistancePath.Add(new IntPoint(aPaths[shapeA][pointA]));
                            minimumDistancePath.Add(new IntPoint((pIter.Current.X + aPaths[shapeA][pointA].X) / 2.0f, (pIter.Current.Y + aPaths[shapeA][pointA].Y) / 2.0f));
                            minimumDistancePath.Add(new IntPoint(pIter.Current.X, pIter.Current.Y));
                            currentMinimum = currentDistance;
                        }
                    }
                }
            }

            spaceResult result = new spaceResult
            {
                resultPaths = new Paths {minimumDistancePath},
                // k-d tree distance is the squared distance. Need to scale and sqrt
                distance = Math.Sqrt(currentMinimum / Utils.myPow(CentralProperties.scaleFactorForOperation, 2))
            };

            if (resultNeedsInversion)
            {
                result.distance *= -1;
            }

            return result;
        }

        void overlap(Paths aPaths, Paths bPaths, EntropySettings simulationSettings, bool runThreaded)
        {
            bool completeOverlap = false;
            foreach (Path layerAPath in aPaths)
            {
                foreach (Path layerBPath in bPaths)
                {
                    Paths overlapShape = new Paths();

                    // Check for complete overlap
                    Clipper c = new Clipper {PreserveCollinear = true};
                    // Try a union and see whether the point count of the perimeter changes. This might break for re-entrant cases, but those are problematic anyway.
                    Paths fullOverlapCheck = new Paths();
                    c.AddPath(layerAPath, PolyType.ptSubject, true);
                    c.AddPath(layerBPath, PolyType.ptClip, true);
                    c.Execute(ClipType.ctUnion, fullOverlapCheck);
                    double aArea = Math.Abs(Clipper.Area(layerAPath));
                    double bArea = Math.Abs(Clipper.Area(layerBPath));
                    double uArea = 0;
                    foreach (Path t in fullOverlapCheck)
                    {
                        uArea += Clipper.Area(t);
                    }
                    uArea = Math.Abs(uArea);

                    // If overlap area matches either of the input areas, we have a full overlap
                    if ((Math.Abs(aArea - uArea) < Double.Epsilon) || (Math.Abs(bArea - uArea) < Double.Epsilon))
                    {
                        completeOverlap = true;
                    }

                    if ((simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacing) || (simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacingOld)) // spacing
                    {
                        // Perform an area check in case of overlap.
                        // Overlap means X/Y negative space needs to be reported.
                        AreaHandler aH = new AreaHandler(new Paths { layerAPath }, new Paths { layerBPath }, maySimplify: false, perPoly: false, scaleFactorForPointF: 1.0);
                        overlapShape = aH.listOfOutputPoints;//.ToList();
                    }

                    if (!completeOverlap && ((simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosure) || ((simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosureOld)))) // enclosure
                    {
                        // Need to find the region outside our enclosure shape. We use the modifier to handle this.
                        c.Clear();
                        c.ZFillFunction = ZFillCallback;
                        c.StrictlySimple = true;
                        c.PreserveCollinear = true;
                        // Try cutting layerB from layerA
                        c.AddPath(layerAPath, PolyType.ptSubject, true);
                        c.AddPath(layerBPath, PolyType.ptClip, true);
                        c.Execute(ClipType.ctDifference, overlapShape);
                    }

                    if ((!completeOverlap) && (overlapShape.Any())) // call if there's an overlap, and it's incomplete.
                    {
                        spaceResult result = doPartialOverlap(overlapShape, layerAPath, layerBPath, simulationSettings, runThreaded);
                        if (result.done)
                        {
                            if ((!resultPaths.Any()) || (result.distance < resultDistance))
                            {
                                resultPaths = result.resultPaths;
                                resultDistance = result.distance;
                            }
                        }
                    }
                }
            }
        }

        spaceResult doPartialOverlap(Paths overlapShape, Path aPath, Path bPath, EntropySettings simulationSettings, bool runThreaded)
        {
            spaceResult result = new spaceResult();

            // KDTree to store the points from our input shape(s)
            int aPathCount = aPath.Count;
            int bPathCount = bPath.Count;
            KDTree<GeoLibPointF> aTree = new KDTree<GeoLibPointF>(2, aPathCount);
            KDTree<GeoLibPointF> bTree = new KDTree<GeoLibPointF>(2, bPathCount);
            for (int pt = 0; pt < aPathCount; pt++)
            {
                aTree.AddPoint(new double[] { aPath[pt].X, aPath[pt].Y }, new GeoLibPointF(aPath[pt].X, aPath[pt].Y));
            }
            for (int pt = 0; pt < bPathCount; pt++)
            {
                bTree.AddPoint(new double[] { bPath[pt].X, bPath[pt].Y }, new GeoLibPointF(bPath[pt].X, bPath[pt].Y));
            }

            int oCount = overlapShape.Count();
            
            try
            {
                // Process the overlap shape polygon(s) to evaluate the overlap.
                for (Int32 poly = 0; poly < oCount; poly++)
                {
                    // Brute force decomposition of the overlap shape into A and B edge contributions.
                    // ClipperLib isn't robust for this, so we have to brute force it : line clipping fails and PointInPolygon is unreliable.

                    int ptCount = overlapShape[poly].Count;
                    bool[] residesOnAEdge = new bool[ptCount];
                    bool[] residesOnBEdge = new bool[ptCount];

                    for (int pt = 0; pt < ptCount; pt++)
                    {
                        residesOnAEdge[pt] = false;
                        residesOnBEdge[pt] = false;
                        if (overlapShape[poly][pt].Z == -1)
                        {
                            residesOnAEdge[pt] = true;
                            residesOnBEdge[pt] = true;
                        }
                        else
                        {
                            // Use the KDTree approach to find the nearest neighbor on each input shape to the point on the overlap geometry.
                            // Minimum distance wins ownership.
                            var aIter = aTree.NearestNeighbors(new double[] { overlapShape[poly][pt].X, overlapShape[poly][pt].Y }, 1);
                            aIter.MoveNext();
                            double distanceToAEdge = Math.Sqrt(aIter.CurrentDistance);
                            var bIter = bTree.NearestNeighbors(new double[] { overlapShape[poly][pt].X, overlapShape[poly][pt].Y }, 1);
                            bIter.MoveNext();
                            double distanceToBEdge = Math.Sqrt(bIter.CurrentDistance);
                            if (distanceToAEdge < distanceToBEdge)
                            {
                                // This overlap point is close to a point on A, so tag it and move to the next overlap point.
                                residesOnAEdge[pt] = true;
                            }
                            else
                            {
                                // This overlap point is close to a point on B, so tag it and move to the next overlap point.
                                residesOnBEdge[pt] = true;
                            }
                        }
                    }

                    // Now we need to construct our Paths for the overlap edges, based on the true/false case for each array.
                    Paths aOverlapEdge = new Paths();
                    Paths bOverlapEdge = new Paths();

                    Path tempPath = new Path();
                    for (int i = 0; i < ptCount; i++)
                    {
                        if (residesOnAEdge[i])
                        {
                            tempPath.Add(new IntPoint(overlapShape[poly][i]));
                        }
                        else // not found on A edge, probably resides on B edge, but we'll check that separately to keep code readable.
                        {
                            // If we have a tempPath with more than a single point in it, we have been creating a segment.
                            // Since we haven't found this point in the A edge, commit the segment and clear the temporary path.
                            if (tempPath.Count >= 1)
                            {
                                // We have some points, but now we're getting a new segment.
                                aOverlapEdge.Add(tempPath.ToList());
                                tempPath.Clear();
                            }
                        }
                    }
                    if (tempPath.Count > 1)
                    {
                        aOverlapEdge.Add(tempPath.ToList());
                    }

                    tempPath.Clear();
                    for (int i = 0; i < ptCount; i++)
                    {
                        if (residesOnBEdge[i])
                        {
                            tempPath.Add(new IntPoint(overlapShape[poly][i]));
                        }
                        else
                        {
                            if (tempPath.Count > 1)
                            {
                                // We have some points, but now we're getting a new segment.
                                bOverlapEdge.Add(tempPath.ToList());
                                tempPath.Clear();
                            }
                        }
                    }
                    if (tempPath.Count > 1)
                    {
                        bOverlapEdge.Add(tempPath.ToList());
                    }

                    // Walk our edges to figure out the overlap.
                    foreach (Path t in aOverlapEdge)
                    {
                        foreach (Path t1 in bOverlapEdge)
                        {
                            spaceResult tResult = overlapAssess(simulationSettings, overlapShape[poly], t, t1, aPath, bPath, runThreaded);
                            if ((result.resultPaths.Count == 0) || (tResult.distance > result.distance))
                            {
                                result.distance = tResult.distance;
                                result.resultPaths = tResult.resultPaths;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorReporter.showMessage_OK(e.ToString(), "Oops");
            }
            result.done = true;
            if (!simulationSettings.debugCalc)
            {
                result.distance = -result.distance / CentralProperties.scaleFactorForOperation;
            }
            return result;
        }

        spaceResult overlapAssess(EntropySettings simulationSettings, Path overlapPoly, Path aOverlapEdge, Path bOverlapEdge, Path aPath, Path bPath, bool runThreaded)
        {
            spaceResult result = new spaceResult();
            double maxDistance_orthogonalFallback = 0; // fallback for the overlap case of orthogonal geometry, where self-intersections occur.
            int maxDistance_fallbackIndex = 0;
            double maxDistance = 0; // for the overlap cases.

            double lengthA = 0;
            double lengthB = 0;
            Path extractedPath = new Path();
            bool usingAEdge = false;

            /* Prior to 1.7, we used the edge from the layerB combination to raycast. This was not ideal.
                We should have used the shortest edge. 1.7 adds an option to make this the behavior, and the old 
                approach is retained for now as well.
            */

            IntPoint shortestPathBeforeStartPoint = new IntPoint(0, 0);
            IntPoint shortestPathAfterEndPoint = new IntPoint(0, 0);

            if ((simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacing))
            {
                Path aSource = aOverlapEdge;
                Path bSource = bOverlapEdge;
                // Find the shortest edge length and use that for the projection reference
                // Calculate lengths and check.
                for (int pt = 0; pt < aSource.Count - 1; pt++)
                {
                    lengthA += Math.Sqrt(Utils.myPow((aSource[pt + 1].X - aSource[pt].X), 2) + Utils.myPow((aSource[pt + 1].Y - aSource[pt].Y), 2));
                }
                for (int pt = 0; pt < bSource.Count - 1; pt++)
                {
                    lengthB += Math.Sqrt(Utils.myPow((bSource[pt + 1].X - bSource[pt].X), 2) + Utils.myPow((bSource[pt + 1].Y - bSource[pt].Y), 2));
                }

                extractedPath = bSource; // need a default in case of equivalent lengths

                if (extractedPath.Count == 0)
                {
                    // No common overlap.
                    result.distance = 0;
                    return result;
                }

                // Here we need to go back to our input polygons to derive the edge segment normal for the start/end of each edge.
                if (lengthA < lengthB)
                {
                    usingAEdge = true;
                    extractedPath = aSource;
                    int startIndex = aPath.FindIndex(p => p.Equals(extractedPath[0]));
                    if (startIndex == 0)
                    {
                        startIndex = aPath.Count - 1;
                    }
                    if (startIndex == -1)
                    {
                        // Failed to find it cheaply. Let's try a different approach.
                        double startDistanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[0], aPath[0]);
                        startIndex = 0;
                        for (int pt = 1; pt < aPath.Count; pt++)
                        {
                            double distanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[0], aPath[pt]);
                            if (distanceCheck < startDistanceCheck)
                            {
                                startDistanceCheck = distanceCheck;
                                startIndex = pt;
                            }
                        }
                    }

                    int endIndex = aPath.FindIndex(p => p.Equals(extractedPath[^1]));
                    if (endIndex == aPath.Count - 1)
                    {
                        endIndex = 0;
                    }
                    if (endIndex == -1)
                    {
                        // Failed to find it cheaply. Let's try a different approach.
                        double endDistanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[^1], aPath[0]);
                        endIndex = 0;
                        for (int pt = 1; pt < aPath.Count; pt++)
                        {
                            double distanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[^1], aPath[pt]);
                            if (distanceCheck < endDistanceCheck)
                            {
                                endDistanceCheck = distanceCheck;
                                endIndex = pt;
                            }
                        }
                    }

                    shortestPathBeforeStartPoint = new IntPoint(aPath[startIndex]);
                    shortestPathAfterEndPoint = new IntPoint(aPath[endIndex]);
                }
                else
                {
                    int startIndex = bPath.FindIndex(p => p.Equals(extractedPath[0]));
                    if (startIndex == 0)
                    {
                        startIndex = bPath.Count - 1;
                    }
                    if (startIndex == -1)
                    {
                        // Failed to find it cheaply. Let's try a different approach.
                        double startDistanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[0], bPath[0]);
                        startIndex = 0;

                        for (int pt = 1; pt < bPath.Count; pt++)
                        {
                            double distanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[0], bPath[pt]);
                            if (distanceCheck < startDistanceCheck)
                            {
                                startDistanceCheck = distanceCheck;
                                startIndex = pt;
                            }
                        }
                    }

                    int endIndex = bPath.FindIndex(p => p.Equals(extractedPath[^1]));
                    if (endIndex == bPath.Count - 1)
                    {
                        endIndex = 0;
                    }
                    if (endIndex == -1)
                    {
                        // Failed to find it cheaply. Let's try a different approach.
                        double endDistanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[^1], bPath[0]);
                        endIndex = 0;
                        for (int pt = 1; pt < bPath.Count; pt++)
                        {
                            double distanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[^1], bPath[pt]);
                            if (distanceCheck < endDistanceCheck)
                            {
                                endDistanceCheck = distanceCheck;
                                endIndex = pt;
                            }
                        }
                    }

                    shortestPathBeforeStartPoint = new IntPoint(bPath[startIndex]);
                    shortestPathAfterEndPoint = new IntPoint(bPath[endIndex]);
                }
            }

            if ((simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.spacingOld) || (simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosure) || (simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.spacingCalcModes.enclosureOld))
            {

                extractedPath = bOverlapEdge;

                if (extractedPath.Count == 0) // No common edge.
                {
                    result.distance = 0;
                    return result;
                }

                int startIndex = bPath.FindIndex(p => p.Equals(extractedPath[0]));
                if (startIndex == 0)
                {
                    startIndex = bPath.Count - 1;
                }
                if (startIndex == -1)
                {
                    // Failed to find it cheaply. Let's try a different approach.
                    double startDistanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[0], bPath[0]);
                    startIndex = 0;
                    for (int pt = 1; pt < bPath.Count; pt++)
                    {
                        double distanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[0], bPath[pt]);
                        if (distanceCheck < startDistanceCheck)
                        {
                            startDistanceCheck = distanceCheck;
                            startIndex = pt;
                        }
                    }
                }

                int endIndex = bPath.FindIndex(p => p.Equals(extractedPath[^1]));
                if (endIndex == bPath.Count - 1)
                {
                    endIndex = 0;
                }
                if (endIndex == -1)
                {
                    // Failed to find it cheaply. Let's try a different approach.
                    double endDistanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[^1], bPath[0]);
                    for (int pt = 1; pt < bPath.Count; pt++)
                    {
                        double distanceCheck = GeoWrangler.distanceBetweenPoints(extractedPath[^1], bPath[pt]);
                        endIndex = 0;
                        if (distanceCheck < endDistanceCheck)
                        {
                            endDistanceCheck = distanceCheck;
                            endIndex = pt;
                        }
                    }
                }

                try
                {
                    shortestPathBeforeStartPoint = new IntPoint(bPath[startIndex]);
                }
                catch (Exception)
                {
                    ErrorReporter.showMessage_OK("distanceHandler: shortestPathBeforeStartPoint failed.", "Oops");
                }
                try
                {
                    shortestPathAfterEndPoint = new IntPoint(bPath[endIndex]);
                }
                catch (Exception)
                {
                    ErrorReporter.showMessage_OK("distanceHandler: shortestPathAfterEndPoint failed.", "Oops");
                }
            }

            // No blurry rays, so no point running the inner loop threaded. We thread the outer loop (the emission edge raycast), though. Testing showed small performance improvement for this approach.
            RayCast rc = new RayCast(extractedPath, overlapPoly, CentralProperties.scaleFactorForOperation * CentralProperties.scaleFactorForOperation, runThreaded, invert, 0, true, false, shortestPathBeforeStartPoint, shortestPathAfterEndPoint);

            if (debug || simulationSettings.debugCalc)
            {
                result.done = true;
                result.resultPaths = rc.getRays();
                result.distance = -1;
                return result;
            }

            Paths clippedLines = rc.getClippedRays();

            bool validOverlapFound = false;

            // Need to scan for the maximum path length to record the overlap.
            for (Int32 line = 0; line < clippedLines.Count(); line++)
            {
                double lineLength = rc.getRayLength(line);
                // With the new LER implementation, normals can be fired in problematic directions.
                // Valid overlaps don't have start and end points on the same polygon.
                // Happily, we feed this polygon-wise, which makes the evaluation much easier.
                bool validOverlap = true;

                double startPointCheck_A_dist = GeoWrangler.distanceBetweenPoints(clippedLines[line][0], aOverlapEdge[0]);
                double endPointCheck_A_dist = GeoWrangler.distanceBetweenPoints(clippedLines[line][1], aOverlapEdge[0]);
                foreach (IntPoint t in aOverlapEdge)
                {
                    double temp_startPointCheck_A_dist = GeoWrangler.distanceBetweenPoints(clippedLines[line][0], t);
                    double temp_endPointCheck_A_dist = GeoWrangler.distanceBetweenPoints(clippedLines[line][1], t);
                    if (temp_startPointCheck_A_dist < startPointCheck_A_dist)
                    {
                        startPointCheck_A_dist = temp_startPointCheck_A_dist;
                    }
                    if (temp_endPointCheck_A_dist < endPointCheck_A_dist)
                    {
                        endPointCheck_A_dist = temp_endPointCheck_A_dist;
                    }
                }

                double startPointCheck_B_dist = GeoWrangler.distanceBetweenPoints(clippedLines[line][0], bOverlapEdge[0]);
                double endPointCheck_B_dist = GeoWrangler.distanceBetweenPoints(clippedLines[line][1], bOverlapEdge[0]);
                foreach (IntPoint t in bOverlapEdge)
                {
                    double temp_startPointCheck_B_dist = GeoWrangler.distanceBetweenPoints(clippedLines[line][0], t);
                    double temp_endPointCheck_B_dist = GeoWrangler.distanceBetweenPoints(clippedLines[line][1], t);
                    if (temp_startPointCheck_B_dist < startPointCheck_B_dist)
                    {
                        startPointCheck_B_dist = temp_startPointCheck_B_dist;
                    }
                    if (temp_endPointCheck_B_dist < endPointCheck_B_dist)
                    {
                        endPointCheck_B_dist = temp_endPointCheck_B_dist;
                    }
                }

                bool ortho = (clippedLines[line][0].X == clippedLines[line][1].X) || (clippedLines[line][0].Y == clippedLines[line][1].Y);

                double threshold = 1500; // arbitrary, dialed in by hand.

                if ((ortho) || ((usingAEdge && ((startPointCheck_A_dist < threshold) && (endPointCheck_A_dist < threshold))) ||
                                (!usingAEdge && ((startPointCheck_B_dist < threshold) && (endPointCheck_B_dist < threshold))))
                )
                {
                    // This is a special situation, it turns out.
                    // There is one specific scenario where this overlap case (start and end on the same geometry) is valid - orthogonal shapes with a bisection.
                    // The orthogonal shapes cause the rays to hit the opposite side of the shape. We don't want to reject this.
                    if (lineLength > maxDistance_orthogonalFallback)
                    {
                        maxDistance_fallbackIndex = line;
                        maxDistance_orthogonalFallback = lineLength;
                    }
                }
                else
                {
                    if ((usingAEdge && ((startPointCheck_A_dist < threshold) && (endPointCheck_B_dist > threshold))) ||
                        (!usingAEdge && ((startPointCheck_B_dist < threshold) && (endPointCheck_A_dist > threshold))))
                    {
                        validOverlap = false;
                    }

                    if (validOverlap && (lineLength > maxDistance))
                    {
                        validOverlapFound = true;
                        maxDistance = lineLength;
                        Path tempPath = new Path
                        {
                            new IntPoint(clippedLines[line][0]),
                            new IntPoint(clippedLines[line][0]),
                            new IntPoint(clippedLines[line][clippedLines[line].Count() - 1])
                        };
                        result.resultPaths.Clear();
                        result.resultPaths.Add(tempPath);
                    }
                }
            }

            try
            {
                if (((clippedLines.Any()) && (!validOverlapFound)) || (maxDistance_orthogonalFallback > maxDistance))
                {
                    // Couldn't find a valid overlap so assume the orthogonal fallback is needed.
                    maxDistance = maxDistance_orthogonalFallback;
                    Path tempPath = new Path
                    {
                        new IntPoint(clippedLines[maxDistance_fallbackIndex][0]),
                        new IntPoint(clippedLines[maxDistance_fallbackIndex][0]),
                        new IntPoint(
                            clippedLines[maxDistance_fallbackIndex]
                                [clippedLines[maxDistance_fallbackIndex].Count() - 1])
                    };
                    result.resultPaths.Clear();
                    result.resultPaths.Add(tempPath);
                }
            }
            catch (Exception)
            {
                // Harmless - we'll reject the case and move on.
            }

            result.distance = maxDistance;
            return result;
        }
    }
}
