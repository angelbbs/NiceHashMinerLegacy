/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Interfaces;
using NiceHashMiner.Miners.Grouping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Stats;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using Timer = System.Timers.Timer;
using static NiceHashMiner.Devices.ComputeDeviceManager;
using System.Threading;
using System.Globalization;
using System.Management;
using System.Diagnostics;
using System.Reflection;
using NiceHashMinerLegacy.Divert;

namespace NiceHashMiner.Miners
{
    using GroupedDevices = SortedSet<string>;

    public class MiningSession
    {
        private const string Tag = "MiningSession";
        private const string DoubleFormat = "F12";

        // session varibles fixed
        public static  string _miningLocation;

        public static  string _btcAdress;
        public static  string _worker;
        private List<MiningDevice> _miningDevices;
        private readonly IMainFormRatesComunication _mainFormRatesComunication;

        private readonly AlgorithmSwitchingManager _switchingManager;

        // session varibles changing
        // GroupDevices hash code doesn't work correctly use string instead
        //Dictionary<GroupedDevices, GroupMiners> _groupedDevicesMiners;
        public static Dictionary<string, GroupMiner> _runningGroupMiners = new Dictionary<string, GroupMiner>();

        private GroupMiner _ethminerNvidiaPaused;
        private GroupMiner _ethminerAmdPaused;


        private bool _isProfitable;

        private bool _isConnectedToInternet;
        private readonly bool _isMiningRegardlesOfProfit;

        // timers
        private readonly Timer _preventSleepTimer;
        public static Timer _smaCheckTimer;

        // check internet connection
        private readonly Timer _internetCheckTimer;
        public static bool FuncAttached = false;

        public bool IsMiningEnabled => _miningDevices.Count > 0;

        private bool IsCurrentlyIdle => !IsMiningEnabled || !_isConnectedToInternet || !_isProfitable;

        private static Action nonStaticMethodAction;
        
        public List<int> ActiveDeviceIndexes
        {
            get
            {
                var minerIDs = new List<int>();
                if (!IsCurrentlyIdle)
                {
                    foreach (var miner in _runningGroupMiners.Values)
                    {
                        minerIDs.AddRange(miner.DevIndexes);
                    }
                }

                return minerIDs;
            }
        }


        
        public static void StopEvent()
        {
            //NiceHashMiner.Switching.AlgorithmSwitchingManager.
            //SwichMostProfitableGroupUpMethod.
            //NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheck = null;
            /*
            foreach (EventHandler eh in NiceHashMiner.Switching.AlgorithmSwitchingManager)
            {
                MyRealEvent -= eh;
            }
            delegates.Clear();
            */
            //EventHandler<SmaUpdateEventArgs> SwichMostProfitableGroupUpMethod = new SwichMostProfitableGroupUpMethod();
            //NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheck -= SwichMostProfitableGroupUpMethod;
        }
        
        public MiningSession(List<ComputeDevice> devices,
            IMainFormRatesComunication mainFormRatesComunication,
            string miningLocation, string worker, string btcAdress)
        {
            // init fixed
            _mainFormRatesComunication = mainFormRatesComunication;
            _miningLocation = miningLocation;
            _switchingManager = new AlgorithmSwitchingManager();
            //NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheck -= this.SwichMostProfitableGroupUpMethod;
            if (!FuncAttached)
            {
                Helpers.ConsolePrint("MiningSession", "Process attached");
                NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheck += SwichMostProfitableGroupUpMethod;
                
                FuncAttached = true;
            }
            //_switchingManager.SmaCheck += SwichMostProfitableGroupUpMethod;

            _btcAdress = btcAdress;
            _worker = worker;

            // initial settup
            _miningDevices = GroupSetupUtils.GetMiningDevices(devices, true);
            if (_miningDevices.Count > 0)
            {
                GroupSetupUtils.AvarageSpeeds(_miningDevices);
            }

            // init timer stuff
            _preventSleepTimer = new Timer();
            _preventSleepTimer.Elapsed += PreventSleepTimer_Tick;
            // sleep time is minimal 1 minute
            _preventSleepTimer.Interval = 20 * 1000; // leave this interval, it works

            // set internet checking
            _internetCheckTimer = new Timer();
            _internetCheckTimer.Elapsed += InternetCheckTimer_Tick;
            _internetCheckTimer.Interval = 1 * 1000 * 60; // every minute

            // assume profitable
            _isProfitable = true;
            // assume we have internet
            _isConnectedToInternet = true;

            if (IsMiningEnabled)
            {
                _preventSleepTimer.Start();
                _internetCheckTimer.Start();
            }

            /*
            _switchingManager.Stop();
            _switchingManager.Start();
            */
            AlgorithmSwitchingManager.Stop();
            AlgorithmSwitchingManager.Start();
            _isMiningRegardlesOfProfit = ConfigManager.GeneralConfig.MinimumProfit == 0;
            //NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheck -= SwichMostProfitableGroupUpMethod;
        }

        #region Timers stuff

        private void InternetCheckTimer_Tick(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.IdleWhenNoInternetAccess)
            {
                _isConnectedToInternet = Helpers.IsConnectedToInternet();
            }
        }

        private void PreventSleepTimer_Tick(object sender, ElapsedEventArgs e)
        {
            // when mining keep system awake, prevent sleep
            Helpers.PreventSleep();
        }

        #endregion

        #region Start/Stop
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

        public static void RestartMiner(string ProcessTag)
        {
            if (_runningGroupMiners != null)
            {
                foreach (var groupMiner in _runningGroupMiners.Values)
                {
                    if (groupMiner.Miner.ProcessTag().Contains(ProcessTag))
                    {
                        try
                        {
                            int k = ProcessTag.IndexOf("pid(");
                            int i = ProcessTag.IndexOf(")|bin");
                            var cpid = ProcessTag.Substring(k + 4, i - k - 4).Trim();

                            int pid = int.Parse(cpid, CultureInfo.InvariantCulture);
                            KillProcessAndChildren(pid);
                        }
                        catch (Exception e)
                        {
                            Helpers.ConsolePrint("Restart miner", "RestartMiner(): " + e.Message);
                        }
                    }
                    //Form_Main.ActiveForm.Focus();//костыль. иначе появляется бордюр у кнопки
                }

//                _runningGroupMiners = new Dictionary<string, GroupMiner>();
            }
        }

        public void StopAllMiners()
        {
            if (_runningGroupMiners != null)
            {
                foreach (var groupMiner in _runningGroupMiners.Values)
                {
                    groupMiner.End();
                }

                _runningGroupMiners = new Dictionary<string, GroupMiner>();
            }

            if (_ethminerNvidiaPaused != null)
            {
                _ethminerNvidiaPaused.End();
                _ethminerNvidiaPaused = null;
            }

            if (_ethminerAmdPaused != null)
            {
                _ethminerAmdPaused.End();
                _ethminerAmdPaused = null;
            }

            //_switchingManager.Stop();
            AlgorithmSwitchingManager.Stop();

            _mainFormRatesComunication?.ClearRatesAll();

            // restroe/enable sleep
            _preventSleepTimer.Stop();
            _internetCheckTimer.Stop();
            Helpers.AllowMonitorPowerdownAndSleep();
        }

        public void StopAllMinersNonProfitable()
        {
            if (_runningGroupMiners != null)
            {
                foreach (var groupMiner in _runningGroupMiners.Values)
                {
                    groupMiner.End();
                }

                _runningGroupMiners = new Dictionary<string, GroupMiner>();
            }

            if (_ethminerNvidiaPaused != null)
            {
                _ethminerNvidiaPaused.End();
                _ethminerNvidiaPaused = null;
            }

            if (_ethminerAmdPaused != null)
            {
                _ethminerAmdPaused.End();
                _ethminerAmdPaused = null;
            }

            _mainFormRatesComunication?.ClearRates(-1);
        }

        #endregion Start/Stop

        private static string CalcGroupedDevicesKey(GroupedDevices group)
        {
            return string.Join(", ", group);
        }

        public string GetActiveMinersGroup()
        {
            if (IsCurrentlyIdle)
            {
                return "IDLE";
            }

            var activeMinersGroup = "";

            //get unique miner groups like CPU, NVIDIA, AMD,...
            var uniqueMinerGroups = new HashSet<string>();
            foreach (var miningDevice in _miningDevices)
            {
                //if (miningDevice.MostProfitableKey != AlgorithmType.NONE) {
                uniqueMinerGroups.Add(GroupNames.GetNameGeneral(miningDevice.Device.DeviceType));
                //}
            }

            if (uniqueMinerGroups.Count > 0 && _isProfitable)
            {
                activeMinersGroup = string.Join("/", uniqueMinerGroups);
            }

            return activeMinersGroup;
        }

        public double GetTotalRate()
        {
            double totalRate = 0;

            if (_runningGroupMiners != null)
            {
                totalRate += _runningGroupMiners.Values.Sum(groupMiner => groupMiner.CurrentRate);
            }

            return totalRate;
        }
        public double GetTotalPowerRate()
        {
            double totalPowerRate = 0;

            if (_runningGroupMiners != null)
            {
                totalPowerRate += _runningGroupMiners.Values.Sum(groupMiner => groupMiner.PowerRate);
            }

            return totalPowerRate;
        }
        public double GetTotalPower()
        {
            double totalPower = 0;

            if (_runningGroupMiners != null)
            {
                totalPower += _runningGroupMiners.Values.Sum(groupMiner => groupMiner.TotalPower);
            }

            return totalPower;
        }
        // full of state
        private bool CheckIfProfitable(double currentProfit, bool log = true)
        {
            // TODO FOR NOW USD ONLY
            var currentProfitUsd = (currentProfit * ExchangeRateApi.GetUsdExchangeRate());
            _isProfitable =
                _isMiningRegardlesOfProfit
                || !_isMiningRegardlesOfProfit && currentProfitUsd >= ConfigManager.GeneralConfig.MinimumProfit;
            if (log)
            {
                Helpers.ConsolePrint(Tag, "Current Global profit: " + currentProfitUsd.ToString("F8") + " USD/Day");
                if (!_isProfitable)
                {
                    Helpers.ConsolePrint(Tag,
                        "Current Global profit: NOT PROFITABLE MinProfit " +
                        ConfigManager.GeneralConfig.MinimumProfit.ToString("F8") +
                        " USD/Day");
                }
                else
                {
                    var profitabilityInfo = _isMiningRegardlesOfProfit
                        ? "mine always regardless of profit"
                        : ConfigManager.GeneralConfig.MinimumProfit.ToString("F8") + " USD/Day";
                    Helpers.ConsolePrint(Tag, "Current Global profit: IS PROFITABLE MinProfit " + profitabilityInfo);
                }
            }

            return _isProfitable;
        }

        private bool CheckIfShouldMine(double currentProfit, bool log = true)
        {
            // if profitable and connected to internet mine
            var shouldMine = CheckIfProfitable(currentProfit, log) && _isConnectedToInternet;
            if (shouldMine)
            {
                _mainFormRatesComunication.HideNotProfitable();
            }
            else
            {
                if (!_isConnectedToInternet)
                {
                    // change msg
                    if (log) Helpers.ConsolePrint(Tag, "NO INTERNET!!! Stopping mining.");
                    _mainFormRatesComunication.ShowNotProfitable(
                        International.GetText("Form_Main_MINING_NO_INTERNET_CONNECTION"));
                }
                else
                {
                    _mainFormRatesComunication.ShowNotProfitable(
                        International.GetText("Form_Main_MINING_NOT_PROFITABLE"));
                }

                // return don't group
                StopAllMinersNonProfitable();
            }

            return shouldMine;
        }

       
        public void SwichMostProfitableGroupUpMethod(object sender, SmaUpdateEventArgs e)
        {
#if (SWITCH_TESTING)
            MiningDevice.SetNextTest();
#endif
            AlgorithmSwitchingManager.SmaCheckTimerOnElapsedRun = true;
            var profitableDevices = new List<MiningPair>();
            var currentProfit = 0.0d;
            var prevStateProfit = 0.0d;
            foreach (var device in _miningDevices)
            {
                // calculate profits
                device.CalculateProfits(e.NormalizedProfits);
                // check if device has profitable algo
                if (device.HasProfitableAlgo())
                {
                    profitableDevices.Add(device.GetMostProfitablePair());
                    currentProfit += device.GetCurrentMostProfitValue;
                    prevStateProfit += device.GetPrevMostProfitValue;
                }
            }
            var stringBuilderFull = new StringBuilder();
            stringBuilderFull.AppendLine("Current device profits:");
            double smaTmp = 0;
            foreach (var device in _miningDevices)
            {
                var stringBuilderDevice = new StringBuilder();
                stringBuilderDevice.AppendLine($"\tProfits for {device.Device.Uuid} ({device.Device.GetFullName()}):");
                
                foreach (var algo in device.Algorithms)
                {
                    smaTmp = smaTmp + algo.CurNhmSmaDataVal;
                    stringBuilderDevice.AppendLine(
                        $"\tPROFIT = {Math.Round(algo.CurrentProfit, 10).ToString(DoubleFormat).PadRight(17)}" +
                            $"\tSPEED = {Math.Round(algo.AvaragedSpeed, 3).ToString().PadRight(13)}" +
                            $"\tNHSMA = {algo.CurNhmSmaDataVal.ToString().PadRight(21)}" +
                            $"\t{algo.AlgorithmStringID.PadRight(28)}" +
                            $"\t less than {device.GetMostProfitableString()} {(((device.GetCurrentMostProfitValue - algo.CurrentProfit) / device.GetCurrentMostProfitValue) * 100):0.00}%"
                        );
                    if (algo is DualAlgorithm dualAlg)
                    {
                        stringBuilderDevice.AppendLine(
                            $"\t\t\t\t  Secondary:\t\t {dualAlg.SecondaryAveragedSpeed:e5}" +
                                $"\t\t\t  {dualAlg.SecondaryCurNhmSmaDataVal:e5}"
                            );
                    }
                }
                // most profitable
                stringBuilderDevice.AppendLine(
                    $"\tMOST PROFITABLE ALGO: {device.GetMostProfitableString()}, PROFIT: {device.GetCurrentMostProfitValue.ToString(DoubleFormat)}");
                stringBuilderFull.AppendLine(stringBuilderDevice.ToString());
            }
            Helpers.ConsolePrint(Tag, stringBuilderFull.ToString());
            if (smaTmp == 0)
            {
                Helpers.ConsolePrint(Tag, "SMA Error. Restart program");
                Form_Main.MakeRestart(0);
                return;
            }
            // check if should mine
            // Only check if profitable inside this method when getting SMA data, cheching during mining is not reliable
            if (CheckIfShouldMine(currentProfit) == false)
            {
                foreach (var device in _miningDevices)
                {
                    device.SetNotMining();
                }
                AlgorithmSwitchingManager.SmaCheckTimerOnElapsedRun = false;
                return;
            }

            // check profit threshold
            bool needSwitch = false;
            foreach (var device in _miningDevices)
            {
                Helpers.ConsolePrint(Tag, $"PrevStateProfit {prevStateProfit}, CurrentProfit {currentProfit}");
                //if (prevStateProfit > 0 && currentProfit > 0)
                {
                    var a = Math.Max(prevStateProfit, currentProfit);
                    var b = Math.Min(prevStateProfit, currentProfit);
                    //double percDiff = Math.Abs((PrevStateProfit / CurrentProfit) - 1);
                    var percDiff = ((a - b)) / Math.Abs(b);
                    if (percDiff <= ConfigManager.GeneralConfig.SwitchProfitabilityThreshold)
                    {
                        // don't switch
                        Helpers.ConsolePrint(Tag,
                            $"{device.Device.GetFullName()}: Will NOT SWITCH profit diff is {percDiff * 100}%, current threshold {ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100}%");
                        // RESTORE OLD PROFITS STATE
                        //foreach (var device in _miningDevices)
                        {
                            device.RestoreOldProfitsState();
                        }

                        //return;
                    }
                    else
                    {
                        if (device.GetMostProfitableString().Contains("DaggerHashimoto3GB") && ConfigManager.GeneralConfig.DivertRun)
                        {
                            if (Divert.DaggerHashimoto3GBForce)
                            {
                                if (Divert.DaggerHashimoto3GBProfit && DHClient.checkConnection)
                                {
                                    //Divert.Dagger3GBEpochCount = 0;
                                    //Divert.DaggerHashimoto3GBForce = false;
                                    //DHClient.checkConnection = false;

                                }
                            }
                        }

                                    needSwitch = true;
                        Helpers.ConsolePrint(Tag,
                            $"Will SWITCH profit diff is {percDiff * 100}%, current threshold {ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100}%");
                    }
                }
            }
            if (!needSwitch)
            {
                AlgorithmSwitchingManager.SmaCheckTimerOnElapsedRun = false;
                return;
            }
            // group new miners
            var newGroupedMiningPairs = new Dictionary<string, List<MiningPair>>();
            // group devices with same supported algorithms
            {
                var currentGroupedDevices = new List<GroupedDevices>();
                for (var first = 0; first < profitableDevices.Count; ++first)
                {
                    var firstDev = profitableDevices[first].Device;
                    // check if is in group
                    var isInGroup = currentGroupedDevices.Any(groupedDevices => groupedDevices.Contains(firstDev.Uuid));
                    // if device is not in any group create new group and check if other device should group
                    if (isInGroup == false)
                    {
                        var newGroup = new GroupedDevices();
                        var miningPairs = new List<MiningPair>()
                        {
                            profitableDevices[first]
                        };
                        newGroup.Add(firstDev.Uuid);
                        for (var second = first + 1; second < profitableDevices.Count; ++second)
                        {
                            // check if we should group
                            var firstPair = profitableDevices[first];
                            var secondPair = profitableDevices[second];
                            if (GroupingLogic.ShouldGroup(firstPair, secondPair))
                            {
                                var secondDev = profitableDevices[second].Device;
                                newGroup.Add(secondDev.Uuid);
                                miningPairs.Add(profitableDevices[second]);
                            }
                        }

                        currentGroupedDevices.Add(newGroup);
                        newGroupedMiningPairs[CalcGroupedDevicesKey(newGroup)] = miningPairs;
                    }
                }
            }
            //bool IsMinerStatsCheckUpdate = false;
            {
                // check which groupMiners should be stopped and which ones should be started and which to keep running
                var toStopGroupMiners = new Dictionary<string, GroupMiner>();
                var toRunNewGroupMiners = new Dictionary<string, GroupMiner>();
                var noChangeGroupMiners = new Dictionary<string, GroupMiner>();
                // check what to stop/update
                foreach (var runningGroupKey in _runningGroupMiners.Keys)
                {
                    if (newGroupedMiningPairs.ContainsKey(runningGroupKey) == false)
                    {
                        // runningGroupKey not in new group definately needs to be stopped and removed from curently running
                        toStopGroupMiners[runningGroupKey] = _runningGroupMiners[runningGroupKey];
                    }
                    else
                    {
                        // runningGroupKey is contained but needs to check if mining algorithm is changed
                        var miningPairs = newGroupedMiningPairs[runningGroupKey];
                        var newAlgoType = GetMinerPairAlgorithmType(miningPairs);
                        if (newAlgoType != AlgorithmType.NONE && newAlgoType != AlgorithmType.INVALID)
                        {
                            // Check if dcri optimal value has changed
                            var dcriChanged = false;
                            foreach (var mPair in _runningGroupMiners[runningGroupKey].Miner.MiningSetup.MiningPairs)
                            {
                                if (mPair.Algorithm is DualAlgorithm algo
                                    && algo.TuningEnabled
                                    && algo.MostProfitableIntensity != algo.CurrentIntensity)
                                {
                                    dcriChanged = true;
                                    break;
                                }
                            }

                            // if algoType valid and different from currently running update
                            if (newAlgoType != _runningGroupMiners[runningGroupKey].DualAlgorithmType || dcriChanged)
                            {
                                // remove current one and schedule to stop mining
                                toStopGroupMiners[runningGroupKey] = _runningGroupMiners[runningGroupKey];
                                // create new one TODO check if DaggerHashimoto
                                GroupMiner newGroupMiner = null;
                                if (newAlgoType == AlgorithmType.DaggerHashimoto)
                                {
                                    if (_ethminerNvidiaPaused != null && _ethminerNvidiaPaused.Key == runningGroupKey)
                                    {
                                        newGroupMiner = _ethminerNvidiaPaused;
                                    }

                                    if (_ethminerAmdPaused != null && _ethminerAmdPaused.Key == runningGroupKey)
                                    {
                                        newGroupMiner = _ethminerAmdPaused;
                                    }
                                }

                                if (newGroupMiner == null)
                                {
                                    newGroupMiner = new GroupMiner(miningPairs, runningGroupKey);
                                }

                                toRunNewGroupMiners[runningGroupKey] = newGroupMiner;
                            }
                            else
                                noChangeGroupMiners[runningGroupKey] = _runningGroupMiners[runningGroupKey];
                        }
                    }
                }

                // check brand new
                foreach (var kvp in newGroupedMiningPairs)
                {
                    var key = kvp.Key;
                    var miningPairs = kvp.Value;
                    if (_runningGroupMiners.ContainsKey(key) == false)
                    {
                        var newGroupMiner = new GroupMiner(miningPairs, key);
                        toRunNewGroupMiners[key] = newGroupMiner;
                    }
                }

                if ((toStopGroupMiners.Values.Count > 0) || (toRunNewGroupMiners.Values.Count > 0))
                {
                    var stringBuilderPreviousAlgo = new StringBuilder();
                    var stringBuilderCurrentAlgo = new StringBuilder();
                    var stringBuilderNoChangeAlgo = new StringBuilder();

                    // stop old miners
                    foreach (var toStop in toStopGroupMiners.Values)
                    {
                        stringBuilderPreviousAlgo.Append($"{toStop.DevicesInfoString}: {toStop.AlgorithmType}, ");

                        toStop.Stop();
                        _runningGroupMiners.Remove(toStop.Key);
                        // TODO check if daggerHashimoto and save
                        if (toStop.AlgorithmType == AlgorithmType.DaggerHashimoto)
                        {
                            if (toStop.DeviceType == DeviceType.NVIDIA)
                            {
                                _ethminerNvidiaPaused = toStop;
                            }
                            else if (toStop.DeviceType == DeviceType.AMD)
                            {
                                _ethminerAmdPaused = toStop;
                            }
                        }
                    }

                    // start new miners
                    foreach (var toStart in toRunNewGroupMiners.Values)
                    {
                        stringBuilderCurrentAlgo.Append($"{toStart.DevicesInfoString}: {toStart.AlgorithmType}, ");

                        toStart.Start(_miningLocation, _btcAdress, _worker);
                        _runningGroupMiners[toStart.Key] = toStart;
                    }

                    // which miners dosen't change
                    foreach (var noChange in noChangeGroupMiners.Values)
                        stringBuilderNoChangeAlgo.Append($"{noChange.DevicesInfoString}: {noChange.AlgorithmType}, ");

                    if (stringBuilderPreviousAlgo.Length > 0)
                        Helpers.ConsolePrint(Tag, $"Stop Mining: {stringBuilderPreviousAlgo}");

                    if (stringBuilderCurrentAlgo.Length > 0)
                        Helpers.ConsolePrint(Tag, $"Now Mining : {stringBuilderCurrentAlgo}");

                    if (stringBuilderNoChangeAlgo.Length > 0)
                        Helpers.ConsolePrint(Tag, $"No change  : {stringBuilderNoChangeAlgo}");
                }
            }

            // stats quick fix code
            //if (_currentAllGroupedDevices.Count != _previousAllGroupedDevices.Count) {
            //await MinerStatsCheck();
            //}
            AlgorithmSwitchingManager.SmaCheckTimerOnElapsedRun = false;
            _mainFormRatesComunication?.ForceMinerStatsUpdate();
        }

        private AlgorithmType GetMinerPairAlgorithmType(List<MiningPair> miningPairs)
        {
            if (miningPairs.Count > 0)
            {
                return miningPairs[0].Algorithm.DualNiceHashID;
            }

            return AlgorithmType.NONE;
        }

        public async Task MinerStatsCheck()
        {
            var currentProfit = 0.0d;
            _mainFormRatesComunication.ClearRates(_runningGroupMiners.Count);
            var checks = new List<GroupMiner>(_runningGroupMiners.Values);
            try
            {
                foreach (var groupMiners in checks)
                {
                    var m = groupMiners.Miner;

                    // skip if not running or if await already in progress
                    // if (!Miner.IsRunning || m.IsUpdatingApi) continue;
                    if (!m.IsRunning || m.IsUpdatingApi) continue;
                    // continue;

                    m.IsUpdatingApi = true;
                    var ad = await m.GetSummaryAsync();
                    m.IsUpdatingApi = false;
                    if (ad == null)
                    {
                        Helpers.ConsolePrint(m.MinerTag(), "GetSummary returned null..");
                    }

                    // set rates
                    //NHSmaData.TryGetPaying(ad.AlgorithmID, out var paying);
                    if (ad != null && NHSmaData.TryGetPaying(ad.AlgorithmID, out var paying))
                    {
                        groupMiners.CurrentRate = paying * ad.Speed * 0.000000001;
                        NHSmaData.TryGetPaying(ad.SecondaryAlgorithmID, out var secPaying);
                        double CurrentRateSec = secPaying * ad.SecondarySpeed * 0.000000001;
                        //if (NHSmaData.TryGetPaying(ad.SecondaryAlgorithmID, out var secPaying))

                            groupMiners.CurrentRate += secPaying * ad.SecondarySpeed * 0.000000001;


                        //Helpers.ConsolePrint(m.MinerTag(), "groupMiners.CurrentRate: " + groupMiners.CurrentRate.ToString());
                        // Deduct power costs
                        //var powerUsage = ad.PowerUsage > 0 ? ad.PowerUsage : groupMiners.TotalPower;
                        double powerUsage = 0;
                        /*
                        // если групп > 1, то задваивается
                        foreach (var computeDevice in Available.Devices)
                        {
                            powerUsage += computeDevice.PowerUsage;
                        }
                        */
                        double psuE = (double)ConfigManager.GeneralConfig.PowerPSU / 100;
                        //groupMiners.CurrentRate -= ExchangeRateApi.GetKwhPriceInBtc() * powerUsage * 24 / 1000;
                        double totalPowerUsage = (powerUsage + (int)ConfigManager.GeneralConfig.PowerMB) / psuE;
                        groupMiners.PowerRate = ExchangeRateApi.GetKwhPriceInBtc() * totalPowerUsage * 24 / 1000;
                    }
                    else
                    {
                        if (groupMiners.AlgorithmType != AlgorithmType.DaggerHashimoto3GB || groupMiners.AlgorithmType != AlgorithmType.DaggerHashimoto4GB)
                        {
                            groupMiners.CurrentRate = 0;
                        }
                        // set empty
                        //if (groupMiners.DualAlgorithmType != AlgorithmType.NONE || groupMiners.AlgorithmType == AlgorithmType.Eaglesong)
                       // if (groupMiners.DualAlgorithmType == AlgorithmType.Eaglesong)

                            ad = new ApiData(groupMiners.DualAlgorithmType);

                        /*
                        else
                        {
                            ad = new ApiData(groupMiners.AlgorithmType);
                        }
                        */
                    }
                    currentProfit += groupMiners.CurrentRate;
                    // Update GUI
                    _mainFormRatesComunication.AddRateInfo(m.MinerTag(), groupMiners.DevicesInfoString, ad,
                        groupMiners.CurrentRate, groupMiners.PowerRate, groupMiners.StartMinerTime,
                        m.IsApiReadException, m.ProcessTag());
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("Exception: ", e.ToString());
                //Helpers.ConsolePrint(Tag, e.Message);
            }
        }
    }
}
