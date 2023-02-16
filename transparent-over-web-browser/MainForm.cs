using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace transparent_over_web_browser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Change shape of Label control, removing transparent pixels.
            label1.BackColor = Color.Transparent;
            Bitmap bitmap = new Bitmap(label1.Width, label1.Height);
            label1.DrawToBitmap(bitmap, label1.ClientRectangle);
            var region = new Region(label1.ClientRectangle);
            for (int x = 0; x < label1.Width; x++) for (int y = 0; y < label1.Height; y++)
                {
                    if (bitmap.GetPixel(x, y).A == 0)
                    {
                        region.Exclude(new Rectangle(x, y, 1, 1));
                    }
                }
            label1.Region = region;
            label1.MouseMove += _dragHelper.Restart;

            this.Controls.Add(_dragHelper);
        }
        private readonly DragHelper _dragHelper = new DragHelper();
    }
    class DragHelper : Label
    {
        public int Latency { get; set; } = 750;
        int _wdtCount = 0;
        public void Restart(object sender, MouseEventArgs e)
        {
            if (_dragEnabled)
            {
                if (sender is Label label)
                {
                    _label = label;
                    int captureCount = ++_wdtCount;
                    label.Visible = false;
                    Size = label.Size;
                    Visible = true;
                    Location = label.Location;
                    BackColor = label.BackColor;
                    Font = label.Font;
                    Text = label.Text;
                    TextAlign = label.TextAlign;
                    BringToFront();

                    Task
                    .Delay(Latency)
                    .GetAwaiter()
                    .OnCompleted(() =>
                    {
                        if (captureCount.Equals(_wdtCount))
                        {
                            if (MouseButtons.Equals(MouseButtons.None))
                            {
                                Visible = false;
                                label.Visible = true;
                            }
                        }
                    });
                }
            }
        }
        Label _label = null;

        Point
            // Where's the cursor in relation to screen when mouse button is pressed?
            _mouseDownScreen = new Point(),
            // Where's the 'map' control when mouse button is pressed?
            _controlDownPoint = new Point(),
            // How much has the mouse moved from it's original mouse-down location?
            _mouseDelta = new Point();

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _dragEnabled= false;
            _mouseDownScreen = PointToScreen(e.Location);
            _controlDownPoint = Location;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (MouseButtons.Equals(MouseButtons.Left))
            {
                var screen = PointToScreen(e.Location);
                _mouseDelta = new Point(screen.X - _mouseDownScreen.X, screen.Y - _mouseDownScreen.Y);
                var newControlLocation = new Point(_controlDownPoint.X + _mouseDelta.X, _controlDownPoint.Y + _mouseDelta.Y);
                if (!Location.Equals(newControlLocation))
                {
                    Location = newControlLocation;
                }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _label.Visible = true;
            _label.Location = Location;
            Visible = false;
            // Latency before enable again
            Task
            .Delay(Latency)
            .GetAwaiter()
            .OnCompleted(() =>
            {
                _dragEnabled = true;
            });
        }
        bool _dragEnabled = true;
    }
}
