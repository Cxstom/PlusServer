using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorGuildFurni : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Item.GroupId, out Group))
            {
                Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
                Message.WriteString(Item.ExtraData);
            }
            else
            {
                Message.WriteInteger((Item.LimitedNo > 0 ? 256 : 0) + 2);
                Message.WriteInteger(5);
                Message.WriteString(Item.ExtraData);
                Message.WriteString(Group.Id.ToString());
                Message.WriteString(Group.Badge);
                Message.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(Group.Colour1, true));
                Message.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(Group.Colour2, false));
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