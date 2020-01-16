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

namespace NiceHashMiner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
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

            // #1 first initialize config
            ConfigManager.InitializeConfig();

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
                //**
                /*
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 4)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (File.Exists("internals\\MinerOptionPackage_glg.json"))
                        File.Delete("internals\\MinerOptionPackage_glg.json");

                    if (File.Exists("internals\\MinerOptionPackage_ClaymoreDual.json"))
                        File.Delete("internals\\MinerOptionPackage_ClaymoreDual.json");

                    if (File.Exists("bin\\ccminer_klaust\\ccminer.exe"))
                        File.Delete("bin\\ccminer_klaust\\ccminer.exe");
                    ConfigManager.GeneralConfig.ForkFixVersion = 4;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 4.1)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (File.Exists("internals\\MinerOptionPackage_ClaymoreDual.json"))
                        File.Delete("internals\\MinerOptionPackage_ClaymoreDual.json");

                    ConfigManager.GeneralConfig.ForkFixVersion = 4.1;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 5)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (File.Exists("bin\\xmrig\\xmrig.exe"))
                        File.Delete("bin\\xmrig\\xmrig.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 5;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 6)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (File.Exists("bin\\xmrig\\xmrig.exe"))
                        File.Delete("bin\\xmrig\\xmrig.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 6;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 7)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    ConfigManager.GeneralConfig.ForkFixVersion = 7;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 8)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    ConfigManager.GeneralConfig.ForkFixVersion = 8;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 8.2)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    ConfigManager.GeneralConfig.ForkFixVersion = 8.2;
                }
                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 9)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    if (File.Exists("bin\\xmrig\\xmrig.exe"))
                        File.Delete("bin\\xmrig\\xmrig.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 9;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 9.1)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    if (File.Exists("bin\\xmrig\\xmrig.exe"))
                        File.Delete("bin\\xmrig\\xmrig.exe");

                    if (File.Exists("bin_3rdparty\\t-rex\\t-rex.exe"))
                        File.Delete("bin_3rdparty\\t-rex\\t-rex.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 9.1;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 9.2)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    if (File.Exists("bin\\xmrig\\xmrig.exe"))
                        File.Delete("bin\\xmrig\\xmrig.exe");

                    if (File.Exists("bin_3rdparty\\t-rex\\t-rex.exe"))
                        File.Delete("bin_3rdparty\\t-rex\\t-rex.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 9.2;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 9.3)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    if (File.Exists("bin\\xmrig\\xmrig.exe"))
                        File.Delete("bin\\xmrig\\xmrig.exe");

                    if (File.Exists("bin_3rdparty\\t-rex\\t-rex.exe"))
                        File.Delete("bin_3rdparty\\t-rex\\t-rex.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 9.3;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 10)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    if (File.Exists("bin\\xmrig\\xmrig.exe"))
                        File.Delete("bin\\xmrig\\xmrig.exe");

                    if (File.Exists("bin_3rdparty\\t-rex\\t-rex.exe"))
                        File.Delete("bin_3rdparty\\t-rex\\t-rex.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 10;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 11)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    if (File.Exists("bin\\xmrig\\xmrig.exe"))
                        File.Delete("bin\\xmrig\\xmrig.exe");

                    if (File.Exists("bin_3rdparty\\t-rex\\t-rex.exe"))
                        File.Delete("bin_3rdparty\\t-rex\\t-rex.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 11;
                }

                if (Configs.ConfigManager.GeneralConfig.ForkFixVersion < 11.1)
                {
                    Helpers.ConsolePrint("NICEHASH", "Old version");
                    if (Directory.Exists("internals"))
                        Directory.Delete("internals", true);

                    if (File.Exists("bin_3rdparty\\t-rex\\t-rex.exe"))
                        File.Delete("bin_3rdparty\\t-rex\\t-rex.exe");

                    ConfigManager.GeneralConfig.ForkFixVersion = 11.1;
                }
*/
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
                //**
                Thread.Sleep(100);

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
                    Application.Run(new Form_ChooseLanguage());
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
                        Application.Run(new Form_Main());
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
