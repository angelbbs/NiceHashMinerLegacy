using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Interfaces;
using NiceHashMiner.Miners;
using NiceHashMiner.Stats;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NiceHashMiner.Forms.Components
{
    public partial class AlgorithmsListView : UserControl
    {
        private const int ENABLED = 0;
        private const int ALGORITHM = 1;
        private const int MINER = 2;
        private const int SPEED = 3;
        //private const int SECSPEED = 4;
        private const int POWER = 4;
        private const int RATIO = 5;
        private const int RATE = 6;
        public static bool isListViewEnabled = true;
        private static ulong minMem = (ulong)(1024 * 1024 * 8);
        public interface IAlgorithmsListView
        {
            void SetCurrentlySelected(ListViewItem lvi, ComputeDevice computeDevice);
            void HandleCheck(ListViewItem lvi);
            void ChangeSpeed(ListViewItem lvi);
        }

        public IAlgorithmsListView ComunicationInterface { get; set; }

        public IBenchmarkCalculation BenchmarkCalculation { get; set; }

        internal static ComputeDevice _computeDevice;

        private class DefaultAlgorithmColorSeter : IListItemCheckColorSetter
        {
            //private static readonly Color DisabledColor = Color.FromArgb(Form_Main._backColor.ToArgb() + 40 * 256 * 256 * 256 + 40 * 256 * 256 + 40 * 256 + 40);
            //public static Color DisabledColor = ConfigManager.GeneralConfig.ColorProfileIndex != 0 ? Color.FromArgb(Form_Main._backColor.ToArgb() + 40 * 256 * 256 * 256 + 40 * 256 * 256 + 40 * 256 + 40) : Color.DarkGray;
            public static Color DisabledColor = Form_Main._backColor;
            public static Color DisabledForeColor = Color.Gray;
            //  private static readonly Color DisabledColor = Form_Main._backColor;
            private static readonly Color BenchmarkedColor = Form_Main._backColor;
            private static readonly Color UnbenchmarkedColor = Color.LightBlue;

            public void LviSetColor(ListViewItem lvi)
            {
                int yellowInc = 0;
                int greenInc = 0;

                if (Form_Main._windowColor.B < 127)
                {
                    yellowInc = 60;
                }
                else
                {
                    yellowInc = -30;
                }
                if (Form_Main._windowColor.G < 127)
                {
                    greenInc = 10;
                }
                else
                {
                    greenInc = -5;
                }
                Color _DefaultBackColorHighlight = Color.FromArgb(Form_Main._windowColor.A, Form_Main._windowColor.R,
                    Form_Main._windowColor.G + greenInc, Form_Main._windowColor.B + yellowInc);

                if (!isListViewEnabled)
                {
                    //  return;
                }
                if (lvi.Tag is Algorithm algorithm)
                {
                    if (!algorithm.Enabled && !algorithm.IsBenchmarkPending)
                    {
                        if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
                        {
                            lvi.BackColor = DisabledColor;
                        }
                        else
                        {
                            lvi.BackColor = SystemColors.ControlLightLight;
                        }
                        lvi.ForeColor = DisabledForeColor;
                        if (ConfigManager.GeneralConfig.ColorizeTables)
                        {
                            if (lvi.Index % 2 == 1)
                            {
                                lvi.BackColor = _DefaultBackColorHighlight;
                            }
                        }
                    }
                    else if (!algorithm.BenchmarkNeeded && !algorithm.IsBenchmarkPending)
                    {
                        lvi.BackColor = BenchmarkedColor;
                        if (isListViewEnabled)
                        {
                            lvi.ForeColor = Form_Main._foreColor;
                            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
                            {
                                lvi.BackColor = DisabledColor;
                            }
                            else
                            {
                                lvi.BackColor = SystemColors.ControlLightLight;
                            }
                            if (ConfigManager.GeneralConfig.ColorizeTables)
                            {
                                if (lvi.Index % 2 == 1)
                                {
                                    lvi.BackColor = _DefaultBackColorHighlight;
                                }
                            }
                        }
                        else
                        {
                            lvi.ForeColor = DisabledForeColor;
                            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
                            {
                                lvi.BackColor = DisabledColor;
                            }
                            else
                            {
                                lvi.BackColor = SystemColors.ControlLightLight;
                            }
                            if (ConfigManager.GeneralConfig.ColorizeTables)
                            {
                                if (lvi.Index % 2 == 1)
                                {
                                    lvi.BackColor = _DefaultBackColorHighlight;
                                }
                            }
                        }
                    }
                    else
                    {
                        lvi.BackColor = UnbenchmarkedColor;
                    }

                }
            }
        }

        private readonly IListItemCheckColorSetter _listItemCheckColorSetter = new DefaultAlgorithmColorSeter();

        // disable checkboxes when in benchmark mode
        private bool _isInBenchmark = false;

        // helper for benchmarking logic
        public bool IsInBenchmark
        {
            get => _isInBenchmark;
            set
            {
                if (value)
                {
                    _isInBenchmark = true;
                    listViewAlgorithms.CheckBoxes = false;
                }
                else
                {
                    _isInBenchmark = false;
                    listViewAlgorithms.CheckBoxes = true;
                }
            }
        }

        public AlgorithmsListView()
        {
            InitializeComponent();
            listViewAlgorithms.DoubleBuffer();
            AlgorithmsListView.colorListViewHeader(ref listViewAlgorithms, Form_Main._backColor, Form_Main._textColor);


            // callback initializations
            listViewAlgorithms.ItemSelectionChanged += ListViewAlgorithms_ItemSelectionChanged;
            listViewAlgorithms.ItemChecked += (ItemCheckedEventHandler)ListViewAlgorithms_ItemChecked;
            listViewAlgorithms.MultiSelect = true;
            listViewAlgorithms.FullRowSelect = true;

            IsInBenchmark = false;
            //ComCtrlExtensions.WindowExplorerTheme(listViewAlgorithms, true);
        }
        public static void colorListViewHeader(ref ListView list, Color backColor, Color foreColor)
        {
            list.OwnerDraw = true;
            list.DrawColumnHeader +=
            new DrawListViewColumnHeaderEventHandler
            (
            (sender, e) => headerDraw(sender, e, backColor, foreColor)
            );
            list.DrawItem += new DrawListViewItemEventHandler(bodyDraw);

        }
        private static void headerDraw(object sender, DrawListViewColumnHeaderEventArgs e, Color backColor, Color foreColor)
        {
            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }


            using (SolidBrush foreBrush = new SolidBrush(foreColor))
            {
                StringFormat sf = new StringFormat();
                if ((e.ColumnIndex == 0))
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Near;
                }
                else
                {
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                }
                e.Graphics.DrawString(e.Header.Text, e.Font, foreBrush, e.Bounds, sf);
            }
        }

        private static void bodyDraw(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                using (SolidBrush backBrush = new SolidBrush(Form_Main._backColor))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }
            }
            else
            {
                using (SolidBrush backBrush = new SolidBrush(SystemColors.ControlLightLight))
                {
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                }
            }

        }
        public void InitLocale()
        {
            var _backColor = Form_Main._backColor;
            var _foreColor = Form_Main._foreColor;
            var _textColor = Form_Main._textColor;

            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                listViewAlgorithms.BackColor = _backColor;
                listViewAlgorithms.ForeColor = _textColor;
                this.BackColor = _backColor;
            }
            else
            {
                listViewAlgorithms.BackColor = SystemColors.ControlLightLight;
                listViewAlgorithms.ForeColor = _textColor;
                this.BackColor = SystemColors.ControlLightLight;
            }

            listViewAlgorithms.Columns[ENABLED].Text = International.GetText("AlgorithmsListView_Enabled");
            listViewAlgorithms.Columns[ALGORITHM].Text = International.GetText("AlgorithmsListView_Algorithm");
            listViewAlgorithms.Columns[MINER].Text = International.GetText("AlgorithmsListView_Miner");
            listViewAlgorithms.Columns[SPEED].Text = International.GetText("AlgorithmsListView_Speed");
            //listViewAlgorithms.Columns[SECSPEED].Text = International.GetText("Form_DcriValues_SecondarySpeed");
            listViewAlgorithms.Columns[POWER].Text = International.GetText("AlgorithmsListView_Power");

            listViewAlgorithms.Columns[RATIO].Text = International.GetText("AlgorithmsListView_Ratio");
            listViewAlgorithms.Columns[RATE].Text = International.GetText("AlgorithmsListView_Rate");
            //listViewAlgorithms.Columns[RATE].Width = 0;
            listViewAlgorithms.Columns[ALGORITHM].Width = ConfigManager.GeneralConfig.ColumnListALGORITHM;
            listViewAlgorithms.Columns[MINER].Width = ConfigManager.GeneralConfig.ColumnListMINER;
            listViewAlgorithms.Columns[SPEED].Width = ConfigManager.GeneralConfig.ColumnListSPEED;
            listViewAlgorithms.Columns[POWER].Width = ConfigManager.GeneralConfig.ColumnListPOWER;
            listViewAlgorithms.Columns[RATIO].Width = ConfigManager.GeneralConfig.ColumnListRATIO;
            listViewAlgorithms.Columns[RATE].Width = ConfigManager.GeneralConfig.ColumnListRATE;
        }

        public void SetAlgorithms(ComputeDevice computeDevice, bool isEnabled, bool fromBenchmark = false)
        {
            bool hideUnused = ConfigManager.GeneralConfig.Hide_unused_algorithms;
            _computeDevice = computeDevice;

            listViewAlgorithms.BeginUpdate();
            listViewAlgorithms.Items.Clear();

            Font fontRegular = new Font(this.Font, FontStyle.Regular);
            Font fontBold = new Font(this.Font, FontStyle.Bold);
            foreach (var alg in computeDevice.GetAlgorithmSettings())
            {
                if (hideUnused && !alg.Enabled)
                {
                    continue;
                }
                var name = "";
                var miner = "";
                var secondarySpeed = "";
                var totalSpeed = "";
                var payingRatio = "";

                if (alg is DualAlgorithm dualAlg)
                {
                    name = dualAlg.DualAlgorithmNameCustom;
                    miner = alg.MinerBaseTypeName;
                    secondarySpeed = dualAlg.SecondaryBenchmarkSpeedString();
                    totalSpeed = alg.BenchmarkSpeedString() + "/ " + secondarySpeed;
                    payingRatio = alg.CurPayingRatio + "/" + alg.CurSecondPayingRatio;
                }
                else
                {
                    name = alg.AlgorithmNameCustom;
                    miner = alg.MinerBaseTypeName;
                    totalSpeed = alg.BenchmarkSpeedString();
                    payingRatio = alg.CurPayingRatio;
                }
                if (Form_additional_mining.isAlgoZIL(name, alg.MinerBaseType, computeDevice.DeviceType) &&
                        ConfigManager.GeneralConfig.AdditionalMiningPlusSymbol)
                {
                    //if (miner.ToLower().Contains("miniz") && ConfigManager.GeneralConfig.ZIL_mining_state == 1)
                    //{

                    //} else
                    {
                        name = name + "+";
                    }
                }
                if (miner.ToLower().Contains("nbminer") && name.ToLower().Contains("beam"))
                {
                    miner = miner + MinerVersion.GetMinerVersion("nbminer.39.5");
                }
                else if (miner.ToLower().Contains("nbminer") && name.ToLower().Contains("kawpow"))
                {
                    miner = miner + MinerVersion.GetMinerVersion("nbminer.39.5");
                }
                else if (miner.ToLower().Contains("nbminer") && name.ToLower().Contains("ergo"))
                {
                    miner = miner + MinerVersion.GetMinerVersion("nbminer.39.5");
                }
                else
                {
                    miner = miner + MinerVersion.GetMinerVersion(miner);
                }

                if (!alg.Hidden)
                {
                    var lvi = new ListViewItem();
                    lvi.SubItems.Add(name);
                    lvi.SubItems.Add(miner);
                    lvi.SubItems.Add(totalSpeed);
                    if (alg.PowerUsage <= 0)
                    {
                        lvi.SubItems.Add("--");
                    }
                    else
                    {
                        alg.PowerUsage = Math.Round(alg.PowerUsage, 0);
                        if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
                        {
                            lvi.SubItems.Add(alg.PowerUsage.ToString() + " Вт");
                        }
                        else
                        {
                            lvi.SubItems.Add(alg.PowerUsage.ToString() + " W");
                        }
                    }
                    double.TryParse(alg.CurPayingRate, out var valueRate);
                    double.TryParse(alg.CurSecondPayingRate, out var valueRateSecond);

                    if (Form_additional_mining.isAlgoZIL(alg.AlgorithmName, alg.MinerBaseType, computeDevice.DeviceType))
                    {
                        //if (miner.ToLower().Contains("miniz") && ConfigManager.GeneralConfig.ZIL_mining_state == 1)
                        //{

                        //}
                        //else
                        {
                            valueRate += valueRate * Form_Main.ZilFactor;
                        }
                    }

                    double WithPowerRate = 0;
                    WithPowerRate = (valueRate + valueRateSecond) - ExchangeRateApi.GetKwhPriceInBtc() * alg.PowerUsage * 24 * Form_Main._factorTimeUnit / 1000;

                    if (!ConfigManager.GeneralConfig.DecreasePowerCost)
                    {
                        WithPowerRate = valueRate + valueRateSecond;
                    }
                    string rateCurrencyString = ExchangeRateApi
                             .ConvertToActiveCurrency((WithPowerRate) * ExchangeRateApi.GetUsdExchangeRate() * Form_Main._factorTimeUnit)
                             .ToString("F2", CultureInfo.InvariantCulture);
                    string fiatCurrencyName = $"{ExchangeRateApi.ActiveDisplayCurrency}/" +
                         International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
                    string btcCurrencyName = "BTC/" +
                         International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());

                    lvi.SubItems.Add(payingRatio);
                    if (ConfigManager.GeneralConfig.DecreasePowerCost)
                    {
                        if (ConfigManager.GeneralConfig.FiatCurrency)
                        {
                            columnHeader6.Text = fiatCurrencyName;
                            lvi.SubItems.Add(rateCurrencyString);
                        }
                        else
                        {
                            columnHeader6.Text = btcCurrencyName;
                            lvi.SubItems.Add(WithPowerRate.ToString("F8"));
                        }
                    }
                    else
                    {
                        if (ConfigManager.GeneralConfig.FiatCurrency)
                        {
                            columnHeader6.Text = fiatCurrencyName;
                            lvi.SubItems.Add(rateCurrencyString);
                        }
                        else
                        {
                            columnHeader6.Text = btcCurrencyName;
                            lvi.SubItems.Add(WithPowerRate.ToString("F8"));
                        }
                    }
                    lvi.Tag = alg;
                    lvi.Checked = alg.Enabled;

                    try
                    {
                        if (alg.Forced)
                        {
                            lvi.Font = fontBold;
                        }
                        else
                        {
                            lvi.Font = fontRegular;
                        }
                        listViewAlgorithms.Items.Add(lvi);
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("SetAlgorithms", ex.ToString());
                    }

                } else
                {
                    Helpers.ConsolePrint("****lvi", name);
                }
            }
            listViewAlgorithms.EndUpdate();
            isListViewEnabled = isEnabled;
            listViewAlgorithms.CheckBoxes = isEnabled;
        }

        public void UpdateLvi()
        {
            Font fontRegular = new Font(this.Font, FontStyle.Regular);
            Font fontBold = new Font(this.Font, FontStyle.Bold);
            try
            {
                if (_computeDevice != null && listViewAlgorithms.Items != null)
                {
                    foreach (ListViewItem lvi in listViewAlgorithms.Items)
                    {
                        if (lvi != null && lvi.Tag is Algorithm algorithm)
                        {
                            var algo = lvi.Tag as Algorithm;
                            if (algo != null)
                            {
                                try
                                {
                                    if (algo.Forced)
                                    {
                                        lvi.Font = fontBold;
                                    }
                                    else
                                    {
                                        lvi.Font = fontRegular;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Helpers.ConsolePrint("UpdateLvi", ex.ToString());
                                }

                                if (algorithm is DualAlgorithm dualAlg)
                                {

                                    lvi.SubItems[RATIO].Text = algorithm.CurPayingRatio + "/" + algorithm.CurSecondPayingRatio;
                                    if (IsInBenchmark && algorithm.BenchmarkSecondarySpeed == 0)
                                    {
                                        lvi.SubItems[SPEED].Text = algorithm.BenchmarkSpeedString();
                                    }
                                    else
                                    {
                                        lvi.SubItems[SPEED].Text = algorithm.BenchmarkSpeedString() +
                                                                   "/ " + algorithm.SecondaryBenchmarkSpeedString();
                                    }

                                }
                                else
                                {
                                    lvi.SubItems[SPEED].Text = algorithm.BenchmarkSpeedString();
                                    lvi.SubItems[RATIO].Text = algorithm.CurPayingRatio;
                                }
                                double.TryParse(algorithm.CurPayingRate, out var valueRate);
                                double.TryParse(algorithm.CurSecondPayingRate, out var valueRateSecond);

                                if (Form_additional_mining.isAlgoZIL(algo.AlgorithmName, algo.MinerBaseType, _computeDevice.DeviceType))
                                {
                                    valueRate += valueRate * Form_Main.ZilFactor;
                                }
                                double WithPowerRate = 0;
                                WithPowerRate = (valueRate + valueRateSecond) - ExchangeRateApi.GetKwhPriceInBtc() * algorithm.PowerUsage * 24 * Form_Main._factorTimeUnit / 1000;

                                if (!ConfigManager.GeneralConfig.DecreasePowerCost)
                                {
                                    WithPowerRate = valueRate + valueRateSecond;
                                }

                                string rateCurrencyString = ExchangeRateApi
                                         .ConvertToActiveCurrency((WithPowerRate) * ExchangeRateApi.GetUsdExchangeRate() * Form_Main._factorTimeUnit)
                                         .ToString("F2", CultureInfo.InvariantCulture);
                                string fiatCurrencyName = $"{ExchangeRateApi.ActiveDisplayCurrency}/" +
                                     International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());
                                string btcCurrencyName = "BTC/" +
                                     International.GetText(ConfigManager.GeneralConfig.TimeUnit.ToString());

                                if (ConfigManager.GeneralConfig.DecreasePowerCost)
                                {
                                    if (ConfigManager.GeneralConfig.FiatCurrency)
                                    {
                                        columnHeader6.Text = fiatCurrencyName;
                                        lvi.SubItems[RATE].Text = rateCurrencyString;
                                    }
                                    else
                                    {
                                        columnHeader6.Text = btcCurrencyName;
                                        lvi.SubItems[RATE].Text = WithPowerRate.ToString("F8");
                                    }
                                }
                                else
                                {
                                    if (ConfigManager.GeneralConfig.FiatCurrency)
                                    {
                                        columnHeader6.Text = fiatCurrencyName;
                                        lvi.SubItems[RATE].Text = rateCurrencyString;
                                    }
                                    else
                                    {
                                        columnHeader6.Text = btcCurrencyName;
                                        lvi.SubItems[RATE].Text = WithPowerRate.ToString("F8");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("UpdateLvi", er.ToString());
            }
        }
        public void RepaintStatus(bool isEnabled, string uuid)
        {
            if (_computeDevice != null && _computeDevice.Uuid == uuid)
            {
                isListViewEnabled = isEnabled;
                listViewAlgorithms.CheckBoxes = isEnabled;

                foreach (ListViewItem lvi in listViewAlgorithms.Items)
                {
                    var algo = lvi.Tag as Algorithm;
                    if (algo != null)
                    {
                        if (algo is DualAlgorithm dualAlg)
                        {
                            if (IsInBenchmark && algo.BenchmarkSecondarySpeed == 0)
                            {
                                lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString();
                            }
                            else
                            {
                                lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString() +
                                                           "/ " + dualAlg.SecondaryBenchmarkSpeedString();
                            }
                        }
                        else
                        {
                            lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString();
                            if (algo.NiceHashID == AlgorithmType.KAWPOWLite && algo.Enabled)
                            {
                                minMem = Math.Min(minMem, _computeDevice.GpuRam / 1024);
                                if (minMem > (ulong)(1024 * 1024 * 2.7) && minMem < (ulong)(1024 * 1024 * 3.7))
                                {
                                    Form_Main.KawpowLite3GB = true;
                                    Form_Main.KawpowLite4GB = false;
                                    Form_Main.KawpowLite5GB = false;
                                }
                                if (minMem > (ulong)(1024 * 1024 * 3.7) && minMem < (ulong)(1024 * 1024 * 4.7))
                                {
                                    Form_Main.KawpowLite3GB = false;
                                    Form_Main.KawpowLite4GB = true;
                                    Form_Main.KawpowLite5GB = false;
                                }
                                if (minMem > (ulong)(1024 * 1024 * 4.7) && minMem < (ulong)(1024 * 1024 * 5.7))
                                {
                                    Form_Main.KawpowLite3GB = false;
                                    Form_Main.KawpowLite4GB = false;
                                    Form_Main.KawpowLite5GB = true;
                                }
                            }
                        }

                        lvi.Checked = algo.Enabled;

                        algo.PowerUsage = Math.Round(algo.PowerUsage, 0);
                        if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
                        {
                            lvi.SubItems[POWER].Text = algo.PowerUsage.ToString() + " Вт";
                        }
                        else
                        {
                            lvi.SubItems[POWER].Text = algo.PowerUsage.ToString() + " W";
                        }
                        if (algo.PowerUsage <= 0)
                        {
                            lvi.SubItems[POWER].Text = "--";
                        }
                        _listItemCheckColorSetter.LviSetColor(lvi);
                    }
                }
            }
        }

        #region Callbacks Events

        private void ListViewAlgorithms_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ComunicationInterface?.SetCurrentlySelected(e.Item, _computeDevice);
        }

        private void ListViewAlgorithms_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (IsInBenchmark)
            {
                //listViewAlgorithms.CheckBoxes = false;
                // e.Item.Checked = !e.Item.Checked;
                //return;
            }

            if (e.Item.Tag is Algorithm algo)
            {
                algo.Enabled = e.Item.Checked;
                                
                if (Form_Main.KawpowLite && algo.NiceHashID == AlgorithmType.KAWPOWLite && algo.Enabled)
                {
                    Form_Main.KawpowLiteEnabled = true;
                }

                if (!IsInBenchmark)
                {

                }
            }

            ComunicationInterface?.HandleCheck(e.Item);
            var lvi = e.Item;
            _listItemCheckColorSetter.LviSetColor(lvi);
            // update benchmark status data
            BenchmarkCalculation?.CalcBenchmarkDevicesAlgorithmQueue();
        }

        #endregion //Callbacks Events

        public void ResetListItemColors()
        {
            foreach (ListViewItem lvi in listViewAlgorithms.Items)
            {
                _listItemCheckColorSetter?.LviSetColor(lvi);
            }
        }

        // on benchmark
        public void SetSpeedStatus(ComputeDevice computeDevice, Algorithm algorithm, string status)
        {
            if (algorithm != null)
            {
                algorithm.BenchmarkStatus = status;
                // gui update only if same as selected
                if (_computeDevice != null && computeDevice.Uuid == _computeDevice.Uuid)
                {
                    foreach (ListViewItem lvi in listViewAlgorithms.Items)
                    {
                        if (lvi.Tag is Algorithm algo && algo.AlgorithmStringID == algorithm.AlgorithmStringID)
                        {
                            // TODO handle numbers
                            if (algo != null)
                            {
                                //lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString();
                                if (algo is DualAlgorithm dualAlgo && algo.BenchmarkSecondarySpeed > 0)
                                {
                                    lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString() +
                                                               "/ " + algo.SecondaryBenchmarkSpeedString();
                                }
                                else
                                {
                                    lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString();
                                }

                            }

                            double.TryParse(algorithm.CurPayingRate, out var valueRate);

                            if (Form_additional_mining.isAlgoZIL(algo.AlgorithmName, algo.MinerBaseType, computeDevice.DeviceType))
                            {
                                valueRate += valueRate * Form_Main.ZilFactor;
                            }

                            var WithPowerRate = valueRate - ExchangeRateApi.GetKwhPriceInBtc() * algorithm.PowerUsage * 24 * Form_Main._factorTimeUnit / 1000;

                            if (ConfigManager.GeneralConfig.DecreasePowerCost)
                            {
                                //lvi.SubItems[RATE].Text = WithPowerRate.ToString("F8");
                            }
                            else
                            {
                                //lvi.SubItems[RATE].Text = algo.CurPayingRate;
                            }
                            //lvi.SubItems[RATE].Text = algorithm.CurPayingRate;
                            algorithm.PowerUsage = Math.Round(algorithm.PowerUsage, 0);
                            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
                            {
                                lvi.SubItems[POWER].Text = algorithm.PowerUsage.ToString() + " Вт";
                            }
                            else
                            {
                                lvi.SubItems[POWER].Text = algorithm.PowerUsage.ToString() + " W";
                            }
                            if (algorithm.PowerUsage <= 0)
                            {
                                lvi.SubItems[POWER].Text = "--";
                            }

                            if (algorithm is DualAlgorithm dualAlg)
                            {
                                //lvi.SubItems[RATIO].Text = algorithm.CurPayingRatio + "/" + dualAlg.SecondaryCurPayingRatio;
                                lvi.SubItems[RATIO].Text = algorithm.CurPayingRatio + "/" + algorithm.CurSecondPayingRatio;
                            }
                            else
                            {
                                lvi.SubItems[RATIO].Text = algorithm.CurPayingRatio;
                            }

                            _listItemCheckColorSetter.LviSetColor(lvi);
                            break;
                        }
                    }
                }
            }
        }

        private void ListViewAlgorithms_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (!isListViewEnabled)
                {
                    listViewAlgorithms.SelectedItems.Clear();
                    return;
                }
                if (IsInBenchmark) return;
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Items.Clear();
                    Bitmap _EnableBitmap = new Bitmap(Properties.Resources.Ok_normal, 14, 14);
                    Bitmap _DisableBitmap = new Bitmap(Properties.Resources.Delete_normal, 14, 14);
                    // enable all
                    {
                        var enableAllItems = new ToolStripMenuItem
                        {
                            Image = _EnableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = International.GetText("AlgorithmsListView_ContextMenu_EnableAll")
                        };
                        enableAllItems.Click += ToolStripMenuItemEnableAll_Click;
                        contextMenuStrip1.Items.Add(enableAllItems);
                    }
                    // disable all
                    {
                        var disableAllItems = new ToolStripMenuItem
                        {
                            Image = _DisableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = International.GetText("AlgorithmsListView_ContextMenu_DisableAll")
                        };
                        disableAllItems.Click += ToolStripMenuItemDisableAll_Click;
                        contextMenuStrip1.Items.Add(disableAllItems);
                    }
                    // enable selected
                    this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                    {
                        string _text = Text = International.GetText("AlgorithmsListView_ContextMenu_EnableItem") + " " +
                            listViewAlgorithms.SelectedItems[0].SubItems[1].Text + " (" +
                            listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")";

                        if (listViewAlgorithms.SelectedItems.Count > 1)
                        {
                            _text = International.GetText("AlgorithmsListView_ContextMenu_EnableSelected");
                        }

                        var testItem = new ToolStripMenuItem
                        {
                            Image = _EnableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = _text
                        };
                        testItem.Click += ToolStripMenuItemEnableSelected_Click;
                        contextMenuStrip1.Items.Add(testItem);
                    }
                    // disable selected
                    {
                        string _text = Text = International.GetText("AlgorithmsListView_ContextMenu_DisableItem") + " " +
                            listViewAlgorithms.SelectedItems[0].SubItems[1].Text + " (" +
                            listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")";

                        if (listViewAlgorithms.SelectedItems.Count > 1)
                        {
                            _text = International.GetText("AlgorithmsListView_ContextMenu_DisableSelected");
                        }
                        var testItem = new ToolStripMenuItem
                        {
                            Image = _DisableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = _text
                        };
                        testItem.Click += ToolStripMenuItemDisableSelected_Click;
                        contextMenuStrip1.Items.Add(testItem);
                    }
                    this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                    // enable benchmarked only
                    {
                        var enableBenchedItem = new ToolStripMenuItem
                        {
                            Image = _EnableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = International.GetText("AlgorithmsListView_ContextMenu_EnableBenched")
                        };
                        enableBenchedItem.Click += ToolStripMenuItemEnableBenched_Click;
                        contextMenuStrip1.Items.Add(enableBenchedItem);
                    }
                    this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                    // clear item
                    {
                        string _text = International.GetText("AlgorithmsListView_ContextMenu_ClearItem") + " " +
                            listViewAlgorithms.SelectedItems[0].SubItems[1].Text +
                            " (" + listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")";
                        if (listViewAlgorithms.SelectedItems.Count > 1)
                        {
                            _text = International.GetText("AlgorithmsListView_ContextMenu_ClearSelectedItem");
                        }
                        var clearItem = new ToolStripMenuItem
                        {
                            Image = _DisableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = _text
                        };
                        clearItem.Click += ToolStripMenuItemClear_Click;
                        contextMenuStrip1.Items.Add(clearItem);

                        //
                        var al = listViewAlgorithms.SelectedItems[0].SubItems[1].Text + " (" +
                            listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")";
                        _text = International.GetText("AlgorithmsListView_ContextMenu_ClearItemAllDevices").Replace("*", al);
                        if (listViewAlgorithms.SelectedItems.Count > 1)
                        {
                            _text = International.GetText("AlgorithmsListView_ContextMenu_ClearSelectedItemAllDevices");
                        }
                        var clearItemAllDevices = new ToolStripMenuItem
                        {
                            Image = _DisableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = _text
                        };
                        clearItemAllDevices.Click += ToolStripMenuItemClearAllDevices_Click;
                        contextMenuStrip1.Items.Add(clearItemAllDevices);
                        //
                        var clearItemAll = new ToolStripMenuItem
                        {
                            Image = _DisableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = International.GetText("AlgorithmsListView_ContextMenu_ClearItemAll")
                        };
                        clearItemAll.Click += ToolStripMenuItemClearAll_Click;
                        contextMenuStrip1.Items.Add(clearItemAll);
                        this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                    }
                    {//EnableAlgosSelected
                        string al =  listViewAlgorithms.SelectedItems[0].SubItems[1].Text + " (" +
                                listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")";
                        string _text = International.GetText("Form_Settings_EnableAlgos").Replace("*", al);
                        if (listViewAlgorithms.SelectedItems.Count > 1)
                        {
                            _text = International.GetText("Form_Settings_EnableAlgosSelectedAll");
                        }
                        var Enablealgo = new ToolStripMenuItem
                        {
                            Image = _EnableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = _text
                        };
                        Enablealgo.Click += ToolStripMenuEnablealgo_Click;
                        contextMenuStrip1.Items.Add(Enablealgo);
                    }
                    {//DisableAlgosSelected
                        var al = listViewAlgorithms.SelectedItems[0].SubItems[1].Text + " (" +
                            listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")";
                        string _text = International.GetText("Form_Settings_DisableAlgos").Replace("*", al);
                        if (listViewAlgorithms.SelectedItems.Count > 1)
                        {
                            _text = International.GetText("Form_Settings_DisableAlgosSelectedAll");
                        }
                        var Enablealgo = new ToolStripMenuItem
                        {
                            Image = _DisableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = _text
                        };
                        Enablealgo.Click += ToolStripMenuDisablealgo_Click;
                        contextMenuStrip1.Items.Add(Enablealgo);
                    }
                    //force
                    if (listViewAlgorithms.SelectedItems.Count == 1)
                    {

                        this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                        var forceItem = new ToolStripMenuItem
                        {
                            Image = _EnableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = International.GetText("AlgorithmsListView_ContextMenu_ForceItem") + " " +
                            listViewAlgorithms.SelectedItems[0].SubItems[1].Text +
                            " (" + listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")"
                        };
                        forceItem.Click += ToolStripMenuItemForce_Click;
                        forceItem.Enabled = !IsForced();
                        /*
                        if (IsForceEnabled())
                        {
                          forceItem.Enabled = false;
                        }
                        else
                        {
                            forceItem.Enabled = !IsForced();
                        }
                        */
                        contextMenuStrip1.Items.Add(forceItem);

                        var DisableforceItem = new ToolStripMenuItem
                        {
                            Image = _DisableBitmap,
                            ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None,
                            Text = International.GetText("AlgorithmsListView_ContextMenu_DisableForceItem") + " " +
                            listViewAlgorithms.SelectedItems[0].SubItems[1].Text +
                            " (" + listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")"
                        };
                        DisableforceItem.Click += ToolStripMenuItemDisableForce_Click;

                        DisableforceItem.Enabled = IsForced();
                        contextMenuStrip1.Items.Add(DisableforceItem);
                    }

                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
            catch (Exception)
            {

            }
            if (!IsInBenchmark)
            {
                Form_Main.SomeAlgoEnabled = false;
                foreach (ListViewItem lvi in listViewAlgorithms.Items)
                {
                    if (lvi != null && lvi.Tag is Algorithm algorithm)
                    {
                        if (!lvi.SubItems[1].Text.ToLower().Contains("lite"))
                        {
                            if (lvi.Checked == true)
                            {
                                Form_Main.SomeAlgoEnabled = true;
                            }
                        }
                        
                        if (lvi.SubItems[1].Text.ToLower().Contains("lite") && lvi.Checked == true)
                        {
                            Form_Main.LiteAlgos = true;
                        }
                    }
                }

                if (!Form_Main.SomeAlgoEnabled && Form_Main.LiteAlgos)
                {
                    MessageBox.Show(International.GetText("Form_Settings_Lite_Warning"),
                    International.GetText("Warning_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }


            }
        }

        private void ToolStripMenuEnablealgo_Click(object sender, EventArgs e)
        {
            string aName = "";
            MinerBaseType mName = MinerBaseType.NONE;
            var miningDevices = ComputeDeviceManager.Available.Devices;
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        aName = algorithm.AlgorithmName;
                        mName = algorithm.MinerBaseType;

                        foreach (var device in miningDevices)
                        {
                            //Helpers.ConsolePrint("", device.Name);
                            if (device != null)
                            {
                                var devicesAlgos = device.GetAlgorithmSettings();
                                foreach (var a in devicesAlgos)
                                {
                                    if (a.AlgorithmName == aName && a.MinerBaseType == mName)
                                    {
                                        a.Enabled = true;
                                        RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ToolStripMenuDisablealgo_Click(object sender, EventArgs e)
        {
            string aName = "";
            MinerBaseType mName = MinerBaseType.NONE;
            var miningDevices = ComputeDeviceManager.Available.Devices;
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        aName = algorithm.AlgorithmName;
                        mName = algorithm.MinerBaseType;

                        foreach (var device in miningDevices)
                        {
                            //Helpers.ConsolePrint("", device.Name);
                            if (device != null)
                            {
                                var devicesAlgos = device.GetAlgorithmSettings();
                                foreach (var a in devicesAlgos)
                                {
                                    if (a.AlgorithmName == aName && a.MinerBaseType == mName)
                                    {
                                        a.Enabled = false;
                                        RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ToolStripMenuItemEnableAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listViewAlgorithms.Items)
            {
                lvi.Checked = true;
            }
        }

        private void ToolStripMenuItemDisableAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listViewAlgorithms.Items)
            {
                lvi.Checked = false;
            }
        }

        private void ToolStripMenuItemClear_Click(object sender, EventArgs e)
        {
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        algorithm.BenchmarkSpeed = 0;
                        algorithm.BenchmarkSecondarySpeed = 0;
                        algorithm.PowerUsage = 0;
                        algorithm.CurrentProfit = 0;
                        if (algorithm is DualAlgorithm dualAlgo)
                        {
                            dualAlgo.BenchmarkSecondarySpeed = 0;
                        }

                        RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                        // update benchmark status data
                        BenchmarkCalculation?.CalcBenchmarkDevicesAlgorithmQueue();
                        // update settings
                        ComunicationInterface?.ChangeSpeed(lvi);
                    }
                }
            }
        }

        private void ToolStripMenuItemClearAllDevices_Click(object sender, EventArgs e)
        {
            string aName = "";
            MinerBaseType mName = MinerBaseType.NONE;
            var miningDevices = ComputeDeviceManager.Available.Devices;

            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        aName = algorithm.AlgorithmName;
                        mName = algorithm.MinerBaseType;

                        foreach (var device in miningDevices)
                        {
                            //Helpers.ConsolePrint("", device.Name);
                            if (device != null)
                            {
                                var devicesAlgos = device.GetAlgorithmSettings();
                                foreach (var a in devicesAlgos)
                                {
                                    if (a.AlgorithmName == aName && a.MinerBaseType == mName)
                                    {
                                        a.BenchmarkSpeed = 0;
                                        a.BenchmarkSecondarySpeed = 0;
                                        if (a is DualAlgorithm dualAlgo)
                                        {
                                            dualAlgo.BenchmarkSecondarySpeed = 0;
                                        }
                                        a.PowerUsage = 0;
                                        a.CurrentProfit = 0;
                                        RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                                        BenchmarkCalculation?.CalcBenchmarkDevicesAlgorithmQueue();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ToolStripMenuItemClearAll_Click(object sender, EventArgs e)
        {
            if (_computeDevice != null)
            {
                var dialogRes = MessageBox.Show(International.GetText("Form_Settings_DelBenchmarks"), "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogRes == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                foreach (ListViewItem lvi in listViewAlgorithms.Items)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        algorithm.BenchmarkSpeed = 0;
                        algorithm.BenchmarkSecondarySpeed = 0;
                        if (algorithm is DualAlgorithm dualAlgo)
                        {
                            dualAlgo.BenchmarkSecondarySpeed = 0;
                        }
                        algorithm.PowerUsage = 0;
                        algorithm.CurrentProfit = 0;
                        RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                        BenchmarkCalculation?.CalcBenchmarkDevicesAlgorithmQueue();

                        ComunicationInterface?.ChangeSpeed(lvi);
                    }
                }
            }
        }
        private void ToolStripMenuItemEnableSelected_Click(object sender, EventArgs e)
        {
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.Items)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        lvi.Checked = lvi.Selected;
                        if (lvi.Selected && algorithm.BenchmarkSpeed <= 0)
                        {
                            RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                            ComunicationInterface?.ChangeSpeed(lvi);
                        }
                    }
                }
            }
        }

        private void ToolStripMenuItemDisableSelected_Click(object sender, EventArgs e)
        {
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        lvi.Checked = !lvi.Selected;
                        if (lvi.Selected && algorithm.BenchmarkSpeed <= 0)
                        {
                            RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                            ComunicationInterface?.ChangeSpeed(lvi);
                        }
                    }
                }
            }
        }

        private void ToolStripMenuItemEnableBenched_Click(object sender, EventArgs e)
        {
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.Items)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        if (algorithm.BenchmarkSpeed > 0)
                        {
                            lvi.Checked = true;
                            RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                        }
                        else
                        {
                            lvi.Checked = false;
                            RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                        }
                    }
                }
            }
        }

        private bool IsForced()
        {
            foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
            {
                if (lvi.Tag is Algorithm algorithm)
                {
                    return algorithm.Forced;
                }
            }
            return false;
        }
        private bool IsForceEnabled()
        {
            foreach (ListViewItem lvi in listViewAlgorithms.Items)
            {
                if (lvi.Tag is Algorithm algorithm)
                {
                    if (algorithm.Forced)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void DisableAllForces()
        {
            foreach (ListViewItem lvi in listViewAlgorithms.Items)
            {
                if (lvi.Tag is Algorithm algorithm)
                {
                    algorithm.Forced = false;
                }
            }
            return;
        }

        private void ToolStripMenuItemForce_Click(object sender, EventArgs e)
        {
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        if (algorithm.BenchmarkSpeed > 0)
                        {
                            DisableAllForces();
                            algorithm.Forced = true;
                            RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                            BenchmarkCalculation?.CalcBenchmarkDevicesAlgorithmQueue();
                            ComunicationInterface?.ChangeSpeed(lvi);
                        }
                        else
                        {
                            string miner = algorithm.MinerBaseTypeName;
                            string name = algorithm.AlgorithmName;
                            if (miner.ToLower().Contains("nbminer") && name.ToLower().Contains("beam"))
                            {
                                miner = miner + MinerVersion.GetMinerVersion("nbminer.39.5");
                            }
                            else if (miner.ToLower().Contains("nbminer") && name.ToLower().Contains("kawpow"))
                            {
                                miner = miner + MinerVersion.GetMinerVersion("nbminer.39.5");
                            }
                            else if (miner.ToLower().Contains("nbminer") && name.ToLower().Contains("ergo"))
                            {
                                miner = miner + MinerVersion.GetMinerVersion("nbminer.39.5");
                            }
                            else
                            {
                                miner = miner + MinerVersion.GetMinerVersion(miner);
                            }

                            MessageBox.Show(string.Format(International.GetText("Form_Benchmark_listView_NeedBenchmark"),
                                algorithm.AlgorithmName + " (" + miner + ")"));
                        }
                    }
                }
            }
        }
        private void ToolStripMenuItemDisableForce_Click(object sender, EventArgs e)
        {
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        algorithm.Forced = false;
                        RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                        BenchmarkCalculation?.CalcBenchmarkDevicesAlgorithmQueue();
                        ComunicationInterface?.ChangeSpeed(lvi);
                    }
                }
            }
        }

        private void listViewAlgorithms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listViewAlgorithms_EnabledChanged(object sender, EventArgs e)
        {
            //  AlgorithmsListView.colorListViewHeader(ref listViewAlgorithms, Color.Red, Form_Main._textColor);
        }

        private void listViewAlgorithms_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewAlgorithms != null & listViewAlgorithms.SelectedItems.Count > 0)
                {
                    
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("listViewAlgorithms_Click", ex.ToString());
            }
            if (!isListViewEnabled)
            {
                listViewAlgorithms.SelectedItems.Clear();
            }
        }

        private void listViewAlgorithms_ItemChecked_1(object sender, ItemCheckedEventArgs e)
        {
            if (!isListViewEnabled)
            {
                listViewAlgorithms.SelectedItems.Clear();
            }
            if (IsInBenchmark)
            {
                //listViewAlgorithms.CheckBoxes = false;
                //e.Item.Checked = !e.Item.Checked;
            }
        }

        private void listViewAlgorithms_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (mouseDown)
            {
                e.NewValue = e.CurrentValue;
            }
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift || (ModifierKeys == (Keys.Control | Keys.Shift)))
            {
                e.NewValue = e.CurrentValue;
            }
            if (!isListViewEnabled)
            {
                listViewAlgorithms.SelectedItems.Clear();
            }
        }

        private void listViewAlgorithms_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            listViewAlgorithms.BeginUpdate();

            if (e.ColumnIndex == 6)
            {
                ResizeAutoSizeColumn(listViewAlgorithms, 1);
            }
            else
            {
                ResizeAutoSizeColumn(listViewAlgorithms, 6);
            }
            /*
            if (e.ColumnIndex == 1)
            {
                ResizeAutoSizeColumn(listViewAlgorithms, 6);
            }
            */
            listViewAlgorithms.EndUpdate();
        }
        static private void ResizeAutoSizeColumn(ListView listView, int autoSizeColumnIndex)
        {
            // Do some rudimentary (parameter) validation.
            if (listView == null) throw new ArgumentNullException("listView");
            if (listView.View != View.Details || listView.Columns.Count <= 0 || autoSizeColumnIndex < 0) return;
            if (autoSizeColumnIndex >= listView.Columns.Count)
                throw new IndexOutOfRangeException("Parameter autoSizeColumnIndex is outside the range of column indices in the ListView.");

            // Sum up the width of all columns except the auto-resizing one.
            int otherColumnsWidth = 0;
            foreach (ColumnHeader header in listView.Columns)
                if (header.Index != autoSizeColumnIndex)
                    otherColumnsWidth += header.Width;

            // Calculate the (possibly) new width of the auto-resizable column.
            int autoSizeColumnWidth = listView.ClientRectangle.Width - otherColumnsWidth;

            // Finally set the new width of the auto-resizing column, if it has changed.
            if (listView.Columns[autoSizeColumnIndex].Width != autoSizeColumnWidth)
                listView.Columns[autoSizeColumnIndex].Width = autoSizeColumnWidth;

        }

        private void listViewAlgorithms_Resize(object sender, EventArgs e)
        {
            //ResizeColumn();
            listViewAlgorithms.BeginUpdate();
            ResizeAutoSizeColumn(listViewAlgorithms, 6);
            listViewAlgorithms.EndUpdate();
        }

        private void listViewAlgorithms_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ConfigManager.GeneralConfig.ColumnListALGORITHM = listViewAlgorithms.Columns[ALGORITHM].Width;
            ConfigManager.GeneralConfig.ColumnListMINER = listViewAlgorithms.Columns[MINER].Width;
            ConfigManager.GeneralConfig.ColumnListSPEED = listViewAlgorithms.Columns[SPEED].Width;
            ConfigManager.GeneralConfig.ColumnListPOWER = listViewAlgorithms.Columns[POWER].Width;
            ConfigManager.GeneralConfig.ColumnListRATIO = listViewAlgorithms.Columns[RATIO].Width;
            ConfigManager.GeneralConfig.ColumnListRATE = listViewAlgorithms.Columns[RATE].Width;
        }

        private void listViewAlgorithms_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (ConfigManager.GeneralConfig.ColumnSort)
            {
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparer(2);
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparer(e.Column);
                ConfigManager.GeneralConfig.ColumnListSort = e.Column;
            }
        }

        private void AlgorithmsListView_Load(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.ColumnSort)
            {
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparer(2);
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparer(ConfigManager.GeneralConfig.ColumnListSort);
            }
        }
        bool mouseDown = false;
        private void listViewAlgorithms_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
        }

        private void listViewAlgorithms_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void listViewAlgorithms_MouseLeave(object sender, EventArgs e)
        {
            mouseDown = false;
        }
    }
    class ListViewColumnComparer : IComparer
    {
        public int ColumnIndex { get; set; }

        public ListViewColumnComparer(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }

        public int Compare(object x, object y)
        {
            try
            {
                return String.Compare(
                ((ListViewItem)x).SubItems[ColumnIndex].Text,
                ((ListViewItem)y).SubItems[ColumnIndex].Text);
            }
            catch (Exception) // если вдруг столбец пустой (или что-то пошло не так)
            {
                return 0;
            }
        }
    }
    static class ComCtrlExtensions
    {
        private const string EXPLORER = "EXPLORER";

        [DllImport("uxtheme.dll", ExactSpelling = true, PreserveSig = false, CharSet = CharSet.Auto)]
        public static extern void SetWindowTheme(IntPtr hWnd, string subAppName, string subIdList);

        public static void WindowExplorerTheme(this ListView ctrl, bool enable)
        {
            string appName = enable ? EXPLORER : null;
            SetWindowTheme(ctrl.Handle, appName, null);
        }
    }
}
