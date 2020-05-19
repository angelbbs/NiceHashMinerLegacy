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
        public static NetworkStream serverStream = null;
        private static List<TcpClient> tcpClientList = new List<TcpClient>();
        public static bool checkConnection = false;
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
            while (true)
            {
                //Helpers.ConsolePrint("DaggerHashimoto3GB", "checkConnection: " + checkConnection);
                Thread.Sleep(1000);
                if (!checkConnection) break;

                if (tcpClient == null)
                {
                    Helpers.ConsolePrint("DaggerHashimoto3GB", "Start connection");
                    new Task(() => ConnectToPool()).Start();
                } else
                {
                    Helpers.ConsolePrint("DaggerHashimoto3GB", "tcpClient != null");
                }
                
                if (!tcpClient.Connected)
                {
                    if (checkConnection)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Reconnect wait: " + waitReconnect.ToString() + " sec");
                        Thread.Sleep(1000 * waitReconnect);
                        new Task(() => ConnectToPool()).Start();
                    }
                }
                else
                {
                    Helpers.ConsolePrint("DaggerHashimoto3GB", "tcpClient.Connected");
                }
            }
        }
        public static void StopConnection()
        {
            Helpers.ConsolePrint("DaggerHashimoto3GB", "StopConnection()");
            try
            {
                Thread.Sleep(200);
                //if (tcpClient != null)
                {
                   
                    if (DHClient.serverStream != null)
                    {
                        serverStream.Close();
                        DHClient.serverStream = null;
                    }
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("DaggerHashimoto3GB", ex.ToString());
            }
        }
        public static void StartConnection()
        {
            //NHSmaData.UpdatePayingForAlgo(AlgorithmType.DaggerHashimoto3GB, 0.0d);
            //NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
            checkConnection = true;
            new Task(() => ConnectToPool()).Start();
        }

        public static void ConnectToPool()
        {
            //NHSmaData.UpdatePayingForAlgo(AlgorithmType.DaggerHashimoto3GB, 0.0d);
            //NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
            LingerOption lingerOption = new LingerOption(true, 0);
            while (checkConnection)
            {
                string[,] myServers = Form_Main.myServers;
                Random r = new Random();
                int r1 = r.Next(0, 5);
                //IPAddress addr = IPAddress.Parse(DNStoIP("daggerhashimoto." + myServers[r1, 0] + ".nicehash.com"));
                IPAddress addr = IPAddress.Parse(DNStoIP("daggerhashimoto." + myServers[0, 0] + ".nicehash.com"));
                IPAddress addrl = IPAddress.Parse("0.0.0.0");

                serverStream = null;
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
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
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Exception: " + ex);
                    }
                } else
                {
                    Helpers.ConsolePrint("DaggerHashimoto3GB", "Already connected");
                    ReadFromServer(serverStream, tcpClient);
                }
                                       
                if (!checkConnection)
                {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Disconnected. Stop connecting");
                        Thread.Sleep(1000);
                    break;
                } else
                {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Disconnected. Need reconnect");
                    checkConnection = true;
                        Thread.Sleep(1000);
                }

                Thread.Sleep(5 * 1000);
            }
            Helpers.ConsolePrint("DaggerHashimoto3GB", "Disconnected. End connection");
            
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
            waitReconnect = 300;

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
                                break;
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

                                        } else
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
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", tosend);
                                //break;
                            }

                            if (poolAnswer.Contains("client.reconnect"))
                            {
                                Helpers.ConsolePrint("DaggerHashimoto3GB", "Reconnect receive");
                                waitReconnect = 600;
                                Divert.Dagger3GBEpochCount = 999;
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
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Disconnected ex: " + ex.Message);
                        break;
                    }
                }
            }
        }
    }

}
