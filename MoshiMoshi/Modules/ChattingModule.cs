using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using MoshiMoshi.Services;

namespace MoshiMoshi.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class ChattingModule : ModuleBase<SocketCommandContext>
    {
        IServiceProvider services;
        DataService dataService;

        public ChattingModule(IServiceProvider _services)
        {
            services = _services;
            dataService = _services.GetRequiredService<DataService>();
        }

        [Command("call")]
        public Task Call()
        {
            //check if player already in queue.
            if (dataService.playerQueue.Exists(x => x.userID == Context.User.Id))
            {
                ReplyAsync("You are already queued for a conversation! [There are " + dataService.playerQueue.Count + " queued ppl]");
            }
            else
            {
                //create UserAccount Instance and add to queue
                dataService.playerQueue.Add(new UserAccount(Context.User.Id, services));
                ReplyAsync("You have been queued for a conversation...  [There are " + dataService.playerQueue.Count + " queued ppl]");
            }


            return Task.CompletedTask;
        }

        [Command("reveal")]
        public async Task Reveal()
        {
            ChatSession session = dataService.sessions.Find(x => x.user.FirstOrDefault(y => y.userAccount.userID == Context.User.Id) != null);

            if (session == null) return;

            await session.user.FirstOrDefault(x => x.userAccount.userID == Context.User.Id).Reveal();

            return;
        }

        [Command("end")]
        public async Task End()
        {
            ChatSession session = dataService.sessions.Find(x => x.user.FirstOrDefault(y => y.userAccount.userID == Context.User.Id) != null);

            if (session == null) return;

            await session.user.FirstOrDefault(x => x.userAccount.userID == Context.User.Id).EndSession();

            return;
        }

    }
}