using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetThreadDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumId = Packet.PopInt(); //Maybe Forum ID
            var ThreadId = Packet.PopInt(); //Maybe Thread ID
            var StartIndex = Packet.PopInt(); //Start index
            var length = Packet.PopInt(); //List Length

            var Forum = PlusEnvironment.GetGame().GetGroupForumManager().GetForum(ForumId);

            if (Forum == null)
            {
                return;
            }

            var Thread = Forum.GetThread(ThreadId);
            if (Thread == null)
            {
                Session.SendNotification("Forum discussion introuvable!");
                return;
            }


            Session.SendMessage(new ThreadDataComposer(Thread, StartIndex, length));

        }
    }
}
