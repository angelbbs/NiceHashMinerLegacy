using Newtonsoft.Json;
using NiceHashMiner.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Forms
{
    public partial class FormAddProfile : Form
    {
        private int desiredStartLocationX;
        private int desiredStartLocationY;
        public FormAddProfile(int x, int y) 
        {
            InitializeComponent();
            this.Text = International.GetText("Form_Settings_Adding_new_profile");
            label_NewProfileName.Text = International.GetText("Form_Settings_Enter_profile_name");
            button_Save.Text = International.GetText("Button_Save");
            button_Cancel.Text = International.GetText("Button_Cancel");
            this.BackColor = Form_Main._backColor;
            this.ForeColor = Form_Main._textColor;
            button_Save.BackColor = Form_Main._backColor;
            button_Save.ForeColor = Form_Main._textColor;
            button_Save.Enabled = false;
            button_Cancel.BackColor = Form_Main._backColor;
            button_Cancel.ForeColor = Form_Main._textColor;
            label_NewProfileName.BackColor = Form_Main._backColor;
            label_NewProfileName.ForeColor = Form_Main._textColor;
            textBox_NewProfileName.BackColor = Form_Main._backColor;
            textBox_NewProfileName.ForeColor = Form_Main._textColor;
            textBox_NewProfileName.BorderStyle = BorderStyle.FixedSingle;

            this.desiredStartLocationX = x;
            this.desiredStartLocationY = y;
            Load += new EventHandler(Form2_Load);
        }
        private void Form2_Load(object sender, System.EventArgs e)
        {
            this.SetDesktopLocation(desiredStartLocationX, desiredStartLocationY);
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            if (Profiles.Profile.CheckExistProfile(textBox_NewProfileName.Text))
            {
                MessageBox.Show(string.Format(International.GetText("Form_Settings_ProfileExist"),
                textBox_NewProfileName.Text), "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Profiles.Profile.SaveProfile(textBox_NewProfileName.Text, ConfigManager.GeneralConfig.ProfileName);
            }
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox_NewProfileName_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_NewProfileName.Text))
            {
                button_Save.Enabled = false;
            } else
            {
                button_Save.Enabled = true;
            }
        }
    }
}
