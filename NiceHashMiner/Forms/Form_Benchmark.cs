using NiceHashMiner.Algorithms;
using NiceHashMiner.Benchmarking;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Interfaces;
using NiceHashMiner.Miners;
using NiceHashMiner.Properties;
using NiceHashMiner.Utils;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace NiceHashMiner.Forms
{
    public partial class Form_Benchmark : Form, IListItemCheckColorSetter, IBenchmarkForm, IBenchmarkCalculation
    {
        // private static readonly Color DisabledColor = Color.FromArgb(Form_Main._backColor.ToArgb() + 40*256*256*256 + 40*256*256 + 40*256 + 40);
        public static Color DisabledColor = Form_Main._backColor;
        public static Color DisabledForeColor = Color.Gray;
        private static readonly Color BenchmarkedColor = Form_Main._backColor;
        private static readonly Color UnbenchmarkedColor = Color.LightBlue;

        public static AlgorithmBenchmarkSettingsType _algorithmOption =
            AlgorithmBenchmarkSettingsType.SelectedUnbenchmarkedAlgorithms;

        public static int _bechmarkCurrentIndex = 0;
        public static int _benchmarkAlgorithmsCount = 0;

        private List<Tuple<ComputeDevice, Queue<Algorithm>>> _benchmarkDevicesAlgorithmQueue;

        private Dictionary<string, BenchmarkSettingsStatus> _benchmarkDevicesAlgorithmStatus;
        //private AlgorithmType _singleBenchmarkType = AlgorithmType.NONE;

        private readonly Timer _benchmarkingTimer;
        public int _dotCount;

        private bool _hasFailedAlgorithms;
        private List<BenchmarkHandler> _runningBenchmarkThreads = new List<BenchmarkHandler>();
        private Dictionary<ComputeDevice, Algorithm> _statusCheckAlgos;

        private readonly bool ExitWhenFinished;

        public bool StartMining { get; private set; }

        public bool InBenchmark { get; private set; }
        public static bool BenchmarkStarted = false;

        public string benchmarkfail = "";
        private static Timer UpdateListView_timer;
        public static bool FormBenchmarkMoved = false;
        private ComputeDevice _selectedComputeDevice;
        private bool benchmarkRepeat = false;
        public Form_Benchmark(BenchmarkPerformanceType benchmarkPerformanceType = BenchmarkPerformanceType.Standard,
            bool autostart = false)
        {
            InitializeComponent();
            Icon = Resources.logo;
            Algorithm.BenchmarkActive = true;
            StartMining = false;

            // set first device selected 
            if (ComputeDeviceManager.Available.Devices.Count > 0)
            {
                _selectedComputeDevice = ComputeDeviceManager.Available.Devices[0];
                algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled, true);
            }


            // clear prev pending statuses
            foreach (var dev in ComputeDeviceManager.Available.Devices)
            {
                foreach (var algo in dev.GetAlgorithmSettings())
                    algo.ClearBenchmarkPendingFirst();
            }
            benchmarkOptions1.SetPerformanceType(benchmarkPerformanceType);

            // benchmark only unique devices
            devicesListViewEnableControl1.SetIListItemCheckColorSetter(this);
            devicesListViewEnableControl1.SetComputeDevices(ComputeDeviceManager.Available.Devices);

            InitLocale();
            Rectangle screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            if (ConfigManager.GeneralConfig.BenchmarkFormLeft + ConfigManager.GeneralConfig.BenchmarkFormWidth <= screenSize.Size.Width &&
                ConfigManager.GeneralConfig.BenchmarkFormTop + ConfigManager.GeneralConfig.BenchmarkFormHeight <= screenSize.Size.Height)
            {
                if (ConfigManager.GeneralConfig.BenchmarkFormTop + ConfigManager.GeneralConfig.BenchmarkFormLeft != 0)
                {
                    this.Top = ConfigManager.GeneralConfig.BenchmarkFormTop;
                    this.Left = ConfigManager.GeneralConfig.BenchmarkFormLeft;
                }
                else
                {
                    this.StartPosition = FormStartPosition.CenterScreen;
                }
                this.Width = ConfigManager.GeneralConfig.BenchmarkFormWidth;
                this.Height = ConfigManager.GeneralConfig.BenchmarkFormHeight;
            }
            else
            {
                this.Top = 0;
                this.Left = 0;
            }

            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                this.BackColor = Form_Main._backColor;
                this.ForeColor = Form_Main._foreColor;

                foreach (var lbl in this.Controls.OfType<Label>()) lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<LinkLabel>()) lbl.LinkColor = Color.LightBlue;

                foreach (var lbl in this.Controls.OfType<GroupBox>()) lbl.BackColor = Form_Main._backColor;

                foreach (var lbl in this.Controls.OfType<HScrollBar>())
                    lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<ListBox>()) lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<ListControl>()) lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<ListView>()) lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<ListView>()) lbl.ForeColor = Form_Main._textColor;
                foreach (var lbl in this.Controls.OfType<ListViewItem>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._textColor;
                }
                foreach (var lbl in this.Controls.OfType<StatusBar>())
                    lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<ComboBox>()) lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<ComboBox>()) lbl.ForeColor = Form_Main._foreColor;

                foreach (var lbl in this.Controls.OfType<GroupBox>()) lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<GroupBox>()) lbl.ForeColor = Form_Main._textColor;
                // foreach (var lbl in this.Controls.OfType<ComboBox>()) lbl.ForeColor = _foreColor;

                foreach (var lbl in this.Controls.OfType<TextBox>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._foreColor;
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                }
                foreach (var lbl in this.Controls.OfType<StatusStrip>()) lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<StatusStrip>()) lbl.ForeColor = Form_Main._foreColor;
                foreach (var lbl in this.Controls.OfType<ToolStripStatusLabel>()) lbl.BackColor = Form_Main._backColor;
                foreach (var lbl in this.Controls.OfType<ToolStripStatusLabel>()) lbl.ForeColor = Form_Main._foreColor;

                foreach (var lbl in this.Controls.OfType<Button>()) lbl.BackColor = Form_Main._backColor;

                foreach (var lbl in this.Controls.OfType<Button>())
                {
                    lbl.BackColor = Form_Main._backColor;
                    lbl.ForeColor = Form_Main._textColor;
                    lbl.FlatStyle = FlatStyle.Flat;
                    lbl.FlatAppearance.BorderColor = Form_Main._textColor;
                    lbl.FlatAppearance.BorderSize = 1;
                }

                // Form_Benchmark.ActiveForm.Enabled = true;


                foreach (var lbl in this.Controls.OfType<CheckBox>()) lbl.BackColor = Form_Main._backColor;
                // DevicesListViewEnableControl.listViewDevices.BackColor = _backColor;
                devicesListViewEnableControl1.BackColor = Form_Main._backColor;
                devicesListViewEnableControl1.ForeColor = Form_Main._foreColor;
                algorithmsListView1.BackColor = Form_Main._backColor;
                algorithmsListView1.ForeColor = Form_Main._foreColor;
                //DevicesListViewEnableControl.DefaultDevicesColorSeter.
                //   DevicesListViewEnableControl.DefaultDevicesColorSeter.EnabledColor = _backColor;
                //  devicesListViewEnableControl1.listViewDevices.Items[0].UseItemStyleForSubItems = false;


            }

            _benchmarkingTimer = new Timer();
            _benchmarkingTimer.Tick += BenchmarkingTimer_Tick;
            _benchmarkingTimer.Interval = 500;


            //Dictionary<string, string> benchNamesUUIDs = new Dictionary<string, string>();
            //// name, UUID
            //Dictionary<string, string> benchNamesUUIDs = new Dictionary<string, string>();
            //// initialize benchmark settings for same cards to only copy settings
            //foreach (var cDev in ComputeDeviceManager.Available.Devices) {
            //    var plainDevName = cDev.Name;
            //    if (benchNamesUUIDs.ContainsKey(plainDevName)) {
            //        cDev.Enabled = false;
            //        cDev.BenchmarkCopyUUID = benchNamesUUIDs[plainDevName];
            //    } else if (cDev.Enabled == true) {
            //        benchNamesUUIDs.Add(plainDevName, cDev.UUID);
            //        //cDev.Enabled = true; // enable benchmark
            //        cDev.BenchmarkCopyUUID = null;
            //    }
            //}

            //groupBoxAlgorithmBenchmarkSettings.Enabled = _singleBenchmarkType == AlgorithmType.NONE;
            devicesListViewEnableControl1.Enabled = true;
            devicesListViewEnableControl1.SetDeviceSelectionChangedCallback(DevicesListView1_ItemSelectionChanged);

            devicesListViewEnableControl1.SetAlgorithmsListView(algorithmsListView1);
            devicesListViewEnableControl1.IsBenchmarkForm = true;
            devicesListViewEnableControl1.IsSettingsCopyEnabled = true;

            ResetBenchmarkProgressStatus();
            CalcBenchmarkDevicesAlgorithmQueue();
            devicesListViewEnableControl1.ResetListItemColors();

            // to update laclulation status
            devicesListViewEnableControl1.BenchmarkCalculation = this;
            algorithmsListView1.BenchmarkCalculation = this;

            // set first device selected {
            if (ComputeDeviceManager.Available.Devices.Count > 0)
            {
                var firstComputedevice = ComputeDeviceManager.Available.Devices[0];
                algorithmsListView1.SetAlgorithms(firstComputedevice, firstComputedevice.Enabled);
            }

            if (autostart)
            {
                ExitWhenFinished = true;
                StartStopBtn_Click(null, null);
            }

            if (UpdateListView_timer == null)
            {
                UpdateListView_timer = new Timer();
                UpdateListView_timer.Tick += UpdateLvi_Tick;
                UpdateListView_timer.Interval = 200;
                UpdateListView_timer.Start();
            }
            if (ConfigManager.GeneralConfig.AlwaysOnTop) this.TopMost = true;

            progressBarBenchmarkSteps.Maximum = 0;
            progressBarBenchmarkSteps.Value = 0;
            SetLabelBenchmarkSteps(0, 0);
        }

        private void UpdateLvi_Tick(object sender, EventArgs e)
        {
            algorithmsListView1.UpdateLvi();
        }


        #region IBenchmarkCalculation methods

        public void CalcBenchmarkDevicesAlgorithmQueue()
        {
            _benchmarkAlgorithmsCount = 0;
            _benchmarkDevicesAlgorithmStatus = new Dictionary<string, BenchmarkSettingsStatus>();
            _benchmarkDevicesAlgorithmQueue = new List<Tuple<ComputeDevice, Queue<Algorithm>>>();
            foreach (var cDev in ComputeDeviceManager.Available.Devices)
            {
                var algorithmQueue = new Queue<Algorithm>();
                foreach (var algo in cDev.GetAlgorithmSettings())
                    if (ShoulBenchmark(algo))
                    {
                        algorithmQueue.Enqueue(algo);
                        algo.SetBenchmarkPendingNoMsg();
                    }
                    else
                    {
                        algo.ClearBenchmarkPending();
                    }


                BenchmarkSettingsStatus status;
                if (cDev.Enabled)
                {
                    _benchmarkAlgorithmsCount += algorithmQueue.Count;
                    status = algorithmQueue.Count == 0 ? BenchmarkSettingsStatus.NONE : BenchmarkSettingsStatus.TODO;
                    _benchmarkDevicesAlgorithmQueue.Add(
                        new Tuple<ComputeDevice, Queue<Algorithm>>(cDev, algorithmQueue)
                    );
                }
                else
                {

                    status = algorithmQueue.Count == 0
                        ? BenchmarkSettingsStatus.DISABLED_NONE
                        : BenchmarkSettingsStatus.DISABLED_TODO;

                }

                _benchmarkDevicesAlgorithmStatus[cDev.Uuid] = status;
            }

            // GUI stuff
            /*
            progressBarBenchmarkSteps.Maximum = _benchmarkAlgorithmsCount;
            progressBarBenchmarkSteps.Value = 0;
            SetLabelBenchmarkSteps(0, _benchmarkAlgorithmsCount);
            _bechmarkCurrentIndex = 0;
            */
        }

        #endregion

        #region IBenchmarkForm methods

        public void AddToStatusCheck(ComputeDevice device, Algorithm algorithm)
        {
            Invoke((MethodInvoker)delegate
           {
               _statusCheckAlgos[device] = algorithm;
           });
        }

        public void RemoveFromStatusCheck(ComputeDevice device, Algorithm algorithm)
        {
            Invoke((MethodInvoker)delegate
           {
               _statusCheckAlgos.Remove(device);
           });
        }

        public void EndBenchmarkForDevice(ComputeDevice device, bool failedAlgos)
        {
            _hasFailedAlgorithms = failedAlgos || _hasFailedAlgorithms;
            lock (_runningBenchmarkThreads)
            {
                _runningBenchmarkThreads.RemoveAll(x => x.Device == device);

                if (_runningBenchmarkThreads.Count <= 0)
                    EndBenchmark();
            }
        }


        public void SetCurrentStatus(ComputeDevice device, Algorithm algorithm, string status)
        {
            Invoke((MethodInvoker)delegate
           {
               algorithmsListView1.SetSpeedStatus(device, algorithm, status);
           });
            //algorithmsListView1.UpdateLvi();
        }

        public void StepUpBenchmarkStepProgress()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)StepUpBenchmarkStepProgress);
            }
            else
            {
                _bechmarkCurrentIndex++;
                SetLabelBenchmarkSteps(_bechmarkCurrentIndex, _benchmarkAlgorithmsCount);
                if (_bechmarkCurrentIndex <= progressBarBenchmarkSteps.Maximum)
                    progressBarBenchmarkSteps.Value = _bechmarkCurrentIndex;
            }
        }

        #endregion

        #region IListItemCheckColorSetter methods

        public void LviSetColor(ListViewItem lvi)
        {
            if (lvi.Tag is ComputeDevice cDevice && _benchmarkDevicesAlgorithmStatus != null)
            {
                var uuid = cDevice.Uuid;
                if (!cDevice.Enabled)
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
                }
                else
                    switch (_benchmarkDevicesAlgorithmStatus[uuid])
                    {
                        case BenchmarkSettingsStatus.TODO:
                        case BenchmarkSettingsStatus.DISABLED_TODO:
                            lvi.BackColor = UnbenchmarkedColor;
                            lvi.ForeColor = Form_Main._foreColor;
                            break;
                        case BenchmarkSettingsStatus.NONE:
                        case BenchmarkSettingsStatus.DISABLED_NONE:
                            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
                            {
                                lvi.BackColor = BenchmarkedColor;
                            }
                            else
                            {
                                lvi.BackColor = SystemColors.ControlLightLight;
                            }
                            // lvi.BackColor = BenchmarkedColor;
                            lvi.ForeColor = Form_Main._foreColor;
                            break;
                    }
                //// enable disable status, NOT needed
                //if (cdvo.IsEnabled && _benchmarkDevicesAlgorithmStatus[uuid] >= BenchmarkSettingsStatus.DISABLED_NONE) {
                //    _benchmarkDevicesAlgorithmStatus[uuid] -= 2;
                //} else if (!cdvo.IsEnabled && _benchmarkDevicesAlgorithmStatus[uuid] <= BenchmarkSettingsStatus.TODO) {
                //    _benchmarkDevicesAlgorithmStatus[uuid] += 2;
                //}
            }
        }

        #endregion

        private void CopyBenchmarks()
        {
            Helpers.ConsolePrint("CopyBenchmarks", "Checking for benchmarks to copy");
            foreach (var cDev in ComputeDeviceManager.Available.Devices)
                // check if copy
                if (!cDev.Enabled && cDev.BenchmarkCopyUuid != null)
                {
                    var copyCdevSettings = ComputeDeviceManager.Available.GetDeviceWithUuid(cDev.BenchmarkCopyUuid);
                    if (copyCdevSettings != null)
                    {
                        Helpers.ConsolePrint("CopyBenchmarks", $"Copy from {cDev.Uuid} to {cDev.BenchmarkCopyUuid}");
                        cDev.CopyBenchmarkSettingsFrom(copyCdevSettings);
                    }
                }
        }

        private void BenchmarkingTimer_Tick(object sender, EventArgs e)
        {
            if (InBenchmark)
                foreach (var key in _statusCheckAlgos.Keys)
                {
                    string percent;
                    if (_statusCheckAlgos[key].BenchmarkProgressPercent <= 0)
                    {
                        percent = International.GetText("Form_Benchmark_BenchmarkProgress");
                        algorithmsListView1.SetSpeedStatus(key, _statusCheckAlgos[key], GetDotsWaitString() + percent);
                    }
                    else
                    {

                        percent = _statusCheckAlgos[key].BenchmarkProgressPercent.ToString() + "%";
                        algorithmsListView1.SetSpeedStatus(key, _statusCheckAlgos[key], percent);
                    }
                }
        }

        private string GetDotsWaitString()
        {
            Thread.Sleep(10);
            _dotCount++;
            switch (_dotCount)
            {
                case 1:
                    return ".";
                case 2:
                    return "..";
                case 3:
                    return "...";
                case 4:
                    return "....";
            }

            if (_dotCount > 5)
            {
                _dotCount = 1;
            }
            return ".";
        }

        private void InitLocale()
        {
            Text = International.GetText("Form_Benchmark_title"); //International.GetText("SubmitResultDialog_title");
            //labelInstruction.Text = International.GetText("SubmitResultDialog_labelInstruction");
            StartStopBtn.Text = International.GetText("SubmitResultDialog_StartBtn");
            CloseBtn.Text = International.GetText("SubmitResultDialog_CloseBtn");

            // TODO fix locale for benchmark enabled label
            devicesListViewEnableControl1.InitLocale();
            benchmarkOptions1.InitLocale();
            algorithmsListView1.InitLocale();
            groupBoxBenchmarkProgress.Text = International.GetText("FormBenchmark_Benchmark_GroupBoxStatus");
            radioButton_SelectedUnbenchmarked.Text =
                International.GetText("FormBenchmark_Benchmark_All_Selected_Unbenchmarked");
            radioButton_RE_SelectedUnbenchmarked.Text =
                International.GetText("FormBenchmark_Benchmark_All_Selected_ReUnbenchmarked");
            checkBox_StartMiningAfterBenchmark.Text = International.GetText("Form_Benchmark_checkbox_StartMiningAfterBenchmark");
            checkBoxHideUnused.Text = International.GetText("Form_Settings_checkBox_Hide_Unused");
            checkBox_StartMiningAfterBenchmark.Enabled = !Form_Main.MiningStarted;
            checkBoxHideUnused.Checked = ConfigManager.GeneralConfig.Hide_unused_algorithms;
        }

        #region Start/Stop methods

        private void StartStopBtn_Click(object sender, EventArgs e)
        {
            if (InBenchmark)
            {
                StopButonClick();
                BenchmarkStoppedGuiSettings();
                RunCMDAfterBenchmark();
            }
            else if (StartButonClick())
            {
                StartStopBtn.Text = International.GetText("Form_Benchmark_buttonStopBenchmark");
            }
        }

        public void StopBenchmark()
        {
            if (InBenchmark)
            {
                StopButonClick();
                BenchmarkStoppedGuiSettings();
            }
        }

        private void BenchmarkStoppedGuiSettings()
        {
            StartStopBtn.Text = International.GetText("Form_Benchmark_buttonStartBenchmark");
            foreach (var deviceAlgosTuple in _benchmarkDevicesAlgorithmQueue)
            {
                foreach (var algo in deviceAlgosTuple.Item2) algo.ClearBenchmarkPending();
                algorithmsListView1.RepaintStatus(deviceAlgosTuple.Item1.Enabled, deviceAlgosTuple.Item1.Uuid);
            }

            ResetBenchmarkProgressStatus();
            CalcBenchmarkDevicesAlgorithmQueue();
            benchmarkOptions1.Enabled = true;

            algorithmsListView1.IsInBenchmark = false;
            devicesListViewEnableControl1.IsInBenchmark = false;

            CloseBtn.Enabled = true;
            foreach (var cdev in ComputeDeviceManager.Available.Devices)
            {
                cdev.MiningHashrate = 0;
                cdev.MiningHashrateSecond = 0;
            }
        }

        // TODO add list for safety and kill all miners
        private void StopButonClick()
        {
            _benchmarkingTimer.Stop();
            InBenchmark = false;
            Form_Main.InBenchmark = false;
            BenchmarkStarted = false;
            Helpers.ConsolePrint("FormBenchmark", "StopButonClick() benchmark routine stopped");
            //// copy benchmarked
            //CopyBenchmarks();
            lock (_runningBenchmarkThreads)
            {
                foreach (var handler in _runningBenchmarkThreads) handler.InvokeQuit();
            }

            if (ExitWhenFinished) Close();
        }

        private bool StartButonClick()
        {
            /*
            bool ismining = false;
            foreach (var cDev in ComputeDeviceManager.Available.Devices)
            {
                Helpers.ConsolePrint(cDev.Name, cDev.AlgorithmID.ToString());
                if (cDev.AlgorithmID != 0)
                {
                    ismining = true;
                    break;
                }
            }
            */
            if (Form_Main.MiningStarted)
            {
                MessageBox.Show(International.GetText("Form_Benchmark_Stop_mining_first"),
                    International.GetText("Error_with_Exclamation"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            Form_Main.nanominerCount = 0;
            CalcBenchmarkDevicesAlgorithmQueue();
            // device selection check scope
            {
                var noneSelected = ComputeDeviceManager.Available.Devices.All(cDev => !cDev.Enabled);
                if (noneSelected)
                {
                    MessageBox.Show(International.GetText("FormBenchmark_No_Devices_Selected_Msg"),
                        International.GetText("FormBenchmark_No_Devices_Selected_Title"),
                        MessageBoxButtons.OK);
                    return false;
                }
            }
            // device todo benchmark check scope
            {
                var nothingToBench =
                    _benchmarkDevicesAlgorithmStatus.All(statusKpv => statusKpv.Value != BenchmarkSettingsStatus.TODO);
                if (nothingToBench)
                {
                    MessageBox.Show(International.GetText("FormBenchmark_Nothing_to_Benchmark_Msg"),
                        International.GetText("FormBenchmark_Nothing_to_Benchmark_Title"),
                        MessageBoxButtons.OK);
                    return false;
                }
            }

            _hasFailedAlgorithms = false;
            _statusCheckAlgos = new Dictionary<ComputeDevice, Algorithm>();
            lock (_runningBenchmarkThreads)
            {
                _runningBenchmarkThreads = new List<BenchmarkHandler>();
            }

            // disable gui controls
            benchmarkOptions1.Enabled = false;
            CloseBtn.Enabled = false;
            algorithmsListView1.IsInBenchmark = true;
            devicesListViewEnableControl1.IsInBenchmark = true;
            // set benchmark pending status
            foreach (var deviceAlgosTuple in _benchmarkDevicesAlgorithmQueue)
            {
                foreach (var algo in deviceAlgosTuple.Item2) algo.SetBenchmarkPending();
                if (deviceAlgosTuple.Item1 != null)
                    algorithmsListView1.RepaintStatus(deviceAlgosTuple.Item1.Enabled, deviceAlgosTuple.Item1.Uuid);
            }

            progressBarBenchmarkSteps.Maximum = _benchmarkAlgorithmsCount;
            progressBarBenchmarkSteps.Value = 0;
            SetLabelBenchmarkSteps(0, _benchmarkAlgorithmsCount);

            StartBenchmark();

            return true;
        }

        private bool ShoulBenchmark(Algorithm algorithm)
        {
            var isBenchmarked = !algorithm.BenchmarkNeeded;
            switch (_algorithmOption)
            {
                case AlgorithmBenchmarkSettingsType.SelectedUnbenchmarkedAlgorithms when !isBenchmarked &&
                                                                                         algorithm.Enabled:
                    return true;
                case AlgorithmBenchmarkSettingsType.UnbenchmarkedAlgorithms when !isBenchmarked:
                    return true;
                case AlgorithmBenchmarkSettingsType.ReBecnhSelectedAlgorithms when algorithm.Enabled:
                    return true;
                case AlgorithmBenchmarkSettingsType.AllAlgorithms:
                    return true;
            }

            return false;
        }

        private void StartBenchmark()
        {
            InBenchmark = true;
            Form_Main.InBenchmark = true;
            BenchmarkStarted = true;
            lock (_runningBenchmarkThreads)
            {
                foreach (var pair in _benchmarkDevicesAlgorithmQueue)
                {
                    var handler = new BenchmarkHandler(pair.Item1, pair.Item2, this, benchmarkOptions1.PerformanceType);
                    _runningBenchmarkThreads.Add(handler);
                }
                // Don't start until list is populated
                foreach (var thread in _runningBenchmarkThreads)
                {
                    thread.Start();
                }
            }

            _benchmarkingTimer.Start();
        }

        public static void RunCMDAfterBenchmark()
        {
            foreach (var filePath in MinersBins.ALL_FILES_BINS)
            {
                string[] sep = { "/", "." };
                string toKill = filePath.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];

                if (toKill != "x64" && toKill != "txt")
                {
                    Helpers.ConsolePrint("RunCMDAfterBenchmark", "Try kill: " + toKill);
                    foreach (var process in Process.GetProcessesByName(toKill))
                    {
                        try { process.Kill(); }
                        catch (Exception e) { Helpers.ConsolePrint("RunCMDAfterBenchmark", e.ToString()); }
                    }
                }

            }

            /*
            bool CreateNoWindow = false;
            var CMDconfigHandle = new Process

            {
                StartInfo =
                {
                    FileName = "AfterBenchmark.cmd"
                }
            };

            CMDconfigHandle.StartInfo.FileName = "AfterBenchmark.cmd";

            if (!File.Exists(CMDconfigHandle.StartInfo.FileName))
            {
                return null;
            }

            var cmd = "";
            FileStream fs = new FileStream(CMDconfigHandle.StartInfo.FileName, FileMode.Open, FileAccess.Read);
            StreamReader w = new StreamReader(fs);
            cmd = w.ReadToEnd();
            w.Close();

            if (cmd.ToUpper().Trim().Contains("SET NOVISIBLE=TRUE"))
            {
                CreateNoWindow = true;
            }
            if (cmd.ToUpper().Trim().Contains("SET RUN=FALSE"))
            {
                return null;
            }

            Thread.Sleep(100);

            CMDconfigHandle.StartInfo.Arguments = "";
            CMDconfigHandle.StartInfo.UseShellExecute = false;
            CMDconfigHandle.StartInfo.CreateNoWindow = CreateNoWindow;
            Thread.Sleep(150);
            Helpers.ConsolePrint("RunCMDAfterBenchmark", "Start CMD: " + CMDconfigHandle.StartInfo.FileName + CMDconfigHandle.StartInfo.Arguments);
            CMDconfigHandle.Start();

            try
            {
                if (!CMDconfigHandle.WaitForExit(10 * 1000))
                {
                    CMDconfigHandle.Kill();
                    CMDconfigHandle.WaitForExit(5 * 1000);
                    CMDconfigHandle.Close();
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("KillCMDAfterBenchmark", e.ToString());
            }

            Thread.Sleep(5);
            return CMDconfigHandle;
            */
        }
        private void EndBenchmark()
        {
            Invoke((MethodInvoker)delegate
           {
               _benchmarkingTimer.Stop();
               InBenchmark = false;
               Form_Main.InBenchmark = false;
               BenchmarkStarted = false;
               Helpers.ConsolePrint("FormBenchmark", "EndBenchmark() benchmark routine finished");

                BenchmarkStoppedGuiSettings();
               RunCMDAfterBenchmark();
               // check if all ok
               if (!_hasFailedAlgorithms && StartMining == false)
               {
                   MessageBox.Show(
                       International.GetText("FormBenchmark_Benchmark_Finish_Succes_MsgBox_Msg"),
                       International.GetText("FormBenchmark_Benchmark_Finish_MsgBox_Title"),
                       MessageBoxButtons.OK);
               }
               else if (StartMining == false)
               {
                   /*
                   if (!benchmarkRepeat)
                   {
                       benchmarkRepeat = true;
                       StartButonClick();
                       CalcBenchmarkDevicesAlgorithmQueue();
                       if (ExitWhenFinished || StartMining) Close();
                       return;
                   }
                   */
                   var result = MessageBox.Show(
                       International.GetText("FormBenchmark_Benchmark_Finish_Fail_MsgBox_Msg"),
                       International.GetText("FormBenchmark_Benchmark_Finish_MsgBox_Title"),
                       MessageBoxButtons.OK);
                   
                   /*
                   if (result == DialogResult.Retry)
                   {
                       StartButonClick();
                       return;
                   }
                   */
                   // get unbenchmarked from criteria and disable - это ошибка
                   CalcBenchmarkDevicesAlgorithmQueue();
                   /*
                   foreach (var deviceAlgoQueue in _benchmarkDevicesAlgorithmQueue)
                       foreach (var algorithm in deviceAlgoQueue.Item2)
                           algorithm.Enabled = false;
                   */
               }

               if (ExitWhenFinished || StartMining) Close();
           });
        }

        #endregion

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            if (UpdateListView_timer != null)
            {
                UpdateListView_timer.Stop();
                UpdateListView_timer = null;
            }

            Close();
        }

        private void FormBenchmark_New_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (InBenchmark)
            {
                e.Cancel = true;
                return;
            }
            Algorithm.BenchmarkActive = false;
            // disable all pending benchmark
            foreach (var cDev in ComputeDeviceManager.Available.Devices)
                foreach (var algorithm in cDev.GetAlgorithmSettings())
                    algorithm.ClearBenchmarkPending();

            // save already benchmarked algorithms
            ConfigManager.CommitBenchmarks();
            // check devices without benchmarks
            foreach (var cdev in ComputeDeviceManager.Available.Devices)
                if (cdev.Enabled)
                {
                    var enabled = cdev.GetAlgorithmSettings().Any(algo => algo.BenchmarkSpeed > 0);
                    cdev.Enabled = enabled;
                }
            if (Form_Benchmark.ActiveForm != null)
            {
                if (ConfigManager.GeneralConfig.Save_windows_size_and_position)
                {
                    ConfigManager.GeneralConfig.BenchmarkFormHeight = Form_Benchmark.ActiveForm.Height;
                    ConfigManager.GeneralConfig.BenchmarkFormWidth = Form_Benchmark.ActiveForm.Width;
                    ConfigManager.GeneralConfig.BenchmarkFormTop = Form_Benchmark.ActiveForm.Top;
                    ConfigManager.GeneralConfig.BenchmarkFormLeft = Form_Benchmark.ActiveForm.Left;
                }
            }
            ConfigManager.GeneralConfigFileCommit();
            if (UpdateListView_timer != null)
            {
                UpdateListView_timer.Stop();
                UpdateListView_timer = null;
            }

        }

        private void DevicesListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //algorithmSettingsControl1.Deselect();
            // show algorithms
            _selectedComputeDevice =
                ComputeDeviceManager.Available.GetCurrentlySelectedComputeDevice(e.ItemIndex, true);
            algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
        }

        private void RadioButton_SelectedUnbenchmarked_CheckedChanged_1(object sender, EventArgs e)
        {
            _algorithmOption = AlgorithmBenchmarkSettingsType.SelectedUnbenchmarkedAlgorithms;
            CalcBenchmarkDevicesAlgorithmQueue();
            devicesListViewEnableControl1.ResetListItemColors();
            algorithmsListView1.ResetListItemColors();
        }

        private void RadioButton_RE_SelectedUnbenchmarked_CheckedChanged(object sender, EventArgs e)
        {
            _algorithmOption = AlgorithmBenchmarkSettingsType.ReBecnhSelectedAlgorithms;
            CalcBenchmarkDevicesAlgorithmQueue();
            devicesListViewEnableControl1.ResetListItemColors();
            algorithmsListView1.ResetListItemColors();
        }

        private void CheckBox_StartMiningAfterBenchmark_CheckedChanged(object sender, EventArgs e)
        {
            StartMining = checkBox_StartMiningAfterBenchmark.Checked;
        }

        private enum BenchmarkSettingsStatus
        {
            NONE = 0,
            TODO,
            DISABLED_NONE,
            DISABLED_TODO
        }


        #region Benchmark progress GUI stuff

        private void SetLabelBenchmarkSteps(int current, int max)
        {
            labelBenchmarkSteps.Text =
                string.Format(International.GetText("FormBenchmark_Benchmark_Step"), current, max);
        }

        private void ResetBenchmarkProgressStatus()
        {
            progressBarBenchmarkSteps.Value = 0;
        }

        #endregion // Benchmark progress GUI stuff

        private void algorithmsListView1_Load(object sender, EventArgs e)
        {

        }

        private void devicesListViewEnableControl1_Load(object sender, EventArgs e)
        {
        }

        private void Form_Benchmark_ResizeBegin(object sender, EventArgs e)
        {
            FormBenchmarkMoved = true;
        }

        private void Form_Benchmark_ResizeEnd(object sender, EventArgs e)
        {
            FormBenchmarkMoved = false;
        }

        private void checkBoxHideUnused_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.GeneralConfig.Hide_unused_algorithms = checkBoxHideUnused.Checked;
            try
            {
                if (_selectedComputeDevice == null) return;
                algorithmsListView1.SetAlgorithms(_selectedComputeDevice, _selectedComputeDevice.Enabled);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
