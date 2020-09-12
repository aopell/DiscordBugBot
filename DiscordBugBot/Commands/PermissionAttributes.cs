using Discord;
using Discord.Commands;
using DiscordBugBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot.Commands
{
    public class VoterRequiredAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildUser user = context.User as IGuildUser;
            if (user is null) return Task.FromResult(PreconditionResult.FromError("This command needs to be ran in a server"));
            if (user.GuildPermissions.ManageGuild) return Task.FromResult(PreconditionResult.FromSuccess());

            GuildOptions options = DiscordBot.MainInstance.DataStore.GuildOptions.Find(user.GuildId);
            if (options is null) return Task.FromResult(PreconditionResult.FromError("Please run `setup` before using this command"));

            return Task.FromResult(user.RoleIds.Contains(options.VoterRoleId) || user.RoleIds.Contains(options.ModeratorRoleId)
                                       ? PreconditionResult.FromSuccess()
                                       : PreconditionResult.FromError($"This command requires the role `{user.Guild.GetRole(options.VoterRoleId).Name}`"));
        }
    }

    public class ModeratorRequiredAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildUser user = context.User as IGuildUser;
            if (user is null) return Task.FromResult(PreconditionResult.FromError("This command needs to be ran in a server"));
            if (user.GuildPermissions.ManageGuild) return Task.FromResult(PreconditionResult.FromSuccess());

            GuildOptions options = DiscordBot.MainInstance.DataStore.GuildOptions.Find(user.GuildId);
            if (options is null) return Task.FromResult(PreconditionResult.FromError("Please run `setup` before using this command"));

            return Task.FromResult(user.RoleIds.Contains(options.ModeratorRoleId)
                                       ? PreconditionResult.FromSuccess()
                                       : PreconditionResult.FromError($"This command requires the role `{user.Guild.GetRole(options.ModeratorRoleId).Name}`"));
        }
    }
}
