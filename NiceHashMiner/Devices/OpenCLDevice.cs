/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;

namespace NiceHashMiner.Devices
{
    [Serializable]
    public class OpenCLDevice
    {
        public int BUS_ID { get; set; } = -1; // -1 indicates that it is not set
        public uint DeviceID { get; set; }
        public ulong _CL_DEVICE_GLOBAL_MEM_SIZE { get; set; } = 0;
        public string _CL_DEVICE_NAME { get; set; }
        public string _CL_DEVICE_TYPE { get; set; }
        public string _CL_DEVICE_VENDOR { get; set; }
        public uint _CL_DEVICE_VENDOR_ID { get; set; }
        public string _CL_DEVICE_VERSION { get; set; }
        public string _CL_DRIVER_VERSION { get; set; }
        public string _CL_DEVICE_BOARD_NAME_AMD { get; set; }

    }
}
