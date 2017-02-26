using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Groups.Forums;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Outgoing.Groups.Forums;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Plus.Communication.Packets.Incoming.Groups.Forums
{
    class DeletePostEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            int threadId = packet.PopInt();
            int postId = packet.PopInt();
            int status = packet.PopInt();

            Group group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out group))
                return;

            if (!session.GetHabbo().GetPermissions().HasRight("group_forum_membership_override"))
            {
                if (group.GetForum().ForumModerationSetting == 2 && !group.IsAdmin(session.GetHabbo().Id) && group.CreatorId != session.GetHabbo().Id)
                {
                    session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("forum.permissions.admin_create_only"));
                    return;
                }
                else if (group.GetForum().ForumModerationSetting == 3 && group.CreatorId != session.GetHabbo().Id)
                {
                    session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("forum.permissions.creator_only"));
                    return;
                }
            }

            GroupThread thread = null;
            if (!group.GetForum().TryGetThread(threadId, out thread))
                return;

            GroupPost post = null;
            if (!thread.TryGetPost(postId, out post))
            {
                //No post was found, are we trying to delete the thread?
                if (postId == threadId)
                {
                    thread.Deleted = !thread.Deleted;
                    thread.Pinned = false;
                    thread.ModeratorId = (thread.Deleted ? session.GetHabbo().Id : 0);

                    // Mark this thread as update required for the task.
                    group.GetForum().UpdateRequired = true;

                    session.SendPacket(new ThreadDataComposer(group, thread, thread.GetPosts().ToList(), 0));
                    session.SendPacket(new RoomNotificationComposer(thread.Deleted ? "forums.message.hidden" : "forums.message.restored"));
                }
                return;
            }

            post.Deleted = !post.Deleted;
            post.ModeratorId = (post.Deleted ? session.GetHabbo().Id : 0);

            // Mark this thread as update required for the task.
            group.GetForum().UpdateRequired = true;

            session.SendPacket(new PostUpdatedComposer(group, post));
            session.SendPacket(new RoomNotificationComposer(post.Deleted ? "forums.message.hidden" : "forums.message.restored"));
        }
    }
}