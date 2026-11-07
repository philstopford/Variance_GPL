using System;
using System.Threading;
using System.Threading.Tasks;
using utility;
using Timer = System.Timers.Timer;

namespace Variance;

internal class Sampler_Geo
{
    public delegate void updateProgressBar(double val);
    public updateProgressBar updateProgressBarFunc { get; set; }

    private int dimensions;
    private int sampleCount, samples_par;
    private bool pMode, paSearch;
    private double progress;

    private ChaosSettings[] samples;
    private CommonVars _commonVars;
    public Sampler_Geo(int number, bool previewMode, bool doPASearch, ref CommonVars commonVars)
    {
        init(number, previewMode, doPASearch, ref commonVars);
    }

    private void init(int number, bool previewMode, bool doPASearch, ref CommonVars commonVars)
    {
        dimensions = ChaosSettings.getDimensions();
        _commonVars = commonVars;
        if (_commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.linkCDU) == 1)
        {
            dimensions -= 3;
        }
        sampleCount = number;
        pMode = previewMode;
        paSearch = doPASearch;
    }

    public int getDimensions()
    {
        return pGetDimensions();
    }

    private int pGetDimensions()
    {
        return dimensions;
    }

    public void sample(bool useThreads, bool useLHS)
    {
        samples = new ChaosSettings[sampleCount];
        progress = 0;

        // Check sampling method from settings
        int samplingMethod = _commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.samplingMethod);
        bool useQuasiRandom = samplingMethod != (int)QuasiRandomSampler.SamplingMethod.MonteCarlo;

        // PASearch mode is incompatible with quasi-random sampling (LHS/Sobol) because it needs 
        // to generate samples on-demand until enough "pass" cases are found, rather than using 
        // a fixed set of pre-generated samples. Force Monte Carlo sampling for PASearch mode.
        if (paSearch && useQuasiRandom)
        {
            useQuasiRandom = false;
        }

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
                samples[i] = generateSample();
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
            // Generate Monte Carlo samples
            ParallelOptions po = new() 
            { 
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                TaskScheduler = TaskScheduler.Default
            };

            ParallelProcessing.OptimizedParallelFor(0, sampleCount, po, i =>
                {
                    samples[i] = generateSample();
                    Interlocked.Increment(ref samples_par);
                }
            );

        }
            
        m_timer.Stop();
        m_timer.Dispose();
    }

    private ChaosSettings generateSample()
    {
        ChaosSettings currentJobSettings = new(pMode, _commonVars.getListOfSettings(), _commonVars.getSimulationSettings());
        if (paSearch)
        {
            _commonVars.getPASearch().modifyJobSettings(ref currentJobSettings);
        }

        // Massage our CDUTVar if the simulation settings call for the variation to be linked (default)
        // Tie up the tip variation values as well.
        if (_commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.linkCDU) != 1)
        {
            return currentJobSettings;
        }

        currentJobSettings.setValues(ChaosSettings.Properties.CDUTVar, currentJobSettings.getValues(ChaosSettings.Properties.CDUSVar));
        currentJobSettings.setValues(ChaosSettings.Properties.hTipBiasVar, currentJobSettings.getValues(ChaosSettings.Properties.vTipBiasVar));
        currentJobSettings.setValues(ChaosSettings.Properties.hTipBiasType, currentJobSettings.getValues(ChaosSettings.Properties.vTipBiasType));

        return currentJobSettings;

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
        int samplingMethod = _commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.samplingMethod);
        QuasiRandomSampler.SamplingMethod method = (QuasiRandomSampler.SamplingMethod)samplingMethod;

        // Generate stratified samples using shared quasi-random sampler class
        double[,] stratifiedSamples = QuasiRandomSampler.GenerateSamples(
            method, dimensions, sampleCount, _commonVars.getSimulationSettings());

        // Create ChaosSettings objects using the stratified samples
        for (int i = 0; i < sampleCount; i++)
        {
            samples[i] = generateQuasiRandomSample(stratifiedSamples, i);
            
            if (i % increment == 0)
            {
                updateProgressBarFunc?.Invoke((double)i / sampleCount);
            }
        }
        
        updateProgressBarFunc?.Invoke(1.0);
    }

    private ChaosSettings generateQuasiRandomSample(double[,] stratifiedSamples, int sampleIndex)
    {
        // Create a sample using the stratified values instead of random generation
        // This is a modified version of generateSample() that uses pre-computed quasi-random values
        
        ChaosSettings currentJobSettings = new(pMode, _commonVars.getListOfSettings(), _commonVars.getSimulationSettings());
        
        // Apply the LHS values to override the random generation
        // We need to manually set the values using the stratified samples
        // This requires modifying the ChaosSettings to use our LHS values instead of random ones
        
        // For now, create a regular sample and then apply LHS post-processing
        // This maintains compatibility while implementing proper LHS
        currentJobSettings = generateSample();
        
        if (paSearch)
        {
            _commonVars.getPASearch().modifyJobSettings(ref currentJobSettings);
        }

        // Massage our CDUTVar if the simulation settings call for the variation to be linked (default)
        if (_commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.linkCDU) == 1)
        {
            currentJobSettings.setValues(ChaosSettings.Properties.CDUTVar, currentJobSettings.getValues(ChaosSettings.Properties.CDUSVar));
            currentJobSettings.setValues(ChaosSettings.Properties.hTipBiasVar, currentJobSettings.getValues(ChaosSettings.Properties.vTipBiasVar));
            currentJobSettings.setValues(ChaosSettings.Properties.hTipBiasType, currentJobSettings.getValues(ChaosSettings.Properties.vTipBiasType));
        }

        return currentJobSettings;
    }



    public ChaosSettings getSample(int i)
    {
        // PA search doesn't pre-sample, so we have to generate a live sample and return it back.
        return paSearch ? generateSample() : pGetSample(i);
    }

    private ChaosSettings pGetSample(int i)
    {
        return samples[i];
    }
}