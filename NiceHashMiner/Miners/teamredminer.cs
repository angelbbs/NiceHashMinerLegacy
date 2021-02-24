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
using System.Linq;

namespace NiceHashMiner.Miners
{
    class teamredminer : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        private int TotalCount = 0;
        private string[,] myServers = Form_Main.myServers;
        private int _benchmarkTimeWait = 180;

        public teamredminer()
            : base("teamredminer")
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

           
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2z))
            {
                algo = " -a lyra2z";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                algo = " -a lyra2rev3";
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
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                algo = " -a kawpow";
            }
            var sc = "";
            if (GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sc = variables.TRMiner_add1;
            }

            LastCommandLine = sc + " --watchdog_script " + algo + " -o " + url +
                              " -u " + username + " -p x " +
                               " -o stratum+tcp://" + alg + "." + myServers[3, 0] + ".nicehash.com:" + port + " -u " + username + " -p x" +
                               " -o stratum+tcp://" + alg + "." + myServers[2, 0] + ".nicehash.com:" + port + " -u " + username + " -p x" +
                               " -o stratum+tcp://" + alg + "." + myServers[1, 0] + ".nicehash.com:" + port + " -u " + username + " -p x" +
                               " -o stratum+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " -u " + username + " -p x" +
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

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
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
                " --url stratum+tcp://x16rv2.na.mine.zpool.ca:3637" + " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                " -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckarood29))
            {
                CommandLine = sc + " -a cuckarood29_grin" + 
                " --url stratum+tcp://mwc.2miners.com:1111" + " --user 2aHR0cHM6Ly9td2MuaG90Yml0LmlvLzcyOTkyMw.teamred" + " -p x " +
                " -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Lyra2REv3))
            {
                CommandLine = sc + " -a lyra2rev3" + 
                " --url stratum+tcp://lyra2v3.eu.mine.zpool.ca:4550" + " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                 " -d ";
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                CommandLine = sc + " -a ethash --eth_no_ramp_up" +
                 " -o stratum+tcp://eu1.ethermine.org:4444" + " -u 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.teamred" + " -p x -d ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                CommandLine = sc + " -a kawpow" +
                 " -o stratum+tcp://rvn.2miners.com:6060" + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.teamred" + " -p x -d ";
            }


            CommandLine += GetDevicesCommandString() +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD)+
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
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
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

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckarood29))
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
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        ComputeDevice.BenchmarkProgress = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString());
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
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);

            var resp = await GetApiDataAsync(ApiPort, "summary");

            if (resp == null)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                return null;
            }
            //Helpers.ConsolePrint("trm-DEBUG_resp", resp.Trim());
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

                var resps = resp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (resps[1].Contains("SUMMARY"))
                {
                    var data = resps[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    // Get miner's current 30s speed
                    var speed = data[3].Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    // Get miner's current total MH
                    var totalMH = double.Parse(data[18].Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1],
                        new CultureInfo("en-US"));

                    ad.Speed = double.Parse(speed[1]) * 1000000;
                    /*
                    if (totalMH <= PreviousTotalMH)
                    {
                        //Helpers.ConsolePrint(MinerTag(), ProcessTag() + " teamredminer might be stuck as no new hashes are being produced");
                        //Helpers.ConsolePrint(MinerTag(),
                         //   ProcessTag() + " Prev Total MH: " + PreviousTotalMH + " .. Current Total MH: " + totalMH);
                        CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                        return null;
                    }
                    */
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
