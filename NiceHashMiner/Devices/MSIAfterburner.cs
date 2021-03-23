using MSI.Afterburner;
using MSI.Afterburner.Exceptions;
using NiceHashMiner.Configs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Devices
{
    public static class MSIAfterburner
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowTitle);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        public static ControlMemory macm;
        public static HardwareMonitor mahm;

        public static bool MSIAfterburnerCheckPath()
        {
            string msiabpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\MSI Afterburner\\MSIAfterburner.exe";
            if (File.Exists(msiabpath)) return true;
            return false;
        }

        public static bool MSIAfterburnerRUN(bool forceRun = false)
        {
            string msiabpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\MSI Afterburner\\MSIAfterburner.exe";
            if (ConfigManager.GeneralConfig.ABEnableOverclock || forceRun)
            {
                if (ConfigManager.GeneralConfig.AB_ForceRun && !Process.GetProcessesByName("MSIAfterburner").Any())
                {
                    if (!MSIAfterburnerCheckPath())
                    {
                        new Task(() =>
                            MessageBox.Show(string.Format(International.GetText("FormSettings_AB_FileNotFound"),
                    msiabpath), "MSI Afterburner error!",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();
                        return false;
                    }

                    
                    Process P = new Process();
                    try
                    {
                        P.StartInfo.FileName = msiabpath;
                        P.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        P.StartInfo.UseShellExecute = false;
                        P.Start();
                        /*
                        do
                        {
                            Thread.Sleep(100);
                            if (P.Responding)
                            {
                                break;
                            }
                        } while (true);
                        */
                        int repeats = 0;
                        do
                        {
                            Helpers.ConsolePrint("MSI AB", "Check MSI Afterburner instance. Try " + repeats.ToString());
                            if (Process.GetProcessesByName("MSIAfterburner").Any())
                            {
                                IntPtr wdwIntPtr = FindWindow(null, "MSI Afterburner ");

                                if ((int)wdwIntPtr > 1)
                                {
                                    Thread.Sleep(2000);//обязательная пауза
                                    break;
                                }
                                repeats++;
                                Thread.Sleep(1000);
                            }
                        } while (repeats < 30);
                        

                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("MSI AB error", "Process exists? Exception on run: " + ex.Message);
                    }
                }
                if (ConfigManager.GeneralConfig.ABMinimize)
                {
                    int repeats = 0;
                    do
                    {
                        repeats++;
                        Thread.Sleep(100);
                        if (Process.GetProcessesByName("MSIAfterburner").Any())
                        {
                            IntPtr wdwIntPtr = FindWindow(null, "MSI Afterburner ");

                            if ((int)wdwIntPtr < 1) continue;

                            ShowWindow(wdwIntPtr, ShowWindowEnum.ShowMinimized);
                            break;
                        }
                    } while (repeats < 50);
                }
               
            }
            return true;
        }

        public static void MSIAfterburnerKill()
        {
            foreach (var process in Process.GetProcessesByName("MSIAfterburner"))
            {
                try { process.Kill(); }
                catch (Exception e) { Helpers.ConsolePrint("MSIAfterburnerKill", e.ToString()); }
            }
        }
        public static bool MSIAfterburnerInit()
        {
            string _ex = "";
            int repeatsab = 0;
            do
            {
                try
                {
                    Helpers.ConsolePrint("*************", "MSIAfterburnerInit 1");
                    macm = new ControlMemory();
                    macm.Connect();
                   // macm.Reinitialize();
                    Helpers.ConsolePrint("*************", "MSIAfterburnerInit 2");
                    mahm = new HardwareMonitor();
                    mahm.Connect();
                    //mahm.ReloadAll();
                    Helpers.ConsolePrint("*************", "MSIAfterburnerInit 3");

                    Helpers.ConsolePrint("*************", "***** MSI AFTERTERBURNER CONTROL HEADER *****");
                    Helpers.ConsolePrint("*************", macm.Header.ToString().Replace(";", "\r\n"));

                    // print out current MACM GPU Entry values
                    for (int i = 0; i < macm.Header.GpuEntryCount; i++)
                    {
                        Helpers.ConsolePrint("*************", "***** MSI AFTERTERBURNER GPU " + i.ToString() + " *****");
                        Helpers.ConsolePrint("*************", macm.GpuEntries[i].ToString().Replace(";", "\r\n"));
                    }
                    //macm.GpuEntries[0].CoreClockCur = 1400;
                    //macm.CommitChanges();
                    /*
                    for (int i = 0; i < macm.Header.GpuEntryCount; i++)
                    {
                        Helpers.ConsolePrint("macm: " + i.ToString(), macm.GpuEntries[i].ToString().Replace(";", "\r\n"));
                    }
                    //macm.GpuEntries[0].CoreClockCur = 1400;
                    //macm.CommitChanges();
                    // print out current Entry values
                    for (int i = 0; i < mahm.Header.GpuEntryCount; i++)
                    {
                        Helpers.ConsolePrint("mahm: " + i.ToString(), "gpuid=" + mahm.GpuEntries[i].GpuId.ToString() +
                            " " + mahm.Entries[i].ToString().Replace(";", "\r\n"));

                    }
                    */
                    break;
                }
                catch (Exception ex)
                {
                    //_ex = ex.Message;
                    //Connected to invalid MSI Afterburner shared memory.
                    //Could not connect to MSI Afterburner 2.1 or later.

                    //GPU 0 does not support changing the core voltage.
                    Helpers.ConsolePrint("MSI AB error:", ex.Message);
                    if (ex.InnerException != null)
                        Helpers.ConsolePrint("MSI AB error message", ex.InnerException.Message);

                    //break;
                    if (macm != null) macm.Disconnect();
                    if (mahm != null) mahm.Disconnect();

                    MSIAfterburner.macm = null;
                    MSIAfterburner.mahm = null;

                    if (ex.Message.Contains("not connect"))
                    {
                        Thread.Sleep(2000);
                    }

                    if (ex.Message.Contains("invalid"))
                    {
                        Helpers.ConsolePrint("MSI AB", "Killing old instance AB and run again");
                        MSIAfterburnerKill();
                        Thread.Sleep(1000);
                        MSIAfterburnerRUN(true);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
                repeatsab++;
            } while (repeatsab < 5);

            if (repeatsab >= 5)
            {
                Helpers.ConsolePrint("MSI AB error:", "Fatal error");
                return false;
            }
            return true;
        }
    }
}
