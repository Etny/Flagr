using Flagr.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.States
{
    class ResultsState : State
    {

        public int Score { get; set; } = 0;
        public int Questions { get; set; } = 10;
        public int CorrectAnswers { get; set; } = 0;
        public int ScorePerAnswer { get; set; } = 100;

        private TextLabel scorePositionLabel;
        private TextLabel scoreLabel;
        private TextLabel answersPositionLabel;
        private TextLabel answersLabel;
        private TextLabel gradePositionLabel;
        private TextLabel gradeLabel;
        private int labelDistanceX = 150;
        private int labelDistanceY = 150;

        private Grade[] grades = { Grade.F, Grade.Dminus, Grade.D, Grade.C, Grade.Cplus, Grade.B, Grade.Bplus, Grade.A, Grade.Aplus, Grade.S, Grade.Splus };

        private Grade scoredGrade;

        private Button returnButton;

        public ResultsState()
        {
            TextLabel baseLabel = new TextLabel()
            {
                Font = new Font("Arial", 50, FontStyle.Bold)
            };

            scorePositionLabel = baseLabel.Clone();
            scorePositionLabel.Location = new Point(Program.Width / 2 - labelDistanceX, Program.Height / 2);
            scorePositionLabel.Text = "Score:";

            gradePositionLabel = baseLabel.Clone();
            gradePositionLabel.Location = new Point(Program.Width / 2 - labelDistanceX, Program.Height / 2 + labelDistanceY);
            gradePositionLabel.Text = "Grade:";

            answersPositionLabel = baseLabel.Clone();
            answersPositionLabel.Location = new Point(Program.Width / 2 - labelDistanceX, Program.Height / 2 - labelDistanceY);
            answersPositionLabel.Text = "Correct:";

            scoreLabel = baseLabel.Clone();
            scoreLabel.Location = new Point(Program.Width / 2 + labelDistanceX, Program.Height / 2);

            answersLabel = baseLabel.Clone();
            answersLabel.Location = new Point(Program.Width / 2 + labelDistanceX, Program.Height / 2 - labelDistanceY);

            gradeLabel = baseLabel.Clone();
            gradeLabel.Location = new Point(Program.Width / 2 + labelDistanceX, Program.Height / 2 + labelDistanceY);
            gradeLabel.Font = new Font("Arial", 70, FontStyle.Bold);

            returnButton = new Button(new Point(Program.Width - 170, Program.Height - 50), new Size(250, 50));
            returnButton.Label.Text = "Return to Menu";
            returnButton.DrawMode = DrawMode.Centered;
            returnButton.OnSelect += ReturnPressed;
        }

        public void ReturnPressed(object sender, EventArgs e)
        {
            Program.SetCurrentState(new TransitionState(this, Program.MainMenu, .4f, .1f, .4f));
        }

        private Grade CalculateGrade() 
        {
            if (CorrectAnswers >= Questions) return Grade.Splus;

            //int maxScore = (Questions * 2 * ScorePerAnswer) - (270);
            //float ratio = (float)Score / (float)maxScore;
            float ratio = ((float)CorrectAnswers / (float)Questions);

            Grade currentGrade = Grade.Error;

            foreach(Grade g in grades)
            {
                if (ratio <= g.requiredRatio)
                    break;

                currentGrade = g;
            }

            return currentGrade;
        }

        public void FinalizeScore()
        {
            scoredGrade = CalculateGrade();
            //scoredGrade = grades[index];
            scoreLabel.Text = Score + "";

            gradeLabel.Text = scoredGrade.text;
            gradeLabel.Color = scoredGrade.color;

            float ratio = ((float)CorrectAnswers / (float)Questions);
            answersLabel.Text = (int)(ratio*100) + "%";
        }

        public override void Update(DeltaTime deltaTime)
        {
            /*
            delay -= deltaTime.Seconds;

            if (delay <= 0) { 
                delay = 2;
                index++;
                if (index >= grades.Length) index = 0;
                FinalizeScore();
            }*/

            returnButton.Update(deltaTime);

            Draw(); 
        }

        private void Draw()
        {
            graphics.FillRectangle(Program.BackgroundBrush, 0, 0, Program.Width, Program.Height);

            scorePositionLabel.Draw(graphics);
            scoreLabel.Draw(graphics);
            gradePositionLabel.Draw(graphics);
            gradeLabel.Draw(graphics);
            answersPositionLabel.Draw(graphics);
            answersLabel.Draw(graphics);

            returnButton.Draw(graphics);
        }

        private class Grade
        {
            public static readonly Grade F = new Grade("F", Color.Gray, 0f);
            public static readonly Grade Dminus = new Grade("D-", Color.Purple, .25f);
            public static readonly Grade D = new Grade("D", Color.DarkViolet, .30f);
            public static readonly Grade C = new Grade("C", Color.DarkCyan, .45f);
            public static readonly Grade Cplus = new Grade("C+", Color.Cyan, .6f);
            public static readonly Grade B = new Grade("B", Color.Gold, .65f);
            public static readonly Grade Bplus = new Grade("B+", Color.Yellow, .70f);
            public static readonly Grade A = new Grade("A", Color.LightGreen, .80f);
            public static readonly Grade Aplus = new Grade("A+", Color.Green, .85f);
            public static readonly Grade S = new Grade("S", Color.Orange, .90f);
            public static readonly Grade Splus = new Grade("S+", Color.Red, 1f);

            public static readonly Grade Error = new Grade("Error", Color.LightCoral, 0f);

            public String text;
            public Color color;
            public float requiredRatio;

            public Grade(String text, Color c, float requiredRatio)
            {
                this.text = text;
                this.color = c;
                this.requiredRatio = requiredRatio;
            }
        }

    }
}
