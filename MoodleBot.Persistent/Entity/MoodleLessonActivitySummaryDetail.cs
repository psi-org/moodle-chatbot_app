using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleLessonActivitySummaryDetail
    {
        [JsonProperty("userid")]
        public int UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attempts")]
        public List<Attempt> Attempts { get; set; }
    }

    public class Attempt
    {
        [JsonProperty("starttime")]
        public string StartTime { get; set; }

        [JsonProperty("endtime")]
        public string EndTime { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("completed")]
        public bool Completed { get; set; }
    }

    public class MoodleLessonActivitySummaryRequest : MoodleAPIBaseParameter
    {
        public MoodleLessonActivitySummaryRequest(IConfiguration configuration) : base(configuration) { }

        [JsonProperty("lessonid")]
        public long LessonId { get; set; }

        [JsonProperty("completed")]
        public int Completed { get; set; }
    }
}
