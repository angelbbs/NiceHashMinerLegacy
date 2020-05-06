﻿/*
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
    public class DGrin
    {
        private static IntPtr DivertHandle;
        private static readonly uint MaxPacket = 2048;

        private static string DevFeeIP = "";
        private static ushort DevFeePort = 0;

        private static string DivertIP = "";
        private static ushort DivertPort = 0;

        private static string DivertIP_Proxy1 = "";
        private static ushort DivertPort_Proxy1 = 0;
        private static ushort DivertPort_Proxy2 = 0;

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
        private static string DivertLogin2 = "";
        private static string DivertLogin3 = "";

        private static string PacketPayloadData;
        private static string OwnerPID = "-1";
        private static int stratumRatio = 0;
        private static List<string> _oldPorts = new List<string>();

        private static WinDivertBuffer newpacket = new WinDivertBuffer();
        private static WinDivertParseResult parse_result;
        private static int PacketLen;
        private static string RemoteIP;
        private static bool noPayload = true;

       
        [HandleProcessCorruptedStateExceptions]
        public static IntPtr GrinDivertStart(List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
            {
            Divert.Grindivert_running = true;

            DivertIP_Proxy1 = variables.ProxyIP;
            DivertPort_Proxy1 = 3011;
            DivertPort_Proxy2 = 3003;//ssl

            filter = "(!loopback && outbound ? (tcp.DstPort == 3030 || tcp.DstPort == 4040 || tcp.DstPort == 6666" +
                " || tcp.DstPort == 8443 || tcp.DstPort == 13654 || " +
                "tcp.DstPort == 4416 || tcp.DstPort == 6667 || tcp.DstPort == 13030 || " +
                "tcp.DstPort == 3011 || tcp.DstPort == 3003)" +
                " : " +
                "(tcp.SrcPort == 3030 || tcp.SrcPort == 4040 || tcp.SrcPort == 6666 || tcp.SrcPort == 13654 ||" +
                " tcp.SrcPort == 4416 || tcp.SrcPort == 6667 || tcp.SrcPort == 13030 || " +
                "tcp.SrcPort == 3011 || tcp.SrcPort == 3003)" +
                ")";

            DivertHandle = Divert.OpenWinDivert(filter);
            if (DivertHandle == IntPtr.Zero || DivertHandle == new IntPtr(-1))
            {
                return new IntPtr(-1);
            }
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

            int count = 0;
            IntPtr recvEvent = IntPtr.Zero;
            bool modified = false;
            bool result;

            do
            {
                try
                {
nextCycle:
                    if (Divert.Grindivert_running)
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

                                Divert.Grindivert_running = false;
                                Helpers.ConsolePrint($"WinDivertSharp", "WinDivertRecv error.");
                                continue;
                            }
                        }

                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                        if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) != 8443 & Divert.SwapOrder(parse_result.TcpHeader->SrcPort) != 8443)
                        {
                            np++;
                        }

                        if (Divert._SaveDivertPackets)
                        {
                            if (!Directory.Exists("temp")) Directory.CreateDirectory("temp");
                            string cpacket0 = "";
                            for (int i = 0; i < readLen; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket0 = cpacket0 + (char)packet[i];

                            }
                            if (cpacket0.Length > 60)
                                File.WriteAllText("temp/" + np.ToString() + "old-" + addr.Direction.ToString() + ".pkt", cpacket0);
                        }

                       
                        if (addr.Direction == WinDivertDirection.Outbound && parse_result != null && processIdList != null)
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction, _oldPorts);
                        }
                        else
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction, _oldPorts);
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

                            PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                        //***************
                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            count++;
                            Helpers.ConsolePrint("WinDivertSharp", "Zhash COUNT = " + count.ToString());
                            if (count > 5)
                            {
                                Divert.Zhashdivert_running = false;
                                WinDivert.WinDivertClose(DivertHandle);
                                Divert.processIdListZhash.Clear();
                                Divert.gminer_runningZhash = false;
                                continue;
                            }
                        }
                            //список соответствия src port и dst ip
                            if (!Divert.CheckSrcPort(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString()))
                        {
                            InboundPorts.Add(Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                            ":" + parse_result.IPv4Header->DstAddr.ToString());
                        }

                        if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 6666 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 6667 ||//g31
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3030 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4040) 
                        {
                            DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                            DevFeePort = parse_result.TcpHeader->DstPort;
                            Helpers.ConsolePrint("WinDivertSharp",
                            "(" + OwnerPID.ToString() + ") -> Devfee connection to (" +
                            DevFeeIP + ":" + Divert.SwapOrder(DevFeePort) + ")");

                            DivertIP = DivertIP_Proxy1;
                            DivertPort = Divert.SwapOrder(DivertPort_Proxy1);
                            modified = true;
                            goto changeSrcDst;
                        }
                        if ((Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 13030 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4416) && !Divert._certInstalled)
                        {
                            goto sendPacket;
                        }
                        if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 13030 ||
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4416)
                        {
                            DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                            DevFeePort = parse_result.TcpHeader->DstPort;
                            Helpers.ConsolePrint("WinDivertSharp",
                            "(" + OwnerPID.ToString() + ") -> Devfee connection to (" +
                            DevFeeIP + ":" + Divert.SwapOrder(DevFeePort) + ")");

                            DivertIP = DivertIP_Proxy1;
                            DivertPort = Divert.SwapOrder(DivertPort_Proxy2);
                            modified = true;
                            goto changeSrcDst;
                        }

                        //*************************************************************************************************************


                        if (addr.Direction == WinDivertDirection.Inbound &&
                            //проверку входящего соединения можно упростить
                            //но могут путаться пакеты, если несколько соединений одновременно
                            //parse_result.TcpHeader->SrcPort == DivertPort && parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &&
                            !OwnerPID.Equals("-1")
                            )
                        {
                            if (parse_result.PacketPayloadLength > 16)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                goto changeSrcDst; //входящее соединение, только меняем адреса
                            }
                            else
                            {
                                goto changeSrcDst; //пакет пустой, меняем адреса
                            }
                        }

                        Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") Unknown connection");
                        //"DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                       // "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                       // " len: " + readLen.ToString() + " packetLength: " + parse_result.PacketPayloadLength.ToString());
                        modified = false;
                        goto sendPacket;


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
                                "-> New DevFee port: " + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());
                            //     "-> New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());

                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                            }
                            //modified = false;
                            goto sendPacket;
                        }

                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction, _oldPorts);
                        }
                        else
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction, _oldPorts);
                        }

                        if (parse_result.TcpHeader->SrcPort == DivertPort &&
                                addr.Direction == WinDivertDirection.Inbound &&
                                parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &&
                               !OwnerPID.Equals("-1"))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") DEVFEE SESSION: <- " +
                               // "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                               "DevFee SrcPort: " + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                " len: " + readLen.ToString());
                            parse_result.TcpHeader->SrcPort = DevFeePort;
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            RemoteIP = Divert.GetRemoteIP(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());
                            parse_result.IPv4Header->SrcAddr = IPAddress.Parse(RemoteIP);
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") "
                                + "<- New DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                "  New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());

                            count = -1;
                            goto sendPacket;
                        }

                        sendPacket:
                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                        /*                        
                                                if (modified)
                                                {
                                                    addr.PseudoIPChecksum = true;
                                                    addr.PseudoTCPChecksum = true;
                                                    WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.NoIpChecksum);
                                                }
                          */
                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                        parse_result.TcpHeader->Checksum = 0;
                        var crc = Divert.CalcTCPChecksum(packet, readLen);

                        parse_result.IPv4Header->Checksum = 0;
                        var pIPv4Header = Divert.getBytes(*parse_result.IPv4Header);
                        var crch = Divert.CalcIpChecksum(pIPv4Header, pIPv4Header.Length);

                        parse_result.IPv4Header->Checksum = crch;
                        parse_result.TcpHeader->Checksum = crc;
                        if (Divert._SaveDivertPackets)
                        {
                            if (!Directory.Exists("temp")) Directory.CreateDirectory("temp");
                            string cpacket1 = "";
                            for (int i = 0; i < readLen; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket1 = cpacket1 + (char)packet[i];

                            }
                            if (cpacket1.Length > 60)
                                File.WriteAllText("temp/" + np.ToString() + "new-" + addr.Direction.ToString() + ".pkt", cpacket1);
                        }

                        //OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);

                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction, _oldPorts);
                        }
                        else
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction, _oldPorts);
                        }

                        if (!WinDivert.WinDivertSend(handle, packet, readLen, ref addr))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
                        }
                    }
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("WinDivertSharp error: ", e.ToString());
                    Thread.Sleep(300);
                }
                finally
                {

                }
                Thread.Sleep(1);
            }
            while (Divert.Grindivert_running);

        }
    }
}
