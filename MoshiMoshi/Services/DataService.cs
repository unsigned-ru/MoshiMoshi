using MoshiMoshi.Classes;
using System;
using System.Collections.Generic;

namespace MoshiMoshi.Services
{
    public class DataService
    {
        public Dictionary<ulong, UserAccount> userQueue = new Dictionary<ulong, UserAccount>();
        public Dictionary<ulong, SessionAccount> sessionAccounts = new Dictionary<ulong, SessionAccount>();
        public List<ChatSession> sessions = new List<ChatSession>();

        private readonly IServiceProvider services;

        public DataService(IServiceProvider _services)
        {
            services = _services;
        }
    }
}