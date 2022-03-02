
namespace NiceHashMiner.Forms
{
    partial class Form_Downloading
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.LoadText = new System.Windows.Forms.Label();
            this.UnzippingText = new System.Windows.Forms.Label();
            this.labelTitle2 = new System.Windows.Forms.Label();
            this.progressBarUnzipping = new ProgressBarSample.TextProgressBar();
            this.progressBarDownloading = new ProgressBarSample.TextProgressBar();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(12, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(35, 13);
            this.labelTitle.TabIndex = 399;
            this.labelTitle.Text = "label1";
            // 
            // LoadText
            // 
            this.LoadText.AutoSize = true;
            this.LoadText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.LoadText.Location = new System.Drawing.Point(6, 65);
            this.LoadText.Name = "LoadText";
            this.LoadText.Size = new System.Drawing.Size(22, 13);
            this.LoadText.TabIndex = 400;
            this.LoadText.Text = "     ";
            // 
            // UnzippingText
            // 
            this.UnzippingText.AutoSize = true;
            this.UnzippingText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.UnzippingText.Location = new System.Drawing.Point(6, 155);
            this.UnzippingText.Name = "UnzippingText";
            this.UnzippingText.Size = new System.Drawing.Size(25, 13);
            this.UnzippingText.TabIndex = 403;
            this.UnzippingText.Text = "      ";
            // 
            // labelTitle2
            // 
            this.labelTitle2.AutoSize = true;
            this.labelTitle2.Location = new System.Drawing.Point(12, 99);
            this.labelTitle2.Name = "labelTitle2";
            this.labelTitle2.Size = new System.Drawing.Size(35, 13);
            this.labelTitle2.TabIndex = 402;
            this.labelTitle2.Text = "label1";
            // 
            // progressBarUnzipping
            // 
            this.progressBarUnzipping.CustomText = "";
            this.progressBarUnzipping.Location = new System.Drawing.Point(9, 121);
            this.progressBarUnzipping.Margin = new System.Windows.Forms.Padding(0);
            this.progressBarUnzipping.Name = "progressBarUnzipping";
            this.progressBarUnzipping.ProgressColor = System.Drawing.SystemColors.ControlDark;
            this.progressBarUnzipping.Size = new System.Drawing.Size(308, 23);
            this.progressBarUnzipping.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarUnzipping.TabIndex = 401;
            this.progressBarUnzipping.TextColor = System.Drawing.Color.Black;
            this.progressBarUnzipping.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.progressBarUnzipping.VisualMode = ProgressBarSample.ProgressBarDisplayMode.Percentage;
            // 
            // progressBarDownloading
            // 
            this.progressBarDownloading.CustomText = "";
            this.progressBarDownloading.Location = new System.Drawing.Point(9, 31);
            this.progressBarDownloading.Margin = new System.Windows.Forms.Padding(0);
            this.progressBarDownloading.Name = "progressBarDownloading";
            this.progressBarDownloading.ProgressColor = System.Drawing.SystemColors.ControlDark;
            this.progressBarDownloading.Size = new System.Drawing.Size(308, 23);
            this.progressBarDownloading.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarDownloading.TabIndex = 398;
            this.progressBarDownloading.TextColor = System.Drawing.Color.Black;
            this.progressBarDownloading.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.progressBarDownloading.VisualMode = ProgressBarSample.ProgressBarDisplayMode.Percentage;
            // 
            // Form_Downloading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 184);
            this.ControlBox = false;
            this.Controls.Add(this.UnzippingText);
            this.Controls.Add(this.labelTitle2);
            this.Controls.Add(this.progressBarUnzipping);
            this.Controls.Add(this.LoadText);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.progressBarDownloading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Downloading";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_Downloading";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelTitle;
        public ProgressBarSample.TextProgressBar progressBarDownloading;
        private System.Windows.Forms.Label LoadText;
        private System.Windows.Forms.Label UnzippingText;
        private System.Windows.Forms.Label labelTitle2;
        public ProgressBarSample.TextProgressBar progressBarUnzipping;
    }
}
