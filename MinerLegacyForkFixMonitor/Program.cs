using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinerLegacyForkFixMonitor
{
    static class Program
    {
        private static List<int> processIdList = new List<int>();
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Logger.ConfigureWithFile();
            var mainproc = Process.GetCurrentProcess();
            Helpers.ConsolePrint("Monitor", "Start monitoring process ID: " + argv[0]);
            //Application.Run(new MainForm());
            while (true)
            {
                try
                {
                    var p = Process.GetProcessById(int.Parse(argv[0]));
                    //Helpers.ConsolePrint("Monitor", "Process exist");
                }
                catch
                {
                    Helpers.ConsolePrint("Monitor", "Process not exist");
                    foreach (var pid in processIdList)
                    {
                        try
                        {
                            var pToKill = Process.GetProcessById(pid);
                            Helpers.ConsolePrint("Monitor", "Killing PID: " + pToKill.ToString());
                            pToKill.Kill();
                        } catch
                        {

                        }
                    }
                    foreach (var process in Process.GetProcessesByName("miner"))
                    {
                        try { process.Kill(); }
                        catch (Exception e) {
                        }
                    }

                    Thread.Sleep(1000);
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
                    try
                    {
                        //Process proc = Process.GetProcessById(pid);
                        //proc.Kill();
                    }
                    catch (ArgumentException)
                    {
                        // Process already exited.
                    }
                }
                Thread.Sleep(1000 * 5);
            }
            Helpers.ConsolePrint("Monitor", "Stop");
        }
    }
}
