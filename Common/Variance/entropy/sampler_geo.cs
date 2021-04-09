using System;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace Variance
{
    class Sampler_Geo
    {
        public delegate void updateProgressBar(double val);
        public updateProgressBar updateProgressBarFunc { get; set; }

        int dimensions;
        int sampleCount, samples_par;
        bool pMode, paSearch;
        double progress;

        ChaosSettings[] samples;
        CommonVars _commonVars;
        public Sampler_Geo(int number, bool previewMode, bool doPASearch, ref CommonVars commonVars)
        {
            init(number, previewMode, doPASearch, ref commonVars);
        }

        void init(int number, bool previewMode, bool doPASearch, ref CommonVars commonVars)
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

        int pGetDimensions()
        {
            return dimensions;
        }

        public void sample(bool useThreads)
        {
            samples = new ChaosSettings[sampleCount];
            progress = 0;
            if (useThreads)
            {
                threaded();
            }
            else
            {
                unthreaded();
            }
        }

        void unthreaded()
        {
            int increment = sampleCount / 100;
            if (increment < 1)
            {
                increment = 1;
            }
            updateProgressBarFunc?.Invoke(progress);

            for (int i = 0; i < sampleCount; i++)
            {
                samples[i] = generateSample();
                if (i % increment == 0)
                {
                    updateProgressBarFunc?.Invoke(progress);
                    progress += 0.01;
                }
            }
        }

        void updateHost(object sender, EventArgs e)
        {
            progress = (double)samples_par / sampleCount;
            updateProgressBarFunc?.Invoke(progress);
        }

        void threaded()
        {
            samples_par = 0;
            Timer m_timer = new Timer();
            // Set up timers for the UI refresh
            m_timer.AutoReset = true;
            m_timer.Interval = CentralProperties.timer_interval;
            m_timer.Start();
            m_timer.Elapsed += updateHost;

            ParallelOptions po = new ParallelOptions();

            Parallel.For(0, sampleCount, po, (i, loopState) =>
            {
                samples[i] = generateSample();
                Interlocked.Increment(ref samples_par);
            }
            );

            m_timer.Stop();
            m_timer.Dispose();
        }

        ChaosSettings generateSample()
        {
            ChaosSettings currentJobSettings = new ChaosSettings(pMode, _commonVars.getListOfSettings(), _commonVars.getSimulationSettings());
            if (paSearch)
            {
                _commonVars.getPASearch().modifyJobSettings(ref currentJobSettings);
            }

            // Massage our CDUTVar if the simulation settings call for the variation to be linked (default)
            // Tie up the tip variation values as well.
            if (_commonVars.getSimulationSettings().getValue(EntropySettings.properties_i.linkCDU) == 1)
            {
                currentJobSettings.setValues(ChaosSettings.properties.CDUTVar, currentJobSettings.getValues(ChaosSettings.properties.CDUSVar));
                currentJobSettings.setValues(ChaosSettings.properties.hTipBiasVar, currentJobSettings.getValues(ChaosSettings.properties.vTipBiasVar));
                currentJobSettings.setValues(ChaosSettings.properties.hTipBiasType, currentJobSettings.getValues(ChaosSettings.properties.vTipBiasType));
            }

            return currentJobSettings;

        }

        public ChaosSettings getSample(int i)
        {
            // PA search doesn't presample, so we have to generate a live sample and return it back.
            if (paSearch)
            {
                return generateSample();
            }
            return pGetSample(i);
        }

        ChaosSettings pGetSample(int i)
        {
            return samples[i];
        }
    }
}
