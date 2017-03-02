using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Currency;
using Plus.HabboHotel.Users.Currency.Type;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.Communication.RCON.Commands.User
{
    class TakeUserCurrencyCommand : IRCONCommand
    {
        public string Description
        {
            get { return "This command is used to take a specified amount of a specified currency from a user."; }
        }

        public string Parameters
        {
            get { return "%userId% %currency% %amount%"; }
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

            int amount = 0;
            if (!int.TryParse(parameters[2].ToString(), out amount))
                return false;

            if (currency == "coins" || currency == "credits")
            {
                client.GetHabbo().Credits -= amount;

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `users` SET `credits` = @credits WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("credits", client.GetHabbo().Credits);
                    dbClient.AddParameter("id", userId);
                    dbClient.RunQuery();
                }

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

            currencyType.Amount -= amount;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_currencies` SET `amount` = @amount WHERE `type` = @type AND `user_id` = @id LIMIT 1");
                dbClient.AddParameter("amount", currencyType.Amount);
                dbClient.AddParameter("type", currencyType.Type);
                dbClient.AddParameter("id", userId);
                dbClient.RunQuery();
            }
            
            client.SendPacket(new HabboActivityPointNotificationComposer(currencyType.Amount, amount, currencyType.Type));

            return true;
        }
    }
}