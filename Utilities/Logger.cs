using NUnit.Framework;

namespace HerokuAppAutomation.Utilities
{
    public static class Logger
    {
        // Log issue function (add logging mechanism here, e.g., file logging, test report logging, etc.)
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}][{level.ToString().ToUpper()}] {message}";
            TestContext.WriteLine(logMessage);
        }

        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }
    }
}
