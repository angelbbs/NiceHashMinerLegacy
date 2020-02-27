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
        public static bool gminer_runningBeam = false;
        public static volatile bool Ethashdivert_running = true;
        public static volatile bool Zhashdivert_running = true;
        public static volatile bool Beamdivert_running = true;

        public static bool BlockGMinerApacheTomcat;

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
        public static List<string> processIdListBeam = new List<string>();

        private static IntPtr DEthashHandle = (IntPtr)0;
        private static IntPtr DZhashHandle = (IntPtr)0;
        private static IntPtr DBeamHandle = (IntPtr)0;

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

        
        [HandleProcessCorruptedStateExceptions]
        public static IntPtr DivertStart(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform, bool log, bool BlockGMinerApacheTomcatConfig)
        {

            logging = log;
            BlockGMinerApacheTomcat = BlockGMinerApacheTomcatConfig;

            if ( CurrentAlgorithmType == 47 && MinerName.ToLower() == "xmrig") //for testing. Disable in productuon
            {
                return IntPtr.Zero;
                //return DXMrig.XMRigDivertStart(processId, CurrentAlgorithmType, MinerName);
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
                                    break;
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
                            break;
                        }

                    }
                    Thread.Sleep(400);
                } while (gminer_runningZhash);
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
                            Helpers.ConsolePrint("WinDivertSharp", "Add new GMiner Zhash ChildPid: " + childPID.ToString());
                            processIdListBeam.RemoveAll(x => x.Contains("gminer: force"));
                            Helpers.ConsolePrint("WinDivertSharp", "processIdListBeam: " + String.Join(" ", processIdListBeam));
                            break;
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
