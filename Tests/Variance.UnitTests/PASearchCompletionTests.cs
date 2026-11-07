using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Variance.UnitTests;

public class PASearchCompletionTests
{
    [TestCase(true, 25000, false)]
    [TestCase(true, 1, false)]
    [TestCase(true, 64, true)]
    [TestCase(false, 64, false)]
    [TestCase(false, 64, true)]
    public void Search_FillsEveryResultSlotAndPublishesSummary(bool parallel, int target, bool rejectFirst)
    {
        VarianceContext context = new(false, "", 8, 8);
        CommonVars settings = new(context);
        DirectoryInfo directory = new(TestContext.CurrentContext.TestDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Tests", "Variance.UnitTests", "TestData", "area_perpoly.variance")))
            directory = directory.Parent;
        Assert.That(directory, Is.Not.Null, "Locate the checked-in area_perpoly project");
        settings.storage.setLayerSettings = settings.setLayerSettings;
        string loadResult = settings.storage.loadSimulationSettings("4.2.3",
            Path.Combine(directory!.FullName, "Tests", "Variance.UnitTests", "TestData", "area_perpoly.variance"),
            settings.getSimulationSettings(), settings.getSimulationSettings_nonSim(), settings.getListOfSettings(),
            settings.getImplantSimulationSettings(), settings.getImplantSettings_nonSim(), settings.getImplantSettings(),
            settings.getNonSimulationSettings());
        Assert.That(loadResult, Is.Empty);
        PASearch search = settings.getPASearch();
        search.numberofPassCases = target;
        search.useFilter[0] = true;
        search.filterValues[0] = rejectFirst ? double.MaxValue : 90;
        search.filterIsMaxValue[0] = false;
        search.setPASearchable(0, Array.IndexOf(PASearch.paNames, "xOL"), true);
        search.setPASearchable(0, Array.IndexOf(PASearch.paNames, "yOL"), true);
        search.setUpperLimit(0, Array.IndexOf(PASearch.paNames, "xOL"), 10m);
        search.setUpperLimit(0, Array.IndexOf(PASearch.paNames, "yOL"), 10m);
        Entropy engine = new(ref context, settings);
        bool summarized = false, completed = false;
        int attempts = 0;
        void AfterAttempt()
        {
            if (Interlocked.Increment(ref attempts) == 1 && rejectFirst)
                search.filterValues[0] = 90;
        }
        engine.abortRunFunc = () =>
        {
            AfterAttempt();
            // Bound regressions such as reading past a one-element sampler.
            if (attempts > target * 20) settings.runAbort = true;
        };
        engine.abortRunFuncMT = (_, cancellation, token) =>
        {
            AfterAttempt();
            if (attempts > target * 20 + 100)
            {
                cancellation.Cancel();
                token.ThrowIfCancellationRequested();
            }
        };
        engine.postSimPASearchUIFunc = package =>
        {
            Assert.That(package.results_count, Is.EqualTo(search.numberofPassCases));
            Assert.That(package.getListOfResults().All(result => result != null), Is.True,
                "Passing results must occupy unique, contiguous slots");
            search.calculateMeanAndStdDev(package);
            foreach (int pa in new[] { Array.IndexOf(PASearch.paNames, "xOL"), Array.IndexOf(PASearch.paNames, "yOL") })
            {
                Assert.That(double.IsFinite(double.Parse(search.getMeanValue(0, pa))), Is.True);
                Assert.That(double.IsFinite(double.Parse(search.getSDValue(0, pa))), Is.True);
            }
            Assert.That(engine.currentProgress, Is.EqualTo(target));
            Assert.That(package.runTime, Is.GreaterThan(0));
            if (target > 1)
                Assert.That(package.getListOfResults().Select(r => r.getField(Results.fields_d.olx, 0)).Distinct().Count(), Is.GreaterThan(1),
                    "Search attempts must generate fresh overlay samples");
            summarized = true;
        };
        engine.postSimUIFunc = () => completed = true;
        engine.EntropyRun(target, null, parallel, true);
        Assert.That(summarized, Is.True);
        Assert.That(completed, Is.True);
        if (rejectFirst) Assert.That(attempts, Is.GreaterThanOrEqualTo(target));
    }
}
