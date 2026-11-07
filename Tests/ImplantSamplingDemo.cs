using System;
using Variance;

namespace Variance.Demo
{
    /// <summary>
    /// Simple demonstration of Latin Hypercube Sampling vs Monte Carlo for sampler_implant.
    /// Shows the performance difference for a complex 5D function evaluation.
    /// </summary>
    class ImplantSamplingDemo
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Implant Sampler: Monte Carlo vs Latin Hypercube Sampling Demo ===\n");
            
            // Define a complex 5D test function for implant parameters
            // This represents some complex physical process involving all 5 implant variations
            Func<double[], double> complexFunction = (x) => 
            {
                // Multimodal function with interactions between dimensions
                double resistCD = x[0];      // resistCDVar
                double resistHeight = x[1];  // resistHeightVar  
                double resistTopCRR = x[2];  // resistTopCRRVar
                double tilt = x[3];          // tiltVar
                double twist = x[4];         // twistVar
                
                // Complex function with cross-dimensional interactions
                double result = 
                    Math.Exp(-0.5 * (resistCD * resistCD + resistHeight * resistHeight)) * 
                    Math.Cos(resistTopCRR + tilt) +
                    Math.Exp(-0.3 * (tilt * tilt + twist * twist)) * 
                    Math.Sin(resistCD * resistHeight) +
                    Math.Exp(-0.7 * resistTopCRR * resistTopCRR) * 
                    Math.Cos(tilt * twist);
                    
                return result;
            };
            
            EntropySettings settings = new EntropySettings();
            int sampleCount = 2000;
            settings.setValue(EntropySettings.properties_i.nCases, sampleCount);
            
            Console.WriteLine($"Evaluating complex 5D function with {sampleCount} samples...\n");
            
            // === Monte Carlo Sampling ===
            settings.setValue(EntropySettings.properties_i.samplingMethod, (int)CentralProperties.SamplingMethod.MonteCarlo);
            
            var mcStartTime = DateTime.Now;
            Sampler_Implant mcSampler = new Sampler_Implant(sampleCount, false, settings);
            mcSampler.sample(false);
            
            double mcSum = 0.0;
            double mcSumSquares = 0.0;
            
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
                
                double value = complexFunction(x);
                mcSum += value;
                mcSumSquares += value * value;
            }
            
            var mcEndTime = DateTime.Now;
            double mcMean = mcSum / sampleCount;
            double mcVariance = (mcSumSquares / sampleCount) - (mcMean * mcMean);
            double mcStdError = Math.Sqrt(mcVariance / sampleCount);
            
            // === Latin Hypercube Sampling ===
            settings.setValue(EntropySettings.properties_i.samplingMethod, (int)CentralProperties.SamplingMethod.LatinHypercube);
            
            var lhsStartTime = DateTime.Now;
            Sampler_Implant lhsSampler = new Sampler_Implant(sampleCount, false, settings);
            lhsSampler.sample(false);
            
            double lhsSum = 0.0;
            double lhsSumSquares = 0.0;
            
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
                
                double value = complexFunction(x);
                lhsSum += value;
                lhsSumSquares += value * value;
            }
            
            var lhsEndTime = DateTime.Now;
            double lhsMean = lhsSum / sampleCount;
            double lhsVariance = (lhsSumSquares / sampleCount) - (lhsMean * lhsMean);
            double lhsStdError = Math.Sqrt(lhsVariance / sampleCount);
            
            // === Results Comparison ===
            Console.WriteLine("RESULTS:");
            Console.WriteLine("========");
            Console.WriteLine($"Monte Carlo:");
            Console.WriteLine($"  Mean Estimate: {mcMean:F6} ± {mcStdError:F6}");
            Console.WriteLine($"  Sampling Time: {(mcEndTime - mcStartTime).TotalMilliseconds:F1} ms");
            Console.WriteLine();
            Console.WriteLine($"Latin Hypercube:");
            Console.WriteLine($"  Mean Estimate: {lhsMean:F6} ± {lhsStdError:F6}");
            Console.WriteLine($"  Sampling Time: {(lhsEndTime - lhsStartTime).TotalMilliseconds:F1} ms");
            Console.WriteLine();
            
            double errorImprovement = Math.Abs(1.0 - lhsStdError / mcStdError) * 100;
            double biasReduction = Math.Abs(lhsMean - mcMean);
            
            Console.WriteLine($"PERFORMANCE COMPARISON:");
            Console.WriteLine($"=======================");
            Console.WriteLine($"Standard Error Improvement: {errorImprovement:F1}%");
            Console.WriteLine($"Estimate Difference: {biasReduction:F6}");
            Console.WriteLine($"LHS/MC Error Ratio: {lhsStdError/mcStdError:F3}");
            
            if (lhsStdError < mcStdError)
            {
                Console.WriteLine("\n✓ LHS shows improved precision (lower standard error)");
            }
            else
            {
                Console.WriteLine("\n~ Results are similar for this particular function");
            }
            
            Console.WriteLine("\nThe benefits of LHS become more pronounced with:");
            Console.WriteLine("• Higher dimensional problems (more parameters)");
            Console.WriteLine("• Smooth, well-behaved functions");
            Console.WriteLine("• Larger sample sizes");
            Console.WriteLine("• Integration/expectation estimation tasks");
        }
    }
}