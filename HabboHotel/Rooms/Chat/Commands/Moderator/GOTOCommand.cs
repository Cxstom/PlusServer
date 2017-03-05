using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GOTOCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_goto"; }
        }

        public string Parameters
        {
            get { return "%room_id%"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("You must specify a room id!");
                return;
            }

            int roomId = 0;
            if (!int.TryParse(Params[1], out roomId))
            {
                Session.SendWhisper("You must enter a valid room ID");
            }
            else
            {
                Room room = null;
                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out room))
                {
                    Session.SendWhisper("This room does not exist!");
                    return;
                }

                Session.GetHabbo().PrepareRoom(room.Id, "");
            }
        }
    }
}