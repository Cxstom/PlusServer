using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;
using Plus.HabboHotel.Items.Data.Toner;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorBackgroundToner : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            if (Item.RoomId != 0)
            {
                if (Item.GetRoom().TonerData == null)
                    Item.GetRoom().TonerData = new TonerData(Item.Id);

                Message.WriteInteger((Item.LimitedNo > 0 ? 256 : 0) + 5);
                Message.WriteInteger(4);
                Message.WriteInteger(Item.GetRoom().TonerData.Enabled);
                Message.WriteInteger(Item.GetRoom().TonerData.Hue);
                Message.WriteInteger(Item.GetRoom().TonerData.Saturation);
                Message.WriteInteger(Item.GetRoom().TonerData.Lightness);
            }
            else
            {
                Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
                Message.WriteString(string.Empty);
            }
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
        }
    }
}