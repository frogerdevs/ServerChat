using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerChat.Chatting;
using ServerChat.Core;

namespace ServerChat
{
    public class VoiceElementSessionServer
    {
        public void Start()
        {
            lock (SyncVar)
            {
                //if (State == State.Stopped)
                //{
                    //s_State = State.Starting;
                    ThreadStart ts = new ThreadStart(mainThread);
                    s_MainCodeThread = new Thread(ts);
                    s_MainCodeThread.Name = "VoiceElementSessionServer";
                    s_MainCodeThread.Start();
                    WriteLog("VoiceElementSessionServer Starting...");

                //}
                //else
                //{
                //    WriteLog("VoiceElementSessionServer is in the " + State.ToString() + " state.  Cannot start VoiceElementSessionServer at this time.");
                //}
            }
        }

        private object s_SyncVar = new object();

        public object SyncVar
        {
            get { return s_SyncVar; }
        }

        public WebSocketChatListener ChatListener { get; private set; }
        public Thread s_MainCodeThread { get; private set; }
        public DBSession dbSession = new DBSession();
        private int shutdown = 0;
        //public AutoResetEvent ThreadEvent
        //{
        //    get { return s_ThreadEvent; }
        //}

        private void mainThread()
        {
            ChatListener = new WebSocketChatListener(dbSession);  // new WebSocketChatListener(dbSession, s_Log);

        }
        #region MessengerMobile

        public void StartChat()
        {
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion

        public void WriteLog(string message)
        {
            try
            {
                message = string.Format(@"VoiceElementSessionServer :> {0}", message);
                //if (Log != null)
                //    Log.Write(message);//voiceleementserver.log

                //veLog.Write(message);//server.log folder
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:ffffff").PadRight(18) + message);//write on console
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:ffffff").PadRight(18) + "WriteLog Error:" + ex);
            }
        }

        public void StopImmediate()
        {
            lock (SyncVar)
            {
                //if (State == State.Running || State == State.StoppingControlled)
                //{
                    //s_State = State.StoppingImmediate;
                    //ThreadEvent.Set();
                    WriteLog("VoiceElementSessionServer StoppingImmediate.");
                    shutdown = 1;
                    ShutDownVoiceElementServer();
                //}
                //else
                //{
                //    WriteLog("VoiceElementSessionServer is in the " + State.ToString() + " state.  Cannot stop VoiceElementSessionServer at this time.");
                //    shutdown = 0;
                //}
            }
        }

        public void ShutDownVoiceElementServer()
        {
            try
            {
                while (shutdown == 0)
                {
                    Thread.Sleep(3000);
                }

                if (shutdown == 1)
                {
                    //DisconnectOntraceVe();
                    //if (wsListener != null)
                    //    wsListener.Stop();
                    //if (wsConfListener != null)
                    //    wsConfListener.Stop();
                    if(ChatListener!=null)
                        ChatListener.Stop();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
