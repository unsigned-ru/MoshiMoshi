using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace MoshiMoshi.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [Summary("Contains the general use commands.")]
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {
        IServiceProvider services;
        CommandService commandService;

        public GeneralModule(IServiceProvider _services)
        {
            services = _services;
            commandService = services.GetRequiredService<CommandService>();
        }

        [Command("ping")]
        [Alias("pong", "hello")]
        [Summary("A generic test command.")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("help")]
        [Summary("Displays a list of available commands")]
        public async Task Help()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder()
            {
                Title = "Available command modules",
                Footer = new EmbedFooterBuilder() { Text = "To see the commands execute '!help <ModuleName>'" }
            };

            foreach (ModuleInfo module in commandService.Modules)
            {
                // Get the module Summary attribute information
                string embedFieldText = module.Summary ?? "No description available\n";
                embedBuilder.AddField($"**{module.Name}**", embedFieldText, true);
            }

            await ReplyAsync("", false, embedBuilder.Build());
        }
        [Command("help")]
        [Priority(1)]
        public async Task Help(string module)
        {
            //find module
            ModuleInfo targetModule = commandService.Modules.FirstOrDefault(x => x.Name.ToLower().Replace("module","") == module.ToLower().Replace("module", ""));
            if (targetModule == null)
            {
                await ReplyAsync("Could not find a module with that name.");
                return;
            }
             

            EmbedBuilder embedBuilder = new EmbedBuilder()
            {
                Title = $"{targetModule.Name} commands",
            };

            //only get commands of priority 0 to avoid duplicates because of overloading;
            foreach (CommandInfo command in targetModule.Commands.Where(x => x.Priority == 0))
            {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";
                embedBuilder.AddField($"**{command.Name}**", embedFieldText, true);
            }

            await ReplyAsync("", false, embedBuilder.Build());
        }
    }


}