using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace NiceHashMiner.Switching
{
    /// <summary>
    /// Maintains global registry of NH SMA
    /// </summary>
    public static class NHSmaData
    {
        private const string Tag = "NHSMAData";
        private const string CachedFile = "configs\\cached_sma.json";

        public static bool Initialized { get; private set; }
        /// <summary>
        /// True iff there has been at least one SMA update
        /// </summary>
        public static bool HasData { get; private set; }

        // private static Dictionary<AlgorithmType, List<double>> _recentPaying;

        // Global list of SMA data, should be accessed with a lock since callbacks/timers update it
        private static Dictionary<AlgorithmType, NiceHashSmaTmp> _currentSma;
        private static Dictionary<AlgorithmType, NiceHashSma> _finalSma;
        // Global list of stable algorithms, should be accessed with a lock
        private static HashSet<AlgorithmType> _stableAlgorithms;

        // Public for tests only
        private static void Initialize()
        {
            if (Initialized) return;
            Helpers.ConsolePrint("NHSMA", "Try initialize SMA");
            _currentSma = null;
            _finalSma = null;
            _stableAlgorithms = null;
            _currentSma = new Dictionary<AlgorithmType, NiceHashSmaTmp>();
            _finalSma = new Dictionary<AlgorithmType, NiceHashSma>();
            _stableAlgorithms = new HashSet<AlgorithmType>();
            /*
            Dictionary<AlgorithmType, double> cacheDict = null;
            try
            {
                var cache = File.ReadAllText(CachedFile);
                cacheDict = JsonConvert.DeserializeObject<Dictionary<AlgorithmType, double>>(cache);
            }
            catch (FileNotFoundException)
            {
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint(Tag, e.ToString());
            }
            */
            // _recentPaying = new Dictionary<AlgorithmType, List<double>>();
            foreach (AlgorithmType algo in Enum.GetValues(typeof(AlgorithmType)))
            {
                if (algo >= 0)
                {
                    var paying = 0d;
                        HasData = true;

                    _currentSma[algo] = new NiceHashSmaTmp
                    {
                        Port = (int)algo + 3333,
                        Name = algo.ToString().ToLower(),
                        Algo = (int)algo,
                        Paying = paying
                    };
                   
                    _finalSma[algo] = new NiceHashSma
                    {
                        Port = (int)algo + 3333,
                        Name = algo.ToString().ToLower(),
                        Algo = (int)algo,
                        Paying = paying
                    };
                    
                }

                if (algo == AlgorithmType.ZIL)
                {
                    var paying = 0d;
                    HasData = true;

                    _currentSma[algo] = new NiceHashSmaTmp
                    {
                        Port = (int)0,
                        Name = algo.ToString().ToLower(),
                        Algo = (int)algo,
                        Paying = paying
                    };

                    _finalSma[algo] = new NiceHashSma
                    {
                        Port = (int)0,
                        Name = algo.ToString().ToLower(),
                        Algo = (int)algo,
                        Paying = paying
                    };

                }

                if (algo == AlgorithmType.KAWPOWLite)
                {
                    var paying = 0d;
                    HasData = true;

                    _currentSma[algo] = new NiceHashSmaTmp
                    {
                        Port = 3385,
                        Name = algo.ToString().ToLower(),
                        Algo = (int)algo,
                        Paying = paying
                    };
                    _finalSma[algo] = new NiceHashSma
                    {
                        Port = 3385,
                        Name = algo.ToString().ToLower(),
                        Algo = (int)algo,
                        Paying = paying
                    };
                }
            }
            Initialized = true;
            FinalizeSma();
        }

        public static void InitializeIfNeeded()
        {
            if (!Initialized) Initialize();
        }

        #region Update Methods

        /// <summary>
        /// Change SMA profits to new values
        /// </summary>
        /// <param name="newSma">Algorithm/profit dictionary with new values</param>
        public static void UpdateSmaPaying(Dictionary<AlgorithmType, double> newSma, bool average = true)
        {
            InitializeIfNeeded();
            CheckInit();

            lock (_currentSma)
            {
                try
                {
                    foreach (var algo in newSma.Keys)
                    {
                        if (!(algo).ToString().Contains("UNUSED"))
                        {
                            //Helpers.ConsolePrint("UpdateSmaPaying", algo.ToString() + ": " +
                              //      "old value:\t" + _currentSma[algo].Paying.ToString() + " new value:\t" + newSma[algo]);
                        }
                        if (_currentSma.ContainsKey(algo))
                        {
                            if (_currentSma[algo].Paying > 0 && newSma[algo] > _currentSma[algo].Paying * 100)
                            {
                                Helpers.ConsolePrint("UpdateSmaPaying", "NH API bug. " + algo.ToString() + ": " +
                                    "old value: " + _currentSma[algo].Paying.ToString() + " new value: " + newSma[algo]);
                                continue;
                            }
                            
                            if (average)
                            {
                                if (_currentSma[algo].Paying > 0 && newSma[algo] > 0)
                                {
                                    _currentSma[algo].Paying = (_currentSma[algo].Paying + newSma[algo]) / 2;
                                }
                                if (_currentSma[algo].Paying <= 0 && newSma[algo] > 0)
                                {
                                    _currentSma[algo].Paying = newSma[algo];
                                }
                                if (_currentSma[algo].Paying > 0 && newSma[algo] <= 0)
                                {
                                    //nothing
                                }
                            }
                            else
                            {
                                _currentSma[algo].Paying = newSma[algo];
                            }
                        }
                        //
                        if (algo == AlgorithmType.KAWPOWLite)
                        {
                            _currentSma[algo].Paying = newSma[algo];
                        }
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint(Tag, e.ToString());
                }

                if (ConfigManager.GeneralConfig.UseSmaCache)
                {
                    // Cache while in lock so file is not accessed on multiple threads
                    /*
                    try
                    {
                        var cache = JsonConvert.SerializeObject(newSma);
                        File.WriteAllText(CachedFile, cache);
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint(Tag, e.ToString());
                    }
                    */
                }
            }

            HasData = true;
        }

        /// <summary>
        /// Change SMA profit for one algo
        /// </summary>
        public static void UpdatePayingForAlgo(AlgorithmType algo, double paying, bool average = false)
        {
            InitializeIfNeeded();
            CheckInit();
            if (double.IsNaN(paying)) return;
            lock (_currentSma)
            {
                if (!_currentSma.ContainsKey(algo))
                    throw new ArgumentException("Algo not setup in SMA");

                if (!(algo).ToString().Contains("UNUSED"))
                {
                    //Helpers.ConsolePrint("UpdateSmaPaying", algo.ToString() + ": " +
                      //      "old value:\t" + _currentSma[algo].Paying.ToString() + " new value:\t" + paying);
                }

                if (paying != 0)
                {
                    if (_currentSma[algo].Paying > 0 && paying > _currentSma[algo].Paying * 100)
                    {
                        Helpers.ConsolePrint("UpdatePayingForAlgo", "NH API bug. " + algo.ToString() + ": " +
                            "old value: " + _currentSma[algo].Paying.ToString() + " new value: " + paying);
                        return;
                    }

                    if (average)
                    {
                        _currentSma[algo].Paying = (paying + _currentSma[algo].Paying) / 2;
                    }
                    else
                    {
                        _currentSma[algo].Paying = paying;
                    }
                }
                if (algo == AlgorithmType.KAWPOWLite)
                {
                    _currentSma[algo].Paying = paying;
                }
            }
            HasData = true;
        }

        /// <summary>
        /// Update list of stable algorithms
        /// </summary>
        /// <param name="algorithms">Algorithms that are stable</param>
        public static void UpdateStableAlgorithms(IEnumerable<AlgorithmType> algorithms)
        {
            CheckInit();
            var sb = new StringBuilder();
            sb.AppendLine("Updating stable algorithms");
            var hasChange = false;

            lock (_stableAlgorithms)
            {
                var algosEnumd = algorithms as AlgorithmType[] ?? algorithms.ToArray();
                foreach (var algo in algosEnumd)
                {
                    if (_stableAlgorithms.Add(algo) && algo != AlgorithmType.NeoScrypt)
                    {
                        sb.AppendLine($"\tADDED {algo}");
                        hasChange = true;
                    }
                }

                _stableAlgorithms.RemoveWhere(algo =>
                {
                    if (algosEnumd.Contains(algo)) return false;

                    sb.AppendLine($"\tREMOVED {algo}");
                    hasChange = true;
                    return true;
                });
            }
            if (!hasChange)
            {
                sb.AppendLine("\tNone changed");
            }
            Helpers.ConsolePrint(Tag, sb.ToString());
        }

        #endregion

        # region Get Methods

        /// <summary>
        /// Attempt to get SMA for an algorithm
        /// </summary>
        /// <param name="algo">Algorithm</param>
        /// <param name="sma">Variable to place SMA in</param>
        /// <returns>True iff we know about this algo</returns>
        public static bool TryGetSma(AlgorithmType algo, out NiceHashSma sma)
        {
            InitializeIfNeeded();
            CheckInit();
            lock (_finalSma)
            {
                if (_finalSma.ContainsKey(algo))
                {
                    sma = _finalSma[algo];
                    return true;
                }
            }

            sma = null;
            return false;
        }

        public static void FinalizeSma()
        {
            Random r = new Random();
            int r1 = r.Next(5, 15);
            Thread.Sleep(100 * r1);

            Helpers.ConsolePrint("NHSMA", "FinalizeSma");
            InitializeIfNeeded();
            CheckInit();
            _finalSma.Clear();

            lock (_finalSma)
            {
                try
                {
                    foreach (var final_sma in _currentSma)
                    {
                        NiceHashSma v = new NiceHashSma();
                        v.Algo = final_sma.Value.Algo;
                        v.Name = final_sma.Value.Name;
                        v.Paying = final_sma.Value.Paying;
                        v.Port = final_sma.Value.Port;
                        if (!((AlgorithmType)v.Algo).ToString().Contains("UNUSED"))
                        {
                            //Helpers.ConsolePrint("FinalizeSma", ((AlgorithmType)v.Algo).ToString() + ":\t" + v.Paying.ToString());
                        }
                        _finalSma.Add(final_sma.Key, v);
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint(Tag, ex.ToString());
                }
            }
            
            /*
            try
            {
                var cache = JsonConvert.SerializeObject(_finalSma);
                File.WriteAllText(CachedFile, cache);
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint(Tag, e.ToString());
            }
            */
        }

        /// <summary>
        /// Attempt to get paying rate for an algorithm
        /// </summary>
        /// <param name="algo">Algorithm</param>
        /// <param name="sma">Variable to place paying in</param>
        /// <returns>True iff we know about this algo</returns>
        public static bool TryGetPaying(AlgorithmType algo, out double paying)
        {
            InitializeIfNeeded();
            CheckInit();

            if (TryGetSma(algo, out NiceHashSma sma))
            {
                paying = sma.Paying;
                if (algo == AlgorithmType.KAWPOWLite && !Divert.KawpowLiteGoodEpoch)
                {
                    paying = 0.0d;
                }
                return true;
            }

            paying = default(double);
            return false;
        }

        #endregion

        #region Get Methods

        public static bool IsAlgorithmStable(AlgorithmType algo)
        {
            CheckInit();
            lock (_stableAlgorithms)
            {
                return _stableAlgorithms.Contains(algo);
            }
        }

        /// <summary>
        /// Filters SMA profits based on whether the algorithm is stable
        /// </summary>
        /// <param name="stable">True to get stable, false to get unstable</param>
        /// <returns>Filtered Algorithm/double map</returns>
        public static Dictionary<AlgorithmType, double> FilteredCurrentProfits()
        {
            CheckInit();
            var dict = new Dictionary<AlgorithmType, double>();
            lock (_finalSma)
            {
                foreach (var kvp in _finalSma)
                {
                    //if (_stableAlgorithms.Contains(kvp.Key) == Enabled)
                    //{
                    dict[kvp.Key] = kvp.Value.Paying;
                    //}
                }
            }

            return dict;
        }

        #endregion

        /// <summary>
        /// Helper to ensure initialization
        /// </summary>
        private static void CheckInit()
        {
            if (!Initialized)
                throw new InvalidOperationException("NHSmaData cannot be used before initialization");
        }
    }
}
