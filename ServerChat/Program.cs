using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerChat
{
    class Program
    {
        public static VoiceElementSessionServer veSessionServer { get; private set; }

        static void Main(string[] args)
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);


            veSessionServer = new VoiceElementSessionServer();
            veSessionServer.Start();

            while (!exitSystem)
            {
                Thread.Sleep(500);
            }
        }

        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;
        static bool exitSystem = false;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            System.Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");
            System.Console.WriteLine("-------------------------------------------------------------------");
            //WebSocketListener.LoggedUserDict.Clear();
            System.Console.WriteLine("--------------WebSocketListener.listLoggedUser.Clear-----------");
            //do your cleanup here
            veSessionServer.StopImmediate();
            Thread.Sleep(3000); //simulate some cleanup delay
            System.Console.WriteLine("Cleanup complete");

            //allow main to run off
            exitSystem = true;

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion
    }
}
