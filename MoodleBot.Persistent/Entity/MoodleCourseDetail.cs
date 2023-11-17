using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;
using System;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleCourseDetail
    {
        [JsonProperty("courseid")]
        public int CourseId { get; set; }

        [JsonProperty("coursecategoryid")]
        public string CoursecategoryId { get; set; }

        [JsonProperty("sortorder")]
        public int SortOrder { get; set; }

        [JsonProperty("shortname")]
        public string ShortName { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("courseimg")]
        public string CourseImage { get; set; }

        [JsonProperty("userid")]
        public long? UserId { get; set; }

        [JsonProperty("coursestatusid")]
        public int CourseStatusId { get; set; }

        [JsonProperty("coursestatus")]
        public string CourseStatus { get; set; }

        [JsonProperty("enrol")]
        public string Enrol { get; set; }

        [JsonProperty("courseprogress")]
        public decimal? CourseProgress { get; set; }

        [JsonProperty("rawgrade")]
        public decimal? Rawgrade { get; set; }

        [JsonProperty("percentagegrade")]
        public decimal? PercentageGrade { get; set; }

        [JsonProperty("gradepass")]
        public decimal? GradePass { get; set; }

        [JsonProperty("enrolledtime")]
        public DateTime? EnrolledTime { get; set; }

        [JsonProperty("timestarted")]
        public DateTime? TimeStarted { get; set; }

        [JsonProperty("timecompleted")]
        public DateTime? TimeCompleted { get; set; }
    }

    public class MoodleCourseRequest : MoodleAPIBaseParameter
    {
        public MoodleCourseRequest(IConfiguration configuration) : base(configuration)
        {

        }
        [JsonProperty("courseid")]
        public long? CourseId { get; set; }
    }
}
