using HerokuAppAutomation.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace HerokuAppAutomation.Pages
{
    public class FileUploadPage
    {
        private readonly IWebDriver driver;
        private readonly Action restartBrowserCallback; // Delegate to call RestartBrowser from BaseTest
        private const string FileUploadUrl = "https://the-internet.herokuapp.com/upload";
        public int TimeoutInSeconds = 60;

        // Locators for File Upload
        private By fileUploadInput = By.Id("file-upload");
        private By uploadButton = By.Id("file-submit");
        private By uploadedFileMessage = By.Id("uploaded-files");

        // Constructor
        public FileUploadPage(IWebDriver driver, Action restartBrowserCallback)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver), "Driver cannot be null.");
            this.restartBrowserCallback = restartBrowserCallback ?? throw new ArgumentNullException(nameof(restartBrowserCallback), "Restart browser callback cannot be null.");
        }

        /// <summary>
        /// Navigate to the File Upload page and ensure the page is ready.
        /// </summary>
        public void NavigateToFileUpload()
        {
            try 
            {
                Logger.Log("Starting with a clean browser state...");

                // Clear cookies and refresh the page to ensure a clean session
                driver.Manage().Cookies.DeleteAllCookies(); // Delete all cookies to start fresh
                driver.Navigate().Refresh(); // Refresh to ensure the page reloads from a clean state

                Logger.Log("Navigating to File Upload page...");
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120); // Increase page load timeout
                driver!.Navigate().GoToUrl(FileUploadUrl);

                Logger.Log("Waiting for URL to match the expected File Upload URL...");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
                wait.Until(d => d.Url.Contains("upload"));

                if (IsApplicationErrorPresent())
                {
                    Logger.Log("Application error detected in iframe.", Logger.LogLevel.Error);
                    ScreenshotHelper.TakeScreenshot(driver, "ApplicationErrorDetected.png");
                    throw new Exception("Navigation failed due to an application error.");
                }

                // Wait for file upload input to be visible
                Logger.Log("Waiting for file upload input element to become visible...");
                wait.Until(ExpectedConditions.ElementIsVisible(fileUploadInput));
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.Log($"Timeout occurred while waiting for file upload input: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "NavigateToFiLeUploadTimeout.png");
                throw new Exception($"Timeout during navigation: {ex.Message}");
            }
        }

        /// <summary>
        /// Upload a file using the file input and submit
        /// </summary>
        /// <param name="filePath">Path of the file to be uploaded</param>
        public void UploadFile(string filePath)
        {
            try
            {
                // Interact with the file input
                var fileInput = driver.FindElement(By.Id("file-upload"));
                fileInput.SendKeys(filePath);

                // Click the upload button
                var uploadButton = driver.FindElement(By.Id("file-submit"));
                uploadButton.Click();

                Logger.Log("File upload initiated successfully.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.Log($"UploadFile failed: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "UploadFileFailure.png");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error during file upload: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "UploadFileError.png");
                throw;
            }
        }

        /// <summary>
        /// Get the uploaded file name displayed on the page
        /// </summary>
        /// <returns>Uploaded file name as a string</returns>
        public string GetUploadedFileName()
        {
            try
            {
                Logger.Log("Waiting for uploaded file name...");
                IWebElement uploadedFile = WaitForElementToBeVisible(uploadedFileMessage);
                Logger.Log($"Uploaded file name located: {uploadedFile.Text}");
                return uploadedFile.Text;
            }
            catch (Exception ex)
            {
                ScreenshotHelper.TakeScreenshot(driver, "GetUploadedFileNameFailure.png");
                throw new Exception($"GetUploadedFileName failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Utility method to wait until an element is visible
        /// </summary>
        /// <param name="locator">Element locator</param>
        /// <returns>The visible IWebElement</returns>
        private IWebElement WaitForElementToBeVisible(By locator)
        {
            try
            {
                Logger.Log($"Waiting for element to be visible: {locator}");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
                IWebElement element = wait.Until(ExpectedConditions.ElementIsVisible(locator));
                Logger.Log($"Element found and visible: {locator}");
                return element;
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.Log($"Timeout while waiting for element to be visible: {locator}. Error: {ex.Message}", Logger.LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Method to check if the application error iframe is present.
        /// </summary>
        public bool IsApplicationErrorPresent()
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
                restartBrowserCallback.Invoke();
            }
        }
    }
}
