You're correct that the reason a WinForms control appears transparent is that the "the background of a transparent Windows Forms control is painted by its parent" as explained in the MS [documentation](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-give-your-control-a-transparent-background). This means the parent control's background will be drawn into the Label _even if there's another control (you mentioned web browser) in between_.

The remedy is to change the _shape_ of the Label control itself, by assigning a drawable `Region` that excludes the transparent pixels.

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Change shape of Label control, removing transparent pixels.
            label1.BackColor = Color.Transparent;
            Bitmap bitmap = new Bitmap(label1.Width, label1.Height);
            label1.DrawToBitmap(bitmap, label1.ClientRectangle);
            Region region = new Region(label1.ClientRectangle);
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

[![design mode and runtime][1]][1]


  [1]: https://i.stack.imgur.com/tn2vG.png