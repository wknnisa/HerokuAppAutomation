using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages; // LoginPage class is in this namespace
using NUnit.Framework;

namespace HerokuAppAutomation.Tests.Login
{
    [TestFixture]
    public class VerifyLoginTests : BaseTest
    {
        private LoginPage? loginPage;

        [SetUp]
        public void TestSetUp()
        {
            // Initialize LoginPage after driver is set up
            loginPage = new LoginPage(driver!);

            // Navigate to the login page
            loginPage.NavigateToLogin();
        }

        // Define the URLs
        private const string SecureUrl = "https://the-internet.herokuapp.com/secure";
        private const string HomeUrl = "https://the-internet.herokuapp.com/";

        // Test case that receives the browser type for successful login
        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void LoginSuccess(BrowserType browserType)
        {
            SetupBrowser(browserType); // Initialize browser based on test case

            // Perform a valid login
            loginPage!.Login("tomsmith", "SuperSecretPassword!");
            Assert.That(driver!.Url, Is.EqualTo(SecureUrl));

            // Navigate to home page and back to verify session persistence
            driver.Navigate().GoToUrl(HomeUrl);
            driver.Navigate().GoToUrl(SecureUrl);
            Assert.That(driver.Url, Is.EqualTo(SecureUrl));

            CleanUp(); // Close browser
        }

        // Test case for unsuccessful login (wrong username)
        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void LoginFailsWithInvalidUsername(BrowserType browserType)
        {
            SetupBrowser(browserType);

            // Test with an invalid username
            loginPage!.Login("invalidUser", "SuperSecretPassword!");

            // Validate error message
            Assert.That(loginPage.IsErrorMessageDisplayed("Your username is invalid!"), Is.True);

            CleanUp();
        }

        // Test case for unsuccessful login (wrong password)
        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void LoginFailsWithInvalidPassword(BrowserType browserType)
        {
            SetupBrowser(browserType);

            // Initialize LoginPage after driver is set up
            loginPage = new LoginPage(driver!);
            loginPage.NavigateToLogin();  // Navigate to the login page

            // Test with an invalid password
            loginPage!.Login("tomsmith", "invalidPassword");

            // Validate error message
            Assert.That(loginPage.IsErrorMessageDisplayed("Your password is invalid!"), Is.True);

            CleanUp();
        }
    }
}
