using Newtonsoft.Json;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using NiceHashMinerLegacy.Common.Enums;
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

namespace NiceHashMiner.Miners
{
    public class Phoenix : Miner
    {
        public Phoenix()
            : base("Phoenix")
        {
        }

        private bool _benchmarkException => MiningSetup.MinerPath == MinerPaths.Data.Phoenix;
        private int TotalCount = 6;
        private const int TotalDelim = 2;
        private string platform = "";
        double dSpeed = 0;
        double speed = 0;
        string cSpeed = "";
        int count = 0;
        string ResponseFromPhoenix;
        private int _benchmarkTimeWait = 120;
        private double _power = 0.0d;
        double _powerUsage = 0;

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 8;
        }
        private string GetServer(string algo, string username, string port)
        {
            string ret = "";
            string ssl = "";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                ssl = "ssl://";
            }
            else
            {
                port = "1" + port;
                ssl = "";
            }

            int n = 0;
            foreach (string serverUrl in Globals.MiningLocation)
            {
                n++;
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + "-pool" + n.ToString() + " " + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + "-pool" + n.ToString() + " " + ssl + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " ";
                }
                if (n >= 2) break;
            }
            return ret.Replace("-pool1", "-pool");
        }
        private string GetStartCommand(string btcAdress, string worker)
        {
            var username = GetUsername(btcAdress, worker);

            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = " -nvidia ";
                }
                else
                {
                    platform = " -amd ";
                }
            }
            try
            {
                if (File.Exists("miners\\phoenix\\epools.txt"))
                    File.Delete("miners\\phoenix\\epools.txt");
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetStartCommand", ex.ToString());
            }

            Thread.Sleep(200);
            string ssl = "";
            string port = "3353";
            if (ConfigManager.GeneralConfig.ProxySSL)
            {
                ssl = "ssl://";
            } else
            {
                ssl = "";
            }
            string coin = "";
            string algo = "daggerhashimoto";
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                coin = "-coin etc ";
                algo = "etchash";
                port = "3393";
            }

            DeviceType devtype = DeviceType.NVIDIA;
            if (platform == " -amd ")
            {
                devtype = DeviceType.AMD;
            }
            return " -gpus " + GetDevicesCommandString() + platform + "-retrydelay 10 " + coin +
                GetServer(algo, username, port) + " -wal " + username + " -pass x" +
                   " -cdmport  127.0.0.1:" + ApiPort + " -proto 4 " +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, devtype);
        }

        private string GetStartBenchmarkCommand(string url, string btcAdress, string worker, string psw = "x", bool benchmark = false)
        {
            var platform = "";
            DeviceType devtype = DeviceType.NVIDIA;
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = " -nvidia ";
                }
                else
                {
                    platform = " -amd ";
                    devtype = DeviceType.AMD;
                }
            }
            try
            {
                if (File.Exists("miners\\phoenix\\epools.txt"))
                    File.Delete("miners\\phoenix\\epools.txt");
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetStartCommand", ex.ToString());
            }
            Thread.Sleep(200);
            string pool = $" -pool {Links.CheckDNS(url)} -wal {btcAdress} -cdmport 127.0.0.1:{ApiPort} -pass " + psw + " ";
            /*
            if (benchmark)
            {
                pool = $" -bench 100 -cdmport 127.0.0.1:{ApiPort} ";
            }
            */
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            return " -gpus " + GetDevicesCommandString() + platform + "-retrydelay 10"
                   + pool +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, devtype);

        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            var ids = MiningSetup.MiningPairs.Select(mPair => (mPair.Device.IDByBus + 1).ToString()).ToList();
            deviceStringCommand += string.Join(",", ids);
            return deviceStringCommand;
        }

        public override void Start(string btcAdress, string worker)
        {
            string url = "";
            LastCommandLine = GetStartCommand(btcAdress, worker);
            //IsApiReadException = false;
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            Thread.Sleep(200);
            if (ProcessHandle != null)
            {
                try
                {
                    ProcessHandle.Kill();
                }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
        }


        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            _benchmarkTimeWait = time;
            string ret = "";
            if (algorithm.NiceHashID == AlgorithmType.DaggerHashimoto)
            {
                ret = GetStartBenchmarkCommand(Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020"), "0x266b27bd794d1A65ab76842ED85B067B415CD505.Phoenix", "");
            }

            if (algorithm.NiceHashID == AlgorithmType.ETCHash)
            {
                ret = GetStartBenchmarkCommand(Links.CheckDNS("stratum+tcp://etc.2miners.com:1010"), "0x266b27bd794d1A65ab76842ED85B067B415CD505.Phoenix", "");
            }
            return ret;
        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
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

            int delay_before_calc_hashrate = 5;
            int MinerStartDelay = 25;

            if (commandLine.ToString().Contains("amd"))
            {
                delay_before_calc_hashrate = 30;
                _benchmarkTimeWait = _benchmarkTimeWait + 60;
            }
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
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 180)
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

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        _powerUsage += _power;
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 10);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() + " power: " + _power.ToString());
                            if (commandLine.ToString().Contains("amd"))
                            {
                                summspeed = Math.Max(summspeed, ad.Result.Speed);
                            }
                            else
                            {
                                summspeed += ad.Result.Speed;
                            }
                        }
                        else
                        {
                            Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                        }

                        if (repeats >= _benchmarkTimeWait - MinerStartDelay - 10)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Benchmark ended");
                            ad.Dispose();
                            benchmarkTimer.Stop();

                            int pid = BenchmarkHandle.Id;
                            try { ProcessHandle.SendCtrlC((uint)Process.GetCurrentProcess().Id); } catch { }
                            Thread.Sleep(1000);

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            EndBenchmarkProcces();

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
                if (commandLine.ToString().Contains("amd"))
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = summspeed;
                }
                else
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
                }
                BenchmarkAlgorithm.PowerUsageBenchmark = (_powerUsage / repeats);
                BenchmarkThreadRoutineFinish();
            }
        }
        #endregion // Decoupled benchmarking routines


        private class JsonApiResponse
        {
#pragma warning disable IDE1006 // Naming Styles
            public List<string> result { get; set; }
            public int id { get; set; }
            public object error { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }
        protected double ApiReadMult = 1;
        protected AlgorithmType SecondaryAlgorithmType = AlgorithmType.NONE;
        public bool IsDual()
        {
            return (SecondaryAlgorithmType != AlgorithmType.NONE);
        }

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            ApiReadMult = 1000;
            JsonApiResponse resp = null;
            try
            {
                var bytesToSend = Encoding.ASCII.GetBytes("{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\"}\n");
                var client = new TcpClient("127.0.0.1", ApiPort);
                var nwStream = client.GetStream();
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                var bytesToRead = new byte[client.ReceiveBufferSize];
                var bytesRead = await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                var respStr = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                resp = JsonConvert.DeserializeObject<JsonApiResponse>(respStr, Globals.JsonSettings);
                client.Close();
                //Helpers.ConsolePrint("API: ", respStr);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), "GetSummary exception: " + ex.Message);
            }

            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);

            if (resp != null && resp.error == null)
            {
                if (resp.result != null && resp.result.Count > 4)
                {
                    var speeds = resp.result[3].Split(';');
                    var secondarySpeeds = (IsDual()) ? resp.result[5].Split(';') : new string[0];
                    ad.Speed = 0;
                    ad.SecondarySpeed = 0;

                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderByDescending(pair => pair.Device.DeviceType)
                              .ThenBy(pair => pair.Device.IDByBus + 1).ToList();
                    if (Form_Main.NVIDIA_orderBug)
                    {
                        sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
                    }

                    int dev = 0;
                    foreach (var speed in speeds)
                    {
                        double tmpSpeed;
                        try
                        {
                            tmpSpeed = double.Parse(speed, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            tmpSpeed = 0;
                        }
                        _power = sortedMinerPairs[dev].Device.PowerUsage;
                        sortedMinerPairs[dev].Device.MiningHashrate = tmpSpeed * ApiReadMult;
                        dev++;
                        ad.Speed += tmpSpeed;
                    }

                    foreach (var speed in secondarySpeeds)
                    {
                        double tmpSpeed;
                        try
                        {
                            tmpSpeed = double.Parse(speed, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            tmpSpeed = 0;
                        }

                        ad.SecondarySpeed += tmpSpeed;
                    }

                    ad.Speed *= ApiReadMult;
                    ad.SecondarySpeed *= ApiReadMult;
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                }

                if (ad.Speed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }

                if (ad.Speed < 0)
                {
                    Helpers.ConsolePrint(MinerTag(), "Reporting negative speeds will restart...");
                    Restart();
                }
            }

            return ad;
        }

    }

}
