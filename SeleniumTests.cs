using NUnit.Framework; // Make sure NUnit is installed
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HerokuAppAutomation
{
    public class SeleniumTests
    {
        private IWebDriver? driver;
        //private WebDriverWait wait;

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

        [Test]
        public void SuccessfulLogin() 
        {
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com/login");
            
            // find by id
            var usernameTxtField = driver.FindElement(By.Id("username"));
            // enter username
            usernameTxtField.SendKeys("tomsmith");

            var passwordTxtField = driver.FindElement(By.Id("password"));
            // enter password
            passwordTxtField.SendKeys("SuperSecretPassword!");

            // find by xpath
            var loginBtn = driver.FindElement(By.XPath("//button[@type='submit']"));
            // click login button
            loginBtn.Click();

            // Check for successful login by verifying the URL or welcome message
            Assert.That(driver.Url, Is.EqualTo("https://the-internet.herokuapp.com/secure"));
        }

        [Test]
        public void UnsuccessfulLogin()
        {
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com/login");

            // Direct Interaction than using variable
            driver!.FindElement(By.Id("username")).SendKeys("test");
            driver!.FindElement(By.Id("password")).SendKeys("test");

            driver!.FindElement(By.XPath("//button[@type='submit']")).Click();

            // Find the element and then get its text using class and id
            string errorMsg = driver.FindElement(By.CssSelector("div#flash.flash.error")).Text; 
            // (By.CssSelector("#flash") - faster execution but there could be non-unique id

            // Check that an error message is displayed
            Assert.That(errorMsg, Does.Contain("Your username is invalid!"));
        }

        [Test]
        public void FileUpload() 
        {
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com/upload");

            //var chooseFileBtn = driver.FindElement(By.Id("file-upload"));
            var chooseFileBtn = driver.FindElement(By.CssSelector("input[type='file'][id='file-upload'][name='file']"));
            //chooseFileBtn.Click();

            string fileUpload = @"C:\Users\HP\Downloads\sample1.jpg";
            chooseFileBtn.SendKeys(fileUpload);

            var uploadBtn = driver.FindElement(By.CssSelector("input#file-upload"));
            //uploadBtn.Click();

            string successMsg = driver.FindElement(By.CssSelector("h3")).Text;
            Assert.That(successMsg, Is.EqualTo("File Uploaded!"));
        }

        [TearDown] // Runs after each test
        public void Cleanup()
        {
            driver!.Quit(); // Closes the browser
        }
    }
}
