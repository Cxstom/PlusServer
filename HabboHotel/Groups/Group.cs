using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Groups.Forums;

namespace Plus.HabboHotel.Groups
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdminOnlyDeco { get; set; }
        public string Badge { get; set; }
        public int CreateTime { get; set; }
        public int CreatorId { get; set; }
        public string Description { get; set; }
        public int RoomId { get; set; }
        public int Colour1 { get; set; }
        public int Colour2 { get; set; }
        public bool ForumEnabled { get; set; }
        public GroupType GroupType { get; set; }
        
        private List<int> _members;
        private List<int> _requests;
        private List<int> _administrators;

        private GroupForum _forum;

        public Group(int Id, string Name, string Description, string Badge, int RoomId, int Owner, int Time, int Type, int Colour1, int Colour2, int AdminOnlyDeco, bool forumEnabled)
        {
            this.Id = Id;
            this.Name = Name;
            this.Description = Description;
            this.RoomId = RoomId;
            this.Badge = Badge;
            this.CreateTime = Time;
            this.CreatorId = Owner;
            this.Colour1 = (Colour1 == 0) ? 1 : Colour1;
            this.Colour2 = (Colour2 == 0) ? 1 : Colour2;

            switch (Type)
            {
                case 0:
                    this.GroupType = GroupType.OPEN;
                    break;
                case 1:
                    this.GroupType = GroupType.LOCKED;
                    break;
                case 2:
                    this.GroupType = GroupType.PRIVATE;
                    break;
            }

            this.AdminOnlyDeco = AdminOnlyDeco;
            this.ForumEnabled = forumEnabled;

            this._members = new List<int>();
            this._requests = new List<int>();
            this._administrators = new List<int>();

            if (this.ForumEnabled)
                InitForum(false);

            InitMembers();
        }

        /// <summary>
        /// Used to load the GroupForum, we use this on the initial initialization, and also if a user has just bought a forum for a group.
        /// </summary>
        /// <param name="purchased"></param>
        public void InitForum(bool purchased = false)
        {
            if (purchased)
                this.ForumEnabled = true;

            DataRow row = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `group_forum_settings` WHERE `group_id` = @GroupId LIMIT 1");
                dbClient.AddParameter("GroupId", this.Id);
                row = dbClient.GetRow();
            }

            if (row == null)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("INSERT INTO `group_forum_settings` (`group_id`) VALUES ('" + this.Id + "')");
                    dbClient.SetQuery("SELECT * FROM `group_forum_settings` WHERE `group_id` = @GroupId LIMIT 1");
                    dbClient.AddParameter("GroupId", this.Id);
                    row = dbClient.GetRow();
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `groups` SET `forum_enabled` = '1' WHERE `id` = '" + this.Id + "' LIMIT 1");
            }

            if (row != null)
            {
                this._forum = new GroupForum(this.Id, Convert.ToInt32(row["readability_setting"]), Convert.ToInt32(row["post_creation_setting"]), Convert.ToInt32(row["thread_creation_setting"]), Convert.ToInt32(row["moderation_setting"]), Convert.ToInt32(row["score"]));
            }
        }

        public void InitMembers()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetMembers = null;
                dbClient.SetQuery("SELECT `user_id`, `rank` FROM `group_memberships` WHERE `group_id` = @id");
                dbClient.AddParameter("id", this.Id);
                GetMembers = dbClient.GetTable();

                if (GetMembers != null)
                {
                    foreach (DataRow Row in GetMembers.Rows)
                    {
                        int UserId = Convert.ToInt32(Row["user_id"]);
                        bool IsAdmin = Convert.ToInt32(Row["rank"]) != 0;

                        if (IsAdmin)
                        {
                            if (!this._administrators.Contains(UserId))
                                this._administrators.Add(UserId);
                        }
                        else
                        {
                            if (!this._members.Contains(UserId))
                                this._members.Add(UserId);
                        }
                    }
                }

                DataTable GetRequests = null;
                dbClient.SetQuery("SELECT `user_id` FROM `group_requests` WHERE `group_id` = @id");
                dbClient.AddParameter("id", this.Id);
                GetRequests = dbClient.GetTable();

                if (GetRequests != null)
                {
                    foreach (DataRow Row in GetRequests.Rows)
                    {
                        int UserId = Convert.ToInt32(Row["user_id"]);

                        if (this._members.Contains(UserId) || this._administrators.Contains(UserId))
                        {
                            dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + this.Id + "' AND `user_id` = '" + UserId + "'");
                        }
                        else if (!this._requests.Contains(UserId))
                        {
                            this._requests.Add(UserId);
                        }
                    }
                }
            }
        }

        public List<int> GetMembers
        {
            get { return this._members.ToList(); }
        }

        public List<int> GetRequests
        {
            get { return this._requests.ToList(); }
        }

        public List<int> GetAdministrators
        {
            get { return this._administrators.ToList(); }
        }

        public List<int> GetAllMembers
        {
            get
            {
                List<int> Members = new List<int>(this._administrators.ToList());
                Members.AddRange(this._members.ToList());

                return Members;
            }
        }

        public int MemberCount
        {
            get { return this._members.Count + this._administrators.Count; }
        }

        public int RequestCount
        {
            get { return this._requests.Count; }
        }

        public bool IsMember(int Id)
        {
            return this._members.Contains(Id) || this._administrators.Contains(Id);
        }

        public bool IsAdmin(int Id)
        {
            return this._administrators.Contains(Id);
        }

        public bool HasRequest(int Id)
        {
            return this._requests.Contains(Id);
        }

        public void MakeAdmin(int Id)
        {
            if (this._members.Contains(Id))
                this._members.Remove(Id);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE group_memberships SET `rank` = '1' WHERE `user_id` = @uid AND `group_id` = @gid LIMIT 1");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }

            if (!this._administrators.Contains(Id))
                this._administrators.Add(Id);
        }

        public void TakeAdmin(int UserId)
        {
            if (!this._administrators.Contains(UserId))
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE group_memberships SET `rank` = '0' WHERE user_id = @uid AND group_id = @gid");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", UserId);
                dbClient.RunQuery();
            }

            this._administrators.Remove(UserId);
            this._members.Add(UserId);
        }

        public void AddMember(int Id)
        {
            if (this.IsMember(Id) || this.GroupType == GroupType.LOCKED && this._requests.Contains(Id))
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (this.IsAdmin(Id))
                {
                    dbClient.SetQuery("UPDATE `group_memberships` SET `rank` = '0' WHERE user_id = @uid AND group_id = @gid");
                    this._administrators.Remove(Id);
                    this._members.Add(Id);
                }
                else if (this.GroupType == GroupType.LOCKED)
                {
                    dbClient.SetQuery("INSERT INTO `group_requests` (user_id, group_id) VALUES (@uid, @gid)");
                    this._requests.Add(Id);
                }
                else
                {
                    dbClient.SetQuery("INSERT INTO `group_memberships` (user_id, group_id) VALUES (@uid, @gid)");
                    this._members.Add(Id);
                }

                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }
        }

        public void DeleteMember(int Id)
        {
            if (IsMember(Id))
            {
                if (this._members.Contains(Id))
                    this._members.Remove(Id);
            }
            else if (IsAdmin(Id))
            {
                if (this._administrators.Contains(Id))
                    this._administrators.Remove(Id);
            }
            else
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM group_memberships WHERE user_id=@uid AND group_id=@gid LIMIT 1");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }
        }

        public void HandleRequest(int Id, bool Accepted)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (Accepted)
                {
                    dbClient.SetQuery("INSERT INTO group_memberships (user_id, group_id) VALUES (@uid, @gid)");
                    dbClient.AddParameter("gid", this.Id);
                    dbClient.AddParameter("uid", Id);
                    dbClient.RunQuery();

                    this._members.Add(Id);
                }

                dbClient.SetQuery("DELETE FROM group_requests WHERE user_id=@uid AND group_id=@gid LIMIT 1");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }

            if (this._requests.Contains(Id))
                this._requests.Remove(Id);
        }

        public void ClearRequests()
        {
            this._requests.Clear();
        }

        public GroupForum GetForum()
        {
            return this._forum;
        }

        public void Dispose()
        {
            if (this._forum != null)
            {
                this._forum.Dispose();

                //And finally..
                this._forum = null;
            }
            
            this._requests.Clear();
            this._members.Clear();
            this._administrators.Clear();
        }
    }
}
