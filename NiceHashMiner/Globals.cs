/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using Newtonsoft.Json;
using System.Collections.Generic;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner
{
    public class Globals
    {
        // Constants
        public static string[] MiningLocation = {"eu", "usa", "hk", "jp", "in", "br", "Auto"};

        //public static readonly string DemoUser = "3LysVG8rv8gzcYVAqkqLUzjSJSrE6pATJB";
        public static readonly string DemoUser = "38GGAkeaa4qm799ZKg3YsoEMpiEHhh7dE4";
        public static readonly string DemoUserNew = "38GGAkeaa4qm799ZKg3YsoEMpiEHhh7dE4";

        // change this if TOS changes
        public static int CurrentTosVer = 4;

        // Variables
        public static JsonSerializerSettings JsonSettings = null;

        public static int ThreadsPerCpu;

        // quickfix guard for checking internet conection
        public static bool IsFirstNetworkCheckTimeout = true;

        public static int FirstNetworkCheckTimeoutTimeMs = 500;
        public static int FirstNetworkCheckTimeoutTries = 5;


        public static string GetLocationUrl(AlgorithmType algorithmType, string miningLocation, NhmConectionType conectionType)
        {
            if (!NHSmaData.TryGetSma(algorithmType, out var sma)) return "";

            var name = sma.Name;
            var nPort = sma.Port;
            var sslPort = 30000 + nPort;

            // NHMConectionType.NONE
            var prefix = "";
            var port = nPort;
            if (miningLocation.Contains("Auto"))
            {
                miningLocation = miningLocation.Replace("Auto", "eu");
            }
            switch (conectionType)
            {
                case NhmConectionType.LOCKED:
                    return miningLocation;
                case NhmConectionType.STRATUM_TCP:
                    prefix = "stratum+tcp://";
                    break;
                case NhmConectionType.STRATUM_SSL:
                    prefix = "stratum+ssl://";
                    port = sslPort;
                    break;
            }
            var nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
                return prefix
                   + name
                   + "." + miningLocation
                   + nhsuff + ".nicehash.com:"
                   + port;

        }

        public static string GetBitcoinUser()
        {
                return BitcoinAddress.ValidateBitcoinAddress(Configs.ConfigManager.GeneralConfig.BitcoinAddressNew.Trim())
                    ? Configs.ConfigManager.GeneralConfig.BitcoinAddressNew.Trim()
                    : DemoUserNew;

        }
    }
}
