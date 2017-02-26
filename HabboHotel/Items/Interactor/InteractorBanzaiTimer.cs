using System;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorBanzaiTimer : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
            Message.WriteString(Item.ExtraData);
        }

        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (!HasRights)
            {
                return;
            }

            int oldValue = 0;

            if (!int.TryParse(Item.ExtraData, out oldValue))
            {
                Item.ExtraData = "0";
                oldValue = 0;
            }

            if (Request == 0 && oldValue == 0)
            {
                oldValue = 30;
            }
            else if (Request == 2)
            {
                if (Item.GetRoom().GetBanzai().isBanzaiActive && Item.pendingReset && oldValue > 0)
                {
                    oldValue = 0;
                    Item.pendingReset = false;
                }
                else
                {
                    if (oldValue < 30)
                        oldValue = 30;
                    else if (oldValue == 30)
                        oldValue = 60;
                    else if (oldValue == 60)
                        oldValue = 120;
                    else if (oldValue == 120)
                        oldValue = 180;
                    else if (oldValue == 180)
                        oldValue = 300;
                    else if (oldValue == 300)
                        oldValue = 600;
                    else
                        oldValue = 0;
                    Item.UpdateNeeded = false;
                }
            }
            else if (Request == 1 || Request == 0)
            {
                if (Request == 1 && oldValue == 0)
                {
                    Item.ExtraData = "30";
                    oldValue = 30;
                }

                if (!Item.GetRoom().GetBanzai().isBanzaiActive)
                {
                    Item.UpdateNeeded = !Item.UpdateNeeded;

                    if (Item.UpdateNeeded)
                    {
                        Item.GetRoom().GetBanzai().BanzaiStart();
                    }

                    Item.pendingReset = true;
                }
                else
                {
                    Item.UpdateNeeded = !Item.UpdateNeeded;

                    if (Item.UpdateNeeded)
                    {
                        Item.GetRoom().GetBanzai().BanzaiEnd(true);
                    }

                    Item.pendingReset = true;
                }
            }


            Item.ExtraData = Convert.ToString(oldValue);
            Item.UpdateState();
        }

        public void OnWiredTrigger(Item Item)
        {
            if (Item.GetRoom().GetBanzai().isBanzaiActive)
                Item.GetRoom().GetBanzai().BanzaiEnd(true);

            Item.pendingReset = true;
            Item.UpdateNeeded = true;
            Item.ExtraData = "30";
            Item.UpdateState();

            if (!Item.GetRoom().GetBanzai().isBanzaiActive)
                Item.GetRoom().GetBanzai().BanzaiStart();
        }

        public void OnCycle(Item Item)
        {
            if (string.IsNullOrEmpty(Item.ExtraData))
                return;

            int seconds = 0;

            try
            {
                seconds = int.Parse(Item.ExtraData);
            }
            catch { }

            if (seconds > 0)
            {
                if (Item.interactionCountHelper == 1)
                {
                    seconds--;
                    Item.interactionCountHelper = 0;

                    if (Item.GetRoom().GetBanzai().isBanzaiActive)
                    {
                        Item.ExtraData = seconds.ToString();
                        Item.UpdateState();
                    }
                    else
                        return;
                }
                else
                    Item.interactionCountHelper++;

                Item.UpdateCounter = 1;
            }
            else
            {
                Item.UpdateCounter = 0;
                Item.GetRoom().GetBanzai().BanzaiEnd();
            }
        }
    }
}