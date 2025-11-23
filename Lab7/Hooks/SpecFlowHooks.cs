using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using TechTalk.SpecFlow;
using Lab7.Base;
using Allure.Net.Commons;
using System.Text;

namespace Lab7.Hooks
{
    /// <summary>
    /// SpecFlow hooks for WebDriver lifecycle management
    /// </summary>
    [Binding]
    public class SpecFlowHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private static string? _currentAllureTestUuid;

        public SpecFlowHooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // Initialize report before any scenarios run
            ReportManager.InitializeReport();
            
            // Ensure allure-results directory exists
            var allureResultsDir = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");
            if (!Directory.Exists(allureResultsDir))
            {
                Directory.CreateDirectory(allureResultsDir);
            }
        }

        [BeforeScenario(Order = 0)]
        public void BeforeScenario()
        {
            // Initialize Allure test result
            _currentAllureTestUuid = Guid.NewGuid().ToString();
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            var scenarioDescription = _scenarioContext.ScenarioInfo.Description;
            var featureTitle = _featureContext.FeatureInfo.Title;
            
            AllureLifecycle.Instance.StartTestCase(new TestResult
            {
                uuid = _currentAllureTestUuid,
                name = scenarioTitle,
                description = string.IsNullOrEmpty(scenarioDescription) ? scenarioTitle : scenarioDescription,
                fullName = $"{featureTitle}::{scenarioTitle}",
                historyId = _currentAllureTestUuid,
                labels = new List<Label>
                {
                    Label.Feature(featureTitle),
                    Label.Suite("SpecFlow Tests"),
                    Label.TestClass("GoogleSearchTests")
                },
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            });

            // Store Allure UUID in scenario context
            _scenarioContext.Set(_currentAllureTestUuid, "AllureTestUuid");

            // Automatically download and setup ChromeDriver matching installed Chrome version
            new DriverManager().SetUpDriver(new ChromeConfig());

            var options = new ChromeOptions();
            
            // Basic options
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            
            // Set browser language to English - multiple methods for maximum compatibility
            options.AddArgument("--lang=en-US");
            options.AddArgument("--accept-lang=en-US,en");
            options.AddUserProfilePreference("intl.accept_languages", "en-US,en");
            options.AddUserProfilePreference("intl.selected_languages", "en-US,en");
            options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
            
            // Options to avoid bot detection
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            
            // Set a real user agent to avoid detection (with English language)
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36");

            var driver = new ChromeDriver(options);
            
            // Execute JavaScript to hide webdriver property and set language preferences
            var executor = driver as IJavaScriptExecutor;
            executor?.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
            
            // Override navigator language properties to force English
            executor?.ExecuteScript(@"
                Object.defineProperty(navigator, 'language', {get: () => 'en-US'});
                Object.defineProperty(navigator, 'languages', {get: () => ['en-US', 'en']});
                Object.defineProperty(navigator, 'userLanguage', {get: () => 'en-US'});
            ");
            
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

            // Store driver in scenario context
            _scenarioContext.Set<IWebDriver>(driver, "WebDriver");

            // Initialize test case for reporting
            ReportManager.CreateTest(scenarioTitle, scenarioDescription);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            var driver = _scenarioContext.Get<IWebDriver>("WebDriver");
            
            // Complete test case reporting
            var scenarioStatus = _scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK ? "Pass" : "Fail";
            ReportManager.CompleteTestCase(scenarioStatus);
            
            // Complete Allure test result
            var allureUuid = _scenarioContext.ContainsKey("AllureTestUuid") 
                ? _scenarioContext.Get<string>("AllureTestUuid") 
                : _currentAllureTestUuid;
            
            if (!string.IsNullOrEmpty(allureUuid))
            {
                try
                {
                    // Reactivate the test context by starting the test case with the same UUID
                    // This ensures the context is active for UpdateTestCase
                    var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
                    var scenarioDescription = _scenarioContext.ScenarioInfo.Description;
                    var featureTitle = _featureContext.FeatureInfo.Title;
                    
                    // Start test case again to activate context
                    AllureLifecycle.Instance.StartTestCase(new TestResult
                    {
                        uuid = allureUuid,
                        name = scenarioTitle,
                        description = string.IsNullOrEmpty(scenarioDescription) ? scenarioTitle : scenarioDescription,
                        fullName = $"{featureTitle}::{scenarioTitle}",
                        historyId = allureUuid,
                        labels = new List<Label>
                        {
                            Label.Feature(featureTitle),
                            Label.Suite("SpecFlow Tests"),
                            Label.TestClass("GoogleSearchTests")
                        }
                    });
                    
                    // Now update the test result with final status
                    AllureLifecycle.Instance.UpdateTestCase(testResult =>
                    {
                        testResult.status = _scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK 
                            ? Status.passed 
                            : Status.failed;
                        testResult.statusDetails = _scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK
                            ? null
                            : new StatusDetails
                            {
                                message = "Test scenario failed",
                                trace = _scenarioContext.TestError?.ToString() ?? "Unknown error"
                            };
                        testResult.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    });
                    
                    // Stop and write the test case
                    AllureLifecycle.Instance.StopTestCase();
                    AllureLifecycle.Instance.WriteTestCase();
                }
                catch (Exception ex)
                {
                    // If Allure operations fail, log but don't break the test
                    Console.WriteLine($"[WARN] Could not complete Allure test result: {ex.Message}");
                }
            }
            
            // Cleanup WebDriver
            driver?.Quit();
            driver?.Dispose();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            // Flush report after all scenarios complete
            ReportManager.FlushReport();
        }
    }
}
