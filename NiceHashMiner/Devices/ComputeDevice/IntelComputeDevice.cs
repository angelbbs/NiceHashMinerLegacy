using ATI.ADL;
using LibreHardwareMonitor.Hardware;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
//using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NiceHashMiner.Devices
{
    public class IntelComputeDevice : ComputeDevice
    {
        private readonly int _adapterIndex;
        private readonly long _adapterHandle;

        public override int FanSpeed //percent
        {
            get
            {
                try
                {
                    if (ConfigManager.GeneralConfig.DisableMonitoringINTEL) return -1;
                    return IntelDevicesList.Single(p => p.DeviceHandle == _adapterHandle).FanSpeed;
                } catch (Exception ex)
                {
                    return -1;
                }
            }
        }
        public override int FanSpeedRPM
        {
            get
            {
                try
                {
                    if (ConfigManager.GeneralConfig.DisableMonitoringINTEL) return -1;
                    return IntelDevicesList.Single(p => p.DeviceHandle == _adapterHandle).FanSpeedRPM;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        public override float Temp
        {
            get
            {
                try
                {
                    if (ConfigManager.GeneralConfig.DisableMonitoringINTEL) return -1;
                    return IntelDevicesList.Single(p => p.DeviceHandle == _adapterHandle).Temp;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        public override float TempMemory
        {
            get
            {
                try
                {
                    if (ConfigManager.GeneralConfig.DisableMonitoringINTEL) return -1;
                    return IntelDevicesList.Single(p => p.DeviceHandle == _adapterHandle).TempMemory;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        public override float Load
        {
            get
            {
                try
                {
                    if (ConfigManager.GeneralConfig.DisableMonitoringINTEL) return -1;
                    return IntelDevicesList.Single(p => p.DeviceHandle == _adapterHandle).Load;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        public override float MemLoad
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringINTEL) return -1;
                return -1;
            }
        }

        public override double PowerUsage
        {
            get
            {
                try
                {
                    if (ConfigManager.GeneralConfig.DisableMonitoringINTEL) return -1;
                    return IntelDevicesList.Single(p => p.DeviceHandle == _adapterHandle).PowerUsage;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }

        private class IntelDev
        {
            public long DeviceHandle;
            public float Load;
            public float MemLoad;
            public float Temp;
            public float TempMemory;
            public int FanSpeed;
            public int FanSpeedRPM;
            public double PowerUsage;
        }
        private static List<IntelDev> IntelDevicesList;
        public static void IntelDevicesListInit(List<OpenCLDevice> oclList)
        {
            IntelDevicesList = new List<IntelDev>();
            foreach (var dev in oclList)
            {
                IntelDev _dev = new IntelDev();
                _dev.DeviceHandle = dev.DeviceHandle;
                IntelDevicesList.Add(_dev);
            }
        }
        public static void SetTelemetry()
        {
            if (IntelDevicesList == null) return;
            foreach (IntelDev dev in IntelDevicesList)
            {
                dev.FanSpeed = Querying.IntelQuery.GetFan(dev.DeviceHandle, true);
                dev.FanSpeedRPM = Querying.IntelQuery.GetFan(dev.DeviceHandle, false);
                dev.Load = Querying.IntelQuery.GetLoad(dev.DeviceHandle);
                dev.MemLoad = Querying.IntelQuery.GetLoad(dev.DeviceHandle);
                dev.PowerUsage = Querying.IntelQuery.GetPower(dev.DeviceHandle);
                dev.Temp = (float)Querying.IntelQuery.GetTemperature(dev.DeviceHandle, false);
                dev.TempMemory = (float)Querying.IntelQuery.GetTemperature(dev.DeviceHandle, true);
            }
        }

        public IntelComputeDevice(IntelGpuDevice intelDevice, int gpuCount, bool isDetectionFallback, int adl2Index)
            : base(intelDevice.DeviceID,
                intelDevice.DeviceName,
                true,
                DeviceGroupType.INTEL_OpenCL,
                intelDevice.IsEtherumCapable(),
                DeviceType.INTEL,
                string.Format(International.GetText("ComputeDevice_Short_Name_AMD_GPU"), gpuCount),
                intelDevice.DeviceGlobalMemory, intelDevice.IntelManufacturer, intelDevice.MonitorConnected, false)
        {
            Uuid = isDetectionFallback
                ? GetUuid(ID, GroupNames.GetGroupName(DeviceGroupType, ID), Name, DeviceGroupType)
                : intelDevice.UUID;
            BusID = intelDevice.BusID;
            Codename = intelDevice.Codename;
            InfSection = intelDevice.InfSection;
            AlgorithmSettings = GroupAlgorithms.CreateForDeviceList(this);
            DriverDisableAlgos = intelDevice.DriverDisableAlgos;
            Index = ID + ComputeDeviceManager.Available.AvailCpus + ComputeDeviceManager.Available.AvailNVGpus + ComputeDeviceManager.Available.AvailAmdGpus + ComputeDeviceManager.Available.AvailIntelGpus;
            _adapterIndex = intelDevice.AdapterIndex;
            _adapterHandle = intelDevice.DeviceHandle;
        }
    }

}
