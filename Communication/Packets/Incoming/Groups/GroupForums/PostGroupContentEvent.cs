using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class PostGroupContentEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumId = Packet.PopInt();
            var ThreadId = Packet.PopInt();
            var Caption = Packet.PopString();
            var Message = Packet.PopString();

            var Forum = PlusEnvironment.GetGame().GetGroupForumManager().GetForum(ForumId);
            if (Forum == null)
            {
               
                return;
            }

            var IsNewThread = ThreadId == 0;
            if (IsNewThread)
            {
                var Thread = Forum.CreateThread(Session.GetHabbo().Id, Caption);
                var Post = Thread.CreatePost(Session.GetHabbo().Id, Message);

                Session.SendMessage(new ThreadCreatedComposer(Session, Thread));
              

            }
            else
            {
                var Thread = Forum.GetThread(ThreadId);
                if (Thread == null)
                {
                    Session.SendNotification("Opss!.. Forum Thread doesn't exists!");
                    return;
                }

                var Post = Thread.CreatePost(Session.GetHabbo().Id, Message);
                Session.SendMessage(new ThreadReplyComposer(Session, Post));
            }


        }
    }
}
