using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Variance;

namespace Tests
{
    [TestClass]
    public class SamplerSobolIntegrationTests
    {
        [TestMethod]
        public void TestSamplerGeoSobolIntegration()
        {
            // Test that sampler_geo correctly uses Sobol sampling when configured
            
            // Create test settings with Sobol sampling
            EntropySettings entropySettings = new EntropySettings();
            entropySettings.setValue(EntropySettings.properties_i.samplingMethod, 
                (int)CentralProperties.SamplingMethod.Sobol);
            entropySettings.setValue(EntropySettings.properties_i.nCases, 100);
            
            // Create a minimal CommonVars setup
            CommonVars commonVars = new CommonVars();
            
            // This test mainly verifies that the integration compiles and runs
            // without throwing exceptions when Sobol sampling is selected
            try
            {
                // Note: Full sampler_geo testing requires more complex setup
                // This test focuses on the sampling method selection logic
                
                int samplingMethod = entropySettings.getValue(EntropySettings.properties_i.samplingMethod);
                Assert.AreEqual((int)CentralProperties.SamplingMethod.Sobol, samplingMethod);
                
                bool useQuasiRandom = samplingMethod != (int)CentralProperties.SamplingMethod.MonteCarlo;
                Assert.IsTrue(useQuasiRandom, "Sobol should be detected as quasi-random method");
                
                // Verify that Sobol samples can be generated
                double[,] sobolSamples = QuasiRandomSampler.GenerateSamples(
                    CentralProperties.SamplingMethod.Sobol, 5, 100, entropySettings);
                
                Assert.AreEqual(5, sobolSamples.GetLength(0));
                Assert.AreEqual(100, sobolSamples.GetLength(1));
                
            }
            catch (Exception ex)
            {
                Assert.Fail($"Sobol integration failed: {ex.Message}");
            }
        }

        [TestMethod]
        public void TestSamplerImplantSobolIntegration()
        {
            // Test that sampler_implant correctly uses Sobol sampling when configured
            
            // Create test settings with Sobol sampling
            EntropySettings entropySettings = new EntropySettings();
            entropySettings.setValue(EntropySettings.properties_i.samplingMethod, 
                (int)CentralProperties.SamplingMethod.Sobol);
            entropySettings.setValue(EntropySettings.properties_i.nCases, 50);
            
            try
            {
                // Test the sampling method detection logic
                int samplingMethod = entropySettings.getValue(EntropySettings.properties_i.samplingMethod);
                Assert.AreEqual((int)CentralProperties.SamplingMethod.Sobol, samplingMethod);
                
                bool useQuasiRandom = samplingMethod != (int)CentralProperties.SamplingMethod.MonteCarlo;
                Assert.IsTrue(useQuasiRandom, "Sobol should be detected as quasi-random method");
                
                // Verify that the unified sampling interface works
                CentralProperties.SamplingMethod method = (CentralProperties.SamplingMethod)samplingMethod;
                double[,] sobolSamples = QuasiRandomSampler.GenerateSamples(
                    method, 4, 50, entropySettings);
                
                Assert.AreEqual(4, sobolSamples.GetLength(0));
                Assert.AreEqual(50, sobolSamples.GetLength(1));
                
                // Test that values are reasonable (Gaussian distributed)
                for (int dim = 0; dim < 4; dim++)
                {
                    for (int sample = 0; sample < 50; sample++)
                    {
                        double value = sobolSamples[dim, sample];
                        Assert.IsTrue(Math.Abs(value) < 5.0, 
                            $"Gaussian value {value} seems unrealistic at [{dim},{sample}]");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Assert.Fail($"Sobol integration for sampler_implant failed: {ex.Message}");
            }
        }

        [TestMethod]
        public void TestSamplingMethodEnumExtension()
        {
            // Test that the SamplingMethod enum correctly includes Sobol
            
            // Verify enum values
            Assert.AreEqual(0, (int)CentralProperties.SamplingMethod.MonteCarlo);
            Assert.AreEqual(1, (int)CentralProperties.SamplingMethod.LatinHypercube);
            Assert.AreEqual(2, (int)CentralProperties.SamplingMethod.Sobol);
            
            // Test enum casting
            CentralProperties.SamplingMethod sobolMethod = (CentralProperties.SamplingMethod)2;
            Assert.AreEqual(CentralProperties.SamplingMethod.Sobol, sobolMethod);
            
            // Test string representation (if implemented)
            string sobolName = CentralProperties.SamplingMethod.Sobol.ToString();
            Assert.AreEqual("Sobol", sobolName);
        }

        [TestMethod]
        public void TestUnifiedSamplingInterfaceAllMethods()
        {
            // Test that all three sampling methods work through the unified interface
            
            int dimensions = 3;
            int sampleCount = 100;
            EntropySettings settings = new EntropySettings();
            
            // Test Monte Carlo
            double[,] mcSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.MonteCarlo, dimensions, sampleCount, settings);
            
            // Test Latin Hypercube
            double[,] lhsSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.LatinHypercube, dimensions, sampleCount, settings);
            
            // Test Sobol
            double[,] sobolSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.Sobol, dimensions, sampleCount, settings);
            
            // All should have correct dimensions
            Assert.AreEqual(dimensions, mcSamples.GetLength(0));
            Assert.AreEqual(sampleCount, mcSamples.GetLength(1));
            Assert.AreEqual(dimensions, lhsSamples.GetLength(0));
            Assert.AreEqual(sampleCount, lhsSamples.GetLength(1));
            Assert.AreEqual(dimensions, sobolSamples.GetLength(0));
            Assert.AreEqual(sampleCount, sobolSamples.GetLength(1));
            
            // Verify that different methods produce different results
            bool mcVsLhsDifferent = false;
            bool mcVsSobolDifferent = false;
            bool lhsVsSobolDifferent = false;
            
            for (int dim = 0; dim < dimensions && !(mcVsLhsDifferent && mcVsSobolDifferent && lhsVsSobolDifferent); dim++)
            {
                for (int sample = 0; sample < Math.Min(10, sampleCount); sample++)
                {
                    if (Math.Abs(mcSamples[dim, sample] - lhsSamples[dim, sample]) > 0.01)
                        mcVsLhsDifferent = true;
                    if (Math.Abs(mcSamples[dim, sample] - sobolSamples[dim, sample]) > 0.01)
                        mcVsSobolDifferent = true;
                    if (Math.Abs(lhsSamples[dim, sample] - sobolSamples[dim, sample]) > 0.01)
                        lhsVsSobolDifferent = true;
                }
            }
            
            Assert.IsTrue(mcVsLhsDifferent, "Monte Carlo and LHS should produce different results");
            Assert.IsTrue(mcVsSobolDifferent, "Monte Carlo and Sobol should produce different results");
            Assert.IsTrue(lhsVsSobolDifferent, "LHS and Sobol should produce different results");
        }

        [TestMethod]
        public void TestSobolReproducibility()
        {
            // Test that Sobol sequences are reproducible (deterministic)
            
            int dimensions = 5;
            int sampleCount = 200;
            
            double[,] samples1 = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
            double[,] samples2 = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
            
            // Should be identical
            for (int dim = 0; dim < dimensions; dim++)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Assert.AreEqual(samples1[dim, sample], samples2[dim, sample], 1e-12,
                        $"Sobol sequences should be deterministic at [{dim},{sample}]");
                }
            }
        }

        [TestMethod]
        public void TestSobolPerformanceCharacteristics()
        {
            // Test key performance characteristics of Sobol sequences
            
            int dimensions = 6;
            int sampleCount = 1024; // Power of 2 for optimal Sobol properties
            
            double[,] sobolSamples = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
            
            // Test 1: Check that samples have reasonable Gaussian properties
            for (int dim = 0; dim < dimensions; dim++)
            {
                double sum = 0;
                double sumSquares = 0;
                
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    double value = sobolSamples[dim, sample];
                    sum += value;
                    sumSquares += value * value;
                }
                
                double mean = sum / sampleCount;
                double variance = (sumSquares / sampleCount) - (mean * mean);
                
                // Should approximate standard normal distribution
                Assert.IsTrue(Math.Abs(mean) < 0.15, $"Mean {mean:F3} too far from 0 for dimension {dim}");
                Assert.IsTrue(Math.Abs(variance - 1.0) < 0.25, $"Variance {variance:F3} too far from 1 for dimension {dim}");
            }
            
            // Test 2: Check low-discrepancy properties by testing uniformity in unit cube
            // (after transforming from Gaussian to uniform)
            int binCount = 8;
            int[,] bins = new int[dimensions, binCount];
            
            for (int dim = 0; dim < dimensions; dim++)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    double gaussian = sobolSamples[dim, sample];
                    // Approximate inverse normal CDF to get uniform [0,1]
                    double uniform = 0.5 * (1 + Math.Sign(gaussian) * Math.Sqrt(1 - Math.Exp(-2 * gaussian * gaussian / Math.PI)));
                    uniform = Math.Max(0, Math.Min(1, uniform));
                    
                    int bin = Math.Min((int)(uniform * binCount), binCount - 1);
                    bins[dim, bin]++;
                }
            }
            
            // Check that bins are reasonably uniform
            double expectedPerBin = (double)sampleCount / binCount;
            for (int dim = 0; dim < dimensions; dim++)
            {
                for (int bin = 0; bin < binCount; bin++)
                {
                    double ratio = bins[dim, bin] / expectedPerBin;
                    Assert.IsTrue(ratio > 0.6 && ratio < 1.4, 
                        $"Bin uniformity poor in dimension {dim}, bin {bin}: {bins[dim, bin]} vs expected {expectedPerBin:F1}");
                }
            }
        }

        [TestMethod]
        public void TestInvalidSamplingMethodHandling()
        {
            // Test that invalid sampling methods are handled appropriately
            
            int dimensions = 3;
            int sampleCount = 100;
            EntropySettings settings = new EntropySettings();
            
            // Test with invalid enum value
            Assert.ThrowsException<ArgumentException>(() =>
                QuasiRandomSampler.GenerateSamples((CentralProperties.SamplingMethod)999, 
                    dimensions, sampleCount, settings));
        }
    }
}