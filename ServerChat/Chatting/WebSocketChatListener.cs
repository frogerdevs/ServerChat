using System;
using System.Collections.Generic;
using System.Data;
using ServerChat.Chatting.Models;
using ServerChat.Chatting.Service;
using ServerChat.Core;
using WebSocketSharp.Server;

namespace ServerChat.Chatting
{
    public class WebSocketChatListener : WebSocketBehavior
    {
        private DBSession dbsession = new DBSession();
        public DataTable WebSocketDT { get; set; }
        public WebSocketServer wsServer { get; private set; }
        static int ProjectID;

        public Dictionary<string, List<IWebSocketSession>> UsersLogin;
        public Dictionary<string, List<MemberModel>> Conversations;

        public WebSocketChatListener(DBSession dbsession_)
        {
            //WebSocketDT = new DataTable(){ };
            //WebSocketDT.Columns.Add("WebsocketsId", typeof(int));
            //WebSocketDT.Columns.Add("Id", typeof(string));
            //WebSocketDT.Columns.Add("SocketIp", typeof(string));
            //WebSocketDT.Columns.Add("SocketPort", typeof(string));
            //WebSocketDT.Columns.Add("MaxConnections", typeof(int));

            //// Here we add five DataRows.
            //WebSocketDT.Rows.Add(25, "Indocin", "David", DateTime.Now);
            //WebSocketDT.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
            //WebSocketDT.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
            //WebSocketDT.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
            //WebSocketDT.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);

            UsersLogin = new Dictionary<string, List<IWebSocketSession>>();
            Conversations = new Dictionary<string, List<MemberModel>>();

            dbsession = dbsession_;
            ProjectID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ProjectID"]);
            //WebSocketDT = dbsession.GetWebsockets(ProjectID, "CHAT");
            WebSocketDT = dbsession.GetWebsocketsDataTable();
            if (WebSocketDT == null)
            {
                WriteLog("No WebSocketConfListener Defined.");
                return;
            }
            else
            {
                foreach (DataRow row in WebSocketDT.Rows)
                {
                    var SocketIp = row["SocketIp"].ToString();
                    var Socketport = row["SocketPort"].ToString();
                    string url = "ws://" + SocketIp + ":" + Socketport;
                    wsServer = new WebSocketServer(url);
                    //wsServer = new WebSocketServer("ws://192.168.3.55:4649");
                    wsServer.AddWebSocketService<Echo>("/echo");
                    //wsServer.AddWebSocketService<Chat>("/chat");
                    wsServer.AddWebSocketService("/chat", () =>  new Chat(this));
                    wsServer.Start();
                    if (wsServer.IsListening)
                    {
                        Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wsServer.Port);
                        foreach (var path in wsServer.WebSocketServices.Paths)
                            Console.WriteLine("- {0}", path);
                    }
                    //Console.WriteLine("\nPress Enter key to stop the server...");
                    //Console.ReadLine();

                    //wsServer.Stop();
                }
            }
        }

        private void WriteLog(string message)
        {
            //Log.Write(message);//voiceleementserver.log
            //veLog.Write(message);//server.log folder
            System.Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:ffffff").PadRight(18) + message);//write on console
        }
        public void Stop()
        {
            WriteLog("WebSocket(s) stopping...");
            if(wsServer.IsListening)
                wsServer.Stop();
        }
    }
}
