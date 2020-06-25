using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MoshiMoshi.Services
{
    public class EventService
    {
        IServiceProvider services;
        DiscordSocketClient client;

        public EventService(IServiceProvider _services)
        {
            services = _services;
            client = services.GetRequiredService<DiscordSocketClient>();
        }

        public void Initialize()
        {
            //bind events
            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;
        }


        //event functions
        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }
}
