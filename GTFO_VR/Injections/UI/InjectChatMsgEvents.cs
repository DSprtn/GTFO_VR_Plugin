using GTFO_VR.Events;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Injections.UI
{
    [HarmonyPatch(typeof(PUI_GameEventLog), nameof(PUI_GameEventLog.AddLogItem))]
    internal class InjectChatMsgEvents
    {
        private static void Prefix(string log, eGameEventChatLogType type)
        {
            ChatMsgEvents.ChatMsgReceived(log);
        }
    }
}
