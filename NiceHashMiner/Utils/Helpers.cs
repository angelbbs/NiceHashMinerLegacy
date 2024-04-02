using Microsoft.Win32;
using NiceHashMiner.Configs;
using NiceHashMiner.PInvoke;
using NiceHashMiner.Utils;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace NiceHashMiner
{
    internal class Helpers : PInvokeHelpers
    {
        private static readonly bool Is64BitProcess = (IntPtr.Size == 8);
        public static bool Is64BitOperatingSystem = Is64BitProcess || InternalCheckIsWow64();
        public static readonly bool IsElevated;

        static Helpers()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                IsElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static IPAddress GetLocalIP()
        {
            return IPAddress.Parse(Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString()) ?? IPAddress.None;
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
                    File.Move(tempFileName, FileName);
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("WriteAllBytesThrough", ex.ToString());
                }

                // replace the contents
                //File.Replace(tempFileName, FileName, tempFileName + ".tmp");
                if (File.Exists(tempFileName + ".tmp")) File.Delete(tempFileName + ".tmp");
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllBytesThrough", ex.ToString());
            }
        }

        public static bool AntivirusInstalled()
        {
            string wmipathstr = @"\" + Environment.MachineName + @"\root\SecurityCenter";
            bool ret = false;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\SecurityCenter2", "SELECT * FROM AntivirusProduct");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    uint productState = (uint)queryObj["productState"];
                    uint realtimeStatus = (productState & 0xff00) >> 8;
                    string displayName = (string)queryObj["displayName"];
                    //Helpers.ConsolePrint("Antivirus installed: ", displayName);
                    switch (realtimeStatus)
                    {
                        case 0x00:
                            Helpers.ConsolePrint(displayName, "off"); ret = false; break;
                        case 0x01:
                            Helpers.ConsolePrint(displayName, "expired"); break;
                        case 0x10:
                            Helpers.ConsolePrint(displayName, "on"); ret = true; break;
                        case 0x11:
                            Helpers.ConsolePrint(displayName, "snoozed"); ret = false; break;
                        default:
                            Helpers.ConsolePrint(displayName, "unknown"); break;
                    }
                }
                return ret;
            }

            catch (Exception e)
            {
                Helpers.ConsolePrint("Check antivirus error: ", e.Message);
            }
            Helpers.ConsolePrint("Antivirus not detected", "");
            return false;
        }
        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (var p = Process.GetCurrentProcess())
                {
                    return IsWow64Process(p.Handle, out var retVal) && retVal;
                }
            }
            return false;
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

                if (ConfigManager.GeneralConfig.LogToFile && Logger.IsInit)
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

        public static uint GetIdleTime()
        {
            var lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }

        public static void DisableWindowsErrorReporting(bool en)
        {
            //bool failed = false;

            ConsolePrint("NICEHASH", "Trying to enable/disable Windows error reporting");

            // CurrentUser
            try
            {
                using (var rk = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\Windows Error Reporting"))
                {
                    if (rk != null)
                    {
                        var o = rk.GetValue("DontShowUI");
                        if (o != null)
                        {
                            var val = (int)o;
                            ConsolePrint("NICEHASH", "Current DontShowUI value: " + val);

                            if (val == 0 && en)
                            {
                                ConsolePrint("NICEHASH", "Setting register value to 1.");
                                rk.SetValue("DontShowUI", 1);
                            }
                            else if (val == 1 && !en)
                            {
                                ConsolePrint("NICEHASH", "Setting register value to 0.");
                                rk.SetValue("DontShowUI", 0);
                            }
                        }
                        else
                        {
                            ConsolePrint("NICEHASH", "Registry key not found .. creating one..");
                            rk.CreateSubKey("DontShowUI", RegistryKeyPermissionCheck.Default);
                            ConsolePrint("NICEHASH", "Setting register value to 1..");
                            rk.SetValue("DontShowUI", en ? 1 : 0);
                        }
                    }
                    else
                        ConsolePrint("NICEHASH", "Unable to open SubKey.");
                }
            }
            catch (Exception ex)
            {
                ConsolePrint("NICEHASH", "Unable to access registry. Error: " + ex.Message);
            }
        }

        public static string FormatSpeedOutput(double speed, string separator = " ", string format = "F3")
        {
            string ret;
            if (speed < 1000)
                ret = (speed).ToString(format, CultureInfo.InvariantCulture) + separator;
            else if (speed < 100000)
                ret = (speed * 0.001).ToString(format, CultureInfo.InvariantCulture) + separator + "k";
            else if (speed < 100000000)
                ret = (speed * 0.000001).ToString(format, CultureInfo.InvariantCulture) + separator + "M";
            else
                ret = (speed * 0.000000001).ToString(format, CultureInfo.InvariantCulture) + separator + "G";
            return ret;
        }

        public static string GetAlgorithmSpeedUnit(AlgorithmType algo)
        {
            string unit = "";
            switch (algo)
            {
                case AlgorithmType.ZHash:
                case AlgorithmType.ZelHash:
                case AlgorithmType.BeamV3:
                    unit = "Sol/s ";
                    break;
                case AlgorithmType.CuckooCycle:
                case AlgorithmType.GrinCuckatoo32:
                    unit = "G/s ";
                    break;
                default:
                    unit = "H/s ";
                    break;
            }
            return unit;
        }

        public static string FormatDualSpeedOutput(double primarySpeed, double secondarySpeed = 0, double thirdSpeed = 0,
            AlgorithmType algo = AlgorithmType.NONE, AlgorithmType algo2 = AlgorithmType.NONE, AlgorithmType algo3 = AlgorithmType.NONE)
        {
            //Helpers.ConsolePrint("FormatDualSpeedOutput", "algo: " + algo.ToString() + " primarySpeed: " + primarySpeed.ToString());
            //Helpers.ConsolePrint("FormatDualSpeedOutput", "algo2: " + algo2.ToString() + " secondarySpeed: " + secondarySpeed.ToString());
            //Helpers.ConsolePrint("FormatDualSpeedOutput", "algo3: " + algo3.ToString() + " thirdSpeed: " + thirdSpeed.ToString());
            string ret = "";
            string first = "";
            string second = "";
            string third = "";
            string format = "F3";
            if (algo2 == AlgorithmType.Empty) algo2 = AlgorithmType.NONE;
            if (algo3 == AlgorithmType.Empty) algo3 = AlgorithmType.NONE;
            if (algo == AlgorithmType.BeamV3) format = "F1";
            if (algo == AlgorithmType.ZelHash) format = "F1";
            if (algo == AlgorithmType.ZHash) format = "F1";

            if (algo != AlgorithmType.NONE && algo2 == AlgorithmType.NONE && algo3 == AlgorithmType.NONE)
            {
                ret = FormatSpeedOutput(primarySpeed, " ", format) + GetAlgorithmSpeedUnit(algo); 
            } else
            {
                first = FormatSpeedOutput(primarySpeed, " ", format);
                second = FormatSpeedOutput(secondarySpeed, " ", format);
                third = FormatSpeedOutput(thirdSpeed, " ", format);
                
                if (algo == AlgorithmType.NONE && algo2 != AlgorithmType.NONE)
                {
                    ret = second + GetAlgorithmSpeedUnit(algo2);
                } 
                
                if (algo != AlgorithmType.NONE && algo2 != AlgorithmType.NONE)
                {
                    ret = first + GetAlgorithmSpeedUnit(algo) + "/ " + second + GetAlgorithmSpeedUnit(algo2);
                }
                if (algo == AlgorithmType.NONE && algo2 == AlgorithmType.NONE && algo3 != AlgorithmType.NONE)
                {
                    ret = third + GetAlgorithmSpeedUnit(algo3);
                }

                double allhash = primarySpeed + secondarySpeed + thirdSpeed;
                if (allhash == 0)
                {
                    //ret = "--";
                    //return ret;
                }
            }
            //Helpers.ConsolePrint("FormatDualSpeedOutput", ret);
            return ret;
        }

        
        public static string GetMotherboardID()
        {
            var mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            var moc = mos.Get();
            var serial = "";
            foreach (ManagementObject mo in moc)
            {
                serial = (string)mo["SerialNumber"];
            }

            return serial;
        }

        // TODO could have multiple cpus
        public static string GetCpuID()
        {
            var id = "N/A";
            try
            {
                var mbs = new ManagementObjectSearcher("Select * From Win32_processor");
                var mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                {
                    id = mo["ProcessorID"].ToString();
                }
            }
            catch { }
            return id;
        }

        public static bool WebRequestTestGoogle()
        {
            const string url = "http://www.google.com";
            try
            {
                var myRequest = System.Net.WebRequest.Create(url);
                myRequest.Timeout = Globals.FirstNetworkCheckTimeoutTimeMs;
                myRequest.GetResponse();
            }
            catch (System.Net.WebException)
            {
                return false;
            }
            return true;
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

        public static bool IsConnectedToInternet()
        {
            bool returnValue;
            try
            {
                returnValue = InternetGetConnectedState(out _, 0);
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
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
            catch (FormatException ex)
            {
                return 0;
            }
        }

        // IsWMI enabled
        public static bool IsWmiEnabled()
        {
            try
            {
                new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem").Get();
                ConsolePrint("NICEHASH", "WMI service seems to be running, ManagementObjectSearcher returned success.");
                return true;
            }
            catch
            {
                ConsolePrint("NICEHASH", "ManagementObjectSearcher not working need WMI service to be running");
            }
            return false;
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
                {"GPU_FORCE_64BIT_PTR", "1"},
                {"CUDA_DEVICE_ORDER", "PCI_BUS_ID"}
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

        
        
        public static void WriteAllTextWithBackup(string FilePath, string contents)
        {
            string path = FilePath;
            var tempPath = FilePath + ".tmp";

            // create the backup name
            var backup = FilePath + ".backup";

            // delete any existing backups
            try
            {
                if (File.Exists(backup))
                    File.Delete(backup);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }

            // get the bytes
            var data = Encoding.ASCII.GetBytes(contents);

            //copy file
            try
            {
                // write the data to a temp file
                File.WriteAllText(tempPath, contents);

                LockManager.GetLock(path, () =>
                {
                    if (File.Exists(path)) File.Delete(path);
                    File.Move(tempPath, path);
                });
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
                return;
            }
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }
        }
    }
}
