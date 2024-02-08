namespace NiceHashMiner.Forms.Components {
    partial class AlgorithmSettingsControl {
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
            this.groupBoxSelectedAlgorithmSettings = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.fieldBoxBenchmarkSpeed = new NiceHashMiner.Forms.Components.Field();
            this.secondaryFieldBoxBenchmarkSpeed = new NiceHashMiner.Forms.Components.Field();
            this.field_PowerUsage = new NiceHashMiner.Forms.Components.Field();
            this.groupBoxExtraLaunchParameters = new System.Windows.Forms.GroupBox();
            this.richTextBoxExtraLaunchParameters = new System.Windows.Forms.TextBox();
            this.groupBoxSelectedAlgorithmSettings.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBoxExtraLaunchParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSelectedAlgorithmSettings
            // 
            this.groupBoxSelectedAlgorithmSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSelectedAlgorithmSettings.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxSelectedAlgorithmSettings.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.groupBoxSelectedAlgorithmSettings.Location = new System.Drawing.Point(4, 4);
            this.groupBoxSelectedAlgorithmSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 1);
            this.groupBoxSelectedAlgorithmSettings.Name = "groupBoxSelectedAlgorithmSettings";
            this.groupBoxSelectedAlgorithmSettings.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxSelectedAlgorithmSettings.Size = new System.Drawing.Size(376, 258);
            this.groupBoxSelectedAlgorithmSettings.TabIndex = 11;
            this.groupBoxSelectedAlgorithmSettings.TabStop = false;
            this.groupBoxSelectedAlgorithmSettings.Text = "Selected Algorithm Settings:";
            this.groupBoxSelectedAlgorithmSettings.UseCompatibleTextRendering = true;
            this.groupBoxSelectedAlgorithmSettings.Resize += new System.EventHandler(this.groupBoxSelectedAlgorithmSettings_Resize);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.fieldBoxBenchmarkSpeed);
            this.flowLayoutPanel1.Controls.Add(this.secondaryFieldBoxBenchmarkSpeed);
            this.flowLayoutPanel1.Controls.Add(this.field_PowerUsage);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxExtraLaunchParameters);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 19);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(368, 235);
            this.flowLayoutPanel1.TabIndex = 12;
            // 
            // fieldBoxBenchmarkSpeed
            // 
            this.fieldBoxBenchmarkSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldBoxBenchmarkSpeed.AutoSize = true;
            this.fieldBoxBenchmarkSpeed.BackColor = System.Drawing.Color.Transparent;
            this.fieldBoxBenchmarkSpeed.EntryText = "";
            this.fieldBoxBenchmarkSpeed.LabelText = "Benchmark Speed (H/s):";
            this.fieldBoxBenchmarkSpeed.Location = new System.Drawing.Point(4, 0);
            this.fieldBoxBenchmarkSpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.fieldBoxBenchmarkSpeed.Name = "fieldBoxBenchmarkSpeed";
            this.fieldBoxBenchmarkSpeed.Size = new System.Drawing.Size(358, 30);
            this.fieldBoxBenchmarkSpeed.TabIndex = 1;
            // 
            // secondaryFieldBoxBenchmarkSpeed
            // 
            this.secondaryFieldBoxBenchmarkSpeed.AutoSize = true;
            this.secondaryFieldBoxBenchmarkSpeed.BackColor = System.Drawing.Color.Transparent;
            this.secondaryFieldBoxBenchmarkSpeed.EntryText = "";
            this.secondaryFieldBoxBenchmarkSpeed.LabelText = "Secondary Speed (H/s):";
            this.secondaryFieldBoxBenchmarkSpeed.Location = new System.Drawing.Point(4, 30);
            this.secondaryFieldBoxBenchmarkSpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.secondaryFieldBoxBenchmarkSpeed.Name = "secondaryFieldBoxBenchmarkSpeed";
            this.secondaryFieldBoxBenchmarkSpeed.Size = new System.Drawing.Size(358, 30);
            this.secondaryFieldBoxBenchmarkSpeed.TabIndex = 16;
            // 
            // field_PowerUsage
            // 
            this.field_PowerUsage.AutoSize = true;
            this.field_PowerUsage.BackColor = System.Drawing.Color.Transparent;
            this.field_PowerUsage.EntryText = "";
            this.field_PowerUsage.LabelText = "Power Usage (W):";
            this.field_PowerUsage.Location = new System.Drawing.Point(4, 60);
            this.field_PowerUsage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.field_PowerUsage.Name = "field_PowerUsage";
            this.field_PowerUsage.Size = new System.Drawing.Size(358, 30);
            this.field_PowerUsage.TabIndex = 15;
            // 
            // groupBoxExtraLaunchParameters
            // 
            this.groupBoxExtraLaunchParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxExtraLaunchParameters.Controls.Add(this.richTextBoxExtraLaunchParameters);
            this.groupBoxExtraLaunchParameters.Location = new System.Drawing.Point(4, 94);
            this.groupBoxExtraLaunchParameters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxExtraLaunchParameters.Name = "groupBoxExtraLaunchParameters";
            this.groupBoxExtraLaunchParameters.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxExtraLaunchParameters.Size = new System.Drawing.Size(357, 66);
            this.groupBoxExtraLaunchParameters.TabIndex = 14;
            this.groupBoxExtraLaunchParameters.TabStop = false;
            this.groupBoxExtraLaunchParameters.Text = "Extra Launch Parameters:";
            this.groupBoxExtraLaunchParameters.UseCompatibleTextRendering = true;
            // 
            // richTextBoxExtraLaunchParameters
            // 
            this.richTextBoxExtraLaunchParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxExtraLaunchParameters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxExtraLaunchParameters.Location = new System.Drawing.Point(8, 20);
            this.richTextBoxExtraLaunchParameters.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.richTextBoxExtraLaunchParameters.Multiline = true;
            this.richTextBoxExtraLaunchParameters.Name = "richTextBoxExtraLaunchParameters";
            this.richTextBoxExtraLaunchParameters.Size = new System.Drawing.Size(341, 39);
            this.richTextBoxExtraLaunchParameters.TabIndex = 17;
            // 
            // AlgorithmSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxSelectedAlgorithmSettings);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "AlgorithmSettingsControl";
            this.Size = new System.Drawing.Size(380, 271);
            this.groupBoxSelectedAlgorithmSettings.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBoxExtraLaunchParameters.ResumeLayout(false);
            this.groupBoxExtraLaunchParameters.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSelectedAlgorithmSettings;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Field fieldBoxBenchmarkSpeed;
        private Field secondaryFieldBoxBenchmarkSpeed;
        private Field field_PowerUsage;
        private System.Windows.Forms.GroupBox groupBoxExtraLaunchParameters;
        private System.Windows.Forms.TextBox richTextBoxExtraLaunchParameters;
    }
}
