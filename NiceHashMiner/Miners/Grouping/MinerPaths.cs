/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner.Configs.ConfigJsonFile;
using NiceHashMiner.Devices;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Miners.Grouping
{
    public class MinerPathPackageFile : ConfigFile<MinerPathPackage>
    {
        public MinerPathPackageFile(string name)
            : base(Folders.Internals, $"{name}.json", $"{name}_old.json")
        { }
    }

    public class MinerPathPackage
    {
        public string Name;
        public DeviceGroupType DeviceType;
        public List<MinerTypePath> MinerTypes;

        public MinerPathPackage(DeviceGroupType type, List<MinerTypePath> paths)
        {
            DeviceType = type;
            MinerTypes = paths;
            Name = DeviceType.ToString();
        }
    }

    public class MinerTypePath
    {
        public string Name;
        public MinerBaseType Type;
        public List<MinerPath> Algorithms;

        public MinerTypePath(MinerBaseType type, List<MinerPath> paths)
        {
            Type = type;
            Algorithms = paths;
            Name = type.ToString();
        }
    }

    public class MinerPath
    {
        public string Name;
        public AlgorithmType Algorithm;
        public string Path;

        public MinerPath(AlgorithmType algo, string path)
        {
            Algorithm = algo;
            Path = path;
            Name = Algorithm.ToString();
        }
    }

    /// <summary>
    /// MinerPaths, used just to store miners paths strings. Only one instance needed
    /// </summary>
    public static class MinerPaths
    {
        public static class Data
        {
            // root binary folder
            private const string Bin = @"bin";

            /// <summary>
            /// ccminers
            /// </summary>
//            public const string CcminerDecred = Bin + @"\ccminer_decred\ccminer.exe";

//            public const string CcminerNanashi = Bin + @"\ccminer_nanashi\ccminer.exe"; //deprecated
            public const string CcminerNeoscrypt = Bin + @"\ccminer_neoscrypt\ccminer.exe";
//            public const string CcminerSp = Bin + @"\ccminer_sp\ccminer.exe";
            public const string CcminerTPruvot = Bin + @"\ccminer_tpruvot\ccminer.exe";
//            public const string CcminerCryptonight = Bin + @"\ccminer_cryptonight\ccminer.exe";
//            public const string CcminerX11Gost = Bin + @"\ccminer_x11gost\ccminer.exe";
            public const string CcminerKlausT = Bin + @"\ccminer_klaust\ccminer.exe";
            public const string CcminerMTP = Bin + @"\ccminer_mtp\ccminer.exe";

            /// <summary>
            /// ethminers
            /// </summary>
            public const string Ethminer = Bin + @"\ethminer\ethminer.exe";

            /// <summary>
            /// sgminers
            /// </summary>
            public const string Sgminer560General = Bin + @"\sgminer-5-6-0-general\sgminer.exe";

//            public const string SgminerGm = Bin + @"\sgminer-gm\sgminer.exe";
            public const string SgminerKl = Bin + @"\sgminer-kl\sgminer.exe";

            public const string NhEqMiner = Bin + @"\nheqminer_v0.4b\NhEqMiner.exe";
            public const string Excavator = Bin + @"\excavator\excavator.exe";

            public const string XmrStackCpuMiner = Bin + @"\xmr-stak-cpu\xmr-stak-cpu.exe";
            public const string XmrStakAmd = Bin + @"\xmr-stak-amd\xmr-stak-amd.exe";
            public const string XmrStak = Bin + @"\xmr-stak\xmr-stak.exe";
            public const string Xmrig = Bin + @"\xmrig\xmrig.exe";
            public const string XmrigAMD = Bin + @"\xmrig\xmrig.exe";
            public const string XmrigNVIDIA = Bin + @"\xmrig\xmrig.exe";
            public const string XmrStakHeavy = Bin + @"\xmr-stak_heavy\xmr-stak.exe";

            public const string CpuMiner = Bin + @"\cpuminer_opt\cpuminer.exe";
            public const string lyclMiner = Bin + @"\lyclMiner\lyclMiner.exe";
            public const string mkxminer = Bin3rdParty + @"\mkxminer\mkxminer.exe";
            public const string teamredminer = Bin3rdParty + @"\teamredminer\teamredminer.exe";
            public const string Phoenix = Bin3rdParty + @"\Phoenix\PhoenixMiner.exe";
            public const string Nanominer = Bin3rdParty + @"\Nanominer\nanominer.exe";
            public const string lolMiner = Bin3rdParty + @"\lolMiner\lolMiner.exe";
            public const string lolMinerBEAM = Bin3rdParty + @"\lolMinerBEAM\lolMiner.exe";
            public const string EthLargement = Bin3rdParty + @"\ethlargement\OhGodAnETHlargementPill-r2.exe";
            public const string None = "";

            // root binary folder
            private const string Bin3rdParty = @"bin_3rdparty";

            public const string ClaymoreZcashMiner = Bin3rdParty + @"\claymore_zcash\ZecMiner64.exe";
            public const string ClaymoreCryptoNightMiner = Bin3rdParty + @"\claymore_cryptonight\NsGpuCNMiner.exe";
            public const string ClaymoreNeoscryptMiner = Bin3rdParty + @"\claymore_neoscrypt\NeoScryptMiner.exe";
            public const string OptiminerZcashMiner = Bin3rdParty + @"\optiminer_zcash_win\Optiminer.exe";
            public const string ClaymoreDual = Bin3rdParty + @"\claymore_dual\EthDcrMiner64.exe";
            public const string Ewbf = Bin3rdParty + @"\ewbf\miner.exe";
            public const string Prospector = Bin3rdParty + @"\prospector\prospector.exe";
            public const string Dstm = Bin3rdParty + @"\dstm\zm.exe";
            public const string CastXMR = Bin3rdParty + @"\castxmr\cast_xmr-vega.exe";
            public const string hsrneoscrypt = Bin3rdParty + @"\hsrminer_neoscrypt\hsrminer_neoscrypt.exe";
            public const string CryptoDredge = Bin3rdParty + @"\CryptoDredge\CryptoDredge.exe";
           // public const string CryptoDredgeV8 = Bin3rdParty + @"\CryptoDredgeV8\CryptoDredge.exe";
            public const string ZEnemy = Bin3rdParty + @"\Z-Enemy\z-enemy.exe";
            public const string trex = Bin3rdParty + @"\t-rex\t-rex.exe";
            public const string SRBMiner = Bin3rdParty + @"\SRBMiner\SRBMiner-MULTI.exe";
            public const string GMiner = Bin3rdParty + @"\gminer\miner.exe";
            public const string Bminer = Bin3rdParty + @"\bminer\bminer.exe";
            public const string WildRig = Bin3rdParty + @"\WildRig\wildrig.exe";
            public const string TTMiner = Bin3rdParty + @"\TT-Miner\TT-Miner.exe";
            public const string NBMiner = Bin3rdParty + @"\NBMiner\nbminer.exe";
            public const string miniZ = Bin3rdParty + @"\miniZ\miniZ.exe";
        }

        // NEW START
        ////////////////////////////////////////////
        // Pure functions
        //public static bool IsMinerAlgorithmAvaliable(List<Algorithm> algos, MinerBaseType minerBaseType, AlgorithmType algorithmType) {
        //    return algos.FindIndex((a) => a.MinerBaseType == minerBaseType && a.NiceHashID == algorithmType) > -1;
        //}

        public static string GetPathFor(MinerBaseType minerBaseType, AlgorithmType algoType,
            DeviceGroupType devGroupType, bool def = false)
        {
            if (!def & ConfigurableMiners.Contains(minerBaseType))
            {
                // Override with internals
                var path = MinerPathPackages.Find(p => p.DeviceType == devGroupType)
                    .MinerTypes.Find(p => p.Type == minerBaseType)
                    .Algorithms.Find(p => p.Algorithm == algoType);
                if (path != null)
                {
                    if (File.Exists(path.Path))
                    {
                        return path.Path;
                    }
                    Helpers.ConsolePrint("PATHS", $"Path {path.Path} not found, using defaults");
                }
            }
            // Temp workaround
            if (minerBaseType == MinerBaseType.XmrStak && algoType == AlgorithmType.CryptoNightHeavy)
                return Data.XmrStakHeavy;
            if (minerBaseType == MinerBaseType.CryptoDredge && algoType == AlgorithmType.CryptoNightV8)
                // return NvidiaGroups.CryptoDredgeV8(algoType, devGroupType);
                return Data.CryptoDredge;

            switch (minerBaseType)
            {
                case MinerBaseType.ccminer:
                    return NvidiaGroups.Ccminer_path(algoType, devGroupType);
                case MinerBaseType.sgminer:
                    return AmdGroup.SgminerPath(algoType);
                case MinerBaseType.nheqminer:
                    return Data.NhEqMiner;
                case MinerBaseType.ethminer:
                    return Data.Ethminer;
                case MinerBaseType.Claymore:
                    return AmdGroup.ClaymorePath(algoType);
                case MinerBaseType.OptiminerAMD:
                    return Data.OptiminerZcashMiner;
                //case MinerBaseType.excavator:
                //    return Data.Excavator;
                case MinerBaseType.XmrStak:
                    return Data.XmrStak;
                case MinerBaseType.ccminer_alexis:
                    return NvidiaGroups.CcminerUnstablePath(algoType, devGroupType);
                case MinerBaseType.experimental:
                    return Experimental.GetPath(algoType, devGroupType);
                case MinerBaseType.EWBF:
                    return Data.Ewbf;
                case MinerBaseType.Prospector:
                    return Data.Prospector;
                case MinerBaseType.Xmrig:
                    return Data.Xmrig;
                case MinerBaseType.XmrigAMD:
                    return Data.XmrigAMD;
                case MinerBaseType.SRBMiner:
                    return Data.SRBMiner;
                case MinerBaseType.dstm:
                    return Data.Dstm;
                case MinerBaseType.cpuminer:
                    return Data.CpuMiner;
                case MinerBaseType.CastXMR:
                    return Data.CastXMR;
                case MinerBaseType.lyclMiner:
                    return Data.lyclMiner;
                case MinerBaseType.hsrneoscrypt:
                    return NvidiaGroups.hsrneoscrypt_path(algoType, devGroupType);
                case MinerBaseType.CryptoDredge:
                    return NvidiaGroups.CryptoDredge(algoType, devGroupType);
                case MinerBaseType.ZEnemy:
                    return NvidiaGroups.ZEnemy(algoType, devGroupType);
                case MinerBaseType.trex:
                    return NvidiaGroups.trex(algoType, devGroupType);
                case MinerBaseType.mkxminer:
                    return Data.mkxminer;
                case MinerBaseType.teamredminer:
                    return Data.teamredminer;
                case MinerBaseType.Phoenix:
                    return Data.Phoenix;
                case MinerBaseType.GMiner:
                    return Data.GMiner;
                case MinerBaseType.lolMiner:
                    return Data.lolMiner;
                case MinerBaseType.WildRig:
                    return Data.WildRig;
                case MinerBaseType.lolMinerBEAM:
                    return Data.lolMinerBEAM;
                case MinerBaseType.Bminer:
                    return Data.Bminer;
                case MinerBaseType.TTMiner:
                    return Data.TTMiner;
                case MinerBaseType.XmrigNVIDIA:
                    return Data.Xmrig;
                case MinerBaseType.NBMiner:
                    return Data.NBMiner;
                case MinerBaseType.miniZ:
                    return Data.miniZ;
                case MinerBaseType.Nanominer:
                    return Data.Nanominer;
            }
            return Data.None;
        }

        public static string GetPathFor(ComputeDevice computeDevice,
            Algorithm algorithm /*, Options: MinerPathsConfig*/)
        {
            if (computeDevice == null || algorithm == null)
            {
                return Data.None;
            }

            return GetPathFor(
                algorithm.MinerBaseType,
                algorithm.NiceHashID,
                computeDevice.DeviceGroupType
            );
        }

        public static bool IsValidMinerPath(string minerPath)
        {
            // TODO make a list of valid miner paths and check that instead
            return minerPath != null && Data.None != minerPath && minerPath != "";
        }

        /**
         * InitAlgorithmsMinerPaths gets and sets miner paths
         */
        public static List<Algorithm> GetAndInitAlgorithmsMinerPaths(List<Algorithm> algos,
            ComputeDevice computeDevice /*, Options: MinerPathsConfig*/)
        {
            var retAlgos = algos.FindAll((a) => a != null).ConvertAll((a) =>
            {
                a.MinerBinaryPath = GetPathFor(computeDevice, a /*, Options*/);
                return a;
            });

            return retAlgos;
        }
        // NEW END

        ////// private stuff from here on
        private static class NvidiaGroups
        {
            private static string CcminerSM3X(AlgorithmType algorithmType)
            {
                return Data.CcminerTPruvot;
            }

            private static string CcminerSM5XOrSM6X(AlgorithmType algorithmType)
            {
                switch (algorithmType)
                {
                    case AlgorithmType.Lbry:
                    case AlgorithmType.Blake2s:
                    case AlgorithmType.Skunk:
                    case AlgorithmType.Keccak:
                    case AlgorithmType.Lyra2z:
                        return Data.CcminerTPruvot;
                    case AlgorithmType.Sia:
                    case AlgorithmType.Nist5:
                    case AlgorithmType.NeoScrypt:
                        return Data.CcminerKlausT;
                }

                return Data.CcminerMTP;
            }

            public static string hsrneoscrypt_path(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
                {
                    return Data.hsrneoscrypt;
                }
                // CN exception
                if (nvidiaGroup == DeviceGroupType.NVIDIA_6_x && algorithmType == AlgorithmType.CryptoNight)
                {
                    return Data.hsrneoscrypt;
                }
                // sm5x and sm6x have same settings otherwise
                if (nvidiaGroup == DeviceGroupType.NVIDIA_5_x || nvidiaGroup == DeviceGroupType.NVIDIA_6_x)
                {
                    return Data.hsrneoscrypt; ;
                }
                // TODO wrong case?
                return Data.None; // should not happen
            }

            public static string CryptoDredge(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
                {
                    return Data.CryptoDredge;
                }
                // sm5x and sm6x have same settings otherwise
                if (nvidiaGroup == DeviceGroupType.NVIDIA_5_x || nvidiaGroup == DeviceGroupType.NVIDIA_6_x)
                {
                    return Data.CryptoDredge; ;
                }
                // TODO wrong case?
                return Data.None; // should not happen
            }

            public static string ZEnemy(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
                {
                    return Data.ZEnemy;
                }
                // sm5x and sm6x have same settings otherwise
                if (nvidiaGroup == DeviceGroupType.NVIDIA_5_x || nvidiaGroup == DeviceGroupType.NVIDIA_6_x)
                {
                    return Data.ZEnemy;
                }
                // TODO wrong case?
                return Data.None; // should not happen
            }

            public static string TTMiner(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
                {
                    return Data.TTMiner;
                }
                // sm5x and sm6x have same settings otherwise
                if (nvidiaGroup == DeviceGroupType.NVIDIA_5_x || nvidiaGroup == DeviceGroupType.NVIDIA_6_x)
                {
                    return Data.TTMiner;
                }
                // TODO wrong case?
                return Data.None; // should not happen
            }

            public static string NBMiner(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
                {
                    return Data.NBMiner;
                }
                // sm5x and sm6x have same settings otherwise
                if (nvidiaGroup == DeviceGroupType.NVIDIA_5_x || nvidiaGroup == DeviceGroupType.NVIDIA_6_x)
                {
                    return Data.NBMiner;
                }
                // TODO wrong case?
                return Data.None; // should not happen
            }
            public static string miniZ(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
                {
                    return Data.miniZ;
                }
                // sm5x and sm6x have same settings otherwise
                if (nvidiaGroup == DeviceGroupType.NVIDIA_5_x || nvidiaGroup == DeviceGroupType.NVIDIA_6_x)
                {
                    return Data.miniZ;
                }
                // TODO wrong case?
                return Data.None; // should not happen
            }

            public static string trex(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
                {
                    return Data.trex;
                }
                // sm5x and sm6x have same settings otherwise
                if (nvidiaGroup == DeviceGroupType.NVIDIA_5_x || nvidiaGroup == DeviceGroupType.NVIDIA_6_x)
                {
                    return Data.trex; ;
                }
                // TODO wrong case?
                return Data.None; // should not happen
            }

            public static string Ccminer_path(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                switch (nvidiaGroup)
                {
                    // sm21 and sm3x no longer have same settings since tpruvot dropped 21 support
                    case DeviceGroupType.NVIDIA_3_x:
                        return CcminerSM3X(algorithmType);
                    // CN exception
                    case DeviceGroupType.NVIDIA_6_x when algorithmType == AlgorithmType.CryptoNight:
                        return Data.CcminerTPruvot;
                    // sm5x and sm6x have same settings otherwise
                    case DeviceGroupType.NVIDIA_5_x:
                    case DeviceGroupType.NVIDIA_6_x:
                        return CcminerSM5XOrSM6X(algorithmType);
                }
                // TODO wrong case?
                return Data.None; // should not happen
            }

            public static string CcminerUnstablePath(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm5x and sm6x have same settings
                // TODO wrong case?
                return Data.None; // should not happen
            }
        }

        private static class AmdGroup
        {
            public static string SgminerPath(AlgorithmType type)
            {
                if (AlgorithmType.X16R == type)
                {
                    return Data.SgminerKl;
                }
                return Data.SgminerKl;
            }

            public static string ClaymorePath(AlgorithmType type)
            {
                switch (type)
                {
                    case AlgorithmType.Equihash:
                        return Data.ClaymoreZcashMiner;
                    case AlgorithmType.CryptoNightV7:
                        return Data.ClaymoreCryptoNightMiner;
                    case AlgorithmType.DaggerHashimoto:
                        return Data.ClaymoreDual;
                    case AlgorithmType.NeoScrypt:
                        return Data.ClaymoreNeoscryptMiner;
                }
                return Data.None; // should not happen
            }
            public static string CastXMR(AlgorithmType algorithmType)
            {
                if (AlgorithmType.CryptoNightV7 == algorithmType || AlgorithmType.CryptoNightHeavy == algorithmType || AlgorithmType.CryptoNightV8 == algorithmType )
                {
                    return Data.CastXMR;
                }
                return Data.CastXMR;
            }
            public static string lyclMiner(AlgorithmType algorithmType)
            {
                if (AlgorithmType.Lyra2REv3 == algorithmType )
                {
                    return Data.lyclMiner;
                }
                return Data.lyclMiner;
            }
            public static string mkxminer(AlgorithmType algorithmType)
            {
                if (AlgorithmType.Lyra2REv2 == algorithmType )
                {
                    return Data.mkxminer;
                }
                return Data.mkxminer;
            }
            public static string teamredminer(AlgorithmType algorithmType)
            {
                if (AlgorithmType.Lyra2z == algorithmType )
                {
                    return Data.teamredminer;
                }
                if (AlgorithmType.CryptoNightV8 == algorithmType)
                {
                    return Data.teamredminer;
                }
                if (AlgorithmType.Lyra2REv3 == algorithmType)
                {
                    return Data.teamredminer;
                }
                return Data.teamredminer;
            }
            public static string lolMiner(AlgorithmType algorithmType)
            {
                if (AlgorithmType.ZHash == algorithmType )
                {
                    return Data.lolMiner;
                }
                if (AlgorithmType.GrinCuckatoo31 == algorithmType)
                {
                    return Data.lolMiner;
                }
                return Data.lolMiner;
            }
        }

        // unstable miners, NVIDIA for now
        private static class Experimental
        {
            public static string GetPath(AlgorithmType algoType, DeviceGroupType devGroupType)
            {
                return devGroupType == DeviceGroupType.NVIDIA_6_x
                    ? NvidiaGroups.Ccminer_path(algoType, devGroupType)
                    : Data.None;
            }
        }

        private static readonly List<MinerPathPackage> MinerPathPackages = new List<MinerPathPackage>();

        private static readonly List<MinerBaseType> ConfigurableMiners = new List<MinerBaseType>
        {
            MinerBaseType.ccminer,
            MinerBaseType.sgminer
        };

        public static void InitializePackages()
        {
            var defaults = new List<MinerPathPackage>();
            for (var i = DeviceGroupType.NONE + 1; i < DeviceGroupType.LAST; i++)
            {
                var package = GroupAlgorithms.CreateDefaultsForGroup(i);
                var minerTypePaths = (from type in ConfigurableMiners
                    where package.ContainsKey(type)
                    let minerPaths = package[type].Select(algo =>
                        new MinerPath(algo.NiceHashID, GetPathFor(type, algo.NiceHashID, i, true))).ToList()
                    select new MinerTypePath(type, minerPaths)).ToList();
                if (minerTypePaths.Count > 0)
                {
                    defaults.Add(new MinerPathPackage(i, minerTypePaths));
                }
            }

            foreach (var pack in defaults)
            {
                var packageName = $"MinerPathPackage_{pack.Name}";
                var packageFile = new MinerPathPackageFile(packageName);
                var readPack = packageFile.ReadFile();
                if (readPack == null)
                {
                    // read has failed
                    Helpers.ConsolePrint("MinerPaths", "Creating internal paths config " + packageName);
                    MinerPathPackages.Add(pack);
                    packageFile.Commit(pack);
                }
                else
                {
                    Helpers.ConsolePrint("MinerPaths", "Loading internal paths config " + packageName);
                    var isChange = false;
                    foreach (var miner in pack.MinerTypes)
                    {
                        var readMiner = readPack.MinerTypes.Find(x => x.Type == miner.Type);
                        if (readMiner != null)
                        {
                            // file contains miner type
                            foreach (var algo in miner.Algorithms)
                            {
                                if (!readMiner.Algorithms.Exists(x => x.Algorithm == algo.Algorithm))
                                {
                                    // file does not contain algo on this miner
                                    Helpers.ConsolePrint("PATHS",
                                        $"Algorithm {algo.Name} not found in miner {miner.Name} on device {pack.Name}. Adding default");
                                    readMiner.Algorithms.Add(algo);
                                    isChange = true;
                                }
                            }
                        }
                        else
                        {
                            // file does not contain miner type
                            Helpers.ConsolePrint("PATHS", $"Miner {miner.Name} not found on device {pack.Name}");
                            readPack.MinerTypes.Add(miner);
                            isChange = true;
                        }
                    }
                    MinerPathPackages.Add(readPack);
                    if (isChange) packageFile.Commit(readPack);
                }
            }
        }
    }
}
