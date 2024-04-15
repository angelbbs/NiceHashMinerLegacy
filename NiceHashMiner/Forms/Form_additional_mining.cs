using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
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
    public partial class Form_additional_mining : Form
    {
        public Form_additional_mining()
        {
            InitializeComponent();
            this.TopMost = true;
            this.Text = International.GetText("Form_Settings_button_ZIL_additional_mining");

            Form_Main.ZIL_mining_state = ConfigManager.GeneralConfig.ZIL_mining_state;
            switch (Form_Main.ZIL_mining_state)
            {
                case 0:
                    radioButton1.Checked = true; break;
                case 1:
                    radioButton2.Checked = true; break;
                case 2:
                    radioButton3.Checked = true; break;
                default:
                    radioButton1.Checked = true; break;
            }

            groupBox6.Text = International.GetText("Form_Settings_groupBox_ZIL_mining");
            radioButton1.Text = International.GetText("Form_Settings_radioButton1_ZIL_additional_mining");
            radioButton2.Text = International.GetText("Form_Settings_radioButton2_ZIL_additional_mining");
            radioButton3.Text = International.GetText("Form_Settings_radioButton3_ZIL_additional_mining");
            button_Save.Text = International.GetText("Form_Settings_buttonSaveAPI");
            button_Cancel.Text = International.GetText("Form_Settings_buttonCancelAPI");

            textBox_Pool.Text = ConfigManager.GeneralConfig.ZIL_mining_pool;
            textBox_Port.Text = ConfigManager.GeneralConfig.ZIL_mining_port;
            textBox_Wallet.Text = ConfigManager.GeneralConfig.ZIL_mining_wallet;
            //checkBox_ZIL_Mining_Enable.Text = International.GetText("Form_Settings_checkBox_ZIL_Mining_Enable");

            if (ConfigManager.GeneralConfig.ZIL_mining_state == 0)
            {
                TabControlZILadditionalMining.Enabled = false;
            } else
            {
                TabControlZILadditionalMining.Enabled = true;
            }

            if (ConfigManager.GeneralConfig.ZIL_mining_state == 2)
            {
                label_Pool.Enabled = true;
                textBox_Pool.Enabled = true;
                labelPort.Enabled = true;
                textBox_Port.Enabled = true;
                labelWallet.Enabled = true;
                textBox_Wallet.Enabled = true;
            }
            else
            {
                label_Pool.Enabled = false;
                textBox_Pool.Enabled = false;
                labelPort.Enabled = false;
                textBox_Port.Enabled = false;
                labelWallet.Enabled = false;
                textBox_Wallet.Enabled = false;
            }

            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                this.BackColor = Form_Main._backColor;
                this.ForeColor = Form_Main._foreColor;
                this.TabControlZILadditionalMining.DisplayStyle = TabStyle.Angled;
                this.TabControlZILadditionalMining.DisplayStyleProvider.Opacity = 0.8F;

                this.TabControlZILadditionalMining.DisplayStyleProvider.TextColor = Color.White;
                this.TabControlZILadditionalMining.DisplayStyleProvider.TextColorDisabled = Color.White;
                this.TabControlZILadditionalMining.DisplayStyleProvider.BorderColor = Color.Transparent;
                this.TabControlZILadditionalMining.DisplayStyleProvider.BorderColorHot = Form_Main._foreColor;

                foreach (var lbl in this.Controls.OfType<Button>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._textColor;
                    lbl.FlatStyle = FlatStyle.Flat;
                    lbl.FlatAppearance.BorderColor = Form_Main._textColor;
                    lbl.FlatAppearance.BorderSize = 1;
                }

                TabControlZILadditionalMining.SelectedTab.BackColor = Form_Main._backColor;

                tabPageGMiner.BackColor = Form_Main._backColor;
                tabPageGMiner.ForeColor = Form_Main._foreColor;
                tabPageSRBMiner.BackColor = Form_Main._backColor;
                tabPageSRBMiner.ForeColor = Form_Main._foreColor;
                tabPageNanominer.BackColor = Form_Main._backColor;
                tabPageNanominer.ForeColor = Form_Main._foreColor;
                tabPageminiZ.BackColor = Form_Main._backColor;
                tabPageminiZ.ForeColor = Form_Main._foreColor;
                tabPageRigel.BackColor = Form_Main._backColor;
                tabPageRigel.ForeColor = Form_Main._foreColor;

                textBox_Pool.BackColor = Form_Main._backColor;
                textBox_Pool.ForeColor = Form_Main._foreColor;
                textBox_Pool.BorderStyle = BorderStyle.FixedSingle;

                textBox_Port.BackColor = Form_Main._backColor;
                textBox_Port.ForeColor = Form_Main._foreColor;
                textBox_Port.BorderStyle = BorderStyle.FixedSingle;

                textBox_Wallet.BackColor = Form_Main._backColor;
                textBox_Wallet.ForeColor = Form_Main._foreColor;
                textBox_Wallet.BorderStyle = BorderStyle.FixedSingle;
            }

            //checkBox_ZIL_Mining_Enable.Checked =
              //  ConfigManager.GeneralConfig.ZIL_Mining_Enable;
            //checkBox_ZIL_Mining_Enable.Enabled = !ConfigManager.GeneralConfig.ForceZIL_Mining_Disable;

            checkBox_GMINER_NVIDIA_Autolykos.Checked =
                ConfigManager.GeneralConfig.ZILConfigGMiner.AutolykosKHeavyHash_NVIDIA;
            checkBox_GMINER_NVIDIA_AutolykosIronFish.Checked =
                ConfigManager.GeneralConfig.ZILConfigGMiner.AutolykosIronFish_NVIDIA;
            checkBox_GMINER_NVIDIA_BeamV3.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.BeamV3_NVIDIA;
            checkBox_GMINER_NVIDIA_CuckooCycle.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.CuckooCycle_NVIDIA;
            checkBox_GMINER_NVIDIA_GrinCuckatoo32.Checked =
                ConfigManager.GeneralConfig.ZILConfigGMiner.GrinCuckatoo32_NVIDIA;
            checkBox_GMINER_NVIDIA_KAWPOW.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_NVIDIA;
            checkBox_GMINER_NVIDIA_KarlsenHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.KarlsenHash_NVIDIA;
            checkBox_GMINER_NVIDIA_IronFish.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.IronFish_NVIDIA;
            checkBox_GMINER_NVIDIA_Octopus.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.Octopus_NVIDIA;
            checkBox_GMINER_NVIDIA_OctopusIronFish.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.OctopusIronFish_NVIDIA;
            checkBox_GMINER_NVIDIA_ZelHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_NVIDIA;
            checkBox_GMINER_NVIDIA_ZHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_NVIDIA;

            checkBox_GMINER_AMD_KAWPOW.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_AMD;
            checkBox_GMINER_AMD_ZelHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_AMD;
            checkBox_GMINER_AMD_ZHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_AMD;
            
            
            checkBox_SRBMINER_AMD_Autolykos.Checked = ConfigManager.GeneralConfig.ZILConfigSRBMiner.Autolykos_AMD;

            checkBox_NANOMINER_AMD_Autolykos.Checked = ConfigManager.GeneralConfig.ZILConfigNanominer.Autolykos_AMD;

            checkBox_Rigel_NVIDIA_KAWPOW.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.KAWPOW_NVIDIA;
            checkBox_Rigel_NVIDIA_Autolykos.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.Autolykos_NVIDIA;
            checkBox_Rigel_NVIDIA_AutolykosKarlsenHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.AutolykosKarlsenHash_NVIDIA;
            checkBox_Rigel_NVIDIA_AutolykosPyrinHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.AutolykosPyrinHash_NVIDIA;
            checkBox_Rigel_NVIDIA_FishHashKarlsenHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.FishHashKarlsenHash_NVIDIA;
            checkBox_Rigel_NVIDIA_FishHashPyrinHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.FishHashPyrinHash_NVIDIA;
            checkBox_Rigel_NVIDIA_DaggerKarlsenHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.DaggerKarlsenHash_NVIDIA;
            checkBox_Rigel_NVIDIA_Nexapow.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.Nexapow_NVIDIA;
            checkBox_Rigel_NVIDIA_FishHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.FishHash_NVIDIA;
            checkBox_Rigel_NVIDIA_PyrinHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.PyrinHash_NVIDIA;
            checkBox_Rigel_NVIDIA_KarlsenHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.KarlsenHash_NVIDIA;
            checkBox_Rigel_NVIDIA_Octopus.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.Octopus_NVIDIA;
            checkBox_Rigel_NVIDIA_OctopusKarlsenHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.OctopusKarlsenHash_NVIDIA;
            checkBox_Rigel_NVIDIA_OctopusPyrinHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.OctopusPyrinHash_NVIDIA;
            checkBox_Rigel_NVIDIA_ETCHashKarlsenHash.Checked = ConfigManager.GeneralConfig.ZILConfigRigel.ETCHashKarlsenHash_NVIDIA;

            checkBox_MINIZ_NVIDIA_BeamV3.Checked = ConfigManager.GeneralConfig.ZILConfigminiZ.BeamV3_NVIDIA;
            checkBox_MINIZ_NVIDIA_Octopus.Checked = ConfigManager.GeneralConfig.ZILConfigminiZ.Octopus_NVIDIA;
            checkBox_MINIZ_NVIDIA_ZelHash.Checked = ConfigManager.GeneralConfig.ZILConfigminiZ.ZelHash_NVIDIA;
            checkBox_MINIZ_NVIDIA_ZHash.Checked = ConfigManager.GeneralConfig.ZILConfigminiZ.ZHash_NVIDIA;
            checkBox_MINIZ_AMD_ZelHash.Checked = ConfigManager.GeneralConfig.ZILConfigminiZ.ZelHash_AMD;
            checkBox_MINIZ_AMD_ZHash.Checked = ConfigManager.GeneralConfig.ZILConfigminiZ.ZHash_AMD;

            //до тех пор, пока autolykos не починят
            /*
            checkBox_SRBMINER_AMD_Autolykos.Checked = false;
            checkBox_SRBMINER_AMD_Autolykos.Enabled = false;
            checkBox_SRBMINER_AMD_AutolykosKHeavyHash.Checked = false;
            checkBox_SRBMINER_AMD_AutolykosKHeavyHash.Enabled = false;
            */
            //
            //TabControlZILadditionalMining.TabPages.RemoveByKey("tabPageRigel");
        }

        public static bool isAlgoZIL(string algo, MinerBaseType minerBaseType, DeviceType deviceType)
        {
            if (ConfigManager.GeneralConfig.ZIL_mining_state == 0) return false;
            //if (ConfigManager.GeneralConfig.ZIL_Mining_Enable) return true;

            if (minerBaseType == MinerBaseType.GMiner)
            {
                if (deviceType == DeviceType.NVIDIA)
                {
                    switch (algo)
                    {
                        case "Autolykos":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.Autolykos_NVIDIA;
                        case "AutolykosKHeavyHash":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.AutolykosKHeavyHash_NVIDIA;
                        case "AutolykosIronFish":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.AutolykosIronFish_NVIDIA;
                        case "BeamV3":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.BeamV3_NVIDIA;
                        case "CuckooCycle":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.CuckooCycle_NVIDIA;
                        case "GrinCuckatoo32":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.GrinCuckatoo32_NVIDIA;
                        case "KAWPOW":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_NVIDIA;
                        case "KarlsenHash":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.KarlsenHash_NVIDIA;
                        case "IronFish":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.IronFish_NVIDIA;
                        case "Octopus":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.Octopus_NVIDIA;
                        case "OctopusKHeavyHash":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.OctopusKHeavyHash_NVIDIA;
                        case "OctopusIronFish":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.OctopusIronFish_NVIDIA;
                        case "ZelHash":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_NVIDIA;
                        case "ZHash":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_NVIDIA;
                        default:
                            return false;
                    }
                }
                if (deviceType == DeviceType.AMD)
                {
                    switch (algo)
                    {
                        case "KAWPOW":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_AMD;
                        case "ZelHash":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_AMD;
                        case "ZHash":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_AMD;
                        default:
                            return false;
                    }
                }
            }

            if (minerBaseType == MinerBaseType.SRBMiner)
            {
                if (deviceType == DeviceType.AMD)
                {
                    switch (algo)
                    {
                        case "Autolykos":
                            return ConfigManager.GeneralConfig.ZILConfigSRBMiner.Autolykos_AMD;
                        case "AutolykosKHeavyHash":
                            return ConfigManager.GeneralConfig.ZILConfigSRBMiner.AutolykosKHeavyHash_AMD;
                        case "KHeavyHash":
                            return ConfigManager.GeneralConfig.ZILConfigSRBMiner.KHeavyHash_AMD;
                        default:
                            return false;
                    }
                }
            }

            if (minerBaseType == MinerBaseType.Nanominer)
            {
                if (deviceType == DeviceType.AMD)
                {
                    switch (algo)
                    {
                        case "Autolykos":
                            return ConfigManager.GeneralConfig.ZILConfigNanominer.Autolykos_AMD;
                        default:
                            return false;
                    }
                }
            }
            if (minerBaseType == MinerBaseType.Rigel)
            {
                if (deviceType == DeviceType.NVIDIA)
                {
                    switch (algo)
                    {
                        case "Autolykos":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.Autolykos_NVIDIA;
                        case "KarlsenHash":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.KarlsenHash_NVIDIA;
                        case "FishHashKarlsenHash":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.FishHashKarlsenHash_NVIDIA;
                        case "DaggerKarlsenHash":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.DaggerKarlsenHash_NVIDIA;
                        case "AutolykosKarlsenHash":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.AutolykosKarlsenHash_NVIDIA;
                        case "KAWPOW":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.KAWPOW_NVIDIA;
                        case "NexaPow":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.Nexapow_NVIDIA;
                        case "FishHash":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.FishHash_NVIDIA;
                        case "Octopus":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.Octopus_NVIDIA;
                        case "OctopusKarlsenHash":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.OctopusKarlsenHash_NVIDIA;
                        case "ETCHashKarlsenHash":
                            return ConfigManager.GeneralConfig.ZILConfigRigel.ETCHashKarlsenHash_NVIDIA;
                        default:
                            return false;
                    }
                }
            }
            if (minerBaseType == MinerBaseType.miniZ)
            {
                if (deviceType == DeviceType.NVIDIA)
                {
                    switch (algo)
                    {
                        case "ZelHash":
                            return ConfigManager.GeneralConfig.ZILConfigminiZ.ZelHash_NVIDIA;
                        case "ZHash":
                            return ConfigManager.GeneralConfig.ZILConfigminiZ.ZHash_NVIDIA;
                        case "BeamV3":
                            return ConfigManager.GeneralConfig.ZILConfigminiZ.BeamV3_NVIDIA;
                        case "Octopus":
                            return ConfigManager.GeneralConfig.ZILConfigminiZ.Octopus_NVIDIA;
                        default:
                            return false;
                    }
                }
                if (deviceType == DeviceType.NVIDIA)
                {
                    switch (algo)
                    {
                        case "ZelHash":
                            return ConfigManager.GeneralConfig.ZILConfigminiZ.ZelHash_AMD;
                        case "ZHash":
                            return ConfigManager.GeneralConfig.ZILConfigminiZ.ZHash_AMD;
                        default:
                            return false;
                    }
                }
            }

            return false;
        }
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.ZIL_mining_state = Form_Main.ZIL_mining_state;
            //ConfigManager.GeneralConfig.ZIL_Mining_Enable =
              //  checkBox_ZIL_Mining_Enable.Checked;

            ConfigManager.GeneralConfig.ZILConfigGMiner.Autolykos_NVIDIA =
                checkBox_GMINER_NVIDIA_Autolykos.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.AutolykosIronFish_NVIDIA =
                checkBox_GMINER_NVIDIA_AutolykosIronFish.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.BeamV3_NVIDIA = checkBox_GMINER_NVIDIA_BeamV3.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.CuckooCycle_NVIDIA = checkBox_GMINER_NVIDIA_CuckooCycle.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.GrinCuckatoo32_NVIDIA =
                checkBox_GMINER_NVIDIA_GrinCuckatoo32.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.KarlsenHash_NVIDIA = checkBox_GMINER_NVIDIA_KarlsenHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.IronFish_NVIDIA = checkBox_GMINER_NVIDIA_IronFish.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.Octopus_NVIDIA = checkBox_GMINER_NVIDIA_Octopus.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.OctopusIronFish_NVIDIA = checkBox_GMINER_NVIDIA_OctopusIronFish.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_NVIDIA = checkBox_GMINER_NVIDIA_ZelHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_NVIDIA = checkBox_GMINER_NVIDIA_ZHash.Checked;

            ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_AMD = checkBox_GMINER_AMD_KAWPOW.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_AMD = checkBox_GMINER_AMD_ZelHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_AMD = checkBox_GMINER_AMD_ZHash.Checked;


            ConfigManager.GeneralConfig.ZILConfigSRBMiner.Autolykos_AMD = checkBox_SRBMINER_AMD_Autolykos.Checked;

            ConfigManager.GeneralConfig.ZILConfigNanominer.Autolykos_AMD = checkBox_NANOMINER_AMD_Autolykos.Checked;

            ConfigManager.GeneralConfig.ZILConfigRigel.Autolykos_NVIDIA = checkBox_Rigel_NVIDIA_Autolykos.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.KarlsenHash_NVIDIA = checkBox_Rigel_NVIDIA_KarlsenHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.FishHashKarlsenHash_NVIDIA = checkBox_Rigel_NVIDIA_FishHashKarlsenHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.FishHashPyrinHash_NVIDIA = checkBox_Rigel_NVIDIA_FishHashPyrinHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.DaggerKarlsenHash_NVIDIA = checkBox_Rigel_NVIDIA_DaggerKarlsenHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.AutolykosKarlsenHash_NVIDIA = checkBox_Rigel_NVIDIA_AutolykosKarlsenHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.AutolykosPyrinHash_NVIDIA = checkBox_Rigel_NVIDIA_AutolykosPyrinHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.KAWPOW_NVIDIA = checkBox_Rigel_NVIDIA_KAWPOW.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.Nexapow_NVIDIA = checkBox_Rigel_NVIDIA_Nexapow.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.FishHash_NVIDIA = checkBox_Rigel_NVIDIA_FishHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.PyrinHash_NVIDIA = checkBox_Rigel_NVIDIA_PyrinHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.Octopus_NVIDIA = checkBox_Rigel_NVIDIA_Octopus.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.OctopusKarlsenHash_NVIDIA = checkBox_Rigel_NVIDIA_OctopusKarlsenHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.OctopusPyrinHash_NVIDIA = checkBox_Rigel_NVIDIA_OctopusPyrinHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigRigel.ETCHashKarlsenHash_NVIDIA = checkBox_Rigel_NVIDIA_ETCHashKarlsenHash.Checked;

            ConfigManager.GeneralConfig.ZILConfigminiZ.ZelHash_AMD = checkBox_MINIZ_AMD_ZelHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigminiZ.ZHash_AMD = checkBox_MINIZ_AMD_ZHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigminiZ.ZelHash_NVIDIA = checkBox_MINIZ_NVIDIA_ZelHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigminiZ.ZHash_NVIDIA = checkBox_MINIZ_NVIDIA_ZHash.Checked;

            if (Form_Main.ZIL_mining_state == 2)
            {
                if (string.IsNullOrEmpty(textBox_Pool.Text)) return;
                if (string.IsNullOrEmpty(textBox_Port.Text)) return;
                if (string.IsNullOrEmpty(textBox_Wallet.Text)) return;
                ConfigManager.GeneralConfig.ZIL_mining_pool = textBox_Pool.Text;
                ConfigManager.GeneralConfig.ZIL_mining_port = textBox_Port.Text;
                ConfigManager.GeneralConfig.ZIL_mining_wallet = textBox_Wallet.Text;
            }

            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                Form_Main.ZIL_mining_state = 0;
                TabControlZILadditionalMining.Enabled = false;

                label_Pool.Enabled = false;
                textBox_Pool.Enabled = false;
                labelPort.Enabled = false;
                textBox_Port.Enabled = false;
                labelWallet.Enabled = false;
                textBox_Wallet.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                Form_Main.ZIL_mining_state = 1;
                TabControlZILadditionalMining.Enabled = true;

                label_Pool.Enabled = false;
                textBox_Pool.Enabled = false;
                labelPort.Enabled = false;
                textBox_Port.Enabled = false;
                labelWallet.Enabled = false;
                textBox_Wallet.Enabled = false;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                Form_Main.ZIL_mining_state = 2;
                TabControlZILadditionalMining.Enabled = true;

                label_Pool.Enabled = true;
                textBox_Pool.Enabled = true;
                labelPort.Enabled = true;
                textBox_Port.Enabled = true;
                labelWallet.Enabled = true;
                textBox_Wallet.Enabled = true;
            }
        }

        private void textBox_Port_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number))
            {
                e.Handled = true;
            }
        }
    }
}
