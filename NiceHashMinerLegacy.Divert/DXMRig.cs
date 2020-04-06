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
    public class DXMrig
    {
       // private static Timer _divertTimer;
        private static volatile bool divert_running = true;
        private static IntPtr DivertHandle;
        private static readonly uint MaxPacket = 2048;

        private static string DevFeeIP = "";
        private static string DevFeeIPName = "";
        private static ushort DevFeePort = 0;

        private static string DevFeeIP1 = "";
        private static string DevFeeIPName1 = "";
        private static ushort DevFeePort1 = 0;

        private static string DevFeeIP2 = "";
        private static string DevFeeIPName2 = "";
        private static ushort DevFeePort2 = 0;

        private static string DevFeeIP3 = "";
        private static string DevFeeIPName3 = "";
        private static ushort DevFeePort3 = 0;

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

        private static string NicehashDstIP;
        private static ushort NicehashSrcPort;
        private static ushort NicehashPort;

        private static string filter = "";

        private static string DivertLogin = "";
        private static string DivertLogin1 = "";
        private static string DivertLogin2 = "";

        private static string PacketPayloadData;
        private static string packetdata;
        private static List<Connection> _allConnections = new List<Connection>();
        public static bool logging;
        private static string processName = "";
        private static bool noPayload = true;

        private static bool CheckParityConnections(int processId, ushort SrcPort)
        {
            _allConnections.Clear();
            _allConnections.AddRange(NetworkInformation.GetTcpV4Connections());

            //Helpers.ConsolePrint("WinDivertSharp", "processId: " + processId.ToString() + " SrcPort: " + SrcPort.ToString());
            //Helpers.ConsolePrint("WinDivertSharp", "NetworkInformation.GetTcpV4Connections(): " + NetworkInformation.GetTcpV4Connections());
            for (int i = 0; i < _allConnections.Count; i++)
            {
                //Helpers.ConsolePrint("WinDivertSharp", "CheckParityConnections: " + _allConnections[i].OwningPid.ToString() + " " + _allConnections[i].LocalEndPoint.ToString());
                if (_allConnections[i].OwningPid == processId &
                    _allConnections[i].LocalEndPoint.ToString().Contains(SrcPort.ToString()) &
                    processName.ToLower() == "xmrig.exe")
                {
                    return true;
                }
            }
            /*
            if (_allConnections.Find(c => c.OwningPid == processId) != null && _allConnections.Find(c => c.LocalEndPoint.Port == Divert.SwapOrder(SrcPort)) != null)
            {
                return true;
            }
            */
            return false;
        }

        [HandleProcessCorruptedStateExceptions]
        public static IntPtr XMRigDivertStart(int processId, int CurrentAlgorithmType, string MinerName)
            {
            divert_running = true;
            Helpers.ConsolePrint("WinDivertSharp", "Divert START for process ID: " + processId.ToString() + " Miner: " + MinerName + " CurrentAlgorithmType: " + CurrentAlgorithmType);

                DevFeeIP1 = "";
                DevFeeIPName1 = "pool.supportxmr.com";
                DevFeePort1 = 3333;
                
                DivertIP = "";
                // DivertIPName = "randomxmonero.in.nicehash.com";
                // DivertPortCheckParityConnections= 3380;
                DivertIPName1 = "pool.supportxmr.com";
                DivertPort1 = 3333;

                //DivertIPName1 = "xmr-eu1.nanopool.org";
                //DivertPort1 = 14444;

                NicehashPort = 3380; //порт основного соединения

                DivertLogin1 = "42fV4v2EC4EALhKWKNCEJsErcdJygynt7RJvFZk8HSeYA9srXdJt58D9fQSwZLqGHbijCSMqSP4mU7inEEWNyer6F7PiqeX.divert";
                filter = "ip && tcp && (inbound ? (tcp.SrcPort == 3380 || tcp.SrcPort == 3333 || tcp.SrcPort == 14444) : (tcp.DstPort == 3380 || tcp.DstPort == 3333 || tcp.DstPort == 14444))";//xmr

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
            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueTime, 8000);
            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueSize, 33554432);

            RunDivert(DivertHandle, processId, CurrentAlgorithmType, MinerName);

            /*
            var threads = new List<Thread>();

            for (int i = 0; i < Environment.ProcessorCount; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    RunDiversion(handle);
                }));

                threads.Last().Start();
            }

            foreach (var dt in threads)
            {
                dt.Join();
            }
            
            WinDivert.WinDivertClose(handle);
            */

            /*
                        while (true)
                        {
                            ProcessTest(s_testData.UpperWinDivertHandle, "(tcp? tcp.DstPort == 80: true) and (udp? udp.DstPort == 80: true)",
             TestData.EchoRequestData, true);
                        }
                        */
            Helpers.ConsolePrint("WinDivertSharp", "Divert handle: " + DivertHandle.ToString());
            return DivertHandle;
        }

        private static Task<bool> RunDivert(IntPtr handle, int processId, int CurrentAlgorithmType, string MinerName)
        {

            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                RunDivert1(handle, processId, CurrentAlgorithmType, MinerName);
                return t.Task;
            });
        }

        // Calculates the TCP checksum using the IP Header and TCP Header.
        // Ensure the TCPHeader contains an even number of bytes before passing to this method.
        // If an odd number, pad with a 0 byte just for checksumming purposes.
        static ushort GetTCPChecksum(byte[] IPHeader, byte[] TCPHeader)
        {
            uint sum = 0;
            // TCP Header
            for (int x = 0; x < TCPHeader.Length; x += 2)
            {
                sum += ntoh(BitConverter.ToUInt16(TCPHeader, x));
            }
            // Pseudo header - Source Address
            sum += ntoh(BitConverter.ToUInt16(IPHeader, 12));
            sum += ntoh(BitConverter.ToUInt16(IPHeader, 14));
            // Pseudo header - Dest Address
            sum += ntoh(BitConverter.ToUInt16(IPHeader, 16));
            sum += ntoh(BitConverter.ToUInt16(IPHeader, 18));
            // Pseudo header - Protocol
            sum += ntoh(BitConverter.ToUInt16(new byte[] { 0, IPHeader[9] }, 0));
            // Pseudo header - TCP Header length
            sum += (UInt16)TCPHeader.Length;
            // 16 bit 1's compliment
            while ((sum >> 16) != 0) { sum = ((sum & 0xFFFF) + (sum >> 16)); }
            sum = ~sum;
            return (ushort)ntoh((UInt16)sum);
        }

        private static ushort ntoh(UInt16 In)
        {
            int x = IPAddress.NetworkToHostOrder(In);
            return (ushort)(x >> 16);
        }
        private static byte[] getBytes(object str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }
        public static ushort ComputeHeaderIpChecksum(byte[] header, int length)
        {
            ushort word16;
            long sum = 0;
            for (int i = 0; i < length; i += 2)
            {
                word16 = (ushort)(((header[i] << 8) & 0xFF00) + (header[i + 1] & 0xFF));
                sum += word16;
            }

            while ((sum >> 16) != 0)
            {
                sum = (sum & 0xFFFF) + (sum >> 16);
            }

            return (ushort)~sum;
        }
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe static async Task RunDivert1(IntPtr handle, int processId, int CurrentAlgorithmType, string MinerName)
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
            processName = Divert.GetProcessName(processId);
            WinDivertBuffer newpacket = new WinDivertBuffer();

            do
            {
                try
                {
                    // Thread.Sleep(10);
                    if (divert_running)
                    {
                        packetData = null;
                        readLen = 0;
                        modified = false;

                        recvAsyncIoLen = 0;
                        recvOverlapped = new NativeOverlapped();

                        recvEvent = Kernel32.CreateEvent(IntPtr.Zero, false, false, IntPtr.Zero);
                        //recvOverlapped.EventHandle = recvEvent;

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
                                //if (DivertHandle == IntPtr.Zero || DivertHandle == new IntPtr(-1) || DivertHandle == null)
                                {
                                    divert_running = false;
                                }
                                Helpers.ConsolePrint($"WinDivertSharp", "Unknown IO error ID {0} while awaiting overlapped result. DivertHandle="+ DivertHandle.ToString());
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

                        string cpacket0 = "";
                        for (int i = 0; i < readLen; i++)
                        {
                           // if (packet[i] >= 32)
                                cpacket0 = cpacket0 + (char)packet[i];

                        }
                        //if (cpacket0.Length > 60)
                        File.WriteAllText(np.ToString()+ "old-" + addr.Direction.ToString() + ".pkt", cpacket0);

                        if (noPayload && np > 8)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "Stop divert");
                            modified = false;
                            goto sendPacket;
                        }

                        var parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                        //parse_result.TcpHeader->Checksum = 48875;
                        //**+++++++++++++++++++++++++
                        var crc = GetTCPChecksum(getBytes(*parse_result.IPv4Header), getBytes(*parse_result.TcpHeader));
                        Helpers.ConsolePrint("WinDivertSharp", ": " + np + " " + crc);
                        Helpers.ConsolePrint("WinDivertSharp", "old TcpHeader->HdrLength: " + np + " " + (parse_result.TcpHeader->HdrLength));
                        Helpers.ConsolePrint("WinDivertSharp", "old IPv4Header->Length: " + np + " " + (Divert.SwapOrder(parse_result.IPv4Header->Length)));
                        Helpers.ConsolePrint("WinDivertSharp", "old TcpHeader->Checksum: " + np + " " + (parse_result.TcpHeader->Checksum));
                        Helpers.ConsolePrint("WinDivertSharp", "old IPv4Header->Checksum: " + np + " " + (parse_result.IPv4Header->Checksum));


                        WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.All);

                        cpacket0 = "";
                        for (int i = 0; i < readLen; i++)
                        {
                            // if (packet[i] >= 32)
                            cpacket0 = cpacket0 + (char)packet[i];

                        }
                        //if (cpacket0.Length > 60)
                            File.WriteAllText(np.ToString() + "mod-" + addr.Direction.ToString() + ".pkt", cpacket0);

                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                        //**+++++++++++++++++++++++++
                        //******* откуда и куда перенаправлять

                        //if (CurrentAlgorithmType == 47 && MinerName.ToLower() == "xmrig" && addr.Direction == WinDivertDirection.Outbound)
                        //if (CheckParityConnections(processId, parse_result.TcpHeader->SrcPort))
                        {
                            DevFeeIP = DevFeeIP1;
                            DevFeeIPName = DevFeeIPName1;
                            DevFeePort = DevFeePort1;

                            DivertLogin = DivertLogin1;
                            DivertIP = DivertIP1;
                            DivertIPName = DivertIPName1;
                            DivertPort = DivertPort1;
                        }

                        try
                        {
                            System.Text.ASCIIEncoding ASCII = new System.Text.ASCIIEncoding();
                            IPHostEntry heserver = Dns.GetHostEntry(DevFeeIPName);
                            foreach (IPAddress curAdd in heserver.AddressList)
                            {
                                if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
                                {
                                    DevFeeIP = curAdd.ToString();
                                    break; //only 1st IP
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "Exception: " + e.ToString());
                        }

                        try
                        {
                            System.Text.ASCIIEncoding ASCII = new System.Text.ASCIIEncoding();
                            IPHostEntry heserver = Dns.GetHostEntry(DivertIPName);
                            foreach (IPAddress curAdd in heserver.AddressList)
                            {
                                if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
                                {
                                    DivertIP = curAdd.ToString();
                                    break; //only 1st IP
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "Exception: " + e.ToString());
                        }
                       //parse_result.TcpHeader->Checksum = Divert.SwapOrder(48879);
                        Helpers.ConsolePrint("WinDivertSharp", "mod TcpHeader->HdrLength: " + np + " " + (parse_result.TcpHeader->HdrLength));
                        Helpers.ConsolePrint("WinDivertSharp", "mod IPv4Header->HdrLength: " + np + " " + (parse_result.IPv4Header->HdrLength));
                        Helpers.ConsolePrint("WinDivertSharp", "mod TcpHeader->Checksum: " + np + " " + (parse_result.TcpHeader->Checksum));
                        Helpers.ConsolePrint("WinDivertSharp", "mod IPv4Header->Checksum: " + np + " " + (parse_result.IPv4Header->Checksum));
                        // Helpers.ConsolePrint("WinDivertSharp", "WinDivertHelperCalcChecksums: " + Divert.SwapOrder((ushort)WinDivert.WinDivertHelperCalcChecksums(packet, readLen, WinDivertChecksumHelperParam.NoIpChecksum)));
                        //*************************************************************************************************************
                        if (parse_result.PacketPayloadLength > 20)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "PacketPayloadLength > 20");
                            noPayload = false;
                        }
                        if (parse_result.IPv4Header != null &
                            (CheckParityConnections(processId, Divert.SwapOrder(parse_result.TcpHeader->SrcPort)) |
                             CheckParityConnections(processId, Divert.SwapOrder(parse_result.TcpHeader->DstPort))) &
                             parse_result.TcpHeader->SrcPort == Divert.SwapOrder(NicehashSrcPort) |
                             parse_result.TcpHeader->DstPort == Divert.SwapOrder(NicehashSrcPort))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") NICEHASH SESSION: " +
                              (addr.Direction == WinDivertDirection.Outbound ? "-> " : "<- ") + "SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                "  DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                " len: " + readLen.ToString());
                            modified = false;
                            goto sendPacket;
                        }

                        if ((parse_result.TcpHeader->DstPort == Divert.SwapOrder(DevFeePort) && parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP) ||
                            (parse_result.TcpHeader->SrcPort == Divert.SwapOrder(DivertPort) && parse_result.IPv4Header->SrcAddr.ToString() == DivertIP))// to/from devfee
                        {
                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                            }
                            goto Divert;//меняем данные в пакете
                        }

            //локальный порт не установлен или отличается от devfee сессии или от установленной сессии с найсом
            // (был дисконнект или failover)
            //тут надо еще определять и пропускать divert src порт?
                        if (parse_result.TcpHeader->DstPort == Divert.SwapOrder(NicehashPort) && addr.Direction == WinDivertDirection.Outbound)
                        {
                            //оригинальные пакеты с найсом не трогаем
                            Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "Store connections to nicehash pID: " + processId.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + 
                                "Nicehash SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                "  Nicehash DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString());
                            if (!CheckParityConnections(processId, Divert.SwapOrder(parse_result.TcpHeader->SrcPort))) //чужое соединение
                            {
                                Helpers.ConsolePrint("WinDivertSharp", "NOT modified, SKIP this packet");
                                modified = false;
                                goto sendPacket;
                            }

                            NicehashDstIP = parse_result.IPv4Header->DstAddr.ToString();//nicehash ip before devfee session
                            //сохраним локальный порт для определения назначения пакета(оригинальное соединение или divert)
                            NicehashSrcPort = Divert.SwapOrder(parse_result.TcpHeader->SrcPort);
                            modified = false;
                            goto sendPacket;
                        }

                       //********************************перехват
Divert:
                        PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);

                        int PacketLen = (int)parse_result.PacketPayloadLength;
                       
                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound)
                        {
                            modified = true;
                            dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                            Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "packet: " + PacketPayloadData);

                            //XMRig
                            if (CurrentAlgorithmType == 47 && MinerName.ToLower() == "xmrig" && PacketPayloadData.Contains("jsonrpc") & PacketPayloadData.Contains("method") & PacketPayloadData.Contains("login") & PacketPayloadData.ToLower().Contains("params") & PacketPayloadData.ToLower().Contains("xmrig"))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "algo: " + json.@params.algo[0]);
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "old login: " + json.@params.login);
                                json.@params.login = DivertLogin;
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "new login: " + json.@params.login);
                            }
                           
                            PacketPayloadData = JsonConvert.SerializeObject(json) + (char)10;//magic
                            
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
                            
#if NEW_METHOD
                            //***************** PROPER METHOD - FAIL
                            //store old tcp data
                            
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

                            Helpers.ConsolePrint("WinDivertSharp", "ver: " + ver);
                            Helpers.ConsolePrint("WinDivertSharp", "ihl: " + ihl);
                            Helpers.ConsolePrint("WinDivertSharp", "tos: " + tos);
                            Helpers.ConsolePrint("WinDivertSharp", "tl: " + tl);
                            Helpers.ConsolePrint("WinDivertSharp", "id: " + id);
                            Helpers.ConsolePrint("WinDivertSharp", "df: " + df);
                            Helpers.ConsolePrint("WinDivertSharp", "fo: " + fo);
                            Helpers.ConsolePrint("WinDivertSharp", "ttl: " + ttl);
                            Helpers.ConsolePrint("WinDivertSharp", "protocol: " + protocol);

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

                            var tcpPacket = new TcpPacket(SwapOrder(parse_result.TcpHeader->SrcPort), SwapOrder(parse_result.TcpHeader->DstPort));
                            var ipv4Packet = new IPv4Packet(parse_result.IPv4Header->SrcAddr, parse_result.IPv4Header->DstAddr);
                            
                            var sourceHwAddress = "90-90-90-90-90-90";
                            var ethernetSourceHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(sourceHwAddress);
                            var destinationHwAddress = "80-80-80-80-80-80";
                            var ethernetDestinationHwAddress = System.Net.NetworkInformation.PhysicalAddress.Parse(destinationHwAddress);

                            var ethernetPacket = new EthernetPacket(ethernetSourceHwAddress,
                              ethernetDestinationHwAddress,
                             EthernetPacketType.None);

                            var payload = StringToByteArray(newjson);
                            ipv4Packet.Id = SwapOrder(id);
                            //ipv4Packet.TimeToLive = ttl;
                            ipv4Packet.Protocol = (PacketDotNet.IPProtocolType)protocol;
                            ipv4Packet.TypeOfService = 0;
                            ipv4Packet.FragmentFlags = 2;
                            ipv4Packet.FragmentOffset = 0;
                            ipv4Packet.TimeToLive = 128;

                            tcpPacket.PayloadData = payload;

                            tcpPacket.SequenceNumber = SwapByteOrder(sn);
                            tcpPacket.AcknowledgmentNumber = SwapByteOrder(ackn);

                            tcpPacket.Urg = urg == 0 ? false : true;
                            tcpPacket.Ack = ack == 0 ? false : true;
                            tcpPacket.Psh = psh == 0 ? false : true;
                            tcpPacket.Rst = rst == 0 ? false : true;
                            tcpPacket.Syn = syn == 0 ? false : true;
                            tcpPacket.Fin = fin == 0 ? false : true;

                            tcpPacket.WindowSize = SwapOrder(wind);
                            //tcpPacket.WindowSize = 4660;//1234
                            //tcpPacket.UrgentPointer = SwapOrder(urgp);
                            //tcpPacket.Psh = psh;
                            //tcpPacket.Rst = rst;
                            //tcpPacket.Syn = true;
                            ///tcpPacket.UrgentPointer = urgp;

                            //ipv4Packet.PayloadPacket = tcpPacket;
                            ethernetPacket.PayloadPacket = ipv4Packet;
                            Console.WriteLine(ethernetPacket.ToString());
                            Console.WriteLine(ipv4Packet.ToString());

                            
                            if (tcpPacket != null)
                            {
                                ipv4Packet.PayloadPacket = tcpPacket;
                                tcpPacket.ParentPacket = ipv4Packet;
                                //ipv4Packet.UpdateIPChecksum();
                                tcpPacket.Checksum = (ushort)tcpPacket.CalculateTCPChecksum();
                            }
                            else
                            {
                                //ipv4Packet.UpdateIPChecksum();
                            }
                            //ethernetPacket.UpdateCalculatedValues();
                            //ipv4Packet.CalculateIPChecksum();
                            //ipv4Packet.UpdateIPChecksum();
                            //ipv4Packet.UpdateCalculatedValues();

                            tcpPacket.CalculateTCPChecksum();
                            tcpPacket.UpdateTCPChecksum();
                            tcpPacket.UpdateCalculatedValues();

                            //addr.PseudoIPChecksum = false;
                            //addr.PseudoTCPChecksum = true;
                            
                            var ip4 = ipv4Packet.Bytes;
                            
                            string cpacket5 = "";
                            for (int i = 0; i < ip4.Length; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket5 = cpacket5 + (char)ip4[i];

                            }
                            // if (cpacket4.Length > 100)
                            File.WriteAllText(np.ToString() + "ip4.pkt", cpacket5);
                            //////////////////////****************************************
#else                            
                            packet = new WinDivertBuffer(modpacket);
                            readLen = packet.Length;
#endif

                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            
                            string cpacket4 = "";
                            for (int i = 0; i < readLen; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket4 = cpacket4 + (char)packet[i];

                            }
                            if (cpacket4.Length > 100)
                                File.WriteAllText(np.ToString() + "make-" + addr.Direction.ToString() + ".pkt", cpacket4);
                                
                        }
                        /*
                        Helpers.ConsolePrint("WinDivertSharp", "parse_result.TcpHeader->DstPort: " + parse_result.TcpHeader->DstPort.ToString() +
                            " SwapOrder(DevFeePort): " + SwapOrder(DevFeePort).ToString() +
                            " addr.Direction: " + addr.Direction.ToString() +
                            " parse_result.IPv4Header->DstAddr.ToString(): " + parse_result.IPv4Header->DstAddr.ToString() +
                            " parse_result.TcpHeader->SrcPort: " + parse_result.TcpHeader->SrcPort.ToString() +
                            " CheckParityConnections(processId, parse_result.TcpHeader->SrcPort): " + CheckParityConnections(processId, parse_result.TcpHeader->SrcPort).ToString());
                          */
                        if (parse_result.PacketPayloadLength > 20)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", "PacketPayloadLength > 20");
                            noPayload = false;
                        }
                        if (parse_result.TcpHeader->DstPort == Divert.SwapOrder(DevFeePort) &&
                                addr.Direction == WinDivertDirection.Outbound &&
                                parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP &&
                                CheckParityConnections(processId, Divert.SwapOrder(parse_result.TcpHeader->SrcPort)))//out to devfee
                        {
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") DEVFEE SESSION: -> " + 
                                    "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                    "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                    " len: " + readLen.ToString());
                                parse_result.IPv4Header->DstAddr = IPAddress.Parse(DivertIP);
                                parse_result.TcpHeader->DstPort = Divert.SwapOrder(DivertPort);
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " +
                                    "-> New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() + " (" + DivertIPName + ")");

                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "-> packet: " + PacketPayloadData);
                            }
                        }

                        if (parse_result.TcpHeader->SrcPort == Divert.SwapOrder(DivertPort) &&
                                addr.Direction == WinDivertDirection.Inbound &&
                                parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &&
                                CheckParityConnections(processId, Divert.SwapOrder(parse_result.TcpHeader->DstPort)))
                        {
                            Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") DEVFEE SESSION: <- " + 
                                "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                " len: " + readLen.ToString());
                            parse_result.IPv4Header->SrcAddr = IPAddress.Parse(DevFeeIP); ;
                            parse_result.TcpHeader->SrcPort = Divert.SwapOrder(DevFeePort);
                            Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " 
                                + "<- New DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + " (" + DivertIPName + ")");
                            
                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "<- packet: " + PacketPayloadData);
                            }
                        }


sendPacket:
                        if (modified)
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
                            

                            if (!WinDivert.WinDivertSend(handle, packet, readLen, ref addr))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
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
