using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NiceHashMiner.Configs;
using NiceHashMiner.Forms;
using NiceHashMiner.Miners;
using NiceHashMiner.Stats;
using NiceHashMinerLegacy.Common.Enums;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace NiceHashMiner.Updater
{
    public static class Updater
    {
        private static bool _autoupdate;
        public static string DownloadedMinersLocation = "temp/miners.zip";
        public static void Downloader(bool autoupdate)
        {
            string link = Links.CheckDNS(Form_Main.browser_download_url);
            string host = new Uri(Form_Main.browser_download_url).Host;

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
            try
            {
                if (!Directory.Exists("temp")) Directory.CreateDirectory("temp");
                if (File.Exists(fileName) != true)
                {
                    File.Delete(fileName);
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("Downloader", ex.ToString());
            }

            Helpers.ConsolePrint("Updater", "Try download " + link);
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += ((sender, args) =>
                    {
                        if (args.Error == null)
                        {
                            client_DownloadFileCompleted(sender, args);
                        }
                        else
                        {
                            try
                            {
                                Helpers.ConsolePrint("Updater error: ", args.Error.ToString());
                                wc.DownloadFileTaskAsync(new Uri(link), "temp/" + Form_Main.progName);
                            }
                            catch (Exception ex)
                            {
                                Helpers.ConsolePrint("Updater error: ", ex.Message);
                            }
                        }
                    });
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    wc.Headers.Add("Host", host);
                    wc.UseDefaultCredentials = false;
                    wc.DownloadFileTaskAsync(new Uri(link), "temp/" + Form_Main.progName);
                }
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

        public static void EmergencyDownloader(string url, bool ssl = true)
        {
            string link = Links.CheckDNS(url);
            string host = new Uri(url).Host;

            try
            {
                if (!Directory.Exists("temp")) Directory.CreateDirectory("temp");
                if (File.Exists(DownloadedMinersLocation))
                {
                    File.Delete(DownloadedMinersLocation);
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("EmergencyDownloader", ex.ToString());
            }

            Helpers.ConsolePrint("EmergencyDownloader", "Try download " + link);
            try
            {
                if (ssl)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                else
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)0;
                }

                Form_Downloading form = new Form_Downloading();
                form.Show();
                form.progressBarDownloading.Maximum = 100;
                form.progressBarDownloading.Value = 0;
                form.Update();
                Form_Main._deviceStatusTimer.Stop();

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += ((sender, args) =>
                    {
                        if (args.Error == null)
                        {
                            form.client_EmergencyDownloadFileCompleted(sender, args);
                        }
                        else
                        {
                            try
                            {
                                Helpers.ConsolePrint("EmergencyDownloader error: ", args.Error.ToString());
                                wc.DownloadFileTaskAsync(new Uri(link), DownloadedMinersLocation);
                                    //.ContinueWith(t => t.Exception.Message);
                            } catch (Exception ex)
                            {
                                Helpers.ConsolePrint("EmergencyDownloader error: ", ex.Message);
                            }
                        }
                    });
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(form.client_EmergencyDownloadProgressChanged);
                    wc.Headers.Add("Host", host);
                    wc.UseDefaultCredentials = false;
                    wc.DownloadFileTaskAsync(new Uri(link), DownloadedMinersLocation);
                      //.ContinueWith(t => t.Exception.Message);
                }
            }
            catch (WebException er)
            {
                if (!ConfigManager.GeneralConfig.ProgramAutoUpdate && !_autoupdate)
                {
                    //MessageBox.Show(er.Message + " Response: " + er.Response);
                }
                Helpers.ConsolePrint("EmergencyDownloader error: ", er.Message);
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
                    //if (Miner._cooldownCheckTimer != null && Miner._cooldownCheckTimer.Enabled) Miner._cooldownCheckTimer.Stop();
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
                        //if (Miner._cooldownCheckTimer != null && Miner._cooldownCheckTimer.Enabled) Miner._cooldownCheckTimer.Stop();
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
                try
                {
                    if (Directory.Exists("backup"))
                    {
                        var dirInfo = new DirectoryInfo("backup");
                        foreach (var file in dirInfo.GetFiles()) file.Delete();
                        dirInfo.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("CreateBackup", ex.ToString());
                }
                CMDconfigHandleBackup.StartInfo.Arguments = "a -tzip -mx3 -ssw -r -y -x!backup backup\\backup_" + fname + ".zip";
                CMDconfigHandleBackup.StartInfo.UseShellExecute = false;
                CMDconfigHandleBackup.StartInfo.CreateNoWindow = false;
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
                        "taskkill /F /IM \"NvidiaGPUGetDataHost.exe\"\r\n" +
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

        public static string betweenStrings(string text, string start, string end)
        {
            int p1 = text.IndexOf(start) + start.Length;
            int p2 = text.IndexOf(end, p1);

            if (end == "") return (text.Substring(p1));
            else return text.Substring(p1, p2 - p1);
        }

        public static void ShowHistory(bool force)
        {
            string fileHistory = "";
            if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
            {
                fileHistory = "Help\\history_ru.txt";
            }
            else
            {
                fileHistory = "Help\\history_en.txt";
            }

            if (File.Exists(fileHistory))
            {
                string history = "";
                try
                {
                    history = File.ReadAllText(fileHistory);
                    if (!history.Contains("Fork Fix " + Form_Main.currentVersion.ToString()))
                    {
                        File.Delete(fileHistory);
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("ShowHistory", ex.ToString());
                }
            }

            if (!File.Exists(fileHistory))
            {
                string AllReleases = "";
                try
                {
                    AllReleases = GetGITHUBReleases();

                    File.WriteAllText(fileHistory, AllReleases);
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("ShowHistory", ex.ToString());
                }
                force = true;
            }

            if (force)
            {
                try
                {
                    var notepadProcess = new Process

                    {
                        StartInfo =
                {
                    FileName = "notepad.exe"
                }
                    };

                    notepadProcess.StartInfo.Arguments = fileHistory;
                    notepadProcess.StartInfo.UseShellExecute = false;
                    notepadProcess.StartInfo.CreateNoWindow = false;
                    notepadProcess.Start();
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("notepad", ex.Message);
                }
            }
        }


        public static string GetGITHUBReleases()
        {
            string url = Links.githubAllReleases;
            string r1 = GetGitHubAPIData(url, "api.github.com");
            string ret = "";
            if (r1 != null & !r1.Contains("(404)"))
            {
                try
                {
                    JArray nhjson = (JArray)JsonConvert.DeserializeObject(r1, Globals.JsonSettings);
                    foreach (dynamic vers in nhjson)
                    {
                        string tag_name = vers.tag_name;
                        string gitbody = vers.body;
                        string published_at = vers.published_at;
                        //Helpers.ConsolePrint(published_at, "*** " + tag_name.Replace("_", " "));
                        ret = ret + published_at + " *** " + tag_name.Replace("_", " ") + " ***";
                        if (ConfigManager.GeneralConfig.Language == LanguageType.Ru)
                        {
                            //Helpers.ConsolePrint("****gitbody:", betweenStrings(gitbody, "RUS:", "Обсуждение тут"));
                            ret = ret + betweenStrings(gitbody, "RUS:", "Обсуждение тут");
                        } else
                        {
                            Helpers.ConsolePrint("----gitbody:", gitbody);
                            Helpers.ConsolePrint("****gitbody:", betweenStrings(gitbody, "EN:", "Russian discussion forum"));
                            ret = ret + betweenStrings(gitbody, "EN:", "Russian discussion forum");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("GITHUB", ex.ToString());
                    Helpers.ConsolePrint("GITHUB", "Dev github account banned or not found!");
                    return "";
                }
            }
            else
            {
                Helpers.ConsolePrint("GITHUB", "ERROR! Dev github account banned or not found!");
                Form_Main.githubBuild = 0;
                Form_Main.githubVersion = 0;
                return "";
            }
            return ret;
        }

        public static double GetGITHUBVersion()
        {
            //github
            string url = Links.githubLatestRelease;
            string tagname = "";
            string r1 = GetGitHubAPIData(url, "api.github.com");
            if (r1 != null & !r1.Contains("(404)"))
            {
                try
                {
                    //Helpers.ConsolePrint("*****", r1);
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
                    Form_Main.miners_url = "https://github.com/angelbbs/NiceHashMinerLegacy/releases/download/Fork_Fix_" + Form_Main.githubVersion + "/miners.zip";
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
            }
            else
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
            Helpers.ConsolePrint("GITLAB", "Start check gitlab");
            string url = Links.gitlabRepositoryTags;
            string r2 = GetGitHubAPIData(url, "gitlab.com");
            if (r2 != null & !r2.Contains("(404)"))
            {
                try
                {
                    dynamic gitlabjson = JsonConvert.DeserializeObject(r2, Globals.JsonSettings);
                    string tag = (gitlabjson[0].name).ToString();
                    Double.TryParse(tag.Replace("Fork_Fix_", "").ToString(), out Form_Main.gitlabVersion);
                    Helpers.ConsolePrint("GITLAB", tag);

                    url = Links.gitlabLastRelease + tag;
                    string r3 = GetGitHubAPIData(url, "gitlab.com");
                    dynamic gitlabjson2 = JsonConvert.DeserializeObject(r3, Globals.JsonSettings);
                    int count = gitlabjson2.assets.count;
                    foreach (var l in gitlabjson2.assets.links)
                    {
                        string url0 = l.url;
                        string name = l.name;
                        if (name.Contains("installer"))
                        {
                            if (GetGITHUBVersion() == 0)
                            {
                                Form_Main.browser_download_url = url0;
                                //Form_Main.progName = url0.Substring(url0.LastIndexOfAny(@"/".ToCharArray()) + 1);
                                Form_Main.progName = "NHML.Fork.Fix." + Form_Main.gitlabVersion.ToString() + ".Setup.exe";
                            }
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
        public static string GetGitHubAPIData(string URL, string host)
        {
            string ResponseFromServer;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebRequest WR = (HttpWebRequest)WebRequest.Create(URL);
                WR.UserAgent = "NiceHashMinerLegacy/" + Application.ProductVersion;
                WR.Timeout = 10 * 1000;
                WR.Credentials = CredentialCache.DefaultCredentials;
                WR.Host = host;
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
        public static void MinersEmergencyDownloading(string mpath)
        {
            Helpers.ConsolePrint("*******", "Start MinersEmergencyDownloading");
            try
            {
                if (File.Exists("miners.zip")) File.Delete("miners.zip");

                var cports = new Process
                {
                    StartInfo =
                        {
                            FileName = "powershell.exe",
                            UseShellExecute = true,
                            Arguments = "-file common\\MinersEmergencyDownloading.ps1 \"" + mpath + "\"",
                            RedirectStandardError = false,
                            RedirectStandardOutput = false,
                            CreateNoWindow = false,
                            WindowStyle = ProcessWindowStyle.Normal
                }
                };
                cports.Start();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("MinersEmergencyDownloading", ex.ToString());
            }

            do
            {
                Thread.Sleep(1000);
            } while (!File.Exists("miners.zip"));
        }
    }
}
