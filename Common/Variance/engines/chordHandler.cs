using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib1;
using geoWrangler;
using KDTree;

namespace Variance;

using Path = List<IntPoint>;
using Paths = List<List<IntPoint>>;

internal class ChordHandler
{
    private Path a;
    private Path b;
    public Paths a_chordPaths { get; private set; }// 0 is top, 1 is bottom
    public Paths b_chordPaths { get; private set; } // 0 is left, 1 is right
    public double[] aChordLengths { get; private set; }
    public double[] bChordLengths { get; private set; }
    private int aPath_maxX_index;
    private int bPath_maxY_index;
    private int aPath_minX_index;
    private int bPath_minY_index;
    private KDTree<IntPoint> aTree;
    private KDTree<IntPoint> bTree;

    private void ZFillCallback(IntPoint bot1, IntPoint top1, IntPoint bot2, IntPoint top2, ref IntPoint pt)
    {
        pt.Z = -1; // Tag our intersection points.
    }

    private void doPathA()
    {
        aChordLengths[0] = 0.0;
        aChordLengths[1] = 0.0;

        int pt = 0; // will set based on start condition checks

        Path testPath = new();
        // Gather all of our points on the top edge for the nearest neighbor search
        while (pt <= aPath_maxX_index)
        {
            testPath.Add(new IntPoint(a[pt]));
            pt++;
        }

        Clipper c = new() {ZFillFunction = ZFillCallback, PreserveCollinear = true};
        c.AddPath(testPath, PolyType.ptSubject, false);
        c.AddPath(b, PolyType.ptClip, true);
        PolyTree polyTree_bottom = new();
        c.Execute(ClipType.ctIntersection, polyTree_bottom);
        Paths bottomChords = Clipper.OpenPathsFromPolyTree(polyTree_bottom); // potentially more than one chord.

        // Now we evaluate the lower edge
        // We have to start from the max index because it could be shared between top and bottom chords.
        pt = aPath_maxX_index;
        testPath.Clear();
        c.Clear();

        bool aDone = false;
            
        while (!aDone)
        {
            pt %= a.Count;
            testPath.Add(new IntPoint(a[pt]));
            if (pt == aPath_minX_index)
            {
                aDone = true;
            }
            pt++;
        }

        c.AddPath(testPath, PolyType.ptSubject, false);
        c.AddPath(b, PolyType.ptClip, true);
        PolyTree polyTree_top = new();
        c.Execute(ClipType.ctIntersection, polyTree_top);
        Paths topChords = Clipper.OpenPathsFromPolyTree(polyTree_top); // potentially more than one chord.

        // Now let's see what we have.

        double minBottomChordLength = 0;
        Path bottomChord = new() {new IntPoint(0, 0)};
        // safety in case we have no chords on the top.
        foreach (Path t in bottomChords)
        {
            // Does this chord segment actually belong to the 'B' geometry.
            bool bottomEdgeIsFromA = true;
            // First point and last point might not be matched to original geometry (imperfect intersection)
            for (int bCPt = 1; bCPt < t.Count - 1; bCPt++)
            {
                NearestNeighbour<IntPoint> pIter = aTree.NearestNeighbors(new double[] { t[bCPt].X, t[bCPt].Y }, 1);
                while (pIter.MoveNext())
                {
                    if (!(pIter.CurrentDistance > 0))
                    {
                        continue;
                    }

                    bottomEdgeIsFromA = false;
                    break;
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
            if (bottomChords.Count != 1 && (bottomChords.Count <= 1 || t[0].Z != -1 || t[^1].Z != -1))
            {
                continue;
            }

            double chordLength = 0;
            // Assess length of each chord and only report the minimum length one.
            for (int chordpt = 0; chordpt < t.Count - 1; chordpt++)
            {
                chordLength += GeoWrangler.distanceBetweenPoints(t[chordpt], t[chordpt + 1]);
            }

            if (minBottomChordLength != 0 && !(chordLength < minBottomChordLength))
            {
                continue;
            }

            minBottomChordLength = chordLength;
            bottomChord = t;
        }

        double minTopChordLength = 0;
        Path topChord = new() {new IntPoint(0, 0)};
        // safety in case we have no chords on the top.
        foreach (Path t in topChords)
        {
            // Does this chord segment actually belong to the 'B' geometry.
            bool topEdgeIsFromA = true;
            // First point and last point might not be matched to original geometry (imperfect intersection)
            for (int tCPt = 1; tCPt < t.Count - 1; tCPt++)
            {
                NearestNeighbour<IntPoint> pIter = aTree.NearestNeighbors(new double[] { t[tCPt].X, t[tCPt].Y }, 1);
                while (pIter.MoveNext())
                {
                    if (!(pIter.CurrentDistance > 0))
                    {
                        continue;
                    }

                    topEdgeIsFromA = false;
                    break;
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
            if (topChords.Count != 1 && (topChords.Count <= 1 || t[0].Z != -1 || t[^1].Z != -1))
            {
                continue;
            }

            double chordLength = 0;
            // Assess length of each chord and only report the minimum length one.
            for (int chordpt = 0; chordpt < t.Count - 1; chordpt++)
            {
                chordLength += GeoWrangler.distanceBetweenPoints(t[chordpt], t[chordpt + 1]);
            }

            if (minTopChordLength != 0 && !(chordLength < minTopChordLength))
            {
                continue;
            }

            minTopChordLength = chordLength;
            topChord = t;
        }

        aChordLengths[0] = minTopChordLength;
        a_chordPaths.Add(topChord.ToList());

        aChordLengths[1] = minBottomChordLength;
        a_chordPaths.Add(bottomChord.ToList());
    }

    private void doPathB()
    {
        bChordLengths[0] = 0.0;
        bChordLengths[1] = 0.0;

        int pt = 0; // will set based on start condition checks

        Path testPath = new();

        // Gather all of our points on the left edge
        while (pt <= bPath_maxY_index)
        {
            testPath.Add(new IntPoint(b[pt]));
            pt++;
        }

        pt = Math.Max(0, pt - 1);

        Clipper c = new() {PreserveCollinear = true, ZFillFunction = ZFillCallback};
        c.AddPath(testPath, PolyType.ptSubject, false);
        c.AddPath(a, PolyType.ptClip, true);
        PolyTree polyTree_right = new();
        c.Execute(ClipType.ctIntersection, polyTree_right);
        Paths rightChords = Clipper.OpenPathsFromPolyTree(polyTree_right); // potentially more than one chord.

        // Now we evaluate the right edge
        // We have to start from the max index because it could be shared between top and bottom chords.
        testPath.Clear();
        c.Clear();

        bool bDone = false;
            
        while (!bDone)
        {
            pt %= b.Count;
            testPath.Add(new IntPoint(b[pt]));
            if (pt == bPath_minY_index)
            {
                bDone = true;
            }
            pt++;
        }

        c.ZFillFunction = ZFillCallback;
        c.AddPath(testPath, PolyType.ptSubject, false);
        c.AddPath(a, PolyType.ptClip, true);
        PolyTree polyTree_left = new();
        c.Execute(ClipType.ctIntersection, polyTree_left);
        Paths leftChords = Clipper.OpenPathsFromPolyTree(polyTree_left); // potentially more than one chord.

        // Now let's see what we have.

        double minRightChordLength = 0;
        Path rightChord = new() {new IntPoint(0, 0)};
        // safety in case we have no chords on the right.

        foreach (Path t in rightChords)
        {
            // Does this chord segment actually belong to the 'B' geometry.
            bool rightEdgeIsFromB = true;
            // First point and last point might not be matched to original geometry (imperfect intersection)
            for (int rCPt = 1; rCPt < t.Count - 1; rCPt++)
            {
                NearestNeighbour<IntPoint> pIter = bTree.NearestNeighbors(new double[] { t[rCPt].X, t[rCPt].Y }, 1);
                while (pIter.MoveNext())
                {
                    if (!(pIter.CurrentDistance > 0))
                    {
                        continue;
                    }

                    rightEdgeIsFromB = false;
                    break;
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
            if (rightChords.Count != 1 && (rightChords.Count <= 1 || t[0].Z != -1 || t[^1].Z != -1))
            {
                continue;
            }

            double chordLength = 0;
            // Assess length of each chord and only report the minimum length one.
            for (int chordpt = 0; chordpt < t.Count - 1; chordpt++)
            {
                chordLength += GeoWrangler.distanceBetweenPoints(t[chordpt], t[chordpt + 1]);
            }

            if (minRightChordLength != 0 && !(chordLength < minRightChordLength))
            {
                continue;
            }

            minRightChordLength = chordLength;
            rightChord = t;
        }

        double minLeftChordLength = 0;
        Path leftChord = new() {new IntPoint(0, 0)};
        // safety in case we have no chords on the left.
        foreach (Path t in leftChords)
        {
            // Does this chord segment actually belong to the 'B' geometry.
            bool leftEdgeIsFromB = true;
            // First point and last point might not be matched to original geometry (imperfect intersection)
            for (int lCPt = 1; lCPt < t.Count - 1; lCPt++)
            {
                NearestNeighbour<IntPoint> pIter = bTree.NearestNeighbors(new double[] { t[lCPt].X, t[lCPt].Y }, 1);
                while (pIter.MoveNext())
                {
                    if (!(pIter.CurrentDistance > 0))
                    {
                        continue;
                    }

                    leftEdgeIsFromB = false;
                    break;
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
            if (leftChords.Count != 1 && (leftChords.Count <= 1 || t[0].Z != -1 || t[^1].Z != -1))
            {
                continue;
            }

            double chordLength = 0;
            // Assess length of each chord and only report the minimum length one.
            for (int chordpt = 0; chordpt < t.Count - 1; chordpt++)
            {
                chordLength += GeoWrangler.distanceBetweenPoints(t[chordpt], t[chordpt + 1]);
            }

            if (minLeftChordLength != 0 && !(chordLength < minLeftChordLength))
            {
                continue;
            }

            minLeftChordLength = chordLength;
            leftChord = t;
        }

        bChordLengths[0] = minLeftChordLength;
        b_chordPaths.Add(leftChord.ToList());

        bChordLengths[1] = minRightChordLength;
        b_chordPaths.Add(rightChord.ToList());

    }

    public ChordHandler(Paths aSource, Paths bSource, EntropySettings simulationSettings)
    {
        chordHandlerLogic(aSource, bSource, simulationSettings);
    }

    private void chordHandlerLogic(Paths aSource, Paths bSource, EntropySettings simulationSettings)
    {
        a_chordPaths = new Paths(2) {new() {new IntPoint(0, 0)}, new() {new IntPoint(0, 0)}};
        b_chordPaths = new Paths(2) {new() {new IntPoint(0, 0)}, new() {new IntPoint(0, 0)}};
        aChordLengths = new[] {0.0, 0.0};
        bChordLengths = new[] {0.0, 0.0};

        List<ChordHandler> cHList = new();
        for (int aIndex = 0; aIndex < aSource.Count; aIndex++)
        {
            for (int bIndex = 0; bIndex < bSource.Count; bIndex++)
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

        foreach (ChordHandler t in cHList)
        {
            for (int resultIndex = 0; resultIndex < 2; resultIndex++)
            {
                // Extract 'a' results only if non-zero
                if (t.aChordLengths[resultIndex] != 0.0)
                {
                    // We have either our first non-zero chord length or a chord length lower than previously recorded.
                    if (aChordLengths[resultIndex] == 0.0 || t.aChordLengths[resultIndex] < aChordLengths[resultIndex])
                    {
                        aChordLengths[resultIndex] = t.aChordLengths[resultIndex];
                        a_chordPaths[resultIndex] = t.a_chordPaths[resultIndex].ToList();
                    }
                }

                // Extract 'b' results only if non-zero
                if (t.bChordLengths[resultIndex] == 0.0 || bChordLengths[resultIndex] != 0.0 &&
                    !(t.bChordLengths[resultIndex] <
                      bChordLengths[resultIndex]))
                {
                    continue;
                }

                // We have either our first non-zero chord length or a chord length lower than previously recorded.
                bChordLengths[resultIndex] = t.bChordLengths[resultIndex];
                b_chordPaths[resultIndex] = t.b_chordPaths[resultIndex].ToList();
            }
        }
    }

    private ChordHandler(int aPathIndex, Paths aSource, int bPathIndex, Paths bSource, EntropySettings simulationSettings)
    {
        pChordHandlerLogic(aPathIndex, aSource, bPathIndex, bSource, simulationSettings);
    }

    private void pChordHandlerLogic(int aPathIndex, Paths aSource, int bPathIndex, Paths bSource, EntropySettings simulationSettings)
    {
        a = aSource[aPathIndex].ToList();
        b = bSource[bPathIndex].ToList();

        // Get our chord path storage sorted out.
        a_chordPaths = new Paths(2);
        b_chordPaths = new Paths(2);

        aChordLengths = new[] { 0.0, 0.0 };
        bChordLengths = new[] { 0.0, 0.0 };

        // Max and min indices for each path.
        // We'll use these for our chord within the edge checks.
        aPath_maxX_index = GeoWrangler.MaxX(a);
        bPath_maxY_index = GeoWrangler.MaxY(b);
        aPath_minX_index = GeoWrangler.MinX(a);
        bPath_minY_index = GeoWrangler.MinY(b);

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

        double cutOffValue = simulationSettings.getResolution() * CentralProperties.scaleFactorForOperation / 10; // arbitrary cut-off since rounding errors don't always mean 0 for a glancing contact.

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
                aChordLengths[0] > 0 && aChordLengths[1] == 0 ||
                aChordLengths[1] > 0 && aChordLengths[0] == 0
            )

            {
                bChordsValid = false;
            }
            else
            {
                if (aChordLengths[0] == 0 && aChordLengths[1] == 0)
                {
                    aChordsValid = false;
                }
            }
        }

        if (!aChordsValid)
        {
            a_chordPaths[0] = new Path {new(0, 0)};
            a_chordPaths[1] = new Path {new(0, 0)};
            aChordLengths = new double[] { 0, 0 };
        }

        if (bChordsValid)
        {
            return;
        }

        b_chordPaths[0] = new Path {new(0, 0)};
        b_chordPaths[1] = new Path {new(0, 0)};
        bChordLengths = new double[] { 0, 0 };
    }
}