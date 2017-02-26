using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Plus.HabboHotel.Items.Interactor
{
    class InteractorCannon : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger(Item.LimitedNo > 0 ? 256 : 0);
            Message.WriteString(Item.ExtraData);
        }

        public void OnPlace(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClients.GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if(Room == null)
                return;

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Actor == null)
                return;

            if (Item.ExtraData == "1")
                return;

            if(Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) > 2)
                return;

            Item.ExtraData = "1";
            Item.UpdateState(false, true);

            Item.RequestUpdate(2, true);
        }

        public void OnWiredTrigger(Item Item)
        {
        }

        public void OnCycle(Item Item)
        {
            if (Item.ExtraData != "1")
                return;

            #region Target Calculation
            Point TargetStart = Item.Coordinate;
            List<Point> TargetSquares = new List<Point>();
            switch (Item.Rotation)
            {
                case 0:
                    {
                        TargetStart = new Point(Item.GetX - 1, Item.GetY);

                        if (!TargetSquares.Contains(TargetStart))
                            TargetSquares.Add(TargetStart);

                        for (int I = 1; I <= 3; I++)
                        {
                            Point TargetSquare = new Point(TargetStart.X - I, TargetStart.Y);

                            if (!TargetSquares.Contains(TargetSquare))
                                TargetSquares.Add(TargetSquare);
                        }

                        break;
                    }

                case 2:
                    {
                        TargetStart = new Point(Item.GetX, Item.GetY - 1);

                        if (!TargetSquares.Contains(TargetStart))
                            TargetSquares.Add(TargetStart);

                        for (int I = 1; I <= 3; I++)
                        {
                            Point TargetSquare = new Point(TargetStart.X, TargetStart.Y - I);

                            if (!TargetSquares.Contains(TargetSquare))
                                TargetSquares.Add(TargetSquare);
                        }

                        break;
                    }

                case 4:
                    {
                        TargetStart = new Point(Item.GetX + 2, Item.GetY);

                        if (!TargetSquares.Contains(TargetStart))
                            TargetSquares.Add(TargetStart);

                        for (int I = 1; I <= 3; I++)
                        {
                            Point TargetSquare = new Point(TargetStart.X + I, TargetStart.Y);

                            if (!TargetSquares.Contains(TargetSquare))
                                TargetSquares.Add(TargetSquare);
                        }

                        break;
                    }

                case 6:
                    {
                        TargetStart = new Point(Item.GetX, Item.GetY + 2);


                        if (!TargetSquares.Contains(TargetStart))
                            TargetSquares.Add(TargetStart);

                        for (int I = 1; I <= 3; I++)
                        {
                            Point TargetSquare = new Point(TargetStart.X, TargetStart.Y + I);

                            if (!TargetSquares.Contains(TargetSquare))
                                TargetSquares.Add(TargetSquare);
                        }

                        break;
                    }
            }
            #endregion

            if (TargetSquares.Count > 0)
            {
                foreach (Point Square in TargetSquares.ToList())
                {
                    List<RoomUser> affectedUsers = Item.GetRoom().GetGameMap().GetRoomUsers(Square).ToList();

                    if (affectedUsers == null || affectedUsers.Count == 0)
                        continue;

                    foreach (RoomUser Target in affectedUsers)
                    {
                        if (Target == null || Target.IsBot || Target.IsPet)
                            continue;

                        if (Target.GetClient() == null || Target.GetClient().GetHabbo() == null)
                            continue;

                        if (Item.GetRoom().CheckRights(Target.GetClient(), true))
                            continue;

                        Target.ApplyEffect(4);
                        Target.GetClient().SendPacket(new RoomNotificationComposer("Kicked from room", "You were hit by a cannonball!", "room_kick_cannonball", ""));
                        Target.ApplyEffect(0);
                        Item.GetRoom().GetRoomUserManager().RemoveUserFromRoom(Target.GetClient(), true);
                    }
                }
            }

            Item.ExtraData = "2";
            Item.UpdateState(false, true);
        }
    }
}
