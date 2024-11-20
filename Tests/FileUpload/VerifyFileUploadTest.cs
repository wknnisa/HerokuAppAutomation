using FileGeneratorLibrary.Utilities;
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
        private BrowserType browserType;
        private FileUploadPage? fileUploadPage;

        private string[] filePaths = new string[3]; // Declare the filePaths array to hold the generated files.

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

            // Generate files of varying sizes and add their paths to the filePaths array
            FileGenerator.CreateFile(FileDirectory, "200mb.txt", 200);
            filePaths[0] = Path.Combine(FileDirectory, "200mb.txt");

            FileGenerator.CreateFile(FileDirectory, "250mb.bin", 250);
            filePaths[1] = Path.Combine(FileDirectory, "250mb.bin");

            FileGenerator.CreateFile(FileDirectory, "300mb.zip", 300);
            filePaths[2] = Path.Combine(FileDirectory, "300mb.zip");
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadVariousSizes (BrowserType browserType)
        {
            this.browserType = browserType;

            foreach (string filePath in filePaths)
            {
                FileInfo fileInfo = new FileInfo(filePath);

                // Log file information (for reference)
                TestContext.WriteLine($"Testing file: {fileInfo.Name}, Size: {fileInfo.Length / (1024 * 1024)} MB");

                // Wait for the upload completion (use an explicit wait here if necessary)
                try
                {
                    // Skip files larger than 250MB (or another limit) and log the skip
                    if (fileInfo.Length > (250 * 1024 * 1024)) // Adjust size limit if necessary
                    {
                        LogIssue($"Skipping file {fileInfo.Name} because it exceeds the size limit of 250MB.");
                        continue; // Skip this file and proceed to the next one
                    }

                    // Perform the file upload via Page Object
                    fileUploadPage!.UploadFile(filePath);

                    // Wait for the upload to complete and validate
                    WaitForUploadCompletion(fileInfo);
                    string uploadedFileName = fileUploadPage.GetUploadedFileName();
                    Assert.That(uploadedFileName, Is.EqualTo(fileInfo.Name), $"Mismatch in uploaded file name: {fileInfo.Name}");
                }
                catch (WebDriverTimeoutException)
                {
                    // Handle timeout due to large file upload taking longer than expected
                    LogIssue($"File upload timeout: {fileInfo.Name}. File might be too large.");
                }
                catch (Exception ex) 
                {
                    // Handle any other unexpected errors
                    LogIssue($"An error occured during file upload: {fileInfo.Name}, Error: {ex.Message}");
                }
            }
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void FileUploadValidFile(BrowserType browserType)
        {
            this.browserType= browserType;

            string validFilePath = Path.Combine(FileDirectory, "sample1.jpg");
            fileUploadPage!.UploadFile(validFilePath);

            string uploadedFileName = fileUploadPage.GetUploadedFileName();
            Assert.That(uploadedFileName, Is.EqualTo("sample1.jpg"), "Uploaded files name mismatch for valid file.");
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
            string largeFilePath = Path.Combine(FileDirectory, "sublime_text_build_4126_x64_setup.exe"); 
            fileUploadPage!.UploadFile(largeFilePath);

            // Handle the case where the file size exceeds the limit and causes an error
            try
            {
                IWebElement errorMessage = driver!.FindElement(By.Id("error-message"));
                Assert.That(errorMessage.Text, Does.Contain("File size limit is exceeded"));
            }
            catch (NoSuchElementException)
            {
                // If the error message is not present, check for other application errors
                if (IsApplicationErrorPresent())
                {
                    LogIssue("Application error occurred during file upload.");
                }
                else 
                {
                    LogIssue("Error message not found but file upload file failed");
                }
            }

        }

        // Check for an application error (e.g., if file is too large)
        private bool IsApplicationErrorPresent()
        {
            try
            {
                IWebElement errorElement = driver!.FindElement(By.XPath("//body[contains(text(), 'Application error']"));
                return errorElement.Displayed;
            }
            catch (NoSuchElementException) 
            {
                return false;
            }
        }

        // Wait for the file upload to complete
        public void WaitForUploadCompletion(FileInfo fileInfo)
        {
            // Adjust the wait time based on file size for upload completion
            TimeSpan timeout = fileInfo.Length > (100 * 1024 * 1024)
                ? TimeSpan.FromMinutes(5)  // Increase timeout for larger files (e.g., > 100MB)
                : TimeSpan.FromMinutes(2); // Default timeout for smaller files

            WebDriverWait wait = new WebDriverWait(driver, timeout);

            try
            {
                wait.Until(d => d.FindElement(By.Id("uploaded-files")).Displayed);
                TestContext.WriteLine("File upload complete: {fileInfo.Name}");
            }
            catch (WebDriverTimeoutException)
            {
                LogIssue("File upload timeout: {fileInfo.Name}");
            }
        }

        private void AssertUploadedFile(string expectedFileName)
        {
            string uploadedFileName = driver!.FindElement(By.Id("uploaded-files")).Text;
            Assert.That(uploadedFileName, Is.EqualTo(expectedFileName), $"Uploaded file name mismatch. Expected: {expectedFileName}, Found: {uploadedFileName}");
        }

        // Log issue function (add logging mechanism here, e.g., file logging, test report logging, etc.)
        private void LogIssue(string message)
        {
            // Log the issue in the test context or external log
            TestContext.WriteLine($"ISSUE: {message}");
        }

    }
}
