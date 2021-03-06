﻿using Flagr.Flags;
using Flagr.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flagr.States
{
    class GalleryState : State
    {
        private FlagContainer[] containers;

        private int flagWidth;
        private int flagHeight;

        private float scrollSpeed = 1;
        private int scrollY = 0;
        private int maxScrollY = 0;

        private int firstDrawIndex = 0;
        private int lastDrawIndex = 0;

        private bool drawNameLabel = false;
        private TextLabel NameLabel;
        private float nameLabelFadeTime = .3f;
        private float nameLabelFade = 0f;
        private Point lastMouseLocation = Point.Empty;

        private UI.ScrollBar scrollBar = new UI.ScrollBar();

        private Stopwatch timer;
        private long last = 0;

        public GalleryState()
        {
            flagWidth = Program.Flags.MaxWidth;
            flagHeight = Program.Flags.MaxHeight;

            PopulateContainers();

            maxScrollY = (containers.Length / 3) * flagHeight;

            SetContainerLocations();

            NameLabel = new TextLabel()
            {
                Font = new Font("Arial", 30, FontStyle.Bold),
                Bounds = new Size(flagWidth, flagHeight),
                DrawBackdrop = true,
                BackdropSpacing = 0
            };

            timer = new Stopwatch();
        }
        protected override void KeyPressed(KeyEventArgs e, bool repeating)
        {
            if (e.KeyCode == Keys.Escape)
                Program.SetCurrentState(new TransitionState(this, Program.MainMenu, .4f, .1f, .4f));
        }

        private void PopulateContainers()
        {
            var flags = Program.Flags.GetFlags();

            containers = new FlagContainer[flags.Count];

            for (int i = 0; i < flags.Count; i++)
                containers[i] = new FlagContainer() { Flag = flags[i] };
        }

        public override void Scroll(MouseEventArgs e)
        {
            scrollY -= (int)(e.Delta * scrollSpeed);

            if (scrollY < 0) scrollY = 0;
            if (scrollY > maxScrollY) scrollY = maxScrollY;

            scrollBar.ScrollPosition = (float)((float)scrollY / (float)maxScrollY);
            scrollBar.Popup();

            lastMouseLocation.Y = -1;

            SetContainerLocations();
        }

        public override void Update(DeltaTime deltaTime)
        {
            if (!timer.IsRunning)
                timer.Start();

          //  Console.WriteLine("{0} {1}", (float)((timer.ElapsedMilliseconds - last)/1000f), deltaTime.Seconds);


            UpdateContainers();

            if (drawNameLabel && nameLabelFade < nameLabelFadeTime)
            {
                nameLabelFade += deltaTime.Seconds;

                if (nameLabelFade > nameLabelFadeTime)
                    nameLabelFade = nameLabelFadeTime;
            }

            scrollBar.Update(deltaTime);

           // Console.WriteLine(deltaTime.Milliseconds);
           // Console.WriteLine(nameLabelFade);

            Draw();


            last = timer.ElapsedMilliseconds;
        }

        private bool IsVisible(int y)
        {
            return -(flagHeight / 2) <= y && (Program.Height + flagHeight / 2) >= y;
        }

        private void SetContainerLocations()
        {
            bool firstVis = false;
            lastDrawIndex = -1;

            for (int i = 0; i < containers.Length; i++)
            {
                FlagContainer f = containers[i];

                int x = ((i % 3) * flagWidth) + (flagWidth / 2);
                int y = ((i / 3) * flagHeight) + (flagHeight / 2) - scrollY;

                f.Location = new Point(x, y);

                bool visible = IsVisible(y);

                if(visible != firstVis)
                {
                    if (visible) firstDrawIndex = i;
                    else lastDrawIndex = i-1;

                    firstVis = visible;
                }

                if (visible != f.Flag.IsImageLoaded)
                    f.Flag.ToggleImage();

                f.DrawPlaceholder = visible;
            }

            if (lastDrawIndex < 0)
                lastDrawIndex = containers.Length - 1;
        }

        private void UpdateContainers()
        {
            if (Program.Input.MouseLocation.Equals(lastMouseLocation))
                return;

            lastMouseLocation = Program.Input.MouseLocation;

            Rectangle r = new Rectangle(0, 0, flagWidth, flagHeight);
            bool nameSet = false;

            for (int i = firstDrawIndex; i <= lastDrawIndex; i++)
            {
                FlagContainer f = containers[i];

                r.Location = f.GetOrigin();
                r.Size = f.Flag.ImageSize;

                if (r.Contains(Program.Input.MouseLocation))
                {
                    if (NameLabel.Text != f.Flag.Country)
                        nameLabelFade = 0;

                    NameLabel.SetLocation(f.Location.X, f.Location.Y + (f.Flag.ImageSize.Height/3));
                    NameLabel.Text = f.Flag.Country;

                    nameSet = true;
                }
            }

            drawNameLabel = nameSet;
           
        }

        private void Draw()
        {
            graphics.FillRectangle(Program.BackgroundBrush, 0, 0, Program.Width, Program.Height);

            for (int i = firstDrawIndex; i <= lastDrawIndex; i++)
                containers[i].Draw(graphics);

            scrollBar.Draw(graphics);

            if (drawNameLabel)
            {
                int alpha = nameLabelFade >= nameLabelFadeTime ? 255 : (int)((nameLabelFade / nameLabelFadeTime) * (float)255);            

                NameLabel.Color = Color.FromArgb((NameLabel.Color.ToArgb() & 0x00FFFFFF) | alpha << 24);
                NameLabel.BackdropColor = Color.FromArgb((NameLabel.BackdropColor.ToArgb() & 0x00FFFFFF) | alpha << 24);

                NameLabel.Draw(graphics);
            }
        }
    }
}
