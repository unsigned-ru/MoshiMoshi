using System;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using MoshiMoshi.Classes;
using MoshiMoshi.Services;

namespace MoshiMoshi.Modules
{
    // Modules must be public and inherit from an IModuleBase
    [Summary("Contains commands regarding chatting sessions.")]
    [Remarks("")]
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
        [Summary("Find a partner to talk to.")]
        public Task Call()
        {
            //check if player already in queue.
            //TODO: change on release
            if (false) //dataService.userQueue.ContainsKey(Context.User.Id)
            {
                ReplyAsync("You are already queued for a conversation! [There are " + dataService.userQueue.Count + " queued users]");
            }
            else
            {
                //create UserAccount Instance and add to queue
                dataService.userQueue.Add(Context.User.Id, new UserAccount(Context.User.Id, services));
                ReplyAsync("You have been queued for a conversation...  [There are " + dataService.userQueue.Count + " queued users]");
            }


            return Task.CompletedTask;
        }

        [Command("reveal")]
        [Summary("Reveal you discord tag to your session partner. (only usable in session)")]
        public async Task Reveal()
        {
            if (dataService.userQueue.ContainsKey(Context.User.Id)) return;

            SessionAccount account = dataService.sessionAccounts[Context.User.Id];
            await account.Reveal();
            return;
        }

        [Command("end")]
        [Summary("End the current session with your partner. (only usable in session)")]
        public async Task End()
        {
            if (dataService.userQueue.ContainsKey(Context.User.Id)) return;

            SessionAccount account = dataService.sessionAccounts[Context.User.Id];
            await account.EndSession();

            return;
        }

    }
}