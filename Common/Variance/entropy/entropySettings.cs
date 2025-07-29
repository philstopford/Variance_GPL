#nullable enable
using System;
using System.Linq;
using Error;

namespace Variance;

[Serializable]
public class EntropySettings
{
    public bool debugCalc { get; set; } // job engine specific debug flag from user.

    private DOESettings? DOESettings;
    public DOESettings getDOESettings()
    {
        return pGetDOESettings();
    }

    private DOESettings pGetDOESettings()
    {
        return DOESettings!;
    }

    public enum properties_i { nCases, linkCDU, subMode, cSeg, optC, oType, rngType, ler }

    private int numberOfCases;
    private int linkTipandSideCDU;
    private int subMode; // allows adjustment of calculation mode to subtypes.
    private int cornerSegments; // CR step count
    private int optimizeCorners; // overrides the CR stepping with a linear check against edge resolution (see EntropyShape)

    private int lerFromLWR_by_sqrt2;

    private int outputType;

    // RNG type for run.
    private int rngType;

    public int getValue(properties_i p)
    {
        return pGetValue(p);
    }

    private int pGetValue(properties_i p)
    {
        int ret = p switch
        {
            properties_i.cSeg => cornerSegments,
            properties_i.linkCDU => linkTipandSideCDU,
            properties_i.nCases => numberOfCases,
            properties_i.optC => optimizeCorners,
            properties_i.oType => outputType,
            properties_i.rngType => rngType,
            properties_i.subMode => subMode,
            properties_i.ler => lerFromLWR_by_sqrt2,
            _ => 0
        };

        return ret;
    }

    public void setValue(properties_i p, int val)
    {
        pSetValue(p, val);
    }

    private void pSetValue(properties_i p, int val)
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

    private void pDefaultValue(properties_i p)
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

    public static int getDefaultValue(properties_i p)
    {
        return pGetDefaultValue(p);
    }

    private static int pGetDefaultValue(properties_i p)
    {
        int ret = p switch
        {
            properties_i.cSeg => default_cornerSegments,
            properties_i.linkCDU => default_linkTipandSideCDU,
            properties_i.nCases => default_numberOfCases,
            properties_i.optC => default_optimizeCorners,
            properties_i.oType => default_outputType,
            properties_i.rngType => default_rngType,
            properties_i.subMode => default_subMode,
            properties_i.ler => default_lerFromLWR_by_sqrt2,
            _ => 0
        };

        return ret;
    }

    private double resolution;

    public double getResolution()
    {
        return pGetResolution();
    }

    private double pGetResolution()
    {
        return resolution;
    }

    public void setResolution(double val)
    {
        pSetResolution(val);
    }

    private void pSetResolution(double val)
    {
        resolution = val;
    }

    public void defaultResolution()
    {
        pDefaultResolution();
    }

    private void pDefaultResolution()
    {
        resolution = default_resolution;
    }

    // Index in respective drop-down
    public enum properties_o { layer, twoLayer, fourLayer, eightLayer }

    private int[]? layerOperator;
    private int[]? twoLayerOperator;
    private int[]? fourLayerOperator;
    private int[]? eightLayerOperator;

    public int[] getOperator(properties_o p)
    {
        return pGetOperator(p);
    }

    private int[] pGetOperator(properties_o p)
    {
        int[] ret = p switch
        {
            properties_o.layer => layerOperator!,
            properties_o.twoLayer => twoLayerOperator!,
            properties_o.fourLayer => fourLayerOperator!,
            properties_o.eightLayer => eightLayerOperator!,
            _ => []
        };

        return ret;
    }

    public int getOperatorValue(properties_o p, int index)
    {
        return pGetOperatorValue(p, index);
    }

    private int pGetOperatorValue(properties_o p, int index)
    {
        return pGetOperator(p)[index];
    }

    public void setOperatorValue(properties_o p, int index, int val)
    {
        pSetOperatorValue(p, index, val);
    }

    private void pSetOperatorValue(properties_o p, int index, int val)
    {
        switch (p)
        {
            case properties_o.layer:
                layerOperator![index] = val;
                break;
            case properties_o.twoLayer:
                twoLayerOperator![index] = val;
                break;
            case properties_o.fourLayer:
                fourLayerOperator![index] = val;
                break;
            case properties_o.eightLayer:
                eightLayerOperator![index] = val;
                break;
        }
    }

    public void defaultOperator(properties_o p, int index)
    {
        pDefaultOperator(p, index);
    }

    private void pDefaultOperator(properties_o p, int index)
    {
        switch (p)
        {
            case properties_o.layer:
                layerOperator![index] = default_layerOperator;
                break;
            case properties_o.twoLayer:
                twoLayerOperator![index] = default_layerBoolOperator;
                break;
            case properties_o.fourLayer:
                fourLayerOperator![index] = default_layerBoolOperator;
                break;
            case properties_o.eightLayer:
                eightLayerOperator![index] = default_layerBoolOperator;
                break;
        }
    }

    private const int default_outputType = (int)geoAnalysis.Supported.calcModes.area;
    private const int default_numberOfCases = 25000;
    private const double default_resolution = 1.0;
    private const int default_cornerSegments = 90;
    private const int default_layerOperator = 0;
    private const int default_layerBoolOperator = 0;
    private const int default_linkTipandSideCDU = 1;
    private const int default_optimizeCorners = 1;
    private const int default_lerFromLWR_by_sqrt2 = 1;
    private const int default_rngType = 0;
    private const int default_subMode = 0;

    public EntropySettings()
    {
        pMCSettings();
    }

    private void pMCSettings()
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

    private string pTileListToString(bool zeroIndex)
    {
        int offset = 1;
        if (zeroIndex)
        {
            offset = 0;
        }
        string returnString = "";
        if (DOESettings!.getTileList_ColRow().Count == 0)
        {
            return returnString;
        }

        int entry = 0;
        returnString += DOESettings.getTileList_Value(entry, 0) + offset + "," + (DOESettings.getTileList_Value(entry, 1) + offset);
        entry++;
        while (entry < DOESettings.getTileList_ColRow().Count)
        {
            returnString += ";" + (DOESettings.getTileList_Value(entry, 0) + offset) + "," + (DOESettings.getTileList_Value(entry, 1) + offset);
            entry++;
        }
        return returnString;
    }

    public bool setTileList(string tmpString, bool zeroIndex)
    {
        return pSetTileList(tmpString, zeroIndex);
    }

    private bool pSetTileList(string tmpString, bool zeroIndex)
    {
        int offset = -1;
        if (zeroIndex)
        {
            offset = 0;
        }

        // We need to parse our tile list
        bool entryOK = false;
        char[] tileSep = [';'];
        char[] coordSep = [','];

        if (tmpString.Length <= 0)
        {
            return entryOK;
        }

        // We have some user entries. Perform some more validation
        string[] tmpStringArray = tmpString.Split(coordSep);
        int colValue;
        int rowValue;
        switch (tmpStringArray.Length)
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
                    DOESettings!.resetTileList_ColRow();
                    DOESettings.addTileList_Value([colValue + offset, rowValue + offset]);
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
                if (tmpCoordPairArray.Length == 0)
                {
                    // User used incorrect syntax. Failsafe.
                    ErrorReporter.showMessage_OK("Couldn't find a coordinate pair", "Invalid input");
                    break;
                }
                // We have some initial entries that appear to be valid.
                DOESettings!.resetTileList_ColRow();
                foreach (string t in tmpCoordPairArray)
                {
                    tmpStringArray = t.Split(coordSep);
                    if (tmpStringArray.Length != 2)
                    {
                        continue;
                    }

                    try
                    {
                        colValue = Convert.ToInt32(tmpStringArray[0]);
                        rowValue = Convert.ToInt32(tmpStringArray[1]);
                        DOESettings.addTileList_Value([colValue + offset, rowValue + offset]);
                    }
                    catch (Exception)
                    {
                        // Invalid input; ignore it.
                    }
                }
                entryOK = DOESettings.getTileList_ColRow().Count != 0;
                break;
        }
        return entryOK;
    }
}