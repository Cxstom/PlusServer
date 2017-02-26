using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Items.Wired;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    class InteractorSwitch : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
            Message.WriteString(Item.ExtraData);
        }

        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
            {
                int Modes = Item.GetBaseItem().Modes - 1;

                if (!HasRights || Modes <= 0)
                    return;
                
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_SWITCH);

                int CurrentMode = 0;
                int NewMode = 0;

                if (string.IsNullOrEmpty(Item.ExtraData))
                    Item.ExtraData = "0";

                if (!int.TryParse(Item.ExtraData, out CurrentMode))
                    return;

                if (CurrentMode <= 0)
                    NewMode = 1;
                else if (CurrentMode >= Modes)
                    NewMode = 0;
                else
                    NewMode = CurrentMode + 1;
                
                Item.ExtraData = NewMode.ToString();
                Item.UpdateState();
                
                Item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, Session.GetHabbo(), Item);
            }
            else
                User.MoveTo(Item.SquareInFront);
        }

        public void OnWiredTrigger(Item Item)
        {
            int Modes = Item.GetBaseItem().Modes - 1;

            if (Modes <= 0)
                return;
            
            int CurrentMode = 0;
            int NewMode = 0;

            if (string.IsNullOrEmpty(Item.ExtraData))
                Item.ExtraData = "0";

            if (!int.TryParse(Item.ExtraData, out CurrentMode))
                return;

            if (CurrentMode <= 0)
                NewMode = 1;
            else if (CurrentMode >= Modes)
                NewMode = 0;
            else
                NewMode = CurrentMode + 1;

            Item.ExtraData = NewMode.ToString();
            Item.UpdateState();
        }

        public void OnCycle(Item Item)
        {
        }
    }
}