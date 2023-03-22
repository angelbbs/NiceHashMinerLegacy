using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Forms;
using NiceHashMiner.Miners;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
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

        private const int DeviceUpdateLaunchDelay = 20 * 1000;
        private const int DeviceUpdateInterval = 45 * 1000;

        private static bool RigProfitsFirstRun = false;

        public static double Balance { get; private set; }
        public static string Version = "";

        public static bool IsAlive => _socket?.IsAlive ?? false;

        public static event EventHandler OnSmaUpdate;
        public static event EventHandler<SocketEventArgs> OnVersionBurn;

        public static NiceHashSocket _socket;
        public static NiceHashSocket _socketold;

        public static System.Timers.Timer _deviceUpdateTimer;

        public static bool remoteMiningStart = false;
        public static bool remoteMiningStop = false;
        public static bool remoteUpdateUI = false;
        private static bool DeviceStatusRunning = false;

        private static List<AlgorithmType> smaAlgos = new List<AlgorithmType>();
        private static List<string> markets = new List<string>();
        public static string serverTime;

        private static void LoadCachedSMAData()
        {
            if (File.Exists("configs\\sma.dat"))
            {
                try
                {
                    dynamic jsonData = (File.ReadAllText("configs\\sma.dat"));
                    Helpers.ConsolePrint("LoadCachedSMAData", "Using previous SMA");
                    JArray smadata = (JArray.Parse(jsonData));
                    SetAlgorithmRates(smadata, 1, 12, false);//LoadCachedSMAData
                }
                catch (Exception er)
                {
                    Helpers.ConsolePrint("SMA.DAT", er.ToString());
                }
            }
        }

        public static void StartConnection(string address)
        {
            try
            {
                _deviceUpdateTimer = new System.Timers.Timer(DeviceUpdateInterval);
                _deviceUpdateTimer.Elapsed += DeviceStatus_TickNew;
                _deviceUpdateTimer.Start();

                NHSmaData.InitializeIfNeeded();
                //LoadCachedSMAData();
                _socket = null;
                _socket = new NiceHashSocket(address);

                _socket.OnDataReceived += SocketOnOnDataReceived;

                Helpers.ConsolePrint("SOCKET-address:", address);
                new Task(() => _socket.StartConnectionNew()).Start();
                //_socket.StartConnectionNew();
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
            try
            {
                if (e.IsText)
                {
                    if (!e.Data.Contains("exchange_rates"))
                    {
                        Helpers.ConsolePrint("SOCKET", "Received: " + e.Data);
                    }
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
                                if (Form_Main.SMAdelayTick < 30) break;
                                Form_Main.SMAdelayTick = 0;
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
                                } else
                                {
                                    if (firstSMA)
                                    {
                                        firstSMA = false;
                                        
                                        Thread.Sleep(500);
                                        SetAlgorithmRates(message.data, 1, 12, false, "WS");
                                        NiceHashStats.GetSmaAPI(true);
                                        NHSmaData.FinalizeSma();

                                    } else
                                    {
                                        do
                                        {
                                            Thread.Sleep(500);
                                        } while (Form_Main.Uptime.Seconds != 5 && Form_Main.Uptime.Seconds != 35);
                                        //SetAlgorithmRates(message.data, 1, 12, true, "WS");
                                    }
                                }

                                if (Miner.IsRunningNew)
                                {
                                    Form_Main.smaCount++;
                                }
                                else
                                {
                                    Form_Main.smaCount = 0;
                                }
                                if (Form_Main.smaCount > 3)
                                {
                                    dynamic jsonData = (File.ReadAllText("configs\\sma.dat"));
                                    Helpers.ConsolePrint("SocketReceive", "Using previous SMA");
                                    JArray smadata = (JArray.Parse(jsonData));
                                    SetAlgorithmRates(smadata);
                                }

                                if (Form_Main.smaCount > 5)
                                {
                                    Helpers.ConsolePrint("SocketOnOnDataReceived", "PROFIT calc Error. Restart program");

                                    Form_Main.MakeRestart(0);
                                    return;
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
                        //Received: {"method":"mining.disable","id":38019,"device":"3-mMpW1bZrwFK66tGss0WQmA"}
                        //{"method":"mining.disable","id":90934,"device":"3-+BYFhtXwHVS-1+4YlNHOKw"}
                        case "mining.enable":
                            RemoteMiningEnable(message.id.Value.ToString(), message.device.Value.ToString(), true);
                            break;
                        case "mining.disable":
                            RemoteMiningEnable(message.id.Value.ToString(), message.device.Value.ToString(), false);
                            break;
                        case "mining.set.power_mode":
                            RemoteMiningNotImplemented(message.id.Value.ToString());
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
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + ",0]}";
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
        public static async Task RemoteMiningStart(string id, string device)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",1,\"Remote management disabled\"]}";
                return;
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + ",0]}";
            if (Miner.IsRunningNew)
            {
                await _socket.SendData(cExecuted);
                Helpers.ConsolePrint("REMOTE", "Already mining");
                return;
            }
            remoteMiningStart = true;
            Thread.Sleep(3000);
            await _socket.SendData(cExecuted);
            Helpers.ConsolePrint("REMOTE", "Mining start. ID:" + id + " Device:" + device);
        }

        public static async Task RemoteMiningStop(string id, string device)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-1,\"Remote management disabled\"]}";
                return;
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + ",0]}";
            if (!Miner.IsRunningNew)
            {
                await _socket.SendData(cExecuted);
                Helpers.ConsolePrint("REMOTE", "Already stopped");
                return;
            }
            remoteMiningStop = true;
            Thread.Sleep(2000);
            await _socket.SendData(cExecuted);
            Helpers.ConsolePrint("REMOTE", "Mining stop. ID:" + id + " Device:" + device);
        }
        public static async Task RemoteSetWorker(string id, string worker)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-999,\"Remote management disabled\"]}";
                return;
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + ",0]}";
            await _socket.SendData(cExecuted);
            ConfigManager.GeneralConfig.WorkerName = worker;
            _socket.StartConnectionNew();
        }
        public static async Task RemoteSetUsername(string id)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-1,\"Remote management disabled\"]}";
                return;
            }
            var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + ",0]}";
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

        public static bool GetRigProfitInternalRUN()
        {
            try
            {
                if (ConfigManager.GeneralConfig.EnableAPIkeys)
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

        [HandleProcessCorruptedStateExceptions]
        public static bool GetSmaAPI(bool immediately = false)
        {
            bool ret = false;
            if (!immediately)
            {
                do
                {
                    Thread.Sleep(500);
                } while (Form_Main.Uptime.Seconds != 15 && Form_Main.Uptime.Seconds != 45);
            }

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
            try
            {
                if (!GetSmaAPI(true))
                {
                    if (System.IO.File.Exists("configs\\sma.dat"))
                    {
                        dynamic jsonData = (File.ReadAllText("configs\\sma.dat"));
                        Helpers.ConsolePrint("LoadSMA", "Using previous SMA");
                        JArray smadata = (JArray.Parse(jsonData));
                        SetAlgorithmRates(smadata);//LoadSMA
                    }
                    else
                    {
                        Helpers.ConsolePrint("LoadSMA", "Using default SMA");
                        dynamic defsma = "[[21,\"1.1637063156e-08\"],[50,\"1.5700000000e+04\"],[5,\"2.3910000000e-07\"],[54,\"9.9593453508e+02\"],[56,\"8.3154640439e-04\"],[23,\"8.8917404737e-08\"],[32,\"4.1550000000e-04\"],[43,\"4.1000000000e+03\"],[42,\"1.9636363636e+00\"],[8,\"6.6641064511e-03\"],[47,\"8.7152481058e-01\"],[36,\"3.1534919293e+02\"],[52,\"1.4753894679e-03\"],[14,\"7.0010000000e-07\"],[28,\"1.3914259087e-09\"],[46,\"3.5115871886e-04\"],[57,\"2.6596125572e-04\"],[33,\"1.2604450871e-04\"],[39,\"4.9647058824e+03\"],[24,\"9.3575041979e-01\"],[20,\"9.3350169094e-04\"],[51,\"5.2500182871e-08\"],[48,\"1.8300000000e-08\"],[58,\"9.2579601837e+02\"]]";
                        JArray smadata = (JArray.Parse(defsma));
                        SetAlgorithmRates(smadata);//LoadSMA
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("SOCKET", ex.Message);
                Helpers.ConsolePrint("SOCKET", "Using default SMA");
                dynamic defsma = "[[21,\"1.1637063156e-08\"],[50,\"1.5700000000e+04\"],[5,\"2.3910000000e-07\"],[54,\"9.9593453508e+02\"],[56,\"8.3154640439e-04\"],[23,\"8.8917404737e-08\"],[32,\"4.1550000000e-04\"],[43,\"4.1000000000e+03\"],[42,\"1.9636363636e+00\"],[8,\"6.6641064511e-03\"],[47,\"8.7152481058e-01\"],[36,\"3.1534919293e+02\"],[52,\"1.4753894679e-03\"],[14,\"7.0010000000e-07\"],[28,\"1.3914259087e-09\"],[46,\"3.5115871886e-04\"],[57,\"2.6596125572e-04\"],[33,\"1.2604450871e-04\"],[39,\"4.9647058824e+03\"],[24,\"9.3575041979e-01\"],[20,\"9.3350169094e-04\"],[51,\"5.2500182871e-08\"],[48,\"1.8300000000e-08\"],[58,\"9.2579601837e+02\"]]";
                JArray smadata = (JArray.Parse(defsma));
                SetAlgorithmRates(smadata);//LoadSMA
                Helpers.ConsolePrint("OLDSMA", ex.ToString());
            }
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
                payingDict.Add(AlgorithmType.DaggerHashimoto3GB, 0.0d);
                payingDict.Add(AlgorithmType.DaggerHashimoto4GB, 0.0d);
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

                        if (!NHSmaData.TryGetPaying(algoKey, out double paying))
                        {
                            Helpers.ConsolePrint("SetAlgorithmRates", "ERROR! Unknown algo: " + algoKey.ToString());
                        }

                        if (!ConfigManager.GeneralConfig.Use_Last24hours)
                        {
                            if (!algoKey.ToString().Contains("UNUSED") && type.ToLower().Contains("ws"))
                            {
                                //Helpers.ConsolePrint("SetAlgorithmRates", algoKey.ToString() + " updated. Type: " + type);
                                NHSmaData.UpdatePayingForAlgo(algoKey, Math.Abs(algo[1].Value<double>() * mult), average);//first init?
                            }

                            if ((Math.Abs(algo[1].Value<double>() * mult)) != 0 && !algoKey.ToString().Contains("UNUSED")
                                && type.ToLower().Equals("current"))
                            {
                                //Helpers.ConsolePrint("SetAlgorithmRates", algoKey.ToString() + " updated. Type: " + type);
                                NHSmaData.UpdatePayingForAlgo(algoKey, Math.Abs(algo[1].Value<double>() * mult), true);
                                if (algoKey == AlgorithmType.DaggerHashimoto || algoKey == AlgorithmType.ETCHash)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, Math.Abs(algo[1].Value<double>() * mult), false);
                                }
                            }

                            if ((Math.Abs(algo[1].Value<double>() * mult)) != 0 && !algoKey.ToString().Contains("UNUSED")
                                && type.ToLower().Equals("order"))
                            {
                                //Helpers.ConsolePrint("SetAlgorithmRates", algoKey.ToString() + " updated. Type: " + type);
                                NHSmaData.UpdatePayingForAlgo(algoKey, Math.Abs(algo[1].Value<double>() * mult), true);
                            }
                        }
                        else
                        {
                            if (!algoKey.ToString().Contains("UNUSED") && type.ToLower().Equals("ws"))
                            {
                                //Helpers.ConsolePrint("SetAlgorithmRates", algoKey.ToString() + " updated. Type: " + type);
                                NHSmaData.UpdatePayingForAlgo(algoKey, Math.Abs(algo[1].Value<double>() * mult), average);//first init?
                            }

                            if ((Math.Abs(algo[1].Value<double>() * mult)) != 0 && !algoKey.ToString().Contains("UNUSED") &&
                                type.ToLower().Equals("current"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, Math.Abs(algo[1].Value<double>() * mult), true);
                                if (algoKey == AlgorithmType.DaggerHashimoto || algoKey == AlgorithmType.ETCHash)
                                {
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.ZIL, Math.Abs(algo[1].Value<double>() * mult), false);
                                }
                            }

                            if ((Math.Abs(algo[1].Value<double>() * mult)) != 0 && !algoKey.ToString().Contains("UNUSED") &&
                                type.ToLower().Equals("order"))
                            {
                                NHSmaData.UpdatePayingForAlgo(algoKey, Math.Abs(algo[1].Value<double>() * mult), true);
                            }

                            if ((Math.Abs(algo[1].Value<double>() * mult)) != 0 && !algoKey.ToString().Contains("UNUSED") &&
                                type.ToLower().Equals("24h"))
                            {
                                if ((algoKey == AlgorithmType.DaggerHashimoto ||
                                    algoKey == AlgorithmType.ETCHash))
                                {
                                    NHSmaData.UpdatePayingForAlgo(algoKey, Math.Abs(algo[1].Value<double>() * mult), false);
                                } else
                                {
                                    NHSmaData.UpdatePayingForAlgo(algoKey, Math.Abs(algo[1].Value<double>() * mult), true);
                                }
                            }
                        }
                    }
                }

                //testing
                //payingDict[AlgorithmType.ZelHash] = 12345;

                //Helpers.ConsolePrint("*** 1", payingDict[AlgorithmType.DaggerHashimoto3GB].ToString());
                //Helpers.ConsolePrint("*** 2", payingDict[AlgorithmType.DaggerHashimoto4GB].ToString());
                //Helpers.ConsolePrint("*** 3", payingDict[AlgorithmType.DaggerHashimoto].ToString());
                /*
                payingDict[AlgorithmType.DaggerHashimoto3GB] = payingDict[AlgorithmType.DaggerHashimoto];
                payingDict[AlgorithmType.DaggerHashimoto4GB] = payingDict[AlgorithmType.DaggerHashimoto];

                NHSmaData.UpdateSmaPaying(payingDict, average);
                */

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


        public static void DeviceStatus_TickNew(object sender, ElapsedEventArgs e)
        {
            SetDeviceStatus(null);
        }

        #region Device
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        //Используется в OC Tune
        public class Root
        {
            public List<Device> devices { get; set; }
            public int id { get; set; }
            public object error { get; set; }
        }
        public class Device
        {
            public int device_id { get; set; }
            public string name { get; set; }
            public int gpgpu_type { get; set; }
            public string subvendor { get; set; }
            public Details details { get; set; }
            public string uuid { get; set; }
            public int gpu_temp { get; set; }
            public int gpu_load { get; set; }
            public int gpu_load_memctrl { get; set; }
            public int gpu_power_mode { get; set; }
            public double gpu_power_usage { get; set; }
            public double gpu_power_limit_current { get; set; }
            public double gpu_power_limit_min { get; set; }
            public double gpu_power_limit_max { get; set; }
            public double gpu_power_limit_default { get; set; }
            public double gpu_tdp_current { get; set; }
            public int gpu_clock_core_max { get; set; }
            public int gpu_clock_core { get; set; }
            public int gpu_clock_memory { get; set; }
            public int gpu_clock_memory_default { get; set; }
            public int gpu_fan_speed { get; set; }
            public int gpu_fan_speed_rpm { get; set; }
            public object gpu_memory_free { get; set; }
            public object gpu_memory_used { get; set; }
            public int intensity { get; set; }
            public int hw_errors { get; set; }
            public int hw_errors_success { get; set; }
            public KernelTimes kernel_times { get; set; }
            public OcData oc_data { get; set; }
            public List<Fan> fans { get; set; }
            public bool too_hot { get; set; }
            public int __vram_temp { get; set; }
            public int __hotspot_temp { get; set; }
            public Smartfan smartfan { get; set; }
            public OcLimits oc_limits { get; set; }
            public int gpu_mvolt_core { get; set; }
            public GpuMemoryTimings gpu_memory_timings { get; set; }
            public bool optimize_locked { get; set; }
        }
        public class Details
        {
            public int cuda_id { get; set; }
            public int sm_major { get; set; }
            public int sm_minor { get; set; }
            public int bus_id { get; set; }
            public bool sli { get; set; }
            public int bus_slot_id { get; set; }
            public string ram_maker { get; set; }
            public string pci_ident { get; set; }
            public bool is_enterprise { get; set; }
        }

        public class KernelTimes
        {
            public int avg { get; set; }
            public int min { get; set; }
            public int max { get; set; }
            public int umed { get; set; }
        }

        public class Mt
        {
        }

        public class OcData
        {
            public int core_clock_delta { get; set; }
            public int memory_clock_delta { get; set; }
            public int power_limit_watts { get; set; }
            public int power_limit_tdp { get; set; }
            public int core_clock_limit { get; set; }
            public List<object> core_uvolt { get; set; }
            public List<object> vfc { get; set; }
            public Mt mt { get; set; }
        }

        public class Fan
        {
            public int current_level { get; set; }
            public int current_rpm { get; set; }
            public int max_level { get; set; }
            public int min_level { get; set; }
            public bool is_auto { get; set; }
            public int max_rpm { get; set; }
        }

        public class Smartfan
        {
            public int mode { get; set; }
            public int fixed_speed { get; set; }
            public int target_gpu { get; set; }
            public int target_vram { get; set; }
            public int start_level { get; set; }
            public int override_level_min { get; set; }
            public int override_level_max { get; set; }
            public int decrease_k { get; set; }
            public int increase_k { get; set; }
            public int increase_n_gpu { get; set; }
            public int increase_n_vram { get; set; }
        }

        public class OcLimits
        {
            public int core_delta_min { get; set; }
            public int core_delta_max { get; set; }
            public int vram_delta_min { get; set; }
            public int vram_delta_max { get; set; }
            public int tdp_min { get; set; }
            public int tdp_max { get; set; }
        }

        public class Timings
        {
            public int RC { get; set; }
            public int RFC { get; set; }
            public int RAS { get; set; }
            public int RP { get; set; }
            public int CFG0_R0 { get; set; }
            public int CL { get; set; }
            public int WL { get; set; }
            public int RD_RCD { get; set; }
            public int WR_RCD { get; set; }
            public int CFG1_R0 { get; set; }
            public int RPRE { get; set; }
            public int WPRE { get; set; }
            public int CDLR { get; set; }
            public int WR { get; set; }
            public int W2R_BUS { get; set; }
            public int R2W_BUS { get; set; }
            public int PDEX { get; set; }
            public int PDEN2PDEX { get; set; }
            public int FAW { get; set; }
            public int AOND { get; set; }
            public int CCDL { get; set; }
            public int CCDS { get; set; }
            public int REFRESH_LO { get; set; }
            public int REFRESH { get; set; }
            public int RRD { get; set; }
            public int DELAY0 { get; set; }
            public int CFG4_R0 { get; set; }
            public int ADR_MIN { get; set; }
            public int CFG5_R0 { get; set; }
            public int WRCRC { get; set; }
            public int CFG5_R1 { get; set; }
            public int OFFSET0 { get; set; }
            public int DELAY0_MSB { get; set; }
            public int OFFSET1 { get; set; }
            public int OFFSET2 { get; set; }
            public int DELAY01 { get; set; }
        }

        public class GpuMemoryTimings
        {
            public bool bEditable { get; set; }
            public Timings timings { get; set; }
        }

        #endregion

        public static async void SetDeviceStatus(object state, bool devName = false)
        {
            Helpers.ConsolePrint("SOCKET", "DeviceStatusRunning: " + DeviceStatusRunning);
            if (DeviceStatusRunning) return;
            DeviceStatusRunning = true;
            var devicesOld = ComputeDeviceManager.Available.Devices;

            var _computeDevicesResort = ComputeDeviceManager.ReSortDevices(devicesOld);
            var _computeDevices = devicesOld;

            var rigStatus = CalcRigStatusString();
            var activeIDs = MinersManager.GetActiveMinersIndexes();
            string type;
            string b64Web;
            string nuuid = "";
            double HashRate = 0.0d;
            double SecondHashRate = 0.0d;
            double ThirdHashRate = 0.0d;

            if (state != null)
                rigStatus = state.ToString();

            var paramList = new List<JToken>
            {
                rigStatus
            };

            //Root devicesDataRootEx = new Root();
            //devicesDataRootEx.id = 1;
            //devicesDataRootEx.devices = new List<Device>();

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
                        int status = 0;
                        if (device.DeviceType == DeviceType.CPU)
                        {
                            type = "1";
                            status = 8;
                            b64Web = UUID.GetB64UUID(device.NewUuid);
                            nuuid = $"{type}-{b64Web}";
                        }
                        if (device.DeviceType == DeviceType.NVIDIA)
                        {
                            type = "2";
                            status = 16;
                            b64Web = UUID.GetB64UUID(device.Uuid);
                            nuuid = $"{type}-{b64Web}";
                        }
                        if (device.DeviceType == DeviceType.AMD)
                        {
                            type = "3";
                            status = 24;
                            b64Web = UUID.GetB64UUID(device.Uuid);
                            nuuid = $"{type}-{b64Web}";
                        }
                        if (device.DeviceType == DeviceType.INTEL)
                        {
                            type = "4";
                            status = 24;
                            b64Web = UUID.GetB64UUID(device.Uuid);
                            nuuid = $"{type}-{b64Web}";
                        }
                        device.DevUuid = nuuid;
                        var deviceName = device.Name;

                        string NvidiaLHR = "";
                        if (device.NvidiaLHR && device.DeviceType == DeviceType.NVIDIA)
                        {
                            NvidiaLHR = "(LHR)";
                        }

                        deviceName = deviceName + " " + NvidiaLHR;

                        string Manufacturer = "";
                        string GpuRam = "";

                        if (device.DeviceType == DeviceType.NVIDIA)
                        {
                            if (ConfigManager.GeneralConfig.Show_NVdevice_manufacturer)
                            {
                                deviceName = deviceName.Replace("NVIDIA", "");
                                if (!deviceName.Contains(ComputeDevice.GetManufacturer(device.Manufacturer)))
                                {
                                    Manufacturer = ComputeDevice.GetManufacturer(device.Manufacturer) + " ";
                                }
                            }
                            else
                            {
                                deviceName = deviceName.Replace(ComputeDevice.GetManufacturer(device.Manufacturer) + " ", "");
                                if (!deviceName.Contains("NVIDIA")) deviceName = "NVIDIA " + deviceName;
                            }
                        }

                        GpuRam = (device.GpuRam / 1073741824).ToString() + "GB";
                        if (ConfigManager.GeneralConfig.Show_ShowDeviceMemSize && device.DeviceType != DeviceType.CPU)
                        {
                            if (deviceName.Contains(GpuRam))
                            {
                                GpuRam = "";
                            }
                            else
                            {
                                deviceName = deviceName + " " + GpuRam;
                            }
                        }
                        else
                        {
                            deviceName = deviceName.Replace(GpuRam, "");
                            GpuRam = "";
                        }


                        if (device.DeviceType == DeviceType.AMD)
                        {
                            if (ConfigManager.GeneralConfig.Show_AMDdevice_manufacturer)
                            {
                                if (!deviceName.Contains(ComputeDevice.GetManufacturer(device.Manufacturer)))
                                {
                                    Manufacturer = ComputeDevice.GetManufacturer(device.Manufacturer) + " ";
                                }
                            }
                            else
                            {
                                deviceName = deviceName.Replace(ComputeDevice.GetManufacturer(device.Manufacturer) + " ", "");
                            }

                            GpuRam = (device.GpuRam / 1073741824).ToString() + "GB";
                            if (ConfigManager.GeneralConfig.Show_ShowDeviceMemSize && device.DeviceType != DeviceType.CPU)
                            {
                                if (deviceName.Contains(GpuRam))
                                {
                                    GpuRam = "";
                                }
                                else
                                {
                                    deviceName = deviceName + " " + GpuRam;
                                }
                            }
                            else
                            {
                                deviceName = deviceName.Replace(GpuRam, "");
                                GpuRam = "";
                            }
                        }

                        if (device.DeviceType == DeviceType.INTEL)
                        {
                            if (ConfigManager.GeneralConfig.Show_INTELdevice_manufacturer)
                            {
                                if (!deviceName.Contains(ComputeDevice.GetManufacturer(device.Manufacturer)))
                                {
                                    deviceName = deviceName.Replace("Intel ", "");
                                    Manufacturer = ComputeDevice.GetManufacturer(device.Manufacturer) + " ";
                                }
                            }
                            else
                            {
                                deviceName = deviceName.Replace(ComputeDevice.GetManufacturer(device.Manufacturer) + " ", "");
                            }

                            GpuRam = (device.GpuRam / 1073741824).ToString() + "GB";
                            if (ConfigManager.GeneralConfig.Show_ShowDeviceMemSize && device.DeviceType != DeviceType.CPU)
                            {
                                if (deviceName.Contains(GpuRam))
                                {
                                    GpuRam = "";
                                }
                                else
                                {
                                    deviceName = deviceName + " " + GpuRam;
                                }
                            }
                            else
                            {
                                deviceName = deviceName.Replace(GpuRam, "");
                                GpuRam = "";
                            }
                        }

                        if (device.MonitorConnected && ConfigManager.GeneralConfig.Show_displayConected)
                        {
                            Manufacturer = "> " + Manufacturer;
                        }

                        if (!devName)
                        {
                            deviceName = "";
                            Manufacturer = "";
                        }

                        //**********не работает
                        /*
                        var deviceEx = new Device();

                        var details = new Details();
                        var kernel_times = new KernelTimes();
                        var oc_data = new OcData();
                        var fans = new List<Fan>();
                        var smartfan = new Smartfan();
                        var oc_limits = new OcLimits();
                        var gpu_memory_timings = new GpuMemoryTimings();

                        deviceEx.device_id = dev;
                        deviceEx.name = deviceName;
                        deviceEx.gpgpu_type = 1;
                        deviceEx.subvendor = "10de";
                        deviceEx.__hotspot_temp = 54;
                        deviceEx.__vram_temp = 55;
                        deviceEx.uuid = "GPU-338e79dd-29a3-0744-0e26-3683d42fcc70";
                        deviceEx.gpu_fan_speed = 56;
                        deviceEx.gpu_fan_speed_rpm = 1256;
                        deviceEx.gpu_load = 57;
                        deviceEx.gpu_load_memctrl = 58;
                        deviceEx.gpu_power_usage = 59;
                        deviceEx.gpu_temp = 60;

                        deviceEx.details = details;
                        deviceEx.kernel_times = kernel_times;
                        deviceEx.oc_data = oc_data;
                        deviceEx.fans = fans;
                        deviceEx.smartfan = smartfan;
                        deviceEx.oc_limits = oc_limits;
                        deviceEx.gpu_memory_timings = gpu_memory_timings;
                        devicesDataRootEx.devices.Add(deviceEx);
                        */
                        //***********

                        //В оригинальном NH при второй отправке данных вместо названия
                        //устройства (Manufacturer + deviceName) = null
                        //Вместо nuuid используется порядковый номер устройства (string). Без проверки на уникальность!!!
                        //{"method":"miner.status","params":["STOPPED",[["","0",
                        //Оставим как правильно
                        //{"method":"miner.status","params":["STOPPED",[["Intel(R) Core(TM) i7-3630QM CPU @ 2.40GHz","1-YBxRn6UfL1O7dUk6NNR5EA",
                    var array = new JArray
                    {
                        Manufacturer + deviceName,
                        //dev.ToString()
                        nuuid
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
                        } else
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
                        if (rigs == 1 & (device.AlgorithmID == -9) || device.AlgorithmID == -12) //dagger 3-4
                        {
                            speedsJson.Add(new JArray(20, HashRate)); //  номер алгоритма, хешрейт
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
                            /*
                            if (ConfigManager.GeneralConfig.QM_mode)
                            {
                                array.Add(Math.Round(TempMemory * 65536 + Math.Round(device.Temp)));
                            } else
                            */
                            {
                                array.Add(Math.Round(Math.Round(device.Temp)));
                            }
                            array.Add(device.FanSpeedRPM);
                            array.Add((int)Math.Round(device.PowerUsage));
                        }
                        else
                        {
                            /*
                            if (ConfigManager.GeneralConfig.QM_mode)
                            {
                                array.Add(Math.Round(TempMemoryResort * 65536 + Math.Round(deviceResort.Temp)));
                            } else
                            */
                            {
                                array.Add(Math.Round(Math.Round(deviceResort.Temp)));
                            }
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
                //var sendDataEx = JsonConvert.SerializeObject(devicesDataRootEx);
                if (_socket != null)
                {
                    await _socket.SendData(sendData);
                   // await _socket.SendData(sendData2);
                    //await _socket.SendData(sendDataEx);
                    //Helpers.ConsolePrint("SetDeviceStatus", "sendDataEx -> " + sendDataEx);
                }
            }
            catch (Exception ex2)
            {
                Helpers.ConsolePrint("SetDeviceStatus", ex2.ToString());
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
                //Form_API_keys.GetSavedAPIkeyData();
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
                wr.Timeout = 1 * 1000;
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
                Form_Main.apiConnectionsErrors++;
                return null;
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetNiceHashApiDataWithSecret", ex.ToString());
                Form_Main.errorAPIkeystring = ex.Message;
                Form_Main.apiConnectionsErrors++;
                return null;
            }
            Form_Main.apiConnectionsErrors = 0;
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

