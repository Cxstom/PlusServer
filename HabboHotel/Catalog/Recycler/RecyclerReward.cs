using Plus.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Catalog.Recycler
{
    class RecyclerReward
    {
        internal int BaseId;
        internal uint RewardLevel;

        internal RecyclerReward(int BaseId, uint RewardLevel)
        {
            this.BaseId = BaseId;
            this.RewardLevel = RewardLevel;
        }

        internal ItemData GetBaseItem()
        {
            ItemData Item = null;
            if (PlusEnvironment.GetGame().GetItemManager().GetItem(this.BaseId, out Item))
            {
                return Item;
            }
            else
                return null;
        }        
    }
}
