using System;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace Variance
{
    class Sampler_Implant
    {
        public delegate void updateProgressBar(double val);
        public updateProgressBar updateProgressBarFunc { get; set; }

        ChaosSettings_implant[] samples;

        int dimensions;
        int sampleCount, samples_par;
        bool pMode;
        double progress;

        EntropySettings entropySettings_implant;

        public Sampler_Implant(int number, bool previewMode, EntropySettings entropySettings)
        {
            init(number, previewMode, entropySettings);
        }

        void init(int number, bool previewMode, EntropySettings entropySettings)
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

        int pGetDimensions()
        {
            return dimensions;
        }
        public void sample(bool useThreads)
        {
            progress = 0;
            samples = new ChaosSettings_implant[sampleCount];

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
                ChaosSettings_implant currentJobSettings = new ChaosSettings_implant(pMode, entropySettings_implant);
                samples[i] = currentJobSettings;

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
                ChaosSettings_implant currentJobSettings = new ChaosSettings_implant(pMode, entropySettings_implant);
                samples[i] = currentJobSettings;
                Interlocked.Increment(ref samples_par);
            }
            );

            m_timer.Stop();
            m_timer.Dispose();
        }

        public ChaosSettings_implant getSample(int i)
        {
            return pGetSample(i);
        }

        ChaosSettings_implant pGetSample(int i)
        {
            return samples[i];
        }
    }
}
