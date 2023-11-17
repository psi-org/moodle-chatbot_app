using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleActivityDetail
    {
        [JsonProperty("courseactivityid")]
        public int CourseActivityId { get; set; }

        [JsonProperty("instanceid")]
        public int InstanceId { get; set; }

        [JsonProperty("courseid")]
        public long CourseId { get; set; }

        [JsonProperty("courseshortname")]
        public string CourseShortName { get; set; }

        [JsonProperty("coursefullname")]
        public string CourseFullName { get; set; }

        [JsonProperty("sectionid")]
        public int SectionId { get; set; }

        [JsonProperty("sectionname")]
        public string SectionName { get; set; }

        [JsonProperty("activityid")]
        public int ActivityId { get; set; }

        [JsonProperty("activitytypeid")]
        public int ActivitytypeId { get; set; }

        [JsonProperty("activitytype")]
        public string ActivityType { get; set; }

        [JsonProperty("activityvisible")]
        public int ActivityVisible { get; set; }

        [JsonProperty("activityname")]
        public string ActivityName { get; set; }

        [JsonProperty("activityimg")]
        public string ActivityImg { get; set; }
        
        [JsonProperty("activitydescription")]
        public string ActivityDescription { get; set; }

        [JsonProperty("activitysequencelist")]
        public string ActivitySequenceList { get; set; }

        [JsonProperty("isfirstactivity")]
        public bool IsFirstActivity { get; set; }

        [JsonProperty("islastactivity")]
        public bool IsLastActivity { get; set; }

        [JsonProperty("isfirstsectionactivity")]
        public bool IsFirstSectionActivity { get; set; }

        [JsonProperty("islastsectionactivity")]
        public bool IsLastSectionActivity { get; set; }

        [JsonProperty("activityviewed")]
        public int ActivityViewed { get; set; }

        [JsonProperty("userid")]
        public long? UserId { get; set; }

        [JsonProperty("activitycompletionsstatusid")]
        public int ActivityCompletionsStatusId { get; set; }

        [JsonProperty("activitycompletionstatus")]
        public string ActivityCompletionStatus { get; set; }

        [JsonProperty("activitygrademax")]
        public double? ActivityGradeMax { get; set; }

        [JsonProperty("activitygradepass")]
        public double? ActivityGradePass { get; set; }

        [JsonProperty("activitygradetype")]
        public int? ActivityGradeType { get; set; }

        [JsonProperty("activityrawgrade")]
        public double? ActivityRawGrade { get; set; }

        [JsonProperty("activitypercentagegrade")]
        public double ActivityPercentageGrade { get; set; }

        [JsonProperty("lastattemptid")]
        public int? LastAttemptId { get; set; }

        [JsonProperty("lastattemptstatte")]
        public string LastAttemptState { get; set; }

        [JsonProperty("numofattempts")]
        public int? NumOfAttempts { get; set; }

        [JsonProperty("activityattempts")]
        public List<Activityattempt> ActivityAttempts { get; set; }

        [JsonProperty("activitycompletiontimemodified")]
        public DateTime? ActivityCompletionTimeModified { get; set; }

        [JsonProperty("activitygradetimecreated")]
        public DateTime? Activitygradetimecreated { get; set; }

        [JsonProperty("activitygradetimemodified")]
        public DateTime? Activitygradetimemodified { get; set; }
        
        [JsonProperty("accessrestriction")]
        public bool IsAccessRestriction { get; set; }
        
        [JsonProperty("accessrestrictionmessage")]
        public string AccessRestrictionMessage { get; set; }

        [JsonProperty("accessrestrictionid")]
        public List<ActivityRestrictionId> AccessRestrictionId { get; set; }
        
        [JsonProperty("showcertificate")]
        public bool ShouldShowCertificate { get; set; }
    }

    public class Activityattempt
    {
        [JsonProperty("quizid")]
        public long QuizId { get; set; }

        [JsonProperty("quizattemptid")]
        public int QuizattemptId { get; set; }

        [JsonProperty("userid")]
        public int UserId { get; set; }

        [JsonProperty("attempt")]
        public int Attempt { get; set; }

        [JsonProperty("uniqueid")]
        public int UniqueId { get; set; }

        [JsonProperty("currentpage")]
        public int Currentpage { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("sumgrades")]
        public double? Sumgrades { get; set; }

        [JsonProperty("timestarted")]
        public DateTime TimeStarted { get; set; }

        [JsonProperty("timefinished")]
        public DateTime TimeFinished { get; set; }
    }

    public class ActivityRestrictionId
    {
        [JsonProperty("cmid")]
        public long ActivityId { get; set; }
    }

    public class MoodleActivityRequest : MoodleAPIBaseParameter
    {
        public MoodleActivityRequest(IConfiguration configuration) : base(configuration)
        {

        }

        [JsonProperty("courseid")]
        public long? CourseId { get; set; }

        [JsonProperty("courseactivityid")]
        public long? CourseActivityId { get; set; }

        [JsonProperty("activityid")]
        public long? ActivityId { get; set; }
    }
}
