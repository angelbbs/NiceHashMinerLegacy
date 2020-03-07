/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using Microsoft.Win32;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Miners.Parsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Windows.Forms;
using NiceHashMiner.Devices.Algorithms;
using NiceHashMiner.Stats;
using NiceHashMinerLegacy.Common.Enums;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Diagnostics;

namespace NiceHashMiner.Forms
{
    public partial class Form_Settings : Form
    {
        private readonly bool _isInitFinished = false;
        private bool _isChange = false;

        public bool IsChange
        {
            get => _isChange;
            private set => _isChange = _isInitFinished && value;
        }

        private bool _isCredChange = false;
        public bool IsChangeSaved { get; private set; }
        public bool IsRestartNeeded { get; private set; }

        // most likely we wil have settings only per unique devices
        private const bool ShowUniqueDeviceList = true;

        private ComputeDevice _selectedComputeDevice;

        private readonly RegistryKey _rkStartup;

        private bool _isStartupChanged = false;
        private static Timer UpdateListView_timer;

        public Form_Settings()
        {
            Process thisProc = Process.GetCurrentProcess();
            thisProc.PriorityClass = ProcessPriorityClass.High;

            InitializeComponent();
            Icon = Properties.Resources.logo;

            //ret = 1; // default
            IsChange = false;
            IsChangeSaved = false;

            // backup settings
            ConfigManager.CreateBackup();

            // initialize form
            InitializeFormTranslations();

            // Initialize toolTip
            InitializeToolTip();

            // Initialize tabs
            this.comboBox_ColorProfile.Items.Add("Default");
            this.comboBox_ColorProfile.Items.Add("Gray");
            this.comboBox_ColorProfile.Items.Add("Dark");
            this.comboBox_ColorProfile.Items.Add("Black&White");
            this.comboBox_ColorProfile.Items.Add("Silver");
            this.comboBox_ColorProfile.Items.Add("Gold");
            this.comboBox_ColorProfile.Items.Add("DarkRed");
            this.comboBox_ColorProfile.Items.Add("DarkGreen");
            this.comboBox_ColorProfile.Items.Add("DarkBlue");
            this.comboBox_ColorProfile.Items.Add("DarkMagenta");
            this.comboBox_ColorProfile.Items.Add("DarkOrange");
            this.comboBox_ColorProfile.Items.Add("DarkViolet");
            this.comboBox_ColorProfile.Items.Add("DarkSlateBlue");
            this.comboBox_ColorProfile.Items.Add("Tan");

            InitializeGeneralTab();

            // initialization calls
            InitializeDevicesTab();
            // link algorithm list with algorithm settings control
            algorithmSettingsControl1.Enabled = false;
            algorithmsListView1.ComunicationInterface = algorithmSettingsControl1;
            //algorithmsListView1.RemoveRatioRates();
            /*
            Form_Settings.comboBox_ColorProfile.Items.AddRange(new object[] {
            "Default",
            "Gray",
            "Dark",
            "Black",
            "DarkGreen"});
            */

            // set first device selected {
            if (ComputeDeviceManager.Available.Devices.Count > 0)
            {
                _selectedComputeDevice = ComputeDeviceManager.Available.Devices[0];
                algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
                groupBoxAlgorithmSettings.Text = string.Format(International.GetText("FormSettings_AlgorithmsSettings"),
                    _selectedComputeDevice.Name);
               // groupBoxAlgorithmSettings.ForeColor = Form_Main._foreColor;
            }

            // At the very end set to true
            _isInitFinished = true;

            try
            {
                _rkStartup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            }
            catch (SecurityException)
            {
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SETTINGS", e.ToString());
            }
            Form_Settings.ActiveForm.Update();
            if (ConfigManager.GeneralConfig.AlwaysOnTop) this.TopMost = true;

            if (UpdateListView_timer == null)
            {
                UpdateListView_timer = new Timer();
                UpdateListView_timer.Tick += UpdateLvi_Tick;
                UpdateListView_timer.Interval = 10000;
                UpdateListView_timer.Start();
            }

            thisProc.PriorityClass = ProcessPriorityClass.Normal;
            thisProc.Dispose();
        }
        private void UpdateLvi_Tick(object sender, EventArgs e)
        {
            algorithmsListView1.UpdateLvi();
        }

            #region Initializations

            private void InitializeToolTip()
        {
            // Setup Tooltips
            toolTip1.SetToolTip(comboBox_Language, International.GetText("Form_Settings_ToolTip_Language"));
            toolTip1.SetToolTip(label_Language, International.GetText("Form_Settings_ToolTip_Language"));
            toolTip1.SetToolTip(pictureBox_Language, International.GetText("Form_Settings_ToolTip_Language"));
            /*
            toolTip1.SetToolTip(checkBox_DebugConsole,
                International.GetText("Form_Settings_ToolTip_checkBox_DebugConsole"));
            toolTip1.SetToolTip(pictureBox_DebugConsole,
                International.GetText("Form_Settings_ToolTip_checkBox_DebugConsole"));
                */
            //toolTip1.SetToolTip(textBox_BitcoinAddress, International.GetText("Form_Settings_ToolTip_BitcoinAddress"));
            /*
            toolTip1.SetToolTip(label_BitcoinAddressNew, International.GetText("Form_Settings_ToolTip_BitcoinAddress"));
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                //toolTip1.SetToolTip(label_BitcoinAddress, "Биткоин адрес (старая платформа)");
                toolTip1.SetToolTip(label_BitcoinAddressNew, "Биткоин адрес (новая платформа)");
            }
            toolTip1.SetToolTip(pictureBox_Info_BitcoinAddress,
                International.GetText("Form_Settings_ToolTip_BitcoinAddress"));

            toolTip1.SetToolTip(textBox_WorkerName, International.GetText("Form_Settings_ToolTip_WorkerName"));
            toolTip1.SetToolTip(label_WorkerName, International.GetText("Form_Settings_ToolTip_WorkerName"));
            toolTip1.SetToolTip(pictureBox_WorkerName, International.GetText("Form_Settings_ToolTip_WorkerName"));

            toolTip1.SetToolTip(comboBox_ServiceLocation,
                International.GetText("Form_Settings_ToolTip_ServiceLocation"));
            toolTip1.SetToolTip(label_ServiceLocation, International.GetText("Form_Settings_ToolTip_ServiceLocation"));
            toolTip1.SetToolTip(pictureBox_ServiceLocation,
                International.GetText("Form_Settings_ToolTip_ServiceLocation"));
*/
            toolTip1.SetToolTip(comboBox_TimeUnit, International.GetText("Form_Settings_ToolTip_TimeUnit"));
            toolTip1.SetToolTip(label_TimeUnit, International.GetText("Form_Settings_ToolTip_TimeUnit"));
            toolTip1.SetToolTip(pictureBox_TimeUnit, International.GetText("Form_Settings_ToolTip_TimeUnit"));

            toolTip1.SetToolTip(checkBox_HideMiningWindows,
                International.GetText("Form_Settings_ToolTip_checkBox_HideMiningWindows"));
            toolTip1.SetToolTip(pictureBox_HideMiningWindows,
                International.GetText("Form_Settings_ToolTip_checkBox_HideMiningWindows"));

            toolTip1.SetToolTip(checkBox_MinimizeToTray,
                International.GetText("Form_Settings_ToolTip_checkBox_MinimizeToTray"));
            toolTip1.SetToolTip(pictureBox_MinimizeToTray,
                International.GetText("Form_Settings_ToolTip_checkBox_MinimizeToTray"));
            /*
            toolTip1.SetToolTip(checkBox_Use3rdPartyMiners,
                International.GetText("Form_Settings_General_3rdparty_ToolTip"));
            toolTip1.SetToolTip(pictureBox_Use3rdPartyMiners,
                International.GetText("Form_Settings_General_3rdparty_ToolTip"));
                */
            toolTip1.SetToolTip(checkBox_AllowMultipleInstances,
                International.GetText("Form_Settings_General_AllowMultipleInstances_ToolTip"));
            toolTip1.SetToolTip(pictureBox_AllowMultipleInstances,
                International.GetText("Form_Settings_General_AllowMultipleInstances_ToolTip"));

            toolTip1.SetToolTip(label_MinProfit, International.GetText("Form_Settings_ToolTip_MinimumProfit"));
            toolTip1.SetToolTip(pictureBox_MinProfit, International.GetText("Form_Settings_ToolTip_MinimumProfit"));
            toolTip1.SetToolTip(textBox_MinProfit, International.GetText("Form_Settings_ToolTip_MinimumProfit"));

            toolTip1.SetToolTip(textBox_SwitchMaxSeconds,
                International.GetText("Form_Settings_ToolTip_SwitchMaxSeconds"));
            toolTip1.SetToolTip(label_SwitchMaxSeconds,
                International.GetText("Form_Settings_ToolTip_SwitchMaxSeconds"));
            toolTip1.SetToolTip(pictureBox_SwitchMaxSeconds,
                International.GetText("Form_Settings_ToolTip_SwitchMaxSeconds"));

            toolTip1.SetToolTip(textBox_SwitchMinSeconds,
                International.GetText("Form_Settings_ToolTip_SwitchMinSeconds"));
            toolTip1.SetToolTip(label_SwitchMinSeconds,
                International.GetText("Form_Settings_ToolTip_SwitchMinSeconds"));
            toolTip1.SetToolTip(pictureBox_SwitchMinSeconds,
                International.GetText("Form_Settings_ToolTip_SwitchMinSeconds"));
            /*
            toolTip1.SetToolTip(textBox_MinerAPIQueryInterval,
                International.GetText("Form_Settings_ToolTip_MinerAPIQueryInterval"));
            toolTip1.SetToolTip(label_MinerAPIQueryInterval,
                International.GetText("Form_Settings_ToolTip_MinerAPIQueryInterval"));
            toolTip1.SetToolTip(pictureBox_MinerAPIQueryInterval,
                International.GetText("Form_Settings_ToolTip_MinerAPIQueryInterval"));
                */
            toolTip1.SetToolTip(textBox_MinerRestartDelayMS,
                International.GetText("Form_Settings_ToolTip_MinerRestartDelayMS"));
            toolTip1.SetToolTip(label_MinerRestartDelayMS,
                International.GetText("Form_Settings_ToolTip_MinerRestartDelayMS"));
            toolTip1.SetToolTip(pictureBox_MinerRestartDelayMS,
                International.GetText("Form_Settings_ToolTip_MinerRestartDelayMS"));

            toolTip1.SetToolTip(textBox_APIBindPortStart,
                International.GetText("Form_Settings_ToolTip_APIBindPortStart"));
            toolTip1.SetToolTip(label_APIBindPortStart,
                International.GetText("Form_Settings_ToolTip_APIBindPortStart"));
            toolTip1.SetToolTip(pictureBox_APIBindPortStart,
                International.GetText("Form_Settings_ToolTip_APIBindPortStart"));
            /*
            toolTip1.SetToolTip(comboBox_DagLoadMode, International.GetText("Form_Settings_ToolTip_DagGeneration"));
            toolTip1.SetToolTip(label_DagGeneration, International.GetText("Form_Settings_ToolTip_DagGeneration"));
            toolTip1.SetToolTip(pictureBox_DagGeneration, International.GetText("Form_Settings_ToolTip_DagGeneration"));
            */
            /*
            benchmarkLimitControlCPU.SetToolTip(ref toolTip1, "CPUs");
            benchmarkLimitControlNVIDIA.SetToolTip(ref toolTip1, "NVIDIA GPUs");
            benchmarkLimitControlAMD.SetToolTip(ref toolTip1, "AMD GPUs");
            */
            toolTip1.SetToolTip(checkBox_DisableDetectionNVIDIA,
                string.Format(International.GetText("Form_Settings_ToolTip_checkBox_DisableDetection"), "NVIDIA"));
            toolTip1.SetToolTip(checkBox_DisableDetectionAMD,
                string.Format(International.GetText("Form_Settings_ToolTip_checkBox_DisableDetection"), "AMD"));
            toolTip1.SetToolTip(pictureBox_DisableDetectionNVIDIA,
                string.Format(International.GetText("Form_Settings_ToolTip_checkBox_DisableDetection"), "NVIDIA"));
            toolTip1.SetToolTip(pictureBox_DisableDetectionAMD,
                string.Format(International.GetText("Form_Settings_ToolTip_checkBox_DisableDetection"), "AMD"));

            toolTip1.SetToolTip(checkBox_AutoScaleBTCValues,
                International.GetText("Form_Settings_ToolTip_checkBox_AutoScaleBTCValues"));
            toolTip1.SetToolTip(pictureBox_AutoScaleBTCValues,
                International.GetText("Form_Settings_ToolTip_checkBox_AutoScaleBTCValues"));

            toolTip1.SetToolTip(checkBox_StartMiningWhenIdle,
                International.GetText("Form_Settings_ToolTip_checkBox_StartMiningWhenIdle"));
            toolTip1.SetToolTip(pictureBox_StartMiningWhenIdle,
                International.GetText("Form_Settings_ToolTip_checkBox_StartMiningWhenIdle"));

            toolTip1.SetToolTip(textBox_MinIdleSeconds, International.GetText("Form_Settings_ToolTip_MinIdleSeconds"));
            toolTip1.SetToolTip(label_MinIdleSeconds, International.GetText("Form_Settings_ToolTip_MinIdleSeconds"));
            toolTip1.SetToolTip(pictureBox_MinIdleSeconds,
                International.GetText("Form_Settings_ToolTip_MinIdleSeconds"));

            toolTip1.SetToolTip(checkBox_LogToFile, International.GetText("Form_Settings_ToolTip_checkBox_LogToFile"));
         //   toolTip1.SetToolTip(pictureBox_LogToFile,
           //     International.GetText("Form_Settings_ToolTip_checkBox_LogToFile"));


            toolTip1.SetToolTip(textBox_LogMaxFileSize, International.GetText("Form_Settings_ToolTip_LogMaxFileSize"));
            toolTip1.SetToolTip(label_LogMaxFileSize, International.GetText("Form_Settings_ToolTip_LogMaxFileSize"));
            toolTip1.SetToolTip(pictureBox_LogMaxFileSize,
                International.GetText("Form_Settings_ToolTip_LogMaxFileSize"));
            /*
            toolTip1.SetToolTip(checkBox_ShowDriverVersionWarning,
                International.GetText("Form_Settings_ToolTip_checkBox_ShowDriverVersionWarning"));
            toolTip1.SetToolTip(pictureBox_ShowDriverVersionWarning,
                International.GetText("Form_Settings_ToolTip_checkBox_ShowDriverVersionWarning"));
                */
            /*
            toolTip1.SetToolTip(checkBox_DisableWindowsErrorReporting,
                International.GetText("Form_Settings_ToolTip_checkBox_DisableWindowsErrorReporting"));
            toolTip1.SetToolTip(pictureBox_DisableWindowsErrorReporting,
                International.GetText("Form_Settings_ToolTip_checkBox_DisableWindowsErrorReporting"));
                */
            toolTip1.SetToolTip(checkBox_ShowInternetConnectionWarning,
                International.GetText("Form_Settings_ToolTip_checkBox_ShowInternetConnectionWarning"));
            toolTip1.SetToolTip(pictureBox_ShowInternetConnectionWarning,
                International.GetText("Form_Settings_ToolTip_checkBox_ShowInternetConnectionWarning"));

            toolTip1.SetToolTip(checkBox_NVIDIAP0State,
                International.GetText("Form_Settings_ToolTip_checkBox_NVIDIAP0State"));
            toolTip1.SetToolTip(pictureBox_NVIDIAP0State,
                International.GetText("Form_Settings_ToolTip_checkBox_NVIDIAP0State"));

            toolTip1.SetToolTip(checkBox_RunScriptOnCUDA_GPU_Lost,
                International.GetText("Form_Settings_ToolTip_checkBox_RunScriptOnCUDA_GPU_Lost"));
            toolTip1.SetToolTip(pictureBox_RunScriptOnCUDA_GPU_Lost,
                International.GetText("Form_Settings_ToolTip_checkBox_RunScriptOnCUDA_GPU_Lost"));

            toolTip1.SetToolTip(checkBox_RunAtStartup,
                International.GetText("Form_Settings_ToolTip_checkBox_RunAtStartup"));
            toolTip1.SetToolTip(pictureBox_RunAtStartup,
                International.GetText("Form_Settings_ToolTip_checkBox_RunAtStartup"));


            toolTip1.SetToolTip(checkBox_AutoStartMining,
                International.GetText("Form_Settings_ToolTip_checkBox_AutoStartMining"));
            toolTip1.SetToolTip(pictureBox_AutoStartMining,
                International.GetText("Form_Settings_ToolTip_checkBox_AutoStartMining"));

            /*
            toolTip1.SetToolTip(textBox_ethminerDefaultBlockHeight,
                International.GetText("Form_Settings_ToolTip_ethminerDefaultBlockHeight"));
            toolTip1.SetToolTip(label_ethminerDefaultBlockHeight,
                International.GetText("Form_Settings_ToolTip_ethminerDefaultBlockHeight"));
            toolTip1.SetToolTip(pictureBox_ethminerDefaultBlockHeight,
                International.GetText("Form_Settings_ToolTip_ethminerDefaultBlockHeight"));
                */
            toolTip1.SetToolTip(label_displayCurrency, International.GetText("Form_Settings_ToolTip_DisplayCurrency"));
            toolTip1.SetToolTip(pictureBox_displayCurrency,
                International.GetText("Form_Settings_ToolTip_DisplayCurrency"));
            toolTip1.SetToolTip(currencyConverterCombobox,
                International.GetText("Form_Settings_ToolTip_DisplayCurrency"));

            // Setup Tooltips CPU
            /*
            toolTip1.SetToolTip(comboBox_CPU0_ForceCPUExtension,
                International.GetText("Form_Settings_ToolTip_CPU_ForceCPUExtension"));
            toolTip1.SetToolTip(label_CPU0_ForceCPUExtension,
                International.GetText("Form_Settings_ToolTip_CPU_ForceCPUExtension"));
            toolTip1.SetToolTip(pictureBox_CPU0_ForceCPUExtension,
                International.GetText("Form_Settings_ToolTip_CPU_ForceCPUExtension"));
                */
            // amd disable temp control
            /*
            toolTip1.SetToolTip(checkBox_AMD_DisableAMDTempControl,
                International.GetText("Form_Settings_ToolTip_DisableAMDTempControl"));
            toolTip1.SetToolTip(pictureBox_AMD_DisableAMDTempControl,
                International.GetText("Form_Settings_ToolTip_DisableAMDTempControl"));
                */
            // disable default optimizations
            /*
            toolTip1.SetToolTip(checkBox_DisableDefaultOptimizations,
                International.GetText("Form_Settings_ToolTip_DisableDefaultOptimizations"));
            toolTip1.SetToolTip(pictureBox_DisableDefaultOptimizations,
                International.GetText("Form_Settings_ToolTip_DisableDefaultOptimizations"));
                */
            // internet connection mining check
           // toolTip1.SetToolTip(checkBox_IdleWhenNoInternetAccess,
             //   International.GetText("Form_Settings_ToolTip_ContinueMiningIfNoInternetAccess"));
           // toolTip1.SetToolTip(pictureBox_IdleWhenNoInternetAccess,
             //   International.GetText("Form_Settings_ToolTip_ContinueMiningIfNoInternetAccess"));

            // IFTTT notification check
            /*
            toolTip1.SetToolTip(checkBox_UseIFTTT, International.GetText("Form_Settings_ToolTip_UseIFTTT"));
            toolTip1.SetToolTip(pictureBox_UseIFTTT, International.GetText("Form_Settings_ToolTip_UseIFTTT"));
            */
            toolTip1.SetToolTip(pictureBox_SwitchProfitabilityThreshold,
                International.GetText("Form_Settings_ToolTip_SwitchProfitabilityThreshold"));
            toolTip1.SetToolTip(label_SwitchProfitabilityThreshold,
                International.GetText("Form_Settings_ToolTip_SwitchProfitabilityThreshold"));

            toolTip1.SetToolTip(pictureBox_MinimizeMiningWindows,
                International.GetText("Form_Settings_ToolTip_MinimizeMiningWindows"));
            toolTip1.SetToolTip(checkBox_MinimizeMiningWindows,
                International.GetText("Form_Settings_ToolTip_MinimizeMiningWindows"));

            // Electricity cost
            toolTip1.SetToolTip(label_ElectricityCost, International.GetText("Form_Settings_ToolTip_ElectricityCost"));
            toolTip1.SetToolTip(textBox_ElectricityCost, International.GetText("Form_Settings_ToolTip_ElectricityCost"));
            toolTip1.SetToolTip(pictureBox_ElectricityCost, International.GetText("Form_Settings_ToolTip_ElectricityCost"));

            Text = International.GetText("Form_Settings_Title");

            algorithmSettingsControl1.InitLocale(toolTip1);
        }

        #region Form this

        private void InitializeFormTranslations()
        {
            buttonDefaults.Text = International.GetText("Form_Settings_buttonDefaultsText");
            buttonSaveClose.Text = International.GetText("Form_Settings_buttonSaveText");
            buttonCloseNoSave.Text = International.GetText("Form_Settings_buttonCloseNoSaveText");
        }

        #endregion //Form this

        #region Tab General

        private void InitializeGeneralTabTranslations()
        {
           // checkBox_DebugConsole.Text = International.GetText("Form_Settings_General_DebugConsole");
            checkBox_AutoStartMining.Text = International.GetText("Form_Settings_General_AutoStartMining");
            checkBox_HideMiningWindows.Text = International.GetText("Form_Settings_General_HideMiningWindows");
            checkBox_MinimizeToTray.Text = International.GetText("Form_Settings_General_MinimizeToTray");
            checkBox_DisableDetectionNVIDIA.Text =
                string.Format(International.GetText("Form_Settings_General_DisableDetection"), "NVIDIA");
            checkBox_DisableDetectionAMD.Text =
                string.Format(International.GetText("Form_Settings_General_DisableDetection"), "AMD");
            checkBox_AutoScaleBTCValues.Text = International.GetText("Form_Settings_General_AutoScaleBTCValues");
            checkBox_StartMiningWhenIdle.Text = International.GetText("Form_Settings_General_StartMiningWhenIdle");
           // checkBox_ShowDriverVersionWarning.Text =
             //   International.GetText("Form_Settings_General_ShowDriverVersionWarning");
          //  checkBox_DisableWindowsErrorReporting.Text =
         //       International.GetText("Form_Settings_General_DisableWindowsErrorReporting");
            checkBox_ShowInternetConnectionWarning.Text =
                International.GetText("Form_Settings_General_ShowInternetConnectionWarning");
           // checkBox_Use3rdPartyMiners.Text = International.GetText("Form_Settings_General_3rdparty_Text");
            checkBox_NVIDIAP0State.Text = International.GetText("Form_Settings_General_NVIDIAP0State");
            checkBox_LogToFile.Text = International.GetText("Form_Settings_General_LogToFile");
            /*
            checkBox_AMD_DisableAMDTempControl.Text =
                International.GetText("Form_Settings_General_DisableAMDTempControl");
                */
            checkBox_AllowMultipleInstances.Text =
                International.GetText("Form_Settings_General_AllowMultipleInstances_Text");
            checkBox_RunAtStartup.Text = International.GetText("Form_Settings_General_RunAtStartup");
            checkBox_MinimizeMiningWindows.Text = International.GetText("Form_Settings_General_MinimizeMiningWindows");
           // checkBox_UseIFTTT.Text = International.GetText("Form_Settings_General_UseIFTTT");
            checkBox_RunScriptOnCUDA_GPU_Lost.Text =
                International.GetText("Form_Settings_General_RunScriptOnCUDA_GPU_Lost");

            label_Language.Text = International.GetText("Form_Settings_General_Language") + ":";
            label1.Text = "Color profile (partial)";
            //label_BitcoinAddress.Text = International.GetText("BitcoinAddress") + ":";
            var newver = NiceHashStats.Version.Replace(",", ".");
            var ver = Configs.ConfigManager.GeneralConfig.ForkFixVersion;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            var build = buildDate.ToString("u").Replace("-", "").Replace(":", "").Replace("Z", "").Replace(" ", ".");
            linkLabelCurrentVersion.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabelCurrentVersion.Text = "Current version: " + ver + ". Build: " + build;

            linkLabelNewVersion.LinkBehavior = LinkBehavior.NeverUnderline;
            Double.TryParse(build.ToString(), out Form_Main.buildDcurrent);
            linkLabelNewVersion.Text = "No new version or build";
            buttonUpdate.Visible = false;

            if (Form_Main.buildDcurrent < Form_Main.buildD)
            {
                linkLabelNewVersion.Text = "New build: " + Form_Main.buildD;
                buttonUpdate.Visible = true;
                linkLabelNewVersion.LinkBehavior = LinkBehavior.SystemDefault;
            }
            var programVersion = ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",", ".");
            if (ConfigManager.GeneralConfig.ForkFixVersion < Form_Main.githubVersion)
            {
                linkLabelNewVersion.Text = "New version: " + Form_Main.githubVersion.ToString();
                buttonUpdate.Visible = true;
                linkLabelNewVersion.LinkBehavior = LinkBehavior.SystemDefault;
            }
            if (Form_Main.githubVersion <= 0)
            {
                linkLabelNewVersion.Text = "Error";
                buttonUpdate.Visible = false;
            }

            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
              //  label_BitcoinAddress.Text = "Биткоин адрес (старая платформа)";
                //label_BitcoinAddressNew.Text = "Биткоин адрес:";
                checkBox_Allow_remote_management.Text = "Разрешить удаленное управление";
                checkBox_Send_actual_version_info.Text = "Отправлять инфо о версии программы";
                checkBox_Additional_info_about_device.Text = "Дополнительная информация об устройстве";
                Checkbox_Save_windows_size_and_position.Text = "Сохранять позицию и размер окон";
                checkBox_sorting_list_of_algorithms.Text = "Сортировать список алгоритмов";
                checkBox_ShowFanAsPercent.Text = "Показать обороты вентилятора в процентах";
                checkbox_Group_same_devices.Text = "Группировать одинаковые устройства";
                checkBox_Force_mining_if_nonprofitable.Text = "Принудительный майнинг, если не прибыльно";
                checkBox_Show_profit_with_power_consumption.Text = "Показывать прибыльность с учетом рахода на эл.энергию";
                checkBox_fiat.Text = "Показывать прибыльность алгоритмов в фиатной валюте";
                checkBox_AlwaysOnTop.Text = "Поверх остальных окон";
                label_psu.Text = "КПД блока питания (%)";
                label_MBpower.Text = "Потребление м.платы, диска и т.п. (Вт)";
                checkBox_Disable_extra_launch_parameter_checking.Text = "Отключить проверку дополнительных параметров";
                checkBox_DisableDetectionCPU.Text = "Отключить определение CPU";
                label_AutoStartMiningDelay.Text = "Задержка автозапуска майнинга (сек)";
                groupBoxMOPA.Text = "Метод получения прибыльности алгоритмов";
                radioButtonMOPA1.Text = "Стандартный NHM";
                radioButtonMOPA2.Text = "Текущая прибыльность";
                radioButtonMOPA3.Text = "Средняя прибыльн. за 5 мин.";
                radioButtonMOPA4.Text = "Средняя прибыльность за 24 часа";
                radioButtonMOPA5.Text = "Наибольшая прибыльность по всем методам";
                label1.Text = "Цветовой профиль (частично)";
                groupBox1.Text = "Устройства";
                //algorithmSettingsControl1.
                //groupBoxSelectedAlgorithmSettings

                label_switching_algorithms.Text = "Переключать алгоритмы";
                //comboBox_switching_algorithms.Items.Add("немедленно (не рекомендуется)");
                comboBox_switching_algorithms.Items.Add("через 1 мин");
                comboBox_switching_algorithms.Items.Add("через 3 мин");
                comboBox_switching_algorithms.Items.Add("через 5 мин");
                comboBox_switching_algorithms.Items.Add("через 10 мин");
                comboBox_switching_algorithms.Items.Add("через 15 мин");
                comboBox_switching_algorithms.Items.Add("случайно (стандартный NHM)");

                label_devices_count.Text = "Кол-во видимых устройств";
                tabPageAbout.Text = "О программе";

                groupBoxInfo.Text = "Информация";
                groupBoxUpdates.Text = "Версия";
                buttonLicence.Text = "Просмотр лицензии";
                richTextBoxInfo.ReadOnly = true;

                richTextBoxInfo.SelectionFont = new Font(richTextBoxInfo.Font, FontStyle.Bold);
                richTextBoxInfo.AppendText("Miner Legacy Fork Fix");
                richTextBoxInfo.SelectionFont = new Font(richTextBoxInfo.Font, FontStyle.Regular);
                richTextBoxInfo.AppendText(" (для NiceHash) это неофициальная ветка программы, " +
                    "созданная https://github.com/angelbbs при поддержке сообществ " +
                    "https://bitcointalk.org/index.php?topic=5132119.0 " +
                    "https://github.com/angelbbs/NiceHashMinerLegacy/issues");
                groupBoxUpdates.Text = "Обновление программы";
                linkLabelCurrentVersion.Text = "Текущая версия: " + ver + ". Сборка: " + build;
                linkLabelNewVersion.Text = "Нет новой версии или сборки";
                //richTextBoxNewVersion.Text = "Новая сборка: " + build;
                buttonCheckNewVersion.Text = "Проверить сейчас";
                buttonUpdate.Text = "Обновить сейчас";

                if (Form_Main.buildDcurrent < Form_Main.buildD)
                {
                    linkLabelNewVersion.Text = "Новая сборка: " + Form_Main.buildD.ToString();
                    linkLabelNewVersion.LinkBehavior = LinkBehavior.SystemDefault;
                    buttonUpdate.Visible = true;
                }

                if (ConfigManager.GeneralConfig.ForkFixVersion < Form_Main.githubVersion)
                {
                    linkLabelNewVersion.Text = "Новая версия: " + Form_Main.githubVersion.ToString();
                    linkLabelNewVersion.LinkBehavior = LinkBehavior.NeverUnderline;
                    buttonUpdate.Visible = true;
                }
                if (Form_Main.githubVersion <= 0)
                {
                    linkLabelNewVersion.Text = "Ошибка проверки новой версии";
                }
            }
            else
            {
                //comboBox_switching_algorithms.Items.Add("immediately (not recommended)");
                comboBox_switching_algorithms.Items.Add("after 1 min");
                comboBox_switching_algorithms.Items.Add("after 3 min");
                comboBox_switching_algorithms.Items.Add("after 5 min");
                comboBox_switching_algorithms.Items.Add("after 10 min");
                comboBox_switching_algorithms.Items.Add("after 15 min");
                comboBox_switching_algorithms.Items.Add("random (standard NHM)");
            }

            comboBox_devices_count.Items.Add("6");
            comboBox_devices_count.Items.Add("7");
            comboBox_devices_count.Items.Add("8");
            comboBox_devices_count.Items.Add("9");
            comboBox_devices_count.Items.Add("10");
            comboBox_devices_count.Items.Add("11");
            comboBox_devices_count.Items.Add("12");

            checkBox_RunEthlargement.Enabled = Helpers.IsElevated;

            radioButtonMOPA1.Checked = ConfigManager.GeneralConfig.MOPA1;
            radioButtonMOPA2.Checked = ConfigManager.GeneralConfig.MOPA2;
            radioButtonMOPA3.Checked = ConfigManager.GeneralConfig.MOPA3;
            radioButtonMOPA4.Checked = ConfigManager.GeneralConfig.MOPA4;
            radioButtonMOPA5.Checked = ConfigManager.GeneralConfig.MOPA5;
            /*
            label_WorkerName.Text = International.GetText("WorkerName") + ":";
            label_ServiceLocation.Text = International.GetText("Service_Location") + ":";
            {
                var i = 0;
                foreach (var loc in Globals.MiningLocation)
                {
                    if (i != 6)
                    {
                        comboBox_ServiceLocation.Items[i++] = International.GetText("LocationName_" + loc);
                    }
                    else
                    {
                        comboBox_ServiceLocation.Items[i++] = "Auto";
                    }
                }
            }
            */

            label_MinIdleSeconds.Text = International.GetText("Form_Settings_General_MinIdleSeconds") + ":";
            label_MinerRestartDelayMS.Text = International.GetText("Form_Settings_General_MinerRestartDelayMS") + ":";
            /*
            label_MinerAPIQueryInterval.Text =
                International.GetText("Form_Settings_General_MinerAPIQueryInterval") + ":";
                */
            label_LogMaxFileSize.Text = International.GetText("Form_Settings_General_LogMaxFileSize") + ":";

            label_SwitchMaxSeconds.Text =
                International.GetText("Form_Settings_General_SwitchMaxSeconds") + ":";
            label_SwitchMinSeconds.Text = International.GetText("Form_Settings_General_SwitchMinSeconds") + ":";
            /*
            label_ethminerDefaultBlockHeight.Text =
                International.GetText("Form_Settings_General_ethminerDefaultBlockHeight") + ":";
            label_DagGeneration.Text = International.GetText("Form_Settings_DagGeneration") + ":";
            */
            label_APIBindPortStart.Text = International.GetText("Form_Settings_APIBindPortStart") + ":";

            label_MinProfit.Text = International.GetText("Form_Settings_General_MinimumProfit") + ":";

            label_displayCurrency.Text = International.GetText("Form_Settings_DisplayCurrency");

          //  label_IFTTTAPIKey.Text = International.GetText("Form_Settings_IFTTTAPIKey");

            label_ElectricityCost.Text = International.GetText("Form_Settings_ElectricityCost");

            // Benchmark time limits
            // internationalization change
            /*
            groupBoxBenchmarkTimeLimits.Text =
                International.GetText("Form_Settings_General_BenchmarkTimeLimits_Title") + ":";
            benchmarkLimitControlCPU.GroupName =
                International.GetText("Form_Settings_General_BenchmarkTimeLimitsCPU_Group") + ":";
            benchmarkLimitControlNVIDIA.GroupName =
                International.GetText("Form_Settings_General_BenchmarkTimeLimitsNVIDIA_Group") + ":";
            benchmarkLimitControlAMD.GroupName =
                International.GetText("Form_Settings_General_BenchmarkTimeLimitsAMD_Group") + ":";
            // moved from constructor because of editor
            benchmarkLimitControlCPU.InitLocale();
            benchmarkLimitControlNVIDIA.InitLocale();
            benchmarkLimitControlAMD.InitLocale();
            */
            // device enabled listview translation
            devicesListViewEnableControl1.InitLocale();
            Rectangle screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            if (ConfigManager.GeneralConfig.SettingsFormLeft + ConfigManager.GeneralConfig.SettingsFormWidth <= screenSize.Size.Width &&
                ConfigManager.GeneralConfig.SettingsFormTop + ConfigManager.GeneralConfig.SettingsFormHeight <= screenSize.Size.Height)
            {
                if (ConfigManager.GeneralConfig.SettingsFormTop + ConfigManager.GeneralConfig.SettingsFormLeft != 0)
                {
                    this.Top = ConfigManager.GeneralConfig.SettingsFormTop;
                    this.Left = ConfigManager.GeneralConfig.SettingsFormLeft;
                }
                else
                {
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
                this.Width = ConfigManager.GeneralConfig.SettingsFormWidth;
                this.Height = ConfigManager.GeneralConfig.SettingsFormHeight;
            } else
            {
                this.Top = 0;
                this.Left = 0;
            }


            algorithmsListView1.InitLocale();

            comboBox_ColorProfile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                  this.BackColor = Form_Main._backColor;
                 this.ForeColor = Form_Main._foreColor;
                  this.tabControlGeneral.DisplayStyle = TabStyle.Angled;
                  this.tabControlGeneral.DisplayStyleProvider.Opacity = 0.8F;

                this.tabControlGeneral.DisplayStyleProvider.TextColor = Color.White;
                this.tabControlGeneral.DisplayStyleProvider.TextColorDisabled = Color.White;
                this.tabControlGeneral.DisplayStyleProvider.BorderColor = Color.Transparent;
                this.tabControlGeneral.DisplayStyleProvider.BorderColorHot = Form_Main._foreColor;

                foreach (var lbl in this.Controls.OfType<Button>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._textColor;
                    lbl.FlatStyle = FlatStyle.Flat;
                    lbl.FlatAppearance.BorderColor = Form_Main._textColor;
                    lbl.FlatAppearance.BorderSize = 1;
                }

                tabControlGeneral.SelectedTab.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<TabControl>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                tabPageAdvanced1.BackColor = Form_Main._backColor;
                tabPageAdvanced1.ForeColor = Form_Main._foreColor;

                tabPageDevicesAlgos.BackColor = Form_Main._backColor;
                tabPageDevicesAlgos.ForeColor = Form_Main._foreColor;

                tabPageAbout.BackColor = Form_Main._backColor;
                tabPageAbout.ForeColor = Form_Main._foreColor;

                foreach (var lbl in tabPageAdvanced1.Controls.OfType<GroupBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }


                foreach (var lbl in tabPageDevicesAlgos.Controls.OfType<GroupBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                foreach (var lbl in tabPageDevicesAlgos.Controls.OfType<Button>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._textColor;
                    lbl.FlatStyle = FlatStyle.Flat;
                    lbl.FlatAppearance.BorderColor = Form_Main._textColor;
                    lbl.FlatAppearance.BorderSize = 1;
                }

                    richTextBoxInfo.BackColor = Form_Main._backColor;
                    richTextBoxInfo.ForeColor = Form_Main._textColor;
                linkLabelCurrentVersion.BackColor = Form_Main._backColor;
                linkLabelCurrentVersion.ForeColor = Form_Main._textColor;
                linkLabelCurrentVersion.LinkColor = Form_Main._textColor;
                linkLabelCurrentVersion.ActiveLinkColor = Form_Main._textColor;
                linkLabelCurrentVersion.MouseLeave += (s, e) => linkLabelCurrentVersion.LinkBehavior = LinkBehavior.NeverUnderline;
                linkLabelCurrentVersion.MouseEnter += (s, e) => linkLabelCurrentVersion.LinkBehavior =LinkBehavior.AlwaysUnderline;
                linkLabelNewVersion.MouseLeave += (s, e) => linkLabelNewVersion.LinkBehavior = LinkBehavior.NeverUnderline;
                linkLabelNewVersion.MouseEnter += (s, e) => linkLabelNewVersion.LinkBehavior = LinkBehavior.AlwaysUnderline;

                linkLabelNewVersion.BackColor = Form_Main._backColor;
                linkLabelNewVersion.ForeColor = Form_Main._textColor;
                linkLabelNewVersion.LinkColor = Form_Main._textColor;

                foreach (var lbl in tabPageDevicesAlgos.Controls.OfType<Button>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                foreach (var lbl in tabPageDevicesAlgos.Controls.OfType<UserControl>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                //*
                foreach (var lbl in tabPageAbout.Controls.OfType<GroupBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                foreach (var lbl in tabPageAbout.Controls.OfType<Button>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._textColor;
                    lbl.FlatStyle = FlatStyle.Flat;
                    lbl.FlatAppearance.BorderColor = Form_Main._textColor;
                    lbl.FlatAppearance.BorderSize = 1;
                }

                foreach (var lbl in tabPageAbout.Controls.OfType<Button>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                foreach (var lbl in tabPageAbout.Controls.OfType<UserControl>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                //comboBox_ServiceLocation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                comboBox_TimeUnit.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                currencyConverterCombobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                comboBox_Language.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                comboBox_ColorProfile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                comboBox_switching_algorithms.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                comboBox_devices_count.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;

                foreach (var lbl in this.tabPageGeneral.Controls.OfType<GroupBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                foreach (var lbl in this.tabPageGeneral.Controls.OfType<CheckBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }
                checkBox_Force_mining_if_nonprofitable.BackColor = Form_Main._backColor;
                checkBox_Force_mining_if_nonprofitable.ForeColor = Form_Main._textColor;

                checkBox_Show_profit_with_power_consumption.BackColor = Form_Main._backColor;
                checkBox_Show_profit_with_power_consumption.ForeColor = Form_Main._textColor;

                checkBox_Allow_remote_management.BackColor = Form_Main._backColor;
                checkBox_Allow_remote_management.ForeColor = Form_Main._textColor;

                checkBox_Send_actual_version_info.BackColor = Form_Main._backColor;
                checkBox_Send_actual_version_info.ForeColor = Form_Main._textColor;

                checkBox_Additional_info_about_device.BackColor = Form_Main._backColor;
                checkBox_Additional_info_about_device.ForeColor = Form_Main._textColor;

                Checkbox_Save_windows_size_and_position.BackColor = Form_Main._backColor;
                Checkbox_Save_windows_size_and_position.ForeColor = Form_Main._textColor;

                checkBox_sorting_list_of_algorithms.BackColor = Form_Main._backColor;
                checkBox_sorting_list_of_algorithms.ForeColor = Form_Main._textColor;

                checkBox_ShowFanAsPercent.BackColor = Form_Main._backColor;
                checkBox_ShowFanAsPercent.ForeColor = Form_Main._textColor;

                checkbox_Group_same_devices.BackColor = Form_Main._backColor;
                checkbox_Group_same_devices.ForeColor = Form_Main._textColor;

                checkBox_Disable_extra_launch_parameter_checking.BackColor = Form_Main._backColor;
                checkBox_Disable_extra_launch_parameter_checking.ForeColor = Form_Main._textColor;

                checkBox_RunEthlargement.BackColor = Form_Main._backColor;
                checkBox_RunEthlargement.ForeColor = Form_Main._textColor;
                if (checkBox_RunEthlargement.Enabled == false)
                {
                    label_RunEthlargement.ForeColor = Color.Gray;
                }
                else
                {
                    label_RunEthlargement.ForeColor = Form_Main._foreColor;
                }
                /*
                textBox_BitcoinAddressNew.BackColor = Form_Main._backColor;
                textBox_BitcoinAddressNew.ForeColor = Form_Main._foreColor;
                textBox_BitcoinAddressNew.BorderStyle = BorderStyle.FixedSingle;
                */
                textBox_APIBindPortStart.BackColor = Form_Main._backColor;
                textBox_APIBindPortStart.ForeColor = Form_Main._foreColor;
                textBox_APIBindPortStart.BorderStyle = BorderStyle.FixedSingle;

                textBox_AutoStartMiningDelay.BackColor = Form_Main._backColor;
                textBox_AutoStartMiningDelay.ForeColor = Form_Main._foreColor;
                textBox_AutoStartMiningDelay.BorderStyle = BorderStyle.FixedSingle;

                textBox_ElectricityCost.BackColor = Form_Main._backColor;
                textBox_ElectricityCost.ForeColor = Form_Main._foreColor;
                textBox_ElectricityCost.BorderStyle = BorderStyle.FixedSingle;

                textBox_psu.BackColor = Form_Main._backColor;
                textBox_psu.ForeColor = Form_Main._foreColor;
                textBox_psu.BorderStyle = BorderStyle.FixedSingle;

                textBox_mb.BackColor = Form_Main._backColor;
                textBox_mb.ForeColor = Form_Main._foreColor;
                textBox_mb.BorderStyle = BorderStyle.FixedSingle;
                /*
                textBox_IFTTTKey.BackColor = Form_Main._backColor;
                textBox_IFTTTKey.ForeColor = Form_Main._foreColor;
                textBox_IFTTTKey.BorderStyle = BorderStyle.FixedSingle;
                */
                textBox_LogMaxFileSize.BackColor = Form_Main._backColor;
                textBox_LogMaxFileSize.ForeColor = Form_Main._foreColor;
                textBox_LogMaxFileSize.BorderStyle = BorderStyle.FixedSingle;

                textBox_MinerRestartDelayMS.BackColor = Form_Main._backColor;
                textBox_MinerRestartDelayMS.ForeColor = Form_Main._foreColor;
                textBox_MinerRestartDelayMS.BorderStyle = BorderStyle.FixedSingle;

                textBox_MinIdleSeconds.BackColor = Form_Main._backColor;
                textBox_MinIdleSeconds.ForeColor = Form_Main._foreColor;
                textBox_MinIdleSeconds.BorderStyle = BorderStyle.FixedSingle;

                textBox_MinProfit.BackColor = Form_Main._backColor;
                textBox_MinProfit.ForeColor = Form_Main._foreColor;
                textBox_MinProfit.BorderStyle = BorderStyle.FixedSingle;

                textBox_SwitchMaxSeconds.BackColor = Form_Main._backColor;
                textBox_SwitchMaxSeconds.ForeColor = Form_Main._foreColor;
                textBox_SwitchMaxSeconds.BorderStyle = BorderStyle.FixedSingle;

                textBox_SwitchMinSeconds.BackColor = Form_Main._backColor;
                textBox_SwitchMinSeconds.ForeColor = Form_Main._foreColor;
                textBox_SwitchMinSeconds.BorderStyle = BorderStyle.FixedSingle;

                textBox_SwitchProfitabilityThreshold.BackColor = Form_Main._backColor;
                textBox_SwitchProfitabilityThreshold.ForeColor = Form_Main._foreColor;
                textBox_SwitchProfitabilityThreshold.BorderStyle = BorderStyle.FixedSingle;
                /*
                textBox_WorkerName.BackColor = Form_Main._backColor;
                textBox_WorkerName.ForeColor = Form_Main._foreColor;
                textBox_WorkerName.BorderStyle = BorderStyle.FixedSingle;

                pictureBox_Info_BitcoinAddress.Image = NiceHashMiner.Properties.Resources.info_white_18;
                */
                pictureBox_AllowMultipleInstances.Image = NiceHashMiner.Properties.Resources.info_white_18;
               // pictureBox_AMD_DisableAMDTempControl.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_APIBindPortStart.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_AutoScaleBTCValues.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_AutoStartMining.Image = NiceHashMiner.Properties.Resources.info_white_18;
             //   pictureBox_DebugConsole.Image = NiceHashMiner.Properties.Resources.info_white_18;
             //   pictureBox_DisableDefaultOptimizations.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_DisableDetectionAMD.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_DisableDetectionNVIDIA.Image = NiceHashMiner.Properties.Resources.info_white_18;
              //  pictureBox_DisableWindowsErrorReporting.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_displayCurrency.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_ElectricityCost.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_HideMiningWindows.Image = NiceHashMiner.Properties.Resources.info_white_18;
             //   pictureBox_IdleWhenNoInternetAccess.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_Language.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_LogMaxFileSize.Image = NiceHashMiner.Properties.Resources.info_white_18;
               // pictureBox_LogToFile.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinerRestartDelayMS.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinIdleSeconds.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinimizeMiningWindows.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinimizeToTray.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinProfit.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_NVIDIAP0State.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_RunAtStartup.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_RunScriptOnCUDA_GPU_Lost.Image = NiceHashMiner.Properties.Resources.info_white_18;
               // pictureBox_ServiceLocation.Image = NiceHashMiner.Properties.Resources.info_white_18;
              //  pictureBox_ShowDriverVersionWarning.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_ShowInternetConnectionWarning.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_StartMiningWhenIdle.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_SwitchMaxSeconds.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_SwitchMinSeconds.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_SwitchProfitabilityThreshold.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_TimeUnit.Image = NiceHashMiner.Properties.Resources.info_white_18;
             //   pictureBox_Use3rdPartyMiners.Image = NiceHashMiner.Properties.Resources.info_white_18;
              //  pictureBox_UseIFTTT.Image = NiceHashMiner.Properties.Resources.info_white_18;
               // pictureBox_WorkerName.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox1.Image = NiceHashMiner.Properties.Resources.info_white_18;

                devicesListViewEnableControl1.BackColor = Form_Main._backColor;
                devicesListViewEnableControl1.ForeColor = Form_Main._foreColor;
                algorithmsListView1.BackColor = Form_Main._backColor;
                algorithmsListView1.ForeColor = Form_Main._foreColor;
                tabPageGeneral.BackColor = Form_Main._backColor;
                tabPageGeneral.ForeColor = Form_Main._foreColor;
            } else
            {
                devicesListViewEnableControl1.BackColor = SystemColors.ControlLightLight;
                devicesListViewEnableControl1.ForeColor = Form_Main._foreColor;
                algorithmsListView1.BackColor = SystemColors.ControlLightLight;
                algorithmsListView1.ForeColor = Form_Main._foreColor;
            }

            // Setup Tooltips CPU
            /*
            label_CPU0_ForceCPUExtension.Text =
                International.GetText("Form_Settings_General_CPU_ForceCPUExtension") + ":";
                */
            // new translations
            tabControlGeneral.TabPages[0].Text = International.GetText("FormSettings_Tab_General");
            tabControlGeneral.TabPages[1].Text = International.GetText("FormSettings_Tab_Advanced");
            tabControlGeneral.TabPages[2].Text = International.GetText("FormSettings_Tab_Devices_Algorithms");
            groupBox_Main.Text = International.GetText("FormSettings_Tab_General_Group_Main");
            groupBox_Localization.Text = International.GetText("FormSettings_Tab_General_Group_Localization");
            groupBox_Logging.Text = International.GetText("FormSettings_Tab_General_Group_Logging");
            groupBox_Misc.Text = International.GetText("FormSettings_Tab_General_Group_Misc");
            // advanced
            groupBox_Miners.Text = International.GetText("FormSettings_Tab_Advanced_Group_Miners");
            /*
            groupBoxBenchmarkTimeLimits.Text =
                International.GetText("FormSettings_Tab_Advanced_Group_BenchmarkTimeLimits");
                */
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                buttonGPUtuning.Text = "Настройка GPU";
            }
            //buttonSelectedProfit.Text =
              //  International.GetText("FormSettings_Tab_Devices_Algorithms_Check_SingleProfitability");

         //   checkBox_DisableDefaultOptimizations.Text =
           //     International.GetText("Form_Settings_Text_DisableDefaultOptimizations");
          //  checkBox_IdleWhenNoInternetAccess.Text =
          //      International.GetText("Form_Settings_Text_ContinueMiningIfNoInternetAccess");

            label_SwitchProfitabilityThreshold.Text =
                International.GetText("Form_Settings_General_SwitchProfitabilityThreshold");
        }

        private void InitializeGeneralTabCallbacks()
        {
            // Add EventHandler for all the general tab's checkboxes
            {
                checkBox_AutoScaleBTCValues.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionCPU.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionAMD.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionNVIDIA.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
              //  checkBox_IdleWhenNoInternetAccess.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_MinimizeToTray.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_HideMiningWindows.CheckedChanged +=GeneralCheckBoxes_CheckedChanged;
                checkBox_AlwaysOnTop.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
              //  checkBox_DebugConsole.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
             //   checkBox_ShowDriverVersionWarning.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
              //  checkBox_DisableWindowsErrorReporting.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ShowInternetConnectionWarning.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_StartMiningWhenIdle.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_NVIDIAP0State.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_LogToFile.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_AutoStartMining.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_AllowMultipleInstances.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_MinimizeMiningWindows.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
               // checkBox_UseIFTTT.CheckedChanged += CheckBox_UseIFTTT_CheckChanged;
                checkBox_RunScriptOnCUDA_GPU_Lost.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Allow_remote_management.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Send_actual_version_info.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Force_mining_if_nonprofitable.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Show_profit_with_power_consumption.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Additional_info_about_device.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                Checkbox_Save_windows_size_and_position.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_sorting_list_of_algorithms.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ShowFanAsPercent.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_fiat.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkbox_Group_same_devices.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Disable_extra_launch_parameter_checking.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
               // checkBox_AMD_DisableAMDTempControl.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_RunEthlargement.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
            }
            // Add EventHandler for all the general tab's textboxes
            {
               // textBox_BitcoinAddress.Leave += GeneralTextBoxes_Leave;
               /*
                textBox_BitcoinAddressNew.Leave += GeneralTextBoxes_Leave;
                textBox_WorkerName.Leave += GeneralTextBoxes_Leave;
                */
              //  textBox_IFTTTKey.Leave += GeneralTextBoxes_Leave;
                // these are ints only
                textBox_SwitchMaxSeconds.Leave += GeneralTextBoxes_Leave;
                textBox_SwitchMinSeconds.Leave += GeneralTextBoxes_Leave;
               // textBox_MinerAPIQueryInterval.Leave += GeneralTextBoxes_Leave;
                textBox_MinerRestartDelayMS.Leave += GeneralTextBoxes_Leave;
                textBox_AutoStartMiningDelay.Leave += GeneralTextBoxes_Leave;
                textBox_MinIdleSeconds.Leave += GeneralTextBoxes_Leave;
                textBox_LogMaxFileSize.Leave += GeneralTextBoxes_Leave;
         //       textBox_ethminerDefaultBlockHeight.Leave += GeneralTextBoxes_Leave;
                textBox_APIBindPortStart.Leave += GeneralTextBoxes_Leave;
                textBox_MinProfit.Leave += GeneralTextBoxes_Leave;
                textBox_ElectricityCost.Leave += GeneralTextBoxes_Leave;
                textBox_psu.Leave += GeneralTextBoxes_Leave;
                textBox_mb.Leave += GeneralTextBoxes_Leave;
                textBox_SwitchProfitabilityThreshold.Leave += GeneralTextBoxes_Leave;
                // set int only keypress
                textBox_SwitchMaxSeconds.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                textBox_SwitchMinSeconds.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
              //  textBox_MinerAPIQueryInterval.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                textBox_MinerRestartDelayMS.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                textBox_MinIdleSeconds.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                textBox_LogMaxFileSize.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
         //       textBox_ethminerDefaultBlockHeight.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                textBox_APIBindPortStart.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                // set double only keypress
                textBox_MinProfit.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBox_ElectricityCost.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBox_psu.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBox_mb.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
            }
            // Add EventHandler for all the general tab's textboxes
            {
                comboBox_Language.Leave += GeneralComboBoxes_Leave;
               // comboBox_ServiceLocation.Leave += GeneralComboBoxes_Leave;
                comboBox_TimeUnit.Leave += GeneralComboBoxes_Leave;
                comboBox_ColorProfile.Leave += GeneralComboBoxes_Leave;
                comboBox_switching_algorithms.Leave += GeneralComboBoxes_Leave;
                comboBox_devices_count.Leave += GeneralComboBoxes_Leave;
          //      comboBox_DagLoadMode.Leave += GeneralComboBoxes_Leave;
            }

            // CPU exceptions
            /*
            comboBox_CPU0_ForceCPUExtension.SelectedIndex = (int) ConfigManager.GeneralConfig.ForceCPUExtension;
            comboBox_CPU0_ForceCPUExtension.SelectedIndexChanged +=
                ComboBox_CPU0_ForceCPUExtension_SelectedIndexChanged;
                */
            // fill dag dropdown
            /*
            comboBox_DagLoadMode.Items.Clear();
            for (var i = 0; i < (int) DagGenerationType.END; ++i)
            {
                comboBox_DagLoadMode.Items.Add(MinerEtherum.GetDagGenerationString((DagGenerationType) i));
            }

            // set selected
            comboBox_DagLoadMode.SelectedIndex = (int) ConfigManager.GeneralConfig.EthminerDagGenerationType;
            */
        }

        private void InitializeGeneralTabFieldValuesReferences()
        {
            // Checkboxes set checked value
            {
              //  checkBox_DebugConsole.Checked = ConfigManager.GeneralConfig.DebugConsole;
                checkBox_AutoStartMining.Checked = ConfigManager.GeneralConfig.AutoStartMining;
                if (checkBox_AutoStartMining.Checked)
                {
                    textBox_AutoStartMiningDelay.Enabled = true;
                }
                else
                {
                    textBox_AutoStartMiningDelay.Enabled = false;
                }
                checkBox_HideMiningWindows.Checked = ConfigManager.GeneralConfig.HideMiningWindows;
                checkBox_MinimizeToTray.Checked = ConfigManager.GeneralConfig.MinimizeToTray;
                checkBox_AlwaysOnTop.Checked = ConfigManager.GeneralConfig.AlwaysOnTop;
                checkBox_DisableDetectionNVIDIA.Checked =
                    ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionNVIDIA;
                checkBox_DisableDetectionCPU.Checked = ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionCPU;
                checkBox_DisableDetectionAMD.Checked = ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionAMD;
                checkBox_AutoScaleBTCValues.Checked = ConfigManager.GeneralConfig.AutoScaleBTCValues;
                checkBox_StartMiningWhenIdle.Checked = ConfigManager.GeneralConfig.StartMiningWhenIdle;
             //   checkBox_ShowDriverVersionWarning.Checked = ConfigManager.GeneralConfig.ShowDriverVersionWarning;
              //  checkBox_DisableWindowsErrorReporting.Checked =
               //     ConfigManager.GeneralConfig.DisableWindowsErrorReporting;
                checkBox_ShowInternetConnectionWarning.Checked =
                    ConfigManager.GeneralConfig.ShowInternetConnectionWarning;
                checkBox_NVIDIAP0State.Checked = ConfigManager.GeneralConfig.NVIDIAP0State;
                checkBox_LogToFile.Checked = ConfigManager.GeneralConfig.LogToFile;
                if (checkBox_LogToFile.Checked)
                {
                    textBox_LogMaxFileSize.Enabled = true;
                } else
                {
                    textBox_LogMaxFileSize.Enabled = false;
                }
              //  checkBox_AMD_DisableAMDTempControl.Checked = ConfigManager.GeneralConfig.DisableAMDTempControl;
              //    checkBox_DisableDefaultOptimizations.Checked = ConfigManager.GeneralConfig.DisableDefaultOptimizations;
             //   checkBox_IdleWhenNoInternetAccess.Checked = ConfigManager.GeneralConfig.IdleWhenNoInternetAccess;
              //  checkBox_Use3rdPartyMiners.Checked =
                //    ConfigManager.GeneralConfig.Use3rdPartyMiners == Use3rdPartyMiners.YES;
                checkBox_AllowMultipleInstances.Checked = ConfigManager.GeneralConfig.AllowMultipleInstances;
                checkBox_RunAtStartup.Checked = IsInStartupRegistry();
                checkBox_MinimizeMiningWindows.Checked = ConfigManager.GeneralConfig.MinimizeMiningWindows;
                checkBox_MinimizeMiningWindows.Enabled = !ConfigManager.GeneralConfig.HideMiningWindows;
               // checkBox_UseIFTTT.Checked = ConfigManager.GeneralConfig.UseIFTTT;
                checkBox_RunScriptOnCUDA_GPU_Lost.Checked = ConfigManager.GeneralConfig.RunScriptOnCUDA_GPU_Lost;
                checkBox_Allow_remote_management.Checked = ConfigManager.GeneralConfig.Allow_remote_management;
                checkBox_Send_actual_version_info.Checked = ConfigManager.GeneralConfig.Send_actual_version_info;
                checkBox_Force_mining_if_nonprofitable.Checked = ConfigManager.GeneralConfig.Force_mining_if_nonprofitable;
                checkBox_Show_profit_with_power_consumption.Checked = ConfigManager.GeneralConfig.DecreasePowerCost;
                checkBox_fiat.Checked = ConfigManager.GeneralConfig.FiatCurrency;
                checkBox_Additional_info_about_device.Checked = ConfigManager.GeneralConfig.Additional_info_about_device;
                Checkbox_Save_windows_size_and_position.Checked = ConfigManager.GeneralConfig.Save_windows_size_and_position;
                checkBox_sorting_list_of_algorithms.Checked = ConfigManager.GeneralConfig.ColumnSort;
                checkBox_ShowFanAsPercent.Checked = ConfigManager.GeneralConfig.ShowFanAsPercent;
                checkbox_Group_same_devices.Checked = ConfigManager.GeneralConfig.Group_same_devices;
                checkBox_Disable_extra_launch_parameter_checking.Checked = ConfigManager.GeneralConfig.Disable_extra_launch_parameter_checking;
                checkBox_RunEthlargement.Checked = ConfigManager.GeneralConfig.UseEthlargement;
            }

            // Textboxes
            {
               // textBox_BitcoinAddress.Text = ConfigManager.GeneralConfig.BitcoinAddress;
               /*
                textBox_BitcoinAddressNew.Text = ConfigManager.GeneralConfig.BitcoinAddressNew;
                textBox_WorkerName.Text = ConfigManager.GeneralConfig.WorkerName;
                */
              //  textBox_IFTTTKey.Text = ConfigManager.GeneralConfig.IFTTTKey;
                //textBox_IFTTTKey.Enabled = ConfigManager.GeneralConfig.UseIFTTT;
                textBox_SwitchMaxSeconds.Text = ConfigManager.GeneralConfig.SwitchSmaTimeChangeSeconds.Upper.ToString();
                textBox_SwitchMinSeconds.Text = ConfigManager.GeneralConfig.SwitchSmaTimeChangeSeconds.Lower.ToString();
               // textBox_MinerAPIQueryInterval.Text = ConfigManager.GeneralConfig.MinerAPIQueryInterval.ToString();
                textBox_MinerRestartDelayMS.Text = ConfigManager.GeneralConfig.MinerRestartDelayMS.ToString();
                textBox_MinIdleSeconds.Text = ConfigManager.GeneralConfig.MinIdleSeconds.ToString();
                textBox_LogMaxFileSize.Text = ConfigManager.GeneralConfig.LogMaxFileSize.ToString();
                textBox_AutoStartMiningDelay.Text = ConfigManager.GeneralConfig.AutoStartMiningDelay.ToString();
                /*
                textBox_ethminerDefaultBlockHeight.Text =
                    ConfigManager.GeneralConfig.ethminerDefaultBlockHeight.ToString();
                    */
                textBox_APIBindPortStart.Text = ConfigManager.GeneralConfig.ApiBindPortPoolStart.ToString();
                textBox_MinProfit.Text =
                    ConfigManager.GeneralConfig.MinimumProfit.ToString("F2").Replace(',', '.'); // force comma;
                textBox_SwitchProfitabilityThreshold.Text = ((ConfigManager.GeneralConfig.SwitchProfitabilityThreshold)*100)
                    .ToString("F2").Replace(',', '.'); // force comma;
                textBox_ElectricityCost.Text = ConfigManager.GeneralConfig.KwhPrice.ToString("0.0000");
                textBox_psu.Text = ConfigManager.GeneralConfig.PowerPSU.ToString();
                textBox_mb.Text = ConfigManager.GeneralConfig.PowerMB.ToString();
            }

            // set custom control referances
            {
                /*
                benchmarkLimitControlCPU.TimeLimits = ConfigManager.GeneralConfig.BenchmarkTimeLimits.CPU;
                benchmarkLimitControlNVIDIA.TimeLimits = ConfigManager.GeneralConfig.BenchmarkTimeLimits.NVIDIA;
                benchmarkLimitControlAMD.TimeLimits = ConfigManager.GeneralConfig.BenchmarkTimeLimits.AMD;
                */
                // here we want all devices
                devicesListViewEnableControl1.SetComputeDevices(ComputeDeviceManager.Available.Devices);
                devicesListViewEnableControl1.SetAlgorithmsListView(algorithmsListView1);
                devicesListViewEnableControl1.IsSettingsCopyEnabled = true;
            }

            // Add language selections list
            {
                var lang = International.GetAvailableLanguages();

                comboBox_Language.Items.Clear();
                for (var i = 0; i < lang.Count; i++)
                {
                    comboBox_Language.Items.Add(lang[(LanguageType) i]);
                }
            }

            // Add time unit selection list
            {
                var timeunits = new Dictionary<TimeUnitType, string>();

                foreach (TimeUnitType timeunit in Enum.GetValues(typeof(TimeUnitType)))
                {
                    timeunits.Add(timeunit, International.GetText(timeunit.ToString()));
                    comboBox_TimeUnit.Items.Add(timeunits[timeunit]);
                }
            }

            // ComboBox
            {
                comboBox_Language.SelectedIndex = (int) ConfigManager.GeneralConfig.Language;
                //comboBox_ServiceLocation.SelectedIndex = ConfigManager.GeneralConfig.ServiceLocation;
                comboBox_TimeUnit.SelectedItem = International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
                currencyConverterCombobox.SelectedItem = ConfigManager.GeneralConfig.DisplayCurrency;
                comboBox_ColorProfile.SelectedIndex = ConfigManager.GeneralConfig.ColorProfileIndex;
                comboBox_switching_algorithms.SelectedIndex = ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex;
                comboBox_devices_count.SelectedIndex = ConfigManager.GeneralConfig.DevicesCountIndex;
            }
        }

        private void InitializeGeneralTab()
        {
            InitializeGeneralTabTranslations();
            InitializeGeneralTabCallbacks();
            InitializeGeneralTabFieldValuesReferences();
        }

        #endregion //Tab General

        #region Tab Devices

        private void InitializeDevicesTab()
        {
            InitializeDevicesCallbacks();
        }

        private void InitializeDevicesCallbacks()
        {
            devicesListViewEnableControl1.SetDeviceSelectionChangedCallback(DevicesListView1_ItemSelectionChanged);
        }

        #endregion //Tab Devices

        #endregion // Initializations

        #region Form Callbacks

        #region Tab General

        private void GeneralCheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            // indicate there has been a change
            IsChange = true;
          //  ConfigManager.GeneralConfig.DebugConsole = checkBox_DebugConsole.Checked;
            ConfigManager.GeneralConfig.AutoStartMining = checkBox_AutoStartMining.Checked;
//            label_AutoStartMiningDelay.Enabled = checkBox_AutoStartMining.Checked;
            textBox_AutoStartMiningDelay.Enabled = checkBox_AutoStartMining.Checked;
            ConfigManager.GeneralConfig.HideMiningWindows = checkBox_HideMiningWindows.Checked;
            ConfigManager.GeneralConfig.MinimizeToTray = checkBox_MinimizeToTray.Checked;
            ConfigManager.GeneralConfig.AlwaysOnTop = checkBox_AlwaysOnTop.Checked;
            ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionNVIDIA =
                checkBox_DisableDetectionNVIDIA.Checked;
            ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionAMD = checkBox_DisableDetectionAMD.Checked;
            ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionCPU = checkBox_DisableDetectionCPU.Checked;
            ConfigManager.GeneralConfig.AutoScaleBTCValues = checkBox_AutoScaleBTCValues.Checked;
            ConfigManager.GeneralConfig.StartMiningWhenIdle = checkBox_StartMiningWhenIdle.Checked;
          //  ConfigManager.GeneralConfig.ShowDriverVersionWarning = checkBox_ShowDriverVersionWarning.Checked;
          //  ConfigManager.GeneralConfig.DisableWindowsErrorReporting = checkBox_DisableWindowsErrorReporting.Checked;
          //  ConfigManager.GeneralConfig.ShowInternetConnectionWarning = checkBox_ShowInternetConnectionWarning.Checked;
            ConfigManager.GeneralConfig.NVIDIAP0State = checkBox_NVIDIAP0State.Checked;
            ConfigManager.GeneralConfig.LogToFile = checkBox_LogToFile.Checked;
         //   ConfigManager.GeneralConfig.IdleWhenNoInternetAccess = checkBox_IdleWhenNoInternetAccess.Checked;
          //  ConfigManager.GeneralConfig.UseIFTTT = checkBox_UseIFTTT.Checked;
            ConfigManager.GeneralConfig.AllowMultipleInstances = checkBox_AllowMultipleInstances.Checked;
            ConfigManager.GeneralConfig.MinimizeMiningWindows = checkBox_MinimizeMiningWindows.Checked;
            ConfigManager.GeneralConfig.RunScriptOnCUDA_GPU_Lost = checkBox_RunScriptOnCUDA_GPU_Lost.Checked;
            ConfigManager.GeneralConfig.Allow_remote_management = checkBox_Allow_remote_management.Checked;
            ConfigManager.GeneralConfig.Send_actual_version_info = checkBox_Send_actual_version_info.Checked;
            ConfigManager.GeneralConfig.Force_mining_if_nonprofitable = checkBox_Force_mining_if_nonprofitable.Checked;
            ConfigManager.GeneralConfig.DecreasePowerCost = checkBox_Show_profit_with_power_consumption.Checked;
            ConfigManager.GeneralConfig.FiatCurrency = checkBox_fiat.Checked;
            ConfigManager.GeneralConfig.Additional_info_about_device = checkBox_Additional_info_about_device.Checked;
            ConfigManager.GeneralConfig.Save_windows_size_and_position = Checkbox_Save_windows_size_and_position.Checked;
            ConfigManager.GeneralConfig.ColumnSort = checkBox_sorting_list_of_algorithms.Checked;
            ConfigManager.GeneralConfig.ShowFanAsPercent = checkBox_ShowFanAsPercent.Checked;
            ConfigManager.GeneralConfig.Group_same_devices = checkbox_Group_same_devices.Checked;
            ConfigManager.GeneralConfig.Disable_extra_launch_parameter_checking = checkBox_Disable_extra_launch_parameter_checking.Checked;
           // ConfigManager.GeneralConfig.DisableAMDTempControl = checkBox_AMD_DisableAMDTempControl.Checked;
            ConfigManager.GeneralConfig.UseEthlargement = checkBox_RunEthlargement.Checked;
            if (checkBox_LogToFile.Checked)
            {
                textBox_LogMaxFileSize.Enabled = true;
            } else
            {
                textBox_LogMaxFileSize.Enabled = false;
            }
        }
        /*
        private void CheckBox_AMD_DisableAMDTempControl_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;

            // indicate there has been a change
            IsChange = true;
            ConfigManager.GeneralConfig.DisableAMDTempControl = checkBox_AMD_DisableAMDTempControl.Checked;
            foreach (var cDev in ComputeDeviceManager.Available.Devices)
            {
                if (cDev.DeviceType == DeviceType.AMD)
                {
                    foreach (var algorithm in cDev.GetAlgorithmSettings())
                    {
                        if (algorithm.NiceHashID != AlgorithmType.DaggerHashimoto)
                        {
                            algorithm.ExtraLaunchParameters += AmdGpuDevice.TemperatureParam;
                            algorithm.ExtraLaunchParameters = ExtraLaunchParametersParser.ParseForMiningPair(
                                new MiningPair(cDev, algorithm), algorithm.NiceHashID, DeviceType.AMD, false);
                        }
                    }
                }
            }
        }
        */
        private void CheckBox_DisableDefaultOptimizations_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;

            // indicate there has been a change
            IsChange = true;
           // ConfigManager.GeneralConfig.DisableDefaultOptimizations = checkBox_DisableDefaultOptimizations.Checked;
            if (ConfigManager.GeneralConfig.DisableDefaultOptimizations)
            {
                foreach (var cDev in ComputeDeviceManager.Available.Devices)
                {
                    foreach (var algorithm in cDev.GetAlgorithmSettings())
                    {
                        algorithm.ExtraLaunchParameters = "";
                        if (cDev.DeviceType == DeviceType.AMD && algorithm.NiceHashID != AlgorithmType.DaggerHashimoto)
                        {
                            algorithm.ExtraLaunchParameters += AmdGpuDevice.TemperatureParam;
                            algorithm.ExtraLaunchParameters = ExtraLaunchParametersParser.ParseForMiningPair(
                                new MiningPair(cDev, algorithm), algorithm.NiceHashID, cDev.DeviceType, false);
                        }
                    }
                }
            }
            else
            {
                foreach (var cDev in ComputeDeviceManager.Available.Devices)
                {
                    if (cDev.DeviceType == DeviceType.CPU) continue; // cpu has no defaults
                    var deviceDefaultsAlgoSettings = GroupAlgorithms.CreateForDeviceList(cDev);
                    foreach (var defaultAlgoSettings in deviceDefaultsAlgoSettings)
                    {
                        var toSetAlgo = cDev.GetAlgorithm(defaultAlgoSettings);
                        if (toSetAlgo != null)
                        {
                            toSetAlgo.ExtraLaunchParameters = defaultAlgoSettings.ExtraLaunchParameters;
                            toSetAlgo.ExtraLaunchParameters = ExtraLaunchParametersParser.ParseForMiningPair(
                                new MiningPair(cDev, toSetAlgo), toSetAlgo.NiceHashID, cDev.DeviceType, false);
                        }
                    }
                }
            }
        }

        private void checkBox_RunAtStartup_CheckedChanged_1(object sender, EventArgs e)
        {
            _isStartupChanged = true;
        }

        private bool IsInStartupRegistry()
        {
            // Value is stored in registry
            var startVal = "";
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            try
            {
                startVal = (string) runKey.GetValue(Application.ProductName);
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("REGISTRY", e.ToString());
            }
            return startVal == Application.ExecutablePath;
        }

        private void GeneralTextBoxes_Leave(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            IsChange = true;
           // if (ConfigManager.GeneralConfig.BitcoinAddress != textBox_BitcoinAddress.Text.Trim()) _isCredChange = true;
           // ConfigManager.GeneralConfig.BitcoinAddress = textBox_BitcoinAddress.Text.Trim();
           /*
            if (ConfigManager.GeneralConfig.BitcoinAddressNew != textBox_BitcoinAddressNew.Text.Trim()) _isCredChange = true;
            ConfigManager.GeneralConfig.BitcoinAddressNew = textBox_BitcoinAddressNew.Text.Trim();
            if (ConfigManager.GeneralConfig.WorkerName != textBox_WorkerName.Text.Trim()) _isCredChange = true;
            ConfigManager.GeneralConfig.WorkerName = textBox_WorkerName.Text.Trim();
            */
            ConfigManager.GeneralConfig.SwitchSmaTimeChangeSeconds.Upper =
                Helpers.ParseInt(textBox_SwitchMaxSeconds.Text);
            ConfigManager.GeneralConfig.SwitchSmaTimeChangeSeconds.Lower = Helpers.ParseInt(textBox_SwitchMinSeconds.Text);
           // ConfigManager.GeneralConfig.MinerAPIQueryInterval = Helpers.ParseInt(textBox_MinerAPIQueryInterval.Text);
            ConfigManager.GeneralConfig.MinerRestartDelayMS = Helpers.ParseInt(textBox_MinerRestartDelayMS.Text);
            ConfigManager.GeneralConfig.MinIdleSeconds = Helpers.ParseInt(textBox_MinIdleSeconds.Text);
            ConfigManager.GeneralConfig.LogMaxFileSize = Helpers.ParseLong(textBox_LogMaxFileSize.Text);
            ConfigManager.GeneralConfig.AutoStartMiningDelay = Helpers.ParseInt(textBox_AutoStartMiningDelay.Text);

            /*
            ConfigManager.GeneralConfig.ethminerDefaultBlockHeight =
                Helpers.ParseInt(textBox_ethminerDefaultBlockHeight.Text);
                */
            ConfigManager.GeneralConfig.ApiBindPortPoolStart = Helpers.ParseInt(textBox_APIBindPortStart.Text);
            // min profit
            ConfigManager.GeneralConfig.MinimumProfit = Helpers.ParseDouble(textBox_MinProfit.Text);
            ConfigManager.GeneralConfig.SwitchProfitabilityThreshold =
                Helpers.ParseDouble(textBox_SwitchProfitabilityThreshold.Text) / 100;

          //  ConfigManager.GeneralConfig.IFTTTKey = textBox_IFTTTKey.Text.Trim();

            ConfigManager.GeneralConfig.KwhPrice = Helpers.ParseDouble(textBox_ElectricityCost.Text);
            ConfigManager.GeneralConfig.PowerMB = Helpers.ParseInt(textBox_mb.Text);
            ConfigManager.GeneralConfig.PowerPSU = Helpers.ParseInt(textBox_psu.Text);

            // Fix bounds
            ConfigManager.GeneralConfig.FixSettingBounds();
            // update strings
            textBox_MinProfit.Text =
                ConfigManager.GeneralConfig.MinimumProfit.ToString("F2").Replace(',', '.'); // force comma
            textBox_SwitchProfitabilityThreshold.Text = (ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100)
                .ToString("F2").Replace(',', '.'); // force comma
            textBox_SwitchMaxSeconds.Text = ConfigManager.GeneralConfig.SwitchSmaTimeChangeSeconds.Upper.ToString();
            textBox_SwitchMinSeconds.Text = ConfigManager.GeneralConfig.SwitchSmaTimeChangeSeconds.Lower.ToString();
          //  textBox_MinerAPIQueryInterval.Text = ConfigManager.GeneralConfig.MinerAPIQueryInterval.ToString();
            textBox_MinerRestartDelayMS.Text = ConfigManager.GeneralConfig.MinerRestartDelayMS.ToString();
            textBox_MinIdleSeconds.Text = ConfigManager.GeneralConfig.MinIdleSeconds.ToString();
            textBox_LogMaxFileSize.Text = ConfigManager.GeneralConfig.LogMaxFileSize.ToString();
            textBox_AutoStartMiningDelay.Text = ConfigManager.GeneralConfig.AutoStartMiningDelay.ToString();
            //      textBox_ethminerDefaultBlockHeight.Text = ConfigManager.GeneralConfig.ethminerDefaultBlockHeight.ToString();
            textBox_APIBindPortStart.Text = ConfigManager.GeneralConfig.ApiBindPortPoolStart.ToString();
            textBox_ElectricityCost.Text = ConfigManager.GeneralConfig.KwhPrice.ToString("0.0000");
            textBox_psu.Text = ConfigManager.GeneralConfig.PowerPSU.ToString("");
            textBox_mb.Text = ConfigManager.GeneralConfig.PowerMB.ToString("");
        }

        private void GeneralComboBoxes_Leave(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            IsChange = true;
            ConfigManager.GeneralConfig.Language = (LanguageType) comboBox_Language.SelectedIndex;
            //ConfigManager.GeneralConfig.ServiceLocation = comboBox_ServiceLocation.SelectedIndex;
            ConfigManager.GeneralConfig.ColorProfileIndex = comboBox_ColorProfile.SelectedIndex;
            ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex = comboBox_switching_algorithms.SelectedIndex;
            ConfigManager.GeneralConfig.DevicesCountIndex = comboBox_devices_count.SelectedIndex;
            ConfigManager.GeneralConfig.TimeUnit = (TimeUnitType) comboBox_TimeUnit.SelectedIndex;
            /*
            ConfigManager.GeneralConfig.EthminerDagGenerationType =
                (DagGenerationType) comboBox_DagLoadMode.SelectedIndex;
                */
        }

        private void ComboBox_CPU0_ForceCPUExtension_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmbbox = (ComboBox) sender;
            ConfigManager.GeneralConfig.ForceCPUExtension = (CpuExtensionType) cmbbox.SelectedIndex;
        }

        #endregion //Tab General


        #region Tab Device

        private void DevicesListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            algorithmSettingsControl1.Deselect();
            // show algorithms
            _selectedComputeDevice =
                ComputeDeviceManager.Available.GetCurrentlySelectedComputeDevice(e.ItemIndex, ShowUniqueDeviceList);
            algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
            groupBoxAlgorithmSettings.Text = string.Format(International.GetText("FormSettings_AlgorithmsSettings"),
                _selectedComputeDevice.Name);
        }

        private void ButtonSelectedProfit_Click(object sender, EventArgs e)
        {
            if (_selectedComputeDevice == null)
            {
                MessageBox.Show(International.GetText("FormSettings_ButtonProfitSingle"),
                    International.GetText("Warning_with_Exclamation"),
                    MessageBoxButtons.OK);
                return;
            }

            var url = Links.NhmProfitCheck + _selectedComputeDevice.Name;
            foreach (var algorithm in _selectedComputeDevice.GetAlgorithmSettingsFastest())
            {
                var id = (int) algorithm.NiceHashID;
                url += "&speed" + id + "=" + ProfitabilityCalculator
                           .GetFormatedSpeed(algorithm.BenchmarkSpeed, algorithm.NiceHashID)
                           .ToString("F2", CultureInfo.InvariantCulture);
            }

            url += "&nhmver=" + Application.ProductVersion; // Add version info
            url += "&cost=1&power=1"; // Set default power and cost to 1
            System.Diagnostics.Process.Start(url);
        }

        private void ButtonGPUtuning_Click(object sender, EventArgs e)
        {
             System.Diagnostics.Process.Start("GPU-Tuning.exe");
        }

        #endregion //Tab Device


        private void ToolTip1_Popup(object sender, PopupEventArgs e)
        {
            toolTip1.ToolTipTitle = International.GetText("Form_Settings_ToolTip_Explaination");
        }

        #region Form Buttons

        private void ButtonDefaults_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(International.GetText("Form_Settings_buttonDefaultsMsg"),
                International.GetText("Form_Settings_buttonDefaultsTitle"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                IsChange = true;
                IsChangeSaved = true;
                ConfigManager.GeneralConfig.SetDefaults();

                International.Initialize(ConfigManager.GeneralConfig.Language);
                InitializeGeneralTabFieldValuesReferences();
                InitializeGeneralTabTranslations();
            }
        }

        private void ButtonSaveClose_Click(object sender, EventArgs e)
        {

            MessageBox.Show(International.GetText("Form_Settings_buttonSaveMsg"),
                International.GetText("Form_Settings_buttonSaveTitle"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsChange = true;
            IsChangeSaved = true;

            if (_isCredChange)
            {
                //if (ConfigManager.GeneralConfig.NewPlatform)
                if (true)
                {
                    NiceHashStats.SetCredentials(ConfigManager.GeneralConfig.BitcoinAddressNew.Trim(),
                    ConfigManager.GeneralConfig.WorkerName.Trim());
                }
                /*
                else
                {
                    NiceHashStats.SetCredentials(ConfigManager.GeneralConfig.BitcoinAddress.Trim(),
                                       ConfigManager.GeneralConfig.WorkerName.Trim());
                }
                */
            }
            if (UpdateListView_timer != null)
            {
                UpdateListView_timer.Stop();
                UpdateListView_timer = null;
            }
            Form_Settings.ActiveForm.Close();
         //   devicesListViewEnableControl1.Refresh();
        }

        private void ButtonCloseNoSave_Click(object sender, EventArgs e)
        {
            if (UpdateListView_timer != null)
            {
                UpdateListView_timer.Stop();
                UpdateListView_timer = null;
            }
            IsChangeSaved = false;
            Close();
        }

        #endregion // Form Buttons

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsChange && !IsChangeSaved)
            {
                var result = MessageBox.Show(International.GetText("Form_Settings_buttonCloseNoSaveMsg"),
                    International.GetText("Form_Settings_buttonCloseNoSaveTitle"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (UpdateListView_timer != null)
            {
                UpdateListView_timer.Stop();
                UpdateListView_timer = null;
            }

            if (Form_Benchmark.ActiveForm != null)
            {
                if (ConfigManager.GeneralConfig.Save_windows_size_and_position)
                {
                    ConfigManager.GeneralConfig.SettingsFormHeight = Form_Settings.ActiveForm.Height;
                    ConfigManager.GeneralConfig.SettingsFormWidth = Form_Settings.ActiveForm.Width;
                    ConfigManager.GeneralConfig.SettingsFormTop = Form_Settings.ActiveForm.Top;
                    ConfigManager.GeneralConfig.SettingsFormLeft = Form_Settings.ActiveForm.Left;
                    ConfigManager.GeneralConfigFileCommit();
                }
            }
            // check restart parameters change
            IsRestartNeeded = ConfigManager.IsRestartNeeded();

            if (IsChangeSaved)
            {

            ConfigManager.GeneralConfigFileCommit();
                ConfigManager.CommitBenchmarks();
                International.Initialize(ConfigManager.GeneralConfig.Language);

                if (_isStartupChanged)
                {
                    // Commit to registry
                    try
                    {
                        if (checkBox_RunAtStartup.Checked)
                        {
                            // Add NHML to startup registry
                            _rkStartup?.SetValue(Application.ProductName, Application.ExecutablePath);
                        }
                        else
                        {
                            _rkStartup?.DeleteValue(Application.ProductName, false);
                        }
                    }
                    catch (Exception er)
                    {
                        Helpers.ConsolePrint("REGISTRY", er.ToString());
                    }
                }
            }
            else
            {
                ConfigManager.RestoreBackup();
            }
        }

        private void CurrencyConverterCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = currencyConverterCombobox.SelectedItem.ToString();
            ConfigManager.GeneralConfig.DisplayCurrency = selected;
        }

        #endregion Form Callbacks

        private void TabControlGeneral_Selected(object sender, TabControlEventArgs e)
        {
            // set first device selected {
            if (ComputeDeviceManager.Available.Devices.Count > 0)
            {
                algorithmSettingsControl1.Deselect();
            }
        }

        private void CheckBox_Use3rdPartyMiners_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            /*
            if (checkBox_Use3rdPartyMiners.Checked)
            {
                // Show TOS
                Form tos = new Form_3rdParty_TOS();
                tos.ShowDialog(this);
                checkBox_Use3rdPartyMiners.Checked =
                    ConfigManager.GeneralConfig.Use3rdPartyMiners == Use3rdPartyMiners.YES;
            }
            else
            {
                ConfigManager.GeneralConfig.Use3rdPartyMiners = Use3rdPartyMiners.NO;
            }
            */
        }

        private void CheckBox_HideMiningWindows_CheckChanged(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            IsChange = true;
            ConfigManager.GeneralConfig.HideMiningWindows = checkBox_HideMiningWindows.Checked;
            checkBox_MinimizeMiningWindows.Enabled = !checkBox_HideMiningWindows.Checked;
        }
        /*
        private void CheckBox_UseIFTTT_CheckChanged(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            IsChange = true;

            ConfigManager.GeneralConfig.UseIFTTT = checkBox_UseIFTTT.Checked;

            textBox_IFTTTKey.Enabled = checkBox_UseIFTTT.Checked;
        }
        */
        private void algorithmsListView1_Load(object sender, EventArgs e)
        {

        }

        private void groupBoxAlgorithmSettings_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox_AutoStartMining_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_RunScriptOnCUDA_GPU_Lost_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_Send_actual_version_info_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void algorithmSettingsControl1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox_Force_mining_if_nonprofitable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox_AutoStartMiningDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
        char number = e.KeyChar;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void radioButtonMOPA1_CheckedChanged_1(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.MOPA1 = radioButtonMOPA1.Checked;
        }

        private void radioButtonMOPA2_CheckedChanged_1(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.MOPA2 = radioButtonMOPA2.Checked;
        }

        private void radioButtonMOPA3_CheckedChanged_1(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.MOPA3 = radioButtonMOPA3.Checked;
        }

        private void radioButtonMOPA4_CheckedChanged_1(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.MOPA4 = radioButtonMOPA4.Checked;
        }

        private void textBox_BitcoinAddressNew_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox_ElectricityCost_Click(object sender, EventArgs e)
        {

        }

        private void textBox_ElectricityCost_TextChanged(object sender, EventArgs e)
        {

        }

        private void label_ElectricityCost_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox_TimeUnit_Click(object sender, EventArgs e)
        {

        }

        private void label_TimeUnit_Click(object sender, EventArgs e)
        {

        }

        private void comboBox_TimeUnit_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label_IFTTTAPIKey_Click(object sender, EventArgs e)
        {

        }

        private void textBox_IFTTTKey_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox_UseIFTTT_Click(object sender, EventArgs e)
        {

        }

        private void checkBox_UseIFTTT_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox_WorkerName_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox_MinProfit_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox_ServiceLocation_Click(object sender, EventArgs e)
        {

        }

        private void textBox_MinProfit_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox_IdleWhenNoInternetAccess_Click(object sender, EventArgs e)
        {

        }

        private void label_MinProfit_Click(object sender, EventArgs e)
        {

        }

        private void label_WorkerName_Click(object sender, EventArgs e)
        {

        }

        private void label_ServiceLocation_Click(object sender, EventArgs e)
        {

        }

        private void comboBox_ServiceLocation_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void comboBox_ColorProfile_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void textBox_WorkerName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label_BitcoinAddressNew_Click(object sender, EventArgs e)
        {

        }

        private void devicesListViewEnableControl1_Load(object sender, EventArgs e)
        {

        }
        private Dictionary<TabPage, Color> TabColors = new Dictionary<TabPage, Color>();
        private void SetTabHeader(TabPage page, Color color)
        {
            TabColors[page] = color;
            tabControlGeneral.Invalidate();
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //  using (Brush br = new SolidBrush(tabColorDictionary[tabControlGeneral.TabPages[e.Index]]))
            var bc = new SolidBrush(Form_Main._backColor);
            var fc = new Pen(Form_Main._backColor);
            var wc = new SolidBrush(Form_Main._windowColor);
            var tc = new SolidBrush(Form_Main._textColor);
            var gr = new SolidBrush(Color.Gray);
            using (Brush br = bc)
            {
                e.Graphics.FillRectangle(br, e.Bounds);
                SizeF sz = e.Graphics.MeasureString(tabControlGeneral.TabPages[e.Index].Text, e.Font);
                e.Graphics.DrawString(tabControlGeneral.TabPages[e.Index].Text, e.Font, tc, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

                Rectangle rect = e.Bounds;
                rect.Offset(0, 1);
                rect.Inflate(0, -1);
                e.Graphics.DrawRectangle(fc, rect);
                e.DrawFocusRectangle();
            }

        }
        private void tabpage_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush fillBrush = new SolidBrush(Form_Main._backColor);
            e.Graphics.FillRectangle(fillBrush, e.ClipRectangle);
        }

        private void Form_Settings_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush fillBrush = new SolidBrush(Form_Main._backColor);
            e.Graphics.FillRectangle(fillBrush, e.ClipRectangle);
        }

      //  private void tabControlGeneral_DrawItem(object sender, DrawItemEventArgs e)
       // {
            private void tabControlGeneral_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
            {
                TabPage CurrentTab = tabControlGeneral.TabPages[e.Index];
                Rectangle ItemRect = tabControlGeneral.GetTabRect(e.Index);
                SolidBrush FillBrush = new SolidBrush(Color.Red);
                SolidBrush TextBrush = new SolidBrush(Color.White);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                //If we are currently painting the Selected TabItem we'll
                //change the brush colors and inflate the rectangle.
                if (System.Convert.ToBoolean(e.State & DrawItemState.Selected))
                {
                    FillBrush.Color = Color.White;
                    TextBrush.Color = Color.Red;
                    ItemRect.Inflate(2, 2);
                }

                //Set up rotation for left and right aligned tabs
                if (tabControlGeneral.Alignment == TabAlignment.Left || tabControlGeneral.Alignment == TabAlignment.Right)
                {
                    float RotateAngle = 90;
                    if (tabControlGeneral.Alignment == TabAlignment.Left)
                        RotateAngle = 270;
                    PointF cp = new PointF(ItemRect.Left + (ItemRect.Width / 2), ItemRect.Top + (ItemRect.Height / 2));
                    e.Graphics.TranslateTransform(cp.X, cp.Y);
                    e.Graphics.RotateTransform(RotateAngle);
                    ItemRect = new Rectangle(-(ItemRect.Height / 2), -(ItemRect.Width / 2), ItemRect.Height, ItemRect.Width);
                }

                //Next we'll paint the TabItem with our Fill Brush
                e.Graphics.FillRectangle(FillBrush, ItemRect);

                //Now draw the text.
                e.Graphics.DrawString(CurrentTab.Text, e.Font, TextBrush, (RectangleF)ItemRect, sf);

                //Reset any Graphics rotation
                e.Graphics.ResetTransform();

                //Finally, we should Dispose of our brushes.
                FillBrush.Dispose();
                TextBrush.Dispose();
            }

        private void comboBox_ServiceLocation_DrawItem(object sender, DrawItemEventArgs e)
        {
            var cmb = (ComboBox)sender;
            if (cmb == null) return;


            e.DrawBackground();

            // change background color
            var bc = new SolidBrush(Form_Main._backColor);
            var fc = new SolidBrush(Form_Main._foreColor);
            var wc = new SolidBrush(Form_Main._windowColor);
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

        private void comboBox_TimeUnit_DrawItem(object sender, DrawItemEventArgs e)
        {
            comboBox_ServiceLocation_DrawItem(sender, e);
        }

        private void comboBox_Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            devicesListViewEnableControl1.InitLocale();
        }

        private void comboBox_Language_DrawItem(object sender, DrawItemEventArgs e)
        {
            comboBox_ServiceLocation_DrawItem(sender, e);
        }

        private void currencyConverterCombobox_DrawItem(object sender, DrawItemEventArgs e)
        {
            comboBox_ServiceLocation_DrawItem(sender, e);
        }

        private void comboBox_ColorProfile_DrawItem(object sender, DrawItemEventArgs e)
        {
            Color _backColor;
            Color _foreColor;
            Color _windowColor;
            Color _textColor;
            switch (e.Index)
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
                    _textColor = ConfigManager.GeneralConfig.ColorProfiles.DarkGreen[3];
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButtonMOPA5_CheckedChanged_1(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.MOPA5 = radioButtonMOPA5.Checked;
        }

        private void checkBox_RunEthlargement_CheckedChanged(object sender, EventArgs e)
        {
            if (Enabled == false)
            {
                label_RunEthlargement.ForeColor = Color.Gray;
            } else
            {
                label_RunEthlargement.ForeColor = Form_Main._foreColor;
            }
        }

        private void buttonGPUtuning_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("GPU-Tuning.exe");
        }

        private void textBox_MinIdleSeconds_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_SwitchProfitabilityThreshold_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form_Settings_Deactivate(object sender, EventArgs e)
        {
            /*
            if (UpdateListView_timer != null)
            {
                UpdateListView_timer.Stop();
                UpdateListView_timer = null;
            }
            */
        }

        private void label_MinIdleSeconds_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox_MinIdleSeconds_Click(object sender, EventArgs e)
        {

        }

        private void groupBox_Misc_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox_switching_algorithms_DrawItem(object sender, DrawItemEventArgs e)
        {
            comboBox_ServiceLocation_DrawItem(sender, e);
        }

        private void textBox_MinerRestartDelayMS_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_Show_profit_with_power_consumption_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label_LogMaxFileSize_Click(object sender, EventArgs e)
        {

        }

        private void groupBox_Main_Enter(object sender, EventArgs e)
        {

        }

        private void label_ElectricityCost_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox_ElectricityCost_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox_Force_mining_if_nonprofitable_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void pictureBox_ElectricityCost_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void comboBox_devices_count_DrawItem(object sender, DrawItemEventArgs e)
        {
            comboBox_ServiceLocation_DrawItem(sender, e);
        }

        private void buttonLicence_Click(object sender, EventArgs e)
        {
            Form ifrm = new Form_ChooseLanguage(false);
            ifrm.Show();
        }

        private void richTextBoxInfo_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void buttonCheckNewVersion_Click(object sender, EventArgs e)
        {
            Double.TryParse(NiceHashStats.GetVersion().Item1, out Form_Main.githubVersion);
            Form_Main.buildD = NiceHashStats.GetVersion().Item2;

            linkLabelNewVersion.Text = "No new version or build";
            if (Form_Main.buildDcurrent < Form_Main.buildD)
            {
                linkLabelNewVersion.Text = "New build: " + Form_Main.buildD.ToString();
                buttonUpdate.Visible = true;
            }
            var programVersion = ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",", ".");
            if (ConfigManager.GeneralConfig.ForkFixVersion < Form_Main.githubVersion)
            {
                linkLabelNewVersion.Text = "New version: " + Form_Main.githubVersion.ToString();
                buttonUpdate.Visible = true;
            }

            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                linkLabelNewVersion.Text = "Нет новой версии или сборки";
                buttonUpdate.Visible = false;
                if (Form_Main.buildDcurrent > Form_Main.buildD)
                {
                    linkLabelNewVersion.Text = "Новая сборка: " + Form_Main.buildD.ToString();
                    buttonUpdate.Visible = true;
                }
                if (ConfigManager.GeneralConfig.ForkFixVersion < Form_Main.githubVersion)
                {
                    linkLabelNewVersion.Text = "Новая версия: " + Form_Main.githubVersion.ToString();
                    buttonUpdate.Visible = true;
                }
                if (Form_Main.githubVersion <= 0)
                {
                    linkLabelNewVersion.Text = "Ошибка проверки новой версии";
                    buttonUpdate.Visible = false;
                }
            }
            linkLabelNewVersion.Update();
        }

        private void linkLabelNewVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/angelbbs/NiceHashMinerLegacy/releases");
        }

        private void linkLabelCurrentVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/angelbbs/NiceHashMinerLegacy/releases");
        }

        private void linkLabelCurrentVersion_MouseEnter(object sender, EventArgs e)
        {
        }
    }
}
