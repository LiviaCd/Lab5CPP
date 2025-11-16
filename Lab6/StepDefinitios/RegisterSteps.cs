using Lab6.Core.Configuration;
using Lab6.Core.Constants;
using Lab6.Helpers;
using Lab6.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Lab6.StepDefinitions
{
    [Binding]
    public class RegisterSteps
    {
        private readonly IWebDriver _driver;
        private readonly HomePage homePage;
        private readonly RegisterPage registerPage;

        public RegisterSteps(IWebDriver driver)
        {
            _driver = driver;
            homePage = new HomePage(driver);
            registerPage = new RegisterPage(driver);
        }

        // -------------------------------
        // GIVEN
        // -------------------------------

        [Given(@"I am on the homepage")]
        public void GivenIAmOnTheHomepage()
        {
            _driver.Navigate().GoToUrl(AppConfig.HomePageUrl);
        }

        // -------------------------------
        // WHEN
        // -------------------------------

        [When(@"I click on the Sign Up button")]
        public void WhenIClickOnTheSignUpButton()
        {
            homePage.ClickSignUp();
        }

        // -------------------------------
        // THEN – popup visibility
        // -------------------------------

        [Then(@"the register popup should be displayed")]
        public void ThenTheRegisterPopupShouldBeDisplayed()
        {
            AssertionHelper.AssertElementVisible(
                _driver,
                registerPage.NameField,
                "Register popup",
                AppConfig.ShortTimeoutSeconds
            );
        }

        [Then(@"the register fields should be visible")]
        public void ThenTheRegisterFieldsShouldBeVisible()
        {
            // Wait for modal to be fully visible
            WaitHelper.WaitForModalVisible(_driver, ElementSelectors.RegisterModal.ModalId);
            
            // Assert all fields are visible
            AssertionHelper.AssertElementVisible(_driver, registerPage.NameField, "Name field");
            AssertionHelper.AssertElementVisible(_driver, registerPage.EmailField, "Email field");
            AssertionHelper.AssertElementVisible(_driver, registerPage.PasswordField, "Password field");
            AssertionHelper.AssertElementVisible(_driver, registerPage.ConfirmPasswordField, "Confirm Password field");
            AssertionHelper.AssertElementVisible(_driver, registerPage.SubmitButton, "Submit button");
            AssertionHelper.AssertElementVisible(_driver, registerPage.FacebookButton, "Facebook button");
            AssertionHelper.AssertElementVisible(_driver, registerPage.TwitterButton, "Twitter button");
            AssertionHelper.AssertElementVisible(_driver, registerPage.InstagramButton, "Instagram button");
            AssertionHelper.AssertElementVisible(_driver, registerPage.PinterestButton, "Pinterest button");
        }

        // -------------------------------
        // INPUT STEPS
        // -------------------------------

        [When(@"I enter ""(.*)"" in the Name field")]
        public void WhenIEnterInTheNameField(string name)
        {
            registerPage.EnterName(name);
            var nameField = WaitHelper.WaitForElementVisible(_driver, registerPage.NameField);
            Assert.That(nameField.GetAttribute("value"), Is.EqualTo(name));
        }

        [When(@"I enter ""(.*)"" in the Email field")]
        public void WhenIEnterInTheEmailField(string email)
        {
            registerPage.EnterEmail(email);
            var emailField = WaitHelper.WaitForElementVisible(_driver, registerPage.EmailField);
            Assert.That(emailField.GetAttribute("value"), Is.EqualTo(email));
        }

        [When(@"I enter ""(.*)"" in the Password field")]
        public void WhenIEnterInThePasswordField(string pass)
        {
            registerPage.EnterPassword(pass);
            var passwordField = WaitHelper.WaitForElementVisible(_driver, registerPage.PasswordField);
            Assert.That(passwordField.GetAttribute("value"), Is.EqualTo(pass));
        }

        [When(@"I enter ""(.*)"" in the Confirm Password field")]
        public void WhenIEnterInTheConfirmPasswordField(string pass)
        {
            registerPage.EnterConfirmPassword(pass);
            var confirmPasswordField = WaitHelper.WaitForElementVisible(_driver, registerPage.ConfirmPasswordField);
            Assert.That(confirmPasswordField.GetAttribute("value"), Is.EqualTo(pass));
        }

        // -------------------------------
        // SUBMIT
        // -------------------------------

        [When(@"I click the Register Sign Up button")]
        public void WhenIClickTheRegisterSignUpButton()
        {
            registerPage.ClickSignUp();
        }

        [Then(@"the registration should be submitted")]
        public void ThenTheRegistrationShouldBeSubmitted()
        {
            AssertionHelper.AssertRegistrationSuccess(_driver, ElementSelectors.RegisterModal.ModalId);
        }

        // -------------------------------
        // ERROR VALIDATION STEPS
        // -------------------------------

        [Then(@"the registration should fail with an error message")]
        public void ThenTheRegistrationShouldFailWithAnErrorMessage()
        {
            AssertionHelper.AssertRegistrationFailure(
                _driver,
                ElementSelectors.RegisterModal.ModalId,
                registerPage.IsErrorMessageDisplayed()
            );
        }

        // -------------------------------
        // SOCIAL LOGIN STEPS
        // -------------------------------

        [When(@"I click on the Facebook button")]
        public void WhenIClickOnTheFacebookButton()
        {
            WaitHelper.WaitForElementVisible(_driver, registerPage.FacebookButton);
            registerPage.ClickFacebookButton();
        }

        [Then(@"I should be redirected to Facebook login")]
        public void ThenIShouldBeRedirectedToFacebookLogin()
        {
            var wait = WaitHelper.CreateWait(_driver, AppConfig.ShortTimeoutSeconds);
            
            // Wait for URL to change or check if we're on Facebook domain
            wait.Until(driver =>
            {
                var currentUrl = driver.Url.ToLower();
                return currentUrl.Contains("facebook.com") || currentUrl.Contains("facebook");
            });

            Assert.That(
                _driver.Url.ToLower().Contains("facebook"),
                Is.True,
                "Should be redirected to Facebook login page."
            );
        }

        // -------------------------------
        // DATA TABLE STEPS
        // -------------------------------

        [When(@"I fill the registration form with the following data:")]
        public void WhenIFillTheRegistrationFormWithTheFollowingData(Table table)
        {
            // Convert Data Table to dictionary for easier access
            var data = table.Rows[0];
            
            var name = data["Name"];
            var email = data["Email"];
            var password = data["Password"];
            var confirmPassword = data["Confirm Password"];

            // Fill form fields
            if (!string.IsNullOrWhiteSpace(name))
            {
                registerPage.EnterName(name);
                var nameField = WaitHelper.WaitForElementVisible(_driver, registerPage.NameField);
                Assert.That(nameField.GetAttribute("value"), Is.EqualTo(name));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                registerPage.EnterEmail(email);
                var emailField = WaitHelper.WaitForElementVisible(_driver, registerPage.EmailField);
                Assert.That(emailField.GetAttribute("value"), Is.EqualTo(email));
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                registerPage.EnterPassword(password);
                var passwordField = WaitHelper.WaitForElementVisible(_driver, registerPage.PasswordField);
                Assert.That(passwordField.GetAttribute("value"), Is.EqualTo(password));
            }

            if (!string.IsNullOrWhiteSpace(confirmPassword))
            {
                registerPage.EnterConfirmPassword(confirmPassword);
                var confirmPasswordField = WaitHelper.WaitForElementVisible(_driver, registerPage.ConfirmPasswordField);
                Assert.That(confirmPasswordField.GetAttribute("value"), Is.EqualTo(confirmPassword));
            }
        }

        [When(@"I register the following valid users:")]
        public void WhenIRegisterTheFollowingValidUsers(Table table)
        {
            // Process each row in the Data Table
            foreach (var row in table.Rows)
            {
                // Navigate to homepage and open registration modal for each user
                _driver.Navigate().GoToUrl(AppConfig.HomePageUrl);
                homePage.ClickSignUp();
                WaitHelper.WaitForModalVisible(_driver, ElementSelectors.RegisterModal.ModalId);

                // Extract data from current row
                var name = row["Name"];
                var email = row["Email"];
                var password = row["Password"];
                var confirmPassword = row["Confirm Password"];

                // Fill registration form
                registerPage.EnterName(name);
                registerPage.EnterEmail(email);
                registerPage.EnterPassword(password);
                registerPage.EnterConfirmPassword(confirmPassword);

                // Submit registration
                registerPage.ClickSignUp();

                // Wait a moment for form processing
                var wait = WaitHelper.CreateWait(_driver, AppConfig.ShortTimeoutSeconds);
                wait.Until(driver =>
                {
                    try
                    {
                        var modal = driver.FindElement(By.Id(ElementSelectors.RegisterModal.ModalId));
                        return !modal.Displayed;
                    }
                    catch
                    {
                        return true; // Modal closed - success
                    }
                });
            }
        }

        [Then(@"the registration should be successful")]
        public void ThenTheRegistrationShouldBeSuccessful()
        {
            // Verify that registration was successful (modal closed or no error message)
            AssertionHelper.AssertRegistrationSuccess(_driver, ElementSelectors.RegisterModal.ModalId);
        }
    }
}
