using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Windows.Forms;

namespace NiceHashMiner.Forms
{
    public partial class Form_ChooseLanguage : Form
    {
        public static bool FormMainMoved = false;
        private const string TosText =
         "The MIT License (MIT)\r\n\r\n" +
            "Copyright © 2020 angelbbs\r\n\r\n" +
            "Permission is hereby granted, free of charge, to any person obtaining a copy of this software and " +
            "associated documentation files (the «Software»), to deal in the Software without restriction, " +
            "including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, " +
            "and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, " +
            "subject to the following conditions:\r\n\r\n" +
            "The above copyright notice and this permission notice shall be included in all copies or substantial " +
            "portions of the Software.\r\n\r\n" +
            "THE SOFTWARE IS PROVIDED «AS IS», WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT " +
            "LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. " +
            "IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, " +
            "WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH " +
            "THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";
        private const string TosTextRU =
            "Лицензия MIT\r\n\r\n" +
            "Copyright © 2020 angelbbs\r\n\r\n" +
            "Данная лицензия разрешает лицам, получившим копию данного программного обеспечения и сопутствующей " +
            "документации (в дальнейшем именуемыми «Программное Обеспечение»), безвозмездно использовать " +
            "Программное Обеспечение без ограничений, включая неограниченное право на использование, копирование, " +
            "изменение, слияние, публикацию, распространение, сублицензирование и/или продажу копий Программного " +
            "Обеспечения, а также лицам, которым предоставляется данное Программное Обеспечение, при соблюдении " +
            "следующих условий:\r\n\r\n" +
            "Указанное выше уведомление об авторском праве и данные условия должны быть включены во все копии или " +
            "значимые части данного Программного Обеспечения.\r\n\r\n" +
            "ДАННОЕ ПРОГРАММНОЕ ОБЕСПЕЧЕНИЕ ПРЕДОСТАВЛЯЕТСЯ «КАК ЕСТЬ», БЕЗ КАКИХ-ЛИБО ГАРАНТИЙ, ЯВНО ВЫРАЖЕННЫХ ИЛИ " +
            "ПОДРАЗУМЕВАЕМЫХ, ВКЛЮЧАЯ ГАРАНТИИ ТОВАРНОЙ ПРИГОДНОСТИ, СООТВЕТСТВИЯ ПО ЕГО КОНКРЕТНОМУ НАЗНАЧЕНИЮ И " +
            "ОТСУТСТВИЯ НАРУШЕНИЙ, НО НЕ ОГРАНИЧИВАЯСЬ ИМИ. НИ В КАКОМ СЛУЧАЕ АВТОРЫ ИЛИ ПРАВООБЛАДАТЕЛИ НЕ НЕСУТ " +
            "ОТВЕТСТВЕННОСТИ ПО КАКИМ-ЛИБО ИСКАМ, ЗА УЩЕРБ ИЛИ ПО ИНЫМ ТРЕБОВАНИЯМ, В ТОМ ЧИСЛЕ, ПРИ ДЕЙСТВИИ " +
            "КОНТРАКТА, ДЕЛИКТЕ ИЛИ ИНОЙ СИТУАЦИИ, ВОЗНИКШИМ ИЗ-ЗА ИСПОЛЬЗОВАНИЯ ПРОГРАММНОГО ОБЕСПЕЧЕНИЯ ИЛИ ИНЫХ " +
            "ДЕЙСТВИЙ С ПРОГРАММНЫМ ОБЕСПЕЧЕНИЕМ.";

        private bool _type;

        public Form_ChooseLanguage(bool type)
        {
            _type = type;
            InitializeComponent();
            if (!type)
            {
                this.Text = "Miner Legacy Fork Fix license";
            }
            checkBox_TOS.Visible = type;
            label_Instruction.Visible = type;
            comboBox_Languages.Visible = type;
            button_OK.Enabled = !type;
            if (!type)
            {
                this.BackColor = Form_Main._backColor;
                this.ForeColor = Form_Main._foreColor;
                button_OK.FlatStyle = FlatStyle.Flat;
                button_OK.FlatAppearance.BorderColor = Form_Main._textColor;
                button_OK.FlatAppearance.BorderSize = 1;
                textBox_TOS.BackColor = Form_Main._backColor;
                textBox_TOS.ForeColor = Form_Main._foreColor;
                button_OK.BackColor = Form_Main._backColor;
                button_OK.ForeColor = Form_Main._foreColor;
            }
            // Add language selections list
            var lang = International.GetAvailableLanguages();
            comboBox_Languages.Items.Clear();
            for (var i = 0; i < lang.Count; i++)
            {
                comboBox_Languages.Items.Add(lang[(LanguageType)i]);
            }

            comboBox_Languages.SelectedIndex = 0;
            comboBox_Languages.Enabled = true;

            //label_Instruction.Location = new Point((this.Width - label_Instruction.Size.Width) / 2, label_Instruction.Location.Y);
            //button_OK.Location = new Point((this.Width - button_OK.Size.Width) / 2, button_OK.Location.Y);
            //comboBox_Languages.Location = new Point((this.Width - comboBox_Languages.Size.Width) / 2, comboBox_Languages.Location.Y);
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                textBox_TOS.Text = TosTextRU;
                comboBox_Languages.SelectedIndex = 1;
            }
            else
            {
                textBox_TOS.Text = TosText;
                comboBox_Languages.SelectedIndex = 0;
            }
            textBox_TOS.ScrollBars = ScrollBars.Vertical;
            if (ConfigManager.GeneralConfig.AlwaysOnTop) this.TopMost = true;
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.Language = (LanguageType)comboBox_Languages.SelectedIndex;
            ConfigManager.GeneralConfigFileCommit();
            Close();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_TOS.Checked)
            {
                ConfigManager.GeneralConfig.agreedWithTOS = Globals.CurrentTosVer;
                comboBox_Languages.Enabled = true;
                button_OK.Enabled = true;
            }
            else
            {
                ConfigManager.GeneralConfig.agreedWithTOS = 0;
                //comboBox_Languages.Enabled = false;
                button_OK.Enabled = false;
            }
        }

        private void comboBox_Languages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Languages.SelectedIndex == 1)
            {
                textBox_TOS.Text = TosTextRU;
            }
            else
            {
                textBox_TOS.Text = TosText;
            }
            textBox_TOS.Update();
        }

        private void Form_ChooseLanguage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!checkBox_TOS.Checked && _type)
            {
                MessageBox.Show("You must accept the Terms Of Use",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void Form_ChooseLanguage_ResizeBegin(object sender, EventArgs e)
        {
            FormMainMoved = true;
        }

        private void Form_ChooseLanguage_ResizeEnd(object sender, EventArgs e)
        {
            FormMainMoved = false;
        }
    }
}
