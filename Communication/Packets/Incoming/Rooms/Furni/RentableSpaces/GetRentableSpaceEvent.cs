using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items.RentableSpaces;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class GetRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            Room room;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out room))
                return;

            Item item = room.GetRoomItemHandler().GetItem(ItemId);

            if (item == null)
                return;

            if (item.GetBaseItem() == null)
                return;

            if (item.GetBaseItem().InteractionType != InteractionType.RENTABLE_SPACE)
                return;

            RentableSpaceItem _rentableSpace;
            if (!PlusEnvironment.GetGame().GetRentableSpaceManager().GetRentableSpaceItem(ItemId, out _rentableSpace))
            {
                _rentableSpace = PlusEnvironment.GetGame().GetRentableSpaceManager().CreateAndAddItem(ItemId);
            }

            if (_rentableSpace.Rented)
            {
                Session.SendPacket(new RentableSpaceComposer(_rentableSpace.OwnerId, _rentableSpace.OwnerUsername, _rentableSpace.GetExpireSeconds()));
            }
            else
            {
                int errorCode = PlusEnvironment.GetGame().GetRentableSpaceManager().GetButtonErrorCode(Session, _rentableSpace);
                Session.SendPacket(new RentableSpaceComposer(false, errorCode, -1, "", 0, _rentableSpace.Price));
            }
        }
    }
}
