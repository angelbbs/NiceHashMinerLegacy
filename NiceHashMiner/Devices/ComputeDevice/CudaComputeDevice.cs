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
                        var rates = new nvmlUtilization();
                        var ret = NvmlNativeMethods.nvmlDeviceGetUtilizationRates(_nvmlDevice, ref rates);
                        if (ret != nvmlReturn.Success)
                        {
                            Helpers.ConsolePrint("NVML", $"NVML get load failed with code: {ret}");
                        }
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
                        var utemp = 0u;
                        var ret = NvmlNativeMethods.nvmlDeviceGetTemperature(_nvmlDevice, nvmlTemperatureSensors.Gpu,
                            ref utemp);
                        if (ret != nvmlReturn.Success)
                        {
                            Helpers.ConsolePrint("NVML", $"NVML get temp failed with code: {ret}");

                            //Form_Main.needRestart = true;
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
                        var ufan = 0u;
                        var ret = NvmlNativeMethods.nvmlDeviceGetFanSpeed(_nvmlDevice, ref ufan);
                        if (ret != nvmlReturn.Success)
                        {
                        //Form_Main.needRestart = true;
                        Helpers.ConsolePrint("NVML", $"NVML get fan failed with code: {ret}");
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
                try
                {
                    var p = Process.GetProcessesByName("NvidiaGPUGetDataHost");
                }
                catch (Exception ex)
                {
                    GC.Collect();
                }
                try
                {
                    var _power = 0u;
                    nvmlDevice nvmlDevice = new nvmlDevice();
                    byte[] data;
                    int size;
                    MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("NvidiaGPUGetDataHost");
                    using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 1, MemoryMappedFileAccess.Read))
                    {
                        size = reader.ReadByte(0);
                    }
                    using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read))
                    {
                        data = new byte[size];
                        reader.ReadArray<byte>(0, data, 0, size);
                    }

                    int devCount = (size - 1) / 2;
                    for (int dev = 0; dev < devCount; dev++)
                    {
                        var ret = NvmlNativeMethods.nvmlDeviceGetHandleByIndex((uint)dev, ref nvmlDevice);
                        if (ret != nvmlReturn.Success)
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                Helpers.ConsolePrint("NVML", "nvmlDeviceGetHandleByIndex error: " + ret.ToString());
                            }
                            break;
                        }
                        if (_nvmlDevice.Equals(nvmlDevice))
                        {
                            power = data[dev * 2 + 2];
                        }
                    }

                    //IntPtr ptr = GetPowerNativeMethod.GetPtr();
                    //GetPowerNativeMethod.GetPower(_nvmlDevice, ref _power); //<- mem leak 461.40+
                    /*
                    var ret = NvmlNativeMethods.nvmlDeviceGetPowerUsage(_nvmlDevice, ref _power);// <- mem leak 461.40+
                    if (ret != nvmlReturn.Success)
                    {
                        Helpers.ConsolePrint("NVML", $"NVML get power failed with code: {ret}");
                    }
                    */
                    //Helpers.ConsolePrint("NVML", $"NVML get power: {_power.ToString()}");
                    //Marshal.FreeHGlobal(ptr);
                    /*
                    try
                    {
                        Process Mem;
                        Mem = Process.GetCurrentProcess();
                        NativeMethods.SetProcessWorkingSetSize(Mem.Handle, -1, -1);
                        Mem = null;
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("NVML", "Error = " + ex.ToString() + " " + ex.StackTrace.ToString());
                    }
                    */

                    //power = (int)(_power/1000);
                }
                catch (FileNotFoundException e)
                {
                    Helpers.ConsolePrint("NVML", "Error! MemoryMappedFile not found");
                    if (File.Exists("common\\NvidiaGPUGetDataHost.exe"))
                    {
                        var MonitorProc = new Process
                        {
                            StartInfo = { FileName = "common\\NvidiaGPUGetDataHost.exe" }
                        };

                        MonitorProc.StartInfo.UseShellExecute = false;
                        MonitorProc.StartInfo.CreateNoWindow = true;
                        if (MonitorProc.Start())
                        {
                            Helpers.ConsolePrint("NvidiaGPUGetDataHost", "Starting OK");
                        }
                        else
                        {
                            Helpers.ConsolePrint("NvidiaGPUGetDataHost", "Starting ERROR");
                        }
                    }
                    Thread.Sleep(500);
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
