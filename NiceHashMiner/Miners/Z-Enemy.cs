﻿using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Configs;
using System.Threading;
using System.Windows.Forms;
using NiceHashMiner.Devices;
using Newtonsoft.Json;

namespace NiceHashMiner.Miners
{
    public class ZEnemy : Miner
    {
        public ZEnemy() : base("ZEnemy")
        { }

        private int TotalCount = 2;
        private double speed = 0;
        private int _benchmarkTimeWait = 180;
        private double Total = 0;
        private const int TotalDelim = 2;
        private bool _benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.ZEnemy;
        public static string apiRequest = "summary";
        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            var username = GetUsername(btcAdress, worker);

            // IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.ZEnemy;

            var algo = "";
            var apiBind = "";
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            algo = "--algo=" + MiningSetup.MinerName;
            apiBind = " --api-bind-http=" + ApiPort;

            LastCommandLine = algo +
                " --url=" + url + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[1, 0] + ".nicehash.com:" + port + " " + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[2, 0] + ".nicehash.com:" + port + " " + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[3, 0] + ".nicehash.com:" + port + " " + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " --userpass=" + username + ":x" +
                " --url=" + url + " --userpass=" + username + ":x" +
                " --userpass=" + username + ":x" + apiBind +
                " --devices " + GetDevicesCommandString() + " " +
                ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        // new decoupled benchmarking routines

        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            var username = GetUsername(Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim());
            var commandLine = "";
            var timeLimit = (_benchmarkException) ? "" : " --time-limit 300";

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                _benchmarkTimeWait = time;
                commandLine = " --algo=" + algorithm.MinerName +
                " --url=" + url + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[1, 0] + ".nicehash.com:" + port + " " + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[2, 0] + ".nicehash.com:" + port + " " + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[3, 0] + ".nicehash.com:" + port + " " + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " --userpass=" + username + ":x" +
                " --url=" + url + " --userpass=" + username + ":x" +
                " --url=stratum+tcp://x16rv2.na.mine.zpool.ca:3637" + " --userpass=1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2:c=BTC " +
                              timeLimit + " --api-bind-http=" + ApiPort + " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) +
                              " --no-color --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                _benchmarkTimeWait = time;
                commandLine = " -a kawpow" +
                " -o stratum+tcp://rvn.2miners.com:6060" + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.Z-Enemy" +
                " -o " + url + " -u " + username + " -p x" +
                " -o stratum+tcp://" + alg + "." + Form_Main.myServers[1, 0] + ".nicehash.com:" + port + " -u " + username + " -p x" +
                " -o stratum+tcp://" + alg + "." + Form_Main.myServers[2, 0] + ".nicehash.com:" + port + " -u " + username + " -p x" +
                " -o stratum+tcp://" + alg + "." + Form_Main.myServers[3, 0] + ".nicehash.com:" + port + " -u " + username + " -p x" +
                " -o stratum+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " -u " + username + " -p x" +
                              timeLimit + " --api-bind-http=" + ApiPort + " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) +
                              " --no-color --devices ";
            }

            commandLine += GetDevicesCommandString();

            //TotalCount = 2;
            TotalCount = (time / 60);

            Total = 0.0d;

            return commandLine;
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            int count = 0;
            double tmp = 0;


            if (_benchmarkException)
            {
                if (outdata.Contains("GPU") && outdata.Contains("/s")) //GPU#4: ASUS GTX 1060 3GB, 10.56MH/s
                {

                    var st = outdata.IndexOf("- ");
                    var e = outdata.IndexOf("/s [");
                    try
                    {
                        var parse = outdata.Substring(st + 2, e - st - 4).Trim();
                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    // save speed

                    if (outdata.ToUpper().Contains("KH/S"))
                        tmp *= 1000;
                    else if (outdata.ToUpper().Contains("MH/S"))
                        tmp *= 1000000;
                    else if (outdata.ToUpper().Contains("GH/S"))
                        tmp *= 1000000000;

                    speed = tmp;
                    if (speed > 0)
                    {
                        //BenchmarkSignalFinnished = true;
//                        Helpers.ConsolePrint("BENCHMARK", "BenchmarkAlgorithm.BenchmarkSpeed:" + BenchmarkAlgorithm.BenchmarkSpeed.ToString());
                        return true;
                    }
                }
                /*
                if (TotalCount <= 0)
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = speed;
                    BenchmarkSignalFinnished = true;
                    return true;
                }
                */
            }
            return false;
        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            //Helpers.ConsolePrint("BENCHMARK", outdata);
            CheckOutdata(outdata);
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                BenchmarkParseLine(outdata);
            }
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
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 90)
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

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        delay_before_calc_hashrate = 5;
                        MinerStartDelay = 20;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                    {
                        delay_before_calc_hashrate = 2;
                        MinerStartDelay = 5;
                    }

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString());
                            summspeed += ad.Result.Speed;
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
                    if (speed != 0 && ad.Result == null && ad.Result.Speed == 0)
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
                if (BenchmarkAlgorithm.BenchmarkSpeed == 0)
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
                }
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {
                if (BenchmarkAlgorithm.BenchmarkSpeed == 0)
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = speed;
                }
                BenchmarkThreadRoutineFinish();
            }
        }


        #endregion // Decoupled benchmarking routines

        
        public override async Task<ApiData> GetSummaryAsync()
        {
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            string dataToSend;

            dataToSend = GetHttpRequestNhmAgentStrin("summary?gpuinfo=1");
            var resp = await GetApiDataAsync(ApiPort, dataToSend, true);
             
            if (resp == null || !resp.Contains("dev_id"))
            {
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " summary is null");
                ad.Speed = 0;
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                return null;
            }
            try
            {
                string jsonData = resp.Substring(resp.IndexOf('{'));
                //Helpers.ConsolePrint("API", jsonData);
                dynamic respJson = JsonConvert.DeserializeObject(jsonData);
                int devs = 0;
                double total = 0.0d;
                foreach (var dev in respJson.gpus)
                {
                    //Helpers.ConsolePrint("API:", dev.dev_id.ToString());
                    sortedMinerPairs[devs].Device.MiningHashrate = (double)dev.hashrate;
                    total = total + (double)dev.hashrate;
                    devs++;
                }
                ad.Speed = total;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ex.ToString());
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                return null;
            }
            
            CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            // check if speed zero
            if (ad.Speed == 0) CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;

            return ad;
        }
    }
}
