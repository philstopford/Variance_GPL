using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Variance;

namespace Variance.UnitTests
{
    /// <summary>
    /// Comprehensive backward compatibility validation tests for XML optimizations.
    /// Ensures that all optimizations maintain 100% identical behavior to the original implementation.
    /// </summary>
    [TestFixture]
    public class XMLOptimizationCompatibilityTests
    {
        private string testXmlPath;
        private Storage storage;
        
        [SetUp]
        public void Setup()
        {
            storage = new Storage();
            testXmlPath = Path.Combine(Path.GetTempPath(), "compat_test.variance");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(testXmlPath))
                File.Delete(testXmlPath);
        }

        /// <summary>
        /// Tests that optimized element caching produces identical results to non-cached access.
        /// This is critical for maintaining backward compatibility.
        /// </summary>
        [Test]
        public void CachedElementAccess_ProducesIdenticalResults()
        {
            // Arrange
            var testXml = CreateTestXMLWithMultipleLayers();
            var rootElement = XElement.Parse(testXml);
            var layerNames = new[] { "layer1", "layer2", "layer3" };
            
            foreach (var layerName in layerNames)
            {
                // Clear cache for consistent testing
                StorageXMLOptimizer.ClearCache();
                
                // Act - Original approach
                var originalElement = rootElement.Descendants(layerName).FirstOrDefault();
                var originalEnabled = originalElement?.Descendants("enabled").FirstOrDefault()?.Value;
                var originalComment = originalElement?.Descendants("comment").FirstOrDefault()?.Value;
                var originalName = originalElement?.Descendants("name").FirstOrDefault()?.Value;
                
                // Act - Optimized approach
                var cachedElement = StorageXMLOptimizer.GetCachedElement(rootElement, layerName);
                var cachedEnabled = cachedElement?.Element("enabled")?.Value;
                var cachedComment = cachedElement?.Element("comment")?.Value;
                var cachedName = cachedElement?.Element("name")?.Value;
                
                // Assert
                Assert.That(cachedEnabled, Is.EqualTo(originalEnabled), $"Enabled values should match for {layerName}");
                Assert.That(cachedComment, Is.EqualTo(originalComment), $"Comment values should match for {layerName}");
                Assert.That(cachedName, Is.EqualTo(originalName), $"Name values should match for {layerName}");
                
                // Test that cache returns same element on subsequent calls
                var cachedElement2 = StorageXMLOptimizer.GetCachedElement(rootElement, layerName);
                Assert.That(cachedElement2?.Value, Is.EqualTo(cachedElement?.Value), 
                    $"Cached element should return identical value on subsequent calls for {layerName}");
            }
        }

        /// <summary>
        /// Validates that safe parsing methods handle all edge cases identically to Convert methods.
        /// </summary>
        [Test]
        [TestCase("123", 123, true)]
        [TestCase("0", 0, true)]
        [TestCase("-456", -456, true)]
        [TestCase("", 0, false)]  // Empty string should use default
        [TestCase("abc", 0, false)]  // Invalid string should use default
        [TestCase("123.45", 0, false)]  // Decimal in int parsing should use default
        [TestCase("  789  ", 789, true)]  // Whitespace should be handled
        public void SafeIntParsing_HandlesAllCasesLikeOriginal(string input, int expectedValue, bool shouldSucceed)
        {
            // Arrange - Test the original approach behavior
            int originalResult = 0;
            bool originalSuccess = false;
            try
            {
                originalResult = Convert.ToInt32(input);
                originalSuccess = true;
            }
            catch
            {
                originalResult = 0; // Default value used in original code
                originalSuccess = false;
            }

            // Act - Test optimized approach
            var element = new XElement("test", input);
            int optimizedResult = StorageXMLOptimizer.GetIntValueSafe(element, 0);
            bool optimizedSuccess = int.TryParse(input, out _);

            // Assert
            if (shouldSucceed)
            {
                Assert.That(optimizedSuccess, Is.EqualTo(originalSuccess), 
                    $"Success status should match for input '{input}'");
                Assert.That(optimizedResult, Is.EqualTo(expectedValue), 
                    $"Parsed value should be {expectedValue} for input '{input}'");
            }
            else
            {
                Assert.That(optimizedResult, Is.EqualTo(0), 
                    $"Should use default value for invalid input '{input}'");
            }
        }

        /// <summary>
        /// Validates safe double parsing against original Convert approach.
        /// </summary>
        [Test]
        [TestCase("123.456", 123.456, true)]
        [TestCase("0.0", 0.0, true)]
        [TestCase("-789.123", -789.123, true)]
        [TestCase("", 0.0, false)]
        [TestCase("abc", 0.0, false)]
        [TestCase("123", 123.0, true)]  // Integer should parse as double
        public void SafeDoubleParsing_HandlesAllCasesLikeOriginal(string input, double expectedValue, bool shouldSucceed)
        {
            // Arrange - Test original approach
            double originalResult = 0.0;
            bool originalSuccess = false;
            try
            {
                originalResult = Convert.ToDouble(input);
                originalSuccess = true;
            }
            catch
            {
                originalResult = 0.0;
                originalSuccess = false;
            }

            // Act - Test optimized approach
            var element = new XElement("test", input);
            double optimizedResult = StorageXMLOptimizer.GetDoubleValueSafe(element, 0.0);
            bool optimizedSuccess = double.TryParse(input, out _);

            // Assert
            if (shouldSucceed)
            {
                Assert.That(optimizedResult, Is.EqualTo(expectedValue).Within(0.0001), 
                    $"Parsed value should be {expectedValue} for input '{input}'");
                Assert.That(optimizedSuccess, Is.EqualTo(originalSuccess), 
                    $"Success status should match for input '{input}'");
            }
            else
            {
                Assert.That(optimizedResult, Is.EqualTo(0.0).Within(0.0001),
                    $"Should use default value for invalid input '{input}'");
            }
        }

        /// <summary>
        /// Tests that background layer batch processing produces identical results to individual processing.
        /// </summary>
        [Test]
        public void BackgroundLayerBatch_ProducesIdenticalResults()
        {
            // Arrange
            var layerXml = @"<layer1>
                <layer1BG>1</layer1BG>
                <layer2BG>0</layer2BG>
                <layer3BG>1</layer3BG>
                <layer4BG>1</layer4BG>
                <layer5BG>0</layer5BG>
                <layer6BG>1</layer6BG>
                <layer7BG>0</layer7BG>
                <layer8BG>0</layer8BG>
                <layer9BG>1</layer9BG>
                <layer10BG>1</layer10BG>
                <layer11BG>0</layer11BG>
                <layer12BG>1</layer12BG>
                <layer13BG>1</layer13BG>
                <layer14BG>0</layer14BG>
                <layer15BG>1</layer15BG>
                <layer16BG>0</layer16BG>
            </layer1>";
            var layerElement = XElement.Parse(layerXml);
            int maxBgLayers = 16;

            // Act - Individual processing (original approach simulation)
            var individualResults = new int[maxBgLayers];
            for (int i = 0; i < maxBgLayers; i++)
            {
                var bgElementName = $"layer{i + 1}BG";
                var bgElement = layerElement.Descendants(bgElementName).FirstOrDefault();
                if (bgElement != null && int.TryParse(bgElement.Value, out int value))
                {
                    individualResults[i] = value;
                }
                else
                {
                    individualResults[i] = 0;
                }
            }

            // Act - Batch processing (optimized approach)
            var batchResults = StorageXMLOptimizer.GetBackgroundLayersBatch(layerElement, maxBgLayers);

            // Assert
            Assert.That(batchResults.Length, Is.EqualTo(individualResults.Length), 
                "Both approaches should return arrays of the same length");
            
            for (int i = 0; i < maxBgLayers; i++)
            {
                Assert.That(batchResults[i], Is.EqualTo(individualResults[i]), 
                    $"Background layer {i + 1} value should match: individual={individualResults[i]}, batch={batchResults[i]}");
            }
        }

        /// <summary>
        /// Tests that version parsing optimization produces identical results to original Split approach.
        /// </summary>
        [Test]
        [TestCase("3.5.2")]
        [TestCase("1.0.0")]
        [TestCase("2.10.15")]
        [TestCase("4.1.3.7")]
        [TestCase("1")]  // Single version number
        [TestCase("")]   // Empty string
        public void OptimizedVersionParsing_ProducesIdenticalResults(string version)
        {
            // Act - Original approach
            var originalTokens = version.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Act - Optimized approach
            var optimizedTokens = StorageXMLOptimizer.ParseVersionOptimized(version);

            // Assert
            Assert.That(optimizedTokens.Length, Is.EqualTo(originalTokens.Length), 
                $"Token count should match for version '{version}'");
            
            for (int i = 0; i < originalTokens.Length; i++)
            {
                Assert.That(optimizedTokens[i], Is.EqualTo(originalTokens[i]), 
                    $"Token {i} should match for version '{version}': '{originalTokens[i]}' vs '{optimizedTokens[i]}'");
            }
        }

        /// <summary>
        /// Tests handling of missing XML elements to ensure graceful degradation.
        /// </summary>
        [Test]
        public void MissingElements_HandledGracefully()
        {
            // Arrange - XML with missing elements
            var incompleteXml = @"<layer1>
                <name>TestLayer</name>
                <!-- missing: enabled, comment -->
                <layer1BG>1</layer1BG>
                <!-- missing: layer2BG, layer3BG, etc. -->
            </layer1>";
            var layerElement = XElement.Parse(incompleteXml);

            // Act & Assert - Should not throw exceptions
            Assert.DoesNotThrow(() =>
            {
                var cachedElement = StorageXMLOptimizer.GetCachedElement(layerElement, "layer1");
                Assert.That(cachedElement, Is.Not.Null);

                var enabledValue = StorageXMLOptimizer.GetIntValueSafe(cachedElement.Element("enabled"), 0);
                Assert.That(enabledValue, Is.EqualTo(0), "Missing enabled element should return default value");

                var commentValue = StorageXMLOptimizer.GetStringValueSafe(cachedElement.Element("comment"), "");
                Assert.That(commentValue, Is.EqualTo(""), "Missing comment element should return default value");

                var bgValues = StorageXMLOptimizer.GetBackgroundLayersBatch(cachedElement, 16);
                Assert.That(bgValues[0], Is.EqualTo(1), "layer1BG should be found");
                Assert.That(bgValues[1], Is.EqualTo(0), "layer2BG should default to 0");
            });
        }

        /// <summary>
        /// Tests that null elements are handled safely without exceptions.
        /// </summary>
        [Test]
        public void NullElements_HandledSafely()
        {
            // Act & Assert - Should not throw exceptions
            Assert.DoesNotThrow(() =>
            {
                var intValue = StorageXMLOptimizer.GetIntValueSafe(null, 42);
                Assert.That(intValue, Is.EqualTo(42), "Null element should return default int value");

                var doubleValue = StorageXMLOptimizer.GetDoubleValueSafe(null, 3.14);
                Assert.That(doubleValue, Is.EqualTo(3.14).Within(0.001), "Null element should return default double value");

                var stringValue = StorageXMLOptimizer.GetStringValueSafe(null, "default");
                Assert.That(stringValue, Is.EqualTo("default"), "Null element should return default string value");

                var cachedElement = StorageXMLOptimizer.GetCachedElement(new XElement("root"), "nonexistent");
                Assert.That(cachedElement, Is.Null, "Non-existent element should return null");
            });
        }

        #region Helper Methods

        private string CreateTestXMLWithMultipleLayers()
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<Variance>
    <version>3.5.2</version>
    <layer1>
        <name>Layer1</name>
        <comment>First layer comment</comment>
        <enabled>1</enabled>
        <omitLayer>0</omitLayer>
    </layer1>
    <layer2>
        <name>Layer2</name>
        <comment>Second layer comment</comment>
        <enabled>0</enabled>
        <omitLayer>1</omitLayer>
    </layer2>
    <layer3>
        <name>Layer3</name>
        <comment>Third layer comment</comment>
        <enabled>1</enabled>
        <omitLayer>0</omitLayer>
    </layer3>
</Variance>";
        }

        #endregion
    }
}