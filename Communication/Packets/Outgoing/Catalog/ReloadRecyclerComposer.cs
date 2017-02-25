using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    class ReloadRecyclerComposer : ServerPacket
    {
        public ReloadRecyclerComposer()
            : base(ServerPacketHeader.ReloadRecyclerComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(0);
        }
    }
}