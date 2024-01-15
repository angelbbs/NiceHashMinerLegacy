using NiceHashMiner.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Forms
{
    public partial class Form_Lite_Algo : Form
    {
        public Form_Lite_Algo()
        {
            InitializeComponent();
            textBox_MaxEpoch3gb.Text = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch3GB.ToString();
            textBox_MaxEpoch4gb.Text = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch4GB.ToString();
            textBox_MaxEpoch5gb.Text = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch5GB.ToString();

            this.Text = International.GetText("Form_Lite_Title");
            this.BackColor = Form_Main._backColor;
            this.ForeColor = Form_Main._textColor;

            button_Save.Text = International.GetText("Form_Lite_Save");
            button_Save.BackColor = Form_Main._backColor;
            button_Save.ForeColor = Form_Main._textColor;
            button_Cancel.Text = International.GetText("Form_Lite_Cancel");
            button_Cancel.BackColor = Form_Main._backColor;
            button_Cancel.ForeColor = Form_Main._textColor;

            Font fontRegular = new Font(this.Font, FontStyle.Regular);
            Font fontBold = new Font(this.Font, FontStyle.Bold);

            label_MaxEpoch3.Text = International.GetText("Form_Lite_3GB");
            textBox_MaxEpoch3gb.BackColor = Form_Main._backColor;
            textBox_MaxEpoch3gb.ForeColor = Form_Main._textColor;
            textBox_MaxEpoch3gb.BorderStyle = BorderStyle.FixedSingle;
            label_MaxEpoch4.Text = International.GetText("Form_Lite_4GB");
            textBox_MaxEpoch4gb.BackColor = Form_Main._backColor;
            textBox_MaxEpoch4gb.ForeColor = Form_Main._textColor;
            textBox_MaxEpoch4gb.BorderStyle = BorderStyle.FixedSingle;
            label_MaxEpoch5.Text = International.GetText("Form_Lite_5GB");
            textBox_MaxEpoch5gb.BackColor = Form_Main._backColor;
            textBox_MaxEpoch5gb.ForeColor = Form_Main._textColor;
            textBox_MaxEpoch5gb.BorderStyle = BorderStyle.FixedSingle;

            if (Form_Main.KawpowLite3GB)
            {
                label_MaxEpoch3.Font = fontBold;
                label_MaxEpoch4.Font = fontRegular;
                label_MaxEpoch5.Font = fontRegular;
            }
            if (Form_Main.KawpowLite4GB)
            {
                label_MaxEpoch3.Font = fontRegular;
                label_MaxEpoch4.Font = fontBold;
                label_MaxEpoch5.Font = fontRegular;
            }
            if (Form_Main.KawpowLite5GB)
            {
                label_MaxEpoch3.Font = fontRegular;
                label_MaxEpoch4.Font = fontRegular;
                label_MaxEpoch5.Font = fontBold;
            }
        }

        private void textBox_MaxEpoch3gb_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number))
            {
                e.Handled = true;
            }
        }

        private void textBox_MaxEpoch4gb_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number))
            {
                e.Handled = true;
            }
        }

        private void textBox_MaxEpoch5gb_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number))
            {
                e.Handled = true;
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox_MaxEpoch3gb.Text, out int KawpowLiteMaxEpoch3GB);
            ConfigManager.GeneralConfig.KawpowLiteMaxEpoch3GB = KawpowLiteMaxEpoch3GB;
            int.TryParse(textBox_MaxEpoch4gb.Text, out int KawpowLiteMaxEpoch4GB);
            ConfigManager.GeneralConfig.KawpowLiteMaxEpoch4GB = KawpowLiteMaxEpoch4GB;
            int.TryParse(textBox_MaxEpoch5gb.Text, out int KawpowLiteMaxEpoch5GB);
            ConfigManager.GeneralConfig.KawpowLiteMaxEpoch5GB = KawpowLiteMaxEpoch5GB;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
