using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Forms;
using NiceHashMiner.Miners;
using NiceHashMiner.Miners.Grouping;
using NiceHashMiner.Stats.V4;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using NiceHashMinerLegacy.UUID;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WebSocketSharp;

namespace NiceHashMiner.Stats
{
    public class SocketEventArgs : EventArgs
    {
        public readonly string Message;

        public SocketEventArgs(string message)
        {
            Message = message;
        }
    }

    internal class NiceHashStats
    {
        #region JSON Models
#pragma warning disable 649, IDE1006
        private class NicehashCredentials
        {
            public string method = "credentials.set";
            public string btc;
            public string worker;
        }

        private class NicehashDeviceStatus
        {
            public string method = "devices.status";
            public List<JArray> devices;
        }
        private class NicehashDeviceStatusNew
        {
            public string method = "miner.status";
            public List<JArray> devices;
        }
        private class MinerStatusMessage
        {
            public string method = "miner.status";
            [JsonProperty("params")]
            public List<JToken> param { get; set; }
        }


        public class ExchangeRateJson
        {
            public List<Dictionary<string, string>> exchanges { get; set; }
            public Dictionary<string, double> exchanges_fiat { get; set; }
        }
#pragma warning restore 649, IDE1006
        #endregion

        private const int DeviceUpdateInterval = 56 * 1000;

        public static double Balance { get; private set; }
        public static string Version = "";

        public static bool IsAlive => _socket?.IsAlive ?? false;

        public static event EventHandler OnSmaUpdate;

        public static NiceHashSocket _socket;

        public static System.Timers.Timer _deviceUpdateTimer;

        public static bool remoteMiningStart = false;
        public static bool remoteMiningStop = false;
        public static bool remoteUpdateUI = false;
        private static bool DeviceStatusRunning = false;

        private static List<AlgorithmType> smaAlgos = new List<AlgorithmType>();
        private static List<string> markets = new List<string>();
        public static string serverTime;

        public static void StartConnection(string address)
        {
            try
            {
                _deviceUpdateTimer = new System.Timers.Timer(DeviceUpdateInterval);
                _deviceUpdateTimer.Elapsed += DeviceStatus_Tick;
                _deviceUpdateTimer.Start();

                //NHSmaData.InitializeIfNeeded();
                //LoadSMA();

                _socket = null;
                _socket = new NiceHashSocket(address);

                _socket.OnDataReceived += SocketOnOnDataReceived;

                Helpers.ConsolePrint("SOCKET-address:", address);
                new Task(() => _socket.StartConnection()).Start();
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("StartConnection", er.ToString());
            }
            /*
            finally
            {
                _deviceUpdateTimer = new System.Timers.Timer(DeviceUpdateInterval);
                _deviceUpdateTimer.Elapsed += DeviceStatus_TickNew;
                _deviceUpdateTimer.Start();
            }
            */
        }

        #region Socket Callbacks
        private static void SocketOnOnDataReceived(object sender, MessageEventArgs e)
        {
            new Task(() => SocketReceive(sender, e)).Start();
        }
        private static bool firstSMA = true;
        private static void SocketReceive(object sender, MessageEventArgs e)
        {
            Form_Main.wssConnectionsErrors = 0;
            Form_Main.TotalConnectionsErrors = 0;
            try
            {
                if (e.IsText)
                {
                    if (ConfigManager.GeneralConfig.SaveProtocolData)
                    {
                        Helpers.ConsolePrint("SOCKET", $"Received: {e.Data}");
                    }
                    else
                    {
                        Helpers.ConsolePrint("SOCKET", $"Received: {e.Data.Substring(0, 20)}...");
                    }
                    /*
                    if (!e.Data.Contains("exchange_rates"))
                    {
                        Helpers.ConsolePrint("SOCKET", "Received: " + e.Data);
                    }
                    */
                    string jsondata = e.Data;
                    if (jsondata.EndsWith("\""))
                    {
                        jsondata = jsondata.Substring(0, jsondata.Length - 1);
                    }
                    dynamic message = JsonConvert.DeserializeObject(jsondata);
                    // Helpers.ConsolePrint("SOCKET", "Received1: " + e.Data);
                    switch (message.method.Value)
                    {
                            case "sma":
                            {
                                new Task(() => NiceHashStats.SetDeviceStatus(null, true, "SocketReceive")).Start();

                                if (File.Exists("configs\\sma.dat")) File.Delete("configs\\sma.dat");
                                string stw = (string)JsonConvert.SerializeObject(message.data);
                                File.WriteAllText("configs\\sma.dat", stw);

                                foreach (var algo in message.data)
                                {
                                    var algoKey = (AlgorithmType)algo[0];
                                    if (!smaAlgos.Contains(algoKey))
                                    {
                                        smaAlgos.Add(algoKey);
                                    }
                                }

                                double tmp = 0.0d;
                                JArray data = message.data;
                                foreach (var algo in data)
                                {
                                    if (algo == null) return;
                                    var algoKeyTmp = (AlgorithmType)algo[0].Value<int>();
                                    double.TryParse((string)algo[1], out var payingTmp);
                                    tmp = tmp + payingTmp;
                                }
                                if (tmp == 0)
                                {
                                    Helpers.ConsolePrint("SMA WS", "All algos zero!");
                                    return;
                                }
                                else
                                {
                                    if (firstSMA)
                                    {
                                        firstSMA = false;

                                        Thread.Sleep(500);
                                        SetAlgorithmRates(message.data, 1, 12, false, "WS");
                                        NiceHashStats.GetSmaAPI(true);
                                        NHSmaData.FinalizeSma();

                                    }
                                    else
                                    {
                                        do
                                        {
                                            Thread.Sleep(500);
                                        } while (Form_Main.Uptime.Seconds != 5 && Form_Main.Uptime.Seconds != 35);
                                        SetAlgorithmRates(message.data, 1, 12, true, "WS");
                                    }
                                }
                                break;
                            }

                        case "markets":
                            foreach (string market in message.data)
                            {
                                markets.Add(market);
                            }
                            break;

                        case "balance":
                            //if (!ConfigManager.GeneralConfig.ChartEnable)
                            {
                                SetBalance(message.value.Value);
                            }
                            break;
                        case "versions":
                            ConfigManager.GeneralConfig.NHMVersion = message.v3.Value;
                            break;

                        case "miner.call.action":
                            var action = ActionMutableMap.ActionList.Find((a) => a.ActionID == (int)message.action_id.Value);
                            if (action.ActionType == SupportedAction.ActionStartMining)
                            {
                                string pars = "";
                                foreach(string s in message.parameters)
                                {
                                    pars = pars + s.ToString();
                                }
                                RemoteMiningStart(message.id.ToString(), pars);
                            }
                            if (action.ActionType == SupportedAction.ActionStopMining)
                            {
                                string pars = "";
                                foreach (string s in message.parameters)
                                {
                                    pars = pars + s.ToString();
                                }
                                RemoteMiningStop(message.id.ToString(), pars);
                            }
                            if (action.ActionType == SupportedAction.ActionRestart)
                            {
                                var OSrestartR = new ProcessStartInfo("shutdown")
                                {
                                    WindowStyle = ProcessWindowStyle.Minimized
                                };
                                OSrestartR.Arguments = "-r -f -t 10";
                                Helpers.ConsolePrint("*************", "Restart Windows");
                                var cExecuted = "{\"method\":\"executed\",\"params\":[" + message.id.ToString() + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
                                _socket.SendData(cExecuted);
                                Process.Start(OSrestartR);
                            }
                            break;
                        case "mining.start":
                            RemoteMiningStart(message.id.Value.ToString(), message.device.Value);
                            break;
                        case "mining.stop":
                            RemoteMiningStop(message.id.Value.ToString(), message.device.Value);
                            break;
                        case "mining.set.username":
                            RemoteSetUsername(message.id.Value.ToString());
                            break;
                        case "mining.set.worker":
                            RemoteSetWorker(message.id.Value.ToString(), message.worker.Value);
                            break;
                        case "mining.set.group":
                            RemoteMiningNotImplemented(message.id.Value.ToString());
                            break;

                        case "mining.enable":
                            RemoteMiningEnable(message.id.Value.ToString(), message.device.Value.ToString(), true);
                            break;
                        case "mining.disable":
                            RemoteMiningEnable(message.id.Value.ToString(), message.device.Value.ToString(), false);
                            break;
                        case "mining.set.power_mode":
                            RemoteMiningNotImplemented(message.id.Value.ToString());
                            break;
                        case "miner.set.mutable":
                            try
                            {
                                JArray properties = message.properties;
                                if (properties is JArray)
                                {
                                    foreach (var prop in properties)
                                    {
                                        int prop_id = prop.Value<int?>("prop_id") ?? -1;

                                        if (prop_id == 100)//autoupdate
                                        {
                                            bool value = prop.Value<bool?>("value") ?? false;
                                            RemoteAutoUpdate(message.id.Value.ToString(), value);
                                            Form_Settings.ForceClosingForm = true;
                                            Thread.Sleep(1000);
                                            Form_Settings.ForceClosingForm = false;
                                        }
                                    }
                                }

                                JArray devices = message.devices;//102,103
                                JArray rig_properties = message.properties;//104,106
                                if (rig_properties is JArray)
                                {
                                    foreach (var rigprop in rig_properties)
                                    {
                                        int prop_id = rigprop.Value<int?>("prop_id") ?? -1;
                                        if (prop_id == 104)//miners settings per rig
                                        {
                                            RemoteMinersSettingsRig(message.id.Value.ToString(), rigprop);
                                            Form_Settings.ForceClosingForm = true;
                                            Thread.Sleep(1000);
                                            Form_Settings.ForceClosingForm = false;
                                        }
                                        if (prop_id == 106)//miners benchmark per rig
                                        {
                                            RemoteBenchmarkSettingsRig(message.id.Value.ToString(), rigprop);
                                            Form_Settings.ForceClosingForm = true;
                                            Thread.Sleep(1000);
                                            Form_Settings.ForceClosingForm = false;
                                        }
                                    }
                                }
                                
                                if (devices is JArray)
                                {
                                    foreach (var dev in devices)
                                    {
                                        string id = dev.Value<string>("id") ?? "unknown device";
                                        JArray dev_properties = dev.Value<JArray>("properties") ?? null;
                                        if (dev_properties is JArray)
                                        {
                                            foreach (var devprop in dev_properties)
                                            {
                                                int prop_id = devprop.Value<int?>("prop_id") ?? -1;
                                                if (prop_id == 102)//miners settings per device
                                                {
                                                    RemoteMinersSettings(message.id.Value.ToString(), devprop);
                                                    Form_Settings.ForceClosingForm = true;
                                                    Thread.Sleep(1000);
                                                    Form_Settings.ForceClosingForm = false;
                                                }
                                                if (prop_id == 103)//benchmark settings per device
                                                {
                                                    JToken value = devprop.Value<JToken>("value") ?? null;
                                                    if (value is JToken)
                                                    {
                                                        RemoteBenchmarkSettings(message.id.Value.ToString(), value);
                                                        Form_Settings.ForceClosingForm = true;
                                                        Thread.Sleep(1000);
                                                        Form_Settings.ForceClosingForm = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            } catch (Exception ex)
                            {
                                Helpers.ConsolePrint("miner.set.mutable", ex.ToString());
                            }
                            break;
                        case "exchange_rates":
                            SetExchangeRates(message.data.Value);
                            break;
                        case "miner.reset":
                            var OSrestart = new ProcessStartInfo("shutdown")
                            {
                                WindowStyle = ProcessWindowStyle.Minimized
                            };
                            OSrestart.Arguments = "-r -f -t 10";
                            Helpers.ConsolePrint("*************", "Restart Windows");
                            Process.Start(OSrestart);
                            break;
                    }
                }

            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("SOCKET", er.ToString());
            }
            //GC.Collect();
        }

        public class RootobjectCurrent
        {
            public MiningAlgorithms[] miningAlgorithms { get; set; }
        }
        public class MiningAlgorithms
        {
            public string algorithm { get; set; }
            public string title { get; set; }
            public string speed { get; set; }
            public string paying { get; set; }
        }

        public class Rootobject5m
        {
            public Algos5m[] algos { get; set; }
        }
        public class Algos5m
        {
            public int a { get; set; }
            public string p { get; set; }
            public float s { get; set; }
        }
        public class Rootobject24h
        {
            public List<Algos24h> algos { get; set; }
        }
        public class Algos24h
        {
            public int a { get; set; }
            public string p { get; set; }
            public double s { get; set; }
        }

        public class ProfitsSMA
        {
            public string Method { get; set; }
            public IList<IList<object>> Data { get; set; }
        }

        
        public static async Task RemoteBenchmarkSettings(string id, JToken value)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-999,\"Remote management disabled\"]}";
                return;
            }
            JToken JT_value = (JToken)JsonConvert.DeserializeObject(value.ToString());

            if (JT_value is JToken)
            {
                string device_id = JT_value.Value<string>("device_id") ?? "unknown device";
                string device_name = JT_value.Value<string>("device_name") ?? "unknown device";

                var devData = ComputeDeviceManager.Available.Devices.FirstOrDefault(dev => dev.DevUuid == device_id);

                JArray miners = JT_value.Value<JArray>("miners") ?? null;
                SetMinersBenchmarks(devData, miners);
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            await _socket.SendData(cExecuted);

        }

        public static async Task RemoteBenchmarkSettingsRig(string id, JToken devprop)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-999,\"Remote management disabled\"]}";
                return;
            }

            string value = devprop.Value<string>("value") ?? null;
            value = value.Replace("\\\"", "\"");
            JToken _value = (JToken)JsonConvert.DeserializeObject(value);
            if (_value is JToken)
            {
                JArray devices = _value.Value<JArray>("devices") ?? null;
                if (devices is JArray)
                {
                    foreach (var device in devices)
                    {
                        string device_id = device.Value<string>("device_id") ?? "unknown device";
                        string device_name = device.Value<string>("device_name") ?? "unknown device name";
                        var devData = ComputeDeviceManager.Available.Devices.FirstOrDefault(dev => dev.DevUuid == device_id);
                        JArray miners = device.Value<JArray>("miners") ?? null;
                        SetMinersBenchmarks(devData, miners);
                    }
                }
            }

            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            await _socket.SendData(cExecuted);
        }

        private static void SetMinersBenchmarks(ComputeDevice devData, JArray miners)
        {
            if (miners is JArray)
            {
                foreach (var miner in miners)
                {
                    string miner_id = miner.Value<string>("id") ?? "unknown miner";
                    JArray combination = miner.Value<JArray>("combination") ?? null;
                    if (combination is JArray)
                    {
                        foreach (var _combination in combination)
                        {
                            string combination_id = _combination.Value<string>("id") ?? "unknown algorithm";
                            JArray algorithm = _combination.Value<JArray>("algorithm") ?? null;
                            if (algorithm is JArray)
                            {
                                foreach (var _algorithm in algorithm)
                                {
                                    string _algorithm_id = _algorithm.Value<string>("id") ?? "unknown algorithm_id";
                                    string _algorithm_speed = _algorithm.Value<string>("speed") ?? "unknown algorithm_id";
                                    foreach (var _algo in devData.GetAlgorithmSettings())
                                    {
                                        if (miner_id.Contains(_algo.MinerBaseType.ToString()))
                                        {
                                            if (combination_id == _algo.DualNiceHashID.ToString())
                                            {
                                                int.TryParse(_algorithm_id, out int i_algorithm_id);
                                                if (i_algorithm_id == (int)_algo.NiceHashID)
                                                {
                                                    double.TryParse(_algorithm_speed, out double d_algorithm_speed);
                                                    _algo.BenchmarkSpeed = d_algorithm_speed;
                                                }
                                            }
                                            if (_algo is DualAlgorithm _dualalgo)
                                            {
                                                if (combination_id == _algo.DualNiceHashID.ToString())
                                                {
                                                    int.TryParse(_algorithm_id, out int i_algorithm_id);
                                                    if (i_algorithm_id == (int)_dualalgo.SecondaryNiceHashID)
                                                    {
                                                        double.TryParse(_algorithm_speed, out double d_algorithm_speed);
                                                        _dualalgo.BenchmarkSecondarySpeed = d_algorithm_speed;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static async Task RemoteMinersSettingsRig(string id, JToken devprop)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-999,\"Remote management disabled\"]}";
                return;
            }

            string value = devprop.Value<string>("value") ?? null;
            value = value.Replace("\\\"", "\"");
            JToken _value = (JToken)JsonConvert.DeserializeObject(value);

            if (_value is JToken)
            {
                JArray devices = _value.Value<JArray>("devices") ?? null;
                if (devices is JArray)
                {
                    foreach (var device in devices)
                    {
                        string device_id = device.Value<string>("device_id") ?? "unknown device";
                        string device_name = device.Value<string>("device_name") ?? "unknown device name";
                        var devData = ComputeDeviceManager.Available.Devices.FirstOrDefault(dev => dev.DevUuid == device_id);
                        JArray miners = device.Value<JArray>("miners") ?? null;
                        SetMinersSettings(devData, miners);
                    }
                }
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            await _socket.SendData(cExecuted);
        }


        public static async Task RemoteMinersSettings(string id, JToken devprop)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-999,\"Remote management disabled\"]}";
                return;
            }

            string value = devprop.Value<string>("value") ?? null;
            value = value.Replace("\\\"", "\"");
            JToken _value = (JToken)JsonConvert.DeserializeObject(value);

            if (_value is JToken)
            {
                string device_id = _value.Value<string>("device_id") ?? "unknown device";
                var devData = ComputeDeviceManager.Available.Devices.FirstOrDefault(dev => dev.DevUuid == device_id);
                JArray miners = _value.Value<JArray>("miners") ?? null;
                SetMinersSettings(devData, miners);
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            await _socket.SendData(cExecuted);
        }

        private static void SetMinersSettings(ComputeDevice devData, JArray miners)
        {
            if (miners is JArray)
            {
                foreach (var miner in miners)
                {
                    string miner_id = miner.Value<string>("id") ?? "unknown miner";
                    bool miner_enabled = miner.Value<bool?>("enabled") ?? false;
                    JArray algorithms = miner.Value<JArray>("algorithms") ?? null;
                    if (algorithms is JArray)
                    {
                        foreach (var algo in algorithms)
                        {
                            string algo_id = algo.Value<string>("id") ?? "unknown miner";
                            bool algo_enabled = algo.Value<bool?>("enabled") ?? false;
                            foreach (var _algo in devData.GetAlgorithmSettings())
                            {
                                if (miner_id.Contains(_algo.MinerBaseType.ToString()))
                                {
                                    if (algo_id == _algo.AlgorithmNameCustom)
                                    {
                                        _algo.Enabled = algo_enabled && miner_enabled;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task RemoteAutoUpdate(string id, bool autoupdate)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-999,\"Remote management disabled\"]}";
                return;
            }
            ConfigManager.GeneralConfig.ProgramAutoUpdate = autoupdate;
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            await _socket.SendData(cExecuted);

        }

        public static async Task RemoteMiningEnable(string id, string deviceToSwitch, bool Enabled)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",1,\"Remote management disabled\"]}";
                return;
            }

            var _computeDevicesResort = ComputeDeviceManager.ReSortDevices(ComputeDeviceManager.Available.Devices);
            var _computeDevices = ComputeDeviceManager.Available.Devices;
            bool miningStarted = Form_Main.MiningStarted;

            if (!Form_Main.NVIDIA_orderBug)
            {
                foreach (var dev in _computeDevices)
                {
                    if (deviceToSwitch.Equals(dev.DevUuid))
                    {
                        dev.Enabled = Enabled;
                    }
                }
            }
            else
            {
                foreach (var dev in _computeDevicesResort)
                {
                    if (deviceToSwitch.Equals(dev.DevUuid))
                    {
                        dev.Enabled = Enabled;
                    }
                }
            }

            if (miningStarted)
            {
                MinersManager.StopAllMiners();
                remoteMiningStop = true;
                Thread.Sleep(2000);
                remoteMiningStart = true;
            }

            Helpers.ConsolePrint("REMOTE", "id: " + id + " device: " + deviceToSwitch);
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            await _socket.SendData(cExecuted);
            return;
        }
        public static async Task RemoteMiningNotImplemented(string id)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",1,\"Remote management disabled\"]}";
                //await _socket.SendData(cExecutedDisabled);
                return;
            }
            Helpers.ConsolePrint("REMOTE", "Not implemented");
            var cExecutedNotImplemented = "{\"method\":\"executed\",\"params\":[" + id + ",-3,\"Not implemented in Fork Fix " + ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",", ".") + "\"]}";
            //await _socket.SendData(cExecutedNotImplemented);
            return;
        }
        public static async Task RemoteMiningStart(string id, string par)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",1,\"Remote management disabled\"]}";
                return;
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            if (Miner.IsRunningNew)
            {
                await _socket.SendData(cExecuted);
                Helpers.ConsolePrint("REMOTE", "Already mining");
                return;
            }
            remoteMiningStart = true;
            Thread.Sleep(3000);
            await _socket.SendData(cExecuted);
            Helpers.ConsolePrint("REMOTE", "Mining start. ID:" + id + " Par:" + par);
        }

        public static async Task RemoteMiningStop(string id, string par)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-1,\"Remote management disabled\"]}";
                return;
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            if (!Miner.IsRunningNew)
            {
                await _socket.SendData(cExecuted);
                Helpers.ConsolePrint("REMOTE", "Already stopped");
                return;
            }
            remoteMiningStop = true;
            Thread.Sleep(2000);
            await _socket.SendData(cExecuted);
            Helpers.ConsolePrint("REMOTE", "Mining stop. ID:" + id + " Device:" + par);
        }
        public static async Task RemoteSetWorker(string id, string worker)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-999,\"Remote management disabled\"]}";
                //await _socket.SendData(cExecutedDisabled);
                return;
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            await _socket.SendData(cExecuted);
            ConfigManager.GeneralConfig.WorkerName = worker;
            new Task(() => _socket.StartConnection()).Start();
            //_socket.StartConnection();
        }
        public static async Task RemoteSetUsername(string id)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-1,\"Remote management disabled\"]}";
                return;
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + "," + ((int)NhmwsSetResult.CHANGED).ToString() + "]}";
            if (!Miner.IsRunningNew)
            {
                await _socket.SendData(cExecuted);
                Helpers.ConsolePrint("REMOTE", "Already stopped");
                return;
            }
            remoteMiningStop = true;
            Thread.Sleep(2000);
            await _socket.SendData(cExecuted);
            //Helpers.ConsolePrint("REMOTE", "Mining stop. ID:" + id + " Device:" + device);
        }

        public static bool GetSmaAPIOrder()
        {
            Helpers.ConsolePrint("NHM_API_info", "Trying GetSmaAPIOrder");

            try
            {
                ProfitsSMA profdata = new ProfitsSMA();
                List<ProfitsSMA> profdata2 = new List<ProfitsSMA>();
                string outProf = "[\n";
                var _currentSma = new Dictionary<AlgorithmType, NiceHashSma>();
                int Algo = 0;
                foreach (AlgorithmType algo in Enum.GetValues(typeof(AlgorithmType)))
                {
                    if (smaAlgos.Contains(algo) && !algo.ToString().ToUpper().Contains("UNUSED") &&
                        !algo.ToString().ToUpper().Contains("RANDOMX"))
                    {
                        string a = algo.ToString().ToUpper();
                        //Helpers.ConsolePrint("GetSmaAPIOrder: ", a);
                        string resp = GetNiceHashApiData(Links.NhmHashpower + a + "&size=10", "x");
                        //Helpers.ConsolePrint("GetSmaAPIOrder: ", resp);
                        dynamic json = JsonConvert.DeserializeObject(resp);
                        if (json == null) return false;
                        var stats = json.stats;
                        if (stats == null) return false;
                        string token;
                        double maxpay = 0.0d;
                        int activeOrders = 0;
                        double price = 0.0d;
                        foreach (string _market in markets)
                        {
                            token = _market;
                            //локация не влияет на прибыльность. найс платит по среднему со всех рынков
                            //if (!resp.Contains("\"" + _market + "\"") || !_market.ToUpper().Contains(Form_Main.myServers[0, 0].Split('-')[0].ToUpper()))
                            if (!resp.Contains("\"" + _market + "\""))
                            {
                                continue;
                            }
                            dynamic market = stats.SelectToken(token);
                            double priceFactor = (double)Convert.ToDouble(market.priceFactor, CultureInfo.InvariantCulture.NumberFormat);

                            foreach (var orders in market.orders)
                            {
                                int rigsCount = (int)((int)Convert.ToDouble(orders.rigsCount, CultureInfo.InvariantCulture.NumberFormat));
                                if (rigsCount > 0)
                                {
                                    double limit = (double)Convert.ToDouble(orders.limit, CultureInfo.InvariantCulture.NumberFormat);
                                    double payingSpeed = (double)Convert.ToDouble(orders.payingSpeed, CultureInfo.InvariantCulture.NumberFormat);
                                    //if (limit == 0.0d || limit > payingSpeed)
                                    {
                                        activeOrders++;
                                        price = price + (double)Convert.ToDouble(orders.price, CultureInfo.InvariantCulture.NumberFormat) / priceFactor * 1000000000;
                                    }
                                }
                            }
                        }

                        maxpay = price / activeOrders;
                        //Helpers.ConsolePrint("GetSmaAPIOrder: " + a, maxpay.ToString());
                        Algo = (int)algo;
                        var AlgorithmName = AlgorithmNiceHashNames.GetName(algo);
                        outProf = outProf + "  [\n" + "    " + Algo + ",\n" + "    " + maxpay.ToString() + "\n" + "  ],\n";
                    }
                }
                outProf = outProf.Remove(outProf.Length - 2) + "]";
                JArray smadata = (JArray.Parse(outProf));
                NiceHashStats.SetAlgorithmRates(smadata, 1, 15, true, "Order");//GetSmaAPIOrder
                return true;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetSmaAPIOrder", ex.Message);
                Helpers.ConsolePrint("GetSmaAPIOrder", "GetSmaAPIOrder fatal ERROR");
                return false;
            }
            return false;
        }

        public static bool GetSmaAPICurrent()
        {
            Helpers.ConsolePrint("NHM_API_info", "Trying GetSmaAPICurrent");

            try
            {
                string resp;
                resp = NiceHashStats.GetNiceHashApiData(Links.NhmSimplemultialgo, "x");
                if (resp != null)
                {
                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                    {
                        // Helpers.ConsolePrint("NHM_API_info", resp);
                    }
                    //Helpers.ConsolePrint("NHM_API_info", resp);
                    dynamic list;
                    list = JsonConvert.DeserializeObject<RootobjectCurrent>(resp);

                    ProfitsSMA profdata = new ProfitsSMA();

                    List<ProfitsSMA> profdata2 = new List<ProfitsSMA>();

                    string outProf = "[\n";

                    var _currentSma = new Dictionary<AlgorithmType, NiceHashSma>();
                    foreach (var miningAlgorithms in list.miningAlgorithms)
                    {
                        int Algo = 0;
                        foreach (AlgorithmType algo in Enum.GetValues(typeof(AlgorithmType)))
                        {
                            if (algo >= 0)
                            {
                                Algo = (int)algo;
                                var AlgorithmName = AlgorithmNiceHashNames.GetName(algo);
                                if (AlgorithmName == miningAlgorithms.title)
                                {
                                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                                    {
                                        //Helpers.ConsolePrint("SMA-DATA-APICurrent: ", miningAlgorithms.title + " - " + Algo + " - " + miningAlgorithms.paying);
                                    }
                                    outProf = outProf + "  [\n" + "    " + Algo + ",\n" + "    " + miningAlgorithms.paying + "\n" + "  ],\n";
                                    var algoKey = (AlgorithmType)algo;
                                    if (!smaAlgos.Contains(algoKey))
                                    {
                                        smaAlgos.Add(algoKey);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    outProf = outProf.Remove(outProf.Length - 2) + "]";

                    // Helpers.ConsolePrint("SMA-DATA-APICurrent: ", outProf);
                    JArray smadata = (JArray.Parse(outProf));

                    NiceHashStats.SetAlgorithmRates(smadata, 10, 15, true, "Current");//GetSmaAPICurrent

                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                    {
                        Helpers.ConsolePrint("NHM_API_info", "GetSmaAPICurrent OK");
                    }
                    return true;
                }
                Helpers.ConsolePrint("NHM_API_info", "GetSmaAPICurrent ERROR");
                //Form_Main.NicehashAPIerrorDescription = "ERROR";
                return false;

            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("NHM_API_info", ex.Message);
                Helpers.ConsolePrint("NHM_API_info", "GetSmaAPICurrent fatal ERROR");
                return false;
            }
            return false;
        }

        public static bool GetSmaAPI24h()
        {
            Helpers.ConsolePrint("NHM_API_info", "Trying GetSmaAPI24h");

            try
            {
                string resp;
                resp = NiceHashStats.GetNiceHashApiData(Links.Nhm24h, "x");
                if (resp != null)
                {
                    //Helpers.ConsolePrint("NHM_API_info", resp);
                    dynamic list;
                    list = JsonConvert.DeserializeObject<Rootobject24h>(resp);

                    ProfitsSMA profdata = new ProfitsSMA();

                    List<ProfitsSMA> profdata2 = new List<ProfitsSMA>();

                    string outProf = "[\n";

                    var _currentSma = new Dictionary<AlgorithmType, NiceHashSma>();
                    foreach (var miningAlgorithms in list.algos)
                    {
                        int Algo = 0;
                        foreach (AlgorithmType algo in Enum.GetValues(typeof(AlgorithmType)))
                        {
                            if (algo >= 0)
                            {
                                Algo = (int)algo;
                                //var AlgorithmName = AlgorithmNiceHashNames.GetName(algo);
                                if (Algo == miningAlgorithms.a)
                                {
                                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                                    {
                                        //Helpers.ConsolePrint("SMA-DATA-APICurrent: ", miningAlgorithms.title + " - " + Algo + " - " + miningAlgorithms.paying);
                                    }
                                    outProf = outProf + "  [\n" + "    " + miningAlgorithms.a + ",\n" + "    " + miningAlgorithms.p + "\n" + "  ],\n";
                                    var algoKey = (AlgorithmType)algo;
                                    if (!smaAlgos.Contains(algoKey))
                                    {
                                        smaAlgos.Add(algoKey);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    outProf = outProf.Remove(outProf.Length - 2) + "]";

                    // Helpers.ConsolePrint("SMA-DATA-APICurrent: ", outProf);
                    JArray smadata = (JArray.Parse(outProf));

                    NiceHashStats.SetAlgorithmRates(smadata, 10, 15, false, "24h");//GetSmaAPI24h

                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                    {
                        Helpers.ConsolePrint("NHM_API_info", "GetSmaAPI24h OK");
                    }
                    return true;
                }
                Helpers.ConsolePrint("NHM_API_info", "GetSmaAPI24h ERROR");
                return false;

            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("NHM_API_info", ex.Message);
                Helpers.ConsolePrint("NHM_API_info", "GetSmaAPI24h fatal ERROR");
                return false;
            }
            return false;

        }

        public static string GetApiFlags()
        {
            string apistr = Links.ApiFlags;
            string resp;
            try
            {
                resp = NiceHashStats.GetNiceHashApiDataWithSecret(apistr, false);
                if (resp != null)
                {
                    dynamic respFlags = JsonConvert.DeserializeObject(resp);
                    foreach(var flag in respFlags.list)
                    {
                        if (flag.flagName == "IS_MAINTENANCE" && flag.flagValue == true)
                        {
                            return "(" + International.GetText("Form_Main_NHflagMaintenance") + ")";
                        }
                        if (flag.flagName == "SYSTEM_UNAVAILABLE" && flag.flagValue == true)
                        {
                            return "(" + International.GetText("Form_Main_NHflagSystemUnavialable") + ")";
                        }
                    }
                }
                else
                {
                    return "";
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("GetApiFlags", ex.ToString());
            }
            return "";
        }

        public static bool GetRigProfit()
        {
            if (Form_Main.GetBTCwalletType().Equals("P2SH"))
            {
                return GetRigProfitInternal();//internal wallet P2SH
            }
            else if (Form_Main.GetBTCwalletType().Equals("P2PKH"))
            {
                return GetRigProfitExternal(); //external wallet P2PKH
            }
            else if (Form_Main.GetBTCwalletType().Equals("SegWit"))
            {
                return GetRigProfitExternal(); //external wallet SegWit
            }
            return false;
        }

        public static bool GetRigProfitInternal()
        {
            return Form_Main.lastRigProfit.Success;
        }

        public static bool GetRigProfitInternalRUN(bool force = false)
        {
            try
            {
                if (ConfigManager.GeneralConfig.EnableAPIkeys || force)
                {
                    string apistr = Links.ServerTime;
                    string resp;
                    resp = NiceHashStats.GetNiceHashApiDataWithSecret(apistr, false);
                    if (resp != null)
                    {
                        dynamic respTime = JsonConvert.DeserializeObject(resp);
                        serverTime = respTime.serverTime;
                    } else
                    {
                        serverTime = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                    }
                    /*
                    //надо давать разрешение на просмотр баланса
                    if (true)
                    {
                        apistr = Links.Balance;
                        resp = NiceHashStats.GetNiceHashApiDataWithSecret(apistr, true);
                        if (resp != null)
                        {
                            dynamic respJson = JsonConvert.DeserializeObject(resp);
                            double totalBalance = respJson.totalBalance;
                            Helpers.ConsolePrint("totalBalance", totalBalance.ToString() + " BTC");
                            SetBalance(totalBalance.ToString());
                        }
                    }
                    */
                    apistr = Links.RigDetails + ConfigManager.GeneralConfig.MachineGuid;
                    resp = NiceHashStats.GetNiceHashApiDataWithSecret(apistr, true);
                    if (resp != null)
                    {
                        dynamic respJson = JsonConvert.DeserializeObject(resp);

                        if (respJson.rigId == NiceHashSocket.RigID)
                        {
                            Form_Main.lastRigProfit.Success = true;
                            if (respJson.profitability > Form_Main.lastRigProfit.currentProfitAPI * 100 &&
                                Form_Main.lastRigProfit.currentProfitAPI != 0 && respJson.profitability != 0)
                            {
                                Helpers.ConsolePrint("GetRigProfitInternal too high. Ignoring", (respJson.profitability * 1000).ToString());
                                Form_Main.lastRigProfit.currentProfitAPI = 0;
                                Form_Main.lastRigProfit.Message = "GetRigProfitInternal too high. Ignoring";
                            }
                            else if (respJson.profitability * 100 < Form_Main.lastRigProfit.currentProfitAPI &&
                                Form_Main.lastRigProfit.currentProfitAPI != 0 && respJson.profitability != 0)
                            {
                                Helpers.ConsolePrint("GetRigProfitInternal too low. Ignoring", (respJson.profitability * 1000).ToString());
                                Form_Main.lastRigProfit.currentProfitAPI = 0;
                                Form_Main.lastRigProfit.Message = "GetRigProfitInternal too low. Ignoring";
                            }
                            else
                            {
                                double localProfitability = respJson.localProfitability;
                                if (localProfitability > 0)
                                {
                                    Form_Main.lastRigProfit.totalRate = localProfitability;
                                }
                                Form_Main.lastRigProfit.currentProfitAPI = respJson.profitability;
                                Helpers.ConsolePrint("GetRigProfitInternal", (respJson.profitability * 1000).ToString());
                            }
                        }

                        double unpaidAmount = respJson.unpaidAmount;
                        Helpers.ConsolePrint("Rig unpaidAmount", (unpaidAmount * 1000).ToString() + " mBTC");
                        //SetBalance(unpaidAmount.ToString());//only this rig

                        if (ConfigManager.GeneralConfig.ChartEnable)
                        {
                            Form_Main.TotalProfitabilityFromNH = Form_Main.TotalProfitabilityFromNH + Form_Main.lastRigProfit.currentProfitAPI / 1440;
                        }
                        else
                        {
                            Form_Main.TotalProfitabilityFromNH = 0;
                        }
                    }
                    else
                    {
                        Form_Main.lastRigProfit.currentProfitAPI = 0;
                        Form_Main.lastRigProfit.unpaidAmount = 0;
                        Form_Main.lastRigProfit.Success = false;
                        Form_Main.lastRigProfit.Message = "Response is null";
                        return false;
                    }
                    
                }
                else
                {
                    Form_Main.lastRigProfit.currentProfitAPI = 0;
                    Form_Main.lastRigProfit.unpaidAmount = 0;
                    Form_Main.lastRigProfit.Success = false;
                    Form_Main.lastRigProfit.Message = "Not enabled";
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetRigProfitInternal", ex.Message);
                Form_Main.errorAPIkeystring = ex.Message;
                Form_Main.lastRigProfit.Success = false;
                Form_Main.lastRigProfit.Message = ex.Message;
                return false;
            }
            Form_Main.errorAPIkeystring = "No errors";
            return true;
        }
        public static bool GetRigProfitExternal()
        {
            try
            {
                if (ConfigManager.GeneralConfig.ChartEnable || !Form_Main.walletType.Equals("P2SH"))
                {
                    string apistr = Links.NhmExternal + Globals.GetBitcoinUser() + "/rigs2?sort=NAME&page=0";
                    string resp;
                    resp = NiceHashStats.GetNiceHashApiData(apistr, "");
                    if (resp != null)
                    {
                        //Helpers.ConsolePrint("NHM_API_info", resp);
                        dynamic respJson = JsonConvert.DeserializeObject(resp);
                        var Rigs = respJson.miningRigs;

                        foreach (var rig in Rigs)
                        {
                            if (rig.rigId.ToString() == NiceHashSocket.RigID)
                            {
                                if (rig.profitability > Form_Main.lastRigProfit.currentProfitAPI * 100 &&
                                    Form_Main.lastRigProfit.currentProfitAPI != 0 && rig.profitability != 0)
                                {
                                    Helpers.ConsolePrint("GetRigProfitExternal too high. Ignoring", (rig.profitability * 1000).ToString());
                                    //Helpers.ConsolePrint("Form_Main.lastRigProfit.currentProfitAPI", (Form_Main.lastRigProfit.currentProfitAPI * 1000).ToString());
                                }
                                else if (rig.profitability * 100 < Form_Main.lastRigProfit.currentProfitAPI &&
                                    Form_Main.lastRigProfit.currentProfitAPI != 0 && rig.profitability != 0)
                                {
                                    Helpers.ConsolePrint("GetRigProfitExternal too low. Ignoring", (rig.profitability * 1000).ToString());
                                    //Helpers.ConsolePrint("Form_Main.lastRigProfit.currentProfitAPI", (Form_Main.lastRigProfit.currentProfitAPI * 1000).ToString());
                                }
                                else
                                {
                                    Form_Main.lastRigProfit.currentProfitAPI = rig.profitability;
                                    Helpers.ConsolePrint("GetRigProfitExternal", (rig.profitability * 1000).ToString());
                                }
                            }
                        }
                        double unpaidAmount = respJson.unpaidAmount;
                        double externalBalance = respJson.externalBalance;

                        Helpers.ConsolePrint("Rig unpaidAmount", (unpaidAmount * 1000).ToString() + " mBTC");
                        if (ConfigManager.GeneralConfig.Show_wallet_balance)
                        {
                            Helpers.ConsolePrint("Total externalBalance", (externalBalance * 1000).ToString() + " mBTC");
                            SetBalance(externalBalance.ToString());
                        }
                        else
                        {
                            SetBalance(unpaidAmount.ToString());
                        }

                        if (ConfigManager.GeneralConfig.ChartEnable)
                        {
                            Form_Main.TotalProfitabilityFromNH = Form_Main.TotalProfitabilityFromNH + Form_Main.lastRigProfit.currentProfitAPI / 1440;
                            //Helpers.ConsolePrint("TotalProfitabilityFromNH", (Form_Main.TotalProfitabilityFromNH).ToString());
                        }
                        else
                        {
                            Form_Main.TotalProfitabilityFromNH = 0;
                        }
                    }
                    else
                    {
                        Form_Main.lastRigProfit.currentProfitAPI = 0;
                        Form_Main.lastRigProfit.unpaidAmount = 0;
                    }
                }
                else
                {
                    Form_Main.lastRigProfit.currentProfitAPI = 0;
                    Form_Main.lastRigProfit.unpaidAmount = 0;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetRigProfitExternal", ex.Message);
                return false;
            }
            return false;

        }

        public static bool emptypool = true;
        [HandleProcessCorruptedStateExceptions]
        public static bool GetSmaAPI(bool immediately = false)
        {
            bool ret = false;
            if (!emptypool) return false;

            if (!immediately)
            {
                emptypool = false;
                do
                {
                    Thread.Sleep(500);
                } while (Form_Main.Uptime.Seconds != 15 && Form_Main.Uptime.Seconds != 45);
            }
            emptypool = true;

            if (Form_Main.Uptime.Seconds == 45)//фактическая прибыльность 1 раз в минуту
            {
                new Task(() => GetRigProfitInternalRUN()).Start();
            }

            try
            {
                //запускать последовательно в одном потоке
                ret = GetSmaAPICurrent();
                if (ConfigManager.GeneralConfig.Use_Last24hours)
                {
                    NiceHashStats.GetSmaAPI24h();
                }
                if (ConfigManager.GeneralConfig.Use_orders_price)
                {
                    GetSmaAPIOrder();
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("SOCKET", ex.Message);
            }
            return ret;
        }

        public static void LoadSMA()
        {
            string defsma = "[[46,\"2.2546252877e-05\"],[20,\"1.1387477623e-04\"],[47,\"9.1086414342e-01\"],[50,\"1.7406077348e+04\"],[21,\"2.5000000000e-09\"],[62,\"2.7613822484e-05\"],[48,\"1.0112168714e-08\"],[60,\"1.0179830311e-04\"],[5,\"5.3030000000e-07\"],[32,\"1.3000000000e-03\"],[42,\"1.0000000000e-01\"],[23,\"5.1948323410e-08\"],[24,\"3.8460079677e-01\"],[39,\"1.0000000000e+02\"],[56,\"2.1799406971e-04\"],[57,\"5.8912717827e-05\"],[52,\"3.4559911985e-04\"],[43,\"1.3005183421e+03\"],[54,\"2.6833765269e+02\"],[33,\"6.2358921162e-06\"],[36,\"1.1025005778e+02\"],[28,\"7.4205507607e-09\"],[8,\"3.4233713066e-03\"],[14,\"2.4701000000e-06\"],[61,\"3.1005608253e-04\"],[58,\"1.6037806480e+02\"]]";
            try
            {
                if (System.IO.File.Exists("configs\\sma.dat"))
                {
                    dynamic jsonData = (File.ReadAllText("configs\\sma.dat"));
                    Helpers.ConsolePrint("LoadSMA", "Using previous SMA");
                    JArray smadata = (JArray.Parse(jsonData));
                    SetAlgorithmRates(smadata, 1, 12, false, "WS");
                }
                else
                {
                    Helpers.ConsolePrint("LoadSMA", "Using default SMA");
                    JArray smadata = (JArray.Parse(defsma));
                    SetAlgorithmRates(smadata, 1, 12, false, "WS");
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("SOCKET", ex.Message);
                Helpers.ConsolePrint("SOCKET", "Using default SMA");
                JArray smadata = (JArray.Parse(defsma));
                SetAlgorithmRates(smadata, 1, 12, false, "WS");
                Helpers.ConsolePrint("OLDSMA", ex.ToString());
            }
            GetSmaAPICurrent();
        }
        #endregion

        #region Incoming socket calls
        public static void ClearAlgorithmRates()
        {
            var _currentSma = new Dictionary<AlgorithmType, NiceHashSma>();
            var payingDict = new Dictionary<AlgorithmType, double>();
            try
            {
                foreach (AlgorithmType algo in Enum.GetValues(typeof(AlgorithmType)))
                {
                    if (algo >= 0)
                    {
                        var paying = 0d;

                        _currentSma[algo] = new NiceHashSma
                        {
                            Port = (int)algo + 3333,
                            Name = algo.ToString().ToLower(),
                            Algo = (int)algo,
                            Paying = paying
                        };
                        payingDict[algo] = paying;
                    }
                }

                NHSmaData.UpdateSmaPaying(payingDict);

                Thread.Sleep(10);
                OnSmaUpdate?.Invoke(null, EventArgs.Empty);

            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.Message);
            }
        }
        public static void SetAlgorithmRates(JArray data, int multipl = 1, double treshold = 12.0,
            bool average = true, string type = "WS")
        {
            double mult = 1;
            double paying = 0.0d;
            if (ConfigManager.GeneralConfig.NicehashMiningFee)
            {
                mult = multipl * 0.98;//nicehash mining fee
            } else
            {
                mult = multipl * 1.0;
            }

            try
            {
                var payingDict = new Dictionary<AlgorithmType, double>();
                if (data != null)
                {
                    foreach (var algo in data)
                    {
                        if (algo == null) return;
                        var algoKey = (AlgorithmType)algo[0].Value<int>();

                        if (algoKey.ToString().Contains("UNUSED"))
                        {
                            continue;
                        }

                        if (!NHSmaData.TryGetPaying(algoKey, out double payingFromDict))
                        {
                            Helpers.ConsolePrint("SetAlgorithmRates", "ERROR! Unknown algo: " + algoKey.ToString());
                        }

                        paying = Math.Abs(algo[1].Value<double>() * mult);

                        if (double.IsNaN(paying))
                        {
                            paying = 0;
                        }

                        if (algoKey == AlgorithmType.KAWPOWLite && !Divert.KawpowLiteGoodEpoch)
                        {
                            paying = 0;
                            Helpers.ConsolePrint("SetAlgorithmRates", "KawpowLiteGoodEpoch false. Set paying to 0");
                        }

                         if (!ConfigManager.GeneralConfig.Use_Last24hours)
                         {
                            if (!algoKey.ToString().Contains("UNUSED") && type.ToLower().Contains("ws"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, paying, average);//first init?
                                if (algoKey == AlgorithmType.DaggerHashimoto)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, paying, average);
                                }
                                if (algoKey == AlgorithmType.KAWPOW)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, paying, false);
                                }
                            }

                            if (paying != 0 && !algoKey.ToString().Contains("UNUSED")
                                && type.ToLower().Equals("current"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, paying, true);
                                if (algoKey == AlgorithmType.DaggerHashimoto)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, paying, true);
                                }
                                if (algoKey == AlgorithmType.KAWPOW)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, paying, false);
                                }
                            }

                            if (paying != 0 && !algoKey.ToString().Contains("UNUSED")
                                && type.ToLower().Equals("order"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, paying, true);
                                if (algoKey == AlgorithmType.DaggerHashimoto)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, paying, false);
                                }
                                if (algoKey == AlgorithmType.KAWPOW)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, paying, false);
                                }
                            }
                        }
                        else
                        {
                            if (!algoKey.ToString().Contains("UNUSED") && type.ToLower().Equals("ws"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, paying, average);//first init?
                                if (algoKey == AlgorithmType.DaggerHashimoto)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, paying, false);
                                }
                                if (algoKey == AlgorithmType.KAWPOW)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, paying, false);
                                }
                            }

                            if (paying != 0 && !algoKey.ToString().Contains("UNUSED") &&
                                type.ToLower().Equals("current"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, paying, true);
                                if (algoKey == AlgorithmType.DaggerHashimoto)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, paying, false);
                                }
                                if (algoKey == AlgorithmType.KAWPOW)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, paying, false);
                                }
                            }

                            if (paying != 0 && !algoKey.ToString().Contains("UNUSED") &&
                                type.ToLower().Equals("order"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, paying, true);
                                if (algoKey == AlgorithmType.DaggerHashimoto)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, paying, false);
                                }
                                if (algoKey == AlgorithmType.KAWPOW)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, paying, false);
                                }
                            }

                            if (paying != 0 && !algoKey.ToString().Contains("UNUSED") &&
                                type.ToLower().Equals("24h"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, paying, true);
                                if (algoKey == AlgorithmType.DaggerHashimoto)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, paying, false);
                                }
                                if (algoKey == AlgorithmType.KAWPOW)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, paying, false);
                                }
                            }
                        }
                    }
                }

                //testing
                //payingDict[AlgorithmType.RandomX] = 12345;

                NHSmaData.UpdateSmaPaying(payingDict, average);

                Thread.Sleep(10);
                OnSmaUpdate?.Invoke(null, EventArgs.Empty);

            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
            }
        }

        private static double SetProf(string prof)
        {
            double profitabilityFromNH = 0d;
            try
            {
                if (double.TryParse(prof, NumberStyles.Float, CultureInfo.InvariantCulture, out profitabilityFromNH))
                {
                    Form_Main.profitabilityFromNH = profitabilityFromNH;
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
                return 0d;
            }
            return profitabilityFromNH;
        }
        private static void SetBalance(string balance)
        {
            try
            {
                if (double.TryParse(balance, NumberStyles.Float, CultureInfo.InvariantCulture, out var bal))
                {
                    Balance = bal;
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
            }
        }

        private static void SetExchangeRates(string data)
        {
            //Helpers.ConsolePrint("SetExchangeRates", data);
            try
            {
                var exchange = JsonConvert.DeserializeObject<ExchangeRateJson>(data);
                if (exchange?.exchanges_fiat != null && exchange.exchanges != null)
                {
                    foreach (var exchangePair in exchange.exchanges)
                    {
                        if (exchangePair.TryGetValue("coin", out var coin) &&
                            coin == "BTC" &&
                            exchangePair.TryGetValue("USD", out var usd) &&
                            double.TryParse(usd, NumberStyles.Float, CultureInfo.InvariantCulture, out var usdD))
                        {
                            ExchangeRateApi.UsdBtcRate = usdD;
                            break;
                        }
                    }
                    ExchangeRateApi.UpdateExchangesFiat(exchange.exchanges_fiat);
                    Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
            }
        }

        #endregion

        #region Outgoing socket calls

        public static async Task SetCredentials(string btc, string worker)
        {
            return;
            var data = new NicehashCredentials
            {
                btc = btc,
                worker = worker
            };
            if (BitcoinAddress.ValidateBitcoinAddress(data.btc) && BitcoinAddress.ValidateWorkerName(worker))
            {
                var sendData = JsonConvert.SerializeObject(data);
                if (_socket != null)
                {
                    await _socket.SendData(sendData);
                }
            }
        }
        internal static TcpClient tcpClientGoogle = null;
        public static void ConnectToGoogle(string request = "GET / HTTP/1.1\r\n\r\n")
        {
            if (!ConfigManager.GeneralConfig.DivertRun)
            {
                Form_Main.GoogleAnswer = "";
                return;
            }
            try
            {
                tcpClientGoogle = new TcpClient();
                Form_Main.GoogleIP = Form_Main.DNStoIP("www.google.com");
                tcpClientGoogle.SendTimeout = 1000 * 1;
                tcpClientGoogle.ReceiveTimeout = 1000 * 1;
                tcpClientGoogle.Connect(Form_Main.GoogleIP, 80);
                NetworkStream serverStream = tcpClientGoogle.GetStream();
                serverStream.WriteTimeout = 1000 * 1;
                serverStream.ReadTimeout = 1000 * 1;

                byte[] messageGoogle = new byte[1024];
                int GoogleBytes;
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                var Request = enc.GetBytes(request);

                if (serverStream == null)
                {
                    Helpers.ConsolePrint("ConnectToGoogle", "Error in serverStream");
                    return;
                }
                serverStream.Write(Request, 0, Request.Length);
                if (tcpClientGoogle.Connected)
                {
                    for (int i = 0; i < 1024; i++)
                    {
                        messageGoogle[i] = 0;
                    }
                    GoogleBytes = serverStream.Read(messageGoogle, 0, 1024); //HTTP/1.1 200 OK
                    Form_Main.GoogleAnswer = Encoding.ASCII.GetString(messageGoogle);
                    Form_Main.GoogleAvailable = true;
                    //Helpers.ConsolePrint("ConnectToGoogle", "Answer: " + GoogleAnswer);
                    if (tcpClientGoogle != null)
                    {
                        tcpClientGoogle.Client.Disconnect(false);
                        tcpClientGoogle.Client.Shutdown(SocketShutdown.Both);
                        tcpClientGoogle.Close();
                        tcpClientGoogle.Dispose();
                        tcpClientGoogle = null;
                    }
                    serverStream.Close();
                    serverStream.Dispose();
                    serverStream = null;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("ConnectToGoogle", "Disconnected: " + ex.Message);
                Form_Main.GoogleAvailable = false;
                if (tcpClientGoogle != null)
                {
                    tcpClientGoogle.Client.Disconnect(false);
                    tcpClientGoogle.Client.Shutdown(SocketShutdown.Both);
                    tcpClientGoogle.Close();
                    tcpClientGoogle.Dispose();
                    tcpClientGoogle = null;
                }
            }
        }

        public static List<(AlgorithmType type, double speed)> GetSpeedForDevice(string deviceUuid)
        {
            var ret = new List<(AlgorithmType type, double speed)>();
            var devData = ComputeDeviceManager.Available.Devices.FirstOrDefault(dev => dev.DevUuid == deviceUuid);
            if (devData.AlgorithmID != (int)AlgorithmType.Empty && devData.AlgorithmID != (int)AlgorithmType.NONE)
            {
                ret.Add(((AlgorithmType)devData.AlgorithmID, devData.MiningHashrate));
            }
            if (devData.SecondAlgorithmID != (int)AlgorithmType.Empty && devData.SecondAlgorithmID != (int)AlgorithmType.NONE)
            {
                ret.Add(((AlgorithmType)devData.SecondAlgorithmID, devData.MiningHashrateSecond));
            }
            if (devData.ThirdAlgorithmID != (int)AlgorithmType.Empty && devData.ThirdAlgorithmID != (int)AlgorithmType.NONE)
            {
                ret.Add(((AlgorithmType)devData.ThirdAlgorithmID, devData.MiningHashrateThird));
            }
            return ret;
        }


        public static RigStatus CalcRigStatus()
        {
            /*
            if (!isInitFinished)
            {
                return RigStatus.Pending;
            }
            */
            // TODO check if we are connected to ws if not retrun offline state

            // check devices
            var allDevs = ComputeDeviceManager.Available.Devices;
            // now assume we have all disabled
            var rigState = RigStatus.Disabled;
            // ORDER MATTERS!!!, we are excluding pending state
            var anyDisabled = allDevs.Any(dev => dev.IsDisabled);
            if (anyDisabled)
            {
                rigState = RigStatus.Disabled;
            }
            var anyStopped = allDevs.Any(dev => dev.State == DeviceState.Stopped);
            if (anyStopped)
            {
                rigState = RigStatus.Stopped;
            }
#if NHMWS4
            var anyMining = allDevs.Any(dev => dev.State == DeviceState.Mining || dev.State == DeviceState.Gaming);
#else
            var anyMining = allDevs.Any(dev => dev.State == DeviceState.Mining);
#endif
            if (anyMining)
            {
                rigState = RigStatus.Mining;
            }
            var anyBenchmarking = allDevs.Any(dev => dev.State == DeviceState.Benchmarking);
            if (anyBenchmarking)
            {
                rigState = RigStatus.Benchmarking;
            }
            var anyError = allDevs.Any(dev => dev.State == DeviceState.Error);
            if (anyError)
            {
                rigState = RigStatus.Error;
            }
            return rigState;
        }


        public static void DeviceStatus_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                var _curState = NiceHashSocket._webSocket.ReadyState;
                if (_curState == WebSocketSharp.WebSocketState.Open)
                {
                    SetDeviceStatus(null, false, "DeviceStatus_Tick");
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("DeviceStatus_Tick", ex.ToString());
            }
        }

        public static async void SetDeviceStatus(object state, bool devName = false, string from = "")
        {
            Helpers.ConsolePrint("SetDeviceStatus", "DeviceStatusRunning: " + DeviceStatusRunning +
                " called from: " + from);
            if (DeviceStatusRunning) return;
            DeviceStatusRunning = true;
            var devicesOld = ComputeDeviceManager.Available.Devices;

            var _computeDevicesResort = ComputeDeviceManager.ReSortDevices(devicesOld);
            var _computeDevices = devicesOld;

            var rigStatus = CalcRigStatusString();
            var activeIDs = MinersManager.GetActiveMinersIndexes();
            double HashRate = 0.0d;
            double SecondHashRate = 0.0d;
            double ThirdHashRate = 0.0d;

            if (state != null)
                rigStatus = state.ToString();

            var paramList = new List<JToken>
            {
                rigStatus
            };

            var deviceList = new JArray();
            var devices = new JArray();
            try
            {
                for (int dev = 0; dev < _computeDevices.Count; dev++)
                {
                    var device = _computeDevices[dev];
                    var deviceResort = _computeDevicesResort[dev];

                    try
                    {
                        if (device.Enabled)
                        {
                            /*
                            if (Miner.IsRunningNew)
                            {
                                device.State = DeviceState.Mining;
                                deviceResort.State = DeviceState.Mining;
                            }
                            else
                            {
                                device.State = DeviceState.Stopped;
                                device.AlgorithmID = (int)AlgorithmType.NONE;
                                device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                                device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                                deviceResort.State = DeviceState.Stopped;
                                deviceResort.AlgorithmID = (int)AlgorithmType.NONE;
                                deviceResort.SecondAlgorithmID = (int)AlgorithmType.NONE;
                                deviceResort.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            }
                            */
                        } else// чтоб не было в кабинете - "некоторые устройства не майнят"
                        {
                            device.State = DeviceState.Disabled;
                            deviceResort.State = DeviceState.Disabled;
                            device.AlgorithmID = (int)AlgorithmType.NONE;
                            device.SecondAlgorithmID = (int)AlgorithmType.NONE;
                            device.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                            deviceResort.AlgorithmID = (int)AlgorithmType.NONE;
                            deviceResort.SecondAlgorithmID = (int)AlgorithmType.NONE;
                            deviceResort.ThirdAlgorithmID = (int)AlgorithmType.NONE;
                        }
                       

                        int status = 0;
                        if (device.DeviceType == DeviceType.CPU)
                        {
                            status = 8;
                        }
                        if (device.DeviceType == DeviceType.NVIDIA)
                        {
                            status = 16;
                        }
                        if (device.DeviceType == DeviceType.AMD)
                        {
                            status = 24;
                        }
                        if (device.DeviceType == DeviceType.INTEL)
                        {
                            status = 24;
                        }
                        
                        //В оригинальном NH при второй отправке данных вместо названия
                        //устройства (Manufacturer + deviceName) = null
                        //Вместо nuuid используется порядковый номер устройства (string). Без проверки на уникальность!!!
                        //{"method":"miner.status","params":["STOPPED",[["","0",
                        //Оставим как правильно
                        //{"method":"miner.status","params":["STOPPED",[["Intel(R) Core(TM) i7-3630QM CPU @ 2.40GHz","1-YBxRn6UfL1O7dUk6NNR5EA",
                    var array = new JArray
                    {
                        device.NameCustom,
                        device.DevUuid
                    };

                        int rigs = 0;
                        if (rigStatus == "STOPPED")
                        {
                            rigs = 0;
                        }
                        if (rigStatus == "MINING")
                        {
                            rigs = 1;
                        }
                        if (rigStatus == "PENDING")
                        {
                            rigs = 0;
                        }
                        if (device.Enabled)
                        {
                            status = status + rigs + Convert.ToInt32(device.Enabled);

                        }
                        array.Add(status);

                        //Если ADL2_New_QueryPMLogData_Get отдает ERR_NOT_SUPPORTED = -8, то MSI AB всё-равно рисует
                        //график загрузки контроллера памяти на AMD, что есть чудо! И этих данных нет в mahm.
                        //Походу, MSI AB просто рисует фейковый график в этом случае.
                        //Helpers.ConsolePrint("********", MSIAfterburner.GetDeviceMemoryLoad(device.BusID).Data.ToString());

                        //array.Add((int)Math.Round(device.Load));
                        int memload = (int)Math.Round(device.MemLoad);
                        if (device.DeviceType == DeviceType.INTEL)
                        {
                            array.Add((int)Math.Round(device.Load));
                        }
                        else
                        {
                            array.Add(memload << 16 | (int)Math.Round(device.Load));//Загрузка контроллера памяти? Кому это надо?
                        }
                        var speedsJson = new JArray();

                        HashRate = device.MiningHashrate;
                        SecondHashRate = device.MiningHashrateSecond;
                        ThirdHashRate = device.MiningHashrateThird;

                        if (rigs == 1)
                        {
                            if (device.AlgorithmID > 0)
                            {
                                speedsJson.Add(new JArray(device.AlgorithmID, HashRate)); //  номер алгоритма, хешрейт
                            }
                            if (device.SecondAlgorithmID > 0)
                            {
                                speedsJson.Add(new JArray(device.SecondAlgorithmID, SecondHashRate));
                            }
                            if (device.ThirdAlgorithmID > 0)
                            {
                                speedsJson.Add(new JArray(device.ThirdAlgorithmID, ThirdHashRate));
                            }
                        }

                        if (rigs == 1 & (device.AlgorithmID == (int)AlgorithmType.KAWPOWLite)) //KawpowLite
                        {
                            speedsJson.Add(new JArray((int)AlgorithmType.KAWPOW, HashRate)); //  номер алгоритма, хешрейт
                        }

                        array.Add(speedsJson);

                        //костыль для amd
                        float TempMemory = device.TempMemory;
                        float TempMemoryResort = deviceResort.TempMemory;
                        if (TempMemory < 0) TempMemory = 0;
                        if (TempMemoryResort < 0) TempMemoryResort = 0;

                        // Hardware monitoring
                        if (!Form_Main.NVIDIA_orderBug)
                        {
                            array.Add(Math.Round(Math.Round(device.Temp)));
                            array.Add(device.FanSpeedRPM);
                            array.Add((int)Math.Round(device.PowerUsage));
                        }
                        else
                        {
                            array.Add(Math.Round(Math.Round(deviceResort.Temp)));
                            array.Add(deviceResort.FanSpeedRPM);
                            array.Add((int)Math.Round(deviceResort.PowerUsage));
                        }
                        // Power mode
                        array.Add(-1);

                        // Intensity mode
                        array.Add(0);

                        //fan speen percent
                        if (!Form_Main.NVIDIA_orderBug)
                        {
                            if (device.DeviceType != DeviceType.CPU)
                            {
                                array.Add(device.FanSpeed);
                            }
                            else
                            {
                                array.Add(-1);
                            }
                        }
                        else
                        {
                            if (deviceResort.DeviceType != DeviceType.CPU)
                            {
                                array.Add(deviceResort.FanSpeed);
                            }
                            else
                            {
                                array.Add(-1);
                            }
                        }

                        if (ConfigManager.GeneralConfig.QM_mode)
                        {
                            int memTemp = (int)device.TempMemory + 128;
                            //array.Add("V=1;CCC=0;CVC=0;MCC=0;MCS=0;MCD=0;MT=" + memTemp.ToString() + ";KTUMED=-2;OP=-2;OPA=EfficientLow:12,Efficient:11,High:3,Medium:2,Lite:1;");
                            array.Add("V=1;MT=" + memTemp.ToString() + ";OP=-1;OPA=Manual:0;");
                        }
                        deviceList.Add(array);
                    }
                    catch (Exception ex) { Helpers.ConsolePrint("SOCKET", ex.ToString()); }
                    DeviceStatusRunning = false;
                }
                paramList.Add(deviceList);

                var data = new MinerStatusMessage
                {
                    param = paramList
                };
                var sendData = JsonConvert.SerializeObject(data);
                //

                IOrderedEnumerable<ComputeDevice> computeDevices = null;
                if (!Form_Main.NVIDIA_orderBug)
                {

                    computeDevices = _computeDevices.OrderBy(d => d.DeviceType).ThenBy(d => d.BusID);
                } else
                {
                    computeDevices = _computeDevicesResort.OrderBy(d => d.DeviceType).ThenBy(d => d.BusID);
                }

                var _login = NiceHashMiner.Stats.V4.MessageParserV4.CreateLoginMessage(Configs.ConfigManager.GeneralConfig.BitcoinAddressNew,
                    Configs.ConfigManager.GeneralConfig.WorkerName, Configs.ConfigManager.GeneralConfig.MachineGuid,
                    computeDevices);
                V4.MinerState nextState = NiceHashMiner.Stats.V4.MessageParserV4.GetMinerState(_login.Worker, computeDevices);
                var nextStateJson4 = JsonConvert.SerializeObject(nextState);
                //{"method":"miner.state","mdv":[2],"odv":["527","fe80::245a:3b6d:a118:84ad%22"],"mmv":[2,"worker1"],"devices":[{"mdv":[4,[]],"odv":["71","1",""],"mmv":[4]},{"mdv":[2,[[58,45.88]]],"odv":["70","90","100","0","74","miniZ 2.2c"],"mmv":[2]}]}
                if (_socket != null)
                {
                    if (Form_Main.NHMWSProtocolVersion == 4)
                    {
                        await _socket.SendData(nextStateJson4);
                    }
                    else
                    {
                        await _socket.SendData(sendData);
                    }
                } else
                {
                    Helpers.ConsolePrint("SetDeviceStatus", "Socket error!");
                    _socket = null;
                    Thread.Sleep(1000);
                    new Task(() => StartConnection("")).Start();
                }
            }
            catch (Exception ex2)
            {
                Helpers.ConsolePrint("SetDeviceStatus", ex2.ToString());
                DeviceStatusRunning = false;
            }
            DeviceStatusRunning = false;
        }

        #endregion
        private static int _location = 0;
        public static string GetNiceHashApiData(string url, string worker)
        {
            bool proxy = false;//test
            string proxyUrl = "";
            if (ConfigManager.GeneralConfig.ServiceLocation > 0)
            {
                proxy = true;
                proxyUrl = Globals.MiningLocation[_location];
            }

            if (ConfigManager.GeneralConfig.ServiceLocation > 0 && Form_Main.apiConnectionsErrors > 3)
            {
                _location++;
                if (_location >= Globals.MiningLocation.Length)
                {
                    _location = 0;
                }
                proxyUrl = Globals.MiningLocation[_location];
            }

            var uri = new Uri(url);
            if (proxy)
            {
                url = url.Replace("api2.nicehash.com", proxyUrl + ":7443");
            }
            string host = new Uri(url).Host;
            var responseFromServer = "";
            try
            {
                var activeMinersGroup = MinersManager.GetActiveMinersGroup();
                ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var wr = (HttpWebRequest)WebRequest.Create(new Uri(url));

                string RequestId = System.Guid.NewGuid().ToString().Replace("-", "");

                //wr.UserAgent = "NiceHashMiner/" + ConfigManager.GeneralConfig.NHMVersion;
                wr.UserAgent = "name=Edge;version=100.0.1185.39;buildNumber=1;os=Windows;osVersion=10;deviceVersion=amd64;lang=en";
                wr.Headers.Add("X-Request-Id", RequestId);
                wr.Headers.Add("X-User-Lang", "en");

                wr.Host = "api2.nicehash.com:443";
                wr.Timeout = 5 * 1000;
                var response = wr.GetResponse();
                var ss = response.GetResponseStream();
                if (ss != null)
                {
                    ss.ReadTimeout = 3 * 1000;
                    var reader = new StreamReader(ss);
                    responseFromServer = reader.ReadToEnd();
                    if (responseFromServer.Length == 0 || responseFromServer[0] != '{')
                        throw new Exception("Not JSON!");
                    reader.Close();
                }
                response.Close();
            }
            catch (WebException wex)
            {
                Helpers.ConsolePrint("GetNiceHashApiData", wex.Message);
                Form_Main.apiConnectionsErrors++;
                return null;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetNiceHashApiData", ex.ToString());
                Form_Main.apiConnectionsErrors++;
                return null;
            }
            Form_Main.apiConnectionsErrors = 0;
            return responseFromServer;
        }
        private static string HashBySegments(string key, string apiKey, string time, string nonce, string orgId, string method, string encodedPath, string query, string bodyStr)
        {
            List<string> segments = new List<string>();
            segments.Add(apiKey);
            segments.Add(time);
            segments.Add(nonce);
            segments.Add(null);
            segments.Add(orgId);
            segments.Add(null);
            segments.Add(method);
            segments.Add(encodedPath == null ? null : encodedPath);
            segments.Add(query == null ? null : query);

            if (bodyStr != null && bodyStr.Length > 0)
            {
                segments.Add(bodyStr);
            }
            return CalcHMACSHA256Hash(JoinSegments(segments), key);
        }
        private static string JoinSegments(List<string> segments)
        {
            var sb = new System.Text.StringBuilder();
            bool first = true;
            foreach (var segment in segments)
            {
                if (!first)
                {
                    sb.Append("\x00");
                }
                else
                {
                    first = false;
                }

                if (segment != null)
                {
                    sb.Append(segment);
                }
            }
            return sb.ToString();
        }
        private static string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.Default;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new System.Security.Cryptography.HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }
        private static string getPath(string url)
        {
            var arrSplit = url.Split('?');
            return arrSplit[0];
        }
        private static string getQuery(string url)
        {
            var arrSplit = url.Split('?');

            if (arrSplit.Length == 1)
            {
                return null;
            }
            else
            {
                return arrSplit[1];
            }
        }
        public static string GetNiceHashApiDataWithSecret(string url, bool auth)
        {
            bool proxy = false;//test
            string proxyUrl = "";
            if (ConfigManager.GeneralConfig.ServiceLocation > 0)
            {
                proxy = true;
                proxyUrl = Globals.MiningLocation[_location];
            }

            if (ConfigManager.GeneralConfig.ServiceLocation > 0 && Form_Main.apiConnectionsErrors > 3)
            {
                _location++;
                if (_location >= Globals.MiningLocation.Length)
                {
                    _location = 0;
                }
                proxyUrl = Globals.MiningLocation[_location];
            }

            proxyUrl = Links.CheckDNS(proxyUrl).Replace("stratum+tcp://", "");
            var uri = new Uri(url);
            if (proxy)
            {
                url = url.Replace("api2.nicehash.com", proxyUrl + ":7443");
            }
            string host = new Uri(url).Host;
            var responseFromServer = "";

            if ((Form_Main.orgId + Form_Main.apiKey + Form_Main.apiSecret).IsNullOrEmpty())
            {
                new Task(() => Form_API_keys.GetSavedAPIkeyData()).Start();
            }

            string orgId = Form_Main.orgId;
            string apiKey = Form_Main.apiKey;
            string apiSecret = Form_Main.apiSecret;


            try
            {
                ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var wr = (HttpWebRequest)WebRequest.Create(new Uri(url));
                if (auth)
                {
                    string nonce = System.Guid.NewGuid().ToString().Replace("-", "");
                    string RequestId = System.Guid.NewGuid().ToString().Replace("-", "");
                    string digest = HashBySegments(apiSecret, apiKey, serverTime, nonce, orgId, "GET", getPath(uri.LocalPath), getQuery(uri.Query), null);
                    //wr.UserAgent = "NiceHashMiner/" + Application.ProductVersion;
                    wr.UserAgent = "name=Edge;version=100.0.1185.39;buildNumber=1;os=Windows;osVersion=10;deviceVersion=amd64;lang=en";
                    wr.Headers.Add("X-Time", serverTime);
                    wr.Headers.Add("X-Nonce", nonce);
                    wr.Headers.Add("X-Organization-Id", orgId);
                    wr.Headers.Add("X-Auth", apiKey + ":" + digest);
                    wr.Headers.Add("X-Request-Id", RequestId);
                    wr.Headers.Add("X-User-Lang", "en");
                }
                wr.Host = "api2.nicehash.com:443";
                wr.Timeout = 2 * 1000;
                var response = wr.GetResponse();
                var ss = response.GetResponseStream();
                if (ss != null)
                {
                    ss.ReadTimeout = 2 * 1000;
                    var reader = new StreamReader(ss);
                    responseFromServer = reader.ReadToEnd();
                    if (responseFromServer.Length == 0 || responseFromServer[0] != '{')
                        throw new Exception("Not JSON!");
                    reader.Close();
                }
                response.Close();
            }
            catch (WebException wex)
            {
                Helpers.ConsolePrint("GetNiceHashApiDataWithSecret", wex.Message);
                Form_Main.errorAPIkeystring = wex.Message;
                if (auth)
                {
                    Form_Main.apiConnectionsErrors++;
                    Form_Main.NicehashAPIerrorDescription = "API error: " + wex.Message.Split(':')[1];
                }
                return null;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetNiceHashApiDataWithSecret", ex.ToString());
                Form_Main.errorAPIkeystring = ex.Message;
                Form_Main.apiConnectionsErrors++;
                return null;
            }
            if (auth)
            {
                Form_Main.apiConnectionsErrors = 0;
                Form_Main.NicehashAPIerrorDescription = "";
            }
            return responseFromServer;
        }

        public static string CalcRigStatusString()
        {
            if (Miner.IsRunningNew)
            {
                return "MINING";
            }
            else
            {
                return "STOPPED";
            }
        }
    }
}


namespace TimerDispose
{
    /// <summary>
    /// A timer-containing class that can be disposed safely by allowing the timer
    /// callback that it must exit/cancel its processes
    /// </summary>
    class TimerOwner : IDisposable
    {
        const int dueTime = 5 * 100;       //halve a second
        const int timerPeriod = 1 * 1000;   //Repeat timer every one second (make it Timeout.Inifinite if no repeating required)

        private TimerCanceller timerCanceller = new TimerCanceller();

        private System.Threading.Timer timer;


        public void releaseTimer()
        {
            timerCanceller.Cancelled = true;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            timer.Dispose();
        }

        public void Dispose()
        {
            releaseTimer();
            GC.SuppressFinalize(this);
        }
    }

    class TimerCanceller
    {
        public bool Cancelled = false;
    }

}

