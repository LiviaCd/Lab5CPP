using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Lab6.Helpers
{
    public static class WaitHelper
    {
        public static WebDriverWait CreateWait(IWebDriver driver, int timeoutSeconds = 15)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        }

        public static IWebElement WaitForElementVisible(IWebDriver driver, By selector, int timeoutSeconds = 15)
        {
            var wait = CreateWait(driver, timeoutSeconds);
            return wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(selector);
                    return element.Displayed ? element : null;
                }
                catch
                {
                    return null;
                }
            });
        }

        public static void WaitForModalVisible(IWebDriver driver, string modalId, int timeoutSeconds = 15)
        {
            var wait = CreateWait(driver, timeoutSeconds);
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
        }

        public static bool IsElementVisible(IWebDriver driver, By selector, int timeoutSeconds = 15)
        {
            try
            {
                var element = WaitForElementVisible(driver, selector, timeoutSeconds);
                return element != null;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}

