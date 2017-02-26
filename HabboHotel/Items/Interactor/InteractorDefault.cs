using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items.Wired;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorDefault : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
            Message.WriteString(Item.GetBaseItem().InteractionType != InteractionType.FOOTBALL_GATE ? Item.ExtraData : string.Empty);
        }

        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }
        
        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            int Modes = Item.GetBaseItem().Modes - 1;

            if (!HasRights || Modes <= 0)
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

            if (NewMode == 0 && Item.Data.InteractionType == InteractionType.GATE)
            {
                if (!Item.GetRoom().GetGameMap().itemCanBePlacedHere(Item.GetX, Item.GetY))
                    return;
            }

            Item.ExtraData = NewMode.ToString();
            Item.UpdateState();

            if (Item.Data.InteractionType == InteractionType.GATE)
                Item.GetRoom().GetGameMap().updateMapForItem(Item);
            Item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, Session.GetHabbo(), Item);
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

            if (NewMode == 0 && Item.Data.InteractionType == InteractionType.GATE)
            {
                if (!Item.GetRoom().GetGameMap().itemCanBePlacedHere(Item.GetX, Item.GetY))
                    return;
            }

            Item.ExtraData = NewMode.ToString();
            Item.UpdateState();

            if (Item.Data.InteractionType == InteractionType.GATE)
                Item.GetRoom().GetGameMap().updateMapForItem(Item);
        }

        public void OnCycle(Item Item)
        {
        }
    }
}