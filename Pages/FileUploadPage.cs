using HerokuAppAutomation.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HerokuAppAutomation.Pages
{
    public class FileUploadPage
    {
        private readonly IWebDriver driver;
        private const string FileUploadUrl = "https://the-internet.herokuapp.com/upload";
        private const int TimeoutInSeconds = 60;

        // Locators for File Upload
        private By iframeLocator = By.XPath("//iframe[@src='www.herokucdn.com/error-pages/application-error.html']");
        private By fileUploadInput = By.Id("file-upload");
        private By uploadButton = By.Id("file-submit");
        private By uploadedFileMessage = By.Id("uploaded-files");

        // Constructor
        public FileUploadPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver), "Driver cannot be null.");
        }

        /// <summary>
        /// Navigate to the File Upload page and handle the iframe context
        /// </summary>
        public void NavigateToFileUpload()
        {
            try 
            {
                driver!.Navigate().GoToUrl(FileUploadUrl);
                Logger.Log("Navigated to File Upload page successfully.");

                // Switch to iframe if it exists
                IWebElement iframeElement = WaitForElementToBeVisible(iframeLocator);
                driver.SwitchTo().Frame(iframeElement);

                Logger.Log("Switched to iframe successfully.");
            }
            catch (Exception ex)
            {
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
