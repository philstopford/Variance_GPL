using System;
using System.Threading;
using System.Threading.Tasks;
using utility;
using Timer = System.Timers.Timer;

namespace Variance;

internal class Sampler_Implant
{
    public delegate void updateProgressBar(double val);
    public updateProgressBar updateProgressBarFunc { get; set; }

    private ChaosSettings_implant[] samples;

    private int dimensions;
    private int sampleCount, samples_par;
    private bool pMode;
    private double progress;

    private EntropySettings entropySettings_implant;

    public Sampler_Implant(int number, bool previewMode, EntropySettings entropySettings)
    {
        init(number, previewMode, entropySettings);
    }

    private void init(int number, bool previewMode, EntropySettings entropySettings)
    {
        dimensions = ChaosSettings_implant.getDimensions();
        sampleCount = number;
        pMode = previewMode;
        entropySettings_implant = entropySettings;
    }

    public int getDimensions()
    {
        return pGetDimensions();
    }

    private int pGetDimensions()
    {
        return dimensions;
    }
    public void sample(bool useThreads)
    {
        progress = 0;
        samples = new ChaosSettings_implant[sampleCount];

        // Check sampling method from settings
        int samplingMethod = entropySettings_implant.getValue(EntropySettings.properties_i.samplingMethod);
        bool useQuasiRandom = samplingMethod != (int)QuasiRandomSampler.SamplingMethod.MonteCarlo;

        if (useThreads)
        {
            threaded(useQuasiRandom);
        }
        else
        {
            unthreaded(useQuasiRandom);
        }
    }

    private void unthreaded(bool useQuasiRandom)
    {
        int increment = sampleCount / 100;
        if (increment < 1)
        {
            increment = 1;
        }
        updateProgressBarFunc?.Invoke(progress);

        if (useQuasiRandom)
        {
            // Generate samples using quasi-random methods (LHS or Sobol)
            generateQuasiRandomSamples();
        }
        else
        {
            // Generate Monte Carlo samples
            for (int i = 0; i < sampleCount; i++)
            {
                ChaosSettings_implant currentJobSettings = new(pMode, entropySettings_implant);
                samples[i] = currentJobSettings;

                if (i % increment != 0)
                {
                    continue;
                }

                updateProgressBarFunc?.Invoke(progress);
                progress += 0.01;
            }
        }
    }

    private void updateHost(object sender, EventArgs e)
    {
        progress = (double)samples_par / sampleCount;
        updateProgressBarFunc?.Invoke(progress);
    }

    private void threaded(bool useQuasiRandom)
    {
        samples_par = 0;
        // Set up timers for the UI refresh
        Timer m_timer = new() {AutoReset = true, Interval = CentralProperties.timer_interval};
        m_timer.Start();
        m_timer.Elapsed += updateHost;

        if (useQuasiRandom)
        {
            // Generate samples using quasi-random methods (LHS or Sobol)
            generateQuasiRandomSamples();
            samples_par = sampleCount; // Update progress manually for quasi-random methods
        }
        else
        {
            // Generate Monte Carlo samples using OptimizedParallelFor
            ParallelProcessing.OptimizedParallelFor(0, sampleCount, i =>
            {
                ChaosSettings_implant currentJobSettings = new(pMode, entropySettings_implant);
                samples[i] = currentJobSettings;
                Interlocked.Increment(ref samples_par);
            });
        }

        m_timer.Stop();
        m_timer.Dispose();
    }

    public ChaosSettings_implant getSample(int i)
    {
        return pGetSample(i);
    }

    private ChaosSettings_implant pGetSample(int i)
    {
        return samples[i];
    }

    private void generateQuasiRandomSamples()
    {
        // Use shared quasi-random sampling implementation
        int increment = sampleCount / 100;
        if (increment < 1)
        {
            increment = 1;
        }

        // Get the sampling method from settings
        int samplingMethod = entropySettings_implant.getValue(EntropySettings.properties_i.samplingMethod);
        QuasiRandomSampler.SamplingMethod method = (QuasiRandomSampler.SamplingMethod)samplingMethod;

        // Generate stratified samples using shared quasi-random sampler class
        double[,] stratifiedSamples = QuasiRandomSampler.GenerateSamples(
            method, dimensions, sampleCount, entropySettings_implant);

        // Create ChaosSettings_implant objects using the stratified samples
        // Use OptimizedParallelFor for better performance
        ParallelProcessing.OptimizedParallelFor(0, sampleCount, i =>
        {
            // Extract the quasi-random values for this sample
            double[] quasiRandomValues = new double[dimensions];
            for (int dim = 0; dim < dimensions; dim++)
            {
                quasiRandomValues[dim] = stratifiedSamples[dim, i];
            }

            // Create ChaosSettings_implant using the quasi-random constructor
            samples[i] = new ChaosSettings_implant(pMode, entropySettings_implant, quasiRandomValues);
            
            if (i % increment == 0)
            {
                updateProgressBarFunc?.Invoke((double)i / sampleCount);
            }
        });
        
        updateProgressBarFunc?.Invoke(1.0);
    }
}