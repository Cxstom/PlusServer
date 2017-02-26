﻿using System;

using Plus.Utilities;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
     class ObjectUpdateComposer : ServerPacket
    {
         public ObjectUpdateComposer(Item Item, int UserId)
             : base(ServerPacketHeader.ObjectUpdateMessageComposer)
         {
            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.GetBaseItem().SpriteId);
            base.WriteInteger(Item.GetX);
            base.WriteInteger(Item.GetY);
            base.WriteInteger(Item.Rotation);
            base.WriteString(String.Format("{0:0.00}", TextHandling.GetString(Item.GetZ)));
            base.WriteString(String.Empty);

            if (Item.Data.InteractionType != InteractionType.GIFT)
                this.WriteInteger(Item.Data.InteractionType == InteractionType.WALLPAPER ? 2 : Item.Data.InteractionType == InteractionType.FLOOR ? 3 : Item.Data.InteractionType == InteractionType.LANDSCAPE ? 4 : 1);

            Item.Interactor.SerializeExtradata(this, Item);
            if (Item.LimitedNo > 0)
            {
                base.WriteInteger(Item.LimitedNo);
                base.WriteInteger(Item.LimitedTot);
            }

            base.WriteInteger(-1); // to-do: check
            base.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            base.WriteInteger(UserId);
        }
    }
}