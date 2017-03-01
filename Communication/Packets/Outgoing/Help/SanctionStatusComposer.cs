using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Help
{
    class SanctionStatusComposer : ServerPacket
    {
        public SanctionStatusComposer()
            : base(ServerPacketHeader.SanctionStatusMessageComposer)
        {
            // Very messy stuff here...

            base.WriteBoolean(false); //if true, the last sanction text color will be red.
            base.WriteBoolean(true); // if false, user is on probation time (After 30-60 days being clean, sanction status is fully wiped)

            base.WriteString(""); // Alert Type (ALERT, MUTE, BAN_PERMANENT [empty is BAN])
            base.WriteInteger(6); // Sanction Time in hours (if Time > 24 converted in days)
            base.WriteInteger(0); // idk
            base.WriteString("cfh.reason.EMPTY"); // Sanction Reason (cfh.reason.EMPTY by default means no sanction)
            base.WriteString("2016-11-23"); // Sanction Time Started

            base.WriteInteger(1125); // Probation Time Left in hours (Linked to the second boolean)

            base.WriteString("ALERT"); // Next Sanction (ALERT, MUTE, BAN_PERMANENT, empty is BAN)
            base.WriteInteger(18); // Next Sanction in hours (if Time > 24 converted in days)
            base.WriteInteger(0); // idk

            base.WriteBoolean(false); //if true and second boolean is false = The user is currently muted.
            // if true and second boolean is true = The user is not muted.

            base.WriteString(""); // Trade lock time if user is trade lock, empty means no trade lock
        }
    }
}