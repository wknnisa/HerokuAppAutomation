using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages; // LoginPage class is in this namespace
using NUnit.Framework;

namespace HerokuAppAutomation.Tests.Login
{
    [TestFixture]
    public class VerifyLoginTests : BaseTest
    {
        private LoginPage? loginPage;
        private BrowserType browserType; // Class-level declaration

        [SetUp]
        public void TestSetUp()
        {
            // Ensure browserType is set before setup
            if (driver == null)
            {
                SetupBrowser(browserType); // Use browserType set by the test method
            }

            // Initialize LoginPage after driver is set up
            if (driver == null)
            {
                throw new InvalidOperationException("Driver is not initialized.");
            }

            loginPage = new LoginPage(driver);

            // Validate loginPage initialization
            if (loginPage == null)
            {
                throw new InvalidOperationException("LoginPage is not initialized.");
            }

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
            this.browserType = browserType; // Assign browserType dynamically for TestSetUp to use

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
            this.browserType = browserType;

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
            this.browserType = browserType;

            // Test with an invalid password
            loginPage!.Login("tomsmith", "invalidPassword");

            // Validate error message
            Assert.That(loginPage.IsErrorMessageDisplayed("Your password is invalid!"), Is.True);

            CleanUp();
        }
    }
}
