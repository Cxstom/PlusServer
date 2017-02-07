using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Groups.Forums
{
    public class GroupForumManager
    {
        List<GroupForum> Forums;

        public GroupForumManager()
        {
            Forums = new List<GroupForum>();

        }

        public GroupForum GetForum(int GroupId)
        {
            GroupForum f = null;
            return TryGetForum(GroupId, out f) ? f : null;
        }

        public GroupForum CreateGroupForum(Group Gp)
        {
            GroupForum GF;
            if (TryGetForum(Gp.Id, out GF))
                return GF;

            using (IQueryAdapter adap = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("INSERT INTO group_forums_settings (group_id) VALUES (@gp)");
                adap.AddParameter("gp", Gp.Id);
                adap.RunQuery();

                adap.SetQuery("UPDATE groups SET has_forum = '1' WHERE id = @id");
                adap.AddParameter("id", Gp.Id);
                adap.RunQuery();
            }

            GF = new GroupForum(Gp);
            Forums.Add(GF);
            return GF;
        }

        public bool TryGetForum(int Id, out GroupForum Forum)
        {
            if ((Forum = Forums.FirstOrDefault(c => c.Id == Id)) != null)
                return true;

            Group Gp;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Id, out Gp))
                return false;

            if (!Gp.HasForum)
                return false;

            Forum = new GroupForum(Gp);
            Forums.Add(Forum);
            return true;
        }

        public List<GroupForum> GetForumsByUserId(int Userid)
        {
            GroupForum F;
            return PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Userid).Where(c => TryGetForum(c.Id, out F)).Select(c => GetForum(c.Id)).ToList();
        }
    }
}
