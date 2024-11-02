using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HerokuAppAutomation
{
    public class LoginTests
    {
        private IWebDriver? driver;
        private const string LoginUrl = "https://the-internet.herokuapp.com/login";
        private const string SecureUrl = "https://the-internet.herokuapp.com/secure";
        private const string HomeUrl = "https://the-internet.herokuapp.com/";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver(@"C:\WebDrivers\");
        }

        [Test]
        public void SuccessfulLogin()
        {
            NavigateToLogin();

            // Case Sensitivity Test
            string username = "tomsmith";
            string password = "SuperSecretPassword!";

            // Valid Login
            PerformLogin(username, password);
            Assert.That(driver!.Url, Is.EqualTo("SecureUrl"));

            // Navigate back to the login page for further tests
            NavigateToLogin();

            // Case Sensitive Check for Username
            PerformLogin("TomSmith", password); // Different case for username
            Assert.That(driver.Url, Is.Not.EqualTo("SecureUrl")); // Should not log in

            // Navigate back to the login page again for password check
            NavigateToLogin();

            // Case Sensitive Check for Password
            PerformLogin(username, "supersecretpassword!"); // Different case for password
            Assert.That(driver.Url, Is.Not.EqualTo("SecureUrl")); // Should not log in

            // Session Persistence Test
            driver!.Navigate().GoToUrl("HomeUrl");
            Assert.That(driver.Url, Is.EqualTo("HomeUrl")); // Should be on home page

            // Navigate back to secure page
            driver!.Navigate().GoToUrl("SecureUrl");
            Assert.That(driver.Url, Is.EqualTo("SecureUrl")); // Should still be logged in
        }

        private void PerformLogin(string username, string password) 
        {
            driver!.FindElement(By.Id("username")).Clear();
            driver!.FindElement(By.Id("username")).SendKeys(username);
            driver!.FindElement(By.Id("password")).Clear();
            driver!.FindElement(By.Id("password")).SendKeys(password);
            driver!.FindElement(By.XPath("//button[@type='submit']")).Click();
        }

        private void NavigateToLogin()
        {
            driver!.Navigate().GoToUrl("LoginUrl");
        }

        [TearDown]
        public void CleanUp() 
        { 
            driver!.Quit();
        }
    }
}
