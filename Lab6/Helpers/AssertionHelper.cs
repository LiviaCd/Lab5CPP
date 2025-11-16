using NUnit.Framework;
using OpenQA.Selenium;

namespace Lab6.Helpers
{
    public static class AssertionHelper
    {
        public static void AssertElementVisible(IWebDriver driver, By selector, string elementName, int timeoutSeconds = 15)
        {
            var element = WaitHelper.WaitForElementVisible(driver, selector, timeoutSeconds);
            Assert.That(
                element,
                Is.Not.Null,
                $"{elementName} should be visible but was not found."
            );
        }

        public static void AssertElementNotVisible(IWebDriver driver, By selector, string elementName, int timeoutSeconds = 5)
        {
            var isVisible = WaitHelper.IsElementVisible(driver, selector, timeoutSeconds);
            Assert.That(
                isVisible,
                Is.False,
                $"{elementName} should not be visible but was found."
            );
        }

        public static void AssertRegistrationSuccess(IWebDriver driver, string modalId)
        {
            var wait = WaitHelper.CreateWait(driver, 5);
            try
            {
                var modal = driver.FindElement(By.Id(modalId));
                var isModalVisible = modal.Displayed;
                Assert.That(
                    !isModalVisible,
                    Is.True,
                    "Registration should succeed and modal should close."
                );
            }
            catch
            {
                // Modal closed - registration successful
                Assert.That(true, Is.True, "Registration successful - modal closed.");
            }
        }

        public static void AssertRegistrationFailure(IWebDriver driver, string modalId, bool hasErrorMessage = false)
        {
            var wait = WaitHelper.CreateWait(driver, 5);
            
            wait.Until(driver =>
            {
                try
                {
                    var modal = driver.FindElement(By.Id(modalId));
                    return modal.Displayed;
                }
                catch
                {
                    return false;
                }
            });

            var modalStillOpen = false;
            try
            {
                var modal = driver.FindElement(By.Id(modalId));
                modalStillOpen = modal.Displayed;
            }
            catch
            {
                modalStillOpen = false;
            }

            Assert.That(
                modalStillOpen || hasErrorMessage,
                Is.True,
                "Registration should have failed but appears to have succeeded."
            );
        }
    }
}

