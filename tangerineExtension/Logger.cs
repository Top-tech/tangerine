using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tangerineExtension.LogInterface;

namespace tangerineExtension
{
    public sealed class Logger
    {
        public static void Setup(string fileName)
        {
            LogManager.Setup(fileName);
        }

        public static void Info(string message)
        {
            LogManager.Log(LogManager.Level.Info, message);
        }
        public static void Warn(string message)
        {
            LogManager.Log(LogManager.Level.Warn, message);
        }
        public static void Error(string message)
        {
            LogManager.Log(LogManager.Level.Error, message);
        }
    }
}
