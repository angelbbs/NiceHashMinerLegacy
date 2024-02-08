
namespace NiceHashMiner.Forms
{
    partial class Form_API_keys
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
            this.textBox_Organization_ID = new System.Windows.Forms.TextBox();
            this.label_Organization_ID = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBox_APIkey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_APIsecret = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.linkLabelGetAPIkey = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // textBox_Organization_ID
            // 
            this.textBox_Organization_ID.HideSelection = false;
            this.textBox_Organization_ID.Location = new System.Drawing.Point(95, 9);
            this.textBox_Organization_ID.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_Organization_ID.Name = "textBox_Organization_ID";
            this.textBox_Organization_ID.Size = new System.Drawing.Size(230, 20);
            this.textBox_Organization_ID.TabIndex = 382;
            this.textBox_Organization_ID.TabStop = false;
            // 
            // label_Organization_ID
            // 
            this.label_Organization_ID.AutoSize = true;
            this.label_Organization_ID.Location = new System.Drawing.Point(11, 12);
            this.label_Organization_ID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Organization_ID.Name = "label_Organization_ID";
            this.label_Organization_ID.Size = new System.Drawing.Size(80, 13);
            this.label_Organization_ID.TabIndex = 383;
            this.label_Organization_ID.Text = "Organization ID";
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(175, 100);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(97, 23);
            this.buttonSave.TabIndex = 384;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(308, 100);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(97, 23);
            this.buttonCancel.TabIndex = 385;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBox_APIkey
            // 
            this.textBox_APIkey.Location = new System.Drawing.Point(95, 35);
            this.textBox_APIkey.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_APIkey.Name = "textBox_APIkey";
            this.textBox_APIkey.Size = new System.Drawing.Size(230, 20);
            this.textBox_APIkey.TabIndex = 386;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 387;
            this.label1.Text = "API key";
            // 
            // textBox_APIsecret
            // 
            this.textBox_APIsecret.Location = new System.Drawing.Point(95, 61);
            this.textBox_APIsecret.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_APIsecret.Name = "textBox_APIsecret";
            this.textBox_APIsecret.PasswordChar = '•';
            this.textBox_APIsecret.Size = new System.Drawing.Size(503, 20);
            this.textBox_APIsecret.TabIndex = 388;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 389;
            this.label2.Text = "API secret";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(480, 100);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(118, 23);
            this.buttonDelete.TabIndex = 390;
            this.buttonDelete.Text = "Delete API keys";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // linkLabelGetAPIkey
            // 
            this.linkLabelGetAPIkey.AutoSize = true;
            this.linkLabelGetAPIkey.Location = new System.Drawing.Point(413, 12);
            this.linkLabelGetAPIkey.Name = "linkLabelGetAPIkey";
            this.linkLabelGetAPIkey.Size = new System.Drawing.Size(64, 13);
            this.linkLabelGetAPIkey.TabIndex = 391;
            this.linkLabelGetAPIkey.TabStop = true;
            this.linkLabelGetAPIkey.Text = "Get API key";
            this.linkLabelGetAPIkey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGetAPIkey_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(413, 38);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(81, 13);
            this.linkLabel1.TabIndex = 392;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "API permissions";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Form_API_keys
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 135);
            this.ControlBox = false;
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.linkLabelGetAPIkey);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.textBox_APIsecret);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_APIkey);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBox_Organization_ID);
            this.Controls.Add(this.label_Organization_ID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_API_keys";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setup API keys";
            this.ResizeBegin += new System.EventHandler(this.Form_API_keys_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form_API_keys_ResizeEnd);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Organization_ID;
        private System.Windows.Forms.Label label_Organization_ID;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBox_APIkey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.TextBox textBox_APIsecret;
        private System.Windows.Forms.LinkLabel linkLabelGetAPIkey;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
