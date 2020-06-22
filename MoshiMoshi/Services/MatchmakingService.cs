using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

namespace MoshiMoshi.Services
{
    public class MatchmakingService
    {
        readonly IServiceProvider services;
        DataService dataService;

        public MatchmakingService(IServiceProvider _services)
        {
            services = _services;
            dataService = services.GetRequiredService<DataService>();
        }

        public Task InitializeAsync()
        {
            var timer = new System.Timers.Timer(5 * 1000);
            timer.Elapsed += async (sender, e) => await Matchmaking();
            timer.Start();
            return Task.CompletedTask;
        } 
            
        Task Matchmaking()
        {
            Console.WriteLine("Matchmaking called.");
            //check if atleast two players are queued.
            if (dataService.playerQueue.Count < 2) return Task.CompletedTask;

            //shuffle the queue
            Utils.Shuffle(dataService.playerQueue);

            //TODO: find users with similar interests

            //pair users in pairs of two
            for (int i = dataService.playerQueue.Count; i > 1; i -= 2)
            {
                //get accounts
                UserAccount player1 = dataService.playerQueue[i-1];
                UserAccount player2 = dataService.playerQueue[i-2];

                //create session & remove players from queue.
                ChatSession newSession = new ChatSession(player1, player2, services);
                dataService.sessions.Add(newSession);
                dataService.playerQueue.Remove(player1);
                dataService.playerQueue.Remove(player2);
                newSession.InitializeChat(); //initialize the chat
            }
            

            return Task.CompletedTask;
        }


    }
}
