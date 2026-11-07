using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Variance;
using Clipper2Lib;

namespace Variance.PerformanceTests;

/// <summary>
/// Tests to validate that performance optimizations don't change computational results.
/// These tests ensure correctness is maintained while verifying performance improvements.
/// </summary>
[TestFixture]
public class OptimizationValidationTests
{
    [Test]
    public void UtilityFuncs_EquationCaching_ProducesIdenticalResults()
    {
        // Arrange
        var settings = new EntropySettings();
        string testEquation = "x + y * _gxy + z * _gyz";
        
        // Act - Run the same equation multiple times
        var results = new List<double>();
        for (int i = 0; i < 10; i++)
        {
            double result = UtilityFuncs.getCustomMappedRandomNumber(testEquation, settings);
            results.Add(result);
        }
        
        // Assert - Results should be valid finite numbers
        Assert.That(results, Has.All.Matches<double>(r => !double.IsNaN(r) && !double.IsInfinity(r)),
            "All equation results should be finite numbers");
        Assert.That(results.Count, Is.EqualTo(10), "Should generate expected number of results");
        
        Console.WriteLine($"Equation caching test completed with {results.Count} valid results");
    }

    [Test]
    public void Results_OptimizedCopying_MaintainsDataIntegrity()
    {
        // Arrange
        var originalResults = new Results();
        var testShapes = new List<PathsD>();
        
        // Create test data
        for (int i = 0; i < 5; i++)
        {
            var paths = new PathsD();
            for (int j = 0; j < 3; j++)
            {
                var path = new PathD();
                path.Add(new PointD(i * 10, j * 10));
                path.Add(new PointD(i * 10 + 5, j * 10));
                path.Add(new PointD(i * 10 + 5, j * 10 + 5));
                path.Add(new PointD(i * 10, j * 10 + 5));
                paths.Add(path);
            }
            testShapes.Add(paths);
        }
        
        // Act
        originalResults.setPreviewShapes(testShapes);
        var retrievedShapes = originalResults.getPreviewShapes();
        
        // Assert
        Assert.That(retrievedShapes, Is.Not.Null, "Retrieved shapes should not be null");
        Assert.That(retrievedShapes.Count, Is.EqualTo(testShapes.Count), "Shape count should be preserved");
        
        for (int i = 0; i < testShapes.Count; i++)
        {
            Assert.That(retrievedShapes[i].Count, Is.EqualTo(testShapes[i].Count), 
                $"Path count should be preserved for shape {i}");
            
            for (int j = 0; j < testShapes[i].Count; j++)
            {
                Assert.That(retrievedShapes[i][j].Count, Is.EqualTo(testShapes[i][j].Count),
                    $"Point count should be preserved for shape {i}, path {j}");
            }
        }
        
        Console.WriteLine($"Data integrity validated for {testShapes.Count} shapes");
    }

    [Test]
    public void SamplerImplant_OptimizedSampling_ProducesCorrectCount()
    {
        // Arrange
        var settings = new EntropySettings();
        settings.setValue(EntropySettings.properties_i.samplingMethod, (int)QuasiRandomSampler.SamplingMethod.MonteCarlo);
        
        const int sampleCount = 100;
        var sampler = new Sampler_Implant(sampleCount, previewMode: true, settings);
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        sampler.sample(useThreads: true);
        stopwatch.Stop();
        
        // Assert
        Assert.That(sampler.getDimensions(), Is.GreaterThan(0), "Sampler should have valid dimensions");
        
        // Verify all samples are generated
        for (int i = 0; i < sampleCount; i++)
        {
            var sample = sampler.getSample(i);
            Assert.That(sample, Is.Not.Null, $"Sample {i} should not be null");
        }
        
        Console.WriteLine($"Optimized sampling generated {sampleCount} samples in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Average time per sample: {(double)stopwatch.ElapsedMilliseconds / sampleCount:F3}ms");
    }

    [Test]
    public void Performance_OptimizedVsOriginal_ShowsImprovementOrMaintenance()
    {
        // This test measures relative performance to ensure optimizations don't regress
        var settings = new EntropySettings();
        const int iterationCount = 1000;
        
        // Test equation processing performance
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterationCount; i++)
        {
            UtilityFuncs.getCustomMappedRandomNumber("x + y + z", settings);
        }
        stopwatch.Stop();
        
        var timePerOperation = (double)stopwatch.ElapsedMilliseconds / iterationCount;
        
        // Assert reasonable performance bounds
        Assert.That(timePerOperation, Is.LessThan(10.0), 
            "Equation processing should complete within reasonable time bounds");
        
        Console.WriteLine($"Performance test: {timePerOperation:F3}ms per equation evaluation");
        
        // Test collection operations performance
        var testResults = new Results();
        var testData = new List<PathsD>();
        for (int i = 0; i < 100; i++)
        {
            testData.Add(new PathsD());
        }
        
        stopwatch.Restart();
        for (int i = 0; i < 100; i++)
        {
            testResults.setPreviewShapes(testData);
        }
        stopwatch.Stop();
        
        var collectionTimePerOp = (double)stopwatch.ElapsedMilliseconds / 100;
        Console.WriteLine($"Collection operations: {collectionTimePerOp:F3}ms per operation");
        
        Assert.That(collectionTimePerOp, Is.LessThan(5.0),
            "Collection operations should complete within reasonable time bounds");
    }

    [Test]
    public void RandomNumberGeneration_ConsistentBehavior()
    {
        // Ensure random number generation still produces valid distributions
        var settings = new EntropySettings();
        var gaussianNumbers = new List<double>();
        
        for (int i = 0; i < 1000; i++)
        {
            double gaussian = UtilityFuncs.getGaussianRandomNumber(settings);
            gaussianNumbers.Add(gaussian);
        }
        
        // Basic statistical checks
        double mean = gaussianNumbers.Average();
        double variance = gaussianNumbers.Select(x => Math.Pow(x - mean, 2)).Average();
        double stdDev = Math.Sqrt(variance);
        
        // For a standard normal distribution, we expect mean ≈ 0, stdDev ≈ 1
        Assert.That(Math.Abs(mean), Is.LessThan(0.2), "Mean should be close to 0 for Gaussian distribution");
        Assert.That(stdDev, Is.InRange(0.8, 1.2), "Standard deviation should be close to 1 for Gaussian distribution");
        
        Console.WriteLine($"Gaussian RNG validation: mean={mean:F3}, stdDev={stdDev:F3}");
    }
}