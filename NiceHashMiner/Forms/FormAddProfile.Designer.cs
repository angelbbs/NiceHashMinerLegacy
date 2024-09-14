
namespace NiceHashMiner.Forms
{
    partial class FormAddProfile
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
            this.label_NewProfileName = new System.Windows.Forms.Label();
            this.textBox_NewProfileName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Cancel.Location = new System.Drawing.Point(301, 16);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 6;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Save
            // 
            this.button_Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Save.Location = new System.Drawing.Point(220, 16);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 5;
            this.button_Save.Text = "Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // label_NewProfileName
            // 
            this.label_NewProfileName.AutoSize = true;
            this.label_NewProfileName.Location = new System.Drawing.Point(9, 21);
            this.label_NewProfileName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_NewProfileName.Name = "label_NewProfileName";
            this.label_NewProfileName.Size = new System.Drawing.Size(104, 13);
            this.label_NewProfileName.TabIndex = 358;
            this.label_NewProfileName.Text = "Enter a profile name:";
            // 
            // textBox_NewProfileName
            // 
            this.textBox_NewProfileName.Location = new System.Drawing.Point(117, 18);
            this.textBox_NewProfileName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_NewProfileName.Name = "textBox_NewProfileName";
            this.textBox_NewProfileName.Size = new System.Drawing.Size(84, 20);
            this.textBox_NewProfileName.TabIndex = 359;
            this.textBox_NewProfileName.TextChanged += new System.EventHandler(this.textBox_NewProfileName_TextChanged);
            // 
            // FormAddProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(391, 51);
            this.ControlBox = false;
            this.Controls.Add(this.textBox_NewProfileName);
            this.Controls.Add(this.label_NewProfileName);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddProfile";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormAddProfile";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Label label_NewProfileName;
        private System.Windows.Forms.TextBox textBox_NewProfileName;
    }
}