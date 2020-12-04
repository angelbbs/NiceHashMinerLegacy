using System;
using System.Linq;
using System.Windows.Forms;
using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;


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
            switch (performanceType)
            {
                case BenchmarkPerformanceType.Quick:
                    radioButton_QuickBenchmark.Checked = true;
                    PerformanceType = BenchmarkPerformanceType.Quick;
                    break;
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
        }

        public void InitLocale()
        {
            groupBox1.Text = International.GetText("BenchmarkOptions_Benchmark_Type");
            radioButton_QuickBenchmark.Text = International.GetText("Form_Benchmark_radioButton_QuickBenchmark");
            radioButton_StandardBenchmark.Text = International.GetText("Form_Benchmark_radioButton_StandardBenchmark");
            radioButton_PreciseBenchmark.Text = International.GetText("Form_Benchmark_radioButton_PreciseBenchmark");
        }

        private void RadioButton_QuickBenchmark_CheckedChanged(object sender, EventArgs e)
        {
            PerformanceType = BenchmarkPerformanceType.Quick;
        }

        private void RadioButton_StandardBenchmark_CheckedChanged(object sender, EventArgs e)
        {
            PerformanceType = BenchmarkPerformanceType.Standard;
        }

        private void RadioButton_PreciseBenchmark_CheckedChanged(object sender, EventArgs e)
        {
            PerformanceType = BenchmarkPerformanceType.Precise;
        }
    }
}
