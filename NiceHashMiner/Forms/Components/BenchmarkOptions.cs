using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Linq;
using System.Windows.Forms;


namespace NiceHashMiner.Forms.Components
{
    public partial class BenchmarkOptions : UserControl
    {
        public BenchmarkPerformanceType PerformanceType { get; private set; }

        public BenchmarkOptions()
        {
            InitializeComponent();
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                this.BackColor = Form_Main._backColor;
                this.ForeColor = Form_Main._foreColor;
                foreach (var lbl in this.Controls.OfType<GroupBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                }
            }
        }

        public void SetPerformanceType(BenchmarkPerformanceType performanceType)
        {
            /*
            switch (performanceType)
            {
                case BenchmarkPerformanceType.Standard:
                    radioButton_StandardBenchmark.Checked = true;
                    PerformanceType = BenchmarkPerformanceType.Standard;
                    break;
                case BenchmarkPerformanceType.Precise:
                    radioButton_PreciseBenchmark.Checked = true;
                    PerformanceType = BenchmarkPerformanceType.Precise;
                    break;
                default:
                    radioButton_StandardBenchmark.Checked = true;
                    break;
            }
            */
            //radioButton_StandardBenchmark.Checked = ConfigManager.GeneralConfig.StandartBenchmarkTime;
            radioButton_PreciseBenchmark.Checked = !ConfigManager.GeneralConfig.StandartBenchmarkTime;
        }

        public void InitLocale()
        {
            groupBox1.Text = International.GetText("BenchmarkOptions_Benchmark_Type");
            radioButton_StandardBenchmark.Text = International.GetText("Form_Benchmark_radioButton_StandardBenchmark");
            radioButton_PreciseBenchmark.Text = International.GetText("Form_Benchmark_radioButton_PreciseBenchmark");
        }

        private void RadioButton_StandardBenchmark_CheckedChanged(object sender, EventArgs e)
        {
            PerformanceType = BenchmarkPerformanceType.Standard;
            ConfigManager.GeneralConfig.StandartBenchmarkTime = true;
        }

        private void RadioButton_PreciseBenchmark_CheckedChanged(object sender, EventArgs e)
        {
            PerformanceType = BenchmarkPerformanceType.Precise;
            ConfigManager.GeneralConfig.StandartBenchmarkTime = false;
        }
    }
}
