using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace tangerineExtension.LogInterface
{
    internal class FileWriter
    {
        private bool exit = false;
        private readonly string filePath;
        private static readonly string LogFileFolder = @"Logs\";
        private readonly AutoResetEvent writeLogEvent = new AutoResetEvent(false);
        private readonly ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
        private StorageFile logFile;

        public FileWriter(string fileBaseName)
        {
            filePath = CreateFileName(fileBaseName);
            filePath = Path.Combine(LogFileFolder, filePath);
            CleanUpLogs(fileBaseName);
            Task.Run(LoggingThread);
        }

        ~FileWriter()
        {
            exit = true;
            writeLogEvent.Set();
        }

        public void WriteMessage(string logMessage)
        {
            logQueue.Enqueue(logMessage);
            writeLogEvent.Set();
        }

        private string CreateFileName(string fileBaseName)
        {
            if (string.IsNullOrWhiteSpace(fileBaseName))
            {
                fileBaseName = "log";
            }
            string currentTime = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
            return $"{fileBaseName}_{currentTime}.log";
        }

        private void CleanUpLogs(string fileBase)
        {
            string logFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, LogFileFolder);
            if (Directory.Exists(logFolder))
            {
                List<FileInfo> fileList = new DirectoryInfo(logFolder).GetFiles($"{fileBase}_????????-??????-???.log").ToList();
                if (fileList.Count > 30)
                {
                    fileList.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase));
                    foreach (FileInfo file in fileList)
                    {
                        try
                        {
                            File.Delete(file.FullName);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                        }
                    }
                }
            }
        }

        private async Task LoggingThread()
        {
            while (writeLogEvent.WaitOne())
            {
                try
                {
                    StringBuilder sbLongLog = new StringBuilder();
                    while (logQueue.TryDequeue(out string log))
                    {
                        sbLongLog.Append(log);
                        sbLongLog.Append("\r\n");
                    }
                    if (logFile == null)
                    {
                        await GenerateLogFileAsync();
                    }
                    File.AppendAllText(logFile.Path, sbLongLog.ToString());
                }
                catch (Exception e)
                {
                    Debug.Fail(e.ToString());
                }
                if (exit)
                {
                    return;
                }
            }
        }

        private async Task<StorageFile> GenerateLogFileAsync()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                logFile = await storageFolder.CreateFileAsync(filePath, 
                    CreationCollisionOption.GenerateUniqueName).AsTask().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get file at location: " + filePath + Environment.NewLine + ex.Message);
            }

            return logFile;
        }
    }
}
