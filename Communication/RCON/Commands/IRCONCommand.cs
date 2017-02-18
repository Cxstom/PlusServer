using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.RCON.Commands
{
    public interface IRCONCommand
    {
        string Parameters { get; }
        string Description { get; }
        bool TryExecute(string[] parameters);
    }
}