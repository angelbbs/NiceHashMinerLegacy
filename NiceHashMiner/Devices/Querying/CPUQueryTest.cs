using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Devices.Querying
{
    public class CPUQueryTest
    {
        public static void Monitor()
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };

            computer.Open();
            //computer.Accept(new UpdateVisitor());

            foreach (IHardware hardware in computer.Hardware)
            {
                Helpers.ConsolePrint("CPUQueryTest", "Hardware Name: " + hardware.Name +
                            ", HardwareType: " + hardware.HardwareType);

                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    Helpers.ConsolePrint("CPUQueryTest", "\tSubhardware: " + subhardware.Name +
                        ", SubhardwareType: " + subhardware.HardwareType);

                    foreach (ISensor sensor in subhardware.Sensors)
                    {
                        Helpers.ConsolePrint("CPUQueryTest", "\t\tSensor Name: " + sensor.Name +
                            ", Sensor SensorType: " + sensor.SensorType + ", Sensor Value: " + sensor.Value);
                    }
                }

                foreach (ISensor sensor in hardware.Sensors)
                {
                    Helpers.ConsolePrint("CPUQueryTest", "\tSensor Name: " + sensor.Name +
                            ", Sensor SensorType: " + sensor.SensorType + ", Sensor Value: " + sensor.Value);
                }
            }

            computer.Close();
        }
    }
}
