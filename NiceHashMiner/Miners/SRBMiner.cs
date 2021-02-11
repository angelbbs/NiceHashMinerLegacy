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
        private int _benchmarkTimeWait = 210;

       // private int TotalCount = 2;
        private const int TotalDelim = 2;
        int count = 0;
        private double speed = 0;
        private double tmp = 0;
        private string[,] myServers = Form_Main.myServers;

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
               + $" --pool stratum+tcp://{algo}.{myServers[1, 0]}.nicehash.com:{port} --wallet {username} "
               + $" --pool stratum+tcp://{algo}.{myServers[2, 0]}.nicehash.com:{port} --wallet {username} "
               + $" --pool stratum+tcp://{algo}.{myServers[3, 0]}.nicehash.com:{port} --wallet {username} "
               + $" --pool stratum+tcp://{algo}.{myServers[4, 0]}.nicehash.com:{port} --wallet {username} "
               + $" --pool stratum+tcp://{algo}.{myServers[0, 0]}.nicehash.com:{port} --wallet {username} ";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                var algo = "ethash";
                var port = "3353";
                return $" --main-pool-reconnect 2 --a0-is-zil --disable-cpu --algorithm ethash --pool {url} --wallet {username} --nicehash true --api-enable --api-port {ApiPort} "
               + $" --pool stratum+tcp://{algo}.{myServers[1, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://{algo}.{myServers[2, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://{algo}.{myServers[3, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://{algo}.{myServers[4, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://{algo}.{myServers[0, 0]}.nicehash.com:{port} --wallet {username} --nicehash true " +
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

            if (File.Exists(GetLogFileName()))
                File.Delete(GetLogFileName());
            Thread.Sleep(500);

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX))
            {
                algo = "randomxmonero";
                port = "3380";

                return $" --algorithm randomx --pool stratum+tcp://{algo}.{myServers[0, 0]}.nicehash.com:{port} --wallet {username}"
                + $" --pool stratum+tcp://pool.supportxmr.com:3333 --wallet 42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.benchmark --nicehash false --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()} {extras}";
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
            }
            catch (Exception ex)
            {
                //Helpers.ConsolePrint("API", ex.Message);
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

            Thread.Sleep(1000);
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
              // _benchmarkTimeWait = time;//SRBMiner report hashrate every 3 min
            return GetStartBenchmarkCommand(server, Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim());
        }

        protected override void BenchmarkThreadRoutine(object CommandLine)
        {
            BenchmarkThreadRoutineAlternate(CommandLine, _benchmarkTimeWait);
        }
        protected void BenchmarkThreadRoutineAlternate(object commandLine, int benchmarkTimeWait)
        {
            CleanOldLogs();

            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;
            int repeats = 0;
            double summspeed = 0.0d;
            BenchmarkAlgorithm.BenchmarkSpeed = 0;
            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                Helpers.ConsolePrint("BENCHMARK-routineAlt", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in : " + benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string)commandLine);
                BenchmarkHandle.WaitForExit(benchmarkTimeWait + 2);
                var benchmarkTimer = new Stopwatch();
                benchmarkTimer.Reset();
                benchmarkTimer.Start();
                //BenchmarkThreadRoutineStartSettup();
                // wait a little longer then the benchmark routine if exit false throw
                //var timeoutTime = BenchmarkTimeoutInSeconds(BenchmarkTimeInSeconds);
                //var exitSucces = BenchmarkHandle.WaitForExit(timeoutTime * 1000);
                // don't use wait for it breaks everything
                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                var keepRunning = true;
                while (keepRunning && IsActiveProcess(BenchmarkHandle.Id))
                {
                    //string outdata = BenchmarkHandle.StandardOutput.ReadLine();
                    //BenchmarkOutputErrorDataReceivedImpl(outdata);
                    // terminate process situations
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (benchmarkTimeWait + 2)
                        || BenchmarkSignalQuit
                        || BenchmarkSignalFinnished
                        || BenchmarkSignalHanged
                        || BenchmarkSignalTimedout
                        || BenchmarkException != null)
                    {
                        var imageName = MinerExeName.Replace(".exe", "");
                        // maybe will have to KILL process
                        KillProspectorClaymoreMinerBase(imageName);
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

                    // wait a second reduce CPU load
                    Thread.Sleep(1000);
                    var ad = GetSummaryAsync();

                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        Helpers.ConsolePrint(MinerTag(), ad.Result.Speed.ToString());
                        repeats++;
                        if (repeats > 20)//skip first 20s
                        {
                            summspeed += ad.Result.Speed;
                        }
                        if (repeats >= 40)
                        {
                            BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / 20, 1);//20s speed
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
                /*
                BenchmarkAlgorithm.BenchmarkSpeed = 0;
                // find latest log file
                string latestLogFile = "";
                var dirInfo = new DirectoryInfo(WorkingDirectory);
                foreach (var file in dirInfo.GetFiles(GetLogFileName()))
                {
                    latestLogFile = file.Name;
                    break;
                }

                BenchmarkHandle?.WaitForExit(10000);
                // read file log
                if (File.Exists(WorkingDirectory + latestLogFile))
                {
                    var lines = File.ReadAllLines(WorkingDirectory + latestLogFile);
                    ProcessBenchLinesAlternate(lines);
                }
                */
                BenchmarkThreadRoutineFinish();
            }
        }

        protected override void ProcessBenchLinesAlternate(string[] lines)
        {
            int kspeed = 1;
            foreach (var line in lines)
            {
                Helpers.ConsolePrint(MinerTag(), line);
                BenchLines.Add(line);
                var lineLowered = line.ToLower();

                if (lineLowered.Contains("total:".ToLower()))
                {
                    var st = lineLowered.IndexOf("total: ".ToLower());
                    var e = lineLowered.IndexOf("/s".ToLower());

                    if (lineLowered.Contains("kh/s"))
                        kspeed = 1000;
                    else if (lineLowered.Contains("mh/s"))
                        kspeed = 1000000;
                   // count++;
                    var parse = lineLowered.Substring(st + 7, e - st - 9).Trim().Replace(",", ".");
                    try
                    {
                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                    }

                    speed = speed + tmp;
                    BenchmarkAlgorithm.BenchmarkSpeed = speed;
                    /*
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                    }
                    */
                    //BenchmarkAlgorithm.BenchmarkSpeed = speed;
                }
            }
        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            Helpers.ConsolePrint(MinerTag(), outdata);
            return false;
        }
        #endregion
    }

}
