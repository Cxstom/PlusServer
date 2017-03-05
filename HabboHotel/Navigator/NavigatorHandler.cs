﻿using Plus.Communication.Packets.Outgoing;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Plus.HabboHotel.Navigator
{
    static class NavigatorHandler
    {
        public static void Search(ServerPacket Message, SearchResultList SearchResult, string SearchData, GameClient Session, int FetchLimit)
        {
            //Switching by categorys.
            switch (SearchResult.CategoryType)
            {
                default:
                    Message.WriteInteger(0);
                    break;

                case NavigatorCategoryType.QUERY:
                    {
                        #region Query
                        if (SearchData.ToLower().StartsWith("owner:"))
                        {
                            if (SearchData.Length > 0)
                            {
                                int UserId = 0;
                                DataTable GetRooms = null;
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    if (SearchData.ToLower().StartsWith("owner:"))
                                    {
                                        dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @username LIMIT 1");
                                        dbClient.AddParameter("username", SearchData.Remove(0, 6));
                                        UserId = dbClient.GetInteger();

                                        dbClient.SetQuery("SELECT * FROM `rooms` WHERE `owner` = '" + UserId + "' and `state` != 'invisible' ORDER BY `users_now` DESC LIMIT 50");
                                        GetRooms = dbClient.GetTable();
                                    }
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (GetRooms != null)
                                {
                                    foreach (DataRow Row in GetRooms.Rows)
                                    {
                                        RoomData Data = null;
                                        if (!RoomFactory.TryGetData(Convert.ToInt32(Row["id"]), out Data))
                                            continue;
                                        
                                        if (!Results.Contains(Data))
                                            Results.Add(Data);
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                                }
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("tag:"))
                        {
                            SearchData = SearchData.Remove(0, 4);
                            ICollection<RoomData> TagMatches = PlusEnvironment.GetGame().GetRoomManager().SearchTaggedRooms(SearchData);

                            Message.WriteInteger(TagMatches.Count);
                            foreach (RoomData Data in TagMatches.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                            }
                        }
                        else if (SearchData.ToLower().StartsWith("group:"))
                        {
                            SearchData = SearchData.Remove(0, 6);
                            ICollection<RoomData> GroupRooms = PlusEnvironment.GetGame().GetRoomManager().SearchGroupRooms(SearchData);

                            Message.WriteInteger(GroupRooms.Count);
                            foreach (RoomData Data in GroupRooms.ToList())
                            {
                                RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                            }
                        }
                        else
                        {
                            if (SearchData.Length > 0)
                            {
                                DataTable Table = null;
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT `id`,`caption`,`description`,`roomtype`,`owner`,`state`,`category`,`users_now`,`users_max`,`model_name`,`score`,`allow_pets`,`allow_pets_eat`,`room_blocking_disabled`,`allow_hidewall`,`password`,`wallpaper`,`floor`,`landscape`,`floorthick`,`wallthick`,`mute_settings`,`kick_settings`,`ban_settings`,`chat_mode`,`chat_speed`,`chat_size`,`trade_settings`,`group_id`,`tags`,`push_enabled`,`pull_enabled`,`enables_enabled`,`respect_notifications_enabled`,`pet_morphs_allowed`,`spush_enabled`,`spull_enabled` FROM rooms WHERE `caption` LIKE @query ORDER BY `users_now` DESC LIMIT 50");
                                    dbClient.AddParameter("query", "%" + SearchData + "%");
                                    Table = dbClient.GetTable();
                                }

                                List<RoomData> Results = new List<RoomData>();
                                if (Table != null)
                                {
                                    foreach (DataRow Row in Table.Rows)
                                    {
                                        if (Convert.ToString(Row["state"]) == "invisible")
                                            continue;

                                        RoomData Data = null;
                                        if (!RoomFactory.TryGetData(Convert.ToInt32(Row["id"]), out Data))
                                            continue;
                                        
                                        if (!Results.Contains(Data))
                                            Results.Add(Data);
                                    }
                                }

                                Message.WriteInteger(Results.Count);
                                foreach (RoomData Data in Results.ToList())
                                {
                                    RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                                }
                            }
                        }
                        #endregion

                        break;
                    }

                case NavigatorCategoryType.FEATURED:
                    {
                        #region Featured
                        List<RoomData> Rooms = new List<RoomData>();
                        ICollection<FeaturedRoom> Featured = PlusEnvironment.GetGame().GetNavigator().GetFeaturedRooms(SearchResult.Id);
                        foreach (FeaturedRoom FeaturedItem in Featured.ToList())
                        {
                            if (FeaturedItem == null)
                                continue;

                            RoomData Data = null;
                            if (!RoomFactory.TryGetData(FeaturedItem.RoomId, out Data))
                                continue;
                            
                            if (!Rooms.Contains(Data))
                                Rooms.Add(Data);
                        }

                        Message.WriteInteger(Rooms.Count);
                        foreach (RoomData Data in Rooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        #endregion

                        break;
                    }

                case NavigatorCategoryType.STAFF_PICKS:
                    {
                        #region Featured
                        List<RoomData> rooms = new List<RoomData>();

                        ICollection<StaffPick> picks = PlusEnvironment.GetGame().GetNavigator().GetStaffPicks();
                        foreach (StaffPick pick in picks.ToList())
                        {
                            if (pick == null)
                                continue;

                            RoomData Data = null;
                            if (!RoomFactory.TryGetData(pick.RoomId, out Data))
                                continue;

                            if (!rooms.Contains(Data))
                                rooms.Add(Data);
                        }

                        Message.WriteInteger(rooms.Count);
                        foreach (RoomData data in rooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, data, data.Promotion);
                        }
                        #endregion
                        break;
                    }

                case NavigatorCategoryType.POPULAR:
                    {
                        List<RoomData> PopularRooms = PlusEnvironment.GetGame().GetRoomManager().GetPopularRooms(-1, FetchLimit);

                        Message.WriteInteger(PopularRooms.Count);
                        foreach (RoomData Data in PopularRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.RECOMMENDED:
                    {
                        List<RoomData> RecommendedRooms = PlusEnvironment.GetGame().GetRoomManager().GetRecommendedRooms(FetchLimit);

                        Message.WriteInteger(RecommendedRooms.Count);
                        foreach (RoomData Data in RecommendedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.CATEGORY:
                    {
                        List<RoomData> GetRoomsByCategory = PlusEnvironment.GetGame().GetRoomManager().GetRoomsByCategory(SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetRoomsByCategory.Count);
                        foreach (RoomData Data in GetRoomsByCategory.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.MY_ROOMS:
                    {
                        ICollection<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(Session.GetHabbo().Id);

                        Message.WriteInteger(rooms.Count);
                        foreach (RoomData Data in rooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.MY_FAVORITES:
                    {
                        List<RoomData> Favourites = new List<RoomData>();
                        foreach (int Id in Session.GetHabbo().FavoriteRooms.ToArray())
                        {
                            RoomData Data = null;
                            if (!RoomFactory.TryGetData(Id, out Data))
                                continue;

                            if (!Favourites.Contains(Data))
                                Favourites.Add(Data);
                        }

                        Favourites = Favourites.Take(FetchLimit).ToList();

                        Message.WriteInteger(Favourites.Count);
                        foreach (RoomData Data in Favourites.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.MY_GROUPS:
                    List<RoomData> MyGroups = new List<RoomData>();

                    foreach (Group Group in PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().Id).ToList())
                    {
                        if (Group == null)
                            continue;

                        RoomData Data = null;
                        if (!RoomFactory.TryGetData(Group.RoomId, out Data))
                            continue;

                        if (!MyGroups.Contains(Data))
                            MyGroups.Add(Data);
                    }

                    MyGroups = MyGroups.Take(FetchLimit).ToList();

                    Message.WriteInteger(MyGroups.Count);
                    foreach (RoomData Data in MyGroups.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MY_FRIENDS_ROOMS:
                    List<RoomData> MyFriendsRooms = new List<RoomData>();
                    foreach (MessengerBuddy buddy in Session.GetHabbo().GetMessenger().GetFriends().Where(p => p.InRoom))
                    {
                        if (buddy == null || !buddy.InRoom || buddy.UserId == Session.GetHabbo().Id)
                            continue;

                        if (!MyFriendsRooms.Contains(buddy.CurrentRoom))
                            MyFriendsRooms.Add(buddy.CurrentRoom);
                    }

                    Message.WriteInteger(MyFriendsRooms.Count);
                    foreach (RoomData Data in MyFriendsRooms.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.MY_RIGHTS:
                    List<RoomData> MyRights = new List<RoomData>();

                    DataTable GetRights = null;
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT `room_id` FROM `room_rights` WHERE `user_id` = @UserId LIMIT @FetchLimit");
                        dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                        dbClient.AddParameter("FetchLimit", FetchLimit);
                        GetRights = dbClient.GetTable();

                        foreach (DataRow Row in GetRights.Rows)
                        {
                            RoomData Data = null;
                            if (!RoomFactory.TryGetData(Convert.ToInt32(Row["room_id"]), out Data))
                                continue;

                            if (!MyRights.Contains(Data))
                                MyRights.Add(Data);
                        }
                    }

                    Message.WriteInteger(MyRights.Count);
                    foreach (RoomData Data in MyRights.ToList())
                    {
                        RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                    }
                    break;

                case NavigatorCategoryType.TOP_PROMOTIONS:
                    {
                        List<RoomData> GetPopularPromotions = PlusEnvironment.GetGame().GetRoomManager().GetOnGoingRoomPromotions(FetchLimit);

                        Message.WriteInteger(GetPopularPromotions.Count);
                        foreach (RoomData Data in GetPopularPromotions.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }

                case NavigatorCategoryType.PROMOTION_CATEGORY:
                    {
                        List<RoomData> GetPromotedRooms = PlusEnvironment.GetGame().GetRoomManager().GetPromotedRooms(SearchResult.Id, FetchLimit);

                        Message.WriteInteger(GetPromotedRooms.Count);
                        foreach (RoomData Data in GetPromotedRooms.ToList())
                        {
                            RoomAppender.WriteRoom(Message, Data, Data.Promotion);
                        }
                        break;
                    }
            }
        }
    }
}