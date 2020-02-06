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
    public class Divert
    {
       // private static Timer _divertTimer;
        
        public static bool logging;

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

        public static List<uint> processIdList = new List<uint>();
        private static IntPtr DClaymore = (IntPtr)0;
        /*
        public void Add(int processId)
        {
            processIdList.Add(processId);
        }
        */
        public static IntPtr DivertStart(int processId, int CurrentAlgorithmType, string MinerName, string strPlatform, bool log)
        {
            //Helpers.ConsolePrint("WinDivertSharp", "Divert START for process ID: " + processId.ToString() + " Miner: " + MinerName + " CurrentAlgorithmType: " + CurrentAlgorithmType);
            logging = log;
            /*
            if (processIdList.Count > 1)
            {
                for (int i = 0; i < processIdList.Count; i++)
                {
                    Helpers.ConsolePrint("WinDivertSharp", "processIdList " + processIdList[i].ToString());
                }
            }
            */
            if ( CurrentAlgorithmType == 47 && MinerName.ToLower() == "xmrig") //for testing. Disable in productuon
            {
                return IntPtr.Zero;
                //return DXMrig.XMRigDivertStart(processId, CurrentAlgorithmType, MinerName);
            }
            //надо передавать id процесса в существующий поток
            if (CurrentAlgorithmType == 20 && MinerName.ToLower() == "claymoredual") 
            {
                processIdList.Add((uint)processId);
                if (processIdList.Count > 1)
                {
                    //Helpers.ConsolePrint("WinDivertSharp", "Mixed rig detected: " + processIdList.Count.ToString() + " " + String.Join(",", processIdList));
                    Helpers.ConsolePrint("WinDivertSharp", "Divert handle: " + DClaymore.ToString() + ". Added " + processId.ToString() + " to divert process list: " + " " + String.Join(",", processIdList));
                    return DClaymore;
                }
                DClaymore = DClaymoreDual.ClaymoreDualDivertStart(processIdList, CurrentAlgorithmType, MinerName, strPlatform);
                Helpers.ConsolePrint("WinDivertSharp", "New Divert handle: " + DClaymore.ToString() + ". Initiated by " + processId.ToString() + " to divert process list: " + " " + String.Join(",", processIdList));
                return DClaymore;
            }
            return new IntPtr(0);
        }

        public static void DivertStop(IntPtr DivertHandle, int Pid)
        {
            int dh = (int)DivertHandle;
            //if (DivertHandle != IntPtr.Zero | DivertHandle != new IntPtr(-1) | DivertHandle != new IntPtr(0) | DivertHandle  != (IntPtr)0)
            if (processIdList.Count <= 1 && dh != 0 && Divert.processIdList.Contains((uint)Pid))
            {
                //divert_running = false;
                Thread.Sleep(50);
                WinDivert.WinDivertClose(DivertHandle);
                processIdList.Remove((uint)Pid);
                DivertHandle = new IntPtr(0);
                Helpers.ConsolePrint("WinDivertSharp", "Divert STOP for handle: " + dh.ToString() + " ProcessID: " + Pid.ToString());
                Helpers.ConsolePrint("WinDivertSharp", "divert process list: " + " " + String.Join(",", processIdList));
                Thread.Sleep(50);
            }
            else
            {
                if (processIdList.Contains((uint)Pid))
                {
                    Helpers.ConsolePrint("WinDivertSharp", "Try to remove processId " + Pid.ToString() + " from divert process list: " + " " + String.Join(",", processIdList));
                    processIdList.Remove((uint)Pid);
                }
                if (processIdList.Count < 1)
                {
                    Helpers.ConsolePrint("WinDivertSharp", "Warning! Empty processIdList");
                }
            }
        }

       
    }
}
