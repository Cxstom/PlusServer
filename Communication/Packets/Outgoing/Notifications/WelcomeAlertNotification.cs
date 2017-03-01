using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Notifications
{
    class WelcomeAlertComposer : ServerPacket
    {
        public WelcomeAlertComposer(string Message)
            : base(ServerPacketHeader.WelcomeAlertComposer)
        {
            base.WriteString(Message);
        }
    }
}