using Swan.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertWin32Core
{
    public class LoggingUtils
    {
        private static string LoggingFile { set; get; }

        public static void InitLogger()
        {
            if (LoggingFile != null)
            {
                return;
            }

            LoggingFile = Path.GetTempFileName() + ".convert.log";
            Logger.RegisterLogger(new FileLogger(LoggingFile, false));
            $"Start logging to {LoggingFile}".Info();
        }

        public static List<string> ReadLastNLogs(int n)
        {
            var logs = new List<string>();
            if (File.Exists(LoggingFile))
            {
                using (var reader = new StreamReader(LoggingFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        logs.Add(line);
                        while (logs.Count > n)
                        {
                            logs.RemoveAt(0);
                        }
                    }
                }
            }
            return logs;
        }
    }
}
