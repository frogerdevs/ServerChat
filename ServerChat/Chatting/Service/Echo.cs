using System;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ServerChat.Chatting.Service
{
    public class Echo : WebSocketBehavior
    {
        private string username= String.Empty;
        private string deviceid = String.Empty;
        protected override void OnOpen()
        {
            username = Context.QueryString["username"];
            deviceid = Context.QueryString["deviceid"];

            WriteLog("Connected UserId:" + username + " DeviceId:" + deviceid);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var name = Context.QueryString["username"];
            //Send (!name.IsNullOrEmpty () ? String.Format ("\"{0}\" to {1}", e.Data, name) : e.Data);
            //var ses = Sessions.Sessions.FirstOrDefault(x => x.Context.QueryString["username"] == name);


            if (e.IsText)
            {
                Send(!name.IsNullOrEmpty() ? String.Format("\"{0}\" to {1}", e.Data, name) : e.Data);
                return;
            }
            if (e.IsBinary)
            {
                Send(e.RawData);
                return;
            }
        }


        private void WriteLog(string message)
        {
            System.Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:ffffff").PadRight(18) + message);//write on console
        }

    }
}
