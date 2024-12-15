using FileGeneratorLibrary.Utilities;
using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages;
using HerokuAppAutomation.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HerokuAppAutomation.Tests.FileUpload
{
    public class VerifyFileUploadTest : BaseTest
    {
        private const string UploadUrl = "https://the-internet.herokuapp.com/upload";
        private const string FileDirectory = @"C:\TestFiles\";

        private const string ValidFileName = "200mb.txt";
        private const int ValidFileSize = 200;

        private const string LargeFileName = "500mb.zip";
        private const int LargeFileSize = 500;

        private FileUploadPage? fileUploadPage;

        [SetUp]
        public void TestSetUp()
        {
            if (driver == null)
            {
                SetupBrowser(browserType); // Ensure the browser is set up if not already done
            }

            // Pass RestartBrowser method as a delegate to FileUploadPage
            fileUploadPage = new FileUploadPage(driver!, RestartBrowser); // Initialize the page object

            // Adjust timeout specifically for Edge if it's detected
            if (browserType == BrowserType.Edge)
            {
                Logger.Log("Adjusting timeout for Edge browser...");
                fileUploadPage.TimeoutInSeconds = 120; // Increase timeout for Edge due to slowness
            }

            // Generate pre-defined test files
            GenerateTestFiles();


            NavigateToFileUploadWithRetries(3);
        }

        private void GenerateTestFiles()
        {
            if (!Directory.Exists(FileDirectory))
            {
                Directory.CreateDirectory(FileDirectory);
            }

            // Generate files of varying sizes
            FileGenerator.CreateFile(FileDirectory, ValidFileName, ValidFileSize); // Valid file
            FileGenerator.CreateFile(FileDirectory, LargeFileName, LargeFileSize); // Large file to trigger error
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadSizeLimit(BrowserType browserType)
        {
            this.browserType = browserType;

            string largeFilePath = Path.Combine(FileDirectory, LargeFileName);
            bool isAppErrorPresent = false;
            int maxRetries = 2;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Logger.Log($"[Attempt {attempt}] Starting FileUploadSizeLimit test on {browserType}...");

                    // Attempt to upload the large file
                    fileUploadPage!.UploadFile(largeFilePath);

                    // Check for Heroku application error
                    isAppErrorPresent = fileUploadPage!.IsApplicationErrorPresent();
                    Assert.That(isAppErrorPresent, Is.True, "Expected an application error due to large file upload.");
                    break; // Exit loop if the test passes
                }
                catch (WebDriverTimeoutException ex)
                {
                    Logger.Log($"[Attempt {attempt}] Timeout detected during navigation or upload: {ex.Message}", Logger.LogLevel.Error);

                    if (attempt < maxRetries)
                    {
                        Logger.Log("Retrying navigation or upload after timeout...");
                        ReinitializeBrowser(browserType);
                    }
                    else
                    {
                        CaptureErrorDetails($"Timeout_Error_Attempt{attempt}");
                        Assert.Fail($"Test failed after max retries due to timeout: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"[Attempt {attempt}] Unexpected exception caught: {ex.Message}", Logger.LogLevel.Error);

                    // Capture evidence
                    CaptureErrorDetails($"Unexpected_Error_Attempt{attempt}");

                    if (attempt >= maxRetries)
                    {
                        Assert.Fail($"Test failed with unexpected exception after {maxRetries} attempts: {ex.Message}");
                    }
                }
            }

            if (!isAppErrorPresent)
            {
                Assert.That(false, Is.True, "Expected an error for large file uploads, but the upload succeeded.");
            }

            Logger.Log("FileUploadSizeLimit test completed successfully.");
            
        }

        private void CaptureErrorDetails(string v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reinitializes the browser instance to recover from errors.
        /// </summary>
        private void ReinitializeBrowser(BrowserType browserType)
        {
            Logger.Log("Reinitializing browser instance...");
            DriverManager.
        }

        /// <summary>
        /// Retry mechanism for navigating to File Upload page
        /// </summary>
        /// <param name="maxRetries">Maximum number of retry attempts</param>
        public void NavigateToFileUploadWithRetries(int maxRetries)
        {
            maxRetries = 3;
            int attempts = 0;

            while (attempts < maxRetries)
            {
                try
                {
                    attempts++;
                    Logger.Log($"[Retry] Attempt {attempts}: Navigating to File Upload page (Edge).");
                    fileUploadPage!.NavigateToFileUpload();
                    return; // Exit if navigation succeeds
                }
                catch (WebDriverTimeoutException ex)
                {
                    Logger.Log($"[Info] Timeout occurred during navigation: {ex.Message}");
                    ScreenshotHelper.TakeScreenshot(driver!, $"NavigateToFileUploadTimeout_Attempt{attempts}.png");

                    if (attempts >= maxRetries)
                    {
                        Logger.Log("[Error] Max retries reached. Failing test.", Logger.LogLevel.Error);
                        throw new Exception("Navigation failed after multiple retries.", ex);
                    }
                }
            }
            
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadValidFile(BrowserType browserType)
        {
            this.browserType = browserType;

            try
            {
                string validFilePath = Path.Combine(FileDirectory, ValidFileName);

                Logger.Log($"Uploading valid file: {validFilePath}");
                fileUploadPage!.UploadFile(validFilePath);

                string uploadedFileName = fileUploadPage.GetUploadedFileName();
                Assert.That(uploadedFileName, Is.EqualTo(ValidFileName), "Uploaded file name mismatch for valid file.");
                Logger.Log("Valid file upload test passed.");
            }
            catch (TimeoutException ex)
            {
                Logger.Log($"Timeout occurred: {ex.Message}. Restarting browser...", Logger.LogLevel.Error);
                RestartBrowser();
                Assert.Fail($"Test failed due to timeout: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                Logger.Log($"Test failed with exception : {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver!, "FileUploadValidFileError.png"); // Take screenshot on failure
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }

        }

    }
}
