/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
            this.groupBox_Main = new System.Windows.Forms.GroupBox();
            this.checkBox_Force_mining_if_nonprofitable = new System.Windows.Forms.CheckBox();
            this.pictureBox_ElectricityCost = new System.Windows.Forms.PictureBox();
            this.textBox_ElectricityCost = new System.Windows.Forms.TextBox();
            this.label_ElectricityCost = new System.Windows.Forms.Label();
            this.pictureBox_TimeUnit = new System.Windows.Forms.PictureBox();
            this.label_TimeUnit = new System.Windows.Forms.Label();
            this.comboBox_TimeUnit = new System.Windows.Forms.ComboBox();
            this.label_IFTTTAPIKey = new System.Windows.Forms.Label();
            this.textBox_IFTTTKey = new System.Windows.Forms.TextBox();
            this.pictureBox_UseIFTTT = new System.Windows.Forms.PictureBox();
            this.checkBox_UseIFTTT = new System.Windows.Forms.CheckBox();
            this.checkBox_IdleWhenNoInternetAccess = new System.Windows.Forms.CheckBox();
            this.pictureBox_MinProfit = new System.Windows.Forms.PictureBox();
            this.textBox_MinProfit = new System.Windows.Forms.TextBox();
            this.pictureBox_IdleWhenNoInternetAccess = new System.Windows.Forms.PictureBox();
            this.label_MinProfit = new System.Windows.Forms.Label();
            this.groupBox_Misc = new System.Windows.Forms.GroupBox();
            this.Checkbox_Save_windows_size_and_position = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_AutoStartMiningDelay = new System.Windows.Forms.TextBox();
            this.label_AutoStartMiningDelay = new System.Windows.Forms.Label();
            this.comboBox_ColorProfile = new System.Windows.Forms.ComboBox();
            this.checkBox_Send_actual_version_info = new System.Windows.Forms.CheckBox();
            this.checkBox_Allow_remote_management = new System.Windows.Forms.CheckBox();
            this.pictureBox_RunScriptOnCUDA_GPU_Lost = new System.Windows.Forms.PictureBox();
            this.checkBox_RunScriptOnCUDA_GPU_Lost = new System.Windows.Forms.CheckBox();
            this.pictureBox_ShowInternetConnectionWarning = new System.Windows.Forms.PictureBox();
            this.checkBox_ShowInternetConnectionWarning = new System.Windows.Forms.CheckBox();
            this.checkBox_MinimizeMiningWindows = new System.Windows.Forms.CheckBox();
            this.pictureBox_MinimizeMiningWindows = new System.Windows.Forms.PictureBox();
            this.pictureBox_RunAtStartup = new System.Windows.Forms.PictureBox();
            this.checkBox_RunAtStartup = new System.Windows.Forms.CheckBox();
            this.checkBox_AllowMultipleInstances = new System.Windows.Forms.CheckBox();
            this.checkBox_AutoStartMining = new System.Windows.Forms.CheckBox();
            this.checkBox_HideMiningWindows = new System.Windows.Forms.CheckBox();
            this.pictureBox_AllowMultipleInstances = new System.Windows.Forms.PictureBox();
            this.checkBox_MinimizeToTray = new System.Windows.Forms.CheckBox();
            this.pictureBox_DisableWindowsErrorReporting = new System.Windows.Forms.PictureBox();
            this.pictureBox_ShowDriverVersionWarning = new System.Windows.Forms.PictureBox();
            this.pictureBox_StartMiningWhenIdle = new System.Windows.Forms.PictureBox();
            this.pictureBox_AutoScaleBTCValues = new System.Windows.Forms.PictureBox();
            this.pictureBox_AutoStartMining = new System.Windows.Forms.PictureBox();
            this.pictureBox_MinimizeToTray = new System.Windows.Forms.PictureBox();
            this.pictureBox_HideMiningWindows = new System.Windows.Forms.PictureBox();
            this.checkBox_AutoScaleBTCValues = new System.Windows.Forms.CheckBox();
            this.checkBox_DisableWindowsErrorReporting = new System.Windows.Forms.CheckBox();
            this.checkBox_StartMiningWhenIdle = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowDriverVersionWarning = new System.Windows.Forms.CheckBox();
            this.groupBox_Logging = new System.Windows.Forms.GroupBox();
            this.label_LogMaxFileSize = new System.Windows.Forms.Label();
            this.textBox_LogMaxFileSize = new System.Windows.Forms.TextBox();
            this.checkBox_LogToFile = new System.Windows.Forms.CheckBox();
            this.pictureBox_DebugConsole = new System.Windows.Forms.PictureBox();
            this.pictureBox_LogMaxFileSize = new System.Windows.Forms.PictureBox();
            this.pictureBox_LogToFile = new System.Windows.Forms.PictureBox();
            this.checkBox_DebugConsole = new System.Windows.Forms.CheckBox();
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
            this.label_RunEthlargement = new System.Windows.Forms.Label();
            this.checkBox_RunEthlargement = new System.Windows.Forms.CheckBox();
            this.pictureBox_MinerRestartDelayMS = new System.Windows.Forms.PictureBox();
            this.pictureBox_APIBindPortStart = new System.Windows.Forms.PictureBox();
            this.pictureBox_SwitchMaxSeconds = new System.Windows.Forms.PictureBox();
            this.pictureBox_SwitchProfitabilityThreshold = new System.Windows.Forms.PictureBox();
            this.pictureBox_SwitchMinSeconds = new System.Windows.Forms.PictureBox();
            this.pictureBox_MinIdleSeconds = new System.Windows.Forms.PictureBox();
            this.label_MinIdleSeconds = new System.Windows.Forms.Label();
            this.label_SwitchMaxSeconds = new System.Windows.Forms.Label();
            this.label_MinerRestartDelayMS = new System.Windows.Forms.Label();
            this.textBox_SwitchMinSeconds = new System.Windows.Forms.TextBox();
            this.label_APIBindPortStart = new System.Windows.Forms.Label();
            this.textBox_SwitchProfitabilityThreshold = new System.Windows.Forms.TextBox();
            this.label_SwitchProfitabilityThreshold = new System.Windows.Forms.Label();
            this.textBox_APIBindPortStart = new System.Windows.Forms.TextBox();
            this.label_SwitchMinSeconds = new System.Windows.Forms.Label();
            this.textBox_MinIdleSeconds = new System.Windows.Forms.TextBox();
            this.textBox_SwitchMaxSeconds = new System.Windows.Forms.TextBox();
            this.textBox_MinerRestartDelayMS = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkbox_Group_same_devices = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowFanAsPercent = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBox_DisableDetectionCPU = new System.Windows.Forms.CheckBox();
            this.checkBox_Additional_info_about_device = new System.Windows.Forms.CheckBox();
            this.pictureBox_NVIDIAP0State = new System.Windows.Forms.PictureBox();
            this.checkBox_NVIDIAP0State = new System.Windows.Forms.CheckBox();
            this.pictureBox_DisableDetectionAMD = new System.Windows.Forms.PictureBox();
            this.checkBox_DisableDetectionNVIDIA = new System.Windows.Forms.CheckBox();
            this.pictureBox_DisableDetectionNVIDIA = new System.Windows.Forms.PictureBox();
            this.checkBox_DisableDetectionAMD = new System.Windows.Forms.CheckBox();
            this.tabPageDevicesAlgos = new System.Windows.Forms.TabPage();
            this.checkBox_Disable_extra_launch_parameter_checking = new System.Windows.Forms.CheckBox();
            this.groupBoxAlgorithmSettings = new System.Windows.Forms.GroupBox();
            this.algorithmsListView1 = new NiceHashMiner.Forms.Components.AlgorithmsListView();
            this.buttonGPUtuning = new System.Windows.Forms.Button();
            this.algorithmSettingsControl1 = new NiceHashMiner.Forms.Components.AlgorithmSettingsControl();
            this.devicesListViewEnableControl1 = new NiceHashMiner.Forms.Components.DevicesListViewEnableControl();
            this.tabControlGeneral.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBox_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ElectricityCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_TimeUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_UseIFTTT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinProfit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_IdleWhenNoInternetAccess)).BeginInit();
            this.groupBox_Misc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RunScriptOnCUDA_GPU_Lost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ShowInternetConnectionWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinimizeMiningWindows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RunAtStartup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AllowMultipleInstances)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableWindowsErrorReporting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ShowDriverVersionWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_StartMiningWhenIdle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AutoScaleBTCValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AutoStartMining)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinimizeToTray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_HideMiningWindows)).BeginInit();
            this.groupBox_Logging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DebugConsole)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LogMaxFileSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LogToFile)).BeginInit();
            this.groupBox_Localization.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_displayCurrency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Language)).BeginInit();
            this.tabPageAdvanced1.SuspendLayout();
            this.groupBoxMOPA.SuspendLayout();
            this.groupBox_Miners.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinerRestartDelayMS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_APIBindPortStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SwitchMaxSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SwitchProfitabilityThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SwitchMinSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinIdleSeconds)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_NVIDIAP0State)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableDetectionAMD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableDetectionNVIDIA)).BeginInit();
            this.tabPageDevicesAlgos.SuspendLayout();
            this.groupBoxAlgorithmSettings.SuspendLayout();
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
            // groupBox_Main
            // 
            this.groupBox_Main.Controls.Add(this.checkBox_Force_mining_if_nonprofitable);
            this.groupBox_Main.Controls.Add(this.pictureBox_ElectricityCost);
            this.groupBox_Main.Controls.Add(this.textBox_ElectricityCost);
            this.groupBox_Main.Controls.Add(this.label_ElectricityCost);
            this.groupBox_Main.Controls.Add(this.pictureBox_TimeUnit);
            this.groupBox_Main.Controls.Add(this.label_TimeUnit);
            this.groupBox_Main.Controls.Add(this.comboBox_TimeUnit);
            this.groupBox_Main.Controls.Add(this.label_IFTTTAPIKey);
            this.groupBox_Main.Controls.Add(this.textBox_IFTTTKey);
            this.groupBox_Main.Controls.Add(this.pictureBox_UseIFTTT);
            this.groupBox_Main.Controls.Add(this.checkBox_UseIFTTT);
            this.groupBox_Main.Controls.Add(this.checkBox_IdleWhenNoInternetAccess);
            this.groupBox_Main.Controls.Add(this.pictureBox_MinProfit);
            this.groupBox_Main.Controls.Add(this.textBox_MinProfit);
            this.groupBox_Main.Controls.Add(this.pictureBox_IdleWhenNoInternetAccess);
            this.groupBox_Main.Controls.Add(this.label_MinProfit);
            this.groupBox_Main.Location = new System.Drawing.Point(6, 6);
            this.groupBox_Main.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Main.Name = "groupBox_Main";
            this.groupBox_Main.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Main.Size = new System.Drawing.Size(346, 199);
            this.groupBox_Main.TabIndex = 386;
            this.groupBox_Main.TabStop = false;
            this.groupBox_Main.Text = "Main:";
            // 
            // checkBox_Force_mining_if_nonprofitable
            // 
            this.checkBox_Force_mining_if_nonprofitable.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Force_mining_if_nonprofitable.Location = new System.Drawing.Point(156, 151);
            this.checkBox_Force_mining_if_nonprofitable.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Force_mining_if_nonprofitable.Name = "checkBox_Force_mining_if_nonprofitable";
            this.checkBox_Force_mining_if_nonprofitable.Size = new System.Drawing.Size(188, 38);
            this.checkBox_Force_mining_if_nonprofitable.TabIndex = 376;
            this.checkBox_Force_mining_if_nonprofitable.Text = "Force mining if nonprofitable";
            this.checkBox_Force_mining_if_nonprofitable.UseVisualStyleBackColor = true;
            // 
            // pictureBox_ElectricityCost
            // 
            this.pictureBox_ElectricityCost.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_ElectricityCost.Location = new System.Drawing.Point(132, 138);
            this.pictureBox_ElectricityCost.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_ElectricityCost.Name = "pictureBox_ElectricityCost";
            this.pictureBox_ElectricityCost.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_ElectricityCost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_ElectricityCost.TabIndex = 375;
            this.pictureBox_ElectricityCost.TabStop = false;
            // 
            // textBox_ElectricityCost
            // 
            this.textBox_ElectricityCost.Location = new System.Drawing.Point(12, 158);
            this.textBox_ElectricityCost.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_ElectricityCost.Name = "textBox_ElectricityCost";
            this.textBox_ElectricityCost.Size = new System.Drawing.Size(138, 20);
            this.textBox_ElectricityCost.TabIndex = 373;
            // 
            // label_ElectricityCost
            // 
            this.label_ElectricityCost.AutoSize = true;
            this.label_ElectricityCost.Location = new System.Drawing.Point(12, 138);
            this.label_ElectricityCost.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_ElectricityCost.Name = "label_ElectricityCost";
            this.label_ElectricityCost.Size = new System.Drawing.Size(117, 13);
            this.label_ElectricityCost.TabIndex = 374;
            this.label_ElectricityCost.Text = "Electricity Cost (/KWh):";
            // 
            // pictureBox_TimeUnit
            // 
            this.pictureBox_TimeUnit.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_TimeUnit.Location = new System.Drawing.Point(310, 19);
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
            this.label_TimeUnit.Location = new System.Drawing.Point(168, 19);
            this.label_TimeUnit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_TimeUnit.Name = "label_TimeUnit";
            this.label_TimeUnit.Size = new System.Drawing.Size(52, 13);
            this.label_TimeUnit.TabIndex = 371;
            this.label_TimeUnit.Text = "TimeUnit:";
            // 
            // comboBox_TimeUnit
            // 
            this.comboBox_TimeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TimeUnit.FormattingEnabled = true;
            this.comboBox_TimeUnit.Location = new System.Drawing.Point(168, 40);
            this.comboBox_TimeUnit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_TimeUnit.Name = "comboBox_TimeUnit";
            this.comboBox_TimeUnit.Size = new System.Drawing.Size(160, 21);
            this.comboBox_TimeUnit.TabIndex = 370;
            this.comboBox_TimeUnit.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_TimeUnit_DrawItem);
            this.comboBox_TimeUnit.SelectedIndexChanged += new System.EventHandler(this.comboBox_TimeUnit_SelectedIndexChanged);
            // 
            // label_IFTTTAPIKey
            // 
            this.label_IFTTTAPIKey.AutoSize = true;
            this.label_IFTTTAPIKey.Location = new System.Drawing.Point(115, 90);
            this.label_IFTTTAPIKey.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_IFTTTAPIKey.Name = "label_IFTTTAPIKey";
            this.label_IFTTTAPIKey.Size = new System.Drawing.Size(81, 13);
            this.label_IFTTTAPIKey.TabIndex = 369;
            this.label_IFTTTAPIKey.Text = "IFTTT API Key:";
            // 
            // textBox_IFTTTKey
            // 
            this.textBox_IFTTTKey.Enabled = false;
            this.textBox_IFTTTKey.Location = new System.Drawing.Point(112, 107);
            this.textBox_IFTTTKey.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_IFTTTKey.Name = "textBox_IFTTTKey";
            this.textBox_IFTTTKey.Size = new System.Drawing.Size(210, 20);
            this.textBox_IFTTTKey.TabIndex = 368;
            // 
            // pictureBox_UseIFTTT
            // 
            this.pictureBox_UseIFTTT.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_UseIFTTT.Location = new System.Drawing.Point(90, 104);
            this.pictureBox_UseIFTTT.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_UseIFTTT.Name = "pictureBox_UseIFTTT";
            this.pictureBox_UseIFTTT.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_UseIFTTT.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_UseIFTTT.TabIndex = 367;
            this.pictureBox_UseIFTTT.TabStop = false;
            // 
            // checkBox_UseIFTTT
            // 
            this.checkBox_UseIFTTT.AutoSize = true;
            this.checkBox_UseIFTTT.Location = new System.Drawing.Point(12, 108);
            this.checkBox_UseIFTTT.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_UseIFTTT.Name = "checkBox_UseIFTTT";
            this.checkBox_UseIFTTT.Size = new System.Drawing.Size(78, 17);
            this.checkBox_UseIFTTT.TabIndex = 366;
            this.checkBox_UseIFTTT.Text = "Use IFTTT";
            this.checkBox_UseIFTTT.UseVisualStyleBackColor = true;
            // 
            // checkBox_IdleWhenNoInternetAccess
            // 
            this.checkBox_IdleWhenNoInternetAccess.AutoSize = true;
            this.checkBox_IdleWhenNoInternetAccess.Location = new System.Drawing.Point(12, 66);
            this.checkBox_IdleWhenNoInternetAccess.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_IdleWhenNoInternetAccess.Name = "checkBox_IdleWhenNoInternetAccess";
            this.checkBox_IdleWhenNoInternetAccess.Size = new System.Drawing.Size(205, 17);
            this.checkBox_IdleWhenNoInternetAccess.TabIndex = 365;
            this.checkBox_IdleWhenNoInternetAccess.Text = "Continue Mining If No Internet Access";
            this.checkBox_IdleWhenNoInternetAccess.UseVisualStyleBackColor = true;
            // 
            // pictureBox_MinProfit
            // 
            this.pictureBox_MinProfit.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_MinProfit.Location = new System.Drawing.Point(132, 20);
            this.pictureBox_MinProfit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_MinProfit.Name = "pictureBox_MinProfit";
            this.pictureBox_MinProfit.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_MinProfit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_MinProfit.TabIndex = 364;
            this.pictureBox_MinProfit.TabStop = false;
            // 
            // textBox_MinProfit
            // 
            this.textBox_MinProfit.Location = new System.Drawing.Point(12, 40);
            this.textBox_MinProfit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MinProfit.Name = "textBox_MinProfit";
            this.textBox_MinProfit.Size = new System.Drawing.Size(138, 20);
            this.textBox_MinProfit.TabIndex = 334;
            // 
            // pictureBox_IdleWhenNoInternetAccess
            // 
            this.pictureBox_IdleWhenNoInternetAccess.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_IdleWhenNoInternetAccess.Location = new System.Drawing.Point(310, 66);
            this.pictureBox_IdleWhenNoInternetAccess.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_IdleWhenNoInternetAccess.Name = "pictureBox_IdleWhenNoInternetAccess";
            this.pictureBox_IdleWhenNoInternetAccess.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_IdleWhenNoInternetAccess.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_IdleWhenNoInternetAccess.TabIndex = 364;
            this.pictureBox_IdleWhenNoInternetAccess.TabStop = false;
            // 
            // label_MinProfit
            // 
            this.label_MinProfit.AutoSize = true;
            this.label_MinProfit.Location = new System.Drawing.Point(12, 20);
            this.label_MinProfit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MinProfit.Name = "label_MinProfit";
            this.label_MinProfit.Size = new System.Drawing.Size(115, 13);
            this.label_MinProfit.TabIndex = 357;
            this.label_MinProfit.Text = "Minimum Profit ($/day):";
            // 
            // groupBox_Misc
            // 
            this.groupBox_Misc.Controls.Add(this.Checkbox_Save_windows_size_and_position);
            this.groupBox_Misc.Controls.Add(this.label1);
            this.groupBox_Misc.Controls.Add(this.textBox_AutoStartMiningDelay);
            this.groupBox_Misc.Controls.Add(this.label_AutoStartMiningDelay);
            this.groupBox_Misc.Controls.Add(this.comboBox_ColorProfile);
            this.groupBox_Misc.Controls.Add(this.checkBox_Send_actual_version_info);
            this.groupBox_Misc.Controls.Add(this.checkBox_Allow_remote_management);
            this.groupBox_Misc.Controls.Add(this.pictureBox_RunScriptOnCUDA_GPU_Lost);
            this.groupBox_Misc.Controls.Add(this.checkBox_RunScriptOnCUDA_GPU_Lost);
            this.groupBox_Misc.Controls.Add(this.pictureBox_ShowInternetConnectionWarning);
            this.groupBox_Misc.Controls.Add(this.checkBox_ShowInternetConnectionWarning);
            this.groupBox_Misc.Controls.Add(this.checkBox_MinimizeMiningWindows);
            this.groupBox_Misc.Controls.Add(this.pictureBox_MinimizeMiningWindows);
            this.groupBox_Misc.Controls.Add(this.pictureBox_RunAtStartup);
            this.groupBox_Misc.Controls.Add(this.checkBox_RunAtStartup);
            this.groupBox_Misc.Controls.Add(this.checkBox_AllowMultipleInstances);
            this.groupBox_Misc.Controls.Add(this.checkBox_AutoStartMining);
            this.groupBox_Misc.Controls.Add(this.checkBox_HideMiningWindows);
            this.groupBox_Misc.Controls.Add(this.pictureBox_AllowMultipleInstances);
            this.groupBox_Misc.Controls.Add(this.checkBox_MinimizeToTray);
            this.groupBox_Misc.Controls.Add(this.pictureBox_DisableWindowsErrorReporting);
            this.groupBox_Misc.Controls.Add(this.pictureBox_ShowDriverVersionWarning);
            this.groupBox_Misc.Controls.Add(this.pictureBox_StartMiningWhenIdle);
            this.groupBox_Misc.Controls.Add(this.pictureBox_AutoScaleBTCValues);
            this.groupBox_Misc.Controls.Add(this.pictureBox_AutoStartMining);
            this.groupBox_Misc.Controls.Add(this.pictureBox_MinimizeToTray);
            this.groupBox_Misc.Controls.Add(this.pictureBox_HideMiningWindows);
            this.groupBox_Misc.Controls.Add(this.checkBox_AutoScaleBTCValues);
            this.groupBox_Misc.Controls.Add(this.checkBox_DisableWindowsErrorReporting);
            this.groupBox_Misc.Controls.Add(this.checkBox_StartMiningWhenIdle);
            this.groupBox_Misc.Controls.Add(this.checkBox_ShowDriverVersionWarning);
            this.groupBox_Misc.Location = new System.Drawing.Point(358, 6);
            this.groupBox_Misc.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Misc.Name = "groupBox_Misc";
            this.groupBox_Misc.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Misc.Size = new System.Drawing.Size(288, 429);
            this.groupBox_Misc.TabIndex = 391;
            this.groupBox_Misc.TabStop = false;
            this.groupBox_Misc.Text = "Misc:";
            // 
            // Checkbox_Save_windows_size_and_position
            // 
            this.Checkbox_Save_windows_size_and_position.AutoSize = true;
            this.Checkbox_Save_windows_size_and_position.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Checkbox_Save_windows_size_and_position.Location = new System.Drawing.Point(4, 400);
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
            this.label1.Location = new System.Drawing.Point(4, 370);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 379;
            this.label1.Text = "Color profile";
            // 
            // textBox_AutoStartMiningDelay
            // 
            this.textBox_AutoStartMiningDelay.Location = new System.Drawing.Point(241, 42);
            this.textBox_AutoStartMiningDelay.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_AutoStartMiningDelay.Name = "textBox_AutoStartMiningDelay";
            this.textBox_AutoStartMiningDelay.Size = new System.Drawing.Size(27, 20);
            this.textBox_AutoStartMiningDelay.TabIndex = 377;
            // 
            // label_AutoStartMiningDelay
            // 
            this.label_AutoStartMiningDelay.AutoSize = true;
            this.label_AutoStartMiningDelay.Location = new System.Drawing.Point(28, 45);
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
            this.comboBox_ColorProfile.Location = new System.Drawing.Point(166, 366);
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
            this.checkBox_Send_actual_version_info.Location = new System.Drawing.Point(4, 343);
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
            this.checkBox_Allow_remote_management.Location = new System.Drawing.Point(4, 320);
            this.checkBox_Allow_remote_management.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Allow_remote_management.Name = "checkBox_Allow_remote_management";
            this.checkBox_Allow_remote_management.Size = new System.Drawing.Size(150, 17);
            this.checkBox_Allow_remote_management.TabIndex = 374;
            this.checkBox_Allow_remote_management.Text = "Allow remote management";
            this.checkBox_Allow_remote_management.UseVisualStyleBackColor = true;
            // 
            // pictureBox_RunScriptOnCUDA_GPU_Lost
            // 
            this.pictureBox_RunScriptOnCUDA_GPU_Lost.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_RunScriptOnCUDA_GPU_Lost.Location = new System.Drawing.Point(250, 296);
            this.pictureBox_RunScriptOnCUDA_GPU_Lost.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_RunScriptOnCUDA_GPU_Lost.Name = "pictureBox_RunScriptOnCUDA_GPU_Lost";
            this.pictureBox_RunScriptOnCUDA_GPU_Lost.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_RunScriptOnCUDA_GPU_Lost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_RunScriptOnCUDA_GPU_Lost.TabIndex = 373;
            this.pictureBox_RunScriptOnCUDA_GPU_Lost.TabStop = false;
            // 
            // checkBox_RunScriptOnCUDA_GPU_Lost
            // 
            this.checkBox_RunScriptOnCUDA_GPU_Lost.AutoSize = true;
            this.checkBox_RunScriptOnCUDA_GPU_Lost.Location = new System.Drawing.Point(4, 297);
            this.checkBox_RunScriptOnCUDA_GPU_Lost.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_RunScriptOnCUDA_GPU_Lost.Name = "checkBox_RunScriptOnCUDA_GPU_Lost";
            this.checkBox_RunScriptOnCUDA_GPU_Lost.Size = new System.Drawing.Size(191, 17);
            this.checkBox_RunScriptOnCUDA_GPU_Lost.TabIndex = 372;
            this.checkBox_RunScriptOnCUDA_GPU_Lost.Text = "Run script when CUDA GPU is lost";
            this.checkBox_RunScriptOnCUDA_GPU_Lost.UseVisualStyleBackColor = true;
            // 
            // pictureBox_ShowInternetConnectionWarning
            // 
            this.pictureBox_ShowInternetConnectionWarning.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_ShowInternetConnectionWarning.Location = new System.Drawing.Point(250, 273);
            this.pictureBox_ShowInternetConnectionWarning.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_ShowInternetConnectionWarning.Name = "pictureBox_ShowInternetConnectionWarning";
            this.pictureBox_ShowInternetConnectionWarning.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_ShowInternetConnectionWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_ShowInternetConnectionWarning.TabIndex = 371;
            this.pictureBox_ShowInternetConnectionWarning.TabStop = false;
            // 
            // checkBox_ShowInternetConnectionWarning
            // 
            this.checkBox_ShowInternetConnectionWarning.AutoSize = true;
            this.checkBox_ShowInternetConnectionWarning.Location = new System.Drawing.Point(4, 274);
            this.checkBox_ShowInternetConnectionWarning.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ShowInternetConnectionWarning.Name = "checkBox_ShowInternetConnectionWarning";
            this.checkBox_ShowInternetConnectionWarning.Size = new System.Drawing.Size(192, 17);
            this.checkBox_ShowInternetConnectionWarning.TabIndex = 370;
            this.checkBox_ShowInternetConnectionWarning.Text = "Show Internet Connection Warning";
            this.checkBox_ShowInternetConnectionWarning.UseVisualStyleBackColor = true;
            // 
            // checkBox_MinimizeMiningWindows
            // 
            this.checkBox_MinimizeMiningWindows.AutoSize = true;
            this.checkBox_MinimizeMiningWindows.Location = new System.Drawing.Point(4, 90);
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
            this.pictureBox_MinimizeMiningWindows.Location = new System.Drawing.Point(250, 89);
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
            this.pictureBox_RunAtStartup.Location = new System.Drawing.Point(250, 250);
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
            this.checkBox_RunAtStartup.Location = new System.Drawing.Point(4, 251);
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
            this.checkBox_AllowMultipleInstances.Location = new System.Drawing.Point(4, 228);
            this.checkBox_AllowMultipleInstances.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AllowMultipleInstances.Name = "checkBox_AllowMultipleInstances";
            this.checkBox_AllowMultipleInstances.Size = new System.Drawing.Size(139, 17);
            this.checkBox_AllowMultipleInstances.TabIndex = 365;
            this.checkBox_AllowMultipleInstances.Text = "Allow Multiple Instances";
            this.checkBox_AllowMultipleInstances.UseVisualStyleBackColor = true;
            // 
            // checkBox_AutoStartMining
            // 
            this.checkBox_AutoStartMining.AutoSize = true;
            this.checkBox_AutoStartMining.Location = new System.Drawing.Point(6, 19);
            this.checkBox_AutoStartMining.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AutoStartMining.Name = "checkBox_AutoStartMining";
            this.checkBox_AutoStartMining.Size = new System.Drawing.Size(102, 17);
            this.checkBox_AutoStartMining.TabIndex = 315;
            this.checkBox_AutoStartMining.Text = "Autostart Mining";
            this.checkBox_AutoStartMining.UseVisualStyleBackColor = true;
            // 
            // checkBox_HideMiningWindows
            // 
            this.checkBox_HideMiningWindows.AutoSize = true;
            this.checkBox_HideMiningWindows.Location = new System.Drawing.Point(4, 67);
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
            this.pictureBox_AllowMultipleInstances.Location = new System.Drawing.Point(250, 227);
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
            this.checkBox_MinimizeToTray.Location = new System.Drawing.Point(4, 113);
            this.checkBox_MinimizeToTray.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_MinimizeToTray.Name = "checkBox_MinimizeToTray";
            this.checkBox_MinimizeToTray.Size = new System.Drawing.Size(100, 17);
            this.checkBox_MinimizeToTray.TabIndex = 316;
            this.checkBox_MinimizeToTray.Text = "MinimizeToTray";
            this.checkBox_MinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // pictureBox_DisableWindowsErrorReporting
            // 
            this.pictureBox_DisableWindowsErrorReporting.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_DisableWindowsErrorReporting.Location = new System.Drawing.Point(250, 204);
            this.pictureBox_DisableWindowsErrorReporting.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_DisableWindowsErrorReporting.Name = "pictureBox_DisableWindowsErrorReporting";
            this.pictureBox_DisableWindowsErrorReporting.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_DisableWindowsErrorReporting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_DisableWindowsErrorReporting.TabIndex = 364;
            this.pictureBox_DisableWindowsErrorReporting.TabStop = false;
            // 
            // pictureBox_ShowDriverVersionWarning
            // 
            this.pictureBox_ShowDriverVersionWarning.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_ShowDriverVersionWarning.Location = new System.Drawing.Point(250, 181);
            this.pictureBox_ShowDriverVersionWarning.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_ShowDriverVersionWarning.Name = "pictureBox_ShowDriverVersionWarning";
            this.pictureBox_ShowDriverVersionWarning.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_ShowDriverVersionWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_ShowDriverVersionWarning.TabIndex = 364;
            this.pictureBox_ShowDriverVersionWarning.TabStop = false;
            // 
            // pictureBox_StartMiningWhenIdle
            // 
            this.pictureBox_StartMiningWhenIdle.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_StartMiningWhenIdle.Location = new System.Drawing.Point(250, 158);
            this.pictureBox_StartMiningWhenIdle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_StartMiningWhenIdle.Name = "pictureBox_StartMiningWhenIdle";
            this.pictureBox_StartMiningWhenIdle.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_StartMiningWhenIdle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_StartMiningWhenIdle.TabIndex = 364;
            this.pictureBox_StartMiningWhenIdle.TabStop = false;
            // 
            // pictureBox_AutoScaleBTCValues
            // 
            this.pictureBox_AutoScaleBTCValues.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_AutoScaleBTCValues.Location = new System.Drawing.Point(250, 135);
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
            this.pictureBox_AutoStartMining.Location = new System.Drawing.Point(252, 18);
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
            this.pictureBox_MinimizeToTray.Location = new System.Drawing.Point(250, 112);
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
            this.pictureBox_HideMiningWindows.Location = new System.Drawing.Point(250, 66);
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
            this.checkBox_AutoScaleBTCValues.Location = new System.Drawing.Point(4, 136);
            this.checkBox_AutoScaleBTCValues.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_AutoScaleBTCValues.Name = "checkBox_AutoScaleBTCValues";
            this.checkBox_AutoScaleBTCValues.Size = new System.Drawing.Size(128, 17);
            this.checkBox_AutoScaleBTCValues.TabIndex = 321;
            this.checkBox_AutoScaleBTCValues.Text = "AutoScaleBTCValues";
            this.checkBox_AutoScaleBTCValues.UseVisualStyleBackColor = true;
            // 
            // checkBox_DisableWindowsErrorReporting
            // 
            this.checkBox_DisableWindowsErrorReporting.AutoSize = true;
            this.checkBox_DisableWindowsErrorReporting.Location = new System.Drawing.Point(4, 205);
            this.checkBox_DisableWindowsErrorReporting.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableWindowsErrorReporting.Name = "checkBox_DisableWindowsErrorReporting";
            this.checkBox_DisableWindowsErrorReporting.Size = new System.Drawing.Size(173, 17);
            this.checkBox_DisableWindowsErrorReporting.TabIndex = 324;
            this.checkBox_DisableWindowsErrorReporting.Text = "DisableWindowsErrorReporting";
            this.checkBox_DisableWindowsErrorReporting.UseVisualStyleBackColor = true;
            // 
            // checkBox_StartMiningWhenIdle
            // 
            this.checkBox_StartMiningWhenIdle.AutoSize = true;
            this.checkBox_StartMiningWhenIdle.Location = new System.Drawing.Point(4, 159);
            this.checkBox_StartMiningWhenIdle.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_StartMiningWhenIdle.Name = "checkBox_StartMiningWhenIdle";
            this.checkBox_StartMiningWhenIdle.Size = new System.Drawing.Size(125, 17);
            this.checkBox_StartMiningWhenIdle.TabIndex = 322;
            this.checkBox_StartMiningWhenIdle.Text = "StartMiningWhenIdle";
            this.checkBox_StartMiningWhenIdle.UseVisualStyleBackColor = true;
            // 
            // checkBox_ShowDriverVersionWarning
            // 
            this.checkBox_ShowDriverVersionWarning.AutoSize = true;
            this.checkBox_ShowDriverVersionWarning.Location = new System.Drawing.Point(4, 182);
            this.checkBox_ShowDriverVersionWarning.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_ShowDriverVersionWarning.Name = "checkBox_ShowDriverVersionWarning";
            this.checkBox_ShowDriverVersionWarning.Size = new System.Drawing.Size(156, 17);
            this.checkBox_ShowDriverVersionWarning.TabIndex = 323;
            this.checkBox_ShowDriverVersionWarning.Text = "ShowDriverVersionWarning";
            this.checkBox_ShowDriverVersionWarning.UseVisualStyleBackColor = true;
            // 
            // groupBox_Logging
            // 
            this.groupBox_Logging.Controls.Add(this.label_LogMaxFileSize);
            this.groupBox_Logging.Controls.Add(this.textBox_LogMaxFileSize);
            this.groupBox_Logging.Controls.Add(this.checkBox_LogToFile);
            this.groupBox_Logging.Controls.Add(this.pictureBox_DebugConsole);
            this.groupBox_Logging.Controls.Add(this.pictureBox_LogMaxFileSize);
            this.groupBox_Logging.Controls.Add(this.pictureBox_LogToFile);
            this.groupBox_Logging.Controls.Add(this.checkBox_DebugConsole);
            this.groupBox_Logging.Location = new System.Drawing.Point(6, 287);
            this.groupBox_Logging.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Logging.Name = "groupBox_Logging";
            this.groupBox_Logging.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Logging.Size = new System.Drawing.Size(346, 70);
            this.groupBox_Logging.TabIndex = 388;
            this.groupBox_Logging.TabStop = false;
            this.groupBox_Logging.Text = "Logging:";
            // 
            // label_LogMaxFileSize
            // 
            this.label_LogMaxFileSize.AutoSize = true;
            this.label_LogMaxFileSize.Location = new System.Drawing.Point(174, 19);
            this.label_LogMaxFileSize.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LogMaxFileSize.Name = "label_LogMaxFileSize";
            this.label_LogMaxFileSize.Size = new System.Drawing.Size(84, 13);
            this.label_LogMaxFileSize.TabIndex = 357;
            this.label_LogMaxFileSize.Text = "LogMaxFileSize:";
            // 
            // textBox_LogMaxFileSize
            // 
            this.textBox_LogMaxFileSize.Location = new System.Drawing.Point(174, 41);
            this.textBox_LogMaxFileSize.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_LogMaxFileSize.Name = "textBox_LogMaxFileSize";
            this.textBox_LogMaxFileSize.Size = new System.Drawing.Size(160, 20);
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
            // pictureBox_DebugConsole
            // 
            this.pictureBox_DebugConsole.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_DebugConsole.Location = new System.Drawing.Point(130, 42);
            this.pictureBox_DebugConsole.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_DebugConsole.Name = "pictureBox_DebugConsole";
            this.pictureBox_DebugConsole.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_DebugConsole.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_DebugConsole.TabIndex = 364;
            this.pictureBox_DebugConsole.TabStop = false;
            // 
            // pictureBox_LogMaxFileSize
            // 
            this.pictureBox_LogMaxFileSize.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_LogMaxFileSize.Location = new System.Drawing.Point(318, 19);
            this.pictureBox_LogMaxFileSize.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_LogMaxFileSize.Name = "pictureBox_LogMaxFileSize";
            this.pictureBox_LogMaxFileSize.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_LogMaxFileSize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_LogMaxFileSize.TabIndex = 364;
            this.pictureBox_LogMaxFileSize.TabStop = false;
            // 
            // pictureBox_LogToFile
            // 
            this.pictureBox_LogToFile.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_LogToFile.Location = new System.Drawing.Point(130, 19);
            this.pictureBox_LogToFile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_LogToFile.Name = "pictureBox_LogToFile";
            this.pictureBox_LogToFile.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_LogToFile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_LogToFile.TabIndex = 364;
            this.pictureBox_LogToFile.TabStop = false;
            // 
            // checkBox_DebugConsole
            // 
            this.checkBox_DebugConsole.AutoSize = true;
            this.checkBox_DebugConsole.Location = new System.Drawing.Point(6, 42);
            this.checkBox_DebugConsole.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DebugConsole.Name = "checkBox_DebugConsole";
            this.checkBox_DebugConsole.Size = new System.Drawing.Size(96, 17);
            this.checkBox_DebugConsole.TabIndex = 313;
            this.checkBox_DebugConsole.Text = "DebugConsole";
            this.checkBox_DebugConsole.UseVisualStyleBackColor = true;
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
            this.groupBox_Localization.Location = new System.Drawing.Point(6, 211);
            this.groupBox_Localization.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Localization.Name = "groupBox_Localization";
            this.groupBox_Localization.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Localization.Size = new System.Drawing.Size(346, 70);
            this.groupBox_Localization.TabIndex = 385;
            this.groupBox_Localization.TabStop = false;
            this.groupBox_Localization.Text = "Localization:";
            // 
            // label_Language
            // 
            this.label_Language.AutoSize = true;
            this.label_Language.Location = new System.Drawing.Point(6, 16);
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
            this.pictureBox_displayCurrency.Location = new System.Drawing.Point(314, 16);
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
            this.pictureBox_Language.Location = new System.Drawing.Point(178, 16);
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
            this.comboBox_Language.Location = new System.Drawing.Point(6, 36);
            this.comboBox_Language.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.comboBox_Language.Name = "comboBox_Language";
            this.comboBox_Language.Size = new System.Drawing.Size(190, 21);
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
            this.currencyConverterCombobox.Location = new System.Drawing.Point(212, 36);
            this.currencyConverterCombobox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.currencyConverterCombobox.Name = "currencyConverterCombobox";
            this.currencyConverterCombobox.Size = new System.Drawing.Size(122, 21);
            this.currencyConverterCombobox.Sorted = true;
            this.currencyConverterCombobox.TabIndex = 381;
            this.currencyConverterCombobox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.currencyConverterCombobox_DrawItem);
            this.currencyConverterCombobox.SelectedIndexChanged += new System.EventHandler(this.CurrencyConverterCombobox_SelectedIndexChanged);
            // 
            // label_displayCurrency
            // 
            this.label_displayCurrency.AutoSize = true;
            this.label_displayCurrency.Location = new System.Drawing.Point(212, 16);
            this.label_displayCurrency.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_displayCurrency.Name = "label_displayCurrency";
            this.label_displayCurrency.Size = new System.Drawing.Size(89, 13);
            this.label_displayCurrency.TabIndex = 382;
            this.label_displayCurrency.Text = "Display Currency:";
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
            this.groupBoxMOPA.Location = new System.Drawing.Point(5, 156);
            this.groupBoxMOPA.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxMOPA.Name = "groupBoxMOPA";
            this.groupBoxMOPA.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxMOPA.Size = new System.Drawing.Size(641, 68);
            this.groupBoxMOPA.TabIndex = 396;
            this.groupBoxMOPA.TabStop = false;
            this.groupBoxMOPA.Text = "Method of obtaining profitability of algorithms:";
            // 
            // radioButtonMOPA5
            // 
            this.radioButtonMOPA5.AutoSize = true;
            this.radioButtonMOPA5.Location = new System.Drawing.Point(242, 45);
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
            this.radioButtonMOPA3.Location = new System.Drawing.Point(465, 19);
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
            this.groupBox_Miners.Controls.Add(this.label_RunEthlargement);
            this.groupBox_Miners.Controls.Add(this.checkBox_RunEthlargement);
            this.groupBox_Miners.Controls.Add(this.pictureBox_MinerRestartDelayMS);
            this.groupBox_Miners.Controls.Add(this.pictureBox_APIBindPortStart);
            this.groupBox_Miners.Controls.Add(this.pictureBox_SwitchMaxSeconds);
            this.groupBox_Miners.Controls.Add(this.pictureBox_SwitchProfitabilityThreshold);
            this.groupBox_Miners.Controls.Add(this.pictureBox_SwitchMinSeconds);
            this.groupBox_Miners.Controls.Add(this.pictureBox_MinIdleSeconds);
            this.groupBox_Miners.Controls.Add(this.label_MinIdleSeconds);
            this.groupBox_Miners.Controls.Add(this.label_SwitchMaxSeconds);
            this.groupBox_Miners.Controls.Add(this.label_MinerRestartDelayMS);
            this.groupBox_Miners.Controls.Add(this.textBox_SwitchMinSeconds);
            this.groupBox_Miners.Controls.Add(this.label_APIBindPortStart);
            this.groupBox_Miners.Controls.Add(this.textBox_SwitchProfitabilityThreshold);
            this.groupBox_Miners.Controls.Add(this.label_SwitchProfitabilityThreshold);
            this.groupBox_Miners.Controls.Add(this.textBox_APIBindPortStart);
            this.groupBox_Miners.Controls.Add(this.label_SwitchMinSeconds);
            this.groupBox_Miners.Controls.Add(this.textBox_MinIdleSeconds);
            this.groupBox_Miners.Controls.Add(this.textBox_SwitchMaxSeconds);
            this.groupBox_Miners.Controls.Add(this.textBox_MinerRestartDelayMS);
            this.groupBox_Miners.Location = new System.Drawing.Point(6, 6);
            this.groupBox_Miners.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Miners.Name = "groupBox_Miners";
            this.groupBox_Miners.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox_Miners.Size = new System.Drawing.Size(641, 144);
            this.groupBox_Miners.TabIndex = 389;
            this.groupBox_Miners.TabStop = false;
            this.groupBox_Miners.Text = "Miners:";
            // 
            // label_RunEthlargement
            // 
            this.label_RunEthlargement.AutoSize = true;
            this.label_RunEthlargement.Location = new System.Drawing.Point(28, 116);
            this.label_RunEthlargement.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_RunEthlargement.Name = "label_RunEthlargement";
            this.label_RunEthlargement.Size = new System.Drawing.Size(92, 13);
            this.label_RunEthlargement.TabIndex = 402;
            this.label_RunEthlargement.Text = "Run Ethlargement";
            // 
            // checkBox_RunEthlargement
            // 
            this.checkBox_RunEthlargement.AutoSize = true;
            this.checkBox_RunEthlargement.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_RunEthlargement.Location = new System.Drawing.Point(10, 116);
            this.checkBox_RunEthlargement.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_RunEthlargement.Name = "checkBox_RunEthlargement";
            this.checkBox_RunEthlargement.Size = new System.Drawing.Size(15, 14);
            this.checkBox_RunEthlargement.TabIndex = 401;
            this.checkBox_RunEthlargement.UseVisualStyleBackColor = true;
            this.checkBox_RunEthlargement.CheckedChanged += new System.EventHandler(this.checkBox_RunEthlargement_CheckedChanged);
            // 
            // pictureBox_MinerRestartDelayMS
            // 
            this.pictureBox_MinerRestartDelayMS.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_MinerRestartDelayMS.Location = new System.Drawing.Point(394, 15);
            this.pictureBox_MinerRestartDelayMS.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_MinerRestartDelayMS.Name = "pictureBox_MinerRestartDelayMS";
            this.pictureBox_MinerRestartDelayMS.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_MinerRestartDelayMS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_MinerRestartDelayMS.TabIndex = 385;
            this.pictureBox_MinerRestartDelayMS.TabStop = false;
            // 
            // pictureBox_APIBindPortStart
            // 
            this.pictureBox_APIBindPortStart.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_APIBindPortStart.Location = new System.Drawing.Point(617, 15);
            this.pictureBox_APIBindPortStart.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_APIBindPortStart.Name = "pictureBox_APIBindPortStart";
            this.pictureBox_APIBindPortStart.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_APIBindPortStart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_APIBindPortStart.TabIndex = 385;
            this.pictureBox_APIBindPortStart.TabStop = false;
            // 
            // pictureBox_SwitchMaxSeconds
            // 
            this.pictureBox_SwitchMaxSeconds.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_SwitchMaxSeconds.Location = new System.Drawing.Point(394, 59);
            this.pictureBox_SwitchMaxSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_SwitchMaxSeconds.Name = "pictureBox_SwitchMaxSeconds";
            this.pictureBox_SwitchMaxSeconds.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_SwitchMaxSeconds.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_SwitchMaxSeconds.TabIndex = 385;
            this.pictureBox_SwitchMaxSeconds.TabStop = false;
            // 
            // pictureBox_SwitchProfitabilityThreshold
            // 
            this.pictureBox_SwitchProfitabilityThreshold.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_SwitchProfitabilityThreshold.Location = new System.Drawing.Point(617, 61);
            this.pictureBox_SwitchProfitabilityThreshold.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_SwitchProfitabilityThreshold.Name = "pictureBox_SwitchProfitabilityThreshold";
            this.pictureBox_SwitchProfitabilityThreshold.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_SwitchProfitabilityThreshold.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_SwitchProfitabilityThreshold.TabIndex = 385;
            this.pictureBox_SwitchProfitabilityThreshold.TabStop = false;
            // 
            // pictureBox_SwitchMinSeconds
            // 
            this.pictureBox_SwitchMinSeconds.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_SwitchMinSeconds.Location = new System.Drawing.Point(168, 59);
            this.pictureBox_SwitchMinSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_SwitchMinSeconds.Name = "pictureBox_SwitchMinSeconds";
            this.pictureBox_SwitchMinSeconds.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_SwitchMinSeconds.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_SwitchMinSeconds.TabIndex = 385;
            this.pictureBox_SwitchMinSeconds.TabStop = false;
            // 
            // pictureBox_MinIdleSeconds
            // 
            this.pictureBox_MinIdleSeconds.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_MinIdleSeconds.Location = new System.Drawing.Point(168, 15);
            this.pictureBox_MinIdleSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_MinIdleSeconds.Name = "pictureBox_MinIdleSeconds";
            this.pictureBox_MinIdleSeconds.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_MinIdleSeconds.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_MinIdleSeconds.TabIndex = 385;
            this.pictureBox_MinIdleSeconds.TabStop = false;
            // 
            // label_MinIdleSeconds
            // 
            this.label_MinIdleSeconds.AutoSize = true;
            this.label_MinIdleSeconds.Location = new System.Drawing.Point(10, 15);
            this.label_MinIdleSeconds.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MinIdleSeconds.Name = "label_MinIdleSeconds";
            this.label_MinIdleSeconds.Size = new System.Drawing.Size(86, 13);
            this.label_MinIdleSeconds.TabIndex = 356;
            this.label_MinIdleSeconds.Text = "MinIdleSeconds:";
            // 
            // label_SwitchMaxSeconds
            // 
            this.label_SwitchMaxSeconds.AutoSize = true;
            this.label_SwitchMaxSeconds.Location = new System.Drawing.Point(242, 59);
            this.label_SwitchMaxSeconds.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_SwitchMaxSeconds.Name = "label_SwitchMaxSeconds";
            this.label_SwitchMaxSeconds.Size = new System.Drawing.Size(86, 13);
            this.label_SwitchMaxSeconds.TabIndex = 378;
            this.label_SwitchMaxSeconds.Text = "SwitchMaxSecs:";
            // 
            // label_MinerRestartDelayMS
            // 
            this.label_MinerRestartDelayMS.AutoSize = true;
            this.label_MinerRestartDelayMS.Location = new System.Drawing.Point(242, 15);
            this.label_MinerRestartDelayMS.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MinerRestartDelayMS.Name = "label_MinerRestartDelayMS";
            this.label_MinerRestartDelayMS.Size = new System.Drawing.Size(113, 13);
            this.label_MinerRestartDelayMS.TabIndex = 375;
            this.label_MinerRestartDelayMS.Text = "MinerRestartDelayMS:";
            // 
            // textBox_SwitchMinSeconds
            // 
            this.textBox_SwitchMinSeconds.Location = new System.Drawing.Point(10, 78);
            this.textBox_SwitchMinSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_SwitchMinSeconds.Name = "textBox_SwitchMinSeconds";
            this.textBox_SwitchMinSeconds.Size = new System.Drawing.Size(178, 20);
            this.textBox_SwitchMinSeconds.TabIndex = 342;
            // 
            // label_APIBindPortStart
            // 
            this.label_APIBindPortStart.AutoSize = true;
            this.label_APIBindPortStart.Location = new System.Drawing.Point(465, 15);
            this.label_APIBindPortStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_APIBindPortStart.Name = "label_APIBindPortStart";
            this.label_APIBindPortStart.Size = new System.Drawing.Size(118, 13);
            this.label_APIBindPortStart.TabIndex = 357;
            this.label_APIBindPortStart.Text = "API Bind port pool start:";
            // 
            // textBox_SwitchProfitabilityThreshold
            // 
            this.textBox_SwitchProfitabilityThreshold.Location = new System.Drawing.Point(465, 79);
            this.textBox_SwitchProfitabilityThreshold.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_SwitchProfitabilityThreshold.Name = "textBox_SwitchProfitabilityThreshold";
            this.textBox_SwitchProfitabilityThreshold.Size = new System.Drawing.Size(172, 20);
            this.textBox_SwitchProfitabilityThreshold.TabIndex = 333;
            this.textBox_SwitchProfitabilityThreshold.TextChanged += new System.EventHandler(this.textBox_SwitchProfitabilityThreshold_TextChanged);
            // 
            // label_SwitchProfitabilityThreshold
            // 
            this.label_SwitchProfitabilityThreshold.AutoSize = true;
            this.label_SwitchProfitabilityThreshold.Location = new System.Drawing.Point(463, 59);
            this.label_SwitchProfitabilityThreshold.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_SwitchProfitabilityThreshold.Name = "label_SwitchProfitabilityThreshold";
            this.label_SwitchProfitabilityThreshold.Size = new System.Drawing.Size(136, 13);
            this.label_SwitchProfitabilityThreshold.TabIndex = 361;
            this.label_SwitchProfitabilityThreshold.Text = "SwitchProfitabilityThreshold";
            // 
            // textBox_APIBindPortStart
            // 
            this.textBox_APIBindPortStart.Location = new System.Drawing.Point(465, 33);
            this.textBox_APIBindPortStart.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_APIBindPortStart.Name = "textBox_APIBindPortStart";
            this.textBox_APIBindPortStart.Size = new System.Drawing.Size(172, 20);
            this.textBox_APIBindPortStart.TabIndex = 334;
            // 
            // label_SwitchMinSeconds
            // 
            this.label_SwitchMinSeconds.AutoSize = true;
            this.label_SwitchMinSeconds.Location = new System.Drawing.Point(10, 59);
            this.label_SwitchMinSeconds.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_SwitchMinSeconds.Name = "label_SwitchMinSeconds";
            this.label_SwitchMinSeconds.Size = new System.Drawing.Size(83, 13);
            this.label_SwitchMinSeconds.TabIndex = 362;
            this.label_SwitchMinSeconds.Text = "SwitchMinSecs:";
            // 
            // textBox_MinIdleSeconds
            // 
            this.textBox_MinIdleSeconds.Location = new System.Drawing.Point(10, 33);
            this.textBox_MinIdleSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MinIdleSeconds.Name = "textBox_MinIdleSeconds";
            this.textBox_MinIdleSeconds.Size = new System.Drawing.Size(178, 20);
            this.textBox_MinIdleSeconds.TabIndex = 335;
            this.textBox_MinIdleSeconds.TextChanged += new System.EventHandler(this.textBox_MinIdleSeconds_TextChanged);
            // 
            // textBox_SwitchMaxSeconds
            // 
            this.textBox_SwitchMaxSeconds.Location = new System.Drawing.Point(242, 78);
            this.textBox_SwitchMaxSeconds.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_SwitchMaxSeconds.Name = "textBox_SwitchMaxSeconds";
            this.textBox_SwitchMaxSeconds.Size = new System.Drawing.Size(172, 20);
            this.textBox_SwitchMaxSeconds.TabIndex = 337;
            // 
            // textBox_MinerRestartDelayMS
            // 
            this.textBox_MinerRestartDelayMS.Location = new System.Drawing.Point(242, 33);
            this.textBox_MinerRestartDelayMS.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MinerRestartDelayMS.Name = "textBox_MinerRestartDelayMS";
            this.textBox_MinerRestartDelayMS.Size = new System.Drawing.Size(172, 20);
            this.textBox_MinerRestartDelayMS.TabIndex = 340;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkbox_Group_same_devices);
            this.groupBox1.Controls.Add(this.checkBox_ShowFanAsPercent);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionCPU);
            this.groupBox1.Controls.Add(this.checkBox_Additional_info_about_device);
            this.groupBox1.Controls.Add(this.pictureBox_NVIDIAP0State);
            this.groupBox1.Controls.Add(this.checkBox_NVIDIAP0State);
            this.groupBox1.Controls.Add(this.pictureBox_DisableDetectionAMD);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionNVIDIA);
            this.groupBox1.Controls.Add(this.pictureBox_DisableDetectionNVIDIA);
            this.groupBox1.Controls.Add(this.checkBox_DisableDetectionAMD);
            this.groupBox1.Location = new System.Drawing.Point(5, 230);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBox1.Size = new System.Drawing.Size(641, 117);
            this.groupBox1.TabIndex = 394;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Devices:";
            // 
            // checkbox_Group_same_devices
            // 
            this.checkbox_Group_same_devices.AutoSize = true;
            this.checkbox_Group_same_devices.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkbox_Group_same_devices.Location = new System.Drawing.Point(242, 65);
            this.checkbox_Group_same_devices.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkbox_Group_same_devices.Name = "checkbox_Group_same_devices";
            this.checkbox_Group_same_devices.Size = new System.Drawing.Size(123, 17);
            this.checkbox_Group_same_devices.TabIndex = 402;
            this.checkbox_Group_same_devices.Text = "Group same devices";
            this.checkbox_Group_same_devices.UseVisualStyleBackColor = true;
            // 
            // checkBox_ShowFanAsPercent
            // 
            this.checkBox_ShowFanAsPercent.AutoSize = true;
            this.checkBox_ShowFanAsPercent.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_ShowFanAsPercent.Location = new System.Drawing.Point(242, 42);
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
            this.pictureBox1.Location = new System.Drawing.Point(10, 64);
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
            this.checkBox_DisableDetectionCPU.Location = new System.Drawing.Point(32, 64);
            this.checkBox_DisableDetectionCPU.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionCPU.Name = "checkBox_DisableDetectionCPU";
            this.checkBox_DisableDetectionCPU.Size = new System.Drawing.Size(133, 17);
            this.checkBox_DisableDetectionCPU.TabIndex = 399;
            this.checkBox_DisableDetectionCPU.Text = "Disable CPU detection";
            this.checkBox_DisableDetectionCPU.UseVisualStyleBackColor = true;
            // 
            // checkBox_Additional_info_about_device
            // 
            this.checkBox_Additional_info_about_device.AutoSize = true;
            this.checkBox_Additional_info_about_device.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Additional_info_about_device.Location = new System.Drawing.Point(242, 19);
            this.checkBox_Additional_info_about_device.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_Additional_info_about_device.Name = "checkBox_Additional_info_about_device";
            this.checkBox_Additional_info_about_device.Size = new System.Drawing.Size(157, 17);
            this.checkBox_Additional_info_about_device.TabIndex = 398;
            this.checkBox_Additional_info_about_device.Text = "Additional info about device";
            this.checkBox_Additional_info_about_device.UseVisualStyleBackColor = true;
            // 
            // pictureBox_NVIDIAP0State
            // 
            this.pictureBox_NVIDIAP0State.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_NVIDIAP0State.Location = new System.Drawing.Point(10, 87);
            this.pictureBox_NVIDIAP0State.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pictureBox_NVIDIAP0State.Name = "pictureBox_NVIDIAP0State";
            this.pictureBox_NVIDIAP0State.Size = new System.Drawing.Size(18, 18);
            this.pictureBox_NVIDIAP0State.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_NVIDIAP0State.TabIndex = 396;
            this.pictureBox_NVIDIAP0State.TabStop = false;
            // 
            // checkBox_NVIDIAP0State
            // 
            this.checkBox_NVIDIAP0State.AutoSize = true;
            this.checkBox_NVIDIAP0State.Location = new System.Drawing.Point(32, 87);
            this.checkBox_NVIDIAP0State.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_NVIDIAP0State.Name = "checkBox_NVIDIAP0State";
            this.checkBox_NVIDIAP0State.Size = new System.Drawing.Size(100, 17);
            this.checkBox_NVIDIAP0State.TabIndex = 394;
            this.checkBox_NVIDIAP0State.Text = "NVIDIAP0State";
            this.checkBox_NVIDIAP0State.UseVisualStyleBackColor = true;
            // 
            // pictureBox_DisableDetectionAMD
            // 
            this.pictureBox_DisableDetectionAMD.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_DisableDetectionAMD.Location = new System.Drawing.Point(10, 41);
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
            // 
            // pictureBox_DisableDetectionNVIDIA
            // 
            this.pictureBox_DisableDetectionNVIDIA.Image = global::NiceHashMiner.Properties.Resources.info_black_18;
            this.pictureBox_DisableDetectionNVIDIA.Location = new System.Drawing.Point(10, 19);
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
            this.checkBox_DisableDetectionAMD.Location = new System.Drawing.Point(32, 41);
            this.checkBox_DisableDetectionAMD.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.checkBox_DisableDetectionAMD.Name = "checkBox_DisableDetectionAMD";
            this.checkBox_DisableDetectionAMD.Size = new System.Drawing.Size(161, 17);
            this.checkBox_DisableDetectionAMD.TabIndex = 391;
            this.checkBox_DisableDetectionAMD.Text = "Disable AMD GPU detection";
            this.checkBox_DisableDetectionAMD.UseVisualStyleBackColor = true;
            // 
            // tabPageDevicesAlgos
            // 
            this.tabPageDevicesAlgos.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageDevicesAlgos.Controls.Add(this.checkBox_Disable_extra_launch_parameter_checking);
            this.tabPageDevicesAlgos.Controls.Add(this.groupBoxAlgorithmSettings);
            this.tabPageDevicesAlgos.Controls.Add(this.buttonGPUtuning);
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
            // checkBox_Disable_extra_launch_parameter_checking
            // 
            this.checkBox_Disable_extra_launch_parameter_checking.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_Disable_extra_launch_parameter_checking.AutoSize = true;
            this.checkBox_Disable_extra_launch_parameter_checking.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBox_Disable_extra_launch_parameter_checking.Location = new System.Drawing.Point(360, 224);
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
            this.groupBoxAlgorithmSettings.Location = new System.Drawing.Point(6, 244);
            this.groupBoxAlgorithmSettings.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxAlgorithmSettings.Name = "groupBoxAlgorithmSettings";
            this.groupBoxAlgorithmSettings.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.groupBoxAlgorithmSettings.Size = new System.Drawing.Size(639, 253);
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
            this.algorithmsListView1.Size = new System.Drawing.Size(627, 169);
            this.algorithmsListView1.TabIndex = 2;
            // 
            // buttonGPUtuning
            // 
            this.buttonGPUtuning.Location = new System.Drawing.Point(8, 20);
            this.buttonGPUtuning.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonGPUtuning.Name = "buttonGPUtuning";
            this.buttonGPUtuning.Size = new System.Drawing.Size(141, 23);
            this.buttonGPUtuning.TabIndex = 394;
            this.buttonGPUtuning.Text = "GPU tuning";
            this.buttonGPUtuning.UseVisualStyleBackColor = true;
            this.buttonGPUtuning.Click += new System.EventHandler(this.buttonGPUtuning_Click_1);
            // 
            // algorithmSettingsControl1
            // 
            this.algorithmSettingsControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.algorithmSettingsControl1.Location = new System.Drawing.Point(360, 8);
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
            this.devicesListViewEnableControl1.Location = new System.Drawing.Point(8, 51);
            this.devicesListViewEnableControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.devicesListViewEnableControl1.Name = "devicesListViewEnableControl1";
            this.devicesListViewEnableControl1.SaveToGeneralConfig = false;
            this.devicesListViewEnableControl1.Size = new System.Drawing.Size(348, 165);
            this.devicesListViewEnableControl1.TabIndex = 397;
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
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_Settings_Paint);
            this.tabControlGeneral.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.groupBox_Main.ResumeLayout(false);
            this.groupBox_Main.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ElectricityCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_TimeUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_UseIFTTT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinProfit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_IdleWhenNoInternetAccess)).EndInit();
            this.groupBox_Misc.ResumeLayout(false);
            this.groupBox_Misc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RunScriptOnCUDA_GPU_Lost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ShowInternetConnectionWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinimizeMiningWindows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_RunAtStartup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AllowMultipleInstances)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableWindowsErrorReporting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_ShowDriverVersionWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_StartMiningWhenIdle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AutoScaleBTCValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_AutoStartMining)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinimizeToTray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_HideMiningWindows)).EndInit();
            this.groupBox_Logging.ResumeLayout(false);
            this.groupBox_Logging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DebugConsole)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LogMaxFileSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_LogToFile)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinerRestartDelayMS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_APIBindPortStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SwitchMaxSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SwitchProfitabilityThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SwitchMinSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_MinIdleSeconds)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_NVIDIAP0State)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableDetectionAMD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DisableDetectionNVIDIA)).EndInit();
            this.tabPageDevicesAlgos.ResumeLayout(false);
            this.tabPageDevicesAlgos.PerformLayout();
            this.groupBoxAlgorithmSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSaveClose;
        private System.Windows.Forms.Button buttonDefaults;
        private System.Windows.Forms.Button buttonCloseNoSave;

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CustomTabControl tabControlGeneral;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.GroupBox groupBox_Main;
        private System.Windows.Forms.CheckBox checkBox_Force_mining_if_nonprofitable;
        private System.Windows.Forms.PictureBox pictureBox_ElectricityCost;
        private System.Windows.Forms.TextBox textBox_ElectricityCost;
        private System.Windows.Forms.Label label_ElectricityCost;
        private System.Windows.Forms.PictureBox pictureBox_TimeUnit;
        private System.Windows.Forms.Label label_TimeUnit;
        private System.Windows.Forms.ComboBox comboBox_TimeUnit;
        private System.Windows.Forms.Label label_IFTTTAPIKey;
        private System.Windows.Forms.TextBox textBox_IFTTTKey;
        private System.Windows.Forms.PictureBox pictureBox_UseIFTTT;
        private System.Windows.Forms.CheckBox checkBox_UseIFTTT;
        private System.Windows.Forms.CheckBox checkBox_IdleWhenNoInternetAccess;
        private System.Windows.Forms.PictureBox pictureBox_MinProfit;
        private System.Windows.Forms.TextBox textBox_MinProfit;
        private System.Windows.Forms.PictureBox pictureBox_IdleWhenNoInternetAccess;
        private System.Windows.Forms.Label label_MinProfit;
        private System.Windows.Forms.GroupBox groupBox_Misc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_AutoStartMiningDelay;
        private System.Windows.Forms.Label label_AutoStartMiningDelay;
        private System.Windows.Forms.ComboBox comboBox_ColorProfile;
        private System.Windows.Forms.CheckBox checkBox_Send_actual_version_info;
        private System.Windows.Forms.CheckBox checkBox_Allow_remote_management;
        private System.Windows.Forms.PictureBox pictureBox_RunScriptOnCUDA_GPU_Lost;
        private System.Windows.Forms.CheckBox checkBox_RunScriptOnCUDA_GPU_Lost;
        private System.Windows.Forms.PictureBox pictureBox_ShowInternetConnectionWarning;
        private System.Windows.Forms.CheckBox checkBox_ShowInternetConnectionWarning;
        private System.Windows.Forms.CheckBox checkBox_MinimizeMiningWindows;
        private System.Windows.Forms.PictureBox pictureBox_MinimizeMiningWindows;
        private System.Windows.Forms.PictureBox pictureBox_RunAtStartup;
        private System.Windows.Forms.CheckBox checkBox_RunAtStartup;
        private System.Windows.Forms.CheckBox checkBox_AllowMultipleInstances;
        private System.Windows.Forms.CheckBox checkBox_AutoStartMining;
        private System.Windows.Forms.CheckBox checkBox_HideMiningWindows;
        private System.Windows.Forms.PictureBox pictureBox_AllowMultipleInstances;
        private System.Windows.Forms.CheckBox checkBox_MinimizeToTray;
        private System.Windows.Forms.PictureBox pictureBox_DisableWindowsErrorReporting;
        private System.Windows.Forms.PictureBox pictureBox_ShowDriverVersionWarning;
        private System.Windows.Forms.PictureBox pictureBox_StartMiningWhenIdle;
        private System.Windows.Forms.PictureBox pictureBox_AutoScaleBTCValues;
        private System.Windows.Forms.PictureBox pictureBox_AutoStartMining;
        private System.Windows.Forms.PictureBox pictureBox_MinimizeToTray;
        private System.Windows.Forms.PictureBox pictureBox_HideMiningWindows;
        private System.Windows.Forms.CheckBox checkBox_AutoScaleBTCValues;
        private System.Windows.Forms.CheckBox checkBox_DisableWindowsErrorReporting;
        private System.Windows.Forms.CheckBox checkBox_StartMiningWhenIdle;
        private System.Windows.Forms.CheckBox checkBox_ShowDriverVersionWarning;
        private System.Windows.Forms.GroupBox groupBox_Logging;
        private System.Windows.Forms.Label label_LogMaxFileSize;
        private System.Windows.Forms.TextBox textBox_LogMaxFileSize;
        private System.Windows.Forms.CheckBox checkBox_LogToFile;
        private System.Windows.Forms.PictureBox pictureBox_DebugConsole;
        private System.Windows.Forms.PictureBox pictureBox_LogMaxFileSize;
        private System.Windows.Forms.PictureBox pictureBox_LogToFile;
        private System.Windows.Forms.CheckBox checkBox_DebugConsole;
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
        private System.Windows.Forms.PictureBox pictureBox_MinerRestartDelayMS;
        private System.Windows.Forms.PictureBox pictureBox_APIBindPortStart;
        private System.Windows.Forms.PictureBox pictureBox_SwitchMaxSeconds;
        private System.Windows.Forms.PictureBox pictureBox_SwitchProfitabilityThreshold;
        private System.Windows.Forms.PictureBox pictureBox_SwitchMinSeconds;
        private System.Windows.Forms.PictureBox pictureBox_MinIdleSeconds;
        private System.Windows.Forms.Label label_MinIdleSeconds;
        private System.Windows.Forms.Label label_SwitchMaxSeconds;
        private System.Windows.Forms.Label label_MinerRestartDelayMS;
        private System.Windows.Forms.TextBox textBox_SwitchMinSeconds;
        private System.Windows.Forms.Label label_APIBindPortStart;
        private System.Windows.Forms.TextBox textBox_SwitchProfitabilityThreshold;
        private System.Windows.Forms.Label label_SwitchProfitabilityThreshold;
        private System.Windows.Forms.TextBox textBox_APIBindPortStart;
        private System.Windows.Forms.Label label_SwitchMinSeconds;
        private System.Windows.Forms.TextBox textBox_MinIdleSeconds;
        private System.Windows.Forms.TextBox textBox_SwitchMaxSeconds;
        private System.Windows.Forms.TextBox textBox_MinerRestartDelayMS;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionCPU;
        private System.Windows.Forms.CheckBox checkBox_Additional_info_about_device;
        private System.Windows.Forms.PictureBox pictureBox_NVIDIAP0State;
        private System.Windows.Forms.CheckBox checkBox_NVIDIAP0State;
        private System.Windows.Forms.PictureBox pictureBox_DisableDetectionAMD;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionNVIDIA;
        private System.Windows.Forms.PictureBox pictureBox_DisableDetectionNVIDIA;
        private System.Windows.Forms.CheckBox checkBox_DisableDetectionAMD;
        private System.Windows.Forms.TabPage tabPageDevicesAlgos;
        private System.Windows.Forms.CheckBox checkBox_Disable_extra_launch_parameter_checking;
        private Components.AlgorithmSettingsControl algorithmSettingsControl1;
        private System.Windows.Forms.GroupBox groupBoxAlgorithmSettings;
        private Components.AlgorithmsListView algorithmsListView1;
        private System.Windows.Forms.Button buttonGPUtuning;
        private Components.DevicesListViewEnableControl devicesListViewEnableControl1;
        private System.Windows.Forms.CheckBox checkBox_RunEthlargement;
        private System.Windows.Forms.Label label_RunEthlargement;
        private System.Windows.Forms.CheckBox checkBox_ShowFanAsPercent;
        private System.Windows.Forms.CheckBox Checkbox_Save_windows_size_and_position;
        private System.Windows.Forms.CheckBox checkbox_Group_same_devices;
    }
}
