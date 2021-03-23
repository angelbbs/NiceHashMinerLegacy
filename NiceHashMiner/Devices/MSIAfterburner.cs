using MSI.Afterburner;
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
                        do
                        {
                            Thread.Sleep(100);
                            if (P.Responding)
                            {
                                break;
                            }
                        } while (true);
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
        public static bool MSIAfterburnerInit()
        {
            try
            {
                macm = new ControlMemory();
                mahm = new HardwareMonitor();
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("MSI AB error:", ex.Message);
                if (ex.InnerException != null)
                    Helpers.ConsolePrint("MSI AB error message", ex.InnerException.Message);
                return false;
            }
            return true;
        }
    }
}
