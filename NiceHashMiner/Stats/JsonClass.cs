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
        public int SecondaryAlgorithmID { get; set; } = -1;
        public string SecondaryAlgorithm { get; set; } = "NONE";
        public bool IsDualAlgorithm { get; set; } = false;
        public string MinerName { get; set; } = "NONE";
        public string MinerVersion { get; set; } = "NONE";
        public double MiningHashrate { get; set; } = -1;
        public string DescHashrate { get; set; } = "";
        public double MiningHashrateSecond { get; set; } = -1;
        public string DescHashrateSecond { get; set; } = "";
        public int Temp { get; set; } = -1;
        public int TempMemory { get; set; } = -1;
        public int Load { get; set; } = -1;
        public int MemLoad { get; set; } = -1;
        public int Fan { get; set; } = -1;
        public int FanRPM { get; set; } = -1;
    }

    //NHM interfaces
    internal interface IMethod
    {
        string Method { get; }
    }

    public interface ICoreClock
    {
        int CoreClock { get; }
    }
    public interface IMemoryClock
    {
        int MemoryClock { get; }
    }
    public enum TDPSettingType
    {
        UNSUPPORTED,
        DISABLED,
        SIMPLE,
        PERCENTAGE
    }
    public enum TDPSimpleType
    {
        LOW,
        MEDIUM,
        HIGH
    }

    public interface ITDP
    {
        TDPSettingType SettingType { get; set; }

        double TDPPercentage { get; }
        bool SetTDP(double percentage);
        TDPSimpleType TDPSimple { get; }
        bool SetTDPSimple(TDPSimpleType level);
    }
    public interface ITDPWatts
    {
        int TDPWatts { get; }
    }
    public interface ICoreVoltage
    {
        int CoreVoltage { get; }
    }

    public interface ILoad
    {
        float Load { get;  }
    }
    public interface IFanSpeedRPM
    {
        int FanSpeedRPM { get; }
    }
    public interface IGetFanSpeedPercentage
    {
        (int status, int percentage) GetFanSpeedPercentage();
    }
    public interface IMemControllerLoad
    {
        int MemoryControllerLoad { get; }
    }
    public interface IPowerUsage
    {
        double PowerUsage { get; }
    }
    public interface ISpecialTemps : IVramTemp, IHotspotTemp
    { }
    public interface IVramTemp
    {
        int VramTemp { get; }
    }
    public interface IHotspotTemp
    {
        int HotspotTemp { get; }
    }
    public interface ITemp
    {
        float Temp { get; }
    }


    public enum DeviceDynamicProperties
    {
        NONE,
        Load,
        MemoryControllerLoad,
        Temperature,
        FanSpeedPercentage,
        PowerUsage,
        VramTemp,
        //HotspotTemp,
        //CoreClock,
        //MemClock,
        //TDP,
        //TDPWatts,
        //CoreVoltage,
        //CoreClockDelta,
        //MemClockDelta,
        FanSpeedRPM
    }

    internal interface ISendMessage
    {
    }
}
