﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flagr
{
    public partial class Form1 : Form
    {

        public static Bitmap Buffer;
        public static Graphics BufferGraphics;
        private Graphics graphics;

        public Form1()
        {
            InitializeComponent();

            Buffer = new Bitmap(Program.Width, Program.Height);
            BufferGraphics = Graphics.FromImage(Buffer);
            graphics = CreateGraphics();

            // BufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // BufferGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            BufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            BufferGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.Load += Form1_Load;

         //  this.Canvas.Image = Buffer;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(Buffer, -1, 0);

            base.OnPaint(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = (5);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        protected  void OnPaintBackground1(PaintEventArgs e)
        {
            // Don't call this.  Just do nothing.

            //base.OnPaintBackground(e);

        }
    }
}
