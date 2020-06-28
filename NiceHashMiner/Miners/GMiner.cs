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
        private double _benchmarkSumSecond;
        //|  GPU0 63 C  53.8 Sol/s    4/0 127 W 0.42 Sol/W |
      //  private const string LookForStart = "total speed: ";
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
            //Helpers.ConsolePrint(MinerTag(), "SecondaryAlgorithmType: " + SecondaryAlgorithmType.ToString());
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
            var ssl = "";
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
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Eaglesong)
            {
                algo = "eaglesong";
                algoName = "eaglesong";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Handshake)
            {
                algo = "Handshake";
                algoName = "handshake";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
            {
                algo = "kawpow";
                algoName = "kawpow";
                ssl = " --ssl 0";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroo29BFC)
            {
                algo = "bfc";
                algoName = "cuckaroo29bfc";
                ssl = " --ssl 0";
            }
            
            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }

            if (SecondaryAlgorithmType == AlgorithmType.Eaglesong)
            {
                var durl = url.Replace("daggerhashimoto", "eaglesong");
                return GetDevicesCommandString()
                      + " --algo eth+ckb --server " + url + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver " + durl.Split(':')[0] + ":3381 --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[1, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver eaglesong." + myServers[1, 0] + ".nicehash.com:3381" + " --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[2, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver eaglesong." + myServers[2, 0] + ".nicehash.com:3381" + " --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[3, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver eaglesong." + myServers[3, 0] + ".nicehash.com:3381" + " --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[4, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver eaglesong." + myServers[4, 0] + ".nicehash.com:3381" + " --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[5, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver eaglesong." + myServers[5, 0] + ".nicehash.com:3381" + " --duser " + username + " --ssl 0"
                      + " --api " + ApiPort;
            }
            if (SecondaryAlgorithmType == AlgorithmType.Handshake)
            {
                var durl = url.Replace("daggerhashimoto", "handshake");
                return GetDevicesCommandString()
                      + " --algo eth+hns --server " + url + " --user " + username + " --ssl 0 --proto stratum"
                      //+ " --dserver hns.pool.blackminer.com:9052 --duser hs1qjq6nglhcmx2xnd30kt3s2rq3fphft459va796j --ssl 0"
                      + " --dserver " + durl.Split(':')[0] + ":3384 --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[1, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver handshake." + myServers[1, 0] + ".nicehash.com:3384" + " --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[2, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver handshake." + myServers[2, 0] + ".nicehash.com:3384" + " --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[3, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver handshake." + myServers[3, 0] + ".nicehash.com:3384" + " --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[4, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver handshake." + myServers[4, 0] + ".nicehash.com:3384" + " --duser " + username + " --ssl 0"
                      + " --server daggerhashimoto." + myServers[5, 0] + ".nicehash.com:" + url.Split(':')[1] + " --user " + username + " --ssl 0 --proto stratum"
                      + " --dserver handshake." + myServers[5, 0] + ".nicehash.com:3384" + " --duser " + username + " --ssl 0"
                      + " --api " + ApiPort;
            }

            var ret = GetDevicesCommandString()
                      + " --algo " + algo + pers + " --server " + url.Split(':')[0]
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --server " + algoName + "." + myServers[1, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --server " + algoName + "." + myServers[2, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --server " + algoName + "." + myServers[3, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --server " + algoName + "." + myServers[4, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
                      + " --server " + algoName + "." + myServers[5, 0] + nhsuff + ".nicehash.com" + nicehashstratum
                      + " --user " + username + " --pass x --port " + url.Split(':')[1] + ssl
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
            if (File.Exists("miners\\gminer\\"+ suff + GetLogFileName()))
                File.Delete("miners\\gminer\\"+ suff + GetLogFileName());

            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --pers auto --algo 144_5" +
                " --server btg.2miners.com --user GeKYDPRcemA3z9okSUhe9DdLQ7CRhsDBgX.gminer --pass x --port 4040 " +
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
                " --server beam.2miners.com:5252 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x --ssl 1 " +
                " --server beam-asia.sparkpool.com:12222 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x --ssl 1 " +
                " --server beamv2.eu.nicehash.com:3378 --user " + username + " --pass x --ssl 0" +
                " --server beamv2.hk.nicehash.com:3378 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                //_benchmarkTimeWait = 180;
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo BeamHashIII" +
                " --server beam.2miners.com:5252 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x --ssl 1 " +
                " --server beam-asia.sparkpool.com:12222 --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x --ssl 1 " +
                " --server beamv2.eu.nicehash.com:3378 --user " + username + " --pass x --ssl 0" +
                " --server beamv2.hk.nicehash.com:3378 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo cuckaroo29" +
                //" --server grin.sparkpool.com --user angelbbs@mail.ru/" + worker + " --pass x --port 6666 --ssl 0" +
                " --server grincuckaroo29.eu" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3371 --ssl 0" +
                " --server grincuckaroo29.hk" + nhsuff + ".nicehash.com --user " + username + " --pass x --port 3371 --ssl 0" +
                GetDevicesCommandString();
            }
            //start miner.exe--algo cuckarood29 --server eu.frostypool.com:3516 --user angelbbs
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo cuckarood29" +
                " --server eu.frostypool.com:3516 --user angelbbs --ssl 0" +
                " --server grincuckarood29.eu:3377" + nhsuff + ".nicehash.com --user " + username + " --ssl 0" +
                " --server grincuckarood29.hk:3377" + nhsuff + ".nicehash.com --user " + username + " --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo cuckaroom29" +
                " --server grin.2miners.com:3030 --user 2aHR0cHM6Ly9kZXBvc2l0Z3Jpbi5rdWNvaW4uY29tL2RlcG9zaXQvMTg2MTU0MTY0MA.gminer --pass x  --ssl 0" +
                " --server grin.sparkpool.com:6666 --user angelbbs@mail.ru/" + worker + " --pass x --ssl 0" +
                " --server cuckaroom.eu.nicehash.com:3382 --user " + username + " --pass x --ssl 0" +
                " --server cuckaroom.hk.nicehash.com:3382 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo grin31" +
                " --server grin.2miners.com:3030 --user 2aHR0cHM6Ly9kZXBvc2l0Z3Jpbi5rdWNvaW4uY29tL2RlcG9zaXQvMTg2MTU0MTY0MA.gminer --pass x  --ssl 0" +
                " --server grin.sparkpool.com:6667 --user angelbbs@mail.ru/" + worker + " --pass x --ssl 0" +
                " --server grincuckatoo31.eu.nicehash.com:3372 --user " + username + " --pass x --ssl 0" +
                " --server grincuckatoo31.hk.nicehash.com:3372 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo grin32" +
                " --server grin.2miners.com:3030 --user 2aHR0cHM6Ly9kZXBvc2l0Z3Jpbi5rdWNvaW4uY29tL2RlcG9zaXQvMTg2MTU0MTY0MA.gminer --pass x  --ssl 0" +
                " --server grincuckatoo32.eu.nicehash.com:3383 --user " + username + " --pass x --ssl 0" +
                " --server grincuckatoo32.hk.nicehash.com:3383 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo aeternity" +
                //" --server ae.f2pool.com --user ak_2f9AMwztStKs5roPmT592wTbUEeTyqRgYVZNrc5TyZfr94m7fM." + worker + " --pass x --port 7898 --ssl 0" +
                " --server ae.2miners.com:4040 --user ak_2f9AMwztStKs5roPmT592wTbUEeTyqRgYVZNrc5TyZfr94m7fM.gminer --pass x --ssl 0" +
                " --server cuckoocycle.eu.nicehash.com:3376 --user " + username + " --pass x --ssl 0" +
                " --server cuckoocycle.hk.nicehash.com:3376 --user " + username + " --pass x --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo ethash" +
                " --server eu1.ethermine.org:4444 --user 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.GMiner --pass x --ssl 0 --proto proxy" +
                " --server daggerhashimoto.eu.nicehash.com:3353 --user " + username + " --pass x --ssl 0 --proto stratum" +
                " --server daggerhashimoto.hk.nicehash.com:3353 --user " + username + " --pass x --ssl 0 --proto stratum" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo kawpow" +
                " --server rvn.2miners.com:6060 --user RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.GMiner --pass x " +
                " --server kawpow.eu.nicehash.com:3385 --user " + username + " --pass x --proto stratum" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroo29BFC)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo bfc" +
                " --server bfc.f2pool.com:4900 --user angelbbs.GMiner --pass x " +
                " --server cuckaroo29bfc.eu.nicehash.com:3386 --user " + username + " --pass x --proto stratum" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Eaglesong)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo eaglesong" +
                " --server ckb.2miners.com:6464 --user ckb1qyqxhhuuldj8kkxfvef5cj2f02065f25uq3qc3n7sv.gminer --ssl 0" +
                " --server eaglesong.eu" + nhsuff + ".nicehash.com:3381 --user " + username + " --ssl 0" +
                " --server eaglesong.hk" + nhsuff + ".nicehash.com:3381 --user " + username + " --ssl 0" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Handshake)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo handshake" +
                " --server hns.f2pool.com:6000 --user hs1qjq6nglhcmx2xnd30kt3s2rq3fphft459va796j.gminer --ssl 0" +
                " --server handshake.eu" + nhsuff + ".nicehash.com:3384 --user " + username + " --ssl 0" +
                " --server handshake.hk" + nhsuff + ".nicehash.com:3384 --user " + username + " --ssl 0" +
                GetDevicesCommandString();
            }

            if (SecondaryAlgorithmType == AlgorithmType.Eaglesong)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo eth+ckb" +
                " --server eu1.ethermine.org:4444 --user 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.GMiner --ssl 0 --proto proxy --dserver ckb.2miners.com:6464 --duser ckb1qyqxhhuuldj8kkxfvef5cj2f02065f25uq3qc3n7sv.gminer" +
                " --server daggerhashimoto.eu.nicehash.com:3353 --user " + username + " --ssl 0 --proto stratum --dserver eaglesong.eu.nicehash.com:3381 --duser " + username + " --ssl 0" +
                " --server daggerhashimoto.hk.nicehash.com:3353 --user " + username + " --ssl 0 --proto stratum --dserver eaglesong.hk.nicehash.com:3381 --duser " + username + " --ssl 0" +
                GetDevicesCommandString();
            }

            if (SecondaryAlgorithmType == AlgorithmType.Handshake)
            {
                ret = " --logfile " + suff + GetLogFileName() + " --color 0 --pec --algo eth+hns" +
                " --server eu1.ethermine.org:4444 --user 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1.GMiner --ssl 0 --proto proxy --dserver hns.f2pool.com:6000 --duser hs1qjq6nglhcmx2xnd30kt3s2rq3fphft459va796j.gminer" +
                " --server daggerhashimoto.eu.nicehash.com:3353 --user " + username + " --ssl 0 --proto stratum --dserver handshake.eu.nicehash.com:3384 --duser " + username + " --ssl 0" +
                " --server daggerhashimoto.hk.nicehash.com:3353 --user " + username + " --ssl 0 --proto stratum --dserver eaglesong.hk.nicehash.com:3381 --duser " + username + " --ssl 0" +
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

                while (IsActiveProcess(BenchmarkHandle.Id))
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

                        //keepRunning = false;
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
                Thread.Sleep(1000 * 5);
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
                            Helpers.ConsolePrint(MinerTag(), lineLowered);
                            if (lineLowered.Contains(LookForStart))
                            {
                                _benchmarkSum += GetNumber(lineLowered);
                                if (BenchmarkAlgorithm is DualAlgorithm dualBenchAlgo)
                                {
                                    _benchmarkSumSecond += GetNumberSecond(lineLowered);
                                }
                                    ++_benchmarkReadCount;
                            }
                        }
                    }

                    if (_benchmarkReadCount > 0)
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = _benchmarkSum / _benchmarkReadCount;
                        if (BenchmarkAlgorithm is DualAlgorithm dualBenchAlgo)
                        {
                            dualBenchAlgo.SecondaryBenchmarkSpeed = _benchmarkSumSecond / _benchmarkReadCount;
                        }
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
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckaroo29 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32 ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                return GetNumber(outdata, LookForStart, "g/s");
            } else if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.Eaglesong ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroo29BFC ||
                MiningSetup.CurrentAlgorithmType == AlgorithmType.Handshake)
            {
                return GetNumber(outdata, LookForStart, "h/s");
            } else
            {
                return GetNumber(outdata, LookForStart, LookForEnd);
            }
        }
        //|  GPU0 58 C 378.05 MH/s    0/0  87 W  4.35 MH/W |
        //|  GPU0 61 C  24.50 MH/s + 291.49 MH/s  1/0 + 0/0 113 W  216.79 KH/W |
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
                public double speed2 { get; set; }
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
            ApiData ad;

            if (SecondaryAlgorithmType == AlgorithmType.Eaglesong)
            {
                ad = new ApiData(AlgorithmType.DaggerEaglesong);
                ad.AlgorithmID = AlgorithmType.DaggerHashimoto;
                ad.SecondaryAlgorithmID = AlgorithmType.Eaglesong;
            } else if (SecondaryAlgorithmType == AlgorithmType.Handshake)
            {
                ad = new ApiData(AlgorithmType.DaggerHandshake);
                ad.AlgorithmID = AlgorithmType.DaggerHashimoto;
                ad.SecondaryAlgorithmID = AlgorithmType.Handshake;
            }
            else
            {
                ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            }
            //Helpers.ConsolePrint("GMINER API", "SecondaryAlgorithmType: " + SecondaryAlgorithmType.ToString() + " ad.AlgorithmID: " + ad.AlgorithmID.ToString() + " ad.SecondaryAlgorithmID: " + ad.SecondaryAlgorithmID.ToString());
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

            dynamic resp = JsonConvert.DeserializeObject<JsonApiResponse>(ResponseFromGMiner);

            //Helpers.ConsolePrint("API resp...........", resp);
            if (resp != null)
            {
                for (var i = 0; i < resp.devices.Length; i++)
                {
                    total = total + resp.devices[i].speed;
                    if (SecondaryAlgorithmType == AlgorithmType.Eaglesong)
                    {
                        totalSec = totalSec + resp.devices[i].speed2;
                    }
                    if (SecondaryAlgorithmType == AlgorithmType.Handshake)
                    {
                        totalSec = totalSec + resp.devices[i].speed2;
                    }
                }

                ad.Speed = total;
                if (SecondaryAlgorithmType == AlgorithmType.Eaglesong)
                {
                    ad.SecondarySpeed = totalSec;
                }
                if (SecondaryAlgorithmType == AlgorithmType.Handshake)
                {
                    ad.SecondarySpeed = totalSec;
                }

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
