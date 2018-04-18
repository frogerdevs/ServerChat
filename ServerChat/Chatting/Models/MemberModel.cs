using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerChat.Chatting.Models
{
    public class MemberModel
    {
        public string UserName { get; set; }
        public MemberType UserType { get; set; }
    }

    public enum MemberType
    {
        User,
        Customer,
        Silent,
        Bargein,
        Coaching
    }
}
