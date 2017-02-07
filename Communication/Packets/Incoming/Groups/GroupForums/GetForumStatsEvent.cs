using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetForumStatsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var GroupForumId = Packet.PopInt();

            GroupForum Forum;
            if (!PlusEnvironment.GetGame().GetGroupForumManager().TryGetForum(GroupForumId, out Forum))
            {
                Session.SendNotification("Oops, There was a problem. Contact Staff to resolve this.");
                return;
            }

            Session.SendMessage(new ForumDataComposer(Forum, Session));

        }
    }
}
