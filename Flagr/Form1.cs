using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flagr
{
    public partial class Form1 : Form
    {

        public static Bitmap Buffer;
        public static Graphics BufferGraphics;
        private Graphics graphics;

        private Stopwatch timer;

        public Form1()
        {
            InitializeComponent();

            Buffer = new Bitmap(Program.Width, Program.Height);
            BufferGraphics = Graphics.FromImage(Buffer);
            graphics = CreateGraphics();

             //BufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // BufferGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
         //   BufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
           // BufferGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
           
         //   SetStyle(ControlStyles.UserPaint, true);
          //  SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.Load += Form1_Load;
            this.MouseWheel += Form1_MouseWheel;

         //  this.Canvas.Image = Buffer;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            Program.CurrentState.Scroll(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(Buffer, -1, 0);

            base.OnPaint(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer = new Stopwatch();
            Thread t = new Thread(new ThreadStart(Run))
            {
                IsBackground = true
            };

            t.Start();
        }

        private void Run()
        {
            int target = 5;
            long last = 0;
            int delta = 0;

            timer.Start();

            last = timer.ElapsedMilliseconds;

            while (true)
            {
                this.Invalidate();

                delta = (int)(timer.ElapsedMilliseconds - last);
                last = timer.ElapsedMilliseconds;

                Thread.Sleep(target > delta ? target - delta : 1);
            }
        }

        protected  void OnPaintBackground1(PaintEventArgs e)
        {
            // Don't call this.  Just do nothing.

            //base.OnPaintBackground(e);

        }
    }
}
