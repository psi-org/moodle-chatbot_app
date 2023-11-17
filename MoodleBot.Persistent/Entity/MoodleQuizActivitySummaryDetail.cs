using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;
using System;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleQuizActivitySummaryDetail
    {
        [JsonProperty("courseid")]
        public int CourseId;

        [JsonProperty("courseactivityid")]
        public int CourseActivityId;

        [JsonProperty("quizid")]
        public int QuizId;

        [JsonProperty("quizattemptid")]
        public int QuizAttemptId;

        [JsonProperty("userid")]
        public int UserId;

        [JsonProperty("attempt")]
        public int Attempt;

        [JsonProperty("uniqueid")]
        public int UniqueId;

        [JsonProperty("currentpage")]
        public int CurrentPage;

        [JsonProperty("attemptstate")]
        public string AttemptState;

        [JsonProperty("activitygrademax")]
        public double ActivityGradeMax;

        [JsonProperty("attemptfinalgrade")]
        public double AttemptFinalGrade;

        [JsonProperty("attemptpercentagegrade")]
        public double AttemptPercentageGrade;

        [JsonProperty("timestarted")]
        public DateTime TimeStarted;

        [JsonProperty("timefinished")]
        public DateTime? TimeFinished;

        [JsonProperty("timetakeninminutes")]
        public int? TimeTakenInMinutes;
    }

    public class QuizActivitySummaryRequest : MoodleAPIBaseParameter
    {
        public QuizActivitySummaryRequest(IConfiguration configuration) : base(configuration) { }

        [JsonProperty("quizattemptid")]
        public long QuizAttemptId { get; set; }
    }
}
