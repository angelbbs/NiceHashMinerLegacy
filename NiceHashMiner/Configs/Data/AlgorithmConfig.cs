using NiceHashMinerLegacy.Common.Enums;
using System;

namespace NiceHashMiner.Configs.Data
{
    [Serializable]
    public class AlgorithmConfig
    {
        public string Name = ""; // Used as an indicator for easier user interaction
        public AlgorithmType NiceHashID = AlgorithmType.NONE;
        public AlgorithmType SecondaryNiceHashID = AlgorithmType.NONE;
        public MinerBaseType MinerBaseType = MinerBaseType.NONE;
        public string AlgorithmNameCustom = "";
        public double BenchmarkSpeed = 0;
        public double BenchmarkSecondarySpeed = 0;
        public string ExtraLaunchParameters = "";
        public bool Enabled = true;
        public bool Hidden = false;
        public bool Forced = false;
        public int LessThreads = 0;
        public double PowerUsage = 0;
        //configs/benchmark_...json
        //public int gpu_clock = 0;
        //public int mem_clock = 0;
        //public double gpu_voltage = 0.0d;
        //public double mem_voltage = 0.0d;
        //public int power_limit = 0;
        //public int fan = 0;
        //public int fan_flag = 0;
        //public int thermal_limit = 0;
    }
}
