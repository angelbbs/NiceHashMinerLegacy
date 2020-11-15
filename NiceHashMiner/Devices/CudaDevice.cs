/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
        public int NvmlInitialized = -1;
        public int NvmlLoaded = -1;
    }

    [Serializable]
    public class CudaDevices2
    {
        public ulong DeviceGlobalMemory;
        public uint DeviceID;
        public string DeviceName;
        public int HasMonitorConnected;
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
            return $"{VendorName} {DeviceName}";
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
            return DeviceGlobalMemory >= ComputeDevice.Memory4Gb && SM_major >= 3;
        }
    }
}
