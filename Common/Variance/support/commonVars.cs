using color;
using entropyRNG;
using Error;
using geoCoreLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using utility;

namespace Variance
{
    public class CommonVars
    {
        bool changed;

        public void setChanged(bool val)
        {
            pSetChanged(val);
        }

        void pSetChanged(bool val)
        {
            changed = val;
        }

        public bool isChanged()
        {
            return pIsChanged();
        }

        bool pIsChanged()
        {
            return changed;
        }

        public enum hashes { gc, settings, entropy, implant, entropyImplant }

        string geoCoreHash;

        string listOfSettingsHash;

        string entropyGeoHash;

        string implantHash;
        string entropyImplantHash;

        public string getHash(hashes p)
        {
            return pGetHash(p);
        }

        string pGetHash(hashes p)
        {
            string ret = "";
            switch (p)
            {
                case hashes.gc:
                    ret = geoCoreHash;
                    break;
                case hashes.settings:
                    ret = listOfSettingsHash;
                    break;
                case hashes.entropy:
                    ret = entropyGeoHash;
                    break;
                case hashes.implant:
                    ret = implantHash;
                    break;
                case hashes.entropyImplant:
                    ret = entropyImplantHash;
                    break;
            }

            return ret;
        }
        
        public Thread mainThreadIndex;
        Colors colors;
        public Colors getColors()
        {
            return pGetColors();
        }

        Colors pGetColors()
        {
            return colors;
        }

        public void setColors(Colors source)
        {
            pSetColors(source);
        }

        void pSetColors(Colors source)
        {
            colors = source;
        }

        public System.Timers.Timer m_timer { get; set; }

        public bool userCancelQuery { get; set; } // used to track whether user is being queried.
        public bool cancelling { get; set; } // used to track whether we are currently in the abort check method, to avoid duplicate calls.
        public bool runAbort { get; set; } // whether to abort or not, based on the user response to the dialog tracked by userCancelQuery.
        public bool loadAbort { get; set; } // whether to abort or not, used by GDS loading system.

        public enum properties_gl { aa, fill, points }

        bool AA;
        bool filledPolygons;
        bool drawPoints;

        bool friendlyNumber;

        public void setFriendly(bool val)
        {
            pSetFriendly(val);
        }

        void pSetFriendly(bool val)
        {
            friendlyNumber = val;
        }

        public bool getFriendly()
        {
            return pGetFriendly();
        }

        bool pGetFriendly()
        {
            return friendlyNumber;
        }

        public void setOpenGLProp(properties_gl p, bool val)
        {
            pSetOpenGLProp(p, val);
        }

        void pSetOpenGLProp(properties_gl p, bool val)
        {
            switch (p)
            {
                case properties_gl.aa:
                    AA = val;
                    break;
                case properties_gl.fill:
                    filledPolygons = val;
                    break;
                case properties_gl.points:
                    drawPoints = val;
                    break;
            }
        }

        public bool getOpenGLProp(properties_gl p)
        {
            return pGetOpenGLProp(p);
        }

        bool pGetOpenGLProp(properties_gl p)
        {
            bool ret = false;
            switch (p)
            {
                case properties_gl.aa:
                    ret = AA;
                    break;
                case properties_gl.fill:
                    ret = filledPolygons;
                    break;
                case properties_gl.points:
                    ret = drawPoints;
                    break;
            }

            return ret;
        }

        bool layerPreviewDOETile;

        public bool getLayerPreviewDOETile()
        {
            return pGetLayerPreviewDOETile();
        }

        bool pGetLayerPreviewDOETile()
        {
            return layerPreviewDOETile;
        }

        public void setLayerPreviewDOETile(bool val)
        {
            pSetLayerPreviewDOETile(val);
        }

        void pSetLayerPreviewDOETile(bool val)
        {
            layerPreviewDOETile = val;
        }

        public enum gl_i { zoom }

        Int32 openGLZoomFactor;

        public Int32 getGLInt(gl_i i)
        {
            return pGetGLInt(i);
        }

        Int32 pGetGLInt(gl_i i)
        {
            int ret = 0;
            switch (i)
            {
                case gl_i.zoom:
                    ret = openGLZoomFactor;
                    break;
            }
            return ret;
        }

        public void setGLInt(gl_i i, Int32 val)
        {
            pSetGLInt(i, val);
        }

        void pSetGLInt(gl_i i, Int32 val)
        {
            switch (i)
            {
                case gl_i.zoom:
                    openGLZoomFactor = val;
                    break;
            }
        }

        public enum opacity_gl { fg, bg }
        double openGLFGOpacity;
        double openGLBGOpacity;

        public double getOpacity(opacity_gl o)
        {
            return pGetOpacity(o);
        }

        double pGetOpacity(opacity_gl o)
        {
            double ret = 0;
            switch (o)
            {
                case opacity_gl.fg:
                    ret = openGLFGOpacity;
                    break;
                case opacity_gl.bg:
                    ret = openGLBGOpacity;
                    break;
            }

            return ret;
        }

        public void setOpacity(opacity_gl o, double val)
        {
            pSetOpacity(o, val);
        }

        void pSetOpacity(opacity_gl o, double val)
        {
            switch (o)
            {
                case opacity_gl.fg:
                    openGLFGOpacity = val;
                    break;
                case opacity_gl.bg:
                    openGLBGOpacity = val;
                    break;
            }
        }

        public object drawingLock { get; set; }
        public object implantDrawingLock { get; set; }

        bool geoCoreCDVariation;

        public bool getGCCDV()
        {
            return pGetGCCDV();
        }

        bool pGetGCCDV()
        {
            return geoCoreCDVariation;
        }

        public void setGCCDV(bool val)
        {
            pSetGCCDV(val);
        }

        void pSetGCCDV(bool val)
        {
            geoCoreCDVariation = val;
        }

        int replayMode;

        public int getReplayMode()
        {
            return pGetReplayMode();
        }

        int pGetReplayMode()
        {
            return replayMode;
        }

        public void setReplayMode(int val)
        {
            pSetReplayMode(val);
        }

        void pSetReplayMode(int val)
        {
            replayMode = val;
        }

        public Int32 HTCount { get; set; }

        public Storage storage { get; set; }

        // UI list elements that get referenced in DataContext.
        public ObservableCollection<string>[] subshapes { get; set; }
        public ObservableCollection<string> subShapesList_exp { get; set; }
        public ObservableCollection<string> layerNames { get; set; }

        // Collection that we use to populate the custom RNG mapping menus on variations.
        public static string boxMuller = "Box-Muller";
        public ObservableCollection<string> rngCustomMapping { get; set; }

        bool simulationRunning;
        public bool isSimRunning()
        {
            return pIsSimRunning();
        }

        bool pIsSimRunning()
        {
            return simulationRunning;
        }

        public void setSimRunning(bool val)
        {
            pSetSimRunning(val);
        }

        void pSetSimRunning(bool val)
        {
            simulationRunning = val;
        }

        public string version { get; set; }
        public string author { get; set; }
        public string titleText { get; set; }

        public string projectFileName = "";
        bool warningShown, geoCoreCDUWarningShown;

        public bool wasWarningShown()
        {
            return pWarningShown();
        }

        bool pWarningShown()
        {
            return warningShown;
        }

        public void setWarningShown(bool val)
        {
            pSetWarningShown(val);
        }

        void pSetWarningShown(bool val)
        {
            warningShown = val;
        }

        string[] calcMode = new string[] { "AREA", "SPACING_ENCLOSURE", "CHORD", "ANGLE" };
        public enum calcModes { area, enclosure_spacing_overlap, chord, angle };
        string[] spacEncMode = new string[] { "SPACING", "ENCLOSURE", "SPACINGOLD", "ENCLOSUREOLD" };
        public enum areaCalcModes { all, perpoly };
        public enum spacingCalcModes { spacing, enclosure, spacingOld, enclosureOld }; // exp triggers projection from shortest edge for overlap evaluation.
        public enum chordCalcElements { none, a, b };
        public enum upperTabNames { twoD, Implant, oneD, Utilities };
        public enum twoDTabNames { layer, settings, DOE, paSearch };

        public static string[] csvHeader = new string[] { "CDUSVar", "CDUTVar", "LWRVar", "LWRSeed", "LWR2Var", "LWR2Seed", "horTipBiasVar", "verTipBiasVar", "iCVar", "oCVar", "overlayX", "overlayY", "wobbleVar" };

        public enum external_Type { svg, gds, oas }
        List<string> externalTypes = new List<string>() { "SVG", "GDS", "Oasis" };
        public enum external_Filter { none, lte, gte }
        List<string> externalFilterList = new List<string>() { "", "<=", ">=" };

        public List<string> getExternalTypes()
        {
            return externalTypes;
        }

        public List<string> getExternalFilterList()
        {
            return externalFilterList;
        }

        NonSimulationSettings nonSimulationSettings;

        public NonSimulationSettings getNonSimulationSettings()
        {
            return pGetNonSimulationSettings();
        }

        NonSimulationSettings pGetNonSimulationSettings()
        {
            return nonSimulationSettings;
        }

        EntropyImplantSettings implantSettings;
        public EntropyImplantSettings getImplantSettings()
        {
            return pGetImplantSettings();
        }

        EntropyImplantSettings pGetImplantSettings()
        {
            return implantSettings;
        }

        EntropySettings implantSimulationSettings;
        public EntropySettings getImplantSimulationSettings()
        {
            return pGetImplantSimulationSettings();
        }

        EntropySettings pGetImplantSimulationSettings()
        {
            return implantSimulationSettings;
        }

        EntropySettings_nonSim implantSettings_nonSim;
        public EntropySettings_nonSim getImplantSettings_nonSim()
        {
            return pGetImplantSettings_nonSim();
        }

        EntropySettings_nonSim pGetImplantSettings_nonSim()
        {
            return implantSettings_nonSim;
        }

        bool copyPrepped;
        public void setCopy(int index)
        {
            pSetCopy(index);
        }

        void pSetCopy(int index)
        {
            copyPrepped = true;
            pSetCopyLayerSettings(index);
            pSetCopyLayerGHSettings(index);
            pSetCopyDOEUse(index);
        }

        public bool isCopyPrepped()
        {
            return pIsCopyPrepped();
        }

        bool pIsCopyPrepped()
        {
            return copyPrepped;
        }

        public void clearCopy()
        {
            pClearCopy();
        }

        void pClearCopy()
        {
            copyPrepped = false;
        }

        public void paste(int index, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false)
        {
            pPaste(index, gdsOnly, updateGeoCoreGeometryFromFile);
        }

        void pPaste(int index, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false)
        {
            pSetLayerSettings(copyLayerSettings, index, gdsOnly, updateGeoCoreGeometryFromFile);
        }

        public void setLayerSettings(EntropyLayerSettings entropyLayerSettings, int index, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false)
        {
            pSetLayerSettings(entropyLayerSettings, index, gdsOnly, updateGeoCoreGeometryFromFile);
        }

        void pSetLayerSettings(EntropyLayerSettings entropyLayerSettings, int index, bool gdsOnly, bool updateGeoCoreGeometryFromFile = false)
        {
            // This is used by the pasteHandler and clearHandler to set user interface values to align with the settings.
            // It is also used by the load from disk file system, using a temporary MCSettings instance as the source
            // In the case of the clearHandler, we get sent a new MCLayerSettings instance, so we have to handle that.
            // Check our copyFrom reference. We need to do this early before anything could change.

            if (!gdsOnly)
            {
                if (isCopyPrepped())
                {
                    if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr) == 1) || (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr) == 1) || (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr) == 1))
                    {
                        // User pasting into the correlation layer. Disable correlation settings accordingly.
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == index)
                        {
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.xOL_corr, 0);
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.xOL_corr_ref, -1);
                        }
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == index)
                        {
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.yOL_corr, 0);
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, -1);
                        }
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.xOL_ref) == index)
                        {
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.xOL_ref, -1);
                        }
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.yOL_ref) == index)
                        {
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.yOL_ref, -1);
                        }
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == index)
                        {
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.CDU_corr, 0);
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.CDU_corr_ref, -1);
                        }
                        if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == index)
                        {
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.tCDU_corr, 0);
                            entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, -1);
                        }
                    }

                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerA) == index)
                    {
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.bLayerA, -1);
                    }

                    if (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.bLayerB) == index)
                    {
                        entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.bLayerB, -1);
                    }
                }
            }

            // Remove any average overlay reference to the layer we're in, just for safety.
            entropyLayerSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, index, 0);
            entropyLayerSettings.setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, index, 0);


            try
            {
                if (isCopyPrepped())
                {
                    getSimulationSettings().getDOESettings().setLayerAffected(index, copyDOEUse);
                }
            }
            catch (Exception)
            {
            }

            if ((entropyLayerSettings.getDecimal(EntropyLayerSettings.properties_decimal.sCDU) != 0.0m) && (entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE) &&
                (geoCoreCDVariation == false))
            {
                if (!geoCoreCDUWarningShown)
                {
                    geoCoreCDUWarningShown = true;
                    ErrorReporter.showMessage_OK("Project uses Oasis/GDS CD variation.", "Overriding preference.");
                    geoCoreCDVariation = true;
                }
            }

            if (isCopyPrepped())
            {
                // Align the external data.
                getGeoCoreHandler(index).readValues(copyLayerGHSettings);
            }
            else
            {
                getGeoCoreHandler(index).setValid(false);
                if ((entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.shapeIndex) == (Int32)CommonVars.shapeNames.GEOCORE) && (entropyLayerSettings.isReloaded()))
                {
                    getGeoCoreHandler(index).setFilename(entropyLayerSettings.getString(EntropyLayerSettings.properties_s.file));
                    getGeoCoreHandler(index).setValid(true);
                    bool updateSuccess = false;
                    if (updateGeoCoreGeometryFromFile)
                    {
                        getGeoCoreHandler(index).getGeo().updateCollections();
                        // Get the indices based on the stored structure / layerdatatype
                        int structureFromFile = Array.IndexOf(getGeoCoreHandler(index).getGeo().getStructureList().ToArray(), entropyLayerSettings.getString(EntropyLayerSettings.properties_s.structure));
                        if (structureFromFile != -1)
                        {
                            int ldFromFile = Array.IndexOf(getGeoCoreHandler(index).getGeo().getActiveStructureLDList().ToArray(), entropyLayerSettings.getString(EntropyLayerSettings.properties_s.lD));
                            if (ldFromFile != -1)
                            {
                                entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.structure, structureFromFile);
                                entropyLayerSettings.setInt(EntropyLayerSettings.properties_i.lD, ldFromFile);
                                getGeoCoreHandler(index).getGeo().updateGeometry(entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.structure), entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lD));
                                getGeoCoreHandler(index).setPoints(entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.structure), entropyLayerSettings.getInt(EntropyLayerSettings.properties_i.lD));
                                // Map our points into the layer's file data, if the layer is active.
                                entropyLayerSettings.setFileData(getGeoCoreHandler(index).getGeo().points(flatten: true));
                                updateSuccess = true;
                            }
                        }

                        if (!updateSuccess)
                        {
                            // Settings could not be mapped to external file data.
                            ErrorReporter.showMessage_OK("Could not find named structure or layer/datatype. Using stored geometry.", "Layout reference error");
                        }
                    }

                    if (!updateGeoCoreGeometryFromFile || (updateGeoCoreGeometryFromFile && !updateSuccess))
                    {
                        getGeoCoreHandler(index).getGeo().structureList_.Clear();
                        getGeoCoreHandler(index).getGeo().structureList_.Add(entropyLayerSettings.getString(EntropyLayerSettings.properties_s.structure));
                        getGeoCoreHandler(index).getGeo().activeStructure_LayerDataTypeList_.Clear();
                        getGeoCoreHandler(index).getGeo().activeStructure_LayerDataTypeList_.Add(entropyLayerSettings.getString(EntropyLayerSettings.properties_s.lD));
                    }
                }
            }

            // Commit our settings to the list.
            getLayerSettings(index).adjustSettings(entropyLayerSettings, gdsOnly);
        }

        EntropyLayerSettings copyLayerSettings;
        public void setCopyLayerSettings(int index)
        {
            pSetCopyLayerSettings(index);
        }

        void pSetCopyLayerSettings(int index)
        {
            copyLayerSettings.adjustSettings(getLayerSettings(index), gdsOnly: false);
        }

        GeoCoreHandler copyLayerGHSettings;
        public void setCopyLayerGHSettings(int index)
        {
            pSetCopyLayerGHSettings(index);
        }

        void pSetCopyLayerGHSettings(int index)
        {
            copyLayerGHSettings.readValues(getGeoCoreHandler(index));
        }

        public void pasteGeoCoreHandler(int index)
        {
            pPasteGeoCoreHandler(index);
        }

        void pPasteGeoCoreHandler(int index)
        {
            getGeoCoreHandler(index).getGeo().readValues(copyLayerGHSettings.getGeo());
        }

        int copyDOEUse;

        public int getCopyDOEUse()
        {
            return pGetCopyDOEUse();
        }

        int pGetCopyDOEUse()
        {
            return copyDOEUse;
        }

        void pSetCopyDOEUse(int index)
        {
            copyDOEUse = getSimulationSettings().getDOESettings().getLayerAffected(index);
        }

        EntropySettings simulationSettings;

        public EntropySettings getSimulationSettings()
        {
            return pGetSimulationSettings();
        }

        EntropySettings pGetSimulationSettings()
        {
            return simulationSettings;
        }

        EntropySettings_nonSim simulationSettings_nonSim;

        public EntropySettings_nonSim getSimulationSettings_nonSim()
        {
            return pGetSimulationSettings_nonSim();
        }

        EntropySettings_nonSim pGetSimulationSettings_nonSim()
        {
            return simulationSettings_nonSim;
        }

        PASearch paSearch;
        public PASearch getPASearch()
        {
            return pGetPASearch();
        }

        PASearch pGetPASearch()
        {
            return paSearch;
        }

        List<EntropyLayerSettings> listOfSettings;

        public List<EntropyLayerSettings> getListOfSettings()
        {
            return pGetistOfSettings();
        }

        List<EntropyLayerSettings> pGetistOfSettings()
        {
            return listOfSettings;
        }

        public EntropyLayerSettings getLayerSettings(int i)
        {
            return pGetLayerSettings(i);
        }

        EntropyLayerSettings pGetLayerSettings(int i)
        {
            return listOfSettings[i];
        }

        public bool isLayerActive(int i)
        {
            return pIsLayerActive(i);
        }

        bool pIsLayerActive(int i)
        {
            return getLayerSettings(i).isLayerActive();
        }

        public bool interLayerRelationship_2(int i)
        {
            return pInterLayerRelationship_2(i);
        }

        bool pInterLayerRelationship_2(int i)
        {
            bool status = isLayerActive(i * 2) && isLayerActive((i * 2) + 1);
            return status;
        }

        public bool interLayerRelationship_4(int i)
        {
            return pInterLayerRelationship_4(i);
        }

        bool pInterLayerRelationship_4(int i)
        {
            bool status = isLayerActive(i * 4) || isLayerActive((i * 4) + 1);
            status = status && (isLayerActive((i * 4) + 2) || isLayerActive((i * 4) + 3));

            return status;
        }

        public ObservableCollection<string> calcMode_names { get; set; }

        List<string> availableShapes;
        public List<string> getAvailableShapes()
        {
            return pGetAvailableShapes();
        }

        List<string> pGetAvailableShapes()
        {
            return availableShapes;
        }

        string[] shapesShortNames = new string[] { "NONE", "RECT", "L", "T", "X", "U", "S", "GEOCORE", "BOOLEAN" };
        public enum shapeNames { none, rect, Lshape, Tshape, Xshape, Ushape, Sshape, GEOCORE, BOOLEAN };
        List<string> availableTipsLocations;
        public List<string> getAvailableTipsLocations()
        {
            return pGetAvailableTipsLocations();
        }

        List<string> pGetAvailableTipsLocations()
        {
            return availableTipsLocations;
        }

        public enum tipLocations { none, L, R, LR, T, B, TB, TL, TR, TLR, BL, BR, BLR, TBL, TBR, all };

        List<string> availableSubShapePositions;

        public List<string> getAvailableSubShapePositions()
        {
            return pGetAvailableSubShapePositions();
        }

        List<string> pGetAvailableSubShapePositions()
        {
            return availableSubShapePositions;
        }

        public enum subShapeLocations { TL, TR, BL, BR, TS, RS, BS, LS, C };

        List<string> noiseTypes;
        public List<string> getNoiseTypes()
        {
            return pGetNoiseTypes();
        }

        List<string> pGetNoiseTypes()
        {
            return noiseTypes;
        }

        public enum noiseIndex { perlin, simplex, opensimplex };

        List<string> polyFillTypes;
        public List<string> getPolyFillTypes()
        {
            return pGetPolyFillTypes();
        }

        List<string> pGetPolyFillTypes()
        {
            return polyFillTypes;
        }

        public enum PolyFill { pftEvenOdd, pftNonZero, pftPositive, pftNegative };

        List<string> openGLModeList;

        public List<string> getOpenGLModeList()
        {
            return pGetOpenGLModeList();
        }

        List<string> pGetOpenGLModeList()
        {
            return openGLModeList;
        }

        SimulationPreview simPreview;

        public SimulationPreview getSimPreview()
        {
            return pGetSimPreview();
        }

        SimulationPreview pGetSimPreview()
        {
            return simPreview;
        }

        List<GeoCoreHandler> geoCore_Handlers;
        public List<GeoCoreHandler> getGeoCoreHandlers()
        {
            return pGetGeoCoreHandlers();
        }

        List<GeoCoreHandler> pGetGeoCoreHandlers()
        {
            return geoCore_Handlers;
        }

        public GeoCoreHandler getGeoCoreHandler(int index)
        {
            return pGetGeoCoreHandler(index);
        }

        GeoCoreHandler pGetGeoCoreHandler(int index)
        {
            return geoCore_Handlers[index];
        }

        public ObservableCollection<string>[] structureList { get; set; }
        public ObservableCollection<string>[] activeStructure_LayerDataTypeList { get; set; }

        public ObservableCollection<string> structureList_exp { get; set; }
        public ObservableCollection<string> activeStructure_LayerDataTypeList_exp { get; set; }

        // These are used to manage layer refresh events.
        public enum uiActive { settings, doe, implant }
        bool settingsUIActive;
        bool DOESettingsUIActive;
        bool implantUIActive;

        public bool getActiveUI(uiActive u)
        {
            return pGetActiveUI(u);
        }

        bool pGetActiveUI(uiActive u)
        {
            bool ret = false;
            switch (u)
            {
                case uiActive.doe:
                    ret = DOESettingsUIActive;
                    break;
                case uiActive.settings:
                    ret = settingsUIActive;
                    break;
                case uiActive.implant:
                    ret = implantUIActive;
                    break;
            }
            return ret;
        }

        public void setActiveUI(uiActive u, bool val)
        {
            pSetActiveUI(u, val);
        }

        void pSetActiveUI(uiActive u, bool val)
        {
            switch (u)
            {
                case uiActive.doe:
                    DOESettingsUIActive = val;
                    break;
                case uiActive.implant:
                    implantUIActive = val;
                    break;
                case uiActive.settings:
                    settingsUIActive = val;
                    break;
            }
        }

        public CommonVars(VarianceContext varianceContext)
        {
            pCommonVars(varianceContext);
        }

        public void reset(VarianceContext varianceContext)
        {
            pReset(varianceContext);
        }

        void pReset(VarianceContext varianceContext)
        {
            simulationSettings = new EntropySettings();
            nonSimulationSettings = new NonSimulationSettings(CentralProperties.version);

            storage = new Storage();
            warningShown = false;
            geoCoreCDUWarningShown = false;
            // simPreview set-up
            simPreview = new SimulationPreview(ref varianceContext);
            runAbort = false;
            simulationRunning = false;

            openGLZoomFactor = varianceContext.openGLZoomFactor;
            openGLFGOpacity = varianceContext.FGOpacity;
            openGLBGOpacity = varianceContext.BGOpacity;
            AA = varianceContext.AA;
            filledPolygons = varianceContext.FilledPolygons;
            drawPoints = varianceContext.drawPoints;
            geoCoreCDVariation = varianceContext.geoCoreCDVariation;
            layerPreviewDOETile = varianceContext.layerPreviewDOETile;

            friendlyNumber = varianceContext.friendlyNumber;

            HTCount = varianceContext.HTCount;

            // This is our copy layer reference. false denotes no copy buffer is valid.
            copyPrepped = false;

            simulationSettings = new EntropySettings();
            nonSimulationSettings = new NonSimulationSettings(CentralProperties.version);
            simulationSettings_nonSim = new EntropySettings_nonSim();
            paSearch = new PASearch();
            implantSettings_nonSim = new EntropySettings_nonSim();
            // This will contain our settings for the 2D simulation component
            listOfSettings = new List<EntropyLayerSettings>();
            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                listOfSettings.Add(new EntropyLayerSettings());
                listOfSettings[layer].setString(EntropyLayerSettings.properties_s.name, (layer + 1).ToString());
                layerNames[layer] = (layer + 1).ToString();
                // External file data lists
                geoCore_Handlers[layer].setValid(false);
                geoCore_Handlers[layer].getGeo().reset();
            }

            activeStructure_LayerDataTypeList_exp = activeStructure_LayerDataTypeList[0];
            structureList_exp = structureList[0];

            // Implant
            implantSettings = new EntropyImplantSettings();
            implantSimulationSettings = new EntropySettings();
            implantSimulationSettings.setValue(EntropySettings.properties_i.rngType, (Int32)commonRNG.rngIndex.system_random);
            implantSimulationSettings.setValue(EntropySettings.properties_i.optC, 0); // implant shadowing is defined by corners and optimization greatly impacts result.

            settingsUIActive = true;
            DOESettingsUIActive = true;

            setHashes();
        }

        void pCommonVars(VarianceContext varianceContext)
        {
            mainThreadIndex = System.Threading.Thread.CurrentThread;
            subshapes = new ObservableCollection<string>[CentralProperties.maxLayersForMC];
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                subshapes[i] = new ObservableCollection<string>() { "1" };
            }

            subShapesList_exp = new ObservableCollection<string>();
            subShapesList_exp = subshapes[0];

            rngCustomMapping = new ObservableCollection<string>();
            rngCustomMapping.Add(boxMuller); // default. Not removable.
            for (int i = 0; i < varianceContext.rngMappingEquations.Count; i++)
            {
                if (varianceContext.rngMappingEquations[i] != boxMuller) // skip the default.
                {
                    rngCustomMapping.Add(varianceContext.rngMappingEquations[i]);
                }
            }

            noiseTypes = new List<string>() { "Perlin", "Simplex", "OpenSimplex" };

            availableSubShapePositions = new List<string>() { "Corner: Top Left", "Corner: Top Right", "Corner: Bottom Left", "Corner: Bottom Right",
                                                             "Middle: Top Side", "Middle: Right Side", "Middle: Bottom Side", "Middle: Left Side",
                                                             "Center"};

            availableTipsLocations = new List<string>() { "None", "Left", "Right", "Left & Right",
                                                        "Top", "Bottom", "Top & Bottom",
                                                        "Top & Left", "Top & Right", "Top & Left & Right",
                                                        "Bottom & Left", "Bottom & Right", "Bottom & Left & Right",
                                                        "Top & Bottom & Left", "Top & Bottom & Right",
                                                    "All"};

            availableShapes = new List<string>() { "(None)", "Rectangle/Square", "L-shape", "T-shape", "X-shape", "U-shape", "S-shape", "GDS/Oasis", "Boolean" };
            calcMode_names = new ObservableCollection<string>() { "Compute Area Distribution",
                                                    "Compute Spacing/Overlap Distribution",
                                                    "Compute Chord Distribution",
                                                    "Compute Angle Distribution"
                                                  };
            polyFillTypes = new List<string>() { "Even/Odd", "Non-zero", "Positive", "Negative" };

            openGLModeList = new List<string>() { "VBO", "Immediate" };

            drawingLock = new object();
            implantDrawingLock = new object();
            userCancelQuery = false;
            colors = varianceContext.colors;
            version = CentralProperties.version;
            author = "Phil Stopford (phil.stopford@gmail.com)";
            titleText = CentralProperties.productName + " " + version + " (" + varianceContext.licenceName + ")";

            copyLayerGHSettings = new GeoCoreHandler();
            copyLayerSettings = new EntropyLayerSettings();

            layerNames = new ObservableCollection<string>();

            // External file data lists
            geoCore_Handlers = new List<GeoCoreHandler>();
            structureList = new ObservableCollection<string>[CentralProperties.maxLayersForMC];
            activeStructure_LayerDataTypeList = new ObservableCollection<string>[CentralProperties.maxLayersForMC];
            for (int layer = 0; layer < CentralProperties.maxLayersForMC; layer++)
            {
                geoCore_Handlers.Add(new GeoCoreHandler());
                geoCore_Handlers[layer].getGeo().baseScale = 1000;
                structureList[layer] = geoCore_Handlers[layer].getGeo().structureList_;
                activeStructure_LayerDataTypeList[layer] = geoCore_Handlers[layer].getGeo().activeStructure_LayerDataTypeList_;
                layerNames.Add((layer + 1).ToString());
            }
            structureList_exp = new ObservableCollection<string>();
            activeStructure_LayerDataTypeList_exp = new ObservableCollection<string>();

            pReset(varianceContext);
        }

        public void getBooleanEquation(List<string> linesToWrite)
        {
            pGetBooleanEquation(linesToWrite);
        }

        void pGetBooleanEquation(List<string> linesToWrite)
        {
            // Boolean equation
            string layer12BoolEq = "";
            string layer34BoolEq = "";
            string layer56BoolEq = "";
            string layer78BoolEq = "";
            string layer910BoolEq = "";
            string layer1112BoolEq = "";
            string layer1314BoolEq = "";
            string layer1516BoolEq = "";
            bool layerABoolEq = false;
            bool layer1234BoolEq = false;
            bool layer5678BoolEq = false;
            bool layerBBoolEq = false;
            bool layer9101112BoolEq = false;
            bool layer13141516BoolEq = false;

            string boolString = "";

            if (interLayerRelationship_4(0))
            {
                layer1234BoolEq = true;
            }

            if (interLayerRelationship_4(1))
            {
                layer5678BoolEq = true;
            }

            if (layer1234BoolEq && layer5678BoolEq)
            {
                layerABoolEq = true;
            }

            if (interLayerRelationship_4(2))
            {
                layer9101112BoolEq = true;
            }

            if (interLayerRelationship_4(3))
            {
                layer13141516BoolEq = true;
            }

            if (layer9101112BoolEq && layer13141516BoolEq)
            {
                layerBBoolEq = true;
            }

            if (isLayerActive(0) || isLayerActive(1))
            {
                if (layerABoolEq)
                {
                    boolString += "{";
                }
                if (layer1234BoolEq)
                {
                    boolString += "[";
                }

                if (isLayerActive(0))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 0) == 1)
                    {
                        layer12BoolEq = "NOT ";
                    }
                    if (listOfSettings[0].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer12BoolEq += "layer0";
                    }
                    else
                    {
                        layer12BoolEq += listOfSettings[0].getString(EntropyLayerSettings.properties_s.name);
                    }
                }

                if (interLayerRelationship_2(0))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 0) == 0)
                    {
                        layer12BoolEq += " AND ";
                    }
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 0) == 1)
                    {
                        layer12BoolEq += " OR ";
                    }
                }

                if (isLayerActive(1))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 1) == 1)
                    {
                        layer12BoolEq += " NOT ";
                    }
                    if (listOfSettings[1].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer12BoolEq += "layer1";
                    }
                    else
                    {
                        layer12BoolEq += listOfSettings[1].getString(EntropyLayerSettings.properties_s.name);
                    }
                }
                if (interLayerRelationship_2(0))
                {
                    boolString += "(";
                }
                boolString += layer12BoolEq;
                if (interLayerRelationship_2(0))
                {
                    boolString += ")";
                }
                boolString += " ";
            }

            if (isLayerActive(2) || isLayerActive(3))
            {
                if (isLayerActive(2))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 2) == 1)
                    {
                        layer34BoolEq = "NOT ";
                    }
                    if (listOfSettings[2].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer34BoolEq += "layer2";
                    }
                    else
                    {
                        layer34BoolEq += listOfSettings[2].getString(EntropyLayerSettings.properties_s.name);
                    }
                }
                if (interLayerRelationship_2(1))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 1) == 0)
                    {
                        layer34BoolEq += " AND ";
                    }
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 1) == 1)
                    {
                        layer34BoolEq += " OR ";
                    }
                }
                if (isLayerActive(3))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 3) == 1)
                    {
                        layer34BoolEq += " NOT ";
                    }
                    if (listOfSettings[3].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer34BoolEq += "layer3";
                    }
                    else
                    {
                        layer34BoolEq += listOfSettings[3].getString(EntropyLayerSettings.properties_s.name);
                    }
                }

                if (layer1234BoolEq)
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 0) == 0)
                    {
                        boolString += "AND ";
                    }
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 0) == 1)
                    {
                        boolString += "OR ";
                    }
                }

                if (interLayerRelationship_2(1))
                {
                    boolString += "(";
                }
                boolString += layer34BoolEq;
                if (interLayerRelationship_2(1))
                {
                    boolString += ")";
                }

                if (layer1234BoolEq)
                {
                    boolString += "]";
                }
                if (layerABoolEq)
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, 0) == 0)
                    {
                        boolString += " AND ";
                    }
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, 0) == 1)
                    {
                        boolString += " OR ";
                    }
                }

                if (isLayerActive(4) || isLayerActive(5))
                {
                    if (layer5678BoolEq)
                    {
                        boolString += "[";
                    }

                    if (isLayerActive(4))
                    {
                        if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 4) == 1)
                        {
                            layer56BoolEq = "NOT ";
                        }
                        if (listOfSettings[4].getString(EntropyLayerSettings.properties_s.name) == "")
                        {
                            layer56BoolEq += "layer4";
                        }
                        else
                        {
                            layer56BoolEq += listOfSettings[4].getString(EntropyLayerSettings.properties_s.name);
                        }
                    }
                    if (interLayerRelationship_2(2))
                    {
                        if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 2) == 0)
                        {
                            layer56BoolEq += " AND ";
                        }
                        if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 2) == 1)
                        {
                            layer56BoolEq += " OR ";
                        }
                    }
                    if (isLayerActive(5))
                    {
                        if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 5) == 1)
                        {
                            layer56BoolEq += " NOT ";
                        }
                        if (listOfSettings[5].getString(EntropyLayerSettings.properties_s.name) == "")
                        {
                            layer56BoolEq += "layer5";
                        }
                        else
                        {
                            layer56BoolEq += listOfSettings[5].getString(EntropyLayerSettings.properties_s.name);
                        }
                    }
                    if (interLayerRelationship_2(2))
                    {
                        boolString += "(";
                    }
                    boolString += layer56BoolEq;
                    if (interLayerRelationship_2(2))
                    {
                        boolString += ")";
                    }
                }

                if (isLayerActive(6) || isLayerActive(7))
                {
                    if (listOfSettings[6].getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                    {
                        if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 6) == 1)
                        {
                            layer78BoolEq = "NOT ";
                        }
                        if (listOfSettings[6].getString(EntropyLayerSettings.properties_s.name) == "")
                        {
                            layer78BoolEq += "layer6";
                        }
                        else
                        {
                            layer78BoolEq += listOfSettings[6].getString(EntropyLayerSettings.properties_s.name);
                        }
                    }
                    if (interLayerRelationship_2(3))
                    {
                        if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 3) == 0)
                        {
                            layer78BoolEq += " AND ";
                        }
                        if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 3) == 1)
                        {
                            layer78BoolEq += " OR ";
                        }
                    }
                    if (isLayerActive(7))
                    {
                        if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 7) == 1)
                        {
                            layer78BoolEq += " NOT ";
                        }
                        if (listOfSettings[7].getString(EntropyLayerSettings.properties_s.name) == "")
                        {
                            layer78BoolEq += "layer7";
                        }
                        else
                        {
                            layer78BoolEq += listOfSettings[7].getString(EntropyLayerSettings.properties_s.name);
                        }
                    }

                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 1) == 0)
                    {
                        boolString += " AND ";
                    }
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 1) == 1)
                    {
                        boolString += " OR ";
                    }

                    if (interLayerRelationship_2(3))
                    {
                        boolString += "(";
                    }
                    boolString += layer78BoolEq;
                    if (interLayerRelationship_2(3))
                    {
                        boolString += ")";
                    }

                    if (layer5678BoolEq)
                    {
                        boolString += "]";
                    }
                }

                if (layerABoolEq)
                {
                    boolString += "}";
                }
                boolString += " ";
            }

            switch (simulationSettings.getValue(EntropySettings.properties_i.oType))
            {
                case (int)CommonVars.calcModes.area: // area
                    boolString += "AND";
                    break;
                case (int)CommonVars.calcModes.enclosure_spacing_overlap: // spacing
                    boolString += "MIN SPACE/OVERLAP/ENCL TO/WITH";
                    break;
                case (int)CommonVars.calcModes.chord: // chords
                    boolString += "MIN CHORDS WITH";
                    break;
                case (int)CommonVars.calcModes.angle: // angle
                    boolString += "MIN INTERSECTION ANGLE WITH";
                    break;
            }

            boolString += " ";

            if (isLayerActive(8) || isLayerActive(9))
            {
                if (layerBBoolEq)
                {
                    boolString += "{";
                }

                if (layer9101112BoolEq)
                {
                    boolString += "[";
                }

                if (isLayerActive(8))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 8) == 1)
                    {
                        layer910BoolEq = "NOT ";
                    }
                    if (listOfSettings[8].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer910BoolEq += "layer8";
                    }
                    else
                    {
                        layer910BoolEq += listOfSettings[8].getString(EntropyLayerSettings.properties_s.name);
                    }
                }
                if (interLayerRelationship_2(4))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 4) == 0)
                    {
                        layer910BoolEq += " AND ";
                    }
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 4) == 1)
                    {
                        layer910BoolEq += " OR ";
                    }
                }
                if (isLayerActive(9))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 9) == 1)
                    {
                        layer910BoolEq += " NOT ";
                    }
                    if (listOfSettings[9].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer910BoolEq += "layer9";
                    }
                    else
                    {
                        layer910BoolEq += listOfSettings[9].getString(EntropyLayerSettings.properties_s.name);
                    }
                }

                if (interLayerRelationship_2(4))
                {
                    boolString += "(";
                }

                boolString += layer910BoolEq;

                if (interLayerRelationship_2(4))
                {
                    boolString += ")";
                }
            }

            if (isLayerActive(10) || isLayerActive(11))
            {
                if (isLayerActive(10))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 10) == 1)
                    {
                        layer1112BoolEq = "NOT ";
                    }
                    if (listOfSettings[10].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer1112BoolEq += "layer10";
                    }
                    else
                    {
                        layer1112BoolEq += listOfSettings[10].getString(EntropyLayerSettings.properties_s.name);
                    }
                }
                if (interLayerRelationship_2(5))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 5) == 0)
                    {
                        layer1112BoolEq += " AND ";
                    }
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 5) == 1)
                    {
                        layer1112BoolEq += " OR ";
                    }
                }
                if (isLayerActive(11))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 11) == 1)
                    {
                        layer1112BoolEq += " NOT ";
                    }
                    if (listOfSettings[11].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer1112BoolEq += "layer11";
                    }
                    else
                    {
                        layer1112BoolEq += listOfSettings[11].getString(EntropyLayerSettings.properties_s.name);
                    }
                }

                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 2) == 0)
                {
                    boolString += " AND ";
                }
                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 2) == 1)
                {
                    boolString += " OR ";
                }

                if (interLayerRelationship_2(5))
                {
                    boolString += "(";
                }
                boolString += layer1112BoolEq;
                if (interLayerRelationship_2(5))
                {
                    boolString += ")";
                }

                if (layer9101112BoolEq)
                {
                    boolString += "]";
                }
            }

            if (layerBBoolEq)
            {
                boolString += " ";
                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, 1) == 0)
                {
                    boolString += "AND ";
                }
                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.eightLayer, 1) == 1)
                {
                    boolString += "OR ";
                }
            }

            if (layer13141516BoolEq)
            {
                boolString += "[";
            }

            if (isLayerActive(12))
            {
                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 12) == 1)
                {
                    layer1314BoolEq = "NOT ";
                }
                if (listOfSettings[12].getString(EntropyLayerSettings.properties_s.name) == "")
                {
                    layer1314BoolEq += "layer12";
                }
                else
                {
                    layer1314BoolEq += listOfSettings[12].getString(EntropyLayerSettings.properties_s.name);
                }
            }
            if (interLayerRelationship_2(6))
            {
                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 6) == 0)
                {
                    layer1314BoolEq += " AND ";
                }
                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 6) == 1)
                {
                    layer1314BoolEq += " OR ";
                }
            }
            if (isLayerActive(13))
            {
                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 13) == 1)
                {
                    layer1314BoolEq += " NOT ";
                }
                if (listOfSettings[13].getString(EntropyLayerSettings.properties_s.name) == "")
                {
                    layer1314BoolEq += "layer13";
                }
                else
                {
                    layer1314BoolEq += listOfSettings[13].getString(EntropyLayerSettings.properties_s.name);
                }
            }

            if (interLayerRelationship_2(6))
            {
                boolString += "(";
            }
            boolString += layer1314BoolEq;
            if (interLayerRelationship_2(6))
            {
                boolString += ")";
            }

            if (isLayerActive(14) || isLayerActive(15))
            {
                if (isLayerActive(14))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 14) == 1)
                    {
                        layer1516BoolEq = "NOT ";
                    }
                    if (listOfSettings[14].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer1516BoolEq += "layer14";
                    }
                    else
                    {
                        layer1516BoolEq += listOfSettings[14].getString(EntropyLayerSettings.properties_s.name);
                    }
                }
                if (interLayerRelationship_2(7))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 7) == 0)
                    {
                        layer1516BoolEq += " AND ";
                    }
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.twoLayer, 7) == 1)
                    {
                        layer1516BoolEq += " OR ";
                    }
                }
                if (isLayerActive(15))
                {
                    if (simulationSettings.getOperatorValue(EntropySettings.properties_o.layer, 15) == 1)
                    {
                        layer1516BoolEq += " NOT ";
                    }
                    if (listOfSettings[15].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        layer1516BoolEq += "layer15";
                    }
                    else
                    {
                        layer1516BoolEq += listOfSettings[15].getString(EntropyLayerSettings.properties_s.name);
                    }
                }

                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 3) == 0)
                {
                    boolString += " AND ";
                }
                if (simulationSettings.getOperatorValue(EntropySettings.properties_o.fourLayer, 3) == 1)
                {
                    boolString += " OR ";
                }

                if (interLayerRelationship_2(7))
                {
                    boolString += "(";
                }
                boolString += layer1516BoolEq;
                if (interLayerRelationship_2(7))
                {
                    boolString += ")";
                }

                if (layer13141516BoolEq)
                {
                    boolString += "]";
                }
            }
            if (layerBBoolEq)
            {
                boolString += "}";
            }

            linesToWrite.Add("Equation: " + boolString);
        }

        public void getSimulationSettings_implant(List<string> linesToWrite)
        {
            pGetSimulationSettings_implant(linesToWrite);
        }

        void pGetSimulationSettings_implant(List<string> linesToWrite)
        {
            // Simulation settings
            linesToWrite.Add("Simulation Settings:");
            linesToWrite.Add("  Number of Cases: " + implantSimulationSettings.getValue(EntropySettings.properties_i.nCases));
            linesToWrite.Add("  Edge resolution: " + implantSimulationSettings.getResolution());
            linesToWrite.Add("  Corner segments: " + implantSimulationSettings.getValue(EntropySettings.properties_i.cSeg));
            if (implantSimulationSettings.getValue(EntropySettings.properties_i.optC) == 1)
            {
                linesToWrite.Add("  Override corner angle step by edge resolution: yes");
            }
            else
            {
                linesToWrite.Add("  Override corner angle step by edge resolution: no");
            }
            linesToWrite.Add("RNG: " + commonRNG.rngTypes[implantSimulationSettings.getValue(EntropySettings.properties_i.rngType)]);
            linesToWrite.Add("");
        }

        public void getSimulationSettings(List<string> linesToWrite)
        {
            pGetSimulationSettings(linesToWrite);
        }

        void pGetSimulationSettings(List<string> linesToWrite)
        {
            // Simulation settings
            linesToWrite.Add("Simulation Settings:");
            linesToWrite.Add("  Number of Cases: " + simulationSettings.getValue(EntropySettings.properties_i.nCases));
            linesToWrite.Add("  Edge resolution: " + simulationSettings.getResolution());
            linesToWrite.Add("  Corner segments: " + simulationSettings.getValue(EntropySettings.properties_i.cSeg));
            if (simulationSettings.getValue(EntropySettings.properties_i.optC) == 1)
            {
                linesToWrite.Add("  Override corner angle step by edge resolution: yes");
            }
            else
            {
                linesToWrite.Add("  Override corner angle step by edge resolution: no");
            }
            if (simulationSettings.getValue(EntropySettings.properties_i.linkCDU) == 1)
            {
                linesToWrite.Add("  Linked tip/side CDU: yes");
            }
            else
            {
                linesToWrite.Add("  Linked tip/side CDU: no");
            }
            if (simulationSettings.getValue(EntropySettings.properties_i.ler) == 1)
            {
                linesToWrite.Add("  LER: LWR/sqrt(2)");
            }
            else
            {
                linesToWrite.Add("  LER: LWR/2");
            }
            linesToWrite.Add("RNG: " + commonRNG.rngTypes[simulationSettings.getValue(EntropySettings.properties_i.rngType)]);
            linesToWrite.Add("");
        }

        public void getLayerSettings(Int32 layer, ref List<string> linesToWrite, bool onlyActive)
        {
            pGetLayerSettings(layer, ref linesToWrite, onlyActive);
        }

        void pGetLayerSettings(Int32 layer, ref List<string> linesToWrite, bool onlyActive)
        {
            string layerName = "";
            if (!onlyActive || (onlyActive && (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.enabled) == 1)))
            {
                // Layer was active in simulation.
                string layerRefString = "Layer " + layer;
                layerName = listOfSettings[layer].getString(EntropyLayerSettings.properties_s.name);
                if (layerName == "")
                {
                    layerName = "layer" + layer;
                }
                if (!onlyActive)
                {
                    if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.enabled) == 1)
                    {
                        linesToWrite.Add("ACTIVE");
                    }
                    else
                    {
                        linesToWrite.Add("INACTIVE");
                    }
                }
                linesToWrite.Add(layerRefString + " Name: " + layerName);
                string shapeType = layerRefString + " Shape: ";
                int shapeParts = 1;
                bool externalShape = false;
                linesToWrite.Add(shapeType + availableShapes[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.shapeIndex)]);
                switch (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.shapeIndex))
                {
                    case (int)CentralProperties.typeShapes.L:
                        shapeParts = 2;
                        break;
                    case (int)CentralProperties.typeShapes.T:
                        shapeParts = 2;
                        break;
                    case (int)CentralProperties.typeShapes.U:
                        shapeParts = 2;
                        break;
                    case (int)CentralProperties.typeShapes.X:
                        shapeParts = 2;
                        break;
                    case (int)CentralProperties.typeShapes.S:
                        shapeParts = 3;
                        break;
                    case (int)CentralProperties.typeShapes.GEOCORE:
                        externalShape = true;
                        break;
                }
                if (externalShape)
                {
                    linesToWrite.Add("Layout File: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.file));

                    linesToWrite.Add("Layout Cell: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.structure));
                    linesToWrite.Add("Layout Layer/Datatype: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.lD));
                    linesToWrite.Add("Contouring: " + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.gCSEngine));
                    linesToWrite.Add("Per-Poly: " + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.perPoly));
                    linesToWrite.Add("Poly Fill Type: " + polyFillTypes[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.fill)]);
                    linesToWrite.Add("Horizontal Offset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset));
                    linesToWrite.Add("Vertical Offset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset));
                    linesToWrite.Add("Horizontal Overlay: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.xOL));
                    linesToWrite.Add("Horizontal Overlay RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.xOL_RNG));
                    linesToWrite.Add("Vertical Overlay: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.yOL));
                    linesToWrite.Add("Vertical Overlay RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.yOL_RNG));
                    linesToWrite.Add("Side Bias: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.sBias));
                    linesToWrite.Add("Side CDU: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.sCDU));
                    linesToWrite.Add("Side CDU RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.sCDU_RNG));
                    string[] tempArray = summaryFile_LitData(layer);
                    for (int i = 0; i < tempArray.Length; i++)
                    {
                        linesToWrite.Add(tempArray[i]);
                    }
                }
                else
                {
                    // Internal shape
                    for (int part = 0; part < shapeParts; part++)
                    {
                        linesToWrite.Add("Subshape " + part + ": ");
                        switch (part)
                        {
                            case 0:
                                linesToWrite.Add("  HLength: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s0HorLength));
                                linesToWrite.Add("  VLength: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s0VerLength));
                                linesToWrite.Add("  HOffset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s0HorOffset));
                                linesToWrite.Add("  VOffset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s0VerOffset));
                                linesToWrite.Add("  TipLocations: " + availableTipsLocations[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.shape0Tip)]);
                                break;
                            case 1:
                                linesToWrite.Add("  HLength: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s1HorLength));
                                linesToWrite.Add("  VLength: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s1VerLength));
                                linesToWrite.Add("  HOffset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s1HorOffset));
                                linesToWrite.Add("  VOffset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s1VerOffset));
                                linesToWrite.Add("  TipLocations: " + availableTipsLocations[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.shape1Tip)]);
                                break;
                            case 2:
                                linesToWrite.Add("  HLength: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s2HorLength));
                                linesToWrite.Add("  VLength: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s2VerLength));
                                linesToWrite.Add("  HOffset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s2HorOffset));
                                linesToWrite.Add("  VOffset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.s2VerOffset));
                                linesToWrite.Add("  TipLocations: " + availableTipsLocations[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.shape2Tip)]);
                                break;
                        }
                    }

                    // Subshape reference
                    linesToWrite.Add("SubshapeReference: " + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.subShapeIndex));
                    // Subshape positioning
                    linesToWrite.Add("PositionInSubshape: " + availableSubShapePositions[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.posIndex)]);

                    // Global offsets.
                    linesToWrite.Add("Horizontal Offset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.gHorOffset));
                    linesToWrite.Add("Vertical Offset: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.gVerOffset));

                    // Rotation
                    linesToWrite.Add("Rotation: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.rot));
                    // Wobble
                    linesToWrite.Add("Wobble: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.wobble));
                    linesToWrite.Add("Wobble RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.wobble_RNG));

                    // Biases
                    linesToWrite.Add("Side Bias: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.sBias));
                    linesToWrite.Add("HorTip Bias: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.hTBias));
                    linesToWrite.Add("  HorTip Bias +ve Var: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.hTPVar));
                    linesToWrite.Add("  HorTip Bias +ve Var RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.hTipPVar_RNG));
                    linesToWrite.Add("  HorTip Bias -ve Var: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.hTNVar));
                    linesToWrite.Add("  HorTip Bias -ve Var RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.hTipNVar_RNG));
                    linesToWrite.Add("VerTip Bias: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.vTBias));
                    linesToWrite.Add("  VerTip Bias +ve Var: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.vTPVar));
                    linesToWrite.Add("  VerTip Bias +ve Var RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.vTipPVar_RNG));
                    linesToWrite.Add("  VerTip Bias -ve Var: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.vTNVar));
                    linesToWrite.Add("  VerTip Bias -ve Var RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.vTipNVar_RNG));
                    linesToWrite.Add("Proximity Bias: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.pBias));
                    linesToWrite.Add("  Proximity Bias Isolated Distance: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.pBiasDist));
                    linesToWrite.Add("  Proximity Bias Side Rays: " + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.proxRays));

                    // Corner rounding.
                    linesToWrite.Add("Inner CRR: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.iCR));
                    linesToWrite.Add("  Inner CR Var: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.iCV));
                    linesToWrite.Add("  Inner CR Var RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.iCV_RNG));
                    linesToWrite.Add("Outer CRR: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.oCR));
                    linesToWrite.Add("  Outer CR Var: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.oCV));
                    linesToWrite.Add("  Outer CR Var RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.oCV_RNG));

                    // CDU/LWR
                    linesToWrite.Add("LWR: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.lwr));
                    linesToWrite.Add("LWR RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.lwr_RNG));
                    linesToWrite.Add("LWR Frequency: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.lwrFreq));
                    linesToWrite.Add("LWR Noise: " + noiseTypes[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.lwrType)]);
                    linesToWrite.Add("LWR2: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.lwr2));
                    linesToWrite.Add("LWR2 RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.lwr2_RNG));
                    linesToWrite.Add("LWR2 Frequency: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.lwr2Freq));
                    linesToWrite.Add("LWR2 Noise: " + noiseTypes[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.lwr2Type)]);
                    linesToWrite.Add("Side CDU: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.sCDU));
                    linesToWrite.Add("Side CDU RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.sCDU_RNG));
                    linesToWrite.Add("Tips CDU: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.tCDU));
                    linesToWrite.Add("Tips CDU RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.tCDU_RNG));

                    // Overlay
                    linesToWrite.Add("Horizontal Overlay: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.xOL));
                    linesToWrite.Add("Horizontal Overlay RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.xOL_RNG));
                    linesToWrite.Add("Vertical Overlay: " + listOfSettings[layer].getDecimal(EntropyLayerSettings.properties_decimal.yOL));
                    linesToWrite.Add("Vertical Overlay RNG Mapping: " + listOfSettings[layer].getString(EntropyLayerSettings.properties_s.yOL_RNG));
                    // Correlation data
                    string[] tempArray = summaryFile_LitData(layer);
                    for (int i = 0; i < tempArray.Length; i++)
                    {
                        linesToWrite.Add(tempArray[i]);
                    }
                }
                linesToWrite.Add("");
            }
        }

        string[] summaryFile_LitData(int layer)
        {
            try
            {
                string xOverlayCorrString = "X Overlay correlation: ";
                string yOverlayCorrString = "Y Overlay correlation: ";
                string xOverlayRefString = "";
                string yOverlayRefString = "";
                string xOverlayRefAvString = "";
                string yOverlayRefAvString = "";
                if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_av) == 1)
                {
                    xOverlayRefAvString = "ACTIVE: ";
                    xOverlayRefString = "INACTIVE: ";
                }
                else
                {
                    xOverlayRefString = "ACTIVE: ";
                    xOverlayRefAvString = "INACTIVE: ";
                }
                if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_av) == 1)
                {
                    yOverlayRefAvString = "ACTIVE: ";
                    yOverlayRefString = "INACTIVE: ";
                }
                else
                {
                    yOverlayRefString = "ACTIVE: ";
                    yOverlayRefAvString = "INACTIVE: ";
                }
                xOverlayRefString += "X Overlay reference layer: ";
                xOverlayRefAvString += "X Overlay reference layers: ";
                yOverlayRefString += "Y Overlay reference layer: ";
                yOverlayRefAvString += "Y Overlay reference layers: ";

                string cduCorrString = "CDU correlation: ";
                string tipCDUCorrString = "Tip CDU correlation: ";

                if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == -1)
                {
                    xOverlayCorrString += "None";
                }
                else
                {
                    if (listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_corr_ref)].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        xOverlayCorrString += listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_corr_ref)].getString(EntropyLayerSettings.properties_s.name);
                    }
                    else
                    {
                        xOverlayCorrString += "layer" + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_corr_ref);
                    }
                }

                if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == -1)
                {
                    yOverlayCorrString += "None";
                }
                else
                {
                    if (listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_corr_ref)].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        yOverlayCorrString += listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_corr_ref)].getString(EntropyLayerSettings.properties_s.name);
                    }
                    else
                    {
                        yOverlayCorrString += "layer" + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_corr_ref);
                    }
                }

                if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_ref) == -1)
                {
                    xOverlayRefString += "None";
                }
                else
                {
                    if (listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_ref)].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        xOverlayRefString += listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_ref)].getString(EntropyLayerSettings.properties_s.name);
                    }
                    else
                    {
                        xOverlayRefString += "layer" + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.xOL_ref);
                    }
                }

                if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_ref) == -1)
                {
                    yOverlayRefString += "None";
                }
                else
                {
                    if (listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_ref)].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        yOverlayRefString += listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_ref)].getString(EntropyLayerSettings.properties_s.name);
                    }
                    else
                    {
                        yOverlayRefString += "layer" + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.yOL_ref);
                    }
                }

                string xOLAV_layers = "";

                for (int tmp = 0; tmp < listOfSettings[layer].getIntArray(EntropyLayerSettings.properties_intarray.xOLRefs).Length; tmp++)
                {
                    if (listOfSettings[layer].getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, tmp) == 1)
                    {
                        xOLAV_layers += (tmp + 1) + " ";
                    }
                }

                if (xOLAV_layers == "")
                {
                    xOverlayRefAvString += "None";
                }
                else
                {
                    xOverlayRefAvString += xOLAV_layers;
                }

                string yOLAV_layers = "";

                for (int tmp = 0; tmp < listOfSettings[layer].getIntArray(EntropyLayerSettings.properties_intarray.yOLRefs).Length; tmp++)
                {
                    if (listOfSettings[layer].getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, tmp) == 1)
                    {
                        yOLAV_layers += (tmp + 1) + " ";
                    }
                }

                if (yOLAV_layers == "")
                {
                    yOverlayRefAvString += "None";
                }
                else
                {
                    yOverlayRefAvString += yOLAV_layers;
                }


                if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == -1)
                {
                    cduCorrString += "None";
                }
                else
                {
                    if (listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.CDU_corr_ref)].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        cduCorrString += listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.CDU_corr_ref)].getString(EntropyLayerSettings.properties_s.name);
                    }
                    else
                    {
                        cduCorrString += "layer" + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.CDU_corr_ref);
                    }
                }


                if (listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == -1)
                {
                    tipCDUCorrString += "None";
                }
                else
                {
                    if (listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref)].getString(EntropyLayerSettings.properties_s.name) == "")
                    {
                        tipCDUCorrString += listOfSettings[listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref)].getString(EntropyLayerSettings.properties_s.name);
                    }
                    else
                    {
                        tipCDUCorrString += "layer" + listOfSettings[layer].getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref);
                    }
                }

                string[] returnData = new string[8];
                returnData[0] = xOverlayCorrString;
                returnData[1] = yOverlayCorrString;
                returnData[2] = xOverlayRefString;
                returnData[3] = yOverlayRefString;
                returnData[4] = xOverlayRefAvString;
                returnData[5] = yOverlayRefAvString;
                returnData[6] = cduCorrString;
                returnData[7] = tipCDUCorrString;
                return returnData;
            }
            catch (Exception)
            {
                return new string[] { "" };
            }
        }

        public void getUtilitiesSettings(List<string> linesToWrite)
        {
            pGetUtilitiesSettings(linesToWrite);
        }

        void pGetUtilitiesSettings(List<string> linesToWrite)
        {
            linesToWrite.Add("Utilities Settings:");
            linesToWrite.Add("  Email Server: " + nonSimulationSettings.host);
            linesToWrite.Add("  Email Port: " + nonSimulationSettings.port);
            linesToWrite.Add("  Email Port: " + nonSimulationSettings.ssl);
            linesToWrite.Add("  Email Address: " + nonSimulationSettings.emailAddress);
            linesToWrite.Add("  Email On Completion: " + nonSimulationSettings.emailOnCompletion);
            linesToWrite.Add("  Email Per Job: " + nonSimulationSettings.emailPerJob);
        }

        public void getDOESettings(List<string> linesToWrite)
        {
            pGetDOESettings(linesToWrite);
        }

        void pGetDOESettings(List<string> linesToWrite)
        {
            // Layout DOE settings
            string layersAffected = "";
            for (int i = 0; i < simulationSettings.getDOESettings().getLayersAffected().Length; i++)
            {
                if (simulationSettings.getDOESettings().getLayerAffected(i) == 1)
                {
                    string layerName = listOfSettings[i].getString(EntropyLayerSettings.properties_s.name);
                    if (layerName == "")
                    {
                        layerName = "(layer" + i + ") ";
                    }
                    layersAffected += layerName;
                }
            }
            if (layersAffected != "")
            {
                linesToWrite.Add("Layout DOE Settings:");
                linesToWrite.Add("  Affected layers: " + layersAffected);
                linesToWrite.Add("  Column Offset: " + simulationSettings.getDOESettings().getDouble(DOESettings.properties_d.colOffset));
                linesToWrite.Add("  Row Offset: " + simulationSettings.getDOESettings().getDouble(DOESettings.properties_d.rowOffset));
                linesToWrite.Add("  Column Pitch: " + simulationSettings.getDOESettings().getDouble(DOESettings.properties_d.colPitch));
                linesToWrite.Add("  Row Pitch: " + simulationSettings.getDOESettings().getDouble(DOESettings.properties_d.rowPitch));
            }
        }

        public void setHashes()
        {
            pSetHashes();
        }

        void pSetHashes()
        {
            geoCoreHash = Utils.GetMD5Hash(geoCore_Handlers);
            listOfSettingsHash = Utils.GetMD5Hash(listOfSettings);
            entropyGeoHash = Utils.GetMD5Hash(simulationSettings);
            implantHash = Utils.GetMD5Hash(implantSettings);
            entropyImplantHash = Utils.GetMD5Hash(implantSimulationSettings);
            changed = false;
        }

        public string[] getHashes()
        {
            return pGetHashes();
        }

        string[] pGetHashes()
        {
            string[] hashes = new string[5];
            hashes[0] = Utils.GetMD5Hash(geoCore_Handlers);
            hashes[1] = Utils.GetMD5Hash(listOfSettings);
            hashes[2] = Utils.GetMD5Hash(simulationSettings);
            hashes[3] = Utils.GetMD5Hash(implantSettings);
            hashes[4] = Utils.GetMD5Hash(implantSimulationSettings);

            return hashes;
        }

        public void setHashes(string[] hashes)
        {
            pSetHashes(hashes);
        }

        void pSetHashes(string[] hashes)
        {
            geoCoreHash = hashes[0];
            listOfSettingsHash = hashes[1];
            entropyGeoHash = hashes[2];
            implantHash = hashes[3];
            entropyImplantHash = hashes[4];
        }

        public void checkChanged()
        {
            pCheckChanged();
        }

        void pCheckChanged()
        {
            string tmp = Utils.GetMD5Hash(geoCore_Handlers);
            if ((geoCoreHash != null) && (tmp != geoCoreHash))
            {
                changed = true;
                return;
            }

            tmp = Utils.GetMD5Hash(listOfSettings);
            if ((listOfSettingsHash != null) && (tmp != listOfSettingsHash))
            {
                changed = true;
                return;
            }

            tmp = Utils.GetMD5Hash(simulationSettings);
            if ((entropyGeoHash != null) && (tmp != entropyGeoHash))
            {
                changed = true;
                return;
            }

            tmp = Utils.GetMD5Hash(implantSettings);
            if ((implantHash != null) && (tmp != implantHash))
            {
                changed = true;
                return;
            }

            tmp = Utils.GetMD5Hash(implantSimulationSettings);
            if ((entropyImplantHash != null) && (tmp != entropyImplantHash))
            {
                changed = true;
                return;
            }

            changed = false;
        }

        public bool nonGaussianInputs()
        {
            return pNonGaussianInputs();
        }

        bool pNonGaussianInputs()
        {
            bool nonGaussianvalues = true;
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                nonGaussianvalues = listOfSettings[i].nonGaussianValues();
                if (nonGaussianvalues)
                {
                    break;
                }
            }

            return nonGaussianvalues;
        }
    }
}
