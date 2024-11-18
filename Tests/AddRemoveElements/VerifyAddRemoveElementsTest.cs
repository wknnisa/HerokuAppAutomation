using HerokuAppAutomation.Base;
using HerokuAppAutomation.Pages;
using NUnit.Framework;

namespace HerokuAppAutomation.Tests.AddRemoveElements
{
    [TestFixture]
    public class VerifyAddRemoveElementsTest : BaseTest
    {
        private AddRemoveElementsPage? addRemoveElementsPage;
        private BrowserType browserType;

        [SetUp]
        public void TestSetup()
        {
            if (driver == null) 
            { 
                SetupBrowser(browserType);
            }

            Assert.That(driver, Is.Not.Null, "Driver is not initialized.");
            addRemoveElementsPage = new AddRemoveElementsPage(driver!);

            addRemoveElementsPage.NavigateToAddRemoveElements();
        }

        [Test]
        [TestCase(BrowserType.Chrome)]
        [TestCase(BrowserType.Firefox)]
        [TestCase(BrowserType.Edge)]
        public void AddRemoveMultipleElementsTest(BrowserType browserType)
        {
            this.browserType = browserType;

            int elementsToAdd = 5; // Number of elements to add

            // First Loop: Add elements
            // Loop to click "Add Element" button multiple times
            for (int i = 0; i < elementsToAdd; i++)
            {
                addRemoveElementsPage!.AddElement();
            }

            // Verify correct number of "Delete" buttons
            int deleteButtonCount = addRemoveElementsPage!.GetDeleteButtonCount();
            Assert.That(deleteButtonCount, Is.EqualTo(elementsToAdd), $"Expected {elementsToAdd} 'Delete' buttons, but found {deleteButtonCount}");

            // Second Loop: Remove elements one by one
            // starts from the last "Delete" button
            for (int i = elementsToAdd - 1; i >= 0; i--)
            {
                addRemoveElementsPage!.RemoveElement();
                deleteButtonCount = addRemoveElementsPage!.GetDeleteButtonCount();
                Assert.That(deleteButtonCount, Is.EqualTo(i), $"Expected {i} 'Delete' buttons remaining, but found {deleteButtonCount}.");
            }

            // Verify no "Delete" buttons remain
            deleteButtonCount = addRemoveElementsPage!.GetDeleteButtonCount();
            Assert.That(deleteButtonCount, Is.EqualTo(0), "Expected no 'Delete' buttons remaining.");
        }
    }
}
