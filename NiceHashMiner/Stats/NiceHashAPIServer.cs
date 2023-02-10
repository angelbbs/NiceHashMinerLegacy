using Newtonsoft.Json;
using NiceHashMiner.Algorithms;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Stats
{
    /// <summary>
    /// Wrapper around TcpListener that exposes the Active property
    /// </summary>
    public class TcpListenerEx : TcpListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Net.Sockets.TcpListener"/> class with the specified local endpoint.
        /// </summary>
        /// <param name="localEP">An <see cref="T:System.Net.IPEndPoint"/> that represents the local endpoint to which to bind the listener <see cref="T:System.Net.Sockets.Socket"/>. </param><exception cref="T:System.ArgumentNullException"><paramref name="localEP"/> is null. </exception>
        public TcpListenerEx(IPEndPoint localEP) : base(localEP)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Net.Sockets.TcpListener"/> class that listens for incoming connection attempts on the specified local IP address and port number.
        /// </summary>
        /// <param name="localaddr">An <see cref="T:System.Net.IPAddress"/> that represents the local IP address. </param><param name="port">The port on which to listen for incoming connection attempts. </param><exception cref="T:System.ArgumentNullException"><paramref name="localaddr"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="port"/> is not between <see cref="F:System.Net.IPEndPoint.MinPort"/> and <see cref="F:System.Net.IPEndPoint.MaxPort"/>. </exception>
        public TcpListenerEx(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        public new bool Active
        {
            get { return base.Active; }
        }
    }
    public static class TimeExtensions
    {
        public static TimeSpan StripMilliseconds(this TimeSpan time)
        {
            return new TimeSpan(time.Days, time.Hours, time.Minutes, time.Seconds);
        }
    }
    class NiceHashAPIServer
    {
        public static double balance;
        public static double Rate;
        public static double Power;
        public static double TotalPower;
        public static double PowerRate;
        public static double PowerRateFiat;
        public static double TotalPowerSpent;
        public static double TotalPowerSpentFiat;

        private static TcpClient client = new TcpClient();
        private static Stream clientStream = null;
        private static TcpListenerEx RemoteListener;
        private static bool listen;
        private static long ToUnixTime(DateTime dt)
        {
            TimeSpan timeSpan = (dt - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }
        public static void Listener(bool enable)
        {
            int Port = ConfigManager.GeneralConfig.RigAPiPort;
            s:
            try
            {
                if (!enable)
                {
                    listen = false;
                    Helpers.ConsolePrint("NiceHashAPIServer", "Stop listener on port " + Port.ToString());
                    client.Close();
                    RemoteListener.Server.Close();
                    NiceHashSocket.DropIPPort(Process.GetCurrentProcess().Id, "127.0.0.1", (uint)Port, false);
                    NiceHashSocket.DropIPPort(Process.GetCurrentProcess().Id, "0.0.0.0", (uint)Port, false);
                    return;
                }
                else
                {
                    if (listen) return;
                    listen = true;
                    NiceHashSocket.DropIPPort(Process.GetCurrentProcess().Id, "127.0.0.1", (uint)Port, false);
                    NiceHashSocket.DropIPPort(Process.GetCurrentProcess().Id, "0.0.0.0", (uint)Port, false);
                    Thread.Sleep(1000 * 2);
                    Helpers.ConsolePrint("NiceHashAPIServer", "Try start listener on port " + Port.ToString());
                    RemoteListener = new TcpListenerEx(IPAddress.Any, Port);
                    RemoteListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);
                    RemoteListener.Start();
                }
                while (RemoteListener.Active)
                {
                    try
                    {
                        client = RemoteListener.AcceptTcpClient();
                        clientStream = client.GetStream();
                        string IPClient = Convert.ToString(((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address);

                        new Task(() => ReadFromClient(RemoteListener, client, clientStream)).Start();

                    }
                    catch (Exception ex)
                    {
                        Helpers.ConsolePrint("NiceHashAPIServer", ex.Message);
                        listen = false;
                        return;
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                listen = false;
                Helpers.ConsolePrint("NiceHashAPIServer", "Already started?");
                Helpers.ConsolePrint("NiceHashAPIServer", ex.ToString());
                if (client != null)
                {
                    client.Close();
                }
                if (RemoteListener != null)
                {
                    RemoteListener.Server.Close();
                }
                NiceHashSocket.DropIPPort(Process.GetCurrentProcess().Id, "127.0.0.1", (uint)Port, false);
                NiceHashSocket.DropIPPort(Process.GetCurrentProcess().Id, "0.0.0.0", (uint)Port, false);
                return;
            }
            Thread.Sleep(1000);
            goto s;
        }

        public static void ReadFromClient(TcpListener Listener, TcpClient client, Stream clientStream)
        {
            //http://localhost:7001/version
            byte[] clientMessage = new byte[1];
            byte[] clientMessageRaw = new byte[1024];
            try
            {
                while (true)
                {
                    Thread.Sleep(1);
                    int clientBytes = 0;
                    try
                    {
                        clientBytes = clientStream.Read(clientMessageRaw, 0, 1024);
                    }
                    catch (Exception ex)
                    {
                        if (client != null)
                        {
                            client.Close();
                        }
                        if (clientStream != null)
                        {
                            clientStream.Close();
                            clientStream.Dispose();
                            clientStream = null;
                        }
                        break;
                    }

                    if (clientBytes == 0)
                    {
                        if (client != null)
                        {
                            client.Close();
                        }
                        if (clientStream != null)
                        {
                            clientStream.Close();
                            clientStream.Dispose();
                            clientStream = null;
                        }
                        break;
                    }
                    else if (clientBytes > 1024)
                    {
                        if (client != null)
                        {
                            client.Close();
                        }
                        if (clientStream != null)
                        {
                            clientStream.Close();
                            clientStream.Dispose();
                            clientStream = null;
                        }
                        break;
                    }
                    else
                    {
                        clientMessage = clientMessageRaw;
                        Array.Resize(ref clientMessage, clientBytes);
                        var s1 = Encoding.ASCII.GetString(clientMessage);

                        if (s1.Contains("GET /none HTTP/1.1"))
                        {
                            if (client != null)
                            {
                                client.Close();
                            }
                            if (clientStream != null)
                            {
                                clientStream.Close();
                                clientStream.Dispose();
                                clientStream = null;
                            }
                            break;
                        }

                        string request = s1.Replace("GET /", "").Replace(" HTTP/1.1", "").ToLower().Split('\r')[0];
                        string Header = "";
                        string responce = "";
                        byte[] bytesresponce;
                        if (s1.ToUpper().Contains("GET / HTTP/1.1"))
                        {
                            Header = "HTTP/1.1 200 OK\r\n";
                            Header += "Content-Type: text/html\r\n\r\n";
                            responce = Header + "Miner Legacy Fork Fix " + ConfigManager.GeneralConfig.ForkFixVersion.ToString() +
                                " (for NiceHash) API server";
                            bytesresponce = Encoding.ASCII.GetBytes(responce);
                            clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                            clientStream.Flush();
                        }
                        else if (s1.ToUpper().Contains("GET /HELP HTTP/1.1"))
                        {
                            Header = "HTTP/1.1 200 OK\r\n";
                            Header += "Content-Type: text/json\r\n\r\n";
                            string text;
                            string help = "";
                            try
                            {
                                help = File.ReadAllText("Help\\API.txt");
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("NiceHashAPIServer", ex.ToString());
                                help = "File Help\\API.txt not found\r\n";
                            }


                            responce = Header + help;
                            bytesresponce = Encoding.ASCII.GetBytes(responce);
                            clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                            clientStream.Flush();
                        }
                        else if (s1.ToUpper().Contains("GET /VERSION HTTP/1.1"))
                        {
                            Header = "HTTP/1.1 200 OK\r\n";
                            Header += "Content-Type: application/json\r\n\r\n";
                            responce = Header + "{\"version\":\"" +
                                "Miner Legacy Fork Fix " + ConfigManager.GeneralConfig.ForkFixVersion.ToString() +
                                " (for NiceHash)" +
                                "\"}\r\n";
                            bytesresponce = Encoding.ASCII.GetBytes(responce);
                            clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                            clientStream.Flush();
                        }
                        else if (s1.ToUpper().Contains("GET /SUMMARY HTTP/1.1"))
                        {
                            DateTime uptime = new DateTime() + Form_Main.Uptime;
                            Header = "HTTP/1.1 200 OK\r\n";
                            Header += "Content-Type: text/json\r\n\r\n";
                            List<Device> MiningDevicesList = new List<Device>();
                            double RateFiat = ExchangeRateApi.ConvertToActiveCurrency((Rate - PowerRate) *
                                ExchangeRateApi.GetUsdExchangeRate());
                            double powerspentfiat = ExchangeRateApi.ConvertToActiveCurrency(PowerRate *
                                ExchangeRateApi.GetUsdExchangeRate());


                            foreach (var dev in ComputeDeviceManager.Available.Devices)
                            {
                                Device MiningDevice = new Device();
                                MiningDevice.Name = dev.Name;
                                MiningDevice.DeviceType = dev.DeviceType.ToString();
                                MiningDevice.Manufacturer = ComputeDevice.GetManufacturer(dev.Manufacturer);
                                MiningDevice.GpuRam = (long)dev.GpuRam;


                                MiningDevice.PlatformNum = (int)dev.DeviceType;
                                MiningDevice.Codename = dev.Codename;
                                MiningDevice.UUID = dev.Uuid;
                                MiningDevice.DevUUID = dev.DevUuid;
                                MiningDevice.ID = dev.ID;
                                MiningDevice.Index = dev.Index;
                                MiningDevice.BusID = dev.BusID;
                                MiningDevice.NvidiaLHR = dev.NvidiaLHR;
                                MiningDevice.Enabled = dev.Enabled;
                                MiningDevice.MonitorConnected = dev.MonitorConnected;
                                MiningDevice.AlgorithmID = dev.AlgorithmID;
                                MiningDevice.Algorithm = ((AlgorithmType)dev.AlgorithmID).ToString();
                                string unit;
                                switch ((AlgorithmType)dev.AlgorithmID)
                                {
                                    case AlgorithmType.ZHash:
                                    case AlgorithmType.ZelHash:
                                    case AlgorithmType.BeamV3:
                                        unit = "Sol/s";
                                        break;
                                    case AlgorithmType.CuckooCycle:
                                    case AlgorithmType.GrinCuckatoo32:
                                        unit = "G/s";
                                        break;
                                    default:
                                        unit = "H/s";
                                        break;
                                }
                                MiningDevice.SecondaryAlgorithmID = dev.SecondAlgorithmID;
                                MiningDevice.SecondaryAlgorithm = ((AlgorithmType)dev.SecondAlgorithmID).ToString();
                                string unit2;
                                switch ((AlgorithmType)dev.SecondAlgorithmID)
                                {
                                    case AlgorithmType.ZHash:
                                    case AlgorithmType.ZelHash:
                                    case AlgorithmType.BeamV3:
                                        unit2 = "Sol/s";
                                        break;
                                    case AlgorithmType.CuckooCycle:
                                    case AlgorithmType.GrinCuckatoo32:
                                        unit2 = "G/s";
                                        break;
                                    default:
                                        unit2 = "H/s";
                                        break;
                                }
                                if (dev.SecondAlgorithmID > 0)
                                {
                                    MiningDevice.IsDualAlgorithm = true;
                                }

                                MiningDevice.MinerName = dev.MinerName;
                                MiningDevice.MinerVersion = "Unknown";

                                MiningDevice.MiningHashrate = dev.MiningHashrate;
                                MiningDevice.DescHashrate = unit;
                                MiningDevice.MiningHashrateSecond = dev.MiningHashrateSecond;
                                MiningDevice.DescHashrateSecond = unit2;

                                MiningDevice.Temp = (int)dev.Temp;
                                MiningDevice.TempMemory = (int)dev.TempMemory;
                                MiningDevice.Load = (int)dev.Load;
                                MiningDevice.MemLoad = (int)dev.MemLoad;
                                MiningDevice.Fan = (int)dev.FanSpeed;
                                MiningDevice.FanRPM = (int)dev.FanSpeedRPM;
                                MiningDevicesList.Add(MiningDevice);
                            }

                            var _root = new Root
                            {
                                platform = "NiceHash",
                                Version = ConfigManager.GeneralConfig.ForkFixVersion.ToString(),
                                RigStatus = NiceHashStats.CalcRigStatusString(),
                                RigDateTime = DateTime.Now,
                                RigDateTimeUnix = ToUnixTime(DateTime.Now),
                                Uptime = Form_Main.Uptime.StripMilliseconds(),
                                UptimeSeconds = (long)(Form_Main.Uptime.StripMilliseconds().TotalSeconds),
                                Wallet = ConfigManager.GeneralConfig.BitcoinAddressNew,
                                Worker = ConfigManager.GeneralConfig.WorkerName,
                                MiningLocation = ConfigManager.GeneralConfig.ServiceLocation,
                                MiningDevices = MiningDevicesList,
                                Currency = ExchangeRateApi.ActiveDisplayCurrency,
                                Rate_mBTC = Rate,
                                Rate_Fiat = ExchangeRateApi.ConvertToActiveCurrency(Rate *
                                ExchangeRateApi.GetUsdExchangeRate()),
                                Balance_mBTC = NiceHashStats.Balance * 1000,
                                Balance_Fiat = ExchangeRateApi.ConvertToActiveCurrency(balance *
                                ExchangeRateApi.GetUsdExchangeRate() / 1000),
                                PowerSpent_mBTC = PowerRate * 1000,
                                PowerSpent_Fiat = PowerRateFiat,
                                Power = (int)Power,
                                TotalPower = TotalPower,
                                TotalPowerSpent_Fiat = TotalPowerSpentFiat,
                                TotalPowerSpent_mBTC = (TotalPowerSpentFiat) / ExchangeRateApi.ConvertToActiveCurrency(ExchangeRateApi.GetUsdExchangeRate()),
                                BTCcurrencyRate = ExchangeRateApi.ConvertToActiveCurrency(ExchangeRateApi.GetUsdExchangeRate())
                            };


                            var s = JsonConvert.SerializeObject(_root, Formatting.Indented);
                            responce = Header + s +
                                "\r\n";
                            bytesresponce = Encoding.ASCII.GetBytes(responce);
                            clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                            clientStream.Flush();
                        }
                        else
                        {
                            Header = "HTTP/1.1 404 Not found\r\n";
                            Header += "Content-Type: text/html\r\n\r\n";
                            responce = Header + "404\r\n";
                            bytesresponce = Encoding.ASCII.GetBytes(responce);
                            clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                            clientStream.Flush();
                        }

                        if (client != null)
                        {
                            client.Close();
                        }
                        if (clientStream != null)
                        {
                            clientStream.Close();
                            clientStream.Dispose();
                            clientStream = null;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (client != null)
                {
                    client.Close();
                }
                if (clientStream != null)
                {
                    clientStream.Close();
                    clientStream.Dispose();
                    clientStream = null;
                }
            }
        }


    }
}
