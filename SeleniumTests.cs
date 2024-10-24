using NUnit.Framework; // Make sure NUnit is installed
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HerokuAppAutomation
{
    public class SeleniumTests
    {
        private IWebDriver? driver;

        [SetUp] // Runs before each test
        public void Setup()
        {
            driver = new ChromeDriver(@"C:\WebDrivers\"); // Update with your path
        }

        [Test]
        public void VerifyHomePageTitle()
        {
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com");
            // Use Assert.That with Is.EqualTo for assertions in NUnit 4.x
            Assert.That(driver.Title, Is.EqualTo("The Internet"));
        }

        [TearDown] // Runs after each test
        public void Cleanup()
        {
            driver!.Quit(); // Closes the browser
        }
    }
}
