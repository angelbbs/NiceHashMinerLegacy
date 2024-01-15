using HashLib;
using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinDivertSharp;

namespace NiceHashMiner.Miners
{
    class KawpowClient
    {
        private static TcpClient tcpClient = null;
        private static NetworkStream serverStream = null;
        private static int MaxEpoch = 0;
        private static readonly int ETHASH_EPOCH_LENGTH = 7500;

        public static void CheckConnectionToPool()
        {
            if (Form_Main.KawpowLite3GB) MaxEpoch = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch3GB;
            if (Form_Main.KawpowLite4GB) MaxEpoch = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch4GB;
            if (Form_Main.KawpowLite5GB) MaxEpoch = ConfigManager.GeneralConfig.KawpowLiteMaxEpoch5GB;
            Helpers.ConsolePrint("KawpowClient", "Max epoch for KAWPOWLite is " + MaxEpoch.ToString());

            while (true)
            {
                try
                {
                    if (!Divert.checkConnectionKawpowLite) break;

                    if (tcpClient == null)
                    {
                        Helpers.ConsolePrint("KawpowLiteMonitor", "Start connection");
                        new Task(() => ConnectToPool()).Start();
                        Thread.Sleep(5000);
                    } else
                    {
                        //Helpers.ConsolePrint("KawpowLiteMonitor", "tcpClient Connected");
                        if (serverStream is object)
                        {
                            //Helpers.ConsolePrint("KawpowLiteMonitor", "serverStream Connected");
                        }
                    }
                    if (tcpClient is object && tcpClient.Connected && Divert.KawpowLiteMonitorNeedReconnect && serverStream is object)
                    {
                        Divert.KawpowLiteMonitorNeedReconnect = false;
                        Helpers.ConsolePrint("KawpowLiteMonitor", "Need reconnect due divert detect bad epoch");
                        if (tcpClient.Client.Connected)
                        {
                            tcpClient.Client.Disconnect(true);
                        }
                    }
                } catch (Exception ex)
                {
                    Helpers.ConsolePrint("KawpowLiteMonitor", ex.ToString());
                }

                if (tcpClient is object && !tcpClient.Connected)
                {
                    if (serverStream is object)
                    {
                        serverStream.Close();
                        serverStream = null;
                    }
                    if (tcpClient is object)
                    {
                        tcpClient.Close();
                        tcpClient = null;
                    }
                }
                Thread.Sleep(5000);
                if (Divert.KawpowLiteForceStop)
                {
                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, 0.0d);
                    NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
                }
            }
            Helpers.ConsolePrint("KawpowLiteMonitor", "Monitor stopped");
            try
            {
                if (serverStream is object)
                {
                    serverStream.Close();
                    serverStream = null;
                }
                if (tcpClient is object)
                {
                    if (tcpClient.Client.Connected)
                    {
                        tcpClient.Client.Disconnect(true);
                    }
                    tcpClient.Close();
                    tcpClient = null;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("KawpowLiteMonitor", ex.ToString());
            }
        }
        public static void StopConnection()
        {
            Divert.checkConnectionKawpowLite = false;
            Helpers.ConsolePrint("KawpowLiteMonitor", "StopConnection");
            try
            {
                if (tcpClient.Client.Connected)
                {
                    tcpClient.Client.Disconnect(true);
                }
                Thread.Sleep(200);
            }
            catch (Exception ex)
            {
                //Helpers.ConsolePrint("KawpowLiteMonitor", ex.ToString());
            }
        }

        public static void ConnectToPool()
        {
            try
            {
                Helpers.ConsolePrint("KawpowLiteMonitor", "Connect to Nicehash");
                LingerOption lingerOption = new LingerOption(true, 0);

                int _location = ConfigManager.GeneralConfig.ServiceLocation;
                if (ConfigManager.GeneralConfig.ServiceLocation >= Globals.MiningLocation.Length)
                {
                    _location = ConfigManager.GeneralConfig.ServiceLocation - 1;
                }
                var serv = Links.CheckDNS("kawpow." +
                    Globals.MiningLocation[_location], true).Replace("stratum+tcp://", "");
                IPAddress addr = IPAddress.Parse(serv);
                IPAddress addrl = IPAddress.Parse("0.0.0.0");

                int port = 3385;
                if (Globals.MiningLocation[_location].ToLower().Contains("auto"))
                {
                    port = 9200;
                }
                else
                {
                    port = 13385;
                }
                var iep = new IPEndPoint(addrl, port);

                if (tcpClient == null)
                {
                    try
                    {
                        using (tcpClient = new TcpClient() { SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = lingerOption })
                        {
                            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                            tcpClient.ConnectAsync(addr, port);

                            while (!tcpClient.Connected)
                            {
                                Thread.Sleep(1000);
                            }
                            using (serverStream = tcpClient.GetStream())
                            {
                                serverStream.ReadTimeout = 1000 * 240;
                                ReadFromServer(serverStream, tcpClient);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("KawpowLiteMonitor", "Exception: " + ex.ToString());
                    }
                }
                else
                {
                    Helpers.ConsolePrint("KawpowLiteMonitor", "Already connected");
                }
                Helpers.ConsolePrint("KawpowLiteMonitor", "Disconnected.");
                
                if (serverStream is object)
                {
                    serverStream.Close();
                    serverStream = null;
                }
                
                if (tcpClient is object)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("KawpowLiteMonitor", ex.ToString());
            }
        }

        public static int Epoch(string Seedhash)
        {
            int.TryParse(Seedhash, out int sInt);
            return sInt / ETHASH_EPOCH_LENGTH;
        }

        public static void ReadFromServer(Stream serverStream, TcpClient tcpClient) //от пула
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            bool checkserverStream = true;

            byte[] messagePool = new byte[8192];

            string subscribe = "{\"id\":1,\"method\":\"mining.subscribe\",\"params\":[\"GMiner/3.43\",null,\"kawpow.auto.nicehash.com\",\"443\"]}" + (char)10;
            string btcAdress = Configs.ConfigManager.GeneralConfig.BitcoinAddressNew;
            string worker = Configs.ConfigManager.GeneralConfig.WorkerName;
            string username = btcAdress + "." + worker + "$" + NiceHashMiner.Stats.NiceHashSocket.RigID;
            string extranonce = "{\"id\":3, \"method\": \"mining.extranonce.subscribe\", \"params\": []}" + (char)10;
            string authorize = "{\"id\": 2, \"method\": \"mining.authorize\", \"params\": [\"" + username + "\", \"x\"]}" + (char)10;
            string noop = "{\"id\": 50, \"method\": \"mining.noop\"}" + (char)10;
            byte[] subscribeBytes = Encoding.ASCII.GetBytes(subscribe);
            byte[] authorizeBytes = Encoding.ASCII.GetBytes(extranonce + authorize);
            byte[] noopBytes = Encoding.ASCII.GetBytes(noop);

            int epoch = 999;
            int GoodEpochCount = 0;

            try
            {
                if (serverStream == null)
                {
                    Helpers.ConsolePrint("KawpowLiteMonitor", "Error in serverStream");
                    return;
                }
                serverStream.Write(subscribeBytes, 0, subscribeBytes.Length);

                for (int i = 0; i < 1024; i++)
                {
                    messagePool[i] = 0;
                }

                while (Divert.checkConnectionKawpowLite && checkserverStream)
                {
                    Thread.Sleep(100);
                    int serverBytes;

                    try
                    {
                        if (tcpClient.Connected)
                        {
                            for (int i = 0; i < 1024; i++)
                            {
                                messagePool[i] = 0;
                            }

                            try
                            {
                                serverBytes = serverStream.Read(messagePool, 0, 8192);
                            } catch (IOException ioex)
                            {
                                Helpers.ConsolePrint("KawpowLiteMonitor", "IOException: " + ioex.Message);
                            }

                            bool clientZero = true;
                            for (int i = 0; i < 2048; i++)
                            {
                                if (messagePool[i] != (char)0)
                                {
                                    clientZero = false;
                                }
                            }
                            if (clientZero)
                            {
                                //Helpers.ConsolePrint("KawpowLiteMonitor", "clientZero");
                                if (tcpClient.Client.Connected)
                                {
                                    tcpClient.Client.Disconnect(false);
                                }
                                break;
                            }

                            // jsonrpc
                            var poolData = Encoding.ASCII.GetString(messagePool);

                            var poolAnswer = poolData.Split((char)0)[0];
                            //Helpers.ConsolePrint("KawpowLiteMonitor", "<- " + poolAnswer);

                            if (poolAnswer.Contains("id\":1,\"error\":null") && !poolAnswer.Contains("method"))
                            {
                                serverStream.Write(authorizeBytes, 0, authorizeBytes.Length);
                            }

                            if (poolAnswer.Contains("mining.notify") && poolAnswer.Contains("method"))//job
                            {
                                poolAnswer = poolAnswer.Replace("}{", "}" + (char)10 + "{");
                                int amount = poolAnswer.Split(new char[] { (char)10 }, StringSplitOptions.None).Count() - 1;

                                for (var i = 0; i <= amount; i++)
                                {
                                    if (poolAnswer.Split((char)10)[i].Contains("mining.notify"))
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(poolAnswer.Split((char)10)[i]);
                                        string seedhash = json.@params[5];
                                        epoch = Epoch(seedhash);

                                        if (epoch <= MaxEpoch)
                                        {
                                            Helpers.ConsolePrint("KawpowLiteMonitor", "Good epoch: " + epoch.ToString());
                                            NHSmaData.TryGetPaying(AlgorithmType.KAWPOW, out var paying);
                                            NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, paying);
                                            //Divert.KawpowLiteForceStop = false;//это должен устанавливать divert
                                            Divert.KawpowLiteGoodEpoch = true;
                                        }
                                        else
                                        {
                                            Helpers.ConsolePrint("KawpowLiteMonitor", "Bad epoch: " + epoch.ToString());
                                            //NHSmaData.UpdatePayingForAlgo(AlgorithmType.KAWPOWLite, 0.0d);
                                        }
                                    }
                                }
                            }

                            if (poolAnswer.Contains("set_difficulty"))
                            {
                                //serverStream.Write(subscribeBytes, 0, subscribeBytes.Length);
                            }

                            if (poolAnswer.Contains("false"))
                            {
                                //Helpers.ConsolePrint("KawpowLiteMonitor", tosend);
                                //break;
                            }

                            if (poolAnswer.Contains("client.reconnect"))
                            {
                                Helpers.ConsolePrint("KawpowLiteMonitor", "Reconnect receive");
                                if (tcpClient is object)
                                {
                                    tcpClient.Close();
                                    tcpClient = null;
                                }
                            }

                            if (poolAnswer.Contains("Invalid JSON request"))
                            {
                                //Helpers.ConsolePrint("KawpowLiteMonitor", tosend);
                                break;
                            }

                            byte[] bytes = Encoding.ASCII.GetBytes(poolAnswer);
                            //serverStream.Write(bytes, 0, bytes.Length);
                            bytes = null;
                        }
                        else
                        {
                            Helpers.ConsolePrint("KawpowLiteMonitor", "Disconnected");
                            if (serverStream is object)
                            {
                                serverStream.Close();
                                serverStream = null;
                            }
                            if (tcpClient is object)
                            {
                                tcpClient.Close();
                                tcpClient = null;
                            }
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("KawpowLiteMonitor", "Disconnected ex: " + ex.Message);
                        if (serverStream is object)
                        {
                            serverStream.Close();
                            serverStream = null;
                        }
                        if (tcpClient is object)
                        {
                            tcpClient.Close();
                            tcpClient = null;
                        }
                        break;
                    }
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("KawpowLiteMonitor", ex.ToString());
            }
        }
    }
}
