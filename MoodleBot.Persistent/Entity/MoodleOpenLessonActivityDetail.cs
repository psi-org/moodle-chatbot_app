using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleOpenLessonActivityDetail
    {
        [JsonProperty("startpageid")]
        public long StartPageId { get; set; }
        
        [JsonProperty("lastseenpageid")]
        public long LastSeenPageId { get; set; }
    }

    public class MoodleOpenLessonActivityRequest : MoodleAPIBaseParameter
    {
        public MoodleOpenLessonActivityRequest(IConfiguration configuration) : base(configuration) { }

        [JsonProperty("lessonid")]
        public long LessonId { get; set; }
    }
}
