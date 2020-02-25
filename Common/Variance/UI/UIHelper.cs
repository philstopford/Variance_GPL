﻿using color;
using Eto.Drawing;
using geoLib;
using System.Threading.Tasks;

namespace Variance
{
    public static class UIHelper
    {
        public static Color myColorToColor(MyColor sourceColor)
        {
            Color returnColor;
            returnColor = Color.FromArgb(sourceColor.R, sourceColor.G, sourceColor.B);
            return returnColor;
        }

        public static MyColor colorToMyColor(Color sourceColor)
        {
            return new MyColor(sourceColor.R, sourceColor.G, sourceColor.B);
        }

        public static PointF myPointFToPointF(GeoLibPointF sourcePoint)
        {
            return new PointF((float)sourcePoint.X, (float)sourcePoint.Y);
        }

        public static GeoLibPointF pointFTomyPointF(PointF sourcePoint)
        {
            return new GeoLibPointF(sourcePoint.X, sourcePoint.Y);
        }

        public static PointF[] myPointFArrayToPointFArray(GeoLibPointF[] sourceArray)
        {
            int length = sourceArray.Length;
            PointF[] returnArray = new PointF[length];
            Parallel.For(0, length, (i) =>
            // for (int i = 0; i < length; i++)
            {
                returnArray[i] = myPointFToPointF(sourceArray[i]);
            });

            return returnArray;
        }

        public static GeoLibPointF[] pointFArrayTomyPointFArray(PointF[] sourceArray)
        {
            int length = sourceArray.Length;
            GeoLibPointF[] returnArray = new GeoLibPointF[length];
            Parallel.For(0, length, (i) =>
            // for (int i = 0; i < length; i++)
            {
                returnArray[i] = pointFTomyPointF(sourceArray[i]);
            });

            return returnArray;
        }
    }
}