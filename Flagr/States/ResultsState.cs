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

        private TextLabel scoreLabel;

        public ResultsState()
        {
            scoreLabel = new TextLabel()
            {
                Font = new Font("Arial", 50, FontStyle.Bold),
                DrawMode = DrawMode.Centered,
                Location = new Point(Program.Width / 2, Program.Height / 2),
                Color = Color.White
            };
        }

        public void FinalizeScore()
        {
            scoreLabel.Text = Score + "";
        }

        public override void Update(DeltaTime deltaTime)
        {

            Draw(); 
        }

        private void Draw()
        {
            graphics.FillRectangle(Brushes.Black, 0, 0, Program.Width, Program.Height);

            scoreLabel.Draw(graphics);
        }

    }
}
