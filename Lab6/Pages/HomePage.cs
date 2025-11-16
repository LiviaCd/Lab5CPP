using Lab6.Core.Base;
using Lab6.Core.Constants;
using OpenQA.Selenium;

namespace Lab6.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver) : base(driver)
        {
        }

        public void ClickSignUp()
        {
            var signUpButton = FindElement(ElementSelectors.HomePage.SignUpButton);
            signUpButton.Click();
        }
    }
}
