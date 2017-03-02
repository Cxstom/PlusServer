using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.HabboHotel.Currency;
using Plus.HabboHotel.Users.Currency.Type;
using Plus.Database.Interfaces;

using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Users.Currency
{
    public sealed class CurrencyComponent
    {
        private readonly Habbo _player;
        private readonly ConcurrentDictionary<int, CurrencyType> _currecies;

        public CurrencyComponent(Habbo habbo)
        {
            this._player = habbo;
            this._currecies = new ConcurrentDictionary<int, CurrencyType>();
        }

        public bool Init()
        {
            if (this._currecies.Count > 0)
                return false;
            
            DataTable getcurrencies = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `type`, `amount` FROM `user_currencies` WHERE `user_id` = @uid");
                dbClient.AddParameter("uid", _player.Id);
                getcurrencies = dbClient.GetTable();

                if (getcurrencies != null)
                {
                    // Add the duckets for the user by default
                    int duckets = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.starting_duckets"));
                    if (getcurrencies.Rows.Count == 0)
                    {
                        dbClient.SetQuery("INSERT INTO `user_currencies` (`type`, `amount`, `user_id`) VALUES ('0', @amount, @uid)");
                        dbClient.AddParameter("amount", duckets);
                        dbClient.AddParameter("uid", _player.Id);
                        dbClient.RunQuery();

                        this._currecies.TryAdd(0, new CurrencyType(0, duckets));
                    }
                    foreach (DataRow dRow in getcurrencies.Rows)
                    {
                        this._currecies.TryAdd(Convert.ToInt32(dRow["type"]), new CurrencyType(Convert.ToInt32(dRow["type"]), Convert.ToInt32(dRow["amount"])));
                    }
                }
            }
            return true;
        }

        public void CheckPointsTimer()
        {
            foreach (CurrencyType currency in this._currecies.Values)
            {
                CurrencyDefinition currencyDefinition = null;
                if (!PlusEnvironment.GetGame().GetCurrencyManager().GetCurrencyByType(currency.Type, out currencyDefinition))
                    continue;

                currency.Amount += currencyDefinition.Reward;
                if (this._player.GetClient() != null)
                    this._player.GetClient().SendPacket(new HabboActivityPointNotificationComposer(currency.Amount, currencyDefinition.Reward, currency.Type));
            }
        }

        public bool TryGet(int type, out CurrencyType currencyType, bool addToDatabase = false)
        {
            if (!this._currecies.ContainsKey(type) && addToDatabase)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_currencies` (`type`, `amount`, `user_id`) VALUES (@type, '0', @uid)");
                    dbClient.AddParameter("type", type);
                    dbClient.AddParameter("uid", _player.Id);
                    dbClient.RunQuery();

                    this._currecies.TryAdd(type, new CurrencyType(type, 0));
                }
            }
            return this._currecies.TryGetValue(type, out currencyType);
        }

        public ICollection<CurrencyType> GetCurrencies
        {
            get { return this._currecies.Values; }
        }

        public void Dispose()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                foreach (CurrencyType currency in this._currecies.Values)
                {
                    dbClient.SetQuery("UPDATE `user_currencies` SET `amount` = @amount WHERE `type` = @type AND `user_id` = @uid LIMIT 1;");
                    dbClient.AddParameter("type", currency.Type);
                    dbClient.AddParameter("amount", currency.Amount);
                    dbClient.AddParameter("uid", _player.Id);
                    dbClient.RunQuery();
                }
            }
            this._currecies.Clear();
        }
    }
}