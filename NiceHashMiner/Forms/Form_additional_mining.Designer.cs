
namespace NiceHashMiner.Forms
{
    partial class Form_additional_mining
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TabControlZILadditionalMining = new System.Windows.Forms.CustomTabControl();
            this.tabPageGMiner = new System.Windows.Forms.TabPage();
            this.groupBox_NVIDIA = new System.Windows.Forms.GroupBox();
            this.checkBox_GMINER_NVIDIA_Autolykos = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_ZHash = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_ZelHash = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_Octopus = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_KHeavyHash = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_KAWPOW = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_GrinCuckatoo32 = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_CuckooCycle = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_BeamV3 = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash = new System.Windows.Forms.CheckBox();
            this.groupBox_AMD = new System.Windows.Forms.GroupBox();
            this.checkBox_GMINER_AMD_ZHash = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_AMD_ZelHash = new System.Windows.Forms.CheckBox();
            this.checkBox_GMINER_AMD_KAWPOW = new System.Windows.Forms.CheckBox();
            this.tabPageNanominer = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_NANOMINER_AMD_Autolykos = new System.Windows.Forms.CheckBox();
            this.tabPageRigel = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_Rigel_NVIDIA_KHeavyHash = new System.Windows.Forms.CheckBox();
            this.tabPageSRBMiner = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_SRBMINER_AMD_KHeavyHash = new System.Windows.Forms.CheckBox();
            this.checkBox_SRBMINER_AMD_Autolykos = new System.Windows.Forms.CheckBox();
            this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash = new System.Windows.Forms.CheckBox();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.checkBox_ZIL_Mining_Enable = new System.Windows.Forms.CheckBox();
            this.TabControlZILadditionalMining.SuspendLayout();
            this.tabPageGMiner.SuspendLayout();
            this.groupBox_NVIDIA.SuspendLayout();
            this.groupBox_AMD.SuspendLayout();
            this.tabPageNanominer.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPageRigel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPageSRBMiner.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControlZILadditionalMining
            // 
            this.TabControlZILadditionalMining.Controls.Add(this.tabPageGMiner);
            this.TabControlZILadditionalMining.Controls.Add(this.tabPageNanominer);
            this.TabControlZILadditionalMining.Controls.Add(this.tabPageRigel);
            this.TabControlZILadditionalMining.Controls.Add(this.tabPageSRBMiner);
            // 
            // 
            // 
            this.TabControlZILadditionalMining.DisplayStyleProvider.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.TabControlZILadditionalMining.DisplayStyleProvider.BorderColorHot = System.Drawing.SystemColors.ControlDark;
            this.TabControlZILadditionalMining.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.TabControlZILadditionalMining.DisplayStyleProvider.CloserColor = System.Drawing.Color.DarkGray;
            this.TabControlZILadditionalMining.DisplayStyleProvider.FocusTrack = true;
            this.TabControlZILadditionalMining.DisplayStyleProvider.HotTrack = true;
            this.TabControlZILadditionalMining.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TabControlZILadditionalMining.DisplayStyleProvider.Opacity = 1F;
            this.TabControlZILadditionalMining.DisplayStyleProvider.Overlap = 0;
            this.TabControlZILadditionalMining.DisplayStyleProvider.Padding = new System.Drawing.Point(6, 3);
            this.TabControlZILadditionalMining.DisplayStyleProvider.Radius = 2;
            this.TabControlZILadditionalMining.DisplayStyleProvider.ShowTabCloser = false;
            this.TabControlZILadditionalMining.DisplayStyleProvider.TextColor = System.Drawing.SystemColors.ControlText;
            this.TabControlZILadditionalMining.DisplayStyleProvider.TextColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.TabControlZILadditionalMining.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText;
            this.TabControlZILadditionalMining.HotTrack = true;
            this.TabControlZILadditionalMining.Location = new System.Drawing.Point(0, 0);
            this.TabControlZILadditionalMining.Name = "TabControlZILadditionalMining";
            this.TabControlZILadditionalMining.SelectedIndex = 0;
            this.TabControlZILadditionalMining.Size = new System.Drawing.Size(372, 189);
            this.TabControlZILadditionalMining.TabIndex = 0;
            // 
            // tabPageGMiner
            // 
            this.tabPageGMiner.Controls.Add(this.groupBox_NVIDIA);
            this.tabPageGMiner.Controls.Add(this.groupBox_AMD);
            this.tabPageGMiner.Location = new System.Drawing.Point(4, 23);
            this.tabPageGMiner.Name = "tabPageGMiner";
            this.tabPageGMiner.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGMiner.Size = new System.Drawing.Size(364, 162);
            this.tabPageGMiner.TabIndex = 0;
            this.tabPageGMiner.Text = "GMiner";
            this.tabPageGMiner.UseVisualStyleBackColor = true;
            // 
            // groupBox_NVIDIA
            // 
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_Autolykos);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_ZHash);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_ZelHash);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_Octopus);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_KHeavyHash);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_KAWPOW);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_GrinCuckatoo32);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_CuckooCycle);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_BeamV3);
            this.groupBox_NVIDIA.Controls.Add(this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash);
            this.groupBox_NVIDIA.Location = new System.Drawing.Point(6, 63);
            this.groupBox_NVIDIA.Name = "groupBox_NVIDIA";
            this.groupBox_NVIDIA.Size = new System.Drawing.Size(348, 92);
            this.groupBox_NVIDIA.TabIndex = 1;
            this.groupBox_NVIDIA.TabStop = false;
            this.groupBox_NVIDIA.Text = "NVIDIA";
            // 
            // checkBox_GMINER_NVIDIA_Autolykos
            // 
            this.checkBox_GMINER_NVIDIA_Autolykos.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_Autolykos.Location = new System.Drawing.Point(6, 19);
            this.checkBox_GMINER_NVIDIA_Autolykos.Name = "checkBox_GMINER_NVIDIA_Autolykos";
            this.checkBox_GMINER_NVIDIA_Autolykos.Size = new System.Drawing.Size(72, 17);
            this.checkBox_GMINER_NVIDIA_Autolykos.TabIndex = 9;
            this.checkBox_GMINER_NVIDIA_Autolykos.Text = "Autolykos";
            this.checkBox_GMINER_NVIDIA_Autolykos.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_ZHash
            // 
            this.checkBox_GMINER_NVIDIA_ZHash.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_ZHash.Location = new System.Drawing.Point(245, 65);
            this.checkBox_GMINER_NVIDIA_ZHash.Name = "checkBox_GMINER_NVIDIA_ZHash";
            this.checkBox_GMINER_NVIDIA_ZHash.Size = new System.Drawing.Size(58, 17);
            this.checkBox_GMINER_NVIDIA_ZHash.TabIndex = 8;
            this.checkBox_GMINER_NVIDIA_ZHash.Text = "ZHash";
            this.checkBox_GMINER_NVIDIA_ZHash.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_ZelHash
            // 
            this.checkBox_GMINER_NVIDIA_ZelHash.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_ZelHash.Location = new System.Drawing.Point(173, 65);
            this.checkBox_GMINER_NVIDIA_ZelHash.Name = "checkBox_GMINER_NVIDIA_ZelHash";
            this.checkBox_GMINER_NVIDIA_ZelHash.Size = new System.Drawing.Size(66, 17);
            this.checkBox_GMINER_NVIDIA_ZelHash.TabIndex = 7;
            this.checkBox_GMINER_NVIDIA_ZelHash.Text = "ZelHash";
            this.checkBox_GMINER_NVIDIA_ZelHash.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_Octopus
            // 
            this.checkBox_GMINER_NVIDIA_Octopus.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_Octopus.Location = new System.Drawing.Point(101, 65);
            this.checkBox_GMINER_NVIDIA_Octopus.Name = "checkBox_GMINER_NVIDIA_Octopus";
            this.checkBox_GMINER_NVIDIA_Octopus.Size = new System.Drawing.Size(66, 17);
            this.checkBox_GMINER_NVIDIA_Octopus.TabIndex = 6;
            this.checkBox_GMINER_NVIDIA_Octopus.Text = "Octopus";
            this.checkBox_GMINER_NVIDIA_Octopus.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_KHeavyHash
            // 
            this.checkBox_GMINER_NVIDIA_KHeavyHash.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_KHeavyHash.Location = new System.Drawing.Point(6, 65);
            this.checkBox_GMINER_NVIDIA_KHeavyHash.Name = "checkBox_GMINER_NVIDIA_KHeavyHash";
            this.checkBox_GMINER_NVIDIA_KHeavyHash.Size = new System.Drawing.Size(89, 17);
            this.checkBox_GMINER_NVIDIA_KHeavyHash.TabIndex = 5;
            this.checkBox_GMINER_NVIDIA_KHeavyHash.Text = "KHeavyHash";
            this.checkBox_GMINER_NVIDIA_KHeavyHash.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_KAWPOW
            // 
            this.checkBox_GMINER_NVIDIA_KAWPOW.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_KAWPOW.Location = new System.Drawing.Point(245, 42);
            this.checkBox_GMINER_NVIDIA_KAWPOW.Name = "checkBox_GMINER_NVIDIA_KAWPOW";
            this.checkBox_GMINER_NVIDIA_KAWPOW.Size = new System.Drawing.Size(77, 17);
            this.checkBox_GMINER_NVIDIA_KAWPOW.TabIndex = 4;
            this.checkBox_GMINER_NVIDIA_KAWPOW.Text = "KAWPOW";
            this.checkBox_GMINER_NVIDIA_KAWPOW.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_GrinCuckatoo32
            // 
            this.checkBox_GMINER_NVIDIA_GrinCuckatoo32.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_GrinCuckatoo32.Location = new System.Drawing.Point(101, 42);
            this.checkBox_GMINER_NVIDIA_GrinCuckatoo32.Name = "checkBox_GMINER_NVIDIA_GrinCuckatoo32";
            this.checkBox_GMINER_NVIDIA_GrinCuckatoo32.Size = new System.Drawing.Size(103, 17);
            this.checkBox_GMINER_NVIDIA_GrinCuckatoo32.TabIndex = 3;
            this.checkBox_GMINER_NVIDIA_GrinCuckatoo32.Text = "GrinCuckatoo32";
            this.checkBox_GMINER_NVIDIA_GrinCuckatoo32.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_CuckooCycle
            // 
            this.checkBox_GMINER_NVIDIA_CuckooCycle.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_CuckooCycle.Location = new System.Drawing.Point(6, 42);
            this.checkBox_GMINER_NVIDIA_CuckooCycle.Name = "checkBox_GMINER_NVIDIA_CuckooCycle";
            this.checkBox_GMINER_NVIDIA_CuckooCycle.Size = new System.Drawing.Size(89, 17);
            this.checkBox_GMINER_NVIDIA_CuckooCycle.TabIndex = 2;
            this.checkBox_GMINER_NVIDIA_CuckooCycle.Text = "CuckooCycle";
            this.checkBox_GMINER_NVIDIA_CuckooCycle.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_BeamV3
            // 
            this.checkBox_GMINER_NVIDIA_BeamV3.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_BeamV3.Location = new System.Drawing.Point(245, 19);
            this.checkBox_GMINER_NVIDIA_BeamV3.Name = "checkBox_GMINER_NVIDIA_BeamV3";
            this.checkBox_GMINER_NVIDIA_BeamV3.Size = new System.Drawing.Size(66, 17);
            this.checkBox_GMINER_NVIDIA_BeamV3.TabIndex = 1;
            this.checkBox_GMINER_NVIDIA_BeamV3.Text = "BeamV3";
            this.checkBox_GMINER_NVIDIA_BeamV3.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_NVIDIA_AutolykosKHeavyHash
            // 
            this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.AutoSize = true;
            this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.Location = new System.Drawing.Point(101, 19);
            this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.Name = "checkBox_GMINER_NVIDIA_AutolykosKHeavyHash";
            this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.Size = new System.Drawing.Size(135, 17);
            this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.TabIndex = 0;
            this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.Text = "AutolykosKHeavyHash";
            this.checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.UseVisualStyleBackColor = true;
            // 
            // groupBox_AMD
            // 
            this.groupBox_AMD.Controls.Add(this.checkBox_GMINER_AMD_ZHash);
            this.groupBox_AMD.Controls.Add(this.checkBox_GMINER_AMD_ZelHash);
            this.groupBox_AMD.Controls.Add(this.checkBox_GMINER_AMD_KAWPOW);
            this.groupBox_AMD.Location = new System.Drawing.Point(6, 6);
            this.groupBox_AMD.Name = "groupBox_AMD";
            this.groupBox_AMD.Size = new System.Drawing.Size(348, 51);
            this.groupBox_AMD.TabIndex = 0;
            this.groupBox_AMD.TabStop = false;
            this.groupBox_AMD.Text = "AMD";
            // 
            // checkBox_GMINER_AMD_ZHash
            // 
            this.checkBox_GMINER_AMD_ZHash.AutoSize = true;
            this.checkBox_GMINER_AMD_ZHash.Location = new System.Drawing.Point(161, 19);
            this.checkBox_GMINER_AMD_ZHash.Name = "checkBox_GMINER_AMD_ZHash";
            this.checkBox_GMINER_AMD_ZHash.Size = new System.Drawing.Size(58, 17);
            this.checkBox_GMINER_AMD_ZHash.TabIndex = 2;
            this.checkBox_GMINER_AMD_ZHash.Text = "ZHash";
            this.checkBox_GMINER_AMD_ZHash.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_AMD_ZelHash
            // 
            this.checkBox_GMINER_AMD_ZelHash.AutoSize = true;
            this.checkBox_GMINER_AMD_ZelHash.Location = new System.Drawing.Point(89, 19);
            this.checkBox_GMINER_AMD_ZelHash.Name = "checkBox_GMINER_AMD_ZelHash";
            this.checkBox_GMINER_AMD_ZelHash.Size = new System.Drawing.Size(66, 17);
            this.checkBox_GMINER_AMD_ZelHash.TabIndex = 1;
            this.checkBox_GMINER_AMD_ZelHash.Text = "ZelHash";
            this.checkBox_GMINER_AMD_ZelHash.UseVisualStyleBackColor = true;
            // 
            // checkBox_GMINER_AMD_KAWPOW
            // 
            this.checkBox_GMINER_AMD_KAWPOW.AutoSize = true;
            this.checkBox_GMINER_AMD_KAWPOW.Location = new System.Drawing.Point(6, 19);
            this.checkBox_GMINER_AMD_KAWPOW.Name = "checkBox_GMINER_AMD_KAWPOW";
            this.checkBox_GMINER_AMD_KAWPOW.Size = new System.Drawing.Size(77, 17);
            this.checkBox_GMINER_AMD_KAWPOW.TabIndex = 0;
            this.checkBox_GMINER_AMD_KAWPOW.Text = "KAWPOW";
            this.checkBox_GMINER_AMD_KAWPOW.UseVisualStyleBackColor = true;
            // 
            // tabPageNanominer
            // 
            this.tabPageNanominer.Controls.Add(this.groupBox2);
            this.tabPageNanominer.Location = new System.Drawing.Point(4, 23);
            this.tabPageNanominer.Name = "tabPageNanominer";
            this.tabPageNanominer.Size = new System.Drawing.Size(364, 162);
            this.tabPageNanominer.TabIndex = 2;
            this.tabPageNanominer.Text = "Nanominer";
            this.tabPageNanominer.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_NANOMINER_AMD_Autolykos);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(348, 51);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "AMD";
            // 
            // checkBox_NANOMINER_AMD_Autolykos
            // 
            this.checkBox_NANOMINER_AMD_Autolykos.AutoSize = true;
            this.checkBox_NANOMINER_AMD_Autolykos.Location = new System.Drawing.Point(6, 19);
            this.checkBox_NANOMINER_AMD_Autolykos.Name = "checkBox_NANOMINER_AMD_Autolykos";
            this.checkBox_NANOMINER_AMD_Autolykos.Size = new System.Drawing.Size(72, 17);
            this.checkBox_NANOMINER_AMD_Autolykos.TabIndex = 1;
            this.checkBox_NANOMINER_AMD_Autolykos.Text = "Autolykos";
            this.checkBox_NANOMINER_AMD_Autolykos.UseVisualStyleBackColor = true;
            // 
            // tabPageRigel
            // 
            this.tabPageRigel.Controls.Add(this.groupBox3);
            this.tabPageRigel.Location = new System.Drawing.Point(4, 23);
            this.tabPageRigel.Name = "tabPageRigel";
            this.tabPageRigel.Size = new System.Drawing.Size(364, 162);
            this.tabPageRigel.TabIndex = 3;
            this.tabPageRigel.Text = "Rigel";
            this.tabPageRigel.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox_Rigel_NVIDIA_KHeavyHash);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(348, 92);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "NVIDIA";
            // 
            // checkBox_Rigel_NVIDIA_KHeavyHash
            // 
            this.checkBox_Rigel_NVIDIA_KHeavyHash.AutoSize = true;
            this.checkBox_Rigel_NVIDIA_KHeavyHash.Location = new System.Drawing.Point(6, 19);
            this.checkBox_Rigel_NVIDIA_KHeavyHash.Name = "checkBox_Rigel_NVIDIA_KHeavyHash";
            this.checkBox_Rigel_NVIDIA_KHeavyHash.Size = new System.Drawing.Size(89, 17);
            this.checkBox_Rigel_NVIDIA_KHeavyHash.TabIndex = 5;
            this.checkBox_Rigel_NVIDIA_KHeavyHash.Text = "KHeavyHash";
            this.checkBox_Rigel_NVIDIA_KHeavyHash.UseVisualStyleBackColor = true;
            // 
            // tabPageSRBMiner
            // 
            this.tabPageSRBMiner.Controls.Add(this.groupBox1);
            this.tabPageSRBMiner.Location = new System.Drawing.Point(4, 23);
            this.tabPageSRBMiner.Name = "tabPageSRBMiner";
            this.tabPageSRBMiner.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSRBMiner.Size = new System.Drawing.Size(364, 162);
            this.tabPageSRBMiner.TabIndex = 1;
            this.tabPageSRBMiner.Text = "SRBMiner";
            this.tabPageSRBMiner.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_SRBMINER_AMD_KHeavyHash);
            this.groupBox1.Controls.Add(this.checkBox_SRBMINER_AMD_Autolykos);
            this.groupBox1.Controls.Add(this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 51);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "AMD";
            // 
            // checkBox_SRBMINER_AMD_KHeavyHash
            // 
            this.checkBox_SRBMINER_AMD_KHeavyHash.AutoSize = true;
            this.checkBox_SRBMINER_AMD_KHeavyHash.Location = new System.Drawing.Point(225, 19);
            this.checkBox_SRBMINER_AMD_KHeavyHash.Name = "checkBox_SRBMINER_AMD_KHeavyHash";
            this.checkBox_SRBMINER_AMD_KHeavyHash.Size = new System.Drawing.Size(89, 17);
            this.checkBox_SRBMINER_AMD_KHeavyHash.TabIndex = 2;
            this.checkBox_SRBMINER_AMD_KHeavyHash.Text = "KHeavyHash";
            this.checkBox_SRBMINER_AMD_KHeavyHash.UseVisualStyleBackColor = true;
            // 
            // checkBox_SRBMINER_AMD_Autolykos
            // 
            this.checkBox_SRBMINER_AMD_Autolykos.AutoSize = true;
            this.checkBox_SRBMINER_AMD_Autolykos.Location = new System.Drawing.Point(6, 19);
            this.checkBox_SRBMINER_AMD_Autolykos.Name = "checkBox_SRBMINER_AMD_Autolykos";
            this.checkBox_SRBMINER_AMD_Autolykos.Size = new System.Drawing.Size(72, 17);
            this.checkBox_SRBMINER_AMD_Autolykos.TabIndex = 1;
            this.checkBox_SRBMINER_AMD_Autolykos.Text = "Autolykos";
            this.checkBox_SRBMINER_AMD_Autolykos.UseVisualStyleBackColor = true;
            // 
            // checkBox_SRBMINER_AMD_AutolykosKHeavyHash
            // 
            this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash.AutoSize = true;
            this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash.Location = new System.Drawing.Point(84, 19);
            this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash.Name = "checkBox_SRBMINER_AMD_AutolykosKHeavyHash";
            this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash.Size = new System.Drawing.Size(135, 17);
            this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash.TabIndex = 0;
            this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash.Text = "AutolykosKHeavyHash";
            this.checkBox_SRBMINER_AMD_AutolykosKHeavyHash.UseVisualStyleBackColor = true;
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Save.Location = new System.Drawing.Point(208, 203);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 1;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Cancel.Location = new System.Drawing.Point(289, 203);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 2;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // checkBox_ZIL_Mining_Enable
            // 
            this.checkBox_ZIL_Mining_Enable.AutoSize = true;
            this.checkBox_ZIL_Mining_Enable.Location = new System.Drawing.Point(16, 207);
            this.checkBox_ZIL_Mining_Enable.Name = "checkBox_ZIL_Mining_Enable";
            this.checkBox_ZIL_Mining_Enable.Size = new System.Drawing.Size(162, 17);
            this.checkBox_ZIL_Mining_Enable.TabIndex = 3;
            this.checkBox_ZIL_Mining_Enable.Text = "Enable ZIL mining on all algo";
            this.checkBox_ZIL_Mining_Enable.UseVisualStyleBackColor = true;
            this.checkBox_ZIL_Mining_Enable.CheckedChanged += new System.EventHandler(this.checkBox_ZIL_Mining_Enable_CheckedChanged);
            // 
            // Form_additional_mining
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 238);
            this.Controls.Add(this.checkBox_ZIL_Mining_Enable);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.TabControlZILadditionalMining);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_additional_mining";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form_additional_mining";
            this.TabControlZILadditionalMining.ResumeLayout(false);
            this.tabPageGMiner.ResumeLayout(false);
            this.groupBox_NVIDIA.ResumeLayout(false);
            this.groupBox_NVIDIA.PerformLayout();
            this.groupBox_AMD.ResumeLayout(false);
            this.groupBox_AMD.PerformLayout();
            this.tabPageNanominer.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPageRigel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPageSRBMiner.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CustomTabControl TabControlZILadditionalMining;
        private System.Windows.Forms.TabPage tabPageGMiner;
        private System.Windows.Forms.TabPage tabPageSRBMiner;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.GroupBox groupBox_AMD;
        private System.Windows.Forms.CheckBox checkBox_GMINER_AMD_KAWPOW;
        private System.Windows.Forms.CheckBox checkBox_GMINER_AMD_ZelHash;
        private System.Windows.Forms.CheckBox checkBox_GMINER_AMD_ZHash;
        private System.Windows.Forms.GroupBox groupBox_NVIDIA;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_BeamV3;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_AutolykosKHeavyHash;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_CuckooCycle;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_GrinCuckatoo32;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_KAWPOW;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_KHeavyHash;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_Octopus;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_ZelHash;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_ZHash;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_SRBMINER_AMD_KHeavyHash;
        private System.Windows.Forms.CheckBox checkBox_SRBMINER_AMD_Autolykos;
        private System.Windows.Forms.CheckBox checkBox_SRBMINER_AMD_AutolykosKHeavyHash;
        private System.Windows.Forms.CheckBox checkBox_GMINER_NVIDIA_Autolykos;
        private System.Windows.Forms.TabPage tabPageNanominer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_NANOMINER_AMD_Autolykos;
        private System.Windows.Forms.TabPage tabPageRigel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox_Rigel_NVIDIA_KHeavyHash;
        private System.Windows.Forms.CheckBox checkBox_ZIL_Mining_Enable;
    }
}