﻿using OpenQA.Selenium;

namespace HerokuAppAutomation.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver driver;
        private const string LoginUrl = "https://the-internet.herokuapp.com/login";

        // Constructor
        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // Locators for login fields
        private By usernameField = By.Id("username");
        private By passwordField = By.Id("password");
        private By loginButton = By.XPath("//button[@type='submit']");

        // Method to navigate to the login page
        public void NavigateToLogin()
        {
            driver.Navigate().GoToUrl(LoginUrl);
        }

        // Method to perform login
        public void Login(string username, string password)
        {
            driver.FindElement(usernameField).Clear();
            driver.FindElement(usernameField).SendKeys(username);
            driver.FindElement(passwordField).Clear();
            driver.FindElement(passwordField).SendKeys(password);
            driver.FindElement(loginButton).Click();
        }
    }
}