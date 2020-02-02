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

namespace NiceHashMiner.Miners
{
    class lolMinerBEAM : Miner
    {
        private readonly int GPUPlatformNumber;
        Stopwatch _benchmarkTimer = new Stopwatch();
        int count = 0;
        double speed = 0;
        private string[,] myServers = Form_Main.myServers;
        public lolMinerBEAM()
            : base("lolMinerBEAM")
        {
            //GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
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
            LastCommandLine = "--coin BEAM --pool " + url + ";beam." + myServers[1, 0] + nhsuff + ".nicehash.com;beam." + myServers[2, 0] + nhsuff + ".nicehash.com;beam." + myServers[3, 0] + nhsuff + ".nicehash.com;beam." + myServers[4, 0] + nhsuff + ".nicehash.com;beam." + myServers[5, 0] + nhsuff + ".nicehash.com" +
                              " --port " + port+";"+port +";"+ port+";"+ port+";"+ port+";"+ port+
                              " --user " + username + ";" + username + ";" + username + ";" + username + ";" + username + ";" + username +
                              " -p x;x;x;x;x;x --tls 0;0;0;0;0;0 " + apiBind +
                              " " +
                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                              " --devices ";

            LastCommandLine += GetDevicesCommandString();//amd карты перечисляются первыми
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
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            CommandLine = "--coin BEAM " +
                " --pool beam-eu.sparkpool.com;beam-asia.sparkpool.com;beam.eu" + nhsuff + ".nicehash.com;beam.hk" + nhsuff + ".nicehash.com" +
                " --port 2222;12222;3370;3370" +
                " --user 2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9."+ worker+ ";2c20485d95e81037ec2d0312b000b922f444c650496d600d64b256bdafa362bafc9." + worker + ";" + username+";"+username+
                " --pass x;x;x;x --tls 1;1;0;0 " +
                                              ExtraLaunchParametersParser.ParseForMiningSetup(
                                                                MiningSetup,
                                                                DeviceType.AMD) +
                " --devices ";

            CommandLine += GetDevicesCommandString(); //amd карты перечисляются первыми

            return CommandLine;

        }
        
       
        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            var ids = new List<string>();
            var amdDeviceCount = ComputeDeviceManager.Query.AmdDevices.Count;
            var allDeviceCount = ComputeDeviceManager.Query.GpuCount;
            Helpers.ConsolePrint("lolMinerBEAMIndexing", $"Found {allDeviceCount} Total GPU devices");
            Helpers.ConsolePrint("lolMinerBEAMIndexing", $"Found {amdDeviceCount} AMD devices");
            //   var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.ID.ToString()).ToList();
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.DeviceType).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                int id = mPair.Device.IDByBus + variables.mPairDeviceIDByBus_lolBeam;
                if (ConfigManager.GeneralConfig.lolMinerOldEnumeration)
                    id = mPair.Device.ID;
                if (id < 0)
                {
                    Helpers.ConsolePrint("lolMinerBEAMIndexing", "ID too low: " + id + " skipping device");
                    continue;
                }

                if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    Helpers.ConsolePrint("lolMinerBEAMIndexing", "NVIDIA found. Increasing index");
                    id ++;
                }
                Helpers.ConsolePrint("lolMinerBEAMIndexing", "ID: " + id );
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

            if ( (outdata.Contains("Share accepted") && speed != 0 && count > 4) || (count > 8 && speed != 0) )
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
