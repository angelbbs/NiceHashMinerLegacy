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
using System.Diagnostics;
using System.Security.Principal;
using System.Management;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Linq;
using HashLib;

namespace NiceHashMinerLegacy.Divert
{
    public class DDagger3GB
    {
        private static IntPtr DivertHandle;

        private static string filter = "";

        private static string PacketPayloadData;
        private static string OwnerPID = "-1";

        private static List<string> _oldPorts = new List<string>();

        private static WinDivertBuffer newpacket = new WinDivertBuffer();
        private static WinDivertParseResult parse_result;
        private static string job = "";



        public static byte[] StringToByteArray(String hex)
        {
            int numChars = hex.Length;
            byte[] bytes = new byte[numChars / 2];
            for (int i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static int Epoch(string Seedhash)
        {
            byte[] seedhashArray = StringToByteArray(Seedhash);
            byte[] s = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            IHash hash = HashFactory.Crypto.SHA3.CreateKeccak256();
            int i;
            for (i = 0; i < 2048; ++i)
            {
                if (s.SequenceEqual(seedhashArray))
                    break;
                s = hash.ComputeBytes(s).GetBytes();
            }
            if (i >= 2048)
                throw new Exception("Invalid seedhash.");
            return i;
        }


        [HandleProcessCorruptedStateExceptions]
        public static IntPtr Dagger3GBDivertStart(List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform, int MaxEpoch)
            {
            Divert.Dagger3GBdivert_running = true;

            filter = "(!loopback && outbound ? (tcp.DstPort == 3353)" +
                " : " +
                "(tcp.SrcPort == 3353)" +
                ")";

            DivertHandle = Divert.OpenWinDivert(filter);
            if (DivertHandle == IntPtr.Zero || DivertHandle == new IntPtr(-1))
            {
                return new IntPtr(-1);
            }

            RunDivert(DivertHandle, processIdList, CurrentAlgorithmType, MinerName, strPlatform, MaxEpoch);

            return DivertHandle;
        }

        [HandleProcessCorruptedStateExceptions]
        internal static Task<bool> RunDivert(IntPtr handle, List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform, int MaxEpoch)
        {

            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                RunDivert1(handle, processIdList, CurrentAlgorithmType, MinerName, strPlatform, MaxEpoch);
                return t.Task;
            });
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HandleProcessCorruptedStateExceptions]
        internal unsafe static async Task RunDivert1(IntPtr handle, List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform, int MaxEpoch)
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
                    if (Divert.Dagger3GBdivert_running)
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

                                Divert.Dagger3GBdivert_running = false;
                                Helpers.ConsolePrint($"WinDivertSharp", "WinDivertRecv error.");
                                continue;
                            }
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


                        if (addr.Direction == WinDivertDirection.Inbound && !OwnerPID.Equals("-1"))
                        {
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            //******************************
                            if (parse_result.PacketPayloadLength > 20)
                                {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                PacketPayloadData = PacketPayloadData.Replace("}{", "}" + (char)10 + "{");
                                //Helpers.ConsolePrint("WinDivertSharp", "<- " + PacketPayloadData);

                                if (PacketPayloadData.Contains("mining.notify") && PacketPayloadData.Contains("method"))//job
                                {
                                    int amount = PacketPayloadData.Split(new char[] { (char)10 }, StringSplitOptions.None).Count() - 1;

                                    //Helpers.ConsolePrint("WinDivertSharp", "amount: " + amount.ToString());
                                    for (var i = 0; i <= amount; i++)
                                    {
                                        //Helpers.ConsolePrint("WinDivertSharp", "PacketPayloadData.Split((char)10)[i]: " + PacketPayloadData.Split((char)10)[i]);
                                        if (PacketPayloadData.Split((char)10)[i].Contains("mining.notify"))
                                        //if (PacketPayloadData.Split('}')[i].Contains("mining.notify"))
                                        {
                                            dynamic json = JsonConvert.DeserializeObject(PacketPayloadData.Split((char)10)[i]);
                                            string seedhash = json.@params[1];
                                            //Helpers.ConsolePrint("WinDivertSharp", "seedhash = " + seedhash);
                                            var epoch = Epoch(seedhash);
                                            Helpers.ConsolePrint("WinDivertSharp", "Epoch = " + epoch.ToString());

                                            if (epoch <= MaxEpoch) //win 7
                                            {
                                                Divert.Dagger3GBEpochCount = 0;
                                            }
                                            else
                                            {
                                                packet.Dispose();
                                                Divert.Dagger3GBEpochCount++;

                                                if (Divert.Dagger3GBEpochCount > 0)
                                                {
                                                    Divert.DaggerHashimoto3GBForce = true;
                                                    Divert.Dagger3GBEpochCount = 999;
                                                    Divert.checkConnection3GB = false;
                                                }
                                                goto nextCycle;
                                            }
                                        }
                                    }
                                }
                            }
                            //******************************
                        }
                        /*
                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                        parse_result.TcpHeader->Checksum = 0;
                        var crc = Divert.CalcTCPChecksum(packet, readLen);

                        parse_result.IPv4Header->Checksum = 0;
                        var pIPv4Header = Divert.getBytes(*parse_result.IPv4Header);
                        var crch = Divert.CalcIpChecksum(pIPv4Header, pIPv4Header.Length);

                        parse_result.IPv4Header->Checksum = crch;
                        parse_result.TcpHeader->Checksum = crc;
                        */
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
                    /*
                    parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                    parse_result.TcpHeader->Checksum = 0;
                    var crc = Divert.CalcTCPChecksum(packet, readLen);

                    parse_result.IPv4Header->Checksum = 0;
                    var pIPv4Header = Divert.getBytes(*parse_result.IPv4Header);
                    var crch = Divert.CalcIpChecksum(pIPv4Header, pIPv4Header.Length);

                    parse_result.IPv4Header->Checksum = crch;
                    parse_result.TcpHeader->Checksum = crc;

                    if (!WinDivert.WinDivertSend(handle, packet, readLen, ref addr))
                    {
                        Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
                    }
                    */
                }
                Thread.Sleep(1);
            }
            while (Divert.Dagger3GBdivert_running);

        }
    }
}
