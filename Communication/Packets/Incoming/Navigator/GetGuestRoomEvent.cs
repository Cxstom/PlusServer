using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class GetGuestRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomId = Packet.PopInt();

            RoomData roomData = null;
            if (!RoomFactory.TryGetData(roomId, out roomData))
                return;

            bool isLoading = Packet.PopInt() == 1;
            bool checkEntry = Packet.PopInt() == 1;

            Session.SendPacket(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));
        }
    }
}
