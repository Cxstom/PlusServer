using System;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Items.Interactor
{
    class InteractorMannequin : IFurniInteractor
    {
        public void SerializeExtradata(ServerPacket Message, Item Item)
        {
            Message.WriteInteger((Item.LimitedNo > 0 ? 256 : 0) + 1);
            Message.WriteInteger(3);
            if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                string[] Stuff = Item.ExtraData.Split(Convert.ToChar(5));
                Message.WriteString("GENDER");
                Message.WriteString(Stuff[0]);
                Message.WriteString("FIGURE");
                Message.WriteString(Stuff[1]);
                Message.WriteString("OUTFIT_NAME");
                Message.WriteString(Stuff[2]);
            }
            else
            {
                Message.WriteString("GENDER");
                Message.WriteString("m");
                Message.WriteString("FIGURE");
                Message.WriteString("");
                Message.WriteString("OUTFIT_NAME");
                Message.WriteString("My Look");
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
            if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                String[] Stuff = Item.ExtraData.Split(Convert.ToChar(5));
                Session.GetHabbo().Gender = Stuff[0].ToUpper();
                Dictionary<String, String> NewFig = new Dictionary<String, String>();
                NewFig.Clear();
                foreach (String Man in Stuff[1].Split('.'))
                {
                    foreach (String Fig in Session.GetHabbo().Look.Split('.'))
                    {
                        if (Fig.Split('-')[0] == Man.Split('-')[0])
                        {
                            if (NewFig.ContainsKey(Fig.Split('-')[0]) && !NewFig.ContainsValue(Man))
                            {
                                NewFig.Remove(Fig.Split('-')[0]);
                                NewFig.Add(Fig.Split('-')[0], Man);
                            }
                            else if (!NewFig.ContainsKey(Fig.Split('-')[0]) && !NewFig.ContainsValue(Man))
                            {
                                NewFig.Add(Fig.Split('-')[0], Man);
                            }
                        }
                        else
                        {
                            if (!NewFig.ContainsKey(Fig.Split('-')[0]))
                            {
                                NewFig.Add(Fig.Split('-')[0], Fig);
                            }
                        }
                    }
                }

                string Final = "";
                foreach (String Str in NewFig.Values)
                {
                    Final += Str + ".";
                }


                Session.GetHabbo().Look = Final.TrimEnd('.');

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE users SET look = @look, gender = @gender WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    dbClient.AddParameter("look", Session.GetHabbo().Look);
                    dbClient.AddParameter("gender", Session.GetHabbo().Gender);
                    dbClient.RunQuery();
                }

                Room Room = Session.GetHabbo().CurrentRoom;
                if (Room != null)
                {
                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
                    if (User != null)
                    {
                        Session.SendPacket(new UserChangeComposer(User, true));
                        Session.GetHabbo().CurrentRoom.SendPacket(new UserChangeComposer(User, false));
                    }
                }
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
