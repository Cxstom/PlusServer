using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using MoreLinq;
using Plus.HabboHotel.Users.Messenger;
using Plus.Communication.Packets.Outgoing.Messenger;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class GetBuddyRequestsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<MessengerRequest> Requests = Session.GetHabbo().GetMessenger().GetRequests().ToList();
            
            if (Requests.Count() == 0)
            {
                Session.SendMessage(new BuddyRequestsComposer(Requests, 0));
            }
            else
            {
                int page = 0;
                foreach (ICollection<MessengerBuddy> batch in Requests.Batch(700))
                {
                    Session.SendMessage(new BuddyRequestsComposer(Requests, page));
                    page++;
                }
            }
        }
    }
}
