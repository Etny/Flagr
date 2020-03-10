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

        public static Stopwatch timer;

        public Form1()
        {
            InitializeComponent();

            Buffer = new Bitmap(Program.Width, Program.Height);
            BufferGraphics = Graphics.FromImage(Buffer);
       
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
            Program.Update();

            e.Graphics.DrawImageUnscaled(Buffer, 0, 0);

          //  base.OnPaint(e);
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
            long last;
            int delta;

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
