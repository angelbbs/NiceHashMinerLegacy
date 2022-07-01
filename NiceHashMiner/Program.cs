using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMiner.Miners;
using NiceHashMiner.Stats;
using NiceHashMiner.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace NiceHashMiner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        static void Main(string[] argv)
        {
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
            var proc = Process.GetCurrentProcess();
            if (hasAdministrativeRight == false)
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = Application.ExecutablePath;
                try
                {
                    Process.Start(processInfo);
                }
                catch (Win32Exception e)
                {
                    Helpers.ConsolePrint("Error start as Administrator: ", e.ToString());
                }
                proc.Kill();
            }

            // Set working directory to exe
            var pathSet = false;
            var path = Path.GetDirectoryName(Application.ExecutablePath);
            if (path != null)
            {
                Environment.CurrentDirectory = path;
                pathSet = true;
            }

            // Add common folder to path for launched processes
            var pathVar = Environment.GetEnvironmentVariable("PATH");
            pathVar += ";" + Path.Combine(Environment.CurrentDirectory, "common");
            Environment.SetEnvironmentVariable("PATH", pathVar);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            //Console.OutputEncoding = System.Text.Encoding.Unicode;
            // #0 set this first so data parsing will work correctly
            Globals.JsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Culture = CultureInfo.InvariantCulture
            };

            bool BackupRestoreFile = false;
            if (Directory.Exists("backup"))
            {
                var dirInfo = new DirectoryInfo("backup");
                foreach (var file in dirInfo.GetFiles())
                {
                    if (file.Name.Contains("backup_") && file.Name.Contains(".zip"))
                    {
                        BackupRestoreFile = true;
                    }
                }
            }

            // #1 first initialize config
            if (!ConfigManager.InitializeConfig() && BackupRestoreFile)
            {
                var dialogRes = Utils.MessageBoxEx.Show("Restore from backup?", "Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Question, 15000);
                if (dialogRes == System.Windows.Forms.DialogResult.Yes)
                {
                    if (ConfigManager.GeneralConfig.Use_OpenHardwareMonitor)
                    {
                        var CMDconfigHandleOHM = new Process

                        {
                            StartInfo =
                            {
                                FileName = "sc.exe"
                            }
                        };

                        CMDconfigHandleOHM.StartInfo.Arguments = "stop winring0_1_2_0";
                        CMDconfigHandleOHM.StartInfo.UseShellExecute = false;
                        CMDconfigHandleOHM.StartInfo.CreateNoWindow = true;
                        CMDconfigHandleOHM.Start();
                    }
                    MinersManager.StopAllMiners();
                    System.Threading.Thread.Sleep(5000);
                    Process.Start("backup\\restore.cmd");
                }
            }

            var mainproc = Process.GetCurrentProcess();
            if (ConfigManager.GeneralConfig.ProgramMonitoring)
            {
                if (File.Exists("MinerLegacyForkFixMonitor.exe"))
                {
                    var MonitorProc = new Process
                    {
                        StartInfo =
                {
                    FileName = "MinerLegacyForkFixMonitor.exe"
                }
                    };

                    MonitorProc.StartInfo.Arguments = mainproc.Id.ToString();
                    MonitorProc.StartInfo.UseShellExecute = false;
                    MonitorProc.StartInfo.CreateNoWindow = true;
                    if (MonitorProc.Start())
                    {
                        Helpers.ConsolePrint("Monitor", "Starting OK");
                    }
                    else
                    {
                        Helpers.ConsolePrint("Monitor", "Starting ERROR");
                    }
                }
            }


            //checking for incompatibilities
            if (ConfigManager.GeneralConfig.AllowMultipleInstances && ConfigManager.GeneralConfig.ProgramMonitoring)
            {
                ConfigManager.GeneralConfig.AllowMultipleInstances = false;
                ConfigManager.GeneralConfigFileCommit();
            }
            // #2 check if multiple instances are allowed
            var startProgram = true;
            if (ConfigManager.GeneralConfig.AllowMultipleInstances == false)
            {
                try
                {
                    var current = Process.GetCurrentProcess();
                    foreach (var process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            startProgram = false;
                        }
                    }
                }
                catch { }
            }

            if (startProgram)
            {
                if (ConfigManager.GeneralConfig.LogToFile)
                {
                    Logger.ConfigureWithFile();
                }
                ConfigManager.GeneralConfig.ServiceLocation = 0;//это добавить в релиз
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 47.1)
                {
                    ConfigManager.GeneralConfig.ChartEnable = false;
                    Helpers.ConsolePrint("NICEHASH", "Previous version: " + Configs.ConfigManager.GeneralConfig.ForkFixVersion.ToString());
                    ConfigManager.GeneralConfig.ForkFixVersion = 47.1;
                }
                //**
                //Thread.Sleep(100);
                //********************************************************************
                if (!Directory.Exists("configs\\overclock")) Directory.CreateDirectory("configs\\overclock");
                new StorePermission(PermissionState.Unrestricted) { Flags = StorePermissionFlags.AddToStore }.Assert();
                X509Certificate2 certificate = new X509Certificate2(Properties.Resources.rootCA, "", X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

                using (var storeCU = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    storeCU.Open(OpenFlags.ReadWrite | OpenFlags.MaxAllowed);

                    foreach (X509Certificate2 cert in storeCU.Certificates)
                    {
                        if (!cert.IssuerName.Name.Contains("Angelbbs"))
                        {
                            //Helpers.ConsolePrint("X509Store", cert.SerialNumber);
                            //Helpers.ConsolePrint("X509Store", cert.IssuerName.Name);
                            //Helpers.ConsolePrint("X509Store", cert.Subject);
                            //storeCU.Remove(cert);
                            storeCU.Add(certificate);
                            storeCU.Close();
                            //Helpers.ConsolePrint("X509Store", "Certificate exist");
                            break;
                        }
                    }
                    storeCU.Close();
                }

                using (var storeLM = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    storeLM.Open(OpenFlags.ReadWrite | OpenFlags.MaxAllowed);

                    foreach (X509Certificate2 cert in storeLM.Certificates)
                    {
                        if (!cert.IssuerName.Name.Contains("Angelbbs"))
                        {
                            //Helpers.ConsolePrint("X509Store", cert.SerialNumber);
                            //Helpers.ConsolePrint("X509Store", cert.IssuerName.Name);
                            //Helpers.ConsolePrint("X509Store", cert.Subject);
                            //storeLM.Remove(cert);
                            storeLM.Add(certificate);
                            storeLM.Close();
                            //Helpers.ConsolePrint("X509Store", "Certificate exist");
                            break;
                        }
                    }
                    storeLM.Close();
                }

                //check after install
                using (var store2 = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    store2.Open(OpenFlags.ReadWrite | OpenFlags.MaxAllowed);

                    foreach (X509Certificate2 cert in store2.Certificates)
                    {
                        if (cert.IssuerName.Name.Contains("Angelbbs"))
                        {
                            Form_Main.CertInstalled = true;
                            break;
                        }
                    }
                    store2.Close();
                }

                var CMDconfigHandleWD = new Process
                {
                    StartInfo =
                {
                    FileName = "sc.exe"
                }
                };

                CMDconfigHandleWD.StartInfo.Arguments = "stop WinDivert1.4";
                CMDconfigHandleWD.StartInfo.UseShellExecute = false;
                CMDconfigHandleWD.StartInfo.CreateNoWindow = true;
                CMDconfigHandleWD.Start();

                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
                Helpers.ConsolePrint("NICEHASH", "Starting up NiceHashMiner Legacy Fork Fix: Build date " + buildDate);
                // init active display currency after config load
                ExchangeRateApi.ActiveDisplayCurrency = ConfigManager.GeneralConfig.DisplayCurrency;

                // #2 then parse args
                var commandLineArgs = new CommandLineParser(argv);

                // Helpers.ConsolePrint("NICEHASH", "Starting up NiceHashMiner v" + Application.ProductVersion);

                if (!pathSet)
                {
                    Helpers.ConsolePrint("NICEHASH", "Path not set to executable");
                }

                var tosChecked = ConfigManager.GeneralConfig.agreedWithTOS == Globals.CurrentTosVer;
                if (!tosChecked || !ConfigManager.GeneralConfigIsFileExist() && !commandLineArgs.IsLang)
                {
                    Helpers.ConsolePrint("NICEHASH",
                        "No config file found. Running NiceHash Miner Legacy for the first time. Choosing a default language.");
                    Application.Run(new Form_ChooseLanguage(true));
                }

                // Init languages
                International.Initialize(ConfigManager.GeneralConfig.Language);

                if (commandLineArgs.IsLang)
                {
                    Helpers.ConsolePrint("NICEHASH", "Language is overwritten by command line parameter (-lang).");
                    International.Initialize(commandLineArgs.LangValue);
                    ConfigManager.GeneralConfig.Language = commandLineArgs.LangValue;
                }

                // check WMI
                if (Helpers.IsWmiEnabled())
                {
                    // if (ConfigManager.GeneralConfig.agreedWithTOS == Globals.CurrentTosVer)
                    {
                        try
                        {
                            //Application.Run(new Form_Main());
                            var formmain = new Form_Main();
                            formmain.Hide();
                            Application.Run(formmain);
                        }
                        catch (Exception e)
                        {
                            Helpers.ConsolePrint("NICEHASH", e.Message);
                        }
                    }
                }

                else
                {
                    MessageBox.Show(International.GetText("Program_WMI_Error_Text"),
                        International.GetText("Program_WMI_Error_Title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
    }
}
