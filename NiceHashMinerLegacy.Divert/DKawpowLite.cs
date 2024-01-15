using HashLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WinDivertSharp;

namespace NiceHashMinerLegacy.Divert
{
    public class DKawpowLite
    {
        private static IntPtr DivertHandle;

        private static string filter = "";

        private static string PacketPayloadData;
        private static string OwnerPID = "-1";

        private static List<string> _oldPorts = new List<string>();

        private static WinDivertBuffer newpacket = new WinDivertBuffer();
        private static WinDivertParseResult parse_result;
        private static string job = "";
        private static readonly int ETHASH_EPOCH_LENGTH = 7500;
        private static int KawpowLiteEpochCount = 0;
        private static int KawpowLiteEpochCountSimulate = 0;


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
            int.TryParse(Seedhash, out int sInt);
            return sInt / ETHASH_EPOCH_LENGTH;
        }


        [HandleProcessCorruptedStateExceptions]
        public static IntPtr KawpowLiteDivertStart(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform, int MaxEpoch)
        {
            Divert.KawpowLitedivert_running = true;
            KawpowLiteEpochCount = 0;

            filter = "(!loopback && outbound ? (tcp.DstPort == 13385 || tcp.DstPort == 9200)" +
                " : " +
                "(tcp.SrcPort == 13385 || tcp.SrcPort == 9200)" +
                ")";

            DivertHandle = Divert.OpenWinDivert(filter);
            if (DivertHandle == IntPtr.Zero || DivertHandle == new IntPtr(-1))
            {
                Helpers.ConsolePrint("KawpowLiteDivert", "OpenWinDivert ERROR");
                return new IntPtr(-1);
            }

            RunDivert(DivertHandle, processId, CurrentAlgorithmType, MinerName, strPlatform, MaxEpoch);

            return DivertHandle;
        }

        [HandleProcessCorruptedStateExceptions]
        internal static Task<bool> RunDivert(IntPtr handle, int processId, int CurrentAlgorithmType, string MinerName, string strPlatform, int MaxEpoch)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                RunDivert1(handle, processId, CurrentAlgorithmType, MinerName, strPlatform, MaxEpoch);
                return t.Task;
            });
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [HandleProcessCorruptedStateExceptions]
        internal unsafe static async Task RunDivert1(IntPtr handle, int processId, int CurrentAlgorithmType, string MinerName, string strPlatform, int MaxEpoch)
        {
            var packet = new WinDivertBuffer();
            var addr = new WinDivertAddress();
            uint readLen = 0;
            List<string> InboundPorts = new List<string>();
            List<string> processIdList = new List<string>();
            processIdList.Add(processId.ToString());

            KawpowLiteEpochCountSimulate = 0;

            IntPtr recvEvent = IntPtr.Zero;
            bool result;
            do
            {
                try
                {
                    nextCycle:
                    if (Divert.KawpowLitedivert_running)
                    {
                        readLen = 0;
                        PacketPayloadData = null;
                        packet.Dispose();

                        addr.Reset();

                        packet = new WinDivertBuffer();
                        result = WinDivert.WinDivertRecv(handle, packet, ref addr, ref readLen);

                        if (!result)
                        {
                            {

                                Divert.KawpowLitedivert_running = false;
                                Helpers.ConsolePrint("KawpowLiteDivert", "WinDivertRecv error.");
                                continue;
                            }
                        }

                        if (Divert.KawpowLitedivert_running == false)
                        {
                            break;
                        }

                        parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);

                        if (addr.Direction == WinDivertDirection.Outbound && parse_result != null && processId > 0)
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->SrcPort, addr.Direction, _oldPorts);
                        }
                        else
                        {
                            OwnerPID = Divert.CheckParityConnections(processIdList, parse_result.TcpHeader->DstPort, addr.Direction, _oldPorts);
                        }
                        
                        if (addr.Direction == WinDivertDirection.Inbound && !OwnerPID.Equals("-1"))
                        //if (addr.Direction == WinDivertDirection.Inbound)
                        {
                            parse_result = WinDivert.WinDivertHelperParsePacket(packet, readLen);
                            //******************************
                            if (parse_result.PacketPayloadLength > 20)
                            {
                                PacketPayloadData = Divert.PacketPayloadToString(parse_result.PacketPayload, parse_result.PacketPayloadLength);
                                PacketPayloadData = PacketPayloadData.Replace("}{", "}" + (char)10 + "{");
                                //Helpers.ConsolePrint("KawpowLiteDivert", "<- " + PacketPayloadData);

                                if (PacketPayloadData.Contains("mining.notify") && PacketPayloadData.Contains("method"))//job
                                {
                                    int amount = PacketPayloadData.Split(new char[] { (char)10 }, StringSplitOptions.None).Count() - 1;

                                    //Helpers.ConsolePrint("KawpowLiteDivert", "amount: " + amount.ToString());
                                    for (var i = 0; i <= amount; i++)
                                    {
                                        //Helpers.ConsolePrint("KawpowLiteDivert", "PacketPayloadData.Split((char)10)[i]: " + PacketPayloadData.Split((char)10)[i]);
                                        if (PacketPayloadData.Split((char)10)[i].Contains("mining.notify"))
                                        //if (PacketPayloadData.Split('}')[i].Contains("mining.notify"))
                                        {
                                            dynamic json = JsonConvert.DeserializeObject(PacketPayloadData.Split((char)10)[i]);
                                            string seedhash = json.@params[5];
                                            //Helpers.ConsolePrint("KawpowLiteDivert", "seedhash = " + seedhash);
                                            var epoch = Epoch(seedhash);
                                            /*
                                            KawpowLiteEpochCountSimulate++;
                                            if (KawpowLiteEpochCountSimulate >= 20)
                                            {
                                                epoch = 777;
                                                Helpers.ConsolePrint("KawpowLiteDivert", "Simulate epoch: " + epoch.ToString());
                                            }
                                            */
                                            if (epoch <= MaxEpoch)
                                            {
                                                Helpers.ConsolePrint("KawpowLiteDivert", "Good epoch: " + epoch.ToString());
                                                KawpowLiteEpochCount = 0;
                                                Divert.KawpowLiteForceStop = false;
                                                Divert.KawpowLiteGoodEpoch = true;
                                            }
                                            else
                                            {
                                                Helpers.ConsolePrint("KawpowLiteDivert", "Bad epoch: " + epoch.ToString());
                                                packet.Dispose();
                                                KawpowLiteEpochCount++;

                                                if (KawpowLiteEpochCount == 2)
                                                {
                                                    DropPort(processId, 13385);
                                                    DropPort(processId, 9200);
                                                }

                                                if (KawpowLiteEpochCount >= 3)
                                                {
                                                    Divert.KawpowLiteForceStop = true;
                                                    Divert.KawpowLiteGoodEpoch = false;
                                                    Divert.KawpowLiteMonitorNeedReconnect = true;
                                                    break;
                                                }
                                                goto nextCycle;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!WinDivert.WinDivertSend(handle, packet, readLen, ref addr))
                        {
                            Helpers.ConsolePrint("KawpowLiteDivert", "(" + OwnerPID.ToString() + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
                        }
                    }
                }
                catch (NullReferenceException nfe)
                {
                    Helpers.ConsolePrint("KawpowLiteDivert", "NullReferenceException: " + nfe.ToString());
                }
                catch (Exception e)
                {
                    Helpers.ConsolePrint("KawpowLiteDivert", e.ToString());
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
                        Helpers.ConsolePrint("KawpowLiteDivert", "(" + OwnerPID.ToString() + ") " + "Write Err: {0}", Marshal.GetLastWin32Error());
                    }
                    */
                }
                Thread.Sleep(1);
            }
            while (Divert.KawpowLitedivert_running);
            Helpers.ConsolePrint("KawpowLiteDivert", "WinDivertClose: " + handle.ToInt32().ToString()); 
            //WinDivert.WinDivertClose(DivertHandle);
            WinDivert.WinDivertClose(handle);
        }
        public static void DropPort(int processId, uint port)
        {
            var cports = new Process
            {
                StartInfo =
                        {
                            FileName = "utils/cports-x64/cports.exe",
                            UseShellExecute = false,
                            Arguments = "/close * * * " + port.ToString() + " " + processId.ToString(),
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
            }
                        };

            try
            {
                if (!cports.Start())
                {
                    Helpers.ConsolePrint("DropPort", "cports process could not start");
                }
                else
                {
                    if (cports.WaitForExit(1000))
                    {
                        cports.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("DropPort", ex.ToString());
            }
            Helpers.ConsolePrint("DropPort", "Drop port " + port.ToString() + " completed");
        }
    }
}
