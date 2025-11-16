using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Lab6.Core.Base
{
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        protected IWebElement FindElement(By locator)
        {
            return Wait.Until(driver => driver.FindElement(locator));
        }

        protected bool IsElementDisplayed(By locator)
        {
            try
            {
                var element = Driver.FindElement(locator);
                return element.Displayed;
            }
            catch
            {
                return false;
            }
        }

        protected void WaitForElementVisible(By locator, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(locator);
                    return element.Displayed;
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}

