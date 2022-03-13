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

        public override void Start(string url, string btcAdress, string worker)
        {
            string username = GetUsername(btcAdress, worker);
            url = url.Replace("stratum+ssl", "stratum+tcp").Replace("33341", "3341");
            LastCommandLine = " " + GetDevicesCommandString() + " -mport -" + ApiPort + " -pool " + Links.CheckDNS(url) +
                                  " -wal " + username + " -psw x -dbg -1 -ftime 10 -retrydelay 5";

            List<string> ResolvedServers = MiningSession.GetResolvedServers("daggerhashimoto");
            String epools = String.Format("POOL: {0}:3341, WALLET: {1}, PSW: x", ResolvedServers[1], username) + "\n"
               + String.Format("POOL: {0}:3341, WALLET: {1}, PSW: x", ResolvedServers[2], username) + "\n"
               + String.Format("POOL: {0}:3341, WALLET: {1}, PSW: x", ResolvedServers[0], username) + "\n";

            FileStream fs = new FileStream("miners\\claymore_neoscrypt\\pools.txt", FileMode.Create, FileAccess.Write);
            StreamWriter w = new StreamWriter(fs);
            w.WriteAsync(epools);
            w.Flush();
            w.Close();
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

            // network workaround
            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], NhmConectionType.STRATUM_TCP);
            // demo for benchmark
            string username = Globals.GetBitcoinUser();
            if (ConfigManager.GeneralConfig.WorkerName.Length > 0)
                username += "." + ConfigManager.GeneralConfig.WorkerName.Trim();

            return $" {GetDevicesCommandString()} -mport -{ApiPort} -pool " + Links.CheckDNS("stratum+tcp://neoscrypt.eu.mine.zpool.ca") + ":4233 -wal 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 -psw c=BTC -logfile " + GetLogFileName();
        }

    }
}
