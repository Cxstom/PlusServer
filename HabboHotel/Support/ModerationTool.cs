using System;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

using Plus.Database.Interfaces;

using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.HabboHotel.Support
{   
    /// <summary>
    /// TODO: Utilize ModerationTicket.cs
    /// </summary>
    public class ModerationTool
    {

        #region Support Tickets
        /*public void SendNewTicket(GameClient Session, int Category, int ReportedUser, String Message, List<string> Messages)
        {
            int TicketId = 0;
            SupportTicket Ticket;

            if (Session.GetHabbo().CurrentRoomId <= 0)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO moderation_tickets (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + Category + "','open','" + Session.GetHabbo().Id + "','" + ReportedUser + "','0',@message,'0','','" + PlusEnvironment.GetUnixTimestamp() + "')");
                    dbClient.AddParameter("message", Message);
                    TicketId = Convert.ToInt32(dbClient.InsertQuery());

                    dbClient.RunQuery("UPDATE `user_info` SET `cfhs` = `cfhs` + '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                }

                Ticket = new SupportTicket(TicketId, 1, 7, Session.GetHabbo().Id, ReportedUser, Message, 0, "", PlusEnvironment.GetUnixTimestamp(), Messages);

                Tickets.Add(Ticket);

                SendTicketToModerators(Ticket);
                return;
            }

            RoomData Data = PlusEnvironment.GetGame().GetRoomManager().GenerateRoomData(Session.GetHabbo().CurrentRoomId);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO moderation_tickets (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + Category + "','open','" + Session.GetHabbo().Id + "','" + ReportedUser + "','0',@message,'" + Data.Id + "',@name,'" + PlusEnvironment.GetUnixTimestamp() + "')");
                dbClient.AddParameter("message", Message);
                dbClient.AddParameter("name", Data.Name);
                TicketId = Convert.ToInt32(dbClient.InsertQuery());

                dbClient.RunQuery("UPDATE user_info SET cfhs = cfhs + 1 WHERE user_id = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            Ticket = new SupportTicket(TicketId, 1, 7, Session.GetHabbo().Id, ReportedUser, Message, Data.Id, Data.Name, PlusEnvironment.GetUnixTimestamp(), Messages);
            Tickets.Add(Ticket);
            SendTicketToModerators(Ticket);
        }*/
        #endregion
    }
}