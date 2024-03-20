using Microsoft.Win32;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners;
using NiceHashMiner.Stats;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public static bool ForceClosingForm = false;
        //public static bool Zil_GMiner = false;
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
            if (!ConfigManager.GeneralConfig.DisableTooltips)
            {
                InitializeToolTip();
            }

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
            if (ForceClosingForm)
            {
                ButtonSaveClose_Click(null, null);
            }
        }

        #region Initializations

        private void InitializeToolTip()
        {
            // Setup Tooltips
            toolTip1.SetToolTip(comboBox_Language, International.GetText("Form_Settings_ToolTip_Language"));
            toolTip1.SetToolTip(label_Language, International.GetText("Form_Settings_ToolTip_Language"));

            toolTip1.SetToolTip(comboBox_TimeUnit, International.GetText("Form_Settings_ToolTip_TimeUnit"));
            toolTip1.SetToolTip(label_TimeUnit, International.GetText("Form_Settings_ToolTip_TimeUnit"));

            toolTip1.SetToolTip(checkBox_HideMiningWindows,
                International.GetText("Form_Settings_ToolTip_checkBox_HideMiningWindows"));

            toolTip1.SetToolTip(checkBox_MinimizeToTray,
                International.GetText("Form_Settings_ToolTip_checkBox_MinimizeToTray"));

            toolTip1.SetToolTip(checkBox_AllowMultipleInstances,
                International.GetText("Form_Settings_General_AllowMultipleInstances_ToolTip"));

            toolTip1.SetToolTip(label_MinProfit, International.GetText("Form_Settings_ToolTip_MinimumProfit"));
            toolTip1.SetToolTip(textBox_MinProfit, International.GetText("Form_Settings_ToolTip_MinimumProfit"));

            toolTip1.SetToolTip(checkBox_DisableDetectionNVIDIA,
                string.Format(International.GetText("Form_Settings_ToolTip_checkBox_DisableDetection"), "NVIDIA"));
            toolTip1.SetToolTip(checkBox_DisableDetectionAMD,
                string.Format(International.GetText("Form_Settings_ToolTip_checkBox_DisableDetection"), "AMD"));
            toolTip1.SetToolTip(checkBox_DisableDetectionINTEL,
                string.Format(International.GetText("Form_Settings_ToolTip_checkBox_DisableDetection"), "INTEL"));

            toolTip1.SetToolTip(checkBox_AutoScaleBTCValues,
                International.GetText("Form_Settings_ToolTip_checkBox_AutoScaleBTCValues"));

            toolTip1.SetToolTip(checkBox_StartMiningWhenIdle,
                International.GetText("Form_Settings_ToolTip_checkBox_StartMiningWhenIdle"));

            toolTip1.SetToolTip(textBox_MinIdleSeconds, International.GetText("Form_Settings_ToolTip_MinIdleSeconds"));
            toolTip1.SetToolTip(label_MinIdleSeconds, International.GetText("Form_Settings_ToolTip_MinIdleSeconds"));

            toolTip1.SetToolTip(checkBox_LogToFile, International.GetText("Form_Settings_ToolTip_checkBox_LogToFile"));

            toolTip1.SetToolTip(textBox_LogMaxFileSize, International.GetText("Form_Settings_ToolTip_LogMaxFileSize"));
            toolTip1.SetToolTip(label_LogMaxFileSize, International.GetText("Form_Settings_ToolTip_LogMaxFileSize"));

            toolTip1.SetToolTip(checkBox_RunAtStartup,
                International.GetText("Form_Settings_ToolTip_checkBox_RunAtStartup"));

            toolTip1.SetToolTip(checkBox_AutoStartMining,
                International.GetText("Form_Settings_ToolTip_checkBox_AutoStartMining"));

            toolTip1.SetToolTip(label_displayCurrency, International.GetText("Form_Settings_ToolTip_DisplayCurrency"));

            toolTip1.SetToolTip(currencyConverterCombobox,
                International.GetText("Form_Settings_ToolTip_DisplayCurrency"));

            toolTip1.SetToolTip(label_SwitchProfitabilityThreshold,
                International.GetText("Form_Settings_ToolTip_SwitchProfitabilityThreshold"));

            toolTip1.SetToolTip(checkBox_MinimizeMiningWindows,
                International.GetText("Form_Settings_ToolTip_MinimizeMiningWindows"));

            toolTip1.SetToolTip(checkBoxLast24hours,
                International.GetText("Form_Settings_ToolTip_Last24hours"));

            if (Form_Main.ZilFactor == 0.000d)
            {
                toolTip1.SetToolTip(button_ZIL_additional_mining,
                    string.Format(International.GetText("Form_Settings_ToolTip_ZilFactor")));
            } else
            {
                toolTip1.SetToolTip(button_ZIL_additional_mining,
                                    string.Format(International.GetText("Form_Settings_ToolTip_ZilFactorP"),
                                    (Form_Main.ZilFactor * 100).ToString() + "%"));
            }

            //button_ZIL_additional_mining.Enabled = !ConfigManager.GeneralConfig.ForceZIL_Mining_Disable;

            // Electricity cost
            toolTip1.SetToolTip(label_Schedules, International.GetText("Form_Settings_ToolTip_ElectricityCost"));
            toolTip1.SetToolTip(textBoxScheduleCost1, International.GetText("Form_Settings_ToolTip_ElectricityCost"));
            toolTip1.SetToolTip(textBoxScheduleCost2, International.GetText("Form_Settings_ToolTip_ElectricityCost"));
            toolTip1.SetToolTip(textBoxScheduleCost3, International.GetText("Form_Settings_ToolTip_ElectricityCost"));
            toolTip1.SetToolTip(textBoxScheduleCost4, International.GetText("Form_Settings_ToolTip_ElectricityCost"));
            toolTip1.SetToolTip(textBoxScheduleCost5, International.GetText("Form_Settings_ToolTip_ElectricityCost"));

            Text = International.GetText("Form_Settings_Title");

            algorithmSettingsControl1.InitLocale(toolTip1);
        }

        #region Form this

        private void InitializeFormTranslations()
        {
            buttonDefaults.Text = International.GetText("Form_Settings_buttonDefaultsText");
            buttonSaveClose.Text = International.GetText("Form_Settings_buttonSaveText");
            buttonCloseNoSave.Text = International.GetText("Form_Settings_buttonCloseNoSaveText");
            buttonSetupAPI.Text = International.GetText("Form_Settings_buttonSetupAPI");
        }

        #endregion //Form this

        #region Tab General

        private void InitializeGeneralTabTranslations()
        {
            checkBox_AutoStartMining.Text = International.GetText("Form_Settings_General_AutoStartMining");
            checkBox_HideMiningWindows.Text = International.GetText("Form_Settings_General_HideMiningWindows");
            checkBox_MinimizeToTray.Text = International.GetText("Form_Settings_General_MinimizeToTray");
            labelDisableDetection.Text = International.GetText("Form_Settings_General_DisableDetection");
            labelDisableMonitoring.Text = International.GetText("Form_Settings_General_monitoring");

            checkBox_AutoScaleBTCValues.Text = International.GetText("Form_Settings_General_AutoScaleBTCValues");
            checkBox_StartMiningWhenIdle.Text = International.GetText("Form_Settings_General_StartMiningWhenIdle");

            checkBox_LogToFile.Text = International.GetText("Form_Settings_General_LogToFile");
            checkBoxSaveProtocolData.Text = International.GetText("Form_Settings_General_SaveProtocolData");

            checkBox_AllowMultipleInstances.Text =
                International.GetText("Form_Settings_General_AllowMultipleInstances_Text");
            checkBox_RunAtStartup.Text = International.GetText("Form_Settings_General_RunAtStartup");
            checkBox_MinimizeMiningWindows.Text = International.GetText("Form_Settings_General_MinimizeMiningWindows");
            checkBoxShowMinersVersions.Text = International.GetText("Form_Settings_General_ShowMinersVersions");

            checkBoxCheckingCUDA.Text = International.GetText("Form_Settings_checkBox_CheckingCUDA");
            checkBoxRestartDriver.Text = International.GetText("Form_Settings_checkBox_RestartDriver");
            checkBoxRestartWindows.Text = International.GetText("Form_Settings_checkBox_RestartWindows");
            checkBox_EnableAPI.Text = International.GetText("Form_Settings_checkBox_EnableAPI");
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                checkBox_show_NVdevice_manufacturer.Location = new Point(checkBox_show_NVdevice_manufacturer.Location.X + 26, checkBox_show_NVdevice_manufacturer.Location.Y); ;
                checkBox_show_AMDdevice_manufacturer.Location = new Point(checkBox_show_AMDdevice_manufacturer.Location.X + 26, checkBox_show_AMDdevice_manufacturer.Location.Y);
                checkBox_show_INTELdevice_manufacturer.Location = new Point(checkBox_show_INTELdevice_manufacturer.Location.X + 26, checkBox_show_INTELdevice_manufacturer.Location.Y);

                checkBoxRestartDriver.Location = new Point(checkBoxRestartDriver.Location.X + 26, checkBoxRestartDriver.Location.Y);
                checkBoxRestartWindows.Location = new Point(checkBoxRestartWindows.Location.X + 26, checkBoxRestartWindows.Location.Y);
            }

            checkBoxDriverWarning.Text = International.GetText("Form_Settings_General_ShowDriverVersionWarning");

            checkBoxAutoupdate.Text = International.GetText("Form_Settings_checkBoxAutoupdate");
            checkBoxHistory.Text = International.GetText("Form_Settings_checkBoxHistory");
            checkBox_BackupBeforeUpdate.Text = International.GetText("Form_Settings_checkBox_backup_before_update");
            checkBox_ABEnableOverclock.Text = International.GetText("FormSettings_ABEnableOverclock");
            checkBox_ABMinimize.Text = International.GetText("FormSettings_AB_Minimize");
            checkBox_ABDefault_mining_stopped.Text = International.GetText("FormSettings_ABDefault_mining_stopped");
            checkBox_ABDefault_program_closing.Text = International.GetText("FormSettings_ABDefault_program_closing");
            linkLabel3.Text = International.GetText("FormSettings_AB_HowToUse");
            linkLabelGetAPIkey.Text = International.GetText("FormSettings_API_HowToUse");
            labelCheckforprogramupdatesevery.Text = International.GetText("Form_Settings_labelCheckforprogramupdatesevery");

            label_Language.Text = International.GetText("Form_Settings_General_Language") + ":";
            label1.Text = International.GetText("Form_Settings_Color_profile");

            button_Lite_Algo.Text = International.GetText("Form_Settings_LiteAlgosSettings");

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
                if (Form_Main.currentVersion < Form_Main.gitlabVersion)
                {
                    linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newversion") + Form_Main.gitlabVersion.ToString();
                    buttonUpdate.Visible = true;
                }
                if (Form_Main.githubVersion <= 0 && Form_Main.gitlabVersion <= 0)
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
            if (ConfigManager.GeneralConfig.ForkFixVersion < Form_Main.gitlabVersion)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newversion") + Form_Main.gitlabVersion.ToString();
                buttonUpdate.Visible = true;
                linkLabelNewVersion2.LinkBehavior = LinkBehavior.SystemDefault;
            }
            if (Form_Main.githubVersion <= 0 && Form_Main.gitlabVersion <= 0)
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
            checkBox_ShowPowerOfDisabledDevices.Text = International.GetText("Form_Settings_checkBox_ShowPowerOfDisabledDevices");
            checkBoxInstall_root_certificates.Text = International.GetText("Form_Settings_checkBox_Install_root_certificates");
            checkBox_Additional_info_about_device.Text = International.GetText("Form_Settings_checkBox_Additional_info_about_device");
            checkBox_DisplayConnected.Text = International.GetText("Form_Settings_checkBox_DisplayConnected");

            checkBox_show_NVdevice_manufacturer.Text = International.GetText("Form_Settings_checkBox_show_NVdevice_manufacturer");
            checkBox_orderPrice.Text = International.GetText("Form_Settings_checkBox_orderPrice");
            checkBoxLast24hours.Text = International.GetText("Form_Settings_checkBox_Last24hours");
            checkBoxShortTerm.Text = International.GetText("Form_Settings_checkBox_ShortTerm");
            checkBoxMiningFee.Text = International.GetText("Form_Settings_checkBox_checkBoxMiningFee");
            checkBox_Show_memory_temp.Text = International.GetText("Form_Settings_checkBox_show_memory_temp");
            label_show_manufacturer.Text = International.GetText("Form_Settings_label_show_manufacturer");
            label_restart_nv_lost.Text = International.GetText("Form_Settings_label_restart_nv_lost");
            checkBox_show_AMDdevice_manufacturer.Text = International.GetText("Form_Settings_checkBox_show_AMDdevice_manufacturer");
            checkBox_show_INTELdevice_manufacturer.Text = International.GetText("Form_Settings_checkBox_show_INTELdevice_manufacturer");
            checkBox_ShowDeviceMemSize.Text = International.GetText("Form_Settings_checkBox_show_device_memsize");
            //checkBox_ShowDeviceBusId.Text = International.GetText("Form_Settings_checkBox_show_device_busId");

            checkbox_Use_OpenHardwareMonitor.Text = International.GetText("Form_Settings_checkbox_Use_OpenHardwareMonitor");
            Checkbox_Save_windows_size_and_position.Text = International.GetText("Form_Settings_Checkbox_Save_windows_size_and_position");
            checkBox_sorting_list_of_algorithms.Text = International.GetText("Form_Settings_checkBox_sorting_list_of_algorithms");
            checkBox_DisableTooltips.Text = International.GetText("Form_Settings_checkBox_DisableToltips");
            checkBox_program_monitoring.Text = International.GetText("Form_Settings_checkBox_program_monitoring");
            checkBoxEnableRigRemoteView.Text = International.GetText("Form_Settings_checkBox_EnableRigRemoteView");
            checkBox_ShowFanAsPercent.Text = International.GetText("Form_Settings_checkBox_ShowFanAsPercent");
            checkbox_Group_same_devices.Text = International.GetText("Form_Settings_checkbox_Group_same_devices");
            checkBox_withPower.Text = International.GetText("Form_Settings_checkbox_withPower");
            checkBox_By_profitability_of_all_devices.Text = International.GetText("FormSettings_By_profitability_of_all_devices");
            checkBox_Force_mining_if_nonprofitable.Text = International.GetText("Form_Settings_checkBox_Force_mining_if_nonprofitable");
            checkbox_wallet_balance.Text = International.GetText("Form_Main_Checkbox_wallet_balance");
            checkBox_Show_profit_with_power_consumption.Text = International.GetText("Form_Settings_checkBox_Show_profit_with_power_consumption");
            checkBox_Show_Total_Power.Text = International.GetText("Form_Settings_checkBox_Show_Total_Power");
            checkBox_fiat.Text = International.GetText("Form_Settings_checkBox_fiat");
            checkBox_AlwaysOnTop.Text = International.GetText("Form_Settings_checkBox_AlwaysOnTop");
            label_psu.Text = International.GetText("Form_Settings_label_psu");
            label_MBpower.Text = International.GetText("Form_Settings_label_MBpower");
            labelAddAMD.Text = International.GetText("Form_Settings_label_AddAMD");
            checkBox_Disable_extra_launch_parameter_checking.Text = International.GetText("Form_Settings_checkBox_Disable_extra_launch_parameter_checking");
            checkBoxHideUnused.Text = International.GetText("Form_Settings_checkBox_Hide_Unused");
            checkBoxHideUnused2.Text = International.GetText("Form_Settings_checkBox_Hide_Unused");
            button_ZIL_additional_mining.Text = International.GetText("Form_Settings_button_ZIL_additional_mining");
            checkBox_DisableDetectionCPU.Text = International.GetText("Form_Settings_checkBox_DisableDetectionCPU");
            label_AutoStartMiningDelay.Text = International.GetText("Form_Settings_label_AutoStartMiningDelay");
            groupBox1.Text = International.GetText("Form_Settings_groupBox1");
            groupBoxAPIkeys.Text = International.GetText("Form_Settings_groupBoxAPIkeys");

            label_switching_algorithms.Text = International.GetText("Form_Settings_label_switching_algorithms");
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms1"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms3"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms5"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms10"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms15"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms30"));
            comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms60"));
            //comboBox_switching_algorithms.Items.Add(International.GetText("Form_Settings_comboBox_switching_algorithms0"));

            label_devices_count.Text = International.GetText("Form_Settings_label_devices_count");
            tabPageAbout.Text = International.GetText("Form_Settings_tabPageAbout");
            groupBoxInfo.Text = International.GetText("Form_Settings_groupBoxInfo");
            groupBoxUpdates.Text = International.GetText("Form_Settings_groupBoxUpdates");
            groupBoxBackup.Text = International.GetText("Form_Settings_groupBoxBackup");
            buttonLicence.Text = International.GetText("Form_Settings_buttonLicence");
            buttonHistory.Text = International.GetText("Form_Settings_buttonHistory");

            groupBoxConnection.Text = International.GetText("FormSettings_Tab_Advanced_Group_Connection");
            checkBoxProxySSL.Text = International.GetText("FormSettings_Tab_Advanced_checkBoxProxySSL");
            checkBoxEnableProxy.Text = International.GetText("FormSettings_Tab_Advanced_checkBoxEnableProxy");
            //checkBoxProxyAsFailover.Text = International.GetText("FormSettings_Tab_Advanced_ProxyAsFailover");
            //checkBoxStale.Text = International.GetText("FormSettings_Tab_Advanced_StaleProxy");
            if (Globals.MiningLocation.Length > 1)
            {
                checkBoxEnableProxy.Enabled = true;
                checkBoxProxySSL.Enabled = true;
            } else
            {
                checkBoxEnableProxy.Enabled = false;
                checkBoxProxySSL.Enabled = false;
            }

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

            label_MinIdleSeconds.Text = International.GetText("Form_Settings_General_MinIdleSeconds") + ":";
            label_LogMaxFileSize.Text = International.GetText("Form_Settings_General_LogMaxFileSize") + ":";
            label_MinProfit.Text = International.GetText("Form_Settings_General_MinimumProfit") + ":";
            label_displayCurrency.Text = International.GetText("Form_Settings_DisplayCurrency");
            label_Schedules.Text = International.GetText("Form_Settings_label_Schedules");
            label_Schedules2.Text = International.GetText("Form_Settings_label_Schedules2");
            labelFrom1.Text = International.GetText("Form_Settings_label_From");
            labelFrom2.Text = International.GetText("Form_Settings_label_From");
            labelFrom3.Text = International.GetText("Form_Settings_label_From");
            labelFrom4.Text = International.GetText("Form_Settings_label_From");
            labelFrom5.Text = International.GetText("Form_Settings_label_From");
            labelTo1.Text = International.GetText("Form_Settings_label_To");
            labelTo2.Text = International.GetText("Form_Settings_label_To");
            labelTo3.Text = International.GetText("Form_Settings_label_To");
            labelTo4.Text = International.GetText("Form_Settings_label_To");
            labelTo5.Text = International.GetText("Form_Settings_label_To");
            labelCost1.Text = International.GetText("Form_Settings_label_Cost");
            labelCost2.Text = International.GetText("Form_Settings_label_Cost");
            labelCost3.Text = International.GetText("Form_Settings_label_Cost");
            labelCost4.Text = International.GetText("Form_Settings_label_Cost");
            labelCost5.Text = International.GetText("Form_Settings_label_Cost");

            labelPowerCurrency1.Text = ConfigManager.GeneralConfig.DisplayCurrency + "/" + International.GetText("Form_Main_Power6") + "." + International.GetText("Hour");
            labelPowerCurrency2.Text = ConfigManager.GeneralConfig.DisplayCurrency + "/" + International.GetText("Form_Main_Power6") + "." + International.GetText("Hour");
            labelPowerCurrency3.Text = ConfigManager.GeneralConfig.DisplayCurrency + "/" + International.GetText("Form_Main_Power6") + "." + International.GetText("Hour");
            labelPowerCurrency4.Text = ConfigManager.GeneralConfig.DisplayCurrency + "/" + International.GetText("Form_Main_Power6") + "." + International.GetText("Hour");
            labelPowerCurrency5.Text = ConfigManager.GeneralConfig.DisplayCurrency + "/" + International.GetText("Form_Main_Power6") + "." + International.GetText("Hour");
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
            }
            else
            {
                this.Top = 0;
                this.Left = 0;
            }

            algorithmsListView1.InitLocale();
            algorithmsListViewOverClock1.InitLocale();

            comboBox_ColorProfile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;

            //checkBox_RunAtStartup.BackColor = Color.DarkGreen;

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

                foreach (var lbl in this.Controls.OfType<ListBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                tabPageAdvanced1.BackColor = Form_Main._backColor;
                tabPageAdvanced1.ForeColor = Form_Main._foreColor;

                tabPageDevicesAlgos.BackColor = Form_Main._backColor;
                tabPageDevicesAlgos.ForeColor = Form_Main._foreColor;

                tabPageOverClock.BackColor = Form_Main._backColor;
                tabPageOverClock.ForeColor = Form_Main._foreColor;

                tabPageAbout.BackColor = Form_Main._backColor;
                tabPageAbout.ForeColor = Form_Main._foreColor;

                tabPagePower.BackColor = Form_Main._backColor;
                tabPagePower.ForeColor = Form_Main._foreColor;

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
                foreach (var lbl in tabPageOverClock.Controls.OfType<GroupBox>())
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
                foreach (var lbl in tabPageOverClock.Controls.OfType<Button>())
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
                linkLabelCurrentVersion.MouseEnter += (s, e) => linkLabelCurrentVersion.LinkBehavior = LinkBehavior.AlwaysUnderline;
                linkLabelNewVersion2.MouseLeave += (s, e) => linkLabelNewVersion2.LinkBehavior = LinkBehavior.NeverUnderline;
                linkLabelNewVersion2.MouseEnter += (s, e) => linkLabelNewVersion2.LinkBehavior = LinkBehavior.AlwaysUnderline;

                linkLabelNewVersion2.BackColor = Form_Main._backColor;
                linkLabelNewVersion2.ForeColor = Form_Main._textColor;
                linkLabelNewVersion2.LinkColor = Form_Main._textColor;

                linkLabel3.BackColor = Form_Main._backColor;
                linkLabel3.ForeColor = Form_Main._textColor;
                linkLabel3.LinkColor = Form_Main._textColor;
                linkLabel3.ActiveLinkColor = Form_Main._textColor;
                linkLabel3.MouseLeave += (s, e) => linkLabel3.LinkBehavior = LinkBehavior.NeverUnderline;
                linkLabel3.MouseEnter += (s, e) => linkLabel3.LinkBehavior = LinkBehavior.AlwaysUnderline;

                linkLabelRigRemoteView.BackColor = Form_Main._backColor;
                linkLabelRigRemoteView.ForeColor = Form_Main._textColor;
                linkLabelRigRemoteView.LinkColor = Form_Main._textColor;
                linkLabelRigRemoteView.ActiveLinkColor = Form_Main._textColor;
                linkLabelRigRemoteView.MouseLeave += (s, e) => linkLabelRigRemoteView.LinkBehavior = LinkBehavior.NeverUnderline;
                linkLabelRigRemoteView.MouseEnter += (s, e) => linkLabelRigRemoteView.LinkBehavior = LinkBehavior.AlwaysUnderline;

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

                foreach (var lbl in tabPageOverClock.Controls.OfType<UserControl>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                //comboBox_ServiceLocation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                comboBox_TimeUnit.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
                comboBoxZones.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
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

                foreach (var lbl in this.tabPageOverClock.Controls.OfType<CheckBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }
                foreach (var lbl in this.tabPageOverClock.Controls.OfType<PictureBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }

                checkBox_BackupBeforeUpdate.BackColor = Form_Main._backColor;
                checkBox_BackupBeforeUpdate.ForeColor = Form_Main._textColor;
                checkBoxAutoupdate.BackColor = Form_Main._backColor;
                checkBoxAutoupdate.ForeColor = Form_Main._textColor;
                checkBoxHistory.BackColor = Form_Main._backColor;
                checkBoxHistory.ForeColor = Form_Main._textColor;

                checkBox_Force_mining_if_nonprofitable.BackColor = Form_Main._backColor;
                checkBox_Force_mining_if_nonprofitable.ForeColor = Form_Main._textColor;

                checkbox_wallet_balance.BackColor = Form_Main._backColor;
                checkbox_wallet_balance.ForeColor = Form_Main._textColor;

                checkBox_Show_profit_with_power_consumption.BackColor = Form_Main._backColor;
                checkBox_Show_profit_with_power_consumption.ForeColor = Form_Main._textColor;
                checkBox_Show_Total_Power.BackColor = Form_Main._backColor;
                checkBox_Show_Total_Power.ForeColor = Form_Main._textColor;

                checkBox_Allow_remote_management.BackColor = Form_Main._backColor;
                checkBox_Allow_remote_management.ForeColor = Form_Main._textColor;

                checkBox_Send_actual_version_info.BackColor = Form_Main._backColor;
                checkBox_Send_actual_version_info.ForeColor = Form_Main._textColor;

                checkBox_ShowPowerOfDisabledDevices.BackColor = Form_Main._backColor;
                checkBox_ShowPowerOfDisabledDevices.ForeColor = Form_Main._textColor;

                checkBoxInstall_root_certificates.BackColor = Form_Main._backColor;
                checkBoxInstall_root_certificates.ForeColor = Form_Main._textColor;

                checkBox_Additional_info_about_device.BackColor = Form_Main._backColor;
                checkBox_Additional_info_about_device.ForeColor = Form_Main._textColor;

                checkBox_DisplayConnected.BackColor = Form_Main._backColor;
                checkBox_DisplayConnected.ForeColor = Form_Main._textColor;

                checkBox_show_NVdevice_manufacturer.BackColor = Form_Main._backColor;
                checkBox_show_NVdevice_manufacturer.ForeColor = Form_Main._textColor;

                label_show_manufacturer.BackColor = Form_Main._backColor;
                label_show_manufacturer.ForeColor = Form_Main._textColor;

                checkBox_orderPrice.BackColor = Form_Main._backColor;
                checkBox_orderPrice.ForeColor = Form_Main._textColor;

                checkBoxLast24hours.BackColor = Form_Main._backColor;
                checkBoxLast24hours.ForeColor = Form_Main._textColor;

                checkBoxShortTerm.BackColor = Form_Main._backColor;
                checkBoxShortTerm.ForeColor = Form_Main._textColor;

                checkBoxMiningFee.BackColor = Form_Main._backColor;
                checkBoxMiningFee.ForeColor = Form_Main._textColor;

                checkBox_Show_memory_temp.BackColor = Form_Main._backColor;
                checkBox_Show_memory_temp.ForeColor = Form_Main._textColor;
                checkBox_show_AMDdevice_manufacturer.BackColor = Form_Main._backColor;
                checkBox_show_AMDdevice_manufacturer.ForeColor = Form_Main._textColor;

                checkBox_show_INTELdevice_manufacturer.BackColor = Form_Main._backColor;
                checkBox_show_INTELdevice_manufacturer.ForeColor = Form_Main._textColor;

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

                checkBox_DisableTooltips.BackColor = Form_Main._backColor;
                checkBox_DisableTooltips.ForeColor = Form_Main._textColor;

                checkBox_program_monitoring.BackColor = Form_Main._backColor;
                checkBox_program_monitoring.ForeColor = Form_Main._textColor;

                checkBoxEnableRigRemoteView.BackColor = Form_Main._backColor;
                checkBoxEnableRigRemoteView.ForeColor = Form_Main._textColor;

                checkBox_ShowFanAsPercent.BackColor = Form_Main._backColor;
                checkBox_ShowFanAsPercent.ForeColor = Form_Main._textColor;

                checkbox_Group_same_devices.BackColor = Form_Main._backColor;
                checkbox_Group_same_devices.ForeColor = Form_Main._textColor;

                checkBox_withPower.BackColor = Form_Main._backColor;
                checkBox_withPower.ForeColor = Form_Main._textColor;

                checkBox_By_profitability_of_all_devices.BackColor = Form_Main._backColor;
                checkBox_By_profitability_of_all_devices.ForeColor = Form_Main._textColor;

                checkBox_Disable_extra_launch_parameter_checking.BackColor = Form_Main._backColor;
                checkBox_Disable_extra_launch_parameter_checking.ForeColor = Form_Main._textColor;

                checkBoxHideUnused.BackColor = Form_Main._backColor;
                checkBoxHideUnused.ForeColor = Form_Main._textColor;
                checkBoxHideUnused2.BackColor = Form_Main._backColor;
                checkBoxHideUnused2.ForeColor = Form_Main._textColor;

                //checkBox_Zil_GMiner.BackColor = Form_Main._backColor;
                //checkBox_Zil_GMiner.ForeColor = Form_Main._textColor;

                checkBox_ABEnableOverclock.BackColor = Form_Main._backColor;
                checkBox_ABEnableOverclock.ForeColor = Form_Main._textColor;
                checkBox_ABDefault_mining_stopped.BackColor = Form_Main._backColor;
                checkBox_ABDefault_mining_stopped.ForeColor = Form_Main._textColor;
                checkBox_ABDefault_program_closing.BackColor = Form_Main._backColor;
                checkBox_ABDefault_program_closing.ForeColor = Form_Main._textColor;

                checkBox_ABMinimize.BackColor = Form_Main._backColor;
                checkBox_ABMinimize.ForeColor = Form_Main._textColor;

                textBox_AutoStartMiningDelay.BackColor = Form_Main._backColor;
                textBox_AutoStartMiningDelay.ForeColor = Form_Main._foreColor;
                textBox_AutoStartMiningDelay.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleFrom1.BackColor = Form_Main._backColor;
                textBoxScheduleFrom1.ForeColor = Form_Main._foreColor;
                textBoxScheduleFrom1.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleTo1.BackColor = Form_Main._backColor;
                textBoxScheduleTo1.ForeColor = Form_Main._foreColor;
                textBoxScheduleTo1.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleCost1.BackColor = Form_Main._backColor;
                textBoxScheduleCost1.ForeColor = Form_Main._foreColor;
                textBoxScheduleCost1.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleFrom2.BackColor = Form_Main._backColor;
                textBoxScheduleFrom2.ForeColor = Form_Main._foreColor;
                textBoxScheduleFrom2.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleTo2.BackColor = Form_Main._backColor;
                textBoxScheduleTo2.ForeColor = Form_Main._foreColor;
                textBoxScheduleTo2.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleCost2.BackColor = Form_Main._backColor;
                textBoxScheduleCost2.ForeColor = Form_Main._foreColor;
                textBoxScheduleCost2.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleFrom3.BackColor = Form_Main._backColor;
                textBoxScheduleFrom3.ForeColor = Form_Main._foreColor;
                textBoxScheduleFrom3.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleTo3.BackColor = Form_Main._backColor;
                textBoxScheduleTo3.ForeColor = Form_Main._foreColor;
                textBoxScheduleTo3.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleCost3.BackColor = Form_Main._backColor;
                textBoxScheduleCost3.ForeColor = Form_Main._foreColor;
                textBoxScheduleCost3.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleFrom4.BackColor = Form_Main._backColor;
                textBoxScheduleFrom4.ForeColor = Form_Main._foreColor;
                textBoxScheduleFrom4.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleTo4.BackColor = Form_Main._backColor;
                textBoxScheduleTo4.ForeColor = Form_Main._foreColor;
                textBoxScheduleTo4.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleCost4.BackColor = Form_Main._backColor;
                textBoxScheduleCost4.ForeColor = Form_Main._foreColor;
                textBoxScheduleCost4.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleFrom5.BackColor = Form_Main._backColor;
                textBoxScheduleFrom5.ForeColor = Form_Main._foreColor;
                textBoxScheduleFrom5.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleTo5.BackColor = Form_Main._backColor;
                textBoxScheduleTo5.ForeColor = Form_Main._foreColor;
                textBoxScheduleTo5.BorderStyle = BorderStyle.FixedSingle;

                textBoxScheduleCost5.BackColor = Form_Main._backColor;
                textBoxScheduleCost5.ForeColor = Form_Main._foreColor;
                textBoxScheduleCost5.BorderStyle = BorderStyle.FixedSingle;


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

                textBoxAPIport.BackColor = Form_Main._backColor;
                textBoxAPIport.ForeColor = Form_Main._foreColor;
                textBoxAPIport.BorderStyle = BorderStyle.FixedSingle;

                textBox_MinProfit.BackColor = Form_Main._backColor;
                textBox_MinProfit.ForeColor = Form_Main._foreColor;
                textBox_MinProfit.BorderStyle = BorderStyle.FixedSingle;

                textBox_SwitchProfitabilityThreshold.BackColor = Form_Main._backColor;
                textBox_SwitchProfitabilityThreshold.ForeColor = Form_Main._foreColor;
                textBox_SwitchProfitabilityThreshold.BorderStyle = BorderStyle.FixedSingle;

                labelRestartProgram.BackColor = Form_Main._backColor;
                labelRestartProgram.ForeColor = Form_Main._foreColor;

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
            }
            else
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
            tabControlGeneral.TabPages[1].Text = International.GetText("FormSettings_Tab_Power");
            tabControlGeneral.TabPages[2].Text = International.GetText("FormSettings_Tab_Advanced");
            tabControlGeneral.TabPages[3].Text = International.GetText("FormSettings_Tab_Devices_Algorithms");
            tabControlGeneral.TabPages[4].Text = International.GetText("FormSettings_ABOverclockTab");

            groupBox_Main.Text = International.GetText("FormSettings_Tab_General_Group_Main");
            groupBoxTariffs.Text = International.GetText("FormSettings_Tab_General_Group_Tariffs");
            groupBox_additionally.Text = International.GetText("FormSettings_Tab_General_Group_Additionally");
            groupBox_Localization.Text = International.GetText("FormSettings_Tab_General_Group_Localization");
            groupBox_Logging.Text = International.GetText("FormSettings_Tab_General_Group_Logging");
            groupBox_RigManager.Text = International.GetText("FormSettings_Tab_General_Group_RigManager");
            groupBox_Misc.Text = International.GetText("FormSettings_Tab_General_Group_Misc");
            // advanced
            groupBox_Miners.Text = International.GetText("FormSettings_Tab_Advanced_Group_Miners");

            label_SwitchProfitabilityThreshold.Text =
                International.GetText("Form_Settings_General_SwitchProfitabilityThreshold");
            if (!ComputeDeviceManager.Available.HasNvidia)
            {
                //checkBox_Zil_GMiner.Enabled = false;
            }
            if (ConfigManager.GeneralConfig.EnableAPI)
            {
                buttonSetupAPI.Enabled = true;
            } else
            {
                buttonSetupAPI.Enabled = false;
            }
        }

        private void InitializeGeneralTabCallbacks()
        {
            // Add EventHandler for all the general tab's checkboxes
            {
                checkBox_AutoScaleBTCValues.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionCPU.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionAMD.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionNVIDIA.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableDetectionINTEL.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxCPUmonitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxNVMonitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxAMDmonitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxINTELmonitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_MinimizeToTray.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_HideMiningWindows.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_AlwaysOnTop.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_StartMiningWhenIdle.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                //checkBox_NVIDIAP0State.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_LogToFile.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxSaveProtocolData.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_AutoStartMining.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_AllowMultipleInstances.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_MinimizeMiningWindows.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxShowMinersVersions.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxCheckingCUDA.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxRestartDriver.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxDriverWarning.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxRestartWindows.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_EnableAPI.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Allow_remote_management.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Send_actual_version_info.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ShowPowerOfDisabledDevices.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxInstall_root_certificates.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Force_mining_if_nonprofitable.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkbox_wallet_balance.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Show_profit_with_power_consumption.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Show_Total_Power.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Additional_info_about_device.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisplayConnected.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_show_NVdevice_manufacturer.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_orderPrice.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxLast24hours.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxShortTerm.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxMiningFee.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Show_memory_temp.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_show_AMDdevice_manufacturer.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_show_INTELdevice_manufacturer.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ShowDeviceMemSize.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                //checkBox_ShowDeviceBusId.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkbox_Use_OpenHardwareMonitor.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                Checkbox_Save_windows_size_and_position.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_sorting_list_of_algorithms.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_DisableTooltips.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_program_monitoring.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxEnableRigRemoteView.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxAPI.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ShowFanAsPercent.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_fiat.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkbox_Group_same_devices.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_withPower.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_By_profitability_of_all_devices.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxAutoupdate.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxHistory.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_BackupBeforeUpdate.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_Disable_extra_launch_parameter_checking.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxHideUnused.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxHideUnused2.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                //checkBox_Zil_GMiner.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ABEnableOverclock.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ABDefault_mining_stopped.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ABDefault_program_closing.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBox_ABMinimize.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxProxySSL.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                checkBoxEnableProxy.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                //checkBoxProxyAsFailover.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
                //checkBoxStale.CheckedChanged += GeneralCheckBoxes_CheckedChanged;
            }
            // Add EventHandler for all the general tab's textboxes
            {
                textBox_AutoStartMiningDelay.Leave += GeneralTextBoxes_Leave;
                textBox_MinIdleSeconds.Leave += GeneralTextBoxes_Leave;
                textBoxAPIport.Leave += GeneralTextBoxes_Leave;
                textBox_LogMaxFileSize.Leave += GeneralTextBoxes_Leave;
                textBox_MinProfit.Leave += GeneralTextBoxes_Leave;
                textBox_psu.Leave += GeneralTextBoxes_Leave;
                textBox_mb.Leave += GeneralTextBoxes_Leave;
                textBoxAddAMD.Leave += GeneralTextBoxes_Leave;
                textBox_SwitchProfitabilityThreshold.Leave += GeneralTextBoxes_Leave;

                textBoxScheduleFrom1.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleTo1.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleCost1.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleFrom2.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleTo2.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleCost2.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleFrom3.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleTo3.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleCost3.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleFrom4.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleTo4.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleCost4.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleFrom5.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleTo5.Leave += GeneralTextBoxes_Leave;
                textBoxScheduleCost5.Leave += GeneralTextBoxes_Leave;

                // set int only keypress
                textBox_MinIdleSeconds.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                textBoxAPIport.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
                // set double only keypress
                textBox_MinProfit.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBoxScheduleCost1.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBoxScheduleCost2.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBoxScheduleCost3.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBoxScheduleCost4.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBoxScheduleCost5.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBox_psu.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBox_mb.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
                textBoxAddAMD.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
            }
            // Add EventHandler for all the general tab's textboxes
            {
                comboBox_Language.Leave += GeneralComboBoxes_Leave;
                comboBox_TimeUnit.Leave += GeneralComboBoxes_Leave;
                comboBoxZones.Leave += GeneralComboBoxes_Leave;
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
                if (Form_Main.walletType.Equals("P2SH"))
                {
                    checkbox_wallet_balance.Checked = false;
                    ConfigManager.GeneralConfig.Show_wallet_balance = false;
                }
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
                checkBox_DisableDetectionINTEL.Checked = ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionINTEL;
                checkBoxCPUmonitoring.Checked = ConfigManager.GeneralConfig.DisableMonitoringCPU;
                checkBoxNVMonitoring.Checked = ConfigManager.GeneralConfig.DisableMonitoringNVIDIA;
                checkBoxAMDmonitoring.Checked = ConfigManager.GeneralConfig.DisableMonitoringAMD;
                checkBoxINTELmonitoring.Checked = ConfigManager.GeneralConfig.DisableMonitoringINTEL;
                checkBox_AutoScaleBTCValues.Checked = ConfigManager.GeneralConfig.AutoScaleBTCValues;
                checkBox_StartMiningWhenIdle.Checked = ConfigManager.GeneralConfig.StartMiningWhenIdle;
                //checkBox_NVIDIAP0State.Checked = ConfigManager.GeneralConfig.NVIDIAP0State;
                checkBox_LogToFile.Checked = ConfigManager.GeneralConfig.LogToFile;
                checkBoxSaveProtocolData.Checked = ConfigManager.GeneralConfig.SaveProtocolData;
                if (checkBox_LogToFile.Checked)
                {
                    textBox_LogMaxFileSize.Enabled = true;
                    checkBoxSaveProtocolData.Enabled = true;
                }
                else
                {
                    textBox_LogMaxFileSize.Enabled = false;
                    checkBoxSaveProtocolData.Enabled = false;
                }

                checkBox_AllowMultipleInstances.Checked = ConfigManager.GeneralConfig.AllowMultipleInstances;
                checkBox_RunAtStartup.Checked = IsInStartupRegistry();
                checkBoxShowMinersVersions.Checked = ConfigManager.GeneralConfig.ShowMinersVersions;
                checkBox_MinimizeMiningWindows.Checked = ConfigManager.GeneralConfig.MinimizeMiningWindows;
                checkBox_MinimizeMiningWindows.Enabled = !ConfigManager.GeneralConfig.HideMiningWindows;
                checkBoxCheckingCUDA.Checked = ConfigManager.GeneralConfig.CheckingCUDA;
                checkBoxRestartDriver.Checked = ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost;
                checkBoxDriverWarning.Checked = ConfigManager.GeneralConfig.ShowDriverVersionWarning;
                checkBoxRestartWindows.Checked = ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost;
                checkBox_EnableAPI.Checked = ConfigManager.GeneralConfig.EnableAPIkeys;
                checkBox_Allow_remote_management.Checked = ConfigManager.GeneralConfig.Allow_remote_management;
                checkBox_Send_actual_version_info.Checked = ConfigManager.GeneralConfig.Send_actual_version_info;
                checkBox_ShowPowerOfDisabledDevices.Checked = ConfigManager.GeneralConfig.ShowPowerOfDisabledDevices;
                checkBoxInstall_root_certificates.Checked = ConfigManager.GeneralConfig.InstallRootCerts;
                checkBox_Force_mining_if_nonprofitable.Checked = ConfigManager.GeneralConfig.Force_mining_if_nonprofitable;
                checkbox_wallet_balance.Checked = ConfigManager.GeneralConfig.Show_wallet_balance;
                checkBox_Show_profit_with_power_consumption.Checked = ConfigManager.GeneralConfig.DecreasePowerCost;
                checkBox_Show_Total_Power.Checked = ConfigManager.GeneralConfig.ShowTotalPower;
                checkBox_fiat.Checked = ConfigManager.GeneralConfig.FiatCurrency;
                checkBox_Additional_info_about_device.Checked = ConfigManager.GeneralConfig.Additional_info_about_device;
                checkBox_DisplayConnected.Checked = ConfigManager.GeneralConfig.Show_displayConected;
                checkBox_show_NVdevice_manufacturer.Checked = ConfigManager.GeneralConfig.Show_NVdevice_manufacturer;
                checkBox_orderPrice.Checked = ConfigManager.GeneralConfig.Use_orders_price;
                checkBoxLast24hours.Checked = ConfigManager.GeneralConfig.Use_Last24hours;
                checkBoxShortTerm.Checked = ConfigManager.GeneralConfig.ShortTerm;
                checkBoxMiningFee.Checked = ConfigManager.GeneralConfig.NicehashMiningFee;
                checkBox_Show_memory_temp.Checked = ConfigManager.GeneralConfig.Show_memory_temperature;
                checkBox_show_AMDdevice_manufacturer.Checked = ConfigManager.GeneralConfig.Show_AMDdevice_manufacturer;
                checkBox_show_INTELdevice_manufacturer.Checked = ConfigManager.GeneralConfig.Show_INTELdevice_manufacturer;
                checkBox_ShowDeviceMemSize.Checked = ConfigManager.GeneralConfig.Show_ShowDeviceMemSize;
                //checkBox_ShowDeviceBusId.Checked = ConfigManager.GeneralConfig.Show_ShowDeviceBusId;
                checkbox_Use_OpenHardwareMonitor.Checked = ConfigManager.GeneralConfig.Use_OpenHardwareMonitor;
                Checkbox_Save_windows_size_and_position.Checked = ConfigManager.GeneralConfig.Save_windows_size_and_position;
                checkBox_DisableTooltips.Checked = ConfigManager.GeneralConfig.DisableTooltips;
                checkBox_program_monitoring.Checked = ConfigManager.GeneralConfig.ProgramMonitoring;
                checkBoxEnableRigRemoteView.Checked = ConfigManager.GeneralConfig.EnableRigRemoteView;
                checkBoxAPI.Checked = ConfigManager.GeneralConfig.EnableAPI;
                checkBox_sorting_list_of_algorithms.Checked = ConfigManager.GeneralConfig.ColumnSort;
                checkBox_ShowFanAsPercent.Checked = ConfigManager.GeneralConfig.ShowFanAsPercent;
                checkbox_Group_same_devices.Checked = ConfigManager.GeneralConfig.Group_same_devices;
                checkBox_withPower.Checked = ConfigManager.GeneralConfig.with_power;
                checkBox_By_profitability_of_all_devices.Checked = ConfigManager.GeneralConfig.By_profitability_of_all_devices;
                checkBoxAutoupdate.Checked = ConfigManager.GeneralConfig.ProgramAutoUpdate;
                checkBoxHistory.Checked = ConfigManager.GeneralConfig.ShowHistory;
                checkBox_BackupBeforeUpdate.Checked = ConfigManager.GeneralConfig.BackupBeforeUpdate;
                checkBox_Disable_extra_launch_parameter_checking.Checked = ConfigManager.GeneralConfig.Disable_extra_launch_parameter_checking;
                checkBoxHideUnused.Checked = ConfigManager.GeneralConfig.Hide_unused_algorithms;
                checkBoxHideUnused2.Checked = ConfigManager.GeneralConfig.Hide_unused_algorithms;
                //checkBox_Zil_GMiner.Checked = ConfigManager.GeneralConfig.Zilliqua_GMiner;
                checkBox_ABEnableOverclock.Checked = ConfigManager.GeneralConfig.ABEnableOverclock;
                checkBox_ABDefault_mining_stopped.Checked = ConfigManager.GeneralConfig.ABDefaultMiningStopped;
                checkBox_ABDefault_program_closing.Checked = ConfigManager.GeneralConfig.ABDefaultProgramClosing;
                checkBox_ABMinimize.Checked = ConfigManager.GeneralConfig.ABMinimize;
                checkBoxProxySSL.Checked = ConfigManager.GeneralConfig.ProxySSL;
                checkBoxEnableProxy.Checked = ConfigManager.GeneralConfig.EnableProxy;
                //checkBoxProxyAsFailover.Checked = ConfigManager.GeneralConfig.ProxyAsFailover;
                //checkBoxStale.Checked = ConfigManager.GeneralConfig.StaleProxy;

                //Zil_GMiner = ConfigManager.GeneralConfig.Zilliqua_GMiner;

                ConfigManager.GeneralConfig.ProxyAsFailover = false; //отключим до лучших времён
                ConfigManager.GeneralConfig.StaleProxy = false;

                if (checkBoxEnableProxy.Checked)
                {
                    if (Globals.MiningLocation.Length > 1)
                    {
                        checkBoxProxySSL.Enabled = true;
                    } else
                    {
                        checkBoxProxySSL.Enabled = false;
                    }
                }
                else
                {
                    checkBoxProxySSL.Enabled = false;
                }
                if (checkBoxEnableRigRemoteView.Checked)
                {
                    string ip = GetLocalIPAddress();
                    if (!string.IsNullOrEmpty(ip))
                    {
                        linkLabelRigRemoteView.Text = "http://" + ip + ":" + ConfigManager.GeneralConfig.RigRemoteViewPort.ToString();
                    }
                    linkLabelRigRemoteView.Visible = true;
                }
                else
                {
                    linkLabelRigRemoteView.Visible = false;
                }
                if (checkBoxAPI.Checked)
                {

                } else
                {
                    textBoxAPIport.Enabled = false;
                }
            }

            // Textboxes
            {
                textBox_MinIdleSeconds.Text = ConfigManager.GeneralConfig.MinIdleSeconds.ToString();
                textBoxAPIport.Text = ConfigManager.GeneralConfig.RigAPiPort.ToString();
                textBox_LogMaxFileSize.Text = ConfigManager.GeneralConfig.LogMaxFileSize.ToString();
                textBox_AutoStartMiningDelay.Text = ConfigManager.GeneralConfig.AutoStartMiningDelay.ToString();
                textBox_SwitchProfitabilityThreshold.Text = ((ConfigManager.GeneralConfig.SwitchProfitabilityThreshold) * 100)
                    .ToString("F1").Replace(',', '.'); // force comma;
                textBox_psu.Text = ConfigManager.GeneralConfig.PowerPSU.ToString();
                textBox_mb.Text = ConfigManager.GeneralConfig.PowerMB.ToString();
                textBoxAddAMD.Text = ConfigManager.GeneralConfig.PowerAddAMD.ToString();

                SetZoneTable(ConfigManager.GeneralConfig.PowerTarif);
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
                comboBoxZones.SelectedIndex = ConfigManager.GeneralConfig.PowerTarif;

                currencyConverterCombobox.SelectedItem = ConfigManager.GeneralConfig.DisplayCurrency;
                comboBox_ColorProfile.SelectedIndex = ConfigManager.GeneralConfig.ColorProfileIndex;

                    comboBox_switching_algorithms.SelectedIndex = ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex;

                comboBox_devices_count.SelectedIndex = ConfigManager.GeneralConfig.DevicesCountIndex;
                comboBoxCheckforprogramupdatesevery.SelectedIndex = ConfigManager.GeneralConfig.ProgramUpdateIndex;
                comboBoxRestartProgram.SelectedIndex = ConfigManager.GeneralConfig.ProgramRestartIndex;
            }

            checkBox_ABMinimize.Enabled = checkBox_ABEnableOverclock.Checked;
            checkBox_ABDefault_mining_stopped.Enabled = checkBox_ABEnableOverclock.Checked;
            checkBox_ABDefault_program_closing.Enabled = checkBox_ABEnableOverclock.Checked;
            Form_Main.OverclockEnabled = checkBox_ABEnableOverclock.Checked;

            if (!Form_Main.GetBTCwalletType().Equals("P2SH"))
            {
                buttonSetupAPI.Enabled = false;
                checkBox_EnableAPI.Enabled = false;
                groupBoxAPIkeys.Enabled = false;
            }
        }

        private void SetZoneTable(int PowerTarif)
        {
            if (PowerTarif == 0)
            {
                textBoxScheduleFrom1.Text = ConfigManager.GeneralConfig.ZoneSchedule1[0].Replace("23:59:59", "24:00");
                textBoxScheduleTo1.Text = ConfigManager.GeneralConfig.ZoneSchedule1[1].Replace("23:59:59", "24:00");
                textBoxScheduleCost1.Text = ConfigManager.GeneralConfig.ZoneSchedule1[2];

                textBoxScheduleFrom2.Visible = false;
                textBoxScheduleFrom3.Visible = false;
                textBoxScheduleFrom4.Visible = false;
                textBoxScheduleFrom5.Visible = false;
                textBoxScheduleTo2.Visible = false;
                textBoxScheduleTo3.Visible = false;
                textBoxScheduleTo4.Visible = false;
                textBoxScheduleTo5.Visible = false;
                textBoxScheduleCost2.Visible = false;
                textBoxScheduleCost3.Visible = false;
                textBoxScheduleCost4.Visible = false;
                textBoxScheduleCost5.Visible = false;

                labelFrom2.Visible = false;
                labelFrom3.Visible = false;
                labelFrom4.Visible = false;
                labelFrom5.Visible = false;
                labelTo2.Visible = false;
                labelTo3.Visible = false;
                labelTo4.Visible = false;
                labelTo5.Visible = false;
                labelCost2.Visible = false;
                labelCost3.Visible = false;
                labelCost4.Visible = false;
                labelCost5.Visible = false;
                labelPowerCurrency2.Visible = false;
                labelPowerCurrency3.Visible = false;
                labelPowerCurrency4.Visible = false;
                labelPowerCurrency5.Visible = false;
            }
            if (PowerTarif == 1)
            {
                textBoxScheduleFrom1.Text = ConfigManager.GeneralConfig.ZoneSchedule2[0].Replace("23:59:59", "24:00");
                textBoxScheduleTo1.Text = ConfigManager.GeneralConfig.ZoneSchedule2[1].Replace("23:59:59", "24:00");
                textBoxScheduleCost1.Text = ConfigManager.GeneralConfig.ZoneSchedule2[2];
                textBoxScheduleFrom2.Text = ConfigManager.GeneralConfig.ZoneSchedule2[3].Replace("23:59:59", "24:00");
                textBoxScheduleTo2.Text = ConfigManager.GeneralConfig.ZoneSchedule2[4].Replace("23:59:59", "24:00");
                textBoxScheduleCost2.Text = ConfigManager.GeneralConfig.ZoneSchedule2[5];

                textBoxScheduleFrom2.Visible = true;
                textBoxScheduleTo2.Visible = true;
                textBoxScheduleCost2.Visible = true;
                labelFrom2.Visible = true;
                labelTo2.Visible = true;
                labelCost2.Visible = true;
                labelPowerCurrency2.Visible = true;

                textBoxScheduleFrom3.Visible = false;
                textBoxScheduleFrom4.Visible = false;
                textBoxScheduleFrom5.Visible = false;
                textBoxScheduleTo3.Visible = false;
                textBoxScheduleTo4.Visible = false;
                textBoxScheduleTo5.Visible = false;
                textBoxScheduleCost3.Visible = false;
                textBoxScheduleCost4.Visible = false;
                textBoxScheduleCost5.Visible = false;

                labelFrom3.Visible = false;
                labelFrom4.Visible = false;
                labelFrom5.Visible = false;
                labelTo3.Visible = false;
                labelTo4.Visible = false;
                labelTo5.Visible = false;
                labelCost3.Visible = false;
                labelCost4.Visible = false;
                labelCost5.Visible = false;
                labelPowerCurrency3.Visible = false;
                labelPowerCurrency4.Visible = false;
                labelPowerCurrency5.Visible = false;
            }
            if (PowerTarif == 2)
            {
                textBoxScheduleFrom1.Text = ConfigManager.GeneralConfig.ZoneSchedule3[0].Replace("23:59:59", "24:00");
                textBoxScheduleTo1.Text = ConfigManager.GeneralConfig.ZoneSchedule3[1].Replace("23:59:59", "24:00");
                textBoxScheduleCost1.Text = ConfigManager.GeneralConfig.ZoneSchedule3[2];
                textBoxScheduleFrom2.Text = ConfigManager.GeneralConfig.ZoneSchedule3[3].Replace("23:59:59", "24:00");
                textBoxScheduleTo2.Text = ConfigManager.GeneralConfig.ZoneSchedule3[4].Replace("23:59:59", "24:00");
                textBoxScheduleCost2.Text = ConfigManager.GeneralConfig.ZoneSchedule3[5];
                textBoxScheduleFrom3.Text = ConfigManager.GeneralConfig.ZoneSchedule3[6].Replace("23:59:59", "24:00");
                textBoxScheduleTo3.Text = ConfigManager.GeneralConfig.ZoneSchedule3[7].Replace("23:59:59", "24:00");
                textBoxScheduleCost3.Text = ConfigManager.GeneralConfig.ZoneSchedule3[8];
                textBoxScheduleFrom4.Text = ConfigManager.GeneralConfig.ZoneSchedule3[9].Replace("23:59:59", "24:00");
                textBoxScheduleTo4.Text = ConfigManager.GeneralConfig.ZoneSchedule3[10].Replace("23:59:59", "24:00");
                textBoxScheduleCost4.Text = ConfigManager.GeneralConfig.ZoneSchedule3[11];
                textBoxScheduleFrom5.Text = ConfigManager.GeneralConfig.ZoneSchedule3[12].Replace("23:59:59", "24:00");
                textBoxScheduleTo5.Text = ConfigManager.GeneralConfig.ZoneSchedule3[13].Replace("23:59:59", "24:00");
                textBoxScheduleCost5.Text = ConfigManager.GeneralConfig.ZoneSchedule3[14];

                textBoxScheduleFrom2.Visible = true;
                textBoxScheduleFrom3.Visible = true;
                textBoxScheduleFrom4.Visible = true;
                textBoxScheduleFrom5.Visible = true;
                textBoxScheduleTo2.Visible = true;
                textBoxScheduleTo3.Visible = true;
                textBoxScheduleTo4.Visible = true;
                textBoxScheduleTo5.Visible = true;
                textBoxScheduleCost2.Visible = true;
                textBoxScheduleCost3.Visible = true;
                textBoxScheduleCost4.Visible = true;
                textBoxScheduleCost5.Visible = true;

                labelFrom2.Visible = true;
                labelFrom3.Visible = true;
                labelFrom4.Visible = true;
                labelFrom5.Visible = true;
                labelTo2.Visible = true;
                labelTo3.Visible = true;
                labelTo4.Visible = true;
                labelTo5.Visible = true;
                labelCost2.Visible = true;
                labelCost3.Visible = true;
                labelCost4.Visible = true;
                labelCost5.Visible = true;
                labelPowerCurrency2.Visible = true;
                labelPowerCurrency3.Visible = true;
                labelPowerCurrency4.Visible = true;
                labelPowerCurrency5.Visible = true;
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
            ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionINTEL = checkBox_DisableDetectionINTEL.Checked;
            ConfigManager.GeneralConfig.DisableMonitoringAMD = checkBoxAMDmonitoring.Checked;
            ConfigManager.GeneralConfig.DisableMonitoringCPU = checkBoxCPUmonitoring.Checked;
            ConfigManager.GeneralConfig.DisableMonitoringNVIDIA = checkBoxNVMonitoring.Checked;
            ConfigManager.GeneralConfig.DisableMonitoringINTEL = checkBoxINTELmonitoring.Checked;
            ConfigManager.GeneralConfig.AutoScaleBTCValues = checkBox_AutoScaleBTCValues.Checked;
            ConfigManager.GeneralConfig.StartMiningWhenIdle = checkBox_StartMiningWhenIdle.Checked;
            //ConfigManager.GeneralConfig.NVIDIAP0State = checkBox_NVIDIAP0State.Checked;
            ConfigManager.GeneralConfig.LogToFile = checkBox_LogToFile.Checked;
            ConfigManager.GeneralConfig.SaveProtocolData = checkBoxSaveProtocolData.Checked;
            ConfigManager.GeneralConfig.AllowMultipleInstances = checkBox_AllowMultipleInstances.Checked;
            ConfigManager.GeneralConfig.MinimizeMiningWindows = checkBox_MinimizeMiningWindows.Checked;
            ConfigManager.GeneralConfig.ShowMinersVersions = checkBoxShowMinersVersions.Checked;
            ConfigManager.GeneralConfig.RestartDriverOnCUDA_GPU_Lost = checkBoxRestartDriver.Checked;
            ConfigManager.GeneralConfig.CheckingCUDA = checkBoxCheckingCUDA.Checked;
            ConfigManager.GeneralConfig.ShowDriverVersionWarning = checkBoxDriverWarning.Checked;
            ConfigManager.GeneralConfig.RestartWindowsOnCUDA_GPU_Lost = checkBoxRestartWindows.Checked;
            ConfigManager.GeneralConfig.EnableAPIkeys = checkBox_EnableAPI.Checked;
            ConfigManager.GeneralConfig.Allow_remote_management = checkBox_Allow_remote_management.Checked;
            ConfigManager.GeneralConfig.Send_actual_version_info = checkBox_Send_actual_version_info.Checked;
            ConfigManager.GeneralConfig.ShowPowerOfDisabledDevices = checkBox_ShowPowerOfDisabledDevices.Checked;
            ConfigManager.GeneralConfig.InstallRootCerts = checkBoxInstall_root_certificates.Checked;
            ConfigManager.GeneralConfig.Force_mining_if_nonprofitable = checkBox_Force_mining_if_nonprofitable.Checked;
            ConfigManager.GeneralConfig.Show_wallet_balance = checkbox_wallet_balance.Checked;
            ConfigManager.GeneralConfig.DecreasePowerCost = checkBox_Show_profit_with_power_consumption.Checked;
            ConfigManager.GeneralConfig.ShowTotalPower = checkBox_Show_Total_Power.Checked;
            ConfigManager.GeneralConfig.FiatCurrency = checkBox_fiat.Checked;
            ConfigManager.GeneralConfig.Additional_info_about_device = checkBox_Additional_info_about_device.Checked;
            ConfigManager.GeneralConfig.Show_displayConected = checkBox_DisplayConnected.Checked;
            ConfigManager.GeneralConfig.Show_NVdevice_manufacturer = checkBox_show_NVdevice_manufacturer.Checked;
            ConfigManager.GeneralConfig.Use_orders_price = checkBox_orderPrice.Checked;
            ConfigManager.GeneralConfig.Use_Last24hours = checkBoxLast24hours.Checked;
            ConfigManager.GeneralConfig.ShortTerm = checkBoxShortTerm.Checked;
            ConfigManager.GeneralConfig.NicehashMiningFee = checkBoxMiningFee.Checked;
            ConfigManager.GeneralConfig.Show_memory_temperature = checkBox_Show_memory_temp.Checked;
            ConfigManager.GeneralConfig.Show_AMDdevice_manufacturer = checkBox_show_AMDdevice_manufacturer.Checked;
            ConfigManager.GeneralConfig.Show_INTELdevice_manufacturer = checkBox_show_INTELdevice_manufacturer.Checked;
            ConfigManager.GeneralConfig.Show_ShowDeviceMemSize = checkBox_ShowDeviceMemSize.Checked;
            //ConfigManager.GeneralConfig.Show_ShowDeviceBusId = checkBox_ShowDeviceBusId.Checked;
            ConfigManager.GeneralConfig.Use_OpenHardwareMonitor = checkbox_Use_OpenHardwareMonitor.Checked;
            if (checkbox_Use_OpenHardwareMonitor.Checked)
            {
                if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                {
                    Form_Main.thisComputer = new LibreHardwareMonitor.Hardware.Computer();
                    Form_Main.thisComputer.IsGpuEnabled = true;
                    Form_Main.thisComputer.IsCpuEnabled = true;
                    Form_Main.thisComputer.Open();
                }
            }

            ConfigManager.GeneralConfig.Save_windows_size_and_position = Checkbox_Save_windows_size_and_position.Checked;
            ConfigManager.GeneralConfig.ColumnSort = checkBox_sorting_list_of_algorithms.Checked;
            ConfigManager.GeneralConfig.DisableTooltips = checkBox_DisableTooltips.Checked;
            ConfigManager.GeneralConfig.ProgramMonitoring = checkBox_program_monitoring.Checked;
            ConfigManager.GeneralConfig.EnableRigRemoteView = checkBoxEnableRigRemoteView.Checked;
            ConfigManager.GeneralConfig.EnableAPI = checkBoxAPI.Checked;
            ConfigManager.GeneralConfig.ShowFanAsPercent = checkBox_ShowFanAsPercent.Checked;
            ConfigManager.GeneralConfig.Group_same_devices = checkbox_Group_same_devices.Checked;
            ConfigManager.GeneralConfig.with_power = checkBox_withPower.Checked;
            ConfigManager.GeneralConfig.By_profitability_of_all_devices = checkBox_By_profitability_of_all_devices.Checked;
            ConfigManager.GeneralConfig.ProgramAutoUpdate = checkBoxAutoupdate.Checked;
            ConfigManager.GeneralConfig.ShowHistory = checkBoxHistory.Checked;
            ConfigManager.GeneralConfig.BackupBeforeUpdate = checkBox_BackupBeforeUpdate.Checked;
            ConfigManager.GeneralConfig.Disable_extra_launch_parameter_checking = checkBox_Disable_extra_launch_parameter_checking.Checked;
            ConfigManager.GeneralConfig.Hide_unused_algorithms = checkBoxHideUnused.Checked;
            ConfigManager.GeneralConfig.Hide_unused_algorithms = checkBoxHideUnused2.Checked;
            //ConfigManager.GeneralConfig.Zilliqua_GMiner = checkBox_Zil_GMiner.Checked;
            /*
            Zil_GMiner = checkBox_Zil_GMiner.Checked;
            if (!checkBox_Zil_GMiner.Checked)
            {
                //ConfigManager.GeneralConfig.ZilFactor = 0.000d;
            }
            */
            ConfigManager.GeneralConfig.ABEnableOverclock = checkBox_ABEnableOverclock.Checked;
            ConfigManager.GeneralConfig.ABDefaultMiningStopped = checkBox_ABDefault_mining_stopped.Checked;
            ConfigManager.GeneralConfig.ABDefaultProgramClosing = checkBox_ABDefault_program_closing.Checked;
            ConfigManager.GeneralConfig.ABMinimize = checkBox_ABMinimize.Checked;
            ConfigManager.GeneralConfig.EnableProxy = checkBoxEnableProxy.Checked;
            ConfigManager.GeneralConfig.ProxySSL = checkBoxProxySSL.Checked;
            //ConfigManager.GeneralConfig.ProxyAsFailover = checkBoxProxyAsFailover.Checked;
            //ConfigManager.GeneralConfig.StaleProxy = checkBoxStale.Checked;
            if (checkBox_LogToFile.Checked)
            {
                textBox_LogMaxFileSize.Enabled = true;
                checkBoxSaveProtocolData.Enabled = true;
            }
            else
            {
                textBox_LogMaxFileSize.Enabled = false;
                checkBoxSaveProtocolData.Enabled = false;
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
                startVal = (string)runKey.GetValue(Application.ProductName);
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
            ConfigManager.GeneralConfig.RigAPiPort = Helpers.ParseInt(textBoxAPIport.Text);
            ConfigManager.GeneralConfig.LogMaxFileSize = Helpers.ParseLong(textBox_LogMaxFileSize.Text);
            ConfigManager.GeneralConfig.AutoStartMiningDelay = Helpers.ParseInt(textBox_AutoStartMiningDelay.Text);
            // min profit
            ConfigManager.GeneralConfig.MinimumProfit = Helpers.ParseDouble(textBox_MinProfit.Text);
            ConfigManager.GeneralConfig.SwitchProfitabilityThreshold =
                Helpers.ParseDouble(textBox_SwitchProfitabilityThreshold.Text) / 100;

            ConfigManager.GeneralConfig.PowerMB = Helpers.ParseInt(textBox_mb.Text);
            ConfigManager.GeneralConfig.PowerAddAMD = Helpers.ParseInt(textBoxAddAMD.Text);
            ConfigManager.GeneralConfig.PowerPSU = Helpers.ParseInt(textBox_psu.Text);

            ConfigManager.GeneralConfig.ZoneSchedule1 = Form_Main.ZoneSchedule1;
            ConfigManager.GeneralConfig.ZoneSchedule2 = Form_Main.ZoneSchedule2;
            ConfigManager.GeneralConfig.ZoneSchedule3 = Form_Main.ZoneSchedule3;

            // Fix bounds
            ConfigManager.GeneralConfig.FixSettingBounds();
            // update strings
            textBox_MinProfit.Text =
                ConfigManager.GeneralConfig.MinimumProfit.ToString("F2").Replace(',', '.'); // force comma
            textBox_SwitchProfitabilityThreshold.Text = (ConfigManager.GeneralConfig.SwitchProfitabilityThreshold * 100)
                .ToString("F1").Replace(',', '.'); // force comma
            textBox_MinIdleSeconds.Text = ConfigManager.GeneralConfig.MinIdleSeconds.ToString();
            textBoxAPIport.Text = ConfigManager.GeneralConfig.RigAPiPort.ToString();
            textBox_LogMaxFileSize.Text = ConfigManager.GeneralConfig.LogMaxFileSize.ToString();
            textBox_AutoStartMiningDelay.Text = ConfigManager.GeneralConfig.AutoStartMiningDelay.ToString();
            textBox_psu.Text = ConfigManager.GeneralConfig.PowerPSU.ToString("");
            textBox_mb.Text = ConfigManager.GeneralConfig.PowerMB.ToString("");
            textBoxAddAMD.Text = ConfigManager.GeneralConfig.PowerAddAMD.ToString("");
        }

        private void GeneralComboBoxes_Leave(object sender, EventArgs e)
        {
            if (!_isInitFinished) return;
            IsChange = true;
            ConfigManager.GeneralConfig.Language = (LanguageType)comboBox_Language.SelectedIndex;
            ConfigManager.GeneralConfig.ColorProfileIndex = comboBox_ColorProfile.SelectedIndex;
            ConfigManager.GeneralConfig.SwitchingAlgorithmsIndex = comboBox_switching_algorithms.SelectedIndex;
            ConfigManager.GeneralConfig.DevicesCountIndex = comboBox_devices_count.SelectedIndex;
            ConfigManager.GeneralConfig.ProgramUpdateIndex = comboBoxCheckforprogramupdatesevery.SelectedIndex;
            ConfigManager.GeneralConfig.ProgramRestartIndex = comboBoxRestartProgram.SelectedIndex;
            ConfigManager.GeneralConfig.TimeUnit = (TimeUnitType)comboBox_TimeUnit.SelectedIndex;
            ConfigManager.GeneralConfig.PowerTarif = comboBoxZones.SelectedIndex;
        }

        private void ComboBox_CPU0_ForceCPUExtension_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmbbox = (ComboBox)sender;
            ConfigManager.GeneralConfig.ForceCPUExtension = (CpuExtensionType)cmbbox.SelectedIndex;
        }

        #endregion //Tab General


        #region Tab Device

        private void DevicesListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift || (ModifierKeys == (Keys.Control | Keys.Shift)))
            {
                return;
            }
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
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift || (ModifierKeys == (Keys.Control | Keys.Shift)))
            {
                return;
            }
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

        private void ButtonGPUtuning_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("GPU-Tuning.exe");
        }

        #endregion //Tab Device


        private void ToolTip1_Popup(object sender, PopupEventArgs e)
        {
            if (!ConfigManager.GeneralConfig.DisableTooltips)
            {
                toolTip1.ToolTipTitle = International.GetText("Form_Settings_ToolTip_Explaination");
            }
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
            if (!ForceClosingForm)
            {
                MessageBox.Show(International.GetText("Form_Settings_buttonSaveMsg"),
                    International.GetText("Form_Settings_buttonSaveTitle"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            IsChange = true;
            IsChangeSaved = true;
            new Task(() => NiceHashStats.GetRigProfit()).Start();
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
            Close();
            new Task(() => NiceHashStats.SetDeviceStatus(null, true, "ButtonSaveClose_Click")).Start();
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
            if (!ForceClosingForm)
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
            } else
            {
                ConfigManager.GeneralConfigFileCommit();
                ConfigManager.CommitBenchmarks();
                ForceClosingForm = false;
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

        private void textBox_AutoStartMiningDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }
        private void comboBox_TimeUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
        }


        private void comboBox_ColorProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Form_Settings_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush fillBrush = new SolidBrush(Form_Main._backColor);
            e.Graphics.FillRectangle(fillBrush, e.ClipRectangle);
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

        private void checkBox_RunEthlargement_CheckedChanged(object sender, EventArgs e)
        {
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
            Form_Main.githubVersion = Updater.Updater.GetGITHUBVersion();
            Form_Main.gitlabVersion = Updater.Updater.GetGITLABVersion();

            //Form_Main.githubBuild = Updater.Updater.GetVersion().Item2;

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
            if (Form_Main.currentVersion < Form_Main.gitlabVersion)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Newversion") + Form_Main.gitlabVersion.ToString();
                buttonUpdate.Visible = true;
            }
            if (Form_Main.githubVersion <= 0 && Form_Main.gitlabVersion <= 0)
            {
                linkLabelNewVersion2.Text = International.GetText("Form_Settings_Errorwhencheckingnewversion");
                buttonUpdate.Visible = false;
            }
            linkLabelNewVersion2.Update();
        }

        private void linkLabelNewVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Form_Main.githubVersion > 0)
            {
                System.Diagnostics.Process.Start(Links.githubReleases);
            }
            if (Form_Main.githubVersion <= 0 && Form_Main.gitlabVersion > 0)
            {
                System.Diagnostics.Process.Start(Links.gitlabReleases);
            }
        }

        private void linkLabelCurrentVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Links.githubReleases);
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

        private void checkBox_AlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void buttonRestoreBackup_Click(object sender, EventArgs e)
        {
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

             CMDconfigHandleOHM = new Process

                {
                    StartInfo =
                {
                    FileName = "sc.exe"
                }
                };

                CMDconfigHandleOHM.StartInfo.Arguments = "stop R0NiceHashMinerLegacy";
                CMDconfigHandleOHM.StartInfo.UseShellExecute = false;
                CMDconfigHandleOHM.StartInfo.CreateNoWindow = true;
                CMDconfigHandleOHM.Start();

            CMDconfigHandleOHM = new Process

            {
                StartInfo =
                {
                    FileName = "sc.exe"
                }
            };

            CMDconfigHandleOHM.StartInfo.Arguments = "delete R0NiceHashMinerLegacy";
            CMDconfigHandleOHM.StartInfo.UseShellExecute = false;
            CMDconfigHandleOHM.StartInfo.CreateNoWindow = true;
            CMDconfigHandleOHM.Start();


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
            Process.Start(Links.StatusNicehash);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*
            var ps = Miner.PingServers();
            if (ps < 0)
            {
                richTextBoxCheckNiceHashservers.Text += "NiceHash speedtest servers down. Try alternate method\n";
                Miner.PingServers("daggerhashimoto");
                for (int i = 0; i < 4; i++)
                {
                    var server = "daggerhashimoto." + Globals.MiningLocation[i];
                    richTextBoxCheckNiceHashservers.Text += server + " ping: " + Form_Main.myServers[i, 1] + " ms\n";
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    var server = "speedtest." + Globals.MiningLocation[i];
                    richTextBoxCheckNiceHashservers.Text += server + " ping: " + Form_Main.myServers[i, 1] + " ms\n";
                }
            }
            */
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
                checkBoxCheckingCUDA.Enabled = false;
                checkBox_show_NVdevice_manufacturer.Enabled = false;
            }
            else
            {
                checkBoxNVMonitoring.Enabled = true;
                checkBoxRestartWindows.Enabled = true;
                checkBoxRestartDriver.Enabled = true;
                checkBoxCheckingCUDA.Enabled = true;
                checkBox_show_NVdevice_manufacturer.Enabled = true;
            }
        }

        private void checkBox_DisableDetectionAMD_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DisableDetectionAMD.Checked)
            {
                checkBoxAMDmonitoring.Enabled = false;
                checkBox_show_AMDdevice_manufacturer.Enabled = false;
                if (checkBox_DisableDetectionCPU.Checked)
                {
                    checkbox_Use_OpenHardwareMonitor.Enabled = false;
                }
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
                if (checkBox_DisableDetectionAMD.Checked)
                {
                    checkbox_Use_OpenHardwareMonitor.Enabled = false;
                }
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
            checkBox_ABMinimize.Enabled = checkBox_ABEnableOverclock.Checked;
            checkBox_ABDefault_mining_stopped.Enabled = checkBox_ABEnableOverclock.Checked;
            checkBox_ABDefault_program_closing.Enabled = checkBox_ABEnableOverclock.Checked;

            var oc = tabPageOverClock;
            //tabControlGeneral.TabPages.Remove(oc);
            Form_Main.OverclockEnabled = checkBox_ABEnableOverclock.Checked;
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

                checkBox_ABEnableOverclock.Text = International.GetText("FormSettings_ABEnableOverclock");
                checkBox_ABEnableOverclock.Update();
                if (!MSIAfterburner.MSIAfterburnerInit())
                {
                    MessageBox.Show(International.GetText("FormSettings_AB_Error"), "MSI Afterburner error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
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

        private void linkLabel3_Click(object sender, EventArgs e)
        {
            Process.Start("Help\\How to use MSI Afterburner for overclocking.avi");
        }

        private void currencyConverterCombobox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.KwhPrice > 0)
            {
                MessageBox.Show(International.GetText("FormSettings_changePowerCost"), "Next");
            }
        }

        private void currencyConverterCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = currencyConverterCombobox.SelectedItem.ToString();
            ConfigManager.GeneralConfig.DisplayCurrency = selected;
        }

        private void checkBox_orderPrice_CheckedChanged(object sender, EventArgs e)
        {
            if (!ConfigManager.GeneralConfig.Use_orders_price && checkBox_orderPrice.Checked)
            {
                MessageBox.Show(International.GetText("Form_Settings_checkBox_orderPrice_Warning1"),
        International.GetText("Warning_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (checkBox_orderPrice.Checked) checkBoxLast24hours.Checked = false;
            checkBoxLast24hours.Enabled = !checkBox_orderPrice.Checked;
            ConfigManager.GeneralConfig.Use_Last24hours = false;
        }

        private void buttonSetupAPI_Click(object sender, EventArgs e)
        {
            Form ifrmAPI = new Form_API_keys();
            ifrmAPI.ShowDialog();
        }

        private void checkBox_EnableAPI_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_EnableAPI.Checked)
            {
                if (!Form_API_keys.GetSavedAPIkeyData())
                {
                    MessageBox.Show(International.GetText("Form_Settings_firstAPI") + "\n" +
                        Form_Main.lastRigProfit.Message);
                    checkBox_EnableAPI.Checked = false;
                    //Form_Main.checkBox_EnableAPI = false;
                } else
                {
                    if (!NiceHashStats.GetRigProfitInternalRUN(true))
                    {
                        MessageBox.Show(Form_Main.errorAPIkeystring,
                        International.GetText("Warning_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Form_Main.checkBox_EnableAPI = true;
                    }
                }
                buttonSetupAPI.Enabled = true;
            } else
            {
                Form_Main.NicehashAPIerrorDescription = "";

                if (Form_API_keys.GetSavedAPIkeyData())
                {
                    MessageBox.Show(International.GetText("Form_Settings_chart_disabled"),
                    International.GetText("Warning_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ConfigManager.GeneralConfig.ChartEnable = false;
                    Form_Main.checkBox_EnableAPI = false;
                    Form_Main.Form_RigProfitChartRunning = false;
                }
                buttonSetupAPI.Enabled = false;
            }
        }

        private void checkbox_wallet_balance_Click(object sender, EventArgs e)
        {
            if (Form_Main.walletType.Equals("P2SH") && checkbox_wallet_balance.Checked)
            {
                MessageBox.Show(International.GetText("Form_Settings_Only_external_wallet"),
                                    International.GetText("Warning_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                checkbox_wallet_balance.Checked = false;
            }
        }

        private void checkBoxEnableProxy_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.EnableProxy = checkBoxEnableProxy.Checked;
            if (checkBoxEnableProxy.Checked)
            {
                checkBoxProxySSL.Enabled = true;
                //checkBoxProxyAsFailover.Enabled = true;
                //checkBoxStale.Enabled = true;
            } else
            {
                checkBoxProxySSL.Enabled = false;
                //checkBoxProxyAsFailover.Enabled = false;
                //checkBoxStale.Enabled = false;
            }
        }
        public static string GetLocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                localIP = ip.ToString();
                string[] temp = localIP.Split('.');
                if (ip.AddressFamily == AddressFamily.InterNetwork && (temp[0] == "192") || temp[0] == "10" || temp[0] == "172")
                {
                    break;
                }
                else
                {
                    localIP = null;
                }
            }
            return localIP;
        }
        private void checkBoxEnableRigRemoteView_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnableRigRemoteView.Checked)
            {
                string ip = GetLocalIPAddress();
                if (!string.IsNullOrEmpty(ip))
                {
                    linkLabelRigRemoteView.Text = "http://" + ip + ":" + ConfigManager.GeneralConfig.RigRemoteViewPort.ToString();
                }
                linkLabelRigRemoteView.Visible = true;
                new Task(() => NiceHashServer.Listener(true)).Start();
            } else
            {
                linkLabelRigRemoteView.Visible = false;
                new Task(() => NiceHashServer.Listener(false)).Start();
            }
        }

        private void linkLabelRigRemoteView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string ip = GetLocalIPAddress();
            string link = "";
            if (!string.IsNullOrEmpty(ip))
            {
                link = "http://" + ip + ":" + ConfigManager.GeneralConfig.RigRemoteViewPort.ToString();
                System.Diagnostics.Process.Start(link);
            }
        }

        private void comboBoxZones_DrawItem(object sender, DrawItemEventArgs e)
        {
            comboBox_ServiceLocation_DrawItem(sender, e);
        }

        private void comboBoxZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetZoneTable(comboBoxZones.SelectedIndex);
        }

        private void SaveSchedules()
        {
            if (comboBoxZones.SelectedIndex == 0)
            {
                string _textBoxScheduleFrom1 = textBoxScheduleFrom1.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleTo1 = textBoxScheduleTo1.Text.Replace("24:00", "23:59:59");

                try
                {
                    Form_Main.ZoneSchedule1[0] = DateTime.Parse(_textBoxScheduleFrom1).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleFrom1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleFrom1.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule1[1] = DateTime.Parse(_textBoxScheduleTo1).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleTo1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleTo1.Focus();
                }

                try
                {
                    Form_Main.ZoneSchedule1[2] = double.Parse(textBoxScheduleCost1.Text).ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format error: " + textBoxScheduleCost1.Text,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBoxScheduleCost1.Focus();
                }
            }

            if (comboBoxZones.SelectedIndex == 1)
            {
                string _textBoxScheduleFrom1 = textBoxScheduleFrom1.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleTo1 = textBoxScheduleTo1.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleFrom2 = textBoxScheduleFrom2.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleTo2 = textBoxScheduleTo2.Text.Replace("24:00", "23:59:59");
                //1
                try
                {
                    Form_Main.ZoneSchedule2[0] = DateTime.Parse(_textBoxScheduleFrom1).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleFrom1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleFrom1.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule2[1] = DateTime.Parse(_textBoxScheduleTo1).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleTo1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleTo1.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule2[2] = double.Parse(textBoxScheduleCost1.Text).ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format error: " + textBoxScheduleCost1.Text,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBoxScheduleCost1.Focus();
                }
                //2
                try
                {
                    Form_Main.ZoneSchedule2[3] = DateTime.Parse(_textBoxScheduleFrom2).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleFrom2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleFrom2.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule2[4] = DateTime.Parse(_textBoxScheduleTo2).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleTo2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleTo2.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule2[5] = double.Parse(textBoxScheduleCost2.Text).ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format error: " + textBoxScheduleCost2.Text,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBoxScheduleCost2.Focus();
                }
            }

            if (comboBoxZones.SelectedIndex == 2)
            {
                string _textBoxScheduleFrom1 = textBoxScheduleFrom1.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleTo1 = textBoxScheduleTo1.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleFrom2 = textBoxScheduleFrom2.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleTo2 = textBoxScheduleTo2.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleFrom3 = textBoxScheduleFrom3.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleTo3 = textBoxScheduleTo3.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleFrom4 = textBoxScheduleFrom4.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleTo4 = textBoxScheduleTo4.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleFrom5 = textBoxScheduleFrom5.Text.Replace("24:00", "23:59:59");
                string _textBoxScheduleTo5 = textBoxScheduleTo5.Text.Replace("24:00", "23:59:59");
                //1
                try
                {
                    Form_Main.ZoneSchedule3[0] = DateTime.Parse(_textBoxScheduleFrom1).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleFrom1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleFrom1.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[1] = DateTime.Parse(_textBoxScheduleTo1).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleTo1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleTo1.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[2] = double.Parse(textBoxScheduleCost1.Text).ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format error: " + textBoxScheduleCost1.Text,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBoxScheduleCost1.Focus();
                }
                //2
                try
                {
                    Form_Main.ZoneSchedule3[3] = DateTime.Parse(_textBoxScheduleFrom2).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleFrom2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleFrom2.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[4] = DateTime.Parse(_textBoxScheduleTo2).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleTo2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleTo2.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[5] = double.Parse(textBoxScheduleCost2.Text).ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format error: " + textBoxScheduleCost2.Text,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBoxScheduleCost2.Focus();
                }
                //3
                try
                {
                    Form_Main.ZoneSchedule3[6] = DateTime.Parse(_textBoxScheduleFrom3).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleFrom3.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleFrom3.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[7] = DateTime.Parse(_textBoxScheduleTo3).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleTo3.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleTo3.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[8] = double.Parse(textBoxScheduleCost3.Text).ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format error: " + textBoxScheduleCost3.Text,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBoxScheduleCost3.Focus();
                }
                //4
                try
                {
                    Form_Main.ZoneSchedule3[9] = DateTime.Parse(_textBoxScheduleFrom4).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleFrom4.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleFrom4.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[10] = DateTime.Parse(_textBoxScheduleTo4).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleTo4.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleTo4.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[11] = double.Parse(textBoxScheduleCost4.Text).ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format error: " + textBoxScheduleCost4.Text,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBoxScheduleCost4.Focus();
                }
                //5
                try
                {
                    Form_Main.ZoneSchedule3[12] = DateTime.Parse(_textBoxScheduleFrom5).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleFrom5.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleFrom5.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[13] = DateTime.Parse(_textBoxScheduleTo5).ToString("T");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Time format error: " + textBoxScheduleTo5.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxScheduleTo5.Focus();
                }
                try
                {
                    Form_Main.ZoneSchedule3[14] = double.Parse(textBoxScheduleCost5.Text).ToString();
                }
                catch (FormatException)
                {
                    MessageBox.Show("Format error: " + textBoxScheduleCost5.Text,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBoxScheduleCost5.Focus();
                }
            }
        }

        private void textBoxScheduleFrom1_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleTo1_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleCost1_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleFrom2_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleTo2_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleFrom3_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleTo3_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleFrom4_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleTo4_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleFrom5_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleTo5_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleCost2_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleCost3_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleCost4_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void textBoxScheduleCost5_Leave(object sender, EventArgs e)
        {
            SaveSchedules();
        }

        private void checkBoxShortTerm_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShortTerm.Checked)
            {
                comboBox_switching_algorithms.Enabled = false;
            } else
            {
                comboBox_switching_algorithms.Enabled = true;
            }
        }

        private void checkBoxAPI_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAPI.Checked)
            {
                textBoxAPIport.Enabled = false;
                new Task(() => NiceHashAPIServer.Listener(true)).Start();
            } else
            {
                textBoxAPIport.Enabled = true;
                new Task(() => NiceHashAPIServer.Listener(false)).Start();
            }
        }

        private void linkLabelGetAPIkey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var notepad = new Process
            {
                StartInfo = { FileName = "notepad.exe" }
            };

            notepad.StartInfo.Arguments = "Help\\API.txt";
            notepad.Start();
        }

        private void checkBoxShowMinersVersions_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.ShowMinersVersions = checkBoxShowMinersVersions.Checked;
        }

        private void checkBox_Disable_extra_launch_parameter_checking_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.Disable_extra_launch_parameter_checking = checkBox_Disable_extra_launch_parameter_checking.Checked;
        }

        private void checkBoxHideUnused_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.Hide_unused_algorithms = checkBoxHideUnused.Checked;
            checkBoxHideUnused2.Checked = checkBoxHideUnused.Checked;
            try
            {
                if (_selectedComputeDevice == null) return;
                algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
            } catch (Exception ex)
            {

            }
        }

        private void checkBoxHideUnused2_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.Hide_unused_algorithms = checkBoxHideUnused2.Checked;
            checkBoxHideUnused.Checked = checkBoxHideUnused2.Checked;
            try
            {
                if (_selectedComputeDevice == null) return;
                algorithmsListViewOverClock1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
            }
            catch (Exception ex)
            {

            }
        }

        private void tabControlGeneral_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_selectedComputeDevice == null) return;
                algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
                algorithmsListViewOverClock1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
            }
            catch (Exception ex)
            {

            }
        }

        private void button_ZIL_additional_mining_Click(object sender, EventArgs e)
        {
            var settings = new Form_additional_mining();
            try
            {
                //   SetChildFormCenter(settings);
                settings.ShowDialog();
                algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("settings", er.ToString());
            }
            //Form_additional_mining.ActiveForm.ShowDialog();
        }

        private void checkBoxLast24hours_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLast24hours.Checked) checkBox_orderPrice.Checked = false;
            checkBox_orderPrice.Enabled = !checkBoxLast24hours.Checked;
            ConfigManager.GeneralConfig.Use_orders_price = false;
        }

        private void checkBox_DisableDetectionINTEL_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_DisableDetectionINTEL.Checked)
            {
                checkBoxINTELmonitoring.Enabled = false;
                checkBox_show_INTELdevice_manufacturer.Enabled = false;
            }
            else
            {
                checkBoxINTELmonitoring.Enabled = true;
                checkBox_show_INTELdevice_manufacturer.Enabled = true;
            }
        }

        private void checkBox_show_INTELdevice_manufacturer_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonHistory_Click(object sender, EventArgs e)
        {
            new Task(() => Updater.Updater.ShowHistory(true)).Start();
        }

        private void button_Lite_Algo_Click(object sender, EventArgs e)
        {
            int dh = Divert.CheckWinDivert();
            if (dh <= 0)
            {
                MessageBox.Show(International.GetText("Form_Settings_WinDivertError") + ". Error: " + dh.ToString(),
                                International.GetText("Error_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Form_Main.KawpowLite)
            {
                MessageBox.Show(International.GetText("Form_Settings_LiteNoSuitableGPUs"),
                                International.GetText("Form_Settings_groupBoxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var lite = new Form_Lite_Algo();
            try
            {
                lite.ShowDialog();
                algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("lite", er.ToString());
            }
        }
    }

}
