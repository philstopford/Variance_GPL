using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Variance
{
    public class PASearch
    {
        /*
        // Disabled entries pending review for suitability and practical implementation.
            Wobble : needs to support per-polygon variation at point-of-use.
            sideBias : deep shape engine support, to propagate the PA search awareness
            tipBias : need support for positive/negative limits and variation.
            proxBias : deep shape engine support, like others.
        */
        public static string[] paNames = new string[] { "xOL", "yOL", "SCDU", "TCDU", "LWR", "LWR2", "hTipNVar", "hTipPVar", "vTipNVar", "vTipPVar", "ICR", "OCR", "Wobble" };
        public enum paEnum { XOL, YOL, SCDU, TCDU, LWR, LWR2, HTIPNVAR, HTIPPVAR, VTIPNVAR, VTIPPVAR, ICR, OCR, WOB }

        bool[,] searchablePAs;
        decimal[,] upperLimit;
        decimal[,] originalValues; // to allow restoration later.
        string[,] meanValues;

        public string getMeanValue(int x, int y)
        {
            return pGetMeanValue(x, y);
        }

        string pGetMeanValue(int x, int y)
        {
            return meanValues[x, y];
        }

        string[,] stdDevValues;

        public string getSDValue(int x, int y)
        {
            return pGetSDValue(x, y);
        }

        string pGetSDValue(int x, int y)
        {
            return stdDevValues[x, y];
        }

        // Expose these as they are less complicated than the layer case.
        public int numberofPassCases { get; set; }
        public double[] filterValues { get; set; }
        public bool[] useFilter { get; set; }
        public bool[] filterIsMaxValue { get; set; }

        public PASearch()
        {
            pPASearch();
        }

        void pPASearch()
        {
            originalValues = new decimal[CentralProperties.maxLayersForMC, paNames.Length];
            searchablePAs = new bool[CentralProperties.maxLayersForMC, paNames.Length];
            upperLimit = new decimal[CentralProperties.maxLayersForMC, paNames.Length];
            meanValues = new string[CentralProperties.maxLayersForMC, paNames.Length];
            stdDevValues = new string[CentralProperties.maxLayersForMC, paNames.Length];
            // 4 fields based on chord case.
            filterValues = new double[4];
            useFilter = new bool[4];
            filterIsMaxValue = new bool[4];
            pReset();
        }

        public void reset()
        {
            pReset();
        }

        void pReset()
        {
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                pResetLayer(i);
            }
        }

        public void resetLayer(int layer)
        {
            pResetLayer(layer);
        }

        void pResetLayer(int layer)
        {
            if ((layer < 0) || (layer > CentralProperties.maxLayersForMC))
            {
                return;
            }
            Parallel.For(0, paNames.Length, (j) =>
            // for (int j = 0; j < paNames.Length; j++)
            {
                searchablePAs[layer, j] = false;
                upperLimit[layer, j] = 0;
                meanValues[layer, j] = "";
                stdDevValues[layer, j] = "";
            });
        }

        public bool isPASearchable(int layer, int index)
        {
            return pIsPASearchable(layer, index);
        }

        bool pIsPASearchable(int layer, int index)
        {
            return searchablePAs[layer, index];
        }

        public void setPASearchable(int layer, int index, bool searchable)
        {
            pSetPASearchable(layer, index, searchable);
        }

        void pSetPASearchable(int layer, int index, bool searchable)
        {
            searchablePAs[layer, index] = searchable;
        }

        // Note that the set methods below return a value to the caller, which allows for transposition and lower limit violations to be handled.

        // Upper limit
        public decimal setUpperLimit(int layer, int index, decimal upperVal)
        {
            return pSetUpperLimit(layer, index, upperVal);
        }

        decimal pSetUpperLimit(int layer, int index, decimal upperVal)
        {
            upperLimit[layer, index] = upperVal;
            return upperLimit[layer, index];
        }

        public double getUpperLimit(int layer, int index)
        {
            return pGetUpperLimit(layer, index);
        }

        double pGetUpperLimit(int layer, int index)
        {
            return Convert.ToDouble(upperLimit[layer, index]);
        }

        public bool paAllowsNegativeValues(int index)
        {
            return pPAAllowsNegativeValues(index);
        }

        bool pPAAllowsNegativeValues(int index)
        {

            if (paNames[index].Contains("Bias"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool[] getEnabledState(EntropyLayerSettings layerSettings)
        {
            return pGetEnabledState(layerSettings);
        }

        bool[] pGetEnabledState(EntropyLayerSettings layerSettings)
        {
            int length = paNames.Length;
            bool[] enabled = new bool[length];
            Parallel.For(0, length, (i) =>
            // for (int i = 0; i < length; i++)
            {
                bool enable = layerSettings.getInt(EntropyLayerSettings.properties_i.enabled) == 1;
                if (enable)
                {
                    if (layerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CommonVars.shapeNames.GEOCORE)
                    {
                        // Disable for geoCore case, as appropriate.
                        if (layerSettings.getInt(EntropyLayerSettings.properties_i.gCSEngine) == 0)
                        {
                            switch (i)
                            {
                                case (int)paEnum.XOL:
                                case (int)paEnum.YOL:
                                    // case (int)paEnum.LWR:
                                    // case (int)paEnum.SBIAS:
                                    // case (int)paEnum.PBIAS:
                                    enable = true;
                                    break;
                                default:
                                    enable = false;
                                    break;
                            }
                        }
                    }
                }
                enabled[i] = enable;
            });

            return enabled;
        }

        public void applySearchValues(ref CommonVars commonVars)
        {
            pBackupOriginalValues(commonVars);
            pSetValues(ref commonVars);
        }

        void pBackupOriginalValues(CommonVars commonVars)
        {
            Parallel.For(0, CentralProperties.maxLayersForMC, (i) =>
            // for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (searchablePAs[i, (int)paEnum.XOL])
                {
                    originalValues[i, (int)paEnum.XOL] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.xOL);
                }
                if (searchablePAs[i, (int)paEnum.YOL])
                {
                    originalValues[i, (int)paEnum.YOL] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.yOL);
                }
                if (searchablePAs[i, (int)paEnum.SCDU])
                {
                    originalValues[i, (int)paEnum.SCDU] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.sCDU);
                }
                if (searchablePAs[i, (int)paEnum.TCDU])
                {
                    originalValues[i, (int)paEnum.TCDU] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.tCDU);
                }
                if (searchablePAs[i, (int)paEnum.LWR])
                {
                    originalValues[i, (int)paEnum.LWR] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.lwr);
                }
                if (searchablePAs[i, (int)paEnum.LWR2])
                {
                    originalValues[i, (int)paEnum.LWR2] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.lwr2);
                }
                if (searchablePAs[i, (int)paEnum.HTIPNVAR])
                {
                    originalValues[i, (int)paEnum.HTIPNVAR] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.hTNVar);
                }
                if (searchablePAs[i, (int)paEnum.HTIPPVAR])
                {
                    originalValues[i, (int)paEnum.HTIPPVAR] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.hTPVar);
                }
                if (searchablePAs[i, (int)paEnum.VTIPNVAR])
                {
                    originalValues[i, (int)paEnum.VTIPNVAR] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.vTNVar);
                }
                if (searchablePAs[i, (int)paEnum.VTIPPVAR])
                {
                    originalValues[i, (int)paEnum.VTIPPVAR] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.vTPVar);
                }
                if (searchablePAs[i, (int)paEnum.ICR])
                {
                    originalValues[i, (int)paEnum.ICR] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.iCR);
                }
                if (searchablePAs[i, (int)paEnum.OCR])
                {
                    originalValues[i, (int)paEnum.OCR] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.oCR);
                }
                if (searchablePAs[i, (int)paEnum.WOB])
                {
                    originalValues[i, (int)paEnum.WOB] = commonVars.getLayerSettings(i).getDecimal(EntropyLayerSettings.properties_decimal.wobble);
                }
            });
        }

        void pSetValues(ref CommonVars commonVars)
        {
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (searchablePAs[i, (int)paEnum.XOL])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.xOL, upperLimit[i, (int)paEnum.XOL]);
                }
                if (searchablePAs[i, (int)paEnum.YOL])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.yOL, upperLimit[i, (int)paEnum.YOL]);
                }
                if (searchablePAs[i, (int)paEnum.SCDU])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.sCDU, upperLimit[i, (int)paEnum.SCDU]);
                }
                if (searchablePAs[i, (int)paEnum.TCDU])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.tCDU, upperLimit[i, (int)paEnum.TCDU]);
                }
                if (searchablePAs[i, (int)paEnum.LWR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.lwr, upperLimit[i, (int)paEnum.LWR]);
                }
                if (searchablePAs[i, (int)paEnum.LWR2])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.lwr2, upperLimit[i, (int)paEnum.LWR2]);
                }
                if (searchablePAs[i, (int)paEnum.HTIPNVAR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.hTNVar, upperLimit[i, (int)paEnum.HTIPNVAR]);
                }
                if (searchablePAs[i, (int)paEnum.HTIPPVAR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.hTPVar, upperLimit[i, (int)paEnum.HTIPPVAR]);
                }
                if (searchablePAs[i, (int)paEnum.VTIPNVAR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.vTNVar, upperLimit[i, (int)paEnum.VTIPNVAR]);
                }
                if (searchablePAs[i, (int)paEnum.VTIPPVAR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.vTPVar, upperLimit[i, (int)paEnum.VTIPPVAR]);
                }
                if (searchablePAs[i, (int)paEnum.ICR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.iCR, upperLimit[i, (int)paEnum.ICR]);
                }
                if (searchablePAs[i, (int)paEnum.OCR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.oCR, upperLimit[i, (int)paEnum.OCR]);
                }
                if (searchablePAs[i, (int)paEnum.WOB])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.wobble, upperLimit[i, (int)paEnum.WOB]);
                }
            }
        }

        public void removeSearchValues(ref CommonVars commonVars)
        {
            pRestoreOriginalValues(ref commonVars);
        }

        void pRestoreOriginalValues(ref CommonVars commonVars)
        {
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (searchablePAs[i, (int)paEnum.XOL])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.xOL, originalValues[i, (int)paEnum.XOL]);
                }
                if (searchablePAs[i, (int)paEnum.YOL])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.yOL, originalValues[i, (int)paEnum.YOL]);
                }
                if (searchablePAs[i, (int)paEnum.SCDU])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.sCDU, originalValues[i, (int)paEnum.SCDU]);
                }
                if (searchablePAs[i, (int)paEnum.TCDU])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.tCDU, originalValues[i, (int)paEnum.TCDU]);
                }
                if (searchablePAs[i, (int)paEnum.LWR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.lwr, originalValues[i, (int)paEnum.LWR]);
                }
                if (searchablePAs[i, (int)paEnum.LWR2])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.lwr2, originalValues[i, (int)paEnum.LWR2]);
                }
                if (searchablePAs[i, (int)paEnum.HTIPNVAR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.hTNVar, originalValues[i, (int)paEnum.HTIPNVAR]);
                }
                if (searchablePAs[i, (int)paEnum.HTIPPVAR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.hTPVar, originalValues[i, (int)paEnum.HTIPPVAR]);
                }
                if (searchablePAs[i, (int)paEnum.VTIPNVAR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.vTNVar, originalValues[i, (int)paEnum.VTIPNVAR]);
                }
                if (searchablePAs[i, (int)paEnum.VTIPPVAR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.vTPVar, originalValues[i, (int)paEnum.VTIPPVAR]);
                }
                if (searchablePAs[i, (int)paEnum.ICR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.iCR, originalValues[i, (int)paEnum.ICR]);
                }
                if (searchablePAs[i, (int)paEnum.OCR])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.oCR, originalValues[i, (int)paEnum.OCR]);
                }
                if (searchablePAs[i, (int)paEnum.WOB])
                {
                    commonVars.getLayerSettings(i).setDecimal(EntropyLayerSettings.properties_decimal.wobble, originalValues[i, (int)paEnum.WOB]);
                }
            }
        }

        public void calculateMeanAndStdDev(SimResultPackage resultPackage)
        {
            pCalculateMeanAndStdDev(resultPackage);
        }

        void pCalculateMeanAndStdDev(SimResultPackage resultPackage)
        {
            // Find out whether we are actually interested in a given PA for a layer
            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                layer_meanStdDev(resultPackage, layer);
            }
        }

        void layer_meanStdDev(SimResultPackage resultPackage, int layer)
        {
            for (int pa = 0; pa < paNames.Length; pa++)
            {
                pa_layer_meanStdDev(resultPackage, layer, pa);
            }
        }

        void pa_layer_meanStdDev(SimResultPackage resultPackage, int layer, int pa)
        {
            // We're interested in this PA, so we need to pull out the values and calculate the mean and standard deviation for this input to the simulation.
            if (searchablePAs[layer, pa])
            {
                List<double> rawValues = new List<double>();
                for (int resultField = 0; resultField < resultPackage.getListOfResults().Count; resultField++)
                {
                    double factor = 0;
                    switch (pa)
                    {
                        case (int)paEnum.XOL:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.olx, layer);
                            break;
                        case (int)paEnum.YOL:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.oly, layer);
                            break;
                        case (int)paEnum.SCDU:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.svar, layer);
                            break;
                        case (int)paEnum.TCDU:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.tvar, layer);
                            break;
                        case (int)paEnum.LWR:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.lwr, layer);
                            break;
                        case (int)paEnum.LWR2:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.lwr2, layer);
                            break;
                        case (int)paEnum.HTIPNVAR:
                        case (int)paEnum.HTIPPVAR:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.htip, layer);
                            break;
                        case (int)paEnum.VTIPNVAR:
                        case (int)paEnum.VTIPPVAR:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.vtip, layer);
                            break;
                        case (int)paEnum.ICR:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.icv, layer);
                            break;
                        case (int)paEnum.OCR:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.ocv, layer);
                            break;
                        case (int)paEnum.WOB:
                            factor = resultPackage.getResult(resultField).getField(Results.fields_d.wob, layer);
                            break;
                    }
                    // Since we're talking variations, we need absolute values - can't have a negative CDU value, for example.
                    // RNG expects 3-sigma PA, so we need to convert back to 3-sigma for reporting.
                    rawValues.Add(Math.Abs(factor) * Convert.ToDouble(upperLimit[layer, pa]));
                }
                double meanVal = mean(rawValues);
                if (meanVal != -1)
                {
                    meanValues[layer, pa] = meanVal.ToString("0.##");
                    if (!resultPackage.nonGaussianInput)
                    {
                        stdDevValues[layer, pa] = stdDev(rawValues).ToString("0.##");
                    }
                    else
                    {
                        stdDevValues[layer, pa] = "N/A";
                    }
                }
                else
                {
                    meanValues[layer, pa] = "N/A";
                    stdDevValues[layer, pa] = "N/A";
                }
            }
            else
            {
                meanValues[layer, pa] = "";
                stdDevValues[layer, pa] = "";
            }
        }

        double mean(List<double> rawValues)
        {
            if (rawValues.Count == 0)
            {
                return -1;
            }
            else
            {
                return rawValues.Average();
            }
        }

        double stdDev(List<double> rawValues)
        {
            // Welford's approach
            double m = 0.0;
            double s = 0.0;
            int k = 1;

            for (int value = 0; value < rawValues.Count; value++)
            {
                double tmpM = m;

                double _value = rawValues[value];
                m += (_value - tmpM) / k;
                s += (_value - tmpM) * (_value - m);
                k++;
            }

            return Math.Sqrt(s / (k - 2));
        }

        public void modifyJobSettings(ref ChaosSettings jobSettings)
        {
            pModifyJobSettings(ref jobSettings);
        }

        // In Gaussian mode, variations are scaled from 3-sigma to 1-sigma, but in PA search, we need to reverse this, so upscale the RNG value accordingly.
        void pModifyJobSettings(ref ChaosSettings jobSettings)
        {
            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                for (int pa = 0; pa < paNames.Length; pa++)
                {
                    if (isPASearchable(layer, pa))
                    {
                        switch (pa)
                        {
                            case (int)paEnum.XOL:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.overlayX, layer, jobSettings.getValue(ChaosSettings.properties.overlayX, layer) * 3);
                                }
                                break;
                            case (int)paEnum.YOL:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.overlayY, layer, jobSettings.getValue(ChaosSettings.properties.overlayY, layer) * 3);
                                }
                                break;
                            case (int)paEnum.SCDU:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.CDUSVar, layer, jobSettings.getValue(ChaosSettings.properties.CDUSVar, layer) * 3);
                                }
                                break;
                            case (int)paEnum.TCDU:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.CDUTVar, layer, jobSettings.getValue(ChaosSettings.properties.CDUTVar, layer) * 3);
                                }
                                break;
                            case (int)paEnum.LWR:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.LWRVar, layer, jobSettings.getValue(ChaosSettings.properties.LWRVar, layer) * 3);
                                }
                                break;
                            case (int)paEnum.LWR2:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.LWR2Var, layer, jobSettings.getValue(ChaosSettings.properties.LWR2Var, layer) * 3);
                                }
                                break;
                            case (int)paEnum.HTIPNVAR:
                            case (int)paEnum.HTIPPVAR:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.hTipBiasVar, layer, jobSettings.getValue(ChaosSettings.properties.hTipBiasVar, layer) * 3);
                                }
                                break;
                            case (int)paEnum.VTIPPVAR:
                            case (int)paEnum.VTIPNVAR:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.vTipBiasVar, layer, jobSettings.getValue(ChaosSettings.properties.vTipBiasVar, layer) * 3);
                                }
                                break;
                            case (int)paEnum.ICR:
                                jobSettings.setBool(ChaosSettings.bools.icPA, layer, true);
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.icVar, layer, jobSettings.getValue(ChaosSettings.properties.icVar, layer) * 3);
                                }
                                break;
                            case (int)paEnum.OCR:
                                jobSettings.setBool(ChaosSettings.bools.ocPA, layer, true);
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.ocVar, layer, jobSettings.getValue(ChaosSettings.properties.ocVar, layer) * 3);
                                }
                                break;
                            case (int)paEnum.WOB:
                                if (!jobSettings.getCustomRNGMapping())
                                {
                                    jobSettings.setValue(ChaosSettings.properties.wobbleVar, layer, jobSettings.getValue(ChaosSettings.properties.wobbleVar, layer) * 3);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}