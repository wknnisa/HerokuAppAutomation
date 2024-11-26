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
        private const int TimeoutInSeconds = 60;

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
                Logger.Log("Navigating to File Upload page...");
                driver!.Navigate().GoToUrl(FileUploadUrl);
                Logger.Log("Navigated to File Upload page successfully.");

                if (driver!.Url != FileUploadUrl) 
                {
                    throw new Exception($"Driver failed to navigate to the correct URL. Current URL: {driver.Url}");
                }

                // Explicit wait for the file upload input to be visible
                Logger.Log("Waiting for file upload input element to become visible...");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
                wait.Until(ExpectedConditions.ElementIsVisible(fileUploadInput));
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
                NavigateToFileUpload();

                // Wait for file input and upload button
                IWebElement uploadInput = WaitForElementToBeVisible(fileUploadInput);
                uploadInput.SendKeys(filePath);
                Logger.Log($"File path '{filePath}' sent to the file input.");

                IWebElement uploadBtn = WaitForElementToBeVisible(uploadButton);
                uploadBtn.Click();
                Logger.Log("Clicked the upload button successfully.");
            }
            catch (Exception ex)
            {
                ScreenshotHelper.TakeScreenshot(driver, "UploadFileFailure.png");
                throw new Exception($"UploadFile failed: {ex.Message}");
            }
            finally
            {
                // Ensure switching back to default content
                driver.SwitchTo().DefaultContent();
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
