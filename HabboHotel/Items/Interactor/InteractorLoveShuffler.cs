using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;
using Plus.Utilities;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorLoveShuffler : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
            Message.WriteString(Item.ExtraData);
        }

        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "-1";
            Item.UpdateNeeded = true;
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "-1";
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (!HasRights)
            {
                return;
            }

            if (Item.ExtraData != "0")
            {
                Item.ExtraData = "0";
                Item.UpdateState(false, true);
                Item.RequestUpdate(10, true);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
            if (Item.ExtraData != "0")
            {
                Item.ExtraData = "0";
                Item.UpdateState(false, true);
                Item.RequestUpdate(10, true);
            }
        }

        public void OnCycle(Item Item)
        {
            if (Item.ExtraData == "0")
            {
                Item.ExtraData = RandomNumber.GenerateNewRandom(1, 4).ToString();
                Item.RequestUpdate(20, false);
            }
            else if (Item.ExtraData != "-1")
                Item.ExtraData = "-1";

            Item.UpdateState(false, true);
        }
    }
}