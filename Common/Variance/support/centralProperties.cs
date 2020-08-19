namespace Variance
{
    public static class CentralProperties
    {
        public const string productName = "Variance";
        public const string version = "4.2.7";
        public const int maxLayersForMC = 16; // maximum number of supported layers in MC system
        public const int scaleFactorForOperation = 10000;
        public enum typeShapes { none, rectangle, L, T, X, U, S, GEOCORE, BOOLEAN };

        public const int timer_interval = 1000;
    }
}
