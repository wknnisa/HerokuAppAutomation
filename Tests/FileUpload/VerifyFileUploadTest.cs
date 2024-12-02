﻿using FileGeneratorLibrary.Utilities;
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

        private BrowserType browserType;
        private FileUploadPage? fileUploadPage;

        [SetUp]
        public void TestSetUp()
        {
            if (driver == null)
            {
                SetupBrowser(browserType); // Ensure the browser is set up if not already done
            }

            fileUploadPage = new FileUploadPage(driver!); // Initialize the page object

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
            catch (Exception ex)
            {
                // Handle any unexpected errors
                Logger.Log($"Test failed with exception : {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver!, "FileUploadValidFileError.png"); // Take screenshot on failure
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }

        }

        /// <summary>
        /// Method to check if the application error iframe is present.
        /// </summary>
        private bool IsApplicationErrorPresent()
        {
            try
            {
                // Locate the iframe by src or other identifying attributes
                var errorIframe = driver!.FindElement(By.CssSelector("iframe[src='https://www.herokucdn.com/error-pages/application-error.html']"));

                if (errorIframe.Displayed)
                {
                    Logger.Log("Application error iframe detected.");
                    return true;
                }
                return false;
            }
            catch (NoSuchElementException)
            {
                Logger.Log("No application error iframe found.", Logger.LogLevel.Info);
                return false; // If iframe is not found, assume no application error
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error while checking for application error iframe: {ex.Message}", Logger.LogLevel.Error);
                return false; // Fail-safe: return false on unexpected exceptions
            }
        }

        /// <summary>
        /// Method to restart the browser if there's a critical failure or timeout
        /// </summary>
        private void RestartBrowser()
        {

            Logger.Log("Restarting browser...");

            driver!.Quit();
            SetupBrowser(browserType); // Restart browser
        }

        /// <summary>
        /// Helper method to handle timeout errors and take appropriate actions
        /// </summary>
        private void HandleTimeoutError(string message)
        {
            Logger.Log($"Timeout error occurred: {message}", Logger.LogLevel.Error);
            ScreenshotHelper.TakeScreenshot(driver!, "TimeoutError.png");

            if (IsApplicationErrorPresent())
            {
                Logger.Log("Detected an application error after timeout.", Logger.LogLevel.Error);
                Assert.Fail("Test failed due to an application error.");
            }
            else
            {
                Logger.Log("No application error detected. Restarting browser to recover.", Logger.LogLevel.Warning);
                RestartBrowser();
            }
        }
    }
}
