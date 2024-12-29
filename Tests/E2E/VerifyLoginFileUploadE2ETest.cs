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
        private const int MaxRetries = 3; // Retry limit

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

            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    Logger.Log($"Attempt {attempt + 1} of {MaxRetries}");

                    // Run the core E2E test logic
                    RunFileUploadTest();

                    Logger.Log("E2E Login and File Upload test passed successfully.");
                    return; // Exit on success
                }
                catch (Exception ex)
                {
                    Logger.Log($"Attempt {attempt} failed: {ex.Message}", Logger.LogLevel.Warning);

                    if (attempt < MaxRetries)
                    {
                        Logger.Log("Restarting the browser for a fresh attempt...");
                        RestartBrowser(); // Ensures clean state before retrying
                    }
                    else
                    {
                        Logger.Log("Max retry attempts reached. Failing the test.", Logger.LogLevel.Error);
                        ScreenshotHelper.TakeScreenshot(driver!, $"E2ETestError_Attempt{attempt}.png");
                        Assert.Fail($"E2E test failed after {MaxRetries} attempts: {ex.Message}");
                    }
                }
            }
        }

        private void RunFileUploadTest()
        {
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
        }
    }
}
