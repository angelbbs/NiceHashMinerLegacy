namespace NiceHashMiner.Forms {
    partial class Form_ChooseLanguage {
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
            this.label_Instruction = new System.Windows.Forms.Label();
            this.comboBox_Languages = new System.Windows.Forms.ComboBox();
            this.button_OK = new System.Windows.Forms.Button();
            this.checkBox_TOS = new System.Windows.Forms.CheckBox();
            this.textBox_TOS = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label_Instruction
            // 
            this.label_Instruction.AutoSize = true;
            this.label_Instruction.Location = new System.Drawing.Point(12, 368);
            this.label_Instruction.Name = "label_Instruction";
            this.label_Instruction.Size = new System.Drawing.Size(134, 13);
            this.label_Instruction.TabIndex = 0;
            this.label_Instruction.Text = "Choose a default language";
            // 
            // comboBox_Languages
            // 
            this.comboBox_Languages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Languages.Enabled = false;
            this.comboBox_Languages.FormattingEnabled = true;
            this.comboBox_Languages.Location = new System.Drawing.Point(15, 384);
            this.comboBox_Languages.Name = "comboBox_Languages";
            this.comboBox_Languages.Size = new System.Drawing.Size(131, 21);
            this.comboBox_Languages.TabIndex = 1;
            this.comboBox_Languages.SelectedIndexChanged += new System.EventHandler(this.comboBox_Languages_SelectedIndexChanged);
            // 
            // button_OK
            // 
            this.button_OK.Enabled = false;
            this.button_OK.Location = new System.Drawing.Point(168, 382);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(106, 23);
            this.button_OK.TabIndex = 2;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.Button_OK_Click);
            // 
            // checkBox_TOS
            // 
            this.checkBox_TOS.AutoSize = true;
            this.checkBox_TOS.Location = new System.Drawing.Point(15, 330);
            this.checkBox_TOS.Name = "checkBox_TOS";
            this.checkBox_TOS.Size = new System.Drawing.Size(147, 17);
            this.checkBox_TOS.TabIndex = 3;
            this.checkBox_TOS.Text = "I accept license condition";
            this.checkBox_TOS.UseVisualStyleBackColor = true;
            this.checkBox_TOS.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // textBox_TOS
            // 
            this.textBox_TOS.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_TOS.Location = new System.Drawing.Point(13, 13);
            this.textBox_TOS.Multiline = true;
            this.textBox_TOS.Name = "textBox_TOS";
            this.textBox_TOS.ReadOnly = true;
            this.textBox_TOS.Size = new System.Drawing.Size(418, 311);
            this.textBox_TOS.TabIndex = 4;
            this.textBox_TOS.TabStop = false;
            // 
            // Form_ChooseLanguage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 421);
            this.Controls.Add(this.textBox_TOS);
            this.Controls.Add(this.checkBox_TOS);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.comboBox_Languages);
            this.Controls.Add(this.label_Instruction);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_ChooseLanguage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Miner Legacy Fork Fix license  / Choose Language";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_ChooseLanguage_FormClosing);
            this.ResizeBegin += new System.EventHandler(this.Form_ChooseLanguage_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form_ChooseLanguage_ResizeEnd);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Instruction;
        private System.Windows.Forms.ComboBox comboBox_Languages;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.CheckBox checkBox_TOS;
        private System.Windows.Forms.TextBox textBox_TOS;
    }
}