using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;

namespace MoodleBot.Persistent.Entity
{
    public class MoodlePostFeedbackAnswerDetail
    {
        [JsonProperty("endmessage")]
        public string EndMessage { get; set; }

        [JsonProperty("islastquestion")]
        public bool IsLastQuestion { get; set; }
    }

    public class MoodlePostFeedbackAnswerRequest : MoodleAPIBaseParameter
    {
        public MoodlePostFeedbackAnswerRequest(IConfiguration configuration) : base(configuration) { } 

        [JsonProperty("itemid")]
        public long Itemid { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
