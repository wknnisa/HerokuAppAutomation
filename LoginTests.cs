using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;

namespace HerokuAppAutomation
{
    public class LoginTests
    {
        private IWebDriver? driver;

        // Enum for browser types
        public enum BrowserType 
        { 
            Chrome, Firefox, Edge
        }

        // Define the URLs
        private const string LoginUrl = "https://the-internet.herokuapp.com/login";
        private const string SecureUrl = "https://the-internet.herokuapp.com/secure";
        private const string HomeUrl = "https://the-internet.herokuapp.com/";

        [SetUp]
        public void Setup()
        {

        }

        // Test case that receives the browser type
        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void SuccessfulLogin(BrowserType browserType)
        {
            SetupBrowser(browserType); // Initialize browser based on test case

            NavigateToLogin();

            // Case Sensitivity Test
            string username = "tomsmith";
            string password = "SuperSecretPassword!";

            // Valid Login
            PerformLogin(username, password);
            Assert.That(driver!.Url, Is.EqualTo(SecureUrl));

            // Navigate back to the login page for further tests
            NavigateToLogin();

            // Case Sensitive Check for Username
            PerformLogin("TomSmith", password); // Different case for username
            Assert.That(driver.Url, Is.Not.EqualTo(SecureUrl)); // Should not log in

            // Navigate back to the login page again for password check
            NavigateToLogin();

            // Case Sensitive Check for Password
            PerformLogin(username, "supersecretpassword!"); // Different case for password
            Assert.That(driver.Url, Is.Not.EqualTo(SecureUrl)); // Should not log in

            // Session Persistence Test
            driver!.Navigate().GoToUrl(HomeUrl);
            Assert.That(driver.Url, Is.EqualTo(HomeUrl)); // Should be on home page

            // Navigate back to secure page
            driver!.Navigate().GoToUrl(SecureUrl);
            Assert.That(driver.Url, Is.EqualTo(SecureUrl)); // Should still be logged in

            CleanUp();
        }

        // Method to set up the browser, based on the input browser type
        private void SetupBrowser(BrowserType browserType)
        {
            // Declare ChromeOptions, FirefoxOptions, EdgeOptions outside of the switch
            ChromeOptions chromeOptions = new ChromeOptions();
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            EdgeOptions edgeOptions = new EdgeOptions();

            // Common options for all browsers
            chromeOptions.AddArgument("--disable-gpu"); // Disable GPU acceleration for Chrome
            chromeOptions.AddArgument("--start-maximixed"); // Start maximized for Chrome

            firefoxOptions.AddArgument("--start-maximized"); // Start maximized for Firefox
            edgeOptions.AddArgument("--start-maximized"); // Start maximized for Edge

            // Initialize driver based on selected browser
            switch (browserType)
            {
                case BrowserType.Chrome:
                    driver = new ChromeDriver(@"C:\WebDrivers\");
                    break;
                case BrowserType.Firefox:
                    driver = new FirefoxDriver(@"C:\WebDrivers\");
                    break;
                case BrowserType.Edge:
                    driver = new EdgeDriver(@"C:\WebDrivers\");
                    break;
                default:
                    throw new ArgumentException("Unsupported browser");
            }
        }

        // Helper method for logging in
        private void PerformLogin(string username, string password) 
        {
            driver!.FindElement(By.Id("username")).Clear();
            driver!.FindElement(By.Id("username")).SendKeys(username);
            driver!.FindElement(By.Id("password")).Clear();
            driver!.FindElement(By.Id("password")).SendKeys(password);
            driver!.FindElement(By.XPath("//button[@type='submit']")).Click();
        }

        // Helper method for navigating to the login page
        private void NavigateToLogin()
        {
            driver!.Navigate().GoToUrl(LoginUrl);
        }

        // Cleanup method to close the browser after each test
        [TearDown]
        public void CleanUp() 
        { 
            driver!.Quit();
        }
    }
}
