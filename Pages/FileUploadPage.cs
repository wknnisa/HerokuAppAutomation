using OpenQA.Selenium;

namespace HerokuAppAutomation.Pages
{
    public class FileUploadPage
    {
        private readonly IWebDriver driver;
        private const string FileUploadUrl = "https://the-internet.herokuapp.com/upload";

        // Constructor
        public FileUploadPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver), "Driver cannot be null.");
        }

        // Locators for File Upload
        private By fileUploadInput = By.Id("file-upload");
        private By uploadButton = By.Id("file-submit");
        private By uploadedFileMessage = By.Id("uploaded-files");

        // Method to navigate to the File Upload page
        public void NavigateToFileUpload()
        {
            driver!.Navigate().GoToUrl(FileUploadUrl);
        }

        // Method to upload a file
        public void UploadFile(string filePath)
        {
            driver.FindElement(fileUploadInput).SendKeys(filePath);
            driver.FindElement(uploadButton).Click();
        }

        // Method to get the uploaded file name
        public string GetUploadedFileName()
        {
            return driver.FindElement(uploadedFileMessage).Text;
        }
    }
}
