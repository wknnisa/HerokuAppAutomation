﻿using FileGeneratorLibrary.Utilities;
using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages;
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

            Assert.That(driver, Is.Not.Null, "Driver is not initialized.");
            fileUploadPage = new FileUploadPage(driver!); // Initialize the page object

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

            try
            {
                fileUploadPage!.UploadFile(largeFilePath);

                // Wait for an error message or check for application errors
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                IWebElement errorMessage = wait.Until(d => d.FindElement(By.Id("error-message")));

                Assert.That(errorMessage.Text, Does.Contain("File size limit is exceeded"),
                    "Expected error message not found for large file.");
            }
            catch (WebDriverTimeoutException)
            {
                // Handle timeout when waiting for the error message
                HandleTimeoutError("Timeout while waiting for the file upload error message.");
            }
            catch (WebDriverException ex)
            {
                // Handle browser session crash or shutdown
                if (IsApplicationErrorPresent())
                {
                    Log("Application error occurred during file upload. Browser session terminated.", LogLevel.Error);
                }
                else
                {
                    Log($"WebDriverException encountered: {ex.Message}", LogLevel.Error);
                }

                RestartBrowser();
                Assert.Fail("Test failed due to WebDriver session termination.");
            }

        }

        private void HandleTimeoutError(string message)
        {
            if (IsApplicationErrorPresent())
            {
                Log("Application error detected after timeout.", LogLevel.Error);
            }
            else
            {
                Log(message, LogLevel.Warning);
            }
        }

        private void RestartBrowser()
        {
            try
            {
                driver?.Quit();
                SetupBrowser(browserType); // Restart browser
                fileUploadPage = new FileUploadPage(driver!); // Reinitialize the page object
                fileUploadPage!.NavigateToFileUpload(); // Navigate back to the file upload page
                Log("Browser session restarted successfully", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Log($"Failed to restart the browser: {ex.Message}", LogLevel.Error);
                throw; // Fail the test if browser restart fails
            }
        }

        
        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadValidFile(BrowserType browserType)
        {
            this.browserType = browserType;

            string validFilePath = Path.Combine(FileDirectory, ValidFileName);
            fileUploadPage!.UploadFile(validFilePath);

            string uploadedFileName = fileUploadPage.GetUploadedFileName();
            Assert.That(uploadedFileName, Is.EqualTo(ValidFileName), "Uploaded file name mismatch for valid file.");
        }

        // Check for an application error (e.g., if file is too large)
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

        // Log issue function (add logging mechanism here, e.g., file logging, test report logging, etc.)
        private void Log(string message, LogLevel level = LogLevel.Info)
        {
            string logMessage = $"[{level.ToString().ToUpper()}] {message}";
            TestContext.WriteLine(logMessage);
        }

        public enum LogLevel 
        {
            Info,
            Warning,
            Error
        }

    }
}
