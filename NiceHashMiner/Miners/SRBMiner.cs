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

        private const int TotalDelim = 2;
        int count = 0;
        private double speed = 0;
        private double tmp = 0;
        private bool IsInBenchmark = false;

        public SRBMiner() : base("SRBMiner") {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
        }

        public override void Start(string url, string btcAdress, string worker) {
            IsInBenchmark = false;
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.SRBMiner;

            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
        }

        private string GetStartCommand(string url, string btcAddress, string worker) {
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            string username = GetUsername(btcAddress, worker);
            url = url.Replace("stratum+tcp://", "");
            string ethurl = url.Replace("autolykos", "daggerhashimoto").Split(':')[0]; ;
            string zilurl = url.Split(':')[0];

            if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                var port = "3390";
                return $" --main-pool-reconnect 2 --disable-cpu --a0-is-zil --multi-algorithm-job-mode 3 " +
                    $"--algorithm ethash;autolykos2 " +
                    $"--pool {ethurl}:3353;{zilurl}:3390 " +
                    $"--pool daggerhashimoto.{Form_Main.myServers[1, 0]}.nicehash.com:3353;autolykos.{Form_Main.myServers[1, 0]}.nicehash.com:3390 " +
                    $"--pool daggerhashimoto.{Form_Main.myServers[2, 0]}.nicehash.com:3353;autolykos.{Form_Main.myServers[1, 0]}.nicehash.com:3390 " +
                    $"--pool daggerhashimoto.{Form_Main.myServers[3, 0]}.nicehash.com:3353;autolykos.{Form_Main.myServers[1, 0]}.nicehash.com:3390 " +
                    $"--pool daggerhashimoto.{Form_Main.myServers[0, 0]}.nicehash.com:3353;autolykos.{Form_Main.myServers[1, 0]}.nicehash.com:3390 " +
                    $"--wallet {username};{username}  --password x;x --api-enable --api-port {ApiPort} " +
               "--gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
            }
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
               + $" --pool stratum+tcp://daggerhashimoto.{Form_Main.myServers[1, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://daggerhashimoto.{Form_Main.myServers[2, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://daggerhashimoto.{Form_Main.myServers[3, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://daggerhashimoto.{Form_Main.myServers[0, 0]}.nicehash.com:{port} --wallet {username} --nicehash true " +
               "--gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                var port = "3390";
                return $" --main-pool-reconnect 2 --disable-cpu --algorithm autolykos2 --pool {url} --wallet {username} --nicehash true --api-enable --api-port {ApiPort} "
               + $" --pool stratum+tcp://autolykos.{Form_Main.myServers[1, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://autolykos.{Form_Main.myServers[2, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://autolykos.{Form_Main.myServers[3, 0]}.nicehash.com:{port} --wallet {username} --nicehash true "
               + $" --pool stratum+tcp://autolykos.{Form_Main.myServers[0, 0]}.nicehash.com:{port} --wallet {username} --nicehash true " +
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
            IsInBenchmark = true;
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
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                algo = "autolykos2";
                port = "3390";

                return $" --disable-cpu --algorithm autolykos2" +
                    $" --pool stratum+tcp://pool.eu.woolypooly.com:3100" +
                    $" --wallet 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
            }

            return "unknown";
        }

        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            StopDriver();
        }

        private void StopDriver()
        {
            //srbminer driver
            var CMDconfigHandleWD = new Process

            {
                StartInfo =
                {
                    FileName = "sc.exe"
                }
            };

            CMDconfigHandleWD.StartInfo.Arguments = "stop winio";
            CMDconfigHandleWD.StartInfo.UseShellExecute = false;
            CMDconfigHandleWD.StartInfo.CreateNoWindow = true;
            CMDconfigHandleWD.Start();
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5;  // 5 min
        }
        public override async Task<ApiData> GetSummaryAsync()
        {

            var ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType, MiningSetup.MiningPairs[0]);
            string ResponseFromSRBMiner;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString());
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 3 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 2 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromSRBMiner = await Reader.ReadToEndAsync();

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
            //Helpers.ConsolePrint("API ResponseFromSRBMiner:", ResponseFromSRBMiner.ToString());
            
            try
            {
                int totalsMain = 0;
                int totalsSecond = 0;
                if (resp != null)
                {
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                    int devs = sortedMinerPairs.Count;
                    if ((MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) ||
                        MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos)) &&
                        MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
                    {
                        devs = 0;
                        foreach (var mPair in sortedMinerPairs)
                        {
                            try
                            {
                                string token = $"algorithms[0].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash = resp.SelectToken(token);
                                int gpu_hr = (int)Convert.ToInt32(hash, CultureInfo.InvariantCulture.NumberFormat);
                                mPair.Device.MiningHashrate = gpu_hr;

                            } catch (Exception ex)
                            {
                                Helpers.ConsolePrint("API Exception:", ex.ToString());
                            }
                            devs++;
                        }

                        totalsMain = resp.algorithms[0].hashrate.gpu.total;
                    }
                    if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        devs = 0;
                        foreach (var mPair in sortedMinerPairs)
                        {
                            try
                            {
                                int gpu_hr1 = 0;
                                string token0 = $"algorithms[0].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash0 = resp.SelectToken(token0);
                                int gpu_hr0 = (int)Convert.ToInt32(hash0, CultureInfo.InvariantCulture.NumberFormat);
                                if (IsInBenchmark == false)
                                {
                                    string token1 = $"algorithms[1].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                    var hash1 = resp.SelectToken(token1);
                                    gpu_hr1 = (int)Convert.ToInt32(hash1, CultureInfo.InvariantCulture.NumberFormat);
                                } else
                                {
                                    gpu_hr1 = 0;
                                }
                                if (gpu_hr0 > 0)
                                {
                                    mPair.Device.MiningHashrate = gpu_hr0;
                                }
                                else
                                {
                                    mPair.Device.MiningHashrate = gpu_hr1;
                                }
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("API Exception:", ex.ToString());
                            }
                            devs++;
                        }
                        if (IsInBenchmark == false)
                        {
                            totalsMain = resp.algorithms[1].hashrate.gpu.total;
                            totalsSecond = resp.algorithms[0].hashrate.gpu.total;
                        } else
                        {
                            totalsMain = resp.algorithms[0].hashrate.gpu.total;
                        }
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX))
                    {
                        totalsMain = resp.algorithms[0].hashrate.cpu.total;
                        foreach (var mPair in sortedMinerPairs)
                        {
                                mPair.Device.MiningHashrate = totalsMain;
                        }
                    }
                    
                    ad.Speed = totalsMain;
                    ad.SecondarySpeed = totalsSecond;

                    if (ad.Speed + ad.SecondarySpeed == 0)
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
                Helpers.ConsolePrint("API error", ex.ToString());
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
                double BenchmarkSpeed = 0.0d;
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
                            BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
                            Helpers.ConsolePrint(MinerTag(), "Benchmark ended. BenchmarkSpeed: " + BenchmarkSpeed.ToString());
                            ad.Dispose();
                            benchmarkTimer.Stop();

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                            {
                                //EndBenchmarkProcces();
                            }
                            StopDriver();
                            break;
                        }

                    }
                }

                if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    BenchmarkAlgorithm.BenchmarkProgressPercent = -1;
                    BenchmarkThreadRoutineSecond();
                }
                BenchmarkAlgorithm.BenchmarkSpeed = BenchmarkSpeed;

            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ex.ToString());
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {
                EndBenchmarkProcces();
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
        private void BenchmarkThreadRoutineSecond()
        {
            if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                BenchmarkSignalQuit = false;
                BenchmarkSignalHanged = false;
                BenchmarkSignalFinnished = false;
                BenchmarkException = null;
                double repeats = 0;
                double summspeedSecond = 0.0d;

                int delay_before_calc_hashrate = 10;
                int MinerStartDelay = 10;

                Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

                try
                {
                    var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
                    string secondcommandLine = $" --disable-cpu --algorithm ethash" +
                    $" --pool stratum+tcp://us-east.ethash-hub.miningpoolhub.com:20565" +
                    $" --wallet angelbbs.SRBMiner --nicehash true" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;

                    Helpers.ConsolePrint("BENCHMARK", "Second Benchmark starts");
                    Helpers.ConsolePrint(MinerTag(), "Second Benchmark should end in: " + _benchmarkTimeWait + " seconds");
                    BenchmarkHandle = BenchmarkStartProcess((string)secondcommandLine);
                    //BenchmarkHandle.WaitForExit(_benchmarkTimeWait + 2);
                    var secondbenchmarkTimer = new Stopwatch();
                    secondbenchmarkTimer.Reset();
                    secondbenchmarkTimer.Start();

                    BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                    BenchmarkThreadRoutineStartSettup(); //need for benchmark log
                    while (IsActiveProcess(BenchmarkHandle.Id))
                    {
                        if (secondbenchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 60)
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
                                summspeedSecond += ad.Result.Speed;
                            }
                            else
                            {
                                Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                            }

                            if (repeats >= _benchmarkTimeWait - MinerStartDelay - 15)
                            {
                                Helpers.ConsolePrint(MinerTag(), "Benchmark ended");
                                ad.Dispose();
                                secondbenchmarkTimer.Stop();

                                BenchmarkHandle.Kill();
                                BenchmarkHandle.Dispose();
                                //EndBenchmarkProcces();
                                StopDriver();
                                break;
                            }

                        }
                    }
                    BenchmarkAlgorithm.BenchmarkSecondarySpeed = Math.Round(summspeedSecond / (repeats - delay_before_calc_hashrate), 2);
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint(MinerTag(), ex.ToString());
                    BenchmarkThreadRoutineCatch(ex);
                }
                finally
                {
                    //BenchmarkThreadRoutineFinish();
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
