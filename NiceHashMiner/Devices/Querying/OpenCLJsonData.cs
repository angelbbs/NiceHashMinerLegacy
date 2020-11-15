/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Devices.Querying
{
    [Serializable]
    public class OpenCLPlatform
    {
        public List<OpenCLDevice> Devices = new List<OpenCLDevice>();
        public string PlatformName { get; set; } = "NONE";
        public int PlatformNum { get; set; } = -1;
        public string PlatformVendor { get; set; } = "NONE";
    }

    [Serializable]
    public class OpenCLJsonData
    {
        public string ErrorString = "NONE";
        public List<OpenCLPlatform> Platforms = new List<OpenCLPlatform>();
        public string Status { get; set; }

    }
}
