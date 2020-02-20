using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagr.States
{
    class TransitionState : State
    {
        public State Current { get; protected set; }
        public State New { get; protected set; }
        public float FadeOutDuration { get; protected set; }
        public float PauseDuration { get; protected set; }
        public float FadeInDuration { get; protected set; }
        public FadeStage CurrentFadeStage { get; protected set; } = FadeStage.FadeOut;

        private float fadeTime = 0;

        public TransitionState(State Current, State New, float FadeOutDuration, float PauseDuration, float FadeInDuration)
        {
            this.Current = Current;
            this.New = New;
            this.FadeOutDuration = FadeOutDuration;
            this.PauseDuration = PauseDuration;
            this.FadeInDuration = FadeInDuration;
        }

        public override void Update(DeltaTime deltaTime)
        {
            fadeTime += deltaTime.Seconds;
            float alpha;
            Color c;

            switch (CurrentFadeStage)
            {
                case FadeStage.FadeOut:

                    Current.Update(deltaTime);

                    alpha = ((fadeTime / FadeOutDuration) * 255f);

                    if (alpha < 0 || alpha > 255) alpha = 255;

                    c = Color.FromArgb((int)alpha, 0, 0, 0);

                    graphics.FillRectangle(new SolidBrush(c), 0, 0, Program.Width, Program.Height);

                    if(fadeTime >= FadeOutDuration)
                    {
                        fadeTime = 0;
                        if (PauseDuration > 0) CurrentFadeStage = FadeStage.Pause;
                        else CurrentFadeStage = FadeStage.FadeIn;
                    }                 

                    break;

                case FadeStage.Pause:

                    graphics.FillRectangle(Brushes.Black, 0, 0, Program.Width, Program.Height);

                    if (fadeTime >= PauseDuration)
                    {
                        fadeTime = 0;
                        CurrentFadeStage = FadeStage.FadeIn;
                    }

                    break;

                case FadeStage.FadeIn:

                    New.Update(deltaTime);

                    alpha = ((1f - (fadeTime / FadeInDuration)) * 255f);

                    if (alpha < 0 || alpha > 255) alpha = 0;

                    c = Color.FromArgb((int)alpha, 0, 0, 0);

                    graphics.FillRectangle(new SolidBrush(c), 0, 0, Program.Width, Program.Height);

                    if (fadeTime >= FadeInDuration)
                        Program.SetCurrentState(New);

                    break;
            }
        }

        public enum FadeStage
        {
            FadeOut, Pause, FadeIn
        }

    }
}
