using ManagedCuda.Nvml;
using NVIDIA.NVAPI;
using System;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Configs;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NiceHashMiner.Forms;

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

                        //сомнительно...
                        /*
                        if (result == NvStatus.NVIDIA_DEVICE_NOT_FOUND)
                        {
                            int check = ComputeDeviceManager.Query.CheckVideoControllersCountMismath();
                            if (ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost)
                            {
                                var onGpusLost = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\OnGPUsLost.bat")
                                {
                                    WindowStyle = ProcessWindowStyle.Minimized
                                };
                                onGpusLost.Arguments = "1 " + check;
                                Helpers.ConsolePrint("ERROR", "Restart Windows due CUDA GPU#" + check.ToString() + " is lost");
                                Process.Start(onGpusLost);
                                Thread.Sleep(2000);
                            }
                            if (ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost)
                            {
                                var onGpusLost = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\OnGPUsLost.bat")
                                {
                                    WindowStyle = ProcessWindowStyle.Minimized
                                };
                                onGpusLost.Arguments = "2 " + check;
                                Helpers.ConsolePrint("ERROR", "Restart driver due CUDA GPU#" + check.ToString() + " is lost");
                                Form_Benchmark.RunCMDAfterBenchmark();
                                Thread.Sleep(1000);
                                Process.Start(onGpusLost);
                                Thread.Sleep(2000);
                            }
                        }
                        */
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
                cudaDevice.DeviceGlobalMemory, cudaDevice.CUDAManufacturer, cudaDevice.MonitorConnected)
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
