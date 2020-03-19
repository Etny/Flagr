using Flagr.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.States
{
    class SpeedResultsState : State
    {

        public int Score { get; set; } = 0;
        public int Questions { get; set; } = 10;
        public int CorrectAnswers { get; set; } = 0;

        public int Missed = 0;

        private TextLabel scorePositionLabel;
        private TextLabel scoreLabel;
        private TextLabel answersPositionLabel;
        private TextLabel answersLabel;
        private TextLabel missedPositionLabel;
        private TextLabel missedLabel;
        private int labelDistanceX = 150;
        private int labelDistanceY = 150;

        private Button returnButton;

        public SpeedResultsState()
        {
            TextLabel baseLabel = new TextLabel()
            {
                Font = new Font("Arial", 50, FontStyle.Bold)
            };

            scorePositionLabel = baseLabel.Clone();
            scorePositionLabel.Location = new Point(Program.Width / 2 - labelDistanceX, Program.Height / 2 - labelDistanceY);
            scorePositionLabel.Text = "Score:";

            missedPositionLabel = baseLabel.Clone();
            missedPositionLabel.Location = new Point(Program.Width / 2 - labelDistanceX, Program.Height / 2 + labelDistanceY);
            missedPositionLabel.Text = "Missed:";

            answersPositionLabel = baseLabel.Clone();
            answersPositionLabel.Location = new Point(Program.Width / 2 - labelDistanceX, Program.Height / 2);
            answersPositionLabel.Text = "Correct:";

            scoreLabel = baseLabel.Clone();
            scoreLabel.Location = new Point(Program.Width / 2 + labelDistanceX, Program.Height / 2 - labelDistanceY);

            answersLabel = baseLabel.Clone();
            answersLabel.Location = new Point(Program.Width / 2 + labelDistanceX, Program.Height / 2);

            missedLabel = baseLabel.Clone();
            missedLabel.Location = new Point(Program.Width / 2 + labelDistanceX, Program.Height / 2 + labelDistanceY);

            returnButton = new Button(new Point(Program.Width - 170, Program.Height - 50), new Size(250, 50));
            returnButton.Label.Text = "Return to Menu";
            returnButton.DrawMode = DrawMode.Centered;
            returnButton.OnSelect += ReturnPressed;
        }
        public void ReturnPressed(object sender, EventArgs e)
        {
            Program.SetCurrentState(new TransitionState(this, Program.MainMenu, .4f, .1f, .4f));
        }


        public void FinalizeScore()
        {
            scoreLabel.Text = Score + "";

            float correctRatio = ((float)CorrectAnswers / (float)Questions);
            answersLabel.Text = (int)(correctRatio * 100) + "%";

            float missedRatio = ((float)Missed / (float)Questions);
            missedLabel.Text = (int)(missedRatio * 100) + "%";
        }

        public override void Update(DeltaTime deltaTime)
        {
            returnButton.Update(deltaTime);
            
            Draw(); 
        }

        private void Draw()
        {
            graphics.FillRectangle(Program.BackgroundBrush, 0, 0, Program.Width, Program.Height);

            scorePositionLabel.Draw(graphics);
            scoreLabel.Draw(graphics);
            missedPositionLabel.Draw(graphics);
            missedLabel.Draw(graphics);
            answersPositionLabel.Draw(graphics);
            answersLabel.Draw(graphics);

            returnButton.Draw(graphics);
        }


    }
}
