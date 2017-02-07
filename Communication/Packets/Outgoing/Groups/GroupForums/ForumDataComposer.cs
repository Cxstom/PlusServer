using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class ForumDataComposer : ServerPacket
    {
        public ForumDataComposer(GroupForum Forum, GameClient Session)
            : base(ServerPacketHeader.ForumDataMessageComposer)
        {
            base.WriteInteger(Forum.Id);
            base.WriteString(Forum.Group.Name); // Groupe Nom
            base.WriteString(Forum.Group.Description); // Déscription
            base.WriteString(Forum.Group.Badge); // Groupe Badge code

            base.WriteInteger(Forum.Threads.Count); // Forum discussion compte
            base.WriteInteger(0); // Autheur ID
            base.WriteInteger(0); //Score ?
            base.WriteInteger(0); // Dernier discussion
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteString("not_member");
            base.WriteInteger(0);

            base.WriteInteger(Forum.Settings.WhoCanRead);
            base.WriteInteger(Forum.Settings.WhoCanPost);
            base.WriteInteger(Forum.Settings.WhoCanInitDiscussions);
            base.WriteInteger(Forum.Settings.WhoCanModerate);

            //Permissions i think
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanRead));
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanPost));
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanInitDiscussions));
            base.WriteString(Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate));
            base.WriteString("-System");

            base.WriteBoolean(Forum.Group.CreatorId == Session.GetHabbo().Id); // Le créateur d'un groupe forums
            base.WriteBoolean(Forum.Group.IsAdmin(Session.GetHabbo().Id) && Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) == ""); // un Administrateur

        }
    }
}
