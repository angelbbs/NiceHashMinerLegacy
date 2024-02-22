using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Miners.Grouping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NiceHashMiner.Miners
{
    public static class MinerVersion
    {
        public class MinerData
        {
            public string MinerName;
            public string MinerPath;
            public long MinerSize;
            public string MinerVersion = "(Miner error)";
        }
        public static List<MinerData> MinerDataList = new List<MinerData>();
        public static MinerData Get_ClaymoreNeoscrypt()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.ClaymoreNeoscryptMiner;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "Claymore";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-v 1",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    using (var reader = new StringReader(stdOut))
                    {
                        string line = reader.ReadToEnd();
                        if (line != null)
                        {
                            ret.MinerPath = path;
                            ret.MinerSize = new System.IO.FileInfo(path).Length;
                            ret.MinerVersion = line.Replace(System.Environment.NewLine, string.Empty).Trim();
                            P.Close();
                            return ret;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }

        public static MinerData Get_CryptoDredge()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.CryptoDredge;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "CryptoDredge";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-v",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "CryptoDredge ";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + 13).Replace(System.Environment.NewLine, string.Empty).Trim();
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        public static MinerData Get_GMiner()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.GMiner;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "GMiner";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-v",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "GMiner v";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Trim();
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        public static MinerData Get_Rigel()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.Rigel;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "rigel";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-V",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "rigel";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Trim();
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        public static MinerData Get_lolMiner()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.lolMiner;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "lolMiner";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-v",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                            line = reader.ReadLine();
                        if (line != null)
                        {
                            ret.MinerPath = path;
                            ret.MinerSize = new System.IO.FileInfo(path).Length;
                            ret.MinerVersion = line.Replace(System.Environment.NewLine, string.Empty).Trim();
                            P.Close();
                            return ret;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        public static MinerData Get_miniZ()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.miniZ;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "miniZ";

            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "--nocolor --version",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "miniZ v";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Split('@')[0];
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        public static MinerData Get_nanominer()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.Nanominer;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "Nanominer";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    if (File.Exists("Miners\\Nanominer\\ver.txt")) File.Delete("Miners\\Nanominer\\ver.txt");
                    if (File.Exists("Miners\\Nanominer\\ver.ini")) File.Delete("Miners\\Nanominer\\ver.ini");
                    File.WriteAllText("Miners\\Nanominer\\ver.ini", "logPath=ver.txt\r\n" +
                                                                    "coin = ergo\r\n" +
                                                                    "wallet = ************\r\n");
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                WorkingDirectory = "Miners\\Nanominer",
                                Arguments = "ver.ini",
                                UseShellExecute = false,
                                RedirectStandardOutput = false,
                                RedirectStandardError = false,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    
                    const string findString = "Version ";
                    int _ticks = 0;
                    do
                    {
                        Thread.Sleep(500);
                        _ticks++;
                        if (_ticks > 20) break;
                    } while (!File.Exists("miners\\Nanominer\\ver.txt"));
                    Thread.Sleep(1000);
                    P.Kill();
                    Thread.Sleep(1000);
                    using (var reader = File.OpenText("miners\\Nanominer\\ver.txt"))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Split('-')[0];
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }

        public static MinerData Get_NBMiner()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.NBMiner;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "NBMiner";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-v",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "NBMiner ";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Trim();
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        public static MinerData Get_NBMiner39_5()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = "Miners\\nbminer\\nbminer.39.5.exe";
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "NBMiner.39.5";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-v",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "NBMiner ";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Trim();
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        public static MinerData Get_Phoenix()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.Phoenix;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "Phoenix";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-vs",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            ret.MinerPath = path;
                            ret.MinerSize = new System.IO.FileInfo(path).Length;
                            ret.MinerVersion = line.Replace(System.Environment.NewLine, string.Empty).Trim();
                            P.Close();
                            return ret;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        
        public static MinerData Get_SRBMiner()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.SRBMiner;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "SRBMiner";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    if (File.Exists("Miners\\SRBMiner\\ver.txt")) File.Delete("Miners\\SRBMiner\\ver.txt");
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "--disable-gpu --algorithm yespowertide --pool q:0 --wallet Q --log-file ver.txt",
                                UseShellExecute = false,
                                RedirectStandardOutput = false,
                                RedirectStandardError = false,
                                CreateNoWindow = true,
                                WorkingDirectory = "miners\\SRBMiner"
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    //var stdOut = P.StandardOutput.ReadToEnd();
                    //var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "Miner version: ";
                    int _ticks = 0;
                    do
                    {
                        Thread.Sleep(500);
                        _ticks++;
                        if (_ticks > 20) break;
                    } while (!File.Exists("miners\\SRBMiner\\ver.txt") && !P.HasExited);

                    //P.Close();
                    Thread.Sleep(1000);
                    using (var reader = File.OpenText("miners\\SRBMiner\\ver.txt"))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Trim();
                                //P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }

        public static MinerData Get_TRex()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.trex;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "trex";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "--version",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "T-Rex NVIDIA GPU miner v";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Split(' ')[0];
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }

        public static MinerData Get_TeamRedMiner()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.teamredminer;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "teamredminer";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "-v",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "Team Red Miner version";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var index = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(index + findString.Length).
                                    Replace(System.Environment.NewLine, string.Empty).Trim();
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }
        public static MinerData Get_XMRig()
        {
            List<MinerData> _MinerDataList = new List<MinerData>();
            string path = MinerPaths.Data.Xmrig;
            long filesize = 0l;
            string version = "";
            MinerData ret = new MinerData();
            ret.MinerName = "Xmrig";
            try
            {
                if (!File.Exists(path)) return ret;
                if (File.Exists("Configs\\MinersData.json"))
                {
                    string json = File.ReadAllText("Configs\\MinersData.json");
                    dynamic md = JsonConvert.DeserializeObject<List<MinerData>>(json);
                    if (md != null)
                    {
                        foreach (var m in md)
                        {
                            string _path = m.MinerPath;
                            if (!string.IsNullOrEmpty(_path) && _path.Equals(path))
                            {
                                filesize = m.MinerSize;
                                version = m.MinerVersion;
                                ret.MinerPath = path;
                                ret.MinerSize = filesize;
                                ret.MinerVersion = version.Trim(' ');
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
            }
            
            if (filesize != new System.IO.FileInfo(path).Length)
            {
                try
                {
                    var P = new Process
                    {
                        StartInfo =
                            {
                                FileName = path,
                                Arguments = "--version",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = true
                            }
                    };
                    P.Start();
                    P.WaitForExit(2 * 1000);

                    var stdOut = P.StandardOutput.ReadToEnd();
                    var stdErr = P.StandardError.ReadToEnd();

                    const string findString = "XMRig ";
                    using (var reader = new StringReader(stdOut))
                    {
                        var line = string.Empty;
                        do
                        {
                            line = reader.ReadLine();
                            if (line != null && line.Contains(findString))
                            {
                                var xmrig = line.IndexOf(findString);
                                ret.MinerPath = path;
                                ret.MinerSize = new System.IO.FileInfo(path).Length;
                                ret.MinerVersion = line.Substring(xmrig + 6).Replace(System.Environment.NewLine, string.Empty).Trim();
                                P.Close();
                                return ret;
                            }
                        } while (line != null);
                    }
                }
                catch (Exception ex)
                {
                    //Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                    ret.MinerPath = path;
                    ret.MinerSize = new System.IO.FileInfo(path).Length;
                    ret.MinerVersion = "";
                    return ret;
                }
            }
            return ret;
        }

        public static string GetMinerVersion(string minerName)
        {
            if (!ConfigManager.GeneralConfig.ShowMinersVersions) return "";
            try
            {
                foreach (var miner in MinerVersion.MinerDataList)
                {
                    if (miner.MinerName.ToLower().Equals(minerName.ToLower()))
                    {
                        if (miner.MinerVersion == null) continue;
                        if (miner.MinerVersion.Length > 0)
                        {
                            return " " + miner.MinerVersion.TrimEnd(' ');
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("GetMinerVersion", "ERROR. Miners files not available?");
                Helpers.ConsolePrint("GetMinerVersion", ex.ToString());
                return "";
            }
            return "";
        }
    }
}
