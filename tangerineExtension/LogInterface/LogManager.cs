using System;

namespace tangerineExtension.LogInterface
{
    internal class LogManager
    {
        internal enum Level
        {
            Info = 0,
            Warn = 1,
            Error = 2,
            None = 3,
        }
        private static bool initialized = false;
        private static FileWriter fileWriter;

        public static void Setup(string fileName)
        {
            if (!initialized)
            {
                fileWriter = new FileWriter(fileName);
                initialized = true;
            }
        }

        public static void Log(Level level, string message)
        {
            if (initialized)
            {
                string logMassage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: {level} - {message}";
                fileWriter.WriteMessage(logMassage);
            }
        }
    }
}
