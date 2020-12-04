using NiceHashMiner.Algorithms;
using NiceHashMiner.Devices;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Miners
{
    public static class MinerFactory
    {
        private static Miner CreateClaymore(Algorithm algorithm)
        {
            switch (algorithm.NiceHashID)
            {
                case AlgorithmType.DaggerHashimoto:
                    return new ClaymoreDual(algorithm.SecondaryNiceHashID);
                case AlgorithmType.DaggerHashimoto3GB:
                    return new ClaymoreDual(algorithm.SecondaryNiceHashID);
                case AlgorithmType.DaggerHashimoto4GB:
                    return new ClaymoreDual(algorithm.SecondaryNiceHashID);
                case AlgorithmType.NeoScrypt:
                    return new ClaymoreNeoscryptMiner();
            }

            return null;
        }

        public static Miner CreateMiner(DeviceType deviceType, Algorithm algorithm)
        {
            switch (algorithm.MinerBaseType)
            {
                case MinerBaseType.Claymore:
                    return CreateClaymore(algorithm);
                case MinerBaseType.Xmrig:
                    return new Xmrig();
                case MinerBaseType.SRBMiner:
                    return new SRBMiner();
                case MinerBaseType.hsrneoscrypt:
                    return new hsrneoscrypt();
                case MinerBaseType.CryptoDredge:
                    return new CryptoDredge();
                case MinerBaseType.ZEnemy:
                    return new ZEnemy();
                case MinerBaseType.trex:
                    return new trex();
                case MinerBaseType.teamredminer:
                    return new teamredminer();
                case MinerBaseType.Phoenix:
                    return new Phoenix();
                case MinerBaseType.GMiner:
                    return new GMiner(algorithm.SecondaryNiceHashID);
                case MinerBaseType.lolMiner:
                    return new lolMiner();
                case MinerBaseType.WildRig:
                    return new WildRig();
                case MinerBaseType.NBMiner:
                    return new NBMiner(algorithm.SecondaryNiceHashID);
                case MinerBaseType.miniZ:
                    return new miniZ();
                case MinerBaseType.Nanominer:
                    return new Nanominer();
                case MinerBaseType.Kawpowminer:
                    return new Kawpowminer();
            }

            return null;
        }

        // create miner creates new miners based on device type and algorithm/miner path
        public static Miner CreateMiner(ComputeDevice device, Algorithm algorithm)
        {
            if (device != null && algorithm != null)
            {
                return CreateMiner(device.DeviceType, algorithm);
            }

            return null;
        }
    }
}
