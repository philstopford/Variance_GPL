using System;
using geoCoreLib;

namespace Variance
{
    public partial class MainForm
    {
        void switchSimulationLayersOver(Int32 aIndex, Int32 bIndex)
        {
            // Extract our related settings for migration to the destination layer.
            int a_useForDOE = commonVars.getSimulationSettings().getDOESettings().getLayerAffected(aIndex);

            // Make a copy of our a settings before we clobber them
            EntropyLayerSettings aSettings = new EntropyLayerSettings(commonVars.getLayerSettings(aIndex), gdsOnly: false);

            // Need to also backup the 'a' geoCore definitions.
            GeoCoreHandler ach = new GeoCoreHandler();
            ach.readValues(commonVars.getGeoCoreHandler(aIndex));

            // Figure out whether we have inter-layer properties that need to be remapped.
            bool compXOLRefA = commonVars.getLayerSettings(bIndex).getInt(EntropyLayerSettings.properties_i.xOL_ref) == aIndex;
            bool compXOLCorrA = commonVars.getLayerSettings(bIndex).getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == aIndex;
            bool compYOLRefA = commonVars.getLayerSettings(bIndex).getInt(EntropyLayerSettings.properties_i.yOL_ref) == aIndex;
            bool compYOLCorrA = commonVars.getLayerSettings(bIndex).getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == aIndex;
            bool compXOLAvRefA = commonVars.getLayerSettings(bIndex).getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, aIndex) == 1;
            bool compYOLAvRefA = commonVars.getLayerSettings(bIndex).getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, aIndex) == 1;
            bool compSCDUCorrA = commonVars.getLayerSettings(bIndex).getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == aIndex;
            bool compTCDUCorrA = commonVars.getLayerSettings(bIndex).getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == aIndex;
            bool compBoolLyrAA = commonVars.getLayerSettings(bIndex).getInt(EntropyLayerSettings.properties_i.bLayerA) == aIndex;
            bool compBoolLyrBA = commonVars.getLayerSettings(bIndex).getInt(EntropyLayerSettings.properties_i.bLayerB) == aIndex;

            bool compXOLRefB = commonVars.getLayerSettings(aIndex).getInt(EntropyLayerSettings.properties_i.xOL_ref) == bIndex;
            bool compXOLCorrB = commonVars.getLayerSettings(aIndex).getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == bIndex;
            bool compYOLRefB = commonVars.getLayerSettings(aIndex).getInt(EntropyLayerSettings.properties_i.yOL_ref) == bIndex;
            bool compYOLCorrB = commonVars.getLayerSettings(aIndex).getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == bIndex;
            bool compXOLAvRefB = commonVars.getLayerSettings(aIndex).getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, bIndex) == 1;
            bool compYOLAvRefB = commonVars.getLayerSettings(aIndex).getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, bIndex) == 1;
            bool compSCDUCorrB = commonVars.getLayerSettings(aIndex).getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == bIndex;
            bool compTCDUCorrB = commonVars.getLayerSettings(aIndex).getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == bIndex;
            bool compBoolLyrAB = commonVars.getLayerSettings(aIndex).getInt(EntropyLayerSettings.properties_i.bLayerA) == bIndex;
            bool compBoolLyrBB = commonVars.getLayerSettings(aIndex).getInt(EntropyLayerSettings.properties_i.bLayerB) == bIndex;

            // Copy from b to a
            copy(bIndex);
            paste(aIndex);

            // Fix up layer settings.
            if (compXOLRefA)
            {
                commonVars.getLayerSettings(aIndex).setInt(EntropyLayerSettings.properties_i.xOL_ref, bIndex);
            }
            if (compXOLCorrA)
            {
                commonVars.getLayerSettings(aIndex).setInt(EntropyLayerSettings.properties_i.xOL_corr_ref, bIndex);
            }
            if (compYOLRefA)
            {
                commonVars.getLayerSettings(aIndex).setInt(EntropyLayerSettings.properties_i.yOL_ref, bIndex);
            }
            if (compYOLCorrA)
            {
                commonVars.getLayerSettings(aIndex).setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, bIndex);
            }
            if (compXOLAvRefA)
            {
                commonVars.getLayerSettings(aIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, bIndex, 1);
            }
            if (compYOLAvRefA)
            {
                commonVars.getLayerSettings(aIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, bIndex, 1);
            }
            if (compSCDUCorrA)
            {
                commonVars.getLayerSettings(aIndex).setInt(EntropyLayerSettings.properties_i.CDU_corr_ref, bIndex);
            }
            if (compTCDUCorrA)
            {
                commonVars.getLayerSettings(aIndex).setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, bIndex);
            }
            if (compBoolLyrAA)
            {
                commonVars.getLayerSettings(aIndex).setInt(EntropyLayerSettings.properties_i.bLayerA, bIndex);
            }
            if (compBoolLyrBA)
            {
                commonVars.getLayerSettings(aIndex).setInt(EntropyLayerSettings.properties_i.bLayerB, bIndex);
            }

            // Since we changed something in the fix-up, we should refresh the layer UI with the modified settings.
            // Should not be necessary any more.
            // setLayerUIFromSettings(aIndex);

            // Copy from back-up a Settings to b
            commonVars.setCopy(aIndex);
            commonVars.setLayerSettings(aSettings, bIndex, false);
            commonVars.getSimulationSettings().getDOESettings().setLayerAffected(layer: bIndex, a_useForDOE);
            commonVars.getGeoCoreHandler(bIndex).readValues(ach);

            // Fix up layer settings.
            if (compXOLRefB)
            {
                commonVars.getLayerSettings(bIndex).setInt(EntropyLayerSettings.properties_i.xOL_ref, aIndex);
            }
            if (compXOLCorrB)
            {
                commonVars.getLayerSettings(bIndex).setInt(EntropyLayerSettings.properties_i.xOL_corr_ref, aIndex);
            }
            if (compYOLRefB)
            {
                commonVars.getLayerSettings(bIndex).setInt(EntropyLayerSettings.properties_i.yOL_ref, aIndex);
            }
            if (compYOLCorrB)
            {
                commonVars.getLayerSettings(bIndex).setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, aIndex);
            }
            if (compXOLAvRefB)
            {
                commonVars.getLayerSettings(bIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, aIndex, 1);
            }
            if (compYOLAvRefB)
            {
                commonVars.getLayerSettings(bIndex).setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, aIndex, 1);
            }
            if (compSCDUCorrB)
            {
                commonVars.getLayerSettings(bIndex).setInt(EntropyLayerSettings.properties_i.CDU_corr_ref, aIndex);
            }
            if (compTCDUCorrB)
            {
                commonVars.getLayerSettings(bIndex).setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, aIndex);
            }
            if (compBoolLyrAB)
            {
                commonVars.getLayerSettings(bIndex).setInt(EntropyLayerSettings.properties_i.bLayerA, aIndex);
            }
            if (compBoolLyrBB)
            {
                commonVars.getLayerSettings(bIndex).setInt(EntropyLayerSettings.properties_i.bLayerB, aIndex);
            }

            commonVars.clearCopy();

            switchSimulationLayers_FixDeps(aIndex, bIndex);

            // Make a single run when on the right tab, to populate the preview structures, and only if simulation not running.
            if (!commonVars.isSimRunning() && (getSubTabSelectedIndex() == (int)CommonVars.twoDTabNames.settings))
            {
                entropyControl.update(commonVars);
            }

            reviewBooleanInputs();

            // Set our viewports up.
            double[] aViewport = getViewportCamera(aIndex);
            setViewportCamera(aIndex, getViewportCamera(bIndex));
            setViewportCamera(bIndex, aViewport);

            drawSimulationPanelHandler(false);
        }

        void switchSimulationLayers_FixDeps(int aIndex, int bIndex)
        {
            bool alreadyFrozen = globalUIFrozen;

            if (!alreadyFrozen)
            {
                globalUIFrozen = true;
            }

            // Scan other layers for dependencies on our remapped layers and adjust as needed.
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if ((i == aIndex) || (i == bIndex))
                {
                    continue;
                }

                if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.xOL_ref) == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.xOL_ref, bIndex);
                }

                if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.yOL_ref) == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.yOL_ref, bIndex);
                }

                if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.xOL_corr_ref) == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.xOL_corr_ref, bIndex);
                }

                if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, bIndex);
                }

                if (commonVars.getLayerSettings(i).getIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, aIndex) == 1)
                {
                    commonVars.getLayerSettings(i).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, aIndex, 0);
                    commonVars.getLayerSettings(i).setIntArrayValue(EntropyLayerSettings.properties_intarray.xOLRefs, bIndex, 1);
                }

                if (commonVars.getLayerSettings(i).getIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, aIndex) == 1)
                {
                    commonVars.getLayerSettings(i).setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, aIndex, 0);
                    commonVars.getLayerSettings(i).setIntArrayValue(EntropyLayerSettings.properties_intarray.yOLRefs, bIndex, 1);
                }

                if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.yOL_corr_ref) == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.yOL_corr_ref, bIndex);
                }

                if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.CDU_corr_ref) == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.CDU_corr_ref, bIndex);
                }

                if (commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.tCDU_corr_ref) == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.tCDU_corr_ref, bIndex);
                }

                int boolLayer1 = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.bLayerA);

                if (boolLayer1 == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.bLayerA, bIndex);
                }
                else if (boolLayer1 == bIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.bLayerA, aIndex);
                }


                int boolLayer2 = commonVars.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.bLayerB);

                if (boolLayer2 == bIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.bLayerB, aIndex);
                }
                else if (boolLayer2 == aIndex)
                {
                    commonVars.getLayerSettings(i).setInt(EntropyLayerSettings.properties_i.bLayerB, bIndex);
                }
            }

            if (!alreadyFrozen)
            {
                globalUIFrozen = false;
            }
        }

        void switchSimulationLayers12Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(0, 1);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers23Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(1, 2);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers34Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(2, 3);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers56Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(4, 5);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers67Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(5, 6);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers78Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(6, 7);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers910Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(8, 9);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers1011Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(9, 10);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers1112Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(10, 11);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers1314Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(12, 13);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers1415Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(13, 14);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers1516Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(14, 15);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers15Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(0, 4);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers26Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(1, 5);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers37Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(2, 6);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers48ver(object sender, EventArgs e)
        {
            switchSimulationLayersOver(3, 7);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers913Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(8, 12);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers1115Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(10, 14);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationLayers1216Over(object sender, EventArgs e)
        {
            switchSimulationLayersOver(11, 15);
            entropySettingsChanged(sender, e);
        }

        void switchSimulationAllALayersOver(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                switchSimulationLayersOver(i, i + 4);
            }
            entropySettingsChanged(sender, e);
        }

        void switchSimulationAllBLayersOver(object sender, EventArgs e)
        {
            for (int i = 8; i < 12; i++)
            {
                switchSimulationLayersOver(i, i + 4);
            }
            entropySettingsChanged(sender, e);
        }
    }
}