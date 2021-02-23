using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System.Threading.Tasks;
using System.Threading;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Switching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMinerLegacy.Common.Enums;
using System.Linq;

namespace NiceHashMiner.Miners
{
    public class lyclMiner : Miner
    {
        private int st = 0;
        private double speed = 0.0d;
        private string hashspeed = "";
        int iter = 0;

        public lyclMiner() : base("lyclMiner") { }

        bool benchmarkException {
            get {
                return MiningSetup.MinerPath == MinerPaths.Data.lyclMiner;
            }
        }

        protected override int GetMaxCooldownTimeInMilliseconds() {
            return 60 * 1000 * 12;
        }


        public override void Start(string url, string btcAdress, string worker)
        {
            if (!IsInit)
            {
                Helpers.ConsolePrint(MinerTag(), "MiningSetup is not initialized exiting Start()");
                return;
            }
            string username = GetUsername(btcAdress, worker);

            IsApiReadException = true; //** in miner

            if (File.Exists("bin\\lyclMiner\\lyclMinerNHML.conf"))
                File.Delete("bin\\lyclMiner\\lyclMinerNHML.conf");

            GenerateConfig("");
            Thread.Sleep(100);
            var conf = "";

            FileStream fs = new FileStream("bin\\lyclMiner\\forbench.", FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader w = new StreamReader(fs);
            conf = w.ReadToEnd();
            w.Close();
            string[] ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.IDByBus.ToString()).ToArray();

            Thread.Sleep(100);
            //url = Globals.GetLocationUrl(AlgorithmType.Lyra2REv3, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], NhmConectionType.STRATUM_TCP);
            conf = conf.Replace("stratum+tcp://example.com:port", url);
            conf = conf.Replace("user", username);
            string newconf = "";
            string[] textArray = conf.Split('\n');
            string[] worksize = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).Replace("worksize=", "").Split(',');

            Array.Reverse(worksize);

            Helpers.ConsolePrint(MinerTag(), ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD));


            int k = 0;

            for (int i = 0; i < textArray.Length; i++)
            {
                string str = textArray[i].ToString();

                if (!str.Contains("DeviceIndex = ") )
                    {
                        newconf = newconf + str + "\n";
                    }

                if (str.Contains("DeviceIndex = ") && str.Contains("BinaryFormat ="))
                {
                    Helpers.ConsolePrint("lyclminer3", "");
                    int st1 = str.IndexOf("DeviceIndex = ");
                    int end1 = str.IndexOf("BinaryFormat");
                    string dev = str.Substring(st1 + 15, end1 - st1 - 17);

                        if (Array.IndexOf(ids, dev) < 0)
                        {
                            str = str.Replace("DeviceIndex = \"", "DeviceIndex = \"-255 ");
                        }

                        if (str.Contains("WorkSize ="))
                        {
                            int st2 = str.IndexOf("WorkSize = ");
                            int end2 = str.IndexOf(">");
                            string work = str.Substring(st2 + 12, end2 - st2 - 12 - 1);

                        if (k < worksize.Length && Array.IndexOf(ids, dev) >= 0 && worksize[k].Length != 0) //костыль
                        {
                            str = str.Replace(work, worksize[k].Trim());
                            k++;
                        }
                         newconf = newconf + str + "\n";

                        }

                }
            }

                FileStream fs2 = new FileStream("bin\\lyclMiner\\lyclMinerNHML.conf", FileMode.Create, FileAccess.ReadWrite);
                StreamWriter w2 = new StreamWriter(fs2);
                w2.Write(newconf);
                //Thread.Sleep(1000);

                w2.Flush();
                //Thread.Sleep(1000);

                w2.Close();
                LastCommandLine = " lyclMinerNHML.conf";
                Thread.Sleep(100);
            ProcessHandle = _Start();

        }

        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        // new decoupled benchmarking routines
        #region Decoupled benchmarking routines

        protected void GenerateConfig(string configfilename)
        {
            var benchmarkconfigHandle = new Process
            {
                StartInfo =
                {
                    FileName = MiningSetup.MinerPath
                }
            };

            {
                BenchmarkProcessPath = benchmarkconfigHandle.StartInfo.FileName;
                Helpers.ConsolePrint(MinerTag(), "Using miner: " + benchmarkconfigHandle.StartInfo.FileName);
                benchmarkconfigHandle.StartInfo.WorkingDirectory = WorkingDirectory;
            }
            if (MinersSettingsManager.MinerSystemVariables.ContainsKey(Path))
            {
                foreach (var kvp in MinersSettingsManager.MinerSystemVariables[Path])
                {
                    var envName = kvp.Key;
                    var envValue = kvp.Value;
                    benchmarkconfigHandle.StartInfo.EnvironmentVariables[envName] = envValue;
                }
            }
            if (File.Exists("bin\\lyclMiner\\forbench" + configfilename))
                File.Delete("bin\\lyclMiner\\forbench" + configfilename);

            Thread.Sleep(500);

            benchmarkconfigHandle.StartInfo.Arguments = " -gr forbench" + configfilename;
            benchmarkconfigHandle.StartInfo.UseShellExecute = false;
            benchmarkconfigHandle.StartInfo.RedirectStandardError = true;
            benchmarkconfigHandle.StartInfo.RedirectStandardOutput = true;
            benchmarkconfigHandle.StartInfo.CreateNoWindow = true;
            Thread.Sleep(250);
            Helpers.ConsolePrint(MinerTag(), "Start bench: " + benchmarkconfigHandle.StartInfo.FileName + benchmarkconfigHandle.StartInfo.Arguments);

            try { benchmarkconfigHandle.Start();  }
                catch (Exception e) { Helpers.ConsolePrint(MinerDeviceName, e.ToString()); }


            try
            {
                if (!benchmarkconfigHandle.WaitForExit(10 * 1000))
                {
                    benchmarkconfigHandle.Kill();
                    benchmarkconfigHandle.WaitForExit(5 * 1000);
                    benchmarkconfigHandle.Close();
                }
            }
            catch { }

            Thread.Sleep(50);
        }
        protected override string GetDevicesCommandString()
        {
            string extraParams = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            string deviceStringCommand = "";
            List<string> ids = new List<string>();

            foreach (var mPair in MiningSetup.MiningPairs.OrderByDescending(pair => pair.Device.IDByBus))
            {
                var id = mPair.Device.IDByBus;
                ids.Add(id.ToString());
            }
            deviceStringCommand += String.Join("", ids);

            return deviceStringCommand;
        }

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {

            string configfilename = GetLogFileName();
            GenerateConfig(configfilename);

              Thread.Sleep(1000);

            Helpers.ConsolePrint("lyclMiner", "Start benchmark after config is generated");

            var conf = "";
            FileStream fs = new FileStream("bin\\lyclMiner\\forbench" + configfilename, FileMode.OpenOrCreate, FileAccess.Read);
            StreamReader w = new StreamReader(fs);
            conf = w.ReadToEnd();
            w.Close();

            Thread.Sleep(500);
            var url = Globals.GetLocationUrl(AlgorithmType.Lyra2REv3, Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], NhmConectionType.STRATUM_TCP);
            var username = Globals.GetBitcoinUser();
            string[] ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.IDByBus.ToString()).ToArray();
            conf = conf.Replace("stratum+tcp://example.com:port", "stratum+tcp://lyra2v3.eu.mine.zpool.ca:4550");
            conf = conf.Replace("user", "1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2");
            conf = conf.Replace("Password = \"x\"", "Password = \"c=BTC\"");


            string newconf = "";
            string[] textArray = conf.Split('\n');
            string[] worksize = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).Replace("worksize=", "").Split(',');

            Array.Reverse(worksize);
            int k = 1;

            for (int i = 0; i < textArray.Length; i++)
            {
                string str = textArray[i].ToString();

                if (!str.Contains("DeviceIndex = "))
                {
                    newconf = newconf + str + "\n";
                }

                if (str.Contains("DeviceIndex = ") && str.Contains("BinaryFormat ="))
                {
                    Helpers.ConsolePrint("lyclminer3", "");
                    int st1 = str.IndexOf("DeviceIndex = ");
                    int end1 = str.IndexOf("BinaryFormat");
                    string dev = str.Substring(st1 + 15, end1 - st1 - 17);

                    if (Array.IndexOf(ids, dev) < 0)
                    {
                        str = str.Replace("DeviceIndex = \"", "DeviceIndex = \"-255 ");
                    }

                    if (str.Contains("WorkSize ="))
                    {
                        int st2 = str.IndexOf("WorkSize = ");
                        int end2 = str.IndexOf(">");
                        string work = str.Substring(st2 + 12, end2 - st2 - 12 - 1);
                        if (k < worksize.Length)
                        {
                            str = str.Replace(work, worksize[k].Trim());
                        }
                        newconf = newconf + str + "\n";
                        k++;
                    }

                }
            }

            Helpers.ConsolePrint("lyclMiner.confNEW:", newconf);
            FileStream fs2 = new FileStream("bin\\lyclMiner\\"+ configfilename, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter w2 = new StreamWriter(fs2);
            w2.Write(newconf);
            w2.Flush();
            w2.Close();

            return " " + configfilename;

        }
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }
        /*
        protected override bool BenchmarkParseLine(string outdata) {
            Helpers.ConsolePrint(MinerTag(), outdata);

            if (benchmarkException)
            {
                if (outdata.Contains("Accepted"))
                {
                    iter++;
                    int end = outdata.IndexOf("H/s");
                    if (outdata.Contains("MH,")) st = outdata.IndexOf("MH,");
                    if (outdata.Contains("kH,")) st = outdata.IndexOf("kH,");
                    if (outdata.Contains("GH,")) st = outdata.IndexOf("GH,");
                    hashspeed = outdata.Substring(st + 3, end - st - 4);
                    speed = Double.Parse(hashspeed, CultureInfo.InvariantCulture);
                    if (outdata.Contains("MH/s"))
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = speed * 1000000;
                    }
                    if (outdata.Contains("kH/s"))
                    {
                        BenchmarkAlgorithm.BenchmarkSpeed = speed * 1000;
                    }
                    if (iter >= 20)
                    {
                        BenchmarkSignalFinnished = true;
                        return true;
                    }
                }
                if (outdata.Contains("stratum_recv_line failed"))
                {
                    BenchmarkException = new Exception("Unknown error");
                    InBenchmark = "Stratum error";
                }
            }
            return false;
        }
        */
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata) {
            CheckOutdata(outdata);
        }

        #endregion // Decoupled benchmarking routines

        public override async Task<ApiData> GetSummaryAsync()
        {
            if (!IsApiReadException) return await GetSummaryCpuCcminerAsync();
            // check if running
            if (ProcessHandle == null)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from CryptoNight Proccess is null");
                return null;
            }
            try
            {
                Process.GetProcessById(ProcessHandle.Id);
            }
            catch (ArgumentException ex)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from CryptoNight reason: " + ex.Message);
                return null; // will restart outside
            }
            catch (InvalidOperationException ex)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Could not read data from CryptoNight reason: " + ex.Message);
                return null; // will restart outside
            }
            /*
            var totalSpeed = MiningSetup.MiningPairs
                .Select(miningPair =>
                    miningPair.Device.GetAlgorithm(MinerBaseType.lyclMiner, AlgorithmType.Lyra2REv3, AlgorithmType.NONE))
                .Where(algo => algo != null).Sum(algo => algo.BenchmarkSpeed);
                */
            var totalSpeed = 0.0d;
            foreach (var miningPair in MiningSetup.MiningPairs)
            {
                var algo = miningPair.Device.GetAlgorithm(MinerBaseType.lyclMiner, AlgorithmType.Lyra2REv3, AlgorithmType.NONE);
                if (algo != null)
                {
                    totalSpeed += algo.BenchmarkSpeed;
                }
            }

            var lyclminerData = new ApiData(MiningSetup.CurrentAlgorithmType)
            {
                Speed = totalSpeed
            };
            CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            // check if speed zero
            if (lyclminerData.Speed == 0) CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            return lyclminerData;
        }
    }
}
