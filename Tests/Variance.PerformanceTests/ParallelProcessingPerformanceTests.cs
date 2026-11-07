using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Variance.utility;

namespace Variance.PerformanceTests;

[TestFixture]
public class ParallelProcessingPerformanceTests
{
    private const int SmallArraySize = 500;
    private const int MediumArraySize = 10000;
    private const int LargeArraySize = 100000;
    private const int WarmupIterations = 3;
    private const int BenchmarkIterations = 5;

    [Test]
    public void Benchmark_OptimizedParallelFor_vs_StandardParallel()
    {
        Console.WriteLine("=== ParallelProcessing Performance Benchmark ===");
        Console.WriteLine($"CPU Cores: {Environment.ProcessorCount}");
        Console.WriteLine();
        
        // Test different array sizes
        BenchmarkArraySize("Small Array", SmallArraySize);
        BenchmarkArraySize("Medium Array", MediumArraySize);
        BenchmarkArraySize("Large Array", LargeArraySize);
    }

    private void BenchmarkArraySize(string testName, int arraySize)
    {
        Console.WriteLine($"--- {testName} ({arraySize:N0} elements) ---");
        
        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            var _ = new double[arraySize];
            ParallelProcessing.OptimizedParallelFor(0, arraySize, j => Math.Sin(j * 0.1));
            Parallel.For(0, arraySize, j => Math.Sin(j * 0.1));
        }

        // Benchmark OptimizedParallelFor
        double optimizedTotalTime = 0;
        for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
        {
            var optimizedArray = new double[arraySize];
            var sw = Stopwatch.StartNew();
            
            ParallelProcessing.OptimizedParallelFor(optimizedArray, i => 
                Math.Sin(i * 0.1) * Math.Cos(i * 0.2) + Math.Sqrt(i + 1));
            
            sw.Stop();
            optimizedTotalTime += sw.Elapsed.TotalMilliseconds;
        }
        double optimizedAvgTime = optimizedTotalTime / BenchmarkIterations;

        // Benchmark Standard Parallel.For
        double standardTotalTime = 0;
        for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
        {
            var standardArray = new double[arraySize];
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            var sw = Stopwatch.StartNew();
            
            Parallel.For(0, arraySize, options, i =>
            {
                standardArray[i] = Math.Sin(i * 0.1) * Math.Cos(i * 0.2) + Math.Sqrt(i + 1);
            });
            
            sw.Stop();
            standardTotalTime += sw.Elapsed.TotalMilliseconds;
        }
        double standardAvgTime = standardTotalTime / BenchmarkIterations;

        // Calculate improvement
        double improvementPercent = ((standardAvgTime - optimizedAvgTime) / standardAvgTime) * 100;
        double throughputOptimized = arraySize / optimizedAvgTime * 1000; // operations per second
        double throughputStandard = arraySize / standardAvgTime * 1000;

        Console.WriteLine($"OptimizedParallelFor: {optimizedAvgTime:F2} ms ({throughputOptimized:N0} ops/sec)");
        Console.WriteLine($"Standard Parallel.For: {standardAvgTime:F2} ms ({throughputStandard:N0} ops/sec)");
        Console.WriteLine($"Performance improvement: {improvementPercent:F1}%");
        Console.WriteLine();
        
        // Ensure OptimizedParallelFor is not significantly slower
        Assert.LessOrEqual(optimizedAvgTime, standardAvgTime * 1.1, 
            "OptimizedParallelFor should not be significantly slower than standard Parallel.For");
    }

    [Test]
    public void Benchmark_VectorizedParallelFor_Performance()
    {
        Console.WriteLine("=== VectorizedParallelFor Performance Test ===");
        
        const int arraySize = 50000;
        const int batchSize = 16;
        
        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            var _ = new float[arraySize];
            ParallelProcessing.VectorizedParallelFor(_, j => (float)Math.Sin(j * 0.1), batchSize);
        }

        double totalTime = 0;
        for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
        {
            var array = new float[arraySize];
            var sw = Stopwatch.StartNew();
            
            ParallelProcessing.VectorizedParallelFor(array, i => 
                (float)(Math.Sin(i * 0.1) * Math.Cos(i * 0.2)), batchSize);
            
            sw.Stop();
            totalTime += sw.Elapsed.TotalMilliseconds;
        }
        
        double avgTime = totalTime / BenchmarkIterations;
        double throughput = arraySize / avgTime * 1000;
        
        Console.WriteLine($"VectorizedParallelFor: {avgTime:F2} ms ({throughput:N0} ops/sec)");
        Console.WriteLine($"Batch size: {batchSize}");
        
        // Verify correctness
        var testArray = new float[100];
        ParallelProcessing.VectorizedParallelFor(testArray, i => i * 2.0f, batchSize);
        Assert.AreEqual(0.0f, testArray[0]);
        Assert.AreEqual(2.0f, testArray[1]);
        Assert.AreEqual(198.0f, testArray[99]);
    }

    [Test]
    public void Benchmark_ThresholdOptimization_SmallArrays()
    {
        Console.WriteLine("=== Small Array Threshold Optimization Test ===");
        
        var smallSizes = new[] { 10, 50, 100, 500, 1000, 2000 };
        
        foreach (var size in smallSizes)
        {
            // Test both approaches
            double sequentialTime = 0;
            double parallelTime = 0;
            
            // Sequential approach
            for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
            {
                var array = new double[size];
                var sw = Stopwatch.StartNew();
                
                for (int i = 0; i < size; i++)
                {
                    array[i] = Math.Sin(i * 0.1) + Math.Cos(i * 0.2);
                }
                
                sw.Stop();
                sequentialTime += sw.Elapsed.TotalMilliseconds;
            }
            
            // OptimizedParallelFor (which should choose sequential for small arrays)
            for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
            {
                var array = new double[size];
                var sw = Stopwatch.StartNew();
                
                ParallelProcessing.OptimizedParallelFor(array, i => 
                    Math.Sin(i * 0.1) + Math.Cos(i * 0.2));
                
                sw.Stop();
                parallelTime += sw.Elapsed.TotalMilliseconds;
            }
            
            sequentialTime /= BenchmarkIterations;
            parallelTime /= BenchmarkIterations;
            
            Console.WriteLine($"Size {size,4}: Sequential {sequentialTime:F3}ms, " +
                            $"OptimizedParallel {parallelTime:F3}ms, " +
                            $"Ratio: {parallelTime/sequentialTime:F2}x");
        }
    }

    [Test]
    public void Benchmark_ChaosEngine_LayerOperations()
    {
        Console.WriteLine("=== ChaosEngine-style Layer Operations Performance ===");
        
        // Simulate layer boolean operations similar to ChaosEngine
        const int layerCount = 8;
        const int operationsPerLayer = 5000;
        
        // Test array operations similar to boolean layer processing
        var layers = new bool[layerCount][];
        for (int i = 0; i < layerCount; i++)
        {
            layers[i] = new bool[operationsPerLayer];
        }

        // Warmup
        for (int w = 0; w < WarmupIterations; w++)
        {
            ParallelProcessing.OptimizedParallelFor(0, layerCount, i =>
            {
                for (int j = 0; j < operationsPerLayer; j++)
                {
                    layers[i][j] = (i + j) % 2 == 0;
                }
            });
        }

        double totalTime = 0;
        for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
        {
            var sw = Stopwatch.StartNew();
            
            // Process layers in parallel (similar to ChaosEngine boolean operations)
            ParallelProcessing.OptimizedParallelFor(0, layerCount, i =>
            {
                // Simulate complex layer boolean operations
                for (int j = 0; j < operationsPerLayer; j++)
                {
                    // Simulate geometric calculations
                    double value = Math.Sin(i + j) * Math.Cos(i * j + 1);
                    layers[i][j] = value > 0.0;
                }
            });
            
            sw.Stop();
            totalTime += sw.Elapsed.TotalMilliseconds;
        }
        
        double avgTime = totalTime / BenchmarkIterations;
        double totalOperations = layerCount * operationsPerLayer;
        double throughput = totalOperations / avgTime * 1000;
        
        Console.WriteLine($"Layer operations: {avgTime:F2} ms");
        Console.WriteLine($"Total operations: {totalOperations:N0}");
        Console.WriteLine($"Throughput: {throughput:N0} ops/sec");
        Console.WriteLine($"Layers processed: {layerCount}");
        Console.WriteLine($"Operations per layer: {operationsPerLayer:N0}");
        
        // Verify some results
        Assert.IsTrue(layers.Length == layerCount);
        Assert.IsTrue(layers[0].Length == operationsPerLayer);
    }
}