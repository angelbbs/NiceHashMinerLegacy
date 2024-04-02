namespace NiceHashMiner.Forms
{
    partial class Form_Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonSaveClose = new System.Windows.Forms.Button();
            this.buttonDefaults = new System.Windows.Forms.Button();
            this.buttonCloseNoSave = new System.Windows.Forms.Button();
            this.tabControlGeneral = new System.Windows.Forms.CustomTabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.groupBox_RigManager = new System.Windows.Forms.GroupBox();
            this.checkBox_ShowPowerOfDisabledDevices = new System.Windows.Forms.CheckBox();
            this.checkBox_Allow_remote_management = new System.Windows.Forms.CheckBox();
            this.checkBox_Send_actual_version_info = new System.Windows.Forms.CheckBox();
            this.groupBoxServer = new System.Windows.Forms.GroupBox();
            this.linkLabelGetAPIkey = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAPIport = new System.Windows.Forms.TextBox();
            this.checkBoxAPI = new System.Windows.Forms.CheckBox();
            this.linkLabelRigRemoteView = new System.Windows.Forms.LinkLabel();
            this.checkBoxEnableRigRemoteView = new System.Windows.Forms.CheckBox();
            this.groupBoxAPIkeys = new System.Windows.Forms.GroupBox();
            this.buttonSetupAPI = new System.Windows.Forms.Button();
            this.checkBox_EnableAPI = new System.Windows.Forms.CheckBox();
            this.groupBox_Idle = new System.Windows.Forms.GroupBox();
            this.checkBox_StartMiningWhenIdle = new System.Windows.Forms.CheckBox();
            this.label_MinIdleSeconds = new System.Windows.Forms.Label();
            this.textBox_MinIdleSeconds = new System.Windows.Forms.TextBox();
            this.groupBox_Main = new System.Windows.Forms.GroupBox();
            this.checkbox_wallet_balance = new System.Windows.Forms.CheckBox();
            this.checkBox_fiat = new System.Windows.Forms.CheckBox();
            this.checkBox_Force_mining_if_nonprofitable = new System.Windows.Forms.CheckBox();
            this.label_TimeUnit = new System.Windows.Forms.Label();
            this.comboBox_TimeUnit = new System.Windows.Forms.ComboBox();
            this.textBox_MinProfit = new System.Windows.Forms.TextBox();
            this.label_MinProfit = new System.Windows.Forms.Label();
            this.checkBox_AutoScaleBTCValues = new System.Windows.Forms.CheckBox();
            this.groupBox_Misc = new System.Windows.Forms.GroupBox();
            this.checkBoxInstall_root_certificates = new System.Windows.Forms.CheckBox();
            this.checkBox_DisableTooltips = new System.Windows.Forms.CheckBox();
            this.labelRestartProgram = new System.Windows.Forms.Label();
            this.checkBoxShowMinersVersions = new System.Windows.Forms.CheckBox();
            this.comboBoxRestartProgram = new System.Windows.Forms.ComboBox();
            this.checkBox_program_monitoring = new System.Windows.Forms.CheckBox();
            this.checkBox_AlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.checkBox_sorting_list_of_algorithms = new System.Windows.Forms.CheckBox();
            this.Checkbox_Save_windows_size_and_position = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_AutoStartMiningDelay = new System.Windows.Forms.TextBox();
            this.label_AutoStartMiningDelay = new System.Windows.Forms.Label();
            this.checkBox_AllowMultipleInstances = new System.Windows.Forms.CheckBox();
            this.comboBox_ColorProfile = new System.Windows.Forms.ComboBox();
            this.checkBox_MinimizeMiningWindows = new System.Windows.Forms.CheckBox();
            this.checkBox_RunAtStartup = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoStartMining = new System.Windows.Forms.CheckBox();
            this.checkBox_HideMiningWindows = new System.Windows.Forms.CheckBox();
            this.checkBox_MinimizeToTray = new System.Windows.Forms.CheckBox();
            this.groupBox_Logging = new System.Windows.Forms.GroupBox();
            this.checkBoxSaveProtocolData = new System.Windows.Forms.CheckBox();
            this.label_LogMaxFileSize = new System.Windows.Forms.Label();
            this.textBox_LogMaxFileSize = new System.Windows.Forms.TextBox();
            this.checkBox_LogToFile = new System.Windows.Forms.CheckBox();
            this.groupBox_Localization = new System.Windows.Forms.GroupBox();
            this.label_Language = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.comboBox_Language = new System.Windows.Forms.ComboBox();
            this.currencyConverterCombobox = new System.Windows.Forms.ComboBox();
            this.label_displayCurrency = new System.Windows.Forms.Label();
            this.tabPagePower = new System.Windows.Forms.TabPage();
            this.groupBox_additionally = new System.Windows.Forms.GroupBox();
            this.checkBox_Show_Total_Power = new System.Windows.Forms.CheckBox();
            this.label_psu = new System.Windows.Forms.Label();
            this.checkBox_Show_profit_with_power_consumption = new System.Windows.Forms.CheckBox();
            this.textBox_psu = new System.Windows.Forms.TextBox();
            this.label_MBpower = new System.Windows.Forms.Label();
            this.labelAddAMD = new System.Windows.Forms.Label();
            this.textBox_mb = new System.Windows.Forms.TextBox();
            this.textBoxAddAMD = new System.Windows.Forms.TextBox();
            this.groupBoxTariffs = new System.Windows.Forms.GroupBox();
            this.labelTo5 = new System.Windows.Forms.Label();
            this.labelTo4 = new System.Windows.Forms.Label();
            this.labelTo3 = new System.Windows.Forms.Label();
            this.labelTo2 = new System.Windows.Forms.Label();
            this.labelPowerCurrency5 = new System.Windows.Forms.Label();
            this.textBoxScheduleTo5 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleFrom5 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleCost5 = new System.Windows.Forms.TextBox();
            this.labelCost5 = new System.Windows.Forms.Label();
            this.labelFrom5 = new System.Windows.Forms.Label();
            this.labelPowerCurrency4 = new System.Windows.Forms.Label();
            this.textBoxScheduleTo4 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleFrom4 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleCost4 = new System.Windows.Forms.TextBox();
            this.labelCost4 = new System.Windows.Forms.Label();
            this.labelFrom4 = new System.Windows.Forms.Label();
            this.labelPowerCurrency3 = new System.Windows.Forms.Label();
            this.textBoxScheduleTo3 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleFrom3 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleCost3 = new System.Windows.Forms.TextBox();
            this.labelCost3 = new System.Windows.Forms.Label();
            this.labelFrom3 = new System.Windows.Forms.Label();
            this.labelPowerCurrency2 = new System.Windows.Forms.Label();
            this.textBoxScheduleTo2 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleFrom2 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleCost2 = new System.Windows.Forms.TextBox();
            this.labelCost2 = new System.Windows.Forms.Label();
            this.labelFrom2 = new System.Windows.Forms.Label();
            this.labelPowerCurrency1 = new System.Windows.Forms.Label();
            this.textBoxScheduleTo1 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleFrom1 = new System.Windows.Forms.MaskedTextBox();
            this.textBoxScheduleCost1 = new System.Windows.Forms.TextBox();
            this.labelCost1 = new System.Windows.Forms.Label();
            this.labelTo1 = new System.Windows.Forms.Label();
            this.labelFrom1 = new System.Windows.Forms.Label();
            this.label_Schedules2 = new System.Windows.Forms.Label();
            this.label_Schedules = new System.Windows.Forms.Label();
            this.comboBoxZones = new System.Windows.Forms.ComboBox();
            this.tabPageAdvanced1 = new System.Windows.Forms.TabPage();
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.checkBoxEnableProxy = new System.Windows.Forms.CheckBox();
            this.checkBoxProxySSL = new System.Windows.Forms.CheckBox();
            this.groupBox_Miners = new System.Windows.Forms.GroupBox();
            this.checkBoxMiningFee = new System.Windows.Forms.CheckBox();
            this.checkBoxShortTerm = new System.Windows.Forms.CheckBox();
            this.checkBoxLast24hours = new System.Windows.Forms.CheckBox();
            this.checkBox_orderPrice = new System.Windows.Forms.CheckBox();
            this.checkBox_withPower = new System.Windows.Forms.CheckBox();
            this.checkBox_By_profitability_of_all_devices = new System.Windows.Forms.CheckBox();
            this.label_switching_algorithms = new System.Windows.Forms.Label();
            this.comboBox_switching_algorithms = new System.Windows.Forms.ComboBox();
            this.textBox_SwitchProfitabilityThreshold = new System.Windows.Forms.TextBox();
            this.label_SwitchProfitabilityThreshold = new System.Windows.Forms.Label();
            this.checkbox_Group_same_devices = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxINTELmonitoring = new System.Windows.Forms.CheckBox();
            this.labelDisableMonitoring = new System.Windows.Forms.Label();
            this.checkBox_DisableDetectionINTEL = new System.Windows.Forms.CheckBox();
            this.labelDisableDetection = new System.Windows.Forms.Label();
            this.checkBoxAMDmonitoring = new System.Windows.Forms.CheckBox();
            this.checkBoxNVMonitoring = new System.Windows.Forms.CheckBox();
            this.checkBox_show_INTELdevice_manufacturer = new System.Windows.Forms.CheckBox();
            this.checkBox_Show_memory_temp = new System.Windows.Forms.CheckBox();
            this.checkBox_DisableDetectionNVIDIA = new System.Windows.Forms.CheckBox();
            this.label_restart_nv_lost = new System.Windows.Forms.Label();
            this.label_show_manufacturer = new System.Windows.Forms.Label();
            this.checkBoxCheckingCUDA = new System.Windows.Forms.CheckBox();
            this.checkBox_DisplayConnected = new System.Windows.Forms.CheckBox();
            this.checkBox_show_AMDdevice_manufacturer = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowDeviceMemSize = new System.Windows.Forms.CheckBox();
            this.checkBox_show_NVdevice_manufacturer = new System.Windows.Forms.CheckBox();
            this.checkBoxDriverWarning = new System.Windows.Forms.CheckBox();
            this.checkBoxCPUmonitoring = new System.Windows.Forms.CheckBox();
            this.checkBoxRestartDriver = new System.Windows.Forms.CheckBox();
            this.checkBoxRestartWindows = new System.Windows.Forms.CheckBox();
            this.checkbox_Use_OpenHardwareMonitor = new System.Windows.Forms.CheckBox();
            this.label_devices_count = new System.Windows.Forms.Label();
            this.comboBox_devices_count = new System.Windows.Forms.ComboBox();
            this.checkBox_ShowFanAsPercent = new System.Windows.Forms.CheckBox();
            this.checkBox_DisableDetectionCPU = new System.Windows.Forms.CheckBox();
            this.checkBox_Additional_info_about_device = new System.Windows.Forms.CheckBox();
            this.checkBox_DisableDetectionAMD = new System.Windows.Forms.CheckBox();
            this.tabPageDevicesAlgos = new System.Windows.Forms.TabPage();
            this.button_Lite_Algo = new System.Windows.Forms.Button();
            this.button_ZIL_additional_mining = new System.Windows.Forms.Button();
            this.checkBoxHideUnused = new System.Windows.Forms.CheckBox();
            this.checkBox_Disable_extra_launch_parameter_checking = new System.Windows.Forms.CheckBox();
            this.groupBoxAlgorithmSettings = new System.Windows.Forms.GroupBox();
            this.algorithmsListView1 = new NiceHashMiner.Forms.Components.AlgorithmsListView();
            this.algorithmSettingsControl1 = new NiceHashMiner.Forms.Components.AlgorithmSettingsControl();
            this.devicesListViewEnableControl1 = new NiceHashMiner.Forms.Components.DevicesListViewEnableControl();
            this.tabPageOverClock = new System.Windows.Forms.TabPage();
            this.checkBoxHideUnused2 = new System.Windows.Forms.CheckBox();
            this.checkBox_ABDefault_program_closing = new System.Windows.Forms.CheckBox();
            this.checkBox_ABDefault_mining_stopped = new System.Windows.Forms.CheckBox();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.checkBox_ABMinimize = new System.Windows.Forms.CheckBox();
            this.checkBox_ABEnableOverclock = new System.Windows.Forms.CheckBox();
            this.groupBoxOverClockSettings = new System.Windows.Forms.GroupBox();
            this.algorithmsListViewOverClock1 = new NiceHashMiner.Forms.Components.AlgorithmsListViewOverClock();
            this.devicesListViewEnableControl2 = new NiceHashMiner.Forms.Components.DevicesListViewEnableControl();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.groupBoxBackup = new System.Windows.Forms.GroupBox();
            this.checkBox_BackupBeforeUpdate = new System.Windows.Forms.CheckBox();
            this.labelBackupCopy = new System.Windows.Forms.Label();
            this.buttonRestoreBackup = new System.Windows.Forms.Button();
            this.buttonCreateBackup = new System.Windows.Forms.Button();
            this.groupBoxUpdates = new System.Windows.Forms.GroupBox();
            this.checkBoxAutoupdate = new System.Windows.Forms.CheckBox();
            this.labelCheckforprogramupdatesevery = new System.Windows.Forms.Label();
            this.comboBoxCheckforprogramupdatesevery = new System.Windows.Forms.ComboBox();
            this.linkLabelCurrentVersion = new System.Windows.Forms.LinkLabel();
            this.linkLabelNewVersion2 = new System.Windows.Forms.LinkLabel();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.buttonCheckNewVersion = new System.Windows.Forms.Button();
            this.progressBarUpdate = new ProgressBarSample.TextProgressBar();
            this.groupBoxInfo = new System.Windows.Forms.GroupBox();
            this.checkBoxHistory = new System.Windows.Forms.CheckBox();
            this.buttonHistory = new System.Windows.Forms.Button();
            this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
            this.buttonLicence = new System.Windows.Forms.Button();
            this.tabControlGeneral.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBox_RigManager.SuspendLayout();
            this.groupBoxServer.SuspendLayout();
            this.groupBoxAPIkeys.SuspendLayout();
            this.groupBox_Idle.SuspendLayout();
            this.groupBox_Main.SuspendLayout();
            this.groupBox_Misc.SuspendLayout();
            this.groupBox_Logging.SuspendLayout();
            this.groupBox_Localization.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.tabPagePower.SuspendLayout();
            this.groupBox_additionally.SuspendLayout();
            this.groupBoxTariffs.SuspendLayout();
            this.tabPageAdvanced1.SuspendLayout();
            this.groupBoxConnection.SuspendLayout();
            this.groupBox_Miners.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageDevicesAlgos.SuspendLayout();
            this.groupBoxAlgorithmSettings.SuspendLayout();
            this.tabPageOverClock.SuspendLayout();
            this.groupBoxOverClockSettings.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            this.groupBoxBackup.SuspendLayout();
            this.groupBoxUpdates.SuspendLayout();
            this.groupBoxInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.ToolTip1_Popup);
            // 
            // buttonSaveClose
            // 
            this.buttonSaveClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveClose.Location = new System.Drawing.Point(398, 525);
            this.buttonSaveClose.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonSaveClose.Name = "buttonSaveClose";
            this.buttonSaveClose.Size = new System.Drawing.Size(134, 23);
            this.buttonSaveClose.TabIndex = 44;
            this.buttonSaveClose.Text = "&Save and Close";
            this.buttonSaveClose.UseVisualStyleBackColor = true;
            this.buttonSaveClose.Click += new System.EventHandler(this.ButtonSaveClose_Click);
            // 
            // buttonDefaults
            // 
            this.buttonDefaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDefaults.Location = new System.Drawing.Point(320, 525);
            this.buttonDefaults.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonDefaults.Name = "buttonDefaults";
            this.buttonDefaults.Size = new System.Drawing.Size(74, 23);
            this.buttonDefaults.TabIndex = 43;
            this.buttonDefaults.Text = "&Defaults";
            this.buttonDefaults.UseVisualStyleBackColor = true;
            this.buttonDefaults.Click += new System.EventHandler(this.ButtonDefaults_Click);
            // 
            // buttonCloseNoSave
            // 
            this.buttonCloseNoSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCloseNoSave.Location = new System.Drawing.Point(536, 525);
            this.buttonCloseNoSave.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonCloseNoSave.Name = "buttonCloseNoSave";
            this.buttonCloseNoSave.Size = new System.Drawing.Size(134, 23);
            this.buttonCloseNoSave.TabIndex = 45;
            this.buttonCloseNoSave.Text = "&Close without Saving";
            this.buttonCloseNoSave.UseVisualStyleBackColor = true;
            this.buttonCloseNoSave.Click += new System.EventHandler(this.ButtonCloseNoSave_Click);
            // 
            // tabControlGeneral
            // 
            this.tabControlGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlGeneral.Controls.Add(this.tabPageGeneral);
            this.tabControlGeneral.Controls.Add(this.tabPagePower);
            this.tabControlGeneral.Controls.Add(this.tabPageAdvanced1);
            this.tabControlGeneral.Controls.Add(this.tabPageDevicesAlgos);
            this.tabControlGeneral.Controls.Add(this.tabPageOverClock);
            this.tabControlGeneral.Controls.Add(this.tabPageAbout);
            // 
            // 
            // 
            this.tabControlGeneral.DisplayStyleProvider.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.tabControlGeneral.DisplayStyleProvider.BorderColorHot = System.Drawing.SystemColors.ControlDark;
            this.tabControlGeneral.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.tabControlGeneral.DisplayStyleProvider.CloserColor = System.Drawing.Color.DarkGray;
            this.tabControlGeneral.DisplayStyleProvider.FocusTrack = true;
            this.tabControlGeneral.DisplayStyleProvider.HotTrack = true;
            this.tabControlGeneral.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tabControlGeneral.DisplayStyleProvider.Opacity = 1F;
            this.tabControlGeneral.DisplayStyleProvider.Overlap = 0;
            this.tabControlGeneral.DisplayStyleProvider.Padding = new System.Drawing.Point(6, 3);
            this.tabControlGeneral.DisplayStyleProvider.Radius = 2;
            this.tabControlGeneral.DisplayStyleProvider.ShowTabCloser = false;
            this.tabControlGeneral.DisplayStyleProvider.TextColor = System.Drawing.SystemColors.ControlText;
            this.tabControlGeneral.DisplayStyleProvider.TextColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.tabControlGeneral.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText;
            this.tabControlGeneral.HotTrack = true;
            this.tabControlGeneral.Location = new System.Drawing.Point(3, 1);
            this.tabControlGeneral.Name = "tabControlGeneral";
            this.tabControlGeneral.SelectedIndex = 0;
            this.tabControlGeneral.Size = new System.Drawing.Size(677, 518);
            this.tabControlGeneral.TabIndex = 47;
            this.tabControlGeneral.SelectedIndexChanged += new System.EventHandler(this.tabControlGeneral_SelectedIndexChanged);
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageGeneral.Controls.Add(this.groupBox_RigManager);
            this.tabPageGeneral.Controls.Add(this.groupBoxServer);
            this.tabPageGeneral.Controls.Add(this.groupBoxAPIkeys);
            this.tabPageGeneral.Controls.Add(this.groupBox_Idle);
            this.tabPageGeneral.Controls.Add(this.groupBox_Main);
            this.tabPageGeneral.Controls.Add(this.groupBox_Misc);
            this.tabPageGeneral.Controls.Add(this.groupBox_Logging);
            this.tabPageGeneral.Controls.Add(this.groupBox_Localization);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 23);
            this.tabPageGeneral.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(669, 491);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            // 
            // groupBox_RigManager
            // 
            this.groupBox_RigManager.Controls.Add(this.checkBox_ShowPowerOfDisabledDevices);
            this.groupBox_RigManager.Controls.Add(this.checkBox_Allow_remote_management);
            this.groupBox_RigManager.Controls.Add(this.checkBox_Send_actual_version_info);
            this.groupBox_RigManager.Location = new System.Drawing.Point(376, 375);
            this.groupBox_RigManager.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_RigManager.Name = "groupBox_RigManager";
            this.groupBox_RigManager.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_RigManager.Size = new System.Drawing.Size(288, 110);
            this.groupBox_RigManager.TabIndex = 395;
            this.groupBox_RigManager.TabStop = false;
            this.groupBox_RigManager.Text = "Rig manager:";
            // 
            // checkBox_ShowPowerOfDisabledDevices
            // 
            this.checkBox_ShowPowerOfDisabledDevices.AutoSize = true;
            this.checkBox_ShowPowerOfDisabledDevices.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ShowPowerOfDisabledDevices.Location = new System.Drawing.Point(6, 65);
            this.checkBox_ShowPowerOfDisabledDevices.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ShowPowerOfDisabledDevices.Name = "checkBox_ShowPowerOfDisabledDevices";
            this.checkBox_ShowPowerOfDisabledDevices.Size = new System.Drawing.Size(211, 17);
            this.checkBox_ShowPowerOfDisabledDevices.TabIndex = 376;
            this.checkBox_ShowPowerOfDisabledDevices.Text = "Show power usage of disabled devices";
            this.checkBox_ShowPowerOfDisabledDevices.UseVisualStyleBackColor = true;
            // 
            // checkBox_Allow_remote_management
            // 
            this.checkBox_Allow_remote_management.AutoSize = true;
            this.checkBox_Allow_remote_management.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Allow_remote_management.Location = new System.Drawing.Point(6, 19);
            this.checkBox_Allow_remote_management.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Allow_remote_management.Name = "checkBox_Allow_remote_management";
            this.checkBox_Allow_remote_management.Size = new System.Drawing.Size(150, 17);
            this.checkBox_Allow_remote_management.TabIndex = 374;
            this.checkBox_Allow_remote_management.Text = "Allow remote management";
            this.checkBox_Allow_remote_management.UseVisualStyleBackColor = true;
            // 
            // checkBox_Send_actual_version_info
            // 
            this.checkBox_Send_actual_version_info.AutoSize = true;
            this.checkBox_Send_actual_version_info.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Send_actual_version_info.Location = new System.Drawing.Point(6, 42);
            this.checkBox_Send_actual_version_info.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Send_actual_version_info.Name = "checkBox_Send_actual_version_info";
            this.checkBox_Send_actual_version_info.Size = new System.Drawing.Size(140, 17);
            this.checkBox_Send_actual_version_info.TabIndex = 375;
            this.checkBox_Send_actual_version_info.Text = "Send actual version info";
            this.checkBox_Send_actual_version_info.UseVisualStyleBackColor = true;
            // 
            // groupBoxServer
            // 
            this.groupBoxServer.Controls.Add(this.linkLabelGetAPIkey);
            this.groupBoxServer.Controls.Add(this.label2);
            this.groupBoxServer.Controls.Add(this.textBoxAPIport);
            this.groupBoxServer.Controls.Add(this.checkBoxAPI);
            this.groupBoxServer.Controls.Add(this.linkLabelRigRemoteView);
            this.groupBoxServer.Controls.Add(this.checkBoxEnableRigRemoteView);
            this.groupBoxServer.Location = new System.Drawing.Point(6, 354);
            this.groupBoxServer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxServer.Name = "groupBoxServer";
            this.groupBoxServer.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxServer.Size = new System.Drawing.Size(365, 66);
            this.groupBoxServer.TabIndex = 394;
            this.groupBoxServer.TabStop = false;
            this.groupBoxServer.Text = "Internal server:";
            // 
            // linkLabelGetAPIkey
            // 
            this.linkLabelGetAPIkey.AutoSize = true;
            this.linkLabelGetAPIkey.Location = new System.Drawing.Point(243, 43);
            this.linkLabelGetAPIkey.Name = "linkLabelGetAPIkey";
            this.linkLabelGetAPIkey.Size = new System.Drawing.Size(61, 13);
            this.linkLabelGetAPIkey.TabIndex = 413;
            this.linkLabelGetAPIkey.TabStop = true;
            this.linkLabelGetAPIkey.Text = "How to use";
            this.linkLabelGetAPIkey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGetAPIkey_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 43);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 412;
            this.label2.Text = "API port:";
            // 
            // textBoxAPIport
            // 
            this.textBoxAPIport.Location = new System.Drawing.Point(190, 39);
            this.textBoxAPIport.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxAPIport.Name = "textBoxAPIport";
            this.textBoxAPIport.Size = new System.Drawing.Size(48, 20);
            this.textBoxAPIport.TabIndex = 411;
            // 
            // checkBoxAPI
            // 
            this.checkBoxAPI.AutoSize = true;
            this.checkBoxAPI.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxAPI.Location = new System.Drawing.Point(6, 42);
            this.checkBoxAPI.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxAPI.Name = "checkBoxAPI";
            this.checkBoxAPI.Size = new System.Drawing.Size(111, 17);
            this.checkBoxAPI.TabIndex = 410;
            this.checkBoxAPI.Text = "Enable API server";
            this.checkBoxAPI.UseVisualStyleBackColor = true;
            this.checkBoxAPI.CheckedChanged += new System.EventHandler(this.checkBoxAPI_CheckedChanged);
            // 
            // linkLabelRigRemoteView
            // 
            this.linkLabelRigRemoteView.AutoSize = true;
            this.linkLabelRigRemoteView.Location = new System.Drawing.Point(212, 20);
            this.linkLabelRigRemoteView.Name = "linkLabelRigRemoteView";
            this.linkLabelRigRemoteView.Size = new System.Drawing.Size(55, 13);
            this.linkLabelRigRemoteView.TabIndex = 409;
            this.linkLabelRigRemoteView.TabStop = true;
            this.linkLabelRigRemoteView.Text = "linkLabel1";
            this.linkLabelRigRemoteView.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelRigRemoteView_LinkClicked);
            // 
            // checkBoxEnableRigRemoteView
            // 
            this.checkBoxEnableRigRemoteView.AutoSize = true;
            this.checkBoxEnableRigRemoteView.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxEnableRigRemoteView.Location = new System.Drawing.Point(6, 19);
            this.checkBoxEnableRigRemoteView.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxEnableRigRemoteView.Name = "checkBoxEnableRigRemoteView";
            this.checkBoxEnableRigRemoteView.Size = new System.Drawing.Size(133, 17);
            this.checkBoxEnableRigRemoteView.TabIndex = 408;
            this.checkBoxEnableRigRemoteView.Text = "Enable rig remote view";
            this.checkBoxEnableRigRemoteView.UseVisualStyleBackColor = true;
            this.checkBoxEnableRigRemoteView.CheckedChanged += new System.EventHandler(this.checkBoxEnableRigRemoteView_CheckedChanged);
            // 
            // groupBoxAPIkeys
            // 
            this.groupBoxAPIkeys.Controls.Add(this.buttonSetupAPI);
            this.groupBoxAPIkeys.Controls.Add(this.checkBox_EnableAPI);
            this.groupBoxAPIkeys.Location = new System.Drawing.Point(6, 304);
            this.groupBoxAPIkeys.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxAPIkeys.Name = "groupBoxAPIkeys";
            this.groupBoxAPIkeys.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxAPIkeys.Size = new System.Drawing.Size(365, 44);
            this.groupBoxAPIkeys.TabIndex = 393;
            this.groupBoxAPIkeys.TabStop = false;
            this.groupBoxAPIkeys.Text = "API keys:";
            // 
            // buttonSetupAPI
            // 
            this.buttonSetupAPI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSetupAPI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSetupAPI.Location = new System.Drawing.Point(141, 13);
            this.buttonSetupAPI.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonSetupAPI.Name = "buttonSetupAPI";
            this.buttonSetupAPI.Size = new System.Drawing.Size(145, 23);
            this.buttonSetupAPI.TabIndex = 323;
            this.buttonSetupAPI.Text = "Setup API keys";
            this.buttonSetupAPI.UseVisualStyleBackColor = true;
            this.buttonSetupAPI.Click += new System.EventHandler(this.buttonSetupAPI_Click);
            // 
            // checkBox_EnableAPI
            // 
            this.checkBox_EnableAPI.AutoSize = true;
            this.checkBox_EnableAPI.Location = new System.Drawing.Point(9, 19);
            this.checkBox_EnableAPI.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_EnableAPI.Name = "checkBox_EnableAPI";
            this.checkBox_EnableAPI.Size = new System.Drawing.Size(104, 17);
            this.checkBox_EnableAPI.TabIndex = 322;
            this.checkBox_EnableAPI.Text = "Enable API keys";
            this.checkBox_EnableAPI.UseVisualStyleBackColor = true;
            this.checkBox_EnableAPI.CheckedChanged += new System.EventHandler(this.checkBox_EnableAPI_CheckedChanged);
            // 
            // groupBox_Idle
            // 
            this.groupBox_Idle.Controls.Add(this.checkBox_StartMiningWhenIdle);
            this.groupBox_Idle.Controls.Add(this.label_MinIdleSeconds);
            this.groupBox_Idle.Controls.Add(this.textBox_MinIdleSeconds);
            this.groupBox_Idle.Location = new System.Drawing.Point(6, 254);
            this.groupBox_Idle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Idle.Name = "groupBox_Idle";
            this.groupBox_Idle.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Idle.Size = new System.Drawing.Size(365, 44);
            this.groupBox_Idle.TabIndex = 392;
            this.groupBox_Idle.TabStop = false;
            this.groupBox_Idle.Text = "Idle:";
            // 
            // checkBox_StartMiningWhenIdle
            // 
            this.checkBox_StartMiningWhenIdle.AutoSize = true;
            this.checkBox_StartMiningWhenIdle.Location = new System.Drawing.Point(6, 19);
            this.checkBox_StartMiningWhenIdle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_StartMiningWhenIdle.Name = "checkBox_StartMiningWhenIdle";
            this.checkBox_StartMiningWhenIdle.Size = new System.Drawing.Size(125, 17);
            this.checkBox_StartMiningWhenIdle.TabIndex = 322;
            this.checkBox_StartMiningWhenIdle.Text = "StartMiningWhenIdle";
            this.checkBox_StartMiningWhenIdle.UseVisualStyleBackColor = true;
            // 
            // label_MinIdleSeconds
            // 
            this.label_MinIdleSeconds.AutoSize = true;
            this.label_MinIdleSeconds.Location = new System.Drawing.Point(170, 20);
            this.label_MinIdleSeconds.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MinIdleSeconds.Name = "label_MinIdleSeconds";
            this.label_MinIdleSeconds.Size = new System.Drawing.Size(86, 13);
            this.label_MinIdleSeconds.TabIndex = 356;
            this.label_MinIdleSeconds.Text = "MinIdleSeconds:";
            this.label_MinIdleSeconds.Click += new System.EventHandler(this.label_MinIdleSeconds_Click);
            // 
            // textBox_MinIdleSeconds
            // 
            this.textBox_MinIdleSeconds.Location = new System.Drawing.Point(316, 16);
            this.textBox_MinIdleSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MinIdleSeconds.Name = "textBox_MinIdleSeconds";
            this.textBox_MinIdleSeconds.Size = new System.Drawing.Size(28, 20);
            this.textBox_MinIdleSeconds.TabIndex = 335;
            this.textBox_MinIdleSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_MinIdleSeconds.TextChanged += new System.EventHandler(this.textBox_MinIdleSeconds_TextChanged);
            // 
            // groupBox_Main
            // 
            this.groupBox_Main.Controls.Add(this.checkbox_wallet_balance);
            this.groupBox_Main.Controls.Add(this.checkBox_fiat);
            this.groupBox_Main.Controls.Add(this.checkBox_Force_mining_if_nonprofitable);
            this.groupBox_Main.Controls.Add(this.label_TimeUnit);
            this.groupBox_Main.Controls.Add(this.comboBox_TimeUnit);
            this.groupBox_Main.Controls.Add(this.textBox_MinProfit);
            this.groupBox_Main.Controls.Add(this.label_MinProfit);
            this.groupBox_Main.Controls.Add(this.checkBox_AutoScaleBTCValues);
            this.groupBox_Main.Location = new System.Drawing.Point(6, 6);
            this.groupBox_Main.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Main.Name = "groupBox_Main";
            this.groupBox_Main.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Main.Size = new System.Drawing.Size(365, 114);
            this.groupBox_Main.TabIndex = 386;
            this.groupBox_Main.TabStop = false;
            this.groupBox_Main.Text = "Main:";
            this.groupBox_Main.Enter += new System.EventHandler(this.groupBox_Main_Enter);
            // 
            // checkbox_wallet_balance
            // 
            this.checkbox_wallet_balance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkbox_wallet_balance.Location = new System.Drawing.Point(179, 89);
            this.checkbox_wallet_balance.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkbox_wallet_balance.Name = "checkbox_wallet_balance";
            this.checkbox_wallet_balance.Size = new System.Drawing.Size(182, 18);
            this.checkbox_wallet_balance.TabIndex = 385;
            this.checkbox_wallet_balance.Text = "Show wallet balance";
            this.checkbox_wallet_balance.UseVisualStyleBackColor = true;
            this.checkbox_wallet_balance.Click += new System.EventHandler(this.checkbox_wallet_balance_Click);
            // 
            // checkBox_fiat
            // 
            this.checkBox_fiat.AutoSize = true;
            this.checkBox_fiat.Location = new System.Drawing.Point(9, 45);
            this.checkBox_fiat.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_fiat.Name = "checkBox_fiat";
            this.checkBox_fiat.Size = new System.Drawing.Size(239, 17);
            this.checkBox_fiat.TabIndex = 382;
            this.checkBox_fiat.Text = "Show profitability of algorithms in fiat currency";
            this.checkBox_fiat.UseVisualStyleBackColor = true;
            this.checkBox_fiat.CheckedChanged += new System.EventHandler(this.checkBox_fiat_CheckedChanged);
            // 
            // checkBox_Force_mining_if_nonprofitable
            // 
            this.checkBox_Force_mining_if_nonprofitable.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Force_mining_if_nonprofitable.Location = new System.Drawing.Point(9, 67);
            this.checkBox_Force_mining_if_nonprofitable.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Force_mining_if_nonprofitable.Name = "checkBox_Force_mining_if_nonprofitable";
            this.checkBox_Force_mining_if_nonprofitable.Size = new System.Drawing.Size(318, 18);
            this.checkBox_Force_mining_if_nonprofitable.TabIndex = 376;
            this.checkBox_Force_mining_if_nonprofitable.Text = "Force mining if nonprofitable";
            this.checkBox_Force_mining_if_nonprofitable.UseVisualStyleBackColor = true;
            this.checkBox_Force_mining_if_nonprofitable.CheckedChanged += new System.EventHandler(this.checkBox_Force_mining_if_nonprofitable_CheckedChanged_1);
            // 
            // label_TimeUnit
            // 
            this.label_TimeUnit.AutoSize = true;
            this.label_TimeUnit.Location = new System.Drawing.Point(209, 22);
            this.label_TimeUnit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_TimeUnit.Name = "label_TimeUnit";
            this.label_TimeUnit.Size = new System.Drawing.Size(52, 13);
            this.label_TimeUnit.TabIndex = 371;
            this.label_TimeUnit.Text = "TimeUnit:";
            this.label_TimeUnit.Click += new System.EventHandler(this.label_TimeUnit_Click_1);
            // 
            // comboBox_TimeUnit
            // 
            this.comboBox_TimeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TimeUnit.FormattingEnabled = true;
            this.comboBox_TimeUnit.Location = new System.Drawing.Point(282, 19);
            this.comboBox_TimeUnit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_TimeUnit.Name = "comboBox_TimeUnit";
            this.comboBox_TimeUnit.Size = new System.Drawing.Size(62, 21);
            this.comboBox_TimeUnit.TabIndex = 370;
            this.comboBox_TimeUnit.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_TimeUnit_DrawItem);
            this.comboBox_TimeUnit.SelectedIndexChanged += new System.EventHandler(this.comboBox_TimeUnit_SelectedIndexChanged);
            // 
            // textBox_MinProfit
            // 
            this.textBox_MinProfit.Location = new System.Drawing.Point(128, 19);
            this.textBox_MinProfit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MinProfit.Name = "textBox_MinProfit";
            this.textBox_MinProfit.Size = new System.Drawing.Size(39, 20);
            this.textBox_MinProfit.TabIndex = 334;
            this.textBox_MinProfit.TextChanged += new System.EventHandler(this.textBox_MinProfit_TextChanged_1);
            // 
            // label_MinProfit
            // 
            this.label_MinProfit.AutoSize = true;
            this.label_MinProfit.Location = new System.Drawing.Point(6, 21);
            this.label_MinProfit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MinProfit.Name = "label_MinProfit";
            this.label_MinProfit.Size = new System.Drawing.Size(115, 13);
            this.label_MinProfit.TabIndex = 357;
            this.label_MinProfit.Text = "Minimum Profit ($/day):";
            // 
            // checkBox_AutoScaleBTCValues
            // 
            this.checkBox_AutoScaleBTCValues.AutoSize = true;
            this.checkBox_AutoScaleBTCValues.Location = new System.Drawing.Point(9, 90);
            this.checkBox_AutoScaleBTCValues.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AutoScaleBTCValues.Name = "checkBox_AutoScaleBTCValues";
            this.checkBox_AutoScaleBTCValues.Size = new System.Drawing.Size(128, 17);
            this.checkBox_AutoScaleBTCValues.TabIndex = 321;
            this.checkBox_AutoScaleBTCValues.Text = "AutoScaleBTCValues";
            this.checkBox_AutoScaleBTCValues.UseVisualStyleBackColor = true;
            // 
            // groupBox_Misc
            // 
            this.groupBox_Misc.Controls.Add(this.checkBoxInstall_root_certificates);
            this.groupBox_Misc.Controls.Add(this.checkBox_DisableTooltips);
            this.groupBox_Misc.Controls.Add(this.labelRestartProgram);
            this.groupBox_Misc.Controls.Add(this.checkBoxShowMinersVersions);
            this.groupBox_Misc.Controls.Add(this.comboBoxRestartProgram);
            this.groupBox_Misc.Controls.Add(this.checkBox_program_monitoring);
            this.groupBox_Misc.Controls.Add(this.checkBox_AlwaysOnTop);
            this.groupBox_Misc.Controls.Add(this.checkBox_sorting_list_of_algorithms);
            this.groupBox_Misc.Controls.Add(this.Checkbox_Save_windows_size_and_position);
            this.groupBox_Misc.Controls.Add(this.label1);
            this.groupBox_Misc.Controls.Add(this.textBox_AutoStartMiningDelay);
            this.groupBox_Misc.Controls.Add(this.label_AutoStartMiningDelay);
            this.groupBox_Misc.Controls.Add(this.checkBox_AllowMultipleInstances);
            this.groupBox_Misc.Controls.Add(this.comboBox_ColorProfile);
            this.groupBox_Misc.Controls.Add(this.checkBox_MinimizeMiningWindows);
            this.groupBox_Misc.Controls.Add(this.checkBox_RunAtStartup);
            this.groupBox_Misc.Controls.Add(this.checkBox_AutoStartMining);
            this.groupBox_Misc.Controls.Add(this.checkBox_HideMiningWindows);
            this.groupBox_Misc.Controls.Add(this.checkBox_MinimizeToTray);
            this.groupBox_Misc.Location = new System.Drawing.Point(375, 6);
            this.groupBox_Misc.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Misc.Name = "groupBox_Misc";
            this.groupBox_Misc.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Misc.Size = new System.Drawing.Size(288, 363);
            this.groupBox_Misc.TabIndex = 391;
            this.groupBox_Misc.TabStop = false;
            this.groupBox_Misc.Text = "Misc:";
            this.groupBox_Misc.Enter += new System.EventHandler(this.groupBox_Misc_Enter);
            // 
            // checkBoxInstall_root_certificates
            // 
            this.checkBoxInstall_root_certificates.AutoSize = true;
            this.checkBoxInstall_root_certificates.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxInstall_root_certificates.Location = new System.Drawing.Point(7, 287);
            this.checkBoxInstall_root_certificates.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxInstall_root_certificates.Name = "checkBoxInstall_root_certificates";
            this.checkBoxInstall_root_certificates.Size = new System.Drawing.Size(128, 17);
            this.checkBoxInstall_root_certificates.TabIndex = 409;
            this.checkBoxInstall_root_certificates.Text = "Install root certificates";
            this.checkBoxInstall_root_certificates.UseVisualStyleBackColor = true;
            // 
            // checkBox_DisableTooltips
            // 
            this.checkBox_DisableTooltips.AutoSize = true;
            this.checkBox_DisableTooltips.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_DisableTooltips.Location = new System.Drawing.Point(7, 183);
            this.checkBox_DisableTooltips.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableTooltips.Name = "checkBox_DisableTooltips";
            this.checkBox_DisableTooltips.Size = new System.Drawing.Size(97, 17);
            this.checkBox_DisableTooltips.TabIndex = 407;
            this.checkBox_DisableTooltips.Text = "Disable tooltips";
            this.checkBox_DisableTooltips.UseVisualStyleBackColor = true;
            // 
            // labelRestartProgram
            // 
            this.labelRestartProgram.AutoSize = true;
            this.labelRestartProgram.Location = new System.Drawing.Point(4, 337);
            this.labelRestartProgram.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelRestartProgram.Name = "labelRestartProgram";
            this.labelRestartProgram.Size = new System.Drawing.Size(82, 13);
            this.labelRestartProgram.TabIndex = 406;
            this.labelRestartProgram.Text = "Restart program";
            // 
            // checkBoxShowMinersVersions
            // 
            this.checkBoxShowMinersVersions.AutoSize = true;
            this.checkBoxShowMinersVersions.Location = new System.Drawing.Point(7, 264);
            this.checkBoxShowMinersVersions.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxShowMinersVersions.Name = "checkBoxShowMinersVersions";
            this.checkBoxShowMinersVersions.Size = new System.Drawing.Size(128, 17);
            this.checkBoxShowMinersVersions.TabIndex = 408;
            this.checkBoxShowMinersVersions.Text = "Show miners versions";
            this.checkBoxShowMinersVersions.UseVisualStyleBackColor = true;
            this.checkBoxShowMinersVersions.CheckedChanged += new System.EventHandler(this.checkBoxShowMinersVersions_CheckedChanged);
            // 
            // comboBoxRestartProgram
            // 
            this.comboBoxRestartProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRestartProgram.FormattingEnabled = true;
            this.comboBoxRestartProgram.Location = new System.Drawing.Point(136, 334);
            this.comboBoxRestartProgram.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBoxRestartProgram.Name = "comboBoxRestartProgram";
            this.comboBoxRestartProgram.Size = new System.Drawing.Size(138, 21);
            this.comboBoxRestartProgram.TabIndex = 405;
            this.comboBoxRestartProgram.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxRestartProgram_DrawItem);
            // 
            // checkBox_program_monitoring
            // 
            this.checkBox_program_monitoring.AutoSize = true;
            this.checkBox_program_monitoring.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_program_monitoring.Location = new System.Drawing.Point(7, 310);
            this.checkBox_program_monitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_program_monitoring.Name = "checkBox_program_monitoring";
            this.checkBox_program_monitoring.Size = new System.Drawing.Size(116, 17);
            this.checkBox_program_monitoring.TabIndex = 384;
            this.checkBox_program_monitoring.Text = "Program monitoring";
            this.checkBox_program_monitoring.UseVisualStyleBackColor = true;
            this.checkBox_program_monitoring.CheckedChanged += new System.EventHandler(this.checkBox_program_monitoring_CheckedChanged);
            // 
            // checkBox_AlwaysOnTop
            // 
            this.checkBox_AlwaysOnTop.AutoSize = true;
            this.checkBox_AlwaysOnTop.Location = new System.Drawing.Point(7, 137);
            this.checkBox_AlwaysOnTop.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AlwaysOnTop.Name = "checkBox_AlwaysOnTop";
            this.checkBox_AlwaysOnTop.Size = new System.Drawing.Size(96, 17);
            this.checkBox_AlwaysOnTop.TabIndex = 382;
            this.checkBox_AlwaysOnTop.Text = "Always on Top";
            this.checkBox_AlwaysOnTop.UseVisualStyleBackColor = true;
            this.checkBox_AlwaysOnTop.CheckedChanged += new System.EventHandler(this.checkBox_AlwaysOnTop_CheckedChanged);
            // 
            // checkBox_sorting_list_of_algorithms
            // 
            this.checkBox_sorting_list_of_algorithms.AutoSize = true;
            this.checkBox_sorting_list_of_algorithms.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_sorting_list_of_algorithms.Location = new System.Drawing.Point(7, 206);
            this.checkBox_sorting_list_of_algorithms.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_sorting_list_of_algorithms.Name = "checkBox_sorting_list_of_algorithms";
            this.checkBox_sorting_list_of_algorithms.Size = new System.Drawing.Size(136, 17);
            this.checkBox_sorting_list_of_algorithms.TabIndex = 381;
            this.checkBox_sorting_list_of_algorithms.Text = "Sorting list of algorithms";
            this.checkBox_sorting_list_of_algorithms.UseVisualStyleBackColor = true;
            // 
            // Checkbox_Save_windows_size_and_position
            // 
            this.Checkbox_Save_windows_size_and_position.AutoSize = true;
            this.Checkbox_Save_windows_size_and_position.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Checkbox_Save_windows_size_and_position.Location = new System.Drawing.Point(7, 160);
            this.Checkbox_Save_windows_size_and_position.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Checkbox_Save_windows_size_and_position.Name = "Checkbox_Save_windows_size_and_position";
            this.Checkbox_Save_windows_size_and_position.Size = new System.Drawing.Size(176, 17);
            this.Checkbox_Save_windows_size_and_position.TabIndex = 380;
            this.Checkbox_Save_windows_size_and_position.Text = "Save windows size and position";
            this.Checkbox_Save_windows_size_and_position.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 234);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 379;
            this.label1.Text = "Color profile";
            // 
            // textBox_AutoStartMiningDelay
            // 
            this.textBox_AutoStartMiningDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_AutoStartMiningDelay.Location = new System.Drawing.Point(179, 41);
            this.textBox_AutoStartMiningDelay.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_AutoStartMiningDelay.Name = "textBox_AutoStartMiningDelay";
            this.textBox_AutoStartMiningDelay.Size = new System.Drawing.Size(20, 20);
            this.textBox_AutoStartMiningDelay.TabIndex = 377;
            // 
            // label_AutoStartMiningDelay
            // 
            this.label_AutoStartMiningDelay.AutoSize = true;
            this.label_AutoStartMiningDelay.Location = new System.Drawing.Point(203, 45);
            this.label_AutoStartMiningDelay.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_AutoStartMiningDelay.Name = "label_AutoStartMiningDelay";
            this.label_AutoStartMiningDelay.Size = new System.Drawing.Size(47, 13);
            this.label_AutoStartMiningDelay.TabIndex = 376;
            this.label_AutoStartMiningDelay.Text = "seconds";
            // 
            // checkBox_AllowMultipleInstances
            // 
            this.checkBox_AllowMultipleInstances.AutoSize = true;
            this.checkBox_AllowMultipleInstances.Location = new System.Drawing.Point(7, 67);
            this.checkBox_AllowMultipleInstances.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AllowMultipleInstances.Name = "checkBox_AllowMultipleInstances";
            this.checkBox_AllowMultipleInstances.Size = new System.Drawing.Size(139, 17);
            this.checkBox_AllowMultipleInstances.TabIndex = 365;
            this.checkBox_AllowMultipleInstances.Text = "Allow Multiple Instances";
            this.checkBox_AllowMultipleInstances.UseVisualStyleBackColor = true;
            this.checkBox_AllowMultipleInstances.CheckedChanged += new System.EventHandler(this.checkBox_AllowMultipleInstances_CheckedChanged);
            // 
            // comboBox_ColorProfile
            // 
            this.comboBox_ColorProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ColorProfile.FormattingEnabled = true;
            this.comboBox_ColorProfile.Location = new System.Drawing.Point(172, 230);
            this.comboBox_ColorProfile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_ColorProfile.Name = "comboBox_ColorProfile";
            this.comboBox_ColorProfile.Size = new System.Drawing.Size(102, 21);
            this.comboBox_ColorProfile.TabIndex = 378;
            this.comboBox_ColorProfile.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_ColorProfile_DrawItem);
            this.comboBox_ColorProfile.SelectedIndexChanged += new System.EventHandler(this.comboBox_ColorProfile_SelectedIndexChanged);
            // 
            // checkBox_MinimizeMiningWindows
            // 
            this.checkBox_MinimizeMiningWindows.AutoSize = true;
            this.checkBox_MinimizeMiningWindows.Location = new System.Drawing.Point(7, 113);
            this.checkBox_MinimizeMiningWindows.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_MinimizeMiningWindows.Name = "checkBox_MinimizeMiningWindows";
            this.checkBox_MinimizeMiningWindows.Size = new System.Drawing.Size(141, 17);
            this.checkBox_MinimizeMiningWindows.TabIndex = 368;
            this.checkBox_MinimizeMiningWindows.Text = "MinimizeMiningWindows";
            this.checkBox_MinimizeMiningWindows.UseVisualStyleBackColor = true;
            // 
            // checkBox_RunAtStartup
            // 
            this.checkBox_RunAtStartup.AutoSize = true;
            this.checkBox_RunAtStartup.Location = new System.Drawing.Point(7, 21);
            this.checkBox_RunAtStartup.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_RunAtStartup.Name = "checkBox_RunAtStartup";
            this.checkBox_RunAtStartup.Size = new System.Drawing.Size(120, 17);
            this.checkBox_RunAtStartup.TabIndex = 366;
            this.checkBox_RunAtStartup.Text = "Start With Windows";
            this.checkBox_RunAtStartup.UseVisualStyleBackColor = true;
            this.checkBox_RunAtStartup.CheckedChanged += new System.EventHandler(this.checkBox_RunAtStartup_CheckedChanged_1);
            // 
            // checkBox_AutoStartMining
            // 
            this.checkBox_AutoStartMining.AutoSize = true;
            this.checkBox_AutoStartMining.Location = new System.Drawing.Point(7, 44);
            this.checkBox_AutoStartMining.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AutoStartMining.Name = "checkBox_AutoStartMining";
            this.checkBox_AutoStartMining.Size = new System.Drawing.Size(152, 17);
            this.checkBox_AutoStartMining.TabIndex = 315;
            this.checkBox_AutoStartMining.Text = "Autostart Mining with delay";
            this.checkBox_AutoStartMining.UseVisualStyleBackColor = true;
            this.checkBox_AutoStartMining.CheckedChanged += new System.EventHandler(this.checkBox_AutoStartMining_CheckedChanged_1);
            // 
            // checkBox_HideMiningWindows
            // 
            this.checkBox_HideMiningWindows.AutoSize = true;
            this.checkBox_HideMiningWindows.Location = new System.Drawing.Point(7, 90);
            this.checkBox_HideMiningWindows.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_HideMiningWindows.Name = "checkBox_HideMiningWindows";
            this.checkBox_HideMiningWindows.Size = new System.Drawing.Size(123, 17);
            this.checkBox_HideMiningWindows.TabIndex = 315;
            this.checkBox_HideMiningWindows.Text = "HideMiningWindows";
            this.checkBox_HideMiningWindows.UseVisualStyleBackColor = true;
            // 
            // checkBox_MinimizeToTray
            // 
            this.checkBox_MinimizeToTray.AutoSize = true;
            this.checkBox_MinimizeToTray.Location = new System.Drawing.Point(169, 137);
            this.checkBox_MinimizeToTray.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_MinimizeToTray.Name = "checkBox_MinimizeToTray";
            this.checkBox_MinimizeToTray.Size = new System.Drawing.Size(100, 17);
            this.checkBox_MinimizeToTray.TabIndex = 316;
            this.checkBox_MinimizeToTray.Text = "MinimizeToTray";
            this.checkBox_MinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // groupBox_Logging
            // 
            this.groupBox_Logging.Controls.Add(this.checkBoxSaveProtocolData);
            this.groupBox_Logging.Controls.Add(this.label_LogMaxFileSize);
            this.groupBox_Logging.Controls.Add(this.textBox_LogMaxFileSize);
            this.groupBox_Logging.Controls.Add(this.checkBox_LogToFile);
            this.groupBox_Logging.Location = new System.Drawing.Point(6, 182);
            this.groupBox_Logging.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Logging.Name = "groupBox_Logging";
            this.groupBox_Logging.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Logging.Size = new System.Drawing.Size(365, 66);
            this.groupBox_Logging.TabIndex = 388;
            this.groupBox_Logging.TabStop = false;
            this.groupBox_Logging.Text = "Logging:";
            // 
            // checkBoxSaveProtocolData
            // 
            this.checkBoxSaveProtocolData.AutoSize = true;
            this.checkBoxSaveProtocolData.Location = new System.Drawing.Point(6, 42);
            this.checkBoxSaveProtocolData.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxSaveProtocolData.Name = "checkBoxSaveProtocolData";
            this.checkBoxSaveProtocolData.Size = new System.Drawing.Size(277, 17);
            this.checkBoxSaveProtocolData.TabIndex = 358;
            this.checkBoxSaveProtocolData.Text = "Saving communication data with the Nicehash server";
            this.checkBoxSaveProtocolData.UseVisualStyleBackColor = true;
            // 
            // label_LogMaxFileSize
            // 
            this.label_LogMaxFileSize.AutoSize = true;
            this.label_LogMaxFileSize.Location = new System.Drawing.Point(135, 20);
            this.label_LogMaxFileSize.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LogMaxFileSize.Name = "label_LogMaxFileSize";
            this.label_LogMaxFileSize.Size = new System.Drawing.Size(84, 13);
            this.label_LogMaxFileSize.TabIndex = 357;
            this.label_LogMaxFileSize.Text = "LogMaxFileSize:";
            this.label_LogMaxFileSize.Click += new System.EventHandler(this.label_LogMaxFileSize_Click);
            // 
            // textBox_LogMaxFileSize
            // 
            this.textBox_LogMaxFileSize.Location = new System.Drawing.Point(283, 17);
            this.textBox_LogMaxFileSize.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_LogMaxFileSize.Name = "textBox_LogMaxFileSize";
            this.textBox_LogMaxFileSize.Size = new System.Drawing.Size(61, 20);
            this.textBox_LogMaxFileSize.TabIndex = 334;
            // 
            // checkBox_LogToFile
            // 
            this.checkBox_LogToFile.AutoSize = true;
            this.checkBox_LogToFile.Location = new System.Drawing.Point(6, 19);
            this.checkBox_LogToFile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_LogToFile.Name = "checkBox_LogToFile";
            this.checkBox_LogToFile.Size = new System.Drawing.Size(72, 17);
            this.checkBox_LogToFile.TabIndex = 327;
            this.checkBox_LogToFile.Text = "Log to file";
            this.checkBox_LogToFile.UseVisualStyleBackColor = true;
            // 
            // groupBox_Localization
            // 
            this.groupBox_Localization.Controls.Add(this.label_Language);
            this.groupBox_Localization.Controls.Add(this.pictureBox5);
            this.groupBox_Localization.Controls.Add(this.comboBox_Language);
            this.groupBox_Localization.Controls.Add(this.currencyConverterCombobox);
            this.groupBox_Localization.Controls.Add(this.label_displayCurrency);
            this.groupBox_Localization.Location = new System.Drawing.Point(6, 126);
            this.groupBox_Localization.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Localization.Name = "groupBox_Localization";
            this.groupBox_Localization.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Localization.Size = new System.Drawing.Size(365, 50);
            this.groupBox_Localization.TabIndex = 385;
            this.groupBox_Localization.TabStop = false;
            this.groupBox_Localization.Text = "Localization:";
            // 
            // label_Language
            // 
            this.label_Language.AutoSize = true;
            this.label_Language.Location = new System.Drawing.Point(6, 22);
            this.label_Language.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Language.Name = "label_Language";
            this.label_Language.Size = new System.Drawing.Size(58, 13);
            this.label_Language.TabIndex = 358;
            this.label_Language.Text = "Language:";
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox5.Location = new System.Drawing.Point(-58, 59);
            this.pictureBox5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(18, 18);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox5.TabIndex = 364;
            this.pictureBox5.TabStop = false;
            // 
            // comboBox_Language
            // 
            this.comboBox_Language.BackColor = System.Drawing.SystemColors.Control;
            this.comboBox_Language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Language.FormattingEnabled = true;
            this.comboBox_Language.Location = new System.Drawing.Point(68, 19);
            this.comboBox_Language.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_Language.Name = "comboBox_Language";
            this.comboBox_Language.Size = new System.Drawing.Size(99, 21);
            this.comboBox_Language.TabIndex = 328;
            this.comboBox_Language.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_Language_DrawItem);
            this.comboBox_Language.SelectedIndexChanged += new System.EventHandler(this.comboBox_Language_SelectedIndexChanged);
            // 
            // currencyConverterCombobox
            // 
            this.currencyConverterCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.currencyConverterCombobox.FormattingEnabled = true;
            this.currencyConverterCombobox.Items.AddRange(new object[] {
            "AUD",
            "BGN",
            "BRL",
            "CAD",
            "CHF",
            "CNY",
            "CZK",
            "DKK",
            "EUR",
            "GBP",
            "HKD",
            "HRK",
            "HUF",
            "IDR",
            "ILS",
            "INR",
            "JPY",
            "KRW",
            "MXN",
            "MYR",
            "NOK",
            "NZD",
            "PHP",
            "PLN",
            "RON",
            "RUB",
            "SEK",
            "SGD",
            "THB",
            "TRY",
            "USD",
            "ZAR"});
            this.currencyConverterCombobox.Location = new System.Drawing.Point(278, 19);
            this.currencyConverterCombobox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.currencyConverterCombobox.Name = "currencyConverterCombobox";
            this.currencyConverterCombobox.Size = new System.Drawing.Size(64, 21);
            this.currencyConverterCombobox.Sorted = true;
            this.currencyConverterCombobox.TabIndex = 381;
            this.currencyConverterCombobox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.currencyConverterCombobox_DrawItem);
            this.currencyConverterCombobox.SelectedIndexChanged += new System.EventHandler(this.currencyConverterCombobox_SelectedIndexChanged);
            this.currencyConverterCombobox.SelectionChangeCommitted += new System.EventHandler(this.currencyConverterCombobox_SelectionChangeCommitted);
            // 
            // label_displayCurrency
            // 
            this.label_displayCurrency.AutoSize = true;
            this.label_displayCurrency.Location = new System.Drawing.Point(209, 22);
            this.label_displayCurrency.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_displayCurrency.Name = "label_displayCurrency";
            this.label_displayCurrency.Size = new System.Drawing.Size(52, 13);
            this.label_displayCurrency.TabIndex = 382;
            this.label_displayCurrency.Text = "Currency:";
            // 
            // tabPagePower
            // 
            this.tabPagePower.Controls.Add(this.groupBox_additionally);
            this.tabPagePower.Controls.Add(this.groupBoxTariffs);
            this.tabPagePower.Location = new System.Drawing.Point(4, 23);
            this.tabPagePower.Name = "tabPagePower";
            this.tabPagePower.Size = new System.Drawing.Size(669, 491);
            this.tabPagePower.TabIndex = 6;
            this.tabPagePower.Text = "Power";
            this.tabPagePower.UseVisualStyleBackColor = true;
            // 
            // groupBox_additionally
            // 
            this.groupBox_additionally.Controls.Add(this.checkBox_Show_Total_Power);
            this.groupBox_additionally.Controls.Add(this.label_psu);
            this.groupBox_additionally.Controls.Add(this.checkBox_Show_profit_with_power_consumption);
            this.groupBox_additionally.Controls.Add(this.textBox_psu);
            this.groupBox_additionally.Controls.Add(this.label_MBpower);
            this.groupBox_additionally.Controls.Add(this.labelAddAMD);
            this.groupBox_additionally.Controls.Add(this.textBox_mb);
            this.groupBox_additionally.Controls.Add(this.textBoxAddAMD);
            this.groupBox_additionally.Location = new System.Drawing.Point(343, 6);
            this.groupBox_additionally.Name = "groupBox_additionally";
            this.groupBox_additionally.Size = new System.Drawing.Size(320, 184);
            this.groupBox_additionally.TabIndex = 395;
            this.groupBox_additionally.TabStop = false;
            this.groupBox_additionally.Text = "Additionally";
            // 
            // checkBox_Show_Total_Power
            // 
            this.checkBox_Show_Total_Power.AutoSize = true;
            this.checkBox_Show_Total_Power.Location = new System.Drawing.Point(8, 101);
            this.checkBox_Show_Total_Power.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Show_Total_Power.Name = "checkBox_Show_Total_Power";
            this.checkBox_Show_Total_Power.Size = new System.Drawing.Size(220, 17);
            this.checkBox_Show_Total_Power.TabIndex = 386;
            this.checkBox_Show_Total_Power.Text = "Show total power consumption for uptime";
            this.checkBox_Show_Total_Power.UseVisualStyleBackColor = true;
            // 
            // label_psu
            // 
            this.label_psu.AutoSize = true;
            this.label_psu.Location = new System.Drawing.Point(5, 22);
            this.label_psu.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_psu.Name = "label_psu";
            this.label_psu.Size = new System.Drawing.Size(94, 13);
            this.label_psu.TabIndex = 391;
            this.label_psu.Text = "PSU efficiency (%)";
            // 
            // checkBox_Show_profit_with_power_consumption
            // 
            this.checkBox_Show_profit_with_power_consumption.AutoSize = true;
            this.checkBox_Show_profit_with_power_consumption.Location = new System.Drawing.Point(8, 126);
            this.checkBox_Show_profit_with_power_consumption.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Show_profit_with_power_consumption.Name = "checkBox_Show_profit_with_power_consumption";
            this.checkBox_Show_profit_with_power_consumption.Size = new System.Drawing.Size(196, 17);
            this.checkBox_Show_profit_with_power_consumption.TabIndex = 377;
            this.checkBox_Show_profit_with_power_consumption.Text = "Show profit with power consumption";
            this.checkBox_Show_profit_with_power_consumption.UseVisualStyleBackColor = true;
            this.checkBox_Show_profit_with_power_consumption.CheckedChanged += new System.EventHandler(this.checkBox_Show_profit_with_power_consumption_CheckedChanged);
            // 
            // textBox_psu
            // 
            this.textBox_psu.Location = new System.Drawing.Point(110, 17);
            this.textBox_psu.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_psu.Name = "textBox_psu";
            this.textBox_psu.Size = new System.Drawing.Size(39, 20);
            this.textBox_psu.TabIndex = 390;
            // 
            // label_MBpower
            // 
            this.label_MBpower.AutoSize = true;
            this.label_MBpower.Location = new System.Drawing.Point(5, 49);
            this.label_MBpower.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MBpower.Name = "label_MBpower";
            this.label_MBpower.Size = new System.Drawing.Size(271, 13);
            this.label_MBpower.TabIndex = 389;
            this.label_MBpower.Text = "Power consumption of Motherboard, HDD(SSD) etc (W)";
            // 
            // labelAddAMD
            // 
            this.labelAddAMD.AutoSize = true;
            this.labelAddAMD.Location = new System.Drawing.Point(5, 76);
            this.labelAddAMD.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAddAMD.Name = "labelAddAMD";
            this.labelAddAMD.Size = new System.Drawing.Size(221, 13);
            this.labelAddAMD.TabIndex = 393;
            this.labelAddAMD.Text = "Additional AMD GPU power consumption (W)";
            // 
            // textBox_mb
            // 
            this.textBox_mb.Location = new System.Drawing.Point(280, 46);
            this.textBox_mb.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_mb.Name = "textBox_mb";
            this.textBox_mb.Size = new System.Drawing.Size(35, 20);
            this.textBox_mb.TabIndex = 388;
            // 
            // textBoxAddAMD
            // 
            this.textBoxAddAMD.Location = new System.Drawing.Point(251, 73);
            this.textBoxAddAMD.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxAddAMD.Name = "textBoxAddAMD";
            this.textBoxAddAMD.Size = new System.Drawing.Size(37, 20);
            this.textBoxAddAMD.TabIndex = 392;
            // 
            // groupBoxTariffs
            // 
            this.groupBoxTariffs.Controls.Add(this.labelTo5);
            this.groupBoxTariffs.Controls.Add(this.labelTo4);
            this.groupBoxTariffs.Controls.Add(this.labelTo3);
            this.groupBoxTariffs.Controls.Add(this.labelTo2);
            this.groupBoxTariffs.Controls.Add(this.labelPowerCurrency5);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleTo5);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleFrom5);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleCost5);
            this.groupBoxTariffs.Controls.Add(this.labelCost5);
            this.groupBoxTariffs.Controls.Add(this.labelFrom5);
            this.groupBoxTariffs.Controls.Add(this.labelPowerCurrency4);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleTo4);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleFrom4);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleCost4);
            this.groupBoxTariffs.Controls.Add(this.labelCost4);
            this.groupBoxTariffs.Controls.Add(this.labelFrom4);
            this.groupBoxTariffs.Controls.Add(this.labelPowerCurrency3);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleTo3);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleFrom3);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleCost3);
            this.groupBoxTariffs.Controls.Add(this.labelCost3);
            this.groupBoxTariffs.Controls.Add(this.labelFrom3);
            this.groupBoxTariffs.Controls.Add(this.labelPowerCurrency2);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleTo2);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleFrom2);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleCost2);
            this.groupBoxTariffs.Controls.Add(this.labelCost2);
            this.groupBoxTariffs.Controls.Add(this.labelFrom2);
            this.groupBoxTariffs.Controls.Add(this.labelPowerCurrency1);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleTo1);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleFrom1);
            this.groupBoxTariffs.Controls.Add(this.textBoxScheduleCost1);
            this.groupBoxTariffs.Controls.Add(this.labelCost1);
            this.groupBoxTariffs.Controls.Add(this.labelTo1);
            this.groupBoxTariffs.Controls.Add(this.labelFrom1);
            this.groupBoxTariffs.Controls.Add(this.label_Schedules2);
            this.groupBoxTariffs.Controls.Add(this.label_Schedules);
            this.groupBoxTariffs.Controls.Add(this.comboBoxZones);
            this.groupBoxTariffs.Location = new System.Drawing.Point(5, 6);
            this.groupBoxTariffs.Name = "groupBoxTariffs";
            this.groupBoxTariffs.Size = new System.Drawing.Size(332, 184);
            this.groupBoxTariffs.TabIndex = 394;
            this.groupBoxTariffs.TabStop = false;
            this.groupBoxTariffs.Text = "Tariffs";
            // 
            // labelTo5
            // 
            this.labelTo5.AutoSize = true;
            this.labelTo5.Location = new System.Drawing.Point(78, 153);
            this.labelTo5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTo5.Name = "labelTo5";
            this.labelTo5.Size = new System.Drawing.Size(16, 13);
            this.labelTo5.TabIndex = 433;
            this.labelTo5.Text = "to";
            // 
            // labelTo4
            // 
            this.labelTo4.AutoSize = true;
            this.labelTo4.Location = new System.Drawing.Point(78, 127);
            this.labelTo4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTo4.Name = "labelTo4";
            this.labelTo4.Size = new System.Drawing.Size(16, 13);
            this.labelTo4.TabIndex = 432;
            this.labelTo4.Text = "to";
            // 
            // labelTo3
            // 
            this.labelTo3.AutoSize = true;
            this.labelTo3.Location = new System.Drawing.Point(78, 101);
            this.labelTo3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTo3.Name = "labelTo3";
            this.labelTo3.Size = new System.Drawing.Size(16, 13);
            this.labelTo3.TabIndex = 431;
            this.labelTo3.Text = "to";
            // 
            // labelTo2
            // 
            this.labelTo2.AutoSize = true;
            this.labelTo2.Location = new System.Drawing.Point(78, 75);
            this.labelTo2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTo2.Name = "labelTo2";
            this.labelTo2.Size = new System.Drawing.Size(16, 13);
            this.labelTo2.TabIndex = 430;
            this.labelTo2.Text = "to";
            // 
            // labelPowerCurrency5
            // 
            this.labelPowerCurrency5.AutoSize = true;
            this.labelPowerCurrency5.Location = new System.Drawing.Point(232, 153);
            this.labelPowerCurrency5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPowerCurrency5.Name = "labelPowerCurrency5";
            this.labelPowerCurrency5.Size = new System.Drawing.Size(61, 13);
            this.labelPowerCurrency5.TabIndex = 429;
            this.labelPowerCurrency5.Text = "USD/kW.h";
            // 
            // textBoxScheduleTo5
            // 
            this.textBoxScheduleTo5.AsciiOnly = true;
            this.textBoxScheduleTo5.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleTo5.Location = new System.Drawing.Point(98, 150);
            this.textBoxScheduleTo5.Mask = "00:00";
            this.textBoxScheduleTo5.Name = "textBoxScheduleTo5";
            this.textBoxScheduleTo5.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleTo5.TabIndex = 428;
            this.textBoxScheduleTo5.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleTo5.Leave += new System.EventHandler(this.textBoxScheduleTo5_Leave);
            // 
            // textBoxScheduleFrom5
            // 
            this.textBoxScheduleFrom5.AsciiOnly = true;
            this.textBoxScheduleFrom5.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleFrom5.Location = new System.Drawing.Point(35, 150);
            this.textBoxScheduleFrom5.Mask = "00:00";
            this.textBoxScheduleFrom5.Name = "textBoxScheduleFrom5";
            this.textBoxScheduleFrom5.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleFrom5.TabIndex = 427;
            this.textBoxScheduleFrom5.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleFrom5.Leave += new System.EventHandler(this.textBoxScheduleFrom5_Leave);
            // 
            // textBoxScheduleCost5
            // 
            this.textBoxScheduleCost5.Location = new System.Drawing.Point(188, 150);
            this.textBoxScheduleCost5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxScheduleCost5.Name = "textBoxScheduleCost5";
            this.textBoxScheduleCost5.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleCost5.TabIndex = 426;
            this.textBoxScheduleCost5.WordWrap = false;
            this.textBoxScheduleCost5.Leave += new System.EventHandler(this.textBoxScheduleCost5_Leave);
            // 
            // labelCost5
            // 
            this.labelCost5.AutoSize = true;
            this.labelCost5.Location = new System.Drawing.Point(146, 153);
            this.labelCost5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCost5.Name = "labelCost5";
            this.labelCost5.Size = new System.Drawing.Size(27, 13);
            this.labelCost5.TabIndex = 425;
            this.labelCost5.Text = "cost";
            // 
            // labelFrom5
            // 
            this.labelFrom5.AutoSize = true;
            this.labelFrom5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelFrom5.Location = new System.Drawing.Point(5, 154);
            this.labelFrom5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFrom5.Name = "labelFrom5";
            this.labelFrom5.Size = new System.Drawing.Size(27, 13);
            this.labelFrom5.TabIndex = 424;
            this.labelFrom5.Text = "from";
            this.labelFrom5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPowerCurrency4
            // 
            this.labelPowerCurrency4.AutoSize = true;
            this.labelPowerCurrency4.Location = new System.Drawing.Point(232, 127);
            this.labelPowerCurrency4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPowerCurrency4.Name = "labelPowerCurrency4";
            this.labelPowerCurrency4.Size = new System.Drawing.Size(61, 13);
            this.labelPowerCurrency4.TabIndex = 423;
            this.labelPowerCurrency4.Text = "USD/kW.h";
            // 
            // textBoxScheduleTo4
            // 
            this.textBoxScheduleTo4.AsciiOnly = true;
            this.textBoxScheduleTo4.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleTo4.Location = new System.Drawing.Point(98, 124);
            this.textBoxScheduleTo4.Mask = "00:00";
            this.textBoxScheduleTo4.Name = "textBoxScheduleTo4";
            this.textBoxScheduleTo4.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleTo4.TabIndex = 422;
            this.textBoxScheduleTo4.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleTo4.Leave += new System.EventHandler(this.textBoxScheduleTo4_Leave);
            // 
            // textBoxScheduleFrom4
            // 
            this.textBoxScheduleFrom4.AsciiOnly = true;
            this.textBoxScheduleFrom4.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleFrom4.Location = new System.Drawing.Point(35, 124);
            this.textBoxScheduleFrom4.Mask = "00:00";
            this.textBoxScheduleFrom4.Name = "textBoxScheduleFrom4";
            this.textBoxScheduleFrom4.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleFrom4.TabIndex = 421;
            this.textBoxScheduleFrom4.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleFrom4.Leave += new System.EventHandler(this.textBoxScheduleFrom4_Leave);
            // 
            // textBoxScheduleCost4
            // 
            this.textBoxScheduleCost4.Location = new System.Drawing.Point(188, 124);
            this.textBoxScheduleCost4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxScheduleCost4.Name = "textBoxScheduleCost4";
            this.textBoxScheduleCost4.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleCost4.TabIndex = 420;
            this.textBoxScheduleCost4.WordWrap = false;
            this.textBoxScheduleCost4.Leave += new System.EventHandler(this.textBoxScheduleCost4_Leave);
            // 
            // labelCost4
            // 
            this.labelCost4.AutoSize = true;
            this.labelCost4.Location = new System.Drawing.Point(146, 127);
            this.labelCost4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCost4.Name = "labelCost4";
            this.labelCost4.Size = new System.Drawing.Size(27, 13);
            this.labelCost4.TabIndex = 419;
            this.labelCost4.Text = "cost";
            // 
            // labelFrom4
            // 
            this.labelFrom4.AutoSize = true;
            this.labelFrom4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelFrom4.Location = new System.Drawing.Point(5, 128);
            this.labelFrom4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFrom4.Name = "labelFrom4";
            this.labelFrom4.Size = new System.Drawing.Size(27, 13);
            this.labelFrom4.TabIndex = 418;
            this.labelFrom4.Text = "from";
            this.labelFrom4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPowerCurrency3
            // 
            this.labelPowerCurrency3.AutoSize = true;
            this.labelPowerCurrency3.Location = new System.Drawing.Point(232, 101);
            this.labelPowerCurrency3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPowerCurrency3.Name = "labelPowerCurrency3";
            this.labelPowerCurrency3.Size = new System.Drawing.Size(61, 13);
            this.labelPowerCurrency3.TabIndex = 417;
            this.labelPowerCurrency3.Text = "USD/kW.h";
            // 
            // textBoxScheduleTo3
            // 
            this.textBoxScheduleTo3.AsciiOnly = true;
            this.textBoxScheduleTo3.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleTo3.Location = new System.Drawing.Point(98, 98);
            this.textBoxScheduleTo3.Mask = "00:00";
            this.textBoxScheduleTo3.Name = "textBoxScheduleTo3";
            this.textBoxScheduleTo3.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleTo3.TabIndex = 416;
            this.textBoxScheduleTo3.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleTo3.Leave += new System.EventHandler(this.textBoxScheduleTo3_Leave);
            // 
            // textBoxScheduleFrom3
            // 
            this.textBoxScheduleFrom3.AsciiOnly = true;
            this.textBoxScheduleFrom3.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleFrom3.Location = new System.Drawing.Point(35, 98);
            this.textBoxScheduleFrom3.Mask = "00:00";
            this.textBoxScheduleFrom3.Name = "textBoxScheduleFrom3";
            this.textBoxScheduleFrom3.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleFrom3.TabIndex = 415;
            this.textBoxScheduleFrom3.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleFrom3.Leave += new System.EventHandler(this.textBoxScheduleFrom3_Leave);
            // 
            // textBoxScheduleCost3
            // 
            this.textBoxScheduleCost3.Location = new System.Drawing.Point(188, 98);
            this.textBoxScheduleCost3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxScheduleCost3.Name = "textBoxScheduleCost3";
            this.textBoxScheduleCost3.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleCost3.TabIndex = 414;
            this.textBoxScheduleCost3.WordWrap = false;
            this.textBoxScheduleCost3.Leave += new System.EventHandler(this.textBoxScheduleCost3_Leave);
            // 
            // labelCost3
            // 
            this.labelCost3.AutoSize = true;
            this.labelCost3.Location = new System.Drawing.Point(146, 101);
            this.labelCost3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCost3.Name = "labelCost3";
            this.labelCost3.Size = new System.Drawing.Size(27, 13);
            this.labelCost3.TabIndex = 413;
            this.labelCost3.Text = "cost";
            // 
            // labelFrom3
            // 
            this.labelFrom3.AutoSize = true;
            this.labelFrom3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelFrom3.Location = new System.Drawing.Point(5, 102);
            this.labelFrom3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFrom3.Name = "labelFrom3";
            this.labelFrom3.Size = new System.Drawing.Size(27, 13);
            this.labelFrom3.TabIndex = 412;
            this.labelFrom3.Text = "from";
            this.labelFrom3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPowerCurrency2
            // 
            this.labelPowerCurrency2.AutoSize = true;
            this.labelPowerCurrency2.Location = new System.Drawing.Point(232, 75);
            this.labelPowerCurrency2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPowerCurrency2.Name = "labelPowerCurrency2";
            this.labelPowerCurrency2.Size = new System.Drawing.Size(61, 13);
            this.labelPowerCurrency2.TabIndex = 411;
            this.labelPowerCurrency2.Text = "USD/kW.h";
            // 
            // textBoxScheduleTo2
            // 
            this.textBoxScheduleTo2.AsciiOnly = true;
            this.textBoxScheduleTo2.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleTo2.Location = new System.Drawing.Point(98, 72);
            this.textBoxScheduleTo2.Mask = "00:00";
            this.textBoxScheduleTo2.Name = "textBoxScheduleTo2";
            this.textBoxScheduleTo2.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleTo2.TabIndex = 410;
            this.textBoxScheduleTo2.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleTo2.Leave += new System.EventHandler(this.textBoxScheduleTo2_Leave);
            // 
            // textBoxScheduleFrom2
            // 
            this.textBoxScheduleFrom2.AsciiOnly = true;
            this.textBoxScheduleFrom2.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleFrom2.Location = new System.Drawing.Point(35, 72);
            this.textBoxScheduleFrom2.Mask = "00:00";
            this.textBoxScheduleFrom2.Name = "textBoxScheduleFrom2";
            this.textBoxScheduleFrom2.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleFrom2.TabIndex = 409;
            this.textBoxScheduleFrom2.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleFrom2.Leave += new System.EventHandler(this.textBoxScheduleFrom2_Leave);
            // 
            // textBoxScheduleCost2
            // 
            this.textBoxScheduleCost2.Location = new System.Drawing.Point(188, 72);
            this.textBoxScheduleCost2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxScheduleCost2.Name = "textBoxScheduleCost2";
            this.textBoxScheduleCost2.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleCost2.TabIndex = 408;
            this.textBoxScheduleCost2.WordWrap = false;
            this.textBoxScheduleCost2.Leave += new System.EventHandler(this.textBoxScheduleCost2_Leave);
            // 
            // labelCost2
            // 
            this.labelCost2.AutoSize = true;
            this.labelCost2.Location = new System.Drawing.Point(146, 75);
            this.labelCost2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCost2.Name = "labelCost2";
            this.labelCost2.Size = new System.Drawing.Size(27, 13);
            this.labelCost2.TabIndex = 407;
            this.labelCost2.Text = "cost";
            // 
            // labelFrom2
            // 
            this.labelFrom2.AutoSize = true;
            this.labelFrom2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelFrom2.Location = new System.Drawing.Point(5, 76);
            this.labelFrom2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFrom2.Name = "labelFrom2";
            this.labelFrom2.Size = new System.Drawing.Size(27, 13);
            this.labelFrom2.TabIndex = 406;
            this.labelFrom2.Text = "from";
            this.labelFrom2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPowerCurrency1
            // 
            this.labelPowerCurrency1.AutoSize = true;
            this.labelPowerCurrency1.Location = new System.Drawing.Point(232, 49);
            this.labelPowerCurrency1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPowerCurrency1.Name = "labelPowerCurrency1";
            this.labelPowerCurrency1.Size = new System.Drawing.Size(61, 13);
            this.labelPowerCurrency1.TabIndex = 405;
            this.labelPowerCurrency1.Text = "USD/kW.h";
            // 
            // textBoxScheduleTo1
            // 
            this.textBoxScheduleTo1.AsciiOnly = true;
            this.textBoxScheduleTo1.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleTo1.Location = new System.Drawing.Point(98, 46);
            this.textBoxScheduleTo1.Mask = "00:00";
            this.textBoxScheduleTo1.Name = "textBoxScheduleTo1";
            this.textBoxScheduleTo1.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleTo1.TabIndex = 404;
            this.textBoxScheduleTo1.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleTo1.Leave += new System.EventHandler(this.textBoxScheduleTo1_Leave);
            // 
            // textBoxScheduleFrom1
            // 
            this.textBoxScheduleFrom1.AsciiOnly = true;
            this.textBoxScheduleFrom1.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.textBoxScheduleFrom1.Location = new System.Drawing.Point(35, 47);
            this.textBoxScheduleFrom1.Mask = "00:00";
            this.textBoxScheduleFrom1.Name = "textBoxScheduleFrom1";
            this.textBoxScheduleFrom1.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleFrom1.TabIndex = 403;
            this.textBoxScheduleFrom1.ValidatingType = typeof(System.DateTime);
            this.textBoxScheduleFrom1.Leave += new System.EventHandler(this.textBoxScheduleFrom1_Leave);
            // 
            // textBoxScheduleCost1
            // 
            this.textBoxScheduleCost1.Location = new System.Drawing.Point(188, 46);
            this.textBoxScheduleCost1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxScheduleCost1.Name = "textBoxScheduleCost1";
            this.textBoxScheduleCost1.Size = new System.Drawing.Size(39, 20);
            this.textBoxScheduleCost1.TabIndex = 402;
            this.textBoxScheduleCost1.WordWrap = false;
            this.textBoxScheduleCost1.Leave += new System.EventHandler(this.textBoxScheduleCost1_Leave);
            // 
            // labelCost1
            // 
            this.labelCost1.AutoSize = true;
            this.labelCost1.Location = new System.Drawing.Point(146, 49);
            this.labelCost1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCost1.Name = "labelCost1";
            this.labelCost1.Size = new System.Drawing.Size(27, 13);
            this.labelCost1.TabIndex = 401;
            this.labelCost1.Text = "cost";
            // 
            // labelTo1
            // 
            this.labelTo1.AutoSize = true;
            this.labelTo1.Location = new System.Drawing.Point(78, 49);
            this.labelTo1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTo1.Name = "labelTo1";
            this.labelTo1.Size = new System.Drawing.Size(16, 13);
            this.labelTo1.TabIndex = 399;
            this.labelTo1.Text = "to";
            // 
            // labelFrom1
            // 
            this.labelFrom1.AutoSize = true;
            this.labelFrom1.Location = new System.Drawing.Point(5, 50);
            this.labelFrom1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelFrom1.Name = "labelFrom1";
            this.labelFrom1.Size = new System.Drawing.Size(27, 13);
            this.labelFrom1.TabIndex = 397;
            this.labelFrom1.Text = "from";
            // 
            // label_Schedules2
            // 
            this.label_Schedules2.AutoSize = true;
            this.label_Schedules2.Location = new System.Drawing.Point(232, 22);
            this.label_Schedules2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Schedules2.Name = "label_Schedules2";
            this.label_Schedules2.Size = new System.Drawing.Size(10, 13);
            this.label_Schedules2.TabIndex = 396;
            this.label_Schedules2.Text = ".";
            // 
            // label_Schedules
            // 
            this.label_Schedules.AutoSize = true;
            this.label_Schedules.Location = new System.Drawing.Point(5, 22);
            this.label_Schedules.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Schedules.Name = "label_Schedules";
            this.label_Schedules.Size = new System.Drawing.Size(159, 13);
            this.label_Schedules.TabIndex = 395;
            this.label_Schedules.Text = "Schedules of tariffs for electricity";
            // 
            // comboBoxZones
            // 
            this.comboBoxZones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxZones.FormattingEnabled = true;
            this.comboBoxZones.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.comboBoxZones.Location = new System.Drawing.Point(188, 19);
            this.comboBoxZones.Name = "comboBoxZones";
            this.comboBoxZones.Size = new System.Drawing.Size(39, 21);
            this.comboBoxZones.TabIndex = 394;
            this.comboBoxZones.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxZones_DrawItem);
            this.comboBoxZones.SelectedIndexChanged += new System.EventHandler(this.comboBoxZones_SelectedIndexChanged);
            // 
            // tabPageAdvanced1
            // 
            this.tabPageAdvanced1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageAdvanced1.Controls.Add(this.groupBoxConnection);
            this.tabPageAdvanced1.Controls.Add(this.groupBox_Miners);
            this.tabPageAdvanced1.Controls.Add(this.groupBox1);
            this.tabPageAdvanced1.Location = new System.Drawing.Point(4, 23);
            this.tabPageAdvanced1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageAdvanced1.Name = "tabPageAdvanced1";
            this.tabPageAdvanced1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageAdvanced1.Size = new System.Drawing.Size(669, 491);
            this.tabPageAdvanced1.TabIndex = 2;
            this.tabPageAdvanced1.Text = "Advanced";
            // 
            // groupBoxConnection
            // 
            this.groupBoxConnection.Controls.Add(this.checkBoxEnableProxy);
            this.groupBoxConnection.Controls.Add(this.checkBoxProxySSL);
            this.groupBoxConnection.Location = new System.Drawing.Point(4, 399);
            this.groupBoxConnection.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxConnection.Size = new System.Drawing.Size(657, 51);
            this.groupBoxConnection.TabIndex = 395;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "Connection";
            // 
            // checkBoxEnableProxy
            // 
            this.checkBoxEnableProxy.AutoSize = true;
            this.checkBoxEnableProxy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxEnableProxy.Location = new System.Drawing.Point(11, 19);
            this.checkBoxEnableProxy.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxEnableProxy.Name = "checkBoxEnableProxy";
            this.checkBoxEnableProxy.Size = new System.Drawing.Size(137, 17);
            this.checkBoxEnableProxy.TabIndex = 405;
            this.checkBoxEnableProxy.Text = "Using proxy connection";
            this.checkBoxEnableProxy.UseVisualStyleBackColor = true;
            this.checkBoxEnableProxy.CheckedChanged += new System.EventHandler(this.checkBoxEnableProxy_CheckedChanged);
            // 
            // checkBoxProxySSL
            // 
            this.checkBoxProxySSL.AutoSize = true;
            this.checkBoxProxySSL.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxProxySSL.Location = new System.Drawing.Point(304, 19);
            this.checkBoxProxySSL.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxProxySSL.Name = "checkBoxProxySSL";
            this.checkBoxProxySSL.Size = new System.Drawing.Size(142, 17);
            this.checkBoxProxySSL.TabIndex = 402;
            this.checkBoxProxySSL.Text = "SSL connection to proxy";
            this.checkBoxProxySSL.UseVisualStyleBackColor = true;
            // 
            // groupBox_Miners
            // 
            this.groupBox_Miners.Controls.Add(this.checkBoxMiningFee);
            this.groupBox_Miners.Controls.Add(this.checkBoxShortTerm);
            this.groupBox_Miners.Controls.Add(this.checkBoxLast24hours);
            this.groupBox_Miners.Controls.Add(this.checkBox_orderPrice);
            this.groupBox_Miners.Controls.Add(this.checkBox_withPower);
            this.groupBox_Miners.Controls.Add(this.checkBox_By_profitability_of_all_devices);
            this.groupBox_Miners.Controls.Add(this.label_switching_algorithms);
            this.groupBox_Miners.Controls.Add(this.comboBox_switching_algorithms);
            this.groupBox_Miners.Controls.Add(this.textBox_SwitchProfitabilityThreshold);
            this.groupBox_Miners.Controls.Add(this.label_SwitchProfitabilityThreshold);
            this.groupBox_Miners.Controls.Add(this.checkbox_Group_same_devices);
            this.groupBox_Miners.Location = new System.Drawing.Point(6, 6);
            this.groupBox_Miners.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Miners.Name = "groupBox_Miners";
            this.groupBox_Miners.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Miners.Size = new System.Drawing.Size(657, 153);
            this.groupBox_Miners.TabIndex = 389;
            this.groupBox_Miners.TabStop = false;
            this.groupBox_Miners.Text = "Miners:";
            // 
            // checkBoxMiningFee
            // 
            this.checkBoxMiningFee.AutoSize = true;
            this.checkBoxMiningFee.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxMiningFee.Location = new System.Drawing.Point(10, 130);
            this.checkBoxMiningFee.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxMiningFee.Name = "checkBoxMiningFee";
            this.checkBoxMiningFee.Size = new System.Drawing.Size(187, 17);
            this.checkBoxMiningFee.TabIndex = 422;
            this.checkBoxMiningFee.Text = "Including NiceHash mining fee 2%";
            this.checkBoxMiningFee.UseVisualStyleBackColor = true;
            // 
            // checkBoxShortTerm
            // 
            this.checkBoxShortTerm.AutoSize = true;
            this.checkBoxShortTerm.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxShortTerm.Location = new System.Drawing.Point(10, 107);
            this.checkBoxShortTerm.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxShortTerm.Name = "checkBoxShortTerm";
            this.checkBoxShortTerm.Size = new System.Drawing.Size(257, 17);
            this.checkBoxShortTerm.TabIndex = 421;
            this.checkBoxShortTerm.Text = "Do not react to a short-term change in profitability";
            this.checkBoxShortTerm.UseVisualStyleBackColor = true;
            this.checkBoxShortTerm.CheckedChanged += new System.EventHandler(this.checkBoxShortTerm_CheckedChanged);
            // 
            // checkBoxLast24hours
            // 
            this.checkBoxLast24hours.AutoSize = true;
            this.checkBoxLast24hours.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxLast24hours.Location = new System.Drawing.Point(307, 84);
            this.checkBoxLast24hours.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxLast24hours.Name = "checkBoxLast24hours";
            this.checkBoxLast24hours.Size = new System.Drawing.Size(227, 17);
            this.checkBoxLast24hours.TabIndex = 420;
            this.checkBoxLast24hours.Text = "Use the average price for the last 24 hours";
            this.checkBoxLast24hours.UseVisualStyleBackColor = true;
            this.checkBoxLast24hours.CheckedChanged += new System.EventHandler(this.checkBoxLast24hours_CheckedChanged);
            // 
            // checkBox_orderPrice
            // 
            this.checkBox_orderPrice.AutoSize = true;
            this.checkBox_orderPrice.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_orderPrice.Location = new System.Drawing.Point(11, 84);
            this.checkBox_orderPrice.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_orderPrice.Name = "checkBox_orderPrice";
            this.checkBox_orderPrice.Size = new System.Drawing.Size(116, 17);
            this.checkBox_orderPrice.TabIndex = 419;
            this.checkBox_orderPrice.Text = "Use the order price";
            this.checkBox_orderPrice.UseVisualStyleBackColor = true;
            this.checkBox_orderPrice.CheckedChanged += new System.EventHandler(this.checkBox_orderPrice_CheckedChanged);
            // 
            // checkBox_withPower
            // 
            this.checkBox_withPower.AutoSize = true;
            this.checkBox_withPower.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_withPower.Location = new System.Drawing.Point(307, 61);
            this.checkBox_withPower.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_withPower.Name = "checkBox_withPower";
            this.checkBox_withPower.Size = new System.Drawing.Size(216, 17);
            this.checkBox_withPower.TabIndex = 406;
            this.checkBox_withPower.Text = "Taking into account power consumption";
            this.checkBox_withPower.UseVisualStyleBackColor = true;
            // 
            // checkBox_By_profitability_of_all_devices
            // 
            this.checkBox_By_profitability_of_all_devices.AutoSize = true;
            this.checkBox_By_profitability_of_all_devices.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_By_profitability_of_all_devices.Location = new System.Drawing.Point(432, 36);
            this.checkBox_By_profitability_of_all_devices.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_By_profitability_of_all_devices.Name = "checkBox_By_profitability_of_all_devices";
            this.checkBox_By_profitability_of_all_devices.Size = new System.Drawing.Size(155, 17);
            this.checkBox_By_profitability_of_all_devices.TabIndex = 405;
            this.checkBox_By_profitability_of_all_devices.Text = "By profitability of all devices";
            this.checkBox_By_profitability_of_all_devices.UseVisualStyleBackColor = true;
            // 
            // label_switching_algorithms
            // 
            this.label_switching_algorithms.AutoSize = true;
            this.label_switching_algorithms.Location = new System.Drawing.Point(8, 18);
            this.label_switching_algorithms.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_switching_algorithms.Name = "label_switching_algorithms";
            this.label_switching_algorithms.Size = new System.Drawing.Size(103, 13);
            this.label_switching_algorithms.TabIndex = 404;
            this.label_switching_algorithms.Text = "Switching algorithms";
            this.label_switching_algorithms.Click += new System.EventHandler(this.label2_Click);
            // 
            // comboBox_switching_algorithms
            // 
            this.comboBox_switching_algorithms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_switching_algorithms.FormattingEnabled = true;
            this.comboBox_switching_algorithms.Location = new System.Drawing.Point(11, 34);
            this.comboBox_switching_algorithms.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_switching_algorithms.Name = "comboBox_switching_algorithms";
            this.comboBox_switching_algorithms.Size = new System.Drawing.Size(170, 21);
            this.comboBox_switching_algorithms.TabIndex = 403;
            this.comboBox_switching_algorithms.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_switching_algorithms_DrawItem);
            this.comboBox_switching_algorithms.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // textBox_SwitchProfitabilityThreshold
            // 
            this.textBox_SwitchProfitabilityThreshold.Location = new System.Drawing.Point(394, 34);
            this.textBox_SwitchProfitabilityThreshold.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_SwitchProfitabilityThreshold.Name = "textBox_SwitchProfitabilityThreshold";
            this.textBox_SwitchProfitabilityThreshold.Size = new System.Drawing.Size(34, 20);
            this.textBox_SwitchProfitabilityThreshold.TabIndex = 333;
            this.textBox_SwitchProfitabilityThreshold.TextChanged += new System.EventHandler(this.textBox_SwitchProfitabilityThreshold_TextChanged);
            // 
            // label_SwitchProfitabilityThreshold
            // 
            this.label_SwitchProfitabilityThreshold.AutoSize = true;
            this.label_SwitchProfitabilityThreshold.Location = new System.Drawing.Point(189, 37);
            this.label_SwitchProfitabilityThreshold.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_SwitchProfitabilityThreshold.Name = "label_SwitchProfitabilityThreshold";
            this.label_SwitchProfitabilityThreshold.Size = new System.Drawing.Size(177, 13);
            this.label_SwitchProfitabilityThreshold.TabIndex = 361;
            this.label_SwitchProfitabilityThreshold.Text = "Switching threshold of profitability, %";
            // 
            // checkbox_Group_same_devices
            // 
            this.checkbox_Group_same_devices.AutoSize = true;
            this.checkbox_Group_same_devices.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkbox_Group_same_devices.Location = new System.Drawing.Point(11, 61);
            this.checkbox_Group_same_devices.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkbox_Group_same_devices.Name = "checkbox_Group_same_devices";
            this.checkbox_Group_same_devices.Size = new System.Drawing.Size(123, 17);
            this.checkbox_Group_same_devices.TabIndex = 402;
            this.checkbox_Group_same_devices.Text = "Group same devices";
            this.checkbox_Group_same_devices.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxINTELmonitoring);
            this.groupBox1.Controls.Add(this.labelDisableMonitoring);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionINTEL);
            this.groupBox1.Controls.Add(this.labelDisableDetection);
            this.groupBox1.Controls.Add(this.checkBoxAMDmonitoring);
            this.groupBox1.Controls.Add(this.checkBoxNVMonitoring);
            this.groupBox1.Controls.Add(this.checkBox_show_INTELdevice_manufacturer);
            this.groupBox1.Controls.Add(this.checkBox_Show_memory_temp);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionNVIDIA);
            this.groupBox1.Controls.Add(this.label_restart_nv_lost);
            this.groupBox1.Controls.Add(this.label_show_manufacturer);
            this.groupBox1.Controls.Add(this.checkBoxCheckingCUDA);
            this.groupBox1.Controls.Add(this.checkBox_DisplayConnected);
            this.groupBox1.Controls.Add(this.checkBox_show_AMDdevice_manufacturer);
            this.groupBox1.Controls.Add(this.checkBox_ShowDeviceMemSize);
            this.groupBox1.Controls.Add(this.checkBox_show_NVdevice_manufacturer);
            this.groupBox1.Controls.Add(this.checkBoxDriverWarning);
            this.groupBox1.Controls.Add(this.checkBoxCPUmonitoring);
            this.groupBox1.Controls.Add(this.checkBoxRestartDriver);
            this.groupBox1.Controls.Add(this.checkBoxRestartWindows);
            this.groupBox1.Controls.Add(this.checkbox_Use_OpenHardwareMonitor);
            this.groupBox1.Controls.Add(this.label_devices_count);
            this.groupBox1.Controls.Add(this.comboBox_devices_count);
            this.groupBox1.Controls.Add(this.checkBox_ShowFanAsPercent);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionCPU);
            this.groupBox1.Controls.Add(this.checkBox_Additional_info_about_device);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionAMD);
            this.groupBox1.Location = new System.Drawing.Point(6, 165);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Size = new System.Drawing.Size(658, 228);
            this.groupBox1.TabIndex = 394;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Devices:";
            // 
            // checkBoxINTELmonitoring
            // 
            this.checkBoxINTELmonitoring.AutoSize = true;
            this.checkBoxINTELmonitoring.Location = new System.Drawing.Point(183, 88);
            this.checkBoxINTELmonitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxINTELmonitoring.Name = "checkBoxINTELmonitoring";
            this.checkBoxINTELmonitoring.Size = new System.Drawing.Size(57, 17);
            this.checkBoxINTELmonitoring.TabIndex = 426;
            this.checkBoxINTELmonitoring.Text = "INTEL";
            this.checkBoxINTELmonitoring.UseVisualStyleBackColor = true;
            // 
            // labelDisableMonitoring
            // 
            this.labelDisableMonitoring.AutoSize = true;
            this.labelDisableMonitoring.Location = new System.Drawing.Point(8, 66);
            this.labelDisableMonitoring.Name = "labelDisableMonitoring";
            this.labelDisableMonitoring.Size = new System.Drawing.Size(96, 13);
            this.labelDisableMonitoring.TabIndex = 425;
            this.labelDisableMonitoring.Text = "Disable monitoring:";
            // 
            // checkBox_DisableDetectionINTEL
            // 
            this.checkBox_DisableDetectionINTEL.AutoSize = true;
            this.checkBox_DisableDetectionINTEL.Location = new System.Drawing.Point(183, 42);
            this.checkBox_DisableDetectionINTEL.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionINTEL.Name = "checkBox_DisableDetectionINTEL";
            this.checkBox_DisableDetectionINTEL.Size = new System.Drawing.Size(57, 17);
            this.checkBox_DisableDetectionINTEL.TabIndex = 424;
            this.checkBox_DisableDetectionINTEL.Text = "INTEL";
            this.checkBox_DisableDetectionINTEL.UseVisualStyleBackColor = true;
            this.checkBox_DisableDetectionINTEL.CheckedChanged += new System.EventHandler(this.checkBox_DisableDetectionINTEL_CheckedChanged);
            // 
            // labelDisableDetection
            // 
            this.labelDisableDetection.AutoSize = true;
            this.labelDisableDetection.Location = new System.Drawing.Point(8, 20);
            this.labelDisableDetection.Name = "labelDisableDetection";
            this.labelDisableDetection.Size = new System.Drawing.Size(92, 13);
            this.labelDisableDetection.TabIndex = 423;
            this.labelDisableDetection.Text = "Disable detection:";
            // 
            // checkBoxAMDmonitoring
            // 
            this.checkBoxAMDmonitoring.AutoSize = true;
            this.checkBoxAMDmonitoring.Location = new System.Drawing.Point(129, 88);
            this.checkBoxAMDmonitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxAMDmonitoring.Name = "checkBoxAMDmonitoring";
            this.checkBoxAMDmonitoring.Size = new System.Drawing.Size(50, 17);
            this.checkBoxAMDmonitoring.TabIndex = 410;
            this.checkBoxAMDmonitoring.Text = "AMD";
            this.checkBoxAMDmonitoring.UseVisualStyleBackColor = true;
            // 
            // checkBoxNVMonitoring
            // 
            this.checkBoxNVMonitoring.AutoSize = true;
            this.checkBoxNVMonitoring.Location = new System.Drawing.Point(63, 88);
            this.checkBoxNVMonitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxNVMonitoring.Name = "checkBoxNVMonitoring";
            this.checkBoxNVMonitoring.Size = new System.Drawing.Size(62, 17);
            this.checkBoxNVMonitoring.TabIndex = 409;
            this.checkBoxNVMonitoring.Text = "NVIDIA";
            this.checkBoxNVMonitoring.UseVisualStyleBackColor = true;
            // 
            // checkBox_show_INTELdevice_manufacturer
            // 
            this.checkBox_show_INTELdevice_manufacturer.AutoSize = true;
            this.checkBox_show_INTELdevice_manufacturer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_show_INTELdevice_manufacturer.Location = new System.Drawing.Point(560, 19);
            this.checkBox_show_INTELdevice_manufacturer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_show_INTELdevice_manufacturer.Name = "checkBox_show_INTELdevice_manufacturer";
            this.checkBox_show_INTELdevice_manufacturer.Size = new System.Drawing.Size(57, 17);
            this.checkBox_show_INTELdevice_manufacturer.TabIndex = 422;
            this.checkBox_show_INTELdevice_manufacturer.Text = "INTEL";
            this.checkBox_show_INTELdevice_manufacturer.UseVisualStyleBackColor = true;
            this.checkBox_show_INTELdevice_manufacturer.CheckedChanged += new System.EventHandler(this.checkBox_show_INTELdevice_manufacturer_CheckedChanged);
            // 
            // checkBox_Show_memory_temp
            // 
            this.checkBox_Show_memory_temp.AutoSize = true;
            this.checkBox_Show_memory_temp.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Show_memory_temp.Location = new System.Drawing.Point(302, 42);
            this.checkBox_Show_memory_temp.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Show_memory_temp.Name = "checkBox_Show_memory_temp";
            this.checkBox_Show_memory_temp.Size = new System.Drawing.Size(244, 17);
            this.checkBox_Show_memory_temp.TabIndex = 421;
            this.checkBox_Show_memory_temp.Text = "Show device memory temperature if supported";
            this.checkBox_Show_memory_temp.UseVisualStyleBackColor = true;
            // 
            // checkBox_DisableDetectionNVIDIA
            // 
            this.checkBox_DisableDetectionNVIDIA.AutoSize = true;
            this.checkBox_DisableDetectionNVIDIA.Location = new System.Drawing.Point(63, 42);
            this.checkBox_DisableDetectionNVIDIA.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionNVIDIA.Name = "checkBox_DisableDetectionNVIDIA";
            this.checkBox_DisableDetectionNVIDIA.Size = new System.Drawing.Size(62, 17);
            this.checkBox_DisableDetectionNVIDIA.TabIndex = 390;
            this.checkBox_DisableDetectionNVIDIA.Text = "NVIDIA";
            this.checkBox_DisableDetectionNVIDIA.UseVisualStyleBackColor = true;
            this.checkBox_DisableDetectionNVIDIA.CheckedChanged += new System.EventHandler(this.checkBox_DisableDetectionNVIDIA_CheckedChanged);
            // 
            // label_restart_nv_lost
            // 
            this.label_restart_nv_lost.AutoSize = true;
            this.label_restart_nv_lost.Location = new System.Drawing.Point(299, 181);
            this.label_restart_nv_lost.Name = "label_restart_nv_lost";
            this.label_restart_nv_lost.Size = new System.Drawing.Size(167, 13);
            this.label_restart_nv_lost.TabIndex = 420;
            this.label_restart_nv_lost.Text = "Restart when NVIDIA GPU is lost:";
            // 
            // label_show_manufacturer
            // 
            this.label_show_manufacturer.AutoSize = true;
            this.label_show_manufacturer.Location = new System.Drawing.Point(301, 20);
            this.label_show_manufacturer.Name = "label_show_manufacturer";
            this.label_show_manufacturer.Size = new System.Drawing.Size(137, 13);
            this.label_show_manufacturer.TabIndex = 419;
            this.label_show_manufacturer.Text = "Show device manufacturer:";
            // 
            // checkBoxCheckingCUDA
            // 
            this.checkBoxCheckingCUDA.AutoSize = true;
            this.checkBoxCheckingCUDA.Location = new System.Drawing.Point(10, 180);
            this.checkBoxCheckingCUDA.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxCheckingCUDA.Name = "checkBoxCheckingCUDA";
            this.checkBoxCheckingCUDA.Size = new System.Drawing.Size(227, 17);
            this.checkBoxCheckingCUDA.TabIndex = 417;
            this.checkBoxCheckingCUDA.Text = "Checking NVIDIA GPU on program startup";
            this.checkBoxCheckingCUDA.UseVisualStyleBackColor = true;
            // 
            // checkBox_DisplayConnected
            // 
            this.checkBox_DisplayConnected.AutoSize = true;
            this.checkBox_DisplayConnected.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_DisplayConnected.Location = new System.Drawing.Point(302, 111);
            this.checkBox_DisplayConnected.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisplayConnected.Name = "checkBox_DisplayConnected";
            this.checkBox_DisplayConnected.Size = new System.Drawing.Size(221, 17);
            this.checkBox_DisplayConnected.TabIndex = 416;
            this.checkBox_DisplayConnected.Text = "Show which GPU display is connected to";
            this.checkBox_DisplayConnected.UseVisualStyleBackColor = true;
            // 
            // checkBox_show_AMDdevice_manufacturer
            // 
            this.checkBox_show_AMDdevice_manufacturer.AutoSize = true;
            this.checkBox_show_AMDdevice_manufacturer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_show_AMDdevice_manufacturer.Location = new System.Drawing.Point(506, 19);
            this.checkBox_show_AMDdevice_manufacturer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_show_AMDdevice_manufacturer.Name = "checkBox_show_AMDdevice_manufacturer";
            this.checkBox_show_AMDdevice_manufacturer.Size = new System.Drawing.Size(50, 17);
            this.checkBox_show_AMDdevice_manufacturer.TabIndex = 415;
            this.checkBox_show_AMDdevice_manufacturer.Text = "AMD";
            this.checkBox_show_AMDdevice_manufacturer.UseVisualStyleBackColor = true;
            // 
            // checkBox_ShowDeviceMemSize
            // 
            this.checkBox_ShowDeviceMemSize.AutoSize = true;
            this.checkBox_ShowDeviceMemSize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ShowDeviceMemSize.Location = new System.Drawing.Point(302, 65);
            this.checkBox_ShowDeviceMemSize.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ShowDeviceMemSize.Name = "checkBox_ShowDeviceMemSize";
            this.checkBox_ShowDeviceMemSize.Size = new System.Drawing.Size(148, 17);
            this.checkBox_ShowDeviceMemSize.TabIndex = 414;
            this.checkBox_ShowDeviceMemSize.Text = "Show device memory size";
            this.checkBox_ShowDeviceMemSize.UseVisualStyleBackColor = true;
            // 
            // checkBox_show_NVdevice_manufacturer
            // 
            this.checkBox_show_NVdevice_manufacturer.AutoSize = true;
            this.checkBox_show_NVdevice_manufacturer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_show_NVdevice_manufacturer.Location = new System.Drawing.Point(440, 19);
            this.checkBox_show_NVdevice_manufacturer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_show_NVdevice_manufacturer.Name = "checkBox_show_NVdevice_manufacturer";
            this.checkBox_show_NVdevice_manufacturer.Size = new System.Drawing.Size(62, 17);
            this.checkBox_show_NVdevice_manufacturer.TabIndex = 413;
            this.checkBox_show_NVdevice_manufacturer.Text = "NVIDIA";
            this.checkBox_show_NVdevice_manufacturer.UseVisualStyleBackColor = true;
            this.checkBox_show_NVdevice_manufacturer.CheckedChanged += new System.EventHandler(this.checkBox_show_device_manufacturer_CheckedChanged);
            // 
            // checkBoxDriverWarning
            // 
            this.checkBoxDriverWarning.AutoSize = true;
            this.checkBoxDriverWarning.Location = new System.Drawing.Point(10, 157);
            this.checkBoxDriverWarning.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxDriverWarning.Name = "checkBoxDriverWarning";
            this.checkBoxDriverWarning.Size = new System.Drawing.Size(165, 17);
            this.checkBoxDriverWarning.TabIndex = 412;
            this.checkBoxDriverWarning.Text = "Show Driver Version Warning";
            this.checkBoxDriverWarning.UseVisualStyleBackColor = true;
            // 
            // checkBoxCPUmonitoring
            // 
            this.checkBoxCPUmonitoring.AutoSize = true;
            this.checkBoxCPUmonitoring.Location = new System.Drawing.Point(10, 88);
            this.checkBoxCPUmonitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxCPUmonitoring.Name = "checkBoxCPUmonitoring";
            this.checkBoxCPUmonitoring.Size = new System.Drawing.Size(48, 17);
            this.checkBoxCPUmonitoring.TabIndex = 411;
            this.checkBoxCPUmonitoring.Text = "CPU";
            this.checkBoxCPUmonitoring.UseVisualStyleBackColor = true;
            // 
            // checkBoxRestartDriver
            // 
            this.checkBoxRestartDriver.AutoSize = true;
            this.checkBoxRestartDriver.Location = new System.Drawing.Point(547, 180);
            this.checkBoxRestartDriver.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxRestartDriver.Name = "checkBoxRestartDriver";
            this.checkBoxRestartDriver.Size = new System.Drawing.Size(54, 17);
            this.checkBoxRestartDriver.TabIndex = 408;
            this.checkBoxRestartDriver.Text = "Driver";
            this.checkBoxRestartDriver.UseVisualStyleBackColor = true;
            this.checkBoxRestartDriver.CheckedChanged += new System.EventHandler(this.checkBoxRestartDriver_CheckedChanged);
            // 
            // checkBoxRestartWindows
            // 
            this.checkBoxRestartWindows.AutoSize = true;
            this.checkBoxRestartWindows.Location = new System.Drawing.Point(473, 180);
            this.checkBoxRestartWindows.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxRestartWindows.Name = "checkBoxRestartWindows";
            this.checkBoxRestartWindows.Size = new System.Drawing.Size(70, 17);
            this.checkBoxRestartWindows.TabIndex = 397;
            this.checkBoxRestartWindows.Text = "Windows";
            this.checkBoxRestartWindows.UseVisualStyleBackColor = true;
            this.checkBoxRestartWindows.CheckedChanged += new System.EventHandler(this.checkBoxRestartWindows_CheckedChanged);
            // 
            // checkbox_Use_OpenHardwareMonitor
            // 
            this.checkbox_Use_OpenHardwareMonitor.AutoSize = true;
            this.checkbox_Use_OpenHardwareMonitor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkbox_Use_OpenHardwareMonitor.Location = new System.Drawing.Point(10, 134);
            this.checkbox_Use_OpenHardwareMonitor.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkbox_Use_OpenHardwareMonitor.Name = "checkbox_Use_OpenHardwareMonitor";
            this.checkbox_Use_OpenHardwareMonitor.Size = new System.Drawing.Size(216, 17);
            this.checkbox_Use_OpenHardwareMonitor.TabIndex = 407;
            this.checkbox_Use_OpenHardwareMonitor.Text = "Use OpenHardwareMonitor (CPU, AMD)";
            this.checkbox_Use_OpenHardwareMonitor.UseVisualStyleBackColor = true;
            // 
            // label_devices_count
            // 
            this.label_devices_count.AutoSize = true;
            this.label_devices_count.Location = new System.Drawing.Point(299, 158);
            this.label_devices_count.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_devices_count.Name = "label_devices_count";
            this.label_devices_count.Size = new System.Drawing.Size(107, 13);
            this.label_devices_count.TabIndex = 406;
            this.label_devices_count.Text = "Visible devices count";
            // 
            // comboBox_devices_count
            // 
            this.comboBox_devices_count.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_devices_count.FormattingEnabled = true;
            this.comboBox_devices_count.Location = new System.Drawing.Point(446, 155);
            this.comboBox_devices_count.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_devices_count.Name = "comboBox_devices_count";
            this.comboBox_devices_count.Size = new System.Drawing.Size(41, 21);
            this.comboBox_devices_count.TabIndex = 405;
            this.comboBox_devices_count.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_devices_count_DrawItem);
            // 
            // checkBox_ShowFanAsPercent
            // 
            this.checkBox_ShowFanAsPercent.AutoSize = true;
            this.checkBox_ShowFanAsPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ShowFanAsPercent.Location = new System.Drawing.Point(302, 135);
            this.checkBox_ShowFanAsPercent.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ShowFanAsPercent.Name = "checkBox_ShowFanAsPercent";
            this.checkBox_ShowFanAsPercent.Size = new System.Drawing.Size(144, 17);
            this.checkBox_ShowFanAsPercent.TabIndex = 401;
            this.checkBox_ShowFanAsPercent.Text = "Show fan rpm as percent";
            this.checkBox_ShowFanAsPercent.UseVisualStyleBackColor = true;
            // 
            // checkBox_DisableDetectionCPU
            // 
            this.checkBox_DisableDetectionCPU.AutoSize = true;
            this.checkBox_DisableDetectionCPU.Location = new System.Drawing.Point(11, 42);
            this.checkBox_DisableDetectionCPU.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionCPU.Name = "checkBox_DisableDetectionCPU";
            this.checkBox_DisableDetectionCPU.Size = new System.Drawing.Size(48, 17);
            this.checkBox_DisableDetectionCPU.TabIndex = 399;
            this.checkBox_DisableDetectionCPU.Text = "CPU";
            this.checkBox_DisableDetectionCPU.UseVisualStyleBackColor = true;
            this.checkBox_DisableDetectionCPU.CheckedChanged += new System.EventHandler(this.checkBox_DisableDetectionCPU_CheckedChanged);
            // 
            // checkBox_Additional_info_about_device
            // 
            this.checkBox_Additional_info_about_device.AutoSize = true;
            this.checkBox_Additional_info_about_device.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Additional_info_about_device.Location = new System.Drawing.Point(302, 89);
            this.checkBox_Additional_info_about_device.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Additional_info_about_device.Name = "checkBox_Additional_info_about_device";
            this.checkBox_Additional_info_about_device.Size = new System.Drawing.Size(157, 17);
            this.checkBox_Additional_info_about_device.TabIndex = 398;
            this.checkBox_Additional_info_about_device.Text = "Additional info about device";
            this.checkBox_Additional_info_about_device.UseVisualStyleBackColor = true;
            // 
            // checkBox_DisableDetectionAMD
            // 
            this.checkBox_DisableDetectionAMD.AutoSize = true;
            this.checkBox_DisableDetectionAMD.Location = new System.Drawing.Point(129, 42);
            this.checkBox_DisableDetectionAMD.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionAMD.Name = "checkBox_DisableDetectionAMD";
            this.checkBox_DisableDetectionAMD.Size = new System.Drawing.Size(50, 17);
            this.checkBox_DisableDetectionAMD.TabIndex = 391;
            this.checkBox_DisableDetectionAMD.Text = "AMD";
            this.checkBox_DisableDetectionAMD.UseVisualStyleBackColor = true;
            this.checkBox_DisableDetectionAMD.CheckedChanged += new System.EventHandler(this.checkBox_DisableDetectionAMD_CheckedChanged);
            // 
            // tabPageDevicesAlgos
            // 
            this.tabPageDevicesAlgos.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDevicesAlgos.Controls.Add(this.button_Lite_Algo);
            this.tabPageDevicesAlgos.Controls.Add(this.button_ZIL_additional_mining);
            this.tabPageDevicesAlgos.Controls.Add(this.checkBoxHideUnused);
            this.tabPageDevicesAlgos.Controls.Add(this.checkBox_Disable_extra_launch_parameter_checking);
            this.tabPageDevicesAlgos.Controls.Add(this.groupBoxAlgorithmSettings);
            this.tabPageDevicesAlgos.Controls.Add(this.algorithmSettingsControl1);
            this.tabPageDevicesAlgos.Controls.Add(this.devicesListViewEnableControl1);
            this.tabPageDevicesAlgos.Location = new System.Drawing.Point(4, 23);
            this.tabPageDevicesAlgos.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageDevicesAlgos.Name = "tabPageDevicesAlgos";
            this.tabPageDevicesAlgos.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageDevicesAlgos.Size = new System.Drawing.Size(669, 491);
            this.tabPageDevicesAlgos.TabIndex = 1;
            this.tabPageDevicesAlgos.Text = "Devices/Algorithms";
            // 
            // button_Lite_Algo
            // 
            this.button_Lite_Algo.Location = new System.Drawing.Point(378, 220);
            this.button_Lite_Algo.Name = "button_Lite_Algo";
            this.button_Lite_Algo.Size = new System.Drawing.Size(285, 23);
            this.button_Lite_Algo.TabIndex = 404;
            this.button_Lite_Algo.Text = "\"Lite\" algorithms mining settings";
            this.button_Lite_Algo.UseVisualStyleBackColor = true;
            this.button_Lite_Algo.Click += new System.EventHandler(this.button_Lite_Algo_Click);
            // 
            // button_ZIL_additional_mining
            // 
            this.button_ZIL_additional_mining.Location = new System.Drawing.Point(378, 191);
            this.button_ZIL_additional_mining.Name = "button_ZIL_additional_mining";
            this.button_ZIL_additional_mining.Size = new System.Drawing.Size(285, 23);
            this.button_ZIL_additional_mining.TabIndex = 403;
            this.button_ZIL_additional_mining.Text = "Additional Zilliqua (ZIL) mining settings";
            this.button_ZIL_additional_mining.UseVisualStyleBackColor = true;
            this.button_ZIL_additional_mining.Click += new System.EventHandler(this.button_ZIL_additional_mining_Click);
            // 
            // checkBoxHideUnused
            // 
            this.checkBoxHideUnused.AutoSize = true;
            this.checkBoxHideUnused.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxHideUnused.Location = new System.Drawing.Point(8, 220);
            this.checkBoxHideUnused.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxHideUnused.Name = "checkBoxHideUnused";
            this.checkBoxHideUnused.Size = new System.Drawing.Size(136, 17);
            this.checkBoxHideUnused.TabIndex = 400;
            this.checkBoxHideUnused.Text = "Hide unused algorithms";
            this.checkBoxHideUnused.UseVisualStyleBackColor = true;
            this.checkBoxHideUnused.CheckedChanged += new System.EventHandler(this.checkBoxHideUnused_CheckedChanged);
            // 
            // checkBox_Disable_extra_launch_parameter_checking
            // 
            this.checkBox_Disable_extra_launch_parameter_checking.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_Disable_extra_launch_parameter_checking.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Disable_extra_launch_parameter_checking.Location = new System.Drawing.Point(387, 156);
            this.checkBox_Disable_extra_launch_parameter_checking.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Disable_extra_launch_parameter_checking.Name = "checkBox_Disable_extra_launch_parameter_checking";
            this.checkBox_Disable_extra_launch_parameter_checking.Size = new System.Drawing.Size(270, 27);
            this.checkBox_Disable_extra_launch_parameter_checking.TabIndex = 399;
            this.checkBox_Disable_extra_launch_parameter_checking.Text = "Disable extra launch parameter checking";
            this.checkBox_Disable_extra_launch_parameter_checking.UseVisualStyleBackColor = true;
            this.checkBox_Disable_extra_launch_parameter_checking.CheckedChanged += new System.EventHandler(this.checkBox_Disable_extra_launch_parameter_checking_CheckedChanged);
            // 
            // groupBoxAlgorithmSettings
            // 
            this.groupBoxAlgorithmSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAlgorithmSettings.Controls.Add(this.algorithmsListView1);
            this.groupBoxAlgorithmSettings.Location = new System.Drawing.Point(6, 248);
            this.groupBoxAlgorithmSettings.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxAlgorithmSettings.Name = "groupBoxAlgorithmSettings";
            this.groupBoxAlgorithmSettings.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxAlgorithmSettings.Size = new System.Drawing.Size(657, 301);
            this.groupBoxAlgorithmSettings.TabIndex = 395;
            this.groupBoxAlgorithmSettings.TabStop = false;
            this.groupBoxAlgorithmSettings.Text = "Algorithm settings for selected device:";
            // 
            // algorithmsListView1
            // 
            this.algorithmsListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.algorithmsListView1.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.algorithmsListView1.BackColor = System.Drawing.SystemColors.Control;
            this.algorithmsListView1.BenchmarkCalculation = null;
            this.algorithmsListView1.ComunicationInterface = null;
            this.algorithmsListView1.IsInBenchmark = false;
            this.algorithmsListView1.Location = new System.Drawing.Point(6, 21);
            this.algorithmsListView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.algorithmsListView1.Name = "algorithmsListView1";
            this.algorithmsListView1.Size = new System.Drawing.Size(645, 217);
            this.algorithmsListView1.TabIndex = 2;
            // 
            // algorithmSettingsControl1
            // 
            this.algorithmSettingsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.algorithmSettingsControl1.Location = new System.Drawing.Point(378, -1);
            this.algorithmSettingsControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.algorithmSettingsControl1.Name = "algorithmSettingsControl1";
            this.algorithmSettingsControl1.Size = new System.Drawing.Size(285, 193);
            this.algorithmSettingsControl1.TabIndex = 396;
            // 
            // devicesListViewEnableControl1
            // 
            this.devicesListViewEnableControl1.BackColor = System.Drawing.SystemColors.Control;
            this.devicesListViewEnableControl1.BenchmarkCalculation = null;
            this.devicesListViewEnableControl1.FirstColumnText = "Enabled";
            this.devicesListViewEnableControl1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.devicesListViewEnableControl1.IsInBenchmark = false;
            this.devicesListViewEnableControl1.IsMining = false;
            this.devicesListViewEnableControl1.Location = new System.Drawing.Point(8, 8);
            this.devicesListViewEnableControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.devicesListViewEnableControl1.Name = "devicesListViewEnableControl1";
            this.devicesListViewEnableControl1.SaveToGeneralConfig = false;
            this.devicesListViewEnableControl1.Size = new System.Drawing.Size(348, 204);
            this.devicesListViewEnableControl1.TabIndex = 397;
            // 
            // tabPageOverClock
            // 
            this.tabPageOverClock.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageOverClock.Controls.Add(this.checkBoxHideUnused2);
            this.tabPageOverClock.Controls.Add(this.checkBox_ABDefault_program_closing);
            this.tabPageOverClock.Controls.Add(this.checkBox_ABDefault_mining_stopped);
            this.tabPageOverClock.Controls.Add(this.linkLabel3);
            this.tabPageOverClock.Controls.Add(this.checkBox_ABMinimize);
            this.tabPageOverClock.Controls.Add(this.checkBox_ABEnableOverclock);
            this.tabPageOverClock.Controls.Add(this.groupBoxOverClockSettings);
            this.tabPageOverClock.Controls.Add(this.devicesListViewEnableControl2);
            this.tabPageOverClock.Location = new System.Drawing.Point(4, 23);
            this.tabPageOverClock.Name = "tabPageOverClock";
            this.tabPageOverClock.Size = new System.Drawing.Size(669, 491);
            this.tabPageOverClock.TabIndex = 5;
            this.tabPageOverClock.Text = "OverClock";
            // 
            // checkBoxHideUnused2
            // 
            this.checkBoxHideUnused2.AutoSize = true;
            this.checkBoxHideUnused2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxHideUnused2.Location = new System.Drawing.Point(371, 162);
            this.checkBoxHideUnused2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxHideUnused2.Name = "checkBoxHideUnused2";
            this.checkBoxHideUnused2.Size = new System.Drawing.Size(136, 17);
            this.checkBoxHideUnused2.TabIndex = 409;
            this.checkBoxHideUnused2.Text = "Hide unused algorithms";
            this.checkBoxHideUnused2.UseVisualStyleBackColor = true;
            this.checkBoxHideUnused2.CheckedChanged += new System.EventHandler(this.checkBoxHideUnused2_CheckedChanged);
            // 
            // checkBox_ABDefault_program_closing
            // 
            this.checkBox_ABDefault_program_closing.AutoSize = true;
            this.checkBox_ABDefault_program_closing.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ABDefault_program_closing.Location = new System.Drawing.Point(371, 85);
            this.checkBox_ABDefault_program_closing.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ABDefault_program_closing.Name = "checkBox_ABDefault_program_closing";
            this.checkBox_ABDefault_program_closing.Size = new System.Drawing.Size(227, 17);
            this.checkBox_ABDefault_program_closing.TabIndex = 408;
            this.checkBox_ABDefault_program_closing.Text = "Reset by default when closing the program";
            this.checkBox_ABDefault_program_closing.UseVisualStyleBackColor = true;
            // 
            // checkBox_ABDefault_mining_stopped
            // 
            this.checkBox_ABDefault_mining_stopped.AutoSize = true;
            this.checkBox_ABDefault_mining_stopped.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ABDefault_mining_stopped.Location = new System.Drawing.Point(371, 62);
            this.checkBox_ABDefault_mining_stopped.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ABDefault_mining_stopped.Name = "checkBox_ABDefault_mining_stopped";
            this.checkBox_ABDefault_mining_stopped.Size = new System.Drawing.Size(201, 17);
            this.checkBox_ABDefault_mining_stopped.TabIndex = 407;
            this.checkBox_ABDefault_mining_stopped.Text = "Reset by default after mining stopped";
            this.checkBox_ABDefault_mining_stopped.UseVisualStyleBackColor = true;
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel3.Location = new System.Drawing.Point(368, 112);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(55, 13);
            this.linkLabel3.TabIndex = 406;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "linkLabel3";
            this.linkLabel3.Click += new System.EventHandler(this.linkLabel3_Click);
            // 
            // checkBox_ABMinimize
            // 
            this.checkBox_ABMinimize.AutoSize = true;
            this.checkBox_ABMinimize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ABMinimize.Location = new System.Drawing.Point(371, 39);
            this.checkBox_ABMinimize.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ABMinimize.Name = "checkBox_ABMinimize";
            this.checkBox_ABMinimize.Size = new System.Drawing.Size(143, 17);
            this.checkBox_ABMinimize.TabIndex = 404;
            this.checkBox_ABMinimize.Text = "Minimize MSI Afterburner";
            this.checkBox_ABMinimize.UseVisualStyleBackColor = true;
            // 
            // checkBox_ABEnableOverclock
            // 
            this.checkBox_ABEnableOverclock.AutoSize = true;
            this.checkBox_ABEnableOverclock.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ABEnableOverclock.Location = new System.Drawing.Point(371, 16);
            this.checkBox_ABEnableOverclock.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ABEnableOverclock.Name = "checkBox_ABEnableOverclock";
            this.checkBox_ABEnableOverclock.Size = new System.Drawing.Size(109, 17);
            this.checkBox_ABEnableOverclock.TabIndex = 401;
            this.checkBox_ABEnableOverclock.Text = "Enable overclock";
            this.checkBox_ABEnableOverclock.UseVisualStyleBackColor = true;
            this.checkBox_ABEnableOverclock.CheckedChanged += new System.EventHandler(this.checkBox_ABEnableOverclock_CheckedChanged);
            // 
            // groupBoxOverClockSettings
            // 
            this.groupBoxOverClockSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOverClockSettings.Controls.Add(this.algorithmsListViewOverClock1);
            this.groupBoxOverClockSettings.Location = new System.Drawing.Point(4, 197);
            this.groupBoxOverClockSettings.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxOverClockSettings.Name = "groupBoxOverClockSettings";
            this.groupBoxOverClockSettings.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxOverClockSettings.Size = new System.Drawing.Size(662, 291);
            this.groupBoxOverClockSettings.TabIndex = 400;
            this.groupBoxOverClockSettings.TabStop = false;
            this.groupBoxOverClockSettings.Text = "Overclock settings for selected device:";
            // 
            // algorithmsListViewOverClock1
            // 
            this.algorithmsListViewOverClock1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.algorithmsListViewOverClock1.BackColor = System.Drawing.SystemColors.Control;
            this.algorithmsListViewOverClock1.ComunicationInterface = null;
            this.algorithmsListViewOverClock1.Location = new System.Drawing.Point(5, 19);
            this.algorithmsListViewOverClock1.Name = "algorithmsListViewOverClock1";
            this.algorithmsListViewOverClock1.Size = new System.Drawing.Size(652, 266);
            this.algorithmsListViewOverClock1.TabIndex = 399;
            // 
            // devicesListViewEnableControl2
            // 
            this.devicesListViewEnableControl2.BackColor = System.Drawing.SystemColors.Control;
            this.devicesListViewEnableControl2.BenchmarkCalculation = null;
            this.devicesListViewEnableControl2.FirstColumnText = "Enabled";
            this.devicesListViewEnableControl2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.devicesListViewEnableControl2.IsInBenchmark = false;
            this.devicesListViewEnableControl2.IsMining = false;
            this.devicesListViewEnableControl2.Location = new System.Drawing.Point(6, 16);
            this.devicesListViewEnableControl2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.devicesListViewEnableControl2.Name = "devicesListViewEnableControl2";
            this.devicesListViewEnableControl2.SaveToGeneralConfig = false;
            this.devicesListViewEnableControl2.Size = new System.Drawing.Size(348, 163);
            this.devicesListViewEnableControl2.TabIndex = 398;
            this.devicesListViewEnableControl2.Load += new System.EventHandler(this.devicesListViewEnableControl2_Load);
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageAbout.Controls.Add(this.groupBoxBackup);
            this.tabPageAbout.Controls.Add(this.groupBoxUpdates);
            this.tabPageAbout.Controls.Add(this.groupBoxInfo);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 23);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Size = new System.Drawing.Size(669, 491);
            this.tabPageAbout.TabIndex = 3;
            this.tabPageAbout.Text = "About";
            // 
            // groupBoxBackup
            // 
            this.groupBoxBackup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxBackup.Controls.Add(this.checkBox_BackupBeforeUpdate);
            this.groupBoxBackup.Controls.Add(this.labelBackupCopy);
            this.groupBoxBackup.Controls.Add(this.buttonRestoreBackup);
            this.groupBoxBackup.Controls.Add(this.buttonCreateBackup);
            this.groupBoxBackup.Location = new System.Drawing.Point(4, 131);
            this.groupBoxBackup.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxBackup.Name = "groupBoxBackup";
            this.groupBoxBackup.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxBackup.Size = new System.Drawing.Size(659, 67);
            this.groupBoxBackup.TabIndex = 395;
            this.groupBoxBackup.TabStop = false;
            this.groupBoxBackup.Text = "Backup copies";
            // 
            // checkBox_BackupBeforeUpdate
            // 
            this.checkBox_BackupBeforeUpdate.AutoSize = true;
            this.checkBox_BackupBeforeUpdate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_BackupBeforeUpdate.Location = new System.Drawing.Point(9, 41);
            this.checkBox_BackupBeforeUpdate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_BackupBeforeUpdate.Name = "checkBox_BackupBeforeUpdate";
            this.checkBox_BackupBeforeUpdate.Size = new System.Drawing.Size(165, 17);
            this.checkBox_BackupBeforeUpdate.TabIndex = 408;
            this.checkBox_BackupBeforeUpdate.Text = "Create backup before update";
            this.checkBox_BackupBeforeUpdate.UseVisualStyleBackColor = true;
            // 
            // labelBackupCopy
            // 
            this.labelBackupCopy.AutoSize = true;
            this.labelBackupCopy.Location = new System.Drawing.Point(6, 19);
            this.labelBackupCopy.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBackupCopy.Name = "labelBackupCopy";
            this.labelBackupCopy.Size = new System.Drawing.Size(131, 13);
            this.labelBackupCopy.TabIndex = 407;
            this.labelBackupCopy.Text = "No available backup copy";
            // 
            // buttonRestoreBackup
            // 
            this.buttonRestoreBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRestoreBackup.Location = new System.Drawing.Point(397, 14);
            this.buttonRestoreBackup.Name = "buttonRestoreBackup";
            this.buttonRestoreBackup.Size = new System.Drawing.Size(118, 23);
            this.buttonRestoreBackup.TabIndex = 361;
            this.buttonRestoreBackup.Text = "Restore from backup";
            this.buttonRestoreBackup.UseVisualStyleBackColor = true;
            this.buttonRestoreBackup.Click += new System.EventHandler(this.buttonRestoreBackup_Click);
            // 
            // buttonCreateBackup
            // 
            this.buttonCreateBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCreateBackup.Location = new System.Drawing.Point(277, 14);
            this.buttonCreateBackup.Name = "buttonCreateBackup";
            this.buttonCreateBackup.Size = new System.Drawing.Size(110, 23);
            this.buttonCreateBackup.TabIndex = 359;
            this.buttonCreateBackup.Text = "Create backup";
            this.buttonCreateBackup.UseVisualStyleBackColor = true;
            this.buttonCreateBackup.Click += new System.EventHandler(this.buttonCreateBackup_Click);
            // 
            // groupBoxUpdates
            // 
            this.groupBoxUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxUpdates.Controls.Add(this.checkBoxAutoupdate);
            this.groupBoxUpdates.Controls.Add(this.labelCheckforprogramupdatesevery);
            this.groupBoxUpdates.Controls.Add(this.comboBoxCheckforprogramupdatesevery);
            this.groupBoxUpdates.Controls.Add(this.linkLabelCurrentVersion);
            this.groupBoxUpdates.Controls.Add(this.linkLabelNewVersion2);
            this.groupBoxUpdates.Controls.Add(this.buttonUpdate);
            this.groupBoxUpdates.Controls.Add(this.buttonCheckNewVersion);
            this.groupBoxUpdates.Controls.Add(this.progressBarUpdate);
            this.groupBoxUpdates.Location = new System.Drawing.Point(4, 204);
            this.groupBoxUpdates.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxUpdates.Name = "groupBoxUpdates";
            this.groupBoxUpdates.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxUpdates.Size = new System.Drawing.Size(659, 75);
            this.groupBoxUpdates.TabIndex = 394;
            this.groupBoxUpdates.TabStop = false;
            this.groupBoxUpdates.Text = "Program updates";
            // 
            // checkBoxAutoupdate
            // 
            this.checkBoxAutoupdate.AutoSize = true;
            this.checkBoxAutoupdate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxAutoupdate.Location = new System.Drawing.Point(277, 45);
            this.checkBoxAutoupdate.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxAutoupdate.Name = "checkBoxAutoupdate";
            this.checkBoxAutoupdate.Size = new System.Drawing.Size(84, 17);
            this.checkBoxAutoupdate.TabIndex = 406;
            this.checkBoxAutoupdate.Text = "Auto update";
            this.checkBoxAutoupdate.UseVisualStyleBackColor = true;
            // 
            // labelCheckforprogramupdatesevery
            // 
            this.labelCheckforprogramupdatesevery.AutoSize = true;
            this.labelCheckforprogramupdatesevery.Location = new System.Drawing.Point(6, 46);
            this.labelCheckforprogramupdatesevery.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCheckforprogramupdatesevery.Name = "labelCheckforprogramupdatesevery";
            this.labelCheckforprogramupdatesevery.Size = new System.Drawing.Size(164, 13);
            this.labelCheckforprogramupdatesevery.TabIndex = 405;
            this.labelCheckforprogramupdatesevery.Text = "Check for program updates every";
            // 
            // comboBoxCheckforprogramupdatesevery
            // 
            this.comboBoxCheckforprogramupdatesevery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCheckforprogramupdatesevery.FormattingEnabled = true;
            this.comboBoxCheckforprogramupdatesevery.Location = new System.Drawing.Point(196, 43);
            this.comboBoxCheckforprogramupdatesevery.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBoxCheckforprogramupdatesevery.Name = "comboBoxCheckforprogramupdatesevery";
            this.comboBoxCheckforprogramupdatesevery.Size = new System.Drawing.Size(75, 21);
            this.comboBoxCheckforprogramupdatesevery.TabIndex = 404;
            this.comboBoxCheckforprogramupdatesevery.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxCheckforprogramupdatesevery_DrawItem);
            // 
            // linkLabelCurrentVersion
            // 
            this.linkLabelCurrentVersion.AutoSize = true;
            this.linkLabelCurrentVersion.Location = new System.Drawing.Point(6, 19);
            this.linkLabelCurrentVersion.Name = "linkLabelCurrentVersion";
            this.linkLabelCurrentVersion.Size = new System.Drawing.Size(55, 13);
            this.linkLabelCurrentVersion.TabIndex = 363;
            this.linkLabelCurrentVersion.TabStop = true;
            this.linkLabelCurrentVersion.Text = "linkLabel1";
            this.linkLabelCurrentVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelCurrentVersion_LinkClicked);
            this.linkLabelCurrentVersion.MouseEnter += new System.EventHandler(this.linkLabelCurrentVersion_MouseEnter);
            // 
            // linkLabelNewVersion2
            // 
            this.linkLabelNewVersion2.AutoSize = true;
            this.linkLabelNewVersion2.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelNewVersion2.Location = new System.Drawing.Point(394, 19);
            this.linkLabelNewVersion2.Name = "linkLabelNewVersion2";
            this.linkLabelNewVersion2.Size = new System.Drawing.Size(55, 13);
            this.linkLabelNewVersion2.TabIndex = 362;
            this.linkLabelNewVersion2.TabStop = true;
            this.linkLabelNewVersion2.Text = "linkLabel2";
            this.linkLabelNewVersion2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelNewVersion_LinkClicked);
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUpdate.Location = new System.Drawing.Point(536, 14);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(118, 23);
            this.buttonUpdate.TabIndex = 361;
            this.buttonUpdate.Text = "Update now";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // buttonCheckNewVersion
            // 
            this.buttonCheckNewVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCheckNewVersion.Location = new System.Drawing.Point(277, 14);
            this.buttonCheckNewVersion.Name = "buttonCheckNewVersion";
            this.buttonCheckNewVersion.Size = new System.Drawing.Size(110, 23);
            this.buttonCheckNewVersion.TabIndex = 359;
            this.buttonCheckNewVersion.Text = "Check now";
            this.buttonCheckNewVersion.UseVisualStyleBackColor = true;
            this.buttonCheckNewVersion.Click += new System.EventHandler(this.buttonCheckNewVersion_Click);
            // 
            // progressBarUpdate
            // 
            this.progressBarUpdate.CustomText = "";
            this.progressBarUpdate.Location = new System.Drawing.Point(536, 14);
            this.progressBarUpdate.Margin = new System.Windows.Forms.Padding(0);
            this.progressBarUpdate.Name = "progressBarUpdate";
            this.progressBarUpdate.ProgressColor = System.Drawing.Color.Green;
            this.progressBarUpdate.Size = new System.Drawing.Size(118, 23);
            this.progressBarUpdate.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarUpdate.TabIndex = 396;
            this.progressBarUpdate.TextColor = System.Drawing.Color.Black;
            this.progressBarUpdate.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.progressBarUpdate.VisualMode = ProgressBarSample.ProgressBarDisplayMode.Percentage;
            // 
            // groupBoxInfo
            // 
            this.groupBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInfo.Controls.Add(this.checkBoxHistory);
            this.groupBoxInfo.Controls.Add(this.buttonHistory);
            this.groupBoxInfo.Controls.Add(this.richTextBoxInfo);
            this.groupBoxInfo.Controls.Add(this.buttonLicence);
            this.groupBoxInfo.Location = new System.Drawing.Point(4, 3);
            this.groupBoxInfo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxInfo.Size = new System.Drawing.Size(659, 122);
            this.groupBoxInfo.TabIndex = 393;
            this.groupBoxInfo.TabStop = false;
            this.groupBoxInfo.Text = "Info";
            // 
            // checkBoxHistory
            // 
            this.checkBoxHistory.AutoSize = true;
            this.checkBoxHistory.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxHistory.Location = new System.Drawing.Point(151, 90);
            this.checkBoxHistory.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxHistory.Name = "checkBoxHistory";
            this.checkBoxHistory.Size = new System.Drawing.Size(142, 17);
            this.checkBoxHistory.TabIndex = 409;
            this.checkBoxHistory.Text = "View history after update";
            this.checkBoxHistory.UseVisualStyleBackColor = true;
            // 
            // buttonHistory
            // 
            this.buttonHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHistory.Location = new System.Drawing.Point(5, 86);
            this.buttonHistory.Name = "buttonHistory";
            this.buttonHistory.Size = new System.Drawing.Size(128, 23);
            this.buttonHistory.TabIndex = 359;
            this.buttonHistory.Text = "View history";
            this.buttonHistory.UseVisualStyleBackColor = true;
            this.buttonHistory.Click += new System.EventHandler(this.buttonHistory_Click);
            // 
            // richTextBoxInfo
            // 
            this.richTextBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInfo.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxInfo.Location = new System.Drawing.Point(5, 19);
            this.richTextBoxInfo.Name = "richTextBoxInfo";
            this.richTextBoxInfo.Size = new System.Drawing.Size(649, 32);
            this.richTextBoxInfo.TabIndex = 358;
            this.richTextBoxInfo.Text = "";
            this.richTextBoxInfo.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBoxInfo_LinkClicked);
            // 
            // buttonLicence
            // 
            this.buttonLicence.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLicence.Location = new System.Drawing.Point(5, 57);
            this.buttonLicence.Name = "buttonLicence";
            this.buttonLicence.Size = new System.Drawing.Size(128, 23);
            this.buttonLicence.TabIndex = 357;
            this.buttonLicence.Text = "View licence";
            this.buttonLicence.UseVisualStyleBackColor = true;
            this.buttonLicence.Click += new System.EventHandler(this.buttonLicence_Click);
            // 
            // Form_Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 561);
            this.Controls.Add(this.tabControlGeneral);
            this.Controls.Add(this.buttonDefaults);
            this.Controls.Add(this.buttonSaveClose);
            this.Controls.Add(this.buttonCloseNoSave);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 600);
            this.Name = "Form_Settings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Settings";
            this.Deactivate += new System.EventHandler(this.Form_Settings_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
            this.ResizeBegin += new System.EventHandler(this.Form_Settings_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form_Settings_ResizeEnd);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_Settings_Paint);
            this.tabControlGeneral.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.groupBox_RigManager.ResumeLayout(false);
            this.groupBox_RigManager.PerformLayout();
            this.groupBoxServer.ResumeLayout(false);
            this.groupBoxServer.PerformLayout();
            this.groupBoxAPIkeys.ResumeLayout(false);
            this.groupBoxAPIkeys.PerformLayout();
            this.groupBox_Idle.ResumeLayout(false);
            this.groupBox_Idle.PerformLayout();
            this.groupBox_Main.ResumeLayout(false);
            this.groupBox_Main.PerformLayout();
            this.groupBox_Misc.ResumeLayout(false);
            this.groupBox_Misc.PerformLayout();
            this.groupBox_Logging.ResumeLayout(false);
            this.groupBox_Logging.PerformLayout();
            this.groupBox_Localization.ResumeLayout(false);
            this.groupBox_Localization.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.tabPagePower.ResumeLayout(false);
            this.groupBox_additionally.ResumeLayout(false);
            this.groupBox_additionally.PerformLayout();
            this.groupBoxTariffs.ResumeLayout(false);
            this.groupBoxTariffs.PerformLayout();
            this.tabPageAdvanced1.ResumeLayout(false);
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            this.groupBox_Miners.ResumeLayout(false);
            this.groupBox_Miners.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageDevicesAlgos.ResumeLayout(false);
            this.tabPageDevicesAlgos.PerformLayout();
            this.groupBoxAlgorithmSettings.ResumeLayout(false);
            this.tabPageOverClock.ResumeLayout(false);
            this.tabPageOverClock.PerformLayout();
            this.groupBoxOverClockSettings.ResumeLayout(false);
            this.tabPageAbout.ResumeLayout(false);
            this.groupBoxBackup.ResumeLayout(false);
            this.groupBoxBackup.PerformLayout();
            this.groupBoxUpdates.ResumeLayout(false);
            this.groupBoxUpdates.PerformLayout();
            this.groupBoxInfo.ResumeLayout(false);
            this.groupBoxInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSaveClose;
        private System.Windows.Forms.Button buttonDefaults;
        private System.Windows.Forms.Button buttonCloseNoSave;

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.GroupBox groupBox_Main;
        private System.Windows.Forms.CheckBox checkBox_Force_mining_if_nonprofitable;
        private System.Windows.Forms.Label label_TimeUnit;
        private System.Windows.Forms.ComboBox comboBox_TimeUnit;
        private System.Windows.Forms.TextBox textBox_MinProfit;
        private System.Windows.Forms.Label label_MinProfit;
        private System.Windows.Forms.GroupBox groupBox_Misc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_AutoStartMiningDelay;
        private System.Windows.Forms.Label label_AutoStartMiningDelay;
        private System.Windows.Forms.ComboBox comboBox_ColorProfile;
        private System.Windows.Forms.CheckBox checkBox_Send_actual_version_info;
        private System.Windows.Forms.CheckBox checkBox_Allow_remote_management;
        private System.Windows.Forms.CheckBox checkBox_MinimizeMiningWindows;
        private System.Windows.Forms.CheckBox checkBox_RunAtStartup;
        private System.Windows.Forms.CheckBox checkBox_AllowMultipleInstances;
        private System.Windows.Forms.CheckBox checkBox_AutoStartMining;
        private System.Windows.Forms.CheckBox checkBox_HideMiningWindows;
        private System.Windows.Forms.CheckBox checkBox_MinimizeToTray;
        private System.Windows.Forms.CheckBox checkBox_AutoScaleBTCValues;
        private System.Windows.Forms.CheckBox checkBox_StartMiningWhenIdle;
        private System.Windows.Forms.GroupBox groupBox_Localization;
        private System.Windows.Forms.Label label_Language;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.ComboBox comboBox_Language;
        private System.Windows.Forms.ComboBox currencyConverterCombobox;
        private System.Windows.Forms.Label label_displayCurrency;
        private System.Windows.Forms.TabPage tabPageAdvanced1;
        private System.Windows.Forms.GroupBox groupBox_Miners;
        private System.Windows.Forms.Label label_MinIdleSeconds;
        private System.Windows.Forms.TextBox textBox_SwitchProfitabilityThreshold;
        private System.Windows.Forms.Label label_SwitchProfitabilityThreshold;
        private System.Windows.Forms.TextBox textBox_MinIdleSeconds;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionCPU;
        private System.Windows.Forms.CheckBox checkBox_Additional_info_about_device;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionNVIDIA;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionAMD;
        private System.Windows.Forms.TabPage tabPageDevicesAlgos;
        private System.Windows.Forms.CheckBox checkBox_Disable_extra_launch_parameter_checking;
        private Components.AlgorithmSettingsControl algorithmSettingsControl1;
        private System.Windows.Forms.GroupBox groupBoxAlgorithmSettings;
        private Components.AlgorithmsListView algorithmsListView1;
        private Components.DevicesListViewEnableControl devicesListViewEnableControl1;
        private System.Windows.Forms.CheckBox checkBox_ShowFanAsPercent;
        private System.Windows.Forms.CheckBox Checkbox_Save_windows_size_and_position;
        private System.Windows.Forms.CheckBox checkbox_Group_same_devices;
        private System.Windows.Forms.GroupBox groupBox_Idle;
        private System.Windows.Forms.Label label_switching_algorithms;
        private System.Windows.Forms.ComboBox comboBox_switching_algorithms;
        private System.Windows.Forms.CheckBox checkBox_Show_profit_with_power_consumption;
        private System.Windows.Forms.Label label_devices_count;
        private System.Windows.Forms.ComboBox comboBox_devices_count;
        private System.Windows.Forms.CheckBox checkBox_fiat;
        private System.Windows.Forms.CheckBox checkBox_sorting_list_of_algorithms;
        private System.Windows.Forms.CheckBox checkBox_AlwaysOnTop;
        private System.Windows.Forms.GroupBox groupBoxInfo;
        private System.Windows.Forms.Button buttonLicence;
        private System.Windows.Forms.RichTextBox richTextBoxInfo;
        private System.Windows.Forms.GroupBox groupBoxUpdates;
        private System.Windows.Forms.Button buttonCheckNewVersion;
        private System.Windows.Forms.LinkLabel linkLabelCurrentVersion;
        public System.Windows.Forms.TabPage tabPageAbout;
        private ProgressBarSample.TextProgressBar progressBarUpdate;
        private System.Windows.Forms.GroupBox groupBoxBackup;
        private System.Windows.Forms.Button buttonRestoreBackup;
        private System.Windows.Forms.Button buttonCreateBackup;
        private System.Windows.Forms.Label labelBackupCopy;
        private System.Windows.Forms.CheckBox checkBoxAutoupdate;
        private System.Windows.Forms.Label labelCheckforprogramupdatesevery;
        private System.Windows.Forms.ComboBox comboBoxCheckforprogramupdatesevery;
        public System.Windows.Forms.Button buttonUpdate;
        public System.Windows.Forms.LinkLabel linkLabelNewVersion2;
        public System.Windows.Forms.CustomTabControl tabControlGeneral;
        private System.Windows.Forms.CheckBox checkbox_Use_OpenHardwareMonitor;
        private System.Windows.Forms.CheckBox checkBox_program_monitoring;
        private System.Windows.Forms.CheckBox checkBox_BackupBeforeUpdate;
        private System.Windows.Forms.CheckBox checkBoxRestartDriver;
        private System.Windows.Forms.CheckBox checkBoxRestartWindows;
        private System.Windows.Forms.Label labelRestartProgram;
        private System.Windows.Forms.ComboBox comboBoxRestartProgram;
        private System.Windows.Forms.CheckBox checkBoxCPUmonitoring;
        private System.Windows.Forms.CheckBox checkBoxNVMonitoring;
        private System.Windows.Forms.CheckBox checkBoxAMDmonitoring;
        private System.Windows.Forms.CheckBox checkBoxDriverWarning;
        private System.Windows.Forms.CheckBox checkBox_show_NVdevice_manufacturer;
        private System.Windows.Forms.CheckBox checkBox_ShowDeviceMemSize;
        private System.Windows.Forms.CheckBox checkBox_show_AMDdevice_manufacturer;
        private System.Windows.Forms.CheckBox checkbox_wallet_balance;
        private System.Windows.Forms.TabPage tabPageOverClock;
        private Components.DevicesListViewEnableControl devicesListViewEnableControl2;
        private System.Windows.Forms.GroupBox groupBoxOverClockSettings;
        private Components.AlgorithmsListViewOverClock algorithmsListViewOverClock1;
        private System.Windows.Forms.CheckBox checkBox_ABEnableOverclock;
        private System.Windows.Forms.CheckBox checkBox_ABMinimize;
        private System.Windows.Forms.CheckBox checkBox_By_profitability_of_all_devices;
        public System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.CheckBox checkBox_DisplayConnected;
        private System.Windows.Forms.CheckBox checkBoxCheckingCUDA;
        private System.Windows.Forms.CheckBox checkBox_withPower;
        private System.Windows.Forms.CheckBox checkBox_Show_memory_temp;
        private System.Windows.Forms.Label label_restart_nv_lost;
        private System.Windows.Forms.Label label_show_manufacturer;
        private System.Windows.Forms.CheckBox checkBox_Show_Total_Power;
        private System.Windows.Forms.CheckBox checkBox_ABDefault_program_closing;
        private System.Windows.Forms.CheckBox checkBox_ABDefault_mining_stopped;
        private System.Windows.Forms.CheckBox checkBox_DisableTooltips;
        private System.Windows.Forms.CheckBox checkBox_orderPrice;
        private System.Windows.Forms.GroupBox groupBoxAPIkeys;
        private System.Windows.Forms.Button buttonSetupAPI;
        public System.Windows.Forms.CheckBox checkBox_EnableAPI;
        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.CheckBox checkBoxProxySSL;
        private System.Windows.Forms.CheckBox checkBoxEnableProxy;
        private System.Windows.Forms.CheckBox checkBoxEnableRigRemoteView;
        private System.Windows.Forms.LinkLabel linkLabelRigRemoteView;
        private System.Windows.Forms.GroupBox groupBox_Logging;
        private System.Windows.Forms.Label label_LogMaxFileSize;
        private System.Windows.Forms.TextBox textBox_LogMaxFileSize;
        private System.Windows.Forms.CheckBox checkBox_LogToFile;
        private System.Windows.Forms.TabPage tabPagePower;
        private System.Windows.Forms.TextBox textBoxAddAMD;
        private System.Windows.Forms.Label labelAddAMD;
        private System.Windows.Forms.TextBox textBox_psu;
        private System.Windows.Forms.Label label_psu;
        private System.Windows.Forms.TextBox textBox_mb;
        private System.Windows.Forms.Label label_MBpower;
        private System.Windows.Forms.GroupBox groupBoxTariffs;
        private System.Windows.Forms.Label label_Schedules;
        private System.Windows.Forms.ComboBox comboBoxZones;
        private System.Windows.Forms.Label label_Schedules2;
        private System.Windows.Forms.TextBox textBoxScheduleCost1;
        private System.Windows.Forms.Label labelCost1;
        private System.Windows.Forms.Label labelTo1;
        private System.Windows.Forms.Label labelFrom1;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleFrom1;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleTo1;
        private System.Windows.Forms.Label labelPowerCurrency1;
        private System.Windows.Forms.Label labelPowerCurrency5;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleTo5;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleFrom5;
        private System.Windows.Forms.TextBox textBoxScheduleCost5;
        private System.Windows.Forms.Label labelCost5;
        private System.Windows.Forms.Label labelFrom5;
        private System.Windows.Forms.Label labelPowerCurrency4;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleTo4;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleFrom4;
        private System.Windows.Forms.TextBox textBoxScheduleCost4;
        private System.Windows.Forms.Label labelCost4;
        private System.Windows.Forms.Label labelFrom4;
        private System.Windows.Forms.Label labelPowerCurrency3;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleTo3;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleFrom3;
        private System.Windows.Forms.TextBox textBoxScheduleCost3;
        private System.Windows.Forms.Label labelCost3;
        private System.Windows.Forms.Label labelFrom3;
        private System.Windows.Forms.Label labelPowerCurrency2;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleTo2;
        private System.Windows.Forms.MaskedTextBox textBoxScheduleFrom2;
        private System.Windows.Forms.TextBox textBoxScheduleCost2;
        private System.Windows.Forms.Label labelCost2;
        private System.Windows.Forms.Label labelFrom2;
        private System.Windows.Forms.Label labelTo5;
        private System.Windows.Forms.Label labelTo4;
        private System.Windows.Forms.Label labelTo3;
        private System.Windows.Forms.Label labelTo2;
        private System.Windows.Forms.GroupBox groupBox_additionally;
        private System.Windows.Forms.CheckBox checkBoxLast24hours;
        private System.Windows.Forms.CheckBox checkBoxShortTerm;
        private System.Windows.Forms.GroupBox groupBoxServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAPIport;
        private System.Windows.Forms.CheckBox checkBoxAPI;
        private System.Windows.Forms.LinkLabel linkLabelGetAPIkey;
        private System.Windows.Forms.CheckBox checkBoxShowMinersVersions;
        private System.Windows.Forms.CheckBox checkBoxHideUnused;
        private System.Windows.Forms.CheckBox checkBoxHideUnused2;
        private System.Windows.Forms.Button button_ZIL_additional_mining;
        private System.Windows.Forms.Label labelDisableDetection;
        private System.Windows.Forms.CheckBox checkBox_show_INTELdevice_manufacturer;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionINTEL;
        private System.Windows.Forms.CheckBox checkBoxINTELmonitoring;
        private System.Windows.Forms.Label labelDisableMonitoring;
        private System.Windows.Forms.CheckBox checkBoxMiningFee;
        private System.Windows.Forms.CheckBox checkBoxHistory;
        private System.Windows.Forms.Button buttonHistory;
        private System.Windows.Forms.Button button_Lite_Algo;
        private System.Windows.Forms.CheckBox checkBoxSaveProtocolData;
        private System.Windows.Forms.CheckBox checkBoxInstall_root_certificates;
        private System.Windows.Forms.GroupBox groupBox_RigManager;
        private System.Windows.Forms.CheckBox checkBox_ShowPowerOfDisabledDevices;
    }
}
