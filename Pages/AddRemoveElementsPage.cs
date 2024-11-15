using OpenQA.Selenium;

namespace HerokuAppAutomation.Pages
{
    public class AddRemoveElementsPage
    {
        private readonly IWebDriver? driver;
        private const string AddRemoveElementsUrl = "https://the-internet.herokuapp.com/add_remove_elements/";

        // Constructor
        public AddRemoveElementsPage(IWebDriver driver)
        {
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

        // Method to add an element
        public void AddElement()
        {
            driver!.FindElement(addButton).Click();
        }

        // Method to get the count of "Delete" buttons
        public int GetDeleteButtonCount()
        {
            return driver!.FindElements(deleteButtons).Count();
        }

        // Method to delete an element
        public void DeleteElement(int index) 
        { 
            var deleteButtonsList = driver!.FindElements(deleteButtons);
            deleteButtonsList[index].Click();

        }
    }
}
