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

        //Navigate to the File Upload page and handle the iframe context
        public void NavigateToFileUpload()
        {
            try 
            {
                driver!.Navigate().GoToUrl(FileUploadUrl);
                Logger.Log("Navigated to File Upload page successfully.");

                IWebElement iframeElement = WaitForElementToBeVisible(iframeLocator);
                driver.SwitchTo().Frame(iframeElement);

                Logger.Log("Switched to iframe successfully.");
            }
            
        }

        // Method to upload a file
        public void UploadFile(string filePath)
        {
            try
            {
                Logger.Log("Waiting for file upload input...");
                var uploadInput = WaitForElementToBeVisible(fileUploadInput);
                Logger.Log("File upload input located.");
                uploadInput.SendKeys(filePath);

                Logger.Log("Waiting for upload button...");
                var uploadBtn = WaitForElementToBeVisible(uploadButton);
                Logger.Log("Upload button located. Clicking upload.");
                uploadBtn.Click();
            }
            catch (Exception ex)
            {
                Logger.Log($"UploadFile failed: {ex.Message}", Logger.LogLevel.Error);
                throw;
            }
        }

        // Method to get the uploaded file name
        public string GetUploadedFileName()
        {
            try
            {
                Logger.Log("Waiting for uploaded file name...");
                var uploadedFile = WaitForElementToBeVisible(uploadedFileMessage);
                Logger.Log("Uploaded file name located.");
                return uploadedFile.Text;
            }
            catch (Exception ex)
            {
                Logger.Log($"GetUploadedFileName failed: {ex.Message}", Logger.LogLevel.Error);
                throw;
            }
        }

        // Utility method to wait for an element to be visible
        private IWebElement WaitForElementToBeVisible(By locator)
        {
            // Wait for the page to load completely
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
            Logger.Log($"Waiting for element: {locator}");
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator)); // Wait until the file input is visible
        }
    }
}
