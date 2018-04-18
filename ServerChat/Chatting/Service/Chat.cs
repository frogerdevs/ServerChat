using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ServerChat.Chatting.Models;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ServerChat.Chatting.Service
{
    public class Chat : WebSocketBehavior
    {
        private string username = String.Empty;
        private string device = String.Empty;
        private string deviceid = String.Empty;
        //public Dictionary<string, List<IWebSocketSession>> UsersLogin;
        private string Tag = "ServerChat.Chatting.Service.Chat";
        private WebSocketChatListener _wsListener;
        public Chat() : this(null)
        {
            //UsersLogin = new Dictionary<string, List<IWebSocketSession>>();
        }
        public Chat(WebSocketChatListener listener)
        {
            //_prefix = !prefix.IsNullOrEmpty() ? prefix : "anon#";
            _wsListener = listener;
        }
        //start
        protected override void OnOpen()
        {
            try
            {

                username = Context.QueryString["username"];
                device = Context.QueryString["device"];
                deviceid = Context.QueryString["deviceid"];

                if (_wsListener.UsersLogin == null)
                {
                    _wsListener.UsersLogin = new Dictionary<string, List<IWebSocketSession>>();
                }
                if (_wsListener.UsersLogin.TryGetValue(username, out var wscollections))
                {
                    var wssession =
                        Sessions.Sessions.FirstOrDefault(x => x.Context.QueryString["username"] == username);
                    if (!wscollections.Contains(wssession))
                        wscollections.Add(wssession);
                }
                else
                {
                    _wsListener.UsersLogin.Add(username,
                        new List<IWebSocketSession>()
                        {
                            Sessions.Sessions.FirstOrDefault(x => x.Context.QueryString["username"] == username)
                        });
                }

                WriteLog(Tag + " - Connected UserId:" + username + " Device:" + device + " DeviceId:" + deviceid);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                WriteLog(Tag + e.Message);
                throw;
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                //var username = Context.QueryString["username"];

                //var ses = Sessions.Sessions.FirstOrDefault(x => x.Context.QueryString["username"] == username);

                //var senderwscollection = UsersLogin[username];

                if(_wsListener.Conversations == null)
                    _wsListener.Conversations = new Dictionary<string, List<MemberModel>>();

                if (e.IsText)
                {
                    WriteLog(Tag + " - Text Message From " + Context.QueryString["username"]);
                    var msg = JsonConvert.DeserializeObject<ChatModel>(e.Data);

                    string convid = String.Empty;

                    switch (msg.ChatType)
                    {
                        case ChatType.Chat:
                            if (string.IsNullOrEmpty(msg.ConversationId))
                            {
                                convid = "conv_" + msg.From + msg.To;
                                msg.ConversationId = convid;
                            }
                            else
                            {
                                convid = msg.ConversationId;
                            }

                            WriteLog(Tag + " - Chat with ChatId : " + convid);

                            SendStatusMessage(msg.From, msg.ChatId, MessageStatus.ReceivedInServer);

                            if (_wsListener.Conversations.TryGetValue(convid, out var userList)) //Existing Conversation
                            {
                                //Get List Member
                                foreach (var memberModel in userList)
                                {
                                    if (memberModel.UserName == msg.To)
                                    {
                                        //Get Socket Collections
                                        if (_wsListener.UsersLogin.TryGetValue(memberModel.UserName, out var wscollections))
                                        {
                                            //Start Send Message To each Socket
                                            foreach (var socketSession in wscollections)
                                            {
                                                msg.ConversationId = convid;
                                                socketSession.Context.WebSocket.Send(JsonConvert.SerializeObject(msg));
                                            }
                                            SendStatusMessage(msg.From, msg.ChatId, MessageStatus.ReceivedInDestination);
                                        }
                                    }
                                    else if (memberModel.UserName == msg.From)
                                    {
                                        //SendStatusMessage(msg.From, msg.ChatId, MessageStatus.ReceivedInServer);
                                    }
                                    ////Get Socket Collections
                                    //if (UsersLogin.TryGetValue(memberModel.UserName, out var wscollections))
                                    //{
                                    //    //Start Send Message To each Socket
                                    //    foreach (var socketSession in wscollections)
                                    //    {
                                    //        msg.ConversationId = convid;
                                    //        socketSession.Context.WebSocket.Send(JsonConvert.SerializeObject(msg));
                                    //    }
                                    //}
                                }
                            }
                            else //New Conversation
                            {
                                var mmbrs = new List<MemberModel>()
                                {
                                    new MemberModel {UserName = msg.From, UserType = MemberType.User},
                                    new MemberModel {UserName = msg.To, UserType = MemberType.User}
                                };
                                _wsListener.Conversations.Add(convid, mmbrs);

                                ////send status message to sender
                                //if (UsersLogin.TryGetValue(msg.From, out var wslist))
                                //{
                                //    //Start Send Message To each Socket
                                //    foreach (var socketSession in wslist)
                                //    {
                                //        var respon = new ChatModel()
                                //        {
                                //            ChatId = msg.ChatId,
                                //            ChatType = ChatType.ChangeStatus,
                                //            MessageStatus = MessageStatus.ReceivedInServer
                                //        };
                                //        var socket = socketSession.Context.WebSocket;
                                //        if (socket.IsAlive)
                                //        {
                                //            socket.Send(JsonConvert.SerializeObject(respon));
                                //        }
                                //        //socketSession.Context.WebSocket.Send(JsonConvert.SerializeObject(res));
                                //    }
                                //}

                                //send message to receiver
                                if (_wsListener.UsersLogin.TryGetValue(msg.To, out var wscol))
                                {
                                    //Start Send Message To each Socket
                                    foreach (var socketSession in wscol)
                                    {                                        
                                        var socket = socketSession.Context.WebSocket;
                                        if (socket.IsAlive)
                                        {
                                            socket.Send(JsonConvert.SerializeObject(msg));
                                        }
                                    }

                                    //Send Status Message
                                    SendStatusMessage(msg.To, msg.ChatId, MessageStatus.ReceivedInDestination);
                                }
                                ////Get List Member
                                //foreach (var memberModel in mmbrs)
                                //{
                                //    //Get Socket Collections
                                //    if (UsersLogin.TryGetValue(memberModel.UserName, out var wscollections))//user online
                                //    {
                                //        //Start Send Message To each Socket
                                //        foreach (var socketSession in wscollections)
                                //        {
                                //            var respon = new ChatModel()
                                //            {
                                //                ChatId = msg.ChatId
                                //            };
                                //            var socket = socketSession.Context.WebSocket;
                                //            if (socket.IsAlive)
                                //            {
                                //                socket.Send(JsonConvert.SerializeObject(respon));
                                //            }
                                //            //socketSession.Context.WebSocket.Send(JsonConvert.SerializeObject(res));
                                //        }
                                //    }
                                //    else //user not online
                                //    {
                                //        //send by GCM with DeviceId
                                //    }
                                //}
                            }
                            break;
                        case ChatType.Bargein:
                            convid = msg.ConversationId;
                            break;
                        case ChatType.Silent:
                            convid = msg.ConversationId;
                            break;
                        case ChatType.Coaching:
                            convid = msg.ConversationId;
                            if (_wsListener.Conversations.TryGetValue(convid, out var users))
                            {
                                users.Add(new MemberModel()
                                {
                                    UserName = Context.QueryString["username"],
                                    UserType = MemberType.Coaching
                                });

                                if (_wsListener.UsersLogin.TryGetValue(msg.From, out var wscol))
                                {
                                    foreach (var socketSession in wscol)
                                    {
                                        var socket = socketSession.Context.WebSocket;
                                        if (socket.IsAlive)
                                        {
                                            socket.Send(JsonConvert.SerializeObject(msg));
                                        }
                                    }
                                }
                            }
                            break;
                        case ChatType.ChangeStatus:
                            SendStatusMessage(msg.From, msg.ChatId, msg.MessageStatus.GetValueOrDefault());
                            break;
                        default:
                            break;
                    }


                   



                    ////Send(!name.IsNullOrEmpty() ? String.Format("\"{0}\" to {1}", e.Data, name) : e.Data);

                    //if (ses == null)
                    //{
                    //    WriteLog("User Not Online");
                    //    return;
                    //}
                    //ses.Context.WebSocket.Send(String.Format("\"{0}\" to {1}", e.Data, name));

                    return;
                }
                if (e.IsBinary)
                {
                    Send(e.RawData);
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

        }

        protected override void OnClose(CloseEventArgs e)
        {
            try
            {

                username = Context.QueryString["username"];
                //UsersLogin.Remove(username);
                if (_wsListener.UsersLogin != null)
                {
                    if (_wsListener.UsersLogin.TryGetValue(username, out var wscollection))
                    {
                        var wssession = wscollection.FirstOrDefault(x => x.Context.QueryString["username"] == username);
                        wscollection.Remove(wssession);
                        if (wscollection.Count == 0)
                        {
                            _wsListener.UsersLogin.Remove(username);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void WriteLog(string message)
        {
            System.Console.WriteLine(DateTime.Now.ToString("HH:mm:ss:ffffff").PadRight(18) +
                                     message); //write on console
        }

        #region Method

        private void SendStatusMessage(string user, string chatid, MessageStatus status)
        {
            if (_wsListener.UsersLogin.TryGetValue(user, out var wscol))
            {
                //Start Send Message To each Socket
                foreach (var socketSession in wscol)
                {
                    var respon = new ChatModel()
                    {
                        ChatId = chatid,
                        ChatType = ChatType.ChangeStatus,
                        MessageStatus = status
                    };
                    var socket = socketSession.Context.WebSocket;
                    if (socket.ReadyState == WebSocketState.Open)
                    {
                        socket.Send(JsonConvert.SerializeObject(respon));
                    }
                    //socketSession.Context.WebSocket.Send(JsonConvert.SerializeObject(res));
                }
            }
            else
            {
                WriteLog("User " + user + " Not Online");
            }
        }
        

        #endregion

    }
}
