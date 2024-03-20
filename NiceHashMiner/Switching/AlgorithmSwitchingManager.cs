using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace NiceHashMiner.Switching
{
    /// <summary>
    /// Handles profit switching within a mining session
    /// </summary>
    public class AlgorithmSwitchingManager
    {
        private const string Tag = "SwitchingManager";

        /// <summary>
        /// Emitted when the profits are checked
        /// </summary>
        public static event EventHandler<SmaUpdateEventArgs> SmaCheck;

        public static System.Timers.Timer _smaCheckTimer;
        private static readonly Random _random = new Random();//?

        public static int _ticksForStable;
        private static int _ticksForUnstable;
        private static double _smaCheckTime = 180;
        public static bool SmaCheckTimerOnElapsedRun = false;
        // Simplify accessing config objects
        public static Interval StableRange => ConfigManager.GeneralConfig.SwitchSmaTicksStable;
        public static Interval UnstableRange => ConfigManager.GeneralConfig.SwitchSmaTicksUnstable;
        public static Interval SmaCheckRange => ConfigManager.GeneralConfig.SwitchSmaTimeChangeSeconds;

        public static int MaxHistory => Math.Max(StableRange.Upper, UnstableRange.Upper);

        private static readonly Dictionary<AlgorithmType, AlgorithmHistory> _algosHistory = new Dictionary<AlgorithmType, AlgorithmHistory>();

        private static bool _hasStarted;

        /// <summary>
        /// Currently used normalized profits
        /// </summary>
        private static readonly Dictionary<AlgorithmType, double> _lastLegitPaying = new Dictionary<AlgorithmType, double>();

        public AlgorithmSwitchingManager()
        {
            var miningDevices = ComputeDeviceManager.Available.Devices;
            foreach (var device in miningDevices)
            {
                if (device != null && device.Enabled)
                {
                    var devicesAlgos = device.GetAlgorithmSettings();
                    foreach (var a in devicesAlgos)
                    {
                        if (a.Enabled)
                        {
                            foreach (var kvp in NHSmaData.FilteredCurrentProfits())
                            {
                                if (a.NiceHashID == kvp.Key || a.DualNiceHashID == kvp.Key || a.SecondaryNiceHashID == kvp.Key)
                                {
                                    _algosHistory[kvp.Key] = new AlgorithmHistory(MaxHistory);
                                    _lastLegitPaying[kvp.Key] = kvp.Value;
                                }
                            }
                        }
                    }
                }
            }


            /*
            foreach (var kvp in NHSmaData.FilteredCurrentProfits(false))
            {
                _unstableHistory[kvp.Key] = new AlgorithmHistory(MaxHistory);
                _lastLegitPaying[kvp.Key] = kvp.Value;
            }
            */
        }

        public static void Start()
        {

            if (_smaCheckTimer == null)
            {
                Helpers.ConsolePrint("AlgorithmSwitchingManager", "Start");
                //_smaCheckTimer = new System.Timers.Timer(1000);
                _smaCheckTimer = new System.Timers.Timer();
                _smaCheckTimer.Interval = 60 * 1000;
                _smaCheckTimer.Elapsed += SmaCheckTimerOnElapsed;
                _smaCheckTimer.Start();
            }
            SmaCheckNow();
        }
        public static void SmaCheckNow()
        {
            SmaCheckTimerOnElapsed(null, null);
        }

        public static void Stop()
        {
            Helpers.ConsolePrint("AlgorithmSwitchingManager", "Stop");

            if (_smaCheckTimer != null)
            {
                MiningSession.StopEvent();
                _smaCheckTimer?.Stop();
                _smaCheckTimer.Close();
                _smaCheckTimer.Dispose();
                _smaCheckTimer = null;

                try
                {
                    if (SmaCheck != null)
                    {
                        foreach (Delegate d in SmaCheck.GetInvocationList())
                        {
                            SmaCheck -= (EventHandler<SmaUpdateEventArgs>)d;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("AlgorithmSwitchingManager", ex.ToString());
                }

            }

        }

        /// <summary>
        /// Checks profits and updates normalization based on ticks
        /// </summary>
        public static void SmaCheckTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            SmaCheckTimerOnElapsedRun = true;

            GetTickPeriod();
            var sb = new StringBuilder();
            if (_hasStarted)
            {
                sb.AppendLine("Normalizing profits");
            }
            var stableUpdated = UpdateProfits(_algosHistory, _ticksForStable, sb);
            if (!stableUpdated && _hasStarted)
            {
                sb.AppendLine("No algos affected (either no SMA update or no algos higher");
            }

            if (_hasStarted)
            {
                Helpers.ConsolePrint(Tag, sb.ToString());
            }
            else
            {
                _hasStarted = true;
            }
            var args = new SmaUpdateEventArgs(_lastLegitPaying);
            SmaCheck?.Invoke(sender, args);
        }

        /// <summary>
        /// Check profits for a history dict and update if profit has been higher for required ticks or if it is lower
        /// </summary>
        /// <returns>True iff any profits were postponed or updated</returns>
        private static bool UpdateProfits(Dictionary<AlgorithmType, AlgorithmHistory> history, int ticks, StringBuilder sb)
        {
            var updated = false;
            var cTicks = "min";

            foreach (var algo in history.Keys)
            {
                NHSmaData.TryGetPaying(algo, out var paying);

                if (algo == AlgorithmType.KAWPOWLite && !Divert.KawpowLiteGoodEpoch)
                {
                    paying = 0;
                }

                if (!algo.ToString().Contains("UNUSED"))
                {
                    history[algo].Add(paying);
                    var i = history[algo].CountOverProfit(_lastLegitPaying[algo]);
                    double p1 = 100 - (_lastLegitPaying[algo] / paying) * 100;
                    
                    if (paying > _lastLegitPaying[algo])
                    {
                        updated = true;
                        
                        if (i >= ticks)
                        {
                            _lastLegitPaying[algo] = paying;
                            sb.AppendLine($"\tTAKEN: new profit {paying:e5} {p1:f2}% after {i}/{ticks} {cTicks} for {algo}");
                        }
                        else
                        {
                            sb.AppendLine(
                                $"\tPOSTPONED: new profit {paying:e5} (previously {_lastLegitPaying[algo]:e5}) {p1:f2}%," +
                                $" higher for {i}/{ticks} {cTicks} for {algo}"
                            );
                        }
                    }
                    else
                    {
                        // Profit has gone down
                        updated = true;
                        
                        sb.AppendLine($"\tProfit has gone down: new profit {paying:e5} (previously {_lastLegitPaying[algo]:e5}) {p1:f2}%," +
                            $" less for {i}/{ticks} {cTicks} for {algo}");
                        _lastLegitPaying[algo] = paying;
                    }
                }
            }

            return updated;
        }

        private static void GetTickPeriod()
        {
            // Lock in case this gets called simultaneously
            // Random breaks down when called from multiple threads
            lock (_random)
            {
                _ticksForStable = StableRange.RandomInt(_random);
                _ticksForUnstable = UnstableRange.RandomInt(_random);
                //_smaCheckTime = SmaCheckRange.RandomInt(_random);
                /*
                _ticksForStable = 5;
                _ticksForUnstable = 5;
                _smaCheckTime = 5;
                */
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 0)
                {
                    //_smaCheckTime = 60;
                    _ticksForStable = 1;
                    _ticksForUnstable = 1;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 1)
                {
                    //_smaCheckTime = 60;
                    _ticksForStable = 3;
                    _ticksForUnstable = 3;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 2)
                {
                    //_smaCheckTime = 60;
                    _ticksForStable = 5;
                    _ticksForUnstable = 5;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 3)
                {
                    //_smaCheckTime = 60;
                    _ticksForStable = 10;
                    _ticksForUnstable = 10;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 4)
                {
                    //_smaCheckTime = 60;
                    _ticksForStable = 15;
                    _ticksForUnstable = 15;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 5)
                {
                    //_smaCheckTime = 60;
                    _ticksForStable = 30;
                    _ticksForUnstable = 30;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 6)
                {
                    //_smaCheckTime = 60;
                    _ticksForStable = 60;
                    _ticksForUnstable = 60;
                }
                if (ConfigManager.GeneralConfig.ShortTerm)
                {
                    _ticksForStable = 60;
                    _ticksForUnstable = 60;
                }
            }
        }


    }

    /// <inheritdoc />
    /// <summary>
    /// Event args used for reporting fresh normalized profits
    /// </summary>
    public class SmaUpdateEventArgs : EventArgs
    {
        public readonly Dictionary<AlgorithmType, double> NormalizedProfits;

        public SmaUpdateEventArgs(Dictionary<AlgorithmType, double> profits)
        {
            NormalizedProfits = profits;
        }
    }
}
