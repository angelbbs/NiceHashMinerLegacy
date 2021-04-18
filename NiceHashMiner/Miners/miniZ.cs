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
using System.Windows.Forms;
using NiceHashMiner.Devices;

namespace NiceHashMiner.Miners
{
    public class miniZ : Miner
    {
#pragma warning disable IDE1006
        private class Result
        {
            public uint gpuid { get; set; }
            public uint cudaid { get; set; }
            public string busid { get; set; }
            public uint gpu_status { get; set; }
            public int solver { get; set; }
            public int temperature { get; set; }
            public uint gpu_power_usage { get; set; }
            public double speed_sps { get; set; }
            public uint accepted_shares { get; set; }
            public uint rejected_shares { get; set; }
        }

        private class JsonApiResponse
        {
            public uint id { get; set; }
            public string method { get; set; }
            public object error { get; set; }
            public List<Result> result { get; set; }
        }
#pragma warning restore IDE1006

        private int _benchmarkTimeWait = 2 * 45;
        private const string LookForStart = "(";
        private const string LookForEnd = ")sol/s";
        private double prevSpeed = 0;
        private bool firstStart = true;

        public miniZ() : base("miniZ")
        {
            ConectionType = NhmConectionType.NONE;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            IsApiReadException = false;
            firstStart = true;
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
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

            private string GetStartCommand(string url, string btcAddress, string worker)
        {
            var server = url.Split(':')[0].Replace("stratum+tcp://", "");
            var algo = "";
            var algoName = "";
            string username = GetUsername(btcAddress, worker);
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                algo = "144,5";
                algoName = "zhash";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                algo = "150,5,3";
                algoName = "beamv2";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                algo = "beam3";
                algoName = "beamv3";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                algo = "ethash";
                algoName = "daggerhashimoto";
            }
            string sColor = "";
            if ( GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sColor = " --nocolor";
            }


            var ret = GetDevicesCommandString()
                      + sColor + " --pers auto --par=" + algo
                      + " --url " + username + "@" + server + ":" + url.Split(':')[1]
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[1, 0] + ".nicehash.com:" + url.Split(':')[1]
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[2, 0] + ".nicehash.com:" + url.Split(':')[1]
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[3, 0] + ".nicehash.com:" + url.Split(':')[1]
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + url.Split(':')[1]
                      + " --pass=x" + " --telemetry=" + ApiPort;

            return ret;
        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = MiningSetup.MiningPairs.Aggregate(" --cuda-devices ",
                (current, nvidiaPair) => current + (nvidiaPair.Device.IDByBus + " "));

            deviceStringCommand +=
                " " + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);

            return deviceStringCommand;
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
            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
                Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], ConectionType).Replace("stratum+tcp://", "");
            var algo = "";
            var algoName = "";
            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);
            var stratumPort = "3369";
            var ret = "";

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                algo = "144,5";
                algoName = "zhash";
                ret = GetDevicesCommandString() + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA)
                      + " --pers auto --par=" + algo
                      + " --url GeKYDPRcemA3z9okSUhe9DdLQ7CRhsDBgX.miniz" + ".nhmlff" + "@btg.2miners.com:4040 -p x"
                      + " --url 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2" + ".nhmlff" + "@equihash144.eu.mine.zpool.ca:2144 -p c=BTC"
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + stratumPort
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[1, 0] + ".nicehash.com:" + stratumPort
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[2, 0] + ".nicehash.com:" + stratumPort
                      + " --pass=x" + " --telemetry=" + ApiPort;
                _benchmarkTimeWait = time;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                algo = "beam3";
                algoName = "beamv3";
                stratumPort = "3387";
                ret = GetDevicesCommandString() + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA)
                      + " --pers auto --par=" + algo
                      + " --url ssl://2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.miniz@beam.2miners.com:5252"
                      + " --url ssl://2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.miniz@beam.f2pool.com:5000"
                      //+ " --url ssl://2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9." + worker + "@beam-eu.sparkpool.com:2222"
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[0, 0] + ".nicehash.com:" + stratumPort
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[1, 0] + ".nicehash.com:" + stratumPort
                      + " --url " + username + "@" + algoName + "." + Form_Main.myServers[2, 0] + ".nicehash.com:" + stratumPort
                      + " --pass=x" + " --telemetry=" + ApiPort;
                _benchmarkTimeWait = time;
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                algo = "ethash";
                ret = GetDevicesCommandString() + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA)
                      + " --par=" + algo
                      + " --url tcp://0x9290E50e7CcF1bdC90da8248a2bBaCc5063AeEE1.miniz@eu1.ethermine.org:4444"
                      + " --pass=x" + " --telemetry=" + ApiPort;
                _benchmarkTimeWait = time;
            }
            return ret;
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
                    /*
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 20;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.X16RV2))
                    {
                        delay_before_calc_hashrate = 2;
                        MinerStartDelay = 10;
                    }
                    */
                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        //ComputeDevice.BenchmarkProgress = (int)(benchProgress * 100);
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
        /*
        protected override bool BenchmarkParseLine(string outdata)
        {
            Helpers.ConsolePrint("BENCHMARK", outdata);
            return false;
        }
        */
        protected double GetNumber(string outdata)
        {
            return GetNumber(outdata, LookForStart, LookForEnd);
        }

        protected double GetNumber(string outdata, string lookForStart, string lookForEnd)
        {
            try
            {
                double mult = 1;
                var speedStart = outdata.IndexOf(lookForStart.ToLower());
                var speed = outdata.Substring(speedStart, outdata.Length - speedStart);
                speed = speed.Replace(lookForStart.ToLower(), "");
                speed = speed.Substring(0, speed.IndexOf(lookForEnd.ToLower()));

                if (speed.Contains("k"))
                {
                    mult = 1000;
                    speed = speed.Replace("k", "");
                }
                else if (speed.Contains("m"))
                {
                    mult = 1000000;
                    speed = speed.Replace("m", "");
                }

                //Helpers.ConsolePrint("speed", speed);
                speed = speed.Trim();
                try
                {
                    return double.Parse(speed, CultureInfo.InvariantCulture) * mult;
                }
                catch
                {
                    MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    BenchmarkSignalFinnished = true;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetNumber",
                    ex.Message + " | args => " + outdata + " | " + lookForEnd + " | " + lookForStart);
                MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 0;
        }

        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);

            if (firstStart)
  //          if (ad.Speed <= 0.0001)
            {
               Thread.Sleep(5000);
               ad.Speed = 0;
                firstStart = false;
                return ad;
            }

            JsonApiResponse resp = null;
            try
            {
                var bytesToSend = Encoding.ASCII.GetBytes(variables.miniZ_toSend);
                var client = new TcpClient("127.0.0.1", ApiPort);
                var nwStream = client.GetStream();
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                var bytesToRead = new byte[client.ReceiveBufferSize];
                var bytesRead = await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                var respStr = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                //Helpers.ConsolePrint("miniZ API:", respStr);
                if (!respStr.Contains("}]}") && prevSpeed != 0)
                {
                    client.Close();
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                    ad.Speed = prevSpeed;
                    return ad;
                }
                resp = JsonConvert.DeserializeObject<JsonApiResponse>(respStr, Globals.JsonSettings);
                client.Close();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ex.Message);
                //CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                ad.Speed = prevSpeed;
            }
            try
            {
                if (resp != null && resp.error == null)
                {
                    ad.Speed = resp.result.Aggregate<Result, double>(0, (current, t1) => current + t1.speed_sps);
                    double[] hashrates = new double[resp.result.Count];
                    for (var i = 0; i < resp.result.Count; i++)
                    {
                        //total = total + resp.devices[i].speed;
                        hashrates[i] = resp.result[i].speed_sps;
                    }
                    int dev = 0;
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                    if (Form_Main.NVIDIA_orderBug)
                    {
                        sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
                    }
                    foreach (var mPair in sortedMinerPairs)
                    {
                        mPair.Device.MiningHashrate = hashrates[dev];
                        dev++;
                    }

                    prevSpeed = ad.Speed;
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                    if (ad.Speed == 0)
                    {
                        CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ex.ToString());
                //CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                ad.Speed = prevSpeed;
            }
            return ad;
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5; // 5 minute max, whole waiting time 75seconds
        }
    }
}
