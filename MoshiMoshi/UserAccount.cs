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
        //interests

        public UserAccount(ulong _userID, IServiceProvider services)
        {
            userID = _userID;
            user = services.GetRequiredService<DiscordSocketClient>().GetUser(userID);
            //load data from database...
        }

        public bool CheckIsInSession(List<ChatSession> sessions)
        {
            return sessions.Exists(x => x.user1 == this || x.user2 == this);
        }
    }
}
