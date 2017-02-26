using Plus.HabboHotel.Rooms.Polls;
using Plus.Communication.Packets.Outgoing.Rooms.Polls;

namespace Plus.Communication.Packets.Incoming.Rooms.Polls
{
    class PollStartEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            int pollId = packet.PopInt();

            RoomPoll poll = null;
            if (!PlusEnvironment.GetGame().GetPollManager().TryGetPoll(pollId, out poll))
                return;

            session.SendPacket(new PollContentsComposer(poll));
        }
    }
}
