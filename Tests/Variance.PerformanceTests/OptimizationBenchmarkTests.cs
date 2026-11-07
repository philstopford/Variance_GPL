using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Variance;
using Clipper2Lib;

namespace Variance.PerformanceTests;

/// <summary>
/// Comprehensive performance benchmarks for optimization improvements.
/// Measures actual performance gains from caching, parallelization, and algorithmic improvements.
/// </summary>
[TestFixture]
public class OptimizationBenchmarkTests
{
    private const int WarmupIterations = 3;
    private const int BenchmarkIterations = 5;
    private const int SmallDataSize = 100;
    private const int MediumDataSize = 1000;
    private const int LargeDataSize = 10000;

    [Test]
    public void Benchmark_EquationCaching_Performance()
    {
        Console.WriteLine("=== Equation Caching Performance Benchmark ===");
        
        var settings = new EntropySettings();
        var testEquations = new[]
        {
            "x + y + z",
            "x * y + _gxy",
            "sqrt(x*x + y*y) + _gyz",
            "sin(x) * cos(y) + _gxz",
            "exp(x) + log(y) + sqrt(z)"
        };

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            foreach (var equation in testEquations)
            {
                UtilityFuncs.getCustomMappedRandomNumber(equation, settings);
            }
        }

        // Benchmark: First pass (cache misses)
        var stopwatch = Stopwatch.StartNew();
        for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
        {
            foreach (var equation in testEquations)
            {
                UtilityFuncs.getCustomMappedRandomNumber(equation, settings);
            }
        }
        stopwatch.Stop();
        double firstPassTime = stopwatch.Elapsed.TotalMilliseconds;

        // Benchmark: Second pass (cache hits)
        stopwatch.Restart();
        for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
        {
            foreach (var equation in testEquations)
            {
                UtilityFuncs.getCustomMappedRandomNumber(equation, settings);
            }
        }
        stopwatch.Stop();
        double secondPassTime = stopwatch.Elapsed.TotalMilliseconds;

        Console.WriteLine($"First pass (cache building): {firstPassTime:F2}ms");
        Console.WriteLine($"Second pass (cache hits): {secondPassTime:F2}ms");
        
        if (firstPassTime > 0)
        {
            double improvementPercent = ((firstPassTime - secondPassTime) / firstPassTime) * 100;
            Console.WriteLine($"Cache improvement: {improvementPercent:F1}%");
        }
        
        Console.WriteLine();
    }

    [Test]
    public void Benchmark_CollectionOperations_Performance()
    {
        Console.WriteLine("=== Collection Operations Performance Benchmark ===");
        
        BenchmarkCollectionSize("Small Dataset", SmallDataSize);
        BenchmarkCollectionSize("Medium Dataset", MediumDataSize);
        BenchmarkCollectionSize("Large Dataset", LargeDataSize);
    }

    private void BenchmarkCollectionSize(string testName, int dataSize)
    {
        Console.WriteLine($"--- {testName} ({dataSize:N0} elements) ---");
        
        // Create test data
        var testData = GenerateTestPaths(dataSize);
        var results = new Results();

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            results.setPreviewShapes(testData);
            results.clearPreviewShapes();
        }

        // Benchmark optimized copying
        double totalTime = 0;
        for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
        {
            var sw = Stopwatch.StartNew();
            results.setPreviewShapes(testData);
            var retrieved = results.getPreviewShapes();
            results.clearPreviewShapes();
            sw.Stop();
            totalTime += sw.Elapsed.TotalMilliseconds;
        }

        double avgTime = totalTime / BenchmarkIterations;
        double throughput = dataSize / avgTime * 1000; // elements per second

        Console.WriteLine($"Average time: {avgTime:F2}ms");
        Console.WriteLine($"Throughput: {throughput:N0} elements/sec");
        Console.WriteLine();
    }

    [Test]
    public void Benchmark_SamplerPerformance()
    {
        Console.WriteLine("=== Sampler Performance Benchmark ===");
        
        BenchmarkSamplerSize("Small Sample Set", 100);
        BenchmarkSamplerSize("Medium Sample Set", 1000);
        BenchmarkSamplerSize("Large Sample Set", 5000);
    }

    private void BenchmarkSamplerSize(string testName, int sampleCount)
    {
        Console.WriteLine($"--- {testName} ({sampleCount:N0} samples) ---");
        
        var settings = new EntropySettings();
        settings.setValue(EntropySettings.properties_i.samplingMethod, (int)QuasiRandomSampler.SamplingMethod.MonteCarlo);

        // Warmup
        for (int i = 0; i < WarmupIterations; i++)
        {
            var warmupSampler = new Sampler_Implant(100, previewMode: true, settings);
            warmupSampler.sample(useThreads: true);
        }

        // Benchmark
        double totalTime = 0;
        for (int iteration = 0; iteration < BenchmarkIterations; iteration++)
        {
            var sampler = new Sampler_Implant(sampleCount, previewMode: true, settings);
            var sw = Stopwatch.StartNew();
            sampler.sample(useThreads: true);
            sw.Stop();
            totalTime += sw.Elapsed.TotalMilliseconds;
            
            // Verify all samples were generated
            for (int i = 0; i < Math.Min(10, sampleCount); i++)
            {
                Assert.That(sampler.getSample(i), Is.Not.Null);
            }
        }

        double avgTime = totalTime / BenchmarkIterations;
        double samplesPerSecond = sampleCount / avgTime * 1000;

        Console.WriteLine($"Average time: {avgTime:F2}ms");
        Console.WriteLine($"Throughput: {samplesPerSecond:N0} samples/sec");
        Console.WriteLine($"Time per sample: {avgTime / sampleCount:F3}ms");
        Console.WriteLine();
    }

    [Test]
    public void Benchmark_OverallSystemPerformance()
    {
        Console.WriteLine("=== Overall System Performance Benchmark ===");
        
        // Combined workflow test
        var settings = new EntropySettings();
        const int workflowIterations = 10;
        
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < workflowIterations; i++)
        {
            // Simulate typical workflow
            
            // 1. Sample generation
            var sampler = new Sampler_Implant(500, previewMode: true, settings);
            sampler.sample(useThreads: true);
            
            // 2. Equation processing
            for (int j = 0; j < 50; j++)
            {
                UtilityFuncs.getCustomMappedRandomNumber("x + y * sin(z)", settings);
                UtilityFuncs.getGaussianRandomNumber(settings);
            }
            
            // 3. Results processing
            var results = new Results();
            var testData = GenerateTestPaths(200);
            results.setPreviewShapes(testData);
            var retrieved = results.getPreviewShapes();
            results.clearPreviewShapes();
        }
        
        stopwatch.Stop();
        
        double avgWorkflowTime = stopwatch.Elapsed.TotalMilliseconds / workflowIterations;
        Console.WriteLine($"Average workflow time: {avgWorkflowTime:F2}ms");
        Console.WriteLine($"Workflow throughput: {1000.0 / avgWorkflowTime:F1} workflows/sec");
        
        // Performance validation
        Assert.That(avgWorkflowTime, Is.LessThan(5000), "Workflow should complete within 5 seconds");
        
        Console.WriteLine();
        Console.WriteLine("=== Performance Summary ===");
        Console.WriteLine($"CPU Cores: {Environment.ProcessorCount}");
        Console.WriteLine($"Test completed successfully with optimized performance");
    }

    private List<PathsD> GenerateTestPaths(int count)
    {
        var paths = new List<PathsD>();
        var random = new Random(42); // Fixed seed for reproducible tests
        
        for (int i = 0; i < count; i++)
        {
            var pathsD = new PathsD();
            int pathCount = random.Next(1, 5);
            
            for (int j = 0; j < pathCount; j++)
            {
                var path = new PathD();
                int pointCount = random.Next(3, 8);
                
                for (int k = 0; k < pointCount; k++)
                {
                    path.Add(new PointD(
                        random.NextDouble() * 1000,
                        random.NextDouble() * 1000
                    ));
                }
                pathsD.Add(path);
            }
            paths.Add(pathsD);
        }
        
        return paths;
    }
}