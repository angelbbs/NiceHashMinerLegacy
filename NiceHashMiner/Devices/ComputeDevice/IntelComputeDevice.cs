using ATI.ADL;
using LibreHardwareMonitor.Hardware;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
//using OpenHardwareMonitor.Hardware;
using System;
using System.Runtime.InteropServices;

namespace NiceHashMiner.Devices
{
    public class IntelComputeDevice : ComputeDevice
    {
        private readonly int _adapterIndex;

        
        private int FanSpeedInternal()
        {
            return -1;
        }
        private int FanSpeedInternal8()
        {
            return -1;
        }
        public override int FanSpeed //percent
        {
            get
            {
                return -1;
            }
        }

        private int FanSpeedRPMInternal()
        {
            return -1;
        }
        private int FanSpeedRPMInternal8()
        {
            return -1;
        }
        public override int FanSpeedRPM
        {
            get
            {
                return -1;
            }
        }

        private float TempInternal()
        {
            return -1;
        }
        private int TempInternalN()
        {
            return -1;
        }
        private int TempInternal8()
        {
            return -1;
        }


        public override float Temp
        {
            get
            {
                return -1;
            }
        }

        private int TempMemoryInternal()
        {
            return -1;
        }
        private int TempMemoryInternal8()
        {
            return -2;
        }
        public override float TempMemory
        {
            get
            {
                return -1;
            }
        }

        private int LoadInternal()
        {
            return -1;
        }
        private int LoadInternal8()
        {
            return -1;
        }
        public override float Load
        {
            get
            {
                return -1;
            }
        }

        private int MemLoadInternal()
        {
            return 0;
        }
        public override float MemLoad
        {
            get
            {
                return MemLoadInternal();
            }
        }

        private double PowerUsageInternal()
        {
            return -1;
        }
        private double PowerUsageInternal8()
        {
            return -1;
        }
        public override double PowerUsage
        {
            get
            {
                return -1;
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
            Index = ID + ComputeDeviceManager.Available.AvailCpus + ComputeDeviceManager.Available.AvailNVGpus;
            _adapterIndex = intelDevice.AdapterIndex;
        }
    }

}
