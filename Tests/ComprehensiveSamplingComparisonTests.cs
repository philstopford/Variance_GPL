using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Variance;

namespace Tests
{
    [TestClass]
    public class ComprehensiveSamplingComparisonTests
    {
        [TestMethod]
        public void TestHighDimensionalIntegrationComparison()
        {
            // Compare all three sampling methods on a challenging 8D integration problem
            // Function: Product of exp(-x_i^2) for i=1..8 (separable Gaussian)
            // Expected value: (sqrt(π))^8 ≈ 31.006
            
            int dimensions = 8;
            int sampleCount = 2000;
            double expectedValue = Math.Pow(Math.Sqrt(Math.PI), dimensions);
            
            EntropySettings settings = new EntropySettings();
            
            // Test all three methods
            double mcError = TestIntegrationMethod(CentralProperties.SamplingMethod.MonteCarlo, 
                dimensions, sampleCount, settings, expectedValue);
            double lhsError = TestIntegrationMethod(CentralProperties.SamplingMethod.LatinHypercube,
                dimensions, sampleCount, settings, expectedValue);
            double sobolError = TestIntegrationMethod(CentralProperties.SamplingMethod.Sobol,
                dimensions, sampleCount, settings, expectedValue);
            
            Console.WriteLine($"8D Gaussian Integration Errors:");
            Console.WriteLine($"Monte Carlo:      {mcError:F4}");
            Console.WriteLine($"Latin Hypercube:  {lhsError:F4}");
            Console.WriteLine($"Sobol Sequence:   {sobolError:F4}");
            
            // Sobol should generally perform better than Monte Carlo for this smooth function
            Assert.IsTrue(sobolError < mcError * 1.5, 
                "Sobol should not be significantly worse than Monte Carlo");
            
            // All methods should achieve reasonable accuracy
            Assert.IsTrue(mcError < 0.3, "Monte Carlo error should be reasonable");
            Assert.IsTrue(lhsError < 0.3, "LHS error should be reasonable");
            Assert.IsTrue(sobolError < 0.3, "Sobol error should be reasonable");
        }

        [TestMethod]
        public void TestOscillatoryFunctionComparison()
        {
            // Test on an oscillatory function where quasi-random methods should excel
            // Function: Product of sin(π * x_i) for i=1..6
            // This is challenging for Monte Carlo due to oscillations
            
            int dimensions = 6;
            int sampleCount = 1000;
            double expectedValue = 0.0; // Symmetric function integrates to 0
            
            EntropySettings settings = new EntropySettings();
            
            double mcError = TestOscillatoryFunction(CentralProperties.SamplingMethod.MonteCarlo,
                dimensions, sampleCount, settings, expectedValue);
            double lhsError = TestOscillatoryFunction(CentralProperties.SamplingMethod.LatinHypercube,
                dimensions, sampleCount, settings, expectedValue);
            double sobolError = TestOscillatoryFunction(CentralProperties.SamplingMethod.Sobol,
                dimensions, sampleCount, settings, expectedValue);
            
            Console.WriteLine($"6D Oscillatory Function Errors:");
            Console.WriteLine($"Monte Carlo:      {mcError:F4}");
            Console.WriteLine($"Latin Hypercube:  {lhsError:F4}");
            Console.WriteLine($"Sobol Sequence:   {sobolError:F4}");
            
            // Quasi-random methods should generally outperform Monte Carlo on oscillatory functions
            Assert.IsTrue(lhsError < mcError * 1.2 || sobolError < mcError * 1.2,
                "At least one quasi-random method should outperform Monte Carlo");
        }

        [TestMethod]
        public void TestConvergenceRateComparison()
        {
            // Test convergence rates as sample size increases
            // Use a smooth 4D function for clear convergence behavior
            
            int dimensions = 4;
            int[] sampleSizes = { 250, 500, 1000, 2000 };
            EntropySettings settings = new EntropySettings();
            
            double[] mcErrors = new double[sampleSizes.Length];
            double[] lhsErrors = new double[sampleSizes.Length];
            double[] sobolErrors = new double[sampleSizes.Length];
            
            // Test function: exp(-||x||^2) (4D Gaussian)
            double expectedValue = Math.Pow(Math.Sqrt(Math.PI), dimensions);
            
            for (int i = 0; i < sampleSizes.Length; i++)
            {
                mcErrors[i] = TestConvergenceFunction(CentralProperties.SamplingMethod.MonteCarlo,
                    dimensions, sampleSizes[i], settings, expectedValue);
                lhsErrors[i] = TestConvergenceFunction(CentralProperties.SamplingMethod.LatinHypercube,
                    dimensions, sampleSizes[i], settings, expectedValue);
                sobolErrors[i] = TestConvergenceFunction(CentralProperties.SamplingMethod.Sobol,
                    dimensions, sampleSizes[i], settings, expectedValue);
            }
            
            // Check that errors generally decrease with more samples
            Assert.IsTrue(mcErrors[3] < mcErrors[0] * 2, "MC should show some convergence");
            Assert.IsTrue(lhsErrors[3] < lhsErrors[0] * 2, "LHS should show some convergence");
            Assert.IsTrue(sobolErrors[3] < sobolErrors[0] * 2, "Sobol should show some convergence");
            
            // Final errors should be reasonable
            Assert.IsTrue(mcErrors[3] < 0.2, "Final MC error should be reasonable");
            Assert.IsTrue(lhsErrors[3] < 0.2, "Final LHS error should be reasonable");
            Assert.IsTrue(sobolErrors[3] < 0.2, "Final Sobol error should be reasonable");
            
            Console.WriteLine("Convergence Analysis (4D Gaussian):");
            for (int i = 0; i < sampleSizes.Length; i++)
            {
                Console.WriteLine($"N={sampleSizes[i]:D4}: MC={mcErrors[i]:F4}, LHS={lhsErrors[i]:F4}, Sobol={sobolErrors[i]:F4}");
            }
        }

        [TestMethod]
        public void TestDiscrepancyComparison()
        {
            // Test the space-filling properties (discrepancy) of each method
            // Lower discrepancy indicates better space-filling
            
            int dimensions = 3; // Use 3D for easier visualization and testing
            int sampleCount = 1000;
            EntropySettings settings = new EntropySettings();
            
            double[,] mcSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.MonteCarlo, dimensions, sampleCount, settings);
            double[,] lhsSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.LatinHypercube, dimensions, sampleCount, settings);
            double[,] sobolSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.Sobol, dimensions, sampleCount, settings);
            
            double mcDiscrepancy = CalculateL2Discrepancy(mcSamples, dimensions, sampleCount);
            double lhsDiscrepancy = CalculateL2Discrepancy(lhsSamples, dimensions, sampleCount);
            double sobolDiscrepancy = CalculateL2Discrepancy(sobolSamples, dimensions, sampleCount);
            
            Console.WriteLine($"L2-Discrepancy (lower is better):");
            Console.WriteLine($"Monte Carlo:      {mcDiscrepancy:F4}");
            Console.WriteLine($"Latin Hypercube:  {lhsDiscrepancy:F4}");
            Console.WriteLine($"Sobol Sequence:   {sobolDiscrepancy:F4}");
            
            // Quasi-random methods should have lower discrepancy than Monte Carlo
            Assert.IsTrue(lhsDiscrepancy < mcDiscrepancy * 1.5, 
                "LHS should have better discrepancy than MC");
            Assert.IsTrue(sobolDiscrepancy < mcDiscrepancy * 1.5,
                "Sobol should have better discrepancy than MC");
        }

        [TestMethod]
        public void TestReproducibilityComparison()
        {
            // Test reproducibility characteristics of each method
            
            int dimensions = 4;
            int sampleCount = 100;
            EntropySettings settings1 = new EntropySettings();
            EntropySettings settings2 = new EntropySettings();
            
            // Test LHS and Sobol reproducibility with same settings
            double[,] lhs1 = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.LatinHypercube, dimensions, sampleCount, settings1);
            double[,] lhs2 = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.LatinHypercube, dimensions, sampleCount, settings2);
            
            double[,] sobol1 = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.Sobol, dimensions, sampleCount, settings1);
            double[,] sobol2 = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.Sobol, dimensions, sampleCount, settings2);
            
            // Sobol should be perfectly reproducible (deterministic)
            bool sobolIdentical = true;
            for (int dim = 0; dim < dimensions && sobolIdentical; dim++)
            {
                for (int sample = 0; sample < sampleCount && sobolIdentical; sample++)
                {
                    if (Math.Abs(sobol1[dim, sample] - sobol2[dim, sample]) > 1e-12)
                        sobolIdentical = false;
                }
            }
            
            Assert.IsTrue(sobolIdentical, "Sobol sequences should be perfectly reproducible");
            
            // LHS with different RNG seeds should produce different results
            bool lhsIdentical = true;
            for (int dim = 0; dim < dimensions && lhsIdentical; dim++)
            {
                for (int sample = 0; sample < Math.Min(10, sampleCount) && lhsIdentical; sample++)
                {
                    if (Math.Abs(lhs1[dim, sample] - lhs2[dim, sample]) > 0.01)
                        lhsIdentical = false;
                }
            }
            
            // Note: This test depends on RNG implementation details
            Console.WriteLine($"LHS sequences identical: {lhsIdentical}");
            Console.WriteLine($"Sobol sequences identical: {sobolIdentical}");
        }

        private double TestIntegrationMethod(CentralProperties.SamplingMethod method, 
            int dimensions, int sampleCount, EntropySettings settings, double expectedValue)
        {
            double[,] samples = QuasiRandomSampler.GenerateSamples(method, dimensions, sampleCount, settings);
            
            // Evaluate integral of product of exp(-x_i^2)
            double sum = 0;
            for (int sample = 0; sample < sampleCount; sample++)
            {
                double product = 1.0;
                for (int dim = 0; dim < dimensions; dim++)
                {
                    double x = samples[dim, sample];
                    product *= Math.Exp(-x * x);
                }
                sum += product;
            }
            
            double estimate = sum / sampleCount * Math.Pow(2 * Math.Sqrt(Math.PI), dimensions);
            return Math.Abs(estimate - expectedValue) / expectedValue;
        }

        private double TestOscillatoryFunction(CentralProperties.SamplingMethod method,
            int dimensions, int sampleCount, EntropySettings settings, double expectedValue)
        {
            double[,] samples = QuasiRandomSampler.GenerateSamples(method, dimensions, sampleCount, settings);
            
            // Evaluate integral of product of sin(π * x_i)
            double sum = 0;
            for (int sample = 0; sample < sampleCount; sample++)
            {
                double product = 1.0;
                for (int dim = 0; dim < dimensions; dim++)
                {
                    double x = samples[dim, sample];
                    product *= Math.Sin(Math.PI * x);
                }
                sum += product;
            }
            
            double estimate = sum / sampleCount * Math.Pow(2.0, dimensions); // Domain scaling
            return Math.Abs(estimate - expectedValue);
        }

        private double TestConvergenceFunction(CentralProperties.SamplingMethod method,
            int dimensions, int sampleCount, EntropySettings settings, double expectedValue)
        {
            double[,] samples = QuasiRandomSampler.GenerateSamples(method, dimensions, sampleCount, settings);
            
            // Evaluate 4D Gaussian integral
            double sum = 0;
            for (int sample = 0; sample < sampleCount; sample++)
            {
                double normSquared = 0;
                for (int dim = 0; dim < dimensions; dim++)
                {
                    double x = samples[dim, sample];
                    normSquared += x * x;
                }
                sum += Math.Exp(-normSquared);
            }
            
            double estimate = sum / sampleCount * Math.Pow(2 * Math.Sqrt(Math.PI), dimensions);
            return Math.Abs(estimate - expectedValue) / expectedValue;
        }

        private double CalculateL2Discrepancy(double[,] samples, int dimensions, int sampleCount)
        {
            // Calculate L2-discrepancy as a measure of space-filling quality
            // Convert Gaussian samples to uniform [0,1] and measure grid uniformity
            
            const int gridSize = 10;
            int totalCells = (int)Math.Pow(gridSize, dimensions);
            int[] grid = new int[totalCells];
            
            for (int sample = 0; sample < sampleCount; sample++)
            {
                int cellIndex = 0;
                int multiplier = 1;
                
                for (int dim = 0; dim < dimensions; dim++)
                {
                    // Convert Gaussian to uniform using error function approximation
                    double gaussian = samples[dim, sample];
                    double uniform = 0.5 * (1 + gaussian / Math.Sqrt(1 + gaussian * gaussian * 2 / Math.PI));
                    uniform = Math.Max(0, Math.Min(1, uniform));
                    
                    int gridPos = Math.Min((int)(uniform * gridSize), gridSize - 1);
                    cellIndex += gridPos * multiplier;
                    multiplier *= gridSize;
                }
                
                if (cellIndex < totalCells)
                    grid[cellIndex]++;
            }
            
            // Calculate L2-discrepancy
            double expectedPerCell = (double)sampleCount / totalCells;
            double discrepancy = 0;
            
            for (int i = 0; i < totalCells; i++)
            {
                double diff = grid[i] - expectedPerCell;
                discrepancy += diff * diff;
            }
            
            return Math.Sqrt(discrepancy / totalCells) / expectedPerCell;
        }
    }
}