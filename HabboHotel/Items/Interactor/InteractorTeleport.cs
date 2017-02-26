using System.Drawing;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorTeleport : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
            Message.WriteString(Item.ExtraData);
        }

        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.ClearMovement(true);
                    User.AllowOverride = false;
                    User.CanWalk = true;
                }

                Item.InteractingUser = 0;
            }

            if (Item.InteractingUser2 != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser2);

                if (User != null)
                {
                    User.ClearMovement(true);
                    User.AllowOverride = false;
                    User.CanWalk = true;
                }

                Item.InteractingUser2 = 0;
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.UnlockWalking();
                }

                Item.InteractingUser = 0;
            }

            if (Item.InteractingUser2 != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser2);

                if (User != null)
                {
                    User.UnlockWalking();
                }

                Item.InteractingUser2 = 0;
            }
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Item == null || Item.GetRoom() == null || Session == null || Session.GetHabbo() == null)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.LastInteraction = PlusEnvironment.GetUnixTimestamp();

            // Alright. But is this user in the right position?
            if (User.Coordinate == Item.Coordinate || User.Coordinate == Item.SquareInFront)
            {
                // Fine. But is this tele even free?
                if (Item.InteractingUser != 0)
                {
                    return;
                }

                if (!User.CanWalk || Session.GetHabbo().IsTeleporting || Session.GetHabbo().TeleporterId != 0 ||
                    (User.LastInteraction + 2) - PlusEnvironment.GetUnixTimestamp() < 0)
                    return;

                User.TeleDelay = 2;

                Item.InteractingUser = User.GetClient().GetHabbo().Id;
            }
            else if (User.CanWalk)
            {
                User.MoveTo(Item.SquareInFront);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }

        public void OnCycle(Item Item)
        {
            RoomUser User = null;
            RoomUser User2 = null;

            bool keepDoorOpen = false;
            bool showTeleEffect = false;

            // Do we have a primary user that wants to go somewhere?
            if (Item.InteractingUser > 0)
            {
                User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                // Is this user okay?
                if (User != null)
                {
                    // Is he in the tele?
                    if (User.Coordinate == Item.Coordinate)
                    {
                        //Remove the user from the square
                        User.AllowOverride = false;

                        if (ItemTeleporterFinder.IsTeleLinked(Item.Id, Item.GetRoom()))
                        {
                            showTeleEffect = true;

                            if (true)
                            {
                                // Woop! No more delay.
                                int TeleId = ItemTeleporterFinder.GetLinkedTele(Item.Id);
                                int RoomId = ItemTeleporterFinder.GetTeleRoomId(TeleId, Item.GetRoom());

                                // Do we need to tele to the same room or gtf to another?
                                if (RoomId == Item.RoomId)
                                {
                                    Item GetItem = Item.GetRoom().GetRoomItemHandler().GetItem(TeleId);

                                    if (GetItem == null)
                                    {
                                        User.UnlockWalking();
                                    }
                                    else
                                    {
                                        // Set pos
                                        User.SetPos(GetItem.GetX, GetItem.GetY, GetItem.GetZ);
                                        User.SetRot(GetItem.Rotation, false);

                                        // Force tele effect update (dirty)
                                        GetItem.ExtraData = "2";
                                        GetItem.UpdateState(false, true);

                                        // Set secondary interacting user
                                        GetItem.InteractingUser2 = Item.InteractingUser;
                                        Item.GetRoom().GetGameMap().RemoveUserFromMap(User, new Point(Item.GetX, Item.GetY));

                                        Item.InteractingUser = 0;
                                    }
                                }
                                else
                                {
                                    if (User.TeleDelay == 0)
                                    {
                                        // Let's run the teleport delegate to take futher care of this.. WHY DARIO?!
                                        if (!User.IsBot && User != null && User.GetClient() != null &&
                                            User.GetClient().GetHabbo() != null)
                                        {
                                            User.GetClient().GetHabbo().IsTeleporting = true;
                                            User.GetClient().GetHabbo().TeleportingRoomID = RoomId;
                                            User.GetClient().GetHabbo().TeleporterId = TeleId;
                                            User.GetClient().GetHabbo().PrepareRoom(RoomId, "");
                                            //User.GetClient().SendMessage(new RoomForwardComposer(RoomId));
                                            Item.InteractingUser = 0;
                                        }
                                    }
                                    else
                                    {
                                        User.TeleDelay--;
                                        showTeleEffect = true;
                                    }
                                    //PlusEnvironment.GetGame().GetRoomManager().AddTeleAction(new TeleUserData(User.GetClient().GetMessageHandler(), User.GetClient().GetHabbo(), RoomId, TeleId));
                                }
                                Item.GetRoom().GetGameMap().GenerateMaps();
                                // We're done with this tele. We have another one to bother.
                            }
                            else
                            {
                                // We're linked, but there's a delay, so decrease the delay and wait it out.
                                //User.TeleDelay--;
                            }
                        }
                        else
                        {
                            // This tele is not linked, so let's gtfo.
                            User.UnlockWalking();
                            Item.InteractingUser = 0;
                        }
                    }
                    // Is he in front of the tele?
                    else if (User.Coordinate == Item.SquareInFront)
                    {
                        User.AllowOverride = true;
                        // Open the door
                        keepDoorOpen = true;

                        // Lock his walking. We're taking control over him. Allow overriding so he can get in the tele.
                        if (User.IsWalking && (User.GoalX != Item.GetX || User.GoalY != Item.GetY))
                        {
                            User.ClearMovement(true);
                        }

                        User.CanWalk = false;
                        User.AllowOverride = true;

                        // Move into the tele
                        User.MoveTo(Item.Coordinate.X, Item.Coordinate.Y, true);

                    }
                    // Not even near, do nothing and move on for the next user.
                    else
                    {
                        Item.InteractingUser = 0;
                    }
                }
                else
                {
                    // Invalid user, do nothing and move on for the next user. 
                    Item.InteractingUser = 0;
                }
            }

            // Do we have a secondary user that wants to get out of the tele?
            if (Item.InteractingUser2 > 0)
            {
                User2 = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser2);

                // Is this user okay?
                if (User2 != null)
                {
                    // If so, open the door, unlock the user's walking, and try to push him out in the right direction. We're done with him!
                    keepDoorOpen = true;
                    User2.UnlockWalking();
                    User2.MoveTo(Item.SquareInFront);
                }

                // This is a one time thing, whether the user's valid or not.
                Item.InteractingUser2 = 0;
            }

            // Set the new item state, by priority
            if (showTeleEffect)
            {
                if (Item.ExtraData != "2")
                {
                    Item.ExtraData = "2";
                    Item.UpdateState(false, true);
                }
            }
            else if (keepDoorOpen)
            {
                if (Item.ExtraData != "1")
                {
                    Item.ExtraData = "1";
                    Item.UpdateState(false, true);
                }
            }
            else
            {
                if (Item.ExtraData != "0")
                {
                    Item.ExtraData = "0";
                    Item.UpdateState(false, true);
                }
            }

            // We're constantly going!
            Item.RequestUpdate(1, false);
        }
    }
}