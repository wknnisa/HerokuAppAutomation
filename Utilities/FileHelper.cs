namespace HerokuAppAutomation.Utilities
{
    public static class FileHelper
    {
        private static readonly string GlobalFileDirectory = @"C:\TestFiles\";

        /// <summary>
        /// Generates a test file of specified size in MB.
        /// </summary>
        public static void GenerateTestFile(string fileName, int sizeInMB)
        {
            if (!Directory.Exists(GlobalFileDirectory))
            {
                Directory.CreateDirectory(GlobalFileDirectory);
            }

            string filePath = Path.Combine(GlobalFileDirectory, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.SetLength(sizeInMB * 1024 * 1024); // Set file size
            }
        }

        /// <summary>
        /// Cleans up all files in the global test directory.
        /// </summary>
        public static void CleanUpGlobalTestFiles()
        {
            if (Directory.Exists(GlobalFileDirectory))
            {
                Directory.Delete(GlobalFileDirectory, true); // Delete directory and all files
            }
        }

        /// <summary>
        /// Checks if a file exists in the global test directory.
        /// </summary>
        public static bool IsFileAvailable(string fileName)
        {
            string filePath = Path.Combine(GlobalFileDirectory, fileName);
            return File.Exists(filePath);
        }
    }
}
