/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using ATI.ADL;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Configs;
using OpenHardwareMonitor.Hardware;
using NiceHashMiner.Miners.Grouping;

namespace NiceHashMiner.Devices
{
    public class AmdComputeDevice : ComputeDevice
    {
        private readonly int _adapterIndex; // For ADL
        private readonly int _adapterIndex2; // For ADL2
        private readonly IntPtr _adlContext;

        public override int FanSpeed
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
                    if (ConfigManager.GeneralConfig.ShowFanAsPercent)
                    {
                        adlf = new ADLFanSpeedValue
                        {
                            SpeedType = ADL.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
                        };
                    }
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
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAti)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/atigpu/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (ConfigManager.GeneralConfig.ShowFanAsPercent)
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
                                        else
                                        {
                                            if (sensor.SensorType == SensorType.Fan)
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
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAti)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/atigpu/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
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
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {
                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAti)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/atigpu/", ""), out var gpuId);
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
                        }
                    }
                }
                else
                {
                    try
                    {
                        foreach (var hardware in Form_Main.thisComputer.Hardware)
                        {

                            //hardware.Update();
                            if (hardware.HardwareType == HardwareType.GpuAti)
                            {
                                //hardware.Update();
                                int.TryParse(hardware.Identifier.ToString().Replace("/atigpu/", ""), out var gpuId);
                                if (gpuId == _adapterIndex)
                                {
                                    foreach (var sensor in hardware.Sensors)
                                    {
                                        if (sensor.SensorType == SensorType.Power)
                                        {
                                            if ((int)sensor.Value >= 0)
                                            {
                                                return (int)sensor.Value + addAMD;
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
                amdDevice.DeviceGlobalMemory, amdDevice.AMDManufacturer)
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
