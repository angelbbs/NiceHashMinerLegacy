using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Stats
{
    [Serializable]
    public class Root
    {
        public string platform = "NiceHash";
        public string Version = "";
        public string Wallet;
        public string Worker;
        public string RigStatus;
        public int MiningLocation;
        public string Currency;
        public DateTime RigDateTime;
        public long RigDateTimeUnix;
        public TimeSpan Uptime;
        public long UptimeSeconds;

        public double Rate_mBTC;
        public double Rate_Fiat;
        public double Balance_mBTC;
        public double Balance_Fiat;
        public int Power;
        public double TotalPower;
        public double PowerSpent_mBTC;
        public double PowerSpent_Fiat;
        public double TotalPowerSpent_mBTC;
        public double TotalPowerSpent_Fiat;
        public double BTCcurrencyRate;
        
        public List<Device> MiningDevices = new List<Device>();
    }

    [Serializable]
    public class Device
    {
        public string Name = "NONE";
        public string DeviceType = "NONE";
        public string Manufacturer = "NONE";
        public long GpuRam { get; set; } = -1;
        public int PlatformNum { get; set; } = -1;
        public string Codename { get; set; } = "NONE";
        public string UUID { get; set; } = "NONE";
        public string DevUUID { get; set; } = "NONE";
        public int ID { get; set; } = -1;
        public int Index { get; set; } = -1;
        public int BusID { get; set; } = -1;
        public bool Enabled { get; set; } = false;
        public bool MonitorConnected { get; set; } = false;
        public bool NvidiaLHR { get; set; } = false;
        public int AlgorithmID { get; set; } = -1;
        public string Algorithm { get; set; } = "NONE";
        public string MinerName { get; set; } = "NONE";
        public string MinerVersion { get; set; } = "NONE";
        public double MiningHashrate { get; set; } = -1;
        public double MiningHashrateSecond { get; set; } = -1;
        public int Temp { get; set; } = -1;
        public int TempMemory { get; set; } = -1;
        public int Load { get; set; } = -1;
        public int MemLoad { get; set; } = -1;
        public int Fan { get; set; } = -1;
        public int FanRPM { get; set; } = -1;
    }


}
