using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using utility;

namespace Variance
{
    public class SimResultPackage
    {
        object previewLock;
        string lastMeanStdDev;
        object resultLock;
        object cleavedResultLock;
        object meanStdLock;
        bool state;

        public bool getState()
        {
            return pGetState();
        }

        bool pGetState()
        {
            return state;
        }

        public void setState(bool val)
        {
            pSetState(val);
        }

        void pSetState(bool val)
        {
            state = val;
        }

        Results previewResult;

        public Results getPreviewResult()
        {
            return pGetPreviewResult();
        }

        Results pGetPreviewResult()
        {
            return previewResult;
        }

        Results_implant previewResult_implant;

        public Results_implant getImplantPreviewResult()
        {
            return pGetImplantPreviewResult();
        }

        Results_implant pGetImplantPreviewResult()
        {
            return previewResult_implant;
        }

        List<Results> listOfResults;

        public List<Results> getListOfResults()
        {
            return pGetListOfResults();
        }

        List<Results> pGetListOfResults()
        {
            return listOfResults;
        }

        public Results getResult(int index)
        {
            return pGetResult(index);
        }

        Results pGetResult(int index)
        {
            return listOfResults[index];
        }

        List<Results_implant> listOfResults_implant;

        public List<Results_implant> getListOfResults_implant()
        {
            return pGetListOfResults_implant();
        }

        List<Results_implant> pGetListOfResults_implant()
        {
            return listOfResults_implant;
        }

        public Results_implant getImplantResult(int index)
        {
            return pGetImplantResult(index);
        }

        Results_implant pGetImplantResult(int index)
        {
            return listOfResults_implant[index];
        }

        List<string[]> pGetImplantResultStrings()
        {
            List<string[]> resultStrings = new List<string[]>();
            char[] s = { ',' };
            for (int i = 0; i < listOfResults_implant.Count; i++)
            {
                resultStrings.Add(listOfResults_implant[i].getResult().Split(s));
            }

            return resultStrings;
        }

        List<string[]> pGetResultStrings()
        {
            List<string[]> resultStrings = new List<string[]>();
            char[] s = { ',' };
            for (int i = 0; i < listOfResults.Count; i++)
            {
                resultStrings.Add(listOfResults[i].getResult().Split(s));
            }

            return resultStrings;
        }

        public List<string> getHistograms(int buckets = 100)
        {
            return pGetHistograms(buckets);
        }

        List<string> pGetHistograms(int buckets)
        {
            // Figure out which result package we should use.
            if (listOfResults.Count > 0)
            {
                return pGetHistograms(pGetResultStrings(), buckets);
            }
            else if (listOfResults_implant.Count > 0)
            {
                return pGetHistograms(pGetImplantResultStrings(), buckets);
            }
            else
            {
                return new List<string>() { "No results" };
            }
        }

        List<string> pGetHistograms(List<string[]> results, int buckets)
        {
            int numberOfResultsFields = results[0].Length;

            List<double>[] res = new List<double>[numberOfResultsFields];

#if VARIANCETHREADED
            Parallel.For(0, numberOfResultsFields, (i) =>
#else
            for (int i = 0; i < numberOfResultsFields; i++)
#endif
            {
                res[i] = new List<double>();
            }
#if VARIANCETHREADED
            );
#endif
            for (int r = 0; r < results.Count; r++)
            {
#if VARIANCETHREADED
                Parallel.For(0, numberOfResultsFields, (i) =>
#else
                for (int i = 0; i < numberOfResultsFields; i++)
#endif
                {
                    // Could have "N/A" and need to skip the result in that case.
                    if (results[r][i] != "N/A")
                    {
                        res[i].Add(Convert.ToDouble(results[r][i]));
                    }
                }
#if VARIANCETHREADED
                );
#endif
            }

            return pGetHistograms(res, buckets);
        }

        List<string> pGetHistograms(List<double>[] values, int buckets)
        {
            List<string> histograms = new List<string>();
            // Generate histograms for each set, unless there are no results in that set.
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Count == 0)
                {
                    histograms.Add("No data for result " + i.ToString() + "\r\n");
                }
                else
                {
                    histograms.Add("Histogram for result " + i.ToString() + ":\r\n");
                    Histo h = new Histo(10, values[i].ToArray());
                    histograms.Add(h.StemLeaf(true, buckets));
                }
            }

            return histograms;
        }

        List<List<double>> cleavedResults;
        double[] summedValues;
        double[] summedSquarevalues;
        Int32[] numberOfValues;

        public enum properties { mean, lastMean, stdDev, lastStdDev }

        double[] meanValues;
        double[] lastMeanValues;
        double[] stdDevValues;
        double[] lastStdDevValues;

        public double[] getValues(properties p)
        {
            return pGetValues(p);
        }

        double[] pGetValues(properties p)
        {
            double[] ret = new double[] { };

            switch (p)
            {
                case properties.mean:
                    ret = meanValues;
                    break;
                case properties.lastMean:
                    ret = lastMeanValues;
                    break;
                case properties.stdDev:
                    ret = stdDevValues;
                    break;
                case properties.lastStdDev:
                    ret = lastStdDevValues;
                    break;
            }

            return ret;
        }

        public double getValue(properties p, int index)
        {
            return pGetValue(p, index);
        }

        double pGetValue(properties p, int index)
        {
            double ret = 0;

            switch (p)
            {
                case properties.mean:
                    ret = meanValues[index];
                    break;
                case properties.lastMean:
                    ret = lastMeanValues[index];
                    break;
                case properties.stdDev:
                    ret = stdDevValues[index];
                    break;
                case properties.lastStdDev:
                    ret = lastStdDevValues[index];
                    break;
            }

            return ret;
        }

        public string resultString { get; set; }
        char[] csvSeparator = new char[] { ',' }; // to cleave the results apart.
        Results lastResult;
        Results_implant lastResult_implant;
        public double runTime { get; set; }
        public bool nonGaussianInput { get; set; }

        public void setRunTime(double swTime)
        {
            runTime = swTime;
        }

        public SimResultPackage(ref object previewLock, Int32 numberOfCases, Int32 numberOfResultsFields, bool state = true)
        {
            pSimResultPackage(ref previewLock, numberOfCases, numberOfResultsFields, state);
        }

        void pSimResultPackage(ref object previewLock, Int32 numberOfCases, Int32 numberOfResultsFields, bool state = true)
        {
            nonGaussianInput = false;
            this.previewLock = previewLock;
            resultLock = new object();
            cleavedResultLock = new object();
            meanStdLock = new object();
            resultString = "";
            lastMeanStdDev = "";
            runTime = 0.0;
            previewResult = new Results();
            previewResult_implant = new Results_implant();
            this.state = state;
            listOfResults = new List<Results>(numberOfCases);
            listOfResults_implant = new List<Results_implant>(numberOfCases);
            createCleavedResults(numberOfCases, numberOfResultsFields);
        }

        public void Add(Results_implant newResult)
        {
            pAdd(newResult);
        }

        void pAdd(Results_implant newResult)
        {
            if (Monitor.TryEnter(previewLock))
            {
                try
                {
                    previewResult_implant.set(newResult);
                }
                finally
                {
                    Monitor.Exit(previewLock);
                }
            }
            newResult.clear();

            Monitor.Enter(resultLock);
            try
            {
                listOfResults_implant.Add(newResult);
            }
            finally
            {
                Monitor.Exit(resultLock);
            }

            lastResult_implant = newResult;

            updateCleavedResults(new string[] { newResult.getResult() });
        }

        public void Add(Results newResult, Int32 retainGeometry)
        {
            pAdd(newResult, retainGeometry);
        }

        void pAdd(Results newResult, Int32 retainGeometry)
        {
            if (Monitor.TryEnter(previewLock))
            {
                try
                {
                    previewResult.setSimShapes(newResult.getSimShapes());
                    previewResult.setPreviewShapes(newResult.getPreviewShapes());
                    previewResult.setPoints(newResult.getPoints());
                    previewResult.setValid(newResult.isValid());
                }
                finally
                {
                    Monitor.Exit(previewLock);
                }
            }
            if (retainGeometry == 0)
            {
                newResult.clearPreviewShapes();
                newResult.clearSimShapes();
                newResult.clearPoints();
            }

            Monitor.Enter(resultLock);
            try
            {
                listOfResults.Add(newResult);
            }
            finally
            {
                Monitor.Exit(resultLock);
            }

            lastResult = newResult;
            if ((lastResult.getResult() == "N/A") && (listOfResults.Count() == 1))
            {
                // We just have one result so far and it's a fail - purge and move on.
                resultString = "N/A";
            }
            else
            {
                // Split the result string :
                string[] tempStr = newResult.getResult().Split(csvSeparator);
                updateCleavedResults(tempStr);
            }
        }

        void createCleavedResults(Int32 numberOfCases, Int32 numberOfResultsFields)
        {
            cleavedResults = new List<List<double>>(numberOfResultsFields);
            numberOfValues = new Int32[numberOfResultsFields];
            meanValues = new double[numberOfResultsFields];
            lastMeanValues = new double[numberOfResultsFields];
            stdDevValues = new double[numberOfResultsFields];
            lastStdDevValues = new double[numberOfResultsFields];
            summedValues = new double[numberOfResultsFields];
            summedSquarevalues = new double[numberOfResultsFields];
            for (Int32 i = 0; i < numberOfResultsFields; i++)
            {
                cleavedResults.Add(new List<double>(numberOfCases));
                numberOfValues[i] = 0;
                summedValues[i] = 0.0;
                summedSquarevalues[i] = 0.0;
                meanValues[i] = 0.0;
                lastMeanValues[i] = 0.0;
                stdDevValues[i] = 0.0;
                lastStdDevValues[i] = 0.0;
            }
        }

        public void updateCleavedResults(string[] newResults)
        {
            pUpdateCleavedResults(newResults);
        }

        void pUpdateCleavedResults(string[] newResults)
        {
            Monitor.Enter(cleavedResultLock);
            try
            {
                for (int i = 0; i < newResults.Length; i++)
                {
                    if (newResults[i] == "N/A")
                    {
                    }
                    else
                    {
                        double newValue = Convert.ToDouble(newResults[i]);
                        cleavedResults[i].Add(newValue);
                        summedValues[i] += newValue;
                        summedSquarevalues[i] += newValue * newValue;
                        numberOfValues[i]++;
                    }
                }
            }
            finally
            {
                Monitor.Exit(cleavedResultLock);
            }
        }

        void mean()
        {
            lastMeanValues = meanValues.ToArray();
            for (int i = 0; i < meanValues.Length; i++)
            {
                if (numberOfValues[i] == 0)
                {
                    meanValues[i] = summedValues[i];
                }
                else
                {
                    meanValues[i] = summedValues[i] / numberOfValues[i]; // cleavedResults[i].Average();
                }
            }
        }

        void stdDev()
        {
            if (nonGaussianInput)
            {
                return;
            }
            lastStdDevValues = stdDevValues.ToArray();
            // Welford's approach
            for (int i = 0; i < meanValues.Length; i++)
            {
                double m = 0.0;
                double s = 0.0;
                int k = 1;

                for (int value = 0; value < numberOfValues[i]; value++)
                {
                    double tmpM = m;

                    double _value = cleavedResults[i][value];
                    m += (_value - tmpM) / k;
                    s += (_value - m) * (_value - tmpM);
                    k++;
                }

                stdDevValues[i] = Math.Sqrt(s / (k - 2));
            }
        }

        void setMeanAndStdDev()
        {
            // This is called with cleavedResult under a lock.
            mean();
            stdDev();
        }

        public string getMeanAndStdDev()
        {
            return pGetMeanAndStdDev();
        }

        string pGetMeanAndStdDev()
        {
            string returnString = lastMeanStdDev;
            Monitor.Enter(meanStdLock);
            try
            {
                returnString = "";
                string[] tempString = new string[] { "" };

                if (lastResult != null)
                {
                    tempString = lastResult.getResult().Split(csvSeparator);
                }
                if (lastResult_implant != null)
                {
                    tempString = lastResult_implant.getResult().Split(csvSeparator);
                }
                for (Int32 k = 0; k < meanValues.Length; k++)
                {
                    setMeanAndStdDev();
                    if (k > 0)
                    {
                        returnString += ",";
                    }
                    // Need to handle N/A cases.
                    if (tempString[k] == "N/A")
                    {
                        returnString += "N/A";
                    }
                    else
                    {
                        try
                        {
                            returnString += "x: " + meanValues[k].ToString("0.##") + " (" + (meanValues[k] - lastMeanValues[k]).ToString("0.##") + ")";
                            if (!nonGaussianInput)
                            {
                                returnString += ", s: " + stdDevValues[k].ToString("0.##") + " (" + (stdDevValues[k] - lastStdDevValues[k]).ToString("0.##") + ")";
                            }
                        }
                        catch (Exception)
                        {
                            // Non-critical if an exception is raised, so we'll ignore it and carry on.
                        }
                    }
                }
                lastMeanStdDev = returnString;
            }
            finally
            {
                Monitor.Exit(meanStdLock);
            }
            return returnString;
        }
    }
}
