using Newtonsoft.Json;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
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
    public abstract class ClaymoreBaseMiner : Miner
    {
        protected int BenchmarkTimeWait = 2 * 45; // Ok... this was all wrong
        protected string LookForStart;
        protected string LookForEnd = "h/s";
        protected string SecondaryLookForStart;
        private int _benchmarkTimeWait = 180;
        private double _power = 0.0d;
        double _powerUsage = 0;

        // only dagger change
        protected bool IgnoreZero = false;

        protected double ApiReadMult = 1;
        protected AlgorithmType SecondaryAlgorithmType = AlgorithmType.NONE;

        // CD intensity tuning
        protected const int defaultIntensity = 30;

        protected ClaymoreBaseMiner(string minerDeviceName)
            : base(minerDeviceName)
        {
            ConectionType = NhmConectionType.STRATUM_SSL;
            IsKillAllUsedMinerProcs = true;
        }

        // return true if a secondary algo is being used
        public bool IsDual()
        {
            return (SecondaryAlgorithmType != AlgorithmType.NONE);
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5; // 5 minute max, whole waiting time 75seconds
        }

        private class JsonApiResponse
        {
#pragma warning disable IDE1006 // Naming Styles
            public List<string> result { get; set; }
            public int id { get; set; }
            public object error { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            
            JsonApiResponse resp = null;
            try
            {
                var bytesToSend = Encoding.ASCII.GetBytes("{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\"}n");
                var client = new TcpClient("127.0.0.1", ApiPort);
                var nwStream = client.GetStream();
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                var bytesToRead = new byte[client.ReceiveBufferSize];
                var bytesRead = await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                var respStr = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                resp = JsonConvert.DeserializeObject<JsonApiResponse>(respStr, Globals.JsonSettings);
                client.Close();
                //Helpers.ConsolePrint("ClaymoreMiner API: ", respStr);
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
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.NeoScrypt))
                    {
                        ApiReadMult = 1000;
                    }
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderByDescending(pair => pair.Device.DeviceType)
                                .ThenBy(pair => pair.Device.IDByBus).ToList();
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
                        sortedMinerPairs[dev].Device.MiningHashrate = tmpSpeed * ApiReadMult;
                        _power = sortedMinerPairs[dev].Device.PowerUsage;
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
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                mPair.Device.MiningHashrate = 0;
            }
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected virtual string DeviceCommand(int amdCount = 1)
        {
            return " -di ";
        }


        protected override string GetDevicesCommandString()
        {
            // First by device type (AMD then NV), then by bus ID index
            var sortedMinerPairs = MiningSetup.MiningPairs
                .OrderByDescending(pair => pair.Device.DeviceType)
                .ThenBy(pair => pair.Device.IDByBus)
                .ToList();
            var extraParams = ExtraLaunchParametersParser.ParseForMiningPairs(sortedMinerPairs, DeviceType.AMD);

            var ids = new List<string>();
            var intensities = new List<string>();

            var amdDeviceCount = ComputeDeviceManager.Query.AmdDevices.Count;
            Helpers.ConsolePrint("ClaymoreIndexing", $"Found {amdDeviceCount} AMD devices");

            foreach (var mPair in sortedMinerPairs)
            {
                var id = mPair.Device.IDByBus;
                if (id < 0)
                {
                    // should never happen
                    Helpers.ConsolePrint("ClaymoreIndexing", "ID by Bus too low: " + id + " skipping device");
                    continue;
                }

                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    Helpers.ConsolePrint("ClaymoreIndexing", "NVIDIA device increasing index by " + amdDeviceCount);
                    id += amdDeviceCount;
                }

                if (id > 9)
                {
                    // New >10 GPU support in CD9.8
                    if (id < 36)
                    {
                        // CD supports 0-9 and a-z indexes, so 36 GPUs
                        var idchar = (char)(id + 87); // 10 = 97(a), 11 - 98(b), etc
                        ids.Add(idchar.ToString());
                    }
                    else
                    {
                        Helpers.ConsolePrint("ClaymoreIndexing", "ID " + id + " too high, ignoring");
                    }
                }
                else
                {
                    ids.Add(id.ToString());
                }

                if (mPair.Algorithm is DualAlgorithm algo && algo.TuningEnabled)
                {
                    intensities.Add(algo.CurrentIntensity.ToString());
                }
            }

            var deviceStringCommand = DeviceCommand(amdDeviceCount) + string.Join("", ids);
            var intensityStringCommand = "";
            if (intensities.Count > 0)
            {
                intensityStringCommand = " -dcri " + string.Join(",", intensities);
            }
            return deviceStringCommand + intensityStringCommand + extraParams;
        }

        // benchmark stuff


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
            if (ConfigManager.GeneralConfig.StandartBenchmarkTime)
            {
                _benchmarkTimeWait = 60;
            }
            else
            {
                _benchmarkTimeWait = 180;
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

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.NeoScrypt))
                    {
                        MinerStartDelay = 15;
                        delay_before_calc_hashrate = 5;
                    }

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        _powerUsage += _power;
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
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
                BenchmarkAlgorithm.PowerUsageBenchmark = (_powerUsage / repeats);
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {
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
                BenchmarkThreadRoutineFinish();
            }
        }

        // stub benchmarks read from file
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            //Helpers.ConsolePrint(MinerTag(), outdata);
            CheckOutdata(outdata);
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            //Helpers.ConsolePrint("BenchmarkParseLine", outdata);
            return true;
        }

        protected double GetNumber(string outdata)
        {
            return GetNumber(outdata, LookForStart, LookForEnd);
        }

        protected double GetNumber(string outdata, string LOOK_FOR_START, string LOOK_FOR_END)
        {
            try
            {
                double mult = 1;
                var speedStart = outdata.IndexOf(LOOK_FOR_START, StringComparison.Ordinal);
                var speed = outdata.Substring(speedStart, outdata.Length - speedStart);
                speed = speed.Replace(LOOK_FOR_START, "");
                speed = speed.Substring(0, speed.IndexOf(LOOK_FOR_END, StringComparison.Ordinal));

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
                return (double.Parse(speed, CultureInfo.InvariantCulture) * mult);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetNumber",
                    ex.Message + " | args => " + outdata + " | " + LOOK_FOR_END + " | " + LOOK_FOR_START);
            }

            return 0;
        }
    }
}
