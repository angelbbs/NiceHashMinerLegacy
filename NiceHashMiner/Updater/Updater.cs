﻿using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMiner.Miners;
using NiceHashMiner.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Updater
{
    public static class Updater
    {
        private static bool _autoupdate;
        public static void Downloader(bool autoupdate)
        {
            if (ConfigManager.GeneralConfig.BackupBeforeUpdate)
            {
                CreateBackup();
            }
            if (autoupdate)
            {
                var dialogRes = Utils.MessageBoxEx.Show("Continue update?", "Autoupdate", MessageBoxButtons.YesNo, MessageBoxIcon.Question, 5000);
                if (dialogRes == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            _autoupdate = autoupdate;
            string fileName = "temp/" + Form_Main.progName;
            if(!Directory.Exists("temp")) Directory.CreateDirectory("temp");
            if (File.Exists(fileName) != true)
            {
                File.Delete(fileName);
            }
            Helpers.ConsolePrint("Updater", "Try download " + Form_Main.browser_download_url);
            try
            {
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)SslProtocols.None;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                WebClient client = new WebClient();
                client.UseDefaultCredentials = false;
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

                client.DownloadFileAsync(new Uri(Form_Main.browser_download_url), "temp/" + Form_Main.progName);
            }
            catch (WebException er)
            {
                if (!ConfigManager.GeneralConfig.ProgramAutoUpdate && !_autoupdate)
                {
                    //MessageBox.Show(er.Message + " Response: " + er.Response);
                }
                Helpers.ConsolePrint("Updater error: ", er.Message);
            }
        }
        static void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (ConfigManager.GeneralConfig.ProgramAutoUpdate && _autoupdate)
            {
                if (e.Error != null)
                {
                    Helpers.ConsolePrint("AutoUpdater: ", e.Error.Message);
                    return;
                }
                else
                {
                    string curdir = Environment.CurrentDirectory;
                    Helpers.ConsolePrint("Updater", "Start update to " + curdir);
                    MinersManager.StopAllMiners();
                    if (Miner._cooldownCheckTimer != null && Miner._cooldownCheckTimer.Enabled) Miner._cooldownCheckTimer.Stop();
                    MessageBoxManager.Unregister();
                    ConfigManager.GeneralConfigFileCommit();
                    Thread.Sleep(1000);
                    Process setupProcess = new Process();
                    setupProcess.StartInfo.FileName = @"temp\" + Form_Main.progName;
                    setupProcess.StartInfo.Arguments = "/silent /dir=\"" + curdir + "\"";
                    setupProcess.Start();
                }
            }
            else
            {
                if (e.Error != null)
                {
                    Helpers.ConsolePrint("Updater: ", e.Error.Message);
                    //MessageBox.Show(e.Error.Message);
                }
                else
                {
                    //issetup = true;
                    string curdir = Environment.CurrentDirectory;
                    if (MessageBox.Show(International.GetText("Form_Settings_MessageBoxUpdate"),
                        International.GetText("Program update"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        Helpers.ConsolePrint("Updater", "Start update to " + curdir);
                        MinersManager.StopAllMiners();
                        if (Miner._cooldownCheckTimer != null && Miner._cooldownCheckTimer.Enabled) Miner._cooldownCheckTimer.Stop();
                        MessageBoxManager.Unregister();
                        ConfigManager.GeneralConfigFileCommit();

                        Process setupProcess = new Process();
                        setupProcess.StartInfo.FileName = @"temp\" + Form_Main.progName;
                        setupProcess.StartInfo.Arguments = "/dir=\"" + curdir + "\"";
                        setupProcess.Start();
                    }
                    else
                    {
                        Helpers.ConsolePrint("Updater: ", "Cancel update to " + curdir);
                    }
                }
            }
        }
        static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Form_Main.ProgressBarUpd(e);

        }

        private static void CreateBackup()
        {
            string fname = Form_Main.currentBuild.ToString("00000000.00");
            try
            {
                var CMDconfigHandleBackup = new Process

                {
                    StartInfo =
                {
                    FileName = "utils\\7z.exe"
                }
                };

                if (Directory.Exists("backup"))
                {
                    var dirInfo = new DirectoryInfo("backup");
                    foreach (var file in dirInfo.GetFiles()) file.Delete();
                    dirInfo.Delete();
                }

                CMDconfigHandleBackup.StartInfo.Arguments = "a -tzip -mx3 -ssw -r -y -x!backup backup\\backup_" + fname + ".zip";
                CMDconfigHandleBackup.StartInfo.UseShellExecute = false;
                CMDconfigHandleBackup.StartInfo.CreateNoWindow = false;
                //CMDconfigHandleBackup.Exited += new EventHandler(CMDconfigHandleBackup_Exited);
                CMDconfigHandleBackup.Start();
                CMDconfigHandleBackup.WaitForExit();
                Helpers.ConsolePrint("BACKUP", "Error code: " + CMDconfigHandleBackup.ExitCode);
                if (CMDconfigHandleBackup.ExitCode != 0)
                {
                    //MessageBox.Show("Error code: " + CMDconfigHandleBackup.ExitCode,
                    //"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("Backup", ex.ToString());
                //MessageBox.Show("Unknown error ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Directory.Exists("backup"))
            {
                var dirInfo = new DirectoryInfo("backup");
                foreach (var file in dirInfo.GetFiles())
                {
                    if (file.Name.Contains("backup_") && file.Name.Contains(".zip"))
                    {
                        Form_Main.BackupFileName = file.Name.Replace("backup_", "").Replace(".zip", "");
                        Form_Main.BackupFileDate = file.CreationTime.ToString("dd.MM.yyyy HH:mm");
                    }
                }
                //Form_Benchmark.RunCMDAfterBenchmark();
                try
                {
                    var cmdFile = "@echo off\r\n" +
                        "taskkill /F /IM \"MinerLegacyForkFixMonitor.exe\"\r\n" +
                        "taskkill /F /IM \"NiceHashMinerLegacy.exe\"\r\n" +
                        "timeout /T 2 /NOBREAK\r\n" +
                        "utils\\7z.exe x -r -y " + "backup\\backup_" + fname + ".zip" + "\r\n" +
                        "start NiceHashMinerLegacy.exe\r\n";
                    FileStream fs = new FileStream("backup\\restore.cmd", FileMode.Create, FileAccess.Write);
                    StreamWriter w = new StreamWriter(fs);
                    w.WriteAsync(cmdFile);
                    w.Flush();
                    w.Close();
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("Restore", ex.ToString());
                }
            }
        }

        public static double GetGITHUBVersion()
        {
            //github
            string url = "https://api.github.com/repos/angelbbs/NiceHashMinerLegacy/releases/latest";
            //string url = "https://api.github.com/repos/angelbbs/nhmlff_update/releases/latest";
            string tagname = "";
            string r1 = GetGitHubAPIData(url);
            if (r1 != null & !r1.Contains("(404)"))
            {
                try
                {
                    dynamic nhjson = JsonConvert.DeserializeObject(r1, Globals.JsonSettings);
                    //var latest = Array.Find(nhjson, (n) => n.target_commitish == "master-old");
                    var gitassets = nhjson.assets;
                    for (var i = 0; i < gitassets.Count; i++)
                    {
                        string n = gitassets[i].name;
                        if (n.Contains("Setup.exe"))
                        {
                            Form_Main.progName = n;
                            Form_Main.browser_download_url = gitassets[i].browser_download_url;
                            DateTime build = gitassets[i].updated_at;
                            string buildDate = build.ToString("u").Replace("-", "").Replace(":", "").Replace("Z", "").Replace(" ", ".");
                            Double.TryParse(buildDate.ToString(), out Form_Main.githubBuild);
                        }
                    }
                    tagname = nhjson.tag_name;
                    Double.TryParse(tagname.Replace("Fork_Fix_", "").ToString(), out Form_Main.githubVersion);
                    return (double)Form_Main.githubVersion;
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("GITHUB", ex.ToString());
                    Helpers.ConsolePrint("GITHUB", "Dev github account banned or not found!");
                    Form_Main.githubBuild = 0;
                    Form_Main.githubVersion = 0;
                    return 0.0d;
                }
            } else
            {
                Helpers.ConsolePrint("GITHUB", "ERROR! Dev github account banned or not found!");
                Form_Main.githubBuild = 0;
                Form_Main.githubVersion = 0;
                return 0.0d;
            }
        }
        public static double GetGITLABVersion()
        {
            //gitlab
            string url = "https://gitlab.com/api/v4/projects/26404146/repository/tags";
            string r2 = GetGitHubAPIData(url);
            if (r2 != null & !r2.Contains("(404)"))
            {
                try
                {
                    dynamic gitlabjson = JsonConvert.DeserializeObject(r2, Globals.JsonSettings);
                    string tag = (gitlabjson[0].name).ToString();
                    Double.TryParse(tag.Replace("Fork_Fix_", "").ToString(), out Form_Main.gitlabVersion);
                    Helpers.ConsolePrint("GITLAB", tag);

                    url = "https://gitlab.com/api/v4/projects/26404146/releases/" + tag;
                    string r3 = GetGitHubAPIData(url);
                    dynamic gitlabjson2 = JsonConvert.DeserializeObject(r3, Globals.JsonSettings);
                    int count = gitlabjson2.assets.count;
                    foreach (var l in gitlabjson2.assets.links)
                    {
                        string url0 = l.url;
                        string name = l.name;
                        if (name.Contains("installer"))
                        {
                            Form_Main.browser_download_url = url0;
                            //Form_Main.progName = url0.Substring(url0.LastIndexOfAny(@"/".ToCharArray()) + 1);
                            Form_Main.progName = "NHML.Fork.Fix." + Form_Main.gitlabVersion.ToString() + ".Setup.exe";
                        }
                        if (name.Contains("iners"))
                        {
                            Form_Main.miners_url = url0;
                        }
                    }
                    
                    return (double)Form_Main.gitlabVersion;
                }
                catch (Exception ex2)
                {
                    Helpers.ConsolePrint("GITLAB", ex2.ToString());
                    Helpers.ConsolePrint("GITLAB", "ERROR! Dev gitlab account banned or not found!");
                    Form_Main.gitlabBuild = 0;
                    Form_Main.gitlabVersion = 0;
                    return 0.0d;
                }
            }
            else
            {
                Helpers.ConsolePrint("GITLAB", "ERROR! Dev gitlab account banned or not found!");
                Form_Main.gitlabBuild = 0;
                Form_Main.gitlabVersion = 0;
                return 0.0d;
            }
        }
        public static string GetGitHubAPIData(string URL)
        {
            string ResponseFromServer;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create(URL);
                WR.UserAgent = "NiceHashMinerLegacy/" + Application.ProductVersion;
                WR.Timeout = 10 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                //idHTTP1.IOHandler:= IdSSLIOHandlerSocket1;
                // ServicePointManager.SecurityProtocol = (SecurityProtocolType)SslProtocols.Tls12;
                Thread.Sleep(200);
                WebResponse Response = WR.GetResponse();
                Stream SS = Response.GetResponseStream();
                SS.ReadTimeout = 5 * 1000;
                StreamReader Reader = new StreamReader(SS);
                ResponseFromServer = Reader.ReadToEnd();
                if (ResponseFromServer.Length == 0 || (ResponseFromServer[0] != '{' && ResponseFromServer[0] != '['))
                    throw new Exception("Not JSON!");
                Reader.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GITHUB", ex.Message);
                //MessageBox.Show(ex.Message + " Response: " + ex.Data);
                return ex.Message;
            }
            return ResponseFromServer;
        }
    }
}
