using System;
using System.Linq;
using Error;

namespace Variance
{
    [Serializable]
    public class EntropySettings
    {
        public bool debugCalc { get; set; } // job engine specific debug flag from user.

        DOESettings DOESettings;
        public DOESettings getDOESettings()
        {
            return pGetDOESettings();
        }

        DOESettings pGetDOESettings()
        {
            return DOESettings;
        }

        public enum properties_i { nCases, linkCDU, subMode, cSeg, optC, oType, rngType, ler }

        Int32 numberOfCases;
        Int32 linkTipandSideCDU;
        Int32 subMode; // allows adjustment of calculation mode to subtypes.
        Int32 cornerSegments; // CR step count
        Int32 optimizeCorners; // overrides the CR stepping with a linear check against edge resolution (see EntropyShape)

        Int32 lerFromLWR_by_sqrt2;

        Int32 outputType;

        // RNG type for run.
        Int32 rngType;

        public Int32 getValue(properties_i p)
        {
            return pGetValue(p);
        }

        Int32 pGetValue(properties_i p)
        {
            Int32 ret = 0;
            switch (p)
            {
                case properties_i.cSeg:
                    ret = cornerSegments;
                    break;
                case properties_i.linkCDU:
                    ret = linkTipandSideCDU;
                    break;
                case properties_i.nCases:
                    ret = numberOfCases;
                    break;
                case properties_i.optC:
                    ret = optimizeCorners;
                    break;
                case properties_i.oType:
                    ret = outputType;
                    break;
                case properties_i.rngType:
                    ret = rngType;
                    break;
                case properties_i.subMode:
                    ret = subMode;
                    break;
                case properties_i.ler:
                    ret = lerFromLWR_by_sqrt2;
                    break;
            }

            return ret;
        }

        public void setValue(properties_i p, int val)
        {
            pSetValue(p, val);
        }

        void pSetValue(properties_i p, int val)
        {
            switch (p)
            {
                case properties_i.cSeg:
                    cornerSegments = val;
                    break;
                case properties_i.linkCDU:
                    linkTipandSideCDU = val;
                    break;
                case properties_i.nCases:
                    numberOfCases = val;
                    break;
                case properties_i.optC:
                    optimizeCorners = val;
                    break;
                case properties_i.oType:
                    outputType = val;
                    break;
                case properties_i.rngType:
                    rngType = val;
                    break;
                case properties_i.subMode:
                    subMode = val;
                    break;
                case properties_i.ler:
                    lerFromLWR_by_sqrt2 = val;
                    break;
            }
        }

        public void defaultValue(properties_i p)
        {
            pDefaultValue(p);
        }

        void pDefaultValue(properties_i p)
        {
            switch (p)
            {
                case properties_i.cSeg:
                    cornerSegments = default_cornerSegments;
                    break;
                case properties_i.ler:
                    lerFromLWR_by_sqrt2 = default_lerFromLWR_by_sqrt2;
                    break;
                case properties_i.linkCDU:
                    linkTipandSideCDU = default_linkTipandSideCDU;
                    break;
                case properties_i.nCases:
                    numberOfCases = default_numberOfCases;
                    break;
                case properties_i.optC:
                    optimizeCorners = default_optimizeCorners;
                    break;
                case properties_i.oType:
                    outputType = default_outputType;
                    break;
                case properties_i.rngType:
                    rngType = default_rngType;
                    break;
                case properties_i.subMode:
                    subMode = default_subMode;
                    break;
            }
        }

        public Int32 getDefaultValue(properties_i p)
        {
            return pGetDefaultValue(p);
        }

        Int32 pGetDefaultValue(properties_i p)
        {
            Int32 ret = 0;
            switch (p)
            {
                case properties_i.cSeg:
                    ret = default_cornerSegments;
                    break;
                case properties_i.linkCDU:
                    ret = default_linkTipandSideCDU;
                    break;
                case properties_i.nCases:
                    ret = default_numberOfCases;
                    break;
                case properties_i.optC:
                    ret = default_optimizeCorners;
                    break;
                case properties_i.oType:
                    ret = default_outputType;
                    break;
                case properties_i.rngType:
                    ret = default_rngType;
                    break;
                case properties_i.subMode:
                    ret = default_subMode;
                    break;
                case properties_i.ler:
                    ret = default_lerFromLWR_by_sqrt2;
                    break;
            }

            return ret;
        }

        double resolution;

        public double getResolution()
        {
            return pGetResolution();
        }

        double pGetResolution()
        {
            return resolution;
        }

        public void setResolution(double val)
        {
            pSetResolution(val);
        }

        void pSetResolution(double val)
        {
            resolution = val;
        }

        public void defaultResolution()
        {
            pDefaultResolution();
        }

        void pDefaultResolution()
        {
            resolution = default_resolution;
        }

        // Index in respective drop-down
        public enum properties_o { layer, twoLayer, fourLayer, eightLayer }
        Int32[] layerOperator;
        Int32[] twoLayerOperator;
        Int32[] fourLayerOperator;
        Int32[] eightLayerOperator;

        public Int32[] getOperator(properties_o p)
        {
            return pGetOperator(p);
        }

        Int32[] pGetOperator(properties_o p)
        {
            Int32[] ret = { };
            switch (p)
            {
                case properties_o.layer:
                    ret = layerOperator;
                    break;
                case properties_o.twoLayer:
                    ret = twoLayerOperator;
                    break;
                case properties_o.fourLayer:
                    ret = fourLayerOperator;
                    break;
                case properties_o.eightLayer:
                    ret = eightLayerOperator;
                    break;
            }

            return ret;
        }

        public Int32 getOperatorValue(properties_o p, int index)
        {
            return pGetOperatorValue(p, index);
        }

        Int32 pGetOperatorValue(properties_o p, int index)
        {
            return pGetOperator(p)[index];
        }

        public void setOperatorValue(properties_o p, int index, int val)
        {
            pSetOperatorValue(p, index, val);
        }

        void pSetOperatorValue(properties_o p, int index, int val)
        {
            switch (p)
            {
                case properties_o.layer:
                    layerOperator[index] = val;
                    break;
                case properties_o.twoLayer:
                    twoLayerOperator[index] = val;
                    break;
                case properties_o.fourLayer:
                    fourLayerOperator[index] = val;
                    break;
                case properties_o.eightLayer:
                    eightLayerOperator[index] = val;
                    break;
            }
        }

        public void defaultOperator(properties_o p, int index)
        {
            pDefaultOperator(p, index);
        }

        void pDefaultOperator(properties_o p, int index)
        {
            switch (p)
            {
                case properties_o.layer:
                    layerOperator[index] = default_layerOperator;
                    break;
                case properties_o.twoLayer:
                    twoLayerOperator[index] = default_layerBoolOperator;
                    break;
                case properties_o.fourLayer:
                    fourLayerOperator[index] = default_layerBoolOperator;
                    break;
                case properties_o.eightLayer:
                    eightLayerOperator[index] = default_layerBoolOperator;
                    break;
            }
        }

        static Int32 default_outputType = (Int32)CommonVars.calcModes.area;
        static Int32 default_numberOfCases = 25000;
        static double default_resolution = 1.0;
        static Int32 default_cornerSegments = 90;
        static Int32 default_layerOperator = 0;
        static Int32 default_layerBoolOperator = 0;
        static Int32 default_linkTipandSideCDU = 1;
        static Int32 default_optimizeCorners = 1;
        static Int32 default_lerFromLWR_by_sqrt2 = 1;
        static Int32 default_rngType = 0;
        static Int32 default_subMode = 0;

        public EntropySettings()
        {
            pMCSettings();
        }

        void pMCSettings()
        {
            DOESettings = new DOESettings(); // Handle iDRM DOE where tiles need to be broken out.
            debugCalc = false;
            outputType = default_outputType;
            numberOfCases = default_numberOfCases;
            resolution = default_resolution;
            layerOperator = new int[CentralProperties.maxLayersForMC];
            for (int i = 0; i < layerOperator.Length; i++)
            {
                layerOperator[i] = default_layerOperator;
            }
            twoLayerOperator = new int[CentralProperties.maxLayersForMC / 2];
            for (int i = 0; i < twoLayerOperator.Length; i++)
            {
                twoLayerOperator[i] = default_layerBoolOperator;
            }
            fourLayerOperator = new int[twoLayerOperator.Length / 2];
            for (int i = 0; i < fourLayerOperator.Length; i++)
            {
                fourLayerOperator[i] = default_layerBoolOperator;
            }
            eightLayerOperator = new int[fourLayerOperator.Length / 2];
            for (int i = 0; i < eightLayerOperator.Length; i++)
            {
                eightLayerOperator[i] = default_layerBoolOperator;
            }

            optimizeCorners = default_optimizeCorners;
            linkTipandSideCDU = default_linkTipandSideCDU;
            cornerSegments = default_cornerSegments;
            lerFromLWR_by_sqrt2 = default_lerFromLWR_by_sqrt2;
            rngType = default_rngType; // default
            subMode = default_subMode;
        }

        public string tileListToString(bool zeroIndex = false)
        {
            return pTileListToString(zeroIndex);
        }

        string pTileListToString(bool zeroIndex)
        {
            int offset = 1;
            if (zeroIndex)
            {
                offset = 0;
            }
            string returnString = "";
            if (DOESettings.getTileList_ColRow().Any())
            {
                int entry = 0;
                returnString += (DOESettings.getTileList_Value(entry, 0) + offset) + "," + (DOESettings.getTileList_Value(entry, 1) + offset);
                entry++;
                while (entry < DOESettings.getTileList_ColRow().Count())
                {
                    returnString += ";" + (DOESettings.getTileList_Value(entry, 0) + offset) + "," + (DOESettings.getTileList_Value(entry, 1) + offset);
                    entry++;
                }
            }
            return returnString;
        }

        public bool setTileList(string tmpString, bool zeroIndex)
        {
            return pSetTileList(tmpString, zeroIndex);
        }

        bool pSetTileList(string tmpString, bool zeroIndex)
        {
            int offset = -1;
            if (zeroIndex)
            {
                offset = 0;
            }

            // We need to parse our tile list
            bool entryOK = false;
            char[] tileSep = { ';' };
            char[] coordSep = { ',' };

            if (tmpString.Length > 0)
            {
                // We have some user entries. Perform some more validation
                string[] tmpStringArray = tmpString.Split(coordSep);
                int colValue;
                int rowValue;
                switch (tmpStringArray.Count())
                {
                    case 0:
                        // No entries - abort out
                        break;
                    case 1:
                        // Only one value provided - abort out.
                        break;
                    case 2:
                        // We have one entry. Need to generate valid ints for array.
                        try
                        {
                            colValue = Convert.ToInt32(tmpStringArray[0]);
                            rowValue = Convert.ToInt32(tmpStringArray[1]);
                            DOESettings.resetTileList_ColRow();
                            DOESettings.addTileList_Value(new [] { colValue + offset, rowValue + offset });
                            entryOK = true;
                        }
                        catch (Exception)
                        {
                            // invalid input. We ignore it.
                        }
                        break;
                    default:
                        // All other cases.
                        string[] tmpCoordPairArray = tmpString.Split(tileSep);
                        if (!tmpCoordPairArray.Any())
                        {
                            // User used incorrect syntax. Failsafe.
                            ErrorReporter.showMessage_OK("Couldn't find a coordinate pair", "Invalid input");
                            break;
                        }
                        // We have some initial entries that appear to be valid.
                        DOESettings.resetTileList_ColRow();
                        for (Int32 entry = 0; entry < tmpCoordPairArray.Count(); entry++)
                        {
                            tmpStringArray = tmpCoordPairArray[entry].Split(coordSep);
                            if (tmpStringArray.Count() == 2)
                            {
                                try
                                {
                                    colValue = Convert.ToInt32(tmpStringArray[0]);
                                    rowValue = Convert.ToInt32(tmpStringArray[1]);
                                    DOESettings.addTileList_Value(new [] { colValue + offset, rowValue + offset });
                                }
                                catch (Exception)
                                {
                                    // Invalid input; ignore it.
                                }
                            }
                        }
                        entryOK = DOESettings.getTileList_ColRow().Any();
                        break;
                }
            }
            return entryOK;
        }
    }
}
