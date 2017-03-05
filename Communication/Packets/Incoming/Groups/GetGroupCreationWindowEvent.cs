using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Groups;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class GetGroupCreationWindowEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            ICollection<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(session.GetHabbo().Id);
            foreach (RoomData Data in rooms.ToList())
            {
                if (Data.Group != null)
                    rooms.Remove(Data);
            }

            session.SendPacket(new GroupCreationWindowComposer(rooms));
        }
    }
}
