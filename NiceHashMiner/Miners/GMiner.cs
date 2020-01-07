/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
        private int _benchmarkReadCount;
        private double _benchmarkSum;
        //|  GPU0 63 C  53.8 Sol/s    4/0 127 W 0.42 Sol/W |
      //  private const string LookForStart = "total speed: ";
        private const string LookForStart = " c ";
        private const string LookForEnd = "sol/s";
        private const double DevFee = 2.0;
        string  gminer_var = "";


        public GMiner() : base("GMiner")
        {
            ConectionType = NhmConectionType.NONE;
            //IsNeverHideMiningWindow = true;
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
            //Thread.Sleep(200);
            KillGminer();
            //KillMinerBase("miner");

        }
        private string GetStartCommand(string url, string btcAddress, string worker)
        {
            var algo ="";
            var algoName = "";
            var pers = "";
            var nicehashstratum = "";
            string username = GetUsername(btcAddress, worker);
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
                    {
                        algo = "144_5";
                        algoName = "zhash";
                pers = " --pers auto ";
                    }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Beam)
            {
                        algo = "BeamHashI";
                        algoName = "beam";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                algo = "BeamHashII";
                algoName = "beamv2";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29)
            {
                algo = "cuckaroo29";
                algoName = "grincuckaroo29";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                algo = "grin29";
                algoName = "grincuckarood29";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                algo = "grin31";
                algoName = "grincuckatoo31";
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
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Eaglesong)
            {
                algo = "eaglesong";
                algoName = "eaglesong";
            }

            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }

            var ret = GetDevicesCommandString()
                      + " --algo " + algo + pers + " --server " + url.Split(':')[0]
                      + " --user " + username + " --pass x --port " + url.Split(':')[1]
                      + " --server " + algoName + "." + myServers[1, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1]
                      + " --server " + algoName + "." + myServers[2, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1]
                      + " --server " + algoName + "." + myServers[3, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1]
                      + " --server " + algoName + "." + myServers[4, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1]
                      + " --server " + algoName + "." + myServers[5, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1]
                      + " --api " + ApiPort;
            return ret;
        }
        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " --devices ";
            var ids = new List<string>();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            var extra = "";
            int id;
            foreach (var mPair in sortedMinerPairs)
            {
                if (ConfigManager.GeneralConfig.GMinerIDByBusEnumeration)
                {
                    id = mPair.Device.IDByBus + variables.mPairDeviceIDByBus_GMiner;
                    Helpers.ConsolePrint("GMinerIndexing", "IDByBus: " + id);
                } else
                {
                    id = mPair.Device.ID + variables.mPairDeviceIDByBus_GMiner;
                    Helpers.ConsolePrint("GMinerIndexing", "ID: " + id);
                }
                /*
                if (id < 0)
                {
                    Helpers.ConsolePrint("lolMinerBEAMIndexing", "ID too low: " + id + " skipping device");
                    continue;
                }

                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    Helpers.ConsolePrint("lolMinerBEAMIndexing", "NVIDIA found. Increasing index");
                    id++;
                }
                */

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
           // if (ProcessHandle != null)
            {
                try
                {
                    //ProcessHandle.Kill();
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
                /*
                try { ProcessHandle.SendCtrlC((uint)Process.GetCurrentProcess().Id); } catch { }
                ProcessHandle.Close();
                ProcessHandle = null;
                */
                if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();
            }
            //KillMinerBase("miner");
            //foreach (Process process in Process.GetProcessesByName("miner")) { //kill ewbf to
            //     try { process.Kill(); } catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            //}
        }
        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            CleanOldLogs();
            //_benchmarkTimeWait = Math.Max(time * 3, 180); //
            _benchmarkTimeWait = time;
            var ret = "";
            var suff = "0_";
            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
               Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], ConectionType);
            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    suff = "1_";
                }
            }
            if (File.Exists("bin_3rdparty\\gminer\\"+ suff + GetLogFileName()))
                File.Delete("bin_3rdparty\\gminer\\"+ suff + GetLogFileName());

            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --pers auto --algo 144_5" +
                " --server equihash144.eu.mine.zpool.ca --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 --pass c=BTC --port 2144 " +
                " --server zhash.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3369" +
                " --server zhash.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3369" +
                GetDevicesCommandString();
            }
            Helpers.ConsolePrint("BENCHMARK-suff:", suff);
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Beam)
            {
                //_benchmarkTimeWait = 180; 
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo BeamHashI" +
              //  " --server beam-eu.sparkpool.com --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9." + worker + " --pass x --port 2222 --ssl 1 " +
              //  " --server beam-asia.sparkpool.com --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9." + worker + " --pass x --port 12222 --ssl 1 " +
                " --server beam.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3370 --ssl 0" +
                " --server beam.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3370 --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                //_benchmarkTimeWait = 180; 
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo BeamHashII" +
                " --server beam-eu.sparkpool.com --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9." + worker + " --pass x --port 2222 --ssl 1 " +
                " --server beam-asia.sparkpool.com --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9." + worker + " --pass x --port 12222 --ssl 1 " +
                " --server beamv2.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3378 --ssl 0" +
                " --server beamv2.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3378 --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo grin29" +
                " --server grin.sparkpool.com --user angelbbs@mail.ru/" + worker + " --pass x --port 6666 --ssl 0" +
                " --server grincuckaroo29.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3371 --ssl 0" +
                " --server grincuckaroo29.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3371 --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo grin29" +
                " --server grin.sparkpool.com --user angelbbs@mail.ru/" + worker + " --pass x --port 6666 --ssl 0" +
                " --server grincuckarood29.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3377 --ssl 0" +
                " --server grincuckarood29.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3377 --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo grin31" +
                " --server grin.sparkpool.com --user angelbbs@mail.ru/" + worker + " --pass x --port 6667 --ssl 0" +
                " --server grincuckatoo31.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3372 --ssl 0" +
                " --server grincuckatoo31.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3372 --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo aeternity" +
                " --server ae.f2pool.com --user ak_2f9AMwztStKs5roPmT592wTbUEeTyqRgYVZNrc5TyZfr94m7fM." + worker + " --pass x --port 7898 --ssl 0" +
                " --server ae.2miners.com --user ak_2f9AMwztStKs5roPmT592wTbUEeTyqRgYVZNrc5TyZfr94m7fM." + worker + " --pass x --port 4040 --ssl 0" +
                " --server cuckoocycle.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3376 --ssl 0" +
                " --server cuckoocycle.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3376 --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo ethash" +
                " --server eth-eu.dwarfpool.com --user 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1." + worker + " --pass x --port 8008 --ssl 0 --proto proxy" +
                " --server daggerhashimoto.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3353 --ssl 0 --proto stratum" +
                " --server daggerhashimoto.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3353 --ssl 0 --proto stratum" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Eaglesong)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo eaglesong" +
                " --server ckb.2miners.com:6464 --user ckb1qyqxhhuuldj8kkxfvef5cj2f02065f25uq3qc3n7sv" +
                " --server eaglesong.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3381 --ssl 0" +
                " --server eaglesong.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3381 --ssl 0" +
                GetDevicesCommandString();
            }
            return ret;
        }

        protected override void BenchmarkThreadRoutine(object commandLine)
        {
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in : " + _benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string) commandLine);
                BenchmarkHandle.WaitForExit(_benchmarkTimeWait + 2);
                var benchmarkTimer = new Stopwatch();
                benchmarkTimer.Reset();
                benchmarkTimer.Start();

                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                var keepRunning = true;
                while (keepRunning && IsActiveProcess(BenchmarkHandle.Id))
                {
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 2)
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

                        keepRunning = false;
                        break;
                    }

                    // wait a second reduce CPU load
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {
                Thread.Sleep(1000);
                BenchmarkAlgorithm.BenchmarkSpeed = 0;
                // find latest log file
                var latestLogFile = "";
                var suff = "0_";
                foreach (var pair in MiningSetup.MiningPairs)
                {
                    if (pair.Device.DeviceType == DeviceType.NVIDIA) suff = "1_";
                }
                var dirInfo = new DirectoryInfo(WorkingDirectory);
                foreach (var file in dirInfo.GetFiles(suff + GetLogFileName()))
                {
                    latestLogFile = file.Name;
                    break;
                }

                // read file log
                if (File.Exists(WorkingDirectory + latestLogFile))
                {
                    var lines = new string[0];
                    var read = false;
                    var iteration = 0;
                    while (!read)
                    {
                        if (iteration < 1)
                        {
                            try
                            {
                                lines = File.ReadAllLines(WorkingDirectory + latestLogFile);
                                read = true;
                                Helpers.ConsolePrint(MinerTag(),
                                    "Successfully read log after " + iteration + " iterations");
                               }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint(MinerTag(), ex.Message);
                                Thread.Sleep(200);
                            }

                            iteration++;
                        }
                        else
                        {
                            read = true; // Give up after 10s
                            Helpers.ConsolePrint(MinerTag(), "Gave up on iteration " + iteration);
                        }
                    }

                    var addBenchLines = BenchLines.Count == 0;
                    foreach (var line in lines)
                    {
                        if (line != null)
                        {
                            BenchLines.Add(line);
                            var lineLowered = line.ToLower();
                           // Helpers.ConsolePrint(MinerTag(), lineLowered);
                            if (lineLowered.Contains(LookForStart))
                            {
                                _benchmarkSum += GetNumber(lineLowered);
                                ++_benchmarkReadCount;
                            }
                        }
                    }

                    if (_benchmarkReadCount > 0)
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = _benchmarkSum / _benchmarkReadCount;
                    }
                }

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
            Helpers.ConsolePrint("BENCHMARK", outdata);
            return false;
        }

        protected double GetNumber(string outdata)
        {
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29 || MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29 || MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31 || MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                return GetNumber(outdata, LookForStart, "g/s");
            } else if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto || MiningSetup.CurrentAlgorithmType == AlgorithmType.Eaglesong)
            {
                return GetNumber(outdata, LookForStart, "h/s");
            } else
            {
                return GetNumber(outdata, LookForStart, LookForEnd);
            }
        }
        //|  GPU0 58 C 378.05 MH/s    0/0  87 W  4.35 MH/W |

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

        //{ "uptime":3,"server":"stratum://zhash.eu.nicehash.com:3369","user": "wallet.Farm2",
        //"algorithm":"Equihash 144,5 \"BgoldPoW\"","electricity":0.000,
        //"devices":[{"gpu_id":4,"cuda_id":4,"bus_id":"0000:07:00.0","name":"ASUS GeForce GTX 1060 3GB"
        //,"speed":38,"accepted_shares":0,"rejected_shares":0,"temperature":14,"temperature_limit":90,"power_usage":93}]}
        private class JsonApiResponse
        {
            public class Devices
            {
                public int gpu_id { get; set; }
                public double speed { get; set; }
            }
            public Devices[] devices { get; set; }
        }
        /*
        private class JsonApiResponse
        {
            public uint uptime { get; set; }
            public string server { get; set; }
            public string user { get; set; }
            public string algorithm { get; set; }
            public uint electricity { get; set; }
            public Devices[] devices { get; set; }
        }
        */
        public override async Task<ApiData> GetSummaryAsync()
        {
            //Helpers.ConsolePrint("try API...........", "");
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            string ResponseFromGMiner;
            double total = 0;
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

            dynamic resp = JsonConvert.DeserializeObject<JsonApiResponse>(ResponseFromGMiner);

            //Helpers.ConsolePrint("API resp...........", resp);
            if (resp != null)
            {
                for (var i = 0; i < resp.devices.Length; i++)
                {
                    total = total + resp.devices[i].speed;
                }

                ad.Speed = total;
                    if (ad.Speed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }
                else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                }
            } else
            {
                Helpers.ConsolePrint("GMiner:", "resp - null");
            }

            Thread.Sleep(200);
            return ad;
        }
    }
}
