namespace NiceHashMiner.Forms {
    partial class Form_3rdParty_TOS {
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
            this.button_Agree = new System.Windows.Forms.Button();
            this.label_Tos = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // button_Agree
            //
            this.button_Agree.Location = new System.Drawing.Point(128, 260);
            this.button_Agree.Name = "button_Agree";
            this.button_Agree.Size = new System.Drawing.Size(146, 23);
            this.button_Agree.TabIndex = 0;
            this.button_Agree.Text = "OK";
            this.button_Agree.UseVisualStyleBackColor = true;
            this.button_Agree.Click += new System.EventHandler(this.Button_Agree_Click);
            //
            // label_Tos
            //
            this.label_Tos.AutoSize = true;
            this.label_Tos.Location = new System.Drawing.Point(12, 9);
            this.label_Tos.MaximumSize = new System.Drawing.Size(350, 0);
            this.label_Tos.Name = "label_Tos";
            this.label_Tos.Size = new System.Drawing.Size(35, 13);
            this.label_Tos.TabIndex = 2;
            this.label_Tos.Text = "label1";
            this.label_Tos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // Form_3rdParty_TOS
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 295);
            this.Controls.Add(this.label_Tos);
            this.Controls.Add(this.button_Agree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_3rdParty_TOS";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Disclaimer on usage of 3rd party software";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Agree;
        private System.Windows.Forms.Label label_Tos;
    }
}