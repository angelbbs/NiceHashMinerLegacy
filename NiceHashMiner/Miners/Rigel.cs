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
    public class Rigel : Miner
    {
        private int _benchmarkTimeWait = 120;
        private const double DevFee = 2.0;
        protected AlgorithmType SecondaryAlgorithmType = AlgorithmType.NONE;
        private double _power = 0.0d;
        double _powerUsage = 0;
        int addTime = 0;
        int _apiErrors = 0;
        bool isZILround = false;
        int RejectsLimit = 0;

        public Rigel() : base("Rigel")
        {
            ConectionType = NhmConectionType.NONE;
        }

        public override void Start(string btcAdress, string worker)
        {
            string url = "";
            RejectsLimit = ConfigManager.GeneralConfig.KAWPOW_Rigel_Max_Rejects;
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            DeviceType devtype = DeviceType.NVIDIA;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }

            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            KillRigel();
        }
        private string GetStartCommand(string url, string btcAddress, string worker)
        {
            var algo = "";
            var algo2 = "";
            var algoName = "";
            var algoName2 = "";
            var nicehashstratum = "";
            var ssl = "";
            string port = "";
            string port2 = "";
            string username = GetUsername(btcAddress, worker);

            string ZilMining = "";
            string MainMining = "";
            string ZilAlgo = "";
            DeviceType devtype = DeviceType.NVIDIA;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }

            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Rigel, devtype))
            {
                ZilClient.needConnectionZIL = true;
                ZilClient.StartZilMonitor();
            }

            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Rigel, devtype) &&
                MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NONE)//single
            {
                //прокси не используется
                ZilAlgo = "+zil";
                MainMining = "[1]";
                if (ConfigManager.GeneralConfig.ZIL_mining_state == 1)
                {
                    ZilMining = " -o [2]ethstratum+tcp://daggerhashimoto.auto.nicehash.com:9200 -u [2]" + username + " ";
                }
                if (ConfigManager.GeneralConfig.ZIL_mining_state == 2)
                {
                    ZilMining = " -o [2]ethstratum+tcp://" + 
                        ConfigManager.GeneralConfig.ZIL_mining_pool.Replace("stratum+tcp://", "") + ":" + ConfigManager.GeneralConfig.ZIL_mining_port +
                        " -u [2]" + ConfigManager.GeneralConfig.ZIL_mining_wallet + "." + worker + " ";
                }
            }
            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Rigel, devtype) &&
                MiningSetup.CurrentSecondaryAlgorithmType != AlgorithmType.NONE)//dual
            {
                //прокси не используется
                ZilAlgo = "+zil";
                MainMining = "";
                if (ConfigManager.GeneralConfig.ZIL_mining_state == 1)
                {
                    ZilMining = " -o [3]ethstratum+tcp://daggerhashimoto.auto.nicehash.com:9200 -u [3]" + username + " ";
                }
                if (ConfigManager.GeneralConfig.ZIL_mining_state == 2)
                {
                    ZilMining = " -o [3]ethstratum+tcp://" +
                        ConfigManager.GeneralConfig.ZIL_mining_pool.Replace("stratum+tcp://", "") + ":" + ConfigManager.GeneralConfig.ZIL_mining_port +
                        " -u [3]" + ConfigManager.GeneralConfig.ZIL_mining_wallet + "." + worker + " ";
                }
            }

            if (MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NONE)//single
            {
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
                {
                    algo = "ethash";
                    algoName = "daggerhashimoto";
                    nicehashstratum = "";
                    port = "3353";
                    ZilMining = "";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
                {
                    algo = "etchash";
                    algoName = "etchash";
                    nicehashstratum = "";
                    port = "3393";
                    ZilMining = "";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
                {
                    algo = "kawpow" + ZilAlgo;
                    algoName = "kawpow";
                    nicehashstratum = "";
                    port = "3385";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.NexaPow)
                {
                    algo = "nexapow" + ZilAlgo;
                    algoName = "nexapow";
                    nicehashstratum = "";
                    port = "3396";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos)
                {
                    algo = "autolykos2" + ZilAlgo;
                    algoName = "autolykos";
                    nicehashstratum = "";
                    port = "3390";
                }
                /*
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.IronFish)
                {
                    algo = "ironfish" + ZilAlgo;
                    algoName = "ironfish";
                    nicehashstratum = "";
                    port = "3397";
                }
                */
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KarlsenHash)
                {
                    algo = "karlsenhash" + ZilAlgo;
                    algoName = "karlsenhash";
                    nicehashstratum = "";
                    port = "3398";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus)
                {
                    algo = "octopus" + ZilAlgo;
                    algoName = "octopus";
                    nicehashstratum = "";
                    port = "3389";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Alephium)
                {
                    algo = "alephium" + ZilAlgo;
                    algoName = "alephium";
                    nicehashstratum = "";
                    port = "3399";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash)
                {
                    algo = "fishhash" + ZilAlgo;
                    algoName = "fishhash";
                    nicehashstratum = "";
                    port = "3400";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.PyrinHash)
                {
                    algo = "pyrinhash" + ZilAlgo;
                    algoName = "pyrinhash";
                    nicehashstratum = "";
                    port = "3401";
                }

                return GetDevicesCommandString() + nicehashstratum +
                  " -a " + algo +
                  GetServer(algoName, username, port, MainMining) + ZilMining +
                  " --api-bind 127.0.0.1:" + ApiPort;

            }
            else //dual
            {
                /*
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
                {
                    algo = "autolykos2+ironfish" + ZilAlgo;
                    algoName = "autolykos";
                    algoName2 = "ironfish";
                    nicehashstratum = "";
                    port = "3390";
                    port2 = "3397";
                }
                */
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                {
                    algo = "autolykos2+karlsenhash" + ZilAlgo;
                    algoName = "autolykos";
                    algoName2 = "karlsenhash";
                    nicehashstratum = "";
                    port = "3390";
                    port2 = "3398";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                {
                    algo = "autolykos2+alephium" + ZilAlgo;
                    algoName = "autolykos";
                    algoName2 = "alephium";
                    nicehashstratum = "";
                    port = "3390";
                    port2 = "3399";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
                {
                    algo = "autolykos2+pyrinhash" + ZilAlgo;
                    algoName = "autolykos";
                    algoName2 = "pyrinhash";
                    nicehashstratum = "";
                    port = "3390";
                    port2 = "3401";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                {
                    algo = "octopus+karlsenhash" + ZilAlgo;
                    algoName = "octopus";
                    algoName2 = "karlsenhash";
                    nicehashstratum = "";
                    port = "3389";
                    port2 = "3398";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                {
                    algo = "octopus+alephium" + ZilAlgo;
                    algoName = "octopus";
                    algoName2 = "alephium";
                    nicehashstratum = "";
                    port = "3389";
                    port2 = "3399";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
                {
                    algo = "octopus+pyrinhash" + ZilAlgo;
                    algoName = "octopus";
                    algoName2 = "pyrinhash";
                    nicehashstratum = "";
                    port = "3389";
                    port2 = "3401";
                }
                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                {
                    algo = "etchash+karlsenhash" + ZilAlgo;
                    algoName = "etchash";
                    algoName2 = "karlsenhash";
                    nicehashstratum = "";
                    port = "3393";
                    port2 = "3398";
                }

                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                {
                    algo = "etchash+alephium" + ZilAlgo;
                    algoName = "etchash";
                    algoName2 = "alephium";
                    nicehashstratum = "";
                    port = "3393";
                    port2 = "3399";
                }

                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                {
                    algo = "ethash+karlsenhash" + ZilAlgo;
                    algoName = "daggerhashimoto";
                    algoName2 = "karlsenhash";
                    nicehashstratum = "";
                    port = "3353";
                    port2 = "3398";
                }

                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                {
                    algo = "ethash+alephium" + ZilAlgo;
                    algoName = "daggerhashimoto";
                    algoName2 = "alephium";
                    nicehashstratum = "";
                    port = "3353";
                    port2 = "3399";
                }

                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                {
                    algo = "fishhash+karlsenhash" + ZilAlgo;
                    algoName = "fishhash";
                    algoName2 = "karlsenhash";
                    nicehashstratum = "";
                    port = "3400";
                    port2 = "3398";
                }

                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                {
                    algo = "fishhash+alephium" + ZilAlgo;
                    algoName = "fishhash";
                    algoName2 = "alephium";
                    nicehashstratum = "";
                    port = "3400";
                    port2 = "3399";
                }

                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash &&
                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
                {
                    algo = "fishhash+pyrinhash" + ZilAlgo;
                    algoName = "fishhash";
                    algoName2 = "pyrinhash";
                    nicehashstratum = "";
                    port = "3400";
                    port2 = "3401";
                }

                return GetDevicesCommandString() + nicehashstratum +
                  " -a " + algo +
                  GetServerDual(algoName, algoName2, username, port, port2, MainMining) + ZilMining +
                  " --api-bind 127.0.0.1:" + ApiPort;
            }
            return "Ooops";
        }

        
        private string GetServer(string algo, string username, string port, string MainMining = "")
        {
            string ret = "";
            string psw = "x";
            string stratum = "";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                stratum = "stratum+ssl://";
            }
            else
            {
                port = "1" + port;
                stratum = "stratum+tcp://";
            }
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";

            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " -o " + MainMining + stratum + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u " + MainMining +
                        username + " -p " + psw;
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " -o " + MainMining + stratum + Links.CheckDNS("stratum." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u " + MainMining +
                        username + " -p " + psw + " ";
                }
            }
            return ret;
        }
        private string GetServerDual(string algo, string algo2, string username, string port, string port2, string MainMining = "")
        {
            string ret = "";
            string psw = "x";
            string stratum = "";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                port2 = "4" + port2;
                stratum = "stratum+ssl://";
            }
            else
            {
                port = "1" + port;
                port2 = "1" + port2;
                stratum = "stratum+tcp://";
            }
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";

            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " -o [1]" + stratum + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u [1]" + username +
                                " -o [2]" + stratum + Links.CheckDNS(algo2 + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 -u [2]" + username;
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " -o [1]" + stratum + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " -u [1]" + username +
                        " -o [2]" + stratum + Links.CheckDNS(algo2 + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port2 + " -u [2]" + username;
                }
            }
            return ret;
        }
        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " --no-watchdog -d ";
            var ids = new List<string>();
            var zil = new List<string>();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            var extra = "";
            int id;

            DeviceType devtype = DeviceType.NVIDIA;
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }

            foreach (var mPair in sortedMinerPairs)
            {
                id = mPair.Device.IDByBus;

                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
                }
                else
                {
                    extra = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
                }

                {
                    ids.Add(id.ToString());
                }

                if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Rigel, devtype))
                {
                    if (mPair.Device.GpuRam / 1024 > 9 * 1024 * 1024)
                    {
                        zil.Add("on");
                    }
                    else
                    {
                        zil.Add("off");
                    }
                }
            }

            deviceStringCommand += string.Join(",", ids);
            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Rigel, devtype))
            {
                deviceStringCommand += " --zil-cache-dag " + string.Join(",", zil);
            }
            deviceStringCommand = deviceStringCommand + extra + " ";

            return deviceStringCommand;
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
                var RigelHandle = new Process
                {
                    StartInfo =
                {
                    FileName = "taskkill.exe"
                }
                };
                RigelHandle.StartInfo.Arguments = "/PID " + pid.ToString() + " /F /T";
                RigelHandle.StartInfo.UseShellExecute = false;
                RigelHandle.StartInfo.CreateNoWindow = true;
                RigelHandle.Start();
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
                    Helpers.ConsolePrint("BENCHMARK", "Rigel.exe PID: " + pid.ToString());
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
        public void KillRigel()
        {
            try
            {
                int k = ProcessTag().IndexOf("pid(");
                int i = ProcessTag().IndexOf(")|bin");
                var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();

                int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                Helpers.ConsolePrint("Rigel", "kill Rigel.exe PID: " + pid.ToString());
                KillProcessAndChildren(pid);
                if (ProcessHandle is object) ProcessHandle.Close();
            }
            catch { }
            //if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();

        }
        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.NvidiaLHR && MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
                {
                    //addTime = 60;
                }
            }

            var ret = "";

            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);

            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                ret = " -a ethash" +
                " -o " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020") + " -u 0x266b27bd794d1A65ab76842ED85B067B415CD505.Rigel -p x" +
                " -o " + Links.CheckDNS("stratum+tcp://daggerhashimoto.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " -u " + Globals.DemoUser + " -p x" +
                GetDevicesCommandString();
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
            {
                ret = " -a etchash" +
                " -o " + Links.CheckDNS("stratum+tcp://etc.2miners.com:1010") + " -uer 0x266b27bd794d1A65ab76842ED85B067B415CD505.Rigel -p x" +
                " -o " + Links.CheckDNS("stratum+tcp://etchash.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " -u " + Globals.DemoUser + " -p x" +
                GetDevicesCommandString();
            }
            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
            {
                ret = " -a kawpow" +
                " -o " + Links.CheckDNS("rvn.2miners.com:6060") + " -u RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX.Rigel -p x" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.NexaPow)
            {
                ret = " -a nexapow" +
                " -o " + Links.CheckDNS("stratum-eu.rplant.xyz:7092") + " -u nexa:nqtsq5g55l2jhuazhre8zfzfnyxle543wjlapt4huup3x9gy.Rigel -p x" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos)
            {
                ret = " -a autolykos2" +
                " -o " + Links.CheckDNS("pool.woolypooly.com:3100") + " -u 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.Rigel -p x" +
                GetDevicesCommandString();
            }
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.IronFish)
            {
                ret = " -a ironfish" +
                " -o " + Links.CheckDNS("ru.ironfish.herominers.com:1145") + " -u fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.Rigel -p x" +
                GetDevicesCommandString();
            }
            */
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KarlsenHash)
            {
                ret = " -a karlsenhash" +
                " -o " + Links.CheckDNS("kls.2miners.com:2020") + " -u karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.Rigel -p x" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus)
            {
                ret = " -a octopus" +
                " -o " + Links.CheckDNS("pool.woolypooly.com:3094") + " -u cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd.Rigel -p x" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Alephium)
            {
                ret = " -a alephium" +
                " -o " + Links.CheckDNS("ru.alephium.herominers.com:1199") + " -u 12bjcHBTbdqW3zfDc84qq8z6RNZr33oXgqqaYdZRUD5qC.Rigel -p x" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash)
            {
                ret = " -a fishhash" +
                " -o " + Links.CheckDNS("ru.ironfish.herominers.com:1145") + " -u fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.Rigel -p x" +
                GetDevicesCommandString();
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.PyrinHash)
            {
                ret = " -a pyrinhash" +
                " -o " + Links.CheckDNS("pyi.2miners.com:2121") + " -u pyrin:qzhy95jlwufjp7q8exs5vwzzzru74xgl6sedz2c57t7q2w9lvac0u9es2rt5y.Rigel -p x" +
                GetDevicesCommandString();
            }

            //duals
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.IronFish)
            {
                ret = " -a autolykos2+ironfish" +
                " -o [1]" + Links.CheckDNS("pool.woolypooly.com:3100") + " -u [1]9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.Rigel" +
                " -o [2]" + Links.CheckDNS("ru.ironfish.herominers.com:1145") + " -u [2]fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }
            */
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
            {
                ret = " -a autolykos2+karlsenhash" +
                " -o [1]" + Links.CheckDNS("pool.woolypooly.com:3100") + " -u [1]9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.Rigel" +
                " -o [2]" + Links.CheckDNS("kls.2miners.com:2020") + " -u [2]karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
            {
                ret = " -a autolykos2+alephium" +
                " -o [1]" + Links.CheckDNS("pool.woolypooly.com:3100") + " -u [1]9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.Rigel" +
                " -o [2]" + Links.CheckDNS("ru.alephium.herominers.com:1199") + " -u [2]12bjcHBTbdqW3zfDc84qq8z6RNZr33oXgqqaYdZRUD5qC.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
            {
                ret = " -a autolykos2+pyrinhash" +
                " -o [1]" + Links.CheckDNS("pool.woolypooly.com:3100") + " -u [1]9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.Rigel" +
                " -o [2]" + Links.CheckDNS("pyi.2miners.com:2121") + " -u [2]pyrin:qzhy95jlwufjp7q8exs5vwzzzru74xgl6sedz2c57t7q2w9lvac0u9es2rt5y.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
            {
                ret = " -a octopus+karlsenhash" +
                " -o [1]" + Links.CheckDNS("pool.woolypooly.com:3094") + " -u [1]cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd.Rigel" +
                " -o [2]" + Links.CheckDNS("kls.2miners.com:2020") + " -u [2]karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
            {
                ret = " -a octopus+alephium" +
                " -o [1]" + Links.CheckDNS("pool.woolypooly.com:3094") + " -u [1]cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd.Rigel" +
                " -o [2]" + Links.CheckDNS("ru.alephium.herominers.com:1199") + " -u [2]12bjcHBTbdqW3zfDc84qq8z6RNZr33oXgqqaYdZRUD5qC.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
            {
                ret = " -a octopus+pyrinhash" +
                " -o [1]" + Links.CheckDNS("pool.woolypooly.com:3094") + " -u [1]cfx:aakuw91bx9mfhn808n0tczpwt6z1habut6zjrjapsd.Rigel" +
                " -o [2]" + Links.CheckDNS("pyi.2miners.com:2121") + " -u [2]pyrin:qzhy95jlwufjp7q8exs5vwzzzru74xgl6sedz2c57t7q2w9lvac0u9es2rt5y.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
            {
                ret = " -a etchash+karlsenhash" +
                " -o [1]" + Links.CheckDNS("etc.2miners.com:1010") + " -u [1]0x266b27bd794d1A65ab76842ED85B067B415CD505.Rigel" +
                " -o [2]" + Links.CheckDNS("kls.2miners.com:2020") + " -u [2]karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
            {
                ret = " -a etchash+alephium" +
                " -o [1]" + Links.CheckDNS("etc.2miners.com:1010") + " -u [1]0x266b27bd794d1A65ab76842ED85B067B415CD505.Rigel" +
                " -o [2]" + Links.CheckDNS("ru.alephium.herominers.com:1199") + " -u [2]12bjcHBTbdqW3zfDc84qq8z6RNZr33oXgqqaYdZRUD5qC.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
            {
                ret = " -a ethash+karlsenhash" +
                " -o [1]" + Links.CheckDNS("ethw.2miners.com:2020") + " -u [1]0x266b27bd794d1A65ab76842ED85B067B415CD505.Rigel" +
                " -o [2]" + Links.CheckDNS("kls.2miners.com:2020") + " -u [2]karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
            {
                ret = " -a ethash+alephium" +
                " -o [1]" + Links.CheckDNS("ethw.2miners.com:2020") + " -u [1]0x266b27bd794d1A65ab76842ED85B067B415CD505.Rigel" +
                " -o [2]" + Links.CheckDNS("ru.alephium.herominers.com:1199") + " -u [2]12bjcHBTbdqW3zfDc84qq8z6RNZr33oXgqqaYdZRUD5qC.Rigel " +
                GetDevicesCommandString();
                addTime = 30;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
            {
                ret = " -a fishhash+karlsenhash" +
                " -o [1]" + Links.CheckDNS("ru.ironfish.herominers.com:1145") + " -u [1]fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.Rigel" +
                " -o [2]" + Links.CheckDNS("kls.2miners.com:2020") + " -u [2]karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.Rigel " +
                GetDevicesCommandString();
                addTime = 45;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
            {
                ret = " -a fishhash+alephium" +
                " -o [1]" + Links.CheckDNS("ru.ironfish.herominers.com:1145") + " -u [1]fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.Rigel" +
                " -o [2]" + Links.CheckDNS("ru.alephium.herominers.com:1199") + " -u [2]12bjcHBTbdqW3zfDc84qq8z6RNZr33oXgqqaYdZRUD5qC.Rigel " +
                GetDevicesCommandString();
                addTime = 45;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
            {
                ret = " -a fishhash+pyrinhash" +
                " -o [1]" + Links.CheckDNS("ru.ironfish.herominers.com:1145") + " -u [1]fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.Rigel" +
                " -o [2]" + Links.CheckDNS("pyi.2miners.com:2121") + " -u [2]pyrin:qzhy95jlwufjp7q8exs5vwzzzru74xgl6sedz2c57t7q2w9lvac0u9es2rt5y.Rigel " +
                GetDevicesCommandString();
                addTime = 45;
            }

            _benchmarkTimeWait = time + addTime;
            return ret + " --api-bind 127.0.0.1:" + ApiPort; 
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

            int delay_before_calc_hashrate = 15;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
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

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 30;
                    }
                    /*
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.IronFish))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 10;
                    }
                    */
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.FishHash))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 30;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.PyrinHash))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 30;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KarlsenHash))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Alephium))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.NexaPow))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 30;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 30;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Octopus))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 30;
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

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            ad.ThirdAlgorithmID = AlgorithmType.NONE;

            string ResponseFromRigel;
            double total = 0;
            double total2 = 0;
            double totalZIL = 0;
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
                ResponseFromRigel = await Reader.ReadToEndAsync();
                //Helpers.ConsolePrint("->", ResponseFromRigel);
                if (ResponseFromRigel.Length == 0 || (ResponseFromRigel[0] != '{' && ResponseFromRigel[0] != '['))
                    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception)
            {
                _apiErrors++;
                Helpers.ConsolePrint("GetSummaryAsync", "Rigel-API ERRORs count: " + _apiErrors.ToString());
                if (_apiErrors > 60)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                    Helpers.ConsolePrint("GetSummaryAsync", "Need RESTART Rigel");
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
            //return null;
            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            ad.ThirdAlgorithmID = AlgorithmType.NONE;

            ResponseFromRigel = ResponseFromRigel.Replace("-nan", "0.00");
            ResponseFromRigel = ResponseFromRigel.Replace("(ind)", "");
            //Helpers.ConsolePrint("->", ResponseFromRigel);
            string _miner = "";
            try
            {
                dynamic resp = JsonConvert.DeserializeObject(ResponseFromRigel);
                if (resp != null)
                {
                    if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
                    {
                        foreach (var d in resp.pools.kawpow)
                        {
                            int rejected = d.solution_stat.rejected;
                            if (rejected > RejectsLimit)
                            {
                                Helpers.ConsolePrint("GetSummaryAsync", "RESTART Rigel due rejects above limit: " + RejectsLimit.ToString());
                                Restart();
                            }
                        }
                    }
                    var devices = resp.devices;
                    string algorithm = resp.algorithm;
                    string zil_state = "";
                    zil_state = resp.zil_state;
                    if (string.IsNullOrEmpty(zil_state))
                    {
                        zil_state = "";
                    }

                    double[] hashrates = new double[devices.Count];
                    double[] hashrates2 = new double[devices.Count];
                    double[] hashratesZIL = new double[devices.Count];
                    int i = 0;
                    foreach (var d in resp.devices)
                    {
                        int id = d.id;
                        string name = d.name;
                        bool selected = d.selected;

                        double hashrate = 0.0d;
                        double hashrate2 = 0.0d;
                        double hashrateZIL = 0.0d;

                        dynamic _hashrate = null;
                        dynamic _hashrate2 = null;
                        dynamic _hashrateZIL = null;
                        if (selected)
                        {
                            if (MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NONE)//single
                            {
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOW)
                                {
                                    _hashrate = d.hashrate.kawpow;
                                }
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.NexaPow)
                                {
                                    _hashrate = d.hashrate.nexapow;
                                }
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos)
                                {
                                    _hashrate = d.hashrate.autolykos2;
                                }
                                /*
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.IronFish)
                                {
                                    _hashrate = d.hashrate.ironfish;
                                }
                                */
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash)
                                {
                                    _hashrate = d.hashrate.fishhash;
                                }
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.PyrinHash)
                                {
                                    _hashrate = d.hashrate.pyrinhash;
                                }
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KarlsenHash)
                                {
                                    _hashrate = d.hashrate.karlsenhash;
                                }
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus)
                                {
                                    _hashrate = d.hashrate.octopus;
                                }
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Alephium)
                                {
                                    _hashrate = d.hashrate.alephium;
                                }

                                _hashrateZIL = d.hashrate.zil;

                                if (_hashrate == null)
                                {
                                    hashrate = 0.0d;
                                }
                                else
                                {
                                    if (selected)
                                    {
                                        hashrate = (double)_hashrate;
                                    }
                                }

                                if (_hashrateZIL == null)
                                {
                                    hashrate2 = 0.0d;
                                }
                                else
                                {
                                    if (selected)
                                    {
                                        hashrateZIL = (double)_hashrateZIL;
                                    }
                                }
                            }
                            else //dual
                            {
                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                                {
                                    _hashrate = d.hashrate.autolykos2;
                                    _hashrate2 = d.hashrate.karlsenhash;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                                {
                                    _hashrate = d.hashrate.autolykos2;
                                    _hashrate2 = d.hashrate.alephium;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
                                {
                                    _hashrate = d.hashrate.autolykos2;
                                    _hashrate2 = d.hashrate.pyrinhash;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                                {
                                    _hashrate = d.hashrate.octopus;
                                    _hashrate2 = d.hashrate.karlsenhash;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                                {
                                    _hashrate = d.hashrate.octopus;
                                    _hashrate2 = d.hashrate.alephium;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
                                {
                                    _hashrate = d.hashrate.octopus;
                                    _hashrate2 = d.hashrate.pyrinhash;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                                {
                                    _hashrate = d.hashrate.etchash;
                                    _hashrate2 = d.hashrate.karlsenhash;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                                {
                                    _hashrate = d.hashrate.etchash;
                                    _hashrate2 = d.hashrate.alephium;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                                {
                                    _hashrate = d.hashrate.ethash;
                                    _hashrate2 = d.hashrate.karlsenhash;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                                {
                                    _hashrate = d.hashrate.ethash;
                                    _hashrate2 = d.hashrate.alephium;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
                                {
                                    _hashrate = d.hashrate.fishhash;
                                    _hashrate2 = d.hashrate.karlsenhash;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
                                {
                                    _hashrate = d.hashrate.fishhash;
                                    _hashrate2 = d.hashrate.alephium;
                                }

                                if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash &&
                                    MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
                                {
                                    _hashrate = d.hashrate.fishhash;
                                    _hashrate2 = d.hashrate.pyrinhash;
                                }

                                _hashrateZIL = d.hashrate.zil;

                                if (_hashrate == null)
                                {
                                    hashrate = 0.0d;
                                }
                                else
                                {
                                    if (selected)
                                    {
                                        hashrate = (double)_hashrate;
                                    }
                                }
                                if (_hashrate2 == null)
                                {
                                    hashrate2 = 0.0d;
                                }
                                else
                                {
                                    if (selected)
                                    {
                                        hashrate2 = (double)_hashrate2;
                                    }
                                }

                                if (_hashrateZIL == null)
                                {
                                    hashrateZIL = 0.0d;
                                }
                                else
                                {
                                    if (selected)
                                    {
                                        hashrateZIL = (double)_hashrateZIL;
                                    }
                                }
                            }
                            if (zil_state.Contains("mining"))
                            {
                                isZILround = true;
                                Form_Main.isForceZilRound = true;
                                //Helpers.ConsolePrint("Rigel", "_hashrateZIL: " + hashrateZIL.ToString());
                            }
                            else
                            {
                                isZILround = false;
                                Form_Main.isForceZilRound = false;
                                //Helpers.ConsolePrint("Rigel", "isZILround = false");
                            }
                        }

                        total = total + hashrate;
                        if (isZILround)
                        {
                            total2 = total2 + hashrateZIL;
                            hashrates2[i] = hashrateZIL;
                        }
                        else
                        {
                            total2 = total2 + hashrate2;
                            hashrates2[i] = hashrate2;
                        }
                        totalZIL = totalZIL + hashrateZIL;

                        hashrates[i] = hashrate;
                        hashratesZIL[i] = hashrateZIL;

                        i++;
                    }
                    //int dev = 0;
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                    if (Form_Main.NVIDIA_orderBug)
                    {
                        sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
                    }
                    
                    foreach (var mPair in sortedMinerPairs)
                    {
                        _power = mPair.Device.PowerUsage;
                        mPair.Device.MiningHashrate = hashrates[mPair.Device.ID];
                        mPair.Device.MiningHashrateSecond = hashratesZIL[mPair.Device.ID];

                        //duals
                        if (MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash ||
                            MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium ||
                            MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
                        {
                            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos ||
                                MiningSetup.CurrentAlgorithmType == AlgorithmType.Octopus ||
                                MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto ||
                                MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash ||
                                MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
                            {
                                mPair.Device.MiningHashrate = hashrates[mPair.Device.ID];
                                mPair.Device.MiningHashrateSecond = hashrates2[mPair.Device.ID];
                                mPair.Device.MiningHashrateThird = 0;
                                mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                                mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                        }

                        if (MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NONE)//single
                        {
                            if (isZILround)
                            {
                                mPair.Device.MiningHashrate = 0;
                                mPair.Device.MiningHashrateThird = 0;
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
                        } else //dual
                        {
                            if (isZILround)
                            {
                                mPair.Device.MiningHashrate = 0;
                                mPair.Device.MiningHashrateThird = 0;
                                mPair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                                mPair.Device.SecondAlgorithmID = (int)AlgorithmType.DaggerHashimoto;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                            else
                            {
                                mPair.Device.MiningHashrateThird = 0;
                                mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                                mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                        }
                    }
                    
                }
                else
                {
                    Helpers.ConsolePrint("Rigel:", "resp - null");
                }
                _apiErrors = 0;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("Rigel API:", ex.ToString());
            }
            finally
            {
                ad.ZilRound = false;
                ad.Speed = total;
                ad.SecondarySpeed = total2;
                ad.ThirdSpeed = totalZIL;

                if (isZILround)
                {
                    if (MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NONE)//single+zil
                    {
                        ad.Speed = 0;
                        ad.SecondarySpeed = totalZIL;
                        ad.ThirdSpeed = 0;
                        ad.ZilRound = true;
                        ad.AlgorithmID = AlgorithmType.NONE;
                        ad.SecondaryAlgorithmID = AlgorithmType.DaggerHashimoto;
                        ad.ThirdAlgorithmID = AlgorithmType.NONE;
                    }
                    else
                    {
                        ad.Speed = 0;
                        ad.SecondarySpeed = 0;
                        ad.ThirdSpeed = totalZIL;
                        ad.ZilRound = true;
                        ad.AlgorithmID = AlgorithmType.NONE;
                        ad.SecondaryAlgorithmID = AlgorithmType.NONE;
                        ad.ThirdAlgorithmID = AlgorithmType.DaggerHashimoto;
                    }
                }
                else
                {
                    ad.ZilRound = false;
                    ad.ThirdSpeed = 0;
                    ad.ThirdAlgorithmID = AlgorithmType.NONE;

                    if (MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.NONE)//single no zil
                    {
                        ad.Speed = total;
                        ad.SecondarySpeed = 0;
                        ad.ThirdSpeed = 0;
                        ad.SecondaryAlgorithmID = AlgorithmType.NONE;
                        ad.ThirdAlgorithmID = AlgorithmType.NONE;
                    }
                    else
                    {

                    }

                }
                /*
                Helpers.ConsolePrint("Rigel->", MiningSetup.CurrentAlgorithmType.ToString() + ":" + ad.AlgorithmID.ToString() + " " +
                    MiningSetup.CurrentSecondaryAlgorithmType.ToString() + ":" + ad.SecondaryAlgorithmID.ToString() + " " +
                    ad.ThirdAlgorithmID.ToString() + " " +
                    ad.Speed.ToString() + " " +
                    ad.SecondarySpeed.ToString() + " " +
                    ad.ThirdSpeed.ToString() + " " +
                    isZILround.ToString());
                */
                if (ad.Speed == 0 && ad.SecondarySpeed == 0 && ad.ThirdSpeed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }
                else
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                    DeviceType devtype = DeviceType.NVIDIA;
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                    foreach (var mPair in sortedMinerPairs)
                    {
                        devtype = mPair.Device.DeviceType;
                    }
                }

            }

            Thread.Sleep(100);
            
            return ad;
        }
    }

}
