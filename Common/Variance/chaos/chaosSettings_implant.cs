using entropyRNG;

namespace Variance;

internal class ChaosSettings_implant
{
    public enum Properties { resistCDVar, resistHeightVar, resistTopCRRVar, tiltVar, twistVar }

    private static int dimensions = 5; // number of variations.

    public static int getDimensions()
    {
        return pGetDimensions();
    }

    private static int pGetDimensions()
    {
        return dimensions;
    }

    public double getValue(Properties p)
    {
        return pGetValue((int)p);
    }

    private double pGetValue(int p)
    {
        double retVal = 0;
        switch (p)
        {
            case (int)Properties.resistCDVar:
                retVal = implant_resistCDVar;
                break;
            case (int)Properties.resistHeightVar:
                retVal = implant_resistHeightVar;
                break;
            case (int)Properties.resistTopCRRVar:
                retVal = implant_resistTopCRRVar;
                break;
            case (int)Properties.tiltVar:
                retVal = implant_tiltVar;
                break;
            case (int)Properties.twistVar:
                retVal = implant_twistVar;
                break;
        }

        return retVal;
    }

    private double implant_resistCDVar;
    private double implant_resistHeightVar;
    private double implant_resistTopCRRVar;
    private double implant_tiltVar;
    private double implant_twistVar;
    private bool previewMode;

    public bool isPreview()
    {
        return pIsPreview();
    }

    private bool pIsPreview()
    {
        return previewMode;
    }

    public ChaosSettings_implant(bool previewMode_, EntropySettings entropySettings)
    {
        pChaosSettings_implant(previewMode_, entropySettings);
    }

    private void pChaosSettings_implant(bool previewMode_, EntropySettings entropySettings)
    {
        previewMode = previewMode_;
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
                case (int)commonRNG.rngIndex.mtwister:
                    implant_resistCDVar = MersenneTwister_RNG.random_gauss3()[0];
                    implant_resistHeightVar = MersenneTwister_RNG.random_gauss3()[0];
                    implant_resistTopCRRVar = MersenneTwister_RNG.random_gauss3()[0];
                    implant_tiltVar = MersenneTwister_RNG.random_gauss3()[0];
                    implant_twistVar = MersenneTwister_RNG.random_gauss3()[0];
                    break;
                case (int)commonRNG.rngIndex.crypto:
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