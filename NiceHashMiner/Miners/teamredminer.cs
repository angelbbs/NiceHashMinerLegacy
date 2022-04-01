using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Miners
{
    class teamredminer : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        private int TotalCount = 0;
        private int _benchmarkTimeWait = 180;
        private double _power = 0.0d;
        double _powerUsage = 0;

        public teamredminer()
            : base("teamredminer")
        {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
            IsKillAllUsedMinerProcs = true;
            IsNeverHideMiningWindow = true;

        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000;
        }

        protected override void _Stop(MinerStopType willswitch)
        {
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
            List<string> ResolvedServers = MiningSession.GetResolvedServers(MiningSetup.MinerName.ToLower());

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2z))
            {
                algo = " -a lyra2z";
                port = "3365";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                algo = " -a x16rv2";
                port = "3379";
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                algo = " -a ethash";
                port = "3353";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                algo = " -a kawpow";
                port = "3385";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                algo = " -a autolykos2";
                port = "3390";
            }
            var sc = "";
            if (GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sc = variables.TRMiner_add1;
            }

            LastCommandLine = sc + " --watchdog_script " + algo + 
                              " -o " + ResolvedServers[0] + ":" + (ResolvedServers[0].Contains("auto.") ? "9200" : port) + " -u " + username + " -p x" +
                              " -o " + ResolvedServers[1] + ":" + (ResolvedServers[1].Contains("auto.") ? "9200" : port) + " -u " + username + " -p x" +
                              " -o " + ResolvedServers[2] + ":" + (ResolvedServers[2].Contains("auto.") ? "9200" : port) + " -u " + username + " -p x" +
                              " -o " + ResolvedServers[3] + ":" + (ResolvedServers[3].Contains("auto.") ? "9200" : port) + " -u " + username + " -p x" +
                              apiBind +
                              " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                              " -d ";

            LastCommandLine += GetDevicesCommandString();
            ProcessHandle = _Start();
        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = "";
            var ids = new List<string>();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
            int id;
            foreach (var mPair in sortedMinerPairs)
            {
                id = mPair.Device.IDByBus;
                ids.Add(id.ToString());
            }

            deviceStringCommand += string.Join(",", ids);
            return deviceStringCommand;
        }
        // new decoupled benchmarking routines
        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            var CommandLine = "";
            var apiBind = " --api_listen=127.0.0.1:" + ApiPort;
            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            var sc = "";
            _benchmarkTimeWait = time;
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

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                CommandLine = sc + " -a x16rv2" +
                " --url " + Links.CheckDNS("stratum+tcp://x16rv2.na.mine.zpool.ca:3637") + " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " -d ";
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                CommandLine = sc + " -a ethash --eth_no_ramp_up" +
                 " -o " + Links.CheckDNS("stratum+tcp://eth.2miners.com:2020") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505.teamred" + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                CommandLine = sc + " -a kawpow" +
                 " -o stratum+tcp://rvn.2miners.com:6060" + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.teamred" + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                CommandLine = sc + " -a autolykos2" +
                 " -o " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3100") + " -u 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.teamred" + " -p x -d ";
            }

            CommandLine += GetDevicesCommandString() +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD) +
                apiBind;
            TotalCount = (time / 30) * 2;
            return CommandLine;

        }
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }

        protected override void BenchmarkThreadRoutine(object commandLine)
        {
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;
            double repeats = 0.0d;
            double summspeed = 0.0d;

            int delay_before_calc_hashrate = 10;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                //if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 30;
                }
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in: " + _benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string)commandLine);
                //BenchmarkHandle.WaitForExit(_benchmarkTimeWait + 2);
                var benchmarkTimer = new Stopwatch();
                benchmarkTimer.Reset();
                benchmarkTimer.Start();

                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                BenchmarkThreadRoutineStartSettup(); //need for benchmark log
                while (IsActiveProcess(BenchmarkHandle.Id))
                {
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 60)
                        || BenchmarkSignalQuit
                        || BenchmarkSignalFinnished
                        || BenchmarkSignalHanged
                        || BenchmarkSignalTimedout
                        || BenchmarkException != null)
                    {
                        var imageName = MinerExeName.Replace(".exe", "");
                        // maybe will have to KILL process
                        EndBenchmarkProcces();
                        //  KillMinerBase(imageName);
                        if (BenchmarkSignalTimedout)
                        {
                            throw new Exception("Benchmark timedout");
                        }

                        if (BenchmarkException != null)
                        {
                            throw BenchmarkException;
                        }

                        if (BenchmarkSignalQuit)
                        {
                            throw new Exception("Termined by user request");
                        }

                        if (BenchmarkSignalFinnished)
                        {
                            break;
                        }

                        //keepRunning = false;
                        break;
                    }
                    // wait a second due api request
                    Thread.Sleep(1000);

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        delay_before_calc_hashrate = 30;
                        MinerStartDelay = 0;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                    {
                        delay_before_calc_hashrate = 30;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                    {
                        delay_before_calc_hashrate = 60;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        delay_before_calc_hashrate = 30;
                        MinerStartDelay = 0;
                    }

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        _powerUsage += _power;
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() + " power: " + _power.ToString());
                            summspeed = Math.Max(ad.Result.Speed, summspeed);
                        }
                        else
                        {
                            Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                        }

                        if (repeats >= _benchmarkTimeWait - MinerStartDelay - 15)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Benchmark ended");
                            ad.Dispose();
                            benchmarkTimer.Stop();

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            EndBenchmarkProcces();

                            break;
                        }

                    }
                }
                BenchmarkAlgorithm.BenchmarkSpeed = summspeed;
                BenchmarkAlgorithm.PowerUsageBenchmark = (_powerUsage / repeats);
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {

                BenchmarkThreadRoutineFinish();
            }
        }
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }


        #endregion // Decoupled benchmarking routines

        public override async Task<ApiData> GetSummaryAsync()
        {
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            var resp2 = await GetApiDataAsync(ApiPort, "devs");
            if (resp2 == null)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                ad.Speed = 0.0d;
                return null;
            }
            //Helpers.ConsolePrint("API: ", resp2.Trim());
            try
            {
                var devStatus = resp2.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                int dev = 0;
                double totalSpeed = 0.0d;
                foreach (var s in devStatus)
                {
                    if (s.Contains("GPU="))
                    {
                        var st = s.LastIndexOf("MHS 30s=");
                        var e = s.LastIndexOf(",KHS av=");
                        string cSpeed = s.Substring(st + 8, e - st - 8);
                        //Helpers.ConsolePrint("API: ", cSpeed);
                        double.TryParse(cSpeed, out double devSpeed);
                        sortedMinerPairs[dev].Device.MiningHashrate = devSpeed * 1000000;
                        _power = sortedMinerPairs[dev].Device.PowerUsage;
                        totalSpeed = totalSpeed + devSpeed * 1000000;
                        ad.Speed = totalSpeed;
                        dev++;
                    }

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
