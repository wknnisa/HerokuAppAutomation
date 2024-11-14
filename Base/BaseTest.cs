using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace HerokuAppAutomation.Base
{
    public class BaseTest
    {
        protected IWebDriver? driver;

        // Enum for browser types
        public enum BrowserType
        {
            Chrome, Firefox, Edge
        }

        // Setup method to initialize the browser based on the test case
        public void SetupBrowser(BrowserType browserType)
        {
            switch(browserType)
            {
                case BrowserType.Chrome:
                    driver = new ChromeDriver(@"C:\WebDrivers\");
                    break;
                case BrowserType.Firefox:
                    driver = new FirefoxDriver(@"C:\WebDrivers");
                    break;
                case BrowserType.Edge:
                    driver = new EdgeDriver(@"C:\WebDrivers");
                    break;
                default:
                    throw new ArgumentException("Unsupported Browser");
            }
        }

        // Tear down method to close the browser after each test
        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
        }
    }
}
