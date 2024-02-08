using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Stats;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Forms
{
    public partial class Form_API_keys : Form
    {
        public static bool FormMainMoved = false;
        [Serializable]
        public class jsonAPIdata
        {
            public string Warning;
            public string orgId;
            public string apiKey;
            public string apiSecret;
        }
        private static jsonAPIdata _jsonAPIdata = new jsonAPIdata();
        public Form_API_keys()
        {
            InitializeComponent();

            this.BackColor = Form_Main._backColor;
            this.ForeColor = Form_Main._foreColor;
            foreach (var lbl in this.Controls.OfType<Button>())
            {
                lbl.BackColor = Form_Main._backColor;
                lbl.ForeColor = Form_Main._textColor;
                lbl.FlatStyle = FlatStyle.Flat;
                lbl.FlatAppearance.BorderColor = Form_Main._textColor;
                lbl.FlatAppearance.BorderSize = 1;
            }
            foreach (var lbl in this.Controls.OfType<TextBox>())
            {
                lbl.BackColor = Form_Main._backColor;
                lbl.ForeColor = Form_Main._textColor;
                lbl.BorderStyle = BorderStyle.FixedSingle;
            }
            this.buttonSave.Text = International.GetText("Form_Settings_buttonSaveAPI");
            this.buttonCancel.Text = International.GetText("Form_Settings_buttonCancelAPI");
            this.buttonDelete.Text = International.GetText("Form_Settings_buttonDeletelAPI");
            linkLabelGetAPIkey.Text = International.GetText("Form_Settings_Get_API_key");
            linkLabel1.Text = International.GetText("Form_Settings_API_permissions");
            if (GetSavedAPIkeyData())
            {
                textBox_Organization_ID.Text = Form_Main.orgId;
                textBox_APIkey.Text = Form_Main.apiKey;
                textBox_APIsecret.Text = Form_Main.apiSecret;
            }

        }

        public static bool GetSavedAPIkeyData()
        {
            try
            {
                if (File.Exists("configs//apidata.key"))
                {
                    string fileAPIdata = File.ReadAllText("configs//apidata.key");
                    if (!fileAPIdata[0].ToString().Equals("{"))
                    {
                        return false;
                    }
                    jsonAPIdata jsonAPIdata = JsonConvert.DeserializeObject<jsonAPIdata>(fileAPIdata);

                    Form_Main.orgId = jsonAPIdata.orgId;
                    Form_Main.apiKey = jsonAPIdata.apiKey;
                    Form_Main.apiSecret = jsonAPIdata.apiSecret;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetSavedAPIkeyData", ex.ToString());
                return false;
            }
            return false;
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_API_keys_ResizeBegin(object sender, EventArgs e)
        {
            FormMainMoved = true;
        }

        private void Form_API_keys_ResizeEnd(object sender, EventArgs e)
        {
            FormMainMoved = false;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox_Organization_ID.Text) || string.IsNullOrEmpty(textBox_APIkey.Text) || string.IsNullOrEmpty(textBox_APIsecret.Text))
            {
                MessageBox.Show(International.GetText("Form_Settings_errorAPI_notfilled"), International.GetText("Error_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else
            {
                Form_Main.orgId = textBox_Organization_ID.Text;
                Form_Main.apiKey = textBox_APIkey.Text;
                Form_Main.apiSecret = textBox_APIsecret.Text;
                Form_Main.API_key_validity = true;
                _jsonAPIdata.Warning = "Do not share this file!";
                _jsonAPIdata.orgId = textBox_Organization_ID.Text;
                _jsonAPIdata.apiKey = textBox_APIkey.Text;
                _jsonAPIdata.apiSecret = textBox_APIsecret.Text;
                WriteAllTextWithBackup("configs/apidata.key", JsonConvert.SerializeObject(_jsonAPIdata, Formatting.Indented));
                if (NiceHashStats.GetRigProfitInternalRUN())
                {
                    MessageBox.Show("API key checked and saved into file configs/apidata.key", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                } else
                {
                    MessageBox.Show(Form_Main.errorAPIkeystring, International.GetText("Error_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
        public static void WriteAllTextWithBackup(string FilePath, string contents)
        {
            string path = FilePath;
            var tempPath = FilePath + ".tmp";
            var backup = FilePath + ".backup";
            try
            {
                if (File.Exists(backup))
                    File.Delete(backup);
            }
            catch (Exception ex)
            {
                //Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }
            var data = Encoding.ASCII.GetBytes(contents);
            using (var tempFile = File.Create(tempPath, 4096, FileOptions.WriteThrough))
                tempFile.Write(data, 0, data.Length);

            try
            {
                if (File.Exists(path)) File.Delete(path);
                File.Copy(tempPath, path);
            }
            catch (Exception ex)
            {
                //Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }

            try
            {
                File.Replace(tempPath, path, backup);
            }
            catch (Exception ex)
            {
                //Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }
            
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists("configs/apidata.key"))
                {
                    if (MessageBox.Show("Permanently delete API key?", "OK", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        File.Delete("configs/apidata.key");
                        this.Close();
                        return;
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }
                else
                {
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void linkLabelGetAPIkey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Links.GetAPIkey);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                Process.Start("Help\\API_permissions_ru.png");
            } else
            {
                Process.Start("Help\\API_permissions_en.png");
            }
        }
    }
}
