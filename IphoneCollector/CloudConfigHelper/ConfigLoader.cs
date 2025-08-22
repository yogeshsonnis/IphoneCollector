using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IphoneCollector.CloudConfigHelper
{
    public static class ConfigLoader
    {
        public static CloudConfig Load(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<CloudConfig>(json)
                ?? throw new Exception("Failed to load config.");
        }
    }
}
