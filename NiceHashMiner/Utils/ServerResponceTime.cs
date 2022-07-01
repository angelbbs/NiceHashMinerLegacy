using NiceHashMiner.Configs;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NiceHashMiner.Utils
{
    public class ServerResponceTime
    {
        internal static TcpClient tcpClient = null;
        public static NetworkStream serverStream = null;
        public static int nServer = 0;

        public static DateTime StartTime = new DateTime();
        public static TimeSpan AnswerTime;
        /*
        public static int GetBestServer()
        {
            //string[,] myServers = Form_Main.myServers;
            string[,] myServers = { { "auto.nicehash.com", "20004" } };
            int count = Globals.MiningLocation.Length;
            for (int s = 0; s < count; s++)
            {
                int ReplyTime = ConnectToServer(s);
                myServers[s, 1] = ReplyTime.ToString();

            myServers[ConfigManager.GeneralConfig.ServiceLocation, 1] = "0";
            
            string[,] tmpServers = { { "auto.nicehash.com", "20004" } };
            int ReplyTimeTmp;
            long bestReplyTimeTmp = 19999;
            int iTmp = 0;

            for (int k = 0; k < count; k++)
            {
                for (int i = 0; i < count; i++)
                {
                    ReplyTimeTmp = Convert.ToInt32(myServers[i, 1]);
                    if (ReplyTimeTmp < bestReplyTimeTmp && ReplyTimeTmp != -1)
                    {
                        iTmp = i;
                        bestReplyTimeTmp = ReplyTimeTmp;
                    }

                }
                tmpServers[k, 0] = myServers[iTmp, 0];
                tmpServers[k, 1] = myServers[iTmp, 1];
                myServers[iTmp, 1] = "-1";
                bestReplyTimeTmp = 10000;
            }

            string pr = "| ";
            Form_Main.myServers = tmpServers;
            for (int i = 0; i < count; i++)
            {
                pr += Form_Main.myServers[i, 0] + "=" + (Form_Main.myServers[i, 1].Equals("0") ? "forced | ": Form_Main.myServers[i, 1] + " ms | ");
            }
            Helpers.ConsolePrint("SortedServers", pr);
            return 0;
        }
        */
        public static string DNStoIP(string IPName)
        {
            string ret = Links.CheckDNS("stratum+tcp://" + IPName, true).Replace("stratum+tcp://", "");
            return ret;
            /*
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
            catch (Exception)
            {
                //Console.WriteLine("Exception: " + e.ToString());
            }
            return "";
            */
        }
        /*
        public static int ConnectToServer(int s)
        {
            nServer = s;
            int ms = 0;
            LingerOption lingerOption = new LingerOption(true, 0);
            string[,] myServers = Form_Main.myServers;
            IPAddress addr = IPAddress.Parse("0.0.0.0");
            IPAddress addrl = IPAddress.Parse("0.0.0.0");
            string adr = "";
            try
            {
                adr = Links.CheckDNS("stratum+tcp://daggerhashimoto." + myServers[nServer, 0], true).Replace("stratum+tcp://", "");
                addr = IPAddress.Parse(adr);
                addrl = IPAddress.Parse("0.0.0.0");
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("ConnectToServer", ex.ToString());
            }

            serverStream = null;
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient.Dispose();
                tcpClient = null;
            }

            if (tcpClient == null)
            {
                try
                {
                    //StartTime = DateTime.Now;
                    using (TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork & AddressFamily.InterNetworkV6) {SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = lingerOption })
                    {
                        tcpClient.SendTimeout = 1000;
                        tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                        if (myServers[nServer, 0].Contains("auto"))
                        {
                            tcpClient.ConnectAsync(addr, 9200).Wait(1000);
                        }
                        else
                        {
                            tcpClient.ConnectAsync(addr, 3353).Wait(1000);
                        }

                        while (!tcpClient.Connected)
                        {
                            Thread.Sleep(1);
                        }
                        using (serverStream = tcpClient.GetStream())
                        {
                            serverStream.ReadTimeout = 1000 * 2;
                            StartTime = DateTime.Now;
                            ms = ReadFromServer(serverStream, tcpClient);
                        }
                        tcpClient.Close();
                    }
                }
                catch (SocketException ex)
                {
                    Helpers.ConsolePrint("ConnectToServer", "Server: " + myServers[nServer, 0] + " Error code: " + ex.ErrorCode.ToString());
                    ms = 1000;
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("ConnectToServer", "Server: " + myServers[nServer, 0] + " Exception: " + ex);
                    ms = 1000;
                }
            }
            else
            {
                Helpers.ConsolePrint("ConnectToServer", "Already connected");
                ms = 1000;
                //ReadFromServer(serverStream, tcpClient);
            }

            return ms;
        }
        */
        /*
        public static int ReadFromServer(Stream serverStream, TcpClient tcpClient)
        {
            string[,] myServers = Form_Main.myServers;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

            byte[] messagePool = new byte[8192];
            int ms = 0;

            string subscribe = "{\"id\": 1, \"method\": \"mining.subscribe\", \"params\": [\"EthereumMiner/1.0.0\", \"EthereumStratum/1.0.0\"]}" + (char)10;
            byte[] subscribeBytes = Encoding.ASCII.GetBytes(subscribe);


            if (serverStream == null)
            {
                Helpers.ConsolePrint("ReadFromServer", "Error in serverStream");
                return 1000;
            }

            serverStream.Write(subscribeBytes, 0, subscribeBytes.Length);

            for (int i = 0; i < 1024; i++)
            {
                messagePool[i] = 0;
            }

            while (true)
            {
                Thread.Sleep(1);
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
                            Helpers.ConsolePrint("ReadFromServer", "clientZero");
                            ms = 1000;
                            break;
                        }

                        var poolData = Encoding.ASCII.GetString(messagePool);

                        var poolAnswer = poolData.Split((char)0)[0];
                        var timenow = DateTime.Now;
                        AnswerTime = timenow.Subtract(StartTime);

                        if (poolAnswer.Contains("mining.notify") && !poolAnswer.Contains("method"))
                        {
                            ms = AnswerTime.Milliseconds;
                            break;
                        }

                        if (poolAnswer.Contains("false"))
                        {
                            Helpers.ConsolePrint("ReadFromServer", "Server return - false");
                            ms = 1000;
                            break;
                        }

                        if (poolAnswer.Contains("Invalid JSON request"))
                        {
                            ms = AnswerTime.Milliseconds;
                            break;
                        }

                        byte[] bytes = Encoding.ASCII.GetBytes(poolAnswer);
                        bytes = null;

                    }
                    else
                    {
                        Helpers.ConsolePrint("ReadFromServer", "Disconnected");
                        ms = 1000;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("ReadFromServer", "Disconnected ex: " + ex.Message);
                    ms = 1000;
                    break;
                }

            }

            return ms;
        }
        */

    }
}
