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
    using Microsoft.Win32;
    using NiceHashMinerLegacy.Divert;
    using OpenHardwareMonitor.Hardware;
    using System.ComponentModel;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using static NiceHashMiner.Devices.ComputeDeviceManager;

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
        private Timer _updateTimer;
        private int _updateTimerCount;
        private int _updateTimerRestartProgramCount;
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
        public static int _flowLayoutPanelRatesIndex = 0;

        private const string BetaAlphaPostfixString = "";
        const string ForkString = " Fork Fix ";

        private bool _isDeviceDetectionInitialized = false;

        private bool _isManuallyStarted = false;
        private bool _isNotProfitable = false;

        //private bool _isSmaUpdated = false;
        private Process mainproc = Process.GetCurrentProcess();
        public static double _factorTimeUnit = 1.0;
        public static int nanominerCount = 0;
        private int _mainFormHeight = 0;
        private readonly int _emtpyGroupPanelHeight = 0;
        private int groupBox1Top = 0;
        bool firstStartConnection = false;
        private bool firstRun = false;
        public static Color _backColor;
        public static Color _foreColor;
        public static Color _windowColor;
        public static Color _textColor;
        public static double githubBuild = 0.0d;
        public static double currentBuild = 0.0d;
        public static double currentVersion = 0.0d;
        public static double githubVersion = 0.0d;
        public static string githubName = "";
        public static string github_browser_download_url = "";
        public static string BackupFileName = "";
        public static string BackupFileDate = "";
        public static bool NewVersionExist = false;
        public static bool CertInstalled = false;
//        public static bool DaggerHashimoto3GBProfit = false;
        public static bool DaggerHashimoto3GB = false;
        public static string GoogleIP = "";
        public static string GoogleAnswer = "";
        public static bool GoogleAvailable = false;
        public static bool DivertAvailable = true;
        private static string dialogClearBTC = "You want to delete BTC address?";
        //public static string[,] myServers = { { Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], "20000" }, { "usa", "20001" }, { "hk", "20002" }, { "jp", "20003" }, { "in", "20004" }, { "br", "20005" } };
        public static string[,] myServers = {
            { "eu", "20000" }, { "usa", "20001" }, { "hk", "20002" }, { "jp", "20003" }, { "in", "20004" }, { "br", "20005" }
            //{ Globals.MiningLocation[ConfigManager.GeneralConfig.ServiceLocation], "20000" }, { "usa", "20001" }, { "hk", "20002" }, { "jp", "20003" }, { "in", "20004" }, { "br", "20005" }
        };//need to reread config data?
        internal static bool DeviceStatusTimer_FirstTick = false;
        //private static readonly Computer _gpus = new Computer { GPUEnabled = true };
        public static Computer thisComputer;
        public static DateTime StartTime = new DateTime();
        public static TimeSpan Uptime;
        private static bool CheckVideoControllersCount = false;

        public Form_Main()
        {
            WindowState = FormWindowState.Minimized;
            Helpers.ConsolePrint("NICEHASH", "Start Form_Main");
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
            Helpers.ConsolePrint("NICEHASH", "Start InitializeComponent");
            StartTime = DateTime.Now;
            Process thisProc = Process.GetCurrentProcess();
            thisProc.PriorityClass = ProcessPriorityClass.High;

            InitializeComponent();

            Icon = Properties.Resources.logo;
            Helpers.ConsolePrint("NICEHASH", "Start InitLocalization");

            InitLocalization();
            devicesListViewEnableControl1.Visible = false;
            ComputeDeviceManager.SystemSpecs.QueryAndLog();
            groupBox1Top = groupBox1.Top;

            devicesListViewEnableControl1.Height = 129 + ConfigManager.GeneralConfig.DevicesCountIndex * 17 + 1;
            groupBox1Top += ConfigManager.GeneralConfig.DevicesCountIndex * 17 + 1 ;
            //this.Height += 16;

            if (ConfigManager.GeneralConfig.BitcoinAddressNew.Length == 0)
            {
                buttonBTC_Clear.Enabled = false;
                buttonBTC_Save.Enabled = false;
            }
            Helpers.ConsolePrint("NICEHASH", "Start query RAM");
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

            Text += ForkString;
            //Text += ConfigManager.GeneralConfig.ForkFixVersion.ToString();
            Text += "28 beta 1";


            var internalversion = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = new DateTime(2000, 1, 1).AddDays(internalversion.Build).AddSeconds(internalversion.Revision * 2);
            var build = buildDate.ToString("u").Replace("-", "").Replace(":", "").Replace("Z", "").Replace(" ", ".");
            Double.TryParse(build.ToString(), out Form_Main.currentBuild);
            Form_Main.currentVersion = ConfigManager.GeneralConfig.ForkFixVersion;

            label_NotProfitable.Visible = false;

            InitMainConfigGuiData();

            // for resizing
            InitFlowPanelStart();

            groupBox1.Height = 32;
            if (groupBox1.Size.Height > 0 && Size.Height > 0)
            {
                _emtpyGroupPanelHeight = groupBox1.Size.Height;
                _mainFormHeight = Size.Height - _emtpyGroupPanelHeight;
            }
            else
            {
               // _emtpyGroupPanelHeight = 59;
               // _mainFormHeight = 330 - _emtpyGroupPanelHeight;
            }
            //_mainFormHeight = Size.Height;

            ClearRatesAll();
            thisProc = Process.GetCurrentProcess();
            thisProc.PriorityClass = ProcessPriorityClass.Normal;
            //
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


            toolTip1.SetToolTip(buttonBTC_Clear, "Clear");
            toolTip1.SetToolTip(buttonBTC_Save, "Save");

            labelBitcoinAddressNew.Text = International.GetText("BitcoinAddress") + ":";
            labelWorkerName.Text = International.GetText("WorkerName") + ":";
            if (ConfigManager.GeneralConfig.ShowUptime)
            {
                label_Uptime.Text = International.GetText("Form_Main_Uptime");
                label_Uptime.Visible = true;
            } else
            {
                label_Uptime.Visible = false;
            }
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                labelBitcoinAddressNew.Text = "Биткоин адрес" + ":";
                labelWorkerName.Text = "Имя компьютера" + ":";
                dialogClearBTC = "Вы хотите удалить биткоин адрес?";
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
            toolStripStatusLabel_power1.Text = "";
            toolStripStatusLabel_power2.Text = "";
            toolStripStatusLabel_power3.Text = "";
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
            //BalanceCallback(null, null); // update currency changes

            if (_isDeviceDetectionInitialized)
            {
                devicesListViewEnableControl1.ResetComputeDevices(ComputeDeviceManager.Available.Devices);
            }
        }

        public void AfterLoadComplete()
        {
            if (ConfigManager.GeneralConfig.DivertRun)
            {
                /*
                var CMDconfigHandle = new Process

                {
                    StartInfo =
                {
                    FileName = "sc.exe"
                }
                };

                CMDconfigHandle.StartInfo.Arguments = "start WinDivert1.4";
                CMDconfigHandle.StartInfo.UseShellExecute = false;
                CMDconfigHandle.StartInfo.CreateNoWindow = true;
                CMDconfigHandle.Start();
                */
            }

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
                    /*
                    if (StartMining(false) != StartMiningReturnType.StartMining)
                    {
                        StopMining();
                    }
                    */
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
            }
        }

        public static void ProgressBarUpd(DownloadProgressChangedEventArgs e)
        {
            if (Form_Settings.ProgressProgramUpdate != null)
            {
                Form_Settings.ProgressProgramUpdate.Maximum = (int)e.TotalBytesToReceive / 100;
                Form_Settings.ProgressProgramUpdate.Value = (int)e.BytesReceived / 100;
            }
            if ((int)e.TotalBytesToReceive == (int)e.BytesReceived && Form_Settings.ProgressProgramUpdate != null)
            {
                Form_Settings.ProgressProgramUpdate.Visible = false;
            }
        }

        private bool CheckGithubDownload()
        {
            if (!Directory.Exists("temp")) Directory.CreateDirectory("temp");
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                WebClient client = new WebClient();
                client.UseDefaultCredentials = false;
                client.DownloadFileAsync(new Uri("https://github.com/angelbbs/NiceHashMinerLegacy/raw/master-old/NiceHashMiner/github.test"), "temp/github.test");
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("CheckGithubDownload", ex.ToString());
                return false;
            }
            
            return true;
        }
        static void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Helpers.ConsolePrint("CheckGithubDownload: ", e.Error.Message);
                return;
            } else
            {
                Helpers.ConsolePrint("CheckGithubDownload", "true");
            }
            return;
        }
        private void ResetProtocols()
        {
            var CMDconfigHandleDS = new Process
            {
                StartInfo =
                {
                    FileName = "reg.exe"
                }
            };

            CMDconfigHandleDS.StartInfo.Arguments = "import utils\\ds.reg";
            CMDconfigHandleDS.StartInfo.UseShellExecute = false;
            CMDconfigHandleDS.StartInfo.CreateNoWindow = true;
            CMDconfigHandleDS.Start();
            /*
            if (Configs.ConfigManager.GeneralConfig.ResetProtocols)
            {
                var CMDconfigHandleP = new Process
                {
                    StartInfo =
                {
                    FileName = "reg.exe"
                }
                };

                CMDconfigHandleP.StartInfo.Arguments = "import utils\\Protocols.reg";
                CMDconfigHandleP.StartInfo.UseShellExecute = false;
                CMDconfigHandleP.StartInfo.CreateNoWindow = true;
                CMDconfigHandleP.Start();
            }
            */
        }

        private void CheckUpdates()
        {
            try
            {
                CheckGithub();
                //checkD();
                /*
                NiceHashStats.ConnectToGoogle();
                if (GoogleAnswer.Contains("HTTP"))
                {
                    Helpers.ConsolePrint("ConnectToGoogle", "Connect to google OK");
                }
                //checkD();
                new Task(() => checkD()).Start();
                */

            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("CheckGithub", er.ToString());
            }
        }

        public static void checkD()
        {
            if (ConfigManager.GeneralConfig.DivertRun)
            {
                //divert test
                Process thisProc = Process.GetCurrentProcess();
                int dhandle = (int)Divert.DivertStart(thisProc.Id, -100, 0, "", "", "", ConfigManager.GeneralConfig.DivertLog, false, false, false);
                if (dhandle != -1)
                {
                    Thread.Sleep(500);
                    NiceHashStats.ConnectToGoogle("Check connection");
                    Thread.Sleep(2000);
                    Divert.DivertStop((IntPtr)dhandle, thisProc.Id, -100, 0, true);
                    //Helpers.ConsolePrint("ConnectToGoogle", GoogleAnswer); //is Running?
                    if (GoogleAnswer.Contains("Running")) DivertAvailable = true;
                }
            }
        }

        private void StartupTimer_Tick(object sender, EventArgs e)
        {
            if (!ConfigManager.GeneralConfig.AutoStartMining)
            {
                buttonStopMining.Enabled = false;
               // buttonBTC_Clear.Enabled = true;
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

            // Query Available ComputeDevices
            //Thread.Sleep(100);
            ComputeDeviceManager.Query.QueryDevices(_loadingScreen);
            //Thread.Sleep(200);
            _isDeviceDetectionInitialized = true;

            /////////////////////////////////////////////
            /////// from here on we have our devices and Miners initialized
            ConfigManager.AfterDeviceQueryInitialization();
            _loadingScreen.IncreaseLoadCounterAndMessage(International.GetText("Form_Main_loadtext_SaveConfig"));
            //Thread.Sleep(200);
            // All devices settup should be initialized in AllDevices
            devicesListViewEnableControl1.ResetComputeDevices(ComputeDeviceManager.Available.Devices);
            // set properties after
            devicesListViewEnableControl1.SaveToGeneralConfig = true;
            new Task(() => CheckGithubDownload()).Start();
            _loadingScreen.SetInfoMsg("Set firewall rules");
            new Task(() => Firewall.AddToFirewall()).Start();
           // Firewall.AddToFirewall();

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
           // Thread.Sleep(200);
            _loadingScreen.IncreaseLoadCounterAndMessage(
                International.GetText("Form_Main_loadtext_SetWindowsErrorReporting"));


            Thread.Sleep(10);
            Helpers.DisableWindowsErrorReporting(ConfigManager.GeneralConfig.DisableWindowsErrorReporting);

           // _loadingScreen.IncreaseLoadCounter();
            if (ConfigManager.GeneralConfig.NVIDIAP0State)
            {
                _loadingScreen.SetInfoMsg(International.GetText("Form_Main_loadtext_NVIDIAP0State"));
                Helpers.SetNvidiaP0State();
            }

            _loadingScreen.IncreaseLoadCounterAndMessage(International.GetText("Form_Main_loadtext_CheckLatestVersion"));

            new Task(() => CheckUpdates()).Start();
            new Task(() => ResetProtocols()).Start();

            thisComputer = new OpenHardwareMonitor.Hardware.Computer();
            thisComputer.GPUEnabled = true;
            thisComputer.CPUEnabled = true;
            thisComputer.Open();

            _loadingScreen.IncreaseLoadCounterAndMessage(International.GetText("Form_Main_loadtext_GetNiceHashSMA"));
            // Init ws connection

            NiceHashStats.StartConnection(Links.NhmSocketAddress);

            _loadingScreen.IncreaseLoadCounterAndMessage(International.GetText("Form_Main_loadtext_GetBTCRate"));
            Thread.Sleep(10);

            firstStartConnection = true;
            var runVCRed = !MinersExistanceChecker.IsMinersBinsInit() && !ConfigManager.GeneralConfig.DownloadInit;

            if (!MinersExistanceChecker.IsMinersBinsInit())
            {
                 var result = Utils.MessageBoxEx.Show(International.GetText("Form_Main_bins_folder_files_missing"),
                       International.GetText("Warning_with_Exclamation"),
                     MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 5000);
                if (result == DialogResult.Yes)
                {
                    ConfigManager.GeneralConfigFileCommit();
                    var downloadUnzipForm = new Form_Loading(new MinersDownloader(MinersDownloadManager.MinersDownloadSetup));
                    SetChildFormCenter(downloadUnzipForm);
                    downloadUnzipForm.ShowDialog();
                }
            }
            else
            {
                // all good
                ConfigManager.GeneralConfig.DownloadInit = true;
                ConfigManager.GeneralConfigFileCommit();
            }

            _loadingScreen.IncreaseLoadCounterAndMessage("Check VC redistributable");
            InstallVcRedist();
            _loadingScreen.FinishLoad();

            _AutoStartMiningDelay = ConfigManager.GeneralConfig.AutoStartMiningDelay;
            _autostartTimerDelay = new Timer();
            _autostartTimerDelay.Tick += AutoStartTimer_TickDelay;
            _autostartTimerDelay.Interval = 1000;
            _autostartTimerDelay.Start();

            _autostartTimer = new Timer();
            _autostartTimer.Tick += AutoStartTimer_Tick;
            _autostartTimer.Interval = Math.Max(2000, ConfigManager.GeneralConfig.AutoStartMiningDelay * 1000);
            _autostartTimer.Start();

            //Form_Main.ActiveForm.TopMost = true;
            if (ConfigManager.GeneralConfig.AlwaysOnTop) this.TopMost = true;

        }
        private bool IsVcRedistInstalled()
        {

            // x64 - 14.24.28127
            const int minMajor = 14;
            const int minMinor = 23;
            try
            {
                using (var vcredist = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\14.0\VC\Runtimes\x64"))
                {
                    var major = Int32.Parse(vcredist.GetValue("Major")?.ToString());
                    var minor = Int32.Parse(vcredist.GetValue("Minor")?.ToString());
                    if (major < minMajor) return false;
                    if (minor < minMinor) return false;
                    return true;
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("VcRedist", e.Message);
            }
            return false;
        }

        public void InstallVcRedist()
        {
            if (IsVcRedistInstalled())
            {
                return;
            }
            try
            {
                var vcredistProcess = new Process

                {
                    StartInfo =
                {
                    FileName = "miners//vc_redist.x64.exe"
                }
                };

                vcredistProcess.StartInfo.Arguments = "/install /passive /norestart";
                vcredistProcess.StartInfo.UseShellExecute = false;
                vcredistProcess.StartInfo.CreateNoWindow = false;
                vcredistProcess.Start();
                vcredistProcess.WaitForExit();

            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("VcRedist", e.Message);
            }
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
                    buttonStopMining.Refresh();
                    return;
                }
                else
                {
                    //buttonStartMining.Enabled = false;
                    buttonStopMining.Enabled = true;
                    buttonBTC_Clear.Enabled = false;
                    buttonStopMining.Text = International.GetText("Form_Main_stop") + " (" + _AutoStartMiningDelay.ToString() + ")";
                    buttonStartMining.Update();
                }
            }
            else
            {
                buttonStopMining.Enabled = false;
                buttonBTC_Clear.Enabled = true;
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
                    //this.Height = ConfigManager.GeneralConfig.FormHeight;
                    this.Height = this.MinimumSize.Height + ConfigManager.GeneralConfig.DevicesCountIndex * 17 + 1;
                } else
                {
                   // this.Width = 660; // min width
                }
            }
            if (!Configs.ConfigManager.GeneralConfig.MinimizeToTray)
            {
                WindowState = FormWindowState.Normal;
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

            buttonBTC_Save.FlatStyle = FlatStyle.Flat;
            buttonBTC_Save.FlatAppearance.BorderSize = 0;
            buttonBTC_Save.FlatAppearance.MouseOverBackColor = _backColor;
            buttonBTC_Clear.FlatStyle = FlatStyle.Flat;
            buttonBTC_Clear.FlatAppearance.BorderSize = 0;
            buttonBTC_Clear.FlatAppearance.MouseOverBackColor = _backColor;

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

                buttonBTC_Save.FlatStyle = FlatStyle.Flat;
                buttonBTC_Save.FlatAppearance.BorderSize = 0;
                buttonBTC_Save.UseVisualStyleBackColor = false;

                buttonBTC_Clear.FlatStyle = FlatStyle.Flat;
                buttonBTC_Clear.FlatAppearance.BorderSize = 0;
                buttonBTC_Clear.UseVisualStyleBackColor = false;

                foreach (var lbl in this.Controls.OfType<CheckBox>()) lbl.BackColor = _backColor;
                // DevicesListViewEnableControl.listViewDevices.BackColor = _backColor;
                devicesListViewEnableControl1.BackColor = _backColor;
                devicesListViewEnableControl1.ForeColor = _foreColor;
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

            _updateTimer = new Timer();
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Interval = 1000 * 60;//1 min
            _updateTimerCount = 0;
            _updateTimer.Start();


        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            _updateTimerCount++;
            int period = 0;
            switch (ConfigManager.GeneralConfig.ProgramUpdateIndex)
            {
                case 0:
                    period = 60;
                    break;
                case 1:
                    period = 180;
                    break;
                case 2:
                    period = 360;
                    break;
                case 3:
                    period = 720;
                    break;
                case 4:
                    period = 1140;
                    break;
            }

            if (_updateTimerCount >= period)
            {
                /*
                if (ConfigManager.GeneralConfig.PeriodicalReconnect)
                {
                    try
                    {
                        if (NiceHashSocket._webSocket != null)
                        {
                            Helpers.ConsolePrint("SOCKET", "Periodical reconnect");
                            NiceHashSocket._webSocket.Close();
                        }
                    } catch (Exception ex)
                    {
                        Helpers.ConsolePrint("SOCKET", "Periodical reconnect error: " + ex.ToString());
                    }
                }
                */
                _updateTimerCount = 0;
                bool newver = false;
                try
                {
                    newver = CheckGithub();
                }
                catch (Exception er)
                {
                    Helpers.ConsolePrint("CheckGithub", er.ToString());
                    return;
                }

                if (ConfigManager.GeneralConfig.ProgramAutoUpdate && newver)
                {
                    Updater.Updater.Downloader(true);
                }
            }
            //***********
            _updateTimerRestartProgramCount++;
            int periodRestartProgram = 0;
            switch (ConfigManager.GeneralConfig.ProgramRestartIndex)
            {
                case 0:
                    periodRestartProgram = -1;
                    break;
                case 1:
                    periodRestartProgram = 12;
                    break;
                case 2:
                    periodRestartProgram = 24;
                    break;
                case 3:
                    periodRestartProgram = 72;
                    break;
                case 4:
                    periodRestartProgram = 168;
                    break;
            }
            if (periodRestartProgram < 0) return;
            if (_updateTimerRestartProgramCount >= periodRestartProgram)
            {
                try
                {
                    MinersManager.StopAllMiners();
                    if (Miner._cooldownCheckTimer != null && Miner._cooldownCheckTimer.Enabled) Miner._cooldownCheckTimer.Stop();
                    MessageBoxManager.Unregister();
                    ConfigManager.GeneralConfigFileCommit();
                    try
                    {
                        if (File.Exists("TEMP\\github.test")) File.Delete("TEMP\\github.test");
                    }
                    catch (Exception ex)
                    {

                    }
                    //stop openhardwaremonitor
                    var CMDconfigHandleOHM = new Process

                    {
                        StartInfo =
                        {
                            FileName = "sc.exe"
                        }
                    };

                    CMDconfigHandleOHM.StartInfo.Arguments = "stop winring0_1_2_0";
                    CMDconfigHandleOHM.StartInfo.UseShellExecute = false;
                    CMDconfigHandleOHM.StartInfo.CreateNoWindow = true;
                    CMDconfigHandleOHM.Start();

                    if (GetWinVer(Environment.OSVersion.Version) == 10)
                    {
                        var CMDconfigHandleWD = new Process

                        {
                            StartInfo =
                            {
                                FileName = "sc.exe"
                            }
                        };

                        CMDconfigHandleWD.StartInfo.Arguments = "stop WinDivert1.4";
                        CMDconfigHandleWD.StartInfo.UseShellExecute = false;
                        CMDconfigHandleWD.StartInfo.CreateNoWindow = true;
                        CMDconfigHandleWD.Start();
                    }
                    Thread.Sleep(500);
                    var RestartProgram = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\RestartProgram.cmd")
                    {
                        WindowStyle = ProcessWindowStyle.Minimized
                    };
                    Helpers.ConsolePrint("SheduleRestart", "Shedule restart program after " + (periodRestartProgram/60).ToString() + "h");
                    Process.Start(RestartProgram);
                }
                catch (Exception er)
                {
                    Helpers.ConsolePrint("SheduleRestart", er.ToString());
                    return;
                }
            }
        }
        public bool CheckGithub()
        {
            Helpers.ConsolePrint("GITHUB", "Check new version");
                Helpers.ConsolePrint("GITHUB", "Current version: " + Form_Main.currentVersion.ToString());
                Helpers.ConsolePrint("GITHUB", "Current build: " + Form_Main.currentBuild.ToString());
            bool ret = CheckNewVersion();
                Helpers.ConsolePrint("GITHUB", "GITHUB Version: " + Form_Main.githubVersion.ToString());
                Helpers.ConsolePrint("GITHUB", "GITHUB Build: " + Form_Main.githubBuild.ToString());
            //SetVersion(ghv);
            return ret;
        }
        private bool CheckNewVersion()
        {
            bool ret = false;
            string githubVersion = Updater.Updater.GetVersion().Item1;
            Double.TryParse(githubVersion.ToString(), out Form_Main.githubVersion);
            Form_Main.githubBuild = Updater.Updater.GetVersion().Item2;
            if (linkLabelNewVersion != null)
            {
                if (Form_Main.currentBuild < Form_Main.githubBuild)//testing
                {
                    Form_Main.NewVersionExist = true;
                    linkLabelNewVersion.Text = (string.Format(International.GetText("Form_Main_new_build_released").Replace("{0}", "{0}"), ""));
                    ret = true;
                }
                if (Form_Main.currentVersion < Form_Main.githubVersion)
                {
                    Form_Main.NewVersionExist = true;
                    linkLabelNewVersion.Text = (string.Format(International.GetText("Form_Main_new_version_released").Replace("v{0}", "{0}"), "Fork Fix " + Form_Main.githubVersion.ToString()));
                    ret = true;
                }
                if (Form_Main.githubVersion <= 0)
                {
                    Form_Main.NewVersionExist = false;
                    ret = false;
                }
            }
            return ret;
        }
        private async void MinerStatsCheck_Tick(object sender, EventArgs e)
        {
            await MinersManager.MinerStatsCheck();
        }

        private static void ComputeDevicesCheckTimer_Tick(object sender, EventArgs e)
        {
            bool check = ComputeDeviceManager.Query.CheckVideoControllersCountMismath();
            if (check && CheckVideoControllersCount)
            {
                // less GPUs than before, ACT!
                try
                {
                    if (ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost)
                    {
                        var onGpusLost = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\OnGPUsLost.bat")
                        {
                            WindowStyle = ProcessWindowStyle.Minimized
                        };
                        onGpusLost.Arguments = "1";
                        Helpers.ConsolePrint("ERROR", "Restart Windows due CUDA GPU is lost");
                        Process.Start(onGpusLost);
                    }
                    if (ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost)
                    {
                        var onGpusLost = new ProcessStartInfo(Directory.GetCurrentDirectory() + "\\OnGPUsLost.bat")
                        {
                            WindowStyle = ProcessWindowStyle.Minimized
                        };
                        onGpusLost.Arguments = "2";
                        Helpers.ConsolePrint("ERROR", "Restart driver due CUDA GPU is lost");
                        Process.Start(onGpusLost);
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("NICEHASH", "OnGPUsLost.bat error: " + ex.Message);
                }
            }
                CheckVideoControllersCount = check;
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
            if (flowLayoutPanelRates == null) return;
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
            double panelHeight = 0;
            var groupBox1Height = _emtpyGroupPanelHeight;
            if (flowLayoutPanelRates.Controls.Count > 0)
            {
                var control = flowLayoutPanelRates.Controls[0];
                panelHeight = (int)((GroupProfitControl)control).Size.Height * 1.1;
                groupBox1Height = (int)((visibleGroupCount) * panelHeight - panelHeight / 3.0f);
            }
           // MiningSession._runningGroupMiners = null;
            groupBox1.Size = new Size(groupBox1.Size.Width, groupBox1Height);

            groupBox1.Top = groupBox1Top;
            // set new height
            int newHeight = _mainFormHeight + groupBox1Height - (int)panelHeight / 2;
            //this.MaximumSize = new Size(-1, newHeight);
           // Form_Main.ActiveForm.MinimumSize.Height = newHeight;
            Size = new Size(Size.Width, newHeight + ConfigManager.GeneralConfig.DevicesCountIndex * 17 + 1);
        }


        public void AddRateInfo(string groupName, string deviceStringInfo, ApiData iApiData, double paying, double power,
           DateTime StartMinerTime, bool isApiGetException, string processTag)
        {
            var apiGetExceptionString = isApiGetException ? " **" : "";

            var speedString =
                Helpers.FormatDualSpeedOutput(iApiData.Speed, iApiData.SecondarySpeed, iApiData.AlgorithmID) +
                iApiData.AlgorithmName + apiGetExceptionString;
            //power = 0;
            var rateBtcString = FormatPayingOutput(paying, power);
            if (!ConfigManager.GeneralConfig.DecreasePowerCost)
            {
                power = 0;
            }

            var rateCurrencyString = ExchangeRateApi
                                         .ConvertToActiveCurrency((paying - power) * ExchangeRateApi.GetUsdExchangeRate() * _factorTimeUnit)
                                         .ToString("F2", CultureInfo.InvariantCulture)
                                     + $" {ExchangeRateApi.ActiveDisplayCurrency}/" +
                                     International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());

            try
            {
                // flowLayoutPanelRatesIndex may be OOB, so catch
                ((GroupProfitControl)flowLayoutPanelRates.Controls[_flowLayoutPanelRatesIndex++])
                    .UpdateProfitStats(groupName, deviceStringInfo, speedString, StartMinerTime, rateBtcString, rateCurrencyString, processTag);
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

            try
            {
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
            catch (Exception e)
            {
                Helpers.ConsolePrint("Exception: ", e.ToString());
            }
        }

        public void ForceMinerStatsUpdate()
        {
            try
            {
                new Task(() => MinerStatsCheck_Tick(null, null));
                // BeginInvoke((Action)(() =>
                //{
               // MinerStatsCheck_Tick(null, null);
               //}));
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("NiceHash", e.ToString());
            }
        }

        private void UpdateGlobalRate()
        {
            try
            {
                double psuE = (double)ConfigManager.GeneralConfig.PowerPSU / 100;
                var totalRate = MinersManager.GetTotalRate();
                //var totalPowerRate = MinersManager.GetTotalPowerRate();
                var powerString = "";
                double TotalPower = 0;
                foreach (var computeDevice in Available.Devices)
                {
                    TotalPower += computeDevice.PowerUsage;
                }
                double totalPower = (TotalPower + (int)ConfigManager.GeneralConfig.PowerMB) / psuE;
                totalPower = Math.Round(totalPower, 0);
                var totalPowerRate = ExchangeRateApi.GetKwhPriceInBtc() * totalPower * 24 * _factorTimeUnit / 1000;
                var PowerRateFiat = ExchangeRateApi.GetKwhPriceInBtc() * ExchangeRateApi.GetUsdExchangeRate() * totalPower * 24 * _factorTimeUnit / 1000;

                var powerMB = ExchangeRateApi.GetKwhPriceInBtc() * totalPower * 24 / 1000;

                double totalPowerRateDec = 0;
                if (ConfigManager.GeneralConfig.DecreasePowerCost)
                {
                    totalPowerRateDec = totalPowerRate;
                }

                if (ConfigManager.GeneralConfig.AutoScaleBTCValues && totalRate < 0.1)
                {
                    if (totalPowerRate != 0)
                    {
                        powerString = "(-" + (totalPowerRate * 1000 * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture) + ") ";
                    }
                    if (ConfigManager.GeneralConfig.DecreasePowerCost)
                    {
                        powerString = "";
                    }
                    toolStripStatusLabelBTCDayText.Text = powerString + " " +
                    "mBTC/" + International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
                    toolStripStatusLabelGlobalRateValue.Text =
                ((totalRate - totalPowerRateDec) * 1000 * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture);

                }
                else
                {
                    if (totalPowerRate != 0)
                    {
                        powerString = "(-" + (totalPowerRate * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture) + ") ";
                    }
                    if (ConfigManager.GeneralConfig.DecreasePowerCost)
                    {
                        powerString = "";
                    }
                    toolStripStatusLabelBTCDayText.Text = powerString + " " +
                        "BTC/" + International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
                    toolStripStatusLabelGlobalRateValue.Text =
                        ((totalRate - totalPowerRateDec) * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture);
                }

                if (totalPowerRate != 0)
                {
                    powerString = "(-" + ExchangeRateApi.ConvertToActiveCurrency((totalPowerRate * _factorTimeUnit * ExchangeRateApi.GetUsdExchangeRate()))
                    .ToString("F2", CultureInfo.InvariantCulture) + ") ";
                    if (ConfigManager.GeneralConfig.DecreasePowerCost)
                    {
                        powerString = "";//!!!!!
                    }
                }
                else
                {
                    powerString = "";
                }
                toolStripStatusLabel_power1.Text = International.GetText("Form_Main_Power1");
                toolStripStatusLabel_power3.Text = International.GetText("Form_Main_Power3");
                toolStripStatusLabel_power2.Text = totalPower.ToString();

                toolStripStatusLabelBTCDayValue.Text = ExchangeRateApi
                    .ConvertToActiveCurrency(((totalRate - totalPowerRateDec) * _factorTimeUnit * ExchangeRateApi.GetUsdExchangeRate()))
                    .ToString("F2", CultureInfo.InvariantCulture);
                toolStripStatusLabelBalanceText.Text = powerString + (ExchangeRateApi.ActiveDisplayCurrency + "/") +
                                                       International.GetText(
                                                           ConfigManager.GeneralConfig.TimeUnit.ToString()) + "   " +
                                                       International.GetText("Form_Main_balance") + ":";
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("UpdateGlobalRate error: ", e.ToString());
            }
        }


        private void BalanceCallback()
        {
            try
            {
                //Helpers.ConsolePrint("NICEHASH", "Balance update");
                var balance = NiceHashStats.Balance;
                //if (balance > 0)

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
                    var amountUsd = (balance * ExchangeRateApi.GetUsdExchangeRate());
                    var amount = ExchangeRateApi.ConvertToActiveCurrency(amountUsd);
                    toolStripStatusLabelBalanceDollarText.Text = amount.ToString("F2", CultureInfo.InvariantCulture);
                    toolStripStatusLabelBalanceDollarValue.Text = $"({ExchangeRateApi.ActiveDisplayCurrency})";

            } catch (Exception ex)
            {
                Helpers.ConsolePrint("Balance update", ex.ToString());
            }
            //Helpers.ConsolePrint("NICEHASH", "Balance updated");
        }


        //private void BitcoinExchangeCheck_Tick(object sender, EventArgs e)
        //{
        //    Helpers.ConsolePrint("NICEHASH", "Bitcoin rate get");
        //    ExchangeRateApi.UpdateApi(textBoxWorkerName.Text.Trim());
        //    UpdateExchange();
        //}

        private void ExchangeCallback()
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

          //  Helpers.ConsolePrint("NICEHASH",
            //    "Current Bitcoin rate: " + br.ToString("F2", CultureInfo.InvariantCulture));
        }


        private bool VerifyMiningAddress(bool showError)
        {
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
            if (true)
            {
                Process.Start(Links.CheckStatsNew + textBoxBTCAddress_new.Text.Trim());
            }
        }


        private void LinkLabelChooseBTCWallet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
                Process.Start(Links.NhmBtcWalletFaqNew);
        }

        private void LinkLabelNewVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //Process.Start(_visitUrlNew);
            var settings = new Form_Settings();
            try
            {
                //   SetChildFormCenter(settings);
                settings.tabControlGeneral.SelectedTab = settings.tabPageAbout;
                settings.ShowDialog();
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("settings", er.ToString());
            }
        }

        static int GetWinVer(Version ver)
        {
            if (ver.Major == 6 & ver.Minor == 1)
                return 7;
            else if (ver.Major == 6 & ver.Minor == 2)
                return 8;
            else
                return 10;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            devicesListViewEnableControl1.SaveColumns();
            if (this != null)
            {
                if (ConfigManager.GeneralConfig.Save_windows_size_and_position)
                {
                    ConfigManager.GeneralConfig.FormWidth = this.Width;
                    ConfigManager.GeneralConfig.FormHeight = this.Height;
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
            try
            {
                if (File.Exists("TEMP\\github.test")) File.Delete("TEMP\\github.test");
            } catch (Exception ex)
            {

            }
            //stop openhardwaremonitor
            var CMDconfigHandleOHM = new Process

            {
                StartInfo =
                {
                    FileName = "sc.exe"
                }
            };

            CMDconfigHandleOHM.StartInfo.Arguments = "stop winring0_1_2_0";
            CMDconfigHandleOHM.StartInfo.UseShellExecute = false;
            CMDconfigHandleOHM.StartInfo.CreateNoWindow = true;
            CMDconfigHandleOHM.Start();

            if (GetWinVer(Environment.OSVersion.Version) == 10)
            {
                var CMDconfigHandleWD = new Process

                {
                    StartInfo =
                {
                    FileName = "sc.exe"
                }
                };

                CMDconfigHandleWD.StartInfo.Arguments = "stop WinDivert1.4";
                CMDconfigHandleWD.StartInfo.UseShellExecute = false;
                CMDconfigHandleWD.StartInfo.CreateNoWindow = true;
                CMDconfigHandleWD.Start();
            }
            //mainproc = Process.GetCurrentProcess();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + mainproc.Id.ToString());
            ManagementObjectCollection moc = searcher.Get();
               // Helpers.ConsolePrint("Closing", moc.Count.ToString());
                foreach (ManagementObject mo in moc)
                {
                    try
                    {
                        Process proc = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
                        //Helpers.ConsolePrint("Closing", Convert.ToInt32(mo["ProcessID"]).ToString() + " " + proc.ProcessName);
                    if (!Convert.ToInt32(mo["ProcessID"]).ToString().Contains("NiceHashMinerLegacy"))
                    {
                        if (proc != null)
                        {
                            proc.Kill();
                        }
                    }
                    }
                    catch (Exception ex)
                    {
                    Helpers.ConsolePrint("Closing", ex.ToString());
                    }
                }
                Process mproc = Process.GetProcessById(mainproc.Id);
            Helpers.ConsolePrint("Closing", mproc.Id.ToString() + " " + mproc.ProcessName);
            mproc.Kill();
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
            try
            {
                //   SetChildFormCenter(settings);
                settings.ShowDialog();
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("settings", er.ToString());
            }
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
            DHClient.checkConnection = false;
            firstRun = true;
            _isManuallyStarted = false;
            //AlgorithmSwitchingManager._smaCheckTimer.Enabled = false;
            StopMining();
        }

        private string FormatPayingOutput(double paying, double power)
        {
            string ret;
            if (!ConfigManager.GeneralConfig.DecreasePowerCost)
            {
                power = 0;
            }

            if (ConfigManager.GeneralConfig.AutoScaleBTCValues && paying < 0.1)
                ret = ((paying - power) * 1000 * _factorTimeUnit).ToString("F5", CultureInfo.InvariantCulture) +
                    " mBTC/" +
                      International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
            else
                ret = ((paying - power) * _factorTimeUnit).ToString("F6", CultureInfo.InvariantCulture) +
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
                Process.Start(Links.NhmPayingFaqNew);
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

            foreach (var control in flowLayoutPanelRates.Controls)
            {
                ((GroupProfitControl)control).Width = this.Width - 145;
            }
            //((GroupProfitControl)control).Width = 520;


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

            if (true)
            {
                NiceHashStats._deviceUpdateTimer.Stop();
                new Task(() => NiceHashStats.SetDeviceStatus("MINING")).Start();
                NiceHashStats._deviceUpdateTimer.Start();
                //NiceHashStats.SetDeviceStatus("MINING");
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
                            NiceHashStats._deviceUpdateTimer.Stop();
                            new Task(() => NiceHashStats.SetDeviceStatus("STOPPED")).Start();
                            NiceHashStats._deviceUpdateTimer.Start();
                            //NiceHashStats.SetDeviceStatus("STOPPED");
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
                    NiceHashStats._deviceUpdateTimer.Stop();
                    new Task(() => NiceHashStats.SetDeviceStatus("STOPPED")).Start();
                    NiceHashStats._deviceUpdateTimer.Start();
                    //NiceHashStats.SetDeviceStatus("STOPPED");
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
                    NiceHashStats._deviceUpdateTimer.Stop();
                    new Task(() => NiceHashStats.SetDeviceStatus("STOPPED")).Start();
                    NiceHashStats._deviceUpdateTimer.Start();
                    //NiceHashStats.SetDeviceStatus("STOPPED");
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
            buttonBTC_Clear.Enabled = false;

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

            if (true)
            {
                 btcAdress = _demoMode ? Globals.DemoUser : textBoxBTCAddress_new.Text.Trim();
            }

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
            NiceHashStats._deviceUpdateTimer.Stop();
            new Task(() => NiceHashStats.SetDeviceStatus("MINING")).Start();
            NiceHashStats._deviceUpdateTimer.Start();
            //NiceHashStats.SetDeviceStatus("MINING");
            if (ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost || ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost)
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

        private void CheckDagger3GB()
        {
            if (ConfigManager.GeneralConfig.DivertRun)
            {
                if (Divert.DaggerHashimoto3GBForce)
                {
                    if (Divert.DaggerHashimoto3GBProfit && DHClient.checkConnection)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Force switch ON");
                        NHSmaData.TryGetPaying(AlgorithmType.DaggerHashimoto, out var paying);
                        NHSmaData.UpdatePayingForAlgo(AlgorithmType.DaggerHashimoto3GB, paying);
                        NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
                        Divert.Dagger3GBEpochCount = 0;
                        Divert.DaggerHashimoto3GBForce = false;
                        DHClient.checkConnection = false;
                        //DHClient.needStart = false;
                        //new Task(() => DHClient.StopConnection()).Start();
                    }
                    if (Divert.Dagger3GBEpochCount > 1 && !DHClient.checkConnection)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Force switch OFF");
                        NHSmaData.UpdatePayingForAlgo(AlgorithmType.DaggerHashimoto3GB, 0.0d);
                        NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
                        Divert.DaggerHashimoto3GBForce = false;
                        DHClient.checkConnection = true;
                        //new Task(() => DHClient.StartConnection()).Start();
                    }
                    //DHClient.needStart = false;
                }
            }
        }
        public static string DNStoIP(string IPName)
        {
            try
            {
                var ASCII = new System.Text.ASCIIEncoding();
                var heserver = Dns.GetHostEntry(IPName);
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
                    {
                        return curAdd.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("Exception: " + e.ToString());
            }
            return "";
        }

        private void DeviceStatusTimer_Tick(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.ShowUptime)
            {
                var timenow = DateTime.Now;
                Uptime = timenow.Subtract(StartTime);
                label_Uptime.Visible = true;
                label_Uptime.Text = International.GetText("Form_Main_Uptime") + " " + Uptime.ToString(@"d\ \d\a\y\s\ hh\:mm\:ss");
            }
            if (Form_Main.thisComputer != null)
            {
                foreach (var hardware in Form_Main.thisComputer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.GpuAti || hardware.HardwareType == HardwareType.GpuNvidia)
                    {
                        hardware.Update();
                    }
                }
            }
            if (DeviceStatusTimer_FirstTick) CheckDagger3GB();
            DeviceStatusTimer_FirstTick = true;
            ExchangeCallback();
            BalanceCallback();
            UpdateGlobalRate();

            if (needRestart)
            {
                needRestart = false;
                restartProgram();
            }
            devicesListViewEnableControl1.SetComputeDevicesStatus(ComputeDeviceManager.Available.Devices);
        }
        private void StopMining()
        {
            AlgorithmSwitchingManager.Stop();
            NiceHashStats._deviceUpdateTimer.Stop();
            new Task(() => NiceHashStats.SetDeviceStatus("STOPPED")).Start();
            NiceHashStats._deviceUpdateTimer.Stop();
            //NiceHashStats.SetDeviceStatus("PENDING");
            _minerStatsCheck.Stop();
            //_smaMinerCheck.Stop();
            _computeDevicesCheckTimer?.Stop();

            // Disable IFTTT notification before label call
            _isNotProfitable = false;

            MinersManager.StopAllMiners();
            MiningSession.FuncAttached = false;
            textBoxBTCAddress_new.Enabled = true;
           // textBoxBTCAddress.Enabled = true;
            textBoxWorkerName.Enabled = true;
            comboBoxLocation.Enabled = true;
            buttonBenchmark.Enabled = true;
            buttonStartMining.Enabled = true;
            buttonSettings.Enabled = true;
            devicesListViewEnableControl1.IsMining = false;
            buttonStopMining.Enabled = false;
            buttonBTC_Clear.Enabled = true;

            if (_demoMode)
            {
                _demoMode = false;
                labelDemoMode.Visible = false;
            }

            UpdateGlobalRate();
            //toolStripStatusLabel_power1.Text = "";
            //toolStripStatusLabel_power2.Text = "";
            //toolStripStatusLabel_power3.Text = "";
            //devicesListViewEnableControl1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            //devicesListViewEnableControl1.Update();
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
            /*
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0 && _autostartTimer == null)
            {
                buttonStopMining.ResetText();
                Button btn = (Button)sender;
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;   // center the text
                TextRenderer.DrawText(e.Graphics, International.GetText("Form_Main_stop"), btn.Font, e.ClipRectangle, btn.ForeColor, flags);
            }
            */
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
            /*
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0 && Form_Main.ActiveForm.Enabled == true)
            {
                buttonStartMining.ResetText();
                Button btn = (Button)sender;
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix;   // center the text
                TextRenderer.DrawText(e.Graphics, International.GetText("Form_Main_start"), btn.Font, e.ClipRectangle, btn.ForeColor, flags);

            }
            */
        }

        private void devicesListViewEnableControl1_Resize(object sender, EventArgs e)
        {

        }

        private void Form_Main_ResizeEnd(object sender, EventArgs e)
        {
           // _mainFormHeight = Size.Height;
            //_mainFormHeight = Size.Height - _emtpyGroupPanelHeight;
        }

        private void buttonBTC_Clear_Click(object sender, EventArgs e)
        {
            //Form_Main.ActiveForm.Focus();
            buttonBTC_Clear.ForeColor = Form_Main._backColor;
            var result = MessageBox.Show(dialogClearBTC,"", MessageBoxButtons.YesNo, MessageBoxIcon.Question,

                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                buttonBTC_Clear.Enabled = false;
                textBoxBTCAddress_new.Text = "";
                ConfigManager.GeneralConfig.BitcoinAddressNew = textBoxBTCAddress_new.Text.Trim();
                textBoxBTCAddress_new.Update();
                NiceHashStats.SetCredentials(textBoxBTCAddress_new.Text.Trim(), textBoxWorkerName.Text.Trim());
                NiceHashStats.StartConnection(Links.NhmSocketAddress);
            }
        }

        private void buttonBTC_Save_Click(object sender, EventArgs e)
        {
            if (!BitcoinAddress.ValidateBitcoinAddress(textBoxBTCAddress_new.Text.Trim()) && textBoxBTCAddress_new.Text.Length !=0)
            {
                var result = MessageBox.Show(International.GetText("Form_Main_msgbox_InvalidBTCAddressMsg"),
                    International.GetText("Error_with_Exclamation"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    Process.Start(Links.NhmBtcWalletFaqNew);
                    //textBoxBTCAddress_new.Text = "";
                }

                textBoxBTCAddress_new.Focus();

            } else
            {
                ConfigManager.GeneralConfig.BitcoinAddressNew = textBoxBTCAddress_new.Text.Trim();
                buttonBTC_Save.Enabled = false;
            }
            NiceHashStats.SetCredentials(textBoxBTCAddress_new.Text.Trim(), textBoxWorkerName.Text.Trim());
            NiceHashStats.StartConnection(Links.NhmSocketAddress);
        }

        private void textBoxBTCAddress_new_TextChanged(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.BitcoinAddressNew != textBoxBTCAddress_new.Text.Trim())
            {
                buttonBTC_Clear.Enabled = true;
                buttonBTC_Save.Enabled = true;
            } else
            {
                buttonBTC_Save.Enabled = false;
            }

            if (textBoxBTCAddress_new.Text == "")
            {
                buttonBTC_Clear.Enabled = false;
            }
        }

        private void buttonBTC_Save_MouseMove(object sender, MouseEventArgs e)
        {
            buttonBTC_Save.Image = Properties.Resources.Ok_hot;
        }

        private void buttonBTC_Save_MouseLeave(object sender, EventArgs e)
        {
            buttonBTC_Save.Image = Properties.Resources.Ok_normal;
        }

        private void buttonBTC_Save_Paint(object sender, PaintEventArgs e)
        {
        }

        private void buttonBTC_Clear_MouseMove(object sender, MouseEventArgs e)
        {
            buttonBTC_Clear.Image = Properties.Resources.Close_hot;
        }

        private void buttonBTC_Clear_MouseLeave(object sender, EventArgs e)
        {
            buttonBTC_Clear.Image = Properties.Resources.Close_normal;
        }

        private void buttonBTC_Clear_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonBTC_Clear_MouseDown(object sender, MouseEventArgs e)
        {

        }
    }


}
