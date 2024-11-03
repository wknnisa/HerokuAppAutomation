using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace HerokuAppAutomation.Tests.FileUpload
{
    public class VerifyFileUploadTest
    {
        private IWebDriver? driver;

        public enum BrowserType
        {
            Chrome, Firefox, Edge
        }

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver(@"C:\WebDrivers\");
        }

        private void SetupBrowser(BrowserType browserType)
        {

            ChromeOptions chromeOptions = new ChromeOptions();
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            EdgeOptions edgeOptions = new EdgeOptions();

            switch (browserType)
            {
                case BrowserType.Chrome:
                    driver = new ChromeDriver(@"C:\WebDrivers\", chromeOptions);
                    break;
                case BrowserType.Firefox:
                    driver = new FirefoxDriver(@"C:\WebDrivers\", firefoxOptions);
                    break;
                case BrowserType.Edge:
                    driver = new EdgeDriver(@"C:\WebDrivers\", edgeOptions);
                    break;
                default:
                    throw new ArgumentException("Unsupported Browser");

            }
        }

        [TearDown]
        public void CleanUp()
        {
            driver?.Quit();
        }
    }
}
