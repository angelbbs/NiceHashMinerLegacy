using HashLib;
using Newtonsoft.Json;
using NiceHashMiner.Switching;
using NiceHashMinerLegacy.Common.Enums;
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
        internal static bool checkConnection = false;
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
                    Thread.Sleep(1000 * 10);
                    new Task(() => ConnectToPool()).Start();
                    Thread.Sleep(1000 * 10);
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
                        tcpClient.Close();
                        tcpClient.Dispose();
                    }
                    Thread.Sleep(1000 * 10);
                    Helpers.ConsolePrint("DaggerHashimoto3GB", "Reconnect");
                    new Task(() => ConnectToPool()).Start();
                    Thread.Sleep(1000 * 10);
                }
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
            checkConnection = false;
            if (tcpClient != null)
            {
                tcpClient.Close();
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
                serverStream.ReadTimeout = 1000 * 120;
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
            bool needUpdate = false;
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
            //Helpers.ConsolePrint("DaggerHashimoto3GB", "-> " + Encoding.ASCII.GetString(subscribeBytes));
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
                //Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle1");
                int serverBytes;
                //if (serverStream.CanRead)
                {
                    try
                    {
                  //      Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle2");
                        if (tcpClient.Connected)
                        {
                    //        Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle3");
                            for (int i = 0; i < 1024; i++)
                            {
                                messagePool[i] = 0;
                            }

                            serverBytes = serverStream.Read(messagePool, 0, 8192);
                         //   Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle4");
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
                      //      Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle5");
                            if (clientZero)
                            {
                                //   continue;
                                Helpers.ConsolePrint("DaggerHashimoto3GB", "clientZero");
                            }
                      //      Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle6");
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
                      //      Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle7");
                            var poolAnswer = poolData.Split((char)0)[0];
                            //Helpers.ConsolePrint("DaggerHashimoto3GB", "<- " + poolAnswer);

                            if (poolAnswer.Contains("mining.notify") && !poolAnswer.Contains("method"))
                            {
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", "-> " + Encoding.ASCII.GetString(authorizeBytes));
                                serverStream.Write(authorizeBytes, 0, authorizeBytes.Length);
                            }
                       //     Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle8");
                            if (poolAnswer.Contains("mining.notify") && poolAnswer.Contains("method"))//job
                            {
                                int amount = poolAnswer.Split(new char[] { (char)10 }, StringSplitOptions.None).Count() - 1;
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", amount.ToString());
                                for (var i = 0; i < amount; i++)
                                {
                                    if (poolAnswer.Split((char)10)[i].Contains("mining.notify"))
                                    {
                                        dynamic json = JsonConvert.DeserializeObject(poolAnswer.Split((char)10)[i]);
                                        string seedhash = json.@params[1];
                                        //Helpers.ConsolePrint("DaggerHashimoto3GB", "seedhash = " + seedhash);
                                        epoch = Epoch(seedhash);
                                        //Helpers.ConsolePrint("DaggerHashimoto3GB", "Epoch = " + epoch.ToString());
                                        bool previousEpoch = Epoch3GB;
                                        if (epoch < 235) //win 7
                                        {
                                            Epoch3GB = true;
                                        } else
                                        {
                                            Epoch3GB = false;
                                        }
                                        if (previousEpoch != Epoch3GB) needUpdate = true;
                                    }
                                }
                                if (needUpdate && Epoch3GB)
                                {
                                    Helpers.ConsolePrint("DaggerHashimoto3GB", "Force switch ON. Epoch: " + epoch.ToString());
                                    Form_Main.DaggerHashimoto3GBProfit = true;
                                    NHSmaData.TryGetPaying(AlgorithmType.DaggerHashimoto, out var paying);
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.DaggerHashimoto3GB, paying);
                                    //NiceHashMiner.Switching.AlgorithmSwitchingManager._smaCheckTimer.Stop();
                                    //NiceHashMiner.Switching.AlgorithmSwitchingManager._smaCheckTimer.Start();
                                    NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
                                    needUpdate = false;
                                }
                                if (needUpdate && !Epoch3GB)
                                {
                                    Helpers.ConsolePrint("DaggerHashimoto3GB", "Force switch OFF. Epoch: " + epoch.ToString());
                                    Form_Main.DaggerHashimoto3GBProfit = false;
                                    NHSmaData.UpdatePayingForAlgo(AlgorithmType.DaggerHashimoto3GB, 0.0d);
                                    //NiceHashMiner.Switching.AlgorithmSwitchingManager._smaCheckTimer.Stop();
                                    //NiceHashMiner.Switching.AlgorithmSwitchingManager._smaCheckTimer.Start();
                                    NiceHashMiner.Switching.AlgorithmSwitchingManager.SmaCheckNow();
                                    needUpdate = false;
                                }
                            }
                     //       Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle9");
                            if (poolAnswer.Contains("set_difficulty"))
                            {
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", "-> " + Encoding.ASCII.GetString(subscribeBytes));
                                serverStream.Write(subscribeBytes, 0, subscribeBytes.Length);
                               // Helpers.ConsolePrint("DaggerHashimoto3GB", "-> " + Encoding.ASCII.GetString(noopBytes));
                               // serverStream.Write(noopBytes, 0, noopBytes.Length);
                            }
                     //       Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle10");
                          //  Helpers.ConsolePrint("DaggerHashimoto3GB", poolAnswer);
                            if (poolAnswer.Contains("false"))
                            {
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", tosend);
                                //break;
                            }
                       //     Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle11");
                            if (poolAnswer.Contains("reconnect"))
                            {
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", tosend);
                                break;
                            }
                      //      Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle12");
                            if (poolAnswer.Contains("Invalid JSON request"))
                            {
                                //Helpers.ConsolePrint("DaggerHashimoto3GB", tosend);
                                break;
                            }
                      //      Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle13");
                            byte[] bytes = Encoding.ASCII.GetBytes(poolAnswer);
                            //serverStream.Write(bytes, 0, bytes.Length);
                            bytes = null;
                      //      Helpers.ConsolePrint("DaggerHashimoto3GB", "cycle14");
                        } else
                        {
                            Helpers.ConsolePrint("DaggerHashimoto3GB", "Disconnected");
                            if (tcpClient != null)
                            {
                                tcpClient.Close();
                            }
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("DaggerHashimoto3GB", "Disconnected (Exception)");
                        if (tcpClient != null)
                        {
                            tcpClient.Close();
                        }
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
