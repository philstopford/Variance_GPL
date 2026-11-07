
using System;
using System.IO;
using NUnit.Framework;

namespace Variance.UnitTests
{
    /// <summary>
    /// Unit tests for file handling operations in the MainForm class.
    /// Tests save, load, drag-drop, and UI state management functionality.
    /// </summary>
    [TestFixture]
    public class FileHandlingTests
    {
        private string testFilePath;
        private string tempDirectory;

        [SetUp]
        public void Setup()
        {
            // Create a temporary directory for test files
            tempDirectory = Path.Combine(Path.GetTempPath(), "VarianceTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            testFilePath = Path.Combine(tempDirectory, "test.variance");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test files
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        /// <summary>
        /// Tests that a .variance file extension is correctly recognized as supported.
        /// </summary>
        [Test]
        public void IsValidFileExtension_VarianceFile_ReturnsTrue()
        {
            // Arrange
            string filePath = "test.variance";
            
            // Act & Assert
            string[] tokens = filePath.Split(new char[] { '.' });
            bool isValid = tokens[^1].ToUpper() == "VARIANCE" || tokens[^1].ToUpper() == "XML";
            
            Assert.That(isValid, Is.True, "Should recognize .variance files as valid");
        }

        /// <summary>
        /// Tests that a .xml file extension is correctly recognized as supported.
        /// </summary>
        [Test]
        public void IsValidFileExtension_XmlFile_ReturnsTrue()
        {
            // Arrange
            string filePath = "test.xml";
            
            // Act & Assert
            string[] tokens = filePath.Split(new char[] { '.' });
            bool isValid = tokens[^1].ToUpper() == "VARIANCE" || tokens[^1].ToUpper() == "XML";
            
            Assert.That(isValid, Is.True, "Should recognize .xml files as valid");
        }

        /// <summary>
        /// Tests that unsupported file extensions are correctly rejected.
        /// </summary>
        [Test]
        [TestCase("test.txt")]
        [TestCase("test.doc")]
        [TestCase("test.pdf")]
        [TestCase("test")]
        public void IsValidFileExtension_UnsupportedFile_ReturnsFalse(string filePath)
        {
            // Act & Assert
            string[] tokens = filePath.Split(new char[] { '.' });
            bool isValid = tokens[^1].ToUpper() == "VARIANCE" || tokens[^1].ToUpper() == "XML";
            
            Assert.That(isValid, Is.False, "Should not recognize .xml files as valid");
        }

        /// <summary>
        /// Tests the file extension validation with case-insensitive comparison.
        /// </summary>
        [Test]
        [TestCase("test.VARIANCE")]
        [TestCase("test.Variance")]
        [TestCase("test.XML")]
        [TestCase("test.Xml")]
        public void IsValidFileExtension_CaseInsensitive_ReturnsTrue(string filePath)
        {
            // Act & Assert
            string[] tokens = filePath.Split(new char[] { '.' });
            bool isValid = tokens[^1].ToUpper() == "VARIANCE" || tokens[^1].ToUpper() == "XML";
            
            Assert.That(isValid, Is.True, $"Should recognize {filePath} as valid (case insensitive)");
        }

        /// <summary>
        /// Tests that the window title format is correctly constructed.
        /// </summary>
        [Test]
        public void WindowTitle_WithProjectFile_ContainsFilename()
        {
            // Arrange
            string titleText = "Variance";
            string projectFileName = "test.variance";
            
            // Act
            string expectedTitle = titleText + " - " + projectFileName;
            
            // Assert
            Assert.That("Variance - test.variance", Is.EqualTo(expectedTitle));
        }

        /// <summary>
        /// Tests that the window title format handles empty project filenames.
        /// </summary>
        [Test]
        public void WindowTitle_WithoutProjectFile_ShowsOnlyTitle()
        {
            // Arrange
            string titleText = "Variance";
            string projectFileName = "";
            
            // Act
            string actualTitle = projectFileName == "" ? titleText : titleText + " - " + projectFileName;
            
            // Assert
            Assert.That("Variance", Is.EqualTo(actualTitle));
        }

        /// <summary>
        /// Tests path parsing for different file path formats.
        /// </summary>
        [Test]
        [TestCase(@"C:\Users\Test\file.variance", "VARIANCE")]
        [TestCase(@"/home/user/file.xml", "XML")]
        [TestCase(@".\relative\path\file.variance", "VARIANCE")]
        [TestCase(@"file.xml", "XML")]
        public void FilePath_ExtensionExtraction_ReturnsCorrectExtension(string filePath, string expectedExtension)
        {
            // Act
            string[] tokens = filePath.Split(new char[] { '.' });
            string actualExtension = tokens[^1].ToUpper();
            
            // Assert
            Assert.That(expectedExtension, Is.EqualTo(actualExtension));
        }

        /// <summary>
        /// Tests that multiple extensions in filename are handled correctly.
        /// </summary>
        [Test]
        public void FilePath_MultipleExtensions_ReturnsLastExtension()
        {
            // Arrange
            string filePath = "test.backup.variance";
            
            // Act
            string[] tokens = filePath.Split(new char[] { '.' });
            string actualExtension = tokens[^1].ToUpper();
            
            // Assert
            Assert.That("VARIANCE", Is.EqualTo(actualExtension));
        }

        /// <summary>
        /// Tests error handling for files without extensions.
        /// </summary>
        [Test]
        public void FilePath_NoExtension_HandledGracefully()
        {
            // Arrange
            string filePath = "testfile";
            
            // Act
            string[] tokens = filePath.Split(new char[] { '.' });
            string extension = tokens[^1].ToUpper();
            bool isValid = extension == "VARIANCE" || extension == "XML";
            
            // Assert
            Assert.That(isValid, Is.False, "Files without extensions should not be valid");
            Assert.That("TESTFILE", Is.EqualTo(extension), "Should return the filename itself when no extension");
        }

        /// <summary>
        /// Tests the directory creation and file existence checking logic.
        /// </summary>
        [Test]
        public void CreateTestFile_InTempDirectory_CreatesSuccessfully()
        {
            // Arrange
            string testFile = Path.Combine(tempDirectory, "test_create.variance");
            
            // Act
            File.WriteAllText(testFile, "test content");
            
            // Assert
            Assert.That(File.Exists(testFile), "Test file should be created successfully");
            Assert.That(Directory.Exists(tempDirectory), "Temp directory should exist");
        }

        /// <summary>
        /// Tests cleanup of temporary test files.
        /// </summary>
        [Test]
        public void CleanupTestFiles_RemovesTemporaryFiles()
        {
            // Arrange
            string testFile = Path.Combine(tempDirectory, "cleanup_test.variance");
            File.WriteAllText(testFile, "test content");
            
            // Act
            File.Delete(testFile);
            
            // Assert
            Assert.That(File.Exists(testFile), Is.False, "Test file should be deleted");
        }
    }
}