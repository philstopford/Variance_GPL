using System;
using System.Linq;
using Clipper2Lib;
using shapeEngine;

namespace Variance;

[Serializable]
public class EntropyLayerSettings : ShapeSettings
{
    private const string default_comment = "";
    private readonly int[] default_bgLayers = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    private const int default_showDrawn = 0;

    private static readonly string default_rngmapping = CommonVars.boxMuller;

    private const decimal default_wobble = 0;
    private const decimal default_innerCV = 0;
    private const decimal default_outerCV = 0;
    private const int default_correlatedLWR = 0;
    private const int default_correlatedLWRLayerIndex = -1;
    private const decimal default_sideCDU = 0;
    private const decimal default_tipsCDU = 0;
    private const decimal default_horOverlay = 0;
    private const decimal default_verOverlay = 0;

    private const int default_correlatedTipCDU = 0;
    private const int default_correlatedTipCDULayerIndex = -1;
    private const int default_correlatedCDU = 0;
    private const int default_correlatedCDULayerIndex = -1;
    private const int default_correlatedXOverlay = 0;
    private const int default_correlatedXOverlayLayerIndex = -1;
    private const int default_correlatedYOverlay = 0;
    private const int default_correlatedYOverlayLayerIndex = -1;
    private const int default_overlayXReferenceLayer = -1;
    private const int default_overlayYReferenceLayer = -1;
    private const int default_averageOverlayX = 0;
    private const int default_averageOverlayY = 0;
    private readonly int[] default_overlayXReferenceLayers = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    private readonly int[] default_overlayYReferenceLayers = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    private const int default_fileType = 0; // layout (now exposed by geoCore - could be redundant)
    private readonly PathsD default_fileData = [new PathD { new PointD(0, 0) }];
    private const string default_fileToLoad = "";
    private const string default_ldNameFromFile = "";
    private const string default_structureNameFromFile = "";
    private const int default_ldFromFile = 0;
    private const int default_polyFill = (int)PolyFill.pftNonZero; // non-zero
    private const int default_structureFromFile = 0;
    private const int default_perPoly = 0;
    private const int default_referenceLayout = 0;

    private const decimal default_lensDistortionCoeff1 = 0;
    private const decimal default_lensDistortionCoeff2 = 0;

    private const int default_booleanLayerA = -1;
    private const int default_booleanLayerB = -1;
    private const int default_booleanLayerOpA = 0;
    private const int default_booleanLayerOpB = 0;
    private const int default_booleanLayerOpAB = 0;

    private const int default_omitLayer = 0;

    private const int default_LWRNoisePreview = 0;

    private const int default_removeArtifacts = 0;
    private const int default_removeArtifactsEpsilon = 100;

    private const decimal default_uniBias = 0;
    private const int default_uniBiasAxis = 0;
    private const int default_uniBiasAfterRotation = 0;
    
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
        int[] ret = p switch
        {
            properties_intarray.bglayers => bgLayers,
            properties_intarray.xOLRefs => overlayXReferenceLayers,
            properties_intarray.yOLRefs => overlayYReferenceLayers,
            _ => []
        };

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
        int ret = p switch
        {
            properties_intarray.bglayers => bgLayers[index],
            properties_intarray.xOLRefs => overlayXReferenceLayers[index],
            properties_intarray.yOLRefs => overlayYReferenceLayers[index],
            _ => 0
        };

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
    private int uniBiasAxis;
    private int uniBiasAfterRotation;
    
    public new enum properties_i
    {
        enabled, omit, showDrawn, shapeIndex, shape0Tip, shape1Tip, shape2Tip, subShapeIndex, posIndex, proxRays, proxSideRaysFallOff, edgeSlide, 
        lwrType, lwr2Type, lwrPreview, lwr_corr, lwr_corr_ref, lwr2_corr, lwr2_corr_ref,
        xOL_corr, xOL_corr_ref, yOL_corr, yOL_corr_ref, CDU_corr, CDU_corr_ref, tCDU_corr, tCDU_corr_ref,
        xOL_ref, yOL_ref, xOL_av, yOL_av,
        flipH, flipV, alignX, alignY, structure, lD, fill, fileType, perPoly, refLayout,
        bLayerA, bLayerB, bLayerOpA, bLayerOpB, bLayerOpAB,
        removeArtifacts, removeArtifactsEpsilon,
        uniBiasAxis, uniBiasAfterRotation,
        legacyRounding
    }

    public int getInt(properties_i p)
    {
        return pGetInt(p);
    }

    private int pGetInt(properties_i p)
    {
        int ret = p switch
        {
            properties_i.enabled => getInt(ShapeSettings.properties_i.enabled),
            properties_i.shapeIndex => getInt(ShapeSettings.properties_i.shapeIndex),
            properties_i.shape0Tip => getInt(ShapeSettings.properties_i.subShapeTipLocIndex),
            properties_i.shape1Tip => getInt(ShapeSettings.properties_i.subShape2TipLocIndex),
            properties_i.shape2Tip => getInt(ShapeSettings.properties_i.subShape3TipLocIndex),
            properties_i.subShapeIndex => getInt(ShapeSettings.properties_i.subShapeRefIndex),
            properties_i.posIndex => getInt(ShapeSettings.properties_i.posInSubShapeIndex),
            properties_i.edgeSlide => getInt(ShapeSettings.properties_i.edgeSlide),
            properties_i.proxRays => getInt(ShapeSettings.properties_i.proxRays),
            properties_i.proxSideRaysFallOff => getInt(ShapeSettings.properties_i.proxSideRaysFallOff),
            properties_i.lwrType => getInt(ShapeSettings.properties_i.lwrType),
            properties_i.lwr2Type => getInt(ShapeSettings.properties_i.lwr2Type),
            properties_i.flipH => getInt(ShapeSettings.properties_i.flipH),
            properties_i.flipV => getInt(ShapeSettings.properties_i.flipV),
            properties_i.alignX => getInt(ShapeSettings.properties_i.alignX),
            properties_i.alignY => getInt(ShapeSettings.properties_i.alignY),
            properties_i.legacyRounding => getInt(ShapeSettings.properties_i.legacyRounding),
            properties_i.omit => omitFromSim,
            properties_i.showDrawn => showDrawn,
            properties_i.lwr_corr => correlatedLWR,
            properties_i.lwr_corr_ref => correlatedLWRLayerIndex,
            properties_i.lwr2_corr => correlatedLWR2,
            properties_i.lwr2_corr_ref => correlatedLWR2LayerIndex,
            properties_i.lwrPreview => LWRNoisePreview,
            properties_i.xOL_ref => overlayXReferenceLayer,
            properties_i.yOL_ref => overlayYReferenceLayer,
            properties_i.xOL_av => averageOverlayX,
            properties_i.yOL_av => averageOverlayY,
            properties_i.xOL_corr => correlatedXOverlay,
            properties_i.xOL_corr_ref => correlatedXOverlayLayerIndex,
            properties_i.yOL_corr => correlatedYOverlay,
            properties_i.yOL_corr_ref => correlatedYOverlayLayerIndex,
            properties_i.CDU_corr => correlatedCDU,
            properties_i.CDU_corr_ref => correlatedCDULayerIndex,
            properties_i.tCDU_corr => correlatedTipCDU,
            properties_i.tCDU_corr_ref => correlatedTipCDULayerIndex,
            properties_i.structure => structureFromFile,
            properties_i.lD => ldFromFile,
            properties_i.fill => polyFill,
            properties_i.fileType => fileType,
            properties_i.perPoly => perPoly,
            properties_i.refLayout => referenceLayout,
            properties_i.bLayerA => booleanLayerA,
            properties_i.bLayerB => booleanLayerB,
            properties_i.bLayerOpA => booleanLayerOpA,
            properties_i.bLayerOpB => booleanLayerOpB,
            properties_i.bLayerOpAB => booleanLayerOpAB,
            properties_i.removeArtifacts => removeArtifacts,
            properties_i.removeArtifactsEpsilon => removeArtifactsEpsilon,
            properties_i.uniBiasAxis => uniBiasAxis,
            properties_i.uniBiasAfterRotation => uniBiasAfterRotation,
            _ => 0
        };

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
            case properties_i.legacyRounding:
                setInt(ShapeSettings.properties_i.legacyRounding, val);
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
            case properties_i.uniBiasAxis:
                uniBiasAxis = val;
                break;
            case properties_i.uniBiasAfterRotation:
                uniBiasAfterRotation = val;
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
            case properties_i.legacyRounding:
                defaultInt(ShapeSettings.properties_i.legacyRounding);
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
            case properties_i.uniBiasAxis:
                uniBiasAxis = default_uniBiasAxis;
                break;
            case properties_i.uniBiasAfterRotation:
                uniBiasAfterRotation = default_uniBiasAfterRotation;
                break;
        }
    }

    public static int getDefaultInt(properties_i p)
    {
        return pGetDefaultInt(p);
    }

    private static int pGetDefaultInt(properties_i p)
    {
        int ret = p switch
        {
            properties_i.enabled => getDefaultInt(ShapeSettings.properties_i.enabled),
            properties_i.shapeIndex => getDefaultInt(ShapeSettings.properties_i.shapeIndex),
            properties_i.shape0Tip => getDefaultInt(ShapeSettings.properties_i.subShapeTipLocIndex),
            properties_i.shape1Tip => getDefaultInt(ShapeSettings.properties_i.subShape2TipLocIndex),
            properties_i.shape2Tip => getDefaultInt(ShapeSettings.properties_i.subShape3TipLocIndex),
            properties_i.subShapeIndex => getDefaultInt(ShapeSettings.properties_i.subShapeRefIndex),
            properties_i.posIndex => getDefaultInt(ShapeSettings.properties_i.posInSubShapeIndex),
            properties_i.edgeSlide => getDefaultInt(ShapeSettings.properties_i.edgeSlide),
            properties_i.proxRays => getDefaultInt(ShapeSettings.properties_i.proxRays),
            properties_i.proxSideRaysFallOff => getDefaultInt(ShapeSettings.properties_i.proxSideRaysFallOff),
            properties_i.lwrType or properties_i.lwr2Type => getDefaultInt(ShapeSettings.properties_i.lwrType),
            properties_i.flipH => getDefaultInt(ShapeSettings.properties_i.flipH),
            properties_i.flipV => getDefaultInt(ShapeSettings.properties_i.flipV),
            properties_i.alignX => getDefaultInt(ShapeSettings.properties_i.alignX),
            properties_i.alignY => getDefaultInt(ShapeSettings.properties_i.alignY),
            properties_i.legacyRounding => getDefaultInt(ShapeSettings.properties_i.legacyRounding),
            properties_i.omit => default_omitLayer,
            properties_i.showDrawn => default_showDrawn,
            properties_i.lwrPreview => default_LWRNoisePreview,
            properties_i.lwr_corr or properties_i.lwr2_corr => default_correlatedLWR,
            properties_i.lwr_corr_ref or properties_i.lwr2_corr_ref => default_correlatedLWRLayerIndex,
            properties_i.xOL_ref => default_overlayXReferenceLayer,
            properties_i.yOL_ref => default_overlayYReferenceLayer,
            properties_i.xOL_av => default_averageOverlayX,
            properties_i.yOL_av => default_averageOverlayY,
            properties_i.xOL_corr => default_correlatedXOverlay,
            properties_i.xOL_corr_ref => default_correlatedXOverlayLayerIndex,
            properties_i.yOL_corr => default_correlatedYOverlay,
            properties_i.yOL_corr_ref => default_correlatedYOverlayLayerIndex,
            properties_i.CDU_corr => default_correlatedCDU,
            properties_i.CDU_corr_ref => default_correlatedCDULayerIndex,
            properties_i.tCDU_corr => default_correlatedTipCDU,
            properties_i.tCDU_corr_ref => default_correlatedTipCDULayerIndex,
            properties_i.structure => default_structureFromFile,
            properties_i.lD => default_ldFromFile,
            properties_i.fill => default_polyFill,
            properties_i.fileType => default_fileType,
            properties_i.perPoly => default_perPoly,
            properties_i.refLayout => default_referenceLayout,
            properties_i.bLayerA => default_booleanLayerA,
            properties_i.bLayerB => default_booleanLayerB,
            properties_i.bLayerOpA => default_booleanLayerOpA,
            properties_i.bLayerOpB => default_booleanLayerOpB,
            properties_i.bLayerOpAB => default_booleanLayerOpAB,
            properties_i.removeArtifacts => default_removeArtifacts,
            properties_i.removeArtifactsEpsilon => default_removeArtifactsEpsilon,
            properties_i.uniBiasAxis => default_uniBiasAxis,
            properties_i.uniBiasAfterRotation => default_uniBiasAfterRotation,
            _ => 0
        };

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
        string ret = p switch
        {
            properties_s.comment => comment,
            properties_s.name => getString(ShapeSettings.properties_s.s_name),
            properties_s.hTipNVar_RNG => horTipBiasNVar_RNGMapping,
            properties_s.hTipPVar_RNG => horTipBiasPVar_RNGMapping,
            properties_s.vTipNVar_RNG => verTipBiasNVar_RNGMapping,
            properties_s.vTipPVar_RNG => verTipBiasPVar_RNGMapping,
            properties_s.iCV_RNG => innerCV_RNGMapping,
            properties_s.oCV_RNG => outerCV_RNGMapping,
            properties_s.lwr_RNG => LWR_RNGMapping,
            properties_s.lwr2_RNG => LWR2_RNGMapping,
            properties_s.sCDU_RNG => sideCDU_RNGMapping,
            properties_s.tCDU_RNG => tipsCDU_RNGMapping,
            properties_s.xOL_RNG => horOverlay_RNGMapping,
            properties_s.yOL_RNG => verOverlay_RNGMapping,
            properties_s.wobble_RNG => wobble_RNGMapping,
            properties_s.file => fileToLoad,
            properties_s.structure => structureNameFromFile,
            properties_s.lD => ldNameFromFile,
            _ => ""
        };

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
        string ret = p switch
        {
            properties_s.comment => default_comment,
            properties_s.name => getDefaultString(ShapeSettings.properties_s.s_name),
            properties_s.hTipNVar_RNG or properties_s.hTipPVar_RNG or properties_s.vTipNVar_RNG
                or properties_s.vTipPVar_RNG or properties_s.iCV_RNG or properties_s.oCV_RNG or properties_s.lwr_RNG
                or properties_s.lwr2_RNG or properties_s.sCDU_RNG or properties_s.tCDU_RNG or properties_s.xOL_RNG
                or properties_s.yOL_RNG or properties_s.wobble_RNG => default_rngmapping,
            properties_s.file => default_fileToLoad,
            properties_s.structure => default_structureNameFromFile,
            properties_s.lD => default_ldNameFromFile,
            _ => ""
        };

        return ret;
    }

    private decimal wobble;
    private decimal totalRotation;
    private decimal lensDistortionCoeff1;
    private decimal lensDistortionCoeff2;
    private decimal innerCV;
    private decimal outerCV;
    private decimal sideCDU;
    private decimal tipsCDU;
    private decimal horOverlay;
    private decimal verOverlay;

    private decimal uniBias;

    public new enum properties_decimal
    {
        horLength, verLength, horOffset, verOffset,
        gHorOffset, gVerOffset,
        rot, wobble, totalRotation,
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
        uniBias
    }

    public decimal getDecimal(properties_decimal p, int _subShapeRef = -1)
    {
        return pGetDecimal(p, _subShapeRef);
    }

    private decimal pGetDecimal(properties_decimal p, int _subShapeRef)
    {
        decimal ret = p switch
        {
            properties_decimal.horLength => getDecimal(ShapeSettings.properties_decimal.horLength, _subShapeRef),
            properties_decimal.verLength => getDecimal(ShapeSettings.properties_decimal.verLength, _subShapeRef),
            properties_decimal.horOffset => getDecimal(ShapeSettings.properties_decimal.horOffset, _subShapeRef),
            properties_decimal.verOffset => getDecimal(ShapeSettings.properties_decimal.verOffset, _subShapeRef),
            properties_decimal.gHorOffset => getDecimal(ShapeSettings.properties_decimal.gHorOffset),
            properties_decimal.gVerOffset => getDecimal(ShapeSettings.properties_decimal.gVerOffset),
            properties_decimal.iCR => getDecimal(ShapeSettings.properties_decimal.iCR),
            properties_decimal.oCR => getDecimal(ShapeSettings.properties_decimal.oCR),
            properties_decimal.sBias => getDecimal(ShapeSettings.properties_decimal.sBias),
            properties_decimal.hTBias => getDecimal(ShapeSettings.properties_decimal.hTBias),
            properties_decimal.hTNVar => getDecimal(ShapeSettings.properties_decimal.hTNVar),
            properties_decimal.hTPVar => getDecimal(ShapeSettings.properties_decimal.hTPVar),
            properties_decimal.vTBias => getDecimal(ShapeSettings.properties_decimal.vTBias),
            properties_decimal.vTNVar => getDecimal(ShapeSettings.properties_decimal.vTNVar),
            properties_decimal.vTPVar => getDecimal(ShapeSettings.properties_decimal.vTPVar),
            properties_decimal.lwr => getDecimal(ShapeSettings.properties_decimal.lwr),
            properties_decimal.lwrFreq => getDecimal(ShapeSettings.properties_decimal.lwrFreq),
            properties_decimal.lwr2 => getDecimal(ShapeSettings.properties_decimal.lwr2),
            properties_decimal.lwr2Freq => getDecimal(ShapeSettings.properties_decimal.lwr2Freq),
            properties_decimal.eTension => getDecimal(ShapeSettings.properties_decimal.eTension),
            properties_decimal.rot => getDecimal(ShapeSettings.properties_decimal.rot),
            properties_decimal.pBias => getDecimal(ShapeSettings.properties_decimal.pBias),
            properties_decimal.pBiasDist => getDecimal(ShapeSettings.properties_decimal.pBiasDist),
            properties_decimal.proxSideRaysMultiplier => getDecimal(ShapeSettings.properties_decimal
                .proxSideRaysMultiplier),
            properties_decimal.rayExtension => getDecimal(ShapeSettings.properties_decimal.rayExtension),
            properties_decimal.keyhole_factor => getDecimal(ShapeSettings.properties_decimal.keyhole_factor),
            properties_decimal.wobble => wobble,
            properties_decimal.totalRotation => totalRotation,
            properties_decimal.lDC1 => lensDistortionCoeff1,
            properties_decimal.lDC2 => lensDistortionCoeff2,
            properties_decimal.iCV => innerCV,
            properties_decimal.oCV => outerCV,
            properties_decimal.sCDU => sideCDU,
            properties_decimal.tCDU => tipsCDU,
            properties_decimal.xOL => horOverlay,
            properties_decimal.yOL => verOverlay,
            properties_decimal.uniBias => uniBias,
            _ => 0m
        };

        return ret;
    }

    public static decimal getDefaultDecimal(properties_decimal p, int _subShapeRef = -1)
    {
        return pGetDefaultDecimal(p, _subShapeRef);
    }

    private static decimal pGetDefaultDecimal(properties_decimal p, int _subShapeRef)
    {
        decimal ret = p switch
        {
            properties_decimal.horLength => getDefaultDecimal(ShapeSettings.properties_decimal.horLength, _subShapeRef),
            properties_decimal.verLength => getDefaultDecimal(ShapeSettings.properties_decimal.verLength, _subShapeRef),
            properties_decimal.horOffset => getDefaultDecimal(ShapeSettings.properties_decimal.horOffset, _subShapeRef),
            properties_decimal.verOffset => getDefaultDecimal(ShapeSettings.properties_decimal.verOffset, _subShapeRef),
            properties_decimal.gHorOffset => getDefaultDecimal(ShapeSettings.properties_decimal.gHorOffset),
            properties_decimal.gVerOffset => getDefaultDecimal(ShapeSettings.properties_decimal.gVerOffset),
            properties_decimal.iCR => getDefaultDecimal(ShapeSettings.properties_decimal.iCR),
            properties_decimal.oCR => getDefaultDecimal(ShapeSettings.properties_decimal.oCR),
            properties_decimal.sBias => getDefaultDecimal(ShapeSettings.properties_decimal.sBias),
            properties_decimal.hTBias => getDefaultDecimal(ShapeSettings.properties_decimal.hTBias),
            properties_decimal.hTNVar => getDefaultDecimal(ShapeSettings.properties_decimal.hTNVar),
            properties_decimal.hTPVar => getDefaultDecimal(ShapeSettings.properties_decimal.hTPVar),
            properties_decimal.vTBias => getDefaultDecimal(ShapeSettings.properties_decimal.vTBias),
            properties_decimal.vTNVar => getDefaultDecimal(ShapeSettings.properties_decimal.vTNVar),
            properties_decimal.vTPVar => getDefaultDecimal(ShapeSettings.properties_decimal.vTPVar),
            properties_decimal.lwr or properties_decimal.lwr2 =>
                getDefaultDecimal(ShapeSettings.properties_decimal.lwr),
            properties_decimal.lwrFreq or properties_decimal.lwr2Freq => getDefaultDecimal(ShapeSettings
                .properties_decimal.lwrFreq),
            properties_decimal.eTension => getDefaultDecimal(ShapeSettings.properties_decimal.eTension),
            properties_decimal.rot or properties_decimal.totalRotation => getDefaultDecimal(ShapeSettings
                .properties_decimal.rot),
            properties_decimal.pBias => getDefaultDecimal(ShapeSettings.properties_decimal.pBias),
            properties_decimal.pBiasDist => getDefaultDecimal(ShapeSettings.properties_decimal.pBiasDist),
            properties_decimal.proxSideRaysMultiplier => getDefaultDecimal(ShapeSettings.properties_decimal
                .proxSideRaysMultiplier),
            properties_decimal.rayExtension => getDefaultDecimal(ShapeSettings.properties_decimal.rayExtension),
            properties_decimal.keyhole_factor => getDefaultDecimal(ShapeSettings.properties_decimal.keyhole_factor),
            properties_decimal.wobble => default_wobble,
            properties_decimal.lDC1 => default_lensDistortionCoeff1,
            properties_decimal.lDC2 => default_lensDistortionCoeff2,
            properties_decimal.iCV => default_innerCV,
            properties_decimal.oCV => default_outerCV,
            properties_decimal.sCDU => default_sideCDU,
            properties_decimal.tCDU => default_tipsCDU,
            properties_decimal.xOL => default_horOverlay,
            properties_decimal.yOL => default_verOverlay,
            properties_decimal.uniBias => 0,
            _ => 0m
        };

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
            case properties_decimal.totalRotation:
                totalRotation = val;
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
            case properties_decimal.uniBias:
                uniBias = val;
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
            case properties_decimal.totalRotation:
                defaultDecimal(properties_decimal.totalRotation);
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
            case properties_decimal.uniBias:
                uniBias = 0;
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
        fileData = new PathsD(newdata);
    }

    public void defaultFileData()
    {
        pDefaultFileData();
    }

    private void pDefaultFileData()
    {
        fileData = new PathsD(default_fileData);
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
        totalRotation = default_rotation;
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

        uniBias = default_uniBias;
        uniBiasAxis = default_uniBiasAxis;
        uniBiasAfterRotation = default_uniBiasAfterRotation;

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
            totalRotation = source.totalRotation;
            setDecimal(ShapeSettings.properties_decimal.sBias, source.getDecimal(ShapeSettings.properties_decimal.sBias));
            uniBias = source.uniBias;
            uniBiasAxis = source.uniBiasAxis;
            uniBiasAfterRotation = source.uniBiasAfterRotation;

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
            setInt(ShapeSettings.properties_i.legacyRounding, source.getInt(ShapeSettings.properties_i.legacyRounding));

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
        fileData = new PathsD(source.fileData);
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
