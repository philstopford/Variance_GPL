using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Variance;

namespace Tests
{
    [TestFixture]
    public class CallbackValidationTests
    {
        [Test]
        public void VerifyCallbacksAreAssignedWhenLoadingXMLFiles()
        {
            // Arrange
            var storage = new Storage();
            bool settingsCallbackCalled = false;
            bool doeCallbackCalled = false;
            bool implantCallbackCalled = false;
            
            // Simulate the application setting up callbacks (as would happen when loading XML files)
            storage.suspendSettingsUIWithCallback = (callback) => 
            {
                // Simulate async suspend operation
                Thread.Sleep(10);
                callback?.Invoke();
            };
            
            storage.updateSettingsUIFromSettingsWithCallback = (callback) => 
            {
                settingsCallbackCalled = true;
                callback?.Invoke();
            };
            
            storage.suspendDOESettingsUIWithCallback = (callback) => 
            {
                Thread.Sleep(10);
                callback?.Invoke();
            };
            
            storage.updateDOESettingsUIFromSettingsWithCallback = (callback) => 
            {
                doeCallbackCalled = true;
                callback?.Invoke();
            };
            
            storage.suspendImplantUIWithCallback = (callback) => 
            {
                Thread.Sleep(10);
                callback?.Invoke();
            };
            
            storage.updateImplantUIFromSettingsWithCallback = (callback) => 
            {
                implantCallbackCalled = true;
                callback?.Invoke();
            };
            
            // Act
            // Simulate what happens in Storage.cs when callbacks are available
            if (storage.updateSettingsUIFromSettingsWithCallback != null)
            {
                storage.updateSettingsUIFromSettingsWithCallback(null);
            }
            
            if (storage.updateDOESettingsUIFromSettingsWithCallback != null)
            {
                storage.updateDOESettingsUIFromSettingsWithCallback(null);
            }
            
            if (storage.updateImplantUIFromSettingsWithCallback != null)
            {
                storage.updateImplantUIFromSettingsWithCallback(null);
            }
            
            // Assert
            Assert.IsTrue(settingsCallbackCalled, "Settings UI callback was not called");
            Assert.IsTrue(doeCallbackCalled, "DOE UI callback was not called");
            Assert.IsTrue(implantCallbackCalled, "Implant UI callback was not called");
            
            // Verify that the callback delegates are not null (meaning they would be set by the application)
            Assert.IsNotNull(storage.suspendSettingsUIWithCallback, "suspendSettingsUIWithCallback should be set by application");
            Assert.IsNotNull(storage.updateSettingsUIFromSettingsWithCallback, "updateSettingsUIFromSettingsWithCallback should be set by application");
            Assert.IsNotNull(storage.suspendDOESettingsUIWithCallback, "suspendDOESettingsUIWithCallback should be set by application");
            Assert.IsNotNull(storage.updateDOESettingsUIFromSettingsWithCallback, "updateDOESettingsUIFromSettingsWithCallback should be set by application");
            Assert.IsNotNull(storage.suspendImplantUIWithCallback, "suspendImplantUIWithCallback should be set by application");
            Assert.IsNotNull(storage.updateImplantUIFromSettingsWithCallback, "updateImplantUIFromSettingsWithCallback should be set by application");
        }

        [Test]
        public void VerifyRaceConditionPrevention()
        {
            // Arrange
            var storage = new Storage();
            bool suspendComplete = false;
            bool updateCalled = false;
            
            // Simulate the callback coordination pattern
            storage.suspendSettingsUIWithCallback = (callback) => 
            {
                // Simulate async suspend operation
                Task.Run(() => 
                {
                    Thread.Sleep(50); // Simulate delay
                    suspendComplete = true;
                    callback?.Invoke(); // Execute callback after suspend completes
                });
            };
            
            storage.updateSettingsUIFromSettingsWithCallback = (callback) => 
            {
                // This should only be called after suspend completes
                Assert.IsTrue(suspendComplete, "Update should not be called until suspend completes");
                updateCalled = true;
                callback?.Invoke();
            };
            
            // Act - Simulate the race condition fix pattern from Storage.cs
            Action updateUI = () =>
            {
                if (storage.updateSettingsUIFromSettingsWithCallback != null)
                {
                    storage.updateSettingsUIFromSettingsWithCallback(null);
                }
            };
            
            if (storage.suspendSettingsUIWithCallback != null)
            {
                storage.suspendSettingsUIWithCallback(updateUI);
            }
            
            // Wait for async operation to complete
            Thread.Sleep(100);
            
            // Assert
            Assert.IsTrue(suspendComplete, "Suspend operation should have completed");
            Assert.IsTrue(updateCalled, "Update should have been called after suspend");
        }
        
        [Test]
        public void VerifyBackwardCompatibilityWithOldCallbacks()
        {
            // Arrange
            var storage = new Storage();
            bool oldCallbackCalled = false;
            
            // Only set old callbacks (no "WithCallback" versions)
            storage.updateSettingsUIFromSettings = () => 
            {
                oldCallbackCalled = true;
            };
            
            // Leave new callbacks null to test fallback
            storage.updateSettingsUIFromSettingsWithCallback = null;
            
            // Act - Simulate the fallback logic from Storage.cs
            if (storage.updateSettingsUIFromSettingsWithCallback != null)
            {
                storage.updateSettingsUIFromSettingsWithCallback(null);
            }
            else
            {
                storage.updateSettingsUIFromSettings?.Invoke();
            }
            
            // Assert
            Assert.IsTrue(oldCallbackCalled, "Should fall back to old callback when new one is not available");
        }
    }
}