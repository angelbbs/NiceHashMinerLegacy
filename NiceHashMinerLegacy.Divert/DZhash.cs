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
    public class DZhash
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
        public static IntPtr ZhashDivertStart(List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
            {
            Divert.Zhashdivert_running = true;

            DivertLogin1 = "GeKYDPRcemA3z9okSUhe9DdLQ7CRhsDBgX";

            DivertIP1 = Divert.DNStoIP("btg.2miners.com");
            DivertPort1 = 4040;

filter = "(!loopback && outbound ? (tcp.DstPort == 4040 || tcp.DstPort == 20595)" +
                " : " +
                "(tcp.SrcPort == 4040 || tcp.SrcPort == 20595)" +
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

            IntPtr recvEvent = IntPtr.Zero;
            bool modified = false;
            bool result;

            do
            {
                try
                {
nextCycle:
                    if (Divert.Zhashdivert_running)
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

                                Divert.Zhashdivert_running = false;
                                Helpers.ConsolePrint($"WinDivertSharp", "WinDivertRecv error.");
                                continue;
                            }
                        }

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

                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                        
                        if (addr.Direction == WinDivertDirection.Outbound && parse_result != null && processIdList != null)
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        }


                        if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8443 || Divert.SwapOrder(parse_result.TcpHeader->SrcPort) == 8443)
                        {
                            if (Divert.BlockGMinerApacheTomcat)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") Block gminer Apache Tomcat");
                                goto nextCycle;
                            } else
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") Allow gminer Apache Tomcat");
                                modified = false;
                                goto sendPacket;
                            }
                        }

                        if (addr.Direction == WinDivertDirection.Outbound )
                        {
                            //список соответствия src port и dst ip
                            if(!Divert.CheckSrcPort(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString()))
                            {
                                InboundPorts.Add(Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                ":" + parse_result.IPv4Header->DstAddr.ToString());
                            }

                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4040) //2miners
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",
                                "(" + OwnerPID.ToString() +") -> Devfee connection to (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort;
                                DivertLogin = DivertLogin1;
                                DivertIP = DivertIP1;
                                DivertPort = Divert.SwapOrder(DivertPort1);//swap
                                if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4040 && OwnerPID.Contains("gminer"))
                                {
                                    DivertIP = DevFeeIP;
                                }
                                goto parsePacket;
                            }
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20595) //poolhub
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",
                                "(" + OwnerPID.ToString() + ") -> Devfee connection to (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = parse_result.TcpHeader->DstPort;
                                DivertLogin = DivertLogin1;
                                DivertIP = DivertIP1;
                                DivertPort = Divert.SwapOrder(DivertPort1);//swap
                                /*
                                if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20595 && OwnerPID.Contains("gminer"))
                                {
                                    DivertIP = DevFeeIP;
                                }
                                */
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
                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
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
                            if (PacketPayloadData.Contains("eth_submitWork") && !OwnerPID.Equals("-1"))
                            {
                                goto changeSrcDst;
                            }


                            if (PacketPayloadData.Contains("mining.authorize") && !OwnerPID.Equals("-1"))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "*** mining.authorize");
                                goto modifyData;
                            }
                            if (PacketPayloadData.Contains("mining.submit") && !OwnerPID.Equals("-1"))
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

                            if (PacketPayloadData.Contains("mining.authorize") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4040)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "GMiner login detected to 2miners");
                                PacketPayloadData = "{\"id\":2,\"method\":\"mining.authorize\",\"params\":[\"" + DivertLogin + "\",\"x\"]}" + (char)10;
                                goto changePayloadData;
                            }
                            if (PacketPayloadData.Contains("mining.submit") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4040)
                            {
                                dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                                json.@params[0] = DivertLogin;
                                PacketPayloadData = JsonConvert.SerializeObject(json).Replace(" ", "") + (char)10;
                                //{"id":4,"method":"mining.submit","params":["GZdx44gPVFX7GfeWXA3kyiuXecym3CWGHi","902239708713597","118c535e","0000000000000000000000000000000000000000d54d0300","640cba177cab965e79697053169531e66fd2550f0cc0438a96022358d17be9e2ec04d09f437664c5e2277c902edb340fbf73f90e2096acb86c9b59082f4242d16ea2c11c2018e045ed4d75f333f75a67d5315ccf68ff065b541e0017c0b1ee92a3b7ff41b6"]}
                                goto changePayloadData;
                            }
                            
                            if (PacketPayloadData.Contains("mining.authorize") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20595)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "GMiner login detected to equihash-hub.miningpoolhub.com");
                                PacketPayloadData = "{\"id\":2,\"method\":\"mining.authorize\",\"params\":[\"" + DivertLogin + "\",\"x\"]}" + (char)10;
                                goto changePayloadData;
                            }
                            if (PacketPayloadData.Contains("mining.submit") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20595)
                            {
                                dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                                json.@params[0] = DivertLogin;
                                PacketPayloadData = JsonConvert.SerializeObject(json).Replace(" ", "") + (char)10;
                                //{"id":4,"method":"mining.submit","params":["GZdx44gPVFX7GfeWXA3kyiuXecym3CWGHi","902239708713597","118c535e","0000000000000000000000000000000000000000d54d0300","640cba177cab965e79697053169531e66fd2550f0cc0438a96022358d17be9e2ec04d09f437664c5e2277c902edb340fbf73f90e2096acb86c9b59082f4242d16ea2c11c2018e045ed4d75f333f75a67d5315ccf68ff065b541e0017c0b1ee92a3b7ff41b6"]}
                                goto changePayloadData;
                            }

                            changePayloadData:
                            //*****************************
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
                            readLen = packet.Length;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                            head = null;
                            modpacket = null;
                            newPayload = null;

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
                        if (cpacket1.Length > 100)
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
            while (Divert.Zhashdivert_running);

        }
    }
}
