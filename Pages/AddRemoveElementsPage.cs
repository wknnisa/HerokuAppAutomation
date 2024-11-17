using OpenQA.Selenium;

namespace HerokuAppAutomation.Pages
{
    public class AddRemoveElementsPage
    {
        private readonly IWebDriver driver;
        private const string AddRemoveElementsUrl = "https://the-internet.herokuapp.com/add_remove_elements/";

        // Constructor
        public AddRemoveElementsPage(IWebDriver driver)
        {
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver), "Driver cannot be null.");
            }    
            this.driver = driver;
        }

        // Locators for Add/Remove Elements
        private By addButton = By.XPath("//button[text()='Add Element']");
        private By deleteButtons = By.XPath("//button[text()='Delete']");

        // Method to navigate to Add/Remove Elements page
        public void NavigateToAddRemoveElements()
        {
            driver!.Navigate().GoToUrl(AddRemoveElementsUrl);
        }

        // Method to click the "Add Element" button
        public void AddElement()
        {
            driver!.FindElement(addButton).Click();
        }

        // Method to count the number of "Delete" buttons
        public int GetDeleteButtonCount()
        {
            return driver!.FindElements(deleteButtons).Count();
        }

        // Method to delete the first available button
        public void RemoveElement() 
        { 
            var buttons = driver!.FindElements(deleteButtons);
            if (buttons.Count() > 0) 
            {
                buttons[0].Click(); // Remove the first button
            }
        }
    }
}
