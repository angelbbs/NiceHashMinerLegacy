using NiceHashMiner.Configs;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Devices
{
    [Serializable]
    public class CpuComputeDevice : ComputeDevice
    {
        //private readonly PerformanceCounter _cpuCounter;
        private static int cpuLoad = 0;
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
                        // if (_cpuCounter != null) return _cpuCounter.NextValue();
                        return ComputeDeviceCPU.CpuReader.GetLoad();
                    }
                    catch (Exception)
                    {
                        //    Helpers.ConsolePrint("CPUDIAG", e.ToString());
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
            PerformanceCounter cpuCounter;
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";


            float cpu = cpuCounter.NextValue();
            Thread.Sleep(1000);
            cpuLoad = (int)cpuCounter.NextValue();
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
                        //    Helpers.ConsolePrint("CPUDIAG", e.ToString());
                    }
                } else
                {
                    ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_PerfFormattedData_Counters_ThermalZoneInformation");

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        Double temperature = Convert.ToDouble(queryObj["HighPrecisionTemperature"].ToString());
                        temperature = (temperature - 2732) / 10.0;
                        return (float)temperature;
                    }
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
            /*
            _cpuCounter = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };
            */
        }
    }
}
