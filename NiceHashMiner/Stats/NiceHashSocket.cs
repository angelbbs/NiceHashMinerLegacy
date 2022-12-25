using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.UUID;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

namespace NiceHashMiner.Stats
{
    public class NiceHashSocket
    {
        #region JSON Models
#pragma warning disable 649, IDE1006

        private class NicehashLoginNew
        {
            public string method = "login";
            public string version;
            public int protocol = 1;
            public string btc;
            public string worker;
            public string group;
            public string rig;
        }
#pragma warning restore 649, IDE1006
        #endregion

        public static WebSocket _webSocket;
        public bool IsAlive => _webSocket.ReadyState == WebSocketState.Open;
        public static bool _restartConnection = false;
        private bool _attemptingReconnect;
        public static bool _endConnection = false;
        private bool _connectionAttempted;
        public static bool _connectionEstablished;
        private readonly Random _random = new Random();
        private readonly string _address;
        private readonly string _addressFailover;
        bool isFailover = false;

        public event EventHandler OnConnectionEstablished;
        public event EventHandler<MessageEventArgs> OnDataReceived;
        public event EventHandler OnConnectionLost;
        public static int wssFailures = 0;

        //public static string RigID => UUID.GetDeviceB64UUID();
        public static string RigID => ConfigManager.GeneralConfig.MachineGuid;
        private static int ForceReconnectCount = 0;
        public NiceHashSocket(string address)
        {
            _address = address;
        }

        //****************************************************************************************************************
        public static bool IsIPAddress(string ipAddress)
        {
            System.Net.IPAddress address;
            bool isIPAddres = false;
            if (System.Net.IPAddress.TryParse(ipAddress, out address))
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    isIPAddres = true;
                }
            }
            return isIPAddres;
        }

        private int _location = 0;
        public void StartConnectionNew(string btc = null, string worker = null, string group = null)
        {
            bool proxy = false;//test
            string proxyUrl = "";
            if (ConfigManager.GeneralConfig.ServiceLocation > 0)
            {
                proxy = true;
                proxyUrl = Globals.MiningLocation[0];
            }
            NHSmaData.InitializeIfNeeded();
            _connectionAttempted = true;
            string ResolvedIP = "";
            string link = Links.CheckDNS(Links.NhmSocketAddress);
            if (ConfigManager.GeneralConfig.ServiceLocation > 0 && Form_Main.wssConnectionsErrors > 2)
            {
                _location++;
                if (_location >= Globals.MiningLocation.Length)
                {
                    _location = 0;
                }
                proxyUrl = Globals.MiningLocation[_location];
            }
            //proxyUrl = proxyUrl.Replace("ru.stratum-proxy.ru", "yandex.ru");
            try
            {
                if (_webSocket == null)
                {
                    _webSocket = new WebSocket(Links.NhmSocketAddress);

                    if (!proxy)
                    {
                        ResolvedIP = new Uri(link).Host;
                        Helpers.ConsolePrint("SOCKET", "Start connection to Nicehash directly");
                    }
                    else
                    {
                        Helpers.ConsolePrint("SOCKET", "Start connection to Nicehash through proxy " + proxyUrl);
                        ResolvedIP = Links.CheckDNS(proxyUrl, true).Replace("stratum+tcp://", "");
                        _webSocket.Port = 6443;
                    }
                    if (IsIPAddress(ResolvedIP))
                    {
                        _webSocket.ResolvedIP = ResolvedIP;
                    }
                }
                else
                {
                    Helpers.ConsolePrint("SOCKET", $"Credentials change reconnecting nhmws");
                    _connectionEstablished = false;
                    _restartConnection = true;
                    _webSocket.Close();
                }
                Form_Main.NHConnectingInProgress = true;
                Form_Main.wssConnectionsErrors++;
                
                _webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                _webSocket.OnOpen += ConnectCallback;
                _webSocket.OnMessage += ReceiveCallbackNew;
                _webSocket.OnError += ErrorCallbackNew;
                _webSocket.OnClose += CloseCallbackNew;
                _webSocket.EmitOnPing = true;
                _webSocket.Log.Level = LogLevel.Debug;
                _webSocket.Log.Output = (data, s) => Helpers.ConsolePrint("SOCKET", data.ToString());
                _webSocket.EnableRedirection = true;
                _webSocket.Connect();
                Helpers.ConsolePrint("SOCKET", "Connected?");
                _connectionEstablished = true;
                _restartConnection = false;
                _endConnection = true;
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
            }
            Form_Main.NHConnectingInProgress = false;
        }
        public static void StopConnection()
        {
            try
            {
                if (_webSocket != null)
                {
                    _webSocket.Close();
                    _webSocket = null;
                }
            }
            catch (Exception)
            {

            }
        }

        private void ReceiveCallbackNew(object sender, MessageEventArgs e)
        {
            OnDataReceived?.Invoke(this, e);
        }

        private static void ErrorCallbackNew(object sender, ErrorEventArgs e)
        {
            Helpers.ConsolePrint("NiceHashSocket", $"Error occured: {e.Message}");
        }

        private void CloseCallbackNew(object sender, CloseEventArgs e)
        {
            Helpers.ConsolePrint("NiceHashSocket", $"Connection closed code {e.Code}: {e.Reason}");
            if (!_restartConnection)
            {
                new Task(() => AttemptReconnectNew()).Start();
                //AttemptReconnectNew();
            }
        }

        // Don't call SendData on UI threads, since it will block the thread for a bit if a reconnect is needed
        public bool SendDataNew(string data, bool recurs = false)
        {
            try
            {
                // Make sure connection is open
                if (_webSocket != null && IsAlive)
                {
                    Helpers.ConsolePrint("SOCKETNEW", $"Sending data: {data}");
                    ForceReconnectCount = 0;
                    _webSocket.Send(data);
                    return true;
                }
                else if (_webSocket != null)
                {
                    _webSocket = null; //force
                    new Task(() => StartConnectionNew()).Start();
                    //StartConnectionNew();
                }
                else
                {
                    if (!_connectionAttempted)
                    {
                        Helpers.ConsolePrint("SOCKETNEW", "Data sending attempted before socket initialization");
                    }
                    else
                    {
                        Helpers.ConsolePrint("SOCKETNEW", "webSocket not created, retrying");
                        StartConnectionNew();
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("NiceHashSocketNew", $"Error occured while sending data: {e.Message}");
            }
            return false;
        }

        private bool AttemptReconnectNew()
        {
            if (_attemptingReconnect || _endConnection)
            {
                return false;
            }
            if (IsAlive)
            {
                // no reconnect needed
                return true;
            }
            _attemptingReconnect = true;
            var sleep = 5;

            // More retries on first attempt
            var retries = 5;
            if (_connectionEstablished)
            {
                // Don't wait if no connection yet
                Thread.Sleep(sleep * 1000);
            }
            else
            {
                // Don't not wait again
                _connectionEstablished = true;
            }
            for (var i = 0; i < retries; i++)
            {
                Helpers.ConsolePrint("SOCKET", $"Attempting reconnect" +
                $"" +
                $"" +
                $" in {sleep} seconds");
                try
                {
                    _webSocket.Connect();
                    Thread.Sleep(1000 * sleep);
                    if (IsAlive)
                    {
                        _attemptingReconnect = false;
                        return true;
                    }
                }
                catch (InvalidOperationException e)
                {
                    if (e.Message == "A series of reconnecting has failed.")
                    {
                        // Need to recreate websocket
                        Helpers.ConsolePrint("SOCKET", "Recreating socket");
                        _webSocket = null;
                        StartConnectionNew();
                        break;
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("NiceHashSocketNew", $"Error while attempting reconnect: {e.Message}");
                }
                Thread.Sleep(500);
            }
            _attemptingReconnect = false;
            OnConnectionLost?.Invoke(null, EventArgs.Empty);
            return false;
        }

        public void ConnectCallback(object sender, EventArgs e)
        {
            try
            {
                //send login
                int protocol = 1;
                string btc;
                string worker;
                string group = "";
                string rig = UUID.GetDeviceB64UUID();
                string CpuID = UUID.GetCpuID();
                if (Configs.ConfigManager.GeneralConfig.CpuID.Length == 0)
                {
                    Helpers.ConsolePrint("UUID", "Unknown CPUId detected. Reseting MachineGuid");
                    Configs.ConfigManager.GeneralConfig.CpuID = CpuID;
                    Configs.ConfigManager.GeneralConfig.MachineGuid = "";
                }
                else
                {
                    if (!Configs.ConfigManager.GeneralConfig.CpuID.Equals(CpuID))
                    {
                        Helpers.ConsolePrint("UUID", "Old CpuID: " + ConfigManager.GeneralConfig.CpuID + " " +
                        "New CpuID: " + CpuID);
                        Configs.ConfigManager.GeneralConfig.MachineGuid = "";
                        Configs.ConfigManager.GeneralConfig.CpuID = CpuID;
                        Helpers.ConsolePrint("UUID", "New CPUId detected. Reseting MachineGuid");
                    }

                }

                if (Configs.ConfigManager.GeneralConfig.MachineGuid.Length == 0)
                {
                    Helpers.ConsolePrint("UUID", "Unknown MachineGuid detected. Reseting MachineGuid");
                    Configs.ConfigManager.GeneralConfig.MachineGuid = rig;
                }
                if (Configs.ConfigManager.GeneralConfig.MachineGuid.Contains("+"))
                {
                    Helpers.ConsolePrint("UUID", "(+) in MachineGuid detected. Reseting MachineGuid");
                    Configs.ConfigManager.GeneralConfig.MachineGuid = rig;
                }

                if (!Configs.ConfigManager.GeneralConfig.MachineGuid.Equals(rig))
                {
                    Helpers.ConsolePrint("UUID", "Old MachineGuid: " + ConfigManager.GeneralConfig.MachineGuid + " " +
                        "New MachineGuid: " + rig);
                    Helpers.ConsolePrint("UUID", "Using old MachineGuid from config");
                    rig = Configs.ConfigManager.GeneralConfig.MachineGuid;
                }
                string version = "NHM/" + ConfigManager.GeneralConfig.NHMVersion;
                string versionAdd = "";

                if (ConfigManager.GeneralConfig.QM_mode)
                {
                    protocol = 4;//nhqm 4
                } else
                {
                    protocol = 3;
                }

                if (ConfigManager.GeneralConfig.Send_actual_version_info)
                {
                    version = "Fork Fix " + ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",", ".");
                }
                if (ConfigManager.GeneralConfig.QM_mode)
                {
                    //versionAdd = "/NHQM _v9.0.0.0";//риги еще не подключены 
                    //versionAdd = "/NHQM_ 9.0.0.0";//риги еще не подключены 
                    //versionAdd = "/NHQM_ vqqqq 9.0.0.0";//риги еще не подключены 
                    //versionAdd = "/NHQM_v 0.0.0.0";//обычный режим 
                    //versionAdd = "/NHQM_v9.0.0.0 mode";//обычный режим  

                    //versionAdd = "/NHQM_v 9.0.0.0";//работает 

                    versionAdd = "//Rig manager mode NHQM_v9.0.0.0";//работает, причЄм строка не показываетс€ в менеджере ригов ))

                    //versionAdd = "/NHQM_v9.0.0.0";//работает 
                    //versionAdd = "/NHQM_v0.5.2.0";//работает 

                    //найсовые программеры прив€зали строку "NHQM_vX.X.X.X" к протоколу авторизации. WTF?
                    //¬ерси€ протокола(protocol) ни на что не вли€ет..

                    //без этого говна не показываетс€ в кабинете потребление карт и температура пам€ти
                }
                else
                {
                    versionAdd = "/NHML";
                }

                version = version + versionAdd;

                btc = Configs.ConfigManager.GeneralConfig.BitcoinAddressNew;
                if (btc.IsNullOrEmpty())
                {
                    btc = Globals.DemoUser;
                }
                worker = Configs.ConfigManager.GeneralConfig.WorkerName;


                var login = new NicehashLoginNew
                {
                    version = version,
                    protocol = protocol,
                    btc = btc,
                    worker = worker,
                    group = group,
                    rig = rig

                };
                var loginJson = JsonConvert.SerializeObject(login);

                //new Task(() => SendDataNew(loginJson)).Start();
                SendDataNew(loginJson);
                Thread.Sleep(500);
                if (Form_Main.MiningStarted)
                {
                    NiceHashStats.SetDeviceStatus("MINING", true);
                } else
                {
                    NiceHashStats.SetDeviceStatus("STOPPED", true);
                }

                OnConnectionEstablished?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception er)
            {
                Helpers.ConsolePrint("SOCKET", er.ToString());
            }
        }

        private void ReceiveCallback(object sender, MessageEventArgs e)
        {
            OnDataReceived?.Invoke(this, e);
        }

        private static void ErrorCallback(object sender, ErrorEventArgs e)
        {
            Helpers.ConsolePrint("SOCKET", e.ToString());
        }

        private void CloseCallback(object sender, CloseEventArgs e)
        {
            if (!_restartConnection)
            {
                Helpers.ConsolePrint("SOCKET", $"Connection closed code {e.Code}: {e.Reason}");
                AttemptReconnect();
            }
        }
        private Task<bool> SendAsync(string data)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                _webSocket.SendAsync(data, b => t.TrySetResult(b));
                return t.Task;
            });
        }

        public static void DropIPPort(int processId, string IP, uint port, bool message = true)
        {
            ProcessStartInfo cports;

            cports = new ProcessStartInfo("utils/cports-x64/cports.exe");
            cports.Arguments = "/close * * " + IP + " " + port.ToString() + " " + processId.ToString();
            cports.UseShellExecute = false;
            cports.RedirectStandardError = false;
            cports.RedirectStandardOutput = false;
            cports.CreateNoWindow = true;
            cports.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                Process.Start(cports);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("DropIPPort", ex.Message);
            }
            if (message)
            {
                Helpers.ConsolePrint("DropIPPort", "Drop port " + IP + ":" + port.ToString() + " completed");
            }
        }

        // Don't call SendData on UI threads, since it will block the thread for a bit if a reconnect is needed
        // public bool SendData(string data, bool recurs = false)
        public async Task<bool> SendData(string data, bool recurs = false)
        {
            List<string> IPsList = new List<string>();
            IPHostEntry heserver;

            try
            {
                heserver = Dns.GetHostEntry("nicehash.com");
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    IPsList.Add(curAdd.ToString());
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WinDivertSharp", ex.ToString());
            }

            try
            {
                if (_webSocket != null && IsAlive)
                {
                    // Make sure connection is open
                    // Verify valid JSON and method
                    dynamic dataJson = JsonConvert.DeserializeObject(data);
                    if (dataJson.method == "credentials.set" || dataJson.method == "devices.status" || dataJson.method == "miner.status" || dataJson.method == "login" || dataJson.method == "executed")
                    {
                        Helpers.ConsolePrint("SOCKET", "Sending data: " + data);
                        ForceReconnectCount = 0;
                        _webSocket.Send(data);
                        dataJson = null;
                        return true;
                        //return await SendAsync(data);
                    }
                }
                else if (_webSocket != null)
                {
                    ForceReconnectCount++;
                    if (ForceReconnectCount > 1000)
                    {
                        Helpers.ConsolePrint("SOCKET", "CRITICAL ERROR! Need restart");
                        Form_Main.MakeRestart(0);
                    }

                    Form_Main.NHConnectingInProgress = true;
                    Helpers.ConsolePrint("SOCKET", "Force reconnect");
                    foreach (var ip in IPsList)
                    {
                        DropIPPort(Process.GetCurrentProcess().Id, ip, 443);
                    }
                    _webSocket = null;
                    Thread.Sleep(3000);
                    new Task(() => StartConnectionNew()).Start();
                    //StartConnectionNew();
                }
                else
                {
                    if (!_connectionAttempted)
                    {
                        Helpers.ConsolePrint("SOCKET", "Data sending attempted before socket initialization");
                    }
                    else
                    {
                        Helpers.ConsolePrint("SOCKET", "webSocket not created, retrying");
                        StartConnectionNew();
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
                foreach (var ip in IPsList)
                {
                    DropIPPort(Process.GetCurrentProcess().Id, ip, 443);
                }
                Thread.Sleep(1000);
            }
            return false;
        }
        private bool AttemptReconnect()
        {
            attemptReconnect_Tick();
            NiceHashStats.GetSmaAPICurrent();
            if (ConfigManager.GeneralConfig.Use_Last24hours || ConfigManager.GeneralConfig.ShortTerm)
            {
                NiceHashStats.GetSmaAPI24h();
            } 
            if (ConfigManager.GeneralConfig.Use_orders_price)
            {
                //NiceHashStats.GetSmaAPIOrder();
                new Task(() => NiceHashStats.GetSmaAPIOrder()).Start();
            }
            ExchangeRateApi.GetNewBTCRate();
            if (_attemptingReconnect)
            {
                return false;
            }
            if (IsAlive)
            {
                // no reconnect needed
                return true;
            }
            return false;
        }
        private async void attemptReconnect_Tick()
        {
            _attemptingReconnect = true;
            int sleep = 5;
            Helpers.ConsolePrint("SOCKET", "Attempting reconnect in " + sleep.ToString() + " seconds");
            // More retries on first attempt
            var retries = _connectionEstablished ? 5 : 8;
            if (_connectionEstablished)
            {
                // Don't wait if no connection yet
                await Task.Delay(sleep * 1000);
            }
            else
            {
                // Don't not wait again
                _connectionEstablished = true;
            }
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    _webSocket.Connect();
                    Thread.Sleep(1000 * 2);
                    if (IsAlive)
                    {
                        _attemptingReconnect = false;
                        return;
                    }
                }
                catch (InvalidOperationException e)
                {
                    if (e.Message == "A series of reconnecting has failed.")
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("SOCKET", $"Error while attempting reconnect: {e}");
                }
                Thread.Sleep(1000);
            }
            _attemptingReconnect = false;
            OnConnectionLost?.Invoke(null, EventArgs.Empty);
            return;
        }
    }
}
