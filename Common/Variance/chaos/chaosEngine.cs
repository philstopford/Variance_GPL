using ClipperLib;
using geoWrangler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Variance
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    class ChaosEngine
    {
        Paths listOfOutputPoints;
        public Paths getPaths()
        {
            return pGetPaths();
        }

        Paths pGetPaths()
        {
            return listOfOutputPoints;
        }

        string result;
        public string getResult()
        {
            return pGetResult();
        }

        string pGetResult()
        {
            return result;
        }

        bool outputValid;
        public bool isValid()
        {
            return pIsValid();
        }

        bool pIsValid()
        {
            return outputValid;
        }

        List<PreviewShape> simShapes;
        Paths[] booleanPaths;
        bool[] inputLayerEnabled;

        List<Paths> preFlight(Paths aPath, Paths bPath)
        {
            // Put 0-index point at minX (see method for more notes)
            int aCount = aPath.Count;
#if CHAOSTHREADED
            Parallel.For(0, aCount, (path) =>
#else
            for (Int32 path = 0; path < aCount; path++)
#endif
            {
                aPath[path] = reOrderPath("A", path);
            }
#if CHAOSTHREADED
            );
#endif
            // Put 0-index point at minY (see method for more notes)
            int bCount = bPath.Count;
#if CHAOSTHREADED
            Parallel.For(0, bCount, (path) =>
#else
            for (Int32 path = 0; path < bCount; path++)
#endif
            {
                bPath[path] = reOrderPath("B", path);
            }
#if CHAOSTHREADED
            );
#endif
            List<Paths> returnPaths = new List<Paths>();
            returnPaths.Add(aPath.ToList());
            returnPaths.Add(bPath.ToList());

            return returnPaths;
        }

        Path reOrderPath(string shapeRef, Int32 pathIndex)
        {
            Path sourcePath = new Path();
            Path returnPath = new Path();
            if ((shapeRef.ToUpper() != "A") && (shapeRef.ToUpper() != "B"))
            {
                // Bad callsite. Throw exception.
                throw (new Exception("reOrderPath: No shapeRef supplied!"));
            }
            else
            {
                if (shapeRef.ToUpper() == "A")
                {
                    sourcePath = booleanPaths[0][pathIndex].ToList();
                }
                else
                {
                    sourcePath = booleanPaths[1][pathIndex].ToList();
                }

                returnPath = GeoWrangler.clockwiseAndReorder(sourcePath);

            }
            return returnPath;
        }

        public static Paths customBoolean(int firstLayerOperator, Paths firstLayer, int secondLayerOperator, Paths secondLayer, int booleanFlag, double resolution)
        {
            return pCustomBoolean(firstLayerOperator, firstLayer, secondLayerOperator, secondLayer, booleanFlag, resolution);
        }

        static Paths pCustomBoolean(int firstLayerOperator, Paths firstLayer, int secondLayerOperator, Paths secondLayer, int booleanFlag, double resolution)
        {
            // In principle, 'rigorous' handling is only needed where the cutter is fully enclosed by the subject polygon.
            // The challenge is to know whether this is the case or not.
            // Possibility would be an intersection test and a vertex count and location comparison from before and after, to see whether anything changed.
            bool rigorous = GeoWrangler.enclosed(firstLayer, secondLayer); // this is not a strict check because the enclosure can exist either way for this situation.

            // Need a secondary check because keyholed geometry could be problematic.
            // Both paths will be reviewed; first one to have a keyhole will trigger the rigorous process.
            if (!rigorous)
            {
                try
                {
                    rigorous = GeoWrangler.enclosed(firstLayer, customSizing: 1, strict: true); // force a strict check.

                    if (!rigorous)
                    {
                        // Need a further check because keyholed geometry in B could be problematic.
                        rigorous = GeoWrangler.enclosed(secondLayer, customSizing: 1, strict: true); // force a strict check.
                    }
                }
                catch (Exception)
                {
                    // No big deal - carry on.
                }
            }

            Paths ret;

            ret = layerBoolean(firstLayerOperator, firstLayer, secondLayerOperator, secondLayer, booleanFlag, preserveColinear: true);

            ret = GeoWrangler.gapRemoval(ret).ToList();

            bool holes = false;
            bool gwHoles = false;

            for (int i = 0; i < ret.Count; i++)
            {
                holes = !Clipper.Orientation(ret[i]);
                gwHoles = !GeoWrangler.isClockwise(ret[i]);
                if (holes != gwHoles)
                {
                }
                if (holes)
                {
                    break;
                }
            }

            // Apply the keyholing and rationalize.
            if (holes)
            {
                Paths merged = new Paths();
                Fragmenter f = new Fragmenter(resolution * CentralProperties.scaleFactorForOperation);
                ret = f.fragmentPaths(ret);
                merged = GeoWrangler.makeKeyHole(ret);

                int count = merged.Count;
#if CHAOSTHREADED
                Parallel.For(0, count, (i) =>
#else
                for (int i = 0; i < count; i++)
#endif
                {
                    merged[i] = GeoWrangler.clockwise(merged[i]);
                }
#if CHAOSTHREADED
                );
#endif
                // Squash any accidental keyholes - not ideal, but best option found so far.
                Clipper c1 = new Clipper();
                c1.PreserveCollinear = true;
                c1.AddPaths(merged, PolyType.ptSubject, true);
                c1.Execute(ClipType.ctUnion, ret);
            }

            ret = GeoWrangler.sliverRemoval(ret); // experimental to try and remove any slivers.

            if (rigorous && !holes)
            {
                int count = ret.Count;
#if CHAOSTHREADED
                Parallel.For(0, count, (i) =>
#else
                for (int i = 0; i < count; i++)
#endif
                {
                    ret[i] = GeoWrangler.clockwise(ret[i]);
                    ret[i] = GeoWrangler.close(ret[i]);
                }
#if CHAOSTHREADED
                );
#endif
                // Return here because the attempt to rationalize the geometry below also screws things up, it seems.
                return ret;
            }

            IntRect bounds = Clipper.GetBounds(ret);

            Path bound = new Path();
            bound.Add(new IntPoint(bounds.left, bounds.bottom));
            bound.Add(new IntPoint(bounds.left, bounds.top));
            bound.Add(new IntPoint(bounds.right, bounds.top));
            bound.Add(new IntPoint(bounds.right, bounds.bottom));
            bound.Add(new IntPoint(bounds.left, bounds.bottom));

            Clipper c = new Clipper();

            c.AddPaths(ret, PolyType.ptSubject, true);
            c.AddPath(bound, PolyType.ptClip, true);

            Paths simple = new Paths();
            c.Execute(ClipType.ctIntersection, simple);

            return GeoWrangler.clockwiseAndReorder(simple);
        }

        Paths layerBoolean(EntropySettings simulationSettings, Int32 firstLayer, Int32 secondLayer, int booleanFlag, bool preserveColinear = true)
        {
            Paths firstLayerPaths = GeoWrangler.pathsFromPointFs(simShapes[firstLayer].getPoints(), CentralProperties.scaleFactorForOperation);

            Paths secondLayerPaths = GeoWrangler.pathsFromPointFs(simShapes[secondLayer].getPoints(), CentralProperties.scaleFactorForOperation);

            return layerBoolean(simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, firstLayer), firstLayerPaths,
                        simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, secondLayer), secondLayerPaths, booleanFlag, preserveColinear);
        }

        static Paths layerBoolean(int firstLayerOperator, Paths firstLayerPaths, int secondLayerOperator, Paths secondLayerPaths, int booleanFlag, bool preserveColinear)
        {
            if (firstLayerOperator == 1) // NOT layer handling
            {
                try
                {
                    firstLayerPaths = GeoWrangler.invertTone(firstLayerPaths, false).ToList();
                }
                catch (Exception)
                {
                    // Something blew up.
                }
                firstLayerPaths[0] = GeoWrangler.close(firstLayerPaths[0]);
            }


            if (secondLayerOperator == 1) // NOT layer handling
            {
                try
                {
                    secondLayerPaths = GeoWrangler.invertTone(secondLayerPaths, false).ToList();
                }
                catch (Exception)
                {
                    // Something blew up.
                }
                secondLayerPaths[0] = GeoWrangler.close(secondLayerPaths[0]);
            }

            if (firstLayerPaths[0].Count() <= 1)
            {
                return secondLayerPaths.ToList();
            }
            if (secondLayerPaths[0].Count() <= 1)
            {
                return firstLayerPaths.ToList();
            }

            return layerBoolean(firstLayerPaths, secondLayerPaths, booleanFlag, preserveColinear: preserveColinear);
        }

        static Paths layerBoolean(Paths firstPaths, Paths secondPaths, int booleanFlag, bool preserveColinear = true)
        {
            string booleanType = "AND";
            if (booleanFlag == 1)
            {
                booleanType = "OR";
            }

            Clipper c = new Clipper();
            c.PreserveCollinear = preserveColinear; // important - if we don't do this, we lose the fragmentation on straight edges.

            c.AddPaths(firstPaths, PolyType.ptSubject, true);
            c.AddPaths(secondPaths, PolyType.ptClip, true);

            Paths outputPoints = new Paths();

            if (booleanType == "AND")
            {
                c.Execute(ClipType.ctIntersection, outputPoints, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
            }

            if (booleanType == "OR")
            {
                c.Execute(ClipType.ctUnion, outputPoints, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);
            }
            return outputPoints; // Return our first list of points as the result of the boolean.
        }

        Paths[] layerBoolean(CommonVars commonVars, bool preserveColinear = true)
        {
            // Boolean is structured as:
            // Process two layers to get the interaction of two layers.
            // Process each pair of results for the output of 4 layers
            // Take each pair of results and get the combination of 8 layers.

            EntropySettings simulationSettings = commonVars.getSimulationSettings();

            int limit2 = simulationSettings.getOperator(EntropySettings.properties_o.twoLayer).Length;
            int limit4 = limit2 / 2;
            int limit8 = limit4 / 2;

            Paths[] twoLayerResults = new Paths[limit2];
            Paths[] fourLayerResults = new Paths[limit4];
            Paths[] eightLayerResults = new Paths[limit8];

            Path tPath = new Path();

#if CHAOSTHREADED
            Parallel.For(0, limit2, (i) =>
#else
            for (int i = 0; i < limit2; i++)
#endif
            {
                if (inputLayerEnabled[i * 2] && inputLayerEnabled[(i * 2) + 1])
                {
                    twoLayerResults[i] = layerBoolean(simulationSettings, i * 2, (i * 2) + 1, simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, i), preserveColinear: preserveColinear).ToList();
                }
                else
                {
                    if (inputLayerEnabled[i * 2] && !inputLayerEnabled[(i * 2) + 1])
                    {
                        twoLayerResults[i] = layerBoolean(simulationSettings, i * 2, i * 2, 0, preserveColinear: preserveColinear);
                    }
                    else if (!inputLayerEnabled[i * 2] && inputLayerEnabled[(i * 2) + 1])
                    {
                        twoLayerResults[i] = layerBoolean(simulationSettings, (i * 2) + 1, (i * 2) + 1, 0, preserveColinear: preserveColinear);
                    }
                    else
                    {
                        twoLayerResults[i] = new Paths();
                    }
                }
            }
#if CHAOSTHREADED
            );
#endif
            /* Direct the 4 layer boolean approach
             -2 : no active layers.
             -1 : only the left layer is enabled.
              0 : both layers are enabled.
              1 : only the right layer is enabled.
             */
            int[] doLayer4Boolean = new int[limit4];

#if CHAOSTHREADED
            Parallel.For(0, limit4, (i) =>
#else
            for (int i = 0; i < limit4; i++)
#endif
            {
                if (
                    (inputLayerEnabled[i * 4] || inputLayerEnabled[(i * 4) + 1]) &&
                    (inputLayerEnabled[(i * 4) + 2] || inputLayerEnabled[(i * 4) + 3])
                   )
                {
                    doLayer4Boolean[i] = 0;
                }
                else
                {
                    if (inputLayerEnabled[i * 4] || inputLayerEnabled[(i * 4) + 1])
                    {
                        doLayer4Boolean[i] = -1;
                    }
                    else if (inputLayerEnabled[(i * 4) + 2] || inputLayerEnabled[(i * 4) + 3])
                    {
                        doLayer4Boolean[i] = 1;
                    }
                    else
                    {
                        doLayer4Boolean[i] = -2;
                    }
                }
            }
#if CHAOSTHREADED
            );
#endif
#if CHAOSTHREADED
            Parallel.For(0, limit4, (i) =>
#else
            for (int i = 0; i < limit4; i++)
#endif
            {
                if ((doLayer4Boolean[i] == 0) && ((twoLayerResults[(i * 2)].Count > 0) && (twoLayerResults[(i * 2) + 1].Count > 0)))
                {
                    fourLayerResults[i] = layerBoolean(
                        firstPaths: twoLayerResults[(i * 2)],
                        secondPaths: twoLayerResults[(i * 2) + 1],
                        booleanFlag: simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, i),
                        preserveColinear: preserveColinear
                    ).ToList();
                }
                else
                {
                    switch (doLayer4Boolean[i])
                    {
                        case -1:
                            fourLayerResults[i] = twoLayerResults[(i * 2)].ToList();
                            break;
                        case 1:
                            fourLayerResults[i] = twoLayerResults[(i * 2) + 1].ToList();
                            break;
                        case 0:
                            if (twoLayerResults[(i * 2)].Count > 0)
                            {
                                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, i) == 0)
                                {
                                    fourLayerResults[i] = new Paths();
                                }
                                else
                                {
                                    fourLayerResults[i] = twoLayerResults[i * 2].ToList();
                                }
                            }
                            else if (twoLayerResults[(i * 2) + 1].Count > 0)
                            {
                                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, i) == 0)
                                {
                                    fourLayerResults[i] = new Paths();
                                }
                                else
                                {
                                    fourLayerResults[i] = twoLayerResults[(i * 2) + 1].ToList();
                                }
                            }
                            else
                            {
                                fourLayerResults[i] = new Paths();
                            }
                            break;
                        default:
                            fourLayerResults[i] = new Paths();
                            break;
                    }
                }
            }
#if CHAOSTHREADED
            );
#endif
            /* Direct the 8 layer boolean approach
             -2 : no active layers.
             -1 : only the left layer is enabled.
              0 : both layers are enabled.
              1 : only the right layer is enabled.
             */
            int[] doLayer8Boolean = new int[limit8];

#if CHAOSTHREADED
            Parallel.For(0, limit8, (i) =>
#else
            for (int i = 0; i < limit8; i++)
#endif
            {
                // Are both sides active?
                // 0th loop : 0   1        5
                // next     : 8   9
                // next     : 16  17
                if (
                    (
                     (inputLayerEnabled[i * 8] || inputLayerEnabled[(i * 8) + 1]) ||
                     (inputLayerEnabled[(i * 8) + 2] || inputLayerEnabled[(i * 8) + 3])
                    ) &
                    (
                     (inputLayerEnabled[(i * 8) + 4] || inputLayerEnabled[(i * 8) + 5]) ||
                     (inputLayerEnabled[(i * 8) + 6] || inputLayerEnabled[(i * 8) + 7])
                    )
                   )
                {
                    doLayer8Boolean[i] = 0;
                }
                else
                {
                    if (
                         (inputLayerEnabled[i * 8] || inputLayerEnabled[(i * 8) + 1]) ||
                         (inputLayerEnabled[(i * 8) + 2] || inputLayerEnabled[(i * 8) + 3])
                       )
                    {
                        doLayer8Boolean[i] = -1;
                    }
                    else if (
                         (inputLayerEnabled[(i * 8) + 4] || inputLayerEnabled[(i * 8) + 5]) ||
                         (inputLayerEnabled[(i * 8) + 6] || inputLayerEnabled[(i * 8) + 7])
                       )
                    {
                        doLayer8Boolean[i] = 1;
                    }
                    else
                    {
                        doLayer8Boolean[i] = -2;
                    }
                }
            }
#if CHAOSTHREADED
            );
#endif

#if CHAOSTHREADED
            Parallel.For(0, limit8, (i) =>
#else
            for (int i = 0; i < limit8; i++)
#endif
            {
                if ((doLayer8Boolean[i] == 0) && ((fourLayerResults[(i * 2)].Count > 0) && (fourLayerResults[(i * 2) + 1].Count > 0)))
                {
                    eightLayerResults[i] = layerBoolean(
                        firstPaths: fourLayerResults[(i * 2)],
                        secondPaths: fourLayerResults[(i * 2) + 1],
                        booleanFlag: simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, i),
                        preserveColinear: preserveColinear
                    ).ToList();
                }
                else
                {
                    switch (doLayer8Boolean[i])
                    {
                        case -1:
                            eightLayerResults[i] = fourLayerResults[(i * 2)].ToList();
                            break;
                        case 1:
                            eightLayerResults[i] = fourLayerResults[(i * 2) + 1].ToList();
                            break;
                        case 0:
                            if (fourLayerResults[(i * 2)].Count > 0)
                            {
                                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, i) == 0)
                                {
                                    eightLayerResults[i] = new Paths();
                                    eightLayerResults[i].Add(tPath);
                                }
                                else
                                {
                                    eightLayerResults[i] = fourLayerResults[i * 2].ToList();
                                }
                            }
                            else if (fourLayerResults[(i * 2) + 1].Count > 0)
                            {
                                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, i) == 0)
                                {
                                    eightLayerResults[i] = new Paths();
                                    eightLayerResults[i].Add(tPath);
                                }
                                else
                                {
                                    eightLayerResults[i] = fourLayerResults[(i * 2) + 1].ToList();
                                }
                            }
                            else
                            {
                                eightLayerResults[i] = new Paths();
                                eightLayerResults[i].Add(tPath);
                            }
                            break;
                        default:
                            eightLayerResults[i] = new Paths();
                            eightLayerResults[i].Add(tPath);
                            break;
                    }
                }
            }
#if CHAOSTHREADED
            );
#endif
            return eightLayerResults;
        }

        // Preview mode is intended to allow multi-threaded evaluation for a single case - batch calculations run multiple separate single-threaded evaluations
        public ChaosEngine(CommonVars commonVars, List<PreviewShape> simShapes, bool previewMode)
        {
            pChaosEngine(commonVars, simShapes, previewMode);
        }

        void pChaosEngine(CommonVars commonVars, List<PreviewShape> simShapes, bool previewMode)
        {
            outputValid = false;
            this.simShapes = simShapes;

            listOfOutputPoints = new Paths();

            EntropySettings simulationSettings = commonVars.getSimulationSettings();

            bool sgRemove_a = false;
            bool sgRemove_b = false;

            inputLayerEnabled = new bool[CentralProperties.maxLayersForMC];
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                inputLayerEnabled[i] = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                // Modify our state based on the omit flag (in case this layer is being used by an in-layer boolean elsewhere and the user requested to omit the input layer.
                inputLayerEnabled[i] = inputLayerEnabled[i] && (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.omit) == 0);
                if (!sgRemove_a)
                {
                    if (i < CentralProperties.maxLayersForMC / 2)
                    {
                        sgRemove_a = (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.BOOLEAN);
                    }
                }
                if (!sgRemove_b)
                {
                    if (i >= CentralProperties.maxLayersForMC / 2)
                    {
                        sgRemove_b = (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.BOOLEAN);
                    }
                }
            }

            bool preserveColinear = (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.oType) == (Int32)CommonVars.calcModes.enclosure_spacing_overlap);

            booleanPaths = layerBoolean(commonVars, preserveColinear);

            if (sgRemove_a)
            {
                booleanPaths[0] = GeoWrangler.sliverGapRemoval(booleanPaths[0]);
            }
            if (sgRemove_b)
            {
                booleanPaths[1] = GeoWrangler.sliverGapRemoval(booleanPaths[1]);
            }

            Int32 layerAPathCount_orig = booleanPaths[0].Count();
            Int32 layerBPathCount_orig = booleanPaths[1].Count();

            // Let's validate that we have something reasonable for the inputs before we do something with them.
            bool inputsValid = true;
            if ((layerAPathCount_orig == 0) || (layerBPathCount_orig == 0))
            {
                // Assuming failSafe for each layer, we'd have one point per failed layer, so we fail for anything below or equal to 2. We also can't make a polygon from 2 points.
                inputsValid = false;
            }

            if (inputsValid)
            {
                switch (simulationSettings.getValue(EntropySettings.properties_i.oType))
                {
                    case (int)CommonVars.calcModes.area: // area
                        try
                        {
                            bool perPoly = false;
                            if (simulationSettings.getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.areaCalcModes.perpoly)
                            {
                                perPoly = true;
                            }
                            AreaHandler aH = new AreaHandler(aPaths: booleanPaths[0], bPaths: booleanPaths[1], maySimplify: true, perPoly);
                            // Sum the areas by polygon returned.
                            result = (Convert.ToDouble(result) + aH.area).ToString();
                            listOfOutputPoints.AddRange(aH.listOfOutputPoints);
                            outputValid = true;
                        }
                        catch (Exception)
                        {
                            // rejected case - don't care.
                        }
                        break;

                    case (int)CommonVars.calcModes.enclosure_spacing_overlap: // spacing (or enclosure)
                        DistanceHandler dH = new DistanceHandler(aPaths: booleanPaths[0], bPaths: booleanPaths[1], simulationSettings, previewMode); // in preview mode, raycaster inside this engine will run threaded along the emit edge.

                        // Store minimum case for the per polygon system.
                        if (result == null)
                        {
                            result = dH.distanceString;
                            listOfOutputPoints = dH.resultPaths.ToList();
                        }
                        else
                        {
                            // Overlaps are reported as negative values, so this will handle both spacing and overlap cases.
                            if (Convert.ToDouble(dH.distanceString) < Convert.ToDouble(result))
                            {
                                result = dH.distanceString;
                                listOfOutputPoints = dH.resultPaths.ToList();
                            }
                        }

                        // Viewport needs a polygon - lines aren't handled properly, so let's double up our line.
                        try
                        {
                            for (int poly = 0; poly < listOfOutputPoints.Count; poly++)
                            {
                                Int32 pt = listOfOutputPoints[poly].Count - 1;
                                while (pt > 0)
                                {
                                    listOfOutputPoints[poly].Add(new IntPoint(listOfOutputPoints[poly][pt]));
                                    pt--;
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                        outputValid = true;

                        break;

                    case (int)CommonVars.calcModes.chord: // chord
                        if (result == null)
                        {
                            result = "0.0,0.0,0.0,0.0";
                        }

                        double[] fraggedResult = new double[4];
                        fraggedResult[0] = fraggedResult[1] = fraggedResult[2] = fraggedResult[3] = 0.0;

                        Path tmpPath = new Path();
                        tmpPath.Add(new IntPoint(0, 0));
                        listOfOutputPoints.Add(tmpPath.ToList());
                        listOfOutputPoints.Add(tmpPath.ToList());
                        listOfOutputPoints.Add(tmpPath.ToList());
                        listOfOutputPoints.Add(tmpPath.ToList());

                        try
                        {
                            Paths aPath = booleanPaths[0].ToList();
                            Paths bPath = booleanPaths[1].ToList();
                            List<Paths> cleanedPaths = preFlight(aPath, bPath).ToList();
                            aPath = cleanedPaths[0].ToList();
                            bPath = cleanedPaths[1].ToList();
                            ChordHandler cH = new ChordHandler(aPath, bPath, simulationSettings);

                            // Fragment our result.
                            char[] resultSeparators = new char[] { ',' }; // CSV separator for splitting results for comparison.
                            string[] tmpfraggedResult = result.Split(resultSeparators);
#if CHAOSTHREADED
                            Parallel.For(0, tmpfraggedResult.Length, (i) =>
#else
                            for (Int32 i = 0; i < tmpfraggedResult.Length; i++)
#endif
                            {
                                fraggedResult[i] = Convert.ToDouble(tmpfraggedResult[i]);
                            }
#if CHAOSTHREADED
                            );
#endif
                            fraggedResult[0] = cH.aChordLengths[0] / CentralProperties.scaleFactorForOperation;
                            listOfOutputPoints[0] = cH.a_chordPaths[0].ToList();
                            fraggedResult[1] = cH.aChordLengths[1] / CentralProperties.scaleFactorForOperation;
                            listOfOutputPoints[1] = cH.a_chordPaths[1].ToList();
                            fraggedResult[2] = cH.bChordLengths[0] / CentralProperties.scaleFactorForOperation;
                            listOfOutputPoints[2] = cH.b_chordPaths[0].ToList();
                            fraggedResult[3] = cH.bChordLengths[1] / CentralProperties.scaleFactorForOperation;
                            listOfOutputPoints[3] = cH.b_chordPaths[1].ToList();

                            if (simulationSettings.getValue(EntropySettings.properties_i.subMode) != (int)CommonVars.chordCalcElements.b)
                            {
                                result = fraggedResult[0].ToString() + "," + fraggedResult[1].ToString();
                            }
                            else
                            {
                                result = "N/A,N/A";
                            }

                            if (simulationSettings.getValue(EntropySettings.properties_i.subMode) >= (int)CommonVars.chordCalcElements.b)
                            {
                                result += "," + fraggedResult[2].ToString() + "," + fraggedResult[3].ToString();
                            }
                            else
                            {
                                result += ",N/A,N/A";
                            }
                            outputValid = true;
                        }
                        catch (Exception)
                        {
                            // We don't care about exceptions - these are probably rejected cases from coincident edges.
                        }
                        break;

                    case (int)CommonVars.calcModes.angle: // angle
                        for (Int32 layerAPoly = 0; layerAPoly < layerAPathCount_orig; layerAPoly++)
                        {
                            for (Int32 layerBPoly = 0; layerBPoly < layerBPathCount_orig; layerBPoly++)
                            {
                                try
                                {
                                    angleHandler agH = new angleHandler(layerAPath: booleanPaths[0], layerBPath: booleanPaths[1]);
                                    if (result == null)
                                    {
                                        result = agH.minimumIntersectionAngle.ToString();
                                        listOfOutputPoints = agH.resultPaths.ToList();
                                    }
                                    else
                                    {
                                        if (agH.minimumIntersectionAngle < Convert.ToDouble(result))
                                        {
                                            result = agH.minimumIntersectionAngle.ToString();
                                            listOfOutputPoints.Clear();
                                            listOfOutputPoints = agH.resultPaths.ToList();
                                        }
                                    }
                                    outputValid = true; // mark that we're good for the callsite
                                }
                                catch (Exception)
                                {
                                    // rejected case.
                                }
                            }
                        }
                        break;
                }
            }
            else
            {
                outputValid = false;
                // Need to return empty values.
            }
        }
    }
}
