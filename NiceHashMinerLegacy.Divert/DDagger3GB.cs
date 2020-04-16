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

        internal static string CheckParityConnections(List<string> processIdList, ushort Port, WinDivertDirection dir)
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
            } finally
            {
               
            }
            return "unknown: ?";
        }

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
        public static IntPtr Dagger3GBDivertStart(List<string> processIdList, int CurrentAlgorithmType, string MinerName, string strPlatform)
            {
            Divert.Dagger3GBdivert_running = true;

            filter = "(!loopback && outbound ? (tcp.DstPort == 3353)" +
                " : " +
                "(tcp.SrcPort == 3353)" +
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
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction);
                        }
                        else
                        {
                            OwnerPID = CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction);
                        }


                        if (addr.Direction == WinDivertDirection.Inbound && !OwnerPID.Equals("-1"))
                        {
                            /*
                            Helpers.ConsolePrint("WinDivertSharp", "(" + OwnerPID.ToString() + ") DAGGER3GB SESSION: <- " +
                                "DevFee SrcAdr: " + parse_result.IPv4Header->SrcAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->SrcPort).ToString() +
                                "  DevFee DstAdr: " + parse_result.IPv4Header->DstAddr.ToString() + ":" + Divert.SwapOrder(parse_result.TcpHeader->DstPort).ToString() +
                                " len: " + readLen.ToString());
*/
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            //******************************
                            if (parse_result.PacketPayloadLength > 20)
                                {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                PacketPayloadData = PacketPayloadData.Replace("}{", "}" + (char)10 + "{");
                                Helpers.ConsolePrint("WinDivertSharp", "<- " + PacketPayloadData);

                                if (PacketPayloadData.Contains("mining.notify") && PacketPayloadData.Contains("method"))//job
                                {
                                    int amount = PacketPayloadData.Split(new char[] { (char)10 }, StringSplitOptions.None).Count() - 1;

                                    Helpers.ConsolePrint("WinDivertSharp", "amount: " + amount.ToString());
                                    for (var i = 0; i <= amount; i++)
                                    {
                                        Helpers.ConsolePrint("WinDivertSharp", "PacketPayloadData.Split((char)10)[i]: " + PacketPayloadData.Split((char)10)[i]);
                                        if (PacketPayloadData.Split((char)10)[i].Contains("mining.notify"))
                                        //if (PacketPayloadData.Split('}')[i].Contains("mining.notify"))
                                        {
                                            dynamic json = JsonConvert.DeserializeObject(PacketPayloadData.Split((char)10)[i]);
                                            string seedhash = json.@params[1];
                                            Helpers.ConsolePrint("WinDivertSharp", "seedhash = " + seedhash);
                                            var epoch = Epoch(seedhash);
                                            Helpers.ConsolePrint("WinDivertSharp", "Epoch = " + epoch.ToString());

                                            if (epoch < 235) //win 7
                                            {
                                                Divert.Dagger3GBEpochCount = 0;
                                            }
                                            else
                                            {
                                                Divert.Dagger3GBEpochCount++;
                                                if (Divert.Dagger3GBEpochCount > 2)
                                                {
                                                    Divert.DaggerHashimoto3GBForce = true;
                                                }
                                                //Divert.Dagger3GBEpochCount = 999;
                                                Helpers.ConsolePrint("WinDivertSharp", "Epoch = " + epoch.ToString());
                                                //packet.Dispose();
                                                //goto nextCycle;
                                                /*
                                                if (Divert.Dagger3GBJob.Length > 10)
                                                {
                                                */
                                                //Job not found
                                                /*
                                                dynamic json3gb = JsonConvert.DeserializeObject(Divert.Dagger3GBJob);
                                                string hash1_3gb = json3gb.@params[0];
                                                string seedhash3gb = json3gb.@params[1];
                                                string hash3_3gb = json3gb.@params[2];
                                                var epoch3gb = Epoch(seedhash3gb);

                                                Helpers.ConsolePrint("WinDivertSharp", "Additional job. Epoch = " + epoch3gb.ToString());
                                                json.@params[0] = hash1_3gb; 
                                                json.@params[1] = seedhash3gb; 
                                                json.@params[2] = hash3_3gb;

                                                PacketPayloadData = JsonConvert.SerializeObject(json).Replace(" ", "") + (char)10;
                                                var modpacket = Divert.MakeNewPacket(packet, readLen, PacketPayloadData);
                                                packet.Dispose();
                                                packet = modpacket;
                                                readLen = packet.Length;
                                                WinDivert.WinDivertHelperCalcChecksums(packet, readLen, ref addr, WinDivertChecksumHelperParam.NoIpChecksum);
                                                */
                                                /*
                                            } else
                                            {
                                                Divert.Dagger3GBEpochCount = 999;
                                                Helpers.ConsolePrint("WinDivertSharp", "Epoch = " + epoch.ToString());
                                            }
                                            */
                                            }

                                        }
                                    }
                                    
                                }
                            }
                            //******************************
                        }


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
                }
                Thread.Sleep(1);
            }
            while (Divert.Dagger3GBdivert_running);

        }
    }
}
