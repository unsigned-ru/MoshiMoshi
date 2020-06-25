using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using MoshiMoshi.Classes;
using System.Linq;

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
            if (dataService.userQueue.Count < 2) return Task.CompletedTask;

            //TODO: find users with similar interests

            dataService.userQueue.Shuffle();

            //pair users in pairs of two
            for (int i = dataService.userQueue.Count; i > 1; i -= 2)
            {
                //get accounts
                UserAccount user1 = dataService.userQueue.ElementAt(i - 1).Value;
                UserAccount user2 = dataService.userQueue.ElementAt(i - 2).Value;

                //create session & remove players from queue.
                ChatSession newSession = new ChatSession(user1, user2, services);
                dataService.sessions.Add(newSession);
                dataService.userQueue.Remove(user1.userID);
                dataService.userQueue.Remove(user2.userID);
                newSession.InitializeChat(); //initialize the chat
            }


            return Task.CompletedTask;
        }


    }
}
