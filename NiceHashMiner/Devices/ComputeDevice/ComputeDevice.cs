/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner.Configs;
using NiceHashMiner.Configs.Data;
using NiceHashMiner.Miners.Grouping;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMinerLegacy.Common.Enums;
using System.Threading.Tasks;
using System.Management;
using System;
using NiceHashMinerLegacy.UUID;

namespace NiceHashMiner.Devices
{
    public class ComputeDevice
    {
        public readonly int ID;

        public int Index { get; protected set; } // For socket control, unique

        // to identify equality;
        public readonly string Name; // { get; set; }

        // name count is the short name for displaying in moning groups
        public readonly string NameCount;
        public bool Enabled;

        public readonly DeviceGroupType DeviceGroupType;

        // CPU, NVIDIA, AMD
        public readonly DeviceType DeviceType;

        // UUID now used for saving
        public string Uuid { get; protected set; }
        public string NewUuid { get; protected set; }

        // used for Claymore indexing
        public int BusID { get; protected set; } = -1;
        public int IDByBus = -1;

        // used for lolMiner indexing
        public double lolMinerBusID { get; set; } = -1;

        // CPU extras
        public int Threads { get; protected set; }
        public ulong AffinityMask { get; protected set; }

        // GPU extras
        public ulong GpuRam;
        public readonly bool IsEtherumCapale;
        public static readonly ulong Memory3Gb = 3221225472;
        public static readonly ulong Memory4Gb = 4293918720;
        public static double HashRate = 0.0d;
        // sgminer extra quickfix
        //public readonly bool IsOptimizedVersion;
        public string Codename { get; protected set; }
        public string Manufacturer = "UNK";

        public string InfSection { get; protected set; }

        // amd has some algos not working with new drivers
        public bool DriverDisableAlgos { get; protected set; }

        protected List<Algorithm> AlgorithmSettings;

        public string BenchmarkCopyUuid { get; set; }
        public string TuningCopyUuid { get; set; }
        public int AlgorithmID { get; set; }

        public virtual float Load => -1;
        public virtual float Temp => -1;
        public virtual int FanSpeed => -1;
        public virtual double PowerUsage => -1;
        //********************************************************************************************************************
        private const string Tag = "CPUDetector";
        internal class CPUDetectionResult
        {
            public int NumberOfCPUCores { get; internal set; }
            public int VirtualCoresCount { get; internal set; }
            public bool IsHyperThreadingEnabled => VirtualCoresCount > NumberOfCPUCores;
            public List<CpuInfo> CpuInfos { get; internal set; }
        }

        internal struct CpuInfo
        {
            public string VendorID;
            public string Family;
            //public string Model;
            public string PhysicalID;
            public string ModelName;
            public int NumberOfCores;
        }
        public class BaseDevice
        {
            public BaseDevice(BaseDevice bd)
            {
                DeviceType = bd.DeviceType;
                UUID = bd.UUID;
                Name = bd.Name;
                ID = bd.ID;
            }

            public BaseDevice(DeviceType deviceType, string uuid, string name, int id)
            {
                DeviceType = deviceType;
                UUID = uuid;
                Name = name;
                ID = id;
            }
            public string Name { get; }
            public DeviceType DeviceType { get; }
            public string UUID { get; }

            // TODO the ID will correspond to CPU Index, CUDA ID and AMD/OpenCL ID
            public int ID { get; }
        }

        public class CPUDevice : BaseDevice
        {
            public CPUDevice(BaseDevice bd, int cpuCount, int threadsPerCPU, bool supportsHyperThreading, List<ulong> affinityMasks) : base(bd)
            {
                PhysicalProcessorCount = cpuCount;
                ThreadsPerCPU = threadsPerCPU;
                SupportsHyperThreading = supportsHyperThreading;
                AffinityMasks = affinityMasks;
            }

            public int PhysicalProcessorCount { get; }
            public int ThreadsPerCPU { get; }
            public bool SupportsHyperThreading { get; }
            public List<ulong> AffinityMasks { get; protected set; } // TODO check if this makes any sense
        }

        public static ulong CreateAffinityMask(int index, int percpu)
        {
            ulong mask = 0;
            const ulong one = 0x0000000000000001;
            for (var i = index * percpu; i < (index + 1) * percpu; i++)
                mask = mask | (one << i);
            return mask;
        }

        public static Task<CPUDevice> TryQueryCPUDeviceTask()
        {
            return Task.Run(() =>
            {
                if (!CpuUtils.IsCpuMiningCapable()) return null;

                var cpuDetectResult = QueryCPUDevice();
                // get all CPUs
                var cpuCount = CpuID.GetPhysicalProcessorCount();
                var name = CpuID.GetCpuName().Trim();
                // get all cores (including virtual - HT can benefit mining)
                var threadsPerCpu = cpuDetectResult.VirtualCoresCount / cpuCount;
                // TODO important move this to settings
                var threadsPerCpuMask = threadsPerCpu;
                if (threadsPerCpu * cpuCount > 64)
                {
                    // set lower
                    threadsPerCpuMask = 64;
                }

                List<ulong> affinityMasks = null;
                // multiple CPUs are identified as a single CPU from nhm perspective, it is the miner plugins job to handle this correctly
                if (cpuCount > 1)
                {
                    name = $"({cpuCount}x){name}";
                    affinityMasks = new List<ulong>();
                    for (var i = 0; i < cpuCount; i++)
                    {
                        var affinityMask = CreateAffinityMask(i, threadsPerCpuMask);
                        affinityMasks.Add(affinityMask);
                    }
                }
                var hashedInfo = $"{0}--{name}--{threadsPerCpu}";
                foreach (var cpuInfo in cpuDetectResult.CpuInfos)
                {
                    hashedInfo += $"{cpuInfo.Family}--{cpuInfo.ModelName}--{cpuInfo.NumberOfCores}--{cpuInfo.PhysicalID}--{cpuInfo.VendorID}";
                }
                var uuidHEX = UUID.GetHexUUID(hashedInfo);
                var uuid = $"CPU-{uuidHEX}";

                // plugin device
                var bd = new BaseDevice(DeviceType.CPU, uuid, name, 0);
                var cpu = new CPUDevice(bd, cpuCount, threadsPerCpu, cpuDetectResult.IsHyperThreadingEnabled, affinityMasks);
                return cpu;
            });
        }

        public static CPUDevice TryCPUDevice()
        {
                if (!CpuUtils.IsCpuMiningCapable()) return null;

                var cpuDetectResult = QueryCPUDevice();
                // get all CPUs
                var cpuCount = CpuID.GetPhysicalProcessorCount();
                var name = CpuID.GetCpuName().Trim();
                // get all cores (including virtual - HT can benefit mining)
                var threadsPerCpu = cpuDetectResult.VirtualCoresCount / cpuCount;
                // TODO important move this to settings
                var threadsPerCpuMask = threadsPerCpu;
                if (threadsPerCpu * cpuCount > 64)
                {
                    // set lower
                    threadsPerCpuMask = 64;
                }

                List<ulong> affinityMasks = null;
                // multiple CPUs are identified as a single CPU from nhm perspective, it is the miner plugins job to handle this correctly
                if (cpuCount > 1)
                {
                    name = $"({cpuCount}x){name}";
                    affinityMasks = new List<ulong>();
                    for (var i = 0; i < cpuCount; i++)
                    {
                        var affinityMask = CreateAffinityMask(i, threadsPerCpuMask);
                        affinityMasks.Add(affinityMask);
                    }
                }
                var hashedInfo = $"{0}--{name}--{threadsPerCpu}";
                foreach (var cpuInfo in cpuDetectResult.CpuInfos)
                {
                    hashedInfo += $"{cpuInfo.Family}--{cpuInfo.ModelName}--{cpuInfo.NumberOfCores}--{cpuInfo.PhysicalID}--{cpuInfo.VendorID}";
                }
                var uuidHEX = UUID.GetHexUUID(hashedInfo);
                var uuid = $"CPU-{uuidHEX}";

                // plugin device
                var bd = new BaseDevice(DeviceType.CPU, uuid, name, 0);
                var cpu = new CPUDevice(bd, cpuCount, threadsPerCpu, cpuDetectResult.IsHyperThreadingEnabled, affinityMasks);
                return cpu;
        }
        // maybe this will come in handy
        private static CPUDetectionResult QueryCPUDevice()
        {
            var ret = new CPUDetectionResult
            {
                CpuInfos = GetCpuInfos(),
                VirtualCoresCount = GetVirtualCoresCount(),
                //NumberOfCPUCores = 0 // calculate from CpuInfos
            };
            ret.NumberOfCPUCores = ret.CpuInfos.Select(info => info.NumberOfCores).Sum();
            return ret;
        }

        private static List<CpuInfo> GetCpuInfos()
        {
            var ret = new List<CpuInfo>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
                using (var query = searcher.Get())
                {
                    foreach (var obj in query)
                    {
                        var numberOfCores = Convert.ToInt32(obj.GetPropertyValue("NumberOfCores"));
                        var info = new CpuInfo
                        {
                            Family = obj["Family"].ToString(),
                            VendorID = obj["Manufacturer"].ToString(),
                            ModelName = obj["Name"].ToString(),
                            PhysicalID = obj["ProcessorID"].ToString(),
                            NumberOfCores = numberOfCores
                        };
                        ret.Add(info);
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint(Tag, $"GetCpuInfos error: {e.Message}");
            }
            return ret;
        }

        private static int GetVirtualCoresCount()
        {
            var virtualCoreCount = 0;
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT NumberOfLogicalProcessors FROM Win32_ComputerSystem"))
                using (var query = searcher.Get())
                {
                    foreach (var item in query)
                    {
                        virtualCoreCount += Convert.ToInt32(item.GetPropertyValue("NumberOfLogicalProcessors"));
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint(Tag, $"GetVirtualCoresCount error: {e.Message}");
            }
            return virtualCoreCount;
        }

        private static async Task DetectCPU()
        {
            Helpers.ConsolePrint("DetectCPU", $"DetectCPU START");
            var cpu = await TryQueryCPUDeviceTask();
            //DetectionResult.CPU = cpu;
            if (cpu == null)
            {
                Helpers.ConsolePrint("DetectCPU", $"Found No Compatible CPU");
            }
            else
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"Found CPU:");
                stringBuilder.AppendLine($"\tUUID: {cpu.UUID}");
                stringBuilder.AppendLine($"\tName: {cpu.Name}");
                stringBuilder.AppendLine($"\tPhysicalProcessorCount: {cpu.PhysicalProcessorCount}");
                stringBuilder.AppendLine($"\tThreadsPerCPU: {cpu.ThreadsPerCPU}");
                stringBuilder.AppendLine($"\tSupportsHyperThreading: {cpu.SupportsHyperThreading}");
                Helpers.ConsolePrint("DetectCPU", stringBuilder.ToString());
            }
            Helpers.ConsolePrint("DetectCPU", $"DetectCPU END");
        }
        //********************************************************************************************************************
        // Ambiguous constructor
        protected ComputeDevice(int id, string name, bool enabled, DeviceGroupType group, bool ethereumCapable,
            DeviceType type, string nameCount, ulong gpuRam, string manufacturer)
        {
            ID = id;
            Name = name;
            Enabled = enabled;
            DeviceGroupType = group;
            IsEtherumCapale = ethereumCapable;
            DeviceType = type;
            NameCount = nameCount;
            GpuRam = gpuRam;
            Manufacturer = manufacturer;
        }

        // Fake dev
        public ComputeDevice(int id)
        {
            ID = id;
            Name = "fake_" + id;
            NameCount = Name;
            Enabled = true;
            DeviceType = DeviceType.CPU;
            DeviceGroupType = DeviceGroupType.NONE;
            IsEtherumCapale = false;
            //IsOptimizedVersion = false;
            Codename = "fake";
            Uuid = GetUuid(ID, GroupNames.GetGroupName(DeviceGroupType, ID), Name, DeviceGroupType);
            CPUDevice cpu = TryCPUDevice();
            NewUuid = cpu.UUID;
            GpuRam = 0;
        }

        // combines long and short name
        public string GetFullName()
        {
            return string.Format(International.GetText("ComputeDevice_Full_Device_Name"), NameCount, Name);
        }

        public Algorithm GetAlgorithm(Algorithm modelAlgo)
        {
            return GetAlgorithm(modelAlgo.MinerBaseType, modelAlgo.NiceHashID, modelAlgo.SecondaryNiceHashID);
        }

        public Algorithm GetAlgorithm(MinerBaseType minerBaseType, AlgorithmType algorithmType,
            AlgorithmType secondaryAlgorithmType)
        {
            var toSetIndex = AlgorithmSettings.FindIndex(a =>
                a.NiceHashID == algorithmType && a.MinerBaseType == minerBaseType &&
                a.SecondaryNiceHashID == secondaryAlgorithmType);
            return toSetIndex > -1 ? AlgorithmSettings[toSetIndex] : null;
        }

        //public Algorithm GetAlgorithm(string algoID) {
        //    int toSetIndex = this.AlgorithmSettings.FindIndex((a) => a.AlgorithmStringID == algoID);
        //    if (toSetIndex > -1) {
        //        return this.AlgorithmSettings[toSetIndex];
        //    }
        //    return null;
        //}

        public void CopyBenchmarkSettingsFrom(ComputeDevice copyBenchCDev)
        {
            foreach (var copyFromAlgo in copyBenchCDev.AlgorithmSettings)
            {
                var setAlgo = GetAlgorithm(copyFromAlgo);
                if (setAlgo != null)
                {

                    setAlgo.Enabled = copyFromAlgo.Enabled;
                    setAlgo.BenchmarkSpeed = copyFromAlgo.BenchmarkSpeed;
                    setAlgo.ExtraLaunchParameters = copyFromAlgo.ExtraLaunchParameters;
                    setAlgo.LessThreads = copyFromAlgo.LessThreads;
                    setAlgo.PowerUsage = copyFromAlgo.PowerUsage;
                    NiceHashMiner.Forms.Form_Settings.ActiveForm.Update();

                    if (setAlgo is DualAlgorithm dualSA && copyFromAlgo is DualAlgorithm dualCFA)
                    {
                        dualSA.SecondaryBenchmarkSpeed = dualCFA.SecondaryBenchmarkSpeed;
                    }
                }
            }
        }

        public void CopyTuningSettingsFrom(ComputeDevice copyTuningCDev)
        {
            foreach (var copyFromAlgo in copyTuningCDev.AlgorithmSettings.OfType<DualAlgorithm>())
            {
                if (GetAlgorithm(copyFromAlgo) is DualAlgorithm setAlgo)
                {
                    setAlgo.IntensitySpeeds = new Dictionary<int, double>(copyFromAlgo.IntensitySpeeds);
                    setAlgo.SecondaryIntensitySpeeds = new Dictionary<int, double>(copyFromAlgo.SecondaryIntensitySpeeds);
                    setAlgo.TuningStart = copyFromAlgo.TuningStart;
                    setAlgo.TuningEnd = copyFromAlgo.TuningEnd;
                    setAlgo.TuningInterval = copyFromAlgo.TuningInterval;
                    setAlgo.TuningEnabled = copyFromAlgo.TuningEnabled;
                    setAlgo.IntensityPowers = new Dictionary<int, double>(copyFromAlgo.IntensityPowers);
                    setAlgo.UseIntensityPowers = copyFromAlgo.UseIntensityPowers;
                    setAlgo.IntensityUpToDate = false;
                }
            }
        }

        #region Config Setters/Getters

        // settings
        // setters
        public void SetFromComputeDeviceConfig(ComputeDeviceConfig config)
        {
            if (config != null && config.UUID == Uuid)
            {
                Enabled = config.Enabled;
            }
        }

        public void SetAlgorithmDeviceConfig(DeviceBenchmarkConfig config)
        {
            if (config != null && config.DeviceUUID == Uuid && config.AlgorithmSettings != null)
            {
                AlgorithmSettings = GroupAlgorithms.CreateForDeviceList(this);
                foreach (var conf in config.AlgorithmSettings)
                {
                    var setAlgo = GetAlgorithm(conf.MinerBaseType, conf.NiceHashID, conf.SecondaryNiceHashID);
                    if (setAlgo != null)
                    {
                        setAlgo.BenchmarkSpeed = conf.BenchmarkSpeed;
                        setAlgo.ExtraLaunchParameters = conf.ExtraLaunchParameters;
                        setAlgo.Enabled = conf.Enabled;
                        setAlgo.Hidden = conf.Hidden;
                        if (conf.Hidden)
                        {
                           // setAlgo.Enabled = false;
                        }
                        setAlgo.LessThreads = conf.LessThreads;
                        setAlgo.PowerUsage = conf.PowerUsage;
                        if (setAlgo is DualAlgorithm dualSA)
                        {
                            dualSA.SecondaryBenchmarkSpeed = conf.SecondaryBenchmarkSpeed;
                            var dualConf = config.DualAlgorithmSettings?.Find(a =>
                                a.SecondaryNiceHashID == dualSA.SecondaryNiceHashID);
                            if (dualConf != null)
                            {
                                dualConf.FixSettingsBounds();
                                dualSA.IntensitySpeeds = dualConf.IntensitySpeeds;
                                dualSA.SecondaryIntensitySpeeds = dualConf.SecondaryIntensitySpeeds;
                                dualSA.TuningEnabled = dualConf.TuningEnabled;
                                dualSA.TuningStart = dualConf.TuningStart;
                                dualSA.TuningEnd = dualConf.TuningEnd;
                                dualSA.TuningInterval = dualConf.TuningInterval;
                                dualSA.IntensityPowers = dualConf.IntensityPowers;
                                dualSA.UseIntensityPowers = dualConf.UseIntensityPowers;
                            }
                        }
                    }
                }
            }
        }

        // getters
        public ComputeDeviceConfig GetComputeDeviceConfig()
        {
            var ret = new ComputeDeviceConfig
            {
                Enabled = Enabled,
                Name = Name,
                UUID = Uuid
            };
            return ret;
        }

        public DeviceBenchmarkConfig GetAlgorithmDeviceConfig()
        {
            var ret = new DeviceBenchmarkConfig
            {
                DeviceName = Name,
                DeviceUUID = Uuid
            };
            // init algo settings
            foreach (var algo in AlgorithmSettings)
            {
                // create/setup
                var conf = new AlgorithmConfig
                {
                    Name = algo.AlgorithmStringID,
                    NiceHashID = algo.NiceHashID,
                    MinerBaseType = algo.MinerBaseType,
                    MinerName = algo.MinerName,
                    BenchmarkSpeed = algo.BenchmarkSpeed,
                    ExtraLaunchParameters = algo.ExtraLaunchParameters,
                    Enabled = algo.Enabled,
                    Hidden = algo.Hidden,
                    LessThreads = algo.LessThreads,
                    PowerUsage =  algo.PowerUsage
                };
                // insert
                if (!conf.Hidden)
                {
                    ret.AlgorithmSettings.Add(conf);
                }
                if (algo is DualAlgorithm dualAlgo)
                {
                    conf.SecondaryNiceHashID = dualAlgo.SecondaryNiceHashID;
                    conf.SecondaryBenchmarkSpeed = dualAlgo.SecondaryBenchmarkSpeed;

                    DualAlgorithmConfig dualConf = new DualAlgorithmConfig
                    {
                        Name = algo.AlgorithmStringID,
                        SecondaryNiceHashID = dualAlgo.SecondaryNiceHashID,
                        IntensitySpeeds = dualAlgo.IntensitySpeeds,
                        SecondaryIntensitySpeeds = dualAlgo.SecondaryIntensitySpeeds,
                        TuningEnabled = dualAlgo.TuningEnabled,
                        TuningStart = dualAlgo.TuningStart,
                        TuningEnd = dualAlgo.TuningEnd,
                        TuningInterval = dualAlgo.TuningInterval,
                        IntensityPowers = dualAlgo.IntensityPowers,
                        UseIntensityPowers = dualAlgo.UseIntensityPowers
                    };
                    if (!conf.Hidden)
                    {
                        ret.DualAlgorithmSettings.Add(dualConf);
                    }
                }
            }

            return ret;
        }

        #endregion Config Setters/Getters

        public List<Algorithm> GetAlgorithmSettings()
        {
            // hello state
            var algos = GetAlgorithmSettingsThirdParty(ConfigManager.GeneralConfig.Use3rdPartyMiners);

            var retAlgos = MinerPaths.GetAndInitAlgorithmsMinerPaths(algos, this);
            ;

            // NVIDIA
            if (DeviceGroupType == DeviceGroupType.NVIDIA_5_x || DeviceGroupType == DeviceGroupType.NVIDIA_6_x)
            {
                retAlgos = retAlgos.FindAll(a => a.MinerBaseType != MinerBaseType.nheqminer);
            }
            else if (DeviceType == DeviceType.NVIDIA)
            {
                retAlgos = retAlgos.FindAll(a => a.MinerBaseType != MinerBaseType.eqm);
            }

            // sort by algo
            retAlgos.Sort((a_1, a_2) => (a_1.NiceHashID - a_2.NiceHashID) != 0
                ? (a_1.NiceHashID - a_2.NiceHashID)
                : ((a_1.MinerBaseType - a_2.MinerBaseType) != 0
                    ? (a_1.MinerBaseType - a_2.MinerBaseType)
                    : (a_1.SecondaryNiceHashID - a_2.SecondaryNiceHashID)));

            return retAlgos;
        }

        public List<Algorithm> GetAlgorithmSettingsFastest()
        {
            // hello state
            var algosTmp = GetAlgorithmSettings();
            var sortDict = new Dictionary<AlgorithmType, Algorithm>();
            foreach (var algo in algosTmp)
            {
                var algoKey = algo.NiceHashID;
                if (sortDict.ContainsKey(algoKey))
                {
                    if (sortDict[algoKey].BenchmarkSpeed < algo.BenchmarkSpeed)
                    {
                        sortDict[algoKey] = algo;
                    }
                }
                else
                {
                    sortDict[algoKey] = algo;
                }
            }

            return sortDict.Values.ToList();
        }

        private List<Algorithm> GetAlgorithmSettingsThirdParty(Use3rdPartyMiners use3rdParty)
        {
            if (use3rdParty == Use3rdPartyMiners.YES)
            {
                return AlgorithmSettings;
            }

            var thirdPartyMiners = new List<MinerBaseType>
            {
                MinerBaseType.Claymore,
                MinerBaseType.OptiminerAMD,
                MinerBaseType.EWBF,
                MinerBaseType.Prospector,
                MinerBaseType.dstm,
                MinerBaseType.CryptoDredge,
                MinerBaseType.hsrneoscrypt,
                MinerBaseType.Phoenix,
                MinerBaseType.trex,
                MinerBaseType.ZEnemy,
                MinerBaseType.CastXMR,
                MinerBaseType.SRBMiner,
                MinerBaseType.teamredminer,
                MinerBaseType.GMiner,
                MinerBaseType.lolMiner,
                MinerBaseType.lolMinerBEAM,
                MinerBaseType.Bminer,
                MinerBaseType.TTMiner,
                MinerBaseType.NBMiner,
                MinerBaseType.miniZ,
                MinerBaseType.Nanominer,
                MinerBaseType.Kawpowminer
            };

            return AlgorithmSettings.FindAll(a => thirdPartyMiners.IndexOf(a.MinerBaseType) == -1);
        }

        // static methods

        protected static string GetUuid(int id, string group, string name, DeviceGroupType deviceGroupType)
        {
            var sha256 = new SHA256Managed();
            var hash = new StringBuilder();
            var mixedAttr = id + group + name + (int) deviceGroupType;
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(mixedAttr), 0,
                Encoding.UTF8.GetByteCount(mixedAttr));
            foreach (var b in hashedBytes)
            {
                hash.Append(b.ToString("x2"));
            }

            // GEN indicates the UUID has been generated and cannot be presumed to be immutable
            return "GEN-" + hash;
        }

        internal bool IsAlgorithmSettingsInitialized()
        {
            return AlgorithmSettings != null;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ComputeDevice) obj);
        }

        protected bool Equals(ComputeDevice other)
        {
            return ID == other.ID && DeviceGroupType == other.DeviceGroupType && DeviceType == other.DeviceType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ID;
                hashCode = (hashCode * 397) ^ (int) DeviceGroupType;
                hashCode = (hashCode * 397) ^ (int) DeviceType;
                return hashCode;
            }
        }

        public static bool operator ==(ComputeDevice left, ComputeDevice right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ComputeDevice left, ComputeDevice right)
        {
            return !Equals(left, right);
        }
        public static string GetManufacturer(string man)
        {
            switch (man)
            {
                case "1002":
                    man = "AMD";
                    break;
                case "1043":
                    man = "ASUSTeK";
                    break;
                case "196D":
                    man = "Club 3D";
                    break;
                case "1092":
                    man = "Diamond Multimedia";
                    break;
                case "18BC":
                    man = "GeCube";
                    break;
                case "1458":
                    man = "Gigabyte";
                    break;
                case "17AF":
                    man = "HIS";
                    break;
                case "16F3":
                    man = "Jetway";
                    break;
                case "1462":
                    man = "MSI";
                    break;
                case "1DA2":
                    man = "Sapphire";
                    break;
                case "148C":
                    man = "PowerColor";
                    break;
                case "1545":
                    man = "VisionTek";
                    break;
                case "1682":
                    man = "XFX";
                    break;
                case "107D":
                    man = "Leadtek";
                    break;
                case "10B0":
                    man = "Gainward";
                    break;
                case "10DE":
                    man = "NVIDIA";
                    break;
                case "154B":
                    man = "PNY";
                    break;
                case "19DA":
                    man = "Zotac";
                    break;
                case "19F1":
                    man = "BFG";
                    break;
                case "1B4C":
                    man = "KFA2";
                    break;
                case "3842":
                    man = "EVGA";
                    break;
                case "7377":
                    man = "Colorful";
                    break;
                default:
                    break;
            }

            return man;
        }
    }
}
