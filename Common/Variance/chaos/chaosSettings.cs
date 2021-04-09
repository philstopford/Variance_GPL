using System.Collections.Generic;
using System.Threading.Tasks;

namespace Variance
{
    public class ChaosSettings
    {
        public enum properties { overlayX, overlayY, hTipBiasType, vTipBiasType, hTipBiasVar, vTipBiasVar, icVar, ocVar, LWRVar, LWR2Var, CDUTVar, CDUSVar, wobbleVar }
        public enum ints { lwrSeed, lwr2Seed }
        public enum bools { icPA, ocPA }

        static int dimensions = 15; // number of variations.

        public static int getDimensions()
        {
            return pGetDimensions();
        }

        static int pGetDimensions()
        {
            return dimensions;
        }

        // This holds all of our run specific variations. It should be set up before EntropyShape is called.
        double[] overlayX;
        double[] overlayY;
        double[] hTipBiasType;
        double[] vTipBiasType;
        double[] horTipBiasVar;
        double[] verTipBiasVar;
        double[] iCVar;
        bool[] iC_PAsearch;
        double[] oCVar;
        bool[] oC_PAsearch;
        double[] LWRVar;
        double[] LWR2Var;
        double[] CDUTVar;
        double[] CDUSVar;
        double[] wobbleVar;
        int[] LWRSeed;
        int[] LWR2Seed;
        bool previewMode;
        bool customRNGMapping;

        public bool getPreviewMode()
        {
            return pGetPreviewMode();
        }

        bool pGetPreviewMode()
        {
            return previewMode;
        }

        public void setPreviewMode(bool p)
        {
            pSetPreviewMode(p);
        }

        void pSetPreviewMode(bool p)
        {
            previewMode = p;
        }

        public bool getCustomRNGMapping()
        {
            return pGetCustomRNGMapping();
        }

        bool pGetCustomRNGMapping()
        {
            return customRNGMapping;
        }

        public void setCustomRNGMapping(bool p)
        {
            pSetCustomRNGMapping(p);
        }

        void pSetCustomRNGMapping(bool p)
        {
            customRNGMapping = p;
        }

        public void setValue(properties p, int index, double value)
        {
            pSetValue((int)p, index, value);
        }

        void pSetValue(int p, int index, double value)
        {
            switch (p)
            {
                case (int)properties.overlayX:
                    overlayX[index] = value;
                    break;
                case (int)properties.overlayY:
                    overlayY[index] = value;
                    break;
                case (int)properties.hTipBiasType:
                    hTipBiasType[index] = value;
                    break;
                case (int)properties.vTipBiasType:
                    vTipBiasType[index] = value;
                    break;
                case (int)properties.hTipBiasVar:
                    horTipBiasVar[index] = value;
                    break;
                case (int)properties.vTipBiasVar:
                    verTipBiasVar[index] = value;
                    break;
                case (int)properties.icVar:
                    iCVar[index] = value;
                    break;
                case (int)properties.ocVar:
                    oCVar[index] = value;
                    break;
                case (int)properties.LWRVar:
                    LWRVar[index] = value;
                    break;
                case (int)properties.LWR2Var:
                    LWR2Var[index] = value;
                    break;
                case (int)properties.CDUTVar:
                    CDUTVar[index] = value;
                    break;
                case (int)properties.CDUSVar:
                    CDUSVar[index] = value;
                    break;
                case (int)properties.wobbleVar:
                    wobbleVar[index] = value;
                    break;
            }
        }

        public double getValue(properties p, int index)
        {
            return pGetValue((int)p, index);
        }

        double pGetValue(int p, int index)
        {
            double retVal = 0;
            switch (p)
            {
                case (int)properties.overlayX:
                    retVal = overlayX[index];
                    break;
                case (int)properties.overlayY:
                    retVal = overlayY[index];
                    break;
                case (int)properties.hTipBiasType:
                    retVal = hTipBiasType[index];
                    break;
                case (int)properties.vTipBiasType:
                    retVal = vTipBiasType[index];
                    break;
                case (int)properties.hTipBiasVar:
                    retVal = horTipBiasVar[index];
                    break;
                case (int)properties.vTipBiasVar:
                    retVal = verTipBiasVar[index];
                    break;
                case (int)properties.icVar:
                    retVal = iCVar[index];
                    break;
                case (int)properties.ocVar:
                    retVal = oCVar[index];
                    break;
                case (int)properties.LWRVar:
                    retVal = LWRVar[index];
                    break;
                case (int)properties.LWR2Var:
                    retVal = LWR2Var[index];
                    break;
                case (int)properties.CDUTVar:
                    retVal = CDUTVar[index];
                    break;
                case (int)properties.CDUSVar:
                    retVal = CDUSVar[index];
                    break;
                case (int)properties.wobbleVar:
                    retVal = wobbleVar[index];
                    break;
            }

            return retVal;
        }

        public double[] getValues(properties p)
        {
            return pGetValues((int)p);
        }

        double[] pGetValues(int p)
        {
            double[] retVal = { };
            switch (p)
            {
                case (int)properties.overlayX:
                    retVal = overlayX;
                    break;
                case (int)properties.overlayY:
                    retVal = overlayY;
                    break;
                case (int)properties.hTipBiasType:
                    retVal = hTipBiasType;
                    break;
                case (int)properties.vTipBiasType:
                    retVal = vTipBiasType;
                    break;
                case (int)properties.hTipBiasVar:
                    retVal = horTipBiasVar;
                    break;
                case (int)properties.vTipBiasVar:
                    retVal = verTipBiasVar;
                    break;
                case (int)properties.icVar:
                    retVal = iCVar;
                    break;
                case (int)properties.ocVar:
                    retVal = oCVar;
                    break;
                case (int)properties.LWRVar:
                    retVal = LWRVar;
                    break;
                case (int)properties.LWR2Var:
                    retVal = LWR2Var;
                    break;
                case (int)properties.CDUTVar:
                    retVal = CDUTVar;
                    break;
                case (int)properties.CDUSVar:
                    retVal = CDUSVar;
                    break;
                case (int)properties.wobbleVar:
                    retVal = wobbleVar;
                    break;
            }

            return retVal;
        }

        public void setValues(properties p, double[] values)
        {
            pSetValues((int)p, values);
        }

        void pSetValues(int p, double[] values)
        {
            switch (p)
            {
                case (int)properties.overlayX:
                    overlayX = values;
                    break;
                case (int)properties.overlayY:
                    overlayY = values;
                    break;
                case (int)properties.hTipBiasType:
                    hTipBiasType = values;
                    break;
                case (int)properties.vTipBiasType:
                    vTipBiasType = values;
                    break;
                case (int)properties.hTipBiasVar:
                    horTipBiasVar = values;
                    break;
                case (int)properties.vTipBiasVar:
                    verTipBiasVar = values;
                    break;
                case (int)properties.icVar:
                    iCVar = values;
                    break;
                case (int)properties.ocVar:
                    oCVar = values;
                    break;
                case (int)properties.LWRVar:
                    LWRVar = values;
                    break;
                case (int)properties.LWR2Var:
                    LWR2Var = values;
                    break;
                case (int)properties.CDUTVar:
                    CDUTVar = values;
                    break;
                case (int)properties.CDUSVar:
                    CDUSVar = values;
                    break;
                case (int)properties.wobbleVar:
                    wobbleVar = values;
                    break;
            }
        }

        public void setInt(ints i, int index, int value)
        {
            pSetInt((int)i, index, value);
        }

        void pSetInt(int i, int index, int value)
        {
            switch (i)
            {
                case (int)ints.lwrSeed:
                    LWRSeed[index] = value;
                    break;
                case (int)ints.lwr2Seed:
                    LWR2Seed[index] = value;
                    break;
            }
        }

        public int getInt(ints i, int index)
        {
            return pGetInt((int)i, index);
        }

        int pGetInt(int i, int index)
        {
            int retVal = 0;
            switch (i)
            {
                case (int)ints.lwrSeed:
                    retVal = LWRSeed[index];
                    break;
                case (int)ints.lwr2Seed:
                    retVal = LWR2Seed[index];
                    break;
            }
            return retVal;
        }

        public int[] getInts(ints i)
        {
            return pGetInts((int)i);
        }

        int[] pGetInts(int i)
        {
            int[] retVal = { };
            switch (i)
            {
                case (int)ints.lwrSeed:
                    retVal = LWRSeed;
                    break;
                case (int)ints.lwr2Seed:
                    retVal = LWR2Seed;
                    break;
            }
            return retVal;
        }

        public void setBool(bools b, int index, bool value)
        {
            pSetBool((int)b, index, value);
        }

        void pSetBool(int b, int index, bool value)
        {
            switch (b)
            {
                case (int)bools.icPA:
                    iC_PAsearch[index] = value;
                    break;
                case (int)bools.ocPA:
                    oC_PAsearch[index] = value;
                    break;
            }
        }

        public bool getBool(bools b, int index)
        {
            return pGetBool((int)b, index);
        }

        bool pGetBool(int b, int index)
        {
            bool retVal = false;
            switch (b)
            {
                case (int)bools.icPA:
                    retVal = iC_PAsearch[index];
                    break;
                case (int)bools.ocPA:
                    retVal = oC_PAsearch[index];
                    break;
            }
            return retVal;
        }

        public bool[] getBools(bools b)
        {
            return pGetBools((int)b);
        }

        bool[] pGetBools(int b)
        {
            bool[] retVal = { };
            switch (b)
            {
                case (int)bools.icPA:
                    retVal = iC_PAsearch;
                    break;
                case (int)bools.ocPA:
                    retVal = oC_PAsearch;
                    break;
            }
            return retVal;
        }

        public ChaosSettings(bool previewMode_, List<EntropyLayerSettings> listOfSettings, EntropySettings simSettings) // set to true for preview; else we run with random inputs
        {
            pChaosSettings(previewMode_, listOfSettings, simSettings);
        }

        void pChaosSettings(bool previewMode_, List<EntropyLayerSettings> listOfSettings, EntropySettings simSettings) // set to true for preview; else we run with random inputs
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
#if CHAOSTHREADED
            Parallel.For(0, count, (i) =>
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
                    if ((hTipBiasType[i] < 0.5) && rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipNVar_RNG)))
                    {
                        customRNGMapping = true;
                        horTipBiasVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipNVar_RNG), simSettings);
                    }
                    if ((hTipBiasType[i] >= 0.5) && rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipPVar_RNG)))
                    {
                        customRNGMapping = true;
                        horTipBiasVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipPVar_RNG), simSettings);
                    }

                    verTipBiasVar[i] = UtilityFuncs.getGaussianRandomNumber3(simSettings);
                    // Need to pay attention to sign here, to pull the right custom equation.
                    if ((vTipBiasType[i] < 0.5) && rngCheck(listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipNVar_RNG)))
                    {
                        customRNGMapping = true;
                        verTipBiasVar[i] = UtilityFuncs.getCustomMappedRandomNumber(listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipNVar_RNG), simSettings);
                    }
                    if ((vTipBiasType[i] >= 0.5) && ((listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipPVar_RNG) != "") && (listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipPVar_RNG) != CommonVars.boxMuller)))
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
#if CHAOSTHREADED
            );
#endif
            // Rewalk to handle correlation. We can't do this in the loop above since the correlated layer's settings
            // are likely not available at that point in time.
#if CHAOSTHREADED
            Parallel.For(0, count, (i) =>
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
#if CHAOSTHREADED
            );
#endif
        }

        bool rngCheck(string rngString)
        {
            return ((rngString != "") && (rngString != CommonVars.boxMuller));
        }
    }
}
