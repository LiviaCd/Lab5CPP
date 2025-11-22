using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Xunit;

namespace Lab7.Hooks
{
    /// <summary>
    /// xUnit fixture for WebDriver lifecycle management
    /// </summary>
    public class WebDriverFixture : IDisposable
    {
        public IWebDriver Driver { get; private set; }

        public WebDriverFixture()
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

            Driver = new ChromeDriver(options);
            
            // Execute JavaScript to hide webdriver property and set language preferences
            var executor = Driver as IJavaScriptExecutor;
            executor?.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
            
            // Override navigator language properties to force English
            executor?.ExecuteScript(@"
                Object.defineProperty(navigator, 'language', {get: () => 'en-US'});
                Object.defineProperty(navigator, 'languages', {get: () => ['en-US', 'en']});
                Object.defineProperty(navigator, 'userLanguage', {get: () => 'en-US'});
            ");
            
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        }

        public void Dispose()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }
    }
}

