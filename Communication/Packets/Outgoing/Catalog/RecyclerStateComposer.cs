using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class RecyclerStateComposer : ServerPacket
    {
        public RecyclerStateComposer(int ItemId = 0)
            : base(ServerPacketHeader.RecyclerStateComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(ItemId);
        }
    }
}