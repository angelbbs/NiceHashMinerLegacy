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
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace NiceHashMinerLegacy.Divert
{
    public class Divert
    {
       // private static Timer _divertTimer;
        
        public static bool logging;
        public static bool _certInstalled = false;
        public static bool _SaveDivertPackets;
        public static bool gminer_runningEthash = false;
        public static bool gminer_runningDagger3GB = false;
        public static bool gminer_runningZhash = false;
        public static bool gminer_runningCuckarood29 = false;
        public static bool gminer_runningNeoscrypt = false;
        public static bool gminer_runningHandshake = false;
        public static bool gminer_runningCuckooCycle = false;
        public static bool gminer_runningGrin = false;
        public static bool gminer_runningBeam = false;
        public static volatile bool Ethashdivert_running = true;
        public static volatile bool Dagger3GBdivert_running = true;
        public static volatile bool Zhashdivert_running = true;
        public static volatile bool Cuckarood29divert_running = true;
        public static volatile bool Neoscryptdivert_running = true;
        public static volatile bool Handshakedivert_running = true;
        public static volatile bool CuckooCycledivert_running = true;
        public static volatile bool X16rV2divert_running = true;
        public static volatile bool Grindivert_running = true;
        public static volatile bool RandomXdivert_running = true;
        public static volatile bool Beamdivert_running = true;
        public static volatile bool Testdivert_running = true;

        public static bool BlockGMinerApacheTomcat;
        public static int Dagger3GBEpochCount = 0;
        public static string Dagger3GBJob = "";
        public static bool Dagger3GBCheckConnection = false;
        public static bool DaggerHashimoto3GBProfit = false;
        public static bool DaggerHashimoto3GBForce = false;

        public static UInt32 SwapByteOrder(UInt32 value)
        {
            return
              ((value & 0xff000000) >> 24) |
              ((value & 0x00ff0000) >> 8) |
              ((value & 0x0000ff00) << 8) |
              ((value & 0x000000ff) << 24);
        }

        public static UInt64 SwapByteOrder(UInt64 value)
        {
            return
              ((value & 0xff00000000000000L) >> 56) |
              ((value & 0x00ff000000000000L) >> 40) |
              ((value & 0x0000ff0000000000L) >> 24) |
              ((value & 0x000000ff00000000L) >> 8) |
              ((value & 0x00000000ff000000L) << 8) |
              ((value & 0x0000000000ff0000L) << 24) |
              ((value & 0x000000000000ff00L) << 40) |
              ((value & 0x00000000000000ffL) << 56);
        }

        public static string GetProcessName(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (var process in processList)
            {
                var processName = process["Name"];
                var processPath = process["ExecutablePath"];
                return processName.ToString();
            }
                return "";
            
        }

        public static ushort SwapOrder(ushort val)
        {
            return (ushort)(((val & 0xFF00) >> 8) | ((val & 0x00FF) << 8));
        }

        public static uint SwapOrder(uint val)
        {
            val = (val >> 16) | (val << 16);
            return ((val & 0xFF00) >> 8) | ((val & 0x00FF) << 8);
        }

        public static ulong SwapOrder(ulong val)
        {
            val = (val >> 32) | (val << 32);
            val = ((val & 0xFFFF0000FFFF0000) >> 16) | ((val & 0x0000FFFF0000FFFF) << 16);
            return ((val & 0xFF00FF00FF00FF00) >> 8) | ((val & 0x00FF00FF00FF00FF) << 8);
        }

        public static string ToString(byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
                response += (Char)b;

            return response;
        }

        public static unsafe string PacketPayloadToString(byte* bytes, uint length)
        {
            string data = "";
            for (int i = 0; i < length; i++)
            {
                if (bytes[i] >= 32)
                    data = data + (char)bytes[i];
            }
            return data;
        }

        public static unsafe byte* StringToPacketPayload(string data, int length)
        {
            byte* bytes = null;
            for (int i = 0; i < length; i++)
            {
                if (data[i] >= 32)
                {
                    bytes = bytes + (char)data[i];
                }
            }
            return bytes;
        }

        public static byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }

        public static ushort CalcTCPChecksum(WinDivertBuffer buffer, uint readLen)
        {
            uint sum = 0;
            uint sumTCP = 0;
            uint sumPseudoIP = 0;
            int x;
            int odd = 0;
            uint length = readLen;

            byte[] packet = new byte[readLen];
            for (int i = 0; i < readLen; i++)
            {
                packet[i] = buffer[i];
            }
            // TCP Header 
            for (x = 20; x < packet.Length - 1; x += 2)
            {
                sumTCP += (ushort)(((packet[x + 1] << 8) & 0xFF00) + (packet[x] & 0xFF));
            }

            if (packet.Length % 2 != 0)
            {
                sumTCP += (ushort)((packet[packet.Length - 1] & 0xFF));
                odd = 1;
            }

            // Pseudo header - Source Address
            sumPseudoIP += (ushort)(((packet[13] << 8) & 0xFF00) + (packet[12] & 0xFF));
            sumPseudoIP += (ushort)(((packet[15] << 8) & 0xFF00) + (packet[14] & 0xFF));

            // Pseudo header - Dest Address
            sumPseudoIP += (ushort)(((packet[17] << 8) & 0xFF00) + (packet[16] & 0xFF));
            sumPseudoIP += (ushort)(((packet[19] << 8) & 0xFF00) + (packet[18] & 0xFF));
            // Pseudo header - Protocol
            sumPseudoIP += (ushort)((packet[9] << 8) & 0xFF00);
            // Pseudo header - TCP Header length
            sumPseudoIP += (ushort)(((packet.Length - 20) << 8) & 0xFF00);

            // 16 bit 1's compliment
            sum = sumPseudoIP + sumTCP + (ushort)odd;

            while ((sum >> 16) != 0)
            {
                sum = (sum & 0xFFFF) + (sum >> 16);
            }
            return (ushort)~sum;
        }
        public static ushort CalcIpChecksum(byte[] header, int length)
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
            return Divert.SwapOrder((ushort)~sum);
        }
        public static byte[] getBytes(object str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }


        public static List<string> processIdListEthash = new List<string>();
        public static List<string> processIdListDagger3GB = new List<string>();
        public static List<string> processIdListZhash = new List<string>();
        public static List<string> processIdListCuckarood29 = new List<string>();
        public static List<string> processIdListNeoscrypt = new List<string>();
        public static List<string> processIdListHandshake = new List<string>();
        public static List<string> processIdListCuckooCycle = new List<string>();
        public static List<string> processIdListX16rV2 = new List<string>();
        public static List<string> processIdListGrin = new List<string>();
        public static List<string> processIdListBeam = new List<string>();
        public static List<string> processIdListTest = new List<string>();
        public static List<string> processIdListRandomX = new List<string>();

        private static IntPtr DEthashHandle = (IntPtr)0;
        private static IntPtr DDagger3GBHandle = (IntPtr)0;
        private static IntPtr DZhashHandle = (IntPtr)0;
        private static IntPtr DCuckarood29Handle = (IntPtr)0;
        private static IntPtr DNeoscryptHandle = (IntPtr)0;
        private static IntPtr DHandshakeHandle = (IntPtr)0;
        private static IntPtr DCuckooCycleHandle = (IntPtr)0;
        private static IntPtr DX16rV2Handle = (IntPtr)0;
        private static IntPtr DGrinHandle = (IntPtr)0;
        private static IntPtr DBeamHandle = (IntPtr)0;
        private static IntPtr DTestHandle = (IntPtr)0;
        private static IntPtr DRandomXHandle = (IntPtr)0;

        public static string worker = "";

        public static string LineNumber([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)

        {
            return caller + ": " + lineNumber;
        }

        private static System.Text.ASCIIEncoding ASCII;
        private static IPHostEntry heserver;
        public static string DNStoIP(string DivertIPName)
        {
            try
            {
                ASCII = new System.Text.ASCIIEncoding();
                heserver = Dns.GetHostEntry(DivertIPName);
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
                    {
                        return curAdd.ToString();
                       // break; //only 1st IP
                    }
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("WinDivertSharp", "Exception: " + e.ToString());
            }
            return "";
        }

        public static string GetRemoteIP(List<string> PortList, string port)
        {
            for (var i = 0; i < PortList.Count; i++)
            {
                if (PortList[i].Contains(port))
                {
                    return PortList[i].Split(':')[1];
                }
            }
            return "0.0.0.0";
        }
        public static bool CheckSrcPort(List<string> PortList, string sample)
        {
            bool found = false;
            for (var i = 0; i < PortList.Count; i++)
            {
                if (PortList[i].Contains(sample))
                {
                    found = true;
                }
            }
            if (found == false)
            {
                return false;
            }
            return true;
        }

        public unsafe static WinDivertBuffer MakeNewPacket(WinDivertBuffer packet, uint readLen, string PacketPayloadData)
        {
            WinDivertParseResult parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
            //store old tcp/ip data
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

            var tcpPacket = new TcpPacket(Divert.SwapOrder(parse_result.TcpHeader->SrcPort), Divert.SwapOrder(parse_result.TcpHeader->DstPort));
            var ipv4Packet = new IPv4Packet(parse_result.IPv4Header->SrcAddr, parse_result.IPv4Header->DstAddr);

            var payload = Divert.StringToByteArray(PacketPayloadData);
            ipv4Packet.Id = Divert.SwapOrder(id);
            ipv4Packet.TimeToLive = ttl;
            ipv4Packet.Protocol = (PacketDotNet.IPProtocolType.TCP);
            ipv4Packet.TypeOfService = tos;
            ipv4Packet.FragmentFlags = 2;
            ipv4Packet.FragmentOffset = 0;

            tcpPacket.PayloadData = payload;

            tcpPacket.SequenceNumber = Divert.SwapByteOrder(sn);
            tcpPacket.AcknowledgmentNumber = Divert.SwapByteOrder(ackn);

            tcpPacket.Urg = urg == 0 ? false : true;
            tcpPacket.Ack = ack == 0 ? false : true;
            tcpPacket.Psh = psh == 0 ? false : true;
            tcpPacket.Rst = rst == 0 ? false : true;
            tcpPacket.Syn = syn == 0 ? false : true;
            tcpPacket.Fin = fin == 0 ? false : true;

                ipv4Packet.PayloadPacket = tcpPacket;
                tcpPacket.ParentPacket = ipv4Packet;
                ushort pl = (ushort)(tcpPacket.PayloadData.Length + 10);
                //Helpers.ConsolePrint("WinDivertSharp", "tcpPacket.PayloadData.Length: " + pl.ToString());
                tcpPacket.WindowSize = Divert.SwapOrder(pl);
                tcpPacket.Checksum = (ushort)0;


            var ip4 = ipv4Packet.Bytes;
            packet.Dispose();
            packet = new WinDivertBuffer(ip4);
            readLen = packet.Length;
            //    WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.NoIpChecksum);
            return packet;
        }

        public static string CheckParityConnections(List<string> processIdList, ushort Port, WinDivertDirection dir, List<string> _oldPorts)
        {
            try
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
            catch (Exception e)
            {
                Helpers.ConsolePrint("WinDivertSharp error: ", e.ToString());
                Thread.Sleep(500);
            }
            finally
            {

            }
            return "unknown: ?";
        }

        public unsafe static WinDivertBuffer MakeNewPacket2(WinDivertBuffer packet, uint readLen, string PacketPayloadData = "")
        {
            try
            {
                WinDivertParseResult parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                //store old tcp/ip data
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
                Helpers.ConsolePrint("WinDivertSharp", "ackn: " + ackn);
                Helpers.ConsolePrint("WinDivertSharp", "fin: " + fin);
                Helpers.ConsolePrint("WinDivertSharp", "psh: " + psh);
                Helpers.ConsolePrint("WinDivertSharp", "rst: " + rst);
                Helpers.ConsolePrint("WinDivertSharp", "sn: " + sn);
                Helpers.ConsolePrint("WinDivertSharp", "syn: " + syn);
                Helpers.ConsolePrint("WinDivertSharp", "urg: " + urg);
                Helpers.ConsolePrint("WinDivertSharp", "urgp: " + urgp);
                Helpers.ConsolePrint("WinDivertSharp", "wind: " + wind);


                var tcpPacket = new TcpPacket(Divert.SwapOrder(parse_result.TcpHeader->SrcPort), Divert.SwapOrder(parse_result.TcpHeader->DstPort));

                var ipv4Packet = new IPv4Packet(parse_result.IPv4Header->SrcAddr, parse_result.IPv4Header->DstAddr);


                if (PacketPayloadData.Length > 0)
                {
                    var payload = Divert.StringToByteArray(PacketPayloadData);
                    tcpPacket.PayloadData = payload;
                }
                else
                {
                    var payload = new byte[0];
                    tcpPacket.PayloadData = payload;
                }
                ipv4Packet.Id = Divert.SwapOrder(id);
                ipv4Packet.TimeToLive = ttl;
                ipv4Packet.Protocol = (PacketDotNet.IPProtocolType.TCP);
                ipv4Packet.TypeOfService = tos;
                ipv4Packet.FragmentFlags = 2;
                ipv4Packet.FragmentOffset = 0;


                tcpPacket.SequenceNumber = Divert.SwapByteOrder(sn);
                tcpPacket.AcknowledgmentNumber = Divert.SwapByteOrder(ackn);

                tcpPacket.Urg = urg == 0 ? false : true;
                tcpPacket.Ack = ack == 0 ? false : true;
                tcpPacket.Psh = psh == 0 ? false : true;
                tcpPacket.Rst = rst == 0 ? false : true;
                tcpPacket.Syn = syn == 0 ? false : true;
                tcpPacket.Fin = fin == 0 ? false : true;

                //            Helpers.ConsolePrint("WinDivertSharp", "TcpHeader->Checksum: " + parse_result.TcpHeader->Checksum.ToString());
                //            Helpers.ConsolePrint("WinDivertSharp", "IPv4Header->Checksum: " + parse_result.IPv4Header->Checksum.ToString());

                ipv4Packet.PayloadPacket = tcpPacket;
                tcpPacket.ParentPacket = ipv4Packet;
                ushort pl = (ushort)(tcpPacket.PayloadData.Length + 10);
                //Helpers.ConsolePrint("WinDivertSharp", "tcpPacket.PayloadData.Length: " + pl.ToString());
                tcpPacket.WindowSize = Divert.SwapOrder(pl);
                tcpPacket.Checksum = (ushort)0;
                Helpers.ConsolePrint("WinDivertSharp", "calc crc: " + tcpPacket.CalculateTCPChecksum());
                tcpPacket.UpdateCalculatedValues();


                var ip4 = ipv4Packet.Bytes;
                packet.Dispose();
                packet = new WinDivertBuffer(ip4);
                readLen = packet.Length;
                //    WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.NoIpChecksum);
                return packet;
            } catch (Exception e)
                {
                    Helpers.ConsolePrint("WinDivertSharp error: ", e.ToString());
                }
            return packet;
        }

        internal static Task<bool> GetDagger3GB(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListDagger3GB.Add(MinerName.ToLower() + ": " + processId.ToString());
                DDagger3GBHandle = DDagger3GB.Dagger3GBDivertStart(processIdListDagger3GB, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DDagger3GBHandle.ToString() + ". Initiated by " + processId.ToString() + " (Dagger3GB) to divert process list: " + " " + String.Join(",", processIdListDagger3GB));

                do
                {
                    childPID = GetChildProcess(processId);
                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListDagger3GB).Contains(childPID.ToString()))
                        {
                            processIdListDagger3GB.Add(MinerName.ToLower() + ": " + processId.ToString() + " " + childPID.ToString() + " %" + DDagger3GBHandle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new Dagger3GB ChildPid: " + childPID.ToString());
                           // processIdListDagger3GB.RemoveAll(x => x.Contains("claymoredual: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListDagger3GB: " + String.Join(" ", processIdListDagger3GB));
                            //break;
                        }

                    }
                    Thread.Sleep(400);
                } while (Dagger3GBdivert_running);
                return t.Task;
            });
        }
        private static IntPtr DivertHandle;
        public static IntPtr OpenWinDivert(string filter)
        {
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
            WinDivert.WinDivertSetParam(DivertHandle, WinDivertParam.QueueSize, 2097152);
            return DivertHandle;
        }

        [HandleProcessCorruptedStateExceptions]
        public static IntPtr DivertStart(int processId, int CurrentAlgorithmType, int SecondaryAlgorithmType, string MinerName, string strPlatform,
            string w, bool log, bool SaveDiverPackets, bool BlockGMinerApacheTomcatConfig, bool CertInstalled)
        {
            _certInstalled = CertInstalled;
            logging = log;
            //logging = false;
            _SaveDivertPackets = SaveDiverPackets;
            //_SaveDivertPackets = false;
            worker = w;
            BlockGMinerApacheTomcat = BlockGMinerApacheTomcatConfig;
            Helpers.ConsolePrint("WinDivertSharp", "Miner: " + MinerName + " Algo: " + CurrentAlgorithmType + " AlgoDual: " + SecondaryAlgorithmType);
            
            if ( CurrentAlgorithmType == 47 && MinerName.ToLower() == "xmrig") //for testing. Disable in productuon
            {
                return IntPtr.Zero;
              //  return DXMrig.XMRigDivertStart(processId, CurrentAlgorithmType, MinerName);
            }
            
            if (CurrentAlgorithmType == 47 && MinerName.ToLower().Equals("xmrig")) 
            {
                return IntPtr.Zero;
                //ssl
                RandomXdivert_running = true;
                if (MinerName.ToLower() == "xmrig")
                {
                    processIdListRandomX.Add("xmrig: " + processId.ToString());
                    if (processIdListRandomX.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DRandomXHandle.ToString() + ". Added " + processId.ToString() + " (RandomX) to divert process list: " + " " + String.Join(",", processIdListRandomX));
                        return DRandomXHandle;
                    }
                    DRandomXHandle = DRandomX.RandomXDivertStart(processIdListRandomX, CurrentAlgorithmType, MinerName);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DRandomXHandle.ToString() + ". Initiated by " + processId.ToString() + " (RandomX) to divert process list: " + " " + String.Join(",", processIdListRandomX));
                    return DRandomXHandle;
                }
            }

            //***********************************************************************************
            if (CurrentAlgorithmType == -9) //dagerhashimoto3gb
            {
                Dagger3GBdivert_running = true;
                //if (MinerName.ToLower() == "claymoredual")
                {
                    
                    processIdListDagger3GB.Add(MinerName.ToLower() + ": " + processId.ToString());
                    if (processIdListDagger3GB.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DDagger3GBHandle.ToString() + ". Added " + processId.ToString() + " (Dagger3GB) to divert process list: " + " " + String.Join(",", processIdListDagger3GB));
                        return DDagger3GBHandle;
                    }
                    DDagger3GBHandle = DDagger3GB.Dagger3GBDivertStart(processIdListDagger3GB, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DDagger3GBHandle.ToString() + ". Initiated by " + processId.ToString() + " (Dagger3GB) to divert process list: " + " " + String.Join(",", processIdListDagger3GB));
                    return DDagger3GBHandle;
                    
                    //processIdListEthash.Add("gminer: force");
                    //gminer_runningEthash = true;
                    //GetDagger3GB(processId, CurrentAlgorithmType, MinerName, strPlatform);
                }
                
            }

            //***********************************************************************************
            if (CurrentAlgorithmType == 20 && SecondaryAlgorithmType == -1) //dagerhashimoto
            {
                if (!Divert._certInstalled) return new IntPtr(-1);
                Ethashdivert_running = true;
                if (MinerName.ToLower() == "claymoredual")
                {
                    processIdListEthash.Add("claymoredual: " + processId.ToString());
                    if (processIdListEthash.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DEthashHandle.ToString() + ". Added " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));
                        return DEthashHandle;
                    }
                    DEthashHandle = DEthash.EthashDivertStart(processIdListEthash, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DEthashHandle.ToString() + ". Initiated by " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));
                    return DEthashHandle;
                }

                if (MinerName.ToLower() == "phoenix")
                {
                    processIdListEthash.Add("phoenix: " + processId.ToString());
                    if (processIdListEthash.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DEthashHandle.ToString() + ". Added " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));
                        return DEthashHandle;
                    }
                    DEthashHandle = DEthash.EthashDivertStart(processIdListEthash, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DEthashHandle.ToString() + ". Initiated by " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));
                    return DEthashHandle;
                }

                if (MinerName.ToLower() == "nbminer")
                {
                    processIdListEthash.Add("nbminer: " + processId.ToString());
                    if (processIdListEthash.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DEthashHandle.ToString() + ". Added " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));
                        return DEthashHandle;
                    }
                    DEthashHandle = DEthash.EthashDivertStart(processIdListEthash, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DEthashHandle.ToString() + ". Initiated by " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));
                    if (SecondaryAlgorithmType == 51)
                    {
                        processIdListHandshake.Add("nbiner: " + processId.ToString());
                        if (processIdListHandshake.Count > 1)
                        {
                            Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DHandshakeHandle.ToString() + ". Added " + processId.ToString() + " (Handshake) to divert process list: " + " " + String.Join(",", processIdListHandshake));
                            return DHandshakeHandle;
                        }
                        DHandshakeHandle = DHandshake.HandshakeDivertStart(processIdListHandshake, SecondaryAlgorithmType, MinerName, strPlatform);
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DHandshakeHandle.ToString() + ". Initiated by " + processId.ToString() + " (Handshake) to divert process list: " + " " + String.Join(",", processIdListHandshake));

                    }
                    return DEthashHandle;
                }

                if (MinerName.ToLower() == "nanominer")
                {
                    processIdListEthash.Add("nanominer: " + processId.ToString());
                    if (processIdListEthash.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DEthashHandle.ToString() + ". Added " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));
                        return DEthashHandle;
                    }
                    DEthashHandle = DEthash.EthashDivertStart(processIdListEthash, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DEthashHandle.ToString() + ". Initiated by " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));
                    return DEthashHandle;
                }

                if (MinerName.ToLower() == "gminer")
                {
                    processIdListEthash.Add("gminer: force");
                    gminer_runningEthash = true;
                    GetGMinerEthash(processId, CurrentAlgorithmType, MinerName, strPlatform);
                    if (SecondaryAlgorithmType == 51)
                    {
                        processIdListHandshake.Add("gminer: force");
                        gminer_runningHandshake = true;
                        GetGMinerHandshake(processId, CurrentAlgorithmType, MinerName, strPlatform);
                    }
                }
            }
            //******************************************************************************************
            if (CurrentAlgorithmType == 36) //zhash
            {
                Zhashdivert_running = true;
                if (MinerName.ToLower() == "gminer")
                {
                    processIdListZhash.Add("gminer: force");
                    gminer_runningZhash = true;
                    GetGMinerZhash(processId, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert handle: " + DZhashHandle.ToString());
                }

                if (MinerName.ToLower() == "miniz")
                {
                    processIdListZhash.Add("miniz: " + processId.ToString());
                    if (processIdListZhash.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DZhashHandle.ToString() + ". Added " + processId.ToString() + " (Zhash) to divert process list: " + " " + String.Join(",", processIdListZhash));
                        return DZhashHandle;
                    }
                    DZhashHandle = DZhash.ZhashDivertStart(processIdListZhash, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DZhashHandle.ToString() + ". Initiated by " + processId.ToString() + " (Zhash) to divert process list: " + " " + String.Join(",", processIdListZhash));
                    return DZhashHandle;
                }
            }

            //******************************************************************************************
            if (CurrentAlgorithmType == 44) //Cuckarood29
            {
                Cuckarood29divert_running = true;
                if (MinerName.ToLower() == "gminer")
                {
                    processIdListCuckarood29.Add("gminer: force");
                    gminer_runningCuckarood29 = true;
                    GetGMinerCuckarood29(processId, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert handle: " + DCuckarood29Handle.ToString());
                }
                /*
                if (MinerName.ToLower() == "nbminer")
                {
                    processIdListCuckarood29.Add("nbminer: " + processId.ToString());
                    if (processIdListCuckarood29.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DCuckarood29Handle.ToString() + ". Added " + processId.ToString() + " (Cuckarood29) to divert process list: " + " " + String.Join(",", processIdListCuckarood29));
                        return DCuckarood29Handle;
                    }
                    DCuckarood29Handle = DCuckarood29.Cuckarood29DivertStart(processIdListCuckarood29, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DCuckarood29Handle.ToString() + ". Initiated by " + processId.ToString() + " (Cuckarood29) to divert process list: " + " " + String.Join(",", processIdListCuckarood29));
                    return DCuckarood29Handle;
                }
                */
            }

            //******************************************************************************************
            if (CurrentAlgorithmType == 8) //Neoscrypt
            {
                Neoscryptdivert_running = true;

                //if (MinerName.ToLower() == "miniz")
                {
                    processIdListNeoscrypt.Add(MinerName + ": " + processId.ToString());
                    if (processIdListNeoscrypt.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DNeoscryptHandle.ToString() + ". Added " + processId.ToString() + " (Neoscrypt) to divert process list: " + " " + String.Join(",", processIdListNeoscrypt));
                        return DNeoscryptHandle;
                    }
                    DNeoscryptHandle = DNeoscrypt.NeoscryptDivertStart(processIdListNeoscrypt, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DNeoscryptHandle.ToString() + ". Initiated by " + processId.ToString() + " (Neoscrypt) to divert process list: " + " " + String.Join(",", processIdListNeoscrypt));
                    return DNeoscryptHandle;
                }
            }
            //******************************************************************************************
            if (CurrentAlgorithmType == 51 || SecondaryAlgorithmType == 51) //Handshake
            {
                Handshakedivert_running = true;
                if (MinerName.ToLower() == "gminer")
                {
                    processIdListHandshake.Add("gminer: force");
                    gminer_runningHandshake = true;
                    GetGMinerHandshake(processId, CurrentAlgorithmType, MinerName, strPlatform);
                }

                if (MinerName.ToLower() == "nbminer")
                {
                    processIdListHandshake.Add("nbiner: " + processId.ToString());
                    if (processIdListHandshake.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DHandshakeHandle.ToString() + ". Added " + processId.ToString() + " (Handshake) to divert process list: " + " " + String.Join(",", processIdListHandshake));
                        return DHandshakeHandle;
                    }
                    DHandshakeHandle = DHandshake.HandshakeDivertStart(processIdListHandshake, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DHandshakeHandle.ToString() + ". Initiated by " + processId.ToString() + " (Handshake) to divert process list: " + " " + String.Join(",", processIdListHandshake));
                    return DHandshakeHandle;
                }
            }
            //******************************************************************************************
            if (CurrentAlgorithmType == 43) //cuckoocycle
            {
                CuckooCycledivert_running = true;
                if (MinerName.ToLower() == "gminer")
                {
                    processIdListCuckooCycle.Add("gminer: force");
                    gminer_runningCuckooCycle = true;
                    GetGMinerCuckooCycle(processId, CurrentAlgorithmType, MinerName, strPlatform);
                }

                if (MinerName.ToLower() == "nbminer")
                {
                    processIdListCuckooCycle.Add("nbminer: " + processId.ToString());
                    if (processIdListCuckooCycle.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DCuckooCycleHandle.ToString() + ". Added " + processId.ToString() + " (CuckooCycle) to divert process list: " + " " + String.Join(",", processIdListCuckooCycle));
                        return DCuckooCycleHandle;
                    }
                    DCuckooCycleHandle = DCuckooCycle.CuckooCycleDivertStart(processIdListCuckooCycle, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DCuckooCycleHandle.ToString() + ". Initiated by " + processId.ToString() + " (CuckooCycle) to divert process list: " + " " + String.Join(",", processIdListCuckooCycle));
                    return DCuckooCycleHandle;
                }
            }
            //******************************************************************************************
            if (CurrentAlgorithmType == 46) //x16rv2
            {
                if (MinerName.ToLower().Contains("trex"))
                {
                    processIdListX16rV2.Add("trex: " + processId.ToString());
                    if (processIdListX16rV2.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DX16rV2Handle.ToString() + ". Added " + processId.ToString() + " (X16rV2) to divert process list: " + " " + String.Join(",", processIdListX16rV2));
                        return DX16rV2Handle;
                    }
                    DX16rV2Handle = DX16rV2.X16rV2DivertStart(processIdListX16rV2, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DX16rV2Handle.ToString() + ". Initiated by " + processId.ToString() + " (X16rV2) to divert process list: " + " " + String.Join(",", processIdListX16rV2));
                    return DX16rV2Handle;
                }
            }

            //******************************************************************************************
            if (CurrentAlgorithmType == 39 || CurrentAlgorithmType == 49 || CurrentAlgorithmType == 50) //Grin
            {
                if (!Divert._certInstalled) return new IntPtr(-1);
                Grindivert_running = true;
                if (MinerName.ToLower() == "gminer")
                {
                    processIdListGrin.Add("gminer: force");
                    gminer_runningGrin = true;
                    GetGMinerGrin(processId, CurrentAlgorithmType, MinerName, strPlatform);
                }
            }
            
            
            //******************************************************************************************
            if (CurrentAlgorithmType == 45) //beam v2
            {
                if (!Divert._certInstalled) return new IntPtr(-1);
                Beamdivert_running = true;
                if (MinerName.ToLower() == "gminer")
                {
                    processIdListBeam.Add("gminer: force");
                    gminer_runningBeam = true;
                    GetGMinerBeam(processId, CurrentAlgorithmType, MinerName, strPlatform);
                }
                if (MinerName.ToLower() == "miniz")
                {
                    processIdListBeam.Add("miniz: " + processId.ToString());
                    if (processIdListBeam.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DBeamHandle.ToString() + ". Added " + processId.ToString() + " (BeamV2) to divert process list: " + " " + String.Join(",", processIdListBeam));
                        return DBeamHandle;
                    }
                    DBeamHandle = DBeam.BeamDivertStart(processIdListBeam, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DBeamHandle.ToString() + ". Initiated by " + processId.ToString() + " (BeamV2) to divert process list: " + " " + String.Join(",", processIdListBeam));
                    return DBeamHandle;
                }
            }

            //******************************************************************************************
            if (CurrentAlgorithmType == -100) //test
            {
                Testdivert_running = true;
                    processIdListTest.Add("NiceHashMinerLegacy: " + processId.ToString());
                    if (processIdListTest.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DTestHandle.ToString() + ". Added " + processId.ToString() + " (Test) to divert process list: " + " " + String.Join(",", processIdListTest));
                        return DTestHandle;
                    }
                DTestHandle = DTest.TestDivertStart(processIdListTest, CurrentAlgorithmType, MinerName, strPlatform);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DTestHandle.ToString() + ". Initiated by " + processId.ToString() + " (Test) to divert process list: " + " " + String.Join(",", processIdListTest));
                    return DTestHandle;
            }
            return new IntPtr(0);
        }

        private static int GetParentProcess(int Id)
        {
            int parentPid = 0;
            using (ManagementObject mo = new ManagementObject("win32_process.handle='" + Id.ToString() + "'"))
            {
                mo.Get();
                parentPid = Convert.ToInt32(mo["ParentProcessId"]);
            }
            return parentPid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ParentProcessUtilities
        {
            // These members must match PROCESS_BASIC_INFORMATION
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;

            [DllImport("ntdll.dll")]
            private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

            /// <summary>
            /// Gets the parent process of the current process.
            /// </summary>
            /// <returns>An instance of the Process class.</returns>
            public static Process GetParentProcess()
            {
                return GetParentProcess(Process.GetCurrentProcess().Handle);
            }

            /// <summary>
            /// Gets the parent process of specified process.
            /// </summary>
            /// <param name="id">The process id.</param>
            /// <returns>An instance of the Process class.</returns>
            public static Process GetParentProcess(int id)
            {
                Process process = Process.GetProcessById(id);
                return GetParentProcess(process.Handle);
            }

            /// <summary>
            /// Gets the parent process of a specified process.
            /// </summary>
            /// <param name="handle">The process handle.</param>
            /// <returns>An instance of the Process class.</returns>
            public static Process GetParentProcess(IntPtr handle)
            {
                ParentProcessUtilities pbi = new ParentProcessUtilities();
                int returnLength;
                int status = NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
                if (status != 0)
                    throw new Win32Exception(status);

                try
                {
                    return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
                }
                catch (ArgumentException)
                {
                    // not found
                    return null;
                }
            }
        }
        public static int GetChildProcess(int ProcessId)
        {
            Process[] localByName = Process.GetProcessesByName("miner");
            foreach (var processName in localByName)
            {
                int t = Process.GetProcessById(processName.Id).Id;
                int p = GetParentProcess(t);
                if (p == ProcessId)
                {
                    return t;
                }
            }

            return -1;
        }

        //GMiner порождает дочерний процесс который поднимает соединение с devfee пулом через 10-15 секунд после старта
        [HandleProcessCorruptedStateExceptions]
        internal static Task<bool> GetGMinerEthash(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListEthash.Add("gminer: " + processId.ToString() + " null");
                DEthashHandle = DEthash.EthashDivertStart(processIdListEthash, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DEthashHandle.ToString() + ". Initiated by " + processId.ToString() + " (ethash) to divert process list: " + " " + String.Join(",", processIdListEthash));

                do
                {
                    childPID = GetChildProcess(processId);
                    if (childPID > 0) 
                    {
                           if (!String.Join(" ", processIdListEthash).Contains(childPID.ToString()))
                           {
                           processIdListEthash.Add("gminer: " + processId.ToString() + " " + childPID.ToString() + " %" + DEthashHandle.ToString());
                                    Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Ethash ChildPid: " + childPID.ToString());
                                    processIdListEthash.RemoveAll(x => x.Contains("gminer: force"));
                                    Helpers.ConsolePrint("WinDivertSharp", "processIdListEthash: " + String.Join(" ", processIdListEthash));
                                    //break;
                           }
                                
                    }
                    Thread.Sleep(400);
                } while (gminer_runningEthash);
                return t.Task;
            });
        }
        //************************************************************************
        internal static Task<bool> GetGMinerZhash(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListZhash.Add("gminer: " + processId.ToString() + " null");
                DZhashHandle = DZhash.ZhashDivertStart(processIdListZhash, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DZhashHandle.ToString() + ". Initiated by " + processId.ToString() + " (Zhash) to divert process list: " + " " + String.Join(",", processIdListZhash));

                do
                {
                    childPID = GetChildProcess(processId);

                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListZhash).Contains(childPID.ToString()))
                        {
                            processIdListZhash.Add("gminer: " + processId.ToString() + " " + childPID.ToString() + " %" + DZhashHandle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Zhash ChildPid: " + childPID.ToString() + " DZhashHandle: " + DZhashHandle.ToString());
                            processIdListZhash.RemoveAll(x => x.Contains("gminer: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListZhash: " + String.Join(" ", processIdListZhash));
                            //break;
                        }

                    }
                    Thread.Sleep(400);
                } while (gminer_runningZhash);
                return t.Task;
            });
        }

        //************************************************************************
        internal static Task<bool> GetGMinerCuckarood29(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListCuckarood29.Add("gminer: " + processId.ToString() + " null");
                DCuckarood29Handle = DCuckarood29.Cuckarood29DivertStart(processIdListCuckarood29, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DCuckarood29Handle.ToString() + ". Initiated by " + processId.ToString() + " (Cuckarood29) to divert process list: " + " " + String.Join(",", processIdListCuckarood29));

                do
                {
                    childPID = GetChildProcess(processId);

                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListCuckarood29).Contains(childPID.ToString()))
                        {
                            processIdListCuckarood29.Add("gminer: " + processId.ToString() + " " + childPID.ToString() + " %" + DCuckarood29Handle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Cuckarood29 ChildPid: " + childPID.ToString() + " DCuckarood29Handle: " + DCuckarood29Handle.ToString());
                            processIdListCuckarood29.RemoveAll(x => x.Contains("gminer: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListCuckarood29: " + String.Join(" ", processIdListCuckarood29));
                            //break;
                        }

                    }
                    Thread.Sleep(400);
                } while (gminer_runningCuckarood29);
                return t.Task;
            });
        }
        //************************************************************************
        internal static Task<bool> GetGMinerHandshake(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListHandshake.Add("gminer: " + processId.ToString() + " null");
                DHandshakeHandle = DHandshake.HandshakeDivertStart(processIdListHandshake, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DHandshakeHandle.ToString() + ". Initiated by " + processId.ToString() + " (Handshake) to divert process list: " + " " + String.Join(",", processIdListHandshake));

                do
                {
                    childPID = GetChildProcess(processId);

                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListHandshake).Contains(childPID.ToString()))
                        {
                            processIdListHandshake.Add("gminer: " + processId.ToString() + " " + childPID.ToString() + " %" + DHandshakeHandle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Handshake ChildPid: " + childPID.ToString());
                            processIdListHandshake.RemoveAll(x => x.Contains("gminer: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListHandshake: " + String.Join(" ", processIdListHandshake));
                            //break;
                        }

                    }
                    Thread.Sleep(400);
                } while (gminer_runningHandshake);
                return t.Task;
            });
        }
        //************************************************************************
        internal static Task<bool> GetGMinerCuckooCycle(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListCuckooCycle.Add("gminer: " + processId.ToString() + " null");
                DCuckooCycleHandle = DCuckooCycle.CuckooCycleDivertStart(processIdListCuckooCycle, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DCuckooCycleHandle.ToString() + ". Initiated by " + processId.ToString() + " (CuckooCycle) to divert process list: " + " " + String.Join(",", processIdListCuckooCycle));

                do
                {
                    childPID = GetChildProcess(processId);

                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListCuckooCycle).Contains(childPID.ToString()))
                        {
                            processIdListCuckooCycle.Add("gminer: " + processId.ToString() + " " + childPID.ToString() + " %" + DCuckooCycleHandle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner CuckooCycle ChildPid: " + childPID.ToString());
                            processIdListCuckooCycle.RemoveAll(x => x.Contains("gminer: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListCuckooCycle: " + String.Join(" ", processIdListCuckooCycle));
                            //break;
                        }

                    }
                    Thread.Sleep(400);
                } while (gminer_runningCuckooCycle);
                return t.Task;
            });
        }
        //************************************************************************
        internal static Task<bool> GetGMinerGrin(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListGrin.Add("gminer: " + processId.ToString() + " null");
                DGrinHandle = DGrin.GrinDivertStart(processIdListGrin, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DGrinHandle.ToString() + ". Initiated by " + processId.ToString() + " (Grin) to divert process list: " + " " + String.Join(",", processIdListGrin));

                do
                {
                    childPID = GetChildProcess(processId);

                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListGrin).Contains(childPID.ToString()))
                        {
                            processIdListGrin.Add("gminer: " + processId.ToString() + " " + childPID.ToString() + " %" + DGrinHandle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Grin ChildPid: " + childPID.ToString());
                            processIdListGrin.RemoveAll(x => x.Contains("gminer: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListGrin: " + String.Join(" ", processIdListGrin));
                            //break;
                        }

                    }
                    Thread.Sleep(300);
                } while (gminer_runningGrin);
                return t.Task;
            });
        }
        
        //************************************************************************
        internal static Task<bool> GetGMinerBeam(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListBeam.Add("gminer: " + processId.ToString() + " null");
                DBeamHandle = DBeam.BeamDivertStart(processIdListBeam, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DBeamHandle.ToString() + ". Initiated by " + processId.ToString() + " (BeamV2) to divert process list: " + " " + String.Join(",", processIdListBeam));

                do
                {
                    childPID = GetChildProcess(processId);

                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListBeam).Contains(childPID.ToString()))
                        {
                            processIdListBeam.Add("gminer: " + processId.ToString() + " " + childPID.ToString() + " %" + DBeamHandle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Beam ChildPid: " + childPID.ToString());
                            processIdListBeam.RemoveAll(x => x.Contains("gminer: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListBeam: " + String.Join(" ", processIdListBeam));
                           // break;
                        }

                    }
                    Thread.Sleep(400);
                } while (gminer_runningBeam);
                return t.Task;
            });
        }

        public static void StopAll()
        {
            processIdListBeam.Clear();
            processIdListCuckarood29.Clear();
            processIdListCuckooCycle.Clear();
            processIdListDagger3GB.Clear();
            processIdListEthash.Clear();
            processIdListGrin.Clear();
            processIdListHandshake.Clear();
            processIdListNeoscrypt.Clear();
            processIdListRandomX.Clear();
            processIdListX16rV2.Clear();
            processIdListZhash.Clear();
        }

        public static void DivertStop(IntPtr DivertHandle, int Pid, int CurrentAlgorithmType, int SecondaryAlgorithmType)
        {
            //ethash
            if (CurrentAlgorithmType == 20 && SecondaryAlgorithmType == -1)
            {
                if (!Divert._certInstalled) return;
                int dh = (int)DivertHandle;
                if (processIdListEthash.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListEthash).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);
                            processIdListEthash.Clear();

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListEthash));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListEthash).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListEthash.Count; i++)
                        {
                            if (processIdListEthash[i].Contains(Pid.ToString()) && processIdListEthash[i].Contains("%"))
                            {
                                int.TryParse(processIdListEthash[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListEthash));
                        processIdListEthash.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListEthash.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListEthash. Stopping Ethash divert thread.");
                        Divert.Ethashdivert_running = false;
                    }
                }
                //check gminer divert is running
                bool gminerfound = false;
                for (var i = 0; i < processIdListEthash.Count; i++)
                {
                    if (processIdListEthash[i].Contains("gminer"))
                    {
                        gminerfound = true;
                    }
                }
                if (gminerfound == false)
                {
                    gminer_runningEthash = false;
                }
            }
            //********************************************************************************************
            
            if (CurrentAlgorithmType == -9) //dagerhashimoto3gb
            {
               
                int dh = (int)DivertHandle;
                if (processIdListDagger3GB.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListDagger3GB).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    Divert.Dagger3GBdivert_running = false;
                    WinDivert.WinDivertClose(DivertHandle);

                    processIdListDagger3GB.Clear();


                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListDagger3GB));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListDagger3GB).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListDagger3GB.Count; i++)
                        {
                            if (processIdListDagger3GB[i].Contains(Pid.ToString()) && processIdListDagger3GB[i].Contains("%"))
                            {
                                int.TryParse(processIdListDagger3GB[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListDagger3GB));
                        processIdListDagger3GB.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListDagger3GB.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListDagger3GB. Stopping Dagger3GB divert thread.");
                        Divert.Dagger3GBdivert_running = false;
                    }
                }
               
            }
        
            //********************************************************************************************
            //zhash
            if (CurrentAlgorithmType == 36)
            {
                Zhashdivert_running = false;
                int dh = (int)DivertHandle;
                if (processIdListZhash.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListZhash).Contains(Pid.ToString()))
                {
                    processIdListZhash.Clear();


                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListZhash));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListZhash).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListZhash.Count; i++)
                        {
                            if (processIdListZhash[i].Contains(Pid.ToString()) && processIdListZhash[i].Contains("%"))
                            {
                                int.TryParse(processIdListZhash[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListZhash));
                        processIdListZhash.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListZhash.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListZhash. Stopping Zhash divert thread.");
                        Zhashdivert_running = false;
                        //Thread.Sleep(50);
                        WinDivert.WinDivertClose(DivertHandle);
                        DivertHandle = new IntPtr(0);
                    }
                }
                //check gminer divert is running
                bool gminerfound = false;
                for (var i = 0; i < processIdListZhash.Count; i++)
                {
                    if (processIdListZhash[i].Contains("gminer"))
                    {
                        gminerfound = true;
                    }
                }
                if (gminerfound == false)
                {
                    gminer_runningZhash = false;
                }
            }

            //********************************************************************************************
            //Cuckarood29
            if (CurrentAlgorithmType == 44)
            {
                Cuckarood29divert_running = false;
                int dh = (int)DivertHandle;
                if (processIdListCuckarood29.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListCuckarood29).Contains(Pid.ToString()))
                {
                    processIdListCuckarood29.Clear();

                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListCuckarood29));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListCuckarood29).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListCuckarood29.Count; i++)
                        {
                            if (processIdListCuckarood29[i].Contains(Pid.ToString()) && processIdListCuckarood29[i].Contains("%"))
                            {
                                int.TryParse(processIdListCuckarood29[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListCuckarood29));
                        processIdListCuckarood29.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListCuckarood29.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListCuckarood29. Stopping Cuckarood29 divert thread.");
                        Cuckarood29divert_running = false;
                        //Thread.Sleep(50);
                        WinDivert.WinDivertClose(DivertHandle);
                        DivertHandle = new IntPtr(0);
                    }
                }
                //check gminer divert is running
                bool gminerfound = false;
                for (var i = 0; i < processIdListCuckarood29.Count; i++)
                {
                    if (processIdListCuckarood29[i].Contains("gminer"))
                    {
                        gminerfound = true;
                    }
                }
                if (gminerfound == false)
                {
                    gminer_runningCuckarood29 = false;
                }
            }
            //********************************************************************************************
            //Neoscrypt
            if (CurrentAlgorithmType == 8)
            {
                int dh = (int)DivertHandle;
                if (processIdListNeoscrypt.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListNeoscrypt).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);


                            processIdListNeoscrypt.Clear();

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListNeoscrypt));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListNeoscrypt).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListNeoscrypt.Count; i++)
                        {
                            if (processIdListNeoscrypt[i].Contains(Pid.ToString()) && processIdListNeoscrypt[i].Contains("%"))
                            {
                                int.TryParse(processIdListNeoscrypt[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListNeoscrypt));
                        processIdListNeoscrypt.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListNeoscrypt.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListNeoscrypt. Stopping Neoscrypt divert thread.");
                        Neoscryptdivert_running = false;
                    }
                }
            }

            //********************************************************************************************
            if (CurrentAlgorithmType == 51 || SecondaryAlgorithmType == 51)//Handshake
            {
                int dh = (int)DivertHandle;
                if (processIdListHandshake.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListHandshake).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                            processIdListHandshake.Clear();

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListHandshake));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListHandshake).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListHandshake.Count; i++)
                        {
                            if (processIdListHandshake[i].Contains(Pid.ToString()) && processIdListHandshake[i].Contains("%"))
                            {
                                int.TryParse(processIdListHandshake[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListHandshake));
                        processIdListHandshake.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListHandshake.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListHandshake. Stopping Handshake divert thread.");
                        Handshakedivert_running = false;
                    }
                }
                //check gminer divert is running
                bool gminerfound = false;
                for (var i = 0; i < processIdListHandshake.Count; i++)
                {
                    if (processIdListHandshake[i].Contains("gminer"))
                    {
                        gminerfound = true;
                    }
                }
                if (gminerfound == false)
                {
                    gminer_runningHandshake = false;
                }
            }
            //*********************************************************************
            if (CurrentAlgorithmType == 43) //cuckoocycle
            {
                int dh = (int)DivertHandle;
                if (processIdListCuckooCycle.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListCuckooCycle).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                            processIdListCuckooCycle.Clear();

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListCuckooCycle));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListCuckooCycle).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListCuckooCycle.Count; i++)
                        {
                            if (processIdListCuckooCycle[i].Contains(Pid.ToString()) && processIdListCuckooCycle[i].Contains("%"))
                            {
                                int.TryParse(processIdListCuckooCycle[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListCuckooCycle));
                        processIdListCuckooCycle.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListCuckooCycle.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListCuckooCycle. Stopping CuckooCycle divert thread.");
                        CuckooCycledivert_running = false;
                    }
                }
                //check gminer divert is running
                bool gminerfound = false;
                for (var i = 0; i < processIdListCuckooCycle.Count; i++)
                {
                    if (processIdListCuckooCycle[i].Contains("gminer"))
                    {
                        gminerfound = true;
                    }
                }
                if (gminerfound == false)
                {
                    gminer_runningCuckooCycle = false;
                }
            }
            //********************************************************************************************
            //x16rv2
            if (CurrentAlgorithmType == 46)
            {
                int dh = (int)DivertHandle;
                if (processIdListX16rV2.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListX16rV2).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                            processIdListX16rV2.Clear();

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListX16rV2));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListX16rV2).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListX16rV2.Count; i++)
                        {
                            if (processIdListX16rV2[i].Contains(Pid.ToString()) && processIdListX16rV2[i].Contains("%"))
                            {
                                int.TryParse(processIdListX16rV2[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListX16rV2));
                        processIdListX16rV2.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListX16rV2.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListX16rV2. Stopping X16rV2 divert thread.");
                        X16rV2divert_running = false;
                    }
                }
            }

            //********************************************************************************************
            //Grin
            if (CurrentAlgorithmType == 39 || CurrentAlgorithmType == 49 || CurrentAlgorithmType == 50) //Grin
            {
                if (!Divert._certInstalled) return;
                int dh = (int)DivertHandle;
                if (processIdListGrin.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListGrin).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                            processIdListGrin.Clear();

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListGrin));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListGrin).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListGrin.Count; i++)
                        {
                            if (processIdListGrin[i].Contains(Pid.ToString()) && processIdListGrin[i].Contains("%"))
                            {
                                int.TryParse(processIdListGrin[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListGrin));
                        processIdListGrin.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListGrin.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListGrin. Stopping Grin divert thread.");
                        Grindivert_running = false;
                    }
                }
                //check gminer divert is running
                bool gminerfound = false;
                for (var i = 0; i < processIdListGrin.Count; i++)
                {
                    if (processIdListGrin[i].Contains("gminer"))
                    {
                        gminerfound = true;
                    }
                }
                if (gminerfound == false)
                {
                    gminer_runningGrin = false;
                }
            }
            //********************************************************************************************
            
            //randomx
            /*
            if (CurrentAlgorithmType == 47)
            {
                int dh = (int)DivertHandle;
                if (processIdListRandomX.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListRandomX).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                    for (var i = 0; i < processIdListRandomX.Count; i++)
                    {
                        if (processIdListRandomX[i].Contains(Pid.ToString()))
                        {
                            processIdListRandomX.RemoveAt(i);
                        }
                    }

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListRandomX));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListRandomX).Contains(Pid.ToString()))
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListRandomX));
                        for (var i = 0; i < processIdListRandomX.Count; i++)
                        {
                            if (processIdListRandomX[i].Contains(Pid.ToString()))
                            {
                                processIdListRandomX.RemoveAt(i);
                                i = 0;
                                continue;
                            }
                        }
                    }
                    if (processIdListRandomX.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListRandomX. Stopping RandomX divert thread.");
                        RandomXdivert_running = false;
                    }
                }
                
            }
            */
            //********************************************************************************************
            //beam
            
            if (CurrentAlgorithmType == 45)
            {
                if (!Divert._certInstalled) return;
                int dh = (int)DivertHandle;
                if (processIdListBeam.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListBeam).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                            processIdListBeam.Clear();

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListBeam));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListBeam).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListBeam.Count; i++)
                        {
                            if (processIdListBeam[i].Contains(Pid.ToString()) && processIdListBeam[i].Contains("%"))
                            {
                                int.TryParse(processIdListBeam[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListBeam));
                        processIdListBeam.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListBeam.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListBeam. Stopping BeamV2 divert thread.");
                        Beamdivert_running = false;
                    }
                }
                //check gminer divert is running
                bool gminerfound = false;
                for (var i = 0; i < processIdListBeam.Count; i++)
                {
                    if (processIdListBeam[i].Contains("gminer"))
                    {
                        gminerfound = true;
                    }
                }
                if (gminerfound == false)
                {
                    gminer_runningBeam = false;
                }
            }
            //********************************************************************************************
            //test

            if (CurrentAlgorithmType == -100)
            {
                int dh = (int)DivertHandle;
                if (processIdListTest.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListTest).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                    processIdListTest.Clear();

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListTest));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListTest).Contains(Pid.ToString()))
                    {
                        for (var i = 0; i < processIdListTest.Count; i++)
                        {
                            if (processIdListTest[i].Contains(Pid.ToString()) && processIdListTest[i].Contains("%"))
                            {
                                int.TryParse(processIdListTest[i].Split('%')[1], out var dHandle);
                                Helpers.ConsolePrint("WinDivertSharp", "Try to close divert handle: " + dHandle.ToString());
                                WinDivert.WinDivertClose((IntPtr)dHandle);
                                break;
                            }
                        }
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListTest));
                        processIdListTest.RemoveAll(x => x.Contains(Pid.ToString()));

                    }
                    if (processIdListTest.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListTest. Stopping Test divert thread.");
                        Testdivert_running = false;
                    }
                }
                
            }
        }
    }
}
