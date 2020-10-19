using geoLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Variance
{
    [Serializable]
    public class EntropyLayerSettings
    {
        static string default_comment = "";
        static Int32[] default_bgLayers = new Int32[CentralProperties.maxLayersForMC] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static Int32 default_enabled = 0;
        static Int32 default_geoCoreShapeEngine = 0;
        static Int32 default_showDrawn = 0;
        static Int32 default_alignGeom = 0;
        static string default_layerName = "";
        static Int32 default_shapeIndex = (Int32)CentralProperties.typeShapes.none;

        static decimal default_subShapeHorLength = 0;
        static decimal default_subShapeHorOffset = 0;
        static decimal default_subShapeVerLength = 0;
        static decimal default_subShapeVerOffset = 0;
        static Int32 default_subShapeTipLocIndex = 0;
        static decimal default_subShape2HorLength = 0;
        static decimal default_subShape2HorOffset = 0;
        static decimal default_subShape2VerLength = 0;
        static decimal default_subShape2VerOffset = 0;
        static Int32 default_subShape2TipLocIndex = 0;
        static decimal default_subShape3HorLength = 0;
        static decimal default_subShape3HorOffset = 0;
        static decimal default_subShape3VerLength = 0;
        static decimal default_subShape3VerOffset = 0;
        static Int32 default_subShape3TipLocIndex = 0;
        static Int32 default_subShapeRefIndex = 0;
        static Int32 default_posInSubShapeIndex = (int)CommonVars.subShapeLocations.BL;

        static decimal default_globalHorOffset = 0;
        static decimal default_globalVerOffset = 0;

        static decimal default_sideBias = 0;
        static decimal default_horTipBias = 0;
        static string default_rngmapping = CommonVars.boxMuller;
        static decimal default_horTipBiasNVar = 0;
        static decimal default_horTipBiasPVar = 0;
        static decimal default_verTipBias = 0;
        static decimal default_verTipBiasNVar = 0;
        static decimal default_verTipBiasPVar = 0;
        static decimal default_rotation = 0;
        static decimal default_proximityBias = 0;
        static decimal default_proximityIsoDistance = 0;
        static Int32 default_proximitySideRays = 2;
        static Int32 default_proximitySideRaysFallOff = 0;
        static decimal default_proximitySideRaysFallOffMultiplier = 1.0m;

        static Int32 default_edgeSlide = 0;
        static decimal default_edgeSlideTension = 0.35m;

        static decimal default_wobble = 0;
        static decimal default_innerCRR = 0;
        static decimal default_innerCV = 0;
        static decimal default_outerCRR = 0;
        static decimal default_outerCV = 0;
        static decimal default_LWR = 0;
        static Int32 default_LWRNoiseType = (Int32)CommonVars.noiseIndex.perlin;
        static Int32 default_correlatedLWR = 0;
        static Int32 default_correlatedLWRLayerIndex = -1;
        static Int32 default_LWRNoisePreview = 0;
        static decimal default_LWRNoiseFreq = 0.2m;
        static decimal default_sideCDU = 0;
        static decimal default_tipsCDU = 0;
        static decimal default_horOverlay = 0;
        static decimal default_verOverlay = 0;

        static Int32 default_correlatedTipCDU = 0;
        static Int32 default_correlatedTipCDULayerIndex = -1;
        static Int32 default_correlatedCDU = 0;
        static Int32 default_correlatedCDULayerIndex = -1;
        static Int32 default_correlatedXOverlay = 0;
        static Int32 default_correlatedXOverlayLayerIndex = -1;
        static Int32 default_correlatedYOverlay = 0;
        static Int32 default_correlatedYOverlayLayerIndex = -1;
        static Int32 default_overlayXReferenceLayer = -1;
        static Int32 default_overlayYReferenceLayer = -1;
        static Int32 default_averageOverlayX = 0;
        static Int32 default_averageOverlayY = 0;
        static Int32[] default_overlayXReferenceLayers = new Int32[CentralProperties.maxLayersForMC] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static Int32[] default_overlayYReferenceLayers = new Int32[CentralProperties.maxLayersForMC] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        static Int32 default_flipH = 0;
        static Int32 default_flipV = 0;

        static Int32 default_fileType = 0; // layout (now exposed by geoCore - could be redundant)
        static List<GeoLibPointF[]> default_fileData = new List<GeoLibPointF[]> { new GeoLibPointF[] { new GeoLibPointF(0, 0) } };
        static string default_fileToLoad = "";
        static string default_ldNameFromFile = "";
        static string default_structureNameFromFile = "";
        static Int32 default_ldFromFile = 0;
        static Int32 default_polyFill = (Int32)CommonVars.PolyFill.pftNonZero; // non-zero
        static Int32 default_structureFromFile = 0;
        static Int32 default_perPoly = 0;
        static Int32 default_referenceLayout = 0;

        static decimal default_lensDistortionCoeff1 = 0;
        static decimal default_lensDistortionCoeff2 = 0;

        static Int32 default_booleanLayerA = -1;
        static Int32 default_booleanLayerB = -1;
        static Int32 default_booleanLayerOpA = 0;
        static Int32 default_booleanLayerOpB = 0;
        static Int32 default_booleanLayerOpAB = 0;

        static Int32 default_omitLayer = 0;

        [NonSerialized]
        Int32[] bgLayers;

        Int32[] overlayXReferenceLayers;
        Int32[] overlayYReferenceLayers;

        public enum properties_intarray
        {
            bglayers, xOLRefs, yOLRefs
        }

        public bool isLayerActive()
        {
            return pIsLayerActive();
        }

        bool pIsLayerActive()
        {
            return ((enabled == 1) && (omitFromSim == 0));
        }

        public Int32[] getIntArray(properties_intarray p)
        {
            return pGetIntArray(p);
        }

        Int32[] pGetIntArray(properties_intarray p)
        {
            Int32[] ret = new Int32[] { };
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

        public void setIntArray(properties_intarray p, Int32[] val)
        {
            pSetIntArray(p, val);
        }

        void pSetIntArray(properties_intarray p, Int32[] val)
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

        void pDefaultIntArray(properties_intarray p)
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

        int pGetIntArrayValue(properties_intarray p, int index)
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

        void pSetIntArrayValue(properties_intarray p, int index, int val)
        {
            switch (p)
            {
                case properties_intarray.bglayers:
                    bgLayers[index] = val; ;
                    break;
                case properties_intarray.xOLRefs:
                    overlayXReferenceLayers[index] = val;
                    break;
                case properties_intarray.yOLRefs:
                    overlayYReferenceLayers[index] = val;
                    break;
            }
        }

        Int32 enabled;
        Int32 omitFromSim; // prevent layer being included in simulation.
        Int32 geoCoreShapeEngine;

        [NonSerialized]
        Int32 showDrawn;

        Int32 shapeIndex;
        Int32 subShapeTipLocIndex;
        Int32 subShape2TipLocIndex;
        Int32 subShape3TipLocIndex;
        Int32 subShapeRefIndex;
        Int32 posInSubShapeIndex;
        Int32 proximitySideRays;
        Int32 edgeSlide;
        Int32 LWRNoiseType;
        Int32 LWR2NoiseType;
        Int32 correlatedLWR;
        Int32 correlatedLWRLayerIndex;
        Int32 correlatedLWR2;
        Int32 correlatedLWR2LayerIndex;

        [NonSerialized]
        Int32 LWRNoisePreview;

        Int32 correlatedXOverlay;
        Int32 correlatedXOverlayLayerIndex;
        Int32 correlatedYOverlay;
        Int32 correlatedYOverlayLayerIndex;
        Int32 correlatedCDU;
        Int32 correlatedCDULayerIndex;
        Int32 correlatedTipCDU;
        Int32 correlatedTipCDULayerIndex;
        Int32 overlayXReferenceLayer;
        Int32 overlayYReferenceLayer;
        Int32 averageOverlayX;
        Int32 averageOverlayY;
        Int32 flipH;
        Int32 flipV;
        Int32 alignGeomX, alignGeomY;
        Int32 structureFromFile;
        Int32 ldFromFile;
        Int32 polyFill;
        Int32 fileType; // holds the value of the file type selected (0 == GDS, 1 == Oasis). Might be redundant with geoCore.
        Int32 perPoly;
        Int32 referenceLayout;
        Int32 booleanLayerA;
        Int32 booleanLayerB;
        Int32 booleanLayerOpA;
        Int32 booleanLayerOpB;
        Int32 booleanLayerOpAB;

        Int32 proxSideRaysFallOff;

        public enum properties_i
        {
            enabled, omit, gCSEngine, showDrawn, shapeIndex, shape0Tip, shape1Tip, shape2Tip, subShapeIndex, posIndex, proxRays, proxSideRaysFallOff, edgeSlide, 
            lwrType, lwr2Type, lwrPreview, lwr_corr, lwr_corr_ref, lwr2_corr, lwr2_corr_ref,
            xOL_corr, xOL_corr_ref, yOL_corr, yOL_corr_ref, CDU_corr, CDU_corr_ref, tCDU_corr, tCDU_corr_ref,
            xOL_ref, yOL_ref, xOL_av, yOL_av,
            flipH, flipV, alignX, alignY, structure, lD, fill, fileType, perPoly, refLayout,
            bLayerA, bLayerB, bLayerOpA, bLayerOpB, bLayerOpAB
        }

        public Int32 getInt(properties_i p)
        {
            return pGetInt(p);
        }

        Int32 pGetInt(properties_i p)
        {
            int ret = 0;
            switch (p)
            {
                case properties_i.enabled:
                    ret = enabled;
                    break;
                case properties_i.omit:
                    ret = omitFromSim;
                    break;
                case properties_i.gCSEngine:
                    ret = geoCoreShapeEngine;
                    break;
                case properties_i.showDrawn:
                    ret = showDrawn;
                    break;
                case properties_i.shapeIndex:
                    ret = shapeIndex;
                    break;
                case properties_i.shape0Tip:
                    ret = subShapeTipLocIndex;
                    break;
                case properties_i.shape1Tip:
                    ret = subShape2TipLocIndex;
                    break;
                case properties_i.shape2Tip:
                    ret = subShape3TipLocIndex;
                    break;
                case properties_i.subShapeIndex:
                    ret = subShapeRefIndex;
                    break;
                case properties_i.posIndex:
                    ret = posInSubShapeIndex;
                    break;
                case properties_i.proxRays:
                    ret = proximitySideRays;
                    break;
                case properties_i.edgeSlide:
                    ret = edgeSlide;
                    break;
                case properties_i.lwrType:
                    ret = LWRNoiseType;
                    break;
                case properties_i.lwr2Type:
                    ret = LWR2NoiseType;
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
                case properties_i.alignX:
                    ret = alignGeomX;
                    break;
                case properties_i.alignY:
                    ret = alignGeomY;
                    break;
                case properties_i.flipH:
                    ret = flipH;
                    break;
                case properties_i.flipV:
                    ret = flipV;
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
                case properties_i.proxSideRaysFallOff:
                    ret = proxSideRaysFallOff;
                    break;
            }

            return ret;
        }

        public void setInt(properties_i p, int val)
        {
            pSetInt(p, val);
        }

        void pSetInt(properties_i p, int val)
        {
            switch (p)
            {
                case properties_i.enabled:
                    enabled = val;
                    break;
                case properties_i.omit:
                    omitFromSim = val;
                    break;
                case properties_i.gCSEngine:
                    geoCoreShapeEngine = val;
                    break;
                case properties_i.showDrawn:
                    showDrawn = val;
                    break;
                case properties_i.shapeIndex:
                    shapeIndex = val;
                    break;
                case properties_i.shape0Tip:
                    subShapeTipLocIndex = val;
                    break;
                case properties_i.shape1Tip:
                    subShape2TipLocIndex = val;
                    break;
                case properties_i.shape2Tip:
                    subShape3TipLocIndex = val;
                    break;
                case properties_i.subShapeIndex:
                    subShapeRefIndex = val;
                    break;
                case properties_i.posIndex:
                    posInSubShapeIndex = val;
                    break;
                case properties_i.proxRays:
                    proximitySideRays = val;
                    break;
                case properties_i.edgeSlide:
                    edgeSlide = val;
                    break;
                case properties_i.lwrType:
                    LWRNoiseType = val;
                    break;
                case properties_i.lwr2Type:
                    LWR2NoiseType = val;
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
                case properties_i.alignX:
                    alignGeomX = val;
                    break;
                case properties_i.alignY:
                    alignGeomY = val;
                    break;
                case properties_i.flipH:
                    flipH = val;
                    break;
                case properties_i.flipV:
                    flipV = val;
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
                case properties_i.proxSideRaysFallOff:
                    proxSideRaysFallOff = val;
                    break;
            }
        }

        public void defaultInt(properties_i p)
        {
            pDefaultInt(p);
        }

        void pDefaultInt(properties_i p)
        {
            switch (p)
            {
                case properties_i.alignX:
                case properties_i.alignY:
                    alignGeomY = default_alignGeom;
                    break;
                case properties_i.CDU_corr:
                    correlatedCDU = default_correlatedCDU;
                    break;
                case properties_i.CDU_corr_ref:
                    correlatedCDULayerIndex = default_correlatedCDULayerIndex;
                    break;
                case properties_i.edgeSlide:
                    edgeSlide = default_edgeSlide;
                    break;
                case properties_i.enabled:
                    enabled = default_enabled;
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
                case properties_i.flipH:
                    flipH = default_flipH;
                    break;
                case properties_i.flipV:
                    flipV = default_flipV;
                    break;
                case properties_i.gCSEngine:
                    geoCoreShapeEngine = default_geoCoreShapeEngine;
                    break;
                case properties_i.lD:
                    ldFromFile = default_ldFromFile;
                    break;
                case properties_i.lwrPreview:
                    LWRNoisePreview = default_LWRNoisePreview;
                    break;
                case properties_i.lwrType:
                    LWRNoiseType = default_LWRNoiseType;
                    break;
                case properties_i.lwr2Type:
                    LWR2NoiseType = default_LWRNoiseType;
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
                case properties_i.posIndex:
                    posInSubShapeIndex = default_posInSubShapeIndex;
                    break;
                case properties_i.proxRays:
                    proximitySideRays = default_proximitySideRays;
                    break;
                case properties_i.refLayout:
                    referenceLayout = default_referenceLayout;
                    break;
                case properties_i.shape0Tip:
                    subShapeTipLocIndex = default_subShapeTipLocIndex;
                    break;
                case properties_i.shape1Tip:
                    subShape2TipLocIndex = default_subShape2TipLocIndex;
                    break;
                case properties_i.shape2Tip:
                    subShape3TipLocIndex = default_subShape3TipLocIndex;
                    break;
                case properties_i.shapeIndex:
                    shapeIndex = default_shapeIndex;
                    break;
                case properties_i.showDrawn:
                    showDrawn = default_showDrawn;
                    break;
                case properties_i.structure:
                    structureFromFile = default_structureFromFile;
                    break;
                case properties_i.subShapeIndex:
                    posInSubShapeIndex = default_posInSubShapeIndex;
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
                case properties_i.proxSideRaysFallOff:
                    proxSideRaysFallOff = default_proximitySideRaysFallOff;
                    break;
            }
        }

        public static Int32 getDefaultInt(properties_i p)
        {
            return pGetDefaultInt(p);
        }

        static Int32 pGetDefaultInt(properties_i p)
        {
            int ret = 0;
            switch (p)
            {
                case properties_i.enabled:
                    ret = default_enabled;
                    break;
                case properties_i.omit:
                    ret = default_omitLayer;
                    break;
                case properties_i.gCSEngine:
                    ret = default_geoCoreShapeEngine;
                    break;
                case properties_i.showDrawn:
                    ret = default_showDrawn;
                    break;
                case properties_i.shapeIndex:
                    ret = default_shapeIndex;
                    break;
                case properties_i.shape0Tip:
                    ret = default_subShapeTipLocIndex;
                    break;
                case properties_i.shape1Tip:
                    ret = default_subShape2TipLocIndex;
                    break;
                case properties_i.shape2Tip:
                    ret = default_subShape3TipLocIndex;
                    break;
                case properties_i.subShapeIndex:
                    ret = default_subShapeRefIndex;
                    break;
                case properties_i.posIndex:
                    ret = default_posInSubShapeIndex;
                    break;
                case properties_i.proxRays:
                    ret = default_proximitySideRays;
                    break;
                case properties_i.edgeSlide:
                    ret = default_edgeSlide;
                    break;
                case properties_i.lwrType:
                case properties_i.lwr2Type:
                    ret = default_LWRNoiseType;
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
                case properties_i.alignX:
                case properties_i.alignY:
                    ret = default_alignGeom;
                    break;
                case properties_i.flipH:
                    ret = default_flipH;
                    break;
                case properties_i.flipV:
                    ret = default_flipV;
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
                case properties_i.proxSideRaysFallOff:
                    ret = default_proximitySideRaysFallOff;
                    break;
            }

            return ret;
        }

        string comment;
        string layerName;
        string horTipBiasPVar_RNGMapping;
        string horTipBiasNVar_RNGMapping;
        string verTipBiasPVar_RNGMapping;
        string verTipBiasNVar_RNGMapping;
        string innerCV_RNGMapping;
        string outerCV_RNGMapping;
        string LWR_RNGMapping;
        string LWR2_RNGMapping;
        string sideCDU_RNGMapping;
        string tipsCDU_RNGMapping;
        string horOverlay_RNGMapping;
        string verOverlay_RNGMapping;
        string wobble_RNGMapping;
        string fileToLoad; // file value to load.
        string structureNameFromFile;
        string ldNameFromFile;

        public enum properties_s
        {
            comment, name, hTipPVar_RNG, hTipNVar_RNG, vTipPVar_RNG, vTipNVar_RNG, iCV_RNG, oCV_RNG, lwr_RNG, lwr2_RNG, sCDU_RNG, tCDU_RNG,
            xOL_RNG, yOL_RNG, wobble_RNG,
            file, structure, lD
        }

        public string getString(properties_s p)
        {
            return pGetString(p);
        }

        string pGetString(properties_s p)
        {
            string ret = "";

            switch (p)
            {
                case properties_s.comment:
                    ret = comment;
                    break;
                case properties_s.name:
                    ret = layerName;
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

        void pSetString(properties_s p, string val)
        {
            switch (p)
            {
                case properties_s.comment:
                    comment = val;
                    break;
                case properties_s.name:
                    layerName = val;
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

        void pDefaultString(properties_s p)
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
                    layerName = default_layerName;
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

        static string pGetDefaultString(properties_s p)
        {
            string ret = "";

            switch (p)
            {
                case properties_s.comment:
                    ret = default_comment;
                    break;
                case properties_s.name:
                    ret = default_layerName;
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

        decimal subShapeHorLength;
        decimal subShapeHorOffset;
        decimal subShapeVerLength;
        decimal subShapeVerOffset;
        decimal subShape2HorLength;
        decimal subShape2HorOffset;
        decimal subShape2VerLength;
        decimal subShape2VerOffset;
        decimal subShape3HorLength;
        decimal subShape3HorOffset;
        decimal subShape3VerLength;
        decimal subShape3VerOffset;
        decimal globalHorOffset;
        decimal globalVerOffset;
        decimal rotation;
        decimal wobble;
        decimal sideBias;
        decimal horTipBias;
        decimal horTipBiasPVar;
        decimal horTipBiasNVar;
        decimal verTipBias;
        decimal verTipBiasPVar;
        decimal verTipBiasNVar;
        decimal proximityBias;
        decimal proximityIsoDistance;
        decimal lensDistortionCoeff1;
        decimal lensDistortionCoeff2;
        decimal innerCRR;
        decimal outerCRR;
        decimal innerCV;
        decimal outerCV;
        decimal LWR;
        decimal LWR2;
        decimal edgeSlideTension;
        decimal LWRNoiseFreq;
        decimal LWR2NoiseFreq;
        decimal sideCDU;
        decimal tipsCDU;
        decimal horOverlay;
        decimal verOverlay;
        decimal proxSideRaysMultiplier;

        public enum properties_decimal
        {
            s0HorLength, s0VerLength, s0HorOffset, s0VerOffset,
            s1HorLength, s1VerLength, s1HorOffset, s1VerOffset,
            s2HorLength, s2VerLength, s2HorOffset, s2VerOffset,
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
            proxSideRaysMultiplier
        }

        public decimal getDecimal(properties_decimal p)
        {
            return pGetDecimal(p);
        }

        decimal pGetDecimal(properties_decimal p)
        {
            decimal ret = 0m;
            switch (p)
            {
                case properties_decimal.s0HorLength:
                    ret = subShapeHorLength;
                    break;
                case properties_decimal.s0VerLength:
                    ret = subShapeVerLength;
                    break;
                case properties_decimal.s0HorOffset:
                    ret = subShapeHorOffset;
                    break;
                case properties_decimal.s0VerOffset:
                    ret = subShapeVerOffset;
                    break;
                case properties_decimal.s1HorLength:
                    ret = subShape2HorLength;
                    break;
                case properties_decimal.s1VerLength:
                    ret = subShape2VerLength;
                    break;
                case properties_decimal.s1HorOffset:
                    ret = subShape2HorOffset;
                    break;
                case properties_decimal.s1VerOffset:
                    ret = subShape2VerOffset;
                    break;
                case properties_decimal.s2HorLength:
                    ret = subShape3HorLength;
                    break;
                case properties_decimal.s2VerLength:
                    ret = subShape3VerLength;
                    break;
                case properties_decimal.s2HorOffset:
                    ret = subShape3HorOffset;
                    break;
                case properties_decimal.s2VerOffset:
                    ret = subShape3VerOffset;
                    break;
                case properties_decimal.gHorOffset:
                    ret = globalHorOffset;
                    break;
                case properties_decimal.gVerOffset:
                    ret = globalVerOffset;
                    break;
                case properties_decimal.rot:
                    ret = rotation;
                    break;
                case properties_decimal.wobble:
                    ret = wobble;
                    break;
                case properties_decimal.sBias:
                    ret = sideBias;
                    break;
                case properties_decimal.hTBias:
                    ret = horTipBias;
                    break;
                case properties_decimal.hTNVar:
                    ret = horTipBiasNVar;
                    break;
                case properties_decimal.hTPVar:
                    ret = horTipBiasPVar;
                    break;
                case properties_decimal.vTBias:
                    ret = verTipBias;
                    break;
                case properties_decimal.vTNVar:
                    ret = verTipBiasNVar;
                    break;
                case properties_decimal.vTPVar:
                    ret = verTipBiasPVar;
                    break;
                case properties_decimal.pBias:
                    ret = proximityBias;
                    break;
                case properties_decimal.pBiasDist:
                    ret = proximityIsoDistance;
                    break;
                case properties_decimal.lDC1:
                    ret = lensDistortionCoeff1;
                    break;
                case properties_decimal.lDC2:
                    ret = lensDistortionCoeff2;
                    break;
                case properties_decimal.iCR:
                    ret = innerCRR;
                    break;
                case properties_decimal.iCV:
                    ret = innerCV;
                    break;
                case properties_decimal.oCR:
                    ret = outerCRR;
                    break;
                case properties_decimal.oCV:
                    ret = outerCV;
                    break;
                case properties_decimal.lwr:
                    ret = LWR;
                    break;
                case properties_decimal.lwrFreq:
                    ret = LWRNoiseFreq;
                    break;
                case properties_decimal.lwr2:
                    ret = LWR2;
                    break;
                case properties_decimal.lwr2Freq:
                    ret = LWR2NoiseFreq;
                    break;
                case properties_decimal.eTension:
                    ret = edgeSlideTension;
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
                case properties_decimal.proxSideRaysMultiplier:
                    ret = proxSideRaysMultiplier;
                    break;
            }

            return ret;
        }

        public static decimal getDefaultDecimal(properties_decimal p)
        {
            return pGetDefaultDecimal(p);
        }

        static decimal pGetDefaultDecimal(properties_decimal p)
        {
            decimal ret = 0m;
            switch (p)
            {
                case properties_decimal.s0HorLength:
                    ret = default_subShapeHorLength;
                    break;
                case properties_decimal.s0VerLength:
                    ret = default_subShapeVerLength;
                    break;
                case properties_decimal.s0HorOffset:
                    ret = default_subShapeHorOffset;
                    break;
                case properties_decimal.s0VerOffset:
                    ret = default_subShapeVerOffset;
                    break;
                case properties_decimal.s1HorLength:
                    ret = default_subShape2HorLength;
                    break;
                case properties_decimal.s1VerLength:
                    ret = default_subShape2VerLength;
                    break;
                case properties_decimal.s1HorOffset:
                    ret = default_subShape2HorOffset;
                    break;
                case properties_decimal.s1VerOffset:
                    ret = default_subShape2VerOffset;
                    break;
                case properties_decimal.s2HorLength:
                    ret = default_subShape3HorLength;
                    break;
                case properties_decimal.s2VerLength:
                    ret = default_subShape3VerLength;
                    break;
                case properties_decimal.s2HorOffset:
                    ret = default_subShape3HorOffset;
                    break;
                case properties_decimal.s2VerOffset:
                    ret = default_subShape3VerOffset;
                    break;
                case properties_decimal.gHorOffset:
                    ret = default_globalHorOffset;
                    break;
                case properties_decimal.gVerOffset:
                    ret = default_globalVerOffset;
                    break;
                case properties_decimal.rot:
                    ret = default_rotation;
                    break;
                case properties_decimal.wobble:
                    ret = default_wobble;
                    break;
                case properties_decimal.sBias:
                    ret = default_sideBias;
                    break;
                case properties_decimal.hTBias:
                    ret = default_horTipBias;
                    break;
                case properties_decimal.hTNVar:
                    ret = default_horTipBiasNVar;
                    break;
                case properties_decimal.hTPVar:
                    ret = default_horTipBiasPVar;
                    break;
                case properties_decimal.vTBias:
                    ret = default_verTipBias;
                    break;
                case properties_decimal.vTNVar:
                    ret = default_verTipBiasNVar;
                    break;
                case properties_decimal.vTPVar:
                    ret = default_verTipBiasPVar;
                    break;
                case properties_decimal.pBias:
                    ret = default_proximityBias;
                    break;
                case properties_decimal.pBiasDist:
                    ret = default_proximityIsoDistance;
                    break;
                case properties_decimal.lDC1:
                    ret = default_lensDistortionCoeff1;
                    break;
                case properties_decimal.lDC2:
                    ret = default_lensDistortionCoeff2;
                    break;
                case properties_decimal.iCR:
                    ret = default_innerCRR;
                    break;
                case properties_decimal.iCV:
                    ret = default_innerCV;
                    break;
                case properties_decimal.oCR:
                    ret = default_outerCRR;
                    break;
                case properties_decimal.oCV:
                    ret = default_outerCV;
                    break;
                case properties_decimal.lwr:
                case properties_decimal.lwr2:
                    ret = default_LWR;
                    break;
                case properties_decimal.lwrFreq:
                case properties_decimal.lwr2Freq:
                    ret = default_LWRNoiseFreq;
                    break;
                case properties_decimal.eTension:
                    ret = default_edgeSlideTension;
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
                case properties_decimal.proxSideRaysMultiplier:
                    ret = default_proximitySideRaysFallOffMultiplier;
                    break;
            }

            return ret;
        }

        public void setDecimal(properties_decimal p, decimal val)
        {
            pSetDecimal(p, val);
        }

        void pSetDecimal(properties_decimal p, decimal val)
        {
            switch (p)
            {
                case properties_decimal.s0HorLength:
                    subShapeHorLength = val;
                    break;
                case properties_decimal.s0VerLength:
                    subShapeVerLength = val;
                    break;
                case properties_decimal.s0HorOffset:
                    subShapeHorOffset = val;
                    break;
                case properties_decimal.s0VerOffset:
                    subShapeVerOffset = val;
                    break;
                case properties_decimal.s1HorLength:
                    subShape2HorLength = val;
                    break;
                case properties_decimal.s1VerLength:
                    subShape2VerLength = val;
                    break;
                case properties_decimal.s1HorOffset:
                    subShape2HorOffset = val;
                    break;
                case properties_decimal.s1VerOffset:
                    subShape2VerOffset = val;
                    break;
                case properties_decimal.s2HorLength:
                    subShape3HorLength = val;
                    break;
                case properties_decimal.s2VerLength:
                    subShape3VerLength = val;
                    break;
                case properties_decimal.s2HorOffset:
                    subShape3HorOffset = val;
                    break;
                case properties_decimal.s2VerOffset:
                    subShape3VerOffset = val;
                    break;
                case properties_decimal.gHorOffset:
                    globalHorOffset = val;
                    break;
                case properties_decimal.gVerOffset:
                    globalVerOffset = val;
                    break;
                case properties_decimal.rot:
                    rotation = val;
                    break;
                case properties_decimal.wobble:
                    wobble = val;
                    break;
                case properties_decimal.sBias:
                    sideBias = val;
                    break;
                case properties_decimal.hTBias:
                    horTipBias = val;
                    break;
                case properties_decimal.hTNVar:
                    horTipBiasNVar = val;
                    break;
                case properties_decimal.hTPVar:
                    horTipBiasPVar = val;
                    break;
                case properties_decimal.vTBias:
                    verTipBias = val;
                    break;
                case properties_decimal.vTNVar:
                    verTipBiasNVar = val;
                    break;
                case properties_decimal.vTPVar:
                    verTipBiasPVar = val;
                    break;
                case properties_decimal.pBias:
                    proximityBias = val;
                    break;
                case properties_decimal.pBiasDist:
                    proximityIsoDistance = val;
                    break;
                case properties_decimal.lDC1:
                    lensDistortionCoeff1 = val;
                    break;
                case properties_decimal.lDC2:
                    lensDistortionCoeff2 = val;
                    break;
                case properties_decimal.iCR:
                    innerCRR = val;
                    break;
                case properties_decimal.iCV:
                    innerCV = val;
                    break;
                case properties_decimal.oCR:
                    outerCRR = val;
                    break;
                case properties_decimal.oCV:
                    outerCV = val;
                    break;
                case properties_decimal.lwr:
                    LWR = val;
                    break;
                case properties_decimal.lwrFreq:
                    LWRNoiseFreq = val;
                    break;
                case properties_decimal.lwr2:
                    LWR2 = val;
                    break;
                case properties_decimal.lwr2Freq:
                    LWR2NoiseFreq = val;
                    break;
                case properties_decimal.eTension:
                    edgeSlideTension = val;
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
                case properties_decimal.proxSideRaysMultiplier:
                    proxSideRaysMultiplier = val;
                    break;
            }
        }

        public void defaultDecimal(properties_decimal p)
        {
            pDefaultDecimal(p);
        }

        void pDefaultDecimal(properties_decimal p)
        {
            switch (p)
            {
                case properties_decimal.eTension:
                    edgeSlideTension = default_edgeSlideTension;
                    break;
                case properties_decimal.gHorOffset:
                    globalHorOffset = default_globalHorOffset;
                    break;
                case properties_decimal.gVerOffset:
                    globalVerOffset = default_globalVerOffset;
                    break;
                case properties_decimal.hTBias:
                    horTipBias = default_horTipBias;
                    break;
                case properties_decimal.hTNVar:
                    horTipBiasNVar = default_horTipBiasNVar;
                    break;
                case properties_decimal.hTPVar:
                    horTipBiasPVar = default_horTipBiasPVar;
                    break;
                case properties_decimal.iCR:
                    innerCRR = default_innerCRR;
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
                case properties_decimal.lwr:
                    LWR = default_LWR;
                    break;
                case properties_decimal.lwr2:
                    LWR2 = default_LWR;
                    break;
                case properties_decimal.lwrFreq:
                    LWRNoiseFreq = default_LWRNoiseFreq;
                    break;
                case properties_decimal.lwr2Freq:
                    LWR2NoiseFreq = default_LWRNoiseFreq;
                    break;
                case properties_decimal.oCR:
                    outerCRR = default_outerCRR;
                    break;
                case properties_decimal.oCV:
                    outerCV = default_outerCV;
                    break;
                case properties_decimal.pBias:
                    proximityBias = default_proximityBias;
                    break;
                case properties_decimal.pBiasDist:
                    proximityIsoDistance = default_proximityIsoDistance;
                    break;
                case properties_decimal.rot:
                    rotation = default_rotation;
                    break;
                case properties_decimal.s0HorLength:
                    subShapeHorLength = default_subShapeHorLength;
                    break;
                case properties_decimal.s0HorOffset:
                    subShapeHorOffset = default_subShapeHorOffset;
                    break;
                case properties_decimal.s0VerLength:
                    subShapeVerLength = default_subShapeVerLength;
                    break;
                case properties_decimal.s0VerOffset:
                    subShapeVerOffset = default_subShapeVerOffset;
                    break;
                case properties_decimal.s1HorLength:
                    subShape2HorLength = default_subShape2HorLength;
                    break;
                case properties_decimal.s1HorOffset:
                    subShape2HorOffset = default_subShape2HorOffset;
                    break;
                case properties_decimal.s1VerLength:
                    subShape2VerLength = default_subShape2VerLength;
                    break;
                case properties_decimal.s1VerOffset:
                    subShape2VerOffset = default_subShape2VerOffset;
                    break;
                case properties_decimal.s2HorLength:
                    subShape3HorLength = default_subShape3HorLength;
                    break;
                case properties_decimal.s2HorOffset:
                    subShape3HorOffset = default_subShape3HorOffset;
                    break;
                case properties_decimal.s2VerLength:
                    subShape3VerLength = default_subShape3VerLength;
                    break;
                case properties_decimal.s2VerOffset:
                    subShape3VerOffset = default_subShape3VerOffset;
                    break;
                case properties_decimal.sBias:
                    sideBias = default_sideBias;
                    break;
                case properties_decimal.sCDU:
                    sideCDU = default_sideCDU;
                    break;
                case properties_decimal.tCDU:
                    tipsCDU = default_tipsCDU;
                    break;
                case properties_decimal.vTBias:
                    verTipBias = default_verTipBias;
                    break;
                case properties_decimal.vTNVar:
                    verTipBiasNVar = default_verTipBiasNVar;
                    break;
                case properties_decimal.vTPVar:
                    verTipBiasPVar = default_verTipBiasPVar;
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
                case properties_decimal.proxSideRaysMultiplier:
                    proxSideRaysMultiplier = default_proximitySideRaysFallOffMultiplier;
                    break;
            }
        }

        bool reloadedFileData; // if true, we pulled in point data from a reloaded XML file and shouldn't obliterate it.
        public bool isReloaded()
        {
            return pIsReloaded();
        }

        bool pIsReloaded()
        {
            return reloadedFileData;
        }

        public void setReloaded(bool val)
        {
            pSetReloaded(val);
        }

        void pSetReloaded(bool val)
        {
            reloadedFileData = val;
        }

        List<GeoLibPointF[]> fileData; // holds the parsed point data of our level, for file-based content. Allows for multiple polys in layer

        public List<GeoLibPointF[]> getFileData()
        {
            return pGetFileData();
        }

        List<GeoLibPointF[]> pGetFileData()
        {
            return fileData;
        }

        public void setFileData(List<GeoLibPointF[]> newdata)
        {
            pSetFileData(newdata);
        }

        void pSetFileData(List<GeoLibPointF[]> newdata)
        {
            fileData = newdata.ToList();
        }

        public void defaultFileData()
        {
            pDefaultFileData();
        }

        void pDefaultFileData()
        {
            fileData = default_fileData.ToList();
        }

        public EntropyLayerSettings()
        {
            pEntropyLayerSettings();
        }

        void pEntropyLayerSettings()
        {
            comment = default_comment;
            bgLayers = default_bgLayers.ToArray();
            enabled = default_enabled;
            geoCoreShapeEngine = default_geoCoreShapeEngine;
            shapeIndex = default_shapeIndex;
            layerName = default_layerName;

            subShapeHorLength = default_subShapeHorLength;
            subShapeVerLength = default_subShapeVerLength;
            subShapeHorOffset = default_subShapeHorOffset;
            subShapeVerOffset = default_subShapeVerOffset;
            subShapeTipLocIndex = default_subShapeTipLocIndex;
            subShape2HorLength = default_subShape2HorLength;
            subShape2VerLength = default_subShape2VerLength;
            subShape2HorOffset = default_subShape2HorOffset;
            subShape2VerOffset = default_subShape2VerOffset;
            subShape2TipLocIndex = default_subShape2TipLocIndex;
            subShape3HorLength = default_subShape3HorLength;
            subShape3VerLength = default_subShape3VerLength;
            subShape3HorOffset = default_subShape3HorOffset;
            subShape3VerOffset = default_subShape3VerOffset;
            subShape3TipLocIndex = default_subShape3TipLocIndex;
            subShapeRefIndex = default_subShapeRefIndex;
            posInSubShapeIndex = default_posInSubShapeIndex;

            rotation = default_rotation;
            wobble = default_wobble;
            wobble_RNGMapping = default_rngmapping;
            sideBias = default_sideBias;
            horTipBias = default_horTipBias;
            horTipBiasNVar = default_horTipBiasNVar;
            horTipBiasNVar_RNGMapping = default_rngmapping;
            horTipBiasPVar = default_horTipBiasPVar;
            horTipBiasPVar_RNGMapping = default_rngmapping;
            verTipBias = default_verTipBias;
            verTipBiasNVar = default_verTipBiasNVar;
            verTipBiasNVar_RNGMapping = default_rngmapping;
            verTipBiasPVar = default_verTipBiasPVar;
            verTipBiasPVar_RNGMapping = default_rngmapping;
            proximityBias = default_proximityBias;
            proximityIsoDistance = default_proximityIsoDistance;
            proximitySideRays = default_proximitySideRays;

            innerCRR = default_innerCRR;
            innerCV = default_innerCV;
            innerCV_RNGMapping = default_rngmapping;
            outerCRR = default_outerCRR;
            outerCV = default_outerCV;
            outerCV_RNGMapping = default_rngmapping;
            edgeSlide = default_edgeSlide;
            edgeSlideTension = default_edgeSlideTension;

            LWR = default_LWR;
            LWR_RNGMapping = default_rngmapping;
            LWRNoiseType = default_LWRNoiseType;
            LWRNoisePreview = default_LWRNoisePreview;
            LWRNoiseFreq = default_LWRNoiseFreq;
            LWR2 = default_LWR;
            LWR2_RNGMapping = default_rngmapping;
            LWR2NoiseType = default_LWRNoiseType;
            LWR2NoiseFreq = default_LWRNoiseFreq;

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
            flipH = default_flipH;
            flipV = default_flipV;
            alignGeomX = default_alignGeom;
            alignGeomY = default_alignGeom;
            showDrawn = default_showDrawn;

            lensDistortionCoeff1 = default_lensDistortionCoeff1;
            lensDistortionCoeff2 = default_lensDistortionCoeff2;

            booleanLayerA = default_booleanLayerA;
            booleanLayerB = default_booleanLayerB;
            booleanLayerOpA = default_booleanLayerOpA;
            booleanLayerOpB = default_booleanLayerOpB;
            booleanLayerOpAB = default_booleanLayerOpAB;

            omitFromSim = default_omitLayer;

            proxSideRaysFallOff = default_proximitySideRaysFallOff;
            proxSideRaysMultiplier = default_proximitySideRaysFallOffMultiplier;
        }

        public void adjustSettings(EntropyLayerSettings source, bool gdsOnly)
        {
            pAdjustSettings(source, gdsOnly);
        }

        void pAdjustSettings(EntropyLayerSettings source, bool gdsOnly)
        {
            comment = source.comment;
            bgLayers = source.bgLayers.ToArray();
            enabled = source.enabled;
            geoCoreShapeEngine = source.geoCoreShapeEngine;
            showDrawn = source.showDrawn;
            layerName = source.layerName;
            shapeIndex = source.shapeIndex;

            if (!gdsOnly)
            {
                subShapeHorLength = source.subShapeHorLength;
                subShapeHorOffset = source.subShapeHorOffset;
                subShapeVerLength = source.subShapeVerLength;
                subShapeVerOffset = source.subShapeVerOffset;
                subShapeTipLocIndex = source.subShapeTipLocIndex;

                subShape2HorLength = source.subShape2HorLength;
                subShape2HorOffset = source.subShape2HorOffset;
                subShape2VerLength = source.subShape2VerLength;
                subShape2VerOffset = source.subShape2VerOffset;
                subShape2TipLocIndex = source.subShape2TipLocIndex;

                subShape3HorLength = source.subShape3HorLength;
                subShape3HorOffset = source.subShape3HorOffset;
                subShape3VerLength = source.subShape3VerLength;
                subShape3VerOffset = source.subShape3VerOffset;
                subShape3TipLocIndex = source.subShape3TipLocIndex;

                subShapeRefIndex = source.subShapeRefIndex;
                posInSubShapeIndex = source.posInSubShapeIndex;

                globalHorOffset = source.globalHorOffset;
                globalVerOffset = source.globalVerOffset;

                rotation = source.rotation;
                wobble = source.wobble;
                wobble_RNGMapping = source.wobble_RNGMapping;
                sideBias = source.sideBias;

                horTipBias = source.horTipBias;
                horTipBiasNVar = source.horTipBiasNVar;
                horTipBiasNVar_RNGMapping = source.horTipBiasNVar_RNGMapping;
                horTipBiasPVar = source.horTipBiasPVar;
                horTipBiasPVar_RNGMapping = source.horTipBiasPVar_RNGMapping;
                verTipBias = source.verTipBias;
                verTipBiasNVar = source.verTipBiasNVar;
                verTipBiasNVar_RNGMapping = source.verTipBiasNVar_RNGMapping;
                verTipBiasPVar = source.verTipBiasPVar;
                verTipBiasPVar_RNGMapping = source.verTipBiasPVar_RNGMapping;
                proximityBias = source.proximityBias;
                proximityIsoDistance = source.proximityIsoDistance;
                proximitySideRays = source.proximitySideRays;

                innerCRR = source.innerCRR;
                innerCV = source.innerCV;
                innerCV_RNGMapping = source.innerCV_RNGMapping;
                outerCRR = source.outerCRR;
                outerCV = source.outerCV;
                outerCV_RNGMapping = source.outerCV_RNGMapping;
                edgeSlide = source.edgeSlide;
                edgeSlideTension = source.edgeSlideTension;

                LWR = source.LWR;
                LWRNoiseType = source.LWRNoiseType;
                LWRNoisePreview = source.LWRNoisePreview;
                LWRNoiseFreq = source.LWRNoiseFreq;
                LWR2 = source.LWR2;
                LWR2NoiseType = source.LWR2NoiseType;
                LWR2NoiseFreq = source.LWR2NoiseFreq;

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

                flipH = source.flipH;
                flipV = source.flipV;
                alignGeomX = source.alignGeomX;
                alignGeomY = source.alignGeomY;

                lensDistortionCoeff1 = source.lensDistortionCoeff1;
                lensDistortionCoeff2 = source.lensDistortionCoeff2;

                booleanLayerA = source.booleanLayerA;
                booleanLayerB = source.booleanLayerB;
                booleanLayerOpA = source.booleanLayerOpA;
                booleanLayerOpB = source.booleanLayerOpB;
                booleanLayerOpAB = source.booleanLayerOpAB;

                omitFromSim = source.omitFromSim;

                proxSideRaysFallOff = source.proxSideRaysFallOff;
                proxSideRaysMultiplier = source.proxSideRaysMultiplier;
            }

            // layout stuff

            fileToLoad = source.fileToLoad;
            structureFromFile = source.structureFromFile;
            structureNameFromFile = source.structureNameFromFile;
            ldFromFile = source.ldFromFile;
            ldNameFromFile = source.ldNameFromFile;
            polyFill = source.polyFill;
            fileData = source.fileData.ToList();
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

        bool pNonGaussianValues()
        {
            bool gaussianvalues = horOverlay_RNGMapping == CommonVars.boxMuller;
            gaussianvalues = gaussianvalues && (verOverlay_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (innerCV_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (outerCV_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (sideCDU_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (tipsCDU_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (horTipBiasNVar_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (horTipBiasPVar_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (verTipBiasNVar_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (verTipBiasPVar_RNGMapping == CommonVars.boxMuller);
            gaussianvalues = gaussianvalues && (wobble_RNGMapping == CommonVars.boxMuller);

            return !gaussianvalues;
        }
    }
}
