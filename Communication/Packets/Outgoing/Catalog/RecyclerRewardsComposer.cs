using System;
using System.Collections.Generic;

using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.Catalog.Recycler;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class RecyclerRewardsComposer : ServerPacket
    {
        public RecyclerRewardsComposer()
            : base(ServerPacketHeader.RecyclerRewardsMessageComposer)
        {
            List<int> Levels = PlusEnvironment.GetGame().GetCatalog().GetEcotronRewardsLevels();
            base.WriteInteger(Levels.Count); // Count of items
            foreach (int l in Levels)
            {
                base.WriteInteger(l); // category
                base.WriteInteger(l); // rarity
                List<RecyclerReward> Rewards = PlusEnvironment.GetGame().GetCatalog().GetEcotronRewardsForLevel(uint.Parse(l.ToString()));
                base.WriteInteger(Rewards.Count); // Count in category
                foreach (RecyclerReward R in Rewards)
                {
                    base.WriteString(R.GetBaseItem().PublicName); // sprite name
                    base.WriteInteger(1); // ?
                    base.WriteString(R.GetBaseItem().Type.ToString()); // type
                    base.WriteInteger(R.GetBaseItem().SpriteId); // sprite id
                }
            }
        }
    }
}