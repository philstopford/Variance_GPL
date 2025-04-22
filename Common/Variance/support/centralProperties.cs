using shapeEngine;

namespace Variance;

public static class CentralProperties
{
    public const string productName = "Variance";
    public const string version = "6.1";
    public const int maxLayersForMC = 16; // maximum number of supported layers in MC system

    public enum shapeNames
    {
        none,
        rect,
        Lshape,
        Tshape,
        Xshape,
        Ushape,
        Sshape,
        GEOCORE,
        BOOLEAN,
    }

    public static int[] shapeTable = new[] {
        (int)ShapeLibrary.shapeNames_all.none,
        (int)ShapeLibrary.shapeNames_all.rect,
        (int)ShapeLibrary.shapeNames_all.Lshape,
        (int)ShapeLibrary.shapeNames_all.Tshape,
        (int)ShapeLibrary.shapeNames_all.Xshape,
        (int)ShapeLibrary.shapeNames_all.Ushape,
        (int)ShapeLibrary.shapeNames_all.Sshape,
        (int)ShapeLibrary.shapeNames_all.GEOCORE,
        (int)ShapeLibrary.shapeNames_all.BOOLEAN
    };

    public const int timer_interval = 1000;
}