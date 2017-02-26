using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public interface IFurniInteractor
    {
        void SerializeExtradata(ServerPacket message, Item item);
        void OnPlace(GameClient session, Item item);
        void OnRemove(GameClient session, Item item);
        void OnTrigger(GameClient session, Item item, int request, bool hasRights);
        void OnWiredTrigger(Item item);
        void OnCycle(Item item);
    }
}