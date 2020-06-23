using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MoshiMoshi.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoshiMoshi
{
    public class UserAccount
    {
        public readonly ulong userID;
        public readonly IUser user;
        public IServiceProvider services;


        private ChatSession session
        {
            get
            {
                return this.session;
            }
            set
            {
                this.session = value;
                value.SessionDestroyed += SessionDestroyed;
            }
        }

        private void SessionDestroyed(object sender, EventArgs e)
        {
            session = null;
        }

        //interests

        public UserAccount(ulong _userID, IServiceProvider _services)
        {
            services = _services;
            userID = _userID;
            user = _services.GetRequiredService<DiscordSocketClient>().GetUser(userID);
            
            //TODO: load data from database...
        }

        public bool CheckIsInSession(List<ChatSession> sessions)
        {
            return sessions.Exists(x => x.user1.userAccount == this || x.user2.userAccount == this);
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
            if (hasRevealed) return userAccount.user.Username;
            else return userAccount.services.GetRequiredService<ConfigService>().config.anonDisplayName;
        }
    }

}
