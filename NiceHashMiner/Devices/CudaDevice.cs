using System;
using System.Collections.Generic;

namespace NiceHashMiner.Devices
{
    [Serializable]
    public class CudaDevicesList
    {
        public List<CudaDevices2> CudaDevices = new List<CudaDevices2>();
        public string DriverVersion = "NONE";
        public string ErrorString = "NONE";
    }

    [Serializable]
    public class CudaDevices2
    {
        public ulong DeviceGlobalMemory;
        public uint DeviceID;
        public string DeviceName;
        public string CUDAManufacturer;
        public int HasMonitorConnected;
        public bool MonitorConnected;
        public bool NvidiaLHR;
        public int SMX;
        public int SM_major;
        public int SM_minor;
        public string UUID;
        public int VendorID;
        public string VendorName;
        public int pciBusID;
        public uint pciDeviceId; //!< The combined 16-bit device id and 16-bit vendor id
        public uint pciSubSystemId; //!< The 32-bit Sub System Device ID

        public string GetName()
        {

            if (VendorName == "UNKNOWN")
            {
                VendorName = string.Format(International.GetText("ComputeDevice_UNKNOWN_VENDOR_REPLACE"), VendorID);
            }

            //return $"{VendorName} {DeviceName}";
            return $"{DeviceName}";
        }

        public bool IsEtherumCapable()
        {
            // exception devices
            if (DeviceName.Contains("750") && DeviceName.Contains("Ti"))
            {
                Helpers.ConsolePrint("CudaDevice",
                    "GTX 750Ti found! By default this device will be disabled for ethereum as it is generally too slow to mine on it.");
                return false;
            }
            return DeviceGlobalMemory >= 5293918720 && SM_major >= 3;
        }
    }
}
