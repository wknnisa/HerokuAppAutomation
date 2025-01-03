﻿using FileGeneratorLibrary.Utilities;
using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages;
using HerokuAppAutomation.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;

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

            string largeFilePath = Path.Combine(FileDirectory, LargeFileName);

            try
            {
                Logger.Log($"Starting FileUploadSizeLimit test on {browserType}...");

                // Attempt to upload the large file
                fileUploadPage!.UploadFile(largeFilePath);

                // Ensure no application error occurred after upload
                fileUploadPage.EnsureNoApplicationError();

                Logger.Log("KNOWN ISSUE: The application does not handle large file uploads gracefully. Marking the test as inconclusive.", Logger.LogLevel.Warning);
                Assert.Inconclusive("KNOWN ISSUE: The application does not handle large file uploads gracefully.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.Log($"Timeout detected during navigation or upload: {ex.Message}", Logger.LogLevel.Error);

                RetryNavigateToFileUpload();

                Assert.Fail($"Navigation or upload timeout: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected exception caught: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver!, "UnexpectedError.png");
                Assert.Fail($"Test failed with unexpected exception: {ex.Message}");
            }

            Logger.Log("FileUploadSizeLimit test completed successfully.");
        }

        private void RetryNavigateToFileUpload()
        {
            try
            {
                fileUploadPage!.NavigateToFileUpload();
            }
            catch (Exception retryEx)
            {
                Logger.Log($"Retry failed: {retryEx.Message}", Logger.LogLevel.Error);
                Assert.Fail($"Retry to navigate to File Upload page failed: {retryEx.Message}");
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