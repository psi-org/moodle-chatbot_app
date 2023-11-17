using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleFeedbackQuestionDetail
    {
        [JsonProperty("activitytitle")]
        public string ActivityTitle { get; set; }

        [JsonProperty("activityimg")]
        public string ActivityImage { get; set; }

        [JsonProperty("itemid")]
        public int? QuestionId { get; set; }

        [JsonProperty("itemname")]
        public string FeedbackQuestion { get; set; }

        [JsonProperty("itemposition")]
        public string ItemPosition { get; set; }

        [JsonProperty("itemlabel")]
        public string ItemLabel { get; set; }

        [JsonProperty("itemtype")]
        public string ItemType { get; set; }

        [JsonProperty("completed")]
        public bool Completed { get; set; }

        [JsonProperty("answers")]
        public List<Choices> Choices { get; set; }
    }
    public class Choices
    {
        [JsonProperty("answer")]
        public string Choice { get; set; }
    }

    public class MoodleFeedbackQuestionDetailRequest : MoodleAPIBaseParameter
    {
        public MoodleFeedbackQuestionDetailRequest(IConfiguration configuration) : base(configuration) { } 

        [JsonProperty("feedbackid")]
        public long FeedbackId { get; set; }
    }
}
