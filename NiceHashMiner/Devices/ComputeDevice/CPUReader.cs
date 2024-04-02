using LibreHardwareMonitor.Hardware;
using NiceHashMiner;
using System;
using System.Collections.Generic;

namespace ComputeDeviceCPU
{
    public class CpuReader
    {
        public static int GetTemperaturesInCelsius()
        {
            int _ret = -1;
            if (Form_Main.thisComputer is not object) return -1;
            if (Form_Main.thisComputer.Hardware == null) return -1;

            foreach (var hardware in Form_Main.thisComputer.Hardware)
            {
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                    {
                        if (sensor.Name == "CPU Package" || sensor.Name.Contains("Core ("))
                        {
                            _ret = (int)sensor.Value.Value;
                            break;
                        }
                    }
                }
            }
            return _ret;
        }

        public static int GetPower()
        {
            int _ret = -1;
            if (Form_Main.thisComputer is not object) return -1;
            if (Form_Main.thisComputer.Hardware == null) return -1;
            try
            {
                foreach (var hardware in Form_Main.thisComputer.Hardware)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Power && sensor.Value.HasValue)
                        {
                            if (sensor.Name == "CPU Package" || sensor.Name == "Package")
                            {
                                _ret = (int)sensor.Value;
                                break;
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {

            }
            return _ret;
        }

        public static int GetFan()
        {
            int _ret = -1;
            if (Form_Main.thisComputer is not object) return -1;
            if (Form_Main.thisComputer.Hardware == null) return -1;
            foreach (var hardware in Form_Main.thisComputer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Motherboard)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.SubHardware)
                    {
                        sensor.Update();
                        if (sensor.HardwareType == HardwareType.SuperIO)
                        {
                            foreach (var superio in hardware.SubHardware)
                            {
                                superio.Update();
                                foreach (var sens2 in superio.Sensors)
                                {
                                    if (sens2.SensorType == SensorType.Fan)
                                    {
                                        if (sens2.Name == "Fan #1" || sens2.Name == "Fan #2" || sens2.Name == "CPU Fan")
                                        {
                                            _ret = (int)sens2.Value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return _ret;
        }
        public static int GetLoad()
        {
            int _ret = -1;
            if (Form_Main.thisComputer is not object) return -1;
            if (Form_Main.thisComputer.Hardware == null) return -1;
            try
            {
                foreach (var hardware in Form_Main.thisComputer.Hardware)
                {
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
                        {
                            if (sensor.Name == "Package" || sensor.Name == "CPU Total")
                            {
                                _ret = (int)sensor.Value.Value;
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {

            }
            return _ret;
        }
    }

}
