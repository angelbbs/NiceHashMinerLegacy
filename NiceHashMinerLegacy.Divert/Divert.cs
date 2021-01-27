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
        public static bool logging;
        public static volatile bool Dagger3GBdivert_running = true;
        public static volatile bool Dagger4GBdivert_running = true;
        public static bool gminer_runningDagger3GB = false;
        public static bool gminer_runningDagger4GB = false;
        public static int Dagger3GBEpochCount = 0;
        public static int Dagger4GBEpochCount = 0;
        public static string Dagger3GBJob = "";
        public static string Dagger4GBJob = "";
        public static bool Dagger3GBCheckConnection = false;
        public static bool Dagger4GBCheckConnection = false;
        public static bool DaggerHashimoto3GBProfit = false;
        public static bool DaggerHashimoto4GBProfit = false;
        public static bool DaggerHashimoto3GBForce = false;
        public static bool DaggerHashimoto4GBForce = false;
        public static bool checkConnection3GB = false;
        public static bool checkConnection4GB = false;
        public static bool firstRun3GB = false;
        public static bool firstRun4GB = false;
        public static List<string> processIdListDagger3GB = new List<string>();
        public static List<string> processIdListDagger4GB = new List<string>();
        private static IntPtr DDagger3GBHandle = (IntPtr)0;
        private static IntPtr DDagger4GBHandle = (IntPtr)0;
        public static bool _SaveDivertPackets;
        public static bool _certInstalled = false;
        public static volatile bool Testdivert_running = true;
        public static List<string> processIdListTest = new List<string>();
        private static IntPtr DTestHandle = (IntPtr)0;

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




        public static string worker = "";

        public static string LineNumber([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)

        {
            return caller + ": " + lineNumber;
        }

        private static System.Text.ASCIIEncoding ASCII;
        private static IPHostEntry heserver;
        public static string[] DNStoIP(string DivertIPName)
        {
            //var arrString = new string[3];
            string[] arrString = { "111.222.212.121", "111.222.212.121", "111.222.212.121" };
            int i = 0;
            try
            {
                ASCII = new System.Text.ASCIIEncoding();
                heserver = Dns.GetHostEntry(DivertIPName);
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetwork.ToString())
                    {
                        arrString[i] = curAdd.ToString();
                        //return curAdd.ToString();
                       // break; //only 1st IP
                    } else
                    {
                        arrString[i] = "111.222.212.121";
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("WinDivertSharp", "Exception: " + e.ToString());
            }
            return arrString;
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


        internal static Task<bool> GetDagger3GB(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform, int MaxEpoch)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListDagger3GB.Add(MinerName.ToLower() + ": " + processId.ToString());
                DDagger3GBHandle = DDagger3GB.Dagger3GBDivertStart(processIdListDagger3GB, CurrentAlgorithmType, MinerName, strPlatform, MaxEpoch);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DDagger3GBHandle.ToString() + ". Initiated by " + processId.ToString() + " (Dagger3GB) to divert process list: " + " " + String.Join(",", processIdListDagger3GB));

                do
                {
                    childPID = PublicFunc.GetChildProcess(processId);
                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListDagger3GB).Contains(childPID.ToString()))
                        {
                            processIdListDagger3GB.Add(MinerName.ToLower() + ": " + processId.ToString() + " " + childPID.ToString() + " %" + DDagger3GBHandle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new Dagger3GB ChildPid: " + childPID.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListDagger3GB: " + String.Join(" ", processIdListDagger3GB));
                        }

                    }
                    Thread.Sleep(400);
                } while (Dagger3GBdivert_running);
                return t.Task;
            });
        }
        internal static Task<bool> GetDagger4GB(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform, int MaxEpoch)
        {
            return Task.Run(() =>
            {
                var t = new TaskCompletionSource<bool>();
                var _allConnections = new List<Connection>();
                int childPID = -1;

                processIdListDagger4GB.Add(MinerName.ToLower() + ": " + processId.ToString());
                DDagger4GBHandle = DDagger4GB.Dagger4GBDivertStart(processIdListDagger4GB, CurrentAlgorithmType, MinerName, strPlatform, MaxEpoch);
                Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DDagger4GBHandle.ToString() + ". Initiated by " + processId.ToString() + " (Dagger4GB) to divert process list: " + " " + String.Join(",", processIdListDagger4GB));

                do
                {
                    childPID = PublicFunc.GetChildProcess(processId);
                    if (childPID > 0)
                    {
                        if (!String.Join(" ", processIdListDagger4GB).Contains(childPID.ToString()))
                        {
                            processIdListDagger4GB.Add(MinerName.ToLower() + ": " + processId.ToString() + " " + childPID.ToString() + " %" + DDagger4GBHandle.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "Add new Dagger4GB ChildPid: " + childPID.ToString());
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListDagger4GB: " + String.Join(" ", processIdListDagger4GB));
                        }

                    }
                    Thread.Sleep(400);
                } while (Dagger4GBdivert_running);
                return t.Task;
            });
        }


        public static IntPtr OpenWinDivert(string filter)
        {
            uint errorPos = 0;
            IntPtr DivertHandle;
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
            string w, bool log, bool SaveDiverPackets, bool BlockGMinerApacheTomcatConfig, bool DivertEnabled, int MaxEpoch)
        {
            if (!DivertEnabled) return new IntPtr(0); 
            logging = log;
            logging = false;
            _SaveDivertPackets = SaveDiverPackets;
            _SaveDivertPackets = false;
            worker = w;
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
                    DDagger3GBHandle = DDagger3GB.Dagger3GBDivertStart(processIdListDagger3GB, CurrentAlgorithmType, MinerName, strPlatform, MaxEpoch);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DDagger3GBHandle.ToString() + ". Initiated by " + processId.ToString() + " (Dagger3GB) to divert process list: " + " " + String.Join(",", processIdListDagger3GB));
                    return DDagger3GBHandle;

                    //processIdListEthash.Add("gminer: force");
                    //gminer_runningEthash = true;
                    //GetDagger3GB(processId, CurrentAlgorithmType, MinerName, strPlatform);
                }

            }
            //******************************************************************************************
            if (CurrentAlgorithmType == -12) //dagerhashimoto4gb
            {
                Dagger4GBdivert_running = true;
                //if (MinerName.ToLower() == "claymoredual")
                {

                    processIdListDagger4GB.Add(MinerName.ToLower() + ": " + processId.ToString());
                    if (processIdListDagger4GB.Count > 1)
                    {
                        Helpers.ConsolePrint("WinDivertSharp", MinerName + " divert handle: " + DDagger4GBHandle.ToString() + ". Added " + processId.ToString() + " (Dagger4GB) to divert process list: " + " " + String.Join(",", processIdListDagger4GB));
                        return DDagger4GBHandle;
                    }
                    DDagger4GBHandle = DDagger4GB.Dagger4GBDivertStart(processIdListDagger4GB, CurrentAlgorithmType, MinerName, strPlatform, MaxEpoch);
                    Helpers.ConsolePrint("WinDivertSharp", MinerName + " new Divert handle: " + DDagger4GBHandle.ToString() + ". Initiated by " + processId.ToString() + " (Dagger4GB) to divert process list: " + " " + String.Join(",", processIdListDagger4GB));
                    //return DDagger4GBHandle;

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




        public static void DivertStop(IntPtr DivertHandle, int Pid, int CurrentAlgorithmType,
            int SecondaryAlgorithmType, bool DivertEnabled, string MinerName, string platform = "")
        {
            if (!DivertEnabled) return;
            Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + ((int)DivertHandle).ToString() +
                " Pid: " + Pid.ToString() +
                " CurrentAlgorithmType: " + CurrentAlgorithmType.ToString() +
                " SecondaryAlgorithmType: " + SecondaryAlgorithmType.ToString());
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
                        " ProcessID: " + Pid.ToString() + " " + PublicFunc.GetProcessName(Pid));
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
                            " " + " " + PublicFunc.GetProcessName(Pid) +
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
                        " ProcessID: " + Pid.ToString() + " " + PublicFunc.GetProcessName(Pid));
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
                            " " + " " + PublicFunc.GetProcessName(Pid) +
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
