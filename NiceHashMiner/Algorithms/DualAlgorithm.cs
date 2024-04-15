using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace NiceHashMiner.Algorithms
{
    public class DualAlgorithm : Algorithm
    {
        #region Identity

        /// <summary>
        /// AlgorithmType used as the secondary for this algorithm
        /// </summary>
        public override AlgorithmType SecondaryNiceHashID { get; }
        /// <summary>
        /// Friendly name for secondary algorithm
        /// </summary>
        public readonly string SecondaryAlgorithmName;
        /// <summary>
        /// Current SMA profitability for the secondary algorithm type in BTC/GH/Day
        /// </summary>
        public double SecondaryCurNhmSmaDataVal = 0;
        /// <summary>
        /// AlgorithmType that uniquely identifies this choice of primary/secondary types
        /// </summary>
        //public override AlgorithmType DualNiceHashID => Helpers.DualAlgoFromAlgos(NiceHashID, SecondaryNiceHashID);
        public override AlgorithmType DualNiceHashID
        {
            get
            {
                if (NiceHashID == AlgorithmType.DaggerHashimoto)
                {
                    switch (SecondaryNiceHashID)
                    {
                        case AlgorithmType.Alephium:
                            return AlgorithmType.DaggerAlephium;
                        case AlgorithmType.KarlsenHash:
                            return AlgorithmType.DaggerKarlsenHash;
                    }
                }
                if (NiceHashID == AlgorithmType.FishHash)
                {
                    switch (SecondaryNiceHashID)
                    {
                        case AlgorithmType.Alephium:
                            return AlgorithmType.FishHashAlephium;
                        case AlgorithmType.KarlsenHash:
                            return AlgorithmType.FishHashKarlsenHash;
                        case AlgorithmType.PyrinHash:
                            return AlgorithmType.FishHashPyrinHash;
                    }
                }
                if (NiceHashID == AlgorithmType.Autolykos)
                {
                    switch (SecondaryNiceHashID)
                    {
                        case AlgorithmType.DaggerHashimoto:
                            return AlgorithmType.AutolykosZil;
                        case AlgorithmType.KarlsenHash:
                            return AlgorithmType.AutolykosKarlsenHash;
                        case AlgorithmType.Alephium:
                            return AlgorithmType.AutolykosAlephium;
                        case AlgorithmType.PyrinHash:
                            return AlgorithmType.AutolykosPyrinHash;
                    }
                }
                if (NiceHashID == AlgorithmType.Octopus)
                {
                    switch (SecondaryNiceHashID)
                    {
                        case AlgorithmType.KarlsenHash:
                            return AlgorithmType.OctopusKarlsenHash;
                        case AlgorithmType.Alephium:
                            return AlgorithmType.OctopusAlephium;
                        case AlgorithmType.PyrinHash:
                            return AlgorithmType.OctopusPyrinHash;
                    }
                }
                if (NiceHashID == AlgorithmType.ETCHash)
                {
                    switch (SecondaryNiceHashID)
                    {
                        case AlgorithmType.KarlsenHash:
                            return AlgorithmType.ETCHashKarlsenHash;
                        case AlgorithmType.Alephium:
                            return AlgorithmType.ETCHashAlephium;
                    }
                }

                return NiceHashID;
            }
        }



        #endregion

        #region Intensity tuning

        /// <summary>
        /// Current intensity while mining or benchmarking
        /// </summary>
        public int CurrentIntensity = -1;

        /// <summary>
        /// Lower bound for intensity tuning
        /// </summary>
        public int TuningStart = 5;
        /// <summary>
        /// Upper bound for intensity tuning
        /// </summary>
        public int TuningEnd = 180;
        /// <summary>
        /// Interval for intensity tuning
        /// </summary>
        public int TuningInterval = 25;

        /// <summary>
        /// Dictionary of intensity values to speeds in hashrates
        /// </summary>
        public Dictionary<int, double> IntensitySpeeds;
        /// <summary>
        /// Dictionary of intensity values to secondary speeds in hashrates
        /// </summary>
        public Dictionary<int, double> SecondaryIntensitySpeeds;
        /// <summary>
        /// Get or set whether intensity tuning is enabled
        /// </summary>
        public bool TuningEnabled;

        // And backups
        private Dictionary<int, double> _intensitySpeedsBack;
        private Dictionary<int, double> _secondaryIntensitySpeedsBack;
        private bool _tuningEnabledBack;
        private int _tuningStartBack;
        private int _tuningEndBack;
        private int _tuningIntervalBack;

        /// <summary>
        /// Get or set whether intensity profitability is up to date
        /// <para>This should generally be set to false if tuning speeds or SMA profits are changed</para>
        /// </summary>
        public bool IntensityUpToDate;

        private int _mostProfitableIntensity = -1;
        /// <summary>
        /// Get the most profitable intensity value for this algorithm
        /// <para>If IntensityUpToDate = false, intensity profit will be updated first</para>
        /// </summary>
        public int MostProfitableIntensity
        {
            get
            {
                // UpdateProfitableIntensity() can take some time, so we store most profitable and only update when needed
                if (!IntensityUpToDate) UpdateProfitableIntensity();
                return _mostProfitableIntensity;
            }
        }

        /// <summary>
        /// Sorted list of intensities that are selected for tuning
        /// </summary>
        private SortedSet<int> SelectedIntensities
        {
            get
            {
                var list = new SortedSet<int>();
                for (var i = TuningStart;
                    i <= TuningEnd;
                    i += TuningInterval)
                {
                    list.Add(i);
                }

                return list;
            }
        }

        /// <summary>
        /// Sorted list of all intensities that are selected for tuning or have speeds
        /// </summary>
        public SortedSet<int> AllIntensities
        {
            get
            {
                var list = new List<int>(IntensitySpeeds.Keys);
                list.AddRange(SecondaryIntensitySpeeds.Keys);
                list.AddRange(SelectedIntensities);
                return new SortedSet<int>(list);
            }
        }

        #endregion
        public readonly string DualAlgorithmNameCustom;
        #region Mining settings

        private double _secondaryBenchmarkSpeed;

        /// <summary>
        /// Gets the secondary averaged speed for this algorithm in H/s
        /// <para>When multiple devices of the same model are used, this will be set to their averaged hashrate</para>
        /// </summary>
        public double SecondaryAveragedSpeed { get; set; }

        #region Power Switching

        /// <summary>
        /// Dictionary of intensity values and power usage for each
        /// </summary>
        public Dictionary<int, double> IntensityPowers;
        /// <summary>
        /// Get or set whether we should use different powers for intensities
        /// </summary>
        public bool UseIntensityPowers;
        // Backup of above
        private Dictionary<int, double> _intensityPowersBack;
        private bool _useIntensityPowersBack;

        public override double PowerUsage
        {
            get
            {
                if (UseIntensityPowers &&
                    MostProfitableIntensity > 0 &&
                    IntensityPowers.TryGetValue(MostProfitableIntensity, out var power))
                {
                    return power;
                }

                return base.PowerUsage;
            }
            set => base.PowerUsage = value;
        }

        #endregion


        public DualAlgorithm(MinerBaseType minerBaseType, AlgorithmType niceHashID, AlgorithmType secondaryNiceHashID, string _DualAlgorithmNameCustom = "WOW!UnknownDualAlgo")
            : base(minerBaseType, niceHashID, "")
        {
            SecondaryNiceHashID = secondaryNiceHashID;

            AlgorithmName = AlgorithmNiceHashNames.GetName(DualNiceHashID); // needed to add secondary
            SecondaryAlgorithmName = AlgorithmNiceHashNames.GetName(secondaryNiceHashID);
            AlgorithmStringID = MinerBaseTypeName + "_" + AlgorithmName;
            DualAlgorithmNameCustom = _DualAlgorithmNameCustom;
            //SecondaryBenchmarkSpeed = 0.0d;

            IntensitySpeeds = new Dictionary<int, double>();
            SecondaryIntensitySpeeds = new Dictionary<int, double>();
            IntensityPowers = new Dictionary<int, double>();
        }

        private void UpdateProfitableIntensity()
        {
            if (!NHSmaData.HasData)
            {
                _mostProfitableIntensity = -1;
                IntensityUpToDate = true;
                return;
            }

            var maxProfit = 0d;
            var intensity = -1;
            // Max sure to use single | here so second expression evaluates
            if (NHSmaData.TryGetPaying(NiceHashID, out var paying) |
                NHSmaData.TryGetPaying(SecondaryNiceHashID, out var secPaying))
            {
                foreach (var key in IntensitySpeeds.Keys)
                {
                    var profit = IntensitySpeeds[key] * paying;
                    if (SecondaryIntensitySpeeds.TryGetValue(key, out var speed))
                    {
                        profit += speed * secPaying;
                    }

                    if (profit > maxProfit)
                    {
                        maxProfit = profit;
                        intensity = key;
                    }
                }
            }

            _mostProfitableIntensity = intensity;
            IntensityUpToDate = true;
        }

        private bool IsIntensityEmpty(int i)
        {
            if (!IntensitySpeeds.ContainsKey(i) || !SecondaryIntensitySpeeds.ContainsKey(i)) return true;
            return IntensitySpeeds[i] <= 0 || SecondaryIntensitySpeeds[i] <= 0;
        }

        public bool IncrementToNextEmptyIntensity()
        {
            // Return false if no more needed increment
            if (!TuningEnabled) return false;
            CurrentIntensity = SelectedIntensities.FirstOrDefault(IsIntensityEmpty);
            return CurrentIntensity > 0;
        }

        public bool StartTuning()
        {
            // Return false if no benchmark needed
            CurrentIntensity = TuningStart;
            return IncrementToNextEmptyIntensity();
        }

        public override void ClearBenchmarkPendingFirst()
        {
            base.ClearBenchmarkPendingFirst();
            CurrentIntensity = -1;
        }

        

        public double SpeedForIntensity(int intensity)
        {
            IntensitySpeeds.TryGetValue(intensity, out var speed);
            return speed;
        }

        public double SecondarySpeedForIntensity(int intensity)
        {
            SecondaryIntensitySpeeds.TryGetValue(intensity, out var speed);
            return speed;
        }

    }
}
#endregion
