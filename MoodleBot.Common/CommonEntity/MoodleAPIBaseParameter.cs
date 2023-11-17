using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MoodleBot.Common.CommonEntity
{
    public class MoodleAPIBaseParameter
    {
        public MoodleAPIBaseParameter(IConfiguration configuration)
        {
            AuthencticationToken = configuration.GetMoodleConfig("AuthToken");
            ResponseFormatType = configuration.GetMoodleConfig("ResponseFormatType");
        }

        [JsonProperty("userid")]
        public long? UserId { get; set; }

        [JsonProperty("wsfunction")]
        public string FunctionName { get; set; }

        [JsonProperty("wstoken")]
        public string AuthencticationToken { get; }

        [JsonProperty("moodlewsrestformat")]
        public string ResponseFormatType { get; }
    }
}
