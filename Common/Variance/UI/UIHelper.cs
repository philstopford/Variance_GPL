using color;
using Eto.Drawing;
using System.Threading.Tasks;
using Clipper2Lib;

namespace Variance;

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

    public static PointF myPointFToPointF(PointD sourcePoint)
    {
        return new PointF((float)sourcePoint.x, (float)sourcePoint.y);
    }

    public static PointD pointFTomyPointF(PointF sourcePoint)
    {
        return new PointD(sourcePoint.X, sourcePoint.Y);
    }

    public static PointF[] myPointFArrayToPointFArray(PathD sourceArray)
    {
        int length = sourceArray.Count;
        PointF[] returnArray = new PointF[length];
#if !VARIANCESINGLETHREADED
        Parallel.For(0, length, i =>
#else
            for (int i = 0; i < length; i++)
#endif
            {
                returnArray[i] = myPointFToPointF(sourceArray[i]);
            }
#if !VARIANCESINGLETHREADED
        );
#endif
        return returnArray;
    }

    public static PathD pointFArrayTomyPointFArray(PointF[] sourceArray)
    {
        int length = sourceArray.Length;
        PathD returnArray = Helper.initedPathD(length);
#if !VARIANCESINGLETHREADED
        Parallel.For(0, length, i =>
#else
            for (int i = 0; i < length; i++)
#endif
            {
                returnArray[i] = pointFTomyPointF(sourceArray[i]);
            }
#if !VARIANCESINGLETHREADED
        );
#endif
        return returnArray;
    }
}