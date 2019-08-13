﻿using System;
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

namespace NiceHashMiner.Miners
{
    class lolMiner : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        int count = 0;
        double speed = 0;

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
                nhsuff = "-new";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.ZHash)
            {
                LastCommandLine = "--coin AUTO144_5 --pool " + url + ";zhash.hk" + nhsuff + ".nicehash.com;zhash.jp" + nhsuff + ".nicehash.com;zhash.usa" + nhsuff + ".nicehash.com;zhash.in" + nhsuff + ".nicehash.com;zhash.br" + nhsuff + ".nicehash.com" +
                              " --port " + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" +
                              " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" +
                              " -p x;x;x;x;x;x " + apiBind +
                              " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                              " --devices ";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Beam)
            {
                LastCommandLine = "--coin BEAM --pool " + url + ";beam.hk" + nhsuff + ".nicehash.com;beam.jp" + nhsuff + ".nicehash.com;beam.usa" + nhsuff + ".nicehash.com;beam.in" + nhsuff + ".nicehash.com;beam.br" + nhsuff + ".nicehash.com" +
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
                LastCommandLine = "--coin GRIN-AT31 --pool " + url + ";grincuckatoo31.hk" + nhsuff + ".nicehash.com;grincuckatoo31.jp" + nhsuff + ".nicehash.com;grincuckatoo31.usa" + nhsuff + ".nicehash.com;grincuckatoo31.in" + nhsuff + ".nicehash.com;grincuckatoo31.br" + nhsuff + ".nicehash.com" +
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
                LastCommandLine = "--coin GRIN-AD29 --pool " + url + ";grincuckarood29.hk" + nhsuff + ".nicehash.com;grincuckarood29.jp" + nhsuff + ".nicehash.com;grincuckarood29.usa" + nhsuff + ".nicehash.com;grincuckarood29.in" + nhsuff + ".nicehash.com;grincuckarood29.br" + nhsuff + ".nicehash.com" +
                             " --port " + port + ";" + port + ";" + port + ";" + port + ";" + port + ";" + port +
                             " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username +
                             " -p x;x;x;x;x;x --tls 0;0;0;0;0;0 " + apiBind +
                             " " +
                             ExtraLaunchParametersParser.ParseForMiningSetup(
                                                               MiningSetup,
                                                               DeviceType.AMD) +
                             " --devices ";
            }

            LastCommandLine += GetDevicesCommandString() + " ";//amd карты перечисляются первыми
            ProcessHandle = _Start();
        }

        // new decoupled benchmarking routines
        #region Decoupled benchmarking routines

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
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
                nhsuff = "-new";
            }

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Beam)
            {
                CommandLine = "--coin BEAM " +
                " --pool beam-eu.sparkpool.com;beam-asia.sparkpool.com;beam.eu" + nhsuff + ".nicehash.com;beam.hk" + nhsuff + ".nicehash.com" +
                " --port 2222;12222;3370;3370" +
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
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.GrinCuckarood29)
            {
                CommandLine = "--coin GRIN-AD29 " +
                " --pool grin.sparkpool.com;grincuckaroo29.usa" + nhsuff + ".nicehash.com --port 6666;3372 --user angelbbs@mail.ru." + worker + ";" + username + " --pass x;x" +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                MiningSetup,
                                                DeviceType.AMD) +
                " --devices ";
            }
            CommandLine += GetDevicesCommandString(); //amd карты перечисляются первыми
           
            return CommandLine;

        }
        /*
        protected override string GetDevicesCommandString0()
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
*/
        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            var ids = new List<string>();
            var amdDeviceCount = ComputeDeviceManager.Query.AmdDevices.Count;
            var allDeviceCount = ComputeDeviceManager.Query.GpuCount;
            Helpers.ConsolePrint("lolMinerIndexing", $"Found {allDeviceCount} Total GPU devices");
            Helpers.ConsolePrint("lolMinerIndexing", $"Found {amdDeviceCount} AMD devices");
            //   var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.ID.ToString()).ToList();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
               // var id = mPair.Device.ID;
                int id = mPair.Device.IDByBus + variables.mPairDeviceIDByBus_lolBeam;
                if (ConfigManager.GeneralConfig.lolMinerOldEnumeration)
                    id = mPair.Device.ID;
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
        protected override bool BenchmarkParseLine(string outdata) {
            string hashSpeed = "";

            //Average speed (30s): 25.5 sol/s 
            //GPU 3: Share accepted (45 ms)
            //Average speed (30s): 0.13 g/s 
            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.Beam)
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
            if ((outdata.Contains("Share accepted") && speed != 0 && count > 4) || (count > 8 && speed != 0))
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
