using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Stats
{
    //JSON API пока не будет
    [Serializable]
    public class JsonClass
    {
        public string platform = "nicehash";
        public List<Devices> MiningDevices = new List<Devices>();
        public string Status { get; set; }
    }

    [Serializable]
    public class Devices
    {
        public string Name = "NONE";
        public int PlatformNum { get; set; } = -1;
        public string PlatformVendor { get; set; } = "NONE";
    }


}
