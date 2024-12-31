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
                // Navigate directly to the File Upload URL
                driver!.Navigate().GoToUrl(FileUploadUrl);

                // Wait for the page to load and ensure the file upload input is ready
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
                wait.Until(ExpectedConditions.ElementIsVisible(fileUploadInput));
                wait.Until(ExpectedConditions.ElementToBeClickable(fileUploadInput));

                Logger.Log("Successfully navigated to the File Upload page.");
            }
            catch (WebDriverTimeoutException ex)
            {
                Logger.Log($"Navigation timeout: {ex.Message}", Logger.LogLevel.Error);
                ScreenshotHelper.TakeScreenshot(driver, "NavigateTimeout.png");
                throw new Exception($"Failed to navigate to the File Upload page: {ex.Message}");
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
                    // Ensure no application error is present
                    EnsureNoApplicationError() ;

                    // Locate the file input and interact with it
                    var fileInput = WaitForElementToBeClickable(fileUploadInput);
                    fileInput.SendKeys(filePath);

                    // Locate the upload button and click it
                    var uploadButtonElement = WaitForElementToBeClickable(uploadButton);
                    uploadButtonElement.Click();

                    Logger.Log("File upload initiated successfully.");

                    // Verify no application error occurred after clicking upload
                    EnsureNoApplicationError();

                    return; // Exit the loop if successful.
                }

                catch(StaleElementReferenceException ex)
                {
                    retryCount++;
                    Logger.Log($"Stale element encountered. Retrying upload ({retryCount}/{maxRetries}): {ex.Message}", Logger.LogLevel.Warning);

                    // Refresh page to ensure elements are reset
                    RefreshPage();
                }
                catch(WebDriverException ex) when (ex.Message.Contains("disposed object"))
                {
                    retryCount++;
                    Logger.Log($"WebDriver exception: {ex.Message}. Retrying upload ({retryCount}/{maxRetries})", Logger.LogLevel.Warning);

                    // Restart browser to ensure WebDriver is fresh
                    restartBrowserCallback.Invoke();
                }

                catch (Exception ex)
                {
                    retryCount++;
                    Logger.Log($"Error during upload attempt ({retryCount}/{maxRetries}): {ex.Message}", Logger.LogLevel.Warning);

                    if (retryCount == maxRetries)
                    {
                        ScreenshotHelper.TakeScreenshot(driver, "UploadError.png");
                        throw;
                    }

                    // Optional: Refresh page or restart browser for next retry
                    if (retryCount < maxRetries)
                    {
                        RefreshPage();
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the current page to reset its state.
        /// </summary>
        private void RefreshPage()
        {
            driver.Navigate().Refresh();
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
            wait.Until(ExpectedConditions.ElementIsVisible(fileUploadInput));
            Logger.Log("Page refreshed successfully.");
        }

        /// <summary>
        /// Waits for an element to be clickable.
        /// </summary>
        /// <param name="locator">The locator of the element to wait for.</param>
        /// <returns>The clickable IWebElement.</returns>
        private IWebElement WaitForElementToBeClickable(By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(TimeoutInSeconds));
            return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
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
            // Check if the application error iframe is present
            if (driver.FindElements(By.CssSelector("iframe[src*='application-error.html']")).Any())
            {
                // Capture a screenshot if application error iframe is found
                ScreenshotHelper.TakeScreenshot(driver, "ApplicationErrorDetected.png");

                // Throw an exception to stop further test execution, indicating an application error
                throw new Exception("Application error detected. Test cannot proceed");
            }
        }
    }
}
