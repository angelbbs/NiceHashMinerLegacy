using NiceHashMiner.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NiceHashMiner.Updater
{
    public static class Updater
    {
        public static void Downloader()
        {
            string fileName = "temp/" + Form_Main.githubName;
            if(!Directory.Exists("temp")) Directory.CreateDirectory("temp");
            if (File.Exists(fileName) != true)
            {
                File.Delete(fileName);
            }
            Helpers.ConsolePrint("Updater", "Try download " + Form_Main.github_browser_download_url);
            WebClient client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.DownloadFileAsync(new Uri(Form_Main.github_browser_download_url), "temp/"+Form_Main.githubName);
        }
        static void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //то что будет после скачивания файла
        }
        static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Form_Main.ProgressBarUpd(e);
            
        }

        public static void DownloadSetup(string version)
        {
            /*
         DownloadSetup ProgramSetup = new DownloadSetup(Form_Main.github_browser_download_url, Form_Main.githubName, "temp");
        var downloadSetup =
        new Form_Loading(new MinersDownloader(ProgramSetup));
            //SetChildFormCenter(downloadSetup);
           // downloadSetup.ShowDialog();
           */
            Downloader();
        }
    }
}
