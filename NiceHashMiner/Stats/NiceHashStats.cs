using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners;
using NiceHashMiner.Switching;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using NiceHashMinerLegacy.Common.Enums;
using WebSocketSharp;
using System.Timers;
using System.Threading.Tasks;

using NiceHashMiner.Configs;
using static NiceHashMiner.Devices.ComputeDeviceManager;
using NiceHashMinerLegacy.UUID;
using NiceHashMiner.Miners.Grouping;
using System.Management;
using System.Text;
using System.Runtime.ExceptionServices;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Net.Sockets;
using NiceHashMiner.Algorithms;
using System.Diagnostics;

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

    //internal static class NiceHashStats
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

        private static void LoadCachedSMAData()
        {
            if (File.Exists("configs\\sma.dat"))
            {
                try
                {
                    dynamic jsonData = (File.ReadAllText("configs\\sma.dat"));
                    Helpers.ConsolePrint("SOCKET", "Using previous SMA");
                    JArray smadata = (JArray.Parse(jsonData));
                    SetAlgorithmRates(smadata);
                }
                catch (Exception er)
                {
                    Helpers.ConsolePrint("SMA.DAT", er.ToString());
                }
            }
        }

        public static void StartConnection(string address)
        {
            NHSmaData.InitializeIfNeeded();
            LoadCachedSMAData();
            
            try
            {
                _socket = null;
                _socket = new NiceHashSocket(address);

                //_socket.OnConnectionEstablished += SocketOnOnConnectionEstablished;
                _socket.OnDataReceived += SocketOnOnDataReceived;
                //   _socket.OnConnectionLost += SocketOnOnConnectionLost;

                Helpers.ConsolePrint("SOCKET-address:", address);
                _socket.StartConnectionNew();
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("StartConnection", er.ToString());
            }
            
    _deviceUpdateTimer = new System.Timers.Timer(DeviceUpdateInterval);
            _deviceUpdateTimer.Elapsed += DeviceStatus_TickNew;
            _deviceUpdateTimer.Start();

        }

        #region Socket Callbacks
        private static void SocketOnOnDataReceived(object sender, MessageEventArgs e)
        {

            try
            {
                if (e.IsText)
                {
                    Helpers.ConsolePrint("SOCKET", "Received: " + e.Data);
                    dynamic message = JsonConvert.DeserializeObject(e.Data);
                   // Helpers.ConsolePrint("SOCKET", "Received1: " + e.Data);
                    switch (message.method.Value)
                    {
                        case "sma":
                            {
                                if (Form_Main.SMAdelayTick < 30) break;
                                Form_Main.SMAdelayTick = 0;
                                //***************************
                                FileStream fs = new FileStream("configs\\sma.dat", FileMode.Create, FileAccess.Write);
                                StreamWriter w = new StreamWriter(fs);
                                w.Write(message.data);
                                //w.Write(JsonConvert.SerializeObject(message));
                                w.Flush();
                                w.Close();
                                foreach (var algo in message.data)
                                {
                                    var algoKey = (AlgorithmType)algo[0];
                                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                                    {
                                        Helpers.ConsolePrint("SMA-DATA-WS: ", Enum.GetName(typeof(AlgorithmType), algoKey) + " ("+ algo[0].ToString() + ") - " + algo[1]);
                                    }
                                }
                                if (ConfigManager.GeneralConfig.MOPA5)
                                {
                                    ClearAlgorithmRates();
                                }

                                SetAlgorithmRates(message.data);
                                GetSmaAPI();
                                
                                if (AlgorithmSwitchingManager._smaCheckTimer != null)
                                {
                                    AlgorithmSwitchingManager._smaCheckTimer.Stop();
                                    AlgorithmSwitchingManager._smaCheckTimer.Dispose();
                                    AlgorithmSwitchingManager._smaCheckTimer = null;
                                    AlgorithmSwitchingManager.Start();//************************
                                }

                                if (Miner.IsRunningNew)
                                {
                                    Form_Main.smaCount++;
                                } else
                                {
                                    Form_Main.smaCount = 0;
                                }
                                //Helpers.ConsolePrint("Form_Main.smaCount", Form_Main.smaCount.ToString());
                                if (Form_Main.smaCount > 3)
                                {
                                    dynamic jsonData = (File.ReadAllText("configs\\sma.dat"));
                                    Helpers.ConsolePrint("SOCKET", "Using previous SMA");
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

                        case "balance":
                            SetBalance(message.value.Value);
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
                            RemoteMiningNotImplemented(message.id.Value.ToString());
                            break;
                        case "mining.set.worker":
                            RemoteMiningNotImplemented(message.id.Value.ToString());
                            //RemoteWorkerRename(message.id.Value.ToString(), message.worker);
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

        public class Rootobject
        {
            public Result result { get; set; }
            public string method { get; set; }
        }

        public class Result
        {
            public Simplemultialgo[] simplemultialgo { get; set; }
        }
        public class Simplemultialgo
        {
            public string paying { get; set; }
            public int port { get; set; }
            public string name { get; set; }
            public int algo { get; set; }
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
            public Algos24h[] algos { get; set; }
        }
        public class Algos24h
        {
            public int a { get; set; }
            public string p { get; set; }
            public float s { get; set; }
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
                //await _socket.SendData(cExecutedDisabled);
                return;
            }
            Helpers.ConsolePrint("REMOTE", "id: "+id+" device: "+ deviceToSwitch);

            //var cExecutedNotImplemented = "{\"method\":\"executed\",\"params\":[" + id + ",1,\"Not implemented in Fork Fix " + ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",", ".") + "\"]}";
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
            var cExecutedNotImplemented = "{\"method\":\"executed\",\"params\":[" + id + ",1,\"Not implemented in Fork Fix " + ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",",".") + "\"]}";
            //await _socket.SendData(cExecutedNotImplemented);
            return;
        }
            public static async Task RemoteMiningStart(string id, string device)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",1,\"Remote management disabled\"]}";
                //await _socket.SendData(cExecutedDisabled);
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
           //Thread.Sleep(1000);
           //await _socket.SendData(cExecuted);
        }
        public static async Task RemoteWorkerRename(string id, string worker)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                return;
            }
            Configs.ConfigManager.GeneralConfig.WorkerName = worker;
           // var cExecuted = "{\"method\":\"executed\",\"params\":[" + id + ",0]}";
            //await _socket.SendData(cExecuted);
            //Helpers.ConsolePrint("REMOTE", "Worker renamed");
            return;
        }
        public static async Task RemoteMiningStop(string id, string device)
        {
            if (!ConfigManager.GeneralConfig.Allow_remote_management)
            {
                Helpers.ConsolePrint("REMOTE", "Remote management disabled");
                var cExecutedDisabled = "{\"method\":\"executed\",\"params\":[" + id + ",-1,\"Remote management disabled\"]}";
                //await _socket.SendData(cExecutedDisabled);
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
            //Thread.Sleep(1000);
            //await _socket.SendData(cExecuted);
        }

        

        public static bool GetSmaAPICurrent()
        {
            Helpers.ConsolePrint("NHM_API_info", "Trying GetSmaAPICurrent");

            try
            {
                string resp;
                    resp = NiceHashStats.GetNiceHashApiData("https://api2.nicehash.com/main/api/v2/public/simplemultialgo/info", "x");
                if (resp != null)
                {
                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                    {
                       // Helpers.ConsolePrint("NHM_API_info", resp);
                    }

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
                                        Helpers.ConsolePrint("SMA-DATA-APICurrent: ", miningAlgorithms.title + " - " + Algo + " - " + miningAlgorithms.paying);
                                    }

                                    //miningAlgorithms.paying = "0.00016";
                                    outProf = outProf + "  [\n" + "    " + Algo + ",\n" + "    " + miningAlgorithms.paying + "\n" + "  ],\n";
                                    break;
                                }
                            }
                        }
                    }
                    outProf = outProf.Remove(outProf.Length - 2) + "]";

                   // Helpers.ConsolePrint("SMA-DATA-APICurrent: ", outProf);
                    JArray smadata = (JArray.Parse(outProf));

                    NiceHashStats.SetAlgorithmRates(smadata, 10, 15);
                    /*
                    FileStream fs = new FileStream("configs\\sma.dat", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.Write(smadata);
                    //w.Write(JsonConvert.SerializeObject(message));
                    w.Flush();
                    w.Close();
                    */
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
        public static bool GetSmaAPI5m()
        {
            Helpers.ConsolePrint("NHM_API_info", "Trying GetSmaAPI5m");

            try
            {
                string resp;
                resp = NiceHashStats.GetNiceHashApiData("https://api2.nicehash.com/main/api/v2/public/stats/global/current", "x");
                if (resp != null)
                {
                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                    {
                      //  Helpers.ConsolePrint("NHM_API_info", resp);
                    }

                    dynamic list;
                    list = JsonConvert.DeserializeObject<Rootobject5m>(resp);

                    ProfitsSMA profdata = new ProfitsSMA();

                    List<ProfitsSMA> profdata2 = new List<ProfitsSMA>();

                    string outProf = "[\n";

                    var _currentSma = new Dictionary<AlgorithmType, NiceHashSma>();
                    foreach (var algos in list.algos)
                    {
                                {
                                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                                    {
                                        Helpers.ConsolePrint("SMA-DATA-API5m: ", algos.a + " - " +  algos.p);
                                    }
                                    outProf = outProf + "  [\n" + "    " + algos.a + ",\n" + "    \"" + algos.p + "\"\n" + "  ],\n";
                                }
                    }
                    outProf = outProf.Remove(outProf.Length - 2) + "]";

                   //  Helpers.ConsolePrint("SMA-DATA-APICurrent: ", outProf);
                    JArray smadata = (JArray.Parse(outProf));

                    NiceHashStats.SetAlgorithmRates(smadata, 10, 15);
                    /*
                    FileStream fs = new FileStream("configs\\sma.dat", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.Write(smadata);
                    //w.Write(JsonConvert.SerializeObject(message));
                    w.Flush();
                    w.Close();
                    */
                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                    {
                        Helpers.ConsolePrint("NHM_API_info", "GetSmaAPI5m OK");
                    }
                    return true;
                }
                Helpers.ConsolePrint("NHM_API_info", "GetSmaAPI5m ERROR");
                return false;

            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("NHM_API_info", ex.Message);
                Helpers.ConsolePrint("NHM_API_info", "GetSmaAPI5m fatal ERROR");
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
                resp = NiceHashStats.GetNiceHashApiData("https://api2.nicehash.com/main/api/v2/public/stats/global/24h", "x");
                if (resp != null)
                {
                    if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                    {
                      //  Helpers.ConsolePrint("NHM_API_info", resp);
                    }

                    dynamic list;
                    list = JsonConvert.DeserializeObject<Rootobject24h>(resp);

                    ProfitsSMA profdata = new ProfitsSMA();

                    List<ProfitsSMA> profdata2 = new List<ProfitsSMA>();

                    string outProf = "[\n";

                    var _currentSma = new Dictionary<AlgorithmType, NiceHashSma>();
                    foreach (var algos in list.algos)
                    {
                        {
                            if (!ConfigManager.GeneralConfig.NoShowApiInLog)
                            {
                                Helpers.ConsolePrint("SMA-DATA-API24h: ", algos.a + " - " + algos.p);
                            }
                            outProf = outProf + "  [\n" + "    " + algos.a + ",\n" + "    \"" + algos.p + "\"\n" + "  ],\n";
                        }
                    }
                    outProf = outProf.Remove(outProf.Length - 2) + "]";

                 //   Helpers.ConsolePrint("SMA-DATA-API24h: ", outProf);
                    JArray smadata = (JArray.Parse(outProf));

                    NiceHashStats.SetAlgorithmRates(smadata, 10, 5);

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
        public static bool GetRigProfit()//big traffic
        {
            try
            {
                if (ConfigManager.GeneralConfig.ChartEnable)
                {
                    string apistr = "https://api2.nicehash.com/main/api/v2/mining/external/" + Globals.GetBitcoinUser() + "/rigs2?sort=NAME&page=0";
                    string resp;
                    Helpers.ConsolePrint("NHM_API_info", "Trying GetRigProfit");
                    resp = NiceHashStats.GetNiceHashApiData(apistr, "");
                    //resp = NiceHashStats.GetNiceHashApiData("https://api2.nicehash.com/main/api/v2/mining/external/" +  "/rigs/activeWorkers?sortDirection=ASC", "x");
                    //Helpers.ConsolePrint("NHM_API_info", apistr);
                    if (resp != null)
                    {
                        //Helpers.ConsolePrint("NHM_API_info", resp);
                        dynamic respJson = JsonConvert.DeserializeObject(resp);
                        var Rigs = respJson.miningRigs;

                        foreach (var rig in Rigs)
                        {
                            if (rig.rigId.ToString() == NiceHashSocket.RigID)
                            {
                                /*
                                Helpers.ConsolePrint("NHM_API_info", rig.rigId.ToString());
                                Helpers.ConsolePrint("NHM_API_info", rig.name.ToString());
                                Helpers.ConsolePrint("NHM_API_info", rig.profitability.ToString());
                                Helpers.ConsolePrint("NHM_API_info", rig.unpaidAmount.ToString());
                                */
                                if (rig.profitability > Form_Main.lastRigProfit.currentProfitAPI * 10 &&
                                    Form_Main.lastRigProfit.currentProfitAPI != 0 && rig.profitability != 0)
                                {
                                    Helpers.ConsolePrint("GetRigProfit too high. Ignoring", (rig.profitability * 1000).ToString());
                                    //Helpers.ConsolePrint("Form_Main.lastRigProfit.currentProfitAPI", (Form_Main.lastRigProfit.currentProfitAPI * 1000).ToString());
                                }
                                else if (rig.profitability * 10 < Form_Main.lastRigProfit.currentProfitAPI &&
                                    Form_Main.lastRigProfit.currentProfitAPI != 0 && rig.profitability != 0)
                                {
                                    Helpers.ConsolePrint("GetRigProfit too low. Ignoring", (rig.profitability * 1000).ToString());
                                    //Helpers.ConsolePrint("Form_Main.lastRigProfit.currentProfitAPI", (Form_Main.lastRigProfit.currentProfitAPI * 1000).ToString());
                                }
                                else
                                {
                                    Form_Main.lastRigProfit.currentProfitAPI = rig.profitability;
                                    Helpers.ConsolePrint("GetRigProfit", (rig.profitability * 1000).ToString());
                                }
//                                Form_Main.lastRigProfit.unpaidAmount = rig.unpaidAmount;
                            }
                        }
                        double unpaidAmount = respJson.unpaidAmount;
                        Helpers.ConsolePrint("unpaidAmount", (unpaidAmount * 1000).ToString());
                        SetBalance(unpaidAmount.ToString());
                    } else
                    {
                        Form_Main.lastRigProfit.currentProfitAPI = 0;
                        Form_Main.lastRigProfit.unpaidAmount = 0;
                    }
                } else
                {
                    Form_Main.lastRigProfit.currentProfitAPI = 0;
                    Form_Main.lastRigProfit.unpaidAmount = 0;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("NHM_API_info", ex.Message);
                Helpers.ConsolePrint("NHM_API_info", "GetRigProfit fatal ERROR");
                return false;
            }
            return false;

        }

        [HandleProcessCorruptedStateExceptions]
        public static bool GetSmaAPI()
        {

            try
            {
                if (ConfigManager.GeneralConfig.MOPA2)
                {
                    GetSmaAPICurrent(); //bug *10
                }
                if (ConfigManager.GeneralConfig.MOPA3)
                {
                    GetSmaAPI5m(); //bug *10
                }
                if (ConfigManager.GeneralConfig.MOPA4)
                {
                    GetSmaAPI24h(); //bug *10
                }
                if (ConfigManager.GeneralConfig.MOPA5)
                {
                    //GetSmaAPI24h(); //bug *10
                    GetSmaAPI5m(); //bug *10
                    GetSmaAPICurrent(); //bug *10
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("SOCKET", ex.Message);

            }
            return true;
        }

        public static void LoadSMA()
        {
            if (!ConfigManager.GeneralConfig.NoShowApiInLog)
            {
                Helpers.ConsolePrint("SMA", "Trying LoadSMA");
            }
            try
            {
                //******
                if (!GetSmaAPI())
                {
                    if (System.IO.File.Exists("configs\\sma.dat"))
                    {
                        /*
                        if (AlgorithmRates == null || niceHashData == null)
                        {
                            niceHashData = new NiceHashData();
                            AlgorithmRates = niceHashData.NormalizedSMA();
                        }
                        */

                        dynamic jsonData = (File.ReadAllText("configs\\sma.dat"));
                        Helpers.ConsolePrint("SOCKET", "Using previous SMA");
                        JArray smadata = (JArray.Parse(jsonData));
                        SetAlgorithmRates(smadata);
                    }
                    else
                    {
                        Helpers.ConsolePrint("SOCKET", "Using default SMA");
                        /*
                        if (AlgorithmRates == null || niceHashData == null)
                        {
                            niceHashData = new NiceHashData();
                            AlgorithmRates = niceHashData.NormalizedSMA();
                        }
                        */
                        dynamic defsma = "[[5,\"5.999963193e-07\"],[36,\"659.5557431\"],[42,\"33.73843367\"],[8,\"0.008899999201\"],[38,\"721.326346\"],[32,\"0.0002005014359\"],[24,\"5.666958028\"],[33,\"0.001290840303\"],[30,\"1.14051987\"],[37,\"100.0002035\"],[7,\"4.999946511e-06\"],[45,\"1324.211344\"],[22,\"0.3053852139\"],[34,\"2.787742099\"],[39,\"47716.02121\"],[44,\"7123.200162\"],[40,\"0.0006006972195\"],[20,\"0.001430123323\"],[23,\"6.561117697e-07\"],[43,\"5731.578685\"],[21,\"9.782149555e-11\"],[14,\"4.644947288e-05\"],[29,\"0.0001507043319\"],[28,\"9.999998266e-08\"],[31,\"0\"]]";
                        JArray smadata = (JArray.Parse(defsma));
                        SetAlgorithmRates(smadata);
                    }
                }
                //******
                if (System.IO.File.Exists("configs\\balance.dat"))
                {
                    FileStream fs3 = new FileStream("configs\\balance.dat", FileMode.Open, FileAccess.Read);
                    StreamReader w3 = new StreamReader(fs3);
                    String fakeSMA3 = w3.ReadToEnd();
                    dynamic message3 = JsonConvert.DeserializeObject(fakeSMA3);
                    //Helpers.ConsolePrint("SOCKET-oldSMA", "Received: " + fakeSMA3);
                    Helpers.ConsolePrint("SOCKET", "Using previous balance");
                    w3.Close();
                    if (message3.method == "balance")
                    {
                        SetBalance(message3.value.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("SOCKET", ex.Message);
                Helpers.ConsolePrint("SOCKET", "Using default SMA");
                /*
                if (AlgorithmRates == null || niceHashData == null)
                {
                    niceHashData = new NiceHashData();
                    AlgorithmRates = niceHashData.NormalizedSMA();
                }
                */
                dynamic defsma = "[[5,\"0.00031031\"],[7,\"0.00401\"],[8,\"0.26617936\"],[14,\"0.00677556\"],[20,\"0.00833567\"],[21,\"0.00005065\"],[22,\"352.1073569\"],[23,\"0.00064179\"],[24,\"620.89332464\"],[25,\"0.00009207\"],[26,\"0.01044116\"],[27,\"0.00005085\"],[28,\"0.00003251\"],[29,\"0.00778864\"]]";
                JArray smadata = (JArray.Parse(defsma));
                SetAlgorithmRates(smadata);
                Helpers.ConsolePrint("OLDSMA", ex.ToString());
            }
        }



        private static void SocketOnOnConnectionEstablished(object sender, EventArgs e)
        {
            //DeviceStatus_Tick(null); // Send device to populate rig stats
            //LoadSMA(); //for first run
            //string ghv = GetVersion("");
            //Helpers.ConsolePrint("GITHUB", ghv);
            //SetVersion(ghv);
           // GetSmaAPICurrent();
           // OnConnectionEstablished?.Invoke(null, EventArgs.Empty);
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
        public static void SetAlgorithmRates(JArray data, int mult = 1, double treshold = 12.0)
        {
            bool bug = false;
            bool alg_zero = false;
            try
            {
                var payingDict = new Dictionary<AlgorithmType, double>();
                if (data != null)
                {
                    foreach (var algo in data)
                    {
                        if (algo == null) return;
                        bug = false;
                        alg_zero = false;
                        var algoKey = (AlgorithmType)algo[0].Value<int>();
                        if (!NHSmaData.TryGetPaying(algoKey, out double paying))
                        {
                            Helpers.ConsolePrint("SMA API", "ERROR! Unknown algo: "+ algoKey.ToString());
                            //payingDict[algoKey] = algo[1].Value<double>();
                        }

                        if (paying == 0)
                        {
                            payingDict[algoKey] = Math.Abs(algo[1].Value<double>());
                        }

                        //if (Math.Abs(algo[1].Value<double>()) != 0 && paying != 0 && !algoKey.ToString().Contains("UNUSED"))
                        if (paying != 0 && !algoKey.ToString().Contains("UNUSED"))
                        {
                            if (paying * treshold < Math.Abs(algo[1].Value<double>()) * mult ||
                               paying / treshold > Math.Abs(algo[1].Value<double>()) * mult)
                            {
                                //Helpers.ConsolePrint("SMA API", algoKey.ToString() + " - " +
                                //(paying * treshold).ToString() + " : " + (Math.Abs(algo[1].Value<double>()) * mult).ToString());
                                Helpers.ConsolePrint("SMA API", "Bug found in " + algoKey.ToString() +
                                    ". Old: " + paying.ToString() +
                                    " New: " + Math.Abs(algo[1].Value<double>()) * mult +
                                    " Change in profitability more than " + (treshold * 100).ToString() + "%. Ignoring");
                            }
                            else
                            {
                                if (ConfigManager.GeneralConfig.MOPA5 && paying < Math.Abs(algo[1].Value<double>() * mult))
                                {
                                    //Helpers.ConsolePrint("SMA API", algoKey.ToString() + " BIGGER - " +
                                    //(paying).ToString() + " : " + (Math.Abs(algo[1].Value<double>()) * mult).ToString());
                                    payingDict[algoKey] = Math.Abs(algo[1].Value<double>() * mult);
                                }
                                if (!ConfigManager.GeneralConfig.MOPA5)
                                {
                                    payingDict[algoKey] = Math.Abs(algo[1].Value<double>() * mult);
                                    //Helpers.ConsolePrint("SMA API", algoKey.ToString() + " - " +
                                    //(paying).ToString() + " : " + (Math.Abs(algo[1].Value<double>()) * mult).ToString());
                                }
                            }
                        }
                        /*
                        if (Math.Abs(algo[1].Value<double>()) == 0)
                        {
                            Helpers.ConsolePrint("SMA API", "Bug found in " + algoKey.ToString() +
                                                                ". Old: " + paying.ToString() +
                                                                " New: " + Math.Abs(algo[1].Value<double>()) * mult +
                                                                " Zero ignoring");
                        }
                        */
                        /*
                        if (ConfigManager.GeneralConfig.DaggerOrderMaxPay > 0 && algoKey == AlgorithmType.DaggerHashimoto && Math.Abs(algo[1].Value<double>()) > ConfigManager.GeneralConfig.DaggerOrderMaxPay)
                        {
                            Helpers.ConsolePrint("SMA", "Sets DaggerHashimoto to 0");
                            payingDict[algoKey] = 0;
                        }
                        */
                    }
                }

                NHSmaData.UpdateSmaPaying(payingDict);
                
                Thread.Sleep(10);
                OnSmaUpdate?.Invoke(null, EventArgs.Empty);
                
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
            }
        }

        private static void SetStableAlgorithms(JArray stable)
        {
            /*
            var stables = stable.Select(algo => (AlgorithmType) algo.Value<int>());
            NHSmaData.UpdateStableAlgorithms(stables);
            */
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
           // Helpers.ConsolePrint("SOCKET", "Received5: " + balance);
            try
            {
                if (double.TryParse(balance, NumberStyles.Float, CultureInfo.InvariantCulture, out var bal))
                {
                  //  Helpers.ConsolePrint("SOCKET", "Received6: " + balance);
                    Balance = bal;
                    //OnBalanceUpdate?.Invoke(null, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
            }
         //  Helpers.ConsolePrint("SOCKET", "Received7: " + balance);
        }

        private static void SetExchangeRates(string data)
        {
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
                    //OnExchangeUpdate?.Invoke(null, EventArgs.Empty);
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

                // Send as task since SetCredentials is called from UI threads
                //Task.Factory.StartNew(() => _socket?.SendData(sendData));
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
            //_socket.ConnectCallback(null, null);
           // _socket.SendData(sendData);
            SetDeviceStatus(null);
        }
        public static async void SetDeviceStatus(object state, bool devName = false)
        {
            var devices = ComputeDeviceManager.Available.Devices;
            
            var _computeDevicesResort = ComputeDeviceManager.ReSortDevices(devices);
            var _computeDevices = devices;

            var rigStatus = CalcRigStatusString();
            var activeIDs = MinersManager.GetActiveMinersIndexes();
            string type;
            string b64Web;
            string nuuid = "";
            double HashRate = 0.0d;

            if (state != null)
                rigStatus = state.ToString();
            {
            }
            var paramList = new List<JToken>
            {
                rigStatus
            };

            var deviceList = new JArray();
            //foreach (var device in _computeDevices)
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
                    var deviceName = device.Name;
                    
                    string Manufacturer = "";
                    string GpuRam = "";

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

                    GpuRam = (device.GpuRam / 1073741824).ToString() + "GB";
                    if (ConfigManager.GeneralConfig.Show_ShowDeviceMemSize)
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
                        if (ConfigManager.GeneralConfig.Show_ShowDeviceMemSize)
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

                    if (!devName)
                    {
                        deviceName = "";
                        Manufacturer = "";
                    }

                    /*
                    if (rigStatus != "PENDING")
                    {
                        deviceName = "";
                    }
                    */
                    var array = new JArray
                    {
                        Manufacturer + deviceName,
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
                        //status = status + Convert.ToInt32(Miner.IsRunningNew) + Convert.ToInt32(device.Enabled);

                    }
                    //var status = 9;
                    array.Add(status);

                    array.Add((int)Math.Round(device.Load));


                    var speedsJson = new JArray();

                    HashRate = device.MiningHashrate;

                    if (rigs == 1 & device.AlgorithmID > 0)
                    {
                        speedsJson.Add(new JArray(device.AlgorithmID, HashRate)); //  номер алгоритма, хешрейт
                    }
                    if (rigs == 1 & (device.AlgorithmID == -9) || device.AlgorithmID == -12) //dagger 3-4
                    {
                        speedsJson.Add(new JArray(20, HashRate)); //  номер алгоритма, хешрейт
                    }

                    array.Add(speedsJson);
                    // Hardware monitoring
                    if (!Form_Main.NVIDIA_orderBug)
                    {
                        array.Add((int)Math.Round(device.Temp));
                        array.Add(device.FanSpeedRPM);
                        array.Add((int)Math.Round(device.PowerUsage));
                    } else
                    {
                        array.Add((int)Math.Round(deviceResort.Temp));
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
                    } else
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

                    deviceList.Add(array);
                }
                catch (Exception ex) { Helpers.ConsolePrint("SOCKET", ex.ToString()); }
            }

            try
            {
                paramList.Add(deviceList);

                var data = new MinerStatusMessage
                {
                    param = paramList
                };
                var sendData = JsonConvert.SerializeObject(data);

                if (_socket != null)
                {
                    await _socket.SendData(sendData);
                    //Helpers.ConsolePrint("SetDeviceStatus", "sendData -> " + sendData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"DeviceStatus_TickNew: {ex.ToString()}");
            }
        }
       

        #endregion

        public static string GetNiceHashApiData(string url, string worker)
        {
            var responseFromServer = "";
            try
            {
                var activeMinersGroup = MinersManager.GetActiveMinersGroup();

                var wr = (HttpWebRequest) WebRequest.Create(url);
                wr.UserAgent = "NiceHashMiner/" + Application.ProductVersion;
                if (worker.Length > 64) worker = worker.Substring(0, 64);
                wr.Headers.Add("NiceHash-Worker-ID", worker);
                wr.Headers.Add("NHM-Active-Miners-Group", activeMinersGroup);
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
            catch (Exception ex)
            {
                Helpers.ConsolePrint("NICEHASH", ex.Message);
                return null;
            }
            return responseFromServer;
        }
        public static string GetNiceHashApiDataNew(string url, string worker)
        {
            var responseFromServer = "";
            try
            {
                var activeMinersGroup = MinersManager.GetActiveMinersGroup();

                var wr = (HttpWebRequest)WebRequest.Create(url);
                wr.UserAgent = "NiceHashMiner/" + Application.ProductVersion;
                //if (worker.Length > 64) worker = worker.Substring(0, 64);
                //wr.Headers.Add("NiceHash-Worker-ID", worker);
                //wr.Headers.Add("NHM-Active-Miners-Group", activeMinersGroup);
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
            catch (Exception ex)
            {
                Helpers.ConsolePrint("NICEHASH", ex.Message);
                return null;
            }
            return responseFromServer;
        }

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static string CalcRigStatusString()
        {
            /*
            var rigState = CalcRigStatus();
            switch (rigState)
            {
                case RigStatus.Offline: return "OFFLINE";
                case RigStatus.Stopped: return "STOPPED";
                case RigStatus.Mining: return "MINING";
                case RigStatus.Benchmarking: return "BENCHMARKING";
                case RigStatus.Error: return "ERROR";
                case RigStatus.Pending: return "PENDING";
                case RigStatus.Disabled: return "DISABLED";
            }
            */
            //return "UNKNOWN";
            //var m = GroupMiner.Miner;

            // skip if not running or if await already in progress

            if (Miner.IsRunningNew)
            {
                return "MINING";
            } else
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

        public TimerOwner()
        {
            timerInit(dueTime);
        }

        byte[] dummy = new byte[100000];

        /// <summary>
        ///
        /// </summary>
        /// <param name="dueTime">Pass dueTime for the first time, then TimerPeriod will be passed automatically</param>
        private void timerInit(int dueTime)
        {

            timer = new System.Threading.Timer(timerCallback,
                timerCanceller,     //this is the trick, it will be kept in the heap until it is consumed by the callback
                dueTime,
                Timeout.Infinite
            );

        }

        private void timerCallback(object state)
        {
            try
            {
                //First exit if the timer was stoped before calling callback. This info is saved in state
                var canceller = (TimerCanceller)state;
                /*
                if (canceller.Cancelled)
                {
                    return; //
                }
                */
                //Your logic goes here. Please take care ! the callback might have already been called before stoping the timer
                //and we might be already here after intending of stoping the timer. In most cases it is fine but try not to consume
                //an object of this class because it might be already disposed. If you have to do that, hopefully it will be catched by
                //the ObjectDisposedException below




                dummy[1] = 50;  //just messing up with the object after it might be disposed/nulled

                //Yes, we need to check again. Read above note
                if (canceller.Cancelled)
                {
                    //Dispose any resource that might have been initialized above
                    return; //
                }

                if (timerPeriod != Timeout.Infinite)
                {
                    timerInit(timerPeriod);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("A disposed object accessed");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("A nulled object accessed");
            }
            catch (Exception ex)
            {

            }
        }

        public void releaseTimer()
        {
            timerCanceller.Cancelled = true;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            timer.Dispose();
        }

        public void Dispose()
        {
            releaseTimer();
            dummy = null;   //for testing
            GC.SuppressFinalize(this);
        }
    }

    class TimerCanceller
    {
        public bool Cancelled = false;
    }

}

