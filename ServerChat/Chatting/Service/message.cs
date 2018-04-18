using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace ServerChat.Chatting.Service
{
    class Message : IWebSocketSession
    {
        public WebSocketContext Context { get; }
        public string ID { get; }
        public string Protocol { get; }
        public DateTime StartTime { get; }
        public WebSocketState State { get; }

        private WebSocket _websocket;

        private void start()
        {
            //_websocket.sta
        }
    }
}
