/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Management;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System.Threading;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Miners
{
    class teamredminer : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        int count = 0;
        private int TotalCount = 0;
        private double speed = 0;
        private double tmp = 0;
        private string[,] myServers = Form_Main.myServers;

        public teamredminer()
            : base("teamredminer_AMD")
        {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
            IsKillAllUsedMinerProcs = true;
            IsNeverHideMiningWindow = true;

        }
       
        protected override int GetMaxCooldownTimeInMilliseconds() {
            return 60*1000;
        }
        
        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        //    Killteamredminer();
        }
        static int GetWinVer(Version ver)
        {
            if (ver.Major == 6 & ver.Minor == 1)
                return 7;
            else if (ver.Major == 6 & ver.Minor == 2)
                return 8;
            else
                return 10;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            string username = GetUsername(btcAdress, worker);
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.teamredminer;
            IsApiReadException = false;

            //add failover
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);

            var algo = "";
            var apiBind = " --api_listen=127.0.0.1:" + ApiPort;
            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightV8))
            {
                algo = " -a cnv8";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightR))
            {
                algo = " -a cnr";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2z))
            {
                algo = " -a lyra2z";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                algo = " -a lyra2rev3";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16R))
            {
                algo = " -a x16r";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                algo = " -a x16rv2";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckarood29))
            {
                algo = " -a cuckarood29_grin";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.MTP))
            {
                algo = " -a mtp --allow_all_devices";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                algo = " -a ethash";
            }
            var sc = "";
            if (GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sc = variables.TRMiner_add1;
            }

            LastCommandLine = sc + " --watchdog_script " + algo + " -o " + url +
                              " -u " + username + " -p x " +
                               " -o stratum+tcp://" + alg + "." + myServers[4, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                               " -o stratum+tcp://" + alg + "." + myServers[3, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                               " -o stratum+tcp://" + alg + "." + myServers[2, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                               " -o stratum+tcp://" + alg + "." + myServers[1, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                               " -o stratum+tcp://" + alg + "." + myServers[0, 0] + nhsuff + ".nicehash.com:" + port + " -u " + username + " -p x" +
                              apiBind +
                              " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                              " -d ";

            LastCommandLine += GetDevicesCommandString();
            ProcessHandle = _Start();
        }

        // new decoupled benchmarking routines
        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
            var CommandLine = "";
            var apiBind = " --api_listen=127.0.0.1:" + ApiPort;
            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            var sc = "";
            if (GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sc = variables.TRMiner_add1;
            }
            // demo for benchmark
            string username = Globals.GetBitcoinUser();
            string worker = "";
            if (ConfigManager.GeneralConfig.WorkerName.Length > 0)
            {
                username += "." + ConfigManager.GeneralConfig.WorkerName.Trim();
                worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            }
            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2z))
            {
                CommandLine = sc + " -a lyra2z" + apiBind +
                " --url stratum+tcp://lyra2z.eu.mine.zpool.ca:4553" + " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " --url stratum+tcp://lyra2z.eu" + nhsuff + ".nicehash.com:3365" + " --user " + username + " - p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16R))
            {
                CommandLine = sc + " -a x16r" + apiBind +
                " --url stratum+tcp://x16r.eu.mine.zpool.ca:3636" + " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " --url stratum+tcp://x16r.eu" + nhsuff + ".nicehash.com:3366" + " --user " + username + " - p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                CommandLine = sc + " -a x16rv2" + apiBind +
                " --url stratum+tcp://x16rv2.na.mine.zpool.ca:3637" + " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " --url stratum+tcp://x16rv2.eu" + nhsuff + ".nicehash.com:3379" + " --user " + username + " - p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckarood29))
            {
                CommandLine = sc + " -a cuckarood29_grin" + apiBind +
                " --url stratum+tcp://grin.sparkpool.com:6666" + " --user angelbbs@mail.ru/" + worker + " -p x " +
                " --url stratum+tcp://grincuckaroo29.eu" + nhsuff + ".nicehash.com:3371" + " --user " + username + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                CommandLine = sc + " -a lyra2rev3" + apiBind +
                " --url stratum+tcp://lyra2v3.eu.mine.zpool.ca:4550" + " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                 " --url stratum+tcp://lyra2rev3.eu" + nhsuff + ".nicehash.com:3373" + " --user " + username + " -p x -d ";
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightV8))
            {
                CommandLine = sc + " -a cnv8" + apiBind +
                " --url stratum+tcp://cryptonightv8.eu" + nhsuff + ".nicehash.com:3367" + " --user " + username + " -p x -d ";
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightR))
            {
                CommandLine = sc + " -a cnr" +
                " -o stratum+tcp://cryptonightr.eu" + nhsuff + ".nicehash.com:3375" + " -u " + username + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.MTP))
            {
                CommandLine = sc + " -a mtp --allow_all_devices" +
                 " -o stratum+tcp://xzc.2miners.com:8080" + " -u aMGfYX8ARy4wKE57fPxkEBcnNuHegDBweE" + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                CommandLine = sc + " -a ethash" +
                 " -o stratum+tcp://eu1.ethermine.org:4444" + " -u 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.TRM" + " -p x -d ";
            }
            //return $" -o stratum+tcp://xmr-eu.dwarfpool.com:8005 {variant} -u 42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.{worker} -p x {extras} --api-port {ApiPort} --donate-level=1"
            /*
            CommandLine = " -a lyra2z "+
                          " --url " + url +
                          " --user " + username +
                          " -p x " +
                          ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                          " -d ";
*/
            CommandLine += GetDevicesCommandString();
            TotalCount = (time / 30) * 2;
            return CommandLine;

        }

        protected override bool BenchmarkParseLine(string outdata) {
            string hashSpeed = "";
            int kspeed = 1;

            //Helpers.ConsolePrint("TEAMRED:", outdata);
            //[2019-02-02 23:44:25] GPU 0 [64C, fan 39%] lyra2rev3: 22.58Mh/s, avg 22.93Mh/s,
            //[2019-03-09 11:21:02] GPU 1 [ 0C, fan  0%]       cnr: 3.072kh/s, avg 1.531kh/s, pool 1.579kh/s a:3 r:0 hw:0
            if (outdata.Contains("lyra2rev3: "))
            {
                int i = outdata.IndexOf("lyra2rev3: ");
                int k = outdata.IndexOf("h/s, avg");
                hashSpeed = outdata.Substring(i + 11, k - i - 12).Trim();
                Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("H/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0, 70).ToUpper().Contains("KH/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 70).ToUpper().Contains("MH/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    if (count - 3 > 0)
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = speed / (count - 3); ////PVS-Studio - stupid program! Выше условие на больше нуля
                    }
                    else BenchmarkAlgorithm.BenchmarkSpeed = 0d;
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }
            if (outdata.Contains("cnv8: "))
            {
                int i = outdata.IndexOf("cnv8: ");
                int k = outdata.IndexOf("h/s, avg");
                hashSpeed = outdata.Substring(i + 5, k - i - 6).Trim();
                Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("H/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0,65).ToUpper().Contains("KH/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("MH/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    if (count - 3 > 0)
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = speed / (count - 3);
                    }
                    else BenchmarkAlgorithm.BenchmarkSpeed = 0d;
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }
            if (outdata.Contains("cnr: "))
            {
                int i = outdata.IndexOf("cnr: ");
                int k = outdata.IndexOf("h/s, avg");
                hashSpeed = outdata.Substring(i + 5, k - i - 6).Trim();
                //Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("H/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("KH/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("MH/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    if (count - 3 > 0)
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = speed / (count - 3);
                    }
                    else BenchmarkAlgorithm.BenchmarkSpeed = 0d;
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }

            if (outdata.Contains("mtp: "))
            {
                int i = outdata.IndexOf("mtp: ");
                int k = outdata.IndexOf("h/s, avg");
                hashSpeed = outdata.Substring(i + 5, k - i - 6).Trim();
                //Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("H/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("KH/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("MH/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    BenchmarkAlgorithm.BenchmarkSpeed = speed / (count - 3);
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }
            //Total                      ethash: 28.63Mh/s, avg 25.96Mh/s, pool 49.99Mh/s 
            if (outdata.Contains("ethash: "))
            {
                int i = outdata.IndexOf("ethash: ");
                int k = outdata.IndexOf("h/s, avg");
                hashSpeed = outdata.Substring(i + 8, k - i - 9).Trim();
                Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("H/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0, 70).ToUpper().Contains("KH/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 70).ToUpper().Contains("MH/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    BenchmarkAlgorithm.BenchmarkSpeed = speed / (count - 3);
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }
            //GPU 0 [56C, fan 35%] lyra2z: 1.410Mh/s, avg 1.437Mh/s, pool   0.0 h/s a:0 r:0 hw:0
            if (outdata.Contains("lyra2z: ") )
            {
                int i = outdata.IndexOf("lyra2z: ");
                int k = outdata.IndexOf("h/s, avg");
                hashSpeed = outdata.Substring(i+8 , k - i-9).Trim();
                Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("H/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("KH/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("MH/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    BenchmarkAlgorithm.BenchmarkSpeed =  speed / (count - 3);
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }
            if (outdata.Contains("x16r: "))
            {
                int i = outdata.IndexOf("avg ");
                int k = outdata.IndexOf("h/s, pool");
                hashSpeed = outdata.Substring(i + 4, k - i - 5).Trim();
                Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("H/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("KH/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("MH/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    BenchmarkAlgorithm.BenchmarkSpeed = speed / (count - 3);
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }
            if (outdata.Contains("x16rv2: "))
            {
                int i = outdata.IndexOf("avg ");
                int k = outdata.IndexOf("h/s, pool");
                hashSpeed = outdata.Substring(i + 4, k - i - 5).Trim();
                Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("H/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("KH/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("MH/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    BenchmarkAlgorithm.BenchmarkSpeed = speed / (count - 3);
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }
            if (outdata.Contains("cuckarood29_grin: "))
            {
                int i = outdata.IndexOf("avg ");
                int k = outdata.IndexOf("h/s, avg");
                hashSpeed = outdata.Substring(i + 4, k - i - 5).Trim();
                Helpers.ConsolePrint(hashSpeed, "");
                if (outdata.ToUpper().Contains("G/S"))
                {
                    kspeed = 1;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("KG/S"))
                {
                    kspeed = 1000;
                }
                if (outdata.Substring(0, 65).ToUpper().Contains("MG/S"))
                {
                    kspeed = 1000000;
                }
                count++;
                if (count >= 4) //skip 2*30=1min
                {
                    try
                    {
                        tmp = Double.Parse(hashSpeed, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    if (tmp > 0)
                    {
                        speed = speed + tmp;
                    }
                    else
                    {
                        count--;
                    }
                    BenchmarkAlgorithm.BenchmarkSpeed = speed / (count - 3);
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
            }
            
            return false;

        }

      
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }


        #endregion // Decoupled benchmarking routines

        // TODO _currentMinerReadStatus
        public override async Task<ApiData> GetSummaryAsync()
        {
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);

            var resp = await GetApiDataAsync(ApiPort, "summary");
            
            //Helpers.ConsolePrint("trm-DEBUG_resp", resp.Trim());

            if (resp == null)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                return null;
            }

            try
            {
                // Checks if all the GPUs are Alive first
                var resp2 = await GetApiDataAsync(ApiPort, "devs");
                if (resp2 == null)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                    return null;
                }

                var checkGpuStatus = resp2.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                /*
                for (var i = 1; i < checkGpuStatus.Length - 1; i++)
                {
                    if (checkGpuStatus[i].Contains("Enabled=Y") && !checkGpuStatus[i].Contains("Status=Alive"))
                    {
                        Helpers.ConsolePrint(MinerTag(),
                            ProcessTag() + " GPU " + i + ": Sick/Dead/NoStart/Initialising/Disabled/Rejecting/Unknown");
                        CurrentMinerReadStatus = MinerApiReadStatus.WAIT;
                        return null;
                    }
                }
                */
                var resps = resp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (resps[1].Contains("SUMMARY"))
                {
                    var data = resps[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    // Get miner's current total speed
                    var speed = data[4].Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    // Get miner's current total MH
                    var totalMH = double.Parse(data[18].Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1],
                        new CultureInfo("en-US"));

                    ad.Speed = double.Parse(speed[1]) * 1000;

                    if (totalMH <= PreviousTotalMH)
                    {
                        Helpers.ConsolePrint(MinerTag(), ProcessTag() + " teamredminer might be stuck as no new hashes are being produced");
                        Helpers.ConsolePrint(MinerTag(),
                            ProcessTag() + " Prev Total MH: " + PreviousTotalMH + " .. Current Total MH: " + totalMH);
                        CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                        return null;
                    }

                    PreviousTotalMH = totalMH;
                }
                else
                {
                    ad.Speed = 0;
                }
            }
            catch
            {
                CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                return null;
            }

            CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            // check if speed zero
            if (ad.Speed == 0) CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;

            return ad;
        }
    }
}
