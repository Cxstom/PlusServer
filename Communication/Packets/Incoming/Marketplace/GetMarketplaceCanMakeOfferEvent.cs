using Plus.Communication.Packets.Outgoing.Marketplace;

namespace Plus.Communication.Packets.Incoming.Marketplace
{
    public class GetMarketplaceCanMakeOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            session.SendPacket(new MarketplaceCanMakeOfferResultComposer((session.GetHabbo().TradingLockExpiry > 0 ? 6 : 1)));
        }
    }
}