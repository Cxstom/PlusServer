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

                return;

                return;

            if (errorCode > 0)
            {
            }
            else
            {
                {
                    

                }
                else
                {
                    
                }
            }


        }
    }
}
