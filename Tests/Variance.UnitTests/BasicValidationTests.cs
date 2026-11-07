using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Variance.UnitTests
{
    /// <summary>
    /// Basic validation tests to ensure the test infrastructure is working correctly.
    /// These tests validate fundamental concepts and patterns used throughout the codebase.
    /// </summary>
    [TestFixture]
    public class BasicValidationTests
    {
        /// <summary>
        /// Tests that basic string operations work as expected.
        /// This validates the test infrastructure setup.
        /// </summary>
        [Test]
        public void StringOperations_BasicValidation_WorksCorrectly()
        {
            // Arrange
            string testString = "test.variance";
            
            // Act
            string[] parts = testString.Split('.');
            string extension = parts[^1].ToUpper();
            
            // Assert
            Assert.That("VARIANCE", Is.EqualTo(extension));
            Assert.That(2, Is.EqualTo(parts.Length));
        }

        /// <summary>
        /// Tests that file extension validation logic works correctly.
        /// This pattern is used throughout the file handling code.
        /// </summary>
        [Test]
        [TestCase("file.variance", true)]
        [TestCase("file.xml", true)]
        [TestCase("file.txt", false)]
        [TestCase("file", false)]
        public void FileExtensionValidation_VariousExtensions_ReturnsExpectedResult(string filename, bool expectedValid)
        {
            // Act
            string[] tokens = filename.Split(new char[] { '.' });
            bool isValid = tokens.Length > 1 && (tokens[^1].ToUpper() == "VARIANCE" || tokens[^1].ToUpper() == "XML");
            
            // Assert
            Assert.That(expectedValid, Is.EqualTo(isValid), $"Extension validation for {filename} should return {expectedValid}");
        }

        /// <summary>
        /// Tests that list operations work as expected for UI collections.
        /// </summary>
        [Test]
        public void ListOperations_BasicCRUD_WorksCorrectly()
        {
            // Arrange
            var testList = new List<string>();
            
            // Act & Assert - Add
            testList.Add("Item 1");
            Assert.That(1, Is.EqualTo(testList.Count));
            Assert.That("Item 1", Is.EqualTo(testList));
            
            // Act & Assert - Update
            testList[0] = "Updated Item 1";
            Assert.That("Updated Item 1", Is.EqualTo(testList[0]));
            
            // Act & Assert - Remove
            testList.Remove("Updated Item 1");
            Assert.That(0, Is.EqualTo(testList.Count));
        }

        /// <summary>
        /// Tests that array operations work as expected for multi-layer collections.
        /// </summary>
        [Test]
        public void ArrayOperations_MultipleCollections_WorksCorrectly()
        {
            // Arrange
            const int layerCount = 3;
            var collections = new List<string>[layerCount];
            
            // Act
            for (int i = 0; i < layerCount; i++)
            {
                collections[i] = new List<string>();
                collections[i].Add($"Layer {i} Item");
            }
            
            // Assert
            Assert.That(layerCount, Is.EqualTo(collections.Length));
            for (int i = 0; i < layerCount; i++)
            {
                Assert.That(collections[i], !Is.Null);
                Assert.That(1, Is.EqualTo(collections[i].Count));
                Assert.That($"Layer {i} Item", Is.EqualTo(collections[i][0]));
            }
        }

        /// <summary>
        /// Tests that boolean state management works correctly.
        /// This pattern is used throughout the simulation system.
        /// </summary>
        [Test]
        public void BooleanStateManagement_SetAndGet_WorksCorrectly()
        {
            // Arrange
            bool state = false;
            
            // Act & Assert - Initial state
            Assert.That(state, Is.False);
            
            // Act & Assert - Set true
            state = true;
            Assert.That(state, Is.True);
            
            // Act & Assert - Set false
            state = false;
            Assert.That(state, Is.False);
        }

        /// <summary>
        /// Tests that window title formatting works correctly.
        /// This validates the pattern used in saveEnabler method.
        /// </summary>
        [Test]
        public void WindowTitleFormatting_VariousStates_FormatsCorrectly()
        {
            // Arrange
            string titleText = "Variance";
            
            // Test empty filename
            string projectFileName = "";
            string title = projectFileName == "" ? titleText : titleText + " - " + projectFileName;
            Assert.That("Variance", Is.EqualTo(title));
            
            // Test with filename
            projectFileName = "myproject.variance";
            title = projectFileName == "" ? titleText : titleText + " - " + projectFileName;
            Assert.That("Variance - myproject.variance", Is.EqualTo(title));
        }

        /// <summary>
        /// Tests that exception handling patterns work correctly.
        /// </summary>
        [Test]
        public void ExceptionHandling_BasicPatterns_WorkCorrectly()
        {
            // Test that no exception is thrown for valid operations
            Assert.DoesNotThrow(() =>
            {
                var list = new List<string>();
                list.Add("test");
                _ = list.Count;
            });
            
            // Test that expected exceptions are thrown
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var list = new List<string>();
                _ = list[0]; // Should throw for empty list
            });
        }

        /// <summary>
        /// Tests performance characteristics of basic operations.
        /// </summary>
        [Test]
        public void PerformanceBaseline_BasicOperations_CompletesQuickly()
        {
            // Arrange
            const int operationCount = 10000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act
            var list = new List<string>();
            for (int i = 0; i < operationCount; i++)
            {
                list.Add($"Item {i}");
            }
            
            stopwatch.Stop();
            
            // Assert
            Assert.That(operationCount, Is.EqualTo(list.Count));
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000), "Basic operations should complete quickly");
        }
    }
}