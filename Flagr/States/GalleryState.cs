using Flagr.Flags;
using Flagr.UI;
using System;
using System.Collections.Generic;
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

        private float scrollSpeed = .2f;
        private int scrollY = 0;
        private int maxScrollY = 0;

        public GalleryState()
        {
            flagWidth = Program.Flags.MaxWidth;
            flagHeight = Program.Flags.MaxHeight;

            PopulateContainers();

            maxScrollY = (containers.Length / (Program.Width / flagWidth) - 1) * flagHeight;
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
        }

        public override void Update(DeltaTime deltaTime)
        {

            SetContainerLocations();

            Draw();
        }

        private bool IsVisible(int y)
        {
            return -(flagHeight / 2) <= y && (Program.Height + flagHeight / 2) > y;
        }

        private void SetContainerLocations()
        {
            for (int i = 0; i < containers.Length; i++)
            {
                FlagContainer f = containers[i];

                int x = (i % 3) * flagWidth + (flagWidth / 2);
                int y = (i / 3) * flagHeight + (flagHeight / 2) - scrollY;

                f.Location = new Point(x, y);
                
                if (IsVisible(y) != f.Flag.IsImageLoaded)
                    f.Flag.ToggleImage();              
            }
        }

        private void Draw()
        {
            graphics.FillRectangle(Brushes.White, 0, 0, Program.Width, Program.Height);

            foreach (FlagContainer f in containers)
                f.Draw(graphics);

        }
    }
}
