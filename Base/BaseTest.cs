using HerokuAppAutomation.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Communication;
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
        protected BrowserType browserType;

        /// <summary>
        /// Sets up the browser driver based on the specified browser type.
        /// </summary>
        public void SetupBrowser(BrowserType browser)
        {
            Logger.Log($"Setting up browser: {browser}");

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

        /// <summary>
        /// Restarts the browser and sets up a fresh driver instance.
        /// </summary>
        protected void RestartBrowser()
        {
            Logger.Log("Restarting the browser...");

            if (driver != null)
            {
                try
                {
                    driver?.Quit();
                    Logger.Log("Browser session ended successfully.");
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error during browser quit: {ex.Message}", Logger.LogLevel.Error);
                }
                finally
                {
                    driver = null;
                }
            }

            SetupBrowser(browserType);
            Logger.Log("Browser restarted and ready for testing.");
        }

        /// <summary>
        /// Cleans up the driver after each test.
        /// </summary>
        [TearDown]
        public void CleanUp()
        {
            if (driver != null) 
            {
                driver?.Quit(); // Close and dispose of the browser
                driver = null;   // Reset the driver to ensure no reuse
            }
            Logger.Log("Browser instance cleaned up.");
        }
    }
}
