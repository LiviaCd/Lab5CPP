using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;

namespace Lab5CPP
{
    public class YouTubeTest
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public YouTubeTest(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        }

        public void RunTest()
        {
            Logger.LogStep("TEST START", LogType.Highlight);

            try
            {
                OpenYouTube();
                SearchForTerm("computer");
                VerifyHeader();
            }
            catch (NoSuchElementException e)
            {
                Logger.LogStep($"Element not found: {e.Message}", LogType.Error);
            }
            catch (WebDriverTimeoutException)
            {
                Logger.LogStep("Timeout waiting for element", LogType.Error);
            }
            catch (Exception ex)
            {
                Logger.LogStep($"Unexpected error: {ex.Message}", LogType.Error);
            }
            finally
            {
                _driver.Quit();
                Logger.LogStep("TEST END", LogType.Highlight);
            }
        }

        private void OpenYouTube()
        {
            _driver.Navigate().GoToUrl("https://www.youtube.com");
            _driver.Manage().Window.Maximize();
            Logger.LogStep("YouTube opened successfully", LogType.Success);
        }

        private void SearchForTerm(string term)
        {
            var searchBox = _wait.Until(d => d.FindElement(By.Name("search_query")));
            Logger.LogStep("Search box found", LogType.Success);

            searchBox.SendKeys(term);
            searchBox.Submit();
            Logger.LogStep($"Search for '{term}' performed", LogType.Success);

            _wait.Until(d => d.FindElement(By.Id("contents")));
            Logger.LogStep("Search results loaded", LogType.Success);
        }

        private void VerifyHeader()
        {
            var header = _driver.FindElement(By.Id("masthead-container"));
            if (header.Displayed)
                Logger.LogStep("YouTube header is displayed correctly", LogType.Success);
            else
                Logger.LogStep("YouTube header is NOT displayed", LogType.Error);
        }
    }
}
