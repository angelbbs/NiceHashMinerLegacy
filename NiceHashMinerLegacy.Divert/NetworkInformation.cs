using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace NiceHashMinerLegacy.Divert
{
    public enum IpVersion : uint
    {
        IPv4 = 2,
        IPv6 = 23
    }

    public static class NetworkInformation
    {
        private const int ErrorInsufficientBuffer = 122;
        private const int Successfully = 0;

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort,
            IpVersion ipVersion, TcpTableClass tblClass, int reserved);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedUdpTable(IntPtr pTcpTable, ref int dwOutBufLen, bool sort,
            IpVersion ipVersion, UdpTableClass tblClass, int reserved);


        public static List<Connection> GetProcessTcpActivity(int pid)
        {
            Tcp6RowOwnerPid[] t6 = GetTcpConnections<Tcp6RowOwnerPid>(IpVersion.IPv6);
            TcpRowOwnerPid[] t4 = GetTcpConnections<TcpRowOwnerPid>(IpVersion.IPv4);

            List<Connection> list = new List<Connection>();

            for (int i = 0; i < t6.Length; i++)
            {
                if (pid < 0 || t6[i].OwningPid == pid)
                {
                    list.Add(new Connection(ref t6[i]));
                }
            }

            for (int i = 0; i < t4.Length; i++)
            {
                if (pid < 0 || t4[i].OwningPid == pid)
                {
                    list.Add(new Connection(ref t4[i]));
                }
            }

            return list;
        }

        public static Connection[] GetTcpV6Connections()
        {
            Tcp6RowOwnerPid[] t = GetTcpConnections<Tcp6RowOwnerPid>(IpVersion.IPv6);
            Connection[] connectInfo = new Connection[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                connectInfo[i] = new Connection(ref t[i]);
            }
            return connectInfo;
        }

        public static Connection[] GetTcpV4Connections()
        {
            TcpRowOwnerPid[] t = GetTcpConnections<TcpRowOwnerPid>(IpVersion.IPv4);
            Connection[] connectInfo = new Connection[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                connectInfo[i] = new Connection(ref t[i]);
            }
            return connectInfo;
        }

        public static Connection[] GetUdpV4Connections()
        {
            UdpRowOwnerPid[] t = GetUdpConnections<UdpRowOwnerPid>(IpVersion.IPv4);
            Connection[] connectInfo = new Connection[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                connectInfo[i] = new Connection(ref t[i]);
            }
            return connectInfo;
        }

        public static Connection[] GetUdpV6Connections()
        {
            Udp6RowOwnerPid[] t = GetUdpConnections<Udp6RowOwnerPid>(IpVersion.IPv6);

            Connection[] connectInfo = new Connection[t.Length];
            for (int i = 0; i < t.Length; i++)
            {
                connectInfo[i] = new Connection(ref t[i]);
            }
            return connectInfo;
        }

        private static void ReadData<T>(IntPtr buffer, out T[] tTable)
        {
            Type rowType = typeof(T);
            int sizeRow = Marshal.SizeOf(rowType);
            long buffAddress = buffer.ToInt64();

            int count = Marshal.ReadInt32(buffer);
            int offcet = Marshal.SizeOf(typeof(Int32));

            tTable = new T[count];
            for (int i = 0; i < tTable.Length; i++)
            {
                //calc position for next array element
                var memoryPos = new IntPtr(buffAddress + offcet);
                //read element
                tTable[i] = (T)Marshal.PtrToStructure(memoryPos, rowType);

                offcet += sizeRow;
            }
        }

        private static T[] GetUdpConnections<T>(IpVersion ipVersion)
        {
            T[] tTable;

            int buffSize = 0;

            // how much memory do we need?
            GetExtendedUdpTable(IntPtr.Zero, ref buffSize, false, ipVersion, UdpTableClass.UdpTableOwnerPid, 0);

            IntPtr buffer = Marshal.AllocHGlobal(buffSize);
            try
            {
                uint retVal = GetExtendedUdpTable(buffer, ref buffSize, false, ipVersion,
                                                  UdpTableClass.UdpTableOwnerPid, 0);

                while (retVal == ErrorInsufficientBuffer) //buffer should be greater?
                {
                    buffer = Marshal.ReAllocHGlobal(buffer, new IntPtr(buffSize));
                    retVal = GetExtendedUdpTable(buffer, ref buffSize, false, ipVersion, UdpTableClass.UdpTableOwnerPid, 0);
                }

                if (retVal != Successfully)
                    return null;

                ReadData(buffer, out tTable);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(buffer);
            }
            return tTable;
        }

        private static T[] GetTcpConnections<T>(IpVersion ipVersion)
        {
            T[] tTable;

            int buffSize = 0;

            // how much memory do we need?
            GetExtendedTcpTable(IntPtr.Zero, ref buffSize, false, ipVersion, TcpTableClass.TcpTableOwnerPidAll, 0);

            IntPtr buffer = Marshal.AllocHGlobal(buffSize);
            try
            {
                uint retVal = GetExtendedTcpTable(buffer, ref buffSize, false, ipVersion,
                                                  TcpTableClass.TcpTableOwnerPidAll, 0);

                while (retVal == ErrorInsufficientBuffer) //buffer should be greater?
                {
                    buffer = Marshal.ReAllocHGlobal(buffer, new IntPtr(buffSize));
                    retVal = GetExtendedTcpTable(buffer, ref buffSize, false, ipVersion,
                                                 TcpTableClass.TcpTableOwnerPidAll, 0);
                }

                if (retVal != Successfully) return null;

                ReadData(buffer, out tTable);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(buffer);
            }
            return tTable;
        }

        #region Nested type: Tcp6RowOwnerPid

        [StructLayout(LayoutKind.Sequential)]
        public struct Tcp6RowOwnerPid
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] LocalAddr;
            public uint LocalScopeId;
            public uint LocalPort;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] RemoteAddr;
            public uint RemoteScopeId;
            public uint RemotePort;
            public TcpState State;
            public uint OwningPid;
        }

        #endregion

        #region Nested type: TcpRowOwnerPid

        [StructLayout(LayoutKind.Sequential)]
        public struct TcpRowOwnerPid
        {
            public TcpState State;
            public uint LocalAddr;
            public uint LocalPort;
            public uint RemoteAddr;
            public uint RemotePort;
            public uint OwningPid;
        }

        #endregion

        #region Nested type: TcpTableClass

        private enum TcpTableClass
        {
            TcpTableBasicListener,
            TcpTableBasicConnections,
            TcpTableBasicAll,
            TcpTableOwnerPidListener,
            TcpTableOwnerPidConnections,
            TcpTableOwnerPidAll,
            TcpTableOwnerModuleListener,
            TcpTableOwnerModuleConnections,
            TcpTableOwnerModuleAll
        }

        #endregion

        #region Nested type: Udp6RowOwnerPid

        [StructLayout(LayoutKind.Sequential)]
        public struct Udp6RowOwnerPid
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] LocalAddr;
            public uint LocalScopeId;
            public uint LocalPort;
            public uint OwningPid;
        }

        #endregion

        #region Nested type: UdpRowOwnerPid

        [StructLayout(LayoutKind.Sequential)]
        public struct UdpRowOwnerPid
        {
            public uint LocalAddr;
            public uint LocalPort;
            public uint OwningPid;
        }

        #endregion

        #region Nested type: UdpTableClass

        internal enum UdpTableClass
        {
            UdpTableBasic,
            UdpTableOwnerPid,
            UdpTableOwnerModule
        }

        #endregion
    }


}


