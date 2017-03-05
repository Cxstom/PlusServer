using System;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Navigator;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class SaveRoomSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            int roomId = packet.PopInt();

            Room room = null;
            if (!PlusEnvironment.GetGame().GetRoomManager().TryLoadRoom(roomId, out room))
                return;

            if (!room.CheckRights(session, true))
                return;

            string Name = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            string Description = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            RoomAccess Access = RoomAccessUtility.ToRoomAccess(packet.PopInt());
            string Password = packet.PopString();
            int MaxUsers = packet.PopInt();
            int CategoryId = packet.PopInt();
            int TagCount = packet.PopInt();

            List<string> Tags = new List<string>();
            StringBuilder formattedTags = new StringBuilder();

            for (int i = 0; i < TagCount; i++)
            {
                if (i > 0)
                {
                    formattedTags.Append(",");
                }

                string tag = packet.PopString().ToLower();

                Tags.Add(tag);
                formattedTags.Append(tag);
            }

            int TradeSettings = packet.PopInt();//2 = All can trade, 1 = owner only, 0 = no trading.
            int AllowPets = Convert.ToInt32(PlusEnvironment.BoolToEnum(packet.PopBoolean()));
            int AllowPetsEat = Convert.ToInt32(PlusEnvironment.BoolToEnum(packet.PopBoolean()));
            int RoomBlockingEnabled = Convert.ToInt32(PlusEnvironment.BoolToEnum(packet.PopBoolean()));
            int Hidewall = Convert.ToInt32(PlusEnvironment.BoolToEnum(packet.PopBoolean()));
            int WallThickness = packet.PopInt();
            int FloorThickness = packet.PopInt();
            int WhoMute = packet.PopInt(); // mute
            int WhoKick = packet.PopInt(); // kick
            int WhoBan = packet.PopInt(); // ban
            int chatMode = packet.PopInt();
            int chatSize = packet.PopInt();
            int chatSpeed = packet.PopInt();
            int chatDistance = packet.PopInt();
            int extraFlood = packet.PopInt();

            if (Name.Length < 1)
                return;

            if (Name.Length > 60)
                Name = Name.Substring(0, 60);

            if (Access == RoomAccess.PASSWORD && Password.Length == 0)
                Access = RoomAccess.OPEN;

            string AccessStr = Password.Length > 0 ? "password" : "open";
            switch (Access)
            {
                default:
                case RoomAccess.OPEN:
                    AccessStr = "open";
                    break;

                case RoomAccess.PASSWORD:
                    AccessStr = "password";
                    break;

                case RoomAccess.DOORBELL:
                    AccessStr = "locked";
                    break;

                case RoomAccess.INVISIBLE:
                    AccessStr = "invisible";
                    break;
            }

            if (MaxUsers < 0)
                MaxUsers = 10;

            if (MaxUsers > 50)
                MaxUsers = 50;

            SearchResultList SearchResultList = null;
            if (!PlusEnvironment.GetGame().GetNavigator().TryGetSearchResultList(CategoryId, out SearchResultList))
                CategoryId = 36;

            if (SearchResultList.CategoryType != NavigatorCategoryType.CATEGORY || SearchResultList.RequiredRank > session.GetHabbo().Rank || (session.GetHabbo().Id != room.OwnerId && session.GetHabbo().Rank >= SearchResultList.RequiredRank))
                CategoryId = 36;

            if (TagCount > 2)
                return;

            if (TradeSettings < 0 || TradeSettings > 2)
                TradeSettings = 0;

            if (AllowPets < 0 || AllowPets > 1)
                AllowPets = 0;

            if (AllowPetsEat < 0 || AllowPetsEat > 1)
                AllowPetsEat = 0;

            if (RoomBlockingEnabled < 0 || RoomBlockingEnabled > 1)
                RoomBlockingEnabled = 0;

            if (Hidewall < 0 || Hidewall > 1)
                Hidewall = 0;

            if (WallThickness < -2 || WallThickness > 1)
                WallThickness = 0;

            if (FloorThickness < -2 || FloorThickness > 1)
                FloorThickness = 0;

            if (WhoMute < 0 || WhoMute > 1)
                WhoMute = 0;

            if (WhoKick < 0 || WhoKick > 1)
                WhoKick = 0;

            if (WhoBan < 0 || WhoBan > 1)
                WhoBan = 0;

            if (chatMode < 0 || chatMode > 1)
                chatMode = 0;

            if (chatSize < 0 || chatSize > 2)
                chatSize = 0;

            if (chatSpeed < 0 || chatSpeed > 2)
                chatSpeed = 0;

            if (chatDistance < 0)
                chatDistance = 1;

            if (chatDistance > 99)
                chatDistance = 100;

            if (extraFlood < 0 || extraFlood > 2)
                extraFlood = 0;

            room.AllowPets = AllowPets;
            room.AllowPetsEating = AllowPetsEat;
            room.RoomBlockingEnabled = RoomBlockingEnabled;
            room.Hidewall = Hidewall;

            room.Name = Name;
            room.Access = Access;
            room.Description = Description;
            room.Category = CategoryId;
            room.Password = Password;

            room.WhoCanBan = WhoBan;
            room.WhoCanKick = WhoKick;
            room.WhoCanMute = WhoMute;

            room.ClearTags();
            room.AddTagRange(Tags);
            room.UsersMax = MaxUsers;

            room.WallThickness = WallThickness;
            room.FloorThickness = FloorThickness;

            room.chatMode = chatMode;
            room.chatSize = chatSize;
            room.chatSpeed = chatSpeed;
            room.chatDistance = chatDistance;
            room.extraFlood = extraFlood;

            room.TradeSettings = TradeSettings;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `rooms` SET `caption` = @caption, `description` = @description, `password` = @password, `category` = @categoryId, `state` = @state, `tags` = @tags, `users_max` = @maxUsers, `allow_pets` = @allowPets, `allow_pets_eat` = @allowPetsEat, `room_blocking_disabled` = @roomBlockingDisabled, `allow_hidewall` = @allowHidewall, `floorthick` = @floorThick, `wallthick` = @wallThick, `mute_settings` = @muteSettings, `kick_settings` = @kickSettings, `ban_settings` = @banSettings, `chat_mode` = @chatMode, `chat_size` = @chatSize, `chat_speed` = @chatSpeed, `chat_extra_flood` = @extraFlood, `chat_hearing_distance` = @chatDistance, `trade_settings` = @tradeSettings WHERE `id` = @roomId LIMIT 1");
                dbClient.AddParameter("caption", room.Name);
                dbClient.AddParameter("description", room.Description);
                dbClient.AddParameter("password", room.Password);
                dbClient.AddParameter("categoryId", CategoryId);
                dbClient.AddParameter("state", AccessStr);
                dbClient.AddParameter("tags", formattedTags.ToString());
                dbClient.AddParameter("maxUsers", MaxUsers);
                dbClient.AddParameter("allowPets", AllowPets.ToString());
                dbClient.AddParameter("allowPetsEat", AllowPetsEat.ToString());
                dbClient.AddParameter("roomBlockingDisabled", RoomBlockingEnabled.ToString());
                dbClient.AddParameter("allowHidewall", room.Hidewall.ToString());
                dbClient.AddParameter("floorThick", room.FloorThickness.ToString());
                dbClient.AddParameter("wallThick", room.WallThickness.ToString());
                dbClient.AddParameter("muteSettings", room.WhoCanMute.ToString());
                dbClient.AddParameter("kickSettings", room.WhoCanKick.ToString());
                dbClient.AddParameter("banSettings", room.WhoCanBan.ToString());
                dbClient.AddParameter("chatMode", room.chatMode.ToString());
                dbClient.AddParameter("chatSize", room.chatSize.ToString());
                dbClient.AddParameter("chatSpeed", room.chatSpeed.ToString());
                dbClient.AddParameter("extraFlood", room.extraFlood.ToString());
                dbClient.AddParameter("chatDistance", room.chatDistance.ToString());
                dbClient.AddParameter("tradeSettings", room.TradeSettings.ToString());
                dbClient.AddParameter("roomId", room.Id);
                dbClient.RunQuery();
            }

            room.GetGameMap().GenerateMaps();

            if (session.GetHabbo().CurrentRoom == null)
            {
                session.SendPacket(new RoomSettingsSavedComposer(room.RoomId));
                session.SendPacket(new RoomInfoUpdatedComposer(room.RoomId));
                session.SendPacket(new RoomVisualizationSettingsComposer(room.WallThickness, room.FloorThickness, PlusEnvironment.EnumToBool(room.Hidewall.ToString())));
            }
            else
            {
                room.SendPacket(new RoomSettingsSavedComposer(room.RoomId));
                room.SendPacket(new RoomInfoUpdatedComposer(room.RoomId));
                room.SendPacket(new RoomVisualizationSettingsComposer(room.WallThickness, room.FloorThickness, PlusEnvironment.EnumToBool(room.Hidewall.ToString())));
            }

            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModDoorModeSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModWalkthroughSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModChatScrollSpeedSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModChatFloodFilterSeen", 1);
            PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModChatHearRangeSeen", 1);
        }
    }
}
