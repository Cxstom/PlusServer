using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;
using Plus.HabboHotel.Rooms.Games.Teams;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorBanzaiFloor : IFurniInteractor
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
            if (Item.value == 3)
            {
                if (Item.interactionCountHelper == 1)
                {
                    Item.interactionCountHelper = 0;

                    switch (Item.team)
                    {
                        case TEAM.BLUE:
                            {
                                Item.ExtraData = "11";
                                break;
                            }

                        case TEAM.GREEN:
                            {
                                Item.ExtraData = "8";
                                break;
                            }

                        case TEAM.RED:
                            {
                                Item.ExtraData = "5";
                                break;
                            }

                        case TEAM.YELLOW:
                            {
                                Item.ExtraData = "14";
                                break;
                            }
                    }
                }
                else
                {
                    Item.ExtraData = "";
                    Item.interactionCountHelper++;
                }

                Item.UpdateState();
                Item.interactionCount++;

                if (Item.interactionCount < 16)
                    Item.UpdateCounter = 1;
                else
                    Item.UpdateCounter = 0;
            }
        }
    }
}