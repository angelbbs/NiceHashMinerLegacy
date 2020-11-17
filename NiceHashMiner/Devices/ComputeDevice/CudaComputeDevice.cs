/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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

namespace NiceHashMiner.Devices
{
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
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    var load = -1;
                    try
                    {
                        var rates = new nvmlUtilization();
                        var ret = NvmlNativeMethods.nvmlDeviceGetUtilizationRates(_nvmlDevice, ref rates);
                        if (ret != nvmlReturn.Success)
                            throw new Exception($"NVML get load failed with code: {ret}");

                        load = (int)rates.gpu;
                    }
                    catch (Exception e)
                    {
                        //Helpers.ConsolePrint("NVML", e.ToString());
                    }
                    return load;
                }
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
                                                return (int)sensor.Value;
                                            }
                                            else return -1;
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
                }
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
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    var temp = -1f;
                    try
                    {
                        var utemp = 0u;
                        var ret = NvmlNativeMethods.nvmlDeviceGetTemperature(_nvmlDevice, nvmlTemperatureSensors.Gpu,
                            ref utemp);
                        if (ret != nvmlReturn.Success)
                        {
                            Form_Main.needRestart = true;
                            //ComputeDeviceManager.Query.Nvidia.QueryCudaDevices();
                            //if(ComputeDeviceManager.Query.CheckVideoControllersCountMismath())

                            // throw new Exception($"NVML get temp failed with code: {ret}");
                        }
                        temp = utemp;
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("NVML", e.ToString());
                    }
                    return temp;
                }
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
                                                return (int)sensor.Value;
                                            }
                                            else return -1;
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
                }
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

        public override int FanSpeed
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA)
                {
                    return -1;
                }

                if (!ConfigManager.GeneralConfig.ShowFanAsPercent)
                    {
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
                    else
                    {
                        var fan = -1;

                        try
                        {
                            var ufan = 0u;
                            var ret = NvmlNativeMethods.nvmlDeviceGetFanSpeed(_nvmlDevice, ref ufan);
                            if (ret != nvmlReturn.Success)
                            {
                                Form_Main.needRestart = true;
                            }
                            fan = (int)ufan;
                        }
                        catch (Exception e)
                        {
                            Helpers.ConsolePrint("NVML", e.ToString());
                        }

                        return fan;
                    }
            }
        }

        public override double PowerUsage
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA)
                {
                    return -1;
                }
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                { 
                    try
                    {
                        var power = 0u;
                        var ret = NvmlNativeMethods.nvmlDeviceGetPowerUsage(_nvmlDevice, ref power);
                        if (ret != nvmlReturn.Success)
                            throw new Exception($"NVML power get failed with status: {ret}");

                        return power * 0.001;
                    }
                    catch (Exception e)
                    {
                        // Helpers.ConsolePrint("NVML", e.ToString());
                    }
                }
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
                                                return (int)sensor.Value;
                                            }
                                            else return -1;
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
                }
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
                cudaDevice.DeviceGlobalMemory)
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
        }
    }
}
