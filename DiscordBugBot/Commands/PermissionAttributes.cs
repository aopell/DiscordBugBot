using Discord;
using Discord.Commands;
using DiscordBugBot.Data;
using DiscordBugBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBugBot.Commands
{
    public class VoterRequiredAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildUser user = context.User as IGuildUser;
            if (user is null) return PreconditionResult.FromError("This command needs to be ran in a server");
            if (user.GuildPermissions.ManageGuild) return PreconditionResult.FromSuccess();

            GuildOptions options = await services.GetRequiredService<BugBotDataContext>().GuildOptions.FindAsync(user.GuildId);
            if (options is null) return PreconditionResult.FromError("Please run `setup` before using this command");

            return user.RoleIds.Contains(options.VoterRoleId) || user.RoleIds.Contains(options.ModeratorRoleId)
                       ? PreconditionResult.FromSuccess()
                       : PreconditionResult.FromError($"This command requires the role `{user.Guild.GetRole(options.VoterRoleId).Name}`");
        }
    }

    public class ModeratorRequiredAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildUser user = context.User as IGuildUser;
            if (user is null) return PreconditionResult.FromError("This command needs to be ran in a server");
            if (user.GuildPermissions.ManageGuild) return PreconditionResult.FromSuccess();

            GuildOptions options = await services.GetRequiredService<BugBotDataContext>().GuildOptions.FindAsync(user.GuildId);
            if (options is null) return PreconditionResult.FromError("Please run `setup` before using this command");

            return user.RoleIds.Contains(options.ModeratorRoleId)
                        ? PreconditionResult.FromSuccess()
                        : PreconditionResult.FromError($"This command requires the role `{user.Guild.GetRole(options.ModeratorRoleId).Name}`");
        }
    }
}
