using HashLib;
using Newtonsoft.Json;
using NiceHashMinerLegacy.Divert;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Utils
{
    public class ServerResponceTime
    {
        internal static TcpClient tcpClient = null;
        public static NetworkStream serverStream = null;
        public static int nServer = 0;

        public static DateTime StartTime = new DateTime();
        public static TimeSpan AnswerTime;
        public static int GetBestServer()
        {
            string[,] myServers = Form_Main.myServers;

            for (int s = 0; s < 4; s++)
            {
                var ReplyTime = ConnectToServer(s);
                myServers[s, 1] = ReplyTime.ToString();
            }

            string[,] tmpServers = { { "eu", "20000" }, { "eu-north", "20001" }, { "usa", "20002" }, { "usa-east", "20003" } };
            int ReplyTimeTmp;
            long bestReplyTimeTmp = 19999;
            int iTmp = 0;

            for (int k = 0; k < 4; k++)
            {
                for (int i = 0; i < 4; i++)
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

            Form_Main.myServers = tmpServers;
            for (int i = 0; i < 4; i++)
            {

                Helpers.ConsolePrint("SortedServers", Form_Main.myServers[i, 0] + " " + Form_Main.myServers[i, 1]);
            }
            Helpers.ConsolePrint("BestServer", Form_Main.myServers[0, 0]);
            return 0;
        }

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

        public static int ConnectToServer(int s)
        {
            nServer = s;
            int ms = 0;
            LingerOption lingerOption = new LingerOption(true, 0);
            string[,] myServers = Form_Main.myServers;
            IPAddress addr = IPAddress.Parse("0.0.0.0");
            IPAddress addrl = IPAddress.Parse("0.0.0.0");
            try
            {
                addr = IPAddress.Parse(DNStoIP("daggerhashimoto." + myServers[nServer, 0] + ".nicehash.com"));
                addrl = IPAddress.Parse("0.0.0.0");
            } catch (Exception ex)
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
                    StartTime = DateTime.Now;
                    using (TcpClient tcpClient = new TcpClient() { SendTimeout = 2000, ReceiveTimeout = 2000, LingerState = lingerOption })
                    {
                        tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                        tcpClient.ConnectAsync(addr, 3353);

                        while (!tcpClient.Connected)
                        {
                            Thread.Sleep(1);
                        }
                        using (serverStream = tcpClient.GetStream())
                        {
                            serverStream.ReadTimeout = 1000 * 2;
                            ms = ReadFromServer(serverStream, tcpClient);
                        }
                        tcpClient.Close();
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("ConnectToServer", "Exception: " + ex);
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
            Helpers.ConsolePrint(myServers[nServer, 0] + ".nicehash.com", ms.ToString() + " ms");
            return ms;
        }


    }
}
