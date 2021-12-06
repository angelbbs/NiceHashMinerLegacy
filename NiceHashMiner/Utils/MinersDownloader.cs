using MyDownloader.Core;
using MyDownloader.Core.Extensions;
using MyDownloader.Core.UI;
using MyDownloader.Extension.Protocols;
using NiceHashMiner.Interfaces;
using SharpCompress.Archive;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace NiceHashMiner.Utils
{
    public class MinersDownloader
    {
        private const string Tag = "MinersDownloader";

        private readonly DownloadSetup _downloadSetup;

        private Downloader _downloader;
        private System.Threading.Timer _timer;
        private int _ticksSinceUpdate;
        private long _lastProgress;
        private Thread _unzipThread;

        private bool _isDownloadSizeInit;

        private IMinerUpdateIndicator _minerUpdateIndicator;

        public MinersDownloader(DownloadSetup downloadSetup)
        {
            _downloadSetup = downloadSetup;

            var extensions = new List<IExtension>();
            try
            {
                extensions.Add(new CoreExtention());
                extensions.Add(new HttpFtpProtocolExtension());
            }
            catch { }
        }

        public void Start(IMinerUpdateIndicator minerUpdateIndicator)
        {
            _minerUpdateIndicator = minerUpdateIndicator;

            // if something not right delete previous and download new
            try
            {
                if (File.Exists(_downloadSetup.BinsZipLocation))
                {
                    File.Delete(_downloadSetup.BinsZipLocation);
                }
                if (Directory.Exists(_downloadSetup.ZipedFolderName))
                {
                    Directory.Delete(_downloadSetup.ZipedFolderName, true);
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("MinersDownloader", e.Message);
            }
            Download();
        }

        // #2 download the file
        private void Download()
        {
            _lastProgress = 0;
            _ticksSinceUpdate = 0;

            _minerUpdateIndicator.SetTitle(International.GetText("MinersDownloadManager_Title_Downloading"));

            DownloadManager.Instance.DownloadEnded += DownloadCompleted;

            var location = ResourceLocation.FromURL(_downloadSetup.BinsDownloadUrl);
            var mirrors = new ResourceLocation[0];

            _downloader = DownloadManager.Instance.Add(
                location,
                mirrors,
                _downloadSetup.BinsZipLocation,
                5,
                true);

            _timer = new System.Threading.Timer(TmrRefresh_Tick);
            _timer.Change(0, 500);
        }

        #region Download delegates

        private void TmrRefresh_Tick(object stateInfo)
        {
            if (_downloader == null || _downloader.State != DownloaderState.Working) return;
            if (!_isDownloadSizeInit)
            {
                _isDownloadSizeInit = true;
                _minerUpdateIndicator.SetMaxProgressValue((int)(_downloader.FileSize / 1024));
            }

            if (_downloader.LastError != null)
            {
                Helpers.ConsolePrint("MinersDownloader", _downloader.LastError.Message);
            }

            var speedString = $"{_downloader.Rate / 1024d:0.00} kb/s";
            var percString = _downloader.Progress.ToString("0.00") + "%";
            var labelDownloaded =
                $"{_downloader.Transfered / 1024d / 1024d:0.00} MB / {_downloader.FileSize / 1024d / 1024d:0.00} MB";
            _minerUpdateIndicator.SetProgressValueAndMsg((int)(_downloader.Transfered / 1024d),
                $"{speedString}   {percString}   {labelDownloaded}");

            // Diagnostic stuff
            if (_downloader.Transfered > _lastProgress)
            {
                _ticksSinceUpdate = 0;
                _lastProgress = _downloader.Transfered;
            }
            else if (_ticksSinceUpdate > 20)
            {
                _ticksSinceUpdate = 0;
                    Helpers.ConsolePrint("MinersDownloader", "Maximum ticks reached, switching to mirror and restart");
                    try
                    {
                        if (File.Exists("configs//download_from_mirror.flag"))
                        {
                            File.Delete("configs//download_from_mirror.flag");
                        }
                        File.Create("configs//download_from_mirror.flag");
                        Form_Main.MakeRestart(0);
                    }
                    catch
                    {

                    }
                    Updater.Updater.GetGITLABVersion();
            }
            else
            {
                Helpers.ConsolePrint("MinersDownloader", "No progress in ticks " + _ticksSinceUpdate);
                _ticksSinceUpdate++;
            }
        }

        // The event that will trigger when the WebClient is completed
        private void DownloadCompleted(object sender, DownloaderEventArgs e)
        {
            _timer.Dispose();

            if (_downloader != null)
            {
                if (_downloader.State == DownloaderState.EndedWithError)
                {
                    Helpers.ConsolePrint("MinersDownloader", _downloader.LastError.Message);
                }
                else if (_downloader.State == DownloaderState.Ended)
                {
                    Helpers.ConsolePrint(Tag, "DownloadCompleted Success");
                    Thread.Sleep(100);
                    var tryCount = 50;
                    while (!File.Exists(_downloadSetup.BinsZipLocation) && tryCount > 0) { --tryCount; }

                    UnzipStart();
                }
            }
        }

        #endregion Download delegates


        private void UnzipStart()
        {
            try
            {
                _minerUpdateIndicator.SetTitle(International.GetText("MinersDownloadManager_Title_Settup"));
            }
            catch { }
            _unzipThread = new Thread(UnzipThreadRoutine);
            _unzipThread.Start();
        }

        private void UnzipThreadRoutine()
        {
            Forms.Form_Benchmark.RunCMDAfterBenchmark();
            try
            {
                if (File.Exists(_downloadSetup.BinsZipLocation))
                {
                    Helpers.ConsolePrint(Tag, _downloadSetup.BinsZipLocation + " already downloaded");
                    Helpers.ConsolePrint(Tag, "unzipping");

                    // if using other formats as zip are returning 0
                    var fileArchive = new FileInfo(_downloadSetup.BinsZipLocation);
                    var archive = ArchiveFactory.Open(_downloadSetup.BinsZipLocation);
                    _minerUpdateIndicator.SetMaxProgressValue(100);
                    long sizeCount = 0;
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            sizeCount += entry.CompressedSize;
                            Helpers.ConsolePrint(Tag, entry.Key);

                            var prog = sizeCount / (double)fileArchive.Length * 100;
                            _minerUpdateIndicator.SetProgressValueAndMsg((int)prog, entry.Key.Replace("miners/", ""));
                            entry.WriteToDirectory("", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                    }
                    archive.Dispose();
                    // after unzip stuff
                    _minerUpdateIndicator.FinishMsg(true);
                    // remove bins zip
                    try
                    {
                        if (File.Exists(_downloadSetup.BinsZipLocation))
                        {
                            File.Delete(_downloadSetup.BinsZipLocation);
                        }
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("MinersDownloader.UnzipThreadRoutine", "Cannot delete exception: " + e.Message);
                    }
                }
                else
                {
                    Helpers.ConsolePrint(Tag, $"UnzipThreadRoutine {_downloadSetup.BinsZipLocation} file not found");
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint(Tag, "UnzipThreadRoutine has encountered an error: " + e.Message);

                //untested 
                var dialogRes = Utils.MessageBoxEx.Show(e.Message + "\r\n Restart Windows?",
                    "Autoupdate", MessageBoxButtons.YesNo, MessageBoxIcon.Question, 300000);//5min
                if (dialogRes == System.Windows.Forms.DialogResult.Yes)
                {
                    var OSrestart = new ProcessStartInfo("shutdown")
                    {
                        WindowStyle = ProcessWindowStyle.Minimized
                    };
                    OSrestart.Arguments = "-r -f -t 10";
                    Helpers.ConsolePrint("UnzipThreadRoutine", "Restart Windows");
                    Process.Start(OSrestart);

                }
                else
                {
                    MessageBox.Show("Error!");
                    Form_Main.MakeRestart(0);
                }

            }
        }
    }
}
