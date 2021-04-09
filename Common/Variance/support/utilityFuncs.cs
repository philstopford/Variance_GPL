using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using entropyRNG;
using Error;
using info.lundin.math;
using utility; // string to equation mapping.

namespace Variance
{
    public static class UtilityFuncs
    {
        public static double getCustomMappedRandomNumber(string equation, EntropySettings settings)
        {
            return pGetCustomMappedRandomNumber(equation, settings);
        }

        static double pGetCustomMappedRandomNumber(string equation, EntropySettings settings)
        {
            double value = 1E-15;

            double xvalue = pGetRandomDouble(settings);
            while (xvalue < value)
            {
                xvalue = pGetRandomDouble(settings);
            }

            double yvalue = pGetRandomDouble(settings);
            while (yvalue < value)
            {
                yvalue = pGetRandomDouble(settings);
            }

            double zvalue = pGetRandomDouble(settings);
            while (zvalue < value)
            {
                zvalue = pGetRandomDouble(settings);
            }

            try
            {
                ExpressionParser parser = new ExpressionParser();
                parser.Values.Add("x", xvalue);
                parser.Values.Add("y", yvalue);
                parser.Values.Add("z", zvalue);
                // Substitute any Box-Muller queries with the full form.
                equation = equation.Replace("_gxy", "(sqrt(-2 * ln(y)) * cos(2 * PI * x))");
                equation = equation.Replace("_gyz", "(sqrt(-2 * ln(z)) * cos(2 * PI * y))");
                equation = equation.Replace("_gxz", "(sqrt(-2 * ln(z)) * cos(2 * PI * x))");
                value = parser.Parse(equation);
            }
            catch (Exception e)
            {
                ErrorReporter.showMessage_OK(e.ToString(), "RNG mapper error");
            }

            return value;
        }

        public static double getGaussianRandomNumber3(EntropySettings settings)
        {
            return pGetGaussianRandomNumber3(settings);
        }

        static double pGetGaussianRandomNumber3(EntropySettings settings)
        {
            double value;
            switch (settings.getValue(EntropySettings.properties_i.rngType))
            {
                case (Int32)commonRNG.rngIndex.crypto:
                    value = Crypto_RNG.random_gauss3()[0];
                    break;
                case (Int32)commonRNG.rngIndex.mtwister:
                    value = MersenneTwister_RNG.random_gauss3()[0];
                    break;
                default:
                    value = RNG.random_gauss3()[0];
                    break;
            }
            return value;
        }

        public static double getGaussianRandomNumber(EntropySettings settings)
        {
            return pGetGaussianRandomNumber(settings);
        }

        static double pGetGaussianRandomNumber(EntropySettings settings)
        {
            double value;
            switch (settings.getValue(EntropySettings.properties_i.rngType))
            {
                case (Int32)commonRNG.rngIndex.crypto:
                    value = Crypto_RNG.random_gauss()[0];
                    break;
                case (Int32)commonRNG.rngIndex.mtwister:
                    value = MersenneTwister_RNG.random_gauss()[0];
                    break;
                default:
                    value = RNG.random_gauss()[0];
                    break;
            }
            return value;
        }

        public static Int32 getRandomInt(EntropySettings settings)
        {
            return pGetRandomInt(settings);
        }

        static Int32 pGetRandomInt(EntropySettings settings)
        {
            Int32 value;
            switch (settings.getValue(EntropySettings.properties_i.rngType))
            {
                case (Int32)commonRNG.rngIndex.crypto:
                    value = Crypto_RNG.nextint();
                    break;
                case (Int32)commonRNG.rngIndex.mtwister:
                    value = MersenneTwister_RNG.nextint();
                    break;
                default:
                    value = RNG.nextint();
                    break;
            }
            return value;
        }

        public static double getRandomDouble(EntropySettings settings)
        {
            return pGetRandomDouble(settings);
        }

        static double pGetRandomDouble(EntropySettings settings)
        {
            double value;
            switch (settings.getValue(EntropySettings.properties_i.rngType))
            {
                case (Int32)commonRNG.rngIndex.crypto:
                    value = Crypto_RNG.nextdouble();
                    break;
                case (Int32)commonRNG.rngIndex.mtwister:
                    value = MersenneTwister_RNG.nextdouble();
                    break;
                default:
                    value = RNG.nextdouble();
                    break;
            }
            return value;
        }

        public static void resetDebugLog(string filename)
        {
            if (filename == "")
            {
                return;
            }
            File.WriteAllText(filename, "");
        }

        public static void debugLog(string filename, string debugString)
        {
            if (filename == "")
            {
                return;
            }

            using (StreamWriter sw = File.AppendText(filename))
            {
                sw.WriteLine(debugString);
            }
        }

        public static double do1DCalculation_3sig_edge(
        double layer1_overlayx, double layer1_overlayy,
        double layer1_cdu, double layer1_lwr,
        double layer1_addvar,
        double layer2_overlayx, double layer2_overlayy,
        double layer2_cdu, double layer2_lwr,
        double layer2_addvar)
        {
            return Math.Sqrt(
                Utils.myPow(layer1_overlayx, 2) +
                Utils.myPow(layer1_overlayy, 2) +
                Utils.myPow(layer2_overlayx, 2) +
                Utils.myPow(layer2_overlayy, 2) +
                Utils.myPow((layer1_cdu / 2), 2) +
                Utils.myPow((layer2_cdu / 2), 2) +
                Utils.myPow((layer1_lwr / 2), 2) +
                Utils.myPow((layer2_lwr / 2), 2) +
                Utils.myPow((layer1_addvar / 2), 2) +
                Utils.myPow((layer2_addvar / 2), 2));
        }

        public static double do1DCalculation_3sig(
        double layer1_overlayx, double layer1_overlayy,
        double layer1_cdu, double layer1_lwr,
        double layer1_bias, double layer1_addvar,
        double layer2_overlayx, double layer2_overlayy,
        double layer2_cdu, double layer2_lwr,
        double layer2_bias, double layer2_addvar, double minIns)
        {
            double edgeVal = do1DCalculation_3sig_edge(layer1_overlayx, layer1_overlayy, layer1_cdu, layer1_lwr, layer1_addvar,
                layer2_overlayx, layer2_overlayy, layer2_cdu, layer2_lwr, layer2_addvar);
            return minIns + edgeVal + (0.5 * (layer1_bias + layer2_bias));
        }

        public static double do1DCalculation_4sig(
        double layer1_overlayx, double layer1_overlayy,
        double layer1_cdu, double layer1_lwr,
        double layer1_bias, double layer1_addvar,
        double layer2_overlayx, double layer2_overlayy,
        double layer2_cdu, double layer2_lwr,
        double layer2_bias, double layer2_addvar, double minIns)
        {
            double edgeVal = (4.0f / 3.0f) * do1DCalculation_3sig_edge(layer1_overlayx, layer1_overlayy, layer1_cdu, layer1_lwr, layer1_addvar,
                layer2_overlayx, layer2_overlayy, layer2_cdu, layer2_lwr, layer2_addvar);
            return minIns + edgeVal + (0.5 * (layer1_bias + layer2_bias));
        }

        public static double do1DCalculation_nsig(double n,
        double layer1_overlayx, double layer1_overlayy,
        double layer1_cdu, double layer1_lwr,
        double layer1_bias, double layer1_addvar,
        double layer2_overlayx, double layer2_overlayy,
        double layer2_cdu, double layer2_lwr,
        double layer2_bias, double layer2_addvar, double minIns)
        {
            double edgeVal = (n / 3.0f) * do1DCalculation_3sig_edge(layer1_overlayx, layer1_overlayy, layer1_cdu, layer1_lwr, layer1_addvar,
                layer2_overlayx, layer2_overlayy, layer2_cdu, layer2_lwr, layer2_addvar);
            return minIns + edgeVal + (0.5 * (layer1_bias + layer2_bias));
        }

        public static bool readiDRMCSVFile(ref CommonVars commonVars, string fileName)
        {
            return pReadiDRMCSVFile(ref commonVars, fileName);
        }

        static bool pReadiDRMCSVFile(ref CommonVars commonVars, string fileName)
        {
            bool reading = false;
            List<Int32[]> tilesToRun = new List<Int32[]>(); // will be col,row
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    char[] csvSeparators = { ',' };
                    char[] equalSeparators = { '=' };
                    string[] parsedString;

                    reading = true;
                    // Read in non-CSV data first
                    // Pattern line
                    sr.ReadLine();
                    // Now we have the meat of the data we need.
                    string tempString = sr.ReadLine();
                    parsedString = tempString.Split(csvSeparators);
                    if (parsedString.Count() >= 6)
                    {
                        // We have 6 values to extract from this line.
                        // X-offset, Y-offset, X pitch, Y pitch, rows, cols
                        // Convert from um to nm in the process.
                        // Take the second result of the split which is the numeric value. Convert it to double, multiply by 1000 to convert units. Make int.
                        commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.colOffset, (1000 * Convert.ToDouble(parsedString[0].Split(equalSeparators)[1])));
                        commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.rowOffset, (1000 * Convert.ToDouble(parsedString[1].Split(equalSeparators)[1])));
                        commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.colPitch, (1000 * Convert.ToDouble(parsedString[2].Split(equalSeparators)[1])));
                        commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.rowPitch, (1000 * Convert.ToDouble(parsedString[3].Split(equalSeparators)[1])));
                        commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.rows, Convert.ToInt32(Convert.ToDouble(parsedString[4].Split(equalSeparators)[1])));
                        commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.cols, Convert.ToInt32(Convert.ToDouble(parsedString[5].Split(equalSeparators)[1])));
                    }
                    else
                    {
                        // We need to break here.
                        ErrorReporter.showMessage_OK("CSV file doesn't match expectations.", "Aborting.");
                        return false;
                    }

                    sr.ReadLine(); // skip a line that we don't need.

                    while (!sr.EndOfStream)
                    {
                        // Assuming Pass/Fail,Column,Row,Co-ord X,Co-ord Y,,,,,,,,,,,,,
                        tempString = sr.ReadLine();
                        parsedString = tempString.Split(csvSeparators);
                        if ((parsedString[0] == "") || (parsedString[0] == "Occupy matrix"))
                        {
                            break;
                        }

                        if (Convert.ToInt32(parsedString[0]) == 1)
                        {
                            // Pass case - need to add to list.
                            tilesToRun.Add(new [] { Convert.ToInt32(parsedString[1]), Convert.ToInt32(parsedString[2]) });
                        }
                    }
                }

                commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.iDRM, true);
                commonVars.getSimulationSettings().getDOESettings().setTileList_ColRow(tilesToRun);
                commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 1);
                commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTile, 0);

            }
            catch (Exception)
            {
                return reading;
            }

            commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.iDRM, true);
            commonVars.getSimulationSettings().getDOESettings().setTileList_ColRow(tilesToRun);
            commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 1);
            commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTile, 0);
            return reading;
        }

        public static bool readQuiltCSVFile(ref CommonVars commonVars, string fileName)
        {
            return pReadQuiltCSVFile(ref commonVars, fileName);
        }

        static bool pReadQuiltCSVFile(ref CommonVars commonVars, string fileName)
        {
            bool reading = false;
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    reading = true;
                    char[] csvSeparators = { ',' };

                    // Read in non-CSV data first
                    // Pattern line
                    string check = sr.ReadLine();
                    if (!check.StartsWith("Quilt"))
                    {
                        return false;
                    }
                    // Now we have the meat of the data we need.
                    string tempString = sr.ReadLine();
                    string[] parsedString = tempString.Split(csvSeparators);
                    if (parsedString.Count() >= 6)
                    {
                        // We have 6 values to extract from this line.
                        // X-offset, Y-offset, X pitch, Y pitch, rows, cols
                        // Convert from um to nm in the process.
                        // Take the second result of the split which is the numeric value. Convert it to double, multiply by 1000 to convert units. Make int.
                        commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.colOffset, Convert.ToDouble(parsedString[0]));
                        commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.rowOffset, Convert.ToDouble(parsedString[1]));
                        commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.colPitch, Convert.ToDouble(parsedString[2]));
                        commonVars.getSimulationSettings().getDOESettings().setDouble(DOESettings.properties_d.rowPitch, Convert.ToDouble(parsedString[3]));
                        commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.rows, Convert.ToInt32(parsedString[4]));
                        commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.cols, Convert.ToInt32(parsedString[5]));
                    }
                    else
                    {
                        // We need to break here.
                        ErrorReporter.showMessage_OK("CSV file doesn't match expectations.", "Aborting.");
                        return false;
                    }
                }
                reading = false;
            }
            catch (Exception)
            {
                return reading;
            }

            commonVars.getSimulationSettings().getDOESettings().setBool(DOESettings.properties_b.quilt, true);
            commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.uTileList, 0);
            commonVars.getSimulationSettings().getDOESettings().setInt(DOESettings.properties_i.sTile, 0);
            return reading;
        }

        static void extractValuesFromSummaryFile(DOEResults doeResults, string[] DOETokens, string filename)
        {
            string DOEName = DOETokens[0];
            // Get our index in doeResults for convenience
            Int32 index = 0;
            bool foundDOE = false;
            while (index < doeResults.getResults().Count())
            {
                if (doeResults.getResults()[index].getName() == DOEName)
                {
                    foundDOE = true;
                    break;
                }
                index++;
            }

            if (!foundDOE)
            {
                // int oops = 1;
            }

            StreamReader file = new StreamReader(filename);
            // Our col and row indices have prefixes; need to remove and obtain the index.
            string[] tempArray = DOETokens[1].Split('l');
            Int32 column = Convert.ToInt32(tempArray[1]);
            tempArray = DOETokens[2].Split('w');
            Int32 row = Convert.ToInt32(tempArray[1]);

            // Store our cell in the DOE list.
            doeResults.getResults()[index].AddCellToDOE(column, row);

            bool headerRead = false;
            bool eqtnFound = false;
            string line;
            List<string> header = new List<string>();
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("mean and standard deviation"))
                {
                    headerRead = true;
                    doeResults.getResults()[index].setDOEHeaderInformation(header);
                    // We have a result line.
                    // We need to carve it up. Happily, we know our format and can rely on it!
                    string leaderString = "result ";

                    leaderString = leaderString + line.Substring(leaderString.Length, 1) + ": ";

                    int meanStart = line.LastIndexOf("x", StringComparison.CurrentCulture);

                    // We need to replace our ',' in the result string to avoid issues with the CSV format.
                    string resultLine = line.Substring(meanStart, line.Length - meanStart);
                    string[] resultLineTokens = resultLine.Split(',');
                    resultLine = resultLineTokens[0] + "(" + resultLineTokens[1].Substring(1, resultLineTokens[1].Length - 1) + ")";

                    // Populate our cell-level list of results for the DOEName, with the list of result strings that we find in the file.
                    doeResults.getResults()[index].AddResultToCell(column, row, leaderString + resultLine);
                }
                else
                {
                    if (!eqtnFound)
                    {
                        string searchString = "Equation";
                        if ((line.Length > searchString.Length) && (line.Substring(0, searchString.Length) == searchString))
                        {
                            eqtnFound = true;
                        }
                    }
                    if ((!headerRead) && (eqtnFound))
                    {
                        header.Add(line);
                    }
                }
            }
        }

        public static string generateDOESummaryFile(DOEResults doeResults, string path)
        {
            return pGenerateDOESummaryFile(doeResults, path);
        }

        static string pGenerateDOESummaryFile(DOEResults doeResults, string path)
        {
            bool error = false;
            string summaryFile = "";
            try
            {
                for (int result = 0; result < doeResults.getResults().Count; result++)
                {
                    // We walk our doeResults, collected by name, and summarize each one in a grid layout in CSV.
                    // Retrieve the grid size from our results.
                    int colCount = 0;
                    int rowCount = 0;
                    for (int cell = 0; cell < doeResults.getResults()[result].getCells().Count(); cell++)
                    {
                        int col = doeResults.getResults()[result].getCells()[cell].getColIndex();
                        int row = doeResults.getResults()[result].getCells()[cell].getRowIndex();
                        if (col > colCount)
                        {
                            colCount = col;
                        }
                        if (row > rowCount)
                        {
                            rowCount = row;
                        }
                    }

                    // Deal with array size vs index offset.
                    colCount++;
                    rowCount++;

                    // Create our 2D string array to match our DOE results.
                    string[,] doeResultsSummaryGrid = new string[colCount, rowCount];

                    // Populate cells where results are available; empty cells will be indicated.
                    for (int cell = 0; cell < doeResults.getResults()[result].getCells().Count(); cell++)
                    {
                        string resultString = "";
                        int resultCount = doeResults.getResults()[result].getCells()[cell].getResults().getValues().Count();
                        for (int resultIndex = 0; resultIndex < resultCount; resultIndex++)
                        {
                            resultString += doeResults.getResults()[result].getCells()[cell].getResults().getValues()[resultIndex];

                            // To work with our CSV format, we use ';' to separate multiple results in the same field.
                            if ((resultCount > 1) && (resultIndex != resultCount - 1))
                            {
                                resultString += ";";
                            }
                        }
                        // No result for cell, so indicate to user.
                        if (resultString == "")
                        {
                            resultString = "No result(s)";
                        }

                        // Assign result string to corresponding cell of grid.
                        doeResultsSummaryGrid[doeResults.getResults()[result].getCells()[cell].getColIndex(), doeResults.getResults()[result].getCells()[cell].getRowIndex()] = resultString;
                    }

                    // Now we have our grid, we need to save it out.
                    summaryFile = Path.Combine(path, doeResults.getResults()[result].getName() + "_DOESummary.txt");

                    List<string> resultStringList = doeResults.getResults()[result].getHeaderInformation().ToList();

                    // Compile our string list that we will write.
                    for (int row = 0; row < rowCount; row++)
                    {
                        string resultsRow = "";
                        for (int col = 0; col < colCount; col++)
                        {
                            resultsRow += doeResultsSummaryGrid[col, row];
                            if ((colCount > 1) && (col != colCount - 1))
                            {
                                resultsRow += ",";
                            }
                        }
                        resultStringList.Add(resultsRow);
                    }

                    File.WriteAllLines(summaryFile, resultStringList);
                }
            }
            catch (Exception)
            {
                error = true;
            }
            if (!error)
            {
                return "Summary written to: " + summaryFile;
            }

            return "";
        }

        public static string summarizeDOEResults(string path, string[] summaryFilesFound)
        {
            return pSummarizeDOEResults(path, summaryFilesFound);
        }

        static string pSummarizeDOEResults(string path, string[] summaryFilesFound)
        {
            // We need to account for the possibility of more than one DOE run being in the folder.
            // We're only targeting the summary files (not CSV).
            // File name is of the form userFileName_colX_rowY_summary.txt

            if (summaryFilesFound.Length == 0)
            {
                return "";
            }

            Int32 index = 0;

            string fileToRead = Path.Combine(path, summaryFilesFound[index]);

            char[] tokens = { '_' };
            string[] DOETokens = summaryFilesFound[index].Split(tokens);
            // Create our DOE results instance. This will contain all of DOEs and collect all of their results as well.
            DOEResults doeResults = new DOEResults(DOETokens[0]);
            extractValuesFromSummaryFile(doeResults, DOETokens, fileToRead);

            index++;
            while (index < summaryFilesFound.Length)
            {
                fileToRead = Path.Combine(path, summaryFilesFound[index]);
                DOETokens = summaryFilesFound[index].Split(tokens);

                bool found = false;
                for (int result = 0; result < doeResults.getResults().Count(); result++)
                {
                    if (doeResults.getResults()[result].getName() == DOETokens[0])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) // New DOE name, need to add it to our list.
                {
                    doeResults.AddResult(DOETokens[0]);
                }
                extractValuesFromSummaryFile(doeResults, DOETokens, fileToRead);
                index++;
            }
            return generateDOESummaryFile(doeResults, path);
        }
    }
}
