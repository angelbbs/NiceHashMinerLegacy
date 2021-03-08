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
using System.Management;
using NiceHashMiner.Devices;

namespace NiceHashMiner.Miners
{
    public class Nanominer : Miner
    {
        private int _benchmarkTimeWait = 180;
        private string[,] myServers = Form_Main.myServers;
        string ResponseFromNanominer;
        public string platform = "";

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

            var cfgFile =
               String.Format("webPort = {0}", ApiPort) + "\n"
               + String.Format("protocol = stratum\n")
               + String.Format(param) + "\n"
               + String.Format("[Ethash]\n")
               + String.Format("devices = {0}", GetDevicesCommandString()) + "\n"
               + String.Format("wallet = {0}", btcAdress) + "\n"
               + String.Format("rigName = \"{0}\"", rigName) + "\n"
               + String.Format("pool1 = {0}", url) + "\n"
               + String.Format("pool2 = daggerhashimoto.{0}.nicehash.com:3353", myServers[0, 0]) + "\n"
               + String.Format("pool3 = daggerhashimoto.{0}.nicehash.com:3353", myServers[1, 0]) + "\n"
               + String.Format("pool4 = daggerhashimoto.{0}.nicehash.com:3353", myServers[2, 0]) + "\n"
               + String.Format("pool5 = daggerhashimoto.{0}.nicehash.com:3353", myServers[3, 0]) + "\n";

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
            /*
            +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.AMD) +
                   ExtraLaunchParametersParser.ParseForMiningSetup(MiningSetup, DeviceType.NVIDIA);
                   */
        }


        protected override string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";
            var ids = new List<string>();
            var amdDeviceCount = ComputeDeviceManager.Query.AmdDevices.Count;
            var allDeviceCount = ComputeDeviceManager.Query.GpuCount;
            if (platform.Contains("amd"))
            {
                Helpers.ConsolePrint("lolMinerIndexing", $"Found {allDeviceCount} Total GPU devices");
                Helpers.ConsolePrint("lolMinerIndexing", $"Found {amdDeviceCount} AMD devices");
                var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.BusID).ToList();
                foreach (var mPair in sortedMinerPairs)
                {
                    double id = mPair.Device.IDByBus + allDeviceCount - amdDeviceCount;

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

                    Helpers.ConsolePrint("NanominerIndexing", "ID: " + id);
                    {
                        ids.Add(id.ToString());
                    }

                }
                deviceStringCommand += string.Join(",", ids);
            } else
            {
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
            Form_Main.nanominerCount++;
            if (Form_Main.nanominerCount >= Environment.ProcessorCount)
            {
                do
                {
                    Thread.Sleep(1000);
                } while (Form_Main.nanominerCount >= Environment.ProcessorCount);
            }

            var server = Globals.GetLocationUrl(algorithm.NiceHashID,
                Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], ConectionType).Replace("stratum+tcp://", "");
            var username = Globals.GetBitcoinUser();
            var rigName = ConfigManager.GeneralConfig.WorkerName.Trim();

            if (File.Exists("miners\\Nanominer\\bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini"))
                File.Delete("miners\\Nanominer\\bench_nh" + GetDevicesCommandString().Trim(' ') + ".ini");

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

            if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto)
            {
                var cfgFile =
                   String.Format("webPort = {0}", ApiPort) + "\n"
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
                    Form_Main.nanominerCount--;
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
        // stub benchmarks read from file
        protected override void BenchmarkOutputErrorDataReceivedImpl(string outdata)
        {
            CheckOutdata(outdata);
        }
        protected override bool BenchmarkParseLine(string outdata)
        {
            return true;
        }
        /*
        protected override bool BenchmarkParseLine(string outdata)
        {
            return false;
        }
        */
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
            int dSpeed = 0;
            var ad = new ApiData(MiningSetup.CurrentAlgorithmType);
            try
            {
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + ApiPort.ToString() + "/stats");
                WR.UserAgent = "GET / HTTP/1.1\r\n\r\n";
                WR.Timeout = 15 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 10 * 1000;
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
            try
            {
                //Helpers.ConsolePrint("API", ResponseFromNanominer);
                dynamic json = JsonConvert.DeserializeObject(ResponseFromNanominer);
                if (json == null) return ad;
                var cSpeed1 = (json.Algorithms[0].Ethash);
                if (cSpeed1 == null) return ad;
                var cSpeed = (json.Algorithms[0].Ethash.Total.Hashrate);
                dSpeed = (int)Convert.ToDouble(cSpeed, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("API", ex.Message);
                return null;
            }
            ad.Speed = dSpeed;
            if (ad.Speed == 0)
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
