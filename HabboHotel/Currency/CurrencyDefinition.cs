namespace Plus.HabboHotel.Currency
{
    public class CurrencyDefinition
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public int Reward { get; set; }

        public CurrencyDefinition(string name, int type, int reward)
        {
            this.Name = name;
            this.Type = type;
            this.Reward = reward;
        }
    }
}