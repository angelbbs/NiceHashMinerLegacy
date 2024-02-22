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
using System.Management;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Miners
{
    public class Nanominer : Miner
    {
        private int _benchmarkTimeWait = 180;
        string ResponseFromNanominer;
        public string platform = "";
        public FileStream fs;
        private bool IsInBenchmark = false;
        private double _power = 0.0d;
        double _powerUsage = 0;
        int hashrateErrorCount = 0;

        public Nanominer() : base("Nanominer")
        {
            ConectionType = NhmConectionType.NONE;
        }

        public override void Start(string btcAdress, string worker)
        {
            LastCommandLine = GetStartCommand(btcAdress, worker);
            ProcessHandle = _Start();
            try
            {
                do
                {
                    Thread.Sleep(1000);
                } while (!File.Exists("miners\\Nanominer\\" + GetLogFileName()));
                Thread.Sleep(1000);
                fs = new FileStream("miners\\Nanominer\\" + GetLogFileName(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ex.Message);
            }
        }
        private string GetServer(string algo, string username, string port)
        {
            string ret = "";
            string ssl = "";
            if (ConfigManager.GeneralConfig.ProxySSL && Globals.MiningLocation.Length > 1)
            {
                port = "4" + port;
                ssl = "useSSL = true\n";
            }
            else
            {
                port = "1" + port;
                ssl = "useSSL = false\n";
            }
            int n = 0;
            foreach (string serverUrl in Globals.MiningLocation)
            {
                n++;
                if (serverUrl.Contains("auto"))
                {
                    ret = ret + "pool" + n.ToString() + " = "  + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":9200 ";
                    if (!ConfigManager.GeneralConfig.ProxyAsFailover) break;
                }
                else
                {
                    ret = ret + ssl;
                    ret = ret + "pool" + n.ToString() + " = " + Links.CheckDNS(algo + "." + serverUrl).Replace("stratum+tcp://", "") + ":" + port + " ";
                }
                ret = ret + "\n";
            }
            return ret.Replace("-pool1", "-pool");
        }
        private string GetStartCommand(string btcAdress, string worker)
        {
            IsInBenchmark = false;
            var param = "";
            bool zilEnabled = false;
            bool zilPoolEnabled = false;
            DeviceType devtype = DeviceType.NVIDIA;
            foreach (var pair in MiningSetup.MiningPairs)
            {
                devtype = pair.Device.DeviceType;
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

            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Nanominer, devtype))
            {
                ZilClient.needConnectionZIL = true;
                ZilClient.StartZilMonitor();
            }

            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Nanominer, devtype) &&
                ConfigManager.GeneralConfig.ZIL_mining_state == 1)
            {
                zilEnabled = true;
                zilPoolEnabled = false;
            }
            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Nanominer, devtype) &&
                ConfigManager.GeneralConfig.ZIL_mining_state == 2)
            {
                zilEnabled = true;
                zilPoolEnabled = true;
            }


            try
            {
                if (File.Exists("miners\\Nanominer\\config_nh_" + platform + ".ini"))
                    File.Delete("miners\\Nanominer\\config_nh_" + platform + ".ini");
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetStartCommand", ex.ToString());
            }
            string username = GetUsername(btcAdress, worker);
            string rigName = username.Split('.')[1];

            string cfgFile = "";
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                try
                {
                    if (File.Exists("miners\\Nanominer\\" + GetLogFileName()))
                        File.Delete("miners\\Nanominer\\" + GetLogFileName());
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("GetStartCommand", ex.ToString());
                }
                cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("logPath=" + GetLogFileName() + "\n")
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + String.Format(param) + "\n"
                   + String.Format("[Ethash]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + GetServer("daggerhashimoto", username, "3353");

                if (ConfigManager.GeneralConfig.StaleProxy)
                {
                    cfgFile = cfgFile + "rigPassword = stale\n";
                }
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash))
            {
                try
                {
                    if (File.Exists("miners\\Nanominer\\" + GetLogFileName()))
                        File.Delete("miners\\Nanominer\\" + GetLogFileName());
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("GetStartCommand", ex.ToString());
                }
                cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("logPath=" + GetLogFileName() + "\n")
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + String.Format(param) + "\n"
                   + String.Format("[Etchash]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + GetServer("etchash", username, "3393");

                if (ConfigManager.GeneralConfig.StaleProxy)
                {
                    cfgFile = cfgFile + "rigPassword = stale\n";
                }
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                try
                {
                    if (File.Exists("miners\\Nanominer\\" + GetLogFileName()))
                        File.Delete("miners\\Nanominer\\" + GetLogFileName());
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("GetStartCommand", ex.ToString());
                }
                cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("logPath=" + GetLogFileName() + "\n")
                   + String.Format(param) + "\n"
                   + String.Format("[autolykos]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + GetServer("autolykos", username, "3390");
                if (ConfigManager.GeneralConfig.StaleProxy)
                {
                    cfgFile = cfgFile + "rigPassword = stale\n";
                }
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW))
            {
                try
                {
                    if (File.Exists("miners\\Nanominer\\" + GetLogFileName()))
                        File.Delete("miners\\Nanominer\\" + GetLogFileName());
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("GetStartCommand", ex.ToString());
                }
                cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("logPath=" + GetLogFileName() + "\n")
                   + String.Format(param) + "\n"
                   + String.Format("coin = RVN\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + GetServer("kawpow", username, "3385");
                if (ConfigManager.GeneralConfig.StaleProxy)
                {
                    cfgFile = cfgFile + "rigPassword = stale\n";
                }
            }

            try
            {
                FileStream fs = new FileStream("miners\\Nanominer\\config_nh_" + platform + ".ini", FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(fs);
                w.WriteAsync(cfgFile);
                w.Flush();
                w.Close();
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("GetStartCommand", e.ToString());
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && zilEnabled && !zilPoolEnabled)
            {
                try
                {
                    if (File.Exists("miners\\Nanominer\\" + GetLogFileName()))
                        File.Delete("miners\\Nanominer\\" + GetLogFileName());
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("GetStartCommand", ex.ToString());
                }
                cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("logPath=" + GetLogFileName() + "\n")
                   + String.Format(param) + "\n"
                   + String.Format("[autolykos]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + String.Format("protocol = stratum\n")
                   + GetServer("autolykos", username, "3390") + "\n"
                + String.Format("[zil]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + String.Format("protocol = stratum\n")
                   + String.Format("zilEpoch = 1\n")
                   + GetServer("daggerhashimoto", username, "3353");
                if (ConfigManager.GeneralConfig.StaleProxy)
                {
                    cfgFile = cfgFile + "rigPassword = stale\n";
                }
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && zilEnabled && zilPoolEnabled)
            {
                try
                {
                    if (File.Exists("miners\\Nanominer\\" + GetLogFileName()))
                        File.Delete("miners\\Nanominer\\" + GetLogFileName());
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("GetStartCommand", ex.ToString());
                }
                cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("logPath=" + GetLogFileName() + "\n")
                   + String.Format(param) + "\n"
                   + String.Format("[autolykos]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + String.Format("protocol = stratum\n")
                   + GetServer("autolykos", username, "3390") + "\n"
                + String.Format("[zil]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", ConfigManager.GeneralConfig.ZIL_mining_wallet) + "\n"
                   + String.Format("rigName = \"{0}\"", worker) + "\n"
                   + String.Format("protocol = stratum\n")
                   + String.Format("zilEpoch = 1\n")
                   + "pool1 = " + ConfigManager.GeneralConfig.ZIL_mining_pool.Replace("stratum+tcp://", "") + 
                   ":" + ConfigManager.GeneralConfig.ZIL_mining_port + "\n";
                if (ConfigManager.GeneralConfig.StaleProxy)
                {
                    cfgFile = cfgFile + "rigPassword = stale\n";
                }
            }
            try
            {
                FileStream fs = new FileStream("miners\\Nanominer\\config_nh_" + platform + ".ini", FileMode.Create, FileAccess.Write);
                StreamWriter w = new StreamWriter(fs);
                w.WriteAsync(cfgFile);
                w.Flush();
                w.Close();
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("GetStartCommand", e.ToString());
            }

            return " config_nh_" + platform + ".ini";
        }


        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            var ids = new List<string>();
            var amdDeviceCount = ComputeDeviceManager.Query.AmdDevices.Count;
            var intelDeviceCount = ComputeDeviceManager.Query.IntelDevices.Count;
            var nvidiaDeviceCount = ComputeDeviceManager.Query._cudaDevices.CudaDevices.Count;
            var allDeviceCount = ComputeDeviceManager.Query.GpuCount;
            Helpers.ConsolePrint("NanominerIndexing", "platform: " + platform);

            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
            if (Form_Main.NVIDIA_orderBug)
            {
                sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
            }

            Helpers.ConsolePrint("NanominerIndexing", $"Found {allDeviceCount} Total GPU devices");
            Helpers.ConsolePrint("NanominerIndexing", $"Found {nvidiaDeviceCount} NVIDIA devices");
            Helpers.ConsolePrint("NanominerIndexing", $"Found {amdDeviceCount} AMD devices");
            Helpers.ConsolePrint("NanominerIndexing", $"Found {intelDeviceCount} INTEL devices");
            if (platform.Contains("amd"))
            {
                foreach (var mPair in sortedMinerPairs)
                {
                    //int id = (int)mPair.Device.lolMinerBusID + intelDeviceCount + nvidiaDeviceCount;
                    int id = (int)mPair.Device.lolMinerBusID;

                    if (id < 0)
                    {
                        Helpers.ConsolePrint("NanominerIndexing", "ID too low: " + id + " skipping device");
                        continue;
                    }
                    /*
                    if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                    {
                        Helpers.ConsolePrint("NanominerIndexing", "NVIDIA found. Increasing index");
                        id++;
                    }
                    if (mPair.Device.DeviceType == DeviceType.INTEL)
                    {
                        Helpers.ConsolePrint("NanominerIndexing", "INTEL found. Increasing index");
                        id++;
                    }
                    */
                    Helpers.ConsolePrint("NanominerIndexing", "Mining ID: " + id);
                    {
                        //devices[dev] = id.ToString();
                        ids.Add(id.ToString());
                        //dev++;
                    }

                }
                deviceStringCommand += string.Join(",", ids);
            }
            if (platform.Contains("intel"))
            {
                foreach (var mPair in sortedMinerPairs)
                {
                    int id = (int)mPair.Device.ID;

                    if (id < 0)
                    {
                        Helpers.ConsolePrint("NanominerIndexing", "ID too low: " + id + " skipping device");
                        continue;
                    }

                    Helpers.ConsolePrint("NanominerIndexing", "Mining ID: " + id);
                    {
                        //devices[dev] = id.ToString();
                        //dev++;
                        ids.Add(id.ToString());
                    }

                }
                deviceStringCommand += string.Join(",", ids);
            }
            if (platform.Contains("nvidia"))
            {
                foreach (var mPair in sortedMinerPairs)
                {
                    int id = mPair.Device.IDByBus;

                    if (id < 0)
                    {
                        Helpers.ConsolePrint("NanominerIndexing", "ID too low: " + id + " skipping device");
                        continue;
                    }

                    Helpers.ConsolePrint("NanominerIndexing", "Mining ID: " + id);
                    {
                        //devices[dev] = id.ToString();
                        //dev++;
                    }
                }
                var ids2 = MiningSetup.MiningPairs.Select(mPair => (mPair.Device.lolMinerBusID).ToString()).ToList();
                deviceStringCommand += string.Join(",", ids2);
            }
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

        protected bool IsProcessExist()
        {
            foreach (var process in Process.GetProcessesByName("nanominer"))
            {
                using (ManagementObject mo = new ManagementObject("win32_process.handle='" + process.Id.ToString() + "'"))
                {
                    mo.Get();
                    if (Convert.ToInt32(mo["ParentProcessId"]) == ProcessHandle.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override string BenchmarkCreateCommandLine(Algorithm algorithm, int time)
        {
            IsInBenchmark = true;

            if (Form_Main.nanominerCount > 0)
            {
                do
                {
                    Thread.Sleep(1000);
                } while (Form_Main.nanominerCount > 0);
            }
            Form_Main.nanominerCount++;

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

            try
            {
                if (File.Exists("miners\\Nanominer\\bench_nh_" + platform + GetDevicesCommandString().Trim(' ') + ".ini"))
                    File.Delete("miners\\Nanominer\\bench_nh_" + platform + GetDevicesCommandString().Trim(' ') + ".ini");

                if (File.Exists("miners\\Nanominer\\bench_nh_second_" + platform + GetDevicesCommandString().Trim(' ') + ".ini"))
                    File.Delete("miners\\Nanominer\\bench_nh_second_" + platform + GetDevicesCommandString().Trim(' ') + ".ini");
            }
            catch (Exception)
            {

            }

            if (algorithm.NiceHashID == AlgorithmType.DaggerHashimoto)
            {
                var cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).TrimStart(' ') + (char)10
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA).TrimStart(' ') + (char)10
                   + String.Format("[Ethash]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString().Trim(' ')) + "\n"
                   + String.Format("wallet = 0x266b27bd794d1A65ab76842ED85B067B415CD505") + "\n"
                   + String.Format("rigName = Nanominer") + "\n"
                   + String.Format("pool1 = " + Links.CheckDNS("stratum+tcp://ethw.2miners.com:2020").Replace("stratum+tcp://", "")) + "\n";

                try
                {
                    FileStream fs = new FileStream("miners\\Nanominer\\bench_nh_" + platform + GetDevicesCommandString().Trim(' ') + ".ini", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.WriteAsync(cfgFile);
                    w.Flush();
                    w.Close();
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("GetStartCommand", e.ToString());
                }
                //Thread.Sleep(1000);
                _benchmarkTimeWait = time;
            }

            if (algorithm.NiceHashID == AlgorithmType.ETCHash)
            {
                var cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.INTEL).TrimStart(' ') + (char)10
                   + String.Format("[Etchash]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString().Trim(' ')) + "\n"
                   + String.Format("wallet = 0x266b27bd794d1A65ab76842ED85B067B415CD505") + "\n"
                   + String.Format("rigName = Nanominer") + "\n"
                   + String.Format("pool1 = " + Links.CheckDNS("stratum+tcp://etc.2miners.com:1010").Replace("stratum+tcp://", "")) + "\n";

                try
                {
                    FileStream fs = new FileStream("miners\\Nanominer\\bench_nh_" + platform + GetDevicesCommandString().Trim(' ') + ".ini", FileMode.Create, FileAccess.Write);
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

            if (algorithm.NiceHashID == AlgorithmType.Autolykos)
            {
                var cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).TrimStart(' ') + (char)10
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA).TrimStart(' ') + (char)10
                   + String.Format("[autolykos]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString().Trim(' ')) + "\n"
                   + String.Format("wallet = 9gnVDaLeFa4ETwtrceHepPe9JeaCBGV1PxV5tdNGAvqEmjWF2Lt") + "\n"
                   + String.Format("rigName = Nanominer") + "\n"
                   + String.Format("pool1 = " + Links.CheckDNS("stratum+tcp://pool.woolypooly.com:3100").Replace("stratum+tcp://", "")) + "\n";

                try
                {
                    FileStream fs = new FileStream("miners\\Nanominer\\bench_nh_" + platform + GetDevicesCommandString().Trim(' ') + ".ini", FileMode.Create, FileAccess.Write);
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

            if (algorithm.NiceHashID == AlgorithmType.KAWPOW)
            {
                var cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).TrimStart(' ') + (char)10
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA).TrimStart(' ') + (char)10
                   + String.Format("[kawpow]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString().Trim(' ')) + "\n"
                   + String.Format("wallet = RHzovwc8c2mYvEC3MVwLX3pWfGcgWFjicX") + "\n"
                   + String.Format("rigName = Nanominer") + "\n"
                   + String.Format("pool1 = " + Links.CheckDNS("stratum+tcp://rvn.2miners.com:6060").Replace("stratum+tcp://", "")) + "\n";

                try
                {
                    FileStream fs = new FileStream("miners\\Nanominer\\bench_nh_" + platform + GetDevicesCommandString().Trim(' ') + ".ini", FileMode.Create, FileAccess.Write);
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

            return " bench_nh_" + platform + GetDevicesCommandString().Trim(' ') + ".ini";

        }

        private static void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        public override void EndBenchmarkProcces()
        {
            if (BenchmarkProcessStatus != BenchmarkProcessStatus.Killing && BenchmarkProcessStatus != BenchmarkProcessStatus.DoneKilling)
            {
                BenchmarkProcessStatus = BenchmarkProcessStatus.Killing;
                try
                {
                    Helpers.ConsolePrint("BENCHMARK",
                        $"Trying to kill benchmark process {BenchmarkProcessPath} algorithm {BenchmarkAlgorithm.AlgorithmName}");

                    int k = ProcessTag().IndexOf("pid(");
                    int i = ProcessTag().IndexOf(")|bin");
                    var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();

                    int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                    Helpers.ConsolePrint("BENCHMARK", "nanominer.exe PID: " + pid.ToString());
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
                }
            }
        }

        protected override void BenchmarkThreadRoutine(object commandLine)
        {
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;
            double repeats = 0.0d;
            double summspeed = 0.0d;

            int delay_before_calc_hashrate = 10;
            int MinerStartDelay = 20;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                double BenchmarkSpeed = 0.0d;
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in: " + _benchmarkTimeWait + " seconds");
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
                        var imageName = MinerExeName.Replace(".exe", "");
                        // maybe will have to KILL process
                        BenchmarkHandle.Kill();
                        BenchmarkHandle.Dispose();
                        EndBenchmarkProcces();
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

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        _powerUsage += _power;
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString() + " power: " + _power.ToString());
                            summspeed += ad.Result.Speed;
                        }
                        else
                        {
                            Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                        }

                        if (repeats >= _benchmarkTimeWait - MinerStartDelay - 15)
                        {
                            BenchmarkSpeed = Math.Round(summspeed / (repeats - delay_before_calc_hashrate), 2);
                            Helpers.ConsolePrint(MinerTag(), "Benchmark ended. BenchmarkSpeed: " + BenchmarkSpeed.ToString());
                            ad.Dispose();
                            benchmarkTimer.Stop();

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            Form_Main.nanominerCount = 0;
                            break;
                        }

                    }
                }
                
                BenchmarkAlgorithm.BenchmarkSpeed = BenchmarkSpeed;
                BenchmarkAlgorithm.PowerUsageBenchmark = (_powerUsage / repeats);
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {
                EndBenchmarkProcces();
                BenchmarkThreadRoutineFinish();
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

        private ApiData ad;
        public override ApiData GetApiData()
        {
            return ad;
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            /*
            if (hashrateErrorCount > 12)
            {
                hashrateErrorCount = 0;
                Helpers.ConsolePrint(MinerTag(), "Need Restart nanominer due API error");
                CurrentMinerReadStatus = MinerApiReadStatus.RESTART;
                ad.Speed = 0;
                ad.SecondarySpeed = 0;
                ad.ThirdSpeed = 0;
                return ad;
            }
            */
            CurrentMinerReadStatus = MinerApiReadStatus.WAIT;
            int dSpeed1 = 0;
            int dSpeed2 = 0;
            int gpu_hr = 0;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
            if (Form_Main.NVIDIA_orderBug)
            {
                sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
            }
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/stats");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 5 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 5 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromNanominer = await Reader.ReadToEndAsync();
                Reader.Close();
                Response.Close();
                //Helpers.ConsolePrint("API", ResponseFromNanominer);
            }
            catch (Exception ex)
            {
                hashrateErrorCount++;
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                Helpers.ConsolePrint("API", ex.Message);
                return null;
            }

            ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType, MiningSetup.MiningPairs[0]);

            bool zilEnabled = false;
            DeviceType devtype = DeviceType.NVIDIA;
            foreach (var pair in MiningSetup.MiningPairs)
            {
                devtype = pair.Device.DeviceType;
            }
            if (Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Nanominer, devtype))
            {
                zilEnabled = true;
            }

            try
            {
                int i = 0;
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
                {
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;
                    var cSpeed1 = (json.Algorithms[0].Ethash);
                    if (cSpeed1 == null) return ad;
                    var cSpeed = (json.Algorithms[0].Ethash.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);

                    foreach (var mPair in sortedMinerPairs)
                    {
                        string gpu = mPair.Device.lolMinerBusID.ToString();
                        string token = $"Algorithms[0].Ethash.GPU{gpu}.Hashrate";
                        var hash = (string)json.SelectToken(token);
                        gpu_hr = (int)Convert.ToDouble(hash, CultureInfo.InvariantCulture.NumberFormat);
                        sortedMinerPairs[i].Device.MiningHashrate = gpu_hr;
                        _power = mPair.Device.PowerUsage;
                        //Helpers.ConsolePrint("API", "dev: " + i.ToString() + " hr: " + gpu_hr.ToString());
                        i++;
                    }
                }
                i = 0;
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.ETCHash) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
                {
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;
                    var cSpeed1 = (json.Algorithms[0].Etchash);
                    if (cSpeed1 == null) return ad;
                    var cSpeed = (json.Algorithms[0].Etchash.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);

                    foreach (var mPair in sortedMinerPairs)
                    {
                        string gpu = mPair.Device.lolMinerBusID.ToString();
                        string token = $"Algorithms[0].Etchash.GPU{gpu}.Hashrate";
                        var hash = (string)json.SelectToken(token);
                        gpu_hr = (int)Convert.ToDouble(hash, CultureInfo.InvariantCulture.NumberFormat);
                        sortedMinerPairs[i].Device.MiningHashrate = gpu_hr;
                        _power = mPair.Device.PowerUsage;
                        //Helpers.ConsolePrint("API", "dev: " + i.ToString() + " hr: " + gpu_hr.ToString());
                        i++;
                    }
                }
                i = 0;
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
                {
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;
                    var cSpeed1 = (json.Algorithms[0].Autolykos);
                    if (cSpeed1 == null) return ad;
                    var cSpeed = (json.Algorithms[0].Autolykos.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);

                    foreach (var mPair in sortedMinerPairs)
                    {
                        string gpu = mPair.Device.lolMinerBusID.ToString();
                        string token = $"Algorithms[0].Autolykos.GPU{gpu}.Hashrate";
                        var hash = (string)json.SelectToken(token);
                        gpu_hr = (int)Convert.ToDouble(hash, CultureInfo.InvariantCulture.NumberFormat);
                        sortedMinerPairs[i].Device.MiningHashrate = gpu_hr;
                        //_power = sortedMinerPairs[i].Device.PowerUsage;
                        _power = mPair.Device.PowerUsage;
                        i++;
                    }
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.KAWPOW) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.NONE))
                {
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;
                    var cSpeed1 = (json.Algorithms[0].Kawpow);
                    if (cSpeed1 == null) return ad;
                    var cSpeed = (json.Algorithms[0].Kawpow.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);

                    foreach (var mPair in sortedMinerPairs)
                    {
                        string gpu = "";
                        if (platform.Contains("intel"))
                        {
                            gpu = mPair.Device.ID.ToString();
                        }
                        else
                        {
                            gpu = mPair.Device.lolMinerBusID.ToString();
                        }
                        string token = $"Algorithms[0].Kawpow.GPU{gpu}.Hashrate";
                        var hash = (string)json.SelectToken(token);
                        gpu_hr = (int)Convert.ToDouble(hash, CultureInfo.InvariantCulture.NumberFormat);
                        sortedMinerPairs[i].Device.MiningHashrate = gpu_hr;
                        //_power = sortedMinerPairs[i].Device.PowerUsage;
                        _power = mPair.Device.PowerUsage;
                        i++;
                    }
                }
                //dual mining
                i = 0;
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && zilEnabled && !IsInBenchmark)
                {
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;

                    var cSpeed1 = (json.Algorithms[0].Autolykos.Total.Hashrate);
                    var cSpeed2 = (json.Algorithms[0].Zilliqa.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed1, CultureInfo.InvariantCulture.NumberFormat);
                    dSpeed2 = (int)Convert.ToDouble(cSpeed2, CultureInfo.InvariantCulture.NumberFormat);

                    foreach (var mPair in sortedMinerPairs)
                    {
                        string gpu = mPair.Device.lolMinerBusID.ToString();
                        string token1 = $"Algorithms[0].Autolykos.GPU{gpu}.Hashrate";
                        string token2 = $"Algorithms[0].Zilliqa.GPU{gpu}.Hashrate";
                        var hash1 = (string)json.SelectToken(token1);
                        var hash2 = (string)json.SelectToken(token2);
                        var gpu_hr1 = (int)Convert.ToDouble(hash1, CultureInfo.InvariantCulture.NumberFormat);
                        var gpu_hr2 = (int)Convert.ToDouble(hash2, CultureInfo.InvariantCulture.NumberFormat);

                        if (Form_Main.isZilRound && Form_additional_mining.isAlgoZIL(MiningSetup.AlgorithmName, MinerBaseType.Nanominer, devtype))
                        {
                            mPair.Device.MiningHashrate = 0;
                            mPair.Device.MiningHashrateSecond = gpu_hr2;
                            mPair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                            mPair.Device.SecondAlgorithmID = (int)AlgorithmType.DaggerHashimoto;
                            mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                        }
                        else
                        {
                            mPair.Device.MiningHashrate = gpu_hr1;
                            mPair.Device.MiningHashrateSecond = 0;
                            mPair.Device.MiningHashrateThird = 0;
                            mPair.Device.AlgorithmID = (int)MiningSetup.CurrentAlgorithmType;
                            mPair.Device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                            mPair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                        }
                        //mPair.Device.State = Stats.DeviceState.Mining;
                        _power = mPair.Device.PowerUsage;
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                hashrateErrorCount++;
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                Helpers.ConsolePrint("API", ex.ToString());
                return null;
            }

            ad.ZilRound = false;
            ad.Speed = dSpeed1;
            ad.SecondarySpeed = 0;
            ad.ThirdSpeed = 0;
            ad.AlgorithmID = MiningSetup.CurrentAlgorithmType;
            ad.SecondaryAlgorithmID = AlgorithmType.NONE;
            ad.ThirdAlgorithmID = AlgorithmType.NONE;

            if (zilEnabled && dSpeed2 > 0)//+zil
            {
                ad.Speed = 0;
                ad.SecondarySpeed = dSpeed2;
                ad.ThirdSpeed = 0;
                ad.ZilRound = true;
                ad.AlgorithmID = AlgorithmType.NONE;
                ad.SecondaryAlgorithmID = AlgorithmType.DaggerHashimoto;
            }


            if (ad.Speed + ad.SecondarySpeed == 0)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                hashrateErrorCount++;
            }
            else
            {
                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;

            }

            Thread.Sleep(10);
            return ad;

        }
        protected override void _Stop(MinerStopType willswitch)
        {
            Helpers.ConsolePrint("Nanominer Stop", "");
            DeviceType devtype = DeviceType.AMD;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                devtype = mPair.Device.DeviceType;
            }

            fs.Close();
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5; // 5 minute max, whole waiting time 75seconds
        }
    }
}
