using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DiscordBugBot;
using DiscordBugBot.Commands;
using DiscordBugBot.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBugBot.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private readonly DiscordBot bot;
        private readonly IServiceProvider mainServiceProvider;

        public CommandHandler(DiscordSocketClient client, DiscordBot bot)
        {
            this.client = client;
            this.bot = bot;
            this.mainServiceProvider = bot.MainProvider;
            this.commands = this.mainServiceProvider.GetRequiredService<CommandService>();
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // We use a scoped service for the database - create a scope here.
            // I'm not sure exactly why Discord.NET needs to instantiate modules
            // at this time, but modules are reinstantiated upon command invocation.
            // Thus, use a scope. If something leaks, it will at least be disposed,
            // so it won't be as severe of a leak and if it's used it Should(TM)
            // throw a nice ObjectDisposedException instead of quieter behavior.
            using (var scope = mainServiceProvider.CreateScope())
            {
                await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                                services: scope.ServiceProvider);
            }
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            if (!(messageParam is SocketUserMessage message)) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('+', ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            using (var scope = mainServiceProvider.CreateScope())
            {

                // Create a WebSocket-based command context based on the message
                var context = new SocketCommandContext(client, message);

                // Execute the command with the command context we just
                // created, along with the service provider for precondition checks.

                // Keep in mind that result does not indicate a return value
                // rather an object stating if the command executed successfully.
                var result = await commands.ExecuteAsync(
                    context: context,
                    argPos: argPos,
                    services: scope.ServiceProvider);

                // Optionally, we may inform the user if the command fails
                // to be executed; however, this may not always be desired,
                // as it may clog up the request queue should a user spam a
                // command.
                if (!result.IsSuccess)
                {
                    EmbedBuilder embed = new EmbedBuilder();
                    embed.WithColor(Color.Red);

                    switch (result.Error)
                    {
                        case CommandError.Exception when result is ExecuteResult eResult:
                            await ExceptionMessageHelper.HandleException(eResult.Exception, message.Channel);
                            return;
                        case CommandError.ParseFailed:
                        case CommandError.BadArgCount:
                            var c = commands.Search(context, argPos).Commands.FirstOrDefault().Command;
                            string name = c.Name;
                            var module = c.Module;
                            while (module != null)
                            {
                                if (!string.IsNullOrEmpty(module.Group))
                                {
                                    name = module.Group + " " + name;
                                }
                                module = module.Parent;
                            }
                            embed.WithTitle("Incorrect Command Usage");
                            embed.WithDescription($"Error parsing command. Run `+help {name}` for more information.");
                            break;
                        case CommandError.UnmetPrecondition:
                            embed.WithTitle("Error Executing Command");
                            embed.WithDescription(result.ErrorReason == "" ? "You do not have permission to use this command here." : result.ErrorReason);
                            break;
                        case CommandError.UnknownCommand:
                            // Do nothing
                            return;
                        default:
                            embed.WithTitle("Error Executing Command");
                            embed.WithColor(Color.Red);
                            embed.WithDescription(result.ErrorReason);
                            break;
                    }

                    await context.Channel.SendMessageAsync(embed: embed.Build());
                }
            }

        }
    }
}
