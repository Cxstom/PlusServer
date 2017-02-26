using System;
using System.Drawing;

using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Interactor
{
    // re-code LoveLocks?
    public class InteractorLoveLock : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger((Item.LimitedNo > 0 ? 256 : 0) + 2);
            if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                string[] EData = Item.ExtraData.Split((char)5);
                int I = 0;
                Message.WriteInteger(EData.Length);
                while (I < EData.Length)
                {
                    Message.WriteString(EData[I]);
                    I++;
                }
            }
            else
            {
                Message.WriteInteger(6);
                Message.WriteString("0");
                Message.WriteString("");
                Message.WriteString("");
                Message.WriteString("");
                Message.WriteString("");
                Message.WriteString("");
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
            RoomUser User = null;

            if (Session != null)
                User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
                return;

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
            {
                if (Item.ExtraData == null || Item.ExtraData.Length <= 1 || !Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                {
                    Point pointOne;
                    Point pointTwo;

                    switch (Item.Rotation)
                    {
                        case 2:
                            pointOne = new Point(Item.GetX, Item.GetY + 1);
                            pointTwo = new Point(Item.GetX, Item.GetY - 1);
                            break;

                        case 4:
                            pointOne = new Point(Item.GetX - 1, Item.GetY);
                            pointTwo = new Point(Item.GetX + 1, Item.GetY);
                            break;

                        default:
                            return;
                    }

                    RoomUser UserOne = Item.GetRoom().GetRoomUserManager().GetUserForSquare(pointOne.X, pointOne.Y);
                    RoomUser UserTwo = Item.GetRoom().GetRoomUserManager().GetUserForSquare(pointTwo.X, pointTwo.Y);

                    if(UserOne == null || UserTwo == null)
                        Session.SendNotification("We couldn't find a valid user to lock this love lock with.");
                    else if(UserOne.GetClient() == null || UserTwo.GetClient() == null)
                        Session.SendNotification("We couldn't find a valid user to lock this love lock with.");
                    else if(UserOne.HabboId != Item.UserID && UserTwo.HabboId != Item.UserID)
                        Session.SendNotification("You can only use this item with the item owner.");
                    else
                    {
                        UserOne.CanWalk = false;
                        UserTwo.CanWalk = false;

                        Item.InteractingUser = UserOne.GetClient().GetHabbo().Id;
                        Item.InteractingUser2 = UserTwo.GetClient().GetHabbo().Id;

                        UserOne.GetClient().SendPacket(new LoveLockDialogueMessageComposer(Item.Id));
                        UserTwo.GetClient().SendPacket(new LoveLockDialogueMessageComposer(Item.Id));
                    }


                }
                else
                    return;
            }
            else
            {
                User.MoveTo(Item.SquareInFront);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }

        public void OnCycle(Item Item)
        {
        }
    }
}