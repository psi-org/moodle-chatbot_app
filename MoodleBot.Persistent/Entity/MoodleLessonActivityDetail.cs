using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using MoodleBot.Common.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleLessonActivityDetail
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("contents")]
        public List<Content> Contents { get; set; }

        [JsonProperty("nextpageid")]
        public long NextPageId { get; set; }

        [JsonProperty("currentpage")]
        public long CurrentPage { get; set; }
    }

    public class Content
    {
        [JsonProperty("page_content")]
        public string PageContent { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("typeid")]
        public LessonContentType TypeId { get; set; }
        
        [JsonProperty("position")]
        public int Position { get; set; }
    }

    public class MoodleLessonActivityRequest : MoodleAPIBaseParameter
    {
        public MoodleLessonActivityRequest(IConfiguration configuration) : base(configuration) { }

        [JsonProperty("pageid")]
        public long PageId { get; set; }
        
        [JsonProperty("currentpage")]
        public long CurrentPage { get; set; }
    }
}
