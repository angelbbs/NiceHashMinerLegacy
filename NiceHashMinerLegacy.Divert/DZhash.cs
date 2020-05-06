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

        private static string DivertIP_Proxy1 = "";
        private static ushort DivertPort_Proxy1 = 0;
        private static ushort DivertPort_Proxy2 = 0;

        private static string filter = "";

        private static string DivertLogin = "";
        private static string DivertLogin1 = "";
        private static string DivertLogin2 = "";

        private static string PacketPayloadData;
        private static string OwnerPID = "-1";
        private static int stratumRatio = 0;
        private static List<string> _oldPorts = new List<string>();

        private static WinDivertBuffer newpacket = new WinDivertBuffer();
        private static WinDivertParseResult parse_result;
        private static int PacketLen;
        private static string RemoteIP;
        private static bool sslFixed = false;
        private static bool noPayload = true;
        
        [HandleProcessCorruptedStateExceptions]
        public static IntPtr ZhashDivertStart(List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
            {
            Divert.Zhashdivert_running = true;

            DivertLogin1 = "GeKYDPRcemA3z9okSUhe9DdLQ7CRhsDBgX";
            DivertLogin2 = "angelbbs";

            DivertIP1 = Divert.DNStoIP("btg.2miners.com");
            DivertPort1 = 4040;

            DivertIP_Proxy1 = variables.ProxyIP;
            DivertPort_Proxy1 = 3001;//ssl
            DivertPort_Proxy2 = 3013;//no ssl

            filter = "(!loopback && outbound ? (tcp.DstPort == 4040 || tcp.DstPort == 20595 || tcp.DstPort == 8817 || tcp.DstPort == 14040 ||" +
                "tcp.DstPort == 3001 || tcp.DstPort == 3013)" +
                " : " +
                "(tcp.SrcPort == 4040 || tcp.SrcPort == 20595 || tcp.SrcPort == 8817 || tcp.SrcPort == 14040 || " +
                "tcp.SrcPort == 3001 || tcp.SrcPort == 3013)" +
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

                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                        
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

                        if (addr.Direction == WinDivertDirection.Outbound )
                        {
                            count++;
                            Helpers.ConsolePrint("WinDivertSharp", "COUNT = " + count.ToString());
                            if (count > 5)
                            {
                                Divert.Zhashdivert_running = false;
                                WinDivert.WinDivertClose(DivertHandle);
                                break;
                            }

                            //список соответствия src port и dst ip
                            if(!Divert.CheckSrcPort(InboundPorts, Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString()))
                            {
                                InboundPorts.Add(Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                ":" + parse_result.IPv4Header->DstAddr.ToString());
                            }

                            if (OwnerPID.Contains("miniz") &&
                                (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 14040)
                                ) //ssl
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

                            if (OwnerPID.Contains("miniz") && (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20595)
                                ) 
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

                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 4040) 
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
 
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8817)
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

                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20595) //poolhub
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                Helpers.ConsolePrint("WinDivertSharp",
                                "(" + OwnerPID.ToString() + ") -> Devfee connection to (" +
                                DevFeeIP + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort) + ")");
                                DevFeePort = Divert.SwapOrder(parse_result.TcpHeader->DstPort);
                                DivertLogin = DivertLogin2;
                                DivertIP = DevFeeIP;
                                DivertPort = Divert.SwapOrder(parse_result.TcpHeader->DstPort);
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
                            if (parse_result.PacketPayloadLength > 20)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                
                                if (!sslFixed && OwnerPID.Contains("miniz") && !PacketPayloadData.Contains("method"))
                                {
                                    sslFixed = true;
                                    packet.Dispose();
                                    goto nextCycle;
                                }
                                
                                goto Divert;//меняем данные в пакете
                            }
                            else
                            {
                                goto changeSrcDst; //пакет пустой, меняем адреса
                            }
                        }

                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction, _oldPorts);
                        }
                        else
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction, _oldPorts);
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

                        Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") Unknown connection");
                      //  "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                      //  "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                       // " len: " + readLen.ToString() + " packetLength: " + parse_result.PacketPayloadLength.ToString());
                        modified = false;
                        goto sendPacket;

                        //********************************перехват
Divert:
                        PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);

                        PacketLen = (int)parse_result.PacketPayloadLength;

                        //обход модификации пакета. Иначе пул проглатывает шары
                        if (addr.Direction == WinDivertDirection.Outbound)
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction, _oldPorts);
                        }
                        else
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction, _oldPorts);
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

                            if (PacketPayloadData.Contains("mining.subscribe"))
                            {
                                dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                                json.@params[2] = "dmz";
                                PacketPayloadData = JsonConvert.SerializeObject(json).Replace(" ", "") + (char)10;
                                goto changePayloadData;
                            }
                            //{"id":2,"method":"mining.authorize","params":["GZdx44gPVFX7GfeWXA3kyiuXecym3CWGHi","x"]} 
                            //{"id":2,"method":"mining.authorize","params":["Gby56j47mWjEmT1bsku7VBpJEApsDigpDp.miniZ151tw100","x"]}
                            //{"id":1,"method":"mining.subscribe","params":["miniZ/1.5t2", null,"fee.server","0"]}
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
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8817)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "MiniZ login detected");
                                PacketPayloadData = "{\"id\":2,\"method\":\"mining.authorize\",\"params\":[\"" + DivertLogin + "\",\"x\"]}" + (char)10;
                                goto changePayloadData;
                            }

                            if (PacketPayloadData.Contains("mining.authorize") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 20595)
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "GMiner login detected to equihash-hub.miningpoolhub.com");
                                PacketPayloadData = "{\"id\":2,\"method\":\"mining.authorize\",\"params\":[\"" + DivertLogin + "\",\"x\"]}" + (char)10;
                                goto changePayloadData;
                            }
                            //{"id":4,"method":"mining.submit","params":["Gby56j47mWjEmT1bsku7VBpJEApsDigpDp.miniZ151tw100","393469427744661","b3ba805e","0000000000000000000000440000000000000000cd000000","640696b268040a02338c036b3545885c1b8482ed73b9091b03020b1e3ce2ee65d3aa8597c74e030fa7d794b389bb0c6193a14c0ec5a9e1957f55a6ea747ec6b1c15e6340014198ff535ef581109fe6baefc95bcad437d633e1823382f37fadda75e9b95767"]}
                            if (PacketPayloadData.Contains("mining.submit"))
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
                            // "-> New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());

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
                                 "DevFee SrcPort: " + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                // "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
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
                                count = -1;
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                //Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") <- packet: " + PacketPayloadData);
                            }
                            //modified = false;
                            goto sendPacket;
                        }


sendPacket:
                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                        /*
                        if (modified)
                        {
                            WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.All);
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
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction, _oldPorts);
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
                Thread.Sleep(5);
            }
            while (Divert.Zhashdivert_running);
            Helpers.ConsolePrint("WinDivertSharp", "DZhash stopped");
        }
    }
}
