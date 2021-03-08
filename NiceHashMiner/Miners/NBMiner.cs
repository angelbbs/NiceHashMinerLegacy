using NiceHashMiner.Algorithms;
using NiceHashMiner.Miners.Parsing;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NiceHashMiner.Configs;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;
using NiceHashMiner.Devices;

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
        private string[,] myServers = Form_Main.myServers;

        private string AlgoName
        {
            get
            {
                switch (MiningSetup.CurrentAlgorithmType)
                {
                    case AlgorithmType.GrinCuckaroo29:
                        return "cuckaroo";
                    case AlgorithmType.GrinCuckarood29:
                        return "cuckarood";
                    case AlgorithmType.Cuckaroom:
                        return "cuckaroom";
                    case AlgorithmType.GrinCuckatoo31:
                        return "cuckatoo";
                    case AlgorithmType.GrinCuckatoo32:
                        return "cuckatoo32";
                    case AlgorithmType.CuckooCycle:
                        return "cuckoo_ae";
                    case AlgorithmType.DaggerHashimoto:
                        return "ethash";
                    
                    case AlgorithmType.KAWPOW:
                        return "kawpow";
                        /*
                    case AlgorithmType.Cuckaroo29BFC:
                        return "bfc";
                        */
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

        private string GetStartCommand(string url, string btcAddress, string worker)
        {
            var cmd = "";
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                url = url.Replace("stratum", "ethnh");
            }

            var user = GetUsername(btcAddress, worker);
            string devs = "";
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            var platform = "";
            var extra = "";
            foreach (var mPair in sortedMinerPairs)
            {
                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    devs = string.Join(",", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));
                    platform = "--platform 1 --no-watchdog";
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
                }
                else
                {
                    devs = string.Join(",", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));
                    platform = "--platform 2 --no-watchdog";
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
                }
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 ethnh+tcp://daggerhashimoto." + myServers[1, 0] + ".nicehash.com:3353 -u1 " + user +
                    $" -o2 ethnh+tcp://daggerhashimoto." + myServers[2, 0] + ".nicehash.com:3353 -u2 " + user  +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckaroo29))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 stratum+tcp://grincuckaroo29." + myServers[1, 0] + ".nicehash.com:3371 -u1 " + user +
                    $" -o2 stratum+tcp://grincuckaroo29." + myServers[2, 0] + ".nicehash.com:3371 -u2 " + user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckarood29))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 nicehash+tcp://grincuckarood29." + myServers[1, 0] + ".nicehash.com:3377 -u1 " + user +
                    $" -o2 stratum+tcp://grincuckarood29." + myServers[2, 0] + ".nicehash.com:3377 -u2 " + user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo31))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 stratum+tcp://grincuckatoo31." + myServers[1, 0] + ".nicehash.com:3372 -u1 " + user +
                    $" -o2 stratum+tcp://grincuckatoo31." + myServers[2, 0] + ".nicehash.com:3372 -u2 " +user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 stratum+tcp://grincuckatoo32." + myServers[1, 0] + ".nicehash.com:3383 -u1 " + user +
                    $" -o2 stratum+tcp://grincuckatoo32." + myServers[2, 0] + ".nicehash.com:3383 -u2 " + user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 stratum+tcp://cuckoocycle." + myServers[1, 0] + ".nicehash.com:3376 -u1 " + user +
                    $" -o2 stratum+tcp://cuckoocycle." + myServers[2, 0] + ".nicehash.com:3376 -u2 " + user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            /*
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Cuckaroo29BFC))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 stratum+tcp://cuckaroo29bfc." + myServers[1, 0] + ".nicehash.com:3386 -u1 " + user +
                    $" -o2 stratum+tcp://cuckaroo29bfc." + myServers[2, 0] + ".nicehash.com:3386 -u2 " + user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            */
            
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 stratum+tcp://kawpow." + myServers[1, 0] + ".nicehash.com:3385 -u1 " + user +
                    $" -o2 stratum+tcp://kawpow." + myServers[2, 0] + ".nicehash.com:3385 -u2 " + user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.BeamV3))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 stratum+tcp://beamv3." + myServers[1, 0] + ".nicehash.com:3387 -u1 " + user +
                    $" -o2 stratum+tcp://beamv3." + myServers[2, 0] + ".nicehash.com:3387 -u2 " + user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
            {
                cmd = $"-a {AlgoName} -o {url} -u {user} -o1 stratum+tcp://octopus." + myServers[1, 0] + ".nicehash.com:3389 -u1 " + user +
                    $" -o2 stratum+tcp://octopus." + myServers[2, 0] + ".nicehash.com:3389 -u2 " + user +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            cmd += extra;

            return cmd;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.NBMiner;
            IsApiReadException = false;
            ProcessHandle = _Start();
        }

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            _targetBenchIters = Math.Max(1, (int) Math.Floor(time / 20d));

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
                } else
                {
                    devs = string.Join(",", MiningSetup.MiningPairs.Select(p => p.Device.IDByBus));
                    platform = "--platform 2";
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
                }
            }


            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                cmd = $"-a {AlgoName} -o ethproxy+tcp://eu1.ethermine.org:4444 -u 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.NBMiner -o1 nicehash+tcp://daggerhashimoto." + myServers[0, 0] + ".nicehash.com:3353 -u1 " + username +
                    $" -o2 nicehash+tcp://daggerhashimoto." + myServers[1, 0] + ".nicehash.com:3353 -u2 " + username +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckaroo29))
            {
                //cmd = $"-a {AlgoName} -o stratum+tcp://grin.sparkpool.com:6666 -u angelbbs@mail.ru.{worker} -o1 stratum+tcp://grincuckaroo29." + myServers[0, 0] + nhsuff + ".nicehash.com:3371 -u1 " + username +
                cmd = $"-a {AlgoName} -o stratum+tcp://grincuckaroo29." + myServers[0, 0] + ".nicehash.com:3371 -u " + username +
                    $" -o1 stratum+tcp://grincuckaroo29." + myServers[1, 0] + ".nicehash.com:3371 -u1 " + username +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckarood29))
            {
                cmd = $"-a {AlgoName} -o stratum+tcp://grincuckarood29." + myServers[0, 0] + ".nicehash.com:3377 -u " + username +
                    $" -o1 stratum+tcp://grincuckarood29." + myServers[1, 0] + ".nicehash.com:3377 -u1 " + username +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo31))
            {
                cmd = $"-a {AlgoName} -o stratum+tcp://mwc.2miners.com:1111 -u 2aHR0cHM6Ly9td2MuaG90Yml0LmlvLzcyOTkyMw.nbminer -o1 stratum+tcp://grincuckatoo31." + myServers[0, 0] + ".nicehash.com:3372 -u1 " + username +
                    $" -o2 stratum+tcp://grincuckatoo31." + myServers[1, 0] + ".nicehash.com:3372 -u2 " + username +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
            {
                cmd = $"-a {AlgoName} -o stratum+tcp://grin.2miners.com:3030 -u grin16ek8qgx29ssku0q2cxez7830gh9ndw3ek5yzxe26x34s09528d2sldl6td.nbminer -o1 stratum+tcp://grincuckatoo32." + myServers[0, 0] + ".nicehash.com:3383 -u1 " + username +
                    $" -o2 stratum+tcp://grincuckatoo32." + myServers[1, 0] + ".nicehash.com:3383 -u2 " + username +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle))
            {
                cmd = $"-a {AlgoName} -o stratum+tcp://ae.2miners.com:4040 -u ak_2f9AMwztStKs5roPmT592wTbUEeTyqRgYVZNrc5TyZfr94m7fM.nbminer -o1 stratum+tcp://cuckoocycle." + myServers[0, 0] + ".nicehash.com:3376 -u1 " + username +
                    $" -o2 stratum+tcp://cuckoocycle." + myServers[1, 0] + ".nicehash.com:3376 -u2 " + username +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            /*
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Cuckaroo29BFC))
            {
                cmd = $"-a {AlgoName} -o stratum+tcp://bfc.f2pool.com:4900 -u angelbbs.nbminer -o1 stratum+tcp://cuckaroo29bfc." + myServers[0, 0] + ".nicehash.com:3386 -u1 " + username +
                    $" -o2 stratum+tcp://cuckaroo29bfc." + myServers[1, 0] + ".nicehash.com:3386 -u2 " + username +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            */
            
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                cmd = $"-a {AlgoName} -o stratum+tcp://rvn.2miners.com:6060 -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.nbminer " +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.BeamV3))
            {
                cmd = $"-a {AlgoName} -o stratum+ssl://beam.2miners.com:5252 -u 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.nbminer " +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
            {
                cmd = $"-a {AlgoName} -o stratum+tcp://cfx.woolypooly.com:3094 -u 0x13097ee19fd453AfD6F2ecf155927f2b7380307F.nbminer " +
                    $" --api 127.0.0.1:{ApiPort} -d {devs} -RUN " + platform;
            }
            cmd += extra;
            _benchmarkTimeWait = time;
            return cmd;
            // return GetStartCommand(url, btc, worker);
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
            int MinerStartDelay = 20;

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
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 10);
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

        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            ApiData ad = new ApiData(MiningSetup.CurrentAlgorithmType);


            ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            
            string ResponseFromNBMiner;
            double total = 0;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/api/v1/status");
                WR.UserAgent = "GET / HTTP/1.1\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromNBMiner = await Reader.ReadToEndAsync();
                if (ResponseFromNBMiner.Length == 0 || (ResponseFromNBMiner[0] != '{' && ResponseFromNBMiner[0] != '['))
                    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
            //Helpers.ConsolePrint("NBMiner:", ResponseFromNBMiner);
            dynamic resp = JsonConvert.DeserializeObject<JsonApiResponse>(ResponseFromNBMiner);
            //Helpers.ConsolePrint("NBMiner-resp:", resp);

            if (resp != null)
            {

                ad.Speed = resp.TotalHashrate ?? 0;
                
                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
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
        /*
        protected override bool BenchmarkParseLine(string outdata)
        {
            return false;
        }
        */
    }
}
