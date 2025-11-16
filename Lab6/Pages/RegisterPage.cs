using Lab6.Core.Base;
using Lab6.Core.Constants;
using OpenQA.Selenium;

namespace Lab6.Pages
{
    public class RegisterPage : BasePage
    {
        public RegisterPage(IWebDriver driver) : base(driver)
        {
        }

        // -----------------------------
        // PROPERTIES - Element Locators
        // -----------------------------

        public By NameField => ElementSelectors.RegisterModal.NameField;
        public By EmailField => ElementSelectors.RegisterModal.EmailField;
        public By PasswordField => ElementSelectors.RegisterModal.PasswordField;
        public By ConfirmPasswordField => ElementSelectors.RegisterModal.ConfirmPasswordField;
        public By SubmitButton => ElementSelectors.RegisterModal.SubmitButton;
        public By FacebookButton => ElementSelectors.SocialButtons.FacebookButton;
        public By TwitterButton => ElementSelectors.SocialButtons.TwitterButton;
        public By InstagramButton => ElementSelectors.SocialButtons.InstagramButton;
        public By PinterestButton => ElementSelectors.SocialButtons.PinterestButton;
        public By ErrorMessage => ElementSelectors.RegisterModal.ErrorMessage;

        // -----------------------------
        // FORM INPUT METHODS
        // -----------------------------

        public void EnterName(string name)
        {
            var nameField = FindElement(NameField);
            nameField.Clear();
            nameField.SendKeys(name);
        }

        public void EnterEmail(string email)
        {
            var emailField = FindElement(EmailField);
            emailField.Clear();
            emailField.SendKeys(email);
        }

        public void EnterPassword(string password)
        {
            var passwordField = FindElement(PasswordField);
            passwordField.Clear();
            passwordField.SendKeys(password);
        }

        public void EnterConfirmPassword(string password)
        {
            var confirmPasswordField = FindElement(ConfirmPasswordField);
            confirmPasswordField.Clear();
            confirmPasswordField.SendKeys(password);
        }

        // -----------------------------
        // ACTION METHODS
        // -----------------------------

        public void ClickSignUp()
        {
            var submitButton = FindElement(SubmitButton);
            submitButton.Click();
        }

        public void ClickFacebookButton()
        {
            WaitForElementVisible(FacebookButton);
            var facebookButton = FindElement(FacebookButton);
            facebookButton.Click();
        }

        // -----------------------------
        // VALIDATION METHODS
        // -----------------------------

        public bool IsErrorMessageDisplayed()
        {
            return IsElementDisplayed(ErrorMessage);
        }

        public string GetErrorMessage()
        {
            try
            {
                var errorElement = Driver.FindElement(ErrorMessage);
                return errorElement.Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        public bool IsModalVisible()
        {
            try
            {
                var modal = Driver.FindElement(By.Id(ElementSelectors.RegisterModal.ModalId));
                return modal.Displayed;
            }
            catch
            {
                return false;
            }
        }

        public void WaitForModalVisible()
        {
            WaitForElementVisible(By.Id(ElementSelectors.RegisterModal.ModalId));
        }
    }
}
