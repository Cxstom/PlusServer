using System;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorRoomAd : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger((Item.LimitedNo > 0 ? 256 : 0) + 1);
            Message.WriteString(Item.ExtraData);

            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                Message.WriteInteger(Item.ExtraData.Split(Convert.ToChar(9)).Length / 2);

                for (int i = 0; i <= Item.ExtraData.Split(Convert.ToChar(9)).Length - 1; i++)
                {
                    Message.WriteString(Item.ExtraData.Split(Convert.ToChar(9))[i]);
                }
            }
            else
                Message.WriteInteger(0);
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