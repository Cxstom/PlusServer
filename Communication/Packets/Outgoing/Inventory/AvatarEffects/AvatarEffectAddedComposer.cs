using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectAddedComposer : ServerPacket
    {
        public AvatarEffectAddedComposer(int spriteId, int duration)
            : base(ServerPacketHeader.AvatarEffectAddedMessageComposer)
        {
            base.WriteInteger(spriteId);
            base.WriteInteger(1); // Types
            base.WriteInteger(duration);
            base.WriteBoolean(false); // Permanent
        }
    }
}
