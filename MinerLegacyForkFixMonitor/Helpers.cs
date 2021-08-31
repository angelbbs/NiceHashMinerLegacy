using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;

namespace MinerLegacyForkFixMonitor
{
    internal class Helpers
    //internal class Helpers : PInvokeHelpers
    {
        public static readonly bool IsElevated;

        static Helpers()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                IsElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
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

        public static void ConsolePrint(string grp, string text, params object[] arg)
        {
            ConsolePrint(grp, string.Format(text, arg));
        }

        public static void ConsolePrint(string grp, string text, object arg0)
        {
            ConsolePrint(grp, string.Format(text, arg0));
        }

        public static void ConsolePrint(string grp, string text, object arg0, object arg1)
        {
            ConsolePrint(grp, string.Format(text, arg0, arg1));
        }

        public static void ConsolePrint(string grp, string text, object arg0, object arg1, object arg2)
        {
            ConsolePrint(grp, string.Format(text, arg0, arg1, arg2));
        }

        // parsing helpers
        public static int ParseInt(string text)
        {
            return int.TryParse(text, out var tmpVal) ? tmpVal : 0;
        }

        public static long ParseLong(string text)
        {
            return long.TryParse(text, out var tmpVal) ? tmpVal : 0;
        }

        public static double ParseDouble(string text)
        {
            try
            {
                var parseText = text.Replace(',', '.');
                return double.Parse(parseText, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }


        public static void SetDefaultEnvironmentVariables()
        {
            ConsolePrint("NICEHASH", "Setting environment variables");

            var envNameValues = new Dictionary<string, string>()
            {
                {"GPU_MAX_ALLOC_PERCENT", "100"},
                {"GPU_USE_SYNC_OBJECTS", "1"},
                {"GPU_SINGLE_ALLOC_PERCENT", "100"},
                {"GPU_MAX_HEAP_SIZE", "100"},
                {"GPU_FORCE_64BIT_PTR", "1"}
            };

            foreach (var kvp in envNameValues)
            {
                var envName = kvp.Key;
                var envValue = kvp.Value;
                // Check if all the variables is set
                if (Environment.GetEnvironmentVariable(envName) == null)
                {
                    try { Environment.SetEnvironmentVariable(envName, envValue); }
                    catch (Exception e) { ConsolePrint("NICEHASH", e.ToString()); }
                }

                // Check to make sure all the values are set correctly
                if (!Environment.GetEnvironmentVariable(envName)?.Equals(envValue) ?? false)
                {
                    try { Environment.SetEnvironmentVariable(envName, envValue); }
                    catch (Exception e) { ConsolePrint("NICEHASH", e.ToString()); }
                }
            }
        }

        public static void SetNvidiaP0State()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "nvidiasetp0state.exe",
                    Verb = "runas",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var p = Process.Start(psi);
                p?.WaitForExit();
                if (p?.ExitCode != 0)
                    ConsolePrint("NICEHASH", "nvidiasetp0state returned error code: " + p.ExitCode);
                else
                    ConsolePrint("NICEHASH", "nvidiasetp0state all OK");
            }
            catch (Exception ex)
            {
                ConsolePrint("NICEHASH", "nvidiasetp0state error: " + ex.Message);
            }
        }


    }
}
