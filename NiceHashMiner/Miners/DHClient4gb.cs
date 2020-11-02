﻿using HashLib;
using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Configs.Data;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
using NiceHashMinerLegacy.Divert;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinDivertSharp;

namespace NiceHashMiner.Miners
{
    class DHClient4gb
    {
        internal static TcpClient tcpClient = null;
        public static NetworkStream serverStream = null;
        private static List<TcpClient> tcpClientList = new List<TcpClient>();
        public static bool needStart = false;
        private static int waitReconnect = 300;
        private static int ci = 0;
        public static string DNStoIP(string IPName)
        {
            try
            {
                var ASCII = new System.Text.ASCIIEncoding();
                var heserver = Dns.GetHostEntry(IPName);
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
                    {
                        return curAdd.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("Exception: " + e.ToString());
            }
            return "";
        }

        public static bool isClientConnected( TcpClient _tcpClient)
        {
            if (_tcpClient == null) return false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation c in tcpConnections)
            {
                TcpState stateOfConnection = c.State;

                if (c.LocalEndPoint.Equals(_tcpClient.Client.LocalEndPoint) && c.RemoteEndPoint.Equals(_tcpClient.Client.RemoteEndPoint))
                {
                    if (stateOfConnection == TcpState.Established)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

            }
            return false;
        }
        public static void CheckConnectionToPool()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (!Divert.checkConnection3GB) break;

                if (tcpClient == null)
                {
                    Helpers.ConsolePrint("DaggerHashimoto4GB", "Start connection");
                    new Task(() => ConnectToPool()).Start();
                } else
                {
                    Helpers.ConsolePrint("DaggerHashimoto4GB", "tcpClient != null");
                }

                if (!tcpClient.Connected)
                {
                    if (Divert.checkConnection3GB)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto4GB", "Reconnect wait: " + waitReconnect.ToString() + " sec");
                        Thread.Sleep(1000 * waitReconnect);
                        new Task(() => ConnectToPool()).Start();
                    }
                }
                else
                {
                    Helpers.ConsolePrint("DaggerHashimoto4GB", "tcpClient.Connected");
                }
            }
        }
        public static void StopConnection()
        {
            Divert.checkConnection3GB = false;
            Helpers.ConsolePrint("DaggerHashimoto4GB", "StopConnection()");
            try
            {
                Thread.Sleep(200);
                //if (tcpClient != null)


                    if (DHClient4gb.serverStream != null)
                    {
                        serverStream.Close();
                    DHClient4gb.serverStream = null;
                    }

            } catch (Exception ex)
            {
                Helpers.ConsolePrint("DaggerHashimoto4GB", ex.ToString());
            }
        }
        public static void StartConnection()
        {
            var DivertHandle = Divert.OpenWinDivert("!loopback && outbound && tcp.DstPort == 9999");
            if ((int)DivertHandle <= 0)
            {
                Helpers.ConsolePrint("DaggerHashimoto4GB", "WinDivert error. DaggerHashimoto4GB not running");
                return;
            }
            WinDivert.WinDivertClose(DivertHandle);

            Divert.checkConnection3GB = true;
            new Task(() => ConnectToPool()).Start();
        }

        public static void ConnectToPool()
        {
            LingerOption lingerOption = new LingerOption(true, 0);
            while (Divert.checkConnection3GB)
            {
                string[,] myServers = Form_Main.myServers;
                Random r = new Random();
                int r1 = r.Next(0, 5);
                IPAddress addr = IPAddress.Parse(DNStoIP("daggerhashimoto." + myServers[0, 0] + ".nicehash.com"));
                IPAddress addrl = IPAddress.Parse("0.0.0.0");

                serverStream = null;
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                    tcpClient = null;
                }
                var iep = new IPEndPoint(addrl, 3353);

                if (tcpClient == null)
                {
                    try
                    {
                        using (TcpClient tcpClient = new TcpClient() { SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = lingerOption })
                        {
                            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                            tcpClient.ConnectAsync(addr, 3353);

                            while (!tcpClient.Connected)
                            {
                                Thread.Sleep(1000);
                            }
                            using (serverStream = tcpClient.GetStream())
                            {
                                serverStream.ReadTimeout = 1000 * 240;
                                ReadFromServer(serverStream, tcpClient);
                            }
                            tcpClient.Close();
                        }
                    } catch (Exception ex)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto4GB", "Exception: " + ex);
                    }
                } else
                {
                    Helpers.ConsolePrint("DaggerHashimoto4GB", "Already connected");
                    ReadFromServer(serverStream, tcpClient);
                }

                if (!Divert.checkConnection3GB)
                {
                        Helpers.ConsolePrint("DaggerHashimoto4GB", "Disconnected. Stop connecting");
                        Thread.Sleep(1000);
                    break;
                } else
                {
                        Helpers.ConsolePrint("DaggerHashimoto4GB", "Disconnected. Need reconnect");
                    //StopConnection();
                    Divert.checkConnection3GB = false;
                    Thread.Sleep(5000);
                    Divert.checkConnection3GB = true;
                    StartConnection();
                    //Form_Main.MakeRestart(0);
                }

                Thread.Sleep(5 * 1000);
            }
            Helpers.ConsolePrint("DaggerHashimoto4GB", "Disconnected. End connection");

        }

        public static byte[] StringToByteArray(String hex)
        {
            int numChars = hex.Length;
            byte[] bytes = new byte[numChars / 2];
            for (int i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static int Epoch(string Seedhash)
        {
            byte[] seedhashArray = StringToByteArray(Seedhash);
            byte[] s = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            IHash hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            int i;
                for (i = 0; i < 2048; ++i)
                {
                    if (s.SequenceEqual(seedhashArray))
                        break;
                    s = hash.ComputeBytes(s).GetBytes();
                }
                if (i >= 2048)
                    throw new Exception("Invalid seedhash.");
                return i;
        }

        public static void ReadFromServer(Stream serverStream, TcpClient tcpClient) //от пула
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            bool Epoch4GB = false;

            byte[] messagePool = new byte[8192];
            int np = 0;
            int poolBytes;

            string subscribe = "{\"id\": 1, \"method\": \"mining.subscribe\", \"params\": [\"EthereumMiner/1.0.0\", \"EthereumStratum/1.0.0\"]}" + (char)10;
            string btcAdress = Configs.ConfigManager.GeneralConfig.BitcoinAddressNew;
            string worker = Configs.ConfigManager.GeneralConfig.WorkerName;
            string username = btcAdress + "." + worker + "$" + NiceHashMiner.Stats.NiceHashSocket.RigID;
            string extranonce = "{\"id\":2, \"method\": \"mining.extranonce.subscribe\", \"params\": []}" + (char)10;
            string authorize = "{\"id\": 2, \"method\": \"mining.authorize\", \"params\": [\"" + username + "\", \"x\"]}" + (char)10;
            string noop = "{\"id\": 50, \"method\": \"mining.noop\"}" + (char)10;
            string hashrate = "{\"id\": 16, \"method\": \"mining.hashrate\", \"params\": [\"500000\",\"" + worker + "\"]}" + (char)10;
            string submit = "{\"id\": 4, \"method\": \"mining.submit\", \"params\": [\"" + worker + "\", \"0000000024e7caa6\", \"026d26df7b\"]}" + (char)10;
            byte[] subscribeBytes = Encoding.ASCII.GetBytes(subscribe);
            byte[] authorizeBytes = Encoding.ASCII.GetBytes(extranonce + authorize);
            byte[] noopBytes = Encoding.ASCII.GetBytes(noop);
            byte[] hashrateBytes = Encoding.ASCII.GetBytes(hashrate);
            byte[] submitBytes = Encoding.ASCII.GetBytes(submit);
            int epoch = 999;
            waitReconnect = 300;

            if (serverStream == null)
            {
                Helpers.ConsolePrint("DaggerHashimoto4GB", "Error in serverStream");
                return;
            }
            serverStream.Write(subscribeBytes, 0, subscribeBytes.Length);

            for (int i = 0; i < 1024; i++)
            {
                messagePool[i] = 0;
            }

            while (Divert.checkConnection3GB)
            {
                Thread.Sleep(100);
                int serverBytes;
                //if (serverStream.CanRead)

                try
                {
                    if (tcpClient.Connected)
                    {
                        for (int i = 0; i < 1024; i++)
                        {
                            messagePool[i] = 0;
                        }

                        serverBytes = serverStream.Read(messagePool, 0, 8192);

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
                            //   continue;
                            Helpers.ConsolePrint("DaggerHashimoto4GB", "clientZero");
                            break;
                        }

                        // jsonrpc
                        var poolData = Encoding.ASCII.GetString(messagePool);

                        var poolAnswer = poolData.Split((char)0)[0];
                        //Helpers.ConsolePrint("DaggerHashimoto4GB", "<- " + poolAnswer);

                        if (poolAnswer.Contains("mining.notify") && !poolAnswer.Contains("method"))
                        {
                            serverStream.Write(authorizeBytes, 0, authorizeBytes.Length);
                        }

                        if (poolAnswer.Contains("mining.notify") && poolAnswer.Contains("method"))//job
                        {
                            poolAnswer = poolAnswer.Replace("}{", "}" + (char)10 + "{");
                            int amount = poolAnswer.Split(new char[] { (char)10 }, StringSplitOptions.None).Count() - 1;
                            //Helpers.ConsolePrint("DaggerHashimoto4GB", amount.ToString());
                            for (var i = 0; i <= amount; i++)
                            {
                                if (poolAnswer.Split((char)10)[i].Contains("mining.notify"))
                                {
                                    dynamic json = JsonConvert.DeserializeObject(poolAnswer.Split((char)10)[i]);
                                    string seedhash = json.@params[1];
                                    epoch = Epoch(seedhash);
                                    Helpers.ConsolePrint("DaggerHashimoto4GB", "Epoch = " + epoch.ToString());
                                    bool previousEpoch = Epoch4GB;
                                    if (epoch < ConfigManager.GeneralConfig.DaggerHashimoto4GBMaxEpoch) //win 10
                                    {
                                        Divert.DaggerHashimoto4GBProfit = true;
                                        Divert.DaggerHashimoto4GBForce = true;
                                        Thread.Sleep(2000);//wait for stop

                                    }
                                    else
                                    {

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
                            //Helpers.ConsolePrint("DaggerHashimoto4GB", tosend);
                            //break;
                        }

                        if (poolAnswer.Contains("client.reconnect"))
                        {
                            Helpers.ConsolePrint("DaggerHashimoto4GB", "Reconnect receive");
                            waitReconnect = 600;
                            Divert.Dagger4GBEpochCount = 999;
                            tcpClient.Close();
                            tcpClient.Dispose();
                            tcpClient = null;
                        }

                        if (poolAnswer.Contains("Invalid JSON request"))
                        {
                            //Helpers.ConsolePrint("DaggerHashimoto4GB", tosend);
                            break;
                        }

                        byte[] bytes = Encoding.ASCII.GetBytes(poolAnswer);
                        //serverStream.Write(bytes, 0, bytes.Length);
                        bytes = null;

                    }
                    else
                    {
                        Helpers.ConsolePrint("DaggerHashimoto4GB", "Disconnected");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("DaggerHashimoto4GB", "Disconnected ex: " + ex.Message);
                    break;
                }

            }
        }
    }

}