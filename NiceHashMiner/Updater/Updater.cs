using Newtonsoft.Json;
using NiceHashMiner.Configs;
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
            if (autoupdate)
            {
                var dialogRes = Utils.MessageBoxEx.Show("Continue update?", "Autoupdate", MessageBoxButtons.YesNo, MessageBoxIcon.Question, 5000);
                if (dialogRes == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            _autoupdate = autoupdate;
            string fileName = "temp/" + Form_Main.githubName;
            if(!Directory.Exists("temp")) Directory.CreateDirectory("temp");
            if (File.Exists(fileName) != true)
            {
                File.Delete(fileName);
            }
            Helpers.ConsolePrint("Updater", "Try download " + Form_Main.github_browser_download_url);
            try
            {
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)SslProtocols.None;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                WebClient client = new WebClient();
                client.UseDefaultCredentials = false;
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

                client.DownloadFileAsync(new Uri(Form_Main.github_browser_download_url), "temp/" + Form_Main.githubName);
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
                    setupProcess.StartInfo.FileName = @"temp\" + Form_Main.githubName;
                    setupProcess.StartInfo.Arguments = "/silent /dir=" + curdir;
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
                        setupProcess.StartInfo.FileName = @"temp\" + Form_Main.githubName;
                        setupProcess.StartInfo.Arguments = "/dir=" + curdir;
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

        public static Tuple<string, double> GetVersion()
        {
            string url = "https://api.github.com/repos/angelbbs/NiceHashMinerLegacy/releases/latest";
            //string url = "https://api.github.com/repos/angelbbs/nhmlff_update/releases/latest";
            string r1 = GetGitHubAPIData(url);
            if (r1 == null) return Tuple.Create("", 0d); 
            //github_version nhjson;
            string tagname = "";
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
                        Form_Main.githubName = n;
                        Form_Main.github_browser_download_url = gitassets[i].browser_download_url;
                        DateTime build = gitassets[i].updated_at;
                        string buildDate = build.ToString("u").Replace("-", "").Replace(":", "").Replace("Z", "").Replace(" ", ".");
                        Double.TryParse(buildDate.ToString(), out Form_Main.githubBuild);
                    }
                }
                tagname = nhjson.tag_name;
                Double.TryParse(tagname.Replace("Fork_Fix_", "").ToString(), out Form_Main.githubVersion);
                return Tuple.Create(tagname, (double)Form_Main.githubBuild);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("GITHUB", ex.ToString());
            }
            return Tuple.Create("", (double)Form_Main.githubBuild);
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
                return null;
            }
            return ResponseFromServer;
        }
    }
}
