using Newtonsoft.Json;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Miners
{
    public class SRBMiner : Miner
    {
        private readonly int GPUPlatformNumber;
        private int _benchmarkTimeWait = 180;

        private const int TotalDelim = 2;
        int count = 0;
        private double speed = 0;
        private double tmp = 0;
        private bool IsInBenchmark = false;
        private double _power = 0.0d;
        double _powerUsage = 0;

        public SRBMiner() : base("SRBMiner")
        {
            CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            GPUPlatformNumber = ComputeDeviceManager.Available.AmdOpenCLPlatformNum;
        }

        public override void Start(string btcAdress, string worker)
        {
            string url = "";
            IsInBenchmark = false;
            //IsApiReadException = MiningSetup.MinerPath == MinerPaths.Data.SRBMiner;

            LastCommandLine = GetStartCommand(btcAdress, worker);
            ProcessHandle = _Start();
        }
        private string GetServer(string algo, string username, string port)
        {
            string ret = "";
            string ssl = "";
            string psw = "x";
            if (ConfigManager.GeneralConfig.StaleProxy) psw = "stale";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                ssl = "stratum+ssl://";
            }
            else
            {
                port = "1" + port;
                ssl = "stratum+tcp://";
            }
            string pools = "--pool ";
            string users = " --wallet ";
            string passwords = " --password ";
            string nicehash = " --nicehash ";
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + "!" + Links.CheckDNS(algo + "." + serverUrl) + ":9200";
                    users = users + "!" + username;
                    passwords = passwords + "!x";
                    nicehash = nicehash + "!true";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + "!" + ssl + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port;
                    users = users + "!" + username;
                    passwords = passwords + "!" + psw;
                    nicehash = nicehash + "!true";
                }
            }
            return (pools + ret + users + passwords + nicehash).Replace("--pool !", "--pool ").
                Replace("--wallet !", "--wallet ").Replace("--password !", "--password ").
                Replace("--nicehash !", "--nicehash ") + " ";
        }
        //
        private string GetServer2(string algo1, string algo2, string username, string port, string port2)
        {
            string ret = "";
            string ssl = "";
            if (ConfigManager.GeneralConfig.ProxySSL)
            {
                port = "4" + port;
                port2 = "4" + port2;
                ssl = "stratum+ssl://";
            }
            else
            {
                port = "1" + port;
                port2 = "1" + port2;
                ssl = "stratum+tcp://";
            }
            string pools = "--pool ";
            string users = " --wallet ";
            string passwords = " --password ";
            string nicehash = " --nicehash ";
            foreach (string serverUrl in Globals.MiningLocation)
            {
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + "!" + algo1 + "." + serverUrl + ":9200;" + algo2 + "." + serverUrl + ":9200";
                    users = users + "!" + username + ";" + username;
                    passwords = passwords + "!x;x";
                    nicehash = nicehash + "!true;true";
                    break; //no failover
                }
                else
                {
                    ret = ret + "!" + ssl + algo1 + "." + serverUrl + ":" + port +";" + ssl + algo2 + "." + serverUrl + ":" + port2;
                    users = users + "!" + username + ";" + username;
                    passwords = passwords + "!x;x";
                    nicehash = nicehash + "!true;true";
                    break; //no failover
                }
            }
            return (pools + ret + users + passwords + nicehash).Replace("--pool !", "--pool ").
                Replace("--wallet !", "--wallet ").Replace("--password !", "--password ").
                Replace("--nicehash !", "--nicehash ") + " ";
        }
        private string GetStartCommand(string btcAddress, string worker)
        {
            string username = GetUsername(btcAddress, worker);
            string ZilMining = "";
            string disablePlatform = "--disable-gpu-nvidia";
            DeviceType devtype = DeviceType.NVIDIA;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }

            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.SRBMiner, devtype))
            {
                //прокси не используется
                ZilMining = " --zil-enable --zil-pool stratum+tcp://etchash.auto.nicehash.com:9200 --zil-wallet " + 
                            username + " --zil-esm 2 --disable-worker-watchdog ";
            }
            if (devtype == DeviceType.AMD)
            {
                disablePlatform = "--disable-cpu --disable-gpu-nvidia --disable-gpu-intel ";
            }
            if (devtype == DeviceType.INTEL)
            {
                disablePlatform = "--disable-cpu --disable-gpu-nvidia --disable-gpu-amd ";
            }
            try
            {
                var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, devtype);
                //сначала дуалы
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.KHeavyHash))
                {
                    return " --main-pool-reconnect 2 " + disablePlatform + " --multi-algorithm-job-mode 3 " +
                        $"--algorithm ethash;kaspa " +
                        GetServer2("daggerhashimoto", "kheavyhash", username, "3353", "3395") +
                        $"--api-enable --api-port {ApiPort} " +
                   " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.KHeavyHash))
                {
                    return " --main-pool-reconnect 2 " + disablePlatform + " --multi-algorithm-job-mode 3 " +
                        $"--algorithm autolykos2;kaspa " +
                        GetServer2("autolykos", "kheavyhash", username, "3390", "3395") + ZilMining +
                    $"--api-enable --api-port {ApiPort} " +
                   " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                }
                //
                if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    return " --main-pool-reconnect 2 " + disablePlatform + " --a0-is-zil " +
                        $"--algorithm ethash;autolykos2 " +
                        GetServer2("daggerhashimoto", "autolykos", username, "3353", "3390") + 
                        $"--api-enable --api-port {ApiPort} " +
                   " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                }
                //
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX))
                {
                    var algo = "randomxmonero";
                    var port = "3380";

                    return $" --algorithm randomx --disable-gpu --api-enable --api-port {ApiPort} {extras} " +
                        GetServer(algo, username, port);
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.VerusHash))
                {
                    var algo = "verushash";
                    var port = "3394";

                    return $" --algorithm verushash --disable-gpu --api-enable --api-port {ApiPort} {extras} " +
                        GetServer(algo, username, port);
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    var port = "3353";
                    var algo = "daggerhashimoto";

                    return " --main-pool-reconnect 2 --a0-is-zil " + disablePlatform + $"--algorithm ethash --api-enable --api-port {ApiPort} " +
                    GetServer(algo, username, port) +
                    " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
                {
                    var port = "3393";
                    var algo = "etchash";

                    return " --main-pool-reconnect 2 --a0-is-zil " + disablePlatform + $"--algorithm etchash --api-enable --api-port {ApiPort} " +
                    GetServer(algo, username, port) +
                    " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                {
                    var port = "3390";
                    var algo = "autolykos";

                    return " --main-pool-reconnect 2 " + disablePlatform + $" --algorithm autolykos2 --api-enable --api-port {ApiPort} " +
                    GetServer(algo, username, port) + ZilMining +
                   " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KHeavyHash))
                {
                    var port = "3395";
                    var algo = "kheavyhash";

                    return " --main-pool-reconnect 2 " + disablePlatform + $" --algorithm kaspa --api-enable --api-port {ApiPort} " +
                    GetServer(algo, username, port) + ZilMining +
                   " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.IronFish))
                {
                    var port = "3397";
                    var algo = "ironfish";

                    return " --main-pool-reconnect 2 " + disablePlatform + $" --algorithm blake3_ironfish --api-enable --api-port {ApiPort} " +
                    GetServer(algo, username, port) + ZilMining +
                   " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                }

            } catch (Exception ex)
            {
                Helpers.ConsolePrint("GetStartCommand", ex.ToString());
            }
            return "unsupported algo";

        }

        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";

            var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.IDByBus.ToString()).ToList();
            ids.Sort();
            deviceStringCommand += string.Join("!", ids);

            return deviceStringCommand;
        }
        private string GetStartBenchmarkCommand(string btcAddress, string worker)
        {
            string disablePlatform = "--disable-gpu-nvidia";
            DeviceType devtype = DeviceType.NVIDIA;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }

            if (devtype == DeviceType.AMD)
            {
                disablePlatform = "--disable-cpu --disable-gpu-nvidia --disable-gpu-intel ";
            }
            if (devtype == DeviceType.INTEL)
            {
                disablePlatform = "--disable-cpu --disable-gpu-nvidia --disable-gpu-amd ";
            }

            IsInBenchmark = true;
            var LastCommandLine = GetStartCommand(btcAddress, worker);
            var extras = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, devtype);
            string username = GetUsername(btcAddress, worker);



            //сначала дуалы
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.KHeavyHash))
            {
                return $" " + disablePlatform + " --algorithm autolykos2" +
                    $" --pool {Links.CheckDNS("stratum+tcp://pool.woolypooly.com")}:3100" +
                    $" --wallet 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.SRBMiner" +
                    " --algorithm kaspa" +
                    $" --pool {Links.CheckDNS("stratum+tcp://pool.eu.woolypooly.com")}:3112" +
                    $" --wallet kaspa:qq9y94k2xqumnsgvx6huxn3uugzy8euzxjh9utxe338ck0ufch0hkvvd37vc0.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                _benchmarkTimeWait = 30;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.KHeavyHash))
            {
                return $" " + disablePlatform + " --algorithm ethash" +
                    $" --pool {Links.CheckDNS("stratum+tcp://ethw.2miners.com")}:2020" +
                    $" --wallet 0x266b27bd794d1A65ab76842ED85B067B415CD505.SRBMiner" +
                    " --algorithm kaspa" +
                    $" --pool {Links.CheckDNS("stratum+tcp://pool.eu.woolypooly.com")}:3112" +
                    $" --wallet kaspa:qq9y94k2xqumnsgvx6huxn3uugzy8euzxjh9utxe338ck0ufch0hkvvd37vc0.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                _benchmarkTimeWait = 30;
            }

            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.VerusHash))
            {
                ApiPort = 4040;

                return $" --disable-gpu --algorithm verushash"
                + $" --pool {Links.CheckDNS("stratum+tcp://verushash.mine.zergpool.com")}:3300 --wallet 1JqFnUR3nDFCbNUmWiQ4jX6HRugGzX55L2 --password c=BTC" +
                $" --nicehash true --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName() } {extras}";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX))
            {
                ApiPort = 4040;

                return $" --disable-gpu --algorithm randomx"
                + $" --pool {Links.CheckDNS("stratum+tcp://xmr-eu1.nanopool.org")}:14444 --wallet 42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.benchmark" +
                $" --nicehash false --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName() } {extras}";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                return $" " + disablePlatform + " --algorithm ethash" +
                    $" --pool {Links.CheckDNS("stratum+tcp://ethw.2miners.com")}:2020" +
                    $" --wallet 0x266b27bd794d1A65ab76842ED85B067B415CD505.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                return $" " + disablePlatform + " --algorithm etchash" +
                    $" --pool {Links.CheckDNS("stratum+tcp://etc.2miners.com")}:1010" +
                    $" --wallet 0x266b27bd794d1A65ab76842ED85B067B415CD505.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                return $" " + disablePlatform + " --algorithm autolykos2" +
                    $" --pool {Links.CheckDNS("stratum+tcp://pool.woolypooly.com")}:3100" +
                    $" --wallet 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KHeavyHash))
            {
                return $" " + disablePlatform + " --algorithm kaspa" +
                    $" --pool {Links.CheckDNS("stratum+tcp://pool.eu.woolypooly.com")}:3112" +
                    $" --wallet kaspa:qq9y94k2xqumnsgvx6huxn3uugzy8euzxjh9utxe338ck0ufch0hkvvd37vc0.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                _benchmarkTimeWait = 30;
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.IronFish))
            {
                return $" " + disablePlatform + " --algorithm blake3_ironfish" +
                    $" --pool {Links.CheckDNS("stratum+tcp://ru.ironfish.herominers.com")}:1145" +
                    $" --wallet fb8aaaf8594143a4007c9fe0e0056bd3ca55848d0f5247f7eee8918ca8345521.SRBMiner" +
                    $" --api-enable --api-port {ApiPort} --extended-log --log-file {GetLogFileName()}" +
                " --gpu-id " + GetDevicesCommandString().Trim() + " " + extras;
                _benchmarkTimeWait = 30;
            }

            return "unknown";
        }

        protected override void _Stop(MinerStopType willswitch)
        {
            Helpers.ConsolePrint("SRBMINER Stop", "");
            DeviceType devtype = DeviceType.AMD;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }
            if (Form_Main.ZilMonitorRunning &&
                Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.SRBMiner, devtype))
            {
                ZilClient.needConnectionZIL = false;
            }
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
            StopDriver();
        }

        private void StopDriver()
        {
            //srbminer driver
            var CMDconfigHandleWD = new Process

            {
                StartInfo =
                {
                    FileName = "sc.exe"
                }
            };

            CMDconfigHandleWD.StartInfo.Arguments = "stop winio";
            CMDconfigHandleWD.StartInfo.UseShellExecute = false;
            CMDconfigHandleWD.StartInfo.CreateNoWindow = true;
            CMDconfigHandleWD.Start();
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5;  // 5 min
        }

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {            
            string ResponseFromSRBMiner;
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString());
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 3 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 2 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromSRBMiner = await Reader.ReadToEndAsync();

                Reader.Close();
                Response.Close();
                WR.Abort();
                SS.Close();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API", ex.Message);
                return null;
            }

            dynamic resp = JsonConvert.DeserializeObject(ResponseFromSRBMiner);
            //Helpers.ConsolePrint("API ->:", ResponseFromSRBMiner.ToString());
            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType, MiningSetup.MiningPairs[0]);
            //ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType);
            ad.ThirdAlgorithmID = AlgorithmType.NONE;

            if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
            {
                ad.SecondaryAlgorithmID = MiningSetup.CurrentSecondaryAlgorithmType;
            }

            double totalsMain = 0;
            double totalsSecond = 0;
            double totalsThird = 0;
            /*
            if (ResponseFromSRBMiner.ToLower().Contains("\"name\": \"zil\""))
            {
                Form_Main.isZilRound = true;
            }
            else
            {
                Form_Main.isZilRound = false;
            }
            */
            try
            {
                ad.ZilRound = false;
                
                if (resp != null)
                {
                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();

                    if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE) &&
                        !ResponseFromSRBMiner.ToLower().Contains("\"name\": \"zil\""))//single, no zil
                    {
                        foreach (var mPair in sortedMinerPairs)
                        {
                            try
                            {
                                string token = $"algorithms[0].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash = resp.SelectToken(token);
                                int gpu_hr = (int)Convert.ToDouble(hash, CultureInfo.InvariantCulture.NumberFormat);
                                mPair.Device.MiningHashrate = gpu_hr;
                                _power = mPair.Device.PowerUsage;
                                mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                                mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("API Exception:", ex.ToString());
                            }
                        }
                        dynamic _tm = resp.algorithms[0].hashrate.gpu.total;
                        if (_tm != null)
                        {
                            totalsMain = resp.algorithms[0].hashrate.gpu.total;
                        }
                    }

                    if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE) &&
                        ResponseFromSRBMiner.ToLower().Contains("\"name\": \"zil\""))//single, + zil
                    {
                        foreach (var mPair in sortedMinerPairs)
                        {
                            try
                            {
                                string token0 = $"algorithms[0].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash0 = resp.SelectToken(token0);
                                int gpu_hr0 = (int)Convert.ToInt32(hash0, CultureInfo.InvariantCulture.NumberFormat);
                                mPair.Device.MiningHashrate = gpu_hr0;

                                string token1 = $"algorithms[1].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash1 = resp.SelectToken(token1);
                                int gpu_hr1 = (int)Convert.ToInt32(hash1, CultureInfo.InvariantCulture.NumberFormat);
                                mPair.Device.MiningHashrateSecond = gpu_hr1;

                                if (Form_Main.isZilRound)
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
                                    mPair.Device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                                    mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                                }
                                _power = mPair.Device.PowerUsage;
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("API Exception:", ex.ToString());
                            }
                        }
                        totalsMain = resp.algorithms[0].hashrate.gpu.total;
                        totalsSecond = resp.algorithms[1].hashrate.gpu.total;
                    }

                    if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))//dual no zil
                    {
                        foreach (var mPair in sortedMinerPairs)
                        {
                            try
                            {
                                string token0 = $"algorithms[0].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash0 = resp.SelectToken(token0);
                                int gpu_hr0 = (int) Convert.ToInt32(hash0, CultureInfo.InvariantCulture.NumberFormat);

                                string token1 = $"algorithms[1].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash1 = resp.SelectToken(token1);
                                int gpu_hr1 = (int) Convert.ToInt32(hash1, CultureInfo.InvariantCulture.NumberFormat);

                                mPair.Device.MiningHashrate = gpu_hr0;
                                mPair.Device.MiningHashrateSecond = gpu_hr1;
                                _power = mPair.Device.PowerUsage;
                                mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                                mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                                mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("API Exception:", ex.ToString());
                            }
                        }
                        totalsMain = resp.algorithms[0].hashrate.gpu.total;
                        try
                        {
                            totalsSecond = resp.algorithms[1].hashrate.gpu.total;
                        } catch
                        {
                            totalsSecond = 0;
                        }
                    }

                    if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE) &&
                        ResponseFromSRBMiner.ToLower().Contains("\"name\": \"zil\""))//dual + zil
                    {
                        foreach (var mPair in sortedMinerPairs)
                        {
                            try
                            {
                                string token0 = $"algorithms[0].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash0 = resp.SelectToken(token0);
                                int gpu_hr0 = (int)Convert.ToInt32(hash0, CultureInfo.InvariantCulture.NumberFormat);

                                string token1 = $"algorithms[1].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash1 = resp.SelectToken(token1);
                                int gpu_hr1 = (int)Convert.ToInt32(hash1, CultureInfo.InvariantCulture.NumberFormat);

                                string token2 = $"algorithms[2].hashrate.gpu.gpu{mPair.Device.IDByBus}";
                                var hash2 = resp.SelectToken(token2);
                                int gpu_hr2 = (int)Convert.ToInt32(hash2, CultureInfo.InvariantCulture.NumberFormat);

                                mPair.Device.MiningHashrate = gpu_hr0;
                                mPair.Device.MiningHashrateSecond = gpu_hr1;
                                mPair.Device.MiningHashrateThird = gpu_hr2;

                                if (Form_Main.isZilRound)
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
                                    mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                                }

                                _power = mPair.Device.PowerUsage;
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("API Exception:", ex.ToString());
                            }
                        }
                        totalsMain = resp.algorithms[0].hashrate.gpu.total;
                        totalsSecond = resp.algorithms[1].hashrate.gpu.total;
                        totalsThird = resp.algorithms[2].hashrate.gpu.total;
                    }

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.RandomX) ||
                        MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.VerusHash))
                    {
                        try
                        {
                            totalsMain = resp.algorithms[0].hashrate.cpu.total;
                        }
                        catch (Exception ex)
                        {
                            totalsMain = 0;
                        }
                        foreach (var mPair in sortedMinerPairs)
                        {
                            mPair.Device.MiningHashrate = totalsMain;
                            _power = mPair.Device.PowerUsage;
                            mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                            mPair.Device.SecondAlgorithmID = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                            mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                        }
                    }


                    ad.Speed = totalsMain;
                    ad.SecondarySpeed = totalsSecond;
                    ad.ThirdSpeed = totalsThird;

                    if (ad.Speed + ad.SecondarySpeed + ad.ThirdSpeed == 0)
                    {
                        CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                    }
                    else
                    {
                        CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                        DeviceType devtype = DeviceType.NVIDIA;
                        sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
                        foreach (var mPair in sortedMinerPairs)
                        {
                            devtype = mPair.Device.DeviceType;
                        }
                        if (!Form_Main.ZilMonitorRunning &&
                            Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.SRBMiner, devtype))
                        {
                            ZilClient.needConnectionZIL = true;
                            Form_Main.ZilMonitorRunning = true;
                            ZilClient.StartZilMonitor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API error", ex.ToString());
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                ad.Speed = 0;
                return ad;
            }

            ad.ZilRound = false;
            ad.Speed = totalsMain;
            ad.SecondarySpeed = totalsSecond;
            ad.ThirdSpeed = totalsThird;

            if (Form_Main.isZilRound)
            {
                if (MiningSetup.CurrentSecondaryAlgorithmType != AlgorithmType.NONE)//dual
                {
                    if (ResponseFromSRBMiner.ToLower().Contains("\"name\": \"zil\""))//dual+zil
                    {
                        ad.Speed = 0;
                        ad.SecondarySpeed = 0;
                        ad.ThirdSpeed = totalsThird;
                        ad.ZilRound = true;
                        ad.AlgorithmID = AlgorithmType.NONE;
                        ad.SecondaryAlgorithmID = AlgorithmType.NONE;
                        ad.ThirdAlgorithmID = AlgorithmType.DaggerHashimoto;
                    }
                }
                else
                {
                    if (ResponseFromSRBMiner.ToLower().Contains("\"name\": \"zil\"") && totalsSecond > 0)//+zil
                    {
                        ad.Speed = 0;
                        ad.SecondarySpeed = totalsSecond;
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
                        ad.Speed = totalsMain;
                        ad.SecondarySpeed = totalsSecond;
                    }
                }
                else
                {
                    //if (_algo.ToLower().Contains("zil"))
                    {
                        ad.Speed = totalsMain;
                        ad.SecondarySpeed = 0;
                        ad.SecondaryAlgorithmID = AlgorithmType.NONE;
                    }
                }
            }


            Thread.Sleep(1);
            return ad;
        }

        protected override bool IsApiEof(byte third, byte second, byte last)
        {
            return third == 0x7d && second == 0xa && last == 0x7d;
        }

        #region Benchmark

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            _benchmarkTimeWait = time;
            return GetStartBenchmarkCommand(Globals.GetBitcoinUser(), ConfigManager.GeneralConfig.WorkerName.Trim());
        }

        protected override void BenchmarkThreadRoutine(object commandLine)
        {
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;
            double repeats = 0;
            double summspeed = 0.0d;
            double summspeedSecond = 0.0d;

            int delay_before_calc_hashrate = 10;
            int MinerStartDelay = 10;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                double BenchmarkSpeed = 0.0d;
                double BenchmarkSpeedSecond = 0.0d;
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                _benchmarkTimeWait = _benchmarkTimeWait + 90;
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in: " + _benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess((string)commandLine);
                var benchmarkTimer = new Stopwatch();
                benchmarkTimer.Reset();
                benchmarkTimer.Start();

                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                BenchmarkThreadRoutineStartSettup(); //need for benchmark log
                while (IsActiveProcess(BenchmarkHandle.Id))
                {
                    if (benchmarkTimer.Elapsed.TotalSeconds >= (_benchmarkTimeWait + 90)
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
                        break;
                    }
                    // wait a second due api request
                    Thread.Sleep(1000);

                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                    {
                        MinerStartDelay = 10;
                        delay_before_calc_hashrate = 15;
                        _benchmarkTimeWait = _benchmarkTimeWait - 90;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KHeavyHash))
                    {
                        MinerStartDelay = 5;
                        delay_before_calc_hashrate = 5;
                        _benchmarkTimeWait = _benchmarkTimeWait - 90;
                    }
                    if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.IronFish))
                    {
                        MinerStartDelay = 5;
                        delay_before_calc_hashrate = 5;
                        _benchmarkTimeWait = _benchmarkTimeWait - 90;
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
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() + " second: " + ad.Result.SecondarySpeed.ToString() + " power: " + _power.ToString());
                            summspeed += ad.Result.Speed;
                            summspeedSecond += ad.Result.SecondarySpeed;
                        }
                        else
                        {
                            Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                        }

                        if (repeats >= _benchmarkTimeWait - MinerStartDelay - 15)
                        {
                            BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
                            BenchmarkSpeedSecond = Math.Round(summspeedSecond / (repeats - delay_before_calc_hashrate), 2);
                            Helpers.ConsolePrint(MinerTag(), "Benchmark ended. BenchmarkSpeed: " + BenchmarkSpeed.ToString() + " second: " + BenchmarkSpeedSecond.ToString());
                            ad.Dispose();
                            benchmarkTimer.Stop();

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            if (!MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                            {
                                //EndBenchmarkProcces();
                            }
                            StopDriver();
                            break;
                        }

                    }
                }

                BenchmarkAlgorithm.BenchmarkSpeed = BenchmarkSpeed;
                BenchmarkAlgorithm.BenchmarkSecondarySpeed = BenchmarkSpeedSecond;
                BenchmarkAlgorithm.PowerUsageBenchmark = (_powerUsage / repeats);

            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ex.ToString());
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {
                EndBenchmarkProcces();
                BenchmarkThreadRoutineFinish();

                // find latest log file
                string latestLogFile = "";
                var dirInfo = new DirectoryInfo(WorkingDirectory);
                foreach (var file in dirInfo.GetFiles(GetLogFileName()))
                {
                    latestLogFile = file.Name;
                    break;
                }
                try
                {
                    // read file log
                    if (File.Exists(WorkingDirectory + latestLogFile))
                    {
                        var lines = File.ReadAllLines(WorkingDirectory + latestLogFile);
                        foreach (var line in lines)
                        {
                            if (line != null)
                            {
                                CheckOutdata(line);
                            }
                        }
                        File.Delete(WorkingDirectory + latestLogFile);
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint(MinerTag(), ex.ToString());
                }
            }
        }
        
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }
        #endregion
    }

}
