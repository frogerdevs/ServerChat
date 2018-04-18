using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerChat.Chatting.Models
{
    public class ChatModel
    {
        public string ChatId { get; set; } //guid
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public byte?[] Filebyte { get; set; }
        public DateTime MessageTime { get; set; }
        public bool? IsSend { get; set; }
        public bool? IsRead { get; set; }
        public ChatType ChatType { get; set; }
        public string ConversationId { get; set; }
        public MessageStatus? MessageStatus { get; set; }
    }

    public enum MessageStatus
    {
        Send,
        ReceivedInServer,
        ReceivedInDestination,
        Read
    }
}
