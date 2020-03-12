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
        public static bool gminer_runningEthash = false;
        public static bool gminer_runningZhash = false;
        public static bool gminer_runningCuckaroom29 = false;
        public static bool gminer_runningBeam = false;
        public static volatile bool Ethashdivert_running = true;
        public static volatile bool Zhashdivert_running = true;
        public static volatile bool Cuckaroom29divert_running = true;
        public static volatile bool RandomXdivert_running = true;
        public static volatile bool Beamdivert_running = true;

        public static bool BlockGMinerApacheTomcat;
        //public static WinDivertBuffer SendDataPacket = new WinDivertBuffer(new byte[]
        public static byte[] SendDataPacket =  (new byte[]
{
    /*
            0x45, 0x00, 0x00, 0xC2, 0x92, 0x23, 0xC3, 0x92,
            0x40, 0x00, 0xC2, 0x80, 0x06, 0x00, 0x00, 0xC3,
            0x80, 0xC2, 0xA8, 0x01, 0x6F, 0xC2, 0xAC, 0x41,
            0xC3, 0x8F, 0x6A, 0xC3, 0x96, 0xC2, 0xAF, 0x11,
            0x5C, 0xC3, 0xB7, 0xC2, 0x85, 0x6B, 0xC3, 0x87,
            0x05, 0xC2, 0x9C, 0x29, 0xC3, 0x90, 0x50, 0x18,
            0x40, 0x29, 0x54, 0x14, 0x00, 0x00
            */
            0x45, 0x00, 0x00, 0xC2, 0x92, 0x13, 0xC3, 0xB9,
            0x40, 0x00, 0xC2, 0x80, 0x06, 0x00, 0x00, 0xC3,
            0x80, 0xC2, 0xA8, 0x01, 0x6F, 0xC2, 0xAC, 0x41,
            0xC3, 0x8F, 0x6A, 0xC3, 0xAA, 0xC3, 0xAF, 0x11,
            0x5C, 0x3A, 0x54, 0xC2, 0x8B, 0x3B, 0xC3, 0x87,
            0xC2, 0xB4, 0xC2, 0x99, 0x40, 0x50, 0x18, 0x40,
            0x29, 0x54, 0x14, 0x00, 0x00
    /*
            0x45, 0x00, 0x02, 0x09, 0x48, 0x2d, 0x40, 0x00,
            0x40, 0x06, 0x00, 0x00, 0x0a, 0x0a, 0x0a, 0x0a,
            0x5d, 0xb8, 0xd8, 0x77, 0xa3, 0x1a, 0x00, 0x50,
            0x53, 0x38, 0xcc, 0xc2, 0x56, 0x37, 0xb3, 0x55,
            0x80, 0x18, 0x00, 0x73, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x01, 0x08, 0x0a, 0x00, 0x2c, 0x85, 0x1b,
            0x1b, 0x7f, 0x3a, 0x71, 0x47, 0x45, 0x54, 0x20,
            0x2f, 0x20, 0x48, 0x54, 0x54, 0x50, 0x2f, 0x31,
            0x2e, 0x31, 0x0d, 0x0a, 0x48, 0x6f, 0x73, 0x74,
            0x3a, 0x20, 0x77, 0x77, 0x77, 0x2e, 0x65, 0x78,
            0x61, 0x6d, 0x70, 0x6c, 0x65, 0x2e, 0x63, 0x6f,
            0x6d, 0x0d, 0x0a, 0x43, 0x6f, 0x6e, 0x6e, 0x65,
            0x63, 0x74, 0x69, 0x6f, 0x6e, 0x3a, 0x20, 0x6b,
            0x65, 0x65, 0x70, 0x2d, 0x61, 0x6c, 0x69, 0x76,
            0x65, 0x0d, 0x0a, 0x43, 0x61, 0x63, 0x68, 0x65,
            0x2d, 0x43, 0x6f, 0x6e, 0x74, 0x72, 0x6f, 0x6c,
            0x3a, 0x20, 0x6d, 0x61, 0x78, 0x2d, 0x61, 0x67,
            0x65, 0x3d, 0x30, 0x0d, 0x0a, 0x41, 0x63, 0x63,
            0x65, 0x70, 0x74, 0x3a, 0x20, 0x74, 0x65, 0x78,
            0x74, 0x2f, 0x68, 0x74, 0x6d, 0x6c, 0x2c, 0x61,
            0x70, 0x70, 0x6c, 0x69, 0x63, 0x61, 0x74, 0x69,
            0x6f, 0x6e, 0x2f, 0x78, 0x68, 0x74, 0x6d, 0x6c,
            0x2b, 0x78, 0x6d, 0x6c, 0x2c, 0x61, 0x70, 0x70,
            0x6c, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6f, 0x6e,
            0x2f, 0x78, 0x6d, 0x6c, 0x3b, 0x71, 0x3d, 0x30,
            0x2e, 0x39, 0x2c, 0x69, 0x6d, 0x61, 0x67, 0x65,
            0x2f, 0x77, 0x65, 0x62, 0x70, 0x2c, 0x2a, 0x2f,
            0x2a, 0x3b, 0x71, 0x3d, 0x30, 0x2e, 0x38, 0x0d,
            0x0a, 0x55, 0x73, 0x65, 0x72, 0x2d, 0x41, 0x67,
            0x65, 0x6e, 0x74, 0x3a, 0x20, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58, 0x58,
            0x58, 0x58, 0x0d, 0x0a, 0x41, 0x63, 0x63, 0x65,
            0x70, 0x74, 0x2d, 0x45, 0x6e, 0x63, 0x6f, 0x64,
            0x69, 0x6e, 0x67, 0x3a, 0x20, 0x67, 0x7a, 0x69,
            0x70, 0x2c, 0x64, 0x65, 0x66, 0x6c, 0x61, 0x74,
            0x65, 0x2c, 0x73, 0x64, 0x63, 0x68, 0x0d, 0x0a,
            0x41, 0x63, 0x63, 0x65, 0x70, 0x74, 0x2d, 0x4c,
            0x61, 0x6e, 0x67, 0x75, 0x61, 0x67, 0x65, 0x3a,
            0x20, 0x65, 0x6e, 0x2d, 0x55, 0x53, 0x2c, 0x65,
            0x6e, 0x3b, 0x71, 0x3d, 0x30, 0x2e, 0x38, 0x0d,
            0x0a, 0x49, 0x66, 0x2d, 0x4e, 0x6f, 0x6e, 0x65,
            0x2d, 0x4d, 0x61, 0x74, 0x63, 0x68, 0x3a, 0x20,
            0x22, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33,
            0x33, 0x33, 0x22, 0x0d, 0x0a, 0x49, 0x66, 0x2d,
            0x4d, 0x6f, 0x64, 0x69, 0x66, 0x69, 0x65, 0x64,
            0x2d, 0x53, 0x69, 0x6e, 0x63, 0x65, 0x3a, 0x20,
            0x46, 0x72, 0x69, 0x2c, 0x20, 0x30, 0x33, 0x20,
            0x41, 0x75, 0x67, 0x20, 0x32, 0x30, 0x31, 0x34,
            0x20, 0x31, 0x33, 0x3a, 0x33, 0x33, 0x3a, 0x33,
            0x33, 0x20, 0x47, 0x4d, 0x54, 0x0d, 0x0a, 0x0d,
            0x0a
            */
});


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

        public static List<string> processIdListEthash = new List<string>();
        public static List<string> processIdListZhash = new List<string>();
        public static List<string> processIdListCuckaroom29 = new List<string>();
        public static List<string> processIdListBeam = new List<string>();
        public static List<string> processIdListRandomX = new List<string>();

        private static IntPtr DEthashHandle = (IntPtr)0;
        private static IntPtr DZhashHandle = (IntPtr)0;
        private static IntPtr DCuckaroom29Handle = (IntPtr)0;
        private static IntPtr DBeamHandle = (IntPtr)0;
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
/*
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
*/

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

//            Helpers.ConsolePrint("WinDivertSharp", "TcpHeader->Checksum: " + parse_result.TcpHeader->Checksum.ToString());
//            Helpers.ConsolePrint("WinDivertSharp", "IPv4Header->Checksum: " + parse_result.IPv4Header->Checksum.ToString());

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

        
        [HandleProcessCorruptedStateExceptions]
        public static IntPtr DivertStart(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform, string w, bool log, bool BlockGMinerApacheTomcatConfig)
        {
            logging = log;
            worker = w;
            BlockGMinerApacheTomcat = BlockGMinerApacheTomcatConfig;
            Helpers.ConsolePrint("WinDivertSharp", "Miner: " + MinerName + " Algo: " + CurrentAlgorithmType);
            /*
            if ( CurrentAlgorithmType == 47 && MinerName.ToLower() == "xmrig") //for testing. Disable in productuon
            {
                return IntPtr.Zero;
                //return DXMrig.XMRigDivertStart(processId, CurrentAlgorithmType, MinerName);
            }
            */
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
            if (CurrentAlgorithmType == 20) //dagerhashimoto
            {
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
                }
            }

            //******************************************************************************************
            if (CurrentAlgorithmType == 49) //Cuckaroom29
            {
                Cuckaroom29divert_running = true;
                if (MinerName.ToLower() == "gminer")
                {
                    processIdListCuckaroom29.Add("gminer: force");
                    gminer_runningCuckaroom29 = true;
                    GetGMinerCuckaroom29(processId, CurrentAlgorithmType, MinerName, strPlatform);
                }
            }
            
            /*
            //******************************************************************************************
            if (CurrentAlgorithmType == 45) //beam v2
            {
                Beamdivert_running = true;
                if (MinerName.ToLower() == "gminer")
                {
                    processIdListBeam.Add("gminer: force");
                    gminer_runningBeam = true;
                    GetGMinerBeam(processId, CurrentAlgorithmType, MinerName, strPlatform);
                }
            }
            */
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
                                    processIdListEthash.Add("gminer: " + processId.ToString() + " " + childPID.ToString());
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
                            processIdListZhash.Add("gminer: " + processId.ToString() + " " + childPID.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Zhash ChildPid: " + childPID.ToString());
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
        internal static Task<bool> GetGMinerCuckaroom29(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListCuckaroom29.Add("gminer: " + processId.ToString() + " null");
                DCuckaroom29Handle = DCuckaroom29.Cuckaroom29DivertStart(processIdListCuckaroom29, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DCuckaroom29Handle.ToString() + ". Initiated by " + processId.ToString() + " (Cuckaroom29) to divert process list: " + " " + String.Join(",", processIdListCuckaroom29));

                do
                {
                    childPID = GetChildProcess(processId);

                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListCuckaroom29).Contains(childPID.ToString()))
                        {
                            processIdListCuckaroom29.Add("gminer: " + processId.ToString() + " " + childPID.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Cuckaroom29 ChildPid: " + childPID.ToString());
                            processIdListCuckaroom29.RemoveAll(x => x.Contains("gminer: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListCuckaroom29: " + String.Join(" ", processIdListCuckaroom29));
                            //break;
                        }

                    }
                    Thread.Sleep(400);
                } while (gminer_runningCuckaroom29);
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
                            processIdListBeam.Add("gminer: " + processId.ToString() + " " + childPID.ToString());
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



        public static void DivertStop(IntPtr DivertHandle, int Pid, int CurrentAlgorithmType)
        {
            //ethash
            if (CurrentAlgorithmType == 20)
            {
                int dh = (int)DivertHandle;
                if (processIdListEthash.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListEthash).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                    for (var i = 0; i < processIdListEthash.Count; i++)
                    {
                        if (processIdListEthash[i].Contains(Pid.ToString()))
                        {
                            processIdListEthash.RemoveAt(i);
                        }
                    }

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
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListEthash));
                        processIdListEthash.RemoveAll(x => x.Contains(Pid.ToString()));
                        /*
                        for (var i = 0; i < processIdListEthash.Count; i++)
                        {
                            if (processIdListEthash[i].Contains(Pid.ToString()))
                            {
                                processIdListEthash.RemoveAll(Pid.ToString()));
                                processIdListEthash.RemoveAt(i);
                                i = 0;
                                continue;
                            }
                        }
                        */
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
            //zhash
            if (CurrentAlgorithmType == 36)
            {
                int dh = (int)DivertHandle;
                if (processIdListZhash.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListZhash).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                    for (var i = 0; i < processIdListZhash.Count; i++)
                    {
                        if (processIdListZhash[i].Contains(Pid.ToString()))
                        {
                            processIdListZhash.RemoveAt(i);
                        }
                    }

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListZhash));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListZhash).Contains(Pid.ToString()))
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListZhash));
                        for (var i = 0; i < processIdListZhash.Count; i++)
                        {
                            if (processIdListZhash[i].Contains(Pid.ToString()))
                            {
                                processIdListZhash.RemoveAt(i);
                                i = 0;
                                continue;
                            }
                        }
                    }
                    if (processIdListZhash.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListZhash. Stopping Zhash divert thread.");
                        Zhashdivert_running = false;
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
            //Cuckaroom29
            if (CurrentAlgorithmType == 38)
            {
                int dh = (int)DivertHandle;
                if (processIdListCuckaroom29.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListCuckaroom29).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                    for (var i = 0; i < processIdListCuckaroom29.Count; i++)
                    {
                        if (processIdListCuckaroom29[i].Contains(Pid.ToString()))
                        {
                            processIdListCuckaroom29.RemoveAt(i);
                        }
                    }

                    DivertHandle = new IntPtr(0);
                    Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() +
                        " ProcessID: " + Pid.ToString() + " " + GetProcessName(Pid));
                    Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdListCuckaroom29));
                    Thread.Sleep(50);
                }
                else
                {
                    if (String.Join(" ", Divert.processIdListCuckaroom29).Contains(Pid.ToString()))
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListCuckaroom29));
                        for (var i = 0; i < processIdListCuckaroom29.Count; i++)
                        {
                            if (processIdListCuckaroom29[i].Contains(Pid.ToString()))
                            {
                                processIdListCuckaroom29.RemoveAt(i);
                                i = 0;
                                continue;
                            }
                        }
                    }
                    if (processIdListCuckaroom29.Count < 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdListCuckaroom29. Stopping Cuckaroom29 divert thread.");
                        Cuckaroom29divert_running = false;
                    }
                }
                //check gminer divert is running
                bool gminerfound = false;
                for (var i = 0; i < processIdListCuckaroom29.Count; i++)
                {
                    if (processIdListCuckaroom29[i].Contains("gminer"))
                    {
                        gminerfound = true;
                    }
                }
                if (gminerfound == false)
                {
                    gminer_runningCuckaroom29 = false;
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
            /*
            if (CurrentAlgorithmType == 45)
            {
                int dh = (int)DivertHandle;
                if (processIdListBeam.Count <= 1 && dh != 0 && String.Join(" ", Divert.processIdListBeam).Contains(Pid.ToString()))
                {
                    Thread.Sleep(50);
                    WinDivert.WinDivertClose(DivertHandle);

                    for (var i = 0; i < processIdListBeam.Count; i++)
                    {
                        if (processIdListBeam[i].Contains(Pid.ToString()))
                        {
                            processIdListBeam.RemoveAt(i);
                        }
                    }

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
                        Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() +
                            " " + " " + GetProcessName(Pid) +
                            " from divert process list: " + " " + String.Join(", ", processIdListBeam));
                        for (var i = 0; i < processIdListBeam.Count; i++)
                        {
                            if (processIdListBeam[i].Contains(Pid.ToString()))
                            {
                                processIdListBeam.RemoveAt(i);
                                i = 0;
                                continue;
                            }
                        }
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
            */
        }
    }
}
