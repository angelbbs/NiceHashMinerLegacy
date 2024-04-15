using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Forms;
using NiceHashMiner.Interfaces;
using NiceHashMiner.Miners;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Stats;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace NiceHashMiner
{
    public class ApiData
    {
        public AlgorithmType AlgorithmID;
        public AlgorithmType SecondaryAlgorithmID;
        public AlgorithmType ThirdAlgorithmID;
        public string AlgorithmName;
        public string AlgorithmNameCustom;
        public double Speed;
        public double SecondarySpeed;
        public double ThirdSpeed;
        public double PowerUsage;
        public bool ZilRound;

        public ApiData(AlgorithmType algorithmID, AlgorithmType secondaryAlgorithmID = AlgorithmType.NONE, MiningPair mpairs = null)
        {
            AlgorithmID = algorithmID;
            SecondaryAlgorithmID = secondaryAlgorithmID;

            if (mpairs == null)
            {
                AlgorithmName = AlgorithmNiceHashNames.GetName(DualAlgorithmID());
            }
            else
            {
                if (mpairs.Algorithm is DualAlgorithm dualAlg)
                {
                    AlgorithmName = dualAlg.DualAlgorithmNameCustom;
                }
                else
                {
                    AlgorithmName = mpairs.Algorithm.AlgorithmNameCustom;
                }
            }

            Speed = 0.0;
            SecondarySpeed = 0.0;
            PowerUsage = 0.0;
        }
        public AlgorithmType DualAlgorithmID()
        {
            if (AlgorithmID == AlgorithmType.Autolykos)
            {
                switch (SecondaryAlgorithmID)
                {
                    case AlgorithmType.DaggerHashimoto:
                        return AlgorithmType.AutolykosZil;
                }
            }
            if (AlgorithmID == AlgorithmType.DaggerHashimoto)
            {
                switch (SecondaryAlgorithmID)
                {
                    case AlgorithmType.Autolykos:
                        return AlgorithmType.DaggerAutolykos;
                    case AlgorithmType.KAWPOW:
                        return AlgorithmType.DaggerKAWPOW;
                    case AlgorithmType.Octopus:
                        return AlgorithmType.DaggerOctopus;
                    case AlgorithmType.KarlsenHash:
                        return AlgorithmType.DaggerKarlsenHash;
                    case AlgorithmType.Alephium:
                        return AlgorithmType.DaggerAlephium;
                }
            }
            if (AlgorithmID == AlgorithmType.FishHash)
            {
                switch (SecondaryAlgorithmID)
                {
                    case AlgorithmType.KarlsenHash:
                        return AlgorithmType.FishHashKarlsenHash;
                    case AlgorithmType.Alephium:
                        return AlgorithmType.FishHashAlephium;
                    case AlgorithmType.PyrinHash:
                        return AlgorithmType.FishHashPyrinHash;
                }
            }
            if (AlgorithmID == AlgorithmType.ETCHash)
            {
                switch (SecondaryAlgorithmID)
                {
                    case AlgorithmType.KarlsenHash:
                        return AlgorithmType.ETCHashKarlsenHash;
                    case AlgorithmType.Alephium:
                        return AlgorithmType.ETCHashAlephium;
                }
            }
            if (AlgorithmID == AlgorithmType.Autolykos)
            {
                switch (SecondaryAlgorithmID)
                {
                    case AlgorithmType.KarlsenHash:
                        return AlgorithmType.AutolykosKarlsenHash;
                    case AlgorithmType.Alephium:
                        return AlgorithmType.AutolykosAlephium;
                    case AlgorithmType.PyrinHash:
                        return AlgorithmType.AutolykosPyrinHash;
                }
            }
            if (AlgorithmID == AlgorithmType.Octopus)
            {
                switch (SecondaryAlgorithmID)
                {
                    case AlgorithmType.KarlsenHash:
                        return AlgorithmType.OctopusKarlsenHash;
                    case AlgorithmType.Alephium:
                        return AlgorithmType.OctopusAlephium;
                    case AlgorithmType.PyrinHash:
                        return AlgorithmType.OctopusPyrinHash;
                }
            }

            return AlgorithmID;
        }

    }

    //
    public class MinerPidData
    {
        public string MinerBinPath;
        public int Pid = -1;
        public IntPtr DivertHandle;
    }

    public abstract class Miner
    {
        // MinerIDCount used to identify miners creation
        protected static long MinerIDCount { get; private set; }

        public NhmConectionType ConectionType { get; protected set; }

        // used to identify miner instance
        protected readonly long MinerID;

        private string _minerTag;
        public static string MinerDeviceName { get; set; }

        protected int ApiPort { get; set; }

        // if miner has no API bind port for reading curentlly only CryptoNight on ccminer
        public bool IsApiReadException { get; protected set; }

        public bool IsNeverHideMiningWindow { get; protected set; }

        // mining algorithm stuff
        protected bool IsInit { get; private set; }

        public MiningSetup MiningSetup { get; protected set; }

        // sgminer/zcash claymore workaround
        protected bool IsKillAllUsedMinerProcs { get; set; }


        public bool IsRunning { get; protected set; }
        public static bool IsRunningNew { get; protected set; }
        protected string Path { get; private set; }

        protected string LastCommandLine { get; set; }

        // TODO check this
        protected double PreviousTotalMH;

        // the defaults will be
        protected string WorkingDirectory { get; private set; }

        protected string MinerExeName { get; private set; }
        protected NiceHashProcess ProcessHandle;
        private MinerPidData _currentPidData;
        private readonly List<MinerPidData> _allPidData = new List<MinerPidData>();

        // Benchmark stuff
        public bool BenchmarkSignalQuit;

        public bool BenchmarkSignalHanged;
        private Stopwatch _benchmarkTimeOutStopWatch;
        public bool BenchmarkSignalTimedout;
        protected bool BenchmarkSignalFinnished;
        protected IBenchmarkComunicator BenchmarkComunicator;
        protected bool OnBenchmarkCompleteCalled;
        protected Algorithm BenchmarkAlgorithm { get; set; }
        public BenchmarkProcessStatus BenchmarkProcessStatus { get; protected set; }
        protected string BenchmarkProcessPath { get; set; }
        protected Process BenchmarkHandle { get; set; }
        protected Exception BenchmarkException;
        protected int BenchmarkTimeInSeconds;

        private string _benchmarkLogPath = "";
        protected List<string> BenchLines;

        protected bool TimeoutStandard;


        // TODO maybe set for individual miner cooldown/retries logic variables
        // this replaces MinerAPIGraceSeconds(AMD)
        private const int MinCooldownTimeInMilliseconds = 5 * 1000; // 30 seconds for gminer
        //private const int _MIN_CooldownTimeInMilliseconds = 1000; // TESTING

        //private const int _MAX_CooldownTimeInMilliseconds = 60 * 1000; // 1 minute max, whole waiting time 75seconds
        public int _maxCooldownTimeInMilliseconds; // = GetMaxCooldownTimeInMilliseconds();

        // protected abstract int GetMaxCooldownTimeInMilliseconds();
        private Timer _cooldownCheckTimer;
        protected MinerApiReadStatus CurrentMinerReadStatus { get; set; }
        private int _currentCooldownTimeInSeconds = MinCooldownTimeInMilliseconds;
        private int _currentCooldownTimeInSecondsLeft = MinCooldownTimeInMilliseconds;
        private int CooldownCheck = 0;

        private bool _isEnded;

        public bool IsUpdatingApi = false;
        public int TicksForApiUpdate = 0;

        protected const string HttpHeaderDelimiter = "\r\n\r\n";

        protected bool IsMultiType;
        public static string BenchmarkStringAdd = "";
        public static string InBenchmark = "";
        public bool needChildRestart;



        protected virtual int GetMaxCooldownTimeInMilliseconds()
        {
            return 60 * 1000 * 10;  // 10 min
        }

        protected Miner(string minerDeviceName)
        {
            ConectionType = NhmConectionType.STRATUM_TCP;
            MiningSetup = new MiningSetup(null);
            IsInit = false;
            MinerID = MinerIDCount++;
            Miner.MinerDeviceName = minerDeviceName;
            //MinerDeviceName = minerDeviceName;
            WorkingDirectory = "";

            IsRunning = false;
            IsRunningNew = IsRunning;
            PreviousTotalMH = 0.0;

            LastCommandLine = "";

            IsApiReadException = false;
            // Only set minimize if hide is false (specific miners will override true after)
            IsNeverHideMiningWindow = ConfigManager.GeneralConfig.MinimizeMiningWindows &&
                                      !ConfigManager.GeneralConfig.HideMiningWindows;
            IsKillAllUsedMinerProcs = false;
            _maxCooldownTimeInMilliseconds = GetMaxCooldownTimeInMilliseconds();
            //
            Helpers.ConsolePrint(MinerTag(), "NEW MINER CREATED");
        }

        ~Miner()
        {
            // free the port
            MinersApiPortsManager.RemovePort(ApiPort);
            //DHClientsStop();
            Helpers.ConsolePrint(MinerTag(), "MINER DESTROYED");
        }

        protected void SetWorkingDirAndProgName(string fullPath)
        {
            WorkingDirectory = "";
            Path = fullPath;
            var lastIndex = fullPath.LastIndexOf("\\") + 1;
            if (lastIndex > 0)
            {
                WorkingDirectory = fullPath.Substring(0, lastIndex);
                MinerExeName = fullPath.Substring(lastIndex);
            }
        }

        private void SetApiPort()
        {
            if (IsInit)
            {
                var minerBase = MiningSetup.MiningPairs[0].Algorithm.MinerBaseType;
                var algoType = MiningSetup.MiningPairs[0].Algorithm.NiceHashID;
                var devtype = MiningSetup.MiningPairs[0].Device.DeviceType;
                var path = MiningSetup.MinerPath;
                var reservedPorts = MinersSettingsManager.GetPortsListFor(minerBase, path, algoType);
                ApiPort = -1; // not set
                foreach (var reservedPort in reservedPorts)
                {
                    if (MinersApiPortsManager.IsPortAvaliable(reservedPort))
                    {
                        if (minerBase.Equals("hsrneoscrypt"))
                        {
                            ApiPort = 4001;
                        }
                        else
                        {
                            ApiPort = reservedPort;
                        }
                        break;
                    }
                }
                if (minerBase.ToString().Equals("hsrneoscrypt"))
                {
                    ApiPort = 4001;
                }
                else
                {
                    ApiPort = MinersApiPortsManager.GetAvaliablePort();
                }
                /*
                if (minerBase.ToString().Equals("Nanominer") && devtype == DeviceType.NVIDIA)
                {
                    ApiPort = 4051;
                }
                if (minerBase.ToString().Equals("Nanominer") && devtype == DeviceType.AMD)
                {
                    ApiPort = 4052;
                }
                Helpers.ConsolePrint("SetApiPort********************", "ApiPort: " + ApiPort.ToString());
                */
            }
        }


        public virtual void InitMiningSetup(MiningSetup miningSetup)
        {
            MiningSetup = miningSetup;
            IsInit = MiningSetup.IsInit;
            SetApiPort();
            SetWorkingDirAndProgName(MiningSetup.MinerPath);
            //Thread.Sleep(Math.Max(ConfigManager.GeneralConfig.MinerRestartDelayMS, 500));
        }

        public void InitBenchmarkSetup(MiningPair benchmarkPair)
        {
            InitMiningSetup(new MiningSetup(new List<MiningPair>()
            {
                benchmarkPair
            }));
            BenchmarkAlgorithm = benchmarkPair.Algorithm;
        }

        // TAG for identifying miner
        public string MinerTag()
        {
            MinerDeviceName = MiningSetup.MinerName;
            if (_minerTag == null)
            {
                const string mask = "{0}-MINER_ID({1})-DEVICE_IDs({2})";
                // no devices set
                if (!IsInit)
                {
                    return string.Format(mask, MinerDeviceName, MinerID, "NOT_SET");
                }
                
                // contains ids
                var ids = MiningSetup.MiningPairs.Select(cdevs => cdevs.Device.ID.ToString()).ToList();
                _minerTag = string.Format(mask, MinerDeviceName, MinerID, string.Join(",", ids));
            }

            return _minerTag;
        }

        private static string ProcessTag(MinerPidData pidData)
        {
            return $"[pid({pidData.Pid})|bin({pidData.MinerBinPath})]";
        }

        public string ProcessTag()
        {
            if (_currentPidData == null)
            {
                Helpers.ConsolePrint("ProcessTag", "PidData is NULL. Restart program");
                Stop(MinerStopType.END); // stop miner first
                Thread.Sleep(Math.Max(ConfigManager.GeneralConfig.MinerRestartDelayMS, 500));
                Form_Main.MakeRestart(0);
                return "PidData is NULL";
            } else
            {
                return ProcessTag(_currentPidData);
            }
            return "unknown";
        }

        private static int ChildProcess(MinerPidData pidData)
        {
            return GetChildProcess(pidData.Pid);
        }
        public int ChildProcess()
        {
            return _currentPidData == null ? -1 : ChildProcess(_currentPidData);
        }

        private static int GetParentProcess(int Id)
        {
            int parentPid = 0;
            using (ManagementObject mo = new ManagementObject("win32_process.handle='" + Id.ToString() + "'"))
            {
                mo.Get();
                parentPid = Convert.ToInt32(mo["ParentProcessId"]);
            }
            return parentPid;
        }

        public static int GetChildProcess(int ProcessId, string fname = "miner")
        {
            Process[] localByName = Process.GetProcessesByName(fname);
            foreach (var processName in localByName)
            {
                int t = Process.GetProcessById(processName.Id).Id;
                int p = GetParentProcess(t);
                if (p == ProcessId)
                {
                    return t;
                }
            }
            return -1;
        }

        public void KillAllUsedMinerProcesses()
        {
            var toRemovePidData = new List<MinerPidData>();
            Helpers.ConsolePrint(MinerTag(), "Trying to close all miner processes for this instance:");
            var algo = (int)MiningSetup.CurrentAlgorithmType;
            string strPlatform = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                pair.Device.MiningHashrate = 0;
                pair.Device.MiningHashrateSecond = 0;
                pair.Device.MiningHashrateThird = 0;
                pair.Device.MinerName = "";
                pair.Device.State = DeviceState.Stopped;

                pair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                pair.Device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                pair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;

                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    strPlatform = "NVIDIA";
                }
                else if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    strPlatform = "AMD";
                }
                else if (pair.Device.DeviceType == DeviceType.CPU)
                {
                    strPlatform = "CPU";
                }
            }

            foreach (var pidData in _allPidData)
            {
                try
                {
                    var process = Process.GetProcessById(pidData.Pid);
                    if (pidData.MinerBinPath.Contains(process.ProcessName))
                    {
                        Helpers.ConsolePrint(MinerTag(), $"Trying to close {ProcessTag(pidData)}");
                        try
                        {
                            if (Form_Main.DivertAvailable)
                            {
                                Divert.DivertStop(pidData.DivertHandle, pidData.Pid, algo,
                                    (int)MiningSetup.CurrentSecondaryAlgorithmType, ConfigManager.GeneralConfig.DivertRun,
                                    MinerDeviceName, strPlatform);
                            }

                            process.CloseMainWindow();
                            //process.Kill();
                            process.Close();
                            //process.WaitForExit(1000 * 20);
                        }
                        catch (InvalidOperationException ioex)
                        {
                            Helpers.ConsolePrint(MinerTag(),
                                $"InvalidOperationException closing {ProcessTag(pidData)}, exMsg {ioex.Message}");
                        }
                        catch (Exception e)
                        {
                            Helpers.ConsolePrint(MinerTag(),
                                $"Exception closing {ProcessTag(pidData)}, exMsg {e.Message}");
                        }
                    }
                }
                catch (Exception e)
                {
                    toRemovePidData.Add(pidData);
                    Helpers.ConsolePrint(MinerTag(), $"Nothing to close {ProcessTag(pidData)}, exMsg {e.Message}");
                }
            }

            _allPidData.RemoveAll(x => toRemovePidData.Contains(x));
        }

        public abstract void Start(string btcAdress, string worker);

        protected string GetUsername(string btcAdress, string worker)
        {
            if (worker.Length > 0)
            {
                //return btcAdress + "." + worker + "$" + NiceHashSocket.RigID;
                return btcAdress + "." + worker + "$" + ConfigManager.GeneralConfig.MachineGuid;
            }
            else
            {

            }

            return btcAdress;
        }

        protected abstract void _Stop(MinerStopType willswitch);

        public virtual void Stop(MinerStopType willswitch = MinerStopType.SWITCH)
        {
            var sortedMinerPairs = MiningSetup.MiningPairs.OrderBy(pair => pair.Device.IDByBus).ToList();
            foreach (var mPair in sortedMinerPairs)
            {
                mPair.Device.MiningHashrate = 0;
                mPair.Device.MiningHashrateSecond = 0;
            }

            //new Task(() => NiceHashStats.SetDeviceStatus("PENDING")).Start();
            _cooldownCheckTimer?.Stop();
            _Stop(willswitch);
            PreviousTotalMH = 0.0;
            IsRunning = false;
            IsRunningNew = IsRunning;
            //new Task(() => NiceHashMiner.Utils.ServerResponceTime.GetBestServer()).Start();
            RunCMDBeforeOrAfterMining(false);
            NiceHashStats._deviceUpdateTimer.Stop();
            //new Task(() => NiceHashStats.SetDeviceStatus("STOPPED")).Start();
            NiceHashStats._deviceUpdateTimer.Start();
            //NiceHashStats.SetDeviceStatus("STOPPED");
        }

        public void End()
        {
            _isEnded = true;
            Stop(MinerStopType.FORCE_END);
        }
        protected void KillProcessAndChildren(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher
                        ("Select * From Win32_Process Where ParentProcessID=" + pid);
                ManagementObjectCollection moc = searcher.Get();

                foreach (ManagementObject mo in moc)
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("KillProcessAndChildren", er.ToString());
            }
            finally
            {
                KillAllUsedMinerProcesses();
            }

        }
        protected void Stop_cpu_ccminer_sgminer_nheqminer(MinerStopType willswitch)
        {
            var algo = (int)MiningSetup.CurrentAlgorithmType;
            string strPlatform = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                pair.Device.MiningHashrate = 0;
                pair.Device.MiningHashrateSecond = 0;
                pair.Device.MiningHashrateThird = 0;
                pair.Device.MinerName = "";
                pair.Device.State = DeviceState.Stopped;

                pair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                pair.Device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                pair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;

                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    strPlatform = "NVIDIA";
                }
                else if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    strPlatform = "AMD";
                }
                else if (pair.Device.DeviceType == DeviceType.CPU)
                {
                    strPlatform = "CPU";
                }
            }
            if (IsRunning)
            {
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Shutting down miner");
            }

            if (ProcessHandle != null)
            {
                ProcessHandle._bRunning = false;
                ProcessHandle.ExitEvent = null;
                int k = ProcessTag().IndexOf("pid(");
                int i = ProcessTag().IndexOf(")|bin");
                var cpid = ProcessTag().Substring(k + 4, i - k - 4).Trim();
                int pid = int.Parse(cpid, CultureInfo.InvariantCulture);

                if (algo == (int)AlgorithmType.KAWPOWLite)
                {
                    try
                    {
                        if (Form_Main.KawpowLite && Form_Main.DivertAvailable)
                        {
                            Divert.checkConnectionKawpowLite = true;
                            new Task(() => KawpowClient.CheckConnectionToPool()).Start();
                        }

                        Divert.DivertStop(ProcessHandle.DivertHandle, ProcessHandle.Id, algo,
                            (int)MiningSetup.CurrentSecondaryAlgorithmType, ConfigManager.GeneralConfig.DivertRun, MinerDeviceName, strPlatform);
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("Stop_cpu_ccminer_sgminer_nheqminer error: ", e.ToString());
                    }
                }

                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + " SendCtrlC to stop miner");
                    try { ProcessHandle.SendCtrlC((uint)Process.GetCurrentProcess().Id); } catch { }
                    Thread.Sleep(1000);

                KillProcessAndChildren(pid);

                try
                {
                    if (ProcessHandle is object)
                    {
                        if (ProcessHandle != null)
                        {
                            Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Try force kill miner");
                            ProcessHandle.Kill();
                        }
                    }
                } catch
                {

                }
                //try { ProcessHandle.SendCtrlC((uint)Process.GetCurrentProcess().Id); } catch { }
                if (ProcessHandle != null)
                {
                    ProcessHandle.Close();
                    ProcessHandle = null;
                }

                if (IsKillAllUsedMinerProcs) KillAllUsedMinerProcesses();
            }
        }

        public static bool PiDExist(int processId)
        {
            return true;
            try
            {
                Process[] allProcessesOnLocalMachine = Process.GetProcesses();
                foreach (Process process in allProcessesOnLocalMachine)
                {
                    if (process.Id == processId) return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        protected virtual string GetDevicesCommandString()
        {
            var deviceStringCommand = " ";

            var ids = MiningSetup.MiningPairs.Select(mPair => mPair.Device.ID.ToString()).ToList();
            deviceStringCommand += string.Join(",", ids);

            return deviceStringCommand;
        }

        #region BENCHMARK DE-COUPLED Decoupled benchmarking routines
        protected double BenchmarkParseLine_cpu_hsrneoscrypt_extra(string outdata)
        {
            if (outdata.Contains("Benchmark: ") && outdata.Contains("/s"))
            {
                int i = outdata.IndexOf("Benchmark:");
                int k = outdata.IndexOf("/s");
                string hashspeed = outdata.Substring(i + 11, k - i - 9);
                Helpers.ConsolePrint("BENCHMARK-NS", "Final Speed: " + hashspeed);

                // save speed
                int b = hashspeed.IndexOf(" ");
                if (b < 0)
                {
                    int stub;
                    for (int _i = hashspeed.Length - 1; _i >= 0; --_i)
                    {
                        if (Int32.TryParse(hashspeed[_i].ToString(), out stub))
                        {
                            b = _i;
                            break;
                        }
                    }
                }
                if (b >= 0)
                {
                    string speedStr = hashspeed.Substring(0, b);
                    double spd = Helpers.ParseDouble(speedStr);
                    if (hashspeed.Contains("kH/s"))
                        spd *= 1000;
                    else if (hashspeed.Contains("MH/s"))
                        spd *= 1000000;
                    else if (hashspeed.Contains("GH/s"))
                        spd *= 1000000000;

                    return spd;
                }
            }
            return 0.0d;
        }

        public int BenchmarkTimeoutInSeconds(int timeInSeconds)
        {
            if (TimeoutStandard) return timeInSeconds;
            if (BenchmarkAlgorithm.NiceHashID == AlgorithmType.DaggerHashimoto || BenchmarkAlgorithm.NiceHashID == AlgorithmType.ETCHash)
            {
                return 5 * 60 + 120; // 5 minutes plus two minutes
            }

            return timeInSeconds + 120; // wait time plus two minutes
        }

        // TODO remove algorithm
        protected abstract string BenchmarkCreateCommandLine(Algorithm algorithm, int time);

        // The benchmark config and algorithm must guarantee that they are compatible with miner
        // we guarantee algorithm is supported
        // we will not have empty benchmark configs, all benchmark configs will have device list
        public virtual void BenchmarkStart(int time, IBenchmarkComunicator benchmarkComunicator)
        {
            BenchmarkComunicator = benchmarkComunicator;
            BenchmarkTimeInSeconds = time;
            BenchmarkSignalFinnished = true;
            // check and kill
            BenchmarkHandle = null;
            OnBenchmarkCompleteCalled = false;
            _benchmarkTimeOutStopWatch = null;


            try
            {
                if (!Directory.Exists("logs"))
                {
                    Directory.CreateDirectory("logs");
                }
            }
            catch { }

            BenchLines = new List<string>();
            _benchmarkLogPath =
                $"{Logger.LogPath}Log_{MiningSetup.MiningPairs[0].Device.Uuid}_{MiningSetup.MiningPairs[0].Algorithm.AlgorithmStringID}";

            var commandLine = BenchmarkCreateCommandLine(BenchmarkAlgorithm, time);
            var benchmarkThread = new Thread(BenchmarkThreadRoutine, time);
            benchmarkThread.Start(commandLine);
        }

        protected virtual Process BenchmarkStartProcess(string commandLine)
        {
            RunCMDBeforeOrAfterMining(true);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Helpers.ConsolePrint(MinerTag(), "Starting benchmark: " + commandLine);

            var benchmarkHandle = new Process
            {
                StartInfo =
                {
                    FileName = MiningSetup.MinerPath
                }
            };

            if (benchmarkHandle.StartInfo.FileName.ToLower().Contains("cryptodredge") && (commandLine.ToLower().Contains("neoscrypt") || commandLine.ToLower().Contains("x16rv2")))
            {
                benchmarkHandle.StartInfo.FileName = benchmarkHandle.StartInfo.FileName.Replace("CryptoDredge.exe", "CryptoDredge.0.25.1.exe");
            }
            if (benchmarkHandle.StartInfo.FileName.ToLower().Contains("t-rex") && (commandLine.ToLower().Contains("x16r") || commandLine.ToLower().Contains("x16rv2") ))
            {
                benchmarkHandle.StartInfo.FileName = benchmarkHandle.StartInfo.FileName.Replace("t-rex.exe", "t-rex.0.19.4.exe");
            }
            if (benchmarkHandle.StartInfo.FileName.ToLower().Contains("nbminer") && (commandLine.ToLower().Contains("cuckatoo")))
            {
                benchmarkHandle.StartInfo.FileName = benchmarkHandle.StartInfo.FileName.Replace("nbminer.exe", "nbminer.39.5.exe");
            }
            if (benchmarkHandle.StartInfo.FileName.ToLower().Contains("nbminer") && (commandLine.ToLower().Contains("beam")))
            {
                benchmarkHandle.StartInfo.FileName = benchmarkHandle.StartInfo.FileName.Replace("nbminer.exe", "nbminer.39.5.exe");
            }
            if (benchmarkHandle.StartInfo.FileName.ToLower().Contains("nbminer") && (commandLine.ToLower().Contains("kawpow")))
            {
                benchmarkHandle.StartInfo.FileName = benchmarkHandle.StartInfo.FileName.Replace("nbminer.exe", "nbminer.39.5.exe");
            }
            if (benchmarkHandle.StartInfo.FileName.ToLower().Contains("nbminer") && (commandLine.ToLower().Contains("ergo")))
            {
                benchmarkHandle.StartInfo.FileName = benchmarkHandle.StartInfo.FileName.Replace("nbminer.exe", "nbminer.39.5.exe");
            }

            BenchmarkProcessPath = benchmarkHandle.StartInfo.FileName;
            Helpers.ConsolePrint(MinerTag(), "Using miner: " + benchmarkHandle.StartInfo.FileName);
            benchmarkHandle.StartInfo.WorkingDirectory = WorkingDirectory;
            benchmarkHandle.StartInfo.Arguments = commandLine;
            benchmarkHandle.StartInfo.UseShellExecute = false;
            benchmarkHandle.StartInfo.RedirectStandardError = true;
            benchmarkHandle.StartInfo.RedirectStandardOutput = true;
            benchmarkHandle.StartInfo.CreateNoWindow = true;
            benchmarkHandle.OutputDataReceived += BenchmarkOutputErrorDataReceived;
            benchmarkHandle.ErrorDataReceived += BenchmarkOutputErrorDataReceived;
            benchmarkHandle.Exited += BenchmarkHandle_Exited;

            if (!benchmarkHandle.Start()) return null;

            _currentPidData = new MinerPidData
            {
                MinerBinPath = benchmarkHandle.StartInfo.FileName,
                Pid = benchmarkHandle.Id
            };
            _allPidData.Add(_currentPidData);

            return benchmarkHandle;
        }

        private void BenchmarkHandle_Exited(object sender, EventArgs e)
        {
            BenchmarkSignalFinnished = true;
        }

        private void BenchmarkOutputErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_benchmarkTimeOutStopWatch == null)
            {
                _benchmarkTimeOutStopWatch = new Stopwatch();
                _benchmarkTimeOutStopWatch.Start();
            }
            else if (_benchmarkTimeOutStopWatch.Elapsed.TotalSeconds >
                     BenchmarkTimeoutInSeconds(BenchmarkTimeInSeconds))
            {
                _benchmarkTimeOutStopWatch.Stop();
                BenchmarkSignalTimedout = true;
            }

            var outdata = e.Data;
            if (e.Data != null)
            {
                BenchmarkOutputErrorDataReceivedImpl(outdata);
            }

            // terminate process situations
            if (BenchmarkSignalQuit
                || BenchmarkSignalFinnished
                || BenchmarkSignalHanged
                || BenchmarkSignalTimedout
                || BenchmarkException != null)
            {
                FinishUpBenchmark();
                EndBenchmarkProcces();
            }
        }

        protected virtual void FinishUpBenchmark()
        { }

        protected abstract void BenchmarkOutputErrorDataReceivedImpl(string outdata);

        protected void CheckOutdata(string outdata)
        {
            BenchLines.Add(outdata);
            /*
            // ccminer, cpuminer
            if (outdata.Contains("Cuda error"))
                BenchmarkException = new Exception("CUDA error");
            if (outdata.Contains("is not supported"))
                BenchmarkException = new Exception("N/A");
            if (outdata.Contains("illegal memory access"))
                BenchmarkException = new Exception("CUDA error");
            if (outdata.Contains("unknown error"))
                BenchmarkException = new Exception("Unknown error");
            if (outdata.Contains("No servers could be used! Exiting."))
                BenchmarkException = new Exception("No pools or work can be used for benchmarking");
            //if (outdata.Contains("error") || outdata.Contains("Error"))
            //    BenchmarkException = new Exception("Unknown error #2");
            // Ethminer
            if (outdata.Contains("No GPU device with sufficient memory was found"))
                BenchmarkException = new Exception("[daggerhashimoto] No GPU device with sufficient memory was found.");
            // xmr-stak
            if (outdata.Contains("Press any key to exit"))
                BenchmarkException = new Exception("Xmr-Stak erred, check its logs");
            */
            // lastly parse data
            //Helpers.ConsolePrint("BENCHMARK_CheckOutData", outdata);

        }

        public void InvokeBenchmarkSignalQuit()
        {
            KillAllUsedMinerProcesses();
        }

        protected double BenchmarkParseLine_cpu_ccminer_extra(string outdata)
        {
            if (outdata.Contains("Benchmark: ") && outdata.Contains("/s"))
            {
                var i = outdata.IndexOf("Benchmark:");
                var k = outdata.IndexOf("/s");
                var hashspeed = outdata.Substring(i + 11, k - i - 9);
                Helpers.ConsolePrint("BENCHMARK-CC", "Final Speed: " + hashspeed);

                // save speed
                var b = hashspeed.IndexOf(" ");
                if (b < 0)
                {
                    for (var j = hashspeed.Length - 1; j >= 0; --j)
                    {
                        if (!int.TryParse(hashspeed[j].ToString(), out var _)) continue;
                        b = j;
                        break;
                    }
                }

                if (b >= 0)
                {
                    var speedStr = hashspeed.Substring(0, b);
                    var spd = Helpers.ParseDouble(speedStr);
                    if (hashspeed.Contains("kH/s"))
                        spd *= 1000;
                    else if (hashspeed.Contains("MH/s"))
                        spd *= 1000000;
                    else if (hashspeed.Contains("GH/s"))
                        spd *= 1000000000;

                    return spd;
                }
            }

            return 0.0d;
        }

        public virtual void EndBenchmarkProcces()
        {
            if (BenchmarkHandle != null && BenchmarkProcessStatus != BenchmarkProcessStatus.Killing &&
                BenchmarkProcessStatus != BenchmarkProcessStatus.DoneKilling)
            {
                BenchmarkProcessStatus = BenchmarkProcessStatus.Killing;
                try
                {
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + " SendCtrlC to stop miner");
                    try
                    {
                        if (ProcessHandle is object)
                        {
                            if (Process.GetCurrentProcess() != null)
                            {
                                ProcessHandle.SendCtrlC((uint)Process.GetCurrentProcess().Id);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("BENCHMARK-end", $"algorithm {BenchmarkAlgorithm.AlgorithmName} : " + ex.ToString());
                    }
                    Thread.Sleep(1000);

                    try
                    {
                        int pid = _currentPidData.Pid;
                        if (BenchmarkHandle != null && Process.GetProcessById(pid) != null)
                        {
                            Helpers.ConsolePrint("BENCHMARK-end",
    $"Trying to kill benchmark process {BenchmarkProcessPath} algorithm {BenchmarkAlgorithm.AlgorithmName}");
                            BenchmarkHandle.Kill();
                            BenchmarkHandle.Close();
                            KillAllUsedMinerProcesses();
                        }
                    }
                    catch
                    {

                    }
                }
                catch { }
                finally
                {
                    BenchmarkProcessStatus = BenchmarkProcessStatus.DoneKilling;
                    Helpers.ConsolePrint("BENCHMARK-end",
                        $"Benchmark process {BenchmarkProcessPath} algorithm {BenchmarkAlgorithm.AlgorithmName} CLOSED");
                }
            }
        }


        protected virtual void BenchmarkThreadRoutineStartSettup()
        {
            BenchmarkHandle.BeginErrorReadLine();
            BenchmarkHandle.BeginOutputReadLine();
        }

        protected void BenchmarkThreadRoutineCatch(Exception ex)
        {
            BenchmarkAlgorithm.BenchmarkSpeed = 0;
            BenchmarkAlgorithm.BenchmarkSecondarySpeed = 0;

            Helpers.ConsolePrint(MinerTag(), "Benchmark Exception: " + ex.Message);
            Helpers.ConsolePrint(MinerTag(), "Benchmark Exception: " + ex.ToString());
            if (BenchmarkComunicator != null && !OnBenchmarkCompleteCalled)
            {
                OnBenchmarkCompleteCalled = true;
                BenchmarkComunicator.OnBenchmarkComplete(false, GetFinalBenchmarkString());
            }
        }

        protected virtual string GetFinalBenchmarkString()
        {
            return BenchmarkSignalTimedout && !TimeoutStandard
                ? International.GetText("Benchmark_Timedout")
                : International.GetText("Benchmark_Terminated");
        }

        protected void BenchmarkThreadRoutineFinish()
        {
            BenchmarkAlgorithm.BenchmarkProgressPercent = 0;
            var status = BenchmarkProcessStatus.Finished;
            RunCMDBeforeOrAfterMining(false);

            if (!BenchmarkAlgorithm.BenchmarkNeeded)
            {
                status = BenchmarkProcessStatus.Success;
            }

            try
            {
                using (StreamWriter sw = File.AppendText(_benchmarkLogPath))
                {
                    foreach (var line in BenchLines)
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch { }

            BenchmarkProcessStatus = status;
            if (BenchmarkAlgorithm is DualAlgorithm dualAlg)
            {
                Helpers.ConsolePrint(MinerTag() + " BENCHMARK-finish",
                    "Final Speed: " + Helpers.FormatDualSpeedOutput(BenchmarkAlgorithm.BenchmarkSpeed,
                        BenchmarkAlgorithm.BenchmarkSecondarySpeed, 0, dualAlg.NiceHashID, dualAlg.DualNiceHashID));
            }
            else
            {
                Helpers.ConsolePrint(MinerTag() + " BENCHMARK-finish",
                    "Final Speed: " + Helpers.FormatDualSpeedOutput(BenchmarkAlgorithm.BenchmarkSpeed, 0, 0,
                        BenchmarkAlgorithm.NiceHashID, BenchmarkAlgorithm.DualNiceHashID));
            }

            Helpers.ConsolePrint(MinerTag() + " BENCHMARK-finish", "Benchmark ends");
            if (BenchmarkComunicator != null && !OnBenchmarkCompleteCalled)
            {
                OnBenchmarkCompleteCalled = true;
                var isOK = BenchmarkProcessStatus.Success == status;
                var msg = GetFinalBenchmarkString();
                BenchmarkComunicator.OnBenchmarkComplete(isOK, isOK ? "" : msg);
            }
        }


        protected virtual void BenchmarkThreadRoutine(object commandLine)
        {
            BenchmarkSignalQuit = false;
            BenchmarkSignalHanged = false;
            BenchmarkSignalFinnished = false;
            BenchmarkException = null;

            Thread.Sleep(Math.Max(ConfigManager.GeneralConfig.MinerRestartDelayMS, 500));

            try
            {
                Helpers.ConsolePrint("BENCHMARK-routine", "Benchmark starts");
                BenchmarkHandle = BenchmarkStartProcess((string)commandLine);

                BenchmarkThreadRoutineStartSettup();
                // wait a little longer then the benchmark routine if exit false throw
                //var timeoutTime = BenchmarkTimeoutInSeconds(BenchmarkTimeInSeconds);
                //var exitSucces = BenchmarkHandle.WaitForExit(timeoutTime * 1000);
                // don't use wait for it breaks everything
                BenchmarkProcessStatus = BenchmarkProcessStatus.Running;
                var exited = BenchmarkHandle.WaitForExit((BenchmarkTimeoutInSeconds(BenchmarkTimeInSeconds) + 20) * 1000);
                if (BenchmarkSignalTimedout && !TimeoutStandard)
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

                if (BenchmarkSignalHanged || !exited)
                {
                    throw new Exception("Miner is not responding");
                }

                if (BenchmarkSignalFinnished)
                {
                    //break;
                }
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

        /// <summary>
        /// When parallel benchmarking each device needs its own log files, so this uniquely identifies for the setup
        /// </summary>
        protected string GetDeviceID()
        {
            var ids = MiningSetup.MiningPairs.Select(x => x.Device.ID);
            var idStr = string.Join(",", ids);

            if (!IsMultiType) return idStr;

            // Miners that use multiple dev types need to also discriminate based on that
            var types = MiningSetup.MiningPairs.Select(x => (int)x.Device.DeviceType);
            return $"{string.Join(",", types)}-{idStr}";
        }

        protected string GetLogFileName()
        {
            return $"{GetDeviceID()}_log.txt";
        }

        protected virtual void ProcessBenchLinesAlternate(string[] lines)
        { }

        protected abstract bool BenchmarkParseLine(string outdata);
        
        protected string GetServiceUrl(AlgorithmType algo)
        {
            int _location = ConfigManager.GeneralConfig.ServiceLocation;
            if (ConfigManager.GeneralConfig.ServiceLocation >= Globals.MiningLocation.Length)
            {
                _location = ConfigManager.GeneralConfig.ServiceLocation - 1;
            }
            return Globals.GetLocationUrl(algo, Globals.MiningLocation[_location],
                ConectionType);
        }
        protected bool IsActiveProcess(int pid)
        {
            try
            {
                return Process.GetProcessById(pid) != null;
            }
            catch
            {
                return false;
            }
        }

        #endregion //BENCHMARK DE-COUPLED Decoupled benchmarking routines

        private void MinerDelayStart(string minerpath)
        {
            try
            {
                Process localByName = Process.GetProcessById(Process.GetCurrentProcess().Id);
                var query = "Select * From Win32_Process Where ParentProcessId = " + Process.GetCurrentProcess().Id.ToString();
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection processList = searcher.Get();
                var result = processList.Cast<ManagementObject>().Select(p =>
                    Process.GetProcessById(Convert.ToInt32(p.GetPropertyValue("ProcessId")))).ToList();

                bool minerrunning = false;
                foreach (var process in result)
                {
                    string m = process.ProcessName;
                    string p = process.MainWindowTitle;
                    if (p.ToLower().Contains(minerpath) && (p.ToLower().Contains("gminer") || p.ToLower().Contains("miniz")))
                    {
                        minerrunning = true;
                        break;
                    }
                }
                if (minerrunning) Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("MinerDelayStart", ex.ToString());
            }
        }

        private void DetectLiteMode()
        {
            ulong minMem = (ulong)(1024 * 1024 * 8);
            foreach (var pair in MiningSetup.MiningPairs)
            {
                var algo = pair.Algorithm;
                var _computeDevice = pair.Device;
                if (algo.NiceHashID == AlgorithmType.KAWPOWLite && algo.Enabled)
                {
                    minMem = Math.Min(minMem, _computeDevice.GpuRam / 1024);
                    if (minMem > (ulong)(1024 * 1024 * 2.7) && minMem < (ulong)(1024 * 1024 * 3.7))
                    {
                        Form_Main.KawpowLite3GB = true;
                        Form_Main.KawpowLite4GB = false;
                        Form_Main.KawpowLite5GB = false;
                    }
                    if (minMem > (ulong)(1024 * 1024 * 3.7) && minMem < (ulong)(1024 * 1024 * 4.7))
                    {
                        Form_Main.KawpowLite3GB = false;
                        Form_Main.KawpowLite4GB = true;
                        Form_Main.KawpowLite5GB = false;
                    }
                    if (minMem > (ulong)(1024 * 1024 * 4.7) && minMem < (ulong)(1024 * 1024 * 5.7))
                    {
                        Form_Main.KawpowLite3GB = false;
                        Form_Main.KawpowLite4GB = false;
                        Form_Main.KawpowLite5GB = true;
                    }
                }
            }
        }

        protected virtual NiceHashProcess _Start()
        {
            RunCMDBeforeOrAfterMining(true);
            // never start when ended
            if (_isEnded)
            {
                return null;
            }

            PreviousTotalMH = 0.0;
            if (LastCommandLine.Length == 0) return null;

            var P = new NiceHashProcess();

            if (WorkingDirectory.Length > 1)
            {
                P.StartInfo.WorkingDirectory = WorkingDirectory;
            }

            if (MinersSettingsManager.MinerSystemVariables.ContainsKey(Path))
            {
                foreach (var kvp in MinersSettingsManager.MinerSystemVariables[Path])
                {
                    var envName = kvp.Key;
                    var envValue = kvp.Value;
                    P.StartInfo.EnvironmentVariables[envName] = envValue;
                }
            }

            if (MiningSetup.MinerPath.ToLower().Contains("cryptodredge") && (LastCommandLine.ToLower().Contains("neoscrypt") || LastCommandLine.ToLower().Contains("x16rv2")))
            {
                Path = MiningSetup.MinerPath.Replace("CryptoDredge.exe", "CryptoDredge.0.25.1.exe");
            }
            if (MiningSetup.MinerPath.ToLower().Contains("t-rex") && (LastCommandLine.ToLower().Contains("x16r") || LastCommandLine.ToLower().Contains("x16rv2")))
            {
                Path = MiningSetup.MinerPath.Replace("t-rex.exe", "t-rex.0.19.4.exe");
            }
            if (MiningSetup.MinerPath.ToLower().Contains("nbminer") && (LastCommandLine.ToLower().Contains("cuckatoo")))
            {
                Path = MiningSetup.MinerPath.Replace("nbminer.exe", "nbminer.39.5.exe");
            }
            if (MiningSetup.MinerPath.ToLower().Contains("nbminer") && (LastCommandLine.ToLower().Contains("beam")))
            {
                Path = MiningSetup.MinerPath.Replace("nbminer.exe", "nbminer.39.5.exe");
            }
            if (MiningSetup.MinerPath.ToLower().Contains("nbminer") && (LastCommandLine.ToLower().Contains("kawpow")))
            {
                Path = MiningSetup.MinerPath.Replace("nbminer.exe", "nbminer.39.5.exe");
            }
            if (MiningSetup.MinerPath.ToLower().Contains("nbminer") && (LastCommandLine.ToLower().Contains("ergo")))
            {
                Path = MiningSetup.MinerPath.Replace("nbminer.exe", "nbminer.39.5.exe");
            }

            P.StartInfo.FileName = Path;

            P.ExitEvent = Miner_Exited;
            LastCommandLine = System.Text.RegularExpressions.Regex.Replace(LastCommandLine, @"\s+", " ");
            P.StartInfo.Arguments = LastCommandLine;
            if (IsNeverHideMiningWindow)
            {
                P.StartInfo.CreateNoWindow = false;
                if (ConfigManager.GeneralConfig.HideMiningWindows || ConfigManager.GeneralConfig.MinimizeMiningWindows)
                {
                    P.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    P.StartInfo.UseShellExecute = true;
                }
            }
            else
            {
                P.StartInfo.CreateNoWindow = ConfigManager.GeneralConfig.HideMiningWindows;
            }

            P.StartInfo.UseShellExecute = false;

            try
            {
                NiceHashStats._deviceUpdateTimer.Stop();

                NiceHashStats._deviceUpdateTimer.Start();
                string strPlatform = "";
                foreach (var pair in MiningSetup.MiningPairs)
                {
                    int a = (int)pair.Algorithm.NiceHashID;
                    int b = (int)pair.Algorithm.SecondaryNiceHashID;
                    pair.Device.AlgorithmID = a;
                    pair.Device.SecondAlgorithmID = b;
                    pair.Device.MinerName = MinerDeviceName;
                    pair.Device.State = DeviceState.Mining;

                    if (pair.Device.DeviceType == DeviceType.NVIDIA)
                    {
                        strPlatform = "NVIDIA";
                    }
                    else if (pair.Device.DeviceType == DeviceType.AMD)
                    {
                        strPlatform = "AMD";
                    }
                    else if (pair.Device.DeviceType == DeviceType.CPU)
                    {
                        strPlatform = "CPU";
                    }
                }
                
                GC.Collect();
                
                P.DivertHandle = Divert.DivertStart(P.Id, -1, -1, Path,
                            strPlatform, "", false,
                            false,
                            false, ConfigManager.GeneralConfig.DivertRun,
                            100);
                
                MinerDelayStart(Path);

                if (P.Start())
                {
                    _currentPidData = new MinerPidData
                    {
                        MinerBinPath = P.StartInfo.FileName,
                        Pid = P.Id
                    };
                    _allPidData.Add(_currentPidData);

                    Helpers.ConsolePrint(MinerTag(), "Starting miner " + ProcessTag() + " " + LastCommandLine);
                    IsRunning = true;
                    IsRunningNew = IsRunning;

                    if (Form_Main.DivertAvailable)
                    {
                        int algo = (int)MiningSetup.CurrentAlgorithmType;
                        int algo2 = (int)MiningSetup.CurrentSecondaryAlgorithmType;
                        string w = ConfigManager.GeneralConfig.WorkerName + "$" + NiceHashMiner.Stats.NiceHashSocket.RigID;

                        int MaxEpoch = 0;

                        if (MiningSetup.CurrentAlgorithmType == AlgorithmType.KAWPOWLite)
                        {
                            DetectLiteMode();
                            if (Form_Main.KawpowLite3GB) MaxEpoch = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch3GB;
                            if (Form_Main.KawpowLite4GB) MaxEpoch = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch4GB;
                            if (Form_Main.KawpowLite5GB) MaxEpoch = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch5GB;
                            Helpers.ConsolePrint(MinerTag(), "Max epoch for KAWPOWLite is " + MaxEpoch.ToString());
                            new Task(() => KawpowClient.StopConnection()).Start();
                        }

                        P.DivertHandle = Divert.DivertStart(P.Id, algo, algo2, Path,
                            strPlatform, w, false,
                            false,
                            false, ConfigManager.GeneralConfig.DivertRun,
                            MaxEpoch);
                    }
                    new Task(() => StartCoolDownTimerChecker()).Start();
                    //StartCoolDownTimerChecker();
                    return P;
                }

                Helpers.ConsolePrint(MinerTag(), "NOT STARTED " + ProcessTag() + " " + LastCommandLine);
                return null;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " _Start: " + ex.Message);
                return null;
            }
        }

        protected void StartCoolDownTimerChecker()
        {
            if (ConfigManager.GeneralConfig.CoolDownCheckEnabled)
            {
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Starting cooldown checker");
                if (_cooldownCheckTimer != null && _cooldownCheckTimer.Enabled) _cooldownCheckTimer.Stop();
                // cool down init
                _cooldownCheckTimer = new Timer()
                {
                    Interval = MinCooldownTimeInMilliseconds
                };
                _cooldownCheckTimer.Elapsed += MinerCoolingCheck_Tick;
                _cooldownCheckTimer.Start();
                _currentCooldownTimeInSeconds = MinCooldownTimeInMilliseconds;
                _currentCooldownTimeInSecondsLeft = _currentCooldownTimeInSeconds;
            }
            else
            {
                Helpers.ConsolePrint(MinerTag(), "Cooldown checker disabled");
            }

            CurrentMinerReadStatus = MinerApiReadStatus.NONE;
        }

        protected virtual void Miner_Exited()
        {
            ScheduleRestart(6000);
        }

        protected void ScheduleRestart(int ms)
        {
            if (ProcessHandle != null)
            {
                if (!ProcessHandle._bRunning) return;
            }

            var restartInMs = ConfigManager.GeneralConfig.MinerRestartDelayMS > ms
                ? ConfigManager.GeneralConfig.MinerRestartDelayMS
                : ms;
            Helpers.ConsolePrint(MinerTag(), ProcessTag() + $" directly Miner_Exited Will restart in {restartInMs} ms");
            var algo = (int)MiningSetup.CurrentAlgorithmType;
            string strPlatform = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                pair.Device.MiningHashrate = 0;
                pair.Device.MiningHashrateSecond = 0;
                pair.Device.MiningHashrateThird = 0;
                pair.Device.MinerName = "";
                pair.Device.State = DeviceState.Stopped;

                pair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                pair.Device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                pair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;

                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    strPlatform = "NVIDIA";
                }
                else if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    strPlatform = "AMD";
                }
                else if (pair.Device.DeviceType == DeviceType.CPU)
                {
                    strPlatform = "CPU";
                }
            }
            if (ProcessHandle != null)
            {
                if (algo != (int)AlgorithmType.KAWPOWLite)
                {
                    try
                    {
                        Divert.DivertStop(ProcessHandle.DivertHandle, ProcessHandle.Id, algo,
                            (int)MiningSetup.CurrentSecondaryAlgorithmType, ConfigManager.GeneralConfig.DivertRun, MinerDeviceName, strPlatform);
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("ScheduleRestart error: ", e.ToString());
                    }
                }
            }
            if (ProcessHandle != null)
            {
                try
                {
                    var p = Process.GetProcessById(ProcessHandle.Id);
                    p.Kill();
                }
                catch (Exception)
                {
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + "Process not exist.");
                }
            }
            Thread.Sleep(restartInMs);
            Restart();
        }

        protected void Restart()
        {
            if (_isEnded) return;
            var algo = (int)MiningSetup.CurrentAlgorithmType;
            string strPlatform = "";
            foreach (var pair in MiningSetup.MiningPairs)
            {
                pair.Device.MiningHashrate = 0;
                pair.Device.MiningHashrateSecond = 0;
                pair.Device.MiningHashrateThird = 0;
                pair.Device.MinerName = "";
                pair.Device.State = DeviceState.Stopped;

                pair.Device.AlgorithmID = (int)AlgorithmType.NONE;
                pair.Device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                pair.Device.ThirdAlgorithmID = (int)AlgorithmType.NONE;

                if (pair.Device.DeviceType == DeviceType.NVIDIA)
                {
                    strPlatform = "NVIDIA";
                }
                else if (pair.Device.DeviceType == DeviceType.AMD)
                {
                    strPlatform = "AMD";
                }
                else if (pair.Device.DeviceType == DeviceType.CPU)
                {
                    strPlatform = "CPU";
                }
            }
            if (ProcessHandle != null)
            {
                if (Form_Main.DivertAvailable && (algo != (int)AlgorithmType.KAWPOWLite))
                {
                    try
                    {
                        Divert.DivertStop(ProcessHandle.DivertHandle, ProcessHandle.Id, algo,
                            (int)MiningSetup.CurrentSecondaryAlgorithmType, ConfigManager.GeneralConfig.DivertRun, MinerDeviceName, strPlatform);
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("Restart error: ", e.ToString());
                    }
                }
            }
            Helpers.ConsolePrint(MinerTag(), ProcessTag() + " Restarting miner..");
            Stop(MinerStopType.END); // stop miner first
            Thread.Sleep(Math.Max(ConfigManager.GeneralConfig.MinerRestartDelayMS, 500));
            ProcessHandle = _Start(); // start with old command line
        }

        protected virtual bool IsApiEof(byte third, byte second, byte last)
        {
            return false;
        }

        protected async Task<string> GetApiDataAsync(int port, string dataToSend, bool exitHack = false,
            bool overrideLoop = false)
        {
            string responseFromServer = null;
            try
            {
                var tcpc = new TcpClient("127.0.0.1", port);
                var nwStream = tcpc.GetStream();
                nwStream.ReadTimeout = 2 * 1000;
                nwStream.WriteTimeout = 2 * 1000;

                var bytesToSend = Encoding.ASCII.GetBytes(dataToSend);
                await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);

                var incomingBuffer = new byte[tcpc.ReceiveBufferSize];
                var prevOffset = -1;
                var offset = 0;
                var fin = false;

                while (!fin && tcpc.Client.Connected)
                {
                    var r = await nwStream.ReadAsync(incomingBuffer, offset, tcpc.ReceiveBufferSize - offset);
                    for (var i = offset; i < offset + r; i++)
                    {
                        if (incomingBuffer[i] == 0x7C || incomingBuffer[i] == 0x00
                                                      || (i > 2 && IsApiEof(incomingBuffer[i - 2],
                                                              incomingBuffer[i - 1], incomingBuffer[i]))
                                                      || overrideLoop)
                        {
                            fin = true;
                            break;
                        }
                    }

                    offset += r;
                    if (exitHack)
                    {
                        if (prevOffset == offset)
                        {
                            fin = true;
                            break;
                        }

                        prevOffset = offset;
                    }
                }

                tcpc.Close();

                if (offset > 0)
                    responseFromServer = Encoding.ASCII.GetString(incomingBuffer);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + " GetAPIData reason: " + ex.Message);
                CurrentMinerReadStatus = MinerApiReadStatus.READ_SPEED_ZERO;
                return null;
            }

            return responseFromServer;
        }

        public abstract Task<ApiData> GetSummaryAsync();
        public abstract ApiData GetApiData();

        
        protected string GetHttpRequestNhmAgentStrin(string cmd)
        {
            return "GET /" + cmd + " HTTP/1.1\r\n" +
                   "Host: 127.0.0.1\r\n" +
                   "User-Agent: NiceHashMiner/" + Application.ProductVersion + "\r\n" +
                   "\r\n";
        }

        #region Cooldown/retry logic

        private void MinerCoolingCheck_Tick(object sender, ElapsedEventArgs e)
        {
            if (_isEnded)
            {
                End();
                return;
            }
            //Helpers.ConsolePrint(MinerTag(), ProcessTag() + " running: " + ProcessHandle._bRunning.ToString());
            if (ProcessHandle == null)
            {
                CooldownCheck = 100;
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + "Process not exist. Restart miner");
                CooldownCheck = 0;
                Restart();
            }
            if (ProcessHandle != null && !ProcessHandle._bRunning)
            {
                try
                {
                    var p = Process.GetProcessById(ProcessHandle.Id);
                }
                catch (Exception)
                {
                    CooldownCheck = 100;
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + "Process not exist. Restart miner");
                    CooldownCheck = 0;
                    Restart();
                }
            }

            switch (CurrentMinerReadStatus)
            {
                case MinerApiReadStatus.GOT_READ:
                    //Helpers.ConsolePrint(MinerTag(), ProcessTag() + "MinerApiReadStatus.GOT_READ");
                    CooldownCheck = 0;
                    break;
                case MinerApiReadStatus.READ_SPEED_ZERO:
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + " READ SPEED ZERO, will cool up " + CooldownCheck.ToString());
                    CooldownCheck++;
                    break;
                case MinerApiReadStatus.RESTART:
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + "MinerApiReadStatus.RESTART");
                    CooldownCheck = 100;
                    break;
                default:
                    Helpers.ConsolePrint(MinerTag(), ProcessTag() + "MinerApiReadStatus.UNKNOWN");
                    CooldownCheck++;
                    break;
            }

            if (CooldownCheck > 24)//120 sec
            {
                Helpers.ConsolePrint(MinerTag(), ProcessTag() + "API Error. Restart miner");
                CooldownCheck = 0;
                Restart();
            }
        }

        #endregion //Cooldown/retry logic

        protected Process RunCMDBeforeOrAfterMining(bool isBefore)
        {
            if (ConfigManager.GeneralConfig.ABEnableOverclock)
            {
                if (isBefore)
                {
                    foreach (var dev in MiningSetup.MiningPairs)
                    {
                        if (dev.Device.Enabled)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                string fName = "configs\\overclock\\" + dev.Device.Uuid + "_" + dev.Algorithm.AlgorithmStringID + ".gpu";
                                Helpers.ConsolePrint(MinerTag(), "Try MSIAfterburner.ApplyFromFile: " + fName);
                                if (MSIAfterburner.ApplyFromFile(dev.Device.BusID, fName)) break;
                                Thread.Sleep(100);
                                //MSIAfterburner.CommitChanges(false);
                            }
                        }
                    }
                    MSIAfterburner.Flush();
                    Thread.Sleep(100);
                }
                else
                {

                }
            } else
            {
                if (isBefore)
                {
                    Thread.Sleep(1000);
                }
            }

            bool CreateNoWindow = false;
            var CMDconfigHandle = new Process
            {
                StartInfo =
                {
                    FileName = MiningSetup.MinerPath
                }
            };

            try
            {
                var strPlatform = "";
                var strDual = "SINGLE";
                var strAlgo = AlgorithmNiceHashNames.GetName(MiningSetup.CurrentAlgorithmType);

                var minername = MinerTag();
                int subStr;
                subStr = MinerTag().IndexOf("-");
                if (subStr > 0)
                {
                    minername = MinerTag().Substring(0, subStr);
                }
                if (minername == "ClaymoreCryptoNight" || minername == "ClaymoreZcash" || minername == "ClaymoreDual" || minername == "ClaymoreNeoscrypt")
                {
                    minername = "Claymore";
                }
                minername = minername.Replace("Z-Enemy", "ZEnemy");

                var gpus = "";
                List<string> l = MiningSetup.MiningPairs.Select(mPair => mPair.Device.IDByBus.ToString()).ToList();
                l.Sort();
                gpus += string.Join(",", l);

                foreach (var pair in MiningSetup.MiningPairs)
                {
                    if (pair.Algorithm.DualNiceHashID == AlgorithmType.AutolykosZil ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.DaggerAutolykos ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.AutolykosIronFish ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.DaggerIronFish ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.ETCHashIronFish ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.AutolykosKarlsenHash ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.DaggerKarlsenHash ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.FishHashAlephium ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.FishHashKarlsenHash ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.OctopusKarlsenHash ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.ETCHashKarlsenHash ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.AutolykosAlephium ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.OctopusAlephium ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.ETCHashAlephium ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.DaggerAlephium ||
                        pair.Algorithm.DualNiceHashID == AlgorithmType.OctopusIronFish)
                    {
                        strDual = "DUAL";
                    }
                    if (pair.Device.DeviceType == DeviceType.NVIDIA)
                    {
                        strPlatform = "NVIDIA";
                    }
                    else if (pair.Device.DeviceType == DeviceType.AMD)
                    {
                        strPlatform = "AMD";
                    }
                    else if (pair.Device.DeviceType == DeviceType.INTEL)
                    {
                        strPlatform = "INTEL";
                    }
                    else if (pair.Device.DeviceType == DeviceType.CPU)
                    {
                        strPlatform = "CPU";
                    }
                }

                string MinerDir = MiningSetup.MinerPath.Substring(0, MiningSetup.MinerPath.LastIndexOf("\\"));
                if (isBefore)
                {
                    CMDconfigHandle.StartInfo.FileName = "GPU-Scrypt.cmd";
                }
                else
                {
                    CMDconfigHandle.StartInfo.FileName = "GPU-Reset.cmd";
                }

                {
                    var cmd = "";
                    FileStream fs = new FileStream(CMDconfigHandle.StartInfo.FileName, FileMode.Open, FileAccess.Read);
                    StreamReader w = new StreamReader(fs);
                    cmd = w.ReadToEnd();
                    w.Close();

                    if (cmd.ToUpper().Trim().Contains("SET NOVISIBLE=TRUE"))
                    {
                        CreateNoWindow = true;
                    }
                    if (cmd.ToUpper().Trim().Contains("SET RUN=FALSE"))
                    {
                        return null;
                    }
                }
                Helpers.ConsolePrint(MinerTag(), "Using CMD: " + CMDconfigHandle.StartInfo.FileName);

                if (MinersSettingsManager.MinerSystemVariables.ContainsKey(Path))
                {
                    foreach (var kvp in MinersSettingsManager.MinerSystemVariables[Path])
                    {
                        var envName = kvp.Key;
                        var envValue = kvp.Value;
                        CMDconfigHandle.StartInfo.EnvironmentVariables[envName] = envValue;
                    }
                }

                CMDconfigHandle.StartInfo.Arguments = " " + strPlatform + " " + strDual + " " + strAlgo + " \"" + gpus + "\"" + " " + minername;
                CMDconfigHandle.StartInfo.UseShellExecute = false;
                // CMDconfigHandle.StartInfo.RedirectStandardError = true;
                // CMDconfigHandle.StartInfo.RedirectStandardOutput = true;
                CMDconfigHandle.StartInfo.CreateNoWindow = CreateNoWindow;

                Helpers.ConsolePrint(MinerTag(), "Start CMD: " + CMDconfigHandle.StartInfo.FileName + CMDconfigHandle.StartInfo.Arguments);
                CMDconfigHandle.Start();


                try
                {
                    if (!CMDconfigHandle.WaitForExit(60 * 1000))
                    {
                        CMDconfigHandle.Kill();
                        CMDconfigHandle.WaitForExit(5 * 1000);
                        CMDconfigHandle.Close();
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("KillCMDBeforeOrAfterMining", e.ToString());
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint(MinerTag(), ex.ToString());
            }
            return CMDconfigHandle;
        }
    }
}
