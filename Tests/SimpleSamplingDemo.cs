using System;
using System.Diagnostics;
using System.Linq;

namespace Tests
{
    /// <summary>
    /// Simple demonstration program showing the difference between Monte Carlo 
    /// and Latin Hypercube Sampling for a complex problem.
    /// </summary>
    public class SimpleSamplingDemo
    {
        public static void RunDemo()
        {
            Console.WriteLine("=== Monte Carlo vs Latin Hypercube Sampling Demo ===");
            Console.WriteLine();
            Console.WriteLine("Problem: Estimate π using a high-dimensional integration");
            Console.WriteLine("Function: Volume of unit hypersphere in 6 dimensions");
            Console.WriteLine("Theoretical value: π³/6 ≈ 5.1677");
            Console.WriteLine();
            
            const int sampleSize = 5000;
            const double theoreticalValue = Math.PI * Math.PI * Math.PI / 6.0;
            
            // Monte Carlo estimation
            var mcStopwatch = Stopwatch.StartNew();
            double mcResult = EstimateHypersphereVolumeMC(sampleSize);
            mcStopwatch.Stop();
            
            // LHS estimation  
            var lhsStopwatch = Stopwatch.StartNew();
            double lhsResult = EstimateHypersphereVolumeLHS(sampleSize);
            lhsStopwatch.Stop();
            
            double mcError = Math.Abs(mcResult - theoreticalValue);
            double lhsError = Math.Abs(lhsResult - theoreticalValue);
            double errorImprovement = ((mcError - lhsError) / mcError) * 100;
            
            Console.WriteLine($"Sample size: {sampleSize}");
            Console.WriteLine($"Theoretical value: {theoreticalValue:F6}");
            Console.WriteLine();
            Console.WriteLine($"Monte Carlo result: {mcResult:F6}");
            Console.WriteLine($"Monte Carlo error:  {mcError:F6} ({mcError/theoreticalValue*100:F2}%)");
            Console.WriteLine($"Monte Carlo time:   {mcStopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine();
            Console.WriteLine($"LHS result:         {lhsResult:F6}");
            Console.WriteLine($"LHS error:          {lhsError:F6} ({lhsError/theoreticalValue*100:F2}%)");
            Console.WriteLine($"LHS time:           {lhsStopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine();
            
            if (lhsError < mcError)
            {
                Console.WriteLine($"✓ LHS is {errorImprovement:F1}% more accurate than Monte Carlo");
            }
            else
            {
                Console.WriteLine($"⚠ Monte Carlo performed better this time");
            }
            
            double timeRatio = (double)lhsStopwatch.ElapsedMilliseconds / mcStopwatch.ElapsedMilliseconds;
            Console.WriteLine($"LHS time overhead: {timeRatio:F2}x");
            
            Console.WriteLine();
            Console.WriteLine("Key insights:");
            Console.WriteLine("• LHS provides better space coverage in high dimensions");
            Console.WriteLine("• LHS typically converges faster for smooth functions");
            Console.WriteLine("• LHS has higher setup cost but better sampling efficiency");
            Console.WriteLine("• Benefits increase with problem complexity and dimensionality");
        }
        
        private static double EstimateHypersphereVolumeMC(int sampleCount)
        {
            var random = new Random(42);
            int insideSphere = 0;
            
            for (int i = 0; i < sampleCount; i++)
            {
                double sumSquares = 0;
                for (int dim = 0; dim < 6; dim++)
                {
                    double x = random.NextDouble() * 2 - 1; // [-1, 1]
                    sumSquares += x * x;
                }
                
                if (sumSquares <= 1.0)
                {
                    insideSphere++;
                }
            }
            
            // Volume of unit hypercube is 2^6 = 64
            return (double)insideSphere / sampleCount * 64.0;
        }
        
        private static double EstimateHypersphereVolumeLHS(int sampleCount)
        {
            var random = new Random(42);
            int insideSphere = 0;
            
            // Generate LHS samples
            double[,] samples = new double[sampleCount, 6];
            
            // For each dimension, create stratified samples
            for (int dim = 0; dim < 6; dim++)
            {
                // Create interval indices and shuffle them
                int[] intervals = Enumerable.Range(0, sampleCount).ToArray();
                ShuffleArray(intervals, random);
                
                for (int i = 0; i < sampleCount; i++)
                {
                    // Generate uniform sample within interval
                    double u = (intervals[i] + random.NextDouble()) / sampleCount;
                    samples[i, dim] = u * 2 - 1; // Map to [-1, 1]
                }
            }
            
            // Count samples inside unit hypersphere
            for (int i = 0; i < sampleCount; i++)
            {
                double sumSquares = 0;
                for (int dim = 0; dim < 6; dim++)
                {
                    sumSquares += samples[i, dim] * samples[i, dim];
                }
                
                if (sumSquares <= 1.0)
                {
                    insideSphere++;
                }
            }
            
            // Volume of unit hypercube is 2^6 = 64
            return (double)insideSphere / sampleCount * 64.0;
        }
        
        private static void ShuffleArray(int[] array, Random random)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
        
        public static void Main(string[] args)
        {
            try
            {
                RunDemo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}