using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerokuAppAutomation
{
    public class AddRemoveTests
    {
        private IWebDriver? driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver(@"C\WebDrivers\");
        }

        [Test]
        public void AddRemoveMultipleElementsTest()
        {
            // Navigate to the Add/Remove Elements page
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            // Locate the "Add Element" button
            var addBtn = driver.FindElement(By.XPath("\\button[text='Add Element']"));

            int elementsToAdd = 5; // Number of elements to add

            // Loop to click "Add Element" button multiple times
            for (int i = 0; i < elementsToAdd; i++)
            {
                addBtn.Click(); // Click the "Add Element" button
            }

            // Verify that the correct number of "Delete" buttons have appeared
            var deleteBtn = driver.FindElements(By.XPath("button[text='Delete']"));
            Assert.That(deleteBtn.Count, Is.EqualTo(elementsToAdd), $"Expected {elementsToAdd} 'Delete' buttons, but found {deleteBtn.Count}");
        }

        [TearDown]
        public void CleanUp()
        {
            driver!.Quit();
        }
    }
}
