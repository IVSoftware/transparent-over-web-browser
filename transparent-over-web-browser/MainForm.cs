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
        }
    }
}
