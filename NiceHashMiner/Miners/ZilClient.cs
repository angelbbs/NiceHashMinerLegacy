using HashLib;
using Newtonsoft.Json;
using NiceHashMiner.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace NiceHashMiner.Miners
{
    class ZilClient
    {
        internal static TcpClient tcpClient = null;
        public static NetworkStream serverStream = null;
        private static List<TcpClient> tcpClientList = new List<TcpClient>();

        public static bool needStart = false;
        private static int waitReconnect = 10;
        private static int epochCount = 0;
        public static bool needConnectionZIL = true;
        private static int _delay = 10;

        public static void StartZilMonitor()
        {
            while (true)//zil round monitor via NH connection
            {
                if (!needConnectionZIL) break;
                Form_Main.ZilMonitorRunning = true;

                try
                {
                    WebRequest request = WebRequest.Create("https://api.zilliqa.com/");
                    request.Method = "POST";
                    string postData = "{\r\n" +
                                      "\"id\": \"1\",\r\n" +
                                      "\"jsonrpc\": \"2.0\",\r\n" +
                                      "\"method\": \"GetNumTxBlocks\",\r\n" +
                                      "\"params\": [\"\"]\r\n" +
                                      "}\r\n";
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    request.ContentType = "application/json";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                    WebResponse response = request.GetResponse();
                    if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                    {
                        Helpers.ConsolePrint("ZilMonitor", "ERROR: " + ((HttpWebResponse)response).StatusDescription);
                        continue;
                    }

                    dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    //Helpers.ConsolePrint("ZilMonitor", responseFromServer);
                    dynamic resp = JsonConvert.DeserializeObject(responseFromServer);
                    if (resp != null)
                    {
                        string result = resp.result;
                        //Helpers.ConsolePrint("ZilMonitor", result);
                        string _zil = result.Substring(result.Length - 2, 2);
                        int.TryParse(_zil, out int zil);
                        //Helpers.ConsolePrint("ZilMonitor", zil.ToString());
                        Form_Main.ZilCount = zil;
                        if (zil > 0 & zil < 70)
                        {
                            _delay = 60 * 10;
                            Helpers.ConsolePrint("ZilMonitor", "ZilCount: " + zil.ToString());
                        }
                        if (zil >= 70 & zil < 95)
                        {
                            _delay = 60 * 1;
                            Helpers.ConsolePrint("ZilMonitor", "ZilCount: " + zil.ToString());
                        }
                        if (zil >= 95 & zil < 98)
                        {
                            _delay = 30;
                            Helpers.ConsolePrint("ZilMonitor", "ZilCount: " + zil.ToString());
                        }
                        if (zil >= 98 & zil <= 99)
                        {
                            _delay = 5;
                            Helpers.ConsolePrint("ZilMonitor", "ZilCount: " + zil.ToString());
                        }
                        if (zil == 99)
                        {
                            _delay = 15;
                            Helpers.ConsolePrint("ZilMonitor", "ZilCount: " + zil.ToString());
                            if (!Form_Main.isZilRound)
                            {
                                Helpers.ConsolePrint("ZilMonitor", "Start ZIL round");
                                new Task(() => Stats.NiceHashStats.GetSmaAPICurrent()).Start();
                                if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                                ConfigManager.GeneralConfig.ZilFactor = Form_Main.ZilFactor;
                            }
                            new Task(() => Stats.NiceHashStats.GetSmaAPICurrent()).Start();
                            MinersManager.MinerStatsCheck();
                            Form_Main.isZilRound = true;
                        }
                        if (zil == 0)
                        {
                            _delay = 15;
                            Helpers.ConsolePrint("ZilMonitor", "ZilCount: " + zil.ToString());
                            if (!Form_Main.isZilRound)
                            {
                                Helpers.ConsolePrint("ZilMonitor", "Start ZIL round");
                                new Task(() => Stats.NiceHashStats.GetSmaAPICurrent()).Start();
                                if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                                ConfigManager.GeneralConfig.ZilFactor = Form_Main.ZilFactor;
                            }
                            new Task(() => Stats.NiceHashStats.GetSmaAPICurrent()).Start();
                            MinersManager.MinerStatsCheck();
                            Form_Main.isZilRound = true;
                        }
                        if (zil > 0 & zil < 99)
                        {
                            if (Form_Main.isZilRound)
                            {
                                Form_Main.isZilRound = false;
                                Helpers.ConsolePrint("ZilMonitor", "End ZIL round");
                                new Task(() => Stats.NiceHashStats.GetSmaAPICurrent()).Start();
                                if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                                ConfigManager.GeneralConfig.ZilFactor = Form_Main.ZilFactor;
                                if (ConfigManager.GeneralConfig.RestartGMinerAfterZilRound)
                                {
                                    Form_Main.needGMinerRestart = true;
                                }
                            }
                        }
                    }

                    reader.Close();
                    dataStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("ZilMonitor", ex.ToString());
                }



                Thread.Sleep(1000 * _delay);
            }

                /*
                while (true)//zil round monitor via NH connection
                {
                    if (!needConnectionZIL) break;
                    Form_Main.ZilMonitorRunning = true;

                    if (tcpClient == null)
                    {
                        Thread.Sleep(5000);
                        Helpers.ConsolePrint("ZIL", "Start monitor");
                        new Task(() => ConnectToPool()).Start();
                    }
                    else
                    {

                    }
                    if (tcpClient != null && !tcpClient.Connected)
                    {
                        if (needConnectionZIL)
                        {
                            Helpers.ConsolePrint("ZIL", "Reconnect wait: " + waitReconnect.ToString() + " sec");
                            Thread.Sleep(1000 * waitReconnect);
                            new Task(() => ConnectToPool()).Start();
                        }
                    }
                    else
                    {
                        //Helpers.ConsolePrint("ZIL", "tcpClient.Connected");
                    }
                    Thread.Sleep(1000);
                }

                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                    tcpClient = null;
                }
                */


                Form_Main.ZilMonitorRunning = false;
            Helpers.ConsolePrint("ZIL", "Stop monitor");
        }


        public static void ConnectToPool()
        {
            Helpers.ConsolePrint("ZIL", "Start connection");
            LingerOption lingerOption = new LingerOption(true, 0);
            while (needConnectionZIL)
            {
                /*
                int _location = ConfigManager.GeneralConfig.ServiceLocation;
                if (ConfigManager.GeneralConfig.ServiceLocation >= Globals.MiningLocation.Length)
                {
                    _location = ConfigManager.GeneralConfig.ServiceLocation - 1;
                }
                */
                var serv = Links.CheckDNS("etchash." + 
                    Globals.MiningLocation[0], true).Replace("stratum+tcp://", "");
                IPAddress addr = IPAddress.Parse(serv);
                IPAddress addrl = IPAddress.Parse("0.0.0.0");

                Reconnect:
                //serverStream = null;
                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient.Dispose();
                    tcpClient = null;
                }

                int port = 3393;
                if (Globals.MiningLocation[0].ToLower().Contains("auto"))
                {
                    port = 9200;
                } else
                {
                    port = 13393;
                }
                var iep = new IPEndPoint(addrl, port);

                if (tcpClient == null)
                {
                    try
                    {
                        using (tcpClient = new TcpClient() { SendTimeout = 10000, ReceiveTimeout = 10000, LingerState = lingerOption })
                        {
                            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                            tcpClient.ConnectAsync(addr, port);

                            //Thread.Sleep(1000 * 5);
                            while (!tcpClient.Connected)
                            {
                                Thread.Sleep(1000);
                            }
                            using (serverStream = tcpClient.GetStream())
                            {
                                serverStream.ReadTimeout = 1000 * 120;
                                ReadFromServer(serverStream, tcpClient);
                            }
                            if (tcpClient != null)
                            {
                                tcpClient.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("ZIL", "Exception: " + ex);
                    }
                }
                else
                {
                    Helpers.ConsolePrint("ZIL", "Already connected");
                    ReadFromServer(serverStream, tcpClient);
                }

                if (!needConnectionZIL)
                {
                    Helpers.ConsolePrint("ZIL", "Disconnected. Stop connecting");
                    break;
                }
                else
                {
                    Helpers.ConsolePrint("ZIL", "Disconnected. Need reconnect");
                    //Thread.Sleep(5000);
                    //goto Reconnect;
                    //Thread.Sleep(1000);
                    break;
                }
                Thread.Sleep(5 * 1000);
            }
            serverStream = null;
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient.Dispose();
                tcpClient = null;
            }
            //Helpers.ConsolePrint("ZIL", "Disconnected. End connection");
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
            bool EpochZIL = false;

            byte[] messagePool = new byte[8192];

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
            waitReconnect = 10;
            int GoodEpochCount = 0;

            if (serverStream == null)
            {
                Helpers.ConsolePrint("ZIL", "Error in serverStream");
                return;
            }
            serverStream.Write(subscribeBytes, 0, subscribeBytes.Length);

            for (int i = 0; i < 1024; i++)
            {
                messagePool[i] = 0;
            }

            while (needConnectionZIL)
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
                            Helpers.ConsolePrint("ZIL", "clientZero");
                            break;
                        }

                        // jsonrpc
                        var poolData = Encoding.ASCII.GetString(messagePool);

                        var poolAnswer = poolData.Split((char)0)[0];
                        //Helpers.ConsolePrint("ZIL", "<- " + poolAnswer);

                        if (poolAnswer.Contains("mining.notify") && !poolAnswer.Contains("method"))
                        {
                            serverStream.Write(authorizeBytes, 0, authorizeBytes.Length);
                        }

                        if (poolAnswer.Contains("mining.notify") && poolAnswer.Contains("method"))//job
                        {
                            poolAnswer = poolAnswer.Replace("}{", "}" + (char)10 + "{");
                            int amount = poolAnswer.Split(new char[] { (char)10 }, StringSplitOptions.None).Count() - 1;
                            //Helpers.ConsolePrint("ZIL", amount.ToString());
                            for (var i = 0; i <= amount; i++)
                            {
                                if (poolAnswer.Split((char)10)[i].Contains("mining.notify"))
                                {
                                    dynamic json = JsonConvert.DeserializeObject(poolAnswer.Split((char)10)[i]);
                                    string seedhash = json.@params[1];
                                    epoch = Epoch(seedhash);
                                    //Helpers.ConsolePrint("ZIL", "Epoch = " + epoch.ToString());
                                    bool previousEpoch = EpochZIL;
                                    if (epoch <= ConfigManager.GeneralConfig.ZILMaxEpoch)
                                    {
                                        if (!Form_Main.isZilRound)
                                        {
                                            Helpers.ConsolePrint("ZIL", "Start ZIL round");
                                            new Task(() => Stats.NiceHashStats.GetSmaAPICurrent()).Start();
                                            if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                                            ConfigManager.GeneralConfig.ZilFactor = Form_Main.ZilFactor;
                                        }
                                        new Task(() => Stats.NiceHashStats.GetSmaAPICurrent()).Start();
                                        MinersManager.MinerStatsCheck();
                                        Form_Main.isZilRound = true;
                                    }
                                    else
                                    {
                                        if (Form_Main.isZilRound)
                                        {
                                            epochCount++;
                                            if (epochCount >= 2)
                                            {
                                                Form_Main.isZilRound = false;
                                                epochCount = 0;
                                                Helpers.ConsolePrint("ZIL", "End ZIL round");
                                                new Task(() => Stats.NiceHashStats.GetSmaAPICurrent()).Start();
                                                if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                                                ConfigManager.GeneralConfig.ZilFactor = Form_Main.ZilFactor;
                                                if (ConfigManager.GeneralConfig.RestartGMinerAfterZilRound)
                                                {
                                                    Form_Main.needGMinerRestart = true;
                                                }
                                            }
                                        }
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
                            //Helpers.ConsolePrint("ZIL", tosend);
                            //break;
                        }

                        if (poolAnswer.Contains("client.reconnect"))
                        {
                            Helpers.ConsolePrint("ZIL", "Reconnect receive");
                            waitReconnect = 10;
                            tcpClient.Close();
                            tcpClient.Dispose();
                            tcpClient = null;
                        }

                        if (poolAnswer.Contains("Invalid JSON request"))
                        {
                            break;
                        }

                        byte[] bytes = Encoding.ASCII.GetBytes(poolAnswer);
                        bytes = null;

                    }
                    else
                    {
                        Helpers.ConsolePrint("ZIL", "Disconnected");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("ZIL", "Disconnected ex: " + ex.Message);
                    break;
                }
            }
        }
    }

}
