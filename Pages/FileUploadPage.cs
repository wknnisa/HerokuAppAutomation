using HerokuAppAutomation.Utilities;
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
                // Clear cookies and refresh the page to ensure a clean session
                driver.Manage().Cookies.DeleteAllCookies(); // Delete all cookies to start fresh
                driver.Navigate().Refresh(); // Refresh to ensure the page reloads from a clean state
                driver!.Navigate().GoToUrl(FileUploadUrl);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
                wait.Until(d => d.Url.Contains("upload"));

                if (IsApplicationErrorPresent())
                {
                    ScreenshotHelper.TakeScreenshot(driver, "ApplicationErrorDetected.png");
                    throw new Exception("Navigation failed due to an application error.");
                }

                // Wait for file upload input to be visible
                wait.Until(ExpectedConditions.ElementIsVisible(fileUploadInput));
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.Log($"Timeout occurred while navigating: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "NavigateTimeout.png");
                throw new Exception($"Timeout during navigation: {ex.Message}");
            }
        }

        /// <summary>
        /// Upload a file using the file input and submit
        /// </summary>
        /// <param name="filePath">Path of the file to be uploaded</param>
        public void UploadFile(string filePath)
        {
            int retryCount = 0;
            const int maxRetries = 3;

            while (retryCount < maxRetries)
            {
                try
                {
                    // Interact with the file input
                    var fileInput = driver.FindElement(fileUploadInput);
                    fileInput.SendKeys(filePath);

                    // Click the upload button
                    var uploadButtonElement = driver.FindElement(uploadButton);
                    uploadButtonElement.Click();

                    Logger.Log("File upload initiated successfully.");

                    if (IsApplicationErrorPresent())
                    {
                        ScreenshotHelper.TakeScreenshot(driver, "UploadError.png");
                        throw new Exception("Application error detected during file upload.");
                    }

                    return;
                }

                catch (Exception ex)
                {
                    retryCount++;
                    Logger.Log($"Retrying file upload ({retryCount}/{maxRetries}): {ex.Message}", Logger.LogLevel.Warning);
                    if (retryCount == maxRetries) throw;
                }
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
                IWebElement uploadedFile = WaitForElementToBeVisible(uploadedFileMessage);
                return uploadedFile.Text;
            }
            catch (Exception ex)
            {
                ScreenshotHelper.TakeScreenshot(driver, "GetUploadedFileNameError.png");
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
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        /// <summary>
        /// Method to check if the application error iframe is present.
        /// </summary>
        public bool IsApplicationErrorPresent()
        {
            try
            {
                // Locate the iframe by src or other identifying attributes
                var errorIframe = driver!.FindElement(By.CssSelector("iframe[src*='application-error.html']"));
                return errorIframe.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false; // If iframe is not found, assume no application error
            }
        }

        public void EnsureNoApplicationError()
        { 
            if (driver.FindElements(By.CssSelector("iframe[src*='application-error.html']")).Any())
            {
            }
        }
    }
}
