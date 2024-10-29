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

        [TearDown]
        public void CleanUp() 
        { 
            driver!.Quit();
        }
    }
}
