using NiceHashMiner.Miners;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SystemTimer = System.Timers.Timer;
using Timer = System.Windows.Forms.Timer;

namespace NiceHashMiner.Forms
{
    public partial class Form_RigProfitChart : Form
    {
        public System.Windows.Forms.Timer _RigProfitChartTimer;
        public int ProfitsCount = 0;

        public Form_RigProfitChart()
        {
            InitializeComponent();

        }
        void ChartData(object sender, EventArgs e)
        {
            double totalRate = MinersManager.GetTotalRate();
            var rpl = Form_Main.lastRigProfit;
            //Helpers.ConsolePrint("RigProfitList", rpl.currentProfit.ToString() + " " + totalRate.ToString());
            chartRigProfit.Series["Series1"].Points.AddXY(ProfitsCount, totalRate * 1000);
            chartRigProfit.Series["Series2"].Points.AddXY(ProfitsCount, rpl.currentProfit * 1000);
            ProfitsCount++;

        }

        private void chartRigProfit_Click(object sender, EventArgs e)
        {


        }
        private Timer _updateTimer;
        string[] range = new string[24];
        private void Form_RigProfitChart_Shown(object sender, EventArgs e)
        {
            chartRigProfit.Series["Series1"].LegendText = International.GetText("Form_Main_current_local_profitabilities");
            chartRigProfit.Series["Series1"].BorderWidth = 2;
            chartRigProfit.Series["Series2"].LegendText = International.GetText("Form_Main_current_actual_profitabilities");
            chartRigProfit.Series["Series2"].BorderWidth = 2;
            chartRigProfit.Series["Series1"].ChartType = SeriesChartType.Spline;
            chartRigProfit.Series["Series2"].ChartType = SeriesChartType.Spline;
            //chartRigProfit.ChartAreas[0].AxisX.Maximum = 24;
            chartRigProfit.ChartAreas[0].AxisX.Minimum = 0;
            chartRigProfit.ChartAreas[0].AxisY.Minimum = 0;
            chartRigProfit.ChartAreas[0].AxisX.Interval = 60;//
            chartRigProfit.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chartRigProfit.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartRigProfit.ChartAreas[0].AxisY.Title = "mBTC/" + International.GetText("Day");
            chartRigProfit.ChartAreas[0].AxisX.Title = International.GetText("Form_Main_Uptime");
            //chartRigProfit.ChartAreas[0].BorderWidth = 20;
            //chartRigProfit.ChartAreas[0].AxisX.LabelStyle.Format = "mm:ss";
            //chartRigProfit.Series[0].XValueType = ChartValueType.DateTime;
            range[0] = "0";

            _updateTimer = new Timer();
            _updateTimer.Tick += ChartData;
            _updateTimer.Interval = 1000 * 5;
            _updateTimer.Start();
        }

        private void Form_RigProfitChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            _updateTimer.Stop();
            _updateTimer.Dispose();
        }

        private void chartRigProfit_Customize(object sender, EventArgs e)
        {

        }
    }
}
