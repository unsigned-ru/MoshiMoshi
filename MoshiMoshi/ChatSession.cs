using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoshiMoshi
{
    public class ChatSession
    {
        public UserAccount user1, user2;
        private IServiceProvider services;
        public ChatSession(UserAccount _user1, UserAccount _user2, IServiceProvider _services)
        {
            user1 = _user1;
            user2 = _user2;
            services = _services;
            Console.WriteLine($"Session created: {_user1.user.Username} && {_user2.user.Username}");
        }

        public void InitializeChat()
        {
            DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();

            user1.user.SendMessageAsync($"Your session with anon has been set up.");
            user2.user.SendMessageAsync($"Your session with anon has been set up.");
        }

        public async Task ForwardMessage(SocketUserMessage msg)
        {
            if (msg.Author.Id == user1.userID) 
            {
                //user 1 is sender, user 2 is receiver
                await user2.user.SendMessageAsync($"__**anon:**__ {msg.Content}");
            }
            else if (msg.Author.Id == user2.userID) 
            {
                //user 2 is sender, user 1 is receiver
                await user1.user.SendMessageAsync($"__**anon:**__ {msg.Content}");
            }
        }
    }
}
