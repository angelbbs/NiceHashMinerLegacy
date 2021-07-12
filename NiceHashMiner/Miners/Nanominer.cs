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
using System.Threading;
using System.Threading.Tasks;
using NiceHashMiner.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using System.Windows.Forms;
using System.Net;
using System.Management;
using NiceHashMiner.Devices;

namespace NiceHashMiner.Miners
{
    public class Nanominer : Miner
    {
        private int _benchmarkTimeWait = 180;
        string ResponseFromNanominer;
        public string platform = "";
        public string[] devices;
        public FileStream fs;
        private int offset = 0;
        private bool zilRound = false;
        private bool IsInBenchmark = false;

        public Nanominer() : base("Nanominer")
        {
            ConectionType = NhmConectionType.NONE;
        }

        public override void Start(string url, string btcAdress, string worker)
        {
            IsApiReadException = false;
            LastCommandLine = GetStartCommand(url, btcAdress, worker);
            ProcessHandle = _Start();
            
            do
            {
                Thread.Sleep(1000);
            } while (!File.Exists("miners\\Nanominer\\" + GetLogFileName()));
            Thread.Sleep(1000);
            fs = new FileStream("miners\\Nanominer\\" + GetLogFileName(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            
        }

        private string GetStartCommand(string url, string btcAdress, string worker)
        {
            IsInBenchmark = false;
            var param = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    platform = "nvidia";
                    param = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA).Trim();
                }
                else
                {
                    platform = "amd";
                    param = ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).Trim();
                }
            }

            if (File.Exists("miners\\Nanominer\\config_nh_" + platform +".ini"))
                File.Delete("miners\\Nanominer\\config_nh_" + platform + ".ini");

            string username = GetUsername(btcAdress, worker);
            string rigName = username.Split('.')[1];
            url = url.Replace("stratum+tcp://", "");
            string cfgFile = "";
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("protocol = stratum\n")
                   + String.Format(param) + "\n"
                   + String.Format("[Ethash]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + String.Format("pool1 = {0}", url) + "\n"
                   + String.Format("pool2 = daggerhashimoto.{0}.nicehash.com:3353", Form_Main.myServers[0, 0]) + "\n"
                   + String.Format("pool3 = daggerhashimoto.{0}.nicehash.com:3353", Form_Main.myServers[1, 0]) + "\n"
                   + String.Format("pool4 = daggerhashimoto.{0}.nicehash.com:3353", Form_Main.myServers[2, 0]) + "\n"
                   + String.Format("pool5 = daggerhashimoto.{0}.nicehash.com:3353", Form_Main.myServers[3, 0]) + "\n";
            }
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
            {
                if (File.Exists("miners\\Nanominer\\" + GetLogFileName()))
                    File.Delete("miners\\Nanominer\\" + GetLogFileName());

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
                   + String.Format("pool1 = {0}", url) + "\n"
                   + String.Format("pool2 = autolykos.{0}.nicehash.com:3390", Form_Main.myServers[0, 0]) + "\n"
                   + String.Format("pool3 = autolykos.{0}.nicehash.com:3390", Form_Main.myServers[1, 0]) + "\n"
                   + String.Format("pool4 = autolykos.{0}.nicehash.com:3390", Form_Main.myServers[2, 0]) + "\n"
                   + String.Format("pool5 = autolykos.{0}.nicehash.com:3390", Form_Main.myServers[3, 0]) + "\n";
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
            if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
            {
                if (File.Exists("miners\\Nanominer\\" + GetLogFileName()))
                    File.Delete("miners\\Nanominer\\" + GetLogFileName());

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
                   + String.Format("pool1 = {0}", url) + "\n"
                   + String.Format("pool2 = autolykos.{0}.nicehash.com:3390", Form_Main.myServers[0, 0]) + "\n"
                   + String.Format("pool3 = autolykos.{0}.nicehash.com:3390", Form_Main.myServers[1, 0]) + "\n"
                   + String.Format("pool4 = autolykos.{0}.nicehash.com:3390", Form_Main.myServers[2, 0]) + "\n"
                   + String.Format("pool5 = autolykos.{0}.nicehash.com:3390", Form_Main.myServers[3, 0]) + "\n"
                   + String.Format("[zil]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
                   + String.Format("wallet = {0}", btcAdress) + "\n"
                   + String.Format("rigName = \"{0}\"", rigName) + "\n"
                   + String.Format("zilEpoch = 0\n")
               //    + String.Format("protocol = JSON-RPC\n")
                   + String.Format("pool1 = daggerhashimoto.{0}.nicehash.com:3353", Form_Main.myServers[0, 0]) + "\n"
                   + String.Format("pool2 = daggerhashimoto.{0}.nicehash.com:3353", Form_Main.myServers[1, 0]) + "\n"
                   + String.Format("pool3 = daggerhashimoto.{0}.nicehash.com:3353", Form_Main.myServers[2, 0]) + "\n"
                   + String.Format("pool4 = daggerhashimoto.{0}.nicehash.com:3353", Form_Main.myServers[3, 0]) + "\n";
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
            var allDeviceCount = ComputeDeviceManager.Query.GpuCount;
            Helpers.ConsolePrint("NanominerIndexing", "platform: " + platform);
            int dev = 0;
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
            devices = new string[sortedMinerPairs.Count];
            if (platform.Contains("amd"))
            {
                Helpers.ConsolePrint("NanominerIndexing", $"Found {allDeviceCount} Total GPU devices");
                Helpers.ConsolePrint("NanominerIndexing", $"Found {amdDeviceCount} AMD devices");

                foreach (var mPair in sortedMinerPairs)
                {
                    Helpers.ConsolePrint("NanominerIndexing", "Index: " + mPair.Device.Index);
                    Helpers.ConsolePrint("NanominerIndexing", "Name: " + mPair.Device.Name);
                    Helpers.ConsolePrint("NanominerIndexing", "ID: " + mPair.Device.ID);
                    Helpers.ConsolePrint("NanominerIndexing", "IDbybus: " + mPair.Device.IDByBus);
                    Helpers.ConsolePrint("NanominerIndexing", "busid: " + mPair.Device.BusID);
                    Helpers.ConsolePrint("NanominerIndexing", "lol: " + mPair.Device.lolMinerBusID);
                    //int id = mPair.Device.IDByBus + allDeviceCount - amdDeviceCount;
                    int id = (int)mPair.Device.lolMinerBusID;

                    if (id < 0)
                    {
                        Helpers.ConsolePrint("NanominerIndexing", "ID too low: " + id + " skipping device");
                        continue;
                    }

                    if (mPair.Device.DeviceType == DeviceType.NVIDIA)
                    {
                        Helpers.ConsolePrint("NanominerIndexing", "NVIDIA found. Increasing index");
                        id++;
                    }

                    Helpers.ConsolePrint("NanominerIndexing", "Mining ID: " + id);
                    {
                        devices[dev] = id.ToString();
                        dev++;
                        ids.Add(id.ToString());
                    }

                }
                deviceStringCommand += string.Join(",", ids);
            } else
            {
                foreach (var mPair in sortedMinerPairs)
                {
                    Helpers.ConsolePrint("NanominerIndexing", "Index: " + mPair.Device.Index);
                    Helpers.ConsolePrint("NanominerIndexing", "Name: " + mPair.Device.Name);
                    Helpers.ConsolePrint("NanominerIndexing", "ID: " + mPair.Device.ID);
                    Helpers.ConsolePrint("NanominerIndexing", "IDbybus: " + mPair.Device.IDByBus);
                    Helpers.ConsolePrint("NanominerIndexing", "busid: " + mPair.Device.BusID);
                    Helpers.ConsolePrint("NanominerIndexing", "lol: " + mPair.Device.lolMinerBusID);
                    int id = mPair.Device.IDByBus;

                    if (id < 0)
                    {
                        Helpers.ConsolePrint("NanominerIndexing", "ID too low: " + id + " skipping device");
                        continue;
                    }

                    Helpers.ConsolePrint("NanominerIndexing", "Mining ID: " + id);
                    {
                        devices[dev] = id.ToString();
                        dev++;
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
                else
                {
                    platform = "amd";
                }
            }

            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
                Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], ConectionType).Replace("stratum+tcp://", "");
            var username = Globals.GetBitcoinUser();
            var rigName = ConfigManager.GeneralConfig.WorkerName.Trim();

            if (File.Exists("miners\\Nanominer\\bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini"))
                File.Delete("miners\\Nanominer\\bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini");

            if (File.Exists("miners\\Nanominer\\bench_nh_second" + GetDevicesCommandString().Trim(' ') + ".ini"))
                File.Delete("miners\\Nanominer\\bench_nh_second" + GetDevicesCommandString().Trim(' ') + ".ini");

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
                   + String.Format("wallet = 0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1") + "\n"
                   + String.Format("rigName = Nanominer") + "\n"
                   + String.Format("pool1 = eu1.ethermine.org:4444") + "\n";

                try
                {
                    FileStream fs = new FileStream("miners\\Nanominer\\bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini", FileMode.Create, FileAccess.Write);
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

            if (algorithm.NiceHashID == AlgorithmType.Autolykos && algorithm.DualNiceHashID == AlgorithmType.Autolykos)
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
                   + String.Format("pool1 = pool.eu.woolypooly.com:3100") + "\n";

                try
                {
                    FileStream fs = new FileStream("miners\\Nanominer\\bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini", FileMode.Create, FileAccess.Write);
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
            if (algorithm.NiceHashID == AlgorithmType.Autolykos && algorithm.DualNiceHashID == AlgorithmType.AutolykosZil)
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
                   + String.Format("rigName = NanominerZil") + "\n"
                   + String.Format("pool1 = pool.eu.woolypooly.com:3100") + "\n";

                try
                {
                    FileStream fs = new FileStream("miners\\Nanominer\\bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.WriteAsync(cfgFile);
                    w.Flush();
                    w.Close();
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("GetStartCommand", e.ToString());
                }
                var cfgFile2 =
                   String.Format("webPort = {0}", ApiPort) + "\n"
                   + String.Format("mport = 0\n")
                   + String.Format("protocol = stratum\n")
                   + String.Format("watchdog = false\n")
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD).TrimStart(' ') + (char)10
                   + ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA).TrimStart(' ') + (char)10
                   + String.Format("[ethash]\n")
                   + String.Format("devices = {0}", GetDevicesCommandString().Trim(' ')) + "\n"
                   + String.Format("wallet = angelbbs") + "\n"
                   + String.Format("rigName = NanominerZil") + "\n"
                   + String.Format("pool1 = us-east.ethash-hub.miningpoolhub.com:20565") + "\n";

                try
                {
                    FileStream fs = new FileStream("miners\\Nanominer\\bench_nh_second" + GetDevicesCommandString().Trim(' ') + ".ini", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.WriteAsync(cfgFile2);
                    w.Flush();
                    w.Close();
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("GetStartCommand", e.ToString());
                }
                
                _benchmarkTimeWait = time;
            }

            return " bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini";

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
                    //BenchmarkHandle = null;
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
                        BenchmarkHandle.Kill();
                        BenchmarkHandle.Dispose();
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

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString());
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
                            EndBenchmarkProcces();

                            break;
                        }

                    }
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos) && MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    BenchmarkAlgorithm.BenchmarkProgressPercent = -1;
                    BenchmarkThreadRoutineSecond();
                }
                BenchmarkAlgorithm.BenchmarkSpeed = BenchmarkSpeed;
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {
                EndBenchmarkProcces();
                BenchmarkThreadRoutineFinish();
                Form_Main.nanominerCount--;
            }
        }
        private void BenchmarkThreadRoutineSecond()
        {
            double BenchmarkSpeed = 0.0d;
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;
            double repeats = 0.0d;
            double summspeedSecond = 0.0d;

            int delay_before_calc_hashrate = 10;
            int MinerStartDelay = 20;

            Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);

            try
            {
                Helpers.ConsolePrint("BENCHMARK", "Benchmark starts");
                Helpers.ConsolePrint(MinerTag(), "Benchmark should end in: " + _benchmarkTimeWait + " seconds");
                BenchmarkHandle = BenchmarkStartProcess(" bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini");
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
                        BenchmarkHandle.Kill();
                        BenchmarkHandle.Dispose();
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

                    var ad = GetSummaryAsync();
                    if (ad.Result != null && ad.Result.Speed > 0)
                    {
                        repeats++;
                        double benchProgress = repeats / (_benchmarkTimeWait - MinerStartDelay - 15);
                        BenchmarkAlgorithm.BenchmarkProgressPercent = (int)(benchProgress * 100);
                        if (repeats > delay_before_calc_hashrate)
                        {
                            Helpers.ConsolePrint(MinerTag(), "Useful API Speed: " + ad.Result.Speed.ToString());
                            summspeedSecond += ad.Result.Speed;
                        }
                        else
                        {
                            Helpers.ConsolePrint(MinerTag(), "Delayed API Speed: " + ad.Result.Speed.ToString());
                        }

                        if (repeats >= _benchmarkTimeWait - MinerStartDelay - 15)
                        {
                            BenchmarkSpeed = Math.Round(summspeedSecond / (repeats - delay_before_calc_hashrate), 2);
                            Helpers.ConsolePrint(MinerTag(), "Benchmark ended. BenchmarkSpeed: " + BenchmarkSpeed.ToString());
                            ad.Dispose();
                            benchmarkTimer.Stop();

                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Dispose();
                            //EndBenchmarkProcces();

                            break;
                        }

                    }
                }
                BenchmarkAlgorithm.BenchmarkSecondarySpeed = Math.Round(summspeedSecond / (repeats - delay_before_calc_hashrate), 2);
            }
            catch (Exception ex)
            {
                BenchmarkThreadRoutineCatch(ex);
            }
            finally
            {

                //BenchmarkThreadRoutineFinish();
                Form_Main.nanominerCount--;
            }
        }
        // stub benchmarks read from file
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }

        protected double GetNumber(string outdata)
        {
            return GetNumber(outdata, "Total speed: ", "H/s, Total shares");
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
            int dSpeed1 = 0;
            int dSpeed2 = 0;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType, MiningSetup.MiningPairs[0]);
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
                Helpers.ConsolePrint("API", ex.Message);
                return null;
            }
            try
            {
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.DaggerHashimoto))
                {
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;
                    var cSpeed1 = (json.Algorithms[0].Ethash);
                    if (cSpeed1 == null) return ad;
                    var cSpeed = (json.Algorithms[0].Ethash.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);

                    for (int i = 0; i < sortedMinerPairs.Count; i++)
                    {
                        string gpu = devices[i];
                        string token = $"Algorithms[0].Ethash.GPU{gpu}.Hashrate";
                        var hash = (string)json.SelectToken(token);
                        var gpu_hr = (int)Convert.ToDouble(hash, CultureInfo.InvariantCulture.NumberFormat);
                        sortedMinerPairs[i].Device.MiningHashrate = gpu_hr;
                    }
                }
                if (MiningSetup.CurrentAlgorithmType.Equals(AlgorithmType.Autolykos))
                {
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;
                    var cSpeed1 = (json.Algorithms[0].Autolykos);
                    if (cSpeed1 == null) return ad;
                    var cSpeed = (json.Algorithms[0].Autolykos.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);

                    for (int i = 0; i < sortedMinerPairs.Count; i++)
                    {
                        string gpu = devices[i];
                        string token = $"Algorithms[0].Autolykos.GPU{gpu}.Hashrate";
                        var hash = (string)json.SelectToken(token);
                        var gpu_hr = (int)Convert.ToDouble(hash, CultureInfo.InvariantCulture.NumberFormat);
                        sortedMinerPairs[i].Device.MiningHashrate = gpu_hr;
                    }
                }
                if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) && !IsInBenchmark)
                {
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;

                    var cSpeed1 = (json.Algorithms[0].Autolykos.Total.Hashrate);
                    var cSpeed2 = (json.Algorithms[0].Zilliqa.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed1, CultureInfo.InvariantCulture.NumberFormat);
                    dSpeed2 = (int)Convert.ToDouble(cSpeed2, CultureInfo.InvariantCulture.NumberFormat);

                    for (int i = 0; i < sortedMinerPairs.Count; i++)
                    {
                        string gpu = devices[i];
                        string token1 = $"Algorithms[0].Autolykos.GPU{gpu}.Hashrate";
                        string token2 = $"Algorithms[0].Zilliqa.GPU{gpu}.Hashrate";
                        var hash1 = (string)json.SelectToken(token1);
                        var hash2 = (string)json.SelectToken(token2);
                        var gpu_hr1 = (int)Convert.ToDouble(hash1, CultureInfo.InvariantCulture.NumberFormat);
                        var gpu_hr2 = (int)Convert.ToDouble(hash2, CultureInfo.InvariantCulture.NumberFormat);
                        //костыль из-за бага в api nanominer
                        if (fs.Length > offset)
                        {
                            int count = (int)(fs.Length - offset);
                            byte[] array = new byte[count];
                            fs.Read(array, 0, count);
                            offset = (int)fs.Length;
                            string textFromFile = System.Text.Encoding.Default.GetString(array).Trim();
                            //Helpers.ConsolePrint(MinerTag(), textFromFile);

                            string strStart = "Zilliqa - Total speed:";
                            if (textFromFile.Contains(strStart) && textFromFile.Contains("H/s"))
                            {
                                var speedStart = textFromFile.IndexOf(strStart);
                                var speed = textFromFile.Substring(speedStart + strStart.Length, 6);
                                speed = speed.Replace(strStart, "");
                                speed = speed.Replace(" ", "");
                                double.TryParse(speed, out var zilSpeed);
                                if (zilSpeed > 0)
                                {
                                    zilRound = true;
                                    dSpeed1 = 0;
                                } else
                                {
                                    zilRound = false;
                                    dSpeed2 = 0;
                                }
                            }
                            if (textFromFile.Contains("DAG generated"))
                            {
                                zilRound = true;
                                dSpeed1 = 0;
                            }
                            if (textFromFile.Contains("Ergo - SHARE FOUND"))
                            {
                                zilRound = false;
                                dSpeed2 = 0;
                            }
                        }
                        //
                        if (gpu_hr2 > 0 && zilRound)
                        {
                            sortedMinerPairs[i].Device.MiningHashrate = gpu_hr2;
                            dSpeed1 = 0;
                        }
                        else
                        {
                            sortedMinerPairs[i].Device.MiningHashrate = gpu_hr1;
                            dSpeed2 = 0;
                        }
                    }
                }
                //for benchmark
                if (MiningSetup.CurrentSecondaryAlgorithmType.Equals(AlgorithmType.DaggerHashimoto) && IsInBenchmark)
                {
                    bool IsZil = false;
                    if (ResponseFromNanominer.Contains("Zilliqa"))
                    {
                        IsZil = true;
                    }
                    dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer.Replace("GPU ", "GPU"));
                    if (json == null) return ad;
                    var cSpeed1 = (json.Algorithms[0].Autolykos);
                    if (cSpeed1 == null) return ad;
                    var cSpeed = (json.Algorithms[0].Autolykos.Total.Hashrate);
                    dSpeed1 = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);

                    for (int i = 0; i < sortedMinerPairs.Count; i++)
                    {
                        string gpu = devices[i];
                        string token = "";
                        if (IsZil)
                        {
                            token = $"Algorithms[0].Zilliqa.GPU{gpu}.Hashrate";
                        }
                        else
                        {
                            token = $"Algorithms[0].Autolykos.GPU{gpu}.Hashrate";
                        }
                        var hash = (string)json.SelectToken(token);
                        var gpu_hr = (int)Convert.ToDouble(hash, CultureInfo.InvariantCulture.NumberFormat);
                        sortedMinerPairs[i].Device.MiningHashrate = gpu_hr;
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API", ex.ToString());
                return null;
            }
            ad.Speed = dSpeed1;
            ad.SecondarySpeed = dSpeed2;
            if (ad.Speed + ad.SecondarySpeed == 0)
            {
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
            }
            else
            {
                CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
            }

            Thread.Sleep(10);
            return ad;

        }
        //ethman api тоже не работает
        /*
        public bool IsDual()
        {
            return (MiningSetup.CurrentSecondaryAlgorithmType != AlgorithmType.NONE);
        }
        private class JsonApiResponse
        {
#pragma warning disable IDE1006 // Naming Styles
            public List<string> result { get; set; }
            public int id { get; set; }
            public object error { get; set; }
#pragma warning restore IDE1006 // Naming Styles
        }
        public override async Task<ApiData> GetSummaryAsync()
        {
            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType, MiningSetup.CurrentSecondaryAlgorithmType, MiningSetup.MiningPairs[0]);

            JsonApiResponse resp = null;
            try
            {
                var bytesToSend = Encoding.ASCII.GetBytes("{\"id\":0,\"jsonrpc\":\"2.0\",\"method\":\"miner_getstat1\"}\r\n");
                var client = new TcpClient("127.0.0.1", ApiPort);
                var nwStream = client.GetStream();
                nwStream.ReadTimeout = 2 * 1000;
                nwStream.WriteTimeout = 2 * 1000;
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
                var bytesToRead = new byte[client.ReceiveBufferSize];
                var bytesRead = await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                var respStr = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                resp = JsonConvert.DeserializeObject<JsonApiResponse>(respStr, Globals.JsonSettings);
                client.Close();
                Helpers.ConsolePrint("ClaymoreMiner API: ", respStr);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), "GetSummary exception: " + ex.Message);
            }

            if (resp != null && resp.error == null)
            {
                if (resp.result != null)
                {
                    var speeds = resp.result[3].Split(';');
                    var secondarySpeeds = (IsDual()) ? resp.result[5].Split(';') : new string[0];

                    ad.Speed = 0;
                    ad.SecondarySpeed = 0;

                    var sortedMinerPairs = MiningSetup.MiningPairs.OrderByDescending(pair => pair.Device.DeviceType)
                                .ThenBy(pair => pair.Device.IDByBus).ToList();
                    if (Form_Main.NVIDIA_orderBug)
                    {
                        sortedMinerPairs.Sort((a, b) => a.Device.ID.CompareTo(b.Device.ID));
                    }
                    int dev = 0;
                    foreach (var speed in speeds)
                    {
                        Helpers.ConsolePrint("API: ", "speed: " + speed);
                        double tmpSpeed;
                        try
                        {
                            tmpSpeed = double.Parse(speed, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            tmpSpeed = 0;
                        }
                        Helpers.ConsolePrint("API: ", "tmpSpeed" + tmpSpeed.ToString());
                        
                        if (!speed.Contains("off"))
                        {
                            sortedMinerPairs[dev].Device.MiningHashrate = tmpSpeed;
                            dev++;
                        }
                        ad.Speed += tmpSpeed;
                    }

                    foreach (var speed in secondarySpeeds)
                    {
                        Helpers.ConsolePrint("API: ", "secondarySpeeds " + secondarySpeeds);
                        double tmpSpeed;
                        try
                        {
                            tmpSpeed = double.Parse(speed, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            tmpSpeed = 0;
                        }
                        Helpers.ConsolePrint("API: ", "tmpSpeed2" + tmpSpeed.ToString());
                        ad.SecondarySpeed += tmpSpeed;
                        if (tmpSpeed > 0)
                        {
                            sortedMinerPairs[dev].Device.MiningHashrate = tmpSpeed;
                        }
                    }

                    CurrentMinerReadStatus = MinerApiReadStatus.GOT_READ;
                }

                if (ad.Speed + ad.SecondarySpeed == 0)
                {
                    CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                }

                if (ad.Speed < 0)
                {
                    Helpers.ConsolePrint(MinerTag(), "Reporting negative speeds will restart...");
                    Restart();
                }
            }

            if (fs.Length > offset)
            {
                int count = (int)(fs.Length - offset);
                byte[] array = new byte[count];
                fs.Read(array, 0, count);

                offset = (int)fs.Length;
                string textFromFile = System.Text.Encoding.Default.GetString(array).Trim();
                Helpers.ConsolePrint(MinerTag(), textFromFile);
            }
            return ad;
        }
        */
        protected override void _Stop(MinerStopType willswitch)
        {
            fs.Close();
            Stop_cpu_ccminer_sgminer_nheqminer(willswitch);
        }

        protected override int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 5; // 5 minute max, whole waiting time 75seconds
        }
    }
}
