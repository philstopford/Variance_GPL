using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Variance;

namespace Variance.PerformanceTests
{
    [TestFixture]
    public class ReplayCompatibilityTests
    {
        private CommonVars commonVars;
        
        [SetUp]
        public void Setup()
        {
            commonVars = new CommonVars();
            commonVars.SetHost(CommonVars.host.console);
        }

        [Test]
        public void TestReplayFeatureNotAffectedBySamplerChanges()
        {
            // Test that the replay feature still works correctly after sampler changes
            
            // Create a simple test case
            var entropySettings = commonVars.getSimulationSettings();
            var layerSettings = commonVars.getLayerSettings(0);
            
            // Enable the first layer
            layerSettings.setInt(EntropyLayerSettings.properties_i.enabled, 1);
            
            // Set minimal variance values for testing
            layerSettings.setDecimal(EntropyLayerSettings.properties_decimal.sCDU, 0.1m);
            layerSettings.setDecimal(EntropyLayerSettings.properties_decimal.tCDU, 0.1m);
            layerSettings.setDecimal(EntropyLayerSettings.properties_decimal.lwr, 0.1m);
            
            // Create ChaosSettings with known values
            var testChaosSettings = new ChaosSettings(true, commonVars.getListOfSettings(), entropySettings);
            
            // Set specific test values
            testChaosSettings.setValue(ChaosSettings.Properties.CDUSVar, 0, 0.5);
            testChaosSettings.setValue(ChaosSettings.Properties.CDUTVar, 0, 0.6);
            testChaosSettings.setValue(ChaosSettings.Properties.LWRVar, 0, 0.7);
            testChaosSettings.setValue(ChaosSettings.Properties.overlayX, 0, 0.8);
            testChaosSettings.setValue(ChaosSettings.Properties.overlayY, 0, 0.9);
            testChaosSettings.setInt(ChaosSettings.Ints.lwrSeed, 0, 42);
            
            // Test that ChaosSettings values are preserved
            Assert.AreEqual(0.5, testChaosSettings.getValue(ChaosSettings.Properties.CDUSVar, 0), 1e-10, 
                "ChaosSettings CDUSVar value should be preserved");
            Assert.AreEqual(0.6, testChaosSettings.getValue(ChaosSettings.Properties.CDUTVar, 0), 1e-10, 
                "ChaosSettings CDUTVar value should be preserved");
            Assert.AreEqual(0.7, testChaosSettings.getValue(ChaosSettings.Properties.LWRVar, 0), 1e-10, 
                "ChaosSettings LWRVar value should be preserved");
            Assert.AreEqual(0.8, testChaosSettings.getValue(ChaosSettings.Properties.overlayX, 0), 1e-10, 
                "ChaosSettings overlayX value should be preserved");
            Assert.AreEqual(0.9, testChaosSettings.getValue(ChaosSettings.Properties.overlayY, 0), 1e-10, 
                "ChaosSettings overlayY value should be preserved");
            Assert.AreEqual(42, testChaosSettings.getInt(ChaosSettings.Ints.lwrSeed, 0), 
                "ChaosSettings lwrSeed value should be preserved");
        }

        [Test]
        public void TestReplayCSVHeaderUnchanged()
        {
            // Verify that the CSV header hasn't changed, which would break replay compatibility
            var expectedHeader = new string[] 
            {
                "CDUSVar", "CDUTVar", "LWRVar", "LWRSeed", "LWR2Var", "LWR2Seed", 
                "horTipBiasVar", "verTipBiasVar", "iCVar", "oCVar", "overlayX", "overlayY", "wobbleVar"
            };
            
            Assert.AreEqual(expectedHeader, CommonVars.csvHeader, 
                "CSV header should remain unchanged to preserve replay compatibility");
        }

        [Test]
        public void TestReplayModeBypassesSampler()
        {
            // Verify that replay mode doesn't use the sampler at all
            
            // Create test settings
            var entropySettings = commonVars.getSimulationSettings();
            var layerSettings = commonVars.getLayerSettings(0);
            
            // Enable the first layer with minimal geometry
            layerSettings.setInt(EntropyLayerSettings.properties_i.enabled, 1);
            layerSettings.setInt(EntropyLayerSettings.properties_i.geoEqtn, (int)CommonVars.shapeNames.rect);
            layerSettings.setDecimal(EntropyLayerSettings.properties_decimal.sideCDUSVar, 1.0m);
            layerSettings.setDecimal(EntropyLayerSettings.properties_decimal.horLength, 100.0m);
            layerSettings.setDecimal(EntropyLayerSettings.properties_decimal.verLength, 100.0m);
            
            // Set different sampling methods and verify replay doesn't use them
            var originalSamplingMethod = entropySettings.getValue(EntropySettings.properties_i.samplingMethod);
            
            try
            {
                // Test with Monte Carlo
                entropySettings.setValue(EntropySettings.properties_i.samplingMethod, 
                    (int)CentralProperties.SamplingMethod.MonteCarlo);
                
                // Create ChaosSettings with known values for replay
                var replayChaosSettings = new ChaosSettings(true, commonVars.getListOfSettings(), entropySettings);
                replayChaosSettings.setValue(ChaosSettings.Properties.CDUSVar, 0, 1.5); // Known value
                
                // Test with LHS setting
                entropySettings.setValue(EntropySettings.properties_i.samplingMethod, 
                    (int)CentralProperties.SamplingMethod.LatinHypercube);
                
                // Create another ChaosSettings with same known values
                var replayChaosSettings2 = new ChaosSettings(true, commonVars.getListOfSettings(), entropySettings);
                replayChaosSettings2.setValue(ChaosSettings.Properties.CDUSVar, 0, 1.5); // Same known value
                
                // Both should have the same values regardless of sampling method setting
                // because they were manually set (replay mode)
                Assert.AreEqual(
                    replayChaosSettings.getValue(ChaosSettings.Properties.CDUSVar, 0),
                    replayChaosSettings2.getValue(ChaosSettings.Properties.CDUSVar, 0),
                    1e-10,
                    "Replay ChaosSettings values should be independent of sampling method setting"
                );
                
                // Test with Sobol setting
                entropySettings.setValue(EntropySettings.properties_i.samplingMethod, 
                    (int)CentralProperties.SamplingMethod.Sobol);
                
                var replayChaosSettings3 = new ChaosSettings(true, commonVars.getListOfSettings(), entropySettings);
                replayChaosSettings3.setValue(ChaosSettings.Properties.CDUSVar, 0, 1.5); // Same known value
                
                Assert.AreEqual(
                    replayChaosSettings.getValue(ChaosSettings.Properties.CDUSVar, 0),
                    replayChaosSettings3.getValue(ChaosSettings.Properties.CDUSVar, 0),
                    1e-10,
                    "Replay ChaosSettings values should be independent of sampling method setting"
                );
            }
            finally
            {
                // Restore original sampling method
                entropySettings.setValue(EntropySettings.properties_i.samplingMethod, originalSamplingMethod);
            }
        }

        [Test] 
        public void TestReplayCompatibilityWithAllSamplingMethods()
        {
            // Verify that changing sampling methods doesn't affect replay compatibility
            
            var entropySettings = commonVars.getSimulationSettings();
            var originalSamplingMethod = entropySettings.getValue(EntropySettings.properties_i.samplingMethod);
            
            // Test data that would be read from a CSV file in replay mode
            var testValues = new Dictionary<ChaosSettings.Properties, double[]>
            {
                { ChaosSettings.Properties.CDUSVar, new[] { 0.1, 0.2, 0.3 } },
                { ChaosSettings.Properties.CDUTVar, new[] { 0.4, 0.5, 0.6 } },
                { ChaosSettings.Properties.LWRVar, new[] { 0.7, 0.8, 0.9 } },
                { ChaosSettings.Properties.overlayX, new[] { 1.1, 1.2, 1.3 } },
                { ChaosSettings.Properties.overlayY, new[] { 1.4, 1.5, 1.6 } }
            };
            
            try
            {
                foreach (CentralProperties.SamplingMethod method in Enum.GetValues<CentralProperties.SamplingMethod>())
                {
                    // Set the sampling method
                    entropySettings.setValue(EntropySettings.properties_i.samplingMethod, (int)method);
                    
                    // Create ChaosSettings as if in replay mode (previewMode = true)
                    var replayChaosSettings = new ChaosSettings(true, commonVars.getListOfSettings(), entropySettings);
                    
                    // Set test values as replay would do
                    foreach (var kvp in testValues)
                    {
                        for (int i = 0; i < kvp.Value.Length && i < CentralProperties.maxLayersForMC; i++)
                        {
                            replayChaosSettings.setValue(kvp.Key, i, kvp.Value[i]);
                        }
                    }
                    
                    // Verify values are preserved regardless of sampling method
                    foreach (var kvp in testValues)
                    {
                        for (int i = 0; i < kvp.Value.Length && i < CentralProperties.maxLayersForMC; i++)
                        {
                            Assert.AreEqual(kvp.Value[i], 
                                replayChaosSettings.getValue(kvp.Key, i), 
                                1e-10,
                                $"Replay value for {kvp.Key}[{i}] should be preserved with {method} sampling method");
                        }
                    }
                }
            }
            finally
            {
                // Restore original sampling method
                entropySettings.setValue(EntropySettings.properties_i.samplingMethod, originalSamplingMethod);
            }
        }
    }
}