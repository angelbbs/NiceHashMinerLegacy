using NiceHashMiner.Configs;
using System;
using System.Windows.Forms;

namespace NiceHashMiner.Forms.Components
{
    public partial class Field : UserControl
    {
        public string LabelText
        {
            get => labelFieldIndicator.Text;
            set
            {
                if (value != null)
                {
                    labelFieldIndicator.Text = value;
                }
            }
        }

        public string EntryText
        {
            get => textBox.Text;
            set
            {
                if (value != null)
                {
                    textBox.Text = value;
                }
            }
        }

        public void InitLocale(ToolTip toolTip1, string infoLabel, string infoMsg)
        {
            labelFieldIndicator.Text = infoLabel;
            toolTip1.SetToolTip(labelFieldIndicator, infoMsg);
            toolTip1.SetToolTip(textBox, infoMsg);
            if (ConfigManager.GeneralConfig.ColorProfileIndex != 0)
            {
                textBox.BackColor = Form_Main._backColor;
                textBox.ForeColor = Form_Main._foreColor;
            }
        }

        public void SetInputModeDoubleOnly()
        {
            textBox.KeyPress += TextBoxKeyPressEvents.TextBoxDoubleOnly_KeyPress;
        }

        public void SetInputModeIntOnly()
        {
            textBox.KeyPress += TextBoxKeyPressEvents.TextBoxIntsOnly_KeyPress;
        }

        public void SetOnTextChanged(EventHandler textChanged)
        {
            textBox.TextChanged += textChanged;
        }

        public void SetOnTextLeave(EventHandler textLeave)
        {
            textBox.Leave += textLeave;
        }

        public Field()
        {
            InitializeComponent();
        }
    }
}
