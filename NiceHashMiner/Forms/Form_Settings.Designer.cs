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
            this.groupBox_Idle = new System.Windows.Forms.GroupBox();
            this.checkBox_StartMiningWhenIdle = new System.Windows.Forms.CheckBox();
            this.pictureBox_StartMiningWhenIdle = new System.Windows.Forms.PictureBox();
            this.label_MinIdleSeconds = new System.Windows.Forms.Label();
            this.pictureBox_MinIdleSeconds = new System.Windows.Forms.PictureBox();
            this.textBox_MinIdleSeconds = new System.Windows.Forms.TextBox();
            this.groupBox_Main = new System.Windows.Forms.GroupBox();
            this.checkbox_current_actual_profitabilities = new System.Windows.Forms.CheckBox();
            this.textBoxAddAMD = new System.Windows.Forms.TextBox();
            this.labelAddAMD = new System.Windows.Forms.Label();
            this.checkBox_fiat = new System.Windows.Forms.CheckBox();
            this.textBox_psu = new System.Windows.Forms.TextBox();
            this.label_psu = new System.Windows.Forms.Label();
            this.textBox_mb = new System.Windows.Forms.TextBox();
            this.label_MBpower = new System.Windows.Forms.Label();
            this.checkBox_Show_profit_with_power_consumption = new System.Windows.Forms.CheckBox();
            this.checkBox_Force_mining_if_nonprofitable = new System.Windows.Forms.CheckBox();
            this.pictureBox_ElectricityCost = new System.Windows.Forms.PictureBox();
            this.textBox_ElectricityCost = new System.Windows.Forms.TextBox();
            this.label_ElectricityCost = new System.Windows.Forms.Label();
            this.pictureBox_TimeUnit = new System.Windows.Forms.PictureBox();
            this.label_TimeUnit = new System.Windows.Forms.Label();
            this.comboBox_TimeUnit = new System.Windows.Forms.ComboBox();
            this.pictureBox_MinProfit = new System.Windows.Forms.PictureBox();
            this.textBox_MinProfit = new System.Windows.Forms.TextBox();
            this.label_MinProfit = new System.Windows.Forms.Label();
            this.groupBox_Misc = new System.Windows.Forms.GroupBox();
            this.labelRestartProgram = new System.Windows.Forms.Label();
            this.comboBoxRestartProgram = new System.Windows.Forms.ComboBox();
            this.checkBox_program_monitoring = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowUptime = new System.Windows.Forms.CheckBox();
            this.checkBox_AlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.checkBox_sorting_list_of_algorithms = new System.Windows.Forms.CheckBox();
            this.Checkbox_Save_windows_size_and_position = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_AutoStartMiningDelay = new System.Windows.Forms.TextBox();
            this.label_AutoStartMiningDelay = new System.Windows.Forms.Label();
            this.comboBox_ColorProfile = new System.Windows.Forms.ComboBox();
            this.checkBox_Send_actual_version_info = new System.Windows.Forms.CheckBox();
            this.checkBox_Allow_remote_management = new System.Windows.Forms.CheckBox();
            this.checkBox_MinimizeMiningWindows = new System.Windows.Forms.CheckBox();
            this.pictureBox_MinimizeMiningWindows = new System.Windows.Forms.PictureBox();
            this.pictureBox_RunAtStartup = new System.Windows.Forms.PictureBox();
            this.checkBox_RunAtStartup = new System.Windows.Forms.CheckBox();
            this.checkBox_AllowMultipleInstances = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoStartMining = new System.Windows.Forms.CheckBox();
            this.checkBox_HideMiningWindows = new System.Windows.Forms.CheckBox();
            this.pictureBox_AllowMultipleInstances = new System.Windows.Forms.PictureBox();
            this.checkBox_MinimizeToTray = new System.Windows.Forms.CheckBox();
            this.pictureBox_AutoScaleBTCValues = new System.Windows.Forms.PictureBox();
            this.pictureBox_AutoStartMining = new System.Windows.Forms.PictureBox();
            this.pictureBox_MinimizeToTray = new System.Windows.Forms.PictureBox();
            this.pictureBox_HideMiningWindows = new System.Windows.Forms.PictureBox();
            this.checkBox_AutoScaleBTCValues = new System.Windows.Forms.CheckBox();
            this.groupBox_Logging = new System.Windows.Forms.GroupBox();
            this.label_LogMaxFileSize = new System.Windows.Forms.Label();
            this.textBox_LogMaxFileSize = new System.Windows.Forms.TextBox();
            this.checkBox_LogToFile = new System.Windows.Forms.CheckBox();
            this.pictureBox_LogMaxFileSize = new System.Windows.Forms.PictureBox();
            this.groupBox_Localization = new System.Windows.Forms.GroupBox();
            this.label_Language = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox_displayCurrency = new System.Windows.Forms.PictureBox();
            this.pictureBox_Language = new System.Windows.Forms.PictureBox();
            this.comboBox_Language = new System.Windows.Forms.ComboBox();
            this.currencyConverterCombobox = new System.Windows.Forms.ComboBox();
            this.label_displayCurrency = new System.Windows.Forms.Label();
            this.tabPageAdvanced1 = new System.Windows.Forms.TabPage();
            this.groupBoxMOPA = new System.Windows.Forms.GroupBox();
            this.radioButtonMOPA5 = new System.Windows.Forms.RadioButton();
            this.radioButtonMOPA4 = new System.Windows.Forms.RadioButton();
            this.radioButtonMOPA3 = new System.Windows.Forms.RadioButton();
            this.radioButtonMOPA2 = new System.Windows.Forms.RadioButton();
            this.radioButtonMOPA1 = new System.Windows.Forms.RadioButton();
            this.groupBox_Miners = new System.Windows.Forms.GroupBox();
            this.checkBox_By_profitability_of_all_devices = new System.Windows.Forms.CheckBox();
            this.label_switching_algorithms = new System.Windows.Forms.Label();
            this.comboBox_switching_algorithms = new System.Windows.Forms.ComboBox();
            this.pictureBox_SwitchProfitabilityThreshold = new System.Windows.Forms.PictureBox();
            this.textBox_SwitchProfitabilityThreshold = new System.Windows.Forms.TextBox();
            this.label_SwitchProfitabilityThreshold = new System.Windows.Forms.Label();
            this.checkbox_Group_same_devices = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_show_AMDdevice_manufacturer = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowDeviceMemSize = new System.Windows.Forms.CheckBox();
            this.checkBox_RunEthlargement = new System.Windows.Forms.CheckBox();
            this.checkBox_show_NVdevice_manufacturer = new System.Windows.Forms.CheckBox();
            this.checkBoxDriverWarning = new System.Windows.Forms.CheckBox();
            this.checkBoxCPUmonitoring = new System.Windows.Forms.CheckBox();
            this.checkBoxNVMonitoring = new System.Windows.Forms.CheckBox();
            this.checkBoxAMDmonitoring = new System.Windows.Forms.CheckBox();
            this.checkBoxRestartDriver = new System.Windows.Forms.CheckBox();
            this.checkBoxRestartWindows = new System.Windows.Forms.CheckBox();
            this.checkbox_Use_OpenHardwareMonitor = new System.Windows.Forms.CheckBox();
            this.label_devices_count = new System.Windows.Forms.Label();
            this.comboBox_devices_count = new System.Windows.Forms.ComboBox();
            this.checkBox_ShowFanAsPercent = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBox_DisableDetectionCPU = new System.Windows.Forms.CheckBox();
            this.checkBox_Additional_info_about_device = new System.Windows.Forms.CheckBox();
            this.pictureBox_DisableDetectionAMD = new System.Windows.Forms.PictureBox();
            this.checkBox_DisableDetectionNVIDIA = new System.Windows.Forms.CheckBox();
            this.pictureBox_DisableDetectionNVIDIA = new System.Windows.Forms.PictureBox();
            this.checkBox_DisableDetectionAMD = new System.Windows.Forms.CheckBox();
            this.tabPageDevicesAlgos = new System.Windows.Forms.TabPage();
            this.textBoxMaxEpoch = new System.Windows.Forms.TextBox();
            this.labelMaxEpoch = new System.Windows.Forms.Label();
            this.checkBox_Disable_extra_launch_parameter_checking = new System.Windows.Forms.CheckBox();
            this.groupBoxAlgorithmSettings = new System.Windows.Forms.GroupBox();
            this.algorithmsListView1 = new NiceHashMiner.Forms.Components.AlgorithmsListView();
            this.algorithmSettingsControl1 = new NiceHashMiner.Forms.Components.AlgorithmSettingsControl();
            this.devicesListViewEnableControl1 = new NiceHashMiner.Forms.Components.DevicesListViewEnableControl();
            this.tabPageOverClock = new System.Windows.Forms.TabPage();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.checkBox_ABMinimize = new System.Windows.Forms.CheckBox();
            this.checkBox_ABEnableOverclock = new System.Windows.Forms.CheckBox();
            this.groupBoxOverClockSettings = new System.Windows.Forms.GroupBox();
            this.algorithmsListViewOverClock1 = new NiceHashMiner.Forms.Components.AlgorithmsListViewOverClock();
            this.devicesListViewEnableControl2 = new NiceHashMiner.Forms.Components.DevicesListViewEnableControl();
            this.tabPageTools = new System.Windows.Forms.TabPage();
            this.groupBoxToolsNH = new System.Windows.Forms.GroupBox();
            this.richTextBoxCheckNiceHashservers = new System.Windows.Forms.RichTextBox();
            this.buttonCheckNiceHashservers = new System.Windows.Forms.Button();
            this.buttonCheckNiceHashStatus = new System.Windows.Forms.Button();
            this.groupBoxToolsNET = new System.Windows.Forms.GroupBox();
            this.buttonCurrPorts = new System.Windows.Forms.Button();
            this.groupBoxToolsGPU = new System.Windows.Forms.GroupBox();
            this.buttonNVIDIAinspector = new System.Windows.Forms.Button();
            this.buttonOverdriveNTool = new System.Windows.Forms.Button();
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
            this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
            this.buttonLicence = new System.Windows.Forms.Button();
            this.checkBox_DisplayConnected = new System.Windows.Forms.CheckBox();
            this.tabControlGeneral.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBox_Idle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_StartMiningWhenIdle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinIdleSeconds)).BeginInit();
            this.groupBox_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ElectricityCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_TimeUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinProfit)).BeginInit();
            this.groupBox_Misc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinimizeMiningWindows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RunAtStartup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AllowMultipleInstances)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AutoScaleBTCValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AutoStartMining)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinimizeToTray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_HideMiningWindows)).BeginInit();
            this.groupBox_Logging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LogMaxFileSize)).BeginInit();
            this.groupBox_Localization.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_displayCurrency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Language)).BeginInit();
            this.tabPageAdvanced1.SuspendLayout();
            this.groupBoxMOPA.SuspendLayout();
            this.groupBox_Miners.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SwitchProfitabilityThreshold)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableDetectionAMD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableDetectionNVIDIA)).BeginInit();
            this.tabPageDevicesAlgos.SuspendLayout();
            this.groupBoxAlgorithmSettings.SuspendLayout();
            this.tabPageOverClock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBoxOverClockSettings.SuspendLayout();
            this.tabPageTools.SuspendLayout();
            this.groupBoxToolsNH.SuspendLayout();
            this.groupBoxToolsNET.SuspendLayout();
            this.groupBoxToolsGPU.SuspendLayout();
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
            this.buttonSaveClose.Location = new System.Drawing.Point(398, 473);
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
            this.buttonDefaults.Location = new System.Drawing.Point(320, 473);
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
            this.buttonCloseNoSave.Location = new System.Drawing.Point(536, 473);
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
            this.tabControlGeneral.Controls.Add(this.tabPageAdvanced1);
            this.tabControlGeneral.Controls.Add(this.tabPageDevicesAlgos);
            this.tabControlGeneral.Controls.Add(this.tabPageOverClock);
            this.tabControlGeneral.Controls.Add(this.tabPageTools);
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
            this.tabControlGeneral.Size = new System.Drawing.Size(677, 466);
            this.tabControlGeneral.TabIndex = 47;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageGeneral.Controls.Add(this.groupBox_Idle);
            this.tabPageGeneral.Controls.Add(this.groupBox_Main);
            this.tabPageGeneral.Controls.Add(this.groupBox_Misc);
            this.tabPageGeneral.Controls.Add(this.groupBox_Logging);
            this.tabPageGeneral.Controls.Add(this.groupBox_Localization);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 23);
            this.tabPageGeneral.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(669, 439);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            // 
            // groupBox_Idle
            // 
            this.groupBox_Idle.Controls.Add(this.checkBox_StartMiningWhenIdle);
            this.groupBox_Idle.Controls.Add(this.pictureBox_StartMiningWhenIdle);
            this.groupBox_Idle.Controls.Add(this.label_MinIdleSeconds);
            this.groupBox_Idle.Controls.Add(this.pictureBox_MinIdleSeconds);
            this.groupBox_Idle.Controls.Add(this.textBox_MinIdleSeconds);
            this.groupBox_Idle.Location = new System.Drawing.Point(6, 363);
            this.groupBox_Idle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Idle.Name = "groupBox_Idle";
            this.groupBox_Idle.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Idle.Size = new System.Drawing.Size(365, 70);
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
            // pictureBox_StartMiningWhenIdle
            // 
            this.pictureBox_StartMiningWhenIdle.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_StartMiningWhenIdle.Location = new System.Drawing.Point(171, 18);
            this.pictureBox_StartMiningWhenIdle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_StartMiningWhenIdle.Name = "pictureBox_StartMiningWhenIdle";
            this.pictureBox_StartMiningWhenIdle.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_StartMiningWhenIdle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_StartMiningWhenIdle.TabIndex = 364;
            this.pictureBox_StartMiningWhenIdle.TabStop = false;
            // 
            // label_MinIdleSeconds
            // 
            this.label_MinIdleSeconds.AutoSize = true;
            this.label_MinIdleSeconds.Location = new System.Drawing.Point(3, 41);
            this.label_MinIdleSeconds.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MinIdleSeconds.Name = "label_MinIdleSeconds";
            this.label_MinIdleSeconds.Size = new System.Drawing.Size(86, 13);
            this.label_MinIdleSeconds.TabIndex = 356;
            this.label_MinIdleSeconds.Text = "MinIdleSeconds:";
            this.label_MinIdleSeconds.Click += new System.EventHandler(this.label_MinIdleSeconds_Click);
            // 
            // pictureBox_MinIdleSeconds
            // 
            this.pictureBox_MinIdleSeconds.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_MinIdleSeconds.Location = new System.Drawing.Point(171, 38);
            this.pictureBox_MinIdleSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_MinIdleSeconds.Name = "pictureBox_MinIdleSeconds";
            this.pictureBox_MinIdleSeconds.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_MinIdleSeconds.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_MinIdleSeconds.TabIndex = 385;
            this.pictureBox_MinIdleSeconds.TabStop = false;
            this.pictureBox_MinIdleSeconds.Click += new System.EventHandler(this.pictureBox_MinIdleSeconds_Click);
            // 
            // textBox_MinIdleSeconds
            // 
            this.textBox_MinIdleSeconds.Location = new System.Drawing.Point(146, 38);
            this.textBox_MinIdleSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MinIdleSeconds.Name = "textBox_MinIdleSeconds";
            this.textBox_MinIdleSeconds.Size = new System.Drawing.Size(21, 20);
            this.textBox_MinIdleSeconds.TabIndex = 335;
            this.textBox_MinIdleSeconds.TextChanged += new System.EventHandler(this.textBox_MinIdleSeconds_TextChanged);
            // 
            // groupBox_Main
            // 
            this.groupBox_Main.Controls.Add(this.checkbox_current_actual_profitabilities);
            this.groupBox_Main.Controls.Add(this.textBoxAddAMD);
            this.groupBox_Main.Controls.Add(this.labelAddAMD);
            this.groupBox_Main.Controls.Add(this.checkBox_fiat);
            this.groupBox_Main.Controls.Add(this.textBox_psu);
            this.groupBox_Main.Controls.Add(this.label_psu);
            this.groupBox_Main.Controls.Add(this.textBox_mb);
            this.groupBox_Main.Controls.Add(this.label_MBpower);
            this.groupBox_Main.Controls.Add(this.checkBox_Show_profit_with_power_consumption);
            this.groupBox_Main.Controls.Add(this.checkBox_Force_mining_if_nonprofitable);
            this.groupBox_Main.Controls.Add(this.pictureBox_ElectricityCost);
            this.groupBox_Main.Controls.Add(this.textBox_ElectricityCost);
            this.groupBox_Main.Controls.Add(this.label_ElectricityCost);
            this.groupBox_Main.Controls.Add(this.pictureBox_TimeUnit);
            this.groupBox_Main.Controls.Add(this.label_TimeUnit);
            this.groupBox_Main.Controls.Add(this.comboBox_TimeUnit);
            this.groupBox_Main.Controls.Add(this.pictureBox_MinProfit);
            this.groupBox_Main.Controls.Add(this.textBox_MinProfit);
            this.groupBox_Main.Controls.Add(this.label_MinProfit);
            this.groupBox_Main.Location = new System.Drawing.Point(6, 6);
            this.groupBox_Main.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Main.Name = "groupBox_Main";
            this.groupBox_Main.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Main.Size = new System.Drawing.Size(365, 216);
            this.groupBox_Main.TabIndex = 386;
            this.groupBox_Main.TabStop = false;
            this.groupBox_Main.Text = "Main:";
            this.groupBox_Main.Enter += new System.EventHandler(this.groupBox_Main_Enter);
            // 
            // checkbox_current_actual_profitabilities
            // 
            this.checkbox_current_actual_profitabilities.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkbox_current_actual_profitabilities.Location = new System.Drawing.Point(6, 192);
            this.checkbox_current_actual_profitabilities.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkbox_current_actual_profitabilities.Name = "checkbox_current_actual_profitabilities";
            this.checkbox_current_actual_profitabilities.Size = new System.Drawing.Size(318, 18);
            this.checkbox_current_actual_profitabilities.TabIndex = 385;
            this.checkbox_current_actual_profitabilities.Text = "Show current actual profitability";
            this.checkbox_current_actual_profitabilities.UseVisualStyleBackColor = true;
            // 
            // textBoxAddAMD
            // 
            this.textBoxAddAMD.Location = new System.Drawing.Point(249, 100);
            this.textBoxAddAMD.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxAddAMD.Name = "textBoxAddAMD";
            this.textBoxAddAMD.Size = new System.Drawing.Size(32, 20);
            this.textBoxAddAMD.TabIndex = 383;
            // 
            // labelAddAMD
            // 
            this.labelAddAMD.AutoSize = true;
            this.labelAddAMD.Location = new System.Drawing.Point(6, 102);
            this.labelAddAMD.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelAddAMD.Name = "labelAddAMD";
            this.labelAddAMD.Size = new System.Drawing.Size(221, 13);
            this.labelAddAMD.TabIndex = 384;
            this.labelAddAMD.Text = "Additional AMD GPU power consumption (W)";
            // 
            // checkBox_fiat
            // 
            this.checkBox_fiat.AutoSize = true;
            this.checkBox_fiat.Location = new System.Drawing.Point(6, 149);
            this.checkBox_fiat.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_fiat.Name = "checkBox_fiat";
            this.checkBox_fiat.Size = new System.Drawing.Size(239, 17);
            this.checkBox_fiat.TabIndex = 382;
            this.checkBox_fiat.Text = "Show profitability of algorithms in fiat currency";
            this.checkBox_fiat.UseVisualStyleBackColor = true;
            this.checkBox_fiat.CheckedChanged += new System.EventHandler(this.checkBox_fiat_CheckedChanged);
            // 
            // textBox_psu
            // 
            this.textBox_psu.Location = new System.Drawing.Point(314, 46);
            this.textBox_psu.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_psu.Name = "textBox_psu";
            this.textBox_psu.Size = new System.Drawing.Size(39, 20);
            this.textBox_psu.TabIndex = 380;
            // 
            // label_psu
            // 
            this.label_psu.AutoSize = true;
            this.label_psu.Location = new System.Drawing.Point(209, 49);
            this.label_psu.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_psu.Name = "label_psu";
            this.label_psu.Size = new System.Drawing.Size(94, 13);
            this.label_psu.TabIndex = 381;
            this.label_psu.Text = "PSU efficiency (%)";
            // 
            // textBox_mb
            // 
            this.textBox_mb.Location = new System.Drawing.Point(287, 74);
            this.textBox_mb.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_mb.Name = "textBox_mb";
            this.textBox_mb.Size = new System.Drawing.Size(66, 20);
            this.textBox_mb.TabIndex = 378;
            this.textBox_mb.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label_MBpower
            // 
            this.label_MBpower.AutoSize = true;
            this.label_MBpower.Location = new System.Drawing.Point(6, 76);
            this.label_MBpower.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MBpower.Name = "label_MBpower";
            this.label_MBpower.Size = new System.Drawing.Size(271, 13);
            this.label_MBpower.TabIndex = 379;
            this.label_MBpower.Text = "Power consumption of Motherboard, HDD(SSD) etc (W)";
            this.label_MBpower.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // checkBox_Show_profit_with_power_consumption
            // 
            this.checkBox_Show_profit_with_power_consumption.AutoSize = true;
            this.checkBox_Show_profit_with_power_consumption.Location = new System.Drawing.Point(6, 126);
            this.checkBox_Show_profit_with_power_consumption.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Show_profit_with_power_consumption.Name = "checkBox_Show_profit_with_power_consumption";
            this.checkBox_Show_profit_with_power_consumption.Size = new System.Drawing.Size(196, 17);
            this.checkBox_Show_profit_with_power_consumption.TabIndex = 377;
            this.checkBox_Show_profit_with_power_consumption.Text = "Show profit with power consumption";
            this.checkBox_Show_profit_with_power_consumption.UseVisualStyleBackColor = true;
            this.checkBox_Show_profit_with_power_consumption.CheckedChanged += new System.EventHandler(this.checkBox_Show_profit_with_power_consumption_CheckedChanged);
            // 
            // checkBox_Force_mining_if_nonprofitable
            // 
            this.checkBox_Force_mining_if_nonprofitable.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Force_mining_if_nonprofitable.Location = new System.Drawing.Point(6, 172);
            this.checkBox_Force_mining_if_nonprofitable.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Force_mining_if_nonprofitable.Name = "checkBox_Force_mining_if_nonprofitable";
            this.checkBox_Force_mining_if_nonprofitable.Size = new System.Drawing.Size(318, 18);
            this.checkBox_Force_mining_if_nonprofitable.TabIndex = 376;
            this.checkBox_Force_mining_if_nonprofitable.Text = "Force mining if nonprofitable";
            this.checkBox_Force_mining_if_nonprofitable.UseVisualStyleBackColor = true;
            this.checkBox_Force_mining_if_nonprofitable.CheckedChanged += new System.EventHandler(this.checkBox_Force_mining_if_nonprofitable_CheckedChanged_1);
            // 
            // pictureBox_ElectricityCost
            // 
            this.pictureBox_ElectricityCost.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_ElectricityCost.Location = new System.Drawing.Point(171, 46);
            this.pictureBox_ElectricityCost.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_ElectricityCost.Name = "pictureBox_ElectricityCost";
            this.pictureBox_ElectricityCost.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_ElectricityCost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_ElectricityCost.TabIndex = 375;
            this.pictureBox_ElectricityCost.TabStop = false;
            this.pictureBox_ElectricityCost.Click += new System.EventHandler(this.pictureBox_ElectricityCost_Click_1);
            // 
            // textBox_ElectricityCost
            // 
            this.textBox_ElectricityCost.Location = new System.Drawing.Point(128, 45);
            this.textBox_ElectricityCost.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_ElectricityCost.Name = "textBox_ElectricityCost";
            this.textBox_ElectricityCost.Size = new System.Drawing.Size(39, 20);
            this.textBox_ElectricityCost.TabIndex = 373;
            this.textBox_ElectricityCost.TextChanged += new System.EventHandler(this.textBox_ElectricityCost_TextChanged_1);
            // 
            // label_ElectricityCost
            // 
            this.label_ElectricityCost.AutoSize = true;
            this.label_ElectricityCost.Location = new System.Drawing.Point(6, 48);
            this.label_ElectricityCost.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_ElectricityCost.Name = "label_ElectricityCost";
            this.label_ElectricityCost.Size = new System.Drawing.Size(112, 13);
            this.label_ElectricityCost.TabIndex = 374;
            this.label_ElectricityCost.Text = "Electricity Cost (KWh):";
            this.label_ElectricityCost.Click += new System.EventHandler(this.label_ElectricityCost_Click_1);
            // 
            // pictureBox_TimeUnit
            // 
            this.pictureBox_TimeUnit.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_TimeUnit.Location = new System.Drawing.Point(335, 21);
            this.pictureBox_TimeUnit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_TimeUnit.Name = "pictureBox_TimeUnit";
            this.pictureBox_TimeUnit.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_TimeUnit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_TimeUnit.TabIndex = 372;
            this.pictureBox_TimeUnit.TabStop = false;
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
            this.comboBox_TimeUnit.Location = new System.Drawing.Point(269, 19);
            this.comboBox_TimeUnit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_TimeUnit.Name = "comboBox_TimeUnit";
            this.comboBox_TimeUnit.Size = new System.Drawing.Size(62, 21);
            this.comboBox_TimeUnit.TabIndex = 370;
            this.comboBox_TimeUnit.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_TimeUnit_DrawItem);
            this.comboBox_TimeUnit.SelectedIndexChanged += new System.EventHandler(this.comboBox_TimeUnit_SelectedIndexChanged);
            // 
            // pictureBox_MinProfit
            // 
            this.pictureBox_MinProfit.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_MinProfit.Location = new System.Drawing.Point(171, 20);
            this.pictureBox_MinProfit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_MinProfit.Name = "pictureBox_MinProfit";
            this.pictureBox_MinProfit.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_MinProfit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_MinProfit.TabIndex = 364;
            this.pictureBox_MinProfit.TabStop = false;
            this.pictureBox_MinProfit.Click += new System.EventHandler(this.pictureBox_MinProfit_Click_1);
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
            // groupBox_Misc
            // 
            this.groupBox_Misc.Controls.Add(this.labelRestartProgram);
            this.groupBox_Misc.Controls.Add(this.comboBoxRestartProgram);
            this.groupBox_Misc.Controls.Add(this.checkBox_program_monitoring);
            this.groupBox_Misc.Controls.Add(this.checkBox_ShowUptime);
            this.groupBox_Misc.Controls.Add(this.checkBox_AlwaysOnTop);
            this.groupBox_Misc.Controls.Add(this.checkBox_sorting_list_of_algorithms);
            this.groupBox_Misc.Controls.Add(this.Checkbox_Save_windows_size_and_position);
            this.groupBox_Misc.Controls.Add(this.label1);
            this.groupBox_Misc.Controls.Add(this.textBox_AutoStartMiningDelay);
            this.groupBox_Misc.Controls.Add(this.label_AutoStartMiningDelay);
            this.groupBox_Misc.Controls.Add(this.comboBox_ColorProfile);
            this.groupBox_Misc.Controls.Add(this.checkBox_Send_actual_version_info);
            this.groupBox_Misc.Controls.Add(this.checkBox_Allow_remote_management);
            this.groupBox_Misc.Controls.Add(this.checkBox_MinimizeMiningWindows);
            this.groupBox_Misc.Controls.Add(this.pictureBox_MinimizeMiningWindows);
            this.groupBox_Misc.Controls.Add(this.pictureBox_RunAtStartup);
            this.groupBox_Misc.Controls.Add(this.checkBox_RunAtStartup);
            this.groupBox_Misc.Controls.Add(this.checkBox_AllowMultipleInstances);
            this.groupBox_Misc.Controls.Add(this.checkBox_AutoStartMining);
            this.groupBox_Misc.Controls.Add(this.checkBox_HideMiningWindows);
            this.groupBox_Misc.Controls.Add(this.pictureBox_AllowMultipleInstances);
            this.groupBox_Misc.Controls.Add(this.checkBox_MinimizeToTray);
            this.groupBox_Misc.Controls.Add(this.pictureBox_AutoScaleBTCValues);
            this.groupBox_Misc.Controls.Add(this.pictureBox_AutoStartMining);
            this.groupBox_Misc.Controls.Add(this.pictureBox_MinimizeToTray);
            this.groupBox_Misc.Controls.Add(this.pictureBox_HideMiningWindows);
            this.groupBox_Misc.Controls.Add(this.checkBox_AutoScaleBTCValues);
            this.groupBox_Misc.Location = new System.Drawing.Point(375, 6);
            this.groupBox_Misc.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Misc.Name = "groupBox_Misc";
            this.groupBox_Misc.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Misc.Size = new System.Drawing.Size(288, 427);
            this.groupBox_Misc.TabIndex = 391;
            this.groupBox_Misc.TabStop = false;
            this.groupBox_Misc.Text = "Misc:";
            this.groupBox_Misc.Enter += new System.EventHandler(this.groupBox_Misc_Enter);
            // 
            // labelRestartProgram
            // 
            this.labelRestartProgram.AutoSize = true;
            this.labelRestartProgram.Location = new System.Drawing.Point(4, 398);
            this.labelRestartProgram.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelRestartProgram.Name = "labelRestartProgram";
            this.labelRestartProgram.Size = new System.Drawing.Size(82, 13);
            this.labelRestartProgram.TabIndex = 406;
            this.labelRestartProgram.Text = "Restart program";
            // 
            // comboBoxRestartProgram
            // 
            this.comboBoxRestartProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRestartProgram.FormattingEnabled = true;
            this.comboBoxRestartProgram.Location = new System.Drawing.Point(154, 395);
            this.comboBoxRestartProgram.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBoxRestartProgram.Name = "comboBoxRestartProgram";
            this.comboBoxRestartProgram.Size = new System.Drawing.Size(117, 21);
            this.comboBoxRestartProgram.TabIndex = 405;
            this.comboBoxRestartProgram.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxRestartProgram_DrawItem);
            // 
            // checkBox_program_monitoring
            // 
            this.checkBox_program_monitoring.AutoSize = true;
            this.checkBox_program_monitoring.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_program_monitoring.Location = new System.Drawing.Point(7, 371);
            this.checkBox_program_monitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_program_monitoring.Name = "checkBox_program_monitoring";
            this.checkBox_program_monitoring.Size = new System.Drawing.Size(116, 17);
            this.checkBox_program_monitoring.TabIndex = 384;
            this.checkBox_program_monitoring.Text = "Program monitoring";
            this.checkBox_program_monitoring.UseVisualStyleBackColor = true;
            this.checkBox_program_monitoring.CheckedChanged += new System.EventHandler(this.checkBox_program_monitoring_CheckedChanged);
            // 
            // checkBox_ShowUptime
            // 
            this.checkBox_ShowUptime.AutoSize = true;
            this.checkBox_ShowUptime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ShowUptime.Location = new System.Drawing.Point(7, 349);
            this.checkBox_ShowUptime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ShowUptime.Name = "checkBox_ShowUptime";
            this.checkBox_ShowUptime.Size = new System.Drawing.Size(87, 17);
            this.checkBox_ShowUptime.TabIndex = 383;
            this.checkBox_ShowUptime.Text = "Show uptime";
            this.checkBox_ShowUptime.UseVisualStyleBackColor = true;
            // 
            // checkBox_AlwaysOnTop
            // 
            this.checkBox_AlwaysOnTop.AutoSize = true;
            this.checkBox_AlwaysOnTop.Location = new System.Drawing.Point(7, 158);
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
            this.checkBox_sorting_list_of_algorithms.Location = new System.Drawing.Point(7, 326);
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
            this.Checkbox_Save_windows_size_and_position.Location = new System.Drawing.Point(7, 303);
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
            this.label1.Location = new System.Drawing.Point(7, 280);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 379;
            this.label1.Text = "Color profile";
            // 
            // textBox_AutoStartMiningDelay
            // 
            this.textBox_AutoStartMiningDelay.Location = new System.Drawing.Point(235, 65);
            this.textBox_AutoStartMiningDelay.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_AutoStartMiningDelay.Name = "textBox_AutoStartMiningDelay";
            this.textBox_AutoStartMiningDelay.Size = new System.Drawing.Size(36, 20);
            this.textBox_AutoStartMiningDelay.TabIndex = 377;
            // 
            // label_AutoStartMiningDelay
            // 
            this.label_AutoStartMiningDelay.AutoSize = true;
            this.label_AutoStartMiningDelay.Location = new System.Drawing.Point(27, 68);
            this.label_AutoStartMiningDelay.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_AutoStartMiningDelay.Name = "label_AutoStartMiningDelay";
            this.label_AutoStartMiningDelay.Size = new System.Drawing.Size(139, 13);
            this.label_AutoStartMiningDelay.TabIndex = 376;
            this.label_AutoStartMiningDelay.Text = "Autostart Mining Delay (sec)";
            // 
            // comboBox_ColorProfile
            // 
            this.comboBox_ColorProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ColorProfile.FormattingEnabled = true;
            this.comboBox_ColorProfile.Location = new System.Drawing.Point(169, 276);
            this.comboBox_ColorProfile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_ColorProfile.Name = "comboBox_ColorProfile";
            this.comboBox_ColorProfile.Size = new System.Drawing.Size(102, 21);
            this.comboBox_ColorProfile.TabIndex = 378;
            this.comboBox_ColorProfile.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_ColorProfile_DrawItem);
            this.comboBox_ColorProfile.SelectedIndexChanged += new System.EventHandler(this.comboBox_ColorProfile_SelectedIndexChanged);
            // 
            // checkBox_Send_actual_version_info
            // 
            this.checkBox_Send_actual_version_info.AutoSize = true;
            this.checkBox_Send_actual_version_info.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Send_actual_version_info.Location = new System.Drawing.Point(7, 249);
            this.checkBox_Send_actual_version_info.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Send_actual_version_info.Name = "checkBox_Send_actual_version_info";
            this.checkBox_Send_actual_version_info.Size = new System.Drawing.Size(140, 17);
            this.checkBox_Send_actual_version_info.TabIndex = 375;
            this.checkBox_Send_actual_version_info.Text = "Send actual version info";
            this.checkBox_Send_actual_version_info.UseVisualStyleBackColor = true;
            // 
            // checkBox_Allow_remote_management
            // 
            this.checkBox_Allow_remote_management.AutoSize = true;
            this.checkBox_Allow_remote_management.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Allow_remote_management.Location = new System.Drawing.Point(7, 226);
            this.checkBox_Allow_remote_management.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Allow_remote_management.Name = "checkBox_Allow_remote_management";
            this.checkBox_Allow_remote_management.Size = new System.Drawing.Size(150, 17);
            this.checkBox_Allow_remote_management.TabIndex = 374;
            this.checkBox_Allow_remote_management.Text = "Allow remote management";
            this.checkBox_Allow_remote_management.UseVisualStyleBackColor = true;
            // 
            // checkBox_MinimizeMiningWindows
            // 
            this.checkBox_MinimizeMiningWindows.AutoSize = true;
            this.checkBox_MinimizeMiningWindows.Location = new System.Drawing.Point(7, 112);
            this.checkBox_MinimizeMiningWindows.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_MinimizeMiningWindows.Name = "checkBox_MinimizeMiningWindows";
            this.checkBox_MinimizeMiningWindows.Size = new System.Drawing.Size(141, 17);
            this.checkBox_MinimizeMiningWindows.TabIndex = 368;
            this.checkBox_MinimizeMiningWindows.Text = "MinimizeMiningWindows";
            this.checkBox_MinimizeMiningWindows.UseVisualStyleBackColor = true;
            // 
            // pictureBox_MinimizeMiningWindows
            // 
            this.pictureBox_MinimizeMiningWindows.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_MinimizeMiningWindows.Location = new System.Drawing.Point(253, 111);
            this.pictureBox_MinimizeMiningWindows.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_MinimizeMiningWindows.Name = "pictureBox_MinimizeMiningWindows";
            this.pictureBox_MinimizeMiningWindows.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_MinimizeMiningWindows.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_MinimizeMiningWindows.TabIndex = 369;
            this.pictureBox_MinimizeMiningWindows.TabStop = false;
            // 
            // pictureBox_RunAtStartup
            // 
            this.pictureBox_RunAtStartup.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_RunAtStartup.Location = new System.Drawing.Point(253, 20);
            this.pictureBox_RunAtStartup.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_RunAtStartup.Name = "pictureBox_RunAtStartup";
            this.pictureBox_RunAtStartup.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_RunAtStartup.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_RunAtStartup.TabIndex = 367;
            this.pictureBox_RunAtStartup.TabStop = false;
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
            // checkBox_AllowMultipleInstances
            // 
            this.checkBox_AllowMultipleInstances.AutoSize = true;
            this.checkBox_AllowMultipleInstances.Location = new System.Drawing.Point(7, 203);
            this.checkBox_AllowMultipleInstances.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AllowMultipleInstances.Name = "checkBox_AllowMultipleInstances";
            this.checkBox_AllowMultipleInstances.Size = new System.Drawing.Size(139, 17);
            this.checkBox_AllowMultipleInstances.TabIndex = 365;
            this.checkBox_AllowMultipleInstances.Text = "Allow Multiple Instances";
            this.checkBox_AllowMultipleInstances.UseVisualStyleBackColor = true;
            this.checkBox_AllowMultipleInstances.CheckedChanged += new System.EventHandler(this.checkBox_AllowMultipleInstances_CheckedChanged);
            // 
            // checkBox_AutoStartMining
            // 
            this.checkBox_AutoStartMining.AutoSize = true;
            this.checkBox_AutoStartMining.Location = new System.Drawing.Point(7, 44);
            this.checkBox_AutoStartMining.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AutoStartMining.Name = "checkBox_AutoStartMining";
            this.checkBox_AutoStartMining.Size = new System.Drawing.Size(102, 17);
            this.checkBox_AutoStartMining.TabIndex = 315;
            this.checkBox_AutoStartMining.Text = "Autostart Mining";
            this.checkBox_AutoStartMining.UseVisualStyleBackColor = true;
            this.checkBox_AutoStartMining.CheckedChanged += new System.EventHandler(this.checkBox_AutoStartMining_CheckedChanged_1);
            // 
            // checkBox_HideMiningWindows
            // 
            this.checkBox_HideMiningWindows.AutoSize = true;
            this.checkBox_HideMiningWindows.Location = new System.Drawing.Point(7, 89);
            this.checkBox_HideMiningWindows.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_HideMiningWindows.Name = "checkBox_HideMiningWindows";
            this.checkBox_HideMiningWindows.Size = new System.Drawing.Size(123, 17);
            this.checkBox_HideMiningWindows.TabIndex = 315;
            this.checkBox_HideMiningWindows.Text = "HideMiningWindows";
            this.checkBox_HideMiningWindows.UseVisualStyleBackColor = true;
            // 
            // pictureBox_AllowMultipleInstances
            // 
            this.pictureBox_AllowMultipleInstances.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_AllowMultipleInstances.Location = new System.Drawing.Point(253, 202);
            this.pictureBox_AllowMultipleInstances.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_AllowMultipleInstances.Name = "pictureBox_AllowMultipleInstances";
            this.pictureBox_AllowMultipleInstances.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_AllowMultipleInstances.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_AllowMultipleInstances.TabIndex = 364;
            this.pictureBox_AllowMultipleInstances.TabStop = false;
            // 
            // checkBox_MinimizeToTray
            // 
            this.checkBox_MinimizeToTray.AutoSize = true;
            this.checkBox_MinimizeToTray.Location = new System.Drawing.Point(7, 135);
            this.checkBox_MinimizeToTray.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_MinimizeToTray.Name = "checkBox_MinimizeToTray";
            this.checkBox_MinimizeToTray.Size = new System.Drawing.Size(100, 17);
            this.checkBox_MinimizeToTray.TabIndex = 316;
            this.checkBox_MinimizeToTray.Text = "MinimizeToTray";
            this.checkBox_MinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // pictureBox_AutoScaleBTCValues
            // 
            this.pictureBox_AutoScaleBTCValues.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_AutoScaleBTCValues.Location = new System.Drawing.Point(253, 179);
            this.pictureBox_AutoScaleBTCValues.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_AutoScaleBTCValues.Name = "pictureBox_AutoScaleBTCValues";
            this.pictureBox_AutoScaleBTCValues.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_AutoScaleBTCValues.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_AutoScaleBTCValues.TabIndex = 364;
            this.pictureBox_AutoScaleBTCValues.TabStop = false;
            // 
            // pictureBox_AutoStartMining
            // 
            this.pictureBox_AutoStartMining.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_AutoStartMining.Location = new System.Drawing.Point(253, 43);
            this.pictureBox_AutoStartMining.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_AutoStartMining.Name = "pictureBox_AutoStartMining";
            this.pictureBox_AutoStartMining.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_AutoStartMining.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_AutoStartMining.TabIndex = 364;
            this.pictureBox_AutoStartMining.TabStop = false;
            // 
            // pictureBox_MinimizeToTray
            // 
            this.pictureBox_MinimizeToTray.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_MinimizeToTray.Location = new System.Drawing.Point(253, 134);
            this.pictureBox_MinimizeToTray.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_MinimizeToTray.Name = "pictureBox_MinimizeToTray";
            this.pictureBox_MinimizeToTray.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_MinimizeToTray.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_MinimizeToTray.TabIndex = 364;
            this.pictureBox_MinimizeToTray.TabStop = false;
            // 
            // pictureBox_HideMiningWindows
            // 
            this.pictureBox_HideMiningWindows.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_HideMiningWindows.Location = new System.Drawing.Point(253, 88);
            this.pictureBox_HideMiningWindows.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_HideMiningWindows.Name = "pictureBox_HideMiningWindows";
            this.pictureBox_HideMiningWindows.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_HideMiningWindows.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_HideMiningWindows.TabIndex = 364;
            this.pictureBox_HideMiningWindows.TabStop = false;
            // 
            // checkBox_AutoScaleBTCValues
            // 
            this.checkBox_AutoScaleBTCValues.AutoSize = true;
            this.checkBox_AutoScaleBTCValues.Location = new System.Drawing.Point(7, 180);
            this.checkBox_AutoScaleBTCValues.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AutoScaleBTCValues.Name = "checkBox_AutoScaleBTCValues";
            this.checkBox_AutoScaleBTCValues.Size = new System.Drawing.Size(128, 17);
            this.checkBox_AutoScaleBTCValues.TabIndex = 321;
            this.checkBox_AutoScaleBTCValues.Text = "AutoScaleBTCValues";
            this.checkBox_AutoScaleBTCValues.UseVisualStyleBackColor = true;
            // 
            // groupBox_Logging
            // 
            this.groupBox_Logging.Controls.Add(this.label_LogMaxFileSize);
            this.groupBox_Logging.Controls.Add(this.textBox_LogMaxFileSize);
            this.groupBox_Logging.Controls.Add(this.checkBox_LogToFile);
            this.groupBox_Logging.Controls.Add(this.pictureBox_LogMaxFileSize);
            this.groupBox_Logging.Location = new System.Drawing.Point(6, 304);
            this.groupBox_Logging.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Logging.Name = "groupBox_Logging";
            this.groupBox_Logging.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Logging.Size = new System.Drawing.Size(365, 53);
            this.groupBox_Logging.TabIndex = 388;
            this.groupBox_Logging.TabStop = false;
            this.groupBox_Logging.Text = "Logging:";
            // 
            // label_LogMaxFileSize
            // 
            this.label_LogMaxFileSize.AutoSize = true;
            this.label_LogMaxFileSize.Location = new System.Drawing.Point(133, 20);
            this.label_LogMaxFileSize.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LogMaxFileSize.Name = "label_LogMaxFileSize";
            this.label_LogMaxFileSize.Size = new System.Drawing.Size(84, 13);
            this.label_LogMaxFileSize.TabIndex = 357;
            this.label_LogMaxFileSize.Text = "LogMaxFileSize:";
            this.label_LogMaxFileSize.Click += new System.EventHandler(this.label_LogMaxFileSize_Click);
            // 
            // textBox_LogMaxFileSize
            // 
            this.textBox_LogMaxFileSize.Location = new System.Drawing.Point(270, 17);
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
            // pictureBox_LogMaxFileSize
            // 
            this.pictureBox_LogMaxFileSize.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_LogMaxFileSize.Location = new System.Drawing.Point(337, 18);
            this.pictureBox_LogMaxFileSize.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_LogMaxFileSize.Name = "pictureBox_LogMaxFileSize";
            this.pictureBox_LogMaxFileSize.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_LogMaxFileSize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_LogMaxFileSize.TabIndex = 364;
            this.pictureBox_LogMaxFileSize.TabStop = false;
            // 
            // groupBox_Localization
            // 
            this.groupBox_Localization.Controls.Add(this.label_Language);
            this.groupBox_Localization.Controls.Add(this.pictureBox5);
            this.groupBox_Localization.Controls.Add(this.pictureBox_displayCurrency);
            this.groupBox_Localization.Controls.Add(this.pictureBox_Language);
            this.groupBox_Localization.Controls.Add(this.comboBox_Language);
            this.groupBox_Localization.Controls.Add(this.currencyConverterCombobox);
            this.groupBox_Localization.Controls.Add(this.label_displayCurrency);
            this.groupBox_Localization.Location = new System.Drawing.Point(6, 228);
            this.groupBox_Localization.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Localization.Name = "groupBox_Localization";
            this.groupBox_Localization.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Localization.Size = new System.Drawing.Size(365, 70);
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
            // pictureBox_displayCurrency
            // 
            this.pictureBox_displayCurrency.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_displayCurrency.Location = new System.Drawing.Point(333, 22);
            this.pictureBox_displayCurrency.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_displayCurrency.Name = "pictureBox_displayCurrency";
            this.pictureBox_displayCurrency.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_displayCurrency.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_displayCurrency.TabIndex = 364;
            this.pictureBox_displayCurrency.TabStop = false;
            // 
            // pictureBox_Language
            // 
            this.pictureBox_Language.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_Language.Location = new System.Drawing.Point(171, 22);
            this.pictureBox_Language.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_Language.Name = "pictureBox_Language";
            this.pictureBox_Language.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_Language.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_Language.TabIndex = 364;
            this.pictureBox_Language.TabStop = false;
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
            this.currencyConverterCombobox.Location = new System.Drawing.Point(265, 19);
            this.currencyConverterCombobox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.currencyConverterCombobox.Name = "currencyConverterCombobox";
            this.currencyConverterCombobox.Size = new System.Drawing.Size(64, 21);
            this.currencyConverterCombobox.Sorted = true;
            this.currencyConverterCombobox.TabIndex = 381;
            this.currencyConverterCombobox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.currencyConverterCombobox_DrawItem);
            this.currencyConverterCombobox.SelectedIndexChanged += new System.EventHandler(this.CurrencyConverterCombobox_SelectedIndexChanged);
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
            // tabPageAdvanced1
            // 
            this.tabPageAdvanced1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageAdvanced1.Controls.Add(this.groupBoxMOPA);
            this.tabPageAdvanced1.Controls.Add(this.groupBox_Miners);
            this.tabPageAdvanced1.Controls.Add(this.groupBox1);
            this.tabPageAdvanced1.Location = new System.Drawing.Point(4, 23);
            this.tabPageAdvanced1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageAdvanced1.Name = "tabPageAdvanced1";
            this.tabPageAdvanced1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageAdvanced1.Size = new System.Drawing.Size(669, 439);
            this.tabPageAdvanced1.TabIndex = 2;
            this.tabPageAdvanced1.Text = "Advanced";
            // 
            // groupBoxMOPA
            // 
            this.groupBoxMOPA.Controls.Add(this.radioButtonMOPA5);
            this.groupBoxMOPA.Controls.Add(this.radioButtonMOPA4);
            this.groupBoxMOPA.Controls.Add(this.radioButtonMOPA3);
            this.groupBoxMOPA.Controls.Add(this.radioButtonMOPA2);
            this.groupBoxMOPA.Controls.Add(this.radioButtonMOPA1);
            this.groupBoxMOPA.Location = new System.Drawing.Point(7, 103);
            this.groupBoxMOPA.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxMOPA.Name = "groupBoxMOPA";
            this.groupBoxMOPA.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxMOPA.Size = new System.Drawing.Size(658, 68);
            this.groupBoxMOPA.TabIndex = 396;
            this.groupBoxMOPA.TabStop = false;
            this.groupBoxMOPA.Text = "Method of obtaining profitability of algorithms:";
            // 
            // radioButtonMOPA5
            // 
            this.radioButtonMOPA5.AutoSize = true;
            this.radioButtonMOPA5.Location = new System.Drawing.Point(242, 42);
            this.radioButtonMOPA5.Name = "radioButtonMOPA5";
            this.radioButtonMOPA5.Size = new System.Drawing.Size(183, 17);
            this.radioButtonMOPA5.TabIndex = 399;
            this.radioButtonMOPA5.TabStop = true;
            this.radioButtonMOPA5.Text = "Highest profitability by all methods";
            this.radioButtonMOPA5.UseVisualStyleBackColor = true;
            this.radioButtonMOPA5.CheckedChanged += new System.EventHandler(this.radioButtonMOPA5_CheckedChanged_1);
            // 
            // radioButtonMOPA4
            // 
            this.radioButtonMOPA4.AutoSize = true;
            this.radioButtonMOPA4.Location = new System.Drawing.Point(10, 42);
            this.radioButtonMOPA4.Name = "radioButtonMOPA4";
            this.radioButtonMOPA4.Size = new System.Drawing.Size(139, 17);
            this.radioButtonMOPA4.TabIndex = 398;
            this.radioButtonMOPA4.TabStop = true;
            this.radioButtonMOPA4.Text = "24 hours avg profitability";
            this.radioButtonMOPA4.UseVisualStyleBackColor = true;
            this.radioButtonMOPA4.CheckedChanged += new System.EventHandler(this.radioButtonMOPA4_CheckedChanged_1);
            // 
            // radioButtonMOPA3
            // 
            this.radioButtonMOPA3.AutoSize = true;
            this.radioButtonMOPA3.Location = new System.Drawing.Point(472, 19);
            this.radioButtonMOPA3.Name = "radioButtonMOPA3";
            this.radioButtonMOPA3.Size = new System.Drawing.Size(143, 17);
            this.radioButtonMOPA3.TabIndex = 397;
            this.radioButtonMOPA3.TabStop = true;
            this.radioButtonMOPA3.Text = "5 minutes avg profitability";
            this.radioButtonMOPA3.UseVisualStyleBackColor = true;
            this.radioButtonMOPA3.CheckedChanged += new System.EventHandler(this.radioButtonMOPA3_CheckedChanged_1);
            // 
            // radioButtonMOPA2
            // 
            this.radioButtonMOPA2.AutoSize = true;
            this.radioButtonMOPA2.Location = new System.Drawing.Point(242, 19);
            this.radioButtonMOPA2.Name = "radioButtonMOPA2";
            this.radioButtonMOPA2.Size = new System.Drawing.Size(111, 17);
            this.radioButtonMOPA2.TabIndex = 396;
            this.radioButtonMOPA2.TabStop = true;
            this.radioButtonMOPA2.Text = "Current profitability";
            this.radioButtonMOPA2.UseVisualStyleBackColor = true;
            this.radioButtonMOPA2.CheckedChanged += new System.EventHandler(this.radioButtonMOPA2_CheckedChanged_1);
            // 
            // radioButtonMOPA1
            // 
            this.radioButtonMOPA1.AutoSize = true;
            this.radioButtonMOPA1.Location = new System.Drawing.Point(10, 19);
            this.radioButtonMOPA1.Name = "radioButtonMOPA1";
            this.radioButtonMOPA1.Size = new System.Drawing.Size(96, 17);
            this.radioButtonMOPA1.TabIndex = 395;
            this.radioButtonMOPA1.TabStop = true;
            this.radioButtonMOPA1.Text = "Standard NHM";
            this.radioButtonMOPA1.UseVisualStyleBackColor = true;
            this.radioButtonMOPA1.CheckedChanged += new System.EventHandler(this.radioButtonMOPA1_CheckedChanged_1);
            // 
            // groupBox_Miners
            // 
            this.groupBox_Miners.Controls.Add(this.checkBox_By_profitability_of_all_devices);
            this.groupBox_Miners.Controls.Add(this.label_switching_algorithms);
            this.groupBox_Miners.Controls.Add(this.comboBox_switching_algorithms);
            this.groupBox_Miners.Controls.Add(this.pictureBox_SwitchProfitabilityThreshold);
            this.groupBox_Miners.Controls.Add(this.textBox_SwitchProfitabilityThreshold);
            this.groupBox_Miners.Controls.Add(this.label_SwitchProfitabilityThreshold);
            this.groupBox_Miners.Controls.Add(this.checkbox_Group_same_devices);
            this.groupBox_Miners.Location = new System.Drawing.Point(6, 6);
            this.groupBox_Miners.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Miners.Name = "groupBox_Miners";
            this.groupBox_Miners.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Miners.Size = new System.Drawing.Size(657, 91);
            this.groupBox_Miners.TabIndex = 389;
            this.groupBox_Miners.TabStop = false;
            this.groupBox_Miners.Text = "Miners:";
            // 
            // checkBox_By_profitability_of_all_devices
            // 
            this.checkBox_By_profitability_of_all_devices.AutoSize = true;
            this.checkBox_By_profitability_of_all_devices.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_By_profitability_of_all_devices.Location = new System.Drawing.Point(446, 36);
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
            // pictureBox_SwitchProfitabilityThreshold
            // 
            this.pictureBox_SwitchProfitabilityThreshold.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_SwitchProfitabilityThreshold.Location = new System.Drawing.Point(422, 34);
            this.pictureBox_SwitchProfitabilityThreshold.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_SwitchProfitabilityThreshold.Name = "pictureBox_SwitchProfitabilityThreshold";
            this.pictureBox_SwitchProfitabilityThreshold.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_SwitchProfitabilityThreshold.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_SwitchProfitabilityThreshold.TabIndex = 385;
            this.pictureBox_SwitchProfitabilityThreshold.TabStop = false;
            this.pictureBox_SwitchProfitabilityThreshold.Click += new System.EventHandler(this.pictureBox_SwitchProfitabilityThreshold_Click);
            // 
            // textBox_SwitchProfitabilityThreshold
            // 
            this.textBox_SwitchProfitabilityThreshold.Location = new System.Drawing.Point(394, 34);
            this.textBox_SwitchProfitabilityThreshold.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_SwitchProfitabilityThreshold.Name = "textBox_SwitchProfitabilityThreshold";
            this.textBox_SwitchProfitabilityThreshold.Size = new System.Drawing.Size(24, 20);
            this.textBox_SwitchProfitabilityThreshold.TabIndex = 333;
            this.textBox_SwitchProfitabilityThreshold.TextChanged += new System.EventHandler(this.textBox_SwitchProfitabilityThreshold_TextChanged);
            // 
            // label_SwitchProfitabilityThreshold
            // 
            this.label_SwitchProfitabilityThreshold.AutoSize = true;
            this.label_SwitchProfitabilityThreshold.Location = new System.Drawing.Point(189, 37);
            this.label_SwitchProfitabilityThreshold.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_SwitchProfitabilityThreshold.Name = "label_SwitchProfitabilityThreshold";
            this.label_SwitchProfitabilityThreshold.Size = new System.Drawing.Size(170, 13);
            this.label_SwitchProfitabilityThreshold.TabIndex = 361;
            this.label_SwitchProfitabilityThreshold.Text = "switching_threshold_of_profitability";
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
            this.groupBox1.Controls.Add(this.checkBox_DisplayConnected);
            this.groupBox1.Controls.Add(this.checkBox_show_AMDdevice_manufacturer);
            this.groupBox1.Controls.Add(this.checkBox_ShowDeviceMemSize);
            this.groupBox1.Controls.Add(this.checkBox_RunEthlargement);
            this.groupBox1.Controls.Add(this.checkBox_show_NVdevice_manufacturer);
            this.groupBox1.Controls.Add(this.checkBoxDriverWarning);
            this.groupBox1.Controls.Add(this.checkBoxCPUmonitoring);
            this.groupBox1.Controls.Add(this.checkBoxNVMonitoring);
            this.groupBox1.Controls.Add(this.checkBoxAMDmonitoring);
            this.groupBox1.Controls.Add(this.checkBoxRestartDriver);
            this.groupBox1.Controls.Add(this.checkBoxRestartWindows);
            this.groupBox1.Controls.Add(this.checkbox_Use_OpenHardwareMonitor);
            this.groupBox1.Controls.Add(this.label_devices_count);
            this.groupBox1.Controls.Add(this.comboBox_devices_count);
            this.groupBox1.Controls.Add(this.checkBox_ShowFanAsPercent);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionCPU);
            this.groupBox1.Controls.Add(this.checkBox_Additional_info_about_device);
            this.groupBox1.Controls.Add(this.pictureBox_DisableDetectionAMD);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionNVIDIA);
            this.groupBox1.Controls.Add(this.pictureBox_DisableDetectionNVIDIA);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionAMD);
            this.groupBox1.Location = new System.Drawing.Point(9, 177);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Size = new System.Drawing.Size(658, 256);
            this.groupBox1.TabIndex = 394;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Devices:";
            // 
            // checkBox_show_AMDdevice_manufacturer
            // 
            this.checkBox_show_AMDdevice_manufacturer.AutoSize = true;
            this.checkBox_show_AMDdevice_manufacturer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_show_AMDdevice_manufacturer.Location = new System.Drawing.Point(304, 42);
            this.checkBox_show_AMDdevice_manufacturer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_show_AMDdevice_manufacturer.Name = "checkBox_show_AMDdevice_manufacturer";
            this.checkBox_show_AMDdevice_manufacturer.Size = new System.Drawing.Size(180, 17);
            this.checkBox_show_AMDdevice_manufacturer.TabIndex = 415;
            this.checkBox_show_AMDdevice_manufacturer.Text = "Show AMD device manufacturer";
            this.checkBox_show_AMDdevice_manufacturer.UseVisualStyleBackColor = true;
            // 
            // checkBox_ShowDeviceMemSize
            // 
            this.checkBox_ShowDeviceMemSize.AutoSize = true;
            this.checkBox_ShowDeviceMemSize.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ShowDeviceMemSize.Location = new System.Drawing.Point(304, 65);
            this.checkBox_ShowDeviceMemSize.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ShowDeviceMemSize.Name = "checkBox_ShowDeviceMemSize";
            this.checkBox_ShowDeviceMemSize.Size = new System.Drawing.Size(148, 17);
            this.checkBox_ShowDeviceMemSize.TabIndex = 414;
            this.checkBox_ShowDeviceMemSize.Text = "Show device memory size";
            this.checkBox_ShowDeviceMemSize.UseVisualStyleBackColor = true;
            // 
            // checkBox_RunEthlargement
            // 
            this.checkBox_RunEthlargement.AutoSize = true;
            this.checkBox_RunEthlargement.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_RunEthlargement.Location = new System.Drawing.Point(32, 233);
            this.checkBox_RunEthlargement.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_RunEthlargement.Name = "checkBox_RunEthlargement";
            this.checkBox_RunEthlargement.Size = new System.Drawing.Size(111, 17);
            this.checkBox_RunEthlargement.TabIndex = 401;
            this.checkBox_RunEthlargement.Text = "Run Ethlargement";
            this.checkBox_RunEthlargement.UseVisualStyleBackColor = true;
            this.checkBox_RunEthlargement.CheckedChanged += new System.EventHandler(this.checkBox_RunEthlargement_CheckedChanged);
            // 
            // checkBox_show_NVdevice_manufacturer
            // 
            this.checkBox_show_NVdevice_manufacturer.AutoSize = true;
            this.checkBox_show_NVdevice_manufacturer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_show_NVdevice_manufacturer.Location = new System.Drawing.Point(304, 19);
            this.checkBox_show_NVdevice_manufacturer.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_show_NVdevice_manufacturer.Name = "checkBox_show_NVdevice_manufacturer";
            this.checkBox_show_NVdevice_manufacturer.Size = new System.Drawing.Size(192, 17);
            this.checkBox_show_NVdevice_manufacturer.TabIndex = 413;
            this.checkBox_show_NVdevice_manufacturer.Text = "Show NVIDIA device manufacturer";
            this.checkBox_show_NVdevice_manufacturer.UseVisualStyleBackColor = true;
            this.checkBox_show_NVdevice_manufacturer.CheckedChanged += new System.EventHandler(this.checkBox_show_device_manufacturer_CheckedChanged);
            // 
            // checkBoxDriverWarning
            // 
            this.checkBoxDriverWarning.AutoSize = true;
            this.checkBoxDriverWarning.Location = new System.Drawing.Point(32, 180);
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
            this.checkBoxCPUmonitoring.Location = new System.Drawing.Point(32, 134);
            this.checkBoxCPUmonitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxCPUmonitoring.Name = "checkBoxCPUmonitoring";
            this.checkBoxCPUmonitoring.Size = new System.Drawing.Size(137, 17);
            this.checkBoxCPUmonitoring.TabIndex = 411;
            this.checkBoxCPUmonitoring.Text = "Disable CPU monitoring";
            this.checkBoxCPUmonitoring.UseVisualStyleBackColor = true;
            // 
            // checkBoxNVMonitoring
            // 
            this.checkBoxNVMonitoring.AutoSize = true;
            this.checkBoxNVMonitoring.Location = new System.Drawing.Point(32, 42);
            this.checkBoxNVMonitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxNVMonitoring.Name = "checkBoxNVMonitoring";
            this.checkBoxNVMonitoring.Size = new System.Drawing.Size(177, 17);
            this.checkBoxNVMonitoring.TabIndex = 409;
            this.checkBoxNVMonitoring.Text = "Disable NVIDIA GPU monitoring";
            this.checkBoxNVMonitoring.UseVisualStyleBackColor = true;
            // 
            // checkBoxAMDmonitoring
            // 
            this.checkBoxAMDmonitoring.AutoSize = true;
            this.checkBoxAMDmonitoring.Location = new System.Drawing.Point(32, 88);
            this.checkBoxAMDmonitoring.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxAMDmonitoring.Name = "checkBoxAMDmonitoring";
            this.checkBoxAMDmonitoring.Size = new System.Drawing.Size(165, 17);
            this.checkBoxAMDmonitoring.TabIndex = 410;
            this.checkBoxAMDmonitoring.Text = "Disable AMD GPU monitoring";
            this.checkBoxAMDmonitoring.UseVisualStyleBackColor = true;
            // 
            // checkBoxRestartDriver
            // 
            this.checkBoxRestartDriver.AutoSize = true;
            this.checkBoxRestartDriver.Location = new System.Drawing.Point(304, 222);
            this.checkBoxRestartDriver.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxRestartDriver.Name = "checkBoxRestartDriver";
            this.checkBoxRestartDriver.Size = new System.Drawing.Size(206, 17);
            this.checkBoxRestartDriver.TabIndex = 408;
            this.checkBoxRestartDriver.Text = "Restart driver when CUDA GPU is lost";
            this.checkBoxRestartDriver.UseVisualStyleBackColor = true;
            this.checkBoxRestartDriver.CheckedChanged += new System.EventHandler(this.checkBoxRestartDriver_CheckedChanged);
            // 
            // checkBoxRestartWindows
            // 
            this.checkBoxRestartWindows.AutoSize = true;
            this.checkBoxRestartWindows.Location = new System.Drawing.Point(304, 199);
            this.checkBoxRestartWindows.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBoxRestartWindows.Name = "checkBoxRestartWindows";
            this.checkBoxRestartWindows.Size = new System.Drawing.Size(224, 17);
            this.checkBoxRestartWindows.TabIndex = 397;
            this.checkBoxRestartWindows.Text = "Restart Windows when CUDA GPU is lost";
            this.checkBoxRestartWindows.UseVisualStyleBackColor = true;
            this.checkBoxRestartWindows.CheckedChanged += new System.EventHandler(this.checkBoxRestartWindows_CheckedChanged);
            // 
            // checkbox_Use_OpenHardwareMonitor
            // 
            this.checkbox_Use_OpenHardwareMonitor.AutoSize = true;
            this.checkbox_Use_OpenHardwareMonitor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkbox_Use_OpenHardwareMonitor.Location = new System.Drawing.Point(32, 157);
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
            this.label_devices_count.Location = new System.Drawing.Point(301, 157);
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
            this.comboBox_devices_count.Location = new System.Drawing.Point(446, 154);
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
            this.checkBox_ShowFanAsPercent.Location = new System.Drawing.Point(304, 134);
            this.checkBox_ShowFanAsPercent.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ShowFanAsPercent.Name = "checkBox_ShowFanAsPercent";
            this.checkBox_ShowFanAsPercent.Size = new System.Drawing.Size(144, 17);
            this.checkBox_ShowFanAsPercent.TabIndex = 401;
            this.checkBox_ShowFanAsPercent.Text = "Show fan rpm as percent";
            this.checkBox_ShowFanAsPercent.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox1.Location = new System.Drawing.Point(10, 110);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(18, 18);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 400;
            this.pictureBox1.TabStop = false;
            // 
            // checkBox_DisableDetectionCPU
            // 
            this.checkBox_DisableDetectionCPU.AutoSize = true;
            this.checkBox_DisableDetectionCPU.Location = new System.Drawing.Point(32, 111);
            this.checkBox_DisableDetectionCPU.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionCPU.Name = "checkBox_DisableDetectionCPU";
            this.checkBox_DisableDetectionCPU.Size = new System.Drawing.Size(133, 17);
            this.checkBox_DisableDetectionCPU.TabIndex = 399;
            this.checkBox_DisableDetectionCPU.Text = "Disable CPU detection";
            this.checkBox_DisableDetectionCPU.UseVisualStyleBackColor = true;
            this.checkBox_DisableDetectionCPU.CheckedChanged += new System.EventHandler(this.checkBox_DisableDetectionCPU_CheckedChanged);
            // 
            // checkBox_Additional_info_about_device
            // 
            this.checkBox_Additional_info_about_device.AutoSize = true;
            this.checkBox_Additional_info_about_device.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Additional_info_about_device.Location = new System.Drawing.Point(304, 88);
            this.checkBox_Additional_info_about_device.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Additional_info_about_device.Name = "checkBox_Additional_info_about_device";
            this.checkBox_Additional_info_about_device.Size = new System.Drawing.Size(157, 17);
            this.checkBox_Additional_info_about_device.TabIndex = 398;
            this.checkBox_Additional_info_about_device.Text = "Additional info about device";
            this.checkBox_Additional_info_about_device.UseVisualStyleBackColor = true;
            // 
            // pictureBox_DisableDetectionAMD
            // 
            this.pictureBox_DisableDetectionAMD.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_DisableDetectionAMD.Location = new System.Drawing.Point(10, 64);
            this.pictureBox_DisableDetectionAMD.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_DisableDetectionAMD.Name = "pictureBox_DisableDetectionAMD";
            this.pictureBox_DisableDetectionAMD.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_DisableDetectionAMD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_DisableDetectionAMD.TabIndex = 392;
            this.pictureBox_DisableDetectionAMD.TabStop = false;
            // 
            // checkBox_DisableDetectionNVIDIA
            // 
            this.checkBox_DisableDetectionNVIDIA.AutoSize = true;
            this.checkBox_DisableDetectionNVIDIA.Location = new System.Drawing.Point(32, 19);
            this.checkBox_DisableDetectionNVIDIA.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionNVIDIA.Name = "checkBox_DisableDetectionNVIDIA";
            this.checkBox_DisableDetectionNVIDIA.Size = new System.Drawing.Size(173, 17);
            this.checkBox_DisableDetectionNVIDIA.TabIndex = 390;
            this.checkBox_DisableDetectionNVIDIA.Text = "Disable NVIDIA GPU detection";
            this.checkBox_DisableDetectionNVIDIA.UseVisualStyleBackColor = true;
            this.checkBox_DisableDetectionNVIDIA.CheckedChanged += new System.EventHandler(this.checkBox_DisableDetectionNVIDIA_CheckedChanged);
            // 
            // pictureBox_DisableDetectionNVIDIA
            // 
            this.pictureBox_DisableDetectionNVIDIA.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_DisableDetectionNVIDIA.Location = new System.Drawing.Point(10, 18);
            this.pictureBox_DisableDetectionNVIDIA.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_DisableDetectionNVIDIA.Name = "pictureBox_DisableDetectionNVIDIA";
            this.pictureBox_DisableDetectionNVIDIA.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_DisableDetectionNVIDIA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_DisableDetectionNVIDIA.TabIndex = 393;
            this.pictureBox_DisableDetectionNVIDIA.TabStop = false;
            // 
            // checkBox_DisableDetectionAMD
            // 
            this.checkBox_DisableDetectionAMD.AutoSize = true;
            this.checkBox_DisableDetectionAMD.Location = new System.Drawing.Point(32, 65);
            this.checkBox_DisableDetectionAMD.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionAMD.Name = "checkBox_DisableDetectionAMD";
            this.checkBox_DisableDetectionAMD.Size = new System.Drawing.Size(161, 17);
            this.checkBox_DisableDetectionAMD.TabIndex = 391;
            this.checkBox_DisableDetectionAMD.Text = "Disable AMD GPU detection";
            this.checkBox_DisableDetectionAMD.UseVisualStyleBackColor = true;
            this.checkBox_DisableDetectionAMD.CheckedChanged += new System.EventHandler(this.checkBox_DisableDetectionAMD_CheckedChanged);
            // 
            // tabPageDevicesAlgos
            // 
            this.tabPageDevicesAlgos.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDevicesAlgos.Controls.Add(this.textBoxMaxEpoch);
            this.tabPageDevicesAlgos.Controls.Add(this.labelMaxEpoch);
            this.tabPageDevicesAlgos.Controls.Add(this.checkBox_Disable_extra_launch_parameter_checking);
            this.tabPageDevicesAlgos.Controls.Add(this.groupBoxAlgorithmSettings);
            this.tabPageDevicesAlgos.Controls.Add(this.algorithmSettingsControl1);
            this.tabPageDevicesAlgos.Controls.Add(this.devicesListViewEnableControl1);
            this.tabPageDevicesAlgos.Location = new System.Drawing.Point(4, 23);
            this.tabPageDevicesAlgos.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageDevicesAlgos.Name = "tabPageDevicesAlgos";
            this.tabPageDevicesAlgos.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPageDevicesAlgos.Size = new System.Drawing.Size(669, 439);
            this.tabPageDevicesAlgos.TabIndex = 1;
            this.tabPageDevicesAlgos.Text = "Devices/Algorithms";
            // 
            // textBoxMaxEpoch
            // 
            this.textBoxMaxEpoch.Location = new System.Drawing.Point(233, 220);
            this.textBoxMaxEpoch.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxMaxEpoch.Name = "textBoxMaxEpoch";
            this.textBoxMaxEpoch.Size = new System.Drawing.Size(32, 20);
            this.textBoxMaxEpoch.TabIndex = 408;
            // 
            // labelMaxEpoch
            // 
            this.labelMaxEpoch.AutoSize = true;
            this.labelMaxEpoch.Location = new System.Drawing.Point(5, 223);
            this.labelMaxEpoch.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMaxEpoch.Name = "labelMaxEpoch";
            this.labelMaxEpoch.Size = new System.Drawing.Size(206, 13);
            this.labelMaxEpoch.TabIndex = 407;
            this.labelMaxEpoch.Text = "Maximum Epoch of DaggerHashimoto4GB";
            // 
            // checkBox_Disable_extra_launch_parameter_checking
            // 
            this.checkBox_Disable_extra_launch_parameter_checking.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_Disable_extra_launch_parameter_checking.AutoSize = true;
            this.checkBox_Disable_extra_launch_parameter_checking.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Disable_extra_launch_parameter_checking.Location = new System.Drawing.Point(378, 222);
            this.checkBox_Disable_extra_launch_parameter_checking.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Disable_extra_launch_parameter_checking.Name = "checkBox_Disable_extra_launch_parameter_checking";
            this.checkBox_Disable_extra_launch_parameter_checking.Size = new System.Drawing.Size(219, 17);
            this.checkBox_Disable_extra_launch_parameter_checking.TabIndex = 399;
            this.checkBox_Disable_extra_launch_parameter_checking.Text = "Disable extra launch parameter checking";
            this.checkBox_Disable_extra_launch_parameter_checking.UseVisualStyleBackColor = true;
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
            this.groupBoxAlgorithmSettings.Size = new System.Drawing.Size(657, 249);
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
            this.algorithmsListView1.Size = new System.Drawing.Size(645, 165);
            this.algorithmsListView1.TabIndex = 2;
            // 
            // algorithmSettingsControl1
            // 
            this.algorithmSettingsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.algorithmSettingsControl1.Location = new System.Drawing.Point(378, -1);
            this.algorithmSettingsControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.algorithmSettingsControl1.Name = "algorithmSettingsControl1";
            this.algorithmSettingsControl1.Size = new System.Drawing.Size(285, 208);
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
            this.devicesListViewEnableControl1.Size = new System.Drawing.Size(348, 199);
            this.devicesListViewEnableControl1.TabIndex = 397;
            // 
            // tabPageOverClock
            // 
            this.tabPageOverClock.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageOverClock.Controls.Add(this.linkLabel3);
            this.tabPageOverClock.Controls.Add(this.pictureBox2);
            this.tabPageOverClock.Controls.Add(this.checkBox_ABMinimize);
            this.tabPageOverClock.Controls.Add(this.checkBox_ABEnableOverclock);
            this.tabPageOverClock.Controls.Add(this.groupBoxOverClockSettings);
            this.tabPageOverClock.Controls.Add(this.devicesListViewEnableControl2);
            this.tabPageOverClock.Location = new System.Drawing.Point(4, 23);
            this.tabPageOverClock.Name = "tabPageOverClock";
            this.tabPageOverClock.Size = new System.Drawing.Size(669, 439);
            this.tabPageOverClock.TabIndex = 5;
            this.tabPageOverClock.Text = "OverClock";
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel3.Location = new System.Drawing.Point(394, 166);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(55, 13);
            this.linkLabel3.TabIndex = 406;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "linkLabel3";
            this.linkLabel3.Click += new System.EventHandler(this.linkLabel3_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox2.Location = new System.Drawing.Point(371, 161);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(18, 18);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 405;
            this.pictureBox2.TabStop = false;
            // 
            // checkBox_ABMinimize
            // 
            this.checkBox_ABMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.checkBox_ABEnableOverclock.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.groupBoxOverClockSettings.Location = new System.Drawing.Point(4, 187);
            this.groupBoxOverClockSettings.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxOverClockSettings.Name = "groupBoxOverClockSettings";
            this.groupBoxOverClockSettings.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxOverClockSettings.Size = new System.Drawing.Size(662, 249);
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
            this.algorithmsListViewOverClock1.Size = new System.Drawing.Size(652, 224);
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
            // tabPageTools
            // 
            this.tabPageTools.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageTools.Controls.Add(this.groupBoxToolsNH);
            this.tabPageTools.Controls.Add(this.groupBoxToolsNET);
            this.tabPageTools.Controls.Add(this.groupBoxToolsGPU);
            this.tabPageTools.Location = new System.Drawing.Point(4, 23);
            this.tabPageTools.Name = "tabPageTools";
            this.tabPageTools.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTools.Size = new System.Drawing.Size(669, 439);
            this.tabPageTools.TabIndex = 4;
            this.tabPageTools.Text = "Tools";
            // 
            // groupBoxToolsNH
            // 
            this.groupBoxToolsNH.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxToolsNH.Controls.Add(this.richTextBoxCheckNiceHashservers);
            this.groupBoxToolsNH.Controls.Add(this.buttonCheckNiceHashservers);
            this.groupBoxToolsNH.Controls.Add(this.buttonCheckNiceHashStatus);
            this.groupBoxToolsNH.Location = new System.Drawing.Point(5, 6);
            this.groupBoxToolsNH.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxToolsNH.Name = "groupBoxToolsNH";
            this.groupBoxToolsNH.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxToolsNH.Size = new System.Drawing.Size(657, 108);
            this.groupBoxToolsNH.TabIndex = 398;
            this.groupBoxToolsNH.TabStop = false;
            this.groupBoxToolsNH.Text = "NiceHash";
            // 
            // richTextBoxCheckNiceHashservers
            // 
            this.richTextBoxCheckNiceHashservers.Location = new System.Drawing.Point(326, 19);
            this.richTextBoxCheckNiceHashservers.Name = "richTextBoxCheckNiceHashservers";
            this.richTextBoxCheckNiceHashservers.Size = new System.Drawing.Size(283, 83);
            this.richTextBoxCheckNiceHashservers.TabIndex = 46;
            this.richTextBoxCheckNiceHashservers.Text = "";
            // 
            // buttonCheckNiceHashservers
            // 
            this.buttonCheckNiceHashservers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCheckNiceHashservers.Location = new System.Drawing.Point(177, 19);
            this.buttonCheckNiceHashservers.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonCheckNiceHashservers.Name = "buttonCheckNiceHashservers";
            this.buttonCheckNiceHashservers.Size = new System.Drawing.Size(144, 23);
            this.buttonCheckNiceHashservers.TabIndex = 45;
            this.buttonCheckNiceHashservers.Text = "Check NiceHash servers";
            this.buttonCheckNiceHashservers.UseVisualStyleBackColor = true;
            this.buttonCheckNiceHashservers.Click += new System.EventHandler(this.button3_Click);
            // 
            // buttonCheckNiceHashStatus
            // 
            this.buttonCheckNiceHashStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCheckNiceHashStatus.Location = new System.Drawing.Point(4, 19);
            this.buttonCheckNiceHashStatus.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonCheckNiceHashStatus.Name = "buttonCheckNiceHashStatus";
            this.buttonCheckNiceHashStatus.Size = new System.Drawing.Size(144, 23);
            this.buttonCheckNiceHashStatus.TabIndex = 44;
            this.buttonCheckNiceHashStatus.Text = "Check NiceHash status";
            this.buttonCheckNiceHashStatus.UseVisualStyleBackColor = true;
            this.buttonCheckNiceHashStatus.Click += new System.EventHandler(this.buttonCheckNiceHashStatus_Click);
            // 
            // groupBoxToolsNET
            // 
            this.groupBoxToolsNET.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxToolsNET.Controls.Add(this.buttonCurrPorts);
            this.groupBoxToolsNET.Location = new System.Drawing.Point(5, 120);
            this.groupBoxToolsNET.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxToolsNET.Name = "groupBoxToolsNET";
            this.groupBoxToolsNET.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxToolsNET.Size = new System.Drawing.Size(657, 108);
            this.groupBoxToolsNET.TabIndex = 397;
            this.groupBoxToolsNET.TabStop = false;
            this.groupBoxToolsNET.Text = "Network";
            // 
            // buttonCurrPorts
            // 
            this.buttonCurrPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCurrPorts.Location = new System.Drawing.Point(4, 19);
            this.buttonCurrPorts.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonCurrPorts.Name = "buttonCurrPorts";
            this.buttonCurrPorts.Size = new System.Drawing.Size(144, 23);
            this.buttonCurrPorts.TabIndex = 45;
            this.buttonCurrPorts.Text = "Run CurrPorts";
            this.buttonCurrPorts.UseVisualStyleBackColor = true;
            this.buttonCurrPorts.Click += new System.EventHandler(this.buttonCurrPorts_Click);
            // 
            // groupBoxToolsGPU
            // 
            this.groupBoxToolsGPU.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxToolsGPU.Controls.Add(this.buttonNVIDIAinspector);
            this.groupBoxToolsGPU.Controls.Add(this.buttonOverdriveNTool);
            this.groupBoxToolsGPU.Location = new System.Drawing.Point(4, 234);
            this.groupBoxToolsGPU.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxToolsGPU.Name = "groupBoxToolsGPU";
            this.groupBoxToolsGPU.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxToolsGPU.Size = new System.Drawing.Size(657, 108);
            this.groupBoxToolsGPU.TabIndex = 396;
            this.groupBoxToolsGPU.TabStop = false;
            this.groupBoxToolsGPU.Text = "GPU";
            // 
            // buttonNVIDIAinspector
            // 
            this.buttonNVIDIAinspector.Location = new System.Drawing.Point(178, 19);
            this.buttonNVIDIAinspector.Name = "buttonNVIDIAinspector";
            this.buttonNVIDIAinspector.Size = new System.Drawing.Size(131, 23);
            this.buttonNVIDIAinspector.TabIndex = 1;
            this.buttonNVIDIAinspector.Text = "NVIDIA Inspector";
            this.buttonNVIDIAinspector.UseVisualStyleBackColor = true;
            this.buttonNVIDIAinspector.Click += new System.EventHandler(this.buttonNVIDIAinspector_Click);
            // 
            // buttonOverdriveNTool
            // 
            this.buttonOverdriveNTool.Location = new System.Drawing.Point(18, 20);
            this.buttonOverdriveNTool.Name = "buttonOverdriveNTool";
            this.buttonOverdriveNTool.Size = new System.Drawing.Size(131, 23);
            this.buttonOverdriveNTool.TabIndex = 0;
            this.buttonOverdriveNTool.Text = "AMD OverdriveNTool";
            this.buttonOverdriveNTool.UseVisualStyleBackColor = true;
            this.buttonOverdriveNTool.Click += new System.EventHandler(this.buttonOverdriveNTool_Click);
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageAbout.Controls.Add(this.groupBoxBackup);
            this.tabPageAbout.Controls.Add(this.groupBoxUpdates);
            this.tabPageAbout.Controls.Add(this.groupBoxInfo);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 23);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Size = new System.Drawing.Size(669, 439);
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
            this.groupBoxBackup.Location = new System.Drawing.Point(4, 97);
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
            this.buttonRestoreBackup.Location = new System.Drawing.Point(387, 14);
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
            this.buttonCreateBackup.Location = new System.Drawing.Point(267, 14);
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
            this.groupBoxUpdates.Location = new System.Drawing.Point(4, 170);
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
            this.checkBoxAutoupdate.Location = new System.Drawing.Point(267, 45);
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
            this.comboBoxCheckforprogramupdatesevery.Location = new System.Drawing.Point(188, 43);
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
            this.linkLabelNewVersion2.Location = new System.Drawing.Point(384, 19);
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
            this.buttonCheckNewVersion.Location = new System.Drawing.Point(267, 14);
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
            this.groupBoxInfo.Controls.Add(this.richTextBoxInfo);
            this.groupBoxInfo.Controls.Add(this.buttonLicence);
            this.groupBoxInfo.Location = new System.Drawing.Point(4, 3);
            this.groupBoxInfo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxInfo.Size = new System.Drawing.Size(659, 88);
            this.groupBoxInfo.TabIndex = 393;
            this.groupBoxInfo.TabStop = false;
            this.groupBoxInfo.Text = "Info";
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
            // checkBox_DisplayConnected
            // 
            this.checkBox_DisplayConnected.AutoSize = true;
            this.checkBox_DisplayConnected.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_DisplayConnected.Location = new System.Drawing.Point(304, 110);
            this.checkBox_DisplayConnected.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisplayConnected.Name = "checkBox_DisplayConnected";
            this.checkBox_DisplayConnected.Size = new System.Drawing.Size(221, 17);
            this.checkBox_DisplayConnected.TabIndex = 416;
            this.checkBox_DisplayConnected.Text = "Show which GPU display is connected to";
            this.checkBox_DisplayConnected.UseVisualStyleBackColor = true;
            // 
            // Form_Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 509);
            this.Controls.Add(this.tabControlGeneral);
            this.Controls.Add(this.buttonDefaults);
            this.Controls.Add(this.buttonSaveClose);
            this.Controls.Add(this.buttonCloseNoSave);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 548);
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
            this.groupBox_Idle.ResumeLayout(false);
            this.groupBox_Idle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_StartMiningWhenIdle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinIdleSeconds)).EndInit();
            this.groupBox_Main.ResumeLayout(false);
            this.groupBox_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ElectricityCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_TimeUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinProfit)).EndInit();
            this.groupBox_Misc.ResumeLayout(false);
            this.groupBox_Misc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinimizeMiningWindows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RunAtStartup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AllowMultipleInstances)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AutoScaleBTCValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AutoStartMining)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinimizeToTray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_HideMiningWindows)).EndInit();
            this.groupBox_Logging.ResumeLayout(false);
            this.groupBox_Logging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LogMaxFileSize)).EndInit();
            this.groupBox_Localization.ResumeLayout(false);
            this.groupBox_Localization.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_displayCurrency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Language)).EndInit();
            this.tabPageAdvanced1.ResumeLayout(false);
            this.groupBoxMOPA.ResumeLayout(false);
            this.groupBoxMOPA.PerformLayout();
            this.groupBox_Miners.ResumeLayout(false);
            this.groupBox_Miners.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SwitchProfitabilityThreshold)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableDetectionAMD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableDetectionNVIDIA)).EndInit();
            this.tabPageDevicesAlgos.ResumeLayout(false);
            this.tabPageDevicesAlgos.PerformLayout();
            this.groupBoxAlgorithmSettings.ResumeLayout(false);
            this.tabPageOverClock.ResumeLayout(false);
            this.tabPageOverClock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBoxOverClockSettings.ResumeLayout(false);
            this.tabPageTools.ResumeLayout(false);
            this.groupBoxToolsNH.ResumeLayout(false);
            this.groupBoxToolsNET.ResumeLayout(false);
            this.groupBoxToolsGPU.ResumeLayout(false);
            this.tabPageAbout.ResumeLayout(false);
            this.groupBoxBackup.ResumeLayout(false);
            this.groupBoxBackup.PerformLayout();
            this.groupBoxUpdates.ResumeLayout(false);
            this.groupBoxUpdates.PerformLayout();
            this.groupBoxInfo.ResumeLayout(false);
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
        private System.Windows.Forms.PictureBox pictureBox_ElectricityCost;
        private System.Windows.Forms.TextBox textBox_ElectricityCost;
        private System.Windows.Forms.Label label_ElectricityCost;
        private System.Windows.Forms.PictureBox pictureBox_TimeUnit;
        private System.Windows.Forms.Label label_TimeUnit;
        private System.Windows.Forms.ComboBox comboBox_TimeUnit;
        private System.Windows.Forms.PictureBox pictureBox_MinProfit;
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
        private System.Windows.Forms.PictureBox pictureBox_MinimizeMiningWindows;
        private System.Windows.Forms.PictureBox pictureBox_RunAtStartup;
        private System.Windows.Forms.CheckBox checkBox_RunAtStartup;
        private System.Windows.Forms.CheckBox checkBox_AllowMultipleInstances;
        private System.Windows.Forms.CheckBox checkBox_AutoStartMining;
        private System.Windows.Forms.CheckBox checkBox_HideMiningWindows;
        private System.Windows.Forms.PictureBox pictureBox_AllowMultipleInstances;
        private System.Windows.Forms.CheckBox checkBox_MinimizeToTray;
        private System.Windows.Forms.PictureBox pictureBox_StartMiningWhenIdle;
        private System.Windows.Forms.PictureBox pictureBox_AutoScaleBTCValues;
        private System.Windows.Forms.PictureBox pictureBox_AutoStartMining;
        private System.Windows.Forms.PictureBox pictureBox_MinimizeToTray;
        private System.Windows.Forms.PictureBox pictureBox_HideMiningWindows;
        private System.Windows.Forms.CheckBox checkBox_AutoScaleBTCValues;
        private System.Windows.Forms.CheckBox checkBox_StartMiningWhenIdle;
        private System.Windows.Forms.GroupBox groupBox_Logging;
        private System.Windows.Forms.Label label_LogMaxFileSize;
        private System.Windows.Forms.TextBox textBox_LogMaxFileSize;
        private System.Windows.Forms.CheckBox checkBox_LogToFile;
        private System.Windows.Forms.PictureBox pictureBox_LogMaxFileSize;
        private System.Windows.Forms.GroupBox groupBox_Localization;
        private System.Windows.Forms.Label label_Language;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox_displayCurrency;
        private System.Windows.Forms.PictureBox pictureBox_Language;
        private System.Windows.Forms.ComboBox comboBox_Language;
        private System.Windows.Forms.ComboBox currencyConverterCombobox;
        private System.Windows.Forms.Label label_displayCurrency;
        private System.Windows.Forms.TabPage tabPageAdvanced1;
        private System.Windows.Forms.GroupBox groupBoxMOPA;
        private System.Windows.Forms.RadioButton radioButtonMOPA5;
        private System.Windows.Forms.RadioButton radioButtonMOPA4;
        private System.Windows.Forms.RadioButton radioButtonMOPA3;
        private System.Windows.Forms.RadioButton radioButtonMOPA2;
        private System.Windows.Forms.RadioButton radioButtonMOPA1;
        private System.Windows.Forms.GroupBox groupBox_Miners;
        private System.Windows.Forms.PictureBox pictureBox_SwitchProfitabilityThreshold;
        private System.Windows.Forms.PictureBox pictureBox_MinIdleSeconds;
        private System.Windows.Forms.Label label_MinIdleSeconds;
        private System.Windows.Forms.TextBox textBox_SwitchProfitabilityThreshold;
        private System.Windows.Forms.Label label_SwitchProfitabilityThreshold;
        private System.Windows.Forms.TextBox textBox_MinIdleSeconds;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionCPU;
        private System.Windows.Forms.CheckBox checkBox_Additional_info_about_device;
        private System.Windows.Forms.PictureBox pictureBox_DisableDetectionAMD;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionNVIDIA;
        private System.Windows.Forms.PictureBox pictureBox_DisableDetectionNVIDIA;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionAMD;
        private System.Windows.Forms.TabPage tabPageDevicesAlgos;
        private System.Windows.Forms.CheckBox checkBox_Disable_extra_launch_parameter_checking;
        private Components.AlgorithmSettingsControl algorithmSettingsControl1;
        private System.Windows.Forms.GroupBox groupBoxAlgorithmSettings;
        private Components.AlgorithmsListView algorithmsListView1;
        private Components.DevicesListViewEnableControl devicesListViewEnableControl1;
        private System.Windows.Forms.CheckBox checkBox_RunEthlargement;
        private System.Windows.Forms.CheckBox checkBox_ShowFanAsPercent;
        private System.Windows.Forms.CheckBox Checkbox_Save_windows_size_and_position;
        private System.Windows.Forms.CheckBox checkbox_Group_same_devices;
        private System.Windows.Forms.GroupBox groupBox_Idle;
        private System.Windows.Forms.Label label_switching_algorithms;
        private System.Windows.Forms.ComboBox comboBox_switching_algorithms;
        private System.Windows.Forms.CheckBox checkBox_Show_profit_with_power_consumption;
        private System.Windows.Forms.TextBox textBox_mb;
        private System.Windows.Forms.Label label_MBpower;
        private System.Windows.Forms.TextBox textBox_psu;
        private System.Windows.Forms.Label label_psu;
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
        private System.Windows.Forms.CheckBox checkBox_ShowUptime;
        private System.Windows.Forms.CheckBox checkBox_program_monitoring;
        private System.Windows.Forms.CheckBox checkBox_BackupBeforeUpdate;
        private System.Windows.Forms.CheckBox checkBoxRestartDriver;
        private System.Windows.Forms.CheckBox checkBoxRestartWindows;
        private System.Windows.Forms.Label labelRestartProgram;
        private System.Windows.Forms.ComboBox comboBoxRestartProgram;
        private System.Windows.Forms.CheckBox checkBoxCPUmonitoring;
        private System.Windows.Forms.CheckBox checkBoxNVMonitoring;
        private System.Windows.Forms.CheckBox checkBoxAMDmonitoring;
        private System.Windows.Forms.TextBox textBoxAddAMD;
        private System.Windows.Forms.Label labelAddAMD;
        private System.Windows.Forms.CheckBox checkBoxDriverWarning;
        private System.Windows.Forms.TextBox textBoxMaxEpoch;
        private System.Windows.Forms.Label labelMaxEpoch;
        private System.Windows.Forms.TabPage tabPageTools;
        private System.Windows.Forms.GroupBox groupBoxToolsNET;
        private System.Windows.Forms.GroupBox groupBoxToolsGPU;
        private System.Windows.Forms.Button buttonOverdriveNTool;
        private System.Windows.Forms.GroupBox groupBoxToolsNH;
        private System.Windows.Forms.Button buttonCheckNiceHashservers;
        private System.Windows.Forms.Button buttonCheckNiceHashStatus;
        private System.Windows.Forms.Button buttonCurrPorts;
        private System.Windows.Forms.Button buttonNVIDIAinspector;
        private System.Windows.Forms.RichTextBox richTextBoxCheckNiceHashservers;
        private System.Windows.Forms.CheckBox checkBox_show_NVdevice_manufacturer;
        private System.Windows.Forms.CheckBox checkBox_ShowDeviceMemSize;
        private System.Windows.Forms.CheckBox checkBox_show_AMDdevice_manufacturer;
        private System.Windows.Forms.CheckBox checkbox_current_actual_profitabilities;
        private System.Windows.Forms.TabPage tabPageOverClock;
        private Components.DevicesListViewEnableControl devicesListViewEnableControl2;
        private System.Windows.Forms.GroupBox groupBoxOverClockSettings;
        private Components.AlgorithmsListViewOverClock algorithmsListViewOverClock1;
        private System.Windows.Forms.CheckBox checkBox_ABEnableOverclock;
        private System.Windows.Forms.CheckBox checkBox_ABMinimize;
        private System.Windows.Forms.CheckBox checkBox_By_profitability_of_all_devices;
        public System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.CheckBox checkBox_DisplayConnected;
    }
}
