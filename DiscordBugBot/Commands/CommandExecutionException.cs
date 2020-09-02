using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Commands
{
    public class CommandExecutionException : Exception
    {
        public string Title { get; set; }
        public CommandExecutionException(string message) : this("Error Executing Command", message)
        {
        }

        public CommandExecutionException(string title, string message) : base(message)
        {
            Title = title;
        }
    }
}
