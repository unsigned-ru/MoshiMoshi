using System;
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
        DataService _dataService;

        public ChattingModule(IServiceProvider _services)
        {
            services = _services;
            _dataService = _services.GetRequiredService<DataService>();
        }

        [Command("call")]
        public Task Call()
        {
            ////check if player already in queue.
            //if (_dataService.playerQueue.Exists(x => x.userID == Context.User.Id))
            //{
            //    ReplyAsync("You are already queued for a conversation! [There are " + _dataService.playerQueue.Count + " queued ppl]");
            //}
            //else
            //{
                
            //}
            //create UserAccount Instance and add to queue
            _dataService.playerQueue.Add(new UserAccount(Context.User.Id, services));
            ReplyAsync("You have been queued for a conversation...  [There are " + _dataService.playerQueue.Count + " queued ppl]");

            return Task.CompletedTask;
        }
    }
}