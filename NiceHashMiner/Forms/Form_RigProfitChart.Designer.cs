
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
            this.chartRigProfit = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label_totalEfficiency = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartRigProfit)).BeginInit();
            this.SuspendLayout();
            // 
            // chartRigProfit
            // 
            chartArea1.Name = "ChartArea1";
            this.chartRigProfit.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartRigProfit.Legends.Add(legend1);
            this.chartRigProfit.Location = new System.Drawing.Point(12, 12);
            this.chartRigProfit.Name = "chartRigProfit";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Legend = "Legend1";
            series2.Name = "Series2";
            this.chartRigProfit.Series.Add(series1);
            this.chartRigProfit.Series.Add(series2);
            this.chartRigProfit.Size = new System.Drawing.Size(776, 300);
            this.chartRigProfit.TabIndex = 0;
            this.chartRigProfit.Text = "chart1";
            this.chartRigProfit.Customize += new System.EventHandler(this.chartRigProfit_Customize);
            this.chartRigProfit.Click += new System.EventHandler(this.chartRigProfit_Click);
            // 
            // label_totalEfficiency
            // 
            this.label_totalEfficiency.AutoSize = true;
            this.label_totalEfficiency.Location = new System.Drawing.Point(12, 325);
            this.label_totalEfficiency.Name = "label_totalEfficiency";
            this.label_totalEfficiency.Size = new System.Drawing.Size(56, 13);
            this.label_totalEfficiency.TabIndex = 1;
            this.label_totalEfficiency.Text = "Efficiency:";
            // 
            // Form_RigProfitChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label_totalEfficiency);
            this.Controls.Add(this.chartRigProfit);
            this.Name = "Form_RigProfitChart";
            this.Text = "Form_RigProfitChart";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_RigProfitChart_FormClosing);
            this.Shown += new System.EventHandler(this.Form_RigProfitChart_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chartRigProfit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartRigProfit;
        private System.Windows.Forms.Label label_totalEfficiency;
    }
}