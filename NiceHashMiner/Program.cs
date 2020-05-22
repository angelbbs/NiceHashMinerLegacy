/*
* This is an open source non-commercial project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMiner.PInvoke;
using NiceHashMiner.Utils;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NiceHashMiner.Stats;
using NiceHashMiner.Configs.Data;
using System.Reflection;
using System.Security.Principal;
using System.ComponentModel;
using System.Management;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using NiceHashMiner.Miners;

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
                    MinersManager.StopAllMiners();
                    System.Threading.Thread.Sleep(5000);
                    Process.Start("backup\\restore.cmd");
                }
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

                if (ConfigManager.GeneralConfig.DebugConsole)
                {
                    PInvokeHelpers.AllocConsole();
                }
                
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 11.2)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 11.2;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 12)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 12;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 12.1)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 12.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 13)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 13;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 13.1)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 13.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 14)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 14.0;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 14.1)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 14.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 14.2)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 14.2;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 15)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 15;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 15.1)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 15.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 15.2)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 15.2;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 15.3)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 15.3;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 15.4)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 15.4;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 16)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 16;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 17)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 17;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 18)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 18;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 19)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 19;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 19.1)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 19.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 19.2)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 19.2;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 20)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 20;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 20.1)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 20.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 20.20)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 20.20;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 21)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 21;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 22)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 22;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 22.1)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 22.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 22.2)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 22.2;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 23)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 23;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 23.1)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 23.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 24)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 24;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 24.1)
                {
                    ConfigManager.GeneralConfig.BenchmarkTimeLimits = new BenchmarkTimeLimitsConfig();
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 24.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 25)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 25;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 26)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 26;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 27)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 27;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 27.1)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);
                    ConfigManager.GeneralConfig.ForkFixVersion = 27.1;
                }
                //**
                //Thread.Sleep(100);
                //********************************************************************
                bool certexist = false;
                using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    store.Open(OpenFlags.ReadWrite | OpenFlags.MaxAllowed);

                    foreach (X509Certificate2 cert in store.Certificates)
                    {
                        if (cert.IssuerName.Name.Contains("Angelbbs"))
                        {
                            certexist = true;
                            //Helpers.ConsolePrint("X509Store", cert.SerialNumber);
                            //Helpers.ConsolePrint("X509Store", cert.IssuerName.Name);
                            //Helpers.ConsolePrint("X509Store", cert.Subject);
                            //store.Remove(cert);
                            Form_Main.CertInstalled = true;
                            //Helpers.ConsolePrint("X509Store", "Certificate exist");
                            break;
                        }
                    }
                    store.Close();
                }
                if (!certexist)
                {
                    //00A3302D9D56DBF591
                    X509Certificate2 certificate = new X509Certificate2(Properties.Resources.rootCA, "", X509KeyStorageFlags.PersistKeySet);
                    var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadWrite | OpenFlags.MaxAllowed);
                    store.Add(certificate);
                    //Helpers.ConsolePrint("X509Store", "Add certificate");
                    store.Close();
                }
                
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
                        } catch (Exception e)
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
