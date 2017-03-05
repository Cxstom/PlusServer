using System.Collections.Generic;

using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class PromotableRoomsComposer : ServerPacket
    {
        public PromotableRoomsComposer(ICollection<RoomData> rooms)
            : base(ServerPacketHeader.PromotableRoomsMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(rooms.Count);//Count

            foreach (RoomData data in rooms)
            {
                base.WriteInteger(data.Id);
                base.WriteString(data.Name);
                base.WriteBoolean(false);
            }
        }
    }
}