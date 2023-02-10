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
            this.Text = International.GetText("Form_Settings_button_ZIL_additional_mining");

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
            }

            checkBox_GMINER_NVIDIA_Autolykos.Checked =
                ConfigManager.GeneralConfig.ZILConfigGMiner.Autolykos_NVIDIA;
            checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.Checked =
                ConfigManager.GeneralConfig.ZILConfigGMiner.AutolykosKHeavyHash_NVIDIA;
            checkBox_GMINER_NVIDIA_BeamV3.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.BeamV3_NVIDIA;
            checkBox_GMINER_NVIDIA_CuckooCycle.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.CuckooCycle_NVIDIA;
            checkBox_GMINER_NVIDIA_GrinCuckatoo32.Checked =
                ConfigManager.GeneralConfig.ZILConfigGMiner.GrinCuckatoo32_NVIDIA;
            checkBox_GMINER_NVIDIA_KAWPOW.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_NVIDIA;
            checkBox_GMINER_NVIDIA_KHeavyHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.KHeavyHash_NVIDIA;
            checkBox_GMINER_NVIDIA_Octopus.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.Octopus_NVIDIA;
            checkBox_GMINER_NVIDIA_ZelHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_NVIDIA;
            checkBox_GMINER_NVIDIA_ZHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_NVIDIA;

            checkBox_GMINER_AMD_KAWPOW.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_AMD;
            checkBox_GMINER_AMD_ZelHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_AMD;
            checkBox_GMINER_AMD_ZHash.Checked = ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_AMD;
            
            
            checkBox_SRBMINER_AMD_Autolykos.Checked = ConfigManager.GeneralConfig.ZILConfigSRBMiner.Autolykos_AMD;
            checkBox_SRBMINER_AMD_AutolykosKHeavyHash.Checked = ConfigManager.GeneralConfig.ZILConfigSRBMiner.AutolykosKHeavyHash_AMD;
            checkBox_SRBMINER_AMD_KHeavyHash.Checked = ConfigManager.GeneralConfig.ZILConfigSRBMiner.KHeavyHash_AMD;


            checkBox_NANOMINER_AMD_Autolykos.Checked = ConfigManager.GeneralConfig.ZILConfigNanominer.Autolykos_AMD;
        }

        public static bool isAlgoZIL(string algo, MinerBaseType minerBaseType, DeviceType deviceType)
        {
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
                        case "BeamV3":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.BeamV3_NVIDIA;
                        case "CuckooCycle":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.CuckooCycle_NVIDIA;
                        case "GrinCuckatoo32":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.GrinCuckatoo32_NVIDIA;
                        case "KAWPOW":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_NVIDIA;
                        case "KHeavyHash":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.KHeavyHash_NVIDIA;
                        case "Octopus":
                            return ConfigManager.GeneralConfig.ZILConfigGMiner.Octopus_NVIDIA;
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

            return false;
        }
        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.ZILConfigGMiner.Autolykos_NVIDIA =
                checkBox_GMINER_NVIDIA_Autolykos.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.AutolykosKHeavyHash_NVIDIA =
                checkBox_GMINER_NVIDIA_AutolykosKHeavyHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.BeamV3_NVIDIA = checkBox_GMINER_NVIDIA_BeamV3.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.CuckooCycle_NVIDIA = checkBox_GMINER_NVIDIA_CuckooCycle.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.GrinCuckatoo32_NVIDIA =
                checkBox_GMINER_NVIDIA_GrinCuckatoo32.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_NVIDIA = checkBox_GMINER_NVIDIA_KAWPOW.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.KHeavyHash_NVIDIA = checkBox_GMINER_NVIDIA_KHeavyHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.Octopus_NVIDIA = checkBox_GMINER_NVIDIA_Octopus.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_NVIDIA = checkBox_GMINER_NVIDIA_ZelHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_NVIDIA = checkBox_GMINER_NVIDIA_ZHash.Checked;

            ConfigManager.GeneralConfig.ZILConfigGMiner.KAWPOW_AMD = checkBox_GMINER_AMD_KAWPOW.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.ZelHash_AMD = checkBox_GMINER_AMD_ZelHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigGMiner.ZHash_AMD = checkBox_GMINER_AMD_ZHash.Checked;


            ConfigManager.GeneralConfig.ZILConfigSRBMiner.Autolykos_AMD = checkBox_SRBMINER_AMD_Autolykos.Checked;
            ConfigManager.GeneralConfig.ZILConfigSRBMiner.AutolykosKHeavyHash_AMD =
                checkBox_SRBMINER_AMD_AutolykosKHeavyHash.Checked;
            ConfigManager.GeneralConfig.ZILConfigSRBMiner.KHeavyHash_AMD = checkBox_SRBMINER_AMD_KHeavyHash.Checked;

            ConfigManager.GeneralConfig.ZILConfigNanominer.Autolykos_AMD = checkBox_NANOMINER_AMD_Autolykos.Checked;

            this.Close();
        }
    }
}
