using System.Collections.Generic;
using System.Threading.Tasks;

namespace Variance;

public class ChaosSettings
{
    public enum Properties { overlayX, overlayY, hTipBiasType, vTipBiasType, hTipBiasVar, vTipBiasVar, icVar, ocVar, LWRVar, LWR2Var, CDUTVar, CDUSVar, wobbleVar }
    public enum Ints { lwrSeed, lwr2Seed }
    public enum Bools { icPA, ocPA }

    private const int dimensions = 15; // number of variations.

    public static int getDimensions()
    {
        return pGetDimensions();
    }

    private static int pGetDimensions()
    {
        return dimensions;
    }

    // This holds all of our run specific variations. It should be set up before EntropyShape is called.
    private double[] overlayX;
    private double[] overlayY;
    private double[] hTipBiasType;
    private double[] vTipBiasType;
    private double[] horTipBiasVar;
    private double[] verTipBiasVar;
    private double[] iCVar;
    private bool[] iC_PAsearch;
    private double[] oCVar;
    private bool[] oC_PAsearch;
    private double[] LWRVar;
    private double[] LWR2Var;
    private double[] CDUTVar;
    private double[] CDUSVar;
    private double[] wobbleVar;
    private int[] LWRSeed;
    private int[] LWR2Seed;
    private bool previewMode;
    private bool customRNGMapping;

    public bool getPreviewMode()
    {
        return pGetPreviewMode();
    }

    private bool pGetPreviewMode()
    {
        return previewMode;
    }

    public void setPreviewMode(bool p)
    {
        pSetPreviewMode(p);
    }

    private void pSetPreviewMode(bool p)
    {
        previewMode = p;
    }

    public bool getCustomRNGMapping()
    {
        return pGetCustomRNGMapping();
    }

    private bool pGetCustomRNGMapping()
    {
        return customRNGMapping;
    }

    public void setCustomRNGMapping(bool p)
    {
        pSetCustomRNGMapping(p);
    }

    private void pSetCustomRNGMapping(bool p)
    {
        customRNGMapping = p;
    }

    public void setValue(Properties p, int index, double value)
    {
        pSetValue((int)p, index, value);
    }

    private void pSetValue(int p, int index, double value)
    {
        switch (p)
        {
            case (int)Properties.overlayX:
                overlayX[index] = value;
                break;
            case (int)Properties.overlayY:
                overlayY[index] = value;
                break;
            case (int)Properties.hTipBiasType:
                hTipBiasType[index] = value;
                break;
            case (int)Properties.vTipBiasType:
                vTipBiasType[index] = value;
                break;
            case (int)Properties.hTipBiasVar:
                horTipBiasVar[index] = value;
                break;
            case (int)Properties.vTipBiasVar:
                verTipBiasVar[index] = value;
                break;
            case (int)Properties.icVar:
                iCVar[index] = value;
                break;
            case (int)Properties.ocVar:
                oCVar[index] = value;
                break;
            case (int)Properties.LWRVar:
                LWRVar[index] = value;
                break;
            case (int)Properties.LWR2Var:
                LWR2Var[index] = value;
                break;
            case (int)Properties.CDUTVar:
                CDUTVar[index] = value;
                break;
            case (int)Properties.CDUSVar:
                CDUSVar[index] = value;
                break;
            case (int)Properties.wobbleVar:
                wobbleVar[index] = value;
                break;
        }
    }

    public double getValue(Properties p, int index)
    {
        return pGetValue((int)p, index);
    }

    private double pGetValue(int p, int index)
    {
        double retVal = 0;
        switch (p)
        {
            case (int)Properties.overlayX:
                retVal = overlayX[index];
                break;
            case (int)Properties.overlayY:
                retVal = overlayY[index];
                break;
            case (int)Properties.hTipBiasType:
                retVal = hTipBiasType[index];
                break;
            case (int)Properties.vTipBiasType:
                retVal = vTipBiasType[index];
                break;
            case (int)Properties.hTipBiasVar:
                retVal = horTipBiasVar[index];
                break;
            case (int)Properties.vTipBiasVar:
                retVal = verTipBiasVar[index];
                break;
            case (int)Properties.icVar:
                retVal = iCVar[index];
                break;
            case (int)Properties.ocVar:
                retVal = oCVar[index];
                break;
            case (int)Properties.LWRVar:
                retVal = LWRVar[index];
                break;
            case (int)Properties.LWR2Var:
                retVal = LWR2Var[index];
                break;
            case (int)Properties.CDUTVar:
                retVal = CDUTVar[index];
                break;
            case (int)Properties.CDUSVar:
                retVal = CDUSVar[index];
                break;
            case (int)Properties.wobbleVar:
                retVal = wobbleVar[index];
                break;
        }

        return retVal;
    }

    public double[] getValues(Properties p)
    {
        return pGetValues((int)p);
    }

    private double[] pGetValues(int p)
    {
        double[] retVal = { };
        switch (p)
        {
            case (int)Properties.overlayX:
                retVal = overlayX;
                break;
            case (int)Properties.overlayY:
                retVal = overlayY;
                break;
            case (int)Properties.hTipBiasType:
                retVal = hTipBiasType;
                break;
            case (int)Properties.vTipBiasType:
                retVal = vTipBiasType;
                break;
            case (int)Properties.hTipBiasVar:
                retVal = horTipBiasVar;
                break;
            case (int)Properties.vTipBiasVar:
                retVal = verTipBiasVar;
                break;
            case (int)Properties.icVar:
                retVal = iCVar;
                break;
            case (int)Properties.ocVar:
                retVal = oCVar;
                break;
            case (int)Properties.LWRVar:
                retVal = LWRVar;
                break;
            case (int)Properties.LWR2Var:
                retVal = LWR2Var;
                break;
            case (int)Properties.CDUTVar:
                retVal = CDUTVar;
                break;
            case (int)Properties.CDUSVar:
                retVal = CDUSVar;
                break;
            case (int)Properties.wobbleVar:
                retVal = wobbleVar;
                break;
        }

        return retVal;
    }

    public void setValues(Properties p, double[] values)
    {
        pSetValues((int)p, values);
    }

    private void pSetValues(int p, double[] values)
    {
        switch (p)
        {
            case (int)Properties.overlayX:
                overlayX = values;
                break;
            case (int)Properties.overlayY:
                overlayY = values;
                break;
            case (int)Properties.hTipBiasType:
                hTipBiasType = values;
                break;
            case (int)Properties.vTipBiasType:
                vTipBiasType = values;
                break;
            case (int)Properties.hTipBiasVar:
                horTipBiasVar = values;
                break;
            case (int)Properties.vTipBiasVar:
                verTipBiasVar = values;
                break;
            case (int)Properties.icVar:
                iCVar = values;
                break;
            case (int)Properties.ocVar:
                oCVar = values;
                break;
            case (int)Properties.LWRVar:
                LWRVar = values;
                break;
            case (int)Properties.LWR2Var:
                LWR2Var = values;
                break;
            case (int)Properties.CDUTVar:
                CDUTVar = values;
                break;
            case (int)Properties.CDUSVar:
                CDUSVar = values;
                break;
            case (int)Properties.wobbleVar:
                wobbleVar = values;
                break;
        }
    }

    public void setInt(Ints i, int index, int value)
    {
        pSetInt((int)i, index, value);
    }

    private void pSetInt(int i, int index, int value)
    {
        switch (i)
        {
            case (int)Ints.lwrSeed:
                LWRSeed[index] = value;
                break;
            case (int)Ints.lwr2Seed:
                LWR2Seed[index] = value;
                break;
        }
    }

    public int getInt(Ints i, int index)
    {
        return pGetInt((int)i, index);
    }

    private int pGetInt(int i, int index)
    {
        int retVal = 0;
        switch (i)
        {
            case (int)Ints.lwrSeed:
                retVal = LWRSeed[index];
                break;
            case (int)Ints.lwr2Seed:
                retVal = LWR2Seed[index];
                break;
        }
        return retVal;
    }

    public int[] getInts(Ints i)
    {
        return pGetInts((int)i);
    }

    private int[] pGetInts(int i)
    {
        int[] retVal = { };
        switch (i)
        {
            case (int)Ints.lwrSeed:
                retVal = LWRSeed;
                break;
            case (int)Ints.lwr2Seed:
                retVal = LWR2Seed;
                break;
        }
        return retVal;
    }

    public void setBool(Bools b, int index, bool value)
    {
        pSetBool((int)b, index, value);
    }

    private void pSetBool(int b, int index, bool value)
    {
        switch (b)
        {
            case (int)Bools.icPA:
                iC_PAsearch[index] = value;
                break;
            case (int)Bools.ocPA:
                oC_PAsearch[index] = value;
                break;
        }
    }

    public bool getBool(Bools b, int index)
    {
        return pGetBool((int)b, index);
    }

    private bool pGetBool(int b, int index)
    {
        bool retVal = false;
        switch (b)
        {
            case (int)Bools.icPA:
                retVal = iC_PAsearch[index];
                break;
            case (int)Bools.ocPA:
                retVal = oC_PAsearch[index];
                break;
        }
        return retVal;
    }

    public bool[] getBools(Bools b)
    {
        return pGetBools((int)b);
    }

    private bool[] pGetBools(int b)
    {
        bool[] retVal = { };
        switch (b)
        {
            case (int)Bools.icPA:
                retVal = iC_PAsearch;
                break;
            case (int)Bools.ocPA:
                retVal = oC_PAsearch;
                break;
        }
        return retVal;
    }

    public ChaosSettings(bool previewMode_, List<EntropyLayerSettings> listOfSettings, EntropySettings simSettings) // set to true for preview; else we run with random inputs
    {
        pChaosSettings(previewMode_, listOfSettings, simSettings);
    }

    private void pChaosSettings(bool previewMode_, List<EntropyLayerSettings> listOfSettings, EntropySettings simSettings) // set to true for preview; else we run with random inputs
    {
        customRNGMapping = false;
        previewMode = previewMode_;
        int count = listOfSettings.Count;
        overlayX = new double[count];
        overlayY = new double[count];
        hTipBiasType = new double[count];
        vTipBiasType = new double[count];
        horTipBiasVar = new double[count];
        verTipBiasVar = new double[count];
        iCVar = new double[count];
        iC_PAsearch = new bool[count];
        oCVar = new double[count];
        oC_PAsearch = new bool[count];
        LWRVar = new double[count];
        LWR2Var = new double[count];
        CDUSVar = new double[count];
        CDUTVar = new double[count];
        wobbleVar = new double[count];
        LWRSeed = new int[count];
        LWR2Seed = new int[count];
#if !CHAOSSINGLETHREADED
        Parallel.For(0, count, i =>
#else
            for (int i = 0; i < count; i++)
#endif
            {
                iC_PAsearch[i] = false;
                oC_PAsearch[i] = false;
                LWRSeed[i] = 1;
                LWR2Seed[i] = 1;
                if (previewMode)
                {
                    overlayX[i] = 0;
                    overlayY[i] = 0;
                    hTipBiasType[i] = 0;
                    vTipBiasType[i] = 0;
                    horTipBiasVar[i] = 0;
                    verTipBiasVar[i] = 0;
                    iCVar[i] = 0;
                    oCVar[i] = 0;
                    LWRVar[i] = 0;
                    LWR2Var[i] = 0;
                    CDUSVar[i] = 0;
                    CDUTVar[i] = 0;
                    wobbleVar[i] = 0;
                }
                else
                {
                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.xOL_RNG)))
                    {
                        customRNGMapping = true;
                        overlayX[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.xOL_RNG), simSettings);
                    }
                    else
                    {
                        overlayX[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }
                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.yOL_RNG)))
                    {
                        customRNGMapping = true;
                        overlayY[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.yOL_RNG), simSettings);
                    }
                    else
                    {
                        overlayY[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }
                    hTipBiasType[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    vTipBiasType[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);

                    horTipBiasVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    // Need to pay attention to sign here, to pull the right custom equation.
                    if (hTipBiasType[i] < 0.5 && rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipNVar_RNG)))
                    {
                        customRNGMapping = true;
                        horTipBiasVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipNVar_RNG), simSettings);
                    }
                    if (hTipBiasType[i] >= 0.5 && rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipPVar_RNG)))
                    {
                        customRNGMapping = true;
                        horTipBiasVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipPVar_RNG), simSettings);
                    }

                    verTipBiasVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    // Need to pay attention to sign here, to pull the right custom equation.
                    if (vTipBiasType[i] < 0.5 && rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipNVar_RNG)))
                    {
                        customRNGMapping = true;
                        verTipBiasVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipNVar_RNG), simSettings);
                    }
                    if (vTipBiasType[i] >= 0.5 && listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipPVar_RNG) != "" && listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipPVar_RNG) != CommonVars.boxMuller)
                    {
                        verTipBiasVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipPVar_RNG), simSettings);
                    }

                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.iCV_RNG)))
                    {
                        customRNGMapping = true;
                        iCVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.iCV_RNG), simSettings);
                    }
                    else
                    {
                        iCVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }

                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.oCV_RNG)))
                    {
                        customRNGMapping = true;
                        oCVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.oCV_RNG), simSettings);
                    }
                    else
                    {
                        oCVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }

                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.lwr_RNG)))
                    {
                        customRNGMapping = true;
                        LWRVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.lwr_RNG), simSettings);
                    }
                    else
                    {
                        LWRVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }

                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.lwr2_RNG)))
                    {
                        customRNGMapping = true;
                        LWR2Var[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.lwr2_RNG), simSettings);
                    }
                    else
                    {
                        LWR2Var[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }

                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.sCDU_RNG)))
                    {
                        customRNGMapping = true;
                        CDUSVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.sCDU_RNG), simSettings);
                    }
                    else
                    {
                        CDUSVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }

                    // Note that the TVar will be matched to SVar if the simulation settings call for these to be linked.
                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.tCDU_RNG)))
                    {
                        customRNGMapping = true;
                        CDUTVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.tCDU_RNG), simSettings);
                    }
                    else
                    {
                        CDUTVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }

                    if (rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.wobble_RNG)))
                    {
                        customRNGMapping = true;
                        wobbleVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.wobble_RNG), simSettings);
                    }
                    else
                    {
                        wobbleVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    }

                    LWRSeed[i] = UtilityFuncs.getRandomInt(simSettings);
                    LWR2Seed[i] = UtilityFuncs.getRandomInt(simSettings);
                }
            }
#if !CHAOSSINGLETHREADED
        );
#endif
        // Re-walk to handle correlation. We can't do this in the loop above since the correlated layer's settings
        // are likely not available at that point in time.
#if !CHAOSSINGLETHREADED
        Parallel.For(0, count, i =>
#else
            for (int i = 0; i < count; i++)
#endif
            {
                if (listOfSettings[i].getInt(EntropyLayerSettings.properties_i.xOL_corr) == 1)
                {
                    overlayX[i] = overlayX[listOfSettings[i].getInt(EntropyLayerSettings.properties_i.xOL_corr_ref)];
                }
                if (listOfSettings[i].getInt(EntropyLayerSettings.properties_i.yOL_corr) == 1)
                {
                    overlayY[i] = overlayY[listOfSettings[i].getInt(EntropyLayerSettings.properties_i.yOL_corr_ref)];
                }
                if (listOfSettings[i].getInt(EntropyLayerSettings.properties_i.CDU_corr) == 1)
                {
                    CDUSVar[i] = CDUSVar[listOfSettings[i].getInt(EntropyLayerSettings.properties_i.CDU_corr_ref)];
                }
                if (listOfSettings[i].getInt(EntropyLayerSettings.properties_i.tCDU_corr) == 1)
                {
                    // Note that the TVar will be matched to SVar if the simulation settings call for these to be linked.
                    CDUTVar[i] = CDUTVar[listOfSettings[i].getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref)];
                }
                if (listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr_corr) == 1)
                {
                    LWRSeed[i] = LWRSeed[listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr_corr_ref)];
                }
                if (listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr2_corr) == 1)
                {
                    LWR2Seed[i] = LWR2Seed[listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr2_corr_ref)];
                }
            }
#if !CHAOSSINGLETHREADED
        );
#endif
    }

    public ChaosSettings(ChaosSettings source)
    {
        overlayX = source.overlayX;
        overlayY = source.overlayY;
        hTipBiasType = source.hTipBiasType;
        vTipBiasType = source.vTipBiasType;
        horTipBiasVar = source.horTipBiasVar;
        verTipBiasVar = source.verTipBiasVar;
        iCVar = source.iCVar;
        iC_PAsearch = source.iC_PAsearch;
        oCVar = source.oCVar;
        oC_PAsearch = source.oC_PAsearch;
        LWRVar = source.LWRVar;
        LWR2Var = source.LWR2Var;
        CDUTVar = source.CDUTVar;
        CDUSVar = source.CDUSVar;
        wobbleVar = source.wobbleVar;
        LWRSeed = source.LWRSeed;
        LWR2Seed = source.LWR2Seed;
        previewMode = source.previewMode;
        customRNGMapping = source.customRNGMapping;
    }

    private static bool rngCheck(string rngString)
    {
        return rngString != "" && rngString != CommonVars.boxMuller;
    }
}