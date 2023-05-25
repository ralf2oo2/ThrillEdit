using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using ThrillEdit.BusinessLayer.Models;

namespace ThrillEdit.BusinessLayer
{
    public class ApplicationSettings
    {
        public Settings GetApplicationSettings()
        {
            if(!File.Exists("settings.json"))
            {
                return new Settings();
            }
            string json = File.ReadAllText("settings.json");
            Settings settings = JsonSerializer.Deserialize<Settings>(json);
            return settings;
        }

        public async Task SaveApplicationSettings(Settings settings)
        {
            await using FileStream createStream = File.Create("settings.json");
            await JsonSerializer.SerializeAsync(createStream, settings);
        }
    }
}
