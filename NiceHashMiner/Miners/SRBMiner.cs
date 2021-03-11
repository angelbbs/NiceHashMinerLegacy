using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Parsing;
using NiceHashMiner.Devices;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Miners.Grouping;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;

namespace NiceHashMiner.Miners
{
    public class SRBMiner : Miner
    {
        private readonly int GPUPlatformNumber;
        private int _benchmarkTimeWait = 180;

       // private int TotalCount = 2;
        private const int TotalDelim = 2;
        int count = 0;
        private double speed = 0;
        private double tmp = 0;

        public SRBMiner() : base("SRBMiner") {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
        }

        public override void Start(string url, string btcAdress, string worker) {
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.SRBMiner;

            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
        }

        private string GetStartCommand(string url, string btcAddress, string worker) {
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            string username = GetUsername(btcAddress, worker);
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX))
            {
                var algo = "randomxmonero";
                var port = "3380";
                url = url.Replace("randomx", "randomxmonero");
                return $" --algorithm randomx --pool {url} --wallet {username} --nicehash true --api-enable --api-port {ApiPort} {extras} "
               + $" --pool stratum+tcp://{algo}.{Form_Main.myServers[1, 0]}.nicehash.com:{port} --wallet {username} "
               + $" --pool stratum+tcp://{algo}.{Form_Main.myServers[2, 0]}.nicehash.com:{port} --wallet {username} "
               + $" --pool stratum+tcp://{algo}.{Form_Main.myServers[3, 0]}.nicehash.com:{port} --wallet {username} "
               + $" --pool stratum+tcp://{algo}.{Form_Main.myServers[0, 0]}.nicehash.com:{port} --wallet {username} ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                var algo = "ethash";
                var port = "3353";
                return $" --main-pool-reconnect 2 --a0-is-zil --disable-cpu --algorithm ethash --pool {url} --wallet {username} --nicehash true --api-enable --api-port {ApiPort} "
               + $" --pool stratum+tcp://{algo}.{Form_Main.myServers[1, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://{algo}.{Form_Main.myServers[2, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://{algo}.{Form_Main.myServers[3, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://{algo}.{Form_Main.myServers[0, 0]}.nicehash.com:{port} --wallet {username} --nicehash true " +
               "--gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
            }
            return "unsupported algo";

        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";

            var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.IDByBus.ToString()).ToList();
            ids.Sort();
            deviceStringCommand += string.Join("!", ids);

            return deviceStringCommand;
        }
        private string GetStartBenchmarkCommand(string url, string btcAddress, string worker)
        {
            var LastCommandLine = GetStartCommand(url, btcAddress, worker);
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            string algo;
            string port;
            string username = GetUsername(btcAddress, worker);
            url = url.Replace("stratum+tcp://", "");

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX))
            {
                algo = "randomxmonero";
                port = "3380";
                ApiPort = 4040;

                return $" --algorithm randomx"
                + $" --pool stratum+tcp://pool.supportxmr.com:3333 --wallet 42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.benchmark" +
                $" --nicehash false --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName() } {extras}";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                algo = "ethash";
                port = "3353";

                return $" --disable-cpu --algorithm ethash" +
                    $" --pool stratum+tcp://eu1.ethermine.org:4444" +
                    $" --wallet 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
            }

            return "unknown";
        }

        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5;  // 5 min
        }
        public override async Task<ApiData> GetSummaryAsync()
        {

            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            string ResponseFromSRBMiner;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString());
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromSRBMiner = await Reader.ReadToEndAsync();
                //Helpers.ConsolePrint("API...........", ResponseFromSRBMiner);
                //if (ResponseFromSRBMiner.Length == 0 || (ResponseFromSRBMiner[0] != '{' && ResponseFromSRBMiner[0] != '['))
                //    throw new Exception("Not JSON!");
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

            dynamic resp = JsonConvert.DeserializeObject(ResponseFromSRBMiner);

            try
            {
                int totals = 0;
                if (resp != null)
                {
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        totals = resp.algorithms[0].hashrate.gpu.total;
                    } else
                    {
                        totals = resp.algorithms[0].hashrate.cpu.total;
                    }

                    //Helpers.ConsolePrint("API hashrate...........", totals.ToString());

                    ad.Speed = totals;
                    if (ad.Speed == 0)
                    {
                        CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                    }
                    else
                    {
                        CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                    }
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("API error", ex.Message);
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                ad.Speed = 0;
                return ad;
            }

            Thread.Sleep(1);
            return ad;


        }

        protected override bool IsApiEof(byte third, byte second, byte last) {
            return third == 0x7d && second == 0xa && last == 0x7d;
        }

        #region Benchmark

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
                Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation],
                ConectionType);
               _benchmarkTimeWait = time;
            return GetStartBenchmarkCommand(server, Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim());
        }

        protected override void BenchmarkThreadRoutine(object commandLine)
        {
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;
            double repeats = 0;
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
                // find latest log file
                string latestLogFile = "";
                var dirInfo = new DirectoryInfo(WorkingDirectory);
                foreach (var file in dirInfo.GetFiles(GetLogFileName()))
                {
                    latestLogFile = file.Name;
                    break;
                }
                try
                {
                    // read file log
                    if (File.Exists(WorkingDirectory + latestLogFile))
                    {
                        var lines = File.ReadAllLines(WorkingDirectory + latestLogFile);
                        foreach (var line in lines)
                        {
                            if (line != null)
                            {
                                CheckOutdata(line);
                            }
                        }
                        File.Delete(WorkingDirectory + latestLogFile);
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint(MinerTag(), ex.ToString());
                }
            }
        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }
        #endregion
    }

}
