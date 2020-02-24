using entropyRNG;

namespace Variance
{
    class ChaosSettings_implant
    {
        public enum properties { resistCDVar, resistHeightVar, resistTopCRRVar, tiltVar, twistVar }

        static int dimensions = 5; // number of variations.

        public static int getDimensions()
        {
            return pGetDimensions();
        }

        static int pGetDimensions()
        {
            return dimensions;
        }

        public double getValue(properties p)
        {
            return pGetValue((int)p);
        }

        double pGetValue(int p)
        {
            double retVal = 0;
            switch (p)
            {
                case (int)properties.resistCDVar:
                    retVal = implant_resistCDVar;
                    break;
                case (int)properties.resistHeightVar:
                    retVal = implant_resistHeightVar;
                    break;
                case (int)properties.resistTopCRRVar:
                    retVal = implant_resistTopCRRVar;
                    break;
                case (int)properties.tiltVar:
                    retVal = implant_tiltVar;
                    break;
                case (int)properties.twistVar:
                    retVal = implant_twistVar;
                    break;
            }

            return retVal;
        }

        double implant_resistCDVar;
        double implant_resistHeightVar;
        double implant_resistTopCRRVar;
        double implant_tiltVar;
        double implant_twistVar;
        bool previewMode;

        public bool isPreview()
        {
            return pIsPreview();
        }

        bool pIsPreview()
        {
            return previewMode;
        }

        public ChaosSettings_implant(bool previewMode, EntropySettings entropySettings)
        {
            pChaosSettings_implant(previewMode, entropySettings);
        }

        void pChaosSettings_implant(bool previewMode, EntropySettings entropySettings)
        {
            this.previewMode = previewMode;
            if (previewMode)
            {
                implant_resistCDVar = 0;
                implant_resistHeightVar = 0;
                implant_resistTopCRRVar = 0;
                implant_tiltVar = 0;
                implant_twistVar = 0;
            }
            else
            {
                switch (entropySettings.getValue(EntropySettings.properties_i.rngType))
                {
                    case ((int)commonRNG.rngIndex.mtwister):
                        implant_resistCDVar = MersenneTwister_RNG.random_gauss3()[0];
                        implant_resistHeightVar = MersenneTwister_RNG.random_gauss3()[0];
                        implant_resistTopCRRVar = MersenneTwister_RNG.random_gauss3()[0];
                        implant_tiltVar = MersenneTwister_RNG.random_gauss3()[0];
                        implant_twistVar = MersenneTwister_RNG.random_gauss3()[0];
                        break;
                    case ((int)commonRNG.rngIndex.crypto):
                        implant_resistCDVar = Crypto_RNG.random_gauss3()[0];
                        implant_resistHeightVar = Crypto_RNG.random_gauss3()[0];
                        implant_resistTopCRRVar = Crypto_RNG.random_gauss3()[0];
                        implant_tiltVar = Crypto_RNG.random_gauss3()[0];
                        implant_twistVar = Crypto_RNG.random_gauss3()[0];
                        break;
                    default:
                        implant_resistCDVar = RNG.random_gauss3()[0];
                        implant_resistHeightVar = RNG.random_gauss3()[0];
                        implant_resistTopCRRVar = RNG.random_gauss3()[0];
                        implant_tiltVar = RNG.random_gauss3()[0];
                        implant_twistVar = RNG.random_gauss3()[0];
                        break;
                }
            }
        }
    }
}
