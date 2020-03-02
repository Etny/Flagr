using System.Drawing;
using System.Windows.Forms;

namespace Flagr.States
{
    class StartupState : State
    {
        private bool loading = false;

        private readonly string loadingMessage = "Loading...";

        public StartupState() : base() { }

        public override void Update(DeltaTime deltaTime)
        {
            if (!loading)
            {
                loading = true;

                Font loadFont = new Font("Arial", 40, FontStyle.Bold);
                Size textSize = TextRenderer.MeasureText(loadingMessage, loadFont);

                graphics.FillRectangle(Brushes.Black, 0, 0, Program.Width, Program.Height);
                TextRenderer.DrawText(graphics, loadingMessage, loadFont, new Point(Program.Width / 2 - textSize.Width / 2, Program.Height / 2 - textSize.Height / 2), Color.White);

                base.Update(deltaTime);

                Program.Flags.Populate();
            }
            else
            {
                Program.SetCurrentState(new GalleryState());
            }
        }
    }
}
