using System;
using Xunit;
using Variance;
using System.Collections.Generic;

namespace VarianceTests
{
    public class PASearchSamplingTests
    {
        [Fact]
        public void PASearch_Mode_Forces_MonteCarlo_Sampling_With_LHS_Setting()
        {
            // Arrange
            var commonVars = TestUtils.GetStandardCommonVars();
            
            // Set sampling method to LHS
            commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.samplingMethod, 
                                                       (int)CentralProperties.SamplingMethod.LatinHypercube);
            
            // Create sampler with PASearch enabled
            var sampler = new Sampler_Geo(100, previewMode: false, doPASearch: true, ref commonVars);
            
            // Track if quasi-random sampling was used by checking if getSample returns live samples
            sampler.sample(useThreads: false, useLHS: false);
            
            // Act & Assert
            // In PASearch mode, getSample should always generate live Monte Carlo samples
            var sample1 = sampler.getSample(0);
            var sample2 = sampler.getSample(0); // Same index should give different results in PASearch
            
            // The samples should be different because PASearch generates live samples each time
            Assert.NotEqual(sample1, sample2);
            
            // Verify that the samples are actually different by checking some property values
            bool foundDifference = false;
            for (int layer = 0; layer < CentralProperties.maxLayersForMC && !foundDifference; layer++)
            {
                if (Math.Abs(sample1.getValue(ChaosSettings.Properties.overlayX, layer) - 
                           sample2.getValue(ChaosSettings.Properties.overlayX, layer)) > 1e-10)
                {
                    foundDifference = true;
                }
            }
            
            Assert.True(foundDifference, "PASearch mode should generate different samples each time, not use pre-generated ones");
        }

        [Fact]
        public void PASearch_Mode_Forces_MonteCarlo_Sampling_With_Sobol_Setting()
        {
            // Arrange
            var commonVars = TestUtils.GetStandardCommonVars();
            
            // Set sampling method to Sobol
            commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.samplingMethod, 
                                                       (int)CentralProperties.SamplingMethod.Sobol);
            
            // Create sampler with PASearch enabled
            var sampler = new Sampler_Geo(100, previewMode: false, doPASearch: true, ref commonVars);
            
            // Track if quasi-random sampling was used by checking if getSample returns live samples
            sampler.sample(useThreads: false, useLHS: false);
            
            // Act & Assert
            // In PASearch mode, getSample should always generate live Monte Carlo samples
            var sample1 = sampler.getSample(0);
            var sample2 = sampler.getSample(0); // Same index should give different results in PASearch
            
            // The samples should be different because PASearch generates live samples each time
            Assert.NotEqual(sample1, sample2);
            
            // Verify that the samples are actually different by checking some property values
            bool foundDifference = false;
            for (int layer = 0; layer < CentralProperties.maxLayersForMC && !foundDifference; layer++)
            {
                if (Math.Abs(sample1.getValue(ChaosSettings.Properties.overlayX, layer) - 
                           sample2.getValue(ChaosSettings.Properties.overlayX, layer)) > 1e-10)
                {
                    foundDifference = true;
                }
            }
            
            Assert.True(foundDifference, "PASearch mode should generate different samples each time, not use pre-generated ones");
        }

        [Fact]
        public void Non_PASearch_Mode_Uses_Configured_Sampling_Method_LHS()
        {
            // Arrange
            var commonVars = TestUtils.GetStandardCommonVars();
            
            // Set sampling method to LHS
            commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.samplingMethod, 
                                                       (int)CentralProperties.SamplingMethod.LatinHypercube);
            
            // Create sampler WITHOUT PASearch
            var sampler = new Sampler_Geo(100, previewMode: false, doPASearch: false, ref commonVars);
            
            // Sample generation
            sampler.sample(useThreads: false, useLHS: false);
            
            // Act & Assert
            // In non-PASearch mode, getSample should return the same pre-generated sample
            var sample1 = sampler.getSample(0);
            var sample2 = sampler.getSample(0); // Same index should give same results
            
            // The samples should be identical because they're pre-generated
            Assert.Equal(sample1, sample2);
            
            // Verify that specific properties are identical
            for (int layer = 0; layer < Math.Min(CentralProperties.maxLayersForMC, 3); layer++)
            {
                Assert.Equal(sample1.getValue(ChaosSettings.Properties.overlayX, layer),
                           sample2.getValue(ChaosSettings.Properties.overlayX, layer), 10);
                Assert.Equal(sample1.getValue(ChaosSettings.Properties.overlayY, layer),
                           sample2.getValue(ChaosSettings.Properties.overlayY, layer), 10);
            }
        }

        [Fact]
        public void Non_PASearch_Mode_Uses_Configured_Sampling_Method_Sobol()
        {
            // Arrange
            var commonVars = TestUtils.GetStandardCommonVars();
            
            // Set sampling method to Sobol
            commonVars.getSimulationSettings().setValue(EntropySettings.properties_i.samplingMethod, 
                                                       (int)CentralProperties.SamplingMethod.Sobol);
            
            // Create sampler WITHOUT PASearch
            var sampler = new Sampler_Geo(100, previewMode: false, doPASearch: false, ref commonVars);
            
            // Sample generation
            sampler.sample(useThreads: false, useLHS: false);
            
            // Act & Assert
            // In non-PASearch mode, getSample should return the same pre-generated sample
            var sample1 = sampler.getSample(0);
            var sample2 = sampler.getSample(0); // Same index should give same results
            
            // The samples should be identical because they're pre-generated
            Assert.Equal(sample1, sample2);
            
            // Verify that specific properties are identical
            for (int layer = 0; layer < Math.Min(CentralProperties.maxLayersForMC, 3); layer++)
            {
                Assert.Equal(sample1.getValue(ChaosSettings.Properties.overlayX, layer),
                           sample2.getValue(ChaosSettings.Properties.overlayX, layer), 10);
                Assert.Equal(sample1.getValue(ChaosSettings.Properties.overlayY, layer),
                           sample2.getValue(ChaosSettings.Properties.overlayY, layer), 10);
            }
        }

        [Fact]
        public void Sampler_Implant_Does_Not_Support_PASearch_Mode()
        {
            // Arrange
            var entropySettings = TestUtils.GetStandardEntropySettings();
            
            // Set sampling method to LHS
            entropySettings.setValue(EntropySettings.properties_i.samplingMethod, 
                                   (int)CentralProperties.SamplingMethod.LatinHypercube);
            
            // Create implant sampler (does not have PASearch parameter)
            var sampler = new Sampler_Implant(100, previewMode: false, entropySettings);
            
            // Sample generation
            sampler.sample(useThreads: false);
            
            // Act & Assert
            // Implant sampler should always return the same pre-generated sample
            var sample1 = sampler.getSample(0);
            var sample2 = sampler.getSample(0); // Same index should give same results
            
            // The samples should be identical because they're pre-generated
            Assert.Equal(sample1, sample2);
        }
    }
}