/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner.Configs;
using System;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
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
            DevFee = IsDual() ? 1.5 : 1.0;

            IsMultiType = true;
        }
        private string[,] myServers = Form_Main.myServers;
        // the short form the miner uses for secondary algo in cmd line and log
        public string SecondaryShortName()
        {
            switch (SecondaryAlgorithmType)
            {
                case AlgorithmType.Decred:
                    return "dcr";
                case AlgorithmType.Lbry:
                    return "lbc";
                case AlgorithmType.Pascal:
                    return "pasc";
                case AlgorithmType.Sia:
                    return "sc";
                case AlgorithmType.Blake2s:
                    return "b2s";
                case AlgorithmType.Keccak:
                    return "kc";
            }

            return "";
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 120 * 1000; 
        }

        private string GetStartCommand(string url, string btcAdress, string worker)
        {
            var username = GetUsername(btcAdress, worker);
            // AlgorithmType alg = AlgorithmType.Lbry;
            var dual = AlgorithmType.NONE;
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
                } else
                {
                    epoolsFile = "epoolsAMD" + GetLogFileName().Replace("_log", "");
                    dpoolsFile = "dpoolsAMD" + GetLogFileName().Replace("_log", "");
                }
            }

            if (File.Exists("bin_3rdparty\\claymore_dual\\epools.txt"))
                File.Delete("bin_3rdparty\\claymore_dual\\epools.txt");
            if (File.Exists("bin_3rdparty\\claymore_dual\\dpools.txt"))
                File.Delete("bin_3rdparty\\claymore_dual\\dpools.txt");

            if (File.Exists("bin_3rdparty\\claymore_dual\\"+ epoolsFile))
                File.Delete("bin_3rdparty\\claymore_dual\\"+ epoolsFile);
            if (File.Exists("bin_3rdparty\\claymore_dual\\" + dpoolsFile))
                File.Delete("bin_3rdparty\\claymore_dual\\" + dpoolsFile);

            Thread.Sleep(200);

            String epools = String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[1, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[2, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[3, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[4, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[5, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[0, 0], username) + "\n";
            try
            {
                FileStream fs = new FileStream("bin_3rdparty\\claymore_dual\\"+ epoolsFile, FileMode.Create, FileAccess.Write);
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

                Thread.Sleep(200);
            if (SecondaryAlgorithmType == AlgorithmType.Decred)
            {
                poolport = "3354";
            }
            if (SecondaryAlgorithmType == AlgorithmType.Lbry)
            {
                poolport = "3356";
            }

            if (SecondaryAlgorithmType == AlgorithmType.Blake2s)
            {
                poolport = "3361";
            }
            if (SecondaryAlgorithmType == AlgorithmType.Keccak)
            {
                poolport = "3338";
            }

            if (SecondaryAlgorithmType == AlgorithmType.Pascal)
            {
                poolport = "3358";
            }
            if (SecondaryAlgorithmType == AlgorithmType.Sia)
            {
                poolport = "3360";
            }
            if (!IsDual())
            {
                // leave convenience param for non-dual entry
                foreach (var pair in MiningSetup.MiningPairs)
                {
                    if (!pair.CurrentExtraLaunchParameters.Contains("-dual=")) continue;
                    dual = AlgorithmType.NONE;
                    var coinP = "";
                    if (pair.CurrentExtraLaunchParameters.Contains("Decred"))
                    {
                        dual = AlgorithmType.Decred;
                        coinP = " -dcoin dcr ";
                    }

                    if (pair.CurrentExtraLaunchParameters.Contains("Siacoin"))
                    {
                        dual = AlgorithmType.Sia;
                        coinP = " -dcoin sc";
                    }

                    if (pair.CurrentExtraLaunchParameters.Contains("Lbry"))
                    {
                        dual = AlgorithmType.Lbry;
                        coinP = " -dcoin lbc ";
                    }

                    if (pair.CurrentExtraLaunchParameters.Contains("Pascal"))
                    {
                        dual = AlgorithmType.Pascal;
                        coinP = " -dcoin pasc ";
                    }

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
            else //dual
            {
                String dpools = "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[0, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[1, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[2, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[3, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[4, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[5, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x", username) + "\n";
                try
                {
                    FileStream fs1 = new FileStream("bin_3rdparty\\claymore_dual\\"+dpoolsFile, FileMode.Create, FileAccess.Write);
                    StreamWriter w1 = new StreamWriter(fs1);
                    w1.WriteAsync(dpools);
                    w1.Flush();
                    w1.Close();
                    Thread.Sleep(200);
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("GetStartCommand", e.ToString());
                }

                var urlSecond = Globals.GetLocationUrl(SecondaryAlgorithmType,
                    Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], ConectionType);
                dualModeParams = $" -dcoin {SecondaryShortName()} -dpool {urlSecond} -dwal {username} -dpsw x -dpoolsfile "+dpoolsFile;
            }

            string addParam;
            bool needdcri = true;
            bool isNvidia = false;
            var dcri = "-dcri 3";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.CurrentExtraLaunchParameters.Contains("-dcri"))
                {
                    needdcri = false;
                }

                if (pair.Algorithm is DualAlgorithm algo && algo.TuningEnabled && pair.CurrentExtraLaunchParameters.Contains("-dcri"))
                {
                    if (btcAdress == Globals.DemoUser)
                    {
                        algo.TuningEnabled = true;
                        Helpers.ConsolePrint("Tuning ENABLE ", "");
                    }
                    else
                    {
                        algo.TuningEnabled = false;
                        Helpers.ConsolePrint("Tuning DISABLE ", "");
                    }
                }

                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    isNvidia = true;
                } else
                {
                    isNvidia = false;
                }
            }

            if (SecondaryAlgorithmType == AlgorithmType.Blake2s & needdcri & !istuned)
            {
                if (isNvidia)
                {
                    dcri = "-dcri 10";
                }
                else
                {
                    dcri = "-dcri 3";
                }
                addParam = " "
                    + GetDevicesCommandString()
                    + String.Format("  -epool {0} -ewal {1} -mport 127.0.0.1:-{2} -esm 3 -epsw x -allpools 1 -ftime 10 -retrydelay 5 " + dcri + " ", url, username, ApiPort)
                    + dualModeParams;
            }
            else if (SecondaryAlgorithmType == AlgorithmType.Keccak & needdcri & !istuned)
            {
                if (isNvidia)
                {
                    dcri = "-dcri 10";
                } else
                {
                    dcri = "-dcri 3";
                }

                addParam = " "
                                    + GetDevicesCommandString()
                                    + String.Format("  -epool {0} -ewal {1} -mport 127.0.0.1:-{2} -esm 3 -epsw x -allpools 1 -ftime 10 -retrydelay 5 " +dcri+ " ", url, username, ApiPort)
                                    + dualModeParams;
            }
            else
            {

            addParam = " "
                                    + GetDevicesCommandString()
                                    + String.Format("  -epool {0} -ewal {1} -mport 127.0.0.1:-{2} -esm 3 -epsw x -allpools 1 -ftime 10 -retrydelay 5", url, username, ApiPort)
                                    + dualModeParams;
            }
            return addParam + " -epoolsfile "+epoolsFile;
        }

        private string GetStartBenchmarkCommand(string url, string btcAdress, string worker)
        {
            var username = GetUsername(btcAdress, worker);
            // AlgorithmType alg = AlgorithmType.Lbry;
            var dual = AlgorithmType.NONE;
            string poolport = "3354";
            var dualModeParams = "";
            string epoolsFile = "";
            string dpoolsFile = "";
            var urlSecond = "";
            var esm = "";

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

            if (File.Exists("bin_3rdparty\\claymore_dual\\epools.txt"))
                File.Delete("bin_3rdparty\\claymore_dual\\epools.txt");
            if (File.Exists("bin_3rdparty\\claymore_dual\\dpools.txt"))
                File.Delete("bin_3rdparty\\claymore_dual\\dpools.txt");

            if (File.Exists("bin_3rdparty\\claymore_dual\\" + epoolsFile))
                File.Delete("bin_3rdparty\\claymore_dual\\" + epoolsFile);
            if (File.Exists("bin_3rdparty\\claymore_dual\\" + dpoolsFile))
                File.Delete("bin_3rdparty\\claymore_dual\\" + dpoolsFile);

            Thread.Sleep(200);

            String epools = String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[0, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[1, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[2, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[3, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[4, 0], username) + "\n"
               + String.Format("POOL: daggerhashimoto.{0}.nicehash.com:3353, WALLET: {1}, PSW: x, ESM: 3, ALLPOOLS: 1", myServers[5, 0], username) + "\n";
            try
            {
                FileStream fs = new FileStream("bin_3rdparty\\claymore_dual\\" + epoolsFile, FileMode.Create, FileAccess.Write);
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

            Thread.Sleep(200);
            if (!IsDual())
            {
                // leave convenience param for non-dual entry
                foreach (var pair in MiningSetup.MiningPairs)
                {
                    if (!pair.CurrentExtraLaunchParameters.Contains("-dual=")) continue;
                    dual = AlgorithmType.NONE;
                    var coinP = "";
                    /*
                    if (dual != AlgorithmType.NONE)
                    {
                        urlSecond = Globals.GetLocationUrl(dual,
                            Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation],
                            ConectionType);
                        dualModeParams = $" {coinP} -dpool {urlSecond} -dwal {username}";
                        break;
                    }
                    */
                }
            }
            else //dual
            {
                
                 urlSecond = Globals.GetLocationUrl(SecondaryAlgorithmType,
                    Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], ConectionType);
                username = Globals.GetBitcoinUser();
                var dpsw = "";
                if (SecondaryAlgorithmType == AlgorithmType.Decred)
                {
                    urlSecond = "stratum+tcp://decred.eu.mine.zpool.ca:5744";
                    username = "1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2";
                    dpsw = "c=BTC";
                    poolport = "3354";
                }
                if (SecondaryAlgorithmType == AlgorithmType.Lbry)
                {
                    urlSecond = "stratum+tcp://lbry.eu.mine.zpool.ca:3334";
                    username = "1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2";
                    dpsw = "c=BTC";
                    poolport = "3356";
                }

                if (SecondaryAlgorithmType == AlgorithmType.Blake2s)
                {
                    urlSecond = "stratum+tcp://blake2s.eu.mine.zpool.ca:5766";
                    username = "1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2";
                    dpsw = "c=BTC";
                    poolport = "3361";
                }
                if (SecondaryAlgorithmType == AlgorithmType.Keccak)
                {
                    poolport = "3338";
                    urlSecond = "stratum+tcp://keccak." + myServers[0, 0] + ".nicehash.com:" + poolport;
                    username = Globals.GetBitcoinUser();
                    dpsw = "x";
                }
                
                if (SecondaryAlgorithmType == AlgorithmType.Pascal)
                {
                    poolport = "3358";
                    urlSecond = "stratum+tcp://pascal." + myServers[0, 0] + ".nicehash.com:" + poolport;
                    username = Globals.GetBitcoinUser();
                    dpsw = "x";
                    //esm = "-esm 3";
                }
                if (SecondaryAlgorithmType == AlgorithmType.Sia)
                {
                    poolport = "3360";
                    urlSecond = "stratum+tcp://sia." + myServers[0, 0] + ".nicehash.com:" + poolport;
                    username = Globals.GetBitcoinUser();
                    dpsw = "x";
                    //esm = "-esm 3";
                }
                String dpools = "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[0, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x, ESM: 3, ALLPOOLS: 1", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[1, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x, ESM: 3, ALLPOOLS: 1", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[2, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x, ESM: 3, ALLPOOLS: 1", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[3, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x, ESM: 3, ALLPOOLS: 1", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[4, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x, ESM: 3, ALLPOOLS: 1", username) + "\n"
                 + "POOL: stratum+tcp://" + SecondaryAlgorithmType.ToString().ToLower() + "." + myServers[5, 0] + ".nicehash.com:" + poolport + String.Format(", WALLET: {0}, PSW: x, ESM: 3, ALLPOOLS: 1", username) + "\n";
                try
                {
                    FileStream fs1 = new FileStream("bin_3rdparty\\claymore_dual\\" + dpoolsFile, FileMode.Create, FileAccess.Write);
                    StreamWriter w1 = new StreamWriter(fs1);
                    w1.WriteAsync(dpools);
                    w1.Flush();
                    w1.Close();
                    Thread.Sleep(200);
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("GetStartCommand", e.ToString());
                }

                // urlSecond = Globals.GetLocationUrl(SecondaryAlgorithmType,
                //    Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], ConectionType);
                dualModeParams = $" -dcoin {SecondaryShortName()} -dpool {urlSecond} -dwal {username} -dpsw {dpsw} {esm} -dpoolsfile " + dpoolsFile;
                //dualModeParams = $" -dcoin {SecondaryShortName()} -dpoolsfile " + dpoolsFile;
            }

            string addParam;
            bool needdcri = true;
            bool isNvidia = false;
            var dcri = "-dcri 3";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.CurrentExtraLaunchParameters.Contains("-dcri"))
                {
                    needdcri = false;
                }

                if (pair.Algorithm is DualAlgorithm algo && algo.TuningEnabled && pair.CurrentExtraLaunchParameters.Contains("-dcri"))
                {
                    if (btcAdress == Globals.DemoUser)
                    {
                        algo.TuningEnabled = true;
                        Helpers.ConsolePrint("Tuning ENABLE ", "");
                    }
                    else
                    {
                        algo.TuningEnabled = false;
                        Helpers.ConsolePrint("Tuning DISABLE ", "");
                    }
                }

                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    isNvidia = true;
                }
                else
                {
                    isNvidia = false;
                }
            }

            if (SecondaryAlgorithmType == AlgorithmType.Blake2s & needdcri & !istuned)
            {
                if (isNvidia)
                {
                    dcri = "-dcri 10";
                }
                else
                {
                    dcri = "-dcri 3";
                }
                addParam = " "
                    + GetDevicesCommandString()
                    + String.Format("  -epool stratum+tcp://eu1.ethermine.org:4444 -ewal 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.Claymore -mport 127.0.0.1:-{0} -epsw x -allpools 1 -ftime 10 -retrydelay 5 " + dcri + " ", ApiPort)
                    + dualModeParams;
            }
            else if (SecondaryAlgorithmType == AlgorithmType.Keccak & needdcri & !istuned)
            {
                if (isNvidia)
                {
                    dcri = "-dcri 10";
                }
                else
                {
                    dcri = "-dcri 3";
                }

                addParam = " "
                                    + GetDevicesCommandString()
                                    + String.Format("  -epool stratum+tcp://eu1.ethermine.org:4444 -ewal 9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.Claymore -mport 127.0.0.1:-{0} -epsw x -allpools 1 -ftime 10 -retrydelay 5 " + dcri + " ", ApiPort)
                                    + dualModeParams;
            }
            else if (SecondaryAlgorithmType == AlgorithmType.Pascal || SecondaryAlgorithmType == AlgorithmType.Sia)
            {

                addParam = " "
                                        + GetDevicesCommandString()
                                        + String.Format("  -epool daggerhashimoto.usa" + ".nicehash.com:3353 -ewal {0}/{1} -mport 127.0.0.1:-{2} -epsw x -esm 3 -allpools 1 -ftime 10 -retrydelay 5", username, worker, ApiPort)
                                        + dualModeParams;
            }
            else
            {

                addParam = " "
                                        + GetDevicesCommandString()
                                        + String.Format("  -epool stratum+tcp://eu1.ethermine.org:4444 -ewal 9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.Claymore -mport 127.0.0.1:-{0} -epsw x -allpools 1 -ftime 10 -retrydelay 5", ApiPort)
                                        + dualModeParams;
            }
            return addParam + " -epoolsfile " + epoolsFile;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            var strdual = "";
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
