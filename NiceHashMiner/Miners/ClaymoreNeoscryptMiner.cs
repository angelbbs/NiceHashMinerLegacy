using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace NiceHashMiner.Miners
{
    public class ClaymoreNeoscryptMiner : ClaymoreBaseMiner
    {
        public ClaymoreNeoscryptMiner()
            : base("ClaymoreNeoscryptMiner")
        {
            LookForStart = "ns - total speed:";
        }

        private string GetServer(string algo, string username, string port)
        {
            string ret = "";
            string ssl = "";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "1" + port;//не подключается почему-то
                ssl = "stratum+tcp://";
            }
            else
            {
                port = "1" + port;
                ssl = "stratum+tcp://";
            }

            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " -pool " + ssl + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 ";
                    break;
                }
                else
                {
                    ret = ret + " -pool " + ssl + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " ";
                    break;
                }
            }
            return ret;
        }
        public override void Start(string btcAdress, string worker)
        {
            string psw = "x";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            string username = GetUsername(btcAdress, worker);
            LastCommandLine = " " + GetDevicesCommandString() + " -mport -" + ApiPort +
                GetServer("neoscrypt", username, "3341") +
                " -wal " + username + " -psw " + psw + " -dbg -1 -ftime 10 -retrydelay 5";

            ProcessHandle = _Start();
        }

        // benchmark stuff
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }
        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            BenchmarkTimeWait = time;
            // demo for benchmark
            string username = Globals.GetBitcoinUser();
            if (ConfigManager.GeneralConfig.WorkerName.Length > 0)
                username += "." + ConfigManager.GeneralConfig.WorkerName.Trim();

            return $" {GetDevicesCommandString()} -mport -{ApiPort} -pool " + Links.CheckDNS("stratum+tcp://neoscrypt.eu.mine.zpool.ca") + ":4233 -wal 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 -psw c=BTC -logfile " + GetLogFileName();
        }

    }
}
