using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class GetPromotableRoomsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            List<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(Session.GetHabbo().Id);

            rooms = rooms.Where(x => (x.Promotion == null || x.Promotion.TimestampExpires < PlusEnvironment.GetUnixTimestamp())).ToList();
            Session.SendPacket(new PromotableRoomsComposer(rooms));
        }
    }
}
