using System;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorBadgeDisplay : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger((Item.LimitedNo > 0 ? 256 : 0) + 2);
            Message.WriteInteger(4);
            Message.WriteString("0");//No idea

            string[] data = Item.ExtraData.Split(Convert.ToChar(9));
            if (Item.ExtraData.Contains(Convert.ToChar(9).ToString()))
            {
                Message.WriteString(data[0]); //Badge name
                Message.WriteString(data[1]); //Owner
                Message.WriteString(data[2]); //Date
            }
            else
            {
                Message.WriteString("DEV");
                Message.WriteString("Unknown User");
                Message.WriteString("Unknown Date");
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