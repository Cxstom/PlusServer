﻿using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class ItemsComposer : ServerPacket
    {
        public ItemsComposer(Item[] Objects, Room Room)
            : base(ServerPacketHeader.ItemsMessageComposer)
        {

            base.WriteInteger(1);
            base.WriteInteger(Room.OwnerId);
            base.WriteString(Room.OwnerName);

            base.WriteInteger(Objects.Length);

            foreach (Item Item in Objects)
            {
                WriteWallItem(Item, Room.OwnerId);
            }
        }

        private void WriteWallItem(Item Item, int UserId)
        {
            base.WriteString(Item.Id.ToString());
            base.WriteInteger(Item.Data.SpriteId);

            try
            {
               base.WriteString(Item.wallCoord);
            }
            catch
            {
               base.WriteString("");
            }

            base.WriteString(Item.GetBaseItem().InteractionType == InteractionType.POSTIT ? Item.ExtraData.Split(' ')[0] : Item.ExtraData);
            base.WriteInteger(-1);
            base.WriteInteger((Item.Data.Modes > 1) ? 1 : 0);
            base.WriteInteger(UserId);
        }
    }
}
