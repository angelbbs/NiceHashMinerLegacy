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
        public static volatile bool KawpowLitedivert_running = true;
        public static bool KawpowLiteGoodEpoch = false;
        public static bool KawpowLiteForceStop = false;
        public static bool checkConnectionKawpowLite = false;
        public static bool KawpowLiteMonitorNeedReconnect = false;
        public static List<string> processIdListKawpowLite = new List<string>();
        private static IntPtr DKawpowLiteHandle = (IntPtr)0;


        public static ushort SwapOrder(ushort val)
        {
            return (ushort)(((val & 0xFF00) >> 8) | ((val & 0x00FF) << 8));
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

        public static string worker = "";

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



        public static int CheckWinDivert()
        {
            var DivertHandle = Divert.OpenWinDivert("!loopback && outbound && tcp.DstPort == 9876");
            int ret = (int)DivertHandle;

            if (ret <= 0)
            {
                //Helpers.ConsolePrint("CheckWinDivert", "WinDivert driver error: " + ret.ToString() + ". Lite algos disabled");
                return ret;
            }
            //Helpers.ConsolePrint("CheckWinDivert", "WinDivert OK");
            WinDivert.WinDivertClose(DivertHandle);
            return ret;
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
                Helpers.ConsolePrint("WinDivertSharp", "Invalid handle. Failed to open. Is run as Administrator? Try again");
                Thread.Sleep(200);
                DivertHandle = WinDivert.WinDivertOpen(filter, WinDivertLayer.Network, 0, WinDivertOpenFlags.None);
                if (DivertHandle == IntPtr.Zero || DivertHandle == new IntPtr(-1))
                {
                    Helpers.ConsolePrint("WinDivertSharp", "Invalid handle. Failed to open.");
                    return new IntPtr(-1);
                }
            }
            /*
            MessageBox.Show("OK",
                        "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
              */
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

            

        }
    }
}
