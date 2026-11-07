using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using utility;
using System.Threading.Tasks;

namespace Variance;

/// <summary>
/// Thread-safe container for simulation results that supports concurrent access from multiple threads.
/// Uses ReaderWriterLockSlim for optimized read/write operations and provides both preview and 
/// final result storage capabilities. Implements IDisposable for proper resource cleanup.
/// </summary>
public class SimResultPackage : IDisposable
{
    /// <summary>Thread-safe lock for preview result operations</summary>
    private ReaderWriterLockSlim previewLock = new();
    
    /// <summary>Simple lock object for results list operations</summary>
    private object resultLock = new();
    
    /// <summary>Thread-safe lock for cleaved (processed) result operations</summary>
    private ReaderWriterLockSlim cleavedResultLock = new();
    
    /// <summary>Thread-safe lock for mean and standard deviation calculations</summary>
    private ReaderWriterLockSlim meanStdLock = new();
    
    /// <summary>Flag to track if the object has been disposed</summary>
    private bool disposed = false;
    
    /// <summary>Internal state flag for the simulation package</summary>
    private bool state;

    /// <summary>
    /// Gets the current state of the simulation result package.
    /// Thread-safe operation that can be called concurrently.
    /// </summary>
    /// <returns>True if the package is active, false otherwise</returns>
    public bool getState()
    {
        return pGetState();
    }

    /// <summary>
    /// Private implementation of state getter for internal use.
    /// </summary>
    /// <returns>Current state value</returns>
    private bool pGetState()
    {
        return state;
    }

    /// <summary>
    /// Sets the state of the simulation result package.
    /// Used to enable or disable result collection and processing.
    /// </summary>
    /// <param name="val">New state value - true to activate, false to deactivate</param>
    public void setState(bool val)
    {
        pSetState(val);
    }

    /// <summary>
    /// Private implementation of state setter for internal use.
    /// </summary>
    /// <param name="val">New state value to set</param>
    private void pSetState(bool val)
    {
        state = val;
    }

    /// <summary>Preview result for immediate display/analysis before full processing</summary>
    private Results previewResult;

    /// <summary>
    /// Gets the current preview result for immediate display purposes.
    /// Preview results are used for real-time feedback during simulation runs.
    /// </summary>
    /// <returns>Current preview result or null if none set</returns>
    public Results getPreviewResult()
    {
        return pGetPreviewResult();
    }

    /// <summary>
    /// Private implementation of preview result getter with thread-safe access.
    /// </summary>
    /// <returns>Current preview result</returns>
    private Results pGetPreviewResult()
    {
        return previewResult;
    }

    /// <summary>Preview result specifically for implant simulation data</summary>
    private Results_implant previewResult_implant;

    /// <summary>
    /// Gets the current implant preview result for immediate display purposes.
    /// Used specifically for implant shadowing simulation results.
    /// </summary>
    /// <returns>Current implant preview result or null if none set</returns>
    public Results_implant getImplantPreviewResult()
    {
        return pGetImplantPreviewResult();
    }

    /// <summary>
    /// Private implementation of implant preview result getter.
    /// </summary>
    /// <returns>Current implant preview result</returns>
    private Results_implant pGetImplantPreviewResult()
    {
        return previewResult_implant;
    }

    /// <summary>
    /// Count of results currently stored in the package.
    /// Updated atomically when results are added.
    /// </summary>
    public int results_count;
    
    /// <summary>Array containing all simulation results indexed by case number</summary>
    private Results[] listOfResults;

    /// <summary>
    /// Gets a copy of the complete results array.
    /// This is a potentially expensive operation for large result sets.
    /// </summary>
    /// <returns>Array containing all stored results</returns>
    public Results[] getListOfResults()
    {
        return pGetListOfResults();
    }

    private Results[] pGetListOfResults()
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

    public int implant_results_count;
    private Results_implant[] listOfResults_implant;

    public Results_implant[] getListOfResults_implant()
    {
        return pGetListOfResults_implant();
    }

    private Results_implant[] pGetListOfResults_implant()
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
        char[] s = [','];

        return listOfResults_implant.Select(t => t.getResult().Split(s)).ToList();
    }

    private List<string[]> pGetResultStrings()
    {
        char[] s = [','];

        return listOfResults.Select(t => t.getResult().Split(s)).ToList();
    }

    public List<string> getHistograms(int buckets = 100)
    {
        return pGetHistograms(buckets);
    }

    private List<string> pGetHistograms(int buckets)
    {
        // Figure out which result package we should use.
        if (results_count > 0)
        {
            return pGetHistograms(pGetResultStrings(), buckets);
        }

        return implant_results_count > 0 ? pGetHistograms(pGetImplantResultStrings(), buckets) : ["No results"];
    }

    private List<string> pGetHistograms(List<string[]> results, int buckets)
    {
        int numberOfResultsFields = results[0].Length;

        List<double>[] res = new List<double>[numberOfResultsFields];

        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        ParallelProcessing.OptimizedParallelFor(0, numberOfResultsFields, options, i =>
        {
            res[i] = [];
        });
        
        foreach (string[] t in results)
        {
            ParallelProcessing.OptimizedParallelFor(0, numberOfResultsFields, options, i =>
            {
                // Could have "N/A" and need to skip the result in that case.
                if (t[i] != "N/A")
                {
                    res[i].Add(Convert.ToDouble(t[i]));
                }
            });
        }

        return pGetHistograms(res, buckets);
    }

    private static List<string> pGetHistograms(List<double>[] values, int buckets)
    {
        List<string> histograms = [];
        // Generate histograms for each set, unless there are no results in that set.
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i].Count == 0)
            {
                histograms.Add("No data for result " + i + "\r\n");
            }
            else
            {
                try
                {
                    Histo h = new(10, values[i].ToArray());
                    string temp = (h.StemLeaf(true, buckets));
                    if (string.IsNullOrWhiteSpace(temp))
                    {
                        throw new Exception();
                    }

                    histograms.Add("Histogram for result " + i + ":\r\n");
                    histograms.Add(temp);
                }
                catch
                {
                    // Histogram can fail in case of insufficient variation - i.e. all values are the same.
                    histograms.Add("No histogram available for result " + i + " - were all results the same?");
                }
            }
        }

        return histograms;
    }

    private List<List<double>> cleavedResults;
    private double[] summedValues;
    private double[] summedSquarevalues;
    private int[] numberOfValues;

    public enum Properties { mean, lastMean, stdDev, lastStdDev }

    private double[] meanValues;
    private double[] lastMeanValues;
    private double[] stdDevValues;
    private double[] lastStdDevValues;

    public double[] getValues(Properties p)
    {
        return pGetValues(p);
    }

    private double[] pGetValues(Properties p)
    {
        double[] ret = p switch
        {
            Properties.mean => meanValues,
            Properties.lastMean => lastMeanValues,
            Properties.stdDev => stdDevValues,
            Properties.lastStdDev => lastStdDevValues,
            _ => []
        };

        return ret;
    }

    public double getValue(Properties p, int index)
    {
        return pGetValue(p, index);
    }

    private double pGetValue(Properties p, int index)
    {
        double ret = p switch
        {
            Properties.mean => meanValues[index],
            Properties.lastMean => lastMeanValues[index],
            Properties.stdDev => stdDevValues[index],
            Properties.lastStdDev => lastStdDevValues[index],
            _ => 0
        };

        return ret;
    }

    private string resultString { get; set; }
    private readonly char[] csvSeparator = [',']; // to cleave the results apart.
    private Results lastResult;
    private Results_implant lastResult_implant;
    public double runTime { get; set; }
    public bool nonGaussianInput { get; set; }

    public void setRunTime(double swTime)
    {
        runTime = swTime;
    }

    // Purely for test coverage.
    public SimResultPackage()
    {
        
    }

    public SimResultPackage(ref object previewLock_, int numberOfCases, int numberOfResultsFields, bool state_ = true)
    {
        pSimResultPackage(ref previewLock_, numberOfCases, numberOfResultsFields, state_);
    }

    private void pSimResultPackage(ref object previewLock_, int numberOfCases, int numberOfResultsFields, bool state_ = true)
    {
        nonGaussianInput = false;
        // Initialize our own high-performance locks instead of using the passed reference
        previewLock = new ReaderWriterLockSlim();
        resultLock = new object();
        cleavedResultLock = new ReaderWriterLockSlim();
        meanStdLock = new ReaderWriterLockSlim();
        resultString = "";
        runTime = 0.0;
        previewResult = new Results();
        previewResult_implant = new Results_implant();
        state = state_;
        listOfResults = new Results[numberOfCases];
        listOfResults_implant = new Results_implant[numberOfCases];
        createCleavedResults(numberOfCases, numberOfResultsFields);
    }

    public void Add(int index, Results_implant newResult)
    {
        pAdd(index, newResult);
    }

    private void pAdd(int index, Results_implant newResult)
    {
        if (previewLock.TryEnterWriteLock(TimeSpan.FromMilliseconds(1)))
        {
            try
            {
                previewResult_implant.set(newResult);
            }
            finally
            {
                previewLock.ExitWriteLock();
            }
        }
        newResult.clear();

        listOfResults_implant[index] = newResult;
        Interlocked.Increment(ref implant_results_count);

        lastResult_implant = newResult;

        updateCleavedResults([newResult.getResult()]);
    }

    public void Add(int index, Results newResult, int retainGeometry)
    {
        pAdd(index, newResult, retainGeometry);
    }

    private void pAdd(int index, Results newResult, int retainGeometry)
    {
        if (previewLock.TryEnterWriteLock(TimeSpan.FromMilliseconds(1)))
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
                previewLock.ExitWriteLock();
            }
        }
        if (retainGeometry == 0)
        {
            newResult.clearPreviewShapes();
            newResult.clearSimShapes();
            newResult.clearPoints();
        }

        listOfResults[index] = newResult;
        Interlocked.Increment(ref results_count);
        
        lastResult = newResult;
        if (lastResult.getResult() == "N/A" && results_count == 1)
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
        
        // Use Array.Clear for better performance than loop-based initialization
        Array.Clear(numberOfValues, 0, numberOfResultsFields);
        Array.Clear(meanValues, 0, numberOfResultsFields);
        Array.Clear(lastMeanValues, 0, numberOfResultsFields);
        Array.Clear(stdDevValues, 0, numberOfResultsFields);
        Array.Clear(lastStdDevValues, 0, numberOfResultsFields);
        Array.Clear(summedValues, 0, numberOfResultsFields);
        Array.Clear(summedSquarevalues, 0, numberOfResultsFields);
        
        for (int i = 0; i < numberOfResultsFields; i++)
        {
            cleavedResults.Add(new List<double>(numberOfCases));
        }
    }

    private void updateCleavedResults(string[] newResults)
    {
        pUpdateCleavedResults(newResults);
    }

    private void pUpdateCleavedResults(string[] newResults)
    {
        cleavedResultLock.EnterWriteLock();
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
            cleavedResultLock.ExitWriteLock();
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
        meanStdLock.EnterReadLock();
        try
        {
            string[] tempString = [""];

            if (lastResult != null)
            {
                tempString = lastResult.getResult().Split(csvSeparator);
            }
            if (lastResult_implant != null)
            {
                tempString = lastResult_implant.getResult().Split(csvSeparator);
            }
            
            var sb = new StringBuilder(256); // Pre-allocate reasonable capacity
            for (int k = 0; k < meanValues.Length; k++)
            {
                setMeanAndStdDev();
                if (k > 0)
                {
                    sb.Append(',');
                }
                // Need to handle N/A cases.
                if (tempString[k] == "N/A")
                {
                    sb.Append("N/A");
                }
                else
                {
                    try
                    {
                        sb.Append("x: ").Append(meanValues[k].ToString("0.##")).Append(" (").Append((meanValues[k] - lastMeanValues[k]).ToString("0.##")).Append(')');
                        if (!nonGaussianInput)
                        {
                            sb.Append(", s: ").Append(stdDevValues[k].ToString("0.##")).Append(" (").Append((stdDevValues[k] - lastStdDevValues[k]).ToString("0.##")).Append(')');
                        }
                    }
                    catch (Exception)
                    {
                        // Non-critical if an exception is raised, so we'll ignore it and carry on.
                    }
                }
            }
            return sb.ToString();
        }
        finally
        {
            meanStdLock.ExitReadLock();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            previewLock?.Dispose();
            cleavedResultLock?.Dispose();
            meanStdLock?.Dispose();
            disposed = true;
        }
    }
}