/*
MIT License

Copyright (c) 2023 angelbbs

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace IGCL
{

    public class IGCL
    {
        const int CTL_MAX_DEVICE_NAME_LEN = 100;
        const int CTL_MAX_RESERVED_SIZE = 116;
        const int CTL_FAN_COUNT = 5;
        const int CTL_PSU_COUNT = 5;

        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_init_args_t
        {
            public uint Size;                                  ///< [in] size of this structure
            public uint Version;                                ///< [in] version of this structure
            public uint AppVersion;                  ///< [in][release] App's IGCL version
            public uint flags;                         ///< [in][release] Caller version
            public uint SupportedVersion;            ///< [out][release] IGCL implementation version
            //public byte[] ApplicationUID = new byte[16];            ///< [in] Application Provided Unique ID.Application can pass all 0's as
            public uint ApplicationUID_Data1;                                 ///< [in] Data1
            public ushort ApplicationUID_Data2;                                 ///< [in] Data2
            public ushort ApplicationUID_Data3;                                 ///< [in] Data3
            public uint ApplicationUID_Data40;                               ///< [in] Data4
            public uint ApplicationUID_Data41;                               ///< [in] Data4
                                                                             ///< the default ID
        }
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_application_id_t
        {
            public uint Data1;                                 ///< [in] Data1
            public ushort Data2;                                 ///< [in] Data2
            public ushort Data3;                                 ///< [in] Data3
            //public fixed byte Data4[8];                               ///< [in] Data4
            public uint Data40;                               ///< [in] Data4
            public uint Data41;                               ///< [in] Data4

        }

        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public unsafe struct ctl_device_adapter_properties_t
        {
            public int Size;                                  ///< [in] size of this structure
            public uint Version;                                ///< [in] version of this structure
            public IntPtr pDeviceID;                                ///< [in,out] OS specific Device ID
            public int device_id_size;                        ///< [in] size of the device ID
            public ctl_device_type_t device_type;                  ///< [out] Device Type
            public uint supported_subfunction_flags;///< [out] Supported functions
            public long driver_version;                        ///< [out] Driver version
            public ctl_firmware_version_t firmware_version;        ///< [out] Firmware version
            public uint pci_vendor_id;                         ///< [out] PCI Vendor ID
            public uint pci_device_id;                         ///< [out] PCI Device ID
            public uint rev_id;                                ///< [out] PCI Revision ID
            public uint num_eus_per_sub_slice;                 ///< [out] Number of EUs per sub-slice
            public uint num_sub_slices_per_slice;              ///< [out] Number of sub-slices per slice
            public uint num_slices;                            ///< [out] Number of slices
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CTL_MAX_DEVICE_NAME_LEN)]
            public char[] name;             ///< [out] Device name
            public uint graphics_adapter_properties; ///< [out] Graphics Adapter Properties
            public uint Frequency;                             ///< [out] Clock frequency for this device. Supported only for Version > 0
            public ushort pci_subsys_id;                         ///< [out] PCI SubSys ID, Supported only for Version > 1
            public ushort pci_subsys_vendor_id;                  ///< [out] PCI SubSys Vendor ID, Supported only for Version > 1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CTL_MAX_RESERVED_SIZE)]
            public char[] reserved;           ///< [out] Reserved
        }
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct Luid
        {
            public uint LowPart;
            public int HighPart;
        }

        [Serializable]
        public enum ctl_device_type_t
        {
            CTL_DEVICE_TYPE_GRAPHICS = 1,                   ///< Graphics Device type
            CTL_DEVICE_TYPE_SYSTEM = 2,                     ///< System Device type
            CTL_DEVICE_TYPE_MAX
        }
        
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_firmware_version_t
        {
            public long major_version;                         ///< [out] Major version
            public long minor_version;                         ///< [out] Minor version
            public long build_number;                          ///< [out] Build number

        }

        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_pci_properties_t
        {
            public int Size;                                  ///< [in] size of this structure
            public uint Version;                                ///< [in] version of this structure
            public ctl_pci_address_t address;                      ///< [out] The BDF address
            public ctl_pci_speed_t maxSpeed;                       ///< [out] Fastest port configuration supported by the device (sum of all
                                                                   ///< lanes)
            public bool resizable_bar_supported;                   ///< [out] Support for Resizable Bar on this device.
            public bool resizable_bar_enabled;                     ///< [out] Resizable Bar enabled on this device
        }
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_pci_address_t
        {
            public uint Size;                                  ///< [in] size of this structure
            public uint Version;                                ///< [in] version of this structure
            public uint domain;                                ///< [out] BDF domain
            public int bus;                                   ///< [out] BDF bus
            public uint device;                                ///< [out] BDF device
            public uint function;                              ///< [out] BDF function
        }
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_pci_speed_t
        {
            public uint Size;                                  ///< [in] size of this structure
            public uint Version;                                ///< [in] version of this structure
            public uint gen;                                    ///< [out] The link generation. A value of -1 means that this property is
                                                                ///< unknown.
            public uint width;                                  ///< [out] The number of lanes. A value of -1 means that this property is
                                                                ///< unknown.
            public long maxBandwidth;                           ///< [out] The maximum bandwidth in bytes/sec (sum of all lanes). A value
                                                            ///< of -1 means that this property is unknown.
        }

        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_mem_properties_t
        {
            public int Size;                                  ///< [in] size of this structure
            public uint Version;                                ///< [in] version of this structure
            public ctl_mem_type_t type;                            ///< [out] The memory type
            public ctl_mem_loc_t location;                         ///< [out] Location of this memory (system, device)
            public ulong physicalSize;                          ///< [out] Physical memory size in bytes. A value of 0 indicates that this
                                                               ///< property is not known. However, a call to ::ctlMemoryGetState() will
                                                               ///< correctly return the total size of usable memory.
            public int busWidth;                               ///< [out] Width of the memory bus. A value of -1 means that this property
                                                               ///< is unknown.
            public int numChannels;                            ///< [out] The number of memory channels. A value of -1 means that this
                                                            ///< property is unknown.
        }

        [Serializable]
        public enum ctl_mem_type_t
        {
            CTL_MEM_TYPE_HBM = 0,                           ///< HBM memory
            CTL_MEM_TYPE_DDR = 1,                           ///< DDR memory
            CTL_MEM_TYPE_DDR3 = 2,                          ///< DDR3 memory
            CTL_MEM_TYPE_DDR4 = 3,                          ///< DDR4 memory
            CTL_MEM_TYPE_DDR5 = 4,                          ///< DDR5 memory
            CTL_MEM_TYPE_LPDDR = 5,                         ///< LPDDR memory
            CTL_MEM_TYPE_LPDDR3 = 6,                        ///< LPDDR3 memory
            CTL_MEM_TYPE_LPDDR4 = 7,                        ///< LPDDR4 memory
            CTL_MEM_TYPE_LPDDR5 = 8,                        ///< LPDDR5 memory
            CTL_MEM_TYPE_GDDR4 = 9,                         ///< GDDR4 memory
            CTL_MEM_TYPE_GDDR5 = 10,                        ///< GDDR5 memory
            CTL_MEM_TYPE_GDDR5X = 11,                       ///< GDDR5X memory
            CTL_MEM_TYPE_GDDR6 = 12,                        ///< GDDR6 memory
            CTL_MEM_TYPE_GDDR6X = 13,                       ///< GDDR6X memory
            CTL_MEM_TYPE_GDDR7 = 14,                        ///< GDDR7 memory
            CTL_MEM_TYPE_UNKNOWN = 15,                      ///< UNKNOWN memory
            CTL_MEM_TYPE_MAX
        }

        [Serializable]
        public enum ctl_mem_loc_t
        {
            CTL_MEM_LOC_SYSTEM = 0,                         ///< System memory
            CTL_MEM_LOC_DEVICE = 1,                         ///< On board local device memory
            CTL_MEM_LOC_MAX
        }

        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_temp_properties_t
        {
            public int Size;                                  ///< [in] size of this structure
            public uint Version;                                ///< [in] version of this structure
            public ctl_temp_sensors_t type;                        ///< [out] Which part of the device the temperature sensor measures
            public double maxTemperature;                          ///< [out] Will contain the maximum temperature for the specific device in
                                                            ///< degrees Celsius.
        }
        [Serializable]
        public enum ctl_temp_sensors_t
        {
            CTL_TEMP_SENSORS_GLOBAL = 0,                    ///< The maximum temperature across all device sensors
            CTL_TEMP_SENSORS_GPU = 1,                       ///< The maximum temperature across all sensors in the GPU
            CTL_TEMP_SENSORS_MEMORY = 2,                    ///< The maximum temperature across all sensors in the local memory
            CTL_TEMP_SENSORS_GLOBAL_MIN = 3,                ///< The minimum temperature across all device sensors
            CTL_TEMP_SENSORS_GPU_MIN = 4,                   ///< The minimum temperature across all sensors in the GPU
            CTL_TEMP_SENSORS_MEMORY_MIN = 5,                ///< The minimum temperature across all sensors in the local device memory
            CTL_TEMP_SENSORS_MAX
        }

        [StructLayout(LayoutKind.Explicit)]
        [Serializable]
        public unsafe struct _ctl_power_telemetry_t
        {
            [FieldOffset(0)]
            public int Size;                                  ///< [in] size of this structure
            [FieldOffset(4)]
            public uint Version;                                ///< [in] version of this structure
            [FieldOffset(8)]
            public fixed byte body[800];
        }

        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct ctl_power_telemetry_t
        {
            public uint Size;                                  ///< [in] size of this structure
            public uint Version;                                ///< [in] version of this structure
            public ctl_oc_telemetry_item_t timeStamp;              ///< [out] Snapshot of the timestamp counter that measures the total time
                                                                   ///< since Jan 1, 1970 UTC. It is a decimal value in seconds with a minimum
                                                                   ///< accuracy of 1 millisecond.
            public ctl_oc_telemetry_item_t gpuEnergyCounter;       ///< [out] Snapshot of the monotonic energy counter maintained by hardware.
                                                                   ///< It measures the total energy consumed by the GPU chip. By taking the
                                                                   ///< delta between two snapshots and dividing by the delta time in seconds,
                                                                   ///< an application can compute the average power.
            public ctl_oc_telemetry_item_t gpuVoltage;             ///< [out] Instantaneous snapshot of the voltage feeding the GPU chip. It
                                                                   ///< is measured at the power supply output - chip input will be lower.
            public ctl_oc_telemetry_item_t gpuCurrentClockFrequency;   ///< [out] Instantaneous snapshot of the GPU chip frequency.
            public ctl_oc_telemetry_item_t gpuCurrentTemperature;  ///< [out] Instantaneous snapshot of the GPU chip temperature, read from
                                                                   ///< the sensor reporting the highest value.
            public ctl_oc_telemetry_item_t globalActivityCounter;  ///< [out] Snapshot of the monotonic global activity counter. It measures
                                                                   ///< the time in seconds (accurate down to 1 millisecond) that any GPU
                                                                   ///< engine is busy. By taking the delta between two snapshots and dividing
                                                                   ///< by the delta time in seconds, an application can compute the average
                                                                   ///< percentage utilization of the GPU..
            public ctl_oc_telemetry_item_t renderComputeActivityCounter;   ///< [out] Snapshot of the monotonic 3D/compute activity counter. It
                                                                           ///< measures the time in seconds (accurate down to 1 millisecond) that any
                                                                           ///< 3D render/compute engine is busy. By taking the delta between two
                                                                           ///< snapshots and dividing by the delta time in seconds, an application
                                                                           ///< can compute the average percentage utilization of all 3D
                                                                           ///< render/compute blocks in the GPU.
            public ctl_oc_telemetry_item_t mediaActivityCounter;   ///< [out] Snapshot of the monotonic media activity counter. It measures
                                                                   ///< the time in seconds (accurate down to 1 millisecond) that any media
                                                                   ///< engine is busy. By taking the delta between two snapshots and dividing
                                                                   ///< by the delta time in seconds, an application can compute the average
                                                                   ///< percentage utilization of all media blocks in the GPU.
            //wrong struct?
            public bool gpuPowerLimited;                           ///< [out] Instantaneous indication that the desired GPU frequency is being
                                                                   ///< throttled because the GPU chip is exceeding the maximum power limits.
                                                                   ///< Increasing the power limits using ::ctlOverclockPowerLimitSet() is one
                                                                   ///< way to remove this limitation.
            public bool gpuTemperatureLimited;                     ///< [out] Instantaneous indication that the desired GPU frequency is being
                                                                   ///< throttled because the GPU chip is exceeding the temperature limits.
                                                                   ///< Increasing the temperature limits using
                                                                   ///< ::ctlOverclockTemperatureLimitSet() is one way to reduce this
                                                                   ///< limitation. Improving the cooling solution is another way.
            public bool gpuCurrentLimited;                         ///< [out] Instantaneous indication that the desired GPU frequency is being
                                                                   ///< throttled because the GPU chip has exceeded the power supply current
                                                                   ///< limits. A better power supply is required to reduce this limitation.
            public bool gpuVoltageLimited;                         ///< [out] Instantaneous indication that the GPU frequency cannot be
                                                                   ///< increased because the voltage limits have been reached. Increase the
                                                                   ///< voltage offset using ::ctlOverclockGpuVoltageOffsetSet() is one way to
                                                                   ///< reduce this limitation.
            public bool gpuUtilizationLimited;                     ///< [out] Instantaneous indication that due to lower GPU utilization, the
                                                                   ///< hardware has lowered the GPU frequency.
            public ctl_oc_telemetry_item_t vramEnergyCounter;      ///< [out] Snapshot of the monotonic energy counter maintained by hardware.
                                                                   ///< It measures the total energy consumed by the local memory modules. By
                                                                   ///< taking the delta between two snapshots and dividing by the delta time
                                                                   ///< in seconds, an application can compute the average power.
            public ctl_oc_telemetry_item_t vramVoltage;            ///< [out] Instantaneous snapshot of the voltage feeding the memory
                                                                   ///< modules.
            public ctl_oc_telemetry_item_t vramCurrentClockFrequency;  ///< [out] Instantaneous snapshot of the raw clock frequency driving the
                                                                       ///< memory modules.
            public ctl_oc_telemetry_item_t vramCurrentEffectiveFrequency;  ///< [out] Instantaneous snapshot of the effective data transfer rate that
                                                                           ///< the memory modules can sustain based on the current clock frequency..
            public ctl_oc_telemetry_item_t vramReadBandwidthCounter;   ///< [out] Instantaneous snapshot of the monotonic counter that measures
                                                                       ///< the read traffic from the memory modules. By taking the delta between
                                                                       ///< two snapshots and dividing by the delta time in seconds, an
                                                                       ///< application can compute the average read bandwidth.
            public ctl_oc_telemetry_item_t vramWriteBandwidthCounter;  ///< [out] Instantaneous snapshot of the monotonic counter that measures
                                                                       ///< the write traffic to the memory modules. By taking the delta between
                                                                       ///< two snapshots and dividing by the delta time in seconds, an
                                                                       ///< application can compute the average write bandwidth.
            public ctl_oc_telemetry_item_t vramCurrentTemperature; ///< [out] Instantaneous snapshot of the GPU chip temperature, read from
                                                                   ///< the sensor reporting the highest value.
            public bool vramPowerLimited;                          ///< [out] Instantaneous indication that the memory frequency is being
                                                                   ///< throttled because the memory modules are exceeding the maximum power
                                                                   ///< limits.
            public bool vramTemperatureLimited;                    ///< [out] Instantaneous indication that the memory frequency is being
                                                                   ///< throttled because the memory modules are exceeding the temperature
                                                                   ///< limits.
            public bool vramCurrentLimited;                        ///< [out] Instantaneous indication that the memory frequency is being
                                                                   ///< throttled because the memory modules have exceeded the power supply
                                                                   ///< current limits.
            public bool vramVoltageLimited;                        ///< [out] Instantaneous indication that the memory frequency cannot be
                                                                   ///< increased because the voltage limits have been reached.
            public bool vramUtilizationLimited;                    ///< [out] Instantaneous indication that due to lower memory traffic, the
                                                                   ///< hardware has lowered the memory frequency.
            public ctl_oc_telemetry_item_t totalCardEnergyCounter; ///< [out] Total Card Energy Counter.
            //ctl_psu_info_t psu[CTL_PSU_COUNT];              ///< [out] PSU voltage and power.
            //ctl_oc_telemetry_item_t fanSpeed[CTL_FAN_COUNT];///< [out] Fan speed.
            public ctl_psu_info_t psu0;              ///< [out] PSU voltage and power.
            public ctl_psu_info_t psu1;              ///< [out] PSU voltage and power.
            public ctl_psu_info_t psu2;              ///< [out] PSU voltage and power.
            public ctl_psu_info_t psu3;              ///< [out] PSU voltage and power.
            public ctl_psu_info_t psu4;              ///< [out] PSU voltage and power.

            public ctl_oc_telemetry_item_t fanSpeed0;///< [out] Fan speed.
            public ctl_oc_telemetry_item_t fanSpeed1;///< [out] Fan speed.
            public ctl_oc_telemetry_item_t fanSpeed2;///< [out] Fan speed.
            public ctl_oc_telemetry_item_t fanSpeed3;///< [out] Fan speed.
            //public ctl_oc_telemetry_item_t fanSpeed4;///< [out] Fan speed.
        }


        [StructLayout(LayoutKind.Explicit, Size = 808)]
        [Serializable]
        public struct ctl_power_telemetry_t0
        {
            [FieldOffset(0)]
            public int Size;                                  ///< [in] size of this structure
            [FieldOffset(4)]
            public uint Version;                                ///< [in] version of this structure
            [FieldOffset(8)]
            public ctl_oc_telemetry_item_t timeStamp;              ///< [out] Snapshot of the timestamp counter that measures the total time
                                                                   ///< since Jan 1, 1970 UTC. It is a decimal value in seconds with a minimum
                                                                   ///< accuracy of 1 millisecond.
            [FieldOffset(32)]
            public ctl_oc_telemetry_item_t gpuEnergyCounter;       ///< [out] Snapshot of the monotonic energy counter maintained by hardware.
                                                                   ///< It measures the total energy consumed by the GPU chip. By taking the
                                                                   ///< delta between two snapshots and dividing by the delta time in seconds,
                                                                   ///< an application can compute the average power.
            [FieldOffset(56)]
            public ctl_oc_telemetry_item_t gpuVoltage;             ///< [out] Instantaneous snapshot of the voltage feeding the GPU chip. It
                                                                   ///< is measured at the power supply output - chip input will be lower.
            [FieldOffset(80)]
            public ctl_oc_telemetry_item_t gpuCurrentClockFrequency;   ///< [out] Instantaneous snapshot of the GPU chip frequency.
            [FieldOffset(104)]
            public ctl_oc_telemetry_item_t gpuCurrentTemperature;  ///< [out] Instantaneous snapshot of the GPU chip temperature, read from
                                                                   ///< the sensor reporting the highest value.
            [FieldOffset(128)]
            public ctl_oc_telemetry_item_t globalActivityCounter;  ///< [out] Snapshot of the monotonic global activity counter. It measures
                                                                   ///< the time in seconds (accurate down to 1 millisecond) that any GPU
                                                                   ///< engine is busy. By taking the delta between two snapshots and dividing
                                                                   ///< by the delta time in seconds, an application can compute the average
                                                                   ///< percentage utilization of the GPU..
            [FieldOffset(152)]
            public ctl_oc_telemetry_item_t renderComputeActivityCounter;   ///< [out] Snapshot of the monotonic 3D/compute activity counter. It
                                                                           ///< measures the time in seconds (accurate down to 1 millisecond) that any
                                                                           ///< 3D render/compute engine is busy. By taking the delta between two
                                                                           ///< snapshots and dividing by the delta time in seconds, an application
                                                                           ///< can compute the average percentage utilization of all 3D
                                                                           ///< render/compute blocks in the GPU.
            [FieldOffset(176)]
            public ctl_oc_telemetry_item_t mediaActivityCounter;   ///< [out] Snapshot of the monotonic media activity counter. It measures
                                                                   ///< the time in seconds (accurate down to 1 millisecond) that any media
                                                                   ///< engine is busy. By taking the delta between two snapshots and dividing
                                                                   ///< by the delta time in seconds, an application can compute the average
            /*                                                       ///< percentage utilization of all media blocks in the GPU.
            [FieldOffset(200)]
            public bool gpuPowerLimited;                           ///< [out] Instantaneous indication that the desired GPU frequency is being
                                                                   ///< throttled because the GPU chip is exceeding the maximum power limits.
                                                                   ///< Increasing the power limits using ::ctlOverclockPowerLimitSet() is one
                                                                   ///< way to remove this limitation.
            [FieldOffset(204)]
            public bool gpuTemperatureLimited;                     ///< [out] Instantaneous indication that the desired GPU frequency is being
                                                                   ///< throttled because the GPU chip is exceeding the temperature limits.
                                                                   ///< Increasing the temperature limits using
                                                                   ///< ::ctlOverclockTemperatureLimitSet() is one way to reduce this
                                                                   ///< limitation. Improving the cooling solution is another way.
            [FieldOffset(208)]
            public bool gpuCurrentLimited;                         ///< [out] Instantaneous indication that the desired GPU frequency is being
                                                                   ///< throttled because the GPU chip has exceeded the power supply current
                                                                   ///< limits. A better power supply is required to reduce this limitation.
            [FieldOffset(212)]
            public bool gpuVoltageLimited;                         ///< [out] Instantaneous indication that the GPU frequency cannot be
                                                                   ///< increased because the voltage limits have been reached. Increase the
                                                                   ///< voltage offset using ::ctlOverclockGpuVoltageOffsetSet() is one way to
                                                                   ///< reduce this limitation.
            [FieldOffset(216)]
            public bool gpuUtilizationLimited;                     ///< [out] Instantaneous indication that due to lower GPU utilization, the
                                                                   ///< hardware has lowered the GPU frequency.
*/
            [FieldOffset(200)]
            public bool bool1;
            [FieldOffset(204)]
            public bool bool2;


            [FieldOffset(208)]
            public ctl_oc_telemetry_item_t vramEnergyCounter;      ///< [out] Snapshot of the monotonic energy counter maintained by hardware.
                                                                   ///< It measures the total energy consumed by the local memory modules. By
                                                                   ///< taking the delta between two snapshots and dividing by the delta time
                                                                   ///< in seconds, an application can compute the average power.
            [FieldOffset(232)]
            public ctl_oc_telemetry_item_t vramVoltage;            ///< [out] Instantaneous snapshot of the voltage feeding the memory
                                                                   ///< modules.
            [FieldOffset(256)]
            public ctl_oc_telemetry_item_t vramCurrentClockFrequency;  ///< [out] Instantaneous snapshot of the raw clock frequency driving the
                                                                       ///< memory modules.
            [FieldOffset(280)]
            public ctl_oc_telemetry_item_t vramCurrentEffectiveFrequency;  ///< [out] Instantaneous snapshot of the effective data transfer rate that
                                                                           ///< the memory modules can sustain based on the current clock frequency..
            [FieldOffset(304)]
            public ctl_oc_telemetry_item_t vramReadBandwidthCounter;   ///< [out] Instantaneous snapshot of the monotonic counter that measures
                                                                       ///< the read traffic from the memory modules. By taking the delta between
                                                                       ///< two snapshots and dividing by the delta time in seconds, an
                                                                       ///< application can compute the average read bandwidth.
            [FieldOffset(328)]
            public ctl_oc_telemetry_item_t vramWriteBandwidthCounter;  ///< [out] Instantaneous snapshot of the monotonic counter that measures
                                                                       ///< the write traffic to the memory modules. By taking the delta between
                                                                       ///< two snapshots and dividing by the delta time in seconds, an
                                                                       ///< application can compute the average write bandwidth.
            [FieldOffset(352)]
            public ctl_oc_telemetry_item_t vramCurrentTemperature; ///< [out] Instantaneous snapshot of the GPU chip temperature, read from
            /*                                                      ///< the sensor reporting the highest value.
           [FieldOffset(396)]
           public bool vramPowerLimited;                          ///< [out] Instantaneous indication that the memory frequency is being
                                                                  ///< throttled because the memory modules are exceeding the maximum power
                                                                  ///< limits.
           [FieldOffset(392)]
           public bool vramTemperatureLimited;                    ///< [out] Instantaneous indication that the memory frequency is being
                                                                  ///< throttled because the memory modules are exceeding the temperature
                                                                  ///< limits.
           [FieldOffset(396)]
           public bool vramCurrentLimited;                        ///< [out] Instantaneous indication that the memory frequency is being
                                                                  ///< throttled because the memory modules have exceeded the power supply
                                                                  ///< current limits.
           [FieldOffset(400)]
           public bool vramVoltageLimited;                        ///< [out] Instantaneous indication that the memory frequency cannot be
                                                                  ///< increased because the voltage limits have been reached.
           [FieldOffset(404)]
           public bool vramUtilizationLimited;                    ///< [out] Instantaneous indication that due to lower memory traffic, the
                                                                  ///< hardware has lowered the memory frequency.
            */
            [FieldOffset(376)]
            public bool bool3;
            [FieldOffset(380)]
            public bool bool4;

            [FieldOffset(384)]
            public ctl_oc_telemetry_item_t totalCardEnergyCounter; ///< [out] Total Card Energy Counter.

            //[FieldOffset(408)]
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = CTL_PSU_COUNT)]
            //public ctl_psu_info_t[] psu;              ///< [out] PSU voltage and power. //280
            
            [FieldOffset(408)]
            ctl_psu_info_t psu0;              ///< [out] PSU voltage and power.
            [FieldOffset(464)]
            ctl_psu_info_t psu1;              ///< [out] PSU voltage and power.
            [FieldOffset(520)]
            ctl_psu_info_t psu2;              ///< [out] PSU voltage and power.
            [FieldOffset(576)]
            ctl_psu_info_t psu3;              ///< [out] PSU voltage and power.
            [FieldOffset(632)]
            ctl_psu_info_t psu4;              ///< [out] PSU voltage and power.

            //[FieldOffset(688)]
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = CTL_FAN_COUNT)]
            //public ctl_oc_telemetry_item_t[] fanSpeed;///< [out] Fan speed.//120

            [FieldOffset(688)]
            public ctl_oc_telemetry_item_t fanSpeed0;///< [out] Fan speed.
            [FieldOffset(712)]
            public ctl_oc_telemetry_item_t fanSpeed1;///< [out] Fan speed.
            [FieldOffset(736)]
            public ctl_oc_telemetry_item_t fanSpeed2;///< [out] Fan speed.
            [FieldOffset(760)]
            public ctl_oc_telemetry_item_t fanSpeed3;///< [out] Fan speed.
            [FieldOffset(784)]
            public ctl_oc_telemetry_item_t fanSpeed4;///< [out] Fan speed.
            //808
        }
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        [Serializable]
        public struct ctl_oc_telemetry_item_t//24
        {
            [FieldOffset(0)]
            public bool bSupported;                                ///< [out] Indicates if the value is supported.
            [FieldOffset(4)]
            public ctl_units_t units;                              ///< [out] Indicates the units of the value.
            //public int units;                              ///< [out] Indicates the units of the value.
            [FieldOffset(8)]
            public ctl_data_type_t type;                           ///< [out] Indicates the data type.
            //public int type;                           ///< [out] Indicates the data type.
            [FieldOffset(12)]
            public ctl_data_value_t value;                         ///< [out] The value of type ::ctl_data_type_t and units ::ctl_units_t.
        }
        [Serializable]

        public enum ctl_units_t : int //4 ok
        {
            CTL_UNITS_FREQUENCY_MHZ = 0,                    ///< Type is Frequency with units in MHz.
            CTL_UNITS_OPERATIONS_GTS = 1,                   ///< Type is Frequency with units in GT/s (gigatransfers per second).
            CTL_UNITS_OPERATIONS_MTS = 2,                   ///< Type is Frequency with units in MT/s (megatransfers per second).
            CTL_UNITS_VOLTAGE_VOLTS = 3,                    ///< Type is Voltage with units in Volts.
            CTL_UNITS_POWER_WATTS = 4,                      ///< Type is Power with units in Watts.
            CTL_UNITS_TEMPERATURE_CELSIUS = 5,              ///< Type is Temperature with units in Celsius.
            CTL_UNITS_ENERGY_JOULES = 6,                    ///< Type is Energy with units in Joules.
            CTL_UNITS_TIME_SECONDS = 7,                     ///< Type is Time with units in Seconds.
            CTL_UNITS_MEMORY_BYTES = 8,                     ///< Type is Memory with units in Bytes.
            CTL_UNITS_ANGULAR_SPEED_RPM = 9,                ///< Type is Angular Speed with units in Revolutions per Minute.
            CTL_UNITS_UNKNOWN = 0x4800FFFF,                 ///< Type of units unknown.
            CTL_UNITS_MAX
        }
        [Serializable]
        public enum ctl_data_type_t : int//4 ok
        {
            CTL_DATA_TYPE_INT8 = 0,                         ///< The data type is 8 bit signed integer.
            CTL_DATA_TYPE_UINT8 = 1,                        ///< The data type is 8 bit unsigned integer.
            CTL_DATA_TYPE_INT16 = 2,                        ///< The data type is 16 bit signed integer.
            CTL_DATA_TYPE_UINT16 = 3,                       ///< The data type is 16 bit unsigned integer.
            CTL_DATA_TYPE_INT32 = 4,                        ///< The data type is 32 bit signed integer.
            CTL_DATA_TYPE_UINT32 = 5,                       ///< The data type is 32 bit unsigned integer.
            CTL_DATA_TYPE_INT64 = 6,                        ///< The data type is 64 bit signed integer.
            CTL_DATA_TYPE_UINT64 = 7,                       ///< The data type is 64 bit unsigned integer.
            CTL_DATA_TYPE_FLOAT = 8,                        ///< The data type is 32 bit floating point.
            CTL_DATA_TYPE_DOUBLE = 9,                       ///< The data type is 64 bit floating point.
            CTL_DATA_TYPE_STRING_ASCII = 10,                ///< The data type is an array of 8 bit unsigned integers.
            CTL_DATA_TYPE_STRING_UTF16 = 11,                ///< The data type is an array of 16 bit unsigned integers.
            CTL_DATA_TYPE_STRING_UTF132 = 12,               ///< The data type is an array of 32 bit unsigned integers.
            CTL_DATA_TYPE_UNKNOWN = 0x4800FFFF,             ///< The data type is unknown.
            CTL_DATA_TYPE_MAX
        }
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        [Serializable]
        public unsafe struct ctl_data_value_t//12 ok
        {
            [FieldOffset(0), MarshalAs(UnmanagedType.I1, SizeConst = 1)]
            public byte data8;                                   ///< [out] The data type is 8 bit signed integer.
            [FieldOffset(1), MarshalAs(UnmanagedType.U1, SizeConst = 1)]
            public byte datau8;                                 ///< [out] The data type is 8 bit unsigned integer.
            [FieldOffset(0), MarshalAs(UnmanagedType.I2, SizeConst = 2)]
            public short data16;                                 ///< [out] The data type is 16 bit signed integer.
            [FieldOffset(2), MarshalAs(UnmanagedType.U2, SizeConst = 2)]
            public ushort datau16;                               ///< [out] The data type is 16 bit unsigned integer.
            [FieldOffset(0), MarshalAs(UnmanagedType.I4, SizeConst = 4)]
            public int data32;                                 ///< [out] The data type is 32 bit signed integer.
            [FieldOffset(4), MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint datau32;                               ///< [out] The data type is 32 bit unsigned integer.
            [FieldOffset(0), MarshalAs(UnmanagedType.I8, SizeConst = 8)]
            public long data64;                                 ///< [out] The data type is 64 bit signed integer.
            [FieldOffset(4), MarshalAs(UnmanagedType.U8, SizeConst = 8)]
            public ulong datau64;                               ///< [out] The data type is 64 bit unsigned integer.
            [FieldOffset(0), MarshalAs(UnmanagedType.R4, SizeConst = 4)]
            public float datafloat;                                ///< [out] The data type is 32 bit floating point.
            [FieldOffset(4), MarshalAs(UnmanagedType.R8, SizeConst = 8)]
            public double datadouble;                              ///< [out] The data type is 64 bit floating point.
        }

        [StructLayout(LayoutKind.Explicit)]
        [Serializable]
        public struct ctl_psu_info_t//56 ok
        {
            [FieldOffset(0)]
            public bool bSupported;                                ///< [out] Indicates if this PSU entry is supported.
            [FieldOffset(4)]
            public ctl_psu_type_t psuType;                         ///< [out] Type of the PSU.
            [FieldOffset(8)]
            public ctl_oc_telemetry_item_t energyCounter;          ///< [out] Snapshot of the monotonic energy counter maintained by hardware.
                                                                   ///< It measures the total energy consumed this power source. By taking the
                                                                   ///< delta between two snapshots and dividing by the delta time in seconds,
                                                                   ///< an application can compute the average power.
            [FieldOffset(32)]
            public ctl_oc_telemetry_item_t voltage;                ///< [out] Instantaneous snapshot of the voltage of this power source.
        }
        [Serializable]
        public enum ctl_psu_type_t//4 ok
        {
            CTL_PSU_TYPE_PSU_NONE = 0,                      ///< Type of the PSU is unknown.
            CTL_PSU_TYPE_PSU_PCIE = 1,                      ///< Type of the PSU is PCIe
            CTL_PSU_TYPE_PSU_6PIN = 2,                      ///< Type of the PSU is 6 PIN
            CTL_PSU_TYPE_PSU_8PIN = 3,                      ///< Type of the PSU is 8 PIN
            CTL_PSU_TYPE_MAX
        }

        [Serializable]
        public enum ctl_fan_speed_units_t//4 ok
        {
            CTL_FAN_SPEED_UNITS_RPM = 0,                    ///< The fan speed is in units of revolutions per minute (rpm)
            CTL_FAN_SPEED_UNITS_PERCENT = 1,                ///< The fan speed is a percentage of the maximum speed of the fan
            CTL_FAN_SPEED_UNITS_MAX

        }



        public static uint CTL_IMPL_MAJOR_VERSION = 1;
        public static uint CTL_IMPL_MINOR_VERSION = 1;

        public static int _CRTDBG_ALLOC_MEM_DF = 0x01;
        public static int _CRTDBG_LEAK_CHECK_DF = 0x20;
        public static int _CRTDBG_MODE_DEBUG = 0x2;
        public static int _CRTDBG_MODE_WNDW = 0x4;
        public static uint CTL_INIT_FLAG_USE_LEVEL_ZERO => (uint)CTL_BIT(0);

        [DllImport("Kernel32.dll", EntryPoint = "RtlZeroMemory", SetLastError = false)]
        public static extern void ZeroMemory(IntPtr dest, IntPtr size);

        [DllImport("msvcr120d.dll", SetLastError = true)]
        public static extern int _CrtSetDbgFlag(int newFlag);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("ControlLib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern _ctl_result_t ctlInit(ref ctl_init_args_t CtlInitArgs, ref IntPtr hAPIHandle);
        [DllImport("ControlLib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern _ctl_result_t ctlClose(IntPtr hAPIHandle);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlEnumerateDevices(IntPtr hAPIHandle, ref uint Adapter_count, long[] hDevices);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlGetDeviceProperties(long hAPIHandle, ref ctl_device_adapter_properties_t StDeviceAdapterProperties);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlPciGetProperties(long hDAhandle, ref ctl_pci_properties_t Pci_properties);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlEnumMemoryModules(long hDAhandle, ref uint pCount, long[] phMemory);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlMemoryGetProperties(long hDAhandle, ref ctl_mem_properties_t memoryProperties);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlEnumTemperatureSensors(long hDAhandle, ref uint TemperatureHandlerCount, long[] pTtemperatureHandle);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlTemperatureGetProperties(long hTemperature, ref ctl_temp_properties_t pProperties);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlTemperatureGetState(long hTemperature, ref double temperature);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlPowerTelemetryGet(long hTemperature, ref ctl_power_telemetry_t pPowerTelemetry);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlEnumFans(long hDAhandle, ref uint FanHandlerCount, long[] pFanHandle);

        [DllImport("ControlLib.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern _ctl_result_t ctlFanGetState(long hFan, ctl_fan_speed_units_t units, ref int pSpeed);



        public static int CTL_BIT(int _i)
        {
            return 1 << _i;
        }
        public static uint CTL_MAKE_VERSION(uint _major, uint _minor )
        {
            return (_major << 16) | (_minor & 0x0000ffff);
        }
        
        public enum _ctl_result_t
        {
            CTL_RESULT_SUCCESS = 0x00000000,                ///< success
            CTL_RESULT_SUCCESS_STILL_OPEN_BY_ANOTHER_CALLER = 0x00000001,   ///< success but still open by another caller
            CTL_RESULT_ERROR_SUCCESS_END = 0x0000FFFF,      ///< "Success group error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_GENERIC_START = 0x40000000,    ///< Generic error code starting value, not to be used
            CTL_RESULT_ERROR_NOT_INITIALIZED = 0x40000001,  ///< Result not initialized
            CTL_RESULT_ERROR_ALREADY_INITIALIZED = 0x40000002,  ///< Already initialized
            CTL_RESULT_ERROR_DEVICE_LOST = 0x40000003,      ///< Device hung, reset, was removed, or driver update occurred
            CTL_RESULT_ERROR_OUT_OF_HOST_MEMORY = 0x40000004,   ///< Insufficient host memory to satisfy call
            CTL_RESULT_ERROR_OUT_OF_DEVICE_MEMORY = 0x40000005, ///< Insufficient device memory to satisfy call
            CTL_RESULT_ERROR_INSUFFICIENT_PERMISSIONS = 0x40000006, ///< Access denied due to permission level
            CTL_RESULT_ERROR_NOT_AVAILABLE = 0x40000007,    ///< Resource was removed
            CTL_RESULT_ERROR_UNINITIALIZED = 0x40000008,    ///< Library not initialized
            CTL_RESULT_ERROR_UNSUPPORTED_VERSION = 0x40000009,  ///< Generic error code for unsupported versions
            CTL_RESULT_ERROR_UNSUPPORTED_FEATURE = 0x4000000a,  ///< Generic error code for unsupported features
            CTL_RESULT_ERROR_INVALID_ARGUMENT = 0x4000000b, ///< Generic error code for invalid arguments
            CTL_RESULT_ERROR_INVALID_API_HANDLE = 0x4000000c,   ///< API handle in invalid
            CTL_RESULT_ERROR_INVALID_NULL_HANDLE = 0x4000000d,  ///< Handle argument is not valid
            CTL_RESULT_ERROR_INVALID_NULL_POINTER = 0x4000000e, ///< Pointer argument may not be nullptr
            CTL_RESULT_ERROR_INVALID_SIZE = 0x4000000f,     ///< Size argument is invalid (e.g., must not be zero)
            CTL_RESULT_ERROR_UNSUPPORTED_SIZE = 0x40000010, ///< Size argument is not supported by the device (e.g., too large)
            CTL_RESULT_ERROR_UNSUPPORTED_IMAGE_FORMAT = 0x40000011, ///< Image format is not supported by the device
            CTL_RESULT_ERROR_DATA_READ = 0x40000012,        ///< Data read error
            CTL_RESULT_ERROR_DATA_WRITE = 0x40000013,       ///< Data write error
            CTL_RESULT_ERROR_DATA_NOT_FOUND = 0x40000014,   ///< Data not found error
            CTL_RESULT_ERROR_NOT_IMPLEMENTED = 0x40000015,  ///< Function not implemented
            CTL_RESULT_ERROR_OS_CALL = 0x40000016,          ///< Operating system call failure
            CTL_RESULT_ERROR_KMD_CALL = 0x40000017,         ///< Kernel mode driver call failure
            CTL_RESULT_ERROR_UNLOAD = 0x40000018,           ///< Library unload failure
            CTL_RESULT_ERROR_ZE_LOADER = 0x40000019,        ///< Level0 loader not found
            CTL_RESULT_ERROR_INVALID_OPERATION_TYPE = 0x4000001a,   ///< Invalid operation type
            CTL_RESULT_ERROR_NULL_OS_INTERFACE = 0x4000001b,///< Null OS interface
            CTL_RESULT_ERROR_NULL_OS_ADAPATER_HANDLE = 0x4000001c,  ///< Null OS adapter handle
            CTL_RESULT_ERROR_NULL_OS_DISPLAY_OUTPUT_HANDLE = 0x4000001d,///< Null display output handle
            CTL_RESULT_ERROR_WAIT_TIMEOUT = 0x4000001e,     ///< Timeout in Wait function
            CTL_RESULT_ERROR_PERSISTANCE_NOT_SUPPORTED = 0x4000001f,///< Persistance not supported
            CTL_RESULT_ERROR_PLATFORM_NOT_SUPPORTED = 0x40000020,   ///< Platform not supported
            CTL_RESULT_ERROR_UNKNOWN_APPLICATION_UID = 0x40000021,  ///< Unknown Appplicaion UID in Initialization call
            CTL_RESULT_ERROR_INVALID_ENUMERATION = 0x40000022,  ///< The enum is not valid
            CTL_RESULT_ERROR_FILE_DELETE = 0x40000023,      ///< Error in file delete operation
            CTL_RESULT_ERROR_RESET_DEVICE_REQUIRED = 0x40000024,///< The device requires a reset.
            CTL_RESULT_ERROR_FULL_REBOOT_REQUIRED = 0x40000025, ///< The device requires a full reboot.
            CTL_RESULT_ERROR_LOAD = 0x40000026,             ///< Library load failure
            CTL_RESULT_ERROR_UNKNOWN = 0x4000FFFF,          ///< Unknown or internal error
            CTL_RESULT_ERROR_RETRY_OPERATION = 0x40010000,  ///< Operation failed, retry previous operation again
            CTL_RESULT_ERROR_GENERIC_END = 0x4000FFFF,      ///< "Generic error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_CORE_START = 0x44000000,       ///< Core error code starting value, not to be used
            CTL_RESULT_ERROR_CORE_OVERCLOCK_NOT_SUPPORTED = 0x44000001, ///< The Overclock is not supported.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_VOLTAGE_OUTSIDE_RANGE = 0x44000002, ///< The Voltage exceeds the acceptable min/max.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_FREQUENCY_OUTSIDE_RANGE = 0x44000003,   ///< The Frequency exceeds the acceptable min/max.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_POWER_OUTSIDE_RANGE = 0x44000004,   ///< The Power exceeds the acceptable min/max.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_TEMPERATURE_OUTSIDE_RANGE = 0x44000005, ///< The Power exceeds the acceptable min/max.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_IN_VOLTAGE_LOCKED_MODE = 0x44000006,///< The Overclock is in voltage locked mode.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_RESET_REQUIRED = 0x44000007,///< It indicates that the requested change will not be applied until the
                                                                        ///< device is reset.
            CTL_RESULT_ERROR_CORE_OVERCLOCK_WAIVER_NOT_SET = 0x44000008,///< The $OverclockWaiverSet function has not been called.
            CTL_RESULT_ERROR_CORE_END = 0x0440FFFF,         ///< "Core error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_3D_START = 0x60000000,         ///< 3D error code starting value, not to be used
            CTL_RESULT_ERROR_3D_END = 0x6000FFFF,           ///< "3D error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_MEDIA_START = 0x50000000,      ///< Media error code starting value, not to be used
            CTL_RESULT_ERROR_MEDIA_END = 0x5000FFFF,        ///< "Media error code end value, not to be used
                                                            ///< "
            CTL_RESULT_ERROR_DISPLAY_START = 0x48000000,    ///< Display error code starting value, not to be used
            CTL_RESULT_ERROR_INVALID_AUX_ACCESS_FLAG = 0x48000001,  ///< Invalid flag for Aux access
            CTL_RESULT_ERROR_INVALID_SHARPNESS_FILTER_FLAG = 0x48000002,///< Invalid flag for Sharpness
            CTL_RESULT_ERROR_DISPLAY_NOT_ATTACHED = 0x48000003, ///< Error for Display not attached
            CTL_RESULT_ERROR_DISPLAY_NOT_ACTIVE = 0x48000004,   ///< Error for display attached but not active
            CTL_RESULT_ERROR_INVALID_POWERFEATURE_OPTIMIZATION_FLAG = 0x48000005,   ///< Error for invalid power optimization flag
            CTL_RESULT_ERROR_INVALID_POWERSOURCE_TYPE_FOR_DPST = 0x48000006,///< DPST is supported only in DC Mode
            CTL_RESULT_ERROR_INVALID_PIXTX_GET_CONFIG_QUERY_TYPE = 0x48000007,  ///< Invalid query type for pixel transformation get configuration
            CTL_RESULT_ERROR_INVALID_PIXTX_SET_CONFIG_OPERATION_TYPE = 0x48000008,  ///< Invalid operation type for pixel transformation set configuration
            CTL_RESULT_ERROR_INVALID_SET_CONFIG_NUMBER_OF_SAMPLES = 0x48000009, ///< Invalid number of samples for pixel transformation set configuration
            CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_ID = 0x4800000a,   ///< Invalid block id for pixel transformation
            CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_TYPE = 0x4800000b, ///< Invalid block type for pixel transformation
            CTL_RESULT_ERROR_INVALID_PIXTX_BLOCK_NUMBER = 0x4800000c,   ///< Invalid block number for pixel transformation
            CTL_RESULT_ERROR_INSUFFICIENT_PIXTX_BLOCK_CONFIG_MEMORY = 0x4800000d,   ///< Insufficient memery allocated for BlockConfigs
            CTL_RESULT_ERROR_3DLUT_INVALID_PIPE = 0x4800000e,   ///< Invalid pipe for 3dlut
            CTL_RESULT_ERROR_3DLUT_INVALID_DATA = 0x4800000f,   ///< Invalid 3dlut data
            CTL_RESULT_ERROR_3DLUT_NOT_SUPPORTED_IN_HDR = 0x48000010,   ///< 3dlut not supported in HDR
            CTL_RESULT_ERROR_3DLUT_INVALID_OPERATION = 0x48000011,  ///< Invalid 3dlut operation
            CTL_RESULT_ERROR_3DLUT_UNSUCCESSFUL = 0x48000012,   ///< 3dlut call unsuccessful
            CTL_RESULT_ERROR_AUX_DEFER = 0x48000013,        ///< AUX defer failure
            CTL_RESULT_ERROR_AUX_TIMEOUT = 0x48000014,      ///< AUX timeout failure
            CTL_RESULT_ERROR_AUX_INCOMPLETE_WRITE = 0x48000015, ///< AUX incomplete write failure
            CTL_RESULT_ERROR_I2C_AUX_STATUS_UNKNOWN = 0x48000016,   ///< I2C/AUX unkonown failure
            CTL_RESULT_ERROR_I2C_AUX_UNSUCCESSFUL = 0x48000017, ///< I2C/AUX unsuccessful
            CTL_RESULT_ERROR_LACE_INVALID_DATA_ARGUMENT_PASSED = 0x48000018,///< Lace Incorrrect AggressivePercent data or LuxVsAggressive Map data
                                                                            ///< passed by user
            CTL_RESULT_ERROR_EXTERNAL_DISPLAY_ATTACHED = 0x48000019,///< External Display is Attached hence fail the Display Switch
            CTL_RESULT_ERROR_CUSTOM_MODE_STANDARD_CUSTOM_MODE_EXISTS = 0x4800001a,  ///< Standard custom mode exists
            CTL_RESULT_ERROR_CUSTOM_MODE_NON_CUSTOM_MATCHING_MODE_EXISTS = 0x4800001b,  ///< Non custom matching mode exists
            CTL_RESULT_ERROR_CUSTOM_MODE_INSUFFICIENT_MEMORY = 0x4800001c,  ///< Custom mode insufficent memory
            CTL_RESULT_ERROR_DISPLAY_END = 0x4800FFFF,      ///< "Display error code end value, not to be used
                                                            ///< "
            CTL_RESULT_MAX

        }

        public static void ConsolePrint(string text)
        {
            var dt = "[" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "] ";
            File.AppendAllText("debug.txt", dt + text + "\n");
        }
    }
    
}
