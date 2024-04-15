using Newtonsoft.Json;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        string gminer_var = "";
        protected AlgorithmType SecondaryAlgorithmType = AlgorithmType.NONE;
        private double _power = 0.0d;
        double _powerUsage = 0;
        int addTime = 0;
        int _apiErrors = 0;

        public GMiner(AlgorithmType secondaryAlgorithmType) : base("GMiner")
        {
            ConectionType = NhmConectionType.NONE;
            SecondaryAlgorithmType = secondaryAlgorithmType;
            IsMultiType = true;
        }

        public override void Start(string btcAdress, string worker)
        {
            string url = "";

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
            DeviceType devtype = DeviceType.NVIDIA;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }

            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            KillGminer();
        }
        private string GetStartCommand(string url, string btcAddress, string worker)
        {
            var algo = "";
            var algo2 = "";
            var algoName = "";
            var algoName2 = "";
            var pers = "";
            var nicehashstratum = "";
            var ssl = "";
            string port = "";
            string port2 = "";
            string username = GetUsername(btcAddress, worker);
            
            string ZilMining = "";
            DeviceType devtype = DeviceType.NVIDIA;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }

            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.GMiner, devtype))
            {
                ZilClient.needConnectionZIL = true;
                ZilClient.StartZilMonitor();
            }

            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.GMiner, devtype) &&
                ConfigManager.GeneralConfig.ZIL_mining_state == 1)
            {
                ZilMining = " --zilserver stratum+tcp://daggerhashimoto.auto.nicehash.com:9200 --ziluser " + username + " ";
            }

            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.GMiner, devtype) &&
                ConfigManager.GeneralConfig.ZIL_mining_state == 2)
            {
                ZilMining = " --zilserver " + ConfigManager.GeneralConfig.ZIL_mining_pool + ":" + 
                    ConfigManager.GeneralConfig.ZIL_mining_port + " --ziluser " + ConfigManager.GeneralConfig.ZIL_mining_wallet + "." + worker + " ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                algo = "144_5";
                algoName = "zhash";
                pers = " --pers auto ";
                port = "3369";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZelHash)
            {
                algo = "125_4";
                algoName = "zelhash";
                pers = " --pers auto ";
                port = "3391";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                algo = "BeamHashIII";
                algoName = "beamv3";
                ssl = " --ssl_verification 0";
                port = "3387";
            }
            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                algo = "aeternity";
                algoName = "cuckoocycle";
                port = "3376";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                algo = "ethash";
                algoName = "daggerhashimoto";
                nicehashstratum = " --proto stratum";
                port = "3353";
                ZilMining = "";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
            {
                algo = "etchash";
                algoName = "etchash";
                nicehashstratum = " --proto stratum";
                port = "3393";
                ZilMining = "";
            }
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.IronFish)
            {
                algo = "ironfish";
                algoName = "ironfish";
                nicehashstratum = " --proto stratum";
                port = "3397";
            }
            */
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KarlsenHash)
            {
                algo = "karlsenhash";
                algoName = "karlsenhash";
                nicehashstratum = " --proto stratum";
                port = "3398";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus)
            {
                algo = "octopus";
                algoName = "octopus";
                nicehashstratum = " --proto stratum";
                port = "3389";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
            {
                algo = "kawpow";
                algoName = "kawpow";
                ssl = " --ssl 0";
                nicehashstratum = " --proto stratum";
                port = "3385";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOWLite)
            {
                algo = "kawpow";
                algoName = "kawpow";
                ssl = " --ssl 0";
                nicehashstratum = " --proto stratum";
                port = "3385";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                algo = "grin32";
                algoName = "grincuckatoo32";
                ssl = " --ssl 0";
                nicehashstratum = " --pec ";
                port = "3383";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos)
            {
                algo = "autolykos2";
                algoName = "autolykos";
                ssl = " --ssl 0";
                nicehashstratum = " --proto stratum";
                port = "3390";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KarlsenHash)
            {
                algo = "karlsenhash";
                algoName = "karlsenhash";
                ssl = " --ssl 0";
                nicehashstratum = " --proto stratum";
                port = "3398";
            }

            //
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                algo = "autolykos2";
                algoName = "autolykos";
                nicehashstratum = " --proto stratum";
                port = "3390";

                algo2 = "ironfish";
                algoName2 = "ironfish";
                nicehashstratum = " --proto stratum";
                port2 = "3397";

                return GetDevicesCommandString() + nicehashstratum +
                      " --algo " + algo + " --dalgo " + algo2 + pers +
                      GetServerDual(algoName, algoName2, username, port, port2) + ZilMining +
                      " --api " + ApiPort;
            }
            */
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                algo = "octopus";
                algoName = "octopus";
                //nicehashstratum = " --proto stratum";
                port = "3389";

                algo2 = "ironfish";
                algoName2 = "ironfish";
                nicehashstratum = " --proto stratum";
                port2 = "3397";

                return GetDevicesCommandString() + nicehashstratum +
                      " --algo " + algo + " --dalgo " + algo2 + pers +
                      GetServerDual(algoName, algoName2, username, port, port2) + ZilMining +
                      " --api " + ApiPort;
            }
            */
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                algo = "ethash";
                algoName = "daggerhashimoto";
                nicehashstratum = " --proto stratum";
                port = "3353";

                algo2 = "ironfish";
                algoName2 = "ironfish";
                nicehashstratum = " --proto stratum";
                port2 = "3397";

                return GetDevicesCommandString() + nicehashstratum +
                      " --algo " + algo + " --dalgo " + algo2 + pers +
                      GetServerDual(algoName, algoName2, username, port, port2) +
                      " --api " + ApiPort;
            }
            */
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                algo = "etchash";
                algoName = "etchash";
                nicehashstratum = " --proto stratum";
                port = "3393";

                algo2 = "ironfish";
                algoName2 = "ironfish";
                nicehashstratum = " --proto stratum";
                port2 = "3397";

                return GetDevicesCommandString() + nicehashstratum +
                      " --algo " + algo + " --dalgo " + algo2 + pers +
                      GetServerDual(algoName, algoName2, username, port, port2) +
                      " --api " + ApiPort;
            }
            */

            return GetDevicesCommandString() + nicehashstratum +
                  " --algo " + algo + pers +
                  GetServer(algoName, username, port) + ZilMining +
                  " --api " + ApiPort;
        }

        private string GetServerDual(string algo, string algo2, string username, string port, string port2)
        {
            string ret = "";
            string ssl = "";
            string dssl = "";
            string psw = "x";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                port2 = "4" + port2;
                ssl = "--ssl 1 ";
                dssl = "--dssl 1 ";
            }
            else
            {
                port = "1" + port;
                port2 = "1" + port2;
                ssl = "--ssl 0 ";
                dssl = "--dssl 0 ";
            }
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " -s " + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u " +
                        username + " -p " + psw + " --ssl 0 " +
                        " --dserver " + Links.CheckDNS(algo2 + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 --duser " +
                        username + " --dpass " + psw + " --dssl 0 ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " -s " + Links.CheckDNS("stratum." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u " +
                        username + " -p " + psw + " " + ssl + " " +
                        " --dserver " + Links.CheckDNS("stratum." + serverUrl).Replace("stratum+tcp://", "") + ":" + port2 + " --duser " +
                        username + " --dpass " + psw + " " + dssl;
                }
            }
            return ret;
        }
        private string GetServer(string algo, string username, string port)
        {
            string ret = "";
            string ssl = "";
            string psw = "x";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL)
            {
                port = "4" + port;
                ssl = "--ssl 1 ";
            }
            else
            {
                port = "1" + port;
                ssl = "--ssl 0 ";
            }
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " -s " + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u " + 
                        username + " -p " + psw + " --ssl 0 ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " -s " + Links.CheckDNS("stratum." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u " + 
                        username + " -p " + psw + " " + ssl;
                }
            }
            return ret;
        }
        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = "  --watchdog 0 --devices ";
            var ids = new List<string>();
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
                }
                else
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

            try
            {
                var GMinerHandle = new Process
                {
                    StartInfo =
                {
                    FileName = "taskkill.exe"
                }
                };
                GMinerHandle.StartInfo.Arguments = "/PID " + pid.ToString() + " /F /T";
                GMinerHandle.StartInfo.UseShellExecute = false;
                GMinerHandle.StartInfo.CreateNoWindow = true;
                GMinerHandle.Start();
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("KillProcessAndChildren", ex.ToString());
            }
            
            Thread.Sleep(100);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            /*
            Thread.Sleep(100);
            try
            {
                Process proc = Process.GetProcessById(pid);
                if (proc != new Process()) proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
            */
        }
        public override void EndBenchmarkProcces()
        {
            if (BenchmarkProcessStatus != BenchmarkProcessStatus.Killing && BenchmarkProcessStatus != BenchmarkProcessStatus.DoneKilling)
            {
                BenchmarkProcessStatus = BenchmarkProcessStatus.Killing;
                try
                {
                    Helpers.ConsolePrint("BENCHMARK",
                        $"Trying to kill benchmark process {ProcessTag()} algorithm {BenchmarkAlgorithm.AlgorithmName}");

                    int k = ProcessTag().IndexOf("pid(");
                    int i = ProcessTag().IndexOf(")|bin");
                    var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();

                    int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                    Helpers.ConsolePrint("BENCHMARK", "gminer.exe PID: " + pid.ToString());
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
                Helpers.ConsolePrint("GMINER", "kill gminer.exe PID: " + pid.ToString());
                KillProcessAndChildren(pid);
                if (ProcessHandle is object && ProcessHandle != null)
                {
                    ProcessHandle.Kill();
                    ProcessHandle.Close();
                }
            }
            catch { }
            //if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();

        }
        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.AMD && 
                    (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto ||
                    MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash))
                {
                    addTime = 30;
                }
            }

            var ret = "";

            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                ret = " --color 0 --pec --pers auto --algo 144_5" +
                " --server " + Links.CheckDNS("stratum+tcp://btg.2miners.com").Replace("stratum+tcp://", "") + " --user GeKYDPRcemA3z9okSUhe9DdLQ7CRhsDBgX.gminer --pass x --port 4040 " +
                " --server " + Links.CheckDNS("stratum+tcp://equihash144.eu.mine.zpool.ca").Replace("stratum+tcp://", "") + " --user 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 --pass c=BTC --port 2144 " +
                " --server " + Links.CheckDNS("stratum+tcp://zhash.auto.nicehash.com").Replace("stratum+tcp://", "") + " --user " + Globals.DemoUser + " --pass x --port 9200" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZelHash)
            {
                ret = " --color 0 --pec --pers auto --algo 125_4" +
                " --server " + Links.CheckDNS("stratum+tcp://flux.2miners.com").Replace("stratum+tcp://", "") + " --user t1RyEzV5eAo95LbQiLZfzmGZGK9vTkdeBDd.gminer --pass x --port 9090 " +
                " --server " + Links.CheckDNS("stratum+tcp://zelhash.auto.nicehash.com").Replace("stratum+tcp://", "") + " --user " + Globals.DemoUser + " --pass x --port 9200" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                ret = " --color 0 --pec --algo BeamHashIII" +
                " --server " + Links.CheckDNS("stratum+ssl://beam.2miners.com:5252") + " --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.gminer --pass x " +
                " --server " + Links.CheckDNS("stratum+tcp://beamv3.auto.nicehash.com:9200") + " --user " + Globals.DemoUser + " --pass x" +
                GetDevicesCommandString();
            }

            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                ret = " --color 0 --pec --algo aeternity" +
                " --server " + Links.CheckDNS("stratum+tcp://ae.2miners.com:4040").Replace("stratum+tcp://", "") + " --user ak_25J5KBhdHcsemmgmnaU4QpcRQ9xgKS5ChBwCaZcEUc85qkgcXE.gminer --pass x" +
                " --server " + Links.CheckDNS("stratum+tcp://cuckoocycle.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " --user " + Globals.DemoUser + " --pass x" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                ret = " --color 0 --pec --algo ethash" +
                " --server " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020").Replace("stratum+tcp://", "") + " --user 0x266b27bd794d1A65ab76842ED85B067B415CD505.GMiner --pass x" +
                " --server " + Links.CheckDNS("stratum+tcp://daggerhashimoto.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " --user " + Globals.DemoUser + " --pass x" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
            {
                ret = " --color 0 --pec --algo etchash" +
                " --server " + Links.CheckDNS("stratum+tcp://etc.2miners.com:1010").Replace("stratum+tcp://", "") + " --user 0x266b27bd794d1A65ab76842ED85B067B415CD505.GMiner --pass x" +
                " --server " + Links.CheckDNS("stratum+tcp://etchash.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " --user " + Globals.DemoUser + " --pass x" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
            {
                ret = " --color 0 --pec --algo kawpow" +
                " --server " + Links.CheckDNS("stratum+tcp://rvn.2miners.com:6060").Replace("stratum+tcp://", "") + " --user RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.GMiner --pass x " +
                " --server " + Links.CheckDNS("stratum+tcp://kawpow.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " --user " + Globals.DemoUser + " --pass x" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOWLite)
            {
                ret = " --color 0 --pec --algo kawpow" +
                " --server " + Links.CheckDNS("stratum+tcp://kawpow.mine.zergpool.com:3638").Replace("stratum+tcp://", "") + " --user LPeihdgf7JRQUNq5cwZbBQQgEmh1m7DSgH.GMiner --pass c=LTC,mc=XNA/CLORE/SATOX/GPN/PAPRY/MEWC/FREN/AIPG " +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                ret = " --color 0 --pec --algo grin32" +
                " --server " + Links.CheckDNS("stratum+tcp://grincuckatoo32.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " --user " + Globals.DemoUser + " --pass x" +
                GetDevicesCommandString();
            }
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.IronFish)
            {
                ret = " --color 0 --pec --algo ironfish" +
                " --server " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --user fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521 --pass x" +
                GetDevicesCommandString();
            }
            */
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KarlsenHash)
            {
                ret = " --color 0 --pec --algo karlsenhash" +
                " --server " + Links.CheckDNS("kls.2miners.com:2020").Replace("stratum+tcp://", "") + " --user karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.gminer --pass x" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus)
            {
                ret = " --color 0 --pec --algo octopus" +
                " --server " + Links.CheckDNS("pool.woolypooly.com:3094").Replace("stratum+tcp://", "") + " --user cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd --pass x" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos)
            {
                ret = " --color 0 --pec --algo autolykos2" +
                " --server " + Links.CheckDNS("pool.woolypooly.com:3100").Replace("stratum+tcp://", "") + " --user 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt --pass x " +
                GetDevicesCommandString();
            }
            //duals
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                ret = " --color 0 --pec --algo autolykos2" +
                " --server " + Links.CheckDNS("pool.woolypooly.com:3100").Replace("stratum+tcp://", "") + " --user 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt --pass x " +
                "--dalgo ironfish --dserver " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --duser fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.GMiner --dpass x " +
                GetDevicesCommandString();
                addTime = 60;
            }
            */
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                ret = " --color 0 --pec --algo octopus" +
                " --server " + Links.CheckDNS("pool.woolypooly.com:3094").Replace("stratum+tcp://", "") + " --user cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd --pass x " +
                "--dalgo ironfish --dserver " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --duser fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.GMiner --dpass x " +
                GetDevicesCommandString();
                addTime = 60;
            }
            */
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                ret = " --color 0 --pec --algo ethash" +
                " --server " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020").Replace("stratum+tcp://", "") + " --user 0x266b27bd794d1A65ab76842ED85B067B415CD505.GMiner --pass x " +
                "--dalgo ironfish --dserver " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --duser fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.GMiner --dpass x " +
                GetDevicesCommandString();
                addTime = 60;
            }
            */
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                ret = " --color 0 --pec --algo etchash" +
                " --server " + Links.CheckDNS("stratum+tcp://etc.2miners.com:1010").Replace("stratum+tcp://", "") + " --user 0x266b27bd794d1A65ab76842ED85B067B415CD505.GMiner --pass x " +
                "--dalgo ironfish --dserver " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --duser fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.GMiner --dpass x " +
                GetDevicesCommandString();
                addTime = 30;
            }
            */
            _benchmarkTimeWait = time + addTime;
            return ret + " --api " + ApiPort; 
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
            double secsummspeed = 0.0d;

            int delay_before_calc_hashrate = 5;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                _benchmarkTimeWait = _benchmarkTimeWait + 10;
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) ||
                    MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.AutolykosIronFish))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 30;
                }
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in : " + _benchmarkTimeWait + " seconds");
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
                        break;
                    }
                    // wait a second due api request
                    Thread.Sleep(1000);

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 15;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 15;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 15;
                    }
                    /*
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.IronFish))
                    {
                        MinerStartDelay = 5;
                        delay_before_calc_hashrate = 5;
                    }
                    */
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KarlsenHash))
                    {
                        MinerStartDelay = 5;
                        delay_before_calc_hashrate = 5;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ZHash))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 15;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ZelHash))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 15;
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
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOWLite))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 5;
                    }
                    //duals
                    /*
                    if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.IronFish))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 90;
                    }
                    */
                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        _powerUsage += _power;
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() + " Dual: " + ad.Result.SecondarySpeed.ToString() + " power: " + _power.ToString());
                            summspeed += ad.Result.Speed;
                            secsummspeed += ad.Result.SecondarySpeed;
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

                            try
                            {
                                KillProcessAndChildren(BenchmarkHandle.Id);
                                BenchmarkHandle.Dispose();
                                EndBenchmarkProcces();
                            } catch (Exception ex)
                            {
                                Helpers.ConsolePrint("**", ex.ToString());
                            }

                            break;
                        }

                    }
                }

                BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
                BenchmarkAlgorithm.BenchmarkSecondarySpeed = Math.Round(secsummspeed / (repeats - delay_before_calc_hashrate), 2);
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

        // stub benchmarks read from file
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
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
                public double speed3 { get; set; }
                public string speed_unit { get; set; }
                public string speed_unit2 { get; set; }
                public string speed_unit3 { get; set; }

            }
            public Devices[] devices { get; set; }
            public string miner { get; set; }
            public string algorithm { get; set; }
        }

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            //Helpers.ConsolePrint("try API...........", "");
            //ApiData ad;
            CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            ad.ThirdAlgorithmID = AlgorithmType.NONE;
            DeviceType devtype = DeviceType.NVIDIA;

            string ResponseFromGMiner = "";
            double total = 0;
            double total2 = 0;
            double total3 = 0;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/stat");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 3 * 1000;
                WR.ReadWriteTimeout = 3 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 2 * 1000;
                StreamReader Reader = new StreamReader(SS);
                Reader.BaseStream.ReadTimeout = 3 * 1000;
                ResponseFromGMiner = await Reader.ReadToEndAsync();
                //Helpers.ConsolePrint("->", ResponseFromGMiner);
                if (ResponseFromGMiner.Length == 0 || (ResponseFromGMiner[0] != '{' && ResponseFromGMiner[0] != '['))
                    throw new WebException();
                Reader.Close();
                Response.Close();
            }
            catch (Exception)
            {
                _apiErrors++;
                Helpers.ConsolePrint("GetSummaryAsync", "GMINER-API ERRORs count: " + _apiErrors.ToString());
                if (_apiErrors > 60)
                {
                    Helpers.ConsolePrint("GetSummaryAsync", "Need RESTART GMINER");
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                    ad.Speed = 0;
                    ad.SecondarySpeed = 0;
                    ad.ThirdSpeed = 0;
                    return ad;
                }
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                ad.Speed = 0;
                ad.SecondarySpeed = 0;
                ad.ThirdSpeed = 0;
                return ad;
            }
            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            ad.ThirdAlgorithmID = AlgorithmType.NONE;

            ResponseFromGMiner = ResponseFromGMiner.Replace("-nan", "0.00");
            ResponseFromGMiner = ResponseFromGMiner.Replace("(ind)", "");
            //Helpers.ConsolePrint("->", ResponseFromGMiner);
            string _algo = "";
            string _miner = "";
            try
            {
                dynamic resp = JsonConvert.DeserializeObject<JsonApiResponse>(ResponseFromGMiner);
                if (resp != null)
                {
                    _miner = resp.miner;
                    _algo = resp.algorithm;
                    double[] hashrates = new double[resp.devices.Length];
                    double[] hashrates2 = new double[resp.devices.Length];
                    double[] hashrates3 = new double[resp.devices.Length];
                    for (var i = 0; i < resp.devices.Length; i++)
                    {
                        total = total + resp.devices[i].speed;
                        total2 = total2 + resp.devices[i].speed2;
                        total3 = total3 + resp.devices[i].speed3;
                        hashrates[i] = resp.devices[i].speed;
                        hashrates2[i] = resp.devices[i].speed2;
                        hashrates3[i] = resp.devices[i].speed3;
                        /*
                        Helpers.ConsolePrint("****", " dev: " + i.ToString() + " hr1: " + hashrates[i].ToString() +
                            " hr2: " + hashrates2[i].ToString() +
                            " hr3: " + hashrates3[i].ToString());
                        */
                    }
                    /*
                    if (_algo.ToLower().Contains("zil") && total2 + total3 > 0)
                    {
                        Form_Main.isZilRound = true;
                    } else
                    {
                        Form_Main.isZilRound = false;
                    }
                    */
                    int dev = 0;
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                    if (Form_Main.NVIDIA_orderBug)
                    {
                        sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
                    }

                    foreach (var mPair in sortedMinerPairs)
                    {
                        devtype = mPair.Device.DeviceType;
                        _power = mPair.Device.PowerUsage;
                        mPair.Device.MiningHashrate = hashrates[dev];
                        mPair.Device.MiningHashrateSecond = hashrates2[dev];
                        mPair.Device.MiningHashrateThird = hashrates3[dev];
                        //duals
                        /*
                        if ((MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto ||
                            MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash) &&
                            MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
                            MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
                        {
                            mPair.Device.MiningHashrate = hashrates[dev];
                            mPair.Device.MiningHashrateSecond = hashrates2[dev];
                            mPair.Device.MiningHashrateThird = 0;
                            mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                            mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                            mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                        }

                        if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos &&
                            MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
                        {
                            if (Form_Main.isZilRound && Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.GMiner, devtype))
                            {
                                mPair.Device.MiningHashrate = 0;
                                mPair.Device.MiningHashrateSecond = 0;
                                mPair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                                mPair.Device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.DaggerHashimoto;
                            }
                            else
                            {
                                mPair.Device.MiningHashrateThird = 0;
                                mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                                mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                        }
                        if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus &&
                            MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
                        {
                            if (Form_Main.isZilRound && Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.GMiner, devtype))
                            {
                                mPair.Device.MiningHashrate = 0;
                                mPair.Device.MiningHashrateSecond = 0;
                                mPair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                                mPair.Device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.DaggerHashimoto;
                            }
                            else
                            {
                                mPair.Device.MiningHashrateThird = 0;
                                mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                                mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                        }
                        */
                        //


                        if (MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NONE)//single
                        {
                            if (Form_Main.isZilRound && Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.GMiner, devtype))
                            {
                                mPair.Device.MiningHashrate = 0;
                                mPair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                                mPair.Device.SecondAlgorithmID = (int)AlgorithmType.DaggerHashimoto;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                            else
                            {
                                mPair.Device.MiningHashrateSecond = 0;
                                mPair.Device.MiningHashrateThird = 0;
                                mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                                mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                        }
                        dev++;
                    }
                }
                else
                {
                    Helpers.ConsolePrint("GMiner:", "resp - null");
                }
                _apiErrors = 0;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GMiner API:", ex.ToString());
            }
            finally
            {
                
                if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))//???
                {
                    ad.SecondaryAlgorithmID = MiningSetup.CurrentSecondaryAlgorithmType;
                }

                ad.ZilRound = false;
                ad.Speed = total;
                ad.SecondarySpeed = total2;
                ad.ThirdSpeed = total3;

                if (Form_Main.isZilRound && Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.GMiner, devtype))
                {
                    if (MiningSetup.CurrentSecondaryAlgorithmType != AlgorithmType.NONE)//dual
                    {
                        if (_algo.ToLower().Contains("zil") && total3 > 0)//dual+zil
                        {
                            ad.Speed = 0;
                            ad.SecondarySpeed = 0;
                            ad.ThirdSpeed = total3;
                            ad.ZilRound = true;
                            ad.AlgorithmID = AlgorithmType.NONE;
                            ad.SecondaryAlgorithmID = AlgorithmType.NONE;
                            ad.ThirdAlgorithmID = AlgorithmType.DaggerHashimoto;
                        }
                    }
                    else
                    {
                        if (_algo.ToLower().Contains("zil") && total2 > 0)//+zil
                        {
                            ad.Speed = 0;
                            ad.SecondarySpeed = total2;
                            ad.ThirdSpeed = 0;
                            ad.ZilRound = true;
                            ad.AlgorithmID = AlgorithmType.NONE;
                            ad.SecondaryAlgorithmID = AlgorithmType.DaggerHashimoto;
                        }
                    }
                }
                else
                {
                    ad.ZilRound = false;
                    ad.ThirdSpeed = 0;
                    ad.ThirdAlgorithmID = AlgorithmType.NONE;

                    if (MiningSetup.CurrentSecondaryAlgorithmType != AlgorithmType.NONE)//dual
                    {
                        //if (_algo.ToLower().Contains("zil"))//dual
                        {
                            ad.Speed = total;
                            ad.SecondarySpeed = total2;
                        }
                    }
                    else
                    {
                        //if (_algo.ToLower().Contains("zil"))
                        {
                            ad.Speed = total;
                            ad.SecondarySpeed = 0;
                            ad.SecondaryAlgorithmID = AlgorithmType.NONE;
                        }
                    }
                }

                if (ad.Speed == 0 && ad.SecondarySpeed == 0 && ad.ThirdSpeed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }
                else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                    foreach (var mPair in sortedMinerPairs)
                    {
                        devtype = mPair.Device.DeviceType;
                    }
                }

            }
            /*
            Helpers.ConsolePrint("*******", "CurrentAlgorithmType: " + MiningSetup.CurrentAlgorithmType.ToString() +
       " CurrentSecondaryAlgorithmType: " + MiningSetup.CurrentSecondaryAlgorithmType.ToString() +
       " Form_Main.isZilRound: " + Form_Main.isZilRound.ToString());
            */
            Thread.Sleep(100);
            /*
            //������� ��-�� ���� � Anti-hacking
            if (fs.Length > offset)
            {
                int count = (int)(fs.Length - offset);
                byte[] array = new byte[count];
                fs.Read(array, 0, count);
                offset = (int)fs.Length;
                string textFromFile = System.Text.Encoding.Default.GetString(array).Trim();
                //Helpers.ConsolePrint(MinerTag(), textFromFile);
                if (textFromFile.Contains("Anti-hacking"))
                {
                    Helpers.ConsolePrint(MinerTag(), "GMiner Anti-hacking bug detected.");
                    ad.Speed = 0;
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                }
            }
            */
            return ad;
        }
    }
}
