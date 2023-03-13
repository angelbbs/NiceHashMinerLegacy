using System;

namespace NiceHashMiner.Devices
{
    [Serializable]
    public class IntelGpuDevice
    {
        public int DeviceID => (int)_openClSubset.DeviceID;
        public int BusID => (int)_openClSubset.BUS_ID;
        public string DeviceName; // init this with the ADL
        public string UUID; // init this with the ADL, use PCI_VEN & DEV IDs
        //public ulong DeviceGlobalMemory => _openClSubset._CL_DEVICE_GLOBAL_MEM_SIZE;
        public ulong DeviceGlobalMemory;
        public bool MonitorConnected;

        //public bool UseOptimizedVersion { get; private set; }
        private readonly OpenCLDevice _openClSubset = new OpenCLDevice();

        public readonly string InfSection; // has arhitecture string

        // new drivers make some algorithms unusable 21.19.164.1 => driver not working with NeoScrypt and
        public bool DriverDisableAlgos { get; }

        public string Codename => _openClSubset._CL_DEVICE_NAME;

        public string NewUUID { get; internal set; }
        public string IntelManufacturer = "";

        public int AdapterIndex; // init this with the ADL
        public long DeviceHandle; 

        public IntelGpuDevice(OpenCLDevice openClSubset, bool isOldDriver, string infSection, bool driverDisableAlgo)
        {
            DriverDisableAlgos = driverDisableAlgo;
            InfSection = infSection;
            if (openClSubset != null)
            {
                _openClSubset = openClSubset;
            }
            // Check for optimized version
            // first if not optimized
            Helpers.ConsolePrint("IntelGpuDevice", "List: " + _openClSubset._CL_DEVICE_NAME);
            
        }

        public bool IsEtherumCapable()
        {
            return _openClSubset._CL_DEVICE_GLOBAL_MEM_SIZE >= 5293918720;
        }
    }
}
