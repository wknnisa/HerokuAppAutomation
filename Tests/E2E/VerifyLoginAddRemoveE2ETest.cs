using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages;
using NUnit.Framework;

namespace HerokuAppAutomation.Tests.E2E
{
    [TestFixture]
    public class VerifyLoginAddRemoveE2ETest : BaseTest
    {
        private LoginPage? loginPage;
        private AddRemoveElementsPage? addRemoveElementsPage;
        private BrowserType browserType;

        [SetUp]
        public void TestSetUp()
        {
            if (driver == null)
            {
                SetupBrowser(browserType);
            }

            // Assert that the driver is initialized
            Assert.That(driver, Is.Not.Null, "Driver is not initialized.");

            // Initialize page objects
            loginPage = new LoginPage(driver!);
            addRemoveElementsPage = new AddRemoveElementsPage(driver!);

            // Navigate to the login page for the test
            loginPage.NavigateToLogin();
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void LoginAddRemoveE2E(BrowserType browserType)
        {
            this.browserType = browserType;

            // Step 1: Log in
            loginPage!.Login("tomsmith", "SuperSecretPassword!");
            Assert.That(driver!.Url, Is.EqualTo("https://the-internet.herokuapp.com/secure"), "Failed to login.");

            // Step 2: Navigate to Add/Remove Elements page
            addRemoveElementsPage!.NavigateToAddRemoveElements();

            // Step 3: Add elements
            int elementsToAdd = 5; // Number of elements to add

            for (int i = 0; i < elementsToAdd; i++)
            {
                addRemoveElementsPage!.AddElement();
            }

            // Step 4: Verify added elements
            int deleteButtonCount = addRemoveElementsPage!.GetDeleteButtonCount();
            Assert.That(deleteButtonCount, Is.EqualTo(elementsToAdd), $"Expected {elementsToAdd} 'Delete' buttons, but found {deleteButtonCount}.");

            // Step 5: Remove elements
            for (int i = elementsToAdd - 1; i >= 0; i--)
            {
                addRemoveElementsPage!.RemoveElement();
                deleteButtonCount = addRemoveElementsPage!.GetDeleteButtonCount();
                Assert.That(deleteButtonCount, Is.EqualTo(i), $"Expected {i} 'Delete' buttons remaining, but found {deleteButtonCount}.");
            }

            // Step 6: Verify login persistence
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com/secure");
            Assert.That(driver.Url, Is.EqualTo("https://the-internet.herokuapp.com/secure"));
        }
    }
}
