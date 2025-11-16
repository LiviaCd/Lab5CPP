using OpenQA.Selenium;

namespace Lab6.Core.Constants
{
    public static class ElementSelectors
    {
        public static class RegisterModal
        {
            public const string ModalId = "myModal2";
            
            public static By NameField => By.CssSelector($"#{ModalId} input[name='Name']");
            public static By EmailField => By.CssSelector($"#{ModalId} input[name='Email']");
            public static By PasswordField => By.CssSelector($"#{ModalId} input[name='password']");
            public static By ConfirmPasswordField => By.XPath($"//div[@id='{ModalId}']//input[@name='Confirm Password']");
            public static By SubmitButton => By.CssSelector($"#{ModalId} input[type='submit'][value='Sign Up']");
            public static By ErrorMessage => By.CssSelector($"#{ModalId} .error, #{ModalId} .alert, #{ModalId} .text-danger");
        }

        public static class HomePage
        {
            public static By SignUpButton => By.CssSelector("a[data-target='#myModal2']");
        }

        public static class SocialButtons
        {
            public static By FacebookButton => By.CssSelector($"#{RegisterModal.ModalId} a.facebook");
            public static By TwitterButton => By.CssSelector($"#{RegisterModal.ModalId} a.twitter");
            public static By InstagramButton => By.CssSelector($"#{RegisterModal.ModalId} a.instagram");
            public static By PinterestButton => By.CssSelector($"#{RegisterModal.ModalId} a.pinterest");
        }
    }
}

