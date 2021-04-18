using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Devices;
using System.Net;

namespace NiceHashMiner.Miners
{
    public class trex : Miner
    {
        private int _benchmarkTimeWait = 180;
        private const int TotalDelim = 2;
        public trex() : base("trex")
        {
        }
        private bool _benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.trex;
        public override void Start(string url, string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            var username = GetUsername(btcAdress, worker);

           // IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.trex;

            var algo = "";
            var apiBind = "";
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            algo = "-a " + MiningSetup.MinerName.ToLower();
            apiBind = " --api-bind-http 127.0.0.1:" + ApiPort;
            IsApiReadException = false;

            //  url = url.Replace(".nicehash.", "-new.nicehash.");
            algo = algo.Replace("daggerhashimoto", "ethash");
            url = url.Replace("stratum+tcp", "stratum2+tcp");
            LastCommandLine = algo +
     " -o " + url + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + Form_Main.myServers[1, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + Form_Main.myServers[2, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + Form_Main.myServers[3, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + port + " -u " + username + " -p x " +
     apiBind +
     " -d " + GetDevicesCommandString() + " --no-watchdog " +
     ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
            {
                LastCommandLine = LastCommandLine.Replace("stratum2", "stratum");
            }
            ProcessHandle = _Start();
        }

        // benchmark stuff
        protected void KillMinerBase(string exeName)
        {
            foreach (var process in Process.GetProcessesByName(exeName))
            {
                try { process.Kill(); }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
        }

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            string configfilename = GetLogFileName();
            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            var username = GetUsername(Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim());
            var commandLine = "";
            url = url.Replace("stratum+tcp", "stratum2+tcp");

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                commandLine = "--algo x16rv2" +
                 " -o stratum+tcp://x16rv2.na.mine.zpool.ca:3637" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                 " -o " + url + " -u " + username + " -p x " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) +
                                  " --api-bind-http 127.0.0.1:" + ApiPort +
                              " -d ";
                commandLine += GetDevicesCommandString();
                //_benchmarkTimeWait = 180;
                _benchmarkTimeWait = time;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                commandLine = "--algo kawpow" +
                 " -o stratum+tcp://rvn.2miners.com:6060" + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.trex" + " -p x " +
                 " -o " + url + " -u " + username + " -p x " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                              " -d ";
                commandLine += GetDevicesCommandString();
                _benchmarkTimeWait = time;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                commandLine = "--algo ethash" +
                 " -o stratum+tcp://eu1.ethermine.org:4444" + " -u 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.trex" + " -p x " +
                 " -o " + url + " -u " + username + " -p x " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                              " -d ";
                commandLine += GetDevicesCommandString();
                _benchmarkTimeWait = time;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
            {
                commandLine = "--algo octopus" +
                 " -o stratum+tcp://cfx.woolypooly.com:3094" + " -u 0x13097ee19fd453AfD6F2ecf155927f2b7380307F.trex" + " -p x " +
                 " -o " + url + " -u " + username + " -p x " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog --api-bind-http 127.0.0.1:" + ApiPort +
                              " -d ";
                commandLine += GetDevicesCommandString();
                _benchmarkTimeWait = time;
            }
            return commandLine;
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
                        delay_before_calc_hashrate = 5;
                        MinerStartDelay = 30;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 20;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus)) // not tested
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
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
                            /*
                            var imageName = MinerExeName.Replace(".exe", "");
                            // maybe will have to KILL process
                            KillMinerBase(imageName);
                            int k = ProcessTag().IndexOf("pid(");
                            int i = ProcessTag().IndexOf(")|bin");
                            var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();
                            int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                            KillProcessAndChildren(pid);
                            */
                            break;
                        }

                    }
                }
                BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
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


        // stub benchmarks read from file
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }

        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            string resp = null;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            if (Form_Main.NVIDIA_orderBug)
            {
                sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
            }
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/summary");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                resp = await Reader.ReadToEndAsync();

                Reader.Close();
                Response.Close();
                WR.Abort();
                SS.Close();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API", ex.Message);
                return null;
            }

            if (resp != null)
            {
                //Helpers.ConsolePrint(MinerTag(), "API: " + resp);
                try
                {
                    dynamic respJson = JsonConvert.DeserializeObject(resp);
                    int devs = 0;
                    foreach (var dev in respJson.gpus)
                    {
                        //Helpers.ConsolePrint(MinerTag(), "API device_id: " + dev.device_id + " gpu_id: " + dev.gpu_id + " gpu_user_id: " + " hashrate: " + dev.hashrate);
                        sortedMinerPairs[devs].Device.MiningHashrate = dev.hashrate;
                        devs++;
                    }
                    //Helpers.ConsolePrint(MinerTag(), "API total: " + respJson.hashrate);
                    ad.Speed = respJson.hashrate;
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("API eror", ex.Message);
                    return null;
                }

                if (ad.Speed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }
                else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                }

                if (ad.Speed < 0)
                {
                    Helpers.ConsolePrint(MinerTag(), "Reporting negative speeds will restart...");
                    Restart();
                }
            } else
            {
                Thread.Sleep(1);
            }

            return ad;
        }


        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            if (ProcessHandle != null)
            {
                if (!ConfigManager.GeneralConfig.NoForceTRexClose)
                {
                    Thread.Sleep(500);
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Try force killing miner!");
                    try { KillMinerBase("t-rex"); }
                    catch { }
                }
            }
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5; // 5 minute max, whole waiting time 75seconds
        }
    }
}
