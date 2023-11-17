using Microsoft.Extensions.Configuration;

namespace MoodleBot.Common
{
    public static class ConfigurationExtensions
    {
        public static string Database(this IConfiguration config)
        {
            return config.GetConnectionString("DefaultConnection");
        }

        #region Twilio Configuration
        public static string GetTwilioConfig(this IConfiguration config, string configName)
        {
            return config[configName];
        }
        #endregion

        #region Moodle Configuration
        public static string GetMoodleAPIConfig(this IConfiguration config, string configName)
        {
            return config[$"Moodle:MoodleAPI:{configName}"];
        }
        public static string GetMoodleConfig(this IConfiguration config, string configName)
        {
            return config[$"Moodle:{configName}"];
        }

        public static string GetMoodleMessageEmojis(this IConfiguration config, string configName)
        {
            return config[$"Moodle:MoodleMessage:{configName}"];
        }
        
        public static string GetRootConfig(this IConfiguration config, string configName)
        {
            return config[$"{configName}"];
        }
        #endregion

        #region Blob Storage
        public static string GetBlobStorageConfig(this IConfiguration config, string configName)
        {
            return config[$"BlobStorage:{configName}"];
        }
        #endregion
    }
}
