using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorBanzaiPuck : IFurniInteractor
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
        }

        public void OnWiredTrigger(Item Item)
        {
        }

        public void OnCycle(Item Item)
        {
            if (Item.interactionCount > 4)
            {
                Item.interactionCount++;
                Item.UpdateCounter = 1;
            }
            else
            {
                Item.interactionCount = 0;
                Item.UpdateCounter = 0;
            }
        }
    }
}