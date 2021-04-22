using System;
using System.Collections.Generic;
using System.Linq;
using Error;
using geoLib;
using geoWrangler;
using utility;

namespace Variance
{
    public class EntropyShape
    {
        Fragmenter fragment;

        GeoLibPointF[] points;

        public GeoLibPointF[] getPoints()
        {
            return pGetPoints();
        }

        GeoLibPointF[] pGetPoints()
        {
            return points;
        }

        GeoLibPointF pivot;

        double globalBias_Sides, globalBias_Tips;

        MyVertex[] Vertex; // container for our extended point properties information.
        MyRound[] round1; // container for the rounding data.
        Boolean[] tips; // large array to hold tip information.

        class BoundingBox
        {
            GeoLibPointF midPoint;
            public GeoLibPointF getMidPoint()
            {
                return pGetMidPoint();
            }

            GeoLibPointF pGetMidPoint()
            {
                return midPoint;
            }

            public BoundingBox(List<GeoLibPointF> incomingPoints)
            {
                pBoundingBox(incomingPoints);
            }

            void pBoundingBox(List<GeoLibPointF> incomingPoints)
            {
                if (incomingPoints == null)
                {
                    midPoint = new GeoLibPointF(0.0f, 0.0f);
                }
                else
                {
                    var minX = incomingPoints.Min(p => p.X);
                    var minY = incomingPoints.Min(p => p.Y);
                    var maxX = incomingPoints.Max(p => p.X);
                    var maxY = incomingPoints.Max(p => p.Y);
                    midPoint = new GeoLibPointF(minX + ((maxX - minX) / 2.0f), minY + ((maxY - minY) / 2.0f));
                }
            }
        }

        List<GeoLibPointF> preFlight(List<GeoLibPointF> mcPoints, EntropyLayerSettings entropyLayerSettings)
        {
            // Fragment by resolution
            List<GeoLibPointF> newMCPoints = fragment.fragmentPath(mcPoints);

            bool H = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.flipH) == 1;
            bool V = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.flipV) == 1;
            bool alignX = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.alignX) == 1;
            bool alignY = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.alignY) == 1;

            newMCPoints = GeoWrangler.flip(H, V, alignX, alignY, pivot, newMCPoints);

            List<GeoLibPointF> tempList = new List<GeoLibPointF>();
            // Now to start the re-indexing.
            for (Int32 pt = 0; pt < newMCPoints.Count; pt++)
            {
                bool addPoint;
                if (pt == 0)
                {
                    addPoint = true;
                }
                else
                {
                    addPoint = !((Math.Abs(tempList[^1].X - newMCPoints[pt].X) < Double.Epsilon) && (Math.Abs(tempList[^1].Y - newMCPoints[pt].Y) < Double.Epsilon));
                }

                // Avoid adding duplicate vertices
                if (addPoint)
                {
                    tempList.Add(new GeoLibPointF(newMCPoints[pt].X, newMCPoints[pt].Y));
                }
            }

            return tempList;
        }

        public EntropyShape(EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, Int32 settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, ShapeLibrary shape = null, GeoLibPointF pivot = null)
        {
            makeEntropyShape(entropySettings, entropyLayerSettingsList, settingsIndex, doPASearch, previewMode, chaosSettings, shape, pivot);
        }

        List<GeoLibPointF> makeShape(bool returnEarly, bool cornerCheck, EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, int settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, ShapeLibrary shape)
        {
            List<GeoLibPointF> mcPoints = new List<GeoLibPointF>(); // overall points container. We'll use this to populate and send back our Point array later. Int only...

            Vertex = shape.Vertex;
            tips = shape.tips;
            round1 = shape.round1;
            // Wrangle the tips.
            for (Int32 cp = 0; cp < (Vertex.Count() - 1); cp++) // We don't drive the last point directly - we'll close our shape.
            {
                if (tips[cp])
                {
                    // Note that these are reversed due to top-left origin of drawing!
                    // A positive bias shrinks the shape in this code.....
                    // Values below are correlated in the MCControl system for simulations. In preview mode, these are all zero.
                    double vTipBiasNegVar = chaosSettings.getValue(ChaosSettings.properties.vTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.vTNVar));
                    double vTipBiasPosVar = chaosSettings.getValue(ChaosSettings.properties.vTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.vTPVar));
                    double hTipBiasNegVar = chaosSettings.getValue(ChaosSettings.properties.hTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.hTNVar));
                    double hTipBiasPosVar = chaosSettings.getValue(ChaosSettings.properties.hTipBiasVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.hTPVar));
                    if ((Vertex[cp].direction == typeDirection.down1) && (Vertex[cp].yBiasApplied == false))
                    {
                        Vertex[cp].Y -= Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.vTBias));
                        Vertex[cp].Y -= Convert.ToDouble(globalBias_Tips);
                        if (chaosSettings.getValue(ChaosSettings.properties.vTipBiasType, settingsIndex) < 0.5) // Need to use our negative variation value
                        {
                            Vertex[cp].Y -= vTipBiasNegVar;
                        }
                        else
                        {
                            Vertex[cp].Y -= vTipBiasPosVar;
                        }
                        Vertex[cp].yBiasApplied = true;
                    }
                    if ((Vertex[cp].direction == typeDirection.up1) && (Vertex[cp].yBiasApplied == false))
                    {
                        Vertex[cp].Y += Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.vTBias));
                        Vertex[cp].Y += globalBias_Tips;
                        if (chaosSettings.getValue(ChaosSettings.properties.vTipBiasType, settingsIndex) < 0.5) // Need to use our negative variation value
                        {
                            Vertex[cp].Y += vTipBiasNegVar;
                        }
                        else
                        {
                            Vertex[cp].Y += vTipBiasPosVar;
                        }
                        Vertex[cp].yBiasApplied = true;
                    }
                    if ((Vertex[cp].direction == typeDirection.left1) && (Vertex[cp].xBiasApplied == false))
                    {
                        Vertex[cp].X -= Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.hTBias));
                        Vertex[cp].X -= globalBias_Tips;
                        if (chaosSettings.getValue(ChaosSettings.properties.hTipBiasType, settingsIndex) < 0.5) // Need to use our negative variation value
                        {
                            Vertex[cp].X -= hTipBiasNegVar;
                        }
                        else
                        {
                            Vertex[cp].X -= hTipBiasPosVar;
                        }
                        Vertex[cp].xBiasApplied = true;
                    }
                    if ((Vertex[cp].direction == typeDirection.right1) && (Vertex[cp].xBiasApplied == false))
                    {
                        Vertex[cp].X += Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.hTBias));
                        Vertex[cp].X += globalBias_Tips;
                        if (chaosSettings.getValue(ChaosSettings.properties.hTipBiasType, settingsIndex) < 0.5) // Need to use our negative variation value
                        {
                            Vertex[cp].X += hTipBiasNegVar;
                        }
                        else
                        {
                            Vertex[cp].X += hTipBiasPosVar;
                        }
                        Vertex[cp].xBiasApplied = true;
                    }
                }
            }

            // Global bias for anything that isn't a tip.
            for (Int32 cp = 0; cp < (Vertex.Count() - 1); cp++) // We don't drive the last point directly - we'll close our shape.
            {
                if ((!Vertex[cp].xBiasApplied) && (!tips[cp]))
                {
                    switch (Vertex[cp].direction)
                    {
                        case typeDirection.left1:
                            Vertex[cp].X -= globalBias_Sides;
                            break;
                        case typeDirection.right1:
                            Vertex[cp].X += globalBias_Sides;
                            break;
                    }
                }
                if ((!Vertex[cp].yBiasApplied) && (!tips[cp]))
                {
                    switch (Vertex[cp].direction)
                    {
                        case typeDirection.up1:
                            Vertex[cp].Y += globalBias_Sides;
                            break;
                        case typeDirection.down1:
                            Vertex[cp].Y -= globalBias_Sides;
                            break;
                    }
                }
            }

            // Iterate the corners to apply the bias from the edges.
            for (Int32 corner = 0; corner < round1.Count(); corner++)
            {
                Vertex[round1[corner].index].X = Vertex[round1[corner].verFace].X;
                Vertex[round1[corner].index].Y = Vertex[round1[corner].horFace].Y;
            }

            Vertex[Vertex.Count() - 1] = Vertex[0]; // close the shape.
            round1[round1.Count() - 1] = round1[0];

            // Set the midpoints of the edges to the average between the two corners
            for (Int32 corner = 0; corner < round1.Count(); corner++)
            {
                double previousEdgeLength;

                if (corner == 0)
                {
                    previousEdgeLength = Math.Abs(
                        GeoWrangler.distanceBetweenPoints(new GeoLibPointF(Vertex[round1[corner].index].X, Vertex[round1[corner].index].Y),
                                                        new GeoLibPointF(Vertex[round1[round1.Count() - 1].index].X, Vertex[round1[round1.Count() - 1].index].Y))
                    );
                }
                else
                {
                    previousEdgeLength = Math.Abs(
                        GeoWrangler.distanceBetweenPoints(new GeoLibPointF(Vertex[round1[corner].index].X, Vertex[round1[corner].index].Y),
                                                        new GeoLibPointF(Vertex[round1[corner - 1].index].X, Vertex[round1[corner - 1].index].Y))
                    );
                }

                // Wrap around if we exceed the length
                double nextEdgeLength = Math.Abs(
                    GeoWrangler.distanceBetweenPoints(new GeoLibPointF(Vertex[round1[(corner + 1) % (round1.Count() - 1)].index].X, Vertex[round1[(corner + 1) % (round1.Count() - 1)].index].Y),
                        new GeoLibPointF(Vertex[round1[(corner + 2) % (round1.Count() - 1)].index].X, Vertex[round1[(corner + 2) % (round1.Count() - 1)].index].Y))
                );

                double currentEdgeLength = Math.Abs(
                    GeoWrangler.distanceBetweenPoints(new GeoLibPointF(Vertex[round1[corner].index].X, Vertex[round1[corner].index].Y),
                        new GeoLibPointF(Vertex[round1[(corner + 1) % (round1.Count() - 1)].index].X, Vertex[round1[(corner + 1) % (round1.Count() - 1)].index].Y))
                );

                double offset = 0.5f * currentEdgeLength;
                bool reverseSlide = true; // used in the linear mode to handle reversed case (where ratio is > 1), and the no-slide case.)

                if ((entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.edgeSlide) == 1) && (previousEdgeLength > 0) && (nextEdgeLength > 0))
                {
                    // Now we need to figure out the weighting.
                    double ratio = Math.Abs(nextEdgeLength / previousEdgeLength);
                    bool doLinearSlide = false;

                    if (ratio < 1)
                    {
                        reverseSlide = false;
                        if (ratio < 1E-2)
                        {
                            ratio = 1E-2; // clamp
                        }
                        ratio = (1 / ratio); // normalize into our expected range
                    }

                    if (doLinearSlide)
                    {
                        // Linear
                        offset = Math.Pow(currentEdgeLength / 2.0f, 1.0f / ratio); // ratio * currentEdgeLength / 2.0f;
                    }
                    else
                    {
                        // Sigmoid function to try and provide some upper and lower resistance to the slide.
                        // center is to force 0.5 value of the scaling factor for a ratio of 1
                        // tension controls the shape of the curve, and thus the sensitivity of the response..
                        double center = 1.0f;
                        double tension = Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.eTension));
                        offset = currentEdgeLength * (1 / (1 + Math.Exp(-tension * (center - ratio))));
                    }
                }

                if (corner % 2 == 0)
                {
                    // Get our associated vertical edge Y position
                    double yPoint1;
                    double yPoint2 = Vertex[round1[(corner + 1) % (round1.Count() - 1)].horFace].Y;
                    switch (corner)
                    {
                        case 0:
                            // Need to wrap around for bias look-up
                            yPoint1 = Vertex[round1[round1.Count() - 1].horFace].Y;
                            break;
                        default:
                            yPoint1 = Vertex[round1[corner].horFace].Y;
                            break;
                    }

                    if (yPoint1 < yPoint2)
                    {
                        if (reverseSlide)
                        {
                            Vertex[round1[corner].verFace].Y = yPoint2 - offset;
                        }
                        else
                        {
                            Vertex[round1[corner].verFace].Y = yPoint1 + offset;
                        }
                    }
                    else
                    {
                        if (reverseSlide)
                        {
                            Vertex[round1[corner].verFace].Y = yPoint2 + offset;
                        }
                        else
                        {
                            Vertex[round1[corner].verFace].Y = yPoint1 - offset;
                        }
                    }
                }
                else
                {
                    // Tweak horizontal edge
                    double xPoint1 = Vertex[round1[corner].verFace].X;
                    double xPoint2 = Vertex[round1[(corner + 1) % (round1.Count() - 1)].verFace].X;

                    if (xPoint1 < xPoint2)
                    {
                        if (reverseSlide)
                        {
                            Vertex[round1[corner].horFace].X = xPoint2 - offset;
                        }
                        else
                        {
                            Vertex[round1[corner].horFace].X = xPoint1 + offset;
                        }
                    }
                    else
                    {
                        if (reverseSlide)
                        {
                            Vertex[round1[corner].horFace].X = xPoint2 + offset;
                        }
                        else
                        {
                            Vertex[round1[corner].horFace].X = xPoint1 - offset;
                        }
                    }
                }
            }

            if (returnEarly)
            {
                mcPoints.Clear();
                for (Int32 i = 0; i < Vertex.Count(); i++)
                {
                    mcPoints.Add(new GeoLibPointF(Vertex[i].X, Vertex[i].Y));
                }
                return mcPoints;
            }

            List<GeoLibPointF> mcHorEdgePoints = new List<GeoLibPointF>(); // corner coordinates list, used as a temporary container for each iteration
            List<List<GeoLibPointF>> mcHorEdgePointsList = new List<List<GeoLibPointF>>(); // Hold our lists of doubles for each corner in the shape, in order. We cast these to Int in the mcPoints list.
            List<List<GeoLibPointF>> mcVerEdgePointsList = new List<List<GeoLibPointF>>(); // Hold our lists of doubles for each edge in the shape, in order. We cast these to Int in the mcPoints list.
            
            for (Int32 round = 0; round < round1.Length - 1; round++)
            {
                // Derive our basic coordinates for the three vertices on the edge.
                double start_x = Vertex[round1[round].index].X;
                double start_y = Vertex[round1[round].index].Y;
                double currentHorEdge_mid_x = Vertex[round1[round].horFace].X;
                double currentVerEdge_mid_y = Vertex[round1[round].verFace].Y;
                double end_x = Vertex[round1[round + 1].index].X;
                double end_y = Vertex[round1[round + 1].index].Y;
                double nextVerEdge_mid_y = Vertex[round1[round + 1].verFace].Y;

                // Test whether we have a vertical edge or not. We only process horizontal edges to avoid doubling up
                if (Math.Abs(start_y - end_y) < Double.Epsilon)
                {
                    double mcPX;
                    double mcPY = 0.0f;
                    // Establish corner rounding sign at start and end points of edge. Default is to move outwards (inner CRR)
                    bool startInnerRounding = true;
                    bool endInnerRounding = true;
                    if (round1[round].direction == typeRound.exter)
                    {
                        startInnerRounding = false;
                    }
                    if (round1[round + 1].direction == typeRound.exter)
                    {
                        endInnerRounding = false;
                    }

                    // Now sort out the shift based on face orientation.
                    bool horFaceUp = true;
                    switch (Vertex[round1[round].horFace].direction)
                    {
                        case typeDirection.up1:
                            break;
                        case typeDirection.down1:
                            horFaceUp = false;
                            break;
                    }
                    bool verFaceLeft = true;
                    switch (Vertex[round1[round].verFace].direction)
                    {
                        case typeDirection.left1:
                            break;
                        case typeDirection.right1:
                            verFaceLeft = false;
                            break;
                    }

                    // Segment 1

                    // Clamp radius in each direction, if needed, to available distance
                    double hRadius = round1[round].MaxRadius;
                    double x_Distance = Math.Sqrt(Utils.myPow(currentHorEdge_mid_x - start_x, 2));
                    double vRadius = round1[round].MaxRadius;
                    double y_Distance = Math.Sqrt(Utils.myPow(currentVerEdge_mid_y - start_y, 2));

                    // Add our random variation based on rounding type :

                    bool paSearchAffectsCornerRounding = false;

                    if (doPASearch)
                    {
                        switch (startInnerRounding)
                        {
                            case true:
                                paSearchAffectsCornerRounding = chaosSettings.getBool(ChaosSettings.bools.icPA, settingsIndex);
                                break;
                            default:
                                paSearchAffectsCornerRounding = chaosSettings.getBool(ChaosSettings.bools.ocPA, settingsIndex);
                                break;
                        }
                    }

                    if (paSearchAffectsCornerRounding)
                    {
                        if (startInnerRounding)
                        {
                            hRadius = chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.iCR));
                            vRadius = chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.iCR));
                        }
                        else
                        {
                            hRadius = chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.oCR));
                            vRadius = chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.oCR));
                        }
                    }
                    else
                    {
                        if (!previewMode)
                        {
                            if (startInnerRounding)
                            {
                                hRadius += chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.iCV));
                                vRadius += chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.iCV));
                            }
                            else
                            {
                                hRadius += chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.oCV));
                                vRadius += chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.oCV));
                            }
                        }
                    }

                    if (hRadius > x_Distance)
                    {
                        hRadius = x_Distance;
                    }
                    if (vRadius > y_Distance)
                    {
                        vRadius = y_Distance;
                    }

                    // Clamp for negative radius values that would make no sense
                    if (hRadius < 0)
                    {
                        hRadius = 0;
                    }
                    if (vRadius < 0)
                    {
                        vRadius = 0;
                    }

                    double angleIncrement = 90.0f / entropySettings.getValue(EntropySettings.properties_i.cSeg);

                    // Sweep our corner.
                    double angle = 0.0f;
                    while (angle <= 90.0f)
                    {
                        // Set start condition
                        mcPX = start_x; // X position for new point.
                        mcPY = start_y; // this will hold our Y position for the new point.

                        // Remove full contribution from rounding.
                        if (verFaceLeft)
                        {
                            if (startInnerRounding)
                            {
                                mcPX -= hRadius;
                            }
                            else
                            {
                                mcPX += hRadius;
                            }
                        }
                        else
                        {
                            if (startInnerRounding)
                            {
                                mcPX += hRadius;
                            }
                            else
                            {
                                mcPX -= hRadius;
                            }
                        }
                        if (horFaceUp)
                        {
                            if (startInnerRounding)
                            {
                                mcPY += vRadius;
                            }
                            else
                            {
                                mcPY -= vRadius;
                            }
                        }
                        else
                        {
                            if (startInnerRounding)
                            {
                                mcPY -= vRadius;
                            }
                            else
                            {
                                mcPY += vRadius;
                            }
                        }

                        // Now process corner, adding back contribution from rounding
                        if (verFaceLeft)
                        {
                            if (startInnerRounding)
                            {
                                mcPX += hRadius * Math.Cos(Utils.toRadians(angle));
                            }
                            else
                            {
                                mcPX -= hRadius * Math.Cos(Utils.toRadians(angle));
                            }
                        }
                        else
                        {
                            if (startInnerRounding)
                            {
                                mcPX -= hRadius * Math.Cos(Utils.toRadians(angle));
                            }
                            else
                            {
                                mcPX += hRadius * Math.Cos(Utils.toRadians(angle));
                            }
                        }
                        if (horFaceUp)
                        {
                            if (startInnerRounding)
                            {
                                mcPY -= vRadius * Math.Sin(Utils.toRadians(angle));
                            }
                            else
                            {
                                mcPY += vRadius * Math.Sin(Utils.toRadians(angle));
                            }
                        }
                        else
                        {
                            if (startInnerRounding)
                            {
                                mcPY += vRadius * Math.Sin(Utils.toRadians(angle));
                            }
                            else
                            {
                                mcPY -= vRadius * Math.Sin(Utils.toRadians(angle));
                            }
                        }

                        GeoLibPointF cPt = new GeoLibPointF(mcPX, mcPY);
                        if ((angle == 0) || (Math.Abs(angle - 90) < Double.Epsilon) || (entropySettings.getValue(EntropySettings.properties_i.optC) == 0) ||
                            ((entropySettings.getValue(EntropySettings.properties_i.optC) == 1) &&
                             (Math.Abs(
                               GeoWrangler.distanceBetweenPoints(mcHorEdgePoints[mcHorEdgePoints.Count() - 1], cPt)
                               )
                               > entropySettings.getResolution()
                              )
                             )
                            )
                        {
                            mcHorEdgePoints.Add(cPt);
                        }
                        angle += angleIncrement;
                    }

                    // OK. We now need to add points along the edge based on the simulation settings resolution.
                    // We need to add points from here to just before the midpoint

                    double bridgeX = mcHorEdgePoints[mcHorEdgePoints.Count() - 1].X;

                    // Fragmenter returns first and last points in the point array.
                    GeoLibPointF[] fragments = fragment.fragmentPath(new [] { new GeoLibPointF(bridgeX, mcPY), new GeoLibPointF(currentHorEdge_mid_x, mcPY) });

                    for (int i = 1; i < fragments.Length - 1; i++)
                    {
                        mcHorEdgePoints.Add(fragments[i]);
                    }

                    // Add our midpoint.
                    mcHorEdgePoints.Add(new GeoLibPointF(currentHorEdge_mid_x, mcPY));

                    // Segment 2, plus bridging on first pass through.

                    bool firstPass = true; // With this set, we bridge from midpoint to our first point in the first pass through
                                           // segment 2 of the edge.
                    verFaceLeft = true;
                    switch (Vertex[round1[round + 1].verFace].direction)
                    {
                        case typeDirection.left1:
                            break;
                        case typeDirection.right1:
                            verFaceLeft = false;
                            break;
                    }

                    // Clamp radius to available distance, if needed.
                    hRadius = round1[round + 1].MaxRadius;
                    x_Distance = Math.Sqrt(Utils.myPow(currentHorEdge_mid_x - end_x, 2));
                    vRadius = round1[round + 1].MaxRadius;
                    y_Distance = Math.Sqrt(Utils.myPow(nextVerEdge_mid_y - end_y, 2));

                    // Add our random variation based on rounding type :

                    paSearchAffectsCornerRounding = false;
                    if (doPASearch)
                    {
                        switch (startInnerRounding)
                        {
                            case true:
                                paSearchAffectsCornerRounding = chaosSettings.getBool(ChaosSettings.bools.icPA, settingsIndex);
                                break;
                            default:
                                paSearchAffectsCornerRounding = chaosSettings.getBool(ChaosSettings.bools.ocPA, settingsIndex);
                                break;
                        }
                    }

                    if (paSearchAffectsCornerRounding)
                    {
                        if (startInnerRounding)
                        {
                            hRadius = chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.iCR));
                            vRadius = chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.iCR));
                        }
                        else
                        {
                            hRadius = chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.oCR));
                            vRadius = chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.oCR));
                        }
                    }
                    else
                    {
                        // Add our random variation based on rounding type :
                        if (!previewMode)
                        {
                            if (endInnerRounding)
                            {
                                hRadius += chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.iCV));
                                vRadius += chaosSettings.getValue(ChaosSettings.properties.icVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.iCV));
                            }
                            else
                            {
                                hRadius += chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.oCV));
                                vRadius += chaosSettings.getValue(ChaosSettings.properties.ocVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.oCV));
                            }
                        }
                    }

                    if (hRadius > x_Distance)
                    {
                        hRadius = x_Distance;
                    }
                    if (vRadius > y_Distance)
                    {
                        vRadius = y_Distance;
                    }

                    // Clamp for negative radius values that would make no sense
                    if (hRadius < 0)
                    {
                        hRadius = 0;
                    }
                    if (vRadius < 0)
                    {
                        vRadius = 0;
                    }

                    // Sweep our end corner. We need to run the sweep in the opposite direction.
                    angle = 90.0f;
                    while (angle >= 0.0f)
                    {
                        // Set start conditions
                        mcPX = end_x;
                        mcPY = end_y;

                        // Remove full extent of rounding in each direction, based on face orientation
                        if (verFaceLeft)
                        {
                            if (endInnerRounding)
                            {
                                mcPX -= hRadius;
                            }
                            else
                            {
                                mcPX += hRadius;
                            }
                        }
                        else
                        {
                            if (endInnerRounding)
                            {
                                mcPX += hRadius;
                            }
                            else
                            {
                                mcPX -= hRadius;
                            }
                        }
                        if (horFaceUp)
                        {
                            if (endInnerRounding)
                            {
                                mcPY += vRadius;
                            }
                            else
                            {
                                mcPY -= vRadius;
                            }
                        }
                        else
                        {
                            if (endInnerRounding)
                            {
                                mcPY -= vRadius;
                            }
                            else
                            {
                                mcPY += vRadius;
                            }
                        }

                        // Process corners, adding back the contribution from the rounding based on the angle
                        if (verFaceLeft)
                        {
                            if (endInnerRounding)
                            {
                                mcPX += hRadius * Math.Cos(Utils.toRadians(angle));
                            }
                            else
                            {
                                mcPX -= hRadius * Math.Cos(Utils.toRadians(angle));
                            }
                        }
                        else
                        {
                            if (endInnerRounding)
                            {
                                mcPX -= hRadius * Math.Cos(Utils.toRadians(angle));
                            }
                            else
                            {
                                mcPX += hRadius * Math.Cos(Utils.toRadians(angle));
                            }
                        }
                        if (horFaceUp)
                        {
                            if (endInnerRounding)
                            {
                                mcPY -= vRadius * Math.Sin(Utils.toRadians(angle));
                            }
                            else
                            {
                                mcPY += vRadius * Math.Sin(Utils.toRadians(angle));
                            }
                        }
                        else
                        {
                            if (endInnerRounding)
                            {
                                mcPY += vRadius * Math.Sin(Utils.toRadians(angle));
                            }
                            else
                            {
                                mcPY -= vRadius * Math.Sin(Utils.toRadians(angle));
                            }
                        }

                        // If this is the first pass, we need to add points to the start of the rounding, from the midpoint.
                        if (firstPass)
                        {
                            bridgeX = currentHorEdge_mid_x;

                            // Fragmenter returns first and last points in the point array.
                            fragments = fragment.fragmentPath(new [] { new GeoLibPointF(bridgeX, mcPY), new GeoLibPointF(mcPX, mcPY) });

                            for (int i = 1; i < fragments.Length - 1; i++)
                            {
                                mcHorEdgePoints.Add(fragments[i]);
                            }

                            firstPass = false;
                        }

                        GeoLibPointF cPt = new GeoLibPointF(mcPX, mcPY);
                        if ((angle == 0) || (Math.Abs(angle - 90) < Double.Epsilon) || (entropySettings.getValue(EntropySettings.properties_i.optC) == 0) ||
                            ((entropySettings.getValue(EntropySettings.properties_i.optC) == 1) &&
                             (Math.Abs(
                               GeoWrangler.distanceBetweenPoints(mcHorEdgePoints[mcHorEdgePoints.Count() - 1], cPt)
                               )
                               > entropySettings.getResolution()
                              )
                             )
                            )
                        {
                            mcHorEdgePoints.Add(cPt);
                        }
                        angle -= angleIncrement;
                    }

                    mcHorEdgePointsList.Add(mcHorEdgePoints.ToList()); // make a deep copy of the points.
                    mcHorEdgePoints.Clear(); // clear our list of points to use on the next pass.
                }
            }

            if (cornerCheck)
            {
                mcPoints.Clear();
                for (Int32 i = 0; i < mcHorEdgePointsList.Count(); i++)
                {
                    for (Int32 j = 0; j < mcHorEdgePointsList[i].Count(); j++)
                    {
                        mcPoints.Add(new GeoLibPointF(mcHorEdgePointsList[i][j].X, mcHorEdgePointsList[i][j].Y));
                    }
                }
                mcPoints.Add(new GeoLibPointF(mcPoints[0].X, mcPoints[0].Y));
                return mcPoints;
            }

            // Now we have our corners, let's process the vertical edges. We need the corners in order to get our start/end on each vertical edge.
            for (int edge = 0; edge < mcHorEdgePointsList.Count(); edge++)
            {
                // Get our start and end Y positions for our vertical edge.
                List<GeoLibPointF> startHorEdgePointList = mcHorEdgePointsList[edge];
                Int32 endHorEdgePointListIndex;
                if (edge == 0)
                {
                    endHorEdgePointListIndex = mcHorEdgePointsList.Count() - 1; // need to wrap around.
                }
                else
                {
                    endHorEdgePointListIndex = edge - 1;
                }

                List<GeoLibPointF> endHorEdgePointList = mcHorEdgePointsList[endHorEdgePointListIndex];
                Double vert_x = endHorEdgePointList[endHorEdgePointList.Count() - 1].X;
                Double startPoint_y = endHorEdgePointList[endHorEdgePointList.Count() - 1].Y;
                Double endPoint_y = startHorEdgePointList[0].Y;

                // We get the start and end points here.
                GeoLibPointF[] fragments = fragment.fragmentPath(new [] { new GeoLibPointF(vert_x, startPoint_y), new GeoLibPointF(vert_x, endPoint_y) });

                mcVerEdgePointsList.Add(fragments.ToList()); // make a deep copy of the points.
            }

            // OK. We have our corners and edges. We need to walk them now. We'll apply the subshape 1 offset at the same time.
            for (Int32 section = 0; section < mcVerEdgePointsList.Count; section++)
            {
                for (Int32 point = 0; point < mcVerEdgePointsList[section].Count; point++)
                {
                    double x = mcVerEdgePointsList[section][point].X + Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset));
                    double y = mcVerEdgePointsList[section][point].Y + Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset));
                    mcPoints.Add(new GeoLibPointF(x, y));
                }

                // Corner next.
                // Start and end points match those in the vertical edges, so we avoid them to eliminate duplicates.
                for (Int32 point = 1; point < mcHorEdgePointsList[section].Count() - 1; point++)
                {
                    double x = mcHorEdgePointsList[section][point].X + Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset));
                    double y = mcHorEdgePointsList[section][point].Y + Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset));
                    mcPoints.Add(new GeoLibPointF(x, y));
                }
            }

            return mcPoints;
        }

        void makeEntropyShape(EntropySettings entropySettings, List<EntropyLayerSettings> entropyLayerSettingsList, Int32 settingsIndex, bool doPASearch, bool previewMode, ChaosSettings chaosSettings, ShapeLibrary shape = null, GeoLibPointF pivot_ = null)
        {
            bool geoCoreShapeDefined = (shape != null);
            bool cornerCheck = false;
            bool returnEarly = false;

            double xOverlayVal = 0.0f;
            double yOverlayVal = 0.0f;

            if (pivot_ != null)
            {
                pivot = new GeoLibPointF(pivot_.X, pivot_.Y);
            }

            fragment = new Fragmenter(entropySettings.getResolution(), CentralProperties.scaleFactorForOperation);

            // Define our biases. We will use these later.
            globalBias_Sides = Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.sBias));
            globalBias_Sides += (chaosSettings.getValue(ChaosSettings.properties.CDUSVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.sCDU)) / 2);
            globalBias_Tips = (chaosSettings.getValue(ChaosSettings.properties.CDUTVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.tCDU)) / 2);
            
            if (shape == null)
            {
                shape = new ShapeLibrary(entropyLayerSettingsList[settingsIndex]);
                shape.setShape(entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.shapeIndex));
            }

            // Tip wrangling and shape closure will happen next
            bool failSafe = !shape.shapeValid; // Set failsafe if shape is invalid.

            if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.enabled) == 0)
            {
                failSafe = true;
            }

            List<GeoLibPointF> mcPoints = new List<GeoLibPointF>(); // overall points container. We'll use this to populate and send back our Point array later. Ints only...

            // Handle non-orthogonal case.

            if (!failSafe)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if ((!geoCoreShapeDefined) || (geoCoreShapeDefined && shape.geoCoreShapeOrthogonal))
                {
                    // ReSharper disable twice ConditionIsAlwaysTrueOrFalse
                    mcPoints = makeShape(returnEarly, cornerCheck, entropySettings, entropyLayerSettingsList, settingsIndex, doPASearch, previewMode, chaosSettings, shape);
                }
                else
                {
                    // We have a non-orthogonal geoCore shape, so we take the defined vertices and use them directly. No rounding or tips (tips might be doable at a later date).
                    foreach (MyVertex t in shape.Vertex)
                    {
                        mcPoints.Add(new GeoLibPointF(t.X, t.Y));
                    }
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
                xOverlayVal = chaosSettings.getValue(ChaosSettings.properties.overlayX, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.xOL));

                if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.xOL_av) == 1) // overlay average
                {
                    List<double> overlayValues = new List<double>();
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

                yOverlayVal = chaosSettings.getValue(ChaosSettings.properties.overlayY, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.yOL)); // Overlay shift

                if (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.yOL_av) == 1) // overlay average
                {
                    List<double> overlayValues = new List<double>();
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
            }

            foreach (GeoLibPointF t in mcPoints)
            {
                t.X += xOverlayVal;
                t.Y += yOverlayVal;
            }

            double threshold = 1E-5; // threshold for proximity, to avoid normal extraction issues.
            if (mcPoints.Count > 1)
            {
                // Need to clean up any duplicate points at this point, to avoid causing /0 issues below.
                List<GeoLibPointF> newPoints = new List<GeoLibPointF>();
                for (int pt = 1; pt < mcPoints.Count; pt++)
                {
                    if (Math.Abs(Math.Sqrt(Utils.myPow(mcPoints[pt].X - mcPoints[pt - 1].X, 2) + Utils.myPow(mcPoints[pt].Y - mcPoints[pt - 1].Y, 2))) > threshold)
                    {
                        newPoints.Add(new GeoLibPointF(mcPoints[pt - 1]));
                    }
                }
                if (newPoints.Count != 0)
                {
                    // Close shape.
                    mcPoints = newPoints.ToList();
                    mcPoints.Add(new GeoLibPointF(mcPoints[0]));
                }
            }

            double rotationAngle = Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.rot));
            double rotationVar = chaosSettings.getValue(ChaosSettings.properties.wobbleVar, settingsIndex) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.wobble));
            // Per-poly rotation for layout case (only scenario where we have polygon sheets at this level)
            // Note that we don't have CSV tracking for this case - there's no practical way to record the random values here.
            if ((entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.perPoly) == 1) && (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE))
            {
                rotationVar = UtilityFuncs.getGaussianRandomNumber3(entropySettings) * Convert.ToDouble(entropyLayerSettingsList[settingsIndex].getDecimal(EntropyLayerSettings.properties_decimal.wobble));
            }
            double rotationDirection = UtilityFuncs.getGaussianRandomNumber3(entropySettings);
            if (rotationDirection <= 0.5)
            {
                rotationAngle -= rotationVar;
            }
            else
            {
                rotationAngle += rotationVar;
            }

            if ((rotationAngle != 0) || (((entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.flipH) == 1) || (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.flipV) == 1)) && ((entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.alignX) == 1) || (entropyLayerSettingsList[settingsIndex].getInt(EntropyLayerSettings.properties_i.alignY) == 1))))
            {
                // Get our bounding box.
                BoundingBox bb = new BoundingBox(mcPoints);

                switch (pivot)
                {
                    case null:
                        pivot = new GeoLibPointF(bb.getMidPoint());
                        break;
                }

                // OK. Let's try some rotation and wobble.
                // Temporary separate container for our rotated points, just for now.
                List<GeoLibPointF> rotatedPoints = GeoWrangler.Rotate(pivot, mcPoints, rotationAngle);
                mcPoints.Clear();
                mcPoints = rotatedPoints.ToList();
            }

            // Error handling (failSafe) for no points or no subshape  - safety measure.
            if (!mcPoints.Any())
            {
                mcPoints.Add(new GeoLibPointF(0.0f, 0.0f));
            }

            // Path direction, point order and re-fragmentation (as needed)
            mcPoints = preFlight(mcPoints, entropyLayerSettingsList[settingsIndex]);

            points = new GeoLibPointF[mcPoints.Count()];
            for (Int32 i = 0; i < mcPoints.Count(); i++)
            {
                points[i] = mcPoints[i];
            }

            if ((Math.Abs(points[0].X - points[points.Count() - 1].X) > Double.Epsilon) || (Math.Abs(points[0].Y - points[points.Count() - 1].Y) > Double.Epsilon))
            {
                ErrorReporter.showMessage_OK("Start and end not the same - entropyShape", "Oops");
            }
        }
    }
}
