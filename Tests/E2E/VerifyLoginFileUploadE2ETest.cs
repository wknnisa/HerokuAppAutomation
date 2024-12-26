using FileGeneratorLibrary.Utilities;
using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages; // LoginPage and FileUploadPage classes
using HerokuAppAutomation.Utilities;
using NUnit.Framework;

namespace HerokuAppAutomation.Tests.E2E
{
    [TestFixture]
    public class VerifyLoginFileUploadE2ETest : BaseTest
    {
        private LoginPage? loginPage;
        private FileUploadPage? fileUploadPage;

        private const string SecureUrl = "https://the-internet.herokuapp.com/secure";
        private const string UploadUrl = "https://the-internet.herokuapp.com/upload";
        private const string FileDirectory = @"C:\TestFiles\";
        private const string ValidFileName = "200mb.txt";
        private const int ValidFileSize = 200;

        [SetUp]
        public void TestSetUp()
        {
            // Ensure browser is set up
            if (driver == null)
            {
                SetupBrowser(browserType);
            }

            // Initialize LoginPage and FileUploadPage
            loginPage = new LoginPage(driver!);
            fileUploadPage = new FileUploadPage(driver!, RestartBrowser);

            // Generate the test file if needed
            GenerateTestFiles();
        }

        private void GenerateTestFiles()
        {
            if (!Directory.Exists(FileDirectory))
            {
                Directory.CreateDirectory(FileDirectory);
            }

            // Generate a valid file for upload
            FileGenerator.CreateFile(FileDirectory, ValidFileName, ValidFileSize);
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void VerifyLoginAndFileUpload(BrowserType browserType)
        {
            this.browserType = browserType;
            int maxRetries = 3; // Maximum retry attempts
            int attempts = 0;

            while (attempts < maxRetries)
            {
                try
                {
                    Logger.Log($"Attempt {attempts + 1} of {maxRetries}");

                    // Step 1: Login
                    loginPage!.NavigateToLogin();
                    loginPage.Login("tomsmith", "SuperSecretPassword!");
                    Assert.That(driver!.Url, Is.EqualTo(SecureUrl), "Login was not successful.");

                    // Step 2: Navigate to File Upload page
                    fileUploadPage!.NavigateToFileUpload();

                    // Step 3: Upload a valid file
                    string validFilePath = Path.Combine(FileDirectory, ValidFileName);
                    fileUploadPage.UploadFile(validFilePath);

                    // Step 4: Verify the uploaded file name
                    string uploadedFileName = fileUploadPage.GetUploadedFileName();
                    Assert.That(uploadedFileName, Is.EqualTo(ValidFileName), "Uploaded file name does not match.");

                    Logger.Log("E2E Login and File Upload test passed successfully.");
                    return; // Exit the loop on success
                }
                catch (Exception ex)
                {
                    attempts++;
                    Logger.Log($"Attempt {attempts} failed: {ex.Message}", Logger.LogLevel.Warning);
                    ScreenshotHelper.TakeScreenshot(driver!, $"E2ETestError_Attempt{attempts}.png");

                    if (attempts == maxRetries)
                    {
                        Logger.Log("Max retry attempts reached. Failing the test.", Logger.LogLevel.Error);
                        Assert.Fail($"E2E test failed after {maxRetries} retries: {ex.Message}");
                    }

                    // Optional: Restart the browser for a clean slate
                    RestartBrowser();
                }
            }
        }
    }
}
