using color;
using Eto.Drawing;
using System.Threading.Tasks;
using Clipper2Lib;

namespace Variance;

public static class UIHelper
{
    public static Color myColorToColor(MyColor sourceColor)
    {
        var returnColor = Color.FromArgb(sourceColor.R, sourceColor.G, sourceColor.B);
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
        Parallel.For(0, length, i =>
            {
                returnArray[i] = myPointFToPointF(sourceArray[i]);
            }
        );
        return returnArray;
    }

    public static PathD pointFArrayTomyPointFArray(PointF[] sourceArray)
    {
        int length = sourceArray.Length;
        PathD returnArray = Helper.initedPathD(length);
        Parallel.For(0, length, i =>
            {
                returnArray[i] = pointFTomyPointF(sourceArray[i]);
            }
        );
        return returnArray;
    }
}