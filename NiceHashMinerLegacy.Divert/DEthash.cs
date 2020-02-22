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
using System.Text.RegularExpressions;

namespace NiceHashMinerLegacy.Divert
{
    public class DEthash
    {
        private static volatile bool divert_running = true;
        private static IntPtr DivertHandle;
        private static readonly uint MaxPacket = 2048;

        private static string DevFeeIP = "";
        private static ushort DevFeePort = 0;

        private static string DivertIP = "";
        private static ushort DivertPort = 0;

        private static string DivertIP1 = "";
        private static ushort DivertPort1 = 0;

        private static string DivertIP2 = "";
        private static ushort DivertPort2 = 0;

        private static string DivertIP3 = "";
        private static ushort DivertPort3 = 0;

        private static string DivertIP4 = "";
        private static ushort DivertPort4 = 0;

        private static string DivertIP5 = "";
        private static ushort DivertPort5 = 0;

        private static string DivertIP6 = "";
        private static ushort DivertPort6 = 0;

        private static string filter = "";

        private static string DivertLogin = "";
        private static string DivertLogin1 = "";

        private static string PacketPayloadData;
        private static string OwnerPID = "-1";
        private static int stratumRatio = 0;
        private static List<string> _oldPorts = new List<string>();

        private static WinDivertBuffer newpacket = new WinDivertBuffer();
        private static WinDivertParseResult parse_result;
        private static int PacketLen;
        private static string RemoteIP;


        internal static string CheckParityConnections(List<string> processIdList, ushort Port, WinDivertDirection dir)
        {
            if (String.Join(" ", processIdList).Contains("gminer: force"))
            {
                return "gminer: force";
            }
            
            string ret = "unknown";
            string miner = "";
            Port = Divert.SwapOrder(Port);

            List<Connection> _allConnections = new List<Connection>();
            _allConnections.Clear();
                _allConnections.AddRange(NetworkInformation.GetTcpV4Connections());

                for (int i = 1; i < _allConnections.Count; i++)
                {
                    if (String.Join(" ", processIdList).Contains(_allConnections[i].OwningPid.ToString()) &&
                        (_allConnections[i].LocalEndPoint.Port == Port) ||
                        _allConnections[i].RemoteEndPoint.Port == Port)
                    {
                        ret = _allConnections[i].OwningPid.ToString();
                        for (var j = 0; j < processIdList.Count; j++)
                        {
                            if (processIdList[j].Contains(ret))
                            {
                                miner = processIdList[j].Split(':')[0];
                            }
                        }

                    if (!String.Join(" ", _oldPorts).Contains(Port.ToString()))
                    {
                        _oldPorts.Add(miner + ": " + ret + " : " + Port.ToString());
                        //Helpers.ConsolePrint("WinDivertSharp", "add _oldPorts: " + String.Join(" ", _oldPorts));
                    }
                        _allConnections.Clear();
                        _allConnections = null;
                        return miner + ": " + ret;
                    }
                }
            for (int i = 1; i < _oldPorts.Count; i++)
            {
               //Helpers.ConsolePrint("WinDivertSharp", "_oldPorts: " + String.Join(" ", _oldPorts));
                if (String.Join(" ", _oldPorts).Contains(Port.ToString()))
                {
                    //Helpers.ConsolePrint("WinDivertSharp", "_oldPorts found: " + Port.ToString());
                    return "unknown: ?";
                }
            }

               // Helpers.ConsolePrint("WinDivertSharp", "Error processIdList: " + String.Join(" ", processIdList) +
               // " Port not found: " + Port.ToString() + " Direction: " + dir);
            _allConnections.Clear();
            _allConnections = null;

            return "-1";
        }

        [HandleProcessCorruptedStateExceptions]
        public static IntPtr EthashDivertStart(List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
            {
            divert_running = true;

            DivertLogin1 = "0x9290E50e7CcF1bdC90da8248a2bBaCc5063AeEE1";

            DivertIP1 = Divert.DNStoIP("eu1.ethermine.org");
            DivertPort1 = 4444;
            //DivertIP1 = Divert.DNStoIP("eth-eu1.nanopool.org"); //не понимает логин клеймора
            //DivertIP1 = Divert.DNStoIP("asia1.ethermine.org");
            //DivertPort1 = 9999;

            DivertIP2 = Divert.DNStoIP("eth-eu.dwarfpool.com");//eth.f2pool.com:8008 //49.247.192.198:8008 - etp-kor1.topmining.co.kr:8008
            DivertPort2 = 8008;

            //не работает. пока отключим шиткоины
            //clo.2miners.com:3030 exp.2miners.com:3030 pirl.2miners.com:6060 etp.2miners.com:9292

            DivertIP3 = Divert.DNStoIP("eth.2miners.com");//используется getWork
            DivertPort3 = 2020;

            //   " || tcp.SrcPort == 3030 || tcp.SrcPort == 6060 || tcp.SrcPort == 9292)" +
            //   " || tcp.DstPort == 3030 || tcp.DstPort == 6060 || tcp.DstPort == 9292)" +
filter = "(!loopback && outbound ? (tcp.DstPort == 3333 || tcp.DstPort == 4444 || tcp.DstPort == 13333 ||" +
                " tcp.DstPort == 9999 || tcp.DstPort == 19999 || tcp.DstPort == 14444 || tcp.DstPort == 20555 ||" +
                " tcp.DstPort == 8008 || tcp.DstPort == 3030 || tcp.DstPort == 6060 || tcp.DstPort == 9292 ||" +
                " tcp.DstPort == 3516 || tcp.DstPort == 8443 ||" +
                " tcp.DstPort == 5555)" +
                " : " + 
                "(tcp.SrcPort == 3333 || tcp.SrcPort == 4444 || tcp.SrcPort == 13333 ||" +
                " tcp.SrcPort == 9999 || tcp.SrcPort == 19999 || tcp.SrcPort == 14444 || tcp.SrcPort == 20555 ||" +
                " tcp.SrcPort == 8008 || tcp.SrcPort == 3030 || tcp.SrcPort == 6060 || tcp.SrcPort == 9292 ||" +
                " tcp.SrcPort == 3516 || tcp.SrcPort == 8443)" +
                ")";

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

            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueLen, 2048); //16386
            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueTime, 1000);
            //WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueSize, 33554432);
            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueSize, 2097152);

            RunDivert(DivertHandle, processIdList, CurrentAlgorithmType, MinerName, strPlatform);

            return DivertHandle;
        }

        [HandleProcessCorruptedStateExceptions]
        internal static Task<bool> RunDivert(IntPtr handle, List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
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
        internal unsafe static async Task RunDivert1(IntPtr handle, List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            var packet = new WinDivertBuffer();
            var addr = new WinDivertAddress();
            int np = 1;
            uint readLen = 0;
            List<string> InboundPorts = new List<string>();

        //Span<byte> packetData = null;

            NativeOverlapped recvOverlapped;

            IntPtr recvEvent = IntPtr.Zero;
            uint recvAsyncIoLen = 0;
            bool modified = false;



            do
            {
                try
                {
nextCycle:
                    if (divert_running)
                    {
                        readLen = 0;
                        modified = false;
                        PacketPayloadData = null;
                        packet.Dispose();
                        recvAsyncIoLen = 0;
                        //recvOverlapped = new NativeOverlapped();

                        //recvEvent = Kernel32.CreateEvent(IntPtr.Zero, false, false, IntPtr.Zero);
                        /*
                        if (recvEvent == IntPtr.Zero)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "Failed to initialize receive IO event.");
                            //continue;
                        }
                        */
                        addr.Reset();
                        //recvOverlapped.EventHandle = recvEvent;

                        packet = new WinDivertBuffer();
                        //var result = WinDivert.WinDivertRecvEx(handle, packet, 0, ref addr, ref readLen, ref recvOverlapped);
                        var result = WinDivert.WinDivertRecv(handle, packet, ref addr, ref readLen);
                        /*
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

                            //while (Kernel32.WaitForSingleObject(recvEvent, 1000) == (uint)WaitForSingleObjectResult.WaitTimeout) ;
                            
                            if (!Kernel32.GetOverlappedResult(handle, ref recvOverlapped, ref recvAsyncIoLen, false))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Failed to get overlapped result.");
                                Kernel32.CloseHandle(recvEvent);
                                continue;
                            }
                            //readLen = recvAsyncIoLen;
                            
                        }
                */

                        //Kernel32.CloseHandle(recvEvent);
                        np++;


                        /*
                        string cpacket0 = "";
                        for (int i = 0; i < readLen; i++)
                        {
                           // if (packet[i] >= 32)
                                cpacket0 = cpacket0 + (char)packet[i];

                        }
                        //if (cpacket0.Length > 60)
                        File.WriteAllText(np.ToString()+ "old-" + addr.Direction.ToString() + ".pkt", cpacket0);
                        */

                        /*
                        if (stratumRatio < -7)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "Alternate divert stratum, ratio: " + stratumRatio.ToString());
                            DivertIP1 = Divert.DNStoIP("asia1.ethermine.org");
                            DivertPort1 = 14444;
                            stratumRatio = -100; //фиксируем на альтернативном пуле. Потом подумать, как обратно вернуть
                        }
                        if (stratumRatio > 10)
                        {
                            stratumRatio = 5;
                        }
                        */
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);



                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        }

                        if (CurrentAlgorithmType == 20 &&
                            addr.Direction == WinDivertDirection.Outbound )
                        {
                            //список соответствия src port и dst ip
                            if(!Divert.CheckSrcPort(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString()))
                            {
                                InboundPorts.Add(Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                ":" + parse_result.IPv4Header->DstAddr.ToString());
                            }

                            if (OwnerPID.Contains("nbminer") && (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 13333)) //sparkpool
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",
                                "(" + OwnerPID.ToString() + ") -> Devfee connection to sparkpool.com (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort;
                                DivertLogin = "sp_angelbb";
                                DivertIP = parse_result.IPv4Header->DstAddr.ToString();
                                DivertPort = parse_result.TcpHeader->DstPort;
                                goto parsePacket;
                            }

                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333) //us1.ethpool.org
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",  
                                "(" + OwnerPID.ToString() +") -> Devfee connection to (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort; 
                                DivertLogin = DivertLogin1;
                                DivertIP = DivertIP1;
                                DivertPort = Divert.SwapOrder(DivertPort1);//swap
                                goto parsePacket;
                            }
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008) //eth-eu.dwarfpool.com
                            {
                                if (OwnerPID.Contains("gminer"))
                                {
                                    modified = false;
                                    goto sendPacket;

                                }
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp", 
                                "(" + OwnerPID.ToString() + ") -> Devfee connection to eth-*.dwarfpool.com (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort; //swap 8008
                                DivertLogin = DivertLogin1;
                                DivertIP = DivertIP2;
                                DivertPort = Divert.SwapOrder(DivertPort2);
                                goto parsePacket;
                            }
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4444 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9999 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 19999 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20555 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 14444
                                ) //ethermine.org
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",
                                "(" + OwnerPID.ToString() + ") -> Devfee connection to (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort; 
                                DivertLogin = DivertLogin1;
                                DivertIP = DivertIP1;
                                DivertPort = Divert.SwapOrder(DivertPort1);
                                goto parsePacket;
                            }
                            /*
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3030 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 6060 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9292 ) //2miners 
                            {
                                modified = false;
                                goto sendPacket;
                                
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",
                                "(" + OwnerPID.ToString() + ") -> Devfee connection to 2miners (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort; 
                                DivertLogin = DivertLogin1;
                                DivertIP = DivertIP3;
                                //DivertIP = parse_result.IPv4Header->DstAddr.ToString(); //не менять dstaddr
                                DivertPort = Divert.SwapOrder(DivertPort3);
                                goto parsePacket;
                                
                            }
                            */
                        }


                        //*************************************************************************************************************
                        parsePacket:
                        if (CurrentAlgorithmType == 20 &&
                            addr.Direction == WinDivertDirection.Outbound &&
                            (
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20555 ||
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9999 ||
                            //Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333 ||
                            //Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4444 ||
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 14444 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 5555 
                           // Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008 
                            ))
                        {
                            
                            /*
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") " + "DROP SSL connection to port: " + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());
                            packet.Dispose();
                            goto nextCycle;
                            */
                            
                            modified = false;
                            goto sendPacket;
                            
                        }
                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        if (addr.Direction == WinDivertDirection.Outbound &
                            parse_result.TcpHeader->DstPort == DevFeePort & parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP &
                           !OwnerPID.Equals("-1"))
                        {
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

                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        }

                        if (addr.Direction == WinDivertDirection.Inbound &&
                            //проверку входящего соединения можно упростить
                            //но могут путаться пакеты, если несколько соединений одновременно
                            //parse_result.TcpHeader->SrcPort == DivertPort && parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &&
                            !OwnerPID.Equals("-1"))
                        {
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

                        if (
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3516 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3030 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 6060 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9292 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8443 || //
                            Divert.SwapOrder(parse_result.TcpHeader->SrcPort) == 3516 ||
                            Divert.SwapOrder(parse_result.TcpHeader->SrcPort) == 3030 ||
                            Divert.SwapOrder(parse_result.TcpHeader->SrcPort) == 6060 ||
                            Divert.SwapOrder(parse_result.TcpHeader->SrcPort) == 9292 ||
                            Divert.SwapOrder(parse_result.TcpHeader->SrcPort) == 8443
                            ) //?? gminer 
                        {
                            /*
                            string cpacket10 = "";
                            for (int i = 0; i < readLen; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket10 = cpacket10 + (char)packet[i];

                            }
                            //if (cpacket10.Length > 60)
                                File.WriteAllText(np.ToString() + "gm-" + addr.Direction.ToString() + ".pkt", cpacket10);
                                */
                            modified = false;
                            goto sendPacket;

                        }
                        Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") Unknown connection: " +
                        "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                        "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                        " len: " + readLen.ToString() + " packetLength: " + parse_result.PacketPayloadLength.ToString());
                        modified = false;
                        goto sendPacket;

                        //********************************перехват
Divert:
                        PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);

                        PacketLen = (int)parse_result.PacketPayloadLength;

                        //обход модификации пакета. Иначе пул проглатывает шары
                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        }

                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound)
                        {
                            if (PacketPayloadData.Contains("eth_submitWork") && !OwnerPID.Equals("-1"))
                            {
                                goto changeSrcDst;
                            }

                            if (PacketPayloadData.Contains("eth_login") && !OwnerPID.Equals("-1"))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "*** eth_login");
                                goto modifyData;
                            }

                            if (PacketPayloadData.Contains("eth_submitLogin") && !OwnerPID.Equals("-1"))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "*** eth_submitLogin");
                                goto modifyData;
                            }

                            if (PacketPayloadData.Contains("mining.authorize") && !OwnerPID.Equals("-1"))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "*** mining.authorize");
                                goto modifyData;
                            }
                        }
                        goto changeSrcDst;
modifyData:
                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        if (parse_result.PacketPayloadLength > 1 & addr.Direction == WinDivertDirection.Outbound &
                           !OwnerPID.Equals("-1"))
                        {
                            modified = true;
                            //dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") packet: " + PacketPayloadData);
       
                            //ethpool claymore
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_login") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Claymore login detected to ethpool");
                                PacketPayloadData = "{\"id\":2,\"jsonrpc\":\"2.0\",\"method\":\"eth_login\",\"params\":[\"" + DivertLogin + ".clm\"]}" + (char)10;
                            }

                            //dwarfpool claymore
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") && !PacketPayloadData.Contains(",\"jsonrpc\":\"2.0\"}") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Claymore login detected to dwarfpool");
                                PacketPayloadData = "{\"worker\": \"eth1.0\", \"jsonrpc\": \"2.0\", \"params\": [\"" + DivertLogin +".clm \"], \"id\": 2, \"method\": \"eth_submitLogin\"}" + (char)10;
                                goto changePayloadData;
                            }
                            
                            //ethermine phoenix
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") && PacketPayloadData.Contains(",\"x\"]}") &&
                                (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4444 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 14444))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Phoenix login detected to ethermine or nanopool");
                                PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + ".phx\"]}" + (char)10;
                                goto changePayloadData;
                            }

                            //ethpool phoenix 
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") && !PacketPayloadData.Contains(",\"jsonrpc\":\"2.0\"}") &&
                                !PacketPayloadData.Contains("sp_nbminer") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Phoenix login detected to ethpool");
                                PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + ".phx\"]}" + (char)10;
                                goto changePayloadData;
                            }

                            //nanopool phoenix
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9999)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Phoenix login detected to ethpool");
                                PacketPayloadData = "{\"id\":1,\"jsonrpc\":\"2.0\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + ".phx\"]}" + (char)10;
                                goto changePayloadData;
                            }

                            //miningpoolhub phoenix
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("mining.authorize") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20555)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Phoenix login detected to miningpoolhub");
                                PacketPayloadData = "{\"id\":2,\"jsonrpc\":\"2.\",\"method\":\"eth_submitLogin\",\"worker\":\"eth1.0\",\"params\":[\"" + DivertLogin + "\"]}" + (char)10;
                                goto changePayloadData;
                            }

                            //ethermine gminer
                            if (CurrentAlgorithmType == 20 && !PacketPayloadData.Contains(",\"x\"]}") &&
                                PacketPayloadData.Contains("eth_submitLogin") &&
                                (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4444))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "GMiner login detected to ethermine");
                                PacketPayloadData = "{\"id\":1,\"method\":\"eth_submitLogin\",\"worker\":\"  \",\"params\":[\"" + DivertLogin + ".gmr\"],\"jsonrpc\":\"2.0\"}" + (char)10;
                                goto changePayloadData;
                            }

                            //dwarfpool gminer
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains(",\"jsonrpc\":\"2.0\"}") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "GMiner login detected to dwarfpool");
                                  PacketPayloadData = "{\"id\": 1, \"jsonrpc\": \"2.0\", \"method\": \"eth_login\", \"params\": [\"" + DivertLogin + ".gmr\"]}" + (char)10;
                                goto changePayloadData;
                            }

                            //gminer ethpool
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") && PacketPayloadData.Contains(",\"jsonrpc\":\"2.0\"}") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "GMiner login detected to ethpool");
                                PacketPayloadData = "{\"id\":1,\"method\":\"eth_submitLogin\",\"params\":[\"" + DivertLogin + ".gmr\"],\"jsonrpc\":\"2.0\"}" + (char)10;
                                goto changePayloadData;
                            }

                            //gminer nanopool
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") && PacketPayloadData.Contains(",\"jsonrpc\":\"2.0\"}") &&
                                (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9999 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 19999))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "GMiner login detected to nanopool");
                                PacketPayloadData = "{\"id\":1,\"method\":\"eth_submitLogin\",\"params\":[\"" + DivertLogin + ".gmr\"],\"jsonrpc\":\"2.0\"}" + (char)10;
                                goto changePayloadData;
                            }

                            //gminer 2miners
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") && PacketPayloadData.Contains(",\"jsonrpc\":\"2.0\"}") &&
                                (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3030 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 6060 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9292))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "GMiner login detected to 2miners");
                                PacketPayloadData = "{\"id\":1,\"method\":\"eth_submitLogin\",\"params\":[\"" + DivertLogin + ".gmr\"],\"jsonrpc\":\"2.0\"}" + (char)10;
                                goto changePayloadData;
                            }

                            //nbminer eth
                            if (CurrentAlgorithmType == 20 &&
                                PacketPayloadData.Contains("eth_submitLogin") && PacketPayloadData.Contains("sp_nbminer") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "NBMiner login detected to sparkpool");
                                //PacketPayloadData = "{\"id\":1,\"method\":\"eth_submitLogin\",\"params\":[\"" + DivertLogin + "\"],\"worker\":\"\"}" + (char)10;
                                PacketPayloadData = "{\"id\":1,\"method\":\"eth_submitLogin\",\"params\":[\"" + DivertLogin + "\"],\"worker\":\"default1\"}" + (char)10;
                                goto changePayloadData;
                            }
                            //{"id":1,"method":"eth_submitLogin","params":["sp_nbminer"],"worker":"5EE77B52"}
                            //{"id":1,"method":"eth_submitLogin","params":["sp_nbminer"],"worker":"5EE77B52"}

                            changePayloadData:
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

                            packet.Dispose();
                            packet = new WinDivertBuffer(modpacket);
                            //packet = new WinDivertBuffer(newPayload);
                            readLen = packet.Length;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                            head = null;
                            modpacket = null;
                            newPayload = null;
                            /*
                            string cpacket4 = "";
                            for (int i = 0; i < readLen; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket4 = cpacket4 + (char)packet[i];

                            }
                            //if (cpacket4.Length > 100)
                                File.WriteAllText(np.ToString() + "make-" + addr.Direction.ToString() + ".pkt", cpacket4);
                              */ 
                        }
changeSrcDst:
                        /*
                        Helpers.ConsolePrint("WinDivertSharp", "Before Src: "+ parse_result.IPv4Header->SrcAddr.ToString()+ ":"+ Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                            " Dst: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                            " DevFeePort: " + Divert.SwapOrder(DevFeePort).ToString() +
                            " Direction: " + addr.Direction.ToString() +
                            " CheckParityConnections(processId, parse_result.TcpHeader->SrcPort): " + CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction));
                          */

                        if (parse_result.TcpHeader->DstPort == DevFeePort &&
                                addr.Direction == WinDivertDirection.Outbound &&
                                parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP &&
                               !OwnerPID.Equals("-1"))//out to devfee
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") DEVFEE SESSION: -> " + 
                                    "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                    "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                    " len: " + readLen.ToString());
                                parse_result.IPv4Header->DstAddr = IPAddress.Parse(DivertIP);
                                parse_result.TcpHeader->DstPort = DivertPort;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") " +
                                    "-> New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());

                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                            }
                            modified = false;
                            goto sendPacket;
                        }

                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        }

                        if (parse_result.TcpHeader->SrcPort == DivertPort &&
                                addr.Direction == WinDivertDirection.Inbound &&
                                parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &&
                               !OwnerPID.Equals("-1"))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") DEVFEE SESSION: <- " + 
                                "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                " len: " + readLen.ToString());
                            //parse_result.IPv4Header->SrcAddr = IPAddress.Parse(DevFeeIP); ;
                            parse_result.TcpHeader->SrcPort = DevFeePort;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            RemoteIP = Divert.GetRemoteIP(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());
                            parse_result.IPv4Header->SrcAddr = IPAddress.Parse(RemoteIP);
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") "
                                + "<- New DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                "  New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());

                            
                            //Helpers.ConsolePrint("WinDivertSharp", Divert.GetRemoteIP(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString()));
                            // Helpers.ConsolePrint("WinDivertSharp", String.Join(",", InboundPorts).Split(':')[0]);
                            // Helpers.ConsolePrint("WinDivertSharp", String.Join(",", InboundPorts).Split(':')[1]);

                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") <- packet: " + PacketPayloadData);
                            }
                            modified = false;
                            goto sendPacket;
                        }


sendPacket:
                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                        if (modified)
                        {
                            WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.All);
                        }
                        /*
                        string cpacket1 = "";
                        for (int i = 0; i < readLen; i++)
                        {
                            // if (packet[i] >= 32)
                            cpacket1 = cpacket1 + (char)packet[i];

                        }
                        //if (cpacket1.Length > 100)
                            File.WriteAllText(np.ToString() + "new-" + addr.Direction.ToString() + ".pkt", cpacket1);
                            */

                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);

                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                            /*
                            Helpers.ConsolePrint("WinDivertSharp", "After Src: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
"Dst: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
" DevFeePort: " + Divert.SwapOrder(DevFeePort).ToString() +
" Direction: " + addr.Direction.ToString() +
" CheckParityConnections(processId, parse_result.TcpHeader->SrcPort): " + CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction));
*/
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                            /*
                            Helpers.ConsolePrint("WinDivertSharp", "After Src: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
" Dst: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
" DevFeePort: " + Divert.SwapOrder(DevFeePort).ToString() +
" Direction: " + addr.Direction.ToString() +
" CheckParityConnections(processId, parse_result.TcpHeader->SrcPort): " + CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction));
*/
                        }

                        if (!OwnerPID.Contains("nbminer"))
                        {
                            if (addr.Direction == WinDivertDirection.Outbound)
                            {
                                stratumRatio--;
                            }
                            else
                            {
                                stratumRatio = stratumRatio + 2;
                            }
                        }

                        if (!WinDivert.WinDivertSend(handle, packet, readLen, ref addr))
                        {
                              Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
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
                //GC.Collect();
                Thread.Sleep(1);
            }
            while (divert_running);

        }
    }
}
