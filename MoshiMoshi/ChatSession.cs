using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MoshiMoshi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoshiMoshi
{
    public class ChatSession
    {
        public event EventHandler SessionDestroyed;
        public SessionAccount user1, user2;

        private IServiceProvider services;


        public ChatSession(UserAccount _user1, UserAccount _user2, IServiceProvider _services)
        {
            user1 = new SessionAccount(ref _user1);
            user2 = new SessionAccount(ref _user2);
            services = _services;

            //bind events for session.
            var client = _services.GetRequiredService<DiscordSocketClient>();
            client.MessageReceived += MessageReceived;
            client.MessageUpdated += MessageUpdated;
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> oldMessageCasheable, SocketMessage msg, ISocketMessageChannel channel)
        {
            
            if (user1.userAccount.IsMessageFromUser(msg)) await RelayUpdateMessageForUser(oldMessageCasheable, msg, user1, user2);
            else if (user2.userAccount.IsMessageFromUser(msg)) await RelayUpdateMessageForUser(oldMessageCasheable, msg, user2, user1);
        }

        private async Task MessageReceived(SocketMessage msg)
        {
            if (user1.userAccount.IsMessageFromUser(msg)) await user2.userAccount.user.SendMessageAsync($"__**{user1.GetDisplayName()}:**__ {msg.Content}");
            else if (user2.userAccount.IsMessageFromUser(msg)) await user1.userAccount.user.SendMessageAsync($"__**{user1.GetDisplayName()}:**__ {msg.Content}");
        }

        public void InitializeChat()
        {
            user1.userAccount.user.SendMessageAsync($"Your session with anon has been set up.");
            user2.userAccount.user.SendMessageAsync($"Your session with anon has been set up.");
        }

        public void Destroy(EventArgs e)
        {
            //trigger event
            SessionDestroyed.Invoke(this, e);
            //remove from data list.
            services.GetRequiredService<DataService>().sessions.Remove(this);
        }

        private async Task RelayUpdateMessageForUser(Cacheable<IMessage, ulong> oldMessageCacheable, SocketMessage msg, SessionAccount sender, SessionAccount receiver)
        {
            //get required services
            var client = services.GetRequiredService<DiscordSocketClient>();
            Config config = services.GetRequiredService<ConfigService>().config;

            //get old message content
            string oldMessageContent = oldMessageCacheable.HasValue ? oldMessageCacheable.Value.Content : "";

            //get DM channel of receiver
            var DMChannel = await receiver.userAccount.user.GetOrCreateDMChannelAsync();

            //find message with old content in receiver messages
            var msgBuffer = await DMChannel.GetMessagesAsync(config.sessionMessageUpdateLimit).FlattenAsync();
            var messageToUpdate = msgBuffer.FirstOrDefault(x => x.Content == $"__**{sender.GetDisplayName()}:**__ {oldMessageContent}");

            //update old content to new content for receiver
            if (messageToUpdate != null) await (messageToUpdate as IUserMessage).ModifyAsync((x) => x.Content = $"__**{sender.GetDisplayName()}:**__ {msg.Content}");
        }
    }
}
