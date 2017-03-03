using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.RentableSpaces;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class BuyRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

            int itemId = Packet.PopInt();

            Room room;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out room))
                return;

            if (room == null || room.GetRoomItemHandler() == null)
                return;

            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            RentableSpaceItem _rentableSpace;
            if (!PlusEnvironment.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(itemId, out _rentableSpace))
                return;

            int errorCode = PlusEnvironment.GetGame().GetRentableSpaceManager().GetBuyErrorCode(Session, _rentableSpace);
            if (errorCode > 0)
            {
                Session.SendPacket(new RentableSpaceComposer(_rentableSpace.Rented, errorCode, _rentableSpace.OwnerId, _rentableSpace.OwnerUsername, _rentableSpace.GetExpireSeconds(), _rentableSpace.Price));
            }
            else
            {
                if (PlusEnvironment.GetGame().GetRentableSpaceManager().ConfirmBuy(Session, _rentableSpace, 604800))
                {
                    
                    Session.SendPacket(new RentableSpaceComposer(_rentableSpace.OwnerId, _rentableSpace.OwnerUsername, _rentableSpace.GetExpireSeconds()));
                    item.ExtraData = "2";
                    item.UpdateState(false, true);

                }
                else
                {
                    
                    Session.SendPacket(new RentableSpaceComposer(_rentableSpace.Rented, 400, _rentableSpace.OwnerId, _rentableSpace.OwnerUsername, _rentableSpace.GetExpireSeconds(), _rentableSpace.Price));
                }
            }


        }
    }
}
