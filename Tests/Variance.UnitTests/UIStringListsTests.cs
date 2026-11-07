using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Variance;

namespace Variance.UnitTests
{
    /// <summary>
    /// Unit tests for the UIStringLists class that manages collections of strings
    /// used in UI controls throughout the Variance application.
    /// </summary>
    [TestFixture]
    public class UIStringListsTests
    {
        private MainForm.UIStringLists uiStringLists;

        [SetUp]
        public void Setup()
        {
            uiStringLists = new MainForm.UIStringLists();
        }

        /// <summary>
        /// Tests that ObservableCollection properties can be initialized and modified.
        /// </summary>
        [Test]
        public void ObservableCollection_Initialization_WorksCorrectly()
        {
            // Arrange & Act
            uiStringLists.calcModes = new ObservableCollection<string>();
            uiStringLists.calcModes.Add("Test Mode");
            
            // Assert
            Assert.That(uiStringLists.calcModes, !Is.Null);
            Assert.That(1, Is.EqualTo(uiStringLists.calcModes.Count));
            Assert.That("Test Mode", Is.EqualTo(uiStringLists.calcModes[0]));
        }

        /// <summary>
        /// Tests that List properties can be initialized and populated.
        /// </summary>
        [Test]
        public void List_Initialization_WorksCorrectly()
        {
            // Arrange & Act
            uiStringLists.shapes = new List<string> { "Rectangle", "Circle", "Polygon" };
            
            // Assert
            Assert.That(uiStringLists.shapes, !Is.Null);
            Assert.That(3, Is.EqualTo(uiStringLists.shapes.Count));
            Assert.That( uiStringLists.shapes, Contains.Item("Rectangle"));
            Assert.That(uiStringLists.shapes, Contains.Item("Circle"));
            Assert.That(uiStringLists.shapes, Contains.Item("Polygon"));
        }

        /// <summary>
        /// Tests that array properties can be initialized with multiple collections.
        /// </summary>
        [Test]
        public void ArrayOfCollections_Initialization_WorksCorrectly()
        {
            // Arrange
            const int layerCount = 3;
            
            // Act
            uiStringLists.subShapeList = new ObservableCollection<string>[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                uiStringLists.subShapeList[i] = new ObservableCollection<string>();
                uiStringLists.subShapeList[i].Add($"SubShape_{i}");
            }
            
            // Assert
            Assert.That(uiStringLists.subShapeList, !Is.Null);
            Assert.That(layerCount, Is.EqualTo(uiStringLists.subShapeList.Length));
            
            for (int i = 0; i < layerCount; i++)
            {
                Assert.That(uiStringLists.subShapeList[i], !Is.Null);
                Assert.That(1, Is.EqualTo(uiStringLists.subShapeList[i].Count));
                Assert.That($"SubShape_{i}", Is.EqualTo(uiStringLists.subShapeList[i][0]));
            }
        }

        /// <summary>
        /// Tests ObservableCollection change notification behavior.
        /// </summary>
        [Test]
        public void ObservableCollection_ChangeNotification_FiresEvents()
        {
            // Arrange
            uiStringLists.layerNames = new ObservableCollection<string>();
            bool eventFired = false;
            
            uiStringLists.layerNames.CollectionChanged += (sender, e) => eventFired = true;
            
            // Act
            uiStringLists.layerNames.Add("Layer 1");
            
            // Assert
            Assert.That(eventFired, Is.True, "CollectionChanged event should fire when items are added");
            Assert.That(1, Is.EqualTo(uiStringLists.layerNames.Count));
        }

        /// <summary>
        /// Tests that GeoCore structure lists can handle typical workflow.
        /// </summary>
        [Test]
        public void GeoCoreStructureList_TypicalWorkflow_WorksCorrectly()
        {
            // Arrange
            const int layerCount = 2;
            uiStringLists.geoCoreStructureList = new ObservableCollection<string>[layerCount];
            
            // Act - Initialize collections
            for (int i = 0; i < layerCount; i++)
            {
                uiStringLists.geoCoreStructureList[i] = new ObservableCollection<string>();
            }
            
            // Add some typical structure names
            uiStringLists.geoCoreStructureList[0].Add("ACTIVE");
            uiStringLists.geoCoreStructureList[0].Add("POLY");
            uiStringLists.geoCoreStructureList[1].Add("METAL1");
            uiStringLists.geoCoreStructureList[1].Add("VIA1");
            
            // Assert
            Assert.That(2, Is.EqualTo(uiStringLists.geoCoreStructureList[0].Count));
            Assert.That(2, Is.EqualTo(uiStringLists.geoCoreStructureList[1].Count));
            Assert.That(uiStringLists.geoCoreStructureList[0].ToList(), Contains.Item("ACTIVE"));
            Assert.That(uiStringLists.geoCoreStructureList[1].ToList(), Contains.Item("METAL1"));
        }

        /// <summary>
        /// Tests that RNG type lists contain expected values.
        /// </summary>
        [Test]
        public void RngTypeList_CommonValues_AreSupported()
        {
            // Arrange & Act
            uiStringLists.rngTypeList = new List<string>
            {
                "Mersenne Twister",
                "System Random",
                "Crypto Random"
            };
            
            // Assert
            Assert.That(3, Is.EqualTo(uiStringLists.rngTypeList.Count));
            Assert.That(uiStringLists.rngTypeList, Contains.Item("Mersenne Twister"));
            Assert.That(uiStringLists.rngTypeList, Contains.Item("System Random"));
            Assert.That(uiStringLists.rngTypeList, Contains.Item("Crypto Random"));
        }

        /// <summary>
        /// Tests that Boolean operation lists contain expected values.
        /// </summary>
        [Test]
        public void BooleanList_CommonOperations_AreSupported()
        {
            // Arrange & Act
            uiStringLists.booleanList = new List<string>
            {
                "Union",
                "Difference", 
                "Intersection",
                "XOR"
            };
            
            // Assert
            Assert.That(4, Is.EqualTo(uiStringLists.booleanList.Count));
            Assert.That(uiStringLists.booleanList, Contains.Item("Union"));
            Assert.That(uiStringLists.booleanList, Contains.Item("Difference"));
            Assert.That(uiStringLists.booleanList, Contains.Item("Intersection"));
            Assert.That(uiStringLists.booleanList, Contains.Item("XOR"));
        }

        /// <summary>
        /// Tests that tip location lists contain expected values.
        /// </summary>
        [Test]
        public void TipLocationList_CommonLocations_AreSupported()
        {
            // Arrange & Act
            uiStringLists.tipLocs = new List<string>
            {
                "Top Left",
                "Top Right",
                "Bottom Left",
                "Bottom Right",
                "Center"
            };
            
            // Assert
            Assert.That(5, Is.EqualTo(uiStringLists.tipLocs.Count));
            Assert.That(uiStringLists.tipLocs, Contains.Item("Top Left"));
            Assert.That(uiStringLists.tipLocs, Contains.Item("Center"));
        }

        /// <summary>
        /// Tests that noise type lists support Line Edge Roughness types.
        /// </summary>
        [Test]
        public void NoiseTypeList_LERTypes_AreSupported()
        {
            // Arrange & Act
            uiStringLists.noiseTypeList = new List<string>
            {
                "Perlin",
                "Simplex",
                "White",
                "Pink"
            };
            
            // Assert
            Assert.That(4, Is.EqualTo(uiStringLists.noiseTypeList.Count));
            Assert.That(uiStringLists.noiseTypeList, Contains.Item("Perlin"));
            Assert.That(uiStringLists.noiseTypeList, Contains.Item("Simplex"));
        }

        /// <summary>
        /// Tests that falloff function lists contain expected mathematical functions.
        /// </summary>
        [Test]
        public void FallOffList_MathematicalFunctions_AreSupported()
        {
            // Arrange & Act
            uiStringLists.fallOffList = new List<string>
            {
                "Linear",
                "Gaussian",
                "Exponential",
                "Cosine"
            };
            
            // Assert
            Assert.That(4, Is.EqualTo(uiStringLists.fallOffList.Count));
            Assert.That(uiStringLists.fallOffList, Contains.Item("Linear"));
            Assert.That(uiStringLists.fallOffList, Contains.Item("Gaussian"));
        }

        /// <summary>
        /// Tests error handling for null collection access.
        /// </summary>
        [Test]
        public void NullCollection_Access_HandledGracefully()
        {
            // Arrange - collections are null by default
            
            // Act & Assert
            Assert.That(uiStringLists.shapes, Is.Null);
            Assert.That(uiStringLists.calcModes, Is.Null);
            Assert.That(uiStringLists.layerNames, Is.Null);
            
            // These should not throw exceptions
            Assert.DoesNotThrow(() => {
                var shapes = uiStringLists.shapes;
                var modes = uiStringLists.calcModes;
            });
        }

        /// <summary>
        /// Tests that export-specific collections work independently.
        /// </summary>
        [Test]
        public void ExportCollections_IndependentOperation_WorksCorrectly()
        {
            // Arrange
            uiStringLists.geoCoreStructureList_exp = new ObservableCollection<string>();
            uiStringLists.subShapesList_exp = new ObservableCollection<string>();
            
            // Act
            uiStringLists.geoCoreStructureList_exp.Add("Export Structure");
            uiStringLists.subShapesList_exp.Add("Export SubShape");
            
            // Assert
            Assert.That(1, Is.EqualTo(uiStringLists.geoCoreStructureList_exp.Count));
            Assert.That(1, Is.EqualTo(uiStringLists.subShapesList_exp.Count));
            Assert.That("Export Structure", Is.EqualTo(uiStringLists.geoCoreStructureList_exp[0]));
            Assert.That("Export SubShape", Is.EqualTo(uiStringLists.subShapesList_exp[0]));
        }

        /// <summary>
        /// Tests collection capacity and performance with large datasets.
        /// </summary>
        [Test]
        public void LargeCollection_Performance_IsAcceptable()
        {
            // Arrange
            uiStringLists.layerNames = new ObservableCollection<string>();
            const int itemCount = 1000;
            
            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < itemCount; i++)
            {
                uiStringLists.layerNames.Add($"Layer_{i:D4}");
            }
            
            stopwatch.Stop();
            
            // Assert
            Assert.That(itemCount, Is.EqualTo(uiStringLists.layerNames.Count));
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000), "Adding 1000 items should take less than 1 second");
        }
    }
}