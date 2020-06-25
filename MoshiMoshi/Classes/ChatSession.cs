using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MoshiMoshi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoshiMoshi.Classes
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

            DataService dataService = services.GetRequiredService<DataService>();
            foreach (SessionAccount sessionAccount in user)
            {
                sessionAccount.userAccount.session = this;
                dataService.sessionAccounts.Add(sessionAccount.userAccount.userID, sessionAccount);
            }
            bindEvents();
        }


        public void InitializeChat()
        {
            //TODO: create embed, some extra info
            Config config = services.GetRequiredService<ConfigService>().config;
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "✅ Session has been set up! ✅",
                Description = "Please keep the session **SFW**, make sure you follow the rules and report those who do not.",
                Fields =
                {
                    new EmbedFieldBuilder() { Name = "Revealing yourself", Value = $"To reveal you discord tag so you can add eachother as friends, you can use the command `{config.prefix}reveal`" },
                    new EmbedFieldBuilder() { Name = "Ending the conversation", Value = $"To end the conversation, you can use command `{config.prefix}end`" },
                    new EmbedFieldBuilder() { Name = "Report", Value = $"[under construction]" },
                    new EmbedFieldBuilder() { Name = "Timer", Value = "After a large time of inactivity the session will automatically end." },
                }
            };
            foreach (SessionAccount sessionAccount in user)
                sessionAccount.userAccount.user.SendMessageAsync("", false, embed.Build());
        }

        public void Destroy(EventArgs e)
        {
            //trigger event
            SessionDestroyed.Invoke(this, e);

            DataService dataService = services.GetRequiredService<DataService>();
            //remove from data list.
            dataService.sessions.Remove(this);
            //remove sessionAccounts from dictionary from
            foreach (SessionAccount sessionAccount in user)
                dataService.sessionAccounts.Remove(sessionAccount.userAccount.userID);

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
            var messageToUpdate = msgBuffer.FirstOrDefault(x => x.Content == $"**{sender.GetDisplayName()}:** {oldMessageContent}");

            //update old content to new content for receiver
            if (messageToUpdate != null) await (messageToUpdate as IUserMessage).ModifyAsync((x) => x.Content = $"**{sender.GetDisplayName()}:** {msg.Content}");
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

        #region events
        private async Task MessageUpdated(Cacheable<IMessage, ulong> oldMessageCasheable, SocketMessage msg, ISocketMessageChannel channel)
        {
            if (channel as IDMChannel == null) return;
            if (user[0].userAccount.IsMessageFromUser(msg)) await RelayUpdateMessageForUser(oldMessageCasheable, msg, user[0], user[1]);
            else if (user[1].userAccount.IsMessageFromUser(msg)) await RelayUpdateMessageForUser(oldMessageCasheable, msg, user[1], user[0]);
        }

        private async Task MessageReceived(SocketMessage msg)
        {
            if (msg.Channel as IDMChannel == null || msg.Content.StartsWith(services.GetRequiredService<ConfigService>().config.prefix)) return;
            if (user[0].userAccount.IsMessageFromUser(msg)) await user[1].userAccount.user.SendMessageAsync($"**{user[0].GetDisplayName()}:** {msg.Content}");
            else if (user[1].userAccount.IsMessageFromUser(msg)) await user[0].userAccount.user.SendMessageAsync($"**{user[1].GetDisplayName()}:** {msg.Content}");
        }
        #endregion events

    }
}