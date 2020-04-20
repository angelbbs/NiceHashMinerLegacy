using HashLib;
using Newtonsoft.Json;
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


namespace NiceHashMiner.Miners
{
    class DHClient
    {
        internal static TcpClient tcpClient = null;
        public static bool checkConnection = false;
        public static bool needUpdate = false;
        private static int waitReconnect = 60;
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
        /*
        public static TcpState GetState(this TcpClient tcpClient)
        {
            var foo = IPGlobalProperties.GetIPGlobalProperties()
              .GetActiveTcpConnections()
              .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient.Client.LocalEndPoint));
            return foo != null ? foo.State : TcpState.Unknown;
        }
        */

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
            //new Task(() => ConnectToPool()).Start();
            while (checkConnection)
            {
                Thread.Sleep(500);
                /*
                if (tcpClient == null)
                {
                    new Task(() => ConnectToPool()).Start();
                }
                */
                //if (!isClientConnected(tcpClient) && checkConnection)
                if (!checkConnection) break;

                if (tcpClient == null)
                {
                    Helpers.ConsolePrint("DaggerHashimoto3GB", "Start connection");
                    Thread.Sleep(1000 * 2);
                    new Task(() => ConnectToPool()).Start();
                    Thread.Sleep(1000 * 5);
                }
                if (!tcpClient.Connected)
                {
                    /*
                    if (serverStream != null)
                    {
                        serverStream.Close();
                        serverStream.Dispose();
                    }
                    */
                    if (tcpClient != null)
                    {
                        /*
                        tcpClient.Close();
                        tcpClient.Dispose();
                        */
                    }

                    if (checkConnection)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Reconnect wait: " + waitReconnect.ToString() + " sec");
                        Thread.Sleep(1000 * waitReconnect);
                        new Task(() => ConnectToPool()).Start();
                    }
                }
            }
            if (tcpClient != null)
            {
                tcpClient.Client.Disconnect(false);
                tcpClient.Close();
            }
            /*
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient.Dispose();
            }
            */
        }
        public static void StopConnection()
        {
            try
            {
                checkConnection = false;
                /*
                if (tcpClient != null)
                {
                    tcpClient.Client.Disconnect(false);
                    tcpClient.Close();
                }
                */
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("DaggerHashimoto3GB", ex.ToString());
            }
        }
        public static void StartConnection()
        {
            NHSmaData.UpdatePayingForAlgo(AlgorithmType.DaggerHashimoto3GB, 0.0d);
            NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
            //Thread.Sleep(1000 * 10);
            Helpers.ConsolePrint("DaggerHashimoto3GB", "Start monitoring");
            //new Task(() => ConnectToPool()).Start();
            //Thread.Sleep(1000);
            checkConnection = true;
            tcpClient = null;
            new Task(() => CheckConnectionToPool()).Start();
        }

        public static void ConnectToPool()
        {
            //while (true)
            {
                NHSmaData.UpdatePayingForAlgo(AlgorithmType.DaggerHashimoto3GB, 0.0d);
                NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(DNStoIP("daggerhashimoto.eu.nicehash.com")), 3353);
                NetworkStream serverStream = tcpClient.GetStream();
                serverStream.ReadTimeout = 1000 * 180;
                ReadFromServer(serverStream, tcpClient);
                
                checkConnection = true;
                //new Task(() => DHClient.CheckConnectionToPool()).Start();

                /*
                while (tcpClient.Connected)
                {
                    Thread.Sleep(1000);
                }
                */
                //Helpers.ConsolePrint("DaggerHashimoto3GB", "Wait 10 sec");
                //Thread.Sleep(1000 * 10);
            }
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
            bool Epoch3GB = false;
            
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
            waitReconnect = 60;

            if (serverStream == null)
            {
                Helpers.ConsolePrint("DaggerHashimoto3GB", "Error in serverStream");
                return;
            }
            serverStream.Write(subscribeBytes, 0, subscribeBytes.Length);

            for (int i = 0; i < 1024; i++)
            {
                messagePool[i] = 0;
            }

            while (checkConnection)
            {
                Thread.Sleep(100);
                int serverBytes;
                //if (serverStream.CanRead)
                {
                    try
                    {
                        if (tcpClient.Connected)
                        {
                            for (int i = 0; i < 1024; i++)
                            {
                                messagePool[i] = 0;
                            }

                            serverBytes = serverStream.Read(messagePool, 0, 8192);
                            //serverBytes = serverStream.Read(messagePool, 0, 8192);
                            //object o = bf.Deserialize(serverStream);
                            //string msg = (string)o;
                            //var poolData = msg.ToString();
                            //Helpers.ConsolePrint("DaggerHashimoto3GB", "read: " + msg);
                            //serverStream.ReadTimeout = 1000 * 60;
                            //от пула все нули
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
                                Helpers.ConsolePrint("DaggerHashimoto3GB", "clientZero");
                            }
                            /*
                            if (SavePackets)
                            {
                                if (!Directory.Exists("temp//3GB")) Directory.CreateDirectory("temp//3GB");
                                np++;
                                string cpacket0 = "";
                                for (int i = 0; i < 2048; i++)
                                {
                                    cpacket0 = cpacket0 + (char)messagePool[i];
                                }
                                //if (cpacket0.Length > 60)
                                File.WriteAllText("temp//3GB//" + np.ToString() + "server.pkt", cpacket0);
                            }
                            */
                            // jsonrpc
                            var poolData = Encoding.ASCII.GetString(messagePool);
                            /*
                            if (!s1.Contains("jsonrpc"))
                            {
                                break;
                            }
                            */
                            var poolAnswer = poolData.Split((char)0)[0];
                            //Helpers.ConsolePrint("DaggerHashimoto3GB", "<- " + poolAnswer);

                            if (poolAnswer.Contains("mining.notify") && !poolAnswer.Contains("method"))
                            {
                                serverStream.Write(authorizeBytes, 0, authorizeBytes.Length);
                            }

                            if (poolAnswer.Contains("mining.notify") && poolAnswer.Contains("method"))//job
                            {
                                poolAnswer = poolAnswer.Replace("}{", "}" + (char)10 + "{");
                                int amount = poolAnswer.Split(new char[] { (char)10 }, StringSplitOptions.None).Count() - 1;
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", amount.ToString());
                                for (var i = 0; i <= amount; i++)
                                {
                                    if (poolAnswer.Split((char)10)[i].Contains("mining.notify"))
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(poolAnswer.Split((char)10)[i]);
                                        string seedhash = json.@params[1];
                                        epoch = Epoch(seedhash);
                                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Epoch = " + epoch.ToString());
                                        bool previousEpoch = Epoch3GB;
                                        if (epoch < 235) //win 7
                                        {
                                            Divert.DaggerHashimoto3GBProfit = true;
                                            Divert.DaggerHashimoto3GBForce = true;
                                            Thread.Sleep(2000);//wait for stop
                                            /*
                                            if (previousEpoch != Epoch3GB) needUpdate = true;
                                            Divert.Dagger3GBEpochCount = 0;
                                            Divert.Dagger3GBJob = poolAnswer.Split((char)10)[i];
                                            Epoch3GB = true;
                                            */
                                            //checkConnection = false;
                                        } else
                                        {
                                            //Divert.Dagger3GBEpochCount++;
                                           // Epoch3GB = false;
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
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", tosend);
                                //break;
                            }

                            if (poolAnswer.Contains("client.reconnect"))
                            {
                                Helpers.ConsolePrint("DaggerHashimoto3GB", "Reconnect receive");
                                waitReconnect = 600;
                                Divert.Dagger3GBEpochCount = 999;
                                if (tcpClient != null)
                                {
                                    if (tcpClient.Client != null)
                                    {
                                        tcpClient.Client.Disconnect(false);
                                        tcpClient.Client.Shutdown(SocketShutdown.Both);
                                    }
                                    tcpClient.Close();
                                    tcpClient.Dispose();
                                    tcpClient = null;
                                }
                                serverStream.Close();
                                serverStream.Dispose();
                                serverStream = null;
                            }

                            if (poolAnswer.Contains("Invalid JSON request"))
                            {
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", tosend);
                                break;
                            }
 
                            byte[] bytes = Encoding.ASCII.GetBytes(poolAnswer);
                            //serverStream.Write(bytes, 0, bytes.Length);
                            bytes = null;

                        } else
                        {
                            Helpers.ConsolePrint("DaggerHashimoto3GB", "Disconnected");
                            //Divert.Dagger3GBEpochCount = 999;
                            //waitReconnect = 120;
                            if (tcpClient != null)
                            {
                                if (tcpClient.Client != null)
                                {
                                    tcpClient.Client.Disconnect(false);
                                    tcpClient.Client.Shutdown(SocketShutdown.Both);
                                }
                                tcpClient.Close();
                                tcpClient.Dispose();
                                tcpClient = null;
                            }
                            serverStream.Close();
                            serverStream.Dispose();
                            serverStream = null;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Disconnected: " + ex.Message);
                        //Divert.Dagger3GBEpochCount = 999;
                        if (tcpClient != null)
                        {
                            /*
                            if (tcpClient.Client != null)
                            {
                                tcpClient.Client.Disconnect(false);
                                tcpClient.Client.Shutdown(SocketShutdown.Both);
                            }
                            */
                            tcpClient.Close();
                            tcpClient.Dispose();
                            tcpClient = null;
                        }
                        serverStream.Close();
                        serverStream.Dispose();
                        serverStream = null;
                        //Helpers.ConsolePrint("DaggerHashimoto3GB", ex.Message);
                        break;
                    }
                //    Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle15");
                }
                /*
                if (serverBytes == 0)
                {
                    Helpers.ConsolePrint("DaggerHashimoto3GB", "serverBytes == 0");
                    //break;
                }
                */
            }
        }
    }

}
