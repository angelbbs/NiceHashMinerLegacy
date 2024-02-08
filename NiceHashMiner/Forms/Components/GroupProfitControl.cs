using NiceHashMiner.Configs;
using NiceHashMiner.Miners;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Windows.Forms;

namespace NiceHashMiner.Forms.Components
{
    public partial class GroupProfitControl : UserControl
    {
        public GroupProfitControl()
        {
            InitializeComponent();

            labelBTCRateIndicator.Text = International.GetText("Rate");
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                toolTip2.SetToolTip(button_restart, "Перезапуск майнера");
            }
            else
            {
                toolTip2.SetToolTip(button_restart, "Restart miner");
            }
            richTextBoxSpeedValue.SelectionColor = Form_Main._foreColor;
            groupBoxMinerGroup.ForeColor = Form_Main._foreColor;
            groupBoxMinerGroup.BackColor = Form_Main._backColor;
            groupBoxMinerGroup.DoubleBuffer();
        }


        public void UpdateProfitStats(string groupName, string deviceStringInfo,
            string speedString, DateTime StartMinerTime, string btcRateString, string currencyRateString, string ProcessTag)
        {
            if (ConfigManager.GeneralConfig.ShowUptime)
            {
                var timenow = DateTime.Now;
                TimeSpan Uptime = timenow.Subtract(StartMinerTime);
                groupBoxMinerGroup.Text = string.Format(International.GetText("Form_Main_MiningDevices"), deviceStringInfo) +
                    "  " + International.GetText("Form_Main_Miner") + groupName.Split('-')[0] +
                    MinerVersion.GetMinerVersion(groupName.Split('-')[0]) +
                    "  " + International.GetText("Form_Main_Uptime") + " " + Uptime.ToString(@"d\ \d\a\y\s\ hh\:mm\:ss");
            }
            else
            {
                groupBoxMinerGroup.Text = string.Format(International.GetText("Form_Main_MiningDevices"), deviceStringInfo);
            }

            labelBTCRateValue.Text = btcRateString;
            labelCurentcyPerDayVaue.Text = currencyRateString;

            richTextBoxSpeedValue.Rtf = speedString;
            richTextBoxSpeedValue.SelectionStart = 0;
            richTextBoxSpeedValue.SelectionLength = richTextBoxSpeedValue.Text.Length;
            richTextBoxSpeedValue.SelectionColor = Form_Main._foreColor;
            richTextBoxSpeedValue.BackColor = Form_Main._backColor;

            button_restart.Tag = ProcessTag;
            button_restart.Refresh();
        }


        private void button_restart_MouseLeave(object sender, System.EventArgs e)
        {
            button_restart.Image = Properties.Resources.Refresh_disabled;
        }

        private void button_restart_MouseMove(object sender, MouseEventArgs e)
        {
            button_restart.FlatAppearance.MouseOverBackColor = Form_Main._backColor;
            button_restart.Image = Properties.Resources.Refresh_hot_bw;
        }

        private void labelCurentcyPerDayVaue_Click(object sender, System.EventArgs e)
        {

        }

        private void labelBTCRateValue_Click(object sender, System.EventArgs e)
        {

        }

        private void labelBTCRateIndicator_Click(object sender, System.EventArgs e)
        {

        }

        private void buttonBTC_restart(object sender, System.EventArgs e)
        {
            // Form_Main.ActiveForm.Focus();
            Helpers.ConsolePrint("NICEHASH", "Restarting miner: " + button_restart.Tag.ToString());
            MiningSession.RestartMiner(button_restart.Tag.ToString());
            //button_restart.Enabled = false;
            button_restart.ForeColor = Form_Main._backColor;
            button_restart.Image = Properties.Resources.Refresh_disabled;
            button_restart.UseVisualStyleBackColor = false;
            button_restart.FlatAppearance.BorderSize = 0;
            button_restart.Update();
        }
    }
}
