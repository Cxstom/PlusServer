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
        private readonly ConcurrentDictionary<int, CurrencyType> _currencies;

        public CurrencyComponent(Habbo habbo)
        {
            this._player = habbo;
            this._currencies = new ConcurrentDictionary<int, CurrencyType>();
        }

        public bool Init()
        {
            if (this._currencies.Count > 0)
                return false;
            
            DataTable getCurrencies = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `type`, `amount` FROM `user_currencies` WHERE `user_id` = @uid");
                dbClient.AddParameter("uid", _player.Id);
                getCurrencies = dbClient.GetTable();

                if (getCurrencies != null)
                {
                    // Add the duckets for the user by default
                    int duckets = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.starting_duckets"));
                    if (getCurrencies.Rows.Count == 0)
                    {
                        dbClient.SetQuery("INSERT INTO `user_currencies` (`type`, `amount`, `user_id`) VALUES ('0', @amount, @uid)");
                        dbClient.AddParameter("amount", duckets);
                        dbClient.AddParameter("uid", _player.Id);
                        dbClient.RunQuery();

                        this._currencies.TryAdd(0, new CurrencyType(0, duckets));
                    }
                    foreach (DataRow dRow in getCurrencies.Rows)
                    {
                        this._currencies.TryAdd(Convert.ToInt32(dRow["type"]), new CurrencyType(Convert.ToInt32(dRow["type"]), Convert.ToInt32(dRow["amount"])));
                    }
                }
            }
            return true;
        }

        public void CheckPointsTimer()
        {
            foreach (CurrencyType currency in this._currencies.Values)
            {
                CurrencyDefinition currencyDefinition = PlusEnvironment.GetGame().GetCurrencyManager().GetCurrencyByType(currency.Type);
                if (currencyDefinition == null)
                    continue;

                currency.Amount += currencyDefinition.Reward;
                if (this._player.GetClient() != null)
                    this._player.GetClient().SendPacket(new HabboActivityPointNotificationComposer(currency.Amount, currencyDefinition.Reward, currency.Type));
            }
        }

        public bool TryGet(int type, out CurrencyType currencyType, bool addToDatabase = false)
        {
            if (!this._currencies.ContainsKey(type) && addToDatabase)
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_currencies` (`type`, `amount`, `user_id`) VALUES (@type, '0', @uid)");
                    dbClient.AddParameter("type", type);
                    dbClient.AddParameter("uid", _player.Id);
                    dbClient.RunQuery();

                    this._currencies.TryAdd(type, new CurrencyType(type, 0));
                }
            }
            return this._currencies.TryGetValue(type, out currencyType);
        }

        public ICollection<CurrencyType> getCurrencies
        {
            get { return this._currencies.Values; }
        }

        public void SaveCurrency(IQueryAdapter dbClient)
        {
            foreach (CurrencyType currency in this._currencies.Values)
            {
                dbClient.SetQuery("UPDATE `user_currencies` SET `amount` = @amount WHERE `type` = @type AND `user_id` = @uid LIMIT 1");
                dbClient.AddParameter("amount", currency.Amount);
                dbClient.AddParameter("type", currency.Type);
                dbClient.AddParameter("uid", _player.Id);
                dbClient.RunQuery();
            }
        }

        public void Dispose()
        {
            this._currencies.Clear();
        }
    }
}