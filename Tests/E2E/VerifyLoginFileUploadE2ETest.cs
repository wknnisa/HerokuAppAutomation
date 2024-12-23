using FileGeneratorLibrary.Utilities;
using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages;
using NUnit.Framework;

namespace HerokuAppAutomation.Tests.E2E
{
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

            FileGenerator.CreateFile(FileDirectory, ValidFileName, ValidFileSize);
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void VerifyLoginAndFileUpload(BrowserType browserType)
        {
            this.browserType = browserType;

            try
            {
                loginPage!.NavigateToLogin();
                loginPage.Login("tomsmith", "SuperSecretPassword!");
                Assert.That(driver!.Url, Is.EqualTo(SecureUrl), "Login was not successful.");

                fileUploadPage!.NavigateToFileUpload();
            }
        }
    }
}
