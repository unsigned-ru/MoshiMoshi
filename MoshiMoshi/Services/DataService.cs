using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;

namespace MoshiMoshi.Services
{
    public class DataService
    {
        public List<UserAccount> playerQueue = new List<UserAccount>();
        public List<ChatSession> sessions = new List<ChatSession>();
        private readonly IServiceProvider services;

        public DataService(IServiceProvider _services)
        {
            services = _services;


        }
    }
}