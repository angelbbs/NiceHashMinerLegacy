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
using System.IO;
using System.Threading.Tasks;

namespace NiceHashMiner.Forms
{
    public partial class Form_Settings : Form
    {
        private readonly bool _isInitFinished = false;
        private bool _isChange = false;
        public static ProgressBar ProgressProgramUpdate { get; set; }

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
        public static bool FormSettingsMoved = false;
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

            ProgressProgramUpdate = progressBarUpdate;

            // set first device selected {
            if (ComputeDeviceManager.Available.Devices.Count > 0)
            {
                
                _selectedComputeDevice = ComputeDeviceManager.Available.Devices[0];
                algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
                groupBoxAlgorithmSettings.Text = string.Format(International.GetText("FormSettings_AlgorithmsSettings"),
                    _selectedComputeDevice.Name);
                // groupBoxAlgorithmSettings.ForeColor = Form_Main._foreColor;
                //if (_selectedComputeDevice.DeviceType != DeviceType.CPU)

                        algorithmsListViewOverClock1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
                        groupBoxOverClockSettings.Text = string.Format(International.GetText("FormSettings_OverclockSettings"),
                            _selectedComputeDevice.Name);

                //groupBoxAlgorithmSettings.Text = "";
                //groupBoxOverClockSettings.Text = "";
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
            if (Form_Settings.ActiveForm != null)
            {
                Form_Settings.ActiveForm.Update();
            }
            if (ConfigManager.GeneralConfig.AlwaysOnTop) this.TopMost = true;

            if (UpdateListView_timer == null)
            {
                UpdateListView_timer = new Timer();
                UpdateListView_timer.Tick += UpdateLvi_Tick;
                UpdateListView_timer.Interval = 100;
                UpdateListView_timer.Start();
            }

            thisProc.PriorityClass = ProcessPriorityClass.Normal;
            thisProc.Dispose();
        }
        private void UpdateLvi_Tick(object sender, EventArgs e)
        {
            algorithmsListView1.UpdateLvi();
            if (Form_Main.DaggerHashimoto3GBVisible && Form_Main.DaggerHashimotoMaxEpochUpdated)
            {
                labelMaxEpoch.Text = International.GetText("Form_Settings_MaxEpoch") + "3GB";
                textBoxMaxEpoch.Text = ConfigManager.GeneralConfig.DaggerHashimoto3GBMaxEpoch.ToString();
                labelMaxEpoch.Visible = textBoxMaxEpoch.Visible = true;
                Form_Main.DaggerHashimotoMaxEpochUpdated = false;
            } else
            if (Form_Main.DaggerHashimoto4GBVisible && Form_Main.DaggerHashimotoMaxEpochUpdated)
            {
                textBoxMaxEpoch.Text = ConfigManager.GeneralConfig.DaggerHashimoto4GBMaxEpoch.ToString();
                labelMaxEpoch.Text = International.GetText("Form_Settings_MaxEpoch") + "4GB";
                labelMaxEpoch.Visible = textBoxMaxEpoch.Visible = true;
                Form_Main.DaggerHashimotoMaxEpochUpdated = false;
            } else
            if (Form_Main.DaggerHashimoto1070Visible && Form_Main.DaggerHashimotoMaxEpochUpdated)
            {
                textBoxMaxEpoch.Text = ConfigManager.GeneralConfig.DaggerHashimoto1070MaxEpoch.ToString();
                labelMaxEpoch.Text = International.GetText("Form_Settings_MaxEpoch") + "1070";
                labelMaxEpoch.Visible = textBoxMaxEpoch.Visible = true;
                Form_Main.DaggerHashimotoMaxEpochUpdated = false;
            }
            if (!Form_Main.DaggerHashimoto1070Visible && !Form_Main.DaggerHashimoto4GBVisible &&
                !Form_Main.DaggerHashimoto3GBVisible && !Form_Main.DaggerHashimotoMaxEpochUpdated)
            {
                labelMaxEpoch.Visible = textBoxMaxEpoch.Visible = false;
                Form_Main.DaggerHashimotoMaxEpochUpdated = true;
            }
        }

       #region Initializations

        private void InitializeToolTip()
        {
            // Setup Tooltips
            toolTip1.SetToolTip(comboBox_Language, International.GetText("Form_Settings_ToolTip_Language"));
            toolTip1.SetToolTip(label_Language, International.GetText("Form_Settings_ToolTip_Language"));
            toolTip1.SetToolTip(pictureBox_Language, International.GetText("Form_Settings_ToolTip_Language"));

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

            toolTip1.SetToolTip(checkBox_AllowMultipleInstances,
                International.GetText("Form_Settings_General_AllowMultipleInstances_ToolTip"));
            toolTip1.SetToolTip(pictureBox_AllowMultipleInstances,
                International.GetText("Form_Settings_General_AllowMultipleInstances_ToolTip"));

            toolTip1.SetToolTip(label_MinProfit, International.GetText("Form_Settings_ToolTip_MinimumProfit"));
            toolTip1.SetToolTip(pictureBox_MinProfit, International.GetText("Form_Settings_ToolTip_MinimumProfit"));
            toolTip1.SetToolTip(textBox_MinProfit, International.GetText("Form_Settings_ToolTip_MinimumProfit"));
            /*
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

            toolTip1.SetToolTip(textBox_LogMaxFileSize, International.GetText("Form_Settings_ToolTip_LogMaxFileSize"));
            toolTip1.SetToolTip(label_LogMaxFileSize, International.GetText("Form_Settings_ToolTip_LogMaxFileSize"));
            toolTip1.SetToolTip(pictureBox_LogMaxFileSize,
                International.GetText("Form_Settings_ToolTip_LogMaxFileSize"));
            /*
            toolTip1.SetToolTip(checkBox_NVIDIAP0State,
                International.GetText("Form_Settings_ToolTip_checkBox_NVIDIAP0State"));
            toolTip1.SetToolTip(pictureBox_NVIDIAP0State,
                International.GetText("Form_Settings_ToolTip_checkBox_NVIDIAP0State"));
                */
            toolTip1.SetToolTip(checkBox_RunAtStartup,
                International.GetText("Form_Settings_ToolTip_checkBox_RunAtStartup"));
            toolTip1.SetToolTip(pictureBox_RunAtStartup,
                International.GetText("Form_Settings_ToolTip_checkBox_RunAtStartup"));


            toolTip1.SetToolTip(checkBox_AutoStartMining,
                International.GetText("Form_Settings_ToolTip_checkBox_AutoStartMining"));
            toolTip1.SetToolTip(pictureBox_AutoStartMining,
                International.GetText("Form_Settings_ToolTip_checkBox_AutoStartMining"));

            toolTip1.SetToolTip(label_displayCurrency, International.GetText("Form_Settings_ToolTip_DisplayCurrency"));
            toolTip1.SetToolTip(pictureBox_displayCurrency,
                International.GetText("Form_Settings_ToolTip_DisplayCurrency"));
            toolTip1.SetToolTip(currencyConverterCombobox,
                International.GetText("Form_Settings_ToolTip_DisplayCurrency"));

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
            checkBox_AutoStartMining.Text = International.GetText("Form_Settings_General_AutoStartMining");
            checkBox_HideMiningWindows.Text = International.GetText("Form_Settings_General_HideMiningWindows");
            checkBox_MinimizeToTray.Text = International.GetText("Form_Settings_General_MinimizeToTray");
            checkBox_DisableDetectionNVIDIA.Text =
                string.Format(International.GetText("Form_Settings_General_DisableDetection"), "NVIDIA");
            checkBox_DisableDetectionAMD.Text =
                string.Format(International.GetText("Form_Settings_General_DisableDetection"), "AMD");
            checkBoxAMDmonitoring.Text = International.GetText("Form_Settings_General_AMDmonitoring");
            checkBoxNVMonitoring.Text = International.GetText("Form_Settings_General_NVMonitoring");
            checkBoxCPUmonitoring.Text = International.GetText("Form_Settings_General_CPUmonitoring");
            checkBox_AutoScaleBTCValues.Text = International.GetText("Form_Settings_General_AutoScaleBTCValues");
            checkBox_StartMiningWhenIdle.Text = International.GetText("Form_Settings_General_StartMiningWhenIdle");

            //checkBox_NVIDIAP0State.Text = International.GetText("Form_Settings_General_NVIDIAP0State");
            checkBox_LogToFile.Text = International.GetText("Form_Settings_General_LogToFile");

            checkBox_AllowMultipleInstances.Text =
                International.GetText("Form_Settings_General_AllowMultipleInstances_Text");
            checkBox_RunAtStartup.Text = International.GetText("Form_Settings_General_RunAtStartup");
            checkBox_MinimizeMiningWindows.Text = International.GetText("Form_Settings_General_MinimizeMiningWindows");

            checkBoxRestartDriver.Text = International.GetText("Form_Settings_checkBox_RestartDriver");
            checkBoxRestartWindows.Text = International.GetText("Form_Settings_checkBox_RestartWindows");
            checkBoxDriverWarning.Text = International.GetText("Form_Settings_General_ShowDriverVersionWarning");

            checkBoxAutoupdate.Text = International.GetText("Form_Settings_checkBoxAutoupdate");
            checkBox_BackupBeforeUpdate.Text = International.GetText("Form_Settings_checkBox_backup_before_update");
            checkBox_ABEnableOverclock.Text = International.GetText("FormSettings_ABEnableOverclock");
            checkBox_ABOverclock_Relative.Text = International.GetText("FormSettings_ABOverclock_Relative");
            checkBox_AB_ForceRun.Text = International.GetText("FormSettings_AB_ForceRun");
            checkBox_ABMinimize.Text = International.GetText("FormSettings_AB_Minimize");
            labelCheckforprogramupdatesevery.Text = International.GetText("Form_Settings_labelCheckforprogramupdatesevery");

            label_Language.Text = International.GetText("Form_Settings_General_Language") + ":";
            label1.Text = International.GetText("Form_Settings_Color_profile");

            var newver = NiceHashStats.Version.Replace(",", ".");
            var ver = Configs.ConfigManager.GeneralConfig.ForkFixVersion;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            var build = buildDate.ToString("u").Replace("-", "").Replace(":", "").Replace("Z", "").Replace(" ", ".");
            Double.TryParse(build.ToString(), out Form_Main.currentBuild);

            linkLabelCurrentVersion.LinkBehavior = LinkBehavior.NeverUnderline;
            linkLabelCurrentVersion.Text = International.GetText("Form_Settings_Currentversion") +
                ver + International.GetText("Form_Settings_Currentbuild") +
                Form_Main.currentBuild.ToString("00000000.00");

            linkLabelNewVersion2.LinkBehavior = LinkBehavior.NeverUnderline;

            buttonCreateBackup.Text = International.GetText("Form_Settings_Createbackup");
            buttonRestoreBackup.Text = International.GetText("Form_Settings_Restorebackup");

            linkLabelNewVersion2.Text = International.GetText("Form_Settings_Nonewversionorbuild");
            buttonUpdate.Visible = false;
            if (Form_Main.NewVersionExist)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Nonewversionorbuild");
                if (Form_Main.currentBuild < Form_Main.githubBuild)//testing
                {
                    linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newbuild") + Form_Main.githubBuild.ToString("00000000.00");
                    buttonUpdate.Visible = true;
                }

                if (Form_Main.currentVersion < Form_Main.githubVersion)
                {
                    linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newversion") + Form_Main.githubVersion.ToString();
                    buttonUpdate.Visible = true;
                }
                if (Form_Main.githubVersion <= 0)
                {
                    linkLabelNewVersion2.Text = International.GetText("Form_Settings_Errorwhencheckingnewversion");
                    buttonUpdate.Visible = false;
                }
                linkLabelNewVersion2.Update();
            }
            progressBarUpdate.Visible = false;

            if (Form_Main.currentBuild < Form_Main.githubBuild)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newbuild") +
                    Form_Main.githubBuild.ToString("{0:00000000.00}");
                buttonUpdate.Visible = true;
                linkLabelNewVersion2.LinkBehavior = LinkBehavior.SystemDefault;
            }
            string programVersion = ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",", ".");
            if (ConfigManager.GeneralConfig.ForkFixVersion < Form_Main.githubVersion)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newversion") + Form_Main.githubVersion.ToString();
                buttonUpdate.Visible = true;
                linkLabelNewVersion2.LinkBehavior = LinkBehavior.SystemDefault;
            }
            if (Form_Main.githubVersion <= 0)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Errorwhencheckingnewversion");
                buttonUpdate.Visible = false;
            }

            buttonRestoreBackup.Enabled = false;

            labelBackupCopy.Text = International.GetText("Form_Settings_Noavailablebackupcopy");
            try
            {
                if (Directory.Exists("backup"))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo("backup");
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        if (file.Name.Contains("backup_") && file.Name.Contains(".zip"))
                        {
                            buttonRestoreBackup.Enabled = true;
                            Form_Main.BackupFileName = file.Name.Replace("backup_", "").Replace(".zip", "");
                            Form_Main.BackupFileDate = file.CreationTime.ToString("dd.MM.yyyy HH:mm");
                            labelBackupCopy.Text = International.GetText("Form_Settings_Backupcopy") + Form_Main.BackupFileName +
                                " (" + Form_Main.BackupFileDate + ")";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("Backup", ex.ToString());
            }

            checkBox_Allow_remote_management.Text = International.GetText("Form_Settings_checkBox_Allow_remote_management");
            checkBox_Send_actual_version_info.Text = International.GetText("Form_Settings_checkBox_Send_actual_version_info");
            checkBox_Additional_info_about_device.Text = International.GetText("Form_Settings_checkBox_Additional_info_about_device");

            checkBox_show_NVdevice_manufacturer.Text = International.GetText("Form_Settings_checkBox_show_NVdevice_manufacturer");
            checkBox_show_AMDdevice_manufacturer.Text = International.GetText("Form_Settings_checkBox_show_AMDdevice_manufacturer");
            checkBox_ShowDeviceMemSize.Text = International.GetText("Form_Settings_checkBox_show_device_memsize");
            //checkBox_ShowDeviceBusId.Text = International.GetText("Form_Settings_checkBox_show_device_busId");

            checkbox_Use_OpenHardwareMonitor.Text = International.GetText("Form_Settings_checkbox_Use_OpenHardwareMonitor");
            Checkbox_Save_windows_size_and_position.Text = International.GetText("Form_Settings_Checkbox_Save_windows_size_and_position");
            checkBox_sorting_list_of_algorithms.Text = International.GetText("Form_Settings_checkBox_sorting_list_of_algorithms");
            checkBox_ShowUptime.Text = International.GetText("Form_Settings_checkBox_ShowUptime");
            checkBox_program_monitoring.Text = International.GetText("Form_Settings_checkBox_program_monitoring");
            checkBox_ShowFanAsPercent.Text = International.GetText("Form_Settings_checkBox_ShowFanAsPercent");
            checkbox_Group_same_devices.Text = International.GetText("Form_Settings_checkbox_Group_same_devices");
            checkBox_By_profitability_of_all_devices.Text = International.GetText("FormSettings_By_profitability_of_all_devices");
            checkBox_Force_mining_if_nonprofitable.Text = International.GetText("Form_Settings_checkBox_Force_mining_if_nonprofitable");
            checkbox_current_actual_profitabilities.Text = International.GetText("Form_Main_Checkbox_current_actual_profitabilities");
            checkBox_Show_profit_with_power_consumption.Text = International.GetText("Form_Settings_checkBox_Show_profit_with_power_consumption");
            checkBox_fiat.Text = International.GetText("Form_Settings_checkBox_fiat");
            checkBox_AlwaysOnTop.Text = International.GetText("Form_Settings_checkBox_AlwaysOnTop");
            label_psu.Text = International.GetText("Form_Settings_label_psu");
            label_MBpower.Text = International.GetText("Form_Settings_label_MBpower");
            labelAddAMD.Text = International.GetText("Form_Settings_label_AddAMD");
            checkBox_Disable_extra_launch_parameter_checking.Text = International.GetText("Form_Settings_checkBox_Disable_extra_launch_parameter_checking");
            checkBox_DisableDetectionCPU.Text = International.GetText("Form_Settings_checkBox_DisableDetectionCPU");
            label_AutoStartMiningDelay.Text = International.GetText("Form_Settings_label_AutoStartMiningDelay");
            groupBoxMOPA.Text = International.GetText("Form_Settings_groupBoxMOPA");
            radioButtonMOPA1.Text = International.GetText("Form_Settings_radioButtonMOPA1");
            radioButtonMOPA2.Text = International.GetText("Form_Settings_radioButtonMOPA2");
            radioButtonMOPA3.Text = International.GetText("Form_Settings_radioButtonMOPA3");
            radioButtonMOPA4.Text = International.GetText("Form_Settings_radioButtonMOPA4");
            radioButtonMOPA5.Text = International.GetText("Form_Settings_radioButtonMOPA5");
            groupBox1.Text = International.GetText("Form_Settings_groupBox1");

            label_switching_algorithms.Text = International.GetText("Form_Settings_label_switching_algorithms");
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms1"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms3"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms5"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms10"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms15"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms0"));

            label_devices_count.Text = International.GetText("Form_Settings_label_devices_count");
            tabPageAbout.Text = International.GetText("Form_Settings_tabPageAbout");
            groupBoxInfo.Text = International.GetText("Form_Settings_groupBoxInfo");
            groupBoxUpdates.Text = International.GetText("Form_Settings_groupBoxUpdates");
            groupBoxBackup.Text = International.GetText("Form_Settings_groupBoxBackup");
            buttonLicence.Text = International.GetText("Form_Settings_buttonLicence");
            //mem leak here

            richTextBoxInfo.ReadOnly = true;
            richTextBoxInfo.SelectionFont = new Font(richTextBoxInfo.Font, FontStyle.Bold);
            richTextBoxInfo.AppendText("Miner Legacy Fork Fix");
            richTextBoxInfo.SelectionFont = new Font(richTextBoxInfo.Font, FontStyle.Regular);
            richTextBoxInfo.AppendText(International.GetText("Form_Settings_richTextBoxInfo"));
            
            buttonCheckNewVersion.Text = International.GetText("Form_Settings_Checknow");
            buttonUpdate.Text = International.GetText("Form_Settings_Updatenow");

            comboBox_devices_count.Items.Add("6");
            comboBox_devices_count.Items.Add("7");
            comboBox_devices_count.Items.Add("8");
            comboBox_devices_count.Items.Add("9");
            comboBox_devices_count.Items.Add("10");
            comboBox_devices_count.Items.Add("11");
            comboBox_devices_count.Items.Add("12");

            comboBoxCheckforprogramupdatesevery.Items.Add(International.GetText("Form_Settings_comboBoxCheckforprogramupdatesevery1"));
            comboBoxCheckforprogramupdatesevery.Items.Add(International.GetText("Form_Settings_comboBoxCheckforprogramupdatesevery3"));
            comboBoxCheckforprogramupdatesevery.Items.Add(International.GetText("Form_Settings_comboBoxCheckforprogramupdatesevery6"));
            comboBoxCheckforprogramupdatesevery.Items.Add(International.GetText("Form_Settings_comboBoxCheckforprogramupdatesevery12"));
            comboBoxCheckforprogramupdatesevery.Items.Add(International.GetText("Form_Settings_comboBoxCheckforprogramupdatesevery24"));

            labelRestartProgram.Text = International.GetText("Form_Settings_checkBox_RestartProgram");
            comboBoxRestartProgram.Items.Add(International.GetText("Form_Settings_comboBoxRestartProgram0"));
            comboBoxRestartProgram.Items.Add(International.GetText("Form_Settings_comboBoxRestartProgram1"));
            comboBoxRestartProgram.Items.Add(International.GetText("Form_Settings_comboBoxRestartProgram2"));
            comboBoxRestartProgram.Items.Add(International.GetText("Form_Settings_comboBoxRestartProgram3"));
            comboBoxRestartProgram.Items.Add(International.GetText("Form_Settings_comboBoxRestartProgram4"));

            checkBox_RunEthlargement.Enabled = Form_Main.ShouldRunEthlargement;
            checkBox_RunEthlargement.Visible = Form_Main.ShouldRunEthlargement;

            radioButtonMOPA1.Checked = ConfigManager.GeneralConfig.MOPA1;
            radioButtonMOPA2.Checked = ConfigManager.GeneralConfig.MOPA2;
            radioButtonMOPA3.Checked = ConfigManager.GeneralConfig.MOPA3;
            radioButtonMOPA4.Checked = ConfigManager.GeneralConfig.MOPA4;
            radioButtonMOPA5.Checked = ConfigManager.GeneralConfig.MOPA5;

            label_MinIdleSeconds.Text = International.GetText("Form_Settings_General_MinIdleSeconds") + ":";
            //label_MinerRestartDelayMS.Text = International.GetText("Form_Settings_General_MinerRestartDelayMS") + ":";
            label_LogMaxFileSize.Text = International.GetText("Form_Settings_General_LogMaxFileSize") + ":";
            //label_SwitchMaxSeconds.Text =
              //  International.GetText("Form_Settings_General_SwitchMaxSeconds") + ":";
            //label_SwitchMinSeconds.Text = International.GetText("Form_Settings_General_SwitchMinSeconds") + ":";
            //label_APIBindPortStart.Text = International.GetText("Form_Settings_APIBindPortStart") + ":";
            label_MinProfit.Text = International.GetText("Form_Settings_General_MinimumProfit") + ":";
            label_displayCurrency.Text = International.GetText("Form_Settings_DisplayCurrency");
            label_ElectricityCost.Text = International.GetText("Form_Settings_ElectricityCost");

            // device enabled listview translation
            devicesListViewEnableControl1.InitLocale();
            devicesListViewEnableControl2.InitLocale();
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
            algorithmsListViewOverClock1.InitLocale();

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
                progressBarUpdate.BackColor = Form_Main._backColor;
                progressBarUpdate.ForeColor = Form_Main._foreColor;
                progressBarUpdate.ProgressColor = Form_Main._backColor;

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
                linkLabelNewVersion2.MouseLeave += (s, e) => linkLabelNewVersion2.LinkBehavior = LinkBehavior.NeverUnderline;
                linkLabelNewVersion2.MouseEnter += (s, e) => linkLabelNewVersion2.LinkBehavior = LinkBehavior.AlwaysUnderline;

                linkLabelNewVersion2.BackColor = Form_Main._backColor;
                linkLabelNewVersion2.ForeColor = Form_Main._textColor;
                linkLabelNewVersion2.LinkColor = Form_Main._textColor;

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
                comboBoxCheckforprogramupdatesevery.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                comboBoxRestartProgram.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;

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
                foreach (var lbl in this.tabPageAbout.Controls.OfType<CheckBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }
                checkBox_BackupBeforeUpdate.BackColor = Form_Main._backColor;
                checkBox_BackupBeforeUpdate.ForeColor = Form_Main._textColor;
                checkBoxAutoupdate.BackColor = Form_Main._backColor;
                checkBoxAutoupdate.ForeColor = Form_Main._textColor;

                checkBox_Force_mining_if_nonprofitable.BackColor = Form_Main._backColor;
                checkBox_Force_mining_if_nonprofitable.ForeColor = Form_Main._textColor;

                checkbox_current_actual_profitabilities.BackColor = Form_Main._backColor;
                checkbox_current_actual_profitabilities.ForeColor = Form_Main._textColor;

                checkBox_Show_profit_with_power_consumption.BackColor = Form_Main._backColor;
                checkBox_Show_profit_with_power_consumption.ForeColor = Form_Main._textColor;

                checkBox_Allow_remote_management.BackColor = Form_Main._backColor;
                checkBox_Allow_remote_management.ForeColor = Form_Main._textColor;

                checkBox_Send_actual_version_info.BackColor = Form_Main._backColor;
                checkBox_Send_actual_version_info.ForeColor = Form_Main._textColor;

                checkBox_Additional_info_about_device.BackColor = Form_Main._backColor;
                checkBox_Additional_info_about_device.ForeColor = Form_Main._textColor;

                checkBox_show_NVdevice_manufacturer.BackColor = Form_Main._backColor;
                checkBox_show_NVdevice_manufacturer.ForeColor = Form_Main._textColor; 
                checkBox_show_AMDdevice_manufacturer.BackColor = Form_Main._backColor;
                checkBox_show_AMDdevice_manufacturer.ForeColor = Form_Main._textColor;

                checkBox_ShowDeviceMemSize.BackColor = Form_Main._backColor;
                checkBox_ShowDeviceMemSize.ForeColor = Form_Main._textColor;
                /*
                checkBox_ShowDeviceBusId.BackColor = Form_Main._backColor;
                checkBox_ShowDeviceBusId.ForeColor = Form_Main._textColor;
                */
                checkbox_Use_OpenHardwareMonitor.BackColor = Form_Main._backColor;
                checkbox_Use_OpenHardwareMonitor.ForeColor = Form_Main._textColor;

                Checkbox_Save_windows_size_and_position.BackColor = Form_Main._backColor;
                Checkbox_Save_windows_size_and_position.ForeColor = Form_Main._textColor;

                checkBox_sorting_list_of_algorithms.BackColor = Form_Main._backColor;
                checkBox_sorting_list_of_algorithms.ForeColor = Form_Main._textColor;

                checkBox_ShowUptime.BackColor = Form_Main._backColor;
                checkBox_ShowUptime.ForeColor = Form_Main._textColor;

                checkBox_program_monitoring.BackColor = Form_Main._backColor;
                checkBox_program_monitoring.ForeColor = Form_Main._textColor;

                checkBox_ShowFanAsPercent.BackColor = Form_Main._backColor;
                checkBox_ShowFanAsPercent.ForeColor = Form_Main._textColor;

                checkbox_Group_same_devices.BackColor = Form_Main._backColor;
                checkbox_Group_same_devices.ForeColor = Form_Main._textColor;

                checkBox_By_profitability_of_all_devices.BackColor = Form_Main._backColor;
                checkBox_By_profitability_of_all_devices.ForeColor = Form_Main._textColor;

                checkBox_Disable_extra_launch_parameter_checking.BackColor = Form_Main._backColor;
                checkBox_Disable_extra_launch_parameter_checking.ForeColor = Form_Main._textColor;

                checkBox_RunEthlargement.BackColor = Form_Main._backColor;
                checkBox_RunEthlargement.ForeColor = Form_Main._textColor;

                checkBox_ABEnableOverclock.BackColor = Form_Main._backColor;
                checkBox_ABEnableOverclock.ForeColor = Form_Main._textColor;

                checkBox_ABOverclock_Relative.BackColor = Form_Main._backColor;
                checkBox_ABOverclock_Relative.ForeColor = Form_Main._textColor;

                checkBox_AB_ForceRun.BackColor = Form_Main._backColor;
                checkBox_AB_ForceRun.ForeColor = Form_Main._textColor;

                checkBox_ABMinimize.BackColor = Form_Main._backColor;
                checkBox_ABMinimize.ForeColor = Form_Main._textColor;

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

                textBoxAddAMD.BackColor = Form_Main._backColor;
                textBoxAddAMD.ForeColor = Form_Main._foreColor;
                textBoxAddAMD.BorderStyle = BorderStyle.FixedSingle;

                textBox_LogMaxFileSize.BackColor = Form_Main._backColor;
                textBox_LogMaxFileSize.ForeColor = Form_Main._foreColor;
                textBox_LogMaxFileSize.BorderStyle = BorderStyle.FixedSingle;

                textBox_MinIdleSeconds.BackColor = Form_Main._backColor;
                textBox_MinIdleSeconds.ForeColor = Form_Main._foreColor;
                textBox_MinIdleSeconds.BorderStyle = BorderStyle.FixedSingle;

                textBox_MinProfit.BackColor = Form_Main._backColor;
                textBox_MinProfit.ForeColor = Form_Main._foreColor;
                textBox_MinProfit.BorderStyle = BorderStyle.FixedSingle;

                textBox_SwitchProfitabilityThreshold.BackColor = Form_Main._backColor;
                textBox_SwitchProfitabilityThreshold.ForeColor = Form_Main._foreColor;
                textBox_SwitchProfitabilityThreshold.BorderStyle = BorderStyle.FixedSingle;

                textBoxMaxEpoch.BackColor = Form_Main._backColor;
                textBoxMaxEpoch.ForeColor = Form_Main._foreColor;
                textBoxMaxEpoch.BorderStyle = BorderStyle.FixedSingle;

                labelRestartProgram.BackColor = Form_Main._backColor;
                labelRestartProgram.ForeColor = Form_Main._foreColor;

                pictureBox_AllowMultipleInstances.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_AutoScaleBTCValues.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_AutoStartMining.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_DisableDetectionAMD.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_DisableDetectionNVIDIA.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_displayCurrency.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_ElectricityCost.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_HideMiningWindows.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_Language.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_LogMaxFileSize.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinIdleSeconds.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinimizeMiningWindows.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinimizeToTray.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_MinProfit.Image = NiceHashMiner.Properties.Resources.info_white_18;
                //pictureBox_NVIDIAP0State.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_RunAtStartup.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_StartMiningWhenIdle.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_SwitchProfitabilityThreshold.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox_TimeUnit.Image = NiceHashMiner.Properties.Resources.info_white_18;
                pictureBox1.Image = NiceHashMiner.Properties.Resources.info_white_18;

                devicesListViewEnableControl1.BackColor = Form_Main._backColor;
                devicesListViewEnableControl1.ForeColor = Form_Main._foreColor;
                algorithmsListView1.BackColor = Form_Main._backColor;
                algorithmsListView1.ForeColor = Form_Main._foreColor;
                devicesListViewEnableControl2.BackColor = Form_Main._backColor;
                devicesListViewEnableControl2.ForeColor = Form_Main._foreColor;
                algorithmsListViewOverClock1.BackColor = Form_Main._backColor;
                algorithmsListViewOverClock1.ForeColor = Form_Main._foreColor;
                tabPageGeneral.BackColor = Form_Main._backColor;
                tabPageGeneral.ForeColor = Form_Main._foreColor;
            } else
            {
                devicesListViewEnableControl1.BackColor = SystemColors.ControlLightLight;
                devicesListViewEnableControl1.ForeColor = Form_Main._foreColor;
                algorithmsListView1.BackColor = SystemColors.ControlLightLight;
                algorithmsListView1.ForeColor = Form_Main._foreColor;
                devicesListViewEnableControl2.BackColor = SystemColors.ControlLightLight;
                devicesListViewEnableControl2.ForeColor = Form_Main._foreColor;
                algorithmsListViewOverClock1.BackColor = SystemColors.ControlLightLight;
                algorithmsListViewOverClock1.ForeColor = Form_Main._foreColor;
            }

            tabControlGeneral.TabPages[0].Text = International.GetText("FormSettings_Tab_General");
            tabControlGeneral.TabPages[1].Text = International.GetText("FormSettings_Tab_Advanced");
            tabControlGeneral.TabPages[2].Text = International.GetText("FormSettings_Tab_Devices_Algorithms");
            groupBox_Main.Text = International.GetText("FormSettings_Tab_General_Group_Main");
            groupBox_Localization.Text = International.GetText("FormSettings_Tab_General_Group_Localization");
            groupBox_Logging.Text = International.GetText("FormSettings_Tab_General_Group_Logging");
            groupBox_Misc.Text = International.GetText("FormSettings_Tab_General_Group_Misc");
            // advanced
            groupBox_Miners.Text = International.GetText("FormSettings_Tab_Advanced_Group_Miners");
            buttonGPUtuning.Text = International.GetText("Form_Settings_GPUTuning");

            label_SwitchProfitabilityThreshold.Text =
                International.GetText("Form_Settings_General_SwitchProfitabilityThreshold");
            textBoxMaxEpoch.Visible = false;
            labelMaxEpoch.Visible = false;
        }

        private void InitializeGeneralTabCallbacks()
        {
            // Add EventHandler for all the general tab's checkboxes
            {
                checkBox_AutoScaleBTCValues.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionCPU.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionAMD.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionNVIDIA.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxCPUmonitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxNVMonitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxAMDmonitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_MinimizeToTray.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_HideMiningWindows.CheckedChanged +=GeneralCheckBoxes_CheckedChanged;
                checkBox_AlwaysOnTop.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_StartMiningWhenIdle.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                //checkBox_NVIDIAP0State.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_LogToFile.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_AutoStartMining.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_AllowMultipleInstances.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_MinimizeMiningWindows.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxRestartDriver.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxDriverWarning.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxRestartWindows.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Allow_remote_management.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Send_actual_version_info.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Force_mining_if_nonprofitable.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkbox_current_actual_profitabilities.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Show_profit_with_power_consumption.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Additional_info_about_device.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_show_NVdevice_manufacturer.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_show_AMDdevice_manufacturer.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ShowDeviceMemSize.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                //checkBox_ShowDeviceBusId.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkbox_Use_OpenHardwareMonitor.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                Checkbox_Save_windows_size_and_position.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_sorting_list_of_algorithms.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ShowUptime.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_program_monitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ShowFanAsPercent.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_fiat.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkbox_Group_same_devices.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_By_profitability_of_all_devices.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxAutoupdate.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_BackupBeforeUpdate.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Disable_extra_launch_parameter_checking.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_RunEthlargement.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ABEnableOverclock.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ABOverclock_Relative.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_AB_ForceRun.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ABMinimize.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
            }
            // Add EventHandler for all the general tab's textboxes
            {
                textBox_AutoStartMiningDelay.Leave += GeneralTextBoxes_Leave;
                textBox_MinIdleSeconds.Leave += GeneralTextBoxes_Leave;
                textBox_LogMaxFileSize.Leave += GeneralTextBoxes_Leave;
                textBox_MinProfit.Leave += GeneralTextBoxes_Leave;
                textBox_ElectricityCost.Leave += GeneralTextBoxes_Leave;
                textBox_psu.Leave += GeneralTextBoxes_Leave;
                textBox_mb.Leave += GeneralTextBoxes_Leave;
                textBoxAddAMD.Leave += GeneralTextBoxes_Leave;
                textBox_SwitchProfitabilityThreshold.Leave += GeneralTextBoxes_Leave;
                textBoxMaxEpoch.Leave += GeneralTextBoxes_Leave;
                // set int only keypress
                textBox_MinIdleSeconds.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                // set double only keypress
                textBox_MinProfit.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBox_ElectricityCost.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBox_psu.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBox_mb.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBoxAddAMD.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
            }
            // Add EventHandler for all the general tab's textboxes
            {
                comboBox_Language.Leave += GeneralComboBoxes_Leave;
                comboBox_TimeUnit.Leave += GeneralComboBoxes_Leave;
                comboBox_ColorProfile.Leave += GeneralComboBoxes_Leave;
                comboBox_switching_algorithms.Leave += GeneralComboBoxes_Leave;
                comboBox_devices_count.Leave += GeneralComboBoxes_Leave;
                comboBoxCheckforprogramupdatesevery.Leave += GeneralComboBoxes_Leave;
                comboBoxRestartProgram.Leave += GeneralComboBoxes_Leave;
            }
        }

        private void InitializeGeneralTabFieldValuesReferences()
        {
            // Checkboxes set checked value
            {
                if (checkBox_AutoStartMining.Checked)
                {
                    textBox_AutoStartMiningDelay.Enabled = true;
                    label_AutoStartMiningDelay.Enabled = true;
                }
                else
                {
                    textBox_AutoStartMiningDelay.Enabled = false;
                    label_AutoStartMiningDelay.Enabled = false;
                }
                checkBox_AutoStartMining.Checked = ConfigManager.GeneralConfig.AutoStartMining;
                checkBox_HideMiningWindows.Checked = ConfigManager.GeneralConfig.HideMiningWindows;
                checkBox_MinimizeToTray.Checked = ConfigManager.GeneralConfig.MinimizeToTray;
                checkBox_AlwaysOnTop.Checked = ConfigManager.GeneralConfig.AlwaysOnTop;
                checkBox_DisableDetectionNVIDIA.Checked = ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionNVIDIA;
                checkBox_DisableDetectionCPU.Checked = ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionCPU;
                checkBox_DisableDetectionAMD.Checked = ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionAMD;
                checkBoxCPUmonitoring.Checked = ConfigManager.GeneralConfig.DisableMonitoringCPU;
                checkBoxNVMonitoring.Checked = ConfigManager.GeneralConfig.DisableMonitoringNVIDIA;
                checkBoxAMDmonitoring.Checked = ConfigManager.GeneralConfig.DisableMonitoringAMD;
                checkBox_AutoScaleBTCValues.Checked = ConfigManager.GeneralConfig.AutoScaleBTCValues;
                checkBox_StartMiningWhenIdle.Checked = ConfigManager.GeneralConfig.StartMiningWhenIdle;
                //checkBox_NVIDIAP0State.Checked = ConfigManager.GeneralConfig.NVIDIAP0State;
                checkBox_LogToFile.Checked = ConfigManager.GeneralConfig.LogToFile;
                if (checkBox_LogToFile.Checked)
                {
                    textBox_LogMaxFileSize.Enabled = true;
                }
                else
                {
                    textBox_LogMaxFileSize.Enabled = false;
                }

                checkBox_AllowMultipleInstances.Checked = ConfigManager.GeneralConfig.AllowMultipleInstances;
                checkBox_RunAtStartup.Checked = IsInStartupRegistry();
                checkBox_MinimizeMiningWindows.Checked = ConfigManager.GeneralConfig.MinimizeMiningWindows;
                checkBox_MinimizeMiningWindows.Enabled = !ConfigManager.GeneralConfig.HideMiningWindows;
                checkBoxRestartDriver.Checked = ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost;
                checkBoxDriverWarning.Checked = ConfigManager.GeneralConfig.ShowDriverVersionWarning;
                checkBoxRestartWindows.Checked = ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost;
                checkBox_Allow_remote_management.Checked = ConfigManager.GeneralConfig.Allow_remote_management;
                checkBox_Send_actual_version_info.Checked = ConfigManager.GeneralConfig.Send_actual_version_info;
                checkBox_Force_mining_if_nonprofitable.Checked = ConfigManager.GeneralConfig.Force_mining_if_nonprofitable;
                checkbox_current_actual_profitabilities.Checked = ConfigManager.GeneralConfig.Show_current_actual_profitability;
                checkBox_Show_profit_with_power_consumption.Checked = ConfigManager.GeneralConfig.DecreasePowerCost;
                checkBox_fiat.Checked = ConfigManager.GeneralConfig.FiatCurrency;
                checkBox_Additional_info_about_device.Checked = ConfigManager.GeneralConfig.Additional_info_about_device;
                checkBox_show_NVdevice_manufacturer.Checked = ConfigManager.GeneralConfig.Show_NVdevice_manufacturer;
                checkBox_show_AMDdevice_manufacturer.Checked = ConfigManager.GeneralConfig.Show_AMDdevice_manufacturer;
                checkBox_ShowDeviceMemSize.Checked = ConfigManager.GeneralConfig.Show_ShowDeviceMemSize;
                //checkBox_ShowDeviceBusId.Checked = ConfigManager.GeneralConfig.Show_ShowDeviceBusId;
                checkbox_Use_OpenHardwareMonitor.Checked = ConfigManager.GeneralConfig.Use_OpenHardwareMonitor;
                Checkbox_Save_windows_size_and_position.Checked = ConfigManager.GeneralConfig.Save_windows_size_and_position;
                checkBox_ShowUptime.Checked = ConfigManager.GeneralConfig.ShowUptime;
                checkBox_program_monitoring.Checked = ConfigManager.GeneralConfig.ProgramMonitoring;
                checkBox_sorting_list_of_algorithms.Checked = ConfigManager.GeneralConfig.ColumnSort;
                checkBox_ShowFanAsPercent.Checked = ConfigManager.GeneralConfig.ShowFanAsPercent;
                checkbox_Group_same_devices.Checked = ConfigManager.GeneralConfig.Group_same_devices;
                checkBox_By_profitability_of_all_devices.Checked = ConfigManager.GeneralConfig.By_profitability_of_all_devices;
                checkBoxAutoupdate.Checked = ConfigManager.GeneralConfig.ProgramAutoUpdate;
                checkBox_BackupBeforeUpdate.Checked = ConfigManager.GeneralConfig.BackupBeforeUpdate;
                checkBox_Disable_extra_launch_parameter_checking.Checked = ConfigManager.GeneralConfig.Disable_extra_launch_parameter_checking;
                checkBox_RunEthlargement.Checked = ConfigManager.GeneralConfig.UseEthlargement;
                checkBox_AB_ForceRun.Checked = ConfigManager.GeneralConfig.AB_ForceRun;
                checkBox_ABEnableOverclock.Checked = ConfigManager.GeneralConfig.ABEnableOverclock;
                checkBox_ABOverclock_Relative.Checked = ConfigManager.GeneralConfig.ABOverclock_Relative;
                checkBox_ABMinimize.Checked = ConfigManager.GeneralConfig.ABMinimize;
            }

            // Textboxes
            {
                textBox_MinIdleSeconds.Text = ConfigManager.GeneralConfig.MinIdleSeconds.ToString();
                textBox_LogMaxFileSize.Text = ConfigManager.GeneralConfig.LogMaxFileSize.ToString();
                textBox_AutoStartMiningDelay.Text = ConfigManager.GeneralConfig.AutoStartMiningDelay.ToString();
                textBox_SwitchProfitabilityThreshold.Text = ((ConfigManager.GeneralConfig.SwitchProfitabilityThreshold) * 100)
                    .ToString("F1").Replace(',', '.'); // force comma;
                textBox_ElectricityCost.Text = ConfigManager.GeneralConfig.KwhPrice.ToString("0.0000");
                textBox_psu.Text = ConfigManager.GeneralConfig.PowerPSU.ToString();
                textBox_mb.Text = ConfigManager.GeneralConfig.PowerMB.ToString();
                textBoxAddAMD.Text = ConfigManager.GeneralConfig.PowerAddAMD.ToString();
            }

            // set custom control referances
            {
                // here we want all devices
                devicesListViewEnableControl1.SetComputeDevices(ComputeDeviceManager.Available.Devices);
                devicesListViewEnableControl1.SetAlgorithmsListView(algorithmsListView1);
                devicesListViewEnableControl1.IsSettingsCopyEnabled = true;

                devicesListViewEnableControl2.SetComputeDevices(ComputeDeviceManager.Available.Devices, false);
                devicesListViewEnableControl2.SetAlgorithmsListViewOverClock(algorithmsListViewOverClock1);
                devicesListViewEnableControl2.IsSettingsCopyEnabled = true;
            }

            // Add language selections list
            {
                var lang = International.GetAvailableLanguages();

                comboBox_Language.Items.Clear();
                for (var i = 0; i < lang.Count; i++)
                {
                    comboBox_Language.Items.Add(lang[(LanguageType)i]);
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
                comboBox_Language.SelectedIndex = (int)ConfigManager.GeneralConfig.Language;
                //comboBox_ServiceLocation.SelectedIndex = ConfigManager.GeneralConfig.ServiceLocation;
                comboBox_TimeUnit.SelectedItem = International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
                currencyConverterCombobox.SelectedItem = ConfigManager.GeneralConfig.DisplayCurrency;
                comboBox_ColorProfile.SelectedIndex = ConfigManager.GeneralConfig.ColorProfileIndex;
                comboBox_switching_algorithms.SelectedIndex = ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex;
                comboBox_devices_count.SelectedIndex = ConfigManager.GeneralConfig.DevicesCountIndex;
                comboBoxCheckforprogramupdatesevery.SelectedIndex = ConfigManager.GeneralConfig.ProgramUpdateIndex;
                comboBoxRestartProgram.SelectedIndex = ConfigManager.GeneralConfig.ProgramRestartIndex;
            }

            checkBox_AB_ForceRun.Enabled = checkBox_ABEnableOverclock.Checked;
            checkBox_ABMinimize.Enabled = checkBox_ABEnableOverclock.Checked;
            devicesListViewEnableControl2.Enabled = checkBox_ABEnableOverclock.Checked;
            algorithmsListViewOverClock1.Enabled = checkBox_ABEnableOverclock.Checked;

            //if (!ConfigManager.GeneralConfig.ShowToolsFolder)
            {
                var tp = tabPageTools;
                tabControlGeneral.TabPages.Remove(tp);
                //var oc = tabPageOverClock;
                //tabControlGeneral.TabPages.Remove(oc);
            }
      

        }

        private void InitializeGeneralTab()
        {
            InitializeGeneralTabTranslations();//<- mem leak
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
            devicesListViewEnableControl2.SetDeviceSelectionChangedCallback(DevicesListView2_ItemSelectionChanged);
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
            ConfigManager.GeneralConfig.AutoStartMining = checkBox_AutoStartMining.Checked;
            textBox_AutoStartMiningDelay.Enabled = checkBox_AutoStartMining.Checked;
            ConfigManager.GeneralConfig.HideMiningWindows = checkBox_HideMiningWindows.Checked;
            ConfigManager.GeneralConfig.MinimizeToTray = checkBox_MinimizeToTray.Checked;
            ConfigManager.GeneralConfig.AlwaysOnTop = checkBox_AlwaysOnTop.Checked;
            ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionNVIDIA =
                checkBox_DisableDetectionNVIDIA.Checked;
            ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionAMD = checkBox_DisableDetectionAMD.Checked;
            ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionCPU = checkBox_DisableDetectionCPU.Checked;
            ConfigManager.GeneralConfig.DisableMonitoringAMD = checkBoxAMDmonitoring.Checked;
            ConfigManager.GeneralConfig.DisableMonitoringCPU = checkBoxCPUmonitoring.Checked;
            ConfigManager.GeneralConfig.DisableMonitoringNVIDIA = checkBoxNVMonitoring.Checked;
            ConfigManager.GeneralConfig.AutoScaleBTCValues = checkBox_AutoScaleBTCValues.Checked;
            ConfigManager.GeneralConfig.StartMiningWhenIdle = checkBox_StartMiningWhenIdle.Checked;
            //ConfigManager.GeneralConfig.NVIDIAP0State = checkBox_NVIDIAP0State.Checked;
            ConfigManager.GeneralConfig.LogToFile = checkBox_LogToFile.Checked;
            ConfigManager.GeneralConfig.AllowMultipleInstances = checkBox_AllowMultipleInstances.Checked;
            ConfigManager.GeneralConfig.MinimizeMiningWindows = checkBox_MinimizeMiningWindows.Checked;
            ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost = checkBoxRestartDriver.Checked;
            ConfigManager.GeneralConfig.ShowDriverVersionWarning = checkBoxDriverWarning.Checked;
            ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost = checkBoxRestartWindows.Checked;
            ConfigManager.GeneralConfig.Allow_remote_management = checkBox_Allow_remote_management.Checked;
            ConfigManager.GeneralConfig.Send_actual_version_info = checkBox_Send_actual_version_info.Checked;
            ConfigManager.GeneralConfig.Force_mining_if_nonprofitable = checkBox_Force_mining_if_nonprofitable.Checked;
            ConfigManager.GeneralConfig.Show_current_actual_profitability = checkbox_current_actual_profitabilities.Checked;
            ConfigManager.GeneralConfig.DecreasePowerCost = checkBox_Show_profit_with_power_consumption.Checked;
            ConfigManager.GeneralConfig.FiatCurrency = checkBox_fiat.Checked;
            ConfigManager.GeneralConfig.Additional_info_about_device = checkBox_Additional_info_about_device.Checked;
            ConfigManager.GeneralConfig.Show_NVdevice_manufacturer = checkBox_show_NVdevice_manufacturer.Checked;
            ConfigManager.GeneralConfig.Show_AMDdevice_manufacturer = checkBox_show_AMDdevice_manufacturer.Checked;
            ConfigManager.GeneralConfig.Show_ShowDeviceMemSize = checkBox_ShowDeviceMemSize.Checked;
            //ConfigManager.GeneralConfig.Show_ShowDeviceBusId = checkBox_ShowDeviceBusId.Checked;
            ConfigManager.GeneralConfig.Use_OpenHardwareMonitor = checkbox_Use_OpenHardwareMonitor.Checked;
            ConfigManager.GeneralConfig.Save_windows_size_and_position = Checkbox_Save_windows_size_and_position.Checked;
            ConfigManager.GeneralConfig.ColumnSort = checkBox_sorting_list_of_algorithms.Checked;
            ConfigManager.GeneralConfig.ShowUptime = checkBox_ShowUptime.Checked;
            ConfigManager.GeneralConfig.ProgramMonitoring = checkBox_program_monitoring.Checked;
            ConfigManager.GeneralConfig.ShowFanAsPercent = checkBox_ShowFanAsPercent.Checked;
            ConfigManager.GeneralConfig.Group_same_devices = checkbox_Group_same_devices.Checked;
            ConfigManager.GeneralConfig.By_profitability_of_all_devices = checkBox_By_profitability_of_all_devices.Checked;
            ConfigManager.GeneralConfig.ProgramAutoUpdate = checkBoxAutoupdate.Checked;
            ConfigManager.GeneralConfig.BackupBeforeUpdate = checkBox_BackupBeforeUpdate.Checked;
            ConfigManager.GeneralConfig.Disable_extra_launch_parameter_checking = checkBox_Disable_extra_launch_parameter_checking.Checked;
            ConfigManager.GeneralConfig.UseEthlargement = checkBox_RunEthlargement.Checked;
            ConfigManager.GeneralConfig.ABEnableOverclock = checkBox_ABEnableOverclock.Checked;
            ConfigManager.GeneralConfig.ABOverclock_Relative = checkBox_ABOverclock_Relative.Checked;
            ConfigManager.GeneralConfig.AB_ForceRun = checkBox_AB_ForceRun.Checked;
            ConfigManager.GeneralConfig.ABMinimize = checkBox_ABMinimize.Checked;
            if (checkBox_LogToFile.Checked)
            {
                textBox_LogMaxFileSize.Enabled = true;
            } else
            {
                textBox_LogMaxFileSize.Enabled = false;
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
            ConfigManager.GeneralConfig.MinIdleSeconds = Helpers.ParseInt(textBox_MinIdleSeconds.Text);
            ConfigManager.GeneralConfig.LogMaxFileSize = Helpers.ParseLong(textBox_LogMaxFileSize.Text);
            ConfigManager.GeneralConfig.AutoStartMiningDelay = Helpers.ParseInt(textBox_AutoStartMiningDelay.Text);
            // min profit
            ConfigManager.GeneralConfig.MinimumProfit = Helpers.ParseDouble(textBox_MinProfit.Text);
            ConfigManager.GeneralConfig.SwitchProfitabilityThreshold =
                Helpers.ParseDouble(textBox_SwitchProfitabilityThreshold.Text) / 100;

            ConfigManager.GeneralConfig.KwhPrice = Helpers.ParseDouble(textBox_ElectricityCost.Text);
            ConfigManager.GeneralConfig.PowerMB = Helpers.ParseInt(textBox_mb.Text);
            ConfigManager.GeneralConfig.PowerAddAMD = Helpers.ParseInt(textBoxAddAMD.Text);
            ConfigManager.GeneralConfig.PowerPSU = Helpers.ParseInt(textBox_psu.Text);
            if (Form_Main.DaggerHashimoto3GBVisible)
            {
                ConfigManager.GeneralConfig.DaggerHashimoto3GBMaxEpoch = Helpers.ParseInt(textBoxMaxEpoch.Text);
            }
            if (Form_Main.DaggerHashimoto4GBVisible)
            {
                ConfigManager.GeneralConfig.DaggerHashimoto4GBMaxEpoch = Helpers.ParseInt(textBoxMaxEpoch.Text);
            }
            if (Form_Main.DaggerHashimoto1070Visible)
            {
                ConfigManager.GeneralConfig.DaggerHashimoto1070MaxEpoch = Helpers.ParseInt(textBoxMaxEpoch.Text);
            }

            // Fix bounds
            ConfigManager.GeneralConfig.FixSettingBounds();
            // update strings
            textBox_MinProfit.Text =
                ConfigManager.GeneralConfig.MinimumProfit.ToString("F2").Replace(',', '.'); // force comma
            textBox_SwitchProfitabilityThreshold.Text = (ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100)
                .ToString("F1").Replace(',', '.'); // force comma
            textBox_MinIdleSeconds.Text = ConfigManager.GeneralConfig.MinIdleSeconds.ToString();
            textBox_LogMaxFileSize.Text = ConfigManager.GeneralConfig.LogMaxFileSize.ToString();
            textBox_AutoStartMiningDelay.Text = ConfigManager.GeneralConfig.AutoStartMiningDelay.ToString();
            textBox_ElectricityCost.Text = ConfigManager.GeneralConfig.KwhPrice.ToString("0.0000");
            textBox_psu.Text = ConfigManager.GeneralConfig.PowerPSU.ToString("");
            textBox_mb.Text = ConfigManager.GeneralConfig.PowerMB.ToString("");
            textBoxAddAMD.Text = ConfigManager.GeneralConfig.PowerAddAMD.ToString("");
            textBoxMaxEpoch.Text = ConfigManager.GeneralConfig.DaggerHashimoto4GBMaxEpoch.ToString("");
        }

        private void GeneralComboBoxes_Leave(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            IsChange = true;
            ConfigManager.GeneralConfig.Language = (LanguageType) comboBox_Language.SelectedIndex;
            ConfigManager.GeneralConfig.ColorProfileIndex = comboBox_ColorProfile.SelectedIndex;
            ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex = comboBox_switching_algorithms.SelectedIndex;
            ConfigManager.GeneralConfig.DevicesCountIndex = comboBox_devices_count.SelectedIndex;
            ConfigManager.GeneralConfig.ProgramUpdateIndex = comboBoxCheckforprogramupdatesevery.SelectedIndex;
            ConfigManager.GeneralConfig.ProgramRestartIndex = comboBoxRestartProgram.SelectedIndex;
            ConfigManager.GeneralConfig.TimeUnit = (TimeUnitType) comboBox_TimeUnit.SelectedIndex;
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
        private void DevicesListView2_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            algorithmSettingsControl1.Deselect();
            // show algorithms
            _selectedComputeDevice =
                ComputeDeviceManager.Available.GetCurrentlySelectedComputeDevice(e.ItemIndex, ShowUniqueDeviceList);
            //if (_selectedComputeDevice.DeviceType != DeviceType.CPU)
            {
                algorithmsListViewOverClock1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
                groupBoxOverClockSettings.Text = string.Format(International.GetText("FormSettings_OverclockSettings"),
                    _selectedComputeDevice.Name);
            }
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
                ConfigManager.GeneralConfigFileCommit();
                Form_Main.MakeRestart(0);
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
                NiceHashStats.SetCredentials(ConfigManager.GeneralConfig.BitcoinAddressNew.Trim(), ConfigManager.GeneralConfig.WorkerName.Trim());
            }
            if (ConfigManager.GeneralConfig.ABEnableOverclock & MSIAfterburner.Initialized)
            {
                MSIAfterburner.CopyFromTempFiles();
            }
            if (UpdateListView_timer != null)
            {
                UpdateListView_timer.Stop();
                UpdateListView_timer = null;
            }
            if (Form_Settings.ActiveForm != null)
            {
                Form_Settings.ActiveForm.Close();
            }
            new Task(() => NiceHashStats.SetDeviceStatus(null, true)).Start();
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
            richTextBoxInfo.Dispose();
            GC.Collect();
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
        }

        private void CheckBox_HideMiningWindows_CheckChanged(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            IsChange = true;
            ConfigManager.GeneralConfig.HideMiningWindows = checkBox_HideMiningWindows.Checked;
            checkBox_MinimizeMiningWindows.Enabled = !checkBox_HideMiningWindows.Checked;
        }

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
            devicesListViewEnableControl2.InitLocale();
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
            string githubVersion = Updater.Updater.GetVersion().Item1;
            Double.TryParse(githubVersion.ToString(), out Form_Main.githubVersion);
            Form_Main.githubBuild = Updater.Updater.GetVersion().Item2;

            linkLabelNewVersion2.Text = International.GetText("Form_Settings_Nonewversionorbuild");
            if (Form_Main.currentBuild < Form_Main.githubBuild)//testing
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newbuild") + Form_Main.githubBuild.ToString("00000000.00");
                buttonUpdate.Visible = true;
            }

            if (Form_Main.currentVersion < Form_Main.githubVersion)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newversion") + Form_Main.githubVersion.ToString();
                buttonUpdate.Visible = true;
            }
            if (Form_Main.githubVersion <= 0)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Errorwhencheckingnewversion");
                buttonUpdate.Visible = false;
            }
            linkLabelNewVersion2.Update();
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

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            buttonUpdate.Visible = false;
            progressBarUpdate.Visible = true;

            progressBarUpdate.BackColor = Form_Main._backColor;
            progressBarUpdate.TextColor = Form_Main._textColor;
            Updater.Updater.Downloader(false);
        }

        private void buttonCreateBackup_Click(object sender, EventArgs e)
        {
            string fname = Form_Main.currentBuild.ToString("00000000.00");
            try
            {
                var CMDconfigHandleBackup = new Process

                {
                    StartInfo =
                {
                    FileName = "utils\\7z.exe"
                }
                };

                if (Directory.Exists("backup"))
                {
                    var dirInfo = new DirectoryInfo("backup");
                    foreach (var file in dirInfo.GetFiles()) file.Delete();
                    dirInfo.Delete();
                }

                CMDconfigHandleBackup.StartInfo.Arguments = "a -tzip -mx3 -ssw -r -y -x!backup backup\\backup_" + fname + ".zip";
                CMDconfigHandleBackup.StartInfo.UseShellExecute = false;
                CMDconfigHandleBackup.StartInfo.CreateNoWindow = false;
                //CMDconfigHandleBackup.Exited += new EventHandler(CMDconfigHandleBackup_Exited);
                CMDconfigHandleBackup.Start();
                CMDconfigHandleBackup.WaitForExit();
                Helpers.ConsolePrint("BACKUP", "Error code: " + CMDconfigHandleBackup.ExitCode);
                if (CMDconfigHandleBackup.ExitCode != 0)
                {
                    //MessageBox.Show("Error code: " + CMDconfigHandleBackup.ExitCode,
                    //"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("Backup", ex.ToString());
                //MessageBox.Show("Unknown error ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.Exists("backup"))
            {
                var dirInfo = new DirectoryInfo("backup");
                foreach (var file in dirInfo.GetFiles())
                {
                    if (file.Name.Contains("backup_") && file.Name.Contains(".zip"))
                    {
                        Form_Main.BackupFileName = file.Name.Replace("backup_", "").Replace(".zip", "");
                        Form_Main.BackupFileDate = file.CreationTime.ToString("dd.MM.yyyy HH:mm");
                        labelBackupCopy.Text = International.GetText("Form_Settings_Backupcopy") + Form_Main.BackupFileName +
                            " (" + Form_Main.BackupFileDate + ")";
                    }
                }
                Form_Benchmark.RunCMDAfterBenchmark();
                try
                {
                    var cmdFile = "@echo off\r\n" +
                        "taskkill /F /IM \"MinerLegacyForkFixMonitor.exe\"\r\n" +
                        "taskkill /F /IM \"NiceHashMinerLegacy.exe\"\r\n" +
                        //"call AfterBenchmark.cmd\"\r\n" +
                        "timeout /T 2 /NOBREAK\r\n" +
                        "utils\\7z.exe x -r -y " + "backup\\backup_" + fname + ".zip" + "\r\n" +
                        "start NiceHashMinerLegacy.exe\r\n";
                    FileStream fs = new FileStream("backup\\restore.cmd", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.WriteAsync(cmdFile);
                    w.Flush();
                    w.Close();
                    buttonRestoreBackup.Enabled = true;
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("Restore", ex.ToString());
                }
            }

        }
        private void CMDconfigHandleBackup_Exited(object sender, System.EventArgs e)
        {
        }

        private void checkBox_AlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void buttonRestoreBackup_Click(object sender, EventArgs e)
        {
            //stop openhardwaremonitor
            if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
            {
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
            }
            MinersManager.StopAllMiners();
            System.Threading.Thread.Sleep(1000);
            Process.Start("backup\\restore.cmd");

        }

        private void comboBoxCheckforprogramupdatesevery_DrawItem(object sender, DrawItemEventArgs e)
        {
            comboBox_ServiceLocation_DrawItem(sender, e);
        }

        private void checkBoxRestartWindows_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRestartDriver.Checked = false;
            checkBoxRestartDriver.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
        }

        private void checkBoxRestartDriver_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRestartWindows.Checked = false;
            checkBoxRestartDriver.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
        }

        private void comboBoxRestartProgram_DrawItem(object sender, DrawItemEventArgs e)
        {
            comboBox_ServiceLocation_DrawItem(sender, e);
        }

        private void label_TimeUnit_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox_MinProfit_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox_MinProfit_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox_fiat_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonCurrPorts_Click(object sender, EventArgs e)
        {
            var cports = new ProcessStartInfo
            {
                FileName = "utils/cports-x64/cports.exe",
                CreateNoWindow = false,
                UseShellExecute = false
            };
            Process.Start(cports);
        }

        private void buttonOverdriveNTool_Click(object sender, EventArgs e)
        {
            var OverdriveNTool = new ProcessStartInfo
            {
                FileName = "utils/OverdriveNTool.exe",
                CreateNoWindow = false,
                UseShellExecute = false
            };
            Process.Start(OverdriveNTool);
        }

        private void buttonNVIDIAinspector_Click(object sender, EventArgs e)
        {
            var nvidiaInspector = new ProcessStartInfo
            {
                FileName = "utils/nvidiaInspector.exe",
                CreateNoWindow = false,
                UseShellExecute = false
            };
            Process.Start(nvidiaInspector);
        }

        private void buttonCheckNiceHashStatus_Click(object sender, EventArgs e)
        {
            Process.Start("https://status.nicehash.com/");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ps = Miner.PingServers();
            if (ps < 0)
            {
                richTextBoxCheckNiceHashservers.Text += "NiceHash speedtest servers down. Try alternate method\n";
                Miner.PingServers("daggerhashimoto");
                for (int i = 0; i < 4; i++)
                {
                    var server = "daggerhashimoto." + Globals.MiningLocation[i] + ".nicehash.com";
                    richTextBoxCheckNiceHashservers.Text += server + " ping: " + Form_Main.myServers[i, 1] + " ms\n";
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    var server = "speedtest." + Globals.MiningLocation[i] + ".nicehash.com";
                    richTextBoxCheckNiceHashservers.Text += server + " ping: " + Form_Main.myServers[i, 1] + " ms\n";
                }
            }

        }

        private void checkBox_program_monitoring_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_AllowMultipleInstances.Checked && checkBox_program_monitoring.Checked)
            {
                MessageBox.Show(International.GetText("Form_Settings_uncompatible_options1"),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checkBox_AllowMultipleInstances.Checked = false;
            }
        }
        private void checkBox_AllowMultipleInstances_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_AllowMultipleInstances.Checked && checkBox_program_monitoring.Checked)
            {
                    MessageBox.Show(International.GetText("Form_Settings_uncompatible_options1"),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checkBox_AllowMultipleInstances.Checked = false;
            }
        }

        private void checkBox_show_device_manufacturer_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_DisableDetectionNVIDIA_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DisableDetectionNVIDIA.Checked)
            {
                checkBoxNVMonitoring.Enabled = false;
                checkBoxRestartWindows.Enabled = false;
                checkBoxRestartDriver.Enabled = false;
                checkBox_show_NVdevice_manufacturer.Enabled = false;
            } else
            {
                checkBoxNVMonitoring.Enabled = true;
                checkBoxRestartWindows.Enabled = true;
                checkBoxRestartDriver.Enabled = true;
                checkBox_show_NVdevice_manufacturer.Enabled = true;
            }
        }

        private void checkBox_DisableDetectionAMD_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DisableDetectionAMD.Checked)
            {
                checkBoxAMDmonitoring.Enabled = false;
                checkBox_show_AMDdevice_manufacturer.Enabled = false;
            }
            else
            {
                checkBoxAMDmonitoring.Enabled = true;
                checkBox_show_AMDdevice_manufacturer.Enabled = true;
            }
        }

        private void checkBox_DisableDetectionCPU_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DisableDetectionCPU.Checked)
            {
                checkBoxCPUmonitoring.Enabled = false;
            }
            else
            {
                checkBoxCPUmonitoring.Enabled = true;
            }
        }

        private void checkBox_AutoStartMining_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox_AutoStartMining.Checked)
            {
                textBox_AutoStartMiningDelay.Enabled = true;
                label_AutoStartMiningDelay.Enabled = true;
            }
            else
            {
                textBox_AutoStartMiningDelay.Enabled = false;
                label_AutoStartMiningDelay.Enabled = false;
            }
        }

        private void pictureBox_SwitchProfitabilityThreshold_Click(object sender, EventArgs e)
        {

        }

        private void Form_Settings_ResizeBegin(object sender, EventArgs e)
        {
            FormSettingsMoved = true;
        }

        private void Form_Settings_ResizeEnd(object sender, EventArgs e)
        {
            FormSettingsMoved = false;
        }

        private void devicesListViewEnableControl2_Load(object sender, EventArgs e)
        {

        }

        private void checkBox_ABEnableOverclock_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_AB_ForceRun.Enabled = checkBox_ABEnableOverclock.Checked;
            checkBox_ABMinimize.Enabled = checkBox_ABEnableOverclock.Checked;
            devicesListViewEnableControl2.Enabled = checkBox_ABEnableOverclock.Checked;
            algorithmsListViewOverClock1.Enabled = checkBox_ABEnableOverclock.Checked;

            var oc = tabPageOverClock;
            //tabControlGeneral.TabPages.Remove(oc);

            if (checkBox_ABEnableOverclock.Checked && oc.Created)
            {
                string str = " ";
                checkBox_ABEnableOverclock.Text = International.GetText("FormSettings_ABEnableOverclock") + str.PadRight(1, '.');
                checkBox_ABEnableOverclock.Update();
                if (!MSIAfterburner.MSIAfterburnerRUN(true))
                {
                    checkBox_ABEnableOverclock.Checked = false;
                    return;
                }
                /*
                for (int i = 2; i <= 10; i++)
                {
                    checkBox_ABEnableOverclock.Text = International.GetText("FormSettings_ABEnableOverclock") + str.PadRight(i, '.');
                    System.Threading.Thread.Sleep(300);
                    checkBox_ABEnableOverclock.Update();
                }
                */
                checkBox_ABEnableOverclock.Text = International.GetText("FormSettings_ABEnableOverclock");
                checkBox_ABEnableOverclock.Update();
                if (!MSIAfterburner.MSIAfterburnerInit())
                {
                    MessageBox.Show(International.GetText("FormSettings_AB_Error"), "MSI Afterburner error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else
                {
                    MSIAfterburner.InitTempFiles();
                }

            }
            if (!checkBox_ABEnableOverclock.Checked)
            {
                MSIAfterburner.Initialized = false;
                if (MSIAfterburner.macm != null) MSIAfterburner.macm.Disconnect();
                if (MSIAfterburner.mahm != null) MSIAfterburner.mahm.Disconnect();
                MSIAfterburner.macm = null;
                MSIAfterburner.mahm = null;
                
            }
            oc.Focus();
        }
    }
}
