namespace Plus.HabboHotel.Badges
{
    public class BadgeDefinition
    {
        private string _code;
        private string _requiredRight;

        public BadgeDefinition(string code, string requiredRight)
        {
            this._code = code;
            this._requiredRight = requiredRight;
        }

        public string Code
        {
            get { return this._code; }
            set { this._code = value; }
        }

        public string RequiredRight
        {
            get { return this._requiredRight; }
            set { this._requiredRight = value; }
        }
    }
}