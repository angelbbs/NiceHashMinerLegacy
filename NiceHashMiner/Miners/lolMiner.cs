using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Management;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System.Threading;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using static NiceHashMiner.Devices.ComputeDeviceManager;

namespace NiceHashMiner.Miners
{
    class lolMiner : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        private int _benchmarkTimeWait = 180;
        private string[,] myServers = Form_Main.myServers;
        public lolMiner()
            : base("lolMiner")
        {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
            IsKillAllUsedMinerProcs = true;
            IsNeverHideMiningWindow = true;

        }

        protected override int GetMaxCooldownTimeInMilliseconds() {
            return 60*1000;
        }

        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            string username = GetUsername(btcAdress, worker);
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.lolMiner;
            IsApiReadException = false;

            //add failover
            string alg = url.Substring(url.IndexOf("://") + 3, url.IndexOf(".") - url.IndexOf("://") - 3);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);

            //var algo = "";
            url = url.Replace("stratum+tcp://", "");
            //url = url.Substring(0, url.IndexOf(":"));
            var apiBind = " --apiport " + ApiPort;

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                LastCommandLine = "--coin AUTO144_5 --pool " + url + " --user " + username + " --pass x" +
                    " --pool zhash." + myServers[1, 0] + ".nicehash.com:3369 " +" --user " + username + " --pass x" +
                    " --pool zhash." + myServers[2, 0] + ".nicehash.com:3369 " +" --user " + username + " --pass x" +
                    " --pool zhash." + myServers[3, 0] + ".nicehash.com:3369 " +" --user " + username + " --pass x" +
                    apiBind + " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                LastCommandLine = "--algo BEAM-II --pool " + url + " --user " + username + " --pass x --tls 0" +
                " --pool beamv2." + myServers[1, 0] + ".nicehash.com:3378 " + " --user " + username + " --pass x --tls 0" +
                " --pool beamv2." + myServers[2, 0] + ".nicehash.com:3378 " + " --user " + username + " --pass x --tls 0" +
                " --pool beamv2." + myServers[3, 0] + ".nicehash.com:3378 " + " --user " + username + " --pass x --tls 0" +
                             apiBind + " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                LastCommandLine = "--algo BEAM-III --pool " + url + " --user " + username + " --pass x --tls 0" +
                " --pool beamv3." + myServers[1, 0] + ".nicehash.com:3387 " + " --user " + username + " --pass x --tls 0" +
                " --pool beamv3." + myServers[2, 0] + ".nicehash.com:3387 " + " --user " + username + " --pass x --tls 0" +
                " --pool beamv3." + myServers[3, 0] + ".nicehash.com:3387 " + " --user " + username + " --pass x --tls 0" +
                             apiBind + " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                LastCommandLine = "--coin MWC-C31 --pool " + url + " --user " + username + " --pass x" +
                " --pool grincuckatoo31." + myServers[1, 0] + ".nicehash.com:3372 " + " --user " + username + " --pass x" +
                " --pool grincuckatoo31." + myServers[2, 0] + ".nicehash.com:3372 " + " --user " + username + " --pass x" +
                " --pool grincuckatoo31." + myServers[3, 0] + ".nicehash.com:3372 " + " --user " + username + " --pass x" +
                apiBind + " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                LastCommandLine = "--coin GRIN-C32 --pool " + url + " --user " + username + " --pass x" +
                " --pool grincuckatoo32." + myServers[1, 0] + ".nicehash.com:3383 " + " --user " + username + " --pass x" +
                " --pool grincuckatoo32." + myServers[2, 0] + ".nicehash.com:3383 " + " --user " + username + " --pass x" +
                " --pool grincuckatoo32." + myServers[3, 0] + ".nicehash.com:3383 " + " --user " + username + " --pass x" +
                apiBind + " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                LastCommandLine = "--coin MWC-C29D --pool " + url + " --user " + username + " --pass x" +
                " --pool grincuckarood29." + myServers[1, 0] + ".nicehash.com:3377 " + " --user " + username + " --pass x" +
                " --pool grincuckarood29." + myServers[2, 0] + ".nicehash.com:3377 " + " --user " + username + " --pass x" +
                " --pool grincuckarood29." + myServers[3, 0] + ".nicehash.com:3377 " + " --user " + username + " --pass x" +
                apiBind + " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                LastCommandLine = "--coin GRIN-C29M --pool " + url + " --user " + username + " --pass x" +
                " --pool cuckaroom." + myServers[1, 0] + ".nicehash.com:3382 " + " --user " + username + " --pass x" +
                " --pool cuckaroom." + myServers[2, 0] + ".nicehash.com:3382 " + " --user " + username + " --pass x" +
                " --pool cuckaroom." + myServers[3, 0] + ".nicehash.com:3382 " + " --user " + username + " --pass x" +
                apiBind + " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                LastCommandLine = "--algo ETHASH --ethstratum=ETHV1 --pool " + url + " --user " + username + " --pass x" +
                " --pool daggerhashimoto." + myServers[1, 0] + ".nicehash.com:3353 " + " --user " + username + " --pass x" +
                " --pool daggerhashimoto." + myServers[2, 0] + ".nicehash.com:3353 " + " --user " + username + " --pass x" +
                " --pool daggerhashimoto." + myServers[3, 0] + ".nicehash.com:3353 " + " --user " + username + " --pass x" +
                apiBind + " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            LastCommandLine += GetDevicesCommandString() + " ";//
            LastCommandLine = LastCommandLine.Replace("--asm 1", "");
            string sColor = "";
            if (GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sColor = " --nocolor";
            }
            LastCommandLine += sColor;
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

        // new decoupled benchmarking routines
        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
            var apiBind = " --apiport " + ApiPort;
            var CommandLine = "";

            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            // demo for benchmark
            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                CommandLine = "--algo BEAM-II " +
                    " --pool beamv2.usa.nicehash.com:3378 --user " + username + " --pass x --tls 0" +
                                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                CommandLine = "--algo BEAM-III " +
                " --pool beam-eu.sparkpool.com:2222 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.lolMiner --pass x" +
                " --pool beam-asia.sparkpool.com:12222 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.lolMiner --pass x" +
                                              // " --pass x;x;x;x --tls 1;1;0;0 " +
                                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                CommandLine = "--algo EQUI144_5 --pers BgoldPoW" +
                " --pool europe.equihash-hub.miningpoolhub.com:20595 --user angelbbs.lol --pass x" +
                                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                CommandLine = "--coin MWC-C31 " +
                " --pool mwc.2miners.com:1111 --user 2aHR0cHM6Ly9td2MuaG90Yml0LmlvLzcyOTkyMw.lolMiner --pass x " +
                "--pool grincuckatoo31.usa.nicehash.com:3372 --user " +username + " --pass x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                CommandLine = "--coin GRIN-C32" +
                " --pool grin.2miners.com:3030 --user grin16ek8qgx29ssku0q2cxez7830gh9ndw3ek5yzxe26x34s09528d2sldl6td.lolMiner --pass x" +
                " --pool grincuckatoo32.usa.nicehash.com:3383 --user " + username + " --pass x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                CommandLine = "--coin MWC-C29D " +
                " --pool mwc.2miners.com:1111 --user 2aHR0cHM6Ly9td2MuaG90Yml0LmlvLzcyOTkyMw.lolMiner --pass x" +
                " --pool grincuckaroo29.usa.nicehash.com:3372 --user " + username + " --pass x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                CommandLine = "--coin GRIN-C29M " +
                " --pool grin.2miners.com:3030 --user 2aHR0cHM6Ly9kZXBvc2l0Z3Jpbi5rdWNvaW4uY29tL2RlcG9zaXQvMTg2MTU0MTY0MA.lolMiner --pass x" +
                " --pool cuckaroom.usa.nicehash.com:3382 --user " + username + " --pass x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                CommandLine = "--algo ETHASH " +
                " --pool eu1.ethermine.org:4444 --user 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.lolMiner --pass x" +
                " --pool daggerhashimoto.eu.nicehash.com:3353 --user " + username + " --pass x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            CommandLine += GetDevicesCommandString(); //amd карты перечисляются первыми
            _benchmarkTimeWait = time;
            CommandLine = CommandLine.Replace("--asm 1", "");
            string sColor = "";
            if (GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sColor = " --nocolor";
            }
            CommandLine += sColor;
            CommandLine += apiBind;
            return CommandLine;

        }

        protected void GetEnimeration()
        {
            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);

            int edevice = 0;
            double edeviceBus = 0;

            var EnimerationHandle = new Process
            {
                StartInfo =
                {
                    FileName = MiningSetup.MinerPath
                }
            };

            {
                Helpers.ConsolePrint(MinerTag(), "Using miner for enumeration: " + EnimerationHandle.StartInfo.FileName);
                EnimerationHandle.StartInfo.WorkingDirectory = WorkingDirectory;
            }
            if (MinersSettingsManager.MinerSystemVariables.ContainsKey(Path))
            {
                foreach (var kvp in MinersSettingsManager.MinerSystemVariables[Path])
                {
                    var envName = kvp.Key;
                    var envValue = kvp.Value;
                    EnimerationHandle.StartInfo.EnvironmentVariables[envName] = envValue;
                }
            }

            var CommandLine = " --coin BEAM-II " +
                 " --pool localhost --port fake --user " + username + " --pass x --tls 0 --devices 999";//fake port for enumeration

            EnimerationHandle.StartInfo.Arguments = CommandLine;
            EnimerationHandle.StartInfo.UseShellExecute = false;
            EnimerationHandle.StartInfo.RedirectStandardError = true;
            EnimerationHandle.StartInfo.RedirectStandardOutput = true;
            EnimerationHandle.StartInfo.CreateNoWindow = true;
            Thread.Sleep(250);
            Helpers.ConsolePrint(MinerTag(), "Start enumeration: " + EnimerationHandle.StartInfo.FileName + EnimerationHandle.StartInfo.Arguments);
            EnimerationHandle.Start();
            var allDeviceCount = ComputeDeviceManager.Query.GpuCount;
            var allDevices = Available.Devices;
            try
            {
                string outdata = "";
                while (IsActiveProcess(EnimerationHandle.Id))
                {
                    outdata = EnimerationHandle.StandardOutput.ReadLine();
                    Helpers.ConsolePrint(MinerTag(), outdata);

                    if (outdata.Contains("Device"))
                    {
                        string cdevice = Regex.Match(outdata, @"\d+").Value;
                        if (int.TryParse(cdevice, out edevice))
                        {
                            Helpers.ConsolePrint(MinerTag(), edevice.ToString());
                        }

                    }

                    if (outdata.Contains("Address:"))
                    {
                        string cdeviceBus = Regex.Match(outdata, @"\d+").Value;
                        if (double.TryParse(cdeviceBus, out edeviceBus))
                        {
                            Helpers.ConsolePrint(MinerTag(), edeviceBus.ToString());
                            // for (var i = 0; i < allDevices.Count; i++)
                            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.ID).ToList();
                            foreach (var mPair in sortedMinerPairs)
                            {

                                Helpers.ConsolePrint(MinerTag(), " IDByBus=" + mPair.Device.IDByBus.ToString() + " ID=" + mPair.Device.ID.ToString() + " edevice=" + edevice.ToString() + " edeviceBus=" + edeviceBus.ToString());
                                if (mPair.Device.IDByBus == edeviceBus)
                                {
                                      //  mPair.Device.lolMinerBusID = edevice;
                                }
                            }

                            // allDevices[edevice].lolMinerBusID = edeviceBus;
                        }

                    }

                }
            }
            catch (Exception ex)
            {

            }


            try
            {
                if (!EnimerationHandle.WaitForExit(10 * 1000))
                {
                    EnimerationHandle.Kill();
                    EnimerationHandle.WaitForExit(5 * 1000);
                    EnimerationHandle.Close();
                }
            }
            catch { }

            Thread.Sleep(50);
        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            var ids = new List<string>();
            var amdDeviceCount = ComputeDeviceManager.Query.AmdDevices.Count;
            var allDeviceCount = ComputeDeviceManager.Query.GpuCount;
            Helpers.ConsolePrint("lolMinerIndexing", $"Found {allDeviceCount} Total GPU devices");
            Helpers.ConsolePrint("lolMinerIndexing", $"Found {amdDeviceCount} AMD devices");
            //   var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.ID.ToString()).ToList();
            //var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                Helpers.ConsolePrint("lolMinerIndexing", "ID: " + mPair.Device.ID);
                Helpers.ConsolePrint("lolMinerIndexing", "IDbybus: " + mPair.Device.IDByBus);
                Helpers.ConsolePrint("lolMinerIndexing", "busid: " + mPair.Device.BusID);
                Helpers.ConsolePrint("lolMinerIndexing", "lol: " + mPair.Device.lolMinerBusID);

                //список карт выводить --devices 999
                double id = mPair.Device.IDByBus + allDeviceCount - amdDeviceCount;

                if (id < 0)
                {
                    Helpers.ConsolePrint("lolMinerIndexing", "ID too low: " + id + " skipping device");
                    continue;
                }

                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    Helpers.ConsolePrint("lolMinerIndexing", "NVIDIA found. Increasing index");
                    id ++;
                }

                Helpers.ConsolePrint("lolMinerIndexing", "ID: " + id );
                {
                    ids.Add(id.ToString());
                }

            }


            deviceStringCommand += string.Join(",", ids);

            return deviceStringCommand;
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

            int delay_before_calc_hashrate = 10;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 60;
                }
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
                    
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        delay_before_calc_hashrate = 60;
                        MinerStartDelay = 20;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ZHash))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 30;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo31))
                    {
                        delay_before_calc_hashrate = 20;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 30;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.BeamV3))
                    {
                        delay_before_calc_hashrate = 20;
                        MinerStartDelay = 20;
                    }
                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        ComputeDevice.BenchmarkProgress = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString());
                            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
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
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = summspeed;
                }
                else
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
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
        
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }


        #endregion // Decoupled benchmarking routines

        public class lolResponse
        {
            public List<lolGpuResult> result { get; set; }
        }

        public class lolGpuResult
        {
            public double sol_ps { get; set; } = 0;
        }
        // TODO _currentMinerReadStatus
        public override async Task<ApiData> GetSummaryAsync()
        {
            //Helpers.ConsolePrint("try API...........", "");
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            string ResponseFromlolMiner;
            double total = 0;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/summary");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromlolMiner = await Reader.ReadToEndAsync();
                //Helpers.ConsolePrint("API...........", ResponseFromlolMiner);
                //if (ResponseFromlolMiner.Length == 0 || (ResponseFromlolMiner[0] != '{' && ResponseFromlolMiner[0] != '['))
                //    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            if (ResponseFromlolMiner == null)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                return null;
            }
            dynamic resp = JsonConvert.DeserializeObject(ResponseFromlolMiner);
            if (resp != null)
            {
                double totals = resp.Session.Performance_Summary;
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
                {
                    ad.Speed = totals * 1000000;
                }
                else
                {
                    ad.Speed = totals;
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

            Thread.Sleep(100);

            //CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            // check if speed zero
            if (ad.Speed == 0) CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;

            return ad;
        }
    }
}
