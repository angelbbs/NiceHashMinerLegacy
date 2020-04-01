using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Miners
{
    class DHClient
    {
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
        public static void ConnectToPool()
        {
            Helpers.ConsolePrint("DaggerHashimoto3GB", "1");
            //IPEndPoint local = new IPEndPoint(IPAddress.Parse(SourceHost), SourcePort);
            // IPEndPoint remote = new IPEndPoint(IPAddress.Parse(DNStoIP("daggerhashimoto.eu.nicehash.com")), 3353);
            //            Proxy.Process(
            //                  new IPEndPoint(IPAddress.Parse(SourceHost), SourcePort),
            //                new IPEndPoint(IPAddress.Parse(Util.DNStoIP(DestinationHost)), DestinationPort), TLS, TLSout, block, SavePackets, Algorithm, Name);

            TcpClient tcpClient = new TcpClient();
            Helpers.ConsolePrint("DaggerHashimoto3GB", "2");
            tcpClient.Connect(IPAddress.Parse(DNStoIP("daggerhashimoto.eu.nicehash.com")), 3353);
            Helpers.ConsolePrint("DaggerHashimoto3GB", "3");
            NetworkStream serverStream = tcpClient.GetStream();
            Helpers.ConsolePrint("DaggerHashimoto3GB", "4");
            ReadFromServer(serverStream);
        }
        public static void ReadFromServer(Stream serverStream) //от пула
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

            byte[] messagePool = new byte[8192];
            int np = 0;
            int poolBytes;
            //login
            /*
             2020.03.20:15:57:23.437: eths Eth: Send: {"id":1,"method":"mining.subscribe","params":["PhoenixMiner/4.7c","EthereumStratum/1.0.0"]}

2020.03.20:15:57:23.579: eths Eth: Received: {"id":1,"error":null,"result":[["mining.notify","93b163f677519c2f6a6af427252e5c0a","EthereumStratum/1.0.0"],"24bf84"]}
2020.03.20:15:57:23.579: eths Eth: Extranonce set to 24bf84
2020.03.20:15:57:23.580: eths Eth: Subscribed to ethash pool
2020.03.20:15:57:23.580: eths Eth: Send: {"id":2,"method":"mining.extranonce.subscribe","params":[]}
{"id":3,"method":"mining.authorize","params":["3F2v4K3ExF1tqLLwa6Ac3meimSjV3iUZgQ.Farm1$0-2t3LAymH0Ve-dEwJZ-UEcw","x"]}

2020.03.20:15:57:23.685: eths Eth: Received: {"id":2,"result":true,"error":null}
2020.03.20:15:57:23.685: eths Eth: Received: {"id":3,"result":true,"error":null}
2020.03.20:15:57:23.685: eths Eth: Worker 3F2v4K3ExF1tqLLwa6Ac3meimSjV3iUZgQ.Farm1$0-2t3LAymH0Ve-dEwJZ-UEcw authorized
2020.03.20:15:57:23.685: eths Eth: Received: {"id":null,"method":"mining.set_difficulty","params":[2.0]}
2020.03.20:15:57:23.685: eths Eth: Difficulty set to 2
2020.03.20:15:57:23.685: eths Eth: Received: {"id":null,"method":"mining.notify","params":["0000000024cd7f4c","5c6559aac200058e1e380fd8aa8d5c162dde7d916bd59e38cb015dc7a4b8d914","0554cc3a94a891ef10631772d641a78f4770d043687173eced23510f1821134a",true]}
2020.03.20:15:57:23.685: eths Eth: New job #0554cc3a from daggerhashimoto.eu.nicehash.com:3353; diff: 8590MH
2020.03.20:15:57:23.686: GPU1 GPU1: Starting up... (0)
2020.03.20:15:57:23.686: GPU1 GPU1: Generating ethash light cache for epoch #334

             */
            string subscribe = "{\"id\":1,\"method\":\"mining.subscribe\",\"params\":[\"PhoenixMiner/4.7c\",\"EthereumStratum/1.0.0\"]}" + (char)10;
            byte[] bytes1 = Encoding.ASCII.GetBytes(subscribe);
            serverStream.Write(bytes1, 0, bytes1.Length);

            for (int i = 0; i < 1024; i++)
            {
                messagePool[i] = 0;
            }
           // poolBytes = serverStream.Read(messagePool, 0, 8192);
           // var sub1 = Encoding.ASCII.GetString(messagePool);
            /*
            if (!s1.Contains("jsonrpc"))
            {
                break;
            }
            */
           // var sub2 = sub1.Split((char)0)[0];
            //Helpers.ConsolePrint("DaggerHashimoto3GB", sub2);
            Helpers.ConsolePrint("DaggerHashimoto3GB", "5");

            while (true)
            {
                int serverBytes;
                try
                {
                    for (int i = 0; i < 1024; i++)
                    {
                        messagePool[i] = 0;
                    }
                    serverBytes = serverStream.Read(messagePool, 0, 8192);
                    //от пула все нули
                    bool clientZero = true;
                    for (int i = 0; i < 2048; i++)
                    {
                        if (messagePool[i] != (char)0)
                        {
                            clientZero = false;
                        }
                    }
                    if (clientZero) continue;

                    //if (SavePackets)
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
                    // jsonrpc
                    var s1 = Encoding.ASCII.GetString(messagePool);
                    /*
					if (!s1.Contains("jsonrpc"))
					{
						break;
					}
					*/
                    var tosend = s1.Split((char)0)[0];
                    Helpers.ConsolePrint("DaggerHashimoto3GB", tosend);
                    if (tosend.Contains("Invalid JSON request"))
                    {
                        //clientStream.Close();
                        //ClientsCount--;
                        break;
                    }

                    byte[] bytes = Encoding.ASCII.GetBytes(tosend);
                    //serverStream.Write(bytes, 0, bytes.Length);
                    bytes = null;
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("DaggerHashimoto3GB", ex.Message);
                    break;
                }
                if (serverBytes == 0)
                {
                    Helpers.ConsolePrint("DaggerHashimoto3GB", "serverBytes == 0");
                    //break;
                }
            }
        }
    }
}
