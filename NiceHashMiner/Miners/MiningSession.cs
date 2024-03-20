using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Forms;
using NiceHashMiner.Interfaces;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Stats;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace NiceHashMiner.Miners
{
    using GroupedDevices = SortedSet<string>;

    public class MiningSession
    {
        private const string Tag = "MiningSession";
        private const string DoubleFormat = "F12";

        // session varibles fixed
        //public static string _miningLocation;

        public static string _btcAdress;
        public static string _worker;
        public List<MiningDevice> _miningDevices;
        private readonly IMainFormRatesComunication _mainFormRatesComunication;

        private readonly AlgorithmSwitchingManager _switchingManager;

        // session varibles changing
        // GroupDevices hash code doesn't work correctly use string instead
        //Dictionary<GroupedDevices, GroupMiners> _groupedDevicesMiners;
        public static Dictionary<string, GroupMiner> _runningGroupMiners = new Dictionary<string, GroupMiner>();

        private GroupMiner _ethminerNvidiaPaused;
        private GroupMiner _ethminerAmdPaused;
        //private static int _tick = 0;

        private bool _isProfitable;

        private bool _isConnectedToInternet;
        private readonly bool _isMiningRegardlesOfProfit;

        // timers
        private readonly Timer _preventSleepTimer;
        //public static Timer _smaCheckTimer;

        // check internet connection
        private readonly Timer _internetCheckTimer;
        public static bool FuncAttached = false;

        public bool IsMiningEnabled => _miningDevices.Count > 0;

        private bool IsCurrentlyIdle => !IsMiningEnabled || !_isConnectedToInternet || !_isProfitable;
        public static int[] _ticks;

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

        }
        /*
        public static List<string> GetResolvedServers(string algo)
        {
            List<string> ResolvedServers = new List<string>();
            for (int i = 0; i < Globals.MiningLocation.Length; i++)
            {
                string _server = Links.CheckDNS($"stratum+tcp://{algo}.{Globals.MiningLocation[i]}", false);
                ResolvedServers.Add(_server);
            }
            return ResolvedServers;
        }
        */
        public MiningSession(List<ComputeDevice> devices,
            IMainFormRatesComunication mainFormRatesComunication,
            string worker, string btcAdress)
        {
            _ticks = new int[devices.Count];
            for (int d = 0; d < _ticks.Length; d++)
            {
                _ticks[d] = 999;
            }
            // init fixed
            _mainFormRatesComunication = mainFormRatesComunication;
            // _miningLocation = miningLocation;
            _switchingManager = new AlgorithmSwitchingManager();
            if (!FuncAttached)
            {
                Helpers.ConsolePrint("MiningSession", "Process attached");
                NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheck += SwichMostProfitableGroupUpMethod;

                FuncAttached = true;
            }
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
            AlgorithmSwitchingManager.Stop();
            AlgorithmSwitchingManager.Start();
            _isMiningRegardlesOfProfit = ConfigManager.GeneralConfig.MinimumProfit == 0;

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
                try
                {
                    foreach (var groupMiner in _runningGroupMiners.Values)
                    {
                        groupMiner.End();
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("StopAllMiners", e.ToString());
                }
                _runningGroupMiners = new Dictionary<string, GroupMiner>();
            }


            //_switchingManager.Stop();
            AlgorithmSwitchingManager.Stop();

            _mainFormRatesComunication?.ClearRatesAll();

            // restroe/enable sleep
            _preventSleepTimer.Stop();
            _internetCheckTimer.Stop();
            Helpers.AllowMonitorPowerdownAndSleep();
            new Task(() => NiceHashStats.SetDeviceStatus(null, true, "StopAllMiners")).Start();
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
            var profitableDevices = new List<MiningPair>();
            var currentProfitWithoutPower = 0.0d;
            //var mostProfitWithoutPower = 0.0d;
            //var mostProfitWithPower = 0.0d;

            foreach (var device in _miningDevices)
            {
                // calculate profits
                //device.CalculateProfits(e.NormalizedProfits);
                // check if device has profitable algo
                if (device.HasProfitableAlgo())
                {
                    profitableDevices.Add(device.GetMostProfitablePair());
                    //mostProfitWithoutPower += device.GetCurrentMostProfitValueWithoutPower;
                    currentProfitWithoutPower += device.GetCurrentMostProfitValueWithoutPower;
                }
            }

            var totalRate = MinersManager.GetTotalRate();
            var currentProfitUsd = (currentProfit * ExchangeRateApi.GetUsdExchangeRate());
            _isProfitable =
                _isMiningRegardlesOfProfit
                || !_isMiningRegardlesOfProfit && currentProfitUsd >= ConfigManager.GeneralConfig.MinimumProfit;
            if (log)
            {
                Helpers.ConsolePrint(Tag, "Mining profit: " + BTC2Fiat(totalRate) + " Most profit: " + BTC2Fiat(currentProfitWithoutPower) + " with power: " + currentProfitUsd.ToString("F8") + " USD/Day");
                if (!_isProfitable)
                {
                    Helpers.ConsolePrint(Tag,
                        "Current Global profit: NOT PROFITABLE MinProfit " +
                        ConfigManager.GeneralConfig.MinimumProfit.ToString("F8") +
                        " USD/Day");
                }
                else
                {
                    /*
                    var profitabilityInfo = _isMiningRegardlesOfProfit
                        ? "mine always regardless of profit"
                        : ConfigManager.GeneralConfig.MinimumProfit.ToString("F8") + " USD/Day";
                    Helpers.ConsolePrint(Tag, "Current Global profit: IS PROFITABLE MinProfit " + profitabilityInfo);
                    */
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
                    if (ConfigManager.GeneralConfig.Force_mining_if_nonprofitable)
                    {
                        shouldMine = true;
                    }
                    else
                    {
                        _mainFormRatesComunication.ShowNotProfitable(
                            International.GetText("Form_Main_MINING_NOT_PROFITABLE"));
                    }
                }

                // return don't group
                StopAllMinersNonProfitable();
            }

            return shouldMine;
        }

        private string BTC2Fiat(double btc)
        {
            return ExchangeRateApi.ConvertToActiveCurrency(btc * ExchangeRateApi.GetUsdExchangeRate()).ToString("F2") + " " + ExchangeRateApi.ActiveDisplayCurrency;
        }

        private double prev_percDiff = 0.0d;

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
                    if (ConfigManager.GeneralConfig.with_power)
                    {
                        currentProfit += device.GetCurrentMostProfitValue;
                        prevStateProfit += device.GetPrevMostProfitValue;
                    }
                    else
                    {
                        currentProfit += device.GetCurrentMostProfitValueWithoutPower;
                        prevStateProfit += device.GetPrevMostProfitValueWithoutPower;
                    }
                }
            }
            var stringBuilderFull = new StringBuilder();
            stringBuilderFull.AppendLine("Current device profits:");
            double smaTmp = 0;

            Form_Main.KawpowLiteEnabled = false;
            foreach (var device in _miningDevices)
            {
                foreach (var algo in device.Algorithms)
                {
                    if (algo.NiceHashID == AlgorithmType.KAWPOWLite)
                    {
                        Form_Main.KawpowLiteEnabled = true;
                    }
                    smaTmp = smaTmp + algo.CurNhmSmaDataVal;
                }
                // most profitable
                string profitStr = BTC2Fiat(device.GetCurrentMostProfitValueWithoutPower);
                string profitStrWithPower = BTC2Fiat(device.GetCurrentMostProfitValue);
                string currentStr = BTC2Fiat(device.GetPrevMostProfitValueWithoutPower);
                string currentStrWithPower = BTC2Fiat(device.GetPrevMostProfitValue);
                Helpers.ConsolePrint($"BusID {device.Device.BusID}", $"({ device.Device.GetFullName()}) " +
                    $"CURRENT ALGO: {device.GetCurrentProfitableString()} PROFIT: {currentStr} with pwr: {currentStrWithPower}" +
                    $" - MOST PROFITABLE ALGO: {device.GetMostProfitableString()} PROFIT: {profitStr} with pwr: {profitStrWithPower}");
            }
            Form_Main.smaCount = 0;
            if (smaTmp == 0)
            {
                if (Miner.IsRunningNew)
                {
                    Form_Main.smaCount++;
                }
                else
                {
                    Form_Main.smaCount = 0;
                }
                if (Form_Main.smaCount > 3)
                {
                    dynamic jsonData = (File.ReadAllText("configs\\sma.dat"));
                    Helpers.ConsolePrint("SwichMostProfitableGroupUpMethod", "Using previous SMA");
                    JArray smadata = (JArray.Parse(jsonData));
                    NiceHashStats.SetAlgorithmRates(smadata);
                }

                if (Form_Main.smaCount > 5)
                {
                    Helpers.ConsolePrint(Tag, "SMA Error. Restart program");
                    Form_Main.MakeRestart(0);
                    return;
                }
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
            double percDiff = 0.0d;

            bool bFormSettings = false;
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == "Form_Settings")
                {
                    bFormSettings = true;
                    break;
                }
                else
                {

                }
            }
            
            if (ConfigManager.GeneralConfig.By_profitability_of_all_devices)
            {
                if (ConfigManager.GeneralConfig.with_power)
                {
                    Helpers.ConsolePrint("Profitability of all devices with power", $"   ** Current profit {BTC2Fiat(prevStateProfit)}, Profit after switching {BTC2Fiat(currentProfit)}");
                }
                else
                {
                    Helpers.ConsolePrint("Profitability of all devices", $"   ** Current profit {BTC2Fiat(prevStateProfit)}, Profit after switching {BTC2Fiat(currentProfit)}");
                }
                double a = Math.Max(prevStateProfit, currentProfit);
                double b = Math.Min(prevStateProfit, currentProfit);
                percDiff = ((a - b)) / Math.Abs(b);

                if (percDiff <= ConfigManager.GeneralConfig.SwitchProfitabilityThreshold)
                {
                    // don't switch
                    Helpers.ConsolePrint(Tag,
                        $"{"Total rig profit"}: Will NOT SWITCH profit diff is {Math.Round(percDiff * 100, 2):f2}%, current threshold {ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100}%");
                    //CheckForceSwitch(percDiff);
                    // RESTORE OLD PROFITS STATE
                    if (!Divert.KawpowLiteForceStop)
                    {
                        foreach (var device in _miningDevices)
                        {
                            device.RestoreOldProfitsState();
                        }
                    }
                }
                else
                {
                    if (!Divert.KawpowLiteForceStop)
                    {
                        if ((Form_Main.ZilCount == 96 || Form_Main.ZilCount == 97 || Form_Main.ZilCount == 98) && !Form_Main._NeedMiningStart)
                        {
                            Helpers.ConsolePrint(Tag, "Switching disabled because ZIL round is expected");
                            needSwitch = false;
                            // RESTORE OLD PROFITS STATE
                            foreach (var device in _miningDevices)
                            {
                                device.RestoreOldProfitsState();
                            }
                            return;
                        }
                        /*
                        if ((Form_Main.isZilRound || Form_Main.ZilCount == 99 || Form_Main.ZilCount == 0) && !Form_Main._NeedMiningStart)
                        {
                            Helpers.ConsolePrint(Tag, "Switching disabled during ZIL round");
                            needSwitch = false;
                            // RESTORE OLD PROFITS STATE
                            foreach (var device in _miningDevices)
                            {
                                device.RestoreOldProfitsState();
                            }
                            return;
                        }
                        */
                        if ((Form_Main.ZilCount == 1 || Form_Main.ZilCount == 2) && !Form_Main._NeedMiningStart)
                        {
                            Helpers.ConsolePrint(Tag, "Switching disabled after ZIL round");
                            needSwitch = false;
                            // RESTORE OLD PROFITS STATE
                            foreach (var device in _miningDevices)
                            {
                                device.RestoreOldProfitsState();
                            }
                            return;
                        }
                    }

                    if (_ticks[0] + 1 >= AlgorithmSwitchingManager._ticksForStable)
                    {
                        if (prev_percDiff > percDiff + percDiff * 0.2)
                        {
                            if (!Divert.KawpowLiteForceStop)
                            {
                                _ticks[0] = _ticks[0] - 1;
                                needSwitch = false;
                                Helpers.ConsolePrint(Tag,
                                    $"Switching delayed due profit down. Profit diff is {Math.Round(percDiff * 100, 2):f2}%, current threshold {ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100}%");
                                foreach (var device in _miningDevices)
                                {
                                    device.RestoreOldProfitsState();
                                }
                            }
                        }
                        else
                        {
                            _ticks[0] = 0;
                            needSwitch = true;
                            Helpers.ConsolePrint(Tag,
                                $"Will SWITCH profit diff is {Math.Round(percDiff * 100, 2):f2}%, current threshold {ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100}%");
                        }

                        if (bFormSettings)
                        {
                            Helpers.ConsolePrint(Tag,
                                    "Switching delayed due dialog Settings opened");
                            needSwitch = false;
                            foreach (var device in _miningDevices)
                            {
                                device.RestoreOldProfitsState();
                            }
                        }
                    }
                    else
                    {
                        _ticks[0]++;
                        needSwitch = false;
                        Helpers.ConsolePrint(Tag, $"Will NOT SWITCH profit diff is {Math.Round(percDiff * 100, 2):f2}%. Switching period has not been exceeded: " +
                            _ticks[0].ToString() + "/" + AlgorithmSwitchingManager._ticksForStable.ToString() + " min");
                        //CheckForceSwitch(percDiff);
                        // RESTORE OLD PROFITS STATE
                        if (!Divert.KawpowLiteForceStop)
                        {
                            foreach (var device in _miningDevices)
                            {
                                device.RestoreOldProfitsState();
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var device in _miningDevices)
                {
                    currentProfit = device.GetCurrentMostProfitValue;
                    prevStateProfit = device.GetPrevMostProfitValue;

                    Helpers.ConsolePrint(Tag, $"PrevStateProfit {BTC2Fiat(prevStateProfit)}, CurrentProfit {BTC2Fiat(currentProfit)}");
                    var a = Math.Max(prevStateProfit, currentProfit);
                    var b = Math.Min(prevStateProfit, currentProfit);
                    percDiff = ((a - b)) / Math.Abs(b);
                    if (percDiff <= ConfigManager.GeneralConfig.SwitchProfitabilityThreshold)
                    {
                        // don't switch
                        Helpers.ConsolePrint(Tag,
                            $"{device.Device.GetFullName()}: Will NOT SWITCH profit diff is {percDiff * 100:f2}%, current threshold {ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100}%");
                        //CheckForceSwitch(percDiff);
                        // RESTORE OLD PROFITS STATE
                        //foreach (var device in _miningDevices)
                        if (!Divert.KawpowLiteForceStop)
                        {
                            device.RestoreOldProfitsState();
                        }
                    }
                    else
                    {
                        if (!Divert.KawpowLiteForceStop)
                        {
                            if ((Form_Main.ZilCount == 96 || Form_Main.ZilCount == 97 || Form_Main.ZilCount == 98) && !Form_Main._NeedMiningStart)
                            {
                                Helpers.ConsolePrint(Tag, "Switching disabled because ZIL round is expected for " + device.Device.Name);
                                needSwitch = false;
                                device.RestoreOldProfitsState();
                                continue;
                            }
                            /*
                            if ((Form_Main.isZilRound || Form_Main.ZilCount == 99 || Form_Main.ZilCount == 0) && !Form_Main._NeedMiningStart)
                            {
                                Helpers.ConsolePrint(Tag, "Switching disabled during ZIL round for " + device.Device.Name);
                                needSwitch = false;
                                device.RestoreOldProfitsState();
                                continue;
                            }
                            */
                            if ((Form_Main.ZilCount == 1 || Form_Main.ZilCount == 2 || Form_Main.ZilCount == 3) && !Form_Main._NeedMiningStart)
                            {
                                Helpers.ConsolePrint(Tag, "Switching disabled after ZIL round for " + device.Device.Name);
                                needSwitch = false;
                                device.RestoreOldProfitsState();
                                continue;
                            }
                        }
                        if (_ticks[device.Device.Index] + 1 >= AlgorithmSwitchingManager._ticksForStable)
                        {
                            if (prev_percDiff > percDiff + percDiff * 0.2)
                            {
                                if (!Divert.KawpowLiteForceStop)
                                {
                                    _ticks[device.Device.Index] = _ticks[device.Device.Index] - 1;
                                    needSwitch = false;
                                    Helpers.ConsolePrint(Tag,
                                        $"Switching delayed due profit down. Profit diff is {Math.Round(percDiff * 100, 2):f2}%, current threshold {ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100}%");
                                    device.RestoreOldProfitsState();
                                }
                            }
                            else
                            {
                                _ticks[device.Device.Index] = 0;
                                needSwitch = true;
                                Helpers.ConsolePrint(Tag,
                                    $"{device.Device.GetFullName()}: Will SWITCH profit diff is {Math.Round(percDiff * 100, 2)}%, current threshold {ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100}%");
                            }

                            if (bFormSettings)
                            {
                                Helpers.ConsolePrint(Tag,
                                        "Switching delayed due dialog Settings opened");
                                needSwitch = false;
                                device.RestoreOldProfitsState();
                            }
                        }
                        else
                        {
                            _ticks[device.Device.Index]++;
                            needSwitch = false;
                            if (!Divert.KawpowLiteForceStop)
                            {
                                Helpers.ConsolePrint(Tag, $"{device.Device.GetFullName()}: Will NOT SWITCH profit diff is {Math.Round(percDiff * 100, 2):f2}%. Switching period has not been exceeded: " +
                                _ticks[device.Device.Index].ToString() + "/" + AlgorithmSwitchingManager._ticksForStable.ToString() + " min");
                                //CheckForceSwitch(percDiff);
                                // RESTORE OLD PROFITS STATE
                                //foreach (var device2 in _miningDevices)
                                //{
                                //  device2.RestoreOldProfitsState();
                                //}
                                device.RestoreOldProfitsState();
                            }
                        }
                    }
                }
            }
            prev_percDiff = percDiff;
            Form_Main._NeedMiningStart = false;

            if (Divert.KawpowLiteForceStop && Form_Main.KawpowLiteEnabled)
            {
                Helpers.ConsolePrint(Tag, "Force switch from KawpowLite mining");
                Divert.KawpowLiteForceStop = false;
                needSwitch = true;
                //Divert.KawpowLitedivert_running = false;
            }

            if (!needSwitch)
            {
                AlgorithmSwitchingManager.SmaCheckTimerOnElapsedRun = false;
                return;
            }

            //чтоб после переключения не было еще одного переключения
            if (ConfigManager.GeneralConfig.By_profitability_of_all_devices)
            {
                _ticks[0] = 0;
            } else
            {
                foreach (var device in _miningDevices)
                {
                    _ticks[device.Device.Index] = 0;
                }
            }

            NewGrouping(profitableDevices);
        }

        private void NewGrouping(List<MiningPair> profitableDevices)
        {
            Form_Main.SwitchCount++;
            Helpers.ConsolePrint("SWITCHING", "Number of switches: " + Form_Main.SwitchCount.ToString() + " Uptime: " + Form_Main.Uptime.ToString(@"d\ \d\a\y\s\ hh\:mm\:ss"));
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
                        toStop.StartMinerTime = new DateTime(0);
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
                        toStart.StartMinerTime = DateTime.Now;
                        stringBuilderCurrentAlgo.Append($"{toStart.DevicesInfoString}: {toStart.AlgorithmType} : {toStart.DualAlgorithmType}, ");
                        //toStart.Start(_miningLocation, _btcAdress, _worker);
                        if (ConfigManager.GeneralConfig.ServiceLocation == 0)
                        {
                            toStart.Start(_btcAdress, _worker);
                        }
                        else
                        {
                            toStart.Start(_btcAdress, _worker);
                        }
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

        private void GMinersRestart(List<Miner> _checks)
        {
            foreach (Miner m in _checks)
            {
                try
                {
                    if (m.needChildRestart)
                    {
                        Thread.Sleep(6000);
                        Helpers.ConsolePrint(m.MinerTag(), "Restart gminer process after ZIL round");
                        Process proc = Process.GetProcessById(m.ChildProcess());
                        if (proc != new Process()) proc.Kill();
                    }
                }
                catch (ArgumentException)
                {
                    // Process already exited.
                }
            }
            _checks.Clear();
        }

        public async Task MinerStatsCheck()
        {
            Zil _zil = null;
            try
            {
                _zil = JsonConvert.DeserializeObject<Zil>(File.ReadAllText("configs\\zil.json"), Globals.JsonSettings);
                if (_zil == null) throw new ArgumentNullException("_zil = null");
            }
            catch (Exception ex)
            {
                _zil = new Zil
                {
                    RateNoZil = 0.5,
                    RateNoZilCount = 50000,
                    RateZil = 0.02,
                    RateZilCount = 2000,
                    ZilRatio = 0.04,
                    ZilFactor = 0.04
                };
                try
                {
                    Helpers.ConsolePrint("MinerStatsCheck", ex.ToString());
                    Helpers.WriteAllTextWithBackup("configs\\zil.json", JsonConvert.SerializeObject(_zil, Formatting.Indented));
                    _zil = null;
                    _zil = JsonConvert.DeserializeObject<Zil>(File.ReadAllText("configs\\zil.json"), Globals.JsonSettings);
                } catch (Exception ex2)
                {
                    Helpers.ConsolePrint("MinerStatsCheck", ex.ToString());
                }
            }

            var currentProfit = 0.0d;
            _mainFormRatesComunication.ClearRates(_runningGroupMiners.Count);
            var checks = new List<GroupMiner>(_runningGroupMiners.Values);
            var _checks = new List<Miner>();
            try
            {
                foreach (var groupMiners in checks)
                {
                    Miner m = groupMiners.Miner;
                    // skip if not running or if await already in progress
                    // if (!Miner.IsRunning || m.IsUpdatingApi) continue;
                    //m.TicksForApiUpdate++;
                    //if (m.TicksForApiUpdate >= 5) m.TicksForApiUpdate = 0;

                    if (!m.IsRunning || m.IsUpdatingApi || m == null) continue;
                    // continue;

                    m.IsUpdatingApi = true;
                    try
                    {
                        new Task(() => m.GetSummaryAsync()).Start();
                    }
                    catch (NullReferenceException ex)
                    {
                        Helpers.ConsolePrint("MinerStatsCheck", ex.ToString());
                    }

                    var ad = m.GetApiData();
                    //var ad = await m.GetSummaryAsync();
                    m.IsUpdatingApi = false;
                    /*
                    if (ad == null)
                    {
                        Helpers.ConsolePrint(m.MinerTag(), "GetSummary returned null..");
                        Thread.Sleep(1500);
                        m.IsUpdatingApi = true;
                        ad = await m.GetSummaryAsync();
                        m.IsUpdatingApi = false;

                    }
                    */
                    // set rates
                    if (ad != null)
                    {
                        Form_Main.RateNoZil = _zil.RateNoZil;
                        Form_Main.RateZil = _zil.RateZil;
                        if (ad.ZilRound)
                        {
                            if (ad.SecondaryAlgorithmID != AlgorithmType.NONE)//single
                            {
                                NHSmaData.TryGetPaying(AlgorithmType.ZIL, out var secPaying);
                                if (ConfigManager.GeneralConfig.ZIL_mining_state != 1) secPaying = 0;
                                groupMiners.CurrentRate = secPaying * ad.SecondarySpeed * 0.000000001 * 1;
                            }
                            if (ad.ThirdAlgorithmID != AlgorithmType.NONE)//dual
                            {
                                NHSmaData.TryGetPaying(AlgorithmType.ZIL, out var thirdPaying);
                                if (ConfigManager.GeneralConfig.ZIL_mining_state != 1) thirdPaying = 0;
                                groupMiners.CurrentRate = thirdPaying * ad.ThirdSpeed * 0.000000001 * 1;
                            }
                            if (Form_additional_mining.isAlgoZIL(ad.AlgorithmName, groupMiners.MinerBaseType, groupMiners.DeviceType))
                            {
                                Form_Main.RateZil += groupMiners.CurrentRate;
                                _zil.RateZilCount++;
                            }
                        }
                        else
                        {
                            if (ad.AlgorithmID != AlgorithmType.NONE)
                            {
                                NHSmaData.TryGetPaying(ad.AlgorithmID, out var paying);
                                groupMiners.CurrentRate = paying * ad.Speed * 0.000000001;
                                //Helpers.ConsolePrint("paying * ad.Speed * 0.000000001", (paying * ad.Speed * 0.000000001).ToString());
                            }
                            if (ad.SecondaryAlgorithmID != AlgorithmType.NONE)
                            {
                                NHSmaData.TryGetPaying(ad.SecondaryAlgorithmID, out var secPaying);
                                groupMiners.CurrentRate += secPaying * ad.SecondarySpeed * 0.000000001;
                                //Helpers.ConsolePrint("ad.SecondaryAlgorithmID", (ad.SecondaryAlgorithmID).ToString());
                                //Helpers.ConsolePrint("secPaying * ad.SecondarySpeed * 0.000000001", (secPaying * ad.SecondarySpeed * 0.000000001).ToString());
                            }
                            if (ad.ThirdAlgorithmID != AlgorithmType.NONE)
                            {
                                NHSmaData.TryGetPaying(ad.ThirdAlgorithmID, out var thirdPaying);
                                groupMiners.CurrentRate += thirdPaying * ad.ThirdSpeed * 0.000000001;
                                //Helpers.ConsolePrint("thirdPaying * ad.ThirdSpeed * 0.000000001", (thirdPaying * ad.ThirdSpeed * 0.000000001).ToString());
                            }
                            if (Form_additional_mining.isAlgoZIL(ad.AlgorithmName, groupMiners.MinerBaseType, groupMiners.DeviceType))
                            {
                                Form_Main.RateNoZil += groupMiners.CurrentRate;
                                _zil.RateNoZilCount++;

                                if (_zil.RateNoZilCount >= 100000)
                                {
                                    _zil.RateNoZilCount = _zil.RateNoZilCount / 20;
                                    _zil.RateZilCount = _zil.RateZilCount / 20;
                                    _zil.RateNoZil = _zil.RateNoZil / 20;
                                    _zil.RateZil = _zil.RateZil / 20;
                                }
                            }
                        }
                        Form_Main.ZilFactor = _zil.ZilFactor;

                        if (Form_Main.RateZil * 100 < Form_Main.RateNoZil)//fix overprice from api
                        {
                            Form_Main.RateNoZil = Form_Main.RateNoZil / 100;
                        }

                        _zil.RateNoZil = Form_Main.RateNoZil;
                        _zil.RateZil = Form_Main.RateZil;

                        double _RateNoZil = Form_Main.RateNoZil / _zil.RateNoZilCount;//0.0067
                        double _RateZil = Form_Main.RateZil   / _zil.RateZilCount;

                        if (_zil.RateNoZilCount != 0)
                        {
                            _zil.ZilRatio = (double)((double)_zil.RateZilCount / (double)_zil.RateNoZilCount);
                        }
                        double _zilRatio = _zil.ZilRatio * 0.75;

                        Form_Main.ZilFactor = Math.Round((_RateZil * _zilRatio) / _RateNoZil, 3);

                        if (Form_Main.ZilFactor > 0.1)
                        {
                            _RateZil = _RateZil * 0.5;
                            Form_Main.RateZil = Form_Main.RateZil * 0.5;
                            Form_Main.ZilFactor = Form_Main.ZilFactor * 0.5;
                            _zil.RateZil = Form_Main.RateZil;
                        }
                        if (Form_Main.ZilFactor < 0.01)
                        {
                            _RateZil = _RateZil * 2;
                            Form_Main.RateZil = Form_Main.RateZil * 2;
                            Form_Main.ZilFactor = Form_Main.ZilFactor * 2;
                            _zil.RateZil = Form_Main.RateZil;
                        }
                        _zil.ZilFactor = Form_Main.ZilFactor;

                        if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                        if (double.IsNaN(_RateZil)) _RateZil = 0.0d;
                        if (double.IsNaN(_RateNoZil)) _RateZil = 0.0d;
                        if (Form_additional_mining.isAlgoZIL(ad.AlgorithmName, groupMiners.MinerBaseType, groupMiners.DeviceType))
                        {
                            groupMiners.CurrentRate += groupMiners.CurrentRate * Form_Main.ZilFactor;
                        }
                        // Deduct power costs
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
                        if (groupMiners.AlgorithmType != AlgorithmType.KAWPOWLite)
                        {
                            groupMiners.CurrentRate = 0;
                        }
                        ad = new ApiData(groupMiners.DualAlgorithmType);
                    }
                    currentProfit += groupMiners.CurrentRate;
                    // Update GUI
                    _mainFormRatesComunication.AddRateInfo(m.MinerTag(), groupMiners.DevicesInfoString, ad,
                        groupMiners.CurrentRate, groupMiners.PowerRate, groupMiners.StartMinerTime,
                        m.IsApiReadException, m.ProcessTag(), groupMiners, checks.Count);
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("Exception: ", e.ToString());
            }
            if (Form_Main.needGMinerRestart)
            {
                Form_Main.needGMinerRestart = false;
                new Task(() => GMinersRestart(_checks)).Start();
            }

            try
            {
                var s = JsonConvert.SerializeObject(_zil, Formatting.Indented);
                Helpers.WriteAllTextWithBackup("configs\\zil.json", s);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("MinerStatsCheck", ex.ToString());
            }
            GC.Collect();
        }
        private class Zil
        {
            public double RateZil; 
            public int RateZilCount; 
            public double RateNoZil; 
            public int RateNoZilCount; 
            public double ZilRatio; 
            public double ZilFactor; 
        }

    }
}
