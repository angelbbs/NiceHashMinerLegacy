/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
namespace NiceHashMiner.Forms.Components {
    partial class GroupProfitControl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.groupBoxMinerGroup = new System.Windows.Forms.GroupBox();
            this.button_restart = new System.Windows.Forms.Button();
            this.labelSpeedIndicator = new System.Windows.Forms.Label();
            this.labelCurentcyPerDayVaue = new System.Windows.Forms.Label();
            this.labelBTCRateValue = new System.Windows.Forms.Label();
            this.labelBTCRateIndicator = new System.Windows.Forms.Label();
            this.labelSpeedValue = new System.Windows.Forms.Label();
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxMinerGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMinerGroup
            // 
            this.groupBoxMinerGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMinerGroup.Controls.Add(this.button_restart);
            this.groupBoxMinerGroup.Controls.Add(this.labelSpeedIndicator);
            this.groupBoxMinerGroup.Controls.Add(this.labelCurentcyPerDayVaue);
            this.groupBoxMinerGroup.Controls.Add(this.labelBTCRateValue);
            this.groupBoxMinerGroup.Controls.Add(this.labelBTCRateIndicator);
            this.groupBoxMinerGroup.Controls.Add(this.labelSpeedValue);
            this.groupBoxMinerGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxMinerGroup.Location = new System.Drawing.Point(0, 0);
            this.groupBoxMinerGroup.Name = "groupBoxMinerGroup";
            this.groupBoxMinerGroup.Padding = new System.Windows.Forms.Padding(3, 3, 8, 3);
            this.groupBoxMinerGroup.Size = new System.Drawing.Size(540, 40);
            this.groupBoxMinerGroup.TabIndex = 108;
            this.groupBoxMinerGroup.TabStop = false;
            this.groupBoxMinerGroup.Text = "Mining Devices { N/A } ";
            // 
            // button_restart
            // 
            this.button_restart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_restart.FlatAppearance.BorderSize = 0;
            this.button_restart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_restart.Image = global::NiceHashMiner.Properties.Resources.Refresh_normal;
            this.button_restart.Location = new System.Drawing.Point(522, 2);
            this.button_restart.Margin = new System.Windows.Forms.Padding(1);
            this.button_restart.Name = "button_restart";
            this.button_restart.Size = new System.Drawing.Size(20, 20);
            this.button_restart.TabIndex = 114;
            this.button_restart.Tag = "";
            this.toolTip2.SetToolTip(this.button_restart, "Restart miner");
            this.button_restart.UseVisualStyleBackColor = false;
            this.button_restart.Click += new System.EventHandler(this.buttonBTC_restart);
            this.button_restart.MouseLeave += new System.EventHandler(this.button_restart_MouseLeave);
            this.button_restart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.button_restart_MouseMove);
            // 
            // labelSpeedIndicator
            // 
            this.labelSpeedIndicator.AutoSize = true;
            this.labelSpeedIndicator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelSpeedIndicator.Location = new System.Drawing.Point(6, 16);
            this.labelSpeedIndicator.Name = "labelSpeedIndicator";
            this.labelSpeedIndicator.Size = new System.Drawing.Size(47, 13);
            this.labelSpeedIndicator.TabIndex = 108;
            this.labelSpeedIndicator.Text = "Speed:";
            // 
            // labelCurentcyPerDayVaue
            // 
            this.labelCurentcyPerDayVaue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCurentcyPerDayVaue.AutoSize = true;
            this.labelCurentcyPerDayVaue.Location = new System.Drawing.Point(445, 16);
            this.labelCurentcyPerDayVaue.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.labelCurentcyPerDayVaue.Name = "labelCurentcyPerDayVaue";
            this.labelCurentcyPerDayVaue.Size = new System.Drawing.Size(61, 13);
            this.labelCurentcyPerDayVaue.TabIndex = 104;
            this.labelCurentcyPerDayVaue.Text = "0.00 $/Day";
            this.labelCurentcyPerDayVaue.Click += new System.EventHandler(this.labelCurentcyPerDayVaue_Click);
            // 
            // labelBTCRateValue
            // 
            this.labelBTCRateValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBTCRateValue.AutoSize = true;
            this.labelBTCRateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelBTCRateValue.Location = new System.Drawing.Point(336, 16);
            this.labelBTCRateValue.Name = "labelBTCRateValue";
            this.labelBTCRateValue.Size = new System.Drawing.Size(112, 13);
            this.labelBTCRateValue.TabIndex = 105;
            this.labelBTCRateValue.Text = "0.00000000 BTC/Day";
            this.labelBTCRateValue.Click += new System.EventHandler(this.labelBTCRateValue_Click);
            // 
            // labelBTCRateIndicator
            // 
            this.labelBTCRateIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBTCRateIndicator.AutoSize = true;
            this.labelBTCRateIndicator.Location = new System.Drawing.Point(293, 16);
            this.labelBTCRateIndicator.Name = "labelBTCRateIndicator";
            this.labelBTCRateIndicator.Size = new System.Drawing.Size(33, 13);
            this.labelBTCRateIndicator.TabIndex = 106;
            this.labelBTCRateIndicator.Text = "Rate:";
            this.labelBTCRateIndicator.Click += new System.EventHandler(this.labelBTCRateIndicator_Click);
            // 
            // labelSpeedValue
            // 
            this.labelSpeedValue.AutoSize = true;
            this.labelSpeedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelSpeedValue.Location = new System.Drawing.Point(73, 16);
            this.labelSpeedValue.Name = "labelSpeedValue";
            this.labelSpeedValue.Size = new System.Drawing.Size(30, 13);
            this.labelSpeedValue.TabIndex = 107;
            this.labelSpeedValue.Text = "N/A";
            // 
            // GroupProfitControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Controls.Add(this.groupBoxMinerGroup);
            this.Name = "GroupProfitControl";
            this.Size = new System.Drawing.Size(548, 43);
            this.groupBoxMinerGroup.ResumeLayout(false);
            this.groupBoxMinerGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMinerGroup;
        private System.Windows.Forms.Label labelSpeedIndicator;
        private System.Windows.Forms.Label labelCurentcyPerDayVaue;
        private System.Windows.Forms.Label labelBTCRateValue;
        private System.Windows.Forms.Label labelBTCRateIndicator;
        private System.Windows.Forms.Label labelSpeedValue;
        private System.Windows.Forms.Button button_restart;
        private System.Windows.Forms.ToolTip toolTip2;
    }
}
