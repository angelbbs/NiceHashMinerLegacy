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
                                    Thread.Sleep(3000);//обязательная пауза
                                    break;
                                }
                                repeats++;
                                Thread.Sleep(1000);
                            }
                            Thread.Sleep(1000);
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
                    //macm.ReloadAll();
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
                        macm.GpuEntries[0].FanFlagsCur = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None;
                        macm.GpuEntries[0].FanSpeedCur = 80;
                        //macm.GpuEntries[i].Flags = MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK; //2147630216
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Flags: " + macm.GpuEntries[i].Flags.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Header.HeaderSize: " + macm.Header.HeaderSize.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Header.GpuEntrySize: " + macm.Header.GpuEntrySize.ToString());

                        macm.GpuEntries[i].CoreClockBoostCur = -234000  - i * 1000;
                        macm.CommitChanges(i);
                        //System.Threading.Thread.Sleep(2000);
                        //macm.ReloadAll();
                        Helpers.ConsolePrint("MSIAfterburnerInit", macm.GpuEntries[i].ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Index: " + macm.GpuEntries[i].Index.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuId: " + mahm.GpuEntries[i].GpuId.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Index: " + mahm.GpuEntries[i].Index.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Device: " + mahm.GpuEntries[i].Device.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "IsMaster: " + macm.GpuEntries[i].IsMaster.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "AuxVoltageBoostCur: " + macm.GpuEntries[i].AuxVoltageBoostCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "AuxVoltageBoostDef: " + macm.GpuEntries[i].AuxVoltageBoostDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "AuxVoltageBoostMax: " + macm.GpuEntries[i].AuxVoltageBoostMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "AuxVoltageBoostMin: " + macm.GpuEntries[i].AuxVoltageBoostMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "AuxVoltageCur: " + macm.GpuEntries[i].AuxVoltageCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "AuxVoltageDef: " + macm.GpuEntries[i].AuxVoltageDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "AuxVoltageMax: " + macm.GpuEntries[i].AuxVoltageMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "AuxVoltageMin: " + macm.GpuEntries[i].AuxVoltageMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreClockBoostCur: " + macm.GpuEntries[i].CoreClockBoostCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreClockBoostDef: " + macm.GpuEntries[i].CoreClockBoostDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreClockBoostMax: " + macm.GpuEntries[i].CoreClockBoostMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreClockBoostMin: " + macm.GpuEntries[i].CoreClockBoostMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreClockCur: " + macm.GpuEntries[i].CoreClockCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreClockDef: " + macm.GpuEntries[i].CoreClockDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreClockMax: " + macm.GpuEntries[i].CoreClockMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreClockMin: " + macm.GpuEntries[i].CoreClockMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreVoltageBoostCur: " + macm.GpuEntries[i].CoreVoltageBoostCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreVoltageBoostDef: " + macm.GpuEntries[i].CoreVoltageBoostDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreVoltageBoostMax: " + macm.GpuEntries[i].CoreVoltageBoostMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreVoltageBoostMin: " + macm.GpuEntries[i].CoreVoltageBoostMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreVoltageCur: " + macm.GpuEntries[i].CoreVoltageCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreVoltageDef: " + macm.GpuEntries[i].CoreVoltageDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreVoltageMax: " + macm.GpuEntries[i].CoreVoltageMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "CoreVoltageMin: " + macm.GpuEntries[i].CoreVoltageMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "FanFlagsCur: " + macm.GpuEntries[i].FanFlagsCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "FanFlagsDef: " + macm.GpuEntries[i].FanFlagsDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "FanSpeedCur: " + macm.GpuEntries[i].FanSpeedCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "FanSpeedDef: " + macm.GpuEntries[i].FanSpeedDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "FanSpeedMax: " + macm.GpuEntries[i].FanSpeedMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "FanSpeedMin: " + macm.GpuEntries[i].FanSpeedMin.ToString());

                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryClockBoostCur: " + macm.GpuEntries[i].MemoryClockBoostCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryClockBoostDef: " + macm.GpuEntries[i].MemoryClockBoostDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryClockBoostMax: " + macm.GpuEntries[i].MemoryClockBoostMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryClockBoostMin: " + macm.GpuEntries[i].MemoryClockBoostMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryClockCur: " + macm.GpuEntries[i].MemoryClockCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryClockDef: " + macm.GpuEntries[i].MemoryClockDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryClockMax: " + macm.GpuEntries[i].MemoryClockMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryClockMin: " + macm.GpuEntries[i].MemoryClockMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryVoltageBoostCur: " + macm.GpuEntries[i].MemoryVoltageBoostCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryVoltageBoostDef: " + macm.GpuEntries[i].MemoryVoltageBoostDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryVoltageBoostMax: " + macm.GpuEntries[i].MemoryVoltageBoostMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryVoltageBoostMin: " + macm.GpuEntries[i].MemoryVoltageBoostMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryVoltageCur: " + macm.GpuEntries[i].MemoryVoltageCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryVoltageDef: " + macm.GpuEntries[i].MemoryVoltageDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryVoltageMax: " + macm.GpuEntries[i].MemoryVoltageMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "MemoryVoltageMin: " + macm.GpuEntries[i].MemoryVoltageMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "PowerLimitCur: " + macm.GpuEntries[i].PowerLimitCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "PowerLimitDef: " + macm.GpuEntries[i].PowerLimitDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "PowerLimitMax: " + macm.GpuEntries[i].PowerLimitMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "PowerLimitMin: " + macm.GpuEntries[i].PowerLimitMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "ShaderClockCur: " + macm.GpuEntries[i].ShaderClockCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "ShaderClockDef: " + macm.GpuEntries[i].ShaderClockDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "ShaderClockMax: " + macm.GpuEntries[i].ShaderClockMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "ShaderClockMin: " + macm.GpuEntries[i].ShaderClockMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "ThermalLimitCur: " + macm.GpuEntries[i].ThermalLimitCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "ThermalLimitDef: " + macm.GpuEntries[i].ThermalLimitDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "ThermalLimitMax: " + macm.GpuEntries[i].ThermalLimitMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "ThermalLimitMin: " + macm.GpuEntries[i].ThermalLimitMin.ToString());


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
