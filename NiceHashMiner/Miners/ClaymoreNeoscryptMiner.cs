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
            List<string> ResolvedServers = MiningSession.GetResolvedServers("neoscrypt");
            string username = GetUsername(btcAdress, worker);
            url = url.Replace("stratum+ssl", "stratum+tcp").Replace("33341", "3341");
            LastCommandLine = " " + GetDevicesCommandString() + " -mport -" + ApiPort + " -pool " + ResolvedServers[0] + ":" + (ResolvedServers[0].Contains("auto.") ? "9200" : "3353") +
                                  " -wal " + username + " -psw x -dbg -1 -ftime 10 -retrydelay 5";

            String epools = String.Format("POOL: {0}:{1}, WALLET: {2}, PSW: x", ResolvedServers[1], (ResolvedServers[1].Contains("auto") ? "9200" : "3341"), username) + "\n"
               + String.Format("POOL: {0}:{1}, WALLET: {2}, PSW: x", ResolvedServers[2], (ResolvedServers[2].Contains("auto") ? "9200" : "3341"), username) + "\n"
               + String.Format("POOL: {0}:{1}, WALLET: {2}, PSW: x", ResolvedServers[3], (ResolvedServers[3].Contains("auto") ? "9200" : "3341"), username) + "\n";

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
