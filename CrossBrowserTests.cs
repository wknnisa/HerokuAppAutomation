using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;

namespace HerokuAppAutomation
{
    public class CrossBrowserTests
    {
        private IWebDriver? driver;

        public enum BrowserType
        {
            Chrome, Firefox, Edge
        }

        [SetUp]
        public void Setup()
        {
            
        }

        // Test case for each browser
        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void VerifyHomePageTitle(BrowserType browserType)
        {
            // Initialize browser driver based on TestCase parameter
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

            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com");
            Assert.That(driver.Title, Is.EqualTo("The Internet"));
        }   

        [TearDown]
        public void Cleanup() 
        { 
            driver?.Quit();
        }
    }
}
