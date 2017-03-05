﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms
{
    public static class RoomFactory
    {
        public static List<RoomData> GetRoomsDataByOwnerSortByName(int ownerId)
        {
            List<RoomData> data = new List<RoomData>();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `rooms` WHERE `owner` = @ownerid ORDER BY `caption`;");
                dbClient.AddParameter("ownerid", ownerId);
                DataTable getRooms = dbClient.GetTable();

                if (getRooms != null)
                {
                    foreach (DataRow row in getRooms.Rows)
                    {
                        Room room = null;
                        if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Convert.ToInt32(row["id"]), out room))
                        {
                            data.Add(room);
                        }
                        else
                        {
                            RoomModel model = null;
                            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetModel(Convert.ToString(row["model_name"]), out model))
                            {
                                continue;
                            }

                            // TODO: Revise this?
                            string ownerName = "";
                            dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @owner LIMIT 1");
                            dbClient.AddParameter("owner", Convert.ToInt32(row["owner"]));
                            string result = dbClient.GetString();
                            if (!String.IsNullOrEmpty(result))
                                ownerName = result;

                            data.Add(new RoomData(Convert.ToInt32(row["id"]), Convert.ToString(row["caption"]), Convert.ToString(row["model_name"]), ownerName, Convert.ToInt32(row["owner"]),
                                Convert.ToString(row["password"]), Convert.ToInt32(row["score"]), Convert.ToString(row["roomtype"]), Convert.ToString(row["roomtype"]), Convert.ToInt32(row["users_now"]),
                                Convert.ToInt32(row["users_max"]), Convert.ToInt32(row["category"]), Convert.ToString(row["description"]), Convert.ToString(row["tags"]), Convert.ToString(row["floor"]),
                                Convert.ToString(row["landscape"]), Convert.ToInt32(row["allow_pets"]), Convert.ToInt32(row["allow_pets_eat"]), Convert.ToInt32(row["room_blocking_disabled"]), Convert.ToInt32(row["allow_hidewall"]),
                                Convert.ToInt32(row["wallthick"]), Convert.ToInt32(row["floorthick"]), Convert.ToString(row["wallpaper"]), Convert.ToInt32(row["mute_settings"]), Convert.ToInt32(row["ban_settings"]),
                                Convert.ToInt32(row["kick_settings"]), Convert.ToInt32(row["chat_mode"]), Convert.ToInt32(row["chat_size"]), Convert.ToInt32(row["chat_speed"]), Convert.ToInt32(row["chat_extra_flood"]),
                                Convert.ToInt32(row["chat_hearing_distance"]), Convert.ToInt32(row["trade_settings"]), Convert.ToString(row["push_enabled"]) == "1", Convert.ToString(row["pull_enabled"]) == "1",
                                Convert.ToString(row["spush_enabled"]) == "1", Convert.ToString(row["spull_enabled"]) == "1", Convert.ToString(row["respect_notifications_enabled"]) == "1",
                                Convert.ToString(row["pet_morphs_allowed"]) == "1", Convert.ToInt32(row["group_id"])));
                        }
                    }
                }
            }

            return data;
        }

        public static bool TryGetData(int roomId, out RoomData data)
        {
            Room room = null;
            if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out room))
            {
                data = room;
                return true;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `rooms` WHERE `id` = @id LIMIT 1;");
                dbClient.AddParameter("id", roomId);
                DataRow row = dbClient.GetRow();

                if (row != null)
                {
                    RoomModel model = null;
                    if (!PlusEnvironment.GetGame().GetRoomManager().TryGetModel(Convert.ToString(row["model_name"]), out model))
                    {
                        data = null;
                        return false;
                    }

                    // TODO: Revise this?
                    string ownerName = "";
                    dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @owner LIMIT 1");
                    dbClient.AddParameter("owner", Convert.ToInt32(row["owner"]));
                    string result = dbClient.GetString();
                    if (!String.IsNullOrEmpty(result))
                        ownerName = result;

                    data = new RoomData(Convert.ToInt32(row["id"]), Convert.ToString(row["caption"]), Convert.ToString(row["model_name"]), ownerName, Convert.ToInt32(row["owner"]),
                        Convert.ToString(row["password"]), Convert.ToInt32(row["score"]), Convert.ToString(row["roomtype"]), Convert.ToString(row["roomtype"]), Convert.ToInt32(row["users_now"]),
                        Convert.ToInt32(row["users_max"]), Convert.ToInt32(row["category"]), Convert.ToString(row["description"]), Convert.ToString(row["tags"]), Convert.ToString(row["floor"]),
                        Convert.ToString(row["landscape"]), Convert.ToInt32(row["allow_pets"]), Convert.ToInt32(row["allow_pets_eat"]), Convert.ToInt32(row["room_blocking_disabled"]), Convert.ToInt32(row["allow_hidewall"]),
                        Convert.ToInt32(row["wallthick"]), Convert.ToInt32(row["floorthick"]), Convert.ToString(row["wallpaper"]), Convert.ToInt32(row["mute_settings"]), Convert.ToInt32(row["ban_settings"]),
                        Convert.ToInt32(row["kick_settings"]), Convert.ToInt32(row["chat_mode"]), Convert.ToInt32(row["chat_size"]), Convert.ToInt32(row["chat_speed"]), Convert.ToInt32(row["chat_extra_flood"]),
                        Convert.ToInt32(row["chat_hearing_distance"]), Convert.ToInt32(row["trade_settings"]), Convert.ToString(row["push_enabled"]) == "1", Convert.ToString(row["pull_enabled"]) == "1",
                        Convert.ToString(row["spush_enabled"]) == "1", Convert.ToString(row["spull_enabled"]) == "1", Convert.ToString(row["respect_notifications_enabled"]) == "1",
                        Convert.ToString(row["pet_morphs_allowed"]) == "1", Convert.ToInt32(row["group_id"]));
                    return true;
                }
            }

            data = null;
            return false;
        }
    }
}