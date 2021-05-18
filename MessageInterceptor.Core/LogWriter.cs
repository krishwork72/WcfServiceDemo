using Newtonsoft.Json;
using System;
using System.IO;

namespace MessageInterceptor.Core
{
    public class LogWriter
    {
        public static void Log<T>( T value)
        {
            string logFilePath = @"C:\Logs\Log-" + DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            FileInfo logFileInfo = new FileInfo(logFilePath);
            DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
            {
                using (StreamWriter log = new StreamWriter(fileStream))
                {
                    log.WriteLine("-----------" + DateTime.Now.ToString() + "-----------");
                    log.WriteLine(JsonConvert.SerializeObject(value));
                }
            }
        }
    }
}
