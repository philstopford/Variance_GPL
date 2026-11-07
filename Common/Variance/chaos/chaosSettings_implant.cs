using System;
using entropyRNG;

namespace Variance;

internal class ChaosSettings_implant
{
    public enum Properties { resistCDVar, resistHeightVar, resistTopCRRVar, tiltVar, twistVar }

    private const int dimensions = 5; // number of variations.

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
        double retVal = p switch
        {
            (int)Properties.resistCDVar => implant_resistCDVar,
            (int)Properties.resistHeightVar => implant_resistHeightVar,
            (int)Properties.resistTopCRRVar => implant_resistTopCRRVar,
            (int)Properties.tiltVar => implant_tiltVar,
            (int)Properties.twistVar => implant_twistVar,
            _ => 0
        };

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

    /// <summary>
    /// Constructor for Latin Hypercube Sampling that uses pre-computed stratified values
    /// instead of random generation for better space-filling properties.
    /// </summary>
    /// <param name="previewMode_">Preview mode flag</param>
    /// <param name="entropySettings">Entropy settings (unused for LHS but kept for compatibility)</param>
    /// <param name="lhsValues">Pre-computed LHS values for each dimension</param>
    public ChaosSettings_implant(bool previewMode_, EntropySettings entropySettings, double[] lhsValues)
    {
        pChaosSettings_implant_LHS(previewMode_, lhsValues);
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

    /// <summary>
    /// Initialize ChaosSettings_implant with pre-computed LHS values for proper stratified sampling.
    /// </summary>
    /// <param name="previewMode_">Preview mode flag</param>
    /// <param name="lhsValues">Array of 5 pre-computed Gaussian LHS values for each dimension</param>
    private void pChaosSettings_implant_LHS(bool previewMode_, double[] lhsValues)
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
            if (lhsValues == null || lhsValues.Length != dimensions)
            {
                throw new ArgumentException($"LHS values array must contain exactly {dimensions} values");
            }

            // Use the pre-computed LHS values directly - they are already Gaussian distributed
            implant_resistCDVar = lhsValues[(int)Properties.resistCDVar];
            implant_resistHeightVar = lhsValues[(int)Properties.resistHeightVar];
            implant_resistTopCRRVar = lhsValues[(int)Properties.resistTopCRRVar];
            implant_tiltVar = lhsValues[(int)Properties.tiltVar];
            implant_twistVar = lhsValues[(int)Properties.twistVar];
        }
    }
}