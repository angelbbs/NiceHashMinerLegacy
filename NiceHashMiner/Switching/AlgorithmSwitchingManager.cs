/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using NiceHashMinerLegacy.Common.Enums;
using System.Threading.Tasks;
using NiceHashMiner.Miners;

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

        public static Timer _smaCheckTimer;
        private static readonly Random _random = new Random();//?

        private static int _ticksForStable;
        private static int _ticksForUnstable;
        private static double _smaCheckTime;

        // Simplify accessing config objects
        public static Interval StableRange => ConfigManager.GeneralConfig.SwitchSmaTicksStable;
        public static Interval UnstableRange => ConfigManager.GeneralConfig.SwitchSmaTicksUnstable;
        public static Interval SmaCheckRange => ConfigManager.GeneralConfig.SwitchSmaTimeChangeSeconds;

        public static int MaxHistory => Math.Max(StableRange.Upper, UnstableRange.Upper);

        private static readonly Dictionary<AlgorithmType, AlgorithmHistory> _stableHistory = new Dictionary<AlgorithmType, AlgorithmHistory>();
        private static readonly Dictionary<AlgorithmType, AlgorithmHistory> _unstableHistory = new Dictionary<AlgorithmType, AlgorithmHistory>();

        private static bool _hasStarted;

        /// <summary>
        /// Currently used normalized profits
        /// </summary>
        private static readonly Dictionary<AlgorithmType, double> _lastLegitPaying = new Dictionary<AlgorithmType, double>();

        public AlgorithmSwitchingManager()
        {
            foreach (var kvp in NHSmaData.FilteredCurrentProfits(true))
            {
                _stableHistory[kvp.Key] = new AlgorithmHistory(MaxHistory);
                _lastLegitPaying[kvp.Key] = kvp.Value;
            }
            foreach (var kvp in NHSmaData.FilteredCurrentProfits(false))
            {
                _unstableHistory[kvp.Key] = new AlgorithmHistory(MaxHistory);
                _lastLegitPaying[kvp.Key] = kvp.Value;
            }
        }

        public void Start()
        {
            _smaCheckTimer = new Timer(100);
            _smaCheckTimer.Elapsed += SmaCheckTimerOnElapsed;
            _smaCheckTimer.Start();
            //if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto3GB)
            {
                if (Form_Main.DaggerHashimoto3GB)
                {
                    new Task(() => DHClient.StartConnection()).Start();
                }
            }
        }
        public static void SmaCheckNow()
        {
            SmaCheckTimerOnElapsed(null, null);
        }

        public void Stop()
        {
            //if (MiningSetup.CurrentAlgorithmType == AlgorithmType.DaggerHashimoto3GB)
            {
                if (Form_Main.DaggerHashimoto3GB)
                {
                    DHClient.StopConnection();
                }
            }
            _smaCheckTimer.Stop();
            _smaCheckTimer = null;
        }

        /// <summary>
        /// Checks profits and updates normalization based on ticks
        /// </summary>
        internal static void SmaCheckTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Randomize();

            // Will be null if manually called (in tests)
            if (_smaCheckTimer != null)
                _smaCheckTimer.Interval = _smaCheckTime * 1000;

            var sb = new StringBuilder();

            if (_hasStarted)
            {
                sb.AppendLine("Normalizing profits");
            }

            var stableUpdated = UpdateProfits(_stableHistory, _ticksForStable, sb);
            var unstableUpdated = UpdateProfits(_unstableHistory, _ticksForUnstable, sb);

            if (!stableUpdated && !unstableUpdated && _hasStarted)
            {
                sb.AppendLine("No algos affected (either no SMA update or no algos higher");
                NHSmaData.Initialize();
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
            if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 5) cTicks = "ticks";


            foreach (var algo in history.Keys)
            {
                NHSmaData.TryGetPaying(algo, out var paying);
                if (!algo.ToString().Contains("UNUSED"))
                {
                    history[algo].Add(paying);
                    if (paying > _lastLegitPaying[algo])
                    {
                        updated = true;
                        var i = history[algo].CountOverProfit(_lastLegitPaying[algo]);
                        if (i >= ticks  || algo == AlgorithmType.DaggerHashimoto3GB)
                        {
                            _lastLegitPaying[algo] = paying;
                            sb.AppendLine($"\tTAKEN: new profit {paying:e5} after {i} {cTicks} for {algo}");
                        }
                        else
                        {
                            sb.AppendLine(
                                $"\tPOSTPONED: new profit {paying:e5} (previously {_lastLegitPaying[algo]:e5})," +
                                $" higher for {i}/{ticks} {cTicks} for {algo}"
                            );
                        }
                    }
                    else
                    {
                        // Profit has gone down
                        _lastLegitPaying[algo] = paying;
                    }
                }
            }

            return updated;
        }

        private static void Randomize()
        {
            // Lock in case this gets called simultaneously
            // Random breaks down when called from multiple threads
            lock (_random)
            {
                _ticksForStable = StableRange.RandomInt(_random);
                _ticksForUnstable = UnstableRange.RandomInt(_random);
                _smaCheckTime = SmaCheckRange.RandomInt(_random);
                /*
                _ticksForStable = 5;
                _ticksForUnstable = 5;
                _smaCheckTime = 5;
                */
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 0)
                {
                    _smaCheckTime = 60;
                    _ticksForStable = 1;
                    _ticksForUnstable = 1;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 1)
                {
                    _smaCheckTime = 60;
                    _ticksForStable = 3;
                    _ticksForUnstable = 3;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 2)
                {
                    _smaCheckTime = 60;
                    _ticksForStable = 5;
                    _ticksForUnstable = 5;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 3)
                {
                    _smaCheckTime = 60;
                    _ticksForStable = 10;
                    _ticksForUnstable = 10;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 4)
                {
                        _smaCheckTime = 60;
                        _ticksForStable = 15;
                        _ticksForUnstable = 15;
                }
                if (ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex == 5)
                {
                    _ticksForStable = StableRange.RandomInt(_random);
                    _ticksForUnstable = UnstableRange.RandomInt(_random);
                    _smaCheckTime = SmaCheckRange.RandomInt(_random);
                }
            }
        }

        #region Test methods

        internal double LastPayingForAlgo(AlgorithmType algo)
        {
            return _lastLegitPaying[algo];
        }

        #endregion
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
