/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Forms;
using NiceHashMiner.Forms.Components;
using NiceHashMiner.Interfaces;
using NiceHashMiner.Miners;
using NiceHashMiner.Utils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using NiceHashMiner.Stats;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using SystemTimer = System.Timers.Timer;
using Timer = System.Windows.Forms.Timer;

namespace NiceHashMiner
{
    using System.IO;
    using System.Runtime.InteropServices;

    public partial class Form_Main : Form, Form_Loading.IAfterInitializationCaller, IMainFormRatesComunication
    {
        private string _visitUrlNew = Links.VisitUrlNew;

        public Timer _minerStatsCheck;
        //private Timer _smaMinerCheck;
        //private Timer _bitcoinExchangeCheck;
        private Timer _startupTimer;
        private Timer _remoteTimer;
        private Timer _autostartTimer;
        private Timer _autostartTimerDelay;
        private Timer _deviceStatusTimer;
        private int _AutoStartMiningDelay = 0;
        private Timer _idleCheck;
        private SystemTimer _computeDevicesCheckTimer;
        public static bool needRestart = false;

        private bool _showWarningNiceHashData;
        private bool _demoMode;

        private readonly Random R;

        private Form_Loading _loadingScreen;
        private Form_Benchmark _benchmarkForm;

        private int _flowLayoutPanelVisibleCount = 0;
        private int _flowLayoutPanelRatesIndex = 0;

        private const string BetaAlphaPostfixString = "";
        const string ForkString = " Fork Fix 23 beta 1";

        private bool _isDeviceDetectionInitialized = false;

        private bool _isManuallyStarted = false;
        private bool _isNotProfitable = false;

        //private bool _isSmaUpdated = false;

        private double _factorTimeUnit = 1.0;

        private readonly int _mainFormHeight = 0;
        private readonly int _emtpyGroupPanelHeight = 0;
        bool firstStartConnection = false;
        private bool firstRun = false;
        public static Color _backColor;
        public static Color _foreColor;
        public static Color _windowColor;
        public static Color _textColor;
        //public static string[,] myServers = { { Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], "20000" }, { "usa", "20001" }, { "hk", "20002" }, { "jp", "20003" }, { "in", "20004" }, { "br", "20005" } };
        public static string[,] myServers = {
            { "eu", "20000" }, { "usa", "20001" }, { "hk", "20002" }, { "jp", "20003" }, { "in", "20004" }, { "br", "20005" }
            //{ Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], "20000" }, { "usa", "20001" }, { "hk", "20002" }, { "jp", "20003" }, { "in", "20004" }, { "br", "20005" }
        };//need to reread config data?

        public Form_Main()
        {
            switch (ConfigManager.GeneralConfig.ColorProfileIndex)
            {
                case 0: //default
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DefaultColor[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DefaultColor[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DefaultColor[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DefaultColor[3];
                     break;
                case 1: //gray
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.Gray[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.Gray[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.Gray[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.Gray[3];
                    break;
                case 2: //dark
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.Dark[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.Dark[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.Dark[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.Dark[3];
                    break;
                case 3: //black
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.Black[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.Black[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.Black[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.Black[3];
                    break;
                case 4: //silver
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.Silver[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.Silver[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.Silver[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.Silver[3];
                    break;
                case 5: //gold
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.Gold[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.Gold[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.Gold[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.Gold[3];
                    break;
                case 6: //darkred
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DarkRed[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DarkRed[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DarkRed[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DarkRed[3];
                    break;
                case 7: //darkgreen
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DarkGreen[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DarkGreen[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DarkGreen[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DarkGreen[3];
                    break;
                case 8: //darkblue
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DarkBlue[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DarkBlue[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DarkBlue[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DarkBlue[3];
                    break;
                case 9: //magenta
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DarkMagenta[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DarkMagenta[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DarkMagenta[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DarkMagenta[3];
                    break;
                case 10: //orange
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DarkOrange[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DarkOrange[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DarkOrange[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DarkOrange[3];
                    break;
                case 11: //violet
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DarkViolet[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DarkViolet[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DarkViolet[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DarkViolet[3];
                    break;
                case 12: //darkslateblue
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DarkSlateBlue[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DarkSlateBlue[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DarkSlateBlue[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DarkSlateBlue[3];
                    break;
                case 13: //tan
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.Tan[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.Tan[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.Tan[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.Tan[3];
                    break;
                default:
                    _backColor = ConfigManager.GeneralConfig.ColorProfiles.DefaultColor[0];
                    _foreColor = ConfigManager.GeneralConfig.ColorProfiles.DefaultColor[1];
                    _windowColor = ConfigManager.GeneralConfig.ColorProfiles.DefaultColor[2];
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DefaultColor[3];
                    break;
            }
            InitializeComponent();
            Icon = Properties.Resources.logo;

            InitLocalization();
            devicesListViewEnableControl1.Visible = false;
            ComputeDeviceManager.SystemSpecs.QueryAndLog();

            comboBoxLocation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBoxLocation.DrawItem += new DrawItemEventHandler(comboBoxLocation_DrawItem);
            // Log the computer's amount of Total RAM and Page File Size
            var moc = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem").Get();
            foreach (ManagementObject mo in moc)
            {
                var totalRam = long.Parse(mo["TotalVisibleMemorySize"].ToString()) / 1024;
                var pageFileSize = (long.Parse(mo["TotalVirtualMemorySize"].ToString()) / 1024) - totalRam;
                Helpers.ConsolePrint("NICEHASH", "Total RAM: " + totalRam + "MB");
                Helpers.ConsolePrint("NICEHASH", "Page File Size: " + pageFileSize + "MB");
            }

            R = new Random((int)DateTime.Now.Ticks);

            //            Text += " v" + Application.ProductVersion + BetaAlphaPostfixString;
            /*
            var cPlatform = "";
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                cPlatform = " (для старой платформы NiceHash)";
            }
            else
            {
                cPlatform = " (for old NiceHash platform)";
            }
            */


            Text += ForkString;

            label_NotProfitable.Visible = false;

            InitMainConfigGuiData();

            // for resizing
            InitFlowPanelStart();

            if (groupBox1.Size.Height > 0 && Size.Height > 0)
            {
                _emtpyGroupPanelHeight = groupBox1.Size.Height;
                _mainFormHeight = Size.Height - _emtpyGroupPanelHeight;
            }
            else
            {
                _emtpyGroupPanelHeight = 59;
                _mainFormHeight = 330 - _emtpyGroupPanelHeight;
            }
            ClearRatesAll();

        }

        private void InitLocalization()
        {
            MessageBoxManager.Unregister();
            MessageBoxManager.Yes = International.GetText("Global_Yes");
            MessageBoxManager.No = International.GetText("Global_No");
            MessageBoxManager.OK = International.GetText("Global_OK");
            MessageBoxManager.Cancel = International.GetText("Global_Cancel");
            MessageBoxManager.Retry = International.GetText("Global_Retry");
            MessageBoxManager.Register();

            labelServiceLocation.Text = International.GetText("Service_Location") + ":";
            {
                var i = 0;
                foreach (var loc in Globals.MiningLocation)
                {
                    if (i != 6)
                    {
                        comboBoxLocation.Items[i++] = International.GetText("LocationName_" + loc);
                    }
                    else
                    {
                        comboBoxLocation.Items[i++] = "Auto";
                    }
                }
            }

            labelBitcoinAddressNew.Text = International.GetText("BitcoinAddress") + ":";
            labelWorkerName.Text = International.GetText("WorkerName") + ":";
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
              //  labelBitcoinAddress.Text = "Биткоин адрес (старая платформа)" + ":";
                labelBitcoinAddressNew.Text = "Биткоин адрес" + ":";
                labelWorkerName.Text = "Имя компьютера" + ":";
            }


            linkLabelCheckStats.Text = International.GetText("Form_Main_check_stats");
            linkLabelChooseBTCWallet.Text = International.GetText("Form_Main_choose_bitcoin_wallet");

           // toolStripStatusLabelGlobalRateText.Text = International.GetText("Form_Main_global_rate").Substring(0, 2) + ":";
            toolStripStatusLabelGlobalRateText.Text = International.GetText("Form_Main_global_rate");
            toolStripStatusLabelBTCDayText.Text =
                "BTC/" + International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
            toolStripStatusLabelBalanceText.Text = (ExchangeRateApi.ActiveDisplayCurrency + "/") +
                                                   International.GetText(
                                                       ConfigManager.GeneralConfig.TimeUnit.ToString()) + "     " +
                                                   International.GetText("Form_Main_balance") + ":";

            devicesListViewEnableControl1.InitLocaleMain();
          //  devicesListViewEnableControl1.Focus();

            buttonBenchmark.Text = International.GetText("Form_Main_benchmark");
            buttonSettings.Text = International.GetText("Form_Main_settings");
            buttonStartMining.Text = International.GetText("Form_Main_start");
            buttonStopMining.Text = International.GetText("Form_Main_stop");
            buttonHelp.Text = International.GetText("Form_Main_help");

            label_NotProfitable.Text = International.GetText("Form_Main_MINING_NOT_PROFITABLE");
            groupBox1.Text = International.GetText("Form_Main_Group_Device_Rates");
        }

        public void InitMainConfigGuiData()
        {
            if (ConfigManager.GeneralConfig.ServiceLocation >= 0 &&
                //ConfigManager.GeneralConfig.ServiceLocation < Globals.MiningLocation.Length)
                ConfigManager.GeneralConfig.ServiceLocation < 6)
                comboBoxLocation.SelectedIndex = ConfigManager.GeneralConfig.ServiceLocation;
            else
                comboBoxLocation.SelectedIndex = 6;

            //textBoxBTCAddress.Text = ConfigManager.GeneralConfig.BitcoinAddress;
            textBoxBTCAddress_new.Text = ConfigManager.GeneralConfig.BitcoinAddressNew;
            textBoxWorkerName.Text = ConfigManager.GeneralConfig.WorkerName;

            //radioButtonNewPlatform.Checked = ConfigManager.GeneralConfig.NewPlatform;
            //radioButtonOldPlatform.Checked = !ConfigManager.GeneralConfig.NewPlatform;

            _showWarningNiceHashData = true;
            _demoMode = false;

            // init active display currency after config load
            ExchangeRateApi.ActiveDisplayCurrency = ConfigManager.GeneralConfig.DisplayCurrency;

            // init factor for Time Unit
            switch (ConfigManager.GeneralConfig.TimeUnit)
            {
                case TimeUnitType.Hour:
                    _factorTimeUnit = 1.0 / 24.0;
                    break;
                case TimeUnitType.Day:
                    _factorTimeUnit = 1;
                    break;
                case TimeUnitType.Week:
                    _factorTimeUnit = 7;
                    break;
                case TimeUnitType.Month:
                    _factorTimeUnit = 30;
                    break;
                case TimeUnitType.Year:
                    _factorTimeUnit = 365;
                    break;
            }

            toolStripStatusLabelBalanceDollarValue.Text = "(" + ExchangeRateApi.ActiveDisplayCurrency + ")";
            toolStripStatusLabelBalanceText.Text = (ExchangeRateApi.ActiveDisplayCurrency + "/") +
                                                   International.GetText(
                                                       ConfigManager.GeneralConfig.TimeUnit.ToString()) + "     " +
                                                   International.GetText("Form_Main_balance") + ":";
            BalanceCallback(null, null); // update currency changes

            if (_isDeviceDetectionInitialized)
            {
                devicesListViewEnableControl1.ResetComputeDevices(ComputeDeviceManager.Available.Devices);
            }
        }

        public void AfterLoadComplete()
        {



            _loadingScreen = null;
            Enabled = true;

            _idleCheck = new Timer();
            _idleCheck.Tick += IdleCheck_Tick;
            _idleCheck.Interval = 500;
            _idleCheck.Start();
            devicesListViewEnableControl1.Visible = true;
        }


        private void IdleCheck_Tick(object sender, EventArgs e)
        {
            if (!ConfigManager.GeneralConfig.StartMiningWhenIdle || _isManuallyStarted) return;

            var msIdle = Helpers.GetIdleTime();

            if (_minerStatsCheck.Enabled)
            {
                if (msIdle < (ConfigManager.GeneralConfig.MinIdleSeconds * 1000))
                {
                    StopMining();
                    Helpers.ConsolePrint("NICEHASH", "Resumed from idling");
                }
            }
            else
            {
                if (_benchmarkForm == null && (msIdle > (ConfigManager.GeneralConfig.MinIdleSeconds * 1000)))
                {
                    Helpers.ConsolePrint("NICEHASH", "Entering idling state");
                    if (StartMining(false) != StartMiningReturnType.StartMining)
                    {
                        StopMining();
                    }
                }
            }
        }


        // This is a single shot _benchmarkTimer
        private void StartupTimer_Tick(object sender, EventArgs e)
        {
            if (!ConfigManager.GeneralConfig.AutoStartMining)
            {
                buttonStopMining.Enabled = false;
            } else
            {
                buttonStopMining.Text = buttonStopMining.Text + "...";
            }


            _startupTimer.Stop();
            _startupTimer = null;

            // Internals Init
            // TODO add loading step
            MinersSettingsManager.Init();

            if (!Helpers.Is45NetOrHigher())
            {
                MessageBox.Show(International.GetText("NET45_Not_Installed_msg"),
                    International.GetText("Warning_with_Exclamation"),
                    MessageBoxButtons.OK);

                Close();
                return;
            }

            if (!Helpers.Is64BitOperatingSystem)
            {
                MessageBox.Show(International.GetText("Form_Main_x64_Support_Only"),
                    International.GetText("Warning_with_Exclamation"),
                    MessageBoxButtons.OK);

                Close();
                return;
            }

            // 3rdparty miners check scope #1
            {
                // check if setting set
                if (ConfigManager.GeneralConfig.Use3rdPartyMiners == Use3rdPartyMiners.NOT_SET)
                {
                    // Show TOS
                    Form tos = new Form_3rdParty_TOS();
                    tos.ShowDialog(this);
                }
            }
            // Query Available ComputeDevices
            Thread.Sleep(100);
            ComputeDeviceManager.Query.QueryDevices(_loadingScreen);
            Thread.Sleep(200);
            _isDeviceDetectionInitialized = true;

            /////////////////////////////////////////////
            /////// from here on we have our devices and Miners initialized
            ConfigManager.AfterDeviceQueryInitialization();
            _loadingScreen.IncreaseLoadCounterAndMessage(International.GetText("Form_Main_loadtext_SaveConfig"));
            Thread.Sleep(200);
            // All devices settup should be initialized in AllDevices
            devicesListViewEnableControl1.ResetComputeDevices(ComputeDeviceManager.Available.Devices);
            // set properties after
            devicesListViewEnableControl1.SaveToGeneralConfig = true;


            _minerStatsCheck = new Timer();
            _minerStatsCheck.Tick += MinerStatsCheck_Tick;
            _minerStatsCheck.Interval = ConfigManager.GeneralConfig.MinerAPIQueryInterval * 1000;
            //buttonStopMining.Enabled = false;
            //_smaMinerCheck = new Timer();
            //_smaMinerCheck.Tick += SMAMinerCheck_Tick;
            //_smaMinerCheck.Interval = ConfigManager.GeneralConfig.SwitchMinSecondsFixed * 1000 +
            //                          R.Next(ConfigManager.GeneralConfig.SwitchMinSecondsDynamic * 1000);
            //if (ComputeDeviceManager.Group.ContainsAmdGpus)
            //{
            //    _smaMinerCheck.Interval =
            //        (ConfigManager.GeneralConfig.SwitchMinSecondsAMD +
            //         ConfigManager.GeneralConfig.SwitchMinSecondsFixed) * 1000 +
            //        R.Next(ConfigManager.GeneralConfig.SwitchMinSecondsDynamic * 1000);
            //}


            _loadingScreen.IncreaseLoadCounterAndMessage(
                International.GetText("Form_Main_loadtext_SetEnvironmentVariable"));
            Helpers.SetDefaultEnvironmentVariables();
            Thread.Sleep(200);
            _loadingScreen.IncreaseLoadCounterAndMessage(
                International.GetText("Form_Main_loadtext_SetWindowsErrorReporting"));

            Thread.Sleep(10);
            Helpers.DisableWindowsErrorReporting(ConfigManager.GeneralConfig.DisableWindowsErrorReporting);

            _loadingScreen.IncreaseLoadCounter();
            if (ConfigManager.GeneralConfig.NVIDIAP0State)
            {
                _loadingScreen.SetInfoMsg(International.GetText("Form_Main_loadtext_NVIDIAP0State"));
                Helpers.SetNvidiaP0State();
            }
            Thread.Sleep(100);
            _loadingScreen.IncreaseLoadCounterAndMessage(International.GetText("Form_Main_loadtext_GetNiceHashSMA"));
            // Init ws connection
            NiceHashStats.OnBalanceUpdate += BalanceCallback;
            NiceHashStats.OnSmaUpdate += SmaCallback;
            NiceHashStats.OnVersionUpdate += VersionUpdateCallback;
            NiceHashStats.OnConnectionLost += ConnectionLostCallback;
            NiceHashStats.OnConnectionEstablished += ConnectionEstablishedCallback;
            NiceHashStats.OnVersionBurn += VersionBurnCallback;
            NiceHashStats.OnExchangeUpdate += ExchangeCallback;
            Thread.Sleep(50);
            /*
            if (Configs.ConfigManager.GeneralConfig.NewPlatform)
            {
                NiceHashStats.StartConnection(Links.NhmSocketAddress);
                //NiceHashStats.DeviceStatus_TickNew("PENDING");
            }
            else
            */
            {
                NiceHashStats.StartConnection(Links.NhmSocketAddress);
            }

            // increase timeout
            if (Globals.IsFirstNetworkCheckTimeout)
            {
                //while (!Helpers.WebRequestTestGoogle() && Globals.FirstNetworkCheckTimeoutTries > 0)
                while (Globals.FirstNetworkCheckTimeoutTries > 0)
                {
                    --Globals.FirstNetworkCheckTimeoutTries;
                }
            }

            _loadingScreen.IncreaseLoadCounterAndMessage(
    International.GetText("Form_Main_loadtext_CheckLatestVersion"));
            Thread.Sleep(200);

            string ghv = NiceHashStats.GetVersion("");
            Helpers.ConsolePrint("GITHUB", ghv);
            if (ghv != null)
            {
                NiceHashStats.SetVersion(ghv);
            }

            _loadingScreen.IncreaseLoadCounterAndMessage(International.GetText("Form_Main_loadtext_GetBTCRate"));
            Thread.Sleep(10);
            //// Don't start timer if socket is giving data
            //if (ExchangeRateApi.ExchangesFiat == null)
            //{
            //    // Wait a bit and check again
            //    Thread.Sleep(1000);
            //    if (ExchangeRateApi.ExchangesFiat == null)
            //    {
            //        Helpers.ConsolePrint("NICEHASH", "No exchange from socket yet, getting manually");
            //        _bitcoinExchangeCheck = new Timer();
            //        _bitcoinExchangeCheck.Tick += BitcoinExchangeCheck_Tick;
            //        _bitcoinExchangeCheck.Interval = 1000 * 3601; // every 1 hour and 1 second
            //        _bitcoinExchangeCheck.Start();
            //        BitcoinExchangeCheck_Tick(null, null);
            //    }
            //}

            _loadingScreen.FinishLoad();

            firstStartConnection = true;
            var runVCRed = !MinersExistanceChecker.IsMinersBinsInit() && !ConfigManager.GeneralConfig.DownloadInit;
            // standard miners check scope
            {
                // check if download needed
                if (!MinersExistanceChecker.IsMinersBinsInit() && !ConfigManager.GeneralConfig.DownloadInit)
                {
                    var downloadUnzipForm =
                        new Form_Loading(new MinersDownloader(MinersDownloadManager.StandardDlSetup));
                    SetChildFormCenter(downloadUnzipForm);
                    downloadUnzipForm.ShowDialog();
                }
                // check if files are mising
                if (!MinersExistanceChecker.IsMinersBinsInit())
                {
                    var result = MessageBox.Show(International.GetText("Form_Main_bins_folder_files_missing"),
                        International.GetText("Warning_with_Exclamation"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        ConfigManager.GeneralConfig.DownloadInit = false;
                        ConfigManager.GeneralConfigFileCommit();
                        var pHandle = new Process
                        {
                            StartInfo =
                            {
                                FileName = Application.ExecutablePath
                            }
                        };
                        pHandle.Start();
                        Close();
                        return;
                    }
                }
                else if (!ConfigManager.GeneralConfig.DownloadInit)
                {
                    // all good
                    ConfigManager.GeneralConfig.DownloadInit = true;
                    ConfigManager.GeneralConfigFileCommit();
                }
            }
            // 3rdparty miners check scope #2
            {
                // check if download needed
                if (ConfigManager.GeneralConfig.Use3rdPartyMiners == Use3rdPartyMiners.YES)
                {
                    if (!MinersExistanceChecker.IsMiners3rdPartyBinsInit() &&
                        !ConfigManager.GeneralConfig.DownloadInit3rdParty)
                    {
                        var download3rdPartyUnzipForm =
                            new Form_Loading(new MinersDownloader(MinersDownloadManager.ThirdPartyDlSetup));
                        SetChildFormCenter(download3rdPartyUnzipForm);
                        download3rdPartyUnzipForm.ShowDialog();
                    }
                    // check if files are mising
                    if (!MinersExistanceChecker.IsMiners3rdPartyBinsInit())
                    {
                        var result = MessageBox.Show(International.GetText("Form_Main_bins_folder_files_missing"),
                            International.GetText("Warning_with_Exclamation"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            ConfigManager.GeneralConfig.DownloadInit3rdParty = false;
                            ConfigManager.GeneralConfigFileCommit();
                            var pHandle = new Process
                            {
                                StartInfo =
                                {
                                    FileName = Application.ExecutablePath
                                }
                            };
                            pHandle.Start();
                            Close();
                            return;
                        }
                    }
                    else if (!ConfigManager.GeneralConfig.DownloadInit3rdParty)
                    {
                        // all good
                        ConfigManager.GeneralConfig.DownloadInit3rdParty = true;
                        ConfigManager.GeneralConfigFileCommit();
                    }
                }
            }

            if (runVCRed)
            {
                Helpers.InstallVcRedist();
            }

            _AutoStartMiningDelay = ConfigManager.GeneralConfig.AutoStartMiningDelay;
            _autostartTimerDelay = new Timer();
            _autostartTimerDelay.Tick += AutoStartTimer_TickDelay;
            _autostartTimerDelay.Interval = 1000;
            _autostartTimerDelay.Start();

            _autostartTimer = new Timer();
            _autostartTimer.Tick += AutoStartTimer_Tick;
            _autostartTimer.Interval = Math.Max(2000, ConfigManager.GeneralConfig.AutoStartMiningDelay * 1000);
            _autostartTimer.Start();

            }
        private void AutoStartTimer_TickDelay(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.AutoStartMining)
            {
                _AutoStartMiningDelay--;
                if (firstRun || _AutoStartMiningDelay < 1)
                {
                    _autostartTimerDelay.Stop();
                    _autostartTimerDelay = null;
                    buttonStopMining.Text = International.GetText("Form_Main_stop");
                    return;
                }
                else
                {
                    //buttonStartMining.Enabled = false;
                    buttonStopMining.Enabled = true;
                    buttonStopMining.Text = International.GetText("Form_Main_stop") + " (" + _AutoStartMiningDelay.ToString() + ")";
                    buttonStartMining.Update();
                }
            }
            else
            {
                buttonStopMining.Enabled = false;
            }
        }
            private void AutoStartTimer_Tick(object sender, EventArgs e)
        {
            _autostartTimer.Stop();
            _autostartTimer = null;

            if (ConfigManager.GeneralConfig.AutoStartMining)
            {
                if (firstRun)
                {
                    if (_autostartTimerDelay != null)
                    {
                        _autostartTimerDelay.Stop();
                        _autostartTimerDelay = null;
                        buttonStopMining.Text = International.GetText("Form_Main_stop");
                    }
                    return;
                }
                // well this is started manually as we want it to start at runtime
                _isManuallyStarted = true;
                if (StartMining(false) != StartMiningReturnType.StartMining)
                {
                    _isManuallyStarted = false;
                    StopMining();
                }
            }
        }

        private void SetChildFormCenter(Form form)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(Location.X + (Width - form.Width) / 2, Location.Y + (Height - form.Height) / 2);
        }

        private void Form_Main_Shown(object sender, EventArgs e)
        {

            if (this != null)
            {
                Rectangle screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                if (ConfigManager.GeneralConfig.FormLeft + ConfigManager.GeneralConfig.FormWidth <= screenSize.Size.Width)
                {
                    if (ConfigManager.GeneralConfig.FormTop + ConfigManager.GeneralConfig.FormLeft >= 1)
                    {
                        this.Top = ConfigManager.GeneralConfig.FormTop;
                        this.Left = ConfigManager.GeneralConfig.FormLeft;
                    }
                    this.Width = ConfigManager.GeneralConfig.FormWidth;
                } else
                {
                   // this.Width = 660; // min width
                }
            }

            foreach (var lbl in this.Controls.OfType<Button>())
            {
                lbl.ForeColor = _textColor;
                lbl.FlatStyle = FlatStyle.Flat;
                lbl.FlatAppearance.BorderColor = _textColor;
                lbl.FlatAppearance.BorderSize = 1;
            }
            buttonLogo.FlatAppearance.BorderSize = 0;
            devicesListViewEnableControl1.BackColor = SystemColors.ControlLightLight;

            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                if (this != null)
                {
                    this.BackColor = _backColor;
                    this.ForeColor = _foreColor;
                }
                //this.BackColor = Color.DarkSlateGray; //темно сине-серый
                //this.BackColor = SystemColors.HotTrack;
                foreach (var lbl in this.Controls.OfType<Label>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<LinkLabel>()) lbl.LinkColor = Color.LightBlue;

                foreach (var lbl in this.Controls.OfType<GroupBox>()) lbl.BackColor = _backColor;

                foreach (var lbl in this.Controls.OfType<HScrollBar>())
                    lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<ListBox>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<ListControl>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<ListView>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<ListViewItem>())
                {
                    lbl.BackColor = _backColor;
                    lbl.ForeColor = _textColor;
                }
                foreach (var lbl in this.Controls.OfType<StatusBar>())
                    lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<ComboBox>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<ComboBox>()) lbl.ForeColor = _foreColor;

                foreach (var lbl in this.Controls.OfType<GroupBox>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<GroupBox>()) lbl.ForeColor = _textColor;
                // foreach (var lbl in this.Controls.OfType<ComboBox>()) lbl.ForeColor = _foreColor;

                foreach (var lbl in this.Controls.OfType<TextBox>())
                {
                    lbl.BackColor = _backColor;
                    lbl.ForeColor = _foreColor;
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                }
                foreach (var lbl in this.Controls.OfType<StatusStrip>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<StatusStrip>()) lbl.ForeColor = _foreColor;
                foreach (var lbl in this.Controls.OfType<ToolStripStatusLabel>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<ToolStripStatusLabel>()) lbl.ForeColor = _foreColor;

                //toolStripStatusLabel10.Image = NiceHashMiner.Properties.Resources.NHM_Cash_Register_Bitcoin_transparent_white;


                foreach (var lbl in this.Controls.OfType<Button>()) lbl.BackColor = _backColor;
                foreach (var lbl in this.Controls.OfType<Button>())
                {
                    lbl.ForeColor = _textColor;
                    lbl.FlatStyle = FlatStyle.Flat;
                    lbl.FlatAppearance.BorderColor = _textColor;
                    lbl.FlatAppearance.BorderSize = 1;
                }
                this.Enabled = true;
                buttonLogo.FlatAppearance.BorderSize = 0;

                foreach (var lbl in this.Controls.OfType<CheckBox>()) lbl.BackColor = _backColor;
                // DevicesListViewEnableControl.listViewDevices.BackColor = _backColor;
                devicesListViewEnableControl1.BackColor = _backColor;
                devicesListViewEnableControl1.ForeColor = _foreColor;

                //DevicesListViewEnableControl.DefaultDevicesColorSeter.
                //   DevicesListViewEnableControl.DefaultDevicesColorSeter.EnabledColor = _backColor;
                //  devicesListViewEnableControl1.listViewDevices.Items[0].UseItemStyleForSubItems = false;

            }
            this.Update();
            this.Refresh();
            // general loading indicator
            const int totalLoadSteps = 11;
            _loadingScreen = new Form_Loading(this,
                International.GetText("Form_Loading_label_LoadingText"),
                International.GetText("Form_Main_loadtext_CPU"), totalLoadSteps);

            SetChildFormCenter(_loadingScreen);
            _loadingScreen.Show();
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
              //  Form_Loading.ActiveForm.BackColor = Color.LightGray; // при автозапуске объект неинициализирован
            }
            _startupTimer = new Timer();
            _startupTimer.Tick += StartupTimer_Tick;
            _startupTimer.Interval = 200;
            _startupTimer.Start();

            textBoxBTCAddress_new.Enabled = true;

            _remoteTimer = new Timer();
            _remoteTimer.Tick += RemoteTimer_Tick;
            _remoteTimer.Interval = 200;
            _remoteTimer.Start();

            _deviceStatusTimer = new Timer();
            _deviceStatusTimer.Tick += DeviceStatusTimer_Tick;
            _deviceStatusTimer.Interval = 1000;
            _deviceStatusTimer.Start();

        }

        //        [Obsolete("Deprecated in favour of AlgorithmSwitchingManager timer")]
        //       private async void SMAMinerCheck_Tick(object sender, EventArgs e)
        //        {
        //            _smaMinerCheck.Interval = ConfigManager.GeneralConfig.SwitchMinSecondsFixed * 1000 +
        //                                      R.Next(ConfigManager.GeneralConfig.SwitchMinSecondsDynamic * 1000);
        //            if (ComputeDeviceManager.Group.ContainsAmdGpus)
        //            {
        //                _smaMinerCheck.Interval =
        //                    (ConfigManager.GeneralConfig.SwitchMinSecondsAMD +
        //                     ConfigManager.GeneralConfig.SwitchMinSecondsFixed) * 1000 +
        //                    R.Next(ConfigManager.GeneralConfig.SwitchMinSecondsDynamic * 1000);
        //            }

        //#if (SWITCH_TESTING)
        //            SMAMinerCheck.Interval = MiningDevice.SMAMinerCheckInterval;
        //#endif
        //            if (_isSmaUpdated)
        //            {
        //                // Don't bother checking for new profits unless SMA has changed
        //                _isSmaUpdated = false;
        //                await MinersManager.SwichMostProfitableGroupUpMethod();
        //            }
        //        }

        private static async void MinerStatsCheck_Tick(object sender, EventArgs e)
        {
            await MinersManager.MinerStatsCheck();
        }

        private static void ComputeDevicesCheckTimer_Tick(object sender, EventArgs e)
        {
            if (ComputeDeviceManager.Query.CheckVideoControllersCountMismath())
            {
                // less GPUs than before, ACT!
                try
                {
                    var onGpusLost = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\OnGPUsLost.bat")
                    {
                        WindowStyle = ProcessWindowStyle.Minimized
                    };
                    Process.Start(onGpusLost);
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("NICEHASH", "OnGPUsMismatch.bat error: " + ex.Message);
                }
            }
        }

        private void InitFlowPanelStart()
        {
            flowLayoutPanelRates.Controls.Clear();
            // add for every cdev a
            foreach (var cdev in ComputeDeviceManager.Available.Devices)
            {
                if (cdev.Enabled)
                {
                    var newGroupProfitControl = new GroupProfitControl
                    {
                        Visible = false
                    };
                    flowLayoutPanelRates.Controls.Add(newGroupProfitControl);
                }
            }
        }

        public void ClearRatesAll()
        {
            HideNotProfitable();
            ClearRates(-1);
        }

        public void ClearRates(int groupCount)
        {
            if (InvokeRequired)
            {
                Invoke((Action)delegate { ClearRates(groupCount); });
                return;
            }
            if (_flowLayoutPanelVisibleCount != groupCount)
            {
                _flowLayoutPanelVisibleCount = groupCount;
                // hide some Controls
                var hideIndex = 0;
                foreach (var control in flowLayoutPanelRates.Controls)
                {
                    ((GroupProfitControl)control).Visible = hideIndex < groupCount;
                    ++hideIndex;
                }
            }
            _flowLayoutPanelRatesIndex = 0;
            var visibleGroupCount = 1;
            if (groupCount > 0) visibleGroupCount += groupCount;

            var groupBox1Height = _emtpyGroupPanelHeight;
            if (flowLayoutPanelRates.Controls.Count > 0)
            {
                var control = flowLayoutPanelRates.Controls[0];
                var panelHeight = ((GroupProfitControl)control).Size.Height * 1.1f;
                groupBox1Height = (int)((visibleGroupCount) * panelHeight - panelHeight/3.0f);
            }

            groupBox1.Size = new Size(groupBox1.Size.Width, groupBox1Height);
            // set new height
            Size = new Size(Size.Width, _mainFormHeight + groupBox1Height);
        }

        public void AddRateInfo(string groupName, string deviceStringInfo, ApiData iApiData, double paying, double power,
            bool isApiGetException)
        {
            var apiGetExceptionString = isApiGetException ? " **" : "";

            var speedString =
                Helpers.FormatDualSpeedOutput(iApiData.Speed, iApiData.SecondarySpeed, iApiData.AlgorithmID) +
                iApiData.AlgorithmName + apiGetExceptionString;
            var rateBtcString = FormatPayingOutput(paying, power);
            if (ConfigManager.GeneralConfig.DecreasePowerCost)
            {
                power = 0;
            }
            var rateCurrencyString = ExchangeRateApi
                                         .ConvertToActiveCurrency((paying + power) * ExchangeRateApi.GetUsdExchangeRate() * _factorTimeUnit)
                                         .ToString("F2", CultureInfo.InvariantCulture)
                                     + $"{ExchangeRateApi.ActiveDisplayCurrency}/" +
                                     International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());

            try
            {
                // flowLayoutPanelRatesIndex may be OOB, so catch
                ((GroupProfitControl)flowLayoutPanelRates.Controls[_flowLayoutPanelRatesIndex++])
                    .UpdateProfitStats(groupName, deviceStringInfo, speedString, rateBtcString, rateCurrencyString);
            }
            catch { }

            UpdateGlobalRate();
        }

        public void ShowNotProfitable(string msg)
        {
            if (ConfigManager.GeneralConfig.UseIFTTT)
            {
                if (!_isNotProfitable)
                {
                    Ifttt.PostToIfttt("nicehash", msg);
                    _isNotProfitable = true;
                }
            }

            if (InvokeRequired)
            {
                Invoke((Action)delegate
               {
                   ShowNotProfitable(msg);
               });
            }
            else
            {
                label_NotProfitable.Visible = true;
                label_NotProfitable.Text = msg;
                label_NotProfitable.Invalidate();
            }
        }

        public void HideNotProfitable()
        {
            if (ConfigManager.GeneralConfig.UseIFTTT)
            {
                if (_isNotProfitable)
                {
                    Ifttt.PostToIfttt("nicehash", "Mining is once again profitable and has resumed.");
                    _isNotProfitable = false;
                }
            }

            if (InvokeRequired)
            {
                Invoke((Action)HideNotProfitable);
            }
            else
            {
                label_NotProfitable.Visible = false;
                label_NotProfitable.Invalidate();
            }
        }

        public void ForceMinerStatsUpdate()
        {
            try
            {
                BeginInvoke((Action)(() =>
               {
                   MinerStatsCheck_Tick(null, null);
               }));
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("NiceHash", e.ToString());
            }
        }

        private void UpdateGlobalRate()
        {
            var totalRate = MinersManager.GetTotalRate();
            var totalPowerRate = MinersManager.GetTotalPowerRate();
            var powerString = "";


            //groupMiners.CurrentRate -= ExchangeRateApi.GetKwhPriceInBtc() * powerUsage * 24 / 1000;


            if (ConfigManager.GeneralConfig.AutoScaleBTCValues && totalRate < 0.1)
            {
                if (totalPowerRate != 0)
                {
                    powerString = "(-" + (totalPowerRate * 1000 * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture) + ") ";
                }
                toolStripStatusLabelBTCDayText.Text = powerString + " " +
                    "mBTC/" + International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
                toolStripStatusLabelGlobalRateValue.Text =
                    ((totalRate + totalPowerRate) * 1000 * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture);
            }
            else
            {
                if (totalPowerRate != 0)
                {
                    powerString = "(-" + (totalPowerRate * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture) + ") ";
                }
                toolStripStatusLabelBTCDayText.Text = powerString + " " +
                    "BTC/" + International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
                toolStripStatusLabelGlobalRateValue.Text =
                    ((totalRate + totalPowerRate) * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture);
            }
            if (totalPowerRate != 0)
            {
                powerString = "(-" + ExchangeRateApi.ConvertToActiveCurrency((totalPowerRate * _factorTimeUnit * ExchangeRateApi.GetUsdExchangeRate()))
                .ToString("F2", CultureInfo.InvariantCulture) + ") ";
            }
            else
            {
                powerString = "";
            }
            toolStripStatusLabelBTCDayValue.Text = ExchangeRateApi
                .ConvertToActiveCurrency(((totalRate + totalPowerRate) * _factorTimeUnit * ExchangeRateApi.GetUsdExchangeRate()))
                .ToString("F2", CultureInfo.InvariantCulture);
            toolStripStatusLabelBalanceText.Text = powerString + (ExchangeRateApi.ActiveDisplayCurrency + "/") +
                                                   International.GetText(
                                                       ConfigManager.GeneralConfig.TimeUnit.ToString()) + "   " +
                                                   International.GetText("Form_Main_balance") + ":";
        }


        private void BalanceCallback(object sender, EventArgs e)
        {
            Helpers.ConsolePrint("NICEHASH", "Balance update");
            var balance = NiceHashStats.Balance;
            if (balance > 0)
            {
                if (ConfigManager.GeneralConfig.AutoScaleBTCValues && balance < 0.1)
                {
                    toolStripStatusLabelBalanceBTCCode.Text = "mBTC";
                    toolStripStatusLabelBalanceBTCValue.Text =
                        (balance * 1000).ToString("F5", CultureInfo.InvariantCulture);
                }
                else
                {
                    toolStripStatusLabelBalanceBTCCode.Text = "BTC";
                    toolStripStatusLabelBalanceBTCValue.Text = balance.ToString("F6", CultureInfo.InvariantCulture);
                }

                //Helpers.ConsolePrint("CurrencyConverter", "Using CurrencyConverter" + ConfigManager.Instance.GeneralConfig.DisplayCurrency);
                var amount = (balance * ExchangeRateApi.GetUsdExchangeRate());
                amount = ExchangeRateApi.ConvertToActiveCurrency(amount);
                toolStripStatusLabelBalanceDollarText.Text = amount.ToString("F2", CultureInfo.InvariantCulture);
                toolStripStatusLabelBalanceDollarValue.Text = $"({ExchangeRateApi.ActiveDisplayCurrency})";
            }
        }


        //private void BitcoinExchangeCheck_Tick(object sender, EventArgs e)
        //{
        //    Helpers.ConsolePrint("NICEHASH", "Bitcoin rate get");
        //    ExchangeRateApi.UpdateApi(textBoxWorkerName.Text.Trim());
        //    UpdateExchange();
        //}

        private void ExchangeCallback(object sender, EventArgs e)
        {
            //// We are getting data from socket so stop checking manually
            //_bitcoinExchangeCheck?.Stop();
            //Helpers.ConsolePrint("NICEHASH", "Bitcoin rate get");
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)UpdateExchange);
            }
            else
            {
                UpdateExchange();
            }
        }

        private void UpdateExchange()
        {
            var br = ExchangeRateApi.GetUsdExchangeRate();
            var currencyRate = International.GetText("BenchmarkRatioRateN_A");
            if (br > 0)
            {
                currencyRate = ExchangeRateApi.ConvertToActiveCurrency(br).ToString("F2");
            }

            toolTip1.SetToolTip(statusStrip1, $"1 BTC = {currencyRate} {ExchangeRateApi.ActiveDisplayCurrency}");

            Helpers.ConsolePrint("NICEHASH",
                "Current Bitcoin rate: " + br.ToString("F2", CultureInfo.InvariantCulture));
        }

        private void SmaCallback(object sender, EventArgs e)
        {
            Helpers.ConsolePrint("NICEHASH", "SMA Update");
            //_isSmaUpdated = true;
        }

        private void VersionBurnCallback(object sender, SocketEventArgs e)
        {
            BeginInvoke((Action)(() =>
           {
               StopMining();
               _benchmarkForm?.StopBenchmark();
               var dialogResult = MessageBox.Show(e.Message, International.GetText("Error_with_Exclamation"),
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
               Application.Exit();
           }));
        }


        private void ConnectionLostCallback(object sender, EventArgs e)
        {
            if (!NHSmaData.HasData && ConfigManager.GeneralConfig.ShowInternetConnectionWarning &&
                _showWarningNiceHashData)
            {
                _showWarningNiceHashData = false;
                var dialogResult = MessageBox.Show(International.GetText("Form_Main_msgbox_NoInternetMsg"),
                    International.GetText("Form_Main_msgbox_NoInternetTitle"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (dialogResult == DialogResult.Yes)
                    return;
                if (dialogResult == DialogResult.No)
                    Application.Exit();
            }
        }

        private void ConnectionEstablishedCallback(object sender, EventArgs e)
        {
            /*
            // send credentials
            if (ConfigManager.GeneralConfig.NewPlatform)
            {
                //NiceHashStats.SetCredentials(textBoxBTCAddress_new.Text.Trim(), textBoxWorkerName.Text.Trim());
            } else
            {
                NiceHashStats.SetCredentials(textBoxBTCAddress.Text.Trim(), textBoxWorkerName.Text.Trim());
            }
            */
        }

        private void VersionUpdateCallback(object sender, EventArgs e)
        {
            var ver = NiceHashStats.Version.Replace(",", ".");
            if (ver == null) return;
            //var programVersion = "Fork_Fix_"+ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",",".");
            var programVersion = ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",", ".");
            Helpers.ConsolePrint("Program version: ", programVersion);
            //var ret = programVersion.CompareTo(ver);
            if (ver.Length < 1)
            {
                return;
            }
            ver = ver.Replace("Fork_Fix_", "");
            Helpers.ConsolePrint("Github version: ", ver);
            double vern = double.Parse(ver, CultureInfo.InvariantCulture);
            double programVersionn = double.Parse(programVersion, CultureInfo.InvariantCulture);
            Helpers.ConsolePrint("Program version: ", programVersionn.ToString());
            Helpers.ConsolePrint("Github version: ", vern.ToString());
            //if (ret < 0 || (ret == 0 && BetaAlphaPostfixString != ""))
            if (programVersionn < vern)
            {
                Helpers.ConsolePrint("Old version detected. Update needed.", "");
                SetVersionLabel(string.Format(International.GetText("Form_Main_new_version_released").Replace("v{0}", "{0}"), "Fork Fix " + ver));
                //_visitUrlNew = Links.VisitUrlNew + ver;
                _visitUrlNew = Links.VisitUrlNew;
            }
        }

        private delegate void SetVersionLabelCallback(string text);

        private void SetVersionLabel(string text)
        {
            if (linkLabelNewVersion.InvokeRequired)
            {
                var d = new SetVersionLabelCallback(SetVersionLabel);
                Invoke(d, new object[] { text });
            }
            else
            {
                linkLabelNewVersion.Text = text;
            }
        }

        private bool VerifyMiningAddress(bool showError)
        {

            //if (ConfigManager.GeneralConfig.NewPlatform)
            if (true)
            {
                if (!BitcoinAddress.ValidateBitcoinAddress(textBoxBTCAddress_new.Text.Trim()) && showError)
                {
                    var result = MessageBox.Show(International.GetText("Form_Main_msgbox_InvalidBTCAddressMsg"),
                        International.GetText("Error_with_Exclamation"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        Process.Start(Links.NhmBtcWalletFaqNew);

                    textBoxBTCAddress_new.Focus();
                    return false;
                }
            }
            /*
            else
            {
                if (!BitcoinAddress.ValidateBitcoinAddress(textBoxBTCAddress.Text.Trim()) && showError)
                {
                    var result = MessageBox.Show(International.GetText("Form_Main_msgbox_InvalidBTCAddressMsg"),
                        International.GetText("Error_with_Exclamation"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        Process.Start(Links.NhmBtcWalletFaq);

                    textBoxBTCAddress.Focus();
                    return false;
                }
            }
            */
            if (!BitcoinAddress.ValidateWorkerName(textBoxWorkerName.Text.Trim()) && showError)
            {
                var result = MessageBox.Show(International.GetText("Form_Main_msgbox_InvalidWorkerNameMsg"),
                    International.GetText("Error_with_Exclamation"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                textBoxWorkerName.Focus();
                return false;
            }
            return true;
        }

        private void LinkLabelCheckStats_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!VerifyMiningAddress(true)) return;
            //if (ConfigManager.GeneralConfig.NewPlatform)
            if (true)
            {
                Process.Start(Links.CheckStatsNew + textBoxBTCAddress_new.Text.Trim());
            }
            /*
            else
            {
                Process.Start(Links.CheckStats + textBoxBTCAddress.Text.Trim());
            }
            */
        }


        private void LinkLabelChooseBTCWallet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ConfigManager.GeneralConfig.NewPlatform)
            {
                Process.Start(Links.NhmBtcWalletFaqNew);
            } else
            {
                Process.Start(Links.NhmBtcWalletFaq);
            }
        }

        private void LinkLabelNewVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(_visitUrlNew);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            devicesListViewEnableControl1.SaveColumns();
            if (this != null)
            {
                if (ConfigManager.GeneralConfig.Save_windows_size_and_position)
                {
                    ConfigManager.GeneralConfig.FormWidth = this.Width;
                    if (this.Top + this.Left >= 1)
                    {
                        ConfigManager.GeneralConfig.FormTop = this.Top;
                        ConfigManager.GeneralConfig.FormLeft = this.Left;
                    }
                }
            }
            MinersManager.StopAllMiners();
            if (Miner._cooldownCheckTimer != null && Miner._cooldownCheckTimer.Enabled) Miner._cooldownCheckTimer.Stop();
            MessageBoxManager.Unregister();
            ConfigManager.GeneralConfigFileCommit();
        }

        private void ButtonBenchmark_Click(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.ServiceLocation = comboBoxLocation.SelectedIndex;

            _benchmarkForm = new Form_Benchmark();
          //  SetChildFormCenter(_benchmarkForm);
            _benchmarkForm.ShowDialog();
            var startMining = _benchmarkForm.StartMining;
            _benchmarkForm = null;

            InitMainConfigGuiData();
            if (startMining)
            {
                ButtonStartMining_Click(null, null);
            }
        }


        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            var settings = new Form_Settings();
            //   SetChildFormCenter(settings);
            settings.ShowDialog();
            
            if (settings.IsChange && settings.IsChangeSaved && settings.IsRestartNeeded)
            {
                MessageBox.Show(
                    International.GetText("Form_Main_Restart_Required_Msg"),
                    International.GetText("Form_Main_Restart_Required_Title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                var pHandle = new Process
                {
                    StartInfo =
                    {
                        FileName = Application.ExecutablePath
                    }
                };
                pHandle.Start();
                Close();
            }
            else if (settings.IsChange && settings.IsChangeSaved)
            {
                InitLocalization();
                InitMainConfigGuiData();
            }
        }

        private void ButtonStartMining_Click(object sender, EventArgs e)
        {
            _isManuallyStarted = true;
            if (StartMining(true) == StartMiningReturnType.ShowNoMining)
            {
                _isManuallyStarted = false;
                StopMining();
                MessageBox.Show(International.GetText("Form_Main_StartMiningReturnedFalse"),
                    International.GetText("Warning_with_Exclamation"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void ButtonStopMining_Click(object sender, EventArgs e)
        {
            firstRun = true;
            _isManuallyStarted = false;
            StopMining();
        }

        private string FormatPayingOutput(double paying, double power)
        {
            string ret;
            if (ConfigManager.GeneralConfig.DecreasePowerCost)
            {
                power = 0;
            }

            if (ConfigManager.GeneralConfig.AutoScaleBTCValues && paying < 0.1)
                ret = ((paying + power) * 1000 * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture) +
                    " mBTC/" +
                      International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
            else
                ret = ((paying + power) * _factorTimeUnit).ToString("F6", CultureInfo.InvariantCulture) +
                    " BTC/" +
                      International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());

            return ret;
        }

        private void ButtonLogo_Click(object sender, EventArgs e)
        {
            Process.Start(Links.VisitUrl);
        }

        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            Process.Start(Links.NhmHelp);
        }

        private void ToolStripStatusLabel10_Click(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.NewPlatform)
            {
                Process.Start(Links.NhmPayingFaqNew);
            } else
            {
                Process.Start(Links.NhmPayingFaq);
            }
        }

        private void ToolStripStatusLabel10_MouseHover(object sender, EventArgs e)
        {
            statusStrip1.Cursor = Cursors.Hand;
        }

        private void ToolStripStatusLabel10_MouseLeave(object sender, EventArgs e)
        {
            statusStrip1.Cursor = Cursors.Default;
        }

        private void TextBoxCheckBoxMain_Leave(object sender, EventArgs e)
        {
            //if (ConfigManager.GeneralConfig.NewPlatform)
            if (true)
            {
                if (VerifyMiningAddress(false))
                {
                    if (ConfigManager.GeneralConfig.BitcoinAddressNew != textBoxBTCAddress_new.Text.Trim()
                        || ConfigManager.GeneralConfig.WorkerName != textBoxWorkerName.Text.Trim())
                    {
                        // Reset credentials
                        NiceHashStats.SetCredentials(textBoxBTCAddress_new.Text.Trim(), textBoxWorkerName.Text.Trim());
                    }
                    // Commit to config.json
                    ConfigManager.GeneralConfig.BitcoinAddressNew = textBoxBTCAddress_new.Text.Trim();
                    ConfigManager.GeneralConfig.WorkerName = textBoxWorkerName.Text.Trim();
                    ConfigManager.GeneralConfig.ServiceLocation = comboBoxLocation.SelectedIndex;
                   // ConfigManager.GeneralConfigFileCommit();
                }
            }
            /*
            else
            {
                if (VerifyMiningAddress(false))
                {
                    if (ConfigManager.GeneralConfig.BitcoinAddress != textBoxBTCAddress.Text.Trim()
                        || ConfigManager.GeneralConfig.WorkerName != textBoxWorkerName.Text.Trim())
                    {
                        // Reset credentials
                        NiceHashStats.SetCredentials(textBoxBTCAddress.Text.Trim(), textBoxWorkerName.Text.Trim());
                    }
                    // Commit to config.json
                    ConfigManager.GeneralConfig.BitcoinAddress = textBoxBTCAddress.Text.Trim();
                    ConfigManager.GeneralConfig.WorkerName = textBoxWorkerName.Text.Trim();
                    ConfigManager.GeneralConfig.ServiceLocation = comboBoxLocation.SelectedIndex;
                   // ConfigManager.GeneralConfigFileCommit();
                }
            }
            */
            ConfigManager.GeneralConfigFileCommit();
        }

        // Minimize to system tray if MinimizeToTray is set to true
        private void Form1_Resize(object sender, EventArgs e)
        {

            notifyIcon1.Icon = Properties.Resources.logo;
            notifyIcon1.Text = Application.ProductName + " v" + Application.ProductVersion +
                               "\nDouble-click to restore..";

            if (ConfigManager.GeneralConfig.MinimizeToTray && FormWindowState.Minimized == WindowState)
            {
                notifyIcon1.Visible = true;
                Hide();
            }
            buttonStartMining.Refresh();
            buttonStopMining.Refresh();
        }

        // Restore NiceHashMiner from the system tray
        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        ///////////////////////////////////////
        // Miner control functions
        public enum StartMiningReturnType
        {
            StartMining,
            ShowNoMining,
            IgnoreMsg
        }

        private StartMiningReturnType StartMining(bool showWarnings)
        {
            if (_autostartTimerDelay != null)
            {
                _autostartTimerDelay.Stop();
                _autostartTimerDelay = null;
                buttonStopMining.Text = International.GetText("Form_Main_stop");
            }
            if (_autostartTimer != null)
            {
                _autostartTimer.Stop();
                _autostartTimer = null;
            }
            //if (ConfigManager.GeneralConfig.NewPlatform)
            if (true)
            {
                NiceHashStats.DeviceStatus_TickNew("MINING");
                if (textBoxBTCAddress_new.Text.Equals(""))
                {
                    if (showWarnings)
                    {
                        var result = MessageBox.Show(International.GetText("Form_Main_DemoModeMsg"),
                            International.GetText("Form_Main_DemoModeTitle"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            _demoMode = true;
                            labelDemoMode.Visible = true;
                            labelDemoMode.Text = International.GetText("Form_Main_DemoModeLabel");
                        }
                        else
                        {
                            NiceHashStats.DeviceStatus_TickNew("STOPPED");
                            return StartMiningReturnType.IgnoreMsg;
                        }
                    }
                    else
                    {
                        return StartMiningReturnType.IgnoreMsg;
                    }
                }
                else if (!VerifyMiningAddress(true))
                {
                    NiceHashStats.DeviceStatus_TickNew("STOPPED");
                    return StartMiningReturnType.IgnoreMsg;
                }
            }
            /*
            else
            {
                if (textBoxBTCAddress.Text.Equals(""))
                {
                    if (showWarnings)
                    {
                        var result = MessageBox.Show(International.GetText("Form_Main_DemoModeMsg"),
                            International.GetText("Form_Main_DemoModeTitle"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.Yes)
                        {
                            _demoMode = true;
                            labelDemoMode.Visible = true;
                            labelDemoMode.Text = International.GetText("Form_Main_DemoModeLabel");
                        }
                        else
                        {
                            return StartMiningReturnType.IgnoreMsg;
                        }
                    }
                    else
                    {
                        return StartMiningReturnType.IgnoreMsg;
                    }
                }
                else if (!VerifyMiningAddress(true)) return StartMiningReturnType.IgnoreMsg;
            }
            */
            var hasData = NHSmaData.HasData;
            if (!showWarnings)
            {
                for (var i = 0; i < 10; i++)
                {
                    if (hasData) break;
                    Thread.Sleep(1000);
                    hasData = NHSmaData.HasData;
                    Helpers.ConsolePrint("NICEHASH", $"After {i}s has data: {hasData}");
                }
            }
            if (!hasData)
            {
                Helpers.ConsolePrint("NICEHASH", "No data received within timeout");
                if (showWarnings)
                {
                    MessageBox.Show(International.GetText("Form_Main_msgbox_NullNiceHashDataMsg"),
                        International.GetText("Error_with_Exclamation"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                NiceHashStats.DeviceStatus_TickNew("STOPPED");
                return StartMiningReturnType.IgnoreMsg;
            }

            // Check if there are unbenchmakred algorithms
            var isBenchInit = true;
            foreach (var cdev in ComputeDeviceManager.Available.Devices)
            {
                if (cdev.Enabled)
                {
                    if (cdev.GetAlgorithmSettings().Where(algo => algo.Enabled).Any(algo => algo.BenchmarkSpeed == 0))
                    {
                        isBenchInit = false;
                    }
                }
            }
            // Check if the user has run benchmark first
            if (!isBenchInit)
            {
                var result = DialogResult.No;
                if (showWarnings)
                {
                    result = MessageBox.Show(International.GetText("EnabledUnbenchmarkedAlgorithmsWarning"),
                        International.GetText("Warning_with_Exclamation"),
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                }
                if (result == DialogResult.Yes)
                {
                    _benchmarkForm = new Form_Benchmark(
                        BenchmarkPerformanceType.Standard,
                        true);
                  //  SetChildFormCenter(_benchmarkForm);
                    _benchmarkForm.ShowDialog();
                    _benchmarkForm = null;
                    InitMainConfigGuiData();
                }
                else if (result == DialogResult.No)
                {
                    // check devices without benchmarks
                    foreach (var cdev in ComputeDeviceManager.Available.Devices)
                    {
                        if (cdev.Enabled)
                        {
                            var enabled = cdev.GetAlgorithmSettings().Any(algo => algo.BenchmarkSpeed > 0);
                            cdev.Enabled = enabled;
                        }
                    }
                }
                else
                {
                    NiceHashStats.DeviceStatus_TickNew("STOPPED");
                    return StartMiningReturnType.IgnoreMsg;
                }
            }
           // textBoxBTCAddress.Enabled = false;
            textBoxBTCAddress_new.Enabled = false;
            textBoxWorkerName.Enabled = false;
            comboBoxLocation.Enabled = false;
            //buttonBenchmark.Enabled = false;
            buttonStartMining.Enabled = false;
            //buttonSettings.Enabled = false;
            devicesListViewEnableControl1.IsMining = true;
            buttonStopMining.Enabled = true;

            // Disable profitable notification on start
            _isNotProfitable = false;
            //ConfigManager.GeneralConfig.BitcoinAddress = textBoxBTCAddress.Text.Trim();
            ConfigManager.GeneralConfig.BitcoinAddressNew = textBoxBTCAddress_new.Text.Trim();
            ConfigManager.GeneralConfig.WorkerName = textBoxWorkerName.Text.Trim();
            ConfigManager.GeneralConfig.ServiceLocation = comboBoxLocation.SelectedIndex;

            InitFlowPanelStart();
            ClearRatesAll();

            bool isMining;
            var btcAdress = "";
            //if (ConfigManager.GeneralConfig.NewPlatform)
            if (true)
            {
                 btcAdress = _demoMode ? Globals.DemoUser : textBoxBTCAddress_new.Text.Trim();
            }
            /*
            else
            {
                 btcAdress = _demoMode ? Globals.DemoUser : textBoxBTCAddress.Text.Trim();
            }
            */
            if (comboBoxLocation.SelectedIndex < 6)
            {
                isMining = MinersManager.StartInitialize(this, Globals.MiningLocation[comboBoxLocation.SelectedIndex],
                    textBoxWorkerName.Text.Trim(), btcAdress);
            }
            else
            {
                isMining = MinersManager.StartInitialize(this, Globals.MiningLocation[Miner.PingServers()],
                    textBoxWorkerName.Text.Trim(), btcAdress);
            }

            if (!_demoMode) ConfigManager.GeneralConfigFileCommit();

            //_isSmaUpdated = true; // Always check profits on mining start
            //_smaMinerCheck.Interval = 100;
            //_smaMinerCheck.Start();
            _minerStatsCheck.Start();
            NiceHashStats.DeviceStatus_TickNew("MINING");
            if (ConfigManager.GeneralConfig.RunScriptOnCUDA_GPU_Lost)
            {
                _computeDevicesCheckTimer = new SystemTimer();
                _computeDevicesCheckTimer.Elapsed += ComputeDevicesCheckTimer_Tick;
                _computeDevicesCheckTimer.Interval = 60000;

                _computeDevicesCheckTimer.Start();
            }

            return isMining ? StartMiningReturnType.StartMining : StartMiningReturnType.ShowNoMining;
        }
        private void RemoteTimer_Tick(object sender, EventArgs e)
        {
            if (NiceHashStats.remoteMiningStart)
            {
                NiceHashStats.remoteMiningStart = false;
                StartMining(true);
            }
            if (NiceHashStats.remoteMiningStop)
            {
                NiceHashStats.remoteMiningStop = false;
                StopMining();
            }
            if (NiceHashStats.remoteUpdateUI)
            {
                NiceHashStats.remoteUpdateUI = false;
                InitMainConfigGuiData();
                ConfigManager.GeneralConfigFileCommit();
            }

            //_remoteTimer.Stop();
            //_remoteTimer= null;
        }

        private void restartProgram()
        {
            var pHandle = new Process
            {
                StartInfo =
                    {
                        FileName = Application.ExecutablePath
                    }
            };
           // pHandle.Start();
           // Close();
        }
        private void DeviceStatusTimer_Tick(object sender, EventArgs e)
        {
            if (needRestart)
            {
                needRestart = false;
                restartProgram();
            }
            devicesListViewEnableControl1.SetComputeDevicesStatus(ComputeDeviceManager.Available.Devices);
        }
        private void StopMining()
        {
            NiceHashStats.DeviceStatus_TickNew("PENDING");
            _minerStatsCheck.Stop();
            //_smaMinerCheck.Stop();
            _computeDevicesCheckTimer?.Stop();

            // Disable IFTTT notification before label call
            _isNotProfitable = false;

            MinersManager.StopAllMiners();

            textBoxBTCAddress_new.Enabled = true;
           // textBoxBTCAddress.Enabled = true;
            textBoxWorkerName.Enabled = true;
            comboBoxLocation.Enabled = true;
            buttonBenchmark.Enabled = true;
            buttonStartMining.Enabled = true;
            buttonSettings.Enabled = true;
            devicesListViewEnableControl1.IsMining = false;
            buttonStopMining.Enabled = false;

            if (_demoMode)
            {
                _demoMode = false;
                labelDemoMode.Visible = false;
            }

            UpdateGlobalRate();
        }

        private void comboBoxLocation_SelectedIndexChanged(object sender, EventArgs e)
        { 
            ConfigManager.GeneralConfig.ServiceLocation = comboBoxLocation.SelectedIndex;
        }

        private void comboBoxLocation_DrawItem(object sender, DrawItemEventArgs e)
        {
            var cmb = (ComboBox)sender;
            if (cmb == null) return;


                e.DrawBackground();

            // change background color
            var bc = new SolidBrush(_backColor);
            var fc = new SolidBrush(_foreColor);
            var wc = new SolidBrush(_windowColor);
            var gr = new SolidBrush(Color.Gray);
            e.Graphics.FillRectangle(bc, e.Bounds);


            // change foreground color
            Brush brush = ((e.State & DrawItemState.Selected) > 0) ? fc : gr;
            if (e.Index >= 0)
            {
                e.Graphics.DrawString(cmb.Items[e.Index].ToString(), cmb.Font, brush, e.Bounds);
                e.DrawFocusRectangle();
            }

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        /*
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.NewPlatform = !radioButtonOldPlatform.Checked;
//            ConfigManager.GeneralConfigFileCommit();
        }

        private void radioButtonNewPlatform_CheckedChanged(object sender, EventArgs e)
        {

            ConfigManager.GeneralConfig.NewPlatform = radioButtonNewPlatform.Checked;
            textBoxBTCAddress.Enabled = !radioButtonNewPlatform.Checked;
            textBoxBTCAddress_new.Enabled = radioButtonNewPlatform.Checked;
           // ConfigManager.GeneralConfigFileCommit();
            Thread.Sleep(10);

            if (firstStartConnection && NiceHashSocket._webSocket != null)
            {
                ConfigManager.GeneralConfigFileCommit();
                NiceHashStats._socket = null;
                NiceHashSocket._restartConnection = true;
                NiceHashSocket._webSocket.Close();
                NiceHashSocket._webSocket = null;


                NiceHashStats._deviceUpdateTimer.Change(System.Threading.Timeout.Infinite, 0);
                NiceHashStats._deviceUpdateTimer.Dispose();
                NiceHashStats._deviceUpdateTimer = null;
                NiceHashStats.ClearAlgorithmRates();
                Thread.Sleep(10);

                if (Configs.ConfigManager.GeneralConfig.NewPlatform)
                {
                    NiceHashStats.StartConnection(Links.NhmSocketAddress_new);
                }
                else
                {
                    NiceHashStats.StartConnection(Links.NhmSocketAddress);
                }
            }
        }
        */
        /*
        public class ListViewWithoutScrollBar : ListView
        {
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case 0x83: // WM_NCCALCSIZE
                        int style = (int)GetWindowLong(this.Handle, GWL_STYLE);
                        if ((style & WS_VSCROLL) == WS_VSCROLL)
                            SetWindowLong(this.Handle, GWL_STYLE, style & ~WS_VSCROLL);
                        base.WndProc(ref m);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
            const int GWL_STYLE = -16;
            const int WS_VSCROLL = 0x00200000;

            public static int GetWindowLong(IntPtr hWnd, int nIndex)
            {
                if (IntPtr.Size == 4)
                    return (int)GetWindowLong32(hWnd, nIndex);
                else
                    return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
            }

            public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
            {
                if (IntPtr.Size == 4)
                    return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
                else
                    return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }

            [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
            public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
            public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
            public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
            public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);
        }
        //[DllImport("user32")]
        //private static extern long ShowScrollBar(long hwnd, long wBar, long bShow);
        long SB_HORZ = 0;
        long SB_VERT = 1;
        long SB_BOTH = 3;

        [System.Runtime.InteropServices.DllImport("user32", CallingConvention = System.Runtime.InteropServices.CallingConvention.Winapi)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]

        private static extern bool ShowScrollBar(long hwnd, long wBar, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool bShow);
        private void HideHorizontalScrollBar()
        {
            ShowScrollBar(devicesListViewEnableControl1.Handle.ToInt64(), SB_HORZ, false);
        }
        */
        private void devicesListViewEnableControl1_Load(object sender, EventArgs e)
        {
            /*
            devicesListViewEnableControl1.Enabled = false;
            devicesListViewEnableControl1.HorizontalScroll.Enabled = false;
            devicesListViewEnableControl1.VerticalScroll.Enabled = true;
            devicesListViewEnableControl1.Enabled = true;
            devicesListViewEnableControl1.AutoScroll = false;
           // devicesListViewEnableControl1.
           */
           // devicesListViewEnableControl1.AutoScroll = false;
            // HideHorizontalScrollBar();
           // devicesListViewEnableControl1.VerticalScroll.Enabled = true;

        }

        private void buttonStopMining_EnabledChanged(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                buttonStopMining.ForeColor = buttonStopMining.Enabled == true ? Form_Main._foreColor : Color.Gray;
                buttonStopMining.BackColor = buttonStopMining.Enabled == true ? Form_Main._backColor : Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            }
       }

        private void buttonStopMining_Paint(object sender, PaintEventArgs e)
        {
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                Button btn = (Button)sender;
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;   // center the text
                TextRenderer.DrawText(e.Graphics, buttonStopMining.Text, btn.Font, e.ClipRectangle, btn.ForeColor, flags);
            }
        }

        private void buttonStartMining_EnabledChanged(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                buttonStartMining.ForeColor = buttonStartMining.Enabled == true ? Form_Main._foreColor : Color.Gray;
                buttonStartMining.BackColor = buttonStartMining.Enabled == true ? Form_Main._backColor : Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            }
        }

        private void buttonStartMining_Paint(object sender, PaintEventArgs e)
        {
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                Button btn = (Button)sender;
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;   // center the text
                TextRenderer.DrawText(e.Graphics, buttonStartMining.Text, btn.Font, e.ClipRectangle, btn.ForeColor, flags);
            }
        }
    }


}
