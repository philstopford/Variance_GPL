using ClipperLib; // tiled layout handling, Layout biasing/CDU.
using color;
using Error;
using geoLib;
using geoWrangler;
using Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using utility;

namespace Variance
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public class PreviewShape
    {
        bool DOEDependency; // due to the DOE grid, we need this to sort out offsets. This includes buried references in Booleans. The min X/Y values for this case need to be at least the col/row offset.
        double doeMinX, doeMinY;

        Fragmenter fragment;
        // Class for our preview shapes.
        List<GeoLibPointF[]> previewPoints; // list of polygons defining the shape(s) that will be drawn. In the complex case, we populate this from complexPoints.
        public List<GeoLibPointF[]> getPoints()
        {
            return pGetPoints();
        }

        List<GeoLibPointF[]> pGetPoints()
        {
            return previewPoints;
        }

        public GeoLibPointF[] getPoints(int index)
        {
            return pGetPoints(index);
        }

        GeoLibPointF[] pGetPoints(int index)
        {
            return previewPoints[index];
        }

        public void addPoints(GeoLibPointF[] poly)
        {
            pAddPoints(poly);
        }

        void pAddPoints(GeoLibPointF[] poly)
        {
            previewPoints.Add(poly.ToArray());
        }

        public void setPoints(List<GeoLibPointF[]> newPoints)
        {
            pSetPoints(newPoints);
        }

        void pSetPoints(List<GeoLibPointF[]> newPoints)
        {
            previewPoints = newPoints.ToList();
        }

        public void clearPoints()
        {
            pClearPoints();
        }

        void pClearPoints()
        {
            previewPoints.Clear();
        }

        List<bool> drawnPoly; // to track drawn vs enabled polygons. Can then use for filtering elsewhere.

        public bool getDrawnPoly(int index)
        {
            return pGetDrawnPoly(index);
        }

        bool pGetDrawnPoly(int index)
        {
            return drawnPoly[index];
        }

        List<bool> geoCoreOrthogonalPoly;
        MyColor color;

        public MyColor getColor()
        {
            return pGetColor();
        }

        MyColor pGetColor()
        {
            return color;
        }

        public void setColor(MyColor c)
        {
            pSetColor(c);
        }

        void pSetColor(MyColor c)
        {
            color = new MyColor(c);
        }

        double xOffset;
        double yOffset;

        int _settingsIndex; // track originating layer.

        public Int32 getIndex()
        {
            return pGetIndex();
        }

        Int32 pGetIndex()
        {
            return _settingsIndex;
        }

        void rectangle_offset(EntropyLayerSettings entropyLayerSettings)
        {
            string posInSubShapeString = ((CommonVars.subShapeLocations)entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex)).ToString();
            double tmp_xOffset = 0;
            double tmp_yOffset = 0;

            if ((posInSubShapeString == "TL") ||
                (posInSubShapeString == "TR") ||
                (posInSubShapeString == "TS") ||
                (posInSubShapeString == "RS") ||
                (posInSubShapeString == "LS") ||
                (posInSubShapeString == "C"))
            {
                // Vertical offset needed to put reference corner at world center
                tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));

                // Half the value for a vertical centering requirement
                if ((posInSubShapeString == "RS") ||
                    (posInSubShapeString == "LS") ||
                    (posInSubShapeString == "C"))
                {
                    tmp_yOffset = Convert.ToDouble(tmp_yOffset / 2);
                }
            }
            yOffset -= tmp_yOffset;

            if ((posInSubShapeString == "TR") ||
                (posInSubShapeString == "BR") ||
                (posInSubShapeString == "TS") ||
                (posInSubShapeString == "RS") ||
                (posInSubShapeString == "BS") ||
                (posInSubShapeString == "C"))
            {
                tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));

                // Half the value for horizontal centering conditions
                if ((posInSubShapeString == "TS") ||
                    (posInSubShapeString == "BS") ||
                    (posInSubShapeString == "C"))
                {
                    tmp_xOffset = Convert.ToDouble(tmp_xOffset / 2);
                }
            }
            xOffset += tmp_xOffset;
        }

        void lShape_offset(EntropyLayerSettings entropyLayerSettings)
        {
            string posInSubShapeString = ((CommonVars.subShapeLocations)entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex)).ToString();
            double tmp_xOffset = 0;
            double tmp_yOffset = 0;

            if ((posInSubShapeString == "TL") ||
                (posInSubShapeString == "TR") ||
                (posInSubShapeString == "TS") ||
                (posInSubShapeString == "RS") ||
                (posInSubShapeString == "LS") ||
                (posInSubShapeString == "C"))
            {
                // Vertical offset needed to put reference corner at world center
                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                {
                    tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));
                }
                else
                {
                    tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));
                }

                // Half the value for a vertical centering requirement
                if ((posInSubShapeString == "RS") ||
                    (posInSubShapeString == "LS") ||
                    (posInSubShapeString == "C"))
                {
                    tmp_yOffset = Convert.ToDouble(tmp_yOffset / 2);
                }
            }
            yOffset -= tmp_yOffset;

            if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 1) && ((posInSubShapeString == "LS") || (posInSubShapeString == "BL") || (posInSubShapeString == "TL")))
            {
                tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength)); // essentially the same in X as the RS for subshape 1.
            }
            else
            {
                if ((posInSubShapeString == "TR") ||
                    (posInSubShapeString == "BR") ||
                    (posInSubShapeString == "TS") ||
                    (posInSubShapeString == "RS") ||
                    (posInSubShapeString == "BS") ||
                    (posInSubShapeString == "C"))
                {
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
                    }
                    else
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength));
                    }

                    // Half the value for horizontal centering conditions
                    if ((posInSubShapeString == "TS") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                        {
                            tmp_xOffset = Convert.ToDouble(tmp_xOffset / 2);
                        }
                        else
                        {
                            tmp_xOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) / 2);
                        }
                    }
                }
            }

            xOffset += tmp_xOffset;
        }

        void tShape_offset(EntropyLayerSettings entropyLayerSettings)
        {
            string posInSubShapeString = ((CommonVars.subShapeLocations)entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex)).ToString();
            double tmp_xOffset = 0;
            double tmp_yOffset = 0;

            if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 1) && ((posInSubShapeString == "BR") || (posInSubShapeString == "BL") || (posInSubShapeString == "BS")))
            {
                tmp_yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset));
            }
            else
            {
                if ((posInSubShapeString == "TL") ||
                (posInSubShapeString == "TR") ||
                (posInSubShapeString == "TS") ||
                (posInSubShapeString == "RS") ||
                (posInSubShapeString == "LS") ||
                (posInSubShapeString == "C"))
                {
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                    {
                        tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));
                        // Half the value for a vertical centering requirement
                        if ((posInSubShapeString == "RS") ||
                            (posInSubShapeString == "LS") ||
                            (posInSubShapeString == "C"))
                        {
                            tmp_yOffset = Convert.ToDouble(tmp_yOffset / 2);
                        }
                    }
                    else
                    {
                        tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));
                        // Half the value for a vertical centering requirement
                        if ((posInSubShapeString == "RS") ||
                            (posInSubShapeString == "LS") ||
                            (posInSubShapeString == "C"))
                        {
                            tmp_yOffset = Convert.ToDouble(tmp_yOffset / 2);
                        }
                        tmp_yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset));
                    }

                }
            }
            yOffset -= tmp_yOffset;

            if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 1) && ((posInSubShapeString == "LS") || (posInSubShapeString == "BL") || (posInSubShapeString == "TL")))
            {
                tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength)); // essentially the same in X as the RS for subshape 1.
            }
            else
            {
                if ((posInSubShapeString == "TR") ||
                    (posInSubShapeString == "BR") ||
                    (posInSubShapeString == "TS") ||
                    (posInSubShapeString == "RS") ||
                    (posInSubShapeString == "BS") ||
                    (posInSubShapeString == "C"))
                {
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
                    }
                    else
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength));
                    }

                    // Half the value for horizontal centering conditions
                    if ((posInSubShapeString == "TS") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                        {
                            tmp_xOffset = Convert.ToDouble(tmp_xOffset / 2);
                        }
                        else
                        {
                            tmp_xOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) / 2);
                        }
                    }
                }
            }

            xOffset += tmp_xOffset;
        }

        void xShape_offset(EntropyLayerSettings entropyLayerSettings)
        {
            string posInSubShapeString = ((CommonVars.subShapeLocations)entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex)).ToString();
            double tmp_xOffset = 0;
            double tmp_yOffset = 0;

            if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 1) && ((posInSubShapeString == "BR") || (posInSubShapeString == "BL") || (posInSubShapeString == "BS")))
            {
                tmp_yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset));
            }
            else
            {
                if ((posInSubShapeString == "TL") ||
                    (posInSubShapeString == "TR") ||
                    (posInSubShapeString == "TS") ||
                    (posInSubShapeString == "RS") ||
                    (posInSubShapeString == "LS") ||
                    (posInSubShapeString == "C"))
                {
                    // Vertical offset needed to put reference corner at world center
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                    {
                        tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));
                        // Half the value for a vertical centering requirement
                        if ((posInSubShapeString == "RS") ||
                            (posInSubShapeString == "LS") ||
                            (posInSubShapeString == "C"))
                        {
                            tmp_yOffset = Convert.ToDouble(tmp_yOffset / 2);
                        }
                    }
                    else
                    {
                        tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));
                        // Half the value for a vertical centering requirement
                        if ((posInSubShapeString == "RS") ||
                            (posInSubShapeString == "LS") ||
                            (posInSubShapeString == "C"))
                        {
                            tmp_yOffset = Convert.ToDouble(tmp_yOffset / 2);
                        }
                        tmp_yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset));
                    }

                }
            }
            yOffset -= tmp_yOffset;

            if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 1) && ((posInSubShapeString == "LS") || (posInSubShapeString == "BL") || (posInSubShapeString == "TL")))
            {
                tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset));
            }
            else
            {
                if ((posInSubShapeString == "TR") ||
                    (posInSubShapeString == "BR") ||
                    (posInSubShapeString == "TS") ||
                    (posInSubShapeString == "RS") ||
                    (posInSubShapeString == "BS") ||
                    (posInSubShapeString == "C"))
                {
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
                    }
                    else
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength));
                    }

                    // Half the value for horizontal centering conditions
                    if ((posInSubShapeString == "TS") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
                        {
                            tmp_xOffset = Convert.ToDouble(tmp_xOffset / 2);
                        }
                        else
                        {
                            tmp_xOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) / 2);
                        }
                    }

                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 1)
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset));
                    }
                }
            }

            xOffset += tmp_xOffset;
        }

        void uShape_offset(EntropyLayerSettings entropyLayerSettings)
        {
            string posInSubShapeString = ((CommonVars.subShapeLocations)entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex)).ToString();
            double tmp_xOffset = 0;
            double tmp_yOffset = 0;

            if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex) == 0)
            {
                if ((posInSubShapeString == "TL") ||
                    (posInSubShapeString == "TR") ||
                    (posInSubShapeString == "TS") ||
                    (posInSubShapeString == "RS") ||
                    (posInSubShapeString == "LS") ||
                    (posInSubShapeString == "C"))
                {
                    tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));

                    // Half the value for a vertical centering requirement
                    if ((posInSubShapeString == "RS") ||
                        (posInSubShapeString == "LS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_yOffset = Convert.ToDouble(tmp_yOffset / 2);
                    }
                }
                yOffset -= tmp_yOffset;

                if ((posInSubShapeString == "TR") ||
                    (posInSubShapeString == "BR") ||
                    (posInSubShapeString == "TS") ||
                    (posInSubShapeString == "RS") ||
                    (posInSubShapeString == "BS") ||
                    (posInSubShapeString == "C"))
                {
                    tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));

                    // Half the value for horizontal centering conditions
                    if ((posInSubShapeString == "TS") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_xOffset = Convert.ToDouble(tmp_xOffset / 2);
                    }
                }
            }
            else
            {
                // Subshape 2 is always docked against top edge of subshape 1 in U.
                if ((posInSubShapeString == "TL") ||
                    (posInSubShapeString == "TR") ||
                    (posInSubShapeString == "TS") ||
                    (posInSubShapeString == "RS") ||
                    (posInSubShapeString == "LS") ||
                    (posInSubShapeString == "BL") ||
                    (posInSubShapeString == "BR") ||
                    (posInSubShapeString == "BS") ||
                    (posInSubShapeString == "C"))
                {
                    tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));

                    // Half the value for a vertical centering requirement
                    if ((posInSubShapeString == "RS") ||
                        (posInSubShapeString == "LS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_yOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength) / 2);
                    }

                    // Subtract the value for a subshape 2 bottom edge requirement
                    if ((posInSubShapeString == "BL") ||
                        (posInSubShapeString == "BR") ||
                        (posInSubShapeString == "BS"))
                    {
                        tmp_yOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));
                    }
                }
                yOffset -= tmp_yOffset;

                // Subshape 2 is always H-centered in U. Makes it easy.
                tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength) / 2);

                if ((posInSubShapeString == "TR") ||
                    (posInSubShapeString == "BR") ||
                    (posInSubShapeString == "RS"))
                {
                    tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) / 2);
                }

                if ((posInSubShapeString == "TL") ||
                    (posInSubShapeString == "BL") ||
                    (posInSubShapeString == "LS"))
                {
                    tmp_xOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength) / 2);
                }
            }
            xOffset += tmp_xOffset;
        }

        void sShape_offset(EntropyLayerSettings entropyLayerSettings)
        {
            string posInSubShapeString = ((CommonVars.subShapeLocations)entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex)).ToString();
            double tmp_xOffset = 0;
            double tmp_yOffset = 0;

            switch (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.subShapeIndex))
            {
                case 0:
                    if ((posInSubShapeString == "TL") ||
                        (posInSubShapeString == "TR") ||
                        (posInSubShapeString == "TS") ||
                        (posInSubShapeString == "RS") ||
                        (posInSubShapeString == "LS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));

                        // Half the value for a vertical centering requirement
                        if ((posInSubShapeString == "RS") ||
                            (posInSubShapeString == "LS") ||
                            (posInSubShapeString == "C"))
                        {
                            tmp_yOffset = Convert.ToDouble(tmp_yOffset / 2);
                        }
                    }

                    if ((posInSubShapeString == "TR") ||
                        (posInSubShapeString == "BR") ||
                        (posInSubShapeString == "TS") ||
                        (posInSubShapeString == "RS") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));

                        // Half the value for horizontal centering conditions
                        if ((posInSubShapeString == "TS") ||
                            (posInSubShapeString == "BS") ||
                            (posInSubShapeString == "C"))
                        {
                            tmp_xOffset = Convert.ToDouble(tmp_xOffset / 2);
                        }
                    }
                    break;

                case 1:
                    // Subshape 2 is always vertically offset relative to bottom edge of subshape 1 in S.
                    if ((posInSubShapeString == "TL") ||
                        (posInSubShapeString == "TR") ||
                        (posInSubShapeString == "TS") ||
                        (posInSubShapeString == "RS") ||
                        (posInSubShapeString == "LS") ||
                        (posInSubShapeString == "BL") ||
                        (posInSubShapeString == "BR") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset));

                        // Half the value for a vertical centering requirement
                        if ((posInSubShapeString == "RS") ||
                            (posInSubShapeString == "LS") ||
                            (posInSubShapeString == "C"))
                        {
                            tmp_yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength) / 2);
                        }

                        // Subtract the value for a subshape 2 bottom edge requirement
                        if ((posInSubShapeString == "TL") ||
                            (posInSubShapeString == "TR") ||
                            (posInSubShapeString == "TS"))
                        {
                            tmp_yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));
                        }
                    }

                    // Subshape 2 is always pinned to left edge in S. Makes it easy.

                    if ((posInSubShapeString == "TR") ||
                        (posInSubShapeString == "BR") ||
                        (posInSubShapeString == "RS") ||
                        (posInSubShapeString == "TS") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength));
                        if ((posInSubShapeString == "TS") ||
                            (posInSubShapeString == "C") ||
                            (posInSubShapeString == "BS"))
                        {
                            tmp_xOffset /= 2;
                        }
                    }

                    break;

                case 2:
                    tmp_yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));
                    // Subshape 3 is always offset relative to top edge of subshape 1 in S.
                    if ((posInSubShapeString == "TL") ||
                        (posInSubShapeString == "TR") ||
                        (posInSubShapeString == "TS") ||
                        (posInSubShapeString == "RS") ||
                        (posInSubShapeString == "LS") ||
                        (posInSubShapeString == "BL") ||
                        (posInSubShapeString == "BR") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_yOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset));

                        // Half the value for a vertical centering requirement
                        if ((posInSubShapeString == "RS") ||
                            (posInSubShapeString == "LS") ||
                            (posInSubShapeString == "C"))
                        {
                            tmp_yOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength) / 2);
                        }

                        // Subtract the value for a subshape 2 bottom edge requirement
                        if ((posInSubShapeString == "BL") ||
                            (posInSubShapeString == "BR") ||
                            (posInSubShapeString == "BS"))
                        {
                            tmp_yOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength));
                        }
                    }

                    // Subshape 3 is always pinned to right edge in S. Makes it easy.
                    tmp_xOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));

                    if ((posInSubShapeString == "TL") ||
                        (posInSubShapeString == "BL") ||
                        (posInSubShapeString == "LS"))
                    {
                        tmp_xOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength));
                    }

                    if ((posInSubShapeString == "TS") ||
                        (posInSubShapeString == "BS") ||
                        (posInSubShapeString == "C"))
                    {
                        tmp_xOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength) / 2);
                    }
                    break;
            }

            yOffset -= tmp_yOffset;
            xOffset += tmp_xOffset;
        }

        void customShape_offset(EntropyLayerSettings entropyLayerSettings)
        {
            return; // disabling this because it affects geometry in annoying ways.
            string posInSubShapeString = ((CommonVars.subShapeLocations)entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.posIndex)).ToString();

            // Get the bounding box.
            Clipper c = new Clipper();
            c.PreserveCollinear = true;
            Paths sourcePolyData = new Paths();
            Paths resizedPolyData = new Paths();
            for (int poly = 0; poly < previewPoints.Count; poly++)
            {
                if (!drawnPoly[poly])
                {
                    Path path_ = new Path();
                    for (int pt = 0; pt < previewPoints[poly].Count(); pt++)
                    {
                        path_.Add(new IntPoint(Convert.ToInt64(previewPoints[poly][pt].X * CentralProperties.scaleFactorForOperation),
                                               Convert.ToInt64(previewPoints[poly][pt].Y * CentralProperties.scaleFactorForOperation)));
                    }
                    sourcePolyData.Add(path_);
                }
            }

            IntRect bounds = Clipper.GetBounds(sourcePolyData);
            double minY = Math.Min(bounds.top, bounds.bottom) / CentralProperties.scaleFactorForOperation;
            double maxY = Math.Max(bounds.top, bounds.bottom) / CentralProperties.scaleFactorForOperation;

            double minX = Math.Min(bounds.left, bounds.right) / CentralProperties.scaleFactorForOperation;
            double maxX = Math.Max(bounds.left, bounds.right) / CentralProperties.scaleFactorForOperation;

            if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE)
            {
                minX = Math.Min(0, minX);
                minY = Math.Min(0, minY);
            }
            else if (DOEDependency)
            {
                minX = Math.Min(doeMinX, minX);
                minY = Math.Min(doeMinY, minY);
            }

            double tmp_xOffset = 0;
            double tmp_yOffset = 0;

            if ((posInSubShapeString == "TL") ||
                (posInSubShapeString == "TR") ||
                (posInSubShapeString == "TS"))
            {
                // Vertical offset needed to put reference corner at world center
                tmp_yOffset = -maxY;
            }

            if ((posInSubShapeString == "BL") ||
                (posInSubShapeString == "BR") ||
                (posInSubShapeString == "BS"))
            {
                // Vertical offset needed to put reference corner at world center
                tmp_yOffset = -minY;
            }

            // Half the value for a vertical centering requirement
            if ((posInSubShapeString == "RS") ||
                (posInSubShapeString == "LS") ||
                (posInSubShapeString == "C"))
            {
                tmp_yOffset = -(minY + (maxY - minY) / 2.0f);
            }
            yOffset -= tmp_yOffset;

            if ((posInSubShapeString == "TR") ||
                (posInSubShapeString == "BR") ||
                (posInSubShapeString == "RS"))
            {
                tmp_xOffset = -maxX;
            }

            if ((posInSubShapeString == "TL") ||
                (posInSubShapeString == "BL") ||
                (posInSubShapeString == "LS"))
            {
                tmp_xOffset = -minX;
            }

            // Half the value for horizontal centering conditions
            if ((posInSubShapeString == "TS") ||
                (posInSubShapeString == "BS") ||
                (posInSubShapeString == "C"))
            {
                tmp_xOffset = -(minX + (maxX - minX) / 2.0f);
            }

            xOffset += tmp_xOffset;

            for (Int32 poly = 0; poly < previewPoints.Count(); poly++)
            {
                for (Int32 point = 0; point < previewPoints[poly].Count(); point++)
                {
                    double px = previewPoints[poly][point].X + xOffset;
                    double py = previewPoints[poly][point].Y - yOffset;

                    previewPoints[poly][point] = new GeoLibPointF(px, py);
                }
                if ((previewPoints[poly][0].X != previewPoints[poly][previewPoints[poly].Count() - 1].X) ||
                    (previewPoints[poly][0].Y != previewPoints[poly][previewPoints[poly].Count() - 1].Y))
                {
                    ErrorReporter.showMessage_OK("Start and end not the same - previewShape", "Oops");
                }
            }
        }

        void doOffsets(EntropyLayerSettings entropyLayerSettings)
        {
            // Use our shape-specific offset calculation methods :
            xOffset = 0;
            yOffset = 0;

            switch (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex))
            {
                case (Int32)CentralProperties.typeShapes.rectangle:
                    rectangle_offset(entropyLayerSettings);
                    break;
                case (Int32)CentralProperties.typeShapes.L:
                    lShape_offset(entropyLayerSettings);
                    break;
                case (Int32)CentralProperties.typeShapes.T:
                    tShape_offset(entropyLayerSettings);
                    break;
                case (Int32)CentralProperties.typeShapes.X:
                    xShape_offset(entropyLayerSettings);
                    break;
                case (Int32)CentralProperties.typeShapes.U:
                    uShape_offset(entropyLayerSettings);
                    break;
                case (Int32)CentralProperties.typeShapes.S:
                    sShape_offset(entropyLayerSettings);
                    break;
                case (Int32)CentralProperties.typeShapes.BOOLEAN:
                case (Int32)CentralProperties.typeShapes.GEOCORE:
                    // customShape_offset(entropyLayerSettings);
                    break;
                default:
                    break;
            }

            // Now for global offset.
            xOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset));
            yOffset -= Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset));
        }

        public PreviewShape()
        {
            init();
        }

        void init()
        {
            // Stub to enable direct drive of preview data, primarily for the implant system.
            previewPoints = new List<GeoLibPointF[]>();
            drawnPoly = new List<bool>();
            geoCoreOrthogonalPoly = new List<bool>();
            color = MyColor.Black;
        }

        public PreviewShape(PreviewShape source)
        {
            init(source);
        }

        void init(PreviewShape source)
        {
            _settingsIndex = source._settingsIndex;
            previewPoints = source.previewPoints.ToList();
            drawnPoly = source.drawnPoly.ToList();
            geoCoreOrthogonalPoly = source.geoCoreOrthogonalPoly.ToList();
            color = new MyColor(source.color);
        }

        public PreviewShape(CommonVars commonVars, Int32 settingsIndex, Int32 subShapeIndex, Int32 mode, bool doPASearch, bool previewMode, Int32 currentRow, Int32 currentCol)
        {
            xOffset = 0;
            yOffset = 0;
            init(commonVars, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol);
        }

        public PreviewShape(CommonVars commonVars, ChaosSettings jobSettings_, Int32 settingsIndex, Int32 subShapeIndex, Int32 mode, bool doPASearch, bool previewMode, Int32 currentRow, Int32 currentCol)
        {
            xOffset = 0;
            yOffset = 0;
            init(commonVars, jobSettings_, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol);
        }

        bool exitEarly = false;

        void distortion(CommonVars commonVars, Int32 settingsIndex)
        {
            for (Int32 poly = 0; poly < previewPoints.Count(); poly++)
            {
                // Now let's get some barrel distortion sorted out. Only for non-drawn polygons, and skip if both coefficients are zero to avoid overhead.
                if ((!drawnPoly[poly]) && ((commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC1) != 0) || (commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC2) != 0)))
                {
                    int pCount = previewPoints[poly].Count();
                    Parallel.For(0, pCount, (point) =>
                    // for (Int32 point = 0; point < pCount; point++)
                    {
                        double px = previewPoints[poly][point].X;
                        double py = previewPoints[poly][point].Y;

                        // Coefficients during testing.
                        /*
                        double k1 = 1000.0;
                        double k2 = 500.0;
                        */

                        // Need to calculate a new 'radius' from the origin for each point in the polygon, then scale the X/Y values accordingly in the polygon.
                        // Use scale factor to try and guarantee a -1 to +1 value range
                        px /= CentralProperties.scaleFactorForOperation;
                        py /= CentralProperties.scaleFactorForOperation;

                        double oRadius = Math.Sqrt(Utils.myPow(px, 2) + Utils.myPow(py, 2));
                        // Polynomial radial distortion.
                        // rd = r(1 + (k1 * r^2) + (k2 * r^4)) from Zhang, 1999 (https://www.microsoft.com/en-us/research/wp-content/uploads/2016/11/zhan99.pdf)
                        // we only want a scaling factor for our X, Y coordinates.
                        // '1 -' or '1 +' drive the pincushion/barrel tone. Coefficients being negative will have the same effect, so just pick a direction and stick with it.
                        int amplifier = 1000; // scales up end-user values to work within this approach.
                        double t1 = Convert.ToDouble(commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC1)) * amplifier * Utils.myPow(Math.Abs(oRadius), 2);
                        double t2 = Convert.ToDouble(commonVars.getLayerSettings(settingsIndex).getDecimal(EntropyLayerSettings.properties_decimal.lDC2)) * Utils.myPow(amplifier, 2) * Utils.myPow(Math.Abs(oRadius), 4);
                        double sFactor = 1 - (t1 + t2);

                        px *= sFactor * CentralProperties.scaleFactorForOperation;
                        py *= sFactor * CentralProperties.scaleFactorForOperation;

                        previewPoints[poly][point] = new GeoLibPointF(px, py);

                        // Refragment
                        previewPoints[poly] = fragment.fragmentPath(previewPoints[poly]);
                    });
                }
            }
        }

        void doNoise(int noiseType, int seed, double freq, double amount, double jitterScale)
        {
            // Gets a -1 to +1 noise field. We get a seed from our RNG of choice unless the layer preview mode is set, where a fixed seed is used.
            // Random constants to mitigate continuity effects in noise that cause nodes in the noise across multiple layers, due to periodicity.
            double x_const = 123489.1928734;
            double y_const = 891243.0982134;

            object noiseSource;

            switch (noiseType)
            {
                case (Int32)CommonVars.noiseIndex.opensimplex:
                    noiseSource = new OpenSimplexNoise(seed);
                    break;
                case (Int32)CommonVars.noiseIndex.simplex:
                    noiseSource = new SimplexNoise(seed);
                    break;
                default:
                    noiseSource = new PerlinNoise(seed);
                    break;
            }

            // Need to iterate our preview points.
            for (int poly = 0; poly < previewPoints.Count(); poly++)
            {
                if ((previewPoints[poly].Count() <= 1) || (drawnPoly[poly]))
                {
                    continue;
                }
                GeoLibPointF[] mcPoints = previewPoints[poly].ToArray();
                int ptCount = mcPoints.Count();

                // Create our jittered polygon in a new list to avoid breaking normal computation, etc. by modifying the source.
                object jitterLock = new object();
                GeoLibPointF[] jitteredPoints = new GeoLibPointF[ptCount];

                // We could probably simply cast rays in the raycaster and use those, but for now reinvent the wheel here...
                GeoLibPointF[] normals = new GeoLibPointF[ptCount];
                GeoLibPointF[] previousNormals = new GeoLibPointF[ptCount];
                double dx = 0;
                double dy = 0;
                // Pre-calculate these for the threading to be an option.
                // This is a serial evaluation as we need both the previous and the current normal for each point.
                Parallel.For(0, ptCount - 1, (pt) => 
                // for (Int32 pt = 0; pt < ptCount - 1; pt++)
                {
                    if (pt == 0)
                    {
                        // First vertex needs special care.
                        dx = mcPoints[0].X - mcPoints[ptCount - 2].X;
                        dy = mcPoints[0].Y - mcPoints[ptCount - 2].Y;
                    }
                    else
                    {
                        dx = mcPoints[pt + 1].X - mcPoints[pt].X;
                        dy = mcPoints[pt + 1].Y - mcPoints[pt].Y;
                    }
                    normals[pt] = new GeoLibPointF(-dy, dx);
                });
                normals[normals.Length - 1] = new GeoLibPointF(normals[0]);

                int nLength = normals.Length;
                Parallel.For(1, nLength, (pt) => 
                // for (int pt = 1; pt < nLength; pt++)
                {
                    previousNormals[pt] = new GeoLibPointF(normals[pt - 1]);
                });

                previousNormals[0] = new GeoLibPointF(normals[normals.Length - 2]);

                Parallel.For(0, ptCount - 1, pt =>
                // for (int pt = 0; pt < ptCount - 1; pt++)
                {
                    // We need to average the normals of two edge segments to get the vector we need to displace our point along.
                    // This ensures that we handle corners and fluctuations in a reasonable manner.
                    GeoLibPointF averagedEdgeNormal = new GeoLibPointF((previousNormals[pt].X + normals[pt].X) / 2.0f, (previousNormals[pt].Y + normals[pt].Y) / 2.0f);
                    // Normalize our vector length.
                    double length = Math.Sqrt(Utils.myPow(averagedEdgeNormal.X, 2) + Utils.myPow(averagedEdgeNormal.Y, 2));
                    double normalTolerance = 1E-3;
                    if (length < normalTolerance)
                    {
                        length = normalTolerance;
                    }
                    averagedEdgeNormal.X /= length;
                    averagedEdgeNormal.Y /= length;

                    // Use a tolerance as we're handling floats; we don't expect a normalized absolute value generally above 1.0, ignoring the float error.
                    /*
                    if ((Math.Abs(averagedEdgeNormal.X) - 1 > normalTolerance) || (Math.Abs(averagedEdgeNormal.Y) - 1 > normalTolerance))
                    {
                        ErrorReporter.showMessage_OK("averageNormal exceeded limits: X:" + averagedEdgeNormal.X.ToString() + ",Y:" + averagedEdgeNormal.Y.ToString(), "oops");
                    }
                    */

                    // We can now modify the position of our point and stuff it into our jittered list.
                    double jitterAmount = 0;

                    switch (noiseType)
                    {
                        case (Int32)CommonVars.noiseIndex.opensimplex:
                            jitterAmount = ((OpenSimplexNoise)noiseSource).Evaluate(freq * (mcPoints[pt].X + x_const), freq * (mcPoints[pt].Y + y_const));
                            break;
                        case (Int32)CommonVars.noiseIndex.simplex:
                            jitterAmount = ((SimplexNoise)noiseSource).GetNoise(freq * (mcPoints[pt].X + x_const), freq * (mcPoints[pt].Y + y_const));
                            break;
                        default:
                            jitterAmount = ((PerlinNoise)noiseSource).Noise(freq * (mcPoints[pt].X + x_const), freq * (mcPoints[pt].Y + y_const), 0);
                            break;
                    }

                    jitterAmount *= jitterScale;

                    double jitteredX = mcPoints[pt].X;
                    jitteredX += jitterAmount * averagedEdgeNormal.X;

                    double jitteredY = mcPoints[pt].Y;
                    jitteredY += jitterAmount * averagedEdgeNormal.Y;

                    jitteredPoints[pt] = new GeoLibPointF(jitteredX, jitteredY);
                });
                jitteredPoints[ptCount - 1] = new GeoLibPointF(jitteredPoints[0]);

                // Push back to mcPoints for further processing.
                previewPoints[poly] = jitteredPoints.ToArray();
            }
        }

        void applyNoise(bool previewMode, CommonVars commonVars, ChaosSettings jobSettings, int settingsIndex)
        {

            EntropyLayerSettings entropyLayerSettings = commonVars.getLayerSettings(settingsIndex);

            double lwrConversionFactor;
            if (commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.ler) == 1)
            {
                // On the basis that LWR result of two uncorrelated LER variations so should be treated as RSS().
                lwrConversionFactor = Math.Sqrt(2);
            }
            else
            {
                lwrConversionFactor = 0.5f;
            }

            // LWR, skip if not requested to avoid runtime pain
            if (((!previewMode) || (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1)) && (entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr) != 0))
            {
                double jitterScale = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr)) / lwrConversionFactor; // LWR jitter of edge; use RSS for stricter assessment
                if (!previewMode && (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1) && !jobSettings.getPreviewMode())
                {
                    // This used to be easier, but now we have the case of a non-preview mode, but the layer setting calls for a preview.
                    jitterScale *= jobSettings.getValue(ChaosSettings.properties.LWRVar, settingsIndex);
                }

                doNoise(
                    noiseType: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrType),
                    seed: jobSettings.getInt(ChaosSettings.ints.lwrSeed, settingsIndex),
                    freq: Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwrFreq)),
                    amount: Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr)),
                    jitterScale: jitterScale
                );
            }

            // LWR2, skip if not requested to avoid runtime pain
            if (((!previewMode) || (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1)) && (entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr2) != 0))
            {
                double jitterScale = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr2)) / lwrConversionFactor; // LWR jitter of edge; use RSS for stricter assessment
                if (!previewMode && (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwrPreview) == 1) && !jobSettings.getPreviewMode())
                {
                    // This used to be easier, but now we have the case of a non-preview mode, but the layer setting calls for a preview.
                    jitterScale *= jobSettings.getValue(ChaosSettings.properties.LWR2Var, settingsIndex);
                }

                doNoise(
                    noiseType: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lwr2Type),
                    seed: jobSettings.getInt(ChaosSettings.ints.lwr2Seed, settingsIndex),
                    freq: Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq)),
                    amount: Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.lwr2)),
                    jitterScale: jitterScale
                );
            }
        }

        void proximityBias(CommonVars commonVars, ChaosSettings jobSettings, int settingsIndex)
        {
            // Proximity biasing - where isolated edges get bias based on distance to nearest supporting edge.

            EntropyLayerSettings entropyLayerSettings = commonVars.getLayerSettings(settingsIndex);

            bool proxBiasNeeded = ((entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.pBias) != 0) && (entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.pBiasDist) != 0));

            if (!proxBiasNeeded)
            {
                return;
            }

            bool debug = false;
            bool linear = false;

            List<GeoLibPointF[]> updatedPolys = new List<GeoLibPointF[]>();
            List<GeoLibPointF[]> preOverlapMergePolys = new List<GeoLibPointF[]>();
            List<bool> updatedDrawn = new List<bool>();

            Paths clippedLines = new Paths();
            Paths dRays = new Paths();

            // Scale up our geometry for processing. Force a clockwise point order here due to potential upstream point order changes (e.g. polygon merging)
            Paths sourceGeometry = GeoWrangler.pathsFromPointFs(previewPoints, CentralProperties.scaleFactorForOperation);

            // sourceGeometry = GeoWrangler.removeKeyHoles(sourceGeometry, 1);

            int sCount = sourceGeometry.Count();
            for (int poly = 0; poly < sCount; poly++)
            {
                if ((sourceGeometry[poly].Count() <= 1) || (drawnPoly[poly]))
                {
                    updatedPolys.Add(previewPoints[poly]);
                    if (drawnPoly[poly])
                    {
                        updatedDrawn.Add(true);
                    }
                    else
                    {
                        updatedDrawn.Add(false);
                    }
                    // Nothing to do with drawn or zero count entries.
                    continue;
                }

                Path sourcePoly = sourceGeometry[poly].ToList();
                Path deformedPoly = new Path();

                // Threading operation here gets more tricky than the distance handler. We have a less clear trade off of threading based on the emission edge (the polygon being biased) vs the multisampling emission.
                // In batch calculation mode, this tradeoff gets more awkward.
                // Threading both options also causes major performance degradation as far too many threads are spawned for the host system.
                bool multiSampleThread = false;
                bool emitThread = false;

                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.proxRays) > 1)
                {
                    multiSampleThread = true;
                    // for multipolygon scenarios, avoid threading the multisampling and instead favor threading emitting edge.
                    if (sourceGeometry.Count > 1)
                    {
                        emitThread = true;
                        multiSampleThread = false;
                    }
                }
                else
                {
                    emitThread = true;
                }

                RayCast rc = new RayCast(sourcePoly, sourceGeometry, Convert.ToInt32(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.pBiasDist) * CentralProperties.scaleFactorForOperation), false, false, entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.proxRays), emitThread, multiSampleThread);

                clippedLines = rc.getClippedRays().ToList();
                if (debug)
                {
                    dRays = rc.getClippedRays().ToList();
                }

                // We hope to get the same number of clipped lines back as the number of points that went in....
                int cLCount = clippedLines.Count;
                for (int line = 0; line < cLCount; line++)
                {
                    Int64 displacedX = sourcePoly[line].X;
                    Int64 displacedY = sourcePoly[line].Y;

                    double lineLength = rc.getRayLength(line);

                    // No biasing - ray never made it beyond the surface. Short-cut the 
                    if (lineLength == 0)
                    {
                        deformedPoly.Add(new IntPoint(clippedLines[line][0]));
                        continue;
                    }
                    if (lineLength < 0)
                    {
                        lineLength *= -1;
                    }

                    // Calculate our bias based on this distance and apply it.
                    double biasScaling = (lineLength / CentralProperties.scaleFactorForOperation) / Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.pBiasDist));

                    if (biasScaling > 1)
                    {
                        biasScaling = 1;
                    }

                    // Probably should be a sigmoid, but using this for now.
                    double displacedAmount = 0;

                    if (linear)
                    {
                        displacedAmount = biasScaling * Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.pBias)) * CentralProperties.scaleFactorForOperation;
                    }
                    else
                    {
                        // Using sine to make a ease-in/ease-out effect.
                        displacedAmount = Math.Sin(Utils.toRadians(biasScaling * 90.0f)) * Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.pBias)) * CentralProperties.scaleFactorForOperation;
                    }

                    // Use our cast ray from rc to get a normalized average 
                    IntPoint averagedEdgeNormal = GeoWrangler.intPoint_distanceBetweenPoints(clippedLines[line][clippedLines[line].Count() - 1], clippedLines[line][0]);

                    // Normalize our vector length.
                    double aX = averagedEdgeNormal.X / lineLength;
                    double aY = averagedEdgeNormal.Y / lineLength;

                    displacedY += (Int64)(displacedAmount * aY);
                    displacedX += (Int64)(displacedAmount * aX);

                    deformedPoly.Add(new IntPoint(displacedX, displacedY));
                }
                preOverlapMergePolys.Add(GeoWrangler.pointFFromPath(deformedPoly, CentralProperties.scaleFactorForOperation));
                deformedPoly.Add(new IntPoint(deformedPoly[0]));
            }

            // Check for overlaps and process as needed post-biasing.
            processOverlaps(commonVars, settingsIndex, preOverlapMergePolys, forceOverride: false, PolyFillType.pftNonZero);

            if (debug)
            {
                for (int ray = 0; ray < dRays.Count; ray++)
                {
                    previewPoints.Add(GeoWrangler.pointFFromPath(dRays[ray], CentralProperties.scaleFactorForOperation));
                    drawnPoly.Add(true);
                }
            }
        }

        void init(CommonVars commonVars, Int32 settingsIndex, Int32 subShapeIndex, Int32 mode, bool doPASearch, bool previewMode, Int32 currentRow, Int32 currentCol)
        {
            ChaosSettings jobSettings_ = new ChaosSettings(previewMode, commonVars.getListOfSettings(), commonVars.getSimulationSettings());
            init(commonVars, jobSettings_, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol);
        }

        void init(CommonVars commonVars, ChaosSettings chaosSettings, Int32 settingsIndex, Int32 subShapeIndex, Int32 mode, bool doPASearch, bool previewMode, Int32 currentRow, Int32 currentCol, EntropyLayerSettings entropyLayerSettings = null, bool doClockwiseGeoFix = true, bool process_overlaps = true)
        {
            _settingsIndex = settingsIndex;
            try
            {
                DOEDependency = false;
                fragment = new Fragmenter(commonVars.getSimulationSettings().getResolution(), CentralProperties.scaleFactorForOperation);
                previewPoints = new List<GeoLibPointF[]>();
                drawnPoly = new List<bool>();
                geoCoreOrthogonalPoly = new List<bool>();
                color = MyColor.Black; // overridden later.

                if (entropyLayerSettings == null)
                {
                    entropyLayerSettings = commonVars.getLayerSettings(settingsIndex);
                }
                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE)
                {
                    init_geoCore(commonVars, chaosSettings, settingsIndex, entropyLayerSettings, mode, doPASearch, previewMode, process_overlaps, doClockwiseGeoFix);
                    // Get our offsets configured. We need to check for DOE settings here, to prevent relocation of extracted polygons within the tile during offset evaluation.
                    if (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1)
                    {
                        DOEDependency = true;
                        doeMinX = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
                        doeMinY = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
                    }
                    doOffsets(entropyLayerSettings);
                }
                else // not geoCore related.
                {
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.BOOLEAN)
                    {
                        try
                        {
                            init_boolean(commonVars, chaosSettings, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol, entropyLayerSettings);
                            // Get our offsets configured.
                            // Is any input layer coming from a GDS DOE tile? We need to check for DOE settings here, to prevent relocation of extracted polygons within the tile during offset evaluation.
                            int boolLayer = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerA);
                            while (boolLayer > 0)
                            {
                                DOEDependency = (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(boolLayer) == 1);
                                if (DOEDependency)
                                {
                                    break;
                                }
                                boolLayer = commonVars.getLayerSettings(boolLayer).getInt(EntropyLayerSettings.properties_i.bLayerA);
                            }
                            if (!DOEDependency)
                            {
                                boolLayer = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerB);
                                while (boolLayer > 0)
                                {
                                    DOEDependency = (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(boolLayer) == 1);
                                    if (DOEDependency)
                                    {
                                        break;
                                    }
                                    boolLayer = commonVars.getLayerSettings(boolLayer).getInt(EntropyLayerSettings.properties_i.bLayerB);
                                }
                            }
                            if (DOEDependency)
                            {
                                doeMinX = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.colOffset);
                                doeMinY = commonVars.getSimulationSettings().getDOESettings().getDouble(DOESettings.properties_d.rowOffset);
                            }

                            doOffsets(entropyLayerSettings);
                        }
                        catch (Exception)
                        {
                        }
                        exitEarly = true; // avoid second pass of distortion, etc.
                    }
                    else
                    {
                        if (mode == 0)
                        {
                            // Basic shape - 5 points to make a closed preview. 5th is identical to 1st.
                            GeoLibPointF[] tempArray = new GeoLibPointF[5];

                            // Need exception handling here for overflow cases?
                            Decimal bottom_leftX = 0, bottom_leftY = 0;
                            Decimal top_leftX = 0, top_leftY = 0;
                            Decimal top_rightX = 0, top_rightY = 0;
                            Decimal bottom_rightX = 0, bottom_rightY = 0;
                            if (subShapeIndex == 0)
                            {
                                bottom_leftX = 0;
                                bottom_leftY = 0;
                                top_leftX = 0;
                                top_leftY = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength);
                                top_rightX = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength);
                                top_rightY = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength);
                                bottom_rightX = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength);
                                bottom_rightY = 0;
                                xOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset));
                                yOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset));
                            }
                            if (subShapeIndex == 1)
                            {
                                bottom_leftX = 0;
                                bottom_leftY = 0;
                                top_leftX = 0;
                                top_leftY = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);
                                top_rightX = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength);
                                top_rightY = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength);
                                bottom_rightX = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength);
                                bottom_rightY = 0;
                                xOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset));
                                yOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset));
                            }
                            if (subShapeIndex == 2)
                            {
                                bottom_leftX = 0;
                                bottom_leftY = 0;
                                top_leftX = 0;
                                top_leftY = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength);
                                top_rightX = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength);
                                top_rightY = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength);
                                bottom_rightX = entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength);
                                bottom_rightY = 0;
                                xOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset));
                                yOffset = -Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset));
                                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.Sshape)
                                {
                                    yOffset += Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength)); // offset our subshape to put it in the correct place in the UI.
                                }
                            }

                            // Populate array.
                            tempArray[0] = new GeoLibPointF((double)bottom_leftX, (double)bottom_leftY);
                            tempArray[1] = new GeoLibPointF((double)top_leftX, (double)top_leftY);
                            tempArray[2] = new GeoLibPointF((double)top_rightX, (double)top_rightY);
                            tempArray[3] = new GeoLibPointF((double)bottom_rightX, (double)bottom_rightY);
                            tempArray[4] = new GeoLibPointF(tempArray[0]);

                            // Apply our deltas
                            int tLength = tempArray.Length;
                            Parallel.For(0, tLength, (i) => 
                            //for (Int32 i = 0; i < tLength; i++)
                            {
                                tempArray[i].X += xOffset;
                                tempArray[i].Y += yOffset;
                            });
                            previewPoints.Add(tempArray);
                            drawnPoly.Add(true);
                        }
                        else
                        {
                            // Complex shape
                            try
                            {
                                EntropyShape complexPoints = new EntropyShape(commonVars.getSimulationSettings(), commonVars.getListOfSettings(), settingsIndex, doPASearch, previewMode, chaosSettings);
                                previewPoints.Add(complexPoints.getPoints());
                                drawnPoly.Add(false);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        // Get our offsets configured.
                        doOffsets(entropyLayerSettings);

                        int pCount = previewPoints.Count;
                        for (Int32 poly = 0; poly < pCount; poly++)
                        {
                            int ptCount = previewPoints[poly].Count();
                            Parallel.For(0, ptCount, (point) =>
                            // for (Int32 point = 0; point < ptCount; point++)
                            {
                                double px = previewPoints[poly][point].X + xOffset;
                                double py = previewPoints[poly][point].Y - yOffset;

                                previewPoints[poly][point] = new GeoLibPointF(px, py);
                            });
                            if ((previewPoints[poly][0].X != previewPoints[poly][previewPoints[poly].Count() - 1].X) ||
                                (previewPoints[poly][0].Y != previewPoints[poly][previewPoints[poly].Count() - 1].Y))
                            {
                                ErrorReporter.showMessage_OK("Start and end not the same - previewShape", "Oops");
                            }
                        }
                    }
                }

                if ((!exitEarly) && (mode == 1))
                {
                    // Apply lens distortion.
                    distortion(commonVars, settingsIndex);
                    // Noise and proximity biasing.
                    applyNoise(previewMode, commonVars, chaosSettings, settingsIndex);
                    proximityBias(commonVars, chaosSettings, settingsIndex);
                }
            }
            catch (Exception)
            {
            }
        }

        void init_geoCore(CommonVars commonVars, ChaosSettings chaosSettings, int settingsIndex, EntropyLayerSettings entropyLayerSettings, Int32 mode, bool doPASearch, bool previewMode, bool process_overlaps, bool forceClockwise)
        {
            // We'll use these to shift the points around.
            double xOverlayVal = 0.0f;
            double yOverlayVal = 0.0f;

            xOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset));
            yOffset = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset) + entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset));

            if (mode == 1)
            {
                // We need this check and early return because previewShape is now used in the layer preview
                // mode to handle bias on geoCore elements. Populating this when the layer is not enabled
                // causes the shared structure with the simulation engine to be defined and breaks everything.
                // Instead we just make a zero area polygon (to avoid issues downstream) and return early.
                if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.enabled) == 0)
                {
                    previewPoints.Add(new GeoLibPointF[4]);
                    for (int i = 0; i < 4; i++)
                    {
                        previewPoints[0][i] = new GeoLibPointF(0, 0);
                    }
                    drawnPoly.Add(false);
                    geoCoreOrthogonalPoly.Add(true);
                    return;
                }
                // OK. We need to crop our layout based on the active tile if there is a DOE flag set.
                bool tileHandlingNeeded = false;
                if (commonVars.getSimulationSettings().getDOESettings().getLayerAffected(settingsIndex) == 1)
                {
                    tileHandlingNeeded = true;
                }

                if (previewMode && tileHandlingNeeded)
                {
                    if (!commonVars.getLayerPreviewDOETile())
                    {
                        tileHandlingNeeded = false;
                    }
                }

                // Get overlay figured out.
                if (!previewMode)
                {
                    xOverlayVal = (chaosSettings.getValue(ChaosSettings.properties.overlayX, settingsIndex) * Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.xOL)));
                    yOverlayVal = (chaosSettings.getValue(ChaosSettings.properties.overlayY, settingsIndex) * Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.yOL)));

                    // Handle overlay reference setting
                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_av) == 1) // overlay average
                    {
                        List<double> overlayValues = new List<double>();
                        for (int avgolref_x = 0; avgolref_x < entropyLayerSettings.getIntArray(EntropyLayerSettings.properties_intarray.xOLRefs).Length; avgolref_x++)
                        {
                            if (entropyLayerSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, avgolref_x) == 1)
                            {
                                overlayValues.Add(chaosSettings.getValue(ChaosSettings.properties.overlayX, avgolref_x) * Convert.ToDouble(commonVars.getLayerSettings(avgolref_x).getDecimal(EntropyLayerSettings.properties_decimal.xOL))); // Overlay shift
                            }
                        }

                        xOverlayVal += overlayValues.Average();
                    }
                    else // vanilla overlay reference mode
                    {
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) != -1)
                        {
                            xOverlayVal += (chaosSettings.getValue(ChaosSettings.properties.overlayX, entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref)) * Convert.ToDouble(commonVars.getLayerSettings(entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref)).getDecimal(EntropyLayerSettings.properties_decimal.xOL)));
                        }
                    }

                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_av) == 1) // overlay average
                    {
                        List<double> overlayValues = new List<double>();
                        for (int avgolref_y = 0; avgolref_y < entropyLayerSettings.getIntArray(EntropyLayerSettings.properties_intarray.yOLRefs).Length; avgolref_y++)
                        {
                            if (entropyLayerSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, avgolref_y) == 1)
                            {
                                overlayValues.Add(chaosSettings.getValue(ChaosSettings.properties.overlayY, avgolref_y) * Convert.ToDouble(commonVars.getLayerSettings(avgolref_y).getDecimal(EntropyLayerSettings.properties_decimal.yOL))); // Overlay shift
                            }
                        }

                        yOverlayVal += overlayValues.Average();
                    }
                    else // vanilla overlay reference mode
                    {
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) != -1)
                        {
                            yOverlayVal += (chaosSettings.getValue(ChaosSettings.properties.overlayY, entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref)) * Convert.ToDouble(commonVars.getLayerSettings(entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref)).getDecimal(EntropyLayerSettings.properties_decimal.yOL)));
                        }
                    }
                }

                // Decouple the geometry here to avoid manipulation going back to original source.
                List<GeoLibPointF[]> tempPolyList = new List<GeoLibPointF[]>();
                if (tileHandlingNeeded)
                {
                    tempPolyList = commonVars.getNonSimulationSettings().extractedTile[settingsIndex].ToList();
                }
                else
                {
                    tempPolyList = entropyLayerSettings.getFileData().ToList();
                }
                try
                {
                    double minx = tempPolyList[0][0].X;
                    double miny = tempPolyList[0][0].Y;
                    double maxx = tempPolyList[0][0].X;
                    double maxy = tempPolyList[0][0].Y;
                    int tPCount = tempPolyList.Count;
                    for (Int32 poly = 0; poly < tPCount; poly++)
                    {
                        double min_x = tempPolyList[poly].Min(p => p.X);
                        double min_y = tempPolyList[poly].Min(p => p.Y);
                        double max_x = tempPolyList[poly].Max(p => p.X);
                        double max_y = tempPolyList[poly].Max(p => p.Y);

                        if (min_x < minx)
                        {
                            minx = min_x;
                        }
                        if (min_y < miny)
                        {
                            miny = min_y;
                        }
                        if (max_x > maxx)
                        {
                            maxx = max_x;
                        }
                        if (max_y > maxy)
                        {
                            maxy = max_y;
                        }
                    }

                    GeoLibPointF bb_mid = new GeoLibPointF(minx + (maxx - minx) / 2.0f, miny + (maxy - miny) / 2.0f);
                    bb_mid.X += xOverlayVal + (double)entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset) + (double)entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset);
                    bb_mid.Y += yOverlayVal + (double)entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset) + (double)entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset);

                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.perPoly) == 1)
                    {
                        bb_mid = null;
                    }

                    for (int poly = 0; poly < tPCount; poly++)
                    {
                        GeoLibPointF[] tempPoly;

                        if (tileHandlingNeeded)
                        {
                            // Poly is already closed - presents a problem if we use contouring.
                            int arraySize = tempPolyList[poly].Count();

                            if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1)
                            {
                                if ((tempPolyList[poly][0].X == tempPolyList[poly][tempPolyList[poly].Length - 1].X) && (tempPolyList[poly][0].Y == tempPolyList[poly][tempPolyList[poly].Length - 1].Y))
                                {
                                    arraySize--;
                                }
                            }

                            tempPoly = new GeoLibPointF[arraySize];

                            Parallel.For(0, arraySize, (pt) => 
                            // for (int pt = 0; pt < arraySize; pt++)
                            {
                                tempPoly[pt] = new GeoLibPointF(tempPolyList[poly][pt].X + xOffset, tempPolyList[poly][pt].Y + yOffset);
                            });
                        }
                        else
                        {
                            int polySize = entropyLayerSettings.getFileData()[poly].Count();

                            tempPoly = new GeoLibPointF[polySize];

                            Parallel.For(0, polySize, (pt) => 
                            // for (Int32 pt = 0; pt < polySize; pt++)
                            {
                                tempPoly[pt] = new GeoLibPointF(entropyLayerSettings.getFileData()[poly][pt].X + xOffset, entropyLayerSettings.getFileData()[poly][pt].Y + yOffset);
                            });
                        }

                        bool drawn = false;

                        // Compatibility shim - we need to toggle this behavior due to the ILB passing in mixed orientation geometry that we don't want to clobber.
                        // However, external geometry may need this spin fixing. Although the upper levels should also re-spin geometry properly - we don't assume this.
                        if (forceClockwise)
                        {
                            tempPoly = GeoWrangler.clockwiseAndReorder(tempPoly); // force clockwise order and lower-left at 0 index.
                        }

                        // Strip termination points. Set shape will take care of additional clean-up if needed.
                        tempPoly = GeoWrangler.stripTerminators(tempPoly, false);

                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
                        {
                            previewPoints.Add(fragment.fragmentPath(tempPoly.ToArray()));
                            geoCoreOrthogonalPoly.Add(false); // We need to populate the list, but in this non-contoured case, the value doesn't matter.
                        }
                        else
                        {
                            // Feed tempPoly to shape engine.
                            ShapeLibrary shape = new ShapeLibrary(entropyLayerSettings);

                            shape.setShape(entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex), tempPoly.ToArray()); // feed the shape engine with the geometry using our optional parameter.
                            EntropyShape complexPoints = new EntropyShape(commonVars.getSimulationSettings(), commonVars.getListOfSettings(), settingsIndex, doPASearch, previewMode, chaosSettings, shape, bb_mid);
                            // Add resulting shape to the previewPoints.
                            previewPoints.Add(complexPoints.getPoints());
                            // This list entry does matter - we need to choose the right expansion method in case contouring has been chosen, but the
                            // polygon is not orthogonal.
                            geoCoreOrthogonalPoly.Add(shape.geoCoreShapeOrthogonal);
                        }
                        drawnPoly.Add(drawn);
                    }
                }
                catch (Exception)
                {
                }

                // Overlay
                if (!previewMode)
                {
                    int pCount = previewPoints.Count;
                    for (int poly = 0; poly < pCount; poly++)
                    {
                        if (!drawnPoly[poly])
                        {
                            int ptCount = previewPoints[poly].Count();
                            Parallel.For(0, ptCount, (pt) => 
                            // for (int pt = 0; pt < ptCount; pt++)
                            {
                                previewPoints[poly][pt].X += xOverlayVal;
                                previewPoints[poly][pt].Y += yOverlayVal;
                            });
                        }
                    }
                }

                // Biasing and CDU thanks to clipperLib
                // Note that we have to guard against a number of situations here
                // We do not want to re-bias contoured geoCore data - it's been done already.
                // Additionally, we don't want to assume an overlap for processing where none exists : we'll get back an empty polygon.
                double globalBias_Sides = Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.sBias));
                globalBias_Sides += (chaosSettings.getValue(ChaosSettings.properties.CDUSVar, settingsIndex) * Convert.ToDouble(entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.sCDU)) / 2);
                List<GeoLibPointF[]> resizedLayoutData = new List<GeoLibPointF[]>();
                try
                {
                    if (globalBias_Sides > Double.Epsilon)
                    {
                        List<bool> new_Drawn = new List<bool>();

                        int pCount = previewPoints.Count;
                        for (int poly = 0; poly < pCount; poly++)
                        {
                            // Need to iterate across all polygons and only bias in this manner either:
                            // non-contoured mode
                            // contoured, but non-orthogonal polygons.
                            if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0) ||
                                ((!geoCoreOrthogonalPoly[poly]) && (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1)))
                            {
                                Paths resizedPolyData = new Paths();
                                Path gdsPointData = GeoWrangler.pathFromPointF(previewPoints[poly], CentralProperties.scaleFactorForOperation);
                                ClipperOffset co = new ClipperOffset();
                                co.AddPath(gdsPointData, JoinType.jtMiter, EndType.etClosedPolygon);
                                co.Execute(ref resizedPolyData, Convert.ToDouble(globalBias_Sides * CentralProperties.scaleFactorForOperation));

                                // Store our polygon data (note that we could have ended up with two or more polygons due to reduction)
                                try
                                {
                                    for (int rPoly = 0; rPoly < resizedPolyData.Count(); rPoly++)
                                    {
                                        GeoLibPointF[] rPolyData = GeoWrangler.pointFFromPath(resizedPolyData[rPoly], CentralProperties.scaleFactorForOperation);
                                        resizedLayoutData.Add(rPolyData);

                                        // We need to track our drawn state as we could have a polygon count change.
                                        new_Drawn.Add(drawnPoly[poly]);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            else
                            {
                                new_Drawn.Add(drawnPoly[poly]);
                            }

                            // In case of contoured mode, with orthogonal polygon, we need to store this:
                            if (geoCoreOrthogonalPoly[poly] && (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 1))
                            {
                                // Decouple out of paranoia.
                                resizedLayoutData.Add(previewPoints[poly].ToArray());
                            }
                        }

                        previewPoints = resizedLayoutData.ToList();
                        drawnPoly = new_Drawn.ToList();
                    }
                }
                catch (Exception)
                {

                }

                if (process_overlaps)
                {
                    processOverlaps(commonVars, settingsIndex, previewPoints, forceOverride: false, (PolyFillType)commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.fill));
                }
            }
            else
            {
                // Drawn polygons only.
                // Needed to take this approach, otherwise fileData gets tied to the previewPoints list and things go wrong quickly.
                // .ToList() was insufficient to avoid the link.
                for (Int32 poly = 0; poly < entropyLayerSettings.getFileData().Count(); poly++)
                {
                    int arraySize = entropyLayerSettings.getFileData()[poly].Length;
                    GeoLibPointF[] tmp = new GeoLibPointF[arraySize];
                    Parallel.For(0, arraySize, (pt) => 
                    // for (Int32 pt = 0; pt < arraySize; pt++)
                    {
                        tmp[pt] = new GeoLibPointF(entropyLayerSettings.getFileData()[poly][pt].X + xOffset,
                                               entropyLayerSettings.getFileData()[poly][pt].Y + yOffset);
                    });
                    previewPoints.Add(tmp);
                    bool drawn = true;
                    drawnPoly.Add(drawn);
                }
            }
        }

        void init_boolean(CommonVars commonVars, ChaosSettings chaosSettings, Int32 settingsIndex, Int32 subShapeIndex, Int32 mode, bool doPASearch, bool previewMode, Int32 currentRow, Int32 currentCol, EntropyLayerSettings entropyLayerSettings)
        {
            // Get our two layers' geometry. Avoid keyholes in the process.
            int layerAIndex = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerA);
            if ((settingsIndex == layerAIndex) || (layerAIndex < 0))
            {
                return;
            }
            EntropyLayerSettings layerA = commonVars.getLayerSettings(layerAIndex);
            PreviewShape a_pShape = new PreviewShape(commonVars, layerAIndex, layerA.getInt(EntropyLayerSettings.properties_i.subShapeIndex), mode: 1, doPASearch, previewMode, currentRow, currentCol);

            int layerBIndex = entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerB);
            if ((settingsIndex == layerBIndex) || (layerBIndex < 0))
            {
                return;
            }
            EntropyLayerSettings layerB = commonVars.getLayerSettings(layerBIndex);
            PreviewShape b_pShape = new PreviewShape(commonVars, layerBIndex, layerB.getInt(EntropyLayerSettings.properties_i.subShapeIndex), mode: 1, doPASearch, previewMode, currentRow, currentCol);

            // We need to map the geometry into Paths for use in the Boolean
            Paths layerAPaths = GeoWrangler.pathsFromPointFs(a_pShape.getPoints(), CentralProperties.scaleFactorForOperation);
            Paths layerBPaths = GeoWrangler.pathsFromPointFs(b_pShape.getPoints(), CentralProperties.scaleFactorForOperation);

            // Now this gets interesting. We leverage the Boolean engine in ChaosEngine to get the result we want.
            // This should probably be relocated at some point, but for now, it's an odd interaction.
            Paths booleanPaths = ChaosEngine.customBoolean(
                firstLayerOperator: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerOpA),
                firstLayer: layerAPaths, 
                secondLayerOperator: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerOpB), 
                secondLayer: layerBPaths, 
                booleanFlag: entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerOpAB)
            );

            // This is set later, if needed, to force an early return from the overlap processing path.
            bool forceOverride = false;
            int bpCount = booleanPaths.Count;
            Parallel.For(0, bpCount, (i) => 
            // for (int i = 0; i < bpCount; i++)
            {
                try
                {
                    booleanPaths[i] = GeoWrangler.close(booleanPaths[i]);
                }
                catch (Exception)
                {

                }
            });

            // Scale back down again.
            List<GeoLibPointF[]> booleanGeo = GeoWrangler.pointFsFromPaths(booleanPaths, CentralProperties.scaleFactorForOperation);

            // Process the geometry according to mode, etc.
            // We do this by treating our geometry as a geocore source and calling init with this to set up our instance properties.
            // Feels a little hacky, but it ought to work.
            EntropyLayerSettings tempSettings = new EntropyLayerSettings();
            tempSettings.adjustSettings(entropyLayerSettings, gdsOnly: false);
            tempSettings.setInt(EntropyLayerSettings.properties_i.shapeIndex, (int)CommonVars.shapeNames.GEOCORE);
            tempSettings.setInt(EntropyLayerSettings.properties_i.gCSEngine, 1);
            tempSettings.setFileData(booleanGeo.ToList());
            drawnPoly.Clear();
            previewPoints.Clear();
            init(commonVars, chaosSettings, settingsIndex, subShapeIndex, mode, doPASearch, previewMode, currentRow, currentCol, tempSettings, doClockwiseGeoFix: true, process_overlaps: false); // Avoid the baked-in point order reprocessing which breaks our representation.

            processOverlaps(commonVars, settingsIndex, previewPoints, forceOverride, (PolyFillType)commonVars.getLayerSettings(settingsIndex).getInt(EntropyLayerSettings.properties_i.fill));
        }

        void processOverlaps(CommonVars commonVars, Int32 settingsIndex, List<GeoLibPointF[]> sourceData, bool forceOverride = false, PolyFillType pft = PolyFillType.pftNonZero)
        {
            // Filter drawn, process those, then do not-drawn. This allows for element counts to change.
            List<GeoLibPointF[]> drawnStuff = new List<GeoLibPointF[]>();
            List<GeoLibPointF[]> notDrawnStuff = new List<GeoLibPointF[]>();
            int sCount = sourceData.Count;
            for (int i = 0; i < sCount; i++)
            {
                if (drawnPoly[i])
                {
                    drawnStuff.Add(sourceData[i]);
                }
                else
                {
                    notDrawnStuff.Add(sourceData[i]);
                }
            }
            List<GeoLibPointF[]> processed_Drawn = processOverlaps_core(commonVars, drawnStuff, forceOverride, pft);

            List<GeoLibPointF[]> processed_NotDrawn = processOverlaps_core(commonVars, notDrawnStuff, forceOverride, pft);

            previewPoints.Clear();
            drawnPoly.Clear();

            int pdCount = processed_Drawn.Count;
            int pndCount = processed_NotDrawn.Count;

            for (int i = 0; i < pdCount; i++)
            {
                previewPoints.Add(processed_Drawn[i]);
                drawnPoly.Add(true);
            }

            for (int i = 0; i < pndCount; i++)
            {
                previewPoints.Add(processed_NotDrawn[i]);
                drawnPoly.Add(false);
            }

        }

        List<GeoLibPointF[]> processOverlaps_core(CommonVars commonVars, List<GeoLibPointF[]> sourceData, bool forceOverride = false, PolyFillType pft = PolyFillType.pftNonZero)
        {
            try
            {
                // Can the tessellator help us out here?
                // return GeoWrangler.makeKeyHole(sourceData);

                Clipper c = new Clipper();
                c.PreserveCollinear = true;
                Paths sourcePolyData = GeoWrangler.pathsFromPointFs(sourceData, CentralProperties.scaleFactorForOperation);
                Paths resizedPolyData = new Paths();

                // Union isn't always robust, so get a bounding box and run an intersection boolean to rationalize the geometry.
                IntRect bounds = Clipper.GetBounds(sourcePolyData);
                Path bounding = new Path();
                bounding.Add(new IntPoint(bounds.left, bounds.bottom));
                bounding.Add(new IntPoint(bounds.left, bounds.top));
                bounding.Add(new IntPoint(bounds.right, bounds.top));
                bounding.Add(new IntPoint(bounds.right, bounds.bottom));

                // c.AddPath(bounding, PolyType.ptClip, true);
                c.AddPaths(sourcePolyData, PolyType.ptClip, true);
                c.AddPaths(sourcePolyData, PolyType.ptSubject, true);

                c.Execute(ClipType.ctIntersection, resizedPolyData, pft, pft);

                // Avoid the overlap handling if we don't actually need to do it.

                bool returnEarly = false;

                int rpdCount = resizedPolyData.Count;
                int sCount = sourceData.Count;

                if (rpdCount == sCount)
                {
                    returnEarly = true;
                    for (int i = 0; i < rpdCount; i++)
                    {
                        // Clipper drops the closing vertex.
                        if (resizedPolyData[i].Count != sourceData[i].Length - 1)
                        {
                            returnEarly = false;
                            break;
                        }
                    }
                }

                if (returnEarly)
                {
                    // Secondary check
                    c.Clear();

                    c.AddPath(bounding, PolyType.ptClip, true);
                    // c.AddPaths(sourcePolyData, PolyType.ptClip, true);
                    c.AddPaths(sourcePolyData, PolyType.ptSubject, true);

                    c.Execute(ClipType.ctIntersection, resizedPolyData, pft, pft);

                    // Decompose to outers and cutters
                    Paths[] decomp = GeoWrangler.getDecomposed(resizedPolyData);

                    Paths outers = decomp[(int)GeoWrangler.type.outer];
                    Paths cutters = decomp[(int)GeoWrangler.type.cutter];

                    int oCount = outers.Count;
                    int cCount = cutters.Count;

                    // Is any cutter fully enclosed wtihin an outer?
                    for (int outer = 0; outer < oCount; outer++)
                    {
                        double origArea = Math.Abs(Clipper.Area(outers[outer]));
                        for (int cutter = 0; cutter < cCount; cutter++)
                        {
                            c.Clear();
                            c.AddPath(outers[outer], PolyType.ptSubject, true);
                            c.AddPath(cutters[cutter], PolyType.ptClip, true);
                            Paths test = new Paths();
                            c.Execute(ClipType.ctUnion, test, PolyFillType.pftPositive, PolyFillType.pftPositive);

                            double uArea = 0;
                            for (int i = 0; i < test.Count; i++)
                            {
                                uArea += Math.Abs(Clipper.Area(test[i]));
                            }

                            if (Math.Abs(uArea - origArea) < double.Epsilon)
                            {
                                returnEarly = false;
                                break;
                            }
                        }
                        if (!returnEarly)
                        {
                            break;
                        }
                    }

                    if (returnEarly)
                    {
                        return sourceData.ToList();
                    }
                }

                resizedPolyData = GeoWrangler.close(resizedPolyData);

                // Here, we can run into trouble....we might have a set of polygons which need to get keyholed. For example, where we have fully enclosed 'cutters' within an outer boundary.
                // Can geoWrangler help us out here?
                resizedPolyData = GeoWrangler.makeKeyHole(resizedPolyData).ToList();
                //resizedPolyData = GeoWrangler.sanitize(resizedPolyData).ToList();

                if (resizedPolyData.Count() == 0)
                {
                    return sourceData;
                }

                // We got some resulting geometry from our Boolean so let's process it to send back to the caller.
                List<GeoLibPointF[]> refinedData = new List<GeoLibPointF[]>();

                Fragmenter f = new Fragmenter(commonVars.getSimulationSettings().getResolution(), CentralProperties.scaleFactorForOperation);

                resizedPolyData = GeoWrangler.close(resizedPolyData);

                rpdCount = resizedPolyData.Count;

                for (int rPoly = 0; rPoly < rpdCount; rPoly++)
                {
                    // We have to refragment as the overlap processing changed the geometry heavily.
                    refinedData.Add(f.fragmentPath(GeoWrangler.pointFFromPath(resizedPolyData[rPoly], CentralProperties.scaleFactorForOperation)));
                }

                return refinedData.ToList();
            }
            catch (Exception)
            {
                return sourceData.ToList();
            }
        }
    }
}
