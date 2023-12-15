using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;

namespace NiceHashMiner.Miners.Grouping
{
    public class MiningDevice
    {
        public MiningDevice(ComputeDevice device)
        {
            Device = device;
            foreach (var algo in Device.GetAlgorithmSettings())
            {
                var isAlgoMiningCapable = GroupSetupUtils.IsAlgoMiningCapable(algo);
                var isValidMinerPath = MinerPaths.IsValidMinerPath(algo.MinerBinaryPath);
                if (isAlgoMiningCapable && isValidMinerPath)
                {
                    Algorithms.Add(algo);
                }
            }

            MostProfitableAlgorithmType = AlgorithmType.NONE;
            MostProfitableMinerBaseType = MinerBaseType.NONE;
        }

        public ComputeDevice Device { get; }
        public List<Algorithm> Algorithms = new List<Algorithm>();

        public string GetMostProfitableString()
        {
            return
                Enum.GetName(typeof(MinerBaseType), MostProfitableMinerBaseType)
                + "_"
                + Enum.GetName(typeof(AlgorithmType), MostProfitableAlgorithmType);
        }

        public string GetCurrentProfitableString()
        {
            return
                Enum.GetName(typeof(MinerBaseType), PrevProfitableMinerBaseType)
                + "_"
                + Enum.GetName(typeof(AlgorithmType), PrevProfitableAlgorithmType);
        }

        public AlgorithmType MostProfitableAlgorithmType { get; private set; }

        public MinerBaseType MostProfitableMinerBaseType { get; private set; }

        // prev state
        public AlgorithmType PrevProfitableAlgorithmType { get; private set; }

        public MinerBaseType PrevProfitableMinerBaseType { get; private set; }

        private int GetMostProfitableIndex()
        {
            return Algorithms.FindIndex((a) =>
                a.DualNiceHashID == MostProfitableAlgorithmType && a.MinerBaseType == MostProfitableMinerBaseType);
        }

        private int GetPrevProfitableIndex()
        {
            return Algorithms.FindIndex((a) =>
                a.DualNiceHashID == PrevProfitableAlgorithmType && a.MinerBaseType == PrevProfitableMinerBaseType);
        }

        public double GetCurrentMostProfitValue
        {
            get
            {
                var mostProfitableIndex = GetMostProfitableIndex();
                if (mostProfitableIndex > -1)
                {
                    return Algorithms[mostProfitableIndex].CurrentProfit;
                }
                return 0;
            }
        }
        public double GetCurrentMostProfitValueWithoutPower
        {
            get
            {
                var mostProfitableIndex = GetMostProfitableIndex();
                if (mostProfitableIndex > -1)
                {
                    return Algorithms[mostProfitableIndex].CurrentProfitWithoutPower;
                }
                return 0;
            }
        }

        public double GetPrevMostProfitValue
        {
            get
            {
                var prevProfitableIndex = GetPrevProfitableIndex();
                if (prevProfitableIndex > -1)
                {
                    return Algorithms[prevProfitableIndex].CurrentProfit;
                }

                return 0;
            }
        }
        public double GetPrevMostProfitValueWithoutPower
        {
            get
            {
                var prevProfitableIndex = GetPrevProfitableIndex();
                if (prevProfitableIndex > -1)
                {
                    return Algorithms[prevProfitableIndex].CurrentProfitWithoutPower;
                }

                return 0;
            }
        }

        public MiningPair GetMostProfitablePair()
        {
            return new MiningPair(Device, Algorithms[GetMostProfitableIndex()]);
        }

        public bool HasProfitableAlgo()
        {
            return GetMostProfitableIndex() > -1;
        }

        public void RestoreOldProfitsState()
        {
            // restore last state
            MostProfitableAlgorithmType = PrevProfitableAlgorithmType;
            MostProfitableMinerBaseType = PrevProfitableMinerBaseType;
        }

        public void SetNotMining()
        {
            // device isn't mining (e.g. below profit threshold) so set state to none
            PrevProfitableAlgorithmType = AlgorithmType.NONE;
            PrevProfitableMinerBaseType = MinerBaseType.NONE;
            MostProfitableAlgorithmType = AlgorithmType.NONE;
            MostProfitableMinerBaseType = MinerBaseType.NONE;
        }

        public void CalculateProfits(Dictionary<AlgorithmType, double> profits)
        {
            // save last state
            PrevProfitableAlgorithmType = MostProfitableAlgorithmType;
            PrevProfitableMinerBaseType = MostProfitableMinerBaseType;
            // assume none is profitable
            MostProfitableAlgorithmType = AlgorithmType.NONE;
            MostProfitableMinerBaseType = MinerBaseType.NONE;
            // calculate new profits
            foreach (var algo in Algorithms)
            {
                if (algo is DualAlgorithm algoDual)
                {
                    //Helpers.ConsolePrint("*********", algo.ToString());
                    algoDual.UpdateCurProfit(profits, algo.DeviceType, algo.MinerBaseType);
                }
  //                  Helpers.ConsolePrint("*********", "algo.NiceHashID: " + algo.NiceHashID + " algo.DualNiceHashID: " + algo.DualNiceHashID);
                algo.UpdateCurProfit(profits, algo.DeviceType, algo.MinerBaseType);
            }

            // find max paying value and save key
            double maxProfit = 0;
            if (ConfigManager.GeneralConfig.Force_mining_if_nonprofitable)
            {
                maxProfit = -1000000000000000;
            }
            foreach (var algo in Algorithms)
            {
                if (maxProfit < algo.CurrentProfit)
                {
                    maxProfit = algo.CurrentProfit;
                    MostProfitableAlgorithmType = algo.DualNiceHashID;
                    //Helpers.ConsolePrint("MostProfitableAlgorithmType", MostProfitableAlgorithmType.ToString());
                    MostProfitableMinerBaseType = algo.MinerBaseType;
                    //                        Helpers.ConsolePrint("PROFIT", "WARNING! Mining nonprofitable");
                }
                if (algo.Forced)
                {
                    MostProfitableAlgorithmType = algo.DualNiceHashID;
                    MostProfitableMinerBaseType = algo.MinerBaseType;
                    break;
                }
            }
        }
    }
}
