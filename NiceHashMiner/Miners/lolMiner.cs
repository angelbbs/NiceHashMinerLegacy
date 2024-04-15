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
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static NiceHashMiner.Devices.ComputeDeviceManager;

namespace NiceHashMiner.Miners
{
    class lolMiner : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        private int _benchmarkTimeWait = 180;
        private double _power = 0.0d;
        double _powerUsage = 0;
        string platform = "";
        private int APIerrorsCount = 0;
        public lolMiner()
            : base("lolMiner")
        {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
            IsKillAllUsedMinerProcs = true;
            IsNeverHideMiningWindow = true;

        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000;
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }
        private string GetServer(string algo, string btcAdress, string worker, string port)
        {
            string ret = "";
            string ssl = "";
            string psw = "x";
            string _worker = "";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                ssl = "--tls on ";
            }
            else
            {
                port = "1" + port;
                ssl = "--tls off ";
            }
            if (worker != null)
            {
                _worker = " --worker " + worker;
            } else
            {
                _worker = "";
            }
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " --pool " + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 --user " +
                        //username.Split('.')[0] + " --pass " + psw + " --tls off ";
                        btcAdress + _worker + " --pass " + psw + " --tls off ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " --pool " + Links.CheckDNS("stratum." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " --user " + 
                        //username.Split('.')[0] + " --pass " + psw + " " + ssl;
                        btcAdress + _worker + " --pass " + psw + " " + ssl;
                }
            }
            return ret;
        }
        private string GetServerDual(string algo, string algo2, string algo2pool, string btcAdress, string worker, string port, string port2)
        {
            string ret = "";
            string ssl = "";
            string dualssl = "";
            string psw = "x";
            string _worker = "";
            string _dualworker = "";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL)
            {
                port = "4" + port;
                port2 = "4" + port;
                ssl = "--tls on ";
                dualssl = "--tls on ";
            }
            else
            {
                port = "1" + port;
                port2 = "1" + port;
                ssl = "--tls off ";
                dualssl = "--tls off ";
            }
            if (worker != null)
            {
                _worker = " --worker " + worker;
                _dualworker = " --dualworker " + worker;
            }
            else
            {
                _worker = "";
                _dualworker = "";
            }
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + " --pool " + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 --user " +
                        btcAdress + _worker + " --pass " + psw + " --tls off " +
                        " --dualmode " + algo2 + " --dualpool " + Links.CheckDNS(algo2pool + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 " + " --dualuser " +
                        btcAdress + _dualworker + " --dualpass " + psw + " --dualtls off ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + " --pool " + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " --user " +
                        btcAdress + _worker + " --pass " + psw + " " + ssl +
                        " --dualmode " + algo2 + " --dualpool " + Links.CheckDNS(algo2pool + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port2 + " --dualuser " +
                        btcAdress + _dualworker + " --dualpass " + psw + " " + dualssl;
                }
            }
            return ret;
        }
        public override void Start(string btcAdress, string worker)
        {
            string url = "";
            string username = GetUsername(btcAdress, worker);
            worker = worker + "$" + ConfigManager.GeneralConfig.MachineGuid;
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            var param = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = "nvidia";
                    param = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA).Trim();
                }
                if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    platform = "amd";
                    param = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).Trim();
                }
                if (pair.Device.DeviceType == DeviceType.INTEL)
                {
                    platform = "intel";
                    param = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.INTEL).Trim();
                }
            }
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.lolMiner;
            IsApiReadException = false;

            var apiBind = " --apiport " + ApiPort;

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                LastCommandLine = "--coin AUTO144_5" +
                    GetServer("zhash", username, null, "3369") +
                    apiBind + " " + param +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZelHash)
            {
                LastCommandLine = "--coin ZEL" +
                    GetServer("zelhash", username, null, "3391") +
                    apiBind + " " + param +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                LastCommandLine = "--algo BEAM-III" +
                GetServer("beamv3", username, null, "3387") +
                    apiBind + " " + param +
                              " --devices ";
            }

            
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                LastCommandLine = "--algo C29AE" +
                GetServer("cuckoocycle", username, null, "3376") +
                    apiBind + " " + param +
                              " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                LastCommandLine = "--algo C32" +
                GetServer("grincuckatoo32", username, null, "3383") +
                    apiBind + " " + param +
                              " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                LastCommandLine = "--algo ETHASH --ethstratum=ETHV1" + 
                //LastCommandLine = "--algo ETHASH --ethstratum=ETHV1" + " " +
                GetServer("daggerhashimoto", username, worker, "3353") +
                    apiBind + " " + param +
                              " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
            {
                LastCommandLine = "--algo ETCHASH --ethstratum=ETHV1" +
                GetServer("etchash", username, worker, "3393") +
                    apiBind + " " + param +
                              " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos)
            {
                LastCommandLine = "--algo AUTOLYKOS2" +
                GetServer("autolykos", username, null, "3390") +
                    apiBind + " " + param +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.NexaPow)
            {
                LastCommandLine = "--algo NEXA" +
                GetServer("nexapow", username, null, "3396") +
                    apiBind + " " + param +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash)
            {
                LastCommandLine = "--algo FISHHASH" +
                GetServer("fishhash", username, null, "3400") +
                    apiBind + " " + param +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.PyrinHash)
            {
                LastCommandLine = "--algo PYRIN" +
                GetServer("pyrinhash", username, null, "3401") +
                    apiBind + " " + param +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KarlsenHash)
            {
                LastCommandLine = "--algo KARLSEN" +
                GetServer("karlsenhash", username, null, "3398") +
                    apiBind + " " + param +
                              " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Alephium)
            {
                LastCommandLine = "--algo ALEPH" +
                GetServer("alephium", username, null, "3399") +
                    apiBind + " " + param +
                              " --devices ";
            }
            //duals
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
            {
                LastCommandLine = "--algo FISHHASH" +
                GetServerDual("fishhash", "ALEPHDUAL", "alephium", username, worker, "3400", "3399") +
                    apiBind + " " + param +
                              " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
            {
                LastCommandLine = "--algo FISHHASH" +
                GetServerDual("fishhash", "KARLSENDUAL", "karlsenhash", username, worker, "3400", "3398") +
                    apiBind + " " + param +
                              " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
            {
                LastCommandLine = "--algo FISHHASH" +
                GetServerDual("fishhash", "PYRINDUAL", "pyrinhash", username, worker, "3401", "3398") +
                    apiBind + " " + param +
                              " --devices ";
            }


            LastCommandLine += GetDevicesCommandString() + " ";//
            //LastCommandLine = LastCommandLine.Replace("--asm 1", "");
            string sColor = "";
            if (Form_Main.GetWinVer(Environment.OSVersion.Version) < 8)
            {
                sColor = " --nocolor";
            }
            LastCommandLine += sColor;
            ProcessHandle = _Start();
        }

        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            var apiBind = " --apiport " + ApiPort;
            var CommandLine = "";
            var param = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = "nvidia";
                    param = " " + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA).Trim();
                }
                if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    platform = "amd";
                    param = " " + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).Trim();
                }
                if (pair.Device.DeviceType == DeviceType.INTEL)
                {
                    platform = "intel";
                    param = " " + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.INTEL).Trim();
                }
            }
            // demo for benchmark
            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV3)
            {
                CommandLine = "--algo BEAM-III " +
                " --pool " + Links.CheckDNS("stratum+tcp://beam.2miners.com:5252").Replace("stratum+tcp://", "") + " --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9.lolMiner --pass x" +
                param +
                " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                CommandLine = "--algo EQUI144_5 --pers BgoldPoW" +
                " --pool " + Links.CheckDNS("stratum+tcp://btg.2miners.com:4040").Replace("stratum+tcp://", "") + " --user GeKYDPRcemA3z9okSUhe9DdLQ7CRhsDBgX.lol --pass x" +
                                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZelHash)
            {
                CommandLine = "--coin ZEL" +
                " --pool " + Links.CheckDNS("stratum+tcp://flux.2miners.com:9090").Replace("stratum+tcp://", "") + " --user t1RyEzV5eAo95LbQiLZfzmGZGK9vTkdeBDd.lol --pass x" +
                                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.CuckooCycle)
            {
                CommandLine = "--algo C29AE " +
                " --pool " + Links.CheckDNS("stratum+tcp://ae.2miners.com:4040").Replace("stratum+tcp://", "") + " --user ak_25J5KBhdHcsemmgmnaU4QpcRQ9xgKS5ChBwCaZcEUc85qkgcXE.lolMiner --pass x" +
                " --pool " + Links.CheckDNS("stratum+tcp://cuckoocycle.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " --user " + username + " --pass x" +
                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                CommandLine = "--algo C32 " +
                " --pool " + Links.CheckDNS("stratum+tcp://grin.2miners.com:3030").Replace("stratum+tcp://", "") + " --user grin16ek8qgx29ssku0q2cxez7830gh9ndw3ek5yzxe26x34s09528d2sldl6td.lolMiner --pass x" +
                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                CommandLine = "--algo ETHASH " +
                " --pool " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020").Replace("stratum+tcp://", "") + " --user 0x266b27bd794d1A65ab76842ED85B067B415CD505.lolMiner --pass x" +
                " --pool " + Links.CheckDNS("stratum+tcp://daggerhashimoto.auto.nicehash.com:9200").Replace("stratum+tcp://", "") + " --user " + username + " --pass x" +
                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ETCHash)
            {
                CommandLine = "--algo ETCHASH " +
                " --pool " + Links.CheckDNS("stratum+tcp://etc.2miners.com:1010").Replace("stratum+tcp://", "") + " --user 0x266b27bd794d1A65ab76842ED85B067B415CD505.lolMiner --pass x" +
                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Autolykos)
            {
                CommandLine = "--algo AUTOLYKOS2 " +
                " --pool " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3100").Replace("stratum+tcp://", "") + " --user 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.lolMiner --pass x" +
                              param +
                " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.NexaPow)
            {
                CommandLine = "--algo NEXA " +
                " --pool " + Links.CheckDNS("stratum-eu.rplant.xyz:7092").Replace("stratum+tcp://", "") + " --user nexa:nqtsq5g55l2jhuazhre8zfzfnyxle543wjlapt4huup3x9gy.lolMiner --pass x" +
                              param +
                " --devices ";
            }
            /*
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.IronFish)
            {
                CommandLine = "--algo IRONFISH " +
                " --pool " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --user fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.lolMiner --pass x" +
                              param +
                " --devices ";
            }
            */
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash)
            {
                CommandLine = "--algo FISHHASH " +
                " --pool " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --user fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.lolMiner --pass x" +
                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KarlsenHash)
            {
                CommandLine = "--algo KARLSEN " +
                " --pool " + Links.CheckDNS("kls.2miners.com:2020").Replace("stratum+tcp://", "") + " --user karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.lolMiner --pass x" +
                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Alephium)
            {
                CommandLine = "--algo ALEPH " +
                " --pool " + Links.CheckDNS("ru.alephium.herominers.com:1199").Replace("stratum+tcp://", "") + " --user 12bjcHBTbdqW3zfDc84qq8z6RNZr33oXgqqaYdZRUD5qC.lolMiner --pass x" +
                              param +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.PyrinHash)
            {
                CommandLine = "--algo PYRIN " +
                " --pool " + Links.CheckDNS("pyi.2miners.com:2121").Replace("stratum+tcp://", "") + " --user pyrin:qzhy95jlwufjp7q8exs5vwzzzru74xgl6sedz2c57t7q2w9lvac0u9es2rt5y.lolMiner --pass x" +
                              param +
                " --devices ";
            }
            //duals
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.Alephium)
            {
                CommandLine = "--algo FISHHASH " +
                                " --pool " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --user fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.lolMiner --pass x" +
                                " --dualmode ALEPHDUAL --dualpool " + Links.CheckDNS("ru.alephium.herominers.com:1199").Replace("stratum + tcp://", "") + " --dualuser 12bjcHBTbdqW3zfDc84qq8z6RNZr33oXgqqaYdZRUD5qC.lolMiner --dualpass x" +
                                              param +
                                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.KarlsenHash)
            {
                CommandLine = "--algo FISHHASH " +
                                " --pool " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --user fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.lolMiner --pass x" +
                                " --dualmode KARLSENDUAL --dualpool " + Links.CheckDNS("kls.2miners.com:2020").Replace("stratum + tcp://", "") + " --dualuser karlsen:qrnsjf7ka334kx0rlgfxxvqf04c9qthdltfj7q7amm6nqvmqz9csunnazj64s.lolMiner --dualpass x" +
                                              param +
                                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.FishHash && MiningSetup.CurrentSecondaryAlgorithmType == AlgorithmType.PyrinHash)
            {
                CommandLine = "--algo FISHHASH " +
                                " --pool " + Links.CheckDNS("ru.ironfish.herominers.com:1145").Replace("stratum+tcp://", "") + " --user fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.lolMiner --pass x" +
                                " --dualmode PYRINDUAL --dualpool " + Links.CheckDNS("pyi.2miners.com:2121").Replace("stratum + tcp://", "") + " --dualuser pyrin:qzhy95jlwufjp7q8exs5vwzzzru74xgl6sedz2c57t7q2w9lvac0u9es2rt5y.lolMiner --dualpass x" +
                                              param +
                                " --devices ";
            }

            CommandLine += GetDevicesCommandString() + " "; //amd карты перечисляются первыми
            _benchmarkTimeWait = time;
            //CommandLine = CommandLine.Replace("--asm 1", "");
            string sColor = "";
            //if (GetWinVer(Environment.OSVersion.Version) < 8)
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
            catch (Exception)
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
            var intelDeviceCount = ComputeDeviceManager.Query.IntelDevices.Count;
            var allDeviceCount = ComputeDeviceManager.Query.GpuCount;
            Helpers.ConsolePrint("lolMinerIndexing", $"Found {allDeviceCount} Total GPU devices");
            Helpers.ConsolePrint("lolMinerIndexing", $"Found {amdDeviceCount} AMD devices");
            Helpers.ConsolePrint("lolMinerIndexing", $"Found {intelDeviceCount} INTEL devices");
            //   var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.ID.ToString()).ToList();
            //var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.lolMinerBusID).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                //список карт выводить --devices 999
                //double id = mPair.Device.IDByBus + allDeviceCount - amdDeviceCount;
                int id = (int)mPair.Device.lolMinerBusID;

                if (id < 0)
                {
                    Helpers.ConsolePrint("lolMinerIndexing", "ID too low: " + id + " skipping device");
                    continue;
                }
                /*
                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    Helpers.ConsolePrint("lolMinerIndexing", "NVIDIA found. Increasing index");
                    id ++;
                }
                */
                Helpers.ConsolePrint("lolMinerIndexing", "Minind ID: " + id);
                {
                    ids.Add(id.ToString());
                }

            }


            deviceStringCommand += string.Join(",", ids);

            return deviceStringCommand + " --watchdog exit ";
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
            BenchmarkSignalTimedout = false;
            double repeats = 0.0d;
            double summspeed = 0.0d;
            double secsummspeed = 0.0d;

            int delay_before_calc_hashrate = 15;
            int MinerStartDelay = 10;

            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = "nvidia";
                }
                if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    platform = "amd";
                }
                if (pair.Device.DeviceType == DeviceType.INTEL)
                {
                    platform = "intel";
                }
            }

            //уменьшим время на бенчмарк

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 45;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 45;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 45;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ZHash))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 30;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ZelHash))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 30;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 15;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 15;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.NexaPow))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 15;
                }
                /*
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.IronFish))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 15;
                }
                */
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.FishHash))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 10;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.FishHash) && platform == "amd")
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 80;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KarlsenHash))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 15;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.PyrinHash))
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 15;
                }
                /*
                if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.KHeavyHash))//+dual
                {
                    _benchmarkTimeWait = _benchmarkTimeWait + 60;
                }
                */
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
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
                    {
                        delay_before_calc_hashrate = 60;
                        MinerStartDelay = 20;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                    {
                        delay_before_calc_hashrate = 40;
                        MinerStartDelay = 20;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ZHash))
                    {
                        delay_before_calc_hashrate = 40;
                        MinerStartDelay = 20;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ZelHash))
                    {
                        delay_before_calc_hashrate = 40;
                        MinerStartDelay = 20;
                    }
                    
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.BeamV3))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 20;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.NexaPow))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }
                    /*
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.IronFish))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }
                    */
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.FishHash))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.FishHash) && platform == "amd")
                    {
                        delay_before_calc_hashrate = 90;
                        MinerStartDelay = 30;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KarlsenHash))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.PyrinHash))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Alephium))
                    {
                        delay_before_calc_hashrate = 10;
                        MinerStartDelay = 10;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CuckooCycle))
                    {
                        delay_before_calc_hashrate = 40;
                        MinerStartDelay = 5;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.GrinCuckatoo32))
                    {
                        delay_before_calc_hashrate = 40;
                        MinerStartDelay = 5;
                    }
                    /*
                    if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.KHeavyHash))//dual
                    {
                        delay_before_calc_hashrate = 70;
                        MinerStartDelay = 30;
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
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() +
                                "  Second: " + ad.Result.SecondarySpeed.ToString() +
                                " power: " + _power.ToString() + " after " + repeats.ToString() + " sec");
                            /*
                            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                            {
                                summspeed = Math.Max(summspeed, ad.Result.Speed);
                            }
                            else
                            {
                                summspeed += ad.Result.Speed;
                            }
                            */
                            summspeed += ad.Result.Speed;
                            secsummspeed += ad.Result.SecondarySpeed;
                        }
                        else
                        {
                            Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString() +
                                "  Second: " + ad.Result.SecondarySpeed.ToString() +
                                " power: " + _power.ToString());
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
                BenchmarkAlgorithm.BenchmarkSecondarySpeed = Math.Round(secsummspeed / (repeats - delay_before_calc_hashrate), 2);
                BenchmarkAlgorithm.PowerUsageBenchmark = (_powerUsage / repeats);
                /*
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = summspeed;
                }
                else
                {
                    BenchmarkAlgorithm.BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
                    BenchmarkAlgorithm.PowerUsageBenchmark = (_powerUsage / repeats);
                }
                */
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

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }

        private int[] errors = new int[0];
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO; 
            string ResponseFromlolMiner;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/summary");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 3 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 2 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromlolMiner = await Reader.ReadToEndAsync();
                //Helpers.ConsolePrint("API: ", ResponseFromlolMiner);
                //if (ResponseFromlolMiner.Length == 0 || (ResponseFromlolMiner[0] != '{' && ResponseFromlolMiner[0] != '['))
                //    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception)
            {
                return null;
            }

            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType, MiningSetup.MiningPairs[0]);

            if (ResponseFromlolMiner == null)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.NONE;
                return null;
            }
            //CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            try
            {
                dynamic resp = JsonConvert.DeserializeObject(ResponseFromlolMiner.Replace("\"-nan(ind)\"", "0.0"));
                //Helpers.ConsolePrint("->", ResponseFromlolMiner);
                int mult = 1;
                if (resp != null)
                {
                    int Num_Workers = resp.Num_Workers;
                    if (errors.Length != Num_Workers)
                    {
                        Array.Resize<int>(ref errors, Num_Workers);
                    }
                    if (Num_Workers == 0) return null;
                    int Num_Algorithms = resp.Num_Algorithms;
                    //Helpers.ConsolePrint("API: ", "Num_Workers: " + Num_Workers.ToString());
                    //Helpers.ConsolePrint("API: ", "Num_Algorithms: " + Num_Algorithms.ToString());
                    double[] Total_Performance = new double[Num_Algorithms];
                    double[] Total_Performance2 = new double[Num_Algorithms];
                    double[] hashrates = new double[Num_Workers];
                    double[] hashrates2 = new double[Num_Workers];
                    double totals = 0.0d;
                    double totals2 = 0.0d;
                    if (Num_Algorithms == 1)//single
                    {
                        for (int alg = 0; alg < Num_Algorithms; alg++)
                        {
                            string Algorithm = resp.Algorithms[alg].Algorithm;
                            Total_Performance[alg] = resp.Algorithms[alg].Total_Performance * resp.Algorithms[alg].Performance_Factor;
                            //Helpers.ConsolePrint("API: ", "Algorithm: " + resp.Algorithms[alg].Algorithm);
                            //Helpers.ConsolePrint("API: ", "Total_Performance: " + Total_Performance[alg].ToString());
                            totals = Total_Performance[alg];
                            for (int w = 0; w < Num_Workers; w++)
                            {
                                hashrates[w] = resp.Algorithms[alg].Worker_Performance[w] * resp.Algorithms[alg].Performance_Factor;
                                //Helpers.ConsolePrint("API: ", "hashrates: " + hashrates[w].ToString());
                            }
                        }
                        //ad.SecondaryAlgorithmID = AlgorithmType.NONE;
                    }
                    else //duals
                    {
                        for (int alg = 0; alg < Num_Algorithms; alg++)
                        {
                            string Algorithm = resp.Algorithms[alg].Algorithm;
                            if (Algorithm.Contains("FishHash"))
                            {
                                Total_Performance[alg] = resp.Algorithms[alg].Total_Performance * resp.Algorithms[alg].Performance_Factor;
                                totals = Total_Performance[alg];
                                for (int w = 0; w < Num_Workers; w++)
                                {
                                    hashrates[w] = resp.Algorithms[alg].Worker_Performance[w] * resp.Algorithms[alg].Performance_Factor;
                                }
                            }
                            if (Algorithm.Contains("Blake3-Alephium"))
                            {
                                Total_Performance2[alg] = resp.Algorithms[alg].Total_Performance * resp.Algorithms[alg].Performance_Factor;
                                totals2 = Total_Performance2[alg];
                                for (int w = 0; w < Num_Workers; w++)
                                {
                                    hashrates2[w] = resp.Algorithms[alg].Worker_Performance[w] * resp.Algorithms[alg].Performance_Factor;
                                }
                                ad.SecondaryAlgorithmID = AlgorithmType.Alephium;
                            }
                            if (Algorithm.Contains("HeavyHash-Karlsen"))
                            {
                                Total_Performance2[alg] = resp.Algorithms[alg].Total_Performance * resp.Algorithms[alg].Performance_Factor;
                                totals2 = Total_Performance2[alg];
                                for (int w = 0; w < Num_Workers; w++)
                                {
                                    hashrates2[w] = resp.Algorithms[alg].Worker_Performance[w] * resp.Algorithms[alg].Performance_Factor;
                                }
                                ad.SecondaryAlgorithmID = AlgorithmType.KarlsenHash;
                            }
                            if (Algorithm.Contains("HeavyHash-Pyrin"))
                            {
                                Total_Performance2[alg] = resp.Algorithms[alg].Total_Performance * resp.Algorithms[alg].Performance_Factor;
                                totals2 = Total_Performance2[alg];
                                for (int w = 0; w < Num_Workers; w++)
                                {
                                    hashrates2[w] = resp.Algorithms[alg].Worker_Performance[w] * resp.Algorithms[alg].Performance_Factor;
                                }
                                ad.SecondaryAlgorithmID = AlgorithmType.PyrinHash;
                            }
                        }
                        ad.ThirdAlgorithmID = AlgorithmType.NONE;
                    }

                    ad.Speed = totals;
                    ad.SecondarySpeed = totals2;
                    if (Num_Workers > 0)
                    {
                        int dev = 0;
                        var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
                        if (Form_Main.NVIDIA_orderBug)
                        {
                            sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
                        }
                        foreach (var mPair in sortedMinerPairs)
                        {
                            _power = mPair.Device.PowerUsage;
                            mPair.Device.MiningHashrate = hashrates[dev];
                            mPair.Device.MiningHashrateSecond = hashrates2[dev];

                            if (Num_Algorithms == 1 && hashrates[dev] == 0)
                            {
                                //CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                                //Helpers.ConsolePrint("lolMiner API", "Device " + dev.ToString() + " zero hashrate");
                                errors[dev]++;
                            }
                            if (Num_Algorithms == 2 && hashrates[dev] == 0)
                            {
                                //CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                                //Helpers.ConsolePrint("lolMiner API", "Device " + dev.ToString() + " zero main hashrate");
                                errors[dev]++;
                            }
                            if (Num_Algorithms == 2 && hashrates2[dev] == 0)
                            {
                                //CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                                //Helpers.ConsolePrint("lolMiner API", "Device " + dev.ToString() + " zero second hashrate");
                                errors[dev]++;
                            }

                            if (Num_Algorithms == 1 && hashrates[dev] != 0)
                            {
                                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                                errors[dev] = 0;
                            }
                            if (Num_Algorithms == 2 && hashrates[dev] != 0 && hashrates2[dev] != 0)
                            {
                                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                                errors[dev] = 0;
                            }
                            dev++;
                        }  
                    }

                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint(MinerTag(), e.ToString());
            }

            foreach (var d in errors)
            {
                if (d >= 100)
                {
                    Helpers.ConsolePrint(MinerTag(), "Too many API errors. Need Restarting miner");
                    CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                    ad.Speed = 0;
                    ad.SecondarySpeed = 0;
                    ad.ThirdSpeed = 0;
                    return ad;
                }
            }

            Thread.Sleep(100);
            return ad;
        }
    }
}
