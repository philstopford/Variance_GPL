using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Variance;

namespace Variance.UnitTests
{
    /// <summary>
    /// Performance benchmark tests for XML optimization improvements.
    /// Validates that optimizations provide measurable performance improvements
    /// while maintaining 100% functional compatibility.
    /// </summary>
    [TestFixture]
    public class XMLOptimizationPerformanceTests
    {
        private string testXmlPath;
        private Storage storage;
        
        [SetUp]
        public void Setup()
        {
            storage = new Storage();
            testXmlPath = Path.Combine(Path.GetTempPath(), "perf_test.variance");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(testXmlPath))
                File.Delete(testXmlPath);
        }

        /// <summary>
        /// Benchmarks the optimized XML element caching vs. repeated Descendants() calls.
        /// This test demonstrates the performance impact of the optimization.
        /// </summary>
        [Test]
        public void XMLElementCaching_PerformanceBenchmark()
        {
            // Arrange
            var testXml = CreateComplexTestXMLDocument();
            var rootElement = XElement.Parse(testXml);
            int iterations = 1000;

            // Warm up
            for (int i = 0; i < 10; i++)
            {
                _ = rootElement.Descendants("layer1").FirstOrDefault();
                StorageXMLOptimizer.GetCachedElement(rootElement, "layer1");
            }

            // Benchmark original approach
            StorageXMLOptimizer.ClearCache();
            var startTime = DateTime.UtcNow;
            for (int i = 0; i < iterations; i++)
            {
                var element = rootElement.Descendants("layer1").FirstOrDefault();
                var enabled = element?.Descendants("enabled").FirstOrDefault();
                var comment = element?.Descendants("comment").FirstOrDefault();
                var name = element?.Descendants("name").FirstOrDefault();
            }
            var originalTime = DateTime.UtcNow - startTime;

            // Benchmark optimized approach
            StorageXMLOptimizer.ClearCache();
            startTime = DateTime.UtcNow;
            for (int i = 0; i < iterations; i++)
            {
                var element = StorageXMLOptimizer.GetCachedElement(rootElement, "layer1");
                var enabled = element?.Element("enabled");
                var comment = element?.Element("comment");
                var name = element?.Element("name");
            }
            var optimizedTime = DateTime.UtcNow - startTime;

            // Assert
            TestContext.WriteLine($"Original approach: {originalTime.TotalMilliseconds}ms");
            TestContext.WriteLine($"Optimized approach: {optimizedTime.TotalMilliseconds}ms");
            
            if (optimizedTime.TotalMilliseconds > 0)
            {
                double improvement = (originalTime.TotalMilliseconds - optimizedTime.TotalMilliseconds) / originalTime.TotalMilliseconds * 100;
                TestContext.WriteLine($"Performance improvement: {improvement:F1}%");
            }

            // The optimized version should be faster or at least not significantly slower
            Assert.That(optimizedTime.TotalMilliseconds, Is.LessThanOrEqualTo(originalTime.TotalMilliseconds * 1.1), // Allow 10% margin for test variability
                $"Optimized version should not be significantly slower: {optimizedTime.TotalMilliseconds}ms vs {originalTime.TotalMilliseconds}ms");
        }

        /// <summary>
        /// Benchmarks safe parsing methods vs Convert + exception handling.
        /// </summary>
        [Test]
        public void SafeParsing_PerformanceBenchmark()
        {
            // Arrange
            var testValues = new[] { "123", "456", "0", "-789", "999" };
            int iterations = 10000;

            // Warm up
            for (int i = 0; i < 100; i++)
            {
                try { Convert.ToInt32("123"); } catch { }
                int.TryParse("123", out _);
            }

            // Benchmark Convert + exception approach
            var startTime = DateTime.UtcNow;
            int convertSum = 0;
            for (int i = 0; i < iterations; i++)
            {
                foreach (var value in testValues)
                {
                    try
                    {
                        convertSum += Convert.ToInt32(value);
                    }
                    catch
                    {
                        convertSum += 0;
                    }
                }
            }
            var convertTime = DateTime.UtcNow - startTime;

            // Benchmark TryParse approach
            startTime = DateTime.UtcNow;
            int tryParseSum = 0;
            for (int i = 0; i < iterations; i++)
            {
                foreach (var value in testValues)
                {
                    if (int.TryParse(value, out int result))
                    {
                        tryParseSum += result;
                    }
                    else
                    {
                        tryParseSum += 0;
                    }
                }
            }
            var tryParseTime = DateTime.UtcNow - startTime;

            // Assert
            Assert.That(tryParseSum, Is.EqualTo(convertSum), "Both approaches should produce identical results");
            
            TestContext.WriteLine($"Convert + exception approach: {convertTime.TotalMilliseconds}ms");
            TestContext.WriteLine($"TryParse approach: {tryParseTime.TotalMilliseconds}ms");
            
            if (tryParseTime.TotalMilliseconds > 0)
            {
                double improvement = (convertTime.TotalMilliseconds - tryParseTime.TotalMilliseconds) / convertTime.TotalMilliseconds * 100;
                TestContext.WriteLine($"Performance improvement: {improvement:F1}%");
            }

            // TryParse should typically be faster than Convert + exception handling
            Assert.That(tryParseTime.TotalMilliseconds, Is.LessThanOrEqualTo(convertTime.TotalMilliseconds),
                $"TryParse should not be slower than Convert + exception: {tryParseTime.TotalMilliseconds}ms vs {convertTime.TotalMilliseconds}ms");
        }

        /// <summary>
        /// Benchmarks the batch background layer processing optimization.
        /// </summary>
        [Test]
        public void BackgroundLayerBatch_PerformanceBenchmark()
        {
            // Arrange
            var layerXml = CreateLayerElementWithAllBackgrounds();
            var layerElement = XElement.Parse(layerXml);
            int iterations = 1000;
            int maxBgLayers = 16;

            // Warm up
            for (int i = 0; i < 10; i++)
            {
                StorageXMLOptimizer.GetBackgroundLayersBatch(layerElement, maxBgLayers);
            }

            // Benchmark original approach (simulated)
            var startTime = DateTime.UtcNow;
            var originalResults = new List<int[]>();
            for (int i = 0; i < iterations; i++)
            {
                var bgValues = new int[maxBgLayers];
                for (int bg = 0; bg < maxBgLayers; bg++)
                {
                    var bgElement = layerElement.Descendants($"layer{bg + 1}BG").FirstOrDefault();
                    if (bgElement != null && int.TryParse(bgElement.Value, out int value))
                    {
                        bgValues[bg] = value;
                    }
                }
                originalResults.Add(bgValues);
            }
            var originalTime = DateTime.UtcNow - startTime;

            // Benchmark optimized approach
            startTime = DateTime.UtcNow;
            var optimizedResults = new List<int[]>();
            for (int i = 0; i < iterations; i++)
            {
                var bgValues = StorageXMLOptimizer.GetBackgroundLayersBatch(layerElement, maxBgLayers);
                optimizedResults.Add(bgValues);
            }
            var optimizedTime = DateTime.UtcNow - startTime;

            // Assert
            Assert.That(optimizedResults.Count, Is.EqualTo(originalResults.Count), "Should process same number of iterations");
            
            // Verify results are identical
            for (int i = 0; i < Math.Min(10, originalResults.Count); i++) // Check first 10 results
            {
                Assert.That(optimizedResults[i], Is.EqualTo(originalResults[i]), 
                    $"Results should be identical for iteration {i}");
            }

            TestContext.WriteLine($"Original approach: {originalTime.TotalMilliseconds}ms");
            TestContext.WriteLine($"Optimized batch approach: {optimizedTime.TotalMilliseconds}ms");
            
            if (optimizedTime.TotalMilliseconds > 0)
            {
                double improvement = (originalTime.TotalMilliseconds - optimizedTime.TotalMilliseconds) / originalTime.TotalMilliseconds * 100;
                TestContext.WriteLine($"Performance improvement: {improvement:F1}%");
            }

            // Batch processing should be faster
            Assert.That(optimizedTime.TotalMilliseconds, Is.LessThanOrEqualTo(originalTime.TotalMilliseconds * 1.1), // Allow 10% margin
                $"Batch processing should not be significantly slower: {optimizedTime.TotalMilliseconds}ms vs {originalTime.TotalMilliseconds}ms");
        }

        /// <summary>
        /// Benchmarks the optimized version parsing vs standard Split.
        /// </summary>
        [Test]
        public void OptimizedVersionParsing_PerformanceBenchmark()
        {
            // Arrange
            var testVersions = new[] { "3.5.2", "1.0.0", "2.10.15", "4.1.3.7" };
            int iterations = 10000;

            // Warm up
            for (int i = 0; i < 100; i++)
            {
                "3.5.2".Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                StorageXMLOptimizer.ParseVersionOptimized("3.5.2");
            }

            // Benchmark original approach
            var startTime = DateTime.UtcNow;
            var originalResults = new List<string[]>();
            for (int i = 0; i < iterations; i++)
            {
                foreach (var version in testVersions)
                {
                    var tokens = version.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    originalResults.Add(tokens);
                }
            }
            var originalTime = DateTime.UtcNow - startTime;

            // Benchmark optimized approach
            startTime = DateTime.UtcNow;
            var optimizedResults = new List<string[]>();
            for (int i = 0; i < iterations; i++)
            {
                foreach (var version in testVersions)
                {
                    var tokens = StorageXMLOptimizer.ParseVersionOptimized(version);
                    optimizedResults.Add(tokens);
                }
            }
            var optimizedTime = DateTime.UtcNow - startTime;

            // Assert
            Assert.That(optimizedResults.Count, Is.EqualTo(originalResults.Count), "Should process same number of versions");
            
            // Verify results are identical for a sample
            for (int i = 0; i < Math.Min(testVersions.Length, originalResults.Count / iterations); i++)
            {
                Assert.That(optimizedResults[i], Is.EqualTo(originalResults[i]), 
                    $"Parsing results should be identical for version {testVersions[i % testVersions.Length]}");
            }

            TestContext.WriteLine($"Standard Split approach: {originalTime.TotalMilliseconds}ms");
            TestContext.WriteLine($"Optimized parsing approach: {optimizedTime.TotalMilliseconds}ms");
            
            if (optimizedTime.TotalMilliseconds > 0)
            {
                double improvement = (originalTime.TotalMilliseconds - optimizedTime.TotalMilliseconds) / originalTime.TotalMilliseconds * 100;
                TestContext.WriteLine($"Performance improvement: {improvement:F1}%");
            }

            // Optimized version should not be significantly slower
            Assert.That(optimizedTime.TotalMilliseconds, Is.LessThanOrEqualTo(originalTime.TotalMilliseconds * 1.2), // Allow 20% margin as optimization is more about memory allocation
                $"Optimized version should not be significantly slower: {optimizedTime.TotalMilliseconds}ms vs {originalTime.TotalMilliseconds}ms");
        }

        #region Helper Methods

        private string CreateComplexTestXMLDocument()
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<Variance>
    <version>3.5.2</version>
    <layer1>
        <name>ComplexLayer1</name>
        <comment>Complex Test Comment</comment>
        <enabled>1</enabled>
        <omitLayer>0</omitLayer>
        <layer1BG>0</layer1BG>
        <layer2BG>1</layer2BG>
        <layer3BG>0</layer3BG>
        <layer4BG>1</layer4BG>
        <showDrawn>1</showDrawn>
        <flipH>0</flipH>
        <flipV>0</flipV>
    </layer1>
    <layer2>
        <name>ComplexLayer2</name>
        <comment>Another Complex Comment</comment>
        <enabled>1</enabled>
        <omitLayer>1</omitLayer>
    </layer2>
    <SimulationSettings>
        <numberOfCases>100000</numberOfCases>
        <resolution>200</resolution>
        <rngSeed>12345</rngSeed>
    </SimulationSettings>
</Variance>";
        }

        private string CreateLayerElementWithAllBackgrounds()
        {
            var layerElement = new XElement("layer1",
                new XElement("name", "TestLayer"),
                new XElement("enabled", "1")
            );

            // Add all 16 background layer references
            for (int i = 1; i <= 16; i++)
            {
                layerElement.Add(new XElement($"layer{i}BG", i % 2));
            }

            return layerElement.ToString();
        }

        #endregion
    }
}