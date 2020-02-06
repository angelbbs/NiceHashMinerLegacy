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


namespace NiceHashMinerLegacy.Divert
{
    public class DClaymoreDual
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

        private static string filter = "";

        private static string DivertLogin = "";
        private static string DivertLogin1 = "";
        private static string DivertLogin2 = "";

        private static string PacketPayloadData;
        private static string packetdata;
        
        private static string processName = "";

        internal static bool CheckParityConnections(List<uint> processIdList, ushort Port, string def)
        {
           // return true;
            Port = Divert.SwapOrder(Port);
            Helpers.ConsolePrint("WinDivertSharp", def + " processIdList: " + " (" + String.Join(", ", processIdList) + " Port: " + Port.ToString());
            //Helpers.ConsolePrint("WinDivertSharp", "processId: " + processId.ToString() + " Port: " + Port.ToString());
            List<Connection> _allConnections = new List<Connection>();

            _allConnections.Clear();
            _allConnections.AddRange(NetworkInformation.GetTcpV4Connections());

           // if (Divert.processIdList.Count > 1)
            {
                for (int i = 1; i < _allConnections.Count; i++)
                {
                    if (Divert.processIdList.Contains(_allConnections[i].OwningPid) &&
                        _allConnections[i].LocalEndPoint.Port == Port)
                    {
                        
                        Helpers.ConsolePrint("WinDivertSharp", "CheckParityConnections: " + 
                            _allConnections[i].OwningPid.ToString() + " " + _allConnections[i].LocalEndPoint.ToString() +
                             " processIdList: " + String.Join(", ", processIdList) + " Port: " + Port.ToString());
                             
                        _allConnections = null;
                        return true;
                    }
                }
            }
            /*
            else
            {
                for (int i = 1; i < _allConnections.Count; i++)
                {
                    if (_allConnections[i].OwningPid == (uint)processId &
                        _allConnections[i].LocalEndPoint.Port == Port &
                        processName.ToLower() == "ethdcrminer64.exe")
                    {
                       // Helpers.ConsolePrint("WinDivertSharp", "CheckParityConnections: " + _allConnections[i].OwningPid.ToString() + " " + _allConnections[i].LocalEndPoint.ToString());
                        _allConnections = null;
                        return true;
                    }
                }
            }
            */
            _allConnections = null;
            return false;
        }

        public static IntPtr ClaymoreDualDivertStart(List<uint> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
            {
            divert_running = true;
            Helpers.ConsolePrint("WinDivertSharp", "Divert START for process ID: " + String.Join(", ", processIdList) + " Miner: " + MinerName + " CurrentAlgorithmType: " + CurrentAlgorithmType + " Platform: " + strPlatform);

                DevFeeIP1 = "";
                DevFeeIPName1 = "us1.ethpool.org";
                DevFeePort1 = 3333;

                DevFeeIP2 = "";
                DevFeeIPName2 = "eth-eu.dwarfpool.com";
                DevFeePort2 = 8008;

                DivertLogin1 = "0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1";
                DivertIP1 = "";
                DivertIPName1 = "eu1.ethermine.org";
                DivertPort1 = 4444;

                DivertLogin2 = "0x9290e50e7ccf1bdc90da8248a2bbacc5063aeee1";
                DivertIP2 = "";
                DivertIPName2 = "eth-eu.dwarfpool.com";
                DivertPort2 = 8008;

                //filter = "ip && tcp && (inbound ? (tcp.SrcPort == 3333 || tcp.SrcPort == 4444 || tcp.SrcPort == 8008) : (tcp.DstPort == 3333) || (tcp.DstPort == 8008) || (tcp.DstPort == 4444) || (tcp.DstPort == 5555))";//dagger
                //filter = "!loopback && (ip || tcp) && (inbound ? (tcp.SrcPort == 3333 || tcp.SrcPort == 4444 || tcp.SrcPort == 8008) : (tcp.DstPort == 3333) || (tcp.DstPort == 8008) || (tcp.DstPort == 4444) || (tcp.DstPort == 5555))";//dagger
                filter = "(ip || tcp) && (inbound ? (tcp.SrcPort == 3333 || tcp.SrcPort == 4444 || tcp.SrcPort == 8008) : (tcp.DstPort == 3333) || (tcp.DstPort == 8008) || (tcp.DstPort == 4444) || (tcp.DstPort == 5555))";//dagger

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

            Helpers.ConsolePrint("WinDivertSharp", "Divert handle: " + DivertHandle.ToString());
            return DivertHandle;
        }

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
            processName = "EthDcrMiner64.exe";
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
                        //**+++++++++++++++++++++++++
                        //******* откуда и куда перенаправлять
                        if (CurrentAlgorithmType == 20 & MinerName.ToLower() == "claymoredual" &
                            addr.Direction == WinDivertDirection.Outbound &
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, "1"))
                        {
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333) //us1.ethpool.org
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() +") -> Devfee connection to us1.ethpool.org (" + DevFeeIP + ")");
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + String.Join(", ", processIdList) +") -> Devfee connection to us1.ethpool.org (" + DevFeeIP + ")");
                                DevFeeIPName = DevFeeIPName1;
                                DevFeePort = DevFeePort1;

                                DivertLogin = DivertLogin1;
                                DivertIP = DivertIP1;
                                DivertIPName = DivertIPName1;
                                DivertPort = DivertPort1;
                            }
                            if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008) //eth-eu.dwarfpool.com
                            {
                                DevFeeIP = parse_result.IPv4Header->DstAddr.ToString();
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") -> Devfee connection to eth-eu.dwarfpool.com (" + DevFeeIP + ")");
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + String.Join(", ", processIdList) + ") -> Devfee connection to eth-eu.dwarfpool.com (" + DevFeeIP + ")");
                                DevFeeIPName = DevFeeIPName2;
                                DevFeePort = DevFeePort2;

                                DivertLogin = DivertLogin2;
                                DivertIP = DivertIP2;
                                DivertIPName = DivertIPName2;
                                DivertPort = DivertPort2;
                            }
                        }
                        /*
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
                        */
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
                        
//*************************************************************************************************************
                        if (CurrentAlgorithmType == 20 & MinerName.ToLower() == "claymoredual" &&
                            addr.Direction == WinDivertDirection.Outbound &&
                            (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 5555) )
                        {
                            //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " + "SSL connection. No divert");
                            //continue;
                            modified = false;
                            goto sendPacket;
                        }
                        
                        if (addr.Direction == WinDivertDirection.Outbound &
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == DevFeePort & parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP &
                            CheckParityConnections(processIdList, DevFeePort, "2"))
                        //if ((parse_result.TcpHeader->DstPort == Divert.SwapOrder(DevFeePort) && parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP) ||
                          //  (parse_result.TcpHeader->SrcPort == Divert.SwapOrder(DivertPort) && parse_result.IPv4Header->SrcAddr.ToString() == DivertIP) &&
                            //(CheckParityConnections(processId, parse_result.TcpHeader->DstPort) || CheckParityConnections(processId, parse_result.TcpHeader->SrcPort)))// to/from devfee
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

                        if (addr.Direction == WinDivertDirection.Inbound &
                            Divert.SwapOrder(parse_result.TcpHeader->SrcPort) == DivertPort & parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &
                            CheckParityConnections(processIdList, DevFeePort, "3"))
                        //if ((parse_result.TcpHeader->DstPort == Divert.SwapOrder(DevFeePort) && parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP) ||
                        //  (parse_result.TcpHeader->SrcPort == Divert.SwapOrder(DivertPort) && parse_result.IPv4Header->SrcAddr.ToString() == DivertIP) &&
                        //(CheckParityConnections(processId, parse_result.TcpHeader->DstPort) || CheckParityConnections(processId, parse_result.TcpHeader->SrcPort)))// to/from devfee
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
                        //********************************перехват
Divert:
                        PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);

                        int PacketLen = (int)parse_result.PacketPayloadLength;
                        
                        //dwarfpool fix
                        if (parse_result.PacketPayloadLength > 10 & addr.Direction == WinDivertDirection.Outbound &&
                            Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008 && !PacketPayloadData.Contains("eth_submitLogin") &&
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, "4"))
                        {
                            //modified = false;
                            //goto sendPacket;
                            goto changeSrcDst; //проверить
                        }
                        
                        if (parse_result.PacketPayloadLength > 1 & addr.Direction == WinDivertDirection.Outbound &
                            CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, "5"))
                        {
                            modified = true;
                            dynamic json = JsonConvert.DeserializeObject(PacketPayloadData);
                            //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") packet: " + PacketPayloadData);
       
                            //etpool
                            if (CurrentAlgorithmType == 20 && MinerName.ToLower() == "claymoredual" &&
                                PacketPayloadData.Contains("eth_login") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 3333)
                            {
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") old etpool login: " + json.@params[0]);
                                json.@params[0] = DivertLogin;
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") new etpool login: " + json.@params[0]);
                            }

                            PacketPayloadData = JsonConvert.SerializeObject(json) + (char)10;//magic
                            
                            //dwarfpool
                            if (CurrentAlgorithmType == 20 && MinerName.ToLower() == "claymoredual" &&
                                PacketPayloadData.Contains("eth_submitLogin") &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008)
                            {
                                PacketPayloadData = "{\"worker\": \"eth1.0\", \"jsonrpc\": \"2.0\", \"params\": [\"" + DivertLogin +"\", \"x\"], \"id\": 2, \"method\": \"eth_submitLogin\"}" + (char)10;
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") new dwarfpool login: " + DivertLogin);
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
                            

                            packet = new WinDivertBuffer(modpacket);
                            readLen = packet.Length;

                            if (parse_result.PacketPayloadLength > 10 && addr.Direction == WinDivertDirection.Outbound &&
                                Divert.SwapOrder(parse_result.TcpHeader->DstPort) == 8008)
                            //PacketPayloadData.Contains("eth_getWork"))//dwarfpool fix
                            {
                               // modified = false;
                               // goto sendPacket;
                            }

                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            /*
                            string cpacket4 = "";
                            for (int i = 0; i < readLen; i++)
                            {
                                // if (packet[i] >= 32)
                                cpacket4 = cpacket4 + (char)packet[i];

                            }
                            if (cpacket4.Length > 100)
                                File.WriteAllText(np.ToString() + "make-" + addr.Direction.ToString() + ".pkt", cpacket4);
                              */  
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
                        if (Divert.SwapOrder(parse_result.TcpHeader->DstPort) == DevFeePort &&
                                addr.Direction == WinDivertDirection.Outbound &&
                                parse_result.IPv4Header->DstAddr.ToString() == DevFeeIP &&
                                CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, "6"))//out to devfee
                        {
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") DEVFEE SESSION: -> " + 
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + String.Join(", ", processIdList) + ") DEVFEE SESSION: -> " + 
                                    "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                    "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                    " len: " + readLen.ToString());
                                parse_result.IPv4Header->DstAddr = IPAddress.Parse(DivertIP);
                                parse_result.TcpHeader->DstPort = Divert.SwapOrder(DivertPort);
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") " +
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + String.Join(", ", processIdList) + ") " +
                                    "-> New DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() + " (" + DivertIPName + ")");

                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") -> packet: " + PacketPayloadData);
                            }
                            modified = true;
                        }

                        if (Divert.SwapOrder(parse_result.TcpHeader->SrcPort) == DivertPort &&
                                addr.Direction == WinDivertDirection.Inbound &&
                                parse_result.IPv4Header->SrcAddr.ToString() == DivertIP &&
                                CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, "7"))
                        {
                            //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") DEVFEE SESSION: <- " + 
                            Helpers.ConsolePrint("WinDivertSharp", processName + " (" + String.Join(", ", processIdList) + ") DEVFEE SESSION: <- " + 
                                "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + 
                                "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                " len: " + readLen.ToString());
                            parse_result.IPv4Header->SrcAddr = IPAddress.Parse(DevFeeIP); ;
                            parse_result.TcpHeader->SrcPort = Divert.SwapOrder(DevFeePort);
                            //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") "
                            Helpers.ConsolePrint("WinDivertSharp", processName + " (" + String.Join(", ", processIdList) + ") "
                                + "<- New DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() + " (" + DivertIPName + ")");
                            
                            if (parse_result.PacketPayloadLength > 0)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                //Helpers.ConsolePrint("WinDivertSharp", processName + " (" + processId.ToString() + ") <- packet: " + PacketPayloadData);
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + String.Join(", ", processIdList) + ") <- packet: " + PacketPayloadData);
                            }
                            modified = true;
                        }


sendPacket:
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

                            if (!WinDivert.WinDivertSend(handle, packet, readLen, ref addr))
                            {
                                Helpers.ConsolePrint("WinDivertSharp", processName + " (" + String.Join(", ", processIdList) + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
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
