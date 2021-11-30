
using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;

namespace Variance;

using Paths = List<List<IntPoint>>;

internal class AreaHandler
{
    public double area { get; private set; }
    public Paths listOfOutputPoints { get; private set; }

    private void ZFillCallback(IntPoint bot1, IntPoint top1, IntPoint bot2, IntPoint top2, ref IntPoint pt)
    {
        pt.Z = -1; // Tag our intersection points.
    }

    public AreaHandler(Paths aPaths, Paths bPaths, bool maySimplify, bool perPoly, double scaleFactorForPointF = CentralProperties.scaleFactorForOperation)
    {
        areaHandlerLogic(aPaths, bPaths, scaleFactorForPointF, maySimplify, perPoly);
    }

    private void areaHandlerLogic(Paths aPaths, Paths bPaths, double scaleFactorForPointF, bool maySimplify, bool perPoly)
    {
        Paths tmpPaths = new();
        listOfOutputPoints = new Paths();

        // callsite may not want simplified geometry.
        Clipper c = new() {PreserveCollinear = !maySimplify, ZFillFunction = ZFillCallback};

        c.AddPaths(aPaths, PolyType.ptSubject, true);

        c.AddPaths(bPaths, PolyType.ptClip, true);

        // Boolean AND of the two levels for the area operation.
        try
        {
            c.Execute(ClipType.ctIntersection, tmpPaths); //, firstLayerFillType, secondLayerFillType);
        }
        catch (Exception)
        {
            // Will handle downstream.
        }

        double tmpVal = 0.0;
        if (perPoly)
        {
            tmpVal = -1.0f;
        }

        int polyCount = tmpPaths.Count;
        for (int poly = 0; poly < polyCount; poly++)
        {
            if (perPoly)
            {
                double tmpVal2 = Clipper.Area(tmpPaths[poly]);
                if (!(tmpVal <= -0.0001f) && !(tmpVal2 < tmpVal))
                {
                    continue;
                }

                tmpVal = tmpVal2;
                listOfOutputPoints.Clear();
                listOfOutputPoints.Add(tmpPaths[poly].ToList());
            }
            else
            {
                tmpVal += Clipper.Area(tmpPaths[poly]);
                // Append the result output to the resultPoints list.
                listOfOutputPoints.Add(tmpPaths[poly].ToList());
            }
        }
        // Sum the areas by polygon.
        area = tmpVal / (scaleFactorForPointF * scaleFactorForPointF);
    }
}