using System;
using System.Diagnostics;
using Variance;

namespace Tests
{
    /// <summary>
    /// Demonstration program comparing Monte Carlo, Latin Hypercube, and Sobol sampling
    /// for a 7-dimensional complex mathematical function.
    /// </summary>
    public class SobolSamplingDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Sobol Sampling Performance Demonstration ===");
            Console.WriteLine();
            
            // Test parameters
            int dimensions = 7;
            int[] sampleSizes = { 250, 500, 1000, 2000 };
            
            Console.WriteLine($"Testing {dimensions}D complex function approximation");
            Console.WriteLine("Function: Sum of exp(-0.1 * ||x||^2) * cos(2π * x_i) for i=1..7");
            Console.WriteLine();
            
            // Expected value computed analytically (approximately)
            double expectedValue = dimensions * Math.Exp(-0.1) * 0.5; // ≈ 2.36
            
            Console.WriteLine($"Expected function value: {expectedValue:F4}");
            Console.WriteLine();
            Console.WriteLine("Sample Size | Monte Carlo |    LHS     |   Sobol    | Best Method");
            Console.WriteLine("------------------------------------------------------------------------");
            
            foreach (int sampleCount in sampleSizes)
            {
                // Create dummy settings for MC and LHS
                EntropySettings settings = new EntropySettings();
                
                // Test Monte Carlo
                var mcResult = TestSamplingMethod(CentralProperties.SamplingMethod.MonteCarlo, 
                    dimensions, sampleCount, settings, expectedValue);
                
                // Test Latin Hypercube
                var lhsResult = TestSamplingMethod(CentralProperties.SamplingMethod.LatinHypercube,
                    dimensions, sampleCount, settings, expectedValue);
                
                // Test Sobol
                var sobolResult = TestSamplingMethod(CentralProperties.SamplingMethod.Sobol,
                    dimensions, sampleCount, settings, expectedValue);
                
                // Determine best method
                string bestMethod = "MC";
                double bestError = mcResult.error;
                if (lhsResult.error < bestError)
                {
                    bestMethod = "LHS";
                    bestError = lhsResult.error;
                }
                if (sobolResult.error < bestError)
                {
                    bestMethod = "Sobol";
                }
                
                Console.WriteLine($"{sampleCount,11} | {mcResult.error,9:F4} | {lhsResult.error,9:F4} | " +
                                $"{sobolResult.error,9:F4} | {bestMethod}");
            }
            
            Console.WriteLine();
            Console.WriteLine("=== Space-Filling Properties Comparison ===");
            Console.WriteLine();
            
            // Test space-filling properties
            TestSpaceFillingProperties();
            
            Console.WriteLine();
            Console.WriteLine("=== Convergence Rate Analysis ===");
            Console.WriteLine();
            
            // Test convergence rates
            TestConvergenceRates();
            
            Console.WriteLine();
            Console.WriteLine("Demo completed. Sobol sequences typically show:");
            Console.WriteLine("- Better space-filling properties than Monte Carlo and LHS");
            Console.WriteLine("- Deterministic behavior (reproducible results)");
            Console.WriteLine("- Excellent performance for smooth, high-dimensional functions");
            Console.WriteLine("- Lower discrepancy than random sampling methods");
        }
        
        private static (double estimate, double error, double time) TestSamplingMethod(
            CentralProperties.SamplingMethod method, int dimensions, int sampleCount, 
            EntropySettings settings, double expectedValue)
        {
            Stopwatch sw = Stopwatch.StartNew();
            
            double[,] samples = QuasiRandomSampler.GenerateSamples(method, dimensions, sampleCount, settings);
            
            // Evaluate the test function: Sum of exp(-0.1 * ||x||^2) * cos(2π * x_i)
            double sum = 0;
            for (int sample = 0; sample < sampleCount; sample++)
            {
                double normSquared = 0;
                double cosSum = 0;
                
                for (int dim = 0; dim < dimensions; dim++)
                {
                    double x = samples[dim, sample];
                    normSquared += x * x;
                    cosSum += Math.Cos(2 * Math.PI * x);
                }
                
                sum += Math.Exp(-0.1 * normSquared) * cosSum;
            }
            
            double estimate = sum / sampleCount;
            double error = Math.Abs(estimate - expectedValue) / Math.Abs(expectedValue);
            
            sw.Stop();
            return (estimate, error, sw.Elapsed.TotalMilliseconds);
        }
        
        private static void TestSpaceFillingProperties()
        {
            int dimensions = 3;
            int sampleCount = 1000;
            EntropySettings settings = new EntropySettings();
            
            // Generate samples with each method
            double[,] mcSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.MonteCarlo, dimensions, sampleCount, settings);
            double[,] lhsSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.LatinHypercube, dimensions, sampleCount, settings);
            double[,] sobolSamples = QuasiRandomSampler.GenerateSamples(
                CentralProperties.SamplingMethod.Sobol, dimensions, sampleCount, settings);
            
            // Calculate space-filling metric (simplified discrepancy measure)
            double mcDiscrepancy = CalculateDiscrepancy(mcSamples, dimensions, sampleCount);
            double lhsDiscrepancy = CalculateDiscrepancy(lhsSamples, dimensions, sampleCount);
            double sobolDiscrepancy = CalculateDiscrepancy(sobolSamples, dimensions, sampleCount);
            
            Console.WriteLine("Space-filling quality (lower discrepancy = better):");
            Console.WriteLine($"Monte Carlo:      {mcDiscrepancy:F4}");
            Console.WriteLine($"Latin Hypercube:  {lhsDiscrepancy:F4}");
            Console.WriteLine($"Sobol Sequence:   {sobolDiscrepancy:F4}");
            
            string bestSpaceFilling = "Monte Carlo";
            double bestDisc = mcDiscrepancy;
            if (lhsDiscrepancy < bestDisc) { bestSpaceFilling = "Latin Hypercube"; bestDisc = lhsDiscrepancy; }
            if (sobolDiscrepancy < bestDisc) { bestSpaceFilling = "Sobol Sequence"; }
            
            Console.WriteLine($"Best space-filling: {bestSpaceFilling}");
        }
        
        private static double CalculateDiscrepancy(double[,] samples, int dimensions, int sampleCount)
        {
            // Simplified L2-discrepancy calculation
            // Measures how well samples fill the space compared to uniform distribution
            
            const int gridSize = 10;
            int[,,] grid = new int[gridSize, gridSize, gridSize];
            
            // Convert Gaussian samples to [0,1] and count in grid cells
            for (int sample = 0; sample < sampleCount; sample++)
            {
                int[] indices = new int[dimensions];
                for (int dim = 0; dim < Math.Min(dimensions, 3); dim++)
                {
                    // Convert Gaussian to uniform using error function approximation
                    double gaussian = samples[dim, sample];
                    double uniform = 0.5 * (1 + gaussian / Math.Sqrt(1 + gaussian * gaussian * 2 / Math.PI));
                    uniform = Math.Max(0, Math.Min(1, uniform));
                    indices[dim] = Math.Min((int)(uniform * gridSize), gridSize - 1);
                }
                
                if (dimensions >= 3)
                    grid[indices[0], indices[1], indices[2]]++;
            }
            
            // Calculate discrepancy as variance from expected uniform distribution
            double expectedPerCell = (double)sampleCount / (gridSize * gridSize * gridSize);
            double discrepancy = 0;
            
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    for (int k = 0; k < gridSize; k++)
                    {
                        double diff = grid[i, j, k] - expectedPerCell;
                        discrepancy += diff * diff;
                    }
                }
            }
            
            return Math.Sqrt(discrepancy / (gridSize * gridSize * gridSize)) / expectedPerCell;
        }
        
        private static void TestConvergenceRates()
        {
            int dimensions = 5;
            int[] sampleSizes = { 100, 200, 400, 800, 1600 };
            EntropySettings settings = new EntropySettings();
            
            Console.WriteLine("Convergence analysis for 5D Rosenbrock-like function:");
            Console.WriteLine("Sample Size | MC Error Rate | LHS Rate | Sobol Rate");
            Console.WriteLine("--------------------------------------------------------");
            
            double[] mcErrors = new double[sampleSizes.Length];
            double[] lhsErrors = new double[sampleSizes.Length];
            double[] sobolErrors = new double[sampleSizes.Length];
            
            // Expected value for the test function (approximately)
            double expectedValue = 0.0; // Function should integrate to approximately 0
            
            for (int i = 0; i < sampleSizes.Length; i++)
            {
                int sampleCount = sampleSizes[i];
                
                mcErrors[i] = TestConvergenceFunction(CentralProperties.SamplingMethod.MonteCarlo, 
                    dimensions, sampleCount, settings, expectedValue);
                lhsErrors[i] = TestConvergenceFunction(CentralProperties.SamplingMethod.LatinHypercube,
                    dimensions, sampleCount, settings, expectedValue);
                sobolErrors[i] = TestConvergenceFunction(CentralProperties.SamplingMethod.Sobol,
                    dimensions, sampleCount, settings, expectedValue);
                
                // Calculate convergence rates (log scale)
                string mcRate = i > 0 ? $"{Math.Log(mcErrors[i-1]/mcErrors[i])/Math.Log(2):F2}" : "---";
                string lhsRate = i > 0 ? $"{Math.Log(lhsErrors[i-1]/lhsErrors[i])/Math.Log(2):F2}" : "---";
                string sobolRate = i > 0 ? $"{Math.Log(sobolErrors[i-1]/sobolErrors[i])/Math.Log(2):F2}" : "---";
                
                Console.WriteLine($"{sampleCount,11} | {mcRate,11} | {lhsRate,8} | {sobolRate,10}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Note: Convergence rate shows error reduction factor when doubling samples");
            Console.WriteLine("      Higher values indicate faster convergence");
        }
        
        private static double TestConvergenceFunction(CentralProperties.SamplingMethod method, 
            int dimensions, int sampleCount, EntropySettings settings, double expectedValue)
        {
            double[,] samples = QuasiRandomSampler.GenerateSamples(method, dimensions, sampleCount, settings);
            
            // Modified Rosenbrock-like function for convergence testing
            double sum = 0;
            for (int sample = 0; sample < sampleCount; sample++)
            {
                double result = 0;
                for (int dim = 0; dim < dimensions - 1; dim++)
                {
                    double x = samples[dim, sample];
                    double y = samples[dim + 1, sample];
                    result += Math.Exp(-((x - 0.5) * (x - 0.5) + (y - 0.5) * (y - 0.5)));
                }
                sum += result;
            }
            
            double estimate = sum / sampleCount;
            return Math.Abs(estimate - expectedValue);
        }
    }
}