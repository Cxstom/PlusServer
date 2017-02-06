using System.Collections.Generic;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CatalogIndexComposer : ServerPacket
    {
        public CatalogIndexComposer(GameClient Session, ICollection<CatalogPage> Pages, int Sub = 0)
            : base(ServerPacketHeader.CatalogIndexMessageComposer)
        {
            WriteRootIndex(Session, Pages);

            foreach (CatalogPage Parent in Pages)
            {
                if (Parent.ParentId != -1 || Parent.MinimumRank > Session.GetHabbo().Rank || (Parent.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                    continue;

                WritePage(Parent, CalcTreeSize(Session, Pages, Parent.Id));

                foreach (CatalogPage Child in Pages)
                {
                    if (Child.ParentId != Parent.Id || Child.MinimumRank > Session.GetHabbo().Rank || (Child.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                        continue;

                    WritePage(Child, CalcTreeSize(Session, Pages, Child.Id));
                    foreach (CatalogPage SubChild in Pages)
                    {
                        if (SubChild.ParentId != Child.Id || SubChild.MinimumRank > Session.GetHabbo().Rank)
                            continue;

                        WritePage(SubChild, 0);
                    }
                }
            }

            base.WriteBoolean(false);
            base.WriteString("NORMAL");
        }

        public void WriteRootIndex(GameClient Session, ICollection<CatalogPage> Pages)
        {
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteString("root");
            base.WriteString(string.Empty);
            base.WriteInteger(0);
            base.WriteInteger(CalcTreeSize(Session, Pages, -1));
        }

        public void WritePage(CatalogPage Page, int TreeSize)
        {
            base.WriteBoolean(Page.Visible);
            base.WriteInteger(Page.Icon);
            base.WriteInteger(Page.Id);
            base.WriteString(Page.PageLink);
            base.WriteString(Page.Caption);

            base.WriteInteger(Page.ItemOffers.Count);
            foreach (int i in Page.ItemOffers.Keys)
            {
                base.WriteInteger(i);
            }

            base.WriteInteger(TreeSize);
        }

        public int CalcTreeSize(GameClient Session, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (CatalogPage Page in Pages)
            {
                if (Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1) || Page.ParentId != ParentId)
                    continue;

                if (Page.ParentId == ParentId)
                    i++;
            }

            return i;
        }
    }
}