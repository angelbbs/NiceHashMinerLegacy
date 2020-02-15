/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/

using System;
using WinDivertSharp;
using WinDivertSharp.WinAPI;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using PacketDotNet;
using System.Diagnostics;
using System.Security.Principal;
using System.Management;
using System.Runtime.ExceptionServices;

namespace NiceHashMinerLegacy.Divert
{
    public class DEthash
    {
       // private static Timer _divertTimer;
        private static volatile bool divert_running = true;
        private static IntPtr DivertHandle;
        private static readonly uint MaxPacket = 2048;

        private static string DevFeeIP = "";
        private static ushort DevFeePort = 0;

        private static string DivertIP = "";
        private static string DivertIPName = "";
        private static ushort DivertPort = 0;

        private static string DivertIP1 = "";
        private static string DivertIPName1 = "";
        private static ushort DivertPort1 = 0;

        private static string DivertIP2 = "";
        private static string DivertIPName2 = "";
        private static ushort DivertPort2 = 0;

        private static string DivertIP3 = "";
        private static string DivertIPName3 = "";
        private static ushort DivertPort3 = 0;

        private static string DivertIP4 = "";
        private static string DivertIPName4 = "";
        private static ushort DivertPort4 = 0;

        private static string DivertIP5 = "";
        private static string DivertIPName5 = "";
        private static ushort DivertPort5 = 0;

        private static string DivertIP6 = "";
        private static string DivertIPName6 = "";
        private static ushort DivertPort6 = 0;

        private static string filter = "";

        private static string DivertLogin = "";
        private static string DivertLogin1 = "";
        private static string DivertLogin2 = "";

        private static string PacketPayloadData;
        private static string packetdata;
        
        internal static bool CheckParityConnections(List<uint> processIdList, ushort Port, WinDivertDirection dir)
        {
            bool ret = false;
            Port = Divert.SwapOrder(Port);
            List<Connection> _allConnections = new List<Connection>();

            _allConnections.Clear();
            _allConnections.AddRange(NetworkInformation.GetTcpV4Connections());

            for (int i = 1; i < _allConnections.Count; i++)
            {
                if (processIdList.Contains(_allConnections[i].OwningPid) &&
                    (_allConnections[i].LocalEndPoint.Port == Port) ||
                    _allConnections[i].RemoteEndPoint.Port == Port)
                {
                    /*
                    if (processIdList.Contains(_allConnections[i].OwningPid))
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "CheckParityConnections: " +
                        _allConnections[i].OwningPid.ToString() + " " +
                        _allConnections[i].LocalEndPoint.ToString() + " " +
                        _allConnections[i].RemoteEndPoint.ToString());
                    }
                    */
                    ret = true;
                }
            }
            _allConnections = null;
            /*
            Helpers.ConsolePrint("WinDivertSharp", " processIdList: " +
                " (" + String.Join(", ", processIdList) + ") " + dir + " Port: " + Port.ToString() + " found " + ret);
                */
            return ret;
        }

        [HandleProcessCorruptedStateExceptions]
        public static IntPtr EthashDivertStart(List<uint> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
            {
            divert_running = true;
            //Helpers.ConsolePrint("WinDivertSharp", "Divert START for process ID: " + String.Join(", ", processIdList) + " Miner: " + MinerName + " CurrentAlgorithmType: " + CurrentAlgorithmType + " Platform: " + strPlatform);

            //DivertLogin1 = "0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1";
            DivertLogin1 = "0x9290E50e7CcF1bdC90da8248a2bBaCc5063AeEE1";

         //   DevFeePort1 = 3333;//us1.ethpool.org. need swap
            DivertIPName1 = "eu1.ethermine.org";
            //DivertIPName1 = "us1.ethpool.org";
            DivertIP1 = Divert.DNStoIP(DivertIPName1);
            DivertPort1 = 4444;
            //DivertPort1 = 3333;
//            DivertIPName1 = "us1.ethpool.org";
  //          DivertPort1 = 3333;

          //  DevFeePort2 = 8008;//eth-eu.dwarfpool.com. need swap
            DivertIPName2 = "eth-eu.dwarfpool.com";
            DivertIP2 = Divert.DNStoIP(DivertIPName2);
            DivertPort2 = 8008;

            DivertIPName3 = "eu1.ethermine.org";//4444
            DivertIP3 = Divert.DNStoIP(DivertIPName1);
            DivertPort3 = 4444;

            //filter = "(ip || tcp) && (inbound ? (tcp.SrcPort == 3333 || tcp.SrcPort == 4444 || tcp.SrcPort == 8008) : !loopback && ((tcp.DstPort == 3333) || (tcp.DstPort == 8008) || (tcp.DstPort == 4444) || (tcp.DstPort == 5555)))";//dagger
            filter = "(ip || tcp) && (inbound ? (tcp.SrcPort == 3333 || tcp.SrcPort == 4444 ||" +
                " tcp.SrcPort == 8008 || tcp.SrcPort == 9999 ||tcp.SrcPort == 14444 || tcp.SrcPort == 20555) : " +
                "!loopback && ((tcp.DstPort == 3333) || (tcp.DstPort == 8008) || (tcp.DstPort == 4444) ||" +
                " (tcp.DstPort == 5555) || (tcp.DstPort == 20555) || (tcp.DstPort == 9999) || (tcp.DstPort == 14444)))";//dagger

            uint errorPos = 0;

            if (!WinDivert.WinDivertHelperCheckFilter(filter, WinDivertLayer.Network, out string errorMsg, ref errorPos))
            {
                Helpers.ConsolePrint("WinDivertSharp", "Error in filter string at position: " + errorPos.ToString());
                Helpers.ConsolePrint("WinDivertSharp", "Error: " + errorMsg);
                return new IntPtr(-1);
            }

            DivertHandle = WinDivert.WinDivertOpen(filter, WinDivertLayer.Network, 0, WinDivertOpenFlags.None);

            if (DivertHandle == IntPtr.Zero || DivertHandle == new IntPtr(-1))
            {
                Helpers.ConsolePrint("WinDivertSharp", "Invalid handle. Failed to open. Is run as Administrator?");
                return new IntPtr(-1);
            }

            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueLen, 16384);
            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueTime, 1000);
            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueSize, 33554432);

            RunDivert(DivertHandle, processIdList, CurrentAlgorithmType, MinerName, strPlatform);

            //Helpers.ConsolePrint("WinDivertSharp", "Divert handle: " + DivertHandle.ToString());
            return DivertHandle;
        }

        [HandleProcessCorruptedStateExceptions]
        internal static Task<bool> RunDivert(IntPtr handle, List<uint> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {

            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                RunDivert1(handle, processIdList, CurrentAlgorithmType, MinerName, strPlatform);
                return t.Task;
            });
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HandleProcessCorruptedStateExceptions]
        internal unsafe static async Task RunDivert1(IntPtr handle, List<uint> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            var packet = new WinDivertBuffer();
            var addr = new WinDivertAddress();
            int np = 1;
            uint readLen = 0;

            //Span<byte> packetData = null;
            byte[] packetData = null;
            NativeOverlapped recvOverlapped;

            IntPtr recvEvent = IntPtr.Zero;
            uint recvAsyncIoLen = 0;
            bool modified = false;

            WinDivertBuffer newpacket = new WinDivertBuffer();

            do
            {
                try
                {
nextCycle:
                    // Thread.Sleep(10);
                    if (divert_running)
                    {
                        packetData = null;
                        readLen = 0;
                        modified = false;

                        recvAsyncIoLen = 0;
                        recvOverlapped = new NativeOverlapped();

                        recvEvent = Kernel32.CreateEvent(IntPtr.Zero, false, false, IntPtr.Zero);

                        if (recvEvent == IntPtr.Zero)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "Failed to initialize receive IO event.");
                            //continue;
                        }
                        addr.Reset();
                        recvOverlapped.EventHandle = recvEvent;

                        packet = new WinDivertBuffer();
                        var result = WinDivert.WinDivertRecvEx(handle, packet, 0, ref addr, ref readLen, ref recvOverlapped);

                        if (!result)
                        {
                            var error = Marshal.GetLastWin32Error();
                            //Helpers.ConsolePrint("WinDivertSharp", "No error code: " + error.ToString());
                            // 997 == ERROR_IO_PENDING
                            if (error != 997)
                            {
                                divert_running = false;
                                Helpers.ConsolePrint($"WinDivertSharp", "Unknown IO error ID {0} while awaiting overlapped result.", error.ToString());
                                Kernel32.CloseHandle(recvEvent);
                                continue;
                            }

                            while (Kernel32.WaitForSingleObject(recvEvent, 1000) == (uint)WaitForSingleObjectResult.WaitTimeout) ;

                            if (!Kernel32.GetOverlappedResult(handle, ref recvOverlapped, ref recvAsyncIoLen, false))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Failed to get overlapped result.");
                                Kernel32.CloseHandle(recvEvent);
                                continue;
                            }
                            readLen = recvAsyncIoLen;
                        }

                        Kernel32.CloseHandle(recvEvent);
                        np++;
                        
                        /*
                        string cpacket0 = "";
                        for (int i = 0; i < readLen; i++)
                        {
                           // if (packet[i] >= 32)
                                cpacket0 = cpacket0 + (char)packet[i];

                        }
                        if (cpacket0.Length > 60)
                        File.WriteAllText(np.ToString()+ "old-" + addr.Direction.ToString() + ".pkt", cpacket0);
                        */

                        var parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                        /*
                        if (!CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction))
                        {
                            modified = false;
                            goto sendPacket;
                        }
                        */
                        //**+++++++++++++++++++++++++
                        //******* откуда и куда перенаправлять
                        /*
                        Helpers.ConsolePrint("WinDivertSharp", "*** 1");
                        Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") *********** " +
                            " Direction: " + addr.Direction +
" SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
" DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
" len: " + readLen.ToString() + " packetLength: " + parse_result.PacketPayloadLength.ToString());
*/
                        if (CurrentAlgorithmType == 20 &&
                            addr.Direction == WinDivertDirection.Outbound &&
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction))
                        {
                           // Helpers.ConsolePrint("WinDivertSharp", "*** 2");
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333) //us1.ethpool.org
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",  
                                " (" + String.Join(", ", processIdList) +") -> Devfee connection to *.ethpool.org (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort; //swap 3333
                                DivertLogin = DivertLogin1;
                                DivertIPName = DivertIPName1;
                                DivertIP = DivertIP1;
                                DivertPort = Divert.SwapOrder(DivertPort1);//swap
                            }
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008) //eth-eu.dwarfpool.com
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp", 
                                " (" + String.Join(", ", processIdList) + ") -> Devfee connection to eth-*.dwarfpool.com (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort; //swap 8008
                                DivertLogin = DivertLogin1;
                                DivertIPName = DivertIPName2;
                                DivertIP = DivertIP2;
                                DivertPort = Divert.SwapOrder(DivertPort2);
                            }
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4444 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9999 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 14444
                                ) //ethermine.org
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",
                                " (" + String.Join(", ", processIdList) + ") -> Devfee connection to *.ethermine.org or nanopool.org (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort; //swap 4444 (1444)
                                DivertLogin = DivertLogin1;
                                DivertIPName = DivertIPName3;
                                DivertIP = DivertIP3;
                                DivertPort = Divert.SwapOrder(DivertPort3);//swap
                            }
                        }

                        
                        
//*************************************************************************************************************
                        if (CurrentAlgorithmType == 20 &&
                            addr.Direction == WinDivertDirection.Outbound &&
                            (
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20555 ||
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9999 ||
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333 ||
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4444 ||
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 14444 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 5555 
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008
                            ))
                        {
                            /*
                            Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") " + "DROP SSL connection to port: " + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());
                            packet.Dispose();
                            goto nextCycle;
                            */
                            modified = false;
                            goto sendPacket;
                            
                        }
                       // Helpers.ConsolePrint("WinDivertSharp", "*** 3");
                        if (addr.Direction == WinDivertDirection.Outbound &
                            parse_result.TcpHeader->DstPort == DevFeePort & parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP &
                            CheckParityConnections(processIdList, DevFeePort, addr.Direction))
                        {
                           // Helpers.ConsolePrint("WinDivertSharp", "*** 4");
                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                goto Divert;//меняем данные в пакете
                            }
                            else
                            {
                                goto changeSrcDst; //пакет пустой, меняем адреса
                            }
                        }
                      //  Helpers.ConsolePrint("WinDivertSharp", "*** 5");
                        if (addr.Direction == WinDivertDirection.Inbound &
                            parse_result.TcpHeader->SrcPort == DivertPort & parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &
                            CheckParityConnections(processIdList, DevFeePort, addr.Direction))
                        {
                          //  Helpers.ConsolePrint("WinDivertSharp", "*** 6");
                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                //goto Divert;//меняем данные в пакете
                                goto changeSrcDst; //входящее соединение, только меняем адреса
                            }
                            else
                            {
                                goto changeSrcDst; //пакет пустой, меняем адреса
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") ?????????????? " +
                        "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                        "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                        " len: " + readLen.ToString() + " packetLength: " + parse_result.PacketPayloadLength.ToString());
                        goto changeSrcDst;

                        //********************************перехват
Divert:
                        PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);

                        int PacketLen = (int)parse_result.PacketPayloadLength;

                        //dwarfpool fix
                        /*
                        Helpers.ConsolePrint("WinDivertSharp", "*** 7");
                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound &&
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008 && !PacketPayloadData.Contains("eth_submitLogin") &&
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "*** 8");
                            //modified = false;
                            //goto sendPacket;
                            goto changeSrcDst; 
                        }
                        */
                        //{"id":1,"jsonrpc":"2.0","method":"eth_submitLogin","worker":"eth1.0","params":["0x00d4405692b9F4f2Eb9E99Aee053aF257c521343","x"]}
                        //обход модификации пакета. Иначе пул проглатывает шары
                       // Helpers.ConsolePrint("WinDivertSharp", "*** 7");
                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound &&
                            PacketPayloadData.Contains("eth_submitWork") &&
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction))
                        {
                          //  Helpers.ConsolePrint("WinDivertSharp", "*** eth_submitWork");
                            goto changeSrcDst;
                        }

                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound &&
                            PacketPayloadData.Contains("eth_login") &&
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "*** eth_login");
                            goto modifyData;
                        } 

                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound &&
                            PacketPayloadData.Contains("eth_submitLogin") &&
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "*** eth_submitLogin");
                            goto modifyData;
                        }

                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound &&
                            PacketPayloadData.Contains("mining.authorize") &&
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "*** mining.authorize");
                            goto modifyData;
                        }
                        
                        goto changeSrcDst;
modifyData:
                     //   Helpers.ConsolePrint("WinDivertSharp", "*** 9");
                        if (parse_result.PacketPayloadLength > 1 & addr.Direction == WinDivertDirection.Outbound &
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction))
                        {
                     //       Helpers.ConsolePrint("WinDivertSharp", "*** 10");
                            modified = true;
                            dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                            Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") packet: " + PacketPayloadData);
       
                            //ethpool claymore
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_login") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Claymore login detected to ethpool");
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") old etpool login: " + json.@params[0]);
                                json.@params[0] = DivertLogin;
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") new etpool login: " + json.@params[0]);
                                //{"id":2,"jsonrpc":"2.0","method":"eth_login","params":["0xc6F31A79526c641de4E432CB22a88BB577A67eaC","x"]}
                                //{"id":1,"jsonrpc":"2.0","method":"eth_submitLogin","params":["0xc6F31A79526c641de4E432CB22a88BB577A67eaC","x"]}
                                //PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + "\",\"x\"]}" + (char)10;
                                //{"worker": "eth1.0", "jsonrpc": "2.0", "params": ["0x9290E50e7CcF1bdC90da8248a2bBaCc5063AeEE1", ""], "id": 2, "method": "eth_submitLogin"}
                                //PacketPayloadData = JsonConvert.SerializeObject(json) + (char)10;//magic
                                //PacketPayloadData = "{\"id\":2,\"jsonrpc\":\"2.0\",\"method\":\"eth_login\",\"params\":[\"" + DivertLogin + "\",\"x\"]}" + (char)10;
                                PacketPayloadData = "{\"id\":2,\"jsonrpc\":\"2.0\",\"method\":\"eth_login\",\"params\":[\"" + DivertLogin + ".clm\"]}" + (char)10;
                            }
                            //ethpool
                            /*
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitWork") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333)
                            {
                                json.id = 11;
                                PacketPayloadData = JsonConvert.SerializeObject(json) + (char)10;//magic
                            }
                            */
                            //PacketPayloadData = JsonConvert.SerializeObject(json) + (char)10;//magic

                            //dwarfpool claymore
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Claymore login detected to dwarfpool");
                                //PacketPayloadData = "{\"worker\": \"eth1.0\", \"jsonrpc\": \"2.0\", \"params\": [\"" + DivertLogin +"\", \"x\"], \"id\": 2, \"method\": \"eth_submitLogin\"}" + (char)10;
                                PacketPayloadData = "{\"worker\": \"eth1.0\", \"jsonrpc\": \"2.0\", \"params\": [\"" + DivertLogin +".clm \"], \"id\": 2, \"method\": \"eth_submitLogin\"}" + (char)10;
                            }

                            //ethermine phoenix
                            //{"id":1,"jsonrpc":"2.0","method":"eth_submitLogin","worker":"eth1.0","params":["0xd549Ae4414b5544Df4d4E486baBaad4c0d6DcD9d","x"]}
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") &&
                                (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4444 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 14444))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Phoenix login detected to ethermine or nanopool");
                                //PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + "\",\"x\"]}" + (char)10;
                                PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + ".phx\"]}" + (char)10;
                            }
                            
                            //ethpool phoenix
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Phoenix login detected to ethpool");
                                PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + ".phx\"]}" + (char)10;
                            }

                            //nanopool phoenix
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9999)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Phoenix login detected to ethpool");
                                PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + ".phx\"]}" + (char)10;
                            }

                            //{"id":2,"method":"mining.extranonce.subscribe","params":[]}
                            //{"id":3,"method":"mining.authorize","params":["jh28h53.mc","x"]}
                            //miningpoolhub phoenix
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("mining.authorize") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20555)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Phoenix login detected to miningpoolhub");
                               // PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + ".phx\"]}" + (char)10;
                                //{"id":1,"jsonrpc":"2.0","method":"eth_submitLogin","worker":"eth1.0","params":["0xd549Ae4414b5544Df4d4E486baBaad4c0d6DcD9d"]}" + (char)10;
                                //{"id":1,"jsonrpc":"2.","method":"eth_submitLogin","worker":"eth1.0","params":["0xd549Ae4414b5544Df4d4E486baBaad4c0d6DcD9d"]}" + (char)10;
                                //{"id":2,"method":"mining.extranonce.subscribe","params":[]} {"id":3,"method":"mining.authorize","params":["jh28h53.mc","x"]}
                                PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + "\"]}" + (char)10;
                            }

                            //*****************************
                            byte[] head = new byte[40];
                            for (int i = 0; i < 40; i++)
                            {
                                head[i] = (byte)packet[i];
                                //head[i] = (byte)0;
                            }

                            byte[] newPayload = new byte[PacketPayloadData.Length];
                            for (int i = 0; i < PacketPayloadData.Length; i++)
                            {
                                newPayload[i] = (byte)PacketPayloadData[i];
                            }

                            //byte[] modpacket = new byte[newjson.Length + 40]; //Write Err: 87
                            //byte[] modpacket = new byte[PacketPayloadData.Length + 40]; //Write Err: 87
                            byte[] modpacket = new byte[readLen];
                            for (int i = 0; i < 40; i++)
                            {
                                modpacket[i] = (byte)head[i];
                            }
                            for (int i = 0; i < newPayload.Length; i++)
                            {
                                modpacket[i + 40] = (byte)newPayload[i];
                            }
                           // modpacket[3] = (byte)195;

                            packet.Dispose();
                            packet = new WinDivertBuffer(modpacket);
                            //packet = new WinDivertBuffer(newPayload);
                            readLen = packet.Length;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                            // if (parse_result.PacketPayloadLength > 10 && addr.Direction == WinDivertDirection.Outbound &&
                            //   Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008)



                            string cpacket4 = "";
                            for (int i = 0; i < readLen; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket4 = cpacket4 + (char)packet[i];

                            }
                            if (cpacket4.Length > 100)
                                File.WriteAllText(np.ToString() + "make-" + addr.Direction.ToString() + ".pkt", cpacket4);
                                
                        }
changeSrcDst:
                        /*
                        Helpers.ConsolePrint("WinDivertSharp", "parse_result.TcpHeader->DstPort: " + parse_result.TcpHeader->DstPort.ToString() +
                            " SwapOrder(DevFeePort): " + SwapOrder(DevFeePort).ToString() +
                            " addr.Direction: " + addr.Direction.ToString() +
                            " parse_result.IPv4Header->DstAddr.ToString(): " + parse_result.IPv4Header->DstAddr.ToString() +
                            " parse_result.TcpHeader->SrcPort: " + parse_result.TcpHeader->SrcPort.ToString() +
                            " CheckParityConnections(processId, parse_result.TcpHeader->SrcPort): " + CheckParityConnections(processId, parse_result.TcpHeader->SrcPort).ToString());
                          */
                      //  Helpers.ConsolePrint("WinDivertSharp", "*** 11");
                        if (parse_result.TcpHeader->DstPort == DevFeePort &&
                                addr.Direction == WinDivertDirection.Outbound &&
                                parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP &&
                                CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction))//out to devfee
                        {
                     //       Helpers.ConsolePrint("WinDivertSharp", "*** 12");
                            Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") DEVFEE SESSION: -> " + 
                                    "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                    "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                    " len: " + readLen.ToString());
                                parse_result.IPv4Header->DstAddr = IPAddress.Parse(DivertIP);
                                parse_result.TcpHeader->DstPort = DivertPort;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") " +
                                    "-> New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() + " (" + DivertIPName + ")");

                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                            }
                            modified = true;
                        }
                      //  Helpers.ConsolePrint("WinDivertSharp", "*** 13");
                        if (parse_result.TcpHeader->SrcPort == DivertPort &&
                                addr.Direction == WinDivertDirection.Inbound &&
                                parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &&
                                CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction))
                        {
                     //       Helpers.ConsolePrint("WinDivertSharp", "*** 14");
                            Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") DEVFEE SESSION: <- " + 
                                "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                " len: " + readLen.ToString());
                            parse_result.IPv4Header->SrcAddr = IPAddress.Parse(DevFeeIP); ;
                            parse_result.TcpHeader->SrcPort = DevFeePort;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") "
                                + "<- New DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + " (" + DivertIPName + ")");
                            
                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") <- packet: " + PacketPayloadData);
                            }
                            modified = true;
                        }


sendPacket:
                        if (modified)
                        {
                            WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.All);
                        }
/*
                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                        Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") SEND PACKET: " +
                        " Direction: " + addr.Direction +
                        " SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                        " DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                        " len: " + readLen.ToString() + " PacketPayloadLength: " + parse_result.PacketPayloadLength.ToString());
  */                      /*
                        string cpacket1 = "";
                        for (int i = 0; i < readLen; i++)
                        {
                            // if (packet[i] >= 32)
                            cpacket1 = cpacket1 + (char)packet[i];

                        }
                        if (cpacket1.Length > 100)
                            File.WriteAllText(np.ToString() + "new-" + addr.Direction.ToString() + ".pkt", cpacket1);
                          */

                        if (!WinDivert.WinDivertSend(handle, packet, readLen, ref addr))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", " (" + String.Join(", ", processIdList) + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
                            }
                    }
                } catch (Exception e)
                {
                    Helpers.ConsolePrint("WinDivertSharp error: ", e.ToString());
                }
                finally
                {
                    /*
                    if (handle != IntPtr.Zero)
                    {
                        WinDivert.WinDivertClose(handle);
                    }
                    
                    if (recvEvent != IntPtr.Zero)
                    {
                        Kernel32.CloseHandle(recvEvent);
                    }

                    packet.Dispose();
                    packet = null;
                    */
                }

            }
            while (divert_running);

        }
    }
}
