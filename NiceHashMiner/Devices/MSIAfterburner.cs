using MSI.Afterburner;
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

        public static bool _CheckMSIAfterburner(int count = 10)
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
            } while (countab < count); //default 1 sec
            if (!running)
            {
                return false;
            }
            return true;
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
                        waiting.CloseWaitingBox();
                        return false;
                    }

                    var Pr = Process.GetProcessesByName("MSIAfterburner");
                    if (Pr.Length > 0)
                    {
                        waiting.SetText("", International.GetText("MSIAB_Closing"));
                        MSIAfterburnerKill();
                        int count = 0;
                        do
                        {
                            if (_CheckMSIAfterburner()) break;
                            count++;
                        } while (count < 5);
                        //Thread.Sleep(1000);//обязательная пауза
                    }

                    waiting.SetText("", International.GetText("MSIAB_Starting"));
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
                                    /*
                                    waiting.SetText("", International.GetText("MSIAB_Starting") + " 25%");
                                    Thread.Sleep(1000);//обязательная пауза

                                    waiting.SetText("", International.GetText("MSIAB_Starting") + " 50%");
                                    Thread.Sleep(1000);//обязательная пауза

                                    waiting.SetText("", International.GetText("MSIAB_Starting") + " 75%");
                                    Thread.Sleep(1000);//обязательная пауза

                                    waiting.SetText("", International.GetText("MSIAB_Starting") + " 100%");
                                    Thread.Sleep(1000);//обязательная пауза
                                    */
                                    break;
                                }
                                repeats++;
                                Thread.Sleep(100);
                            }
                            Thread.Sleep(1000);
                        } while (repeats < 15);//15 sec
                        Thread.Sleep(1000);//обязательная пауза

                        waiting.SetText("", International.GetText("MSIAB_Checking"));
                        repeats = 0;
                        bool meminit = false;
                        do
                        {
                            Helpers.ConsolePrint("MSIAfterburnerRUN", "Check MSI Afterburner shared memory. Try " + repeats.ToString());
                            IntPtr handle = MSI.Afterburner.SharedMemory.CheckSharedMemory("MACMSharedMemory", Win32API.FileMapAccess.FileMapAllAccess);

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
                                /*
                                waiting.SetText("", International.GetText("MSIAB_Checking") + " 25%");
                                Thread.Sleep(1000);//обязательная пауза

                                waiting.SetText("", International.GetText("MSIAB_Checking") + " 50%");
                                Thread.Sleep(1000);//обязательная пауза

                                waiting.SetText("", International.GetText("MSIAB_Checking") + " 75%");
                                Thread.Sleep(1000);//обязательная пауза

                                waiting.SetText("", International.GetText("MSIAB_Checking") + " 100%");
                                Thread.Sleep(1000);//обязательная пауза
                                */
                                P.Exited += new EventHandler(MSIABprocessExited);
                                P.EnableRaisingEvents = true;
                                break;
                            }
                            repeats++;
                            Thread.Sleep(1000);
                        } while (repeats < 10);//10 sec
                        Thread.Sleep(1000);//обязательная пауза


                        if (!meminit)
                        {
                            Thread.Sleep(200);
                            waiting.SetText("", "");
                            //waiting.Update();
                            Thread.Sleep(100);
                            try
                            {
                                waiting.CloseWaitingBox();
                            } catch (Exception ex)
                            {
                                Helpers.ConsolePrint("MSIAfterburnerRUN", ex.ToString());
                            }
                            MSIAB_starting = false;
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        waiting.SetText("", "");
                        //waiting.Update();
                        Thread.Sleep(100);
                        try
                        {
                            waiting.CloseWaitingBox();
                        }
                        catch (Exception exept)
                        {
                            Helpers.ConsolePrint("MSIAfterburnerRUN", exept.ToString());
                        }
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
                    try
                    {
                        waiting.CloseWaitingBox();
                        Thread.Sleep(500);
                        waiting.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("MSIAfterburnerRUN", ex.ToString());
                    }
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
        }

        public static void MSIAfterburnerKill()
        {
            foreach (var process in Process.GetProcessesByName("MSIAfterburner"))
            {
                try
                {
                    process.Close();
                    //process.CloseMainWindow();
                }
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
                if (dev.DeviceType != DeviceType.CPU && dev.DeviceType != DeviceType.INTEL)
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
            try
            {
                Thread.Sleep(100);
                waiting.CloseWaitingBox();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("FirstInitFiles", ex.ToString());
            }
            return;
        }
        public static void InitTempFiles()
        {
            CheckMSIAfterburner();
            foreach (var dev in Available.Devices)
                if (dev.DeviceType != DeviceType.CPU && dev.DeviceType != DeviceType.INTEL)
                {
                    foreach (var alg in dev.GetAlgorithmSettings())
                    {
                        try
                        {
                            string fNameSrc = "configs\\overclock\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".gpu";
                            string fNameDst = "temp\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".gputmp";
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
                if (dev.DeviceType != DeviceType.CPU && dev.DeviceType != DeviceType.INTEL)
                {
                    foreach (var alg in dev.GetAlgorithmSettings())
                    {
                        try
                        {
                            string fNameDst = "configs\\overclock\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".gpu";
                            string fNameSrc = "temp\\" + dev.Uuid + "_" + alg.AlgorithmStringID + ".gputmp";
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
                if (mahm.GpuEntries[i].Device.Contains("Intel")) continue;
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    try
                    {
                        macm.ReloadGpuEntry(i);
                        mahm.ReloadGpuEntry((uint)i);
                        devData = macm.GpuEntries[i];
                        found = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("MSIAfterburner GetDeviceData", "Error: " + ex.ToString());
                    }
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

        //MSI AB не отдает данные о загрузке контроллера памяти для AMD. Их нет в MONITORING_SOURCE_ID
        /*
        public static HardwareMonitorEntry GetDeviceMemoryLoad(int _busID)
        {
            CheckMSIAfterburner();
            if (!Initialized) return new HardwareMonitorEntry();
            bool found = false;
            mahm.ReloadHeader();
            var devData = new HardwareMonitorEntry();
            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    try
                    {
                        macm.ReloadGpuEntry(i);
                        mahm.ReloadGpuEntry((uint)i);
                        devData = mahm.GetEntry((uint)i, MONITORING_SOURCE_ID.UNK1);
                        found = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("MSIAfterburner GetDeviceData", "Error: " + ex.ToString());
                    }
                }
            }
            if (!found)
            {
                if (_busID != -1)
                {
                    Helpers.ConsolePrint("MSIAfterburner GetDeviceMemoryLoad", "Error! Device with busID " + _busID.ToString() + " not found!");
                }
            }
            return devData;
        }
        */

        public static void ResetDef(int _busID)
        {
            ControlMemoryGpuEntry _abdata = new ControlMemoryGpuEntry();
            if (!Initialized) return;
            macm.ReloadAll();
            mahm.ReloadAll();
            int index = -1;
            var devType = new DeviceType();
            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                if (mahm.GpuEntries[i].Device.Contains("Intel")) continue;
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    foreach (var dev in Available.Devices)
                    {
                        if (dev.BusID == busID)
                        {
                            devType = dev.DeviceType;
                            _abdata = MSIAfterburner.GetDeviceData(dev.BusID);

                            _abdata.MemoryClockCur = _abdata.MemoryClockDef;
                            _abdata.CoreClockCur = _abdata.CoreClockDef;

                            Thread.Sleep(100);
                            CommitChanges();
                            Thread.Sleep(100);

                            macm.GpuEntries[i] = _abdata;
                            break;
                        }
                    }
                }
            }

        }

        public static bool locked = false;
        public static bool ResetCurveLock(int _busID, bool commit = false)
        {
            bool found = false;
            if (!Initialized) return false;

            try
            {
                macm.ReloadAll();
                mahm.ReloadAll();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner ResetCurveLock", "Error: " + ex.ToString());
                return false;
            }
            int index = -1;
            var devType = new DeviceType();
            
            //*****************************

            int i = 0;
            for (i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                if (mahm.GpuEntries[i].Device.Contains("Intel")) continue;
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    foreach (var dev in Available.Devices)
                    {
                        if (dev.BusID == busID)
                        {
                            devType = dev.DeviceType;
                            macm.ReloadGpuEntry(i);
                            mahm.ReloadGpuEntry((uint)i);
                            found = true;
                            break;
                        }
                    }
                    break;
                }
            }
            if (!found) return false;
            index = macm.GpuEntries[i].Index;
            if (macm.GpuEntries[i].CurveLockIndex != 0)
            {
                locked = true;
                if (commit)
                {
                    Helpers.ConsolePrint("ResetToDefaults", "GPU#" + (index + 1).ToString() + " MSI AB Reset lock curve index: " + macm.GpuEntries[i].CurveLockIndex.ToString());
                    //macm.GpuEntries[i].ClearCurve();
                    macm.GpuEntries[i].CurveLockIndex = 0;
                }
            }

            if (index == -1)
            {
                if (_busID != -1)
                {
                    Helpers.ConsolePrint("MSIAfterburner ResetCurveLock", "Error! Device with busID " + _busID.ToString() + " not found!");
                }
                return false;
            }
            return locked;
        }

        public static void ResetToDefaults(int _busID, string uuid = "", string algo = "",  bool commit = false, bool nocheck = true)
        {
            bool found = false;
            if (!nocheck)
            {
                CheckMSIAfterburner();
            }
            if (!Initialized) return;

            try
            {
                macm.ReloadAll();
                mahm.ReloadAll();
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner ResetToDefaults", "Error: " + ex.ToString());
                return;
            }
            int index = -1;
            var devType = new DeviceType();
            
            int i = 0;
            for (i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                if (mahm.GpuEntries[i].Device.Contains("Intel")) continue;
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    foreach (var dev in Available.Devices)
                    {
                        if (dev.BusID == busID)
                        {
                            devType = dev.DeviceType;
                            macm.ReloadGpuEntry(i);
                            mahm.ReloadGpuEntry((uint)i);
                            found = true;
                            break;
                        }
                    }
                    break;
                }
            }
            if (!found) return;
            index = macm.GpuEntries[i].Index;
            Helpers.ConsolePrint("ResetToDefaults", "GPU#" + (index + 1).ToString() + " BusID: " + _busID.ToString());
            if (devType == DeviceType.NVIDIA)
            {
                if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED))
                {
                    macm.GpuEntries[i].Flags = (MACM_SHARED_MEMORY_GPU_ENTRY_FLAG)(macm.GpuEntries[i].Flags - MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED);
                    macm.GpuEntries[i].ClearCurve();
                }
                
                if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE))
                {
                    //macm.GpuEntries[i].Flags = (MACM_SHARED_MEMORY_GPU_ENTRY_FLAG)(macm.GpuEntries[i].Flags - MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE);
                }
                
                if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK_BOOST))
                {
                    macm.GpuEntries[i].CoreClockBoostCur = macm.GpuEntries[i].CoreClockBoostDef;//nvidia
                }

                if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE_BOOST))
                {
                    macm.GpuEntries[i].CoreVoltageBoostCur = macm.GpuEntries[i].CoreVoltageBoostDef;
                }

                if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK_BOOST))
                {
                    macm.GpuEntries[i].MemoryClockBoostCur = macm.GpuEntries[i].MemoryClockBoostDef;
                }

                if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE_BOOST))
                {
                    macm.GpuEntries[i].MemoryVoltageBoostCur = macm.GpuEntries[i].MemoryVoltageBoostDef;
                }
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
            if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.FAN_SPEED))
            {
                macm.GpuEntries[i].FanFlagsCur = macm.GpuEntries[i].FanFlagsDef;
                if (macm.GpuEntries[i].FanFlagsCur != MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.AUTO)
                {
                    macm.GpuEntries[i].FanSpeedCur = macm.GpuEntries[i].FanSpeedDef;//auto
                }
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

        private static bool ByteArrayCompareWithSimplest(byte[] p_BytesLeft, byte[] p_BytesRight)
        {
            if (p_BytesLeft.Length != p_BytesRight.Length)
                return false;

            var length = p_BytesLeft.Length;

            for (int i = 0; i < length; i++)
            {
                if (p_BytesLeft[i] != p_BytesRight[i])
                    return false;
            }
            return true;
        }

        public static void SaveDefaultDeviceData(int _busID, string FileName)
        {
            CheckMSIAfterburner();
            if (!Initialized) return;
            int index = -1;
            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                if (mahm.GpuEntries[i].Device.Contains("Intel")) continue;
                int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                if (busID == _busID)
                {
                    index = macm.GpuEntries[i].Index;
                    try
                    {
                        macm.ReloadGpuEntry(index);
                        mahm.ReloadGpuEntry((uint)index);
                        //break;
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("MSIAfterburner SaveDefaultDeviceData", "Error: " + ex.ToString());
                        macm.ReloadGpuEntry(i);
                        mahm.ReloadGpuEntry((uint)i);
                        return;
                    }
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
            ResetToDefaults(_busID, "", "", false, false);

            try
            {
                byte[] buffer = RawSerialize(macm.GpuEntries[index], (int)macm.Header.GpuEntrySize);
                Helpers.WriteAllBytesThrough(FileName, buffer);
            }
            catch (Exception ex)
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
                Helpers.WriteAllBytesThrough(FileName, buffer);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner SaveDeviceData", "Error: " + ex.ToString());
            }
        }
        public static bool ApplyFromFile(int _busID, string FileName)
        {
            if (!File.Exists(FileName))
            {
                Helpers.ConsolePrint("MSIAfterburner ReadFromFile", "Error. File not found: " + FileName);
                return true;
            }
                int i = 0;
            ControlMemoryGpuEntry dev = null;
            try
            {
                dev = ReadFromFile(_busID, FileName);

                for (i = 0; i < macm.Header.GpuEntryCount; i++)
                {
                    if (mahm.GpuEntries[i].Device.Contains("Intel")) continue;
                    int.TryParse(mahm.GpuEntries[i].GpuId.ToString().Split('&')[4].Replace("BUS_", ""), out int busID);
                    if (busID == _busID)
                    {
                        macm.GpuEntries[i] = dev;
                        break;
                    }
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner", ex.ToString());
            }
            CommitChanges(false);
            Thread.Sleep(100);
            //***********установить задержки и сделать галочку в настройказ
            byte[] macmbuffer = RawSerialize(macm.GpuEntries[i], (int)macm.Header.GpuEntrySize);
            byte[] devbuffer = File.ReadAllBytes(FileName);

            if (ByteArrayCompareWithSimplest(macmbuffer, devbuffer))
            {
                Helpers.ConsolePrint("MSIAfterburner.ApplyFromFile", "Compare OK. busID " + _busID.ToString());
                return true;
            }
            else
            {
                Helpers.ConsolePrint("MSIAfterburner.ApplyFromFile", "Compare ERROR. busID " + _busID.ToString());
                return false;
            }
        }
        public static void Flush()
        {
            Thread.Sleep(100);
            try
            {
                macm.Flush();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner Flush", "Error: " + ex.ToString());
            }
        }
        public static void CommitChanges(bool flush = true)
        {
            try
            {
                macm.CommitChanges(flush);
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("MSIAfterburner CommitChanges", "Error: " + ex.ToString());
            }
        }
        public static void CommitChanges(int _busID)
        {
            for (int i = 0; i < macm.Header.GpuEntryCount; i++)
            {
                if (mahm.GpuEntries[i].Device.Contains("Intel")) continue;
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
                    if (mahm.GpuEntries[i].Device.Contains("Intel")) continue;
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
                        }
                        else
                        {
                            Helpers.ConsolePrint("MSIAfterburner ReadFromFile", "Error. File not found: " + FileName);
                            return new ControlMemoryGpuEntry();
                        }
                    }
                }
            }
            catch (Exception ex)
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
            } while (repeatsab < 15);

            if (repeatsab >= 15)
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
