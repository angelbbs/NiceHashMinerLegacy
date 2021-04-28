using ManagedCuda.Nvml;
using NVIDIA.NVAPI;
using System;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Configs;
using static NiceHashMiner.Devices.ComputeDeviceManager;
using System.Diagnostics;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;

namespace NiceHashMiner.Devices
{
    [Serializable]
    internal class CudaComputeDevice : ComputeDevice
    {
        private readonly NvPhysicalGpuHandle _nvHandle; // For NVAPI
        private readonly nvmlDevice _nvmlDevice; // For NVML
        private const int GpuCorePState = 0; // memcontroller = 1, videng = 2
        protected int SMMajor;
        protected int SMMinor;
        public readonly bool ShouldRunEthlargement;
        public override float Load
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA)
                {
                    return -1;
                }
                int load = -1;
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        //uint dev = (uint)_nvmlDevice.Pointer;
                        foreach (var d in Form_Main.gpuList)
                        {
                            if (Convert.ToInt32((long)_nvmlDevice.Pointer % Int32.MaxValue) == d.nGpu)
                            {
                                return d.load;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //Helpers.ConsolePrint("NVML", e.ToString());
                    }
                    return load;
                }
                /*
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {

                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuNvidia)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/nvidiagpu/", ""), out var gpuId);
                                if (gpuId == ID)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Load)
                                        {
                                            if ((int)sensor.Value >= 0)
                                            {
                                                load = (int)sensor.Value;
                                            }
                                            else load = -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        Helpers.ConsolePrint("CudaComputeDevice", er.ToString());
                    }
                    return load;
                }
                */
                return -1;
            }
        }


        public override float Temp
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA)
                {
                    return -1;
                }
                var temp = -1f;
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        //uint dev = (uint)_nvmlDevice.Pointer;
                        foreach (var d in Form_Main.gpuList)
                        {
                            if (Convert.ToInt32((long)_nvmlDevice.Pointer % Int32.MaxValue) == d.nGpu)
                            {
                                return d.temp;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("NVML", e.ToString());
                    }
                    return temp;
                }
                /*
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuNvidia)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/nvidiagpu/", ""), out var gpuId);
                                if (gpuId == ID)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Temperature)
                                        {
                                            if ((int)sensor.Value > 0)
                                            {
                                                temp = (int)sensor.Value;
                                            }
                                            else temp = -1;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        Helpers.ConsolePrint("CudaComputeDevice", er.ToString());
                    }
                    return temp;
                }
                */
                return -1;
            }
        }

        private NvPhysicalGpuHandle? _NvPhysicalGpuHandle;
        private NvPhysicalGpuHandle? GetNvPhysicalGpuHandle()
        {
            if (_NvPhysicalGpuHandle.HasValue) return _NvPhysicalGpuHandle.Value;
            if (NVAPI.NvAPI_EnumPhysicalGPUs == null)
            {
                Helpers.ConsolePrint("NVAPI", "NvAPI_EnumPhysicalGPUs unavailable ");
                return null;
            }
            if (NVAPI.NvAPI_GPU_GetBusID == null)
            {
                Helpers.ConsolePrint("NVAPI", "NvAPI_GPU_GetBusID unavailable");
                return null;
            }


            var handles = new NvPhysicalGpuHandle[NVAPI.MAX_PHYSICAL_GPUS];
            var status = NVAPI.NvAPI_EnumPhysicalGPUs(handles, out _);
            if (status != NvStatus.OK)
            {
                Helpers.ConsolePrint("NVAPI", $"Enum physical GPUs failed with status: {status}", TimeSpan.FromMinutes(5));
            }
            else
            {
                foreach (var handle in handles)
                {
                    var idStatus = NVAPI.NvAPI_GPU_GetBusID(handle, out var id);

                    if (idStatus == NvStatus.EXPECTED_PHYSICAL_GPU_HANDLE) continue;

                    if (idStatus != NvStatus.OK)
                    {
                        Helpers.ConsolePrint("NVAPI", "Bus ID get failed with status: " + idStatus, TimeSpan.FromMinutes(5));
                    }
                    else if (id == BusID)
                    {
                        Helpers.ConsolePrint("NVAPI", "Found handle for busid " + id, TimeSpan.FromMinutes(5));
                        _NvPhysicalGpuHandle = handle;
                        return handle;
                    }
                }
            }
            return null;
        }

        public override int FanSpeed //percent
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA)
                {
                    return -1;
                }

                
                    var fan = -1;

                try
                {
                    //uint dev = (uint)_nvmlDevice.Pointer;
                    foreach (var d in Form_Main.gpuList)
                    {
                        if (Convert.ToInt32((long)_nvmlDevice.Pointer % Int32.MaxValue) == d.nGpu)
                        {
                            return (int)d.fan;
                        }
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("NVML", e.ToString());
                }

                    return fan;
                
            }
        }

        public override int FanSpeedRPM
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA)
                {
                    return -1;
                }

                    var fanSpeed = -1;

                    // we got the lock
                    var nvHandle = GetNvPhysicalGpuHandle();
                    if (!nvHandle.HasValue)
                    {
                        Helpers.ConsolePrint("NVAPI", $"FanSpeed nvHandle == null", TimeSpan.FromMinutes(5));
                        return -1;
                    }

                    if (NVAPI.NvAPI_GPU_GetTachReading != null)
                    {
                        var result = NVAPI.NvAPI_GPU_GetTachReading(nvHandle.Value, out fanSpeed);
                        if (result != NvStatus.OK && result != NvStatus.NOT_SUPPORTED)
                        {
                            // GPUs without fans are not uncommon, so don't treat as error and just return -1
                            Helpers.ConsolePrint("NVAPI", "Tach get failed with status: " + result);
                            return -1;
                        }
                    }

                    return fanSpeed;
                
            }
        }
        
        public List<NvData> gpuList = new List<NvData>();
        [Serializable]
        public struct NvData
        {
            //public int status;
            public uint nGpu;
            public uint power;
        }
        public byte[] NVdata;
        public int devCount = 0;
        public int ferrorCount = 0;
        public override double PowerUsage
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA)
                {
                    return -1;
                }
                int power = -1;

                //if (ComputeDeviceManager.Query._currentNvidiaSmiDriver.IsLesserVersionThan(ComputeDeviceManager.Query.LastGoodNvidiaCuda111Driver))
                //{
                //}
                /*
                try
                {
                    var p = Process.GetProcessesByName("NvidiaGPUGetDataHost");
                }
                catch (Exception ex)
                {
                    GC.Collect();
                }
                */

                try
                {
                    foreach (var d in Form_Main.gpuList)
                    {
                        if (Convert.ToInt32((long)_nvmlDevice.Pointer % Int32.MaxValue) == d.nGpu)
                        {
                            return d.power / 1000;
                        }
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("NVML", e.ToString());
                    power = -1;
                }
                return power;
                
                /*
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuNvidia)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/nvidiagpu/", ""), out var gpuId);
                                if (gpuId == ID)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Power)
                                        {
                                            if ((int)sensor.Value >= 0)
                                            {
                                                power = (int)sensor.Value;
                                            }
                                            else power = -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        Helpers.ConsolePrint("CudaComputeDevice", er.ToString());
                    }
                    return power;
                }
                */
                return -1;
            }
        }

        public CudaComputeDevice(CudaDevices2 cudaDevice, DeviceGroupType group, int gpuCount,
            NvPhysicalGpuHandle nvHandle, nvmlDevice nvmlHandle)
            : base((int) cudaDevice.DeviceID,
                cudaDevice.GetName(),
                true,
                group,
                cudaDevice.IsEtherumCapable(),
                DeviceType.NVIDIA,
                string.Format(International.GetText("ComputeDevice_Short_Name_NVIDIA_GPU"), gpuCount),
                cudaDevice.DeviceGlobalMemory, cudaDevice.CUDAManufacturer)
        {
            BusID = cudaDevice.pciBusID;
            SMMajor = cudaDevice.SM_major;
            SMMinor = cudaDevice.SM_minor;
            Uuid = cudaDevice.UUID;
            AlgorithmSettings = GroupAlgorithms.CreateForDeviceList(this);
            Index = ID + ComputeDeviceManager.Available.AvailCpus; // increment by CPU count

            _nvHandle = nvHandle;
            _nvmlDevice = nvmlHandle;
            ShouldRunEthlargement = cudaDevice.DeviceName.Contains("1080") || cudaDevice.DeviceName.Contains("Titan Xp");
            Form_Main.ShouldRunEthlargement = ShouldRunEthlargement;
        }
    }
}
