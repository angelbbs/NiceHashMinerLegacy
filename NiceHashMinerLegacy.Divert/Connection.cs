using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace NiceHashMinerLegacy.Divert
{
    public enum ConnectType
    {
        Tcp,
        Udp,
        TcPv6,
        UdPv6
    }

    public class Connection
    {
        private static Process[] _allProc;

        public static SortedList<int, string> HostNames = new SortedList<int, string>();

        private ConnectType _connectType;
        private IPEndPoint _localEndPoint;
        private uint _owningProcessId;
        private IPEndPoint _remoteEndPoint;
        private TcpState _state;

        #region Constructors

        public Connection(ref NetworkInformation.Tcp6RowOwnerPid info)
        {
            int port = NetworkOrderToHost(info.LocalPort);
            long scope = NetworkOrderToHost(info.LocalScopeId);
            var ip = new IPAddress(info.LocalAddr, scope);
            _localEndPoint = new IPEndPoint(ip, port);

            port = NetworkOrderToHost(info.RemotePort);
            scope = NetworkOrderToHost(info.RemoteScopeId);
            ip = new IPAddress(info.RemoteAddr, scope);
            _remoteEndPoint = new IPEndPoint(ip, port);

            _owningProcessId = info.OwningPid;
            _connectType = ConnectType.TcPv6;
            _state = info.State;
        }

        public Connection(ref NetworkInformation.TcpRowOwnerPid info)
        {
            int port = NetworkOrderToHost(info.LocalPort);
            var ip = new IPAddress(info.LocalAddr);
            _localEndPoint = new IPEndPoint(ip, port);

            port = NetworkOrderToHost(info.RemotePort);
            ip = new IPAddress(info.RemoteAddr);
            _remoteEndPoint = new IPEndPoint(ip, port);

            _owningProcessId = info.OwningPid;
            _connectType = ConnectType.Tcp;
            _state = info.State;
        }

        public Connection(ref NetworkInformation.Udp6RowOwnerPid info)
        {
            int port = NetworkOrderToHost(info.LocalPort);
            long scope = NetworkOrderToHost(info.LocalScopeId);
            var ip = new IPAddress(info.LocalAddr, scope);
            _localEndPoint = new IPEndPoint(ip, port);

            port = 0;
            ip = IPAddress.IPv6Any;
            _remoteEndPoint = new IPEndPoint(ip, port);

            _owningProcessId = info.OwningPid;
            _connectType = ConnectType.UdPv6;
            _state = 0;
        }

        public Connection(ref NetworkInformation.UdpRowOwnerPid info)
        {
            int port = NetworkOrderToHost(info.LocalPort);
            var ip = new IPAddress(info.LocalAddr);
            _localEndPoint = new IPEndPoint(ip, port);

            port = 0;
            ip = IPAddress.Any;
            _remoteEndPoint = new IPEndPoint(ip, port);

            _owningProcessId = info.OwningPid;
            _connectType = ConnectType.Udp;
            _state = 0;
        }

        #endregion

        public static Process[] Processes
        {
            get { return _allProc; }
        }

        [Browsable(false)]
        public string DnsName
        {
            get
            {
                //return _hostName;
                int pointHash = _remoteEndPoint.GetHashCode();

                if (HostNames.ContainsKey(pointHash))
                    return HostNames[pointHash];

                return _remoteEndPoint.ToString();
            }
            //             set { _hostName = value; }
        }

        public string OwningProcess
        {
            get
            {
                if (OwningPid == 0)
                {
                    return string.Format("System Process[{0}]", OwningPid);
                }

                string name = string.Format("UnknownName[{0}]", OwningPid);
                for (int i = 0; i < _allProc.Length; i++)
                {
                    if (_allProc[i].Id != OwningPid) continue;
                    return string.Format("{0} [{1}]", _allProc[i].ProcessName, OwningPid);
                }

                return name;
            }
        }

        [Browsable(false)]
        public uint OwningPid
        {
            get { return _owningProcessId; }
        }

        public IPEndPoint LocalEndPoint
        {
            get { return _localEndPoint; }
        }

        public IPEndPoint RemoteEndPoint
        {
            get { return _remoteEndPoint; }
        }

        public TcpState State
        {
            get { return _state; }
        }

        public ConnectType ConnectType
        {
            get { return _connectType; }
        }


        public static void UpdateProcessList()
        {
            if (_allProc != null)
            {
                for (int i = 0; i < _allProc.Length; i++)
                    _allProc[i].Dispose();
                Array.Clear(_allProc, 0, _allProc.Length);
            }

            _allProc = Process.GetProcesses();
            Array.Sort(_allProc, (p1, p2) => p1.Id - p2.Id);
        }

        private static UInt16 NetworkOrderToHost(UInt32 dwPort)
        {
            var b = new Byte[2];
            // high weight byte
            b[0] = (byte)(dwPort >> 8);
            // low weight byte
            b[1] = (byte)(dwPort & 255);

            return BitConverter.ToUInt16(b, 0);
        }

        public bool Equals(Connection other)
        {
            return _remoteEndPoint.GetHashCode() == other._remoteEndPoint.GetHashCode();
        }

        public bool Equals(IPEndPoint other)
        {
            return _remoteEndPoint.GetHashCode() == other.GetHashCode();
        }
    }
}
