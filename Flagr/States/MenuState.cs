using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagr.UI;

namespace Flagr.States
{
    class MenuState : State
    {

        private Button[] stateButtons;
        private String[] stateNames = { "Quiz", "Boat Quiz ", "Gallery" };
        private int buttonY = (Program.Height / 5) * 2;
        private int buttonSpacing = 50;
        private Size buttonSize = new Size(Program.Width / 5, 80);

         public MenuState()
        {

            PopulateButtons();
        }

        private void PopulateButtons()
        {
            stateButtons = new Button[stateNames.Length];

            for (int i = 0; i < stateButtons.Length; i++)
            {
                Button b = new Button(new Point(Program.Width / 2 + ((i-1) * (buttonSpacing + buttonSize.Width)), buttonY),
                                             buttonSize)
                                            { DrawMode = DrawMode.Centered };

                b.Label.Text = stateNames[i];
                b.clickBuildupDecrease = 0;
                stateButtons[i] = b;
            }
        }

        public override void Update(DeltaTime deltaTime)
        {
            foreach (Button b in stateButtons)
                b.Update(deltaTime);

            Draw();
        }

        private void Draw()
        {
            graphics.FillRectangle(Program.BackgroundBrush, 0, 0, Program.Width, Program.Height);

            foreach (Button b in stateButtons)
                b.Draw(graphics);
        }

    }
}
