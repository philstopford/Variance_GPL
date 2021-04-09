using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using geoWrangler;
using KDTree;

namespace Variance
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    class ChordHandler
    {
        Path a;
        Path b;
        public Paths a_chordPaths { get; set; }// 0 is top, 1 is bottom
        public Paths b_chordPaths { get; set; } // 0 is left, 1 is right
        public double[] aChordLengths { get; set; }
        public double[] bChordLengths { get; set; }
        Int32 aPath_maxX_index;
        Int32 bPath_maxY_index;
        KDTree<IntPoint> aTree;
        KDTree<IntPoint> bTree;

        void ZFillCallback(IntPoint bot1, IntPoint top1, IntPoint bot2, IntPoint top2, ref IntPoint pt)
        {
            pt.Z = -1; // Tag our intersection points.
        }

        void doPathA()
        {
            aChordLengths[0] = 0.0;
            aChordLengths[1] = 0.0;

            Int32 pt = 0; // will set based on start condition checks

            Path testPath = new Path();
            // Gather all of our points on the top edge for the nearest neighbor search
            while (pt <= aPath_maxX_index)
            {
                testPath.Add(new IntPoint(a[pt]));
                pt++;
            }

            Clipper c = new Clipper();
            c.ZFillFunction = ZFillCallback;
            c.PreserveCollinear = true;
            c.AddPath(testPath, PolyType.ptSubject, false);
            c.AddPath(b, PolyType.ptClip, true);
            PolyTree polyTree_top = new PolyTree();
            c.Execute(ClipType.ctIntersection, polyTree_top);
            Paths topChords = Clipper.OpenPathsFromPolyTree(polyTree_top); // potentially more than one chord.

            // Now we evaluate the lower edge
            // We have to start from the max index because it could be shared between top and bottom chords.
            pt = aPath_maxX_index;
            testPath.Clear();
            c.Clear();

            while (pt < a.Count)
            {
                testPath.Add(new IntPoint(a[pt]));
                pt++;
            }

            c.AddPath(testPath, PolyType.ptSubject, false);
            c.AddPath(b, PolyType.ptClip, true);
            PolyTree polyTree_bottom = new PolyTree();
            c.Execute(ClipType.ctIntersection, polyTree_bottom);
            Paths bottomChords = Clipper.OpenPathsFromPolyTree(polyTree_bottom); // potentially more than one chord.

            // Now let's see what we have.

            double minTopChordLength = 0;
            Path topChord = new Path();
            topChord.Add(new IntPoint(0, 0)); // safety in case we have no chords on the top.
            for (int chord = 0; chord < topChords.Count; chord++)
            {
                // Does this chord segment actually belong to the 'B' geometry.
                bool topEdgeIsFromA = true;
                // First point and last point might not be matched to original geometry (imperfect intersection)
                for (int tCPt = 1; tCPt < topChords[chord].Count - 1; tCPt++)
                {
                    var pIter = aTree.NearestNeighbors(new double[] { topChords[chord][tCPt].X, topChords[chord][tCPt].Y }, 1);
                    while (pIter.MoveNext())
                    {
                        if (pIter.CurrentDistance > 0)
                        {
                            topEdgeIsFromA = false;
                            break;
                        }
                    }
                    if (!topEdgeIsFromA)
                    {
                        break;
                    }
                }

                if (!topEdgeIsFromA)
                {
                    continue;
                }

                // skip if false case
                if ((topChords.Count == 1) || ((topChords.Count > 1) && ((topChords[chord][0].Z == -1) && (topChords[chord][topChords[chord].Count - 1].Z == -1))))
                {
                    double chordLength = 0;
                    // Assess length of each chord and only report the minimum length one.
                    for (int chordpt = 0; chordpt < topChords[chord].Count - 1; chordpt++)
                    {
                        chordLength += GeoWrangler.distanceBetweenPoints(topChords[chord][chordpt], topChords[chord][chordpt + 1]);
                    }

                    if ((minTopChordLength == 0) || (chordLength < minTopChordLength))
                    {
                        minTopChordLength = chordLength;
                        topChord = topChords[chord];
                    }
                }
            }

            double minBottomChordLength = 0;
            Path bottomChord = new Path();
            bottomChord.Add(new IntPoint(0, 0)); // safety in case we have no chords on the top.
            for (int chord = 0; chord < bottomChords.Count; chord++)
            {
                // Does this chord segment actually belong to the 'B' geometry.
                bool bottomEdgeIsFromA = true;
                // First point and last point might not be matched to original geometry (imperfect intersection)
                for (int bCPt = 1; bCPt < bottomChords[chord].Count - 1; bCPt++)
                {
                    var pIter = aTree.NearestNeighbors(new double[] { bottomChords[chord][bCPt].X, bottomChords[chord][bCPt].Y }, 1);
                    while (pIter.MoveNext())
                    {
                        if (pIter.CurrentDistance > 0)
                        {
                            bottomEdgeIsFromA = false;
                            break;
                        }
                    }
                    if (!bottomEdgeIsFromA)
                    {
                        break;
                    }
                }

                if (!bottomEdgeIsFromA)
                {
                    continue;
                }

                // skip if false case
                if ((bottomChords.Count == 1) || ((bottomChords.Count > 1) && ((bottomChords[chord][0].Z == -1) && (bottomChords[chord][bottomChords[chord].Count - 1].Z == -1))))
                {
                    double chordLength = 0;
                    // Assess length of each chord and only report the minimum length one.
                    for (int chordpt = 0; chordpt < bottomChords[chord].Count - 1; chordpt++)
                    {
                        chordLength += GeoWrangler.distanceBetweenPoints(bottomChords[chord][chordpt], bottomChords[chord][chordpt + 1]);
                    }

                    if ((minBottomChordLength == 0) || (chordLength < minBottomChordLength))
                    {
                        minBottomChordLength = chordLength;
                        bottomChord = bottomChords[chord];
                    }
                }
            }

            aChordLengths[0] = minBottomChordLength;
            a_chordPaths.Add(bottomChord.ToList());

            aChordLengths[1] = minTopChordLength;
            a_chordPaths.Add(topChord.ToList());
        }

        void doPathB()
        {
            bChordLengths[0] = 0.0;
            bChordLengths[1] = 0.0;

            Int32 pt = 0; // will set based on start condition checks

            Path testPath = new Path();

            // Gather all of our points on the left edge
            while (pt <= bPath_maxY_index)
            {
                testPath.Add(new IntPoint(b[pt]));
                pt++;
            }

            Clipper c = new Clipper();
            c.PreserveCollinear = true;
            c.ZFillFunction = ZFillCallback;
            c.AddPath(testPath, PolyType.ptSubject, false);
            c.AddPath(a, PolyType.ptClip, true);
            PolyTree polyTree_left = new PolyTree();
            c.Execute(ClipType.ctIntersection, polyTree_left);
            Paths leftChords = Clipper.OpenPathsFromPolyTree(polyTree_left); // potentially more than one chord.

            // Now we evaluate the right edge
            // We have to start from the max index because it could be shared between top and bottom chords.
            testPath.Clear();
            c.Clear();

            while (pt < b.Count)
            {
                testPath.Add(new IntPoint(b[pt]));
                pt++;
            }

            c.ZFillFunction = ZFillCallback;
            c.AddPath(testPath, PolyType.ptSubject, false);
            c.AddPath(a, PolyType.ptClip, true);
            PolyTree polyTree_right = new PolyTree();
            c.Execute(ClipType.ctIntersection, polyTree_right);
            Paths rightChords = Clipper.OpenPathsFromPolyTree(polyTree_right); // potentially more than one chord.

            // Now let's see what we have.

            double minLeftChordLength = 0;
            Path leftChord = new Path();
            leftChord.Add(new IntPoint(0, 0)); // safety in case we have no chords on the left.


            for (int chord = 0; chord < leftChords.Count; chord++)
            {
                // Does this chord segment actually belong to the 'B' geometry.
                bool leftEdgeIsFromB = true;
                // First point and last point might not be matched to original geometry (imperfect intersection)
                for (int lCPt = 1; lCPt < leftChords[chord].Count - 1; lCPt++)
                {
                    var pIter = bTree.NearestNeighbors(new double[] { leftChords[chord][lCPt].X, leftChords[chord][lCPt].Y }, 1);
                    while (pIter.MoveNext())
                    {
                        if (pIter.CurrentDistance > 0)
                        {
                            leftEdgeIsFromB = false;
                            break;
                        }
                    }
                    if (!leftEdgeIsFromB)
                    {
                        break;
                    }
                }

                if (!leftEdgeIsFromB)
                {
                    continue;
                }

                // skip if false case
                if ((leftChords.Count == 1) || ((leftChords.Count > 1) && ((leftChords[chord][0].Z == -1) && (leftChords[chord][leftChords[chord].Count - 1].Z == -1))))
                {
                    double chordLength = 0;
                    // Assess length of each chord and only report the minimum length one.
                    for (int chordpt = 0; chordpt < leftChords[chord].Count - 1; chordpt++)
                    {
                        chordLength += GeoWrangler.distanceBetweenPoints(leftChords[chord][chordpt], leftChords[chord][chordpt + 1]);
                    }

                    if ((minLeftChordLength == 0) || (chordLength < minLeftChordLength))
                    {
                        minLeftChordLength = chordLength;
                        leftChord = leftChords[chord];
                    }
                }
            }

            double minRightChordLength = 0;
            Path rightChord = new Path();
            rightChord.Add(new IntPoint(0, 0)); // safety in case we have no chords on the right.
            for (int chord = 0; chord < rightChords.Count; chord++)
            {
                // Does this chord segment actually belong to the 'B' geometry.
                bool rightEdgeIsFromB = true;
                // First point and last point might not be matched to original geometry (imperfect intersection)
                for (int rCPt = 1; rCPt < rightChords[chord].Count - 1; rCPt++)
                {
                    var pIter = bTree.NearestNeighbors(new double[] { rightChords[chord][rCPt].X, rightChords[chord][rCPt].Y }, 1);
                    while (pIter.MoveNext())
                    {
                        if (pIter.CurrentDistance > 0)
                        {
                            rightEdgeIsFromB = false;
                            break;
                        }
                    }
                    if (!rightEdgeIsFromB)
                    {
                        break;
                    }
                }

                if (!rightEdgeIsFromB)
                {
                    continue;
                }

                // skip if false case
                if ((rightChords.Count == 1) || (rightChords.Count > 1 && ((rightChords[chord][0].Z == -1) && (rightChords[chord][rightChords[chord].Count - 1].Z == -1))))
                {
                    double chordLength = 0;
                    // Assess length of each chord and only report the minimum length one.
                    for (int chordpt = 0; chordpt < rightChords[chord].Count - 1; chordpt++)
                    {
                        chordLength += GeoWrangler.distanceBetweenPoints(rightChords[chord][chordpt], rightChords[chord][chordpt + 1]);
                    }

                    if ((minRightChordLength == 0) || (chordLength < minRightChordLength))
                    {
                        minRightChordLength = chordLength;
                        rightChord = rightChords[chord];
                    }
                }
            }

            bChordLengths[0] = minRightChordLength;
            b_chordPaths.Add(rightChord.ToList());

            bChordLengths[1] = minLeftChordLength;
            b_chordPaths.Add(leftChord.ToList());

        }

        public ChordHandler(Paths aSource, Paths bSource, EntropySettings simulationSettings)
        {
            chordHandlerLogic(aSource, bSource, simulationSettings);
        }

        void chordHandlerLogic(Paths aSource, Paths bSource, EntropySettings simulationSettings)
        {
            a_chordPaths = new Paths(2);
            Path tmpPath = new Path();
            tmpPath.Add(new IntPoint(0, 0));
            a_chordPaths.Add(tmpPath.ToList());
            a_chordPaths.Add(tmpPath.ToList());
            b_chordPaths = new Paths(2);
            b_chordPaths.Add(tmpPath.ToList());
            b_chordPaths.Add(tmpPath.ToList());
            aChordLengths = new double[2];
            aChordLengths[0] = aChordLengths[1] = 0.0;
            bChordLengths = new double[2];
            bChordLengths[0] = bChordLengths[1] = 0.0;

            List<ChordHandler> cHList = new List<ChordHandler>();
            for (Int32 aIndex = 0; aIndex < aSource.Count(); aIndex++)
            {
                for (Int32 bIndex = 0; bIndex < bSource.Count(); bIndex++)
                {
                    try
                    {
                        cHList.Add(new ChordHandler(aIndex, aSource, bIndex, bSource, simulationSettings));
                    }
                    catch (Exception)
                    {
                        // Don't care about raised exceptions.
                    }
                }
            }

            /* Review results to extract minimum cases.

				Some additional explanation is needed here. We can have cases where no chord is registered (i.e. zero is reported). However, if we only
				ever checked for a value less than the reported chord length, we'd never have anything other than zero.

				So, in the checks below, the condition against zero is there to take any non-zero value that is reported; then we apply the minimum value from there.
			*/

            for (Int32 result = 0; result < cHList.Count(); result++)
            {
                for (Int32 resultIndex = 0; resultIndex < 2; resultIndex++)
                {
                    // Extract 'a' results only if non-zero
                    if (cHList[result].aChordLengths[resultIndex] != 0.0)
                    {
                        // We have either our first non-zero chord length or a chord length lower than previously recorded.
                        if ((aChordLengths[resultIndex] == 0.0) || (cHList[result].aChordLengths[resultIndex] < aChordLengths[resultIndex]))
                        {
                            aChordLengths[resultIndex] = cHList[result].aChordLengths[resultIndex];
                            a_chordPaths[resultIndex] = cHList[result].a_chordPaths[resultIndex].ToList();
                        }
                    }

                    // Extract 'b' results only if non-zero
                    if (cHList[result].bChordLengths[resultIndex] != 0.0)
                    {
                        // We have either our first non-zero chord length or a chord length lower than previously recorded.
                        if ((bChordLengths[resultIndex] == 0.0) || (cHList[result].bChordLengths[resultIndex] < bChordLengths[resultIndex]))
                        {
                            bChordLengths[resultIndex] = cHList[result].bChordLengths[resultIndex];
                            b_chordPaths[resultIndex] = cHList[result].b_chordPaths[resultIndex].ToList();
                        }
                    }
                }
            }
        }

        ChordHandler(Int32 aPathIndex, Paths aSource, Int32 bPathIndex, Paths bSource, EntropySettings simulationSettings)
        {
            pChordHandlerLogic(aPathIndex, aSource, bPathIndex, bSource, simulationSettings);
        }

        void pChordHandlerLogic(Int32 aPathIndex, Paths aSource, Int32 bPathIndex, Paths bSource, EntropySettings simulationSettings)
        {
            a = aSource[aPathIndex].ToList();
            b = bSource[bPathIndex].ToList();

            // Get our chord path storage sorted out.
            a_chordPaths = new Paths(2);
            b_chordPaths = new Paths(2);

            aChordLengths = new double[] { 0, 0 };
            bChordLengths = new double[] { 0, 0 };

            // Max and min indices for each path.
            // We'll use these for our chord within the edge checks.
            aPath_maxX_index = GeoWrangler.MaxX(a);
            bPath_maxY_index = GeoWrangler.MaxY(b);

            // Set up KDTrees for edge inspection.
            aTree = new KDTree<IntPoint>(2, a.Count);
            for (int aPt = 0; aPt < a.Count; aPt++)
            {
                aTree.AddPoint(new double[] { a[aPt].X, a[aPt].Y }, new IntPoint(a[aPt].X, a[aPt].Y));
            }
            bTree = new KDTree<IntPoint>(2, b.Count);
            for (int bPt = 0; bPt < b.Count; bPt++)
            {
                bTree.AddPoint(new double[] { b[bPt].X, b[bPt].Y }, new IntPoint(b[bPt].X, b[bPt].Y));
            }

            if (simulationSettings.getValue(EntropySettings.properties_i.subMode) != (int)CommonVars.chordCalcElements.b)
            {
                try
                {
                    doPathA();
                }
                catch (Exception)
                {
                }
            }

            if (simulationSettings.getValue(EntropySettings.properties_i.subMode) >= (int)CommonVars.chordCalcElements.b)
            {
                try
                {
                    doPathB();
                }
                catch (Exception)
                {
                }
            }

            double cutOffValue = simulationSettings.getResolution() * CentralProperties.scaleFactorForOperation; // arbitrary cut-off since rounding errors don't always mean 0 for a glancing contact.

            for (int r = 0; r < 2; r++)
            {
                if (aChordLengths[r] <= cutOffValue)
                {
                    aChordLengths[r] = 0;
                }

                if (bChordLengths[r] <= cutOffValue)
                {
                    bChordLengths[r] = 0;
                }
            }

            // So this is where things get a little tricky. It can be the case that we have 'left/right' chords reported for only one of top/bottom, and vice versa.
            // This is a natural consequence of the line clipping and we need to manage the output accordingly.
            // Things otherwise break quite badly because we end up reporting invalid chords (see the chord_4 test case)
            // As such, we need to perform some inspection and tag certain configurations as invalid.
            // Note that this cross-setting can be confusing at first glance.

            bool aChordsValid = true;
            bool bChordsValid = true;

            // We also need to be mindful of the user option for which chords to inspect. We should only need to wrangle this in case all chords are being requested.
            if (simulationSettings.getValue(EntropySettings.properties_i.subMode) > (int)CommonVars.chordCalcElements.b)
            {
                /*
                 * If top > 0 and bottom == 0, can't have left or right chords - no bisection
                 * If top == 0 and bottom > 0, can't have left or right chords - no bisection.
                 * If top == 0 and bottom == 0, can have left or right chords.
                 * 
                 */

                if (
                    ((aChordLengths[0] > 0) && (aChordLengths[1] == 0)) ||
                    ((aChordLengths[1] > 0) && (aChordLengths[0] == 0))
                   )

                {
                    bChordsValid = false;
                }
                else
                {
                    if ((aChordLengths[0] == 0) && (aChordLengths[1] == 0))
                    {
                        aChordsValid = false;
                    }
                }
            }

            if (!aChordsValid)
            {
                a_chordPaths[0] = new Path();
                a_chordPaths[0].Add(new IntPoint(0, 0));
                a_chordPaths[1] = new Path();
                a_chordPaths[1].Add(new IntPoint(0, 0));
                aChordLengths = new double[] { 0, 0 };
            }

            if (!bChordsValid)
            {
                b_chordPaths[0] = new Path();
                b_chordPaths[0].Add(new IntPoint(0, 0));
                b_chordPaths[1] = new Path();
                b_chordPaths[1].Add(new IntPoint(0, 0));
                bChordLengths = new double[] { 0, 0 };
            }
        }
    }
}
