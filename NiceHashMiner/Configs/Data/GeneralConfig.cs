using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NiceHashMiner.Configs.Data
{
    [Serializable]
    public class GeneralConfig
    {
        public Version ConfigFileVersion;
        public double ForkFixVersion;
        public bool ShowSplash = true;
        public string NHMVersion = "3.0.6.5";
        public bool DecreasePowerCost = false;
        public bool ShowTotalPower = false;
        public bool FiatCurrency = false;
        public bool NoShowApiInLog = true;
        public bool NoForceTRexClose;
        public bool UseNegativeProfit = false;
        public bool ShowHiddenAlgos = false;
        public double DaggerOrderMaxPay = 0;
        public int KawpowLiteMaxEpoch3GB = 220;
        public int KawpowLiteMaxEpoch4GB = 354;
        public int KawpowLiteMaxEpoch5GB = 488;
        public int ZILMaxEpoch = 1;
        public LanguageType Language = LanguageType.En;
        public string DisplayCurrency = "USD";
        public bool Show_displayConected = false;
        public bool Show_wallet_balance = false;
        public bool DivertRun = true;
        public bool ShowUptime = true;
        public bool DisableTooltips = false;
        public bool ProgramMonitoring = true;
        public bool EnableRigRemoteView = false;
        public bool EnableAPI = false;
        public int RigRemoteViewPort = 7007;
        public int RigAPiPort = 7001;
        public string BitcoinAddressNew = "";
        public string WorkerName = "worker1";
        public TimeUnitType TimeUnit = TimeUnitType.Day;

        public int PowerTarif = 0;
        public string[] ZoneSchedule1 = { "00:00", "23:59:59", "0.00" };
        public string[] ZoneSchedule2 = { "07:00", "23:00", "0.00", "23:00", "07:00", "0.00" };
        public string[] ZoneSchedule3 = { "23:00", "07:00", "0.00", "07:00", "09:00", "0.00", "09:00", "17:00", "0.00", "17:00", "20:00", "0.00", "20:00", "23:00", "0.00" };

        public string IFTTTKey = "";
        public int ServiceLocation = 0;
        public bool ForceAutoLocation = true;
        public bool AutoStartMining = false;
        public int AutoStartMiningDelay = 0;
        public bool HideMiningWindows = false;
        public bool MinimizeToTray = false;
        public bool AlwaysOnTop = false;
        public bool ShowFanAsPercent = false;
        public bool ShowToolsFolder = false;
        public bool GetMinersVersions = true;
        public bool InstallRootCerts = true;
        public bool MOPA1 = true;
        public bool MOPA2 = false;
        public bool MOPA3 = false;
        public bool MOPA4 = false;
        public bool MOPA5 = false;
        public int ColumnENABLED = 304;
        public int ColumnHASHRATE = 84;
        public int ColumnTEMP = 80;
        public int ColumnLOAD = 66;
        public int ColumnFAN = 56;
        public int ColumnPOWER = 85;

        public int ColumnListALGORITHM = 106;
        public int ColumnListMINER = 82;
        public int ColumnListSPEED = 152;
        public int ColumnListPOWER = 88;
        public int ColumnListRATIO = 90;
        public int ColumnListRATE = 148;
        public int ColumnListGPU_clock = 64;
        public int ColumnListMem_clock = 68;
        public int ColumnListGPU_voltage = 74;
        public int ColumnListMem_voltage = 74;
        public int ColumnListPowerLimit = 66;
        public int ColumnListFan = 44;
        public int ColumnListThermalLimit = 40;
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
        public int ProfitFormWidth = 700;
        public int ProfitFormHeight = 400;
        public int ProfitFormTop = 0;
        public int ProfitFormLeft = 0;
        public bool StartChartWithProgram = false;
        public bool ChartFiat = false;
        public bool ChartEnable = false;
        public bool ABEnableOverclock = false;
        public bool ABDefaultMiningStopped = false;
        public bool ABDefaultProgramClosing = false;
        public bool ABMinimize = false;
        public bool EnableProxy = true;
        public bool ProxySSL = true;
        public bool ProxyAsFailover = false;
        public bool StaleProxy = false;

        public bool MinimizeMiningWindows = false;
        public bool ShowMinersVersions = false;
        public bool StandartBenchmarkTime = true;

        //public int LessThreads;
        public CpuExtensionType ForceCPUExtension = CpuExtensionType.Automatic;

        [Obsolete("Use SwitchSmaTimeChangeSeconds")]
        public int SwitchMinSecondsFixed = 90;
        [Obsolete("Use SwitchSmaTimeChangeSeconds")]
        public int SwitchMinSecondsDynamic = 30;
        [Obsolete("Use SwitchSmaTimeChangeSeconds")]
        public int SwitchMinSecondsAMD = 60;
        public double SwitchProfitabilityThreshold = 0.05; // percent
        public int MinerRestartDelayMS = 500;

        //        public BenchmarkTimeLimitsConfig BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();

        // TODO deprecate this
        public DeviceDetectionConfig DeviceDetection = new DeviceDetectionConfig();

        public bool DisableAMDTempControl = false;
        public bool DisableDefaultOptimizations = false;

        public bool AutoScaleBTCValues = true;
        public bool StartMiningWhenIdle = false;

        public int MinIdleSeconds = 60;
        public bool LogToFile = true;
        public bool SaveProtocolData = false;
        public bool ShowHistory = true;

        // in bytes
        public long LogMaxFileSize = 16777216;

        public bool ShowDriverVersionWarning = true;
        public bool DisableWindowsErrorReporting = true;
        public bool ShowInternetConnectionWarning = true;
        public bool NVIDIAP0State = false;

        public int ethminerDefaultBlockHeight = 2000000;
        public DagGenerationType EthminerDagGenerationType = DagGenerationType.SingleKeep;
        public int ApiBindPortPoolStart = 5100;
        public double MinimumProfit = 0;
        public bool IdleWhenNoInternetAccess = false;
        public bool UseIFTTT = false;
        public bool DownloadInit = false;

        public bool QM_mode = false;
        public int NHMWSProtocolVersion = 4;
        public bool EnableAPIkeys = false;
        public bool CheckingCUDA = false;
        public bool RestartDriverOnCUDA_GPU_Lost = true;
        public bool RestartWindowsOnCUDA_GPU_Lost = false;
        public bool Allow_remote_management = true;
        public bool Send_actual_version_info = true;
        public bool ShowPowerOfDisabledDevices = true;
        public bool Force_mining_if_nonprofitable = true;
        public bool Additional_info_about_device = false;
        public bool Show_NVdevice_manufacturer = true;
        public bool Show_NVIDIA_LHR = true;
        public bool Use_orders_price = false;
        public bool Use_Last24hours = true;
        public bool ShortTerm = false;
        public bool NicehashMiningFee = true;

        public bool Show_memory_temperature = true;
        public bool Show_AMDdevice_manufacturer = true;
        public bool Show_INTELdevice_manufacturer = true;
        public bool Show_ShowDeviceMemSize = true;
        public bool Show_ShowDeviceBusId = false;
        public bool Use_OpenHardwareMonitor = true;
        public bool Disable_extra_launch_parameter_checking = false;
        public bool Hide_unused_algorithms = false;
        public bool Zilliqua_GMiner = true;
        public bool RestartGMinerAfterZilRound = false;
        public bool ZIL_Mining_Enable = true;
        public int ZIL_mining_state = 0;
        public string ZIL_mining_pool = "";
        public string ZIL_mining_port = "";
        public string ZIL_mining_wallet = "";
        public ZILConfigGMiner ZILConfigGMiner = new ZILConfigGMiner();
        public ZILConfigSRBMiner ZILConfigSRBMiner = new ZILConfigSRBMiner();
        public ZILConfigNanominer ZILConfigNanominer = new ZILConfigNanominer();
        public ZILConfigRigel ZILConfigRigel = new ZILConfigRigel();
        public ZILConfigminiZ ZILConfigminiZ = new ZILConfigminiZ();
        public int KAWPOW_Rigel_Max_Rejects = 5;
        public bool AdditionalMiningPlusSymbol = true;
        public bool Save_windows_size_and_position = true;
        public bool Group_same_devices = true;
        public bool StrongDeviceName = true;
        public bool with_power = true;
        public bool By_profitability_of_all_devices = true;
        public string MachineGuid = "";
        public string CpuID = "";
        public bool DisableMonitoringCPU = false;
        public bool DisableMonitoringAMD = false;
        public bool DisableMonitoringNVIDIA = false;
        public bool DisableMonitoringINTEL = false;

        public bool DownloadInit3rdParty = false;

        public bool AllowMultipleInstances = false;

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
        public int PowerAddAMD = 0;

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
        public bool ColorizeTables = true;
        public ColorProfilesConfig ColorProfiles = new ColorProfilesConfig();

        // methods
        public void SetDefaults()
        {
            ConfigFileVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Language = LanguageType.En;
            ForceCPUExtension = CpuExtensionType.Automatic;
            WorkerName = "worker1";
            MachineGuid = "";
            TimeUnit = TimeUnitType.Day;
            ServiceLocation = 0;
            ShowUptime = true;
            ProgramMonitoring = true;
            AutoStartMining = false;
            AutoStartMiningDelay = 0;
            Show_displayConected = false;
            //LessThreads = 0;
            DivertRun = true;
            HideMiningWindows = false;
            MinimizeToTray = false;
            AlwaysOnTop = false;
            //            BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
            DeviceDetection = new DeviceDetectionConfig();
            DisableAMDTempControl = false;
            DisableDefaultOptimizations = false;
            AutoScaleBTCValues = true;
            StartMiningWhenIdle = false;
            LogToFile = true;
            LogMaxFileSize = 10485760;
            ShowDriverVersionWarning = true;
            DisableWindowsErrorReporting = true;
            ShowInternetConnectionWarning = true;
            NVIDIAP0State = false;
            MinerRestartDelayMS = 500;
            SwitchProfitabilityThreshold = 0.05; // percent
            MinIdleSeconds = 60;
            DisplayCurrency = "USD";
            ApiBindPortPoolStart = 4000;
            MinimumProfit = 0;
            DownloadInit = false;
            EnableProxy = true;
            ProxyAsFailover = false;
            ProxySSL = true;
            IdleWhenNoInternetAccess = false;
            DownloadInit3rdParty = false;
            AllowMultipleInstances = false;
            UseIFTTT = false;
            CoolDownCheckEnabled = true;
            CheckingCUDA = false;
            RestartDriverOnCUDA_GPU_Lost = false;
            RestartWindowsOnCUDA_GPU_Lost = false;
            Allow_remote_management = true;
            ForceSkipAMDNeoscryptLyraCheck = false;
            OverrideAMDBusIds = "";
            SwitchSmaTimeChangeSeconds = new Interval(34, 55);
            SwitchSmaTicksStable = new Interval(2, 3);
            SwitchSmaTicksUnstable = new Interval(5, 13);
            UseSmaCache = true;
            ShowFanAsPercent = false;
            MOPA1 = true;
            MOPA2 = false;
            MOPA3 = false;
            MOPA4 = false;
            MOPA5 = false;
            ColumnENABLED = 304;
            ColumnHASHRATE = 84;
            ColumnTEMP = 80;
            ColumnLOAD = 66;
            ColumnFAN = 56;
            ColumnPOWER = 85;

            ColumnListALGORITHM = 106;
            ColumnListMINER = 82;
            ColumnListSPEED = 152;
            ColumnListPOWER = 88;
            ColumnListRATIO = 90;
            ColumnListRATE = 148;
            ColumnListGPU_clock = 64;
            ColumnListMem_clock = 68;
            ColumnListGPU_voltage = 74;
            ColumnListMem_voltage = 74;
            ColumnListPowerLimit = 66;
            ColumnListFan = 44;
            ColumnListThermalLimit = 40;
            ColumnSort = false;
            ColumnListSort = 1;

            FormWidth = 780;
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
            ProfitFormWidth = 700;
            ProfitFormHeight = 400;
            ProfitFormTop = 0;
            ProfitFormLeft = 0;
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
                LogMaxFileSize = 10485760;
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
            /*
                        if (BenchmarkTimeLimits == null)
                        {
                            BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                        }
            */
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
