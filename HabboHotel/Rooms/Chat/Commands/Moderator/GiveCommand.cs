using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Currency;
using Plus.HabboHotel.Users.Currency.Type;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_give"; }
        }

        public string Parameters
        {
            get { return "%username% %type% %amount%"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter a currency type! (coins, duckets, diamonds, gotw)");
                return;
            }

            GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, couldn't find that user!");
                return;
            }

            string currency = Params[2].ToLower();

            int amount = 0;
            if (!int.TryParse(Params[3], out amount))
                return;

            if (currency == "coins" || currency == "credits")
            {
                Target.GetHabbo().Credits += amount;
                Target.SendPacket(new CreditBalanceComposer(Target.GetHabbo().Credits));

                if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                    Target.SendNotification(Session.GetHabbo().Username + " has given you " + amount.ToString() + " Credit(s)!");
                Session.SendWhisper("Successfully given " + amount + " Credit(s) to " + Target.GetHabbo().Username + "!");
                return;
            }

            //let's check currencies

            CurrencyDefinition currencyDefinition = null;
            if (!PlusEnvironment.GetGame().GetCurrencyManager().TryGetCurrency(currency, out currencyDefinition))
            {
                Session.SendWhisper("'" + currency + "' is not a valid currency!");
                return;
            }

            CurrencyType currencyType = null;
            if (!Target.GetHabbo().GetCurrency().TryGet(currencyDefinition.Type, out currencyType, true))
                return;

            currencyType.Amount += amount;
            Target.SendPacket(new HabboActivityPointNotificationComposer(currencyType.Amount, amount, currencyType.Type));

            if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                Target.SendNotification(Session.GetHabbo().Username + " has given you " + amount.ToString() + " " + currencyDefinition.Name + "(s)!");
            Session.SendWhisper("Successfully given " + amount + " " + currencyDefinition.Name + "(s) to " + Target.GetHabbo().Username + "!");

            return;
        }
    }
}