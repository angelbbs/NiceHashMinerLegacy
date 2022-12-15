
namespace NiceHashMiner.Forms
{
    partial class Form_RigProfitChart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_RigProfitChart));
            this.chartRigProfit = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label_totalEfficiency = new System.Windows.Forms.Label();
            this.checkBox_StartChartWithProgram = new System.Windows.Forms.CheckBox();
            this.label_Total_actual_profitabilities = new System.Windows.Forms.Label();
            this.checkBox_Chart_Fiat = new System.Windows.Forms.CheckBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.checkBox_EnableChart = new System.Windows.Forms.CheckBox();
            this.label_Total_local_profit = new System.Windows.Forms.Label();
            this.buttonClear = new System.Windows.Forms.Button();
            this.labelChartDisabled = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.chartRigProfit)).BeginInit();
            this.SuspendLayout();
            // 
            // chartRigProfit
            // 
            this.chartRigProfit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartRigProfit.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.Name = "ChartArea1";
            this.chartRigProfit.ChartAreas.Add(chartArea1);
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.AutoFitMinFontSize = 8;
            legend1.BackColor = System.Drawing.SystemColors.Control;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            legend1.IsTextAutoFit = false;
            legend1.Name = "Legend1";
            this.chartRigProfit.Legends.Add(legend1);
            this.chartRigProfit.Location = new System.Drawing.Point(-3, 0);
            this.chartRigProfit.Margin = new System.Windows.Forms.Padding(0);
            this.chartRigProfit.Name = "chartRigProfit";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Series2";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Series3";
            this.chartRigProfit.Series.Add(series1);
            this.chartRigProfit.Series.Add(series2);
            this.chartRigProfit.Series.Add(series3);
            this.chartRigProfit.Size = new System.Drawing.Size(758, 278);
            this.chartRigProfit.TabIndex = 0;
            this.chartRigProfit.Text = "chart1";
            this.chartRigProfit.Click += new System.EventHandler(this.chartRigProfit_Click);
            this.chartRigProfit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chartRigProfit_MouseDown);
            this.chartRigProfit.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chartRigProfit_MouseMove);
            this.chartRigProfit.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chartRigProfit_MouseUp);
            // 
            // label_totalEfficiency
            // 
            this.label_totalEfficiency.AutoSize = true;
            this.label_totalEfficiency.Location = new System.Drawing.Point(14, 282);
            this.label_totalEfficiency.Name = "label_totalEfficiency";
            this.label_totalEfficiency.Size = new System.Drawing.Size(115, 13);
            this.label_totalEfficiency.TabIndex = 1;
            this.label_totalEfficiency.Text = "Total mining efficiency:";
            this.label_totalEfficiency.Click += new System.EventHandler(this.label_totalEfficiency_Click);
            // 
            // checkBox_StartChartWithProgram
            // 
            this.checkBox_StartChartWithProgram.AutoSize = true;
            this.checkBox_StartChartWithProgram.Location = new System.Drawing.Point(325, 304);
            this.checkBox_StartChartWithProgram.Name = "checkBox_StartChartWithProgram";
            this.checkBox_StartChartWithProgram.Size = new System.Drawing.Size(134, 17);
            this.checkBox_StartChartWithProgram.TabIndex = 5;
            this.checkBox_StartChartWithProgram.Text = "StartChartWithProgram";
            this.checkBox_StartChartWithProgram.UseVisualStyleBackColor = true;
            this.checkBox_StartChartWithProgram.CheckedChanged += new System.EventHandler(this.checkBox_StartChartWithProgram_CheckedChanged);
            // 
            // label_Total_actual_profitabilities
            // 
            this.label_Total_actual_profitabilities.AutoSize = true;
            this.label_Total_actual_profitabilities.Location = new System.Drawing.Point(14, 328);
            this.label_Total_actual_profitabilities.Name = "label_Total_actual_profitabilities";
            this.label_Total_actual_profitabilities.Size = new System.Drawing.Size(126, 13);
            this.label_Total_actual_profitabilities.TabIndex = 6;
            this.label_Total_actual_profitabilities.Text = "Total actual profitabilities:";
            this.label_Total_actual_profitabilities.Click += new System.EventHandler(this.label_Total_actual_profitabilities_Click);
            // 
            // checkBox_Chart_Fiat
            // 
            this.checkBox_Chart_Fiat.AutoSize = true;
            this.checkBox_Chart_Fiat.Location = new System.Drawing.Point(325, 327);
            this.checkBox_Chart_Fiat.Name = "checkBox_Chart_Fiat";
            this.checkBox_Chart_Fiat.Size = new System.Drawing.Size(177, 17);
            this.checkBox_Chart_Fiat.TabIndex = 7;
            this.checkBox_Chart_Fiat.Text = "Show profitability in fiat currency";
            this.checkBox_Chart_Fiat.UseVisualStyleBackColor = true;
            this.checkBox_Chart_Fiat.CheckedChanged += new System.EventHandler(this.checkBox_Chart_Fiat_CheckedChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.FlatAppearance.BorderSize = 0;
            this.buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSave.Image = global::NiceHashMiner.Properties.Resources.floppy2;
            this.buttonSave.Location = new System.Drawing.Point(4, 4);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(0);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(32, 32);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // checkBox_EnableChart
            // 
            this.checkBox_EnableChart.AutoSize = true;
            this.checkBox_EnableChart.Location = new System.Drawing.Point(325, 281);
            this.checkBox_EnableChart.Name = "checkBox_EnableChart";
            this.checkBox_EnableChart.Size = new System.Drawing.Size(138, 17);
            this.checkBox_EnableChart.TabIndex = 9;
            this.checkBox_EnableChart.Text = "Enable profitability chart";
            this.checkBox_EnableChart.UseVisualStyleBackColor = true;
            this.checkBox_EnableChart.CheckedChanged += new System.EventHandler(this.checkBox_EnableChart_CheckedChanged);
            // 
            // label_Total_local_profit
            // 
            this.label_Total_local_profit.AutoSize = true;
            this.label_Total_local_profit.Location = new System.Drawing.Point(14, 305);
            this.label_Total_local_profit.Name = "label_Total_local_profit";
            this.label_Total_local_profit.Size = new System.Drawing.Size(119, 13);
            this.label_Total_local_profit.TabIndex = 10;
            this.label_Total_local_profit.Text = "Total local profitabilities:";
            // 
            // buttonClear
            // 
            this.buttonClear.FlatAppearance.BorderSize = 0;
            this.buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClear.Location = new System.Drawing.Point(4, 50);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(0);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(32, 32);
            this.buttonClear.TabIndex = 11;
            this.buttonClear.Text = "clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // labelChartDisabled
            // 
            this.labelChartDisabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelChartDisabled.AutoSize = true;
            this.labelChartDisabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelChartDisabled.ForeColor = System.Drawing.Color.Red;
            this.labelChartDisabled.Location = new System.Drawing.Point(247, 130);
            this.labelChartDisabled.Name = "labelChartDisabled";
            this.labelChartDisabled.Size = new System.Drawing.Size(259, 25);
            this.labelChartDisabled.TabIndex = 12;
            this.labelChartDisabled.Text = "Profitability chart disabled";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(252, 91);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(286, 23);
            this.progressBar1.TabIndex = 13;
            // 
            // Form_RigProfitChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 351);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.labelChartDisabled);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.label_Total_local_profit);
            this.Controls.Add(this.checkBox_EnableChart);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.checkBox_Chart_Fiat);
            this.Controls.Add(this.label_Total_actual_profitabilities);
            this.Controls.Add(this.checkBox_StartChartWithProgram);
            this.Controls.Add(this.label_totalEfficiency);
            this.Controls.Add(this.chartRigProfit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1280, 390);
            this.MinimumSize = new System.Drawing.Size(720, 390);
            this.Name = "Form_RigProfitChart";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form_RigProfitChart";
            this.Deactivate += new System.EventHandler(this.Form_RigProfitChart_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_RigProfitChart_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_RigProfitChart_FormClosed);
            this.Shown += new System.EventHandler(this.Form_RigProfitChart_Shown);
            this.ResizeBegin += new System.EventHandler(this.Form_RigProfitChart_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Form_RigProfitChart_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.Form_RigProfitChart_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.chartRigProfit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartRigProfit;
        private System.Windows.Forms.Label label_totalEfficiency;
        private System.Windows.Forms.CheckBox checkBox_StartChartWithProgram;
        private System.Windows.Forms.Label label_Total_actual_profitabilities;
        private System.Windows.Forms.CheckBox checkBox_Chart_Fiat;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.CheckBox checkBox_EnableChart;
        private System.Windows.Forms.Label label_Total_local_profit;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label labelChartDisabled;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}