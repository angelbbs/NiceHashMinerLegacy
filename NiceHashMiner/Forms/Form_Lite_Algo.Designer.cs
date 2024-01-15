
namespace NiceHashMiner.Forms
{
    partial class Form_Lite_Algo
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
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.groupBox_KAWPOWLite = new System.Windows.Forms.GroupBox();
            this.textBox_MaxEpoch5gb = new System.Windows.Forms.TextBox();
            this.label_MaxEpoch5 = new System.Windows.Forms.Label();
            this.textBox_MaxEpoch4gb = new System.Windows.Forms.TextBox();
            this.label_MaxEpoch4 = new System.Windows.Forms.Label();
            this.textBox_MaxEpoch3gb = new System.Windows.Forms.TextBox();
            this.label_MaxEpoch3 = new System.Windows.Forms.Label();
            this.groupBox_KAWPOWLite.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Cancel.Location = new System.Drawing.Point(93, 133);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 4;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Save
            // 
            this.button_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Save.Location = new System.Drawing.Point(12, 133);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 3;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // groupBox_KAWPOWLite
            // 
            this.groupBox_KAWPOWLite.Controls.Add(this.textBox_MaxEpoch5gb);
            this.groupBox_KAWPOWLite.Controls.Add(this.label_MaxEpoch5);
            this.groupBox_KAWPOWLite.Controls.Add(this.textBox_MaxEpoch4gb);
            this.groupBox_KAWPOWLite.Controls.Add(this.label_MaxEpoch4);
            this.groupBox_KAWPOWLite.Controls.Add(this.textBox_MaxEpoch3gb);
            this.groupBox_KAWPOWLite.Controls.Add(this.label_MaxEpoch3);
            this.groupBox_KAWPOWLite.Location = new System.Drawing.Point(12, 12);
            this.groupBox_KAWPOWLite.Name = "groupBox_KAWPOWLite";
            this.groupBox_KAWPOWLite.Size = new System.Drawing.Size(276, 106);
            this.groupBox_KAWPOWLite.TabIndex = 5;
            this.groupBox_KAWPOWLite.TabStop = false;
            this.groupBox_KAWPOWLite.Text = "KAWPOWLite";
            // 
            // textBox_MaxEpoch5gb
            // 
            this.textBox_MaxEpoch5gb.Location = new System.Drawing.Point(18, 70);
            this.textBox_MaxEpoch5gb.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MaxEpoch5gb.Name = "textBox_MaxEpoch5gb";
            this.textBox_MaxEpoch5gb.Size = new System.Drawing.Size(39, 20);
            this.textBox_MaxEpoch5gb.TabIndex = 362;
            this.textBox_MaxEpoch5gb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_MaxEpoch5gb_KeyPress);
            // 
            // label_MaxEpoch5
            // 
            this.label_MaxEpoch5.AutoSize = true;
            this.label_MaxEpoch5.Location = new System.Drawing.Point(61, 75);
            this.label_MaxEpoch5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MaxEpoch5.Name = "label_MaxEpoch5";
            this.label_MaxEpoch5.Size = new System.Drawing.Size(149, 13);
            this.label_MaxEpoch5.TabIndex = 363;
            this.label_MaxEpoch5.Text = "Maximum epoch for 5GB GPU";
            // 
            // textBox_MaxEpoch4gb
            // 
            this.textBox_MaxEpoch4gb.Location = new System.Drawing.Point(18, 47);
            this.textBox_MaxEpoch4gb.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MaxEpoch4gb.Name = "textBox_MaxEpoch4gb";
            this.textBox_MaxEpoch4gb.Size = new System.Drawing.Size(39, 20);
            this.textBox_MaxEpoch4gb.TabIndex = 360;
            this.textBox_MaxEpoch4gb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_MaxEpoch4gb_KeyPress);
            // 
            // label_MaxEpoch4
            // 
            this.label_MaxEpoch4.AutoSize = true;
            this.label_MaxEpoch4.Location = new System.Drawing.Point(61, 52);
            this.label_MaxEpoch4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MaxEpoch4.Name = "label_MaxEpoch4";
            this.label_MaxEpoch4.Size = new System.Drawing.Size(149, 13);
            this.label_MaxEpoch4.TabIndex = 361;
            this.label_MaxEpoch4.Text = "Maximum epoch for 4GB GPU";
            // 
            // textBox_MaxEpoch3gb
            // 
            this.textBox_MaxEpoch3gb.Location = new System.Drawing.Point(18, 24);
            this.textBox_MaxEpoch3gb.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_MaxEpoch3gb.Name = "textBox_MaxEpoch3gb";
            this.textBox_MaxEpoch3gb.Size = new System.Drawing.Size(39, 20);
            this.textBox_MaxEpoch3gb.TabIndex = 358;
            this.textBox_MaxEpoch3gb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_MaxEpoch3gb_KeyPress);
            // 
            // label_MaxEpoch3
            // 
            this.label_MaxEpoch3.AutoSize = true;
            this.label_MaxEpoch3.Location = new System.Drawing.Point(61, 29);
            this.label_MaxEpoch3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_MaxEpoch3.Name = "label_MaxEpoch3";
            this.label_MaxEpoch3.Size = new System.Drawing.Size(149, 13);
            this.label_MaxEpoch3.TabIndex = 359;
            this.label_MaxEpoch3.Text = "Maximum epoch for 3GB GPU";
            // 
            // Form_Lite_Algo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 172);
            this.Controls.Add(this.groupBox_KAWPOWLite);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Save);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Lite_Algo";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form_Lite_Algo";
            this.groupBox_KAWPOWLite.ResumeLayout(false);
            this.groupBox_KAWPOWLite.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.GroupBox groupBox_KAWPOWLite;
        private System.Windows.Forms.TextBox textBox_MaxEpoch3gb;
        private System.Windows.Forms.Label label_MaxEpoch3;
        private System.Windows.Forms.TextBox textBox_MaxEpoch5gb;
        private System.Windows.Forms.Label label_MaxEpoch5;
        private System.Windows.Forms.TextBox textBox_MaxEpoch4gb;
        private System.Windows.Forms.Label label_MaxEpoch4;
    }
}