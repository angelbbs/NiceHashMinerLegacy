using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace NiceHashMiner.Miners
{
    public class ClaymoreDual : ClaymoreBaseMiner
    {
        public ClaymoreDual(AlgorithmType secondaryAlgorithmType)
            : base("ClaymoreDual")
        {
            IgnoreZero = true;
            ApiReadMult = 1000;
            ConectionType = NhmConectionType.STRATUM_TCP;
            SecondaryAlgorithmType = secondaryAlgorithmType;

            LookForStart = "eth - total speed:";
            SecondaryLookForStart = SecondaryShortName() + " - total speed:";
            IsMultiType = true;
        }
        // the short form the miner uses for secondary algo in cmd line and log
        public string SecondaryShortName()
        {
            return "";
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 120 * 1000;
        }

        private string GetStartCommand(string url, string btcAdress, string worker)
        {
            var username = GetUsername(btcAdress, worker);
            var dual = AlgorithmType.NONE;
            string poolport = "3354";
            var dualModeParams = "";
            string epoolsFile = "";
            string dpoolsFile = "";

            url = url.Replace("daggerhashimoto3gb", "daggerhashimoto");
            url = url.Replace("daggerhashimoto4gb", "daggerhashimoto");

            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    epoolsFile = "epoolsNV" + GetLogFileName().Replace("_log", "");
                    dpoolsFile = "dpoolsNV" + GetLogFileName().Replace("_log", "");
                }
                else
                {
                    epoolsFile = "epoolsAMD" + GetLogFileName().Replace("_log", "");
                    dpoolsFile = "dpoolsAMD" + GetLogFileName().Replace("_log", "");
                }
            }
            try
            {
                if (File.Exists("miners\\claymore_dual\\epools.txt"))
                    File.Delete("miners\\claymore_dual\\epools.txt");
                if (File.Exists("miners\\claymore_dual\\dpools.txt"))
                    File.Delete("miners\\claymore_dual\\dpools.txt");

                if (File.Exists("miners\\claymore_dual\\" + epoolsFile))
                    File.Delete("miners\\claymore_dual\\" + epoolsFile);
                if (File.Exists("miners\\claymore_dual\\" + dpoolsFile))
                    File.Delete("miners\\claymore_dual\\" + dpoolsFile);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetStartBenchmarkCommand", ex.ToString());
            }
            Thread.Sleep(200);

            List<string> ResolvedServers = MiningSession.GetResolvedServers("daggerhashimoto");
            String epools = String.Format("POOL: {0}:{1}, WALLET: {2}, PSW: x, ESM: 3, ALLPOOLS: 1", ResolvedServers[1], (ResolvedServers[1].Contains("auto") ? "9200" : "3353"), username) + "\n";
            try
            {
                FileStream fs = new FileStream("miners\\claymore_dual\\" + epoolsFile, FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(fs);
                w.WriteAsync(epools);
                w.Flush();
                w.Close();
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("GetStartCommand", e.ToString());
            }

            Thread.Sleep(200);
           
            if (!IsDual())
            {
                // leave convenience param for non-dual entry
                foreach (var pair in MiningSetup.MiningPairs)
                {
                    if (!pair.CurrentExtraLaunchParameters.Contains("-dual=")) continue;
                    dual = AlgorithmType.NONE;
                    var coinP = "";
                    
                    if (dual != AlgorithmType.NONE)
                    {
                        var urlSecond = Globals.GetLocationUrl(dual,
                            Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation],
                            ConectionType);
                        dualModeParams = $" {coinP} -dpool {urlSecond} -dwal {username}";
                        break;
                    }
                }
            }
            

            string addParam;
            bool isNvidia = false;
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    isNvidia = true;
                }
                else
                {
                    isNvidia = false;
                }
            }

            addParam = " "
                                    + GetDevicesCommandString()
                                    + String.Format("  -epool {0}:{1} -ewal {2} -mport 127.0.0.1:-{3} -esm 3 -epsw x -allpools 1 -ftime 10 -retrydelay 5 -checkcert 0", ResolvedServers[0], (ResolvedServers[0].Contains("auto.") ? "9200" : "3353"), username, ApiPort)
                                    + dualModeParams;
           
            return addParam + " -epoolsfile " + epoolsFile;
        }

        private string GetStartBenchmarkCommand(string url, string btcAdress, string worker)
        {
            var username = GetUsername(btcAdress, worker);
            string poolport = "3354";
            var dualModeParams = "";
            string epoolsFile = "";
            string dpoolsFile = "";

            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    epoolsFile = "epoolsNV" + GetLogFileName().Replace("_log", "");
                    dpoolsFile = "dpoolsNV" + GetLogFileName().Replace("_log", "");
                }
                else
                {
                    epoolsFile = "epoolsAMD" + GetLogFileName().Replace("_log", "");
                    dpoolsFile = "dpoolsAMD" + GetLogFileName().Replace("_log", "");
                }
            }
            try
            {
                if (File.Exists("miners\\claymore_dual\\epools.txt"))
                    File.Delete("miners\\claymore_dual\\epools.txt");
                if (File.Exists("miners\\claymore_dual\\dpools.txt"))
                    File.Delete("miners\\claymore_dual\\dpools.txt");

                if (File.Exists("miners\\claymore_dual\\" + epoolsFile))
                    File.Delete("miners\\claymore_dual\\" + epoolsFile);
                if (File.Exists("miners\\claymore_dual\\" + dpoolsFile))
                    File.Delete("miners\\claymore_dual\\" + dpoolsFile);
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("GetStartBenchmarkCommand", ex.ToString());
            }
            Thread.Sleep(200);

            List<string> ResolvedServers = MiningSession.GetResolvedServers("daggerhashimoto");
            String epools = String.Format("POOL: {0}:{1}, WALLET: {2}, PSW: x, ESM: 3, ALLPOOLS: 1", ResolvedServers[0], (ResolvedServers[0].Contains("auto") ? "9200" : "3353"), username) + "\n";
            try
            {
                FileStream fs = new FileStream("miners\\claymore_dual\\" + epoolsFile, FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(fs);
                w.WriteAsync(epools);
                w.Flush();
                w.Close();
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("GetStartCommand", e.ToString());
            }

            bool istuned = false;

            foreach (var mPair in MiningSetup.MiningPairs)
            {
                if (mPair.Algorithm is DualAlgorithm algo && algo.TuningEnabled)
                {
                    // var intensity = algo.MostProfitableIntensity;
                    // if (intensity < 0) intensity = defaultIntensity;
                    istuned = true;
                }
            }

            string addParam = "";
            bool isNvidia = false;
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    isNvidia = true;
                }
                else
                {
                    isNvidia = false;
                }
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto3GB))
            {

                addParam = " "
                                        + GetDevicesCommandString()
                                        + String.Format("  -epool " + Links.CheckDNS("stratum+tcp://us-east.ethash-hub.miningpoolhub.com") + ":20565 -ewal angelbbs.Claymore3 -esm 2 -mport 127.0.0.1:-{0} -epsw c=BTC -allcoins 1 -allpools 1 -ftime 10 -retrydelay 5", ApiPort)
                                        + dualModeParams;
            }
            else if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto4GB))
            {

                addParam = " "
                                        + GetDevicesCommandString()
                                        + String.Format("  -epool " + Links.CheckDNS("stratum+tcp://us-east.ethash-hub.miningpoolhub.com") + ":20565 -ewal angelbbs.Claymore4 -esm 2 -mport 127.0.0.1:-{0} -epsw c=BTC -allcoins 1 -allpools 1 -ftime 10 -retrydelay 5", ApiPort)
                                        + dualModeParams;
            }

            return addParam + " -epoolsfile " + epoolsFile;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            // Update to most profitable intensity
            foreach (var mPair in MiningSetup.MiningPairs)
            {
                if (mPair.Algorithm is DualAlgorithm algo && algo.TuningEnabled)
                {
                    var intensity = algo.MostProfitableIntensity;
                    if (intensity < 0) intensity = defaultIntensity;
                    algo.CurrentIntensity = intensity;
                }
            }

            LastCommandLine = GetStartCommand(url, btcAdress, worker) + " -dbg -1";
            /*
            if (IsDual())
            {
                strdual = "DUAL";
            }

            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    RunCMDBeforeMining("NVIDIA" + " " + strdual, true);
                }
                else if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    RunCMDBeforeMining("AMD" + " " + strdual, true);
                }
                else if (pair.Device.DeviceType == DeviceType.CPU)
                {
                    RunCMDBeforeMining("CPU", true);
                }
            }
*/
            ProcessHandle = _Start();
        }

        protected override string DeviceCommand(int amdCount = 1)
        {
            // If no AMD cards loaded, instruct CD to only regard NV cards for indexing
            // This will allow proper indexing if AMD GPUs or APUs are present in the system but detection disabled
            var ret = (amdCount == 0) ? " -platform 2" : "";
            return ret + base.DeviceCommand(amdCount);
        }

        // benchmark stuff
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }
        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            // network stub
            var url = GetServiceUrl(algorithm.NiceHashID);
            // demo for benchmark
            var ret = GetStartBenchmarkCommand(url, Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim())
                         + " -logfile " + GetLogFileName();
            //BenchmarkTimeWait = Math.Max(60, Math.Min(120, time * 3));
            BenchmarkTimeWait = time;
            return ret;
        }

    }
}
