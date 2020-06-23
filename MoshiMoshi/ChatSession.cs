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
        public SessionAccount[] user;

        private IServiceProvider services;


        public ChatSession(UserAccount _user1, UserAccount _user2, IServiceProvider _services)
        {
            services = _services;

            user = new SessionAccount[] { new SessionAccount(ref _user1), new SessionAccount(ref _user2) };
            foreach (SessionAccount sessionAccount in user) sessionAccount.userAccount.session = this;

            bindEvents();
        }

        private void bindEvents()
        {
            //bind events for session.
            var client = services.GetRequiredService<DiscordSocketClient>();
            client.MessageReceived += MessageReceived;
            client.MessageUpdated += MessageUpdated;
        }
        private void unbindEvents()
        {
            //bind events for session.
            var client = services.GetRequiredService<DiscordSocketClient>();
            client.MessageReceived -= MessageReceived;
            client.MessageUpdated -= MessageUpdated;
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> oldMessageCasheable, SocketMessage msg, ISocketMessageChannel channel)
        {
            if (channel as IDMChannel == null) return;
            if (user[0].userAccount.IsMessageFromUser(msg)) await RelayUpdateMessageForUser(oldMessageCasheable, msg, user[0], user[1]);
            else if (user[1].userAccount.IsMessageFromUser(msg)) await RelayUpdateMessageForUser(oldMessageCasheable, msg, user[1], user[0]);
        }

        private async Task MessageReceived(SocketMessage msg)
        {
            if (msg.Channel as IDMChannel == null || msg.Content.StartsWith(services.GetRequiredService<ConfigService>().config.prefix)) return;
            if (user[0].userAccount.IsMessageFromUser(msg)) await user[1].userAccount.user.SendMessageAsync($"__**{user[0].GetDisplayName()}:**__ {msg.Content}");
            else if (user[1].userAccount.IsMessageFromUser(msg)) await user[0].userAccount.user.SendMessageAsync($"__**{user[1].GetDisplayName()}:**__ {msg.Content}");
        }

        public void InitializeChat()
        {
            foreach (SessionAccount sessionAccount in user) 
                sessionAccount.userAccount.user.SendMessageAsync($"**✅ Your session with anon has been set up. ✅**");
        }

        public void Destroy(EventArgs e)
        {
            //trigger event
            SessionDestroyed.Invoke(this, e);
            //remove from data list.
            services.GetRequiredService<DataService>().sessions.Remove(this);
            unbindEvents();
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