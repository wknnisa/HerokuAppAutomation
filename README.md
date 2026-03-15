# HerokuAppAutomation

UI automation testing project built using **C#, Selenium WebDriver, and NUnit** to practice browser automation and Page Object Model design.

The tests target the public demo website:

https://the-internet.herokuapp.com/

This project was created to explore automated UI testing concepts such as element interaction, page navigation, file upload automation, and reusable test framework structure.

---

## Technologies Used

- C#
- Selenium WebDriver
- NUnit
- .NET
- Chrome / Edge / Firefox WebDrivers

---

## Framework Design

The project follows a **Page Object Model (POM)** structure to separate page interactions from test logic.

This helps keep the automation code organized, reusable, and maintainable.

---

## Project Structure

HerokuAppAutomation
│
├── Base
│ └── BaseTest.cs
│
├── Pages
│ ├── LoginPage.cs
│ ├── AddRemoveElementsPage.cs
│ └── FileUploadPage.cs
│
├── Tests
│ ├── AddRemoveElements
│ │ └── VerifyAddRemoveElementsTest.cs
│ ├── E2E
│ │ ├── VerifyLoginAddRemoveE2ETest.cs
│ │ └── VerifyLoginFileUploadE2ETest.cs
│ ├── FileUpload
│ │ └── VerifyFileUploadTest.cs
│ ├── HomePage
│ │ └── VerifyHomePageTitleTest.cs
│ ├── Login
│ │ └── VerifyLoginTests.cs
│ └── BasicFunctionalityTests.cs
│
├── Utilities
│ ├── FileHelper.cs
│ ├── Logger.cs
│ └── ScreenshotHelper.cs


- **Pages** – Page Object classes that encapsulate locators and interactions for each web page.
- **Tests** – NUnit test classes that verify application behaviour.
- **Utilities** – Shared helper classes such as logging, screenshots, and file helpers.
- **Base** – Base test setup including browser initialization and cleanup.

---

## Example Test Scenarios

### Login Testing
- Navigate to login page
- Submit credentials
- Validate login success or error messages

### Add / Remove Elements
- Click **Add Element** button
- Verify new elements appear
- Remove elements dynamically

### File Upload
- Upload a file using the file input element
- Submit upload
- Verify uploaded file confirmation

### End-to-End Flow
- Execute multiple page interactions as a single scenario.

---

## Purpose

This repository was created as a **learning project for UI automation testing**.

It demonstrates:

- Selenium WebDriver usage
- Page Object Model implementation
- WebDriver waits
- Cross-browser testing setup
- Logging and screenshot utilities for debugging

---

## Test Environment

The automation tests run against the public demo testing website:

https://the-internet.herokuapp.com/

This site is commonly used for practicing Selenium automation scenarios.