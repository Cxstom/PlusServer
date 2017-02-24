using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class GetClientVersionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Build = Packet.PopString();

            if (Build != PlusEnvironment.SWFRevision)
                Session.Disconnect();
        }
    }
}