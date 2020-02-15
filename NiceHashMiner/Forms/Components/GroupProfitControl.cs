/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner.Configs;
using NiceHashMiner.Miners;
using NiceHashMinerLegacy.Common.Enums;
using System.Windows.Forms;

namespace NiceHashMiner.Forms.Components
{
    public partial class GroupProfitControl : UserControl
    {
        public GroupProfitControl()
        {
            InitializeComponent();

            labelSpeedIndicator.Text = International.GetText("ListView_Speed");
            labelBTCRateIndicator.Text = International.GetText("Rate");
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                toolTip2.SetToolTip(button_restart, "Перезапуск майнера");
            }
            else
            {
                toolTip2.SetToolTip(button_restart, "Restar miner");
            }
        }


        public void UpdateProfitStats(string groupName, string deviceStringInfo,
            string speedString, string btcRateString, string currencyRateString, string ProcessTag)
        {
            groupBoxMinerGroup.Text = string.Format(International.GetText("Form_Main_MiningDevices"), deviceStringInfo);
            labelSpeedValue.Text = speedString;
            labelBTCRateValue.Text = btcRateString;
            labelCurentcyPerDayVaue.Text = currencyRateString;
            groupBoxMinerGroup.ForeColor = Form_Main._foreColor;
            groupBoxMinerGroup.BackColor = Form_Main._backColor;
            button_restart.Tag = ProcessTag;

        }

        private void buttonBTC_Save_Click(object sender, System.EventArgs e)
        {
        }

        private void button_restart_MouseLeave(object sender, System.EventArgs e)
        {
            button_restart.Image = Properties.Resources.Refresh_normal;
        }

        private void button_restart_MouseMove(object sender, MouseEventArgs e)
        {
            button_restart.FlatAppearance.MouseOverBackColor = Form_Main._backColor;
            button_restart.Image = Properties.Resources.Refresh_hot;
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
            Form_Main.ActiveForm.Focus();
            Helpers.ConsolePrint("NICEHASH", "Restarting miner: " + button_restart.Tag.ToString());
            MiningSession.RestartMiner(button_restart.Tag.ToString());
        }
    }
}
