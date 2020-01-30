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

        private static Thread appThread;
        private static Stopwatch timer;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppForm = new Form1();
            Flags = new FlagManager();
            CurrentState = new StartupState();
            Input = new InputManager(AppForm);

            appThread = new Thread(new ThreadStart(Run))
            {
                IsBackground = true
            };


            appThread.Start();

            Application.Run(AppForm);
        }

        private static void Run()
        {
            timer = new Stopwatch();
            timer.Start();

            long lastTime = timer.ElapsedMilliseconds;

            while (true)
            {
                int deltaTime = (int)(timer.ElapsedMilliseconds - lastTime);

                CurrentState.Update(new DeltaTime(deltaTime)) ;

                lastTime = timer.ElapsedMilliseconds;

                //int sleepTime = (int)(targetTime - timer.ElapsedMilliseconds);

                //Thread.Sleep(sleepTime > 0 ? sleepTime : targetTime);

                Thread.Sleep(15);
            }
        }
    }
}
