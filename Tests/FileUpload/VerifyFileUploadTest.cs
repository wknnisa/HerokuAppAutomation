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

            fileUploadPage!.NavigateToFileUpload(); // Navigate to the File Upload page before each test

            // Generate pre-defined test files
            GenerateTestFiles();
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

            // Path.Combine - combining the directory path and the file name.
            // Path for a large file that should trigger an error
            string largeFilePath = Path.Combine(FileDirectory, LargeFileName);
            Assert.Throws<Exception>(() => fileUploadPage!.UploadFile(largeFilePath), "Expected an error for large file uploads.");

            
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
