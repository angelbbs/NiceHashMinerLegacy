using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace MinerLegacyForkFixMonitor
{
    static class Program
    {
        private static List<int> processIdList = new List<int>();
        private static int prevUptimeSec = -1;
        private static int stuckCount = 0;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {
            try
            {
                var current = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        return;
                    }
                }
            }
            catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Logger.ConfigureWithFile();
            var mainproc = Process.GetCurrentProcess();
            Helpers.ConsolePrint("Monitor", "Start monitoring process ID: " + argv[0]);
            Thread.Sleep(5000);

            foreach (var process in Process.GetProcessesByName("device_detection"))
            {
                try { process.Kill(); }
                catch (Exception)
                {
                }
            }

            Process p;
            while (true)
            {
                try
                {
                    p = Process.GetProcessById(int.Parse(argv[0]));
                    //Helpers.ConsolePrint("Monitor", "Process exist");
                }
                catch
                {
                    Helpers.ConsolePrint("Monitor", "Process not exist");
                    foreach (var process in Process.GetProcessesByName("miner"))//gmimer
                    {
                        try { process.Kill(); }
                        catch (Exception)
                        {
                        }
                    }
                    foreach (var pid in processIdList)
                    {
                        try
                        {
                            var pToKill = Process.GetProcessById(pid);
                            Helpers.ConsolePrint("Monitor", "Killing PID: " + pToKill.ToString());
                            pToKill.Kill();
                        }
                        catch
                        {

                        }
                    }
                    //Thread.Sleep(1000);
                    //Process.Start("AfterBenchmark.cmd");

                    Thread.Sleep(1000 * 5);
                    if (File.Exists("NiceHashMinerLegacy.exe"))
                    {
                        var MonitorProc = new Process
                        {
                            StartInfo =
                            {
                                FileName = "NiceHashMinerLegacy.exe"
                            }
                        };

                        if (MonitorProc.Start())
                        {
                            Helpers.ConsolePrint("Monitor", "Starting OK");
                        }
                        else
                        {
                            Helpers.ConsolePrint("Monitor", "Starting ERROR");
                            Process.Start("NiceHashMinerLegacy.exe");
                        }
                    }
                    //mainproc.Kill();
                    break;
                }

                Thread.Sleep(1000 * 1);
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher
                        ("Select * From Win32_Process Where ParentProcessID=" + argv[0]);
                    ManagementObjectCollection moc = searcher.Get();
                    if (moc.Count >= 0)
                    {
                        //Helpers.ConsolePrint("Monitor", moc.Count.ToString());
                        foreach (ManagementObject mo in moc)
                        {
                            //Helpers.ConsolePrint("Monitor", Convert.ToInt32(mo["ProcessID"]).ToString());
                            int pr = Convert.ToInt32(mo["ProcessID"]);
                            if (!processIdList.Contains(pr) && pr != mainproc.Id)
                            {
                                processIdList.Add(pr);
                            }
                        }
                    }
                } catch (Exception ex)
                {
                    Helpers.ConsolePrint("Monitor", ex.ToString());
                    Thread.Sleep(1000);
                    continue;
                }

                try
                {
                    DateTime modification = File.GetLastWriteTime(@"logs\log.txt");

                    if (DateTime.Now.Minute > modification.Minute + 5)
                    {
                        Restart(p);
                    }

                    MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("MinerLegacyForkFixMonitor");
                    byte[] b1 = { (byte)'0', (byte)'0', (byte)'0' };
                    using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 100, MemoryMappedFileAccess.Read))
                    {
                        int b = reader.ReadArray<byte>(0, b1, 0, 3);
                        int res = b1[0];
                        if (prevUptimeSec == res) stuckCount++;
                        if (prevUptimeSec != res) stuckCount = 0;
                        prevUptimeSec = res;
                        //Helpers.ConsolePrint("Monitor", "stuckCount: " + stuckCount.ToString());
                    }

                    if (stuckCount > 1720)
                    {
                        Restart(p);  
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("Monitor", ex.ToString());
                    Thread.Sleep(1000);
                    continue;
                }



                Thread.Sleep(1000 * 5);
            }
            Helpers.ConsolePrint("Monitor", "Stop");
        }

        static void Restart(Process p)
        {
            Helpers.ConsolePrint("Monitor", "Main process stuck. Trying restart");
            try
            {
                var tkHandle = new Process
                {
                    StartInfo =
                            {
                                FileName = "taskkill.exe"
                            }
                };
                tkHandle.StartInfo.Arguments = "/PID " + p.Id.ToString() + " /F /T";
                tkHandle.StartInfo.UseShellExecute = false;
                tkHandle.StartInfo.CreateNoWindow = true;
                tkHandle.Start();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("taskkill", ex.ToString());
            }
        }
    }
}
