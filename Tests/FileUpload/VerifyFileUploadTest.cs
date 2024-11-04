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

        private const string UploadUrl = "https://the-internet.herokuapp.com/upload";
        private const string FilePath = @"C:\Users\HP\Downloads\sample1.jpg";

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadValidFile(BrowserType browserType)
        {
            SetupBrowser(browserType);

            driver!.Navigate().GoToUrl(UploadUrl);

            string validFilePath = Path.Combine(FilePath, "sample1.jpg");
            UploadFile(validFilePath);

            IWebElement uploadedMessage = driver.FindElement(By.Id("uploaded-files"));
            Assert.That(uploadedMessage.Text, Is.EqualTo("sample1.jpg"));

            CleanUp();
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadSizeLimit(BrowserType browserType)
        {
            SetupBrowser(browserType);

            driver!.Navigate().GoToUrl(UploadUrl);

            string largeFilePath = Path.Combine(FilePath, "sublime_text_build_4126_x64_setup.exe");
            UploadFile(largeFilePath);

            IWebElement errorMessage = driver.FindElement(By.Id("error-message"));
            Assert.That(errorMessage.Text, Does.Contain("File size limit is exceeded"));

            CleanUp();
        }

        public void UploadFile(string filePath)
        {
            IWebElement fileInput = driver!.FindElement(By.Id("file-upload"));
            fileInput.SendKeys(filePath);

            IWebElement submitButton = driver!.FindElement(By.Id("file-submit"));
            fileInput.Click();
        }

        private void SetupBrowser(BrowserType browserType)
        {

            ChromeOptions chromeOptions = new ChromeOptions();
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            EdgeOptions edgeOptions = new EdgeOptions();

            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--start-maximized");
            firefoxOptions.AddArgument("--start-maximized");
            edgeOptions.AddArgument("--start-maximized");

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
