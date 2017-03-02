using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Currency;
using Plus.HabboHotel.Users.Currency.Type;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.Communication.RCON.Commands.User
{
    class ReloadUserCurrencyCommand : IRCONCommand
    {
        public string Description
        {
            get { return "This command is used to update the users currency from the database."; }
        }

        public string Parameters
        {
            get { return "%userId% %currency%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            // Validate the currency type
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string currency = Convert.ToString(parameters[1]);

            if (currency == "coins" || currency == "credits")
            {
                int credits = 0;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `credits` FROM `users` WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", userId);
                    credits = dbClient.GetInteger();
                }

                client.GetHabbo().Credits = credits;
                client.SendPacket(new CreditBalanceComposer(client.GetHabbo().Credits));
                return true;
            }

            //let's check currencies 

            CurrencyDefinition currencyDefinition = null;
            if (!PlusEnvironment.GetGame().GetCurrencyManager().TryGetCurrency(currency, out currencyDefinition))
                return false;

            CurrencyType currencyType = null;
            if (!client.GetHabbo().GetCurrency().TryGet(currencyDefinition.Type, out currencyType))
                return false;

            int points = 0;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `amount` FROM `user_currencies` WHERE `type` = @type AND `user_id` = @id LIMIT 1");
                dbClient.AddParameter("type", currencyType.Type);
                dbClient.AddParameter("id", userId);
                points = dbClient.GetInteger();
            }

            currencyType.Amount += points;
            client.SendPacket(new HabboActivityPointNotificationComposer(currencyType.Amount, points, currencyType.Type));

            return true;
        }
    }
}