using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs.ConfigJsonFile;
using NiceHashMiner.Devices;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NiceHashMiner.Miners.Grouping
{
    public class MinerPathPackageFile : ConfigFile<MinerPathPackage>
    {
        public MinerPathPackageFile(string name)
            : base(Folders.Temp, $"{name}.json", $"{name}_old.json")
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
            private const string minersBins = @"miners";

            public const string XmrStak = minersBins + @"\xmr-stak\xmr-stak.exe";
            public const string Xmrig = minersBins + @"\xmrig\xmrig.exe";
            public const string lyclMiner = minersBins + @"\lyclMiner\lyclMiner.exe";

            public const string teamredminer = minersBins + @"\teamredminer\teamredminer.exe";
            public const string Phoenix = minersBins + @"\Phoenix\PhoenixMiner.exe";
            public const string Nanominer = minersBins + @"\Nanominer\nanominer.exe";
            public const string Rigel = minersBins + @"\Rigel\rigel.exe";
            public const string lolMiner = minersBins + @"\lolMiner\lolMiner.exe";

            public const string EthLargement = minersBins + @"\ethlargement\OhGodAnETHlargementPill-r2.exe";
            public const string None = "";

            public const string ClaymoreNeoscryptMiner = minersBins + @"\claymore_neoscrypt\NeoScryptMiner.exe";
            public const string ClaymoreDual = minersBins + @"\claymore_dual\EthDcrMiner64.exe";
            public const string Ewbf = minersBins + @"\ewbf\miner.exe";
            public const string CastXMR = minersBins + @"\castxmr\cast_xmr-vega.exe";
            public const string hsrneoscrypt = minersBins + @"\hsrminer_neoscrypt\hsrminer_neoscrypt.exe";
            public const string CryptoDredge = minersBins + @"\CryptoDredge\CryptoDredge.exe";
            public const string ZEnemy = minersBins + @"\Z-Enemy\z-enemy.exe";
            public const string trex = minersBins + @"\t-rex\t-rex.exe";
            public const string SRBMiner = minersBins + @"\SRBMiner\SRBMiner-MULTI.exe";
            public const string GMiner = minersBins + @"\gminer\miner.exe";
            public const string Bminer = minersBins + @"\bminer\bminer.exe";
            public const string WildRig = minersBins + @"\WildRig\wildrig.exe";
            public const string TTMiner = minersBins + @"\TT-Miner\TT-Miner.exe";
            public const string NBMiner = minersBins + @"\NBMiner\nbminer.exe";
            public const string miniZ = minersBins + @"\miniZ\miniZ.exe";
            public const string Kawpowminer = minersBins + @"\kawpowminer\kawpowminer.exe";
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

            switch (minerBaseType)
            {
                case MinerBaseType.Claymore:
                    return AmdGroup.ClaymorePath(algoType);
                case MinerBaseType.XmrStak:
                    return Data.XmrStak;
                case MinerBaseType.ccminer_alexis:
                    return NvidiaGroups.CcminerUnstablePath(algoType, devGroupType);
                case MinerBaseType.EWBF:
                    return Data.Ewbf;
                case MinerBaseType.Xmrig:
                    return Data.Xmrig;
                case MinerBaseType.SRBMiner:
                    return Data.SRBMiner;
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
                case MinerBaseType.Kawpowminer:
                    return Data.Kawpowminer;
                case MinerBaseType.Rigel:
                    return Data.Rigel;
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
            var retAlgos = algos.FindAll((a) => a != null && a.Hidden != true).ConvertAll((a) =>
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
            public static string hsrneoscrypt_path(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
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

            public static string Kawpowminer(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm21 and sm3x have same settings
                if (nvidiaGroup == DeviceGroupType.NVIDIA_2_1 || nvidiaGroup == DeviceGroupType.NVIDIA_3_x)
                {
                    return Data.Kawpowminer;
                }
                // sm5x and sm6x have same settings otherwise
                if (nvidiaGroup == DeviceGroupType.NVIDIA_5_x || nvidiaGroup == DeviceGroupType.NVIDIA_6_x)
                {
                    return Data.Kawpowminer;
                }
                // TODO wrong case?
                return Data.Kawpowminer; // should not happen
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

            public static string CcminerUnstablePath(AlgorithmType algorithmType, DeviceGroupType nvidiaGroup)
            {
                // sm5x and sm6x have same settings
                // TODO wrong case?
                return Data.None; // should not happen
            }
        }

        private static class AmdGroup
        {
            public static string ClaymorePath(AlgorithmType type)
            {
                switch (type)
                {
                    case AlgorithmType.DaggerHashimoto:
                        return Data.ClaymoreDual;
                    case AlgorithmType.NeoScrypt:
                        return Data.ClaymoreNeoscryptMiner;
                }
                return Data.None; // should not happen
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
