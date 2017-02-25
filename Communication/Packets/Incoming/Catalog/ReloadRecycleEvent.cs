using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Catalog.Recycler;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class ReloadRecycleEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new ReloadRecyclerComposer());
        }
    }
}