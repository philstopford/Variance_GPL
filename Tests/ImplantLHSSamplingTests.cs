using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Variance;

namespace Variance.PerformanceTests
{
    /// <summary>
    /// Comprehensive tests for sampler_implant Latin Hypercube Sampling implementation
    /// comparing Monte Carlo vs LHS performance across multiple mathematical problems.
    /// </summary>
    [TestClass]
    public class ImplantLHSSamplingTests
    {
        [TestMethod]
        public void TestImplantSamplerBasicFunctionality()
        {
            // Test basic Monte Carlo vs LHS functionality for sampler_implant
            EntropySettings settings = new EntropySettings();
            settings.setValue(EntropySettings.properties_i.nCases, 1000);
            
            // Test Monte Carlo sampling
            settings.setValue(EntropySettings.properties_i.samplingMethod, (int)CentralProperties.SamplingMethod.MonteCarlo);
            Sampler_Implant mcSampler = new Sampler_Implant(1000, false, settings);
            mcSampler.sample(false);
            
            // Test LHS sampling  
            settings.setValue(EntropySettings.properties_i.samplingMethod, (int)CentralProperties.SamplingMethod.LatinHypercube);
            Sampler_Implant lhsSampler = new Sampler_Implant(1000, false, settings);
            lhsSampler.sample(false);
            
            // Verify both samplers generate the expected number of samples
            for (int i = 0; i < 1000; i++)
            {
                var mcSample = mcSampler.getSample(i);
                var lhsSample = lhsSampler.getSample(i);
                
                Assert.IsNotNull(mcSample, "Monte Carlo sample should not be null");
                Assert.IsNotNull(lhsSample, "LHS sample should not be null");
            }
        }

        [TestMethod]
        public void TestImplantLHSSpaceFillingProperties()
        {
            // Test that LHS provides better space-filling properties than Monte Carlo
            EntropySettings settings = new EntropySettings();
            int sampleCount = 1000;
            settings.setValue(EntropySettings.properties_i.nCases, sampleCount);
            
            // Generate Monte Carlo samples
            settings.setValue(EntropySettings.properties_i.samplingMethod, (int)CentralProperties.SamplingMethod.MonteCarlo);
            Sampler_Implant mcSampler = new Sampler_Implant(sampleCount, false, settings);
            mcSampler.sample(false);
            
            // Generate LHS samples
            settings.setValue(EntropySettings.properties_i.samplingMethod, (int)CentralProperties.SamplingMethod.LatinHypercube);
            Sampler_Implant lhsSampler = new Sampler_Implant(sampleCount, false, settings);
            lhsSampler.sample(false);
            
            // Extract sample values for analysis
            double[,] mcSamples = new double[sampleCount, 5]; // 5 dimensions for implant
            double[,] lhsSamples = new double[sampleCount, 5];
            
            for (int i = 0; i < sampleCount; i++)
            {
                var mcSample = mcSampler.getSample(i);
                var lhsSample = lhsSampler.getSample(i);
                
                mcSamples[i, 0] = mcSample.getValue(ChaosSettings_implant.Properties.resistCDVar);
                mcSamples[i, 1] = mcSample.getValue(ChaosSettings_implant.Properties.resistHeightVar);
                mcSamples[i, 2] = mcSample.getValue(ChaosSettings_implant.Properties.resistTopCRRVar);
                mcSamples[i, 3] = mcSample.getValue(ChaosSettings_implant.Properties.tiltVar);
                mcSamples[i, 4] = mcSample.getValue(ChaosSettings_implant.Properties.twistVar);
                
                lhsSamples[i, 0] = lhsSample.getValue(ChaosSettings_implant.Properties.resistCDVar);
                lhsSamples[i, 1] = lhsSample.getValue(ChaosSettings_implant.Properties.resistHeightVar);
                lhsSamples[i, 2] = lhsSample.getValue(ChaosSettings_implant.Properties.resistTopCRRVar);
                lhsSamples[i, 3] = lhsSample.getValue(ChaosSettings_implant.Properties.tiltVar);
                lhsSamples[i, 4] = lhsSample.getValue(ChaosSettings_implant.Properties.twistVar);
            }
            
            // Calculate space-filling metric: average minimum distance between samples
            double mcSpaceFilling = CalculateSpaceFillingMetric(mcSamples);
            double lhsSpaceFilling = CalculateSpaceFillingMetric(lhsSamples);
            
            Console.WriteLine($"Monte Carlo space-filling metric: {mcSpaceFilling:F6}");
            Console.WriteLine($"LHS space-filling metric: {lhsSpaceFilling:F6}");
            Console.WriteLine($"LHS improvement: {(lhsSpaceFilling / mcSpaceFilling - 1) * 100:F1}%");
            
            // LHS should have better (higher) space-filling properties
            Assert.IsTrue(lhsSpaceFilling > mcSpaceFilling, 
                $"LHS space-filling ({lhsSpaceFilling:F6}) should be better than Monte Carlo ({mcSpaceFilling:F6})");
        }

        [TestMethod]
        public void TestImplantLHSConvergenceAnalysis()
        {
            // Test convergence properties for a complex 5D function
            // Function: Sum of exponentials scaled by each dimension
            Func<double[], double> testFunction = (x) => 
            {
                return Math.Exp(-0.5 * (x[0] * x[0] + x[1] * x[1])) + 
                       Math.Exp(-0.3 * (x[2] * x[2] + x[3] * x[3])) + 
                       Math.Exp(-0.7 * x[4] * x[4]);
            };
            
            // True integral value (approximately, for comparison)
            double trueValue = 2 * Math.PI * Math.Sqrt(2) + 2 * Math.PI * Math.Sqrt(10.0/3.0) + Math.Sqrt(2 * Math.PI / 0.7);
            
            EntropySettings settings = new EntropySettings();
            int[] sampleSizes = { 500, 1000, 2000, 4000 };
            
            double[] mcErrors = new double[sampleSizes.Length];
            double[] lhsErrors = new double[sampleSizes.Length];
            
            for (int s = 0; s < sampleSizes.Length; s++)
            {
                int sampleCount = sampleSizes[s];
                settings.setValue(EntropySettings.properties_i.nCases, sampleCount);
                
                // Monte Carlo estimation
                settings.setValue(EntropySettings.properties_i.samplingMethod, (int)CentralProperties.SamplingMethod.MonteCarlo);
                Sampler_Implant mcSampler = new Sampler_Implant(sampleCount, false, settings);
                mcSampler.sample(false);
                
                double mcEstimate = 0.0;
                for (int i = 0; i < sampleCount; i++)
                {
                    var sample = mcSampler.getSample(i);
                    double[] x = {
                        sample.getValue(ChaosSettings_implant.Properties.resistCDVar),
                        sample.getValue(ChaosSettings_implant.Properties.resistHeightVar),
                        sample.getValue(ChaosSettings_implant.Properties.resistTopCRRVar),
                        sample.getValue(ChaosSettings_implant.Properties.tiltVar),
                        sample.getValue(ChaosSettings_implant.Properties.twistVar)
                    };
                    mcEstimate += testFunction(x);
                }
                mcEstimate /= sampleCount;
                mcErrors[s] = Math.Abs(mcEstimate - trueValue);
                
                // LHS estimation
                settings.setValue(EntropySettings.properties_i.samplingMethod, (int)CentralProperties.SamplingMethod.LatinHypercube);
                Sampler_Implant lhsSampler = new Sampler_Implant(sampleCount, false, settings);
                lhsSampler.sample(false);
                
                double lhsEstimate = 0.0;
                for (int i = 0; i < sampleCount; i++)
                {
                    var sample = lhsSampler.getSample(i);
                    double[] x = {
                        sample.getValue(ChaosSettings_implant.Properties.resistCDVar),
                        sample.getValue(ChaosSettings_implant.Properties.resistHeightVar),
                        sample.getValue(ChaosSettings_implant.Properties.resistTopCRRVar),
                        sample.getValue(ChaosSettings_implant.Properties.tiltVar),
                        sample.getValue(ChaosSettings_implant.Properties.twistVar)
                    };
                    lhsEstimate += testFunction(x);
                }
                lhsEstimate /= sampleCount;
                lhsErrors[s] = Math.Abs(lhsEstimate - trueValue);
                
                Console.WriteLine($"Samples: {sampleCount}, MC Error: {mcErrors[s]:F6}, LHS Error: {lhsErrors[s]:F6}, Improvement: {(1 - lhsErrors[s]/mcErrors[s]) * 100:F1}%");
            }
            
            // LHS should generally perform better (lower error) for larger sample sizes
            double avgImprovement = lhsErrors.Zip(mcErrors, (lhs, mc) => (1 - lhs/mc) * 100).Average();
            Console.WriteLine($"Average LHS improvement: {avgImprovement:F1}%");
            
            Assert.IsTrue(avgImprovement > 5.0, 
                $"LHS should show at least 5% average improvement, but showed {avgImprovement:F1}%");
        }

        [TestMethod]
        public void TestImplantSharedLHSClassIntegration()
        {
            // Test that the shared LatinHypercubeSampler class works correctly with sampler_implant
            EntropySettings settings = new EntropySettings();
            
            // Test the shared LHS class directly
            double[,] stratifiedSamples = LatinHypercubeSampler.GenerateStratifiedSamples(5, 100, settings);
            
            Assert.AreEqual(5, stratifiedSamples.GetLength(0), "Should have 5 dimensions");
            Assert.AreEqual(100, stratifiedSamples.GetLength(1), "Should have 100 samples");
            
            // Verify that each dimension has good stratification properties
            for (int dim = 0; dim < 5; dim++)
            {
                double[] dimSamples = new double[100];
                for (int i = 0; i < 100; i++)
                {
                    dimSamples[i] = stratifiedSamples[dim, i];
                }
                
                // Check that samples span reasonable range for normal distribution
                double min = dimSamples.Min();
                double max = dimSamples.Max();
                double mean = dimSamples.Average();
                double variance = dimSamples.Select(x => (x - mean) * (x - mean)).Average();
                
                Console.WriteLine($"Dimension {dim}: Range [{min:F2}, {max:F2}], Mean: {mean:F3}, Std: {Math.Sqrt(variance):F3}");
                
                // Should span reasonable range for normal distribution (roughly -3 to +3 sigma)
                Assert.IsTrue(max - min > 4.0, $"Dimension {dim} should span reasonable range");
                Assert.IsTrue(Math.Abs(mean) < 0.5, $"Dimension {dim} mean should be close to 0");
                Assert.IsTrue(Math.Sqrt(variance) > 0.7 && Math.Sqrt(variance) < 1.3, 
                    $"Dimension {dim} standard deviation should be close to 1");
            }
        }

        private double CalculateSpaceFillingMetric(double[,] samples)
        {
            // Calculate average minimum distance between samples as a space-filling metric
            int n = samples.GetLength(0);
            int d = samples.GetLength(1);
            
            double totalMinDist = 0.0;
            
            for (int i = 0; i < n; i++)
            {
                double minDist = double.MaxValue;
                
                for (int j = 0; j < n; j++)
                {
                    if (i == j) continue;
                    
                    double dist = 0.0;
                    for (int k = 0; k < d; k++)
                    {
                        double diff = samples[i, k] - samples[j, k];
                        dist += diff * diff;
                    }
                    dist = Math.Sqrt(dist);
                    
                    if (dist < minDist)
                    {
                        minDist = dist;
                    }
                }
                
                totalMinDist += minDist;
            }
            
            return totalMinDist / n;
        }
    }
}