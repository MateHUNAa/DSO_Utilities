using Newtonsoft.Json;
using System;
using System.IO;


namespace DSO_Utilities.Config
{
    public class ConfigManager
    {
        private static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public static ConfigData Load()
        {
            if (!File.Exists(ConfigPath))
                return new ConfigData();

            try
            {
                string json = File.ReadAllText(ConfigPath);
                return JsonConvert.DeserializeObject<ConfigData>(json) ?? new ConfigData();
            } catch
            {
                return new ConfigData();
            }
        }

        public static void Save(ConfigData data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
