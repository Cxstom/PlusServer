using System;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Groups;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms
{
    public class RoomData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string OwnerName { get; set; }
        public int OwnerId { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }
        public RoomAccess Access { get; set; }
        public string Type { get; set; }
        public int UsersMax { get; set; }
        public int UsersNow { get; set; }
        public int Category { get; set; }
        public string Description { get; set; }
        public string Floor { get; set; }
        public string Landscape { get; set; }
        public int AllowPets { get; set; }
        public int AllowPetsEating { get; set; }
        public int RoomBlockingEnabled { get; set; }
        public int Hidewall { get; set; }
        public int WallThickness { get; set; }
        public int FloorThickness { get; set; }
        public string Wallpaper { get; set; }
        public int WhoCanMute { get; set; }
        public int WhoCanBan { get; set; }
        public int WhoCanKick { get; set; }
        public int chatMode { get; set; }
        public int chatSize { get; set; }
        public int chatSpeed { get; set; }
        public int extraFlood { get; set; }
        public int chatDistance { get; set; }
        public int TradeSettings { get; set; }//Default = 2;
        public bool PushEnabled { get; set; }
        public bool PullEnabled { get; set; }
        public bool SPushEnabled { get; set; }
        public bool SPullEnabled { get; set; }
        public bool EnablesEnabled { get; set; }
        public bool RespectNotificationsEnabled { get; set; }
        public bool PetMorphsAllowed { get; set; }

        public List<string> Tags;
        private RoomModel mModel;

        private Group _group;
        private RoomPromotion _promotion;
        private List<int> _usersWithRights;
        private List<string> _wordFilterList;

        public RoomData(int id, string caption, string model, string ownerName, int ownerId, string password, int score, string type, string access, int usersNow, int usersMax, int category, string description,
            string tags, string floor, string landscape, int allowPets, int allowPetsEating, int roomBlockingEnabled, int hidewall, int wallThickness, int floorThickness, string wallpaper, int muteSettings,
            int banSettings, int kickSettings, int chatMode, int chatSize, int chatSpeed, int extraFlood, int chatDistance, int tradeSettings, bool pushEnabled, bool pullEnabled, bool superPushEnabled,
            bool superPullEnabled, bool respectedNotificationsEnabled, bool petMorphsAllowed, int groupId)
        {
            this.Id = id;
            this.Name = caption;
            this.ModelName = model;
            this.OwnerName = ownerName;
            this.OwnerId = ownerId;
            this.Password = password;
            this.Score = score;
            this.Type = type;
            this.Access = RoomAccessUtility.ToRoomAccess(access);
            this.UsersNow = usersNow;
            this.UsersMax = usersMax;
            this.Category = category;
            this.Description = description;

            this.Tags = new List<string>();
            foreach (string Tag in tags.ToString().Split(','))
            {
                Tags.Add(Tag);
            }

            this.Floor = floor;
            this.Landscape = landscape;
            this.AllowPets = allowPets;
            this.AllowPetsEating = allowPetsEating;
            this.RoomBlockingEnabled = roomBlockingEnabled;
            this.Hidewall = hidewall;
            this.WallThickness = wallThickness;
            this.FloorThickness = floorThickness;
            this.Wallpaper = wallpaper;
            this.WhoCanMute = muteSettings;
            this.WhoCanBan = banSettings;
            this.WhoCanKick = kickSettings;
            this.chatMode = chatMode;
            this.chatSize = chatSize;
            this.chatSpeed = chatSpeed;
            this.extraFlood = extraFlood;
            this.chatDistance = chatDistance;
            this.TradeSettings = tradeSettings;
            this.PushEnabled = pushEnabled;
            this.PullEnabled = pullEnabled;
            this.SPushEnabled = superPushEnabled;
            this.SPullEnabled = superPushEnabled;
            this.RespectNotificationsEnabled = respectedNotificationsEnabled;
            this.PetMorphsAllowed = petMorphsAllowed;

            if (groupId > 0)
                PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out this._group);

            LoadPromotions();
            LoadRights();
            LoadFilter();
        }

        public List<int> UsersWithRights
        {
            get { return this._usersWithRights; }
            set { this._usersWithRights = value; }
        }

        public List<string> WordFilterList
        {
            get { return this._wordFilterList; }
            set { this._wordFilterList = value; }
        }

        public RoomPromotion Promotion
        {
            get { return this._promotion; }
            set { this._promotion = value; }
        }

        public Group Group
        {
            get { return this._group; }
            set { this._group = value; }
        }

        public RoomData(RoomData data)
        {
            this.Id = data.Id;
            this.Name = data.Name;
            this.ModelName = data.ModelName;
            this.OwnerName = data.OwnerName;
            this.OwnerId = data.OwnerId;
            this.Password = data.Password;
            this.Score = data.Score;
            this.Type = data.Type;
            this.Access = data.Access;
            this.UsersNow = data.UsersNow;
            this.UsersMax = data.UsersMax;
            this.Category = data.Category;
            this.Description = data.Description;
            this.Tags = data.Tags;
            this.Floor = data.Floor;
            this.Landscape = data.Landscape;
            this.AllowPets = data.AllowPets;
            this.AllowPetsEating = data.AllowPetsEating;
            this.RoomBlockingEnabled = data.RoomBlockingEnabled;
            this.Hidewall = data.Hidewall;
            this.WallThickness = data.WallThickness;
            this.FloorThickness = data.FloorThickness;
            this.Wallpaper = data.Wallpaper;
            this.WhoCanMute = data.WhoCanMute;
            this.WhoCanBan = data.WhoCanBan;
            this.WhoCanKick = data.WhoCanKick;
            this.chatMode = data.chatMode;
            this.chatSize = data.chatSize;
            this.chatSpeed = data.chatSpeed;
            this.extraFlood = data.extraFlood;
            this.chatDistance = data.chatDistance;
            this.TradeSettings = data.TradeSettings;
            this.PushEnabled = data.PushEnabled;
            this.PullEnabled = data.PullEnabled;
            this.SPushEnabled = data.SPushEnabled;
            this.SPullEnabled = data.SPullEnabled;
            this.RespectNotificationsEnabled = data.RespectNotificationsEnabled;
            this.PetMorphsAllowed = data.PetMorphsAllowed;
            this.Group = data.Group;
        }

        public void LoadPromotions()
        {
            DataRow GetPromotion = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_promotions` WHERE `room_id` = " + Id + " LIMIT 1;");
                GetPromotion = dbClient.GetRow();

                if (GetPromotion != null)
                {
                    if (Convert.ToDouble(GetPromotion["timestamp_expire"]) > PlusEnvironment.GetUnixTimestamp())
                        this._promotion = new RoomPromotion(Convert.ToString(GetPromotion["title"]), Convert.ToString(GetPromotion["description"]), Convert.ToDouble(GetPromotion["timestamp_start"]), Convert.ToDouble(GetPromotion["timestamp_expire"]), Convert.ToInt32(GetPromotion["category_id"]));
                }
            }
        }

        public void LoadRights()
        {
            UsersWithRights = new List<int>();
            if (Group != null)
                return;

            DataTable Data = null;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT room_rights.user_id FROM room_rights WHERE room_id = @roomid");
                dbClient.AddParameter("roomid", Id);
                Data = dbClient.GetTable();
            }

            if (Data != null)
            {
                foreach (DataRow Row in Data.Rows)
                {
                    UsersWithRights.Add(Convert.ToInt32(Row["user_id"]));
                }
            }
        }

        private void LoadFilter()
        {
            _wordFilterList = new List<string>();

            DataTable Data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_filter` WHERE `room_id` = @roomid;");
                dbClient.AddParameter("roomid", Id);
                Data = dbClient.GetTable();
            }

            if (Data == null)
                return;

            foreach (DataRow Row in Data.Rows)
            {
                _wordFilterList.Add(Convert.ToString(Row["word"]));
            }
        }

        public bool HasActivePromotion
        {
            get { return this.Promotion != null; }
        }

        public void EndPromotion()
        {
            if (!this.HasActivePromotion)
                return;

            this.Promotion = null;
        }

        public RoomModel Model
        {
            get
            {
                if (mModel == null)
                {
                    mModel = PlusEnvironment.GetGame().GetRoomManager().GetModel(ModelName);
                }
                return mModel;
            }
        }
    }
}