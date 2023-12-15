using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMiner.Stats;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;

namespace NiceHashMiner.Algorithms
{
    public class Algorithm
    {
        /// <summary>
        /// Used for converting SMA values to BTC/H/Day
        /// </summary>
        protected const double Mult = 0.000000001;
        public static bool BenchmarkActive = false;

        #region Identity

        /// <summary>
        /// Friendly display name for this algorithm
        /// </summary>
        public string AlgorithmName { get; protected set; }
        /// <summary>
        /// Friendly name for miner type
        /// </summary>
        public readonly string MinerBaseTypeName;
        /// <summary>
        /// Friendly name for this algorithm/miner combo
        /// </summary>
        public string AlgorithmStringID { get; protected set; }
        /// <summary>
        /// AlgorithmType used by this Algorithm
        /// </summary>
        public readonly AlgorithmType NiceHashID;
        /// <summary>
        /// MinerBaseType used by this algorithm
        /// </summary>
        public readonly MinerBaseType MinerBaseType;
        /// <summary>
        /// Used for miner ALGO flag parameter
        /// </summary>
        public readonly string AlgorithmNameCustom;
        public DeviceType DeviceType;

        #endregion

        #region Mining settings
        /// <summary>
        /// Hashrate in H/s set by benchmark or user
        /// </summary>
        public virtual double BenchmarkSpeed { get; set; }
        public virtual double BenchmarkSecondarySpeed { get; set; }
        /// <summary>
        /// Gets the averaged speed for this algorithm in H/s
        /// <para>When multiple devices of the same model are used, this will be set to their averaged hashrate</para>
        /// </summary>
        public double AvaragedSpeed { get; set; }

        /// <summary>
        /// String containing raw extralaunchparams entered by user
        /// </summary>
        public string ExtraLaunchParameters { get; set; }

        /// <summary>
        /// Get or set whether this algorithm is enabled for mining
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Get or set whether this algorithm is hidden in list
        /// </summary>
        public bool Hidden { get; set; }
        public bool Forced { get; set; }

        // TODO not needed with new xmr-stak?
        public int LessThreads { get; set; }

        /// <summary>
        /// Path to the miner executable
        /// <para>Path may differ for the same miner/algo combos depending on devices and user settings</para>
        /// </summary>
        public string MinerBinaryPath = "";
        /// <summary>
        /// Indicates whether this algorithm requires a benchmark
        /// </summary>
        public virtual bool BenchmarkNeeded => BenchmarkSpeed <= 0;

        #endregion

        #region Profitability

        /// <summary>
        /// Current profit for this algorithm in BTC/Day
        /// </summary>
        public double CurrentProfit { get; set; }
        public double CurrentProfitWithoutPower { get; set; }
        /// <summary>
        /// Current SMA profitability for this algorithm type in BTC/GH/Day
        /// </summary>
        public double CurNhmSmaDataVal { get; set; }

        /// <summary>
        /// Power consumption of this algorithm, in Watts
        /// </summary>
        public virtual double PowerUsage { get; set; }
        public virtual double PowerUsageBenchmark { get; set; }
        public virtual int gpu_clock { get; set; }
        public virtual int gpu_clock_def { get; set; }
        public virtual int gpu_clock_min { get; set; }
        public virtual int gpu_clock_max { get; set; }
        public virtual int mem_clock { get; set; }
        public virtual int mem_clock_def { get; set; }
        public virtual int mem_clock_min { get; set; }
        public virtual int mem_clock_max { get; set; }
        public virtual double gpu_voltage { get; set; }
        public virtual double gpu_voltage_def { get; set; }
        public virtual double gpu_voltage_min { get; set; }
        public virtual double gpu_voltage_max { get; set; }
        public virtual double mem_voltage { get; set; }
        public virtual double mem_voltage_def { get; set; }
        public virtual double mem_voltage_min { get; set; }
        public virtual double mem_voltage_max { get; set; }
        public virtual int power_limit { get; set; }
        public virtual int power_limit_def { get; set; }
        public virtual int power_limit_min { get; set; }
        public virtual int power_limit_max { get; set; }
        public virtual int fan { get; set; }
        public virtual int fan_def { get; set; }
        public virtual int fan_min { get; set; }
        public virtual int fan_max { get; set; }
        public virtual int fan_flag { get; set; }
        public virtual int thermal_limit { get; set; }
        public virtual int thermal_limit_def { get; set; }
        public virtual int thermal_limit_min { get; set; }
        public virtual int thermal_limit_max { get; set; }

        #endregion

        #region Dual stubs

        // Useful placeholders for finding/sorting
        public virtual AlgorithmType SecondaryNiceHashID => AlgorithmType.NONE;
        public virtual AlgorithmType DualNiceHashID => NiceHashID;

        #endregion

        public Algorithm(MinerBaseType minerBaseType, AlgorithmType niceHashID, string _AlgorithmNameCustom = "WOW!UnknownAlgo", bool enabled = true, bool hidden = false)
        {
            NiceHashID = niceHashID;
            AlgorithmName = AlgorithmNiceHashNames.GetName(NiceHashID);
            MinerBaseTypeName = Enum.GetName(typeof(MinerBaseType), minerBaseType);
            AlgorithmStringID = MinerBaseTypeName + "_" + AlgorithmName;
            MinerBaseType = minerBaseType;
            AlgorithmNameCustom = _AlgorithmNameCustom;
            ExtraLaunchParameters = "";
            LessThreads = 0;
            Enabled = enabled;
            Hidden = hidden;
            Forced = false;
            BenchmarkStatus = "";
            BenchmarkProgressPercent = 0;
            gpu_clock = 0;
            mem_clock = 0;
            gpu_voltage = 0.0d;
            mem_voltage = 0.0d;
            power_limit = 0;
            fan = 0;
            thermal_limit = 0;
        }
        #region Benchmark info

        public string BenchmarkStatus { get; set; }
        public int BenchmarkProgressPercent { get; set; }

        public bool IsBenchmarkPending { get; private set; }

        public string CurPayingRatio
        {
            get
            {
                var ratio = International.GetText("BenchmarkRatioRateN_A");
                AlgorithmType _NiceHashID = NiceHashID;

                if (NHSmaData.TryGetPaying(_NiceHashID, out var paying))
                {
                    ratio = paying.ToString("F8");
                }
                return ratio;
            }
        }
        public string CurSecondPayingRatio
        {
            get
            {
                var ratio = International.GetText("BenchmarkRatioRateN_A");
                if (NiceHashID == AlgorithmType.Autolykos && SecondaryNiceHashID == AlgorithmType.DaggerHashimoto && NHSmaData.TryGetPaying(SecondaryNiceHashID, out var paying))//ZIL
                {
                    ratio = (paying / 30).ToString("F8");
                }
                else if (NHSmaData.TryGetPaying(SecondaryNiceHashID, out var paying2))
                {
                    ratio = paying2.ToString("F8");
                }
                return ratio;
            }
        }
        public virtual string CurPayingRate
        {
            get
            {
                var rate = "0.00";
                var payingRate = 0.0d;
                AlgorithmType _NiceHashID = NiceHashID;

                if (BenchmarkSpeed > 0 && NHSmaData.TryGetPaying(_NiceHashID, out var paying))
                {
                    payingRate = BenchmarkSpeed * paying * Mult;
                    rate = payingRate.ToString("F8");
                }
                return rate;
            }
            set
            {
                var rate = International.GetText("BenchmarkRatioRateN_A");
                AlgorithmType _NiceHashID = NiceHashID;

                if (BenchmarkSpeed > 0 && NHSmaData.TryGetPaying(_NiceHashID, out var paying))
                {
                    double.TryParse(value, out var valueBench);
                    var payingRate = valueBench * paying * Mult;
                    rate = payingRate.ToString("F8");
                }
            }
        }
        public virtual string CurSecondPayingRate
        {
            get
            {
                var rate = International.GetText("BenchmarkRatioRateN_A");
                var payingRate = 0.0d;

                if ( BenchmarkSecondarySpeed> 0 && NHSmaData.TryGetPaying(SecondaryNiceHashID, out var paying))
                {
                    payingRate = BenchmarkSecondarySpeed * paying * Mult;
                    rate = payingRate.ToString("F8");
                }

                if (DualNiceHashID == AlgorithmType.AutolykosZil && NHSmaData.TryGetPaying(SecondaryNiceHashID, out var secPaying2))
                {
                    payingRate = BenchmarkSecondarySpeed * (secPaying2 / 30) * Mult;
                    rate = payingRate.ToString("F8");
                }
                return rate;
            }
            set
            {
                var rate = International.GetText("BenchmarkRatioRateN_A");
                if (BenchmarkSpeed > 0 && NHSmaData.TryGetPaying(NiceHashID, out var paying))
                {
                    double.TryParse(value, out var valueBench);
                    var payingRate = valueBench * paying * Mult;
                    rate = payingRate.ToString("F8");
                }
            }
        }
        #endregion

        #region Benchmark methods

        public void SetBenchmarkPending()
        {
            IsBenchmarkPending = true;
            BenchmarkStatus = International.GetText("Algorithm_Waiting_Benchmark");
        }

        public void SetBenchmarkPendingNoMsg()
        {
            IsBenchmarkPending = true;
        }

        private bool IsPendingString()
        {
            return BenchmarkStatus.Contains(".")
                   || BenchmarkStatus.Contains("%");

            //return BenchmarkStatus == International.GetText("Algorithm_Waiting_Benchmark");
        }

        public void ClearBenchmarkPending()
        {
            IsBenchmarkPending = false;
            if (IsPendingString())
            {
                BenchmarkStatus = "";
            }
        }

        public virtual void ClearBenchmarkPendingFirst()
        {
            IsBenchmarkPending = false;
            BenchmarkStatus = "";
        }

        public string BenchmarkSpeedString()
        {
            if (!string.IsNullOrEmpty(BenchmarkStatus) && BenchmarkActive)
            {
                return BenchmarkStatus;
            }

            if (BenchmarkSpeed > 0)
            {
                return Helpers.FormatDualSpeedOutput(BenchmarkSpeed, 0, 0,  NiceHashID);
            }
            return International.GetText("BenchmarkSpeedStringNone");
        }
        public string SecondaryBenchmarkSpeedString()
        {
            if (BenchmarkSecondarySpeed > 0)
            {
                return Helpers.FormatDualSpeedOutput(BenchmarkSecondarySpeed, 0, 0, DualNiceHashID);
            }

            if (!string.IsNullOrEmpty(BenchmarkStatus) && BenchmarkActive)
            {
                return BenchmarkStatus;
            }
            return International.GetText("BenchmarkSpeedStringNone");
        }

        #endregion

        #region Profitability methods

        public virtual void UpdateCurProfit(Dictionary<AlgorithmType, double> profits, DeviceType devtype, MinerBaseType mbt)
        {
            profits.TryGetValue(NiceHashID, out var paying);
            profits.TryGetValue(SecondaryNiceHashID, out var payingSecond);
            CurNhmSmaDataVal = paying;
            if (DualNiceHashID == AlgorithmType.AutolykosZil)
            {
                CurrentProfit = (CurNhmSmaDataVal * AvaragedSpeed + (payingSecond * BenchmarkSecondarySpeed) / 30) * Mult;
            }
            else
            {
                CurrentProfit = (CurNhmSmaDataVal * AvaragedSpeed + payingSecond * BenchmarkSecondarySpeed) * Mult;
            }
            if (Form_additional_mining.isAlgoZIL(AlgorithmName, mbt, devtype))
            {
                CurrentProfit += CurrentProfit * Form_Main.ZilFactor;
            }
            CurrentProfitWithoutPower = CurrentProfit;
            if (ConfigManager.GeneralConfig.with_power)
            {
                SubtractPowerFromProfit();
            }
        }

        protected void SubtractPowerFromProfit()
        {
            // This is power usage in BTC/hr
            var power = PowerUsage / 1000 * ExchangeRateApi.GetKwhPriceInBtc();
            // Now it is power usage in BTC/day
            power *= 24 * Form_Main._factorTimeUnit;
            // Now we subtract from profit, which may make profit negative
            CurrentProfit -= power;
        }

        #endregion
    }
}
