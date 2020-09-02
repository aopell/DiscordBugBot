using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Config
{
    [ConfigFile("Config/secret.json")]
    public class Secret : Config
    {
        public string Token { get; set; }
    }
}
