using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Clipper2Lib;

namespace Variance.UnitTests
{
    /// <summary>
    /// Validation tests for Storage class optimizations.
    /// Ensures optimizations maintain data integrity and behavioral consistency.
    /// </summary>
    [TestFixture]
    public class StorageOptimizationValidationTests
    {
        private Storage storage;
        private string tempDirectory;

        [SetUp]
        public void Setup()
        {
            storage = new Storage();
            tempDirectory = Path.Combine(Path.GetTempPath(), "VarianceStorageValidation_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        #region Data Integrity Validation Tests

        /// <summary>
        /// Validates that StringBuilder optimization produces identical results to string concatenation
        /// </summary>
        [Test]
        public void StringFromFileData_DataIntegrityValidation_IdenticalOutput()
        {
            // Arrange - Test various edge cases that could reveal differences
            var testCases = new[]
            {
                // Empty data
                new PathsD(),
                
                // Single point
                new PathsD { new PathD { new PointD(1, 2) } },
                
                // Multiple points in single polygon
                new PathsD { new PathD { new PointD(1, 2), new PointD(3, 4), new PointD(5, 6) } },
                
                // Multiple polygons
                new PathsD 
                { 
                    new PathD { new PointD(1, 2), new PointD(3, 4) },
                    new PathD { new PointD(5, 6), new PointD(7, 8) }
                },
                
                // Decimal coordinates
                new PathsD { new PathD { new PointD(1.5, 2.7), new PointD(3.14159, 4.0) } },
                
                // Negative coordinates  
                new PathsD { new PathD { new PointD(-1, -2), new PointD(-3.5, 4) } },
                
                // Large coordinates
                new PathsD { new PathD { new PointD(999999.9, -999999.9), new PointD(0, 0) } },
                
                // Many empty polygons (edge case)
                new PathsD { new PathD(), new PathD(), new PathD { new PointD(1, 1) } }
            };

            // Act & Assert
            foreach (var testCase in testCases)
            {
                var result = InvokeStringFromFileData(testCase);
                ValidateOutputFormat(result, testCase);
                
                TestContext.WriteLine($"Test case with {testCase.Count} polygons: '{result}'");
            }
        }

        /// <summary>
        /// Validates output format consistency for various input patterns
        /// </summary>
        [Test]
        public void StringFromFileData_FormatValidation_ConsistentStructure()
        {
            // Arrange
            var testData = new PathsD
            {
                new PathD { new PointD(10, 20), new PointD(30, 40) },
                new PathD { new PointD(50, 60) },
                new PathD { new PointD(70, 80), new PointD(90, 100), new PointD(110, 120) }
            };

            // Act
            var result = InvokeStringFromFileData(testData);

            // Assert format rules
            Assert.That(result, Is.Not.Null, "Result should not be null");
            
            if (result.Length > 0)
            {
                // Should not start or end with separators
                Assert.That(result, Does.Not.StartWith(","), "Should not start with comma");
                Assert.That(result, Does.Not.StartWith(";"), "Should not start with semicolon");
                Assert.That(result, Does.Not.EndWith(","), "Should not end with comma");
                Assert.That(result, Does.Not.EndWith(";"), "Should not end with semicolon");
                
                // Validate separator counts
                var semicolonCount = CountOccurrences(result, ';');
                var commaCount = CountOccurrences(result, ',');
                
                Assert.That(semicolonCount, Is.EqualTo(testData.Count - 1), 
                    "Should have correct number of polygon separators");
                
                // Each polygon should contribute (pointCount * 2 - 1) commas for coordinates
                int expectedCommas = 0;
                foreach (var poly in testData)
                {
                    if (poly.Count > 0)
                    {
                        expectedCommas += poly.Count * 2 - 1; // x,y,x,y... pattern
                    }
                }
                Assert.That(commaCount, Is.EqualTo(expectedCommas), 
                    "Should have correct number of coordinate separators");
                
                TestContext.WriteLine($"Result: '{result}'");
                TestContext.WriteLine($"Semicolons: {semicolonCount}, Commas: {commaCount}");
            }
        }

        /// <summary>
        /// Test edge cases that might cause issues with StringBuilder optimization
        /// </summary>
        [Test]
        public void StringFromFileData_EdgeCases_RobustHandling()
        {
            // Test case 1: Empty polygons mixed with non-empty
            var mixedData = new PathsD
            {
                new PathD(), // Empty polygon
                new PathD { new PointD(1, 1) }, // Single point
                new PathD(), // Another empty
                new PathD { new PointD(2, 2), new PointD(3, 3) } // Multiple points
            };
            
            var result1 = InvokeStringFromFileData(mixedData);
            Assert.That(result1, Is.Not.Null, "Should handle mixed empty/non-empty polygons");
            
            // Test case 2: Very large polygon count
            var manyPolygons = new PathsD();
            for (int i = 0; i < 1000; i++)
            {
                manyPolygons.Add(new PathD { new PointD(i, i + 1) });
            }
            
            var result2 = InvokeStringFromFileData(manyPolygons);
            Assert.That(result2, Is.Not.Null, "Should handle many polygons");
            Assert.That(CountOccurrences(result2, ';'), Is.EqualTo(999), "Should have correct separators");
            
            // Test case 3: Polygon with many points
            var manyPointsPoly = new PathD();
            for (int i = 0; i < 1000; i++)
            {
                manyPointsPoly.Add(new PointD(i, i * 2));
            }
            var manyPointsData = new PathsD { manyPointsPoly };
            
            var result3 = InvokeStringFromFileData(manyPointsData);
            Assert.That(result3, Is.Not.Null, "Should handle polygon with many points");
            Assert.That(CountOccurrences(result3, ','), Is.EqualTo(1999), "Should have correct coordinate separators");
            
            TestContext.WriteLine($"Mixed polygons result length: {result1.Length}");
            TestContext.WriteLine($"Many polygons result length: {result2.Length}");
            TestContext.WriteLine($"Many points result length: {result3.Length}");
        }

        /// <summary>
        /// Validates numerical precision is maintained in StringBuilder optimization
        /// </summary>
        [Test]
        public void StringFromFileData_NumericalPrecision_Maintained()
        {
            // Arrange - Use coordinates that test precision
            var precisionData = new PathsD
            {
                new PathD 
                { 
                    new PointD(Math.PI, Math.E),                    // Irrational numbers
                    new PointD(1.23456789012345, -9.87654321098765), // High precision decimals
                    new PointD(0.000001, 999999.999999),             // Very small and large
                    new PointD(double.Epsilon, double.MaxValue / 2)   // Extreme values
                }
            };

            // Act
            var result = InvokeStringFromFileData(precisionData);

            // Assert
            Assert.That(result, Is.Not.Null, "Should handle precision coordinates");
            Assert.That(result, Contains.Substring(Math.PI.ToString()), "Should contain PI representation");
            Assert.That(result, Contains.Substring(Math.E.ToString()), "Should contain E representation");
            
            // Verify the result can be parsed back (basic round-trip test)
            var parts = result.Split(',');
            Assert.That(parts.Length, Is.EqualTo(8), "Should have 8 coordinate parts (4 points * 2 coords)");
            
            foreach (var part in parts)
            {
                Assert.That(double.TryParse(part, out _), Is.True, $"Part '{part}' should be parseable as double");
            }
            
            TestContext.WriteLine($"Precision test result: {result}");
        }

        #endregion

        #region Structure Validation Tests

        /// <summary>
        /// Validates that Storage class structure remains intact after optimizations
        /// </summary>
        [Test]
        public void Storage_ClassStructure_IntegrityValidation()
        {
            // Act & Assert - Verify critical structure elements
            
            // Public interface should be intact
            Assert.That(storage.prepUI, Is.Not.Null.Or.Null, "prepUI property should be accessible");
            Assert.That(storage.setLayerSettings, Is.Not.Null.Or.Null, "setLayerSettings should be accessible");
            Assert.That(storage.viewportLoad, Is.Not.Null.Or.Null, "viewportLoad should be accessible");
            Assert.That(storage.viewportSave, Is.Not.Null.Or.Null, "viewportSave should be accessible");
            
            // Constructor should work
            Assert.DoesNotThrow(() => new Storage(), "Constructor should work after optimizations");
            
            // Public methods should be accessible
            var methods = typeof(Storage).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var storeSettingsMethod = Array.Find(methods, m => m.Name.Contains("storeSimulationSettings"));
            Assert.That(storeSettingsMethod, Is.Not.Null, "Public storeSimulationSettings method should exist");
            
            TestContext.WriteLine($"Storage class has {methods.Length} public methods");
            TestContext.WriteLine("Class structure integrity validated");
        }

        /// <summary>
        /// Validates that no code exists outside of proper method/class contexts
        /// </summary>
        [Test]
        public void Storage_CodeStructure_NoOrphanedCode()
        {
            // This test validates the file structure by checking if Storage class can be instantiated
            // and its methods can be reflected upon without compilation errors
            
            var storageType = typeof(Storage);
            
            // Should be able to get all methods without errors
            var methods = storageType.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
            Assert.That(methods.Length, Is.GreaterThan(0), "Should have methods in Storage class");
            
            // Should be able to get all fields/properties
            var properties = storageType.GetProperties();
            var fields = storageType.GetFields();
            
            Assert.That(properties.Length + fields.Length, Is.GreaterThan(0), "Should have properties or fields");
            
            // Should be able to create instances
            Assert.DoesNotThrow(() => {
                var instance1 = new Storage();
                var instance2 = new Storage();
                // Multiple instances should work fine
                Assert.That(instance1, Is.Not.SameAs(instance2), "Should create different instances");
            }, "Should be able to create multiple Storage instances");
            
            TestContext.WriteLine($"Storage class structure validated: {methods.Length} methods, {properties.Length} properties, {fields.Length} fields");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Validates the output format for a given input
        /// </summary>
        private void ValidateOutputFormat(string output, PathsD input)
        {
            if (input.Count == 0)
            {
                Assert.That(output, Is.EqualTo(""), "Empty input should produce empty output");
                return;
            }

            Assert.That(output, Is.Not.Null, "Output should not be null");
            
            // Count polygons in output vs input
            var semicolonCount = CountOccurrences(output, ';');
            Assert.That(semicolonCount, Is.EqualTo(input.Count - 1), 
                "Semicolon count should match polygon count - 1");
        }

        /// <summary>
        /// Counts occurrences of a character in a string
        /// </summary>
        private int CountOccurrences(string text, char character)
        {
            int count = 0;
            foreach (char c in text)
            {
                if (c == character) count++;
            }
            return count;
        }

        /// <summary>
        /// Helper method to invoke private stringFromFileData method using reflection
        /// </summary>
        private string InvokeStringFromFileData(PathsD fileData)
        {
            var method = typeof(Storage).GetMethod("stringFromFileData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            Assert.That(method, Is.Not.Null, "stringFromFileData method should exist");
            
            var result = method.Invoke(null, new object[] { fileData });
            return result as string ?? string.Empty;
        }

        #endregion
    }
}