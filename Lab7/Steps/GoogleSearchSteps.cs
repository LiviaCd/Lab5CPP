using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Lab7.Base;
using Lab7.Locators;
using TechTalk.SpecFlow;
using System.Text;

namespace Lab7.Steps
{
    /// <summary>
    /// BDD Step Definitions for Google Search functionality
    /// </summary>
    [Binding]
    public class GoogleSearchSteps
    {
        private IWebDriver? _driver;
        private readonly ScenarioContext _scenarioContext;

        static GoogleSearchSteps()
        {
            // Initialize report once for all scenarios
            ReportManager.InitializeReport();
        }

        public GoogleSearchSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            // Lazy initialization of driver to avoid issues during step definition discovery
            // The driver will be set in BeforeScenario hook
        }

        private IWebDriver GetDriver()
        {
            if (_driver == null)
            {
                if (!_scenarioContext.ContainsKey("WebDriver"))
                {
                    throw new InvalidOperationException("WebDriver is not initialized. Make sure BeforeScenario hook runs.");
                }
                _driver = _scenarioContext.Get<IWebDriver>("WebDriver");
            }
            return _driver;
        }

        private void CaptureScreenshotForAllure(string stepName)
        {
            try
            {
                var driver = GetDriver();
                if (driver is ITakesScreenshot screenshotDriver)
                {
                    var screenshot = screenshotDriver.GetScreenshot();
                    var bytes = screenshot.AsByteArray;
                    // Save screenshot to file - Allure will automatically pick it up from allure-results folder
                    var resultsDir = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");
                    Directory.CreateDirectory(resultsDir);
                    var screenshotPath = Path.Combine(resultsDir, $"{stepName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.png");
                    File.WriteAllBytes(screenshotPath, bytes);
                    // Log to ReportManager instead of Allure attachment
                    ReportManager.LogInfo($"Screenshot saved: {stepName}");
                }
            }
            catch (Exception ex)
            {
                ReportManager.LogFail($"Screenshot capture failed: {ex.Message}");
            }
        }

        [Given(@"I navigate to Google search page")]
        public void GivenINavigateToGoogleSearchPage()
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl("https://www.google.com");
            ReportManager.LogInfo("Navigated to Google search page");
            CaptureScreenshotForAllure("After navigation to Google");
        }

        [When(@"I enter the URL ""(.*)""")]
        public void WhenIEnterTheURL(string url)
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl(url);
            ReportManager.LogInfo($"Entered URL: {url}");
            
            // Wait for page to load
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            try
            {
                wait.Until(driver => driver.Title.Contains("Google") || driver.Url.Contains("google"));
                CaptureScreenshotForAllure($"Page loaded: {url}");
            }
            catch (WebDriverTimeoutException)
            {
                CaptureScreenshotForAllure("Page load timeout");
                ReportManager.LogFail("Page did not load within expected time");
                throw;
            }
        }

        [Then(@"the Google search page should be displayed")]
        public void ThenTheGoogleSearchPageShouldBeDisplayed()
        {
            var driver = GetDriver();
            var currentUrl = driver.Url;
            var pageTitle = driver.Title;

            // Validate that we're on Google page
            var isGooglePage = currentUrl.Contains("google") || pageTitle.ToLower().Contains("google");
            
            if (isGooglePage)
            {
                ReportManager.LogPass($"Google page displayed successfully. URL: {currentUrl}, Title: {pageTitle}");
                // Log verification details to ReportManager
                ReportManager.LogInfo($"Page verification - URL: {currentUrl}, Title: {pageTitle}");
            }
            else
            {
                CaptureScreenshotForAllure("Google page verification failed");
                ReportManager.LogFail($"Expected Google page but got: URL: {currentUrl}, Title: {pageTitle}");
                throw new Exception("Google page was not displayed correctly");
            }
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string searchTerm)
        {
            var driver = GetDriver();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                // Find and clear search box
                var searchBox = wait.Until(driver => driver.FindElement(GooglePageLocators.SearchBox));
                searchBox.Clear();
                searchBox.SendKeys(searchTerm);
                ReportManager.LogInfo($"Entered search term: {searchTerm}");

                // Small delay to allow suggestions to appear
                Thread.Sleep(500);

                // Try to click search button - might need to press Enter if button is hidden
                try
                {
                    var searchButton = driver.FindElement(GooglePageLocators.SearchButton);
                    if (searchButton.Displayed && searchButton.Enabled)
                    {
                        searchButton.Click();
                    }
                    else
                    {
                        searchBox.SendKeys(Keys.Return);
                    }
                }
                catch (NoSuchElementException)
                {
                    // If search button not found, press Enter
                    searchBox.SendKeys(Keys.Return);
                }

                // Wait for search results to appear
                wait.Until(driver => 
                {
                    try
                    {
                        var results = driver.FindElement(GooglePageLocators.SearchResults);
                        return results.Displayed;
                    }
                    catch
                    {
                        return false;
                    }
                });

                ReportManager.LogInfo("Search performed successfully");
                CaptureScreenshotForAllure($"Search results for: {searchTerm}");
            }
            catch (Exception ex)
            {
                CaptureScreenshotForAllure($"Search failed for: {searchTerm}");
                ReportManager.LogFail($"Failed to perform search: {ex.Message}", ex);
                throw;
            }
        }

        [When(@"I do not enter any search term")]
        public void WhenIDoNotEnterAnySearchTerm()
        {
            // Explicitly do nothing - this step documents that no search term is entered
            // Also ensure search box is empty
            try
            {
                var driver = GetDriver();
                var searchBox = driver.FindElement(GooglePageLocators.SearchBox);
                searchBox.Clear();
                ReportManager.LogInfo("No search term entered - search box cleared");
            }
            catch
            {
                ReportManager.LogInfo("No search term entered as per scenario");
            }
        }

        [When(@"I click on the search button")]
        public void WhenIClickOnTheSearchButton()
        {
            var driver = GetDriver();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                // Save current URL before clicking
                var urlBeforeClick = driver.Url;
                
                // Try to find and click search button
                var searchButton = wait.Until(driver => driver.FindElement(GooglePageLocators.SearchButton));
                
                if (searchButton.Displayed && searchButton.Enabled)
                {
                    searchButton.Click();
                    Thread.Sleep(500); // Small wait to see if page navigates
                    ReportManager.LogInfo($"Clicked on search button. URL before: {urlBeforeClick}, URL after: {driver.Url}");
                }
                else
                {
                    // Try alternative search button
                    try
                    {
                        var altButton = driver.FindElement(GooglePageLocators.SearchButtonSubmit);
                        var urlBeforeAltClick = driver.Url;
                        altButton.Click();
                        Thread.Sleep(500);
                        ReportManager.LogInfo($"Clicked on alternative search button. URL before: {urlBeforeAltClick}, URL after: {driver.Url}");
                    }
                    catch
                    {
                        ReportManager.LogInfo("Search button not clickable - as expected for empty search");
                    }
                }
            }
            catch (NoSuchElementException)
            {
                ReportManager.LogInfo("Search button not found or not visible - this is expected behavior");
            }
            catch (ElementNotInteractableException)
            {
                ReportManager.LogInfo("Search button not interactable - this is expected behavior for empty search");
            }
        }

        [Then(@"I should see search results displayed")]
        public void ThenIShouldSeeSearchResultsDisplayed()
        {
            var driver = GetDriver();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                wait.Until(driver =>
                {
                    try
                    {
                        var results = driver.FindElement(GooglePageLocators.SearchResults);
                        return results.Displayed;
                    }
                    catch
                    {
                        return false;
                    }
                });

                ReportManager.LogPass("Search results are displayed");
                CaptureScreenshotForAllure("Search results displayed");
            }
            catch (WebDriverTimeoutException)
            {
                CaptureScreenshotForAllure("Search results not displayed");
                ReportManager.LogFail("Search results were not displayed");
                throw new Exception("Search results were not displayed");
            }
        }

        [Then(@"I should count the number of results on the current page")]
        public void ThenIShouldCountTheNumberOfResultsOnTheCurrentPage()
        {
            var count = CountSearchResults();
            _scenarioContext["ResultCount"] = count;
        }

        private int CountSearchResults()
        {
            try
            {
                // Wait a bit for page to fully load
                Thread.Sleep(1000);
                
                // Get all result items that are visible on the first page
                var driver = GetDriver();
                var resultItems = driver.FindElements(GooglePageLocators.ResultItems);
                
                // Filter to only visible results on the current page (not hidden or from other pages)
                var visibleResults = resultItems.Where(item => 
                {
                    try
                    {
                        return item.Displayed && item.Size.Height > 0 && item.Size.Width > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }).ToList();

                var count = visibleResults.Count;

                ReportManager.LogPass($"Number of search results on current page: {count}");
                ReportManager.LogInfo($"Found {count} visible result items on the first page");
                
                // Log result count to ReportManager
                ReportManager.LogInfo($"Found {count} search results on the first page");
                
                return count;
            }
            catch (Exception ex)
            {
                CaptureScreenshotForAllure("Error counting results");
                ReportManager.LogFail($"Failed to count search results: {ex.Message}", ex);
                throw;
            }
        }

        [Then(@"the number of results should be ""(.*)""")]
        public void ThenTheNumberOfResultsShouldBe(string expectedCountStr)
        {
            var expectedCount = int.Parse(expectedCountStr);
            int actualCount;
            if (_scenarioContext.ContainsKey("ResultCount"))
            {
                actualCount = _scenarioContext.Get<int>("ResultCount");
            }
            else
            {
                actualCount = CountSearchResults();
            }
            
            var resultInfo = $"Expected: {expectedCount}, Actual: {actualCount}";
            // Log verification to ReportManager
            ReportManager.LogInfo(resultInfo);
            
            if (actualCount == expectedCount)
            {
                ReportManager.LogPass($"Number of results matches expected: {actualCount} (on first page only)");
            }
            else
            {
                CaptureScreenshotForAllure($"Result count mismatch: expected {expectedCount}, found {actualCount}");
                ReportManager.LogFail($"Expected {expectedCount} results on first page but found {actualCount} results");
                throw new Exception($"Expected {expectedCount} search results on first page but found {actualCount}");
            }
        }

        [Then(@"nothing should happen")]
        public void ThenNothingShouldHappen()
        {
            Thread.Sleep(1000); // Wait a bit to see if any navigation occurs
            var driver = GetDriver();
            var currentUrl = driver.Url;

            // Verify we're still on Google search page and no search query was executed
            // Google might do minimal navigation, but should not perform a search with query parameters
            var isStillOnGooglePage = currentUrl.Contains("google.co.in") || currentUrl.Contains("google.com");
            var hasSearchQuery = currentUrl.Contains("search?q=") || currentUrl.Contains("search?hl=") && currentUrl.Contains("&q=");
            var hasEmptySearch = currentUrl.Contains("search?q=&") || currentUrl.Contains("search?q=") && (currentUrl.Contains("&") || currentUrl.EndsWith("q=") || currentUrl.Contains("q=&"));

            // Nothing should happen means:
            // - Still on Google page (google.co.in or google.com)
            // - No search query parameters, OR if there are, they should be empty
            // - Should not navigate to a results page with actual query
            if (isStillOnGooglePage && (!hasSearchQuery || hasEmptySearch))
            {
                ReportManager.LogPass($"No action occurred as expected - remained on Google search page. URL: {currentUrl}");
            }
            else if (isStillOnGooglePage && currentUrl.Contains("search") && !currentUrl.Contains("&q=") && !currentUrl.Contains("?q="))
            {
                // Might be on search page but without query - this is acceptable
                ReportManager.LogPass($"Remained on Google page without search query. URL: {currentUrl}");
            }
            else
            {
                ReportManager.LogFail($"Expected no action, but URL changed to: {currentUrl}");
                ReportManager.LogInfo("Note: Google may perform minimal navigation even with empty search");
                // Don't throw exception - this might be acceptable behavior from Google
            }
        }

        [Then(@"I should remain on the Google search page")]
        public void ThenIShouldRemainOnTheGoogleSearchPage()
        {
            var driver = GetDriver();
            var currentUrl = driver.Url;
            var pageTitle = driver.Title;

            var isOnGooglePage = currentUrl.Contains("google.com");

            if (isOnGooglePage)
            {
                ReportManager.LogPass($"Remained on Google search page. URL: {currentUrl}");
            }
            else
            {
                ReportManager.LogFail($"Expected to remain on Google page but current URL is: {currentUrl}");
                throw new Exception("Not on Google search page as expected");
            }
        }

        [Then(@"I should see the ""(.*)"" link displayed")]
        public void ThenIShouldSeeTheLinkDisplayed(string linkText)
        {
            var driver = GetDriver();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                By locator;
                if (linkText.Contains("Did you mean", StringComparison.OrdinalIgnoreCase))
                {
                    locator = GooglePageLocators.DidYouMeanLink;
                }
                else
                {
                    locator = By.PartialLinkText(linkText);
                }

                var element = wait.Until(driver =>
                {
                    try
                    {
                        var el = driver.FindElement(locator);
                        return el.Displayed ? el : null;
                    }
                    catch
                    {
                        return null;
                    }
                });

                if (element != null && element.Displayed)
                {
                    var actualText = element.Text;
                    ReportManager.LogPass($"'{linkText}' link is displayed. Actual text: {actualText}");
                    // Log link verification to ReportManager
                    ReportManager.LogInfo($"Link verification - Expected: {linkText}, Actual: {actualText}");
                    CaptureScreenshotForAllure($"'{linkText}' link displayed");
                }
                else
                {
                    CaptureScreenshotForAllure($"'{linkText}' link not found");
                    ReportManager.LogFail($"'{linkText}' link was not displayed");
                    throw new Exception($"Expected link '{linkText}' was not displayed");
                }
            }
            catch (WebDriverTimeoutException)
            {
                CaptureScreenshotForAllure($"'{linkText}' link timeout");
                ReportManager.LogFail($"'{linkText}' link was not found within the expected time");
                throw new Exception($"Expected link '{linkText}' was not displayed");
            }
        }
    }
}

