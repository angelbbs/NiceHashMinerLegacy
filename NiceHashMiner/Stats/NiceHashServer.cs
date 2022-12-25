using NiceHashMiner.Configs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Stats
{
    public static class StringExtensions
    {
        public static string ToUTF8(this string text)
        {
            return Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(text));
        }
    }
    
    public static class NiceHashServer 
    {
        public static string worker = Configs.ConfigManager.GeneralConfig.WorkerName;
        public static string version = Form_Main.version;
        public static string platform = Form_Main.platform;
        public static string device_header = International.GetText("ListView_Device");
        public static string hashrate_header = International.GetText("Form_Main_device_hashrate");
        public static string temperature_header = International.GetText("Form_Main_device_temp");
        public static string load_header = International.GetText("Form_Main_device_load");
        public static string fan_header = International.GetText("Form_Main_device_fan");
        public static string power_header = International.GetText("Form_Main_device_power");
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
        public static string ToUTF8(this string text)
        {
            return Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(text));
        }

        private static TcpClient client = new TcpClient();
        private static Stream clientStream = null;
        private static TcpListenerEx RemoteListener;
        private static bool listen;
        public static void Listener(bool enable)
        {
            int Port = ConfigManager.GeneralConfig.RigRemoteViewPort;
            try
            {
                if (!enable)
                {
                    listen = false;
                    Helpers.ConsolePrint("NiceHashServer", "Stop listener on port " + Port.ToString());
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
                    Helpers.ConsolePrint("NiceHashServer", "Try start listener on port " + Port.ToString());
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
                        Helpers.ConsolePrint("NiceHashServer", ex.Message);
                        listen = false;
                        return;
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("NiceHashServer", "Already started?");
                Helpers.ConsolePrint("NiceHashServer", ex.ToString());
                return;
            }
            listen = false;
        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        public static void ReadFromClient(TcpListener Listener, TcpClient client, Stream clientStream)
        {
            //http://localhost:7007/version
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
                        if (s1.Contains("GET / HTTP/1.1"))
                        {
                            string html  = File.ReadAllText("html//default.html");
                            //html = ReplaceParams(html);
                            Header = "HTTP/1.1 200 OK\r\n";
                            Header += "Content-Type: text/html\r\n\r\n";
                            responce = Header + html;
                            byte[] bytesresponce = Encoding.ASCII.GetBytes(responce);
                            clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                            clientStream.Flush();
                        }
                        else
                        {
                            if (File.Exists("html//" + request) && request.Contains("css"))
                            {
                                string html = File.ReadAllText("html//" + request);
                                Header = "HTTP/1.1 200 OK\r\n";
                                Header += "Content-Type: text/css\r\n\r\n";
                                responce = Header + html;
                                byte[] bytesresponce = Encoding.ASCII.GetBytes(responce);
                                clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                                clientStream.Flush();
                            } else
                            if (File.Exists("html//" + request) && request.Contains("ico"))
                            {
                                byte[] ico = File.ReadAllBytes("html//" + request);
                                Header = "HTTP/1.1 200 OK\r\n";
                                Header += "Content-Type: image/x-icon\r\n\r\n";
                                responce = Header;
                                byte[] bytesresponce = Encoding.ASCII.GetBytes(responce);
                                clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                                clientStream.Flush();
                                clientStream.Write(ico, 0, ico.Length);
                                clientStream.Flush();
                            }
                            if (request.Contains("desktop.virtualfile"))
                            {
                                byte[] jpg = Form_Main.desktop;
                                if (jpg.Length == 0) break;
                                Header = "HTTP/1.1 200 OK\r\n";
                                Header += "Cache-control: no-cache\r\n";
                                Header += "Content-Type: image/png\r\n\r\n";
                                responce = Header;
                                byte[] bytesresponce = Encoding.ASCII.GetBytes(responce);
                                clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                                clientStream.Flush();
                                clientStream.Write(jpg, 0, jpg.Length);
                                clientStream.Flush();
                            }
                            if (File.Exists("html//" + request.Split('?')[0]) && request.Contains("jpg"))
                            {
                                byte[] jpg = LoagJpegImage("html//" + request.Split('?')[0]);
                                if (jpg.Length == 0) break;
                                Header = "HTTP/1.1 200 OK\r\n";
                                Header += "Cache-control: no-cache\r\n";
                                Header += "Content-Type: image/jpg\r\n\r\n";
                                responce = Header;
                                byte[] bytesresponce = Encoding.ASCII.GetBytes(responce);
                                clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                                clientStream.Flush();
                                clientStream.Write(jpg, 0, jpg.Length);
                                clientStream.Flush();
                            } 
                            if (File.Exists("html//" + request.Split('?')[0]) && request.Contains("png"))
                            {
                                byte[] png = File.ReadAllBytes("html//" + request.Split('?')[0]);
                                Header = "HTTP/1.1 200 OK\r\n";
                                Header += "Cache-control: no-cache\r\n";
                                Header += "Content-Type: image/png\r\n\r\n";
                                responce = Header;
                                byte[] bytesresponce = Encoding.ASCII.GetBytes(responce);
                                clientStream.Write(bytesresponce, 0, bytesresponce.Length);
                                clientStream.Flush();
                                clientStream.Write(png, 0, png.Length);
                                clientStream.Flush();
                            }
                            else
                            {
                                Header = "HTTP/1.1 404 Not found\r\n";
                                Header += "Content-Type: text/html\r\n\r\n";
                                responce = Header + "{\"message\":\"" + request + " not found\"}\r\n";
                            }
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
            } catch (Exception ex)
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

        static byte[] LoagJpegImage(string filename)
        {
            try
            {
                using (System.Drawing.Image img = System.Drawing.Image.FromFile(filename))
                {
                    if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                    {
                        return File.ReadAllBytes(filename);
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                return new byte[0];
            }
            return new byte[0];
        }
        //HTML-шаблон это тупиковый вариант
        private static string ReplaceParams(string html)
        {
            html = html.Replace("$WORKER$", worker);
            html = html.Replace("$VERSION$", version);
            html = html.Replace("$PLATFORM$", platform + "тест");

            html = html.Replace("$DEVICE_HEADER$", device_header);
            html = html.Replace("$HASHRATE_HEADER$", hashrate_header);
            html = html.Replace("$TEMPERATURE_HEADER$", temperature_header);
            html = html.Replace("$LOAD_HEADER$", load_header);
            html = html.Replace("$FAN_HEADER$", fan_header);
            html = html.Replace("$POWER_HEADER$", power_header);
            return html;
        }
    }

}
