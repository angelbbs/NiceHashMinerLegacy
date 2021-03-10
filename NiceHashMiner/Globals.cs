using Newtonsoft.Json;
using System.Collections.Generic;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner
{
    public class Globals
    {
        // Constants
        public static string[] MiningLocation = {"eu", "eu-north", "usa", "usa-east", "Auto"};

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
                //miningLocation = miningLocation.Replace("Auto", "eu");
                miningLocation = Form_Main.myServers[0, 0];
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
                return prefix
                   + name
                   + "." + miningLocation
                   + ".nicehash.com:"
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
