using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Error;
using geoLib;
using shapeEngine;
using utility;

namespace Variance;

public class Storage
{
    public delegate void PrepUI();
    public PrepUI prepUI { get; set; }
    public delegate void StorageSetSettingsCallback(EntropyLayerSettings readSettings, int layer, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false);
    public StorageSetSettingsCallback setLayerSettings { get; set; }
    public delegate void StorageSuspendSettingsUICallback();
    public StorageSuspendSettingsUICallback suspendSettingsUI { get; set; }
    public delegate void StorageSuspendImplantUICallback();
    public StorageSuspendImplantUICallback suspendImplantUI { get; set; }
    public delegate void StorageSuspendGDSSettingsUICallback();
    public StorageSuspendGDSSettingsUICallback suspendDOESettingsUI { get; set; }
    public delegate void StorageUpdateSettingsUICallback();
    public StorageUpdateSettingsUICallback updateSettingsUIFromSettings { get; set; }
    public delegate void StorageUpdateImplantUICallback();
    public StorageUpdateImplantUICallback updateImplantUIFromSettings { get; set; }
    public delegate void StorageUpdateDOESettingsUICallback();
    public StorageUpdateDOESettingsUICallback updateDOESettingsUIFromSettings { get; set; }
    public delegate bool StorageUpdateLoadLayoutCallback(int layer, string filename);
    public StorageUpdateLoadLayoutCallback loadLayout { get; set; }
    public delegate void ResumeUICallback();
    public ResumeUICallback resumeUI { get; set; }

    // Callbacks for viewport values.
    public delegate void viewportLoadCallback(int index, double[] camInfo);
    public viewportLoadCallback viewportLoad { get; set; }
    public delegate double[] viewportSaveCallback(int index); // 2 fields : X and Y, zoom
    public viewportSaveCallback viewportSave { get; set; }

    public delegate void implantViewportLoadCallback(double[] camInfo);
    public implantViewportLoadCallback implantViewportLoad { get; set; }
    public delegate double[] implantViewportSaveCallback(); // 3 fields : X and Y, zoom
    public implantViewportSaveCallback implantViewportSave { get; set; }

    private bool[] loadedLayers;

    public Storage()
    {
        pStorage();
    }

    private void pStorage()
    {
        setLayerSettings = null;
        suspendSettingsUI = null;
        suspendDOESettingsUI = null;
        updateSettingsUIFromSettings = null;
        updateDOESettingsUIFromSettings = null;
    }

    // compatibility shim for old projects pre-3.0

    private void modifyLayerRefs_forLayerChange_1xto21(ref EntropyLayerSettings readSettings,
        int compatibilityLayerOffset)
    {
        if (readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == 2 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == 3)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.CDU_corr_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == 2 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == 3)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == 2 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == 3)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.xOL_corr_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == 2 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == 3)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.yOL_corr_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) == 2 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) == 3)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.xOL_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) == 2 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) == 3)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.yOL_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) + compatibilityLayerOffset);
        }

        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 2 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 2));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 3 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 3));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 2, 0);
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 3, 0);

        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 2 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 2));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 3 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 3));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 2, 0);
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 3, 0);
    }

    private void modifyLayerRefs_forLayerChange_220(ref EntropyLayerSettings readSettings, int compatibilityLayerOffset)
    {
        if (readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == 4 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == 5 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == 6 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == 7)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.CDU_corr_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == 4 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == 5 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == 6 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == 7)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == 4 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == 5 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == 6 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == 7)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.xOL_corr_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == 4 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == 5 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == 6 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == 7)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.yOL_corr_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) == 4 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) == 5 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) == 6 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) == 7)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.xOL_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) + compatibilityLayerOffset);
        }

        if (readSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) == 4 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) == 5 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) == 6 ||
            readSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) == 7)
        {
            readSettings.setInt(EntropyLayerSettings.properties_i.yOL_ref,
                readSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) + compatibilityLayerOffset);
        }

        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 4 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 4));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 5 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 5));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 6 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 6));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 7 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 7));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 4, 0);
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 5, 0);
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 6, 0);
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 7, 0);

        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 4 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 4));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 5 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 5));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 6 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 6));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 7 + compatibilityLayerOffset,
            readSettings.getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 7));
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 4, 0);
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 5, 0);
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 6, 0);
        readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 7, 0);
    }

    private void modifyLayerRefs_forLayerChange(ref EntropyLayerSettings readSettings, int compatibilityLayerOffset)
    {
        switch (compatibilityLayerOffset)
        {
            // v1.x-2.1 file
            case 6:
            {
                modifyLayerRefs_forLayerChange_1xto21(ref readSettings, compatibilityLayerOffset);
                break;
            }
            // v2.2 file
            case 4:
            {
                modifyLayerRefs_forLayerChange_220(ref readSettings, compatibilityLayerOffset);
                break;
            }
        }
    }

    private List<GeoLibPointF[]> fileDataFromString(string fileDataString)
    {
        List<GeoLibPointF[]> returnList = new();

        char[] polySep = { ';' };
        char[] coordSep = { ',' };

        if (fileDataString.Length > 0)
        {
            List<string> hashList = new();

            string[] polyStringArray = fileDataString.Split(polySep);
            foreach (string t in polyStringArray)
            {
                string[] pointStringArray = t.Split(coordSep);
                GeoLibPointF[] polyData = new GeoLibPointF[pointStringArray.Length / 2]; // since we have two coord values per point (X,Y)
                int pt = 0;
                while (pt < pointStringArray.Length)
                {
                    polyData[pt / 2] = new GeoLibPointF((float)Convert.ToDouble(pointStringArray[pt]), (float)Convert.ToDouble(pointStringArray[pt + 1]));
                    pt += 2;
                }

                // Avoid duplicated geometry - this is insurance against older projects files that may have doubled-up polygons included.
                string p_Hash = Utils.GetMD5Hash(polyData);
                switch (hashList.IndexOf(p_Hash))
                {
                    case -1:
                        hashList.Add(p_Hash);
                        returnList.Add(polyData);
                        break;
                }
            }
        }
        else
        {
            returnList.Add(new[] { new GeoLibPointF(0, 0) });
            returnList.Add(new[] { new GeoLibPointF(0, 0) });
            returnList.Add(new[] { new GeoLibPointF(0, 0) });
        }
        return returnList;
    }

    private string stringFromFileData(List<GeoLibPointF[]> fileData)
    {
        string returnString = "";
        if (fileData == null)
        {
            return returnString;
        }

        int poly = 0;
        int pt = 0;
        returnString += fileData[poly][pt].X + "," + fileData[poly][pt].Y;
        pt++;
        while (pt < fileData[poly].Length)
        {
            returnString += "," + fileData[poly][pt].X + "," + fileData[poly][pt].Y;
            pt++;
        }
        poly++;
        while (poly < fileData.Count)
        {
            returnString += ";";
            pt = 0;
            returnString += fileData[poly][0].X + "," + fileData[poly][0].Y;
            pt++;
            while (pt < fileData[poly].Length)
            {
                returnString += "," + fileData[poly][pt].X + "," + fileData[poly][pt].Y;
                pt++;
            }
            poly++;
        }

        return returnString;
    }

    public bool storeSimulationSettings(string filename, EntropySettings simulationSettings, EntropySettings_nonSim simulationSettings_nonSim, List<EntropyLayerSettings> listOfSettings, EntropySettings implantSimSettings, EntropySettings_nonSim implantSettings_nonSim, EntropyImplantSettings implantSettings_, NonSimulationSettings nonSimulationSettings_)
    {
        return pStoreSimulationSettings(filename, simulationSettings, simulationSettings_nonSim, listOfSettings, implantSimSettings, implantSettings_nonSim, implantSettings_, nonSimulationSettings_);
    }

    private void pStoreSimulationSettings_layer(ref XDocument doc, List<EntropyLayerSettings> listOfSettings)
    {
        for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
        {
            double[] camParameters = viewportSave?.Invoke(i);
            XElement xelement = new("layer" + (i + 1),
                new XElement("name", listOfSettings[i].getString(EntropyLayerSettings.properties_s.name)),
                new XElement("comment", listOfSettings[i].getString(EntropyLayerSettings.properties_s.comment)),
                new XElement("enabled", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.enabled)),
                new XElement("omitLayer", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.omit)),
                new XElement("layer1BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 0)),
                new XElement("layer2BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 1)),
                new XElement("layer3BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 2)),
                new XElement("layer4BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 3)),
                new XElement("layer5BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 4)),
                new XElement("layer6BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 5)),
                new XElement("layer7BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 6)),
                new XElement("layer8BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 7)),
                new XElement("layer9BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 8)),
                new XElement("layer10BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 9)),
                new XElement("layer11BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 10)),
                new XElement("layer12BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 11)),
                new XElement("layer13BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 12)),
                new XElement("layer14BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 13)),
                new XElement("layer15BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 14)),
                new XElement("layer16BG", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 15)),
                new XElement("showDrawn", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.showDrawn)),
                new XElement("flipH", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.flipH)),
                new XElement("flipV", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.flipV)),
                new XElement("alignGeomX", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.alignX)),
                new XElement("alignGeomY", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.alignY)),
                new XElement("shapeIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.shapeIndex)),
                new XElement("geoCoreShapeEngine", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.gCSEngine)),
                new XElement("subShapeHorLength", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.horLength, 0)),
                new XElement("subShapeHorOffset", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 0)),
                new XElement("subShapeVerLength", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.verLength, 0)),
                new XElement("subShapeVerOffset", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 0)),
                new XElement("subShapeTipLocIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.shape0Tip)),
                new XElement("subShape2HorLength", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.horLength, 1)),
                new XElement("subShape2HorOffset", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 1)),
                new XElement("subShape2VerLength", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.verLength, 1)),
                new XElement("subShape2VerOffset", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 1)),
                new XElement("subShape2TipLocIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.shape1Tip)),
                new XElement("subShape3HorLength", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.horLength, 2)),
                new XElement("subShape3HorOffset", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.horOffset, 2)),
                new XElement("subShape3VerLength", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.verLength, 2)),
                new XElement("subShape3VerOffset", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.verOffset, 2)),
                new XElement("subShape3TipLocIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.shape2Tip)),
                new XElement("subShapeRefIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.subShapeIndex)),
                new XElement("posInSubShapeIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.posIndex)),
                new XElement("globalHorOffset", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset)),
                new XElement("globalVerOffset", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset)),
                new XElement("rotation", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.rot)),
                new XElement("wobble", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.wobble)),
                new XElement("wobbleRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.wobble_RNG)),
                new XElement("sideBias", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.sBias)),
                new XElement("horTipBias", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.hTBias)),
                new XElement("horTipBiasPVar", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.hTPVar)),
                new XElement("horTipBiasPVarRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipPVar_RNG)),
                new XElement("horTipBiasNVar", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.hTNVar)),
                new XElement("horTipBiasNVarRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.hTipNVar_RNG)),
                new XElement("verTipBias", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.vTBias)),
                new XElement("verTipBiasPVar", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.vTPVar)),
                new XElement("verTipBiasPVarRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipPVar_RNG)),
                new XElement("verTipBiasNVar", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.vTNVar)),
                new XElement("verTipBiasNVarRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.vTipNVar_RNG)),
                new XElement("proximityBias", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.pBias)),
                new XElement("proximityIsoDistance", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.pBiasDist)),
                new XElement("proximitySideRays", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.proxRays)),
                new XElement("proximitySideRayFallOff", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.proxSideRaysFallOff)),
                new XElement("proximitySideRayFallOffMultiplier", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.proxSideRaysMultiplier)),
                new XElement("innerCRR", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.iCR)),
                new XElement("outerCRR", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.oCR)),
                new XElement("innerCV", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.iCV)),
                new XElement("innerCVRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.iCV_RNG)),
                new XElement("outerCV", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.oCV)),
                new XElement("outerCVRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.oCV_RNG)),
                new XElement("edgeSlide", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.edgeSlide)),
                new XElement("edgeSlideTension", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.eTension)),
                new XElement("LWR", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.lwr)),
                new XElement("LWRRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.lwr_RNG)),
                new XElement("LWRNoiseFreq", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.lwrFreq)),
                new XElement("LWRNoiseType", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwrType)),
                new XElement("correlatedLWR", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr_corr)),
                new XElement("correlatedLWRLayerIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr_corr_ref)),
                new XElement("LWR2", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.lwr2)),
                new XElement("LWR2RNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.lwr2_RNG)),
                new XElement("LWR2NoiseFreq", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq)),
                new XElement("LWR2NoiseType", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr2Type)),
                new XElement("correlatedLWR2", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr2_corr)),
                new XElement("correlatedLWR2LayerIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwr2_corr_ref)),
                new XElement("LWRNoisePreview", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lwrPreview)),
                new XElement("sideCDU", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.sCDU)),
                new XElement("sideCDURNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.sCDU_RNG)),
                new XElement("tipsCDU", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.tCDU)),
                new XElement("tipsCDURNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.tCDU_RNG)),
                new XElement("horOverlay", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.xOL)),
                new XElement("horOverlayRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.xOL_RNG)),
                new XElement("verOverlay", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.yOL)),
                new XElement("verOverlayRNGMapping", listOfSettings[i].getString(EntropyLayerSettings.properties_s.yOL_RNG)),
                new XElement("correlatedXOverlay", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.xOL_corr)),
                new XElement("correlatedXOverlayLayerIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.xOL_corr_ref)),
                new XElement("correlatedYOverlay", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.yOL_corr)),
                new XElement("correlatedYOverlayLayerIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.yOL_corr_ref)),
                new XElement("correlatedTipCDU", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.tCDU_corr)),
                new XElement("correlatedTipCDULayerIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref)),
                new XElement("correlatedCDU", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.CDU_corr)),
                new XElement("correlatedCDULayerIndex", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.CDU_corr_ref)),
                new XElement("overlayXReferenceLayer", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.xOL_ref)),
                new XElement("overlayYReferenceLayer", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.yOL_ref)),

                new XElement("averageOverlayX", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.xOL_av)),
                new XElement("overlayXReferenceLayer_1", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 0)),
                new XElement("overlayXReferenceLayer_2", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 1)),
                new XElement("overlayXReferenceLayer_3", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 2)),
                new XElement("overlayXReferenceLayer_4", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 3)),
                new XElement("overlayXReferenceLayer_5", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 4)),
                new XElement("overlayXReferenceLayer_6", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 5)),
                new XElement("overlayXReferenceLayer_7", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 6)),
                new XElement("overlayXReferenceLayer_8", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 7)),
                new XElement("overlayXReferenceLayer_9", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 8)),
                new XElement("overlayXReferenceLayer_10", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 9)),
                new XElement("overlayXReferenceLayer_11", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 10)),
                new XElement("overlayXReferenceLayer_12", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 11)),
                new XElement("overlayXReferenceLayer_13", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 12)),
                new XElement("overlayXReferenceLayer_14", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 13)),
                new XElement("overlayXReferenceLayer_15", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 14)),
                new XElement("overlayXReferenceLayer_16", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 15)),

                new XElement("averageOverlayY", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.yOL_av)),
                new XElement("overlayYReferenceLayer_1", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 0)),
                new XElement("overlayYReferenceLayer_2", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 1)),
                new XElement("overlayYReferenceLayer_3", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 2)),
                new XElement("overlayYReferenceLayer_4", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 3)),
                new XElement("overlayYReferenceLayer_5", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 4)),
                new XElement("overlayYReferenceLayer_6", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 5)),
                new XElement("overlayYReferenceLayer_7", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 6)),
                new XElement("overlayYReferenceLayer_8", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 7)),
                new XElement("overlayYReferenceLayer_9", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 8)),
                new XElement("overlayYReferenceLayer_10", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 9)),
                new XElement("overlayYReferenceLayer_11", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 10)),
                new XElement("overlayYReferenceLayer_12", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 11)),
                new XElement("overlayYReferenceLayer_13", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 12)),
                new XElement("overlayYReferenceLayer_14", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 13)),
                new XElement("overlayYReferenceLayer_15", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 14)),
                new XElement("overlayYReferenceLayer_16", listOfSettings[i].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 15)),

                new XElement("lensDistortionCoeff1", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.lDC1)),
                new XElement("lensDistortionCoeff2", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.lDC2)),

                new XElement("booleanLayerA", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.bLayerA)),
                new XElement("booleanLayerB", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.bLayerB)),
                new XElement("booleanLayerOpA", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.bLayerOpA)),
                new XElement("booleanLayerOpB", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.bLayerOpB)),
                new XElement("booleanLayerOpAB", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.bLayerOpAB)),
                new XElement("rayExtension", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.rayExtension)),

                new XElement("displayZoomFactor", camParameters != null ? camParameters[2] : 1),
                new XElement("viewportX", camParameters != null ? camParameters[0] : 0),
                new XElement("viewportY", camParameters != null ? camParameters[1] : 0),
                new XElement("fileToLoad", listOfSettings[i].getString(EntropyLayerSettings.properties_s.file)),
                new XElement("structureFromFile", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.structure)),
                new XElement("structureNameFromFile", listOfSettings[i].getString(EntropyLayerSettings.properties_s.structure)),
                new XElement("ldFromFile", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.lD)),
                new XElement("ldNameFromFile", listOfSettings[i].getString(EntropyLayerSettings.properties_s.lD)),
                new XElement("fileType", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.fileType)),
                new XElement("polyFill", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.fill)),
                new XElement("geoCoreShapeEngine", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.gCSEngine)),
                new XElement("geoCoreShapeEnginePerPoly", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.perPoly)),
                new XElement("geoCoreReferenceLayout", listOfSettings[i].getInt(EntropyLayerSettings.properties_i.refLayout)),
                new XElement("geoCoreKeyholeSizing", listOfSettings[i].getDecimal(EntropyLayerSettings.properties_decimal.keyhole_factor)),
                // fileData is List<PointF[]> where each list entry is a polygon.
                new XElement("fileData", stringFromFileData(listOfSettings[i].getFileData()))
            );
            doc.Root.Add(xelement);
        }
    }

    private void pStoreSimulationSettings_simulation(ref XDocument doc, EntropySettings simulationSettings, EntropySettings_nonSim simulationSettings_nonSim)
    {
        double[] camParameters = viewportSave?.Invoke(CentralProperties.maxLayersForMC);
        doc.Root.Add(new XElement("settings",
            new XElement("comment", simulationSettings_nonSim.getString(EntropySettings_nonSim.properties_s.comment)),
            new XElement("paSearchComment", simulationSettings_nonSim.getString(EntropySettings_nonSim.properties_s.paComment)),
            new XElement("numberOfCases", simulationSettings.getValue(EntropySettings.properties_i.nCases)),
            new XElement("resolution", simulationSettings.getResolution()),
            new XElement("displayResults", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.results)),
            new XElement("displayZoomFactor", camParameters != null ? camParameters[2] : 1),
            new XElement("viewportX", camParameters != null ? camParameters[0] : 0),
            new XElement("viewportY", camParameters != null ? camParameters[1] : 0),
            new XElement("displayShape", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.shape)),
            new XElement("generateExternal", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.external)),
            new XElement("externalType", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.externalType)),
            new XElement("externalCriteria", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.externalCriteria)),
            new XElement("externalCritCond1", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.externalCritCond1)),
            new XElement("externalCritValue1", simulationSettings_nonSim.getDecimal(EntropySettings_nonSim.properties_d.externalCritCond1)),
            new XElement("externalCritCond2", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.externalCritCond2)),
            new XElement("externalCritValue2", simulationSettings_nonSim.getDecimal(EntropySettings_nonSim.properties_d.externalCritCond2)),
            new XElement("externalCritCond3", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.externalCritCond3)),
            new XElement("externalCritValue3", simulationSettings_nonSim.getDecimal(EntropySettings_nonSim.properties_d.externalCritCond3)),
            new XElement("externalCritCond4", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.externalCritCond4)),
            new XElement("externalCritValue4", simulationSettings_nonSim.getDecimal(EntropySettings_nonSim.properties_d.externalCritCond4)),
            new XElement("generateCSV", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.csv)),
            new XElement("linkTipandSideCDU", simulationSettings.getValue(EntropySettings.properties_i.linkCDU)),
            new XElement("lerFromLWR_by_sqrt2", simulationSettings.getValue(EntropySettings.properties_i.ler)),
            new XElement("subMode", simulationSettings.getValue(EntropySettings.properties_i.subMode)),
            new XElement("cornerSegments", simulationSettings.getValue(EntropySettings.properties_i.cSeg)),
            new XElement("optimizeCorners", simulationSettings.getValue(EntropySettings.properties_i.optC)),
            new XElement("greedyMode", simulationSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.greedy)),
            new XElement("rng", simulationSettings.getValue(EntropySettings.properties_i.rngType)),
            new XElement("outputType", simulationSettings.getValue(EntropySettings.properties_i.oType)),
            new XElement("layer1Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 0)),
            new XElement("layer2Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 1)),
            new XElement("layer3Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 2)),
            new XElement("layer4Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 3)),
            new XElement("layer5Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 4)),
            new XElement("layer6Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 5)),
            new XElement("layer7Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 6)),
            new XElement("layer8Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 7)),
            new XElement("layer9Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 8)),
            new XElement("layer10Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 9)),
            new XElement("layer11Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 10)),
            new XElement("layer12Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 11)),
            new XElement("layer13Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 12)),
            new XElement("layer14Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 13)),
            new XElement("layer15Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 14)),
            new XElement("layer16Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 15)),
            new XElement("layer1_2Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 0)),
            new XElement("layer3_4Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 1)),
            new XElement("layer5_6Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 2)),
            new XElement("layer7_8Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 3)),
            new XElement("layer9_10Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 4)),
            new XElement("layer11_12Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 5)),
            new XElement("layer13_14Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 6)),
            new XElement("layer15_16Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 7)),
            new XElement("layer12_34Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 0)),
            new XElement("layer56_78Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 1)),
            new XElement("layer910_1112Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 2)),
            new XElement("layer1314_1516Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 3)),
            new XElement("layer1234_5678Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, 0)),
            new XElement("layer9101112_13141516Operator", simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, 1))
        ));
    }

    private void pStoreSimulationSettings_DOE(ref XDocument doc, EntropySettings simulationSettings)
    {
        double[] camParameters = viewportSave?.Invoke(CentralProperties.maxLayersForMC + 1);
        doc.Root.Add(new XElement("DOESettings",
            new XElement("displayZoomFactor", camParameters != null ? camParameters[2] : 1),
            new XElement("viewportX", camParameters != null ? camParameters[0] : 0),
            new XElement("viewportY", camParameters != null ? camParameters[1] : 0),
            new XElement("comment", simulationSettings.getDOESettings().getString(DOESettings.properties_s.comment)),
            new XElement("layer1Affected", simulationSettings.getDOESettings().getLayerAffected(0)),
            new XElement("layer2Affected", simulationSettings.getDOESettings().getLayerAffected(1)),
            new XElement("layer3Affected", simulationSettings.getDOESettings().getLayerAffected(2)),
            new XElement("layer4Affected", simulationSettings.getDOESettings().getLayerAffected(3)),
            new XElement("layer5Affected", simulationSettings.getDOESettings().getLayerAffected(4)),
            new XElement("layer6Affected", simulationSettings.getDOESettings().getLayerAffected(5)),
            new XElement("layer7Affected", simulationSettings.getDOESettings().getLayerAffected(6)),
            new XElement("layer8Affected", simulationSettings.getDOESettings().getLayerAffected(7)),
            new XElement("layer9Affected", simulationSettings.getDOESettings().getLayerAffected(8)),
            new XElement("layer10Affected", simulationSettings.getDOESettings().getLayerAffected(9)),
            new XElement("layer11Affected", simulationSettings.getDOESettings().getLayerAffected(10)),
            new XElement("layer12Affected", simulationSettings.getDOESettings().getLayerAffected(11)),
            new XElement("layer13Affected", simulationSettings.getDOESettings().getLayerAffected(12)),
            new XElement("layer14Affected", simulationSettings.getDOESettings().getLayerAffected(13)),
            new XElement("layer15Affected", simulationSettings.getDOESettings().getLayerAffected(14)),
            new XElement("layer16Affected", simulationSettings.getDOESettings().getLayerAffected(15)),
            new XElement("rowPitch", simulationSettings.getDOESettings().getDouble(DOESettings.properties_d.rowPitch)),
            new XElement("colPitch", simulationSettings.getDOESettings().getDouble(DOESettings.properties_d.colPitch)),
            new XElement("rows", simulationSettings.getDOESettings().getInt(DOESettings.properties_i.rows)),
            new XElement("cols", simulationSettings.getDOESettings().getInt(DOESettings.properties_i.cols)),
            new XElement("specificTile", simulationSettings.getDOESettings().getInt(DOESettings.properties_i.sTile)),
            new XElement("specificTile_Row",
                simulationSettings.getDOESettings().getInt(DOESettings.properties_i.sTileRow)),
            new XElement("specificTile_Col",
                simulationSettings.getDOESettings().getInt(DOESettings.properties_i.sTileCol)),
            new XElement("colOffset",
                simulationSettings.getDOESettings().getDouble(DOESettings.properties_d.colOffset)),
            new XElement("rowOffset",
                simulationSettings.getDOESettings().getDouble(DOESettings.properties_d.rowOffset)),
            new XElement("listOfTiles", simulationSettings.getDOESettings().getInt(DOESettings.properties_i.uTileList)),
            new XElement("tileList_ColRow", simulationSettings.tileListToString(true)),
            new XElement("iDRMConfigured",
                simulationSettings.getDOESettings().getBool(DOESettings.properties_b.iDRM) ? 1 : 0)
        ));
    }

    private void pStoreSimulationSettings_implant(ref XDocument doc, EntropySettings implantSimSettings, EntropySettings_nonSim implantSettings_nonSim, EntropyImplantSettings implantSettings_, NonSimulationSettings nonSimulationSettings_)
    {
        double[] camParameters = implantViewportSave?.Invoke();
        doc.Root.Add(new XElement("implant",
            new XElement("displayZoomFactor", camParameters != null ? camParameters[2] : 1),
            new XElement("viewportX", camParameters != null ? camParameters[0] : 0),
            new XElement("viewportY", camParameters != null ? camParameters[1] : 0),
            new XElement("comment", implantSettings_.getComment()),
            new XElement("resistCRR", implantSettings_.getDouble(EntropyImplantSettings.properties_d.cRR)),
            new XElement("resistCRRVar", implantSettings_.getDouble(EntropyImplantSettings.properties_d.cV)),
            new XElement("resistHeight_postDevelop", implantSettings_.getDouble(EntropyImplantSettings.properties_d.h)),
            new XElement("resistHeight_postDevelopVar", implantSettings_.getDouble(EntropyImplantSettings.properties_d.hV)),
            new XElement("resistWidth", implantSettings_.getDouble(EntropyImplantSettings.properties_d.w)),
            new XElement("resistWidthVar", implantSettings_.getDouble(EntropyImplantSettings.properties_d.wV)),
            new XElement("tiltAngle", implantSettings_.getDouble(EntropyImplantSettings.properties_d.tilt)),
            new XElement("tiltAngleVar", implantSettings_.getDouble(EntropyImplantSettings.properties_d.tiltV)),
            new XElement("twistAngle", implantSettings_.getDouble(EntropyImplantSettings.properties_d.twist)),
            new XElement("twistAngleVar", implantSettings_.getDouble(EntropyImplantSettings.properties_d.twistV)),
            new XElement("numberOfCases", implantSimSettings.getValue(EntropySettings.properties_i.nCases)),
            new XElement("cornerSegments", implantSimSettings.getValue(EntropySettings.properties_i.cSeg)),
            new XElement("rngType", implantSimSettings.getValue(EntropySettings.properties_i.rngType)),
            new XElement("generateCSV", implantSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.csv)),
            new XElement("generateExternal", implantSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.external)),
            new XElement("externalType", implantSettings_nonSim.getInt(EntropySettings_nonSim.properties_i.externalType))
        ));
    }

    private bool pStoreSimulationSettings(string filename, EntropySettings simulationSettings, EntropySettings_nonSim simulationSettings_nonSim, List<EntropyLayerSettings> listOfSettings, EntropySettings implantSimSettings, EntropySettings_nonSim implantSettings_nonSim, EntropyImplantSettings implantSettings_, NonSimulationSettings nonSimulationSettings_)
    {
        XDocument doc = new(new XElement("Variance"));
        // ReSharper disable once PossibleNullReferenceException
        doc.Root.Add(new XElement("version", nonSimulationSettings_.version));
        
        pStoreSimulationSettings_layer(ref doc, listOfSettings);
        pStoreSimulationSettings_simulation(ref doc, simulationSettings, simulationSettings_nonSim);
        pStoreSimulationSettings_DOE(ref doc, simulationSettings);
        pStoreSimulationSettings_implant(ref doc, implantSimSettings, implantSettings_nonSim, implantSettings_, nonSimulationSettings_);

        bool savedOK = true;
        try
        {
            doc.Save(filename);
        }
        catch (Exception)
        {
            savedOK = false;
        }
        return savedOK;
    }

    public string loadSimulationSettings(string currentVersion, string filename, EntropySettings simulationSettings, EntropySettings_nonSim simulationSettings_nonSim, List<EntropyLayerSettings> listOfSettings, EntropySettings implantSimSettings, EntropySettings_nonSim implantSettings_nonSim, EntropyImplantSettings implantSettings_, NonSimulationSettings nonSimulationSettings_)
    {
        return pLoadSimulationSettings(currentVersion, filename, simulationSettings, simulationSettings_nonSim, implantSimSettings, implantSettings_nonSim, implantSettings_);
    }

    private void pLoadSimulationSettings_layer(string[] tokenVersion, XElement simulationFromFile)
    {
        for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
        {
            EntropyLayerSettings readSettings = new();
            // Tracking our loaded layers to avoid stomping over them.
            if (loadedLayers[layer])
            {
                continue;
            }

            string layerref = "layer" + (layer + 1);
            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.enabled, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("enabled").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.enabled);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.comment, simulationFromFile.Descendants(layerref).Descendants("comment").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.comment);
            }

            readSettings.defaultIntArray(EntropyLayerSettings.properties_intarray.bglayers);

            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 0, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer1BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 1, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer2BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 2, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer3BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 3, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer4BG").First().Value));
            }
            catch (Exception)
            {
                // default
            }
            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 4, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer5BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 5, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer6BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 6, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer7BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 7, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer8BG").First().Value));
            }
            catch (Exception)
            {

            }
            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 8, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer9BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 9, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer10BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 10, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer11BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 11, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer12BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 12, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer13BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 13, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer14BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 14, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer15BG").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.bglayers, 15, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("layer16BG").First().Value));
            }
            catch (Exception)
            {
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.showDrawn, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("showDrawn").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.showDrawn);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.flipH, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("flipH").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.flipH);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.flipV, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("flipV").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.flipV);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.alignX, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("alignGeom").First().Value));
                readSettings.setInt(EntropyLayerSettings.properties_i.alignY, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("alignGeom").First().Value));
            }
            catch (Exception)
            {
                try
                {
                    readSettings.setInt(EntropyLayerSettings.properties_i.alignX, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("alignGeomX").First().Value));
                }
                catch (Exception)
                {
                    readSettings.defaultInt(EntropyLayerSettings.properties_i.alignX);
                }

                try
                {
                    readSettings.setInt(EntropyLayerSettings.properties_i.alignY, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("alignGeomY").First().Value));
                }
                catch (Exception)
                {
                    readSettings.defaultInt(EntropyLayerSettings.properties_i.alignY);
                }
            }


            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.name, simulationFromFile.Descendants(layerref).Descendants("name").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.name);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.shapeIndex, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("shapeIndex").First().Value));

                // Version 1.9 broke the shape menu continuity as 'S' was inserted before the GDS.

                if (Convert.ToInt32(tokenVersion[0]) == 1 && Convert.ToInt32(tokenVersion[1]) < 9)
                {
                    if (readSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (int)CentralProperties.shapeNames.Sshape)
                    {
                        readSettings.setInt(EntropyLayerSettings.properties_i.shapeIndex, (int)CentralProperties.shapeNames.GEOCORE); //increment to map to the GDS setting in the 1.9 version.
                    }
                }
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.shapeIndex);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.horLength, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShapeHorLength").First().Value), 0);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.horLength, 0);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.horOffset, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShapeHorOffset").First().Value), 0);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.horOffset, 0);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.verLength, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShapeVerLength").First().Value), 0);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.verLength, 0);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.verOffset, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShapeVerOffset").First().Value), 0);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.verOffset, 0);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.shape0Tip, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("subShapeTipLocIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.shape0Tip);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.horLength, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShape2HorLength").First().Value), 1);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.horLength, 1);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.horOffset, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShape2HorOffset").First().Value), 1);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.horOffset, 1);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.verLength, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShape2VerLength").First().Value), 1);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.verLength, 1);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.verOffset, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShape2VerOffset").First().Value), 1);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.verOffset, 1);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.shape1Tip, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("subShape2TipLocIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.shape1Tip);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.horLength, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShape3HorLength").First().Value), 2);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.horLength, 2);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.horOffset, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShape3HorOffset").First().Value), 2);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.horOffset, 2);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.verLength, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShape3VerLength").First().Value), 2);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.verLength, 2);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.verOffset, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("subShape3VerOffset").First().Value), 2);
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.verOffset, 2);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.shape2Tip, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("subShape3TipLocIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.shape2Tip);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.subShapeIndex, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("subShapeRefIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.subShapeIndex);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.posIndex, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("posInSubShapeIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.posIndex);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.gHorOffset, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("globalHorOffset").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.gHorOffset);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.gVerOffset, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("globalVerOffset").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.gVerOffset);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.rot, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("rotation").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.rot);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.wobble, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("wobble").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.wobble);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.wobble_RNG, simulationFromFile.Descendants(layerref).Descendants("wobbleRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.wobble_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.sBias, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("sideBias").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.sBias);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.hTBias, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("horTipBias").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.hTBias);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.hTPVar, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("horTipBiasPVar").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.hTPVar);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.hTipPVar_RNG, simulationFromFile.Descendants(layerref).Descendants("horTipBiasPVarRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.hTipPVar_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.hTNVar, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("horTipBiasNVar").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.hTNVar);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.hTipNVar_RNG, simulationFromFile.Descendants(layerref).Descendants("horTipBiasNVarRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.hTipNVar_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.vTBias, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("verTipBias").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.vTBias);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.vTPVar, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("verTipBiasPVar").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.vTPVar);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.vTipPVar_RNG, simulationFromFile.Descendants(layerref).Descendants("verTipBiasPVarRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.vTipPVar_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.vTNVar, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("verTipBiasNVar").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.vTNVar);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.vTipNVar_RNG, simulationFromFile.Descendants(layerref).Descendants("verTipBiasNVarRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.vTipNVar_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.pBias, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("proximityBias").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.pBias);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.pBiasDist, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("proximityIsoDistance").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.pBiasDist);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.proxRays, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("proximitySideRays").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.proxRays);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.proxSideRaysFallOff, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("proximitySideRayFallOff").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.proxSideRaysFallOff);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.proxSideRaysMultiplier, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("proximitySideRayFallOffMultiplier").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.iCR);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.iCR, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("innerCRR").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.iCR);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.oCR, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("outerCRR").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.oCR);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.iCV, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("innerCV").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.iCV);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.iCV_RNG, simulationFromFile.Descendants(layerref).Descendants("innerCVRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.iCV_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.oCV, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("outerCV").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.oCV);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.oCV_RNG, simulationFromFile.Descendants(layerref).Descendants("outerCVRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.oCV_RNG);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.edgeSlide, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("edgeSlide").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.edgeSlide);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.eTension, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("edgeSlideTension").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.eTension);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.lwr, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("LWR").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.lwr);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.lwr_RNG, simulationFromFile.Descendants(layerref).Descendants("LWRRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.lwr_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.lwrFreq, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("LWRNoiseFreq").First().Value));
            }
            catch (Exception)
            {
                // Compatibility shim for old 3.x projects where setting name in XML was wrong.
                try
                {
                    readSettings.setDecimal(EntropyLayerSettings.properties_decimal.lwrFreq, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("LWRFreq").First().Value));
                }
                catch (Exception)
                {
                    readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.lwrFreq);
                }
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.lwrType, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("LWRNoiseType").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.lwrType);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.lwr_corr, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedLWR").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.lwr_corr);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.lwr_corr_ref, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedLWRLayerIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.lwr_corr_ref);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.lwr2, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("LWR2").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.lwr2);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.lwr2_RNG, simulationFromFile.Descendants(layerref).Descendants("LWR2RNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.lwr2_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("LWR2NoiseFreq").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.lwr2Type, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("LWR2NoiseType").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.lwr2Type);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.lwr2_corr, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedLWR2").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.lwr2_corr);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.lwr2_corr_ref, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedLWR2LayerIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.lwr2_corr_ref);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.lwrPreview, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("LWRNoisePreview").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.lwrPreview);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.sCDU, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("sideCDU").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.sCDU);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.sCDU_RNG, simulationFromFile.Descendants(layerref).Descendants("sideCDURNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.sCDU_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.tCDU, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("tipsCDU").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.tCDU);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.tCDU_RNG, simulationFromFile.Descendants(layerref).Descendants("tipsCDURNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.tCDU_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.xOL, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("horOverlay").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.xOL);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.xOL_RNG, simulationFromFile.Descendants(layerref).Descendants("horOverlayRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.xOL_RNG);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.yOL, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("verOverlay").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.yOL);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.yOL_RNG, simulationFromFile.Descendants(layerref).Descendants("verOverlayRNGMapping").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.yOL_RNG);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.xOL_corr, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedXOverlay").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.xOL_corr);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.xOL_corr_ref, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedXOverlayLayerIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.xOL_corr_ref);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.yOL_corr, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedYOverlay").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.yOL_corr);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedYOverlayLayerIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.yOL_corr_ref);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.CDU_corr, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedCDU").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.CDU_corr);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.CDU_corr_ref, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedCDULayerIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.CDU_corr_ref);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.tCDU_corr, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedTipCDU").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.tCDU_corr); // compatibility shim
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("correlatedTipCDULayerIndex").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.tCDU_corr_ref); // compatibility shim
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.xOL_ref, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.xOL_ref);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.yOL_ref, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.yOL_ref);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.xOL_av, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("averageOverlayX").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.xOL_av);
            }


            // We do this piecewise because 2.0 files have these entries, but only for four layers.
            // We want to capture the settings without clobbering them in a one-size-fits-all exception handler.
            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 0, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_1").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 1, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_2").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 2, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_3").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 3, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_4").First().Value));
            }
            catch (Exception)
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 0, 0);
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 1, 0);
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 2, 0);
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 3, 0);
            }

            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 4, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_5").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 5, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_6").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 6, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_7").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 7, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_8").First().Value));
            }
            catch (Exception)
            {
                for (int i = 4; i < 8; i++)
                {
                    readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, i, 0);
                }
            }

            // v3 added more layers so another piecemeal read.
            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 8, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_9").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 9, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_10").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 10, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_11").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 11, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_12").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 12, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_13").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 13, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_14").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 14, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_15").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, 15, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayXReferenceLayer_16").First().Value));
            }
            catch (Exception)
            {
                for (int i = 8; i < CentralProperties.maxLayersForMC; i++)
                {
                    readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, i, 0);
                }
            }
            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.yOL_av, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("averageOverlayY").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.yOL_av);
            }

            // We do this piecewise because 2.0 files have these entries, but only for four layers.
            // We want to capture the settings without clobbering them in a one-size-fits-all exception handler.
            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 0, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_1").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 1, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_2").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 2, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_3").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 3, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_4").First().Value));
            }
            catch (Exception)
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 0, 0);
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 1, 0);
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 2, 0);
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 3, 0);
            }


            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 4, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_5").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 5, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_6").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 6, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_7").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 7, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_8").First().Value));
            }
            catch (Exception)
            {
                for (int i = 4; i < 8; i++)
                {
                    readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, i, 0);
                }
            }

            try
            {
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 8, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_9").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 9, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_10").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 10, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_11").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 11, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_12").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 12, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_13").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 13, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_14").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 14, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_15").First().Value));
                readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, 15, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("overlayYReferenceLayer_16").First().Value));
            }
            catch (Exception)
            {
                for (int i = 8; i < CentralProperties.maxLayersForMC; i++)
                {
                    readSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, i, 0);
                }
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.lDC1, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("lensDistortionCoeff1").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.lDC1);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.lDC2, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("lensDistortionCoeff2").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.lDC2);
            }

            double x = 0;
            double y = 0;
            double zoom = 1;
            try
            {
                x = Convert.ToDouble(simulationFromFile.Descendants(layerref).Descendants("viewportX").First().Value);
                y = Convert.ToDouble(simulationFromFile.Descendants(layerref).Descendants("viewportY").First().Value);
                zoom = Convert.ToDouble(simulationFromFile.Descendants(layerref).Descendants("displayZoomFactor").First().Value);
            }
            catch (Exception)
            {
            }
            viewportLoad?.Invoke(layer, new[] { x, y, zoom });

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.bLayerA, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("booleanLayerA").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.bLayerA);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.bLayerB, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("booleanLayerB").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.bLayerB);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.bLayerOpA, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("booleanLayerOpA").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.bLayerOpA);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.bLayerOpB, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("booleanLayerOpB").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.bLayerOpB);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.bLayerOpAB, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("booleanLayerOpAB").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.bLayerOpAB);
            }

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.rayExtension, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("rayExtension").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.rayExtension);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.omit, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("omitLayer").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.omit);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.file, simulationFromFile.Descendants(layerref).Descendants("fileToLoad").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.file);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.refLayout, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("geoCoreReferenceLayout").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.refLayout);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.fileType, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("fileType").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.fileType);
            }

            bool loadLayoutOK = false;
            if (readSettings.getInt(EntropyLayerSettings.properties_i.refLayout) == 1)
            {
                // Need to read in our layout file.
                try
                {
                    loadLayoutOK = loadLayout(layer, readSettings.getString(EntropyLayerSettings.properties_s.file));
                }
                catch (Exception)
                {
                    loadLayoutOK = false;
                }
                if (!loadLayoutOK)
                {
                    // Load error in some form.
                    // Error path is unclear.
                    ErrorReporter.showMessage_OK("External file read error. Using project geometry.", readSettings.getString(EntropyLayerSettings.properties_s.file));
                }
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.structure, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("structureFromFile").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.structure);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.structure, simulationFromFile.Descendants(layerref).Descendants("structureNameFromFile").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.structure);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.lD, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("ldFromFile").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.lD);
            }

            try
            {
                readSettings.setString(EntropyLayerSettings.properties_s.lD, simulationFromFile.Descendants(layerref).Descendants("ldNameFromFile").First().Value);
            }
            catch (Exception)
            {
                readSettings.defaultString(EntropyLayerSettings.properties_s.lD);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.fill, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("polyFill").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.fill);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.gCSEngine, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("geoCoreShapeEngine").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.gCSEngine);
            }

            try
            {
                readSettings.setInt(EntropyLayerSettings.properties_i.perPoly, Convert.ToInt32(simulationFromFile.Descendants(layerref).Descendants("geoCoreShapeEnginePerPoly").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultInt(EntropyLayerSettings.properties_i.perPoly);
            }

            readSettings.setReloaded(true);

            try
            {
                readSettings.setDecimal(EntropyLayerSettings.properties_decimal.keyhole_factor, Convert.ToDecimal(simulationFromFile.Descendants(layerref).Descendants("geoCoreKeyholeSizing").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultDecimal(EntropyLayerSettings.properties_decimal.keyhole_factor);
            }

            try
            {
                readSettings.setFileData(fileDataFromString(simulationFromFile.Descendants(layerref).Descendants("fileData").First().Value));
            }
            catch (Exception)
            {
                readSettings.defaultFileData();
            }

            bool updateGeoCoreGeometryFromFile = false;
            if (readSettings.getInt(EntropyLayerSettings.properties_i.refLayout) == 1)
            {
                // Update the internal layout based on the loaded configuration.
                if (loadLayoutOK)
                {
                    updateGeoCoreGeometryFromFile = true;
                }
                else
                {
                    // Reset the reference layout because user saving this project later will break things as we have reset the LD and structure indices.
                    readSettings.defaultInt(EntropyLayerSettings.properties_i.refLayout);
                    readSettings.defaultInt(EntropyLayerSettings.properties_i.lD);
                    readSettings.defaultInt(EntropyLayerSettings.properties_i.structure);
                }
            }

            // Compatibility patch for pre-3.0 project files, due to the introduction of more layers.
            // Layers 3,4 need to be mapped to 5,6 in the new system
            int compatibilityLayerOffset = 0; // to bump layer 3 to layer 5; layer 4 to layer 6.
            switch (Convert.ToInt32(tokenVersion[0]))
            {
                case 1:
                case 2 when Convert.ToInt32(tokenVersion[1]) < 1:
                    compatibilityLayerOffset = 6;
                    break;
                case 2:
                    compatibilityLayerOffset = 4;
                    break;
            }
            if (compatibilityLayerOffset != 0)
            {
                modifyLayerRefs_forLayerChange(ref readSettings, compatibilityLayerOffset);
                if (compatibilityLayerOffset == 6 && layer is 2 or 3 ||
                    compatibilityLayerOffset == 4 && layer is 4 or 5 or 6 or 7)
                {
                    // Full layer remap
                    setLayerSettings?.Invoke(readSettings, layer + compatibilityLayerOffset, false, true);
                    double[] camDetails = viewportSave?.Invoke(layer);
                    viewportLoad?.Invoke(layer + compatibilityLayerOffset, camDetails);
                    viewportLoad?.Invoke(layer, new double[] { 0, 0, 1 });
                    loadedLayers[layer + compatibilityLayerOffset] = true;
                    readSettings = new EntropyLayerSettings();
                }
            }
            setLayerSettings?.Invoke(readSettings, layer, false, updateGeoCoreGeometryFromFile); // avoid resuming the UI
            loadedLayers[layer] = true;
        }
    }

    private bool pLoadSimulationSettings_simulation(string[] tokenVersion, XElement simulationFromFile, ref EntropySettings simulationSettings, ref EntropySettings_nonSim simulationSettings_nonSim)
    {
        bool error = false;
        try
        {
            suspendSettingsUI?.Invoke();

            try
            {
                simulationSettings_nonSim.setString(EntropySettings_nonSim.properties_s.comment, simulationFromFile.Descendants("settings").Descendants("comment").First().Value);
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultString(EntropySettings_nonSim.properties_s.comment);
            }

            try
            {
                simulationSettings_nonSim.setString(EntropySettings_nonSim.properties_s.paComment, simulationFromFile.Descendants("settings").Descendants("paSearchComment").First().Value);
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultString(EntropySettings_nonSim.properties_s.paComment);
            }

            try
            {
                simulationSettings.setValue(EntropySettings.properties_i.nCases, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("numberOfCases").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultValue(EntropySettings.properties_i.nCases);
            }

            try
            {
                simulationSettings.setResolution(Convert.ToDouble(simulationFromFile.Descendants("settings").Descendants("resolution").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultResolution();
            }

            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.results, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("displayResults").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.results);
            }

            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.shape, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("displayShape").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.shape);
            }

            double x = 0;
            double y = 0;
            double zoom = 1;
            try
            {
                x = Convert.ToDouble(simulationFromFile.Descendants("settings").Descendants("viewportX").First().Value);
                y = Convert.ToDouble(simulationFromFile.Descendants("settings").Descendants("viewportY").First().Value);
                zoom = Convert.ToDouble(simulationFromFile.Descendants("settings").Descendants("displayZoomFactor").First().Value);
            }
            catch (Exception)
            {
            }
            viewportLoad?.Invoke(CentralProperties.maxLayersForMC, new[] { x, y, zoom });

            int svgCompat = 0;
            try
            {
                svgCompat = Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("generateSVG").First().Value);
            }
            catch (Exception)
            {
            }

            if (svgCompat == 1)
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.external, 1);
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalType, (int)CommonVars.external_Type.svg);
            }
            else
            {
                try
                {
                    simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.external, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("generateExternal").First().Value));
                }
                catch (Exception)
                {
                    simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.external);
                }
                try
                {
                    simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalType, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalType").First().Value));
                }
                catch (Exception)
                {
                    simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.externalType);
                }
            }
            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.csv, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("generateCSV").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.csv);
            }

            try
            {
                simulationSettings.setValue(EntropySettings.properties_i.linkCDU, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("linkTipandSideCDU").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultValue(EntropySettings.properties_i.linkCDU);
            }

            try
            {
                simulationSettings.setValue(EntropySettings.properties_i.subMode, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("subMode").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultValue(EntropySettings.properties_i.subMode);
            }

            try
            {
                simulationSettings.setValue(EntropySettings.properties_i.cSeg, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("cornerSegments").First().Value));
            }
            catch (Exception)
            {
                try
                {
                    // Compatibility handling for old projects.
                    double angleStep = Convert.ToDouble(simulationFromFile.Descendants("settings").Descendants("angleStep").First().Value);
                    simulationSettings.setValue(EntropySettings.properties_i.cSeg, (int)(90.0f / angleStep));
                }
                catch (Exception)
                {
                    simulationSettings.defaultValue(EntropySettings.properties_i.cSeg);
                }
            }

            try
            {
                simulationSettings.setValue(EntropySettings.properties_i.optC, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("optimizeCorners").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultValue(EntropySettings.properties_i.optC);
            }

            try
            {
                simulationSettings.setValue(EntropySettings.properties_i.ler, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("lerFromLWR_by_sqrt2").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultValue(EntropySettings.properties_i.ler);
            }

            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.greedy, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("greedyMode").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.greedy);
            }

            try
            {
                simulationSettings.setValue(EntropySettings.properties_i.rngType, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("rng").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultValue(EntropySettings.properties_i.rngType);
            }

            try
            {
                simulationSettings.setValue(EntropySettings.properties_i.oType, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("outputType").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultValue(EntropySettings.properties_i.oType);
            }

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                string ts = "layer" + (i + 1) + "Operator";
                try
                {
                    simulationSettings.setOperatorValue(EntropySettings.properties_o.layer, i, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants(ts).First().Value));
                }
                catch (Exception)
                {
                    simulationSettings.defaultOperator(EntropySettings.properties_o.layer, i);
                }
            }

            for (int i = 0; i < simulationSettings.getOperator(EntropySettings.properties_o.twoLayer).Length; i++)
            {
                string ts = "layer" + (i * 2 + 1) + "_" + (i + 1) * 2 + "Operator";
                try
                {
                    simulationSettings.setOperatorValue(EntropySettings.properties_o.twoLayer, i, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants(ts).First().Value));
                }
                catch (Exception)
                {
                    simulationSettings.defaultOperator(EntropySettings.properties_o.twoLayer, i);
                }
            }

            for (int i = 0; i < simulationSettings.getOperator(EntropySettings.properties_o.fourLayer).Length; i++)
            {
                int v = i * simulationSettings.getOperator(EntropySettings.properties_o.fourLayer).Length + 1;
                string ts = "layer" + v + (v + 1) + "_" + (v + 2) + (v + 3) + "Operator";
                try
                {
                    simulationSettings.setOperatorValue(EntropySettings.properties_o.fourLayer, i, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants(ts).First().Value));
                }
                catch (Exception)
                {
                    simulationSettings.defaultOperator(EntropySettings.properties_o.fourLayer, i);
                }
            }

            try
            {
                simulationSettings.setOperatorValue(EntropySettings.properties_o.eightLayer, 0, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("layer1234_5678Operator").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultOperator(EntropySettings.properties_o.eightLayer, 0);
            }

            try
            {
                simulationSettings.setOperatorValue(EntropySettings.properties_o.eightLayer, 1, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("layer9101112_13141516Operator").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.defaultOperator(EntropySettings.properties_o.eightLayer, 1);
            }

            switch (Convert.ToInt32(tokenVersion[0]))
            {
                // Compatibility patch for pre-3.0 project files, due to the introduction of more layers.
                case 1:
                case 2 when Convert.ToInt32(tokenVersion[1]) < 1:
                    simulationSettings.setOperatorValue(EntropySettings.properties_o.layer, 8, simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 2));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.layer, 2);

                    simulationSettings.setOperatorValue(EntropySettings.properties_o.layer, 9, simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 3));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.layer, 3);

                    simulationSettings.setOperatorValue(EntropySettings.properties_o.twoLayer, 4, simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 1));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.twoLayer, 1);
                    break;
                case 2:
                    simulationSettings.setOperatorValue(EntropySettings.properties_o.layer, 8, simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 4));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.layer, 4);

                    simulationSettings.setOperatorValue(EntropySettings.properties_o.layer, 9, simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 5));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.layer, 5);

                    simulationSettings.setOperatorValue(EntropySettings.properties_o.twoLayer, 4, simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 2));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.twoLayer, 2);

                    simulationSettings.setOperatorValue(EntropySettings.properties_o.layer, 10, simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 6));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.layer, 6);

                    simulationSettings.setOperatorValue(EntropySettings.properties_o.layer, 11, simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 7));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.layer, 7);

                    simulationSettings.setOperatorValue(EntropySettings.properties_o.twoLayer, 5, simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 3));
                    simulationSettings.defaultOperator(EntropySettings.properties_o.twoLayer, 3);
                    break;
            }

            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalCriteria, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCriteria").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.externalCriteria);
            }

            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalCritCond1, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCritCond1").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.externalCritCond1);
            }

            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalCritCond2, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCritCond2").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.externalCritCond2);
            }

            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalCritCond3, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCritCond3").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.externalCritCond3);
            }

            try
            {
                simulationSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalCritCond4, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCritCond4").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.externalCritCond4);
            }

            try
            {
                simulationSettings_nonSim.setDecimal(EntropySettings_nonSim.properties_d.externalCritCond1, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCritValue1").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultDecimal(EntropySettings_nonSim.properties_d.externalCritCond1);
            }

            try
            {
                simulationSettings_nonSim.setDecimal(EntropySettings_nonSim.properties_d.externalCritCond2, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCritValue2").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultDecimal(EntropySettings_nonSim.properties_d.externalCritCond2);
            }

            try
            {
                simulationSettings_nonSim.setDecimal(EntropySettings_nonSim.properties_d.externalCritCond3, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCritValue3").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultDecimal(EntropySettings_nonSim.properties_d.externalCritCond3);
            }

            try
            {
                simulationSettings_nonSim.setDecimal(EntropySettings_nonSim.properties_d.externalCritCond4, Convert.ToInt32(simulationFromFile.Descendants("settings").Descendants("externalCritValue4").First().Value));
            }
            catch (Exception)
            {
                simulationSettings_nonSim.defaultDecimal(EntropySettings_nonSim.properties_d.externalCritCond4);
            }

            updateSettingsUIFromSettings?.Invoke();
        }
        catch (Exception)
        {
            ErrorReporter.showMessage_OK("Failed in settings loading", "Error");
            error = true;
        }

        return error;
    }

    private bool pLoadSimulationSettings_DOE(string[] tokenVersion, XElement simulationFromFile, ref EntropySettings simulationSettings)
    {
        bool error = false;

        try
        {
            string doeLabel = "DOESettings";
            if (tokenVersion[0] == "1") // backward compatibility
            {
                doeLabel = "gdsDOESettings";
            }

            suspendDOESettingsUI?.Invoke();
            try
            {
                simulationSettings.getDOESettings().setString(DOESettings.properties_s.comment, simulationFromFile.Descendants(doeLabel).Descendants("comment").First().Value);
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setString(DOESettings.properties_s.comment, DOESettings.default_comment);
            }

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                string t = "layer" + (i + 1) + "Affected";
                try
                {
                    simulationSettings.getDOESettings().setLayerAffected(layer:i, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants(t).First().Value));
                }
                catch (Exception)
                {
                    simulationSettings.getDOESettings().setLayerAffected(layer:i, 0);
                }
            }

            switch (Convert.ToInt32(tokenVersion[0]))
            {
                // Compatibility patch for pre-3.0 project files, due to the introduction of more layers.
                case 1:
                case 2 when Convert.ToInt32(tokenVersion[1]) < 1:
                    simulationSettings.getDOESettings().setLayerAffected(layer: 8, simulationSettings.getDOESettings().getLayerAffected(2));
                    simulationSettings.getDOESettings().setLayerAffected(layer: 2, 0);
                    simulationSettings.getDOESettings().setLayerAffected(layer: 9, simulationSettings.getDOESettings().getLayerAffected(3));
                    simulationSettings.getDOESettings().setLayerAffected(layer: 3, 0);
                    break;
                case 2:
                    simulationSettings.getDOESettings().setLayerAffected(layer: 8, simulationSettings.getDOESettings().getLayerAffected(4));
                    simulationSettings.getDOESettings().setLayerAffected(layer: 4, 0);
                    simulationSettings.getDOESettings().setLayerAffected(layer: 9, simulationSettings.getDOESettings().getLayerAffected(5));
                    simulationSettings.getDOESettings().setLayerAffected(layer: 5, 0);
                    simulationSettings.getDOESettings().setLayerAffected(layer: 10, simulationSettings.getDOESettings().getLayerAffected(6));
                    simulationSettings.getDOESettings().setLayerAffected(layer: 6, 0);
                    simulationSettings.getDOESettings().setLayerAffected(layer: 11, simulationSettings.getDOESettings().getLayerAffected(7));
                    simulationSettings.getDOESettings().setLayerAffected(layer: 7, 0);
                    break;
            }

            try
            {
                simulationSettings.getDOESettings().setDouble(DOESettings.properties_d.rowPitch, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("rowPitch").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setDouble(DOESettings.properties_d.rowPitch, DOESettings.default_rowPitch);
            }

            try
            {
                simulationSettings.getDOESettings().setDouble(DOESettings.properties_d.colPitch, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("colPitch").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setDouble(DOESettings.properties_d.colPitch, DOESettings.default_colPitch);
            }

            try
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.rows, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("rows").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.rows, DOESettings.default_rows);
            }

            try
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.cols, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("cols").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.cols, DOESettings.default_cols);
            }

            try
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.sTile, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("specificTile").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.sTile, DOESettings.default_specificTile);
            }

            try
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.sTileRow, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("specificTile_Row").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.sTileRow, DOESettings.default_specificTile_Row);
            }

            try
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.sTileCol, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("specificTile_Col").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.sTileCol, DOESettings.default_specificTile_Col);
            }

            try
            {
                simulationSettings.getDOESettings().setDouble(DOESettings.properties_d.colOffset, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("colOffset").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setDouble(DOESettings.properties_d.colOffset, DOESettings.default_colOffset);
            }

            try
            {
                simulationSettings.getDOESettings().setDouble(DOESettings.properties_d.rowOffset, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("rowOffset").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setDouble(DOESettings.properties_d.rowOffset, DOESettings.default_rowOffset);
            }

            try
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.uTileList, Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("listOfTiles").First().Value));
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setInt(DOESettings.properties_i.uTileList, DOESettings.default_listOfTiles);
            }

            try
            {
                simulationSettings.getDOESettings().setString(DOESettings.properties_s.list, simulationFromFile.Descendants(doeLabel).Descendants("tileList_ColRow").First().Value);
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setString(DOESettings.properties_s.list, DOESettings.default_trList);
            }

            simulationSettings.setTileList(simulationSettings.getDOESettings().getString(DOESettings.properties_s.list), true);
            try
            {
                if (Convert.ToInt32(simulationFromFile.Descendants(doeLabel).Descendants("iDRMConfigured").First().Value) == 1)
                {
                    simulationSettings.getDOESettings().setBool(DOESettings.properties_b.iDRM, true);
                    simulationSettings.getDOESettings().setInt(DOESettings.properties_i.uTileList, 1);
                }
                else
                {
                    simulationSettings.getDOESettings().setBool(DOESettings.properties_b.iDRM, false);
                }
            }
            catch (Exception)
            {
                simulationSettings.getDOESettings().setBool(DOESettings.properties_b.iDRM, false);
            }

            double x = 0;
            double y = 0;
            double zoom = 1;
            try
            {
                x = Convert.ToDouble(simulationFromFile.Descendants(doeLabel).Descendants("viewportX").First().Value);
                y = Convert.ToDouble(simulationFromFile.Descendants(doeLabel).Descendants("viewportY").First().Value);
                zoom = Convert.ToDouble(simulationFromFile.Descendants(doeLabel).Descendants("displayZoomFactor").First().Value);
            }
            catch (Exception)
            {
            }
            viewportLoad?.Invoke(CentralProperties.maxLayersForMC + 1, new[] { x, y, zoom });

            updateDOESettingsUIFromSettings?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorReporter.showMessage_OK("Failed in DOE loading: " + ex, "Error");
            error = true;
        }

        return error;
    }

    private bool pLoadSimulationSettings_implant(XElement simulationFromFile, ref EntropySettings implantSimSettings, ref EntropySettings_nonSim implantSettings_nonSim, ref EntropyImplantSettings implantSettings_)
    {
        bool error = false;

        try
        {
            const string implantLabel = "implant";

            suspendImplantUI?.Invoke();
            try
            {
                implantSettings_.setComment(simulationFromFile.Descendants(implantLabel).Descendants("comment").First().Value);
            }
            catch (Exception)
            {
                implantSettings_.setComment("");
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.cRR, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("resistCRR").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.cRR);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.cV, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("resistCRRVar").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.cV);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.h, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("resistHeight_postDevelop").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.h);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.hV, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("resistHeight_postDevelopVar").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.hV);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.w, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("resistWidth").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.w);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.wV, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("resistWidthVar").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.wV);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.tilt, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("tiltAngle").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.tilt);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.tiltV, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("tiltAngleVar").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.tiltV);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.twist, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("twistAngle").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.twist);
            }
            try
            {
                implantSettings_.setDouble(EntropyImplantSettings.properties_d.twistV, Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("twistAngleVar").First().Value));
            }
            catch (Exception)
            {
                implantSettings_.defaultDouble(EntropyImplantSettings.properties_d.twistV);
            }
            try
            {
                implantSimSettings.setValue(EntropySettings.properties_i.nCases, Convert.ToInt32(simulationFromFile.Descendants(implantLabel).Descendants("numberOfCases").First().Value));
            }
            catch (Exception)
            {
                implantSimSettings.defaultValue(EntropySettings.properties_i.nCases);
            }
            try
            {
                implantSimSettings.setValue(EntropySettings.properties_i.cSeg, Convert.ToInt32(simulationFromFile.Descendants(implantLabel).Descendants("cornerSegments").First().Value));
            }
            catch (Exception)
            {
                implantSimSettings.defaultValue(EntropySettings.properties_i.cSeg);
            }
            try
            {
                implantSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.csv, Convert.ToInt32(simulationFromFile.Descendants(implantLabel).Descendants("generateCSV").First().Value));
            }
            catch (Exception)
            {
                implantSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.csv);
            }

            int svgCompat = 0;
            try
            {
                svgCompat = Convert.ToInt32(simulationFromFile.Descendants(implantLabel).Descendants("generateSVG").First().Value);
            }
            catch (Exception)
            {
            }

            if (svgCompat == 1)
            {
                implantSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.external, 1);
                implantSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalType, (int)CommonVars.external_Type.svg);
            }
            else
            {
                try
                {
                    implantSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.external, Convert.ToInt32(simulationFromFile.Descendants(implantLabel).Descendants("generateExternal").First().Value));
                }
                catch (Exception)
                {
                    implantSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.external);
                }
                try
                {
                    implantSettings_nonSim.setInt(EntropySettings_nonSim.properties_i.externalType, Convert.ToInt32(simulationFromFile.Descendants(implantLabel).Descendants("externalTyoe").First().Value));
                }
                catch (Exception)
                {
                    implantSettings_nonSim.defaultInt(EntropySettings_nonSim.properties_i.externalType);
                }
            }

            try
            {
                implantSimSettings.setValue(EntropySettings.properties_i.rngType, Convert.ToInt32(simulationFromFile.Descendants(implantLabel).Descendants("rngType").First().Value));
            }
            catch (Exception)
            {
                implantSimSettings.defaultValue(EntropySettings.properties_i.rngType);
            }

            double x = 0;
            double y = 0;
            double zoom = 1;
            try
            {
                x = Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("viewportX").First().Value);
                y = Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("viewportY").First().Value);
                zoom = Convert.ToDouble(simulationFromFile.Descendants(implantLabel).Descendants("displayZoomFactor").First().Value);
            }
            catch (Exception)
            {
            }
            implantViewportLoad?.Invoke(new[] { x, y, zoom });

            updateImplantUIFromSettings?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorReporter.showMessage_OK("Failed in implant loading: " + ex, "Error");
            error = true;
        }

        return error;
    }
    
    private string pLoadSimulationSettings(string currentVersion, string filename, EntropySettings simulationSettings, EntropySettings_nonSim simulationSettings_nonSim, EntropySettings implantSimSettings, EntropySettings_nonSim implantSettings_nonSim, EntropyImplantSettings implantSettings_)
    {
        loadedLayers = new bool[CentralProperties.maxLayersForMC];
        string returnString = "";
        bool error = false;
        XElement simulationFromFile;
        try
        {
            simulationFromFile = XElement.Load(filename);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        // root is required for legacy projects.
        if (simulationFromFile.Name != "Variance" && simulationFromFile.Name != "root")
        {
            return "This is not a " + CentralProperties.productName + " project file.";
        }
        
        string version = simulationFromFile.Descendants("version").First().Value;
        string[] tokenVersion = version.Split(new[] { '.' });

        if (version != currentVersion)
        {
            ErrorReporter.showMessage_OK("Settings file for version " + version, "Legacy import");
        }

        prepUI?.Invoke();
        
        pLoadSimulationSettings_layer(tokenVersion, simulationFromFile);
        error = pLoadSimulationSettings_simulation(tokenVersion, simulationFromFile, ref simulationSettings, ref simulationSettings_nonSim);
        if (!error)
        {
            error = pLoadSimulationSettings_DOE(tokenVersion, simulationFromFile, ref simulationSettings);
        }

        if (!error)
        {
            error = pLoadSimulationSettings_implant(simulationFromFile, ref implantSimSettings,
                ref implantSettings_nonSim, ref implantSettings_);
        }

        resumeUI?.Invoke();

        if (error)
        {
            returnString = "an error occurred";
        }
        return returnString;
    }
}