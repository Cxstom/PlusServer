using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class ScrSendUserInfoComposer : ServerPacket
    {
        public ScrSendUserInfoComposer()
            : base(ServerPacketHeader.ScrSendUserInfoMessageComposer)
        {
            int DisplayMonths = 0;
            int DisplayDays = 0;


            base.WriteString("club_habbo");
            base.WriteInteger(11);
            base.WriteInteger(18);
            base.WriteInteger(1);
            base.WriteInteger(1);
            base.WriteBoolean(true);
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(576);
            base.WriteInteger(60320);
            base.WriteInteger(13119);


           /* base.WriteString("habbo_club");
            base.WriteInteger(DisplayDays);
            base.WriteInteger(2);
            base.WriteInteger(DisplayMonths);
            base.WriteInteger(1);
            base.WriteBoolean(true); // hc
            base.WriteBoolean(true); // vip
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(495);*/
        }
    }
}
