using System;

using Plus.HabboHotel.Cache.Type;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorGift : IFurniInteractor
    {
        private int _colorId = 0;
        private int _ribbonId = 0;
        private bool _showSender = false;
        private string _message = "";
        private string _sender = "";
        private string _look = "";

        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            string[] ExtraData = Item.ExtraData.Split(Convert.ToChar(5));
            if (ExtraData.Length == 7)
            {
                _colorId = int.Parse(ExtraData[6]);
                _ribbonId = int.Parse(ExtraData[6]);
                
                UserCache Purchaser = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Convert.ToInt32(ExtraData[2]));
                if (Purchaser != null)
                {
                    _message = ExtraData[1];
                    _sender = Purchaser.Username;
                    _look = Purchaser.Username;
                    _showSender = true; //to-do
                }
            }

            Message.WriteInteger(_colorId * 1000 + _ribbonId);
            Message.WriteInteger(1);
            Message.WriteInteger(6);
            Message.WriteString("EXTRA_PARAM");
            Message.WriteString("");
            Message.WriteString("MESSAGE");
            Message.WriteString(_message);
            Message.WriteString("PURCHASER_NAME");
            Message.WriteString(_showSender ? _sender : "");
            Message.WriteString("PURCHASER_FIGURE");
            Message.WriteString(_showSender ? _look : "");
            Message.WriteString("PRODUCT_CODE");
            Message.WriteString("");
            Message.WriteString("state");
            Message.WriteString(Item.MagicRemove == true ? "1" : "0");
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