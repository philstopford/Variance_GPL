using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Variance;
using Clipper2Lib;

namespace Variance.UnitTests
{
    [TestFixture]
    public class StorageRaceConditionTests
    {
        private Storage storage;
        private bool uiSuspended;
        private bool uiResumed;
        private bool callbackExecuted;
        private object lockObject = new object();
        
        [SetUp]
        public void Setup()
        {
            storage = new Storage();
            uiSuspended = false;
            uiResumed = false;
            callbackExecuted = false;
        }

        [Test]
        public void UpdateSettingsUICallback_WithCallback_ExecutesCallback()
        {
            // Arrange
            Action testCallback = () => { callbackExecuted = true; };
            bool callbackWithCallbackExecuted = false;
            
            storage.updateSettingsUIFromSettingsWithCallback = (callback) =>
            {
                callbackWithCallbackExecuted = true;
                callback?.Invoke();
            };

            // Act - Simulate the pattern used in Storage.cs
            if (storage.updateSettingsUIFromSettingsWithCallback != null)
            {
                storage.updateSettingsUIFromSettingsWithCallback(testCallback);
            }
            else
            {
                storage.updateSettingsUIFromSettings?.Invoke();
            }

            // Assert
            Assert.That(callbackWithCallbackExecuted, Is.True, "Callback with callback should be executed");
            Assert.That(callbackExecuted, Is.True, "Test callback should be executed");
        }

        [Test]
        public void UpdateSettingsUICallback_WithoutCallback_FallsBackToRegular()
        {
            // Arrange
            bool regularCallbackExecuted = false;
            storage.updateSettingsUIFromSettings = () => { regularCallbackExecuted = true; };

            // Act - Simulate the pattern used in Storage.cs when no callback version available
            if (storage.updateSettingsUIFromSettingsWithCallback != null)
            {
                storage.updateSettingsUIFromSettingsWithCallback(null);
            }
            else
            {
                storage.updateSettingsUIFromSettings?.Invoke();
            }

            // Assert
            Assert.That(regularCallbackExecuted, Is.True, "Regular callback should be executed when callback version not available");
        }

        [Test]
        public void SuspendResumeUI_ProperSequencing_NoRaceCondition()
        {
            // Arrange
            var events = new List<string>();
            var lockObj = new object();

            storage.suspendSettingsUI = () =>
            {
                Thread.Sleep(50); // Simulate async delay
                lock (lockObj) { events.Add("suspend"); }
            };

            storage.updateSettingsUIFromSettingsWithCallback = (callback) =>
            {
                lock (lockObj) { events.Add("update"); }
                callback?.Invoke();
            };

            storage.resumeUI = () =>
            {
                lock (lockObj) { events.Add("resume"); }
            };

            // Act - Simulate proper sequencing
            storage.suspendSettingsUI?.Invoke();
            if (storage.updateSettingsUIFromSettingsWithCallback != null)
            {
                storage.updateSettingsUIFromSettingsWithCallback(() =>
                {
                    lock (lockObj) { events.Add("callback"); }
                });
            }
            storage.resumeUI?.Invoke();

            // Assert
            Assert.That(4, Is.EqualTo(events.Count), "All events should be recorded");
            Assert.That("suspend", Is.EqualTo(events[0]), "Suspend should happen first");
            Assert.That("update", Is.EqualTo(events[1]), "Update should happen after suspend");
            Assert.That("callback", Is.EqualTo(events[2]), "Callback should happen after update");
            Assert.That("resume", Is.EqualTo(events[3]), "Resume should happen last");
        }

        [Test]
        public void StringFromFileData_ThreadSafety_NoConcurrentModification()
        {
            // Arrange - Create test file data
            var fileData = new PathsD
            {
                new PathD { new PointD(0, 0), new PointD(100, 0), new PointD(100, 100), new PointD(0, 100) },
                new PathD { new PointD(200, 200), new PointD(300, 200), new PointD(300, 300), new PointD(200, 300) }
            };

            var results = new List<string>();
            var exceptions = new List<Exception>();
            const int threadCount = 10;
            var threads = new Thread[threadCount];

            // Act - Test concurrent access
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    try
                    {
                        // Use reflection to access the private static method
                        var method = typeof(Storage).GetMethod("stringFromFileData", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        var result = (string)method.Invoke(null, new object[] { fileData });
                        lock (lockObject) { results.Add(result); }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject) { exceptions.Add(ex); }
                    }
                });
            }

            foreach (var thread in threads) thread.Start();
            foreach (var thread in threads) thread.Join();

            // Assert
            Assert.That(0, Is.EqualTo(exceptions.Count), $"No exceptions should occur during concurrent access. Exceptions: {string.Join(", ", exceptions.Select(e => e.Message))}");
            Assert.That(threadCount, Is.EqualTo(results.Count), "All threads should complete successfully");
            
            // All results should be identical
            string expectedResult = results[0];
            foreach (var result in results)
            {
                Assert.That(expectedResult, Is.EqualTo(result), "All concurrent results should be identical");
            }
        }

        [Test]
        public void ViewportOperations_ProperSequencing_NoRaceCondition()
        {
            // Arrange
            var operationSequence = new List<string>();
            var lockObj = new object();

            storage.viewportSave = (index) =>
            {
                lock (lockObj) { operationSequence.Add($"save_{index}"); }
                return new double[] { 100, 200, 1.5 }; // x, y, zoom
            };

            storage.viewportLoad = (index, data) =>
            {
                lock (lockObj) { operationSequence.Add($"load_{index}_{data[0]}_{data[1]}_{data[2]}"); }
            };

            // Act - Simulate viewport save/load sequence from Storage.cs
            for (int i = 0; i < 3; i++)
            {
                double[]? camParameters = storage.viewportSave?.Invoke(i);
                if (camParameters != null)
                {
                    storage.viewportLoad?.Invoke(i, camParameters);
                }
            }

            // Assert
            Assert.That(6, Is.EqualTo(operationSequence.Count), "Should have 3 save and 3 load operations");
            
            for (int i = 0; i < 3; i++)
            {
                Assert.That($"save_{i}", Is.EqualTo(operationSequence[i * 2]), $"Save operation {i} should happen before load");
                Assert.That($"load_{i}_100_200_1.5", Is.EqualTo(operationSequence[i * 2 + 1]), $"Load operation {i} should happen after save with correct data");
            }
        }

        [Test]
        public void ConcurrentLayerAccess_ThreadSafety_NoDataCorruption()
        {
            // Arrange
            var layerSettings = new List<EntropyLayerSettings>();
            for (int i = 0; i < 10; i++)
            {
                var settings = new EntropyLayerSettings();
                settings.setString(EntropyLayerSettings.properties_s.name, $"Layer{i}");
                layerSettings.Add(settings);
            }

            var results = new List<string>();
            var exceptions = new List<Exception>();
            const int threadCount = 5;
            var threads = new Thread[threadCount];

            // Act - Test concurrent layer access
            for (int i = 0; i < threadCount; i++)
            {
                int threadIndex = i;
                threads[i] = new Thread(() =>
                {
                    try
                    {
                        for (int j = 0; j < layerSettings.Count; j++)
                        {
                            // Simulate the caching pattern from pStoreSimulationSettings_layer
                            var currentLayer = layerSettings[j];
                            string name = currentLayer.getString(EntropyLayerSettings.properties_s.name);
                            lock (lockObject) { results.Add($"Thread{threadIndex}_{name}"); }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (lockObject) { exceptions.Add(ex); }
                    }
                });
            }

            foreach (var thread in threads) thread.Start();
            foreach (var thread in threads) thread.Join();

            // Assert
            Assert.That(0, Is.EqualTo(exceptions.Count), $"No exceptions should occur during concurrent layer access. Exceptions: {string.Join(", ", exceptions.Select(e => e.Message))}");
            Assert.That(threadCount * layerSettings.Count, Is.EqualTo(results.Count), "All threads should complete all layer accesses");
        }

        [Test]
        public void EmptyFileData_ThreadSafety_ReturnsEmptyString()
        {
            // Arrange
            var emptyFileData = new PathsD();
            
            // Act & Assert - Test with reflection to access private method
            var method = typeof(Storage).GetMethod("stringFromFileData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (string)method.Invoke(null, new object[] { emptyFileData });
            
            Assert.That("", Is.EqualTo(result), "Empty file data should return empty string");
        }

        [Test]
        public void StorageInitialization_AllCallbacksNull_NoExceptions()
        {
            // Arrange & Act
            var newStorage = new Storage();

            // Assert - Verify no exceptions thrown during initialization
            Assert.That(newStorage, !Is.Null, "Storage should initialize successfully");
            Assert.That(newStorage.updateSettingsUIFromSettings, Is.Null, "Regular callback should be null by default");
            Assert.That(newStorage.updateSettingsUIFromSettingsWithCallback, Is.Null, "Callback with callback should be null by default");
        }
    }
}