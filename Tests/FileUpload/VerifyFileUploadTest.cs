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
            //Path.Combine(FileDirectory, "1mb.jpg"),
            //Path.Combine(FileDirectory, "5mb.jpg"),
            //Path.Combine(FileDirectory, "10mb.jpg"),
            //Path.Combine(FileDirectory, "50mb.jpg"),
            //Path.Combine(FileDirectory, "100mb.jpg"),
            Path.Combine(FileDirectory, "200mb.txt"),
            Path.Combine(FileDirectory, "250mb.bin"),
            Path.Combine(FileDirectory, "275mb.bin"),
            Path.Combine(FileDirectory, "300mb.zip"),
            Path.Combine(FileDirectory, "500mb")
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
            // Check if any file is larger than the size limit
            bool isLargeFile = IsLargeFilePresent();

            // Setup browser with necessary timeouts based on file size
            SetupBrowser(browserType);

            // Navigate to the file upload page
            driver!.Navigate().GoToUrl(UploadUrl);

            foreach (string filePath in filePaths)
            {
                FileInfo fileInfo = new FileInfo(filePath);

                // Log file information (for reference)
                TestContext.WriteLine($"Testing file: {fileInfo.Name}, Size: {fileInfo.Length / (1024 * 1024)} MB");

                // Wait for the upload completion (use an explicit wait here if necessary)
                try
                {
                    // Skip files larger than 250MB (or another limit) and log the skip
                    if (fileInfo.Length > (250 * 1024 * 1024)) // Adjust size limit if necessary
                    {
                        LogIssue($"Skipping file {fileInfo.Name} because it exceeds the size limit of 250MB.");
                        continue; // Skip this file and proceed to the next one
                    }

                    // Perform the file upload
                    UploadFile(filePath);

                    // Adjust the wait time based on file size for upload completion
                    TimeSpan uploadWaitTime = fileInfo.Length > (100 * 1024 * 1024)
                        ? TimeSpan.FromMinutes(5)  // Increase timeout for larger files (e.g., > 100MB)
                        : TimeSpan.FromMinutes(2); // Default timeout for smaller files

                    // Wait for the upload to complete
                    WaitForUploadCompletion(TimeSpan.FromMinutes(isLargeFile ? 5 : 2));

                    // Verify upload was successful by checking the uploaded file name in the UI
                    IWebElement uploadedMessage = driver.FindElement(By.Id("uploaded-files"));

                    if(!uploadedMessage.Text.Contains(fileInfo.Name))
                    {
                        LogIssue($"Upload failed for file: {fileInfo.Name}");
                    }
                    else 
                    {
                        // Log success message
                        TestContext.WriteLine($"Successfully uploaded: {fileInfo.Name}");
                    }

                    // Reset page for the next file upload
                    driver.Navigate().GoToUrl(UploadUrl);

                }
                catch (NoSuchElementException)
                {
                    // Handle cases where the file upload element isn't found
                    LogIssue($"Upload failed for file: {fileInfo.Name}");

                    // Log application error if present
                    if (IsApplicationErrorPresent())
                    {
                        LogIssue($"Application error occured for file {fileInfo.Name}");
                    }
                }

                catch (WebDriverException ex)
                {
                    // Handle browser-related issues like crashes or unexpected shutdowns
                    LogIssue($"Browser crashed during file upload: {fileInfo.Name}, Error: {ex.Message}");

                    // Restart browser and continue
                    driver.Quit();
                    SetupBrowser(browserType);
                    driver?.Navigate().GoToUrl(UploadUrl);
                }

                catch (Exception ex) 
                {
                    // Handle any other unexpected errors
                    LogIssue($"An error occured during file upload: {fileInfo.Name}, Error: {ex.Message}");
                }

                finally
                {
                    // Reset the page after each file upload attempt
                    driver!.Navigate().GoToUrl(UploadUrl);
                }

            }

            // Clean up resources after the test completes
            CleanUp();
        }

        // Log issue function (add logging mechanism here, e.g., file logging, test report logging, etc.)
        private void LogIssue(string message)
        {
            // Log the issue in the test context or external log
            TestContext.WriteLine($"ISSUE: {message}");
        }

        // Wait for the file upload to complete
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
                LogIssue("File upload did not complete within the expected time.");
            }
        }

        // Check if there are large files in the test set
        private bool IsLargeFilePresent()
        {
            return filePaths.Any(filePath => new FileInfo(filePath).Length > (100 * 1024 * 1024));
        }

        // Check if the application has thrown an error
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
            SetupBrowser(browserType);

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
            SetupBrowser(browserType);

            driver!.Navigate().GoToUrl(UploadUrl);

            // Path.Combine - combining the directory path and the file name.
            string largeFilePath = Path.Combine(FileDirectory, "sublime_text_build_4126_x64_setup.exe"); 
            UploadFile(largeFilePath);

            IWebElement errorMessage = driver.FindElement(By.Id("error-message"));
            Assert.That(errorMessage.Text, Does.Contain("File size limit is exceeded"));

            CleanUp();
        }

        // Upload file function
        public void UploadFile(string filePath)
        {
            IWebElement fileInput = driver!.FindElement(By.Id("file-upload"));
            fileInput.SendKeys(filePath);

            IWebElement submitButton = driver!.FindElement(By.Id("file-submit"));
            submitButton.Click();
        }

        // Setup browser with dynamic timeout settings based on file size
        private void SetupBrowser(BrowserType browserType)
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

            // Set standard timeouts (you can adjust these based on general use)
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(3); // Standard page load timeout
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30); // Standard implicit wait
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(60); // Standard script timeout

        }

        // Clean up after the test
        [TearDown]
        public void CleanUp()
        {
            driver?.Quit();
        }
    }
}
