using HerokuAppAutomation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace HerokuAppAutomation.Pages
{
    public class FileUploadPage
    {
        private readonly IWebDriver driver;
        private const string FileUploadUrl = "https://the-internet.herokuapp.com/upload";
        public int TimeoutInSeconds = 60;

        // Locators for File Upload
        private By fileUploadInput = By.Id("file-upload");
        private By uploadButton = By.Id("file-submit");
        private By uploadedFileMessage = By.Id("uploaded-files");

        // Constructor
        public FileUploadPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver), "Driver cannot be null.");
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

                // Wait for file upload input to be visible
                Logger.Log("Waiting for file upload input element to become visible...");
                wait.Until(ExpectedConditions.ElementIsVisible(fileUploadInput));
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.Log($"Timeout occurred while waiting for file upload input: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "NavigateToFiLeUploadTimeout.png");
                throw new Exception($"NavigateToFileUpload failed due to timeout: {ex.Message}");
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
    }
}
