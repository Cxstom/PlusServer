using System;
using System.Data;
using System.Collections.Generic;

using log4net;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Currency
{
    public class CurrencyManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Currency.CurrencyManager");

        private readonly Dictionary<string, CurrencyDefinition> _currencies;

        public CurrencyManager()
        {
            this._currencies = new Dictionary<string, CurrencyDefinition>();
        }

        public void Init()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `name`, `type_id`, `cycle_reward` FROM `currency_deffinitions`;");
                DataTable GetBadges = dbClient.GetTable();

                foreach (DataRow Row in GetBadges.Rows)
                {
                    string currencyName = Convert.ToString(Row["name"]).ToUpper();

                    if (!this._currencies.ContainsKey(currencyName))
                        this._currencies.Add(currencyName, new CurrencyDefinition(currencyName, Convert.ToInt32(Row["type_id"]), Convert.ToInt32(Row["cycle_reward"])));
                }
            }

            log.Info("Loaded " + this._currencies.Count + " Currency Definitions.");
        }
   
        public bool TryGetCurrency(string currencyName, out CurrencyDefinition currency)
        {
            return this._currencies.TryGetValue(currencyName.ToUpper(), out currency);
        }

        public bool GetCurrencyByType(int currencyType, out CurrencyDefinition currency)
        {
            currency = null;
            foreach (CurrencyDefinition currencyDefinition in this._currencies.Values)
            {
                if (currencyDefinition.Type == currencyType)
                    currency = currencyDefinition;
                return true;
            }
            return false;
        }
    }
}