using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBugBot.Models
{
    [NamedArgumentType]
    public class IssueUpdateArgs
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IUser Assignee { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }

        /// <summary>
        /// Indicates actively removing a user. We cannot use null as that indicates an unspecified argument.
        /// </summary>
        public static IUser NoUser => NoUserStub.Instance;

        private class NoUserStub : IUser
        {
            public static NoUserStub Instance { get; } = new NoUserStub();

            public string AvatarId => throw new NotImplementedException();

            public string Discriminator => throw new NotImplementedException();

            public ushort DiscriminatorValue => throw new NotImplementedException();

            public bool IsBot => throw new NotImplementedException();

            public bool IsWebhook => throw new NotImplementedException();

            public string Username => throw new NotImplementedException();

            public DateTimeOffset CreatedAt => throw new NotImplementedException();

            public ulong Id => throw new NotImplementedException();

            public string Mention => throw new NotImplementedException();

            public IActivity Activity => throw new NotImplementedException();

            public UserStatus Status => throw new NotImplementedException();

            public IImmutableSet<ClientType> ActiveClients => throw new NotImplementedException();

            private NoUserStub() { }

            public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            {
                throw new NotImplementedException();
            }

            public string GetDefaultAvatarUrl()
            {
                throw new NotImplementedException();
            }

            public Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null)
            {
                throw new NotImplementedException();
            }
        }
    }
}
