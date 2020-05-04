/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner;
using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeDeviceCPU
{
        public class CpuReader
        {
            private static readonly Computer _computer = new Computer { CPUEnabled = true };
            private static readonly Computer _mainboard = new Computer { MainboardEnabled = true };
            /*
            public static CpuTemperatureReader()
            {
                _computer = new Computer { CPUEnabled = true };
                _computer.Open();
            }
            */
            public static int GetTemperaturesInCelsius()
            {
                // _computer = new Computer { CPUEnabled = true };
                int _ret = -1;
                _computer.Open();
                var coreAndTemperature = new Dictionary<string, float>();

                foreach (var hardware in _computer.Hardware)
                {
                    hardware.Update(); //use hardware.Name to get CPU model
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                        {
                            //  if (sensor.Name == "Package")
                            {
                                _ret = (int)sensor.Value.Value;
                            }
                        }
                    }
                }

                return _ret;
            }

        public static int GetPower()
        {
            // _computer = new Computer { CPUEnabled = true };
            int _ret = -1;
            _computer.Open();
            var coreAndTemperature = new Dictionary<string, float>();

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update(); //use hardware.Name to get CPU model
                foreach (var sensor in hardware.Sensors)
                {
                    //Helpers.ConsolePrint("CPU", sensor.Name + " " + sensor.Value.ToString());
                    if (sensor.SensorType == SensorType.Power && sensor.Value.HasValue)
                    {
                       // Helpers.ConsolePrint("CPU", sensor.Name + " " + sensor.Value.ToString());
                          if (sensor.Name == "CPU Package")
                        {
                            _ret = (int)sensor.Value;
                        }
                    }
                }
            }

            return _ret;
        }

        public static int GetFan()
        {
            // _computer = new Computer { CPUEnabled = true };
            int _ret = -1;
            _mainboard.Open();
            var coreAndTemperature = new Dictionary<string, float>();

            foreach (var hardware in _mainboard.Hardware)
            {
                //hardware.Update(); //use hardware.Name to get CPU model
                
                if (hardware.HardwareType == HardwareType.Mainboard)
                {
                    hardware.Update();
                    //  Helpers.ConsolePrint("!all CPU:", hardware.Name + " " + hardware.HardwareType.ToString());

                    foreach (var sensor in hardware.SubHardware)
                    {
                        sensor.Update();
                      //  Helpers.ConsolePrint("all CPU:", sensor.Name + " " + HardwareType.SuperIO.ToString());
                        
                        if (sensor.HardwareType == HardwareType.SuperIO)
                        {
                            foreach (var superio in hardware.SubHardware)
                            {
                                superio.Update();
                                foreach (var sens2 in superio.Sensors)
                                {
                                    if (sens2.SensorType == SensorType.Fan)
                                    {
                                        if (sens2.Name == "Fan #1" || sens2.Name == "CPU Fan")
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
            // _computer = new Computer { CPUEnabled = true };
            int _ret = -1;
            _computer.Open();
            var coreAndTemperature = new Dictionary<string, float>();

            foreach (var hardware in _computer.Hardware)
            {
                hardware.Update(); //use hardware.Name to get CPU model
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
                    {
                       // Helpers.ConsolePrint("CPU", sensor.Name + " " + sensor.Value.ToString());
                        // if (sensor.Name == "Package")
                        {
                            _ret = (int)sensor.Value.Value;
                        }
                    }
                }
            }
            return _ret;
        }

        public void Dispose()
            {
                try
                {
                    _computer.Close();
                }
                catch (Exception)
                {
                    //ignore closing errors
                }
            }
        }
    
}
