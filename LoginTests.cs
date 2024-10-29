using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HerokuAppAutomation
{
    public class LoginTests
    {
        private IWebDriver? driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver(@"C:\WebDrivers\");
        }

        [Test]
        public void SuccessfulLogin()
        {
            driver!.Navigate().GoToUrl("https://the-internet.herokuapp.com/login");

            // Case Sensitivity Test
            string username = "tomsmith";
            string password = "SuperSecretPassword!";

            // Valid Login
            PerformLogin(username, password);
        }

        private void PerformLogin(string username, string password) 
        {
            driver!.FindElement(By.Id("username")).Clear();
            driver!.FindElement(By.Id("username")).SendKeys(username);
            driver!.FindElement(By.Id("password")).Clear();
            driver!.FindElement(By.Id("password")).SendKeys(password);
            driver!.FindElement(By.XPath("//button[@type='submit']")).Click();
        }

        [TearDown]
        public void CleanUp() 
        { 
            driver!.Quit();
        }
    }
}
