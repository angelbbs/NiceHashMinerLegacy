using Newtonsoft.Json;
using NiceHashMiner.Configs;
using NiceHashMiner.Devices;
using NiceHashMiner.Miners;
using NiceHashMiner.Utils;
using SharpCompress.Archive;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NiceHashMiner.Miners.MinerVersion;

namespace NiceHashMiner.Forms
{
    public partial class Form_Downloading : Form
    {
        private DateTime time2 = new DateTime();
        private int BytesReceived1 = 0;
        private int BytesReceived2 = 0;
        private List<double> speedArray = new List<double>();
        public Form_Downloading()
        {
            InitializeComponent();
            this.Height = 92;
            labelTitle.Text = International.GetText("MinersDownloadManager_Title_Downloading");
            int R = Math.Abs(Color.FromArgb(Form_Main._backColor.ToArgb()).R - 15);
            int G = Math.Abs(Color.FromArgb(Form_Main._backColor.ToArgb()).G - 15);
            int B = Math.Abs(Color.FromArgb(Form_Main._backColor.ToArgb()).B - 15);

            this.BackColor = Color.FromArgb(255, R, G, B);
            if (R * 256 * 256 + G * 256 + B > 12000000)
            {
                this.ForeColor = Color.Black;
            }
            else
            {
                this.ForeColor = Color.White;
            }

            labelTitle.Location = new Point((this.Size.Width - labelTitle.Size.Width) / 2, labelTitle.Location.Y);
            labelTitle.ForeColor = Form_Main._foreColor;
            labelTitle.BackColor = this.BackColor;

            progressBarDownloading.Maximum = 100;
            progressBarDownloading.Value = 0;
            this.Update();
        }
        public void client_EmergencyDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBarDownloading.Maximum = (int)e.TotalBytesToReceive / 100;

            var time1 = DateTime.Now;
            var time3 = time1 - time2;
            if (time3.TotalMilliseconds < 200) return;
            BytesReceived2 = (int)e.BytesReceived;
            var speed = (BytesReceived2 - BytesReceived1) / time3.TotalMilliseconds;
            speedArray.Add(speed);
            var averageSpeed = speedArray.Sum() / speedArray.Count;

            double current = Math.Round((double)e.BytesReceived / 1000000, 2);
            double total = Math.Round((double)e.TotalBytesToReceive / 1000000, 2);
            LoadText.Text = current.ToString("F2") + "MB / " + total.ToString("F2") + "MB" + "   " + International.GetText("MinersDownloadManager_DownloadAverageSpeed") + " " + averageSpeed.ToString("F0") + " KB/s";

            progressBarDownloading.Value = (int)e.BytesReceived / 100;

            time2 = DateTime.Now;
            BytesReceived1 = (int)e.BytesReceived;
        }
        public void client_EmergencyDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Height = 184;
            labelTitle2.Text = International.GetText("MinersDownloadManager_Title_Settup");
            labelTitle2.Location = new Point((this.Size.Width - labelTitle2.Size.Width) / 2, labelTitle2.Location.Y);
            labelTitle.ForeColor = Form_Main._foreColor;
            labelTitle.BackColor = this.BackColor;
            this.Update();

            progressBarDownloading.Value = 100;
            UnzipRoutine();
            Thread.Sleep(200);
            GetMinersVersion();
            Thread.Sleep(2000);
            try
            {
                Form_Main._autostartTimerDelay.Start();
            } catch (Exception ex)
            {
                Helpers.ConsolePrint("client_EmergencyDownloadFileCompleted", ex.ToString());
            }
            Thread.Sleep(1000);//костыль для очередности запуска таймеров

            if (ConfigManager.GeneralConfig.AutoStartMining)
            {
                try
                {
                    if (Form_Main._autostartTimer != null)
                    {
                        Form_Main._autostartTimer.Start();
                    }
                } catch (Exception ex)
                {
                    Helpers.ConsolePrint("client_EmergencyDownloadFileCompleted", ex.ToString());
                }
            }

            Form_Main._deviceStatusTimer.Start();
            Form_Main.DownloadingInProgress = false;
            this.Close();
        }

        private void GetMinersVersion()
        {
            if (ConfigManager.GeneralConfig.GetMinersVersions)
            {
                MinerVersion.MinerDataList.Clear();

                progressBarUnzipping.Maximum = 100;
                progressBarUnzipping.Value = 0;
                progressBarUnzipping.Update();

                var minerdata = new MinerData();

                progressBarUnzipping.Value = 7;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "ClaymoreNeoscrypt";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_ClaymoreNeoscrypt();
                MinerVersion.MinerDataList.Add(minerdata);

                if (ComputeDeviceManager.Query.WindowsDisplayAdapters.HasNvidiaVideoController())
                {
                    progressBarUnzipping.Value = 15;
                    UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "CryptoDredge";
                    UnzippingText.Update();
                    minerdata = MinerVersion.Get_CryptoDredge();
                    MinerVersion.MinerDataList.Add(minerdata);
                }

                progressBarUnzipping.Value = 22;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "GMiner";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_GMiner();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 29;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "lolMiner";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_lolMiner();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 36;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "miniZ";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_miniZ();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 43;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "Nanominer";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_nanominer();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 50;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "NBMiner.39.5";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_NBMiner39_5();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 57;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "NBMiner";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_NBMiner();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 63;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "PhoenixMiner";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_Phoenix();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 70;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "SRBMiner";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_SRBMiner();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 77;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "T-Rex";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_TRex();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 85;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "TeamRedMiner";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_TeamRedMiner();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 92;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "XMRig";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_XMRig();
                MinerVersion.MinerDataList.Add(minerdata);

                progressBarUnzipping.Value = 100;
                UnzippingText.Text = International.GetText("Form_Main_loadtext_GetMinerVersion") + "Rigel";
                UnzippingText.Update();
                minerdata = MinerVersion.Get_Rigel();
                MinerVersion.MinerDataList.Add(minerdata);

                string json = JsonConvert.SerializeObject(MinerDataList, Formatting.Indented);
                try
                {
                    if (File.Exists("Configs\\MinersData.json"))
                    {
                        File.Delete("Configs\\MinersData.json");
                    }
                }
                catch (Exception ex)
                {
                    Helpers.ConsolePrint("CheckMiners", ex.ToString());
                }
                File.WriteAllText("Configs\\MinersData.json", json);
            }
        }
        private void UnzipRoutine()
        {
            Forms.Form_Benchmark.RunCMDAfterBenchmark();

            IArchive archive = ArchiveFactory.Create(ArchiveType.Zip);
            try
            {
                if (File.Exists(Updater.Updater.DownloadedMinersLocation))
                {
                    Helpers.ConsolePrint("UnzipThreadRoutine", Updater.Updater.DownloadedMinersLocation + " already downloaded. Start unzipping");

                    // if using other formats as zip are returning 0
                    var fileArchive = new FileInfo(Updater.Updater.DownloadedMinersLocation);
                    archive = ArchiveFactory.Open(Updater.Updater.DownloadedMinersLocation);
                    progressBarUnzipping.Maximum = 100;
                    progressBarUnzipping.Value = 0;
                    progressBarUnzipping.Update();
                    long sizeCount = 0;
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            sizeCount += entry.CompressedSize;
                            Helpers.ConsolePrint("UnzipThreadRoutine", entry.Key);

                            var prog = sizeCount / (double)fileArchive.Length * 100;
                            progressBarUnzipping.Value = (int)prog;
                            UnzippingText.Text = entry.Key.Replace("miners/", "");
                            UnzippingText.Update();
                            entry.WriteToDirectory("", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                    }
                    archive.Dispose();
                    // after unzip stuff
                    progressBarUnzipping.Value = 100;
                    // remove bins zip
                    try
                    {
                        if (File.Exists(Updater.Updater.DownloadedMinersLocation))
                        {
                            File.Delete(Updater.Updater.DownloadedMinersLocation);
                        }
                    }
                    catch (Exception e)
                    {
                        Helpers.ConsolePrint("UnzipThreadRoutine", "Cannot delete exception: " + e.Message);
                    }
                }
                else
                {
                    Helpers.ConsolePrint("UnzipThreadRoutine", $"UnzipThreadRoutine {Updater.Updater.DownloadedMinersLocation} file not found");
                }
            }
            catch (Exception e)
            {
                Helpers.ConsolePrint("UnzipThreadRoutine", "UnzipThreadRoutine has encountered an error: " + e.Message);
                archive.Dispose();

                //MinersDownloader.MinersEmergencyDownloading(_downloadURL);
                //UnzipRoutine()
            }
            finally
            {

            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x00020000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
    }
}
