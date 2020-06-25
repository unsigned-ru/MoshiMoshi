using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using MoshiMoshi.Services;
using MoshiMoshi.Classes;

namespace MoshiMoshi
{
    class Program
    {
        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                Config config = services.GetRequiredService<ConfigService>().config;

                //bind events before bot starts
                services.GetRequiredService<EventService>().Initialize();

                await client.LoginAsync(TokenType.Bot, config.token);
                await client.StartAsync();

                //initialize services that need initializiation.
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await services.GetRequiredService<MatchmakingService>().InitializeAsync();

                // Block this task until the program is closed.
                await Task.Delay(-1);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            //create client config
            var config = new DiscordSocketConfig();
            config.MessageCacheSize = 10;

            //build service provider
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>(new DiscordSocketClient(config))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<MatchmakingService>()
                .AddSingleton<DataService>()
                .AddSingleton<ConfigService>()
                .AddSingleton<EventService>()
                .BuildServiceProvider();
        }
    }
}