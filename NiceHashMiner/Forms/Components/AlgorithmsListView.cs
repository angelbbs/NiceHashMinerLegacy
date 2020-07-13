/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using NiceHashMiner.Devices;
using NiceHashMiner.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMiner.Stats;
using System.Globalization;
using System.Collections;
using NiceHashMiner.Miners.Grouping;

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
                        } else
                        {
                            lvi.BackColor = SystemColors.ControlLightLight;
                        }
                        lvi.ForeColor = DisabledForeColor;
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
                        } else
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

            // System.Reflection.PropertyInfo dbProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            // dbProp.SetValue(this, true, null);
            //if (ConfigManager.GeneralConfig.ColorProfileIndex == 0)
            //{
              //  AlgorithmsListView.colorListViewHeader(ref listViewAlgorithms, SystemColors.ControlLightLight, Form_Main._textColor);
            //}
            //else

                AlgorithmsListView.colorListViewHeader(ref listViewAlgorithms, Form_Main._backColor, Form_Main._textColor);


            // callback initializations
            listViewAlgorithms.ItemSelectionChanged += ListViewAlgorithms_ItemSelectionChanged;
            listViewAlgorithms.ItemChecked += (ItemCheckedEventHandler) ListViewAlgorithms_ItemChecked;
            IsInBenchmark = false;
         //   listViewAlgorithms.OwnerDraw = true;
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
            /*
            //  foreach (var lbl in this.Controls.OfType<ListView>()) lbl.BackColor = _backColor;
            listViewAlgorithms.BackColor = _backColor;
            listViewAlgorithms.ForeColor = _textColor;
            this.BackColor = _backColor;
            */
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
            listViewAlgorithms.Columns[MINER].Text = "Miner";
            listViewAlgorithms.Columns[SPEED].Text = International.GetText("AlgorithmsListView_Speed");
            //listViewAlgorithms.Columns[SECSPEED].Text = International.GetText("Form_DcriValues_SecondarySpeed");
            listViewAlgorithms.Columns[POWER].Text = "Power";
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                listViewAlgorithms.Columns[MINER].Text = "Майнер";
                listViewAlgorithms.Columns[POWER].Text = "Потребление";
            }



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

        public void SetAlgorithms(ComputeDevice computeDevice, bool isEnabled)
        {
            _computeDevice = computeDevice;
            listViewAlgorithms.BeginUpdate();
            listViewAlgorithms.Items.Clear();
            foreach (var alg in computeDevice.GetAlgorithmSettings())
            {
                if (!alg.Hidden)
                {
                    var lvi = new ListViewItem();

                    var name = "";
                    var miner = "";
                    var secondarySpeed = "";
                    var totalSpeed = "";
                    var payingRatio = "";
                    if (alg is DualAlgorithm dualAlg)
                    {
                       // name = "  ↑ + " + dualAlg.SecondaryAlgorithmName;
                        //name = "  " + char.ConvertFromUtf32(8593) + " + " + dualAlg.SecondaryAlgorithmName;
                        name = dualAlg.AlgorithmName;
                        miner = alg.MinerBaseTypeName;
                        secondarySpeed = dualAlg.SecondaryBenchmarkSpeedString();
                        totalSpeed = alg.BenchmarkSpeedString() + "/" + secondarySpeed;
                        payingRatio = alg.CurPayingRatio + "/" +dualAlg.SecondaryCurPayingRatio;
                    }
                    else
                    {
                        name = alg.AlgorithmName;
                        miner = alg.MinerBaseTypeName;
                        totalSpeed = alg.BenchmarkSpeedString();
                        //name = $"{alg.AlgorithmName} ({alg.MinerBaseTypeName})";
                        payingRatio = alg.CurPayingRatio;
                    }

                    lvi.SubItems.Add(name);
                    lvi.SubItems.Add(miner);

                    //sub.Tag = alg.Value;

                    lvi.SubItems.Add(totalSpeed);
                    //lvi.SubItems.Add(secondarySpeed);
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
                    double WithPowerRate = 0;
                    WithPowerRate = valueRate - ExchangeRateApi.GetKwhPriceInBtc() * alg.PowerUsage * 24 * Form_Main._factorTimeUnit / 1000;
                    if (!ConfigManager.GeneralConfig.DecreasePowerCost)
                    {
                        WithPowerRate = valueRate;
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
                        } else
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
                            lvi.SubItems.Add(alg.CurPayingRate);
                        }
                    }
                    lvi.Tag = alg;
                    lvi.Checked = alg.Enabled;
                    listViewAlgorithms.Items.Add(lvi);
                }
            }

            listViewAlgorithms.EndUpdate();
            //Enabled = isEnabled;
         //   if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
           // {
                isListViewEnabled = isEnabled;
                listViewAlgorithms.CheckBoxes = isEnabled;
            //}
        }

        public void UpdateLvi()
        {
            try
            {
                if (_computeDevice != null)
                {
                    foreach (ListViewItem lvi in listViewAlgorithms.Items)
                    {
                        if (lvi.Tag is Algorithm algorithm)
                        {
                            var algo = lvi.Tag as Algorithm;
                            if (algo != null)
                            {
                                if (algorithm is DualAlgorithm dualAlg)
                                {
                                    lvi.SubItems[RATIO].Text = algorithm.CurPayingRatio + "/" + dualAlg.SecondaryCurPayingRatio;
                                }
                                else
                                {
                                    lvi.SubItems[RATIO].Text = algorithm.CurPayingRatio;
                                }
                                double.TryParse(algorithm.CurPayingRate, out var valueRate);
                                double WithPowerRate = 0;
                                WithPowerRate = valueRate - ExchangeRateApi.GetKwhPriceInBtc() * algorithm.PowerUsage * 24 * Form_Main._factorTimeUnit / 1000;
                                if (!ConfigManager.GeneralConfig.DecreasePowerCost)
                                {
                                    WithPowerRate = valueRate;
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
                                        lvi.SubItems[RATE].Text = algo.CurPayingRate;
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
                            //   lvi.SubItems[SECSPEED].Text = dualAlg.SecondaryBenchmarkSpeedString();
                            lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString() + "/" + dualAlg.SecondaryBenchmarkSpeedString();
                        }
                        else
                        {
                            lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString();
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

                //Visible = isEnabled;
                //Enabled = isEnabled;
              //  if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
               // {

               // }
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
                if (!ConfigManager.GeneralConfig.DivertRun && Form_Main.DaggerHashimoto3GB && 
                    algo.NiceHashID == AlgorithmType.DaggerHashimoto3GB && algo.Enabled)
                {
                    algo.Enabled = false;
                    e.Item.Checked = false;
                    MessageBox.Show("WinDivert driver error. DaggerHashimoto3GB disabled",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (!ConfigManager.GeneralConfig.DivertRun && Form_Main.DaggerHashimoto4GB && 
                    algo.NiceHashID == AlgorithmType.DaggerHashimoto4GB && algo.Enabled)
                {
                    algo.Enabled = false;
                    e.Item.Checked = false;
                    MessageBox.Show("WinDivert driver error. DaggerHashimoto4GB disabled",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                /*
                if (ConfigManager.GeneralConfig.DivertRun)
                {
                    if ((Form_Main.DaggerHashimoto3GB && algo.NiceHashID == AlgorithmType.DaggerHashimoto3GB &&
                        algo.Enabled && !Form_Main.DivertAvailable) ||
                        !ConfigManager.GeneralConfig.DivertRun)
                    {
                        algo.Enabled = false;
                        e.Item.Checked = false;
                        MessageBox.Show("WinDivert driver error. DaggerHashimoto3GB disabled",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    if ((Form_Main.DaggerHashimoto4GB && algo.NiceHashID == AlgorithmType.DaggerHashimoto4GB &&
                        algo.Enabled && !Form_Main.DivertAvailable) ||
                        !ConfigManager.GeneralConfig.DivertRun)
                    {
                        algo.Enabled = false;
                        e.Item.Checked = false;
                        MessageBox.Show("WinDivert driver error. DaggerHashimoto4GB disabled",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                */
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
                                    lvi.SubItems[SPEED].Text = algo.BenchmarkSpeedString();
                            }

                            //    lvi.SubItems[SPEED].Text = algorithm.BenchmarkSpeedString();
                            double.TryParse(algorithm.CurPayingRate, out var valueRate);
                            var WithPowerRate = valueRate - ExchangeRateApi.GetKwhPriceInBtc() * algorithm.PowerUsage * 24 * Form_Main._factorTimeUnit / 1000;

                            if (ConfigManager.GeneralConfig.DecreasePowerCost)
                            {
                                lvi.SubItems[RATE].Text = WithPowerRate.ToString("F8");
                            }
                            else
                            {
                                lvi.SubItems[RATE].Text = algo.CurPayingRate;
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
                                lvi.SubItems[RATIO].Text = algorithm.CurPayingRatio + "/" + dualAlg.SecondaryCurPayingRatio;
                              //  lvi.SubItems[SECSPEED].Text = dualAlg.SecondaryBenchmarkSpeedString();
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
            if (!isListViewEnabled)
            {
                listViewAlgorithms.SelectedItems.Clear();
                return;
            }
            if (IsInBenchmark) return;
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Items.Clear();
                // enable all
                {
                    var enableAllItems = new ToolStripMenuItem
                    {
                        Text = International.GetText("AlgorithmsListView_ContextMenu_EnableAll")
                    };
                    enableAllItems.Click += ToolStripMenuItemEnableAll_Click;
                    contextMenuStrip1.Items.Add(enableAllItems);
                }
                // disable all
                {
                    var disableAllItems = new ToolStripMenuItem
                    {
                        Text = International.GetText("AlgorithmsListView_ContextMenu_DisableAll")
                    };
                    disableAllItems.Click += ToolStripMenuItemDisableAll_Click;
                    contextMenuStrip1.Items.Add(disableAllItems);
                }
                // test this
                {
                    var testItem = new ToolStripMenuItem
                    {
                        Text = International.GetText("AlgorithmsListView_ContextMenu_TestItem") + " " +
                        listViewAlgorithms.SelectedItems[0].SubItems[1].Text + " (" +
                        listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")"
                    };
                    testItem.Click += ToolStripMenuItemTest_Click;
                    contextMenuStrip1.Items.Add(testItem);
                }
                // enable benchmarked only
                {
                    var enableBenchedItem = new ToolStripMenuItem
                    {
                        Text = International.GetText("AlgorithmsListView_ContextMenu_EnableBenched")
                    };
                    enableBenchedItem.Click += ToolStripMenuItemEnableBenched_Click;
                    contextMenuStrip1.Items.Add(enableBenchedItem);
                }
                // clear item
                {
                    var clearItem = new ToolStripMenuItem
                    {
                        Text = International.GetText("AlgorithmsListView_ContextMenu_ClearItem")
                    };
                    clearItem.Click += ToolStripMenuItemClear_Click;
                    contextMenuStrip1.Items.Add(clearItem);
                }
                {
                    var al = listViewAlgorithms.SelectedItems[0].SubItems[1].Text + " (" +
                        listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")";
                    var Enablealgo = new ToolStripMenuItem
                    {
                        Text = International.GetText("Form_Settings_EnableAlgos").Replace("*", al)
                    };
                    Enablealgo.Click += ToolStripMenuEnablealgo_Click;
                    contextMenuStrip1.Items.Add(Enablealgo);
                }
                {
                    var al = listViewAlgorithms.SelectedItems[0].SubItems[1].Text + " (" +
                        listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")";
                    var Enablealgo = new ToolStripMenuItem
                    {
                        Text = International.GetText("Form_Settings_DisableAlgos").Replace("*", al)
                    };
                    Enablealgo.Click += ToolStripMenuDisablealgo_Click;
                    contextMenuStrip1.Items.Add(Enablealgo);
                }

                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void ToolStripMenuEnablealgo_Click(object sender, EventArgs e)
        {
            string aName = "";
            MinerBaseType mName = MinerBaseType.NONE;
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        aName =  algorithm.AlgorithmName;
                        mName = algorithm.MinerBaseType;
                        if (algorithm is DualAlgorithm dualAlgo)
                        {
                        }
                    }
                }
                var miningDevices = ComputeDeviceManager.Available.Devices;
                foreach (var device in miningDevices)
                {
                    Helpers.ConsolePrint("", device.Name);
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
        private void ToolStripMenuDisablealgo_Click(object sender, EventArgs e)
        {
            string aName = "";
            MinerBaseType mName = MinerBaseType.NONE;
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        aName = algorithm.AlgorithmName;
                        mName = algorithm.MinerBaseType;
                        if (algorithm is DualAlgorithm dualAlgo)
                        {
                        }
                    }
                }
                var miningDevices = ComputeDeviceManager.Available.Devices;
                foreach (var device in miningDevices)
                {
                    Helpers.ConsolePrint("", device.Name);
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
                        if (algorithm is DualAlgorithm dualAlgo)
                        {
                            dualAlgo.SecondaryBenchmarkSpeed = 0;
                            dualAlgo.IntensitySpeeds = new Dictionary<int, double>();
                            dualAlgo.SecondaryIntensitySpeeds = new Dictionary<int, double>();
                            dualAlgo.IntensityUpToDate = false;
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

        private void ToolStripMenuItemTest_Click(object sender, EventArgs e)
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
                            // If it has zero speed, set to 1 so it can be tested
                            algorithm.BenchmarkSpeed = 1;
                            RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                            ComunicationInterface?.ChangeSpeed(lvi);
                        }
                    }
                }
            }
        }

        private void toolStripMenuItemOpenDcri_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
            {
                if (lvi.Tag is DualAlgorithm algo)
                {
                    var dcriValues = new FormDcriValues(algo);
                    dcriValues.ShowDialog();
                    RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                    // update benchmark status data
                    BenchmarkCalculation?.CalcBenchmarkDevicesAlgorithmQueue();
                    // update settings
                    ComunicationInterface?.ChangeSpeed(lvi);
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
                        } else
                        {
                            lvi.Checked = false;
                            RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                        }
                    }
                }
            }
        }

        private void toolStripMenuItemTuningEnabled_Checked(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
            {
                if (lvi.Tag is DualAlgorithm algo)
                {
                    algo.TuningEnabled = ((ToolStripMenuItem) sender).Checked;
                    RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
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
            } else
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

        private void AlgorithmsListView_EnabledChanged(object sender, EventArgs e)
        {

        }

        private void AlgorithmsListView_Load(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.ColumnSort)
            {
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparer(2);
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparer(ConfigManager.GeneralConfig.ColumnListSort);
            }
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

}
