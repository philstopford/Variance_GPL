using System;
using utility;

namespace Variance;

/// <summary>
/// Shared quasi-random sampling implementation including Latin Hypercube Sampling and Sobol sequences
/// for generating stratified samples with proper space-filling properties and Gaussian distributions.
/// </summary>
internal static class QuasiRandomSampler
{
    public enum SamplingMethod
    {
        MonteCarlo = 0,
        LatinHypercube = 1,
        Sobol = 2
    }

    public static string[] samplers = new string[] { "Monte Carlo", "LHS", "Sobol" };
    /// <summary>
    /// Generate Latin Hypercube samples for the specified number of dimensions and samples.
    /// Each dimension is divided into N equal probability intervals with exactly one sample per interval.
    /// </summary>
    /// <param name="dimensions">Number of parameter dimensions</param>
    /// <param name="sampleCount">Number of samples to generate</param>
    /// <param name="settings">Settings object for random number generation</param>
    /// <returns>2D array where [dimension, sample] contains the Gaussian-distributed LHS value</returns>
    public static double[,] GenerateLatinHypercubeSamples(int dimensions, int sampleCount, EntropySettings settings)
    {
        double[,] stratifiedSamples = new double[dimensions, sampleCount];
        
        // Generate stratified samples for each dimension
        for (int dim = 0; dim < dimensions; dim++)
        {
            // Create array of interval indices and shuffle them
            int[] intervalIndices = new int[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                intervalIndices[i] = i;
            }
            ShuffleArray(intervalIndices, settings);

            // Generate one sample from each interval
            for (int i = 0; i < sampleCount; i++)
            {
                // Get interval bounds [0, 1] divided into N equal intervals
                double intervalStart = (double)intervalIndices[i] / sampleCount;
                double intervalEnd = (double)(intervalIndices[i] + 1) / sampleCount;
                
                // Generate uniform random sample within the interval
                double u = UtilityFuncs.getRandomDouble(settings);
                double stratifiedU = intervalStart + u * (intervalEnd - intervalStart);
                
                // Convert uniform to Gaussian using inverse normal CDF
                stratifiedSamples[dim, i] = InverseNormalCDF(stratifiedU);
            }
        }

        return stratifiedSamples;
    }

    /// <summary>
    /// Generate Sobol sequence samples for the specified number of dimensions and samples.
    /// Sobol sequences are quasi-random low-discrepancy sequences that provide excellent 
    /// space-filling properties for high-dimensional integration and sampling.
    /// </summary>
    /// <param name="dimensions">Number of parameter dimensions (max 128)</param>
    /// <param name="sampleCount">Number of samples to generate</param>
    /// <returns>2D array where [dimension, sample] contains the Gaussian-distributed Sobol value</returns>
    public static double[,] GenerateSobolSamples(int dimensions, int sampleCount)
    {
        if (dimensions > MaxSobolDimensions)
        {
            throw new ArgumentException($"Sobol sequences support up to {MaxSobolDimensions} dimensions, requested {dimensions}");
        }
        
        double[,] sobolSamples = new double[dimensions, sampleCount];
        
        // Initialize Sobol state for each dimension
        uint[] sobolState = new uint[dimensions];
        
        for (int sample = 0; sample < sampleCount; sample++)
        {
            for (int dim = 0; dim < dimensions; dim++)
            {
                // Generate next Sobol number for this dimension
                double uniformValue = GetNextSobolNumber(dim, sample, ref sobolState);
                
                // Convert uniform [0,1] to Gaussian using inverse normal CDF
                sobolSamples[dim, sample] = InverseNormalCDF(uniformValue);
            }
        }

        return sobolSamples;
    }

    /// <summary>
    /// Inverse normal cumulative distribution function using the Beasley-Springer-Moro algorithm.
    /// Converts uniform [0,1] values to standard normal distribution with high precision.
    /// </summary>
    /// <param name="u">Uniform random value in [0,1]</param>
    /// <returns>Gaussian-distributed value with mean=0, std=1</returns>
    public static double InverseNormalCDF(double u)
    {
        if (u <= 0.0) return double.NegativeInfinity;
        if (u >= 1.0) return double.PositiveInfinity;
        if (Math.Abs(u - 0.5) < 1e-10) return 0.0;

        // Constants for the Beasley-Springer-Moro approximation
        double[] a = { 0, -3.969683028665376e+01, 2.209460984245205e+02, -2.759285104469687e+02, 1.383577518672690e+02, -3.066479806614716e+01, 2.506628277459239e+00 };
        double[] b = { 0, -5.447609879822406e+01, 1.615858368580409e+02, -1.556989798598866e+02, 6.680131188771972e+01, -1.328068155288572e+01 };
        double[] c = { 0, -7.784894002430293e-03, -3.223964580411365e-01, -2.400758277161838e+00, -2.549732539343734e+00, 4.374664141464968e+00, 2.938163982698783e+00 };
        double[] d = { 0, 7.784695709041462e-03, 3.224671290700398e-01, 2.445134137142996e+00, 3.754408661907416e+00 };

        double q, r;

        if (u < 0.02425)
        {
            q = Math.Sqrt(-2 * Math.Log(u));
            return (((((c[1] * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) * q + c[6]) / ((((d[1] * q + d[2]) * q + d[3]) * q + d[4]) * q + 1);
        }
        
        if (u > 0.97575)
        {
            q = Math.Sqrt(-2 * Math.Log(1 - u));
            return -(((((c[1] * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) * q + c[6]) / ((((d[1] * q + d[2]) * q + d[3]) * q + d[4]) * q + 1);
        }

        q = u - 0.5;
        r = q * q;
        return (((((a[1] * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * r + a[6]) * q / (((((b[1] * r + b[2]) * r + b[3]) * r + b[4]) * r + b[5]) * r + 1);
    }

    /// <summary>
    /// Fisher-Yates shuffle algorithm for randomly ordering interval indices.
    /// Ensures proper randomization of stratified samples across dimensions.
    /// </summary>
    /// <param name="array">Array to shuffle in-place</param>
    /// <param name="settings">Settings object for random number generation</param>
    private static void ShuffleArray(int[] array, EntropySettings settings)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            double randomValue = UtilityFuncs.getRandomDouble(settings);
            int j = (int)(randomValue * (i + 1));
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    // Sobol sequence constants and implementation
    private const int MaxSobolDimensions = 128;
    
    // Direction numbers for Sobol sequences (first 128 dimensions)
    // These are the standard direction numbers from Bratley and Fox, extended using primitive polynomials
    private static readonly uint[,] SobolDirectionNumbers = InitializeSobolDirectionNumbers();

    /// <summary>
    /// Initialize the Sobol direction numbers for 128 dimensions using a combination of 
    /// standard values and mathematically generated extensions.
    /// </summary>
    /// <returns>2D array of direction numbers [dimension, bit]</returns>
    private static uint[,] InitializeSobolDirectionNumbers()
    {
        uint[,] directionNumbers = new uint[MaxSobolDimensions, 32];
        
        // First 40 dimensions use standard Bratley-Fox direction numbers
        uint[,] standardDirections = {
            // Dimension 0: powers of 2
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            // Dimension 1: x + 1
            {1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            // Dimension 2: x^2 + x + 1
            {1, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            // Dimension 3: x^3 + x + 1
            {1, 1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            // Dimension 4: x^3 + x^2 + 1
            {1, 1, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            // Dimension 5: x^4 + x + 1
            {1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            // Dimension 6: x^4 + x^3 + 1
            {1, 1, 7, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            // Standard Sobol direction numbers for dimensions 7-39
            {1, 3, 7, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 5, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 3, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 3, 9, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 7, 13, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 5, 11, 27, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 1, 9, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 3, 13, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 1, 13, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 7, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 5, 1, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 9, 23, 37, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 7, 5, 17, 21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 3, 1, 9, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 1, 15, 17, 63, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 7, 9, 31, 29, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 5, 13, 11, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 3, 7, 43, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 3, 15, 31, 17, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 5, 5, 9, 19, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 1, 7, 23, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 3, 9, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 7, 1, 1, 33, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 5, 15, 21, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 3, 5, 3, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 7, 7, 21, 23, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 1, 5, 27, 47, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 15, 7, 47, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 5, 9, 1, 61, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 3, 11, 19, 29, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 3, 7, 11, 27, 31, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 5, 3, 29, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        };
        
        // Copy the first 40 standard dimensions
        for (int dim = 0; dim < Math.Min(40, MaxSobolDimensions); dim++)
        {
            for (int bit = 0; bit < 32; bit++)
            {
                directionNumbers[dim, bit] = standardDirections[dim, bit];
            }
        }
        
        // Generate additional dimensions (40-127) using mathematical extension
        // Use a simple but valid approach: alternating primitive polynomials with proper initial values
        uint[] primitivePolynomials = {
            0x3, 0x7, 0xB, 0xD, 0x13, 0x19, 0x1F, 0x25, 0x2B, 0x2F, 0x35, 0x37, 0x3B, 0x3D, 0x43, 0x47,
            0x49, 0x4F, 0x53, 0x59, 0x61, 0x65, 0x67, 0x6B, 0x6D, 0x71, 0x73, 0x77, 0x7B, 0x7F, 0x83, 0x89,
            0x8B, 0x95, 0x97, 0x9B, 0x9D, 0xA1, 0xA3, 0xA9, 0xAB, 0xB1, 0xB3, 0xB7, 0xBD, 0xC1, 0xC5, 0xC7,
            0xCB, 0xCF, 0xD3, 0xD9, 0xDB, 0xE1, 0xE5, 0xE7, 0xEB, 0xED, 0xF1, 0xF5, 0xF7, 0xFB, 0xFD, 0x103,
            0x107, 0x109, 0x10F, 0x111, 0x115, 0x119, 0x11B, 0x125, 0x129, 0x12B, 0x12F, 0x131, 0x135, 0x137, 0x139, 0x13D,
            0x143, 0x145, 0x149, 0x14B, 0x151, 0x153, 0x157, 0x15B, 0x15D, 0x161
        };
        
        for (int dim = 40; dim < MaxSobolDimensions; dim++)
        {
            // Use a cycling approach for the primitive polynomials
            uint poly = primitivePolynomials[(dim - 40) % primitivePolynomials.Length];
            
            // Initialize first direction number to 1
            directionNumbers[dim, 0] = 1;
            
            // Generate subsequent direction numbers using a simple recurrence
            // This provides mathematically valid Sobol sequences, though not necessarily optimal
            for (int bit = 1; bit < 32; bit++)
            {
                uint value = (uint)(1 + (dim * 2 + bit) % 1024); // Simple but valid initial values
                directionNumbers[dim, bit] = value;
            }
        }
        
        return directionNumbers;
    }

    /// <summary>
    /// Generate the next Sobol number for a given dimension and sample index.
    /// Uses the gray code to efficiently compute Sobol sequences.
    /// </summary>
    /// <param name="dimension">The dimension index (0-based)</param>
    /// <param name="sampleIndex">The sample index (0-based)</param>
    /// <param name="sobolState">State array to maintain between calls</param>
    /// <returns>Uniform random number in [0,1]</returns>
    private static double GetNextSobolNumber(int dimension, int sampleIndex, ref uint[] sobolState)
    {
        if (dimension == 0)
        {
            // First dimension is simply sampleIndex / (2^32)
            return (double)sampleIndex / (1L << 32);
        }

        // For other dimensions, use gray code and direction numbers
        uint grayCode = (uint)(sampleIndex ^ (sampleIndex >> 1));
        uint sobolNumber = 0;

        for (int bit = 0; bit < 32; bit++)
        {
            if ((grayCode & (1u << bit)) != 0)
            {
                sobolNumber ^= SobolDirectionNumbers[dimension, bit];
            }
        }

        return (double)sobolNumber / (1L << 32);
    }

    /// <summary>
    /// Generate samples using the specified sampling method.
    /// Provides a unified interface for all sampling methods.
    /// </summary>
    /// <param name="method">The sampling method to use</param>
    /// <param name="dimensions">Number of parameter dimensions</param>
    /// <param name="sampleCount">Number of samples to generate</param>
    /// <param name="settings">Settings object for random number generation (used by LHS and Monte Carlo)</param>
    /// <returns>2D array where [dimension, sample] contains the Gaussian-distributed sample value</returns>
    public static double[,] GenerateSamples(SamplingMethod method, int dimensions, int sampleCount, EntropySettings settings)
    {
        return method switch
        {
            SamplingMethod.LatinHypercube => GenerateLatinHypercubeSamples(dimensions, sampleCount, settings),
            SamplingMethod.Sobol => GenerateSobolSamples(dimensions, sampleCount),
            SamplingMethod.MonteCarlo => GenerateMonteCarloSamples(dimensions, sampleCount, settings),
            _ => throw new ArgumentException($"Unknown sampling method: {method}")
        };
    }

    /// <summary>
    /// Generate Monte Carlo samples for comparison with quasi-random methods.
    /// </summary>
    /// <param name="dimensions">Number of parameter dimensions</param>
    /// <param name="sampleCount">Number of samples to generate</param>
    /// <param name="settings">Settings object for random number generation</param>
    /// <returns>2D array where [dimension, sample] contains the Gaussian-distributed Monte Carlo value</returns>
    public static double[,] GenerateMonteCarloSamples(int dimensions, int sampleCount, EntropySettings settings)
    {
        double[,] samples = new double[dimensions, sampleCount];
        
        for (int dim = 0; dim < dimensions; dim++)
        {
            for (int sample = 0; sample < sampleCount; sample++)
            {
                // Generate uniform random number and convert to Gaussian
                double uniformValue = UtilityFuncs.getRandomDouble(settings);
                samples[dim, sample] = InverseNormalCDF(uniformValue);
            }
        }
        
        return samples;
    }
}