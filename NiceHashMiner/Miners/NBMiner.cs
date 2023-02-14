using Newtonsoft.Json;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Parsing;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Miners
{
    public class NBMiner : Miner
    {
        private class JsonApiResponse
        {
            public class MinerModel
            {
                public class DeviceModel
                {
                    public double hashrate_raw { get; set; }
                }
                public List<DeviceModel> devices { get; set; }
                public double total_hashrate2_raw { get; set; }
                public double total_hashrate_raw { get; set; }
            }
            public MinerModel miner { get; set; }
            public double? TotalHashrate => miner?.total_hashrate_raw;
            public double? TotalHashrate2 => miner?.total_hashrate2_raw;
        }
        private int _benchmarkTimeWait = 240;
        private int _targetBenchIters;
        private double _power = 0.0d;
        double _powerUsage = 0;

        private string AlgoName
        {
            get
            {
                switch (MiningSetup.CurrentAlgorithmType)
                {
                     case AlgorithmType.CuckooCycle:
                        return "cuckoo_ae";
                    case AlgorithmType.GrinCuckatoo32:
                        return "cuckatoo32";
                    case AlgorithmType.DaggerHashimoto:
                        return "ethash";
                    case AlgorithmType.ETCHash:
                        return "etchash";
                    case AlgorithmType.Autolykos:
                        return "ergo";
                    case AlgorithmType.KAWPOW:
                        return "kawpow";
                    case AlgorithmType.BeamV3:
                        return "beamv3";
                    case AlgorithmType.Octopus:
                        return "octopus";
                    default:
                        return "";
                }
            }
        }
        protected AlgorithmType SecondaryAlgorithmType = AlgorithmType.NONE;

        public NBMiner(AlgorithmType secondaryAlgorithmType) : base("NBMiner")
        {
            SecondaryAlgorithmType = secondaryAlgorithmType;
            IsMultiType = true;
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000;
        }

        private string GetServer(string algo, string username, string port)
        {
            string ret = "";
            string ssl = "";
            string psw = "x";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                ssl = "stratum+ssl://";
            }
            else
            {
                port = "1" + port;
                ssl = "stratum+tcp://";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) || MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                ssl = ssl.Replace("stratum", "nicehash");
            }
            int n = 0;
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " -o" + n.ToString() + " " + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://","") + ":9200 -u" + 
                        n.ToString() + " " + username + " -p" + n.ToString() + " " + psw + " ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " -o" + n.ToString() + " " + ssl + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u" + 
                        n.ToString() + " " + username + " -p" + n.ToString() + " " + psw + " ";
                }
                n++;
                if (n >= 3) break;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
            {
                ret = " -o" + n.ToString() + " " + Links.CheckDNS("grincuckatoo32.auto.nicehash.com").Replace("stratum+tcp://", "") + ":9200 -u" +
                                        n.ToString() + " " + username + " -p" + n.ToString() + " " + psw + " ";
            }

            return ret.Replace(" -o0", " -o").Replace(" -u0", " -u").Replace(" -p0", " -p");
        }
        private string GetStartCommand(string url, string btcAddress, string worker)
        {
            var cmd = "";
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) || MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                url = url.Replace("stratum", "nicehash");
            }

            var username = GetUsername(btcAddress, worker);
            string devs = "";
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            var platform = "";
            var extra = "";
            foreach (var mPair in sortedMinerPairs)
            {
                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    devs = string.Join(",", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));
                    platform = "--platform 1";
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
                }
                else
                {
                    devs = string.Join(",", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));
                    platform = "--platform 2";
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
                }
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                cmd = $"-a {AlgoName}" +
                    GetServer("daggerhashimoto", username, "3353") +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN --enable-dag-cache " + platform;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                cmd = $"-a {AlgoName}" +
                    GetServer("etchash", username, "3393") +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN --enable-dag-cache " + platform;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle))
            {
                cmd = $"-a {AlgoName}" +
                    GetServer("cuckoocycle", username, "3376") +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
            {
                cmd = $"-a {AlgoName}" +
                    GetServer("cuckatoo32", username, "3382????") +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                cmd = $"-a {AlgoName}" +
                    GetServer("kawpow", username, "3385") +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.BeamV3))
            {
                cmd = $"-a {AlgoName}" +
                    GetServer("beamv3", username, "3387") +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
            {
                cmd = $"-a {AlgoName}" +
                    GetServer("octopus", username, "3389") +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                cmd = $"-a {AlgoName}" +
                    GetServer("autolykos", username, "3390") +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            cmd += extra;

            return cmd;
        }

        public override void Start(string btcAdress, string worker)
        {
            string url = "";
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.NBMiner;
            IsApiReadException = false;
            ProcessHandle = _Start();
        }

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            _targetBenchIters = Math.Max(1, (int)Math.Floor(time / 20d));

            var url = GetServiceUrl(algorithm.NiceHashID);
            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            var username = GetUsername(btcAddress, worker);
            var cmd = "";

            string devs = "";
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            var platform = "";
            var extra = "";
            foreach (var mPair in sortedMinerPairs)
            {
                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    devs = string.Join(",", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));
                    platform = "--platform 1";
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
                }
                else
                {
                    devs = string.Join(",", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));
                    platform = "--platform 2";
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
                }
            }


            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                cmd = $"-a {AlgoName} -o " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020") + " -u 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.NBMiner" +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                cmd = $"-a {AlgoName} -o " + Links.CheckDNS("stratum+tcp://etc.2miners.com:1010") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505.NBMiner" +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle))
            {
                cmd = $"-a {AlgoName} -o " + Links.CheckDNS("stratum+tcp://ae.2miners.com:4040") + " -u ak_25J5KBhdHcsemmgmnaU4QpcRQ9xgKS5ChBwCaZcEUc85qkgcXE.nbminer" +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
            {
                cmd = $"-a cuckatoo32 -o " + Links.CheckDNS("stratum+tcp://grincuckatoo32.auto.nicehash.com:9200") + " -u " + Globals.DemoUser +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                cmd = $"-a {AlgoName} -o " + Links.CheckDNS("stratum+tcp://rvn.2miners.com:6060") + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.nbminer " +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.BeamV3))
            {
                cmd = $"-a {AlgoName} -o " + Links.CheckDNS("stratum+ssl://beam.2miners.com:5252") + " -u 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.nbminer " +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
            {
                cmd = $"-a {AlgoName} -o " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3094") + " -u cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd.nbminer " +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                cmd = $"-a {AlgoName} -o " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3100") + " -u 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.nbminer " +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            cmd += extra;
            _benchmarkTimeWait = time;
            return cmd;
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
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) || MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                _benchmarkTimeWait += 30;
            }
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

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) || MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
                    {
                        delay_before_calc_hashrate = 30;
                        MinerStartDelay = 30;
                    }

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
                            summspeed += ad.Result.Speed;
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

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            EndBenchmarkProcces();

                            break;
                        }
                    }
                }
                BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
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

        protected override bool IsApiEof(byte third, byte second, byte last)
        {
            return third == 0x7d && second == 0xa && last == 0x7d;
        }

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            
            string ResponseFromNBMiner;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/api/v1/status");
                WR.UserAgent = "GET / HTTP/1.1\r\n";
                WR.Timeout = 3 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 2 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromNBMiner = await Reader.ReadToEndAsync();
                if (ResponseFromNBMiner.Length == 0 || (ResponseFromNBMiner[0] != '{' && ResponseFromNBMiner[0] != '['))
                    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception)
            {
                return null;
            }

            ad = new ApiData(MiningSetup.CurrentAlgorithmType);

            //Helpers.ConsolePrint("NBMiner:", ResponseFromNBMiner);
            dynamic resp = JsonConvert.DeserializeObject<JsonApiResponse>(ResponseFromNBMiner);

            if (resp != null)
            {
                /*
                foreach (var d in resp.miner.devices)
                {
                    Helpers.ConsolePrint("NBMiner:", "d.hashrate_raw: " + d.hashrate_raw.ToString());
                }
                */
                var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                if (Form_Main.NVIDIA_orderBug)
                {
                    sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
                }
                double[] hashrates = new double[sortedMinerPairs.Count];
                for (var i = 0; i < sortedMinerPairs.Count; i++)
                {
                    hashrates[i] = resp.miner.devices[i].hashrate_raw;
                }
                int dev = 0;

                foreach (var mPair in sortedMinerPairs)
                {
                    _power = mPair.Device.PowerUsage;
                    mPair.Device.MiningHashrate = hashrates[dev];
                    dev++;
                }

                ad.Speed = resp.TotalHashrate ?? 0;
                if (ad.Speed != 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                } else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }
            }
            else
            {
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                Helpers.ConsolePrint("NBMiner:", "resp - null");
            }

            Thread.Sleep(10);
            return ad;
        }


        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }
    }
}
