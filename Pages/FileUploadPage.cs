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

                // Validate if the correct URL is loaded
                if (driver!.Url.Contains("upload")) 
                {
                    throw new Exception($"Driver failed to navigate to the correct URL. Current URL: {driver.Url}");
                }
                Logger.Log("Navigated to File Upload page successfully.");

                // Wait for file upload input to be visible
                Logger.Log("Waiting for file upload input element to become visible...");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
                IWebElement uploadInput = wait.Until(ExpectedConditions.ElementIsVisible(fileUploadInput));
                Logger.Log("File upload input is visible and ready for interaction.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.Log($"Timeout occurred while waiting for file upload input: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "NavigateToFiLeUploadTimeout.png");
                throw new Exception($"NavigateToFileUpload failed due to timeout: {ex.Message}");
            }
            catch (NoSuchElementException ex)
            {
                Logger.Log($"Element not found during navigation: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "NavigateToFileUploadNoSuchElement.png");
                throw new Exception($"NavigateToFileUpload failed due to missing elements: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Unexpected error during navigation: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "NavigateToFileUploadFailure.png");
                throw new Exception($"NavigateToFileUpload failed: {ex.Message}");
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
                // Validate file path
                if (!File.Exists(filePath))
                {
                    throw new Exception($"File not found at: {filePath}");
                }

                NavigateToFileUpload();

                // Wait for file input and upload button to become interactable
                Logger.Log("Waiting for file input to become interactable...");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
                IWebElement uploadInput = wait.Until(ExpectedConditions.ElementToBeClickable(fileUploadInput));
                Logger.Log("File input is interactable.");

                uploadInput.SendKeys(filePath);
                Logger.Log($"File path '{filePath}' sent to the file input.");

                IWebElement uploadBtn = wait.Until(ExpectedConditions.ElementToBeClickable(uploadButton));
                Logger.Log("Upload button is interactable.");

                uploadBtn.Click();
                Logger.Log("Clicked the upload button successfully.");
            }
            catch (Exception ex)
            {
                Logger.Log($"UploadFile failed: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "UploadFileFailure.png");
                throw new Exception($"UploadFile failed: {ex.Message}");
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
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator)); // Wait until the file input is visible
        }
    }
}
