using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HerokuAppAutomation
{
    public class AddRemoveElementsTest
    {
        private IWebDriver? driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver(@"C:\WebDrivers\");
        }

        [Test]
        public void AddRemoveMultipleElementsTest()
        {
            // Navigate to the Add/Remove Elements page
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            // Locate the "Add Element" button
            var addBtn = driver.FindElement(By.XPath("//button[text()='Add Element']"));

            int elementsToAdd = 5; // Number of elements to add

            // First Loop: Add elements
            // Loop to click "Add Element" button multiple times
            for (int i = 0; i < elementsToAdd; i++)
            {
                addBtn.Click(); // Click the "Add Element" button
            }

            // Verify that the correct number of "Delete" buttons have appeared
            var deleteBtn = driver.FindElements(By.XPath("//button[text()='Delete']"));
            Assert.That(deleteBtn.Count, Is.EqualTo(elementsToAdd), $"Expected {elementsToAdd} 'Delete' buttons, but found {deleteBtn.Count}");

            // Second Loop: Remove elements
            // starts from the last "Delete" button
            for (int i = elementsToAdd - 1; i >= 0; i--)
            {
                deleteBtn[i].Click(); // Click the first delete button each time
                deleteBtn = driver.FindElements(By.XPath("//button[text()='Delete']")); // Re-fetch elements
                Assert.That(deleteBtn.Count, Is.EqualTo(i), $"Expected {i} 'Delete' buttons remaining, but found {deleteBtn.Count}.");
            }

            // Verify no "Delete" buttons remain
            Assert.That(driver.FindElements(By.XPath("//button[text()='Delete']")).Count, Is.EqualTo(0), "Expected no 'Delete' buttons remaining.");
        }

        [TearDown]
        public void CleanUp()
        {
            driver!.Quit();
        }
    }
}
