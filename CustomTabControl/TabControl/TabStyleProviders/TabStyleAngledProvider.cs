/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
 */

using System.Drawing;
using System.Drawing.Drawing2D;


namespace System.Windows.Forms
{
    [System.ComponentModel.ToolboxItem(false)]
    public class TabStyleAngledProvider : TabStyleProvider
    {
        public TabStyleAngledProvider(CustomTabControl tabControl) : base(tabControl)
        {
            this._ImageAlign = ContentAlignment.MiddleRight;
            this._Overlap = 7;
            this._Radius = 10;

            //	Must set after the _Radius as this is used in the calculations of the actual padding
            this.Padding = new Point(10, 3);

        }

        public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds)
        {
            switch (this._TabControl.Alignment)
            {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X + this._Radius - 2, tabBounds.Y + 2);
                    path.AddLine(tabBounds.X + this._Radius, tabBounds.Y, tabBounds.Right - this._Radius, tabBounds.Y);
                    path.AddLine(tabBounds.Right - this._Radius + 2, tabBounds.Y + 2, tabBounds.Right, tabBounds.Bottom);
                    break;
                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right - this._Radius + 2, tabBounds.Bottom - 2);
                    path.AddLine(tabBounds.Right - this._Radius, tabBounds.Bottom, tabBounds.X + this._Radius, tabBounds.Bottom);
                    path.AddLine(tabBounds.X + this._Radius - 2, tabBounds.Bottom - 2, tabBounds.X, tabBounds.Y);
                    break;
                case TabAlignment.Left:
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X + 2, tabBounds.Bottom - this._Radius + 2);
                    path.AddLine(tabBounds.X, tabBounds.Bottom - this._Radius, tabBounds.X, tabBounds.Y + this._Radius);
                    path.AddLine(tabBounds.X + 2, tabBounds.Y + this._Radius - 2, tabBounds.Right, tabBounds.Y);
                    break;
                case TabAlignment.Right:
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right - 2, tabBounds.Y + this._Radius - 2);
                    path.AddLine(tabBounds.Right, tabBounds.Y + this._Radius, tabBounds.Right, tabBounds.Bottom - this._Radius);
                    path.AddLine(tabBounds.Right - 2, tabBounds.Bottom - this._Radius + 2, tabBounds.X, tabBounds.Bottom);
                    break;
            }
        }

        protected override Brush GetTabBackgroundBrush(int index)
        {
            LinearGradientBrush fillBrush = null;

            //      Capture the colours dependant on selection state of the tab
            Color dark = Color.Transparent;
            Color light = Color.Transparent;

            if (this._TabControl.SelectedIndex == index)
            {
                dark = Form.DefaultBackColor;
                light = SystemColors.Window;
            }
            else if (!this._TabControl.TabPages[index].Enabled)
            {
                light = dark;
            }
            else if (this.HotTrack && index == this._TabControl.ActiveIndex)
            {
                //      Enable hot tracking
                if (Form.ActiveForm != null)
                {
                    dark = Form.ActiveForm.BackColor;
                    light = dark;
                }
            }

            //      Get the correctly aligned gradient
            Rectangle tabBounds = this.GetTabRect(index);
            tabBounds.Inflate(3, 3);
            tabBounds.X -= 1;
            tabBounds.Y -= 1;
            switch (this._TabControl.Alignment)
            {
                case TabAlignment.Top:
                    fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
                    break;
                case TabAlignment.Bottom:
                    fillBrush = new LinearGradientBrush(tabBounds, dark, light, LinearGradientMode.Vertical);
                    break;
                case TabAlignment.Left:
                    fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Horizontal);
                    break;
                case TabAlignment.Right:
                    fillBrush = new LinearGradientBrush(tabBounds, dark, light, LinearGradientMode.Horizontal);
                    break;
            }

            //      Add the blend
            //  fillBrush.Blend = GetBackgroundBlend();

            return fillBrush;
        }

    }
}
