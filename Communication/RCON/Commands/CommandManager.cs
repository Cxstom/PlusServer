using Plus.Communication.RCON.Commands.Hotel;
using Plus.Communication.RCON.Commands.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.RCON.Commands
{
    public class CommandManager
    { 
        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IRCONCommand> _commands;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager()
        {
            this._commands = new Dictionary<string, IRCONCommand>();
            
            this.RegisterUser();
            this.RegisterHotel();
        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="data">A string of data split by char(1), the first part being the command and the second part being the parameters.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(string data)
        {
            if (data.Length == 0 || string.IsNullOrEmpty(data))
                return false;

            string cmd = data.Split(Convert.ToChar(1))[0];

            IRCONCommand command = null;
            if (this._commands.TryGetValue(cmd.ToLower(), out command))
            {
                string param = null;
                string[] parameters = null;
                if (data.Split(Convert.ToChar(1))[1] != null)
                {
                    param = data.Split(Convert.ToChar(1))[1];
                    parameters = param.ToString().Split(':');
                }

                return command.TryExecute(parameters);
            }
            return false;
        }

        /// <summary>
        /// Registers the commands tailored towards a user.
        /// </summary>
        private void RegisterUser()
        {
            this.Register("reload_user_motto", new ReloadUserMottoCommand());
            this.Register("progress_user_achievement", new ProgressUserAchievementCommand());
        }   

        /// <summary>
        /// Registers the commands tailored towards the hotel.
        /// </summary>
        private void RegisterHotel()
        {
            this.Register("reload_bans", new ReloadBansCommand());
            this.Register("reload_quests", new ReloadQuestsCommand());
            this.Register("reload_server_settings", new ReloadServerSettingsCommand());
            this.Register("reload_vouchers", new ReloadVouchersCommand());
            this.Register("reload_ranks", new ReloadRanksCommand());
            this.Register("reload_navigator", new ReloadNavigatorCommand());
            this.Register("reload_items", new ReloadItemsCommand());
            this.Register("reload_catalog", new ReloadCatalogCommand());
            this.Register("reload_filter", new ReloadFilterCommand());
        }

        /// <summary>
        /// Registers a RCON command.
        /// </summary>
        /// <param name="commandText">Text to type for this command.</param>
        /// <param name="command">The command to execute.</param>
        public void Register(string commandText, IRCONCommand command)
        {
            this._commands.Add(commandText, command);
        }
    }
}