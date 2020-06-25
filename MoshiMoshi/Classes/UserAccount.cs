using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MoshiMoshi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoshiMoshi.Classes
{
    public class UserAccount
    {
        public readonly ulong userID;
        public readonly IUser user;
        public IServiceProvider services;

        private ChatSession _session;
        public ChatSession session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;
                if (value != null) _session.SessionDestroyed += SessionDestroyed;
            }
        }

        public UserAccount(ulong _userID, IServiceProvider _services)
        {
            services = _services;
            userID = _userID;
            user = _services.GetRequiredService<DiscordSocketClient>().GetUser(userID);

            //TODO: load data from database...
        }

        private void SessionDestroyed(object sender, EventArgs e)
        {
            session = null;
        }

        public bool CheckIsInSession(List<ChatSession> sessions)
        {
            return sessions.Exists(x => x.user[0].userAccount == this || x.user[1].userAccount == this);
        }

        public bool IsMessageFromUser(SocketMessage msg)
        {
            return msg.Author.Id == userID;
        }

    }

    public class SessionAccount
    {
        public UserAccount userAccount;

        public bool hasRevealed = false;

        public SessionAccount(ref UserAccount _userAccount)
        {
            userAccount = _userAccount;
        }

        public string GetDisplayName()
        {
            if (hasRevealed) return userAccount.user.ToString();
            else return userAccount.services.GetRequiredService<ConfigService>().config.anonDisplayName;
        }

        public async Task Reveal()
        {
            hasRevealed = true;
            await userAccount.user.SendMessageAsync("**⚠️ You have revealed your discord username. ⚠️**");

            await GetOtherAccount().userAccount.user.SendMessageAsync($"**⚠️ Anon has revealed their discord tag: __{userAccount.user}__ ⚠️**");
        }

        public async Task EndSession()
        {
            await userAccount.user.SendMessageAsync("**❌ You have closed the session. ❌**");

            var otherAccount = GetOtherAccount();
            await otherAccount.userAccount.user.SendMessageAsync($"**❌ {GetDisplayName()} has ended the session.❌**");

            userAccount.session.Destroy(EventArgs.Empty);
        }

        public SessionAccount GetOtherAccount()
        {
            return userAccount.session.user.FirstOrDefault(x => x != this);
        }
        
    }
}