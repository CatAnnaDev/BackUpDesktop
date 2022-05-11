using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackUpDesktop
{
    internal class ConfigInit
    {
        public static string ConfigPath { get; set; } = "Config.json";
        public static ConfigData Config { get; set; }

        public async Task InitializeAsync()
        {
            var json = string.Empty;

            if (!File.Exists(ConfigPath))
            {
                json = JsonConvert.SerializeObject(GenerateNewConfig(), Formatting.Indented);
                File.WriteAllText("Config.json", json, new UTF8Encoding(false));
                await Task.Delay(200);
            }

            json = File.ReadAllText(ConfigPath, new UTF8Encoding(false));
            Config = JsonConvert.DeserializeObject<ConfigData>(json);
        }

        private static ConfigData GenerateNewConfig() => new ConfigData
        {
            Comment = "Please respect single / or \\, and delete example path, you can add all paths you want and more line too",
            Path = new string[] { "C:\\Users\\%userprofile%\\exemple", "C:\\Windows\\exemple", "C:/Windows/SystemApps/exemple", "", "" },

        };
    }
}
