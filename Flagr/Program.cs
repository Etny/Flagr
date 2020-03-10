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

        private static long lastTime;
        private static DeltaTime deltaTime;

        private static Stopwatch pleaseGodWork;

        [MTAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppForm = new Form1();
            Input = new InputManager(AppForm);
            Flags = new FlagManager();
            CurrentState = new StartupState();
            deltaTime = new DeltaTime();


            //  AppForm.Paint += AppForm_Paint;
            pleaseGodWork = new Stopwatch();
            pleaseGodWork.Start();
            lastTime = 0;

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
            long delta = pleaseGodWork.ElapsedMilliseconds - lastTime;

            CurrentState.Update(deltaTime.Set(delta));

            lastTime = pleaseGodWork.ElapsedMilliseconds;
        }
    }
}
