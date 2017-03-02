using System.Collections.Generic;

using Plus.HabboHotel.Users.Currency.Type;

namespace Plus.Communication.Packets.Outgoing.Inventory.Purse
{
    class ActivityPointsComposer : ServerPacket
    {
        public ActivityPointsComposer(ICollection<CurrencyType> currencies)
            : base(ServerPacketHeader.ActivityPointsMessageComposer)
        {
            base.WriteInteger(currencies.Count);
            foreach (CurrencyType currency in currencies)
            {
                base.WriteInteger(currency.Type);
                base.WriteInteger(currency.Amount);
            }
        }
    }
}
