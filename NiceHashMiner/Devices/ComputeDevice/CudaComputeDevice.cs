using ManagedCuda.Nvml;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMiner.Forms;
using NiceHashMinerLegacy.Common.Enums;
using NVIDIA.NVAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;


namespace NiceHashMiner.Devices
{
    [Serializable]
    internal class CudaComputeDevice : ComputeDevice
    {
        private readonly NvPhysicalGpuHandle _nvHandle; // For NVAPI
        private readonly nvmlDevice _nvmlDevice; // For NVML
        private readonly CudaDevices2 _cudaDevice; // For NVML
        private const int GpuCorePState = 0; // memcontroller = 1, videng = 2
        protected int SMMajor;
        protected int SMMinor;
        public readonly bool ShouldRunEthlargement;
        private int errorcount = 0;
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
                        if (_cudaDevice.DeviceID == d.nGpu)
                        {
                            return d.load;
                        }
                    }
                }
                catch (Exception)
                {
                    //Helpers.ConsolePrint("NVML", e.ToString());
                }
                return load;
            }
        }
        public override float MemLoad
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
                    foreach (var d in Form_Main.gpuList)
                    {
                        if (_cudaDevice.DeviceID == d.nGpu)
                        {
                            return d.loadMem;
                        }
                    }
                }
                catch (Exception)
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
                        if (_cudaDevice.DeviceID == d.nGpu)
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

        public override float TempMemory
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA || Form_Main.NvAPIerror)
                {
                    return -1;
                }

                var tempMem = -1;
                var nvHandle = GetNvPhysicalGpuHandle();
                if (!nvHandle.HasValue)
                {
                    Helpers.ConsolePrint("NVAPI", $"TempMemory nvHandle == null", TimeSpan.FromMinutes(5));
                    return -1;
                }

                try
                {
                    foreach (var d in Form_Main.gpuList)
                    {
                        if (_cudaDevice.DeviceID == d.nGpu)
                        {
                            return d.tempMem;
                        }
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("NVML", e.ToString());
                }
                return tempMem;
            }
        }

        private NvPhysicalGpuHandle? _NvPhysicalGpuHandle;
        private NvPhysicalGpuHandle? GetNvPhysicalGpuHandle()
        {
            if (_NvPhysicalGpuHandle.HasValue) return _NvPhysicalGpuHandle.Value;
            if (NVAPI.NvAPI_EnumPhysicalGPUs == null)
            {
                Helpers.ConsolePrint("GetNvPhysicalGpuHandle", "NvAPI_EnumPhysicalGPUs unavailable ");
                return null;
            }
            if (NVAPI.NvAPI_GPU_GetBusID == null)
            {
                Helpers.ConsolePrint("GetNvPhysicalGpuHandle", "NvAPI_GPU_GetBusID unavailable");
                return null;
            }


            var handles = new NvPhysicalGpuHandle[NVAPI.MAX_PHYSICAL_GPUS];
            var status = NVAPI.NvAPI_EnumPhysicalGPUs(handles, out _);
            if (status != NvStatus.OK)
            {
                Helpers.ConsolePrint("GetNvPhysicalGpuHandle", $"Enum physical GPUs failed with status: {status}", TimeSpan.FromMinutes(5));
            }
            else
            {
                foreach (var handle in handles)
                {
                    var idStatus = NVAPI.NvAPI_GPU_GetBusID(handle, out var id);

                    if (idStatus == NvStatus.EXPECTED_PHYSICAL_GPU_HANDLE) continue;

                    if (idStatus != NvStatus.OK)
                    {
                        Helpers.ConsolePrint("GetNvPhysicalGpuHandle", "Bus ID get failed with status: " + idStatus, TimeSpan.FromMinutes(5));
                    }
                    else if (id == BusID)
                    {
                        Helpers.ConsolePrint("GetNvPhysicalGpuHandle", "Found handle for busid " + id, TimeSpan.FromMinutes(5));
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
                        if (_cudaDevice.DeviceID == d.nGpu)
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

       
        private int GetFanSpeedRPM()
        {
            if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA || Form_Main.NvAPIerror)
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
                if (result != NvStatus.OK)
                {
                    var coolersStatus = GetFanCoolersStatus();
                    if (coolersStatus.Count > 0)
                    {
                        uint CurrentLevel = coolersStatus.Items[0].CurrentLevel;
                        uint CurrentRpm = coolersStatus.Items[0].CurrentRpm;
                        fanSpeed = (int)CurrentRpm;
                    }
                }
                if (result != NvStatus.OK && result != NvStatus.NOT_SUPPORTED)
                {
                    return -1;
                }
            }
            return fanSpeed;
        }

        public override int FanSpeedRPM
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringNVIDIA || Form_Main.NvAPIerror)
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
                    if (result != NvStatus.OK)
                    {
                        var coolersStatus = GetFanCoolersStatus();
                        if (coolersStatus.Count > 0)
                        {
                            uint CurrentLevel = coolersStatus.Items[0].CurrentLevel;
                            uint CurrentRpm = coolersStatus.Items[0].CurrentRpm;
                            fanSpeed = (int)CurrentRpm;
                        }
                    }
                    if (result != NvStatus.OK && result != NvStatus.NOT_SUPPORTED)
                    {
                        // GPUs without fans are not uncommon, so don't treat as error and just return -1
                        /*
                        Helpers.ConsolePrint("NVAPI", "Tach get failed with status: " + result);
                        Helpers.ConsolePrint("NVAPI", "_nvmlDevice: " + _nvmlDevice.ToString());
                        Helpers.ConsolePrint("NVAPI", "_nvHandle: " + _nvHandle.ToString());
                        Helpers.ConsolePrint("NVAPI", "fanSpeed: " + fanSpeed.ToString());
                        */
                        //сомнительно...

                        if (result == NvStatus.NVIDIA_DEVICE_NOT_FOUND && ConfigManager.GeneralConfig.CheckingCUDA)
                        {
                            /*
                            Helpers.ConsolePrint("NVAPI", "_nvmlDevice: " + _nvmlDevice.ToString());
                            Helpers.ConsolePrint("NVAPI", "_nvHandle: " + _nvHandle.ToString());
                            */
                            errorcount++;
                            int check = ComputeDeviceManager.Query.CheckVideoControllersCountMismath();
                            if (ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost && errorcount > 5)
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
                            if (ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost && errorcount > 5)
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

                        return -1;
                    }
                } 
                return fanSpeed;
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
                int power = -1;

                try
                {
                    foreach (var d in Form_Main.gpuList)
                    {
                        if (_cudaDevice.DeviceID == d.nGpu)
                        {
                            if (d.power > 1000)
                            {
                                return d.power / 1000;
                            } else
                            {
                                return d.power;
                            }
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

        private NvFanCoolersStatus GetFanCoolersStatus()
        {
            var coolers = new NvFanCoolersStatus();
            coolers.Version = NVAPI.GPU_FAN_COOLERS_STATUS_VER;
            coolers.Items =
              new NvFanCoolersStatusItem[NVAPI.MAX_FAN_COOLERS_STATUS_ITEMS];

            if (!(NVAPI.NvAPI_GPU_ClientFanCoolersGetStatus != null &&
               NVAPI.NvAPI_GPU_ClientFanCoolersGetStatus(_nvHandle, ref coolers)
               == NvStatus.OK))
            {
                coolers.Count = 0;
            }
            return coolers;
        }

        
        public CudaComputeDevice(CudaDevices2 cudaDevice, DeviceGroupType group, int gpuCount,
            NvPhysicalGpuHandle nvHandle, nvmlDevice nvmlHandle)
            : base((int)cudaDevice.DeviceID,
                cudaDevice.GetName(),
                true,
                group,
                cudaDevice.IsEtherumCapable(),
                DeviceType.NVIDIA,
                string.Format(International.GetText("ComputeDevice_Short_Name_NVIDIA_GPU"), gpuCount),
                cudaDevice.DeviceGlobalMemory, cudaDevice.CUDAManufacturer, cudaDevice.MonitorConnected, cudaDevice.NvidiaLHR)
        {
            BusID = cudaDevice.pciBusID;
            SMMajor = cudaDevice.SM_major;
            SMMinor = cudaDevice.SM_minor;
            Uuid = cudaDevice.UUID;
            AlgorithmSettings = GroupAlgorithms.CreateForDeviceList(this);
            Index = ID + ComputeDeviceManager.Available.AvailCpus; // increment by CPU count

            _nvHandle = nvHandle;
            _nvmlDevice = nvmlHandle;
            _cudaDevice = cudaDevice;
            ShouldRunEthlargement = cudaDevice.DeviceName.Contains("1080") || cudaDevice.DeviceName.Contains("Titan Xp");
            Form_Main.ShouldRunEthlargement = ShouldRunEthlargement;
        }
    }
}
