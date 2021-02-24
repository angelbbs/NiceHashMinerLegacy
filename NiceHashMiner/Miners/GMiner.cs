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
using System.Net;
using System.Management;
using NiceHashMiner.Devices;

namespace NiceHashMiner.Miners
{
    public class GMiner : Miner
    {
        private int _benchmarkTimeWait = 120;
        private const string LookForStart = " c ";
        private const string LookForStartDual = "h/s + ";
        private const string LookForEnd = "sol/s";
        private const string LookForEndDual = "h/s  ";
        private const double DevFee = 2.0;
        string  gminer_var = "";
        protected AlgorithmType SecondaryAlgorithmType = AlgorithmType.NONE;
        private string[,] myServers = Form_Main.myServers;

        public GMiner(AlgorithmType secondaryAlgorithmType) : base("GMiner")
        {
            ConectionType = NhmConectionType.NONE;
            //IsNeverHideMiningWindow = true;
            SecondaryAlgorithmType = secondaryAlgorithmType;
            IsMultiType = true;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            const string vcp = "msvcp120.dll";
            var vcpPath = WorkingDirectory + vcp;
            if (!File.Exists(vcpPath))
            {
                try
                {
                    File.Copy(vcp, vcpPath, true);
                    Helpers.ConsolePrint(MinerTag(), $"Copy from {vcp} to {vcpPath} done");
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint(MinerTag(), "Copy msvcp.dll failed: " + e.Message);
                }
            }
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Helpers.ConsolePrint("GMINER Stop", "");
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            KillGminer();
        }
        private string GetStartCommand(string url, string btcAddress, string worker)
        {
            var algo ="";
            var algoName = "";
            var pers = "";
            var nicehashstratum = "";
            var ssl = "";
            string username = GetUsername(btcAddress, worker);
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                algo = "144_5";
                algoName = "zhash";
                pers = " --pers auto ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                algo = "BeamHashII";
                algoName = "beamv2";
                ssl = " --ssl_verification 0";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                algo = "BeamHashIII";
                algoName = "beamv3";
                ssl = " --ssl_verification 0";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29)
            {
                algo = "cuckaroo29";
                algoName = "grincuckaroo29";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                algo = "cuckarood29";
                algoName = "grincuckarood29";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                //algo = "grin29";
                algo = "cuckaroom29";
                algoName = "cuckaroom";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckaRooz29)
            {
                algo = "cuckarooz29";
                algoName = "cuckarooz29";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                algo = "grin31";
                algoName = "grincuckatoo31";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                algo = "grin32";
                algoName = "grincuckatoo32";
                ssl = " --ssl_verification 0";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                algo = "aeternity";
                algoName = "cuckoocycle";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                algo = "ethash";
                algoName = "daggerhashimoto";
                nicehashstratum = " --proto stratum";
            }
            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
            {
                algo = "kawpow";
                algoName = "kawpow";
                ssl = " --ssl 0";
            }
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroo29BFC)
            {
                algo = "bfc";
                algoName = "cuckaroo29bfc";
                ssl = " --ssl 0";
            }
            */

            var ret = GetDevicesCommandString()
                      + " --algo " + algo + pers + " --server " + url.Split(':')[0]
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --server " + algoName + "." + myServers[1, 0] + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --server " + algoName + "." + myServers[2, 0] + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --server " + algoName + "." + myServers[3, 0] + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --api " + ApiPort;
            return ret;
        }
        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " --devices ";
            var ids = new List<string>();
            //var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            var extra = "";
            int id;
            foreach (var mPair in sortedMinerPairs)
            {
                id = mPair.Device.IDByBus;

                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    gminer_var = variables.gminer_var1;
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
                } else
                {
                    gminer_var = variables.gminer_var2;
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
                }

                {
                    ids.Add(id.ToString());
                }

            }

            deviceStringCommand += string.Join(" ", ids);
            deviceStringCommand = deviceStringCommand + extra + " ";

            return gminer_var + deviceStringCommand;
        }

        protected void KillMinerBase(string exeName)
        {
            foreach (var process in Process.GetProcessesByName(exeName))
            {
                try { process.Kill(); }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
        }
        private static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
        public override void EndBenchmarkProcces()
        {
            if (BenchmarkProcessStatus != BenchmarkProcessStatus.Killing && BenchmarkProcessStatus != BenchmarkProcessStatus.DoneKilling)
            {
                BenchmarkProcessStatus = BenchmarkProcessStatus.Killing;
                try
                {
                    Helpers.ConsolePrint("BENCHMARK",
                        $"Trying to kill benchmark process {BenchmarkProcessPath} algorithm {BenchmarkAlgorithm.AlgorithmName}");

                    int k = ProcessTag().IndexOf("pid(");
                    int i = ProcessTag().IndexOf(")|bin");
                    var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();

                    int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                    Helpers.ConsolePrint("BENCHMARK", "gminer.exe PID: "+ pid.ToString());
                    KillProcessAndChildren(pid);
                    BenchmarkHandle.Kill();
                    BenchmarkHandle.Close();
                    if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();
                }
                catch { }
                finally
                {
                    BenchmarkProcessStatus = BenchmarkProcessStatus.DoneKilling;
                    Helpers.ConsolePrint("BENCHMARK",
                        $"Benchmark process {BenchmarkProcessPath} algorithm {BenchmarkAlgorithm.AlgorithmName} KILLED");
                    //BenchmarkHandle = null;
                }
            }
        }
        public void KillGminer()
        {
            try
            {
                int k = ProcessTag().IndexOf("pid(");
                int i = ProcessTag().IndexOf(")|bin");
                var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();

                int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                Helpers.ConsolePrint("GMINER", "gminer.exe PID: " + pid.ToString());
                KillProcessAndChildren(pid);
                ProcessHandle.Kill();
                ProcessHandle.Close();
            }
            catch { }
            if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();
            
        }
        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            _benchmarkTimeWait = time;
            var ret = "";

            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                ret = " --color 0 --pec --pers auto --algo 144_5" +
                " --server btg.2miners.com --user GeKYDPRcemA3z9okSUhe9DdLQ7CRhsDBgX.gminer --pass x --port 4040 " +
                " --server equihash144.eu.mine.zpool.ca --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 --pass c=BTC --port 2144 " +
                " --server zhash.eu" + ".nicehash.com --user " + username + " --pass x --port 3369" +
                " --server zhash.hk" + ".nicehash.com --user " + username + " --pass x --port 3369" +
                GetDevicesCommandString();
            }

            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                ret = " --color 0 --pec --algo BeamHashII" +
                " --server beam.2miners.com:5252 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x --ssl 1 " +
                " --server beam-asia.sparkpool.com:12222 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x --ssl 1 " +
                " --server beamv2.eu.nicehash.com:3378 --user " + username + " --pass x --ssl 0" +
                " --server beamv2.hk.nicehash.com:3378 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                ret = " --color 0 --pec --algo BeamHashIII" +
                " --server beam.2miners.com:5252 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x --ssl 1 " +
                " --server beam-asia.sparkpool.com:12222 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x --ssl 1 " +
                " --server beamv2.eu.nicehash.com:3378 --user " + username + " --pass x --ssl 0" +
                " --server beamv2.hk.nicehash.com:3378 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29)
            {
                ret = " --color 0 --pec --algo cuckaroo29" +
                " --server grincuckaroo29.eu" + ".nicehash.com --user " + username + " --pass x --port 3371 --ssl 0" +
                " --server grincuckaroo29.hk" + ".nicehash.com --user " + username + " --pass x --port 3371 --ssl 0" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                ret = " --color 0 --pec --algo cuckarood29" +
                " --server  mwc.2miners.com:1111 --user 2aHR0cHM6Ly9td2MuaG90Yml0LmlvLzcyOTkyMw.gminer --ssl 0" +
                " --server grincuckarood29.eu:3377" + ".nicehash.com --user " + username + " --ssl 0" +
                " --server grincuckarood29.hk:3377" + ".nicehash.com --user " + username + " --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                ret = " --color 0 --pec --algo cuckaroom29_qitmeer" +
                " --server pmeercuckaroom.uupool.cn:9660 --user Tmk9X8FPuu5SxP6mW32zQ5N68SNsn76xZrY.gminer --pass x  --ssl 0" +
                " --server cuckaroom.eu.nicehash.com:3382 --user " + username + " --pass x --ssl 0" +
                " --server cuckaroom.hk.nicehash.com:3382 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                ret = " --color 0 --pec --algo grin31" +
                " --server mwc.2miners.com:1111 --user 2aHR0cHM6Ly9td2MuaG90Yml0LmlvLzcyOTkyMw.gminer --pass x  --ssl 0" +
                " --server grincuckatoo31.eu.nicehash.com:3372 --user " + username + " --pass x --ssl 0" +
                " --server grincuckatoo31.hk.nicehash.com:3372 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                ret = " --color 0 --pec --algo grin32" +
                " --server grin.2miners.com:3030 --user grin16ek8qgx29ssku0q2cxez7830gh9ndw3ek5yzxe26x34s09528d2sldl6td.gminer --pass x  --ssl 0" +
                " --server grincuckatoo32.eu.nicehash.com:3383 --user " + username + " --pass x --ssl 0" +
                " --server grincuckatoo32.hk.nicehash.com:3383 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                ret = " --color 0 --pec --algo aeternity" +
                " --server ae.2miners.com:4040 --user ak_2f9AMwztStKs5roPmT592wTbUEeTyqRgYVZNrc5TyZfr94m7fM.gminer --pass x --ssl 0" +
                " --server cuckoocycle.eu.nicehash.com:3376 --user " + username + " --pass x --ssl 0" +
                " --server cuckoocycle.hk.nicehash.com:3376 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                ret = " --color 0 --pec --algo ethash" +
                " --server eu1.ethermine.org:4444 --user 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.GMiner --pass x --ssl 0 --proto proxy" +
                " --server daggerhashimoto.eu.nicehash.com:3353 --user " + username + " --pass x --ssl 0 --proto stratum" +
                " --server daggerhashimoto.hk.nicehash.com:3353 --user " + username + " --pass x --ssl 0 --proto stratum" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
            {
                ret = " --color 0 --pec --algo kawpow" +
                " --server rvn.2miners.com:6060 --user RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.GMiner --pass x " +
                " --server kawpow.eu.nicehash.com:3385 --user " + username + " --pass x --proto stratum" +
                GetDevicesCommandString();
            }
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroo29BFC)
            {
                ret = " --color 0 --pec --algo bfc" +
                " --server bfc.f2pool.com:4900 --user angelbbs.GMiner --pass x " +
                " --server cuckaroo29bfc.eu.nicehash.com:3386 --user " + username + " --pass x --proto stratum" +
                GetDevicesCommandString();
            }
            */
            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckaRooz29)
            {
                ret = " --color 0 --pec --algo cuckarooz29" +
                " --server grin.2miners.com:3030 --user 2aHR0cHM6Ly9kZXBvc2l0Z3Jpbi5rdWNvaW4uY29tL2RlcG9zaXQvMTg2MTU0MTY0MA.gminer --pass x  --ssl 0" +
                " --server grin.sparkpool.com:6666 --user angelbbs@mail.ru/" + worker + " --pass x --ssl 0" +
                " --server cuckarooz29.eu.nicehash.com:3388 --user " + username + " --pass x --ssl 0" +
                " --server cuckarooz29.hk.nicehash.com:3388 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            return ret + " --api " + ApiPort; ;
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
            int repeats = 0;
            double summspeed = 0.0d;

            int delay_before_calc_hashrate = 5;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);
            /*
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW) ||
                MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                _benchmarkTimeWait = _benchmarkTimeWait + 30;
            }
            */
            try
            {
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in : " + _benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string) commandLine);
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
                         break;
                    }
                    // wait a second due api request
                    Thread.Sleep(1000);

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 5;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ZHash))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 10;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo31))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 5;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 5;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 5;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.BeamV3))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 5;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 5;
                    }

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        ComputeDevice.BenchmarkProgress = repeats;
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        ComputeDevice.BenchmarkProgress = (int)(benchProgress * 100);
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

                            KillProcessAndChildren(BenchmarkHandle.Id);
                            BenchmarkHandle.Dispose();
                            EndBenchmarkProcces();

                            /*
                            try
                            {
                                int k = ProcessTag().IndexOf("pid(");
                                int i = ProcessTag().IndexOf(")|bin");
                                var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();

                                int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                                Helpers.ConsolePrint("GMINER", "gminer.exe PID: " + pid.ToString());
                                KillProcessAndChildren(pid);
                                ProcessHandle.Kill();
                                ProcessHandle.Close();
                            }
                            catch { }
                            if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();
                            */
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

        protected double GetNumber(string outdata)
        {
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckaRooz29 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                return GetNumber(outdata, LookForStart, "g/s");
            } else if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW
                //MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroo29BFC ||
                )
            {
                return GetNumber(outdata, LookForStart, "h/s");
            } else
            {
                return GetNumber(outdata, LookForStart, LookForEnd);
            }
        }

        protected double GetNumberSecond(string outdata)
        {
                return GetNumber(outdata, LookForStartDual, LookForEndDual);
        }
        protected double GetNumber(string outdata, string lookForStart, string lookForEnd)
        {
            Helpers.ConsolePrint(MinerTag(), outdata);
            try
            {
                double mult = 1;
                var speedStart = outdata.IndexOf(lookForStart);
                var speed = outdata.Substring(speedStart, outdata.Length - speedStart);
                speed = speed.Replace(lookForStart, "");
                speed = speed.Substring(0, speed.IndexOf(lookForEnd));

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

               // Helpers.ConsolePrint("speed", speed);
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
        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5;  // 5 min
        }
        protected override bool IsApiEof(byte third, byte second, byte last)
        {
            return third == 0x7d && second == 0xa && last == 0x7d;
        }

        private class JsonApiResponse
        {
            public class Devices
            {
                public int gpu_id { get; set; }
                public double speed { get; set; }
                public double speed2 { get; set; }
            }
            public Devices[] devices { get; set; }
        }

        public override async Task<ApiData> GetSummaryAsync()
        {
            //Helpers.ConsolePrint("try API...........", "");
            ApiData ad;


            ad = new ApiData(MiningSetup.CurrentAlgorithmType);

            string ResponseFromGMiner;
            double total = 0;
            double totalSec = 0;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString()+"/stat");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromGMiner = await Reader.ReadToEndAsync();
                //Helpers.ConsolePrint("API...........", ResponseFromGMiner);
                if (ResponseFromGMiner.Length == 0 || (ResponseFromGMiner[0] != '{' && ResponseFromGMiner[0] != '['))
                    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            //Helpers.ConsolePrint("API ResponseFromGMiner:", ResponseFromGMiner);
            ResponseFromGMiner = ResponseFromGMiner.Replace("-nan", "0.00");
            try
            {
                dynamic resp = JsonConvert.DeserializeObject<JsonApiResponse>(ResponseFromGMiner);
                if (resp != null)
                {
                    for (var i = 0; i < resp.devices.Length; i++)
                    {
                        total = total + resp.devices[i].speed;

                    }
                }
                else
                {
                    Helpers.ConsolePrint("GMiner:", "resp - null");
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("GMiner API:", "Error JSON parsing");
            } finally
            {
                ad.Speed = total;

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
            return ad;
        }
    }
}
