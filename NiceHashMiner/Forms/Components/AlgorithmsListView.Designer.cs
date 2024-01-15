namespace NiceHashMiner.Forms.Components {
    partial class AlgorithmsListView {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.listViewAlgorithms = new System.Windows.Forms.ListView();
            this.columnHeader0 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // listViewAlgorithms
            // 
            this.listViewAlgorithms.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.listViewAlgorithms.CheckBoxes = true;
            this.listViewAlgorithms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader0,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listViewAlgorithms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAlgorithms.FullRowSelect = true;
            this.listViewAlgorithms.GridLines = true;
            this.listViewAlgorithms.HideSelection = false;
            this.listViewAlgorithms.Location = new System.Drawing.Point(0, 0);
            this.listViewAlgorithms.MultiSelect = false;
            this.listViewAlgorithms.Name = "listViewAlgorithms";
            this.listViewAlgorithms.Size = new System.Drawing.Size(539, 380);
            this.listViewAlgorithms.TabIndex = 11;
            this.listViewAlgorithms.UseCompatibleStateImageBehavior = false;
            this.listViewAlgorithms.View = System.Windows.Forms.View.Details;
            this.listViewAlgorithms.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewAlgorithms_ColumnClick);
            this.listViewAlgorithms.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listViewAlgorithms_ColumnWidthChanged);
            this.listViewAlgorithms.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listViewAlgorithms_ColumnWidthChanging);
            this.listViewAlgorithms.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewAlgorithms_ItemCheck);
            this.listViewAlgorithms.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewAlgorithms_ItemChecked_1);
            this.listViewAlgorithms.SelectedIndexChanged += new System.EventHandler(this.listViewAlgorithms_SelectedIndexChanged);
            this.listViewAlgorithms.EnabledChanged += new System.EventHandler(this.listViewAlgorithms_EnabledChanged);
            this.listViewAlgorithms.Click += new System.EventHandler(this.listViewAlgorithms_Click);
            this.listViewAlgorithms.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListViewAlgorithms_MouseClick);
            this.listViewAlgorithms.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewAlgorithms_MouseDown);
            this.listViewAlgorithms.MouseLeave += new System.EventHandler(this.listViewAlgorithms_MouseLeave);
            this.listViewAlgorithms.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listViewAlgorithms_MouseUp);
            this.listViewAlgorithms.Resize += new System.EventHandler(this.listViewAlgorithms_Resize);
            // 
            // columnHeader0
            // 
            this.columnHeader0.Text = "On";
            this.columnHeader0.Width = 28;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Algorithm";
            this.columnHeader1.Width = 110;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Miner";
            this.columnHeader2.Width = 82;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Speed";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 152;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Power";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 88;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Ratio BTC/GH/Day";
            this.columnHeader5.Width = 90;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "BTC/Day";
            this.columnHeader6.Width = 148;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // AlgorithmsListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.listViewAlgorithms);
            this.Name = "AlgorithmsListView";
            this.Size = new System.Drawing.Size(539, 380);
            this.Load += new System.EventHandler(this.AlgorithmsListView_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader columnHeader0;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        public System.Windows.Forms.ListView listViewAlgorithms;
    }
}
