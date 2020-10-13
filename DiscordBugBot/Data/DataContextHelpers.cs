using Discord;
using Discord.Commands;
using DiscordBugBot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBugBot.Data
{
    public static class DataContextHelpers
    {
        public static string NicknameOrUsername(this SocketCommandContext ctx, ulong userId) => NicknameOrUsername(ctx.Guild.GetUser(userId));

        public static string NicknameOrUsername(IUser user)
        {
            return user is IGuildUser gu && !string.IsNullOrEmpty(gu.Nickname) ? gu.Nickname : user.Username;
        }
    }
}
