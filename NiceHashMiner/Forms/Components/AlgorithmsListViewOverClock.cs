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
using MSI.Afterburner;

namespace NiceHashMiner.Forms.Components
{
    public partial class AlgorithmsListViewOverClock : UserControl
    {
        private const int ENABLED = 0;
        private const int ALGORITHM = 1;
        private const int MINER = 2;
        private const int GPU_clock = 3;
        private const int Mem_clock = 4;
        private const int GPU_voltage = 5;
        private const int Mem_voltage = 6;
        private const int Power_limit = 7;
        private const int Fan = 8;
        //private const int Fan_flag = 9;
        private const int Thermal_limit = 9;
        public static bool isListViewEnabled = true;

        private static int _SubItembIndex = 0;
        private static char _keyPressed;

        public interface IAlgorithmsListViewOverClock
        {
            void SetCurrentlySelected(ListViewItem lvi, ComputeDevice computeDevice);
            void HandleCheck(ListViewItem lvi);
            void ChangeSpeed(ListViewItem lvi);
        }

        public IAlgorithmsListViewOverClock ComunicationInterface { get; set; }

        //public IBenchmarkCalculation BenchmarkCalculation { get; set; }

        internal static ComputeDevice _computeDevice;

        private class DefaultAlgorithmColorSeter : IListItemCheckColorSetter
        {
            public static Color DisabledColor = Form_Main._backColor;
            public static Color DisabledForeColor = Color.Gray;
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

        
        public AlgorithmsListViewOverClock()
        {
            InitializeComponent();
            listViewAlgorithms.DoubleBuffer();
            AlgorithmsListViewOverClock.colorListViewHeader(ref listViewAlgorithms, Form_Main._backColor, Form_Main._textColor);

            // callback initializations
            listViewAlgorithms.ItemSelectionChanged += ListViewAlgorithms_ItemSelectionChanged;
            listViewAlgorithms.ItemChecked += (ItemCheckedEventHandler) ListViewAlgorithms_ItemChecked;

            if (ConfigManager.GeneralConfig.ABEnableOverclock && MSIAfterburner.Initialized)
            {
                MSIAfterburner.InitTempFiles();
            }
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
            listViewAlgorithms.Columns[MINER].Text = "Miner";

            listViewAlgorithms.Columns[ALGORITHM].Width = ConfigManager.GeneralConfig.ColumnListALGORITHM;
            listViewAlgorithms.Columns[MINER].Width = ConfigManager.GeneralConfig.ColumnListMINER;
            listViewAlgorithms.Columns[GPU_clock].Width = ConfigManager.GeneralConfig.ColumnListGPU_clock;
            listViewAlgorithms.Columns[Mem_clock].Width = ConfigManager.GeneralConfig.ColumnListMem_clock;
            listViewAlgorithms.Columns[GPU_voltage].Width = ConfigManager.GeneralConfig.ColumnListGPU_voltage;
            listViewAlgorithms.Columns[Mem_voltage].Width = ConfigManager.GeneralConfig.ColumnListMem_voltage;
            listViewAlgorithms.Columns[Power_limit].Width = ConfigManager.GeneralConfig.ColumnListPowerLimit;
            listViewAlgorithms.Columns[Fan].Width = ConfigManager.GeneralConfig.ColumnListFan;
            //listViewAlgorithms.Columns[Fan_flag].Width = ConfigManager.GeneralConfig.ColumnListFanFlag;
            listViewAlgorithms.Columns[Thermal_limit].Width = ConfigManager.GeneralConfig.ColumnListThermalLimit;
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
                    //ControlMemoryGpuEntry dev = MSIAfterburner.GetDeviceData(computeDevice.BusID);
                    var lvi = new ListViewItem();
                    string name;
                    string miner;
                    int gpu_clock;
                    int mem_clock;
                    double gpu_voltage;
                    double mem_voltage;
                    int power_limit;
                    uint fan;
                    int fan_flag;
                    int thermal_limit;
                    string fName = "temp\\" + computeDevice.Uuid + "_" + alg.AlgorithmStringID + ".tmp";
                    ControlMemoryGpuEntry dev = MSIAfterburner.ReadFromFile(computeDevice.BusID, fName);
                    //читать из временых файлов

                        name = alg.AlgorithmName;
                        miner = alg.MinerBaseTypeName;
                        gpu_clock = dev.CoreClockBoostCur / 1000;
                        mem_clock = dev.MemoryClockBoostCur / 1000;
                        gpu_voltage = dev.CoreVoltageBoostCur;
                        mem_voltage = dev.MemoryVoltageBoostCur;
                        power_limit = dev.PowerLimitCur;
                        fan = dev.FanSpeedCur;
                        fan_flag = (int)dev.FanFlagsCur;
                        thermal_limit = dev.ThermalLimitCur;

                    lvi.SubItems.Add(name);
                    lvi.SubItems.Add(miner);

                    if (dev.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED))
                    {
                        lvi.SubItems.Add("Curve");
                    }
                    else
                    {
                        lvi.SubItems.Add(gpu_clock.ToString());
                    }
                    lvi.SubItems.Add(mem_clock.ToString());
                    lvi.SubItems.Add(gpu_voltage.ToString());
                    lvi.SubItems.Add(mem_voltage.ToString());
                    lvi.SubItems.Add(power_limit.ToString());
                    
                    if (fan_flag == 0)
                    {
                        lvi.SubItems.Add(fan.ToString());
                    }
                    else
                    {
                        lvi.SubItems.Add("Auto");
                    }

                    //lvi.SubItems.Add(fan_flag.ToString());
                    lvi.SubItems.Add(thermal_limit.ToString());
                    
                    lvi.Tag = alg;
                    lvi.Checked = alg.Enabled;
                    listViewAlgorithms.Items.Add(lvi);

                }
            }

            listViewAlgorithms.EndUpdate();
                isListViewEnabled = isEnabled;
                listViewAlgorithms.CheckBoxes = isEnabled;
        }

        public void UpdateLvi()
        {
            /*
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
            */
        }
        public void RepaintStatus(bool isEnabled, string uuid)
        {
            /*
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
            }
            */
        }

        #region Callbacks Events

        private void ListViewAlgorithms_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
           // MessageBox.Show(_computeDevice.Name.ToString());
            //ComunicationInterface?.SetCurrentlySelected(e.Item, _computeDevice);

        }

        private void ListViewAlgorithms_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Tag is Algorithm algo)
            {
                algo.Enabled = e.Item.Checked;
            }

            ComunicationInterface?.HandleCheck(e.Item);
            var lvi = e.Item;
            _listItemCheckColorSetter.LviSetColor(lvi);
        }

        #endregion //Callbacks Events

        public void ResetListItemColors()
        {
            foreach (ListViewItem lvi in listViewAlgorithms.Items)
            {
                _listItemCheckColorSetter?.LviSetColor(lvi);
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
                GetDefMinMax();
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Items.Clear();
                    //test msi ab
                    {
                        var MSIABGET = new ToolStripMenuItem
                        {
                            Text = "Get values from MSI Afterburner"
                        };
                        MSIABGET.Click += ToolStripMenuItemGET_Click;
                        contextMenuStrip1.Items.Add(MSIABGET);
                    }

                    {
                        var MSIABSAVE = new ToolStripMenuItem
                        {
                            Text = "Load GPU data from file to MSI Afterburner"
                        };
                        MSIABSAVE.Click += ToolStripMenuItemLOADFILE_Click;
                        contextMenuStrip1.Items.Add(MSIABSAVE);
                    }
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
                    this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
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
                    this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
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
                        this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                        var clearItem = new ToolStripMenuItem
                        {
                            Text = International.GetText("AlgorithmsListView_ContextMenu_ClearItem") + " " +
                            listViewAlgorithms.SelectedItems[0].SubItems[1].Text +
                            " (" + listViewAlgorithms.SelectedItems[0].SubItems[2].Text + ")"
                        };
                        clearItem.Click += ToolStripMenuItemClear_Click;
                        contextMenuStrip1.Items.Add(clearItem);
                        //this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                        //
                        var clearItemAll = new ToolStripMenuItem
                        {
                            Text = International.GetText("AlgorithmsListView_ContextMenu_ClearItemAll")
                        };
                        clearItemAll.Click += ToolStripMenuItemClearAll_Click;
                        contextMenuStrip1.Items.Add(clearItemAll);
                        this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
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
            } catch (Exception ex)
            {

            }
        }
        private void GetDefMinMax()
        {
            if (!MSIAfterburner.Initialized) return;
            if (_computeDevice != null)
            {
                ControlMemoryGpuEntry _abdata = MSIAfterburner.GetDeviceData(_computeDevice.BusID);
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        algorithm.gpu_clock_def = _abdata.CoreClockBoostDef / 1000;
                        algorithm.gpu_clock_min = _abdata.CoreClockBoostMin / 1000;
                        algorithm.gpu_clock_max = _abdata.CoreClockBoostMax / 1000;

                        algorithm.mem_clock_def = _abdata.MemoryClockBoostDef / 1000;
                        algorithm.mem_clock_min = _abdata.MemoryClockBoostMin / 1000;
                        algorithm.mem_clock_min = _abdata.MemoryClockBoostMax / 1000;

                        algorithm.gpu_voltage_def = _abdata.CoreVoltageBoostDef;
                        algorithm.gpu_voltage_min = _abdata.CoreVoltageBoostMin;
                        algorithm.gpu_voltage_max = _abdata.CoreVoltageBoostMax;

                        algorithm.mem_voltage_def = _abdata.MemoryVoltageBoostDef;
                        algorithm.mem_voltage_min = _abdata.MemoryVoltageBoostMin;
                        algorithm.mem_voltage_max = _abdata.MemoryVoltageBoostMax;

                        algorithm.power_limit_def = _abdata.PowerLimitDef;
                        algorithm.power_limit_min = _abdata.PowerLimitMin;
                        algorithm.power_limit_max = _abdata.PowerLimitMax;

                        algorithm.fan_def = (int)_abdata.FanSpeedDef;
                        algorithm.fan_min = (int)_abdata.FanSpeedMin;
                        algorithm.fan_max = (int)_abdata.FanSpeedMax;

                        algorithm.fan_flag = (int)_abdata.FanFlagsCur;

                        algorithm.thermal_limit_def = _abdata.ThermalLimitDef;
                        algorithm.thermal_limit_min = _abdata.ThermalLimitMin;
                        algorithm.thermal_limit_max = _abdata.ThermalLimitMax;
                    }
                }
            }
        }
        private void ToolStripMenuItemGET_Click(object sender, EventArgs e)
        {
            if (!MSIAfterburner.Initialized) return;
            if (_computeDevice != null)
            {
                Helpers.ConsolePrint("ToolStripMenuItemGET_Click", "**********************");
                ControlMemoryGpuEntry _abdata = MSIAfterburner.GetDeviceData(_computeDevice.BusID);
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        /*
                        algorithm.gpu_clock = _abdata.CoreClockBoostCur / 1000;
                        algorithm.mem_clock = _abdata.MemoryClockBoostCur / 1000;
                        algorithm.gpu_voltage = _abdata.CoreVoltageBoostCur;
                        algorithm.mem_voltage = _abdata.MemoryVoltageBoostCur;
                        algorithm.power_limit = _abdata.PowerLimitCur;
                        algorithm.fan = (int)_abdata.FanSpeedCur;
                        algorithm.fan_flag = (int)_abdata.FanFlagsCur;
                        algorithm.thermal_limit = _abdata.ThermalLimitCur;
                        */
                        string fName = "temp\\" + _computeDevice.Uuid + "_" + algorithm.AlgorithmStringID + ".tmp";
                        MSIAfterburner.SaveDeviceData(_abdata, fName);
                    }
                }
                SetAlgorithms(_computeDevice, _computeDevice.Enabled);
                RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                
            }
        }
        private void ToolStripMenuItemLOADFILE_Click(object sender, EventArgs e)
        {
            if (!MSIAfterburner.Initialized) return;
            if (_computeDevice != null)
            {
                MSIAfterburner.ReadFromFile(_computeDevice.BusID, "gpu"+_computeDevice.Index.ToString()+".gpu");
            }
        }

        private void ToolStripMenuEnablealgo_Click(object sender, EventArgs e)
        {
            string aName = "";
            MinerBaseType mName = MinerBaseType.NONE;
            //if (_computeDevice != null && _computeDevice.DeviceType != DeviceType.CPU)
            if (_computeDevice != null)
            {
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        aName =  algorithm.AlgorithmName;
                        mName = algorithm.MinerBaseType;
                    }
                }
                var miningDevices = ComputeDeviceManager.Available.Devices;
                foreach (var device in miningDevices)
                {
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
            //if (_computeDevice != null && _computeDevice.DeviceType != DeviceType.CPU)
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
                        // update settings
//                        ComunicationInterface?.ChangeSpeed(lvi);
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
                        RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);

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
                            //algorithm.BenchmarkSpeed = 1;
                            RepaintStatus(_computeDevice.Enabled, _computeDevice.Uuid);
                            ComunicationInterface?.ChangeSpeed(lvi);
                        }
                    }
                }
            }
        }

        private void ToolStripMenuItemEnableBenched_Click(object sender, EventArgs e)
        {
            /*
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
            */
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

            if (e.ColumnIndex == 9)
            {
                ResizeAutoSizeColumn(listViewAlgorithms, 1);
            } else
            {
                ResizeAutoSizeColumn(listViewAlgorithms, 9);
            }
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
            ResizeAutoSizeColumn(listViewAlgorithms, 1);
            listViewAlgorithms.EndUpdate();
        }

        private void listViewAlgorithms_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ConfigManager.GeneralConfig.ColumnListALGORITHM = listViewAlgorithms.Columns[ALGORITHM].Width;
            ConfigManager.GeneralConfig.ColumnListMINER = listViewAlgorithms.Columns[MINER].Width;
            ConfigManager.GeneralConfig.ColumnListGPU_clock = listViewAlgorithms.Columns[GPU_clock].Width;
            ConfigManager.GeneralConfig.ColumnListMem_clock = listViewAlgorithms.Columns[Mem_clock].Width;
            ConfigManager.GeneralConfig.ColumnListGPU_voltage = listViewAlgorithms.Columns[GPU_voltage].Width;
            ConfigManager.GeneralConfig.ColumnListMem_voltage = listViewAlgorithms.Columns[Mem_voltage].Width;
            ConfigManager.GeneralConfig.ColumnListPowerLimit = listViewAlgorithms.Columns[Power_limit].Width;
            ConfigManager.GeneralConfig.ColumnListFan = listViewAlgorithms.Columns[Fan].Width;
            //ConfigManager.GeneralConfig.ColumnListFanFlag = listViewAlgorithms.Columns[Fan_flag].Width;
            ConfigManager.GeneralConfig.ColumnListThermalLimit = listViewAlgorithms.Columns[Thermal_limit].Width;

        }

        private void listViewAlgorithms_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (ConfigManager.GeneralConfig.ColumnSort)
            {
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparerOverClock(2);
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparerOverClock(e.Column);
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
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparerOverClock(2);
                listViewAlgorithms.ListViewItemSorter = new ListViewColumnComparerOverClock(ConfigManager.GeneralConfig.ColumnListSort);
            }
        }

        private void AlgorithmsListViewOverClock_DoubleClick(object sender, EventArgs e)
        {

        }

        private void AlgorithmsListViewOverClock_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void listViewAlgorithms_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks > 1)
            {
                ListViewItem item = listViewAlgorithms.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    var c = item.Checked;
                    var CurrentSubItem = item.GetSubItemAt(e.X, e.Y);
                    int SubItembIndex = item.SubItems.IndexOf(CurrentSubItem);
                    if (SubItembIndex < 3) return;

                    TextBox tbox = new TextBox();
                    this.Controls.Add(tbox);
                    //MessageBox.Show(SubItembIndex.ToString());
                    int x_cord = 0;
                    for (int i = 0; i < SubItembIndex; i++)
                        x_cord += listViewAlgorithms.Columns[i].Width;


                    tbox.Width = listViewAlgorithms.Columns[SubItembIndex].Width;
                    tbox.Height = listViewAlgorithms.GetItemRect(0).Height - 2;
                    tbox.Left = x_cord;
                    tbox.Top = item.Position.Y;
                    tbox.Text = item.SubItems[SubItembIndex].Text;
                    tbox.TextAlign = listViewAlgorithms.Columns[SubItembIndex].TextAlign;
                    _SubItembIndex = SubItembIndex;
                    tbox.Leave += DisposeTextBox;
                    tbox.KeyPress += TextBoxKeyPress;
                    listViewAlgorithms.Controls.Add(tbox);
                    tbox.Focus();
                    //tbox.Select(tbox.Text.Length, 1);
                    tbox.SelectAll();
                    item.Checked = !c;
                    //                    ConfigManager.CommitBenchmarks();

                }
            }
        }
        private void DisposeTextBox(object sender, EventArgs e)
        {
            var tb = (sender as TextBox);
            double valuetb = 0.0;
            try
            {
                double.TryParse(tb.Text, out valuetb);
            }
            catch (Exception ex)
            {

            }

            bool outOfRange = false;
            if (e == null) outOfRange = true;
            Algorithm _algorithm = null;
            //tb.Text = valuetb.ToString();

            if (_computeDevice != null && e != null)
            {
                ControlMemoryGpuEntry _abdata = MSIAfterburner.GetDeviceData(_computeDevice.BusID);
                foreach (ListViewItem lvi in listViewAlgorithms.SelectedItems)
                {
                    if (lvi.Tag is Algorithm algorithm)
                    {
                        _algorithm = algorithm;
                        string fName = "temp\\" + _computeDevice.Uuid + "_" + _algorithm.AlgorithmStringID + ".tmp";
                        ControlMemoryGpuEntry _abdataTmp = MSIAfterburner.ReadFromFile(_computeDevice.BusID, fName);
                        //set current
                        //nvidia
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK_BOOST))
                        {
                            algorithm.gpu_clock = _abdataTmp.CoreClockBoostCur;
                        }
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE_BOOST))
                        {
                            algorithm.gpu_voltage = _abdataTmp.CoreVoltageBoostCur;
                        }
                        /*
                        if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED))
                        {
                            macm.GpuEntries[i].Flags = macm.GpuEntries[i].Flags - (int)MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED;
                        }
                        */
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK_BOOST))
                        {
                            algorithm.mem_clock = _abdataTmp.MemoryClockBoostCur;
                        }
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE_BOOST))
                        {
                            algorithm.mem_voltage = _abdataTmp.MemoryVoltageBoostCur;
                        }

                        //amd
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK))
                        {
                            algorithm.gpu_clock = (int)_abdataTmp.CoreClockCur;
                        }
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE))
                        {
                            algorithm.gpu_voltage = _abdataTmp.CoreVoltageCur;
                        }
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK))
                        {
                            algorithm.mem_clock = (int)_abdataTmp.MemoryClockCur;
                        }
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE))
                        {
                            algorithm.mem_voltage = _abdataTmp.MemoryVoltageCur;
                        }

                        //all
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.FAN_SPEED))
                        {
                            algorithm.fan = (int)_abdataTmp.FanSpeedCur;
                        }
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.POWER_LIMIT))
                        {
                            algorithm.power_limit = _abdataTmp.PowerLimitCur;
                        }
                        if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.THERMAL_LIMIT))
                        {
                            algorithm.thermal_limit = _abdataTmp.ThermalLimitCur;
                        }
                        //

                        if (_SubItembIndex == 3)
                        {
                            var min = _abdata.CoreClockBoostMin / 1000;
                            var max = _abdata.CoreClockBoostMax / 1000;
                            if ((int)valuetb < min || (int)valuetb > max)
                            {
                                MessageBox.Show(string.Format(International.GetText("FormSettings_AB_ValueWarning"), min, max));
                                outOfRange = true;
                            }
                            else
                            {
                                algorithm.gpu_clock = (int)valuetb * 1000;
                            }
                        }
                        if (_SubItembIndex == 4)
                        {
                            var min = _abdata.MemoryClockBoostMin / 1000;
                            var max = _abdata.MemoryClockBoostMax / 1000;
                            if ((int)valuetb < min || (int)valuetb > max)
                            {
                                MessageBox.Show(string.Format(International.GetText("FormSettings_AB_ValueWarning"), min, max));
                                outOfRange = true;
                            }
                            else
                            {
                                algorithm.mem_clock = (int)valuetb * 1000;
                            }
                        }
                        if (_SubItembIndex == 5)
                        {
                            var min = _abdata.CoreVoltageBoostMin;
                            var max = _abdata.CoreVoltageBoostMax;
                            if ((int)valuetb < min || (int)valuetb > max)
                            {
                                MessageBox.Show(string.Format(International.GetText("FormSettings_AB_ValueWarning"), min, max));
                                outOfRange = true;
                            }
                            else
                            {
                                algorithm.gpu_voltage = (int)valuetb;
                            }
                        }
                        if (_SubItembIndex == 6)
                        {
                            var min = _abdata.MemoryVoltageBoostMin;
                            var max = _abdata.MemoryVoltageBoostMax;
                            if ((int)valuetb < min || (int)valuetb > max)
                            {
                                MessageBox.Show(string.Format(International.GetText("FormSettings_AB_ValueWarning"), min, max));
                                outOfRange = true;
                            }
                            else
                            {
                                algorithm.mem_voltage = (int)valuetb;
                            }
                        }
                        if (_SubItembIndex == 7)
                        {
                            var min = _abdata.PowerLimitMin;
                            var max = _abdata.PowerLimitMax;
                            if ((int)valuetb < min || (int)valuetb > max)
                            {
                                MessageBox.Show(string.Format(International.GetText("FormSettings_AB_ValueWarning"), min, max));
                                outOfRange = true;
                            }
                            else
                            {
                                algorithm.power_limit = (int)valuetb;
                            }
                        }
                        if (_SubItembIndex == 8)
                        {
                            var min = _abdata.FanSpeedMin;
                            var max = _abdata.FanSpeedMax;
                            if ((int)valuetb < min || (int)valuetb > max)
                            {
                                MessageBox.Show(string.Format(International.GetText("FormSettings_AB_ValueWarning"), min, max));
                                outOfRange = true;
                            }
                            else
                            {
                                algorithm.fan = (int)valuetb;
                                algorithm.fan_flag = (int)MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None; 
                            }
                        }
                        //if (_SubItembIndex == 9) algorithm.fan_flag = (int)valuetb;
                        if (_SubItembIndex == 9)
                        {
                            var min = _abdata.ThermalLimitMin;
                            var max = _abdata.ThermalLimitMax;
                            if ((int)valuetb < min || (int)valuetb > max)
                            {
                                MessageBox.Show(string.Format(International.GetText("FormSettings_AB_ValueWarning"), min, max));
                                outOfRange = true;
                            }
                            else
                            {
                                algorithm.thermal_limit = (int)valuetb;
                            }
                        }

                        }

                }
            }
            if (_keyPressed != 27 && !outOfRange)
            {
                var item = listViewAlgorithms.GetItemAt(0, tb.Top + 1);
                if (item != null) item.SubItems[_SubItembIndex].Text = tb.Text;
                if (_algorithm != null)
                {
                    string fName = "temp\\" + _computeDevice.Uuid + "_" + _algorithm.AlgorithmStringID + ".tmp";
                    ControlMemoryGpuEntry _abdataTmp = MSIAfterburner.ReadFromFile(_computeDevice.BusID, fName);
                    //nvidia
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK_BOOST))
                    {
                        _abdataTmp.CoreClockBoostCur = _algorithm.gpu_clock;
                    }
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE_BOOST))
                    {
                        _abdataTmp.CoreVoltageBoostCur = (int)_algorithm.gpu_voltage;
                    }
                    /*
                    if (macm.GpuEntries[i].Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED))
                    {
                        macm.GpuEntries[i].Flags = macm.GpuEntries[i].Flags - (int)MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MACM_SHARED_MEMORY_GPU_ENTRY_FLAG_VF_CURVE_ENABLED;
                    }
                    */
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK_BOOST))
                    {
                        _abdataTmp.MemoryClockBoostCur = _algorithm.mem_clock;
                    }
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE_BOOST))
                    {
                        _abdataTmp.MemoryVoltageBoostCur = (int)_algorithm.mem_voltage;
                    }

                    //amd
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_CLOCK))
                    {
                        _abdataTmp.CoreClockCur = (uint)_algorithm.gpu_clock;
                    }
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.CORE_VOLTAGE))
                    {
                        _abdataTmp.CoreVoltageCur = (uint)_algorithm.gpu_voltage;
                    }
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_CLOCK))
                    {
                        _abdataTmp.MemoryClockCur = (uint)_algorithm.mem_clock;
                    }
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.MEMORY_VOLTAGE))
                    {
                        _abdataTmp.MemoryVoltageCur = (uint)_algorithm.mem_voltage;
                    }

                    //all
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.FAN_SPEED))
                    {
                        if (_algorithm.fan_flag == 0)
                        {
                            _abdataTmp.FanFlagsCur = MACM_SHARED_MEMORY_GPU_ENTRY_FAN_FLAG.None;
                            _abdataTmp.FanSpeedCur = (uint)_algorithm.fan;
                        }
                    }
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.POWER_LIMIT))
                    {
                        _abdataTmp.PowerLimitCur = _algorithm.power_limit;
                    }
                    if (_abdataTmp.Flags.HasFlag(MACM_SHARED_MEMORY_GPU_ENTRY_FLAG.THERMAL_LIMIT))
                    {
                        _abdataTmp.ThermalLimitCur = _algorithm.thermal_limit;
                    }
                    MSIAfterburner.SaveDeviceData(_abdataTmp, fName);
                }
            }

            tb.Dispose();
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            char DecSep = Convert.ToChar(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);
            char minus = (char)45;
            char inputChar = e.KeyChar;
            var text = sender as TextBox;
            int pos = text.SelectionStart;

            if (_SubItembIndex != 9 && inputChar == DecSep) inputChar = (char)0;

            if ((inputChar <= 47 || inputChar >= 58) &&
                inputChar != 8 &&
                inputChar != '-' &&
                inputChar != DecSep) 
            {
                e.Handled = true;
            }

            if (text.SelectionLength == text.Text.Length)
            {
                text.Text = "";
            }

            if (e.KeyChar == '-' && (sender as TextBox).Text.StartsWith("-"))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '-' && (sender as TextBox).SelectionStart > 1)
            {
                e.Handled = true;
            }
            
            if (text.Text.StartsWith(Convert.ToString(DecSep)))
            {
                // добавление лидирующего ноля
                text.Text = "0" + text.Text;
                text.SelectionStart = pos + 1;
            }
            if (inputChar == DecSep && text.Text.Contains(Convert.ToString(DecSep)))
            {
                e.Handled = true;
                return;
            }

            if (text.Text.StartsWith("-") && !text.Text.Contains("-"))
            {
                // добавление лидирующего -
                text.Text = "-" + text.Text;
                text.SelectionStart = text.Text.Length;
            }
           
            _keyPressed = inputChar;

            if (text.Text == "-" || text.Text == "0." || text.Text == "-.")
            {
                _keyPressed = (char)27;
            }
            
            if (inputChar == 13)
            {
                DisposeTextBox((sender as TextBox), null);
            }
            if (inputChar == 27)
                DisposeTextBox((sender as TextBox), null);
            
            //(sender as TextBox).Dispose();

        }
    }
    class ListViewColumnComparerOverClock : IComparer
    {
        public int ColumnIndex { get; set; }

        public ListViewColumnComparerOverClock(int columnIndex)
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
