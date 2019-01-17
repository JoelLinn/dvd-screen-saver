using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DvdScreenSaver
{
    public partial class MainForm : Form
    {        
        
        #region Preview API's

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion

        bool IsPreviewMode = false;
        Random rnd = new Random();
        float vel = 0.003f; // fraction of screen width per step
        float velX = 0;
        float velY = 0;
        int cornerCount = 0; // how often have we hit the corner

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
        }

        //This constructor is passed the bounds this form is to show in
        //It is used when in normal mode
        public MainForm(Rectangle Bounds, Random Rnd)
        {
            InitializeComponent();
            this.Bounds = Bounds;
            this.rnd = Rnd;
            //hide the cursor
            Cursor.Hide();
        }

        //This constructor is the handle to the select screensaver dialog preview window
        //It is used when in preview mode (/p)
        public MainForm(IntPtr PreviewHandle)
        {
            InitializeComponent();

            //set the preview window as the parent of this window
            SetParent(this.Handle, PreviewHandle);

            //make this a child window, so when the select screensaver dialog closes, this will also close
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            //set our window's size to the size of our window's new parent
            Rectangle ParentRect;
            GetClientRect(PreviewHandle, out ParentRect);
            this.Size = ParentRect.Size;

            //set our location at (0, 0)
            this.Location = new Point(0, 0);

            IsPreviewMode = true;
        }

        #endregion

        #region GUI

        //sets up the fake BSOD
        private void MainForm_Shown(object sender, EventArgs e)
        {
            // set image size
            var r = (float)pictureBox1.Height / pictureBox1.Width;
            pictureBox1.Width = (int)(0.2 * Width);
            pictureBox1.Height = (int)(r * pictureBox1.Width);

            // image location
            pictureBox1.Location = new Point(rnd.Next(0, Width - pictureBox1.Width), rnd.Next(0, Height - pictureBox1.Height));

            // image speed
            velX = velY = Math.Max(vel * Width, 1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var pos = pictureBox1.Location;
            pos.X += (int)velX;
            pos.Y += (int)velY;

            int count = 0;
            if (pos.X < 0)
            {
                velX = Math.Abs(velX);
                count++;
            }
            if (pos.Y < 0)
            {
                velY = Math.Abs(velY);
                count++;
            }
            if (pos.X > Width - pictureBox1.Width)
            {
                velX = -Math.Abs(velX);
                count++;
            }
            if (pos.Y > Height - pictureBox1.Height)
            {
                velY = -Math.Abs(velY);
                count++;
            }

            pictureBox1.Location = pos;


            if (count == 2)
                this.cornerCount++;
            if (this.cornerCount > 0)
            {
                labelCount.Text = this.cornerCount.ToString();
                labelCount.Visible = true;
            }

            var c = new HSLColor(pictureBox1.BackColor);
            c.Luminosity = 120;
            c.Saturation = 120;
            c.Hue = c.Hue < 240 ? c.Hue + .5 : 0;
            pictureBox1.BackColor = c;
        }
        #endregion

        #region User Input

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsPreviewMode) //disable exit functions for preview
            {
                Application.Exit();
            }
        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            if (!IsPreviewMode) //disable exit functions for preview
            {
                Application.Exit();
            }
        }

        //start off OriginalLoction with an X and Y of int.MaxValue, because
        //it is impossible for the cursor to be at that position. That way, we
        //know if this variable has been set yet.
        Point OriginalLocation = new Point(int.MaxValue, int.MaxValue);

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsPreviewMode) //disable exit functions for preview
            {
                //see if originallocat5ion has been set
                if (OriginalLocation.X == int.MaxValue & OriginalLocation.Y == int.MaxValue)
                {
                    OriginalLocation = e.Location;
                }
                //see if the mouse has moved more than 20 pixels in any direction. If it has, close the application.
                if (Math.Abs(e.X - OriginalLocation.X) > 20 | Math.Abs(e.Y - OriginalLocation.Y) > 20)
                {
                    Application.Exit();
                }
            }
        }

        #endregion
    }
}
