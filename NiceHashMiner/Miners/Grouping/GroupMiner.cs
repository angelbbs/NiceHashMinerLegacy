using NiceHashMiner.Configs;
using System.Collections.Generic;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Linq;

namespace NiceHashMiner.Miners.Grouping
{
    public class GroupMiner
    {
        public Miner Miner { get; protected set; }
        public string DevicesInfoString { get; }
        public AlgorithmType AlgorithmType { get; }

        public AlgorithmType DualAlgorithmType { get; }

        // for now used only for dagger identification AMD or NVIDIA
        public DeviceType DeviceType { get; }

        public double CurrentRate { get; set; }
        public double PowerRate { get; set; }
        public string Key { get; }
        public List<int> DevIndexes { get; }

        public double TotalPower { get; }

        public DateTime StartMinerTime { get; protected set; }
        // , string miningLocation, string btcAdress, string worker
        public GroupMiner(List<MiningPair> miningPairs, string key)
        {
            AlgorithmType = AlgorithmType.NONE;
            DualAlgorithmType = AlgorithmType.NONE;
            DevicesInfoString = "N/A";
            CurrentRate = 0;
            PowerRate = 0;
            Key = key;

            if (miningPairs.Count > 0)
            {
                // sort pairs by device id
                miningPairs.Sort((a, b) => a.Device.ID - b.Device.ID);
                // init name scope and IDs
                {
                    var deviceNames = new List<string>();
                    DevIndexes = new List<int>();
                    foreach (var pair in miningPairs.OrderBy(pair => pair.Device.IDByBus))
                    {
                        deviceNames.Add(pair.Device.NameCount);
                        DevIndexes.Add(pair.Device.Index);
                        //TotalPower += pair.Device.PowerUsage;
                        TotalPower += pair.Algorithm.PowerUsage;
                        //Helpers.ConsolePrint("pair.Device.PowerUsage: ", pair.Device.PowerUsage.ToString());
                        //Helpers.ConsolePrint("pair.Algorithm.PowerUsage: ", pair.Algorithm.PowerUsage.ToString());
                        //Helpers.ConsolePrint("TotalPower: ", TotalPower.ToString());
                    }
                    DevicesInfoString = "{ " + string.Join(", ", deviceNames) + " }";
                }
                // init miner
                {
                    var mPair = miningPairs[0];
                    DeviceType = mPair.Device.DeviceType;
                    Miner = MinerFactory.CreateMiner(mPair.Device, mPair.Algorithm);
                    if (Miner != null)
                    {
                        Miner.InitMiningSetup(new MiningSetup(miningPairs));
                        AlgorithmType = mPair.Algorithm.NiceHashID;
                        DualAlgorithmType = mPair.Algorithm.DualNiceHashID;
                        StartMinerTime = DateTime.Now;
                    }
                }
            }
        }

        public void Stop()
        {
            if (Miner != null && Miner.IsRunning)
            {
                Miner.Stop();
                // wait before going on
                System.Threading.Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);
            }
            CurrentRate = 0;
            PowerRate = 0;
        }

        public void End()
        {
            Miner?.End();
            CurrentRate = 0;
            PowerRate = 0;
        }

        public void Start(string miningLocation, string btcAdress, string worker)
        {
            if (Miner.IsRunning)
            {
                return;
            }
            // Wait before new start
            System.Threading.Thread.Sleep(ConfigManager.GeneralConfig.MinerRestartDelayMS);
            var locationUrl = Globals.GetLocationUrl(AlgorithmType, miningLocation, Miner.ConectionType);
            Miner.Start(locationUrl, btcAdress, worker);
        }
    }
}
