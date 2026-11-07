using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Variance.utility;

namespace Variance.PerformanceTests;

[TestFixture]
public class SimResultPackageTests
{
    [Test]
    public void ResultPackage_ParallelAdditions_ProducesCorrectResults()
    {
        // Create a result package
        using var resultPackage = new SimResultPackage();
        
        // Initialize with some test data
        const int numberOfCases = 1000;
        
        resultPackage.setState(true);
        
        // Create mock results for testing
        var results = new Results[numberOfCases];
        for (int i = 0; i < numberOfCases; i++)
        {
            var result = new Results();
            // Create a simple CSV result string for testing
            string resultStr = $"{i * 1.5},{i * 2.0},{i * 0.5},{i + 10},{i * 3}";
            result.setResult(resultStr);
            results[i] = result;
        }
        
        // Measure time for parallel additions
        var stopwatch = Stopwatch.StartNew();
        
        ParallelProcessing.OptimizedParallelFor(0, numberOfCases, i =>
        {
            resultPackage.Add(i, results[i], 0);
        });
        
        stopwatch.Stop();
        
        // Verify we got the expected number of results
        Assert.AreEqual(numberOfCases, resultPackage.results_count);
        
        Console.WriteLine($"OptimizedParallelFor addition of {numberOfCases} results took: {stopwatch.ElapsedMilliseconds}ms");
        
        // Verify data integrity - check a few results
        var retrievedResult = resultPackage.getResult(0);
        Assert.IsNotNull(retrievedResult);
        Assert.AreEqual("0,0,0,10,0", retrievedResult.getResult());
        
        var midResult = resultPackage.getResult(numberOfCases / 2);
        Assert.IsNotNull(midResult);
        
        var lastResult = resultPackage.getResult(numberOfCases - 1);
        Assert.IsNotNull(lastResult);
    }
    
    [Test]
    public void BenchmarkTest_MeasureCurrentPerformance()
    {
        // This test measures current performance to establish baseline
        const int iterations = 10000;
        
        var stopwatch = Stopwatch.StartNew();
        
        // Simulate typical workload
        var tasks = new Task[Environment.ProcessorCount];
        for (int t = 0; t < tasks.Length; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                for (int i = 0; i < iterations / tasks.Length; i++)
                {
                    // Simulate some computational work
                    double result = Math.Sin(i) * Math.Cos(i) + Math.Sqrt(i + 1);
                    result = Math.Log(result + 1);
                }
            });
        }
        
        Task.WaitAll(tasks);
        stopwatch.Stop();
        
        Console.WriteLine($"Baseline performance for {iterations} iterations: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Available processors: {Environment.ProcessorCount}");
        
        // This should complete in reasonable time
        Assert.Less(stopwatch.ElapsedMilliseconds, 10000); // Less than 10 seconds
    }
    
    [Test]
    public void ParallelFor_OptimizationTest_VerifyPerformance()
    {
        // Test to verify our ParallelOptions optimizations are working
        const int arraySize = 100000;
        var data = new double[arraySize];
        var results = new double[arraySize];
        
        // Initialize data
        for (int i = 0; i < arraySize; i++)
        {
            data[i] = i * 0.5;
        }
        
        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        
        var stopwatch = Stopwatch.StartNew();
        
        ParallelProcessing.OptimizedParallelFor(0, arraySize, options, i =>
        {
            results[i] = Math.Sin(data[i]) * Math.Cos(data[i]) + Math.Sqrt(data[i] + 1);
        });
        
        stopwatch.Stop();
        
        Console.WriteLine($"OptimizedParallelFor processing of {arraySize} items took: {stopwatch.ElapsedMilliseconds}ms");
        
        // Verify some results are computed correctly
        Assert.Greater(results[100], 0);
        Assert.Greater(results[arraySize - 1], 0);
        Assert.Less(stopwatch.ElapsedMilliseconds, 5000); // Should be fast
    }

    [Test]
    public void ReaderWriterLockSlim_Performance_Test()
    {
        // Test the performance of our new ReaderWriterLockSlim vs old Monitor approach
        using var resultPackage = new SimResultPackage();
        const int concurrentReads = 1000;
        const int concurrentWrites = 100;
        
        var readTasks = new Task[concurrentReads];
        var writeTasks = new Task[concurrentWrites];
        
        // Initialize with some data
        var result = new Results();
        result.setResult("1.0,2.0,3.0,4.0,5.0");
        
        var stopwatch = Stopwatch.StartNew();
        
        // Start concurrent reads and writes
        for (int i = 0; i < concurrentReads; i++)
        {
            readTasks[i] = Task.Run(() =>
            {
                // Simulate read operations
                _ = resultPackage.getState();
                _ = resultPackage.getState();
            });
        }
        
        for (int i = 0; i < concurrentWrites; i++)
        {
            int index = i;
            writeTasks[i] = Task.Run(() =>
            {
                // Simulate write operations
                resultPackage.Add(index, result, 0);
            });
        }
        
        Task.WaitAll(readTasks);
        Task.WaitAll(writeTasks);
        
        stopwatch.Stop();
        
        Console.WriteLine($"Concurrent operations ({concurrentReads} reads, {concurrentWrites} writes) took: {stopwatch.ElapsedMilliseconds}ms");
        
        // Verify results
        Assert.AreEqual(concurrentWrites, resultPackage.results_count);
        Assert.Less(stopwatch.ElapsedMilliseconds, 5000); // Should be efficient
    }
}