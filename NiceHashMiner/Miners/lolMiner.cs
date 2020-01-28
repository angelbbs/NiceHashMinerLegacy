/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
        int count = 0;
        double speed = 0;
        private int _benchmarkTimeWait = 240;

        public lolMiner()
            : base("lolMiner_AMD")
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
            url = url.Substring(0, url.IndexOf(":"));
            var apiBind = " --apiport " + ApiPort;
            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                LastCommandLine = "--coin AUTO144_5 --pool " + url + ";zhash." + myServers[1, 0] + nhsuff + ".nicehash.com;zhash." + myServers[2, 0] + nhsuff + ".nicehash.com;zhash." + myServers[3, 0] + nhsuff + ".nicehash.com;zhash." + myServers[4, 0] + nhsuff + ".nicehash.com;zhash." + myServers[5, 0] + nhsuff + ".nicehash.com" +
                              " --port " + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" +
                              " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" +
                              " -p x;x;x;x;x;x " + apiBind +
                              " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                LastCommandLine = "--coin BEAM-II --pool " + url + ";beam." + myServers[1, 0] + nhsuff + ".nicehash.com;beam." + myServers[2, 0] + nhsuff + ".nicehash.com;beam." + myServers[3, 0] + nhsuff + ".nicehash.com;beam." + myServers[4, 0] + nhsuff + ".nicehash.com;beam." + myServers[5, 0] + nhsuff + ".nicehash.com" +
                             " --port " + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" + port +
                             " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username +
                             " -p x;x;x;x;x;x --tls 0;0;0;0;0;0 " + apiBind +
                             " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                LastCommandLine = "--coin GRIN-AT31 --pool " + url + ";grincuckatoo31." + myServers[1, 0] + nhsuff + ".nicehash.com;grincuckatoo31." + myServers[2, 0] + nhsuff + ".nicehash.com;grincuckatoo31." + myServers[3, 0] + nhsuff + ".nicehash.com;grincuckatoo31." + myServers[4, 0] + nhsuff + ".nicehash.com;grincuckatoo31." + myServers[5, 0] + nhsuff + ".nicehash.com" +
                             " --port " + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" + port +
                             " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username +
                             " -p x;x;x;x;x;x --tls 0;0;0;0;0;0 " + apiBind +
                             " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                LastCommandLine = "--coin GRIN-C32 --pool " + url + ";grincuckatoo31." + myServers[1, 0] + nhsuff + ".nicehash.com;grincuckatoo31." + myServers[2, 0] + nhsuff + ".nicehash.com;grincuckatoo31." + myServers[3, 0] + nhsuff + ".nicehash.com;grincuckatoo31." + myServers[4, 0] + nhsuff + ".nicehash.com;grincuckatoo31." + myServers[5, 0] + nhsuff + ".nicehash.com" +
                             " --port " + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" + port +
                             " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username +
                             " -p x;x;x;x;x;x --tls 0;0;0;0;0;0 " + apiBind +
                             " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                LastCommandLine = "--coin GRIN-AD29 --pool " + url + ";grincuckarood29." + myServers[1, 0] + nhsuff + ".nicehash.com;grincuckarood29." + myServers[2, 0] + nhsuff + ".nicehash.com;grincuckarood29." + myServers[3, 0] + nhsuff + ".nicehash.com;grincuckarood29." + myServers[4, 0] + nhsuff + ".nicehash.com;grincuckarood29." + myServers[5, 0] + nhsuff + ".nicehash.com" +
                             " --port " + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" + port +
                             " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username +
                             " -p x;x;x;x;x;x --tls 0;0;0;0;0;0 " + apiBind +
                             " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                LastCommandLine = "--coin GRIN-C29M --pool " + url + ";cuckaroom." + myServers[1, 0] + nhsuff + ".nicehash.com;cuckaroom." + myServers[2, 0] + nhsuff + ".nicehash.com;cuckaroom." + myServers[3, 0] + nhsuff + ".nicehash.com;cuckaroom." + myServers[4, 0] + nhsuff + ".nicehash.com;cuckaroom." + myServers[5, 0] + nhsuff + ".nicehash.com" +
                             " --port " + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" + port +
                             " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username +
                             " -p x;x;x;x;x;x --tls 0;0;0;0;0;0 " + apiBind +
                             " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }
            LastCommandLine += GetDevicesCommandString() + " ";//
            ProcessHandle = _Start();
        }

        // new decoupled benchmarking routines
        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
            //GetEnimeration();
            var CommandLine = "";

            string url = Globals.GetLocationUrl(algorithm.NiceHashID, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], this.ConectionType);
            string port = url.Substring(url.IndexOf(".com:") + 5, url.Length - url.IndexOf(".com:") - 5);
            // demo for benchmark
            var btcAddress = Globals.GetBitcoinUser();
            var worker = ConfigManager.GeneralConfig.WorkerName.Trim();
            string username = GetUsername(btcAddress, worker);

            string nhsuff = "";
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                CommandLine = "--coin BEAM-II " +
                " --pool beam-eu.sparkpool.com;beam-asia.sparkpool.com;beamv2.eu" + nhsuff + ".nicehash.com;beamv2.hk" + nhsuff + ".nicehash.com" +
                " --port 2222;12222;3378;3378" +
                " --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9." + worker + ";2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9." + worker + ";" + username + ";" + username +
                " --pass x;x;x;x --tls 1;1;0;0 " +
                                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                CommandLine = "--coin AUTO144_5 --overwritePersonal BgoldPoW" +
                " --pool europe.equihash-hub.miningpoolhub.com --port 20595 --user angelbbs.lol --pass x" +
                                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                CommandLine = "--coin GRIN-AT31 " +
                " --pool grin.sparkpool.com;grincuckatoo31.usa" + nhsuff + ".nicehash.com --port 6667;3372 --user angelbbs@mail.ru." + worker + ";"+username+ " --pass x;x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                CommandLine = "--coin GRIN-C32 " +
                " --pool grin.2miners.com;grincuckatoo32.usa" + nhsuff + ".nicehash.com --port 3030;3383 --user 2aHR0cHM6Ly9kZXBvc2l0Z3Jpbi5rdWNvaW4uY29tL2RlcG9zaXQvMTg2MTU0MTY0MA." + worker + ";" + username + " --pass x;x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                CommandLine = "--coin GRIN-AD29 " +
                " --pool grin.sparkpool.com;grincuckaroo29.usa" + nhsuff + ".nicehash.com --port 6666;3372 --user angelbbs@mail.ru." + worker + ";" + username + " --pass x;x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Cuckaroom)
            {
                CommandLine = "--coin GRIN-C29M " +
                " --pool grin.sparkpool.com;cuckaroom.usa" + nhsuff + ".nicehash.com --port 6666;3382 --user angelbbs@mail.ru." + worker + ";" + username + " --pass x;x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            CommandLine += GetDevicesCommandString(); //amd карты перечисляются первыми
            _benchmarkTimeWait = time;
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
                //BenchmarkProcessPath = EnimerationHandle.StartInfo.FileName;
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

            // Thread.Sleep(500);
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
                //                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " error: " + ex.Message);
                //PidData is NULL error: Ссылка на объект не указывает на экземпляр объекта. 
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
            
            // string output = benchmarkconfigHandle.StandardOutput.ReadToEnd();


            /*               
                        if (outdata.Contains("Setup Miner..."))
                        {

                        }
                        */
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
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.Uuid).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                // var id = mPair.Device.ID;
                //int id = mPair.Device.IDByBus + variables.mPairDeviceIDByBus_lolBeam;
                
                Helpers.ConsolePrint("lolMinerIndexing", "ID: " + mPair.Device.ID);
                Helpers.ConsolePrint("lolMinerIndexing", "IDbybus: " + mPair.Device.IDByBus);
                Helpers.ConsolePrint("lolMinerIndexing", "busid: " + mPair.Device.BusID);
                Helpers.ConsolePrint("lolMinerIndexing", "lol: " + mPair.Device.lolMinerBusID);
                
                //список карт выводить --devices 999
                double id = mPair.Device.lolMinerBusID;
                if (ConfigManager.GeneralConfig.lolMinerOldEnumeration)
                {
                    id = mPair.Device.ID;
                }
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
                Helpers.ConsolePrint("lolMinerIndexing", "ID: " + id );
                {
                    ids.Add(id.ToString());
                }

            }


            deviceStringCommand += string.Join(",", ids);

            return deviceStringCommand;
        }
        protected override bool BenchmarkParseLine(string outdata) {
            string hashSpeed = "";

            //Average speed (30s): 25.5 sol/s 
            //GPU 3: Share accepted (45 ms)
            //Average speed (30s): 0.13 g/s 
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.BeamV2)
            {
                if (outdata.Contains("Average speed (30s):"))
                {
                    int i = outdata.IndexOf("Average speed (30s):");
                    int k = outdata.IndexOf("sol/s");
                    hashSpeed = outdata.Substring(i + 21, k - i - 22).Trim();
                    try
                    { 
                        speed = speed + Double.Parse(hashSpeed, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    count++;
                }
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                if (outdata.Contains("Average speed (30s):"))
                {
                    int i = outdata.IndexOf("Average speed (30s):");
                    int k = outdata.IndexOf("sol/s");
                    hashSpeed = outdata.Substring(i + 21, k - i - 22).Trim();
                    try
                    {
                            speed = speed + Double.Parse(hashSpeed, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    count++;
                }
            }
            //Average speed (30s): 0.13 g/s 
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo31)
            {
                if (outdata.Contains("Average speed (30s):"))
                {
                    int i = outdata.IndexOf("Average speed (30s):");
                    int k = outdata.IndexOf("g/s");
                    hashSpeed = outdata.Substring(i + 21, k - i - 22).Trim();
                    try
                    {
                            speed = speed + Double.Parse(hashSpeed, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    count++;
                }
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckatoo32)
            {
                if (outdata.Contains("Average speed (30s):"))
                {
                    int i = outdata.IndexOf("Average speed (30s):");
                    int k = outdata.IndexOf("g/s");
                    hashSpeed = outdata.Substring(i + 21, k - i - 22).Trim();
                    try
                    {
                        speed = speed + Double.Parse(hashSpeed, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    count++;
                }
            }
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                if (outdata.Contains("Average speed (30s):"))
                {
                    int i = outdata.IndexOf("Average speed (30s):");
                    int k = outdata.IndexOf("g/s");
                    hashSpeed = outdata.Substring(i + 21, k - i - 22).Trim();
                    try
                    {
                            speed = speed + Double.Parse(hashSpeed, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                        return false;
                    }
                    count++;
                }
            }
            /*
            if ((outdata.Contains("Share accepted") && speed != 0 && count > 4) || (count > 8 && speed != 0))
            {
                BenchmarkAlgorithm.BenchmarkSpeed = speed / count;
                BenchmarkSignalFinnished = true;
                return true;
            }
            */
            if ((count > _benchmarkTimeWait / 30 && speed != 0))
            {
                BenchmarkAlgorithm.BenchmarkSpeed = speed / count;
                BenchmarkSignalFinnished = true;
                return true;
            }
            return false;

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
                ad.Speed = totals;
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
