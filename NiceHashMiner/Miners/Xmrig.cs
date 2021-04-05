using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Parsing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using System.Windows.Forms;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using NiceHashMiner.Devices;

namespace NiceHashMiner.Miners
{
    public class Xmrig : Miner
    {
        [DllImport("psapi.dll")]
        public static extern bool EmptyWorkingSet(IntPtr hProcess);

        private int _benchmarkTimeWait = 180;
        private const string LookForStart = "speed 10s/60s/15m";
        private const string LookForEnd = "h/s max";
        private System.Diagnostics.Process CMDconfigHandle;
        private string platform = "";
        string platform_prefix = "";
        public Xmrig() : base("Xmrig")
        { }
        public override void Start(string url, string btcAdress, string worker)
        {
            LastCommandLine = GetStartCommand(url, btcAdress, worker);

            ProcessHandle = _Start();
        }
        /*
        private string GetStartCommand(string url, string btcAdress, string worker)
        {
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.CPU);
            return $" -o {url} -u {btcAdress}.{worker}:x --nicehash {extras} --api-port {ApiPort}";
        }
        */
        public void FreeMem()
        {

            EmptyWorkingSet(Process.GetCurrentProcess().Handle);
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    EmptyWorkingSet(process.Handle);
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint(MinerTag(), ex.Message);
                }
            }

        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            if (platform == "")//cpu
            {
                return "";
            }
            var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.ID.ToString()).ToList();
            deviceStringCommand += string.Join(",", ids);

            return deviceStringCommand;
        }

        private string GetStartCommand(string url, string btcAdress, string worker)
        {
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.CPU);
            var algo = "cryptonightv7";
            var port = "3363";
            var variant = " --variant 1 ";

            string username = GetUsername(btcAdress, worker);

            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = " --no-cpu --cuda-devices=";
                }
                else if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    platform = " --no-cpu --opencl-devices=";
                }
                else if (pair.Device.DeviceType == DeviceType.CPU)
                {
                    platform = "";
                }
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX))
            {
                algo = "randomxmonero";
                port = "3380";
                variant = "";
                url = url.Replace("randomx", "randomxmonero");
                return $" --algo=rx/0 -o {url} {variant} -u {username} -p x --nicehash {extras} --http-port {ApiPort} --donate-level=1 "
               + $" -o stratum+tcp://{algo}.{Form_Main.myServers[1, 0]}.nicehash.com:{port} -u {username} -p x "
               + $" -o stratum+tcp://{algo}.{Form_Main.myServers[2, 0]}.nicehash.com:{port} -u {username} -p x "
               + $" -o stratum+tcp://{algo}.{Form_Main.myServers[3, 0]}.nicehash.com:{port} -u {username} -p x "
               + $" -o stratum+tcp://{algo}.{Form_Main.myServers[0, 0]}.nicehash.com:{port} -u {username} -p x {platform}"
               + GetDevicesCommandString().TrimStart();
            }
            return "unsupported algo";
        }
        private string GetStartBenchmarkCommand(string url, string btcAdress, string worker)
        {
            if (url.Contains("Auto"))
            {
                url = url.Replace("Auto", "eu");
            }

            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = " --no-cpu --cuda-devices=";
                    platform_prefix = "nvidia_";
                }
                else if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    platform = " --no-cpu --opencl-devices=";
                    platform_prefix = "amd_"; ;
                }
                else if (pair.Device.DeviceType == DeviceType.CPU)
                {
                    platform = "";
                    platform_prefix = "cpu_";
                }
            }

            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.CPU);
            var algo = "cryptonightv7";
            var port = "3363";
            var variant = " --variant 1 ";
            string username = GetUsername(btcAdress, worker);

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX))
            {
                algo = "randomxmonero";
                port = "3380";
                variant = "";
                return $" --algo=rx/0 -o stratum+tcp://pool.supportxmr.com:3333 -u 42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.benchmark -p x {extras} --http-port {ApiPort} --donate-level=1 "
                + $" -o stratum+tcp://{algo}.{Form_Main.myServers[0, 0]}.nicehash.com:{port} -u {username}:x {platform}"
               + GetDevicesCommandString().TrimStart();
            }
            return "unsupported algo";
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Helpers.ConsolePrint("XMRIG", "_Stop");
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5; // 5 min
        }

        protected async Task<ApiData> GetSummaryCpuAsyncXMRig(string method = "", bool overrideLoop = false)
        {
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/1/summary");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                var respStr = await Reader.ReadToEndAsync();

                Reader.Close();
                Response.Close();
                WR.Abort();
                SS.Close();
                //Helpers.ConsolePrint(MinerTag(), respStr);

                if (string.IsNullOrEmpty(respStr))
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.NETWORK_EXCEPTION;
                    throw new Exception("Response is empty!");
                }

                dynamic resp = JsonConvert.DeserializeObject(respStr);

                if (resp != null)
                {
                    JArray totals = resp.hashrate.total;
                    foreach (var total in totals)
                    {
                        if (total.Value<string>() == null) continue;
                        ad.Speed = total.Value<double>();
                        break;
                    }
                    foreach (var dev in sortedMinerPairs)
                    {
                        dev.Device.MiningHashrate = ad.Speed;
                    }

                    if (ad.Speed == 0)
                    {
                        CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                    }
                    else
                    {
                        CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                    }
                }
                else
                {
                    throw new Exception($"Response does not contain speed data: {respStr.Trim()}");
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ex.Message);
            }

            return ad;
        }


        public override async Task<ApiData> GetSummaryAsync()
        {
            return await GetSummaryCpuAsyncXMRig();
        }

        protected override bool IsApiEof(byte third, byte second, byte last)
        {
            return third == 0x7d && second == 0xa && last == 0x7d;
        }

        #region Benchmark

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
                Form_Main.myServers[0, 0],
                ConectionType);
            _benchmarkTimeWait = time;
            return GetStartBenchmarkCommand(server, Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim())
                + $" --print-time=10 --nicehash";
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

            }
        }



        protected override void ProcessBenchLinesAlternate(string[] lines)
        {
            // Xmrig reports 2.5s and 60s averages, so prefer to use 60s values for benchmark
            // but fall back on 2.5s values if 60s time isn't hit
            var twoSecTotal = 0d;
            var sixtySecTotal = 0d;
            var twoSecCount = 0;
            var sixtySecCount = 0;
            foreach (var line in lines)
            {
                BenchLines.Add(line);
                var lineLowered = line.ToLower();
                if (!lineLowered.Contains(LookForStart)) continue;
                var speeds = Regex.Match(lineLowered, $"{LookForStart} (.+?) {LookForEnd}").Groups[1].Value.Split();

                try {
                if (double.TryParse(speeds[1], out var sixtySecSpeed))
                    {
                    sixtySecTotal += sixtySecSpeed;
                    ++sixtySecCount;
                    }
                else if (double.TryParse(speeds[0], out var twoSecSpeed))
                    {
                    // Store 10s data in case 60s is never reached
                    twoSecTotal += twoSecSpeed;
                    ++twoSecCount;
                    }
                }
                catch
                {
                    MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    BenchmarkSignalFinnished = true;
                    return;
                }
            }

            if (sixtySecCount > 0 && sixtySecTotal > 0)
            {
                // Run iff 60s averages are reported
                BenchmarkAlgorithm.BenchmarkSpeed = sixtySecTotal / sixtySecCount;
            }
            else if (twoSecCount > 0)
            {
                // Run iff no 60s averages are reported but 2.5s are
                BenchmarkAlgorithm.BenchmarkSpeed = twoSecTotal / twoSecCount;
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
        /*
        protected override bool BenchmarkParseLine(string outdata)
        {
            Helpers.ConsolePrint(MinerTag(), outdata);
            return false;
        }
        */
        #endregion
    }
}
