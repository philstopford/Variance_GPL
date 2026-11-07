using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Variance;

namespace Tests
{
    [TestClass]
    public class SobolSamplingTests
    {
        [TestMethod]
        public void TestSobolSequenceGeneration()
        {
            // Test basic Sobol sequence generation
            int dimensions = 5;
            int sampleCount = 1000;
            
            double[,] sobolSamples = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
            
            // Verify dimensions
            Assert.AreEqual(dimensions, sobolSamples.GetLength(0));
            Assert.AreEqual(sampleCount, sobolSamples.GetLength(1));
            
            // Check that values are properly distributed (should be roughly Gaussian)
            for (int dim = 0; dim < dimensions; dim++)
            {
                double[] dimValues = new double[sampleCount];
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    dimValues[sample] = sobolSamples[dim, sample];
                }
                
                double mean = dimValues.Average();
                double variance = dimValues.Select(x => Math.Pow(x - mean, 2)).Average();
                
                // Gaussian distribution should have mean ~0 and variance ~1
                Assert.IsTrue(Math.Abs(mean) < 0.2, $"Mean {mean} too far from 0 for dimension {dim}");
                Assert.IsTrue(Math.Abs(variance - 1.0) < 0.3, $"Variance {variance} too far from 1 for dimension {dim}");
            }
        }

        [TestMethod]
        public void TestSobolSpaceFillingProperties()
        {
            // Test that Sobol sequences provide good space-filling properties
            int dimensions = 3;
            int sampleCount = 512; // Power of 2 for better Sobol properties
            
            double[,] sobolSamples = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
            
            // Convert to uniform [0,1] for space-filling analysis
            double[,] uniformSamples = new double[dimensions, sampleCount];
            for (int dim = 0; dim < dimensions; dim++)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    // Convert Gaussian to uniform using standard normal CDF approximation
                    double gaussian = sobolSamples[dim, sample];
                    uniformSamples[dim, sample] = 0.5 * (1 + Math.Sign(gaussian) * Math.Sqrt(1 - Math.Exp(-2 * gaussian * gaussian / Math.PI)));
                }
            }
            
            // Test uniformity by checking distribution in bins
            int bins = 8;
            int[,] binCounts = new int[dimensions, bins];
            
            for (int dim = 0; dim < dimensions; dim++)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    int bin = Math.Min((int)(uniformSamples[dim, sample] * bins), bins - 1);
                    binCounts[dim, bin]++;
                }
            }
            
            // Check that each bin has roughly equal representation
            double expectedPerBin = (double)sampleCount / bins;
            for (int dim = 0; dim < dimensions; dim++)
            {
                for (int bin = 0; bin < bins; bin++)
                {
                    double ratio = binCounts[dim, bin] / expectedPerBin;
                    Assert.IsTrue(ratio > 0.7 && ratio < 1.3, 
                        $"Bin {bin} in dimension {dim} has poor uniformity: {binCounts[dim, bin]} vs expected {expectedPerBin}");
                }
            }
        }

        [TestMethod]
        public void TestSobolConvergenceProperties()
        {
            // Test convergence of Sobol sequences for a known function
            // Using 6D hypersphere volume estimation: V = π^3/6 * r^6 for unit sphere
            double expectedVolume = Math.Pow(Math.PI, 3) / 6.0; // ≈ 5.1677
            
            int dimensions = 6;
            int[] sampleSizes = { 100, 500, 1000, 2000 };
            double[] errors = new double[sampleSizes.Length];
            
            for (int i = 0; i < sampleSizes.Length; i++)
            {
                int sampleCount = sampleSizes[i];
                double[,] sobolSamples = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
                
                // Estimate hypersphere volume using indicator function
                int insideCount = 0;
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    double sumSquares = 0;
                    for (int dim = 0; dim < dimensions; dim++)
                    {
                        double value = sobolSamples[dim, sample];
                        sumSquares += value * value;
                    }
                    
                    if (sumSquares <= 1.0)
                    {
                        insideCount++;
                    }
                }
                
                // Volume = (hypercube volume) * (fraction inside sphere)
                double hypercubeVolume = Math.Pow(2.0, dimensions); // [-1,1]^6
                double estimatedVolume = hypercubeVolume * insideCount / sampleCount;
                errors[i] = Math.Abs(estimatedVolume - expectedVolume) / expectedVolume;
            }
            
            // Check that error generally decreases with more samples
            // (allowing for some statistical variation)
            Assert.IsTrue(errors[3] < errors[0] * 2, 
                "Sobol convergence should improve with sample size");
            
            // Final error should be reasonable
            Assert.IsTrue(errors[3] < 0.3, 
                $"Final error {errors[3]:F3} too large for Sobol sequence");
        }

        [TestMethod]
        public void TestSobolDeterministicBehavior()
        {
            // Test that Sobol sequences are deterministic (same input = same output)
            int dimensions = 4;
            int sampleCount = 100;
            
            double[,] samples1 = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
            double[,] samples2 = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
            
            // Should be identical
            for (int dim = 0; dim < dimensions; dim++)
            {
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    Assert.AreEqual(samples1[dim, sample], samples2[dim, sample], 1e-10,
                        $"Sobol sequences should be deterministic at [{dim},{sample}]");
                }
            }
        }

        [TestMethod]
        public void TestSobolDimensionLimit()
        {
            // Test that Sobol sequences respect dimension limits
            int maxDimensions = 128;
            int sampleCount = 100;
            
            // Should work at the limit
            double[,] samples = QuasiRandomSampler.GenerateSobolSamples(maxDimensions, sampleCount);
            Assert.AreEqual(maxDimensions, samples.GetLength(0));
            
            // Should throw exception beyond the limit
            Assert.ThrowsException<ArgumentException>(() =>
                QuasiRandomSampler.GenerateSobolSamples(maxDimensions + 1, sampleCount));
        }

        [TestMethod]
        public void TestUnifiedSamplingInterface()
        {
            // Test the unified sampling interface
            int dimensions = 3;
            int sampleCount = 100;
            
            // Create a dummy settings object (won't be used for Sobol)
            EntropySettings settings = new EntropySettings();
            
            // Test each sampling method
            double[,] mcSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.MonteCarlo, dimensions, sampleCount, settings);
            double[,] lhsSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.LatinHypercube, dimensions, sampleCount, settings);
            double[,] sobolSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.Sobol, dimensions, sampleCount, settings);
            
            // All should have correct dimensions
            Assert.AreEqual(dimensions, mcSamples.GetLength(0));
            Assert.AreEqual(sampleCount, mcSamples.GetLength(1));
            Assert.AreEqual(dimensions, lhsSamples.GetLength(0));
            Assert.AreEqual(sampleCount, lhsSamples.GetLength(1));
            Assert.AreEqual(dimensions, sobolSamples.GetLength(0));
            Assert.AreEqual(sampleCount, sobolSamples.GetLength(1));
            
            // Sobol should be deterministic, MC should not
            double[,] sobolSamples2 = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.Sobol, dimensions, sampleCount, settings);
            
            bool sobolIdentical = true;
            for (int dim = 0; dim < dimensions && sobolIdentical; dim++)
            {
                for (int sample = 0; sample < sampleCount && sobolIdentical; sample++)
                {
                    if (Math.Abs(sobolSamples[dim, sample] - sobolSamples2[dim, sample]) > 1e-10)
                        sobolIdentical = false;
                }
            }
            Assert.IsTrue(sobolIdentical, "Sobol sequences should be deterministic");
        }

        [TestMethod]
        public void TestSobolHighDimensional()
        {
            // Test Sobol sequences in very high dimensions (80+ dimensions)
            int dimensions = 80;
            int sampleCount = 500;
            
            double[,] sobolSamples = QuasiRandomSampler.GenerateSobolSamples(dimensions, sampleCount);
            
            // Verify basic properties
            Assert.AreEqual(dimensions, sobolSamples.GetLength(0));
            Assert.AreEqual(sampleCount, sobolSamples.GetLength(1));
            
            // Check statistical properties for high dimensions
            for (int dim = 0; dim < Math.Min(dimensions, 10); dim++) // Test first 10 dimensions
            {
                double[] dimValues = new double[sampleCount];
                for (int sample = 0; sample < sampleCount; sample++)
                {
                    dimValues[sample] = sobolSamples[dim, sample];
                    // Values should be finite
                    Assert.IsFalse(double.IsNaN(dimValues[sample]), $"NaN value found in dimension {dim}");
                    Assert.IsFalse(double.IsInfinity(dimValues[sample]), $"Infinity value found in dimension {dim}");
                }
                
                double mean = dimValues.Average();
                double variance = dimValues.Select(x => Math.Pow(x - mean, 2)).Average();
                
                // Gaussian distribution should have mean ~0 and variance ~1 (with more tolerance for high dimensions)
                Assert.IsTrue(Math.Abs(mean) < 0.3, $"Mean {mean} too far from 0 for dimension {dim}");
                Assert.IsTrue(Math.Abs(variance - 1.0) < 0.5, $"Variance {variance} too far from 1 for dimension {dim}");
            }
        }

        [TestMethod]
        public void TestSobolMaxDimensions()
        {
            // Test that we can actually use all 128 dimensions
            int maxDimensions = 128;
            int sampleCount = 100;
            
            double[,] samples = QuasiRandomSampler.GenerateSobolSamples(maxDimensions, sampleCount);
            
            // Verify dimensions
            Assert.AreEqual(maxDimensions, samples.GetLength(0));
            Assert.AreEqual(sampleCount, samples.GetLength(1));
            
            // Check that all values are valid (no NaN/Infinity)
            for (int dim = 0; dim < maxDimensions; dim++)
            {
                for (int sample = 0; sample < Math.Min(sampleCount, 10); sample++) // Check first 10 samples
                {
                    double value = samples[dim, sample];
                    Assert.IsFalse(double.IsNaN(value), $"NaN value found in dimension {dim}, sample {sample}");
                    Assert.IsFalse(double.IsInfinity(value), $"Infinity value found in dimension {dim}, sample {sample}");
                }
            }
        }
    }
}