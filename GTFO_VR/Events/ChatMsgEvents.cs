using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Events
{
    public static class ChatMsgEvents
    {
        public static Action<string> OnChatMsgReceived;
        public static void ChatMsgReceived(string msg)
        {
            OnChatMsgReceived?.Invoke(msg);
        }
    }
}
