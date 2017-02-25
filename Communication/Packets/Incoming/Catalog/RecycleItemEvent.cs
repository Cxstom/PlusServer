using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Catalog.Recycler;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class RecycleItemEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            int itemCount = Packet.PopInt();

            if (itemCount != 8)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                for (int i = 0; i < itemCount; i++)
                {
                    Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Packet.PopInt());

                    if (Item != null && Item.GetBaseItem().AllowEcotronRecycle)
                    {
                        dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        dbClient.RunQuery("DELETE FROM `user_presents` WHERE `item_id` = '" + Item.Id + "' LIMIT 1");
                        Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);
                    }
                    else
                        return;
                }
            }

            int NewItemId;
            RecyclerReward Reward = PlusEnvironment.GetGame().GetCatalog().GetRandomEcotronReward();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (@baseId, @habboId, @extra_data)");
                dbClient.AddParameter("baseId", 1478);
                dbClient.AddParameter("habboId", (int)Session.GetHabbo().Id);
                dbClient.AddParameter("extra_data", DateTime.Today.ToString("dd/MM/yyyy"));
                NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                //Insert the present, forever.
                dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES (@itemId, @baseId, @extra_data)");
                dbClient.AddParameter("itemId", NewItemId);
                dbClient.AddParameter("baseId", Reward.BaseId);
                dbClient.AddParameter("extra_data", string.Empty);
                dbClient.RunQuery();
            }

            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            Session.SendPacket(new RecyclerStateComposer(NewItemId));
            Session.SendPacket(new FurniListNotificationComposer(Convert.ToInt32(NewItemId), 1));
            Session.SendWhisper("Thank you for using the Recycler! You received a box in your inventory!");
        }
    }
}