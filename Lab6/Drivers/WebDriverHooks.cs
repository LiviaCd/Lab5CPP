using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Lab6.Drivers
{
    [Binding]
    public sealed class WebDriverHooks
    {
        private readonly ScenarioContext _scenarioContext;

        public WebDriverHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            var options = new ChromeOptions();

            var driver = new ChromeDriver(options);  
            driver.Manage().Window.Maximize();

            _scenarioContext.ScenarioContainer.RegisterInstanceAs<IWebDriver>(driver);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (_scenarioContext.ScenarioContainer.IsRegistered<IWebDriver>())
            {
                var driver = _scenarioContext.ScenarioContainer.Resolve<IWebDriver>();
                driver.Quit();
                driver.Dispose();
            }
        }
    }
}
