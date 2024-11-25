using NUnit.Framework;
using OpenQA.Selenium;

namespace HerokuAppAutomation.Utilities
{
    public static class ScreenshotHelper
    {
        /// <summary>
        /// Captures a screenshot of the current browser state.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="fileName">The file name for the screenshot (e.g., "screenshot.png").</param>
        public static void TakeScreenshot(IWebDriver driver, string fileName) 
        {
            try
            {
                if (driver is ITakesScreenshot screenshotDriver)
                {
                    Screenshot screenshot = screenshotDriver.GetScreenshot();

                    // Ensure the directory exists
                    string screenshotsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");

                    if (!Directory.Exists(screenshotsDir))
                    {
                        Directory.CreateDirectory(screenshotsDir);
                    }

                    // Save the screenshot (file format inferred from fileName extension)
                    string filePath = Path.Combine(screenshotsDir, fileName);
                    screenshot.SaveAsFile(filePath); // No ScreenshotImageFormat needed in Selenium 4

                    TestContext.WriteLine($"Screenshot saved to: {filePath}");
                }
                else
                {
                    TestContext.WriteLine("Driver does not support taking screenshots.");
                }
            }
            catch (Exception ex) 
            {
                TestContext.WriteLine($"Failed to capture screenshot: {ex.Message}");
            }
        }
    }
}
