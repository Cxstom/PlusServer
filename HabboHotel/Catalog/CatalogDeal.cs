using System.Collections.Generic;

using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Catalog
{
    public class CatalogDeal
    {
        public int Id { get; set; }
        public List<CatalogItem> ItemDataList { get; private set; }
        public string DisplayName { get; set; }

        public CatalogDeal(int Id, string Items, string DisplayName, ItemDataManager ItemDataManager)
        {
            this.Id = Id;
            this.DisplayName = DisplayName;
            this.ItemDataList = new List<CatalogItem>();

            string[] SplitItems = Items.Split(';');
            foreach (string Split in SplitItems)
            {
                string[] Item = Split.Split('*');
                int ItemId = 0;
                int Amount = 0;
                if (!int.TryParse(Item[0], out ItemId) || !int.TryParse(Item[1], out Amount))
                    continue;

                ItemData Data = null;
                if (!ItemDataManager.GetItem(ItemId, out Data))
                    continue;

                ItemDataList.Add(new CatalogItem(0, ItemId, Data, string.Empty, 0, 0, 0, 0, Amount, 0, 0, false, "", "", 0));
            }
        }
    }
}
