using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace MoshiMoshi.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");
    }
}