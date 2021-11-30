using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using utility;
using System.Threading.Tasks;

namespace Variance;

public class SimResultPackage
{
    private object previewLock;
    private object resultLock;
    private object cleavedResultLock;
    private object meanStdLock;
    private bool state;

    public bool getState()
    {
        return pGetState();
    }

    private bool pGetState()
    {
        return state;
    }

    public void setState(bool val)
    {
        pSetState(val);
    }

    private void pSetState(bool val)
    {
        state = val;
    }

    private Results previewResult;

    public Results getPreviewResult()
    {
        return pGetPreviewResult();
    }

    private Results pGetPreviewResult()
    {
        return previewResult;
    }

    private Results_implant previewResult_implant;

    public Results_implant getImplantPreviewResult()
    {
        return pGetImplantPreviewResult();
    }

    private Results_implant pGetImplantPreviewResult()
    {
        return previewResult_implant;
    }

    private List<Results> listOfResults;

    public List<Results> getListOfResults()
    {
        return pGetListOfResults();
    }

    private List<Results> pGetListOfResults()
    {
        return listOfResults;
    }

    public Results getResult(int index)
    {
        return pGetResult(index);
    }

    private Results pGetResult(int index)
    {
        return listOfResults[index];
    }

    private List<Results_implant> listOfResults_implant;

    public List<Results_implant> getListOfResults_implant()
    {
        return pGetListOfResults_implant();
    }

    private List<Results_implant> pGetListOfResults_implant()
    {
        return listOfResults_implant;
    }

    public Results_implant getImplantResult(int index)
    {
        return pGetImplantResult(index);
    }

    private Results_implant pGetImplantResult(int index)
    {
        return listOfResults_implant[index];
    }

    private List<string[]> pGetImplantResultStrings()
    {
        char[] s = { ',' };

        return listOfResults_implant.Select(t => t.getResult().Split(s)).ToList();
    }

    private List<string[]> pGetResultStrings()
    {
        char[] s = { ',' };

        return listOfResults.Select(t => t.getResult().Split(s)).ToList();
    }

    public List<string> getHistograms(int buckets = 100)
    {
        return pGetHistograms(buckets);
    }

    private List<string> pGetHistograms(int buckets)
    {
        // Figure out which result package we should use.
        if (listOfResults.Count > 0)
        {
            return pGetHistograms(pGetResultStrings(), buckets);
        }

        return listOfResults_implant.Count > 0 ? pGetHistograms(pGetImplantResultStrings(), buckets) : new List<string> { "No results" };
    }

    private List<string> pGetHistograms(List<string[]> results, int buckets)
    {
        int numberOfResultsFields = results[0].Length;

        List<double>[] res = new List<double>[numberOfResultsFields];

#if !VARIANCESINGLETHREADED
        Parallel.For(0, numberOfResultsFields, i =>
#else
            for (int i = 0; i < numberOfResultsFields; i++)
#endif
            {
                res[i] = new List<double>();
            }
#if !VARIANCESINGLETHREADED
        );
#endif
        foreach (string[] t in results)
        {
#if !VARIANCESINGLETHREADED
            Parallel.For(0, numberOfResultsFields, i =>
#else
                for (int i = 0; i < numberOfResultsFields; i++)
#endif
                {
                    // Could have "N/A" and need to skip the result in that case.
                    if (t[i] != "N/A")
                    {
                        res[i].Add(Convert.ToDouble(t[i]));
                    }
                }
#if !VARIANCESINGLETHREADED
            );
#endif
        }

        return pGetHistograms(res, buckets);
    }

    private static List<string> pGetHistograms(List<double>[] values, int buckets)
    {
        List<string> histograms = new();
        try
        {
            // Generate histograms for each set, unless there are no results in that set.
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Count == 0)
                {
                    histograms.Add("No data for result " + i + "\r\n");
                }
                else
                {
                    histograms.Add("Histogram for result " + i + ":\r\n");
                    Histo h = new(10, values[i].ToArray());
                    histograms.Add(h.StemLeaf(true, buckets));
                }
            }
        }
        catch (Exception)
        {
            // Histogram can fail in case of insufficient variation - i.e. all values are the same.
        }

        return histograms;
    }

    private List<List<double>> cleavedResults;
    private double[] summedValues;
    private double[] summedSquarevalues;
    private int[] numberOfValues;

    public enum properties { mean, lastMean, stdDev, lastStdDev }

    private double[] meanValues;
    private double[] lastMeanValues;
    private double[] stdDevValues;
    private double[] lastStdDevValues;

    public double[] getValues(properties p)
    {
        return pGetValues(p);
    }

    private double[] pGetValues(properties p)
    {
        double[] ret = { };

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

    private double pGetValue(properties p, int index)
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

    private string resultString { get; set; }
    private readonly char[] csvSeparator = { ',' }; // to cleave the results apart.
    private Results lastResult;
    private Results_implant lastResult_implant;
    public double runTime { get; set; }
    public bool nonGaussianInput { get; set; }

    public void setRunTime(double swTime)
    {
        runTime = swTime;
    }

    public SimResultPackage(ref object previewLock_, int numberOfCases, int numberOfResultsFields, bool state_ = true)
    {
        pSimResultPackage(ref previewLock_, numberOfCases, numberOfResultsFields, state_);
    }

    private void pSimResultPackage(ref object previewLock_, int numberOfCases, int numberOfResultsFields, bool state_ = true)
    {
        nonGaussianInput = false;
        previewLock = previewLock_;
        resultLock = new object();
        cleavedResultLock = new object();
        meanStdLock = new object();
        resultString = "";
        runTime = 0.0;
        previewResult = new Results();
        previewResult_implant = new Results_implant();
        state = state_;
        listOfResults = new List<Results>(numberOfCases);
        listOfResults_implant = new List<Results_implant>(numberOfCases);
        createCleavedResults(numberOfCases, numberOfResultsFields);
    }

    public void Add(Results_implant newResult)
    {
        pAdd(newResult);
    }

    private void pAdd(Results_implant newResult)
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

        updateCleavedResults(new [] { newResult.getResult() });
    }

    public void Add(Results newResult, int retainGeometry)
    {
        pAdd(newResult, retainGeometry);
    }

    private void pAdd(Results newResult, int retainGeometry)
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
        if (lastResult.getResult() == "N/A" && listOfResults.Count == 1)
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

    private void createCleavedResults(int numberOfCases, int numberOfResultsFields)
    {
        cleavedResults = new List<List<double>>(numberOfResultsFields);
        numberOfValues = new int[numberOfResultsFields];
        meanValues = new double[numberOfResultsFields];
        lastMeanValues = new double[numberOfResultsFields];
        stdDevValues = new double[numberOfResultsFields];
        lastStdDevValues = new double[numberOfResultsFields];
        summedValues = new double[numberOfResultsFields];
        summedSquarevalues = new double[numberOfResultsFields];
        for (int i = 0; i < numberOfResultsFields; i++)
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

    private void updateCleavedResults(string[] newResults)
    {
        pUpdateCleavedResults(newResults);
    }

    private void pUpdateCleavedResults(string[] newResults)
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

    private void mean()
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

    private void stdDev()
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

    private void setMeanAndStdDev()
    {
        // This is called with cleavedResult under a lock.
        mean();
        stdDev();
    }

    public string getMeanAndStdDev()
    {
        return pGetMeanAndStdDev();
    }

    private string pGetMeanAndStdDev()
    {
        string returnString = "";
        Monitor.Enter(meanStdLock);
        try
        {
            string[] tempString = { "" };

            if (lastResult != null)
            {
                tempString = lastResult.getResult().Split(csvSeparator);
            }
            if (lastResult_implant != null)
            {
                tempString = lastResult_implant.getResult().Split(csvSeparator);
            }
            for (int k = 0; k < meanValues.Length; k++)
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
        }
        finally
        {
            Monitor.Exit(meanStdLock);
        }
        return returnString;
    }
}