using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NiceHashMiner.Forms
{
    public partial class WaitingForm : Form
    {
        bool _ended = false;
        private Task task1;
        public string Title = "";
        public string Text = "";
        public WaitingForm()
        {
            InitializeComponent();
        }
        
        private void WaitingForm_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            this.BringToFront();
            this.CenterToScreen();
        }

        private void WaitingForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        public void CloseWaitingBox()
        {
            try
            {
                _ended = true;
                System.Threading.Thread.Sleep(200);
                task1.Dispose();
            } catch (Exception ex)
            {

            }
        }
        public void ShowWaitingBox()
        {
            task1 = new Task(() => ShowForm());
            task1.Start();
        }
        public void SetText(string Title, string Text)
        {
            this.Title = Title;
            this.Text = Text;
        }
        public void ShowForm()
        {
            this.TopMost = true;
            this.Show();
            while (!_ended)
            {
                //this.Text = Text;
                this.SetLabel(Text);
                this.Update();
                this.Refresh();
                System.Threading.Thread.Sleep(50);
            }
            this.Close();
        }
        private void WaitingForm_Shown(object sender, EventArgs e)
        {

        }
    }
}
