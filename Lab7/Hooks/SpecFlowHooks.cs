using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using TechTalk.SpecFlow;
using Lab7.Base;

namespace Lab7.Hooks
{
    /// <summary>
    /// SpecFlow hooks for WebDriver lifecycle management
    /// </summary>
    [Binding]
    public class SpecFlowHooks
    {
        private readonly ScenarioContext _scenarioContext;

        public SpecFlowHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario(Order = 0)]
        public void BeforeScenario()
        {
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
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            ReportManager.CreateTest(scenarioTitle, _scenarioContext.ScenarioInfo.Description);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            var driver = _scenarioContext.Get<IWebDriver>("WebDriver");
            
            // Complete test case reporting
            var scenarioStatus = _scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK ? "Pass" : "Fail";
            ReportManager.CompleteTestCase(scenarioStatus);
            
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
