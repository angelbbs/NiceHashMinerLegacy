/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;

namespace NiceHashMiner.Configs.Data
{
    [Serializable]
    public class GeneralConfig
    {
        public Version ConfigFileVersion;
        public double ForkFixVersion;
        public bool DecreasePowerCost = false;
        public bool FiatCurrency = false;
        public bool NoShowApiInLog;
        public bool lolMinerOldEnumeration;
        public bool GMinerIDByBusEnumeration = true;
        public bool NoForceTRexClose;
        public bool UseNegativeProfit = false;
        public double DaggerOrderMaxPay = 0;
        public int DaggerHashimoto4GBMaxEpoch = 352;
        public LanguageType Language = LanguageType.En;
        public string DisplayCurrency = "USD";
        public bool DivertRun = true;
        public bool DivertLog = false;
        public bool SaveDivertPackets = false;
        public bool BlockGMinerApacheTomcat = false;
        public bool ShowUptime = true;
        public bool ProgramMonitoring = true;
        public bool DebugConsole = false;
        public bool NewPlatform = true;
        public string BitcoinAddress = "";
        public string BitcoinAddressNew = "";
        public string WorkerName = "worker1";
        public string StratumSuff = "";
        public TimeUnitType TimeUnit = TimeUnitType.Day;
        public string IFTTTKey = "";
        public int ServiceLocation = 0;
        public bool AutoStartMining = false;
        public int AutoStartMiningDelay = 0;
        public bool HideMiningWindows = false;
        public bool MinimizeToTray = false;
        public bool AlwaysOnTop = false;
        public bool Lyra2z = false;
        public bool ShowFanAsPercent = false;
        public bool MOPA1 = true;
        public bool MOPA2 = false;
        public bool MOPA3 = false;
        public bool MOPA4 = false;
        public bool MOPA5 = false;
        public int ColumnENABLED = 304;
        public int ColumnTEMP = 80;
        public int ColumnLOAD = 66;
        public int ColumnFAN = 56;
        public int ColumnPOWER = 85;

        public int ColumnListALGORITHM = 110;
        public int ColumnListMINER = 82;
        public int ColumnListSPEED = 152;
        public int ColumnListPOWER = 88;
        public int ColumnListRATIO = 90;
        public int ColumnListRATE = 148;
        public bool ColumnSort = false;
        public int ColumnListSort = 1;

        public int FormWidth = 700;
        public int FormHeight = 389;
        public int FormTop = 0;
        public int FormLeft = 0;
        public int BenchmarkFormWidth = 700;
        public int BenchmarkFormHeight = 550;
        public int BenchmarkFormTop = 0;
        public int BenchmarkFormLeft = 0;
        public int SettingsFormWidth = 700;
        public int SettingsFormHeight = 616;
        public int SettingsFormTop = 0;
        public int SettingsFormLeft = 0;

        public bool MinimizeMiningWindows = false;
        public bool ResetProtocols = true;

        //public int LessThreads;
        public CpuExtensionType ForceCPUExtension = CpuExtensionType.Automatic;

        [Obsolete("Use SwitchSmaTimeChangeSeconds")]
        public int SwitchMinSecondsFixed = 90;
        [Obsolete("Use SwitchSmaTimeChangeSeconds")]
        public int SwitchMinSecondsDynamic = 30;
        [Obsolete("Use SwitchSmaTimeChangeSeconds")]
        public int SwitchMinSecondsAMD = 60;
        public double SwitchProfitabilityThreshold = 0.05; // percent
        public int MinerAPIQueryInterval = 5;
        public int MinerRestartDelayMS = 500;

        public BenchmarkTimeLimitsConfig BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();

        // TODO deprecate this
        public DeviceDetectionConfig DeviceDetection = new DeviceDetectionConfig();

        public bool DisableAMDTempControl = false;
        public bool DisableDefaultOptimizations = false;

        public bool AutoScaleBTCValues = true;
        public bool StartMiningWhenIdle = false;

        public int MinIdleSeconds = 60;
        public bool LogToFile = true;

        // in bytes
        public long LogMaxFileSize = 16777216;

        public bool ShowDriverVersionWarning = false;
        public bool DisableWindowsErrorReporting = true;
        public bool ShowInternetConnectionWarning = true;
        public bool NVIDIAP0State = false;

        public int ethminerDefaultBlockHeight = 2000000;
        public DagGenerationType EthminerDagGenerationType = DagGenerationType.SingleKeep;
        public int ApiBindPortPoolStart = 5100;
        public double MinimumProfit = 0;
        public bool IdleWhenNoInternetAccess = true;
        public bool UseIFTTT = false;
        public bool DownloadInit = false;

        public bool RestartDriverOnCUDA_GPU_Lost = false;
        public bool RestartWindowsOnCUDA_GPU_Lost = false;
        public bool Allow_remote_management = true;
        public bool Send_actual_version_info = false;
        public bool Force_mining_if_nonprofitable = true;
        public bool Additional_info_about_device = false;
        public bool Use_OpenHardwareMonitor = true;
        public bool Disable_extra_launch_parameter_checking = false;
        public bool Save_windows_size_and_position = true;
        public bool Group_same_devices = true;
        public string MachineGuid = "";
        public string CpuID = "";
        public bool DisableMonitoringCPU = false;
        public bool DisableMonitoringAMD = false;
        public bool DisableMonitoringNVIDIA = false;


        // 3rd party miners
        public Use3rdPartyMiners Use3rdPartyMiners = Use3rdPartyMiners.YES;

        public bool DownloadInit3rdParty = false;

        public bool AllowMultipleInstances = true;

        // device enabled disabled stuff
        public List<ComputeDeviceConfig> LastDevicesSettup = new List<ComputeDeviceConfig>();

        //
        public string hwid = "";

        public int agreedWithTOS = 0;

        // normalization stuff
        [Obsolete]
        public double IQROverFactor = 3.0;
        [Obsolete]
        public int NormalizedProfitHistory = 15;
        [Obsolete]
        public double IQRNormalizeFactor = 0.0;

        public bool CoolDownCheckEnabled = true;

        // Set to skip driver checks to enable Neoscrypt/Lyra2RE on AMD
        public bool ForceSkipAMDNeoscryptLyraCheck = false;

        // Overriding AMDOpenCLDeviceDetection returned Bus IDs (in case of driver error, e.g. 17.12.1)
        public string OverrideAMDBusIds = "";

        public Interval SwitchSmaTimeChangeSeconds = new Interval(34, 55);
        public Interval SwitchSmaTicksStable = new Interval(2, 3);
        public Interval SwitchSmaTicksUnstable = new Interval(5, 13);

        /// <summary>
        /// Cost of electricity in kW-h
        /// </summary>
        public double KwhPrice = 0;
        public int PowerPSU = 80;
        public int PowerMB = 60;

        /// <summary>
        /// True if NHML should try to cache SMA values for next launch
        /// </summary>
        public bool UseSmaCache = true;

        public int ColorProfileIndex = 0;
        public int SwitchingAlgorithmsIndex = 2;
        public int DevicesCountIndex = 1;
        public bool ProgramAutoUpdate = true;
        public bool BackupBeforeUpdate = true;
        public int ProgramUpdateIndex = 1;
        public int ProgramRestartIndex = 0;
        public bool PeriodicalReconnect = true;
        public ColorProfilesConfig ColorProfiles = new ColorProfilesConfig();
        public bool UseEthlargement = false;

        // methods
        public void SetDefaults()
        {
            ConfigFileVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Language = LanguageType.En;
            ForceCPUExtension = CpuExtensionType.Automatic;
            BitcoinAddress = "";
            WorkerName = "worker1";
            TimeUnit = TimeUnitType.Day;
            ServiceLocation = 0;
            ShowUptime = true;
            ProgramMonitoring = true;
            AutoStartMining = false;
            AutoStartMiningDelay = 0;
            //LessThreads = 0;
            DivertRun = true;
            DivertLog = false;
            SaveDivertPackets = false;
            BlockGMinerApacheTomcat = false;
            DebugConsole = false;
            HideMiningWindows = false;
            Lyra2z = false;
            MinimizeToTray = false;
            AlwaysOnTop = false;
            BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
            DeviceDetection = new DeviceDetectionConfig();
            DisableAMDTempControl = false;
            DisableDefaultOptimizations = false;
            AutoScaleBTCValues = true;
            StartMiningWhenIdle = false;
            LogToFile = true;
            LogMaxFileSize = 1048576;
            ShowDriverVersionWarning = false;
            DisableWindowsErrorReporting = true;
            ShowInternetConnectionWarning = true;
            NVIDIAP0State = false;
            MinerRestartDelayMS = 500;
            ethminerDefaultBlockHeight = 2000000;
            SwitchMinSecondsFixed = 90;
            SwitchMinSecondsDynamic = 30;
            SwitchMinSecondsAMD = 90;
            SwitchProfitabilityThreshold = 0.05; // percent
            MinIdleSeconds = 60;
            DisplayCurrency = "USD";
            ApiBindPortPoolStart = 4000;
            MinimumProfit = 0;
            EthminerDagGenerationType = DagGenerationType.SingleKeep;
            DownloadInit = false;
            //ContinueMiningIfNoInternetAccess = false;
            IdleWhenNoInternetAccess = true;
            Use3rdPartyMiners = Use3rdPartyMiners.YES;
            DownloadInit3rdParty = false;
            AllowMultipleInstances = true;
            UseIFTTT = false;
            IQROverFactor = 3.0;
            NormalizedProfitHistory = 15;
            IQRNormalizeFactor = 0.0;
            CoolDownCheckEnabled = true;
            RestartDriverOnCUDA_GPU_Lost = false;
            RestartWindowsOnCUDA_GPU_Lost = false;
            Allow_remote_management = true;
            ForceSkipAMDNeoscryptLyraCheck = false;
            OverrideAMDBusIds = "";
            SwitchSmaTimeChangeSeconds = new Interval(34, 55);
            SwitchSmaTicksStable = new Interval(2, 3);
            SwitchSmaTicksUnstable = new Interval(5, 13);
            UseSmaCache = true;
            UseEthlargement = false;
            ShowFanAsPercent = false;
            MOPA1 = true;
            MOPA2 = false;
            MOPA3 = false;
            MOPA4 = false;
            MOPA5 = false;
            ColumnENABLED = 304;
            ColumnTEMP = 80;
            ColumnLOAD = 66;
            ColumnFAN = 56;
            ColumnPOWER = 85;

            ColumnListALGORITHM = 110;
            ColumnListMINER = 82;
            ColumnListSPEED = 152;
            ColumnListPOWER = 88;
            ColumnListRATIO = 90;
            ColumnListRATE = 148;
            ColumnSort = false;
            ColumnListSort = 1;

            FormWidth = 770;
            FormHeight = 406;
            FormTop = 0;
            FormLeft = 0;
            BenchmarkFormWidth = 700;
            BenchmarkFormHeight = 550;
            BenchmarkFormTop = 0;
            BenchmarkFormLeft = 0;
            SettingsFormWidth = 700;
            SettingsFormHeight = 616;
            SettingsFormTop = 0;
            SettingsFormLeft = 0;
    }

        public void FixSettingBounds()
        {
            ConfigFileVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (string.IsNullOrEmpty(DisplayCurrency)
                || string.IsNullOrWhiteSpace(DisplayCurrency))
            {
                DisplayCurrency = "USD";
            }
            if (SwitchMinSecondsFixed <= 0)
            {
                SwitchMinSecondsFixed = 90;
            }
            if (SwitchMinSecondsDynamic <= 0)
            {
                SwitchMinSecondsDynamic = 30;
            }
            if (SwitchMinSecondsAMD <= 0)
            {
                SwitchMinSecondsAMD = 60;
            }
            if (MinerAPIQueryInterval <= 0)
            {
                MinerAPIQueryInterval = 5;
            }
            if (MinerRestartDelayMS <= 0)
            {
                MinerRestartDelayMS = 500;
            }
            if (MinIdleSeconds <= 0)
            {
                MinIdleSeconds = 60;
            }
            if (LogMaxFileSize <= 0)
            {
                LogMaxFileSize = 1048576;
            }
            // check port start number, leave about 2000 ports pool size, huge yea!
            if (ApiBindPortPoolStart > (65535 - 2000))
            {
                ApiBindPortPoolStart = 5100;
            }
            if (this.ApiBindPortPoolStart <= 4001)  //fix to hsrminer
                {
                    this.ApiBindPortPoolStart = 4002;
                }
            if (BenchmarkTimeLimits == null)
            {
                BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
            }
            if (DeviceDetection == null)
            {
                DeviceDetection = new DeviceDetectionConfig();
            }
            if (LastDevicesSettup == null)
            {
                LastDevicesSettup = new List<ComputeDeviceConfig>();
            }
            if (IQROverFactor < 0)
            {
                IQROverFactor = 3.0;
            }
            if (NormalizedProfitHistory < 0)
            {
                NormalizedProfitHistory = 15;
            }
            if (IQRNormalizeFactor < 0)
            {
                IQRNormalizeFactor = 0.0;
            }
            if (KwhPrice < 0)
            {
                KwhPrice = 0;
            }

            SwitchSmaTimeChangeSeconds.FixRange();
            SwitchSmaTicksStable.FixRange();
            SwitchSmaTicksUnstable.FixRange();
        }

        /*
        [Serializable]
        public class AlgorithmConfig
        {
            public string Name = ""; // Used as an indicator for easier user interaction
            public AlgorithmType NiceHashID = AlgorithmType.NONE;
            public AlgorithmType SecondaryNiceHashID = AlgorithmType.NONE;
            public MinerBaseType MinerBaseType = MinerBaseType.NONE;
            public string MinerName = ""; // probably not needed
            public double BenchmarkSpeed = 0;
            public double SecondaryBenchmarkSpeed = 0;
            public string ExtraLaunchParameters = "";
            public bool Enabled = true;
            public bool Hidden = false;
            public int LessThreads = 0;
            public double PowerUsage = 0;
        }
        */

        [Serializable]
        public class ColorProfilesConfig
        {
            /*
            _backColor;
            _foreColor;
            _windowColor;
            _textColor;
        */
            private static readonly Color[] DefaultColorProfile = { SystemColors.Control, SystemColors.WindowText, SystemColors.Window, SystemColors.ControlText };
            private static readonly Color[] GrayProfile = { SystemColors.ControlDark, SystemColors.WindowText, SystemColors.ControlDark, SystemColors.ControlText };
            private static readonly Color[] DarkProfile = { SystemColors.ControlDarkDark, Color.White, SystemColors.ControlDarkDark, Color.White };
            private static readonly Color[] BlackProfile = { Color.Black, Color.White, Color.Black, Color.White };
            private static readonly Color[] SilverProfile = { Color.Silver, Color.Black, Color.Silver, Color.Black };
            private static readonly Color[] GoldProfile = { Color.DarkGoldenrod, Color.White, Color.DarkGoldenrod, Color.White };
            private static readonly Color[] DarkRedProfile = { Color.DarkRed, Color.White, Color.DarkRed, Color.White };
            private static readonly Color[] DarkGreenProfile = { Color.DarkGreen, Color.White, Color.DarkGreen, Color.White };
            private static readonly Color[] DarkBlueProfile = { Color.DarkBlue, Color.White, Color.DarkBlue, Color.White };
            private static readonly Color[] DarkMagentaProfile = { Color.DarkMagenta, Color.White, Color.DarkMagenta, Color.White };
            private static readonly Color[] DarkOrangeProfile = { Color.DarkOrange, Color.White, Color.DarkOrange, Color.White };
            private static readonly Color[] DarkVioletProfile = { Color.DarkViolet, Color.White, Color.DarkViolet, Color.White };
            private static readonly Color[] DarkSlateBlueProfile = { Color.DarkSlateBlue, Color.White, Color.DarkSlateBlue, Color.White };
            private static readonly Color[] TanProfile = { Color.Tan, Color.Black, Color.Tan, Color.Black };

            private Color[] _DefaultColorProfile = MemoryHelper.DeepClone(DefaultColorProfile);
            private Color[] _GrayColorProfile = MemoryHelper.DeepClone(GrayProfile);
            private Color[] _DarkColorProfile = MemoryHelper.DeepClone(DarkProfile);
            private Color[] _BlackColorProfile = MemoryHelper.DeepClone(BlackProfile);
            private Color[] _SilverColorProfile = MemoryHelper.DeepClone(SilverProfile);
            private Color[] _GoldColorProfile = MemoryHelper.DeepClone(GoldProfile);
            private Color[] _DarkRedProfile = MemoryHelper.DeepClone(DarkRedProfile);
            private Color[] _DarkGreenProfile = MemoryHelper.DeepClone(DarkGreenProfile);
            private Color[] _DarkBlueProfile = MemoryHelper.DeepClone(DarkBlueProfile);
            private Color[] _DarkMagentaProfile = MemoryHelper.DeepClone(DarkMagentaProfile);
            private Color[] _DarkOrangeProfile = MemoryHelper.DeepClone(DarkOrangeProfile);
            private Color[] _DarkVioletProfile = MemoryHelper.DeepClone(DarkVioletProfile);
            private Color[] _DarkSlateBlueProfile = MemoryHelper.DeepClone(DarkSlateBlueProfile);
            private Color[] _TanProfile = MemoryHelper.DeepClone(TanProfile);

            private static bool IsValid(Color[] value)
            {
                return value != null && value.Length == 4;
            }

            public Color[] DefaultColor
            {
                get => DefaultColorProfile;
                set => _DefaultColorProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DefaultColorProfile);
            }
            public Color[] Gray
            {
                get => GrayProfile;
                set => _GrayColorProfile = MemoryHelper.DeepClone(IsValid(value) ? value : GrayProfile);
            }
            public Color[] Dark
            {
                get => DarkProfile;
                set => _DarkColorProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DarkProfile);
            }
            public Color[] Black
            {
                get => BlackProfile;
                set => _BlackColorProfile = MemoryHelper.DeepClone(IsValid(value) ? value : BlackProfile);
            }
            public Color[] Silver
            {
                get => SilverProfile;
                set => _SilverColorProfile = MemoryHelper.DeepClone(IsValid(value) ? value : SilverProfile);
            }
            public Color[] Gold

            {
                get => GoldProfile;
                set => _GoldColorProfile = MemoryHelper.DeepClone(IsValid(value) ? value : GoldProfile);
            }
            public Color[] DarkRed
            {
                get => DarkRedProfile;
                set => _DarkRedProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DarkRedProfile);
            }
            public Color[] DarkGreen
            {
                get => DarkGreenProfile;
                set => _DarkGreenProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DarkGreenProfile);
            }
            public Color[] DarkBlue
            {
                get => DarkBlueProfile;
                set => _DarkBlueProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DarkBlueProfile);
            }
            public Color[] DarkMagenta
            {
                get => DarkMagentaProfile;
                set => _DarkMagentaProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DarkMagentaProfile);
            }
            public Color[] DarkOrange
            {
                get => DarkOrangeProfile;
                set => _DarkOrangeProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DarkOrangeProfile);
            }
            public Color[] DarkViolet
            {
                get => DarkVioletProfile;
                set => _DarkVioletProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DarkVioletProfile);
            }
            public Color[] DarkSlateBlue
            {
                get => DarkSlateBlueProfile;
                set => _DarkSlateBlueProfile = MemoryHelper.DeepClone(IsValid(value) ? value : DarkSlateBlueProfile);
            }
            public Color[] Tan
            {
                get => TanProfile;
                set => _TanProfile = MemoryHelper.DeepClone(IsValid(value) ? value : TanProfile);
            }
            /*
                        public Color[] GetColorProfile(int col)
                        {
                            return DefaultColorProfile;
                        }
                        */
        }
    }



}
