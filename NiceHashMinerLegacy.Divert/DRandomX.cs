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
    public class DRandomX
    {
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
        private static uint ackn = 0;
        private static uint sn = 0;

        private unsafe static void PrintPacketSettings(WinDivertParseResult parse_result)
        {
            var dstaddr = parse_result.IPv4Header->DstAddr;
            var dstport = parse_result.TcpHeader->DstPort;

            var srcaddr = parse_result.IPv4Header->SrcAddr;
            var srcport = parse_result.TcpHeader->SrcPort;

            var hdrlen = parse_result.IPv4Header->HdrLength;
            //var len = parse_result.IPv4Header->Length;

            var ver = parse_result.IPv4Header->Version;
            var ihl = parse_result.IPv4Header->HdrLength;
            var tos = parse_result.IPv4Header->TOS;
            var tl = parse_result.IPv4Header->Length;
            var id = parse_result.IPv4Header->Id;
            var df = parse_result.IPv4Header->Df;
            var fo = parse_result.IPv4Header->FragOff;

            var ttl = parse_result.IPv4Header->TTL;
            var protocol = parse_result.IPv4Header->Protocol;

            var ack = parse_result.TcpHeader->Ack;
            var ackn = parse_result.TcpHeader->AckNum;
            var fin = parse_result.TcpHeader->Fin;
            var psh = parse_result.TcpHeader->Psh;
            var rst = parse_result.TcpHeader->Rst;
            var sn = parse_result.TcpHeader->SeqNum;
            var syn = parse_result.TcpHeader->Syn;
            var urg = parse_result.TcpHeader->Urg;
            var urgp = parse_result.TcpHeader->UrgPtr;
            var wind = parse_result.TcpHeader->Window;

            Helpers.ConsolePrint("WinDivertSharp", "dst: " + dstaddr + ":" + dstport);
            Helpers.ConsolePrint("WinDivertSharp", "dst: " + srcaddr + ":" + srcport);
            Helpers.ConsolePrint("WinDivertSharp", "hdrlen: " + hdrlen);
            //Helpers.ConsolePrint("WinDivertSharp", "len: " + Divert.SwapOrder(len));
            Helpers.ConsolePrint("WinDivertSharp", "ver: " + ver);
            Helpers.ConsolePrint("WinDivertSharp", "ihl: " + ihl);
            Helpers.ConsolePrint("WinDivertSharp", "tos: " + tos);
            Helpers.ConsolePrint("WinDivertSharp", "tl: " + Divert.SwapOrder(tl));
            Helpers.ConsolePrint("WinDivertSharp", "id: " + Divert.SwapOrder(id));
            Helpers.ConsolePrint("WinDivertSharp", "df: " + df);
            Helpers.ConsolePrint("WinDivertSharp", "fo: " + fo);
            Helpers.ConsolePrint("WinDivertSharp", "ttl: " + ttl);
            Helpers.ConsolePrint("WinDivertSharp", "protocol: " + protocol);

            Helpers.ConsolePrint("WinDivertSharp", "ack: " + ack);
            Helpers.ConsolePrint("WinDivertSharp", "ackn: " + Divert.SwapOrder(ackn));
            Helpers.ConsolePrint("WinDivertSharp", "fin: " + fin);
            Helpers.ConsolePrint("WinDivertSharp", "psh: " + psh);
            Helpers.ConsolePrint("WinDivertSharp", "rst: " + rst);
            Helpers.ConsolePrint("WinDivertSharp", "sn: " + Divert.SwapOrder(sn));
            Helpers.ConsolePrint("WinDivertSharp", "syn: " + syn);
            Helpers.ConsolePrint("WinDivertSharp", "urg: " + urg);
            Helpers.ConsolePrint("WinDivertSharp", "urgp: " + urgp);
            Helpers.ConsolePrint("WinDivertSharp", "wind: " + wind);
        }


        internal static string CheckParityConnections(List<string> processIdList, ushort Port, WinDivertDirection dir)
        {
            /*
            if (String.Join(" ", processIdList).Contains("gminer: force"))
            {
                return "gminer: force";
            }
            */
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
                    }
                        _allConnections.Clear();
                        _allConnections = null;
                        return miner + ": " + ret;
                    }
                }
            for (int i = 1; i < _oldPorts.Count; i++)
            {
                if (String.Join(" ", _oldPorts).Contains(Port.ToString()))
                {
                    return "unknown: ?";
                }
            }

            _allConnections.Clear();
            _allConnections = null;

            return "-1";
        }

        [HandleProcessCorruptedStateExceptions]
        public static IntPtr RandomXDivertStart(List<string> processIdList, int CurrentAlgorithmType, string MinerName)
            {
            Divert.RandomXdivert_running = true;

            DivertLogin1 = "42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.donate12";

            DivertIP1 = Divert.DNStoIP("pool.supportxmr.com");
            DivertPort1 = 3333;

filter = "(!loopback && outbound ? (tcp.DstPort == 14444 ||tcp.DstPort == 3333)" +
                " : " +
                "(tcp.SrcPort == 14444 || tcp.SrcPort == 3333)" +
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

            RunDivert(DivertHandle, processIdList, CurrentAlgorithmType, MinerName);

            return DivertHandle;
        }

        [HandleProcessCorruptedStateExceptions]
        internal static Task<bool> RunDivert(IntPtr handle, List<string> processIdList, int CurrentAlgorithmType, string MinerName)
        {

            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                RunDivert1(handle, processIdList, CurrentAlgorithmType, MinerName);
                return t.Task;
            });
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HandleProcessCorruptedStateExceptions]
        internal unsafe static async Task RunDivert1(IntPtr handle, List<string> processIdList, int CurrentAlgorithmType, string MinerName)
        {
            var packet = new WinDivertBuffer();
            var addr = new WinDivertAddress();
            int np = 1;
            uint readLen = 0;
            List<string> InboundPorts = new List<string>();

        //Span<byte> packetData = null;

            IntPtr recvEvent = IntPtr.Zero;
            bool modified = false;
            bool result;

            do
            {
                try
                {
nextCycle:
                    if (Divert.RandomXdivert_running)
                    {
                        readLen = 0;
                        modified = false;
                        PacketPayloadData = null;
                        packet.Dispose();

                        addr.Reset();

                        packet = new WinDivertBuffer();
                        result = WinDivert.WinDivertRecv(handle, packet, ref addr, ref readLen);

                        if (!result)
                        {
                            {

                                Divert.RandomXdivert_running = false;
                                Helpers.ConsolePrint($"WinDivertSharp", "WinDivertRecv error.");
                                continue;
                            }
                        }

                        np++;
                        
                        string cpacket0 = "";
                        for (int i = 0; i < readLen; i++)
                        {
                           // if (packet[i] >= 32)
                                cpacket0 = cpacket0 + (char)packet[i];

                        }
                        //if (cpacket0.Length > 60)
                        File.WriteAllText(np.ToString()+ "old-" + addr.Direction.ToString() + ".pkt", cpacket0);

                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                        /*
                        if (parse_result.TcpHeader->Window == 516)
                        {
                            parse_result.TcpHeader->Window = 519;
                        }
                        */
                        Helpers.ConsolePrint("WinDivertSharp", "BEFORE----------------------------------------------");
                        Helpers.ConsolePrint("WinDivertSharp", "Direction: " + addr.Direction.ToString());
                        PrintPacketSettings(parse_result);



                        if (addr.Direction == WinDivertDirection.Outbound && parse_result != null && processIdList != null)
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        }


                        if (addr.Direction == WinDivertDirection.Outbound )
                        {
                            //список соответствия src port и dst ip
                            if(!Divert.CheckSrcPort(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString()))
                            {
                                InboundPorts.Add(Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                ":" + parse_result.IPv4Header->DstAddr.ToString());
                            }

                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 9999) //??
                            {
                                packet.Dispose();
                                Helpers.ConsolePrint("WinDivertSharp", "BLOCK SSL for testing");
                                goto nextCycle;
                            }

                                if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333 ||
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 14444) //
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


                        }

                        //*************************************************************************************************************
                        parsePacket:

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
                            !OwnerPID.Equals("-1")
                            )
                        {
                            ackn = Divert.SwapOrder(parse_result.TcpHeader->AckNum);
                            Helpers.ConsolePrint("WinDivertSharp", "ackn-sn = " + (ackn - sn).ToString());
                            
                            if (ackn - sn == 508)
                            {
                                // Helpers.ConsolePrint("WinDivertSharp", "ackn - sn == 508");
                                var nackn = ackn - 2;
                                //parse_result.TcpHeader->AckNum = Divert.SwapOrder(nackn);
                                parse_result.TcpHeader->AckNum = Divert.SwapOrder(4386);//обнуляется и нихрена не работает
                                parse_result.TcpHeader->Ack = 1;
                               // parse_result.TcpHeader->Syn = 1;
                                Helpers.ConsolePrint("WinDivertSharp", "new ackn = " + Divert.SwapOrder(parse_result.TcpHeader->AckNum));
                            }
                            
                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                /*
                                var modpacket = Divert.MakeNewPacket(packet, readLen, PacketPayloadData);
                                packet.Dispose();
                                packet = modpacket;
                                readLen = packet.Length;
                                WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.NoIpChecksum);
                                */
                                goto changeSrcDst; //входящее соединение, только меняем адреса
                            }
                            else
                            {
                                goto changeSrcDst; //пакет пустой, меняем адреса
                            }
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
                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        }

                        if(PacketPayloadData == null)
                        {
                            goto changeSrcDst;
                        }

                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound)
                        {
                            if (PacketPayloadData.Contains("login") && !OwnerPID.Equals("-1"))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "*** mining.submit");
                                goto modifyData;
                            }
                        }
                        goto changeSrcDst;
modifyData:
                        if (parse_result.PacketPayloadLength > 1 & addr.Direction == WinDivertDirection.Outbound &
                           !OwnerPID.Equals("-1"))
                        {
                            modified = false;
                            //dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") packet: " + PacketPayloadData);

                            //
                            //{"id":2,"method":"mining.authorize","params":["GZdx44gPVFX7GfeWXA3kyiuXecym3CWGHi","x"]} 

                            
                            //XMRig
                            //if (CurrentAlgorithmType == 47 && MinerName.ToLower() == "xmrig" && PacketPayloadData.Contains("jsonrpc") & PacketPayloadData.Contains("method") & PacketPayloadData.Contains("login") & PacketPayloadData.ToLower().Contains("params") & PacketPayloadData.ToLower().Contains("xmrig"))
                            if (CurrentAlgorithmType == 47 && MinerName.ToLower() == "xmrig" &&
                                PacketPayloadData.Contains("login"))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "Xmrig login detected");
                                dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                                json.@params.login = DivertLogin;
                               // json.@params.agent = "XMRig";
                                PacketPayloadData = JsonConvert.SerializeObject(json) + (char)10;
                                goto changePayloadData;
                            }

                            

                            changePayloadData:
                            //*****************************
                            /*
                            byte[] head = new byte[40];
                            for (int i = 0; i < 40; i++)
                            {
                                head[i] = (byte)packet[i];
                            }

                            byte[] newPayload = new byte[PacketPayloadData.Length];
                            for (int i = 0; i < PacketPayloadData.Length; i++)
                            {
                                newPayload[i] = (byte)PacketPayloadData[i];
                            }

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
                            */

                            var modpacket = Divert.MakeNewPacket(packet, readLen, PacketPayloadData);
                            packet.Dispose();
                            packet = modpacket;
                            readLen = packet.Length;
                            WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.NoIpChecksum);


                            //packet.Dispose();
                            //packet = new WinDivertBuffer(modpacket);
                            //packet = new WinDivertBuffer(newPayload);

                                //parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                                //WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.NoIpChecksum);
                                //WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.All);
                                //WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.NoIpChecksum);
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                            string cpacket5 = "";
                            for (int i = 0; i < packet.Length; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket5 = cpacket5 + (char)packet[i];

                            }
                            // if (cpacket4.Length > 100)
                            File.WriteAllText(np.ToString() + "ip4.pkt", cpacket5);
                            //////////////////////****************************************

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
                            parse_result.TcpHeader->SrcPort = DevFeePort;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            RemoteIP = Divert.GetRemoteIP(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());
                            parse_result.IPv4Header->SrcAddr = IPAddress.Parse(RemoteIP);
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") "
                                + "<- New DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                "  New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());

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
                        /*
                        addr.Reset();
                        addr = new WinDivertAddress();
                        */
                        if (modified)
                        //if (addr.Direction == WinDivertDirection.Inbound)
                        {
                            WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.All);
                        }
                        
                        string cpacket1 = "";
                        for (int i = 0; i < readLen; i++)
                        {
                            // if (packet[i] >= 32)
                            cpacket1 = cpacket1 + (char)packet[i];

                        }
                        if (cpacket1.Length > 100)
                            File.WriteAllText(np.ToString() + "new-" + addr.Direction.ToString() + ".pkt", cpacket1);


                        if (parse_result.IPv4Header->Length == Divert.SwapOrder(548))
                        {
                            //parse_result.IPv4Header->Length = Divert.SwapOrder(546);
                            // parse_result.TcpHeader->Window = (516);
                            //parse_result.PacketPayloadLength = 506;
                             //Helpers.ConsolePrint("WinDivertSharp", "CHANGED parse_result.IPv4Header->Length: " + Divert.SwapOrder(parse_result.IPv4Header->Length));
                            //Helpers.ConsolePrint("WinDivertSharp", "CHANGED parse_result.TcpHeader->Window: " + (parse_result.TcpHeader->Window));
                           // parse_result.TcpHeader->AckNum = Divert.SwapOrder(287454020);
                            //parse_result.TcpHeader->AckNum = parse_result.TcpHeader->AckNum-2;
                            //parse_result.TcpHeader->Ack = 0;
                        }


                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                        //parse_result.TcpHeader->SeqNum = (uint)parse_result.TcpHeader->AckNum;

                        //parse_result.TcpHeader->AckNum = Divert.SwapOrder(287454020);
                        //parse_result.IPv4Header->TTL = 128;
                        // if (parse_result.TcpHeader->Window >= 510 || parse_result.TcpHeader->Window <= 530)
                        {
                       //     parse_result.TcpHeader->Window = 1200;
                        }
                        //parse_result.TcpHeader->Ack = 1;
                        //parse_result.TcpHeader->Syn = 0;
                        Helpers.ConsolePrint("WinDivertSharp", "AFTER----------------------------------------------");
                        Helpers.ConsolePrint("WinDivertSharp", "Direction: " + addr.Direction.ToString());
                        PrintPacketSettings(parse_result);
                        /*
                        if (addr.Direction == WinDivertDirection.Inbound)
                        {
                            var dstaddr = parse_result.IPv4Header->DstAddr;
                            var dstport = parse_result.TcpHeader->DstPort;

                            var srcaddr = parse_result.IPv4Header->SrcAddr;
                            var srcport = parse_result.TcpHeader->SrcPort;

                            var hdrlen = parse_result.IPv4Header->HdrLength;
                            var len = parse_result.IPv4Header->Length;

                            var ver = parse_result.IPv4Header->Version;
                            var ihl = parse_result.IPv4Header->HdrLength;
                            var tos = parse_result.IPv4Header->TOS;
                            var tl = parse_result.IPv4Header->Length;
                            var id = parse_result.IPv4Header->Id;
                            var df = parse_result.IPv4Header->Df;
                            var fo = parse_result.IPv4Header->FragOff;

                            var ttl = parse_result.IPv4Header->TTL;
                            var protocol = parse_result.IPv4Header->Protocol;

                            var ack = parse_result.TcpHeader->Ack;
                            var ackn = parse_result.TcpHeader->AckNum;
                            var fin = parse_result.TcpHeader->Fin;
                            var psh = parse_result.TcpHeader->Psh;
                            var rst = parse_result.TcpHeader->Rst;
                            var sn = parse_result.TcpHeader->SeqNum;
                            var syn = parse_result.TcpHeader->Syn;
                            var urg = parse_result.TcpHeader->Urg;
                            var urgp = parse_result.TcpHeader->UrgPtr;
                            var wind = parse_result.TcpHeader->Window;

                            Helpers.ConsolePrint("WinDivertSharp", "<-ver: " + ver);
                            Helpers.ConsolePrint("WinDivertSharp", "<-ihl: " + ihl);
                            Helpers.ConsolePrint("WinDivertSharp", "<-tos: " + tos);
                            Helpers.ConsolePrint("WinDivertSharp", "<-tl: " + Divert.SwapOrder(tl));
                            Helpers.ConsolePrint("WinDivertSharp", "<-id: " + Divert.SwapOrder(id));
                            Helpers.ConsolePrint("WinDivertSharp", "<-df: " + df);
                            Helpers.ConsolePrint("WinDivertSharp", "<-fo: " + fo);
                            Helpers.ConsolePrint("WinDivertSharp", "<-ttl: " + ttl);
                            Helpers.ConsolePrint("WinDivertSharp", "<-protocol: " + protocol);

                            Helpers.ConsolePrint("WinDivertSharp", "<-ack: " + ack);
                            Helpers.ConsolePrint("WinDivertSharp", "<-ackn: " + ackn);
                            Helpers.ConsolePrint("WinDivertSharp", "<-fin: " + fin);
                            Helpers.ConsolePrint("WinDivertSharp", "<-psh: " + psh);
                            Helpers.ConsolePrint("WinDivertSharp", "<-rst: " + rst);
                            Helpers.ConsolePrint("WinDivertSharp", "<-sn: " + sn);
                            Helpers.ConsolePrint("WinDivertSharp", "<-syn: " + syn);
                            Helpers.ConsolePrint("WinDivertSharp", "<-urg: " + urg);
                            Helpers.ConsolePrint("WinDivertSharp", "<-urgp: " + urgp);
                            Helpers.ConsolePrint("WinDivertSharp", "<-wind: " + wind);
                        }
                        */
                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);

                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            sn = Divert.SwapOrder(parse_result.TcpHeader->SeqNum);//сохранить
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

                        if (!WinDivert.WinDivertSend(handle, packet, readLen, ref addr))
                        {
                              Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
                        }
                    }
                } catch (Exception e)
                {
                    Helpers.ConsolePrint("WinDivertSharp error: ", e.ToString());
                    Thread.Sleep(500);
                }
                finally
                {

                }
                Thread.Sleep(1);
            }
            while (Divert.RandomXdivert_running);

        }
    }
}
