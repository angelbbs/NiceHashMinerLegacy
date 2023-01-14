using NiceHashMiner.Configs;
using NiceHashMiner.Interfaces;
using NiceHashMiner.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace NiceHashMiner
{
    public partial class Form_Loading : Form, IMessageNotifier, IMinerUpdateIndicator
    {
        public interface IAfterInitializationCaller
        {
            void AfterLoadComplete();
        }

        //private int LoadCounter = 0;
        private int TotalLoadSteps;
        private readonly IAfterInitializationCaller AfterInitCaller;

        // init loading stuff
        public Form_Loading(IAfterInitializationCaller initCaller, string loadFormTitle, string startInfoMsg, int totalLoadSteps)
        {
            InitializeComponent();
            int R = Math.Abs(Color.FromArgb(Form_Main._backColor.ToArgb()).R - 15);
            int G = Math.Abs(Color.FromArgb(Form_Main._backColor.ToArgb()).G - 15);
            int B = Math.Abs(Color.FromArgb(Form_Main._backColor.ToArgb()).B - 15);

            //Helpers.ConsolePrint("RGB", "R: " + R.ToString() + " G: " + G.ToString() + " B:" + B.ToString());
            this.BackColor = Color.FromArgb(255, R, G, B);
            if (R * 256 * 256 + G * 256 + B > 12000000)
            {
                this.ForeColor = Color.Black;
            }
            else
            {
                this.ForeColor = Color.White;
            }
            label_LoadingText.Text = loadFormTitle;
            label_LoadingText.Location = new Point((this.Size.Width - label_LoadingText.Size.Width) / 2, label_LoadingText.Location.Y);
            label_LoadingText.ForeColor = Form_Main._foreColor;
            label_LoadingText.BackColor = this.BackColor;
            AfterInitCaller = initCaller;

            TotalLoadSteps = totalLoadSteps;
            progressBar2.Maximum = TotalLoadSteps;
            progressBar2.Value = 0;

            SetInfoMsg(startInfoMsg);
            if (ConfigManager.GeneralConfig.AlwaysOnTop) this.TopMost = true;
        }

        public void IncreaseLoadCounterAndMessage(string infoMsg)
        {
            SetInfoMsg(infoMsg);
            IncreaseLoadCounter();
        }

        public void SetProgressMaxValue(int maxValue)
        {
            this.progressBar2.Maximum = maxValue;
        }
        public void SetInfoMsg(string infoMsg)
        {
            this.LoadText.Text = infoMsg;
        }

        public void IncreaseLoadCounter()
        {
            /*
            LoadCounter++;
            this.progressBar2.Value = LoadCounter;
            this.Update();
            
            if (LoadCounter >= TotalLoadSteps) {
                AfterInitCaller.AfterLoadComplete();
                this.Close();
                this.Dispose();
            }
            */
        }

        public void FinishLoad()
        {
            this.Close();
            this.Dispose();
            AfterInitCaller.AfterLoadComplete();
            /*
            while (LoadCounter < TotalLoadSteps) {
                IncreaseLoadCounter();
            }
            */
        }

        public void SetValueAndMsg(int setValue, string infoMsg)
        {
            //SetInfoMsg(infoMsg);
            progressBar2.Maximum = TotalLoadSteps;
            this.LoadText.Text = infoMsg;
            progressBar2.Value = setValue;
            this.Update();
            progressBar2.Update();
            progressBar2.Refresh();
            this.Update();
            this.Refresh();
            Thread.Sleep(10);
            /*
            if (progressBar2.Value >= progressBar2.Maximum)
            {
                Thread.Sleep(200);
                this.Close();
                this.Dispose();
                AfterInitCaller.AfterLoadComplete();
            }
            */
        }

        #region IMessageNotifier
        public void SetMessage(string infoMsg)
        {
            SetInfoMsg(infoMsg);
        }

        public void SetMessageAndIncrementStep(string infoMsg)
        {
            IncreaseLoadCounterAndMessage(infoMsg);
        }
        #endregion //IMessageNotifier

        #region IMinerUpdateIndicator
        public void SetMaxProgressValue(int max)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.progressBar2.Maximum = max;
                this.progressBar2.Value = 0;
            });
        }

        public void SetProgressValueAndMsg(int value, string msg)
        {
            if (value <= this.progressBar2.Maximum)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.progressBar2.Value = value;
                    this.LoadText.Text = msg;
                    this.progressBar2.Invalidate();
                    this.LoadText.Invalidate();
                });
            }
        }

        public void SetTitle(string title)
        {
            this.Invoke((MethodInvoker)delegate
            {
                label_LoadingText.Text = title;
            });
        }

        public void FinishMsg(bool ok)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (ok)
                {
                    label_LoadingText.Text = "Init Finished!";
                }
                else
                {
                    label_LoadingText.Text = "Init Failed!";
                }
                System.Threading.Thread.Sleep(100);
                Close();
            });
        }

        #endregion IMinerUpdateIndicator


        private void Form_Loading_Shown(object sender, EventArgs e)
        {
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
