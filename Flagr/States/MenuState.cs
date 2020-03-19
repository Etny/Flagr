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
        private int stateButtonY = (Program.Height / 5) * 2;

        private Button[][] startButtons;
        private int selectedStartButtons = -1;

        private int buttonSpacing = 50;
        private Size stateButtonSize = new Size(Program.Width / 5, 80);
        private Size startButtonSize = new Size(Program.Width / 7, 80);
        private int[] questionCount = { 15, 30, 100 };
        private int[] seconds = { 30, 60, 150 };

        private TextLabel descLabel;
        private String[] descs = { "             Guess what country every flag belongs to. \n Score bonus points by getting streaks of correct answers.", 
                                   "   What country does each boat come from? \n Guess quickly before they leave the screen!",
                                   "Browse through all available flags. \n        (Return by pressing Esc)" };

        private Image logo;

        public MenuState()
        {

            descLabel = new TextLabel()
            {
                Font = new Font("Arial", 25, FontStyle.Bold),
                DrawMode = UI.DrawMode.Centered,
                Location = new Point(Program.Width / 2, stateButtonY + Program.Height / 4),
                Text = ""
            };

            logo = (Image)Properties.Resources.logo;

            PopulateButtons();
        }

        private void PopulateButtons()
        {
            stateButtons = new Button[stateNames.Length];

            for (int i = 0; i < stateButtons.Length; i++)
            {
                Button b = new Button(new Point(Program.Width / 2 + ((i-1) * (buttonSpacing + stateButtonSize.Width)), stateButtonY), stateButtonSize)
                                            { DrawMode = DrawMode.Centered };

                b.Label.Text = stateNames[i];
                b.OnSelect += OnStateButtonPress;
                stateButtons[i] = b;
            }

            startButtons = new Button[3][];



            for(int i = 0; i < 3; i++)
                startButtons[i] = new Button[3];

            String[] quizLenghts = { "Short", "Normal", "Long" };
 
            
            for (int i = 0; i < 3; i++)
            {
                Button b = new Button(new Point(Program.Width / 2 + ((i - 1) * (buttonSpacing + startButtonSize.Width)), (Program.Height / 2) + stateButtonY), startButtonSize);
                b.DrawMode = DrawMode.Centered;
                b.Label.Text = quizLenghts[i];
                b.OnSelect += QuizStartButtonPress;

                startButtons[0][i] = b;
                startButtons[1][i] = b;
            }
            

            Button galleryStart = new Button(new Point(Program.Width / 2, (Program.Height / 2) + stateButtonY), startButtonSize);
            galleryStart.Label.Text = "Start";
            galleryStart.DrawMode = DrawMode.Centered;
            galleryStart.OnSelect += GalleryButtonPress;
            startButtons[2][2] = galleryStart;
        }

        public void OnStateButtonPress(object sender, EventArgs args)
        {
            Button pressed = sender as Button;

            if (pressed == null)
                return;

            for(int i = 0; i < startButtons.Length; i++)
            {
                Button b = stateButtons[i];
                if (b != pressed) b.clickBuildupDecrease = 800;
                else selectedStartButtons = i;
            }

            pressed.clickBuildupDecrease = 0;

            descLabel.Text = descs[selectedStartButtons];
        }

        public void GalleryButtonPress(object sender, EventArgs args)
        {
            Program.SetCurrentState(new TransitionState(this, new GalleryState(), .5f, .2f, .5f));
        }

        public void QuizStartButtonPress(object sender, EventArgs args)
        {
            Button pressed = sender as Button;

            if (pressed == null)
                return;

            int index = -1;

            for (int i = 0; i < startButtons.Length; i++)
            {
                if (startButtons[selectedStartButtons][i] == pressed)
                {
                    index = i;
                    break;
                }
            }

            if(selectedStartButtons == 0)
            {
                QuizState state = new QuizState(questionCount[index]);
                Program.SetCurrentState(new TransitionState(this, state, .5f, .2f, .5f));
            }
            else
            {
                SpeedState state = new SpeedState();
                state.Time = (long)(seconds[index] * 1000);
                Program.SetCurrentState(new TransitionState(this, state, .5f, .2f, .5f));
            }
        }

        public override void Update(DeltaTime deltaTime)
        {
            foreach (Button b in stateButtons)
                b.Update(deltaTime);

            if (selectedStartButtons >= 0)
                foreach (Button b in startButtons[selectedStartButtons])
                    if (b != null) b.Update(deltaTime);

            Draw();
        }

        private void Draw()
        {
            graphics.FillRectangle(Program.BackgroundBrush, 0, 0, Program.Width, Program.Height);

            graphics.DrawImage(logo, Program.Width / 2 - (logo.Width / 2), stateButtonY / 2 - (logo.Height / 2), logo.Width, logo.Height);

            foreach (Button b in stateButtons)
                b.Draw(graphics);

            if (selectedStartButtons >= 0)
                foreach (Button b in startButtons[selectedStartButtons])
                    if (b != null) b.Draw(graphics);

            descLabel.Draw(graphics);
        }

    }
}
