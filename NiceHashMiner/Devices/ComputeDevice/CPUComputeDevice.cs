using NiceHashMiner.Configs;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using NiceHashMiner.Utils;

namespace NiceHashMiner.Devices
{
    [Serializable]
    public class CpuComputeDevice : ComputeDevice
    {
        private static int cpuLoad = 0;
        private static float cpuTemp = -1;
        private static PerformanceCounter cpuCounter = new PerformanceCounter();
        private static ManagementObjectSearcher searcher_ThermalZoneInformation =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_PerfFormattedData_Counters_ThermalZoneInformation");

        public override float Load
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringCPU)
                {
                    return -1;
                }
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        return ComputeDeviceCPU.CpuReader.GetLoad();
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("Load", e.ToString());
                    }
                } else
                {
                    new Task(() => GetLoad()).Start();
                    return cpuLoad;
                }
                return -1;
            }
        }
        
        private static void GetLoad()
        {
            try
            {
                cpuCounter.CategoryName = "Processor";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";

                float cpu = cpuCounter.NextValue();
                Thread.Sleep(1000);
                cpuLoad = (int)cpuCounter.NextValue();
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("GetLoad", ex.ToString());
                var CMD = new Process
                {
                    StartInfo =
                            {
                                FileName = "lodctr.exe"
                            }
                };
                CMD.StartInfo.Arguments = "/r";
                CMD.StartInfo.UseShellExecute = false;
                CMD.StartInfo.CreateNoWindow = true;
                CMD.Start();
            }
        }
        private static void GetTemp()
        {
            try
            {
                foreach (ManagementObject queryObj in searcher_ThermalZoneInformation.Get())
                {
                    var temperature = Convert.ToDouble(queryObj["HighPrecisionTemperature"].ToString());
                    temperature = (temperature - 2732) / 10.0;
                    cpuTemp = (float)temperature;
                }
            }
            catch (Exception ex)
            {
                cpuTemp = -1;
            }
        }
        public override float Temp
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringCPU)
                {
                    return -1;
                }
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        return ComputeDeviceCPU.CpuReader.GetTemperaturesInCelsius();
                    }
                    catch (Exception)
                    {
                    }
                } else
                {
                    new Task(() => GetTemp()).Start();
                    return cpuTemp;
                }
                return -1;
            }
        }

        public override int FanSpeed
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringCPU)
                {
                    return -1;
                }
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        int fan = ComputeDeviceCPU.CpuReader.GetFan();
                        if (fan > 0) return Math.Min(5000 / ComputeDeviceCPU.CpuReader.GetFan() * 10, 100);//emulate
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("CPUDIAG", e.ToString());
                    }
                }
                return -1;
            }
        }

        public override int FanSpeedRPM
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringCPU)
                {
                    return -1;
                }
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        return ComputeDeviceCPU.CpuReader.GetFan();
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("CPUDIAG", e.ToString());
                    }
                }
                return -1;
            }
        }

        public override double PowerUsage
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringCPU)
                {
                    return -1;
                }
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        return ComputeDeviceCPU.CpuReader.GetPower();
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("CPUDIAG", e.ToString());
                    }
                }
                return -1;
            }
        }

        private static uint GetPower()
        {
            ManagementObjectSearcher s = new ManagementObjectSearcher("SELECT * FROM Win32_processor");

            ManagementObject management = s.Get().OfType<ManagementObject>().First();

            return Convert.ToUInt16(management["PowerManagementCapabilities"]);
        }

        public CpuComputeDevice(int id, string group, string name, int threads, ulong affinityMask, int cpuCount, bool monitorconnected = false)
            : base(id,
                name,
                true,
                DeviceGroupType.CPU,
                false,
                DeviceType.CPU,
                string.Format(International.GetText("ComputeDevice_Short_Name_CPU"), cpuCount),
                0, "", monitorconnected, false)
        {
            group = "";
            Threads = threads;
            AffinityMask = affinityMask;
            Uuid = GetUuid(ID, GroupNames.GetGroupName(DeviceGroupType, ID), Name, DeviceGroupType);
            CPUDevice cpu = TryCPUDevice();
            if (cpu != null)
            {
                NewUuid = cpu.UUID;
            }
            else NewUuid = "0";
            AlgorithmSettings = GroupAlgorithms.CreateForDeviceList(this);
            Index = ID; // Don't increment for CPU
        }
    }


}
