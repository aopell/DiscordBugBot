using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBugBot.Commands;
using DiscordBugBot.Config;
using DiscordBugBot.Data;
using DiscordBugBot.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot
{
    public class DiscordBot
    {
        public static DiscordBot MainInstance = null;
        public DiscordSocketClient Client { get; private set; }
        public Secret Secret { get; private set; }
        public Options Options { get; private set; }
        public IDataStore DataStore { get; } = new LiteDbDataStore("issues.db");

        public static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            MainInstance = new DiscordBot();

            ConfigFileManager.LoadConfigFiles(MainInstance);
            MainInstance.Client = new DiscordSocketClient();

            MainInstance.Client.Log += MainInstance.Log;
            MainInstance.Client.Ready += MainInstance.Client_Ready;
            MainInstance.Client.ReactionAdded += MainInstance.Client_ReactionAdded;
            MainInstance.Client.MessageReceived += MainInstance.Client_MessageReceived;

            if (MainInstance.Secret?.Token is null)
            {
                throw new ArgumentNullException("Token", "Please specify a token in the secret.json file");
            }

            await MainInstance.Client.LoginAsync(TokenType.Bot, MainInstance.Secret.Token);
            await MainInstance.Client.StartAsync();

            var ch = new CommandHandler(
                MainInstance.Client,
                new CommandService(
                    new CommandServiceConfig()
                    {
                        CaseSensitiveCommands = false,
                        LogLevel = LogSeverity.Info
                    }
                ),
                MainInstance
            );

            await ch.InstallCommandsAsync();

            await Task.Delay(-1);
        }

        private Task Client_MessageReceived(SocketMessage message)
        {
            _ = IssueInlineMentionHelper.ProcessMessageText(message);
            return Task.CompletedTask;
        }

        private Task Client_Ready()
        {
            return Task.CompletedTask;
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = await cachedMessage.GetOrDownloadAsync();
            _ = ReactionMessageHelper.HandleReactionMessage(channel, Client.CurrentUser, reaction, message);
            _ = IssueConfirmationHelper.HandleMessageReaction(channel, reaction, message);
            _ = IssueLogHelper.HandleLogMessageReaction(channel, reaction, reaction.User.IsSpecified ? reaction.User.Value : null, message);
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            // Called on program close, should log this somewhere?
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Called on unhandled exception, should log this somewhere
        }
    }
}
