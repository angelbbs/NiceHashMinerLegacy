using System;
using System.Diagnostics;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Devices
{
    public class CpuComputeDevice : ComputeDevice
    {
        //private readonly PerformanceCounter _cpuCounter;

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
                    catch (Exception e)
                    {
                        //    Helpers.ConsolePrint("CPUDIAG", e.ToString());
                    }
                }
                return -1;
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
                    catch (Exception e)
                    {
                        //    Helpers.ConsolePrint("CPUDIAG", e.ToString());
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

        public CpuComputeDevice(int id, string group, string name, int threads, ulong affinityMask, int cpuCount)
            : base(id,
                name,
                true,
                DeviceGroupType.CPU,
                false,
                DeviceType.CPU,
                string.Format(International.GetText("ComputeDevice_Short_Name_CPU"), cpuCount),
                0, "")
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
