using HerokuAppAutomation.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using static HerokuAppAutomation.Tests.HomePage.VerifyHomePageTitleTest;

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

            // Declare browser options
            ChromeOptions chromeOptions = new ChromeOptions();
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            EdgeOptions edgeOptions = new EdgeOptions();

            // Set common options for all browsers
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--start-maximized");
            firefoxOptions.AddArgument("--start-maximized");
            edgeOptions.AddArgument("--start-maximized");

            // Initialize the driver based on the selected browser
            switch (browserType)
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

            // Validate that the driver was initialized
            if (driver == null)
            {
                throw new InvalidOperationException("Driver was not initialized correctly.");
            }

            // Set default timeouts for all browsers
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(3); // Page load timeout
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30); // Implicit wait
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(60); // Script timeout
        }

        // Tear down method to close the browser after each test
        [TearDown]
        public void CleanUp()
        {
            driver?.Quit(); // Close and dispose of the browser
            driver = null;   // Reset the driver to ensure no reuse
        }

        /// <summary>
        /// Method to restart the browser if there's a critical failure or timeout
        /// </summary>
        protected void RestartBrowser()
        {
            Logger.Log("Restarting browser...");

            driver?.Quit();

            driver = SetupBrowser(browserType);
        }
    }
}
