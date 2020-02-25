/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Configs.Data
{
    [Serializable]
    public class AlgorithmConfig
    {
        public string Name = ""; // Used as an indicator for easier user interaction
        public AlgorithmType NiceHashID = AlgorithmType.NONE;
        public AlgorithmType SecondaryNiceHashID = AlgorithmType.NONE;
        public MinerBaseType MinerBaseType = MinerBaseType.NONE;
        public string MinerName = ""; // probably not needed
        public double BenchmarkSpeed = 0;
        public double SecondaryBenchmarkSpeed = 0;
        public string ExtraLaunchParameters = "";
        public bool Enabled = true;
        public bool Hidden = false;
        public int LessThreads = 0;
        public double PowerUsage = 0;
        public bool OverClockEnabled = false;
        public int Amd1 = 0;
        public int Amd2 = 0;
        public int Amd3 = 0;
        public int Amd4 = 0;
        public int Amd5 = 0;
    }
}
