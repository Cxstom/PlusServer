using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorHopper : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
            Message.WriteString(Item.ExtraData);
        }

        public void OnPlace(GameClient Session, Item Item)
        {
            Item.GetRoom().GetRoomItemHandler().HopperCount++;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO items_hopper (hopper_id, room_id) VALUES (@hopperid, @roomid);");
                dbClient.AddParameter("hopperid", Item.Id);
                dbClient.AddParameter("roomid", Item.RoomId);
                dbClient.RunQuery();
            }

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
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.GetRoom().GetRoomItemHandler().HopperCount--;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM items_hopper WHERE item_id=@hid OR room_id=" + Item.GetRoom().RoomId +
                                  " LIMIT 1");
                dbClient.AddParameter("hid", Item.Id);
                dbClient.RunQuery();
            }

            if (Item.InteractingUser != 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.UnlockWalking();
                }

                Item.InteractingUser = 0;
            }
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Item == null || Item.GetRoom() == null || Session == null || Session.GetHabbo() == null)
                return;
            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            // Alright. But is this user in the right position?
            if (User.Coordinate == Item.Coordinate || User.Coordinate == Item.SquareInFront)
            {
                // Fine. But is this tele even free?
                if (Item.InteractingUser != 0)
                {
                    return;
                }

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
            bool showHopperEffect = false;
            bool keepDoorOpen = false;
            int Pause = 0;
            
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
                        if (User.TeleDelay == 0)
                        {
                            int RoomHopId = ItemHopperFinder.GetAHopper(User.RoomId);
                            int NextHopperId = ItemHopperFinder.GetHopperId(RoomHopId);

                            if (!User.IsBot && User != null && User.GetClient() != null &&
                                User.GetClient().GetHabbo() != null)
                            {
                                User.GetClient().GetHabbo().IsHopping = true;
                                User.GetClient().GetHabbo().HopperId = NextHopperId;
                                User.GetClient().GetHabbo().PrepareRoom(RoomHopId, "");
                                //User.GetClient().SendMessage(new RoomForwardComposer(RoomHopId));
                                Item.InteractingUser = 0;
                            }
                        }
                        else
                        {
                            User.TeleDelay--;
                            showHopperEffect = true;
                        }
                    }
                    // Is he in front of the tele?
                    else if (User.Coordinate == Item.SquareInFront)
                    {
                        User.AllowOverride = true;
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
            if (keepDoorOpen)
            {
                if (Item.ExtraData != "1")
                {
                    Item.ExtraData = "1";
                    Item.UpdateState(false, true);
                }
            }
            else if (showHopperEffect)
            {
                if (Item.ExtraData != "2")
                {
                    Item.ExtraData = "2";
                    Item.UpdateState(false, true);
                }
            }
            else
            {
                if (Item.ExtraData != "0")
                {
                    if (Pause == 0)
                    {
                        Item.ExtraData = "0";
                        Item.UpdateState(false, true);
                        Pause = 2;
                    }
                    else
                    {
                        Pause--;
                    }
                }
            }

            // We're constantly going!
            Item.RequestUpdate(1, false);
        }
    }
}