using HashLib;
using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Stats;
using NiceHashMiner.Switching;
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

/*
 * эпохи монет на 10.09.2023
 * ALT 60
 * AVS 62
 * BLACK 55
 * BTN 13
 * CAU 44
 * CLO 437
 * EGEM 461
 * ELH 28
 * ETHO 413
 * ETHW 595
 * EXP 272
 * OCTA 99
 * REDE 60
 * XPB 1 <<!!!
*/

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
        private static int _delay = 30;
        private static DateTime StartZILTime = new DateTime();
        private static DateTime timenow = new DateTime();
        private static double ZILsec;
        private static int ZILsecMax;

        public static void StartZilMonitor()
        {
            if (ConfigManager.GeneralConfig.ZIL_mining_state == 1)
            {
                ZILsecMax = 150;
            }
            if (ConfigManager.GeneralConfig.ZIL_mining_state == 2)
            {
                ZILsecMax = 105;
            }
            new Task(() => StartZilMonitorAPI()).Start();
            new Task(() => StartZilMonitorNiceHash()).Start();
        }
        public static void StartZilMonitorNiceHash()
        {
            if (Form_Main.ZilMonitorNicehashRunning) return;
            Form_Main.ZilMonitorNicehashRunning = true;
            try
            {
                while (true)//zil round monitor via NH connection
                {
                    if (!needConnectionZIL) break;
                    

                    if (tcpClient == null)
                    {
                        Helpers.ConsolePrint("ZILNiceHash", "Start monitor");
                        new Task(() => ConnectToPool()).Start();
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        continue;
                    }
                    if (tcpClient != null && !tcpClient.Connected)
                    {
                        if (needConnectionZIL)
                        {
                            Helpers.ConsolePrint("ZILNiceHash", "Reconnect wait: disabled" + waitReconnect.ToString() + " sec");
                            //Thread.Sleep(1000 * waitReconnect);
                            //new Task(() => ConnectToPool()).Start();
                        }
                    }
                    else
                    {
                        //Helpers.ConsolePrint("ZILNiceHash", "tcpClient.Connected");
                    }
                    Thread.Sleep(5000);
                }

                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("ZILNiceHash", ex.ToString());
            }

            Form_Main.ZilMonitorNicehashRunning = false;
            Helpers.ConsolePrint("ZILNiceHash", "Stop monitor");
        }

        public static void StartZilMonitorAPI()
        {
            if (Form_Main.ZilMonitorRunning) return;
            Form_Main.ZilMonitorRunning = true;
            Helpers.ConsolePrint("ZilAPI", "Start monitor");
            while (true)//zil round monitor via ZIL API
            {
                if (!needConnectionZIL) break;
                //https://github.com/Zilliqa/apidocs/blob/master/source/index.html.md
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
                        Helpers.ConsolePrint("ZilAPI", "ERROR: " + ((HttpWebResponse)response).StatusDescription);
                        continue;
                    }

                    dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    //Helpers.ConsolePrint("ZilAPI", responseFromServer);
                    dynamic resp = JsonConvert.DeserializeObject(responseFromServer);
                    if (resp != null)
                    {
                        string result = resp.result;
                        string _zil = result.Substring(result.Length - 2, 2);
                        int.TryParse(_zil, out int zil);
                        //Helpers.ConsolePrint("ZilAPI", "ZilBlock: " + zil.ToString());
                        Form_Main.ZilCount = zil;
                        if (zil > 0 & zil < 70)
                        {
                            _delay = 60 * 10;
                        }
                        if (zil >= 70 & zil < 90)
                        {
                            timenow = StartZILTime = DateTime.Now;
                            _delay = 60 * 2;
                        }
                        if (zil >= 90 & zil < 96)
                        {
                            timenow = StartZILTime = DateTime.Now;
                            ZILsec = 0;
                            _delay = 10;
                        }
                        if (zil == 97 || zil == 98)
                        {
                            _delay = 5;
                        }
                        if (zil == 99 || zil == 0)
                        {
                            _delay = 5;
                            NHSmaData.FinalizeSma();
                            if (!Form_Main.isZilRound)
                            {
                                if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                            }
                            MinersManager.MinerStatsCheck();
                        }
                        if (zil > 0 & zil < 99)
                        {
                            if (Form_Main.isZilRound)
                            {
                                NHSmaData.FinalizeSma();
                                if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;

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
                    Helpers.ConsolePrint("ZilAPI", ex.Message);
                }
                //sleep
                int InjectTickSleep = 0;
                do
                {
                    Thread.Sleep(1000);
                    InjectTickSleep++;
                } while (InjectTickSleep < _delay);
                InjectTickSleep = 0;
            }
            Form_Main.ZilMonitorRunning = false;
            Helpers.ConsolePrint("ZilAPI", "Stop monitor");
        }


        public static void ConnectToPool()
        {
            Helpers.ConsolePrint("ZILNiceHash", "Start connection");
            LingerOption lingerOption = new LingerOption(true, 0);
            //while (needConnectionZIL)
            {
                var serv = Links.CheckDNS("daggerhashimoto." +
                    Globals.MiningLocation[0], true).Replace("stratum+tcp://", "");
                IPAddress addr = IPAddress.Parse(serv);

                if (tcpClient != null)
                {
                    tcpClient.Close();
                    tcpClient = null;
                }

                int port = 3353;
                if (Globals.MiningLocation[0].ToLower().Contains("auto"))
                {
                    port = 9200;
                    serv = Links.CheckDNS("daggerhashimoto." + Globals.MiningLocation[0], true).Replace("stratum+tcp://", "");
                    addr = IPAddress.Parse(serv);
                } else
                {
                    port = 13353;
                    serv = Links.CheckDNS("daggerhashimoto." +
                    Globals.MiningLocation[0], true).Replace("stratum+tcp://", "");
                    addr = IPAddress.Parse(serv);
                }

                List<string> IPsList = new List<string>();
                var heserver = Dns.GetHostEntry(Globals.MiningLocation[0].Replace("auto.", ""));
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    IPsList.Add(curAdd.ToString());
                }
                foreach (var ip in IPsList)
                {
                    //NiceHashSocket.DropIPPort(Process.GetCurrentProcess().Id, ip, (uint)port);
                }

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
                        Helpers.ConsolePrint("ZILNiceHash", "Exception: " + ex);
                    }
                }
                else
                {
                    Helpers.ConsolePrint("ZILNiceHash", "Already connected");
                    ReadFromServer(serverStream, tcpClient);
                }

                if (!needConnectionZIL)
                {
                    Helpers.ConsolePrint("ZILNiceHash", "Disconnected. Stop connecting");
                    serverStream = null;
                    if (tcpClient != null)
                    {
                        tcpClient.Close();
                        tcpClient = null;
                    }
                    return;
                    //break;
                }
                else
                {
                    Helpers.ConsolePrint("ZILNiceHash", "Disconnected. Need reconnect");
                    if (tcpClient != null)
                    {
                        tcpClient.Client.Close();
                        tcpClient.Close();
                        tcpClient = null;
                    }
                    serverStream = null;
                    if (tcpClient != null)
                    {
                        tcpClient.Close();
                        tcpClient = null;
                    }
                    return;
                    //break;
                }
                Thread.Sleep(5 * 1000);
            }
            serverStream = null;
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
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
            if (serverStream == null)
            {
                Helpers.ConsolePrint("ZILNiceHash", "Error in serverStream");
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
                    if (tcpClient != null && tcpClient.Connected)
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
                            Helpers.ConsolePrint("ZILNiceHash", "clientZero");
                            break;
                        }

                        var poolData = Encoding.ASCII.GetString(messagePool);
                        var poolAnswer = poolData.Split((char)0)[0];
                        //Helpers.ConsolePrint("ZILNiceHash", "<- " + poolAnswer);

                        if (poolAnswer.Contains("mining.notify") && !poolAnswer.Contains("method"))
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
                                    string seedhash = json.@params[1];
                                    epoch = Epoch(seedhash);
                                    Helpers.ConsolePrint("ZILNiceHash", "Epoch = " + epoch.ToString() +
                                        " [ZilAPI] Block = " + Form_Main.ZilCount.ToString() + " ZILsec: " + ZILsec.ToString());

                                    if (!Form_Main.isZilRound && Form_Main.isForceZilRound)//rigel
                                    {
                                        Form_Main.ZilCount = 99;
                                        epoch = ConfigManager.GeneralConfig.ZILMaxEpoch;
                                        Helpers.ConsolePrint("ZIL", "Force ZIL round");
                                        ZILsec = 0;
                                        StartZILTime = DateTime.Now;
                                    }
                                    if (epoch == ConfigManager.GeneralConfig.ZILMaxEpoch &&
                                        (Form_Main.ZilCount >= 96 || Form_Main.ZilCount <= 0) &&
                                         ZILsec <= ZILsecMax)
                                    {
                                        timenow = DateTime.Now;
                                        if (!Form_Main.isZilRound)
                                        {
                                            Helpers.ConsolePrint("ZILNiceHash", "Start ZIL round?");
                                            StartZILTime = DateTime.Now;
                                            if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                                            MinersManager.MinerStatsCheck();
                                            Form_Main.isZilRound = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Form_Main.isZilRound)
                                        {
                                            epochCount++;
                                            //if ((epochCount >= 2 || ZILsec > ZILsecMax) && !Form_Main.isForceZilRound)
                                            if ((epoch != ConfigManager.GeneralConfig.ZILMaxEpoch || ZILsec > ZILsecMax) && !Form_Main.isForceZilRound)
                                            {
                                                Form_Main.isZilRound = false;
                                                Form_Main.isForceZilRound = false;
                                                epochCount = 0;
                                                Helpers.ConsolePrint("ZILNiceHash", "End ZIL round");
                                                if (double.IsNaN(Form_Main.ZilFactor)) Form_Main.ZilFactor = 0.0d;
                                                if (ConfigManager.GeneralConfig.RestartGMinerAfterZilRound)
                                                {
                                                    Form_Main.needGMinerRestart = true;
                                                }
                                            }
                                        }
                                    }
                                    ZILsec = Math.Round(timenow.Subtract(StartZILTime).TotalSeconds, 0);
                                }
                            }
                        }

                        if (poolAnswer.Contains("set_difficulty"))
                        {
                            //serverStream.Write(subscribeBytes, 0, subscribeBytes.Length);
                        }

                        if (poolAnswer.Contains("false"))
                        {
                            //Helpers.ConsolePrint("ZILNiceHash", tosend);
                            //break;
                        }

                        if (poolAnswer.Contains("client.reconnect"))
                        {
                            Helpers.ConsolePrint("ZILNiceHash", "Reconnect receive");
                            waitReconnect = 10;
                            tcpClient.Close();
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
                        Helpers.ConsolePrint("ZILNiceHash", "Disconnected");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("ZILNiceHash", "Disconnected ex: " + ex.Message);
                    break;
                }
            }
        }
    }
}
