using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomInfoComposer : ServerPacket
    {
        public ModeratorRoomInfoComposer(RoomData data, bool ownerInRoom)
            : base(ServerPacketHeader.ModeratorRoomInfoMessageComposer)
        {
            base.WriteInteger(data.Id);
            base.WriteInteger(data.UsersNow);
            base.WriteBoolean(ownerInRoom); // owner in room
            base.WriteInteger(data.OwnerId);
            base.WriteString(data.OwnerName);
            base.WriteBoolean(data != null);
            base.WriteString(data.Name);
            base.WriteString(data.Description);

            base.WriteInteger(data.Tags.Count);
            foreach (string tag in data.Tags)
            {
                base.WriteString(tag);
            }

            base.WriteBoolean(false);
        }
    }
}
