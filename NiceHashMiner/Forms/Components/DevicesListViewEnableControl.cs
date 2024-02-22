using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Interfaces;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NiceHashMiner.Forms.Components
{
    public partial class DevicesListViewEnableControl : UserControl
    {
        private const int ENABLED = 0;
        private const int HASHRATE = 1;
        private const int TEMP = 2;
        private const int LOAD = 3;
        private const int FAN = 4;
        private const int POWER = 5;
        public static Color EnabledColor = Form_Main._backColor;
        //public static Color DisabledColor = ConfigManager.GeneralConfig.ColorProfileIndex != 0 ? Color.FromArgb(Form_Main._backColor.ToArgb() + 40 * 256 * 256 * 256 + 40 * 256 * 256 + 40 * 256 + 40) : Color.DarkGray;
        public static Color DisabledColor = Form_Main._backColor;
        public static Color DisabledForeColor = Color.Gray;
        //public static Color DisabledColor = SystemColors.ControlLight;

        public class DefaultDevicesColorSeter : IListItemCheckColorSetter
        {

            public void LviSetColor(ListViewItem lvi)
            {
                if (lvi.Tag is ComputeDevice cdvo)
                {
                    if (ConfigManager.GeneralConfig.ColorProfileIndex == 0)
                    {
                        lvi.BackColor = cdvo.Enabled ? SystemColors.ControlLightLight : SystemColors.ControlLightLight;
                        lvi.ForeColor = cdvo.Enabled ? Form_Main._foreColor : DisabledForeColor;
                    }
                    else
                    {
                        lvi.BackColor = cdvo.Enabled ? EnabledColor : DisabledColor;
                        lvi.ForeColor = cdvo.Enabled ? Form_Main._foreColor : DisabledForeColor;
                    }
                }
            }
        }

        private IListItemCheckColorSetter _listItemCheckColorSetter = new DefaultDevicesColorSeter();

        public IBenchmarkCalculation BenchmarkCalculation { get; set; }

        private AlgorithmsListView _algorithmsListView;
        private AlgorithmsListViewOverClock _algorithmsListViewOverClock;

        // disable checkboxes when in benchmark mode
        public static bool _isInBenchmark;

        // helper for benchmarking logic
        public bool IsInBenchmark
        {
            get => _isInBenchmark;
            set
            {
                if (value)
                {
                    _isInBenchmark = true;
                    listViewDevices.CheckBoxes = false;
                }
                else
                {
                    _isInBenchmark = false;
                    listViewDevices.CheckBoxes = true;
                }
            }
        }

        public static bool _isMining;

        public bool IsMining
        {
            get => _isMining;
            set
            {
                if (value)
                {
                    _isMining = true;
                    listViewDevices.CheckBoxes = false;
                }
                else
                {
                    _isMining = false;
                    listViewDevices.CheckBoxes = true;
                }
            }
        }

        public bool IsBenchmarkForm = false;
        public bool IsSettingsCopyEnabled = false;

        public string FirstColumnText
        {
            get => listViewDevices.Columns[ENABLED].Text;
            set
            {
                if (value != null) listViewDevices.Columns[ENABLED].Text = value;
            }
        }


        public bool SaveToGeneralConfig { get; set; }


        public DevicesListViewEnableControl()
        {
            InitializeComponent();

            listViewDevices.DoubleBuffer();
            DevicesListViewEnableControl.colorListViewHeader(ref listViewDevices, Form_Main._backColor, Form_Main._textColor);
            SaveToGeneralConfig = false;
            // intialize ListView callbacks
            listViewDevices.ItemChecked += ListViewDevicesItemChecked;
            this.listViewDevices.ItemCheck += new ItemCheckEventHandler(listViewDevices_ItemCheck);
            IsMining = false;
            BenchmarkCalculation = null;
            listViewDevices.MultiSelect = false;
            //  listViewDevices.OwnerDraw = true;
        }

        public void SetIListItemCheckColorSetter(IListItemCheckColorSetter listItemCheckColorSetter)
        {
            _listItemCheckColorSetter = listItemCheckColorSetter;
        }

        public void SetAlgorithmsListView(AlgorithmsListView algorithmsListView)
        {
            _algorithmsListView = algorithmsListView;
        }

        public void SetAlgorithmsListViewOverClock(AlgorithmsListViewOverClock algorithmsListViewOverClock)
        {
            _algorithmsListViewOverClock = algorithmsListViewOverClock;
        }

        public void ResetListItemColors()
        {
            foreach (ListViewItem lvi in listViewDevices.Items)
            {
                _listItemCheckColorSetter?.LviSetColor(lvi);
            }
        }
        public void SetComputeDevices(List<ComputeDevice> computeDevices, bool includeCPU = true)
        {
            // to not run callbacks when setting new
            var tmpSaveToGeneralConfig = SaveToGeneralConfig;
            SaveToGeneralConfig = false;
            listViewDevices.BeginUpdate();
            listViewDevices.Items.Clear();
            string addInfo = "";
            string Manufacturer = "";
            string GpuRam = "";
            string devNum = "";
            string devInfo = "";
            // set devices
            foreach (var computeDevice in computeDevices)
            {
                Manufacturer = "";
                if (computeDevice.DeviceType == DeviceType.CPU && !includeCPU)
                {
                    //continue;
                }
                devNum = computeDevice.NameCount;
                if (computeDevice.MonitorConnected && ConfigManager.GeneralConfig.Show_displayConected)
                {
                    devNum = "> " + devNum;//   > GPU
                }
                string NvidiaLHR = "";
                if (computeDevice.NvidiaLHR && ConfigManager.GeneralConfig.Show_NVIDIA_LHR)
                {
                    //NvidiaLHR = "(LHR)";
                }
                devInfo = computeDevice.Name + " " + NvidiaLHR;

                if (ConfigManager.GeneralConfig.Additional_info_about_device && computeDevice.DeviceType != DeviceType.CPU &&
                    computeDevice.Uuid.Length >= 8)
                {
                    addInfo = " (" + computeDevice.Uuid.Substring(computeDevice.Uuid.Length - 4, 4).ToUpper() + ")" +
                    " (BusID: " + computeDevice.BusID.ToString() + ")";
                }
                
                var lvi = new ListViewItem
                {
                    Checked = computeDevice.Enabled,
                    //Text = devNum + " " + Manufacturer + " " + devInfo + " " + GpuRam + " " + addInfo,
                    Text = devNum + " " + computeDevice.NameCustom.Replace("> ", "") + " " + addInfo,
                    Tag = computeDevice
                };
                //lvi.SubItems.Add(computeDevice.Name);
                listViewDevices.Items.Add(lvi);
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                _listItemCheckColorSetter.LviSetColor(lvi);
            }
            listViewDevices.EndUpdate();
            listViewDevices.Invalidate(true);
            // reset properties
            SaveToGeneralConfig = tmpSaveToGeneralConfig;
        }

        public void SetComputeDevicesStatus(List<ComputeDevice> computeDevices)
        {
            if (Form_ChooseLanguage.FormMainMoved || Form_API_keys.FormMainMoved || Form_Main.FormMainMoved || Form_RigProfitChart.FormChartMoved || Form_Settings.FormSettingsMoved || Form_Benchmark.FormBenchmarkMoved)
            {
                return;
            }
            int index = 0;
            var _computeDevices = ComputeDeviceManager.ReSortDevices(computeDevices);

            foreach (var computeDevice in _computeDevices)
            {
                //if (computeDevice == null || !computeDevice.Enabled) continue;

                string cHashrate = Helpers.FormatDualSpeedOutput(computeDevice.MiningHashrate, computeDevice.MiningHashrateSecond,
                computeDevice.MiningHashrateThird, (AlgorithmType)computeDevice.AlgorithmID,
                (AlgorithmType)computeDevice.SecondAlgorithmID, (AlgorithmType)computeDevice.ThirdAlgorithmID);
                if (computeDevice.MiningHashrate + computeDevice.MiningHashrateSecond + computeDevice.MiningHashrateThird == 0)
                {
                    cHashrate = "--";
                }
                string cTemp = "";
                if (computeDevice.TempMemory > 0 )
                {
                    cTemp = Math.Truncate(computeDevice.Temp).ToString() + "°/" + Math.Truncate(computeDevice.TempMemory).ToString() + "° C";
                }
                else
                {
                    cTemp = Math.Truncate(computeDevice.Temp).ToString() + "°C";
                }    

                string cLoad = Math.Truncate(computeDevice.Load).ToString() + "%";
                string cFanSpeed = "";

                if (ConfigManager.GeneralConfig.ShowFanAsPercent)
                {
                        cFanSpeed = computeDevice.FanSpeed.ToString() + "%";
                }
                else
                {
                    cFanSpeed = computeDevice.FanSpeedRPM.ToString();
                }
                double _PowerUsage = computeDevice.PowerUsage;
                string cPowerUsage = Math.Truncate(_PowerUsage).ToString();

                if (Math.Truncate(_PowerUsage) == 0)
                {
                    cPowerUsage = "-1";
                }
                if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
                {
                    cPowerUsage = cPowerUsage + " Вт";
                }
                else
                {
                    cPowerUsage = cPowerUsage + " W";
                }
                try
                {
                    if (listViewDevices.Items.Count <= 0) return;
                    if (index >= 0)
                    {
                        if (Form_Benchmark.BenchmarkStarted)
                        {
                            listViewDevices.Items[index].SubItems[1].Text = "--";
                        }
                        else
                        {
                            listViewDevices.Items[index].SubItems[1].Text = cHashrate;
                            //listViewDevices.Items[index].SubItems[1].Text = cHashrate.Contains("0.00") ? "--" : cHashrate;
                        }
                        listViewDevices.Items[index].Checked = computeDevice.Enabled;
                        listViewDevices.Items[index].SubItems[2].Text = cTemp.Contains("-1") ? "--" : cTemp;
                        listViewDevices.Items[index].SubItems[3].Text = cLoad.Contains("-1") ? "--" : cLoad;
                        listViewDevices.Items[index].SubItems[4].Text = cFanSpeed.Contains("-1") ? "--" : cFanSpeed;
                        listViewDevices.Items[index].SubItems[POWER].Text = cPowerUsage.Contains("-1") ? "--" : cPowerUsage;
                    }
                    index++;
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("SetComputeDevicesStatus", e.ToString());
                }
                finally
                {

                }
            }
        }
        public void ResetComputeDevices(List<ComputeDevice> computeDevices)
        {
            SetComputeDevices(computeDevices);
        }
        //List view header formatters
        public static void colorListViewHeader(ref ListView list, Color backColor, Color foreColor)
        {

            list.OwnerDraw = true;
            list.DrawColumnHeader +=
            new DrawListViewColumnHeaderEventHandler
            (
            (sender, e) => headerDraw(sender, e, backColor, foreColor)
            );
            list.DrawItem += new DrawListViewItemEventHandler(bodyDraw);
            list.Columns[TEMP].TextAlign = HorizontalAlignment.Center; //не работает

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
            //lvi.BackColor = cdvo.Enabled ? EnabledColor : DisabledColor;
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
                foreach (var lbl in this.Controls.OfType<ListView>()) lbl.BackColor = _backColor;
                listViewDevices.BackColor = _backColor;
                listViewDevices.ForeColor = _textColor;

                this.BackColor = _backColor;
            }
            else
            {
                foreach (var lbl in this.Controls.OfType<ListView>()) lbl.BackColor = SystemColors.ControlLightLight;
                listViewDevices.BackColor = SystemColors.ControlLightLight;
                listViewDevices.ForeColor = _textColor;

                this.BackColor = SystemColors.ControlLightLight;
            }

            listViewDevices.Columns[ENABLED].Text = " " + International.GetText("ListView_Device");

            listViewDevices.Columns[HASHRATE].Text = International.GetText("Form_Main_device_hashrate");
            listViewDevices.Columns[TEMP].Text = International.GetText("Form_Main_device_temp");
            listViewDevices.Columns[LOAD].Text = International.GetText("Form_Main_device_load");
            listViewDevices.Columns[FAN].Text = International.GetText("Form_Main_device_fan");
            listViewDevices.Columns[POWER].Text = International.GetText("Form_Main_device_power");

            listViewDevices.Columns[HASHRATE].Width = 0;
            listViewDevices.Columns[TEMP].Width = 0;
            listViewDevices.Columns[TEMP].TextAlign = HorizontalAlignment.Center; //не работает
            listViewDevices.Columns[LOAD].Width = 0;
            listViewDevices.Columns[FAN].Width = 0;
            listViewDevices.Columns[POWER].Width = 0;
            //listViewDevices.Columns[0].Width = Width - 4 - SystemInformation.VerticalScrollBarWidth;
            //listViewDevices.Columns[0].Width = Width - SystemInformation.VerticalScrollBarWidth;
            listViewDevices.Columns[0].Width = Width - 4;

        }

        public void InitLocaleMain()
        {
            var _backColor = Form_Main._backColor;
            var _foreColor = Form_Main._foreColor;
            var _textColor = Form_Main._textColor;
            // foreach (var lbl in this.Controls.OfType<ListView>()) lbl.BackColor = _backColor;
            //  listViewDevices.BackColor = _backColor;
            // listViewDevices.ForeColor = _textColor;
            // this.BackColor = _backColor;
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                foreach (var lbl in this.Controls.OfType<ListView>()) lbl.BackColor = _backColor;
                listViewDevices.BackColor = _backColor;
                listViewDevices.ForeColor = _textColor;

                this.BackColor = _backColor;
            }
            else
            {
                foreach (var lbl in this.Controls.OfType<ListView>()) lbl.BackColor = SystemColors.ControlLightLight;
                listViewDevices.BackColor = SystemColors.ControlLightLight;
                listViewDevices.ForeColor = _textColor;

                this.BackColor = SystemColors.ControlLightLight;
            }


            listViewDevices.Columns[ENABLED].Text = " " + International.GetText("ListView_Device");

            listViewDevices.Columns[HASHRATE].Text = International.GetText("Form_Main_device_hashrate");
            listViewDevices.Columns[TEMP].Text = International.GetText("Form_Main_device_temp");
            listViewDevices.Columns[LOAD].Text = International.GetText("Form_Main_device_load");
            listViewDevices.Columns[FAN].Text = International.GetText("Form_Main_device_fan");
            listViewDevices.Columns[POWER].Text = International.GetText("Form_Main_device_power");

            listViewDevices.Columns[ENABLED].Width = ConfigManager.GeneralConfig.ColumnENABLED;
            listViewDevices.Columns[HASHRATE].Width = ConfigManager.GeneralConfig.ColumnHASHRATE;
            listViewDevices.Columns[TEMP].Width = ConfigManager.GeneralConfig.ColumnTEMP;
            listViewDevices.Columns[LOAD].Width = ConfigManager.GeneralConfig.ColumnLOAD;
            listViewDevices.Columns[FAN].Width = ConfigManager.GeneralConfig.ColumnFAN;
            listViewDevices.Columns[POWER].Width = ConfigManager.GeneralConfig.ColumnPOWER;
            //  listViewDevices.Scrollable = true;
        }
        public void SaveColumns()
        {
            // if (listViewDevices.Columns[ENABLED] != null)
            if (listViewDevices.Columns[HASHRATE].Width + listViewDevices.Columns[TEMP].Width + listViewDevices.Columns[LOAD].Width + listViewDevices.Columns[FAN].Width + listViewDevices.Columns[POWER].Width > 0)
            {
                ConfigManager.GeneralConfig.ColumnENABLED = listViewDevices.Columns[ENABLED].Width;
                ConfigManager.GeneralConfig.ColumnTEMP = listViewDevices.Columns[TEMP].Width;
                ConfigManager.GeneralConfig.ColumnHASHRATE = listViewDevices.Columns[HASHRATE].Width;
                ConfigManager.GeneralConfig.ColumnLOAD = listViewDevices.Columns[LOAD].Width;
                ConfigManager.GeneralConfig.ColumnFAN = listViewDevices.Columns[FAN].Width;
                ConfigManager.GeneralConfig.ColumnPOWER = listViewDevices.Columns[POWER].Width;
            }
        }

        private void ListViewDevicesItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Tag is ComputeDevice cDevice)
            {
                cDevice.Enabled = e.Item.Checked;

                if (SaveToGeneralConfig)
                {
                    ConfigManager.GeneralConfigFileCommit();
                }
                if (e.Item is ListViewItem lvi) _listItemCheckColorSetter.LviSetColor(lvi);
                _algorithmsListView?.RepaintStatus(cDevice.Enabled, cDevice.Uuid);
            }
            //BenchmarkCalculation?.CalcBenchmarkDevicesAlgorithmQueue();
        }

        public void SetDeviceSelectionChangedCallback(ListViewItemSelectionChangedEventHandler callback)
        {
            listViewDevices.ItemSelectionChanged += callback;
        }

        private void ListViewDevices_MouseClick(object sender, MouseEventArgs e)
        {
            string Manufacturer = "";
            if (IsInBenchmark) return;
            if (IsMining) return;
            if (ConfigManager.GeneralConfig.DeviceDetection.DisableDetectionCPU)
            {
            }

            Control senderObject = sender as Control;
            string hoveredControl = senderObject.TopLevelControl.Name;

            if (e.Button == MouseButtons.Right && !hoveredControl.Contains("Form_Main"))
            {
                if (listViewDevices.FocusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuStrip1.Items.Clear();
                    try
                    {
                        string tName = "";
                        try
                        {
                            tName = Form_Main.settings.tabControlGeneral.SelectedTab.Name;
                        } catch (Exception)
                        {
                            tName = "Form_Benchmark";
                        }
                        if (listViewDevices.FocusedItem.Tag is ComputeDevice cDevice)
                        {
                            var sameDevTypes =
                                ComputeDeviceManager.Available.GetSameDevicesTypeAsDeviceWithUuid(cDevice.Uuid);
                            if (sameDevTypes.Count > 0)
                            {
                                var copyBenchItem = new ToolStripMenuItem();
                                var copyTuningItem = new ToolStripMenuItem();
                                var copyOverClockItem = new ToolStripMenuItem();
                                foreach (var cDev in sameDevTypes)
                                {
                                    Manufacturer = "";
                                    if (cDev.Enabled)
                                    {
                                        string devInfo = cDev.Name;
                                        string GpuRam = "";
                                        if (cDev.DeviceType == DeviceType.NVIDIA)
                                        {
                                            if (ConfigManager.GeneralConfig.Show_NVdevice_manufacturer)
                                            {
                                                devInfo = devInfo.Replace("NVIDIA", "");
                                                if (!devInfo.Contains(ComputeDevice.GetManufacturer(cDev.Manufacturer)))
                                                {
                                                    Manufacturer = ComputeDevice.GetManufacturer(cDev.Manufacturer);
                                                }
                                            }
                                            else
                                            {
                                                devInfo = devInfo.Replace(ComputeDevice.GetManufacturer(cDev.Manufacturer) + " ", "");
                                                if (!devInfo.Contains("NVIDIA")) devInfo = "NVIDIA " + devInfo;
                                            }

                                            GpuRam = (cDev.GpuRam / 1073741824).ToString() + "GB";
                                            if (ConfigManager.GeneralConfig.Show_ShowDeviceMemSize)
                                            {
                                                if (devInfo.Contains(GpuRam))
                                                {
                                                    GpuRam = "";
                                                }
                                            }
                                            else
                                            {
                                                devInfo = devInfo.Replace(GpuRam, "");
                                                GpuRam = "";
                                            }
                                        }

                                        if (cDev.DeviceType == DeviceType.AMD)
                                        {
                                            if (ConfigManager.GeneralConfig.Show_AMDdevice_manufacturer)
                                            {
                                                if (!devInfo.Contains(ComputeDevice.GetManufacturer(cDev.Manufacturer)))
                                                {
                                                    Manufacturer = ComputeDevice.GetManufacturer(cDev.Manufacturer);
                                                }
                                            }
                                            else
                                            {
                                                devInfo = devInfo.Replace(ComputeDevice.GetManufacturer(cDev.Manufacturer) + " ", "");
                                            }

                                            GpuRam = (cDev.GpuRam / 1073741824).ToString() + "GB";
                                            if (ConfigManager.GeneralConfig.Show_ShowDeviceMemSize)
                                            {
                                                if (devInfo.Contains(GpuRam))
                                                {
                                                    GpuRam = "";
                                                }
                                            }
                                            else
                                            {
                                                devInfo = devInfo.Replace(GpuRam, "");
                                                GpuRam = "";
                                            }
                                        }
                                        
                                        if (!tName.Equals("tabPageOverClock") || hoveredControl.Contains("Form_Benchmark"))
                                        {
                                            var copyBenchDropDownItem = new ToolStripMenuItem
                                            {
                                                Text = (cDev.NameCount).ToString() + " " + Manufacturer + " " + devInfo,
                                                Checked = cDev.Uuid == cDevice.BenchmarkCopyUuid
                                            };
                                            copyBenchDropDownItem.Click += ToolStripMenuItemCopySettings_Click;
                                            copyBenchDropDownItem.Tag = cDev.Uuid;
                                            copyBenchItem.DropDownItems.Add(copyBenchDropDownItem);

                                            var copyTuningDropDownItem = new ToolStripMenuItem
                                            {
                                                Text = (cDev.NameCount).ToString() + " " + Manufacturer + " " + devInfo
                                            };
                                            copyTuningDropDownItem.Click += ToolStripMenuItemCopyTuning_Click;
                                            copyTuningDropDownItem.Tag = cDev.Uuid;
                                            copyTuningItem.DropDownItems.Add(copyTuningDropDownItem);

                                            copyBenchItem.Text = International.GetText("DeviceListView_ContextMenu_CopySettings");
                                            copyTuningItem.Text = International.GetText("DeviceListView_ContectMenu_CopyTuning");
                                            contextMenuStrip1.Items.Add(copyBenchItem);
                                            contextMenuStrip1.Items.Add(copyTuningItem);
                                        }
                                        else
                                        {
                                            if (Form_Main.OverclockEnabled)
                                            {
                                                var copyOverclockDropDownItem = new ToolStripMenuItem
                                                {
                                                    Text = (cDev.NameCount).ToString() + " " + Manufacturer + " " + devInfo,
                                                };
                                                copyOverclockDropDownItem.Click += ToolStripMenuItemCopyOverclock_Click;
                                                copyOverclockDropDownItem.Tag = cDev.Uuid;
                                                copyOverClockItem.DropDownItems.Add(copyOverclockDropDownItem);
                                                copyOverClockItem.Text = International.GetText("DeviceListView_ContextMenu_CopyOverClock");
                                                contextMenuStrip1.Items.Add(copyOverClockItem);
                                            }
                                        }
                                    }
                                }

                            } else
                            {
                                MessageBox.Show(International.GetText("FormSettings_NoSameDevices"), "Stop");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("ListViewDevices_MouseClick", ex.ToString());
                    }
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void ToolStripMenuItem_Click(object sender, bool justTuning)
        {
            if (sender is ToolStripMenuItem item && item.Tag is string uuid
                && listViewDevices.FocusedItem.Tag is ComputeDevice CDevice)
            {
                var copyBenchCDev = ComputeDeviceManager.Available.GetDeviceWithUuid(uuid);

                var result = MessageBox.Show(
                    string.Format(
                        International.GetText("DeviceListView_ContextMenu_CopySettings_Confirm_Dialog_Msg"),
                        copyBenchCDev.GetFullName(), CDevice.GetFullName()),
                    International.GetText("DeviceListView_ContextMenu_CopySettings_Confirm_Dialog_Title"),
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    if (justTuning)
                    {
                        CDevice.BenchmarkCopyUuid = uuid;
                        CDevice.CopyTuningSettingsFrom(copyBenchCDev);
                    }
                    else
                    {
                        CDevice.BenchmarkCopyUuid = uuid;
                        CDevice.CopyBenchmarkSettingsFrom(copyBenchCDev);
                    }
                    if (_algorithmsListView != null)
                    {
                        _algorithmsListView.Update();
                        _algorithmsListView.Refresh();
                        _algorithmsListView.RepaintStatus(CDevice.Enabled, CDevice.Uuid);
                    }
                }
            }
        }
        private void ToolStripMenuItem_ClickOverclock(object sender)
        {
            if (sender is ToolStripMenuItem item && item.Tag is string uuid
                && listViewDevices.FocusedItem.Tag is ComputeDevice CDevice)
            {
                var copyOverclockCDevFrom = ComputeDeviceManager.Available.GetDeviceWithUuid(uuid);

                var result = MessageBox.Show(
                    string.Format(
                        International.GetText("DeviceListView_ContextMenu_CopySettings_Confirm_Dialog_Msg"),
                        copyOverclockCDevFrom.GetFullName() + "\r\n", CDevice.GetFullName()),
                    International.GetText("DeviceListView_ContextMenu_CopySettings_Confirm_Dialog_Title"),
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    CDevice.BenchmarkCopyUuid = uuid;
                    CDevice.CopyOverclockSettingsFrom(copyOverclockCDevFrom, CDevice);

                    if (_algorithmsListViewOverClock != null)
                    {
                        _algorithmsListViewOverClock.SetAlgorithms(CDevice, CDevice.Enabled);
                        _algorithmsListViewOverClock.Update();
                        _algorithmsListViewOverClock.Refresh();
                        _algorithmsListViewOverClock.RepaintStatus(CDevice.Enabled, CDevice.Uuid);
                    }
                }
            }
        }
        private void ToolStripMenuItemCopyOverclock_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem_ClickOverclock(sender);
        }
        private void ToolStripMenuItemCopySettings_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem_Click(sender, false);
        }

        private void ToolStripMenuItemCopyTuning_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem_Click(sender, true);
        }

        private void DevicesListViewEnableControl_Resize(object sender, EventArgs e)
        {
            //ResizeColumn();
            listViewDevices.BeginUpdate();
            ResizeAutoSizeColumn(listViewDevices, 0);
            listViewDevices.EndUpdate();
            // only one
            foreach (ColumnHeader ch in listViewDevices.Columns)
            {
                //  ch.Width = Width;
            }
        }

        public void SetFirstSelected()
        {
            if (listViewDevices.Items.Count > 0)
            {
                listViewDevices.Items[0].Selected = true;
                listViewDevices.Select();
            }
        }

        private void listViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  CheckBox checkbox = (CheckBox)sender;
        }

        private void listViewDevices_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            /*
            CheckBox test = sender as CheckBox;

            for (int i = 0; i < listViewDevices.Items.Count; i++)
            {
                listViewDevices.Items[i].BackColor = DisabledColor;
            }
            */
        }
        private void listViewDevices_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            /*
            if ((e.ColumnIndex == 0))
            {
                CheckBox cck = new CheckBox();
                // With...
                Text = "";
                Visible = true;
                listViewDevices.SuspendLayout();
                e.DrawBackground();
                cck.BackColor = Form_Main._backColor;
                //cck.UseVisualStyleBackColor = true;

                cck.SetBounds(e.Bounds.X, e.Bounds.Y, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width, cck.GetPreferredSize(new Size(e.Bounds.Width, e.Bounds.Height)).Width);
                cck.Size = new Size((cck.GetPreferredSize(new Size((e.Bounds.Width - 1), e.Bounds.Height)).Width + 1), e.Bounds.Height);
                cck.Location = new Point(3, 0);
                listViewDevices.Controls.Add(cck);
                cck.Show();
                cck.BringToFront();
                e.DrawText((TextFormatFlags.VerticalCenter | TextFormatFlags.VerticalCenter));
                cck.Click += new EventHandler(Bink);
                listViewDevices.ResumeLayout(true);
            }
            else
            {
                e.DrawDefault = true;
            }

            var with1 = e.Graphics;
            with1.DrawLines(new Pen(Color.Green), new Point[] { new Point(e.Bounds.Left + e.Bounds.Width, e.Bounds.Top - 1), new Point(e.Bounds.Left + e.Bounds.Width, e.Bounds.Top + e.Bounds.Height) });
            e.DrawText();
            */
        }

        private void listViewDevices_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            listViewDevices.BeginUpdate();

            if (e.ColumnIndex == 5)
            {
                ResizeAutoSizeColumn(listViewDevices, 0);
            }
            if (e.ColumnIndex == 0)
            {
                ResizeAutoSizeColumn(listViewDevices, 5);
            }
            //  ResizeAutoSizeColumn(listViewDevices, 0);

            listViewDevices.EndUpdate();
            listViewDevices.Update();
            listViewDevices.Refresh();


            //   ResizeColumn();
        }
        /*
        private void ResizeColumn()
        {
            listViewDevices.BeginUpdate();
            listViewDevices.Columns[4].Width = -2; //magic
            listViewDevices.EndUpdate();
        }
        */
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

        private void listViewDevices_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {

            //    var with1 = e.Graphics;
            //  with1.DrawLines(new Pen(Color.Green), new Point[] {/*new Point(e.Bounds.Left, e.Bounds.Top - 1),*/new Point(e.Bounds.Left + e.Bounds.Width, e.Bounds.Top - 1), new Point(e.Bounds.Left + e.Bounds.Width, e.Bounds.Top + e.Bounds.Height)/*,new Point(e.Bounds.Left, e.Bounds.Top + e.Bounds.Height)*/});
            // e.DrawText();

        }

        private void listViewDevices_SizeChanged(object sender, EventArgs e)
        {
            //   ResizeAutoSizeColumn(listViewDevices, 0);
        }

        private void listViewDevices_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (listViewDevices.Columns[HASHRATE].Width + listViewDevices.Columns[TEMP].Width + listViewDevices.Columns[LOAD].Width + listViewDevices.Columns[FAN].Width + listViewDevices.Columns[POWER].Width > 0)
            {
                ConfigManager.GeneralConfig.ColumnENABLED = listViewDevices.Columns[ENABLED].Width;
                ConfigManager.GeneralConfig.ColumnHASHRATE = listViewDevices.Columns[HASHRATE].Width;
                ConfigManager.GeneralConfig.ColumnTEMP = listViewDevices.Columns[TEMP].Width;
                ConfigManager.GeneralConfig.ColumnLOAD = listViewDevices.Columns[LOAD].Width;
                ConfigManager.GeneralConfig.ColumnFAN = listViewDevices.Columns[FAN].Width;
                ConfigManager.GeneralConfig.ColumnPOWER = listViewDevices.Columns[POWER].Width;
            }
        }

        private void listViewDevices_MouseHover(object sender, EventArgs e)
        {
            if (ConfigManager.GeneralConfig.DisableTooltips)
            {
                return;
            }
            Control senderObject = sender as Control;
            string hoveredControl = senderObject.TopLevelControl.Name;

            if (!hoveredControl.Contains("Form_Main"))
            {
                ToolTip toolTip1 = new ToolTip();
                toolTip1.AutoPopDelay = 5000;
                toolTip1.InitialDelay = 1000;
                toolTip1.ReshowDelay = 500;
                // Force the ToolTip text to be displayed whether or not the form is active.
                toolTip1.ShowAlways = true;
                toolTip1.IsBalloon = true;
                toolTip1.SetToolTip(this.listViewDevices, International.GetText("listViewDevices_ToolTip"));
            }
        }
    }
    public static class ControlExtensions
    {
        public static void DoubleBuffer(this Control control)
        {
            System.Reflection.PropertyInfo dbProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dbProp.SetValue(control, true, null);
        }
    }
}
