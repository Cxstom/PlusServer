using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Plus.Core;
using Plus.HabboHotel.GameClients;
using System.Collections.Concurrent;
using Plus.Database.Interfaces;
using log4net;
using System.Threading;

namespace Plus.HabboHotel.Rooms
{
    public class RoomManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Rooms.RoomManager");

        private readonly object _roomLoadingSync;
        private readonly ConcurrentDictionary<int, Room> _rooms;
        private readonly Dictionary<string, RoomModel> _roomModels;

        private DateTime _cycleLastExecution;
        

        public RoomManager()
        {
            this._rooms = new ConcurrentDictionary<int, Room>();
            this._roomModels = new Dictionary<string, RoomModel>();
            this._roomLoadingSync = new object();

            this.LoadModels();

            log.Info("Room Manager -> LOADED");
        }

        public void OnCycle()
        {
            try
            {
                TimeSpan sinceLastTime = DateTime.Now - _cycleLastExecution;
                if (sinceLastTime.TotalMilliseconds >= 500)
                {
                    _cycleLastExecution = DateTime.Now;
                    foreach (Room room in this._rooms.Values.ToList())
                    {
                        if (room.Unloaded || room.isCrashed)
                            continue;

                        if (room.ProcessTask == null || room.ProcessTask.IsCompleted)
                        {
                            room.ProcessTask = new Task(room.ProcessRoom);
                            room.ProcessTask.Start();
                            room.IsLagging = 0;
                        }
                        else
                        {
                            room.IsLagging++;
                            if (room.IsLagging >= 30)
                            {
                                room.isCrashed = true;
                                UnloadRoom(room.Id);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }

        public void UnloadRoom(int roomId)
        {
            Room room = null;
            if (this._rooms.TryRemove(roomId, out room))
            {
                room.Dispose();
            }
        }

        public bool TryLoadRoom(int roomId, out Room instance)
        {
            Room inst = null;
            if (this._rooms.TryGetValue(roomId, out inst))
            {
                if (!inst.Unloaded)
                {
                    instance = inst;
                    return true;
                }

                instance = null;
                return false;
            }

            lock (_roomLoadingSync)
            {
                if (_rooms.TryGetValue(roomId, out inst))
                {
                    if (!inst.Unloaded)
                    {
                        instance = inst;
                        return true;
                    }

                    instance = null;
                    return false;
                }

                RoomData data = null;

                if (!RoomFactory.TryGetData(roomId, out data))
                {
                    instance = null;
                    return false;
                }

                Room myInstance = new Room(data);
                if (this._rooms.TryAdd(roomId, myInstance))
                {
                    instance = myInstance;
                    return true;
                }
                
                instance = null;
                return false;
            }
        }

        public bool TryGetRoom(int roomId, out Room room)
        {
            return this._rooms.TryGetValue(roomId, out room);
        }

        public RoomData CreateRoom(GameClient session, string name, string description, string model, int category, int maxVisitors, int tradeSettings, 
            string wallpaper = "0.0", string floor = "0.0", string landscape = "0.0", int wallthick = 0, int floorthick = 0)
        {
            if (!_roomModels.ContainsKey(model))
            {
                session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("room.creation.model.not_found"));
                return null;
            }

            if (name.Length < 3)
            {
                session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("room.creation.name.too_short"));
                return null;
            }

            int roomId = 0;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `rooms` (`roomtype`,`caption`,`description`,`owner`,`model_name`,`category`,`users_max`,`trade_settings`,`wallpaper`,`floor`,`landscape`,`floorthick`,`wallthick`) VALUES ('private',@caption,@description,@UserId,@model,@category,@usersmax,@tradesettings,@wallpaper,@floor,@landscape,@floorthick,@wallthick)");
                dbClient.AddParameter("caption", name);
                dbClient.AddParameter("description", description);
                dbClient.AddParameter("UserId", session.GetHabbo().Id);
                dbClient.AddParameter("model", model);
                dbClient.AddParameter("category", category);
                dbClient.AddParameter("usersmax", maxVisitors);
                dbClient.AddParameter("tradesettings", tradeSettings);
                dbClient.AddParameter("wallpaper", wallpaper);
                dbClient.AddParameter("floor", floor);
                dbClient.AddParameter("landscape", landscape);
                dbClient.AddParameter("floorthick", floorthick);
                dbClient.AddParameter("wallthick", wallthick);

                roomId = Convert.ToInt32(dbClient.InsertQuery());
            }

            RoomData data = new RoomData(roomId, name, model, session.GetHabbo().Username, session.GetHabbo().Id, "", 0, "public", "open", 0, maxVisitors, category, description, string.Empty,
                floor, landscape, 1, 1, 0, 0, wallthick, floorthick, wallpaper, 1, 1, 1, 1, 1, 1, 1, 8, tradeSettings, true, true, true, true, true, true, 0); 

            return data;
        }

        public void LoadModels()
        {
            if (_roomModels.Count > 0)
                _roomModels.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id,door_x,door_y,door_z,door_dir,heightmap,public_items,club_only,poolmap,`wall_height` FROM `room_models` WHERE `custom` = '0'");
                DataTable Data = dbClient.GetTable();

                if (Data == null)
                    return;

                foreach (DataRow Row in Data.Rows)
                {
                    string Modelname = Convert.ToString(Row["id"]);

                    _roomModels.Add(Modelname, new RoomModel(Convert.ToInt32(Row["door_x"]), Convert.ToInt32(Row["door_y"]), (Double)Row["door_z"], Convert.ToInt32(Row["door_dir"]),
                        Convert.ToString(Row["heightmap"]), Convert.ToString(Row["public_items"]), PlusEnvironment.EnumToBool(Row["club_only"].ToString()), Convert.ToString(Row["poolmap"]), Convert.ToInt32(Row["wall_height"])));
                }
            }
        }

        public void LoadModel(string id)
        {
            DataRow row = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id,door_x,door_y,door_z,door_dir,heightmap,public_items,club_only,poolmap,`wall_height` FROM `room_models` WHERE `custom` = '1' AND `id` = '" + id + "' LIMIT 1");
                row = dbClient.GetRow();

                if (row == null)
                    return;

                string modelname = Convert.ToString(row["id"]);
                if (!this._roomModels.ContainsKey(id))
                {
                    this._roomModels.Add(modelname, new RoomModel(Convert.ToInt32(row["door_x"]), Convert.ToInt32(row["door_y"]), Convert.ToDouble(row["door_z"]), Convert.ToInt32(row["door_dir"]),
                      Convert.ToString(row["heightmap"]), Convert.ToString(row["public_items"]), PlusEnvironment.EnumToBool(row["club_only"].ToString()), Convert.ToString(row["poolmap"]), Convert.ToInt32(row["wall_height"])));
                }
            }
        }

        public void ReloadModel(string id)
        {
            if (!this._roomModels.ContainsKey(id))
            {
                this.LoadModel(id);
                return;
            }

            this._roomModels.Remove(id);
            this.LoadModel(id);
        }

        public bool TryGetModel(string id, out RoomModel model)
        {
            if (this._roomModels.ContainsKey(id))
            {
                model = this._roomModels[id];
                return true;
            }

            // Try to load this model.
            LoadModel(id);

            RoomModel customModel = null;
            if (TryGetModel(id, out customModel))
            {
                model = customModel;
                return true;
            }

            model = null;
            return false;
        }

        public RoomModel GetModel(string model)
        {
            if (this._roomModels.ContainsKey(model))
                return this._roomModels[model];

            // Try to load this model.
            LoadModel(model);

            RoomModel customModel = null;

            return TryGetModel(model, out customModel) ? customModel : null;
        }

        public List<RoomData> SearchGroupRooms(string query)
        {
            IEnumerable<RoomData> InstanceMatches =
                (from RoomInstance in this._rooms
                 where RoomInstance.Value.UsersNow >= 0 &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE &&
                 RoomInstance.Value.Group != null &&
                 (RoomInstance.Value.OwnerName.StartsWith(query) ||
                 RoomInstance.Value.Tags.Contains(query) ||
                 RoomInstance.Value.Name.Contains(query))
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(50);
            return InstanceMatches.ToList();
        }

        public List<RoomData> SearchTaggedRooms(string query)
        {
            IEnumerable<RoomData> InstanceMatches =
                (from RoomInstance in this._rooms
                 where RoomInstance.Value.UsersNow >= 0 &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE &&
                 (RoomInstance.Value.Tags.Contains(query))
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(50);
            return InstanceMatches.ToList();
        }

        public List<RoomData> GetPopularRooms(int category, int amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from RoomInstance in this._rooms
                 where RoomInstance.Value.UsersNow > 0 &&
                 (category == -1 || RoomInstance.Value.Category == category) &&
                 RoomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby RoomInstance.Value.Score descending
                 orderby RoomInstance.Value.UsersNow descending
                 select RoomInstance.Value).Take(amount);
            return rooms.ToList();
        }

        public List<RoomData> GetRecommendedRooms(int amount = 50, int currentRoomId = 0)
        {
            IEnumerable<RoomData> rooms =
                (from roomInstance in this._rooms
                 where roomInstance.Value.UsersNow >= 0 &&
                 roomInstance.Value.Score >= 0 &&
                 roomInstance.Value.Access != RoomAccess.INVISIBLE &&
                 roomInstance.Value.Id != currentRoomId
                 orderby roomInstance.Value.Score descending
                 orderby roomInstance.Value.UsersNow descending
                 select roomInstance.Value).Take(amount);
            return rooms.ToList();
        }

        public List<RoomData> GetRoomsByCategory(int category, int amount = 50)
        {
            IEnumerable<RoomData> rooms =
                (from roomInstance in this._rooms
                 where roomInstance.Value.Category == category &&
                 roomInstance.Value.UsersNow > 0 &&
                 roomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby roomInstance.Value.UsersNow descending
                 select roomInstance.Value).Take(amount);
            return rooms.ToList();
        }

        public List<RoomData> GetOnGoingRoomPromotions(int amount = 50)
        {
            IEnumerable<RoomData> rooms = null;

            rooms =
            (from roomInstance in this._rooms
             where (roomInstance.Value.HasActivePromotion) &&
                      roomInstance.Value.Access != RoomAccess.INVISIBLE
                orderby roomInstance.Value.UsersNow descending
                select roomInstance.Value).Take(amount);

            return rooms.ToList();
        }

        public List<RoomData> GetPromotedRooms(int category, int amount = 50)
        {
            IEnumerable<RoomData> rooms = null;

            rooms =
                (from roomInstance in this._rooms
                 where (roomInstance.Value.HasActivePromotion) &&
                 roomInstance.Value.Promotion.CategoryId == category &&
                 roomInstance.Value.Access != RoomAccess.INVISIBLE
                 orderby roomInstance.Value.Promotion.TimestampStarted descending
                 select roomInstance.Value).Take(amount);

            return rooms.ToList();
        }

        public Room TryGetRandomLoadedRoom()
        {
            IEnumerable<Room> room =
                (from roomInstance in this._rooms
                 where (roomInstance.Value.UsersNow > 0 &&
                 roomInstance.Value.Access == RoomAccess.OPEN &&
                 roomInstance.Value.UsersNow < roomInstance.Value.UsersMax)
                 orderby roomInstance.Value.UsersNow descending
                 select roomInstance.Value).Take(1);

            return room.Any() ? room.First() : null;
        }

        public ICollection<Room> GetRooms()
        {
            return this._rooms.Values;
        }

        public int Count
        {
            get { return this._rooms.Count; }
        }

        public void Dispose()
        {
            int length = _rooms.Count;
            int i = 0;
            foreach (Room room in this._rooms.Values.ToList())
            {
                if (room == null)
                    continue;

                PlusEnvironment.GetGame().GetRoomManager().UnloadRoom(room.Id);
                Console.Clear();
                log.Info("<<- SERVER SHUTDOWN ->> ROOM ITEM SAVE: " + string.Format("{0:0.##}", ((double)i / length) * 100) + "%");
                i++;
            }
            log.Info("Done disposing rooms!");
        }
    }
}