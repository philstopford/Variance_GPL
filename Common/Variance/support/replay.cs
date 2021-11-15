using System;
using System.Collections.Generic;
using System.IO;
using Error;
using utility;
using System.Threading.Tasks;

namespace Variance
{
    public class Replay
    {
        bool valid;

        public bool isValid()
        {
            return pIsValid();
        }

        bool pIsValid()
        {
            return valid;
        }

        ChaosSettings chaosSettings;
        public ChaosSettings getChaosSettings()
        {
            return pGetChaosSettings();
        }

        ChaosSettings pGetChaosSettings()
        {
            return chaosSettings;
        }

        string result;
        public string getResult()
        {
            return pGetResult();
        }

        string pGetResult()
        {
            return result;
        }

        CommonVars commonVars_;
        List<string[]> parsed;
        int max;
        string listOfSettingsHash, entropyGeoHash, geoCoreHash;
        int col, row; // for tile extraction.

        public enum properties_i { max, col, row }

        public Int32 getValue(properties_i p)
        {
            return pGetValue(p);
        }

        Int32 pGetValue(properties_i p)
        {
            int ret = 0;
            switch (p)
            {
                case properties_i.col:
                    ret = col;
                    break;
                case properties_i.row:
                    ret = row;
                    break;
                case properties_i.max:
                    ret = max;
                    break;
            }
            return ret;
        }

        public Replay(ref CommonVars commonVars)
        {
            pReplay(ref commonVars);
        }

        public void replay_loadCSV(string filename)
        {
            pLoadCSV(filename);
        }

        void pReplay(ref CommonVars commonVars)
        {
            commonVars_ = commonVars;
            parsed = new List<string[]>();
            pReset();
        }

        void pLoadCSV(string filename)
        {
            try
            {
                // Carve up the filename to see whether we have col and row specified.
                string[] tokens = filename.Split(new[] { '_' });
                // Format is of the form _col0_row0.csv
                string rowString = tokens[^1].Split(new[] { '.' })[0];
                string colString = tokens[^2].Split(new[] { '_' })[0];

                // Attempting to bullet-proof things against random user naming.
                if ((rowString.Substring(0, 3) == "row") && (rowString.Substring(0, 3) == "col"))
                {
                    // Have to fix the indices to align with the expectations in the engine.
                    rowString = rowString.Substring(3, rowString.Length - 3);
                    row = Convert.ToInt32(rowString) + 1;
                    colString = colString.Substring(3, colString.Length - 3);
                    col = Convert.ToInt32(colString) + 1;
                }

                // Need to set the simulation settings data to match our DOE tile.

            }
            catch (Exception)
            {

            }

            try
            {
                string[] lines = File.ReadAllLines(filename);

                // Header, and two hash lines make 3 - we also expect at least one result.
                if (lines.Length < 4)
                {
                    throw new Exception("Invalid file structure");
                }

                // Check for replay hashes in the CSV file.
                Array.Reverse(lines); // speed up the search.                

                bool lFound = false;
                int lIndex = -1;
                bool ehFound = false;
                int eIndex = -1;
                bool gFound = false;
                int gIndex = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("lHash"))
                    {
                        lFound = true;
                        lIndex = i;
                        // Validate that we got the expected strings and extract the checksum data.
                        try
                        {
                            listOfSettingsHash = lines[i].Split(new[] { ',' })[1];
                        }
                        catch (Exception)
                        {
                            throw new Exception("Checksum data not found.");
                        }
                    }
                    else if (lines[i].StartsWith("eHash"))
                    {
                        ehFound = true;
                        eIndex = i;
                        // Validate that we got the expected strings and extract the checksum data.
                        try
                        {
                            entropyGeoHash = lines[i].Split(new[] { ',' })[1];
                        }
                        catch (Exception)
                        {
                            throw new Exception("Checksum data not found.");
                        }
                    }
                    else if (lines[i].StartsWith("gHash"))
                    {
                        gFound = true;
                        gIndex = i;
                        // Validate that we got the expected strings and extract the checksum data.
                        try
                        {
                            geoCoreHash = lines[i].Split(new[] { ',' })[1];
                        }
                        catch (Exception)
                        {
                            throw new Exception("Checksum data not found.");
                        }
                    }
                    if (lFound && ehFound && gFound)
                    {
                        Array.Reverse(lines);
                        break;
                    }
                }

                // Compare the hashes with the loaded project.
                if (!hashCheck())
                {
                    ErrorReporter.showMessage_OK("CSV checksum does not match loaded project.\r\nTrying to continue.", "Data mismatch");
                }

                parsed.Clear();

                int terminatingRow = Math.Max(Math.Max(lIndex, gIndex), eIndex) + 1;

                // Avoid the hash lines.
                for (int line = 0; line < lines.Length - terminatingRow; line++)
                {
                    parsed.Add(lines[line].Split(new[] { ',' }));
                }

                chaosSettings = new ChaosSettings(true, commonVars_.getListOfSettings(), commonVars_.getSimulationSettings());
                getState(0); // set initial state.

                valid = true;
            }
            catch (Exception)
            {
                pReset();
            }

            max = parsed.Count - 1;
        }

        public void getState(int index)
        {
            pGetState(index);
        }

        void pGetState(int index)
        {
            // Compare the hashes with the loaded project.
            // In case the user changed a value and then changed it back, we allow the validity flag to be restored.
            if (!hashCheck())
            {
                ErrorReporter.showMessage_OK("CSV checksum does not match loaded project.", "Data mismatch");
                valid = false;
            }
            else
            {
                valid = true;
            }

            if (index == 0)
            {
                index++; // due to header. Callsites don't need to think about the header, we map around it.
            }
            if (!valid || (index >= parsed.Count))
            {
                return;
            }

            string searchString = "";
            int colIndex;
            // Find out which layers we have active.
            bool[] activeLayers = new bool[CentralProperties.maxLayersForMC];
#if !VARIANCESINGLETHREADED
            Parallel.For(0, CentralProperties.maxLayersForMC, (i) =>
#else
            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
#endif
            {
                activeLayers[i] = (commonVars_.getLayerSettings(i).getInt(EntropyLayerSettings.properties_i.enabled) == 1);
            }
#if !VARIANCESINGLETHREADED
            );
#endif

            for (int i = 0; i < CentralProperties.maxLayersForMC; i++)
            {
                if (!activeLayers[i])
                {
                    continue;
                }

                foreach (string t in CommonVars.csvHeader)
                {
                    searchString = t + (i);
                    colIndex = Array.IndexOf(parsed[0], searchString);
                    switch (t)
                    {
                        case "CDUSVar":
                            chaosSettings.setValue(ChaosSettings.properties.CDUSVar, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "CDUTVar":
                            chaosSettings.setValue(ChaosSettings.properties.CDUTVar, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "LWRVar":
                            chaosSettings.setValue(ChaosSettings.properties.LWRVar, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "LWRSeed":
                            chaosSettings.setInt(ChaosSettings.ints.lwrSeed, i, Convert.ToInt32(parsed[index][colIndex]));
                            break;
                        case "LWR2Var":
                            chaosSettings.setValue(ChaosSettings.properties.LWR2Var, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "LWR2Seed":
                            chaosSettings.setInt(ChaosSettings.ints.lwr2Seed, i, Convert.ToInt32(parsed[index][colIndex]));
                            break;
                        case "horTipBiasVar":
                            chaosSettings.setValue(ChaosSettings.properties.hTipBiasVar, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "verTipBiasVar":
                            chaosSettings.setValue(ChaosSettings.properties.vTipBiasVar, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "iCVar":
                            chaosSettings.setValue(ChaosSettings.properties.icVar, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "oCVar":
                            chaosSettings.setValue(ChaosSettings.properties.ocVar, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "overlayX":
                            chaosSettings.setValue(ChaosSettings.properties.overlayX, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "overlayY":
                            chaosSettings.setValue(ChaosSettings.properties.overlayY, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                        case "wobbleVar":
                            chaosSettings.setValue(ChaosSettings.properties.wobbleVar, i, Convert.ToDouble(parsed[index][colIndex]));
                            break;
                    }
                }
            }

            // Result parsing.
            switch (commonVars_.getSimulationSettings().getValue(EntropySettings.properties_i.oType))
            {
                case (int)CommonVars.calcModes.area:
                    if (commonVars_.getSimulationSettings().getValue(EntropySettings.properties_i.subMode) == (int)CommonVars.areaCalcModes.all)
                    {
                        searchString = "Total Area";
                    }
                    else
                    {
                        searchString = "Minimum Area";
                    }

                    colIndex = Array.IndexOf(parsed[0], searchString);
                    result = parsed[index][colIndex];
                    try
                    {
                        result = Convert.ToDouble(result).ToString("0.##");
                    }
                    catch (Exception)
                    {
                        // possibly no numeric data, so exception could be expected.
                    }
                    break;

                case (int)CommonVars.calcModes.enclosure_spacing_overlap:
                    switch (commonVars_.getSimulationSettings().getValue(EntropySettings.properties_i.subMode))
                    {
                        case (int)CommonVars.spacingCalcModes.spacing:
                        case (int)CommonVars.spacingCalcModes.spacingOld:
                            searchString = "Spacing";
                            break;
                        case (int)CommonVars.spacingCalcModes.enclosure:
                        case (int)CommonVars.spacingCalcModes.enclosureOld:
                            searchString = "Enclosure";
                            break;
                    }

                    colIndex = Array.IndexOf(parsed[0], searchString);
                    result = parsed[index][colIndex];
                    try
                    {
                        result = Convert.ToDouble(result).ToString("0.##");
                    }
                    catch (Exception)
                    {
                        // possibly no numeric data, so exception could be expected.
                    }
                    break;

                case (int)CommonVars.calcModes.chord: // chord output
                    string[] searchStrings = { "AMinTopChord", "AMinBottomChord", "BMinLeftChord", "BMinRightChord" };
                    string[] resultValues = new string[searchStrings.Length];
                    for (int i = 0; i < searchStrings.Length; i++)
                    {
                        colIndex = Array.IndexOf(parsed[0], searchStrings[i]);
                        result = parsed[index][colIndex];
                        try
                        {
                            result = Convert.ToDouble(result).ToString("0.##");
                        }
                        catch (Exception)
                        {
                            // possibly no numeric data, so exception could be expected.
                        }
                        resultValues[i] = result;
                    }
                    result = resultValues[0];
                    for (int i = 1; i < resultValues.Length; i++)
                    {
                        result += "," + resultValues[i];
                    }
                    break;

                case (int)CommonVars.calcModes.angle: // angle output
                    searchString = "MinIntersectionAngle";
                    colIndex = Array.IndexOf(parsed[0], searchString);
                    result = parsed[index][colIndex];
                    break;
            }
        }

        public void reset()
        {
            pReset();
        }

        void pReset()
        {
            valid = false;
            chaosSettings = null;
            col = -1;
            row = -1;
            parsed.Clear();
            max = 0;
        }

        bool hashCheck()
        {
            // Avoid changing the 'project differs from disk file' side of things.
            string[] oldHashes = commonVars_.getHashes();
            bool wasChanged = commonVars_.isChanged();

            // Ensure our hash is current.
            commonVars_.setHashes();
            string losh = Utils.GetMD5Hash(commonVars_.getListOfSettings());
            string ssth = Utils.GetMD5Hash(commonVars_.getSimulationSettings());
            string geoch = Utils.GetMD5Hash(commonVars_.getGeoCoreHandlers());

            // restore the values to avoid trouble.
            commonVars_.setHashes(oldHashes);
            commonVars_.setChanged(wasChanged);

            bool returnVal = (listOfSettingsHash == losh) && (entropyGeoHash == ssth);

            // Reloaded projects with baked geoCore data don't have the full set of input data so the hash will fail. Skip it.
            bool checkGeoCore = true;
            for (int layer = 0; layer < commonVars_.getListOfSettings().Count; layer++)
            {
                if ((commonVars_.getLayerSettings(layer).isReloaded()) && (commonVars_.getLayerSettings(layer).getInt(EntropyLayerSettings.properties_i.refLayout) == 0))
                {
                    checkGeoCore = false;
                }
            }

            if (checkGeoCore)
            {
                returnVal = returnVal && (geoCoreHash == geoch);
            }

            return returnVal;
        }
    }
}