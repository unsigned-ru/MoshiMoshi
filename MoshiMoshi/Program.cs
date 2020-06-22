using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;
using MoshiMoshi.Services;
using System.IO;

namespace MoshiMoshi
{
    class Program
    {
        public Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));

        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;
               
                await client.LoginAsync(TokenType.Bot, config.token);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                services.GetRequiredService<DataService>();
                await services.GetRequiredService<MatchmakingService>().InitializeAsync();


                // Block this task until the program is closed.
                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<MatchmakingService>()
                .AddSingleton<DataService>()
                .BuildServiceProvider();
        }
    }
}