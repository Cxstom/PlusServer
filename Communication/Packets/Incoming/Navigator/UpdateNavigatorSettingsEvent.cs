using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Navigator;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class UpdateNavigatorSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomId = Packet.PopInt();
            if (roomId == 0)
                return;

            RoomData Data = null;
            if (!RoomFactory.TryGetData(roomId, out Data))
                return;

            Session.GetHabbo().HomeRoom = roomId;
            Session.SendPacket(new NavigatorSettingsComposer(roomId));
        }
    }
}
