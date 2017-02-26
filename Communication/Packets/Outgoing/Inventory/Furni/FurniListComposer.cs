using System.Collections.Generic;

using Plus.HabboHotel.Items;
using Plus.HabboHotel.Catalog.Utilities;

namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListComposer : ServerPacket
    {
        public FurniListComposer(ICollection<Item> Items, int pages, int page)
            : base(ServerPacketHeader.FurniListMessageComposer)
        {
            base.WriteInteger(pages);//Pages
            base.WriteInteger(page);//Page?

            base.WriteInteger(Items.Count);
            foreach (Item Item in Items)
            {
                WriteItem(Item);
            }
        }

        private void WriteItem(Item Item)
        {
            base.WriteInteger(Item.Id);
            base.WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.GetBaseItem().SpriteId);

            if (Item.Data.InteractionType != InteractionType.GIFT)
                this.WriteInteger(Item.Data.InteractionType == InteractionType.WALLPAPER ? 2 : Item.Data.InteractionType == InteractionType.FLOOR ? 3 : Item.Data.InteractionType == InteractionType.LANDSCAPE ? 4 : 1);

            Item.Interactor.SerializeExtradata(this, Item);
            if (Item.LimitedNo > 0)
            {
                base.WriteInteger(Item.LimitedNo);
                base.WriteInteger(Item.LimitedTot);
            }

            base.WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
            base.WriteBoolean(Item.GetBaseItem().AllowTrade);
            base.WriteBoolean(Item.LimitedNo == 0 ? Item.GetBaseItem().AllowInventoryStack : false);
            base.WriteBoolean(ItemUtility.IsRare(Item));
            base.WriteInteger(-1);//Seconds to expiration.
            base.WriteBoolean(true);
            base.WriteInteger(-1);//Item RoomId

            if (!Item.IsWallItem)
            {
                base.WriteString(string.Empty);
                base.WriteInteger(0);
            }
        }
    }
}