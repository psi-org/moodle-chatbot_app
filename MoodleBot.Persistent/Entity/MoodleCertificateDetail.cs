using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleCertificateDetail
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("course")]
        public string Course { get; set; }

        [JsonProperty("completiondate")]
        public string CompletionDate { get; set; }

        [JsonProperty("titleposx")]
        public float TitlePosX { get; set; }

        [JsonProperty("titleposy")]
        public float TitlePosY { get; set; }

        [JsonProperty("descriptionposx")]
        public float DescriptionPosX { get; set; }

        [JsonProperty("descriptionposy")]
        public float DescriptionPosY { get; set; }

        [JsonProperty("fullnameposx")]
        public float FullnamePosX { get; set; }

        [JsonProperty("fullnameposy")]
        public float FullnamePosY { get; set; }

        [JsonProperty("courseposx")]
        public float CoursePosX { get; set; }

        [JsonProperty("courseposy")]
        public float CoursePosY { get; set; }

        [JsonProperty("completiondateposx")]
        public float CompletiondatePosX { get; set; }

        [JsonProperty("completiondateposy")]
        public float CompletiondatePosY { get; set; }

        [JsonProperty("activityimg")]
        public string CertificateImage { get; set; }
    }

    public class MoodleCertificateRequest : MoodleAPIBaseParameter
    {
        public MoodleCertificateRequest(IConfiguration configuration) : base(configuration)
        {

        }

        [JsonProperty("courseid")]
        public long CourseId { get; set; }
    }
}
