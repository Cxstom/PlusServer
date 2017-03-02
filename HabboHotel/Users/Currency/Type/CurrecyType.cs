namespace Plus.HabboHotel.Users.Currency.Type
{
    public sealed class CurrencyType
    {
        public int Type { get; set; }
        public int Amount { get; set; }

        public CurrencyType(int type, int amount)
        {
            this.Type = type;
            this.Amount = amount;
        }
    }
}