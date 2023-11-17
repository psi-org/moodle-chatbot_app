using Microsoft.Extensions.Configuration;
using MoodleBot.Common.CommonEntity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MoodleBot.Persistent.Entity
{
    public class MoodleGetNextQuestionDetail
    {
        [JsonProperty("activitytitle")]
        public string ActivityTitle { get; set; }

        [JsonProperty("activityimg")]
        public string ActivityImage { get; set; }

        [JsonProperty("activityattempt")]
        public long? ActivityAttempt { get; set; }

        [JsonProperty("quizattemptsid")]
        public long? QuizAttemptsId { get; set; }

        [JsonProperty("currentpage")]
        public int? CurrentPage { get; set; }

        [JsonProperty("questionname")]
        public string QuestionName { get; set; }

        [JsonProperty("questiontext")]
        public string QuestionText { get; set; }

        [JsonProperty("questiontype")]
        public string QuestionType { get; set; }

        [JsonProperty("showanswerstatus")]
        public bool ShowAnswerStatus;

        [JsonProperty("showanswerfeedback")]
        public bool ShowAnswerFeedback;
        
        [JsonProperty("questionattemptid")]
        public long? QuestionAttemptId { get; set; }

        [JsonProperty("answers")]
        public List<AnswerDetail> Answers { get; set; }
    }

    public class AnswerDetail
    {
        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("answerid")]
        public long? AnswerId { get; set; }

        [JsonProperty("iscorrectanswer")]
        public int? IsCorrectAnswer { get; set; }

        [JsonProperty("feedback")]
        public string Feedback { get; set; }
    }

    public class MoodleGetNextQuestionRequest : MoodleAPIBaseParameter
    {
        public MoodleGetNextQuestionRequest(IConfiguration configuration) : base(configuration)
        {

        }

        [JsonProperty("quiz")]
        public long Quiz { get; set; }
    }
}
