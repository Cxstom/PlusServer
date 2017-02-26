using System.Linq;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items.Wired;
using Plus.HabboHotel.Rooms.Polls;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Rooms.Polls;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class GetRoomEntryDataEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;

            Room Room = session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (session.GetHabbo().InRoom)
            {
                Room OldRoom;

                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(session, false, false);
            }

            if (!Room.GetRoomUserManager().AddAvatarToRoom(session))
            {
                Room.GetRoomUserManager().RemoveUserFromRoom(session, false, false);
                return;//TODO: Remove?
            }

            Room.SendObjects(session);

            //Status updating for messenger, do later as buggy.

            try
            {
                if (session.GetHabbo().GetMessenger() != null)
                    session.GetHabbo().GetMessenger().OnStatusChanged(true);
            }
            catch { }

            if (session.GetHabbo().GetStats().QuestID > 0)
                PlusEnvironment.GetGame().GetQuestManager().QuestReminder(session, session.GetHabbo().GetStats().QuestID);

            session.SendPacket(new RoomEntryInfoComposer(Room.RoomId, Room.CheckRights(session, true)));
            session.SendPacket(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, PlusEnvironment.EnumToBool(Room.Hidewall.ToString())));

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Username);

            if (ThisUser != null && session.GetHabbo().PetId == 0)
                Room.SendPacket(new UserChangeComposer(ThisUser, false));

            session.SendPacket(new RoomEventComposer(Room.RoomData, Room.RoomData.Promotion));

            if (Room.GetWired() != null)
                Room.GetWired().TriggerEvent(WiredBoxType.TriggerRoomEnter, session.GetHabbo());

            RoomPoll poll = null;
            if (PlusEnvironment.GetGame().GetPollManager().TryGetPollForRoom(Room.Id, out poll) && poll.Type == RoomPollType.Poll)
            {
                if (!session.GetHabbo().GetPolls().CompletedPolls.Contains(poll.Id))
                    session.SendPacket(new PollOfferComposer(poll));
            }

            if (PlusEnvironment.GetUnixTimestamp() < session.GetHabbo().FloodTime && session.GetHabbo().FloodTime != 0)
                session.SendPacket(new FloodControlComposer((int)session.GetHabbo().FloodTime - (int)PlusEnvironment.GetUnixTimestamp()));
        }
    }
}