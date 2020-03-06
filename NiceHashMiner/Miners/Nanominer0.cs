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

namespace NiceHashMiner.Miners
{
    public class Nanominer : Miner
    {
        private int _benchmarkTimeWait = 120;
        private int _benchmarkReadCount = 0;
        private double _benchmarkSum;
        private DateTime _started;
        private bool firstStart = true;
        private string[,] myServers = Form_Main.myServers;
        string ResponseFromNanominer;

        public Nanominer() : base("Nanominer")
        {
            ConectionType = NhmConectionType.NONE;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            IsApiReadException = false;
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
        }

        private string GetStartCommand(string url, string btcAdress, string worker)
        {
            if (File.Exists("bin_3rdparty\\Nanominer\\config_nh.ini"))
                File.Delete("bin_3rdparty\\Nanominer\\config_nh.ini");

            string username = GetUsername(btcAdress, worker);
            string rigName = username.Split('.')[1];

            var cfgFile = String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
               + String.Format("webPort = {0}", ApiPort) + "\n"
               + String.Format("protocol = stratum\n")
               + String.Format("[Ethash]\n")
               + String.Format("wallet = {0}", btcAdress) + "\n"
               + String.Format("rigName = {0}", rigName) + "\n"
               + String.Format("pool1 = daggerhashimoto.{0}.nicehash.com:3353", myServers[0, 0]) + "\n"
               + String.Format("pool2 = daggerhashimoto.{0}.nicehash.com:3353", myServers[1, 0]) + "\n"
               + String.Format("pool3 = daggerhashimoto.{0}.nicehash.com:3353", myServers[2, 0]) + "\n"
               + String.Format("pool4 = daggerhashimoto.{0}.nicehash.com:3353", myServers[3, 0]) + "\n"
               + String.Format("pool5 = daggerhashimoto.{0}.nicehash.com:3353", myServers[4, 0]) + "\n"
               + String.Format("pool6 = daggerhashimoto.{0}.nicehash.com:3353", myServers[5, 0]) + "\n";


            try
            {
                FileStream fs = new FileStream("bin_3rdparty\\Nanominer\\config_nh.ini", FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(fs);
                w.WriteAsync(cfgFile);
                w.Flush();
                w.Close();
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("GetStartCommand", e.ToString());
            }

            return " config_nh.ini" +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD) +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
        }


        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            var ids = MiningSetup.MiningPairs.Select(mPair => (mPair.Device.lolMinerBusID).ToString()).ToList();
            deviceStringCommand += string.Join(",", ids);
            return deviceStringCommand;
        }


        // benchmark stuff
        protected void KillMinerBase(string exeName)
        {
            foreach (var process in Process.GetProcessesByName(exeName))
            {
                try { process.Kill(); }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }
            }
        }

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
                Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], ConectionType).Replace("stratum+tcp://", "");
            var username = Globals.GetBitcoinUser();
            var rigName = ConfigManager.GeneralConfig.WorkerName.Trim();

            if (File.Exists("bin_3rdparty\\Nanominer\\config_nh.ini"))
                File.Delete("bin_3rdparty\\Nanominer\\config_nh.ini");

            var platform = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = "nvidia";
                }
                else
                {
                    platform = "amd";
                }
            }
            if (File.Exists("bin_3rdparty\\Nanominer\\." + platform + GetLogFileName()))
                File.Delete("bin_3rdparty\\Nanominer\\." + platform + GetLogFileName());


            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                var cfgFile = String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("logPath = {0}", platform + GetLogFileName()) + "\n"
                   + String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("protocol = stratum\n")
                   + String.Format("[Ethash]\n")
                   + String.Format("wallet = 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1") + "\n"
                   + String.Format("rigName = Nanominer") + "\n"
                   + String.Format("pool1 = eu1.ethermine.org:4444") + "\n";

                try
                {
                    FileStream fs = new FileStream("bin_3rdparty\\Nanominer\\config_nh.ini", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.WriteAsync(cfgFile);
                    w.Flush();
                    w.Close();
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("GetStartCommand", e.ToString());
                }

                _benchmarkTimeWait = time;
            }
            return " config_nh.ini" +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD) +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);

        }
        /*
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
                        KillMinerBase(imageName);
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
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {
                BenchmarkAlgorithm.BenchmarkSpeed = 0;
                // find latest log file
                var platform = "";
                string logfile;
                foreach (var pair in MiningSetup.MiningPairs)
                {
                    if (pair.Device.DeviceType == DeviceType.NVIDIA)
                    {
                        platform = "nvidia";
                    }
                    else
                    {
                        platform = "amd";
                    }
                }
                logfile = WorkingDirectory + "." + platform + GetLogFileName();
                // read file log
                Helpers.ConsolePrint(MinerTag(), logfile);
                if (File.Exists(logfile))
                {
                    //var lines = new string[0];
                    string lines = "";
                   // var read = false;
                    var iteration = 0;
                   // while (!read)
                    {
                        //if (iteration < 10 )
                        {
                            try
                            {
                                //lines = File.ReadAllLines(logfile);
                                using (var myStream = File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    StreamReader reader = new StreamReader(myStream);
                               // }
                               // using (StreamReader reader = new StreamReader(logfile, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                                //{
                                    string line;
                                    lines = reader.ReadToEnd();

                                    //string[] lines = new string[]; // 20 is the amount of lines

                                }
                                //read = true;
                                Helpers.ConsolePrint(MinerTag(),
                                    "Successfully read log");
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint(MinerTag(), ex.Message);
                                Thread.Sleep(200);
                            }

                            iteration++;
                        }

                    }

                    var addBenchLines = BenchLines.Count == 0;


                    if (_benchmarkReadCount > 0)
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = _benchmarkSum / (_benchmarkReadCount-10);
                    }
                }

                BenchmarkThreadRoutineFinish();
            }
        }
        */
        // stub benchmarks read from file
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            Helpers.ConsolePrint("BENCHMARK1", outdata);
            CheckOutdata(outdata);
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            Helpers.ConsolePrint("BENCHMARK", outdata);
            return false;
        }

        protected double GetNumber(string outdata)
        {
            return GetNumber(outdata, "Total speed: ", "/s, Total shares");
        }

        protected double GetNumber(string outdata, string lookForStart, string lookForEnd)
        {
            try
            {
                double mult = 1;
                var speedStart = outdata.IndexOf(lookForStart.ToLower());
                var speed = outdata.Substring(speedStart, outdata.Length - speedStart);
                speed = speed.Replace(lookForStart.ToLower(), "");
                speed = speed.Substring(0, speed.IndexOf(lookForEnd.ToLower()));

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

                //Helpers.ConsolePrint("speed", speed);
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

        public override async Task<ApiData> GetSummaryAsync()
        {
            // CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
            CurrentMinerReadStatus = MinerApiReadStatus.WAIT;

            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/stats");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromNanominer = await Reader.ReadToEndAsync();
                Reader.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API", ex.Message);
                return null;
            }

            // Helpers.ConsolePrint("API", ResponseFromNanominer);
            dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer);
            var cSpeed1 = (json.Algorithms[0].Ethash);
            if (cSpeed1 == null) return ad;
            var cSpeed = (json.Algorithms[0].Ethash.Total.Hashrate);
            // Helpers.ConsolePrint("API", cSpeed.ToString());
            var dSpeed = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);

            ad.Speed = dSpeed;
            if (ad.Speed == 0)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            }
            else
            {
                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            }

            //Thread.Sleep(1000);
            return ad;

        }


        protected override void _Stop(MinerStopType willswitch)
        {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5; // 5 minute max, whole waiting time 75seconds
        }
    }
}
