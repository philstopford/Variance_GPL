using NUnit.Framework;
using System;
using System.Threading;
using Variance;

namespace Variance.UnitTests
{
    /// <summary>
    /// Unit tests for the SimResultPackage class functionality.
    /// Tests core result storage, retrieval, and state management operations.
    /// </summary>
    [TestFixture]
    public class SimResultPackageTests
    {
        private SimResultPackage resultPackage;

        [SetUp]
        public void Setup()
        {
            resultPackage = new SimResultPackage();
        }

        [TearDown]
        public void TearDown()
        {
            resultPackage?.Dispose();
        }

        /// <summary>
        /// Tests basic state management operations.
        /// </summary>
        [Test]
        public void State_SetAndGet_WorksCorrectly()
        {
            // Act
            resultPackage.setState(true);
            
            // Assert
            Assert.That(resultPackage.getState(), Is.True);
            
            // Act
            resultPackage.setState(false);
            
            // Assert
            Assert.That(resultPackage.getState(), Is.False);
        }

        /// <summary>
        /// Tests that initial state is false by default.
        /// </summary>
        [Test]
        public void State_InitialValue_IsFalse()
        {
            // Assert
            Assert.That(resultPackage.getState(), Is.False);
        }

        /// <summary>
        /// Tests adding a single result and retrieving it.
        /// </summary>
        [Test]
        public void AddResult_SingleResult_CanBeRetrieved()
        {
            // Arrange
            resultPackage.setState(true);
            var result = new Results();
            result.setResult("1.0,2.0,3.0");
            
            // Act
            resultPackage.Add(0, result, 0);
            
            // Assert
            Assert.That(1, Is.EqualTo(resultPackage.results_count));
            var retrievedResult = resultPackage.getResult(0);
            Assert.That(retrievedResult, !Is.Null);
            Assert.That("1.0,2.0,3.0", Is.EqualTo(retrievedResult.getResult()));
        }

        /// <summary>
        /// Tests adding multiple results with different indices.
        /// </summary>
        [Test]
        public void AddResult_MultipleResults_AllCanBeRetrieved()
        {
            // Arrange
            resultPackage.setState(true);
            
            // Act
            for (int i = 0; i < 5; i++)
            {
                var result = new Results();
                result.setResult($"{i}.0,{i + 1}.0,{i + 2}.0");
                resultPackage.Add(i, result, 0);
            }
            
            // Assert
            Assert.That(5, Is.EqualTo(resultPackage.results_count));
            
            for (int i = 0; i < 5; i++)
            {
                var retrievedResult = resultPackage.getResult(i);
                Assert.That(retrievedResult, !Is.Null);
                Assert.That($"{i}.0,{i + 1}.0,{i + 2}.0", Is.EqualTo(retrievedResult.getResult()));
            }
        }

        /// <summary>
        /// Tests that results count is maintained correctly.
        /// </summary>
        [Test]
        public void ResultsCount_IncreasesWithAdditions()
        {
            // Arrange
            resultPackage.setState(true);
            Assert.That(0, Is.EqualTo(resultPackage.results_count));
            
            // Act & Assert
            for (int i = 0; i < 10; i++)
            {
                var result = new Results();
                result.setResult($"test_{i}");
                resultPackage.Add(i, result, 0);
                Assert.That(i + 1, Is.EqualTo(resultPackage.results_count));
            }
        }

        /// <summary>
        /// Tests retrieving a result that doesn't exist.
        /// </summary>
        [Test]
        public void GetResult_NonExistentIndex_ReturnsNull()
        {
            // Arrange
            resultPackage.setState(true);
            
            // Act
            var result = resultPackage.getResult(999);
            
            // Assert
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Tests that negative indices are handled gracefully.
        /// </summary>
        [Test]
        public void GetResult_NegativeIndex_ReturnsNull()
        {
            // Arrange
            resultPackage.setState(true);
            
            // Act
            var result = resultPackage.getResult(-1);
            
            // Assert
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Tests preview result functionality.
        /// </summary>
        [Test]
        public void PreviewResult_SetAndGet_WorksCorrectly()
        {
            // Arrange
            var previewResult = new Results();
            previewResult.setResult("preview,data,test");
            
            // Act
            resultPackage.Add(0, previewResult, 0);
            var retrieved = resultPackage.getPreviewResult();
            
            // Assert
            Assert.That(retrieved, !Is.Null);
            Assert.That("preview,data,test", Is.EqualTo(retrieved.getResult()));
        }

        /// <summary>
        /// Tests that preview result is initially null.
        /// </summary>
        [Test]
        public void PreviewResult_InitialValue_IsNull()
        {
            // Act
            var result = resultPackage.getPreviewResult();
            
            // Assert
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Tests thread safety of state operations.
        /// </summary>
        [Test]
        public void State_ThreadSafety_NoDataRace()
        {
            // Arrange
            const int threadCount = 10;
            const int operationsPerThread = 100;
            var threads = new Thread[threadCount];
            var exceptions = new Exception[threadCount];
            
            // Act
            for (int t = 0; t < threadCount; t++)
            {
                int threadIndex = t;
                threads[t] = new Thread(() =>
                {
                    try
                    {
                        for (int i = 0; i < operationsPerThread; i++)
                        {
                            resultPackage.setState(i % 2 == 0);
                            var state = resultPackage.getState();
                            // Just verify we can read the state without exceptions
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptions[threadIndex] = ex;
                    }
                });
                threads[t].Start();
            }
            
            // Wait for all threads to complete
            for (int t = 0; t < threadCount; t++)
            {
                threads[t].Join(TimeSpan.FromSeconds(10));
            }
            
            // Assert
            for (int t = 0; t < threadCount; t++)
            {
                Assert.That(exceptions[t], Is.Null,$"Thread {t} threw exception: {exceptions[t]}");
            }
        }

        /// <summary>
        /// Tests that dispose cleans up resources properly.
        /// </summary>
        [Test]
        public void Dispose_CleansUpResources()
        {
            // Arrange
            var disposablePackage = new SimResultPackage();
            disposablePackage.setState(true);
            
            // Act
            Assert.DoesNotThrow(() => disposablePackage.Dispose());
            
            // Multiple dispose calls should not throw
            Assert.DoesNotThrow(() => disposablePackage.Dispose());
        }

        /// <summary>
        /// Tests adding results with empty/null data.
        /// </summary>
        [Test]
        public void AddResult_EmptyData_HandledGracefully()
        {
            // Arrange
            resultPackage.setState(true);
            var result = new Results();
            result.setResult("");
            
            // Act
            resultPackage.Add(0, result, 0);
            
            // Assert
            Assert.That(1, Is.EqualTo(resultPackage.results_count));
            var retrievedResult = resultPackage.getResult(0);
            Assert.That(retrievedResult, !Is.Null);
            Assert.That("", Is.EqualTo(retrievedResult.getResult()));
        }

        /// <summary>
        /// Tests large dataset handling.
        /// </summary>
        [Test]
        public void AddResult_LargeDataset_HandledEfficiently()
        {
            // Arrange
            resultPackage.setState(true);
            const int resultCount = 1000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Act
            for (int i = 0; i < resultCount; i++)
            {
                var result = new Results();
                result.setResult($"{i},{i * 1.5},{i * 2.0}");
                resultPackage.Add(i, result, 0);
            }
            
            stopwatch.Stop();
            
            // Assert
            Assert.That(resultCount, Is.EqualTo(resultPackage.results_count));
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1000), "Adding 1000 results should take less than 1 second");
            
            // Verify a few random results
            var result500 = resultPackage.getResult(500);
            Assert.That(result500, !Is.Null);
            Assert.That("500,750,1000", Is.EqualTo(result500.getResult()));
        }

        /// <summary>
        /// Tests that results maintain order when added out of sequence.
        /// </summary>
        [Test]
        public void AddResult_OutOfSequence_MaintainsCorrectMapping()
        {
            // Arrange
            resultPackage.setState(true);
            var indices = new[] { 2, 0, 4, 1, 3 };
            
            // Act
            foreach (int index in indices)
            {
                var result = new Results();
                result.setResult($"result_{index}");
                resultPackage.Add(index, result, 0);
            }
            
            // Assert
            Assert.That(5, Is.EqualTo(resultPackage.results_count));
            
            for (int i = 0; i < 5; i++)
            {
                var result = resultPackage.getResult(i);
                Assert.That(result, !Is.Null, $"Result at index {i} should not be null");
                Assert.That($"result_{i}", Is.EqualTo(result.getResult()), $"Result at index {i} has wrong content");
            }
        }
    }
}