/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Parsing;
using NiceHashMiner.Devices;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Miners.Grouping;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace NiceHashMiner.Miners
{
    public class SRBMiner : Miner
    {
        private readonly int GPUPlatformNumber;
        private int _benchmarkTimeWait = 240 + 20;

       // private int TotalCount = 2;
        private const int TotalDelim = 2;
        int count = 0;
        private double speed = 0;
        private double tmp = 0;
        private string[,] myServers = Form_Main.myServers;

        public SRBMiner() : base("SRBMiner") {
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
        }
/*
        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            if (this.MiningSetup.MinerPath == MinerPaths.Data.SRBMiner)
            {
                return 60 * 1000 * 12; // wait for hashrate string
            }
            _maxCooldownTimeInMilliseconds = 60 * 1000 * 12;
            return 60 * 1000 * 12;
        }

*/
        public override void Start(string url, string btcAdress, string worker) {
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.SRBMiner;

            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
        }

        private string GetStartCommand(string url, string btcAdress, string worker) {
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            var algo = "cryptonightv8";
            var port = "3367";
            var variant = " --ccryptonighttype normalv8";
            url = url.Replace("stratum+tcp://", "");
            string nhsuff = "";
            string username = GetUsername(btcAdress, worker);
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            if (File.Exists("bin_3rdparty\\SRBMiner\\poolsV8.txt"))
                File.Delete("bin_3rdparty\\SRBMiner\\poolsV8.txt");
            Thread.Sleep(200);
            var str1 = "{\r\n" +
                       "\"pools\" :\r\n" +
                       "[\r\n";
            var str2 = "        {\r\n" +
                    "                \"pool\" : \"cryptonightv8." + myServers[1, 0] + nhsuff + ".nicehash.com:3367\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightv8." + myServers[2, 0] + nhsuff + ".nicehash.com:3367\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightv8." + myServers[3, 0] + nhsuff + ".nicehash.com:3367\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightv8." + myServers[4, 0] + nhsuff + ".nicehash.com:3367\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightv8." + myServers[5, 0] + nhsuff + ".nicehash.com:3367\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightv8." + myServers[0, 0] + nhsuff + ".nicehash.com:3367\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "        }\r\n"+
                    "]\r\n}";
            try
            {
                FileStream fs = new FileStream("bin_3rdparty\\SRBMiner\\poolsV8.txt", FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(fs);
                w.Write(str1+str2);
                w.Flush();
                w.Close();
                Thread.Sleep(200);
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("poolsV8.txt write error:", e.ToString());
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightHeavy))
            {
                algo = "cryptonightheavy";
                port = "3364";
                variant = " --ccryptonighttype heavy";
                if (File.Exists("bin_3rdparty\\SRBMiner\\poolsH.txt"))
                    File.Delete("bin_3rdparty\\SRBMiner\\poolsH.txt");
                Thread.Sleep(200);
                var strh1 = "{\r\n" +
                           "\"pools\" :\r\n" +
                           "[\r\n";

                var strh2 = "        {\r\n" +
                    "                \"pool\" : \"cryptonightheavy." + myServers[1, 0] + nhsuff + ".nicehash.com:3364\",\r\n" +
                    "                \"wallet\" : \""+ username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightheavy." + myServers[2, 0] + nhsuff + ".nicehash.com:3364\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightheavy." + myServers[3, 0] + nhsuff + ".nicehash.com:3364\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightheavy." + myServers[4, 0] + nhsuff + ".nicehash.com:3364\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightheavy." + myServers[5, 0] + nhsuff + ".nicehash.com:3364\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightheavy." + myServers[0, 0] + nhsuff + ".nicehash.com:3364\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "        }\r\n";
                var strh3 = "]\r\n" +
                           "}";
                try
                {
                    FileStream fs = new FileStream("bin_3rdparty\\SRBMiner\\poolsH.txt", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.Write(strh1 + strh2 + strh3);
                    w.Flush();
                    w.Close();
                    Thread.Sleep(200);
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("poolsH.txt write error:", e.ToString());
                }
                return $" {variant} --cgpuid {GetDevicesCommandString().TrimStart()} {extras} --cnicehash true --apienable --apiport {ApiPort} --cpool {url} --cwallet {username} --cpassword x --pools poolsH.txt";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightR))
            {
                algo = "cryptonighr";
                port = "3375";
                variant = " --ccryptonighttype normalv4";
                if (File.Exists("bin_3rdparty\\SRBMiner\\poolsR.txt"))
                    File.Delete("bin_3rdparty\\SRBMiner\\poolsR.txt");
                Thread.Sleep(200);
                var strh1 = "{\r\n" +
                           "\"pools\" :\r\n" +
                           "[\r\n";

                var strh2 = "        {\r\n" +
                    "                \"pool\" : \"cryptonightr." + myServers[1, 0] + nhsuff + ".nicehash.com:3375\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightr." + myServers[2, 0] + nhsuff + ".nicehash.com:3375\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightr." + myServers[3, 0] + nhsuff + ".nicehash.com:3375\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightr." + myServers[4, 0] + nhsuff + ".nicehash.com:3375\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightr." + myServers[5, 0] + nhsuff + ".nicehash.com:3375\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "},\r\n" +
                    "        {\r\n" +
                    "                \"pool\" : \"cryptonightr." + myServers[0, 0] + nhsuff + ".nicehash.com:3375\",\r\n" +
                    "                \"wallet\" : \"" + username + "\",\r\n" +
                    "                \"password\" : \"x\"\r\n" +
                    "        }\r\n";
                var strh3 = "]\r\n" +
                           "}";
                try
                {
                    FileStream fs = new FileStream("bin_3rdparty\\SRBMiner\\poolsR.txt", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.Write(strh1 + strh2 + strh3);
                    w.Flush();
                    w.Close();
                    Thread.Sleep(200);
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("poolsR.txt write error:", e.ToString());
                }
                return $" {variant} --cgpuid {GetDevicesCommandString().TrimStart()} {extras} --cnicehash true --apienable --apiport {ApiPort} --cpool {url} --cwallet {username} --cpassword x --pools poolsR.txt";
            }

            return $" {variant} --cgpuid {GetDevicesCommandString().TrimStart()} {extras} --cnicehash true --apienable --apiport {ApiPort} --cpool {url} --cwallet {username} --cpassword x --pools poolsV8.txt";
        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";

            var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.IDByBus.ToString()).ToList();
            deviceStringCommand += string.Join(",", ids);

            return deviceStringCommand;
        }
        private string GetStartBenchmarkCommand(string url, string btcAdress, string worker)
        {
            if (url.Contains("Auto"))
            {
                url = url.Replace("Auto", "eu");
            }
            var LastCommandLine = GetStartCommand(url, btcAdress, worker);
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD);
            var algo = "cryptonightv8";
            var port = "3367";
            var variant = " --ccryptonighttype normalv8";
            url = url.Replace("stratum+tcp://", "");

            if (File.Exists(GetLogFileName()))
                File.Delete(GetLogFileName());
            Thread.Sleep(500);
            string nhsuff = "";
            string username = GetUsername(btcAdress, worker);
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                nhsuff = Configs.ConfigManager.GeneralConfig.StratumSuff;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightHeavy))
            {
                algo = "cryptonightheavy";
                port = "3364";
                variant = " --ccryptonighttype heavy";
                return $" {variant} --cgpuid {GetDevicesCommandString().TrimStart()} {extras} --apienable --apiport {ApiPort} --cpool cryptonightheavy.hk{nhsuff}.nicehash.com:3364 --cwallet {username} --cpassword x --logfile {GetLogFileName()} --pools poolsH.txt";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightR))
            {
                algo = "cryptonightr";
                port = "3375";
                variant = " --ccryptonighttype normalv4";
                return $" {variant} --cgpuid {GetDevicesCommandString().TrimStart()} {extras} --apienable --apiport {ApiPort} --cpool xmr-eu1.nanopool.org:14444 --cwallet 42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.{worker} --cpassword x --logfile {GetLogFileName()} --pools poolsR.txt";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.CryptoNightV8))
            {
                algo = "cryptonightv8";
                port = "3367";
                variant = " --ccryptonighttype normalv8";
                return $" {variant} --cgpuid {GetDevicesCommandString().TrimStart()} {extras} --apienable --apiport {ApiPort} --cpool cryptonightv8.hk{nhsuff}.nicehash.com:3367 --cwallet {username} --cpassword x --logfile {GetLogFileName()} --pools poolsV8.txt";
            }
            return "unknown";
        }

        protected override void _Stop(MinerStopType willswitch) {
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5;  // 5 min
        }
        public override async Task<ApiData> GetSummaryAsync()
        {

            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            string ResponseFromSRBMiner;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString());
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 30 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 20 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromSRBMiner = await Reader.ReadToEndAsync();
                //Helpers.ConsolePrint("API...........", ResponseFromSRBMiner);
                //if (ResponseFromSRBMiner.Length == 0 || (ResponseFromSRBMiner[0] != '{' && ResponseFromSRBMiner[0] != '['))
                //    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                //Helpers.ConsolePrint("API", ex.Message);
                return null;
            }

            dynamic resp = JsonConvert.DeserializeObject(ResponseFromSRBMiner);

            if (resp != null)
            {
                int totals = resp.hashrate_total_now;
                //Helpers.ConsolePrint("API hashrate...........", totals.ToString());

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

            Thread.Sleep(1000);
            return ad;


        }

        protected override bool IsApiEof(byte third, byte second, byte last) {
            return third == 0x7d && second == 0xa && last == 0x7d;
        }

        #region Benchmark

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time) {
            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
                Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], 
                ConectionType);
              // _benchmarkTimeWait = time;//SRBMiner report hashrate every 3 min
            return GetStartBenchmarkCommand(server, Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim());
        }
                
        protected override void BenchmarkThreadRoutine(object CommandLine)
        {
            BenchmarkThreadRoutineAlternate(CommandLine, _benchmarkTimeWait);
        }

        protected override void ProcessBenchLinesAlternate(string[] lines)
        {
            int kspeed = 1;
            foreach (var line in lines)
            {
                Helpers.ConsolePrint(MinerTag(), line);
                BenchLines.Add(line);
                var lineLowered = line.ToLower();

                if (lineLowered.Contains("Total:".ToLower()))
                {
                    var st = lineLowered.IndexOf("Total: ".ToLower());
                    var e = lineLowered.IndexOf("/s".ToLower());

                    if (lineLowered.Contains("kh/s"))
                        kspeed = 1000;
                    else if (lineLowered.Contains("mh/s"))
                        kspeed = 1000000;
                    count++;
                    var parse = lineLowered.Substring(st + 7, e - st - 9).Trim().Replace(",", ".");
                    try
                    {
                        tmp = Double.Parse(parse, CultureInfo.InvariantCulture) * kspeed;
                    }
                    catch
                    {
                        MessageBox.Show("Unsupported miner version - " + MiningSetup.MinerPath,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        BenchmarkSignalFinnished = true;
                    }

                    speed = speed + tmp;
                    BenchmarkAlgorithm.BenchmarkSpeed = speed / (count);
                    /*
                    if (count >= TotalCount)
                    {
                        BenchmarkSignalFinnished = true;
                    }
                    */
                    //BenchmarkAlgorithm.BenchmarkSpeed = speed;
                }
            }
        }

        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }

        protected override bool BenchmarkParseLine(string outdata)
        {
            Helpers.ConsolePrint(MinerTag(), outdata);
            return false;
        }
        #endregion
    }
    
}
