﻿using MSI.Afterburner;
using MSI.Afterburner.Exceptions;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMinerLegacy.Common.Enums;
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
using static NiceHashMiner.Devices.ComputeDeviceManager;

namespace NiceHashMiner.Devices
{
    public static class MSIAfterburner
    {
        public static List<ABData> gpuList = new List<ABData>();
        [Serializable]
        public struct ABData
        {
            public int index;
            public int busID;
            public int CoreClockBoostCur;
            public int CoreClockBoostDef;
            public int CoreClockBoostMin;
            public int CoreClockBoostMax;
            public int MemoryClockBoostCur;
            public int MemoryClockBoostDef;
            public int MemoryClockBoostMin;
            public int MemoryClockBoostMax;
            public int CoreVoltageBoostCur;
            public int CoreVoltageBoostDef;
            public int CoreVoltageBoostMin;
            public int CoreVoltageBoostMax;
            public int MemoryVoltageBoostCur;
            public int MemoryVoltageBoostDef;
            public int MemoryVoltageBoostMin;
            public int MemoryVoltageBoostMax;
            public int PowerLimitCur;
            public int PowerLimitDef;
            public int PowerLimitMin;
            public int PowerLimitMax;
            public int FanFlagsCur;//0-none, 1-auto
            public int FanSpeedCur;
            public int FanSpeedDef;
            public int FanSpeedMin;
            public int FanSpeedMax;
            public int ThermalLimitCur;
            public int ThermalLimitDef;
            public int ThermalLimitMin;
            public int ThermalLimitMax;

        }
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowTitle);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);
        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };
        private struct Windowplacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        public static ControlMemory macm;
        public static HardwareMonitor mahm;
        public static bool Initialized = false;
        private static int MSIAB_exited = 0;
        private static bool MSIAB_starting = false;
        public static bool MSIAfterburnerCheckPath()
        {
            string msiabpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\MSI Afterburner\\MSIAfterburner.exe";
            if (File.Exists(msiabpath)) return true;
            return false;
        }

        public static bool CheckMSIAfterburner()
        {
            bool running = false;
            int countab = 0;
            do
            {
                Thread.Sleep(100);
                countab++;
                if (Process.GetProcessesByName("MSIAfterburner").Any())
                {
                    running = true;
                    break;
                }
            } while (countab < 20); //2 sec
            if (!running)
            {
                Helpers.ConsolePrint("CheckMSIAfterburner", "MSIAfterburner not running");
                MSIAfterburnerRUN();
                //MSIAfterburnerInit();
            }

            return true;
        }
        public static bool MSIAfterburnerRUN(bool forceRun = false)
        {
            if (MSIAB_starting) return true;
            MSIAB_starting = true;
            //[Settings]
            //VDDC_Generic_Detection = 1
            string msiabpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\MSI Afterburner\\MSIAfterburner.exe";
            string msiabdir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\MSI Afterburner\\";
            if (ConfigManager.GeneralConfig.ABEnableOverclock || forceRun)
            {
                WaitingForm waiting = new WaitingForm();
                waiting.ShowWaitingBox();
                //if (ConfigManager.GeneralConfig.AB_ForceRun && !Process.GetProcessesByName("MSIAfterburner").Any())
                {
                    if (!MSIAfterburnerCheckPath())
                    {
                        //if (waiting != null) waiting.CloseWaitingBox();
                        new Task(() =>
                            MessageBox.Show(string.Format(International.GetText("FormSettings_AB_FileNotFound"),
                    msiabpath), "MSI Afterburner error!",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();
                        MSIAB_starting = false;
                        return false;
                    }

                    var Pr = Process.GetProcessesByName("MSIAfterburner");
                    if (Pr.Length > 0)
                    {
                        waiting.SetText("", International.GetText("MSIAB_Closing"));
                        MSIAfterburnerKill();
                        Thread.Sleep(1000);//обязательная пауза
                    }

                    waiting.SetText("", International.GetText("MSIAB_Starting"));
                    //waiting.ShowWaitingBox();
                    Process P = new Process();
                    try
                    {

                        P.StartInfo.FileName = msiabpath;

                        P.StartInfo.Verb = "runas";
                        P.StartInfo.UseShellExecute = true;
                        // P.Exited += new EventHandler(MSIABprocessExited);
                        //P.EnableRaisingEvents = true;
                        if (ConfigManager.GeneralConfig.ABMinimize)
                        {
                            P.StartInfo.Arguments = "-m";
                        }
                            P.Start();

                        int repeats = 0;
                        IntPtr wdwIntPtr = new IntPtr();

                        do
                        {
                            Helpers.ConsolePrint("MSIAfterburnerRUN", "Check MSI Afterburner instance. Try " + repeats.ToString());
                            if (Process.GetProcessesByName("MSIAfterburner").Any())
                            {
                                wdwIntPtr = FindWindow(null, "MSI Afterburner ");

                                if ((int)wdwIntPtr > 1)
                                {
                                    Thread.Sleep(1000);//обязательная пауза
                                    waiting.SetText("", International.GetText("MSIAB_Starting") + " 25%");
                                    Thread.Sleep(1000);//обязательная пауза
                                    waiting.SetText("", International.GetText("MSIAB_Starting") + " 50%");
                                    Thread.Sleep(1000);//обязательная пауза
                                    waiting.SetText("", International.GetText("MSIAB_Starting") + " 75%");
                                    Thread.Sleep(1000);//обязательная пауза
                                    waiting.SetText("", International.GetText("MSIAB_Starting") + " 100%");
                                    break;
                                }
                                repeats++;

                            }
                            Thread.Sleep(1000);
                        } while (repeats < 30);
                        waiting.SetText("", International.GetText("MSIAB_Checking"));
                        repeats = 0;
                        bool meminit = false;
                        do
                        {
                            Helpers.ConsolePrint("MSIAfterburnerRUN", "Check MSI Afterburner shared memory. Try " + repeats.ToString());
                            IntPtr handle = MSI.Afterburner.SharedMemory.CheckSharedMemory("MACMSharedMemory", Win32API.FileMapAccess.FileMapAllAccess);
                            //Helpers.ConsolePrint("MSIAfterburnerRUN", "handle: " + handle.ToString());
                            if (handle != IntPtr.Zero)
                            {
                                meminit = true;
                                Win32API.CloseHandle(handle);
                                if (ConfigManager.GeneralConfig.ABMinimize)
                                {
                                    //Windowplacement placement = new Windowplacement();
                                    //GetWindowPlacement(wdwIntPtr, ref placement);
                                    //ShowWindow(wdwIntPtr, ShowWindowEnum.ForceMinimized);
                                }
                                Thread.Sleep(1000);//обязательная пауза
                                waiting.SetText("", International.GetText("MSIAB_Checking") + " 25%");
                                Thread.Sleep(1000);//обязательная пауза
                                waiting.SetText("", International.GetText("MSIAB_Checking") + " 50%");
                                Thread.Sleep(1000);//обязательная пауза
                                waiting.SetText("", International.GetText("MSIAB_Checking") + " 75%");
                                Thread.Sleep(1000);//обязательная пауза
                                waiting.SetText("", International.GetText("MSIAB_Checking") + " 100%");
                                P.Exited += new EventHandler(MSIABprocessExited);
                                P.EnableRaisingEvents = true;
                                break;
                            }
                            repeats++;
                            Thread.Sleep(1000);
                        } while (repeats < 10);

                        if (!meminit)
                        {
                            Thread.Sleep(200);
                            if (waiting != null) waiting.CloseWaitingBox();
                            MSIAB_starting = false;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (waiting != null) waiting.CloseWaitingBox();
                        Helpers.ConsolePrint("MSI AB error", "Process exists? Exception on run: " + ex.Message);
                        new Task(() =>
                        MessageBox.Show(International.GetText("FormSettings_AB_Error"), "MSI Afterburner error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();
                        MSIAB_starting = false;
                        return false;
                    }
                }
                if (waiting != null)
                {
                    Thread.Sleep(100);
                    waiting.CloseWaitingBox();
                }
            }
            MSIAB_starting = false;
            return true;
        }

        private static void MSIABprocessExited(object sender, EventArgs e)
        {
            if (Form_Main.ProgramClosing) return;
            Helpers.ConsolePrint("MSIABprocessExited", "MSI Afterburner exited");
            MSIAB_exited++;
            if (MSIAB_exited >= 5)
            {
                new Task(() =>
                        MessageBox.Show("To many errors. Bad MSI Afterburner installation? Overclock disabled!", "MSI Afterburner error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();
                ConfigManager.GeneralConfig.ABEnableOverclock = false;
                return;

            }
            MSIAfterburnerRUN();
            Thread.Sleep(50);
            //CheckMSIAfterburner();
        }

        public static void MSIAfterburnerKill()
        {
            foreach (var process in Process.GetProcessesByName("MSIAfterburner"))
            {
                try { process.CloseMainWindow(); }
                catch (Exception e) { Helpers.ConsolePrint("MSIAfterburnerKill", e.ToString()); }
            }
            Initialized = false;
        }
        public static void FirstInitFiles()
        {
            WaitingForm waiting = new WaitingForm();
            waiting.SetText("", "Initializing");
            waiting.ShowWaitingBox();
            foreach (var dev in Available.Devices)
            {
                waiting.SetText("", "Initializing " + dev.Name);
                if (dev.DeviceType != DeviceType.CPU)
                {
                    foreach (var alg in dev.GetAlgorithmSettings())
                    {
                        string fName = "configs\\overclock\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".gpu";
                        if (!File.Exists(fName))
                        {
                            Helpers.ConsolePrint("FirstInitFiles", "Init filedata for busId: " + dev.BusID + " algo: " + alg.AlgorithmStringID);
                            SaveDefaultDeviceData(dev.BusID, fName);
                        }
                    }
                }
            }
            waiting.CloseWaitingBox();
            return;
        }
        public static void InitTempFiles()
        {
            CheckMSIAfterburner();
            foreach (var dev in Available.Devices)
                if (dev.DeviceType != DeviceType.CPU)
                {
                    foreach (var alg in dev.GetAlgorithmSettings())
                    {
                        try
                        {
                            string fNameSrc = "configs\\overclock\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".gpu";
                            string fNameDst = "temp\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".tmp";
                            if (!File.Exists(fNameSrc))
                            {
                                SaveDefaultDeviceData(dev.BusID, fNameSrc);
                            }
                            if (File.Exists(fNameDst)) File.Delete(fNameDst);

                            File.Copy(fNameSrc, fNameDst);
                        }
                        catch (Exception ex)
                        {
                            Helpers.ConsolePrint("InitTempFiles", "Error: " + ex.ToString());
                        }
                    }
                }
            return;
        }
        public static void CopyFromTempFiles()
        {
            CheckMSIAfterburner();
            foreach (var dev in Available.Devices)
                if (dev.DeviceType != DeviceType.CPU)
                {
                    foreach (var alg in dev.GetAlgorithmSettings())
                    {
                        try
                        {
                            string fNameDst = "configs\\overclock\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".gpu";
                            string fNameSrc = "temp\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".tmp";
                            if (!File.Exists(fNameSrc))
                            {
                                SaveDefaultDeviceData(dev.BusID, fNameSrc);
                            }
                            if (File.Exists(fNameDst)) File.Delete(fNameDst);

                            File.Copy(fNameSrc, fNameDst);
                        }
                        catch (Exception ex)
                        {
                            Helpers.ConsolePrint("CopyFromTempFiles", "Error: " + ex.ToString());
                        }
                    }
                }
            return;
        }
        public static ControlMemoryGpuEntry GetDeviceData(int _busID)
        {
            CheckMSIAfterburner();
            if (!Initialized) return new ControlMemoryGpuEntry();
            bool found = false;
            macm.ReloadHeader();
            mahm.ReloadHeader();
            var devData = new ControlMemoryGpuEntry();
            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    macm.ReloadGpuEntry(i);
                    mahm.ReloadGpuEntry((uint)i);
                    devData = macm.GpuEntries[i];
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                if (_busID != -1)
                {
                    Helpers.ConsolePrint("MSIAfterburner GetDeviceData", "Error! Device with busID " + _busID.ToString() + " not found!");
                }
            }
            return devData;
        }
        public static void ResetToDefaults(int _busID, bool commit)
        {
            CheckMSIAfterburner();
            if (!Initialized) return;
            macm.ReloadAll();
            mahm.ReloadAll();
            int index = -1;
            var devType = new DeviceType();
            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    foreach (var dev in Available.Devices)
                    {
                        if (dev.BusID == busID)
                        {
                            devType = dev.DeviceType;
                            break;
                        }
                    }
                    index = macm.GpuEntries[i].Index;
                    //Helpers.ConsolePrint("ResetToDefaults", "MSIAfterburner.ResetToDefaults: " + busID.ToString());
                    if (devType == DeviceType.NVIDIA)
                    {
                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK_BOOST))
                        {
                            macm.GpuEntries[i].CoreClockBoostCur = macm.GpuEntries[i].CoreClockBoostDef;//nvidia
                        }

                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE_BOOST))
                        {
                            macm.GpuEntries[i].CoreVoltageBoostCur = macm.GpuEntries[i].CoreVoltageBoostDef;
                        }

                        macm.GpuEntries[i].FanFlagsCur = macm.GpuEntries[i].FanFlagsDef;
                        //macm.GpuEntries[i].FanSpeedCur = macm.GpuEntries[i].FanSpeedDef;//auto

                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED))
                        {
                            macm.GpuEntries[i].Flags = macm.GpuEntries[i].Flags - (int)MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED;
                        }

                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK_BOOST))
                        {
                            macm.GpuEntries[i].MemoryClockBoostCur = macm.GpuEntries[i].MemoryClockBoostDef;
                        }

                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE_BOOST))
                        {
                            macm.GpuEntries[i].MemoryVoltageBoostCur = macm.GpuEntries[i].MemoryVoltageBoostDef;
                        }


                        if (devType == DeviceType.AMD)
                        {
                            if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK))
                            {
                                macm.GpuEntries[i].CoreClockCur = macm.GpuEntries[i].CoreClockDef;//amd
                            }
                            if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE))
                            {
                                macm.GpuEntries[i].CoreVoltageCur = macm.GpuEntries[i].CoreVoltageDef;
                            }
                            if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK))
                            {
                                macm.GpuEntries[i].MemoryClockCur = macm.GpuEntries[i].MemoryClockDef;
                            }
                            if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE))
                            {
                                macm.GpuEntries[i].MemoryVoltageCur = macm.GpuEntries[i].MemoryVoltageDef;
                            }
                        }
                        //all
                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.POWER_LIMIT))
                        {
                            macm.GpuEntries[i].PowerLimitCur = macm.GpuEntries[i].PowerLimitDef;
                        }

                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.SHADER_CLOCK))
                        {
                            macm.GpuEntries[i].ShaderClockCur = macm.GpuEntries[i].ShaderClockDef;//nvidia not suppored
                        }
                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.THERMAL_LIMIT))
                        {
                            macm.GpuEntries[i].ThermalLimitCur = macm.GpuEntries[i].ThermalLimitDef;
                        }
                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.THERMAL_PRIORITIZE))
                        {
                            //macm.GpuEntries[i].thermalPrioritizeCur = macm.GpuEntries[i].thermalPrioritizeDef;
                        }
                        break;
                    }
                }
            }
            if (commit)
            {
                //macm.CommitChanges();
            }
            if (index == -1)
            {
                if (_busID != -1)
                {
                    Helpers.ConsolePrint("MSIAfterburner ResetToDefaults", "Error! Device with busID " + _busID.ToString() + " not found!");
                }
                return;
            }
        }
        public static void SaveDefaultDeviceData(int _busID, string FileName)
        {
            CheckMSIAfterburner();
            if (!Initialized) return;
            int index = -1;
            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    index = macm.GpuEntries[i].Index;
                    macm.ReloadGpuEntry(index);
                    mahm.ReloadGpuEntry((uint)index);
                    break;
                }
            }
            if (index == -1)
            {
                if (_busID != -1)
                {
                    Helpers.ConsolePrint("MSIAfterburner SaveDefaultDeviceData", "Error! Device with busID " + _busID.ToString() + " not found!");
                }
                return;
            }
            ResetToDefaults(_busID, false);

            try
            {
                byte[] buffer = RawSerialize(macm.GpuEntries[index], (int)macm.Header.GpuEntrySize);
                File.WriteAllBytes(FileName, buffer);
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner SaveDefaultDeviceData", "Error: " + ex.ToString());
            }
        }
        public static void SaveDeviceData(ControlMemoryGpuEntry abdata, string FileName)
        {
            CheckMSIAfterburner();
            if (!Initialized) return;

            try
            {
                byte[] buffer = RawSerialize(abdata, (int)macm.Header.GpuEntrySize);
                File.WriteAllBytes(FileName, buffer);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner SaveDeviceData", "Error: " + ex.ToString());
            }
        }
        public static void ApplyFromFile(int _busID, string FileName)
        {
            ControlMemoryGpuEntry dev = ReadFromFile(_busID, FileName);

            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    macm.GpuEntries[i] = dev;
                }
            }
            //macm.CommitChanges();
        }
        public static void CommitChanges()
        {
            macm.CommitChanges();
        }
        public static void CommitChanges(int _busID)
        {
            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    macm.CommitChanges(i);
                    break;
                }
            }
        }
        public static ControlMemoryGpuEntry ReadFromFile(int _busID, string FileName)
        {
            //CheckMSIAfterburner();
            if (!Initialized) return new ControlMemoryGpuEntry();
            ControlMemoryGpuEntry dev = new ControlMemoryGpuEntry();
            int index = -1;
            try
            {
                for (int i = 0; i < macm.Header.GpuEntryCount; i++)
                {
                    int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                    if (busID == _busID)
                    {
                        index = macm.GpuEntries[i].Index;
                        if (File.Exists(FileName))
                        {
                            byte[] buffer = File.ReadAllBytes(FileName);
                            buffer = ReplaceBytes(buffer, Encoding.ASCII.GetBytes("BUS_"), Encoding.ASCII.GetBytes("BUS_" + _busID.ToString()));
                            //Helpers.ConsolePrint("MSIAfterburner ReadFromFile", "Load from file: " + FileName);

                            dev = RawDeserialize(buffer, macm.GpuEntries[i]);
                            if (dev == null)
                            {
                                Helpers.ConsolePrint("MSIAfterburner ReadFromFile", "Error buffer size mismath?");
                                break;
                            }
                            else
                            {
                                break;
                            }
                        } else
                        {
                            Helpers.ConsolePrint("MSIAfterburner ReadFromFile", "Error. File not found: " + FileName);
                            return new ControlMemoryGpuEntry();
                        }
                    }
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner ReadFromFile", "Error: " + ex.ToString());
            }
            if (index == -1)
            {
                if (_busID != -1)//cpu
                {
                    Helpers.ConsolePrint("MSIAfterburner ReadFromFile", "Error! Device with busID " + _busID.ToString() + " not found!");
                }
                return new ControlMemoryGpuEntry();
            }

            return dev;
        }

        public static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            if (repl == null) return src;
            int index = FindBytes(src, search);
            if (index < 0) return src;
            //byte[] dst = new byte[src.Length - search.Length + repl.Length];
            byte[] dst = new byte[src.Length];
            //Buffer.BlockCopy(src, 0, dst, 0, index);
            Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            //Buffer.BlockCopy(src, index + search.Length, dst, index + repl.Length, src.Length - (index + search.Length));
            return dst;
        }

        public static int FindBytes(byte[] src, byte[] find)
        {
            if (src == null || find == null || src.Length == 0 || find.Length == 0 || find.Length > src.Length) return -1;
            for (int i = 0; i < src.Length - find.Length + 1; i++)
            {
                if (src[i] == find[0])
                {
                    for (int m = 1; m < find.Length; m++)
                    {
                        if (src[i + m] != find[m]) break;
                        if (m == find.Length - 1) return i;
                    }
                }
            }
            return -1;
        }
        public static byte[] RawSerialize(ControlMemoryGpuEntry obj, int length)
        {
            IntPtr num = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(obj, num, false);
            byte[] destination = new byte[length];
            Marshal.Copy(num, destination, 0, length);
            Marshal.FreeHGlobal(num);
            return destination;
        }

        internal static ControlMemoryGpuEntry RawDeserialize(byte[] rawdatas, ControlMemoryGpuEntry anytype)
        {
            int num1 = Marshal.SizeOf(anytype) - 8;
            if (num1 > rawdatas.Length)
            {
                Helpers.ConsolePrint("RawDeserialize", "Error! Buffer size mismath: " + num1.ToString() + "!=" + rawdatas.Length.ToString());
                return (ControlMemoryGpuEntry)null;
            }
            IntPtr num2 = Marshal.AllocHGlobal(num1);
            Marshal.Copy(rawdatas, 0, num2, num1);
            ControlMemoryGpuEntry structure = (ControlMemoryGpuEntry)Marshal.PtrToStructure(num2, anytype.GetType());
            Marshal.FreeHGlobal(num2);
            return (ControlMemoryGpuEntry)structure;
        }
        public static bool MSIAfterburnerInit()
        {
            int repeatsab = 0;
            do
            {
                try
                {
                    if (macm != null) macm.Disconnect();
                    if (mahm != null) mahm.Disconnect();
                    macm = new ControlMemory();
                    macm.Connect();
                    mahm = new HardwareMonitor();
                    mahm.Connect();
                    Initialized = true;

                    for (int i = 0; i < macm.Header.GpuEntryCount; i++)
                    {
                        /*
                        if (macm.GpuEntries[i].GpuId.Contains("DEV_9498"))
                        {
                            Helpers.ConsolePrint("MSIAfterburnerInit", "Ahtung!");
                            break;
                        }
                        */
                        /*
                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED))
                        {
                            Helpers.ConsolePrint("MSIAfterburnerInit", "GPU"+i.ToString() + " Curve enabled");
                        }
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.GpuId " + macm.GpuEntries[i].GpuId);
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreClockBoostCur " + macm.GpuEntries[i].CoreClockBoostCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreClockCur " + macm.GpuEntries[i].CoreClockCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreClockDef " + macm.GpuEntries[i].CoreClockDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreClockMax " + macm.GpuEntries[i].CoreClockMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreClockMin " + macm.GpuEntries[i].CoreClockMin.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreVoltageBoostCur" + macm.GpuEntries[i].CoreVoltageBoostCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreVoltageCur" + macm.GpuEntries[i].CoreVoltageCur.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreVoltageDef" + macm.GpuEntries[i].CoreVoltageDef.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreVoltageMax" + macm.GpuEntries[i].CoreVoltageMax.ToString());
                        Helpers.ConsolePrint("MSIAfterburnerInit", "GpuEntries.CoreVoltageMin" + macm.GpuEntries[i].CoreVoltageMin.ToString());
                        */
                    }

                    FirstInitFiles();
                    break;

                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("MSIAfterburnerInit", ex.Message);
                    if (ex.InnerException != null)
                        Helpers.ConsolePrint("MSIAfterburnerInitMessage", ex.InnerException.Message);

                    if (ex.Message.Contains("not support"))
                    {
                        break;
                    }

                    if (ex.Message.Contains("not connect"))
                    {
                        if (macm != null) macm.Disconnect();
                        if (mahm != null) mahm.Disconnect();
                        Initialized = false;
                        MSIAfterburner.macm = null;
                        MSIAfterburner.mahm = null;
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Killing old instance AB and run again");
                        //MSIAfterburnerKill();
                        Thread.Sleep(10);
                        MSIAfterburnerRUN(true);
                        Thread.Sleep(100);
                    }

                    if (ex.Message.Contains("invalid"))
                    {
                        if (macm != null) macm.Disconnect();
                        if (mahm != null) mahm.Disconnect();
                        Initialized = false;
                        MSIAfterburner.macm = null;
                        MSIAfterburner.mahm = null;
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Killing old instance AB and run again");
                        //MSIAfterburnerKill();
                        Thread.Sleep(10);
                        MSIAfterburnerRUN(true);
                        Thread.Sleep(100);
                    }
                    if (ex.Message.Contains("dead"))
                    {
                        if (macm != null) macm.Disconnect();
                        if (mahm != null) mahm.Disconnect();
                        Initialized = false;
                        MSIAfterburner.macm = null;
                        MSIAfterburner.mahm = null;
                        Helpers.ConsolePrint("MSIAfterburnerInit", "Killing old instance AB and run again");
                        //MSIAfterburnerKill();
                        Thread.Sleep(10);
                        MSIAfterburnerRUN(true);
                        Thread.Sleep(100);
                    }
                }
                Thread.Sleep(1000);
                repeatsab++;
            } while (repeatsab < 5);

            if (repeatsab >= 5)
            {
                Helpers.ConsolePrint("MSIAfterburnerInit", "Fatal error");
                new Task(() =>
                        MessageBox.Show(International.GetText("FormSettings_AB_Error"), "MSI Afterburner error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();
                return false;
            }
            return true;
        }
    }
}
