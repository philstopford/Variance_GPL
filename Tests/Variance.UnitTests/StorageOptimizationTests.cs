using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Clipper2Lib;

namespace Variance.UnitTests
{
    /// <summary>
    /// Unit tests for Storage class performance optimizations.
    /// Tests the stringFromFileData method and XML storage functionality.
    /// </summary>
    [TestFixture]
    public class StorageOptimizationTests
    {
        private Storage storage;
        private string tempDirectory;

        [SetUp]
        public void Setup()
        {
            storage = new Storage();
            tempDirectory = Path.Combine(Path.GetTempPath(), "VarianceStorageTests_" + Guid.NewGuid().ToString("N"));
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

        #region stringFromFileData Tests

        /// <summary>
        /// Tests stringFromFileData with empty input
        /// </summary>
        [Test]
        public void StringFromFileData_EmptyInput_ReturnsEmpty()
        {
            // Arrange
            var emptyData = new PathsD();
            
            // Act
            var result = InvokeStringFromFileData(emptyData);
            
            // Assert
            Assert.That(result, Is.EqualTo(""), "Empty input should return empty string");
        }

        /// <summary>
        /// Tests stringFromFileData with single point polygon
        /// </summary>
        [Test]
        public void StringFromFileData_SinglePointPolygon_ReturnsCorrectFormat()
        {
            // Arrange
            var singlePointData = new PathsD
            {
                new PathD { new PointD(10, 20) }
            };
            
            // Act
            var result = InvokeStringFromFileData(singlePointData);
            
            // Assert
            Assert.That(result, Is.EqualTo("10,20"), "Single point should return x,y format");
        }

        /// <summary>
        /// Tests stringFromFileData with single polygon multiple points
        /// </summary>
        [Test]
        public void StringFromFileData_SinglePolygonMultiplePoints_ReturnsCorrectFormat()
        {
            // Arrange
            var multiPointData = new PathsD
            {
                new PathD { new PointD(10, 20), new PointD(30, 40), new PointD(50, 60) }
            };
            
            // Act
            var result = InvokeStringFromFileData(multiPointData);
            
            // Assert
            Assert.That(result, Is.EqualTo("10,20,30,40,50,60"), "Multiple points should be comma separated");
        }

        /// <summary>
        /// Tests stringFromFileData with multiple polygons
        /// </summary>
        [Test]
        public void StringFromFileData_MultiplePolygons_ReturnsCorrectFormat()
        {
            // Arrange
            var multiPolygonData = new PathsD
            {
                new PathD { new PointD(10, 20), new PointD(30, 40) },
                new PathD { new PointD(50, 60), new PointD(70, 80) }
            };
            
            // Act
            var result = InvokeStringFromFileData(multiPolygonData);
            
            // Assert
            Assert.That(result, Is.EqualTo("10,20,30,40;50,60,70,80"), "Multiple polygons should be semicolon separated");
        }

        /// <summary>
        /// Tests stringFromFileData with complex multi-polygon data
        /// </summary>
        [Test]
        public void StringFromFileData_ComplexMultiPolygon_ReturnsCorrectFormat()
        {
            // Arrange
            var complexData = new PathsD
            {
                new PathD { new PointD(1, 2) },
                new PathD { new PointD(3, 4), new PointD(5, 6), new PointD(7, 8) },
                new PathD { new PointD(9, 10), new PointD(11, 12) }
            };
            
            // Act
            var result = InvokeStringFromFileData(complexData);
            
            // Assert
            Assert.That(result, Is.EqualTo("1,2;3,4,5,6,7,8;9,10,11,12"), "Complex data should be properly formatted");
        }

        /// <summary>
        /// Performance test for stringFromFileData with large dataset
        /// </summary>
        [Test]
        public void StringFromFileData_LargeDataset_PerformanceTest()
        {
            // Arrange - Create large dataset
            var largeData = new PathsD();
            for (int poly = 0; poly < 100; poly++)
            {
                var path = new PathD();
                for (int pt = 0; pt < 100; pt++)
                {
                    path.Add(new PointD(pt * 10, pt * 20 + poly * 1000));
                }
                largeData.Add(path);
            }

            // Act & Assert - Should complete within reasonable time
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = InvokeStringFromFileData(largeData);
            stopwatch.Stop();

            Assert.That(result, Is.Not.Empty, "Large dataset should produce non-empty result");
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), "Large dataset should process within 5 seconds");
            Assert.That(result.Contains(";"), Is.True, "Large dataset should contain polygon separators");
            Assert.That(result.Contains(","), Is.True, "Large dataset should contain point separators");
        }

        /// <summary>
        /// Tests stringFromFileData with decimal coordinates
        /// </summary>
        [Test]
        public void StringFromFileData_DecimalCoordinates_ReturnsCorrectFormat()
        {
            // Arrange
            var decimalData = new PathsD
            {
                new PathD { new PointD(10.5, 20.7), new PointD(30.2, 40.9) }
            };
            
            // Act
            var result = InvokeStringFromFileData(decimalData);
            
            // Assert
            Assert.That(result, Is.EqualTo("10.5,20.7,30.2,40.9"), "Decimal coordinates should be preserved");
        }

        /// <summary>
        /// Tests stringFromFileData with negative coordinates
        /// </summary>
        [Test]
        public void StringFromFileData_NegativeCoordinates_ReturnsCorrectFormat()
        {
            // Arrange
            var negativeData = new PathsD
            {
                new PathD { new PointD(-10, -20), new PointD(30, -40) }
            };
            
            // Act
            var result = InvokeStringFromFileData(negativeData);
            
            // Assert
            Assert.That(result, Is.EqualTo("-10,-20,30,-40"), "Negative coordinates should be handled correctly");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper method to invoke private stringFromFileData method using reflection
        /// </summary>
        private string InvokeStringFromFileData(PathsD fileData)
        {
            var method = typeof(Storage).GetMethod("stringFromFileData", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null, "stringFromFileData method should exist");
            
            var result = method.Invoke(null, new object[] { fileData });
            return result as string ?? string.Empty;
        }

        #endregion
    }
}