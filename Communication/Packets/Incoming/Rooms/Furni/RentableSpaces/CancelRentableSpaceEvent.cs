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
    class CancelRentableSpaceEvent : IPacketEvent
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

            int errorCode = PlusEnvironment.GetGame().GetRentableSpaceManager().GetCancelErrorCode(Session, _rentableSpace);

            if (errorCode > 0)
            {
                Session.SendPacket(new RentableSpaceComposer(_rentableSpace.IsRented(), errorCode, _rentableSpace.OwnerId, _rentableSpace.OwnerUsername, _rentableSpace.GetExpireSeconds(), _rentableSpace.Price));
                return;
            }


            if (!PlusEnvironment.GetGame().GetRentableSpaceManager().ConfirmCancel(Session, _rentableSpace))
            {
                Session.SendNotification("global.error");
                return;
            }

            Session.SendPacket(new RentableSpaceComposer(false, 0, 0, "", 0, _rentableSpace.Price));
        }
    }
}
