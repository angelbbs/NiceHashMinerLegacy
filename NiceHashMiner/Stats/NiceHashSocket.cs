﻿/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using Newtonsoft.Json;
using NiceHashMiner.Switching;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

using NiceHashMinerLegacy.UUID;
using NiceHashMiner.Configs;
using SystemTimer = System.Timers.Timer;
using Timer = System.Windows.Forms.Timer;

namespace NiceHashMiner.Stats
{
    public class NiceHashSocket
    {
        #region JSON Models
#pragma warning disable 649, IDE1006

        private class NicehashLogin
        {
            public string method = "login";
            public string version;
            public int protocol = 1;
        }

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
        public  static bool _connectionEstablished;
        private readonly Random _random = new Random();
        private readonly string _address;

       // public event EventHandler OnConnectionEstablished;
        public event EventHandler<MessageEventArgs> OnDataReceived;
       // public event EventHandler OnConnectionLost;
        public static string RigID => UUID.GetDeviceB64UUID();
        private Timer _attemptReconnect;

        public NiceHashSocket(string address)
        {
            _address = address;
        }

        //****************************************************************************************************************

        public void StartConnectionNew(string btc = null, string worker = null, string group = null)
        {
            NHSmaData.InitializeIfNeeded();
            _connectionAttempted = true;

            // TESTNET
#if TESTNET || TESTNETDEV || PRODUCTION_NEW
            _login.rig = ApplicationStateManager.RigID;

            if (btc != null) _login.btc = btc;
            if (worker != null) _login.worker = worker;
            if (group != null) _login.group = group;
#endif
            // Helpers.ConsolePrint("rig:", RigID);
         //  NiceHashStats.LoadSMA(); //for first run
            try
            {
                if (_webSocket == null)
                {
                    _webSocket = new WebSocket(_address, true);

                    //_webSocket.OnOpen += Login;
                    }
                else
                {
                    Helpers.ConsolePrint("SOCKET", $"Credentials change reconnecting nhmws");
                    _connectionEstablished = false;
                    _restartConnection = true;
                    //_webSocket?.Close(CloseStatusCode.Normal, $"Credentials change reconnecting {ApplicationStateManager.Title}.");
                    //_webSocket?.Close(CloseStatusCode.Normal, $"Credentials change reconnecting.");
                    _webSocket.Close();
                }
                Helpers.ConsolePrint("SOCKET", "Connecting");
            //    _webSocket.OnOpen += Login;
                _webSocket.OnOpen += ConnectCallback;
                _webSocket.OnMessage += ReceiveCallbackNew;
                _webSocket.OnError += ErrorCallbackNew;
                _webSocket.OnClose += CloseCallbackNew;
                _webSocket.Log.Level = LogLevel.Debug;
                _webSocket.Log.Output = (data, s) => Helpers.ConsolePrint("SOCKET", data.ToString());
                _webSocket.EnableRedirection = true;

                _webSocket.Connect();
                Helpers.ConsolePrint("SOCKET", "Connected");
                _connectionEstablished = true;
                _restartConnection = false;
                _endConnection = true;

            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", "Connection error: " + e.ToString());
            }
        }

        private void ReceiveCallbackNew(object sender, MessageEventArgs e)
        {
            OnDataReceived?.Invoke(this, e);
        }

        private static void ErrorCallbackNew(object sender, ErrorEventArgs e)
        {
            Helpers.ConsolePrint("NiceHashSocket", $"Error occured: {e.Message}");
            NiceHashStats._deviceUpdateTimer.Stop();
            new Task(() => NiceHashStats.SetDeviceStatus("PENDING")).Start();
            NiceHashStats._deviceUpdateTimer.Start();
        }

        private void CloseCallbackNew(object sender, CloseEventArgs e)
        {
            Helpers.ConsolePrint("NiceHashSocket", $"Connection closed code {e.Code}: {e.Reason}");
            if (e.Code == 1000)
            {
                if (_webSocket != null)
                {
                    _webSocket.Close();
                }
                Thread.Sleep(1000 * 20);
            }

            /*
            if (!_restartConnection)
            {
                AttemptReconnectNew();
            }
            */
        }

        // Don't call SendData on UI threads, since it will block the thread for a bit if a reconnect is needed
        public bool SendDataNew(string data, bool recurs = false)
        {
            //TESTNET
#if TESTNET || TESTNETDEV || PRODUCTION_NEW
            // skip sending if no btc set send only login
            if (CredentialValidators.ValidateBitcoinAddress(_login.btc) == false && data.Contains("{\"method\":\"login\"") == false)
            {
                NiceHashMinerLegacy.Common.Logger.Info("SOCKET", "Skipping SendData no BTC address");
                return false;
            }
#endif
            try
            {
                // Make sure connection is open
                if (_webSocket != null && IsAlive)
                {
                    Helpers.ConsolePrint("SOCKETNEW", $"Sending data: {data}");
                    _webSocket.Send(data);
                    return true;
                }
                else if (_webSocket != null)
                {
                    _webSocket = null; //force
                    StartConnectionNew();
                    /*
                    if (AttemptReconnectNew() && !recurs)
                   // if (AttemptReconnectNew())
                    {
                        // Reconnect was successful, send data again (safety to prevent recursion overload)
                        SendDataNew(data, true);
                    }
                    else
                    {
                        Helpers.ConsolePrint("SOCKETNEW", "Socket connection unsuccessfull, will try again on next device update (1min)");
                    }
                    */
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
                        if (_webSocket != null)
                        {
                            _webSocket.Close();
                            _webSocket = null;
                        }
                        Thread.Sleep(5000);
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
            var sleep = _connectionEstablished ? 10 + _random.Next(0, 20) : 0;
            Helpers.ConsolePrint("SOCKET", $"Attempting reconnect" +
                $"" +
                $"" +
                $" in {sleep} seconds");
            // More retries on first attempt
            var retries = _connectionEstablished ? 5 : 25;
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
                try
                {
                    _webSocket.Connect();
                    Thread.Sleep(100);
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
                        if (_webSocket != null)
                        {
                            _webSocket.Close();
                            _webSocket = null;
                        }
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
           // OnConnectionLost?.Invoke(null, EventArgs.Empty);
            return false;
        }

        
        private void ConnectCallback(object sender, EventArgs e)
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
                    Configs.ConfigManager.GeneralConfig.CpuID = CpuID;
                }

                if (Configs.ConfigManager.GeneralConfig.MachineGuid.Length == 0)
                {
                    Configs.ConfigManager.GeneralConfig.MachineGuid = rig;
                }
                /*
                var macUUID = WindowsMacUtils.GetMAC_UUID();
                Helpers.ConsolePrint("UUID", macUUID);
                Helpers.ConsolePrint("UUID", Configs.ConfigManager.GeneralConfig.MachineGuid);
                Helpers.ConsolePrint("UUID", RigID);
                Helpers.ConsolePrint("UUID", rig);
                Helpers.ConsolePrint("UUID", Configs.ConfigManager.GeneralConfig.CpuID);
                Helpers.ConsolePrint("UUID", CpuID);
                */
                if (!Configs.ConfigManager.GeneralConfig.MachineGuid.Equals(rig) && Configs.ConfigManager.GeneralConfig.CpuID.Equals(CpuID))
                {
                    /*
                    Helpers.ConsolePrint("UUID", Configs.ConfigManager.GeneralConfig.MachineGuid);
                    Helpers.ConsolePrint("UUID", RigID);
                    Helpers.ConsolePrint("UUID", rig);
                    Helpers.ConsolePrint("UUID", Configs.ConfigManager.GeneralConfig.CpuID);
                    Helpers.ConsolePrint("UUID", CpuID);
                    */
                    Helpers.ConsolePrint("UUID", "New MachineGuid. Maybe error. Using previous MachineGuid");
                    rig = Configs.ConfigManager.GeneralConfig.MachineGuid;
                }
                var version = "NHML/1.9.1.12";//на старой платформе нельзя отправлять версию форка. Страница статистики падает )))

                    protocol = 3;
                    version = "NHML/3.0.0.5"; //
                    if (ConfigManager.GeneralConfig.Send_actual_version_info)
                    {
                        version = "NHML/Fork Fix " + ConfigManager.GeneralConfig.ForkFixVersion.ToString().Replace(",", ".");
                    }
                    btc = Configs.ConfigManager.GeneralConfig.BitcoinAddressNew;
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
                    //loginJson = loginJson.Replace("{", " { ");
                   // SendDataNew(loginJson);
                new Task(() => SendDataNew(loginJson)).Start();

                //NiceHashStats._deviceUpdateTimer.Stop();
                Thread.Sleep(1000);
                //new Task(() => NiceHashStats.SetDeviceStatus("PENDING")).Start();
               // Thread.Sleep(1000);
                new Task(() => NiceHashStats.SetDeviceStatus(null)).Start();
                // NiceHashStats._deviceUpdateTimer.Start();


                //NiceHashStats.SetDeviceStatus(null);
                /*
                NiceHashStats.SetDeviceStatus("PENDING");
                Thread.Sleep(100);
                NiceHashStats.SetDeviceStatus("STOPPED");
                */


                //  OnConnectionEstablished?.Invoke(null, EventArgs.Empty);
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
        /*
        private void CloseCallback(object sender, CloseEventArgs e)
        {
            if (!_restartConnection)
            {
                Helpers.ConsolePrint("SOCKET", $"Connection closed code {e.Code}: {e.Reason}");
                AttemptReconnect();
            }
        }
        */
        private Task<bool> SendAsync(string data)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                _webSocket.SendAsync(data, b => t.TrySetResult(b));
                return t.Task;
            });
        }
        // Don't call SendData on UI threads, since it will block the thread for a bit if a reconnect is needed
        // public bool SendData(string data, bool recurs = false)
        public async Task<bool> SendData(string data, bool recurs = false)
        {
            try
            {
                if (_webSocket != null && _webSocket.IsAlive)
                {
                    // Make sure connection is open
                    // Verify valid JSON and method
                    dynamic dataJson = JsonConvert.DeserializeObject(data);
                    if (dataJson.method == "credentials.set" || dataJson.method == "devices.status" || dataJson.method == "miner.status" || dataJson.method == "login" || dataJson.method == "executed")
                    {
                        Helpers.ConsolePrint("SOCKET", "Sending data: " + data);
                        _webSocket.Send(data);
                        //return true;
                        return await SendAsync(data);
                    }
                } else if (_webSocket == null)
                {
                    Helpers.ConsolePrint("SOCKET", "Force reconnect");
                    if (_webSocket != null)
                    {
                        _webSocket.Close();
                        _webSocket = null;
                    }
                    Thread.Sleep(5000);
                    StartConnectionNew();
                    /*
                  //  if (AttemptReconnect() && !recurs)
                    if (AttemptReconnect())
                    {
                        // Reconnect was successful, send data again (safety to prevent recursion overload)
                        //SendData(data, true);
                        await SendData(data, true);
                    } else
                    {
                        Helpers.ConsolePrint("SOCKET", "Socket connection unsuccessfull, will try again on next device update (1min)");
                    }
                    */
                } else
                {
                    if (_webSocket.IsAlive)
                    {
                        Helpers.ConsolePrint("SOCKET", "Socket alive");
                    } else
                    {
                        Helpers.ConsolePrint("SOCKET", "Socket died, retrying");
                        _webSocket.Close();
                        _webSocket = null;
                        Thread.Sleep(5000);
                        StartConnectionNew();
                    }
                }
            } catch (Exception e)
            {
                Helpers.ConsolePrint("SOCKET", e.ToString());
            }
            return false;
        }

        private bool AttemptReconnect0()
        {
            attemptReconnect_Tick();
            NiceHashStats.GetSmaAPICurrent();
           // ExchangeRateApi.GetNewBTCRate();
            if (_attemptingReconnect)
            {
                return false;
            }
            if (IsAlive)
            {
                // no reconnect needed
                return true;
            }

            //   return false;
            /*
            _attemptReconnect = new Timer();
            _attemptReconnect.Tick += attemptReconnect_Tick;
            _attemptReconnect.Interval = 10000;

            _attemptReconnect.Start();
            */

            //  _attemptReconnect = new System.Threading.Timer(DeviceStatus_TickNew, null, DeviceUpdateInterval, DeviceUpdateInterval);
            return false;
        }
        private async void attemptReconnect_Tick()
        {
            //_attemptReconnect.Stop();
            //_attemptReconnect = null;
            _attemptingReconnect = true;
            var sleep = _connectionEstablished ? 10 + _random.Next(0, 5) : 0;
            Helpers.ConsolePrint("SOCKET", "Attempting reconnect in " + sleep + " seconds");
            // More retries on first attempt
            var retries = _connectionEstablished ? 5 : 25;
            if (_connectionEstablished)
            {
                // Don't wait if no connection yet
                await Task.Delay(sleep * 1000);
            } else
            {
                // Don't not wait again
                _connectionEstablished = true;
            }
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    _webSocket.Connect();
                    Thread.Sleep(100);
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
                        // Need to recreate websocket
                       // Helpers.ConsolePrint("SOCKET", "Try old method");
                       // _webSocket = null;
                       // StartConnectionold();
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
          //  OnConnectionLost?.Invoke(null, EventArgs.Empty);
            return;
        }
    }
}
