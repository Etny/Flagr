using Flagr.Flags;
using Flagr.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flagr
{
    static class Program
    {

        public static Form1 AppForm;
        public static State CurrentState;
        public static FlagManager Flags;
        public static InputManager Input;

        public static readonly int Width = 1280;
        public static readonly int Height = 720;

        private static Stopwatch timer;
        private static long lastTime;
        private static DeltaTime deltaTime;

        [MTAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppForm = new Form1();
            Input = new InputManager(AppForm);
            Flags = new FlagManager();
            CurrentState = new StartupState();

            timer = new Stopwatch();
            deltaTime = new DeltaTime();

            timer.Start();

            lastTime = timer.ElapsedMilliseconds;

            AppForm.Paint += AppForm_Paint;

            Application.Run(AppForm);
        }

        private static void AppForm_Paint(object sender, PaintEventArgs e)
        {
            Update();
        }

        public static void SetCurrentState(State s)
        {
            CurrentState.SetCurrentState(false);
            CurrentState = s;
            s.SetCurrentState(true);
        }


        public static void Update()
        {
            int delta = (int)(timer.ElapsedMilliseconds - lastTime);

            CurrentState.Update(deltaTime.Set(delta));

            lastTime = timer.ElapsedMilliseconds;
        }
    }
}
