using NUnit.Framework; // Make sure NUnit is installed
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HerokuAppAutomation
{
    public class SeleniumTests
    {
        private IWebDriver? driver;

        [SetUp] // Runs before each test
        public void Setup()
        {
            driver = new ChromeDriver(@"C:\WebDrivers\"); // Update with your path
        }

        [Test]
        public void VerifyHomePageTitle()
        {
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com");
            // Use Assert.That with Is.EqualTo for assertions in NUnit 4.x
            Assert.That(driver.Title, Is.EqualTo("The Internet"));
        }

        [Test]
        public void AddRemoveElementsTest()
        {
            // Navigate to the Add/Remove Elements page
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com/add_remove_elements/");

            // Click the "Add Element" button
            var addButton = driver.FindElement(By.XPath("//button[text()='Add Element']")); // Locate the button using XPath
            addButton.Click(); // Click the button

            // Verify that a new "Delete" button is displayed
            var deleteButton = driver.FindElement(By.XPath("//button[text()='Delete']"));
            Assert.That(deleteButton.Displayed, Is.True); // Check if the button is displayed

            // Click the "Delete" button
            deleteButton.Click();

            // Verify that the "Delete" button is no longer displayed
            Assert.That(driver.FindElements(By.XPath("//button[text()='Delete']")).Count, Is.EqualTo(0)); // Check count
        }


        [TearDown] // Runs after each test
        public void Cleanup()
        {
            driver!.Quit(); // Closes the browser
        }
    }
}
