using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace HerokuAppAutomation.Base
{
    // Enum for browser types
    public enum BrowserType
    {
        Chrome, Firefox, Edge
    }
    public class BaseTest
    {
        protected IWebDriver? driver;

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

            if (driver == null)
            {
                throw new InvalidOperationException("Driver was not initialized correctly.");
            }
        }

        // Tear down method to close the browser after each test
        [TearDown]
        public void CleanUp()
        {
            driver?.Quit(); // Close and dispose of the browser
            driver = null;   // Reset the driver to ensure no reuse
        }
    }
}
