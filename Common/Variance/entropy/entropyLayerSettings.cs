using System;
using System.Linq;
using Clipper2Lib;
using shapeEngine;

namespace Variance;

[Serializable]
public class EntropyLayerSettings : ShapeSettings
{
    private static string default_comment = "";
    private static int[] default_bgLayers = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private static int default_showDrawn = 0;

    private static string default_rngmapping = CommonVars.boxMuller;

    private static decimal default_wobble = 0;
    private static decimal default_innerCV = 0;
    private static decimal default_outerCV = 0;
    private static int default_correlatedLWR = 0;
    private static int default_correlatedLWRLayerIndex = -1;
    private static decimal default_sideCDU = 0;
    private static decimal default_tipsCDU = 0;
    private static decimal default_horOverlay = 0;
    private static decimal default_verOverlay = 0;

    private static int default_correlatedTipCDU = 0;
    private static int default_correlatedTipCDULayerIndex = -1;
    private static int default_correlatedCDU = 0;
    private static int default_correlatedCDULayerIndex = -1;
    private static int default_correlatedXOverlay = 0;
    private static int default_correlatedXOverlayLayerIndex = -1;
    private static int default_correlatedYOverlay = 0;
    private static int default_correlatedYOverlayLayerIndex = -1;
    private static int default_overlayXReferenceLayer = -1;
    private static int default_overlayYReferenceLayer = -1;
    private static int default_averageOverlayX = 0;
    private static int default_averageOverlayY = 0;
    private static int[] default_overlayXReferenceLayers = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private static int[] default_overlayYReferenceLayers = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    private static int default_fileType = 0; // layout (now exposed by geoCore - could be redundant)
    private static PathsD default_fileData = new() { new () { new (0, 0) } };
    private static string default_fileToLoad = "";
    private static string default_ldNameFromFile = "";
    private static string default_structureNameFromFile = "";
    private static int default_ldFromFile = 0;
    private static int default_polyFill = (int)PolyFill.pftNonZero; // non-zero
    private static int default_structureFromFile = 0;
    private static int default_perPoly = 0;
    private static int default_referenceLayout = 0;

    private static decimal default_lensDistortionCoeff1 = 0;
    private static decimal default_lensDistortionCoeff2 = 0;

    private static int default_booleanLayerA = -1;
    private static int default_booleanLayerB = -1;
    private static int default_booleanLayerOpA = 0;
    private static int default_booleanLayerOpB = 0;
    private static int default_booleanLayerOpAB = 0;

    private static int default_omitLayer = 0;

    private static int default_LWRNoisePreview = 0;

    private static int default_removeArtifacts = 0;
    private static int default_removeArtifactsEpsilon = 100;
    
    [NonSerialized] private int[] bgLayers;

    private int[] overlayXReferenceLayers;
    private int[] overlayYReferenceLayers;

    public enum properties_intarray
    {
        bglayers, xOLRefs, yOLRefs
    }

    public bool isLayerActive()
    {
        return pIsLayerActive();
    }

    private bool pIsLayerActive()
    {
        return getInt(ShapeSettings.properties_i.enabled) == 1 && omitFromSim == 0;
    }

    public int[] getIntArray(properties_intarray p)
    {
        return pGetIntArray(p);
    }

    private int[] pGetIntArray(properties_intarray p)
    {
        int[] ret = { };
        switch (p)
        {
            case properties_intarray.bglayers:
                ret = bgLayers;
                break;
            case properties_intarray.xOLRefs:
                ret = overlayXReferenceLayers;
                break;
            case properties_intarray.yOLRefs:
                ret = overlayYReferenceLayers;
                break;
        }

        return ret;
    }

    public void setIntArray(properties_intarray p, int[] val)
    {
        pSetIntArray(p, val);
    }

    private void pSetIntArray(properties_intarray p, int[] val)
    {
        switch (p)
        {
            case properties_intarray.bglayers:
                bgLayers = val;
                break;
            case properties_intarray.xOLRefs:
                overlayXReferenceLayers = val;
                break;
            case properties_intarray.yOLRefs:
                overlayYReferenceLayers = val;
                break;
        }
    }

    public void defaultIntArray(properties_intarray p)
    {
        pDefaultIntArray(p);
    }

    private void pDefaultIntArray(properties_intarray p)
    {
        switch (p)
        {
            case properties_intarray.bglayers:
                bgLayers = default_bgLayers.ToArray();
                break;
            case properties_intarray.xOLRefs:
                overlayXReferenceLayers = default_overlayXReferenceLayers.ToArray();
                break;
            case properties_intarray.yOLRefs:
                overlayYReferenceLayers = default_overlayYReferenceLayers.ToArray();
                break;
        }
    }

    public int getIntArrayValue(properties_intarray p, int index)
    {
        return pGetIntArrayValue(p, index);
    }

    private int pGetIntArrayValue(properties_intarray p, int index)
    {
        int ret = 0;
        switch (p)
        {
            case properties_intarray.bglayers:
                ret = bgLayers[index];
                break;
            case properties_intarray.xOLRefs:
                ret = overlayXReferenceLayers[index];
                break;
            case properties_intarray.yOLRefs:
                ret = overlayYReferenceLayers[index];
                break;
        }

        return ret;
    }

    public void setIntArrayValue(properties_intarray p, int index, int val)
    {
        pSetIntArrayValue(p, index, val);
    }

    private void pSetIntArrayValue(properties_intarray p, int index, int val)
    {
        switch (p)
        {
            case properties_intarray.bglayers:
                bgLayers[index] = val;
                break;
            case properties_intarray.xOLRefs:
                overlayXReferenceLayers[index] = val;
                break;
            case properties_intarray.yOLRefs:
                overlayYReferenceLayers[index] = val;
                break;
        }
    }

    private int omitFromSim; // prevent layer being included in simulation.
    
    [NonSerialized] private int showDrawn;
    private int correlatedLWR;
    private int correlatedLWRLayerIndex;
    private int correlatedLWR2;
    private int correlatedLWR2LayerIndex;

    [NonSerialized] private int LWRNoisePreview;

    private int correlatedXOverlay;
    private int correlatedXOverlayLayerIndex;
    private int correlatedYOverlay;
    private int correlatedYOverlayLayerIndex;
    private int correlatedCDU;
    private int correlatedCDULayerIndex;
    private int correlatedTipCDU;
    private int correlatedTipCDULayerIndex;
    private int overlayXReferenceLayer;
    private int overlayYReferenceLayer;
    private int averageOverlayX;
    private int averageOverlayY;
    private int structureFromFile;
    private int ldFromFile;
    private int polyFill;
    private int fileType; // holds the value of the file type selected (0 == GDS, 1 == Oasis). Might be redundant with geoCore.
    private int perPoly;
    private int referenceLayout;
    private int booleanLayerA;
    private int booleanLayerB;
    private int booleanLayerOpA;
    private int booleanLayerOpB;
    private int booleanLayerOpAB;
    private int removeArtifacts;
    private int removeArtifactsEpsilon;
    
    public new enum properties_i
    {
        enabled, omit, gCSEngine, showDrawn, shapeIndex, shape0Tip, shape1Tip, shape2Tip, subShapeIndex, posIndex, proxRays, proxSideRaysFallOff, edgeSlide, 
        lwrType, lwr2Type, lwrPreview, lwr_corr, lwr_corr_ref, lwr2_corr, lwr2_corr_ref,
        xOL_corr, xOL_corr_ref, yOL_corr, yOL_corr_ref, CDU_corr, CDU_corr_ref, tCDU_corr, tCDU_corr_ref,
        xOL_ref, yOL_ref, xOL_av, yOL_av,
        flipH, flipV, alignX, alignY, structure, lD, fill, fileType, perPoly, refLayout,
        bLayerA, bLayerB, bLayerOpA, bLayerOpB, bLayerOpAB,
        removeArtifacts, removeArtifactsEpsilon
    }

    public int getInt(properties_i p)
    {
        return pGetInt(p);
    }

    private int pGetInt(properties_i p)
    {
        int ret = 0;
        switch (p)
        {
            case properties_i.enabled:
                ret = getInt(ShapeSettings.properties_i.enabled);
                break;
            case properties_i.shapeIndex:
                ret = getInt(ShapeSettings.properties_i.shapeIndex);
                break;
            case properties_i.gCSEngine:
                ret = getInt(ShapeSettings.properties_i.gCSEngine);
                break;
            case properties_i.shape0Tip:
                ret = getInt(ShapeSettings.properties_i.subShapeTipLocIndex);
                break;
            case properties_i.shape1Tip:
                ret = getInt(ShapeSettings.properties_i.subShape2TipLocIndex);
                break;
            case properties_i.shape2Tip:
                ret = getInt(ShapeSettings.properties_i.subShape3TipLocIndex);
                break;
            case properties_i.subShapeIndex:
                ret = getInt(ShapeSettings.properties_i.subShapeRefIndex);
                break;
            case properties_i.posIndex:
                ret = getInt(ShapeSettings.properties_i.posInSubShapeIndex);
                break;
            case properties_i.edgeSlide:
                ret = getInt(ShapeSettings.properties_i.edgeSlide);
                break;
            case properties_i.proxRays:
                ret = getInt(ShapeSettings.properties_i.proxRays);
                break;
            case properties_i.proxSideRaysFallOff:
                ret = getInt(ShapeSettings.properties_i.proxSideRaysFallOff);
                break;
            case properties_i.lwrType:
                ret = getInt(ShapeSettings.properties_i.lwrType);
                break;
            case properties_i.lwr2Type:
                ret = getInt(ShapeSettings.properties_i.lwr2Type);
                break;
            case properties_i.flipH:
                ret = getInt(ShapeSettings.properties_i.flipH);
                break;
            case properties_i.flipV:
                ret = getInt(ShapeSettings.properties_i.flipV);
                break;
            case properties_i.alignX:
                ret = getInt(ShapeSettings.properties_i.alignX);
                break;
            case properties_i.alignY:
                ret = getInt(ShapeSettings.properties_i.alignY);
                break;
            case properties_i.omit:
                ret = omitFromSim;
                break;
            case properties_i.showDrawn:
                ret = showDrawn;
                break;
            case properties_i.lwr_corr:
                ret = correlatedLWR;
                break;
            case properties_i.lwr_corr_ref:
                ret = correlatedLWRLayerIndex;
                break;
            case properties_i.lwr2_corr:
                ret = correlatedLWR2;
                break;
            case properties_i.lwr2_corr_ref:
                ret = correlatedLWR2LayerIndex;
                break;
            case properties_i.lwrPreview:
                ret = LWRNoisePreview;
                break;
            case properties_i.xOL_ref:
                ret = overlayXReferenceLayer;
                break;
            case properties_i.yOL_ref:
                ret = overlayYReferenceLayer;
                break;
            case properties_i.xOL_av:
                ret = averageOverlayX;
                break;
            case properties_i.yOL_av:
                ret = averageOverlayY;
                break;
            case properties_i.xOL_corr:
                ret = correlatedXOverlay;
                break;
            case properties_i.xOL_corr_ref:
                ret = correlatedXOverlayLayerIndex;
                break;
            case properties_i.yOL_corr:
                ret = correlatedYOverlay;
                break;
            case properties_i.yOL_corr_ref:
                ret = correlatedYOverlayLayerIndex;
                break;
            case properties_i.CDU_corr:
                ret = correlatedCDU;
                break;
            case properties_i.CDU_corr_ref:
                ret = correlatedCDULayerIndex;
                break;
            case properties_i.tCDU_corr:
                ret = correlatedTipCDU;
                break;
            case properties_i.tCDU_corr_ref:
                ret = correlatedTipCDULayerIndex;
                break;
            case properties_i.structure:
                ret = structureFromFile;
                break;
            case properties_i.lD:
                ret = ldFromFile;
                break;
            case properties_i.fill:
                ret = polyFill;
                break;
            case properties_i.fileType:
                ret = fileType;
                break;
            case properties_i.perPoly:
                ret = perPoly;
                break;
            case properties_i.refLayout:
                ret = referenceLayout;
                break;
            case properties_i.bLayerA:
                ret = booleanLayerA;
                break;
            case properties_i.bLayerB:
                ret = booleanLayerB;
                break;
            case properties_i.bLayerOpA:
                ret = booleanLayerOpA;
                break;
            case properties_i.bLayerOpB:
                ret = booleanLayerOpB;
                break;
            case properties_i.bLayerOpAB:
                ret = booleanLayerOpAB;
                break;
            case properties_i.removeArtifacts:
                ret = removeArtifacts;
                break;
            case properties_i.removeArtifactsEpsilon:
                ret = removeArtifactsEpsilon;
                break;
        }

        return ret;
    }

    public void setInt(properties_i p, int val)
    {
        pSetInt(p, val);
    }

    private void pSetInt(properties_i p, int val)
    {
        switch (p)
        {
            case properties_i.enabled:
                setInt(ShapeSettings.properties_i.enabled, val);
                break;
            case properties_i.shapeIndex:
                setInt(ShapeSettings.properties_i.shapeIndex, val);
                break;
            case properties_i.gCSEngine:
                setInt(ShapeSettings.properties_i.gCSEngine, val);
                break;
            case properties_i.shape0Tip:
                setInt(ShapeSettings.properties_i.subShapeTipLocIndex, val);
                break;
            case properties_i.shape1Tip:
                setInt(ShapeSettings.properties_i.subShape2TipLocIndex, val);
                break;
            case properties_i.shape2Tip:
                setInt(ShapeSettings.properties_i.subShape3TipLocIndex, val);
                break;
            case properties_i.subShapeIndex:
                setInt(ShapeSettings.properties_i.subShapeRefIndex, val);
                break;
            case properties_i.posIndex:
                setInt(ShapeSettings.properties_i.posInSubShapeIndex, val);
                break;
            case properties_i.edgeSlide:
                setInt(ShapeSettings.properties_i.edgeSlide, val);
                break;
            case properties_i.proxRays:
                setInt(ShapeSettings.properties_i.proxRays, val);
                break;
            case properties_i.proxSideRaysFallOff:
                setInt(ShapeSettings.properties_i.proxSideRaysFallOff, val);
                break;
            case properties_i.lwrType:
                setInt(ShapeSettings.properties_i.lwrType, val);
                break;
            case properties_i.lwr2Type:
                setInt(ShapeSettings.properties_i.lwr2Type, val);
                break;
            case properties_i.flipH:
                setInt(ShapeSettings.properties_i.flipH, val);
                break;
            case properties_i.flipV:
                setInt(ShapeSettings.properties_i.flipV, val);
                break;
            case properties_i.alignX:
                setInt(ShapeSettings.properties_i.alignX, val);
                break;
            case properties_i.alignY:
                setInt(ShapeSettings.properties_i.alignY, val);
                break;
            case properties_i.showDrawn:
                showDrawn = val;
                break;
            case properties_i.omit:
                omitFromSim = val;
                break;
            case properties_i.lwrPreview:
                LWRNoisePreview = val;
                break;
            case properties_i.lwr_corr:
                correlatedLWR = val;
                break;
            case properties_i.lwr_corr_ref:
                correlatedLWRLayerIndex = val;
                break;
            case properties_i.lwr2_corr:
                correlatedLWR2 = val;
                break;
            case properties_i.lwr2_corr_ref:
                correlatedLWR2LayerIndex = val;
                break;
            case properties_i.xOL_ref:
                overlayXReferenceLayer = val;
                break;
            case properties_i.yOL_ref:
                overlayYReferenceLayer = val;
                break;
            case properties_i.xOL_av:
                averageOverlayX = val;
                break;
            case properties_i.yOL_av:
                averageOverlayY = val;
                break;
            case properties_i.xOL_corr:
                correlatedXOverlay = val;
                break;
            case properties_i.xOL_corr_ref:
                correlatedXOverlayLayerIndex = val;
                break;
            case properties_i.yOL_corr:
                correlatedYOverlay = val;
                break;
            case properties_i.yOL_corr_ref:
                correlatedYOverlayLayerIndex = val;
                break;
            case properties_i.CDU_corr:
                correlatedCDU = val;
                break;
            case properties_i.CDU_corr_ref:
                correlatedCDULayerIndex = val;
                break;
            case properties_i.tCDU_corr:
                correlatedTipCDU = val;
                break;
            case properties_i.tCDU_corr_ref:
                correlatedTipCDULayerIndex = val;
                break;
            case properties_i.structure:
                structureFromFile = val;
                break;
            case properties_i.lD:
                ldFromFile = val;
                break;
            case properties_i.fill:
                polyFill = val;
                break;
            case properties_i.fileType:
                fileType = val;
                break;
            case properties_i.perPoly:
                perPoly = val;
                break;
            case properties_i.refLayout:
                referenceLayout = val;
                break;
            case properties_i.bLayerA:
                booleanLayerA = val;
                break;
            case properties_i.bLayerB:
                booleanLayerB = val;
                break;
            case properties_i.bLayerOpA:
                booleanLayerOpA = val;
                break;
            case properties_i.bLayerOpB:
                booleanLayerOpB = val;
                break;
            case properties_i.bLayerOpAB:
                booleanLayerOpAB = val;
                break;
            case properties_i.removeArtifacts:
                removeArtifacts = val;
                break;
            case properties_i.removeArtifactsEpsilon:
                removeArtifactsEpsilon = val;
                break;
        }
    }

    public void defaultInt(properties_i p)
    {
        pDefaultInt(p);
    }

    private void pDefaultInt(properties_i p)
    {
        switch (p)
        {
            case properties_i.enabled:
                defaultInt(ShapeSettings.properties_i.enabled);
                break;
            case properties_i.shapeIndex:
                defaultInt(ShapeSettings.properties_i.shapeIndex);
                break;
            case properties_i.gCSEngine:
                defaultInt(ShapeSettings.properties_i.gCSEngine);
                break;
            case properties_i.shape0Tip:
                defaultInt(ShapeSettings.properties_i.subShapeTipLocIndex);
                break;
            case properties_i.shape1Tip:
                defaultInt(ShapeSettings.properties_i.subShape2TipLocIndex);
                break;
            case properties_i.shape2Tip:
                defaultInt(ShapeSettings.properties_i.subShape3TipLocIndex);
                break;
            case properties_i.subShapeIndex:
                defaultInt(ShapeSettings.properties_i.subShapeRefIndex);
                break;
            case properties_i.posIndex:
                defaultInt(ShapeSettings.properties_i.posInSubShapeIndex);
                break;
            case properties_i.edgeSlide:
                defaultInt(ShapeSettings.properties_i.edgeSlide);
                break;
            case properties_i.proxRays:
                defaultInt(ShapeSettings.properties_i.proxRays);
                break;
            case properties_i.proxSideRaysFallOff:
                defaultInt(ShapeSettings.properties_i.proxSideRaysFallOff);
                break;
            case properties_i.lwrType:
                defaultInt(ShapeSettings.properties_i.lwrType);
                break;
            case properties_i.lwr2Type:
                defaultInt(ShapeSettings.properties_i.lwr2Type);
                break;
            case properties_i.flipH:
                defaultInt(ShapeSettings.properties_i.flipH);
                break;
            case properties_i.flipV:
                defaultInt(ShapeSettings.properties_i.flipV);
                break;
            case properties_i.alignX:
                defaultInt(ShapeSettings.properties_i.alignX);
                break;
            case properties_i.alignY:
                defaultInt(ShapeSettings.properties_i.alignY);
                break;
            case properties_i.CDU_corr:
                correlatedCDU = default_correlatedCDU;
                break;
            case properties_i.CDU_corr_ref:
                correlatedCDULayerIndex = default_correlatedCDULayerIndex;
                break;
            case properties_i.omit:
                omitFromSim = default_omitLayer;
                break;
            case properties_i.fileType:
                fileType = default_fileType;
                break;
            case properties_i.fill:
                polyFill = default_polyFill;
                break;
            case properties_i.lD:
                ldFromFile = default_ldFromFile;
                break;
            case properties_i.lwrPreview:
                LWRNoisePreview = default_LWRNoisePreview;
                break;
            case properties_i.lwr_corr:
                correlatedLWR = default_correlatedLWR;
                break;
            case properties_i.lwr2_corr:
                correlatedLWR2 = default_correlatedLWR;
                break;
            case properties_i.lwr_corr_ref:
                correlatedLWRLayerIndex = default_correlatedLWRLayerIndex;
                break;
            case properties_i.lwr2_corr_ref:
                correlatedLWR2LayerIndex = default_correlatedLWRLayerIndex;
                break;
            case properties_i.perPoly:
                perPoly = default_perPoly;
                break;
            case properties_i.refLayout:
                referenceLayout = default_referenceLayout;
                break;
            case properties_i.showDrawn:
                showDrawn = default_showDrawn;
                break;
            case properties_i.structure:
                structureFromFile = default_structureFromFile;
                break;
            case properties_i.tCDU_corr:
                correlatedTipCDU = default_correlatedTipCDU;
                break;
            case properties_i.tCDU_corr_ref:
                correlatedTipCDULayerIndex = default_correlatedTipCDULayerIndex;
                break;
            case properties_i.xOL_av:
                averageOverlayX = default_averageOverlayX;
                break;
            case properties_i.xOL_corr:
                correlatedXOverlay = default_correlatedXOverlay;
                break;
            case properties_i.xOL_corr_ref:
                correlatedXOverlayLayerIndex = default_correlatedXOverlayLayerIndex;
                break;
            case properties_i.xOL_ref:
                overlayXReferenceLayer = default_overlayXReferenceLayer;
                break;
            case properties_i.yOL_av:
                averageOverlayY = default_averageOverlayY;
                break;
            case properties_i.yOL_corr:
                correlatedYOverlay = default_correlatedYOverlay;
                break;
            case properties_i.yOL_corr_ref:
                correlatedYOverlayLayerIndex = default_correlatedYOverlayLayerIndex;
                break;
            case properties_i.yOL_ref:
                overlayYReferenceLayer = default_overlayYReferenceLayer;
                break;
            case properties_i.bLayerA:
                booleanLayerA = default_booleanLayerA;
                break;
            case properties_i.bLayerB:
                booleanLayerB = default_booleanLayerA;
                break;
            case properties_i.bLayerOpA:
                booleanLayerOpA = default_booleanLayerA;
                break;
            case properties_i.bLayerOpB:
                booleanLayerOpB = default_booleanLayerA;
                break;
            case properties_i.bLayerOpAB:
                booleanLayerOpAB = default_booleanLayerA;
                break;
            case properties_i.removeArtifacts:
                removeArtifacts = default_removeArtifacts;
                break;
            case properties_i.removeArtifactsEpsilon:
                removeArtifactsEpsilon = default_removeArtifactsEpsilon;
                break;
        }
    }

    public static int getDefaultInt(properties_i p)
    {
        return pGetDefaultInt(p);
    }

    private static int pGetDefaultInt(properties_i p)
    {
        int ret = 0;
        switch (p)
        {
            case properties_i.enabled:
                ret = getDefaultInt(ShapeSettings.properties_i.enabled);
                break;
            case properties_i.shapeIndex:
                ret = getDefaultInt(ShapeSettings.properties_i.shapeIndex);
                break;
            case properties_i.gCSEngine:
                ret = getDefaultInt(ShapeSettings.properties_i.gCSEngine);
                break;
            case properties_i.shape0Tip:
                ret = getDefaultInt(ShapeSettings.properties_i.subShapeTipLocIndex);
                break;
            case properties_i.shape1Tip:
                ret = getDefaultInt(ShapeSettings.properties_i.subShape2TipLocIndex);
                break;
            case properties_i.shape2Tip:
                ret = getDefaultInt(ShapeSettings.properties_i.subShape3TipLocIndex);
                break;
            case properties_i.subShapeIndex:
                ret = getDefaultInt(ShapeSettings.properties_i.subShapeRefIndex);
                break;
            case properties_i.posIndex:
                ret = getDefaultInt(ShapeSettings.properties_i.posInSubShapeIndex);
                break;
            case properties_i.edgeSlide:
                ret = getDefaultInt(ShapeSettings.properties_i.edgeSlide);
                break;
            case properties_i.proxRays:
                ret = getDefaultInt(ShapeSettings.properties_i.proxRays);
                break;
            case properties_i.proxSideRaysFallOff:
                ret = getDefaultInt(ShapeSettings.properties_i.proxSideRaysFallOff);
                break;
            case properties_i.lwrType:
            case properties_i.lwr2Type:
                ret = getDefaultInt(ShapeSettings.properties_i.lwrType);
                break;
            case properties_i.flipH:
                ret = getDefaultInt(ShapeSettings.properties_i.flipH);
                break;
            case properties_i.flipV:
                ret = getDefaultInt(ShapeSettings.properties_i.flipV);
                break;
            case properties_i.alignX:
                ret = getDefaultInt(ShapeSettings.properties_i.alignX);
                break;
            case properties_i.alignY:
                ret = getDefaultInt(ShapeSettings.properties_i.alignY);
                break;
            case properties_i.omit:
                ret = default_omitLayer;
                break;
            case properties_i.showDrawn:
                ret = default_showDrawn;
                break;
            case properties_i.lwrPreview:
                ret = default_LWRNoisePreview;
                break;
            case properties_i.lwr_corr:
            case properties_i.lwr2_corr:
                ret = default_correlatedLWR;
                break;
            case properties_i.lwr_corr_ref:
            case properties_i.lwr2_corr_ref:
                ret = default_correlatedLWRLayerIndex;
                break;
            case properties_i.xOL_ref:
                ret = default_overlayXReferenceLayer;
                break;
            case properties_i.yOL_ref:
                ret = default_overlayYReferenceLayer;
                break;
            case properties_i.xOL_av:
                ret = default_averageOverlayX;
                break;
            case properties_i.yOL_av:
                ret = default_averageOverlayY;
                break;
            case properties_i.xOL_corr:
                ret = default_correlatedXOverlay;
                break;
            case properties_i.xOL_corr_ref:
                ret = default_correlatedXOverlayLayerIndex;
                break;
            case properties_i.yOL_corr:
                ret = default_correlatedYOverlay;
                break;
            case properties_i.yOL_corr_ref:
                ret = default_correlatedYOverlayLayerIndex;
                break;
            case properties_i.CDU_corr:
                ret = default_correlatedCDU;
                break;
            case properties_i.CDU_corr_ref:
                ret = default_correlatedCDULayerIndex;
                break;
            case properties_i.tCDU_corr:
                ret = default_correlatedTipCDU;
                break;
            case properties_i.tCDU_corr_ref:
                ret = default_correlatedTipCDULayerIndex;
                break;
            case properties_i.structure:
                ret = default_structureFromFile;
                break;
            case properties_i.lD:
                ret = default_ldFromFile;
                break;
            case properties_i.fill:
                ret = default_polyFill;
                break;
            case properties_i.fileType:
                ret = default_fileType;
                break;
            case properties_i.perPoly:
                ret = default_perPoly;
                break;
            case properties_i.refLayout:
                ret = default_referenceLayout;
                break;
            case properties_i.bLayerA:
                ret = default_booleanLayerA;
                break;
            case properties_i.bLayerB:
                ret = default_booleanLayerB;
                break;
            case properties_i.bLayerOpA:
                ret = default_booleanLayerOpA;
                break;
            case properties_i.bLayerOpB:
                ret = default_booleanLayerOpB;
                break;
            case properties_i.bLayerOpAB:
                ret = default_booleanLayerOpAB;
                break;
            case properties_i.removeArtifacts:
                ret = default_removeArtifacts;
                break;
            case properties_i.removeArtifactsEpsilon:
                ret = default_removeArtifactsEpsilon;
                break;
        }

        return ret;
    }

    private string comment;
    private string horTipBiasPVar_RNGMapping;
    private string horTipBiasNVar_RNGMapping;
    private string verTipBiasPVar_RNGMapping;
    private string verTipBiasNVar_RNGMapping;
    private string innerCV_RNGMapping;
    private string outerCV_RNGMapping;
    private string LWR_RNGMapping;
    private string LWR2_RNGMapping;
    private string sideCDU_RNGMapping;
    private string tipsCDU_RNGMapping;
    private string horOverlay_RNGMapping;
    private string verOverlay_RNGMapping;
    private string wobble_RNGMapping;
    private string fileToLoad; // file value to load.
    private string structureNameFromFile;
    private string ldNameFromFile;

    public new enum properties_s
    {
        comment, name, hTipPVar_RNG, hTipNVar_RNG, vTipPVar_RNG, vTipNVar_RNG, iCV_RNG, oCV_RNG, lwr_RNG, lwr2_RNG, sCDU_RNG, tCDU_RNG,
        xOL_RNG, yOL_RNG, wobble_RNG,
        file, structure, lD
    }

    public string getString(properties_s p)
    {
        return pGetString(p);
    }

    private string pGetString(properties_s p)
    {
        string ret = "";

        switch (p)
        {
            case properties_s.comment:
                ret = comment;
                break;
            case properties_s.name:
                ret = getString(ShapeSettings.properties_s.s_name);
                break;
            case properties_s.hTipNVar_RNG:
                ret = horTipBiasNVar_RNGMapping;
                break;
            case properties_s.hTipPVar_RNG:
                ret = horTipBiasPVar_RNGMapping;
                break;
            case properties_s.vTipNVar_RNG:
                ret = verTipBiasNVar_RNGMapping;
                break;
            case properties_s.vTipPVar_RNG:
                ret = verTipBiasPVar_RNGMapping;
                break;
            case properties_s.iCV_RNG:
                ret = innerCV_RNGMapping;
                break;
            case properties_s.oCV_RNG:
                ret = outerCV_RNGMapping;
                break;
            case properties_s.lwr_RNG:
                ret = LWR_RNGMapping;
                break;
            case properties_s.lwr2_RNG:
                ret = LWR2_RNGMapping;
                break;
            case properties_s.sCDU_RNG:
                ret = sideCDU_RNGMapping;
                break;
            case properties_s.tCDU_RNG:
                ret = tipsCDU_RNGMapping;
                break;
            case properties_s.xOL_RNG:
                ret = horOverlay_RNGMapping;
                break;
            case properties_s.yOL_RNG:
                ret = verOverlay_RNGMapping;
                break;
            case properties_s.wobble_RNG:
                ret = wobble_RNGMapping;
                break;
            case properties_s.file:
                ret = fileToLoad;
                break;
            case properties_s.structure:
                ret = structureNameFromFile;
                break;
            case properties_s.lD:
                ret = ldNameFromFile;
                break;
        }

        return ret;
    }

    public void setString(properties_s p, string val)
    {
        pSetString(p, val);
    }

    private void pSetString(properties_s p, string val)
    {
        switch (p)
        {
            case properties_s.comment:
                comment = val;
                break;
            case properties_s.name:
                setString(ShapeSettings.properties_s.s_name, val);
                break;
            case properties_s.hTipNVar_RNG:
                horTipBiasNVar_RNGMapping = val;
                break;
            case properties_s.hTipPVar_RNG:
                horTipBiasPVar_RNGMapping = val;
                break;
            case properties_s.vTipNVar_RNG:
                verTipBiasNVar_RNGMapping = val;
                break;
            case properties_s.vTipPVar_RNG:
                verTipBiasPVar_RNGMapping = val;
                break;
            case properties_s.iCV_RNG:
                innerCV_RNGMapping = val;
                break;
            case properties_s.oCV_RNG:
                outerCV_RNGMapping = val;
                break;
            case properties_s.lwr_RNG:
                LWR_RNGMapping = val;
                break;
            case properties_s.lwr2_RNG:
                LWR2_RNGMapping = val;
                break;
            case properties_s.sCDU_RNG:
                sideCDU_RNGMapping = val;
                break;
            case properties_s.tCDU_RNG:
                tipsCDU_RNGMapping = val;
                break;
            case properties_s.xOL_RNG:
                horOverlay_RNGMapping = val;
                break;
            case properties_s.yOL_RNG:
                verOverlay_RNGMapping = val;
                break;
            case properties_s.wobble_RNG:
                wobble_RNGMapping = val;
                break;
            case properties_s.file:
                fileToLoad = val;
                break;
            case properties_s.structure:
                structureNameFromFile = val;
                break;
            case properties_s.lD:
                ldNameFromFile = val;
                break;
        }
    }

    public void defaultString(properties_s p)
    {
        pDefaultString(p);
    }

    private void pDefaultString(properties_s p)
    {
        switch (p)
        {
            case properties_s.comment:
                comment = default_comment;
                break;
            case properties_s.file:
                fileToLoad = default_fileToLoad;
                break;
            case properties_s.hTipNVar_RNG:
                horTipBiasNVar_RNGMapping = default_rngmapping;
                break;
            case properties_s.hTipPVar_RNG:
                horTipBiasPVar_RNGMapping = default_rngmapping;
                break;
            case properties_s.iCV_RNG:
                innerCV_RNGMapping = default_rngmapping;
                break;
            case properties_s.lD:
                ldNameFromFile = default_ldNameFromFile;
                break;
            case properties_s.lwr2_RNG:
                LWR2_RNGMapping = default_rngmapping;
                break;
            case properties_s.lwr_RNG:
                LWR_RNGMapping = default_rngmapping;
                break;
            case properties_s.name:
                defaultString(ShapeSettings.properties_s.s_name);
                break;
            case properties_s.oCV_RNG:
                outerCV_RNGMapping = default_rngmapping;
                break;
            case properties_s.sCDU_RNG:
                sideCDU_RNGMapping = default_rngmapping;
                break;
            case properties_s.structure:
                structureNameFromFile = default_structureNameFromFile;
                break;
            case properties_s.tCDU_RNG:
                tipsCDU_RNGMapping = default_rngmapping;
                break;
            case properties_s.vTipNVar_RNG:
                verTipBiasNVar_RNGMapping = default_rngmapping;
                break;
            case properties_s.vTipPVar_RNG:
                verTipBiasPVar_RNGMapping = default_rngmapping;
                break;
            case properties_s.wobble_RNG:
                wobble_RNGMapping = default_rngmapping;
                break;
            case properties_s.xOL_RNG:
                horOverlay_RNGMapping = default_rngmapping;
                break;
            case properties_s.yOL_RNG:
                verOverlay_RNGMapping = default_rngmapping;
                break;
        }
    }

    public static string getDefaultString(properties_s p)
    {
        return pGetDefaultString(p);
    }

    private static string pGetDefaultString(properties_s p)
    {
        string ret = "";

        switch (p)
        {
            case properties_s.comment:
                ret = default_comment;
                break;
            case properties_s.name:
                ret = getDefaultString(ShapeSettings.properties_s.s_name);
                break;
            case properties_s.hTipNVar_RNG:
            case properties_s.hTipPVar_RNG:
            case properties_s.vTipNVar_RNG:
            case properties_s.vTipPVar_RNG:
            case properties_s.iCV_RNG:
            case properties_s.oCV_RNG:
            case properties_s.lwr_RNG:
            case properties_s.lwr2_RNG:
            case properties_s.sCDU_RNG:
            case properties_s.tCDU_RNG:
            case properties_s.xOL_RNG:
            case properties_s.yOL_RNG:
            case properties_s.wobble_RNG:
                ret = default_rngmapping;
                break;
            case properties_s.file:
                ret = default_fileToLoad;
                break;
            case properties_s.structure:
                ret = default_structureNameFromFile;
                break;
            case properties_s.lD:
                ret = default_ldNameFromFile;
                break;
        }

        return ret;
    }

    private decimal wobble;
    private decimal lensDistortionCoeff1;
    private decimal lensDistortionCoeff2;
    private decimal innerCV;
    private decimal outerCV;
    private decimal sideCDU;
    private decimal tipsCDU;
    private decimal horOverlay;
    private decimal verOverlay;

    public new enum properties_decimal
    {
        horLength, verLength, horOffset, verOffset,
        gHorOffset, gVerOffset,
        rot, wobble,
        sBias, hTBias, hTNVar, hTPVar, vTBias, vTNVar, vTPVar,
        pBias, pBiasDist,
        lDC1, lDC2,
        iCR, oCR, iCV, oCV,
        lwr, lwrFreq, lwr2, lwr2Freq,
        eTension,
        sCDU, tCDU,
        xOL, yOL,
        proxSideRaysMultiplier,
        rayExtension, keyhole_factor,
    }

    public decimal getDecimal(properties_decimal p, int _subShapeRef = -1)
    {
        return pGetDecimal(p, _subShapeRef);
    }

    private decimal pGetDecimal(properties_decimal p, int _subShapeRef)
    {
        decimal ret = 0m;
        switch (p)
        {
            case properties_decimal.horLength:
                ret = getDecimal(ShapeSettings.properties_decimal.horLength, _subShapeRef);
                break;
            case properties_decimal.verLength:
                ret = getDecimal(ShapeSettings.properties_decimal.verLength, _subShapeRef);
                break;
            case properties_decimal.horOffset:
                ret = getDecimal(ShapeSettings.properties_decimal.horOffset, _subShapeRef);
                break;
            case properties_decimal.verOffset:
                ret = getDecimal(ShapeSettings.properties_decimal.verOffset, _subShapeRef);
                break;
            case properties_decimal.gHorOffset:
                ret = getDecimal(ShapeSettings.properties_decimal.gHorOffset);
                break;
            case properties_decimal.gVerOffset:
                ret = getDecimal(ShapeSettings.properties_decimal.gVerOffset);
                break;
            case properties_decimal.iCR:
                ret = getDecimal(ShapeSettings.properties_decimal.iCR);
                break;
            case properties_decimal.oCR:
                ret = getDecimal(ShapeSettings.properties_decimal.oCR);
                break;
            case properties_decimal.sBias:
                ret = getDecimal(ShapeSettings.properties_decimal.sBias);
                break;
            case properties_decimal.hTBias:
                ret = getDecimal(ShapeSettings.properties_decimal.hTBias);
                break;
            case properties_decimal.hTNVar:
                ret = getDecimal(ShapeSettings.properties_decimal.hTNVar);
                break;
            case properties_decimal.hTPVar:
                ret = getDecimal(ShapeSettings.properties_decimal.hTPVar);
                break;
            case properties_decimal.vTBias:
                ret = getDecimal(ShapeSettings.properties_decimal.vTBias);
                break;
            case properties_decimal.vTNVar:
                ret = getDecimal(ShapeSettings.properties_decimal.vTNVar);
                break;
            case properties_decimal.vTPVar:
                ret = getDecimal(ShapeSettings.properties_decimal.vTPVar);
                break;
            case properties_decimal.lwr:
                ret = getDecimal(ShapeSettings.properties_decimal.lwr);
                break;
            case properties_decimal.lwrFreq:
                ret = getDecimal(ShapeSettings.properties_decimal.lwrFreq);
                break;
            case properties_decimal.lwr2:
                ret = getDecimal(ShapeSettings.properties_decimal.lwr2);
                break;
            case properties_decimal.lwr2Freq:
                ret = getDecimal(ShapeSettings.properties_decimal.lwr2Freq);
                break;
            case properties_decimal.eTension:
                ret = getDecimal(ShapeSettings.properties_decimal.eTension);
                break;
            case properties_decimal.rot:
                ret = getDecimal(ShapeSettings.properties_decimal.rot);
                break;
            case properties_decimal.pBias:
                ret = getDecimal(ShapeSettings.properties_decimal.pBias);
                break;
            case properties_decimal.pBiasDist:
                ret = getDecimal(ShapeSettings.properties_decimal.pBiasDist);
                break;
            case properties_decimal.proxSideRaysMultiplier:
                ret = getDecimal(ShapeSettings.properties_decimal.proxSideRaysMultiplier);
                break;
            case properties_decimal.rayExtension:
                ret = getDecimal(ShapeSettings.properties_decimal.rayExtension);
                break;
            case properties_decimal.keyhole_factor:
                ret = getDecimal(ShapeSettings.properties_decimal.keyhole_factor);
                break;
            case properties_decimal.wobble:
                ret = wobble;
                break;
            case properties_decimal.lDC1:
                ret = lensDistortionCoeff1;
                break;
            case properties_decimal.lDC2:
                ret = lensDistortionCoeff2;
                break;
            case properties_decimal.iCV:
                ret = innerCV;
                break;
            case properties_decimal.oCV:
                ret = outerCV;
                break;
            case properties_decimal.sCDU:
                ret = sideCDU;
                break;
            case properties_decimal.tCDU:
                ret = tipsCDU;
                break;
            case properties_decimal.xOL:
                ret = horOverlay;
                break;
            case properties_decimal.yOL:
                ret = verOverlay;
                break;
        }

        return ret;
    }

    public static decimal getDefaultDecimal(properties_decimal p, int _subShapeRef = -1)
    {
        return pGetDefaultDecimal(p, _subShapeRef);
    }

    private static decimal pGetDefaultDecimal(properties_decimal p, int _subShapeRef)
    {
        decimal ret = 0m;
        switch (p)
        {
            case properties_decimal.horLength:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.horLength, _subShapeRef);
                break;
            case properties_decimal.verLength:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.verLength, _subShapeRef);
                break;
            case properties_decimal.horOffset:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.horOffset, _subShapeRef);
                break;
            case properties_decimal.verOffset:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.verOffset, _subShapeRef);
                break;
            case properties_decimal.gHorOffset:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.gHorOffset);
                break;
            case properties_decimal.gVerOffset:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.gVerOffset);
                break;
            case properties_decimal.iCR:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.iCR);
                break;
            case properties_decimal.oCR:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.oCR);
                break;
            case properties_decimal.sBias:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.sBias);
                break;
            case properties_decimal.hTBias:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.hTBias);
                break;
            case properties_decimal.hTNVar:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.hTNVar);
                break;
            case properties_decimal.hTPVar:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.hTPVar);
                break;
            case properties_decimal.vTBias:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.vTBias);
                break;
            case properties_decimal.vTNVar:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.vTNVar);
                break;
            case properties_decimal.vTPVar:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.vTPVar);
                break;
            case properties_decimal.lwr:
            case properties_decimal.lwr2:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.lwr);
                break;
            case properties_decimal.lwrFreq:
            case properties_decimal.lwr2Freq:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.lwrFreq);
                break;
            case properties_decimal.eTension:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.eTension);
                break;
            case properties_decimal.rot:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.rot);
                break;
            case properties_decimal.pBias:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.pBias);
                break;
            case properties_decimal.pBiasDist:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.pBiasDist);
                break;
            case properties_decimal.proxSideRaysMultiplier:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.proxSideRaysMultiplier);
                break;
            case properties_decimal.rayExtension:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.rayExtension);
                break;
            case properties_decimal.keyhole_factor:
                ret = getDefaultDecimal(ShapeSettings.properties_decimal.keyhole_factor);
                break;
            case properties_decimal.wobble:
                ret = default_wobble;
                break;
            case properties_decimal.lDC1:
                ret = default_lensDistortionCoeff1;
                break;
            case properties_decimal.lDC2:
                ret = default_lensDistortionCoeff2;
                break;
            case properties_decimal.iCV:
                ret = default_innerCV;
                break;
            case properties_decimal.oCV:
                ret = default_outerCV;
                break;
            case properties_decimal.sCDU:
                ret = default_sideCDU;
                break;
            case properties_decimal.tCDU:
                ret = default_tipsCDU;
                break;
            case properties_decimal.xOL:
                ret = default_horOverlay;
                break;
            case properties_decimal.yOL:
                ret = default_verOverlay;
                break;
        }

        return ret;
    }

    public void setDecimal(properties_decimal p, decimal val, int _subShapeRef = -1)
    {
        pSetDecimal(p, val, _subShapeRef);
    }

    private void pSetDecimal(properties_decimal p, decimal val, int _subShapeRef)
    {
        switch (p)
        {
            case properties_decimal.horLength:
                setDecimal(ShapeSettings.properties_decimal.horLength, val, _subShapeRef);
                break;
            case properties_decimal.verLength:
                setDecimal(ShapeSettings.properties_decimal.verLength, val, _subShapeRef);
                break;
            case properties_decimal.horOffset:
                setDecimal(ShapeSettings.properties_decimal.horOffset, val, _subShapeRef);
                break;
            case properties_decimal.verOffset:
                setDecimal(ShapeSettings.properties_decimal.verOffset, val, _subShapeRef);
                break;
            case properties_decimal.gHorOffset:
                setDecimal(ShapeSettings.properties_decimal.gHorOffset, val);
                break;
            case properties_decimal.gVerOffset:
                setDecimal(ShapeSettings.properties_decimal.gVerOffset, val);
                break;
            case properties_decimal.iCR:
                setDecimal(ShapeSettings.properties_decimal.iCR, val);
                break;
            case properties_decimal.oCR:
                setDecimal(ShapeSettings.properties_decimal.oCR, val);
                break;
            case properties_decimal.sBias:
                setDecimal(ShapeSettings.properties_decimal.sBias, val);
                break;
            case properties_decimal.hTBias:
                setDecimal(ShapeSettings.properties_decimal.hTBias, val);
                break;
            case properties_decimal.hTNVar:
                setDecimal(ShapeSettings.properties_decimal.hTNVar, val);
                break;
            case properties_decimal.hTPVar:
                setDecimal(ShapeSettings.properties_decimal.hTPVar, val);
                break;
            case properties_decimal.vTBias:
                setDecimal(ShapeSettings.properties_decimal.vTBias, val);
                break;
            case properties_decimal.vTNVar:
                setDecimal(ShapeSettings.properties_decimal.vTNVar, val);
                break;
            case properties_decimal.vTPVar:
                setDecimal(ShapeSettings.properties_decimal.vTPVar, val);
                break;
            case properties_decimal.lwr:
                setDecimal(ShapeSettings.properties_decimal.lwr, val);
                break;
            case properties_decimal.lwrFreq:
                setDecimal(ShapeSettings.properties_decimal.lwrFreq, val);
                break;
            case properties_decimal.lwr2:
                setDecimal(ShapeSettings.properties_decimal.lwr2, val);
                break;
            case properties_decimal.lwr2Freq:
                setDecimal(ShapeSettings.properties_decimal.lwr2Freq, val);
                break;
            case properties_decimal.eTension:
                setDecimal(ShapeSettings.properties_decimal.eTension, val);
                break;
            case properties_decimal.rot:
                setDecimal(ShapeSettings.properties_decimal.rot, val);
                break;
            case properties_decimal.pBias:
                setDecimal(ShapeSettings.properties_decimal.pBias, val);
                break;
            case properties_decimal.pBiasDist:
                setDecimal(ShapeSettings.properties_decimal.pBiasDist, val);
                break;
            case properties_decimal.proxSideRaysMultiplier:
                setDecimal(ShapeSettings.properties_decimal.proxSideRaysMultiplier, val);
                break;
            case properties_decimal.rayExtension:
                setDecimal(ShapeSettings.properties_decimal.rayExtension, val);
                break;
            case properties_decimal.keyhole_factor:
                setDecimal(ShapeSettings.properties_decimal.keyhole_factor, val);
                break;
            case properties_decimal.wobble:
                wobble = val;
                break;
            case properties_decimal.lDC1:
                lensDistortionCoeff1 = val;
                break;
            case properties_decimal.lDC2:
                lensDistortionCoeff2 = val;
                break;
            case properties_decimal.iCV:
                innerCV = val;
                break;
            case properties_decimal.oCV:
                outerCV = val;
                break;
            case properties_decimal.sCDU:
                sideCDU = val;
                break;
            case properties_decimal.tCDU:
                tipsCDU = val;
                break;
            case properties_decimal.xOL:
                horOverlay = val;
                break;
            case properties_decimal.yOL:
                verOverlay = val;
                break;
        }
    }

    public void defaultDecimal(properties_decimal p, int _subShapeRef = -1)
    {
        pDefaultDecimal(p, _subShapeRef);
    }

    private void pDefaultDecimal(properties_decimal p, int _subShapeRef)
    {
        switch (p)
        {
            case properties_decimal.horLength:
                defaultDecimal(ShapeSettings.properties_decimal.horLength, _subShapeRef);
                break;
            case properties_decimal.verLength:
                defaultDecimal(ShapeSettings.properties_decimal.verLength, _subShapeRef);
                break;
            case properties_decimal.horOffset:
                defaultDecimal(ShapeSettings.properties_decimal.horOffset, _subShapeRef);
                break;
            case properties_decimal.verOffset:
                defaultDecimal(ShapeSettings.properties_decimal.verOffset, _subShapeRef);
                break;
            case properties_decimal.gHorOffset:
                defaultDecimal(ShapeSettings.properties_decimal.gHorOffset);
                break;
            case properties_decimal.gVerOffset:
                defaultDecimal(ShapeSettings.properties_decimal.gVerOffset);
                break;
            case properties_decimal.iCR:
                defaultDecimal(ShapeSettings.properties_decimal.iCR);
                break;
            case properties_decimal.oCR:
                defaultDecimal(ShapeSettings.properties_decimal.oCR);
                break;
            case properties_decimal.sBias:
                defaultDecimal(ShapeSettings.properties_decimal.sBias);
                break;
            case properties_decimal.hTBias:
                defaultDecimal(ShapeSettings.properties_decimal.hTBias);
                break;
            case properties_decimal.hTNVar:
                defaultDecimal(ShapeSettings.properties_decimal.hTNVar);
                break;
            case properties_decimal.hTPVar:
                defaultDecimal(ShapeSettings.properties_decimal.hTPVar);
                break;
            case properties_decimal.vTBias:
                defaultDecimal(ShapeSettings.properties_decimal.vTBias);
                break;
            case properties_decimal.vTNVar:
                defaultDecimal(ShapeSettings.properties_decimal.vTNVar);
                break;
            case properties_decimal.vTPVar:
                defaultDecimal(ShapeSettings.properties_decimal.vTPVar);
                break;
            case properties_decimal.lwr:
                defaultDecimal(ShapeSettings.properties_decimal.lwr);
                break;
            case properties_decimal.lwr2:
                defaultDecimal(ShapeSettings.properties_decimal.lwr2);
                break;
            case properties_decimal.lwrFreq:
                defaultDecimal(ShapeSettings.properties_decimal.lwrFreq);
                break;
            case properties_decimal.lwr2Freq:
                defaultDecimal(ShapeSettings.properties_decimal.lwr2Freq);
                break;
            case properties_decimal.eTension:
                defaultDecimal(ShapeSettings.properties_decimal.eTension);
                break;
            case properties_decimal.rot:
                defaultDecimal(ShapeSettings.properties_decimal.rot);
                break;
            case properties_decimal.pBias:
                defaultDecimal(ShapeSettings.properties_decimal.pBias);
                break;
            case properties_decimal.pBiasDist:
                defaultDecimal(ShapeSettings.properties_decimal.pBiasDist);
                break;
            case properties_decimal.proxSideRaysMultiplier:
                defaultDecimal(ShapeSettings.properties_decimal.proxSideRaysMultiplier);
                break;
            case properties_decimal.rayExtension:
                defaultDecimal(ShapeSettings.properties_decimal.rayExtension);
                break;
            case properties_decimal.keyhole_factor:
                defaultDecimal(ShapeSettings.properties_decimal.keyhole_factor);
                break;
            case properties_decimal.iCV:
                innerCV = default_innerCV;
                break;
            case properties_decimal.lDC1:
                lensDistortionCoeff1 = default_lensDistortionCoeff1;
                break;
            case properties_decimal.lDC2:
                lensDistortionCoeff2 = default_lensDistortionCoeff2;
                break;
            case properties_decimal.oCV:
                outerCV = default_outerCV;
                break;
            case properties_decimal.sCDU:
                sideCDU = default_sideCDU;
                break;
            case properties_decimal.tCDU:
                tipsCDU = default_tipsCDU;
                break;
            case properties_decimal.wobble:
                wobble = default_wobble;
                break;
            case properties_decimal.xOL:
                horOverlay = default_horOverlay;
                break;
            case properties_decimal.yOL:
                verOverlay = default_verOverlay;
                break;
        }
    }

    private bool reloadedFileData; // if true, we pulled in point data from a reloaded XML file and shouldn't obliterate it.
    public bool isReloaded()
    {
        return pIsReloaded();
    }

    private bool pIsReloaded()
    {
        return reloadedFileData;
    }

    public void setReloaded(bool val)
    {
        pSetReloaded(val);
    }

    private void pSetReloaded(bool val)
    {
        reloadedFileData = val;
    }

    private PathsD fileData; // holds the parsed point data of our level, for file-based content. Allows for multiple polys in layer

    public PathsD getFileData()
    {
        return pGetFileData();
    }

    private PathsD pGetFileData()
    {
        return fileData;
    }

    public void setFileData(PathsD newdata)
    {
        pSetFileData(newdata);
    }

    private void pSetFileData(PathsD newdata)
    {
        fileData = new(newdata);
    }

    public void defaultFileData()
    {
        pDefaultFileData();
    }

    private void pDefaultFileData()
    {
        fileData = new(default_fileData);
    }

    public EntropyLayerSettings()
    {
        pEntropyLayerSettings();
    }

    private void pEntropyLayerSettings()
    {
        comment = default_comment;
        bgLayers = default_bgLayers.ToArray();
        
        wobble = default_wobble;
        wobble_RNGMapping = default_rngmapping;
        horTipBiasNVar_RNGMapping = default_rngmapping;
        horTipBiasPVar_RNGMapping = default_rngmapping;
        verTipBiasNVar_RNGMapping = default_rngmapping;
        verTipBiasPVar_RNGMapping = default_rngmapping;

        innerCV = default_innerCV;
        innerCV_RNGMapping = default_rngmapping;
        outerCV = default_outerCV;
        outerCV_RNGMapping = default_rngmapping;

        LWR_RNGMapping = default_rngmapping;
        LWRNoisePreview = default_LWRNoisePreview;
        LWR2_RNGMapping = default_rngmapping;

        correlatedLWR = default_correlatedLWR;
        correlatedLWRLayerIndex = default_correlatedLWRLayerIndex;
        correlatedLWR2 = default_correlatedLWR;
        correlatedLWR2LayerIndex = default_correlatedLWRLayerIndex;

        horOverlay = default_horOverlay;
        horOverlay_RNGMapping = default_rngmapping;
        verOverlay = default_verOverlay;
        verOverlay_RNGMapping = default_rngmapping;

        sideCDU = default_sideCDU;
        sideCDU_RNGMapping = default_rngmapping;
        tipsCDU = default_tipsCDU;
        tipsCDU_RNGMapping = default_rngmapping;

        fileType = default_fileType;
        fileData = default_fileData;
        fileToLoad = default_fileToLoad;
        ldFromFile = default_ldFromFile;
        polyFill = default_polyFill;
        ldNameFromFile = default_ldNameFromFile;
        reloadedFileData = false;
        structureFromFile = default_structureFromFile;
        structureNameFromFile = default_structureNameFromFile;
        perPoly = default_perPoly;
        referenceLayout = default_referenceLayout;

        correlatedTipCDU = default_correlatedTipCDU;
        correlatedTipCDULayerIndex = default_correlatedTipCDULayerIndex;
        correlatedCDU = default_correlatedCDU;
        correlatedCDULayerIndex = default_correlatedCDULayerIndex;
        correlatedXOverlay = default_correlatedXOverlay;
        correlatedXOverlayLayerIndex = default_correlatedXOverlayLayerIndex;
        correlatedYOverlay = default_correlatedYOverlay;
        correlatedYOverlayLayerIndex = default_correlatedYOverlayLayerIndex;
        overlayXReferenceLayer = default_overlayXReferenceLayer;
        overlayYReferenceLayer = default_overlayYReferenceLayer;
        averageOverlayX = default_averageOverlayX;
        averageOverlayY = default_averageOverlayY;
        overlayXReferenceLayers = default_overlayXReferenceLayers.ToArray();
        overlayYReferenceLayers = default_overlayYReferenceLayers.ToArray();
        showDrawn = default_showDrawn;

        lensDistortionCoeff1 = default_lensDistortionCoeff1;
        lensDistortionCoeff2 = default_lensDistortionCoeff2;

        booleanLayerA = default_booleanLayerA;
        booleanLayerB = default_booleanLayerB;
        booleanLayerOpA = default_booleanLayerOpA;
        booleanLayerOpB = default_booleanLayerOpB;
        booleanLayerOpAB = default_booleanLayerOpAB;

        omitFromSim = default_omitLayer;

        removeArtifacts = default_removeArtifacts;
        removeArtifactsEpsilon = default_removeArtifactsEpsilon;
    }

    public void adjustSettings(EntropyLayerSettings source, bool gdsOnly)
    {
        pAdjustSettings(source, gdsOnly);
    }

    private void pAdjustSettings(EntropyLayerSettings source, bool gdsOnly)
    {
        comment = source.comment;
        bgLayers = source.bgLayers.ToArray();
        setInt(ShapeSettings.properties_i.enabled, source.getInt(ShapeSettings.properties_i.enabled));
        setInt(ShapeSettings.properties_i.gCSEngine, source.getInt(ShapeSettings.properties_i.gCSEngine));
        setInt(ShapeSettings.properties_i.shapeIndex, source.getInt(ShapeSettings.properties_i.shapeIndex));
        showDrawn = source.showDrawn;

        setString(ShapeSettings.properties_s.s_name, source.getString(ShapeSettings.properties_s.s_name));

        if (!gdsOnly)
        {
            for (int i = 0; i < 3; i++)
            {
                setDecimal(ShapeSettings.properties_decimal.horLength, source.getDecimal(ShapeSettings.properties_decimal.horLength, i), i);
                setDecimal(ShapeSettings.properties_decimal.verLength, source.getDecimal(ShapeSettings.properties_decimal.verLength, i), i);
                setDecimal(ShapeSettings.properties_decimal.horOffset, source.getDecimal(ShapeSettings.properties_decimal.horOffset, i), i);
                setDecimal(ShapeSettings.properties_decimal.verOffset, source.getDecimal(ShapeSettings.properties_decimal.verOffset, i), i);
            }
            setInt(ShapeSettings.properties_i.subShapeTipLocIndex, source.getInt(ShapeSettings.properties_i.subShapeTipLocIndex));
            setInt(ShapeSettings.properties_i.subShape2TipLocIndex, source.getInt(ShapeSettings.properties_i.subShape2TipLocIndex));
            setInt(ShapeSettings.properties_i.subShape3TipLocIndex, source.getInt(ShapeSettings.properties_i.subShape3TipLocIndex));
            
            setInt(ShapeSettings.properties_i.subShapeRefIndex, source.getInt(ShapeSettings.properties_i.subShapeRefIndex));
            setInt(ShapeSettings.properties_i.posInSubShapeIndex, source.getInt(ShapeSettings.properties_i.posInSubShapeIndex));
            
            setDecimal(ShapeSettings.properties_decimal.gHorOffset, source.getDecimal(ShapeSettings.properties_decimal.gHorOffset));
            setDecimal(ShapeSettings.properties_decimal.gVerOffset, source.getDecimal(ShapeSettings.properties_decimal.gVerOffset));
            
            setDecimal(ShapeSettings.properties_decimal.rot, source.getDecimal(ShapeSettings.properties_decimal.rot));
            wobble = source.wobble;
            wobble_RNGMapping = source.wobble_RNGMapping;
            setDecimal(ShapeSettings.properties_decimal.sBias, source.getDecimal(ShapeSettings.properties_decimal.sBias));

            setDecimal(ShapeSettings.properties_decimal.hTBias, source.getDecimal(ShapeSettings.properties_decimal.hTBias));
            setDecimal(ShapeSettings.properties_decimal.hTNVar, source.getDecimal(ShapeSettings.properties_decimal.hTNVar));
            setDecimal(ShapeSettings.properties_decimal.hTPVar, source.getDecimal(ShapeSettings.properties_decimal.hTPVar));

            horTipBiasNVar_RNGMapping = source.horTipBiasNVar_RNGMapping;
            horTipBiasPVar_RNGMapping = source.horTipBiasPVar_RNGMapping;

            setDecimal(ShapeSettings.properties_decimal.vTBias, source.getDecimal(ShapeSettings.properties_decimal.vTBias));
            setDecimal(ShapeSettings.properties_decimal.vTNVar, source.getDecimal(ShapeSettings.properties_decimal.vTNVar));
            setDecimal(ShapeSettings.properties_decimal.vTPVar, source.getDecimal(ShapeSettings.properties_decimal.vTPVar));
            verTipBiasNVar_RNGMapping = source.verTipBiasNVar_RNGMapping;
            verTipBiasPVar_RNGMapping = source.verTipBiasPVar_RNGMapping;

            setDecimal(ShapeSettings.properties_decimal.pBias, source.getDecimal(ShapeSettings.properties_decimal.pBias));
            setDecimal(ShapeSettings.properties_decimal.pBiasDist, source.getDecimal(ShapeSettings.properties_decimal.pBiasDist));
            setInt(ShapeSettings.properties_i.proxRays, source.getInt(ShapeSettings.properties_i.proxRays));
            setInt(ShapeSettings.properties_i.proxSideRaysFallOff, source.getInt(ShapeSettings.properties_i.proxSideRaysFallOff));
            setDecimal(ShapeSettings.properties_decimal.proxSideRaysMultiplier, source.getDecimal(ShapeSettings.properties_decimal.proxSideRaysMultiplier));

            setDecimal(ShapeSettings.properties_decimal.iCR, source.getDecimal(ShapeSettings.properties_decimal.iCR));
            innerCV = source.innerCV;
            innerCV_RNGMapping = source.innerCV_RNGMapping;
            setDecimal(ShapeSettings.properties_decimal.oCR, source.getDecimal(ShapeSettings.properties_decimal.oCR));
            outerCV = source.outerCV;
            outerCV_RNGMapping = source.outerCV_RNGMapping;

            setInt(ShapeSettings.properties_i.edgeSlide, source.getInt(ShapeSettings.properties_i.edgeSlide));
            setDecimal(ShapeSettings.properties_decimal.eTension, source.getDecimal(ShapeSettings.properties_decimal.eTension));

            setDecimal(ShapeSettings.properties_decimal.lwr, source.getDecimal(ShapeSettings.properties_decimal.lwr));
            setDecimal(ShapeSettings.properties_decimal.lwrFreq, source.getDecimal(ShapeSettings.properties_decimal.lwrFreq));
            setInt(ShapeSettings.properties_i.lwrType, source.getInt(ShapeSettings.properties_i.lwrType));
            LWRNoisePreview = source.LWRNoisePreview;

            setDecimal(ShapeSettings.properties_decimal.lwr2, source.getDecimal(ShapeSettings.properties_decimal.lwr2));
            setDecimal(ShapeSettings.properties_decimal.lwr2Freq, source.getDecimal(ShapeSettings.properties_decimal.lwr2Freq));
            setInt(ShapeSettings.properties_i.lwr2Type, source.getInt(ShapeSettings.properties_i.lwr2Type));

            correlatedLWR = source.correlatedLWR;
            correlatedLWRLayerIndex = source.correlatedLWRLayerIndex;
            correlatedLWR2 = source.correlatedLWR2;
            correlatedLWR2LayerIndex = source.correlatedLWR2LayerIndex;

            sideCDU = source.sideCDU;
            sideCDU_RNGMapping = source.sideCDU_RNGMapping;
            tipsCDU = source.tipsCDU;
            tipsCDU_RNGMapping = source.tipsCDU_RNGMapping;

            horOverlay = source.horOverlay;
            horOverlay_RNGMapping = source.horOverlay_RNGMapping;
            verOverlay = source.verOverlay;
            verOverlay_RNGMapping = source.verOverlay_RNGMapping;

            correlatedTipCDU = source.correlatedTipCDU;
            correlatedTipCDULayerIndex = source.correlatedTipCDULayerIndex;

            correlatedCDU = source.correlatedCDU;
            correlatedCDULayerIndex = source.correlatedCDULayerIndex;

            correlatedXOverlay = source.correlatedXOverlay;
            correlatedXOverlayLayerIndex = source.correlatedXOverlayLayerIndex;

            correlatedYOverlay = source.correlatedYOverlay;
            correlatedYOverlayLayerIndex = source.correlatedYOverlayLayerIndex;

            overlayXReferenceLayer = source.overlayXReferenceLayer;
            overlayYReferenceLayer = source.overlayYReferenceLayer;

            averageOverlayX = source.averageOverlayX;
            averageOverlayY = source.averageOverlayY;
            overlayXReferenceLayers = source.overlayXReferenceLayers.ToArray();
            overlayYReferenceLayers = source.overlayYReferenceLayers.ToArray();

            setInt(ShapeSettings.properties_i.flipH, source.getInt(ShapeSettings.properties_i.flipH));
            setInt(ShapeSettings.properties_i.flipV, source.getInt(ShapeSettings.properties_i.flipV));
            setInt(ShapeSettings.properties_i.alignX, source.getInt(ShapeSettings.properties_i.alignX));
            setInt(ShapeSettings.properties_i.alignY, source.getInt(ShapeSettings.properties_i.alignY));

            lensDistortionCoeff1 = source.lensDistortionCoeff1;
            lensDistortionCoeff2 = source.lensDistortionCoeff2;

            booleanLayerA = source.booleanLayerA;
            booleanLayerB = source.booleanLayerB;
            booleanLayerOpA = source.booleanLayerOpA;
            booleanLayerOpB = source.booleanLayerOpB;
            booleanLayerOpAB = source.booleanLayerOpAB;
            setDecimal(ShapeSettings.properties_decimal.rayExtension, source.getDecimal(properties_decimal.rayExtension));

            omitFromSim = source.omitFromSim;
            
            setInt(properties_i.removeArtifacts, source.getInt(properties_i.removeArtifacts));
            setInt(properties_i.removeArtifactsEpsilon, source.getInt(properties_i.removeArtifactsEpsilon));
        }

        // layout stuff

        fileToLoad = source.fileToLoad;
        structureFromFile = source.structureFromFile;
        structureNameFromFile = source.structureNameFromFile;
        ldFromFile = source.ldFromFile;
        ldNameFromFile = source.ldNameFromFile;
        polyFill = source.polyFill;
        fileData = new (source.fileData);
        referenceLayout = source.referenceLayout;
        fileType = source.fileType;

        reloadedFileData = source.reloadedFileData;

        perPoly = source.perPoly;
    }

    public EntropyLayerSettings(EntropyLayerSettings source, bool gdsOnly)
    {
        adjustSettings(source, gdsOnly);
    }

    public bool nonGaussianValues()
    {
        return pNonGaussianValues();
    }

    private bool pNonGaussianValues()
    {
        bool gaussianvalues = horOverlay_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && verOverlay_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && innerCV_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && outerCV_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && sideCDU_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && tipsCDU_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && horTipBiasNVar_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && horTipBiasPVar_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && verTipBiasNVar_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && verTipBiasPVar_RNGMapping == CommonVars.boxMuller;
        gaussianvalues = gaussianvalues && wobble_RNGMapping == CommonVars.boxMuller;

        return !gaussianvalues;
    }
}
