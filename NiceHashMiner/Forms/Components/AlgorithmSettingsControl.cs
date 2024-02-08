using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Stats;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Windows.Forms;

namespace NiceHashMiner.Forms.Components
{
    public partial class AlgorithmSettingsControl : UserControl, AlgorithmsListView.IAlgorithmsListView
    {
        private const int ENABLED = 0;
        private const int ALGORITHM = 1;
        private const int MINER = 2;
        private const int SPEED = 3;
        //private const int SECSPEED = 4;
        private const int POWER = 4;
        private const int RATIO = 5;
        private const int RATE = 6;
        private ComputeDevice _computeDevice;
        private Algorithm _currentlySelectedAlgorithm;
        private ListViewItem _currentlySelectedLvi;

        // winform crappy event workarond
        private bool _selected = false;

        public AlgorithmSettingsControl()
        {
            InitializeComponent();
            fieldBoxBenchmarkSpeed.SetInputModeDoubleOnly();
            secondaryFieldBoxBenchmarkSpeed.SetInputModeDoubleOnly();
            field_PowerUsage.SetInputModeDoubleOnly();

            // field_PowerUsage.SetOnTextLeave(PowerUsage_Leave);
            field_PowerUsage.SetOnTextChanged(TextChangedPowerUsage);
            fieldBoxBenchmarkSpeed.SetOnTextChanged(TextChangedBenchmarkSpeed);
            secondaryFieldBoxBenchmarkSpeed.SetOnTextChanged(SecondaryTextChangedBenchmarkSpeed);
            richTextBoxExtraLaunchParameters.TextChanged += TextChangedExtraLaunchParameters;
        }

        public void Deselect()
        {
            _selected = false;
            groupBoxSelectedAlgorithmSettings.Text = string.Format(International.GetText("AlgorithmsListView_GroupBox"),
                International.GetText("AlgorithmsListView_GroupBox_NONE"));
            Enabled = false;
            fieldBoxBenchmarkSpeed.EntryText = "";
            secondaryFieldBoxBenchmarkSpeed.EntryText = "";
            field_PowerUsage.EntryText = "";
            richTextBoxExtraLaunchParameters.Text = "";
        }

        public void InitLocale(ToolTip toolTip1)
        {
            field_PowerUsage.InitLocale(toolTip1,
                International.GetText("Form_Settings_Algo_PowerUsage") + ":",
                International.GetText("Form_Settings_ToolTip_PowerUsage"));
            fieldBoxBenchmarkSpeed.InitLocale(toolTip1,
                International.GetText("Form_Settings_Algo_BenchmarkSpeed") + ":",
                International.GetText("Form_Settings_ToolTip_AlgoBenchmarkSpeed"));
            secondaryFieldBoxBenchmarkSpeed.InitLocale(toolTip1,
                International.GetText("Form_Settings_Algo_SecondaryBenchmarkSpeed") + ":",
                International.GetText("Form_Settings_ToolTip_AlgoSecondaryBenchmarkSpeed"));
            groupBoxExtraLaunchParameters.Text = International.GetText("Form_Settings_General_ExtraLaunchParameters");
            toolTip1.SetToolTip(groupBoxExtraLaunchParameters,
                International.GetText("Form_Settings_ToolTip_AlgoExtraLaunchParameters"));
            //  toolTip1.SetToolTip(pictureBox1, International.GetText("Form_Settings_ToolTip_AlgoExtraLaunchParameters"));
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                groupBoxSelectedAlgorithmSettings.Text = "Настройки выбранного алгоритма";
                field_PowerUsage.InitLocale(toolTip1, "Потребляемая мощн. (Вт)", "Потребляемая мощность (Вт)");
            }

            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                groupBoxSelectedAlgorithmSettings.BackColor = Form_Main._backColor;
                groupBoxSelectedAlgorithmSettings.ForeColor = Form_Main._foreColor;

                groupBoxExtraLaunchParameters.BackColor = Form_Main._backColor;
                groupBoxExtraLaunchParameters.ForeColor = Form_Main._foreColor;

                //    pictureBox1.Image = NiceHashMiner.Properties.Resources.info_white_18;
                richTextBoxExtraLaunchParameters.BackColor = Form_Main._backColor;
                richTextBoxExtraLaunchParameters.ForeColor = Form_Main._foreColor;
            }
        }

        private static string ParseStringDefault(string value)
        {
            return value ?? "";
        }

        private static string ParseDoubleDefault(double value)
        {
            return value <= 0 ? "" : value.ToString();
        }

        public void SetCurrentlySelected(ListViewItem lvi, ComputeDevice computeDevice)
        {
            // should not happen ever
            if (lvi == null) return;

            _computeDevice = computeDevice;
            if (lvi.Tag is Algorithm algorithm)
            {
                _selected = true;
                _currentlySelectedAlgorithm = algorithm;
                _currentlySelectedLvi = lvi;
                Enabled = lvi.Checked;

                groupBoxSelectedAlgorithmSettings.Text = string.Format(
                    International.GetText("AlgorithmsListView_GroupBox"),
                    $"{algorithm.AlgorithmName} ({algorithm.MinerBaseTypeName})");
                ;

                field_PowerUsage.EntryText = ParseDoubleDefault(Math.Round(algorithm.PowerUsage, 0));
                fieldBoxBenchmarkSpeed.EntryText = ParseDoubleDefault(algorithm.BenchmarkSpeed);
                richTextBoxExtraLaunchParameters.Text = ParseStringDefault(algorithm.ExtraLaunchParameters);
                if (algorithm is DualAlgorithm dualAlgo)
                {
                    //secondaryFieldBoxBenchmarkSpeed.EntryText = ParseDoubleDefault(dualAlgo.SecondaryBenchmarkSpeed);
                    secondaryFieldBoxBenchmarkSpeed.EntryText = ParseDoubleDefault(algorithm.BenchmarkSecondarySpeed);
                    secondaryFieldBoxBenchmarkSpeed.Enabled = true;
                }
                else
                {
                    secondaryFieldBoxBenchmarkSpeed.EntryText = "";
                    secondaryFieldBoxBenchmarkSpeed.Enabled = false;
                }

                Update();
            }
            else
            {
                // TODO this should not be null
            }
        }

        public void HandleCheck(ListViewItem lvi)
        {
            if (ReferenceEquals(_currentlySelectedLvi, lvi))
            {
                Enabled = lvi.Checked;
            }
        }

        public void ChangeSpeed(ListViewItem lvi)
        {
            if (ReferenceEquals(_currentlySelectedLvi, lvi))
            {
                if (lvi.Tag is Algorithm algorithm)
                {
                    fieldBoxBenchmarkSpeed.EntryText = ParseDoubleDefault(algorithm.BenchmarkSpeed);
                    field_PowerUsage.EntryText = ParseDoubleDefault(Math.Round(algorithm.PowerUsage, 0));
                    if (algorithm is DualAlgorithm dualAlgo)
                    {
                        //secondaryFieldBoxBenchmarkSpeed.EntryText = ParseDoubleDefault(dualAlgo.SecondaryBenchmarkSpeed);
                        secondaryFieldBoxBenchmarkSpeed.EntryText = ParseDoubleDefault(algorithm.BenchmarkSecondarySpeed);
                    }
                    else
                    {
                        secondaryFieldBoxBenchmarkSpeed.EntryText = "";
                    }
                }
            }
        }

        private bool CanEdit()
        {
            return _currentlySelectedAlgorithm != null && _selected;
        }

        #region Callbacks Events

        private void TextChangedBenchmarkSpeed(object sender, EventArgs e)
        {
            if (!CanEdit()) return;
            if (double.TryParse(fieldBoxBenchmarkSpeed.EntryText, out var value))
            {
                _currentlySelectedAlgorithm.BenchmarkSpeed = value;
                //_currentlySelectedAlgorithm.CurPayingRate = value.ToString();
            }
            else
            {
                _currentlySelectedAlgorithm.BenchmarkSpeed = 0;
            }
            UpdateSpeedText();
        }
        private void TextChangedPowerUsage(object sender, EventArgs e)
        {
            if (!CanEdit()) return;
            if (double.TryParse(field_PowerUsage.EntryText, out var value))
            {
                _currentlySelectedAlgorithm.PowerUsage = Math.Round(value, 0);
            }
            else
            {
                _currentlySelectedAlgorithm.PowerUsage = 0;
            }
            UpdateSpeedText();
        }

        private void SecondaryTextChangedBenchmarkSpeed(object sender, EventArgs e)
        {
            if (_currentlySelectedAlgorithm is DualAlgorithm dualAlgo)
            {
                //dualAlgo.SecondaryBenchmarkSpeed = secondaryValue;
                if (double.TryParse(secondaryFieldBoxBenchmarkSpeed.EntryText, out var secondaryValue))
                {
                    _currentlySelectedAlgorithm.BenchmarkSecondarySpeed = secondaryValue;
                }
            }
            else
            {
                _currentlySelectedAlgorithm.BenchmarkSecondarySpeed = 0;
            }
            UpdateSpeedText();
        }

        private void UpdateSpeedText()
        {
            var speed = _currentlySelectedAlgorithm.BenchmarkSpeed;
            //var secondarySpeed = (_currentlySelectedAlgorithm is DualAlgorithm dualAlgo) ? dualAlgo.SecondaryBenchmarkSpeed : 0;
            var secondarySpeed = (_currentlySelectedAlgorithm is DualAlgorithm dualAlgo) ? _currentlySelectedAlgorithm.BenchmarkSecondarySpeed : 0;
            var speedString = Helpers.FormatDualSpeedOutput(_currentlySelectedAlgorithm.BenchmarkSpeed, secondarySpeed, 0, _currentlySelectedAlgorithm.NiceHashID, _currentlySelectedAlgorithm.DualNiceHashID);
            speedString = speedString.Replace("--", "");
            AlgorithmType algo = AlgorithmType.NONE;
            /*
            if (_currentlySelectedAlgorithm.DualNiceHashID == AlgorithmType.DaggerEaglesong)
            {
                algo = AlgorithmType.Eaglesong;
            }
            if (_currentlySelectedAlgorithm.DualNiceHashID == AlgorithmType.DaggerHandshake)
            {
                algo = AlgorithmType.Handshake;
            }
            */
            NHSmaData.TryGetPaying(algo, out var payingSec);
            NHSmaData.TryGetPaying(_currentlySelectedAlgorithm.NiceHashID, out var paying);

            var payingRate = speed * paying * 0.000000001;
            var payingRateSec = secondarySpeed * payingSec * 0.000000001;
            var rate = (payingRate + payingRateSec).ToString("F8");


            var WithPowerRate = payingRate + payingRateSec - ExchangeRateApi.GetKwhPriceInBtc() * _currentlySelectedAlgorithm.PowerUsage * 24 * Form_Main._factorTimeUnit / 1000;
            if (ConfigManager.GeneralConfig.DecreasePowerCost)
            {
                rate = WithPowerRate.ToString("F8");
            }

            // update lvi speed
            if (_currentlySelectedLvi != null)
            {
                //_currentlySelectedLvi.SubItems[SPEED].Text = speedString;
                //_currentlySelectedLvi.SubItems[RATE].Text = rate;
                if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
                {
                    _currentlySelectedLvi.SubItems[POWER].Text = _currentlySelectedAlgorithm.PowerUsage.ToString() + " Вт";
                }
                else
                {
                    _currentlySelectedLvi.SubItems[POWER].Text = _currentlySelectedAlgorithm.PowerUsage.ToString() + " W";
                }
            }
        }

        private void PowerUsage_Leave(object sender, EventArgs e)
        {
            if (!CanEdit()) return;

            if (double.TryParse(field_PowerUsage.EntryText, out var value))
            {
                _currentlySelectedAlgorithm.PowerUsage = value;
            }
        }

        private void TextChangedExtraLaunchParameters(object sender, EventArgs e)
        {
            if (!CanEdit()) return;
            var extraLaunchParams = richTextBoxExtraLaunchParameters.Text.Replace("\r\n", " ");
            extraLaunchParams = extraLaunchParams.Replace("\n", " ");
            _currentlySelectedAlgorithm.ExtraLaunchParameters = extraLaunchParams;
        }

        private void groupBoxSelectedAlgorithmSettings_Resize(object sender, EventArgs e)
        {
            //Компьютер\HKEY_CURRENT_USER\Control Panel\Desktop\LogPixels
            groupBoxExtraLaunchParameters.Width = groupBoxSelectedAlgorithmSettings.Width - 14;
            richTextBoxExtraLaunchParameters.Width = groupBoxSelectedAlgorithmSettings.Width - 26;
        }

        #endregion

        //private void buttonBenchmark_Click(object sender, EventArgs e) {
        //    var device = new List<ComputeDevice>();
        //    device.Add(_computeDevice);
        //    var BenchmarkForm = new Form_Benchmark(
        //                BenchmarkPerformanceType.Standard,
        //                false, _currentlySelectedAlgorithm.NiceHashID);
        //    BenchmarkForm.ShowDialog();
        //    fieldBoxBenchmarkSpeed.EntryText = _currentlySelectedAlgorithm.BenchmarkSpeed.ToString();
        //    // update lvi speed
        //    if (_currentlySelectedLvi != null) {
        //        _currentlySelectedLvi.SubItems[2].Text = Helpers.FormatSpeedOutput(_currentlySelectedAlgorithm.BenchmarkSpeed);
        //    }
        //}
    }
}
