using OpenQA.Selenium;

namespace Lab7.Locators
{
    /// <summary>
    /// Generic locators for Google search page elements
    /// Provides relative paths to identify web elements
    /// </summary>
    public static class GooglePageLocators
    {
        // Search box locator
        public static By SearchBox => By.Name("q");
        
        // Search button locator
        public static By SearchButton => By.Name("btnK");
        
        // Alternative search button (when focus is on search box)
        public static By SearchButtonSubmit => By.XPath("(//input[@name='btnK'])[2]");
        
        // Search button in search suggestions dropdown
        public static By SearchButtonInDropdown => By.XPath("//input[@type='submit']");
        
        // Search results container
        public static By SearchResults => By.Id("search");
        
        // Individual search result items
        public static By ResultItems => By.CssSelector(".LC20lb.MBeuO.DKV0Md");
        
        // "Did you mean" link (supports multiple languages)
        public static By DidYouMeanLink => By.CssSelector(".d2IKib");
        
        // Page title
        public static By PageTitle => By.TagName("title");
        
        // Google logo (to verify page loaded)
        public static By GoogleLogo => By.XPath("//img[@alt='Google' or contains(@src,'google')]");
    }
}

