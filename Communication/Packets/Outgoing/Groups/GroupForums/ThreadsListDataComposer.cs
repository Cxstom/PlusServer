using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class ThreadsListDataComposer : ServerPacket
    {
        public ThreadsListDataComposer(GroupForum Forum, GameClient Session, int StartIndex = 0, int MaxLength = 20)
            : base(ServerPacketHeader.ThreadsListDataMessageComposer)
        {
            base.WriteInteger(Forum.GroupId);
            base.WriteInteger(StartIndex);

            var Threads = Forum.Threads;
            if (Threads.Count - 1 >= StartIndex)
                Threads = Threads.GetRange(StartIndex, Math.Min(MaxLength, Threads.Count - StartIndex));

            base.WriteInteger(Threads.Count);

            foreach (var Thread in Threads)
            {
                Thread.SerializeData(Session, this);
            }
        }
    }
}

