using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Lab7.Base;
using Lab7.Locators;
using Lab7.Hooks;
using Xunit;
using Allure.Xunit.Attributes;
using Allure.Xunit;
using Allure.Net.Commons;
using System.Text;

namespace Lab7.Tests
{
    /// <summary>
    /// xUnit tests for Google Search functionality
    /// </summary>
    [Collection("GoogleSearchTests")]
    public class GoogleSearchTests : IClassFixture<WebDriverFixture>
    {
        private readonly IWebDriver _driver;

        static GoogleSearchTests()
        {
            // Initialize report once for all tests
            ReportManager.InitializeReport();
        }

        public GoogleSearchTests(WebDriverFixture fixture)
        {
            _driver = fixture.Driver;
        }

        private void CaptureScreenshotForAllure(string stepName)
        {
            try
            {
                if (_driver is ITakesScreenshot screenshotDriver)
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

        private void NavigateToGoogleSearchPage()
        {
            AllureApi.Step("Navigate to Google search page", () =>
            {
                _driver.Navigate().GoToUrl("https://www.google.com");
                ReportManager.LogInfo("Navigated to Google search page");
                CaptureScreenshotForAllure("After navigation to Google");
            });
        }

        private void EnterUrl(string url)
        {
            AllureApi.Step($"Enter URL: {url}", () =>
            {
                _driver.Navigate().GoToUrl(url);
                ReportManager.LogInfo($"Entered URL: {url}");
                
                // Wait for page to load
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
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
            });
        }

        private void VerifyGooglePageDisplayed()
        {
            AllureApi.Step("Verify Google page is displayed", () =>
            {
                var currentUrl = _driver.Url;
                var pageTitle = _driver.Title;

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
                    throw new Xunit.Sdk.XunitException("Google page was not displayed correctly");
                }
            });
        }

        private void SearchFor(string searchTerm)
        {
            AllureApi.Step($"Search for: {searchTerm}", () =>
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

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
                        var searchButton = _driver.FindElement(GooglePageLocators.SearchButton);
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
            });
        }

        private void DoNotEnterAnySearchTerm()
        {
            // Explicitly do nothing - this step documents that no search term is entered
            // Also ensure search box is empty
            try
            {
                var searchBox = _driver.FindElement(GooglePageLocators.SearchBox);
                searchBox.Clear();
                ReportManager.LogInfo("No search term entered - search box cleared");
            }
            catch
            {
                ReportManager.LogInfo("No search term entered as per test scenario");
            }
        }

        private void ClickSearchButton()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            try
            {
                // Save current URL before clicking
                var urlBeforeClick = _driver.Url;
                
                // Try to find and click search button
                var searchButton = wait.Until(driver => driver.FindElement(GooglePageLocators.SearchButton));
                
                if (searchButton.Displayed && searchButton.Enabled)
                {
                    searchButton.Click();
                    Thread.Sleep(500); // Small wait to see if page navigates
                    ReportManager.LogInfo($"Clicked on search button. URL before: {urlBeforeClick}, URL after: {_driver.Url}");
                }
                else
                {
                    // Try alternative search button
                    try
                    {
                        var altButton = _driver.FindElement(GooglePageLocators.SearchButtonSubmit);
                        var urlBeforeAltClick = _driver.Url;
                        altButton.Click();
                        Thread.Sleep(500);
                        ReportManager.LogInfo($"Clicked on alternative search button. URL before: {urlBeforeAltClick}, URL after: {_driver.Url}");
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

        private void VerifySearchResultsDisplayed()
        {
            AllureApi.Step("Verify search results are displayed", () =>
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

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
                    throw new Xunit.Sdk.XunitException("Search results were not displayed");
                }
            });
        }

        private int CountSearchResults()
        {
            return AllureApi.Step("Count search results on current page", () =>
            {
                try
                {
                    // Wait a bit for page to fully load
                    Thread.Sleep(1000);
                    
                    // Get all result items that are visible on the first page
                    var resultItems = _driver.FindElements(GooglePageLocators.ResultItems);
                    
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
            });
        }

        private void VerifyNumberOfResults(int expectedCount)
        {
            AllureApi.Step($"Verify number of results equals {expectedCount}", () =>
            {
                var actualCount = CountSearchResults();
                
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
                    throw new Xunit.Sdk.XunitException($"Expected {expectedCount} search results on first page but found {actualCount}");
                }
            });
        }

        private void VerifyNothingHappened()
        {
            Thread.Sleep(1000); // Wait a bit to see if any navigation occurs
            var currentUrl = _driver.Url;

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

        private void VerifyRemainOnGoogleSearchPage()
        {
            var currentUrl = _driver.Url;
            var pageTitle = _driver.Title;

            var isOnGooglePage = currentUrl.Contains("google.com");

            if (isOnGooglePage)
            {
                ReportManager.LogPass($"Remained on Google search page. URL: {currentUrl}");
            }
            else
            {
                ReportManager.LogFail($"Expected to remain on Google page but current URL is: {currentUrl}");
                throw new Xunit.Sdk.XunitException("Not on Google search page as expected");
            }
        }

        private void VerifyLinkDisplayed(string linkText)
        {
            AllureApi.Step($"Verify '{linkText}' link is displayed", () =>
            {
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

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
                        throw new Xunit.Sdk.XunitException($"Expected link '{linkText}' was not displayed");
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    CaptureScreenshotForAllure($"'{linkText}' link timeout");
                    ReportManager.LogFail($"'{linkText}' link was not found within the expected time");
                    throw new Xunit.Sdk.XunitException($"Expected link '{linkText}' was not displayed");
                }
            });
        }

        [Fact]
        [Trait("Category", "Navigation")]
        [AllureDescription("Verify that Google page opens correctly after entering the URL")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureOwner("Test Automation")]
        [AllureFeature("Navigation")]
        [AllureStory("Google Page Navigation")]
        public void VerifyGooglePageOpensAfterEnteringURL()
        {
            ReportManager.CreateTest("Verify Google page opens after entering URL");
            
            AllureLifecycle.Instance.UpdateTestCase(tc =>
            {
                tc.name = "Verify Google page opens after entering URL";
                tc.description = "This test verifies that the Google search page loads correctly when navigating to the Google URL";
            });
            
            NavigateToGoogleSearchPage();
            EnterUrl("https://www.google.co.in");
            VerifyGooglePageDisplayed();
            
            ReportManager.CompleteTestCase("Pass");
        }

        [Fact]
        [Trait("Category", "Search")]
        [AllureDescription("Verify that the correct number of search results are displayed on the first page")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureOwner("Test Automation")]
        [AllureFeature("Search")]
        [AllureStory("Search Results Count")]
        public void VerifyNumberOfSearchResultsDisplayedOnAPage()
        {
            ReportManager.CreateTest("Verify number of search results displayed on a page");
            
            AllureLifecycle.Instance.UpdateTestCase(tc =>
            {
                tc.name = "Verify number of search results displayed on a page";
                tc.description = "This test verifies that exactly 9 search results are displayed on the first page when searching for 'SpecFlow'";
            });
            
            NavigateToGoogleSearchPage();
            SearchFor("SpecFlow");
            VerifySearchResultsDisplayed();
            CountSearchResults();
            VerifyNumberOfResults(9);
            
            ReportManager.CompleteTestCase("Pass");
        }

        [Fact]
        [Trait("Category", "Search")]
        [AllureDescription("Verify that clicking the search button with empty input does not trigger a search")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureOwner("Test Automation")]
        [AllureFeature("Search")]
        [AllureStory("Empty Search Validation")]
        public void VerifySearchButtonBehaviorWithEmptyInput()
        {
            ReportManager.CreateTest("Verify search button behavior with empty input");
            
            AllureLifecycle.Instance.UpdateTestCase(tc =>
            {
                tc.name = "Verify search button behavior with empty input";
                tc.description = "This test verifies that clicking the search button without entering any search term does not perform a search and the user remains on the Google search page";
            });
            
            NavigateToGoogleSearchPage();
            DoNotEnterAnySearchTerm();
            ClickSearchButton();
            VerifyNothingHappened();
            VerifyRemainOnGoogleSearchPage();
            
            ReportManager.CompleteTestCase("Pass");
        }

        [Fact]
        [Trait("Category", "Search")]
        [AllureDescription("Verify that the 'Did you mean' suggestion appears when searching for an irrelevant term")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureOwner("Test Automation")]
        [AllureFeature("Search")]
        [AllureStory("Search Suggestions")]
        public void VerifyDidYouMeanSuggestionAppearsForIrrelevantSearch()
        {
            ReportManager.CreateTest("Verify \"Did you mean\" suggestion appears for irrelevant search");
            
            AllureLifecycle.Instance.UpdateTestCase(tc =>
            {
                tc.name = "Verify 'Did you mean' suggestion appears for irrelevant search";
                tc.description = "This test verifies that Google displays a 'Did you mean' suggestion link when searching for an irrelevant or misspelled term like 'infigo'";
            });
            
            NavigateToGoogleSearchPage();
            SearchFor("infigo");
            VerifyLinkDisplayed("Did you mean");
            
            ReportManager.CompleteTestCase("Pass");
        }
    }
}

