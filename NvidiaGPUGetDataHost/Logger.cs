using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.IO;

namespace NvidiaGPUGetDataHost
{
    public class Logger
    {
        public static bool IsInit;
        public static readonly ILog Log = LogManager.GetLogger(typeof(Logger));

        public const string LogPath = @"..\\logs\";

        public static void ConfigureWithFile()
        {

            try
            {
                if (!Directory.Exists(@"logs"))
                {
                    Directory.CreateDirectory(@"logs");
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "NvidiaGPUGetDataHost";
                    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Information, 101, 1);
                }
            }

            IsInit = true;
            try
            {
                var h = (Hierarchy)LogManager.GetRepository();
                h.Root.Level = Level.Info;
                //    h.Root.Level = Level.Warn;
                //    h.Root.Level = Level.Error;

                h.Root.AddAppender(CreateFileAppender());
                h.Configured = true;
            }
            catch (Exception ex)
            {
                IsInit = false;
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "NvidiaGPUGetDataHost";
                    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Information, 101, 1);
                }
            }
        }

        public static IAppender CreateFileAppender()
        {
            var appender = new RollingFileAppender
            {
                Name = "RollingFileAppender",
                File = LogPath + "log_NvidiaGPUGetDataHost.txt",
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                MaxSizeRollBackups = 1,
                MaxFileSize = 16384 * 1024,
                PreserveLogFileNameExtension = true,
                Encoding = System.Text.Encoding.Unicode
            };

            var layout = new PatternLayout
            {
                ConversionPattern = "[%date{yyyy-MM-dd HH:mm:ss}] [%level] %message%newline"
            };
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }
        public static void ConsolePrint(string grp, string text)
        {
            // try will prevent an error if something tries to print an invalid character
            try
            {
                // Console.WriteLine does nothing on x64 while debugging with VS, so use Debug. Console.WriteLine works when run from .exe
#if DEBUG
                Debug.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + grp + "] " + text);
#endif
#if !DEBUG
            Console.WriteLine("[" +DateTime.Now.ToLongTimeString() + "] [" + grp + "] " + text);
#endif
                Logger.Log.Info("[" + grp + "] " + text);
            }
            catch { }  // Not gonna recursively call here in case something is seriously wrong
        }
    }
}
