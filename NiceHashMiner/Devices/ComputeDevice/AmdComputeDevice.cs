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
    public class AmdComputeDevice : ComputeDevice
    {
        private readonly int _adapterIndex; // For ADL
        private readonly int _adapterIndex2; // For ADL2
        private readonly IntPtr _adlContext;

        public override int FanSpeed //percent
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringAMD)
                {
                    return -1;
                }
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    var adlf = new ADLFanSpeedValue
                    {
                        SpeedType = ADL.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
                    };
                    var result = ADL.ADL_Overdrive5_FanSpeed_Get(_adapterIndex, 0, ref adlf);
                    if (result == ADL.ADL_SUCCESS)
                    {
                        return adlf.FanSpeed;
                    }
                }
                else
                {

                    try
                    {
                        foreach (IHardware hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Control)
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
                        Helpers.ConsolePrint("AmdComputeDevice", er.ToString());
                    }
                }
                return -1;
            }
        }

        public override int FanSpeedRPM
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringAMD)
                {
                    return -1;
                }
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    var adlf = new ADLFanSpeedValue
                    {
                        SpeedType = ADL.ADL_DL_FANCTRL_SPEED_TYPE_RPM
                    };
                    var result = ADL.ADL_Overdrive5_FanSpeed_Get(_adapterIndex, 0, ref adlf);
                    if (result == ADL.ADL_SUCCESS)
                    {
                        return adlf.FanSpeed;
                    }
                }
                else
                {

                    try
                    {
                        foreach (IHardware hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Fan && sensor.Value != null)
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
                        Helpers.ConsolePrint("AmdComputeDevice", er.ToString());
                    }
                }
                return -1;
            }
        }

        public override float Temp
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringAMD)
                {
                    return -1;
                }
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    var adlt = new ADLTemperature();
                    var result = ADL.ADL_Overdrive5_Temperature_Get(_adapterIndex, 0, ref adlt);
                    if (result == ADL.ADL_SUCCESS)
                    {
                        return adlt.Temperature * 0.001f;
                    }
                }
                else
                {

                    try
                    {
                        foreach (IHardware hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Temperature && sensor.Name == "GPU Core")
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
                        Helpers.ConsolePrint("AmdComputeDevice", er.ToString());
                    }
                }
                return -1;
            }
        }

        public override float TempMemory
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringAMD)
                {
                    return -1;
                }
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    var temperature = -1;
                    if (_adlContext != IntPtr.Zero && ADL.ADL2_OverdriveN_Temperature_Get != null)
                    {
                        var result = ADL.ADL2_OverdriveN_Temperature_Get(_adlContext, _adapterIndex2, ADLODNTemperatureType.MEMORY, ref temperature);
                        if (result == ADL.ADL_SUCCESS)
                        {
                            if (temperature > 1000)
                            {
                                return -2; //not supported
                            }
                            else
                            {
                                return temperature;
                            }
                        }
                    }
                }
                else
                {

                    try
                    {
                        foreach (IHardware hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Temperature && sensor.Name == "GPU Memory")
                                        {
                                            //Helpers.ConsolePrint("***********mem", sensor.Value.ToString());
                                            if (sensor.Value > 0 && sensor.Value < 1)
                                            {
                                                return (int)(sensor.Value * 1000);
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
                        Helpers.ConsolePrint("AmdComputeDevice", er.ToString());
                    }
                }
                return -1;
            }
        }

        public override float Load
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringAMD)
                {
                    return -1;
                }
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    var adlp = new ADLPMActivity();
                    var result = ADL.ADL_Overdrive5_CurrentActivity_Get(_adapterIndex, ref adlp);
                    if (result == ADL.ADL_SUCCESS)
                    {
                        return adlp.ActivityPercent;
                    }
                }
                else
                {
                    try
                    {
                        foreach (IHardware hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
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
                        Helpers.ConsolePrint("AmdComputeDevice", er.ToString());
                    }
                }
                return -1;
            }
        }

        public override float MemLoad
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringAMD)
                {
                    return 0;
                }
                var aDLPMLogDataOutput = new ADLPMLogDataOutput();
                var result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex2, ref aDLPMLogDataOutput);
                if (result == ADL.ADL_SUCCESS)
                    {
                    int i = (int)ADLSensorType.PMLOG_INFO_ACTIVITY_MEM;
                    if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                    {
                        return aDLPMLogDataOutput.sensors[i].value;
                    }
                }
                return 0;
            }
        }

        public override double PowerUsage
        {
            get
            {
                double addAMD = ConfigManager.GeneralConfig.PowerAddAMD;
                if (ConfigManager.GeneralConfig.DisableMonitoringAMD)
                {
                    return -1;
                }
                if (!ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    var power = -1;
                    if (_adlContext != IntPtr.Zero && ADL.ADL2_Overdrive6_CurrentPower_Get != null)
                    {
                        var result = ADL.ADL2_Overdrive6_CurrentPower_Get(_adlContext, _adapterIndex2, 0, ref power); //0
                        if (result == ADL.ADL_SUCCESS)
                        {
                            return (double)(power / (1 << 8)) + addAMD;
                        } else
                        {
                            result = ADL.ADL2_Overdrive6_CurrentPower_Get(_adlContext, _adapterIndex, 0, ref power); //0
                            if (result == ADL.ADL_SUCCESS)
                            {
                                return (double)(power / (1 << 8)) + addAMD;
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        foreach (IHardware hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Power)
                                        {
                                            if (sensor.Value.HasValue)
                                            {
                                                if ((int)sensor.Value >= 0)
                                                {
                                                    return (int)sensor.Value + addAMD;
                                                }
                                            }
                                        }
                                    }
                                }
                                //internal
                                var power = -1;
                                if (_adlContext != IntPtr.Zero && ADL.ADL2_Overdrive6_CurrentPower_Get != null)
                                {
                                    var result = ADL.ADL2_Overdrive6_CurrentPower_Get(_adlContext, _adapterIndex2, 0, ref power); //0
                                    if (result == ADL.ADL_SUCCESS)
                                    {
                                        //Helpers.ConsolePrint("ADL", power.ToString());
                                        return (double)(power / (1 << 8)) + addAMD;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception er)
                    {
                        Helpers.ConsolePrint("AmdComputeDevice", er.ToString());
                    }
                }

                return -1;
            }
        }

        public AmdComputeDevice(AmdGpuDevice amdDevice, int gpuCount, bool isDetectionFallback, int adl2Index)
            : base(amdDevice.DeviceID,
                amdDevice.DeviceName,
                true,
                DeviceGroupType.AMD_OpenCL,
                amdDevice.IsEtherumCapable(),
                DeviceType.AMD,
                string.Format(International.GetText("ComputeDevice_Short_Name_AMD_GPU"), gpuCount),
                amdDevice.DeviceGlobalMemory, amdDevice.AMDManufacturer, amdDevice.MonitorConnected, false)
        {
            Uuid = isDetectionFallback
                ? GetUuid(ID, GroupNames.GetGroupName(DeviceGroupType, ID), Name, DeviceGroupType)
                : amdDevice.UUID;
            BusID = amdDevice.BusID;
            Codename = amdDevice.Codename;
            InfSection = amdDevice.InfSection;
            AlgorithmSettings = GroupAlgorithms.CreateForDeviceList(this);
            DriverDisableAlgos = amdDevice.DriverDisableAlgos;
            Index = ID + ComputeDeviceManager.Available.AvailCpus + ComputeDeviceManager.Available.AvailNVGpus;
            _adapterIndex = amdDevice.AdapterIndex;

            ADL.ADL2_Main_Control_Create?.Invoke(ADL.ADL_Main_Memory_Alloc, 0, ref _adlContext);
            _adapterIndex2 = adl2Index;
        }
    }
}
