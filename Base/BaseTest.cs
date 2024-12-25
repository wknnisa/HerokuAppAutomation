using HerokuAppAutomation.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using FileHelper = HerokuAppAutomation.Utilities.FileHelper;

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

            driver = browser switch
            {
                BrowserType.Chrome => new ChromeDriver(@"C:\WebDrivers\"),
                BrowserType.Firefox => new FirefoxDriver(@"C:\WebDrivers"),
                BrowserType.Edge => new EdgeDriver(@"C:\WebDrivers"),
                _ => throw new ArgumentException("Unsupported Browser")
            };

            // Validate that the driver was initialized
            if (driver == null)
            {
                throw new InvalidOperationException("Driver was not initialized correctly.");
            }

            // Common browser configurations
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(3); // Page load timeout
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30); // Implicit wait
        }

        /// <summary>
        /// Restarts the browser to ensure a fresh state.
        /// </summary>
        protected void RestartBrowser()
        {
            Logger.Log("Restarting the browser...");

            try
            {
                driver?.Quit();
            }
            catch (Exception ex)
            {
                Logger.Log($"Error during browser quit: {ex.Message}", Logger.LogLevel.Error);
            }
            finally
            {
                driver = null;
                SetupBrowser(browserType);
            }

        }

        /// <summary>
        /// Cleans up browser and other global resources after each test.
        /// </summary>
        [TearDown]
        public void CleanUp()
        {
            try
            {
                // Clean up driver
                driver?.Quit(); // Close and dispose of the browser
                driver = null;

                // Clean up shared resources
                FileHelper.CleanUpGlobalTestFiles();
                Logger.Log("Global resources cleaned up.");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error during browser cleanup: {ex.Message}", Logger.LogLevel.Warning);
            }
        }
    }
}
