using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

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
        private const string FileDirectory = @"C:\TestFiles\";

        string[] filePaths =
        {
            Path.Combine(FileDirectory, "1mb.jpg"),
            //Path.Combine(FileDirectory, "5mb.jpg"),
            //Path.Combine(FileDirectory, "10mb.jpg"),
            //Path.Combine(FileDirectory, "50mb.jpg"),
            //Path.Combine(FileDirectory, "100mb.jpg"),
            //Path.Combine(FileDirectory, "200mb.txt"),
            //Path.Combine(FileDirectory, "250mb.bin"),
            //Path.Combine(FileDirectory, "275mb.bin"),
            //Path.Combine(FileDirectory, "300mb.zip"),
            //Path.Combine(FileDirectory, "500mb")
        };

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadVariousSizes (BrowserType browserType)
        {
            bool isLargeFile = IsLargeFilePresent();

            SetupBrowser(browserType, isLargeFile);

            driver!.Navigate().GoToUrl(UploadUrl);

            foreach (string filePath in filePaths)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                TestContext.WriteLine($"Testing file: {fileInfo.Name}, Size: {fileInfo.Length / (1024 * 1024)} MB");

                // Perform the file upload
                UploadFile(filePath);

                // Wait for the upload completion (use an explicit wait here if necessary)
                try
                {
                    if (fileInfo.Length > (500 * 1024 * 1024))
                    {
                        TestContext.WriteLine($"Skipping file {fileInfo.Name} as it exceeds the size limit.");
                        continue;
                    }

                    UploadFile(filePath);

                    WaitForUploadCompletion(TimeSpan.FromMinutes(isLargeFile ? 5 : 2));

                    IWebElement uploadedMessage = driver.FindElement(By.Id("uploaded-files"));
                    TestContext.WriteLine($"Successfully uploaded: {fileInfo.Name}");

                    driver.Navigate().GoToUrl(UploadUrl);

                }
                catch (NoSuchElementException)
                {
                    TestContext.WriteLine($"Upload failed for file: {fileInfo.Name}");

                    if (IsApplicationErrorPresent())
                    {
                        TestContext.WriteLine($"Application error occured for file {fileInfo.Name}");
                    }
                    else
                    {
                        Assert.Fail($"File size too large or upload failed for {fileInfo.Name}");
                    }
                }

                catch (WebDriverException ex)
                {
                    TestContext.WriteLine($"Browser crashed during file upload: {fileInfo.Name}, Error: {ex.Message}");
                    Assert.Fail($"Test failed due to browser crash: {fileInfo.Name}");

                    driver.Quit();
                    SetupBrowser(browserType, isLargeFile);
                    driver?.Navigate().GoToUrl(UploadUrl);
                }

                catch (Exception ex) 
                {
                    TestContext.WriteLine($"An error occured during file upload: {fileInfo.Name}, Error: {ex.Message}");
                    Assert.Fail($"Test failed due to error: {fileInfo.Name}");
                }

                finally
                {
                    driver!.Navigate().GoToUrl(UploadUrl);
                }

            }

            CleanUp();
        }

        public void WaitForUploadCompletion(TimeSpan timeout)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout);

            try
            {
                wait.Until(d => d.FindElement(By.Id("uploaded-files")).Displayed);
                TestContext.WriteLine("File upload complete");
            }
            catch (WebDriverTimeoutException) 
            {
                TestContext.WriteLine("File upload did not complete within the expected time.");
                Assert.Fail("File upload timed out.");
            }
        }

        private bool IsLargeFilePresent()
        {
            return filePaths.Any(filePath => new FileInfo(filePath).Length > (100 * 1024 * 1024));
        }

        private bool IsApplicationErrorPresent()
        {
            try
            {
                IWebElement errorElement = driver!.FindElement(By.XPath("//body[contains(text(), 'Application error')]"));
                return errorElement.Displayed;
            }
            catch (NoSuchElementException) 
            { 
                return false;
            }
            
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadValidFile(BrowserType browserType)
        {
            //SetupBrowser(browserType);

            driver!.Navigate().GoToUrl(UploadUrl);

            string validFilePath = Path.Combine(FileDirectory, "sample1.jpg");
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
            //SetupBrowser(browserType);

            driver!.Navigate().GoToUrl(UploadUrl);

            // Path.Combine - combining the directory path and the file name.
            string largeFilePath = Path.Combine(FileDirectory, "sublime_text_build_4126_x64_setup.exe"); 
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
            submitButton.Click();
        }

        private void SetupBrowser(BrowserType browserType, bool isLargeFile)
        {
            // Declare browser options
            ChromeOptions chromeOptions = new ChromeOptions();
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            EdgeOptions edgeOptions = new EdgeOptions();

            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--start-maximized");
            firefoxOptions.AddArgument("--start-maximized");
            edgeOptions.AddArgument("--start-maximized");

            // Initialize the driver based on the selected browser
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

            if (isLargeFile) 
            {
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(5); // Increase page load timeout to 3 minutes
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60); // Increase implicit wait to 30 seconds
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(2); // Increase script timeout
            }
            else
            {
                // Set timeout values
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(3); // Increase page load timeout to 3 minutes
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30); // Increase implicit wait to 30 seconds
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(60); // Increase script timeout
            }
            
        }

        [TearDown]
        public void CleanUp()
        {
            driver?.Quit();
        }
    }
}
