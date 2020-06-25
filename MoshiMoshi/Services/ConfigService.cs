using MoshiMoshi.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MoshiMoshi.Services
{
    public class ConfigService
    {
        public Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
    }
}
