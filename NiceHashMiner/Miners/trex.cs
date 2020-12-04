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

namespace NiceHashMiner.Miners
{
    public class trex : Miner
    {
        private int _benchmarkTimeWait = 240;
        private string[,] myServers = Form_Main.myServers;
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
            apiBind = " -b 127.0.0.1:" + ApiPort;
            IsApiReadException = false;

            //  url = url.Replace(".nicehash.", "-new.nicehash.");
            algo = algo.Replace("daggerhashimoto", "ethash");
            url = url.Replace("stratum+tcp", "stratum2+tcp");
            LastCommandLine = algo +
     " -o " + url + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + myServers[1, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + myServers[2, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + myServers[3, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + myServers[4, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + myServers[5, 0] + ".nicehash.com:" + port + " " + " -u " + username + " -p x " +
     " -o stratum2+tcp://" + alg + "." + myServers[0, 0] + ".nicehash.com:" + port + " -u " + username + " -p x " +
     apiBind +
     " -d " + GetDevicesCommandString() + " --no-watchdog " +
     ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA) + " ";

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
            if (File.Exists("miners\\t-rex\\" + GetLogFileName()))
                File.Delete("miners\\t-rex\\" + GetLogFileName());


            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
            {
                commandLine = "--algo x16rv2" +
                 " -o stratum+tcp://x16rv2.na.mine.zpool.ca:3637" + " -u 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + " -p c=BTC " +
                 " -o " + url + " -u " + username + " -p x " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                  MiningSetup,
                                  DeviceType.NVIDIA) +
                                  " -l " + GetLogFileName() + " -b 127.0.0.1:" + ApiPort +
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
                                  DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog -l " + GetLogFileName() + " -b 127.0.0.1:" + ApiPort +
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
                                  DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog -l " + GetLogFileName() + " -b 127.0.0.1:" + ApiPort +
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
                                  DeviceType.NVIDIA) + " --gpu-report-interval 1 --no-watchdog -l " + GetLogFileName() + " -b 127.0.0.1:" + ApiPort +
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

            try
            {
                BenchmarkAlgorithm.BenchmarkSpeed = 0;
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                //Helpers.ConsolePrint(MinerTag(), "Benchmark should end in : " + _benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string) commandLine);
                BenchmarkHandle.WaitForExit(_benchmarkTimeWait + 2);
                var benchmarkTimer = new Stopwatch();
                benchmarkTimer.Reset();
                benchmarkTimer.Start();
                BenchmarkThreadRoutineStartSettup();
                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                var keepRunning = true;
                while (IsActiveProcess(BenchmarkHandle.Id))
                {
                    //string outdata = BenchmarkHandle.StandardOutput.ReadLine();
                    //BenchmarkOutputErrorDataReceivedImpl(outdata);
                    // terminate process situations
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 2)
                        || BenchmarkSignalQuit
                        || BenchmarkSignalFinnished
                        || BenchmarkSignalHanged
                        || BenchmarkSignalTimedout
                        || BenchmarkException != null)
                    {
                        var imageName = MinerExeName.Replace(".exe", "");
                        // maybe will have to KILL process
                        KillMinerBase(imageName);

                        int k = ProcessTag().IndexOf("pid(");
                        int i = ProcessTag().IndexOf(")|bin");
                        var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();
                        int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                        KillProcessAndChildren(pid);

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

                        keepRunning = false;
                        break;
                    }

                    // wait a second due api request
                    Thread.Sleep(1000);

                    int delay_before_calc_hashrate = 90;
                    int bench_time = 30;
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        delay_before_calc_hashrate = 10;
                        bench_time = 20;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                    {
                        delay_before_calc_hashrate = 10;
                        bench_time = 20;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        delay_before_calc_hashrate = 10;
                        bench_time = 20;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus)) // not tested
                    {
                        delay_before_calc_hashrate = 10;
                        bench_time = 20;
                    }

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {

                        repeats++;
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString());
                            summspeed += ad.Result.Speed;
                        }
                        else
                        {
                            Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                        }

                        if (repeats >= bench_time + delay_before_calc_hashrate)
                        {
                            //Helpers.ConsolePrint(MinerTag(), "summspeed: " + summspeed.ToString() + " bench_time:" + bench_time.ToString());
                            BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (bench_time), 2);
                            ad.Dispose();
                            benchmarkTimer.Stop();
                            BenchmarkHandle.Dispose();

                            var imageName = MinerExeName.Replace(".exe", "");
                            // maybe will have to KILL process
                            KillMinerBase(imageName);

                            int k = ProcessTag().IndexOf("pid(");
                            int i = ProcessTag().IndexOf(")|bin");
                            var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();
                            int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                            KillProcessAndChildren(pid);
                            break;
                        }

                    }
                }
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
            return false;
        }



        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            string resp = null;
            try
            {
                var bytesToSend = Encoding.ASCII.GetBytes("summary\r\n");
                var client = new TcpClient("127.0.0.1", ApiPort);
                var nwStream = client.GetStream();
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                var bytesToRead = new byte[client.ReceiveBufferSize];
                var bytesRead = await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                var respStr = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

                client.Close();
                resp = respStr;
                //Helpers.ConsolePrint(MinerTag(), "API: " + respStr);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), "GetSummary exception: " + ex.Message);
            }

            if (resp != null)
            {
                var st = resp.IndexOf(";KHS=");
                var e = resp.IndexOf(";SOLV=");
                var parse = resp.Substring(st + 5, e - st - 5).Trim();
                double tmp = Double.Parse(parse, CultureInfo.InvariantCulture);
                ad.Speed = tmp * 1000;

                if (ad.Speed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }
                else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                }

                // some clayomre miners have this issue reporting negative speeds in that case restart miner
                if (ad.Speed < 0)
                {
                    Helpers.ConsolePrint(MinerTag(), "Reporting negative speeds will restart...");
                    Restart();
                }
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
