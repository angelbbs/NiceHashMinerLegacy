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

        private int FanSpeedInternal()
        {
            var adlf = new ADLFanSpeedValue
            {
                SpeedType = ADL.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
            };
            try
            {
                var result = ADL.ADL_Overdrive5_FanSpeed_Get(_adapterIndex, 0, ref adlf);

                if (result == ADL.ADL_SUCCESS)
                {
                    return adlf.FanSpeed;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }
        private int FanSpeedInternal8()
        {
            var aDLPMLogDataOutput = new ADLPMLogDataOutput();
            try
            {
                var result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex2, ref aDLPMLogDataOutput);
                if (result == ADL.ADL_SUCCESS)
                {
                    int i = (int)ADLSensorType.PMLOG_FAN_PERCENTAGE;
                    if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                    {
                        return aDLPMLogDataOutput.sensors[i].value;
                    }
                }
                else
                {
                    result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex, ref aDLPMLogDataOutput);
                    if (result == ADL.ADL_SUCCESS)
                    {
                        int i = (int)ADLSensorType.PMLOG_FAN_PERCENTAGE;
                        if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                        {
                            return aDLPMLogDataOutput.sensors[i].value;
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }
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
                    int valueFanSpeed = FanSpeedInternal();
                    if (valueFanSpeed >= 0)
                    {
                        return valueFanSpeed;
                    } else
                    {
                        return FanSpeedInternal8();
                    }
                }
                else
                {
                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.Name.Contains("GPU Fan") &&
                                            sensor.SensorType == SensorType.Control && sensor.Value != null)
                                        {
                                            if ((int)sensor.Value >= 0)
                                            {
                                                return (int)sensor.Value;
                                            }
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
                int value = FanSpeedInternal();
                if (value >= 0)
                {
                    return value;
                }
                else
                {
                    return FanSpeedInternal8();
                }
            }
        }

        private int FanSpeedRPMInternal()
        {
            var adlf = new ADLFanSpeedValue
            {
                SpeedType = ADL.ADL_DL_FANCTRL_SPEED_TYPE_RPM
            };
            try
            {
                var result = ADL.ADL_Overdrive5_FanSpeed_Get(_adapterIndex, 0, ref adlf);
                if (result == ADL.ADL_SUCCESS)
                {
                    return adlf.FanSpeed;
                } else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }
        private int FanSpeedRPMInternal8()
        {
            var aDLPMLogDataOutput = new ADLPMLogDataOutput();
            try
            {
                var result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex2, ref aDLPMLogDataOutput);
                if (result == ADL.ADL_SUCCESS)
                {
                    int i = (int)ADLSensorType.PMLOG_FAN_RPM;
                    if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                    {
                        return aDLPMLogDataOutput.sensors[i].value;
                    }
                }
                else
                {
                    result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex, ref aDLPMLogDataOutput);
                    if (result == ADL.ADL_SUCCESS)
                    {
                        int i = (int)ADLSensorType.PMLOG_FAN_RPM;
                        if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                        {
                            return aDLPMLogDataOutput.sensors[i].value;
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
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
                    int valueFanSpeedRPM = FanSpeedRPMInternal();
                    if (valueFanSpeedRPM >= 0)
                    {
                        return valueFanSpeedRPM;
                    } else
                    {
                        return FanSpeedRPMInternal8();
                    }
                }
                else
                {
                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.Name.Contains("GPU Fan") && 
                                            sensor.SensorType == SensorType.Fan && sensor.Value != null)
                                        {
                                            if ((int)sensor.Value >= 0)
                                            {
                                                return (int)sensor.Value;
                                            }
                                        }
                                        /*
                                        if (sensor.SensorType == SensorType.Control && 
                                            sensor.Name == "GPU Fan" && sensor.Value != null)
                                        {
                                            return 0;
                                        }
                                        */
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
                int value = FanSpeedRPMInternal();
                if (value >= 0)
                {
                    return value;
                }
                else
                {
                    return FanSpeedRPMInternal8();
                }
                return -1;
            }
        }

        private float TempInternal()
        {
            var adlt = new ADLTemperature();
            try
            {
                var result = ADL.ADL_Overdrive5_Temperature_Get(_adapterIndex, 0, ref adlt);
                if (result == ADL.ADL_SUCCESS)
                {
                    return adlt.Temperature * 0.001f;
                } else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }
        private int TempInternalN()
        {
            var temperature = -1;
            if (_adlContext != IntPtr.Zero && ADL.ADL2_OverdriveN_Temperature_Get != null)
            {
                var result = ADL.ADL2_OverdriveN_Temperature_Get(_adlContext, _adapterIndex2, ADLODNTemperatureType.CORE, ref temperature);
                if (result == ADL.ADL_SUCCESS)
                {
                    //if (temperature > 1000)
                    {
                        //return -2; //not supported
                    }
                    //else
                    {
                        return (int)(temperature * 0.001f);
                    }
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }
        private int TempInternal8()
        {
            var aDLPMLogDataOutput = new ADLPMLogDataOutput();
            try
            {
                var result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex2, ref aDLPMLogDataOutput);
                if (result == ADL.ADL_SUCCESS)
                {
                    int i = (int)ADLSensorType.PMLOG_TEMPERATURE_EDGE;
                    if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                    {
                        return aDLPMLogDataOutput.sensors[i].value;
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
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
                    int valueTemp = (int)TempInternal();
                    if (valueTemp >= 0)
                    {
                        return valueTemp;
                    } else
                    {
                        valueTemp = (int)TempInternalN();
                        if (valueTemp >= 0)
                        {
                            return valueTemp;
                        }
                        else
                        {
                            return TempInternal8();
                        }
                    }
                }
                else
                {

                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("GPU Core"))
                                        {
                                            if ((int)sensor.Value > 0)
                                            {
                                                return (int)sensor.Value;
                                            }
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
                int value = (int)TempInternal();
                if (value >= 0)
                {
                    return value;
                }
                else
                {
                    value = (int)TempInternalN();
                    if (value >= 0)
                    {
                        return value;
                    }
                    else
                    {
                        return TempInternal8();
                    }
                }
                return -1;
            }
        }

        private int TempMemoryInternal()
        {
            var temperature = -1;
            if (_adlContext != IntPtr.Zero && ADL.ADL2_OverdriveN_Temperature_Get != null)
            {
                var result = ADL.ADL2_OverdriveN_Temperature_Get(_adlContext, _adapterIndex2, ADLODNTemperatureType.MEMORY, ref temperature);
                if (result == ADL.ADL_SUCCESS)
                {
                    if (temperature >= 1000)
                    {
                        return -2; //not supported
                    }
                    else
                    {
                        return temperature;
                    }
                } else
                {
                    return -1;
                }
            }
            return -1;
        }
        private int TempMemoryInternal8()
        {
            var aDLPMLogDataOutput = new ADLPMLogDataOutput();
            try
            {
                var result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex2, ref aDLPMLogDataOutput);
                if (result == ADL.ADL_SUCCESS)
                {
                    int i = (int)ADLSensorType.PMLOG_TEMPERATURE_MEM;
                    if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                    {
                        return aDLPMLogDataOutput.sensors[i].value;
                    }
                }
                return -2;
            }
            catch (Exception ex)
            {
                return -2;
            }
            return -2;
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
                    int valueTempMemory = TempMemoryInternal();
                    if (valueTempMemory >= 0)
                    {
                        return valueTempMemory;
                    } else
                    {
                        return TempMemoryInternal8();
                    }
                }
                else
                {
                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("GPU Memory"))
                                        {
                                            if (sensor.Value > 0 && sensor.Value < 1)
                                            {
                                                return (int)(sensor.Value * 1000);
                                            }
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
                int value = TempMemoryInternal();
                if (value >= 0)
                {
                    return value;
                }
                else
                {
                    return TempMemoryInternal8();
                }
                return -1;
            }
        }

        private int LoadInternal()
        {
            var adlp = new ADLPMActivity();
            try
            {
                var result = ADL.ADL_Overdrive5_CurrentActivity_Get(_adapterIndex, ref adlp);
                if (result == ADL.ADL_SUCCESS)
                {
                    return adlp.ActivityPercent;
                } else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
        }
        private int LoadInternal8()
        {
            var aDLPMLogDataOutput = new ADLPMLogDataOutput();
            try
            {
                var result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex2, ref aDLPMLogDataOutput);
                if (result == ADL.ADL_SUCCESS)
                {
                    int i = (int)ADLSensorType.PMLOG_INFO_ACTIVITY_GFX;
                    if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                    {
                        return aDLPMLogDataOutput.sensors[i].value;
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
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
                    int valueLoad = LoadInternal();
                    if (valueLoad >= 0)
                    {
                        return valueLoad;
                    } else
                    {
                        return LoadInternal8();
                    }
                }
                else
                {
                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.Name.ToLower().Contains("gpu core") & sensor.SensorType == SensorType.Load)
                                        {
                                            if ((int)sensor.Value >= 0)
                                            {
                                                return (int)sensor.Value;
                                            }
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
                int value = LoadInternal();
                if (value >= 0)
                {
                    return value;
                }
                else
                {
                    return LoadInternal8();
                }
                return -1;
            }
        }

        private int MemLoadInternal()
        {
            var aDLPMLogDataOutput = new ADLPMLogDataOutput();
            try
            {
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
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
        }
        public override float MemLoad
        {
            get
            {
                if (ConfigManager.GeneralConfig.DisableMonitoringAMD)
                {
                    return 0;
                }
                return MemLoadInternal();
            }
        }

        private double PowerUsageInternal()
        {
            double addAMD = ConfigManager.GeneralConfig.PowerAddAMD;
            var power = -1;
            if (_adlContext != IntPtr.Zero && ADL.ADL2_Overdrive6_CurrentPower_Get != null)
            {
                var result = ADL.ADL2_Overdrive6_CurrentPower_Get(_adlContext, _adapterIndex2, 0, ref power); //0
                if (result == ADL.ADL_SUCCESS)
                {
                    //Helpers.ConsolePrint("PowerUsageInternal1", "(power / (1 << 8)) + addAMD: " + ((power / (1 << 8)) + addAMD).ToString());
                    //return power;
                    return (double)(power / (1 << 8)) + addAMD;
                }
            }
            return -1;
        }
        private double PowerUsageInternal8()
        {
            var aDLPMLogDataOutput = new ADLPMLogDataOutput();
            double addAMD = ConfigManager.GeneralConfig.PowerAddAMD;
            int power = -1;
            try
            {
                var result = ADL.ADL2_New_QueryPMLogData_Get(_adlContext, _adapterIndex2, ref aDLPMLogDataOutput);
                if (result == ADL.ADL_SUCCESS)
                {
                    int i = (int)ADLSensorType.PMLOG_ASIC_POWER;
                    if (i < aDLPMLogDataOutput.sensors.Length && aDLPMLogDataOutput.sensors[i].supported != 0)
                    {
                        power = aDLPMLogDataOutput.sensors[i].value;
                        return (double)power + addAMD;
                    }
                }
                
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
            return -1;
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
                    int valuePowerUsage = (int)PowerUsageInternal();
                    if (valuePowerUsage > 0)
                    {
                        return valuePowerUsage;
                    } else
                    {
                        return PowerUsageInternal8();
                    }
                }
                else
                {
                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            /*
                            Helpers.ConsolePrint("*********", hardware.Identifier.ToString());
                            Helpers.ConsolePrint("*********", "Hardware: " + hardware.Name);
                            foreach (IHardware subhardware in hardware.SubHardware)
                            {
                                Helpers.ConsolePrint("*********", "\tSubhardware: " + subhardware.Name);
                                foreach (ISensor sensor in subhardware.Sensors)
                                {
                                    Helpers.ConsolePrint("*********", "\t\tSensor: " + sensor.Name + " Value: " + sensor.Value);
                                }
                            }
                            foreach (ISensor sensor in hardware.Sensors)
                            {
                                Helpers.ConsolePrint("*********", "\tSensor: " + sensor.Name + " Value: " + sensor.Value);
                            }
                            */

                            if (hardware.HardwareType == HardwareType.GpuAmd)
                            {
                                int.TryParse(hardware.Identifier.ToString().Replace("/gpu-amd/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.Name.ToLower().Contains("gpu package") & sensor.SensorType == SensorType.Power)
                                        {
                                            if ((int)sensor.Value >= 0)
                                            {
                                                return (int)sensor.Value + addAMD;
                                            }
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
                int value = (int)PowerUsageInternal();
                if (value > 0)
                {
                    return value;
                }
                else
                {
                    return PowerUsageInternal8();
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
