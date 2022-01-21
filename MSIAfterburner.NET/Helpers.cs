using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Principal;

namespace MSI.Afterburner
{
    internal class Helpers
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

        public static void WriteAllBytesThrough(string FileName, byte[] buffer)
        {
            try
            {
                string tempFileName = Path.GetTempFileName();
                if (File.Exists(tempFileName)) File.Delete(tempFileName);

                // write the data to a temp file
                using (var tempFile = File.Create(tempFileName, 4096, FileOptions.WriteThrough))
                    tempFile.Write(buffer, 0, buffer.Length);
                //copy file
                try
                {
                    if (File.Exists(FileName)) File.Delete(FileName);
                    File.Copy(tempFileName, FileName);
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("WriteAllBytesThrough", ex.ToString());
                }

                // replace the contents
                File.Replace(tempFileName, FileName, tempFileName + ".tmp");
                if (File.Exists(tempFileName + ".tmp")) File.Delete(tempFileName + ".tmp");
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllBytesThrough", ex.ToString());
            }
        }

        public static void ConsolePrint(string grp, string text)
        {
            Logger.ConfigureWithFile();
            // try will prevent an error if something tries to print an invalid character
            try
            {
                Debug.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + grp + "] " + text);
            Console.WriteLine("[" +DateTime.Now.ToLongTimeString() + "] [" + grp + "] " + text);

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

        // Checking the version using >= will enable forward compatibility,
        // however you should always compile your code on newer versions of
        // the framework to ensure your app works the same.
        private static bool Is45DotVersion(int releaseKey)
        {
            if (releaseKey >= 393295)
            {
                //return "4.6 or later";
                return true;
            }
            if ((releaseKey >= 379893))
            {
                //return "4.5.2 or later";
                return true;
            }
            if ((releaseKey >= 378675))
            {
                //return "4.5.1 or later";
                return true;
            }
            if ((releaseKey >= 378389))
            {
                //return "4.5 or later";
                return true;
            }
            // This line should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            //return "No 4.5 or later version detected";
            return false;
        }

        public static bool Is45NetOrHigher()
        {
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                return ndpKey?.GetValue("Release") != null && Is45DotVersion((int)ndpKey.GetValue("Release"));
            }
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

        
    }
}
